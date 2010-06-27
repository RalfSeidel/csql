using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using csql.addin.Commands;
using csql.addin.Gui.Views;
using csql.addin.Settings;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using Sqt.VisualStudio;
using System.Reflection;

namespace csql.addin
{
	/// <summary>
	/// The main object for implementing an add-in.
	/// </summary>
	/// <seealso class='IDTExtensibility2' />
	[ComVisible(true)]
	public class CSqlAddin : VsAddin
	{
		/// <summary>
		/// Implements the constructor for the Add-in object. Place your initialization code within this method.
		/// </summary>
		public CSqlAddin()
		{
			Assembly currentAssembly = Assembly.GetExecutingAssembly();
			String imageResourceRoot = "csql.addin.Resources.Images.Commands.";
			VsCommand command;
			int position = 1;

			command = new SettingsCommand();
			command.CommandBarName = "csql";
			command.IconId = 50;
			command.Icon = new VsCommandIcon( currentAssembly, imageResourceRoot + "Settings.bmp" );
			command.Position = position++;
			RegisterCommand( command );

			command = new ExecuteScriptCommand();
			command.CommandBarName = "csql";
			command.IconId = 20;
			command.Icon = new VsCommandIcon( currentAssembly, imageResourceRoot + "ExecuteScript.bmp" );
			command.Position = position++;
			RegisterCommand( command );

			command = new LocateFileCommand();
			command.CommandBarName = "csql";
			command.IconId = 40;
			command.Icon = new VsCommandIcon( currentAssembly, imageResourceRoot + "LocateFile.bmp" );
			command.Position = position++;
			RegisterCommand( command );

			command = new GroupFilesCommand();
			command.CommandBarName = "csql";
			command.IconId = 30;
			command.Icon = new VsCommandIcon( currentAssembly, imageResourceRoot + "GroupFiles.bmp" );
			command.Position = position++;
			RegisterCommand( command );

			command = new AboutDialogCommand();
			command.CommandBarName = "csql";
			command.IconId = 10;
			command.Icon = new VsCommandIcon( currentAssembly, imageResourceRoot + "AboutDialog.bmp" );
			command.Position = position++;
			RegisterCommand( command );
		}

		/// <summary>
		/// Get or create a command bar with the given name.
		/// </summary>
		/// <param name="name">The name of the command bar.</param>
		/// <returns>The command bar with the specified name. If the command bar does not exist the method creates a new one.</returns>
		protected override CommandBar GetCommandBar( string name )
		{
            Debug.WriteLine("GetCommandBar(" + name + ")");

			CommandBars commandBars = (CommandBars)Application.CommandBars;
			foreach ( Object item in commandBars ) {
				CommandBar commandBar = item as CommandBar;
				if ( commandBar == null )
					continue;

				if ( commandBar.Name.Equals( name ) )
					return commandBar;
			}
            
            Debug.WriteLine("Command bar not found. Addin new..." );

            CommandBar newCommandBar = (CommandBar)commandBars.Add(name, MsoBarPosition.msoBarTop, System.Type.Missing, true);
            newCommandBar.Visible = true;
            return newCommandBar;

		}
	}
}