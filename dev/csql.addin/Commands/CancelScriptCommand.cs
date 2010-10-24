using System;
using System.Diagnostics;
using csql.addin.Settings;
using EnvDTE;
using EnvDTE80;
using Sqt.DbcProvider;
using Sqt.VisualStudio;

namespace csql.addin.Commands
{
	/// <summary>
	/// Command to cancel/stop the execution of the current (c)sql script.
	/// </summary>
	internal class CancelScriptCommand : VsCommand
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CancelScriptCommand"/> class.
		/// </summary>
		internal CancelScriptCommand()
			: base( "Cancel" )
		{
		}

		/// <summary>
		/// Check if a script is currently executed.
		/// </summary>
		/// <returns><c>true</c> if a script is currently executed. <c>false</c> otherwise.</returns>
		public override bool CanExecute( VsCommandEventArgs e )
		{
			ExecuteScriptCommand executeCommand = ExecuteScriptCommand.Instance;
			if ( executeCommand == null )
				return false;

			return executeCommand.IsExecuting;
		}

		public override void Execute( VsCommandEventArgs e )
		{
			ExecuteScriptCommand executeCommand = ExecuteScriptCommand.Instance;
			if ( executeCommand == null )
				return;

			executeCommand.CancelExecution();
		}
	}
}
