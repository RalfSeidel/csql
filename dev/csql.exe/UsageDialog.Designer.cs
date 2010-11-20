namespace csql.exe
{
	partial class UsageDialog
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
			System.Windows.Forms.Button okButton;
			System.Windows.Forms.Label infoLabel;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( UsageDialog ) );
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.Option = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Default = new System.Windows.Forms.DataGridViewTextBoxColumn();
			okButton = new System.Windows.Forms.Button();
			infoLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// okButton
			// 
			okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			okButton.BackColor = System.Drawing.SystemColors.ButtonFace;
			okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			okButton.Location = new System.Drawing.Point( 440, 385 );
			okButton.Name = "okButton";
			okButton.Size = new System.Drawing.Size( 75, 23 );
			okButton.TabIndex = 0;
			okButton.Text = "&OK";
			okButton.UseVisualStyleBackColor = false;
			// 
			// dataGridView
			// 
			this.dataGridView.AllowUserToAddRows = false;
			this.dataGridView.AllowUserToDeleteRows = false;
			this.dataGridView.AllowUserToOrderColumns = true;
			this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView.Columns.AddRange( new System.Windows.Forms.DataGridViewColumn[] {
            this.Option,
            this.Description,
            this.Default} );
			this.dataGridView.DataMember = "DataItems";
			this.dataGridView.Enabled = false;
			this.dataGridView.Location = new System.Drawing.Point( 0, 51 );
			this.dataGridView.MultiSelect = false;
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.ReadOnly = true;
			this.dataGridView.RowHeadersWidth = 10;
			this.dataGridView.Size = new System.Drawing.Size( 526, 328 );
			this.dataGridView.TabIndex = 1;
			// 
			// Option
			// 
			this.Option.DataPropertyName = "Option";
			this.Option.HeaderText = "Option";
			this.Option.Name = "Option";
			this.Option.ReadOnly = true;
			// 
			// Description
			// 
			this.Description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.Description.DataPropertyName = "Description";
			this.Description.HeaderText = "Description";
			this.Description.Name = "Description";
			this.Description.ReadOnly = true;
			// 
			// Default
			// 
			this.Default.DataPropertyName = "Default";
			this.Default.HeaderText = "Default";
			this.Default.Name = "Default";
			this.Default.ReadOnly = true;
			// 
			// infoLabel
			// 
			infoLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
			infoLabel.Location = new System.Drawing.Point( 12, 9 );
			infoLabel.Name = "infoLabel";
			infoLabel.Size = new System.Drawing.Size( 503, 39 );
			infoLabel.TabIndex = 0;
			infoLabel.Text = "This program is inteded to run from the command line. See following table for inf" +
    "ormation about valid arguments...";
			// 
			// UsageDialog
			// 
			this.AcceptButton = okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.CancelButton = okButton;
			this.ClientSize = new System.Drawing.Size( 527, 419 );
			this.Controls.Add( infoLabel );
			this.Controls.Add( okButton );
			this.Controls.Add( this.dataGridView );
			this.Icon = ((System.Drawing.Icon)(resources.GetObject( "$this.Icon" )));
			this.Name = "UsageDialog";
			this.Text = "CSQL Usage";
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn Option;
		private System.Windows.Forms.DataGridViewTextBoxColumn Description;
		private System.Windows.Forms.DataGridViewTextBoxColumn Default;
	}
}