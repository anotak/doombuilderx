namespace CodeImp.DoomBuilder.Windows
{
	partial class ErrorsForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.copyselected = new System.Windows.Forms.Button();
            this.clearlist = new System.Windows.Forms.Button();
            this.close = new System.Windows.Forms.Button();
            this.checkerrors = new System.Windows.Forms.Timer(this.components);
            this.checkshow = new System.Windows.Forms.CheckBox();
            this.grid = new System.Windows.Forms.DataGridView();
            this.iconcolumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.textcolumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();
            // 
            // copyselected
            // 
            this.copyselected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.copyselected.Location = new System.Drawing.Point(15, 520);
            this.copyselected.Margin = new System.Windows.Forms.Padding(4);
            this.copyselected.Name = "copyselected";
            this.copyselected.Size = new System.Drawing.Size(152, 31);
            this.copyselected.TabIndex = 1;
            this.copyselected.Text = "Copy Selection";
            this.copyselected.UseVisualStyleBackColor = true;
            this.copyselected.Click += new System.EventHandler(this.copyselected_Click);
            // 
            // clearlist
            // 
            this.clearlist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.clearlist.Location = new System.Drawing.Point(188, 520);
            this.clearlist.Margin = new System.Windows.Forms.Padding(4);
            this.clearlist.Name = "clearlist";
            this.clearlist.Size = new System.Drawing.Size(152, 31);
            this.clearlist.TabIndex = 2;
            this.clearlist.Text = "Clear";
            this.clearlist.UseVisualStyleBackColor = true;
            this.clearlist.Click += new System.EventHandler(this.clearlist_Click);
            // 
            // close
            // 
            this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.close.Location = new System.Drawing.Point(678, 517);
            this.close.Margin = new System.Windows.Forms.Padding(4);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(252, 31);
            this.close.TabIndex = 4;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // checkerrors
            // 
            this.checkerrors.Interval = 1000;
            this.checkerrors.Tick += new System.EventHandler(this.checkerrors_Tick);
            // 
            // checkshow
            // 
            this.checkshow.AutoSize = true;
            this.checkshow.Location = new System.Drawing.Point(376, 525);
            this.checkshow.Margin = new System.Windows.Forms.Padding(4);
            this.checkshow.Name = "checkshow";
            this.checkshow.Size = new System.Drawing.Size(257, 20);
            this.checkshow.TabIndex = 3;
            this.checkshow.Text = "Show this window when errors occur";
            this.checkshow.UseVisualStyleBackColor = true;
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.AllowUserToResizeColumns = false;
            this.grid.AllowUserToResizeRows = false;
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.grid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.grid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.grid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.grid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.ColumnHeadersVisible = false;
            this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iconcolumn,
            this.textcolumn});
            this.grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.grid.Location = new System.Drawing.Point(15, 15);
            this.grid.Margin = new System.Windows.Forms.Padding(4);
            this.grid.Name = "grid";
            this.grid.ReadOnly = true;
            this.grid.RowHeadersVisible = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(2, 4, 2, 5);
            this.grid.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.grid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.ShowCellErrors = false;
            this.grid.ShowCellToolTips = false;
            this.grid.ShowEditingIcon = false;
            this.grid.ShowRowErrors = false;
            this.grid.Size = new System.Drawing.Size(915, 494);
            this.grid.StandardTab = true;
            this.grid.TabIndex = 5;
            // 
            // iconcolumn
            // 
            this.iconcolumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.iconcolumn.HeaderText = "Icon";
            this.iconcolumn.MinimumWidth = 20;
            this.iconcolumn.Name = "iconcolumn";
            this.iconcolumn.ReadOnly = true;
            this.iconcolumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.iconcolumn.Width = 24;
            // 
            // textcolumn
            // 
            this.textcolumn.HeaderText = "Text";
            this.textcolumn.Name = "textcolumn";
            this.textcolumn.ReadOnly = true;
            // 
            // ErrorsForm
            // 
            this.AcceptButton = this.close;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.close;
            this.ClientSize = new System.Drawing.Size(945, 566);
            this.Controls.Add(this.checkshow);
            this.Controls.Add(this.close);
            this.Controls.Add(this.clearlist);
            this.Controls.Add(this.copyselected);
            this.Controls.Add(this.grid);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Errors and Warnings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ErrorsForm_FormClosing);
            this.Shown += new System.EventHandler(this.ErrorsForm_Shown);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ErrorsForm_HelpRequested);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button copyselected;
		private System.Windows.Forms.Button clearlist;
		private System.Windows.Forms.Button close;
		private System.Windows.Forms.Timer checkerrors;
		private System.Windows.Forms.CheckBox checkshow;
		private System.Windows.Forms.DataGridView grid;
		private System.Windows.Forms.DataGridViewImageColumn iconcolumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn textcolumn;
	}
}