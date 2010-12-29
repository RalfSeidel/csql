using System;
using System.Reflection;
using System.Windows.Forms;

namespace csql.addin.Gui
{
	partial class AboutDialog : Form
	{
		public AboutDialog()
		{
			InitializeComponent();
			this.Text = String.Format( "About {0}", AssemblyTitle );
			this.labelProductName.Text = AssemblyProduct;
			this.labelVersion.Text = String.Format( "Version {0}", AssemblyVersion );
			this.labelCopyright.Text = AssemblyCopyright;
			this.labelCompanyName.Text = AssemblyCompany;
			this.textBoxDescription.Text = AssemblyDescription;
			this.logoBox.Image = Resources.SqlServiceLogo;
		}

		#region Assembly Attribute Accessors

		private string AssemblyTitle
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyTitleAttribute ), false );
				if ( attributes.Length > 0 ) {
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if ( titleAttribute.Title != "" ) {
						return titleAttribute.Title;
					}
				}
				return System.IO.Path.GetFileNameWithoutExtension( Assembly.GetExecutingAssembly().CodeBase );
			}
		}

		private string AssemblyVersion
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}

		private string AssemblyDescription
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyDescriptionAttribute ), false );
				if ( attributes.Length == 0 ) {
					return string.Empty;
				}
				return ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		private string AssemblyProduct
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyProductAttribute ), false );
				if ( attributes.Length == 0 ) {
					return string.Empty;
				}
				return ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		private string AssemblyCopyright
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyCopyrightAttribute ), false );
				if ( attributes.Length == 0 ) {
					return string.Empty;
				}
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		private string AssemblyCompany
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyCompanyAttribute ), false );
				if ( attributes.Length == 0 ) {
					return string.Empty;
				}
				return ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}

		#endregion
	}
}
