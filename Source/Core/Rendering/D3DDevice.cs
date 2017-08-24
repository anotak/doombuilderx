
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
using SlimDX.Direct3D9;
using System.ComponentModel;
using CodeImp.DoomBuilder.Geometry;
using SlimDX;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Data;

using Configuration = CodeImp.DoomBuilder.IO.Configuration;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal class D3DDevice
	{
		#region ================== Constants

		// NVPerfHUD device name
		public const string NVPERFHUD_ADAPTER = "NVPerfHUD";

		#endregion

		#region ================== Variables

		// Settings
		private int adapter;
		private Filter postfilter;
		private Filter mipgeneratefilter;
		
		// Main objects
		private static Direct3D d3d;
		private RenderTargetControl rendertarget;
		private Capabilities devicecaps;
		private Device device;
		private Viewport viewport;
		private Dictionary<ID3DResource, ID3DResource> resources;
		private ShaderManager shaders;
		private Surface backbuffer;
		private Surface depthbuffer;
		private TextFont font;
		private ResourceImage fonttexture;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		internal Device Device { get { return device; } }
		public bool IsDisposed { get { return isdisposed; } }
		internal RenderTargetControl RenderTarget { get { return rendertarget; } }
		internal Viewport Viewport { get { return viewport; } }
		internal ShaderManager Shaders { get { return shaders; } }
		internal Surface BackBuffer { get { return backbuffer; } }
		internal Surface DepthBuffer { get { return depthbuffer; } }
		internal TextFont Font { get { return font; } }
		internal Texture FontTexture { get { return fonttexture.Texture; } }
		internal Filter PostFilter { get { return postfilter; } }
		internal Filter MipGenerateFilter { get { return mipgeneratefilter; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal D3DDevice(RenderTargetControl rendertarget)
		{
			// Set render target
			this.rendertarget = rendertarget;

			// Create resources list
			resources = new Dictionary<ID3DResource, ID3DResource>();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		internal void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				foreach(ID3DResource res in resources.Values) res.UnloadResource();
				if(shaders != null) shaders.Dispose();
				rendertarget = null;
				if(backbuffer != null) backbuffer.Dispose();
				if(depthbuffer != null) depthbuffer.Dispose();
				if(device != null) device.Dispose();
				if(font != null) font.Dispose();
				if(fonttexture != null) fonttexture.Dispose();
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Renderstates

		// This completes initialization after the device has started or has been reset
		public void SetupSettings()
		{
			// Setup renderstates
			device.SetRenderState(RenderState.AlphaBlendEnable, false);
			device.SetRenderState(RenderState.AlphaBlendEnable, false);
			device.SetRenderState(RenderState.AlphaFunc, Compare.GreaterEqual);
			device.SetRenderState(RenderState.AlphaRef, 0x0000007E);
			device.SetRenderState(RenderState.AlphaTestEnable, false);
			device.SetRenderState(RenderState.Ambient, Color.White.ToArgb());
			device.SetRenderState(RenderState.AmbientMaterialSource, ColorSource.Material);
			device.SetRenderState(RenderState.AntialiasedLineEnable, false);
			device.SetRenderState(RenderState.Clipping, true);
			device.SetRenderState(RenderState.ColorVertex, false);
			device.SetRenderState(RenderState.ColorWriteEnable, ColorWriteEnable.Red | ColorWriteEnable.Green | ColorWriteEnable.Blue | ColorWriteEnable.Alpha);
			device.SetRenderState(RenderState.CullMode, Cull.None);
			device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
			device.SetRenderState(RenderState.DiffuseMaterialSource, ColorSource.Color1);
			device.SetRenderState(RenderState.DitherEnable, true);
			device.SetRenderState(RenderState.FillMode, FillMode.Solid);
			device.SetRenderState(RenderState.FogEnable, false);
			device.SetRenderState(RenderState.FogTableMode, FogMode.Linear);
			device.SetRenderState(RenderState.Lighting, false);
			device.SetRenderState(RenderState.LocalViewer, false);
			device.SetRenderState(RenderState.NormalizeNormals, false);
			device.SetRenderState(RenderState.PointSpriteEnable, false);
			device.SetRenderState(RenderState.RangeFogEnable, false);
			device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			device.SetRenderState(RenderState.SpecularEnable, false);
			device.SetRenderState(RenderState.StencilEnable, false);
			device.SetRenderState(RenderState.TextureFactor, -1);
			device.SetRenderState(RenderState.ZEnable, false);
			device.SetRenderState(RenderState.ZWriteEnable, false);
			device.PixelShader = null;
			device.VertexShader = null;

			// Matrices
			device.SetTransform(TransformState.World, Matrix.Identity);
			device.SetTransform(TransformState.View, Matrix.Identity);
			device.SetTransform(TransformState.Projection, Matrix.Identity);
			
			// Sampler settings
			if(General.Settings.ClassicBilinear)
			{
				device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
				device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
				device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Linear);
				device.SetSamplerState(0, SamplerState.MipMapLodBias, 0f);
			}
			else
			{
				device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Point);
				device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);
				device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Point);
				device.SetSamplerState(0, SamplerState.MipMapLodBias, 0f);
			}
			
			// Texture addressing
			device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Wrap);
			device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Wrap);
			device.SetSamplerState(0, SamplerState.AddressW, TextureAddress.Wrap);

			// First texture stage
			device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
			device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
			device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Diffuse);
			device.SetTextureStageState(0, TextureStage.ResultArg, TextureArgument.Current);
			device.SetTextureStageState(0, TextureStage.TexCoordIndex, 0);

			// Second texture stage
			device.SetTextureStageState(1, TextureStage.ColorOperation, TextureOperation.Modulate);
			device.SetTextureStageState(1, TextureStage.ColorArg1, TextureArgument.Current);
			device.SetTextureStageState(1, TextureStage.ColorArg2, TextureArgument.TFactor);
			device.SetTextureStageState(1, TextureStage.ResultArg, TextureArgument.Current);
			device.SetTextureStageState(1, TextureStage.TexCoordIndex, 0);

			// No more further stages
			device.SetTextureStageState(2, TextureStage.ColorOperation, TextureOperation.Disable);
			
			// First alpha stage
			device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Modulate);
			device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Texture);
			device.SetTextureStageState(0, TextureStage.AlphaArg2, TextureArgument.Diffuse);

			// Second alpha stage
			device.SetTextureStageState(1, TextureStage.AlphaOperation, TextureOperation.Modulate);
			device.SetTextureStageState(1, TextureStage.AlphaArg1, TextureArgument.Current);
			device.SetTextureStageState(1, TextureStage.AlphaArg2, TextureArgument.TFactor);
			
			// No more further stages
			device.SetTextureStageState(2, TextureStage.AlphaOperation, TextureOperation.Disable);
			
			// Setup material
			Material material = new Material();
			material.Ambient = new Color4(Color.White);
			material.Diffuse = new Color4(Color.White);
			material.Specular = new Color4(Color.White);
			device.Material = material;
			
			// Shader settings
			shaders.World3D.SetConstants(General.Settings.VisualBilinear, true, General.Settings.FilterAnisotropy);
			
			// Texture filters
			postfilter = Filter.Point;
			mipgeneratefilter = Filter.Box;
			
			// Initialize presentations
			Presentation.Initialize();
		}

		#endregion

		#region ================== Initialization
		
		// This starts up Direct3D
		public static void Startup()
		{
			d3d = new Direct3D();
		}
		
		// This terminates Direct3D
		public static void Terminate()
		{
			if(d3d != null)
			{
				d3d.Dispose();
				d3d = null;
			}
		}
		
		// This initializes the graphics
		public bool Initialize()
		{
			PresentParameters displaypp;
			DeviceType devtype;
			
			// Use default adapter
			this.adapter = 0; // Manager.Adapters.Default.Adapter;

			try
			{
				// Make present parameters
				displaypp = CreatePresentParameters(adapter);

				// Determine device type for compatability with NVPerfHUD
				if(d3d.Adapters[adapter].Details.Description.EndsWith(NVPERFHUD_ADAPTER))
					devtype = DeviceType.Reference;
				else
					devtype = DeviceType.Hardware;

				// Get the device capabilities
				devicecaps = d3d.GetDeviceCaps(adapter, devtype);

				// Check if this adapter supports TnL
				if((devicecaps.DeviceCaps & DeviceCaps.HWTransformAndLight) != 0)
				{
					// Initialize with hardware TnL
					device = new Device(d3d, adapter, devtype, rendertarget.Handle,
								CreateFlags.HardwareVertexProcessing, displaypp);
				}
				else
				{
					// Initialize with software TnL
					device = new Device(d3d, adapter, devtype, rendertarget.Handle,
								CreateFlags.SoftwareVertexProcessing, displaypp);
				}
			}
			catch(Exception)
			{
				// Failed
				MessageBox.Show(General.MainWindow, "Unable to initialize the Direct3D video device. Another application may have taken exclusive mode on this video device or the device does not support Direct3D at all.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			// Add event to cancel resize event
			//device.DeviceResizing += new CancelEventHandler(CancelResize);

			// Keep a reference to the original buffers
			backbuffer = device.GetBackBuffer(0, 0);
			depthbuffer = device.DepthStencilSurface;

			// Get the viewport
			viewport = device.Viewport;

			// Create shader manager
			shaders = new ShaderManager(this);
			
			// Font
			postfilter = Filter.Box;		// Only for the font. This will be reset in SetupSettings (see below)
			font = new TextFont();
			fonttexture = new ResourceImage("CodeImp.DoomBuilder.Resources.Font.png");
			fonttexture.LoadImage();
			fonttexture.MipMapLevels = 2;
			fonttexture.CreateTexture();
			
			// Initialize settings
			SetupSettings();
			
			// Done
			return true;
		}

		// This is to disable the automatic resize reset
		private static void CancelResize(object sender, CancelEventArgs e)
		{
			// Cancel resize event
			e.Cancel = true;
		}
		
		// This creates present parameters
		private PresentParameters CreatePresentParameters(int adapter)
		{
			PresentParameters displaypp = new PresentParameters();
			DisplayMode currentmode;
			
			// Get current display mode
			currentmode = d3d.Adapters[adapter].CurrentDisplayMode;

			// Make present parameters
			displaypp.Windowed = true;
			displaypp.SwapEffect = SwapEffect.Discard;
			displaypp.BackBufferCount = 1;
			displaypp.BackBufferFormat = currentmode.Format;
			displaypp.BackBufferWidth = rendertarget.ClientSize.Width;
			displaypp.BackBufferHeight = rendertarget.ClientSize.Height;
			displaypp.EnableAutoDepthStencil = true;
			displaypp.AutoDepthStencilFormat = Format.D16;
			displaypp.Multisample = MultisampleType.None;
			displaypp.PresentationInterval = PresentInterval.Immediate;

			// Return result
			return displaypp;
		}
		
		#endregion

		#region ================== Resetting

		// This registers a resource
		internal void RegisterResource(ID3DResource res)
		{
			// Add resource
			resources.Add(res, res);
		}

		// This unregisters a resource
		internal void UnregisterResource(ID3DResource res)
		{
			// Remove resource
			resources.Remove(res);
		}
		
		// This resets the device and returns true on success
		internal bool Reset()
		{
			PresentParameters displaypp;

			// Test the cooperative level
			Result coopresult = device.TestCooperativeLevel();
			
			// Can we reset?
			//if(coopresult.Name != "D3DERR_DEVICENOTRESET")
			{
				// Unload all Direct3D resources
				foreach(ID3DResource res in resources.Values) res.UnloadResource();

				// Lose backbuffers
				if(backbuffer != null) backbuffer.Dispose();
				if(depthbuffer != null) depthbuffer.Dispose();
				backbuffer = null;
				depthbuffer = null;

				// Make present parameters
				displaypp = CreatePresentParameters(adapter);

				try
				{
					// Reset the device
					device.Reset(displaypp);
				}
				catch(Exception)
				{
					// Failed to re-initialize
					return false;
				}

				// Keep a reference to the original buffers
				backbuffer = device.GetBackBuffer(0, 0);
				depthbuffer = device.DepthStencilSurface;

				// Get the viewport
				viewport = device.Viewport;

				// Reload all Direct3D resources
				foreach(ID3DResource res in resources.Values) res.ReloadResource();

				// Re-apply settings
				SetupSettings();
				
				// Success
				return true;
			}
			/*
			else
			{
				// Failed
				return false;
			}
			*/
		}

		#endregion
		
		#region ================== Rendering

		// This begins a drawing session
		public bool StartRendering(bool clear, Color4 backcolor, Surface target, Surface depthbuffer)
		{
			// Check if we can render
			if(CheckAvailability())
			{
				// Set rendertarget
				device.DepthStencilSurface = depthbuffer;
				device.SetRenderTarget(0, target);
				
				// Clear the screen
				if(clear)
				{
					if(depthbuffer != null)
						device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, backcolor, 1f, 0);
					else
						device.Clear(ClearFlags.Target, backcolor, 1f, 0);
				}

				// Ready to render
				device.BeginScene();
				return true;
			}
			else
			{
				// Minimized, you cannot see anything
				return false;
			}
		}

		// This clears a target
		public void ClearRendertarget(Color4 backcolor, Surface target, Surface depthbuffer)
		{
			// Set rendertarget
			device.DepthStencilSurface = depthbuffer;
			device.SetRenderTarget(0, target);

			if(depthbuffer != null)
				device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, backcolor, 1f, 0);
			else
				device.Clear(ClearFlags.Target, backcolor, 1f, 0);
		}

		// This ends a drawing session
		public void FinishRendering()
		{
			try
			{
				// Done
				device.EndScene();
			}
			// Errors are not a problem here
			catch(Exception) { }
		}

		// This presents what has been drawn
		public void Present()
		{
			try
			{
				device.Present();
			}
			// Errors are not a problem here
			catch(Exception) { }
		}
		
		// This checks if we can use the hardware at this moment
		public bool CheckAvailability()
		{
			// When minimized, the hardware is not available
			if(General.MainWindow.WindowState != FormWindowState.Minimized)
			{
				// Test the cooperative level
				Result coopresult = device.TestCooperativeLevel();

				// Check if device must be reset
				if(!coopresult.IsSuccess)
				{
					// Should we reset?
					if(coopresult.Name == "D3DERR_DEVICENOTRESET")
					{
						// Device is lost and must be reset now
						Reset();
					}

					// Impossible to render at this point
					return false;
				}
				else
				{
					// Read to go!
					return true;
				}
			}
			else
			{
				// Minimized
				return false;
			}
		}
		
		#endregion

		#region ================== Tools

		// Make a color from ARGB
		public static int ARGB(float a, float r, float g, float b)
		{
			return Color.FromArgb((int)(a * 255f), (int)(r * 255f), (int)(g * 255f), (int)(b * 255f)).ToArgb();
		}

		// Make a color from RGB
		public static int RGB(int r, int g, int b)
		{
			return Color.FromArgb(255, r, g, b).ToArgb();
		}

		// This makes a Vector3 from Vector3D
		public static Vector3 V3(Vector3D v3d)
		{
			return new Vector3(v3d.x, v3d.y, v3d.z);
		}

		// This makes a Vector3D from Vector3
		public static Vector3D V3D(Vector3 v3)
		{
			return new Vector3D(v3.X, v3.Y, v3.Z);
		}

		// This makes a Vector2 from Vector2D
		public static Vector2 V2(Vector2D v2d)
		{
			return new Vector2(v2d.x, v2d.y);
		}

		// This makes a Vector2D from Vector2
		public static Vector2D V2D(Vector2 v2)
		{
			return new Vector2D(v2.X, v2.Y);
		}

		#endregion
	}
}
