using EnvDTE80;
using Sqt.VisualStudio;

namespace csql.addin.Commands
{
	internal class GroupFilesCommand : VsCommand
	{
		internal GroupFilesCommand() : base( "Group Files" )
		{
		}

		public override void Execute( VsCommandEventArgs e )
		{
			DTE2 application = e.Application;
			FileGrouper grouper = new FileGrouper( application );
			grouper.GroupProjectItems();
		}
	}
}
