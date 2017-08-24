
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
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	// Vertex
	public class VertexProperties
	{
		private UniFields fields;

		public VertexProperties(Vertex v)
		{
			fields = new UniFields(v.Fields);
		}

		public void Apply(Vertex v)
		{
			v.Fields.BeforeFieldsChange();
			v.Fields.Clear();
			foreach(KeyValuePair<string, UniValue> uv in fields)
				v.Fields.Add(uv.Key, new UniValue(uv.Value));
		}
	}

	// Sector
	public class SectorProperties
	{
		private int floorheight;
		private int ceilheight;
		private string floortexture;
		private string ceilingtexture;
		private int effect;
		private int brightness;
		private int tag;
		private UniFields fields;
		
		public SectorProperties(Sector s)
		{
			floorheight = s.FloorHeight;
			ceilheight = s.CeilHeight;
			floortexture = s.FloorTexture;
			ceilingtexture = s.CeilTexture;
			brightness = s.Brightness;
			effect = s.Effect;
			tag = s.Tag;
			fields = new UniFields(s.Fields);
		}
		
		public void Apply(Sector s)
		{
			s.FloorHeight = floorheight;
			s.CeilHeight = ceilheight;
			s.SetFloorTexture(floortexture);
			s.SetCeilTexture(ceilingtexture);
			s.Brightness = brightness;
			s.Tag = tag;
			s.Effect = effect;
			s.Fields.BeforeFieldsChange();
			s.Fields.Clear();
			foreach(KeyValuePair<string, UniValue> v in fields)
				s.Fields.Add(v.Key, new UniValue(v.Value));
		}
	}

	// Sidedef
	public class SidedefProperties
	{
		private string hightexture;
		private string middletexture;
		private string lowtexture;
		private int offsetx;
		private int offsety;
		private UniFields fields;

		public SidedefProperties(Sidedef s)
		{
			hightexture = s.HighTexture;
			middletexture = s.MiddleTexture;
			lowtexture = s.LowTexture;
			offsetx = s.OffsetX;
			offsety = s.OffsetY;
			fields = new UniFields(s.Fields);
		}
		
		public void Apply(Sidedef s)
		{
			s.SetTextureHigh(hightexture);
			s.SetTextureMid(middletexture);
			s.SetTextureLow(lowtexture);
			s.OffsetX = offsetx;
			s.OffsetY = offsety;
			s.Fields.BeforeFieldsChange();
			s.Fields.Clear();
			foreach(KeyValuePair<string, UniValue> v in fields)
				s.Fields.Add(v.Key, new UniValue(v.Value));
		}
	}

	// Linedef
	public class LinedefProperties
	{
		private SidedefProperties front;
		private SidedefProperties back;
		private Dictionary<string, bool> flags;
		private int action;
		private int activate;
		private int tag;
		private int[] args;
		private UniFields fields;

		public LinedefProperties(Linedef l)
		{
			if(l.Front != null)
				front = new SidedefProperties(l.Front);
			else
				front = null;

			if(l.Back != null)
				back = new SidedefProperties(l.Back);
			else
				back = null;

			flags = l.GetFlags();
			action = l.Action;
			activate = l.Activate;
			tag = l.Tag;
			args = (int[])(l.Args.Clone());
			fields = new UniFields(l.Fields);
		}
		
		public void Apply(Linedef l)
		{
			if((front != null) && (l.Front != null)) front.Apply(l.Front);
			if((back != null) && (l.Back != null)) back.Apply(l.Back);
			l.ClearFlags();
			foreach(KeyValuePair<string, bool> f in flags)
				l.SetFlag(f.Key, f.Value);
			l.Action = action;
			l.Activate = activate;
			l.Tag = tag;
			for(int i = 0; i < l.Args.Length; i++)
				l.Args[i] = args[i];
			l.Fields.BeforeFieldsChange();
			l.Fields.Clear();
			foreach(KeyValuePair<string, UniValue> v in fields)
				l.Fields.Add(v.Key, new UniValue(v.Value));
		}
	}
	
	// Thing
	public class ThingProperties
	{
		private int type;
		private float angle;
		private Dictionary<string, bool> flags;
		private int tag;
		private int action;
		private int[] args;
		private UniFields fields;
		
		public ThingProperties(Thing t)
		{
			type = t.Type;
			angle = t.Angle;
			flags = t.GetFlags();
			tag = t.Tag;
			action = t.Action;       
			args = (int[])(t.Args.Clone());
			fields = new UniFields(t.Fields);
		}
		
		public void Apply(Thing t)
		{
			t.Type = type;
			t.Rotate(angle);
			t.ClearFlags();
			foreach(KeyValuePair<string, bool> f in flags)
				t.SetFlag(f.Key, f.Value);
			t.Tag = tag;
			t.Action = action;
			for(int i = 0; i < t.Args.Length; i++)
				t.Args[i] = args[i];
			t.Fields.BeforeFieldsChange();
			t.Fields.Clear();
			foreach(KeyValuePair<string, UniValue> v in fields)
				t.Fields.Add(v.Key, new UniValue(v.Value));
		}
	}
}
