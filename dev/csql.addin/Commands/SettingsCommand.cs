using System;
using System.Reflection;
using csql.addin.Settings;
using csql.addin.Settings.Gui;
using EnvDTE;
using EnvDTE80;
using Sqt.DbcProvider;
using Sqt.VisualStudio;

namespace csql.addin.Commands
{
	/// <summary>
	/// Command for showing the settings dialog.
	/// </summary>
	internal class SettingsCommand : VsCommand
	{
		private Window toolWindow;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public SettingsCommand()
			: base( "Settings" )
		{
		}

		/// <summary>
		/// Create and show the settings panel.
		/// </summary>
		/// <param name="e"></param>
		public override void Execute( VsCommandEventArgs e )
		{
			if ( toolWindow != null ) {
				toolWindow.Visible = true;
				return;
			}

			try {
				object programmableObject = null;
				DTE2 application = e.Application;
				AddIn addInInstance = e.AddIn;
				Windows2 windows2 = (Windows2)application.Windows;
				Type userCtrlType = typeof( SettingsControl );
				Assembly userCtrlAssembly = userCtrlType.Assembly;
				String windowGuid = "{858C3FCD-3333-4540-A592-F31C1520B174}";

				toolWindow = windows2.CreateToolWindow2( addInInstance, userCtrlAssembly.Location, userCtrlType.FullName, "CSql Settings", windowGuid, ref programmableObject );

				Type resourcesType = typeof( Resources );
				string imageResourceRoot = resourcesType.FullName;
				string imageResourcePath = imageResourceRoot + ".Images.Commands.Settings.bmp";
				var icon = new VsCommandIcon( userCtrlAssembly, imageResourcePath );
				var iconPicture = icon.IconPicture;
				toolWindow.SetTabPicture( iconPicture );

				SettingsControl settingsControl = (SettingsControl)toolWindow.Object;
				settingsControl.SetApplication( application );

				toolWindow.Visible = true;
			}
			catch ( Exception ex ) {
				System.Windows.MessageBox.Show( ex.Message, "csql addin error" );
			}
		}
	}
}
