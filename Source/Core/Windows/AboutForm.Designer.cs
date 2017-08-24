namespace CodeImp.DoomBuilder.Windows
{
	partial class AboutForm
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
			System.Windows.Forms.PictureBox pictureBox1;
			this.close = new System.Windows.Forms.Button();
			this.builderlink = new System.Windows.Forms.LinkLabel();
			this.version = new System.Windows.Forms.Label();
			this.copyversion = new System.Windows.Forms.Button();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.codeimplink = new System.Windows.Forms.LinkLabel();
			label1 = new System.Windows.Forms.Label();
			pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			label1.Location = new System.Drawing.Point(15, 161);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(349, 50);
			label1.TabIndex = 2;
			label1.Text = "Doom Builder is designed and programmed by Pascal vd Heiden.\r\nSeveral game config" +
				"urations were written by various members of the Doom community. See the website " +
				"for a complete list of credits.";
			// 
			// pictureBox1
			// 
			pictureBox1.Image = global::CodeImp.DoomBuilder.Properties.Resources.Splash3_small;
			pictureBox1.Location = new System.Drawing.Point(10, 12);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new System.Drawing.Size(226, 80);
			pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			pictureBox1.TabIndex = 0;
			pictureBox1.TabStop = false;
			// 
			// close
			// 
			this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.close.Location = new System.Drawing.Point(265, 243);
			this.close.Name = "close";
			this.close.Size = new System.Drawing.Size(116, 25);
			this.close.TabIndex = 5;
			this.close.Text = "Close";
			this.close.UseVisualStyleBackColor = true;
			// 
			// builderlink
			// 
			this.builderlink.ActiveLinkColor = System.Drawing.Color.Firebrick;
			this.builderlink.AutoSize = true;
			this.builderlink.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
			this.builderlink.LinkColor = System.Drawing.Color.Firebrick;
			this.builderlink.Location = new System.Drawing.Point(12, 219);
			this.builderlink.Name = "builderlink";
			this.builderlink.Size = new System.Drawing.Size(121, 14);
			this.builderlink.TabIndex = 3;
			this.builderlink.TabStop = true;
			this.builderlink.Text = "www.doombuilder.com";
			this.builderlink.VisitedLinkColor = System.Drawing.Color.Firebrick;
			this.builderlink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.builderlink_LinkClicked);
			// 
			// version
			// 
			this.version.AutoSize = true;
			this.version.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.version.Location = new System.Drawing.Point(15, 118);
			this.version.Name = "version";
			this.version.Size = new System.Drawing.Size(138, 14);
			this.version.TabIndex = 0;
			this.version.Text = "Doom Builder some version";
			// 
			// copyversion
			// 
			this.copyversion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.copyversion.Location = new System.Drawing.Point(265, 113);
			this.copyversion.Name = "copyversion";
			this.copyversion.Size = new System.Drawing.Size(116, 25);
			this.copyversion.TabIndex = 1;
			this.copyversion.Text = "Copy Version";
			this.copyversion.UseVisualStyleBackColor = true;
			this.copyversion.Click += new System.EventHandler(this.copyversion_Click);
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = global::CodeImp.DoomBuilder.Properties.Resources.CLogo;
			this.pictureBox2.Location = new System.Drawing.Point(293, 12);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(88, 80);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox2.TabIndex = 10;
			this.pictureBox2.TabStop = false;
			// 
			// codeimplink
			// 
			this.codeimplink.ActiveLinkColor = System.Drawing.Color.Firebrick;
			this.codeimplink.AutoSize = true;
			this.codeimplink.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
			this.codeimplink.LinkColor = System.Drawing.Color.Firebrick;
			this.codeimplink.Location = new System.Drawing.Point(12, 239);
			this.codeimplink.Name = "codeimplink";
			this.codeimplink.Size = new System.Drawing.Size(103, 14);
			this.codeimplink.TabIndex = 4;
			this.codeimplink.TabStop = true;
			this.codeimplink.Text = "www.codeimp.com";
			this.codeimplink.VisitedLinkColor = System.Drawing.Color.Firebrick;
			this.codeimplink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.codeimplink_LinkClicked);
			// 
			// AboutForm
			// 
			this.AcceptButton = this.close;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.close;
			this.ClientSize = new System.Drawing.Size(393, 280);
			this.Controls.Add(this.codeimplink);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.copyversion);
			this.Controls.Add(this.version);
			this.Controls.Add(this.builderlink);
			this.Controls.Add(this.close);
			this.Controls.Add(label1);
			this.Controls.Add(pictureBox1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About Doom Builder";
			((System.ComponentModel.ISupportInitialize)(pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button close;
		private System.Windows.Forms.LinkLabel builderlink;
		private System.Windows.Forms.Label version;
		private System.Windows.Forms.Button copyversion;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.LinkLabel codeimplink;
	}
}