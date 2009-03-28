using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace csql
{
	/// <summary>
	/// Base class for the processors.
	/// </summary>
	public abstract class Processor : IDisposable	
	{
		private readonly CmdArgs m_cmdArgs;
		private string m_tempFileName;
		private string m_currentFile;
		private int    m_currentLineNo;
		private int    m_currentBatchNo;
		private int    m_currentBatchLineNo;
		private StringBuilder m_batchBuilder;
		private Process m_ppProcess;

		/// <summary>
		/// Classification of the current line just read.
		/// </summary>
		private enum LineType
		{
			/// <summary>
			/// Simple text.
			/// </summary>
			Text,
			/// <summary>
			/// A preprocessor #line directive.
			/// </summary>
			Line,
			/// <summary>
			/// The batch exec command ("go").
			/// </summary>
			Exec
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Processor"/> class.
		/// </summary>
		/// <param name="cmdArgs">The parsed command line arguments.</param>
		protected Processor( CmdArgs cmdArgs )
		{
			m_cmdArgs = cmdArgs;
		}


		public string CurrentFile
		{
			get { return this.m_currentFile; }
		}

		public int CurrentBatchLineNo
		{
			get { return this.m_currentBatchLineNo; }
		}

		/// <summary>
		/// Gets the name of the temp file.
		/// </summary>
		/// <value>The name of the temp file.</value>
		private string TempFileName
		{
			get
			{
				if ( !String.IsNullOrEmpty( m_cmdArgs.TempFile ) )
					return m_cmdArgs.TempFile;

				if ( m_tempFileName == null ) {
					m_tempFileName = Path.GetTempFileName();
				}
				return m_tempFileName;
			}
		}

		/// <summary>
		/// Gets the path of the preprocessor sqtpp.
		/// </summary>
		/// <value>The preprocessor path.</value>
		private static string PreprocessorPath
		{
			get
			{
				Process thisProcess = System.Diagnostics.Process.GetCurrentProcess();
				string thisPath = thisProcess.MainModule.FileName;
				string root = Path.GetPathRoot( thisPath );
				string folder = Path.GetDirectoryName( thisPath );
				string sqtppPath = Path.Combine( Path.Combine( root, folder ), "sqtpp.exe" );

				return sqtppPath;
			}
		
		}

		/// <summary>
		/// Gets a value indicating whether the temporary output file of the preprocessor
		/// has to be deleted.
		/// </summary>
		/// <value><c>true</c> if the temp file needs to be deleted. <c>false</c> otherwise.</value>
		private bool DeleteTempFile
		{
			get
			{
				return !String.IsNullOrEmpty( m_cmdArgs.TempFile ) && !String.IsNullOrEmpty( m_tempFileName );
			}
			
		}


		/// <summary>
		/// Emits the an intro code.
		/// </summary>
		public virtual void SignIn()
		{
		}

		/// <summary>
		/// Preprocess the input file and 
		/// </summary>
		[SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification="Want to catch everything to be able to add file and line number infos." )]
		public virtual void Process()
		{
			m_currentFile        = m_cmdArgs.ScriptFile;
			m_currentLineNo      = 1;
			m_currentBatchNo     = 1;
			m_currentBatchLineNo = 0;
			m_batchBuilder       = new StringBuilder( 4096 );
			try {
				using ( Stream stream = OpenInputFile() )
				using ( StreamReader reader = new StreamReader( stream, true ) ) {
					while ( !reader.EndOfStream ) {
						string line = reader.ReadLine();
						LineType type = GetLineType( line );
						switch ( type ) {
							case LineType.Text:
								m_batchBuilder.AppendLine( line );
								break;
							case LineType.Line:
								ProcessLine( line );
								break;
							case LineType.Exec:
								string batch = m_batchBuilder.ToString();
								if ( !isWhiteSpaceOnly( batch ) ) {
									ProcessExec( m_batchBuilder.ToString() );
									m_currentBatchNo++;
								}
								m_batchBuilder.Length = 0;
								m_currentBatchLineNo = m_currentLineNo + 1;
								break;
							default:
								throw new NotSupportedException( "Unexepected line type: " + type );
						}
						m_currentLineNo++;
					}
				}
				string lastBatch = m_batchBuilder.ToString();
				if ( !isWhiteSpaceOnly( lastBatch ) ) {
					m_currentBatchNo++;
					ProcessExec( lastBatch );
				}
			}
			catch ( TerminateException ) {
				throw;
			}
			catch ( Exception ex ) {
				string message = String.Format( "{0}({1}): Error: {2}", m_currentFile, m_currentBatchLineNo, ex.Message );
				Trace.WriteLineIf( Program.TraceLevel.TraceError, message );
				if ( m_batchBuilder.Length != 0 && Program.TraceLevel.TraceInfo ) {
					Trace.Indent();
					Trace.WriteLine( m_batchBuilder.ToString() );
					Trace.Unindent();
				}
				throw new TerminateException( ExitCode.SqlCommandError );
			}
			finally {
				if ( DeleteTempFile ) {
					try {
						Debug.WriteLineIf( Program.TraceLevel.TraceVerbose, "deleting file " + m_tempFileName );
						File.Delete( m_tempFileName );
					}
					catch {
					}
				}
			}
		}



		/// <summary>
		/// Emits the current batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		private void ProcessExec( string batch )
		{
			ProcessProgress(" Executing batch " + m_currentBatchNo );
			Debug.WriteLineIf( Program.TraceLevel.TraceVerbose, batch );
			try {
				ProcessBatch( batch );
			}
			catch ( System.Data.SqlClient.SqlException ex ) {
				int    lineNo = CurrentBatchLineNo + (ex.LineNumber > 0 ? ex.LineNumber : 0);
				string message = FormatError( ex.Message, lineNo );
				Trace.WriteLineIf( Program.TraceLevel.TraceError, message );
				if ( m_cmdArgs.BreakOnError ) {
					throw new TerminateException( ExitCode.SqlCommandError );
				}
			}
			catch ( System.Data.Odbc.OdbcException ex ) {
				int    lineNo = CurrentBatchLineNo;
				string message = FormatError( ex.Message, lineNo );
				Trace.WriteLineIf( Program.TraceLevel.TraceError, message );
				if ( m_cmdArgs.BreakOnError ) {
					throw new TerminateException( ExitCode.SqlCommandError );
				}
			}
			catch ( System.Data.OleDb.OleDbException ex ) {
				int    lineNo = CurrentBatchLineNo;
				string message = FormatError( ex.Message, lineNo );
				Trace.WriteLineIf( Program.TraceLevel.TraceError, message );
				if ( m_cmdArgs.BreakOnError ) {
					throw new TerminateException( ExitCode.SqlCommandError );
				}
			}
			catch ( System.Data.Common.DbException ex ) {
				int    lineNo = CurrentBatchLineNo;
				string message = FormatError( ex.Message, lineNo );
				Trace.WriteLineIf( Program.TraceLevel.TraceError, message );
				if ( m_cmdArgs.BreakOnError ) {
					throw new TerminateException( ExitCode.SqlCommandError );
				}
			}
		}

		/// <summary>
		/// Examine the #line directive and adjusts corresponing
		/// internal variables.
		/// </summary>
		/// <param name="line">The #line directive.</param>
		private void ProcessLine( string line )
		{
			string[] parts = line.Split( ' ' );
			this.m_currentLineNo = int.Parse( parts[1] );
			this.m_currentFile = parts[2];

			for ( int i = 3; i < parts.Length; ++i ) {
				this.m_currentFile+= ' ';
				this.m_currentFile+= parts[i];
			}
			if ( m_currentFile.Length > 0 ) {
				char firstChar = m_currentFile[0];
				if ( firstChar == '\'' || firstChar == '"' )
					m_currentFile = m_currentFile.Substring( 1 );
			}
			if ( m_currentFile.Length > 0 ) {
				char lastChar  = m_currentFile[m_currentFile.Length-1];
				if ( lastChar == '\'' || lastChar == '"' )
					m_currentFile = m_currentFile.Substring( 0, m_currentFile.Length - 1 );
			}
		}

		/// <summary>
		/// Processes the current batch.
		/// </summary>
		/// <param name="batch">The current batch.</param>
		protected abstract void ProcessProgress( string progressInfo );

		/// <summary>
		/// Processes the current batch.
		/// </summary>
		/// <param name="batch">The current batch.</param>
		protected abstract void ProcessBatch( string batch );

		/// <summary>
		/// Opens the input file.
		/// </summary>
		/// <remarks>
		/// If preprocessor ussage is switched off by the command line arguments
		/// the method will just return a read only stream for the input file.
		/// Otherwise the method returns a stream to the preprocessor output.
		/// </remarks>
		/// <param name="inputFile">The path of the input file.</param>
		/// <returns>The open stream for reading</returns>
		private Stream OpenInputFile()
		{
			if ( this.m_cmdArgs.UsePreprocessor ) {
				return Preprocess();
			} else {
				return new FileStream( m_cmdArgs.ScriptFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan );
			}
		}

		/// <summary>
		/// Preprocesses the specified input file.
		/// </summary>
		/// <param name="inputFile">The path of the input file.</param>
		/// <returns>The path of the file created by the preprocessor.</returns>
		private Stream Preprocess()
		{
			string ppInputFile = m_cmdArgs.ScriptFile;
			string ppCommand   = PreprocessorPath;
			string ppArguments = m_cmdArgs.PreprocessorArgs;
			string ppOutFile   = TempFileName;
			ppArguments += m_cmdArgs.PreprocessorDefines;
			ppArguments += " -o\"" + ppOutFile + "\"";
			ppArguments += " " + ppInputFile;
			string traceMessage = String.Format( "Executing preprocessor with following command line:\r\n{0} {1}", ppCommand, ppArguments );
			Trace.WriteLineIf( Program.TraceLevel.TraceVerbose, traceMessage );
			ProcessStartInfo ppStartInfo = new ProcessStartInfo( ppCommand, ppArguments );
			ppStartInfo.UseShellExecute = false;
			ppStartInfo.RedirectStandardOutput = true;
			ppStartInfo.RedirectStandardError = true;

			try {
				m_ppProcess = new Process();
				m_ppProcess.StartInfo = ppStartInfo;
				m_ppProcess.OutputDataReceived += delegate( object sender, DataReceivedEventArgs e )
				{
					if ( e.Data != null ) {
						Trace.WriteLineIf( Program.TraceLevel.TraceInfo, e.Data );
					}
				};
				m_ppProcess.ErrorDataReceived += delegate( object sender, DataReceivedEventArgs e )
				{
					if ( e.Data != null ) {
						Trace.WriteLineIf( Program.TraceLevel.TraceWarning, e.Data );
					}
				};
				m_ppProcess.Start();
				m_ppProcess.BeginErrorReadLine();
				m_ppProcess.BeginOutputReadLine();
				// TODO: when switching to named pipes eliminate this line.
				m_ppProcess.WaitForExit();

				if ( m_ppProcess.ExitCode != 0 ) {
					throw new TerminateException( ExitCode.PreprocessorError );
				}

				Stream stream = new FileStream( ppOutFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan );

				return stream;
			}
			catch ( TerminateException ) {
				if ( m_ppProcess != null ) {
					m_ppProcess.Dispose();
					m_ppProcess = null;
				}
				throw;
			}
			catch ( Exception ex ) {
				if ( m_ppProcess != null ) {
					m_ppProcess.Dispose();
					m_ppProcess = null;
				}
				string message = ppCommand + " " + ppArguments + ":\r\n" + ex.Message;
				Trace.WriteLineIf( Program.TraceLevel.TraceError, message );
				throw new TerminateException( ExitCode.ArgumentsError );
			}
		}


		/// <summary>
		/// Formats the error message for the output.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="lineNumber">The current line number.</param>
		/// <returns>The formated message.</returns>
		private string FormatError( string message, int lineNumber )
		{
			string error = String.Format( "{0}({1}): Error: {2}", m_currentFile, lineNumber, message );
			return error;
		}



		/// <summary>
		/// Examine the content of the current line.
		/// </summary>
		/// <param name="line">The line from the input just read.</param>
		/// <returns>The line type.</returns>
		private static LineType GetLineType( string line )
		{
			line = line.Trim();

			if ( line.Length == 0 )
				return LineType.Text;

			if ( String.Equals( line, "go", StringComparison.InvariantCultureIgnoreCase ) )
				return LineType.Exec;

			// check for # line no
			if ( line[0] == '#' ) {
				string[] parts = line.Split( ' ' );
				if ( parts.Length >= 3 )
					return LineType.Line;
			}
			return LineType.Text;
		}

		/// <summary>
		/// Emits script exit code.
		/// </summary>
		public virtual void SignOut()
		{
		}

		/// <summary>
		/// Determines whether the specified line contains white space characters only.
		/// </summary>
		/// <param name="line">The line.</param>
		/// <returns>
		/// <c>true</c> if the specified line is empty  or
		/// contains white space characters only.
		/// <c>false</c> otherwise.
		/// </returns>
		private static bool isWhiteSpaceOnly( string line )
		{
			foreach ( char c in line ) {
				if ( !Char.IsWhiteSpace( c ) )
					return false;
			}
			return true;
		}

		#region IDisposable Members

		
		/// <summary>
		/// Finalizer
		/// </summary>
		~Processor()
		{
			Dispose( false );
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/// <summary>
		/// Cleanup implementation.
		/// </summary>
		protected virtual void Dispose( bool isDisposing )
		{
			if ( isDisposing ) {
				if ( m_ppProcess != null ) {
					m_ppProcess.Dispose();
					m_ppProcess = null;
				}
			}
		}

		#endregion

	}
}
