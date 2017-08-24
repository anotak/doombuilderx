using CodeImp.DoomBuilder.Windows;

namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class CurveLinedefsForm
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
			this.distancelabel = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.vertices = new CodeImp.DoomBuilder.Controls.NumericTextbox();
			this.distance = new CodeImp.DoomBuilder.Controls.NumericTextbox();
			this.angle = new CodeImp.DoomBuilder.Controls.NumericTextbox();
			this.verticesbar = new System.Windows.Forms.VScrollBar();
			this.distancebar = new System.Windows.Forms.VScrollBar();
			this.anglebar = new System.Windows.Forms.VScrollBar();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.circular = new System.Windows.Forms.CheckBox();
			this.backwards = new System.Windows.Forms.CheckBox();
			label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(12, 15);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(51, 14);
			label1.TabIndex = 0;
			label1.Text = "Vertices:";
			// 
			// distancelabel
			// 
			this.distancelabel.AutoSize = true;
			this.distancelabel.Location = new System.Drawing.Point(11, 47);
			this.distancelabel.Name = "distancelabel";
			this.distancelabel.Size = new System.Drawing.Size(52, 14);
			this.distancelabel.TabIndex = 1;
			this.distancelabel.Text = "Distance:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(25, 79);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(38, 14);
			this.label3.TabIndex = 2;
			this.label3.Text = "Angle:";
			// 
			// vertices
			// 
			this.vertices.AllowDecimal = false;
			this.vertices.AllowNegative = false;
			this.vertices.AllowRelative = false;
			this.vertices.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.vertices.Location = new System.Drawing.Point(78, 12);
			this.vertices.Name = "vertices";
			this.vertices.Size = new System.Drawing.Size(45, 20);
			this.vertices.TabIndex = 0;
			this.vertices.Text = "8";
			this.vertices.TextChanged += new System.EventHandler(this.vertices_TextChanged);
			this.vertices.Leave += new System.EventHandler(this.vertices_Leave);
			// 
			// distance
			// 
			this.distance.AllowDecimal = false;
			this.distance.AllowNegative = false;
			this.distance.AllowRelative = false;
			this.distance.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.distance.Location = new System.Drawing.Point(78, 44);
			this.distance.Name = "distance";
			this.distance.Size = new System.Drawing.Size(45, 20);
			this.distance.TabIndex = 2;
			this.distance.Text = "128";
			this.distance.TextChanged += new System.EventHandler(this.distance_TextChanged);
			this.distance.Leave += new System.EventHandler(this.distance_Leave);
			// 
			// angle
			// 
			this.angle.AllowDecimal = false;
			this.angle.AllowNegative = false;
			this.angle.AllowRelative = false;
			this.angle.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.angle.Location = new System.Drawing.Point(78, 76);
			this.angle.Name = "angle";
			this.angle.Size = new System.Drawing.Size(45, 20);
			this.angle.TabIndex = 4;
			this.angle.Text = "180";
			this.angle.TextChanged += new System.EventHandler(this.angle_TextChanged);
			this.angle.Leave += new System.EventHandler(this.angle_Leave);
			// 
			// verticesbar
			// 
			this.verticesbar.LargeChange = 1;
			this.verticesbar.Location = new System.Drawing.Point(125, 10);
			this.verticesbar.Maximum = -1;
			this.verticesbar.Minimum = -200;
			this.verticesbar.Name = "verticesbar";
			this.verticesbar.Size = new System.Drawing.Size(19, 24);
			this.verticesbar.TabIndex = 1;
			this.verticesbar.Value = -8;
			this.verticesbar.ValueChanged += new System.EventHandler(this.verticesbar_ValueChanged);
			// 
			// distancebar
			// 
			this.distancebar.LargeChange = 8;
			this.distancebar.Location = new System.Drawing.Point(125, 42);
			this.distancebar.Maximum = 0;
			this.distancebar.Minimum = -10000;
			this.distancebar.Name = "distancebar";
			this.distancebar.Size = new System.Drawing.Size(19, 24);
			this.distancebar.SmallChange = 8;
			this.distancebar.TabIndex = 3;
			this.distancebar.Value = -128;
			this.distancebar.ValueChanged += new System.EventHandler(this.distancebar_ValueChanged);
			// 
			// anglebar
			// 
			this.anglebar.LargeChange = 5;
			this.anglebar.Location = new System.Drawing.Point(125, 74);
			this.anglebar.Maximum = 0;
			this.anglebar.Minimum = -180;
			this.anglebar.Name = "anglebar";
			this.anglebar.Size = new System.Drawing.Size(19, 24);
			this.anglebar.SmallChange = 5;
			this.anglebar.TabIndex = 5;
			this.anglebar.Value = -180;
			this.anglebar.ValueChanged += new System.EventHandler(this.anglebar_ValueChanged);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(84, 167);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(70, 25);
			this.cancel.TabIndex = 9;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.apply.Location = new System.Drawing.Point(7, 167);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(70, 25);
			this.apply.TabIndex = 8;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// circular
			// 
			this.circular.AutoSize = true;
			this.circular.Location = new System.Drawing.Point(22, 108);
			this.circular.Name = "circular";
			this.circular.Size = new System.Drawing.Size(122, 18);
			this.circular.TabIndex = 6;
			this.circular.Text = "Fixed circular curve";
			this.circular.UseVisualStyleBackColor = true;
			this.circular.CheckedChanged += new System.EventHandler(this.circular_CheckedChanged);
			// 
			// backwards
			// 
			this.backwards.AutoSize = true;
			this.backwards.Location = new System.Drawing.Point(22, 132);
			this.backwards.Name = "backwards";
			this.backwards.Size = new System.Drawing.Size(113, 18);
			this.backwards.TabIndex = 7;
			this.backwards.Text = "Curve backwards";
			this.backwards.UseVisualStyleBackColor = true;
			this.backwards.CheckedChanged += new System.EventHandler(this.backwards_CheckedChanged);
			// 
			// CurveLinedefsForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(160, 199);
			this.Controls.Add(this.backwards);
			this.Controls.Add(this.circular);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.anglebar);
			this.Controls.Add(this.distancebar);
			this.Controls.Add(this.verticesbar);
			this.Controls.Add(this.angle);
			this.Controls.Add(this.distance);
			this.Controls.Add(this.vertices);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.distancelabel);
			this.Controls.Add(label1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CurveLinedefsForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Curve Linedefs";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CurveLinedefsForm_FormClosing);
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.CurveLinedefsForm_HelpRequested);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label3;
		private CodeImp.DoomBuilder.Controls.NumericTextbox vertices;
		private CodeImp.DoomBuilder.Controls.NumericTextbox distance;
		private CodeImp.DoomBuilder.Controls.NumericTextbox angle;
		private System.Windows.Forms.VScrollBar verticesbar;
		private System.Windows.Forms.VScrollBar distancebar;
		private System.Windows.Forms.VScrollBar anglebar;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.CheckBox circular;
		private System.Windows.Forms.Label distancelabel;
		private System.Windows.Forms.CheckBox backwards;
	}
}