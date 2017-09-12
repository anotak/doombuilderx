namespace CodeImp.DoomBuilder.Windows
{
	partial class ThingEditForm
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
            System.Windows.Forms.GroupBox groupBox1;
            System.Windows.Forms.GroupBox groupBox2;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label taglabel;
            System.Windows.Forms.Label label7;
            this.thingtype = new CodeImp.DoomBuilder.Controls.ThingBrowserControl();
            this.height = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.angle = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.heightlabel = new System.Windows.Forms.Label();
            this.anglecontrol = new CodeImp.DoomBuilder.Controls.AngleControl();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabproperties = new System.Windows.Forms.TabPage();
            this.spritetex = new System.Windows.Forms.Panel();
            this.settingsgroup = new System.Windows.Forms.GroupBox();
            this.flags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
            this.tabeffects = new System.Windows.Forms.TabPage();
            this.actiongroup = new System.Windows.Forms.GroupBox();
            this.hexenpanel = new System.Windows.Forms.Panel();
            this.arg2 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
            this.arg1 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
            this.arg0 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
            this.arg3 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
            this.arg4 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
            this.arg1label = new System.Windows.Forms.Label();
            this.arg0label = new System.Windows.Forms.Label();
            this.arg3label = new System.Windows.Forms.Label();
            this.arg2label = new System.Windows.Forms.Label();
            this.arg4label = new System.Windows.Forms.Label();
            this.action = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
            this.browseaction = new System.Windows.Forms.Button();
            this.doompanel = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tag = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.newtag = new System.Windows.Forms.Button();
            this.tabcustom = new System.Windows.Forms.TabPage();
            this.fieldslist = new CodeImp.DoomBuilder.Controls.FieldsEditorControl();
            this.cancel = new System.Windows.Forms.Button();
            this.apply = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            label5 = new System.Windows.Forms.Label();
            taglabel = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabproperties.SuspendLayout();
            this.settingsgroup.SuspendLayout();
            this.tabeffects.SuspendLayout();
            this.actiongroup.SuspendLayout();
            this.hexenpanel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabcustom.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            groupBox1.Controls.Add(this.thingtype);
            groupBox1.Location = new System.Drawing.Point(8, 8);
            groupBox1.Margin = new System.Windows.Forms.Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4);
            groupBox1.Size = new System.Drawing.Size(336, 669);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = " Thing ";
            // 
            // thingtype
            // 
            this.thingtype.AutoSize = true;
            this.thingtype.Dock = System.Windows.Forms.DockStyle.Fill;
            this.thingtype.Location = new System.Drawing.Point(4, 20);
            this.thingtype.Margin = new System.Windows.Forms.Padding(8);
            this.thingtype.Name = "thingtype";
            this.thingtype.Size = new System.Drawing.Size(328, 645);
            this.thingtype.TabIndex = 0;
            this.thingtype.OnTypeChanged += new CodeImp.DoomBuilder.Controls.ThingBrowserControl.TypeChangedDeletegate(this.thingtype_OnTypeChanged);
            // 
            // groupBox2
            // 
            groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            groupBox2.Controls.Add(this.height);
            groupBox2.Controls.Add(this.angle);
            groupBox2.Controls.Add(this.heightlabel);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(this.anglecontrol);
            groupBox2.Location = new System.Drawing.Point(496, 546);
            groupBox2.Margin = new System.Windows.Forms.Padding(4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(4);
            groupBox2.Size = new System.Drawing.Size(311, 131);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = " Coordination ";
            // 
            // height
            // 
            this.height.AllowDecimal = false;
            this.height.AllowNegative = true;
            this.height.AllowRelative = true;
            this.height.ButtonStep = 8;
            this.height.Location = new System.Drawing.Point(85, 76);
            this.height.Margin = new System.Windows.Forms.Padding(5);
            this.height.Name = "height";
            this.height.Size = new System.Drawing.Size(90, 27);
            this.height.StepValues = null;
            this.height.TabIndex = 11;
            // 
            // angle
            // 
            this.angle.AllowDecimal = false;
            this.angle.AllowNegative = true;
            this.angle.AllowRelative = true;
            this.angle.ButtonStep = 45;
            this.angle.Location = new System.Drawing.Point(85, 32);
            this.angle.Margin = new System.Windows.Forms.Padding(5);
            this.angle.Name = "angle";
            this.angle.Size = new System.Drawing.Size(90, 27);
            this.angle.StepValues = null;
            this.angle.TabIndex = 10;
            this.angle.WhenTextChanged += new System.EventHandler(this.angle_TextChanged);
            // 
            // heightlabel
            // 
            this.heightlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.heightlabel.AutoSize = true;
            this.heightlabel.Location = new System.Drawing.Point(15, 82);
            this.heightlabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.heightlabel.Name = "heightlabel";
            this.heightlabel.Size = new System.Drawing.Size(64, 16);
            this.heightlabel.TabIndex = 9;
            this.heightlabel.Text = "Z Height:";
            // 
            // label5
            // 
            label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(30, 39);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(48, 16);
            label5.TabIndex = 8;
            label5.Text = "Angle:";
            // 
            // anglecontrol
            // 
            this.anglecontrol.BackColor = System.Drawing.SystemColors.Control;
            this.anglecontrol.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.anglecontrol.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.anglecontrol.Location = new System.Drawing.Point(195, 16);
            this.anglecontrol.Margin = new System.Windows.Forms.Padding(5);
            this.anglecontrol.Name = "anglecontrol";
            this.anglecontrol.Size = new System.Drawing.Size(104, 104);
            this.anglecontrol.TabIndex = 2;
            this.anglecontrol.Value = 0;
            this.anglecontrol.ButtonClicked += new System.EventHandler(this.anglecontrol_ButtonClicked);
            // 
            // taglabel
            // 
            taglabel.AutoSize = true;
            taglabel.Location = new System.Drawing.Point(35, 39);
            taglabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            taglabel.Name = "taglabel";
            taglabel.Size = new System.Drawing.Size(35, 16);
            taglabel.TabIndex = 6;
            taglabel.Text = "Tag:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(19, 38);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(51, 16);
            label7.TabIndex = 9;
            label7.Text = "Action:";
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Controls.Add(this.tabproperties);
            this.tabs.Controls.Add(this.tabeffects);
            this.tabs.Controls.Add(this.tabcustom);
            this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Margin = new System.Windows.Forms.Padding(1);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(825, 721);
            this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabs.TabIndex = 0;
            // 
            // tabproperties
            // 
            this.tabproperties.Controls.Add(this.spritetex);
            this.tabproperties.Controls.Add(groupBox2);
            this.tabproperties.Controls.Add(this.settingsgroup);
            this.tabproperties.Controls.Add(groupBox1);
            this.tabproperties.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabproperties.Location = new System.Drawing.Point(4, 25);
            this.tabproperties.Margin = new System.Windows.Forms.Padding(4);
            this.tabproperties.Name = "tabproperties";
            this.tabproperties.Padding = new System.Windows.Forms.Padding(4);
            this.tabproperties.Size = new System.Drawing.Size(817, 692);
            this.tabproperties.TabIndex = 0;
            this.tabproperties.Text = "Properties";
            this.tabproperties.UseVisualStyleBackColor = true;
            // 
            // spritetex
            // 
            this.spritetex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.spritetex.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.spritetex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.spritetex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.spritetex.Location = new System.Drawing.Point(355, 552);
            this.spritetex.Margin = new System.Windows.Forms.Padding(4);
            this.spritetex.Name = "spritetex";
            this.spritetex.Size = new System.Drawing.Size(129, 124);
            this.spritetex.TabIndex = 22;
            // 
            // settingsgroup
            // 
            this.settingsgroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsgroup.Controls.Add(this.flags);
            this.settingsgroup.Location = new System.Drawing.Point(355, 8);
            this.settingsgroup.Margin = new System.Windows.Forms.Padding(4);
            this.settingsgroup.Name = "settingsgroup";
            this.settingsgroup.Padding = new System.Windows.Forms.Padding(4);
            this.settingsgroup.Size = new System.Drawing.Size(452, 531);
            this.settingsgroup.TabIndex = 1;
            this.settingsgroup.TabStop = false;
            this.settingsgroup.Text = " Settings ";
            // 
            // flags
            // 
            this.flags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flags.AutoScroll = true;
            this.flags.Columns = 2;
            this.flags.Location = new System.Drawing.Point(22, 32);
            this.flags.Margin = new System.Windows.Forms.Padding(5);
            this.flags.Name = "flags";
            this.flags.Size = new System.Drawing.Size(422, 489);
            this.flags.TabIndex = 0;
            // 
            // tabeffects
            // 
            this.tabeffects.Controls.Add(this.actiongroup);
            this.tabeffects.Controls.Add(this.groupBox3);
            this.tabeffects.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabeffects.Location = new System.Drawing.Point(4, 25);
            this.tabeffects.Margin = new System.Windows.Forms.Padding(4);
            this.tabeffects.Name = "tabeffects";
            this.tabeffects.Padding = new System.Windows.Forms.Padding(4);
            this.tabeffects.Size = new System.Drawing.Size(817, 692);
            this.tabeffects.TabIndex = 1;
            this.tabeffects.Text = "Action";
            this.tabeffects.UseVisualStyleBackColor = true;
            // 
            // actiongroup
            // 
            this.actiongroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.actiongroup.Controls.Add(this.hexenpanel);
            this.actiongroup.Controls.Add(label7);
            this.actiongroup.Controls.Add(this.action);
            this.actiongroup.Controls.Add(this.browseaction);
            this.actiongroup.Controls.Add(this.doompanel);
            this.actiongroup.Location = new System.Drawing.Point(8, 98);
            this.actiongroup.Margin = new System.Windows.Forms.Padding(4);
            this.actiongroup.Name = "actiongroup";
            this.actiongroup.Padding = new System.Windows.Forms.Padding(4);
            this.actiongroup.Size = new System.Drawing.Size(800, 579);
            this.actiongroup.TabIndex = 22;
            this.actiongroup.TabStop = false;
            this.actiongroup.Text = " Action ";
            // 
            // hexenpanel
            // 
            this.hexenpanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexenpanel.Controls.Add(this.arg2);
            this.hexenpanel.Controls.Add(this.arg1);
            this.hexenpanel.Controls.Add(this.arg0);
            this.hexenpanel.Controls.Add(this.arg3);
            this.hexenpanel.Controls.Add(this.arg4);
            this.hexenpanel.Controls.Add(this.arg1label);
            this.hexenpanel.Controls.Add(this.arg0label);
            this.hexenpanel.Controls.Add(this.arg3label);
            this.hexenpanel.Controls.Add(this.arg2label);
            this.hexenpanel.Controls.Add(this.arg4label);
            this.hexenpanel.Location = new System.Drawing.Point(8, 66);
            this.hexenpanel.Margin = new System.Windows.Forms.Padding(4);
            this.hexenpanel.Name = "hexenpanel";
            this.hexenpanel.Size = new System.Drawing.Size(785, 505);
            this.hexenpanel.TabIndex = 13;
            // 
            // arg2
            // 
            this.arg2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.arg2.Location = new System.Drawing.Point(224, 79);
            this.arg2.Margin = new System.Windows.Forms.Padding(5);
            this.arg2.Name = "arg2";
            this.arg2.Size = new System.Drawing.Size(116, 30);
            this.arg2.TabIndex = 2;
            // 
            // arg1
            // 
            this.arg1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.arg1.Location = new System.Drawing.Point(224, 46);
            this.arg1.Margin = new System.Windows.Forms.Padding(5);
            this.arg1.Name = "arg1";
            this.arg1.Size = new System.Drawing.Size(116, 30);
            this.arg1.TabIndex = 1;
            // 
            // arg0
            // 
            this.arg0.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.arg0.Location = new System.Drawing.Point(224, 14);
            this.arg0.Margin = new System.Windows.Forms.Padding(5);
            this.arg0.Name = "arg0";
            this.arg0.Size = new System.Drawing.Size(116, 30);
            this.arg0.TabIndex = 0;
            // 
            // arg3
            // 
            this.arg3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.arg3.Location = new System.Drawing.Point(531, 14);
            this.arg3.Margin = new System.Windows.Forms.Padding(5);
            this.arg3.Name = "arg3";
            this.arg3.Size = new System.Drawing.Size(116, 30);
            this.arg3.TabIndex = 3;
            // 
            // arg4
            // 
            this.arg4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.arg4.Location = new System.Drawing.Point(531, 46);
            this.arg4.Margin = new System.Windows.Forms.Padding(5);
            this.arg4.Name = "arg4";
            this.arg4.Size = new System.Drawing.Size(116, 30);
            this.arg4.TabIndex = 4;
            // 
            // arg1label
            // 
            this.arg1label.Location = new System.Drawing.Point(-8, 52);
            this.arg1label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arg1label.Name = "arg1label";
            this.arg1label.Size = new System.Drawing.Size(224, 18);
            this.arg1label.TabIndex = 14;
            this.arg1label.Text = "Argument 2:";
            this.arg1label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arg1label.UseMnemonic = false;
            // 
            // arg0label
            // 
            this.arg0label.Location = new System.Drawing.Point(-8, 20);
            this.arg0label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arg0label.Name = "arg0label";
            this.arg0label.Size = new System.Drawing.Size(224, 18);
            this.arg0label.TabIndex = 12;
            this.arg0label.Text = "Argument 1:";
            this.arg0label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arg0label.UseMnemonic = false;
            // 
            // arg3label
            // 
            this.arg3label.Location = new System.Drawing.Point(300, 20);
            this.arg3label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arg3label.Name = "arg3label";
            this.arg3label.Size = new System.Drawing.Size(224, 18);
            this.arg3label.TabIndex = 20;
            this.arg3label.Text = "Argument 4:";
            this.arg3label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arg3label.UseMnemonic = false;
            // 
            // arg2label
            // 
            this.arg2label.Location = new System.Drawing.Point(-8, 85);
            this.arg2label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arg2label.Name = "arg2label";
            this.arg2label.Size = new System.Drawing.Size(224, 18);
            this.arg2label.TabIndex = 18;
            this.arg2label.Text = "Argument 3:";
            this.arg2label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arg2label.UseMnemonic = false;
            // 
            // arg4label
            // 
            this.arg4label.Location = new System.Drawing.Point(300, 52);
            this.arg4label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arg4label.Name = "arg4label";
            this.arg4label.Size = new System.Drawing.Size(224, 18);
            this.arg4label.TabIndex = 16;
            this.arg4label.Text = "Argument 5:";
            this.arg4label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arg4label.UseMnemonic = false;
            // 
            // action
            // 
            this.action.BackColor = System.Drawing.SystemColors.Control;
            this.action.Cursor = System.Windows.Forms.Cursors.Default;
            this.action.Empty = false;
            this.action.GeneralizedCategories = null;
            this.action.Location = new System.Drawing.Point(78, 34);
            this.action.Margin = new System.Windows.Forms.Padding(5);
            this.action.Name = "action";
            this.action.Size = new System.Drawing.Size(578, 24);
            this.action.TabIndex = 0;
            this.action.Value = 402;
            this.action.ValueChanges += new System.EventHandler(this.action_ValueChanges);
            // 
            // browseaction
            // 
            this.browseaction.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.browseaction.Image = global::CodeImp.DoomBuilder.Properties.Resources.List;
            this.browseaction.Location = new System.Drawing.Point(662, 31);
            this.browseaction.Margin = new System.Windows.Forms.Padding(4);
            this.browseaction.Name = "browseaction";
            this.browseaction.Padding = new System.Windows.Forms.Padding(0, 0, 1, 4);
            this.browseaction.Size = new System.Drawing.Size(35, 31);
            this.browseaction.TabIndex = 1;
            this.browseaction.Text = " ";
            this.browseaction.UseVisualStyleBackColor = true;
            this.browseaction.Click += new System.EventHandler(this.browseaction_Click);
            // 
            // doompanel
            // 
            this.doompanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.doompanel.Location = new System.Drawing.Point(8, 68);
            this.doompanel.Margin = new System.Windows.Forms.Padding(4);
            this.doompanel.Name = "doompanel";
            this.doompanel.Size = new System.Drawing.Size(785, 505);
            this.doompanel.TabIndex = 12;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tag);
            this.groupBox3.Controls.Add(taglabel);
            this.groupBox3.Controls.Add(this.newtag);
            this.groupBox3.Location = new System.Drawing.Point(8, 8);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(800, 82);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = " Identification ";
            // 
            // tag
            // 
            this.tag.AllowDecimal = false;
            this.tag.AllowNegative = false;
            this.tag.AllowRelative = true;
            this.tag.ButtonStep = 1;
            this.tag.Location = new System.Drawing.Point(78, 32);
            this.tag.Margin = new System.Windows.Forms.Padding(5);
            this.tag.Name = "tag";
            this.tag.Size = new System.Drawing.Size(100, 27);
            this.tag.StepValues = null;
            this.tag.TabIndex = 7;
            // 
            // newtag
            // 
            this.newtag.Location = new System.Drawing.Point(192, 34);
            this.newtag.Margin = new System.Windows.Forms.Padding(4);
            this.newtag.Name = "newtag";
            this.newtag.Size = new System.Drawing.Size(95, 29);
            this.newtag.TabIndex = 1;
            this.newtag.Text = "New Tag";
            this.newtag.UseVisualStyleBackColor = true;
            this.newtag.Click += new System.EventHandler(this.newtag_Click);
            // 
            // tabcustom
            // 
            this.tabcustom.Controls.Add(this.fieldslist);
            this.tabcustom.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabcustom.Location = new System.Drawing.Point(4, 25);
            this.tabcustom.Margin = new System.Windows.Forms.Padding(4);
            this.tabcustom.Name = "tabcustom";
            this.tabcustom.Size = new System.Drawing.Size(817, 692);
            this.tabcustom.TabIndex = 2;
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
            this.fieldslist.Location = new System.Drawing.Point(10, 11);
            this.fieldslist.Margin = new System.Windows.Forms.Padding(10, 11, 10, 11);
            this.fieldslist.Name = "fieldslist";
            this.fieldslist.PropertyColumnVisible = true;
            this.fieldslist.PropertyColumnWidth = 150;
            this.fieldslist.Size = new System.Drawing.Size(794, 661);
            this.fieldslist.TabIndex = 1;
            this.fieldslist.TypeColumnVisible = true;
            this.fieldslist.TypeColumnWidth = 100;
            this.fieldslist.ValueColumnVisible = true;
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(698, 741);
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
            this.apply.Location = new System.Drawing.Point(549, 741);
            this.apply.Margin = new System.Windows.Forms.Padding(4);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(140, 31);
            this.apply.TabIndex = 1;
            this.apply.Text = "OK";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // ThingEditForm
            // 
            this.AcceptButton = this.apply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(850, 784);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.apply);
            this.Controls.Add(this.tabs);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ThingEditForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Thing";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ThingEditForm_HelpRequested);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            this.tabs.ResumeLayout(false);
            this.tabproperties.ResumeLayout(false);
            this.settingsgroup.ResumeLayout(false);
            this.tabeffects.ResumeLayout(false);
            this.actiongroup.ResumeLayout(false);
            this.actiongroup.PerformLayout();
            this.hexenpanel.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabcustom.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabproperties;
		private System.Windows.Forms.TabPage tabeffects;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.TabPage tabcustom;
		private System.Windows.Forms.GroupBox settingsgroup;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl flags;
		private System.Windows.Forms.Panel spritetex;
		private CodeImp.DoomBuilder.Controls.AngleControl anglecontrol;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button newtag;
		private System.Windows.Forms.GroupBox actiongroup;
		private System.Windows.Forms.Panel hexenpanel;
		private System.Windows.Forms.Label arg1label;
		private System.Windows.Forms.Label arg0label;
		private System.Windows.Forms.Label arg3label;
		private System.Windows.Forms.Label arg2label;
		private System.Windows.Forms.Label arg4label;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl action;
		private System.Windows.Forms.Button browseaction;
		private System.Windows.Forms.Panel doompanel;
		private CodeImp.DoomBuilder.Controls.FieldsEditorControl fieldslist;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg2;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg1;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg0;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg3;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg4;
		private System.Windows.Forms.Label heightlabel;
		private CodeImp.DoomBuilder.Controls.ThingBrowserControl thingtype;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox angle;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox height;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox tag;
	}
}