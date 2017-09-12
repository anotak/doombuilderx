
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
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class LinedefEditForm : DelayedForm
	{
		// Variables
		private ICollection<Linedef> lines;
		private bool preventchanges = false;
        private int previousaction;

        // Constructor
        public LinedefEditForm()
		{
            previousaction = 0;
			// Initialize
			InitializeComponent();

            if (apply.Location.Y > Height)
            {
                apply.Location = new Point(apply.Location.X, Height - 70);
                cancel.Location = new Point(cancel.Location.X, Height - 70);
            }

            if (tabs.Height > apply.Location.Y - 5)
            {
                tabs.Height = apply.Location.Y - 15;
            }

            // Fill flags list
            foreach (KeyValuePair<string, string> lf in General.Map.Config.LinedefFlags)
				flags.Add(lf.Value, lf.Key);

			// Fill actions list
			action.GeneralizedCategories = General.Map.Config.GenActionCategories;
			action.AddInfo(General.Map.Config.SortedLinedefActions.ToArray());

			// Fill activations list
			activation.Items.AddRange(General.Map.Config.LinedefActivates.ToArray());
			foreach(LinedefActivateInfo ai in General.Map.Config.LinedefActivates) udmfactivates.Add(ai.Title, ai);
			
            // Initialize image selectors
            fronthigh.Initialize();
			frontmid.Initialize();
			frontlow.Initialize();
			backhigh.Initialize();
			backmid.Initialize();
			backlow.Initialize();
			
			// Mixed activations? (UDMF)
			if(General.Map.FormatInterface.HasMixedActivations)
				udmfpanel.Visible = true;
			else if(General.Map.FormatInterface.HasPresetActivations)
				hexenpanel.Visible = true;
			
			// Action arguments?
			if(General.Map.FormatInterface.HasActionArgs)
				argspanel.Visible = true;
			
			// Custom fields?
			if(!General.Map.FormatInterface.HasCustomFields)
				tabs.TabPages.Remove(tabcustom);
			customfrontbutton.Visible = General.Map.FormatInterface.HasCustomFields;
			custombackbutton.Visible = General.Map.FormatInterface.HasCustomFields;
			
			// Arrange panels
			if(General.Map.FormatInterface.HasPresetActivations)
			{
				actiongroup.Height = hexenpanel.Bottom + action.Top + (actiongroup.Width - actiongroup.ClientRectangle.Width);
				this.Height = heightpanel1.Height;
			}
			else if(!General.Map.FormatInterface.HasMixedActivations &&
				    !General.Map.FormatInterface.HasActionArgs &&
				    !General.Map.FormatInterface.HasPresetActivations)
			{
				actiongroup.Height = action.Bottom + action.Top + (actiongroup.Width - actiongroup.ClientRectangle.Width);
				this.Height = heightpanel2.Height;
			}
			
			// Tag?
			if(General.Map.FormatInterface.HasLinedefTag)
			{
				// Match position after the action group
				idgroup.Top = actiongroup.Bottom + actiongroup.Margin.Bottom + idgroup.Margin.Top;
			}
			else
			{
				idgroup.Visible = false;
			}
		}
		
		// This sets up the form to edit the given lines
		public void Setup(ICollection<Linedef> lines)
		{
			LinedefActivateInfo sai;
			Linedef fl;

			preventchanges = true;
			
			// Keep this list
			this.lines = lines;
			if(lines.Count > 1) this.Text = "Edit Linedefs (" + lines.Count + ")";
			
			////////////////////////////////////////////////////////////////////////
			// Set all options to the first linedef properties
			////////////////////////////////////////////////////////////////////////

			// Get first line
			fl = General.GetByIndex(lines, 0);

            // Flags
            foreach (CheckBox c in flags.Checkboxes)
            {
                if (fl.Flags.ContainsKey(c.Tag.ToString()))
                {
                    c.Checked = fl.Flags[c.Tag.ToString()];
                }
                else
                {
                    c.Checked = false;
                }
            }
			
			// Activations
			foreach(LinedefActivateInfo ai in activation.Items)
				if((fl.Activate & ai.Index) == ai.Index) activation.SelectedItem = ai;

			// UDMF Activations
			foreach(CheckBox c in udmfactivates.Checkboxes)
			{
				LinedefActivateInfo ai = (c.Tag as LinedefActivateInfo);
                if (fl.Flags.ContainsKey(ai.Key))
                {
                    c.Checked = fl.Flags[ai.Key];
                }
                else
                {
                    c.Checked = false;
                }
			}

			// Action/tags
			action.Value = fl.Action;
			tag.Text = fl.Tag.ToString();
			arg0.SetValue(fl.Args[0]);
			arg1.SetValue(fl.Args[1]);
			arg2.SetValue(fl.Args[2]);
			arg3.SetValue(fl.Args[3]);
			arg4.SetValue(fl.Args[4]);
			
			// Front side and back side checkboxes
			frontside.Checked = (fl.Front != null);
			backside.Checked = (fl.Back != null);

            // Front settings
            if (fl.Front != null)
            {
                fronthigh.TextureName = fl.Front.HighTexture;
                frontmid.TextureName = fl.Front.MiddleTexture;
                frontlow.TextureName = fl.Front.LowTexture;
                fronthigh.Required = fl.Front.HighRequired();
                frontmid.Required = fl.Front.MiddleRequired();
                frontlow.Required = fl.Front.LowRequired();
                frontsector.Text = fl.Front.Sector.Index.ToString();
                frontoffsetx.Text = fl.Front.OffsetX.ToString();
                frontoffsety.Text = fl.Front.OffsetY.ToString();
            }
            else
            {
                fronthigh.TextureName = string.Empty;
                frontmid.TextureName = string.Empty;
                frontlow.TextureName = string.Empty;
                fronthigh.Required = false;
                frontmid.Required = false;
                frontlow.Required = false;
                frontsector.Text = string.Empty;
                frontoffsetx.Text = string.Empty;
                frontoffsety.Text = string.Empty;
            }

			// Back settings
			if(fl.Back != null)
			{
				backhigh.TextureName = fl.Back.HighTexture;
				backmid.TextureName = fl.Back.MiddleTexture;
				backlow.TextureName = fl.Back.LowTexture;
				backhigh.Required = fl.Back.HighRequired();
				backmid.Required = fl.Back.MiddleRequired();
				backlow.Required = fl.Back.LowRequired();
				backsector.Text = fl.Back.Sector.Index.ToString();
				backoffsetx.Text = fl.Back.OffsetX.ToString();
				backoffsety.Text = fl.Back.OffsetY.ToString();
			}
            else
            {
                backhigh.TextureName = string.Empty;
                backmid.TextureName = string.Empty;
                backlow.TextureName = string.Empty;
                backhigh.Required = false;
                backmid.Required = false;
                backlow.Required = false;
                backsector.Text = string.Empty;
                backoffsetx.Text = string.Empty;
                backoffsety.Text = string.Empty;
            }

            // reset list
            fieldslist.ClearFields();

            // Fill universal fields list
            fieldslist.ListFixedFields(General.Map.Config.LinedefFields);

            // Initialize custom fields editor
            fieldslist.Setup("linedef");

            // Custom fields
            fieldslist.SetValues(fl.Fields, true);

            ////////////////////////////////////////////////////////////////////////
            // Now go for all lines and change the options when a setting is different
            ////////////////////////////////////////////////////////////////////////

            int argResult0 = arg0.GetResult(-1);
            int argResult1 = arg1.GetResult(-1);
            int argResult2 = arg2.GetResult(-1);
            int argResult3 = arg3.GetResult(-1);
            int argResult4 = arg4.GetResult(-1);
            int fltag = fl.Tag;

            // Go for all lines
            foreach (Linedef l in lines)
			{
				// Flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					if(l.Flags.ContainsKey(c.Tag.ToString()))
					{
						if(l.Flags[c.Tag.ToString()] != c.Checked)
						{
							c.ThreeState = true;
							c.CheckState = CheckState.Indeterminate;
						}
					}
				}

				// Activations
				if(activation.Items.Count > 0)
				{
					sai = (activation.Items[0] as LinedefActivateInfo);
					foreach(LinedefActivateInfo ai in activation.Items)
						if((l.Activate & ai.Index) == ai.Index) sai = ai;
					if(sai != activation.SelectedItem) activation.SelectedIndex = -1;
				}

				// UDMF Activations
				foreach(CheckBox c in udmfactivates.Checkboxes)
				{
					LinedefActivateInfo ai = (c.Tag as LinedefActivateInfo);
					if(l.Flags.ContainsKey(ai.Key))
					{
						if(c.Checked != l.Flags[ai.Key])
						{
							c.ThreeState = true;
							c.CheckState = CheckState.Indeterminate;
						}
					}
				}

				// Action/tags
				if(l.Action != action.Value) action.Empty = true;
				if(l.Tag != fltag) tag.Text = "";
                if (l.Args[0] != argResult0) { arg0.ClearValue(); argResult0 = 0; }
                if (l.Args[1] != argResult1) { arg1.ClearValue(); argResult1 = 0; }
                if (l.Args[2] != argResult2) { arg2.ClearValue(); argResult2 = 0; }
                if (l.Args[3] != argResult3) { arg3.ClearValue(); argResult3 = 0; }
                if (l.Args[4] != argResult4) { arg4.ClearValue(); argResult4 = 0;  }
				
				// Front side checkbox
				if((l.Front != null) != frontside.Checked)
				{
					frontside.ThreeState = true;
					frontside.CheckState = CheckState.Indeterminate;
					frontside.AutoCheck = false;
				}

				// Back side checkbox
				if((l.Back != null) != backside.Checked)
				{
					backside.ThreeState = true;
					backside.CheckState = CheckState.Indeterminate;
					backside.AutoCheck = false;
				}

				// Front settings
				if(l.Front != null)
				{
					if(fronthigh.TextureName != l.Front.HighTexture) fronthigh.TextureName = "";
					if(frontmid.TextureName != l.Front.MiddleTexture) frontmid.TextureName = "";
					if(frontlow.TextureName != l.Front.LowTexture) frontlow.TextureName = "";
					if(fronthigh.Required != l.Front.HighRequired()) fronthigh.Required = false;
					if(frontmid.Required != l.Front.MiddleRequired()) frontmid.Required = false;
					if(frontlow.Required != l.Front.LowRequired()) frontlow.Required = false;
					if(frontsector.Text != l.Front.Sector.Index.ToString()) frontsector.Text = "";
					if(frontoffsetx.Text != l.Front.OffsetX.ToString()) frontoffsetx.Text = "";
					if(frontoffsety.Text != l.Front.OffsetY.ToString()) frontoffsety.Text = "";
				}

				// Back settings
				if(l.Back != null)
				{
					if(backhigh.TextureName != l.Back.HighTexture) backhigh.TextureName = "";
					if(backmid.TextureName != l.Back.MiddleTexture) backmid.TextureName = "";
					if(backlow.TextureName != l.Back.LowTexture) backlow.TextureName = "";
					if(backhigh.Required != l.Back.HighRequired()) backhigh.Required = false;
					if(backmid.Required != l.Back.MiddleRequired()) backmid.Required = false;
					if(backlow.Required != l.Back.LowRequired()) backlow.Required = false;
					if(backsector.Text != l.Back.Sector.Index.ToString()) backsector.Text = "";
					if(backoffsetx.Text != l.Back.OffsetX.ToString()) backoffsetx.Text = "";
					if(backoffsety.Text != l.Back.OffsetY.ToString()) backoffsety.Text = "";
					if(General.Map.FormatInterface.HasCustomFields) custombackbutton.Visible = true;
				}
				
				// Custom fields
				fieldslist.SetValues(l.Fields, false);
			}
			
			// Refresh controls so that they show their image
			backhigh.Refresh();
			backmid.Refresh();
			backlow.Refresh();
			fronthigh.Refresh();
			frontmid.Refresh();
			frontlow.Refresh();

			preventchanges = false;
            tabs.SelectedIndex = 0;
        }

        #region ================== Methods
        public void Cleanup()
        {
            lines = null;
            previousaction = 0;
        }
        #endregion

        // Front side (un)checked
        private void frontside_CheckStateChanged(object sender, EventArgs e)
		{
			// Enable/disable panel
			// NOTE: Also enabled when checkbox is grayed!
			frontgroup.Enabled = (frontside.CheckState != CheckState.Unchecked);
		}

		// Back side (un)checked
		private void backside_CheckStateChanged(object sender, EventArgs e)
		{
			// Enable/disable panel
			// NOTE: Also enabled when checkbox is grayed!
			backgroup.Enabled = (backside.CheckState != CheckState.Unchecked);
		}

		// This selects all text in a textbox
		private void SelectAllText(object sender, EventArgs e)
		{
			(sender as TextBox).SelectAll();
		}

		// Apply clicked
		private void apply_Click(object sender, EventArgs e)
		{
			string undodesc = "linedef";
			Sector s;
			int index;
			
			// Verify the tag
			if(General.Map.FormatInterface.HasLinedefTag && ((tag.GetResult(0) < General.Map.FormatInterface.MinTag) || (tag.GetResult(0) > General.Map.FormatInterface.MaxTag)))
			{
				General.ShowWarningMessage("Linedef tag must be between " + General.Map.FormatInterface.MinTag + " and " + General.Map.FormatInterface.MaxTag + ".", MessageBoxButtons.OK);
				return;
			}
			
			// Verify the action
			if((action.Value < General.Map.FormatInterface.MinAction) || (action.Value > General.Map.FormatInterface.MaxAction))
			{
				General.ShowWarningMessage("Linedef action must be between " + General.Map.FormatInterface.MinAction + " and " + General.Map.FormatInterface.MaxAction + ".", MessageBoxButtons.OK);
				return;
			}
			
			// Verify texture offsets
			if((backoffsetx.GetResult(0) < General.Map.FormatInterface.MinTextureOffset) || (backoffsetx.GetResult(0) > General.Map.FormatInterface.MaxTextureOffset) ||
			   (backoffsety.GetResult(0) < General.Map.FormatInterface.MinTextureOffset) || (backoffsety.GetResult(0) > General.Map.FormatInterface.MaxTextureOffset) ||
			   (frontoffsetx.GetResult(0) < General.Map.FormatInterface.MinTextureOffset) || (frontoffsetx.GetResult(0) > General.Map.FormatInterface.MaxTextureOffset) ||
			   (frontoffsety.GetResult(0) < General.Map.FormatInterface.MinTextureOffset) || (frontoffsety.GetResult(0) > General.Map.FormatInterface.MaxTextureOffset))
			{
				General.ShowWarningMessage("Texture offset must be between " + General.Map.FormatInterface.MinTextureOffset + " and " + General.Map.FormatInterface.MaxTextureOffset + ".", MessageBoxButtons.OK);
				return;
			}
			
			// Make undo
			if(lines.Count > 1) undodesc = lines.Count + " linedefs";
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc);
			
			// Go for all the lines
			foreach(Linedef l in lines)
			{
				// Apply all flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					if(c.CheckState == CheckState.Checked) l.SetFlag(c.Tag.ToString(), true);
					else if(c.CheckState == CheckState.Unchecked) l.SetFlag(c.Tag.ToString(), false);
				}
				
				// Apply chosen activation flag
				if(activation.SelectedIndex > -1)
					l.Activate = (activation.SelectedItem as LinedefActivateInfo).Index;
				
				// UDMF activations
				foreach(CheckBox c in udmfactivates.Checkboxes)
				{
					LinedefActivateInfo ai = (c.Tag as LinedefActivateInfo);
					if(c.CheckState == CheckState.Checked) l.SetFlag(ai.Key, true);
					else if(c.CheckState == CheckState.Unchecked) l.SetFlag(ai.Key, false);
				}
				
				// Action/tags
				l.Tag = General.Clamp(tag.GetResult(l.Tag), General.Map.FormatInterface.MinTag, General.Map.FormatInterface.MaxTag);
				if(!action.Empty) l.Action = action.Value;
				l.Args[0] = arg0.GetResult(l.Args[0]);
				l.Args[1] = arg1.GetResult(l.Args[1]);
				l.Args[2] = arg2.GetResult(l.Args[2]);
				l.Args[3] = arg3.GetResult(l.Args[3]);
				l.Args[4] = arg4.GetResult(l.Args[4]);
				
				// Remove front side?
				if((l.Front != null) && (frontside.CheckState == CheckState.Unchecked))
				{
					l.Front.Dispose();
				}
				// Create or modify front side?
				else if(frontside.CheckState == CheckState.Checked)
				{
					// Make sure we have a valid sector (make a new one if needed)
					if(l.Front != null) index = l.Front.Sector.Index; else index = -1;
					index = frontsector.GetResult(index);
					if((index > -1) && (index < General.Map.Map.Sectors.Count))
					{
						s = General.Map.Map.GetSectorByIndex(index);
						if(s == null) s = General.Map.Map.CreateSector();
						if(s != null)
						{
							// Create new sidedef?
							if(l.Front == null) General.Map.Map.CreateSidedef(l, true, s);
							if(l.Front != null)
							{
								// Change sector?
								if(l.Front.Sector != s) l.Front.SetSector(s);

								// Apply settings
								l.Front.OffsetX = General.Clamp(frontoffsetx.GetResult(l.Front.OffsetX), General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset);
								l.Front.OffsetY = General.Clamp(frontoffsety.GetResult(l.Front.OffsetY), General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset);
								l.Front.SetTextureHigh(fronthigh.GetResult(l.Front.HighTexture));
								l.Front.SetTextureMid(frontmid.GetResult(l.Front.MiddleTexture));
								l.Front.SetTextureLow(frontlow.GetResult(l.Front.LowTexture));
							}
						}
					}
				}

				// Remove back side?
				if((l.Back != null) && (backside.CheckState == CheckState.Unchecked))
				{
					l.Back.Dispose();
				}
				// Create or modify back side?
				else if(backside.CheckState == CheckState.Checked)
				{
					// Make sure we have a valid sector (make a new one if needed)
					if(l.Back != null) index = l.Back.Sector.Index; else index = -1;
					index = backsector.GetResult(index);
					if((index > -1) && (index < General.Map.Map.Sectors.Count))
					{
						s = General.Map.Map.GetSectorByIndex(index);
						if(s == null) s = General.Map.Map.CreateSector();
						if(s != null)
						{
							// Create new sidedef?
							if(l.Back == null) General.Map.Map.CreateSidedef(l, false, s);
							if(l.Back != null)
							{
								// Change sector?
								if(l.Back.Sector != s) l.Back.SetSector(s);

								// Apply settings
								l.Back.OffsetX = General.Clamp(backoffsetx.GetResult(l.Back.OffsetX), General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset);
								l.Back.OffsetY = General.Clamp(backoffsety.GetResult(l.Back.OffsetY), General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset);
								l.Back.SetTextureHigh(backhigh.GetResult(l.Back.HighTexture));
								l.Back.SetTextureMid(backmid.GetResult(l.Back.MiddleTexture));
								l.Back.SetTextureLow(backlow.GetResult(l.Back.LowTexture));
							}
						}
					}
				}

				// Custom fields
				fieldslist.Apply(l.Fields);
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
            Cleanup();
			// Done
			General.Map.IsChanged = true;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
            Cleanup();
			// Be gone
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// This finds a new (unused) tag
		private void newtag_Click(object sender, EventArgs e)
		{
			tag.Text = General.Map.Map.GetNewTag().ToString();
		}

		// Action changes
		private void action_ValueChanges(object sender, EventArgs e)
		{
			int showaction = 0;
			
			// Only when line type is known
			if(General.Map.Config.LinedefActions.ContainsKey(action.Value)) showaction = action.Value;
			
			// Change the argument descriptions
			arg0label.Text = General.Map.Config.LinedefActions[showaction].Args[0].Title + ":";
			arg1label.Text = General.Map.Config.LinedefActions[showaction].Args[1].Title + ":";
			arg2label.Text = General.Map.Config.LinedefActions[showaction].Args[2].Title + ":";
			arg3label.Text = General.Map.Config.LinedefActions[showaction].Args[3].Title + ":";
			arg4label.Text = General.Map.Config.LinedefActions[showaction].Args[4].Title + ":";
			arg0label.Enabled = General.Map.Config.LinedefActions[showaction].Args[0].Used;
			arg1label.Enabled = General.Map.Config.LinedefActions[showaction].Args[1].Used;
			arg2label.Enabled = General.Map.Config.LinedefActions[showaction].Args[2].Used;
			arg3label.Enabled = General.Map.Config.LinedefActions[showaction].Args[3].Used;
			arg4label.Enabled = General.Map.Config.LinedefActions[showaction].Args[4].Used;
			if(arg0label.Enabled) arg0.ForeColor = SystemColors.WindowText; else arg0.ForeColor = SystemColors.GrayText;
			if(arg1label.Enabled) arg1.ForeColor = SystemColors.WindowText; else arg1.ForeColor = SystemColors.GrayText;
			if(arg2label.Enabled) arg2.ForeColor = SystemColors.WindowText; else arg2.ForeColor = SystemColors.GrayText;
			if(arg3label.Enabled) arg3.ForeColor = SystemColors.WindowText; else arg3.ForeColor = SystemColors.GrayText;
			if(arg4label.Enabled) arg4.ForeColor = SystemColors.WindowText; else arg4.ForeColor = SystemColors.GrayText;
			arg0.Setup(General.Map.Config.LinedefActions[showaction].Args[0]);
			arg1.Setup(General.Map.Config.LinedefActions[showaction].Args[1]);
			arg2.Setup(General.Map.Config.LinedefActions[showaction].Args[2]);
			arg3.Setup(General.Map.Config.LinedefActions[showaction].Args[3]);
			arg4.Setup(General.Map.Config.LinedefActions[showaction].Args[4]);

            // reset all arguments when linedef action 0 (normal) is chosen or unchosen
            if (!preventchanges && (showaction == 0 || previousaction == 0))
            {
                arg0.SetDefaultValue();
                arg1.SetDefaultValue();
                arg2.SetDefaultValue();
                arg3.SetDefaultValue();
                arg4.SetDefaultValue();
                /*arg0.SetValue(0);
				arg1.SetValue(0);
				arg2.SetValue(0);
				arg3.SetValue(0);
				arg4.SetValue(0);*/
            }

            previousaction = showaction;
        }

		// Browse Action clicked
		private void browseaction_Click(object sender, EventArgs e)
		{
			action.Value = ActionBrowserForm.BrowseAction(this, action.Value);
		}

		// Custom fields on front sides
		private void customfrontbutton_Click(object sender, EventArgs e)
		{
			// Make collection of front sides
			List<MapElement> sides = new List<MapElement>(lines.Count);
			foreach(Linedef l in lines) if(l.Front != null) sides.Add(l.Front);

			// Make undo
			string undodesc = "sidedef";
			if(sides.Count > 1) undodesc = sides.Count + " sidedefs";
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc);
			
			// Edit these
			if(!CustomFieldsForm.ShowDialog(this, "Front side custom fields", "sidedef", sides, General.Map.Config.SidedefFields))
				General.Map.UndoRedo.WithdrawUndo();
		}

		// Custom fields on back sides
		private void custombackbutton_Click(object sender, EventArgs e)
		{
			// Make collection of back sides
			List<MapElement> sides = new List<MapElement>(lines.Count);
			foreach(Linedef l in lines) if(l.Back != null) sides.Add(l.Back);

			// Make undo
			string undodesc = "sidedef";
			if(sides.Count > 1) undodesc = sides.Count + " sidedefs";
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc);
			
			// Edit these
			if(!CustomFieldsForm.ShowDialog(this, "Back side custom fields", "sidedef", sides, General.Map.Config.SidedefFields))
				General.Map.UndoRedo.WithdrawUndo();
		}

		// Help!
		private void LinedefEditForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_linedefedit.html");
			hlpevent.Handled = true;
		}
	}
}
