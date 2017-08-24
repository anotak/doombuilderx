using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.IO;

namespace CodeImp.DoomBuilder.Config
{
	/// <summary>
	/// Category of generalized type options.
	/// </summary>
	public class GeneralizedCategory
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Category properties
		private string title;
		private int offset;
		private int length;
		private List<GeneralizedOption> options;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public string Title { get { return title; } }
		public int Offset { get { return offset; } }
		public int Length { get { return length; } }
		public List<GeneralizedOption> Options { get { return options; } }
		public bool IsDisposed { get { return isdisposed; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal GeneralizedCategory(string structure, string name, Configuration cfg)
		{
			IDictionary opts;
			
			// Initialize
			this.options = new List<GeneralizedOption>();
			
			// Read properties
			this.title = cfg.ReadSetting(structure + "." + name + ".title", "");
			this.offset = cfg.ReadSetting(structure + "." + name + ".offset", 0);
			this.length = cfg.ReadSetting(structure + "." + name + ".length", 0);
			
			// Read the options
			opts = cfg.ReadSetting(structure + "." + name, new Hashtable());
			foreach(DictionaryEntry de in opts)
			{
				// Is this an option and not just some value?
				if(de.Value is IDictionary)
				{
					// Add the option
					this.options.Add(new GeneralizedOption(structure, name, de.Key.ToString(), (IDictionary)de.Value));
				}
			}

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		internal void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				options = null;
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// String representation
		public override string ToString()
		{
			return title;
		}
		
		#endregion
	}
}
