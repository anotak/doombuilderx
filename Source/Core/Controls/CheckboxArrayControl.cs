
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
using System.Drawing.Drawing2D;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public partial class CheckboxArrayControl : UserControl
	{
		// Constants
		private const int SPACING_Y = 1;

		// Variables
		private List<CheckBox> checkboxes;
		private int columns;
		
		// Properties
		public List<CheckBox> Checkboxes { get { return checkboxes; } }
		public int Columns { get { return columns; } set { columns = value; } }
		
		// Constructor
		public CheckboxArrayControl()
		{
			// Initialize
			InitializeComponent();

			// Setup
			checkboxes = new List<CheckBox>();
		}

		// This adds a checkbox
		public CheckBox Add(string text, object tag)
		{
			// Make new checkbox
			CheckBox c = new CheckBox();
			c.AutoSize = true;
			//c.FlatStyle = FlatStyle.System;
			c.UseVisualStyleBackColor = true;
			c.Text = text;
			c.Tag = tag;

			// Add to list
			this.Controls.Add(c);
			checkboxes.Add(c);

			// Return checkbox
			return c;
		}

		// This positions the checkboxes
		public void PositionCheckboxes()
		{
			int boxheight = 0;
			int columnlength;
			int columnwidth;
			int row = 0;
			int col = 0;
			
			// Checks
			if(columns < 1) return;
			if(checkboxes.Count < 1) return;
			
			// Calculate column width
			columnwidth = this.ClientSize.Width / columns;
			
			// Check what the biggest checkbox height is
			foreach(CheckBox c in checkboxes) if(c.Height > boxheight) boxheight = c.Height;

			// Check what the preferred column length is
			columnlength = 1 + (int)Math.Floor((float)(this.ClientSize.Height - boxheight) / (float)(boxheight + SPACING_Y));
			
			// When not all items fit with the preferred column length
			// we have to extend the column length to make it fit
			if((int)Math.Ceiling((float)checkboxes.Count / (float)columnlength) > columns)
			{
				// Make a column length which works for all items
				columnlength = (int)Math.Ceiling((float)checkboxes.Count / (float)columns);
			}

			// Go for all items
			foreach(CheckBox c in checkboxes)
			{
				// Position checkbox
				c.Location = new Point(col * columnwidth, row * boxheight + (row - 1) * SPACING_Y + SPACING_Y);

				// Next position
				if(++row == columnlength)
				{
					row = 0;
					col++;
				}
			}
		}

		// When layout must change
		private void CheckboxArrayControl_Layout(object sender, LayoutEventArgs e)
		{
			PositionCheckboxes();
		}

		private void CheckboxArrayControl_Paint(object sender, PaintEventArgs e)
		{
			if(this.DesignMode)
			{
				Pen p = new Pen(SystemColors.ControlDark, 1);
				p.DashStyle = DashStyle.Dash;
				e.Graphics.DrawRectangle(p, 0, 0, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);
			}
		}
	}
}
