
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
using CodeImp.DoomBuilder.Editing;
using System.Collections.Specialized;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class ConfigurationInfo : IComparable<ConfigurationInfo>
	{
		#region ================== Constants

		private const string MODE_DISABLED_KEY = "disabled";
		private const string MODE_ENABLED_KEY = "enabled";

		// The { and } are invalid key names in a configuration so this ensures this string is unique
		private const string MISSING_NODEBUILDER = "{missing nodebuilder}";

		#endregion
		
		#region ================== Variables

		private string name;
		private string filename;
		private string settingskey;
		private string defaultlumpname;
		private string nodebuildersave;
		private string nodebuildertest;
		private DataLocationList resources;
		private string testprogram;
		private string testparameters;
		private bool testshortpaths;
		private bool customparameters;
		private int testskill;
		private List<ThingsFilter> thingsfilters;
		private List<DefinedTextureSet> texturesets;
		private Dictionary<string, bool> editmodes;
		private string startmode;
		
		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public string Filename { get { return filename; } }
		public string DefaultLumpName { get { return defaultlumpname; } }
		public string NodebuilderSave { get { return nodebuildersave; } internal set { nodebuildersave = value; } }
		public string NodebuilderTest { get { return nodebuildertest; } internal set { nodebuildertest = value; } }
		internal DataLocationList Resources { get { return resources; } }
		public string TestProgram { get { return testprogram; } internal set { testprogram = value; } }
		public string TestParameters { get { return testparameters; } internal set { testparameters = value; } }
		public bool TestShortPaths { get { return testshortpaths; } internal set { testshortpaths = value; } }
		public int TestSkill { get { return testskill; } internal set { testskill = value; } }
		public bool CustomParameters { get { return customparameters; } internal set { customparameters = value; } }
		internal ICollection<ThingsFilter> ThingsFilters { get { return thingsfilters; } }
		internal List<DefinedTextureSet> TextureSets { get { return texturesets; } }
		internal Dictionary<string, bool> EditModes { get { return editmodes; } }
		public string StartMode { get { return startmode; } internal set { startmode = value; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal ConfigurationInfo(Configuration cfg, string filename)
		{
			// Initialize
			this.filename = filename;
			this.settingskey = Path.GetFileNameWithoutExtension(filename).ToLower();
			
			// Load settings from game configuration
			this.name = cfg.ReadSetting("game", "<unnamed game>");
			this.defaultlumpname = cfg.ReadSetting("defaultlumpname", "");
			
			// Load settings from program configuration
			this.nodebuildersave = General.Settings.ReadSetting("configurations." + settingskey + ".nodebuildersave", MISSING_NODEBUILDER);
			this.nodebuildertest = General.Settings.ReadSetting("configurations." + settingskey + ".nodebuildertest", MISSING_NODEBUILDER);
			this.testprogram = General.Settings.ReadSetting("configurations." + settingskey + ".testprogram", "");
			this.testparameters = General.Settings.ReadSetting("configurations." + settingskey + ".testparameters", "");
			this.testshortpaths = General.Settings.ReadSetting("configurations." + settingskey + ".testshortpaths", false);
			this.customparameters = General.Settings.ReadSetting("configurations." + settingskey + ".customparameters", false);
			this.testskill = General.Settings.ReadSetting("configurations." + settingskey + ".testskill", 3);
			this.resources = new DataLocationList(General.Settings.Config, "configurations." + settingskey + ".resources");
			this.startmode = General.Settings.ReadSetting("configurations." + settingskey + ".startmode", "VerticesMode");
			
			// Make list of things filters
			thingsfilters = new List<ThingsFilter>();
			IDictionary cfgfilters = General.Settings.ReadSetting("configurations." + settingskey + ".thingsfilters", new Hashtable());
			foreach(DictionaryEntry de in cfgfilters)
			{
				thingsfilters.Add(new ThingsFilter(General.Settings.Config, "configurations." + settingskey + ".thingsfilters." + de.Key));
			}

			// Make list of texture sets
			texturesets = new List<DefinedTextureSet>();
			IDictionary sets = General.Settings.ReadSetting("configurations." + settingskey + ".texturesets", new Hashtable());
			foreach(DictionaryEntry de in sets)
			{
				texturesets.Add(new DefinedTextureSet(General.Settings.Config, "configurations." + settingskey + ".texturesets." + de.Key));
			}
			
			// Make list of edit modes
			this.editmodes = new Dictionary<string, bool>();
			IDictionary modes = General.Settings.ReadSetting("configurations." + settingskey + ".editmodes", new Hashtable());
			foreach(DictionaryEntry de in modes)
			{
				if(de.Key.ToString().StartsWith(MODE_ENABLED_KEY))
					editmodes.Add(de.Value.ToString(), true);
				else if(de.Key.ToString().StartsWith(MODE_DISABLED_KEY))
					editmodes.Add(de.Value.ToString(), false);
			}
		}
		
		// Constructor
		private ConfigurationInfo()
		{
		}
		
		#endregion

		#region ================== Methods

		/// <summary>
		/// This returns the resource locations as configured.
		/// </summary>
		public DataLocationList GetResources()
		{
			return new DataLocationList(resources);
		}

		// This compares it to other ConfigurationInfo objects
		public int CompareTo(ConfigurationInfo other)
		{
			// Compare
			return name.CompareTo(other.name);
		}

		// This saves the settings to program configuration
		internal void SaveSettings()
		{
			// Write to configuration
			General.Settings.WriteSetting("configurations." + settingskey + ".nodebuildersave", nodebuildersave);
			General.Settings.WriteSetting("configurations." + settingskey + ".nodebuildertest", nodebuildertest);
			General.Settings.WriteSetting("configurations." + settingskey + ".testprogram", testprogram);
			General.Settings.WriteSetting("configurations." + settingskey + ".testparameters", testparameters);
			General.Settings.WriteSetting("configurations." + settingskey + ".testshortpaths", testshortpaths);
			General.Settings.WriteSetting("configurations." + settingskey + ".customparameters", customparameters);
			General.Settings.WriteSetting("configurations." + settingskey + ".testskill", testskill);
			General.Settings.WriteSetting("configurations." + settingskey + ".startmode", startmode);
			resources.WriteToConfig(General.Settings.Config, "configurations." + settingskey + ".resources");
			
			// Write filters to configuration
			General.Settings.DeleteSetting("configurations." + settingskey + ".thingsfilters");
			for(int i = 0; i < thingsfilters.Count; i++)
			{
				thingsfilters[i].WriteSettings(General.Settings.Config,
					"configurations." + settingskey + ".thingsfilters.filter" + i.ToString(CultureInfo.InvariantCulture));
			}

			// Write texturesets to configuration
			for(int i = 0; i < texturesets.Count; i++)
			{
				texturesets[i].WriteToConfig(General.Settings.Config,
					"configurations." + settingskey + ".texturesets.set" + i.ToString(CultureInfo.InvariantCulture));
			}
			
			// Write filters to configuration
			ListDictionary modeslist = new ListDictionary();
			int index = 0;
			foreach(KeyValuePair<string, bool> em in editmodes)
			{
				if(em.Value)
					modeslist.Add(MODE_ENABLED_KEY + index.ToString(CultureInfo.InvariantCulture), em.Key);
				else
					modeslist.Add(MODE_DISABLED_KEY + index.ToString(CultureInfo.InvariantCulture), em.Key);

				index++;
			}
			General.Settings.WriteSetting("configurations." + settingskey + ".editmodes", modeslist);
		}

		// String representation
		public override string ToString()
		{
			return name;
		}

		// This clones the object
		internal ConfigurationInfo Clone()
		{
			ConfigurationInfo ci = new ConfigurationInfo();
			ci.name = this.name;
			ci.filename = this.filename;
			ci.settingskey = this.settingskey;
			ci.nodebuildersave = this.nodebuildersave;
			ci.nodebuildertest = this.nodebuildertest;
			ci.resources = new DataLocationList();
			ci.resources.AddRange(this.resources);
			ci.testprogram = this.testprogram;
			ci.testparameters = this.testparameters;
			ci.testshortpaths = this.testshortpaths;
			ci.customparameters = this.customparameters;
			ci.testskill = this.testskill;
			ci.startmode = this.startmode;
			ci.texturesets = new List<DefinedTextureSet>();
			foreach(DefinedTextureSet s in this.texturesets) ci.texturesets.Add(s.Copy());
			ci.thingsfilters = new List<ThingsFilter>();
			foreach(ThingsFilter f in this.thingsfilters) ci.thingsfilters.Add(new ThingsFilter(f));
			ci.editmodes = new Dictionary<string, bool>(this.editmodes);
			return ci;
		}
		
		// This applies settings from an object
		internal void Apply(ConfigurationInfo ci)
		{
			this.name = ci.name;
			this.filename = ci.filename;
			this.settingskey = ci.settingskey;
			this.nodebuildersave = ci.nodebuildersave;
			this.nodebuildertest = ci.nodebuildertest;
			this.resources = new DataLocationList();
			this.resources.AddRange(ci.resources);
			this.testprogram = ci.testprogram;
			this.testparameters = ci.testparameters;
			this.testshortpaths = ci.testshortpaths;
			this.customparameters = ci.customparameters;
			this.testskill = ci.testskill;
			this.startmode = ci.startmode;
			this.texturesets = new List<DefinedTextureSet>();
			foreach(DefinedTextureSet s in ci.texturesets) this.texturesets.Add(s.Copy());
			this.thingsfilters = new List<ThingsFilter>();
			foreach(ThingsFilter f in ci.thingsfilters) this.thingsfilters.Add(new ThingsFilter(f));
			this.editmodes = new Dictionary<string, bool>(ci.editmodes);
		}
		
		// This applies the defaults
		internal void ApplyDefaults(GameConfiguration gameconfig)
		{
			// Some of the defaults can only be applied from game configuration
			if(gameconfig != null)
			{
				// No nodebuildes set?
				if(nodebuildersave == MISSING_NODEBUILDER) nodebuildersave = gameconfig.DefaultSaveCompiler;
				if(nodebuildertest == MISSING_NODEBUILDER) nodebuildertest = gameconfig.DefaultTestCompiler;
				
				// No texture sets?
				if(texturesets.Count == 0)
				{
					// Copy the default texture sets from the game configuration
					foreach(DefinedTextureSet s in gameconfig.TextureSets)
					{
						// Add a copy to our list
						texturesets.Add(s.Copy());
					}
				}
				
				// No things filters?
				if(thingsfilters.Count == 0)
				{
					// Copy the things filters from game configuration
					foreach(ThingsFilter f in gameconfig.ThingsFilters)
					{
						thingsfilters.Add(new ThingsFilter(f));
					}
				}
			}
			
			// Go for all available editing modes
			foreach(EditModeInfo info in General.Editing.ModesInfo)
			{
				// Is this a mode thats is optional?
				if(info.IsOptional)
				{
					// Add if not listed yet
					if(!editmodes.ContainsKey(info.Type.FullName))
						editmodes.Add(info.Type.FullName, info.Attributes.UseByDefault);
				}
			}
		}
		
		#endregion
	}
}
