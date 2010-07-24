using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using EnvDTE80;
using EnvDTE;


namespace Sqt.VisualStudio
{
	/// <summary>
	/// Properties of a visual studio command.
	/// </summary>
	public abstract class VsCommand
	{
		#region Private fields

		/// <summary>
		/// The command title <see cref="P:Title"/>
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string title;

		/// <summary>
		/// The command bar name <see cref="P:CommandBarName"/>.
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string commandBarName;

		/// <summary>
		/// <see cref="P:Position"/>
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private int position;

		/// <summary>
		/// The command icon. 
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private VsCommandIcon icon;

		/// <summary>
		/// The icon id i.e. the numerical name of the icon in the addin resources.
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private int iconId;

		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="title">The title of the command i.e. the text that appears in the tool bar.</param>
		public VsCommand( string title )
		{
			this.title = title;
		}

		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		/// <remarks>
		/// The default implementation returns the type name of the command 
		/// but removes the "Command" string from the end of the type name,
		/// e.g. For a derived command class named "LocateFileCommand" the property
		/// returns "LocateFile".
		/// </remarks>
		public virtual string Name
		{
			get
			{
				string name = this.GetType().Name;
				if ( name.EndsWith( "Command" ) && name.Length != "Command".Length )
					name = name.Substring( 0, name.Length - "Command".Length );
				return name;
			}
		}

		/// <summary>
		/// The command title displayed in the GUI.
		/// </summary>
		public virtual string Title
		{
			get { return this.title; }
			set { this.title = value; }
		}

		/// <summary>
		/// Gets the name of the command bar for this command.
		/// </summary>
		public virtual string CommandBarName
		{
			get
			{
				if ( String.IsNullOrEmpty( this.commandBarName ) ) {
					return "Tools";
				}
				else {
					return this.commandBarName;
				}
			}
			set { this.commandBarName = value; }
		}

		/// <summary>
		/// Gets the position in the command bar.
		/// </summary>
		public virtual int Position
		{
			get { return this.position; }
			set { this.position = value; }
		}

		/// <summary>
		/// Gets the visual button image.
		/// </summary>
		/// <remarks>
		/// Our first approach to build toolbars with commands used this image
		/// when creating the toolbar buttons. However this doesn't work in
		/// Visual Studio 2010. The current implementation associates the 
		/// command with an image that is identified by and integer id.
		/// </remarks>
		public virtual VsCommandIcon Icon
		{
			get { return this.icon; }
			set { this.icon = value; }
		}

		/// <summary>
		/// Gets the id of the icon in the resources.
		/// </summary>
		/// <remarks>
		/// The icon id is the name of the image in the addin
		/// resource file (e.g. Resources.resx). 
		/// </remarks>
		public virtual int IconId
		{
			get { return this.iconId; }
			set { this.iconId = value; }
		}


		/// <summary>
		/// An array of context guid's in which the command 
		/// is available.
		/// </summary>
		public Guid[] VsContextGuids { get; set; }


		/// <summary>
		/// Downcast the context guid to an array of object
		/// used for calling AddNamedCommand2.
		/// </summary>
		internal object[] VsContextGuidsObjectArray
		{
			get
			{
				if ( VsContextGuids == null )
					return null;

				object[] result = new object[VsContextGuids.Length];
				for ( int i = 0; i < VsContextGuids.Length; ++i ) {
					Guid guid = VsContextGuids[i];
					result[i] = guid;
				}
				return result;
			}
		}


		public virtual bool CanExecute( VsCommandEventArgs e )
		{
			return true;
		}

		public abstract void Execute( VsCommandEventArgs e );
	}
}
