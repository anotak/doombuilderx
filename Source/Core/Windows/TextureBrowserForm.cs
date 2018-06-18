
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
using Microsoft.Win32;
using System.Diagnostics;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
    // ano - as of may-31-2018 this has been merged with flatbrowserform
    // this is partially based on
    // mxd's merging of the two in the gzdb source code
    // greatly reduces code duplication
    internal partial class TextureBrowserForm : Form
	{
		private const int COLUMN_WIDTH_COUNT = 52;
		
		// Variables
		private string selectedname;
		private Point lastposition;
		private Size lastsize;
		private ListViewGroup usedgroup;
		private ListViewGroup availgroup;
		private ListViewItem selectedset;
		private string selecttextureonfill;

        public bool bStillLoading;
        // Properties
        public string SelectedName { get { return selectedname; } }

        private readonly bool browseflats; //mxd

        // Constructor
        public TextureBrowserForm(string selecttexture, bool nbrowseflats)
        {
            bStillLoading = true;

            browseflats = nbrowseflats;

            Cursor.Current = Cursors.WaitCursor;
            ListViewItem item;
            bool foundselecttexture = false;
            long longname = Lump.MakeLongName(selecttexture ?? "");

            // Initialize
            InitializeComponent();
            browser.ApplySettings();

            // Update the used textures
            General.Map.Data.UpdateUsedTextures();

            // Resize columns to maximize available width
            countcolumn.Width = COLUMN_WIDTH_COUNT;
            namecolumn.Width = texturesets.ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth - countcolumn.Width - 2;

            // Fill texture sets list with normal texture sets
            foreach (IFilledTextureSet ts in General.Map.Data.TextureSets)
            {
                int textureCount;
                if (General.Map.Config.MixTexturesFlats)
                {
                    textureCount = ts.Flats.Count + ts.Textures.Count;
                }
                else if (browseflats)
                {
                    textureCount = ts.Flats.Count;
                }
                else
                {
                    textureCount = ts.Textures.Count;
                }

                // ano - hide empty texturesets
                if (textureCount > 0)
                {
                    item = texturesets.Items.Add(ts.Name);
                    item.Tag = ts;
                    item.ImageIndex = 0;
                    item.UseItemStyleForSubItems = false;
                    item.SubItems.Add(textureCount.ToString(), item.ForeColor,
                            item.BackColor, new Font(item.Font, FontStyle.Regular));
                }
            }

            // Add container-specific texture sets
            foreach (ResourceTextureSet ts in General.Map.Data.ResourceTextureSets)
            {
                int textureCount;
                if (General.Map.Config.MixTexturesFlats)
                {
                    textureCount = ts.Flats.Count + ts.Textures.Count;
                }
                else if (browseflats)
                {
                    textureCount = ts.Flats.Count;
                }
                else
                {
                    textureCount = ts.Textures.Count;
                }

                item = texturesets.Items.Add(ts.Name);
                item.Tag = ts;
                item.ImageIndex = 2 + ts.Location.type;
                item.UseItemStyleForSubItems = false;
                item.SubItems.Add(textureCount.ToString(), item.ForeColor,
                        item.BackColor, new Font(item.Font, FontStyle.Regular));
            }

            // Add All textures set
            {
                int textureCount;
                if (General.Map.Config.MixTexturesFlats)
                {
                    textureCount = General.Map.Data.AllTextureSet.Flats.Count + General.Map.Data.AllTextureSet.Textures.Count;
                }
                else if (browseflats)
                {
                    textureCount = General.Map.Data.AllTextureSet.Flats.Count;
                }
                else
                {
                    textureCount = General.Map.Data.AllTextureSet.Textures.Count;
                }

                item = texturesets.Items.Add(General.Map.Data.AllTextureSet.Name);
                item.Tag = General.Map.Data.AllTextureSet;
                item.ImageIndex = 1;
                item.UseItemStyleForSubItems = false;
                item.SubItems.Add(textureCount.ToString(),
                    item.ForeColor, item.BackColor, new Font(item.Font, FontStyle.Regular));
            }

            if (General.Map.Config.MixTexturesFlats)
            {
                if (!browseflats && General.Map.Data.WallsTextureSet != null)
                {
                    // Add all Textures-only set
                    int textureCount;
                    textureCount = General.Map.Data.WallsTextureSet.Textures.Count;

                    item = texturesets.Items.Add(General.Map.Data.WallsTextureSet.Name);
                    item.Tag = General.Map.Data.WallsTextureSet;
                    item.ImageIndex = 1;
                    item.UseItemStyleForSubItems = false;
                    item.SubItems.Add(textureCount.ToString(),
                        item.ForeColor, item.BackColor, new Font(item.Font, FontStyle.Regular));
                }
                else if (browseflats && General.Map.Data.FlatsTextureSet != null)
                {
                    // Add All flats-only set
                    int textureCount;
                    textureCount = General.Map.Data.FlatsTextureSet.Textures.Count;

                    item = texturesets.Items.Add(General.Map.Data.FlatsTextureSet.Name);
                    item.Tag = General.Map.Data.FlatsTextureSet;
                    item.ImageIndex = 1;
                    item.UseItemStyleForSubItems = false;
                    item.SubItems.Add(textureCount.ToString(),
                        item.ForeColor, item.BackColor, new Font(item.Font, FontStyle.Regular));
                }
            }

            // Select the last one that was selected
            // ano - renamed this from "selectname" because there's also a "selectedname"
            // in scope and it's confusing
            string cfgTextureSet = General.Settings.ReadSetting("browserwindow.textureset", "");

            // ano - select flats box if we're on a mixflats/textures cfg and we're looking in "All"
            if (!foundselecttexture && General.Map.Config.MixTexturesFlats && cfgTextureSet == "All")
            {
                cfgTextureSet = browseflats ? "Textures" : "Flats";
            }

            foreach (ListViewItem i in texturesets.Items)
			{
				if(i.Text == cfgTextureSet)
				{
					IFilledTextureSet set = (i.Tag as IFilledTextureSet);
					foreach(ImageData img in set.Textures)
					{
						if(img.LongName == longname)
						{
							i.Selected = true;
							foundselecttexture = true;
							break;
						}
					}
					break;
				}
			}
			
			// If the selected texture was not found in the last-selected set, try finding it in the other sets
			if(!foundselecttexture)
			{
				foreach(ListViewItem i in texturesets.Items)
				{
					IFilledTextureSet set = (i.Tag as IFilledTextureSet);
					foreach(ImageData img in set.Textures)
					{
						if(img.LongName == longname)
						{
							i.Selected = true;
							foundselecttexture = true;
							break;
						}
					}
					if(foundselecttexture) break;
				}
			}
			
			// Texture still now found? Then just select the last used set
			if(!foundselecttexture)
			{
				foreach(ListViewItem i in texturesets.Items)
				{
					if(i.Text == cfgTextureSet)
					{
						i.Selected = true;
						foundselecttexture = true;
						break;
					}
				}
			}

			// WARNING: Some strange behavior of the listview here!
			// When you leave this line out, the list becomes very slow.
			// Also, this does not change the item selected previously.
			texturesets.Items[0].Selected = true;

			// Texture to select when list is filled
			selecttextureonfill = selecttexture;
			
			// Make groups
			usedgroup = browser.AddGroup(browseflats ? "Used Flats" : "Used Textures");
			availgroup = browser.AddGroup(browseflats ? "Available Flats" : "Available Textures");
			
			// Keep last position and size
			lastposition = this.Location;
			lastsize = this.Size;

            // ano - rename labels if we're browsing flats
            if (browseflats)
            {
                Text = "Browse Flats";
                browser.LabelText = "Select or enter a flat name:";
            }

            // Position window from configuration settings
            this.SuspendLayout();

            /*
			this.Location = new Point(General.Settings.ReadSetting("browserwindow.positionx", this.Location.X),
									  General.Settings.ReadSetting("browserwindow.positiony", this.Location.Y));
			*/
            this.Size = new Size(General.Settings.ReadSetting("browserwindow.sizewidth", this.Size.Width),
								 General.Settings.ReadSetting("browserwindow.sizeheight", this.Size.Height));
			this.WindowState = (FormWindowState)General.Settings.ReadSetting("browserwindow.windowstate", (int)FormWindowState.Normal);
			if(this.WindowState == FormWindowState.Normal) this.StartPosition = FormStartPosition.CenterParent;
			this.ResumeLayout(true);
            bStillLoading = false;
        }

		// Selection changed
		private void browser_SelectedItemChanged()
		{
			apply.Enabled = (browser.SelectedItem != null);
		}

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Set selected name and close
			if(browser.SelectedItem != null)
			{
				selectedname = browser.SelectedItem.Text;
				DialogResult = DialogResult.OK;
			}
			else
			{
				selectedname = "";
				DialogResult = DialogResult.Cancel;
			}
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// No selection, close
			selectedname = "";
			DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Activated
		private void TextureBrowserForm_Activated(object sender, EventArgs e)
		{
			// Focus the textbox
			browser.FocusTextbox();
			Cursor.Current = Cursors.Default;
		}

		// Loading
		private void TextureBrowserForm_Load(object sender, EventArgs e)
		{
			// Normal windowstate?
			if(this.WindowState == FormWindowState.Normal)
			{
				// Keep last position and size
				lastposition = this.Location;
				lastsize = this.Size;
			}
		}

		// Resized
		private void TextureBrowserForm_ResizeEnd(object sender, EventArgs e)
		{
			// Normal windowstate?
			if(this.WindowState == FormWindowState.Normal)
			{
				// Keep last position and size
				lastposition = this.Location;
				lastsize = this.Size;
			}
		}

		// Moved
		private void TextureBrowserForm_Move(object sender, EventArgs e)
		{
			// Normal windowstate?
			if(this.WindowState == FormWindowState.Normal)
			{
				// Keep last position and size
				lastposition = this.Location;
				lastsize = this.Size;
			}
		}

		// Closing
		private void TextureBrowserForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			int windowstate;

			// Determine window state to save
			if(this.WindowState != FormWindowState.Minimized)
				windowstate = (int)this.WindowState;
			else
				windowstate = (int)FormWindowState.Normal;

			// Save window settings
			General.Settings.WriteSetting("browserwindow.positionx", lastposition.X);
			General.Settings.WriteSetting("browserwindow.positiony", lastposition.Y);
			General.Settings.WriteSetting("browserwindow.sizewidth", lastsize.Width);
			General.Settings.WriteSetting("browserwindow.sizeheight", lastsize.Height);
			General.Settings.WriteSetting("browserwindow.windowstate", windowstate);
			
			// Save last selected texture set
			if(texturesets.SelectedItems.Count > 0)
				General.Settings.WriteSetting("browserwindow.textureset", texturesets.SelectedItems[0].Text);
			
			// Clean up
			browser.CleanUp();
		}

		// Static method to browse for texture
		// Returns null when cancelled.
		public static string Browse(IWin32Window parent, string select, bool browseflats)
		{
			TextureBrowserForm browser = new TextureBrowserForm(select, browseflats);
			if(browser.ShowDialog(parent) == DialogResult.OK)
			{
				// Return result
				return browser.SelectedName;
			}
			else
			{
				// Cancelled
				return select;
			}
		}
		
		// Texture set selected
		private void texturesets_SelectedIndexChanged(object sender, EventArgs e)
		{
            if (bStillLoading)
            {
                return;
            }

            // Anything slected?
            if (texturesets.SelectedItems.Count > 0)
			{
				selectedset = texturesets.SelectedItems[0];
				FillImagesList();
			}
		}

		// Item double clicked
		private void browser_SelectedItemDoubleClicked()
		{
			if(apply.Enabled) apply_Click(this, EventArgs.Empty);
		}

		// This fills the list of textures, depending on the selected texture set
		private void FillImagesList()
        {
            if (bStillLoading)
            {
                return;
            }
            // Get the selected texture set
            IFilledTextureSet set = (selectedset.Tag as IFilledTextureSet);
			
			// Start adding
			browser.BeginAdding(false);

            if (browseflats)
            {
                // Add all available flats
                foreach (ImageData img in set.Flats)
                    browser.Add(img.Name, img, img, availgroup);

                // Add all used flats
                foreach (ImageData img in set.Flats)
                    if (img.UsedInMap) browser.Add(img.Name, img, img, usedgroup);
            }
            else
            {
                // Add all available textures and mark the images for temporary loading
                foreach (ImageData img in set.Textures)
                    browser.Add(img.Name, img, img, availgroup);

                // Add all used textures and mark the images for permanent loading
                foreach (ImageData img in set.Textures)
                    if (img.UsedInMap) browser.Add(img.Name, img, img, usedgroup);
            }
			
			// Done adding
			browser.EndAdding();
		}

		// Help
		private void TextureBrowserForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_imagesbrowser.html");
			hlpevent.Handled = true;
		}

		private void TextureBrowserForm_Shown(object sender, EventArgs e)
		{
            bStillLoading = false;
            browser.bStillLoading = false;
            FillImagesList();
			// Select texture
			if(!string.IsNullOrEmpty(selecttextureonfill))
			{
				browser.SelectItem(selecttextureonfill, usedgroup);
				selecttextureonfill = null;
			}
		}
	} // class
} // ns