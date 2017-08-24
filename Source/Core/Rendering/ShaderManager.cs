
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
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal class ShaderManager : ID3DResource
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Settings
		private string shadertechnique;
		private bool useshaders;
		
		// Shaders
		private Display2DShader display2dshader;
		private Things2DShader things2dshader;
		private World3DShader world3dshader;
		
		// Device
		private D3DDevice device;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public bool Enabled { get { return useshaders; } }
		public string ShaderTechnique { get { return shadertechnique; } }
		public Display2DShader Display2D { get { return display2dshader; } }
		public Things2DShader Things2D { get { return things2dshader; } }
		public World3DShader World3D { get { return world3dshader; } }
		public bool IsDisposed { get { return isdisposed; } }
		internal D3DDevice D3DDevice { get { return device; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ShaderManager(D3DDevice device)
		{
			// Initialize
			this.device = device;
			
			// Load
			ReloadResource();

			// Register as resource
			device.RegisterResource(this);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				UnloadResource();

				// Unregister as resource
				device.UnregisterResource(this);
				
				// Done
				device = null;
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Resources

		// Clean up resources
		public void UnloadResource()
		{
			display2dshader.Dispose();
			things2dshader.Dispose();
			world3dshader.Dispose();
		}

		// Load resources
		public void ReloadResource()
		{
			Capabilities caps;

			// Check if we can use shaders
			caps = General.Map.Graphics.Device.Capabilities;
			useshaders = (caps.PixelShaderVersion.Major >= 2);
			shadertechnique = "SM20";

			// Initialize effects
			display2dshader = new Display2DShader(this);
			things2dshader = new Things2DShader(this);
			world3dshader = new World3DShader(this);
		}
		
		#endregion
	}
}
