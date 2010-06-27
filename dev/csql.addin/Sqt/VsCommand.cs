using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE80;
using EnvDTE;
using System.Windows.Input;

namespace Sqt.VisualStudio
{
	/// <summary>
	/// Properties of a visual studio command.
	/// </summary>
	public abstract class VsCommand
	{
		/// <summary>
		/// The command title <see cref="P:Title"/>
		/// </summary>
		private string title;

		/// <summary>
		/// The command bar name <see cref="P:CommandBarName"/>.
		/// </summary>
		private string commandBarName;

		/// <summary>
		/// <see cref="P:Position"/>
		/// </summary>
		private int position;

		/// <summary>
		/// <see cref="P:ButtonStyle"/>
		/// </summary>
		private VsCommandIcon icon;

		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		/// <remarks>
		/// The default implementation returns the type name of the command 
		/// but removes the "Command" string from the end of the type name.
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
			get { return this.commandBarName; }
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
		/// Gets the visual button style.
		/// </summary>
		public virtual VsCommandIcon Icon
		{
			get { return this.icon; }
			set { this.icon = value; }
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="title">The title of the command i.e. the text that appears in the tool bar.</param>
		public VsCommand( string title )
		{
			this.title = title;
		}

		public virtual bool CanExecute( VsCommandEventArgs e )
		{
			return true;
		}

		public abstract void Execute( VsCommandEventArgs e );
	}
}
