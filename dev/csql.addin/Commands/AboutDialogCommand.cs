using Sqt.VisualStudio;

namespace csql.addin.Commands
{
	class AboutDialogCommand : VsCommand
	{
		internal AboutDialogCommand()
			: base( "About" )
		{
		}

		public override void Execute( VsCommandEventArgs e )
		{
			var view = new Gui.Views.AboutDialog();
			view.ShowDialog();
		}
	}
}
