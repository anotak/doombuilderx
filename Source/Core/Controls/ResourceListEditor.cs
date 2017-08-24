
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
using System.Globalization;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class ResourceListEditor : UserControl
	{
		#region ================== Delegates / Events

		public delegate void ContentChanged();

		public event ContentChanged OnContentChanged;

		#endregion

		#region ================== Variables

		private Point dialogoffset = new Point(40, 20);

		#endregion

		#region ================== Properties

		public Point DialogOffset { get { return dialogoffset; } set { dialogoffset = value; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ResourceListEditor()
		{
			// Initialize
			InitializeComponent();
			ResizeColumnHeader();

			// Start with a clear list
			resourceitems.Items.Clear();
		}

		#endregion

		#region ================== Methods

		// This gets the icon index for a resource location type
		private int GetIconIndex(int locationtype, bool locked)
		{
			int lockedaddition;
			
			// Locked?
			if(locked) lockedaddition = (images.Images.Count / 2);
				else lockedaddition = 0;
			
			// What type?
			switch(locationtype)
			{
				case DataLocation.RESOURCE_DIRECTORY:
					return 0 + lockedaddition;

				case DataLocation.RESOURCE_WAD:
					return 1 + lockedaddition;

				case DataLocation.RESOURCE_PK3:
					return 2 + lockedaddition;

				default:
					return -1;
			}
		}
		
		// This will show a fixed list
		public void FixedResourceLocationList(DataLocationList list)
		{
			// Start editing list
			resourceitems.BeginUpdate();
			
			// Go for all items
			for(int i = resourceitems.Items.Count - 1; i >= 0; i--)
			{
				// Remove item if fixed
				if(resourceitems.Items[i].ForeColor != SystemColors.WindowText)
					resourceitems.Items.RemoveAt(i);
			}
			
			// Go for all items
			for(int i = list.Count - 1; i >= 0; i--)
			{
				// Add item as fixed
				resourceitems.Items.Insert(0, new ListViewItem(list[i].location));
				resourceitems.Items[0].Tag = list[i];
				resourceitems.Items[0].ImageIndex = GetIconIndex(list[i].type, true);

				// Set disabled
				resourceitems.Items[0].ForeColor = SystemColors.GrayText;
			}

			// Done
			resourceitems.EndUpdate();
		}

		// This will edit the given list
		public void EditResourceLocationList(DataLocationList list)
		{
			// Start editing list
			resourceitems.BeginUpdate();

			// Scroll to top
			if(resourceitems.Items.Count > 0)
				resourceitems.TopItem = resourceitems.Items[0];
			
			// Go for all items
			for(int i = resourceitems.Items.Count - 1; i >= 0; i--)
			{
				// Remove item unless fixed
				if(resourceitems.Items[i].ForeColor == SystemColors.WindowText)
					resourceitems.Items.RemoveAt(i);
			}

			// Go for all items
			for(int i = 0; i < list.Count; i++)
			{
				// Add item
				AddItem(list[i]);
			}

			// Done
			resourceitems.EndUpdate();
			ResizeColumnHeader();
			
			// Raise content changed event
			if(OnContentChanged != null) OnContentChanged();
		}

		// This adds a normal item
		public void AddResourceLocation(DataLocation rl)
		{
			// Add it
			AddItem(rl);

			// Raise content changed event
			if(OnContentChanged != null) OnContentChanged();
		}

		// This adds a normal item
		private void AddItem(DataLocation rl)
		{
			int index;

			// Start editing list
			resourceitems.BeginUpdate();

			// Add item
			index = resourceitems.Items.Count;
			resourceitems.Items.Add(new ListViewItem(rl.location));
			resourceitems.Items[index].Tag = rl;
			resourceitems.Items[index].ImageIndex = GetIconIndex(rl.type, false);
			
			// Set normal color
			resourceitems.Items[index].ForeColor = SystemColors.WindowText;

			// Done
			resourceitems.EndUpdate();
		}
		
		// This fixes the column header in the list
		private void ResizeColumnHeader()
		{
			// Resize column header to full extend
			column.Width = resourceitems.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
		}

		// When the resources list resizes
		private void resources_SizeChanged(object sender, EventArgs e)
		{
			// Resize column header also
			ResizeColumnHeader();
		}

		// Add a resource
		private void addresource_Click(object sender, EventArgs e)
		{
			ResourceOptionsForm resoptions;
			Rectangle startposition;
			
			// Open resource options dialog
			resoptions = new ResourceOptionsForm(new DataLocation(), "Add Resource");
			resoptions.StartPosition = FormStartPosition.Manual;
			startposition = new Rectangle(dialogoffset.X, dialogoffset.Y, 1, 1);
			startposition = this.RectangleToScreen(startposition);
			Screen screen = Screen.FromPoint(startposition.Location);
			if(startposition.X + resoptions.Size.Width > screen.WorkingArea.Right)
				startposition.X = screen.WorkingArea.Right - resoptions.Size.Width;
			if(startposition.Y + resoptions.Size.Height > screen.WorkingArea.Bottom)
				startposition.Y = screen.WorkingArea.Bottom - resoptions.Size.Height;
			resoptions.Location = startposition.Location;
			if(resoptions.ShowDialog(this) == DialogResult.OK)
			{
				// Add resource
				AddItem(resoptions.ResourceLocation);
			}

			// Raise content changed event
			if(OnContentChanged != null) OnContentChanged();
		}

		// Edit resource
		private void editresource_Click(object sender, EventArgs e)
		{
			ResourceOptionsForm resoptions;
			Rectangle startposition;
			ListViewItem selecteditem;
			DataLocation rl;

			// Anything selected?
			if(resourceitems.SelectedItems.Count > 0)
			{
				// Get selected item
				selecteditem = resourceitems.SelectedItems[0];

				// Open resource options dialog
				resoptions = new ResourceOptionsForm((DataLocation)selecteditem.Tag, "Resource Options");
				resoptions.StartPosition = FormStartPosition.Manual;
				startposition = new Rectangle(dialogoffset.X, dialogoffset.Y, 1, 1);
				startposition = this.RectangleToScreen(startposition);
				Screen screen = Screen.FromPoint(startposition.Location);
				if(startposition.X + resoptions.Size.Width > screen.WorkingArea.Right)
					startposition.X = screen.WorkingArea.Right - resoptions.Size.Width;
				if(startposition.Y + resoptions.Size.Height > screen.WorkingArea.Bottom)
					startposition.Y = screen.WorkingArea.Bottom - resoptions.Size.Height;
				resoptions.Location = startposition.Location;
				if(resoptions.ShowDialog(this) == DialogResult.OK)
				{
					// Start editing list
					resourceitems.BeginUpdate();

					// Update item
					rl = resoptions.ResourceLocation;
					selecteditem.Text = rl.location;
					selecteditem.Tag = rl;
					selecteditem.ImageIndex = GetIconIndex(rl.type, false);
					
					// Done
					resourceitems.EndUpdate();
					
					// Raise content changed event
					if(OnContentChanged != null) OnContentChanged();
				}
			}
		}

		// Remove resource
		private void deleteresource_Click(object sender, EventArgs e)
		{
			// Anything selected?
			if(resourceitems.SelectedItems.Count > 0)
			{
				// Remove it
				resourceitems.Items.Remove(resourceitems.SelectedItems[0]);
				ResizeColumnHeader();

				// Raise content changed event
				if(OnContentChanged != null) OnContentChanged();
			}
		}
		
		// Item selected
		private void resourceitems_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			// Anything selected
			if(resourceitems.SelectedItems.Count > 0)
			{
				// Go for all selected items
				for(int i = resourceitems.SelectedItems.Count - 1; i >= 0; i--)
				{
					// Item grayed? Then deselect.
					if(resourceitems.SelectedItems[i].ForeColor != SystemColors.WindowText)
						resourceitems.SelectedItems[i].Selected = false;
				}
			}

			// Anything selected
			if(resourceitems.SelectedItems.Count > 0)
			{
				// Enable buttons
				editresource.Enabled = true;
				deleteresource.Enabled = true;
			}
			else
			{
				// Disable buttons
				editresource.Enabled = false;
				deleteresource.Enabled = false;
			}
		}

		// When an item is double clicked
		private void resourceitems_DoubleClick(object sender, EventArgs e)
		{
			// Click the edit resource button
			if(editresource.Enabled) editresource_Click(sender, e);
		}

		// Returns a list of the resources
		public DataLocationList GetResources()
		{
			DataLocationList list = new DataLocationList();

			// Go for all items
			for(int i = 0; i < resourceitems.Items.Count; i++)
			{
				// Item not grayed?
				if(resourceitems.Items[i].ForeColor == SystemColors.WindowText)
				{
					// Add item to list
					list.Add((DataLocation)resourceitems.Items[i].Tag);
				}
			}

			// Return result
			return list;
		}

		// Item dragged
		private void resourceitems_DragOver(object sender, DragEventArgs e)
		{
			// Raise content changed event
			if(OnContentChanged != null) OnContentChanged();
		}

		// Item dropped
		private void resourceitems_DragDrop(object sender, DragEventArgs e)
		{
			// Raise content changed event
			if(OnContentChanged != null) OnContentChanged();
		}

		// Client size changed
		private void resourceitems_ClientSizeChanged(object sender, EventArgs e)
		{
			// Resize column header
			ResizeColumnHeader();
		}

		#endregion
	}
}
