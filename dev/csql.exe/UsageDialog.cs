using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;

namespace csql.exe
{
	public partial class UsageDialog : Form
	{
		private readonly IEnumerable dataItems;

		/// <summary>
		/// Initializes a new instance of the <see cref="UsageDialog"/> class.
		/// </summary>
		/// <param name="dataItems">The data items.</param>
		public UsageDialog( IEnumerable dataItems )
		{
			Debug.Assert( dataItems != null );
			InitializeComponent();
			this.dataItems = dataItems;

			this.dataGridView.DataSource = dataItems;

			SystemEvents.SessionEnding+=new SessionEndingEventHandler( OnSessionEnding );
			SystemEvents.SessionEnded+=new SessionEndedEventHandler( OnSessionEnded );
		}

		private void OnSessionEnding( object sender, SessionEndingEventArgs e )
		{
			e.Cancel = false;
		}

		private void OnSessionEnded( object sender, SessionEndedEventArgs e )
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}


		/// <summary>
		/// Gets the data items to show.
		/// </summary>
		private IEnumerable DataItems
		{
			get { return this.dataItems; }
		}
	}
}
