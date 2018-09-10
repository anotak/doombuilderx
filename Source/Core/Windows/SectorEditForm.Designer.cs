namespace CodeImp.DoomBuilder.Windows
{
	partial class SectorEditForm
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.GroupBox groupaction;
            System.Windows.Forms.Label taglabel;
            System.Windows.Forms.GroupBox groupeffect;
            System.Windows.Forms.Label label9;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.GroupBox groupfloorceiling;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label6;
            this.tag = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.newtag = new System.Windows.Forms.Button();
            this.brightness = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.browseeffect = new System.Windows.Forms.Button();
            this.effect = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
            this.floorheight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.ceilingheight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.sectorheight = new System.Windows.Forms.Label();
            this.sectorheightlabel = new System.Windows.Forms.Label();
            this.floortex = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
            this.ceilingtex = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
            this.cancel = new System.Windows.Forms.Button();
            this.apply = new System.Windows.Forms.Button();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabproperties = new System.Windows.Forms.TabPage();
            this.tabcustom = new System.Windows.Forms.TabPage();
            this.fieldslist = new CodeImp.DoomBuilder.Controls.FieldsEditorControl();
            this.flatSelectorControl2 = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
            this.flatSelectorControl1 = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
            label1 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            groupaction = new System.Windows.Forms.GroupBox();
            taglabel = new System.Windows.Forms.Label();
            groupeffect = new System.Windows.Forms.GroupBox();
            label9 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            groupfloorceiling = new System.Windows.Forms.GroupBox();
            label5 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            groupaction.SuspendLayout();
            groupeffect.SuspendLayout();
            groupfloorceiling.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabproperties.SuspendLayout();
            this.tabcustom.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(271, 18);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(83, 16);
            label1.TabIndex = 15;
            label1.Text = "Floor";
            label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            label3.Location = new System.Drawing.Point(363, 18);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(83, 16);
            label3.TabIndex = 14;
            label3.Text = "Ceiling";
            label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // groupaction
            // 
            groupaction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            groupaction.Controls.Add(this.tag);
            groupaction.Controls.Add(taglabel);
            groupaction.Controls.Add(this.newtag);
            groupaction.Location = new System.Drawing.Point(9, 362);
            groupaction.Margin = new System.Windows.Forms.Padding(4);
            groupaction.Name = "groupaction";
            groupaction.Padding = new System.Windows.Forms.Padding(4);
            groupaction.Size = new System.Drawing.Size(545, 89);
            groupaction.TabIndex = 2;
            groupaction.TabStop = false;
            groupaction.Text = " Identification ";
            // 
            // tag
            // 
            this.tag.AllowDecimal = false;
            this.tag.AllowNegative = false;
            this.tag.AllowRelative = true;
            this.tag.ButtonStep = 1;
            this.tag.Location = new System.Drawing.Point(111, 32);
            this.tag.Margin = new System.Windows.Forms.Padding(5);
            this.tag.Name = "tag";
            this.tag.Size = new System.Drawing.Size(91, 27);
            this.tag.StepValues = null;
            this.tag.TabIndex = 25;
            // 
            // taglabel
            // 
            taglabel.AutoSize = true;
            taglabel.Location = new System.Drawing.Point(69, 39);
            taglabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            taglabel.Name = "taglabel";
            taglabel.Size = new System.Drawing.Size(35, 16);
            taglabel.TabIndex = 9;
            taglabel.Text = "Tag:";
            // 
            // newtag
            // 
            this.newtag.Location = new System.Drawing.Point(218, 34);
            this.newtag.Margin = new System.Windows.Forms.Padding(4);
            this.newtag.Name = "newtag";
            this.newtag.Size = new System.Drawing.Size(95, 29);
            this.newtag.TabIndex = 1;
            this.newtag.Text = "New Tag";
            this.newtag.UseVisualStyleBackColor = true;
            this.newtag.Click += new System.EventHandler(this.newtag_Click);
            // 
            // groupeffect
            // 
            groupeffect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            groupeffect.Controls.Add(this.brightness);
            groupeffect.Controls.Add(this.browseeffect);
            groupeffect.Controls.Add(label9);
            groupeffect.Controls.Add(this.effect);
            groupeffect.Controls.Add(label8);
            groupeffect.Location = new System.Drawing.Point(9, 220);
            groupeffect.Margin = new System.Windows.Forms.Padding(4);
            groupeffect.Name = "groupeffect";
            groupeffect.Padding = new System.Windows.Forms.Padding(4);
            groupeffect.Size = new System.Drawing.Size(545, 131);
            groupeffect.TabIndex = 1;
            groupeffect.TabStop = false;
            groupeffect.Text = " Effects ";
            // 
            // brightness
            // 
            this.brightness.AllowDecimal = false;
            this.brightness.AllowNegative = true;
            this.brightness.AllowRelative = true;
            this.brightness.ButtonStep = 8;
            this.brightness.Location = new System.Drawing.Point(111, 76);
            this.brightness.Margin = new System.Windows.Forms.Padding(5);
            this.brightness.Name = "brightness";
            this.brightness.Size = new System.Drawing.Size(91, 27);
            this.brightness.StepValues = null;
            this.brightness.TabIndex = 24;
            // 
            // browseeffect
            // 
            this.browseeffect.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.browseeffect.Image = global::CodeImp.DoomBuilder.Properties.Resources.List;
            this.browseeffect.Location = new System.Drawing.Point(481, 32);
            this.browseeffect.Margin = new System.Windows.Forms.Padding(4);
            this.browseeffect.Name = "browseeffect";
            this.browseeffect.Padding = new System.Windows.Forms.Padding(0, 0, 1, 4);
            this.browseeffect.Size = new System.Drawing.Size(35, 31);
            this.browseeffect.TabIndex = 1;
            this.browseeffect.Text = " ";
            this.browseeffect.UseVisualStyleBackColor = true;
            this.browseeffect.Click += new System.EventHandler(this.browseeffect_Click);
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(26, 82);
            label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(79, 16);
            label9.TabIndex = 2;
            label9.Text = "Brightness:";
            // 
            // effect
            // 
            this.effect.BackColor = System.Drawing.Color.Transparent;
            this.effect.Cursor = System.Windows.Forms.Cursors.Default;
            this.effect.Empty = false;
            this.effect.GeneralizedCategories = null;
            this.effect.Location = new System.Drawing.Point(111, 35);
            this.effect.Margin = new System.Windows.Forms.Padding(5);
            this.effect.Name = "effect";
            this.effect.Size = new System.Drawing.Size(362, 24);
            this.effect.TabIndex = 0;
            this.effect.Value = 402;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(48, 39);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(58, 16);
            label8.TabIndex = 0;
            label8.Text = "Special:";
            // 
            // groupfloorceiling
            // 
            groupfloorceiling.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            groupfloorceiling.Controls.Add(this.floorheight);
            groupfloorceiling.Controls.Add(this.ceilingheight);
            groupfloorceiling.Controls.Add(this.sectorheight);
            groupfloorceiling.Controls.Add(this.sectorheightlabel);
            groupfloorceiling.Controls.Add(label5);
            groupfloorceiling.Controls.Add(label2);
            groupfloorceiling.Controls.Add(label4);
            groupfloorceiling.Controls.Add(this.floortex);
            groupfloorceiling.Controls.Add(this.ceilingtex);
            groupfloorceiling.Controls.Add(label6);
            groupfloorceiling.Location = new System.Drawing.Point(9, 8);
            groupfloorceiling.Margin = new System.Windows.Forms.Padding(4);
            groupfloorceiling.Name = "groupfloorceiling";
            groupfloorceiling.Padding = new System.Windows.Forms.Padding(4);
            groupfloorceiling.Size = new System.Drawing.Size(545, 201);
            groupfloorceiling.TabIndex = 0;
            groupfloorceiling.TabStop = false;
            groupfloorceiling.Text = "Floor and Ceiling ";
            // 
            // floorheight
            // 
            this.floorheight.AllowDecimal = false;
            this.floorheight.AllowNegative = true;
            this.floorheight.AllowRelative = true;
            this.floorheight.ButtonStep = 8;
            this.floorheight.Location = new System.Drawing.Point(140, 86);
            this.floorheight.Margin = new System.Windows.Forms.Padding(5);
            this.floorheight.Name = "floorheight";
            this.floorheight.Size = new System.Drawing.Size(110, 27);
            this.floorheight.StepValues = null;
            this.floorheight.TabIndex = 23;
            this.floorheight.WhenTextChanged += new System.EventHandler(this.floorheight_TextChanged);
            // 
            // ceilingheight
            // 
            this.ceilingheight.AllowDecimal = false;
            this.ceilingheight.AllowNegative = true;
            this.ceilingheight.AllowRelative = true;
            this.ceilingheight.ButtonStep = 8;
            this.ceilingheight.Location = new System.Drawing.Point(140, 44);
            this.ceilingheight.Margin = new System.Windows.Forms.Padding(5);
            this.ceilingheight.Name = "ceilingheight";
            this.ceilingheight.Size = new System.Drawing.Size(110, 27);
            this.ceilingheight.StepValues = null;
            this.ceilingheight.TabIndex = 22;
            this.ceilingheight.WhenTextChanged += new System.EventHandler(this.ceilingheight_TextChanged);
            // 
            // sectorheight
            // 
            this.sectorheight.AutoSize = true;
            this.sectorheight.Location = new System.Drawing.Point(141, 136);
            this.sectorheight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.sectorheight.Name = "sectorheight";
            this.sectorheight.Size = new System.Drawing.Size(16, 16);
            this.sectorheight.TabIndex = 21;
            this.sectorheight.Text = "0";
            // 
            // sectorheightlabel
            // 
            this.sectorheightlabel.AutoSize = true;
            this.sectorheightlabel.Location = new System.Drawing.Point(40, 136);
            this.sectorheightlabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.sectorheightlabel.Name = "sectorheightlabel";
            this.sectorheightlabel.Size = new System.Drawing.Size(96, 16);
            this.sectorheightlabel.TabIndex = 20;
            this.sectorheightlabel.Text = "Sector height:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(50, 92);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(88, 16);
            label5.TabIndex = 17;
            label5.Text = "Floor height:";
            // 
            // label2
            // 
            label2.Location = new System.Drawing.Point(296, 22);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(104, 20);
            label2.TabIndex = 15;
            label2.Text = "Floor";
            label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(415, 22);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(104, 20);
            label4.TabIndex = 14;
            label4.Text = "Ceiling";
            label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // floortex
            // 
            this.floortex.Location = new System.Drawing.Point(296, 46);
            this.floortex.Margin = new System.Windows.Forms.Padding(5);
            this.floortex.Name = "floortex";
            this.floortex.Size = new System.Drawing.Size(104, 131);
            this.floortex.TabIndex = 2;
            this.floortex.TextureName = "";
            // 
            // ceilingtex
            // 
            this.ceilingtex.Location = new System.Drawing.Point(415, 46);
            this.ceilingtex.Margin = new System.Windows.Forms.Padding(5);
            this.ceilingtex.Name = "ceilingtex";
            this.ceilingtex.Size = new System.Drawing.Size(104, 131);
            this.ceilingtex.TabIndex = 3;
            this.ceilingtex.TextureName = "";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(41, 50);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(98, 16);
            label6.TabIndex = 19;
            label6.Text = "Ceiling height:";
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(444, 529);
            this.cancel.Margin = new System.Windows.Forms.Padding(4);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(140, 31);
            this.cancel.TabIndex = 2;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // apply
            // 
            this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.apply.Location = new System.Drawing.Point(295, 529);
            this.apply.Margin = new System.Windows.Forms.Padding(4);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(140, 31);
            this.apply.TabIndex = 1;
            this.apply.Text = "OK";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Controls.Add(this.tabproperties);
            this.tabs.Controls.Add(this.tabcustom);
            this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Margin = new System.Windows.Forms.Padding(1);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(571, 495);
            this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabs.TabIndex = 0;
            // 
            // tabproperties
            // 
            this.tabproperties.Controls.Add(groupaction);
            this.tabproperties.Controls.Add(groupeffect);
            this.tabproperties.Controls.Add(groupfloorceiling);
            this.tabproperties.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabproperties.Location = new System.Drawing.Point(4, 25);
            this.tabproperties.Margin = new System.Windows.Forms.Padding(4);
            this.tabproperties.Name = "tabproperties";
            this.tabproperties.Padding = new System.Windows.Forms.Padding(4);
            this.tabproperties.Size = new System.Drawing.Size(563, 466);
            this.tabproperties.TabIndex = 0;
            this.tabproperties.Text = "Properties";
            this.tabproperties.UseVisualStyleBackColor = true;
            // 
            // tabcustom
            // 
            this.tabcustom.Controls.Add(this.fieldslist);
            this.tabcustom.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabcustom.Location = new System.Drawing.Point(4, 25);
            this.tabcustom.Margin = new System.Windows.Forms.Padding(4);
            this.tabcustom.Name = "tabcustom";
            this.tabcustom.Padding = new System.Windows.Forms.Padding(4);
            this.tabcustom.Size = new System.Drawing.Size(563, 466);
            this.tabcustom.TabIndex = 1;
            this.tabcustom.Text = "Custom";
            this.tabcustom.UseVisualStyleBackColor = true;
            // 
            // fieldslist
            // 
            this.fieldslist.AllowInsert = true;
            this.fieldslist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fieldslist.AutoInsertUserPrefix = true;
            this.fieldslist.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.fieldslist.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fieldslist.Location = new System.Drawing.Point(14, 14);
            this.fieldslist.Margin = new System.Windows.Forms.Padding(10);
            this.fieldslist.Name = "fieldslist";
            this.fieldslist.PropertyColumnVisible = true;
            this.fieldslist.PropertyColumnWidth = 150;
            this.fieldslist.Size = new System.Drawing.Size(533, 433);
            this.fieldslist.TabIndex = 1;
            this.fieldslist.TypeColumnVisible = true;
            this.fieldslist.TypeColumnWidth = 100;
            this.fieldslist.ValueColumnVisible = true;
            // 
            // flatSelectorControl2
            // 
            this.flatSelectorControl2.Location = new System.Drawing.Point(271, 37);
            this.flatSelectorControl2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flatSelectorControl2.Name = "flatSelectorControl2";
            this.flatSelectorControl2.Size = new System.Drawing.Size(83, 105);
            this.flatSelectorControl2.TabIndex = 13;
            this.flatSelectorControl2.TextureName = "";
            // 
            // flatSelectorControl1
            // 
            this.flatSelectorControl1.Location = new System.Drawing.Point(363, 37);
            this.flatSelectorControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flatSelectorControl1.Name = "flatSelectorControl1";
            this.flatSelectorControl1.Size = new System.Drawing.Size(83, 105);
            this.flatSelectorControl1.TabIndex = 12;
            this.flatSelectorControl1.TextureName = "";
            // 
            // SectorEditForm
            // 
            this.AcceptButton = this.apply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(596, 572);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.apply);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SectorEditForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Sector";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.SectorEditForm_HelpRequested);
            groupaction.ResumeLayout(false);
            groupaction.PerformLayout();
            groupeffect.ResumeLayout(false);
            groupeffect.PerformLayout();
            groupfloorceiling.ResumeLayout(false);
            groupfloorceiling.PerformLayout();
            this.tabs.ResumeLayout(false);
            this.tabproperties.ResumeLayout(false);
            this.tabcustom.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabproperties;
		private System.Windows.Forms.TabPage tabcustom;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl floortex;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl ceilingtex;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl flatSelectorControl2;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl flatSelectorControl1;
		private CodeImp.DoomBuilder.Controls.FieldsEditorControl fieldslist;
		private System.Windows.Forms.Label sectorheight;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl effect;
		private System.Windows.Forms.Button newtag;
		private System.Windows.Forms.Button browseeffect;
		private System.Windows.Forms.Label sectorheightlabel;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox ceilingheight;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox floorheight;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox brightness;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox tag;
	}
}