namespace CodeImp.DoomBuilder.Windows
{
	partial class LinedefEditForm
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
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label taglabel;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.Label label9;
            System.Windows.Forms.Label label10;
            System.Windows.Forms.Label label11;
            System.Windows.Forms.Label label12;
            System.Windows.Forms.Label activationlabel;
            this.cancel = new System.Windows.Forms.Button();
            this.apply = new System.Windows.Forms.Button();
            this.actiongroup = new System.Windows.Forms.GroupBox();
            this.argspanel = new System.Windows.Forms.Panel();
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
            this.hexenpanel = new System.Windows.Forms.Panel();
            this.activation = new System.Windows.Forms.ComboBox();
            this.action = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
            this.browseaction = new System.Windows.Forms.Button();
            this.udmfpanel = new System.Windows.Forms.Panel();
            this.udmfactivates = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
            this.newtag = new System.Windows.Forms.Button();
            this.settingsgroup = new System.Windows.Forms.GroupBox();
            this.flags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabproperties = new System.Windows.Forms.TabPage();
            this.idgroup = new System.Windows.Forms.GroupBox();
            this.tag = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.tabsidedefs = new System.Windows.Forms.TabPage();
            this.splitter = new System.Windows.Forms.SplitContainer();
            this.frontside = new System.Windows.Forms.CheckBox();
            this.frontgroup = new System.Windows.Forms.GroupBox();
            this.frontoffsety = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.frontoffsetx = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.frontsector = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.customfrontbutton = new System.Windows.Forms.Button();
            this.frontlow = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
            this.frontmid = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
            this.fronthigh = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
            this.backside = new System.Windows.Forms.CheckBox();
            this.backgroup = new System.Windows.Forms.GroupBox();
            this.backoffsety = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.backoffsetx = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.backsector = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.custombackbutton = new System.Windows.Forms.Button();
            this.backlow = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
            this.backmid = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
            this.backhigh = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
            this.tabcustom = new System.Windows.Forms.TabPage();
            this.fieldslist = new CodeImp.DoomBuilder.Controls.FieldsEditorControl();
            this.heightpanel1 = new System.Windows.Forms.Panel();
            this.heightpanel2 = new System.Windows.Forms.Panel();
            label2 = new System.Windows.Forms.Label();
            taglabel = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            activationlabel = new System.Windows.Forms.Label();
            this.actiongroup.SuspendLayout();
            this.argspanel.SuspendLayout();
            this.hexenpanel.SuspendLayout();
            this.udmfpanel.SuspendLayout();
            this.settingsgroup.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabproperties.SuspendLayout();
            this.idgroup.SuspendLayout();
            this.tabsidedefs.SuspendLayout();
            this.splitter.Panel1.SuspendLayout();
            this.splitter.Panel2.SuspendLayout();
            this.splitter.SuspendLayout();
            this.frontgroup.SuspendLayout();
            this.backgroup.SuspendLayout();
            this.tabcustom.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(19, 28);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(51, 16);
            label2.TabIndex = 9;
            label2.Text = "Action:";
            // 
            // taglabel
            // 
            taglabel.AutoSize = true;
            taglabel.Location = new System.Drawing.Point(35, 25);
            taglabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            taglabel.Name = "taglabel";
            taglabel.Size = new System.Drawing.Size(35, 16);
            taglabel.TabIndex = 6;
            taglabel.Text = "Tag:";
            // 
            // label3
            // 
            label3.Location = new System.Drawing.Point(315, 22);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(104, 20);
            label3.TabIndex = 3;
            label3.Text = "Upper";
            label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(429, 22);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(104, 20);
            label4.TabIndex = 4;
            label4.Text = "Middle";
            label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label5
            // 
            label5.Location = new System.Drawing.Point(542, 22);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(104, 20);
            label5.TabIndex = 5;
            label5.Text = "Lower";
            label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(20, 99);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(100, 16);
            label6.TabIndex = 7;
            label6.Text = "Texture Offset:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(20, 99);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(100, 16);
            label7.TabIndex = 7;
            label7.Text = "Texture Offset:";
            // 
            // label8
            // 
            label8.Location = new System.Drawing.Point(546, 22);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(104, 20);
            label8.TabIndex = 5;
            label8.Text = "Lower";
            label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label9
            // 
            label9.Location = new System.Drawing.Point(432, 22);
            label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(104, 20);
            label9.TabIndex = 4;
            label9.Text = "Middle";
            label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label10
            // 
            label10.Location = new System.Drawing.Point(319, 22);
            label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(104, 20);
            label10.TabIndex = 3;
            label10.Text = "Upper";
            label10.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(32, 50);
            label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(90, 16);
            label11.TabIndex = 13;
            label11.Text = "Sector Index:";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(32, 50);
            label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(90, 16);
            label12.TabIndex = 16;
            label12.Text = "Sector Index:";
            // 
            // activationlabel
            // 
            activationlabel.AutoSize = true;
            activationlabel.Location = new System.Drawing.Point(8, 21);
            activationlabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            activationlabel.Name = "activationlabel";
            activationlabel.Size = new System.Drawing.Size(57, 16);
            activationlabel.TabIndex = 10;
            activationlabel.Text = "Trigger:";
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(569, 740);
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
            this.apply.Location = new System.Drawing.Point(420, 740);
            this.apply.Margin = new System.Windows.Forms.Padding(4);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(140, 31);
            this.apply.TabIndex = 1;
            this.apply.Text = "OK";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // actiongroup
            // 
            this.actiongroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.actiongroup.Controls.Add(this.argspanel);
            this.actiongroup.Controls.Add(this.hexenpanel);
            this.actiongroup.Controls.Add(label2);
            this.actiongroup.Controls.Add(this.action);
            this.actiongroup.Controls.Add(this.browseaction);
            this.actiongroup.Controls.Add(this.udmfpanel);
            this.actiongroup.Location = new System.Drawing.Point(10, 221);
            this.actiongroup.Margin = new System.Windows.Forms.Padding(4);
            this.actiongroup.Name = "actiongroup";
            this.actiongroup.Padding = new System.Windows.Forms.Padding(4);
            this.actiongroup.Size = new System.Drawing.Size(666, 395);
            this.actiongroup.TabIndex = 1;
            this.actiongroup.TabStop = false;
            this.actiongroup.Text = " Action ";
            // 
            // argspanel
            // 
            this.argspanel.Controls.Add(this.arg2);
            this.argspanel.Controls.Add(this.arg1);
            this.argspanel.Controls.Add(this.arg0);
            this.argspanel.Controls.Add(this.arg3);
            this.argspanel.Controls.Add(this.arg4);
            this.argspanel.Controls.Add(this.arg1label);
            this.argspanel.Controls.Add(this.arg0label);
            this.argspanel.Controls.Add(this.arg3label);
            this.argspanel.Controls.Add(this.arg2label);
            this.argspanel.Controls.Add(this.arg4label);
            this.argspanel.Location = new System.Drawing.Point(8, 58);
            this.argspanel.Margin = new System.Windows.Forms.Padding(4);
            this.argspanel.Name = "argspanel";
            this.argspanel.Size = new System.Drawing.Size(651, 104);
            this.argspanel.TabIndex = 2;
            this.argspanel.Visible = false;
            // 
            // arg2
            // 
            this.arg2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.arg2.Location = new System.Drawing.Point(154, 69);
            this.arg2.Margin = new System.Windows.Forms.Padding(5);
            this.arg2.Name = "arg2";
            this.arg2.Size = new System.Drawing.Size(116, 30);
            this.arg2.TabIndex = 2;
            // 
            // arg1
            // 
            this.arg1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.arg1.Location = new System.Drawing.Point(154, 36);
            this.arg1.Margin = new System.Windows.Forms.Padding(5);
            this.arg1.Name = "arg1";
            this.arg1.Size = new System.Drawing.Size(116, 30);
            this.arg1.TabIndex = 1;
            // 
            // arg0
            // 
            this.arg0.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.arg0.Location = new System.Drawing.Point(154, 4);
            this.arg0.Margin = new System.Windows.Forms.Padding(5);
            this.arg0.Name = "arg0";
            this.arg0.Size = new System.Drawing.Size(116, 30);
            this.arg0.TabIndex = 0;
            // 
            // arg3
            // 
            this.arg3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.arg3.Location = new System.Drawing.Point(455, 4);
            this.arg3.Margin = new System.Windows.Forms.Padding(5);
            this.arg3.Name = "arg3";
            this.arg3.Size = new System.Drawing.Size(116, 30);
            this.arg3.TabIndex = 3;
            // 
            // arg4
            // 
            this.arg4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.arg4.Location = new System.Drawing.Point(455, 36);
            this.arg4.Margin = new System.Windows.Forms.Padding(5);
            this.arg4.Name = "arg4";
            this.arg4.Size = new System.Drawing.Size(116, 30);
            this.arg4.TabIndex = 4;
            // 
            // arg1label
            // 
            this.arg1label.Location = new System.Drawing.Point(-78, 42);
            this.arg1label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arg1label.Name = "arg1label";
            this.arg1label.Size = new System.Drawing.Size(224, 18);
            this.arg1label.TabIndex = 33;
            this.arg1label.Text = "Argument 2:";
            this.arg1label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arg1label.UseMnemonic = false;
            // 
            // arg0label
            // 
            this.arg0label.Location = new System.Drawing.Point(-78, 10);
            this.arg0label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arg0label.Name = "arg0label";
            this.arg0label.Size = new System.Drawing.Size(224, 18);
            this.arg0label.TabIndex = 32;
            this.arg0label.Text = "Argument 1:";
            this.arg0label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arg0label.UseMnemonic = false;
            // 
            // arg3label
            // 
            this.arg3label.Location = new System.Drawing.Point(224, 10);
            this.arg3label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arg3label.Name = "arg3label";
            this.arg3label.Size = new System.Drawing.Size(224, 18);
            this.arg3label.TabIndex = 36;
            this.arg3label.Text = "Argument 4:";
            this.arg3label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arg3label.UseMnemonic = false;
            // 
            // arg2label
            // 
            this.arg2label.Location = new System.Drawing.Point(-78, 75);
            this.arg2label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arg2label.Name = "arg2label";
            this.arg2label.Size = new System.Drawing.Size(224, 18);
            this.arg2label.TabIndex = 35;
            this.arg2label.Text = "Argument 3:";
            this.arg2label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arg2label.UseMnemonic = false;
            // 
            // arg4label
            // 
            this.arg4label.Location = new System.Drawing.Point(224, 42);
            this.arg4label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.arg4label.Name = "arg4label";
            this.arg4label.Size = new System.Drawing.Size(224, 18);
            this.arg4label.TabIndex = 34;
            this.arg4label.Text = "Argument 5:";
            this.arg4label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arg4label.UseMnemonic = false;
            // 
            // hexenpanel
            // 
            this.hexenpanel.Controls.Add(this.activation);
            this.hexenpanel.Controls.Add(activationlabel);
            this.hexenpanel.Location = new System.Drawing.Point(8, 164);
            this.hexenpanel.Margin = new System.Windows.Forms.Padding(4);
            this.hexenpanel.Name = "hexenpanel";
            this.hexenpanel.Size = new System.Drawing.Size(651, 61);
            this.hexenpanel.TabIndex = 3;
            this.hexenpanel.Visible = false;
            // 
            // activation
            // 
            this.activation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.activation.FormattingEnabled = true;
            this.activation.Location = new System.Drawing.Point(70, 16);
            this.activation.Margin = new System.Windows.Forms.Padding(4);
            this.activation.Name = "activation";
            this.activation.Size = new System.Drawing.Size(545, 24);
            this.activation.TabIndex = 0;
            // 
            // action
            // 
            this.action.BackColor = System.Drawing.Color.Transparent;
            this.action.Cursor = System.Windows.Forms.Cursors.Default;
            this.action.Empty = false;
            this.action.GeneralizedCategories = null;
            this.action.Location = new System.Drawing.Point(78, 24);
            this.action.Margin = new System.Windows.Forms.Padding(5);
            this.action.Name = "action";
            this.action.Size = new System.Drawing.Size(501, 24);
            this.action.TabIndex = 0;
            this.action.Value = 402;
            this.action.ValueChanges += new System.EventHandler(this.action_ValueChanges);
            // 
            // browseaction
            // 
            this.browseaction.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.browseaction.Image = global::CodeImp.DoomBuilder.Properties.Resources.List;
            this.browseaction.Location = new System.Drawing.Point(586, 21);
            this.browseaction.Margin = new System.Windows.Forms.Padding(4);
            this.browseaction.Name = "browseaction";
            this.browseaction.Padding = new System.Windows.Forms.Padding(0, 0, 1, 4);
            this.browseaction.Size = new System.Drawing.Size(35, 31);
            this.browseaction.TabIndex = 1;
            this.browseaction.Text = " ";
            this.browseaction.UseVisualStyleBackColor = true;
            this.browseaction.Click += new System.EventHandler(this.browseaction_Click);
            // 
            // udmfpanel
            // 
            this.udmfpanel.Controls.Add(this.udmfactivates);
            this.udmfpanel.Location = new System.Drawing.Point(8, 179);
            this.udmfpanel.Margin = new System.Windows.Forms.Padding(4);
            this.udmfpanel.Name = "udmfpanel";
            this.udmfpanel.Size = new System.Drawing.Size(631, 210);
            this.udmfpanel.TabIndex = 4;
            this.udmfpanel.Visible = false;
            // 
            // udmfactivates
            // 
            this.udmfactivates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.udmfactivates.AutoScroll = true;
            this.udmfactivates.Columns = 2;
            this.udmfactivates.Location = new System.Drawing.Point(70, 0);
            this.udmfactivates.Margin = new System.Windows.Forms.Padding(5);
            this.udmfactivates.Name = "udmfactivates";
            this.udmfactivates.Size = new System.Drawing.Size(546, 209);
            this.udmfactivates.TabIndex = 0;
            // 
            // newtag
            // 
            this.newtag.Location = new System.Drawing.Point(186, 22);
            this.newtag.Margin = new System.Windows.Forms.Padding(4);
            this.newtag.Name = "newtag";
            this.newtag.Size = new System.Drawing.Size(95, 29);
            this.newtag.TabIndex = 1;
            this.newtag.Text = "New Tag";
            this.newtag.UseVisualStyleBackColor = true;
            this.newtag.Click += new System.EventHandler(this.newtag_Click);
            // 
            // settingsgroup
            // 
            this.settingsgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsgroup.Controls.Add(this.flags);
            this.settingsgroup.Location = new System.Drawing.Point(10, 10);
            this.settingsgroup.Margin = new System.Windows.Forms.Padding(4);
            this.settingsgroup.Name = "settingsgroup";
            this.settingsgroup.Padding = new System.Windows.Forms.Padding(4);
            this.settingsgroup.Size = new System.Drawing.Size(666, 204);
            this.settingsgroup.TabIndex = 0;
            this.settingsgroup.TabStop = false;
            this.settingsgroup.Text = " Settings ";
            // 
            // flags
            // 
            this.flags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flags.AutoScroll = true;
            this.flags.Columns = 3;
            this.flags.Location = new System.Drawing.Point(22, 20);
            this.flags.Margin = new System.Windows.Forms.Padding(5);
            this.flags.Name = "flags";
            this.flags.Size = new System.Drawing.Size(636, 176);
            this.flags.TabIndex = 0;
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(0, 0);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(104, 24);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Controls.Add(this.tabproperties);
            this.tabs.Controls.Add(this.tabsidedefs);
            this.tabs.Controls.Add(this.tabcustom);
            this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Margin = new System.Windows.Forms.Padding(1);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(696, 720);
            this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabs.TabIndex = 0;
            // 
            // tabproperties
            // 
            this.tabproperties.Controls.Add(this.idgroup);
            this.tabproperties.Controls.Add(this.settingsgroup);
            this.tabproperties.Controls.Add(this.actiongroup);
            this.tabproperties.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabproperties.Location = new System.Drawing.Point(4, 25);
            this.tabproperties.Margin = new System.Windows.Forms.Padding(4);
            this.tabproperties.Name = "tabproperties";
            this.tabproperties.Padding = new System.Windows.Forms.Padding(6);
            this.tabproperties.Size = new System.Drawing.Size(688, 691);
            this.tabproperties.TabIndex = 0;
            this.tabproperties.Text = "Properties";
            this.tabproperties.UseVisualStyleBackColor = true;
            // 
            // idgroup
            // 
            this.idgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.idgroup.Controls.Add(this.tag);
            this.idgroup.Controls.Add(taglabel);
            this.idgroup.Controls.Add(this.newtag);
            this.idgroup.Location = new System.Drawing.Point(10, 620);
            this.idgroup.Margin = new System.Windows.Forms.Padding(4);
            this.idgroup.Name = "idgroup";
            this.idgroup.Padding = new System.Windows.Forms.Padding(4);
            this.idgroup.Size = new System.Drawing.Size(666, 56);
            this.idgroup.TabIndex = 2;
            this.idgroup.TabStop = false;
            this.idgroup.Text = " Identification ";
            // 
            // tag
            // 
            this.tag.AllowDecimal = false;
            this.tag.AllowNegative = false;
            this.tag.AllowRelative = true;
            this.tag.ButtonStep = 1;
            this.tag.Location = new System.Drawing.Point(78, 20);
            this.tag.Margin = new System.Windows.Forms.Padding(5);
            this.tag.Name = "tag";
            this.tag.Size = new System.Drawing.Size(94, 27);
            this.tag.StepValues = null;
            this.tag.TabIndex = 7;
            // 
            // tabsidedefs
            // 
            this.tabsidedefs.Controls.Add(this.splitter);
            this.tabsidedefs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabsidedefs.Location = new System.Drawing.Point(4, 25);
            this.tabsidedefs.Margin = new System.Windows.Forms.Padding(4);
            this.tabsidedefs.Name = "tabsidedefs";
            this.tabsidedefs.Padding = new System.Windows.Forms.Padding(6);
            this.tabsidedefs.Size = new System.Drawing.Size(688, 691);
            this.tabsidedefs.TabIndex = 1;
            this.tabsidedefs.Text = "Sidedefs";
            this.tabsidedefs.UseVisualStyleBackColor = true;
            // 
            // splitter
            // 
            this.splitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitter.IsSplitterFixed = true;
            this.splitter.Location = new System.Drawing.Point(6, 6);
            this.splitter.Margin = new System.Windows.Forms.Padding(4);
            this.splitter.Name = "splitter";
            this.splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitter.Panel1
            // 
            this.splitter.Panel1.Controls.Add(this.frontside);
            this.splitter.Panel1.Controls.Add(this.frontgroup);
            // 
            // splitter.Panel2
            // 
            this.splitter.Panel2.Controls.Add(this.backside);
            this.splitter.Panel2.Controls.Add(this.backgroup);
            this.splitter.Size = new System.Drawing.Size(676, 679);
            this.splitter.SplitterDistance = 328;
            this.splitter.SplitterWidth = 5;
            this.splitter.TabIndex = 3;
            // 
            // frontside
            // 
            this.frontside.AutoSize = true;
            this.frontside.Location = new System.Drawing.Point(19, 1);
            this.frontside.Margin = new System.Windows.Forms.Padding(4);
            this.frontside.Name = "frontside";
            this.frontside.Size = new System.Drawing.Size(96, 20);
            this.frontside.TabIndex = 0;
            this.frontside.Text = "Front Side";
            this.frontside.UseVisualStyleBackColor = true;
            this.frontside.CheckStateChanged += new System.EventHandler(this.frontside_CheckStateChanged);
            // 
            // frontgroup
            // 
            this.frontgroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.frontgroup.Controls.Add(this.frontoffsety);
            this.frontgroup.Controls.Add(this.frontoffsetx);
            this.frontgroup.Controls.Add(this.frontsector);
            this.frontgroup.Controls.Add(this.customfrontbutton);
            this.frontgroup.Controls.Add(label11);
            this.frontgroup.Controls.Add(this.frontlow);
            this.frontgroup.Controls.Add(this.frontmid);
            this.frontgroup.Controls.Add(this.fronthigh);
            this.frontgroup.Controls.Add(label6);
            this.frontgroup.Controls.Add(label5);
            this.frontgroup.Controls.Add(label4);
            this.frontgroup.Controls.Add(label3);
            this.frontgroup.Enabled = false;
            this.frontgroup.Location = new System.Drawing.Point(4, 4);
            this.frontgroup.Margin = new System.Windows.Forms.Padding(4);
            this.frontgroup.Name = "frontgroup";
            this.frontgroup.Padding = new System.Windows.Forms.Padding(4);
            this.frontgroup.Size = new System.Drawing.Size(668, 320);
            this.frontgroup.TabIndex = 1;
            this.frontgroup.TabStop = false;
            this.frontgroup.Text = "     ";
            // 
            // frontoffsety
            // 
            this.frontoffsety.AllowDecimal = false;
            this.frontoffsety.AllowNegative = true;
            this.frontoffsety.AllowRelative = true;
            this.frontoffsety.ButtonStep = 1;
            this.frontoffsety.Location = new System.Drawing.Point(214, 92);
            this.frontoffsety.Margin = new System.Windows.Forms.Padding(5);
            this.frontoffsety.Name = "frontoffsety";
            this.frontoffsety.Size = new System.Drawing.Size(78, 27);
            this.frontoffsety.StepValues = null;
            this.frontoffsety.TabIndex = 16;
            // 
            // frontoffsetx
            // 
            this.frontoffsetx.AllowDecimal = false;
            this.frontoffsetx.AllowNegative = true;
            this.frontoffsetx.AllowRelative = true;
            this.frontoffsetx.ButtonStep = 1;
            this.frontoffsetx.Location = new System.Drawing.Point(129, 92);
            this.frontoffsetx.Margin = new System.Windows.Forms.Padding(5);
            this.frontoffsetx.Name = "frontoffsetx";
            this.frontoffsetx.Size = new System.Drawing.Size(78, 27);
            this.frontoffsetx.StepValues = null;
            this.frontoffsetx.TabIndex = 15;
            // 
            // frontsector
            // 
            this.frontsector.AllowDecimal = false;
            this.frontsector.AllowNegative = false;
            this.frontsector.AllowRelative = false;
            this.frontsector.ButtonStep = 1;
            this.frontsector.Location = new System.Drawing.Point(129, 44);
            this.frontsector.Margin = new System.Windows.Forms.Padding(5);
            this.frontsector.Name = "frontsector";
            this.frontsector.Size = new System.Drawing.Size(162, 27);
            this.frontsector.StepValues = null;
            this.frontsector.TabIndex = 14;
            // 
            // customfrontbutton
            // 
            this.customfrontbutton.Location = new System.Drawing.Point(129, 155);
            this.customfrontbutton.Margin = new System.Windows.Forms.Padding(4);
            this.customfrontbutton.Name = "customfrontbutton";
            this.customfrontbutton.Size = new System.Drawing.Size(144, 31);
            this.customfrontbutton.TabIndex = 3;
            this.customfrontbutton.Text = "Custom fields...";
            this.customfrontbutton.UseVisualStyleBackColor = true;
            this.customfrontbutton.Visible = false;
            this.customfrontbutton.Click += new System.EventHandler(this.customfrontbutton_Click);
            // 
            // frontlow
            // 
            this.frontlow.Location = new System.Drawing.Point(542, 46);
            this.frontlow.Margin = new System.Windows.Forms.Padding(5);
            this.frontlow.Name = "frontlow";
            this.frontlow.Required = false;
            this.frontlow.Size = new System.Drawing.Size(104, 140);
            this.frontlow.TabIndex = 6;
            this.frontlow.TextureName = "";
            // 
            // frontmid
            // 
            this.frontmid.Location = new System.Drawing.Point(429, 46);
            this.frontmid.Margin = new System.Windows.Forms.Padding(5);
            this.frontmid.Name = "frontmid";
            this.frontmid.Required = false;
            this.frontmid.Size = new System.Drawing.Size(104, 140);
            this.frontmid.TabIndex = 5;
            this.frontmid.TextureName = "";
            // 
            // fronthigh
            // 
            this.fronthigh.Location = new System.Drawing.Point(315, 46);
            this.fronthigh.Margin = new System.Windows.Forms.Padding(5);
            this.fronthigh.Name = "fronthigh";
            this.fronthigh.Required = false;
            this.fronthigh.Size = new System.Drawing.Size(104, 140);
            this.fronthigh.TabIndex = 4;
            this.fronthigh.TextureName = "";
            // 
            // backside
            // 
            this.backside.AutoSize = true;
            this.backside.Location = new System.Drawing.Point(19, 1);
            this.backside.Margin = new System.Windows.Forms.Padding(4);
            this.backside.Name = "backside";
            this.backside.Size = new System.Drawing.Size(93, 20);
            this.backside.TabIndex = 0;
            this.backside.Text = "Back Side";
            this.backside.UseVisualStyleBackColor = true;
            this.backside.CheckStateChanged += new System.EventHandler(this.backside_CheckStateChanged);
            // 
            // backgroup
            // 
            this.backgroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.backgroup.Controls.Add(this.backoffsety);
            this.backgroup.Controls.Add(this.backoffsetx);
            this.backgroup.Controls.Add(this.backsector);
            this.backgroup.Controls.Add(this.custombackbutton);
            this.backgroup.Controls.Add(label12);
            this.backgroup.Controls.Add(this.backlow);
            this.backgroup.Controls.Add(this.backmid);
            this.backgroup.Controls.Add(this.backhigh);
            this.backgroup.Controls.Add(label7);
            this.backgroup.Controls.Add(label8);
            this.backgroup.Controls.Add(label9);
            this.backgroup.Controls.Add(label10);
            this.backgroup.Enabled = false;
            this.backgroup.Location = new System.Drawing.Point(4, 4);
            this.backgroup.Margin = new System.Windows.Forms.Padding(4);
            this.backgroup.Name = "backgroup";
            this.backgroup.Padding = new System.Windows.Forms.Padding(4);
            this.backgroup.Size = new System.Drawing.Size(671, 339);
            this.backgroup.TabIndex = 1;
            this.backgroup.TabStop = false;
            this.backgroup.Text = "     ";
            // 
            // backoffsety
            // 
            this.backoffsety.AllowDecimal = false;
            this.backoffsety.AllowNegative = true;
            this.backoffsety.AllowRelative = true;
            this.backoffsety.ButtonStep = 1;
            this.backoffsety.Location = new System.Drawing.Point(214, 92);
            this.backoffsety.Margin = new System.Windows.Forms.Padding(5);
            this.backoffsety.Name = "backoffsety";
            this.backoffsety.Size = new System.Drawing.Size(78, 27);
            this.backoffsety.StepValues = null;
            this.backoffsety.TabIndex = 19;
            // 
            // backoffsetx
            // 
            this.backoffsetx.AllowDecimal = false;
            this.backoffsetx.AllowNegative = true;
            this.backoffsetx.AllowRelative = true;
            this.backoffsetx.ButtonStep = 1;
            this.backoffsetx.Location = new System.Drawing.Point(129, 92);
            this.backoffsetx.Margin = new System.Windows.Forms.Padding(5);
            this.backoffsetx.Name = "backoffsetx";
            this.backoffsetx.Size = new System.Drawing.Size(78, 27);
            this.backoffsetx.StepValues = null;
            this.backoffsetx.TabIndex = 18;
            // 
            // backsector
            // 
            this.backsector.AllowDecimal = false;
            this.backsector.AllowNegative = false;
            this.backsector.AllowRelative = false;
            this.backsector.ButtonStep = 1;
            this.backsector.Location = new System.Drawing.Point(129, 44);
            this.backsector.Margin = new System.Windows.Forms.Padding(5);
            this.backsector.Name = "backsector";
            this.backsector.Size = new System.Drawing.Size(162, 27);
            this.backsector.StepValues = null;
            this.backsector.TabIndex = 17;
            // 
            // custombackbutton
            // 
            this.custombackbutton.Location = new System.Drawing.Point(129, 155);
            this.custombackbutton.Margin = new System.Windows.Forms.Padding(4);
            this.custombackbutton.Name = "custombackbutton";
            this.custombackbutton.Size = new System.Drawing.Size(144, 31);
            this.custombackbutton.TabIndex = 3;
            this.custombackbutton.Text = "Custom fields...";
            this.custombackbutton.UseVisualStyleBackColor = true;
            this.custombackbutton.Visible = false;
            this.custombackbutton.Click += new System.EventHandler(this.custombackbutton_Click);
            // 
            // backlow
            // 
            this.backlow.Location = new System.Drawing.Point(546, 46);
            this.backlow.Margin = new System.Windows.Forms.Padding(5);
            this.backlow.Name = "backlow";
            this.backlow.Required = false;
            this.backlow.Size = new System.Drawing.Size(104, 140);
            this.backlow.TabIndex = 6;
            this.backlow.TextureName = "";
            // 
            // backmid
            // 
            this.backmid.Location = new System.Drawing.Point(432, 46);
            this.backmid.Margin = new System.Windows.Forms.Padding(5);
            this.backmid.Name = "backmid";
            this.backmid.Required = false;
            this.backmid.Size = new System.Drawing.Size(104, 140);
            this.backmid.TabIndex = 5;
            this.backmid.TextureName = "";
            // 
            // backhigh
            // 
            this.backhigh.Location = new System.Drawing.Point(319, 46);
            this.backhigh.Margin = new System.Windows.Forms.Padding(5);
            this.backhigh.Name = "backhigh";
            this.backhigh.Required = false;
            this.backhigh.Size = new System.Drawing.Size(104, 140);
            this.backhigh.TabIndex = 4;
            this.backhigh.TextureName = "";
            // 
            // tabcustom
            // 
            this.tabcustom.Controls.Add(this.fieldslist);
            this.tabcustom.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabcustom.Location = new System.Drawing.Point(4, 25);
            this.tabcustom.Margin = new System.Windows.Forms.Padding(4);
            this.tabcustom.Name = "tabcustom";
            this.tabcustom.Padding = new System.Windows.Forms.Padding(4);
            this.tabcustom.Size = new System.Drawing.Size(688, 691);
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
            this.fieldslist.Location = new System.Drawing.Point(14, 14);
            this.fieldslist.Margin = new System.Windows.Forms.Padding(10);
            this.fieldslist.Name = "fieldslist";
            this.fieldslist.PropertyColumnVisible = true;
            this.fieldslist.PropertyColumnWidth = 150;
            this.fieldslist.Size = new System.Drawing.Size(638, 644);
            this.fieldslist.TabIndex = 0;
            this.fieldslist.TypeColumnVisible = true;
            this.fieldslist.TypeColumnWidth = 100;
            this.fieldslist.ValueColumnVisible = true;
            // 
            // heightpanel1
            // 
            this.heightpanel1.BackColor = System.Drawing.Color.Navy;
            this.heightpanel1.Location = new System.Drawing.Point(0, -24);
            this.heightpanel1.Margin = new System.Windows.Forms.Padding(4);
            this.heightpanel1.Name = "heightpanel1";
            this.heightpanel1.Size = new System.Drawing.Size(98, 638);
            this.heightpanel1.TabIndex = 3;
            this.heightpanel1.Visible = false;
            // 
            // heightpanel2
            // 
            this.heightpanel2.BackColor = System.Drawing.Color.Navy;
            this.heightpanel2.Location = new System.Drawing.Point(591, -24);
            this.heightpanel2.Margin = new System.Windows.Forms.Padding(4);
            this.heightpanel2.Name = "heightpanel2";
            this.heightpanel2.Size = new System.Drawing.Size(110, 588);
            this.heightpanel2.TabIndex = 4;
            this.heightpanel2.Visible = false;
            // 
            // LinedefEditForm
            // 
            this.AcceptButton = this.apply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(721, 784);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.apply);
            this.Controls.Add(this.heightpanel1);
            this.Controls.Add(this.heightpanel2);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LinedefEditForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Linedef";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.LinedefEditForm_HelpRequested);
            this.actiongroup.ResumeLayout(false);
            this.actiongroup.PerformLayout();
            this.argspanel.ResumeLayout(false);
            this.hexenpanel.ResumeLayout(false);
            this.hexenpanel.PerformLayout();
            this.udmfpanel.ResumeLayout(false);
            this.settingsgroup.ResumeLayout(false);
            this.tabs.ResumeLayout(false);
            this.tabproperties.ResumeLayout(false);
            this.idgroup.ResumeLayout(false);
            this.idgroup.PerformLayout();
            this.tabsidedefs.ResumeLayout(false);
            this.splitter.Panel1.ResumeLayout(false);
            this.splitter.Panel1.PerformLayout();
            this.splitter.Panel2.ResumeLayout(false);
            this.splitter.Panel2.PerformLayout();
            this.splitter.ResumeLayout(false);
            this.frontgroup.ResumeLayout(false);
            this.frontgroup.PerformLayout();
            this.backgroup.ResumeLayout(false);
            this.backgroup.PerformLayout();
            this.tabcustom.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.GroupBox actiongroup;
		private System.Windows.Forms.GroupBox settingsgroup;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl flags;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Button browseaction;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl action;
		private System.Windows.Forms.Button newtag;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabproperties;
		private System.Windows.Forms.TabPage tabsidedefs;
		private System.Windows.Forms.GroupBox frontgroup;
		private System.Windows.Forms.CheckBox frontside;
		private System.Windows.Forms.CheckBox backside;
		private System.Windows.Forms.GroupBox backgroup;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl frontlow;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl frontmid;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl fronthigh;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl backlow;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl backmid;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl backhigh;
		private System.Windows.Forms.ComboBox activation;
		private System.Windows.Forms.Panel udmfpanel;
		private System.Windows.Forms.Panel hexenpanel;
		private System.Windows.Forms.TabPage tabcustom;
		private CodeImp.DoomBuilder.Controls.FieldsEditorControl fieldslist;
		private System.Windows.Forms.GroupBox idgroup;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl udmfactivates;
		private System.Windows.Forms.Panel argspanel;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg2;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg1;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg0;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg3;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg4;
		private System.Windows.Forms.Label arg1label;
		private System.Windows.Forms.Label arg0label;
		private System.Windows.Forms.Label arg3label;
		private System.Windows.Forms.Label arg2label;
		private System.Windows.Forms.Label arg4label;
		private System.Windows.Forms.SplitContainer splitter;
		private System.Windows.Forms.Button customfrontbutton;
		private System.Windows.Forms.Button custombackbutton;
		private System.Windows.Forms.Panel heightpanel1;
		private System.Windows.Forms.Panel heightpanel2;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox tag;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox frontoffsetx;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox frontsector;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox frontoffsety;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox backoffsety;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox backoffsetx;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox backsector;
	}
}