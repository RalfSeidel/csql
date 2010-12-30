using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace csql.addin.Settings.Gui
{
	/// <summary>
	/// A designer editor like the <see cref="System.Windows.Forms.Design.FileNameEditor"/> editor
	/// used to specify output file.
	/// </summary>
	[System.Runtime.InteropServices.ComVisible( true )]
	internal class OutputFileEditor : UITypeEditor
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public OutputFileEditor()
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

			using ( SaveFileDialog saveFileDialog = new SaveFileDialog() ) {
				string fileName = value as string;
				this.InitializeDialog( saveFileDialog, fileName );
				if ( saveFileDialog.ShowDialog() == DialogResult.OK ) {
					value = saveFileDialog.FileName;
				}
			}
			return value;
		}


		protected virtual void InitializeDialog( SaveFileDialog saveFileDialog, string currentFileName )
		{
			saveFileDialog.CheckFileExists = false;
			saveFileDialog.OverwritePrompt = false;
			saveFileDialog.ValidateNames = true;
			saveFileDialog.RestoreDirectory = true;
			saveFileDialog.AutoUpgradeEnabled = true;
			saveFileDialog.Title = "Select file";
			saveFileDialog.Filter = "SQL Script (*.sql)|*.sql|Text files (*.txt;*.log)|*.txt;*.log|All Files (*.*)|*.*";
			if ( !String.IsNullOrEmpty( currentFileName ) ) {
				saveFileDialog.InitialDirectory = Path.GetDirectoryName( currentFileName );
				saveFileDialog.FileName = currentFileName;
			}
		}

	}
}
