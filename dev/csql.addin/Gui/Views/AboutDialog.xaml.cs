using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;

namespace csql.addin.Gui.Views
{
	/// <summary>
	/// Interaction logic for the about/info dialog.
	/// </summary>
	[ComVisible(false)]
	internal partial class AboutDialog : Window
	{
		public AboutDialog()
		{
			InitializeComponent();
		}

		private void Window_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Escape ) 
				this.Close();
		}
	}
}
