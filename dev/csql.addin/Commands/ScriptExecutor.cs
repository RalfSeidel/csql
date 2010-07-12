using System;
using System.Diagnostics;
using System.IO;

namespace csql.addin.Commands
{
	/// <summary>
	/// Wrapper to call the preprocessor and csql script processing engine.
	/// </summary>
	internal class ScriptExecutor
	{
		private readonly CSqlOptions csqlOptions;

		public ScriptExecutor( CSqlOptions csqlOptions )
		{
			this.csqlOptions = csqlOptions;
		}


		public void Execute()
		{
			GlobalSettings.Verbosity.Level = csqlOptions.Verbosity.Level;

			Processor processor = null;
			try {
				processor = BatchProcessorFactory.CreateProcessor( csqlOptions );
				processor.SignIn();

				processor.Process();

				processor.SignOut();
			}
			catch ( FileNotFoundException ex ) {
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, ex.FileName + ": " + ex.Message );
			}
			catch ( IOException ex ) {
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, ex.Message );
			}
			catch ( TerminateException ex ) {
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceVerbose, "Caught TerminateException" );

				if ( processor != null ) {
					processor.SignOut();
				}
			}
			catch ( Exception ex ) {
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, "Unexpected error: " + ex );
			}
			finally {
				Trace.Flush();
				if ( processor != null ) {
					processor.Dispose();
				}
			}
		}
	}
}
