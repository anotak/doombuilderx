
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
using System.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal class UniversalStreamReader
	{
		#region ================== Constants

		// Name of the UDMF configuration file
		private const string UDMF_CONFIG_NAME = "UDMF.cfg";

		#endregion

		#region ================== Variables

		private Configuration config;
		private bool setknowncustomtypes;
		private bool strictchecking = true;
		
		#endregion

		#region ================== Properties

		public bool SetKnownCustomTypes { get { return setknowncustomtypes; } set { setknowncustomtypes = value; } }
		public bool StrictChecking { get { return strictchecking; } set { strictchecking = value; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public UniversalStreamReader()
		{
			// Make configuration
			config = new Configuration();
			
			// Find a resource named UDMF.cfg
			string[] resnames = General.ThisAssembly.GetManifestResourceNames();
			foreach(string rn in resnames)
			{
				// Found it?
				if(rn.EndsWith(UDMF_CONFIG_NAME, StringComparison.InvariantCultureIgnoreCase))
				{
					// Get a stream from the resource
					Stream udmfcfg = General.ThisAssembly.GetManifestResourceStream(rn);
					StreamReader udmfcfgreader = new StreamReader(udmfcfg, Encoding.ASCII);

					// Load configuration from stream
					config.InputConfiguration(udmfcfgreader.ReadToEnd());

					// Now we add the linedef flags, activations and thing flags
					// to this list, so that these don't show up in the custom
					// fields list either. We use true as dummy value (it has no meaning)

					// Add linedef flags
					foreach(KeyValuePair<string, string> flag in General.Map.Config.LinedefFlags)
						config.WriteSetting("managedfields.linedef." + flag.Key, true);

					// Add linedef activations
					foreach(LinedefActivateInfo activate in General.Map.Config.LinedefActivates)
						config.WriteSetting("managedfields.linedef." + activate.Key, true);
					
					// Add linedef flag translations
					foreach(FlagTranslation f in General.Map.Config.LinedefFlagsTranslation)
					{
						foreach(string fn in f.Fields)
							config.WriteSetting("managedfields.linedef." + fn, true);
					}

					// Add thing flags
					foreach(KeyValuePair<string, string> flag in General.Map.Config.ThingFlags)
						config.WriteSetting("managedfields.thing." + flag.Key, true);

					// Add thing flag translations
					foreach(FlagTranslation f in General.Map.Config.ThingFlagsTranslation)
					{
						foreach(string fn in f.Fields)
							config.WriteSetting("managedfields.thing." + fn, true);
					}

					// Done
					udmfcfgreader.Dispose();
					udmfcfg.Dispose();
					break;
				}
			}
		}

		#endregion

		#region ================== Reading

		// This reads from a stream
		public MapSet Read(MapSet map, Stream stream)
		{
			StreamReader reader = new StreamReader(stream, Encoding.ASCII);
			Dictionary<int, Vertex> vertexlink;
			Dictionary<int, Sector> sectorlink;
			UniversalParser textmap = new UniversalParser();
			textmap.StrictChecking = strictchecking;
			
			try
			{
				// Read UDMF from stream
				textmap.InputConfiguration(reader.ReadToEnd());

				// Check for errors
				if(textmap.ErrorResult != 0)
				{
					// Show parse error
					General.ShowErrorMessage("Error on line " + textmap.ErrorLine + " while parsing UDMF map data:\n" + textmap.ErrorDescription, MessageBoxButtons.OK);
				}
				else
				{
					// Read the map
					vertexlink = ReadVertices(map, textmap);
					sectorlink = ReadSectors(map, textmap);
					ReadLinedefs(map, textmap, vertexlink, sectorlink);
					ReadThings(map, textmap);
				}
			}
			catch(Exception e)
			{
				General.ShowErrorMessage("Unexpected error reading UDMF map data. " + e.GetType().Name + ": " + e.Message, MessageBoxButtons.OK);
			}

			return map;
		}

		// This reads the things
		private void ReadThings(MapSet map, UniversalParser textmap)
		{
			// Get list of entries
			List<UniversalCollection> collections = GetNamedCollections(textmap.Root, "thing");

			// Go for all collections
			map.SetCapacity(0, 0, 0, 0, map.Things.Count + collections.Count);
			for(int i = 0; i < collections.Count; i++)
			{
				// Read fields
				UniversalCollection c = collections[i];
				int[] args = new int[Linedef.NUM_ARGS];
				string where = "thing " + i;
				float x = GetCollectionEntry<float>(c, "x", true, 0.0f, where);
				float y = GetCollectionEntry<float>(c, "y", true, 0.0f, where);
				float height = GetCollectionEntry<float>(c, "height", false, 0.0f, where);
				int tag = GetCollectionEntry<int>(c, "id", false, 0, where);
				int angledeg = GetCollectionEntry<int>(c, "angle", false, 0, where);
				int type = GetCollectionEntry<int>(c, "type", true, 0, where);
				int special = GetCollectionEntry<int>(c, "special", false, 0, where);
				args[0] = GetCollectionEntry<int>(c, "arg0", false, 0, where);
				args[1] = GetCollectionEntry<int>(c, "arg1", false, 0, where);
				args[2] = GetCollectionEntry<int>(c, "arg2", false, 0, where);
				args[3] = GetCollectionEntry<int>(c, "arg3", false, 0, where);
				args[4] = GetCollectionEntry<int>(c, "arg4", false, 0, where);

				// Flags
				Dictionary<string, bool> stringflags = new Dictionary<string, bool>();
				foreach(KeyValuePair<string, string> flag in General.Map.Config.ThingFlags)
					stringflags[flag.Key] = GetCollectionEntry<bool>(c, flag.Key, false, false, where);
				foreach(FlagTranslation ft in General.Map.Config.ThingFlagsTranslation)
				{
					foreach(string field in ft.Fields)
						stringflags[field] = GetCollectionEntry<bool>(c, field, false, false, where);
				}

				// Create new item
				Thing t = map.CreateThing();
				if(t != null)
				{
					t.Update(type, x, y, height, angledeg, stringflags, tag, special, args);

					// Custom fields
					ReadCustomFields(c, t, "thing");
				}
			}
		}

		// This reads the linedefs and sidedefs
		private void ReadLinedefs(MapSet map, UniversalParser textmap,
			Dictionary<int, Vertex> vertexlink, Dictionary<int, Sector> sectorlink)
		{
			// Get list of entries
			List<UniversalCollection> linescolls = GetNamedCollections(textmap.Root, "linedef");
			List<UniversalCollection> sidescolls = GetNamedCollections(textmap.Root, "sidedef");

			// Go for all lines
			map.SetCapacity(0, map.Linedefs.Count + linescolls.Count, map.Sidedefs.Count + sidescolls.Count, 0, 0);
			for(int i = 0; i < linescolls.Count; i++)
			{
				// Read fields
				UniversalCollection lc = linescolls[i];
				int[] args = new int[Linedef.NUM_ARGS];
				string where = "linedef " + i;
				int tag = GetCollectionEntry<int>(lc, "id", false, 0, where);
				int v1 = GetCollectionEntry<int>(lc, "v1", true, 0, where);
				int v2 = GetCollectionEntry<int>(lc, "v2", true, 0, where);
				int special = GetCollectionEntry<int>(lc, "special", false, 0, where);
				args[0] = GetCollectionEntry<int>(lc, "arg0", false, 0, where);
				args[1] = GetCollectionEntry<int>(lc, "arg1", false, 0, where);
				args[2] = GetCollectionEntry<int>(lc, "arg2", false, 0, where);
				args[3] = GetCollectionEntry<int>(lc, "arg3", false, 0, where);
				args[4] = GetCollectionEntry<int>(lc, "arg4", false, 0, where);
				int s1 = GetCollectionEntry<int>(lc, "sidefront", true, -1, where);
				int s2 = GetCollectionEntry<int>(lc, "sideback", false, -1, where);

				// Flags
				Dictionary<string, bool> stringflags = new Dictionary<string, bool>();
				foreach(KeyValuePair<string, string> flag in General.Map.Config.LinedefFlags)
					stringflags[flag.Key] = GetCollectionEntry<bool>(lc, flag.Key, false, false, where);
				foreach(FlagTranslation ft in General.Map.Config.LinedefFlagsTranslation)
				{
					foreach(string field in ft.Fields)
						stringflags[field] = GetCollectionEntry<bool>(lc, field, false, false, where);
				}
				
				// Activations
				foreach(LinedefActivateInfo activate in General.Map.Config.LinedefActivates)
					stringflags[activate.Key] = GetCollectionEntry<bool>(lc, activate.Key, false, false, where);
				
				// Create new linedef
				if(vertexlink.ContainsKey(v1) && vertexlink.ContainsKey(v2))
				{
					// Check if not zero-length
					if(Vector2D.ManhattanDistance(vertexlink[v1].Position, vertexlink[v2].Position) > 0.0001f)
					{
						Linedef l = map.CreateLinedef(vertexlink[v1], vertexlink[v2]);
						if(l != null)
						{
							l.Update(stringflags, 0, tag, special, args);
							l.UpdateCache();

							// Custom fields
							ReadCustomFields(lc, l, "linedef");

							// Read sidedefs and connect them to the line
							if(s1 > -1)
							{
								if(s1 < sidescolls.Count)
									ReadSidedef(map, sidescolls[s1], l, true, sectorlink, s1);
								else
									General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references invalid front sidedef " + s1 + ". Sidedef has been removed.");
							}

							if(s2 > -1)
							{
								if(s2 < sidescolls.Count)
									ReadSidedef(map, sidescolls[s2], l, false, sectorlink, s2);
								else
									General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references invalid back sidedef " + s1 + ". Sidedef has been removed.");
							}
						}
					}
					else
					{
						General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " is zero-length. Linedef has been removed.");
					}
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references one or more invalid vertices. Linedef has been removed.");
				}
			}
		}

		// This reads a single sidedef and connects it to the given linedef
		private void ReadSidedef(MapSet map, UniversalCollection sc, Linedef ld,
								 bool front, Dictionary<int, Sector> sectorlink, int index)
		{
			// Read fields
			string where = "linedef " + ld.Index + (front ? " front sidedef " : " back sidedef ") + index;
			int offsetx = GetCollectionEntry<int>(sc, "offsetx", false, 0, where);
			int offsety = GetCollectionEntry<int>(sc, "offsety", false, 0, where);
			string thigh = GetCollectionEntry<string>(sc, "texturetop", false, "-", where);
			string tlow = GetCollectionEntry<string>(sc, "texturebottom", false, "-", where);
			string tmid = GetCollectionEntry<string>(sc, "texturemiddle", false, "-", where);
			int sector = GetCollectionEntry<int>(sc, "sector", true, 0, where);

			// Create sidedef
			if(sectorlink.ContainsKey(sector))
			{
				Sidedef s = map.CreateSidedef(ld, front, sectorlink[sector]);
				if(s != null)
				{
					s.Update(offsetx, offsety, thigh, tmid, tlow);

					// Custom fields
					ReadCustomFields(sc, s, "sidedef");
				}
			}
			else
			{
				General.ErrorLogger.Add(ErrorType.Warning, "Sidedef references invalid sector " + sector + ". Sidedef has been removed.");
			}
		}

		// This reads the sectors
		private Dictionary<int, Sector> ReadSectors(MapSet map, UniversalParser textmap)
		{
			Dictionary<int, Sector> link;

			// Get list of entries
			List<UniversalCollection> collections = GetNamedCollections(textmap.Root, "sector");

			// Create lookup table
			link = new Dictionary<int, Sector>(collections.Count);

			// Go for all collections
			map.SetCapacity(0, 0, 0, map.Sectors.Count + collections.Count, 0);
			for(int i = 0; i < collections.Count; i++)
			{
				// Read fields
				UniversalCollection c = collections[i];
				string where = "sector " + i;
				int hfloor = GetCollectionEntry<int>(c, "heightfloor", false, 0, where);
				int hceil = GetCollectionEntry<int>(c, "heightceiling", false, 0, where);
				string tfloor = GetCollectionEntry<string>(c, "texturefloor", true, "-", where);
				string tceil = GetCollectionEntry<string>(c, "textureceiling", true, "-", where);
				int bright = GetCollectionEntry<int>(c, "lightlevel", false, 160, where);
				int special = GetCollectionEntry<int>(c, "special", false, 0, where);
				int tag = GetCollectionEntry<int>(c, "id", false, 0, where);

				// Create new item
				Sector s = map.CreateSector();
				if(s != null)
				{
					s.Update(hfloor, hceil, tfloor, tceil, special, tag, bright);

					// Custom fields
					ReadCustomFields(c, s, "sector");

					// Add it to the lookup table
					link.Add(i, s);
				}
			}

			// Return lookup table
			return link;
		}

		// This reads the vertices
		private Dictionary<int, Vertex> ReadVertices(MapSet map, UniversalParser textmap)
		{
			Dictionary<int, Vertex> link;

			// Get list of entries
			List<UniversalCollection> collections = GetNamedCollections(textmap.Root, "vertex");

			// Create lookup table
			link = new Dictionary<int, Vertex>(collections.Count);

			// Go for all collections
			map.SetCapacity(map.Vertices.Count + collections.Count, 0, 0, 0, 0);
			for(int i = 0; i < collections.Count; i++)
			{
				// Read fields
				UniversalCollection c = collections[i];
				string where = "vertex " + i;
				float x = GetCollectionEntry<float>(c, "x", true, 0.0f, where);
				float y = GetCollectionEntry<float>(c, "y", true, 0.0f, where);

				// Create new item
				Vertex v = map.CreateVertex(new Vector2D(x, y));
				if(v != null)
				{
					// Custom fields
					ReadCustomFields(c, v, "vertex");

					// Add it to the lookup table
					link.Add(i, v);
				}
			}

			// Return lookup table
			return link;
		}

		// This reads custom fields from a collection and adds them to a map element
		private void ReadCustomFields(UniversalCollection collection, MapElement element, string elementname)
		{
			element.Fields.BeforeFieldsChange();
			
			// Go for all the elements in the collection
			foreach(UniversalEntry e in collection)
			{
				// Check if not a managed field
				if(!config.SettingExists("managedfields." + elementname + "." + e.Key))
				{
					int type = (int)UniversalType.Integer;

					// Determine default type
					if(e.Value.GetType() == typeof(int)) type = (int)UniversalType.Integer;
					else if(e.Value.GetType() == typeof(float)) type = (int)UniversalType.Float;
					else if(e.Value.GetType() == typeof(bool)) type = (int)UniversalType.Boolean;
					else if(e.Value.GetType() == typeof(string)) type = (int)UniversalType.String;

					// Try to find the type from configuration
					if(setknowncustomtypes)
						type = General.Map.Options.GetUniversalFieldType(elementname, e.Key, type);

					// Make custom field
					element.Fields[e.Key] = new UniValue(type, e.Value);
				}
			}
		}

		// This validates and returns an entry
		private T GetCollectionEntry<T>(UniversalCollection c, string entryname, bool required, T defaultvalue, string where)
		{
			T result = default(T);
			bool found = false;

			// Find the entry
			foreach(UniversalEntry e in c)
			{
				// Check if matches
				if(e.Key == entryname)
				{
					// Let's be kind and cast any int to a float if needed
					if((typeof(T) == typeof(float)) &&
					   (e.Value.GetType() == typeof(int)))
					{
						// Make it a float
						object fvalue = (float)(int)e.Value;
						result = (T)fvalue;
					}
					else
					{
						// Verify type
						e.ValidateType(typeof(T));

						// Found it!
						result = (T)e.Value;
					}

					// Done
					found = true;
				}
			}

			// Not found?
			if(!found)
			{
				// Report error when entry is required!
				if(required)
					General.ErrorLogger.Add(ErrorType.Error, "Error while reading UDMF map data: Missing required field '" + entryname + "' at " + where + ".");

				// Make default entry
				result = defaultvalue;
			}

			// Return result
			return result;
		}

		// This makes a list of all collections with the given name
		private List<UniversalCollection> GetNamedCollections(UniversalCollection collection, string entryname)
		{
			List<UniversalCollection> list = new List<UniversalCollection>();

			// Make list
			foreach(UniversalEntry e in collection)
				if((e.Value is UniversalCollection) && (e.Key == entryname)) list.Add(e.Value as UniversalCollection);

			return list;
		}

		#endregion
	}
}

