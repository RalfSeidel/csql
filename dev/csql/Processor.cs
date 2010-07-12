using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;
using System.Globalization;

namespace csql
{
	/// <summary>
	/// Base class for the processors.
	/// </summary>
	/// <remarks>
	/// TODO: this class needs some refactoring to separate raw command execution
	/// and runtime environment management.
	/// </remarks>
	public abstract class Processor : IDisposable
	{
		private readonly CSqlOptions m_options;
		private string m_pipeName;
		private string m_currentFile;
		private int m_currentFileLineNo;
		private int m_currentBatchNo;
		private int m_currentBatchLineNo;
		private int m_errorCount;
		private IList<BatchContext> m_currentBatchContexts;
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
		protected Processor( CSqlOptions csqlOptions )
		{
			this.m_options = csqlOptions;
		}

		/// <summary>
		/// Get the command line arguments.
		/// </summary>
		/// <value>
		/// The command line arguments.
		/// </value>
		protected CSqlOptions Options
		{
			get { return m_options; }
		}


		/// <summary>
		/// Gets the path of the current file processed.
		/// </summary>
		/// <value>The current file path.</value>
		public string CurrentFile
		{
			get { return this.m_currentFile; }
		}


		/// <summary>
		/// Gets the line offset of the current batch to the start of the input file.
		/// </summary>
		/// <value>The current batch line no.</value>
		public int CurrentBatchLineOffset
		{
			get { return this.m_currentBatchLineNo; }
		}

		/// <summary>
		/// Gets the first batch context i.e. the starting context of the current batch processed.
		/// </summary>
		/// <value>The starting batch context.</value>
		private BatchContext FirstBatchContext
		{
			get { return m_currentBatchContexts[0]; }
		}

		/// <summary>
		/// Flag indicating if a named pipe is used to communicate with the pre processor.
		/// </summary>
		/// <value>Flag for the named pipe usage option.</value>
		private bool UseNamedPipes
		{
			get { return String.IsNullOrEmpty( m_options.TempFile ); }
		}

		/// <summary>
		/// Gets the name of the named pipe.
		/// </summary>
		/// <value>The name of the named pipe.</value>
		private string NamedPipeName
		{
			get
			{
				Debug.Assert( UseNamedPipes );
				if ( m_pipeName == null ) {
					int currentProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
					m_pipeName = "de.sqlservice.sqtpp\\" + currentProcessId.ToString( CultureInfo.InvariantCulture );
				}
				return m_pipeName;
			}
		}

		/// <summary>
		/// Gets the name of the temp file of the preprocessor output.
		/// </summary>
		/// <value>The name of the temp file.</value>
		private string TempFileName
		{
			get
			{
				Debug.Assert( !UseNamedPipes, "Usage of the temp file name is not valid if name pipes are used for the pre processor output." );
				return m_options.TempFile;
			}
		}

		/// <summary>
		/// Gets the path of the preprocessor sqtpp.
		/// </summary>
		/// <remarks>
		/// Currently the preprocessor is expected to be found in the same directory as 
		/// csql itself. 
		/// </remarks>
		/// <value>
		/// The preprocessor path which is the directory of the csql executable combined 
		/// with the file name of the preprocessor executable.
		/// </value>
		protected static string PreprocessorPath
		{
			get
			{
				Assembly assembly = Assembly.GetExecutingAssembly();

				string thisPath = assembly.Location;
				string root = Path.GetPathRoot( thisPath );
				string folder = Path.GetDirectoryName( thisPath );
				string sqtppPath = Path.Combine( Path.Combine( root, folder ), "sqtpp.exe" );

				return sqtppPath;
			}
		}

		/// <summary>
		/// Gets the arguemtns for the preprocessor sqtpp.
		/// </summary>
		/// <value>The preprocessor arguemtns.</value>
		protected string PreprocessorArguments
		{
			get
			{
				SqtppOptions ppOption = m_options.PreprocessorOptions;
				ppOption.InputFile = m_options.ScriptFile;
				ppOption.OutputFile = UseNamedPipes ? NamedPipe.GetPipePath( NamedPipeName ) : TempFileName;
				string ppArguments = ppOption.CommandLineArguments;

				return ppArguments;
			}
		}


