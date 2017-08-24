namespace CodeImp.DoomBuilder.Controls
{
	partial class SectorInfoPanel
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
            System.Windows.Forms.Label label13;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            this.sectorinfo = new System.Windows.Forms.GroupBox();
            this.brightness = new System.Windows.Forms.Label();
            this.height = new System.Windows.Forms.Label();
            this.tag = new System.Windows.Forms.Label();
            this.floor = new System.Windows.Forms.Label();
            this.ceiling = new System.Windows.Forms.Label();
            this.effect = new System.Windows.Forms.Label();
            this.ceilingname = new System.Windows.Forms.Label();
            this.ceilingtex = new System.Windows.Forms.Panel();
            this.floortex = new System.Windows.Forms.Panel();
            this.floorname = new System.Windows.Forms.Label();
            this.floorLabel = new System.Windows.Forms.Label();
            this.ceilLabel = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            this.sectorinfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(129, 56);
            label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(79, 16);
            label13.TabIndex = 14;
            label13.Text = "Brightness:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(156, 32);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(52, 16);
            label5.TabIndex = 8;
            label5.Text = "Height:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(22, 64);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(35, 16);
            label4.TabIndex = 4;
            label4.Text = "Tag:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(16, 50);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(45, 16);
            label3.TabIndex = 3;
            label3.Text = "Floor:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(8, 32);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(55, 16);
            label2.TabIndex = 2;
            label2.Text = "Ceiling:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 14);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(48, 16);
            label1.TabIndex = 0;
            label1.Text = "Effect:";
            // 
            // sectorinfo
            // 
            this.sectorinfo.Controls.Add(this.brightness);
            this.sectorinfo.Controls.Add(label13);
            this.sectorinfo.Controls.Add(this.height);
            this.sectorinfo.Controls.Add(label5);
            this.sectorinfo.Controls.Add(this.tag);
            this.sectorinfo.Controls.Add(this.floor);
            this.sectorinfo.Controls.Add(this.ceiling);
            this.sectorinfo.Controls.Add(label4);
            this.sectorinfo.Controls.Add(label3);
            this.sectorinfo.Controls.Add(label2);
            this.sectorinfo.Controls.Add(this.effect);
            this.sectorinfo.Controls.Add(label1);
            this.sectorinfo.Location = new System.Drawing.Point(0, 0);
            this.sectorinfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.sectorinfo.Name = "sectorinfo";
            this.sectorinfo.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.sectorinfo.Size = new System.Drawing.Size(278, 105);
            this.sectorinfo.TabIndex = 2;
            this.sectorinfo.TabStop = false;
            this.sectorinfo.Text = " Sector ";
            // 
            // brightness
            // 
            this.brightness.AutoSize = true;
            this.brightness.Location = new System.Drawing.Point(211, 56);
            this.brightness.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.brightness.Name = "brightness";
            this.brightness.Size = new System.Drawing.Size(16, 16);
            this.brightness.TabIndex = 17;
            this.brightness.Text = "0";
            // 
            // height
            // 
            this.height.AutoSize = true;
            this.height.Location = new System.Drawing.Point(211, 32);
            this.height.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.height.Name = "height";
            this.height.Size = new System.Drawing.Size(16, 16);
            this.height.TabIndex = 11;
            this.height.Text = "0";
            // 
            // tag
            // 
            this.tag.AutoSize = true;
            this.tag.Location = new System.Drawing.Point(59, 64);
            this.tag.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tag.Name = "tag";
            this.tag.Size = new System.Drawing.Size(16, 16);
            this.tag.TabIndex = 7;
            this.tag.Text = "0";
            // 
            // floor
            // 
            this.floor.AutoSize = true;
            this.floor.Location = new System.Drawing.Point(59, 50);
            this.floor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.floor.Name = "floor";
            this.floor.Size = new System.Drawing.Size(32, 16);
            this.floor.TabIndex = 6;
            this.floor.Text = "360";
            // 
            // ceiling
            // 
            this.ceiling.AutoSize = true;
            this.ceiling.Location = new System.Drawing.Point(59, 32);
            this.ceiling.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ceiling.Name = "ceiling";
            this.ceiling.Size = new System.Drawing.Size(40, 16);
            this.ceiling.TabIndex = 5;
            this.ceiling.Text = "1024";
            // 
            // effect
            // 
            this.effect.AutoSize = true;
            this.effect.Location = new System.Drawing.Point(59, 14);
            this.effect.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.effect.Name = "effect";
            this.effect.Size = new System.Drawing.Size(162, 16);
            this.effect.TabIndex = 1;
            this.effect.Text = "0 - Whacky Pool of Fluid";
            // 
            // ceilingname
            // 
            this.ceilingname.Location = new System.Drawing.Point(405, 80);
            this.ceilingname.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ceilingname.Name = "ceilingname";
            this.ceilingname.Size = new System.Drawing.Size(105, 20);
            this.ceilingname.TabIndex = 1;
            this.ceilingname.Text = "BROWNHUG";
            this.ceilingname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ceilingtex
            // 
            this.ceilingtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ceilingtex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ceilingtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ceilingtex.Location = new System.Drawing.Point(414, 2);
            this.ceilingtex.Margin = new System.Windows.Forms.Padding(0);
            this.ceilingtex.Name = "ceilingtex";
            this.ceilingtex.Size = new System.Drawing.Size(84, 74);
            this.ceilingtex.TabIndex = 0;
            // 
            // floortex
            // 
            this.floortex.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.floortex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.floortex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.floortex.Location = new System.Drawing.Point(300, 2);
            this.floortex.Margin = new System.Windows.Forms.Padding(0);
            this.floortex.Name = "floortex";
            this.floortex.Size = new System.Drawing.Size(84, 74);
            this.floortex.TabIndex = 0;
            // 
            // floorname
            // 
            this.floorname.Location = new System.Drawing.Point(286, 80);
            this.floorname.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.floorname.Name = "floorname";
            this.floorname.Size = new System.Drawing.Size(105, 20);
            this.floorname.TabIndex = 1;
            this.floorname.Text = "BROWNHUG";
            this.floorname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // floorLabel
            // 
            this.floorLabel.AutoSize = true;
            this.floorLabel.Location = new System.Drawing.Point(285, 32);
            this.floorLabel.Name = "floorLabel";
            this.floorLabel.Size = new System.Drawing.Size(12, 16);
            this.floorLabel.TabIndex = 4;
            this.floorLabel.Text = "f";
            // 
            // ceilLabel
            // 
            this.ceilLabel.AutoSize = true;
            this.ceilLabel.Location = new System.Drawing.Point(396, 32);
            this.ceilLabel.Name = "ceilLabel";
            this.ceilLabel.Size = new System.Drawing.Size(15, 16);
            this.ceilLabel.TabIndex = 5;
            this.ceilLabel.Text = "c";
            // 
            // SectorInfoPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.ceilLabel);
            this.Controls.Add(this.ceilingname);
            this.Controls.Add(this.floorLabel);
            this.Controls.Add(this.ceilingtex);
            this.Controls.Add(this.floorname);
            this.Controls.Add(this.floortex);
            this.Controls.Add(this.sectorinfo);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximumSize = new System.Drawing.Size(12500, 125);
            this.MinimumSize = new System.Drawing.Size(125, 105);
            this.Name = "SectorInfoPanel";
            this.Size = new System.Drawing.Size(601, 105);
            this.sectorinfo.ResumeLayout(false);
            this.sectorinfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label brightness;
		private System.Windows.Forms.Label height;
		private System.Windows.Forms.Label tag;
		private System.Windows.Forms.Label floor;
		private System.Windows.Forms.Label ceiling;
		private System.Windows.Forms.Label effect;
		private System.Windows.Forms.Label ceilingname;
		private System.Windows.Forms.Panel ceilingtex;
		private System.Windows.Forms.GroupBox sectorinfo;
        private System.Windows.Forms.Panel floortex;
        private System.Windows.Forms.Label floorname;
        private System.Windows.Forms.Label floorLabel;
        private System.Windows.Forms.Label ceilLabel;
    }
}
