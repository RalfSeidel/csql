using System;
using System.IO;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace csql
{
	class Program
	{
		/// <summary>
		/// The trace level of the program.
		/// </summary>
		public static readonly VerbosityLevel TraceLevel = new VerbosityLevel();

		/// <summary>
		/// The program entry point.
		/// </summary>
		/// <param name="args">The command line arguments.</param>
		/// <returns><see cref="ExitCode.Success"/> if the program ran without an error. 
		/// Another <see cref="ExitCode" /> otherwise.
		/// </returns>
		[SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification="Main has to catch everything." )]
		public static int Main( string[] args )
		{
			Trace.Listeners.Add( new ConsoleTraceListener() );
			CmdArgs cmdArgs = new CmdArgs();
			bool argumentsValid = CommandLine.Parser.ParseArguments( args, cmdArgs );
			bool didTraceCommandLine = false;

			ExitCode exitCode; ;
			if ( !argumentsValid ) {
				if ( TraceLevel.TraceWarning ) {
					TraceCommandLine( args );
					didTraceCommandLine = true;
					CommandLine.Parser.ArgumentsUsage( cmdArgs.GetType() );
				}
				exitCode = ExitCode.ArgumentsError;
			} else {
				TraceLevel.Level = (TraceLevel)cmdArgs.Verbose;
				if ( TraceLevel.TraceVerbose ) {
					TraceCommandLine( args );
					didTraceCommandLine = true;
				}
				try {
					using ( Processor processor = ProcessorFactory.CreateProcessor( cmdArgs ) ) {
						processor.SignIn();

						processor.Process();

						processor.SignOut();
						exitCode = ExitCode.Success;
					}
				}
				catch ( FileNotFoundException ex ) {
					if ( !didTraceCommandLine && TraceLevel.TraceError ) {
						TraceCommandLine( args );
					}
					Trace.WriteLineIf( Program.TraceLevel.TraceError, ex.FileName + ": " + ex.Message );
					exitCode = ExitCode.FileIOError;
				}
				catch ( IOException ex ) {
					if ( !didTraceCommandLine && TraceLevel.TraceError ) {
						TraceCommandLine( args );
					}
					Trace.WriteLineIf( Program.TraceLevel.TraceError, ex.Message );
					exitCode = ExitCode.FileIOError;
				}
				catch ( TerminateException ex ) {
					if ( !didTraceCommandLine && TraceLevel.TraceError ) {
						TraceCommandLine( args );
					}
					exitCode = ex.ExitCode;
				}
				catch ( Exception ex ) {
					if ( !didTraceCommandLine && TraceLevel.TraceError ) {
						TraceCommandLine( args );
					}
					Trace.WriteLineIf( Program.TraceLevel.TraceError, "Unexpected error: " + ex );
					exitCode = ExitCode.UnexpectedError;
				}
				finally {
					Trace.Flush();
				}
			}

			if ( exitCode != ExitCode.Success && System.Diagnostics.Debugger.IsAttached ) {
				Console.Write( "Press any key to continue..." );
				Console.ReadKey();
			}

			return (int)exitCode;
		}

		/// <summary>
		/// Traces the command line of the program.
		/// </summary>
		/// <param name="args">The command line arguments.</param>
		private static void TraceCommandLine( string[] args )
		{
			StringBuilder sb = new StringBuilder();
			Process thisProcess = Process.GetCurrentProcess();
			string thisPath = thisProcess.MainModule.FileName;
			sb.Append( thisPath );
			foreach ( string arg in args ) {
				sb.Append( ' ' );
				sb.Append( arg );
			}
			string traceMessage = sb.ToString();
			Trace.WriteLine( traceMessage );
		}
	}
}
