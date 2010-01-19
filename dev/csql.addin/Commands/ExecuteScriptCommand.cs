using csql.addin.Gui.Views;
using EnvDTE;
using EnvDTE80;
using Sqt.VisualStudio;

namespace csql.addin.Commands
{
	class ExecuteScriptCommand : VsCommand
	{
		internal ExecuteScriptCommand()
			: base( "Execute" )
		{
		}

		public override bool CanExecute( VsCommandEventArgs e )
		{
			DTE2 application = e.Application;
			if ( application.ActiveDocument == null ) {
				return false;
			}

			string fileName = application.ActiveDocument.FullName.ToLowerInvariant();
			if ( !fileName.EndsWith( ".csql" ) && !fileName.EndsWith( ".sql" ) )
				return false;

			return true;
		}

		public override void Execute( VsCommandEventArgs e )
		{
			DTE2 application = e.Application;
			string documentPath = application.ActiveDocument.FullName;

			SettingsItemViewModel settings = CSqlAddIn.GetCurrentSettings( application );
			ScriptExecutor executionHelper = new ScriptExecutor( documentPath, settings );

			executionHelper.Execute();

			// Dokument anzeigen, wenn dieses erstellt wurde und das Flag zur Erzeugung des Distributionfile gesetzt wurde
			if ( System.IO.File.Exists( settings.Distributionfile.Value ) && settings.IsDistributionfileEnabled.Value ) {
				application.ItemOperations.OpenFile( settings.Distributionfile.Value, Constants.vsViewKindCode );
			}
		}
	}
}
