
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
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class ErrorResult
	{
		#region ================== Variables
		
		protected string description;
		protected List<MapElement> viewobjects;
		
		#endregion
		
		#region ================== Properties
		
		public string Description { get { return description; } }
		
		// Override these properties to create buttons
		public virtual int Buttons { get { return 0; } }
		public virtual string Button1Text { get { return ""; } }
		public virtual string Button2Text { get { return ""; } }
		public virtual string Button3Text { get { return ""; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public ErrorResult()
		{
			// Initialize
			viewobjects = new List<MapElement>(1);
		}
		
		#endregion
		
		#region ================== Methods
		
		// When the first button is clicked
		// Return true when map geometry or things have been added/removed so that the checker can restart
		public virtual bool Button1Click()
		{
			return false;
		}
		
		// When the second button is clicked
		// Return true when map geometry or things have been added/removed so that the checker can restart
		public virtual bool Button2Click()
		{
			return false;
		}
		
		// When the third button is clicked
		// Return true when map geometry or things have been added/removed so that the checker can restart
		public virtual bool Button3Click()
		{
			return false;
		}
		
		// This must return the string that is displayed in the listbox
		public override string ToString()
		{
			return "Unknown result";
		}

		// This is called for rendering
		public virtual void PlotSelection(IRenderer2D renderer)
		{
		}

		// This is called for rendering
		public virtual void RenderThingsSelection(IRenderer2D renderer)
		{
		}

		// This is called for rendering
		public virtual void RenderOverlaySelection(IRenderer2D renderer)
		{
		}
		
		// Call this to zoom in on the given selection
		public void ZoomToObject()
		{
			List<Vector2D> points = new List<Vector2D>();
			RectangleF area = MapSet.CreateEmptyArea();
			
			// Add all points to a list
			foreach(MapElement obj in viewobjects)
			{
				if(obj is Vertex)
				{
					points.Add((obj as Vertex).Position);
				}
				else if(obj is Linedef)
				{
					points.Add((obj as Linedef).Start.Position);
					points.Add((obj as Linedef).End.Position);
				}
				else if(obj is Sidedef)
				{
					points.Add((obj as Sidedef).Line.Start.Position);
					points.Add((obj as Sidedef).Line.End.Position);
				}
				else if(obj is Sector)
				{
					Sector s = (obj as Sector);
					foreach(Sidedef sd in s.Sidedefs)
					{
						points.Add(sd.Line.Start.Position);
						points.Add(sd.Line.End.Position);
					}
				}
				else if(obj is Thing)
				{
					Thing t = (obj as Thing);
					Vector2D p = (Vector2D)t.Position;
					points.Add(p);
					points.Add(p + new Vector2D(t.Size * 2.0f, t.Size * 2.0f));
					points.Add(p + new Vector2D(t.Size * 2.0f, -t.Size * 2.0f));
					points.Add(p + new Vector2D(-t.Size * 2.0f, t.Size * 2.0f));
					points.Add(p + new Vector2D(-t.Size * 2.0f, -t.Size * 2.0f));
				}
				else
				{
					General.Fail("Unknown object given to zoom in on.");
				}
			}
			
			// Make a view area from the points
			foreach(Vector2D p in points) area = MapSet.IncreaseArea(area, p);
			
			// Make the area square, using the largest side
			if(area.Width > area.Height)
			{
				float delta = area.Width - area.Height;
				area.Y -= delta * 0.5f;
				area.Height += delta;
			}
			else
			{
				float delta = area.Height - area.Width;
				area.X -= delta * 0.5f;
				area.Width += delta;
			}
			
			// Add padding
			area.Inflate(100f, 100f);
			
			// Zoom to area
			ClassicMode editmode = (General.Editing.Mode as ClassicMode);
			editmode.CenterOnArea(area, 0.6f);
		}
		
		#endregion
	}
}
