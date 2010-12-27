namespace csql.addin.Settings.Gui
{
	partial class SettingsControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.ComboBox editorObjects;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( SettingsControl ) );
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.editorObjects = new System.Windows.Forms.ComboBox();
			this.commandBar = new System.Windows.Forms.ToolStrip();
			this.saveChangesButton = new System.Windows.Forms.ToolStripButton();
			this.copyScriptParameterButton = new System.Windows.Forms.ToolStripButton();
			this.deleteScriptParameterButton = new System.Windows.Forms.ToolStripButton();
			this.commandBar.SuspendLayout();
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
			this.editorObjects.Size = new System.Drawing.Size( 254, 21 );
			this.editorObjects.TabIndex = 0;
			this.editorObjects.SelectionChangeCommitted += new System.EventHandler( this.EditorObjects_SelectionChangeCommitted );
			// 
			// commandBar
			// 
			this.commandBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.commandBar.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.commandBar.Dock = System.Windows.Forms.DockStyle.None;
			this.commandBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.commandBar.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.saveChangesButton,
            this.copyScriptParameterButton,
            this.deleteScriptParameterButton} );
			this.commandBar.Location = new System.Drawing.Point( 229, 3 );
			this.commandBar.Name = "commandBar";
			this.commandBar.Size = new System.Drawing.Size( 103, 25 );
			this.commandBar.TabIndex = 2;
			// 
			// saveChangesButton
			// 
			this.saveChangesButton.BackColor = System.Drawing.SystemColors.Control;
			this.saveChangesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.saveChangesButton.Image = ((System.Drawing.Image)(resources.GetObject( "saveChangesButton.Image" )));
			this.saveChangesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.saveChangesButton.Name = "saveChangesButton";
			this.saveChangesButton.Size = new System.Drawing.Size( 23, 22 );
			this.saveChangesButton.Text = "Save Changes";
			this.saveChangesButton.Click += new System.EventHandler( this.SaveChanges_Click );
			// 
			// copyScriptParameterButton
			// 
			this.copyScriptParameterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.copyScriptParameterButton.Image = ((System.Drawing.Image)(resources.GetObject( "copyScriptParameterButton.Image" )));
			this.copyScriptParameterButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.copyScriptParameterButton.Name = "copyScriptParameterButton";
			this.copyScriptParameterButton.Size = new System.Drawing.Size( 23, 22 );
			this.copyScriptParameterButton.Text = "Clone current script parameter set";
			this.copyScriptParameterButton.Click += new System.EventHandler( this.CopyScriptParameter_Click );
			// 
			// deleteScriptParameterButton
			// 
			this.deleteScriptParameterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.deleteScriptParameterButton.Image = ((System.Drawing.Image)(resources.GetObject( "deleteScriptParameterButton.Image" )));
			this.deleteScriptParameterButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.deleteScriptParameterButton.Name = "deleteScriptParameterButton";
			this.deleteScriptParameterButton.Size = new System.Drawing.Size( 23, 22 );
			this.deleteScriptParameterButton.Text = "Delete current script parameter set";
			this.deleteScriptParameterButton.Click += new System.EventHandler( this.DeleteScriptParameter_Click );
			// 
			// SettingsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.commandBar );
			this.Controls.Add( this.editorObjects );
			this.Controls.Add( this.propertyGrid );
			this.Name = "SettingsControl";
			this.Size = new System.Drawing.Size( 332, 418 );
			this.VisibleChanged += new System.EventHandler( this.SettingsControl_VisibleChanged );
			this.commandBar.ResumeLayout( false );
			this.commandBar.PerformLayout();
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip commandBar;
		private System.Windows.Forms.ToolStripButton saveChangesButton;
		private System.Windows.Forms.ToolStripButton copyScriptParameterButton;
		private System.Windows.Forms.ToolStripButton deleteScriptParameterButton;

	}
}
