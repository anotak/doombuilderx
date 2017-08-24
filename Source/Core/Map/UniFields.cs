#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	/// <summary>
	/// List of universal fields and their values.
	/// </summary>
	public class UniFields : SortedList<string, UniValue>
	{
		// Owner of this list
		protected MapElement owner;
		public MapElement Owner { get { return owner; } internal set { owner = value; } }
		
		// New constructor
		///<summary></summary>
		public UniFields() : base(2)
		{
		}

		// New constructor
		///<summary></summary>
		public UniFields(int capacity) : base(capacity)
		{
		}

		// Copy constructor (makes a deep copy)
		///<summary></summary>
		public UniFields(UniFields copyfrom) : base(copyfrom.Count)
		{
			foreach(KeyValuePair<string, UniValue> v in copyfrom)
				this.Add(v.Key, new UniValue(v.Value));
		}
		
		// New constructor
		///<summary></summary>
		public UniFields(MapElement owner) : base(2)
		{
			this.owner = owner;
		}

		// New constructor
		///<summary></summary>
		public UniFields(MapElement owner, int capacity) : base(capacity)
		{
			this.owner = owner;
		}

		// Copy constructor
		///<summary></summary>
		public UniFields(MapElement owner, UniFields copyfrom) : base(copyfrom)
		{
			this.owner = owner;
		}
		
		/// <summary>Call this before making changes to the fields, or they may not be updated correctly with undo/redo!</summary>
		public void BeforeFieldsChange()
		{
			if(owner != null)
				owner.BeforeFieldsChange();
		}
		
		/// <summary>This returns the value of a field by name, or returns the specified value when no such field exists or the field value fails to convert to the same datatype.</summary>
		public T GetValue<T>(string fieldname, T defaultvalue)
		{
			if(!this.ContainsKey(fieldname))
				return defaultvalue;

			try
			{
				T val = (T)this[fieldname].Value;
				return val;
			}
			catch(InvalidCastException)
			{
				return defaultvalue;
			}
		}

        public static int GetInteger(UniFields fields, string key) { return GetInteger(fields, key, 0); }
        public static int GetInteger(UniFields fields, string key, int defaultvalue)
        {
            if (fields == null) return defaultvalue;
            return fields.GetValue(key, defaultvalue);
        }
    }
}
