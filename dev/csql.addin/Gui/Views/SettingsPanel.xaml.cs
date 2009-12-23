
using System.Windows.Controls;
using csql.addin.Settings;

namespace csql.addin.Gui.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SettingsPanel : UserControl
    {
		/// <summary>
		/// Variable for the <see cref="P:ViewModel"/> property.
		/// </summary>
        private readonly SettingsPanelViewModel viewModel;

		public SettingsPanelViewModel ViewModel 
		{ 
			get { return this.viewModel; } 
		}


        public SettingsPanel(SettingsPanelViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.DataContext = viewModel;
            InitializeComponent();
        }
    }
}
