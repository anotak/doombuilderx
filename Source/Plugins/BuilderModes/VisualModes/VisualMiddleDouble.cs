
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
using System.Drawing;
using System.ComponentModel;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal sealed class VisualMiddleDouble : BaseVisualGeometrySidedef
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Setup

		// Constructor
		public VisualMiddleDouble(BaseVisualMode mode, VisualSector vs, Sidedef s) : base(mode, vs, s)
		{
			// Set render pass
			this.RenderPass = RenderPass.Mask;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// This builds the geometry. Returns false when no geometry created.
		public override bool Setup()
		{
			WorldVertex[] verts;

			int brightness;
			
			// Calculate size of this wall part
			float geotop = (float)Math.Min(Sidedef.Sector.CeilHeight, Sidedef.Other.Sector.CeilHeight);
			float geobottom = (float)Math.Max(Sidedef.Sector.FloorHeight, Sidedef.Other.Sector.FloorHeight);
			float geoheight = geotop - geobottom;
			if(geoheight > 0.001f)
			{
				// Texture given?
				if((Sidedef.MiddleTexture.Length > 0) && (Sidedef.MiddleTexture[0] != '-'))
				{
                    brightness = mode.CalculateBrightness(Sidedef.Sector.Brightness);
					Vector2D t1 = new Vector2D();
					Vector2D t2 = new Vector2D();
					float textop, texbottom;
					float cliptop = 0.0f;
					float clipbottom = 0.0f;
					
					// Load texture
					base.Texture = General.Map.Data.GetTextureImage(Sidedef.LongMiddleTexture);
					if(base.Texture == null)
					{
						base.Texture = General.Map.Data.MissingTexture3D;
                        brightness = mode.CalculateBrightness(255);
                        setuponloadedtexture = Sidedef.LongMiddleTexture;
					}
					else
					{
						if(!base.Texture.IsImageLoaded)
							setuponloadedtexture = Sidedef.LongMiddleTexture;
					}

					// Get texture scaled size
					Vector2D tsz = new Vector2D(base.Texture.ScaledWidth, base.Texture.ScaledHeight);

					// Because the middle texture on a double sided line does not repeat vertically,
					// we first determine the visible portion of the texture
					if(Sidedef.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag))
						textop = geobottom + tsz.y;
					else
						textop = geotop;

					// Apply texture offset
					if (General.Map.Config.ScaledTextureOffsets)
					{
						textop += Sidedef.OffsetY * base.Texture.Scale.y;
					}
					else
					{
						textop += Sidedef.OffsetY;
					}

					
					// Calculate texture portion bottom
					texbottom = textop - tsz.y;

					// Clip texture portion by geometry
					if(geotop < textop) { cliptop = textop - geotop; textop = geotop; }
					if(geobottom > texbottom) { clipbottom = geobottom - texbottom; texbottom = geobottom; }
					
					// Check if anything is still visible
					if((textop - texbottom) > 0.001f)
					{
						// Determine texture coordinatess
						t1.y = cliptop;
						t2.y = tsz.y - clipbottom;

						if (General.Map.Config.ScaledTextureOffsets && !base.Texture.WorldPanning)
						{
							t1.x = Sidedef.OffsetX * base.Texture.Scale.x;
						}
						else
						{
							t1.x = Sidedef.OffsetX;
						} 

						t2.x = t1.x + Sidedef.Line.Length;
						
						// Transform pixel coordinates to texture coordinates
						t1 /= tsz;
						t2 /= tsz;

						// Get world coordinates for geometry
						Vector2D v1, v2;
						if(Sidedef.IsFront)
						{
							v1 = Sidedef.Line.Start.Position;
							v2 = Sidedef.Line.End.Position;
						}
						else
						{
							v1 = Sidedef.Line.End.Position;
							v2 = Sidedef.Line.Start.Position;
						}

						// Make vertices
						verts = new WorldVertex[6];
						verts[0] = new WorldVertex(v1.x, v1.y, texbottom, brightness, t1.x, t2.y);
						verts[1] = new WorldVertex(v1.x, v1.y, textop, brightness, t1.x, t1.y);
						verts[2] = new WorldVertex(v2.x, v2.y, textop, brightness, t2.x, t1.y);
						verts[3] = verts[0];
						verts[4] = verts[2];
						verts[5] = new WorldVertex(v2.x, v2.y, texbottom, brightness, t2.x, t2.y);
						
						// Keep properties
						base.top = textop;
						base.bottom = texbottom;
						
						// Apply vertices
						base.SetVertices(verts);
						return true;
					}
				}
			}
			
			// No geometry for invisible wall
			base.top = geotop;
			base.bottom = geotop;	// bottom same as top so that it has a height of 0 (otherwise it will still be picked up by object picking)
			verts = new WorldVertex[0];
			base.SetVertices(verts);
			return false;
		}
		
		#endregion

		#region ================== Methods

		// Return texture name
		public override string GetTextureName()
		{
			return this.Sidedef.MiddleTexture;
		}

		// This changes the texture
		protected override void SetTexture(string texturename)
		{
			this.Sidedef.SetTextureMid(texturename);
			General.Map.Data.UpdateUsedTextures();
			this.Setup();
		}
		
		#endregion
	}
}
