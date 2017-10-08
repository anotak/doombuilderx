namespace CodeImp.DoomBuilder.Controls
{
	partial class LinedefInfoPanel
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label2;
            this.infopanel = new System.Windows.Forms.GroupBox();
            this.action = new System.Windows.Forms.Label();
            this.unpegged = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.arg5 = new System.Windows.Forms.Label();
            this.arglbl5 = new System.Windows.Forms.Label();
            this.arglbl4 = new System.Windows.Forms.Label();
            this.arg4 = new System.Windows.Forms.Label();
            this.arglbl3 = new System.Windows.Forms.Label();
            this.arglbl2 = new System.Windows.Forms.Label();
            this.arg3 = new System.Windows.Forms.Label();
            this.arglbl1 = new System.Windows.Forms.Label();
            this.arg2 = new System.Windows.Forms.Label();
            this.backoffset = new System.Windows.Forms.Label();
            this.arg1 = new System.Windows.Forms.Label();
            this.backoffsetlabel = new System.Windows.Forms.Label();
            this.frontoffset = new System.Windows.Forms.Label();
            this.frontoffsetlabel = new System.Windows.Forms.Label();
            this.tag = new System.Windows.Forms.Label();
            this.angle = new System.Windows.Forms.Label();
            this.length = new System.Windows.Forms.Label();
            this.frontpanel = new System.Windows.Forms.GroupBox();
            this.frontlowtex = new System.Windows.Forms.Panel();
            this.frontsector = new System.Windows.Forms.Label();
            this.frontlowname = new System.Windows.Forms.Label();
            this.frontmidtex = new System.Windows.Forms.Panel();
            this.fronthighname = new System.Windows.Forms.Label();
            this.fronthightex = new System.Windows.Forms.Panel();
            this.frontmidname = new System.Windows.Forms.Label();
            this.backpanel = new System.Windows.Forms.GroupBox();
            this.backlowtex = new System.Windows.Forms.Panel();
            this.backsector = new System.Windows.Forms.Label();
            this.backlowname = new System.Windows.Forms.Label();
            this.backmidname = new System.Windows.Forms.Label();
            this.backmidtex = new System.Windows.Forms.Panel();
            this.backhighname = new System.Windows.Forms.Label();
            this.backhightex = new System.Windows.Forms.Panel();
            label1 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.infopanel.SuspendLayout();
            this.frontpanel.SuspendLayout();
            this.backpanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(14, 14);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(51, 16);
            label1.TabIndex = 0;
            label1.Text = "Action:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(30, 86);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(35, 16);
            label4.TabIndex = 4;
            label4.Text = "Tag:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(18, 62);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(48, 16);
            label3.TabIndex = 3;
            label3.Text = "Angle:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(11, 39);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(56, 16);
            label2.TabIndex = 2;
            label2.Text = "Length:";
            // 
            // infopanel
            // 
            this.infopanel.Controls.Add(this.unpegged);
            this.infopanel.Controls.Add(this.label6);
            this.infopanel.Controls.Add(this.arg5);
            this.infopanel.Controls.Add(this.arglbl5);
            this.infopanel.Controls.Add(this.arglbl4);
            this.infopanel.Controls.Add(this.arg4);
            this.infopanel.Controls.Add(this.arglbl3);
            this.infopanel.Controls.Add(this.arglbl2);
            this.infopanel.Controls.Add(this.arg3);
            this.infopanel.Controls.Add(this.arglbl1);
            this.infopanel.Controls.Add(this.arg2);
            this.infopanel.Controls.Add(this.backoffset);
            this.infopanel.Controls.Add(this.arg1);
            this.infopanel.Controls.Add(this.backoffsetlabel);
            this.infopanel.Controls.Add(this.frontoffset);
            this.infopanel.Controls.Add(this.frontoffsetlabel);
            this.infopanel.Controls.Add(this.tag);
            this.infopanel.Controls.Add(this.angle);
            this.infopanel.Controls.Add(this.length);
            this.infopanel.Controls.Add(label4);
            this.infopanel.Controls.Add(label3);
            this.infopanel.Controls.Add(label2);
            this.infopanel.Controls.Add(label1);
            this.infopanel.Controls.Add(this.action);
            this.infopanel.Location = new System.Drawing.Point(0, 0);
            this.infopanel.Margin = new System.Windows.Forms.Padding(4);
            this.infopanel.Name = "infopanel";
            this.infopanel.Padding = new System.Windows.Forms.Padding(4);
            this.infopanel.Size = new System.Drawing.Size(576, 105);
            this.infopanel.TabIndex = 1;
            this.infopanel.TabStop = false;
            this.infopanel.Text = " Linedef ";
            // 
            // action
            // 
            this.action.BackColor = System.Drawing.Color.Transparent;
            this.action.Location = new System.Drawing.Point(60, 14);
            this.action.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.action.Name = "action";
            this.action.Size = new System.Drawing.Size(500, 18);
            this.action.TabIndex = 1;
            this.action.Text = "0 - Big Door that goes Wobbly Wobbly";
            this.action.UseMnemonic = false;
            // 
            // unpegged
            // 
            this.unpegged.AutoSize = true;
            this.unpegged.Location = new System.Drawing.Point(225, 39);
            this.unpegged.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.unpegged.Name = "unpegged";
            this.unpegged.Size = new System.Drawing.Size(41, 16);
            this.unpegged.TabIndex = 29;
            this.unpegged.Text = "None";
            this.unpegged.UseMnemonic = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(148, 39);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 16);
            this.label6.TabIndex = 28;
            this.label6.Text = "Unpegged:";
            // 
            // arg5
            // 
            this.arg5.AutoEllipsis = true;
            this.arg5.Location = new System.Drawing.Point(466, 89);
            this.arg5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arg5.Name = "arg5";
            this.arg5.Size = new System.Drawing.Size(104, 18);
            this.arg5.TabIndex = 27;
            this.arg5.Text = "Arg 1:";
            this.arg5.UseMnemonic = false;
            // 
            // arglbl5
            // 
            this.arglbl5.AutoEllipsis = true;
            this.arglbl5.BackColor = System.Drawing.Color.Transparent;
            this.arglbl5.Location = new System.Drawing.Point(308, 89);
            this.arglbl5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arglbl5.Name = "arglbl5";
            this.arglbl5.Size = new System.Drawing.Size(151, 18);
            this.arglbl5.TabIndex = 22;
            this.arglbl5.Text = "Arg 1:";
            this.arglbl5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arglbl5.UseMnemonic = false;
            // 
            // arglbl4
            // 
            this.arglbl4.AutoEllipsis = true;
            this.arglbl4.BackColor = System.Drawing.Color.Transparent;
            this.arglbl4.Location = new System.Drawing.Point(308, 70);
            this.arglbl4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arglbl4.Name = "arglbl4";
            this.arglbl4.Size = new System.Drawing.Size(151, 18);
            this.arglbl4.TabIndex = 21;
            this.arglbl4.Text = "Arg 1:";
            this.arglbl4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arglbl4.UseMnemonic = false;
            // 
            // arg4
            // 
            this.arg4.AutoEllipsis = true;
            this.arg4.Location = new System.Drawing.Point(466, 70);
            this.arg4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arg4.Name = "arg4";
            this.arg4.Size = new System.Drawing.Size(104, 18);
            this.arg4.TabIndex = 26;
            this.arg4.Text = "Arg 1:";
            this.arg4.UseMnemonic = false;
            // 
            // arglbl3
            // 
            this.arglbl3.AutoEllipsis = true;
            this.arglbl3.BackColor = System.Drawing.Color.Transparent;
            this.arglbl3.Location = new System.Drawing.Point(308, 51);
            this.arglbl3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arglbl3.Name = "arglbl3";
            this.arglbl3.Size = new System.Drawing.Size(151, 18);
            this.arglbl3.TabIndex = 20;
            this.arglbl3.Text = "Arg 1:";
            this.arglbl3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arglbl3.UseMnemonic = false;
            // 
            // arglbl2
            // 
            this.arglbl2.AutoEllipsis = true;
            this.arglbl2.BackColor = System.Drawing.Color.Transparent;
            this.arglbl2.Location = new System.Drawing.Point(308, 32);
            this.arglbl2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arglbl2.Name = "arglbl2";
            this.arglbl2.Size = new System.Drawing.Size(151, 18);
            this.arglbl2.TabIndex = 19;
            this.arglbl2.Text = "Arg 1:";
            this.arglbl2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arglbl2.UseMnemonic = false;
            // 
            // arg3
            // 
            this.arg3.AutoEllipsis = true;
            this.arg3.Location = new System.Drawing.Point(466, 51);
            this.arg3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arg3.Name = "arg3";
            this.arg3.Size = new System.Drawing.Size(104, 18);
            this.arg3.TabIndex = 25;
            this.arg3.Text = "Arg 1:";
            this.arg3.UseMnemonic = false;
            // 
            // arglbl1
            // 
            this.arglbl1.AutoEllipsis = true;
            this.arglbl1.BackColor = System.Drawing.Color.Transparent;
            this.arglbl1.Location = new System.Drawing.Point(308, 14);
            this.arglbl1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arglbl1.Name = "arglbl1";
            this.arglbl1.Size = new System.Drawing.Size(151, 18);
            this.arglbl1.TabIndex = 18;
            this.arglbl1.Text = "Arg 1:";
            this.arglbl1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arglbl1.UseMnemonic = false;
            // 
            // arg2
            // 
            this.arg2.AutoEllipsis = true;
            this.arg2.Location = new System.Drawing.Point(466, 32);
            this.arg2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arg2.Name = "arg2";
            this.arg2.Size = new System.Drawing.Size(104, 18);
            this.arg2.TabIndex = 24;
            this.arg2.Text = "Arg 1:";
            this.arg2.UseMnemonic = false;
            // 
            // backoffset
            // 
            this.backoffset.AutoSize = true;
            this.backoffset.Location = new System.Drawing.Point(225, 86);
            this.backoffset.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.backoffset.Name = "backoffset";
            this.backoffset.Size = new System.Drawing.Size(64, 16);
            this.backoffset.TabIndex = 17;
            this.backoffset.Text = "100, 100";
            // 
            // arg1
            // 
            this.arg1.AutoEllipsis = true;
            this.arg1.Location = new System.Drawing.Point(466, 14);
            this.arg1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arg1.Name = "arg1";
            this.arg1.Size = new System.Drawing.Size(104, 18);
            this.arg1.TabIndex = 23;
            this.arg1.Text = "Arg 1:";
            this.arg1.UseMnemonic = false;
            // 
            // backoffsetlabel
            // 
            this.backoffsetlabel.AutoSize = true;
            this.backoffsetlabel.Location = new System.Drawing.Point(139, 86);
            this.backoffsetlabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.backoffsetlabel.Name = "backoffsetlabel";
            this.backoffsetlabel.Size = new System.Drawing.Size(82, 16);
            this.backoffsetlabel.TabIndex = 14;
            this.backoffsetlabel.Text = "Back offset:";
            // 
            // frontoffset
            // 
            this.frontoffset.AutoSize = true;
            this.frontoffset.Location = new System.Drawing.Point(225, 62);
            this.frontoffset.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.frontoffset.Name = "frontoffset";
            this.frontoffset.Size = new System.Drawing.Size(64, 16);
            this.frontoffset.TabIndex = 11;
            this.frontoffset.Text = "100, 100";
            // 
            // frontoffsetlabel
            // 
            this.frontoffsetlabel.AutoSize = true;
            this.frontoffsetlabel.Location = new System.Drawing.Point(138, 62);
            this.frontoffsetlabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.frontoffsetlabel.Name = "frontoffsetlabel";
            this.frontoffsetlabel.Size = new System.Drawing.Size(85, 16);
            this.frontoffsetlabel.TabIndex = 8;
            this.frontoffsetlabel.Text = "Front offset:";
            // 
            // tag
            // 
            this.tag.AutoSize = true;
            this.tag.Location = new System.Drawing.Point(69, 86);
            this.tag.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tag.Name = "tag";
            this.tag.Size = new System.Drawing.Size(16, 16);
            this.tag.TabIndex = 7;
            this.tag.Text = "0";
            // 
            // angle
            // 
            this.angle.AutoSize = true;
            this.angle.Location = new System.Drawing.Point(69, 62);
            this.angle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.angle.Name = "angle";
            this.angle.Size = new System.Drawing.Size(32, 16);
            this.angle.TabIndex = 6;
            this.angle.Text = "360";
            // 
            // length
            // 
            this.length.AutoSize = true;
            this.length.Location = new System.Drawing.Point(69, 39);
            this.length.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.length.Name = "length";
            this.length.Size = new System.Drawing.Size(40, 16);
            this.length.TabIndex = 5;
            this.length.Text = "1024";
            // 
            // frontpanel
            // 
            this.frontpanel.Controls.Add(this.frontlowtex);
            this.frontpanel.Controls.Add(this.frontsector);
            this.frontpanel.Controls.Add(this.frontlowname);
            this.frontpanel.Controls.Add(this.frontmidtex);
            this.frontpanel.Controls.Add(this.fronthighname);
            this.frontpanel.Controls.Add(this.fronthightex);
            this.frontpanel.Controls.Add(this.frontmidname);
            this.frontpanel.Location = new System.Drawing.Point(584, 0);
            this.frontpanel.Margin = new System.Windows.Forms.Padding(4);
            this.frontpanel.Name = "frontpanel";
            this.frontpanel.Padding = new System.Windows.Forms.Padding(4);
            this.frontpanel.Size = new System.Drawing.Size(321, 105);
            this.frontpanel.TabIndex = 2;
            this.frontpanel.TabStop = false;
            this.frontpanel.Text = " Front ";
            // 
            // frontlowtex
            // 
            this.frontlowtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.frontlowtex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.frontlowtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.frontlowtex.Location = new System.Drawing.Point(221, 14);
            this.frontlowtex.Margin = new System.Windows.Forms.Padding(4);
            this.frontlowtex.Name = "frontlowtex";
            this.frontlowtex.Size = new System.Drawing.Size(84, 74);
            this.frontlowtex.TabIndex = 4;
            // 
            // frontsector
            // 
            this.frontsector.AutoSize = true;
            this.frontsector.BackColor = System.Drawing.SystemColors.Control;
            this.frontsector.Location = new System.Drawing.Point(232, 0);
            this.frontsector.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.frontsector.Name = "frontsector";
            this.frontsector.Size = new System.Drawing.Size(77, 16);
            this.frontsector.TabIndex = 6;
            this.frontsector.Text = "Sector 666";
            // 
            // frontlowname
            // 
            this.frontlowname.BackColor = System.Drawing.SystemColors.Control;
            this.frontlowname.Location = new System.Drawing.Point(212, 90);
            this.frontlowname.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.frontlowname.Name = "frontlowname";
            this.frontlowname.Size = new System.Drawing.Size(102, 26);
            this.frontlowname.TabIndex = 5;
            this.frontlowname.Text = "BROWNHUG";
            this.frontlowname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.frontlowname.UseMnemonic = false;
            // 
            // frontmidtex
            // 
            this.frontmidtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.frontmidtex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.frontmidtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.frontmidtex.Location = new System.Drawing.Point(119, 14);
            this.frontmidtex.Margin = new System.Windows.Forms.Padding(4);
            this.frontmidtex.Name = "frontmidtex";
            this.frontmidtex.Size = new System.Drawing.Size(84, 74);
            this.frontmidtex.TabIndex = 2;
            // 
            // fronthighname
            // 
            this.fronthighname.BackColor = System.Drawing.SystemColors.Control;
            this.fronthighname.Location = new System.Drawing.Point(8, 90);
            this.fronthighname.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.fronthighname.Name = "fronthighname";
            this.fronthighname.Size = new System.Drawing.Size(102, 26);
            this.fronthighname.TabIndex = 1;
            this.fronthighname.Text = "BROWNHUG";
            this.fronthighname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.fronthighname.UseMnemonic = false;
            // 
            // fronthightex
            // 
            this.fronthightex.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.fronthightex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.fronthightex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.fronthightex.Location = new System.Drawing.Point(16, 14);
            this.fronthightex.Margin = new System.Windows.Forms.Padding(4);
            this.fronthightex.Name = "fronthightex";
            this.fronthightex.Size = new System.Drawing.Size(84, 74);
            this.fronthightex.TabIndex = 0;
            // 
            // frontmidname
            // 
            this.frontmidname.BackColor = System.Drawing.SystemColors.Control;
            this.frontmidname.Location = new System.Drawing.Point(110, 90);
            this.frontmidname.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.frontmidname.Name = "frontmidname";
            this.frontmidname.Size = new System.Drawing.Size(102, 26);
            this.frontmidname.TabIndex = 3;
            this.frontmidname.Text = "BROWNHUG";
            this.frontmidname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.frontmidname.UseMnemonic = false;
            // 
            // backpanel
            // 
            this.backpanel.Controls.Add(this.backlowtex);
            this.backpanel.Controls.Add(this.backsector);
            this.backpanel.Controls.Add(this.backlowname);
            this.backpanel.Controls.Add(this.backmidname);
            this.backpanel.Controls.Add(this.backmidtex);
            this.backpanel.Controls.Add(this.backhighname);
            this.backpanel.Controls.Add(this.backhightex);
            this.backpanel.Location = new System.Drawing.Point(912, 0);
            this.backpanel.Margin = new System.Windows.Forms.Padding(4);
            this.backpanel.Name = "backpanel";
            this.backpanel.Padding = new System.Windows.Forms.Padding(4);
            this.backpanel.Size = new System.Drawing.Size(321, 105);
            this.backpanel.TabIndex = 3;
            this.backpanel.TabStop = false;
            this.backpanel.Text = " Back ";
            // 
            // backlowtex
            // 
            this.backlowtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.backlowtex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.backlowtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.backlowtex.Location = new System.Drawing.Point(221, 14);
            this.backlowtex.Margin = new System.Windows.Forms.Padding(4);
            this.backlowtex.Name = "backlowtex";
            this.backlowtex.Size = new System.Drawing.Size(84, 74);
            this.backlowtex.TabIndex = 4;
            // 
            // backsector
            // 
            this.backsector.AutoSize = true;
            this.backsector.BackColor = System.Drawing.SystemColors.Control;
            this.backsector.Location = new System.Drawing.Point(232, 0);
            this.backsector.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.backsector.Name = "backsector";
            this.backsector.Size = new System.Drawing.Size(77, 16);
            this.backsector.TabIndex = 7;
            this.backsector.Text = "Sector 666";
            // 
            // backlowname
            // 
            this.backlowname.BackColor = System.Drawing.SystemColors.Control;
            this.backlowname.Location = new System.Drawing.Point(212, 90);
            this.backlowname.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.backlowname.Name = "backlowname";
            this.backlowname.Size = new System.Drawing.Size(102, 26);
            this.backlowname.TabIndex = 5;
            this.backlowname.Text = "BROWNHUG";
            this.backlowname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.backlowname.UseMnemonic = false;
            // 
            // backmidname
            // 
            this.backmidname.BackColor = System.Drawing.SystemColors.Control;
            this.backmidname.Location = new System.Drawing.Point(110, 90);
            this.backmidname.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.backmidname.Name = "backmidname";
            this.backmidname.Size = new System.Drawing.Size(102, 26);
            this.backmidname.TabIndex = 3;
            this.backmidname.Text = "BROWNHUG";
            this.backmidname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.backmidname.UseMnemonic = false;
            // 
            // backmidtex
            // 
            this.backmidtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.backmidtex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.backmidtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.backmidtex.Location = new System.Drawing.Point(119, 14);
            this.backmidtex.Margin = new System.Windows.Forms.Padding(4);
            this.backmidtex.Name = "backmidtex";
            this.backmidtex.Size = new System.Drawing.Size(84, 74);
            this.backmidtex.TabIndex = 2;
            // 
            // backhighname
            // 
            this.backhighname.BackColor = System.Drawing.SystemColors.Control;
            this.backhighname.Location = new System.Drawing.Point(8, 90);
            this.backhighname.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.backhighname.Name = "backhighname";
            this.backhighname.Size = new System.Drawing.Size(102, 26);
            this.backhighname.TabIndex = 1;
            this.backhighname.Text = "BROWNHUG";
            this.backhighname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.backhighname.UseMnemonic = false;
            // 
            // backhightex
            // 
            this.backhightex.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.backhightex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.backhightex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.backhightex.Location = new System.Drawing.Point(16, 14);
            this.backhightex.Margin = new System.Windows.Forms.Padding(4);
            this.backhightex.Name = "backhightex";
            this.backhightex.Size = new System.Drawing.Size(84, 74);
            this.backhightex.TabIndex = 0;
            // 
            // LinedefInfoPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.backpanel);
            this.Controls.Add(this.frontpanel);
            this.Controls.Add(this.infopanel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(12500, 105);
            this.MinimumSize = new System.Drawing.Size(125, 105);
            this.Name = "LinedefInfoPanel";
            this.Size = new System.Drawing.Size(1309, 105);
            this.infopanel.ResumeLayout(false);
            this.infopanel.PerformLayout();
            this.frontpanel.ResumeLayout(false);
            this.frontpanel.PerformLayout();
            this.backpanel.ResumeLayout(false);
            this.backpanel.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label action;
		private System.Windows.Forms.Label tag;
		private System.Windows.Forms.Label angle;
		private System.Windows.Forms.Label length;
		private System.Windows.Forms.Label frontoffset;
		private System.Windows.Forms.Label backoffset;
		private System.Windows.Forms.Panel fronthightex;
		private System.Windows.Forms.Label frontlowname;
		private System.Windows.Forms.Panel frontlowtex;
		private System.Windows.Forms.Label frontmidname;
		private System.Windows.Forms.Panel frontmidtex;
		private System.Windows.Forms.Label fronthighname;
		private System.Windows.Forms.Label backlowname;
		private System.Windows.Forms.Panel backlowtex;
		private System.Windows.Forms.Label backmidname;
		private System.Windows.Forms.Panel backmidtex;
		private System.Windows.Forms.Label backhighname;
		private System.Windows.Forms.Panel backhightex;
		private System.Windows.Forms.GroupBox frontpanel;
		private System.Windows.Forms.GroupBox backpanel;
		private System.Windows.Forms.Label backoffsetlabel;
		private System.Windows.Forms.Label frontoffsetlabel;
		private System.Windows.Forms.Label arglbl5;
		private System.Windows.Forms.Label arglbl4;
		private System.Windows.Forms.Label arglbl3;
		private System.Windows.Forms.Label arglbl2;
		private System.Windows.Forms.Label arglbl1;
		private System.Windows.Forms.Label arg5;
		private System.Windows.Forms.Label arg4;
		private System.Windows.Forms.Label arg3;
		private System.Windows.Forms.Label arg2;
		private System.Windows.Forms.Label arg1;
		private System.Windows.Forms.GroupBox infopanel;
		private System.Windows.Forms.Label unpegged;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label frontsector;
		private System.Windows.Forms.Label backsector;

	}
}
