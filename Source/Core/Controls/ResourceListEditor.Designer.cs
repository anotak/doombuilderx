namespace CodeImp.DoomBuilder.Controls
{
	partial class ResourceListEditor
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
			System.Windows.Forms.SplitContainer buttonsbar2;
			System.Windows.Forms.SplitContainer buttonsbar1;
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "C:\\Windows\\Doom\\Doom2.wad"}, 3, System.Drawing.SystemColors.GrayText, System.Drawing.SystemColors.Window, null);
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "C:\\My\\Little\\Textures\\"}, 2, System.Drawing.SystemColors.GrayText, System.Drawing.SystemColors.Window, null);
			System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("C:\\My\\Little\\Pony.wad", 1);
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResourceListEditor));
			this.editresource = new System.Windows.Forms.Button();
			this.deleteresource = new System.Windows.Forms.Button();
			this.addresource = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.resourceitems = new CodeImp.DoomBuilder.Controls.ResourceListView();
			this.column = new System.Windows.Forms.ColumnHeader();
			this.images = new System.Windows.Forms.ImageList(this.components);
			buttonsbar2 = new System.Windows.Forms.SplitContainer();
			buttonsbar1 = new System.Windows.Forms.SplitContainer();
			buttonsbar2.Panel1.SuspendLayout();
			buttonsbar2.Panel2.SuspendLayout();
			buttonsbar2.SuspendLayout();
			buttonsbar1.Panel1.SuspendLayout();
			buttonsbar1.Panel2.SuspendLayout();
			buttonsbar1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonsbar2
			// 
			buttonsbar2.Dock = System.Windows.Forms.DockStyle.Fill;
			buttonsbar2.IsSplitterFixed = true;
			buttonsbar2.Location = new System.Drawing.Point(0, 0);
			buttonsbar2.Name = "buttonsbar2";
			// 
			// buttonsbar2.Panel1
			// 
			buttonsbar2.Panel1.Controls.Add(this.editresource);
			// 
			// buttonsbar2.Panel2
			// 
			buttonsbar2.Panel2.Controls.Add(this.deleteresource);
			buttonsbar2.Size = new System.Drawing.Size(228, 24);
			buttonsbar2.SplitterDistance = 136;
			buttonsbar2.TabIndex = 0;
			// 
			// editresource
			// 
			this.editresource.Dock = System.Windows.Forms.DockStyle.Fill;
			this.editresource.Enabled = false;
			this.editresource.Location = new System.Drawing.Point(0, 0);
			this.editresource.Name = "editresource";
			this.editresource.Size = new System.Drawing.Size(136, 24);
			this.editresource.TabIndex = 0;
			this.editresource.Text = "Resource options...";
			this.editresource.UseVisualStyleBackColor = true;
			this.editresource.Click += new System.EventHandler(this.editresource_Click);
			// 
			// deleteresource
			// 
			this.deleteresource.Dock = System.Windows.Forms.DockStyle.Fill;
			this.deleteresource.Enabled = false;
			this.deleteresource.Location = new System.Drawing.Point(0, 0);
			this.deleteresource.Name = "deleteresource";
			this.deleteresource.Size = new System.Drawing.Size(88, 24);
			this.deleteresource.TabIndex = 0;
			this.deleteresource.Text = "Remove";
			this.deleteresource.UseVisualStyleBackColor = true;
			this.deleteresource.Click += new System.EventHandler(this.deleteresource_Click);
			// 
			// buttonsbar1
			// 
			buttonsbar1.Dock = System.Windows.Forms.DockStyle.Fill;
			buttonsbar1.IsSplitterFixed = true;
			buttonsbar1.Location = new System.Drawing.Point(0, 0);
			buttonsbar1.Name = "buttonsbar1";
			// 
			// buttonsbar1.Panel1
			// 
			buttonsbar1.Panel1.Controls.Add(this.addresource);
			// 
			// buttonsbar1.Panel2
			// 
			buttonsbar1.Panel2.Controls.Add(buttonsbar2);
			buttonsbar1.Size = new System.Drawing.Size(350, 24);
			buttonsbar1.SplitterDistance = 118;
			buttonsbar1.TabIndex = 0;
			// 
			// addresource
			// 
			this.addresource.Dock = System.Windows.Forms.DockStyle.Fill;
			this.addresource.Location = new System.Drawing.Point(0, 0);
			this.addresource.Name = "addresource";
			this.addresource.Size = new System.Drawing.Size(118, 24);
			this.addresource.TabIndex = 0;
			this.addresource.Text = "Add resource...";
			this.addresource.UseVisualStyleBackColor = true;
			this.addresource.Click += new System.EventHandler(this.addresource_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.resourceitems);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(buttonsbar1);
			this.splitContainer1.Panel2MinSize = 24;
			this.splitContainer1.Size = new System.Drawing.Size(350, 166);
			this.splitContainer1.SplitterDistance = 138;
			this.splitContainer1.TabIndex = 0;
			// 
			// resourceitems
			// 
			this.resourceitems.AllowDrop = true;
			this.resourceitems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.column});
			this.resourceitems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.resourceitems.FullRowSelect = true;
			this.resourceitems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.resourceitems.HideSelection = false;
			this.resourceitems.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3});
			this.resourceitems.Location = new System.Drawing.Point(0, 0);
			this.resourceitems.MultiSelect = false;
			this.resourceitems.Name = "resourceitems";
			this.resourceitems.ShowGroups = false;
			this.resourceitems.ShowItemToolTips = true;
			this.resourceitems.Size = new System.Drawing.Size(350, 138);
			this.resourceitems.SmallImageList = this.images;
			this.resourceitems.TabIndex = 0;
			this.resourceitems.UseCompatibleStateImageBehavior = false;
			this.resourceitems.View = System.Windows.Forms.View.Details;
			this.resourceitems.ClientSizeChanged += new System.EventHandler(this.resourceitems_ClientSizeChanged);
			this.resourceitems.SizeChanged += new System.EventHandler(this.resources_SizeChanged);
			this.resourceitems.DoubleClick += new System.EventHandler(this.resourceitems_DoubleClick);
			this.resourceitems.DragDrop += new System.Windows.Forms.DragEventHandler(this.resourceitems_DragDrop);
			this.resourceitems.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.resourceitems_ItemSelectionChanged);
			this.resourceitems.DragOver += new System.Windows.Forms.DragEventHandler(this.resourceitems_DragOver);
			// 
			// column
			// 
			this.column.Text = "Resource location";
			this.column.Width = 200;
			// 
			// images
			// 
			this.images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("images.ImageStream")));
			this.images.TransparentColor = System.Drawing.Color.Transparent;
			this.images.Images.SetKeyName(0, "Folder.ico");
			this.images.Images.SetKeyName(1, "File.ico");
			this.images.Images.SetKeyName(2, "PK3.ico");
			this.images.Images.SetKeyName(3, "FolderLocked.ico");
			this.images.Images.SetKeyName(4, "FileLocked.ico");
			this.images.Images.SetKeyName(5, "PK3Locked.ico");
			// 
			// ResourceListEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.splitContainer1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "ResourceListEditor";
			this.Size = new System.Drawing.Size(350, 166);
			buttonsbar2.Panel1.ResumeLayout(false);
			buttonsbar2.Panel2.ResumeLayout(false);
			buttonsbar2.ResumeLayout(false);
			buttonsbar1.Panel1.ResumeLayout(false);
			buttonsbar1.Panel2.ResumeLayout(false);
			buttonsbar1.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button deleteresource;
		private System.Windows.Forms.Button editresource;
		private System.Windows.Forms.Button addresource;
		private CodeImp.DoomBuilder.Controls.ResourceListView resourceitems;
		private System.Windows.Forms.ColumnHeader column;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ImageList images;
	}
}
