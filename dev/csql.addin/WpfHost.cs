using System;

using System.Windows.Forms;
using System.Windows.Forms.Integration;

using csql.addin.Gui.Views;

namespace csql.addin
{
    public partial class WpfHost : UserControl
    {
        public WpfHost()
        {
            InitializeComponent();
        }

        private void WpfUserControlHost_Load(object sender, EventArgs e)
        {
            
        }

        public void LoadContent(SettingsPanelViewModel viewModel)
        {
            //Create the ElementHost control for hosting the
            //WPF UserControl.
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;

            //Konfiguration laden
           // Settings.Settings settings = Settings.Settings.LoadFromConfigFile(Settings.Settings.configFile);

            //viewModel = new csql.Addin.Settings.ViewModel(settings);
            SettingsPanel dialog = new SettingsPanel(viewModel);


            host.Child = dialog;

            // Add the ElementHost control to the form's
            // collection of child controls.
            this.Controls.Add(host);
        }
    }
}
