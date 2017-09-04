
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
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Data;
using System.IO;
using System.Diagnostics;
using CodeImp.DoomBuilder.Compilers;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public sealed class TexturesParser : ZDTextParser
	{
		#region ================== Delegates

		#endregion
		
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables

		private Dictionary<string, TextureStructure> textures;
		private Dictionary<string, TextureStructure> flats;
		private Dictionary<string, TextureStructure> sprites;

		#endregion
		
		#region ================== Properties

		public ICollection<TextureStructure> Textures { get { return textures.Values; } }
		public ICollection<TextureStructure> Flats { get { return flats.Values; } }
		public ICollection<TextureStructure> Sprites { get { return sprites.Values; } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		public TexturesParser()
		{
			// Syntax
			whitespace = "\n \t\r";
			specialtokens = ",{}\n";

			// Initialize
			textures = new Dictionary<string, TextureStructure>();
			flats = new Dictionary<string, TextureStructure>();
			sprites = new Dictionary<string, TextureStructure>();
		}
		
		#endregion

		#region ================== Parsing

		// This parses the given stream
		// Returns false on errors
		public override bool Parse(Stream stream, string sourcefilename)
		{
            if (!base.Parse(stream, sourcefilename))
            {
                return false;
            }
			
			// Continue until at the end of the stream
			while(SkipWhitespace(true))
			{
				// Read a token
				string objdeclaration = ReadToken();
				if(objdeclaration != null)
				{
					objdeclaration = objdeclaration.ToLowerInvariant();
					if(objdeclaration == "texture")
					{
						// Read texture structure
						TextureStructure tx = new TextureStructure(this, "texture");
						if(this.HasError) break;

						// if a limit for the texture name length is set make sure that it's not exceeded
						if ((General.Map.Config.MaxTextureNamelength > 0) && (tx.Name.Length > General.Map.Config.MaxTextureNamelength))
						{
							General.ErrorLogger.Add(ErrorType.Error, "Texture name \"" + tx.Name + "\" too long. Texture names must have a length of " + General.Map.Config.MaxTextureNamelength.ToString() + " characters or less");
						}
						else
						{
							// Add the texture
							textures[tx.Name] = tx;
							flats[tx.Name] = tx;
						}
					}
					else if(objdeclaration == "sprite")
					{
						// Read sprite structure
						TextureStructure tx = new TextureStructure(this, "sprite");
						if(this.HasError) break;

						// if a limit for the sprite name length is set make sure that it's not exceeded
						if ((General.Map.Config.MaxTextureNamelength > 0) && (tx.Name.Length > General.Map.Config.MaxTextureNamelength))
						{
							General.ErrorLogger.Add(ErrorType.Error, "Sprite name \"" + tx.Name + "\" too long. Sprite names must have a length of " +  General.Map.Config.MaxTextureNamelength.ToString() + " characters or less");
						}
						else
						{
							// Add the sprite
							sprites[tx.Name] = tx;
						}
					}
					else if(objdeclaration == "walltexture")
					{
						// Read walltexture structure
						TextureStructure tx = new TextureStructure(this, "walltexture");
						if(this.HasError) break;

						// if a limit for the walltexture name length is set make sure that it's not exceeded
						if((General.Map.Config.MaxTextureNamelength > 0) && (tx.Name.Length > General.Map.Config.MaxTextureNamelength))
						{
							General.ErrorLogger.Add(ErrorType.Error, "WallTexture name \"" + tx.Name + "\" too long. WallTexture names must have a length of " + General.Map.Config.MaxTextureNamelength.ToString() + " characters or less");
						}
						else
						{
							// Add the walltexture
							if(!textures.ContainsKey(tx.Name) || (textures[tx.Name].TypeName != "texture"))
								textures[tx.Name] = tx;
						}
					}
					else if(objdeclaration == "flat")
					{
						// Read flat structure
						TextureStructure tx = new TextureStructure(this, "flat");
						if(this.HasError) break;

						// if a limit for the flat name length is set make sure that it's not exceeded
						if((General.Map.Config.MaxTextureNamelength > 0) && (tx.Name.Length > General.Map.Config.MaxTextureNamelength))
						{
							General.ErrorLogger.Add(ErrorType.Error, "Flat name \"" + tx.Name + "\" too long. Flat names must have a length of " + General.Map.Config.MaxTextureNamelength.ToString() + " characters or less");
						}
						else
						{
							// Add the flat
							if(!flats.ContainsKey(tx.Name) || (flats[tx.Name].TypeName != "texture"))
								flats[tx.Name] = tx;
						}
					}
					else
					{
						// Unknown structure!
						// Best we can do now is just find the first { and then
						// follow the scopes until the matching } is found
						string token2;
						do
						{
							if(!SkipWhitespace(true)) break;
							token2 = ReadToken();
							if(token2 == null) break;
						}
						while(token2 != "{");
						int scopelevel = 1;
						do
						{
							if(!SkipWhitespace(true)) break;
							token2 = ReadToken();
							if(token2 == null) break;
							if(token2 == "{") scopelevel++;
							if(token2 == "}") scopelevel--;
						}
						while(scopelevel > 0);
					}
				}
			}
			
			// Return true when no errors occurred
			return (ErrorDescription == null);
		}
		
		#endregion
	}
}
