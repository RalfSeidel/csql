using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace csql
{
	/// <summary>
	/// Base class for the processors.
	/// </summary>
	public abstract class Processor : IDisposable	
	{
		private readonly CmdArgs m_cmdArgs;
		private string m_pipeName;
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

        /// <summary>
        /// Get the command line arguments.
        /// </summary>
        /// <value>
        /// The command line arguments.
        /// </value>
        protected CmdArgs CmdArgs
        {
            get { return m_cmdArgs; }
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
        /// Gets the line number in the current batch.
        /// </summary>
        /// <value>The current batch line no.</value>
		public int CurrentBatchLineNo
		{
			get { return this.m_currentBatchLineNo; }
		}

		/// <summary>
		/// Flag indicating if a named pipe is used to communicate with the pre processor.
		/// </summary>
		/// <value>Flag for the named pipe usage option.</value>
        private bool UseNamedPipes
        {
            get { return String.IsNullOrEmpty( m_cmdArgs.TempFile ); }
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
                    m_pipeName = "de.sqlservice.sqtpp\\" + currentProcessId.ToString();
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
                return m_cmdArgs.TempFile;
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
				Process thisProcess = System.Diagnostics.Process.GetCurrentProcess();
				string thisPath = thisProcess.MainModule.FileName;
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
                string ppArguments = m_cmdArgs.PreprocessorArgs;
                string ppInputFile = m_cmdArgs.ScriptFile;
                string ppOutFile   = UseNamedPipes ? NamedPipe.GetPipePath( NamedPipeName ) : TempFileName;

                ppArguments += m_cmdArgs.PreprocessorDefines;
                ppArguments += " -o\"" + ppOutFile + "\"";
                ppArguments += " " + ppInputFile;
                return ppArguments;
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
                //return !UseNamedPipes && !String.IsNullOrEmpty( TempFileName );
                return false;
			}
		}


		/// <summary>
		/// Emits an entry message.
		/// </summary>
		public virtual void SignIn()
		{
			if ( Program.TraceLevel.TraceInfo ) {
				StringBuilder sb = new StringBuilder();
				Assembly assembly = Assembly.GetEntryAssembly();
				string name = assembly.GetName().Name;
				Version version = assembly.GetName().Version;
				sb.Append( name );
				sb.Append( " " );
				if ( Program.TraceLevel.TraceVerbose ) {
					sb.Append( version.ToString() );
				} else {
					sb.Append( version.ToString(2) );
				}
				sb.Append( " (c) SQL Service GmbH" );

				string message = sb.ToString();
				Trace.WriteLine( message );
			}
		}

		/// <summary>
		/// Emits the an exit/finished message.
		/// </summary>
		public virtual void SignOut()
		{
			Trace.WriteLineIf( Program.TraceLevel.TraceInfo, "** Finished" );
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
				using ( StreamReader reader = new StreamReader( stream, Encoding.Default, true ) ) {
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
								if ( !IsWhiteSpaceOnly( batch ) ) {
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
				if ( !IsWhiteSpaceOnly( lastBatch ) ) {
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
						Debug.WriteLineIf( Program.TraceLevel.TraceVerbose, "deleting file " + TempFileName );
                        File.Delete( TempFileName );
					}
					catch ( IOException ex ) {
                        Debug.WriteLineIf( Program.TraceLevel.TraceError, "Error deleting file " + TempFileName + ":\r\n" + ex.Message );
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
			ProcessProgress( "Executing batch " + m_currentBatchNo );
			Debug.WriteLineIf( Program.TraceLevel.TraceVerbose, batch );
			try {
				ProcessBatch( batch );
			}
			catch ( csql.DbException ex ) {
				string message = FormatError( ex.Message, ex.LineNumber );
				Trace.WriteLineIf( Program.TraceLevel.TraceError, message );
				if ( m_cmdArgs.BreakOnError ) {
					throw new TerminateException( ExitCode.SqlCommandError );
				}
			}
			catch ( System.Data.SqlClient.SqlException ex ) {
				string message = FormatError( ex.Message, ex.LineNumber );
				Trace.WriteLineIf( Program.TraceLevel.TraceError, message );
				if ( m_cmdArgs.BreakOnError ) {
					throw new TerminateException( ExitCode.SqlCommandError );
				}
			}
			catch ( System.Data.Odbc.OdbcException ex ) {
				string message = FormatError( ex.Message, 0 );
				Trace.WriteLineIf( Program.TraceLevel.TraceError, message );
				if ( m_cmdArgs.BreakOnError ) {
					throw new TerminateException( ExitCode.SqlCommandError );
				}
			}
			catch ( System.Data.OleDb.OleDbException ex ) {
				string message = FormatError( ex.Message, 0 );
				Trace.WriteLineIf( Program.TraceLevel.TraceError, message );
				if ( m_cmdArgs.BreakOnError ) {
					throw new TerminateException( ExitCode.SqlCommandError );
				}
			}
			catch ( System.Data.Common.DbException ex ) {
				string message = FormatError( ex.Message, 0 );
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
			string ppCommand   = PreprocessorPath;
			string ppArguments = PreprocessorArguments;
			string traceMessage = String.Format( "Executing preprocessor with following command line:\r\n{0} {1}", ppCommand, ppArguments );
            NamedPipe pipe = null;
			Trace.WriteLineIf( Program.TraceLevel.TraceVerbose, traceMessage );
			ProcessStartInfo ppStartInfo = new ProcessStartInfo( ppCommand, ppArguments );
			ppStartInfo.UseShellExecute = false;
			ppStartInfo.RedirectStandardOutput = true;
			ppStartInfo.RedirectStandardError = true;

			try {
                if ( UseNamedPipes ) {
                    pipe = new NamedPipe( NamedPipeName );
                }

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

				Stream stream;
                if ( pipe == null ) {
                    m_ppProcess.WaitForExit();

                    if ( m_ppProcess.ExitCode != 0 ) {
                        throw new TerminateException( ExitCode.PreprocessorError );
                    }

                    stream = new FileStream( TempFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan );
                } else {
                    stream = pipe.Open();
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
			if ( lineNumber < 0 )
				lineNumber = 0;

			lineNumber += CurrentBatchLineNo;
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
