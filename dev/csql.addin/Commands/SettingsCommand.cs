using System;
using EnvDTE;
using EnvDTE80;
using Sqt.VisualStudio;
using csql.addin.Gui.Views;

namespace csql.addin.Commands
{
	class SettingsCommand : VsCommand
	{
		private Window toolWindow;

		internal SettingsCommand()
			: base( "Settings" )
		{
		}

		public override void Execute( VsCommandEventArgs e )
		{
			if ( toolWindow != null ) {
				toolWindow.Visible = true;
				return;
			}
			WpfHost wpfHost = null;

			try {
				object programmableObject = null;

				DTE2 application = e.Application;
				Windows2 windows2 = (Windows2)application.Windows;
				System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
				AddIn addInInstance = e.AddIn;
				String guidstr = "{858C3FCD-3333-4540-A592-F31C1520B174}";
				Type wpfHostType = typeof( WpfHost );
				toolWindow = windows2.CreateToolWindow2( addInInstance, asm.Location, wpfHostType.FullName, "CSQL Settings", guidstr, ref programmableObject );

				toolWindow.Visible = true;


				//Das Wpf-Control setzten
				wpfHost = (WpfHost)toolWindow.Object;

				//Content laden
				SettingsPanelViewModel settingsViewModel = CSqlAddIn.GetSettingsCollection( application );
				wpfHost.LoadContent( settingsViewModel );
			}
			catch ( Exception ex ) {
				System.Windows.MessageBox.Show( ex.Message, "csql addin error" );
			}
		}
	}
}
