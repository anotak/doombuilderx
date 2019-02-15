
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Editing;

#endregion



/*
 * ano-
 * There is a lot of unneeded code duplication in this file!
 * For example many types of map elements have their own function
 * that does essentially the same thing.
 * 
 * If you do not refactor it, please make sure any *new* code
 * avoids those bad patterns.
 */
namespace CodeImp.DoomBuilder.Config
{
	public class GameConfiguration
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Original configuration
		private Configuration cfg;
		
		// General settings
		private string configname;
		private string enginename;
		private float defaulttexturescale;
		private float defaultflatscale;
		private bool scaledtextureoffsets;
		private string defaultsavecompiler;
		private string defaulttestcompiler;
		private string formatinterface;
		private string soundlinedefflag;
		private string singlesidedflag;
		private string doublesidedflag;
		private string impassableflag;
		private string upperunpeggedflag;
		private string lowerunpeggedflag;
		private bool mixtexturesflats;
		private bool generalizedactions;
		private bool generalizedeffects;
		private int start3dmodethingtype;
		private int linedefactivationsfilter;
		private string testparameters;
		private bool testshortpaths;
		private string makedoortrack;
		private int makedooraction;
		private int makedooractivate;
		private int[] makedoorargs;
		private Dictionary<string, bool> makedoorflags;
		private bool linetagindicatesectors;
		private string decorategames;
        private string skyflatname;
		private int maxtexturenamelength;
		private int leftboundary;
		private int rightboundary;
		private int topboundary;
		private int bottomboundary;
		private bool doomlightlevels;
		
		// Skills
		private List<SkillInfo> skills;

		// Map lumps
		private IDictionary maplumpnames;	// This is old, we should use maplumps instead
		private Dictionary<string, MapLumpInfo> maplumps;
		
		// Texture/flat sources
		private IDictionary textureranges;
		private IDictionary flatranges;
		private IDictionary patchranges;
		private IDictionary spriteranges;
		private IDictionary colormapranges;
		
		// Things
		private List<string> defaultthingflags;
		private Dictionary<string, string> thingflags;
		private List<ThingCategory> thingcategories;
		private Dictionary<int, ThingTypeInfo> things;
		private List<FlagTranslation> thingflagstranslation;
		private List<ThingFlagsCompare> thingflagscompare;
        private Dictionary<string, string> thingflagtooltips; // ano

        // Linedefs
        private Dictionary<string, string> linedefflags;
		private List<string> sortedlinedefflags;
		private Dictionary<int, LinedefActionInfo> linedefactions;
		private List<LinedefActionInfo> sortedlinedefactions;
		private List<LinedefActionCategory> actioncategories;
		private List<LinedefActivateInfo> linedefactivates;
		private List<GeneralizedCategory> genactioncategories;
		private List<FlagTranslation> linedefflagstranslation;
        private Dictionary<string, string> linedefflagtooltips; // ano

        // Sectors
        private Dictionary<int, SectorEffectInfo> sectoreffects;
		private List<SectorEffectInfo> sortedsectoreffects;
		private List<GeneralizedOption> geneffectoptions;
		private StepsList brightnesslevels;

		// Universal fields
		private List<UniversalFieldInfo> linedeffields;
		private List<UniversalFieldInfo> sectorfields;
		private List<UniversalFieldInfo> sidedeffields;
		private List<UniversalFieldInfo> thingfields;
		private List<UniversalFieldInfo> vertexfields;
		
		// Enums
		private Dictionary<string, EnumList> enums;
		
		// Defaults
		private List<DefinedTextureSet> texturesets;
		private List<ThingsFilter> thingfilters;
		
		#endregion

		#region ================== Properties

