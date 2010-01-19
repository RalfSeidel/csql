using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio.CommandBars;
using System.Drawing;
using stdole;

namespace Sqt.VisualStudio
{
	/// <summary>
	/// Base class for visual studio addins.
	/// </summary>
	/// <remarks>
	/// More or less copied from KODA <see cref="http://koda.codeplex.com"/> and extended for 
	/// our needs.
	/// </remarks>
	public abstract class VsAddIn : IDTExtensibility2, IDTCommandTarget
	{
		/// <summary>
		/// Reference to the Visual Studio application object passed 
		/// to the <see cref="M:OnConnection"/>  method.
		/// </summary>
		private DTE2 application;

		/// <summary>
		/// Reference to the Visual Studio application addin object passed 
		/// to the <see cref="M:OnConnection"/>  method.
		/// </summary>
		private AddIn addin;

		private object[] contextGuids;

		/// <summary>
		/// Collection of registered command identified by their name.
		/// </summary>
		private Dictionary<string, VsCommand> vsCommands;

		/// <summary>
		/// Gets the Visual Studio application object.
		/// </summary>
		public DTE2 Application
		{
			get { return this.application; }
		}

		/// <summary>
		/// Gets the Visual Studio add in object.
		/// </summary>
		public AddIn AddIn
		{
			get { return this.addin; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="VsAddin"/> class.
		/// </summary>
		public VsAddIn()
		{
			this.vsCommands = new Dictionary<string, VsCommand>();
		}

		/// <summary>
		/// Registers a command for later integration (during <see cref="M:OnConnection"/>)
		/// </summary>
		/// <param name="command">The command to register.</param>
		protected void RegisterCommand( VsCommand command )
		{
			string fullCommandName = GetFullCommandName( command.Name );
			this.vsCommands.Add( fullCommandName, command );
		}

		/// <summary>
		/// Occurs whenever an add-in is loaded or unloaded from the Visual Studio integrated development environment (IDE).
		/// </summary>
		/// <param name="custom">An empty array that you can use to pass host-specific data for use in the add-in.</param>
		void IDTExtensibility2.OnAddInsUpdate( ref Array custom )
		{
		}

		/// <summary>
		/// Occurs whenever the Visual Studio integrated development environment (IDE) shuts down while an add-in is running.
		/// </summary>
		/// <param name="custom">An empty array that you can use to pass host-specific data for use in the add-in.</param>
		void IDTExtensibility2.OnBeginShutdown( ref Array custom )
		{
		}

		/// <summary>
		/// Occurs whenever an add-in is loaded into Visual Studio.
		/// </summary>
		/// <param name="Application">A reference to an instance of the integrated development environment (IDE), <see cref="T:EnvDTE.DTE"></see>, which is the root object of the Visual Studio automation model.</param>
		/// <param name="ConnectMode">An <see cref="T:Extensibility.ext_ConnectMode"></see> enumeration value that indicates the way the add-in was loaded into Visual Studio.</param>
		/// <param name="AddInInst">An <see cref="T:EnvDTE.AddIn"></see> reference to the add-in's own instance. This is stored for later use, such as determining the parent collection for the add-in.</param>
		/// <param name="custom">An empty array that you can use to pass host-specific data for use in the add-in.</param>
		void IDTExtensibility2.OnConnection( object Application, ext_ConnectMode ConnectMode, object AddInInst, ref Array custom )
		{
			this.application = (DTE2)Application;
			this.addin = (AddIn)AddInInst;
			this.OnInitialize();

			if ( ConnectMode == ext_ConnectMode.ext_cm_UISetup ) {
				this.contextGuids = new object[] { };

				foreach ( VsCommand vsCommand in this.vsCommands.Values ) {
					AddIdeCommand( vsCommand );
				}
			}
		}

		/// <summary>
		/// Occurs whenever an add-in is unloaded from Visual Studio.
		/// </summary>
		/// <param name="RemoveMode">
		/// An <see cref="T:Extensibility.ext_DisconnectMode"></see> enumeration value that 
		/// informs an add- why it was unloaded.
		/// </param>
		/// <param name="custom">
		/// An empty array that you can use to pass host-specific data for use after the add-in unloads.
		/// </param>
		void IDTExtensibility2.OnDisconnection( ext_DisconnectMode RemoveMode, ref Array custom )
		{
			if ( RemoveMode == ext_DisconnectMode.ext_dm_UserClosed ) {
				foreach ( VsCommand vsCommand in this.vsCommands.Values ) {
					RemoveIdeCommand( vsCommand );
				}
			}
		}

		/// <summary>
		/// Occurs whenever an add-in, which is set to load when Visual Studio starts, loads.
		/// </summary>
		/// <param name="custom">An empty array that you can use to pass host-specific data for use when the add-in loads.</param>
		void IDTExtensibility2.OnStartupComplete( ref Array custom )
		{
		}

		/// <summary>
		/// Executes the specified named command.
		/// </summary>
		/// <param name="CmdName">The name of the command to execute.</param>
		/// <param name="ExecuteOption">A <see cref="T:EnvDTE.vsCommandExecOption"></see> constant specifying the execution options.</param>
		/// <param name="VariantIn">A value passed to the command.</param>
		/// <param name="VariantOut">A value passed back to the invoker Exec method after the command executes.</param>
		/// <param name="Handled">true indicates that the command was implemented. false indicates that it was not.</param>
		void IDTCommandTarget.Exec( string cmdName, vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled )
		{
			Handled = false;
			if ( ExecuteOption == vsCommandExecOption.vsCommandExecOptionDoDefault ) {
				if ( this.vsCommands.ContainsKey( cmdName ) ) {
					VsCommand vsCommand = this.vsCommands[cmdName];
					vsCommand.Execute( new VsCommandEventArgs( this.application, this.addin ) );
					Handled = true;
				}
			}
		}

		/// <summary>
		/// Returns the current status (enabled, disabled, hidden, and so forth) of the specified named command.
		/// </summary>
		/// <param name="CmdName">The name of the command to check.</param>
		/// <param name="NeededText">A <see cref="T:EnvDTE.vsCommandStatusTextWanted"></see> constant specifying if information is returned from the check, and if so, what type of information is returned.</param>
		/// <param name="StatusOption">A <see cref="T:EnvDTE.vsCommandStatus"></see> specifying the current status of the command.</param>
		/// <param name="CommandText">The text to return if <see cref="F:EnvDTE.vsCommandStatusTextWanted.vsCommandStatusTextWantedStatus"></see> is specified.</param>
		void IDTCommandTarget.QueryStatus( string cmdName, vsCommandStatusTextWanted neededText, ref vsCommandStatus statusOption, ref object commandText )
		{
			if ( neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone ) {
				if ( this.vsCommands.ContainsKey( cmdName ) ) {
					VsCommand vsCommand = this.vsCommands[cmdName];
					if ( vsCommand.CanExecute( new VsCommandEventArgs( this.application, this.addin ) ) ) {
						statusOption = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
					} else {
						statusOption = vsCommandStatus.vsCommandStatusUnsupported;
					}
				}
			}
		}

		protected virtual void OnInitialize()
		{
		}

		/// <summary>
		/// Gets the command bar with specified name.
		/// </summary>
		/// <param name="name">The name of the command bar usually some like "Tools".</param>
		/// <returns>The command bar or <c>null</c> if no command bar with such name exists.</returns>
		protected virtual CommandBar GetCommandBar( string name )
		{
			CommandBars commandBars = (CommandBars)this.application.CommandBars;
			CommandBar commandBar = commandBars[this.GetLocalizedName( name )];
			return commandBar;
		}

		private string GetLocalizedName( string name )
		{
			/*
			try {
				ResourceManager resourceManager = new ResourceManager( "csql.Resources.CommandBar", Assembly.GetExecutingAssembly() );
				CultureInfo cultureInfo = new CultureInfo( this.Application.LocaleID );
				string resourceName = String.Concat( cultureInfo.TwoLetterISOLanguageName, name );
				string localizedName = resourceManager.GetString( resourceName );
				if ( String.IsNullOrEmpty( localizedName ) )
					return name;
				return localizedName;
			}
			catch {
				return name;
			}
			*/
			return name;
		}

		/// <summary>
		/// Add a command to the Visual Studio shell.
		/// </summary>
		/// <param name="vsCommand">The add in command properties</param>
		private void AddIdeCommand( VsCommand vsCommand )
		{
			Command ideCommand = GetIdeCommand( vsCommand );

			if ( ideCommand == null ) {
				Commands2 ideCommands = (Commands2)this.Application.Commands;
				//StdPicture commandPicture = null;
				//if ( vsCommand.Icon != null ) {
				//    commandPicture = vsCommand.Icon.IconPicture;
				//}
				ideCommand = ideCommands.AddNamedCommand2(
					this.AddIn,
					vsCommand.Name,
					vsCommand.Title,
					String.Empty,
					true,
					null,
					ref this.contextGuids,
					(int)vsCommandStatus.vsCommandStatusSupported | (int)vsCommandStatus.vsCommandStatusEnabled,
					(int)vsCommandStyle.vsCommandStylePictAndText,
					vsCommandControlType.vsCommandControlTypeButton );
			}

			string[] temp = vsCommand.CommandBarName.Split( '.' );
			CommandBar commandBar = GetCommandBar( temp[0] );
			object commandControl;

			if ( temp.Length == 1 ) {
				commandControl= ideCommand.AddControl( commandBar, vsCommand.Position );
			} else if ( temp.Length == 2 ) {
				CommandBarPopup commandBarPopup = (CommandBarPopup)commandBar.Controls[this.GetLocalizedName( temp[1] )];
				commandControl = ideCommand.AddControl( commandBarPopup.CommandBar, vsCommand.Position );
			} else {
				throw new NotSupportedException( "A command hierarchy greater then two is not supported yet" );
			}

			if ( commandControl != null ) {
				CommandBarButton commandButton = (CommandBarButton)commandControl;
				if ( vsCommand.Icon != null ) {
					StdPicture iconPicture = vsCommand.Icon.IconPicture;
					commandButton.Picture = iconPicture;
					StdPicture iconMask = vsCommand.Icon.IconMask;
					commandButton.Mask = iconMask;
					commandButton.Style = MsoButtonStyle.msoButtonIconAndCaption;
				} else {
					commandButton.Style = MsoButtonStyle.msoButtonAutomatic;
				}
			}
		}

		private Command GetIdeCommand( VsCommand vsCommand )
		{
			Commands2 ideCommands = (Commands2)this.Application.Commands;
			string commandName = GetFullCommandName( vsCommand.Name );
			foreach ( Object item in ideCommands ) {
				Command ideCommand = item as Command;
				if ( ideCommand == null ) 
					continue;

				if ( ideCommand.Name.Equals( commandName ) )
					return ideCommand;
			}
			return null;
		}

		/// <summary>
		/// Removes a command added to the visual studio ide.
		/// </summary>
		private void RemoveIdeCommand( VsCommand vsCommand )
		{
			Commands2 ideCommands = (Commands2)this.Application.Commands;
			string commandName = GetFullCommandName( vsCommand.Name );
			foreach ( Object item in ideCommands ) {
				Command ideCommand = item as Command;
				if ( ideCommand == null )
					continue;

				if ( ideCommand.Name.Equals( commandName ) ) {
					ideCommand.Delete();
				}
			}
		}

		/// <summary>
		/// For a command defined by this add in get the full name that is 
		/// used by Visual Studio to manage the commmands.
		/// </summary>
		/// <returns>The concatiation of the full add in type name and the given name.</returns>
		private string GetFullCommandName( string addInCommandName )
		{
			string fullCommandName = String.Format( "{0}.{1}", this.GetType().FullName, addInCommandName );
			return fullCommandName;
		}
	}
}
