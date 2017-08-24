namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class MenusForm
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
			this.menustrip = new System.Windows.Forms.MenuStrip();
			this.linedefsmenu = new System.Windows.Forms.ToolStripMenuItem();
			this.selectsinglesideditem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectdoublesideditem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.fliplinedefsitem = new System.Windows.Forms.ToolStripMenuItem();
			this.flipsidedefsitem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.curvelinedefsitem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.splitlinedefsitem = new System.Windows.Forms.ToolStripMenuItem();
			this.sectorsmenu = new System.Windows.Forms.ToolStripMenuItem();
			this.joinsectorsitem = new System.Windows.Forms.ToolStripMenuItem();
			this.mergesectorsitem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.globalstrip = new System.Windows.Forms.ToolStrip();
			this.manualstrip = new System.Windows.Forms.ToolStrip();
			this.separatorsectors1 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonselectionnumbers = new System.Windows.Forms.ToolStripButton();
			this.buttonbrightnessgradient = new System.Windows.Forms.ToolStripButton();
			this.buttonfloorgradient = new System.Windows.Forms.ToolStripButton();
			this.buttonceilinggradient = new System.Windows.Forms.ToolStripButton();
			this.buttonflipselectionh = new System.Windows.Forms.ToolStripButton();
			this.buttonflipselectionv = new System.Windows.Forms.ToolStripButton();
			this.buttoncurvelinedefs = new System.Windows.Forms.ToolStripButton();
			this.buttoncopyproperties = new System.Windows.Forms.ToolStripButton();
			this.buttonpasteproperties = new System.Windows.Forms.ToolStripButton();
			this.seperatorcopypaste = new System.Windows.Forms.ToolStripSeparator();
			this.menustrip.SuspendLayout();
			this.manualstrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// menustrip
			// 
			this.menustrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.linedefsmenu,
            this.sectorsmenu});
			this.menustrip.Location = new System.Drawing.Point(0, 0);
			this.menustrip.Name = "menustrip";
			this.menustrip.Size = new System.Drawing.Size(423, 24);
			this.menustrip.TabIndex = 0;
			this.menustrip.Text = "menustrip";
			// 
			// linedefsmenu
			// 
			this.linedefsmenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectsinglesideditem,
            this.selectdoublesideditem,
            this.toolStripMenuItem4,
            this.fliplinedefsitem,
            this.flipsidedefsitem,
            this.toolStripMenuItem1,
            this.curvelinedefsitem,
            this.toolStripMenuItem3,
            this.splitlinedefsitem});
			this.linedefsmenu.Name = "linedefsmenu";
			this.linedefsmenu.Size = new System.Drawing.Size(59, 20);
			this.linedefsmenu.Text = "&Linedefs";
			this.linedefsmenu.Visible = false;
			// 
			// selectsinglesideditem
			// 
			this.selectsinglesideditem.Name = "selectsinglesideditem";
			this.selectsinglesideditem.Size = new System.Drawing.Size(202, 22);
			this.selectsinglesideditem.Tag = "selectsinglesided";
			this.selectsinglesideditem.Text = "Select &Single-sided only";
			this.selectsinglesideditem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// selectdoublesideditem
			// 
			this.selectdoublesideditem.Name = "selectdoublesideditem";
			this.selectdoublesideditem.Size = new System.Drawing.Size(202, 22);
			this.selectdoublesideditem.Tag = "selectdoublesided";
			this.selectdoublesideditem.Text = "Select &Double-sided only";
			this.selectdoublesideditem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(199, 6);
			// 
			// fliplinedefsitem
			// 
			this.fliplinedefsitem.Name = "fliplinedefsitem";
			this.fliplinedefsitem.Size = new System.Drawing.Size(202, 22);
			this.fliplinedefsitem.Tag = "fliplinedefs";
			this.fliplinedefsitem.Text = "&Flip Linedefs";
			this.fliplinedefsitem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// flipsidedefsitem
			// 
			this.flipsidedefsitem.Name = "flipsidedefsitem";
			this.flipsidedefsitem.Size = new System.Drawing.Size(202, 22);
			this.flipsidedefsitem.Tag = "flipsidedefs";
			this.flipsidedefsitem.Text = "F&lip Sidedefs";
			this.flipsidedefsitem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(199, 6);
			// 
			// curvelinedefsitem
			// 
			this.curvelinedefsitem.Name = "curvelinedefsitem";
			this.curvelinedefsitem.Size = new System.Drawing.Size(202, 22);
			this.curvelinedefsitem.Tag = "curvelinesmode";
			this.curvelinedefsitem.Text = "&Curve Linedefs...";
			this.curvelinedefsitem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(199, 6);
			// 
			// splitlinedefsitem
			// 
			this.splitlinedefsitem.Name = "splitlinedefsitem";
			this.splitlinedefsitem.Size = new System.Drawing.Size(202, 22);
			this.splitlinedefsitem.Tag = "splitlinedefs";
			this.splitlinedefsitem.Text = "S&plit Linedefs";
			this.splitlinedefsitem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// sectorsmenu
			// 
			this.sectorsmenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.joinsectorsitem,
            this.mergesectorsitem,
            this.toolStripMenuItem2});
			this.sectorsmenu.Name = "sectorsmenu";
			this.sectorsmenu.Size = new System.Drawing.Size(55, 20);
			this.sectorsmenu.Text = "&Sectors";
			this.sectorsmenu.Visible = false;
			// 
			// joinsectorsitem
			// 
			this.joinsectorsitem.Name = "joinsectorsitem";
			this.joinsectorsitem.Size = new System.Drawing.Size(154, 22);
			this.joinsectorsitem.Tag = "joinsectors";
			this.joinsectorsitem.Text = "&Join Sectors";
			this.joinsectorsitem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// mergesectorsitem
			// 
			this.mergesectorsitem.Name = "mergesectorsitem";
			this.mergesectorsitem.Size = new System.Drawing.Size(154, 22);
			this.mergesectorsitem.Tag = "mergesectors";
			this.mergesectorsitem.Text = "&Merge Sectors";
			this.mergesectorsitem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(151, 6);
			// 
			// globalstrip
			// 
			this.globalstrip.Location = new System.Drawing.Point(0, 24);
			this.globalstrip.Name = "globalstrip";
			this.globalstrip.Size = new System.Drawing.Size(423, 25);
			this.globalstrip.TabIndex = 1;
			this.globalstrip.Text = "toolstrip";
			// 
			// manualstrip
			// 
			this.manualstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttoncopyproperties,
            this.buttonpasteproperties,
            this.seperatorcopypaste,
            this.buttonselectionnumbers,
            this.separatorsectors1,
            this.buttonbrightnessgradient,
            this.buttonfloorgradient,
            this.buttonceilinggradient,
            this.buttonflipselectionh,
            this.buttonflipselectionv,
            this.buttoncurvelinedefs});
			this.manualstrip.Location = new System.Drawing.Point(0, 49);
			this.manualstrip.Name = "manualstrip";
			this.manualstrip.Size = new System.Drawing.Size(423, 25);
			this.manualstrip.TabIndex = 2;
			this.manualstrip.Text = "toolStrip1";
			// 
			// separatorsectors1
			// 
			this.separatorsectors1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.separatorsectors1.Name = "separatorsectors1";
			this.separatorsectors1.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonselectionnumbers
			// 
			this.buttonselectionnumbers.CheckOnClick = true;
			this.buttonselectionnumbers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonselectionnumbers.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.ViewSelectionIndex;
			this.buttonselectionnumbers.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonselectionnumbers.Name = "buttonselectionnumbers";
			this.buttonselectionnumbers.Size = new System.Drawing.Size(23, 22);
			this.buttonselectionnumbers.Text = "View Selection Numbering";
			this.buttonselectionnumbers.Click += new System.EventHandler(this.buttonselectionnumbers_Click);
			// 
			// buttonbrightnessgradient
			// 
			this.buttonbrightnessgradient.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonbrightnessgradient.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.BrightnessGradient;
			this.buttonbrightnessgradient.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.buttonbrightnessgradient.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonbrightnessgradient.Name = "buttonbrightnessgradient";
			this.buttonbrightnessgradient.Size = new System.Drawing.Size(23, 22);
			this.buttonbrightnessgradient.Tag = "gradientbrightness";
			this.buttonbrightnessgradient.Text = "Make Brightness Gradient";
			this.buttonbrightnessgradient.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonfloorgradient
			// 
			this.buttonfloorgradient.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonfloorgradient.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.FloorsGradient;
			this.buttonfloorgradient.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonfloorgradient.Name = "buttonfloorgradient";
			this.buttonfloorgradient.Size = new System.Drawing.Size(23, 22);
			this.buttonfloorgradient.Tag = "gradientfloors";
			this.buttonfloorgradient.Text = "Make Floor Heights Gradient";
			this.buttonfloorgradient.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonceilinggradient
			// 
			this.buttonceilinggradient.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonceilinggradient.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.CeilsGradient;
			this.buttonceilinggradient.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonceilinggradient.Name = "buttonceilinggradient";
			this.buttonceilinggradient.Size = new System.Drawing.Size(23, 22);
			this.buttonceilinggradient.Tag = "gradientceilings";
			this.buttonceilinggradient.Text = "Make Ceiling Heights Gradient";
			this.buttonceilinggradient.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonflipselectionh
			// 
			this.buttonflipselectionh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonflipselectionh.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.FlipSelectionH;
			this.buttonflipselectionh.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonflipselectionh.Name = "buttonflipselectionh";
			this.buttonflipselectionh.Size = new System.Drawing.Size(23, 22);
			this.buttonflipselectionh.Tag = "flipselectionh";
			this.buttonflipselectionh.Text = "Flip Selection Horizontally";
			this.buttonflipselectionh.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonflipselectionv
			// 
			this.buttonflipselectionv.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonflipselectionv.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.FlipSelectionV;
			this.buttonflipselectionv.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonflipselectionv.Name = "buttonflipselectionv";
			this.buttonflipselectionv.Size = new System.Drawing.Size(23, 22);
			this.buttonflipselectionv.Tag = "flipselectionv";
			this.buttonflipselectionv.Text = "Flip Selection Vertically";
			this.buttonflipselectionv.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttoncurvelinedefs
			// 
			this.buttoncurvelinedefs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoncurvelinedefs.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.CurveLines;
			this.buttoncurvelinedefs.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoncurvelinedefs.Name = "buttoncurvelinedefs";
			this.buttoncurvelinedefs.Size = new System.Drawing.Size(23, 22);
			this.buttoncurvelinedefs.Tag = "curvelinesmode";
			this.buttoncurvelinedefs.Text = "Curve Linedefs";
			this.buttoncurvelinedefs.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttoncopyproperties
			// 
			this.buttoncopyproperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoncopyproperties.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.CopyProperties;
			this.buttoncopyproperties.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoncopyproperties.Name = "buttoncopyproperties";
			this.buttoncopyproperties.Size = new System.Drawing.Size(23, 22);
			this.buttoncopyproperties.Tag = "classiccopyproperties";
			this.buttoncopyproperties.Text = "Copy Properties";
			this.buttoncopyproperties.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonpasteproperties
			// 
			this.buttonpasteproperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonpasteproperties.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.PasteProperties;
			this.buttonpasteproperties.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonpasteproperties.Name = "buttonpasteproperties";
			this.buttonpasteproperties.Size = new System.Drawing.Size(23, 22);
			this.buttonpasteproperties.Tag = "classicpasteproperties";
			this.buttonpasteproperties.Text = "Paste Properties";
			this.buttonpasteproperties.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// seperatorcopypaste
			// 
			this.seperatorcopypaste.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.seperatorcopypaste.Name = "seperatorcopypaste";
			this.seperatorcopypaste.Size = new System.Drawing.Size(6, 25);
			// 
			// MenusForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(423, 248);
			this.Controls.Add(this.manualstrip);
			this.Controls.Add(this.globalstrip);
			this.Controls.Add(this.menustrip);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MainMenuStrip = this.menustrip;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MenusForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "MenusForm";
			this.menustrip.ResumeLayout(false);
			this.menustrip.PerformLayout();
			this.manualstrip.ResumeLayout(false);
			this.manualstrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menustrip;
		private System.Windows.Forms.ToolStripMenuItem linedefsmenu;
		private System.Windows.Forms.ToolStripMenuItem sectorsmenu;
		private System.Windows.Forms.ToolStripMenuItem fliplinedefsitem;
		private System.Windows.Forms.ToolStripMenuItem flipsidedefsitem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem curvelinedefsitem;
		private System.Windows.Forms.ToolStripMenuItem joinsectorsitem;
		private System.Windows.Forms.ToolStripMenuItem mergesectorsitem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem splitlinedefsitem;
		private System.Windows.Forms.ToolStrip globalstrip;
		private System.Windows.Forms.ToolStrip manualstrip;
		private System.Windows.Forms.ToolStripButton buttonbrightnessgradient;
		private System.Windows.Forms.ToolStripMenuItem selectsinglesideditem;
		private System.Windows.Forms.ToolStripMenuItem selectdoublesideditem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
		private System.Windows.Forms.ToolStripButton buttonflipselectionh;
		private System.Windows.Forms.ToolStripButton buttonflipselectionv;
		private System.Windows.Forms.ToolStripButton buttonselectionnumbers;
		private System.Windows.Forms.ToolStripSeparator separatorsectors1;
		private System.Windows.Forms.ToolStripButton buttonfloorgradient;
		private System.Windows.Forms.ToolStripButton buttonceilinggradient;
		private System.Windows.Forms.ToolStripButton buttoncurvelinedefs;
		private System.Windows.Forms.ToolStripButton buttoncopyproperties;
		private System.Windows.Forms.ToolStripButton buttonpasteproperties;
		private System.Windows.Forms.ToolStripSeparator seperatorcopypaste;
	}
}