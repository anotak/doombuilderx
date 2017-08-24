
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
using System.Drawing;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class FindReplaceObject
	{
		#region ================== Variables

		private object obj;
		private string title;

		#endregion

		#region ================== Properties

		public object Object { get { return obj; } set { obj = value; } }
		public Sector Sector { get { return (Sector)obj; } }
		public Linedef Linedef { get { return (Linedef)obj; } }
		public Sidedef Sidedef { get { return (Sidedef)obj; } }
		public Thing Thing { get { return (Thing)obj; } }
		public Vertex Vertex { get { return (Vertex)obj; } }
		public string Title { get { return title; } set { title = value; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public FindReplaceObject(object obj, string title)
		{
			// Initialize
			this.obj = obj;
			this.title = title;
		}

		#endregion

		#region ================== Methods

		// String representation
		public override string ToString()
		{
			return title;
		}
		
		// This adds the vertices of the object used for view area calculation
		public void AddViewPoints(IList<Vector2D> points)
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
		}
		
		#endregion
	}
}
