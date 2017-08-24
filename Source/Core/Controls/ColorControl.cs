
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class ColorControl : UserControl
	{
		// Constructor
		public ColorControl()
		{
			// Initialize
			InitializeComponent();
		}

		// Properties
		public string Label { get { return label.Text; } set { label.Text = value; } }
		public PixelColor Color { get { return PixelColor.FromColor(panel.BackColor); } set { panel.BackColor = System.Drawing.Color.FromArgb(value.ToInt()); } }

		// Button clicked
		private void button_Click(object sender, EventArgs e)
		{
			// Mouse up first
			button_MouseUp(sender, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
			
			// Show color dialog
			dialog.Color = panel.BackColor;
			if(dialog.ShowDialog(this.ParentForm) == DialogResult.OK)
			{
				// Apply new color
				panel.BackColor = dialog.Color;
			}
		}

		// Resized
		private void ColorControl_Resize(object sender, EventArgs e)
		{
			try
			{
				button.Left = ClientSize.Width - button.Width;
				panel.Left = ClientSize.Width - button.Width - panel.Width - 3;
				label.Left = 0;
				label.Width = panel.Left;
			}
			catch(Exception) { }
		}

		// Mouse pressed on button
		private void button_MouseDown(object sender, MouseEventArgs e)
		{
			// This moves the image 1 pixel to right-bottom
			if(e.Button == MouseButtons.Left)
				button.Padding = new Padding(1, 0, 1, 1);
		}

		// Mouse released on button
		private void button_MouseUp(object sender, MouseEventArgs e)
		{
			// This moves the image 1 pixel to normal position
			if(e.Button == MouseButtons.Left)
				button.Padding = new Padding(0, 0, 2, 3);
			
			// Lose focus
			panel.Focus();
		}

		// Mouse moves over button
		private void button_MouseMove(object sender, MouseEventArgs e)
		{
			// Outside button rect?
			if((e.X < 0) || (e.X >= button.Width) || (e.Y < 0) || (e.Y >= button.Height))
				button.Padding = new Padding(0, 0, 2, 3);
		}
	}
}
