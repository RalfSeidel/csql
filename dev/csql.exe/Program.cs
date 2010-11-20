using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using CommandLine;
using System.Collections.Generic;

namespace csql.exe
{
	/// <summary>
	/// The program entry point.
	/// </summary>
	/// <param name="args">The command line arguments.</param>
	/// <returns><see cref="ExitCode.Success"/> if the program ran without an error. 
	/// Another <see cref="ExitCode" /> otherwise.
	/// </returns>
	[SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification="Main has to catch everything." )]
	class Program
	{
		/// <summary>
		/// Gets the program title.
		/// </summary>
		private static string ProgramTitle
		{
			get
			{
				Assembly currentAssembly = Assembly.GetExecutingAssembly();
				AssemblyName assemblyName = currentAssembly.GetName();
				return assemblyName.Name;
			}
		}

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
			// If run without any argument show dialog with parameter usage.
			// Prefer dialog over command line to make windows client software 
			// logo toolkit happy.
			if ( args.Length == 0 ) {
				string usage = CommandLine.Parser.ArgumentsUsage( typeof(CmdArgs) );
				var usageDialogItems = GetUsageDialogData();
				UsageDialog dialog = new UsageDialog( usageDialogItems );
				dialog.ShowDialog();
				return (int)ExitCode.Success;
			}

			// Set output encoding to ansi because otherwise the output pane 
			// of visual studio will display wrong characters.
			// TODO: Make this behaviour configurable.
			Console.OutputEncoding = Encoding.Default;
			ConsoleTraceListener traceListener = new ConsoleTraceListener();
			Trace.Listeners.Add( traceListener );

			CmdArgs cmdArgs = new CmdArgs();
			bool argumentsValid = CommandLine.Parser.ParseArguments( args, cmdArgs );
			bool didTraceCommandLine = false;

			VerbositySwitch verbosity = GlobalSettings.Verbosity;
			verbosity.Level = (TraceLevel)(cmdArgs.Verbose);

			ExitCode exitCode; ;
			if ( !argumentsValid ) {
				if ( verbosity.TraceWarning ) {
					TraceCommandLine( args );
					didTraceCommandLine = true;
					string usage = CommandLine.Parser.ArgumentsUsage( cmdArgs.GetType() );
					Console.Write( usage );
				}
				exitCode = ExitCode.ArgumentsError;
			} else {
				verbosity.Level = (TraceLevel)cmdArgs.Verbose;
				if ( verbosity.TraceVerbose ) {
					TraceCommandLine( args );
					didTraceCommandLine = true;
				}
				Processor processor = null;
				try {
					CSqlOptions csqlOptions = cmdArgs.CreateCsqlOptions();
					processor = new Processor( csqlOptions );

					processor.SignIn();

					processor.Process();

					processor.SignOut();

					exitCode = ExitCode.Success;
				}
				catch ( FileNotFoundException ex ) {
					if ( !didTraceCommandLine && verbosity.TraceError ) {
						TraceCommandLine( args );
					}
					Trace.WriteLineIf( verbosity.TraceError, ex.FileName + ": " + ex.Message );
					exitCode = ExitCode.FileIOError;
				}
				catch ( IOException ex ) {
					if ( !didTraceCommandLine && verbosity.TraceError ) {
						TraceCommandLine( args );
					}
					Trace.WriteLineIf( verbosity.TraceError, ex.Message );
					exitCode = ExitCode.FileIOError;
				}
				catch ( TerminateException ex ) {
					if ( !didTraceCommandLine && verbosity.TraceError ) {
						TraceCommandLine( args );
					}
					if ( processor != null ) {
						processor.SignOut();
					}
					exitCode = ex.ExitCode;
				}
				catch ( Exception ex ) {
					if ( !didTraceCommandLine && verbosity.TraceError ) {
						TraceCommandLine( args );
					}
					Trace.WriteLineIf( verbosity.TraceError, "Unexpected error: " + ex );
					exitCode = ExitCode.UnexpectedError;
				}
				finally {
					Trace.Flush();
					if ( processor != null ) {
						processor.Dispose();
					}
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


		private static IEnumerable<UsageDialogDataItem> GetUsageDialogData()
		{
			Parser parser = new Parser( typeof( CmdArgs ), null );
			List<UsageDialogDataItem> items = new List<UsageDialogDataItem>();

			foreach ( Parser.Argument arg in parser.Arguments ) {
				UsageDialogDataItem item = new UsageDialogDataItem();
				item.Option = arg.LongName;
				item.Description = arg.FullHelpText;
				item.Default = arg.DefaultValue == null ? string.Empty : arg.DefaultValue.ToString();
				items.Add( item );
			}
			return items;
		}

		private class UsageDialogDataItem
		{
			public string Option { get; set; }
			public string Description { get; set; }
			public string Default { get; set; }
		}
	}
}