		// General settings
		public string Name { get { return configname; } }
		public string EngineName { get { return enginename; } }
		public string DefaultSaveCompiler { get { return defaultsavecompiler; } }
		public string DefaultTestCompiler { get { return defaulttestcompiler; } }
		public float DefaultTextureScale { get { return defaulttexturescale; } }
		public float DefaultFlatScale { get { return defaultflatscale; } }
		public bool ScaledTextureOffsets { get { return scaledtextureoffsets; } }
		public string FormatInterface { get { return formatinterface; } }
		public string SoundLinedefFlag { get { return soundlinedefflag; } }
		public string SingleSidedFlag { get { return singlesidedflag; } }
		public string DoubleSidedFlag { get { return doublesidedflag; } }
		public string ImpassableFlag { get { return impassableflag; } }
		public string UpperUnpeggedFlag { get { return upperunpeggedflag; } }
		public string LowerUnpeggedFlag { get { return lowerunpeggedflag; } }
		public bool MixTexturesFlats { get { return mixtexturesflats; } }
		public bool GeneralizedActions { get { return generalizedactions; } }
		public bool GeneralizedEffects { get { return generalizedeffects; } }
		public int Start3DModeThingType { get { return start3dmodethingtype; } }
		public int LinedefActivationsFilter { get { return linedefactivationsfilter; } }
		public string TestParameters { get { return testparameters; } }
		public bool TestShortPaths { get { return testshortpaths; } }
		public string MakeDoorTrack { get { return makedoortrack; } }
		public int MakeDoorAction { get { return makedooraction; } }
		public int MakeDoorActivate { get { return makedooractivate; } }
		public Dictionary<string, bool> MakeDoorFlags { get { return makedoorflags; } }
		public int[] MakeDoorArgs { get { return makedoorargs; } }
		public bool LineTagIndicatesSectors { get { return linetagindicatesectors ; } }
		public string DecorateGames { get { return decorategames; } }
        public string SkyFlatName { get { return skyflatname; } }
		public int MaxTextureNamelength { get { return maxtexturenamelength; } }
		public int LeftBoundary { get { return leftboundary; } }
		public int RightBoundary { get { return rightboundary; } }
		public int TopBoundary { get { return topboundary; } }
		public int BottomBoundary { get { return bottomboundary; } }
		public bool DoomLightLevels { get { return doomlightlevels; } }
		
		// Skills
		public List<SkillInfo> Skills { get { return skills; } }
		
		// Map lumps
		public IDictionary MapLumpNames { get { return maplumpnames; } }
		public Dictionary<string, MapLumpInfo> MapLumps { get { return maplumps; } }

		// Texture/flat sources
		public IDictionary TextureRanges { get { return textureranges; } }
		public IDictionary FlatRanges { get { return flatranges; } }
		public IDictionary PatchRanges { get { return patchranges; } }
		public IDictionary SpriteRanges { get { return spriteranges; } }
		public IDictionary ColormapRanges { get { return colormapranges; } }

		// Things
		public ICollection<string> DefaultThingFlags { get { return defaultthingflags; } }
		public IDictionary<string, string> ThingFlags { get { return thingflags; } }
		public List<FlagTranslation> ThingFlagsTranslation { get { return thingflagstranslation; } }
		public List<ThingFlagsCompare> ThingFlagsCompare { get { return thingflagscompare; } }
        public Dictionary<string, string> ThingFlagTooltips { get { return thingflagtooltips; } } // ano
		
		// Linedefs
		public IDictionary<string, string> LinedefFlags { get { return linedefflags; } }
		public List<string> SortedLinedefFlags { get { return sortedlinedefflags; } }
		public IDictionary<int, LinedefActionInfo> LinedefActions { get { return linedefactions; } }
		public List<LinedefActionInfo> SortedLinedefActions { get { return sortedlinedefactions; } }
		public List<LinedefActionCategory> ActionCategories { get { return actioncategories; } }
		public List<LinedefActivateInfo> LinedefActivates { get { return linedefactivates; } }
		public List<GeneralizedCategory> GenActionCategories { get { return genactioncategories; } }
		public List<FlagTranslation> LinedefFlagsTranslation { get { return linedefflagstranslation; } }
        public Dictionary<string, string> LinedefFlagTooltips { get { return linedefflagtooltips; } } // ano

        // Sectors
        public IDictionary<int, SectorEffectInfo> SectorEffects { get { return sectoreffects; } }
		public List<SectorEffectInfo> SortedSectorEffects { get { return sortedsectoreffects; } }
		public List<GeneralizedOption> GenEffectOptions { get { return geneffectoptions; } }
		public StepsList BrightnessLevels { get { return brightnesslevels; } }

