
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
using Microsoft.Win32;
using System.Diagnostics;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using System.IO;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class ConfigForm : DelayedForm
	{
		// Variables
		private GameConfiguration gameconfig;
		private ConfigurationInfo configinfo;
		private List<DefinedTextureSet> copiedsets;
		private bool preventchanges = false;
		private bool reloadresources = false;

		// Properties
		public bool ReloadResources { get { return reloadresources; } }

		// Constructor
		public ConfigForm()
		{
			ListViewItem lvi;
			
			// Initialize
			InitializeComponent();
			
			// Make list column header full width
			columnname.Width = listconfigs.ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth - 2;
			
			// Fill list of configurations
			foreach(ConfigurationInfo ci in General.Configs)
			{
				// Add a copy
				lvi = listconfigs.Items.Add(ci.Name);
				lvi.Tag = ci.Clone();

				// This is the current configuration?
				if((General.Map != null) && (General.Map.ConfigSettings.Filename == ci.Filename))
					lvi.Selected = true;
			}
			
			// No skill
			skill.Value = 0;
			
			// Nodebuilders are allowed to be empty
			nodebuildersave.Items.Add(new NodebuilderInfo());
			nodebuildertest.Items.Add(new NodebuilderInfo());
			
			// Fill comboboxes with nodebuilders
			nodebuildersave.Items.AddRange(General.Nodebuilders.ToArray());
			nodebuildertest.Items.AddRange(General.Nodebuilders.ToArray());
			
			// Fill list of editing modes
			foreach(EditModeInfo emi in General.Editing.ModesInfo)
			{
				// Is this mode selectable by the user?
				if(emi.IsOptional)
				{
					lvi = listmodes.Items.Add(emi.Attributes.DisplayName);
					lvi.Tag = emi;
					lvi.SubItems.Add(emi.Plugin.Plug.Name);
				}
			}
		}
		
		// This shows a specific page
		public void ShowTab(int index)
		{
			tabs.SelectedIndex = index;
		}

		// Configuration item selected
		private void listconfigs_SelectedIndexChanged(object sender, EventArgs e)
		{
			NodebuilderInfo ni;
			
			// Item selected?
			if(listconfigs.SelectedItems.Count > 0)
			{
				// Enable panels
				tabs.Enabled = true;

				preventchanges = true;
				
				// Get config info of selected item
				configinfo = listconfigs.SelectedItems[0].Tag as ConfigurationInfo;
				
				// Load the game configuration
				gameconfig = new GameConfiguration(General.LoadGameConfiguration(configinfo.Filename));

				// Set defaults
				configinfo.ApplyDefaults(gameconfig);
				
				// Fill resources list
				configdata.EditResourceLocationList(configinfo.Resources);
				
				// Go for all nodebuilder save items
				nodebuildersave.SelectedIndex = -1;
				for(int i = 0; i < nodebuildersave.Items.Count; i++)
				{
					// Get item
					ni = nodebuildersave.Items[i] as NodebuilderInfo;
					
					// Item matches configuration setting?
					if(string.Compare(ni.Name, configinfo.NodebuilderSave, false) == 0)
					{
						// Select this item
						nodebuildersave.SelectedIndex = i;
						break;
					}
				}
				
				// Go for all nodebuilder test items
				nodebuildertest.SelectedIndex = -1;
				for(int i = 0; i < nodebuildertest.Items.Count; i++)
				{
					// Get item
					ni = nodebuildertest.Items[i] as NodebuilderInfo;
					
					// Item matches configuration setting?
					if(string.Compare(ni.Name, configinfo.NodebuilderTest, false) == 0)
					{
						// Select this item
						nodebuildertest.SelectedIndex = i;
						break;
					}
				}
				
				// Fill skills list
				skill.ClearInfo();
				skill.AddInfo(gameconfig.Skills.ToArray());
				
				// Set test application and parameters
				if(!configinfo.CustomParameters)
				{
					configinfo.TestParameters = gameconfig.TestParameters;
					configinfo.TestShortPaths = gameconfig.TestShortPaths;
				}
				testapplication.Text = configinfo.TestProgram;
				testparameters.Text = configinfo.TestParameters;
				shortpaths.Checked = configinfo.TestShortPaths;
				int skilllevel = configinfo.TestSkill;
				skill.Value = skilllevel - 1;
				skill.Value = skilllevel;
				customparameters.Checked = configinfo.CustomParameters;
				
				// Fill texture sets list
				listtextures.Items.Clear();
				foreach(DefinedTextureSet ts in configinfo.TextureSets)
				{
					ListViewItem item = listtextures.Items.Add(ts.Name);
					item.Tag = ts;
					item.ImageIndex = 0;
				}
				listtextures.Sort();
				
				// Go for all the editing modes in the list
				foreach(ListViewItem lvi in listmodes.Items)
				{
					EditModeInfo emi = (lvi.Tag as EditModeInfo);
					lvi.Checked = (configinfo.EditModes.ContainsKey(emi.Type.FullName) && configinfo.EditModes[emi.Type.FullName]);
				}
				
				// Fill start modes
				RefillStartModes();

				// Done
				preventchanges = false;
			}
		}
		
		// Key released
		private void listconfigs_KeyUp(object sender, KeyEventArgs e)
		{
			// Nothing selected?
			if(listconfigs.SelectedItems.Count == 0)
			{
				// Disable panels
				gameconfig = null;
				configinfo = null;
				configdata.FixedResourceLocationList(new DataLocationList());
				configdata.EditResourceLocationList(new DataLocationList());
				nodebuildersave.SelectedIndex = -1;
				nodebuildertest.SelectedIndex = -1;
				testapplication.Text = "";
				testparameters.Text = "";
				shortpaths.Checked = false;
				skill.Value = 0;
				skill.ClearInfo();
				customparameters.Checked = false;
				tabs.Enabled = false;
				listtextures.Items.Clear();
			}
		}
		
		// Mouse released
		private void listconfigs_MouseUp(object sender, MouseEventArgs e)
		{
			listconfigs_KeyUp(sender, new KeyEventArgs(Keys.None));
		}
		
		// Resource locations changed
		private void resourcelocations_OnContentChanged()
		{
			// Leave when no configuration selected
			if(configinfo == null) return;
			
			// Apply to selected configuration
			configinfo.Resources.Clear();
			configinfo.Resources.AddRange(configdata.GetResources());
			if(!preventchanges) reloadresources = true;
		}

		// Nodebuilder selection changed
		private void nodebuildersave_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Leave when no configuration selected
			if(configinfo == null) return;
			
			// Apply to selected configuration
			if(nodebuildersave.SelectedItem != null)
				configinfo.NodebuilderSave = (nodebuildersave.SelectedItem as NodebuilderInfo).Name;
		}

		// Nodebuilder selection changed
		private void nodebuildertest_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Leave when no configuration selected
			if(configinfo == null) return;

			// Apply to selected configuration
			if(nodebuildertest.SelectedItem != null)
				configinfo.NodebuilderTest = (nodebuildertest.SelectedItem as NodebuilderInfo).Name;
		}
		
		// Test application changed
		private void testapplication_TextChanged(object sender, EventArgs e)
		{
			// Leave when no configuration selected
			if(configinfo == null) return;

			// Apply to selected configuration
			configinfo.TestProgram = testapplication.Text;
		}

		// Test parameters changed
		private void testparameters_TextChanged(object sender, EventArgs e)
		{
			// Leave when no configuration selected
			if(configinfo == null) return;

			// Apply to selected configuration
			configinfo = listconfigs.SelectedItems[0].Tag as ConfigurationInfo;
			configinfo.TestParameters = testparameters.Text;

			// Show example result
			CreateParametersExample();
		}
		
		// This creates a new parameters example
		private void CreateParametersExample()
		{
			// Map loaded?
			if(General.Map != null)
			{
				// Make converted parameters
				testresult.Text = General.Map.Launcher.ConvertParameters(testparameters.Text, skill.Value, shortpaths.Checked);
			}
		}
		
		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			ConfigurationInfo ci;
			
			// Apply configuration items
			foreach(ListViewItem lvi in listconfigs.Items)
			{
				// Get configuration item
				ci = lvi.Tag as ConfigurationInfo;
				
				// Find same configuration info in originals
				foreach(ConfigurationInfo oci in General.Configs)
				{
					// Apply settings when they match
					if(string.Compare(ci.Filename, oci.Filename) == 0) oci.Apply(ci);
				}
			}
			
			// Close
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Close
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Browse test program
		private void browsetestprogram_Click(object sender, EventArgs e)
		{
			// Set initial directory
			if(testapplication.Text.Length > 0)
			{
				try { testprogramdialog.InitialDirectory = Path.GetDirectoryName(testapplication.Text); }
				catch(Exception) { }
			}
			
			// Browse for test program
			if(testprogramdialog.ShowDialog() == DialogResult.OK)
			{
				// Apply
				testapplication.Text = testprogramdialog.FileName;
			}
		}

		// Customize parameters (un)checked
		private void customparameters_CheckedChanged(object sender, EventArgs e)
		{
			// Leave when no configuration selected
			if(configinfo == null) return;

			// Apply to selected configuration
			configinfo.CustomParameters = customparameters.Checked;

			// Update interface
			labelparameters.Visible = customparameters.Checked;
			testparameters.Visible = customparameters.Checked;
			shortpaths.Visible = customparameters.Checked;

			// Check if a map is loaded
			if(General.Map != null)
			{
				// Show parameters example result
				labelresult.Visible = customparameters.Checked;
				testresult.Visible = customparameters.Checked;
				noresultlabel.Visible = false;
			}
			else
			{
				// Cannot show parameters example result
				labelresult.Visible = false;
				testresult.Visible = false;
				noresultlabel.Visible = customparameters.Checked;
			}
		}
		
		// Use short paths changed
		private void shortpaths_CheckedChanged(object sender, EventArgs e)
		{
			// Leave when no configuration selected
			if(configinfo == null) return;
			
			// Apply to selected configuration
			configinfo.TestShortPaths = shortpaths.Checked;
			
			CreateParametersExample();
		}
		
		// Skill changes
		private void skill_ValueChanges(object sender, EventArgs e)
		{
			// Leave when no configuration selected
			if(configinfo == null) return;
			
			// Apply to selected configuration
			configinfo.TestSkill = skill.Value;
			
			CreateParametersExample();
		}

		// Make new texture set
		private void addtextureset_Click(object sender, EventArgs e)
		{
			DefinedTextureSet s = new DefinedTextureSet("New Texture Set");
			TextureSetForm form = new TextureSetForm();
			form.Setup(s);
			if(form.ShowDialog(this) == DialogResult.OK)
			{
				// Add to texture sets
				configinfo.TextureSets.Add(s);
				ListViewItem item = listtextures.Items.Add(s.Name);
				item.Tag = s;
				item.ImageIndex = 0;
				listtextures.Sort();
				reloadresources = true;
			}
		}

		// Edit texture set
		private void edittextureset_Click(object sender, EventArgs e)
		{
			// Texture Set selected?
			if(listtextures.SelectedItems.Count > 0)
			{
				DefinedTextureSet s = (listtextures.SelectedItems[0].Tag as DefinedTextureSet);
				TextureSetForm form = new TextureSetForm();
				form.Setup(s);
				form.ShowDialog(this);
				listtextures.SelectedItems[0].Text = s.Name;
				listtextures.Sort();
				reloadresources = true;
			}
		}
		
		// Remove texture set
		private void removetextureset_Click(object sender, EventArgs e)
		{
			// Texture Set selected?
			while(listtextures.SelectedItems.Count > 0)
			{
				// Remove from config info and list
				DefinedTextureSet s = (listtextures.SelectedItems[0].Tag as DefinedTextureSet);
				configinfo.TextureSets.Remove(s);
				listtextures.SelectedItems[0].Remove();
				reloadresources = true;
			}
		}
		
		// Texture Set selected/deselected
		private void listtextures_SelectedIndexChanged(object sender, EventArgs e)
		{
			edittextureset.Enabled = (listtextures.SelectedItems.Count > 0);
			removetextureset.Enabled = (listtextures.SelectedItems.Count > 0);
			copytexturesets.Enabled = (listtextures.SelectedItems.Count > 0);
		}
		
		// Doubleclicking a texture set
		private void listtextures_DoubleClick(object sender, EventArgs e)
		{
			edittextureset_Click(sender, e);
		}
		
		// Copy selected texture sets
		private void copytexturesets_Click(object sender, EventArgs e)
		{
			// Make copies
			copiedsets = new List<DefinedTextureSet>();
			foreach(ListViewItem item in listtextures.SelectedItems)
			{
				DefinedTextureSet s = (item.Tag as DefinedTextureSet);
				copiedsets.Add(s.Copy());
			}
			
			// Enable button
			pastetexturesets.Enabled = true;
		}
		
		// Paste copied texture sets
		private void pastetexturesets_Click(object sender, EventArgs e)
		{
			if(copiedsets != null)
			{
				// Add copies
				foreach(DefinedTextureSet ts in copiedsets)
				{
					DefinedTextureSet s = ts.Copy();
					ListViewItem item = listtextures.Items.Add(s.Name);
					item.Tag = s;
					item.ImageIndex = 0;
					configinfo.TextureSets.Add(s);
				}
				listtextures.Sort();
				reloadresources = true;
			}
		}
		
		// This will add the default sets from game configuration
		private void restoretexturesets_Click(object sender, EventArgs e)
		{
			// Ask nicely first
			if(MessageBox.Show(this, "This will add the default Texture Sets from the Game Configuration. Do you want to continue?",
				"Add Default Sets", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				// Add copies
				foreach(DefinedTextureSet ts in gameconfig.TextureSets)
				{
					DefinedTextureSet s = ts.Copy();
					ListViewItem item = listtextures.Items.Add(s.Name);
					item.Tag = s;
					item.ImageIndex = 0;
					configinfo.TextureSets.Add(s);
				}
				listtextures.Sort();
				reloadresources = true;
			}
		}
		
		// This is called when an editing mode item is checked or unchecked
		private void listmodes_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			// Leave when no configuration selected
			if(configinfo == null) return;
			
			// Apply changes
			EditModeInfo emi = (e.Item.Tag as EditModeInfo);
			bool currentstate = (configinfo.EditModes.ContainsKey(emi.Type.FullName) && configinfo.EditModes[emi.Type.FullName]);
			if(e.Item.Checked && !currentstate)
			{
				// Add
				configinfo.EditModes[emi.Type.FullName] = true;
			}
			else if(!e.Item.Checked && currentstate)
			{
				// Remove
				configinfo.EditModes[emi.Type.FullName] = false;
			}
			
			preventchanges = true;
			RefillStartModes();
			preventchanges = false;
		}

		// Help requested
		private void ConfigForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_gameconfigurations.html");
			hlpevent.Handled = true;
		}
		
		// This refills the start mode cobobox
		private void RefillStartModes()
		{
			// Refill the startmode combobox
			startmode.Items.Clear();
			foreach(ListViewItem item in listmodes.Items)
			{
				if(item.Checked)
				{
					EditModeInfo emi = (item.Tag as EditModeInfo);
					if(emi.Attributes.SafeStartMode)
					{
						int newindex = startmode.Items.Add(emi);
						if(emi.Type.Name == configinfo.StartMode) startmode.SelectedIndex = newindex;
					}
				}
			}
			
			// Select the first in the combobox if none are selected
			if((startmode.SelectedItem == null) && (startmode.Items.Count > 0))
			{
				startmode.SelectedIndex = 0;
				EditModeInfo emi = (startmode.SelectedItem as EditModeInfo);
				configinfo.StartMode = emi.Type.Name;
			}
		}
		
		// Start mode combobox changed
		private void startmode_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(preventchanges || (configinfo == null)) return;
			
			// Apply start mode
			if(startmode.SelectedItem != null)
			{
				EditModeInfo emi = (startmode.SelectedItem as EditModeInfo);
				configinfo.StartMode = emi.Type.Name;
			}
		}
	}
}
