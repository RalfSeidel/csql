using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio.CommandBars;
using Sqt.VisualStudio.Util;
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
	[ComVisible( true )]
	public abstract class VsAddin : IDTExtensibility2, IDTCommandTarget
	{
		#region Private fields

		/// <summary>
		/// Reference to the Visual Studio application object passed 
		/// to the <see cref="M:OnConnection"/>  method.
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private DTE2 application;

		/// <summary>
		/// Reference to the Visual Studio application addin object passed 
		/// to the <see cref="M:OnConnection"/>  method.
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private AddIn addin;

		/// <summary>
		/// Collection of registered command identified by their name.
		/// </summary>
		private Dictionary<string, VsCommand> vsCommands;

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="VsAddin"/> class.
		/// </summary>
		protected VsAddin()
		{
			this.vsCommands = new Dictionary<string, VsCommand>();
		}

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
		/// Occurs whenever an add-in is loaded or unloaded from the Visual Studio integrated development environment (IDE).
		/// </summary>
		/// <param name="custom">An empty array that you can use to pass host-specific data for use in the add-in.</param>
		[SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This class is intended to hide the complexity of the COM interface" )]
		void IDTExtensibility2.OnAddInsUpdate( ref Array custom )
		{
		}

		/// <summary>
		/// Occurs whenever the Visual Studio integrated development environment (IDE) shuts down while an add-in is running.
		/// </summary>
		/// <param name="custom">An empty array that you can use to pass host-specific data for use in the add-in.</param>
		[SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This class is intended to hide the complexity of the COM interface" )]
		void IDTExtensibility2.OnBeginShutdown( ref Array custom )
		{
		}

		/// <summary>
		/// Occurs whenever an add-in is loaded into Visual Studio.
		/// </summary>
		/// <param name="Application">A reference to an instance of the integrated development environment (IDE), <see cref="T:EnvDTE.DTE"></see>, which is the root object of the Visual Studio automation model.</param>
		/// <param name="ConnectMode">
		/// An <see cref="T:Extensibility.ext_ConnectMode"></see> enumeration value that indicates 
		/// the way the add-in was loaded into Visual Studio.
		/// </param>
		/// <param name="AddInInst">An <see cref="T:EnvDTE.AddIn"></see> reference to the add-in's own instance. This is stored for later use, such as determining the parent collection for the add-in.</param>
		/// <param name="custom">
		/// An empty array that you can use to pass host-specific data for use in the add-in.
		/// </param>
		[SuppressMessage( "Microsoft.StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Naming follows the declaration in the interface." )]
		[SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This class is intended to hide the complexity of the COM interface" )]
		void IDTExtensibility2.OnConnection( object Application, ext_ConnectMode ConnectMode, object AddInInst, ref Array custom )
		{
			Debug.WriteLine( GetType().Name + ".OnConnection( ConnectMode=" + ConnectMode + " )" );
			this.application = (DTE2)Application;
			this.addin = (AddIn)AddInInst;

			switch ( ConnectMode ) {
				case ext_ConnectMode.ext_cm_AfterStartup:
					// The add-in was loaded by hand after startup using the Add-In Manager
					// Initialize it in the same way that when is loaded on startup
					AddIdeCommands();
					OnAfterStartupComplete();
					break;
				case ext_ConnectMode.ext_cm_Startup:
					// The add-in was marked to load on startup and visual studio just started.
					// Do nothing here because the IDE may not be fully initialized.
					// Visual Studio will call OnStartupComplete when fully initialized.
					break;
				case ext_ConnectMode.ext_cm_UISetup:
					// One time initialisation.
					// Do nothing here because commands and the command bars are recreated after
					// every startup.
					break;
				case ext_ConnectMode.ext_cm_CommandLine:
					break;
				case ext_ConnectMode.ext_cm_External:
					break;
				case ext_ConnectMode.ext_cm_Solution:
					break;
				default:
					break;
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
		[SuppressMessage( "Microsoft.StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Naming follows the declaration in the interface." )]
		[SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This class is intended to hide the complexity of the COM interface" )]
		void IDTExtensibility2.OnDisconnection( ext_DisconnectMode RemoveMode, ref Array custom )
		{
			Debug.WriteLine( "OnDisconnection" );

			if ( RemoveMode == ext_DisconnectMode.ext_dm_UserClosed ) {
				StringDictionary commandBarNames = new StringDictionary();

				// Remove all commands and collect their command bar names
				foreach ( VsCommand vsCommand in this.vsCommands.Values ) {
					string commandBarName = vsCommand.CommandBarName;
					if ( !String.IsNullOrEmpty( commandBarName ) && !commandBarNames.ContainsKey( commandBarName ) ) {
						commandBarNames.Add( commandBarName, commandBarName );
					}
					RemoveIdeCommand( vsCommand );
				}

				// Delete the command bars which are empty now.
				foreach ( string commandBarName in commandBarNames.Keys ) {
					CommandBar commandBar = GetCommandBar( commandBarName );
					if ( commandBar != null && !commandBar.BuiltIn && commandBar.Controls.Count == 0 ) {
						commandBar.Delete();
						Debug.WriteLine( "Deleting Command-Bars" );
					}
				}
			}
		}

		/// <summary>
		/// Occurs whenever an add-in, which is set to load when Visual Studio starts, loads.
		/// </summary>
		/// <param name="custom">An empty array that you can use to pass host-specific data for use when the add-in loads.</param>
		[SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This class is intended to hide the complexity of the COM interface" )]
		void IDTExtensibility2.OnStartupComplete( ref Array custom )
		{
			AddIdeCommands();
			OnAfterStartupComplete();
		}

		/// <summary>
		/// Executes the specified named command.
		/// </summary>
		/// <param name="CmdName">The name of the command to execute.</param>
		/// <param name="ExecuteOption">A <see cref="T:EnvDTE.vsCommandExecOption"></see> constant specifying the execution options.</param>
		/// <param name="VariantIn">A value passed to the command.</param>
		/// <param name="VariantOut">A value passed back to the invoker Exec method after the command executes.</param>
		/// <param name="Handled">true indicates that the command was implemented. false indicates that it was not.</param>
		[SuppressMessage( "Microsoft.StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Naming follows the declaration in the interface." )]
		[SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This class is intended to hide the complexity of the COM interface" )]
		void IDTCommandTarget.Exec( string CmdName, vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled )
		{
			Handled = false;
			if ( ExecuteOption == vsCommandExecOption.vsCommandExecOptionDoDefault ) {
				if ( this.vsCommands.ContainsKey( CmdName ) ) {
					VsCommand vsCommand = this.vsCommands[CmdName];
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
		[SuppressMessage( "Microsoft.StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Naming follows the declaration in the interface." )]
		[SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This class is intended to hide the complexity of the COM interface" )]
		void IDTCommandTarget.QueryStatus( string CmdName, vsCommandStatusTextWanted neededText, ref vsCommandStatus statusOption, ref object commandText )
		{
			Debug.WriteLine( "QueryStatus(" + CmdName + ")" );

			if ( neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone ) {
				if ( this.vsCommands.ContainsKey( CmdName ) ) {
					VsCommand vsCommand = this.vsCommands[CmdName];
					if ( vsCommand.CanExecute( new VsCommandEventArgs( this.application, this.addin ) ) ) {
						statusOption = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
					}
					else {
						statusOption = vsCommandStatus.vsCommandStatusSupported;
					}
				}
			}
		}

		/// <summary>
		/// Called immediatly before the addin commands are added.
		/// </summary>
		protected virtual void OnAfterStartupComplete()
		{
			Debug.WriteLine( "OnAfterStartupComplete()" );
		}

		/// <summary>
		/// Registers a command for later integration (during <see cref="M:OnConnection"/>)
		/// </summary>
		/// <param name="command">The command to register.</param>
		protected void RegisterCommand( VsCommand command )
		{
			string fullCommandName = GetFullCommandName( command.Name );

			if ( this.vsCommands.ContainsKey( fullCommandName ) ) {
				throw new ArgumentException( "The command " + fullCommandName + " is already registered", "command" );
			}

			this.vsCommands.Add( fullCommandName, command );
			if ( command.Position == 0 ) {
				int maxPosition = 0;
				foreach ( var c in this.vsCommands.Values ) {
					if ( c.Position > maxPosition )
						maxPosition = c.Position;
				}
				command.Position = maxPosition + 1;
			}
		}

		/// <summary>
		/// Gets the command bar with specified name.
		/// </summary>
		/// <param name="name">The name of the command bar usually some like "Tools".</param>
		/// <returns>The command bar or <c>null</c> if no command bar with such name exists.</returns>
		protected virtual CommandBar GetCommandBar( string name )
		{
			CommandBars commandBars = (CommandBars)this.application.CommandBars;
			CommandBar commandBar = commandBars[name];
			if ( commandBar == null ) {
				application.GetMenuBar( name );
			}
			return commandBar;
		}

		/// <summary>
		/// Add all registered commands to the visual studio shell.
		/// </summary>
		private void AddIdeCommands()
		{
			foreach ( VsCommand vsCommand in this.vsCommands.Values ) {
				AddIdeCommand( vsCommand );
			}
		}

		/// <summary>
		/// Add a command to the Visual Studio shell.
		/// </summary>
		/// <param name="vsCommand">The add in command properties</param>
		private void AddIdeCommand( VsCommand vsCommand )
		{
			Debug.WriteLine( "AddIdeCommand(" + vsCommand.Name + ")" );

			Command ideCommand = FindIdeCommand( vsCommand );

			if ( ideCommand == null ) {
				Commands2 ideCommands = (Commands2)this.Application.Commands;
				bool useMsoButtons = vsCommand.IconId == 0;
				object[] contextGuids = vsCommand.VsContextGuidsObjectArray;
				ideCommand = ideCommands.AddNamedCommand2(
					this.AddIn,
					vsCommand.Name,
					vsCommand.Title,
					String.Empty,
					useMsoButtons,
					vsCommand.IconId,
					ref contextGuids,
					(int)vsCommandStatus.vsCommandStatusSupported | (int)vsCommandStatus.vsCommandStatusEnabled,
					(int)vsCommandStyle.vsCommandStylePictAndText,
					vsCommandControlType.vsCommandControlTypeButton );
			}

			string commandBarName = vsCommand.CommandBarName;
			string[] temp = commandBarName.Split( '.' );
			CommandBar commandBar = GetCommandBar( temp[0] );
			if ( commandBar == null )
				return;

			object commandControl = null;

			if ( temp.Length == 1 ) {
				if ( !ContainsCommand( commandBar.Controls, ideCommand ) ) {
					commandControl = ideCommand.AddControl( commandBar, vsCommand.Position );
				}
			}
			else if ( temp.Length == 2 ) {
				CommandBarPopup commandBarPopup = (CommandBarPopup)commandBar.Controls[temp[1]];
				if ( !ContainsCommand( commandBarPopup.Controls, ideCommand ) ) {
					commandControl = ideCommand.AddControl( commandBarPopup.CommandBar, vsCommand.Position );
				}
			}
			else {
				throw new NotSupportedException( "A command hierarchy greater than two is not supported yet" );
			}

			if ( commandControl != null ) {
				CommandBarButton commandButton = (CommandBarButton)commandControl;
				bool useMsoButtons = vsCommand.IconId == 0;
				if ( useMsoButtons && vsCommand.Icon != null ) {
					StdPicture iconPicture = vsCommand.Icon.IconPicture;
					commandButton.Picture = iconPicture;
					StdPicture iconMask = vsCommand.Icon.IconMask;
					commandButton.Mask = iconMask;
					commandButton.Style = MsoButtonStyle.msoButtonIconAndCaption;
				}
				else {
					commandButton.Style = MsoButtonStyle.msoButtonAutomatic;
				}
			}
		}

		/// <summary>
		/// Determines whether one of the specified command bar controls is associated with the specified command.
		/// </summary>
		private bool ContainsCommand( CommandBarControls commandBarControls, Command ideCommand )
		{
			foreach ( CommandBarControl commandBarControl in commandBarControls ) {
				string commandGuid;
				int commandId;
				application.Commands.CommandInfo( commandBarControl, out commandGuid, out commandId );
				Command controlCommand = application.Commands.Item( commandGuid, commandId );
				if ( ideCommand.Equals( controlCommand ) )
					return true;
			}
			return false;
		}

		/// <summary>
		/// Removes a command added to the visual studio ide.
		/// </summary>
		private void RemoveIdeCommand( VsCommand vsCommand )
		{
			Command ideCommand = FindIdeCommand( vsCommand );
			if ( ideCommand != null ) {
				ideCommand.Delete();
			}
		}


		/// <summary>
		/// Seeks for an existing Visual Studio IDE command with the name name as the specified command.
		/// </summary>
		/// <param name="vsCommand">The command that is seeked.</param>
		/// <returns>
		/// The existing IDE command or <c>null</c> if the command is not yet added to
		/// the Visual Studio IDE.
		/// </returns>
		private Command FindIdeCommand( VsCommand vsCommand )
		{
			Commands2 ideCommands = (Commands2)this.Application.Commands;
			string commandName = GetFullCommandName( vsCommand.Name );
			foreach ( object item in ideCommands ) {
				Command ideCommand = item as Command;
				if ( ideCommand == null )
					continue;

				if ( ideCommand.Name.Equals( commandName ) )
					return ideCommand;
			}
			return null;
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
