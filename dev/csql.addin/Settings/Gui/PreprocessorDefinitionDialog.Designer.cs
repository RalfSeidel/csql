using System.Collections.Generic;
namespace csql.addin.Settings.Gui
{
	partial class PreprocessorDefinitionDialog
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Button acceptButton;
			System.Windows.Forms.Button cancelButton;
			this.gridView = new System.Windows.Forms.DataGridView();
			this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.isEnabledDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.valueDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.preprocessorDefinitionBindingSource = new System.Windows.Forms.BindingSource( this.components );
			acceptButton = new System.Windows.Forms.Button();
			cancelButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.preprocessorDefinitionBindingSource)).BeginInit();
			this.SuspendLayout();
			// 
			// acceptButton
			// 
			acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			acceptButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			acceptButton.Location = new System.Drawing.Point( 300, 265 );
			acceptButton.Name = "acceptButton";
			acceptButton.Size = new System.Drawing.Size( 75, 23 );
			acceptButton.TabIndex = 1;
			acceptButton.Text = "&Ok";
			acceptButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			cancelButton.Location = new System.Drawing.Point( 381, 265 );
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new System.Drawing.Size( 75, 23 );
			cancelButton.TabIndex = 2;
			cancelButton.Text = "&Cancel";
			cancelButton.UseVisualStyleBackColor = true;
			// 
			// gridView
			// 
			this.gridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridView.AutoGenerateColumns = false;
			this.gridView.BackgroundColor = System.Drawing.SystemColors.Window;
			this.gridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridView.Columns.AddRange( new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.isEnabledDataGridViewCheckBoxColumn,
            this.valueDataGridViewTextBoxColumn} );
			this.gridView.DataSource = this.preprocessorDefinitionBindingSource;
			this.gridView.Location = new System.Drawing.Point( 0, 0 );
			this.gridView.Name = "gridView";
			this.gridView.Size = new System.Drawing.Size( 469, 259 );
			this.gridView.TabIndex = 0;
			// 
			// nameDataGridViewTextBoxColumn
			// 
			this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
			this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
			this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
			this.nameDataGridViewTextBoxColumn.Width = 60;
			// 
			// isEnabledDataGridViewCheckBoxColumn
			// 
			this.isEnabledDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			this.isEnabledDataGridViewCheckBoxColumn.DataPropertyName = "IsEnabled";
			this.isEnabledDataGridViewCheckBoxColumn.HeaderText = "Enabled";
			this.isEnabledDataGridViewCheckBoxColumn.Name = "isEnabledDataGridViewCheckBoxColumn";
			this.isEnabledDataGridViewCheckBoxColumn.Width = 52;
			// 
			// valueDataGridViewTextBoxColumn
			// 
			this.valueDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.valueDataGridViewTextBoxColumn.DataPropertyName = "Value";
			this.valueDataGridViewTextBoxColumn.HeaderText = "Value";
			this.valueDataGridViewTextBoxColumn.Name = "valueDataGridViewTextBoxColumn";
			// 
			// preprocessorDefinitionBindingSource
			// 
			this.preprocessorDefinitionBindingSource.DataSource = typeof( System.Collections.Generic.List<csql.addin.Settings.PreprocessorDefinition> );
			// 
			// PreprocessorDefinitionDialog
			// 
			this.AcceptButton = acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = cancelButton;
			this.ClientSize = new System.Drawing.Size( 468, 296 );
			this.Controls.Add( cancelButton );
			this.Controls.Add( acceptButton );
			this.Controls.Add( this.gridView );
			this.Name = "PreprocessorDefinitionDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Preprocessor Definitions";
			((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.preprocessorDefinitionBindingSource)).EndInit();
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.DataGridView gridView;
		private System.Windows.Forms.BindingSource preprocessorDefinitionBindingSource;
		private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewCheckBoxColumn isEnabledDataGridViewCheckBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn valueDataGridViewTextBoxColumn;
	}
}