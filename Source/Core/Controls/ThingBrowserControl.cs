
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
using CodeImp.DoomBuilder.Windows;
using Microsoft.Win32;
using System.Diagnostics;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.IO;
using System.Globalization;
using System.IO;
using CodeImp.DoomBuilder.Compilers;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public partial class ThingBrowserControl : UserControl
	{
		#region ================== Events

		public delegate void TypeChangedDeletegate(ThingTypeInfo value);
		public delegate void TypeDoubleClickDeletegate();

		public event TypeChangedDeletegate OnTypeChanged;
		public event TypeDoubleClickDeletegate OnTypeDoubleClicked;

		#endregion

		#region ================== Variables

		private ICollection<Thing> things;
		private List<TreeNode> nodes;
		private ThingTypeInfo thinginfo;
		private bool doupdatenode;
		private bool doupdatetextbox;
		
		#endregion

		#region ================== Properties

		public string TypeStringValue { get { return typeid.Text; } }

		#endregion

		#region ================== Constructor

		// Constructor
		public ThingBrowserControl()
		{
			InitializeComponent();
		}

		// This sets up the control
		public void Setup()
		{
			// Go for all predefined categories
			typelist.Nodes.Clear();
			nodes = new List<TreeNode>();
			foreach(ThingCategory tc in General.Map.Data.ThingCategories)
			{
				// Create category
				TreeNode cn = typelist.Nodes.Add(tc.Name, tc.Title);
				if((tc.Color >= 0) && (tc.Color < thingimages.Images.Count)) cn.ImageIndex = tc.Color;
				cn.SelectedImageIndex = cn.ImageIndex;
				foreach(ThingTypeInfo ti in tc.Things)
				{
					// Create thing
					TreeNode n = cn.Nodes.Add(ti.Title);
					if((ti.Color >= 0) && (ti.Color < thingimages.Images.Count)) n.ImageIndex = ti.Color;
					n.SelectedImageIndex = n.ImageIndex;
					n.Tag = ti;
					nodes.Add(n);
				}
			}

			doupdatenode = true;
			doupdatetextbox = true;
		}

		#endregion
		
		#region ================== Methods

		// Select a type
		public void SelectType(int type)
		{
			// Set type index
			typeid.Text = type.ToString();
			typeid_TextChanged(this, EventArgs.Empty);
		}

		// Return selected type info
		public ThingTypeInfo GetSelectedInfo()
		{
			return thinginfo;
		}

		// This clears the type
		public void ClearSelectedType()
		{
			doupdatenode = false;

			// Clear selection
			typelist.SelectedNode = null;
			typeid.Text = "";

			// Collapse nodes
			foreach(TreeNode n in nodes)
				if(n.Parent.IsExpanded) n.Parent.Collapse();
			
			doupdatenode = true;
		}

		// Result
		public int GetResult(int original)
		{
			return typeid.GetResult(original);
		}

		#endregion

		#region ================== Events

		// List double-clicked
		private void typelist_DoubleClick(object sender, EventArgs e)
		{
			if(typelist.SelectedNode != null)
			{
				// Node is a child node?
				TreeNode n = typelist.SelectedNode;
				if((n.Nodes.Count == 0) && (n.Tag != null) && (n.Tag is ThingTypeInfo))
				{
					if((OnTypeDoubleClicked != null) && (typeid.Text.Length > 0)) OnTypeDoubleClicked();
				}
			}
		}
		
		// Thing type selection changed
		private void typelist_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if(doupdatetextbox)
			{
				// Anything selected?
				if(typelist.SelectedNode != null)
				{
					TreeNode n = typelist.SelectedNode;

					// Node is a child node?
					if((n.Nodes.Count == 0) && (n.Tag != null) && (n.Tag is ThingTypeInfo))
					{
						ThingTypeInfo ti = (n.Tag as ThingTypeInfo);

						// Show info
						typeid.Text = ti.Index.ToString();
					}
				}
			}
		}

		// Thing type index changed
		private void typeid_TextChanged(object sender, EventArgs e)
		{
			bool knownthing = false;

			// Any text?
			if(typeid.Text.Length > 0)
			{
				// Get the info
				thinginfo = General.Map.Data.GetThingInfoEx(typeid.GetResult(0));
				if(thinginfo != null)
				{
					knownthing = true;

					// Size
					sizelabel.Text = (thinginfo.Radius * 2) + " x " + thinginfo.Height;

					// Hangs from ceiling
					if(thinginfo.Hangs) positionlabel.Text = "Ceiling"; else positionlabel.Text = "Floor";

					// Blocking
					switch(thinginfo.Blocking)
					{
						case ThingTypeInfo.THING_BLOCKING_NONE: blockinglabel.Text = "No"; break;
						case ThingTypeInfo.THING_BLOCKING_FULL: blockinglabel.Text = "Completely"; break;
						case ThingTypeInfo.THING_BLOCKING_HEIGHT: blockinglabel.Text = "True-Height"; break;
						default: blockinglabel.Text = "Unknown"; break;
					}
				}

				if(doupdatenode)
				{
					doupdatetextbox = false;
					int typeindex = typeid.GetResult(0);
					typelist.SelectedNode = null;
					foreach(TreeNode n in nodes)
					{
						// Matching node?
						if((n.Tag as ThingTypeInfo).Index == typeindex)
						{
							// Select this
							n.Parent.Expand();
							typelist.SelectedNode = n;
							n.EnsureVisible();
						}
					}
					doupdatetextbox = true;
				}
			}
			else
			{
				thinginfo = null;
				if(doupdatenode) typelist.SelectedNode = null;
			}

			// No known thing?
			if(!knownthing)
			{
				sizelabel.Text = "-";
				positionlabel.Text = "-";
				blockinglabel.Text = "-";
			}

			// Raise event
			if(OnTypeChanged != null) OnTypeChanged(thinginfo);
		}

		// Layout update!
		private void ThingBrowserControl_Layout(object sender, LayoutEventArgs e)
		{
			ThingBrowserControl_SizeChanged(sender, EventArgs.Empty);
		}

		private void ThingBrowserControl_Resize(object sender, EventArgs e)
		{
			ThingBrowserControl_SizeChanged(sender, EventArgs.Empty);
		}

		private void ThingBrowserControl_SizeChanged(object sender, EventArgs e)
		{
			infopanel.Top = this.ClientSize.Height - infopanel.Height;
			infopanel.Width = this.ClientSize.Width;
			typelist.Width = this.ClientSize.Width;
			typelist.Height = infopanel.Top;

			blockingcaption.Left = infopanel.Width / 2;
			blockinglabel.Left = blockingcaption.Right + blockingcaption.Margin.Right;
			sizecaption.Left = blockingcaption.Right - sizecaption.Width;
			sizelabel.Left = sizecaption.Right + sizecaption.Margin.Right;
		}
		
		#endregion
	}
}