        // Universal fields
        public List<UniversalFieldInfo> LinedefFields { get { return linedeffields; } }
		public List<UniversalFieldInfo> SectorFields { get { return sectorfields; } }
		public List<UniversalFieldInfo> SidedefFields { get { return sidedeffields; } }
		public List<UniversalFieldInfo> ThingFields { get { return thingfields; } }
		public List<UniversalFieldInfo> VertexFields { get { return vertexfields; } }

		// Enums
		public IDictionary<string, EnumList> Enums { get { return enums; } }

		// Defaults
		internal List<DefinedTextureSet> TextureSets { get { return texturesets; } }
		public List<ThingsFilter> ThingsFilters { get { return thingfilters; } }
        
        // gzdb plugin interop
        public bool UDMF { get { return formatinterface == "UniversalMapSetIO"; } }
        public bool HEXEN { get { return formatinterface == "HexenMapSetIO"; } }
        public bool DOOM { get { return formatinterface == "DoomMapSetIO"; } }

        #endregion

        #region ================== Constructor / Disposer

        // Constructor
        internal GameConfiguration(Configuration cfg)
		{
			object obj;
			
			// Initialize
			this.cfg = cfg;
			thingflags = new Dictionary<string, string>();
			defaultthingflags = new List<string>();
			thingcategories = new List<ThingCategory>();
			things = new Dictionary<int, ThingTypeInfo>();
            thingflagtooltips = new Dictionary<string, string>(); // ano
            linedefflags = new Dictionary<string, string>();
			sortedlinedefflags = new List<string>();
			linedefactions = new Dictionary<int, LinedefActionInfo>();
            linedefflagtooltips = new Dictionary<string, string>(); // ano
			actioncategories = new List<LinedefActionCategory>();
			sortedlinedefactions = new List<LinedefActionInfo>();
			linedefactivates = new List<LinedefActivateInfo>();
			genactioncategories = new List<GeneralizedCategory>();
			sectoreffects = new Dictionary<int, SectorEffectInfo>();
			sortedsectoreffects = new List<SectorEffectInfo>();
            geneffectoptions = new List<GeneralizedOption>();
			enums = new Dictionary<string, EnumList>();
			skills = new List<SkillInfo>();
			texturesets = new List<DefinedTextureSet>();
			makedoorargs = new int[Linedef.NUM_ARGS];
			maplumps = new Dictionary<string, MapLumpInfo>();
			thingflagstranslation = new List<FlagTranslation>();
			linedefflagstranslation = new List<FlagTranslation>();
			thingfilters = new List<ThingsFilter>();
			thingflagscompare = new List<ThingFlagsCompare>();
			brightnesslevels = new StepsList();
			makedoorflags = new Dictionary<string, bool>();
			
			// Read general settings
			configname = cfg.ReadSetting("game", "<unnamed game>");
			enginename = cfg.ReadSetting("engine", "");
			defaultsavecompiler = cfg.ReadSetting("defaultsavecompiler", "");
			defaulttestcompiler = cfg.ReadSetting("defaulttestcompiler", "");
			defaulttexturescale = cfg.ReadSetting("defaulttexturescale", 1f);
			defaultflatscale = cfg.ReadSetting("defaultflatscale", 1f);
			scaledtextureoffsets = cfg.ReadSetting("scaledtextureoffsets", true);
			formatinterface = cfg.ReadSetting("formatinterface", "");
			mixtexturesflats = cfg.ReadSetting("mixtexturesflats", false);
			generalizedactions = cfg.ReadSetting("generalizedlinedefs", false);
			generalizedeffects = cfg.ReadSetting("generalizedsectors", false);
			start3dmodethingtype = cfg.ReadSetting("start3dmode", 0);
			linedefactivationsfilter = cfg.ReadSetting("linedefactivationsfilter", 0);
			testparameters = cfg.ReadSetting("testparameters", "");
			testshortpaths = cfg.ReadSetting("testshortpaths", false);
			makedoortrack = cfg.ReadSetting("makedoortrack", "-");
			makedooraction = cfg.ReadSetting("makedooraction", 0);
			makedooractivate = cfg.ReadSetting("makedooractivate", 0);
			linetagindicatesectors = cfg.ReadSetting("linetagindicatesectors", false);
			decorategames = cfg.ReadSetting("decorategames", "");
            skyflatname = cfg.ReadSetting("skyflatname", "F_SKY1");
			maxtexturenamelength = cfg.ReadSetting("maxtexturenamelength", 8);
			leftboundary = cfg.ReadSetting("leftboundary", -32768);
			rightboundary = cfg.ReadSetting("rightboundary", 32767);
			topboundary = cfg.ReadSetting("topboundary", 32767);
			bottomboundary = cfg.ReadSetting("bottomboundary", -32768);
			doomlightlevels = cfg.ReadSetting("doomlightlevels", true);
			for(int i = 0; i < Linedef.NUM_ARGS; i++) makedoorargs[i] = cfg.ReadSetting("makedoorarg" + i.ToString(CultureInfo.InvariantCulture), 0);

			// Flags have special (invariant culture) conversion
			// because they are allowed to be written as integers in the configs
			obj = cfg.ReadSettingObject("soundlinedefflag", 0);
			if(obj is int) soundlinedefflag = ((int)obj).ToString(CultureInfo.InvariantCulture); else soundlinedefflag = obj.ToString();
			obj = cfg.ReadSettingObject("singlesidedflag", 0);
			if(obj is int) singlesidedflag = ((int)obj).ToString(CultureInfo.InvariantCulture); else singlesidedflag = obj.ToString();
			obj = cfg.ReadSettingObject("doublesidedflag", 0);
			if(obj is int) doublesidedflag = ((int)obj).ToString(CultureInfo.InvariantCulture); else doublesidedflag = obj.ToString();
			obj = cfg.ReadSettingObject("impassableflag", 0);
			if(obj is int) impassableflag = ((int)obj).ToString(CultureInfo.InvariantCulture); else impassableflag = obj.ToString();
			obj = cfg.ReadSettingObject("upperunpeggedflag", 0);
			if(obj is int) upperunpeggedflag = ((int)obj).ToString(CultureInfo.InvariantCulture); else upperunpeggedflag = obj.ToString();
			obj = cfg.ReadSettingObject("lowerunpeggedflag", 0);
			if(obj is int) lowerunpeggedflag = ((int)obj).ToString(CultureInfo.InvariantCulture); else lowerunpeggedflag = obj.ToString();
			
			// Get map lumps
			maplumpnames = cfg.ReadSetting("maplumpnames", new Hashtable());

			// Get texture and flat sources
			textureranges = cfg.ReadSetting("textures", new Hashtable());
			flatranges = cfg.ReadSetting("flats", new Hashtable());
			patchranges = cfg.ReadSetting("patches", new Hashtable());
			spriteranges = cfg.ReadSetting("sprites", new Hashtable());
			colormapranges = cfg.ReadSetting("colormaps", new Hashtable());
			
			// Map lumps
			LoadMapLumps();
			
			// Skills
			LoadSkills();

			// Enums
			LoadEnums();
			
			// Things
			LoadThingFlags();
			LoadDefaultThingFlags();
			LoadThingCategories();
			
			// Linedefs
			LoadLinedefFlags();
			LoadLinedefActions();
			LoadLinedefActivations();
			LoadLinedefGeneralizedActions();

			// Sectors
			LoadBrightnessLevels();
			LoadSectorEffects();
			LoadSectorGeneralizedEffects();
			
			// Universal fields
			linedeffields = LoadUniversalFields("linedef");
			sectorfields = LoadUniversalFields("sector");
			sidedeffields = LoadUniversalFields("sidedef");
			thingfields = LoadUniversalFields("thing");
			vertexfields = LoadUniversalFields("vertex");

			// Defaults
			LoadTextureSets();
			LoadThingFilters();

			// Make door flags
			LoadMakeDoorFlags();
		}

