
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal abstract class DataReader
	{
		#region ================== Variables

		protected DataLocation location;
		protected bool issuspended = false;
		protected bool isdisposed = false;
		protected ResourceTextureSet textureset;

		#endregion

		#region ================== Properties

		public DataLocation Location { get { return location; } }
		public bool IsDisposed { get { return isdisposed; } }
		public bool IsSuspended { get { return issuspended; } }
		public ResourceTextureSet TextureSet { get { return textureset; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DataReader(DataLocation dl)
		{
			// Keep information
			location = dl;
			textureset = new ResourceTextureSet(GetTitle(), dl);
		}

		// Disposer
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Done
				textureset = null;
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Management

		// This returns a short name
		public abstract string GetTitle();

		// This suspends use of this resource
		public virtual void Suspend()
		{
			issuspended = true;
		}

		// This resumes use of this resource
		public virtual void Resume()
		{
			issuspended = false;
		}

        // ano - let it know loading is over, sometimes useful to clear cached stuff
        public virtual void EndLoading()
        {
        }

		#endregion

		#region ================== Palette

		// When implemented, this should find and load a PLAYPAL palette
		public virtual Playpal LoadPalette() { return null; }
		
		#endregion

		#region ================== Colormaps

		// When implemented, this loads the colormaps
		public virtual ICollection<ImageData> LoadColormaps() { return null; }

		// When implemented, this returns the colormap lump
		public virtual Stream GetColormapData(string pname) { return null; }

		#endregion

		#region ================== Textures

		// When implemented, this should read the patch names
		public virtual PatchNames LoadPatchNames() { return null; }

		// When implemented, this returns the patch lump
		public virtual Stream GetPatchData(string pname) { return null; }

		// When implemented, this returns the texture lump
		public virtual Stream GetTextureData(string pname) { return null; }

		// When implemented, this loads the textures
		public virtual ICollection<ImageData> LoadTextures(PatchNames pnames) { return null; }
		
		#endregion

		#region ================== Flats
		
		// When implemented, this loads the flats
		public virtual ICollection<ImageData> LoadFlats() { return null; }

		// When implemented, this returns the flat lump
		public virtual Stream GetFlatData(string pname) { return null; }
		
		#endregion
		
		#region ================== Sprites

		// When implemented, this loads the sprites
		public virtual ICollection<ImageData> LoadSprites() { return null; }
		
		// When implemented, this returns the sprite lump
		public virtual Stream GetSpriteData(string pname) { return null; }

		// When implemented, this checks if the given sprite lump exists
		public virtual bool GetSpriteExists(string pname) { return false; }
		
		#endregion

		#region ================== Decorate

		// When implemented, this returns the decorate lump
		public virtual List<Stream> GetDecorateData(string pname) { return new List<Stream>(); }

		#endregion
	}
}
