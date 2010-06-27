using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using Sqt.DbcProvider.Gui;
using System.Xml.Schema;
using Sqt.DbcProvider.GuiTest;

namespace Sqt.DbcProvider
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class DbConnectionView : Window
	{
		public DbConnectionView()
		{
			InitializeComponent();

			MruConnections mruConnections = new MruConnections();
			try {
				mruConnections = MruConnections.LoadFromFile( "MruConnections.xml" );
			}
			catch ( Exception ex ) {
				String message;
				if ( ex.InnerException != null ) {
					message = ex.InnerException.Message;
				} else {
					message = ex.Message;
				}
				MessageBox.Show( message );
			}
			DbConnectionViewModelAdapter vmLoader = new DbConnectionViewModelAdapter();
			DbConnectionViewModel vm = vmLoader.Load( mruConnections );
			this.LoginControl.DataContext = vm;
		}

		private void SaveButton_Click( object sender, RoutedEventArgs e )
		{
			try {
				DbConnectionViewModel vm = (DbConnectionViewModel)this.LoginControl.DataContext;
				DbConnectionViewModelAdapter vmSaver = new DbConnectionViewModelAdapter();
				MruConnections mruConnections = vmSaver.Save( vm );

				using ( Stream stream = new FileStream( "MruConnections2.xml", FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.SequentialScan ) ) {
					XmlSchema xmlSchema = MruConnections.Schema;
					XmlSerializer serializer = new XmlSerializer( typeof( MruConnections ), xmlSchema.TargetNamespace );
					serializer.Serialize( stream, mruConnections );
					stream.Close();
				}
			}
			catch ( Exception ex ) {
				MessageBox.Show( ex.Message );
			}


		}

		private void LoadButton_Click( object sender, RoutedEventArgs e )
		{

		}

		private void Button_Click( object sender, RoutedEventArgs e )
		{
		}

	}
}
