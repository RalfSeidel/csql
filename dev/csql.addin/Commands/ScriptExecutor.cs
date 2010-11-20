using System;
using System.Diagnostics;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace csql.addin.Commands
{
	/// <summary>
	/// Wrapper to call the preprocessor and csql script processing engine.
	/// </summary>
	[SuppressMessage( "Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification="The field processor is disposed internaly. The reference as a field is just hold to allow cancelation while the processor is running." )]
	internal class ScriptExecutor
	{
		private readonly CSqlOptions csqlOptions;
		private readonly Processor processor;

		public ScriptExecutor( CSqlOptions csqlOptions )
		{
			this.csqlOptions = csqlOptions;
			this.processor = new Processor( csqlOptions );
		}


		[SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching general exception to keep visual studio alive no matter what happend while executing the command." )]
		public void Execute()
		{
			GlobalSettings.Verbosity.Level = csqlOptions.Verbosity.Level;

			try {
				this.processor.SignIn();

				this.processor.Process();

				this.processor.SignOut();
			}
			catch ( FileNotFoundException ex ) {
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, ex.FileName + ": " + ex.Message );
			}
			catch ( IOException ex ) {
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, ex.Message );
			}
			catch ( TerminateException ) {
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceVerbose, "Caught TerminateException" );

				this.processor.SignOut();
			}
			catch ( Exception ex ) {
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, "Unexpected error: " + ex );
			}
			finally {
				Trace.Flush();
				processor.Dispose();
			}
		}

		/// <summary>
		/// Cancels the script execution.
		/// </summary>
		internal void Cancel()
		{
			processor.Cancel();
		}
	}
}
