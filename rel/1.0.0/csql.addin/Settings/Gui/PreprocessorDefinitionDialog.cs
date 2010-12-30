using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

namespace csql.addin.Settings.Gui
{
	/// <summary>
	/// A simple form that presents a grid with preprocessor definitions
	/// </summary>
	internal partial class PreprocessorDefinitionDialog : Form
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public PreprocessorDefinitionDialog()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Gets or sets the values edited in the grid.
		/// </summary>
		public ICollection<PreprocessorDefinition> Definitions
		{
			get
			{
				object datasource = this.gridView.DataSource;
				BindingList<PreprocessorDefinition> list = (BindingList<PreprocessorDefinition>)datasource;
				ICollection<PreprocessorDefinition> result = new List<PreprocessorDefinition>();
				foreach ( var item in list ) {
					result.Add( item );
				}

				return result;
			}
			set
			{
				if ( value == null ) {
					this.gridView.DataSource = new BindingList<PreprocessorDefinition>();
				}
				else {
					BindingList<PreprocessorDefinition> list = new BindingList<PreprocessorDefinition>();
					foreach ( var item in value ) {
						list.Add( item );
					}
					this.gridView.DataSource = list;
				}
			}
		}
	}
}
