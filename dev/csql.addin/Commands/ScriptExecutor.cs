using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using csql.addin.Settings;
using Sqt.DbcProvider;

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

            try
            {
                using (Processor processor = BatchProcessorFactory.CreateProcessor(csqlOptions))
                {
                    processor.SignIn();

                    processor.Process();

                    processor.SignOut();
                }
            }
            catch (FileNotFoundException ex)
            {
                Trace.WriteLineIf(GlobalSettings.Verbosity.TraceError, ex.FileName + ": " + ex.Message);
            }
            catch (IOException ex)
            {
                Trace.WriteLineIf(GlobalSettings.Verbosity.TraceError, ex.Message);
            }
            catch (TerminateException ex)
            {
                Trace.WriteLineIf(GlobalSettings.Verbosity.TraceError, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GlobalSettings.Verbosity.TraceError, "Unexpected error: " + ex);
            }
            finally
            {
                Trace.Flush();
            }
        }
    }
}
