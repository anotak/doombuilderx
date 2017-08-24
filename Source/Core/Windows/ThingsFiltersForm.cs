
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.IO;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class ThingsFiltersForm : DelayedForm
	{
		#region ================== Variables

		private bool settingup;

		#endregion
		
		#region ================== Constructor

		// Constructor
		public ThingsFiltersForm()
		{
			settingup = true;
			
			// Initialize
			InitializeComponent();
			
			// Fill types list
			List<ThingTypeInfo> thingtypes = new List<ThingTypeInfo>(General.Map.Data.ThingTypes);
			INumberedTitle[] typeitems = new INumberedTitle[thingtypes.Count];
			for(int i = 0; i < thingtypes.Count; i++) typeitems[i] = thingtypes[i];
			filtertype.AddInfo(typeitems);
			
			// Fill the categories combobox
			filtercategory.Items.Add("(any category)");
			filtercategory.Items.AddRange(General.Map.Data.ThingCategories.ToArray());
			
			// Fill actions list
			filteraction.GeneralizedCategories = General.Map.Config.GenActionCategories;
			filteraction.AddInfo(General.Map.Config.SortedLinedefActions.ToArray());
			
			// Initialize custom fields editor
			fieldslist.ListNoFixedFields();
			fieldslist.Setup("thing");
			
			// Fill checkboxes list
			foreach(KeyValuePair<string, string> flag in General.Map.Config.ThingFlags)
			{
				CheckBox box = filterfields.Add(flag.Value, flag.Key);
				box.ThreeState = true;
				box.CheckStateChanged += new EventHandler(filterfield_Check);
			}
			
			// Fill list of filters
			foreach(ThingsFilter f in General.Map.ConfigSettings.ThingsFilters)
			{
				// Make a copy (we don't want to modify the filters until OK is clicked)
				ThingsFilter nf = new ThingsFilter(f);
				
				// Make item in list
				ListViewItem item = new ListViewItem(nf.Name);
				item.Tag = nf;
				listfilters.Items.Add(item);
				
				// Select item if this is the current filter
				if(General.Map.ThingsFilter == f) item.Selected = true;
			}
			
			// Sort the list
			listfilters.Sort();
			
			// Map format specific fields
			filterzheight.Visible = General.Map.FormatInterface.HasThingHeight;
			labelzheight.Visible = General.Map.FormatInterface.HasThingHeight;
			argumentspanel.Visible = General.Map.FormatInterface.HasActionArgs;
			labeltag.Visible = General.Map.FormatInterface.HasThingTag;
			filtertag.Visible = General.Map.FormatInterface.HasThingTag;
			if(!General.Map.FormatInterface.HasCustomFields)
				tabs.TabPages.Remove(tabcustom);
			if(!General.Map.FormatInterface.HasThingAction && !General.Map.FormatInterface.HasThingTag)
				tabs.TabPages.Remove(tabaction);
			
			// Done
			settingup = false;
		}

		#endregion

		#region ================== Management

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Clear all filters and add the new ones
			General.Map.ConfigSettings.ThingsFilters.Clear();
			foreach(ListViewItem item in listfilters.Items)
				General.Map.ConfigSettings.ThingsFilters.Add(item.Tag as ThingsFilter);
			
			// Update stuff
			General.Map.ChangeThingFilter(new NullThingsFilter());
			General.MainWindow.UpdateThingsFilters();
			
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

		// New Filter clicked
		private void addfilter_Click(object sender, EventArgs e)
		{
			ThingsFilter newf = new ThingsFilter();

			// Make item in list and select it
			ListViewItem item = new ListViewItem(newf.Name);
			item.Tag = newf;
			listfilters.Items.Add(item);
			item.Selected = true;

			// Focus on the name field
			filtername.Focus();
			filtername.SelectAll();
		}

		// Delete Selected clicked
		private void deletefilter_Click(object sender, EventArgs e)
		{
			// Anything selected?
			if(listfilters.SelectedItems.Count > 0)
			{
				// Remove item
				listfilters.Items.Remove(listfilters.SelectedItems[0]);
			}
		}
		
		// Item selected
		private void listfilters_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Anything selected?
			if(listfilters.SelectedItems.Count > 0)
			{
				// Get selected filter
				ThingsFilter f = listfilters.SelectedItems[0].Tag as ThingsFilter;

				// Enable settings
				settingup = true;
				deletefilter.Enabled = true;
				filtergroup.Enabled = true;
				
				// Show name
				filtername.Text = f.Name;
				
				// Properties
				foreach(object c in filtercategory.Items)
				{
					ThingCategory tc = (c as ThingCategory);
					if((tc != null) && (tc.Name == f.CategoryName)) filtercategory.SelectedItem = tc;
				}
				if(filtercategory.SelectedIndex == -1) filtercategory.SelectedIndex = 0;
				
				if(f.ThingType > -1)
					filtertype.Value = f.ThingType;
				else
					filtertype.Empty = true;
				
				if(f.ThingAngle > -1)
					filterangle.Text = f.ThingAngle.ToString();
				else
					filterangle.Text = "";
					
				if(f.ThingZHeight > int.MinValue)
					filterzheight.Text = f.ThingZHeight.ToString();
				else
					filterzheight.Text = "";
				
				// Action
				if(f.ThingAction > -1)
					filteraction.Value = f.ThingAction;
				else
					filteraction.Empty = true;
				
				if(f.ThingArgs[0] > -1) arg0.SetValue(f.ThingArgs[0]); else arg0.ClearValue();
				if(f.ThingArgs[1] > -1) arg1.SetValue(f.ThingArgs[1]); else arg1.ClearValue();
				if(f.ThingArgs[2] > -1) arg2.SetValue(f.ThingArgs[2]); else arg2.ClearValue();
				if(f.ThingArgs[3] > -1) arg3.SetValue(f.ThingArgs[3]); else arg3.ClearValue();
				if(f.ThingArgs[4] > -1) arg4.SetValue(f.ThingArgs[4]); else arg4.ClearValue();
				
				if(f.ThingTag > -1)
					filtertag.Text = f.ThingTag.ToString();
				else
					filtertag.Text = "";
				
				// Flags
				foreach(CheckBox b in filterfields.Checkboxes)
				{
					// Field name forbidden?
					if(f.ForbiddenFields.Contains(b.Tag.ToString()))
					{
						b.CheckState = CheckState.Unchecked;
					}
					// Field name required?
					else if(f.RequiredFields.Contains(b.Tag.ToString()))
					{
						b.CheckState = CheckState.Checked;
					}
					else
					{
						b.CheckState = CheckState.Indeterminate;
					}
				}
				
				// Custom fields
				fieldslist.ClearFields();
				fieldslist.Setup("thing");
				fieldslist.SetValues(f.ThingCustomFields, true);
				
				// Done
				settingup = false;
			}
			else
			{
				// Disable filter settings
				deletefilter.Enabled = false;
				filtergroup.Enabled = false;
				filtername.Text = "";
				filtercategory.SelectedIndex = -1;
				foreach(CheckBox c in filterfields.Checkboxes) c.CheckState = CheckState.Indeterminate;
			}
		}

		// Help
		private void ThingsFiltersForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_thingsfilter.html");
			hlpevent.Handled = true;
		}

		#endregion	

		#region ================== Filter Settings

		// Category changed
		private void filtercategory_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Anything selected?
			if(listfilters.SelectedItems.Count > 0)
			{
				// Get selected filter
				ThingsFilter f = listfilters.SelectedItems[0].Tag as ThingsFilter;
				
				// Category selected
				if((filtercategory.SelectedIndex > -1) && (filtercategory.SelectedItem is ThingCategory))
				{
					// Set new category name
					f.CategoryName = (filtercategory.SelectedItem as ThingCategory).Name;
				}
				else
				{
					// Unset category name
					f.CategoryName = "";
				}
			}
		}
		
		// Rename filter
		private void filtername_Validating(object sender, CancelEventArgs e)
		{
			// Anything selected?
			if(listfilters.SelectedItems.Count > 0)
			{
				// Get selected filter
				ThingsFilter f = listfilters.SelectedItems[0].Tag as ThingsFilter;

				// Name changed?
				if(f.Name != filtername.Text)
				{
					// Update name
					f.Name = filtername.Text;
					listfilters.SelectedItems[0].Text = f.Name;
					listfilters.Sort();
				}
			}
		}

		// Field clicked
		private void filterfield_Check(object sender, EventArgs e)
		{
			// Get the checkbox
			CheckBox box = (sender as CheckBox);
			
			// Not setting up?
			if(!settingup)
			{
				// Anything selected?
				if(listfilters.SelectedItems.Count > 0)
				{
					// Get selected filter
					ThingsFilter f = listfilters.SelectedItems[0].Tag as ThingsFilter;
					
					// New state is required?
					if(box.CheckState == CheckState.Checked)
					{
						f.ForbiddenFields.Remove(box.Tag.ToString());
						if(!f.RequiredFields.Contains(box.Tag.ToString())) f.RequiredFields.Add(box.Tag.ToString());
					}
					// New state is forbidden?
					else if(box.CheckState == CheckState.Unchecked)
					{
						f.RequiredFields.Remove(box.Tag.ToString());
						if(!f.ForbiddenFields.Contains(box.Tag.ToString())) f.ForbiddenFields.Add(box.Tag.ToString());
					}
					else
					{
						f.ForbiddenFields.Remove(box.Tag.ToString());
						f.RequiredFields.Remove(box.Tag.ToString());
					}
				}
			}
		}
		
		private void filtertype_ValueChanges(object sender, EventArgs e)
		{
			// Anything selected?
			if(listfilters.SelectedItems.Count > 0)
			{
				// Get selected filter
				ThingsFilter f = listfilters.SelectedItems[0].Tag as ThingsFilter;
				if(filtertype.Empty)
					f.ThingType = -1;
				else
					f.ThingType = filtertype.GetValue();
			}
		}
		
		private void browsetype_Click(object sender, EventArgs e)
		{
			filtertype.Value = ThingBrowserForm.BrowseThing(this, filtertype.Value);
		}
		
		private void filterangle_WhenTextChanged(object sender, EventArgs e)
		{
			// Anything selected?
			if(listfilters.SelectedItems.Count > 0)
			{
				// Get selected filter
				ThingsFilter f = listfilters.SelectedItems[0].Tag as ThingsFilter;
				f.ThingAngle = filterangle.GetResult(-1);
			}
		}
		
		private void browseangle_Click(object sender, EventArgs e)
		{
			AngleForm af = new AngleForm();
			af.Setup(filterangle.GetResult(-1));
			if(af.ShowDialog() == DialogResult.OK)
				filterangle.Text = af.Value.ToString();
		}
		
		private void filterzheight_WhenTextChanged(object sender, EventArgs e)
		{
			// Anything selected?
			if(listfilters.SelectedItems.Count > 0)
			{
				// Get selected filter
				ThingsFilter f = listfilters.SelectedItems[0].Tag as ThingsFilter;
				f.ThingZHeight = filterzheight.GetResult(int.MinValue);
			}
		}

		private void filteraction_ValueChanges(object sender, EventArgs e)
		{
			int showaction = 0;
			ArgumentInfo[] arginfo;

			// Anything selected?
			if(listfilters.SelectedItems.Count > 0)
			{
				// Get selected filter
				ThingsFilter f = listfilters.SelectedItems[0].Tag as ThingsFilter;
				if(filteraction.Empty)
					f.ThingAction = -1;
				else
					f.ThingAction = filteraction.GetValue();
			}
			
			// Only when line type is known, otherwise use the thing arguments
			if(General.Map.Config.LinedefActions.ContainsKey(filteraction.Value)) showaction = filteraction.Value;
			arginfo = General.Map.Config.LinedefActions[showaction].Args;
			
			// Change the argument descriptions
			arg0label.Text = arginfo[0].Title + ":";
			arg1label.Text = arginfo[1].Title + ":";
			arg2label.Text = arginfo[2].Title + ":";
			arg3label.Text = arginfo[3].Title + ":";
			arg4label.Text = arginfo[4].Title + ":";
			arg0label.Enabled = arginfo[0].Used;
			arg1label.Enabled = arginfo[1].Used;
			arg2label.Enabled = arginfo[2].Used;
			arg3label.Enabled = arginfo[3].Used;
			arg4label.Enabled = arginfo[4].Used;
			if(arg0label.Enabled) arg0.ForeColor = SystemColors.WindowText; else arg0.ForeColor = SystemColors.GrayText;
			if(arg1label.Enabled) arg1.ForeColor = SystemColors.WindowText; else arg1.ForeColor = SystemColors.GrayText;
			if(arg2label.Enabled) arg2.ForeColor = SystemColors.WindowText; else arg2.ForeColor = SystemColors.GrayText;
			if(arg3label.Enabled) arg3.ForeColor = SystemColors.WindowText; else arg3.ForeColor = SystemColors.GrayText;
			if(arg4label.Enabled) arg4.ForeColor = SystemColors.WindowText; else arg4.ForeColor = SystemColors.GrayText;
			arg0.Setup(arginfo[0]);
			arg1.Setup(arginfo[1]);
			arg2.Setup(arginfo[2]);
			arg3.Setup(arginfo[3]);
			arg4.Setup(arginfo[4]);
		}
		
		private void browseaction_Click(object sender, EventArgs e)
		{
			filteraction.Value = ActionBrowserForm.BrowseAction(this, filteraction.Value);
		}
		
		private void filtertag_WhenTextChanged(object sender, EventArgs e)
		{
			// Anything selected?
			if(listfilters.SelectedItems.Count > 0)
			{
				// Get selected filter
				ThingsFilter f = listfilters.SelectedItems[0].Tag as ThingsFilter;
				f.ThingTag = filtertag.GetResult(-1);
			}
		}

		private void arg_Validated(object sender, EventArgs e)
		{
			// Anything selected?
			if(listfilters.SelectedItems.Count > 0)
			{
				// Get selected filter
				ThingsFilter f = listfilters.SelectedItems[0].Tag as ThingsFilter;
				
				int index;
				int.TryParse((sender as Control).Tag.ToString(), out index);
				ArgumentBox filterarg = (sender as ArgumentBox);
				f.ThingArgs[index] = filterarg.GetResult(-1);
			}
		}
		
		private void fieldslist_Validated(object sender, EventArgs e)
		{
			// Anything selected?
			if(listfilters.SelectedItems.Count > 0)
			{
				// Get selected filter
				ThingsFilter f = listfilters.SelectedItems[0].Tag as ThingsFilter;
				fieldslist.Apply(f.ThingCustomFields);
			}
		}
		
		#endregion
	}
}