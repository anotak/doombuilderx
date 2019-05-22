
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
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{

	/* This renders a 2D presentation of the map. This is done in several
	 * layers which each are optimized for a different purpose. Set the
	 * PresentationLayer(s) to specify how to present these layers.
	 */

	internal unsafe sealed class Renderer2D : Renderer, IRenderer2D
	{
		#region ================== Constants

		private const float FSAA_FACTOR = 0.6f;
		private const float THING_ARROW_SIZE = 1.5f;
		private const float THING_ARROW_SHRINK = 2f;
		private const float THING_CIRCLE_SIZE = 1f;
		private const float THING_CIRCLE_SHRINK = 0f;
		private const int THING_BUFFER_SIZE = 100;
		private const float THINGS_BACK_ALPHA = 0.3f;

		private const string FONT_NAME = "Verdana";
		private const int FONT_WIDTH = 0;
		private const int FONT_HEIGHT = 0;

		private const int THING_SHINY = 1;
		private const int THING_SQUARE = 2;
		private const int NUM_THING_TEXTURES = 4;
		internal const int NUM_VIEW_MODES = 4;
		
		#endregion

		#region ================== Variables

		// Rendertargets
		private Texture backtex;
		private Texture plottertex;
		private Texture thingstex;
		private Texture overlaytex;
		private Texture surfacetex;

		// Locking data
		private DataRectangle plotlocked;
		private Surface targetsurface;

		// Rendertarget sizes
		private Size windowsize;
		private Size structsize;
		private Size thingssize;
		private Size overlaysize;
		private Size backsize;
		
		// Font
		private SlimDX.Direct3D9.Font font;

		// Geometry plotter
		private Plotter plotter;

		// Vertices to present the textures
		private VertexBuffer screenverts;
		private FlatVertex[] backimageverts;
		
		// Batch buffer for things rendering
		private VertexBuffer thingsvertices;
		
		// Render settings
		private int vertexsize;
		private RenderLayers renderlayer = RenderLayers.None;
		
		// Surfaces
		private SurfaceManager surfaces;
		
		// Images
		private ResourceImage[] thingtexture;
		
		// View settings (world coordinates)
		private ViewMode viewmode;
		private float scale;
		private float scaleinv;
		private float offsetx;
		private float offsety;
		private float translatex;
		private float translatey;
		private float linenormalsize;
		private float lastgridscale = -1f;
		private int lastgridsize;
		private float lastgridx;
		private float lastgridy;
        private float lastgridrotate;
        private float lastgridoriginx;
        private float lastgridoriginy;
		private RectangleF viewport;
		private RectangleF yviewport;

		// Presentation
		private Presentation present;
		
		#endregion

		#region ================== Properties

		public float OffsetX { get { return offsetx; } }
		public float OffsetY { get { return offsety; } }
		public float TranslateX { get { return translatex; } }
		public float TranslateY { get { return translatey; } }
		public float Scale { get { return scale; } }
		public int VertexSize { get { return vertexsize; } }
		public ViewMode ViewMode { get { return viewmode; } }
		public SurfaceManager Surfaces { get { return surfaces; } }
        // ano - gzdb plugin cross compat
        public bool DrawMapCenter { get { return false; } set { } } 

        #endregion

        #region ================== Constructor / Disposer

        // Constructor
        internal Renderer2D(D3DDevice graphics) : base(graphics)
		{
			// Load thing textures
			thingtexture = new ResourceImage[NUM_THING_TEXTURES];
			for(int i = 0; i < NUM_THING_TEXTURES; i++)
			{
				thingtexture[i] = new ResourceImage("CodeImp.DoomBuilder.Resources.Thing2D_" + i.ToString(CultureInfo.InvariantCulture) + ".png");
				thingtexture[i].UseColorCorrection = false;
				thingtexture[i].LoadImage();
				thingtexture[i].CreateTexture();
			}

			// Create surface manager
			surfaces = new SurfaceManager();

			// Create rendertargets
			CreateRendertargets();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		internal override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Destroy rendertargets
				DestroyRendertargets();
				foreach(ResourceImage i in thingtexture) i.Dispose();
				
				// Dispose surface manager
				surfaces.Dispose();
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Presenting

		// This sets the presentation to use
		public void SetPresentation(Presentation present)
		{
			this.present = new Presentation(present);
		}
		
		// This draws the image on screen
		public void Present()
		{
			General.Plugins.OnPresentDisplayBegin();
			
			// Start drawing
			if(graphics.StartRendering(true, General.Colors.Background.ToColorValue(), graphics.BackBuffer, graphics.DepthBuffer))
			{
				// Renderstates that count for this whole sequence
				graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
				graphics.Device.SetRenderState(RenderState.ZEnable, false);
				graphics.Device.SetRenderState(RenderState.FogEnable, false);
				graphics.Device.SetStreamSource(0, screenverts, 0, sizeof(FlatVertex));
				graphics.Device.SetTransform(TransformState.World, Matrix.Identity);
				graphics.Shaders.Display2D.Begin();

				// Go for all layers
				foreach(PresentLayer layer in present.layers)
				{
					int aapass;

					// Set blending mode
					switch(layer.blending)
					{
						case BlendingMode.None:
							graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
							graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
							graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
							break;

						case BlendingMode.Mask:
							graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
							graphics.Device.SetRenderState(RenderState.AlphaTestEnable, true);
							graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
							break;

						case BlendingMode.Alpha:
							graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
							graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
							graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
							graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
							graphics.Device.SetRenderState(RenderState.TextureFactor, (new Color4(layer.alpha, 1f, 1f, 1f)).ToArgb());
							break;

						case BlendingMode.Additive:
							graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
							graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
							graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
							graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
							graphics.Device.SetRenderState(RenderState.TextureFactor, (new Color4(layer.alpha, 1f, 1f, 1f)).ToArgb());
							break;
					}

					// Check which pass to use
					if(layer.antialiasing && General.Settings.QualityDisplay) aapass = 0; else aapass = 1;

					// Render layer
					switch(layer.layer)
					{
						// BACKGROUND
						case RendererLayer.Background:
							if((backimageverts == null) || (General.Map.Grid.Background.Texture == null)) break;
							graphics.Device.SetTexture(0, General.Map.Grid.Background.Texture);
							graphics.Shaders.Display2D.Texture1 = General.Map.Grid.Background.Texture;
							graphics.Shaders.Display2D.SetSettings(1f / windowsize.Width, 1f / windowsize.Height, FSAA_FACTOR, layer.alpha, false);
							graphics.Shaders.Display2D.BeginPass(aapass);
							graphics.Device.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleStrip, 0, 2, backimageverts);
							graphics.Shaders.Display2D.EndPass();
							graphics.Device.SetStreamSource(0, screenverts, 0, sizeof(FlatVertex));
							break;

						// GRID
						case RendererLayer.Grid:
							graphics.Device.SetTexture(0, backtex);
							graphics.Shaders.Display2D.Texture1 = backtex;
							graphics.Shaders.Display2D.SetSettings(1f / backsize.Width, 1f / backsize.Height, FSAA_FACTOR, layer.alpha, false);
							graphics.Shaders.Display2D.BeginPass(aapass);
							graphics.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
							graphics.Shaders.Display2D.EndPass();
							break;

						// GEOMETRY
						case RendererLayer.Geometry:
							graphics.Device.SetTexture(0, plottertex);
							graphics.Shaders.Display2D.Texture1 = plottertex;
							graphics.Shaders.Display2D.SetSettings(1f / structsize.Width, 1f / structsize.Height, FSAA_FACTOR, layer.alpha, false);
							graphics.Shaders.Display2D.BeginPass(aapass);
							graphics.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
							graphics.Shaders.Display2D.EndPass();
							break;

						// THINGS
						case RendererLayer.Things:
							graphics.Device.SetTexture(0, thingstex);
							graphics.Shaders.Display2D.Texture1 = thingstex;
							graphics.Shaders.Display2D.SetSettings(1f / thingssize.Width, 1f / thingssize.Height, FSAA_FACTOR, layer.alpha, false);
							graphics.Shaders.Display2D.BeginPass(aapass);
							graphics.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
							graphics.Shaders.Display2D.EndPass();
							break;

						// OVERLAY
						case RendererLayer.Overlay:
							graphics.Device.SetTexture(0, overlaytex);
							graphics.Shaders.Display2D.Texture1 = overlaytex;
							graphics.Shaders.Display2D.SetSettings(1f / overlaysize.Width, 1f / overlaysize.Height, FSAA_FACTOR, layer.alpha, false);
							graphics.Shaders.Display2D.BeginPass(aapass);
							graphics.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
							graphics.Shaders.Display2D.EndPass();
							break;

						// SURFACE
						case RendererLayer.Surface:
							graphics.Device.SetTexture(0, surfacetex);
							graphics.Shaders.Display2D.Texture1 = surfacetex;
							graphics.Shaders.Display2D.SetSettings(1f / overlaysize.Width, 1f / overlaysize.Height, FSAA_FACTOR, layer.alpha, false);
							graphics.Shaders.Display2D.BeginPass(aapass);
							graphics.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
							graphics.Shaders.Display2D.EndPass();
							break;
					}
				}

				// Done
				graphics.Shaders.Display2D.End();
				graphics.FinishRendering();
				graphics.Present();

				// Release binds
				graphics.Device.SetTexture(0, null);
				graphics.Shaders.Display2D.Texture1 = null;
				graphics.Device.SetStreamSource(0, null, 0, 0);
			}
			else
			{
				// Request delayed redraw
				General.MainWindow.DelayedRedraw();
			}
		}
		
		#endregion

		#region ================== Management

		// This is called before a device is reset
		// (when resized or display adapter was changed)
		public override void UnloadResource()
		{
			// Destroy rendertargets
			DestroyRendertargets();
		}
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public override void ReloadResource()
		{
			// Re-create rendertargets
			CreateRendertargets();
		}

		// This resets the graphics
		public override void Reset()
		{
			UnloadResource();
			ReloadResource();
		}

		// This destroys the rendertargets
		public void DestroyRendertargets()
		{
			// Trash rendertargets
			if(plottertex != null) plottertex.Dispose();
			if(thingstex != null) thingstex.Dispose();
			if(overlaytex != null) overlaytex.Dispose();
			if(surfacetex != null) surfacetex.Dispose();
			if(backtex != null) backtex.Dispose();
			if(screenverts != null) screenverts.Dispose();
			plottertex = null;
			thingstex = null;
			backtex = null;
			screenverts = null;
			overlaytex = null;
			surfacetex = null;
			
			// Trash things batch buffer
			if(thingsvertices != null) thingsvertices.Dispose();
			thingsvertices = null;
			lastgridscale = -1f;
			lastgridsize = 0;
            lastgridoriginx = 0;
            lastgridoriginy = 0;
            lastgridrotate = 0;

			// Trash font
			if(font != null) font.Dispose();
			font = null;
		}
		
		// Allocates new image memory to render on
		public void CreateRendertargets()
		{
			SurfaceDescription sd;
			DataStream stream;
			FlatVertex[] verts;
			
			// Destroy rendertargets
			DestroyRendertargets();
			
			// Get new width and height
			windowsize.Width = graphics.RenderTarget.ClientSize.Width;
			windowsize.Height = graphics.RenderTarget.ClientSize.Height;

			// Create rendertargets textures
			plottertex = new Texture(graphics.Device, windowsize.Width, windowsize.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
			thingstex = new Texture(graphics.Device, windowsize.Width, windowsize.Height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
			backtex = new Texture(graphics.Device, windowsize.Width, windowsize.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
			overlaytex = new Texture(graphics.Device, windowsize.Width, windowsize.Height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
			surfacetex = new Texture(graphics.Device, windowsize.Width, windowsize.Height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
			
			// Get the real surface sizes
			sd = plottertex.GetLevelDescription(0);
			structsize.Width = sd.Width;
			structsize.Height = sd.Height;
			sd = thingstex.GetLevelDescription(0);
			thingssize.Width = sd.Width;
			thingssize.Height = sd.Height;
			sd = backtex.GetLevelDescription(0);
			backsize.Width = sd.Width;
			backsize.Height = sd.Height;
			sd = overlaytex.GetLevelDescription(0);
			overlaysize.Width = sd.Width;
			overlaysize.Height = sd.Height;
			
			// Clear rendertargets
			// This may cause a crash when resetting because it recursively
			// calls Reset in the Start functions and doesn't get to Finish
			//StartPlotter(true); Finish();
			//StartThings(true); Finish();
			//StartOverlay(true); Finish();
			graphics.ClearRendertarget(General.Colors.Background.WithAlpha(0).ToColorValue(), thingstex.GetSurfaceLevel(0), null);
			graphics.ClearRendertarget(General.Colors.Background.WithAlpha(0).ToColorValue(), overlaytex.GetSurfaceLevel(0), null);
			
			// Create font
			font = new SlimDX.Direct3D9.Font(graphics.Device, FONT_WIDTH, FONT_HEIGHT, FontWeight.Bold, 1, false, CharacterSet.Ansi, Precision.Default, FontQuality.Antialiased, PitchAndFamily.Default, FONT_NAME);
			
			// Create vertex buffers
			screenverts = new VertexBuffer(graphics.Device, 4 * sizeof(FlatVertex), Usage.Dynamic | Usage.WriteOnly, VertexFormat.None, Pool.Default);
			thingsvertices = new VertexBuffer(graphics.Device, THING_BUFFER_SIZE * 12 * sizeof(FlatVertex), Usage.Dynamic | Usage.WriteOnly, VertexFormat.None, Pool.Default);

			// Make screen vertices
			stream = screenverts.Lock(0, 4 * sizeof(FlatVertex), LockFlags.Discard | LockFlags.NoSystemLock);
			verts = CreateScreenVerts(structsize);
			stream.WriteRange<FlatVertex>(verts);
			screenverts.Unlock();
			stream.Dispose();
			
			// Force update of view
			lastgridscale = -1f;
			lastgridsize = 0;
			lastgridx = 0.0f;
			lastgridy = 0.0f;
		    lastgridoriginx = 0;
		    lastgridoriginy = 0;
		    lastgridrotate = 0;
			UpdateTransformations();
		}

		// This makes screen vertices for display
		private FlatVertex[] CreateScreenVerts(Size texturesize)
		{
			FlatVertex[] screenverts = new FlatVertex[4];
			screenverts[0].x = 0.5f;
			screenverts[0].y = 0.5f;
			screenverts[0].c = -1;
			screenverts[0].u = 1f / texturesize.Width;
			screenverts[0].v = 1f / texturesize.Height;
			screenverts[1].x = texturesize.Width - 1.5f;
			screenverts[1].y = 0.5f;
			screenverts[1].c = -1;
			screenverts[1].u = 1f - 1f / texturesize.Width;
			screenverts[1].v = 1f / texturesize.Height;
			screenverts[2].x = 0.5f;
			screenverts[2].y = texturesize.Height - 1.5f;
			screenverts[2].c = -1;
			screenverts[2].u = 1f / texturesize.Width;
			screenverts[2].v = 1f - 1f / texturesize.Height;
			screenverts[3].x = texturesize.Width - 1.5f;
			screenverts[3].y = texturesize.Height - 1.5f;
			screenverts[3].c = -1;
			screenverts[3].u = 1f - 1f / texturesize.Width;
			screenverts[3].v = 1f - 1f / texturesize.Height;
			return screenverts;
		}

		#endregion
		
		#region ================== View

		// This changes view mode
		public void SetViewMode(ViewMode mode)
		{
			viewmode = mode;
		}
		
		// This changes view position
		public void PositionView(float x, float y)
		{
			// Change position in world coordinates
			offsetx = x;
			offsety = y;
			UpdateTransformations();
		}
		
		// This changes zoom
		public void ScaleView(float scale)
		{
			// Change zoom scale
			this.scale = scale;
			UpdateTransformations();
			
			// Show zoom on main window
			General.MainWindow.UpdateZoom(scale);
		}

		// This updates some maths
		private void UpdateTransformations()
		{
			scaleinv = 1f / scale;
			translatex = -offsetx + (windowsize.Width * 0.5f) * scaleinv;
			translatey = -offsety - (windowsize.Height * 0.5f) * scaleinv;
			linenormalsize = 10f * scaleinv;
			vertexsize = (int)(1.7f * scale + 0.5f);
			if(vertexsize < 0) vertexsize = 0;
			if(vertexsize > 4) vertexsize = 4;
			Matrix scaling = Matrix.Scaling((1f / (float)windowsize.Width) * 2f, (1f / (float)windowsize.Height) * -2f, 1f);
			Matrix translate = Matrix.Translation(-(float)windowsize.Width * 0.5f, -(float)windowsize.Height * 0.5f, 0f);
			graphics.Device.SetTransform(TransformState.View, Matrix.Multiply(translate, scaling));
			graphics.Device.SetTransform(TransformState.Projection, Matrix.Identity);
			Vector2D lt = DisplayToMap(new Vector2D(0.0f, 0.0f));
			Vector2D rb = DisplayToMap(new Vector2D((float)windowsize.Width, (float)windowsize.Height));
			viewport = new RectangleF(lt.x, lt.y, rb.x - lt.x, rb.y - lt.y);
			yviewport = new RectangleF(lt.x, rb.y, rb.x - lt.x, lt.y - rb.y);
		}

		// This sets the world matrix for transformation
		private void SetWorldTransformation(bool transform)
		{
			if(transform)
			{
				Matrix translate = Matrix.Translation(translatex, translatey, 0f);
				Matrix scaling = Matrix.Scaling(scale, -scale, 1f);
				graphics.Device.SetTransform(TransformState.World, Matrix.Multiply(translate, scaling));
			}
			else
			{
				graphics.Device.SetTransform(TransformState.World, Matrix.Identity);
			}
		}
		
		/// <summary>
		/// This unprojects display coordinates (screen space) to map coordinates
		/// </summary>
		public Vector2D DisplayToMap(Vector2D mousepos)
		{
			return mousepos.GetInvTransformed(-translatex, -translatey, scaleinv, -scaleinv);
		}
		
		/// <summary>
		/// This projects map coordinates to display coordinates (screen space)
		/// </summary>
		public Vector2D MapToDisplay(Vector2D mappos)
		{
			return mappos.GetTransformed(translatex, translatey, scale, -scale);
		}
		
		#endregion

		#region ================== Colors

		// This returns the color for a thing
		public PixelColor DetermineThingColor(Thing t)
		{
			// Determine color
			if(t.Selected) return General.Colors.Selection;
			else return t.Color;
		}

		// This returns the color for a vertex
		public int DetermineVertexColor(Vertex v)
		{
			// Determine color
			if(v.Selected) return ColorCollection.SELECTION;
			else return ColorCollection.VERTICES;
		}

		// This returns the color for a linedef
		public PixelColor DetermineLinedefColor(Linedef l)
		{
			if(l.Selected)
				return General.Colors.Selection;
			else if(l.ImpassableFlag)
			{
				// Impassable lines
				if(l.Action != 0) return General.Colors.Actions;
				else return General.Colors.Linedefs;
			}
			else
			{
				// Passable lines
				if(l.Action != 0) return General.Colors.Actions.WithAlpha(General.Settings.DoubleSidedAlphaByte);
				else if(l.BlockSoundFlag) return General.Colors.Sounds.WithAlpha(General.Settings.DoubleSidedAlphaByte);
				else return General.Colors.Linedefs.WithAlpha(General.Settings.DoubleSidedAlphaByte);
			}
		}

		#endregion

		#region ================== Start / Finish

		// This begins a drawing session
		public unsafe bool StartPlotter(bool clear)
		{
			if(renderlayer != RenderLayers.None) throw new InvalidOperationException("Renderer starting called before finished previous layer. Call Finish() first!");
			renderlayer = RenderLayers.Plotter;
			try { graphics.Device.SetRenderState(RenderState.FogEnable, false); } catch(Exception) { }
			
			// Rendertargets available?
			if(plottertex != null)
			{
				// Lock structures rendertarget memory
				plotlocked = plottertex.LockRectangle(0, LockFlags.NoSystemLock);

				// Create structures plotter
				plotter = new Plotter((PixelColor*)plotlocked.Data.DataPointer.ToPointer(), plotlocked.Pitch / sizeof(PixelColor), structsize.Height, structsize.Width, structsize.Height);
				if(clear) plotter.Clear();

				// Redraw grid when structures image was cleared
				if(clear)
				{
					RenderBackgroundGrid();
					SetupBackground();
				}
				
				// Ready for rendering
				UpdateTransformations();
				return true;
			}
			else
			{
				// Can't render!
				Finish();
				return false;
			}
		}

		// This begins a drawing session
		public unsafe bool StartThings(bool clear)
		{
			if(renderlayer != RenderLayers.None) throw new InvalidOperationException("Renderer starting called before finished previous layer. Call Finish() first!");
			renderlayer = RenderLayers.Things;
			try { graphics.Device.SetRenderState(RenderState.FogEnable, false); } catch(Exception) { }
			
			// Rendertargets available?
			if(thingstex != null)
			{
				// Set the rendertarget to the things texture
				targetsurface = thingstex.GetSurfaceLevel(0);
				if(graphics.StartRendering(clear, General.Colors.Background.WithAlpha(0).ToColorValue(), targetsurface, null))
				{
					// Ready for rendering
					UpdateTransformations();
					return true;
				}
				else
				{
					// Can't render!
					Finish();
					return false;
				}
			}
			else
			{
				// Can't render!
				Finish();
				return false;
			}
		}

		// This begins a drawing session
		public unsafe bool StartOverlay(bool clear)
		{
			if(renderlayer != RenderLayers.None) throw new InvalidOperationException("Renderer starting called before finished previous layer. Call Finish() first!");
			renderlayer = RenderLayers.Overlay;
			try { graphics.Device.SetRenderState(RenderState.FogEnable, false); } catch(Exception) { }
			
			// Rendertargets available?
			if(overlaytex != null)
			{
				// Set the rendertarget to the things texture
				targetsurface = overlaytex.GetSurfaceLevel(0);
				if(graphics.StartRendering(clear, General.Colors.Background.WithAlpha(0).ToColorValue(), targetsurface, null))
				{
					// Ready for rendering
					UpdateTransformations();
					return true;
				}
				else
				{
					// Can't render!
					Finish();
					return false;
				}
			}
			else
			{
				// Can't render!
				Finish();
				return false;
			}
		}

		// This ends a drawing session
		public void Finish()
		{
			// Clean up plotter
			if(renderlayer == RenderLayers.Plotter)
			{
				if(plottertex != null) plottertex.UnlockRectangle(0);
				if(plotlocked.Data != null) plotlocked.Data.Dispose();
				plotter = null;
			}
			
			// Clean up things / overlay
			if((renderlayer == RenderLayers.Things) || (renderlayer == RenderLayers.Overlay) || (renderlayer == RenderLayers.Surface))
			{
				// Stop rendering
				graphics.FinishRendering();
				
				// Release rendertarget
				try
				{
					graphics.Device.DepthStencilSurface = graphics.DepthBuffer;
					graphics.Device.SetRenderTarget(0, graphics.BackBuffer);
				}
				catch(Exception) { }
				if(targetsurface != null) targetsurface.Dispose();
				targetsurface = null;
			}
			
			// Done
			renderlayer = RenderLayers.None;
		}

		#endregion

		#region ================== Background

		// This sets up background image vertices
		private void SetupBackground()
		{
			Vector2D ltpos, rbpos;
			Vector2D backoffset = new Vector2D((float)General.Map.Grid.BackgroundX, (float)General.Map.Grid.BackgroundY);
			Vector2D backimagesize = new Vector2D((float)General.Map.Grid.Background.ScaledWidth, (float)General.Map.Grid.Background.ScaledHeight);
			Vector2D backimagescale = new Vector2D(General.Map.Grid.BackgroundScaleX, General.Map.Grid.BackgroundScaleY);
			
			// Scale the background image size
			backimagesize *= backimagescale;
			
			// Only if a background image is set
			if((General.Map.Grid.Background != null) &&
			   !(General.Map.Grid.Background is UnknownImage))
			{
				// Make vertices
				backimageverts = CreateScreenVerts(windowsize);

				// Determine map coordinates for view window
				ltpos = DisplayToMap(new Vector2D(0f, 0f));
				rbpos = DisplayToMap(new Vector2D(windowsize.Width, windowsize.Height));
				
				// Offset by given background offset
				ltpos -= backoffset;
				rbpos -= backoffset;
				
				// Calculate UV coordinates
				// NOTE: backimagesize.y is made negative to match Doom's coordinate system
				backimageverts[0].u = ltpos.x / backimagesize.x;
				backimageverts[0].v = ltpos.y / -backimagesize.y;
				backimageverts[1].u = rbpos.x / backimagesize.x;
				backimageverts[1].v = ltpos.y / -backimagesize.y;
				backimageverts[2].u = ltpos.x / backimagesize.x;
				backimageverts[2].v = rbpos.y / -backimagesize.y;
				backimageverts[3].u = rbpos.x / backimagesize.x;
				backimageverts[3].v = rbpos.y / -backimagesize.y;
			}
			else
			{
				// No background image
				backimageverts = null;
			}
		}

		// This renders all grid
		private void RenderBackgroundGrid()
		{
			Plotter gridplotter;
			DataRectangle lockedrect;
			
			// Do we need to redraw grid?
			if((lastgridsize != General.Map.Grid.GridSize) || (lastgridscale != scale) ||
			   (lastgridx != offsetx) || (lastgridy != offsety) || (lastgridrotate != General.Map.Grid.GridRotate) || 
			   (lastgridoriginx != General.Map.Grid.GridOriginX) || (lastgridoriginy != General.Map.Grid.GridOriginY))
			{
				// Lock background rendertarget memory
				lockedrect = backtex.LockRectangle(0, LockFlags.NoSystemLock);

				// Create a plotter
				gridplotter = new Plotter((PixelColor*)lockedrect.Data.DataPointer.ToPointer(), lockedrect.Pitch / sizeof(PixelColor), backsize.Height, backsize.Width, backsize.Height);
				gridplotter.Clear();

			    bool transformed = Math.Abs(General.Map.Grid.GridOriginX) > 1e-4 || Math.Abs(General.Map.Grid.GridOriginY) > 1e-4 || Math.Abs(General.Map.Grid.GridRotate) > 1e-4;

			    if (transformed)
			    {
			        // Render normal grid
			        RenderGridTransformed(General.Map.Grid.GridSizeF, General.Map.Grid.GridRotate,
			            General.Map.Grid.GridOriginX, General.Map.Grid.GridOriginY, General.Colors.Grid, gridplotter);

			        // Render 64 grid
			        if(General.Map.Grid.GridSizeF <= 64)
			        {
			            RenderGridTransformed(64f, General.Map.Grid.GridRotate,
			                General.Map.Grid.GridOriginX, General.Map.Grid.GridOriginY, General.Colors.Grid64, gridplotter);
			        }
			    } 
			    else
			    {
			        // Render normal grid
                    RenderGrid(General.Map.Grid.GridSizeF, General.Colors.Grid, gridplotter);

                    // Render 64 grid
			        if(General.Map.Grid.GridSizeF <= 64) RenderGrid(64f, General.Colors.Grid64, gridplotter);
                }

				// Done
				backtex.UnlockRectangle(0);
				lockedrect.Data.Dispose();
				lastgridscale = scale;
				lastgridsize = General.Map.Grid.GridSize;
				lastgridx = offsetx;
				lastgridy = offsety;
			    lastgridoriginx = General.Map.Grid.GridOriginX;
			    lastgridoriginy = General.Map.Grid.GridOriginY;
			    lastgridrotate = General.Map.Grid.GridRotate;
			}
		}

        	// This renders the grid with a transform applied
		private void RenderGridTransformed(float size, float angle, float originx, float originy, PixelColor c, Plotter gridplotter)
		{
			const int mask = 0x55555555; // dotted line mask

		    // Only render grid when not screen-filling
		    if((size * scale) > 6f)
		    {
			    float sizeinv = 1f / size;

			    // Determine map coordinates for view window
			    Vector2D ltview = DisplayToMap(new Vector2D(0, 0));
			    Vector2D rbview = DisplayToMap(new Vector2D(windowsize.Width, windowsize.Height));
			    Vector2D mapsize = rbview - ltview;

			    Vector2D ltbound = new Vector2D(General.Map.Config.LeftBoundary, General.Map.Config.TopBoundary);
			    Vector2D rbbound = new Vector2D(General.Map.Config.RightBoundary, General.Map.Config.BottomBoundary);

			    // Translate top left boundary and right bottom boundary of map to screen coords
			    Vector2D tlb = ltbound.GetTransformed(translatex, translatey, scale, -scale);
			    Vector2D rbb = rbbound.GetTransformed(translatex, translatey, scale, -scale);

			    Vector2D center = GridSetup.SnappedToGrid(0.5f * (ltview + rbview), size, sizeinv, angle, originx, originy);

			    // Get the angle vectors for the gridlines
			    Vector2D dx = new Vector2D((float)Math.Cos(angle), (float)Math.Sin(angle));
			    Vector2D dy = new Vector2D((float)-Math.Sin(angle), (float)Math.Cos(angle));

			    float maxextent = Math.Max(mapsize.x, mapsize.y);
			    RectangleF bounds = new RectangleF(tlb.x, tlb.y, rbb.x - tlb.x, rbb.y - tlb.y);
			    bounds.Intersect(new RectangleF(0, 0, windowsize.Width, windowsize.Height));

			    bool xminintersect = true, xmaxintersect = true, yminintersect = true, ymaxintersect = true;

			    int num = 0;            
			    while (xminintersect || xmaxintersect || yminintersect || ymaxintersect) {
				    Vector2D xminstart = center - num * size * dy;
				    Vector2D xmaxstart = center + num * size * dy;
				    Vector2D yminstart = center - num * size * dx;
				    Vector2D ymaxstart = center + num * size * dx;

				    Line2D xminscanline = new Line2D(xminstart - dx * maxextent, xminstart + dx * maxextent);
				    Line2D xmaxscanline = new Line2D(xmaxstart - dx * maxextent, xmaxstart + dx * maxextent);
				    Line2D yminscanline = new Line2D(yminstart - dy * maxextent, yminstart + dy * maxextent);
				    Line2D ymaxscanline = new Line2D(ymaxstart - dy * maxextent, ymaxstart + dy * maxextent);

				    Line2D xminplotline = xminscanline.GetTransformed(translatex, translatey, scale, -scale);
				    Line2D xmaxplotline = xmaxscanline.GetTransformed(translatex, translatey, scale, -scale);
				    Line2D yminplotline = yminscanline.GetTransformed(translatex, translatey, scale, -scale);
				    Line2D ymaxplotline = ymaxscanline.GetTransformed(translatex, translatey, scale, -scale);
				    xminplotline = Line2D.ClipToRectangle(xminplotline, bounds, out xminintersect);
				    xmaxplotline = Line2D.ClipToRectangle(xmaxplotline, bounds, out xmaxintersect);
				    yminplotline = Line2D.ClipToRectangle(yminplotline, bounds, out yminintersect);
				    ymaxplotline = Line2D.ClipToRectangle(ymaxplotline, bounds, out ymaxintersect);

				    if (xminintersect)
				    {
					    gridplotter.DrawLineSolid((int)xminplotline.v1.x, (int)xminplotline.v1.y, (int)xminplotline.v2.x, (int)xminplotline.v2.y, ref c, mask);
				    }
				    if (xmaxintersect)
				    {
					    gridplotter.DrawLineSolid((int)xmaxplotline.v1.x, (int)xmaxplotline.v1.y, (int)xmaxplotline.v2.x, (int)xmaxplotline.v2.y, ref c, mask);
				    }
				    if (yminintersect)
				    {
					    gridplotter.DrawLineSolid((int)yminplotline.v1.x, (int)yminplotline.v1.y, (int)yminplotline.v2.x, (int)yminplotline.v2.y, ref c, mask);
				    }
				    if (ymaxintersect)
				    {
					    gridplotter.DrawLineSolid((int)ymaxplotline.v1.x, (int)ymaxplotline.v1.y, (int)ymaxplotline.v2.x, (int)ymaxplotline.v2.y, ref c, mask);
				    }

				    num++;
			    }
            }
		}
		
		// This renders the grid
		private void RenderGrid(float size, PixelColor c, Plotter gridplotter)
		{
			Vector2D ltpos, rbpos;
			Vector2D tlb, rbb;
			Vector2D pos = new Vector2D();
			float sizeinv = 1f / size;
			float ystart, yend;
			float xstart, xend;
			float from, to;

			// Only render grid when not screen-filling
			if((size * scale) > 6f)
			{
				// Determine map coordinates for view window
				ltpos = DisplayToMap(new Vector2D(0, 0));
				rbpos = DisplayToMap(new Vector2D(windowsize.Width, windowsize.Height));

				// Clip to nearest grid
				ltpos = GridSetup.SnappedToGrid(ltpos, size, sizeinv);
				rbpos = GridSetup.SnappedToGrid(rbpos, size, sizeinv);

				// Translate top left boundary and right bottom boundary of map
				// to screen coords
				tlb = new Vector2D(General.Map.Config.LeftBoundary, General.Map.Config.TopBoundary).GetTransformed(translatex, translatey, scale, -scale);
				rbb = new Vector2D(General.Map.Config.RightBoundary, General.Map.Config.BottomBoundary).GetTransformed(translatex, translatey, scale, -scale);

				// Draw all horizontal grid lines
				ystart = rbpos.y > General.Map.Config.BottomBoundary ? rbpos.y : General.Map.Config.BottomBoundary;
				yend = ltpos.y < General.Map.Config.TopBoundary ? ltpos.y : General.Map.Config.TopBoundary;

				for (float y = ystart; y < yend + size; y += size)
				{
					if (y > General.Map.Config.TopBoundary) y = General.Map.Config.TopBoundary;
					else if (y < General.Map.Config.BottomBoundary) y = General.Map.Config.BottomBoundary;

					from = tlb.x < 0 ? 0 : tlb.x;
					to = rbb.x > windowsize.Width ? windowsize.Width : rbb.x;

					pos.y = y;
					pos = pos.GetTransformed(translatex, translatey, scale, -scale);

					// Note: I'm not using Math.Ceiling in this case, because that doesn't work right.
					gridplotter.DrawGridLineH((int)pos.y, (int)Math.Round(from + 0.49999f), (int)Math.Round(to + 0.49999f), ref c);
				}

				// Draw all vertical grid lines
				xstart = ltpos.x > General.Map.Config.LeftBoundary ? ltpos.x : General.Map.Config.LeftBoundary;
				xend = rbpos.x < General.Map.Config.RightBoundary ? rbpos.x : General.Map.Config.RightBoundary;

				for (float x = xstart; x < xend + size; x += size)
				{
					if (x > General.Map.Config.RightBoundary) x = General.Map.Config.RightBoundary;
					else if (x < General.Map.Config.LeftBoundary) x = General.Map.Config.LeftBoundary;

					from = tlb.y < 0 ? 0 : tlb.y;
					to = rbb.y > windowsize.Height ? windowsize.Height : rbb.y;

					pos.x = x;
					pos = pos.GetTransformed(translatex, translatey, scale, -scale);

					// Note: I'm not using Math.Ceiling in this case, because that doesn't work right.
					gridplotter.DrawGridLineV((int)pos.x, (int)Math.Round(from + 0.49999f), (int)Math.Round(to + 0.49999f), ref c);
				}
			}
		}

		#endregion

		#region ================== Things

		// This makes vertices for a thing
		// Returns false when not on the screen
		private bool CreateThingVerts(Thing t, ref FlatVertex[] verts, int offset, PixelColor c)
		{
			float circlesize;
			float arrowsize;
			int color;
			
			// Transform to screen coordinates
			Vector2D screenpos = ((Vector2D)t.Position).GetTransformed(translatex, translatey, scale, -scale);
			
			// Determine sizes
			if(t.FixedSize && (scale > 1.0f))
			{
				circlesize = (t.Size - THING_CIRCLE_SHRINK) * THING_CIRCLE_SIZE;
				arrowsize = (t.Size - THING_ARROW_SHRINK) * THING_ARROW_SIZE;
			}
			else
			{
				circlesize = (t.Size - THING_CIRCLE_SHRINK) * scale * THING_CIRCLE_SIZE;
				arrowsize = (t.Size - THING_ARROW_SHRINK) * scale * THING_ARROW_SIZE;
			}
			
			// Check if the thing is actually on screen
			if(((screenpos.x + circlesize) > 0.0f) && ((screenpos.x - circlesize) < (float)windowsize.Width) &&
				((screenpos.y + circlesize) > 0.0f) && ((screenpos.y - circlesize) < (float)windowsize.Height))
			{
				// Get integral color
				color = c.ToInt();

				// Setup fixed rect for circle
				verts[offset].x = screenpos.x - circlesize;
				verts[offset].y = screenpos.y - circlesize;
				verts[offset].c = color;
				verts[offset].u = 1f / 512f;
				verts[offset].v = 1f / 128f;
				offset++;
				verts[offset].x = screenpos.x + circlesize;
				verts[offset].y = screenpos.y - circlesize;
				verts[offset].c = color;
				verts[offset].u = 0.25f - 1f / 512f;
				verts[offset].v = 1f / 128f;
				offset++;
				verts[offset].x = screenpos.x - circlesize;
				verts[offset].y = screenpos.y + circlesize;
				verts[offset].c = color;
				verts[offset].u = 1f / 512f;
				verts[offset].v = 1f - 1f / 128f;
				offset++;
				verts[offset] = verts[offset - 2];
				offset++;
				verts[offset] = verts[offset - 2];
				offset++;
				verts[offset].x = screenpos.x + circlesize;
				verts[offset].y = screenpos.y + circlesize;
				verts[offset].c = color;
				verts[offset].u = 0.25f - 1f / 512f;
				verts[offset].v = 1f - 1f / 128f;
				offset++;

				float sinarrowsize = (float)Math.Sin(t.Angle + Angle2D.PI * 0.25f) * arrowsize;
				float cosarrowsize = (float)Math.Cos(t.Angle + Angle2D.PI * 0.25f) * arrowsize;
				
				// Setup rotated rect for arrow
				verts[offset].x = screenpos.x + sinarrowsize;
				verts[offset].y = screenpos.y + cosarrowsize;
				verts[offset].c = -1;
				verts[offset].u = 0.50f + t.IconOffset;
				verts[offset].v = 0f;
				offset++;
				verts[offset].x = screenpos.x - cosarrowsize;
				verts[offset].y = screenpos.y + sinarrowsize;
				verts[offset].c = -1;
				verts[offset].u = 0.75f + t.IconOffset;
				verts[offset].v = 0f;
				offset++;
				verts[offset].x = screenpos.x + cosarrowsize;
				verts[offset].y = screenpos.y - sinarrowsize;
				verts[offset].c = -1;
				verts[offset].u = 0.50f + t.IconOffset;
				verts[offset].v = 1f;
				offset++;
				verts[offset] = verts[offset - 2];
				offset++;
				verts[offset] = verts[offset - 2];
				offset++;
				verts[offset].x = screenpos.x - sinarrowsize;
				verts[offset].y = screenpos.y - cosarrowsize;
				verts[offset].c = -1;
				verts[offset].u = 0.75f + t.IconOffset;
				verts[offset].v = 1f;
				
				// Done
				return true;
			}
			else
			{
				// Not on screen
				return false;
			}
		}
		
		// This draws a set of things
		private void RenderThingsBatch(ICollection<Thing> things, float alpha, bool fixedcolor, PixelColor c)
		{
			int thingtextureindex = 0;
			PixelColor tc;
			DataStream stream;
			
			// Anything to render?
			if(things.Count > 0)
			{
				// Make alpha color
				Color4 alphacolor = new Color4(alpha, 1.0f, 1.0f, 1.0f);
				
				// Set renderstates for things rendering
				graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
				graphics.Device.SetRenderState(RenderState.ZEnable, false);
				graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
				graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
				graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
				graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
				graphics.Device.SetRenderState(RenderState.FogEnable, false);
				graphics.Device.SetRenderState(RenderState.TextureFactor, alphacolor.ToArgb());
				graphics.Device.SetStreamSource(0, thingsvertices, 0, FlatVertex.Stride);
				
				// Determine things texture to use
				if(General.Settings.QualityDisplay) thingtextureindex |= THING_SHINY;
				if(General.Settings.SquareThings) thingtextureindex |= THING_SQUARE;
				graphics.Device.SetTexture(0, thingtexture[thingtextureindex].Texture);
				graphics.Shaders.Things2D.Texture1 = thingtexture[thingtextureindex].Texture;
				SetWorldTransformation(false);
				graphics.Shaders.Things2D.SetSettings(alpha);
				
				// Begin drawing
				graphics.Shaders.Things2D.Begin();
				graphics.Shaders.Things2D.BeginPass(0);

				// Determine next lock size
				int locksize = (things.Count > THING_BUFFER_SIZE) ? THING_BUFFER_SIZE : things.Count;
				FlatVertex[] verts = new FlatVertex[THING_BUFFER_SIZE * 12];
				
				// Go for all things
				int buffercount = 0;
				int totalcount = 0;
				foreach(Thing t in things)
				{
					// Create vertices
					tc = fixedcolor ? c : DetermineThingColor(t);
					if(CreateThingVerts(t, ref verts, buffercount * 12, tc))
						buffercount++;
					
					totalcount++;
					
					// Buffer filled?
					if(buffercount == locksize)
					{
						// Write to buffer
						stream = thingsvertices.Lock(0, locksize * 12 * FlatVertex.Stride, LockFlags.Discard);
						stream.WriteRange(verts, 0, buffercount * 12);
						thingsvertices.Unlock();
						stream.Dispose();
						
						// Draw!
						graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, buffercount * 4);
						buffercount = 0;
						
						// Determine next lock size
						locksize = ((things.Count - totalcount) > THING_BUFFER_SIZE) ? THING_BUFFER_SIZE : (things.Count - totalcount);
					}
				}

				// Write to buffer
				stream = thingsvertices.Lock(0, locksize * 12 * FlatVertex.Stride, LockFlags.Discard);
				if(buffercount > 0) stream.WriteRange(verts, 0, buffercount * 12);
				thingsvertices.Unlock();
				stream.Dispose();
				
				// Draw what's still remaining
				if(buffercount > 0)
					graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, buffercount * 4);
				
				// Done
				graphics.Shaders.Things2D.EndPass();
				graphics.Shaders.Things2D.End();
			}
		}
		
		// This adds a thing in the things buffer for rendering
		public void RenderThing(Thing t, PixelColor c, float alpha)
		{
			List<Thing> things = new List<Thing>(1);
			things.Add(t);
			RenderThingsBatch(things, alpha, true, c);
		}
		
		// This adds a thing in the things buffer for rendering
		public void RenderThingSet(ICollection<Thing> things, float alpha)
		{
			RenderThingsBatch(things, alpha, false, new PixelColor());
		}
		
		#endregion

		#region ================== Surface

		// This redraws the surface
		public void RedrawSurface()
		{
			if(renderlayer != RenderLayers.None) throw new InvalidOperationException("Renderer starting called before finished previous layer. Call Finish() first!");
			renderlayer = RenderLayers.Surface;

			// Rendertargets available?
			if(surfacetex != null)
			{
				// Set the rendertarget to the surface texture
				targetsurface = surfacetex.GetSurfaceLevel(0);
				if(graphics.StartRendering(true, General.Colors.Background.WithAlpha(0).ToColorValue(), targetsurface, null))
				{
					// Make sure anything we need is loaded
					General.Map.Data.UnknownTexture3D.CreateTexture();

					// Set transformations
					UpdateTransformations();

					// Set states
					graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
					graphics.Device.SetRenderState(RenderState.ZEnable, false);
					graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
					graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
					graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
					graphics.Device.SetRenderState(RenderState.FogEnable, false);
					SetWorldTransformation(true);
					graphics.Shaders.Display2D.SetSettings(1f, 1f, 0f, 1f, General.Settings.ClassicBilinear);
					
					// Prepare for rendering
					switch(viewmode)
					{
						case ViewMode.Brightness:
							surfaces.RenderSectorBrightness(yviewport);
							surfaces.RenderSectorSurfaces(graphics);
							break;
							
						case ViewMode.FloorTextures:
							surfaces.RenderSectorFloors(yviewport);
							surfaces.RenderSectorSurfaces(graphics);
							break;
							
						case ViewMode.CeilingTextures:
							surfaces.RenderSectorCeilings(yviewport);
							surfaces.RenderSectorSurfaces(graphics);
							break;
					}
				}
			}
			
			// Done
			Finish();
		}

		#endregion

		#region ================== Overlay

		// This renders geometry
		// The geometry must be a triangle list
		public void RenderGeometry(FlatVertex[] vertices, ImageData texture, bool transformcoords)
		{
			Texture t = null;

			if(vertices.Length > 0)
			{
				if(texture != null)
				{
					// Make sure the texture is loaded
					if(!texture.IsImageLoaded) texture.LoadImage();
					if(texture.Texture == null) texture.CreateTexture();
					t = texture.Texture;
				}
				else
				{
					t = General.Map.Data.WhiteTexture.Texture;
				}

				// Set renderstates for rendering
				graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
				graphics.Device.SetRenderState(RenderState.ZEnable, false);
				graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
				graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
				graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
				graphics.Device.SetRenderState(RenderState.FogEnable, false);
				graphics.Shaders.Display2D.Texture1 = t;
				graphics.Device.SetTexture(0, t);
				SetWorldTransformation(transformcoords);
				graphics.Shaders.Display2D.SetSettings(1f, 1f, 0f, 1f, General.Settings.ClassicBilinear);
				
				// Draw
				graphics.Shaders.Display2D.Begin();
				graphics.Shaders.Display2D.BeginPass(1);
				graphics.Device.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleList, 0, vertices.Length / 3, vertices);
				graphics.Shaders.Display2D.EndPass();
				graphics.Shaders.Display2D.End();
			}
		}

        //mxd
        // ano - gzdb plug cross compat (?)
        public void RenderHighlight(FlatVertex[] vertices, int color)
        {
            try
            {
                if (vertices.Length < 3) return;

                // Set renderstates for rendering
                graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
                graphics.Device.SetRenderState(RenderState.ZEnable, false);
                graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
                graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
                graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
                graphics.Device.SetRenderState(RenderState.FogEnable, false);

                SetWorldTransformation(true);
                //graphics.Shaders.Things2D.FillColor = new Color4(color);
                graphics.Shaders.Things2D.SetSettings(1.0f);
                graphics.Shaders.Things2D.Texture1 = thingtexture[0].Texture;
                SetWorldTransformation(false);
                // Draw
                graphics.Shaders.Things2D.Begin();
                graphics.Shaders.Things2D.BeginPass(2);
                graphics.Device.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleList, 0, vertices.Length / 3, vertices);
                graphics.Shaders.Things2D.EndPass();
                graphics.Shaders.Things2D.End();
            }
            catch (Exception) { }
        }

        // This renders text
        public void RenderText(TextLabel text)
		{
			// Update the text if needed
			text.Update(translatex, translatey, scale, -scale);
			
			// Text is created?
			if(text.VertexBuffer != null)
			{
				// Set renderstates for rendering
				graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
				graphics.Device.SetRenderState(RenderState.ZEnable, false);
				graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
				graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
				graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
				graphics.Device.SetRenderState(RenderState.FogEnable, false);
				graphics.Shaders.Display2D.Texture1 = graphics.FontTexture;
				SetWorldTransformation(false);
				graphics.Shaders.Display2D.SetSettings(1f, 1f, 0f, 1f, true);
				graphics.Device.SetTexture(0, graphics.FontTexture);
				graphics.Device.SetStreamSource(0, text.VertexBuffer, 0, FlatVertex.Stride);

				// Draw
				graphics.Shaders.Display2D.Begin();
				graphics.Shaders.Display2D.BeginPass(2);
				graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, text.NumFaces >> 1);
				graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, text.NumFaces);
				graphics.Shaders.Display2D.EndPass();
				graphics.Shaders.Display2D.End();
			}
		}
		
		// This renders a rectangle with given border size and color
		public void RenderRectangle(RectangleF rect, float bordersize, PixelColor c, bool transformrect)
		{
			FlatQuad[] quads = new FlatQuad[4];
			
			/*
			 * Rectangle setup:
			 * 
			 *  --------------------------
			 *  |___________0____________|
			 *  |  |                  |  |
			 *  |  |                  |  |
			 *  |  |                  |  |
			 *  | 2|                  |3 |
			 *  |  |                  |  |
			 *  |  |                  |  |
			 *  |__|__________________|__|
			 *  |           1            |
			 *  --------------------------
			 * 
			 * Don't you just love ASCII art?
			 */
			
			// Calculate positions
			Vector2D lt = new Vector2D(rect.Left, rect.Top);
			Vector2D rb = new Vector2D(rect.Right, rect.Bottom);
			if(transformrect)
			{
				lt = lt.GetTransformed(translatex, translatey, scale, -scale);
				rb = rb.GetTransformed(translatex, translatey, scale, -scale);
			}
			
			// Make quads
			quads[0] = new FlatQuad(PrimitiveType.TriangleStrip, lt.x, lt.y, rb.x, lt.y - bordersize);
			quads[1] = new FlatQuad(PrimitiveType.TriangleStrip, lt.x, rb.y + bordersize, rb.x, rb.y);
			quads[2] = new FlatQuad(PrimitiveType.TriangleStrip, lt.x, lt.y - bordersize, lt.x + bordersize, rb.y + bordersize);
			quads[3] = new FlatQuad(PrimitiveType.TriangleStrip, rb.x - bordersize, lt.y - bordersize, rb.x, rb.y + bordersize);
			quads[0].SetColors(c.ToInt());
			quads[1].SetColors(c.ToInt());
			quads[2].SetColors(c.ToInt());
			quads[3].SetColors(c.ToInt());
			
			// Set renderstates for rendering
			graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
			graphics.Device.SetRenderState(RenderState.ZEnable, false);
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
			graphics.Device.SetRenderState(RenderState.FogEnable, false);
			SetWorldTransformation(false);
			graphics.Device.SetTexture(0, General.Map.Data.WhiteTexture.Texture);
			graphics.Shaders.Display2D.Texture1 = General.Map.Data.WhiteTexture.Texture;
			graphics.Shaders.Display2D.SetSettings(1f, 1f, 0f, 1f, General.Settings.ClassicBilinear);
			
			// Draw
			graphics.Shaders.Display2D.Begin();
			graphics.Shaders.Display2D.BeginPass(1);
			quads[0].Render(graphics);
			quads[1].Render(graphics);
			quads[2].Render(graphics);
			quads[3].Render(graphics);
			graphics.Shaders.Display2D.EndPass();
			graphics.Shaders.Display2D.End();
		}

		// This renders a filled rectangle with given color
		public void RenderRectangleFilled(RectangleF rect, PixelColor c, bool transformrect)
		{
			// Calculate positions
			Vector2D lt = new Vector2D(rect.Left, rect.Top);
			Vector2D rb = new Vector2D(rect.Right, rect.Bottom);
			if(transformrect)
			{
				lt = lt.GetTransformed(translatex, translatey, scale, -scale);
				rb = rb.GetTransformed(translatex, translatey, scale, -scale);
			}

			// Make quad
			FlatQuad quad = new FlatQuad(PrimitiveType.TriangleStrip, lt.x, lt.y, rb.x, rb.y);
			quad.SetColors(c.ToInt());
			
			// Set renderstates for rendering
			graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
			graphics.Device.SetRenderState(RenderState.ZEnable, false);
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
			graphics.Device.SetRenderState(RenderState.FogEnable, false);
			SetWorldTransformation(false);
			graphics.Device.SetTexture(0, General.Map.Data.WhiteTexture.Texture);
			graphics.Shaders.Display2D.Texture1 = General.Map.Data.WhiteTexture.Texture;
			graphics.Shaders.Display2D.SetSettings(1f, 1f, 0f, 1f, General.Settings.ClassicBilinear);

			// Draw
			graphics.Shaders.Display2D.Begin();
			graphics.Shaders.Display2D.BeginPass(1);
			quad.Render(graphics);
			graphics.Shaders.Display2D.EndPass();
			graphics.Shaders.Display2D.End();
		}

		// This renders a filled rectangle with given color
		public void RenderRectangleFilled(RectangleF rect, PixelColor c, bool transformrect, ImageData texture)
		{
			// Calculate positions
			Vector2D lt = new Vector2D(rect.Left, rect.Top);
			Vector2D rb = new Vector2D(rect.Right, rect.Bottom);
			if(transformrect)
			{
				lt = lt.GetTransformed(translatex, translatey, scale, -scale);
				rb = rb.GetTransformed(translatex, translatey, scale, -scale);
			}

			// Make quad
			FlatQuad quad = new FlatQuad(PrimitiveType.TriangleStrip, lt.x, lt.y, rb.x, rb.y);
			quad.SetColors(c.ToInt());

			// Set renderstates for rendering
			graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
			graphics.Device.SetRenderState(RenderState.ZEnable, false);
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
			graphics.Device.SetRenderState(RenderState.FogEnable, false);
			SetWorldTransformation(false);
			graphics.Device.SetTexture(0, texture.Texture);
			graphics.Shaders.Display2D.Texture1 = texture.Texture;
			graphics.Shaders.Display2D.SetSettings(1f, 1f, 0f, 1f, General.Settings.ClassicBilinear);

			// Draw
			graphics.Shaders.Display2D.Begin();
			graphics.Shaders.Display2D.BeginPass(1);
			quad.Render(graphics);
			graphics.Shaders.Display2D.EndPass();
			graphics.Shaders.Display2D.End();
		}

		// This renders a line with given color
		public void RenderLine(Vector2D start, Vector2D end, float thickness, PixelColor c, bool transformcoords)
		{
			FlatVertex[] verts = new FlatVertex[4];
			
			// Calculate positions
			if(transformcoords)
			{
				start = start.GetTransformed(translatex, translatey, scale, -scale);
				end = end.GetTransformed(translatex, translatey, scale, -scale);
			}

			// Calculate offsets
			Vector2D delta = end - start;
			Vector2D dn = delta.GetNormal() * thickness;
			
			// Make vertices
			verts[0].x = start.x - dn.x + dn.y;
			verts[0].y = start.y - dn.y - dn.x;
			verts[0].z = 0.0f;
			verts[0].c = c.ToInt();
			verts[1].x = start.x - dn.x - dn.y;
			verts[1].y = start.y - dn.y + dn.x;
			verts[1].z = 0.0f;
			verts[1].c = c.ToInt();
			verts[2].x = end.x + dn.x + dn.y;
			verts[2].y = end.y + dn.y - dn.x;
			verts[2].z = 0.0f;
			verts[2].c = c.ToInt();
			verts[3].x = end.x + dn.x - dn.y;
			verts[3].y = end.y + dn.y + dn.x;
			verts[3].z = 0.0f;
			verts[3].c = c.ToInt();
			
			// Set renderstates for rendering
			graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
			graphics.Device.SetRenderState(RenderState.ZEnable, false);
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
			graphics.Device.SetRenderState(RenderState.FogEnable, false);
			SetWorldTransformation(false);
			graphics.Device.SetTexture(0, General.Map.Data.WhiteTexture.Texture);
			graphics.Shaders.Display2D.Texture1 = General.Map.Data.WhiteTexture.Texture;
			graphics.Shaders.Display2D.SetSettings(1f, 1f, 0f, 1f, General.Settings.ClassicBilinear);

			// Draw
			graphics.Shaders.Display2D.Begin();
			graphics.Shaders.Display2D.BeginPass(0);
			graphics.Device.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleStrip, 0, 2, verts);
			graphics.Shaders.Display2D.EndPass();
			graphics.Shaders.Display2D.End();
		}

		#endregion

		#region ================== Geometry

		// This renders the linedefs of a sector with special color
		public void PlotSector(Sector s, PixelColor c)
		{
			// Go for all sides in the sector
			foreach(Sidedef sd in s.Sidedefs)
			{
				// Render this linedef
				PlotLinedef(sd.Line, c);

				// Render the two vertices on top
				PlotVertex(sd.Line.Start, DetermineVertexColor(sd.Line.Start));
				PlotVertex(sd.Line.End, DetermineVertexColor(sd.Line.End));
			}
		}

		// This renders the linedefs of a sector
		public void PlotSector(Sector s)
		{
			// Go for all sides in the sector
			foreach(Sidedef sd in s.Sidedefs)
			{
				// Render this linedef
				PlotLinedef(sd.Line, DetermineLinedefColor(sd.Line));

				// Render the two vertices on top
				PlotVertex(sd.Line.Start, DetermineVertexColor(sd.Line.Start));
				PlotVertex(sd.Line.End, DetermineVertexColor(sd.Line.End));
			}
		}

        // mxd
        public void PlotLine(Vector2D start, Vector2D end, PixelColor c, float lengthscaler)
        {
            // Transform coordinates
            Vector2D v1 = start.GetTransformed(translatex, translatey, scale, -scale);
            Vector2D v2 = end.GetTransformed(translatex, translatey, scale, -scale);

            //mxd. Should we bother?
            if ((v2 - v1).GetLengthSq() < linenormalsize * lengthscaler) return;

            // Draw line
            plotter.DrawLineSolid((int)v1.x, (int)v1.y, (int)v2.x, (int)v2.y, ref c);
        }

        // This renders a simple line
        public void PlotLine(Vector2D start, Vector2D end, PixelColor c)
		{
			// Transform coordinates
			Vector2D v1 = start.GetTransformed(translatex, translatey, scale, -scale);
			Vector2D v2 = end.GetTransformed(translatex, translatey, scale, -scale);

			// Draw line
			plotter.DrawLineSolid((int)v1.x, (int)v1.y, (int)v2.x, (int)v2.y, ref c);
		}

		
		// This renders a single linedef
		public void PlotLinedef(Linedef l, PixelColor c)
		{
			// Transform vertex coordinates
			Vector2D v1 = l.Start.Position.GetTransformed(translatex, translatey, scale, -scale);
			Vector2D v2 = l.End.Position.GetTransformed(translatex, translatey, scale, -scale);

			// Draw line
			plotter.DrawLineSolid((int)v1.x, (int)v1.y, (int)v2.x, (int)v2.y, ref c);

			// Calculate normal indicator
			float mx = (v2.x - v1.x) * 0.5f;
			float my = (v2.y - v1.y) * 0.5f;

			// Draw normal indicator
			plotter.DrawLineSolid((int)(v1.x + mx), (int)(v1.y + my),
								  (int)((v1.x + mx) - (my * l.LengthInv) * linenormalsize),
								  (int)((v1.y + my) + (mx * l.LengthInv) * linenormalsize), ref c);
		}
		
		// This renders a set of linedefs
		public void PlotLinedefSet(ICollection<Linedef> linedefs)
		{
            //double time = General.stopwatch.Elapsed.TotalMilliseconds;
            //int count = linedefs.Count;

            // Go for all linedefs
            // ano - special case for Linedef[], faster
            if (linedefs is Linedef[])
            {
                Linedef[] array = (Linedef[])linedefs;
                int count = array.Length;

                for (int i = 0; i < count; i++)
                {
                    Linedef l = array[i];
                    PixelColor color = DetermineLinedefColor(l);

                    // Transform vertex coordinates
                    Vector2D v1 = l.Start.Position.GetTransformed(translatex, translatey, scale, -scale);
                    Vector2D v2 = l.End.Position.GetTransformed(translatex, translatey, scale, -scale);

                    // Draw line
                    plotter.DrawLineSolid((int)v1.x, (int)v1.y, (int)v2.x, (int)v2.y, ref color);
                    
                    // Calculate normal indicator
                    float mx = (v2.x - v1.x) * 0.5f;
                    float my = (v2.y - v1.y) * 0.5f;

                    // Draw normal indicator
                    plotter.DrawLineSolid((int)(v1.x + mx), (int)(v1.y + my),
                                          (int)((v1.x + mx) - (my * l.LengthInv * linenormalsize)),
                                          (int)((v1.y + my) + (mx * l.LengthInv * linenormalsize)), ref color);

                }
            } else {
                foreach (Linedef l in linedefs) PlotLinedef(l, DetermineLinedefColor(l));
            }
            /*
            time = General.stopwatch.Elapsed.TotalMilliseconds - time;
            Logger.WriteLogLine("plotting " + count + " lines in " + time.ToString("0.00000") + " total or " + (time / (double)count).ToString("0.00000") + " per line");
            */
		}

		// This renders a single vertex
		public void PlotVertex(Vertex v, int colorindex)
		{
			// Transform vertex coordinates
			Vector2D nv = v.Position.GetTransformed(translatex, translatey, scale, -scale);

			// Draw pixel here
			plotter.DrawVertexSolid((int)nv.x, (int)nv.y, vertexsize, ref General.Colors.Colors[colorindex], ref General.Colors.BrightColors[colorindex], ref General.Colors.DarkColors[colorindex]);
		}

		// This renders a single vertex at specified coordinates
		public void PlotVertexAt(Vector2D v, int colorindex)
		{
			// Transform vertex coordinates
			Vector2D nv = v.GetTransformed(translatex, translatey, scale, -scale);

			// Draw pixel here
			plotter.DrawVertexSolid((int)nv.x, (int)nv.y, vertexsize, ref General.Colors.Colors[colorindex], ref General.Colors.BrightColors[colorindex], ref General.Colors.DarkColors[colorindex]);
		}
		
		// This renders a set of vertices
		public void PlotVerticesSet(ICollection<Vertex> vertices)
		{
			// Go for all vertices
			foreach(Vertex v in vertices) PlotVertex(v, DetermineVertexColor(v));
		}

		#endregion
	}
}