		/// <summary>
		/// Emits an entry message.
		/// </summary>
		public virtual void SignIn()
		{
			if ( GlobalSettings.Verbosity.TraceInfo && !m_options.NoLogo ) {
				StringBuilder sb = new StringBuilder();
				Assembly assembly = Assembly.GetExecutingAssembly();
				Version version = assembly.GetName().Version;
				string name = GlobalSettings.CSqlProductName;
				sb.Append( name );
				sb.Append( " " );
				if ( GlobalSettings.Verbosity.TraceVerbose ) {
					sb.Append( version.ToString() );
				} else {
					sb.Append( version.ToString( 2 ) );
				}
				sb.Append( " (c) SQL Service GmbH" );
				sb.Append( " - Processing " ).Append( m_options.ScriptFile );

				string message = sb.ToString();
				Trace.WriteLine( message );
			}
		}

		/// <summary>
		/// Emits the an exit/finished message.
		/// </summary>
		public virtual void SignOut()
		{
			if ( !GlobalSettings.Verbosity.TraceInfo )
				return;

			string message = "\r\n*** Finished ";
			switch ( m_errorCount ) {
				case 0:
					message+= "without any error :-)";
					break;
				case 1:
					message+= "with one error";
					if ( m_options.BreakOnError ) {
						message+= " that caused the script execution to stop";
					}
					message+= " :-(";
					break;
				default:
					message+= "with " + m_errorCount + " errors :-(";
					break;
			}


			Trace.WriteLineIf( GlobalSettings.Verbosity.TraceInfo, message );
		}


