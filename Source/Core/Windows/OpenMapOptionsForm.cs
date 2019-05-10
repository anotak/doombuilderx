
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using System.IO;
using System.Collections;
using System.Diagnostics;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class OpenMapOptionsForm : DelayedForm
	{
		// Variables
		private Configuration mapsettings;
		private MapOptions options;
		private WAD wadfile;
		private string filepathname;
		private string selectedmapname;
		
		// Properties
		public string FilePathName { get { return filepathname; } }
		public MapOptions Options { get { return options; } }
		
		// Constructor
		public OpenMapOptionsForm(string filepathname)
		{
			// Initialize
			InitializeComponent();
			this.Text = "Open Map from " + Path.GetFileName(filepathname);
			this.filepathname = filepathname;
			this.options = null;
		}

		// Constructor
		public OpenMapOptionsForm(string filepathname, MapOptions options)
		{
			// Initialize
			InitializeComponent();
			this.Text = "Open Map from " + Path.GetFileName(filepathname);
			this.filepathname = filepathname;
			this.options = options;
			datalocations.EditResourceLocationList(options.Resources);
		}

		// This loads the settings and attempt to find a suitable config
		private void LoadSettings()
		{
			string dbsfile;
			string gameconfig;
			int index;
			
			// Busy
			Cursor.Current = Cursors.WaitCursor;

			// Check if the file exists
			if(!File.Exists(filepathname))
			{
				// WAD file does not exist
				MessageBox.Show(this, "Could not open the WAD file: The file does not exist.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.DialogResult = DialogResult.Cancel;
				this.Close();
				return;
			}
			
			try
			{
				// Open the WAD file
				wadfile = new WAD(filepathname, true);
			}
			catch(Exception)
			{
				// Unable to open WAD file (or its config)
				MessageBox.Show(this, "Could not open the WAD file for reading. Please make sure the file you selected is valid and is not in use by any other application.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				if(wadfile != null) wadfile.Dispose();
				this.DialogResult = DialogResult.Cancel;
				this.Close();
				return;
			}

			// Open the Map Settings configuration
			dbsfile = filepathname.Substring(0, filepathname.Length - 4) + ".dbs";
			if(File.Exists(dbsfile))
				try { mapsettings = new Configuration(dbsfile, true); }
				catch(Exception) { mapsettings = new Configuration(true); }
			else
				mapsettings = new Configuration(true);
			
			// Check strict patches box
			if(options != null)
				strictpatches.Checked = options.StrictPatches;
			else
				strictpatches.Checked = mapsettings.ReadSetting("strictpatches", false);
			
			// Check what game configuration is preferred
			if(options != null)
				gameconfig = options.ConfigFile;
			else
				gameconfig = mapsettings.ReadSetting("gameconfig", "");

            // ano - try to default to last used if we've got nothing
            if (string.IsNullOrEmpty(gameconfig))
            {
                gameconfig = General.Settings.ReadSetting("lastopenedgameconfig", "EE_DoomUDMF.cfg");
            }

			// Go for all configurations
			for(int i = 0; i < General.Configs.Count; i++)
			{
				// Add config name to list
				index = config.Items.Add(General.Configs[i]);

				// This is the preferred game configuration?
				if(General.Configs[i].Filename == gameconfig)
				{
					// Select this item
					config.SelectedIndex = index;
				}
			}

			// Still no configuration selected?
			if(config.SelectedIndex == -1)
			{
				// Then go for all configurations to find a suitable one
				for(int i = 0; i < General.Configs.Count; i++)
				{
					// Check if a resource location is set for this configuration
					if(General.Configs[i].Resources.Count > 0)
					{
						// Match the wad against this configuration
						if(MatchConfiguration(General.Configs[i].Filename, wadfile))
						{
							// Select this item
							config.SelectedIndex = i;
							break;
						}
					}
				}
			}
			
			// Done
			Cursor.Current = Cursors.Default;
		}
		
		// This matches a WAD file with the specified game configuration
		// by checking if the specific lumps are detected
		private bool MatchConfiguration(string configfile, WAD wadfile)
		{
			Configuration cfg;
			IDictionary detectlumps;
			Lump lumpresult;
			bool result = false;
			
			// Load the configuration
			cfg = General.LoadGameConfiguration(configfile);

			// Get the lumps to detect
			detectlumps = cfg.ReadSetting("gamedetect", new Hashtable());

			// Go for all the lumps
			foreach(DictionaryEntry lmp in detectlumps)
			{
				// Setting not broken?
				if((lmp.Value is int) && (lmp.Key is string))
				{
					// Find the lump in the WAD file
					lumpresult = wadfile.FindLump((string)lmp.Key);
					
					// If one of these lumps must exist, and it is found
					if(((int)lmp.Value == 1) && (lumpresult != null))
					{
						// Good result.
						result = true;
					}
					// If this lumps may not exist, and it is found
					else if(((int)lmp.Value == 2) && (lumpresult != null))
					{
						// Bad result.
						result = false;
						break;
					}
					// If this lumps must exist, and is found
					else if(((int)lmp.Value == 3) && (lumpresult != null))
					{
						// Good result.
						result = true;
					}
					// If this lumps must exist, and it is missing
					else if(((int)lmp.Value == 3) && (lumpresult == null))
					{
						// Bad result.
						result = false;
						break;
					}
				}
			}

			// Return result
			return result;
		}

		// Configuration is selected
		private void config_SelectedIndexChanged(object sender, EventArgs e)
		{
			List<ListViewItem> mapnames;
			ConfigurationInfo ci;
			Configuration cfg;
			IDictionary maplumpnames;
			int scanindex, checkoffset;
			int lumpsfound, lumpsrequired = 0;
			string lumpname, selectedname = "";

			// Anything selected?
			if(config.SelectedIndex > -1)
			{
				// Keep selected name, if any
				if(mapslist.SelectedItems.Count > 0)
					selectedname = mapslist.SelectedItems[0].Text;

				// Make an array for the map names
				mapnames = new List<ListViewItem>();

				// Get selected configuration info
				ci = (ConfigurationInfo)config.SelectedItem;
				
				// Load this configuration
				cfg = General.LoadGameConfiguration(ci.Filename);

				// Get the map lump names
				maplumpnames = cfg.ReadSetting("maplumpnames", new Hashtable());

				// Count how many required lumps we have to find
				foreach(DictionaryEntry ml in maplumpnames)
				{
					// Ignore the map header (it will not be found because the name is different)
					if(ml.Key.ToString() != MapManager.CONFIG_MAP_HEADER)
					{
						// Read lump setting and count it
						if(cfg.ReadSetting("maplumpnames." + ml.Key + ".required", false)) lumpsrequired++;
					}
				}

				// Go for all the lumps in the wad
				for(scanindex = 0; scanindex < (wadfile.Lumps.Count - 1); scanindex++)
				{
					// Make sure this lump is not part of the map
					if(!maplumpnames.Contains(wadfile.Lumps[scanindex].Name))
					{
						// Reset check
						lumpsfound = 0;
						checkoffset = 1;

						// Continue while still within bounds and lumps are still recognized
						while(((scanindex + checkoffset) < wadfile.Lumps.Count) &&
							  maplumpnames.Contains(wadfile.Lumps[scanindex + checkoffset].Name))
						{
							// Count the lump when it is marked as required
							lumpname = wadfile.Lumps[scanindex + checkoffset].Name;
							if(cfg.ReadSetting("maplumpnames." + lumpname + ".required", false)) lumpsfound++;

							// Check the next lump
							checkoffset++;
						}

						// Map found? Then add it to the list
						if(lumpsfound >= lumpsrequired)
							mapnames.Add(new ListViewItem(wadfile.Lumps[scanindex].Name));
					}
				}

				// Clear the list and add the new map names
				mapslist.BeginUpdate();
				mapslist.Items.Clear();
				mapslist.Items.AddRange(mapnames.ToArray());
				mapslist.Sort();

				// Go for all items in the list
				foreach(ListViewItem item in mapslist.Items)
				{
					// Was this item previously selected?
					if(item.Text == selectedname)
					{
						// Select it again
						item.Selected = true;
						break;
					}
				}
				if((mapslist.SelectedItems.Count == 0) && (mapslist.Items.Count > 0)) mapslist.Items[0].Selected = true;
				mapslist.EndUpdate();

				// Show configuration resources
				datalocations.FixedResourceLocationList(ci.Resources);
			}
		}
		
		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Configuration selected?
			if(config.SelectedIndex == -1)
			{
				// Select a configuration!
				MessageBox.Show(this, "Please select a game configuration to use for editing your map.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				config.Focus();
				return;
			}
			
			// Collect information
			ConfigurationInfo configinfo = General.Configs[config.SelectedIndex];
			DataLocationList locations = datalocations.GetResources();
			
			// No map selected?
			if(mapslist.SelectedItems.Count == 0)
			{
				// Choose a map!
				MessageBox.Show(this, "Please select a map to load for editing.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				mapslist.Focus();
				return;
			}
			
			// Check if we should warn the user for missing resources
			if((wadfile.Type != WAD.TYPE_IWAD) && (locations.Count == 0) && (configinfo.Resources.Count == 0))
			{
				if(MessageBox.Show(this, "You are about to load a map without selecting any resources. Textures, flats and " +
										 "sprites may not be shown correctly or may not show up at all. Do you want to continue?", Application.ProductName,
										 MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
				{
					return;
				}
			}
			
			// Apply changes
			options.ClearResources();
			options.ConfigFile = configinfo.Filename;
			options.CurrentName = mapslist.SelectedItems[0].Text;
			options.StrictPatches = strictpatches.Checked;
			options.CopyResources(locations);

            // ano
            General.Settings.WriteSetting("lastopenedgameconfig", options.ConfigFile);

            // Hide window
            wadfile.Dispose();
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
		
		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Just hide window
			wadfile.Dispose();
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Window is shown
		private void OpenMapOptionsForm_Shown(object sender, EventArgs e)
		{
			// Update window
			this.Update();
			
			// Load settings
			LoadSettings();
		}

		// Map name doubleclicked
		private void mapslist_DoubleClick(object sender, EventArgs e)
		{
			// Click OK
			if(mapslist.SelectedItems.Count > 0) apply.PerformClick();
		}

		// Map name selected
		private void mapslist_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			DataLocationList locations;
			DataLocationList listedlocations;
			
			// Map previously selected?
			if((selectedmapname != null) && (selectedmapname != ""))
			{
				// Get locations from previous selected map settings
				locations = new DataLocationList(mapsettings, "maps." + selectedmapname + ".resources");
				listedlocations = datalocations.GetResources();
				
				// Remove data locations that this map has in its config
				foreach(DataLocation dl in locations)
					listedlocations.Remove(dl);

				// Set new data locations
				datalocations.EditResourceLocationList(listedlocations);

				// Done
				selectedmapname = null;
			}
			
			// Anything selected?
			if(mapslist.SelectedItems.Count > 0)
			{
				// Get the map name
				selectedmapname = mapslist.SelectedItems[0].Text;
				options = new MapOptions(mapsettings, selectedmapname);
				
				// Get locations from previous selected map settings
				locations = new DataLocationList(mapsettings, "maps." + selectedmapname + ".resources");
				listedlocations = datalocations.GetResources();

				// Add data locations that this map has in its config
				foreach(DataLocation dl in locations)
					if(!listedlocations.Contains(dl)) listedlocations.Add(dl);

				// Set new data locations
				datalocations.EditResourceLocationList(listedlocations);
			}
		}

		// Help
		private void OpenMapOptionsForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_openmapoptions.html");
			hlpevent.Handled = true;
		}
	}
}