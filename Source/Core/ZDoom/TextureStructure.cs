
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
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public sealed class TextureStructure
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Declaration
		private string typename;
		private string name;
		private int width;
		private int height;
		
		// Properties
		private float xscale;
		private float yscale;
		private int xoffset;
		private int yoffset;
		private bool worldpanning;
		
		// Patches
		private List<PatchStructure> patches;
		
		#endregion

		#region ================== Properties

		public string TypeName { get { return typename; } }
		public string Name { get { return name; } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public float XScale { get { return xscale; } }
		public float YScale { get { return yscale; } }
		public int XOffset { get { return xoffset; } }
		public int YOffset { get { return yoffset; } }
		public bool WorldPanning { get { return worldpanning; } }
		public ICollection<PatchStructure> Patches { get { return patches; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal TextureStructure(TexturesParser parser, string typename)
		{
			string tokenstr;
			
			// Initialize
			this.typename = typename;
			patches = new List<PatchStructure>(4);
			xscale = 0.0f;
			yscale = 0.0f;
			
			// There should be 3 tokens separated by 2 commas now:
			// Name, Width, Height

			// First token is the class name
			parser.SkipWhitespace(true);
			name = parser.StripTokenQuotes(parser.ReadToken());
			if(string.IsNullOrEmpty(name))
			{
				parser.ReportError("Expected texture or sprite name");
				return;
			}

			// Now we should find a comma
			parser.SkipWhitespace(true);
			tokenstr = parser.ReadToken();
			if(tokenstr != ",")
			{
				parser.ReportError("Expected a comma");
				return;
			}

			// Next is the texture width
			parser.SkipWhitespace(true);
			tokenstr = parser.ReadToken();
			if(string.IsNullOrEmpty(tokenstr) || !int.TryParse(tokenstr, NumberStyles.Integer, CultureInfo.InvariantCulture, out width))
			{
				parser.ReportError("Expected width in pixels");
				return;
			}

			// Now we should find a comma again
			parser.SkipWhitespace(true);
			tokenstr = parser.ReadToken();
			if(tokenstr != ",")
			{
				parser.ReportError("Expected a comma");
				return;
			}

			// Next is the texture height
			parser.SkipWhitespace(true);
			tokenstr = parser.ReadToken();
			if(string.IsNullOrEmpty(tokenstr) || !int.TryParse(tokenstr, NumberStyles.Integer, CultureInfo.InvariantCulture, out height))
			{
				parser.ReportError("Expected height in pixels");
				return;
			}

			// Next token should be the beginning of the texture scope
			parser.SkipWhitespace(true);
			tokenstr = parser.ReadToken();
			if(tokenstr != "{")
			{
				parser.ReportError("Expected begin of structure");
				return;
			}

			// Now parse the contents of texture structure
			while(parser.SkipWhitespace(true))
			{
				string token = parser.ReadToken();
				token = token.ToLowerInvariant();
				if(token == "xscale")
				{
					if(!ReadTokenFloat(parser, token, out xscale)) return;
				}
				else if(token == "yscale")
				{
					if(!ReadTokenFloat(parser, token, out yscale)) return;
				}
				else if(token == "worldpanning")
				{
					worldpanning = true;
				}
				else if(token == "offset")
				{
					// Read x offset
					if(!ReadTokenInt(parser, token, out xoffset)) return;

					// Now we should find a comma
					parser.SkipWhitespace(true);
					tokenstr = parser.ReadToken();
					if(tokenstr != ",")
					{
						parser.ReportError("Expected a comma");
						return;
					}
					
					// Read y offset
					if(!ReadTokenInt(parser, token, out yoffset)) return;
				}
				else if(token == "patch")
				{
					// Read patch structure
					PatchStructure pt = new PatchStructure(parser);
					if(parser.HasError) break;

					// Add the patch
					patches.Add(pt);
				}
				else if(token == "}")
				{
					// Actor scope ends here,
					// break out of this parse loop
					break;
				}
			}
		}

		#endregion

		#region ================== Methods

		// This reads the next token and sets a floating point value, returns false when failed
		private bool ReadTokenFloat(TexturesParser parser, string propertyname, out float value)
		{
			// Next token is the property value to set
			parser.SkipWhitespace(true);
			string strvalue = parser.ReadToken();
			if(!string.IsNullOrEmpty(strvalue))
			{
				// Try parsing as value
				if(!float.TryParse(strvalue, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
				{
					parser.ReportError("Expected numeric value for property '" + propertyname + "'");
					return false;
				}
				else
				{
					// Success
					return true;
				}
			}
			else
			{
				// Can't find the property value!
				parser.ReportError("Expected a value for property '" + propertyname + "'");
				value = 0.0f;
				return false;
			}
		}

		// This reads the next token and sets an integral value, returns false when failed
		private bool ReadTokenInt(TexturesParser parser, string propertyname, out int value)
		{
			// Next token is the property value to set
			parser.SkipWhitespace(true);
			string strvalue = parser.ReadToken();
			if(!string.IsNullOrEmpty(strvalue))
			{
				// Try parsing as value
				if(!int.TryParse(strvalue, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
				{
					parser.ReportError("Expected integral value for property '" + propertyname + "'");
					return false;
				}
				else
				{
					// Success
					return true;
				}
			}
			else
			{
				// Can't find the property value!
				parser.ReportError("Expected a value for property '" + propertyname + "'");
				value = 0;
				return false;
			}
		}

		// This makes a HighResImage texture for this texture
		internal HighResImage MakeImage(Dictionary<long, ImageData> textures, Dictionary<long, ImageData> flats)
		{
			float scalex, scaley;
			
			// Determine default scale
			float defaultscale = General.Map.Config.DefaultTextureScale;

			// Determine scale for texture
			if(xscale == 0.0f) scalex = defaultscale; else scalex = 1f / xscale;
			if(yscale == 0.0f) scaley = defaultscale; else scaley = 1f / yscale;

			// Make texture
			HighResImage tex = new HighResImage(name, width, height, scalex, scaley, worldpanning);

			// Add patches
			foreach(PatchStructure p in patches)
			{
				tex.AddPatch(new TexturePatch(p.Name.ToUpperInvariant(), p.OffsetX, p.OffsetY, p.FlipX, p.FlipY, 0, new PixelColor(0, 0, 0, 0), p.Alpha, 0));
			}
			
			return tex;
		}
		
		#endregion
	}
}
