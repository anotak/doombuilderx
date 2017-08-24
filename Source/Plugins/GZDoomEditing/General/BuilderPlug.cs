
#region ================== Copyright (c) 2010 Pascal vd Heiden

/*
 * Copyright (c) 2010 Pascal vd Heiden
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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.GZDoomEditing
{
	public class BuilderPlug : Plug
	{
		#region ================== Structures

		private struct SidedefAlignJob
		{
			public Sidedef sidedef;

			public float offsetx;

			// When this is true, the previous sidedef was on the left of
			// this one and the texture X offset of this sidedef can be set
			// directly. When this is false, the length of this sidedef
			// must be subtracted from the X offset first.
			public bool forward;
		}

		#endregion

		#region ================== Variables

		// Static instance
		private static BuilderPlug me;

		// Settings
		private int showvisualthings;			// 0 = none, 1 = sprite only, 2 = sprite caged
		private int changeheightbysidedef;		// 0 = nothing, 1 = change ceiling, 2 = change floor
		private bool visualmodeclearselection;
		private bool usegravity;
		private bool usehighlight;
		private float stitchrange;
		
		// Copy/paste
		private string copiedtexture;
		private string copiedflat;
		private Point copiedoffsets;
		private VertexProperties copiedvertexprops;
		private SectorProperties copiedsectorprops;
		private SidedefProperties copiedsidedefprops;
		private LinedefProperties copiedlinedefprops;
		private ThingProperties copiedthingprops;
		
		#endregion

		#region ================== Properties

		// Static property to access the BuilderPlug
		public static BuilderPlug Me { get { return me; } }
		
		// This is the lowest Doom Builder core revision that is required for this plugin to work
		public override int MinimumRevision { get { return 1394; } }
		
		// Settings
		public int ShowVisualThings { get { return showvisualthings; } set { showvisualthings = value; } }
		public int ChangeHeightBySidedef { get { return changeheightbysidedef; } }
		public bool VisualModeClearSelection { get { return visualmodeclearselection; } }
		public bool UseGravity { get { return usegravity; } set { usegravity = value; } }
		public bool UseHighlight { get { return usehighlight; } set { usehighlight = value; } }
		public float StitchRange { get { return stitchrange; } }
		
		// Copy/paste
		public string CopiedTexture { get { return copiedtexture; } set { copiedtexture = value; } }
		public string CopiedFlat { get { return copiedflat; } set { copiedflat = value; } }
		public Point CopiedOffsets { get { return copiedoffsets; } set { copiedoffsets = value; } }
		public VertexProperties CopiedVertexProps { get { return copiedvertexprops; } set { copiedvertexprops = value; } }
		public SectorProperties CopiedSectorProps { get { return copiedsectorprops; } set { copiedsectorprops = value; } }
		public SidedefProperties CopiedSidedefProps { get { return copiedsidedefprops; } set { copiedsidedefprops = value; } }
		public LinedefProperties CopiedLinedefProps { get { return copiedlinedefprops; } set { copiedlinedefprops = value; } }
		public ThingProperties CopiedThingProps { get { return copiedthingprops; } set { copiedthingprops = value; } }
		
		#endregion

		#region ================== Initialize / Dispose
		
		// This event is called when the plugin is initialized
		public override void OnInitialize()
		{
			base.OnInitialize();

			// Keep a static reference
            me = this;

			// Settings
			showvisualthings = 2;
			usegravity = false;
			usehighlight = true;
			LoadSettings();
		}
		
		// This is called when the plugin is terminated
		public override void Dispose()
		{
			base.Dispose();
		}

		#endregion

		#region ================== Methods

		// This loads the plugin settings
		private void LoadSettings()
		{
			changeheightbysidedef = General.Settings.ReadPluginSetting("BuilderModes", "changeheightbysidedef", 0);
			visualmodeclearselection = General.Settings.ReadPluginSetting("BuilderModes", "visualmodeclearselection", false);
			stitchrange = (float)General.Settings.ReadPluginSetting("BuilderModes", "stitchrange", 20);
		}

		#endregion

		#region ================== Events

		// When the Preferences dialog is closed
		public override void OnClosePreferences(PreferencesController controller)
		{
			base.OnClosePreferences(controller);
			
			// Apply settings that could have been changed
			LoadSettings();
		}
		
		#endregion
		
		#region ================== Classic Mode Surfaces

		// This is called when the vertices are created for the classic mode surfaces
		public override void OnSectorFloorSurfaceUpdate(Sector s, ref FlatVertex[] vertices)
		{
			ImageData img = General.Map.Data.GetFlatImage(s.LongFloorTexture);
			if((img != null) && img.IsImageLoaded)
			{
				// Fetch ZDoom fields
				Vector2D offset = new Vector2D(s.Fields.GetValue("xpanningfloor", 0.0f),
				                               s.Fields.GetValue("ypanningfloor", 0.0f));
				Vector2D scale = new Vector2D(s.Fields.GetValue("xscalefloor", 1.0f),
				                              s.Fields.GetValue("yscalefloor", 1.0f));
				float rotate = s.Fields.GetValue("rotationfloor", 0.0f);
				int color = s.Fields.GetValue("lightcolor", -1);
				int light = s.Fields.GetValue("lightfloor", 0);
				bool absolute = s.Fields.GetValue("lightfloorabsolute", false);
				
				// Setup the vertices with the given settings
				SetupSurfaceVertices(vertices, s, img, offset, scale, rotate, color, light, absolute);
			}
		}

		// This is called when the vertices are created for the classic mode surfaces
		public override void OnSectorCeilingSurfaceUpdate(Sector s, ref FlatVertex[] vertices)
		{
			ImageData img = General.Map.Data.GetFlatImage(s.LongFloorTexture);
			if((img != null) && img.IsImageLoaded)
			{
				// Fetch ZDoom fields
				Vector2D offset = new Vector2D(s.Fields.GetValue("xpanningceiling", 0.0f),
											   s.Fields.GetValue("ypanningceiling", 0.0f));
				Vector2D scale = new Vector2D(s.Fields.GetValue("xscaleceiling", 1.0f),
											  s.Fields.GetValue("yscaleceiling", 1.0f));
				float rotate = s.Fields.GetValue("rotationceiling", 0.0f);
				int color = s.Fields.GetValue("lightcolor", -1);
				int light = s.Fields.GetValue("lightceiling", 0);
				bool absolute = s.Fields.GetValue("lightceilingabsolute", false);
				
				// Setup the vertices with the given settings
				SetupSurfaceVertices(vertices, s, img, offset, scale, rotate, color, light, absolute);
			}
		}

		// This applies the given values on the vertices
		private void SetupSurfaceVertices(FlatVertex[] vertices, Sector s, ImageData img, Vector2D offset,
										  Vector2D scale, float rotate, int color, int light, bool absolute)
		{
			// Prepare for math!
			rotate = Angle2D.DegToRad(rotate);
			Vector2D texscale = new Vector2D(1.0f / img.ScaledWidth, 1.0f / img.ScaledHeight);
			if(!absolute) light = s.Brightness + light;
			PixelColor lightcolor = PixelColor.FromInt(color);
			PixelColor brightness = PixelColor.FromInt(General.Map.Renderer2D.CalculateBrightness(light));
			PixelColor finalcolor = PixelColor.Modulate(lightcolor, brightness);
			color = finalcolor.WithAlpha(255).ToInt();
			
			// Do the math for all vertices
			for(int i = 0; i < vertices.Length; i++)
			{
				Vector2D pos = new Vector2D(vertices[i].x, vertices[i].y);
				pos = pos.GetRotated(rotate);
				pos.y = -pos.y;
				pos = (pos + offset) * scale * texscale;
				vertices[i].u = pos.x;
				vertices[i].v = pos.y;
				vertices[i].c = color;
			}
		}

		#endregion

		#region ================== Texture Alignment

		// This performs texture alignment along all walls that match with the same texture
		// NOTE: This method uses the sidedefs marking to indicate which sides have been aligned
		// When resetsidemarks is set to true, all sidedefs will first be marked false (not aligned).
		// Setting resetsidemarks to false is usefull to align only within a specific selection
		// (set the marked property to true for the sidedefs outside the selection)
		public static void AutoAlignTextures(Sidedef start, SidedefPart part, ImageData texture, bool alignx, bool aligny, bool resetsidemarks)
		{
			Stack<SidedefAlignJob> todo = new Stack<SidedefAlignJob>(50);
			float scalex = (General.Map.Config.ScaledTextureOffsets && !texture.WorldPanning) ? texture.Scale.x : 1.0f;
			float scaley = (General.Map.Config.ScaledTextureOffsets && !texture.WorldPanning) ? texture.Scale.y : 1.0f;

			// Mark all sidedefs false (they will be marked true when the texture is aligned)
			if(resetsidemarks) General.Map.Map.ClearMarkedSidedefs(false);

			if(!texture.IsImageLoaded) return;

			// Determine the Y alignment
			float ystartalign = start.OffsetY;
			switch(part)
			{
				case SidedefPart.Upper: ystartalign += start.Fields.GetValue("offsety_top", 0.0f); break;
				case SidedefPart.Middle: ystartalign += start.Fields.GetValue("offsety_mid", 0.0f); break;
				case SidedefPart.Lower: ystartalign += start.Fields.GetValue("offsety_bottom", 0.0f); break;
			}

			// Begin with first sidedef
			SidedefAlignJob first = new SidedefAlignJob();
			first.sidedef = start;
			first.offsetx = start.OffsetX;
			switch(part)
			{
				case SidedefPart.Upper: first.offsetx += start.Fields.GetValue("offsetx_top", 0.0f); break;
				case SidedefPart.Middle: first.offsetx += start.Fields.GetValue("offsetx_mid", 0.0f); break;
				case SidedefPart.Lower: first.offsetx += start.Fields.GetValue("offsetx_bottom", 0.0f); break;
			}
			first.forward = true;
			todo.Push(first);

			// Continue until nothing more to align
			while(todo.Count > 0)
			{
				Vertex v;
				float forwardoffset;
				float backwardoffset;
				float offsetscalex = 1.0f;
				
				// Get the align job to do
				SidedefAlignJob j = todo.Pop();

				bool matchtop = ((j.sidedef.LongHighTexture == texture.LongName) && j.sidedef.HighRequired());
				bool matchbottom = ((j.sidedef.LongLowTexture == texture.LongName) && j.sidedef.LowRequired());
				bool matchmid = ((j.sidedef.LongMiddleTexture == texture.LongName) && (j.sidedef.MiddleRequired() || ((j.sidedef.MiddleTexture.Length > 0) && (j.sidedef.MiddleTexture[0] != '-'))));

				if(matchtop) offsetscalex = j.sidedef.Fields.GetValue("scalex_top", 1.0f);
				else if(matchbottom) offsetscalex = j.sidedef.Fields.GetValue("scalex_bottom", 1.0f);
				else if(matchmid) offsetscalex = j.sidedef.Fields.GetValue("scalex_mid", 1.0f);

				if(j.forward)
				{
					// Apply alignment
					if(alignx)
					{
						//j.sidedef.OffsetX = j.offsetx;
						float offset = j.offsetx;
						offset %= (float)texture.Height;
						offset -= j.sidedef.OffsetX;

						j.sidedef.Fields.BeforeFieldsChange();
						if(matchtop) j.sidedef.Fields["offsetx_top"] = new UniValue(UniversalType.Float, offset);
						if(matchbottom) j.sidedef.Fields["offsetx_bottom"] = new UniValue(UniversalType.Float, offset);
						if(matchmid) j.sidedef.Fields["offsetx_mid"] = new UniValue(UniversalType.Float, offset);
					}
					if(aligny)
					{
						//j.sidedef.OffsetY = (int)Math.Round((start.Sector.CeilHeight - j.sidedef.Sector.CeilHeight) / scaley) + start.OffsetY;
						float offset = ((float)(start.Sector.CeilHeight - j.sidedef.Sector.CeilHeight) / scaley) + ystartalign;
						offset %= (float)texture.Height;
						offset -= j.sidedef.OffsetY;

						j.sidedef.Fields.BeforeFieldsChange();
						if(matchtop) j.sidedef.Fields["offsety_top"] = new UniValue(UniversalType.Float, offset);
						if(matchbottom) j.sidedef.Fields["offsety_bottom"] = new UniValue(UniversalType.Float, offset);
						if(matchmid) j.sidedef.Fields["offsety_mid"] = new UniValue(UniversalType.Float, offset);
					}
					forwardoffset = j.offsetx + (int)Math.Round(j.sidedef.Line.Length / scalex * offsetscalex);
					backwardoffset = j.offsetx;

					// Done this sidedef
					j.sidedef.Marked = true;

					// Add sidedefs backward (connected to the left vertex)
					v = j.sidedef.IsFront ? j.sidedef.Line.Start : j.sidedef.Line.End;
					AddSidedefsForAlignment(todo, v, false, backwardoffset, texture.LongName);

					// Add sidedefs forward (connected to the right vertex)
					v = j.sidedef.IsFront ? j.sidedef.Line.End : j.sidedef.Line.Start;
					AddSidedefsForAlignment(todo, v, true, forwardoffset, texture.LongName);
				}
				else
				{
					// Apply alignment
					if(alignx)
					{
						//j.sidedef.OffsetX = j.offsetx - (int)Math.Round(j.sidedef.Line.Length / scalex);
						float offset = j.offsetx - (int)Math.Round(j.sidedef.Line.Length / scalex);
						offset %= (float)texture.Height;
						offset -= j.sidedef.OffsetX;

						j.sidedef.Fields.BeforeFieldsChange();
						if(matchtop) j.sidedef.Fields["offsetx_top"] = new UniValue(UniversalType.Float, offset);
						if(matchbottom) j.sidedef.Fields["offsetx_bottom"] = new UniValue(UniversalType.Float, offset);
						if(matchmid) j.sidedef.Fields["offsetx_mid"] = new UniValue(UniversalType.Float, offset);
					}
					if(aligny)
					{
						//j.sidedef.OffsetY = (int)Math.Round((start.Sector.CeilHeight - j.sidedef.Sector.CeilHeight) / scaley) + start.OffsetY;
						float offset = ((float)(start.Sector.CeilHeight - j.sidedef.Sector.CeilHeight) / scaley) + ystartalign;
						offset %= (float)texture.Height;
						offset -= j.sidedef.OffsetY;

						j.sidedef.Fields.BeforeFieldsChange();
						if(matchtop) j.sidedef.Fields["offsety_top"] = new UniValue(UniversalType.Float, offset);
						if(matchbottom) j.sidedef.Fields["offsety_bottom"] = new UniValue(UniversalType.Float, offset);
						if(matchmid) j.sidedef.Fields["offsety_mid"] = new UniValue(UniversalType.Float, offset);
					}
					forwardoffset = j.offsetx;
					backwardoffset = j.offsetx - (int)Math.Round(j.sidedef.Line.Length / scalex * offsetscalex);

					// Done this sidedef
					j.sidedef.Marked = true;

					// Add sidedefs forward (connected to the right vertex)
					v = j.sidedef.IsFront ? j.sidedef.Line.End : j.sidedef.Line.Start;
					AddSidedefsForAlignment(todo, v, true, forwardoffset, texture.LongName);

					// Add sidedefs backward (connected to the left vertex)
					v = j.sidedef.IsFront ? j.sidedef.Line.Start : j.sidedef.Line.End;
					AddSidedefsForAlignment(todo, v, false, backwardoffset, texture.LongName);
				}
			}
		}

		// This adds the matching, unmarked sidedefs from a vertex for texture alignment
		private static void AddSidedefsForAlignment(Stack<SidedefAlignJob> stack, Vertex v, bool forward, float offsetx, long texturelongname)
		{
			foreach(Linedef ld in v.Linedefs)
			{
				Sidedef side1 = forward ? ld.Front : ld.Back;
				Sidedef side2 = forward ? ld.Back : ld.Front;
				if((ld.Start == v) && (side1 != null) && !side1.Marked)
				{
					if(SidedefTextureMatch(side1, texturelongname))
					{
						SidedefAlignJob nj = new SidedefAlignJob();
						nj.forward = forward;
						nj.offsetx = offsetx;
						nj.sidedef = side1;
						stack.Push(nj);
					}
				}
				else if((ld.End == v) && (side2 != null) && !side2.Marked)
				{
					if(SidedefTextureMatch(side2, texturelongname))
					{
						SidedefAlignJob nj = new SidedefAlignJob();
						nj.forward = forward;
						nj.offsetx = offsetx;
						nj.sidedef = side2;
						stack.Push(nj);
					}
				}
			}
		}

		// This checks if any of the sidedef texture match the given texture
		private static bool SidedefTextureMatch(Sidedef sd, long texturelongname)
		{
			return ((sd.LongHighTexture == texturelongname) && sd.HighRequired()) ||
				   ((sd.LongLowTexture == texturelongname) && sd.LowRequired()) ||
				   ((sd.LongMiddleTexture == texturelongname) && (sd.MiddleRequired() || ((sd.MiddleTexture.Length > 0) && (sd.MiddleTexture[0] != '-'))));
		}

		#endregion
	}
}
