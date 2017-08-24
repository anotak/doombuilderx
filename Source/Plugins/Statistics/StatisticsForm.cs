
#region ================== Copyright (c) 2009 Pascal vd Heiden

/*
 * Copyright (c) 2009 Pascal vd Heiden, www.codeimp.com
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
using System.Linq;
using System.Text;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Statistics
{
	public partial class StatisticsForm : Form
	{
		// Constructor
		public StatisticsForm()
		{
			InitializeComponent();
		}

		// We're going to use this to show the form
		public void ShowWindow(Form owner)
		{
			// Position this window in the left-top corner of owner
			this.Location = new Point(owner.Location.X + 20, owner.Location.Y + 90); 

			// Update statistics
			UpdateStats();
			
			// Show it
			base.Show(owner);
		}

		// Form is closing event
		private void StatisticsForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// When the user is closing the window we want to cancel this, because it
			// would also unload (dispose) the form. We only want to hide the window
			// so that it can be re-used next time when this editing mode is activated.
			if(e.CloseReason == CloseReason.UserClosing)
			{
				// Just cancel the editing mode. This will automatically call
				// OnCancel() which will switch to the previous mode and in turn
				// calls OnDisengage() which hides this window.
				General.Editing.CancelMode();
				e.Cancel = true;
			}
		}

		// This function updates all statistics on the form
		public void UpdateStats()
		{
			// You can access the Doom Builder core through the static General class.
			// General.Map gives you access the manager of the map that is being edited.
			// General.Map.Map gives you access to the low level map structure.
			
			verticescount.Text = General.Map.Map.Vertices.Count.ToString();
			linedefscount.Text = General.Map.Map.Linedefs.Count.ToString();
			sidedefscount.Text = General.Map.Map.Sidedefs.Count.ToString();
			sectorscount.Text = General.Map.Map.Sectors.Count.ToString();
			thingscount.Text = General.Map.Map.Things.Count.ToString();
		}

		// Close button clicked
		private void closebutton_Click(object sender, EventArgs e)
		{
			General.Editing.CancelMode();
		}
	}
}
