namespace CodeImp.DoomBuilder.Controls
{
	partial class ImageBrowserControl
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
				CleanUp();
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
			this.splitter = new System.Windows.Forms.SplitContainer();
			this.list = new CodeImp.DoomBuilder.Controls.OptimizedListView();
			this.texturesize = new System.Windows.Forms.Label();
			this.texturesizelabel = new System.Windows.Forms.Label();
			this.objectname = new System.Windows.Forms.TextBox();
			this.label = new System.Windows.Forms.Label();
			this.refreshtimer = new System.Windows.Forms.Timer(this.components);
			this.texturesizetimer = new System.Windows.Forms.Timer(this.components);
			this.splitter.Panel1.SuspendLayout();
			this.splitter.Panel2.SuspendLayout();
			this.splitter.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitter
			// 
			this.splitter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitter.IsSplitterFixed = true;
			this.splitter.Location = new System.Drawing.Point(0, 0);
			this.splitter.Name = "splitter";
			this.splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitter.Panel1
			// 
			this.splitter.Panel1.Controls.Add(this.list);
			// 
			// splitter.Panel2
			// 
			this.splitter.Panel2.Controls.Add(this.texturesize);
			this.splitter.Panel2.Controls.Add(this.texturesizelabel);
			this.splitter.Panel2.Controls.Add(this.objectname);
			this.splitter.Panel2.Controls.Add(this.label);
			this.splitter.Size = new System.Drawing.Size(518, 346);
			this.splitter.SplitterDistance = 312;
			this.splitter.TabIndex = 0;
			this.splitter.TabStop = false;
			// 
			// list
			// 
			this.list.Dock = System.Windows.Forms.DockStyle.Fill;
			this.list.HideSelection = false;
			this.list.Location = new System.Drawing.Point(0, 0);
			this.list.MultiSelect = false;
			this.list.Name = "list";
			this.list.OwnerDraw = true;
			this.list.ShowItemToolTips = true;
			this.list.Size = new System.Drawing.Size(518, 312);
			this.list.TabIndex = 1;
			this.list.TabStop = false;
			this.list.TileSize = new System.Drawing.Size(90, 90);
			this.list.UseCompatibleStateImageBehavior = false;
			this.list.View = System.Windows.Forms.View.Tile;
			this.list.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.list_DrawItem);
			this.list.DoubleClick += new System.EventHandler(this.list_DoubleClick);
			this.list.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.list_ItemSelectionChanged);
			this.list.KeyDown += new System.Windows.Forms.KeyEventHandler(this.list_KeyDown);
			// 
			// texturesize
			// 
			this.texturesize.Location = new System.Drawing.Point(368, 13);
			this.texturesize.Name = "texturesize";
			this.texturesize.Size = new System.Drawing.Size(271, 14);
			this.texturesize.TabIndex = 2;
			this.texturesize.Text = "1024 x 1024";
			this.texturesize.Visible = false;
			// 
			// texturesizelabel
			// 
			this.texturesizelabel.AutoSize = true;
			this.texturesizelabel.Location = new System.Drawing.Point(331, 13);
			this.texturesizelabel.Name = "texturesizelabel";
			this.texturesizelabel.Size = new System.Drawing.Size(31, 14);
			this.texturesizelabel.TabIndex = 1;
			this.texturesizelabel.Text = "Size:";
			this.texturesizelabel.Visible = false;
			// 
			// objectname
			// 
			this.objectname.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.objectname.Location = new System.Drawing.Point(145, 10);
			this.objectname.Name = "objectname";
			this.objectname.Size = new System.Drawing.Size(122, 20);
			this.objectname.TabIndex = 0;
			this.objectname.TabStop = false;
			this.objectname.TextChanged += new System.EventHandler(this.objectname_TextChanged);
			this.objectname.KeyDown += new System.Windows.Forms.KeyEventHandler(this.objectname_KeyDown);
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.Location = new System.Drawing.Point(1, 13);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(138, 14);
			this.label.TabIndex = 0;
			this.label.Text = "Select or type object name:";
			// 
			// refreshtimer
			// 
			this.refreshtimer.Interval = 500;
			this.refreshtimer.Tick += new System.EventHandler(this.refreshtimer_Tick);
			// 
			// texturesizetimer
			// 
			this.texturesizetimer.Interval = 3;
			this.texturesizetimer.Tick += new System.EventHandler(this.texturesizetimer_Tick);
			// 
			// ImageBrowserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.splitter);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "ImageBrowserControl";
			this.Size = new System.Drawing.Size(518, 346);
			this.Resize += new System.EventHandler(this.ImageBrowserControl_Resize);
			this.splitter.Panel1.ResumeLayout(false);
			this.splitter.Panel2.ResumeLayout(false);
			this.splitter.Panel2.PerformLayout();
			this.splitter.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitter;
		private OptimizedListView list;
		private System.Windows.Forms.Timer refreshtimer;
		private System.Windows.Forms.TextBox objectname;
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Label texturesize;
		private System.Windows.Forms.Label texturesizelabel;
		private System.Windows.Forms.Timer texturesizetimer;

	}
}
