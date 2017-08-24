namespace CodeImp.DoomBuilder.Controls
{
	partial class FieldsEditorControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
			this.fieldslist = new System.Windows.Forms.DataGridView();
			this.fieldname = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.fieldtype = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.fieldvalue = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.deleterowstimer = new System.Windows.Forms.Timer(this.components);
			this.browsebutton = new System.Windows.Forms.Button();
			this.enumscombo = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.fieldslist)).BeginInit();
			this.SuspendLayout();
			// 
			// fieldslist
			// 
			this.fieldslist.AllowUserToResizeColumns = false;
			this.fieldslist.AllowUserToResizeRows = false;
			this.fieldslist.BackgroundColor = System.Drawing.SystemColors.Window;
			this.fieldslist.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.fieldslist.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.fieldslist.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			this.fieldslist.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			this.fieldslist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.fieldslist.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fieldname,
            this.fieldtype,
            this.fieldvalue});
			dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle24.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle24.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle24.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle24.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle24.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle24.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.fieldslist.DefaultCellStyle = dataGridViewCellStyle24;
			this.fieldslist.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.fieldslist.Location = new System.Drawing.Point(0, 0);
			this.fieldslist.MultiSelect = false;
			this.fieldslist.Name = "fieldslist";
			this.fieldslist.RowHeadersVisible = false;
			this.fieldslist.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.SystemColors.Window;
			this.fieldslist.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
			this.fieldslist.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			this.fieldslist.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			this.fieldslist.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.fieldslist.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.fieldslist.Size = new System.Drawing.Size(444, 263);
			this.fieldslist.TabIndex = 1;
			this.fieldslist.Scroll += new System.Windows.Forms.ScrollEventHandler(this.fieldslist_Scroll);
			this.fieldslist.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.fieldslist_UserDeletingRow);
			this.fieldslist.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.fieldslist_CellBeginEdit);
			this.fieldslist.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.fieldslist_CellDoubleClick);
			this.fieldslist.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.fieldslist_ColumnHeaderMouseClick);
			this.fieldslist.MouseUp += new System.Windows.Forms.MouseEventHandler(this.fieldslist_MouseUp);
			this.fieldslist.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.fieldslist_CellEndEdit);
			this.fieldslist.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.fieldslist_CellClick);
			this.fieldslist.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.fieldslist_DataError);
			this.fieldslist.SelectionChanged += new System.EventHandler(this.fieldslist_SelectionChanged);
			// 
			// fieldname
			// 
			dataGridViewCellStyle21.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle21.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle21.NullValue = null;
			dataGridViewCellStyle21.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle21.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			this.fieldname.DefaultCellStyle = dataGridViewCellStyle21;
			this.fieldname.Frozen = true;
			this.fieldname.HeaderText = "Property";
			this.fieldname.Name = "fieldname";
			this.fieldname.Width = 150;
			// 
			// fieldtype
			// 
			this.fieldtype.AutoComplete = false;
			dataGridViewCellStyle22.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle22.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle22.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle22.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			this.fieldtype.DefaultCellStyle = dataGridViewCellStyle22;
			this.fieldtype.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
			this.fieldtype.HeaderText = "Type";
			this.fieldtype.Name = "fieldtype";
			this.fieldtype.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.fieldtype.Sorted = true;
			this.fieldtype.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// fieldvalue
			// 
			dataGridViewCellStyle23.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle23.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle23.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle23.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			this.fieldvalue.DefaultCellStyle = dataGridViewCellStyle23;
			this.fieldvalue.HeaderText = "Value";
			this.fieldvalue.Name = "fieldvalue";
			this.fieldvalue.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.fieldvalue.Width = 120;
			// 
			// deleterowstimer
			// 
			this.deleterowstimer.Interval = 1;
			this.deleterowstimer.Tick += new System.EventHandler(this.deleterowstimer_Tick);
			// 
			// browsebutton
			// 
			this.browsebutton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.browsebutton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browsebutton.Image = global::CodeImp.DoomBuilder.Properties.Resources.List;
			this.browsebutton.Location = new System.Drawing.Point(343, 75);
			this.browsebutton.Name = "browsebutton";
			this.browsebutton.Size = new System.Drawing.Size(28, 22);
			this.browsebutton.TabIndex = 2;
			this.browsebutton.TabStop = false;
			this.browsebutton.UseVisualStyleBackColor = true;
			this.browsebutton.Click += new System.EventHandler(this.browsebutton_Click);
			// 
			// enumscombo
			// 
			this.enumscombo.FormattingEnabled = true;
			this.enumscombo.ItemHeight = 14;
			this.enumscombo.Location = new System.Drawing.Point(295, 121);
			this.enumscombo.Name = "enumscombo";
			this.enumscombo.Size = new System.Drawing.Size(128, 22);
			this.enumscombo.TabIndex = 3;
			this.enumscombo.Validating += new System.ComponentModel.CancelEventHandler(this.enumscombo_Validating);
			// 
			// FieldsEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.enumscombo);
			this.Controls.Add(this.browsebutton);
			this.Controls.Add(this.fieldslist);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "FieldsEditorControl";
			this.Size = new System.Drawing.Size(474, 286);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.FieldsEditorControl_Layout);
			this.Resize += new System.EventHandler(this.FieldsEditorControl_Resize);
			((System.ComponentModel.ISupportInitialize)(this.fieldslist)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView fieldslist;
		private System.Windows.Forms.Timer deleterowstimer;
		private System.Windows.Forms.Button browsebutton;
		private System.Windows.Forms.ComboBox enumscombo;
		private System.Windows.Forms.DataGridViewTextBoxColumn fieldname;
		private System.Windows.Forms.DataGridViewComboBoxColumn fieldtype;
		private System.Windows.Forms.DataGridViewTextBoxColumn fieldvalue;
	}
}
