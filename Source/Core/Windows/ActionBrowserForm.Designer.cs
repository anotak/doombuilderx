namespace CodeImp.DoomBuilder.Windows
{
	partial class ActionBrowserForm
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.GroupBox groupBox1;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.GroupBox groupBox2;
            this.category = new System.Windows.Forms.ComboBox();
            this.option7 = new System.Windows.Forms.ComboBox();
            this.option7label = new System.Windows.Forms.Label();
            this.option6 = new System.Windows.Forms.ComboBox();
            this.option6label = new System.Windows.Forms.Label();
            this.option5 = new System.Windows.Forms.ComboBox();
            this.option5label = new System.Windows.Forms.Label();
            this.option4 = new System.Windows.Forms.ComboBox();
            this.option4label = new System.Windows.Forms.Label();
            this.option3 = new System.Windows.Forms.ComboBox();
            this.option3label = new System.Windows.Forms.Label();
            this.option2 = new System.Windows.Forms.ComboBox();
            this.option2label = new System.Windows.Forms.Label();
            this.option1 = new System.Windows.Forms.ComboBox();
            this.option1label = new System.Windows.Forms.Label();
            this.option0 = new System.Windows.Forms.ComboBox();
            this.option0label = new System.Windows.Forms.Label();
            this.cancel = new System.Windows.Forms.Button();
            this.apply = new System.Windows.Forms.Button();
            this.actions = new System.Windows.Forms.TreeView();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabactions = new System.Windows.Forms.TabPage();
            this.prefixespanel = new System.Windows.Forms.Panel();
            this.tabgeneralized = new System.Windows.Forms.TabPage();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label7 = new System.Windows.Forms.Label();
            groupBox2 = new System.Windows.Forms.GroupBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabactions.SuspendLayout();
            this.prefixespanel.SuspendLayout();
            this.tabgeneralized.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(34, 0);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(73, 16);
            label1.TabIndex = 21;
            label1.Text = "S = Switch";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(175, 0);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(99, 16);
            label2.TabIndex = 22;
            label2.Text = "W = Walk over";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(336, 0);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(82, 16);
            label3.TabIndex = 23;
            label3.Text = "G = Gunfire";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(34, 20);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(65, 16);
            label4.TabIndex = 24;
            label4.Text = "D = Door";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(175, 20);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(107, 16);
            label5.TabIndex = 25;
            label5.Text = "R = Repeatable";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(336, 20);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(96, 16);
            label6.TabIndex = 26;
            label6.Text = "1 = Once only";
            // 
            // groupBox1
            // 
            groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            groupBox1.Controls.Add(this.category);
            groupBox1.Controls.Add(label7);
            groupBox1.Location = new System.Drawing.Point(8, 8);
            groupBox1.Margin = new System.Windows.Forms.Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4);
            groupBox1.Size = new System.Drawing.Size(474, 81);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = " Category ";
            // 
            // category
            // 
            this.category.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.category.FormattingEnabled = true;
            this.category.Location = new System.Drawing.Point(148, 31);
            this.category.Margin = new System.Windows.Forms.Padding(4);
            this.category.Name = "category";
            this.category.Size = new System.Drawing.Size(248, 24);
            this.category.TabIndex = 0;
            this.category.SelectedIndexChanged += new System.EventHandler(this.category_SelectedIndexChanged);
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(72, 35);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(70, 16);
            label7.TabIndex = 0;
            label7.Text = "Category:";
            // 
            // groupBox2
            // 
            groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            groupBox2.Controls.Add(this.option7);
            groupBox2.Controls.Add(this.option7label);
            groupBox2.Controls.Add(this.option6);
            groupBox2.Controls.Add(this.option6label);
            groupBox2.Controls.Add(this.option5);
            groupBox2.Controls.Add(this.option5label);
            groupBox2.Controls.Add(this.option4);
            groupBox2.Controls.Add(this.option4label);
            groupBox2.Controls.Add(this.option3);
            groupBox2.Controls.Add(this.option3label);
            groupBox2.Controls.Add(this.option2);
            groupBox2.Controls.Add(this.option2label);
            groupBox2.Controls.Add(this.option1);
            groupBox2.Controls.Add(this.option1label);
            groupBox2.Controls.Add(this.option0);
            groupBox2.Controls.Add(this.option0label);
            groupBox2.Location = new System.Drawing.Point(8, 96);
            groupBox2.Margin = new System.Windows.Forms.Padding(4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(4);
            groupBox2.Size = new System.Drawing.Size(474, 408);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = " Options ";
            // 
            // option7
            // 
            this.option7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.option7.FormattingEnabled = true;
            this.option7.Location = new System.Drawing.Point(148, 350);
            this.option7.Margin = new System.Windows.Forms.Padding(4);
            this.option7.Name = "option7";
            this.option7.Size = new System.Drawing.Size(248, 24);
            this.option7.TabIndex = 7;
            this.option7.Visible = false;
            // 
            // option7label
            // 
            this.option7label.Location = new System.Drawing.Point(4, 354);
            this.option7label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.option7label.Name = "option7label";
            this.option7label.Size = new System.Drawing.Size(136, 24);
            this.option7label.TabIndex = 16;
            this.option7label.Text = "Option:";
            this.option7label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.option7label.Visible = false;
            // 
            // option6
            // 
            this.option6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.option6.FormattingEnabled = true;
            this.option6.Location = new System.Drawing.Point(148, 305);
            this.option6.Margin = new System.Windows.Forms.Padding(4);
            this.option6.Name = "option6";
            this.option6.Size = new System.Drawing.Size(248, 24);
            this.option6.TabIndex = 6;
            this.option6.Visible = false;
            // 
            // option6label
            // 
            this.option6label.Location = new System.Drawing.Point(4, 309);
            this.option6label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.option6label.Name = "option6label";
            this.option6label.Size = new System.Drawing.Size(136, 24);
            this.option6label.TabIndex = 14;
            this.option6label.Text = "Option:";
            this.option6label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.option6label.Visible = false;
            // 
            // option5
            // 
            this.option5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.option5.FormattingEnabled = true;
            this.option5.Location = new System.Drawing.Point(148, 260);
            this.option5.Margin = new System.Windows.Forms.Padding(4);
            this.option5.Name = "option5";
            this.option5.Size = new System.Drawing.Size(248, 24);
            this.option5.TabIndex = 5;
            this.option5.Visible = false;
            // 
            // option5label
            // 
            this.option5label.Location = new System.Drawing.Point(4, 264);
            this.option5label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.option5label.Name = "option5label";
            this.option5label.Size = new System.Drawing.Size(136, 24);
            this.option5label.TabIndex = 12;
            this.option5label.Text = "Option:";
            this.option5label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.option5label.Visible = false;
            // 
            // option4
            // 
            this.option4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.option4.FormattingEnabled = true;
            this.option4.Location = new System.Drawing.Point(148, 215);
            this.option4.Margin = new System.Windows.Forms.Padding(4);
            this.option4.Name = "option4";
            this.option4.Size = new System.Drawing.Size(248, 24);
            this.option4.TabIndex = 4;
            this.option4.Visible = false;
            // 
            // option4label
            // 
            this.option4label.Location = new System.Drawing.Point(4, 219);
            this.option4label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.option4label.Name = "option4label";
            this.option4label.Size = new System.Drawing.Size(136, 24);
            this.option4label.TabIndex = 10;
            this.option4label.Text = "Option:";
            this.option4label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.option4label.Visible = false;
            // 
            // option3
            // 
            this.option3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.option3.FormattingEnabled = true;
            this.option3.Location = new System.Drawing.Point(148, 170);
            this.option3.Margin = new System.Windows.Forms.Padding(4);
            this.option3.Name = "option3";
            this.option3.Size = new System.Drawing.Size(248, 24);
            this.option3.TabIndex = 3;
            this.option3.Visible = false;
            // 
            // option3label
            // 
            this.option3label.Location = new System.Drawing.Point(4, 174);
            this.option3label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.option3label.Name = "option3label";
            this.option3label.Size = new System.Drawing.Size(136, 24);
            this.option3label.TabIndex = 8;
            this.option3label.Text = "Option:";
            this.option3label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.option3label.Visible = false;
            // 
            // option2
            // 
            this.option2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.option2.FormattingEnabled = true;
            this.option2.Location = new System.Drawing.Point(148, 125);
            this.option2.Margin = new System.Windows.Forms.Padding(4);
            this.option2.Name = "option2";
            this.option2.Size = new System.Drawing.Size(248, 24);
            this.option2.TabIndex = 2;
            this.option2.Visible = false;
            // 
            // option2label
            // 
            this.option2label.Location = new System.Drawing.Point(4, 129);
            this.option2label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.option2label.Name = "option2label";
            this.option2label.Size = new System.Drawing.Size(136, 24);
            this.option2label.TabIndex = 6;
            this.option2label.Text = "Option:";
            this.option2label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.option2label.Visible = false;
            // 
            // option1
            // 
            this.option1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.option1.FormattingEnabled = true;
            this.option1.Location = new System.Drawing.Point(148, 80);
            this.option1.Margin = new System.Windows.Forms.Padding(4);
            this.option1.Name = "option1";
            this.option1.Size = new System.Drawing.Size(248, 24);
            this.option1.TabIndex = 1;
            this.option1.Visible = false;
            // 
            // option1label
            // 
            this.option1label.Location = new System.Drawing.Point(4, 84);
            this.option1label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.option1label.Name = "option1label";
            this.option1label.Size = new System.Drawing.Size(136, 24);
            this.option1label.TabIndex = 4;
            this.option1label.Text = "Option:";
            this.option1label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.option1label.Visible = false;
            // 
            // option0
            // 
            this.option0.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.option0.FormattingEnabled = true;
            this.option0.Location = new System.Drawing.Point(148, 35);
            this.option0.Margin = new System.Windows.Forms.Padding(4);
            this.option0.Name = "option0";
            this.option0.Size = new System.Drawing.Size(248, 24);
            this.option0.TabIndex = 0;
            this.option0.Visible = false;
            // 
            // option0label
            // 
            this.option0label.Location = new System.Drawing.Point(4, 39);
            this.option0label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.option0label.Name = "option0label";
            this.option0label.Size = new System.Drawing.Size(136, 24);
            this.option0label.TabIndex = 2;
            this.option0label.Text = "Option:";
            this.option0label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.option0label.Visible = false;
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(371, 717);
            this.cancel.Margin = new System.Windows.Forms.Padding(4);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(140, 34);
            this.cancel.TabIndex = 2;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // apply
            // 
            this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.apply.Location = new System.Drawing.Point(221, 717);
            this.apply.Margin = new System.Windows.Forms.Padding(4);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(140, 34);
            this.apply.TabIndex = 1;
            this.apply.Text = "OK";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // actions
            // 
            this.actions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.actions.HideSelection = false;
            this.actions.Location = new System.Drawing.Point(8, 42);
            this.actions.Margin = new System.Windows.Forms.Padding(4);
            this.actions.Name = "actions";
            this.actions.Size = new System.Drawing.Size(473, 635);
            this.actions.TabIndex = 0;
            this.actions.DoubleClick += new System.EventHandler(this.actions_DoubleClick);
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Controls.Add(this.tabactions);
            this.tabs.Controls.Add(this.tabgeneralized);
            this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabs.ItemSize = new System.Drawing.Size(150, 19);
            this.tabs.Location = new System.Drawing.Point(4, 4);
            this.tabs.Margin = new System.Windows.Forms.Padding(1);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(499, 709);
            this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabs.TabIndex = 0;
            // 
            // tabactions
            // 
            this.tabactions.Controls.Add(this.actions);
            this.tabactions.Controls.Add(this.prefixespanel);
            this.tabactions.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabactions.Location = new System.Drawing.Point(4, 23);
            this.tabactions.Margin = new System.Windows.Forms.Padding(4);
            this.tabactions.Name = "tabactions";
            this.tabactions.Padding = new System.Windows.Forms.Padding(4);
            this.tabactions.Size = new System.Drawing.Size(491, 682);
            this.tabactions.TabIndex = 0;
            this.tabactions.Text = "Predefined Actions";
            this.tabactions.UseVisualStyleBackColor = true;
            // 
            // prefixespanel
            // 
            this.prefixespanel.Controls.Add(label6);
            this.prefixespanel.Controls.Add(label5);
            this.prefixespanel.Controls.Add(label4);
            this.prefixespanel.Controls.Add(label3);
            this.prefixespanel.Controls.Add(label2);
            this.prefixespanel.Controls.Add(label1);
            this.prefixespanel.Location = new System.Drawing.Point(9, 0);
            this.prefixespanel.Margin = new System.Windows.Forms.Padding(4);
            this.prefixespanel.Name = "prefixespanel";
            this.prefixespanel.Size = new System.Drawing.Size(472, 38);
            this.prefixespanel.TabIndex = 27;
            // 
            // tabgeneralized
            // 
            this.tabgeneralized.Controls.Add(groupBox2);
            this.tabgeneralized.Controls.Add(groupBox1);
            this.tabgeneralized.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabgeneralized.Location = new System.Drawing.Point(4, 23);
            this.tabgeneralized.Margin = new System.Windows.Forms.Padding(4);
            this.tabgeneralized.Name = "tabgeneralized";
            this.tabgeneralized.Padding = new System.Windows.Forms.Padding(4);
            this.tabgeneralized.Size = new System.Drawing.Size(491, 596);
            this.tabgeneralized.TabIndex = 1;
            this.tabgeneralized.Text = "Generalized Actions";
            this.tabgeneralized.UseVisualStyleBackColor = true;
            // 
            // ActionBrowserForm
            // 
            this.AcceptButton = this.apply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(524, 753);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.apply);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ActionBrowserForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Action";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            this.tabs.ResumeLayout(false);
            this.tabactions.ResumeLayout(false);
            this.prefixespanel.ResumeLayout(false);
            this.prefixespanel.PerformLayout();
            this.tabgeneralized.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.TreeView actions;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabactions;
		private System.Windows.Forms.TabPage tabgeneralized;
		private System.Windows.Forms.ComboBox category;
		private System.Windows.Forms.ComboBox option7;
		private System.Windows.Forms.Label option7label;
		private System.Windows.Forms.ComboBox option6;
		private System.Windows.Forms.Label option6label;
		private System.Windows.Forms.ComboBox option5;
		private System.Windows.Forms.Label option5label;
		private System.Windows.Forms.ComboBox option4;
		private System.Windows.Forms.Label option4label;
		private System.Windows.Forms.ComboBox option3;
		private System.Windows.Forms.Label option3label;
		private System.Windows.Forms.ComboBox option2;
		private System.Windows.Forms.Label option2label;
		private System.Windows.Forms.ComboBox option1;
		private System.Windows.Forms.Label option1label;
		private System.Windows.Forms.ComboBox option0;
		private System.Windows.Forms.Label option0label;
		private System.Windows.Forms.Panel prefixespanel;
	}
}