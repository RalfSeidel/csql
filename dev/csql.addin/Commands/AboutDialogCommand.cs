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
			var dialog = new Gui.AboutDialog();
			dialog.ShowDialog();
		}
	}
}
