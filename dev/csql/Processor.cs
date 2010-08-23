using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Globalization;
using System.Threading;

namespace csql
{
	/// <summary>
	/// Controls the execution of the script processing.
	/// </summary>
	public class Processor : IDisposable
	{
		private readonly CSqlOptions m_options;
		private readonly IBatchProcessor m_processor;
		private int m_errorCount;
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
		/// Default constructor
		/// </summary>
		public Processor( CSqlOptions csqlOptions )
		{
			this.m_options = csqlOptions;
			this.m_processor = BatchProcessorFactory.CreateProcessor( csqlOptions );
		}


		/// <summary>
		/// Check if the preprocessor encountered an error.
		/// </summary>
		private bool PreprocessorExitedWithError
		{
			get
			{
				return m_ppProcess != null && m_ppProcess.HasExited && m_ppProcess.ExitCode != 0;
			}
		}

		/// <summary>
		/// Called before the first batch is processed.
		/// </summary>
		public void SignIn()
		{
			m_processor.SignIn();
		}

		/// <summary>
		/// Called after the last batch was processed.
		/// </summary>
		public void SignOut()
		{
			try {
				if ( m_ppProcess != null ) {
					if ( !m_ppProcess.HasExited )
						Thread.Sleep( 100 );

					if ( PreprocessorExitedWithError )
						++m_errorCount;
				}
			}
			catch ( InvalidOperationException ) {
				// Ignore any invalid operation when trying to determine the preprocessor exit code.
			}


			if ( GlobalSettings.Verbosity.TraceInfo ) {
				string message = "\r\n*** Finished ";
				switch ( m_errorCount ) {
					case 0:
						message += "without any error :-)";
						break;
					case 1:
						message += "with one error";
						if ( m_options.BreakOnError ) {
							message += " that caused the script execution to stop";
						}
						message += " :-(";
						break;
					default:
						message += "with " + m_errorCount + " errors :-(";
						break;
				}

				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceInfo, message );
			}

			m_processor.SignOut();
			Trace.Flush();
		}

		/// <summary>
		/// Preprocess the input file and 
		/// </summary>
		[SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Want to catch everything to be able to add file and line number infos." )]
		public virtual void Process()
		{
			ProcessorContext processorContext = new ProcessorContext( m_options.ScriptFile );
			try {
				using ( Stream stream = OpenInputFile() )
				using ( StreamReader reader = new StreamReader( stream, Encoding.Default, true ) ) {
					while ( !reader.EndOfStream && !PreprocessorExitedWithError ) {
						string line = reader.ReadLine();
						LineType type = GetLineType( line );
						switch ( type ) {
							case LineType.Text:
								ProcessText( processorContext, line );
								break;
							case LineType.Line:
								ProcessLine( processorContext, line );
								break;
							case LineType.Exec:
								string batch = processorContext.CurrentBatch;
								if ( !IsWhiteSpaceOnly( batch ) ) {
									ProcessBatch( processorContext, batch );
									processorContext.IncrementBatchNumber();
								}
								processorContext.IncrementLineNumber();
								processorContext.StartNextBatch();
								break;
							default:
								throw new NotSupportedException( "Unexepected line type: " + type );
						}
					}
				}
				string lastBatch = processorContext.CurrentBatch;
				if ( !IsWhiteSpaceOnly( lastBatch ) ) {
					ProcessBatch( processorContext, lastBatch );
					processorContext.IncrementBatchNumber();
				}
			}
			catch ( TerminateException ) {
				++m_errorCount;
				throw;
			}
			catch ( Exception ex ) {
				string message = String.Format( "{0}({1}): Error: {2}", processorContext.CurrentFile, processorContext.CurrentBatchLineNo, ex.Message );
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, message );
				string lastBatch = processorContext.CurrentBatch;
				if ( !String.IsNullOrEmpty( lastBatch ) && GlobalSettings.Verbosity.TraceInfo ) {
					Trace.Indent();
					Trace.WriteLine( lastBatch );
					Trace.Unindent();
				}
				throw new TerminateException( ExitCode.SqlCommandError );
			}
		}



		/// <summary>
		/// Indicates if Dispose has already been called
		/// </summary>
		private bool isDisposed;

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
		private void Dispose( bool isDisposing )
		{
			if ( !isDisposed ) {
				if ( isDisposing ) {
					if ( m_processor != null )
						m_processor.Dispose();

					if ( m_ppProcess != null )
						m_ppProcess.Dispose();
				}
				// TODO: cleanup unmanaged objects.
				isDisposed = true;
			}
		}



		/// <summary>
		/// Process a line that appends to the current command batch.
		/// </summary>
		/// <param name="line">The line.</param>
		private static void ProcessText( ProcessorContext processorContext, string line )
		{
			// As long as the batch is still empty skip any leading empty lines
			// and adjust the start context information instead.
			if ( String.IsNullOrEmpty( line.Trim() ) && String.IsNullOrEmpty( processorContext.CurrentBatch ) ) {
				BatchContext startContext = processorContext.FirstBatchContext;
				startContext.LineNumber = processorContext.CurrentFileLineNumber;
				processorContext.IncrementLineNumber();
				return;
			}
			else {
				processorContext.AppendLine( line );
			}
		}

