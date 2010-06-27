namespace csql.addin.Settings.Gui
{
	partial class SettingsControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && (components != null) ) {
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.editorObjects = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// propertyGrid
			// 
			this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid.CommandsVisibleIfAvailable = false;
			this.propertyGrid.Location = new System.Drawing.Point( 0, 30 );
			this.propertyGrid.Margin = new System.Windows.Forms.Padding( 4 );
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.propertyGrid.Size = new System.Drawing.Size( 332, 388 );
			this.propertyGrid.TabIndex = 1;
			this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler( this.PropertyGrid_PropertyValueChanged );
			// 
			// editorObjects
			// 
			this.editorObjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.editorObjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.editorObjects.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.editorObjects.Location = new System.Drawing.Point( 3, 3 );
			this.editorObjects.Name = "editorObjects";
			this.editorObjects.Size = new System.Drawing.Size( 257, 21 );
			this.editorObjects.TabIndex = 0;
			this.editorObjects.SelectionChangeCommitted += new System.EventHandler( this.EditorObjects_SelectionChangeCommitted );
			// 
			// SettingsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.editorObjects );
			this.Controls.Add( this.propertyGrid );
			this.Name = "SettingsControl";
			this.Size = new System.Drawing.Size( 332, 418 );
			this.VisibleChanged += new System.EventHandler( this.SettingsControl_VisibleChanged );
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.ComboBox editorObjects;

	}
}
