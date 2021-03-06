using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using csql.addin.Commands;
using Microsoft.VisualStudio.CommandBars;
using Sqt.VisualStudio;
using EnvDTE;
using Extensibility;
using System.Diagnostics.CodeAnalysis;

namespace csql.addin
{
	/// <summary>
	/// The main object for implementing an add-in.
	/// </summary>
	/// <seealso class='IDTExtensibility2' />
	[ComVisible(true)]
	[SuppressMessage( "Microsoft.Naming", "CA1722:IdentifiersShouldNotHaveIncorrectPrefix", Justification="The C is part of the product name." )]
	public class CSqlAddin : VsAddin
	{
		/// <summary>
		/// Hold a reference to the application solution event because if application.MiscFilesEvents
		/// is used locally only it will be deleted by the garbage collector.
		/// </summary>
		/// <seealso cref="http://www.mztools.com/articles/2005/MZ2005012.aspx">
		/// PRB: Visual Studio .NET events being disconnected from add-in.
		/// </seealso>
		private EnvDTE.ProjectItemsEvents miscProjectItemsEvents;
		private EnvDTE.ProjectItemsEvents csharpProjectItemsEvents;

		/// <summary>
		/// Implements the constructor for the Add-in object. Place your initialization code within this method.
		/// </summary>
		public CSqlAddin()
		{
			Assembly currentAssembly = Assembly.GetExecutingAssembly();
			string imageResourceRoot = "csql.addin.Resources.Images.Commands.";
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

			command = new CancelScriptCommand();
			command.CommandBarName = "csql";
			command.IconId = 21;
			command.Icon = new VsCommandIcon( currentAssembly, imageResourceRoot + "CancelScript.bmp" );
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
		/// Add an event handler for the rename project item event. 
		/// </summary>
		/// <seealso cref="http://www.mztools.com/articles/2005/MZ2005009.aspx">HOWTO: Getting Project and ProjectItem events from a Visual Studio .NET add-in.</seealso>
		protected override void OnAfterStartupComplete()
		{
			this.miscProjectItemsEvents = this.Application.Events.MiscFilesEvents;
			if ( this.miscProjectItemsEvents != null ) {
				this.miscProjectItemsEvents.ItemRenamed += new _dispProjectItemsEvents_ItemRenamedEventHandler( Commands.FileGroupRenamer.OnRename );
			}
			this.csharpProjectItemsEvents = this.Application.Events.GetObject( "CSharpProjectItemsEvents" ) as ProjectItemsEvents;
			if ( this.csharpProjectItemsEvents != null ) {
				this.csharpProjectItemsEvents.ItemRenamed += new _dispProjectItemsEvents_ItemRenamedEventHandler( Commands.FileGroupRenamer.OnRename );
			}
		}


		/// <summary>
		/// Get or create a command bar with the given name.
		/// </summary>
		/// <param name="name">The name of the command bar.</param>
		/// <returns>The command bar with the specified name. If the command bar does not exist the method creates a new one.</returns>
		protected override CommandBar GetCommandBar( string name )
		{
            Debug.WriteLine( "GetCommandBar(" + name + ")" );

			CommandBars commandBars = (CommandBars)Application.CommandBars;
			foreach ( object item in commandBars ) {
				CommandBar commandBar = item as CommandBar;
				if ( commandBar == null )
					continue;

				if ( commandBar.Name.Equals( name ) )
					return commandBar;
			}
            Debug.WriteLine( "Command bar not found. Adding new..." );

            CommandBar newCommandBar = (CommandBar)commandBars.Add( name, MsoBarPosition.msoBarTop, System.Type.Missing, false );
            newCommandBar.Visible = true;
            return newCommandBar;
		}
	}
}