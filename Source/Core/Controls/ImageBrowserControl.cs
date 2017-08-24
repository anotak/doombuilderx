
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
using CodeImp.DoomBuilder.Rendering;
using SlimDX.Direct3D9;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class ImageBrowserControl : UserControl
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Delegates / Events

		public delegate void SelectedItemChangedDelegate();
		public delegate void SelectedItemDoubleClickDelegate();

		public event SelectedItemChangedDelegate SelectedItemChanged;
		public event SelectedItemDoubleClickDelegate SelectedItemDoubleClicked;
		
		#endregion

		#region ================== Variables
		
		// Properties
		private bool preventselection;
		
		// States
		private bool updating;
		private int keepselected;
		
		// All items
		private List<ImageBrowserItem> items;

		// Items visible in the list
		private List<ImageBrowserItem> visibleitems;

        public bool bStillLoading;

        public bool bAdding;
		
		#endregion

		#region ================== Properties

		public bool PreventSelection { get { return preventselection; } set { preventselection = value; } }
		public bool HideInputBox { get { return splitter.Panel2Collapsed; } set { splitter.Panel2Collapsed = value; } }
		public string LabelText { get { return label.Text; } set { label.Text = value; objectname.Left = label.Right + label.Margin.Right + objectname.Margin.Left; } }
		public ListViewItem SelectedItem { get { if(list.SelectedItems.Count > 0) return list.SelectedItems[0]; else return null; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ImageBrowserControl()
		{
            bStillLoading = true;
            bAdding = false;
			// Initialize
			InitializeComponent();
			items = new List<ImageBrowserItem>();
			
			// Move textbox with label
			objectname.Left = label.Right + label.Margin.Right + objectname.Margin.Left;
		}
		
		// This applies the application settings
		public void ApplySettings()
		{
			// Force black background?
			if(General.Settings.BlackBrowsers)
			{
				list.BackColor = Color.Black;
				list.ForeColor = Color.White;
			}

			// Set the size of preview images
			if(General.Map != null)
			{
				int itemwidth = General.Map.Data.Previews.MaxImageWidth + 26;
				int itemheight = General.Map.Data.Previews.MaxImageHeight + 26;
				if(General.Settings.ShowTextureSizes) itemheight += 12;
				list.TileSize = new Size(itemwidth, itemheight);
			}
		}

		// This cleans everything up
		public virtual void CleanUp()
		{
			// Stop refresh timer
			refreshtimer.Enabled = false;
		}

		#endregion

		#region ================== Rendering

		// Draw item
		private void list_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
			if(!updating) (e.Item as ImageBrowserItem).Draw(e.Graphics, e.Bounds);
		}

		// Refresher
		private void refreshtimer_Tick(object sender, EventArgs e)
		{
			bool allpreviewsloaded = true;
			
			// Go for all items
			foreach(ImageBrowserItem i in list.Items)
			{
				// Check if there are still previews that are not loaded
				allpreviewsloaded &= i.IsPreviewLoaded;
				
				// Items needs to be redrawn?
				if(i.CheckRedrawNeeded())
				{
					// Refresh item in list
					//list.RedrawItems(i.Index, i.Index, false);
					list.Invalidate();
				}
			}

			// If all previews were loaded, stop this timer
			if(allpreviewsloaded) refreshtimer.Stop();
			
			UpdateTextureSizeLabel();
		}

		#endregion

		#region ================== Events

		// Name typed
		private void objectname_TextChanged(object sender, EventArgs e)
		{
			// Update list
			RefillList(false);

			// No item selected?
			if(list.SelectedItems.Count == 0)
			{
				// Select first
				SelectFirstItem();
			}
		}

		// Key pressed in textbox
		private void objectname_KeyDown(object sender, KeyEventArgs e)
		{
			// Check what key is pressed
			switch(e.KeyData)
			{
				// Cursor keys
				case Keys.Left: SelectNextItem(SearchDirectionHint.Left); e.SuppressKeyPress = true; break;
				case Keys.Right: SelectNextItem(SearchDirectionHint.Right); e.SuppressKeyPress = true; break;
				case Keys.Up: SelectNextItem(SearchDirectionHint.Up); e.SuppressKeyPress = true;  break;
				case Keys.Down: SelectNextItem(SearchDirectionHint.Down); e.SuppressKeyPress = true; break;

				// Tab
				case Keys.Tab: GoToNextSameTexture(); e.SuppressKeyPress = true; break;
			}
		}

		// Key pressed in list
		private void list_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyData == Keys.Tab)
			{
				GoToNextSameTexture();
				e.SuppressKeyPress = true;
			}
		}
		
		// Selection changed
		private void list_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			// Prevent selecting?
			if(preventselection)
			{
				foreach(ListViewItem i in list.SelectedItems) i.Selected = false;
			}
			else
			{
				// Raise event
				if(SelectedItemChanged != null) SelectedItemChanged();
			}

			UpdateTextureSizeLabel();
		}
		
		// Doublelicking an item
		private void list_DoubleClick(object sender, EventArgs e)
		{
			if(!preventselection && (list.SelectedItems.Count > 0))
				if(SelectedItemDoubleClicked != null) SelectedItemDoubleClicked();
		}
		
		// Control is resized
		private void ImageBrowserControl_Resize(object sender, EventArgs e)
		{
			UpdateTextureSizeLabel();
		}

		// This hides the texture size label
		private void texturesizetimer_Tick(object sender, EventArgs e)
		{
			texturesizetimer.Stop();
			texturesize.Visible = false;
			texturesizelabel.Visible = false;
		}
		
		#endregion

		#region ================== Methods

		// This selects the next texture with the same name as the selected texture
		public void GoToNextSameTexture()
		{
			if(list.SelectedItems.Count > 0)
			{
				ListViewItem selected = list.SelectedItems[0];
				bool foundselected = false;
				foreach(ListViewItem n in visibleitems)
				{
					if((n.Text == selected.Text) && foundselected)
					{
						// This is the next item
						n.Selected = true;
						n.EnsureVisible();
						return;
					}
					
					if(n == selected)
						foundselected = true;
				}

				// Start from the top
				foreach(ListViewItem n in visibleitems)
				{
					if((n.Text == selected.Text) && foundselected)
					{
						// This is the next item
						n.Selected = true;
						n.EnsureVisible();
						return;
					}
				}
			}
		}

		// This selects an item by name
		public void SelectItem(string name, ListViewGroup preferredgroup)
		{
			ListViewItem lvi = null;

			// Not when selecting is prevented
			if(preventselection) return;

			// Search in preferred group first
			if(preferredgroup != null)
			{
				foreach(ListViewItem item in list.Items)
				{
					if(string.Compare(item.Text, name, true) == 0)
					{
						lvi = item;
						if(item.Group == preferredgroup) break;
					}
				}
			}
			
			// Select the item
			if(lvi != null)
			{
				// Select this item
				list.SelectedItems.Clear();
				lvi.Selected = true;
				lvi.EnsureVisible();
			}

			UpdateTextureSizeLabel();
		}
		
		// This performs item sleection by keys
		private void SelectNextItem(SearchDirectionHint dir)
		{
			ListViewItem lvi;
			Point spos;
			
			// Not when selecting is prevented
			if(preventselection) return;
			
			// Nothing selected?
			if(list.SelectedItems.Count == 0)
			{
				// Select first
				SelectFirstItem();
			}
			else
			{
				// Get selected item
				lvi = list.SelectedItems[0];
				Rectangle lvirect = list.GetItemRect(lvi.Index, ItemBoundsPortion.Entire);
				spos = new Point(lvirect.Location.X + lvirect.Width / 2, lvirect.Y + lvirect.Height / 2);
				
				// Try finding 5 times in the given direction
				for(int i = 0; i < 5; i++)
				{
					// Move point in given direction
					switch(dir)
					{
						case SearchDirectionHint.Left: spos.X -= list.TileSize.Width / 2; break;
						case SearchDirectionHint.Right: spos.X += list.TileSize.Width / 2; break;
						case SearchDirectionHint.Up: spos.Y -= list.TileSize.Height / 2; break;
						case SearchDirectionHint.Down: spos.Y += list.TileSize.Height / 2; break;
					}
					
					// Test position
					lvi = list.GetItemAt(spos.X, spos.Y);
					if(lvi != null)
					{
						// Select item
						list.SelectedItems.Clear();
						lvi.Selected = true;
						break;
					}
				}
				
				// Make selection visible
				if(list.SelectedItems.Count > 0) list.SelectedItems[0].EnsureVisible();
			}
			
			UpdateTextureSizeLabel();
		}
		
		// This selectes the first item
		private void SelectFirstItem()
		{
			ListViewItem lvi;
			
			// Not when selecting is prevented
			if(preventselection) return;
			
			// Select first
			if(list.Items.Count > 0)
			{
				list.SelectedItems.Clear();
				lvi = list.GetItemAt(list.TileSize.Width / 2, list.TileSize.Height / 2);
				if(lvi != null)
				{
					lvi.Selected = true;
					lvi.EnsureVisible();
				}
			}

			UpdateTextureSizeLabel();
		}
		
		// This adds a group
		public ListViewGroup AddGroup(string name)
		{
			ListViewGroup grp = new ListViewGroup(name);
			list.Groups.Add(grp);
			return grp;
		}
		
		// This begins adding items
		public void BeginAdding(bool keepselectedindex)
		{
            if (bAdding)
            {
                Logger.WriteLogLine("warning: restarted adding");
            }
            bAdding = true;

			if(keepselectedindex && (list.SelectedItems.Count > 0))
				keepselected = list.SelectedIndices[0];
			else
				keepselected = -1;
			
			// Clean list
			items.Clear();
			
			// Stop updating
			refreshtimer.Enabled = false;
		}

		// This ends adding items
		public void EndAdding()
		{
            if (!bAdding)
            {
                Logger.WriteLogLine("warning: attempted to end adding while not having begun");
                return;
            }

            bAdding = false;
			// Fill list with items
			RefillList(true);

			// Start updating
			refreshtimer.Enabled = true;
		}
		
		// This adds an item
		public void Add(string text, ImageData image, object tag, ListViewGroup group)
		{
			ImageBrowserItem i = new ImageBrowserItem(text, image, tag);
			i.ListGroup = group;
			i.Group = group;
            items.Add(i);
		}
		
		// This adds an item
		public void Add(string text, ImageData image, object tag, ListViewGroup group, string tooltiptext)
		{
			ImageBrowserItem i = new ImageBrowserItem(text, image, tag);
			i.ListGroup = group;
			i.Group = group;
			i.ToolTipText = tooltiptext;
			items.Add(i);
		}

		// This fills the list based on the objectname filter
		private void RefillList(bool selectfirst)
		{
            if (bStillLoading)
            {
                return;
            }

            visibleitems = new List<ImageBrowserItem>();
			
			// Begin updating list
			updating = true;
			//list.SuspendLayout();
			list.BeginUpdate();
			
			// Clear list first
			// Group property of items will be set to null, we will restore it later
			list.Items.Clear();
			
			// Go for all items
			foreach(ImageBrowserItem i in items)
			{
				// Add item if valid
				if(ValidateItem(i))
				{
					i.Group = i.ListGroup;
					i.Selected = false;
					visibleitems.Add(i);
				}
			}
			
			// Fill list
			visibleitems.Sort();
			ListViewItem[] array = new ListViewItem[visibleitems.Count];

            int visibleCount = visibleitems.Count;
            for (int i = 0; i < visibleCount; i++) array[i] = visibleitems[i];

			list.Items.AddRange(array);
			
			// Done updating list
			updating = false;
			list.EndUpdate();
			list.Invalidate();
			//list.ResumeLayout();
			
			// Make selection?
			if(!preventselection && (visibleCount > 0))
			{
				// Select specific item?
				if(keepselected > -1)
				{
					list.Items[keepselected].Selected = true;
					list.Items[keepselected].EnsureVisible();
				}
				// Select first item?
				else if(selectfirst)
				{
					SelectFirstItem();
				}
			}
			
			// Raise event
			if((SelectedItemChanged != null) && !preventselection) SelectedItemChanged();
			UpdateTextureSizeLabel();
            
            Console.WriteLine("a");
        }

		// This validates an item
		private bool ValidateItem(ImageBrowserItem i)
		{
			return i.Text.ToUpperInvariant().Contains(objectname.Text);
		}
		
		// This sends the focus to the textbox
		public void FocusTextbox()
		{
			objectname.Focus();
		}
		
		// This updates the texture size label
		private void UpdateTextureSizeLabel()
		{
			if((list.SelectedItems.Count == 0) ||
			   (splitter.Panel2.ClientSize.Width < (texturesize.Location.X + texturesize.Size.Width)))
			{
				texturesizetimer.Start();
			}
			else
			{
				texturesizetimer.Stop();
				ImageBrowserItem lvi = (list.SelectedItems[0] as ImageBrowserItem);
				if(lvi.icon.IsPreviewLoaded)
					texturesize.Text = lvi.icon.Width + " x " + lvi.icon.Height;
				else
					texturesize.Text = "unknown";
				texturesize.Visible = true;
				texturesizelabel.Visible = true;
			}
		}
		
		#endregion
	}
}
