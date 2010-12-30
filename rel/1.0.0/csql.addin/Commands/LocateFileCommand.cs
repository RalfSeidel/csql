using EnvDTE;
using EnvDTE80;
using Sqt.VisualStudio;

namespace csql.addin.Commands
{
	internal class LocateFileCommand : VsCommand
	{
		internal LocateFileCommand()
			: base( "Locate File" )
		{
		}

		/// <summary>
		/// Determines whether the active document is a member of any loaded project.
		/// </summary>
		public override bool CanExecute( VsCommandEventArgs e )
		{
			DTE2 application = e.Application;

			// Any document open?
			Document activeDocument = application.ActiveDocument;
			if ( activeDocument == null )
				return false;

			ProjectItem projectItem = activeDocument.ProjectItem;
			if ( projectItem == null )
				return false;

			return true;
		}

		public override void Execute( VsCommandEventArgs e )
		{
			DTE2 application = e.Application;
			SolutionExplorerFileLocator locator = new SolutionExplorerFileLocator( application );
			locator.LocateAndSelectCurrentFile();
		}
	}
}
