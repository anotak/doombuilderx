
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
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal abstract class BaseVisualGeometrySector : VisualGeometry, IVisualEventReceiver
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		protected BaseVisualMode mode;
		protected long setuponloadedtexture;

		// This is only used to see if this object has already received a change
		// in a multiselection. The Changed property on the BaseVisualSector is
		// used to indicate a rebuild is needed.
		protected bool changed;
		
		#endregion

		#region ================== Properties
		
		new public BaseVisualSector Sector { get { return (BaseVisualSector)base.Sector; } }
		public bool Changed { get { return changed; } set { changed = value; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public BaseVisualGeometrySector(BaseVisualMode mode, VisualSector vs) : base(vs)
		{
			this.mode = mode;
		}

		#endregion

		#region ================== Methods

		// This changes the height
		protected abstract void ChangeHeight(int amount);
		
		#endregion

		#region ================== Events

		// Unused
		public abstract bool Setup();
		public virtual void OnSelectBegin(){ }
		public virtual void OnEditBegin() { }
		public virtual void OnMouseMove(MouseEventArgs e) { }
		public virtual void OnChangeTextureOffset(int horizontal, int vertical) { }
		public virtual void OnTextureAlign(bool alignx, bool aligny) { }
		public virtual void OnToggleUpperUnpegged() { }
		public virtual void OnToggleLowerUnpegged() { }
		public virtual void OnResetTextureOffset() { }
		public virtual void OnCopyTextureOffsets() { }
		public virtual void OnPasteTextureOffsets() { }
		public virtual void OnInsert() { }
		public virtual void OnDelete() { }
		protected virtual void SetTexture(string texturename) { }
		public virtual void ApplyUpperUnpegged(bool set) { }
		public virtual void ApplyLowerUnpegged(bool set) { }

		// Select or deselect
		public virtual void OnSelectEnd()
		{
			if(this.selected)
			{
				this.selected = false;
				mode.RemoveSelectedObject(this);
			}
			else
			{
				this.selected = true;
				mode.AddSelectedObject(this);
			}
		}
		
		// Processing
		public virtual void OnProcess(long deltatime)
		{
			// If the texture was not loaded, but is loaded now, then re-setup geometry
			if(setuponloadedtexture != 0)
			{
				ImageData t = General.Map.Data.GetFlatImage(setuponloadedtexture);
				if(t != null)
				{
					if(t.IsImageLoaded)
					{
						setuponloadedtexture = 0;
						Setup();
					}
				}
			}
		}

		// Flood-fill textures
		public virtual void OnTextureFloodfill()
		{
			if(BuilderPlug.Me.CopiedFlat != null)
			{
				string oldtexture = GetTextureName();
				long oldtexturelong = Lump.MakeLongName(oldtexture);
				string newtexture = BuilderPlug.Me.CopiedFlat;
				if(newtexture != oldtexture)
				{
					// Get the texture
					ImageData newtextureimage = General.Map.Data.GetFlatImage(newtexture);
					if(newtextureimage != null)
					{
						bool fillceilings = (this is VisualCeiling);
						
						if(fillceilings)
						{
							mode.CreateUndo("Flood-fill ceilings with " + newtexture);
							mode.SetActionResult("Flood-filled ceilings with " + newtexture + ".");
						}
						else
						{
							mode.CreateUndo("Flood-fill floors with " + newtexture);
							mode.SetActionResult("Flood-filled floors with " + newtexture + ".");
						}

						mode.Renderer.SetCrosshairBusy(true);
						General.Interface.RedrawDisplay();

						if(mode.IsSingleSelection)
						{
							// Clear all marks, this will align everything it can
							General.Map.Map.ClearMarkedSectors(false);
						}
						else
						{
							// Limit the alignment to selection only
							General.Map.Map.ClearMarkedSectors(true);
							List<Sector> sectors = mode.GetSelectedSectors();
							foreach(Sector s in sectors) s.Marked = false;
						}
						
						// Do the fill
						Tools.FloodfillFlats(this.Sector.Sector, fillceilings, oldtexturelong, newtextureimage, false);

						// Get the changed sectors
						List<Sector> changes = General.Map.Map.GetMarkedSectors(true);
						foreach(Sector s in changes)
						{
							// Update the visual sector
							if(mode.VisualSectorExists(s))
							{
								BaseVisualSector vs = (mode.GetVisualSector(s) as BaseVisualSector);
								if(fillceilings)
									vs.Ceiling.Setup();
								else
									vs.Floor.Setup();
							}
						}

						General.Map.Data.UpdateUsedTextures();
						mode.Renderer.SetCrosshairBusy(false);
						mode.ShowTargetInfo();
					}
				}
			}
		}
		
		// Copy properties
		public virtual void OnCopyProperties()
		{
			BuilderPlug.Me.CopiedSectorProps = new SectorProperties(Sector.Sector);
			mode.SetActionResult("Copied sector properties.");
		}
		
		// Paste properties
		public virtual void OnPasteProperties()
		{
			if(BuilderPlug.Me.CopiedSectorProps != null)
			{
				mode.CreateUndo("Paste sector properties");
				mode.SetActionResult("Pasted sector properties.");
				BuilderPlug.Me.CopiedSectorProps.Apply(Sector.Sector);
				Sector.UpdateSectorGeometry(true);
				mode.ShowTargetInfo();
			}
		}
		
		// Select texture
		public virtual void OnSelectTexture()
		{
			if(General.Interface.IsActiveWindow)
			{
				string oldtexture = GetTextureName();
				string newtexture = General.Interface.BrowseFlat(General.Interface, oldtexture);
				if(newtexture != oldtexture)
				{
					mode.ApplySelectTexture(newtexture, true);
				}
			}
		}

		// Apply Texture
		public virtual void ApplyTexture(string texture)
		{
			mode.CreateUndo("Change flat " + texture);
			SetTexture(texture);
		}
		
		// Copy texture
		public virtual void OnCopyTexture()
		{
			BuilderPlug.Me.CopiedFlat = GetTextureName();
			if(General.Map.Config.MixTexturesFlats) BuilderPlug.Me.CopiedTexture = GetTextureName();
			mode.SetActionResult("Copied flat " + GetTextureName() + ".");
		}
		
		public virtual void OnPasteTexture() { }

		// Return texture name
		public virtual string GetTextureName() { return ""; }
		
		// Edit button released
		public virtual void OnEditEnd()
		{
			if(General.Interface.IsActiveWindow)
			{
				List<Sector> sectors = mode.GetSelectedSectors();
				DialogResult result = General.Interface.ShowEditSectors(sectors);
				if(result == DialogResult.OK)
				{
					// Rebuild sector
					foreach(Sector s in sectors)
					{
						VisualSector vs = mode.GetVisualSector(s);
						if(vs != null)
							(vs as BaseVisualSector).UpdateSectorGeometry(true);
					}
				}
			}
		}

		// Sector height change
		public virtual void OnChangeTargetHeight(int amount)
		{
			changed = true;
			
			ChangeHeight(amount);

			// Rebuild sector
			Sector.UpdateSectorGeometry(true);
		}
		
		// Sector brightness change
		public virtual void OnChangeTargetBrightness(bool up)
		{
			mode.CreateUndo("Change sector brightness", UndoGroup.SectorBrightnessChange, Sector.Sector.FixedIndex);
			
			if(up)
				Sector.Sector.Brightness = General.Map.Config.BrightnessLevels.GetNextHigher(Sector.Sector.Brightness);
			else
				Sector.Sector.Brightness = General.Map.Config.BrightnessLevels.GetNextLower(Sector.Sector.Brightness);
			
			mode.SetActionResult("Changed sector brightness to " + Sector.Sector.Brightness + ".");

			Sector.Sector.UpdateCache();

			// Rebuild sector
			Sector.UpdateSectorGeometry(false);
		}
		
		#endregion
	}
}
