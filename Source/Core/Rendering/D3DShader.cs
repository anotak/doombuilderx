
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
	internal abstract class D3DShader
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// The manager
		protected ShaderManager manager;
		
		// The effect
		protected Effect effect;

		// The vertex declaration
		protected VertexDeclaration vertexdecl;
		
		// Disposing
		protected bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public D3DShader(ShaderManager manager)
		{
			// Initialize
			this.manager = manager;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				manager = null;
				if(effect != null) effect.Dispose();
				vertexdecl.Dispose();
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// This loads an effect
		protected Effect LoadEffect(string fxfile)
		{
			Effect fx;
			string errors;
			Stream fxdata;
			
			// Return null when not using shaders
			if(!manager.Enabled) return null;
			
			// Load the resource
			fxdata = General.ThisAssembly.GetManifestResourceStream("CodeImp.DoomBuilder.Resources." + fxfile);
			fxdata.Seek(0, SeekOrigin.Begin);
			
			try
			{
				// Compile effect
				fx = Effect.FromStream(General.Map.Graphics.Device, fxdata, null, null, null, ShaderFlags.None, null, out errors);
				if(!string.IsNullOrEmpty(errors))
				{
					throw new Exception("Errors in effect file " + fxfile + ": " + errors);
				}
			}
			catch(Exception)
			{
				// Compiling failed, try with debug information
				try
				{
					// Compile effect
					fx = Effect.FromStream(General.Map.Graphics.Device, fxdata, null, null, null, ShaderFlags.Debug, null, out errors);
					if(!string.IsNullOrEmpty(errors))
					{
						throw new Exception("Errors in effect file " + fxfile + ": " + errors);
					}
				}
				catch(Exception e)
				{
					// No debug information, just crash
					throw new Exception(e.GetType().Name + " while loading effect " + fxfile + ": " + e.Message);
				}
			}
			
			fxdata.Dispose();
			
			// Set the technique to use
			fx.Technique = manager.ShaderTechnique;

			// Return result
			return fx;
		}

		// This applies the shader
		public void Begin()
		{
			// Set vertex declaration
			General.Map.Graphics.Device.VertexDeclaration = vertexdecl;

			// Set effect
			if(manager.Enabled) effect.Begin(FX.DoNotSaveState);
		}

		// This begins a pass
		public virtual void BeginPass(int index)
		{
			if(manager.Enabled) effect.BeginPass(index);
		}

		// This ends a pass
		public void EndPass()
		{
			if(manager.Enabled) effect.EndPass();
		}
		
		// This ends te shader
		public void End()
		{
			if(manager.Enabled) effect.End();
		}

		// This applies properties during a pass
		public void ApplySettings()
		{
			if(manager.Enabled) effect.CommitChanges();
		}
		
		#endregion
	}
}
