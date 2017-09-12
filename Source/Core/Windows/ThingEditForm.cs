
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
	/// <summary>
	/// Dialog window that allows viewing and editing of Thing properties.
	/// </summary>
	public partial class ThingEditForm : DelayedForm
	{
		#region ================== Variables

		private ICollection<Thing> things;
		private List<TreeNode> nodes;
		private ThingTypeInfo thinginfo;
		private bool preventchanges = false;
        private int previousaction;
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor

		// Constructor
		public ThingEditForm()
		{
            //long start = General.stopwatch.ElapsedMilliseconds;
			// Initialize
			InitializeComponent();

            if (apply.Location.Y > Height)
            {
                apply.Location = new Point(apply.Location.X, Height - 70);
                cancel.Location = new Point(cancel.Location.X, Height - 70);
            }

            if(tabs.Height > apply.Location.Y - 5)
            {
                tabs.Height = apply.Location.Y - 15;
            }
            previousaction = 0;
			// Fill flags list
			foreach(KeyValuePair<string, string> tf in General.Map.Config.ThingFlags)
				flags.Add(tf.Value, tf.Key);

			// Fill actions list
			action.GeneralizedCategories = General.Map.Config.GenActionCategories;
			action.AddInfo(General.Map.Config.SortedLinedefActions.ToArray());
            
			// Custom fields?
			if(!General.Map.FormatInterface.HasCustomFields)
				tabs.TabPages.Remove(tabcustom);
			
			// Tag/Effects?
			if(!General.Map.FormatInterface.HasThingAction && !General.Map.FormatInterface.HasThingTag)
				tabs.TabPages.Remove(tabeffects);
			
			// Thing height?
			height.Visible = General.Map.FormatInterface.HasThingHeight;
			heightlabel.Visible = General.Map.FormatInterface.HasThingHeight;
			
			// Setup types list
			thingtype.Setup();
            //Logger.WriteLogLine("thingeditformtime: " + (General.stopwatch.ElapsedMilliseconds - start));

        }

		// This sets up the form to edit the given things
		public void Setup(ICollection<Thing> things)
		{
			Thing ft;

			preventchanges = true;

			// Keep this list
			this.things = things;
			if(things.Count > 1) this.Text = "Edit Things (" + things.Count + ")";
			
			////////////////////////////////////////////////////////////////////////
			// Set all options to the first thing properties
			////////////////////////////////////////////////////////////////////////

			ft = General.GetByIndex(things, 0);
			
			// Set type
			thingtype.SelectType(ft.Type);

            // Flags
            foreach (CheckBox c in flags.Checkboxes)
            {
                if (ft.Flags.ContainsKey(c.Tag.ToString()))
                {
                    c.Checked = ft.Flags[c.Tag.ToString()];
                } else {
                    c.Checked = false;
                }
            }
			
			// Coordination
			angle.Text = ft.AngleDoom.ToString();
			height.Text = ((int)ft.Position.z).ToString();
			
			// Action/tags
			action.Value = ft.Action;
			tag.Text = ft.Tag.ToString();
			arg0.SetValue(ft.Args[0]);
			arg1.SetValue(ft.Args[1]);
			arg2.SetValue(ft.Args[2]);
			arg3.SetValue(ft.Args[3]);
			arg4.SetValue(ft.Args[4]);

            if (General.Map.FormatInterface.HasCustomFields)
            {
                fieldslist.ClearFields();

                // Fill universal fields list
                fieldslist.ListFixedFields(General.Map.Config.ThingFields);

                // Initialize custom fields editor
                fieldslist.Setup("thing");

                // Custom fields
                fieldslist.SetValues(ft.Fields, true);
            }

            ////////////////////////////////////////////////////////////////////////
            // Now go for all lines and change the options when a setting is different
            ////////////////////////////////////////////////////////////////////////

            int argResult0 = arg0.GetResult(-1);
            int argResult1 = arg1.GetResult(-1);
            int argResult2 = arg2.GetResult(-1);
            int argResult3 = arg3.GetResult(-1);
            int argResult4 = arg4.GetResult(-1);
            int fttag = ft.Tag;

            // Go for all things
            foreach (Thing t in things)
			{
				// Type does not match?
				if((thingtype.GetSelectedInfo() != null) &&
				   (thingtype.GetSelectedInfo().Index != t.Type))
					thingtype.ClearSelectedType();
				
				// Flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					if(t.Flags.ContainsKey(c.Tag.ToString()))
					{
						if(t.Flags[c.Tag.ToString()] != c.Checked)
						{
							c.ThreeState = true;
							c.CheckState = CheckState.Indeterminate;
						}
					}
				}
				
				// Coordination
				if(t.AngleDoom.ToString() != angle.Text) angle.Text = "";
				if(((int)t.Position.z).ToString() != height.Text) height.Text = "";

				// Action/tags
				if(t.Action != action.Value) action.Empty = true;
                if (t.Tag != fttag) tag.Text = "";
                if (t.Args[0] != argResult0) { arg0.ClearValue(); argResult0 = 0; }
                if (t.Args[1] != argResult1) { arg1.ClearValue(); argResult1 = 0; }
                if (t.Args[2] != argResult2) { arg2.ClearValue(); argResult2 = 0; }
                if (t.Args[3] != argResult3) { arg3.ClearValue(); argResult3 = 0; }
                if (t.Args[4] != argResult4) { arg4.ClearValue(); argResult4 = 0; }

                // Custom fields
                fieldslist.SetValues(t.Fields, false);
			}

			preventchanges = false;
		}

        #endregion

        #region ================== Methods
        public void Cleanup()
        {
            things = null;
            previousaction = 0;
        }
        #endregion

        #region ================== Interface

        // This finds a new (unused) tag
        private void newtag_Click(object sender, EventArgs e)
		{
			tag.Text = General.Map.Map.GetNewTag().ToString();
		}

		// Selected type changes
		private void thingtype_OnTypeChanged(ThingTypeInfo value)
		{
			thinginfo = value;

			// Update preview image
			if(thinginfo != null)
			{
				if(thinginfo.Sprite.ToLowerInvariant().StartsWith(DataManager.INTERNAL_PREFIX) &&
				   (thinginfo.Sprite.Length > DataManager.INTERNAL_PREFIX.Length))
				{
					General.DisplayZoomedImage(spritetex, General.Map.Data.GetSpriteImage(thinginfo.Sprite).GetBitmap());
				}
				else if((thinginfo.Sprite.Length <= 8) && (thinginfo.Sprite.Length > 0))
				{
					General.DisplayZoomedImage(spritetex, General.Map.Data.GetSpriteImage(thinginfo.Sprite).GetPreview());
				}
				else
				{
					spritetex.BackgroundImage = null;
				}
			}
			else
			{
				spritetex.BackgroundImage = null;
			}
			
			// Update arguments
			action_ValueChanges(this, EventArgs.Empty);
		}
		
		// Action changes
		private void action_ValueChanges(object sender, EventArgs e)
		{
			int showaction = 0;
			ArgumentInfo[] arginfo;

			// Only when line type is known, otherwise use the thing arguments
			if(General.Map.Config.LinedefActions.ContainsKey(action.Value)) showaction = action.Value;
			if((showaction == 0) && (thinginfo != null)) arginfo = thinginfo.Args; else arginfo = General.Map.Config.LinedefActions[showaction].Args;
			
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

			// reset all arguments when linedef action 0 (normal) is chosen or unchosen
			if(!preventchanges && (showaction == 0 || previousaction == 0))
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

		// Angle text changes
		private void angle_TextChanged(object sender, EventArgs e)
		{
			anglecontrol.Value = angle.GetResult(int.MinValue);
		}

		// Angle control clicked
		private void anglecontrol_ButtonClicked(object sender, EventArgs e)
		{
			angle.Text = anglecontrol.Value.ToString();
		}

		// Apply clicked
		private void apply_Click(object sender, EventArgs e)
		{
			List<string> defaultflags = new List<string>();
			string undodesc = "thing";

			// Verify the tag
			if(General.Map.FormatInterface.HasThingTag && ((tag.GetResult(0) < General.Map.FormatInterface.MinTag) || (tag.GetResult(0) > General.Map.FormatInterface.MaxTag)))
			{
				General.ShowWarningMessage("Thing tag must be between " + General.Map.FormatInterface.MinTag + " and " + General.Map.FormatInterface.MaxTag + ".", MessageBoxButtons.OK);
				return;
			}

			// Verify the type
			if(((thingtype.GetResult(0) < General.Map.FormatInterface.MinThingType) || (thingtype.GetResult(0) > General.Map.FormatInterface.MaxThingType)))
			{
				General.ShowWarningMessage("Thing type must be between " + General.Map.FormatInterface.MinThingType + " and " + General.Map.FormatInterface.MaxThingType + ".", MessageBoxButtons.OK);
				return;
			}

			// Verify the action
			if(General.Map.FormatInterface.HasThingAction && ((action.Value < General.Map.FormatInterface.MinAction) || (action.Value > General.Map.FormatInterface.MaxAction)))
			{
				General.ShowWarningMessage("Thing action must be between " + General.Map.FormatInterface.MinAction + " and " + General.Map.FormatInterface.MaxAction + ".", MessageBoxButtons.OK);
				return;
			}

			// Make undo
			if(things.Count > 1) undodesc = things.Count + " things";
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc);
			
			// Go for all the things
			foreach(Thing t in things)
			{
				// Thing type index
				t.Type = General.Clamp(thingtype.GetResult(t.Type), General.Map.FormatInterface.MinThingType, General.Map.FormatInterface.MaxThingType);
				
				// Coordination
				t.Rotate(angle.GetResult(t.AngleDoom));
				t.Move(t.Position.x, t.Position.y, (float)height.GetResult((int)t.Position.z));
				
				// Apply all flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					if(c.CheckState == CheckState.Checked) t.SetFlag(c.Tag.ToString(), true);
					else if(c.CheckState == CheckState.Unchecked) t.SetFlag(c.Tag.ToString(), false);
				}

				// Action/tags
				t.Tag = tag.GetResult(t.Tag);
				if(!action.Empty) t.Action = action.Value;
				t.Args[0] = arg0.GetResult(t.Args[0]);
				t.Args[1] = arg1.GetResult(t.Args[1]);
				t.Args[2] = arg2.GetResult(t.Args[2]);
				t.Args[3] = arg3.GetResult(t.Args[3]);
				t.Args[4] = arg4.GetResult(t.Args[4]);
				
				// Custom fields
				fieldslist.Apply(t.Fields);
				
				// Update settings
				t.UpdateConfiguration();
			}

			// Set as defaults
			foreach(CheckBox c in flags.Checkboxes)
				if(c.CheckState == CheckState.Checked) defaultflags.Add(c.Tag.ToString());
			General.Settings.DefaultThingType = thingtype.GetResult(General.Settings.DefaultThingType);
			General.Settings.DefaultThingAngle = Angle2D.DegToRad((float)angle.GetResult((int)Angle2D.RadToDeg(General.Settings.DefaultThingAngle) - 90) + 90);
			General.Settings.SetDefaultThingFlags(defaultflags);
			
			// Done
			General.Map.IsChanged = true;
            Cleanup();
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
            // Be gone
            Cleanup();
            this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Help
		private void ThingEditForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_thingeditor.html");
			hlpevent.Handled = true;
		}
		
		#endregion
	}
}