		// Destructor
		~GameConfiguration()
		{
			foreach(ThingCategory tc in thingcategories) tc.Dispose();
			foreach(LinedefActionCategory ac in actioncategories) ac.Dispose();
			foreach (ThingFlagsCompare tfc in thingflagscompare) tfc.Dispose();
		}
		
		#endregion

		#region ================== Loading
		
		// This loads the map lumps
		private void LoadMapLumps()
		{
			IDictionary dic;
			
			// Get map lumps list
			dic = cfg.ReadSetting("maplumpnames", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Make map lumps
				MapLumpInfo lumpinfo = new MapLumpInfo(de.Key.ToString(), cfg);
				maplumps.Add(de.Key.ToString(), lumpinfo);
			}
		}
		
		// This loads the enumerations
		private void LoadEnums()
		{
			IDictionary dic;

			// Get enums list
			dic = cfg.ReadSetting("enums", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Make new enum
				EnumList list = new EnumList(de.Key.ToString(), cfg);
				enums.Add(de.Key.ToString(), list);
			}
		}
		
		// This loads a universal fields list
		private List<UniversalFieldInfo> LoadUniversalFields(string elementname)
		{
			List<UniversalFieldInfo> list = new List<UniversalFieldInfo>();
			UniversalFieldInfo uf;
			IDictionary dic;
			
			// Get fields
			dic = cfg.ReadSetting("universalfields." + elementname, new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				try
				{
					// Read the field info and add to list
					uf = new UniversalFieldInfo(elementname, de.Key.ToString(), cfg, enums);
					list.Add(uf);
				}
				catch(Exception)
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Unable to read universal field definition 'universalfields." + elementname + "." + de.Key + "' from game configuration '" + this.Name + "'");
				}
			}

