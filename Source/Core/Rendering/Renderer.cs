
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

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal abstract class Renderer : ID3DResource
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Graphics
		protected D3DDevice graphics;

		// Disposing
		protected bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Renderer(D3DDevice g)
		{
			// Initialize
			this.graphics = g;

			// Register as resource
			g.RegisterResource(this);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		internal virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Unregister resource
				graphics.UnregisterResource(this);
				
				// Done
				graphics = null;
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// This calculates the sector brightness level
		public int CalculateBrightness(int level)
		{
			float flevel = level;

			// Simulat doom light levels
			if((level < 192) && General.Map.Config.DoomLightLevels)
				flevel = (192.0f - (float)(192 - level) * 1.5f);
			
			byte blevel = (byte)General.Clamp((int)flevel, 0, 255);
			PixelColor c = new PixelColor(255, blevel, blevel, blevel);
			return c.ToInt();
		}

		// This is called when the graphics need to be reset
		public virtual void Reset() { }

		// For DirectX resources
		public virtual void UnloadResource() { }
		public virtual void ReloadResource() { }

		#endregion
	}
}
