
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

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class ResultSectorUnclosed : ErrorResult
	{
		#region ================== Variables
		
		private Sector sector;
		private List<Vertex> vertices;
		private int index;
		
		#endregion
		
		#region ================== Properties

		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public ResultSectorUnclosed(Sector s, List<Vertex> v)
		{
			// Initialize
			this.sector = s;
			this.vertices = new List<Vertex>(v);
			this.viewobjects.Add(s);
			foreach(Vertex vv in v) this.viewobjects.Add(vv);
			this.description = "This sector is not a closed region and could cause problems with clipping and rendering in the game. The 'leaks' in the sector are indicated by the colored vertices.";
			this.index = s.Index;
		}
		
		#endregion
		
		#region ================== Methods
		
		// This must return the string that is displayed in the listbox
		public override string ToString()
		{
			return "Sector " + index + " is not closed";
		}
		
		// Rendering
		public override void PlotSelection(IRenderer2D renderer)
		{
			renderer.PlotSector(sector, General.Colors.Selection);
			foreach(Vertex v in vertices)
				renderer.PlotVertex(v, ColorCollection.SELECTION);
		}
		
		#endregion
	}
}
