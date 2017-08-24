namespace CodeImp.DoomBuilder.Controls
{
	partial class ThingBrowserControl
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Monsters");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThingBrowserControl));
            this.sizecaption = new System.Windows.Forms.Label();
            this.blockingcaption = new System.Windows.Forms.Label();
            this.positioncaption = new System.Windows.Forms.Label();
            this.typecaption = new System.Windows.Forms.Label();
            this.sizelabel = new System.Windows.Forms.Label();
            this.blockinglabel = new System.Windows.Forms.Label();
            this.positionlabel = new System.Windows.Forms.Label();
            this.typelist = new System.Windows.Forms.TreeView();
            this.thingimages = new System.Windows.Forms.ImageList(this.components);
            this.infopanel = new System.Windows.Forms.Panel();
            this.typeid = new CodeImp.DoomBuilder.Controls.NumericTextbox();
            this.infopanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // sizecaption
            // 
            this.sizecaption.AutoSize = true;
            this.sizecaption.Location = new System.Drawing.Point(208, 16);
            this.sizecaption.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.sizecaption.Name = "sizecaption";
            this.sizecaption.Size = new System.Drawing.Size(39, 17);
            this.sizecaption.TabIndex = 16;
            this.sizecaption.Text = "Size:";
            // 
            // blockingcaption
            // 
            this.blockingcaption.AutoSize = true;
            this.blockingcaption.Location = new System.Drawing.Point(181, 52);
            this.blockingcaption.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.blockingcaption.Name = "blockingcaption";
            this.blockingcaption.Size = new System.Drawing.Size(65, 17);
            this.blockingcaption.TabIndex = 14;
            this.blockingcaption.Text = "Blocking:";
            // 
            // positioncaption
            // 
            this.positioncaption.AutoSize = true;
            this.positioncaption.Location = new System.Drawing.Point(-2, 52);
            this.positioncaption.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.positioncaption.Name = "positioncaption";
            this.positioncaption.Size = new System.Drawing.Size(62, 17);
            this.positioncaption.TabIndex = 12;
            this.positioncaption.Text = "Position:";
            // 
            // typecaption
            // 
            this.typecaption.AutoSize = true;
            this.typecaption.Location = new System.Drawing.Point(-2, 16);
            this.typecaption.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.typecaption.Name = "typecaption";
            this.typecaption.Size = new System.Drawing.Size(44, 17);
            this.typecaption.TabIndex = 10;
            this.typecaption.Text = "Type:";
            // 
            // sizelabel
            // 
            this.sizelabel.AutoSize = true;
            this.sizelabel.Location = new System.Drawing.Point(250, 16);
            this.sizelabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.sizelabel.Name = "sizelabel";
            this.sizelabel.Size = new System.Drawing.Size(54, 17);
            this.sizelabel.TabIndex = 17;
            this.sizelabel.Text = "16 x 96";
            // 
            // blockinglabel
            // 
            this.blockinglabel.AutoSize = true;
            this.blockinglabel.Location = new System.Drawing.Point(248, 52);
            this.blockinglabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.blockinglabel.Name = "blockinglabel";
            this.blockinglabel.Size = new System.Drawing.Size(84, 17);
            this.blockinglabel.TabIndex = 15;
            this.blockinglabel.Text = "True-Height";
            // 
            // positionlabel
            // 
            this.positionlabel.AutoSize = true;
            this.positionlabel.Location = new System.Drawing.Point(60, 52);
            this.positionlabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.positionlabel.Name = "positionlabel";
            this.positionlabel.Size = new System.Drawing.Size(50, 17);
            this.positionlabel.TabIndex = 13;
            this.positionlabel.Text = "Ceiling";
            // 
            // typelist
            // 
            this.typelist.Dock = System.Windows.Forms.DockStyle.Top;
            this.typelist.HideSelection = false;
            this.typelist.ImageIndex = 0;
            this.typelist.ImageList = this.thingimages;
            this.typelist.Location = new System.Drawing.Point(0, 0);
            this.typelist.Margin = new System.Windows.Forms.Padding(10, 10, 11, 10);
            this.typelist.Name = "typelist";
            treeNode1.Name = "Node0";
            treeNode1.Text = "Monsters";
            this.typelist.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.typelist.SelectedImageIndex = 0;
            this.typelist.Size = new System.Drawing.Size(380, 325);
            this.typelist.TabIndex = 0;
            this.typelist.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.typelist_AfterSelect);
            this.typelist.DoubleClick += new System.EventHandler(this.typelist_DoubleClick);
            // 
            // thingimages
            // 
            this.thingimages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("thingimages.ImageStream")));
            this.thingimages.TransparentColor = System.Drawing.SystemColors.Window;
            this.thingimages.Images.SetKeyName(0, "thing00.png");
            this.thingimages.Images.SetKeyName(1, "thing01.png");
            this.thingimages.Images.SetKeyName(2, "thing02.png");
            this.thingimages.Images.SetKeyName(3, "thing03.png");
            this.thingimages.Images.SetKeyName(4, "thing04.png");
            this.thingimages.Images.SetKeyName(5, "thing05.png");
            this.thingimages.Images.SetKeyName(6, "thing06.png");
            this.thingimages.Images.SetKeyName(7, "thing07.png");
            this.thingimages.Images.SetKeyName(8, "thing08.png");
            this.thingimages.Images.SetKeyName(9, "thing09.png");
            this.thingimages.Images.SetKeyName(10, "thing10.png");
            this.thingimages.Images.SetKeyName(11, "thing11.png");
            this.thingimages.Images.SetKeyName(12, "thing12.png");
            this.thingimages.Images.SetKeyName(13, "thing13.png");
            this.thingimages.Images.SetKeyName(14, "thing14.png");
            this.thingimages.Images.SetKeyName(15, "thing15.png");
            this.thingimages.Images.SetKeyName(16, "thing16.png");
            this.thingimages.Images.SetKeyName(17, "thing17.png");
            this.thingimages.Images.SetKeyName(18, "thing18.png");
            this.thingimages.Images.SetKeyName(19, "thing19.png");
            // 
            // infopanel
            // 
            this.infopanel.Controls.Add(this.sizelabel);
            this.infopanel.Controls.Add(this.typecaption);
            this.infopanel.Controls.Add(this.sizecaption);
            this.infopanel.Controls.Add(this.typeid);
            this.infopanel.Controls.Add(this.blockinglabel);
            this.infopanel.Controls.Add(this.positioncaption);
            this.infopanel.Controls.Add(this.blockingcaption);
            this.infopanel.Controls.Add(this.positionlabel);
            this.infopanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.infopanel.Location = new System.Drawing.Point(0, 326);
            this.infopanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.infopanel.Name = "infopanel";
            this.infopanel.Size = new System.Drawing.Size(380, 74);
            this.infopanel.TabIndex = 18;
            // 
            // typeid
            // 
            this.typeid.AllowDecimal = false;
            this.typeid.AllowNegative = false;
            this.typeid.AllowRelative = false;
            this.typeid.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.typeid.Location = new System.Drawing.Point(51, 12);
            this.typeid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.typeid.Name = "typeid";
            this.typeid.Size = new System.Drawing.Size(84, 22);
            this.typeid.TabIndex = 1;
            this.typeid.TextChanged += new System.EventHandler(this.typeid_TextChanged);
            // 
            // ThingBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.typelist);
            this.Controls.Add(this.infopanel);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ThingBrowserControl";
            this.Size = new System.Drawing.Size(380, 400);
            this.SizeChanged += new System.EventHandler(this.ThingBrowserControl_SizeChanged);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.ThingBrowserControl_Layout);
            this.Resize += new System.EventHandler(this.ThingBrowserControl_Resize);
            this.infopanel.ResumeLayout(false);
            this.infopanel.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label sizelabel;
		private System.Windows.Forms.Label blockinglabel;
		private System.Windows.Forms.Label positionlabel;
		private NumericTextbox typeid;
		private System.Windows.Forms.TreeView typelist;
		private System.Windows.Forms.ImageList thingimages;
		private System.Windows.Forms.Panel infopanel;
		private System.Windows.Forms.Label sizecaption;
		private System.Windows.Forms.Label blockingcaption;
		private System.Windows.Forms.Label positioncaption;
		private System.Windows.Forms.Label typecaption;
	}
}