		/// <summary>
		/// Preprocess the input file and 
		/// </summary>
		[SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification="Want to catch everything to be able to add file and line number infos." )]
		public virtual void Process()
		{
			m_currentFile = m_options.ScriptFile;
			m_currentFileLineNo = 1;
			m_currentBatchNo = 1;
			m_currentBatchLineNo = 1;
			m_batchBuilder = new StringBuilder( 4096 );
			m_currentBatchContexts   = new List<BatchContext>();
			ResetBatchContext();
			try {
				using ( Stream stream = OpenInputFile() )
				using ( StreamReader reader = new StreamReader( stream, Encoding.Default, true ) ) {
					while ( !reader.EndOfStream ) {
						string line = reader.ReadLine();
						LineType type = GetLineType( line );
						switch ( type ) {
							case LineType.Text:
								ProcessText( line );
								break;
							case LineType.Line:
								ProcessLine( line );
								break;
							case LineType.Exec:
								string batch = m_batchBuilder.ToString();
								if ( !IsWhiteSpaceOnly( batch ) ) {
									CheckedProcessBatch( batch );
									m_currentBatchNo++;
								}
								m_batchBuilder.Length = 0;
								m_currentBatchLineNo = 1;
								m_currentFileLineNo++;
								ResetBatchContext();
								break;
							default:
								throw new NotSupportedException( "Unexepected line type: " + type );
						}
					}
				}
				string lastBatch = m_batchBuilder.ToString();
				if ( !IsWhiteSpaceOnly( lastBatch ) ) {
					CheckedProcessBatch( lastBatch );
					m_currentBatchNo++;
				}
			}
			catch ( TerminateException ) {
				throw;
			}
			catch ( Exception ex ) {
				string message = String.Format( "{0}({1}): Error: {2}", m_currentFile, m_currentBatchLineNo, ex.Message );
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, message );
				if ( m_batchBuilder.Length != 0 && GlobalSettings.Verbosity.TraceInfo ) {
					Trace.Indent();
					Trace.WriteLine( m_batchBuilder.ToString() );
					Trace.Unindent();
				}
				throw new TerminateException( ExitCode.SqlCommandError );
			}
		}



		/// <summary>
		/// Emits the current batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		private void CheckedProcessBatch( string batch )
		{
			ProcessProgress( "Executing batch " + m_currentBatchNo );
			Debug.WriteLineIf( GlobalSettings.Verbosity.TraceVerbose, batch );
			try {
				ProcessBatch( batch );
			}
			catch ( DbException ex ) {
				++m_errorCount;
				string message = FormatError( TraceLevel.Error, ex.Message, ex.LineNumber );
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, message );
				if ( m_options.BreakOnError ) {
					throw new TerminateException( ExitCode.SqlCommandError );
				}
			}
			catch ( System.Data.SqlClient.SqlException ex ) {
				++m_errorCount;
				string message = FormatError( TraceLevel.Error, ex.Message, ex.LineNumber );
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, message );
				if ( m_options.BreakOnError ) {
					throw new TerminateException( ExitCode.SqlCommandError );
				}
			}
			catch ( System.Data.Odbc.OdbcException ex ) {
				++m_errorCount;
				string message = FormatError( TraceLevel.Error, ex.Message, 0 );
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, message );
				if ( m_options.BreakOnError ) {
					throw new TerminateException( ExitCode.SqlCommandError );
				}
			}
			catch ( System.Data.OleDb.OleDbException ex ) {
				++m_errorCount;
				string message = FormatError( TraceLevel.Error, ex.Message, 0 );
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, message );
				if ( m_options.BreakOnError ) {
					throw new TerminateException( ExitCode.SqlCommandError );
				}
			}
			catch ( System.Data.Common.DbException ex ) {
				++m_errorCount;
				string message = FormatError( TraceLevel.Error, ex.Message, 0 );
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, message );
				if ( m_options.BreakOnError ) {
					throw new TerminateException( ExitCode.SqlCommandError );
				}
			}
		}

		/// <summary>
		/// Process a line that appends to the current command batch.
		/// </summary>
		/// <param name="line">The line.</param>
		private void ProcessText( string line )
		{
			// As long as the batch is still empty skip any leading empty lines
			// and adjust the start context information instead.
			if ( String.IsNullOrEmpty( line.Trim() ) && m_batchBuilder.Length == 0 ) {
				BatchContext startContext = FirstBatchContext;
				startContext.LineNumber = ++m_currentFileLineNo;
				return;
			} else {
				m_batchBuilder.AppendLine( line );
				m_currentBatchLineNo++;
				m_currentFileLineNo++;
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
			this.m_currentFileLineNo = int.Parse( parts[1], CultureInfo.InvariantCulture );

			// The pre processor may omit the file name if it hasn't changed 
			// since the last #line directive.
			if ( parts.Length > 2 ) {
				this.m_currentFile = parts[2];
				for ( int i = 3; i < parts.Length; ++i ) {
					this.m_currentFile += ' ';
					this.m_currentFile += parts[i];
				}
				if ( m_currentFile.Length > 0 ) {
					char firstChar = m_currentFile[0];
					if ( firstChar == '\'' || firstChar == '"' )
						m_currentFile = m_currentFile.Substring( 1 );
				}
				if ( m_currentFile.Length > 0 ) {
					char lastChar = m_currentFile[m_currentFile.Length - 1];
					if ( lastChar == '\'' || lastChar == '"' )
						m_currentFile = m_currentFile.Substring( 0, m_currentFile.Length - 1 );
				}
			}

			if ( m_batchBuilder.Length == 0 ) {
				BatchContext startContext = FirstBatchContext;
				startContext.File = m_currentFile;
				startContext.LineNumber = m_currentFileLineNo;
			} else {
				BatchContext newContext = new BatchContext( m_currentBatchLineNo, m_currentFile, m_currentFileLineNo );
				m_currentBatchContexts.Add( newContext );
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
			if ( m_options.UsePreprocessor ) {
				return Preprocess();
			} else {
				return new FileStream( m_options.ScriptFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan );
			}
		}

		/// <summary>
		/// Preprocesses the specified input file.
		/// </summary>
		/// <param name="inputFile">The path of the input file.</param>
		/// <returns>The path of the file created by the preprocessor.</returns>
		private Stream Preprocess()
		{
			string ppCommand   = PreprocessorPath;
			string ppArguments = PreprocessorArguments;
			string traceMessage = String.Format( "Executing preprocessor with following command line:\r\n{0} {1}", ppCommand, ppArguments );
			NamedPipe pipe = null;
			Trace.WriteLineIf( GlobalSettings.Verbosity.TraceVerbose, traceMessage );
			ProcessStartInfo ppStartInfo = new ProcessStartInfo( ppCommand, ppArguments );
			ppStartInfo.CreateNoWindow = true;
			ppStartInfo.UseShellExecute = false;
			ppStartInfo.RedirectStandardOutput = true;
			ppStartInfo.RedirectStandardError = true;

			try {
				if ( UseNamedPipes ) {
					pipe = new NamedPipe( NamedPipeName );
				}

				m_ppProcess = new Process();
				m_ppProcess.StartInfo = ppStartInfo;
				m_ppProcess.OutputDataReceived += PreProcessor_OutputDataReceived;
				m_ppProcess.ErrorDataReceived += PreProcessor_ErrorDataReceived;
				m_ppProcess.Start();
				m_ppProcess.BeginErrorReadLine();
				m_ppProcess.BeginOutputReadLine();

				Stream stream;
				if ( pipe == null ) {
					m_ppProcess.WaitForExit();

					if ( m_ppProcess.ExitCode != 0 ) {
						throw new TerminateException( ExitCode.PreprocessorError );
					}

					stream = new FileStream( TempFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan );
				} else if ( !m_ppProcess.HasExited  ) {
					stream = pipe.Open();
				} else {
					stream = null;
				}


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
					// Detach the event handler before disposing. If they are not connected
					// and the program execution stoped because of an exception the pre 
					// processor may send an error message when the named piped is 
					// destroyed. 
					m_ppProcess.OutputDataReceived -= PreProcessor_OutputDataReceived;
					m_ppProcess.ErrorDataReceived -= PreProcessor_ErrorDataReceived;
					m_ppProcess.Dispose();
					m_ppProcess = null;
				}
				string message = ppCommand + " " + ppArguments + ":\r\n" + ex.Message;
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, message );
				throw new TerminateException( ExitCode.ArgumentsError );
			}
		}

		/// <summary>
		/// Handles the OutputDataReceived event of the pre processor std output stream.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event data.</param>
		private void PreProcessor_OutputDataReceived( object sender, DataReceivedEventArgs e )
		{
			if ( e.Data != null ) {
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceInfo, e.Data );
			}
		}

		/// <summary>
		/// Handles the ErrorDataReceived event of of the pre processor std error output stream.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event data.</param>
		private void PreProcessor_ErrorDataReceived( object sender, DataReceivedEventArgs e )
		{
			if ( e.Data != null ) {
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceWarning, e.Data );
			}
		}

		/// <summary>
		/// Clear the batch context.
		/// </summary>
		private void ResetBatchContext()
		{
			m_currentBatchContexts.Clear();
			if ( !String.IsNullOrEmpty( m_currentFile ) ) {
				BatchContext initialContext = new BatchContext( m_currentBatchLineNo, m_currentFile, m_currentFileLineNo );
				m_currentBatchContexts.Add( initialContext );
			}
		}


		/// <summary>
		/// Formats the error message for the output.
		/// </summary>
		/// <param name="severity">
		/// The severity of the (error) message.
		/// </param>
		/// <param name="message">
		/// The error message.
		/// </param>
		/// <param name="errorLineNumber">
		/// The line number where error was reported.
		/// </param>
		/// <returns>The formated message.</returns>
		protected string FormatError( TraceLevel severity, string message, int errorLineNumber )
		{
			int contextCount = m_currentBatchContexts.Count;
			BatchContext context = null;
			for ( int i = contextCount - 1; i >= 0; --i ) {
				context = m_currentBatchContexts[i];
				if ( context.BatchOffset <= errorLineNumber )
					break;
			}
			Debug.Assert( context != null );

			if ( context == null ) {
				string error = String.Format( "{0}({1}): {2}: {3}", m_currentFile, errorLineNumber, severity, message );
				return error;
			} else {
				string contextFile = context.File;
				int contextLineNumber = context.LineNumber + errorLineNumber - context.BatchOffset;
				string error = String.Format( "{0}({1}): {2}: {3}", contextFile, contextLineNumber, severity, message );
				return error;
			}
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
			if ( line.StartsWith( "#line" ) ) {
				string[] parts = line.Split( ' ' );
				if ( parts.Length >= 3 )
					return LineType.Line;
			}
			return LineType.Text;
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
		private static bool IsWhiteSpaceOnly( string line )
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
