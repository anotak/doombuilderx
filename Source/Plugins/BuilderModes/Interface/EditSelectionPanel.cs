
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
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Windows;
using System.Reflection;
using System.Globalization;
using System.Threading;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class EditSelectionPanel : UserControl
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables
		
		// Editing mode
		EditSelectionMode mode;
		
		// Input
		private bool userinput;
		
		// Values
		Vector2D orgpos;
		Vector2D orgsize;
		Vector2D abspos;
		Vector2D relpos;
		Vector2D abssize;
		Vector2D relsize;
		float absrotate;
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public EditSelectionPanel(EditSelectionMode mode)
		{
			InitializeComponent();
			this.mode = mode;
		}
		
		#endregion
		
		#region ================== Methods
		
		// This sets the original size
		public void ShowOriginalValues(Vector2D pos, Vector2D size)
		{
			// Set values
			this.orgpos = pos;
			this.orgsize = size;
			
			// Set controls
			orgposx.Text = pos.x.ToString();
			orgposy.Text = pos.y.ToString();
			orgsizex.Text = size.x.ToString();
			orgsizey.Text = size.y.ToString();
		}
		
		// This sets the dynamic values
		public void ShowCurrentValues(Vector2D pos, Vector2D relpos, Vector2D size, Vector2D relsize, float rotation)
		{
			// Set values
			this.abspos = pos;
			this.relpos = relpos;
			this.abssize = size;
			this.relsize = relsize;
			this.absrotate = Angle2D.RadToDeg(rotation);
			
			// Set controls
			absposx.Text = pos.x.ToString("0.#");
			absposy.Text = pos.y.ToString("0.#");
			relposx.Text = relpos.x.ToString("0.#");
			relposy.Text = relpos.y.ToString("0.#");
			abssizex.Text = size.x.ToString("0.#");
			abssizey.Text = size.y.ToString("0.#");
			relsizex.Text = relsize.x.ToString("0.#");
			relsizey.Text = relsize.y.ToString("0.#");
			absrot.Text = this.absrotate.ToString("0.#");
			
			userinput = false;
		}
		
		#endregion
		
		#region ================== Events
		
		// User input given
		private void WhenTextChanged(object sender, EventArgs e)
		{
			userinput = true;
		}
		
		private void absposx_Validated(object sender, EventArgs e)
		{
			if(userinput)
				mode.SetAbsPosX(absposx.GetResultFloat(this.abspos.x));
		}

		private void absposy_Validated(object sender, EventArgs e)
		{
			if(userinput)
				mode.SetAbsPosY(absposy.GetResultFloat(this.abspos.y));
		}

		private void relposx_Validated(object sender, EventArgs e)
		{
			if(userinput)
				mode.SetRelPosX(relposx.GetResultFloat(this.relpos.x));
		}

		private void relposy_Validated(object sender, EventArgs e)
		{
			if(userinput)
				mode.SetRelPosY(relposy.GetResultFloat(this.relpos.y));
		}

		private void abssizex_Validated(object sender, EventArgs e)
		{
			if(userinput)
				mode.SetAbsSizeX(abssizex.GetResultFloat(this.abssize.x));
		}

		private void abssizey_Validated(object sender, EventArgs e)
		{
			if(userinput)
				mode.SetAbsSizeY(abssizey.GetResultFloat(this.abssize.y));
		}

		private void relsizex_Validated(object sender, EventArgs e)
		{
			if(userinput)
				mode.SetRelSizeX(relsizex.GetResultFloat(this.relsize.x));
		}

		private void relsizey_Validated(object sender, EventArgs e)
		{
			if(userinput)
				mode.SetRelSizeY(relsizey.GetResultFloat(this.relsize.y));
		}

		private void absrot_Validated(object sender, EventArgs e)
		{
			if(userinput)
			{
				float rad = Angle2D.DegToRad(absrot.GetResultFloat(this.absrotate));
				mode.SetAbsRotation(rad);
			}
		}

		private void fliph_Click(object sender, EventArgs e)
		{
			General.Actions.InvokeAction("buildermodes_flipselectionh");
			General.Interface.FocusDisplay();
		}

		private void flipv_Click(object sender, EventArgs e)
		{
			General.Actions.InvokeAction("buildermodes_flipselectionv");
			General.Interface.FocusDisplay();
		}

		private void orgposx_Click(object sender, EventArgs e)
		{
			mode.SetAbsPosX(orgpos.x);
			General.Interface.FocusDisplay();
		}

		private void orgposy_Click(object sender, EventArgs e)
		{
			mode.SetAbsPosY(orgpos.y);
			General.Interface.FocusDisplay();
		}

		private void orgsizex_Click(object sender, EventArgs e)
		{
			mode.SetAbsSizeX(orgsize.x);
			General.Interface.FocusDisplay();
		}

		private void orgsizey_Click(object sender, EventArgs e)
		{
			mode.SetAbsSizeY(orgsize.y);
			General.Interface.FocusDisplay();
		}
		
		#endregion
	}
}
