namespace CodeImp.DoomBuilder.Windows
{
	partial class ConfigForm
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label3;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label10;
			this.labelparameters = new System.Windows.Forms.Label();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabresources = new System.Windows.Forms.TabPage();
			this.configdata = new CodeImp.DoomBuilder.Controls.ResourceListEditor();
			this.tabnodebuilder = new System.Windows.Forms.TabPage();
			this.nodebuildertest = new System.Windows.Forms.ComboBox();
			this.nodebuildersave = new System.Windows.Forms.ComboBox();
			this.tabtesting = new System.Windows.Forms.TabPage();
			this.shortpaths = new System.Windows.Forms.CheckBox();
			this.customparameters = new System.Windows.Forms.CheckBox();
			this.skill = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
			this.browsetestprogram = new System.Windows.Forms.Button();
			this.noresultlabel = new System.Windows.Forms.Label();
			this.testresult = new System.Windows.Forms.TextBox();
			this.labelresult = new System.Windows.Forms.Label();
			this.testparameters = new System.Windows.Forms.TextBox();
			this.testapplication = new System.Windows.Forms.TextBox();
			this.tabtextures = new System.Windows.Forms.TabPage();
			this.listtextures = new System.Windows.Forms.ListView();
			this.smallimages = new System.Windows.Forms.ImageList(this.components);
			this.restoretexturesets = new System.Windows.Forms.Button();
			this.edittextureset = new System.Windows.Forms.Button();
			this.pastetexturesets = new System.Windows.Forms.Button();
			this.copytexturesets = new System.Windows.Forms.Button();
			this.removetextureset = new System.Windows.Forms.Button();
			this.addtextureset = new System.Windows.Forms.Button();
			this.tabmodes = new System.Windows.Forms.TabPage();
			this.startmode = new System.Windows.Forms.ComboBox();
			this.label11 = new System.Windows.Forms.Label();
			this.listmodes = new System.Windows.Forms.ListView();
			this.colmodename = new System.Windows.Forms.ColumnHeader();
			this.colmodeplugin = new System.Windows.Forms.ColumnHeader();
			this.listconfigs = new System.Windows.Forms.ListView();
			this.columnname = new System.Windows.Forms.ColumnHeader();
			this.testprogramdialog = new System.Windows.Forms.OpenFileDialog();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label10 = new System.Windows.Forms.Label();
			this.tabs.SuspendLayout();
			this.tabresources.SuspendLayout();
			this.tabnodebuilder.SuspendLayout();
			this.tabtesting.SuspendLayout();
			this.tabtextures.SuspendLayout();
			this.tabmodes.SuspendLayout();
			this.SuspendLayout();
			// 
			// label5
			// 
			label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(12, 276);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(312, 14);
			label5.TabIndex = 19;
			label5.Text = "Drag items to change order (lower items override higher items).";
			// 
			// label6
			// 
			label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			label6.AutoEllipsis = true;
			label6.Location = new System.Drawing.Point(12, 15);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(452, 37);
			label6.TabIndex = 21;
			label6.Text = "These are the resources that will be loaded when this configuration is chosen for" +
				" editing. Usually you add your IWAD (like doom.wad or doom2.wad) here.";
			// 
			// label3
			// 
			label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			label3.AutoEllipsis = true;
			label3.Location = new System.Drawing.Point(12, 15);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(443, 54);
			label3.TabIndex = 22;
			label3.Text = resources.GetString("label3.Text");
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(12, 86);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(149, 14);
			label2.TabIndex = 24;
			label2.Text = "Configuration for saving map:";
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(35, 125);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(126, 14);
			label7.TabIndex = 26;
			label7.Text = "Configuration for testing:";
			// 
			// label9
			// 
			label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			label9.AutoEllipsis = true;
			label9.Location = new System.Drawing.Point(12, 15);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(452, 54);
			label9.TabIndex = 23;
			label9.Text = "Here you can specify the program settings to use for launching a game engine when" +
				" testing the map. Press F1 for help with custom parameters.";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(15, 62);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(63, 14);
			label1.TabIndex = 24;
			label1.Text = "Application:";
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(21, 97);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(57, 14);
			label8.TabIndex = 34;
			label8.Text = "Skill Level:";
			// 
			// label4
			// 
			label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			label4.AutoEllipsis = true;
			label4.Location = new System.Drawing.Point(12, 15);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(458, 46);
			label4.TabIndex = 24;
			label4.Text = "Texture Sets are a way to group textures and flats into categories, so that you c" +
				"an easily find a texture for the specific style or purpose you need by selecting" +
				" one of the categories.";
			// 
			// label10
			// 
			label10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			label10.AutoEllipsis = true;
			label10.Location = new System.Drawing.Point(12, 15);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(445, 58);
			label10.TabIndex = 25;
			label10.Text = resources.GetString("label10.Text");
			// 
			// labelparameters
			// 
			this.labelparameters.AutoSize = true;
			this.labelparameters.Location = new System.Drawing.Point(15, 159);
			this.labelparameters.Name = "labelparameters";
			this.labelparameters.Size = new System.Drawing.Size(65, 14);
			this.labelparameters.TabIndex = 27;
			this.labelparameters.Text = "Parameters:";
			this.labelparameters.Visible = false;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(617, 381);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 3;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(499, 381);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 2;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.tabresources);
			this.tabs.Controls.Add(this.tabnodebuilder);
			this.tabs.Controls.Add(this.tabtesting);
			this.tabs.Controls.Add(this.tabtextures);
			this.tabs.Controls.Add(this.tabmodes);
			this.tabs.Enabled = false;
			this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.ItemSize = new System.Drawing.Size(100, 19);
			this.tabs.Location = new System.Drawing.Point(248, 12);
			this.tabs.Name = "tabs";
			this.tabs.Padding = new System.Drawing.Point(20, 3);
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(481, 358);
			this.tabs.TabIndex = 1;
			// 
			// tabresources
			// 
			this.tabresources.Controls.Add(label6);
			this.tabresources.Controls.Add(this.configdata);
			this.tabresources.Controls.Add(label5);
			this.tabresources.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabresources.Location = new System.Drawing.Point(4, 23);
			this.tabresources.Name = "tabresources";
			this.tabresources.Padding = new System.Windows.Forms.Padding(6);
			this.tabresources.Size = new System.Drawing.Size(473, 331);
			this.tabresources.TabIndex = 0;
			this.tabresources.Text = "Resources";
			this.tabresources.UseVisualStyleBackColor = true;
			// 
			// configdata
			// 
			this.configdata.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.configdata.DialogOffset = new System.Drawing.Point(-120, 10);
			this.configdata.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.configdata.Location = new System.Drawing.Point(15, 55);
			this.configdata.Name = "configdata";
			this.configdata.Size = new System.Drawing.Size(440, 208);
			this.configdata.TabIndex = 0;
			this.configdata.OnContentChanged += new CodeImp.DoomBuilder.Controls.ResourceListEditor.ContentChanged(this.resourcelocations_OnContentChanged);
			// 
			// tabnodebuilder
			// 
			this.tabnodebuilder.Controls.Add(label7);
			this.tabnodebuilder.Controls.Add(this.nodebuildertest);
			this.tabnodebuilder.Controls.Add(label2);
			this.tabnodebuilder.Controls.Add(this.nodebuildersave);
			this.tabnodebuilder.Controls.Add(label3);
			this.tabnodebuilder.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabnodebuilder.Location = new System.Drawing.Point(4, 23);
			this.tabnodebuilder.Name = "tabnodebuilder";
			this.tabnodebuilder.Padding = new System.Windows.Forms.Padding(6);
			this.tabnodebuilder.Size = new System.Drawing.Size(473, 331);
			this.tabnodebuilder.TabIndex = 1;
			this.tabnodebuilder.Text = "Nodebuilder";
			this.tabnodebuilder.UseVisualStyleBackColor = true;
			// 
			// nodebuildertest
			// 
			this.nodebuildertest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.nodebuildertest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.nodebuildertest.FormattingEnabled = true;
			this.nodebuildertest.Location = new System.Drawing.Point(167, 122);
			this.nodebuildertest.Name = "nodebuildertest";
			this.nodebuildertest.Size = new System.Drawing.Size(288, 22);
			this.nodebuildertest.Sorted = true;
			this.nodebuildertest.TabIndex = 1;
			this.nodebuildertest.SelectedIndexChanged += new System.EventHandler(this.nodebuildertest_SelectedIndexChanged);
			// 
			// nodebuildersave
			// 
			this.nodebuildersave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.nodebuildersave.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.nodebuildersave.FormattingEnabled = true;
			this.nodebuildersave.Location = new System.Drawing.Point(167, 83);
			this.nodebuildersave.Name = "nodebuildersave";
			this.nodebuildersave.Size = new System.Drawing.Size(288, 22);
			this.nodebuildersave.Sorted = true;
			this.nodebuildersave.TabIndex = 0;
			this.nodebuildersave.SelectedIndexChanged += new System.EventHandler(this.nodebuildersave_SelectedIndexChanged);
			// 
			// tabtesting
			// 
			this.tabtesting.Controls.Add(this.shortpaths);
			this.tabtesting.Controls.Add(this.customparameters);
			this.tabtesting.Controls.Add(this.skill);
			this.tabtesting.Controls.Add(label8);
			this.tabtesting.Controls.Add(this.browsetestprogram);
			this.tabtesting.Controls.Add(this.noresultlabel);
			this.tabtesting.Controls.Add(this.testresult);
			this.tabtesting.Controls.Add(this.labelresult);
			this.tabtesting.Controls.Add(this.testparameters);
			this.tabtesting.Controls.Add(this.labelparameters);
			this.tabtesting.Controls.Add(this.testapplication);
			this.tabtesting.Controls.Add(label1);
			this.tabtesting.Controls.Add(label9);
			this.tabtesting.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabtesting.Location = new System.Drawing.Point(4, 23);
			this.tabtesting.Name = "tabtesting";
			this.tabtesting.Padding = new System.Windows.Forms.Padding(6);
			this.tabtesting.Size = new System.Drawing.Size(473, 331);
			this.tabtesting.TabIndex = 2;
			this.tabtesting.Text = "Testing";
			this.tabtesting.UseVisualStyleBackColor = true;
			// 
			// shortpaths
			// 
			this.shortpaths.AutoSize = true;
			this.shortpaths.Location = new System.Drawing.Point(87, 203);
			this.shortpaths.Name = "shortpaths";
			this.shortpaths.Size = new System.Drawing.Size(276, 18);
			this.shortpaths.TabIndex = 5;
			this.shortpaths.Text = "Use short paths and file names (MSDOS 8.3 format)";
			this.shortpaths.UseVisualStyleBackColor = true;
			this.shortpaths.Visible = false;
			this.shortpaths.CheckedChanged += new System.EventHandler(this.shortpaths_CheckedChanged);
			// 
			// customparameters
			// 
			this.customparameters.AutoSize = true;
			this.customparameters.Location = new System.Drawing.Point(86, 132);
			this.customparameters.Name = "customparameters";
			this.customparameters.Size = new System.Drawing.Size(134, 18);
			this.customparameters.TabIndex = 3;
			this.customparameters.Text = "Customize parameters";
			this.customparameters.UseVisualStyleBackColor = true;
			this.customparameters.CheckedChanged += new System.EventHandler(this.customparameters_CheckedChanged);
			// 
			// skill
			// 
			this.skill.BackColor = System.Drawing.Color.Transparent;
			this.skill.Cursor = System.Windows.Forms.Cursors.Default;
			this.skill.Empty = false;
			this.skill.GeneralizedCategories = null;
			this.skill.Location = new System.Drawing.Point(87, 94);
			this.skill.Name = "skill";
			this.skill.Size = new System.Drawing.Size(329, 21);
			this.skill.TabIndex = 2;
			this.skill.Value = 402;
			this.skill.ValueChanges += new System.EventHandler(this.skill_ValueChanges);
			// 
			// browsetestprogram
			// 
			this.browsetestprogram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.browsetestprogram.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browsetestprogram.Image = global::CodeImp.DoomBuilder.Properties.Resources.Folder;
			this.browsetestprogram.Location = new System.Drawing.Point(422, 58);
			this.browsetestprogram.Name = "browsetestprogram";
			this.browsetestprogram.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
			this.browsetestprogram.Size = new System.Drawing.Size(30, 23);
			this.browsetestprogram.TabIndex = 1;
			this.browsetestprogram.Text = " ";
			this.browsetestprogram.UseVisualStyleBackColor = true;
			this.browsetestprogram.Click += new System.EventHandler(this.browsetestprogram_Click);
			// 
			// noresultlabel
			// 
			this.noresultlabel.Location = new System.Drawing.Point(84, 236);
			this.noresultlabel.Name = "noresultlabel";
			this.noresultlabel.Size = new System.Drawing.Size(272, 43);
			this.noresultlabel.TabIndex = 32;
			this.noresultlabel.Text = "An example result cannot be displayed, because it requires a map to be loaded.";
			this.noresultlabel.Visible = false;
			// 
			// testresult
			// 
			this.testresult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.testresult.BackColor = System.Drawing.SystemColors.Control;
			this.testresult.Location = new System.Drawing.Point(86, 233);
			this.testresult.Multiline = true;
			this.testresult.Name = "testresult";
			this.testresult.ReadOnly = true;
			this.testresult.Size = new System.Drawing.Size(366, 79);
			this.testresult.TabIndex = 6;
			this.testresult.Visible = false;
			// 
			// labelresult
			// 
			this.labelresult.AutoSize = true;
			this.labelresult.Location = new System.Drawing.Point(38, 236);
			this.labelresult.Name = "labelresult";
			this.labelresult.Size = new System.Drawing.Size(40, 14);
			this.labelresult.TabIndex = 30;
			this.labelresult.Text = "Result:";
			this.labelresult.Visible = false;
			// 
			// testparameters
			// 
			this.testparameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.testparameters.Location = new System.Drawing.Point(86, 156);
			this.testparameters.Multiline = true;
			this.testparameters.Name = "testparameters";
			this.testparameters.Size = new System.Drawing.Size(366, 41);
			this.testparameters.TabIndex = 4;
			this.testparameters.Visible = false;
			this.testparameters.TextChanged += new System.EventHandler(this.testparameters_TextChanged);
			// 
			// testapplication
			// 
			this.testapplication.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.testapplication.Location = new System.Drawing.Point(86, 59);
			this.testapplication.Name = "testapplication";
			this.testapplication.ReadOnly = true;
			this.testapplication.Size = new System.Drawing.Size(330, 20);
			this.testapplication.TabIndex = 0;
			this.testapplication.TextChanged += new System.EventHandler(this.testapplication_TextChanged);
			// 
			// tabtextures
			// 
			this.tabtextures.Controls.Add(this.listtextures);
			this.tabtextures.Controls.Add(this.restoretexturesets);
			this.tabtextures.Controls.Add(this.edittextureset);
			this.tabtextures.Controls.Add(this.pastetexturesets);
			this.tabtextures.Controls.Add(this.copytexturesets);
			this.tabtextures.Controls.Add(this.removetextureset);
			this.tabtextures.Controls.Add(this.addtextureset);
			this.tabtextures.Controls.Add(label4);
			this.tabtextures.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabtextures.Location = new System.Drawing.Point(4, 23);
			this.tabtextures.Name = "tabtextures";
			this.tabtextures.Size = new System.Drawing.Size(473, 331);
			this.tabtextures.TabIndex = 3;
			this.tabtextures.Text = "Textures";
			this.tabtextures.UseVisualStyleBackColor = true;
			// 
			// listtextures
			// 
			this.listtextures.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listtextures.FullRowSelect = true;
			this.listtextures.HideSelection = false;
			this.listtextures.Location = new System.Drawing.Point(15, 64);
			this.listtextures.Name = "listtextures";
			this.listtextures.ShowGroups = false;
			this.listtextures.Size = new System.Drawing.Size(442, 175);
			this.listtextures.SmallImageList = this.smallimages;
			this.listtextures.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listtextures.TabIndex = 0;
			this.listtextures.UseCompatibleStateImageBehavior = false;
			this.listtextures.View = System.Windows.Forms.View.List;
			this.listtextures.SelectedIndexChanged += new System.EventHandler(this.listtextures_SelectedIndexChanged);
			this.listtextures.DoubleClick += new System.EventHandler(this.listtextures_DoubleClick);
			// 
			// smallimages
			// 
			this.smallimages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("smallimages.ImageStream")));
			this.smallimages.TransparentColor = System.Drawing.Color.Transparent;
			this.smallimages.Images.SetKeyName(0, "KnownTextureSet.ico");
			// 
			// restoretexturesets
			// 
			this.restoretexturesets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.restoretexturesets.Location = new System.Drawing.Point(15, 283);
			this.restoretexturesets.Name = "restoretexturesets";
			this.restoretexturesets.Size = new System.Drawing.Size(140, 24);
			this.restoretexturesets.TabIndex = 6;
			this.restoretexturesets.Text = "Add Default Sets";
			this.restoretexturesets.UseVisualStyleBackColor = true;
			this.restoretexturesets.Click += new System.EventHandler(this.restoretexturesets_Click);
			// 
			// edittextureset
			// 
			this.edittextureset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.edittextureset.Enabled = false;
			this.edittextureset.Location = new System.Drawing.Point(88, 245);
			this.edittextureset.Name = "edittextureset";
			this.edittextureset.Size = new System.Drawing.Size(67, 24);
			this.edittextureset.TabIndex = 2;
			this.edittextureset.Text = "Edit...";
			this.edittextureset.UseVisualStyleBackColor = true;
			this.edittextureset.Click += new System.EventHandler(this.edittextureset_Click);
			// 
			// pastetexturesets
			// 
			this.pastetexturesets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.pastetexturesets.Enabled = false;
			this.pastetexturesets.Location = new System.Drawing.Point(399, 245);
			this.pastetexturesets.Name = "pastetexturesets";
			this.pastetexturesets.Size = new System.Drawing.Size(58, 24);
			this.pastetexturesets.TabIndex = 5;
			this.pastetexturesets.Text = "Paste";
			this.pastetexturesets.UseVisualStyleBackColor = true;
			this.pastetexturesets.Click += new System.EventHandler(this.pastetexturesets_Click);
			// 
			// copytexturesets
			// 
			this.copytexturesets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.copytexturesets.Enabled = false;
			this.copytexturesets.Location = new System.Drawing.Point(335, 245);
			this.copytexturesets.Name = "copytexturesets";
			this.copytexturesets.Size = new System.Drawing.Size(58, 24);
			this.copytexturesets.TabIndex = 4;
			this.copytexturesets.Text = "Copy";
			this.copytexturesets.UseVisualStyleBackColor = true;
			this.copytexturesets.Click += new System.EventHandler(this.copytexturesets_Click);
			// 
			// removetextureset
			// 
			this.removetextureset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.removetextureset.Enabled = false;
			this.removetextureset.Location = new System.Drawing.Point(161, 245);
			this.removetextureset.Name = "removetextureset";
			this.removetextureset.Size = new System.Drawing.Size(68, 24);
			this.removetextureset.TabIndex = 3;
			this.removetextureset.Text = "Remove";
			this.removetextureset.UseVisualStyleBackColor = true;
			this.removetextureset.Click += new System.EventHandler(this.removetextureset_Click);
			// 
			// addtextureset
			// 
			this.addtextureset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.addtextureset.Location = new System.Drawing.Point(15, 245);
			this.addtextureset.Name = "addtextureset";
			this.addtextureset.Size = new System.Drawing.Size(67, 24);
			this.addtextureset.TabIndex = 1;
			this.addtextureset.Text = "Add...";
			this.addtextureset.UseVisualStyleBackColor = true;
			this.addtextureset.Click += new System.EventHandler(this.addtextureset_Click);
			// 
			// tabmodes
			// 
			this.tabmodes.Controls.Add(this.startmode);
			this.tabmodes.Controls.Add(this.label11);
			this.tabmodes.Controls.Add(this.listmodes);
			this.tabmodes.Controls.Add(label10);
			this.tabmodes.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabmodes.Location = new System.Drawing.Point(4, 23);
			this.tabmodes.Name = "tabmodes";
			this.tabmodes.Size = new System.Drawing.Size(473, 331);
			this.tabmodes.TabIndex = 4;
			this.tabmodes.Text = "Modes";
			this.tabmodes.UseVisualStyleBackColor = true;
			// 
			// startmode
			// 
			this.startmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.startmode.FormattingEnabled = true;
			this.startmode.Location = new System.Drawing.Point(239, 288);
			this.startmode.Name = "startmode";
			this.startmode.Size = new System.Drawing.Size(218, 22);
			this.startmode.TabIndex = 27;
			this.startmode.SelectedIndexChanged += new System.EventHandler(this.startmode_SelectedIndexChanged);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(12, 291);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(199, 14);
			this.label11.TabIndex = 26;
			this.label11.Text = "When opening a map, start in this mode:";
			// 
			// listmodes
			// 
			this.listmodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listmodes.CheckBoxes = true;
			this.listmodes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colmodename,
            this.colmodeplugin});
			this.listmodes.FullRowSelect = true;
			this.listmodes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listmodes.Location = new System.Drawing.Point(15, 70);
			this.listmodes.MultiSelect = false;
			this.listmodes.Name = "listmodes";
			this.listmodes.ShowGroups = false;
			this.listmodes.Size = new System.Drawing.Size(442, 202);
			this.listmodes.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listmodes.TabIndex = 0;
			this.listmodes.UseCompatibleStateImageBehavior = false;
			this.listmodes.View = System.Windows.Forms.View.Details;
			this.listmodes.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listmodes_ItemChecked);
			// 
			// colmodename
			// 
			this.colmodename.Text = "Editing Mode";
			this.colmodename.Width = 179;
			// 
			// colmodeplugin
			// 
			this.colmodeplugin.Text = "Plugin";
			this.colmodeplugin.Width = 221;
			// 
			// listconfigs
			// 
			this.listconfigs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.listconfigs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnname});
			this.listconfigs.FullRowSelect = true;
			this.listconfigs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listconfigs.HideSelection = false;
			this.listconfigs.Location = new System.Drawing.Point(12, 12);
			this.listconfigs.MultiSelect = false;
			this.listconfigs.Name = "listconfigs";
			this.listconfigs.ShowGroups = false;
			this.listconfigs.Size = new System.Drawing.Size(230, 358);
			this.listconfigs.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listconfigs.TabIndex = 0;
			this.listconfigs.UseCompatibleStateImageBehavior = false;
			this.listconfigs.View = System.Windows.Forms.View.Details;
			this.listconfigs.SelectedIndexChanged += new System.EventHandler(this.listconfigs_SelectedIndexChanged);
			this.listconfigs.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listconfigs_MouseUp);
			this.listconfigs.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listconfigs_KeyUp);
			// 
			// columnname
			// 
			this.columnname.Text = "Configuration";
			this.columnname.Width = 200;
			// 
			// testprogramdialog
			// 
			this.testprogramdialog.Filter = "Executable Files (*.exe)|*.exe|Batch Files (*.bat)|*.bat";
			this.testprogramdialog.Title = "Browse Test Program";
			// 
			// ConfigForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(740, 416);
			this.Controls.Add(this.listconfigs);
			this.Controls.Add(this.tabs);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConfigForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Game Configurations";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ConfigForm_HelpRequested);
			this.tabs.ResumeLayout(false);
			this.tabresources.ResumeLayout(false);
			this.tabresources.PerformLayout();
			this.tabnodebuilder.ResumeLayout(false);
			this.tabnodebuilder.PerformLayout();
			this.tabtesting.ResumeLayout(false);
			this.tabtesting.PerformLayout();
			this.tabtextures.ResumeLayout(false);
			this.tabmodes.ResumeLayout(false);
			this.tabmodes.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabresources;
		private System.Windows.Forms.TabPage tabnodebuilder;
		private System.Windows.Forms.TabPage tabtesting;
		private CodeImp.DoomBuilder.Controls.ResourceListEditor configdata;
		private System.Windows.Forms.ComboBox nodebuildertest;
		private System.Windows.Forms.ComboBox nodebuildersave;
		private System.Windows.Forms.TextBox testparameters;
		private System.Windows.Forms.TextBox testapplication;
		private System.Windows.Forms.TextBox testresult;
		private System.Windows.Forms.Label labelresult;
		private System.Windows.Forms.ListView listconfigs;
		private System.Windows.Forms.ColumnHeader columnname;
		private System.Windows.Forms.Label noresultlabel;
		private System.Windows.Forms.Button browsetestprogram;
		private System.Windows.Forms.OpenFileDialog testprogramdialog;
		private System.Windows.Forms.CheckBox customparameters;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl skill;
		private System.Windows.Forms.Label labelparameters;
		private System.Windows.Forms.TabPage tabtextures;
		private System.Windows.Forms.Button addtextureset;
		private System.Windows.Forms.Button restoretexturesets;
		private System.Windows.Forms.Button edittextureset;
		private System.Windows.Forms.Button pastetexturesets;
		private System.Windows.Forms.Button copytexturesets;
		private System.Windows.Forms.Button removetextureset;
		private System.Windows.Forms.ListView listtextures;
		private System.Windows.Forms.ImageList smallimages;
		private System.Windows.Forms.TabPage tabmodes;
		private System.Windows.Forms.ListView listmodes;
		private System.Windows.Forms.ColumnHeader colmodename;
		private System.Windows.Forms.ColumnHeader colmodeplugin;
		private System.Windows.Forms.CheckBox shortpaths;
		private System.Windows.Forms.ComboBox startmode;
		private System.Windows.Forms.Label label11;
	}
}