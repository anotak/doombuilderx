
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
using System.Diagnostics;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Windows;
using System.Reflection;
using System.Globalization;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	public partial class MakeDoorForm : DelayedForm
	{
		#region ================== Properties

		public string DoorTexture { get { return doortexture.TextureName; } }
		public string TrackTexture { get { return tracktexture.TextureName; } }
		public string CeilingTexture { get { return ceilingtexture.TextureName; } }
		public string FloorTexture { get { return floortexture.TextureName; } }
		public bool ResetOffsets { get { return resetoffsets.Checked; } }

		#endregion
		
		#region ================== Constructor / Show

		// Constructor
		public MakeDoorForm()
		{
			InitializeComponent();
		}

		// This sets the properties and shows the form
		public DialogResult Show(IWin32Window owner, string doortex, string tracktex, string ceilingtex, string floortex, bool roffsets)
		{
			this.doortexture.TextureName = doortex;
			this.tracktexture.TextureName = tracktex;
			this.ceilingtexture.TextureName = ceilingtex;
			this.floortexture.TextureName = floortex;
			this.resetoffsets.Checked = roffsets;
			return this.ShowDialog(owner);
		}
		
		#endregion
		
		#region ================== Events

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// No door texture selected?
			if(doortexture.TextureName.Length == 0)
			{
				MessageBox.Show(this, "You have to select at least a texture for the door!", "Make Door", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				doortexture.Focus();
			}
			else
			{
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		private void MakeDoorForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("e_sectors.html");
		}
		
		#endregion
	}
}