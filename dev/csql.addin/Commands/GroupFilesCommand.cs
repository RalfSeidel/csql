using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sqt.VisualStudio;
using EnvDTE80;

namespace csql.addin.Commands
{
	class GroupFilesCommand : VsCommand
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
