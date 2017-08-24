
#region ================== Copyright (c) 2012 Pascal vd Heiden

/*
 * Copyright (c) 2012 Pascal vd Heiden, www.codeimp.com
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Plugins.RejectEditor
{
	public class BuilderPlug : Plug
	{
		#region ================== Constants

		public const string REJECT_FIELD_NAME = "editor_rejectstates";

		#endregion

		#region ================== Variables

		// Objects
		private static BuilderPlug me;
		private Dictionary<int, Sector> fixedindexsectors;
		private Dictionary<int, List<RejectSet>> changes;
		private bool[][] unmodifiedtable;

		#endregion

		#region ================== Properties

		// Properties
		public static BuilderPlug Me { get { return me; } }
		public override string Name { get { return "RejectEditor"; } }
		public override int MinimumRevision { get { return 1631; } }
		public Dictionary<int, List<RejectSet>> RejectChanges { get { return changes; } }
		public bool[][] UnmodifiedTable { get { return unmodifiedtable; } }

		#endregion

		#region ================== Initialize / Dispose

		// This event is called when the plugin is initialized
		public override void OnInitialize()
		{
			base.OnInitialize();

			General.Actions.BindMethods(this);

			// Keep a static reference
			me = this;
		}

		// Preferences changed
		public override void OnClosePreferences(PreferencesController controller)
		{
			base.OnClosePreferences(controller);
		}

		// This is called when the plugin is terminated
		public override void Dispose()
		{
			// Clean up
			General.Actions.UnbindMethods(this);
			base.Dispose();
		}

		#endregion

		#region ================== Methods

		// Load the reject table
		public bool[][] LoadTabel()
		{
			// Load the reject data
			using(MemoryStream stream = General.Map.GetLumpData("REJECT"))
			{
				byte[] data = new byte[stream.Length];
				stream.Read(data, 0, (int)stream.Length);

				// If the data does not cover all sectors, resize the array
				// to match (extra bytes are initialied with zeros)
				int numsectors = General.Map.Map.Sectors.Count;
				int requiredsize = (int)Math.Ceiling((double)(numsectors * numsectors) / 8.0);
				if(data.Length > requiredsize) return null;
				if(data.Length < requiredsize) Array.Resize(ref data, requiredsize);

				// Expand the table
				int databit = 0;
				bool[][] table = new bool[numsectors][];
				for(int ss = 0; ss < table.Length; ss++)
				{
					table[ss] = new bool[numsectors];
					bool[] sectors = table[ss];
					for(int ts = 0; ts < sectors.Length; ts++)
					{
						int byteindex = databit >> 3;
						int bitmask = 1 << (databit & 0x07);
						sectors[ts] = (data[byteindex] & bitmask) != 0;
						databit++;
					}
				}

				return table;
			}
		}

		// Store a reject table
		public void StoreTable(bool[][] table)
		{
			// Allocate data memory
			int numsectors = General.Map.Map.Sectors.Count;
			int requiredsize = (int)Math.Ceiling((double)(numsectors * numsectors) / 8.0);
			byte[] data = new byte[requiredsize];

			// Compress the table
			int databit = 0;
			foreach(bool[] sectors in table)
			{
				foreach(bool s in sectors)
				{
					if(s)
					{
						int byteindex = databit >> 3;
						int bitmask = 1 << (databit & 0x07);
						data[byteindex] = (byte)(data[byteindex] | bitmask);
					}
					databit++;
				}
			}

			// Store data into REJECT lump
			using(MemoryStream stream = new MemoryStream(data))
			{
				General.Map.SetLumpData("REJECT", stream);
			}
		}

		public void UpdateFixedIndexLookupTable()
		{
			fixedindexsectors = new Dictionary<int, Sector>(General.Map.Map.Sectors.Count);
			foreach(Sector s in General.Map.Map.Sectors)
			{
				fixedindexsectors.Add(s.FixedIndex, s);
			}
		}

		public Sector GetSectorByFixedIndex(int fixedindex)
		{
			Sector result;
			if(fixedindexsectors.TryGetValue(fixedindex, out result))
				return result;
			else
				return null;
		}

		/// <summary>
		/// During editing in other modes, some sectors may be removed.
		/// This method cleans up any reject states pointing to sectors which no longer exist.
		/// This also updates the fixed index lookup table.
		/// </summary>
		public void CleanUpRejectStates()
		{
			UpdateFixedIndexLookupTable();

			List<int> sourcesectors = new List<int>(changes.Keys);
			foreach(int ss in sourcesectors)
			{
				List<RejectSet> states = changes[ss];

				// Remove any items for which the target sector can't be found
				for(int i = states.Count - 1; i >= 0; i--)
				{
					if(GetSectorByFixedIndex(states[i].target) == null)
						states.RemoveAt(i);
				}

				// If no states remaining, remove the source sector and list from lookup table
				if(states.Count == 0)
					changes.Remove(ss);
			}
		}

		// This decodes a reject state string, which has the following format:
		// sourcesector|targetsector=rejectstate,targetsector=rejectstate,targetsector=rejectstate, ...
		private List<RejectSet> DecodeRejectString(string str, out int sourcesector)
		{
			sourcesector = -1;
			try
			{
				List<RejectSet> result = new List<RejectSet>();

				// Cut off the sourcesector part
				int separator = Array.IndexOf(str.ToCharArray(), '|');
				sourcesector = int.Parse(str.Substring(0, separator), CultureInfo.InvariantCulture);
				str = str.Substring(separator + 1);

				// Translate sector index to fixed index
				sourcesector = General.Map.Map.GetSectorByIndex(sourcesector).FixedIndex;

				// Process the target sector states
				string[] targets = str.Split(',');
				foreach(string t in targets)
				{
					// Split target sector and state
					separator = t.IndexOf('=');
					int targetsector = int.Parse(t.Substring(0, separator), CultureInfo.InvariantCulture);
					RejectState rejectstate = (RejectState)int.Parse(t.Substring(separator + 1), CultureInfo.InvariantCulture);

					// Translate sector index to fixed index
					targetsector = General.Map.Map.GetSectorByIndex(targetsector).FixedIndex;

					result.Add(new RejectSet(targetsector, rejectstate));
				}
				return result;
			}
			catch(Exception)
			{
				if(sourcesector > -1)
					General.ErrorLogger.Add(ErrorType.Error, "The reject information for sector " + sourcesector + " is corrupt.");
				else
					General.ErrorLogger.Add(ErrorType.Error, "Some or all of the reject information is corrupt.");

				return null;
			}
		}

		// This encodes a reject state string
		private string EncodeRejectString(List<RejectSet> rejectstates, int sourcesector)
		{
			// Translate fixed index to sector index
			sourcesector = GetSectorByFixedIndex(sourcesector).Index;

			StringBuilder str = new StringBuilder(rejectstates.Count * 10 + 10);
			foreach(RejectSet s in rejectstates)
			{
				// Translate fixed index to sector index
				int targetindex = GetSectorByFixedIndex(s.target).Index;

				// Add the target sector and state
				if(str.Length > 0) str.Append(",");
				str.Append(targetindex.ToString(CultureInfo.InvariantCulture));
				str.Append("=");
				str.Append(((int)s.state).ToString(CultureInfo.InvariantCulture));
			}

			return sourcesector.ToString(CultureInfo.InvariantCulture) + '|' + str;
		}

		#endregion

		#region ================== Events

		// When a map is opened
		public override void OnMapOpenEnd()
		{
			base.OnMapOpenEnd();

			UpdateFixedIndexLookupTable();

			int numchanges = General.Map.Options.ReadPluginSetting("numchanges", 0);
			changes = new Dictionary<int, List<RejectSet>>();
			for(int i = 0; i < numchanges; i++)
			{
				// Read the reject states string
				string rejectstr = General.Map.Options.ReadPluginSetting("s" + i.ToString(CultureInfo.InvariantCulture), string.Empty);

				// Decode the string
				int sourcesector;
				List<RejectSet> states = DecodeRejectString(rejectstr, out sourcesector);

				// Add to lookup table by source sector
				changes.Add(sourcesector, states);
			}
		}

		// When a map is saved
		public override void OnMapSaveBegin(SavePurpose purpose)
		{
			base.OnMapSaveBegin(purpose);

			if(purpose == SavePurpose.Testing) return;

			CleanUpRejectStates();

			// Write the reject states to the map config
			General.Map.Options.WritePluginSetting("numchanges", changes.Count);
			int index = 0;
			foreach(KeyValuePair<int, List<RejectSet>> rs in changes)
			{
				// Encode the reject settings for this source sector
				string rejectstr = EncodeRejectString(rs.Value, rs.Key);

				// Store in map config
				General.Map.Options.WritePluginSetting("s" + index.ToString(CultureInfo.InvariantCulture), rejectstr);
				index++;
			}
		}

		// When the nodes have been rebuilt
		public override void OnMapNodesRebuilt()
		{
			base.OnMapNodesRebuilt();

			CleanUpRejectStates();

			// Did the nodebuilder make a reject table?
			if(!General.Map.LumpExists("REJECT"))
			{
				General.ErrorLogger.Add(ErrorType.Warning, "Unable to find the REJECT table. Check your nodebuilder settings and make sure you are using a nodebuilder that generates a reject table.");
				return;
			}

			// Load the reject table and make a copy
			unmodifiedtable = LoadTabel();
			if(unmodifiedtable == null)
			{
				General.ErrorLogger.Add(ErrorType.Warning, "The nodebuilder is generating an invalid REJECT table. Check your nodebuilder settings and make sure you are using a nodebuilder that generates a proper reject table.");
				return;
			}

			// Make a copy of the table
			int numsectors = General.Map.Map.Sectors.Count;
			bool[][] table = new bool[numsectors][];
			for(int i = 0; i < numsectors; i++)
			{
				table[i] = new bool[numsectors];
				Array.Copy(unmodifiedtable[i], table[i], numsectors);
			}

			// Apply changes to the reject table
			foreach(KeyValuePair<int, List<RejectSet>> ss in changes)
			{
				Sector ssector = GetSectorByFixedIndex(ss.Key);
				foreach(RejectSet ts in ss.Value)
				{
					Sector tsector = GetSectorByFixedIndex(ts.target);
					if(tsector != null)
					{
						switch(ts.state)
						{
							case RejectState.ForceVisible: table[ssector.Index][tsector.Index] = false; break;
							case RejectState.ForceHidden: table[ssector.Index][tsector.Index] = true; break;
						}
					}
				}
			}

			// Store the new reject table
			StoreTable(table);
		}

		#endregion
	}
}
