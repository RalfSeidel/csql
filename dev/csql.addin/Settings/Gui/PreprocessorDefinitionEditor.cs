using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;

namespace csql.addin.Settings.Gui
{
	/// <summary>
	/// A designer editor like the <see cref="System.Windows.Forms.Design.FileNameEditor"/> editor
	/// used to edit the list of preprocessor definitions.
	/// </summary>
	public class PreprocessorDefinitionEditor : CollectionEditor
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public PreprocessorDefinitionEditor()
			: base( typeof( PreprocessorDefinition ) )
		{
		}

		/// <summary>
		/// Returns the modal style.
		/// </summary>
		/// <returns><see cref="UITypeEditorEditStyle.Modal"/></returns>
		public override UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context )
		{
			return UITypeEditorEditStyle.Modal;
		}

		/// <summary>
		/// Opens the <see cref="System.Windows.Forms.SaveFileDialog"/> and returns the path of the file
		/// the user selected.
		/// </summary>
		public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
		{
			if ( provider == null )
				return value;

			// Check that the editor is called from a windows froms context.
			IWindowsFormsEditorService editorSerivce = (IWindowsFormsEditorService)provider.GetService( typeof( IWindowsFormsEditorService ) );
			if ( editorSerivce == null )
				return value;


			using ( PreprocessorDefinitionDialog dialog = new PreprocessorDefinitionDialog() ) {
				ICollection<PreprocessorDefinition> definitions = (ICollection<PreprocessorDefinition>)value;
				Control ctrl = (Control)editorSerivce;
				dialog.Font = ctrl.Font;
				dialog.Definitions = definitions;
				if ( dialog.ShowDialog() == DialogResult.OK ) {
					value = dialog.Definitions;
				}
			}
			return value;
		}
	}
}
