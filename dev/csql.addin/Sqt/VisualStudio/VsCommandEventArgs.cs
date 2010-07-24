using System;
using System.Diagnostics;
using EnvDTE;
using EnvDTE80;

namespace Sqt.VisualStudio
{
	/// <summary>
	/// Arguments passed when a command event is raised.
	/// </summary>
	public class VsCommandEventArgs : EventArgs
	{
		#region Private fields 

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private readonly DTE2 dte;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private readonly AddIn addIn;

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="VsCommandEventArgs"/> class.
		/// </summary>
		/// <param name="dte">The Visual Studio application object.</param>
		/// <param name="addIn">The Visual Studio add in object.</param>
		public VsCommandEventArgs( DTE2 application, AddIn addIn )
		{
			this.dte = application;
			this.addIn = addIn;
		}

		/// <summary>
		/// Gets the Visual Studio application object.
		/// </summary>
		public DTE2 Application
		{
			get { return this.dte; }
		}

		/// <summary>
		/// Gets the Visual Studio add in object.
		/// </summary>
		public AddIn AddIn
		{
			get { return this.addIn; }
		}

	}
}
