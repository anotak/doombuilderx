namespace CodeImp.DoomBuilder.Controls
{
	partial class ScriptEditorPanel
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptEditorPanel));
			this.tabs = new System.Windows.Forms.TabControl();
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.buttonnew = new System.Windows.Forms.ToolStripDropDownButton();
			this.buttonopen = new System.Windows.Forms.ToolStripButton();
			this.buttonsave = new System.Windows.Forms.ToolStripButton();
			this.buttonsaveall = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonundo = new System.Windows.Forms.ToolStripButton();
			this.buttonredo = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.buttoncut = new System.Windows.Forms.ToolStripButton();
			this.buttoncopy = new System.Windows.Forms.ToolStripButton();
			this.buttonpaste = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonscriptconfig = new System.Windows.Forms.ToolStripDropDownButton();
			this.buttoncompile = new System.Windows.Forms.ToolStripButton();
			this.buttonclose = new System.Windows.Forms.ToolStripButton();
			this.buttonkeywordhelp = new System.Windows.Forms.ToolStripButton();
			this.openfile = new System.Windows.Forms.OpenFileDialog();
			this.savefile = new System.Windows.Forms.SaveFileDialog();
			this.splitter = new System.Windows.Forms.SplitContainer();
			this.label1 = new System.Windows.Forms.Label();
			this.errorlist = new System.Windows.Forms.ListView();
			this.colIndex = new System.Windows.Forms.ColumnHeader();
			this.colDescription = new System.Windows.Forms.ColumnHeader();
			this.colFile = new System.Windows.Forms.ColumnHeader();
			this.errorimages = new System.Windows.Forms.ImageList(this.components);
			this.toolbar.SuspendLayout();
			this.splitter.Panel1.SuspendLayout();
			this.splitter.Panel2.SuspendLayout();
			this.splitter.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Location = new System.Drawing.Point(3, 8);
			this.tabs.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
			this.tabs.Name = "tabs";
			this.tabs.Padding = new System.Drawing.Point(12, 3);
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(720, 401);
			this.tabs.TabIndex = 0;
			this.tabs.TabStop = false;
			this.tabs.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabs_Selecting);
			this.tabs.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tabs_MouseUp);
			// 
			// toolbar
			// 
			this.toolbar.AllowMerge = false;
			this.toolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonnew,
            this.buttonopen,
            this.buttonsave,
            this.buttonsaveall,
            this.toolStripSeparator1,
            this.buttonundo,
            this.buttonredo,
            this.toolStripSeparator2,
            this.buttoncut,
            this.buttoncopy,
            this.buttonpaste,
            this.toolStripSeparator3,
            this.buttonscriptconfig,
            this.buttoncompile,
            this.buttonclose,
            this.buttonkeywordhelp});
			this.toolbar.Location = new System.Drawing.Point(0, 0);
			this.toolbar.Name = "toolbar";
			this.toolbar.Size = new System.Drawing.Size(726, 25);
			this.toolbar.TabIndex = 1;
			// 
			// buttonnew
			// 
			this.buttonnew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonnew.Image = global::CodeImp.DoomBuilder.Properties.Resources.NewScript;
			this.buttonnew.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.buttonnew.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonnew.Margin = new System.Windows.Forms.Padding(6, 1, 0, 2);
			this.buttonnew.Name = "buttonnew";
			this.buttonnew.Size = new System.Drawing.Size(29, 22);
			this.buttonnew.Text = "New File";
			// 
			// buttonopen
			// 
			this.buttonopen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonopen.Image = global::CodeImp.DoomBuilder.Properties.Resources.OpenScript;
			this.buttonopen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.buttonopen.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonopen.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
			this.buttonopen.Name = "buttonopen";
			this.buttonopen.Size = new System.Drawing.Size(23, 22);
			this.buttonopen.Text = "Open File";
			this.buttonopen.Click += new System.EventHandler(this.buttonopen_Click);
			// 
			// buttonsave
			// 
			this.buttonsave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonsave.Enabled = false;
			this.buttonsave.Image = global::CodeImp.DoomBuilder.Properties.Resources.SaveScript;
			this.buttonsave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonsave.Name = "buttonsave";
			this.buttonsave.Size = new System.Drawing.Size(23, 22);
			this.buttonsave.Text = "Save File";
			this.buttonsave.Click += new System.EventHandler(this.buttonsave_Click);
			// 
			// buttonsaveall
			// 
			this.buttonsaveall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonsaveall.Enabled = false;
			this.buttonsaveall.Image = global::CodeImp.DoomBuilder.Properties.Resources.SaveAll;
			this.buttonsaveall.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonsaveall.Name = "buttonsaveall";
			this.buttonsaveall.Size = new System.Drawing.Size(23, 22);
			this.buttonsaveall.Text = "Save All Files";
			this.buttonsaveall.Click += new System.EventHandler(this.buttonsaveall_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonundo
			// 
			this.buttonundo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonundo.Image = global::CodeImp.DoomBuilder.Properties.Resources.Undo;
			this.buttonundo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonundo.Name = "buttonundo";
			this.buttonundo.Size = new System.Drawing.Size(23, 22);
			this.buttonundo.Text = "Undo";
			this.buttonundo.Click += new System.EventHandler(this.buttonundo_Click);
			// 
			// buttonredo
			// 
			this.buttonredo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonredo.Image = global::CodeImp.DoomBuilder.Properties.Resources.Redo;
			this.buttonredo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonredo.Name = "buttonredo";
			this.buttonredo.Size = new System.Drawing.Size(23, 22);
			this.buttonredo.Text = "Redo";
			this.buttonredo.Click += new System.EventHandler(this.buttonredo_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// buttoncut
			// 
			this.buttoncut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoncut.Image = global::CodeImp.DoomBuilder.Properties.Resources.Cut;
			this.buttoncut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoncut.Name = "buttoncut";
			this.buttoncut.Size = new System.Drawing.Size(23, 22);
			this.buttoncut.Text = "Cut Selection";
			this.buttoncut.Click += new System.EventHandler(this.buttoncut_Click);
			// 
			// buttoncopy
			// 
			this.buttoncopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoncopy.Image = global::CodeImp.DoomBuilder.Properties.Resources.Copy;
			this.buttoncopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoncopy.Name = "buttoncopy";
			this.buttoncopy.Size = new System.Drawing.Size(23, 22);
			this.buttoncopy.Text = "Copy Selection";
			this.buttoncopy.Click += new System.EventHandler(this.buttoncopy_Click);
			// 
			// buttonpaste
			// 
			this.buttonpaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonpaste.Image = global::CodeImp.DoomBuilder.Properties.Resources.Paste;
			this.buttonpaste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonpaste.Name = "buttonpaste";
			this.buttonpaste.Size = new System.Drawing.Size(23, 22);
			this.buttonpaste.Text = "Paste";
			this.buttonpaste.Click += new System.EventHandler(this.buttonpaste_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonscriptconfig
			// 
			this.buttonscriptconfig.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonscriptconfig.Enabled = false;
			this.buttonscriptconfig.Image = global::CodeImp.DoomBuilder.Properties.Resources.ScriptPalette;
			this.buttonscriptconfig.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonscriptconfig.Name = "buttonscriptconfig";
			this.buttonscriptconfig.Size = new System.Drawing.Size(29, 22);
			this.buttonscriptconfig.Text = "Change Script Type";
			// 
			// buttoncompile
			// 
			this.buttoncompile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoncompile.Image = global::CodeImp.DoomBuilder.Properties.Resources.ScriptCompile;
			this.buttoncompile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoncompile.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
			this.buttoncompile.Name = "buttoncompile";
			this.buttoncompile.Size = new System.Drawing.Size(23, 22);
			this.buttoncompile.Text = "Compile Script";
			this.buttoncompile.Click += new System.EventHandler(this.buttoncompile_Click);
			// 
			// buttonclose
			// 
			this.buttonclose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.buttonclose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonclose.Image = global::CodeImp.DoomBuilder.Properties.Resources.Close;
			this.buttonclose.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.buttonclose.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonclose.Name = "buttonclose";
			this.buttonclose.Size = new System.Drawing.Size(23, 22);
			this.buttonclose.Text = "Close File";
			this.buttonclose.Click += new System.EventHandler(this.buttonclose_Click);
			// 
			// buttonkeywordhelp
			// 
			this.buttonkeywordhelp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.buttonkeywordhelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonkeywordhelp.Image = global::CodeImp.DoomBuilder.Properties.Resources.ScriptHelp;
			this.buttonkeywordhelp.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonkeywordhelp.Name = "buttonkeywordhelp";
			this.buttonkeywordhelp.Size = new System.Drawing.Size(23, 22);
			this.buttonkeywordhelp.Text = "Keyword Help";
			this.buttonkeywordhelp.Click += new System.EventHandler(this.buttonkeywordhelp_Click);
			// 
			// openfile
			// 
			this.openfile.Title = "Open Script";
			// 
			// savefile
			// 
			this.savefile.Title = "Save Script As";
			// 
			// splitter
			// 
			this.splitter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitter.IsSplitterFixed = true;
			this.splitter.Location = new System.Drawing.Point(0, 25);
			this.splitter.Name = "splitter";
			this.splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitter.Panel1
			// 
			this.splitter.Panel1.Controls.Add(this.tabs);
			// 
			// splitter.Panel2
			// 
			this.splitter.Panel2.Controls.Add(this.label1);
			this.splitter.Panel2.Controls.Add(this.errorlist);
			this.splitter.Size = new System.Drawing.Size(726, 538);
			this.splitter.SplitterDistance = 412;
			this.splitter.TabIndex = 2;
			this.splitter.TabStop = false;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(1);
			this.label1.Size = new System.Drawing.Size(720, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Errors";
			// 
			// errorlist
			// 
			this.errorlist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.errorlist.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colIndex,
            this.colDescription,
            this.colFile});
			this.errorlist.FullRowSelect = true;
			this.errorlist.GridLines = true;
			this.errorlist.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.errorlist.LabelWrap = false;
			this.errorlist.Location = new System.Drawing.Point(3, 19);
			this.errorlist.MultiSelect = false;
			this.errorlist.Name = "errorlist";
			this.errorlist.ShowGroups = false;
			this.errorlist.Size = new System.Drawing.Size(720, 100);
			this.errorlist.SmallImageList = this.errorimages;
			this.errorlist.TabIndex = 0;
			this.errorlist.TabStop = false;
			this.errorlist.UseCompatibleStateImageBehavior = false;
			this.errorlist.View = System.Windows.Forms.View.Details;
			this.errorlist.ItemActivate += new System.EventHandler(this.errorlist_ItemActivate);
			// 
			// colIndex
			// 
			this.colIndex.Text = "";
			this.colIndex.Width = 45;
			// 
			// colDescription
			// 
			this.colDescription.Text = "Description";
			this.colDescription.Width = 500;
			// 
			// colFile
			// 
			this.colFile.Text = "File";
			this.colFile.Width = 150;
			// 
			// errorimages
			// 
			this.errorimages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("errorimages.ImageStream")));
			this.errorimages.TransparentColor = System.Drawing.Color.Transparent;
			this.errorimages.Images.SetKeyName(0, "ScriptError3.png");
			// 
			// ScriptEditorPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.splitter);
			this.Controls.Add(this.toolbar);
			this.Name = "ScriptEditorPanel";
			this.Size = new System.Drawing.Size(726, 563);
			this.toolbar.ResumeLayout(false);
			this.toolbar.PerformLayout();
			this.splitter.Panel1.ResumeLayout(false);
			this.splitter.Panel2.ResumeLayout(false);
			this.splitter.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.ToolStrip toolbar;
		private System.Windows.Forms.ToolStripButton buttonopen;
		private System.Windows.Forms.ToolStripDropDownButton buttonnew;
		private System.Windows.Forms.OpenFileDialog openfile;
		private System.Windows.Forms.SaveFileDialog savefile;
		private System.Windows.Forms.ToolStripButton buttonsave;
		private System.Windows.Forms.ToolStripButton buttonsaveall;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton buttoncompile;
		private System.Windows.Forms.ToolStripButton buttonundo;
		private System.Windows.Forms.ToolStripButton buttonredo;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton buttoncut;
		private System.Windows.Forms.ToolStripButton buttoncopy;
		private System.Windows.Forms.ToolStripButton buttonpaste;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripDropDownButton buttonscriptconfig;
		private System.Windows.Forms.ToolStripButton buttonclose;
		private System.Windows.Forms.SplitContainer splitter;
		private System.Windows.Forms.ListView errorlist;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ColumnHeader colIndex;
		private System.Windows.Forms.ColumnHeader colDescription;
		private System.Windows.Forms.ColumnHeader colFile;
		private System.Windows.Forms.ImageList errorimages;
		private System.Windows.Forms.ToolStripButton buttonkeywordhelp;
	}
}
