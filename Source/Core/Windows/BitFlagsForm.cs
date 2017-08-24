
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.IO;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	public partial class BitFlagsForm : DelayedForm
	{
		#region ================== Variables

		private bool setup;
		private int value;
		
		#endregion

		#region ================== Properties

		public int Value { get { return value; } }

		#endregion
		
		#region ================== Constructor

		// Constructor
		public BitFlagsForm()
		{
			// Initialize
			InitializeComponent();
		}

		#endregion

		#region ================== Events

		// When a checkbox is clicked
		private void box_CheckedChanged(object sender, EventArgs e)
		{
			if(!setup)
			{
				// Now setting up
				setup = true;

				// Get this checkbox
				CheckBox thisbox = (sender as CheckBox);
				
				// Checking or unchecking?
				if(thisbox.Checked)
				{
					// Go for all other options
					foreach(CheckBox b in options.Checkboxes)
					{
						// Not the same box?
						if(b != sender)
						{
							// Overlapping bit flags?
							if(((int)b.Tag & (int)thisbox.Tag) != 0)
							{
								// Uncheck the other
								b.Checked = false;
							}
						}
					}
				}

				// Done
				setup = false;
			}
		}
		
		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Close
			DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			this.value = 0;
			
			// Go for all checkboxes to make the final value
			foreach(CheckBox b in options.Checkboxes)
				if(b.Checked) value |= (int)b.Tag;
			
			// Done
			DialogResult = DialogResult.OK;
			this.Close();
		}

		#endregion

		#region ================== Methods
		
		// Setup from EnumList
		public void Setup(EnumList flags, int value)
		{
			setup = true;
			this.value = value;
			
			// Make a checkbox for each item
			foreach(EnumItem item in flags)
			{
				// Make the checkbox
				CheckBox box = options.Add(item.Title, item.GetIntValue());
				
				// Bind checking event
				box.CheckedChanged += new EventHandler(box_CheckedChanged);

				// Checking the box?
				if((value & (int)box.Tag) == (int)box.Tag)
				{
					box.Checked = true;
					
					// Go for all other checkboxes
					foreach(CheckBox b in options.Checkboxes)
					{
						// Not the same box?
						if(b != box)
						{
							// Overlapping bit flags?
							if(((int)b.Tag & (int)box.Tag) != 0)
							{
								// Uncheck the other
								b.Checked = false;
							}
						}
					}
				}
			}

			setup = false;
		}

		// This shows the dialog
		// Returns the flags or the same flags when cancelled
		public static int ShowDialog(IWin32Window owner, EnumList flags, int value)
		{
			int result = value;
			BitFlagsForm f = new BitFlagsForm();
			f.Setup(flags, value);
			if(f.ShowDialog(owner) == DialogResult.OK) result = f.Value;
			f.Dispose();
			return result;
		}
		
		#endregion
	}
}