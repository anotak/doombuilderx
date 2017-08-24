
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
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class BaseVisualThing : VisualThing, IVisualEventReceiver
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables

		protected BaseVisualMode mode;
		
		private ThingTypeInfo info;
		private bool isloaded;
		private ImageData sprite;
		private float cageradius2;
		private Vector2D pos2d;
		private Vector3D boxp1;
		private Vector3D boxp2;
		
		// Undo/redo
		private int undoticket;

		// If this is set to true, the thing will be rebuilt after the action is performed.
		protected bool changed;

		#endregion
		
		#region ================== Properties

		public bool Changed { get { return changed; } set { changed |= value; } }
		
		#endregion
		
		#region ================== Constructor / Setup
		
		// Constructor
		public BaseVisualThing(BaseVisualMode mode, Thing t) : base(t)
		{
			this.mode = mode;

			// Find thing information
			info = General.Map.Data.GetThingInfo(Thing.Type);

			// Find sprite texture
			if(info.Sprite.Length > 0)
			{
				sprite = General.Map.Data.GetSpriteImage(info.Sprite);
				if(sprite != null) sprite.AddReference();
			}

			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// This builds the thing geometry. Returns false when nothing was created.
		public virtual bool Setup()
		{
			PixelColor sectorcolor = new PixelColor(255, 255, 255, 255);
			
			// Must have a width and height!
			if((info.Radius < 0.1f) || (info.Height < 0.1f)) return false;

			// Find the sector in which the thing resides
			Thing.DetermineSector(mode.BlockMap);

			if(sprite != null)
			{
				if(Thing.Sector != null)
				{
					// Use sector brightness for color shading
					byte brightness = (byte)General.Clamp(Thing.Sector.Brightness, 0, 255);
					sectorcolor = new PixelColor(255, brightness, brightness, brightness);
				}
				
				// Check if the texture is loaded
				sprite.LoadImage();
				isloaded = sprite.IsImageLoaded;
				if(isloaded)
				{
					float offsetx = 0.0f;
					float offsety = 0.0f;
					
					base.Texture = sprite;

					// Determine sprite size and offset
					float radius = sprite.ScaledWidth * 0.5f;
					float height = sprite.ScaledHeight;
					if(sprite is SpriteImage)
					{
						offsetx = (sprite as SpriteImage).OffsetX - radius;
						offsety = (sprite as SpriteImage).OffsetY - height;
					}

					// Scale by thing type/actor scale
					// We do this after the offset x/y determination above, because that is entirely in sprite pixels space
					radius *= info.SpriteScale.Width;
					height *= info.SpriteScale.Height;
					offsetx *= info.SpriteScale.Width;
					offsety *= info.SpriteScale.Height;

					// Make vertices
					WorldVertex[] verts = new WorldVertex[6];
					verts[0] = new WorldVertex(-radius + offsetx, 0.0f, 0.0f + offsety, sectorcolor.ToInt(), 0.0f, 1.0f);
					verts[1] = new WorldVertex(-radius + offsetx, 0.0f, height + offsety, sectorcolor.ToInt(), 0.0f, 0.0f);
					verts[2] = new WorldVertex(+radius + offsetx, 0.0f, height + offsety, sectorcolor.ToInt(), 1.0f, 0.0f);
					verts[3] = verts[0];
					verts[4] = verts[2];
					verts[5] = new WorldVertex(+radius + offsetx, 0.0f, 0.0f + offsety, sectorcolor.ToInt(), 1.0f, 1.0f);
					SetVertices(verts);
				}
				else
				{
					base.Texture = General.Map.Data.Hourglass3D;

					// Determine sprite size
					float radius = Math.Min(info.Radius, info.Height / 2f);
					float height = Math.Min(info.Radius * 2f, info.Height);

					// Make vertices
					WorldVertex[] verts = new WorldVertex[6];
					verts[0] = new WorldVertex(-radius, 0.0f, 0.0f, sectorcolor.ToInt(), 0.0f, 1.0f);
					verts[1] = new WorldVertex(-radius, 0.0f, height, sectorcolor.ToInt(), 0.0f, 0.0f);
					verts[2] = new WorldVertex(+radius, 0.0f, height, sectorcolor.ToInt(), 1.0f, 0.0f);
					verts[3] = verts[0];
					verts[4] = verts[2];
					verts[5] = new WorldVertex(+radius, 0.0f, 0.0f, sectorcolor.ToInt(), 1.0f, 1.0f);
					SetVertices(verts);
				}
			}
			
			// Determine position
			Vector3D pos = Thing.Position;
			if(info.AbsoluteZ)
			{
				// Absolute Z position
				pos.z = Thing.Position.z;
			}
			else if(info.Hangs)
			{
				// Hang from ceiling
				if(Thing.Sector != null) pos.z = Thing.Sector.CeilHeight - info.Height;
				if(Thing.Position.z > 0) pos.z -= Thing.Position.z;
				
				// Check if below floor
				if((Thing.Sector != null) && (pos.z < Thing.Sector.FloorHeight))
				{
					// Put thing on the floor
					pos.z = Thing.Sector.FloorHeight;
				}
			}
			else
			{
				// Stand on floor
				if(Thing.Sector != null) pos.z = Thing.Sector.FloorHeight;
				if(Thing.Position.z > 0) pos.z += Thing.Position.z;
				
				// Check if above ceiling
				if((Thing.Sector != null) && ((pos.z + info.Height) > Thing.Sector.CeilHeight))
				{
					// Put thing against ceiling
					pos.z = Thing.Sector.CeilHeight - info.Height;
				}
			}
			
			// Apply settings
			SetPosition(pos);
			SetCageSize(info.Radius, info.Height);
			SetCageColor(Thing.Color);

			// Keep info for object picking
			cageradius2 = info.Radius * Angle2D.SQRT2;
			cageradius2 = cageradius2 * cageradius2;
			pos2d = pos;
			boxp1 = new Vector3D(pos.x - info.Radius, pos.y - info.Radius, pos.z);
			boxp2 = new Vector3D(pos.x + info.Radius, pos.y + info.Radius, pos.z + info.Height);
			
			// Done
			changed = false;
			return true;
		}
		
		// Disposing
		public override void Dispose()
		{
			if(!IsDisposed)
			{
				if(sprite != null)
				{
					sprite.RemoveReference();
					sprite = null;
				}
			}
			
			base.Dispose();
		}
		
		#endregion
		
		#region ================== Methods
		
		// This forces to rebuild the whole thing
		public void Rebuild()
		{
			// Find thing information
			info = General.Map.Data.GetThingInfo(Thing.Type);

			// Find sprite texture
			if(info.Sprite.Length > 0)
			{
				sprite = General.Map.Data.GetSpriteImage(info.Sprite);
				if(sprite != null) sprite.AddReference();
			}
			
			// Setup visual thing
			Setup();
		}
		
		// This updates the thing when needed
		public override void Update()
		{
			if(!isloaded)
			{
				// Rebuild sprite geometry when sprite is loaded
				if(sprite.IsImageLoaded)
				{
					Setup();
				}
			}
			
			// Let the base update
			base.Update();
		}

		// This performs a fast test in object picking
		public override bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			float distance2 = Line2D.GetDistanceToLineSq(from, to, pos2d, false);
			return (distance2 <= cageradius2);
		}

		// This performs an accurate test for object picking
		public override bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref float u_ray)
		{
			Vector3D delta = to - from;
			float tfar = float.MaxValue;
			float tnear = float.MinValue;
			
			// Ray-Box intersection code
			// See http://www.masm32.com/board/index.php?topic=9941.0
			
			// Check X slab
			if(delta.x == 0.0f)
			{
				if(from.x > boxp2.x || from.x < boxp1.x)
				{
					// Ray is parallel to the planes & outside slab
					return false;
				}
			}
			else
			{
				float tmp = 1.0f / delta.x;
				float t1 = (boxp1.x - from.x) * tmp;
				float t2 = (boxp2.x - from.x) * tmp;
				if(t1 > t2) General.Swap(ref t1, ref t2);
				if(t1 > tnear) tnear = t1;
				if(t2 < tfar) tfar = t2;
				if(tnear > tfar || tfar < 0.0f)
				{
					// Ray missed box or box is behind ray
					return false;
				}
			}
			
			// Check Y slab
			if(delta.y == 0.0f)
			{
				if(from.y > boxp2.y || from.y < boxp1.y)
				{
					// Ray is parallel to the planes & outside slab
					return false;
				}
			}
			else
			{
				float tmp = 1.0f / delta.y;
				float t1 = (boxp1.y - from.y) * tmp;
				float t2 = (boxp2.y - from.y) * tmp;
				if(t1 > t2) General.Swap(ref t1, ref t2);
				if(t1 > tnear) tnear = t1;
				if(t2 < tfar) tfar = t2;
				if(tnear > tfar || tfar < 0.0f)
				{
					// Ray missed box or box is behind ray
					return false;
				}
			}
			
			// Check Z slab
			if(delta.z == 0.0f)
			{
				if(from.z > boxp2.z || from.z < boxp1.z)
				{
					// Ray is parallel to the planes & outside slab
					return false;
				}
			}
			else
			{
				float tmp = 1.0f / delta.z;
				float t1 = (boxp1.z - from.z) * tmp;
				float t2 = (boxp2.z - from.z) * tmp;
				if(t1 > t2) General.Swap(ref t1, ref t2);
				if(t1 > tnear) tnear = t1;
				if(t2 < tfar) tfar = t2;
				if(tnear > tfar || tfar < 0.0f)
				{
					// Ray missed box or box is behind ray
					return false;
				}
			}
			
			// Set interpolation point
			u_ray = (tnear > 0.0f) ? tnear : tfar;
			return true;
		}
		
		#endregion

		#region ================== Events

		// Unused
		public virtual void OnSelectBegin() { }
		public virtual void OnEditBegin() { }
		public virtual void OnMouseMove(MouseEventArgs e) { }
		public virtual void OnChangeTargetBrightness(bool up) { }
		public virtual void OnChangeTextureOffset(int horizontal, int vertical) { }
		public virtual void OnSelectTexture() { }
		public virtual void OnCopyTexture() { }
		public virtual void OnPasteTexture() { }
		public virtual void OnCopyTextureOffsets() { }
		public virtual void OnPasteTextureOffsets() { }
		public virtual void OnTextureAlign(bool alignx, bool aligny) { }
		public virtual void OnToggleUpperUnpegged() { }
		public virtual void OnToggleLowerUnpegged() { }
		public virtual void OnResetTextureOffset() { }
		public virtual void OnProcess(long deltatime) { }
		public virtual void OnTextureFloodfill() { }
		public virtual void OnInsert() { }
		public virtual void OnDelete() { }
		public virtual void ApplyTexture(string texture) { }
		public virtual void ApplyUpperUnpegged(bool set) { }
		public virtual void ApplyLowerUnpegged(bool set) { }
		
		// Return texture name
		public virtual string GetTextureName() { return ""; }

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
		
		// Copy properties
		public virtual void OnCopyProperties()
		{
			BuilderPlug.Me.CopiedThingProps = new ThingProperties(Thing);
			mode.SetActionResult("Copied thing properties.");
		}
		
		// Paste properties
		public virtual void OnPasteProperties()
		{
			if(BuilderPlug.Me.CopiedThingProps != null)
			{
				mode.CreateUndo("Paste thing properties");
				mode.SetActionResult("Pasted thing properties.");
				BuilderPlug.Me.CopiedThingProps.Apply(Thing);
				Thing.UpdateConfiguration();
				this.Rebuild();
				mode.ShowTargetInfo();
			}
		}
		
		// Edit button released
		public virtual void OnEditEnd()
		{
			if(General.Interface.IsActiveWindow)
			{
				List<Thing> things = mode.GetSelectedThings();
				DialogResult result = General.Interface.ShowEditThings(things);
				if(result == DialogResult.OK)
				{
					foreach(Thing t in things)
					{
						VisualThing vt = mode.GetVisualThing(t);
						if(vt != null)
							(vt as BaseVisualThing).Changed = true;
					}
				}
			}
		}
		
		// Raise/lower thing
		public virtual void OnChangeTargetHeight(int amount)
		{
			if(General.Map.FormatInterface.HasThingHeight)
			{
				if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
					undoticket = mode.CreateUndo("Change thing height");

				Thing.Move(Thing.Position + new Vector3D(0.0f, 0.0f, (float)amount));

				mode.SetActionResult("Changed thing height to " + Thing.Position.z + ".");

				this.Changed = true;
			}
		}
		
		#endregion
	}
}
