
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
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.Types
{
	internal class TypesManager : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// List of handler types
		private Dictionary<int, TypeHandlerAttribute> handlertypes;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public TypesManager()
		{
			// Initialize
			handlertypes = new Dictionary<int, TypeHandlerAttribute>();

			// Go for all types in this assembly
			Type[] types = General.ThisAssembly.GetTypes();
			foreach(Type tp in types)
			{
				// Check if this type is a class
				if(tp.IsClass && !tp.IsAbstract && !tp.IsArray)
				{
					// Check if class has an TypeHandler attribute
					if(Attribute.IsDefined(tp, typeof(TypeHandlerAttribute), false))
					{
						// Add the type to the list
						object[] attribs = tp.GetCustomAttributes(typeof(TypeHandlerAttribute), false);
						TypeHandlerAttribute attr = (attribs[0] as TypeHandlerAttribute);
						attr.Type = tp;
						handlertypes.Add(attr.Index, attr);
					}
				}
			}
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				handlertypes.Clear();
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// This returns the type handler for the given argument
		public TypeHandler GetArgumentHandler(ArgumentInfo arginfo)
		{
			Type t = typeof(NullHandler);
			TypeHandlerAttribute ta = null;
			
			// Do we have a handler type for this?
			if(handlertypes.ContainsKey(arginfo.Type))
			{
				ta = handlertypes[arginfo.Type];
				t = ta.Type;
			}

			// Create instance
			TypeHandler th = (TypeHandler)General.ThisAssembly.CreateInstance(t.FullName);
			th.SetupArgument(ta, arginfo);
            if (th == null)
            {
                throw new Exception();
            }
			return th;
		}

		// This returns the type handler for a custom universal field
		public TypeHandler GetFieldHandler(int type, object defaultsetting)
		{
			Type t = typeof(NullHandler);
			TypeHandlerAttribute ta = null;

			// Do we have a handler type for this?
			if(handlertypes.ContainsKey(type))
			{
				ta = handlertypes[type];
				t = ta.Type;
			}

			// Create instance
			TypeHandler th = (TypeHandler)General.ThisAssembly.CreateInstance(t.FullName);
			th.SetupField(ta, null);
			th.SetValue(defaultsetting);
			return th;
		}

		// This returns the type handler for a given universal field
		public TypeHandler GetFieldHandler(UniversalFieldInfo fieldinfo)
		{
			Type t = typeof(NullHandler);
			TypeHandlerAttribute ta = null;

			// Do we have a handler type for this?
			if(handlertypes.ContainsKey(fieldinfo.Type))
			{
				ta = handlertypes[fieldinfo.Type];
				t = ta.Type;
			}

			// Create instance
			TypeHandler th = (TypeHandler)General.ThisAssembly.CreateInstance(t.FullName);
			th.SetupField(ta, fieldinfo);
			th.SetValue(fieldinfo.Default);
			return th;
		}
		
		// This returns all custom attributes
		public TypeHandlerAttribute[] GetCustomUseAttributes()
		{
			List<TypeHandlerAttribute> attribs = new List<TypeHandlerAttribute>();
			foreach(KeyValuePair<int, TypeHandlerAttribute> ta in handlertypes)
				if(ta.Value.IsCustomUsable) attribs.Add(ta.Value);
			return attribs.ToArray();
		}

		// This returns the attribute with the give name
		public TypeHandlerAttribute GetNamedAttribute(string name)
		{
			foreach(KeyValuePair<int, TypeHandlerAttribute> ta in handlertypes)
			{
				if(ta.Value.Name == name) return ta.Value;
			}

			// Nothing found
			return null;
		}

		// This returns the attribute with the give type
		public TypeHandlerAttribute GetAttribute(int type)
		{
			// Do we have a handler type for this?
			if(handlertypes.ContainsKey(type)) return handlertypes[type];
				else return null;
		}
		
		#endregion
	}
}
