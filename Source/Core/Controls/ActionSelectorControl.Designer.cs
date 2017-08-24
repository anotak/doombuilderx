namespace CodeImp.DoomBuilder.Controls
{
	partial class ActionSelectorControl
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
            this.list = new System.Windows.Forms.ComboBox();
            this.numberpanel = new System.Windows.Forms.Panel();
            this.number = new CodeImp.DoomBuilder.Controls.AutoSelectTextbox();
            this.numberpanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // list
            // 
            this.list.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.list.DropDownHeight = 318;
            this.list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.list.FormattingEnabled = true;
            this.list.IntegralHeight = false;
            this.list.Location = new System.Drawing.Point(71, 0);
            this.list.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.list.MaxDropDownItems = 15;
            this.list.Name = "list";
            this.list.Size = new System.Drawing.Size(313, 23);
            this.list.TabIndex = 1;
            this.list.TabStop = false;
            this.list.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.list_DrawItem);
            this.list.SelectionChangeCommitted += new System.EventHandler(this.list_SelectionChangeCommitted);
            this.list.DropDownClosed += new System.EventHandler(this.list_DropDownClosed);
            // 
            // numberpanel
            // 
            this.numberpanel.BackColor = System.Drawing.SystemColors.Window;
            this.numberpanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.numberpanel.Controls.Add(this.number);
            this.numberpanel.Location = new System.Drawing.Point(0, 0);
            this.numberpanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numberpanel.Name = "numberpanel";
            this.numberpanel.Size = new System.Drawing.Size(65, 25);
            this.numberpanel.TabIndex = 0;
            // 
            // number
            // 
            this.number.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.number.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.number.Location = new System.Drawing.Point(4, 1);
            this.number.Margin = new System.Windows.Forms.Padding(4);
            this.number.Name = "number";
            this.number.Size = new System.Drawing.Size(54, 15);
            this.number.TabIndex = 0;
            this.number.Text = "402";
            this.number.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.number.TextChanged += new System.EventHandler(this.number_TextChanged);
            this.number.KeyDown += new System.Windows.Forms.KeyEventHandler(this.number_KeyDown);
            this.number.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.number_KeyPress);
            // 
            // ActionSelectorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.numberpanel);
            this.Controls.Add(this.list);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ActionSelectorControl";
            this.Size = new System.Drawing.Size(478, 26);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.ActionSelectorControl_Layout);
            this.Resize += new System.EventHandler(this.ActionSelectorControl_Resize);
            this.numberpanel.ResumeLayout(false);
            this.numberpanel.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private CodeImp.DoomBuilder.Controls.AutoSelectTextbox number;
		private System.Windows.Forms.ComboBox list;
		private System.Windows.Forms.Panel numberpanel;
	}
}
