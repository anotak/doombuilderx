
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
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	public class TextLabel : IDisposable, ID3DResource
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// The text is stored as a polygon in a vertex buffer
		private VertexBuffer textbuffer;
		private int numfaces;
		private int capacity;
		
		// Text settings
		private string text;
		private RectangleF rect;
		private bool transformcoords;
		private PixelColor color;
		private PixelColor backcolor;
		private float scale;
		private TextAlignmentX alignx;
		private TextAlignmentY aligny;
		private SizeF size;
		
		// This keeps track if changes were made
		private bool updateneeded;
		private float lasttranslatex = float.MinValue;
		private float lasttranslatey;
		private float lastscalex;
		private float lastscaley;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Properties
		public RectangleF Rectangle { get { return rect; } set { rect = value; updateneeded = true; } }
		public float Left { get { return rect.X; } set { rect.X = value; updateneeded = true; } }
		public float Top { get { return rect.Y; } set { rect.Y = value; updateneeded = true; } }
		public float Width { get { return rect.Width; } set { rect.Width = value; updateneeded = true; } }
		public float Height { get { return rect.Height; } set { rect.Height = value; updateneeded = true; } }
		public float Right { get { return rect.Right; } set { rect.Width = value - rect.X + 1f; updateneeded = true; } }
		public float Bottom { get { return rect.Bottom; } set { rect.Height = value - rect.Y + 1f; updateneeded = true; } }
		public string Text { get { return text; } set { if(text != value.ToUpperInvariant()) { text = value.ToUpperInvariant(); updateneeded = true; } } }
		public bool TransformCoords { get { return transformcoords; } set { transformcoords = value; updateneeded = true; } }
		public SizeF TextSize { get { return size; } }
		public float Scale { get { return scale; } set { scale = value; updateneeded = true; } }
		public TextAlignmentX AlignX { get { return alignx; } set { alignx = value; updateneeded = true; } }
		public TextAlignmentY AlignY { get { return aligny; } set { aligny = value; updateneeded = true; } }
		public PixelColor Color { get { return color; } set { color = value; updateneeded = true; } }
		public PixelColor Backcolor { get { return backcolor; } set { backcolor = value; updateneeded = true; } }
		internal VertexBuffer VertexBuffer { get { return textbuffer; } }
		internal int NumFaces { get { return numfaces; } }
		
		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public TextLabel(int capacity)
		{
			// Initialize
			this.text = "";
			this.rect = new RectangleF(0f, 0f, 1f, 1f);
			this.color = new PixelColor(255, 255, 255, 255);
			this.backcolor = new PixelColor(0, 0, 0, 0);
			this.scale = 10f;
			this.alignx = TextAlignmentX.Left;
			this.aligny = TextAlignmentY.Top;
			this.size = new SizeF(0f, 0f);
			this.updateneeded = true;
			this.numfaces = 0;
			this.capacity = capacity;
			
			// Register as resource
			General.Map.Graphics.RegisterResource(this);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				UnloadResource();
				
				// Unregister resource
				General.Map.Graphics.UnregisterResource(this);
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods
		
		// This updates the text if needed
		internal void Update(float translatex, float translatey, float scalex, float scaley)
		{
			FlatVertex[] verts;
			RectangleF absview;
			float beginx = 0;
			float beginy = 0;
			bool colorcode = false;
			int characters = 0;
			byte[] textbytes;
			DataStream stream;

			// Check if transformation changed and needs to be updated
			if(transformcoords)
			{
				if((translatex != lasttranslatex) ||
				   (translatey != lasttranslatey) ||
				   (scalex != lastscalex) ||
				   (scaley != lastscaley)) updateneeded = true;
			}
			
			// Update if needed
			if(updateneeded)
			{
				// Only build when there are any vertices
				if(text.Length > 0)
				{
					// Do we have to make a new buffer?
					if((textbuffer == null) || (text.Length > capacity))
					{
						// Dispose previous
						if(textbuffer != null) textbuffer.Dispose();
						
						// Determine new capacity
						if(capacity < text.Length) capacity = text.Length;
						
						// Create the buffer
						textbuffer = new VertexBuffer(General.Map.Graphics.Device,
													  capacity * 12 * FlatVertex.Stride,
													  Usage.Dynamic | Usage.WriteOnly,
													  VertexFormat.None, Pool.Default);
					}
					
					// Transform?
					if(transformcoords)
					{
						// Calculate absolute coordinates
						Vector2D lt = new Vector2D(rect.Left, rect.Top);
						Vector2D rb = new Vector2D(rect.Right, rect.Bottom);
						lt = lt.GetTransformed(translatex, translatey, scalex, scaley);
						rb = rb.GetTransformed(translatex, translatey, scalex, scaley);
						absview = new RectangleF(lt.x, lt.y, rb.x - lt.x, rb.y - lt.y);
					}
					else
					{
						// Fixed coordinates
						absview = rect;
					}
					
					// Calculate text dimensions
					size = General.Map.Graphics.Font.GetTextSize(text, scale);

					// Align the text horizontally
					switch(alignx)
					{
						case TextAlignmentX.Left: beginx = absview.X; break;
						case TextAlignmentX.Center: beginx = absview.X + (absview.Width - size.Width) * 0.5f; break;
						case TextAlignmentX.Right: beginx = absview.X + absview.Width - size.Width; break;
					}

					// Align the text vertically
					switch(aligny)
					{
						case TextAlignmentY.Top: beginy = absview.Y; break;
						case TextAlignmentY.Middle: beginy = absview.Y + (absview.Height - size.Height) * 0.5f; break;
						case TextAlignmentY.Bottom: beginy = absview.Y + absview.Height - size.Height; break;
					}

					// Get the ASCII bytes for the text
					textbytes = Encoding.ASCII.GetBytes(text);

					// Lock the buffer
					stream = textbuffer.Lock(0, capacity * 12 * FlatVertex.Stride,
									LockFlags.Discard | LockFlags.NoSystemLock);
					
					// Go for all chars in text to create the backgrounds
					float textx = beginx;
					foreach(byte b in textbytes)
						General.Map.Graphics.Font.SetupVertices(stream, b, scale, backcolor.ToInt(),
															ref textx, beginy, size.Height, 0.5f);

					// Go for all chars in text to create the text
					textx = beginx;
					foreach(byte b in textbytes)
						General.Map.Graphics.Font.SetupVertices(stream, b, scale, color.ToInt(),
															ref textx, beginy, size.Height, 0.0f);

					// Done filling the vertex buffer
					textbuffer.Unlock();
					stream.Dispose();

					// Calculate number of triangles
					numfaces = text.Length * 4;
				}
				else
				{
					// No faces in polygon
					numfaces = 0;
					size = new SizeF(0f, 0f);
				}

				// Text updated
				updateneeded = false;
			}
		}

		// This unloads the resources
		public void UnloadResource()
		{
			// Clean up
			if(textbuffer != null) textbuffer.Dispose();
			textbuffer = null;

			// Need to update before we can render
			updateneeded = true;
		}

		// This (re)loads the resources
		public void ReloadResource()
		{
		}
		
		#endregion
	}
}
