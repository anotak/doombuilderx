
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
using System.Drawing;
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

namespace CodeImp.DoomBuilder.GZDoomEditing
{
	internal abstract class BaseVisualGeometrySector : VisualGeometry, IVisualEventReceiver
	{
		#region ================== Constants

		private const float DRAG_ANGLE_TOLERANCE = 0.06f;

		#endregion

		#region ================== Variables

		protected BaseVisualMode mode;
		protected long setuponloadedtexture;

		// This is only used to see if this object has already received a change
		// in a multiselection. The Changed property on the BaseVisualSector is
		// used to indicate a rebuild is needed.
		protected bool changed;

		protected SectorLevel level;
		protected Effect3DFloor extrafloor;
		
		// Undo/redo
		private int undoticket;
		
		// UV dragging
		private float dragstartanglexy;
		private float dragstartanglez;
		private Vector3D dragorigin;
		private Vector3D deltaxy;
		private Vector3D deltaz;
		private int startoffsetx;
		private int startoffsety;
		protected bool uvdragging;
		private int prevoffsetx;		// We have to provide delta offsets, but I don't
		private int prevoffsety;		// want to calculate with delta offsets to prevent
										// inaccuracy in the dragging.
		
		#endregion

		#region ================== Properties
		
		new public BaseVisualSector Sector { get { return (BaseVisualSector)base.Sector; } }
		public bool Changed { get { return changed; } set { changed = value; } }
		public SectorLevel Level { get { return level; } }
		public Effect3DFloor ExtraFloor { get { return extrafloor; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		protected BaseVisualGeometrySector(BaseVisualMode mode, VisualSector vs) : base(vs)
		{
			this.mode = mode;
		}

		#endregion

		#region ================== Methods

		// This changes the height
		protected abstract void ChangeHeight(int amount);

		// This swaps triangles so that the plane faces the other way
		protected void SwapTriangleVertices(WorldVertex[] verts)
		{
			// Swap some vertices to flip all triangles
			for(int i = 0; i < verts.Length; i += 3)
			{
				// Swap
				WorldVertex v = verts[i];
				verts[i] = verts[i + 1];
				verts[i + 1] = v;
			}
		}

		// This is called to update UV dragging
		protected virtual void UpdateDragUV()
		{
			float u_ray = 1.0f;

			// Calculate intersection position
			this.Level.plane.GetIntersection(General.Map.VisualCamera.Position, General.Map.VisualCamera.Target, ref u_ray);
			Vector3D intersect = General.Map.VisualCamera.Position + (General.Map.VisualCamera.Target - General.Map.VisualCamera.Position) * u_ray;

			// Calculate offsets
			Vector3D dragdelta = intersect - dragorigin;
			float offsetx = dragdelta.x;
			float offsety = dragdelta.y;

			// Apply offsets
			int newoffsetx = startoffsetx - (int)Math.Round(offsetx);
			int newoffsety = startoffsety + (int)Math.Round(offsety);
			mode.ApplyFlatOffsetChange(prevoffsetx - newoffsetx, prevoffsety - newoffsety);
			prevoffsetx = newoffsetx;
			prevoffsety = newoffsety;

			mode.ShowTargetInfo();
		}
		
		#endregion

		#region ================== Events

		// Unused
		public virtual void OnEditBegin() { }
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
		protected abstract void MoveTextureOffset(Point xy);
		protected abstract Point GetTextureOffset();

		// Setup this plane
		public bool Setup() { return this.Setup(this.level, this.extrafloor); }
		public virtual bool Setup(SectorLevel level, Effect3DFloor extrafloor)
		{
			this.level = level;
			this.extrafloor = extrafloor;
			return false;
		}

		// Begin select
		public virtual void OnSelectBegin()
		{
			mode.LockTarget();
			dragstartanglexy = General.Map.VisualCamera.AngleXY;
			dragstartanglez = General.Map.VisualCamera.AngleZ;
			dragorigin = pickintersect;
			startoffsetx = GetTextureOffset().X;
			startoffsety = GetTextureOffset().Y;
			prevoffsetx = GetTextureOffset().X;
			prevoffsety = GetTextureOffset().Y;
		}
		
		// Select or deselect
		public virtual void OnSelectEnd()
		{
			mode.UnlockTarget();
			
			// Was dragging?
			if(uvdragging)
			{
				// Dragging stops now
				uvdragging = false;
			}
			else
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
		}

		// Moving the mouse
		public virtual void OnMouseMove(MouseEventArgs e)
		{
			// Dragging UV?
			if(uvdragging)
			{
				UpdateDragUV();
			}
			else
			{
				// Select button pressed?
				if(General.Actions.CheckActionActive(General.ThisAssembly, "visualselect"))
				{
					// Check if tolerance is exceeded to start UV dragging
					float deltaxy = General.Map.VisualCamera.AngleXY - dragstartanglexy;
					float deltaz = General.Map.VisualCamera.AngleZ - dragstartanglez;
					if((Math.Abs(deltaxy) + Math.Abs(deltaz)) > DRAG_ANGLE_TOLERANCE)
					{
						mode.PreAction(UndoGroup.TextureOffsetChange);
						mode.CreateUndo("Change texture offsets");

						// Start drag now
						uvdragging = true;
						mode.Renderer.ShowSelection = false;
						mode.Renderer.ShowHighlight = false;
						UpdateDragUV();
					}
				}
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
			BuilderPlug.Me.CopiedSectorProps = new SectorProperties(level.sector);
			mode.SetActionResult("Copied sector properties.");
		}
		
		// Paste properties
		public virtual void OnPasteProperties()
		{
			if(BuilderPlug.Me.CopiedSectorProps != null)
			{
				mode.CreateUndo("Paste sector properties");
				mode.SetActionResult("Pasted sector properties.");
				BuilderPlug.Me.CopiedSectorProps.Apply(level.sector);
				if(mode.VisualSectorExists(level.sector))
				{
					BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(level.sector);
					vs.UpdateSectorGeometry(true);
				}
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
						if(mode.VisualSectorExists(s))
						{
							BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(s);
							vs.UpdateSectorGeometry(true);
						}
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
			if(mode.VisualSectorExists(level.sector))
			{
				BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(level.sector);
				vs.UpdateSectorGeometry(true);
			}
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

		// Texture offset change
		public virtual void OnChangeTextureOffset(int horizontal, int vertical)
		{
			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Change texture offsets");

			// Apply offsets
			MoveTextureOffset(new Point(-horizontal, -vertical));

			mode.SetActionResult("Changed texture offsets by " + -horizontal + ", " + -vertical + ".");

			// Update sector geometry
			Sector.UpdateSectorGeometry(false);
			Sector.Rebuild();
		}
		
		#endregion
	}
}
