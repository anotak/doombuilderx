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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.action = new System.Windows.Forms.Label();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.backoffset = new System.Windows.Forms.Label();
            this.unpegged = new System.Windows.Forms.Label();
            this.backoffsetlabel = new System.Windows.Forms.Label();
            this.length = new System.Windows.Forms.Label();
            this.frontoffset = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.frontoffsetlabel = new System.Windows.Forms.Label();
            this.angle = new System.Windows.Forms.Label();
            this.tag = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.arglbl1 = new System.Windows.Forms.Label();
            this.arg1 = new System.Windows.Forms.Label();
            this.arglbl2 = new System.Windows.Forms.Label();
            this.arg5 = new System.Windows.Forms.Label();
            this.arglbl3 = new System.Windows.Forms.Label();
            this.arg4 = new System.Windows.Forms.Label();
            this.arglbl5 = new System.Windows.Forms.Label();
            this.arg3 = new System.Windows.Forms.Label();
            this.arglbl4 = new System.Windows.Forms.Label();
            this.arg2 = new System.Windows.Forms.Label();
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
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.frontpanel.SuspendLayout();
            this.backpanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.Dock = System.Windows.Forms.DockStyle.Fill;
            label1.Location = new System.Drawing.Point(4, 0);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(51, 22);
            label1.TabIndex = 0;
            label1.Text = "Action:";
            // 
            // label4
            // 
            label4.Dock = System.Windows.Forms.DockStyle.Fill;
            label4.Location = new System.Drawing.Point(4, 46);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(56, 23);
            label4.TabIndex = 4;
            label4.Text = "Tag:";
            // 
            // label3
            // 
            label3.Dock = System.Windows.Forms.DockStyle.Fill;
            label3.Location = new System.Drawing.Point(4, 23);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(56, 23);
            label3.TabIndex = 3;
            label3.Text = "Angle:";
            // 
            // label2
            // 
            label2.Dock = System.Windows.Forms.DockStyle.Fill;
            label2.Location = new System.Drawing.Point(4, 0);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(56, 23);
            label2.TabIndex = 2;
            label2.Text = "Length:";
            // 
            // infopanel
            // 
            this.infopanel.Controls.Add(this.tableLayoutPanel1);
            this.infopanel.Location = new System.Drawing.Point(0, -2);
            this.infopanel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.infopanel.Name = "infopanel";
            this.infopanel.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.infopanel.Size = new System.Drawing.Size(576, 107);
            this.infopanel.TabIndex = 1;
            this.infopanel.TabStop = false;
            this.infopanel.Text = " Linedef ";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 62.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.5F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 16);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(568, 91);
            this.tableLayoutPanel1.TabIndex = 30;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel5, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(355, 91);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.61972F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 83.38028F));
            this.tableLayoutPanel4.Controls.Add(label1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.action, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(355, 22);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // action
            // 
            this.action.AutoEllipsis = true;
            this.action.Dock = System.Windows.Forms.DockStyle.Fill;
            this.action.Location = new System.Drawing.Point(63, 0);
            this.action.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.action.Name = "action";
            this.action.Size = new System.Drawing.Size(288, 22);
            this.action.TabIndex = 1;
            this.action.Text = "0 - Big Door that goes Wobbly Wobbly";
            this.action.UseMnemonic = false;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 4;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.33811F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.94556F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.22535F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.04225F));
            this.tableLayoutPanel5.Controls.Add(label2, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.backoffset, 3, 2);
            this.tableLayoutPanel5.Controls.Add(this.unpegged, 3, 0);
            this.tableLayoutPanel5.Controls.Add(this.backoffsetlabel, 2, 2);
            this.tableLayoutPanel5.Controls.Add(this.length, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.frontoffset, 3, 1);
            this.tableLayoutPanel5.Controls.Add(this.label6, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.frontoffsetlabel, 2, 1);
            this.tableLayoutPanel5.Controls.Add(this.angle, 1, 1);
            this.tableLayoutPanel5.Controls.Add(label3, 0, 1);
            this.tableLayoutPanel5.Controls.Add(label4, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.tag, 1, 2);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 22);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 3;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(355, 69);
            this.tableLayoutPanel5.TabIndex = 1;
            // 
            // backoffset
            // 
            this.backoffset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backoffset.Location = new System.Drawing.Point(262, 46);
            this.backoffset.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.backoffset.Name = "backoffset";
            this.backoffset.Size = new System.Drawing.Size(89, 23);
            this.backoffset.TabIndex = 17;
            this.backoffset.Text = "100, 100";
            // 
            // unpegged
            // 
            this.unpegged.Dock = System.Windows.Forms.DockStyle.Fill;
            this.unpegged.Location = new System.Drawing.Point(262, 0);
            this.unpegged.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.unpegged.Name = "unpegged";
            this.unpegged.Size = new System.Drawing.Size(89, 23);
            this.unpegged.TabIndex = 29;
            this.unpegged.Text = "None";
            this.unpegged.UseMnemonic = false;
            // 
            // backoffsetlabel
            // 
            this.backoffsetlabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backoffsetlabel.Location = new System.Drawing.Point(173, 46);
            this.backoffsetlabel.Margin = new System.Windows.Forms.Padding(0);
            this.backoffsetlabel.Name = "backoffsetlabel";
            this.backoffsetlabel.Size = new System.Drawing.Size(85, 23);
            this.backoffsetlabel.TabIndex = 14;
            this.backoffsetlabel.Text = "Back offset:";
            // 
            // length
            // 
            this.length.Dock = System.Windows.Forms.DockStyle.Fill;
            this.length.Location = new System.Drawing.Point(68, 0);
            this.length.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.length.Name = "length";
            this.length.Size = new System.Drawing.Size(101, 23);
            this.length.TabIndex = 5;
            this.length.Text = "1024";
            // 
            // frontoffset
            // 
            this.frontoffset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frontoffset.Location = new System.Drawing.Point(262, 23);
            this.frontoffset.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.frontoffset.Name = "frontoffset";
            this.frontoffset.Size = new System.Drawing.Size(89, 23);
            this.frontoffset.TabIndex = 11;
            this.frontoffset.Text = "100, 100";
            // 
            // label6
            // 
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(173, 0);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 23);
            this.label6.TabIndex = 28;
            this.label6.Text = "Unpegged:";
            // 
            // frontoffsetlabel
            // 
            this.frontoffsetlabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frontoffsetlabel.Location = new System.Drawing.Point(173, 23);
            this.frontoffsetlabel.Margin = new System.Windows.Forms.Padding(0);
            this.frontoffsetlabel.Name = "frontoffsetlabel";
            this.frontoffsetlabel.Size = new System.Drawing.Size(85, 23);
            this.frontoffsetlabel.TabIndex = 8;
            this.frontoffsetlabel.Text = "Front offset:";
            // 
            // angle
            // 
            this.angle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.angle.Location = new System.Drawing.Point(68, 23);
            this.angle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.angle.Name = "angle";
            this.angle.Size = new System.Drawing.Size(101, 23);
            this.angle.TabIndex = 6;
            this.angle.Text = "360";
            // 
            // tag
            // 
            this.tag.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tag.Location = new System.Drawing.Point(68, 46);
            this.tag.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tag.Name = "tag";
            this.tag.Size = new System.Drawing.Size(101, 23);
            this.tag.TabIndex = 7;
            this.tag.Text = "0";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel3.Controls.Add(this.arglbl1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.arg1, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.arglbl2, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.arg5, 1, 4);
            this.tableLayoutPanel3.Controls.Add(this.arglbl3, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.arg4, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.arglbl5, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.arg3, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.arglbl4, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.arg2, 1, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(355, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 5;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(213, 91);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // arglbl1
            // 
            this.arglbl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arglbl1.Location = new System.Drawing.Point(0, 0);
            this.arglbl1.Margin = new System.Windows.Forms.Padding(0);
            this.arglbl1.Name = "arglbl1";
            this.arglbl1.Size = new System.Drawing.Size(149, 18);
            this.arglbl1.TabIndex = 18;
            this.arglbl1.Text = "Arg 1:";
            this.arglbl1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arglbl1.UseMnemonic = false;
            // 
            // arg1
            // 
            this.arg1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arg1.Location = new System.Drawing.Point(149, 0);
            this.arg1.Margin = new System.Windows.Forms.Padding(0);
            this.arg1.Name = "arg1";
            this.arg1.Size = new System.Drawing.Size(64, 18);
            this.arg1.TabIndex = 23;
            this.arg1.Text = "Arg 1:";
            this.arg1.UseMnemonic = false;
            // 
            // arglbl2
            // 
            this.arglbl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arglbl2.Location = new System.Drawing.Point(0, 18);
            this.arglbl2.Margin = new System.Windows.Forms.Padding(0);
            this.arglbl2.Name = "arglbl2";
            this.arglbl2.Size = new System.Drawing.Size(149, 18);
            this.arglbl2.TabIndex = 19;
            this.arglbl2.Text = "Arg 1:";
            this.arglbl2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arglbl2.UseMnemonic = false;
            // 
            // arg5
            // 
            this.arg5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arg5.Location = new System.Drawing.Point(149, 72);
            this.arg5.Margin = new System.Windows.Forms.Padding(0);
            this.arg5.Name = "arg5";
            this.arg5.Size = new System.Drawing.Size(64, 19);
            this.arg5.TabIndex = 27;
            this.arg5.Text = "Arg 1:";
            this.arg5.UseMnemonic = false;
            // 
            // arglbl3
            // 
            this.arglbl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arglbl3.Location = new System.Drawing.Point(0, 36);
            this.arglbl3.Margin = new System.Windows.Forms.Padding(0);
            this.arglbl3.Name = "arglbl3";
            this.arglbl3.Size = new System.Drawing.Size(149, 18);
            this.arglbl3.TabIndex = 20;
            this.arglbl3.Text = "Arg 1:";
            this.arglbl3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arglbl3.UseMnemonic = false;
            // 
            // arg4
            // 
            this.arg4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arg4.Location = new System.Drawing.Point(149, 54);
            this.arg4.Margin = new System.Windows.Forms.Padding(0);
            this.arg4.Name = "arg4";
            this.arg4.Size = new System.Drawing.Size(64, 18);
            this.arg4.TabIndex = 26;
            this.arg4.Text = "Arg 1:";
            this.arg4.UseMnemonic = false;
            // 
            // arglbl5
            // 
            this.arglbl5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arglbl5.Location = new System.Drawing.Point(0, 72);
            this.arglbl5.Margin = new System.Windows.Forms.Padding(0);
            this.arglbl5.Name = "arglbl5";
            this.arglbl5.Size = new System.Drawing.Size(149, 19);
            this.arglbl5.TabIndex = 22;
            this.arglbl5.Text = "Arg 1:";
            this.arglbl5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arglbl5.UseMnemonic = false;
            // 
            // arg3
            // 
            this.arg3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arg3.Location = new System.Drawing.Point(149, 36);
            this.arg3.Margin = new System.Windows.Forms.Padding(0);
            this.arg3.Name = "arg3";
            this.arg3.Size = new System.Drawing.Size(64, 18);
            this.arg3.TabIndex = 25;
            this.arg3.Text = "Arg 1:";
            this.arg3.UseMnemonic = false;
            // 
            // arglbl4
            // 
            this.arglbl4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arglbl4.Location = new System.Drawing.Point(0, 54);
            this.arglbl4.Margin = new System.Windows.Forms.Padding(0);
            this.arglbl4.Name = "arglbl4";
            this.arglbl4.Size = new System.Drawing.Size(149, 18);
            this.arglbl4.TabIndex = 21;
            this.arglbl4.Text = "Arg 1:";
            this.arglbl4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arglbl4.UseMnemonic = false;
            // 
            // arg2
            // 
            this.arg2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arg2.Location = new System.Drawing.Point(149, 18);
            this.arg2.Margin = new System.Windows.Forms.Padding(0);
            this.arg2.Name = "arg2";
            this.arg2.Size = new System.Drawing.Size(64, 18);
            this.arg2.TabIndex = 24;
            this.arg2.Text = "Arg 1:";
            this.arg2.UseMnemonic = false;
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
            this.frontlowname.AutoSize = true;
            this.frontlowname.BackColor = System.Drawing.SystemColors.Control;
            this.frontlowname.Location = new System.Drawing.Point(212, 90);
            this.frontlowname.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.frontlowname.MinimumSize = new System.Drawing.Size(102, 26);
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
            this.fronthighname.AutoSize = true;
            this.fronthighname.BackColor = System.Drawing.SystemColors.Control;
            this.fronthighname.Location = new System.Drawing.Point(8, 90);
            this.fronthighname.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.fronthighname.MinimumSize = new System.Drawing.Size(102, 26);
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
            this.frontmidname.AutoSize = true;
            this.frontmidname.BackColor = System.Drawing.SystemColors.Control;
            this.frontmidname.Location = new System.Drawing.Point(110, 90);
            this.frontmidname.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.frontmidname.MinimumSize = new System.Drawing.Size(102, 26);
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
            this.backlowname.AutoSize = true;
            this.backlowname.BackColor = System.Drawing.SystemColors.Control;
            this.backlowname.Location = new System.Drawing.Point(212, 90);
            this.backlowname.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.backlowname.MinimumSize = new System.Drawing.Size(102, 26);
            this.backlowname.Name = "backlowname";
            this.backlowname.Size = new System.Drawing.Size(102, 26);
            this.backlowname.TabIndex = 5;
            this.backlowname.Text = "BROWNHUG";
            this.backlowname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.backlowname.UseMnemonic = false;
            // 
            // backmidname
            // 
            this.backmidname.AutoSize = true;
            this.backmidname.BackColor = System.Drawing.SystemColors.Control;
            this.backmidname.Location = new System.Drawing.Point(110, 90);
            this.backmidname.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.backmidname.MinimumSize = new System.Drawing.Size(102, 26);
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
            this.backhighname.AutoSize = true;
            this.backhighname.BackColor = System.Drawing.SystemColors.Control;
            this.backhighname.Location = new System.Drawing.Point(8, 90);
            this.backhighname.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.backhighname.MinimumSize = new System.Drawing.Size(102, 26);
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
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.frontpanel.ResumeLayout(false);
            this.backpanel.ResumeLayout(false);
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
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    }
}