			// Return result
			return list;
		}
		
		// Things and thing categories
		private void LoadThingCategories()
		{
			IDictionary dic;
			ThingCategory thingcat;
			
			// Get thing categories
			dic = cfg.ReadSetting("thingtypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				if(de.Value is IDictionary)
				{
					// Make a category
					thingcat = new ThingCategory(cfg, de.Key.ToString(), enums);

					// Add all things in category to the big list
					foreach(ThingTypeInfo t in thingcat.Things)
					{
						if(!things.ContainsKey(t.Index))
						{
							things.Add(t.Index, t);
						}
						else
						{
							General.ErrorLogger.Add(ErrorType.Warning, "Thing number " + t.Index + " is defined more than once (as '" + things[t.Index].Title + "' and '" + t.Title + "') in game configuration '" + this.Name + "'");
						}
					}

					// Add category to list
					thingcategories.Add(thingcat);
				}
			}
		}
		
		// Linedef flags
		private void LoadLinedefFlags()
		{
			IDictionary dic;
			
			// Get linedef flags
			dic = cfg.ReadSetting("linedefflags", new Hashtable());
			foreach(DictionaryEntry de in dic)
				linedefflags.Add(de.Key.ToString(), de.Value.ToString());

            // ano
            dic = cfg.ReadSetting("linedefflagtooltips", new Hashtable());
            foreach (DictionaryEntry de in dic)
                linedefflagtooltips.Add(de.Key.ToString(), de.Value.ToString());

            // Get translations
            dic = cfg.ReadSetting("linedefflagstranslation", new Hashtable());
			foreach(DictionaryEntry de in dic)
				linedefflagstranslation.Add(new FlagTranslation(de));
			
			// Sort flags?
			MapSetIO io = MapSetIO.Create(formatinterface);
			if(io.HasNumericLinedefFlags)
			{
				// Make list for integers that we can sort
				List<int> sortlist = new List<int>(linedefflags.Count);
				foreach(KeyValuePair<string, string> f in linedefflags)
				{
					int num;
					if(int.TryParse(f.Key, NumberStyles.Integer, CultureInfo.InvariantCulture, out num)) sortlist.Add(num);
				}
				
				// Sort
				sortlist.Sort();
				
				// Make list of strings
				foreach(int i in sortlist)
					sortedlinedefflags.Add(i.ToString(CultureInfo.InvariantCulture));
			}
			
			// Sort the flags, because they must be compared highest first!
			linedefflagstranslation.Sort();
		}

		// Linedef actions and action categories
		private void LoadLinedefActions()
		{
			Dictionary<string, LinedefActionCategory> cats = new Dictionary<string, LinedefActionCategory>();
			IDictionary dic;
			LinedefActionInfo ai;
			LinedefActionCategory ac;
			int actionnumber;
			
			// Get linedef categories
			dic = cfg.ReadSetting("linedeftypes", new Hashtable());
			foreach(DictionaryEntry cde in dic)
			{
				if(cde.Value is IDictionary)
				{
					// Read category title
					string cattitle = cfg.ReadSetting("linedeftypes." + cde.Key + ".title", "");

					// Make or get category
					if(cats.ContainsKey(cde.Key.ToString()))
						ac = cats[cde.Key.ToString()];
					else
					{
						ac = new LinedefActionCategory(cde.Key.ToString(), cattitle);
						cats.Add(cde.Key.ToString(), ac);
					}

					// Go for all line types in category
					IDictionary catdic = cfg.ReadSetting("linedeftypes." + cde.Key, new Hashtable());
					foreach(DictionaryEntry de in catdic)
					{
						// Check if the item key is numeric
						if(int.TryParse(de.Key.ToString(),
							NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
							CultureInfo.InvariantCulture, out actionnumber))
						{
							// Check if the item value is a structure
							if(de.Value is IDictionary)
							{
                                if (!linedefactions.ContainsKey(actionnumber))
                                {
                                    // Make the line type
                                    ai = new LinedefActionInfo(actionnumber, cfg, cde.Key.ToString(), enums);

                                    // Add action to category and sorted list
                                    sortedlinedefactions.Add(ai);
                                    linedefactions.Add(actionnumber, ai);
                                    ac.Add(ai);
                                }
                                else
                                {
                                    General.ErrorLogger.Add(ErrorType.Warning, "Duplicate linedef action " + actionnumber +" in game configuration '" + this.Name + "'. with cde.key '" + cde.Key.ToString() + "'. with title '" + cfg.ReadSetting("linedeftypes." + cde.Key.ToString() + "." + actionnumber.ToString(CultureInfo.InvariantCulture) + ".title", "") + "'");
                                }
							}
							else
							{
								// Failure
								if(de.Value != null)
									General.ErrorLogger.Add(ErrorType.Warning, "Structure 'linedeftypes' contains invalid types in game configuration '" + this.Name + "'. All types must be expanded structures.");
							}
						}
					}
				}
			}

			// Sort the actions list
			sortedlinedefactions.Sort();
			
			// Copy categories to final list
			actioncategories.Clear();
			actioncategories.AddRange(cats.Values);

			// Sort the categories list
			actioncategories.Sort();
		}

		// Linedef activates
		private void LoadLinedefActivations()
		{
			IDictionary dic;

			// Get linedef activations
			dic = cfg.ReadSetting("linedefactivations", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Add to the list
				linedefactivates.Add(new LinedefActivateInfo(de.Key.ToString(), de.Value.ToString()));
			}

			// Sort the list
			linedefactivates.Sort();
		}

		// Linedef generalized actions
		private void LoadLinedefGeneralizedActions()
		{
			IDictionary dic;

			// Get linedef activations
			dic = cfg.ReadSetting("gen_linedeftypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Check for valid structure
				if(de.Value is IDictionary)
				{
					// Add category
					genactioncategories.Add(new GeneralizedCategory("gen_linedeftypes", de.Key.ToString(), cfg));
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Structure 'gen_linedeftypes' contains invalid entries in game configuration '" + this.Name + "'");
				}
			}
		}

		// Sector effects
		private void LoadSectorEffects()
		{
			IDictionary dic;
			SectorEffectInfo si;
			int actionnumber;
			
			// Get sector effects
			dic = cfg.ReadSetting("sectortypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Try paring the action number
				if(int.TryParse(de.Key.ToString(),
					NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
					CultureInfo.InvariantCulture, out actionnumber))
				{
					// Make effects
					si = new SectorEffectInfo(actionnumber, de.Value.ToString(), true, false);
					
					// Add action to category and sorted list
					sortedsectoreffects.Add(si);
					sectoreffects.Add(actionnumber, si);
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Structure 'sectortypes' contains invalid keys in game configuration '" + this.Name + "'");
				}
			}

			// Sort the actions list
			sortedsectoreffects.Sort();
		}

		// Brightness levels
		private void LoadBrightnessLevels()
		{
			IDictionary dic;
			int level;

			// Get brightness levels structure
			dic = cfg.ReadSetting("sectorbrightness", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Try paring the level
				if(int.TryParse(de.Key.ToString(),
					NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
					CultureInfo.InvariantCulture, out level))
				{
					brightnesslevels.Add(level);
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Structure 'sectorbrightness' contains invalid keys in game configuration '" + this.Name + "'");
				}
			}

			// Sort the list
			brightnesslevels.Sort();
		}

		// Sector generalized effects
		private void LoadSectorGeneralizedEffects()
		{
			IDictionary dic;

			// Get sector effects
			dic = cfg.ReadSetting("gen_sectortypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Check for valid structure
				if(de.Value is IDictionary)
				{
					// Add option
					geneffectoptions.Add(new GeneralizedOption("gen_sectortypes", "", de.Key.ToString(), de.Value as IDictionary));
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Structure 'gen_sectortypes' contains invalid entries in game configuration '" + this.Name + "'");
				}
			}
		}

		// Thing flags
		private void LoadThingFlags()
		{
			IDictionary dic;

			// Get thing flags
			dic = cfg.ReadSetting("thingflags", new Hashtable());
			foreach (DictionaryEntry de in dic)
				thingflags.Add(de.Key.ToString(), de.Value.ToString());

            // ano
            dic = cfg.ReadSetting("thingflagtooltips", new Hashtable());
            foreach (DictionaryEntry de in dic)
                thingflagtooltips.Add(de.Key.ToString(), de.Value.ToString());

            // Get translations
            dic = cfg.ReadSetting("thingflagstranslation", new Hashtable());
			foreach(DictionaryEntry de in dic)
				thingflagstranslation.Add(new FlagTranslation(de));

			// Get thing compare flag info (for the stuck thing error checker
			dic = cfg.ReadSetting("thingflagscompare", new Hashtable());
			foreach (DictionaryEntry de in dic)
			{
				IDictionary gdic = cfg.ReadSetting("thingflagscompare." + de.Key, new Hashtable());

				foreach (DictionaryEntry gde in gdic)
				{
					thingflagscompare.Add(new ThingFlagsCompare(cfg, de.Key.ToString(), gde.Key.ToString()));
				}
			}

			// Sort the translation flags, because they must be compared highest first!
			thingflagstranslation.Sort();
		}

		// Default thing flags
		private void LoadDefaultThingFlags()
		{
			IDictionary dic;

			// Get linedef flags
			dic = cfg.ReadSetting("defaultthingflags", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Check if flag exists
				if(thingflags.ContainsKey(de.Key.ToString()))
				{
					defaultthingflags.Add(de.Key.ToString());
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Structure 'defaultthingflags' contains unknown thing flags in game configuration '" + this.Name + "'");
				}
			}
		}

		// Skills
		private void LoadSkills()
		{
			IDictionary dic;

			// Get skills
			dic = cfg.ReadSetting("skills", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				int num = 0;
				if(int.TryParse(de.Key.ToString(), out num))
				{
					skills.Add(new SkillInfo(num, de.Value.ToString()));
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Structure 'skills' contains invalid skill numbers in game configuration '" + this.Name + "'");
				}
			}
		}
		
		// Texture Sets
		private void LoadTextureSets()
		{
			IDictionary dic;

			// Get sets
			dic = cfg.ReadSetting("texturesets", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				DefinedTextureSet s = new DefinedTextureSet(cfg, "texturesets." + de.Key.ToString());
				texturesets.Add(s);
			}
		}
		
		// Thing Filters
		private void LoadThingFilters()
		{
			IDictionary dic;

			// Get sets
			dic = cfg.ReadSetting("thingsfilters", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				ThingsFilter f = new ThingsFilter(cfg, "thingsfilters." + de.Key.ToString());
				thingfilters.Add(f);
			}
		}

		// Make door flags
		private void LoadMakeDoorFlags()
		{
			IDictionary dic;

			dic = cfg.ReadSetting("makedoorflags", new Hashtable());
			foreach (DictionaryEntry de in dic)
			{
				// Using minus will unset the flag
				if (de.Key.ToString()[0] == '-')
				{
					makedoorflags[de.Key.ToString().TrimStart('-')] = false;
				}
				else
				{
					makedoorflags[de.Key.ToString()] = true;
				}
			}
		}
		
		#endregion

		#region ================== Methods

		// ReadSetting
		public string ReadSetting(string setting, string defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public int ReadSetting(string setting, int defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public float ReadSetting(string setting, float defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public short ReadSetting(string setting, short defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public long ReadSetting(string setting, long defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public bool ReadSetting(string setting, bool defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public byte ReadSetting(string setting, byte defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public IDictionary ReadSetting(string setting, IDictionary defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		
		// This gets a list of things categories
		internal List<ThingCategory> GetThingCategories()
		{
			return new List<ThingCategory>(thingcategories);
		}
		
		// This gets a list of things
		internal Dictionary<int, ThingTypeInfo> GetThingTypes()
		{
			return new Dictionary<int, ThingTypeInfo>(things);
		}
		
		// This checks if an action is generalized or predefined
		public static bool IsGeneralized(int action, List<GeneralizedCategory> categories)
		{
			// Only actions above 0
			if(action > 0)
			{
				// Go for all categories
				foreach(GeneralizedCategory ac in categories)
				{
					// Check if the action is within range of this category
					if((action >= ac.Offset) && (action < (ac.Offset + ac.Length))) return true;
				}
			}

			// Not generalized
			return false;
		}

		// This gets the generalized action category from action number
		public GeneralizedCategory GetGeneralizedActionCategory(int action)
		{
			// Only actions above 0
			if(action > 0)
			{
				// Go for all categories
				foreach(GeneralizedCategory ac in genactioncategories)
				{
					// Check if the action is within range of this category
					if((action >= ac.Offset) && (action < (ac.Offset + ac.Length))) return ac;
				}
			}

			// Not generalized
			return null;
		}

        // gzdb cross compat
        //mxd
        public SectorEffectData GetSectorEffectData(int effect) { return GetSectorEffectData(effect, General.Map.Config.GenEffectOptions); }
        public SectorEffectData GetSectorEffectData(int effect, List<GeneralizedOption> options)
        {
            SectorEffectData result = new SectorEffectData();
            if (effect > 0)
            {
                int cureffect = effect;

                if (General.Map.Config.GeneralizedEffects)
                {
                    for (int i = options.Count - 1; i > -1; i--)
                    {
                        for (int j = options[i].Bits.Count - 1; j > -1; j--)
                        {
                            GeneralizedBit bit = options[i].Bits[j];
                            if (bit.Index > 0 && (cureffect & bit.Index) == bit.Index)
                            {
                                cureffect -= bit.Index;
                                result.GeneralizedBits.Add(bit.Index);
                            }
                        }
                    }
                }

                if (cureffect > 0) result.Effect = cureffect;
            }

            return result;
        }

        // This checks if a specific edit mode class is listed
        public bool IsEditModeSpecified(string classname)
		{
			return cfg.SettingExists("editingmodes." + classname.ToString(CultureInfo.InvariantCulture));
		}
		
		// This returns information on a linedef type
		public LinedefActionInfo GetLinedefActionInfo(int action)
		{
			// Known type?
			if(linedefactions.ContainsKey(action))
			{
				return linedefactions[action];
			}
			else if(action == 0)
			{
				return new LinedefActionInfo(0, "None", true, false);
			}
			else if(IsGeneralized(action, genactioncategories))
			{
				return new LinedefActionInfo(action, "Generalized (" + GetGeneralizedActionCategory(action) + ")", true, true);
			}
			else
			{
				return new LinedefActionInfo(action, "Unknown", false, false);
			}
		}

		// This returns information on a sector effect
		public SectorEffectInfo GetSectorEffectInfo(int effect)
		{
			// Known type?
			if(sectoreffects.ContainsKey(effect))
			{
				return sectoreffects[effect];
			}
			else if(effect == 0)
			{
				return new SectorEffectInfo(0, "None", true, false);
			}
			else
			{
				return new SectorEffectInfo(effect, "Unknown", false, false);
			}
		}
		
		#endregion
	}
}
