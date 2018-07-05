
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
using CodeImp.DoomBuilder.Geometry;
using Microsoft.Win32;
using System.Diagnostics;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class ThingInfoPanel : UserControl
	{
		private int hexenformatwidth;
		private int doomformatwidth;

		// Constructor
		public ThingInfoPanel()
		{
			// Initialize
			InitializeComponent();

			// Hide stuff when in Doom format
			hexenformatwidth = infopanel.Width;
			doomformatwidth = infopanel.Width - 190;
		}

		// This shows the info
		public void ShowInfo(Thing t)
		{
			ThingTypeInfo ti;
			LinedefActionInfo act = null;
			TypeHandler th;
			string actioninfo = "";
			string zinfo;
			float zvalue;

			// Show/hide stuff depending on format
			if(!General.Map.FormatInterface.HasActionArgs)
			{
				arglbl1.Visible = false;
				arglbl2.Visible = false;
				arglbl3.Visible = false;
				arglbl4.Visible = false;
				arglbl5.Visible = false;
				arg1.Visible = false;
				arg2.Visible = false;
				arg3.Visible = false;
				arg4.Visible = false;
				arg5.Visible = false;
				infopanel.Width = doomformatwidth;
			}
			else
			{
				arglbl1.Visible = true;
				arglbl2.Visible = true;
				arglbl3.Visible = true;
				arglbl4.Visible = true;
				arglbl5.Visible = true;
				arg1.Visible = true;
				arg2.Visible = true;
				arg3.Visible = true;
				arg4.Visible = true;
				arg5.Visible = true;
				infopanel.Width = hexenformatwidth;
			}

			// Move panel
			spritepanel.Left = infopanel.Left + infopanel.Width + infopanel.Margin.Right + spritepanel.Margin.Left;
			
			// Lookup thing info
			ti = General.Map.Data.GetThingInfo(t.Type);

			// Get thing action information
			if(General.Map.Config.LinedefActions.ContainsKey(t.Action))
			{
				act = General.Map.Config.LinedefActions[t.Action];
				actioninfo = act.ToString();
			}
			else if(t.Action == 0)
				actioninfo = t.Action.ToString() + " - None";
			else
				actioninfo = t.Action.ToString() + " - Unknown";
			
			// Determine z info to show
			t.DetermineSector();
			if(ti.AbsoluteZ)
			{
				zvalue = t.Position.z;
				zinfo = zvalue.ToString();
			}
			else
			{
				if(t.Sector != null)
				{
					// Hangs from ceiling?
					if(ti.Hangs)
					{
						zvalue = (float)t.Sector.CeilHeight + t.Position.z;
						zinfo = zvalue.ToString();
					}
					else
					{
						zvalue = (float)t.Sector.FloorHeight + t.Position.z;
						zinfo = zvalue.ToString();
					}
				}
				else
				{
					zvalue = t.Position.z;
					if(zvalue >= 0.0f) zinfo = "+" + zvalue.ToString(); else zinfo = zvalue.ToString();
				}
			}

			// Thing info
			infopanel.Text = " Thing " + t.Index + " ";
			type.Text = t.Type + " - " + ti.Title;
			action.Text = actioninfo;
			position.Text = t.Position.x.ToString() + ", " + t.Position.y.ToString() + ", " + zinfo;
			tag.Text = t.Tag.ToString();
			angle.Text = t.AngleDoom.ToString() + "\u00B0";
			
			// Sprite
			if(ti.Sprite.ToLowerInvariant().StartsWith(DataManager.INTERNAL_PREFIX) && (ti.Sprite.Length > DataManager.INTERNAL_PREFIX.Length))
			{
				spritename.Text = "";
				General.DisplayZoomedImage(spritetex, General.Map.Data.GetSpriteImage(ti.Sprite).GetBitmap());
			}
			else if(ti.Sprite.Length > 0)
			{
				spritename.Text = ti.Sprite;
				General.DisplayZoomedImage(spritetex, General.Map.Data.GetSpriteImage(ti.Sprite).GetPreview());
			}
			else
			{
				spritename.Text = "";
				spritetex.BackgroundImage = null;
			}
			
			// Arguments
			if(act != null)
			{
				arglbl1.Text = act.Args[0].Title + ":";
				arglbl2.Text = act.Args[1].Title + ":";
				arglbl3.Text = act.Args[2].Title + ":";
				arglbl4.Text = act.Args[3].Title + ":";
				arglbl5.Text = act.Args[4].Title + ":";
				arglbl1.Enabled = act.Args[0].Used;
				arglbl2.Enabled = act.Args[1].Used;
				arglbl3.Enabled = act.Args[2].Used;
				arglbl4.Enabled = act.Args[3].Used;
				arglbl5.Enabled = act.Args[4].Used;
				arg1.Enabled = act.Args[0].Used;
				arg2.Enabled = act.Args[1].Used;
				arg3.Enabled = act.Args[2].Used;
				arg4.Enabled = act.Args[3].Used;
				arg5.Enabled = act.Args[4].Used;
				th = General.Types.GetArgumentHandler(act.Args[0]);
				th.SetValue(t.Args[0]); arg1.Text = th.GetStringValue();
				th = General.Types.GetArgumentHandler(act.Args[1]);
				th.SetValue(t.Args[1]); arg2.Text = th.GetStringValue();
				th = General.Types.GetArgumentHandler(act.Args[2]);
				th.SetValue(t.Args[2]); arg3.Text = th.GetStringValue();
				th = General.Types.GetArgumentHandler(act.Args[3]);
				th.SetValue(t.Args[3]); arg4.Text = th.GetStringValue();
				th = General.Types.GetArgumentHandler(act.Args[4]);
				th.SetValue(t.Args[4]); arg5.Text = th.GetStringValue();
			}
			else
			{
				arglbl1.Text = "Argument 1:";
				arglbl2.Text = "Argument 2:";
				arglbl3.Text = "Argument 3:";
				arglbl4.Text = "Argument 4:";
				arglbl5.Text = "Argument 5:";
				arglbl1.Enabled = false;
				arglbl2.Enabled = false;
				arglbl3.Enabled = false;
				arglbl4.Enabled = false;
				arglbl5.Enabled = false;
				arg1.Enabled = false;
				arg2.Enabled = false;
				arg3.Enabled = false;
				arg4.Enabled = false;
				arg5.Enabled = false;
				arg1.Text = "-";
				arg2.Text = "-";
				arg3.Text = "-";
				arg4.Text = "-";
				arg5.Text = "-";
			}

			// Show the whole thing
			this.Show();
            //this.Update(); // ano - don't think this is needed, and is slow
        }

        // When visible changed
        protected override void OnVisibleChanged(EventArgs e)
		{
			// Hiding panels
			if(!this.Visible)
			{
				spritetex.BackgroundImage = null;
			}

			// Call base
			base.OnVisibleChanged(e);
		}
	}
}
