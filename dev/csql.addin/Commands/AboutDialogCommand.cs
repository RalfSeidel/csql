using Sqt.VisualStudio;

namespace csql.addin.Commands
{
	internal class AboutDialogCommand : VsCommand
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
