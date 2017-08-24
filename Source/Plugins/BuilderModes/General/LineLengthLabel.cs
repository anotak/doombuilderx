
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
	public class LineLengthLabel : IDisposable
	{
		#region ================== Constants

		private const int TEXT_CAPACITY = 10;
		private const float TEXT_SCALE = 14f;
		private const string VALUE_FORMAT = "0";

		#endregion

		#region ================== Variables

		private TextLabel label;
		private Vector2D start;
		private Vector2D end;
		
		#endregion

		#region ================== Properties

		public TextLabel TextLabel { get { return label; } }
		public Vector2D Start { get { return start; } set { start = value; Update(); } }
		public Vector2D End { get { return end; } set { end = value; Update(); } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public LineLengthLabel()
		{
			// Initialize
			Initialize();
		}

		// Constructor
		public LineLengthLabel(Vector2D start, Vector2D end)
		{
			// Initialize
			Initialize();
			Move(start, end);
		}

		// Initialization
		private void Initialize()
		{
			label = new TextLabel(TEXT_CAPACITY);
			label.AlignX = TextAlignmentX.Center;
			label.AlignY = TextAlignmentY.Middle;
			label.Color = General.Colors.Highlight;
			label.Backcolor = General.Colors.Background;
			label.Scale = TEXT_SCALE;
			label.TransformCoords = true;
		}
		
		// Disposer
		public void Dispose()
		{
			label.Dispose();
		}

		#endregion
		
		#region ================== Methods

		// This updates the text
		private void Update()
		{
			Vector2D delta = end - start;
			float length = delta.GetLength();
			label.Text = length.ToString(VALUE_FORMAT);
			label.Rectangle = new RectangleF(start.x + delta.x * 0.5f, start.y + delta.y * 0.5f, 0f, 0f);
		}

		// This moves the label
		public void Move(Vector2D start, Vector2D end)
		{
			this.start = start;
			this.end = end;
			Update();
		}
		
		#endregion
	}
}
