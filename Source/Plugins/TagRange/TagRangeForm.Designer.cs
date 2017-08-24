namespace CodeImp.DoomBuilder.TagRange
{
	partial class TagRangeForm
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
			this.rangestart = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.itemcountlabel = new System.Windows.Forms.Label();
			this.doubletagwarning = new System.Windows.Forms.Label();
			this.skipdoubletags = new System.Windows.Forms.CheckBox();
			this.okbutton = new System.Windows.Forms.Button();
			this.cancelbutton = new System.Windows.Forms.Button();
			this.outoftagswarning = new System.Windows.Forms.Label();
			this.endtaglabel = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// rangestart
			// 
			this.rangestart.AllowDecimal = false;
			this.rangestart.AllowNegative = false;
			this.rangestart.AllowRelative = false;
			this.rangestart.ButtonStep = 1;
			this.rangestart.Location = new System.Drawing.Point(110, 56);
			this.rangestart.Name = "rangestart";
			this.rangestart.Size = new System.Drawing.Size(96, 24);
			this.rangestart.StepValues = null;
			this.rangestart.TabIndex = 0;
			this.rangestart.WhenTextChanged += new System.EventHandler(this.rangestart_WhenTextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(44, 61);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 14);
			this.label1.TabIndex = 1;
			this.label1.Text = "Start Tag:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(30, 27);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 14);
			this.label2.TabIndex = 2;
			this.label2.Text = "Items to Tag:";
			// 
			// itemcountlabel
			// 
			this.itemcountlabel.AutoSize = true;
			this.itemcountlabel.Location = new System.Drawing.Point(110, 27);
			this.itemcountlabel.Name = "itemcountlabel";
			this.itemcountlabel.Size = new System.Drawing.Size(13, 14);
			this.itemcountlabel.TabIndex = 3;
			this.itemcountlabel.Text = "0";
			// 
			// doubletagwarning
			// 
			this.doubletagwarning.BackColor = System.Drawing.SystemColors.Info;
			this.doubletagwarning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.doubletagwarning.Location = new System.Drawing.Point(12, 130);
			this.doubletagwarning.Name = "doubletagwarning";
			this.doubletagwarning.Padding = new System.Windows.Forms.Padding(3);
			this.doubletagwarning.Size = new System.Drawing.Size(273, 50);
			this.doubletagwarning.TabIndex = 4;
			this.doubletagwarning.Text = "Warning: The tag range contains already used tags.";
			this.doubletagwarning.Visible = false;
			// 
			// skipdoubletags
			// 
			this.skipdoubletags.AutoSize = true;
			this.skipdoubletags.BackColor = System.Drawing.SystemColors.Info;
			this.skipdoubletags.Location = new System.Drawing.Point(63, 154);
			this.skipdoubletags.Name = "skipdoubletags";
			this.skipdoubletags.Size = new System.Drawing.Size(161, 18);
			this.skipdoubletags.TabIndex = 5;
			this.skipdoubletags.Text = "Skip over already used tags";
			this.skipdoubletags.UseVisualStyleBackColor = false;
			this.skipdoubletags.Visible = false;
			this.skipdoubletags.CheckedChanged += new System.EventHandler(this.skipdoubletags_CheckedChanged);
			// 
			// okbutton
			// 
			this.okbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.okbutton.Location = new System.Drawing.Point(41, 197);
			this.okbutton.Name = "okbutton";
			this.okbutton.Size = new System.Drawing.Size(99, 26);
			this.okbutton.TabIndex = 6;
			this.okbutton.Text = "OK";
			this.okbutton.UseVisualStyleBackColor = true;
			this.okbutton.Click += new System.EventHandler(this.okbutton_Click);
			// 
			// cancelbutton
			// 
			this.cancelbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cancelbutton.Location = new System.Drawing.Point(158, 197);
			this.cancelbutton.Name = "cancelbutton";
			this.cancelbutton.Size = new System.Drawing.Size(99, 26);
			this.cancelbutton.TabIndex = 7;
			this.cancelbutton.Text = "Cancel";
			this.cancelbutton.UseVisualStyleBackColor = true;
			this.cancelbutton.Click += new System.EventHandler(this.cancelbutton_Click);
			// 
			// outoftagswarning
			// 
			this.outoftagswarning.BackColor = System.Drawing.SystemColors.Info;
			this.outoftagswarning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.outoftagswarning.Location = new System.Drawing.Point(12, 130);
			this.outoftagswarning.Name = "outoftagswarning";
			this.outoftagswarning.Padding = new System.Windows.Forms.Padding(3);
			this.outoftagswarning.Size = new System.Drawing.Size(273, 50);
			this.outoftagswarning.TabIndex = 8;
			this.outoftagswarning.Text = "The range exceeds the maximum allowed tags and cannot be created.";
			this.outoftagswarning.Visible = false;
			// 
			// endtaglabel
			// 
			this.endtaglabel.AutoSize = true;
			this.endtaglabel.Location = new System.Drawing.Point(110, 95);
			this.endtaglabel.Name = "endtaglabel";
			this.endtaglabel.Size = new System.Drawing.Size(13, 14);
			this.endtaglabel.TabIndex = 10;
			this.endtaglabel.Text = "0";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(49, 95);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(49, 14);
			this.label4.TabIndex = 9;
			this.label4.Text = "End Tag:";
			// 
			// TagRangeForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(298, 231);
			this.Controls.Add(this.endtaglabel);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.cancelbutton);
			this.Controls.Add(this.okbutton);
			this.Controls.Add(this.skipdoubletags);
			this.Controls.Add(this.doubletagwarning);
			this.Controls.Add(this.itemcountlabel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.rangestart);
			this.Controls.Add(this.outoftagswarning);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TagRangeForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Tag Selected Range";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox rangestart;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label itemcountlabel;
		private System.Windows.Forms.Label doubletagwarning;
		private System.Windows.Forms.CheckBox skipdoubletags;
		private System.Windows.Forms.Button okbutton;
		private System.Windows.Forms.Button cancelbutton;
		private System.Windows.Forms.Label outoftagswarning;
		private System.Windows.Forms.Label endtaglabel;
		private System.Windows.Forms.Label label4;
	}
}