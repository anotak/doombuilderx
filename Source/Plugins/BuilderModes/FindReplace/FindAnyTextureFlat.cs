
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Any Texture or Flat", BrowseButton = true)]
	internal class FindAnyTextureFlat : FindReplaceType
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties
		
		public override Image BrowseImage { get { return Properties.Resources.List_Images; } }		
		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public FindAnyTextureFlat()
		{
			// Initialize

		}

		// Destructor
		~FindAnyTextureFlat()
		{
		}

		#endregion

		#region ================== Methods

		// This is called to test if the item should be displayed
		public override bool DetermineVisiblity()
		{
			return General.Map.Config.MixTexturesFlats;
		}


		// This is called when the browse button is pressed
		public override string Browse(string initialvalue)
		{
			return General.Interface.BrowseTexture(BuilderPlug.Me.FindReplaceForm, initialvalue);
		}


		// This is called to perform a search (and replace)
		// Returns a list of items to show in the results list
		// replacewith is null when not replacing
		public override FindReplaceObject[] Find(string value, bool withinselection, string replacewith, bool keepselection)
		{
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Interpret the replacement
			if(replacewith != null)
			{
				// If it cannot be interpreted, set replacewith to null (not replacing at all)
				if(replacewith.Length < 0) replacewith = null;
				if(replacewith.Length > 8) replacewith = null;
				if(replacewith == null)
				{
					MessageBox.Show("Invalid replace value for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return objs.ToArray();
				}
			}

			// Interpret the find
			long longfind = Lump.MakeLongName(value.Trim());
			
			// Where to search?
			ICollection<Sector> seclist = withinselection ? General.Map.Map.GetSelectedSectors(true) : General.Map.Map.Sectors;
			ICollection<Sidedef> sidelist = withinselection ? General.Map.Map.GetSidedefsFromSelectedLinedefs(true) : General.Map.Map.Sidedefs;
			
			// Go for all sectors
			foreach(Sector s in seclist)
			{
				// Flat matches?
				if(s.LongCeilTexture == longfind)
				{
					// Replace and add to list
					if(replacewith != null) s.SetCeilTexture(replacewith);
					objs.Add(new FindReplaceObject(s, "Sector " + s.Index + " (ceiling)"));
				}

				if(s.LongFloorTexture == longfind)
				{
					// Replace and add to list
					if(replacewith != null) s.SetFloorTexture(replacewith);
					objs.Add(new FindReplaceObject(s, "Sector " + s.Index + " (floor)"));
				}
			}
			
			// Go for all sidedefs
			foreach(Sidedef sd in sidelist)
			{
				string side = sd.IsFront ? "front" : "back";
				
				if(sd.LongHighTexture == longfind)
				{
					// Replace and add to list
					if(replacewith != null) sd.SetTextureHigh(replacewith);
					objs.Add(new FindReplaceObject(sd, "Sidedef " + sd.Index + " (" + side + ", high)"));
				}
				
				if(sd.LongMiddleTexture == longfind)
				{
					// Replace and add to list
					if(replacewith != null) sd.SetTextureMid(replacewith);
					objs.Add(new FindReplaceObject(sd, "Sidedef " + sd.Index + " (" + side + ", middle)"));
				}
				
				if(sd.LongLowTexture == longfind)
				{
					// Replace and add to list
					if(replacewith != null) sd.SetTextureLow(replacewith);
					objs.Add(new FindReplaceObject(sd, "Sidedef " + sd.Index + " (" + side + ", low)"));
				}
			}
			
			// When replacing, make sure we keep track of used textures
			if(replacewith != null) General.Map.Data.UpdateUsedTextures();

			return objs.ToArray();
		}

		// This is called when a specific object is selected from the list
		public override void ObjectSelected(FindReplaceObject[] selection)
		{
			if(selection.Length == 1)
			{
				ZoomToSelection(selection);
				if(selection[0].Object is Sector)
					General.Interface.ShowSectorInfo(selection[0].Sector);
				else if(selection[0].Object is Sidedef)
					General.Interface.ShowLinedefInfo(selection[0].Sidedef.Line);
			}
			else
				General.Interface.HideInfo();
			
			General.Map.Map.ClearAllSelected();
			foreach(FindReplaceObject obj in selection)
			{
				if(obj.Object is Sector)
					obj.Sector.Selected = true;
				else if(obj.Object is Sidedef)
					obj.Sidedef.Line.Selected = true;
			}
		}

		// Render selection
		public override void PlotSelection(IRenderer2D renderer, FindReplaceObject[] selection)
		{
			foreach(FindReplaceObject o in selection)
			{
				if(o.Object is Sector)
				{
					foreach(Sidedef sd in o.Sector.Sidedefs)
					{
						renderer.PlotLinedef(sd.Line, General.Colors.Selection);
					}
				}
				else if(o.Object is Sidedef)
				{
					renderer.PlotLinedef(o.Sidedef.Line, General.Colors.Selection);
				}
			}
		}

		// Edit objects
		public override void EditObjects(FindReplaceObject[] selection)
		{
			List<Sector> sectors = new List<Sector>(selection.Length);
			List<Linedef> lines = new List<Linedef>(selection.Length);
			foreach(FindReplaceObject o in selection)
			{
				if(o.Object is Sector)
					sectors.Add(o.Sector);
				else
					lines.Add(o.Sidedef.Line);
			}
			if(sectors.Count > 0) General.Interface.ShowEditSectors(sectors);
			if(lines.Count > 0) General.Interface.ShowEditLinedefs(lines);
		}

		#endregion
	}
}
