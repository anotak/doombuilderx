
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
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal abstract class BaseVisualGeometrySidedef : VisualGeometry, IVisualEventReceiver
	{
		#region ================== Constants
		
		private const float DRAG_ANGLE_TOLERANCE = 0.06f;
		
		#endregion

		#region ================== Variables

		protected BaseVisualMode mode;

		protected float top;
		protected float bottom;
		protected long setuponloadedtexture;
		
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
		// Undo/redo
		private int undoticket;
		
		#endregion
		
		#region ================== Properties
		
		public bool IsDraggingUV { get { return uvdragging; } }
		new public BaseVisualSector Sector { get { return (BaseVisualSector)base.Sector; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor for sidedefs
		public BaseVisualGeometrySidedef(BaseVisualMode mode, VisualSector vs, Sidedef sd) : base(vs, sd)
		{
			this.mode = mode;
			this.deltaz = new Vector3D(0.0f, 0.0f, 1.0f);
			this.deltaxy = (sd.Line.End.Position - sd.Line.Start.Position) * sd.Line.LengthInv;
			if(!sd.IsFront) this.deltaxy = -this.deltaxy;
		}
		
		#endregion

		#region ================== Methods

		// This performs a fast test in object picking
		public override bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			// Check if intersection point is between top and bottom
			return (pickintersect.z >= bottom) && (pickintersect.z <= top);
		}

		// This performs an accurate test for object picking
		public override bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref float u_ray)
		{
			// The fast reject pass is already as accurate as it gets,
			// so we just return the intersection distance here
			u_ray = pickrayu;
			return true;
		}
		
		#endregion

		#region ================== Events
		
		// Unused
		public virtual void OnEditBegin() { }
		protected virtual void SetTexture(string texturename) { }
		public abstract bool Setup();
		
		// Insert middle texture
		public virtual void OnInsert()
		{
			// No middle texture yet?
			if(!Sidedef.MiddleRequired() && (string.IsNullOrEmpty(Sidedef.MiddleTexture) || (Sidedef.MiddleTexture[0] == '-')))
			{
				// Make it now
				mode.CreateUndo("Create middle texture");
				mode.SetActionResult("Created middle texture.");
				General.Settings.FindDefaultDrawSettings();
				Sidedef.SetTextureMid(General.Settings.DefaultTexture);

				// Update
				Sector.Changed = true;
				
				// Other side as well
				if(string.IsNullOrEmpty(Sidedef.Other.MiddleTexture) || (Sidedef.Other.MiddleTexture[0] == '-'))
				{
					Sidedef.Other.SetTextureMid(General.Settings.DefaultTexture);

					// Update
					VisualSector othersector = mode.GetVisualSector(Sidedef.Other.Sector);
					if(othersector is BaseVisualSector) (othersector as BaseVisualSector).Changed = true;
				}
			}
		}

		// Delete texture
		public virtual void OnDelete()
		{
			// Remove texture
			mode.CreateUndo("Delete texture");
			mode.SetActionResult("Deleted a texture.");
			SetTexture("-");

			// Update
			Sector.Changed = true;
		}
		
		// Processing
		public virtual void OnProcess(long deltatime)
		{
			// If the texture was not loaded, but is loaded now, then re-setup geometry
			if(setuponloadedtexture != 0)
			{
				ImageData t = General.Map.Data.GetTextureImage(setuponloadedtexture);
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
		
		// Change target height
		public virtual void OnChangeTargetHeight(int amount)
		{
			switch(BuilderPlug.Me.ChangeHeightBySidedef)
			{
				// Change ceiling
				case 1:
					if(!this.Sector.Ceiling.Changed)
						this.Sector.Ceiling.OnChangeTargetHeight(amount);
					break;

				// Change floor
				case 2:
					if(!this.Sector.Floor.Changed)
						this.Sector.Floor.OnChangeTargetHeight(amount);
					break;

				// Change both
				case 3:
					if(!this.Sector.Floor.Changed)
						this.Sector.Floor.OnChangeTargetHeight(amount);
					if(!this.Sector.Ceiling.Changed)
						this.Sector.Ceiling.OnChangeTargetHeight(amount);
					break;
			}
		}
		
		// Reset texture offsets
		public virtual void OnResetTextureOffset()
		{
			mode.CreateUndo("Reset texture offsets");
			mode.SetActionResult("Texture offsets reset.");

			// Apply offsets
			Sidedef.OffsetX = 0;
			Sidedef.OffsetY = 0;

			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			parts.SetupAllParts();
		}
		
		// Toggle upper-unpegged
		public virtual void OnToggleUpperUnpegged()
		{
			if(this.Sidedef.Line.IsFlagSet(General.Map.Config.UpperUnpeggedFlag))
			{
				// Remove flag
				mode.ApplyUpperUnpegged(false);
			}
			else
			{
				// Add flag
				mode.ApplyUpperUnpegged(true);
			}
		}

		// Toggle lower-unpegged
		public virtual void OnToggleLowerUnpegged()
		{
			if(this.Sidedef.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag))
			{
				// Remove flag
				mode.ApplyLowerUnpegged(false);
			}
			else
			{
				// Add flag
				mode.ApplyLowerUnpegged(true);
			}
		}

		
		// This sets the Upper Unpegged flag
		public virtual void ApplyUpperUnpegged(bool set)
		{
			if(!set)
			{
				// Remove flag
				mode.CreateUndo("Remove upper-unpegged setting");
				mode.SetActionResult("Removed upper-unpegged setting.");
				this.Sidedef.Line.SetFlag(General.Map.Config.UpperUnpeggedFlag, false);
			}
			else
			{
				// Add flag
				mode.CreateUndo("Set upper-unpegged setting");
				mode.SetActionResult("Set upper-unpegged setting.");
				this.Sidedef.Line.SetFlag(General.Map.Config.UpperUnpeggedFlag, true);
			}

			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			parts.SetupAllParts();
			
			// Update other sidedef geometry
			if(Sidedef.Other != null)
			{
				BaseVisualSector othersector = (BaseVisualSector)mode.GetVisualSector(Sidedef.Other.Sector);
				parts = othersector.GetSidedefParts(Sidedef.Other);
				parts.SetupAllParts();
			}
		}
		
		
		// This sets the Lower Unpegged flag
		public virtual void ApplyLowerUnpegged(bool set)
		{
			if(!set)
			{
				// Remove flag
				mode.CreateUndo("Remove lower-unpegged setting");
				mode.SetActionResult("Removed lower-unpegged setting.");
				this.Sidedef.Line.SetFlag(General.Map.Config.LowerUnpeggedFlag, false);
			}
			else
			{
				// Add flag
				mode.CreateUndo("Set lower-unpegged setting");
				mode.SetActionResult("Set lower-unpegged setting.");
				this.Sidedef.Line.SetFlag(General.Map.Config.LowerUnpeggedFlag, true);
			}

			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			parts.SetupAllParts();

			// Update other sidedef geometry
			if(Sidedef.Other != null)
			{
				BaseVisualSector othersector = (BaseVisualSector)mode.GetVisualSector(Sidedef.Other.Sector);
				parts = othersector.GetSidedefParts(Sidedef.Other);
				parts.SetupAllParts();
			}
		}


		// Flood-fill textures
		public virtual void OnTextureFloodfill()
		{
			if(BuilderPlug.Me.CopiedTexture != null)
			{
				string oldtexture = GetTextureName();
				long oldtexturelong = Lump.MakeLongName(oldtexture);
				string newtexture = BuilderPlug.Me.CopiedTexture;
				if(newtexture != oldtexture)
				{
					mode.CreateUndo("Flood-fill textures with " + newtexture);
					mode.SetActionResult("Flood-filled textures with " + newtexture + ".");
					
					mode.Renderer.SetCrosshairBusy(true);
					General.Interface.RedrawDisplay();

					// Get the texture
					ImageData newtextureimage = General.Map.Data.GetTextureImage(newtexture);
					if(newtextureimage != null)
					{
						if(mode.IsSingleSelection)
						{
							// Clear all marks, this will align everything it can
							General.Map.Map.ClearMarkedSidedefs(false);
						}
						else
						{
							// Limit the alignment to selection only
							General.Map.Map.ClearMarkedSidedefs(true);
							List<Sidedef> sides = mode.GetSelectedSidedefs();
							foreach(Sidedef sd in sides) sd.Marked = false;
						}
						
						// Do the alignment
						Tools.FloodfillTextures(this.Sidedef, oldtexturelong, newtextureimage, false);

						// Get the changed sidedefs
						List<Sidedef> changes = General.Map.Map.GetMarkedSidedefs(true);
						foreach(Sidedef sd in changes)
						{
							// Update the parts for this sidedef!
							if(mode.VisualSectorExists(sd.Sector))
							{
								BaseVisualSector vs = (mode.GetVisualSector(sd.Sector) as BaseVisualSector);
								VisualSidedefParts parts = vs.GetSidedefParts(sd);
								parts.SetupAllParts();
							}
						}

						General.Map.Data.UpdateUsedTextures();
						mode.Renderer.SetCrosshairBusy(false);
						mode.ShowTargetInfo();
					}
				}
			}
		}
		
		// Auto-align texture X offsets
		public virtual void OnTextureAlign(bool alignx, bool aligny)
		{
			mode.CreateUndo("Auto-align textures");
			mode.SetActionResult("Auto-aligned textures.");
			
			// Make sure the texture is loaded (we need the texture size)
			if(!base.Texture.IsImageLoaded) base.Texture.LoadImage();
			
			if(mode.IsSingleSelection)
			{
				// Clear all marks, this will align everything it can
				General.Map.Map.ClearMarkedSidedefs(false);
			}
			else
			{
				// Limit the alignment to selection only
				General.Map.Map.ClearMarkedSidedefs(true);
				List<Sidedef> sides = mode.GetSelectedSidedefs();
				foreach(Sidedef sd in sides) sd.Marked = false;
			}
			
			// Do the alignment
			Tools.AutoAlignTextures(this.Sidedef, base.Texture, alignx, aligny, false);

			// Get the changed sidedefs
			List<Sidedef> changes = General.Map.Map.GetMarkedSidedefs(true);
			foreach(Sidedef sd in changes)
			{
				// Update the parts for this sidedef!
				if(mode.VisualSectorExists(sd.Sector))
				{
					BaseVisualSector vs = (mode.GetVisualSector(sd.Sector) as BaseVisualSector);
					VisualSidedefParts parts = vs.GetSidedefParts(sd);
					parts.SetupAllParts();
				}
			}
		}
		
		// Select texture
		public virtual void OnSelectTexture()
		{
			if(General.Interface.IsActiveWindow)
			{
				string oldtexture = GetTextureName();
				string newtexture = General.Interface.BrowseTexture(General.Interface, oldtexture);
				if(newtexture != oldtexture)
				{
					mode.ApplySelectTexture(newtexture, false);
				}
			}
		}

		// Apply Texture
		public virtual void ApplyTexture(string texture)
		{
			mode.CreateUndo("Change texture " + texture);
			SetTexture(texture);
		}
		
		// Paste texture
		public virtual void OnPasteTexture()
		{
			if(BuilderPlug.Me.CopiedTexture != null)
			{
				mode.CreateUndo("Paste texture " + BuilderPlug.Me.CopiedTexture);
				mode.SetActionResult("Pasted texture " + BuilderPlug.Me.CopiedTexture + ".");
				SetTexture(BuilderPlug.Me.CopiedTexture);
			}
		}
		
		// Paste texture offsets
		public virtual void OnPasteTextureOffsets()
		{
			mode.CreateUndo("Paste texture offsets");
			Sidedef.OffsetX = BuilderPlug.Me.CopiedOffsets.X;
			Sidedef.OffsetY = BuilderPlug.Me.CopiedOffsets.Y;
			mode.SetActionResult("Pasted texture offsets " + Sidedef.OffsetX + ", " + Sidedef.OffsetY + ".");
			
			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			parts.SetupAllParts();
		}
		
		// Copy texture
		public virtual void OnCopyTexture()
		{
			BuilderPlug.Me.CopiedTexture = GetTextureName();
			if(General.Map.Config.MixTexturesFlats) BuilderPlug.Me.CopiedFlat = GetTextureName();
			mode.SetActionResult("Copied texture " + GetTextureName() + ".");
		}
		
		// Copy texture offsets
		public virtual void OnCopyTextureOffsets()
		{
			BuilderPlug.Me.CopiedOffsets = new Point(Sidedef.OffsetX, Sidedef.OffsetY);
			mode.SetActionResult("Copied texture offsets " + Sidedef.OffsetX + ", " + Sidedef.OffsetY + ".");
		}

		// Copy properties
		public virtual void OnCopyProperties()
		{
			BuilderPlug.Me.CopiedSidedefProps = new SidedefProperties(Sidedef);
			mode.SetActionResult("Copied sidedef properties.");
		}

		// Paste properties
		public virtual void OnPasteProperties()
		{
			if(BuilderPlug.Me.CopiedSidedefProps != null)
			{
				mode.CreateUndo("Paste sidedef properties");
				mode.SetActionResult("Pasted sidedef properties.");
				BuilderPlug.Me.CopiedSidedefProps.Apply(Sidedef);
				
				// Update sectors on both sides
				BaseVisualSector front = (BaseVisualSector)mode.GetVisualSector(Sidedef.Sector);
				if(front != null) front.Changed = true;
				if(Sidedef.Other != null)
				{
					BaseVisualSector back = (BaseVisualSector)mode.GetVisualSector(Sidedef.Other.Sector);
					if(back != null) back.Changed = true;
				}
				mode.ShowTargetInfo();
			}
		}
		
		// Return texture name
		public virtual string GetTextureName() { return ""; }
		
		// Select button pressed
		public virtual void OnSelectBegin()
		{
			mode.LockTarget();
			dragstartanglexy = General.Map.VisualCamera.AngleXY;
			dragstartanglez = General.Map.VisualCamera.AngleZ;
			dragorigin = pickintersect;
			startoffsetx = Sidedef.OffsetX;
			startoffsety = Sidedef.OffsetY;
			prevoffsetx = Sidedef.OffsetX;
			prevoffsety = Sidedef.OffsetY;
		}
		
		// Select button released
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
				// Add/remove selection
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
		
		// Edit button released
		public virtual void OnEditEnd()
		{
			if(General.Interface.IsActiveWindow)
			{
				List<Linedef> linedefs = mode.GetSelectedLinedefs();
				DialogResult result = General.Interface.ShowEditLinedefs(linedefs);
				if(result == DialogResult.OK)
				{
					foreach(Linedef l in linedefs)
					{
						if(l.Front != null)
						{
							VisualSector vs = mode.GetVisualSector(l.Front.Sector);
							if(vs != null)
								(vs as BaseVisualSector).Changed = true;
						}
						
						if(l.Back != null)
						{
							VisualSector vs = mode.GetVisualSector(l.Back.Sector);
							if(vs != null)
								(vs as BaseVisualSector).Changed = true;
						}
					}
				}
			}
		}
		
		// Mouse moves
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
		
		// This is called to update UV dragging
		protected virtual void UpdateDragUV()
		{
			float u_ray;
			
			// Calculate intersection position
			Line2D ray = new Line2D(General.Map.VisualCamera.Position, General.Map.VisualCamera.Target);
			Sidedef.Line.Line.GetIntersection(ray, out u_ray);
			Vector3D intersect = General.Map.VisualCamera.Position + (General.Map.VisualCamera.Target - General.Map.VisualCamera.Position) * u_ray;
			
			// Calculate offsets
			Vector3D dragdelta = intersect - dragorigin;
			Vector3D dragdeltaxy = dragdelta * deltaxy;
			Vector3D dragdeltaz = dragdelta * deltaz;
			float offsetx = dragdeltaxy.GetLength();
			float offsety = dragdeltaz.GetLength();
			if((Math.Sign(dragdeltaxy.x) < 0) || (Math.Sign(dragdeltaxy.y) < 0) || (Math.Sign(dragdeltaxy.z) < 0)) offsetx = -offsetx;
			if((Math.Sign(dragdeltaz.x) < 0) || (Math.Sign(dragdeltaz.y) < 0) || (Math.Sign(dragdeltaz.z) < 0)) offsety = -offsety;
			
			// Apply offsets
			int newoffsetx = startoffsetx - (int)Math.Round(offsetx);
			int newoffsety = startoffsety + (int)Math.Round(offsety);
			mode.ApplyTextureOffsetChange(prevoffsetx - newoffsetx, prevoffsety - newoffsety);
			prevoffsetx = newoffsetx;
			prevoffsety = newoffsety;
			
			mode.ShowTargetInfo();
		}
		
		// Sector brightness change
		public virtual void OnChangeTargetBrightness(bool up)
		{
			if(!Sector.Changed)
			{
				// Change brightness
				mode.CreateUndo("Change sector brightness", UndoGroup.SectorBrightnessChange, Sector.Sector.FixedIndex);

				if(up)
					Sector.Sector.Brightness = General.Map.Config.BrightnessLevels.GetNextHigher(Sector.Sector.Brightness);
				else
					Sector.Sector.Brightness = General.Map.Config.BrightnessLevels.GetNextLower(Sector.Sector.Brightness);
				
				mode.SetActionResult("Changed sector brightness to " + Sector.Sector.Brightness + ".");

				Sector.Sector.UpdateCache();
				
				// Rebuild sector
				Sector.Changed = true;

				// Go for all things in this sector
				foreach(Thing t in General.Map.Map.Things)
				{
					if(t.Sector == Sector.Sector)
					{
						if(mode.VisualThingExists(t))
						{
							// Update thing
							BaseVisualThing vt = (mode.GetVisualThing(t) as BaseVisualThing);
							vt.Changed = true;
						}
					}
				}
			}
		}
		
		// Texture offset change
		public virtual void OnChangeTextureOffset(int horizontal, int vertical)
		{
			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Change texture offsets");
			
			// Apply offsets
			Sidedef.OffsetX -= horizontal;
			Sidedef.OffsetY -= vertical;

			mode.SetActionResult("Changed texture offsets to " + Sidedef.OffsetX + ", " + Sidedef.OffsetY + ".");
			
			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			parts.SetupAllParts();
		}
		
		#endregion
	}
}
