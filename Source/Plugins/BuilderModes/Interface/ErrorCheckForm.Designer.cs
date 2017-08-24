namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class ErrorCheckForm
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
			this.checks = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.buttoncheck = new System.Windows.Forms.Button();
			this.results = new System.Windows.Forms.ListBox();
			this.resultspanel = new System.Windows.Forms.Panel();
			this.fix3 = new System.Windows.Forms.Button();
			this.fix2 = new System.Windows.Forms.Button();
			this.resultinfo = new System.Windows.Forms.Label();
			this.fix1 = new System.Windows.Forms.Button();
			this.progress = new System.Windows.Forms.ProgressBar();
			this.closebutton = new System.Windows.Forms.Button();
			this.resultspanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// checks
			// 
			this.checks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.checks.AutoScroll = true;
			this.checks.AutoSize = true;
			this.checks.Columns = 2;
			this.checks.Location = new System.Drawing.Point(10, 15);
			this.checks.Margin = new System.Windows.Forms.Padding(1);
			this.checks.Name = "checks";
			this.checks.Size = new System.Drawing.Size(360, 63);
			this.checks.TabIndex = 0;
			// 
			// buttoncheck
			// 
			this.buttoncheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttoncheck.Location = new System.Drawing.Point(254, 89);
			this.buttoncheck.Margin = new System.Windows.Forms.Padding(1);
			this.buttoncheck.Name = "buttoncheck";
			this.buttoncheck.Size = new System.Drawing.Size(116, 25);
			this.buttoncheck.TabIndex = 1;
			this.buttoncheck.Text = "Start Analysis";
			this.buttoncheck.UseVisualStyleBackColor = true;
			this.buttoncheck.Click += new System.EventHandler(this.buttoncheck_Click);
			// 
			// results
			// 
			this.results.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.results.FormattingEnabled = true;
			this.results.IntegralHeight = false;
			this.results.ItemHeight = 14;
			this.results.Location = new System.Drawing.Point(10, 34);
			this.results.Margin = new System.Windows.Forms.Padding(1);
			this.results.Name = "results";
			this.results.Size = new System.Drawing.Size(360, 175);
			this.results.TabIndex = 0;
			this.results.SelectedIndexChanged += new System.EventHandler(this.results_SelectedIndexChanged);
			// 
			// resultspanel
			// 
			this.resultspanel.Controls.Add(this.fix3);
			this.resultspanel.Controls.Add(this.fix2);
			this.resultspanel.Controls.Add(this.resultinfo);
			this.resultspanel.Controls.Add(this.fix1);
			this.resultspanel.Controls.Add(this.progress);
			this.resultspanel.Controls.Add(this.results);
			this.resultspanel.Location = new System.Drawing.Point(0, 124);
			this.resultspanel.Name = "resultspanel";
			this.resultspanel.Size = new System.Drawing.Size(383, 329);
			this.resultspanel.TabIndex = 2;
			// 
			// fix3
			// 
			this.fix3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.fix3.Location = new System.Drawing.Point(256, 292);
			this.fix3.Name = "fix3";
			this.fix3.Size = new System.Drawing.Size(114, 26);
			this.fix3.TabIndex = 3;
			this.fix3.Text = "Fix 3";
			this.fix3.UseVisualStyleBackColor = true;
			this.fix3.Visible = false;
			this.fix3.Click += new System.EventHandler(this.fix3_Click);
			// 
			// fix2
			// 
			this.fix2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.fix2.Location = new System.Drawing.Point(133, 292);
			this.fix2.Name = "fix2";
			this.fix2.Size = new System.Drawing.Size(114, 26);
			this.fix2.TabIndex = 2;
			this.fix2.Text = "Fix 2";
			this.fix2.UseVisualStyleBackColor = true;
			this.fix2.Visible = false;
			this.fix2.Click += new System.EventHandler(this.fix2_Click);
			// 
			// resultinfo
			// 
			this.resultinfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.resultinfo.Enabled = false;
			this.resultinfo.Location = new System.Drawing.Point(12, 215);
			this.resultinfo.Name = "resultinfo";
			this.resultinfo.Size = new System.Drawing.Size(358, 74);
			this.resultinfo.TabIndex = 5;
			this.resultinfo.Text = "Select a result from the list to see more information.";
			// 
			// fix1
			// 
			this.fix1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.fix1.Location = new System.Drawing.Point(10, 292);
			this.fix1.Name = "fix1";
			this.fix1.Size = new System.Drawing.Size(114, 26);
			this.fix1.TabIndex = 1;
			this.fix1.Text = "Fix 1";
			this.fix1.UseVisualStyleBackColor = true;
			this.fix1.Visible = false;
			this.fix1.Click += new System.EventHandler(this.fix1_Click);
			// 
			// progress
			// 
			this.progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.progress.Location = new System.Drawing.Point(10, 3);
			this.progress.Margin = new System.Windows.Forms.Padding(1);
			this.progress.Name = "progress";
			this.progress.Size = new System.Drawing.Size(360, 18);
			this.progress.TabIndex = 3;
			this.progress.Value = 30;
			// 
			// closebutton
			// 
			this.closebutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closebutton.Location = new System.Drawing.Point(-500, 134);
			this.closebutton.Name = "closebutton";
			this.closebutton.Size = new System.Drawing.Size(116, 25);
			this.closebutton.TabIndex = 4;
			this.closebutton.Text = "Close";
			this.closebutton.UseVisualStyleBackColor = true;
			this.closebutton.Click += new System.EventHandler(this.closebutton_Click);
			// 
			// ErrorCheckForm
			// 
			this.AcceptButton = this.buttoncheck;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.closebutton;
			this.ClientSize = new System.Drawing.Size(380, 453);
			this.Controls.Add(this.closebutton);
			this.Controls.Add(this.resultspanel);
			this.Controls.Add(this.buttoncheck);
			this.Controls.Add(this.checks);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ErrorCheckForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Map Analysis";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ErrorCheckForm_FormClosing);
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ErrorCheckForm_HelpRequested);
			this.resultspanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl checks;
		private System.Windows.Forms.Button buttoncheck;
		private System.Windows.Forms.ListBox results;
		private System.Windows.Forms.Panel resultspanel;
		private System.Windows.Forms.ProgressBar progress;
		private System.Windows.Forms.Button fix3;
		private System.Windows.Forms.Button fix2;
		private System.Windows.Forms.Label resultinfo;
		private System.Windows.Forms.Button fix1;
		private System.Windows.Forms.Button closebutton;
	}
}