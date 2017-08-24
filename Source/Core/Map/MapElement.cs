
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
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using SlimDX.Direct3D9;
using System.Drawing;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public abstract class MapElement : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables
		
		// List index
		protected int listindex;
		
		// Univeral fields
		private UniFields fields;
		
		// Marking
		protected bool marked;
		
		// Disposing
		protected bool isdisposed = false;
		
		#endregion
		
		#region ================== Properties

		public int Index { get { return listindex; } internal set { listindex = value; } }
		public UniFields Fields { get { return fields; } }
		public bool Marked { get { return marked; } set { marked = value; } }
		public bool IsDisposed { get { return isdisposed; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal MapElement()
		{
			// Initialize
			fields = new UniFields(this);
		}

		// Disposer
		public virtual void Dispose()
		{
			// Clean up
			fields.Owner = null;
			fields = null;
			
			// Done
			isdisposed = true;
		}

		#endregion

		#region ================== Methods

		// Serialize / deserialize
		internal void ReadWrite(IReadWriteStream s)
		{
			int c = fields.Count;
			s.rwInt(ref c);

			if(s.IsWriting)
			{
				foreach(KeyValuePair<string, UniValue> f in fields)
				{
					s.wString(f.Key);
					f.Value.ReadWrite(s);
				}
			}
			else
			{
				fields = new UniFields(this, c);
				for(int i = 0; i < c; i++)
				{
					string t; s.rString(out t);
					UniValue v = new UniValue(); v.ReadWrite(s);
					fields.Add(t, v);
				}
			}
		}

		// This copies properties to any other element
		public void CopyPropertiesTo(MapElement element)
		{
			element.fields = new UniFields(this, this.fields);
		}
		
		// This must implement the call to the undo system to record the change of properties
		protected abstract void BeforePropsChange();
		
		// This is called before the custom fields change
		internal void BeforeFieldsChange()
		{
			BeforePropsChange();
		}
		
		#endregion
	}
}
