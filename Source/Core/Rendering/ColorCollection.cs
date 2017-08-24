
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
using System.Reflection;
using System.Drawing;
using CodeImp.DoomBuilder.IO;
using SlimDX.Direct3D9;
using SlimDX;

using Configuration = CodeImp.DoomBuilder.IO.Configuration;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	public sealed class ColorCollection
	{
		#region ================== Constants

		// Assist color creation
		private const float BRIGHT_MULTIPLIER = 1.0f;
		private const float BRIGHT_ADDITION = 0.4f;
		private const float DARK_MULTIPLIER = 0.9f;
		private const float DARK_ADDITION = -0.2f;

		// Palette size
		private const int NUM_COLORS = 41;
		public const int NUM_THING_COLORS = 20;
		public const int THING_COLORS_OFFSET = 20;

		// Colors!
		public const int BACKGROUND = 0;
		public const int VERTICES = 1;
		public const int LINEDEFS = 2;
		public const int ACTIONS = 3;
		public const int SOUNDS = 4;
		public const int HIGHLIGHT = 5;
		public const int SELECTION = 6;
		public const int INDICATION = 7;
		public const int GRID = 8;
		public const int GRID64 = 9;
		public const int CROSSHAIR3D = 10;
		public const int HIGHLIGHT3D = 11;
		public const int SELECTION3D = 12;
		public const int SCRIPTBACKGROUND = 13;
		public const int LINENUMBERS = 14;
		public const int PLAINTEXT = 15;
		public const int COMMENTS = 16;
		public const int KEYWORDS = 17;
		public const int LITERALS = 18;
		public const int CONSTANTS = 19;
		public const int THINGCOLOR00 = 20;
		public const int THINGCOLOR01 = 21;
		public const int THINGCOLOR02 = 22;
		public const int THINGCOLOR03 = 23;
		public const int THINGCOLOR04 = 24;
		public const int THINGCOLOR05 = 25;
		public const int THINGCOLOR06 = 26;
		public const int THINGCOLOR07 = 27;
		public const int THINGCOLOR08 = 28;
		public const int THINGCOLOR09 = 29;
		public const int THINGCOLOR10 = 30;
		public const int THINGCOLOR11 = 31;
		public const int THINGCOLOR12 = 32;
		public const int THINGCOLOR13 = 33;
		public const int THINGCOLOR14 = 34;
		public const int THINGCOLOR15 = 35;
		public const int THINGCOLOR16 = 36;
		public const int THINGCOLOR17 = 37;
		public const int THINGCOLOR18 = 38;
		public const int THINGCOLOR19 = 39;
        public const int PORTALS = 40;

        #endregion

        #region ================== Variables

        // Colors
        private PixelColor[] colors;
		private PixelColor[] brightcolors;
		private PixelColor[] darkcolors;
		
		// Color-correction table
		private byte[] correctiontable;
		
		#endregion
		
		#region ================== Properties
		
		public PixelColor[] Colors { get { return colors; } }
		public PixelColor[] BrightColors { get { return brightcolors; } }
		public PixelColor[] DarkColors { get { return darkcolors; } }
		
		public PixelColor Background { get { return colors[BACKGROUND]; } internal set { colors[BACKGROUND] = value; } }
		public PixelColor Vertices { get { return colors[VERTICES]; } internal set { colors[VERTICES] = value; } }
		public PixelColor Linedefs { get { return colors[LINEDEFS]; } internal set { colors[LINEDEFS] = value; } }
		public PixelColor Actions { get { return colors[ACTIONS]; } internal set { colors[ACTIONS] = value; } }
		public PixelColor Sounds { get { return colors[SOUNDS]; } internal set { colors[SOUNDS] = value; } }
		public PixelColor Highlight { get { return colors[HIGHLIGHT]; } internal set { colors[HIGHLIGHT] = value; } }
		public PixelColor Selection { get { return colors[SELECTION]; } internal set { colors[SELECTION] = value; } }
		public PixelColor Indication { get { return colors[INDICATION]; } internal set { colors[INDICATION] = value; } }
		public PixelColor Grid { get { return colors[GRID]; } internal set { colors[GRID] = value; } }
		public PixelColor Grid64 { get { return colors[GRID64]; } internal set { colors[GRID64] = value; } }
		
		public PixelColor Crosshair3D { get { return colors[CROSSHAIR3D]; } internal set { colors[CROSSHAIR3D] = value; } }
		public PixelColor Highlight3D { get { return colors[HIGHLIGHT3D]; } internal set { colors[HIGHLIGHT3D] = value; } }
		public PixelColor Selection3D { get { return colors[SELECTION3D]; } internal set { colors[SELECTION3D] = value; } }
		
		public PixelColor ScriptBackground { get { return colors[SCRIPTBACKGROUND]; } internal set { colors[SCRIPTBACKGROUND] = value; } }
		public PixelColor LineNumbers { get { return colors[LINENUMBERS]; } internal set { colors[LINENUMBERS] = value; } }
		public PixelColor PlainText { get { return colors[PLAINTEXT]; } internal set { colors[PLAINTEXT] = value; } }
		public PixelColor Comments { get { return colors[COMMENTS]; } internal set { colors[COMMENTS] = value; } }
		public PixelColor Keywords { get { return colors[KEYWORDS]; } internal set { colors[KEYWORDS] = value; } }
		public PixelColor Literals { get { return colors[LITERALS]; } internal set { colors[LITERALS] = value; } }
		public PixelColor Constants { get { return colors[CONSTANTS]; } internal set { colors[CONSTANTS] = value; } }
        public PixelColor Portals { get { return colors[PORTALS]; } internal set { colors[PORTALS] = value; } }

        // gzdb cross compat
        public PixelColor InfoLine { get { return colors[GRID64]; } internal set {  } }

        #endregion

        #region ================== Constructor / Disposer

        // Constructor for settings from configuration
        internal ColorCollection(Configuration cfg)
		{
			// Initialize
			colors = new PixelColor[NUM_COLORS];
			brightcolors = new PixelColor[NUM_COLORS];
			darkcolors = new PixelColor[NUM_COLORS];
			
			// Read all colors from config
			for(int i = 0; i < NUM_COLORS; i++)
			{
				// Read color
				colors[i] = PixelColor.FromInt(cfg.ReadSetting("colors.color" + i.ToString(CultureInfo.InvariantCulture), 0));
			}

			// Set new colors
			if(colors[THINGCOLOR00].ToInt() == 0) colors[THINGCOLOR00] = PixelColor.FromColor(Color.DimGray);
			if(colors[THINGCOLOR01].ToInt() == 0) colors[THINGCOLOR01] = PixelColor.FromColor(Color.RoyalBlue);
			if(colors[THINGCOLOR02].ToInt() == 0) colors[THINGCOLOR02] = PixelColor.FromColor(Color.ForestGreen);
			if(colors[THINGCOLOR03].ToInt() == 0) colors[THINGCOLOR03] = PixelColor.FromColor(Color.LightSeaGreen);
			if(colors[THINGCOLOR04].ToInt() == 0) colors[THINGCOLOR04] = PixelColor.FromColor(Color.Firebrick);
			if(colors[THINGCOLOR05].ToInt() == 0) colors[THINGCOLOR05] = PixelColor.FromColor(Color.DarkViolet);
			if(colors[THINGCOLOR06].ToInt() == 0) colors[THINGCOLOR06] = PixelColor.FromColor(Color.DarkGoldenrod);
			if(colors[THINGCOLOR07].ToInt() == 0) colors[THINGCOLOR07] = PixelColor.FromColor(Color.Silver);
			if(colors[THINGCOLOR08].ToInt() == 0) colors[THINGCOLOR08] = PixelColor.FromColor(Color.Gray);
			if(colors[THINGCOLOR09].ToInt() == 0) colors[THINGCOLOR09] = PixelColor.FromColor(Color.DeepSkyBlue);
			if(colors[THINGCOLOR10].ToInt() == 0) colors[THINGCOLOR10] = PixelColor.FromColor(Color.LimeGreen);
			if(colors[THINGCOLOR11].ToInt() == 0) colors[THINGCOLOR11] = PixelColor.FromColor(Color.PaleTurquoise);
			if(colors[THINGCOLOR12].ToInt() == 0) colors[THINGCOLOR12] = PixelColor.FromColor(Color.Tomato);
			if(colors[THINGCOLOR13].ToInt() == 0) colors[THINGCOLOR13] = PixelColor.FromColor(Color.Violet);
			if(colors[THINGCOLOR14].ToInt() == 0) colors[THINGCOLOR14] = PixelColor.FromColor(Color.Yellow);
			if(colors[THINGCOLOR15].ToInt() == 0) colors[THINGCOLOR15] = PixelColor.FromColor(Color.WhiteSmoke);
			if(colors[THINGCOLOR16].ToInt() == 0) colors[THINGCOLOR16] = PixelColor.FromColor(Color.LightPink);
			if(colors[THINGCOLOR17].ToInt() == 0) colors[THINGCOLOR17] = PixelColor.FromColor(Color.DarkOrange);
			if(colors[THINGCOLOR18].ToInt() == 0) colors[THINGCOLOR18] = PixelColor.FromColor(Color.DarkKhaki);
			if(colors[THINGCOLOR19].ToInt() == 0) colors[THINGCOLOR19] = PixelColor.FromColor(Color.Goldenrod);
            if (colors[PORTALS].ToInt() == 0)
            {
                colors[PORTALS] = PixelColor.FromColor(Color.Magenta);
                colors[PORTALS].a = 128;
            }

            // Create assist colors
            CreateAssistColors();
			
			// Create color correction table
			CreateCorrectionTable();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods
		
		// This generates a color-correction table
		internal void CreateCorrectionTable()
		{
			// Determine amounts
			float gamma = (float)(General.Settings.ImageBrightness + 10) * 0.1f;
			float bright = (float)General.Settings.ImageBrightness * 5f;
			
			// Make table
			correctiontable = new byte[256];
			
			// Fill table
			for(int i = 0; i < 256; i++)
			{
				byte b;
				float a = (float)i * gamma + bright;
				if(a < 0f) b = 0; else if(a > 255f) b = 255; else b = (byte)a;
				correctiontable[i] = b;
			}
		}
		
		// This applies color-correction over a block of pixel data
		internal unsafe void ApplColorCorrection(PixelColor* pixels, int numpixels)
		{
			for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
			{
				cp->r = correctiontable[cp->r];
				cp->g = correctiontable[cp->g];
				cp->b = correctiontable[cp->b];
			}
		}
		
		// This clamps a value between 0 and 1
		private float Saturate(float v)
		{
			if(v < 0f) return 0f; else if(v > 1f) return 1f; else return v;
		}

		// This creates assist colors
		internal void CreateAssistColors()
		{
			// Go for all colors
			for(int i = 0; i < NUM_COLORS; i++)
			{
				// Create assist colors
				brightcolors[i] = CreateBrightVariant(colors[i]);
				darkcolors[i] = CreateDarkVariant(colors[i]);
			}
		}
		
		// This creates a brighter color
		public PixelColor CreateBrightVariant(PixelColor pc)
		{
			Color4 o = pc.ToColorValue();
			Color4 c = new Color4(1f, 0f, 0f, 0f);
						
			// Create brighter color
			c.Red = Saturate(o.Red * BRIGHT_MULTIPLIER + BRIGHT_ADDITION);
			c.Green = Saturate(o.Green * BRIGHT_MULTIPLIER + BRIGHT_ADDITION);
			c.Blue = Saturate(o.Blue * BRIGHT_MULTIPLIER + BRIGHT_ADDITION);
			return PixelColor.FromInt(c.ToArgb());
		}

		// This creates a darker color
		public PixelColor CreateDarkVariant(PixelColor pc)
		{
			Color4 o = pc.ToColorValue();
			Color4 c = new Color4(1f, 0f, 0f, 0f);

			// Create darker color
			c.Red = Saturate(o.Red * DARK_MULTIPLIER + DARK_ADDITION);
			c.Green = Saturate(o.Green * DARK_MULTIPLIER + DARK_ADDITION);
			c.Blue = Saturate(o.Blue * DARK_MULTIPLIER + DARK_ADDITION);
			return PixelColor.FromInt(c.ToArgb());
		}

		// This saves colors to configuration
		internal void SaveColors(Configuration cfg)
		{
			// Write all colors to config
			for(int i = 0; i < NUM_COLORS; i++)
			{
				// Write color
				cfg.WriteSetting("colors.color" + i.ToString(CultureInfo.InvariantCulture), colors[i].ToInt());
			}
		}
		
		#endregion
	}
}
