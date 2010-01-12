using System;
using System.Diagnostics;
using System.IO;
using csql.addin.Gui.Views;

namespace csql.addin.Commands
{
	/// <summary>
	/// Wrapper to call the preprocessor and csql script processing engine.
	/// </summary>
    public class ScriptExecutor
    {
        private readonly string scriptfile;
		private readonly SettingsItemViewModel settingsObjectViewModel;

        public ScriptExecutor(string scriptfile, SettingsItemViewModel settingsObjectViewModel)
        {
            this.scriptfile = scriptfile;
            this.settingsObjectViewModel = settingsObjectViewModel;
        }


        public void Execute()
        {
            csql.CsqlOptions csqlOptions = CreateCsqlOptions();
            CallCsql(csqlOptions);
        }

        private csql.CsqlOptions CreateCsqlOptions()
        {
            csql.CsqlOptions csqlOptions = new csql.CsqlOptions();

            csqlOptions.ScriptFile = scriptfile;
            csqlOptions.DistributionFile = settingsObjectViewModel.Distributionfile.Value;
            if (!settingsObjectViewModel.IsDistributionfileEnabled.Value)
                csqlOptions.DistributionFile = "";

            csqlOptions.TempFile = settingsObjectViewModel.TemporaryOutputFile.Value;

            csqlOptions.BreakOnError = settingsObjectViewModel.IsBreakOnErrosEnabled.Value;
            csqlOptions.UsePreprocessor = settingsObjectViewModel.IsPreprocessorEnabled.Value;

            csqlOptions.DbServer = settingsObjectViewModel.DbServer.Value;
            csqlOptions.DbDatabase = settingsObjectViewModel.DbDatabase.Value;
            csqlOptions.DbUser = settingsObjectViewModel.DbUsername.Value;
            csqlOptions.DbPassword = settingsObjectViewModel.DbPassword.Value;

            if (settingsObjectViewModel.DbUseIs.Value)
            {
                csqlOptions.DbUser = "";
                csqlOptions.DbPassword = "";
            }


            csqlOptions.AdditionalPreprocessorArguments = settingsObjectViewModel.GeneratePreprocessorArgs();

            csqlOptions.Verbosity.Level = (System.Diagnostics.TraceLevel)settingsObjectViewModel.VerbosityLevel.Value;

            csqlOptions.NoLogo = false;
            csqlOptions.DbSystem = DbSystem.MsSql;
            csqlOptions.DbDriver = DbDriver.Default;
            csqlOptions.DbServerPort = 0;


            return csqlOptions;
        }

        private void CallCsql(csql.CsqlOptions csqlOptions)
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
