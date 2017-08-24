namespace CodeImp.DoomBuilder.Controls
{
	partial class VertexInfoPanel
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
            System.Windows.Forms.Label label1;
            this.vertexinfo = new System.Windows.Forms.GroupBox();
            this.position = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            this.vertexinfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(16, 42);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(62, 16);
            label1.TabIndex = 2;
            label1.Text = "Position:";
            // 
            // vertexinfo
            // 
            this.vertexinfo.Controls.Add(this.position);
            this.vertexinfo.Controls.Add(label1);
            this.vertexinfo.Location = new System.Drawing.Point(0, 0);
            this.vertexinfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.vertexinfo.Name = "vertexinfo";
            this.vertexinfo.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.vertexinfo.Size = new System.Drawing.Size(204, 105);
            this.vertexinfo.TabIndex = 0;
            this.vertexinfo.TabStop = false;
            this.vertexinfo.Text = " Vertex ";
            // 
            // position
            // 
            this.position.AutoSize = true;
            this.position.Location = new System.Drawing.Point(82, 42);
            this.position.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.position.Name = "position";
            this.position.Size = new System.Drawing.Size(32, 16);
            this.position.TabIndex = 3;
            this.position.Text = "0, 0";
            // 
            // VertexInfoPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.vertexinfo);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximumSize = new System.Drawing.Size(12500, 105);
            this.MinimumSize = new System.Drawing.Size(125, 105);
            this.Name = "VertexInfoPanel";
            this.Size = new System.Drawing.Size(491, 105);
            this.vertexinfo.ResumeLayout(false);
            this.vertexinfo.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label position;
		private System.Windows.Forms.GroupBox vertexinfo;

	}
}
