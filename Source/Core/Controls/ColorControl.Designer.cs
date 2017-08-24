namespace CodeImp.DoomBuilder.Controls
{
	partial class ColorControl
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
			this.label = new System.Windows.Forms.Label();
			this.panel = new System.Windows.Forms.Panel();
			this.button = new System.Windows.Forms.Button();
			this.dialog = new System.Windows.Forms.ColorDialog();
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.Location = new System.Drawing.Point(-3, 0);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(175, 23);
			this.label.TabIndex = 0;
			this.label.Text = "Color name:";
			this.label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panel
			// 
			this.panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel.Location = new System.Drawing.Point(178, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(27, 23);
			this.panel.TabIndex = 1;
			// 
			// button
			// 
			this.button.BackColor = System.Drawing.SystemColors.Control;
			this.button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button.Image = global::CodeImp.DoomBuilder.Properties.Resources.ColorPick;
			this.button.Location = new System.Drawing.Point(322, 0);
			this.button.Name = "button";
			this.button.Padding = new System.Windows.Forms.Padding(0, 0, 2, 3);
			this.button.Size = new System.Drawing.Size(26, 23);
			this.button.TabIndex = 2;
			this.button.UseVisualStyleBackColor = false;
			this.button.MouseMove += new System.Windows.Forms.MouseEventHandler(this.button_MouseMove);
			this.button.Click += new System.EventHandler(this.button_Click);
			this.button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button_MouseDown);
			this.button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button_MouseUp);
			// 
			// dialog
			// 
			this.dialog.FullOpen = true;
			// 
			// ColorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.button);
			this.Controls.Add(this.panel);
			this.Controls.Add(this.label);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximumSize = new System.Drawing.Size(10000, 23);
			this.MinimumSize = new System.Drawing.Size(100, 23);
			this.Name = "ColorControl";
			this.Size = new System.Drawing.Size(348, 23);
			this.Resize += new System.EventHandler(this.ColorControl_Resize);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.Button button;
		private System.Windows.Forms.ColorDialog dialog;
	}
}