		/// <summary>
		/// Examine the #line directive and adjusts corresponing
		/// internal variables.
		/// </summary>
		/// <param name="line">The #line directive.</param>
		private static void ProcessLine( ProcessorContext processorContext, string line )
		{
			string[] parts = line.Split( ' ' );
			int currentFileLineNo = int.Parse( parts[1], CultureInfo.InvariantCulture );
			string currentFile;

			// The pre processor may omit the file name if it hasn't changed 
			// since the last #line directive.
			if ( parts.Length > 2 ) {
				currentFile = parts[2];
				for ( int i = 3; i < parts.Length; ++i ) {
					currentFile += ' ';
					currentFile += parts[i];
				}
				if ( currentFile.Length > 0 ) {
					char firstChar = currentFile[0];
					if ( firstChar == '\'' || firstChar == '"' )
						currentFile = currentFile.Substring( 1 );
				}
				if ( currentFile.Length > 0 ) {
					char lastChar = currentFile[currentFile.Length - 1];
					if ( lastChar == '\'' || lastChar == '"' )
						currentFile = currentFile.Substring( 0, currentFile.Length - 1 );
				}
			}
			else {
				currentFile = String.Empty;
			}

			processorContext.SetNewBatchContext( currentFile, currentFileLineNo );
		}



		/// <summary>
		/// Emits the current batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		private void ProcessBatch( ProcessorContext processorContext, string batch )
		{
			m_processor.ProcessProgress( processorContext, "Executing batch " + processorContext.CurrentBatchNo );
			Debug.WriteLineIf( GlobalSettings.Verbosity.TraceVerbose, batch );
			try {
				m_processor.ProcessBatch( processorContext, batch );
			}
			catch ( Sqt.DbcProvider.DbException ex ) {
				++m_errorCount;
				string message = FormatError( processorContext, TraceLevel.Error, ex.Message, ex.LineNumber );
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, message );
				if ( m_options.BreakOnError ) {
					throw new TerminateException( ExitCode.SqlCommandError );
				}
			}
			catch ( System.Data.SqlClient.SqlException ex ) {
				++m_errorCount;
				string message = FormatError( processorContext, TraceLevel.Error, ex.Message, ex.LineNumber );
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, message );
				if ( m_options.BreakOnError ) {
					throw new TerminateException( ExitCode.SqlCommandError );
				}
			}
			catch ( System.Data.Common.DbException ex ) {
				++m_errorCount;
				string message = FormatError( processorContext, TraceLevel.Error, ex.Message, 0 );
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, message );
				if ( m_options.BreakOnError ) {
					throw new TerminateException( ExitCode.SqlCommandError );
				}
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


		/// <summary>
		/// Opens the input file.
		/// </summary>
		/// <remarks>
		/// If preprocessor usage is switched off by the command line arguments
		/// the method will just return a read only stream for the input file.
		/// Otherwise the method returns a stream to the preprocessor output.
		/// </remarks>
		/// <param name="inputFile">The path of the input file.</param>
		/// <returns>The open stream for reading</returns>
		private Stream OpenInputFile()
		{
			// Always try to open the file even if preprocessor is used to check if it is accessible.
			Stream stream = new FileStream( m_options.ScriptFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan );
			if ( m_options.UsePreprocessor ) {
				stream.Dispose();
				stream = Preprocess();
			}
			return stream;
		}

		/// <summary>
		/// Preprocesses the specified input file.
		/// </summary>
		/// <param name="inputFile">The path of the input file.</param>
		/// <returns>The path of the file created by the preprocessor.</returns>
		private Stream Preprocess()
		{
			PreProcessor preProcessor = new PreProcessor( m_options );
			string ppCommand   = PreProcessor.Command;
			string ppArguments = preProcessor.Arguments;
			string traceMessage = String.Format( "Executing preprocessor with following command line:\r\n{0} {1}", ppCommand, ppArguments );
			NamedPipe pipe = null;
			Trace.WriteLineIf( GlobalSettings.Verbosity.TraceVerbose, traceMessage );
			ProcessStartInfo ppStartInfo = new ProcessStartInfo( ppCommand, ppArguments );
			ppStartInfo.CreateNoWindow = true;
			ppStartInfo.UseShellExecute = false;
			ppStartInfo.RedirectStandardOutput = true;
			ppStartInfo.RedirectStandardError = true;

			try {
				if ( m_options.UseNamedPipes ) {
					pipe = new NamedPipe( preProcessor.NamedPipeName );
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

					stream = new FileStream( m_options.TempFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan );
				}
				else {
					if ( !m_ppProcess.HasExited ) {
						stream = pipe.Open();
					}
					else {
						throw new TerminateException( ExitCode.PreprocessorError );
					}
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
		internal static string FormatError( ProcessorContext processorContext, TraceLevel severity, string message, int errorLineNumber )
		{
			string error = processorContext.FormatError( severity, message, errorLineNumber );
			return error;
		}
	}
}
