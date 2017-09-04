
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
using CodeImp.DoomBuilder.Controls;
using System.Globalization;
using CodeImp.DoomBuilder.Data;
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class PreferencesForm : DelayedForm
	{
		#region ================== Variables

		private PreferencesController controller;
		private bool allowapplycontrol = false;
		private bool disregardshift = false;
		private bool disregardcontrol = false;

		private bool reloadresources = false;
		
		#endregion

		#region ================== Properties

		public bool ReloadResources { get { return reloadresources; } }

		#endregion

		#region ================== Constructor

		// Constructor
		public PreferencesForm()
		{
			Actions.Action[] actions;
			ListViewItem item;
			
			// Initialize
			InitializeComponent();
			
			// Interface
			imagebrightness.Value = General.Settings.ImageBrightness;
			squarethings.Checked = General.Settings.SquareThings;
			doublesidedalpha.Value = (int)((1.0f - General.Settings.DoubleSidedAlpha) * 10.0f);
			defaultviewmode.SelectedIndex = General.Settings.DefaultViewMode;
			fieldofview.Value = General.Settings.VisualFOV / 10;
			mousespeed.Value = General.Settings.MouseSpeed / 100;
			movespeed.Value = General.Settings.MoveSpeed / 100;
			viewdistance.Value = General.Clamp((int)(General.Settings.ViewDistance / 200.0f), viewdistance.Minimum, viewdistance.Maximum);
			invertyaxis.Checked = General.Settings.InvertYAxis;
			scriptfontbold.Checked = General.Settings.ScriptFontBold;
			scriptontop.Checked = General.Settings.ScriptOnTop;
			scripttabwidth.Text = General.Settings.ScriptTabWidth.ToString();
			scriptautoindent.Checked = General.Settings.ScriptAutoIndent;
			previewsize.Value = General.Settings.PreviewImageSize;
			autoscrollspeed.Value = General.Settings.AutoScrollSpeed;
			zoomfactor.Value = General.Settings.ZoomFactor;
			animatevisualselection.Checked = General.Settings.AnimateVisualSelection;
			dockersposition.SelectedIndex = General.Settings.DockersPosition;
			collapsedockers.Checked = General.Settings.CollapseDockers;
			toolbar_file.Checked = General.Settings.ToolbarFile;
			toolbar_script.Checked = General.Settings.ToolbarScript;
			toolbar_undo.Checked = General.Settings.ToolbarUndo;
			toolbar_copy.Checked = General.Settings.ToolbarCopy;
			toolbar_prefabs.Checked = General.Settings.ToolbarPrefabs;
			toolbar_filter.Checked = General.Settings.ToolbarFilter;
			toolbar_viewmodes.Checked = General.Settings.ToolbarViewModes;
			toolbar_geometry.Checked = General.Settings.ToolbarGeometry;
			toolbar_testing.Checked = General.Settings.ToolbarTesting;
			showtexturesizes.Checked = General.Settings.ShowTextureSizes;

            // ano - defaults tab
            defaultTextureBox.Text = General.Settings.SaveDefaultTexture;
            defaultFloorBox.Text = General.Settings.SaveDefaultFloorTexture;
            defaultCeilingBox.Text = General.Settings.SaveDefaultCeilingTexture;
            randomizedefaultsBox.Checked = General.Settings.RandomizeDefaultSectors;

            // Fill fonts list
            scriptfontname.BeginUpdate();
			foreach(FontFamily ff in System.Drawing.FontFamily.Families)
				scriptfontname.Items.Add(ff.Name);
			scriptfontname.EndUpdate();
			
			// Select script font name
			for(int i = 0; i < scriptfontname.Items.Count; i++)
			{
				if(string.Compare(scriptfontname.Items[i].ToString(), General.Settings.ScriptFontName, true) == 0)
					scriptfontname.SelectedIndex = i;
			}

			// Select script font size
			for(int i = 0; i < scriptfontsize.Items.Count; i++)
			{
				if(string.Compare(scriptfontsize.Items[i].ToString(), General.Settings.ScriptFontSize.ToString(CultureInfo.InvariantCulture), true) == 0)
					scriptfontsize.SelectedIndex = i;
			}
			
			// Fill actions list with categories
			foreach(KeyValuePair<string, string> c in General.Actions.Categories)
				listactions.Groups.Add(c.Key, c.Value);
			
			// Fill list of actions
			actions = General.Actions.GetAllActions();
			foreach(Actions.Action a in actions)
			{
				// Create item
				item = listactions.Items.Add(a.Name, a.Title, 0);
				item.SubItems.Add(Actions.Action.GetShortcutKeyDesc(a.ShortcutKey));
				item.SubItems[1].Tag = a.ShortcutKey;

				// Put in category, if the category exists
				if(General.Actions.Categories.ContainsKey(a.Category))
					item.Group = listactions.Groups[a.Category];
			}

			// Set the colors
			// TODO: Make this automated by using the collection
			colorbackcolor.Color = General.Colors.Background;
			colorvertices.Color = General.Colors.Vertices;
			colorlinedefs.Color = General.Colors.Linedefs;
			colorspeciallinedefs.Color = General.Colors.Actions;
			colorsoundlinedefs.Color = General.Colors.Sounds;
			colorhighlight.Color = General.Colors.Highlight;
			colorselection.Color = General.Colors.Selection;
			colorindication.Color = General.Colors.Indication;
			colorgrid.Color = General.Colors.Grid;
			colorgrid64.Color = General.Colors.Grid64;
			colorscriptbackground.Color = General.Colors.ScriptBackground;
			colorlinenumbers.Color = General.Colors.LineNumbers;
			colorplaintext.Color = General.Colors.PlainText;
			colorcomments.Color = General.Colors.Comments;
			colorkeywords.Color = General.Colors.Keywords;
			colorliterals.Color = General.Colors.Literals;
			colorconstants.Color = General.Colors.Constants;
			blackbrowsers.Checked = General.Settings.BlackBrowsers;
			classicbilinear.Checked = General.Settings.ClassicBilinear;
			visualbilinear.Checked = General.Settings.VisualBilinear;
			qualitydisplay.Checked = General.Settings.QualityDisplay;
			
			// Paste options
			pasteoptions.Setup(General.Settings.PasteOptions.Copy());

			// Allow plugins to add tabs
			this.SuspendLayout();
			controller = new PreferencesController(this);
			controller.AllowAddTab = true;
			General.Plugins.OnShowPreferences(controller);
			controller.AllowAddTab = false;
			this.ResumeLayout(true);
			
			// Done
			allowapplycontrol = true;
		}

		#endregion

		#region ================== OK / Cancel

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Let the plugins know
			controller.RaiseAccept();
			
			// Check if we need to reload the resources
			reloadresources |= (General.Settings.ImageBrightness != imagebrightness.Value);
			reloadresources |= (General.Settings.PreviewImageSize != previewsize.Value);

			// Apply interface
			General.Settings.ImageBrightness = imagebrightness.Value;
			General.Settings.SquareThings = squarethings.Checked;
			General.Settings.DoubleSidedAlpha = 1.0f - (float)(doublesidedalpha.Value * 0.1f);
			General.Settings.DefaultViewMode = defaultviewmode.SelectedIndex;
			General.Settings.VisualFOV = fieldofview.Value * 10;
			General.Settings.MouseSpeed = mousespeed.Value * 100;
			General.Settings.MoveSpeed = movespeed.Value * 100;
			General.Settings.ViewDistance = (float)viewdistance.Value * 200.0f;
			General.Settings.InvertYAxis = invertyaxis.Checked;
			General.Settings.ScriptFontBold = scriptfontbold.Checked;
			General.Settings.ScriptFontName = scriptfontname.Text;
			General.Settings.ScriptOnTop = scriptontop.Checked;
			General.Settings.ScriptTabWidth = scripttabwidth.GetResult(General.Settings.ScriptTabWidth);
			General.Settings.ScriptAutoIndent = scriptautoindent.Checked;
			General.Settings.PreviewImageSize = previewsize.Value;
			General.Settings.AutoScrollSpeed = autoscrollspeed.Value;
			General.Settings.ZoomFactor = zoomfactor.Value;
			General.Settings.AnimateVisualSelection = animatevisualselection.Checked;
			General.Settings.DockersPosition = dockersposition.SelectedIndex;
			General.Settings.CollapseDockers = collapsedockers.Checked;
			General.Settings.ToolbarFile = toolbar_file.Checked;
			General.Settings.ToolbarScript = toolbar_script.Checked;
			General.Settings.ToolbarUndo = toolbar_undo.Checked;
			General.Settings.ToolbarCopy = toolbar_copy.Checked;
			General.Settings.ToolbarPrefabs = toolbar_prefabs.Checked;
			General.Settings.ToolbarFilter = toolbar_filter.Checked;
			General.Settings.ToolbarViewModes = toolbar_viewmodes.Checked;
			General.Settings.ToolbarGeometry = toolbar_geometry.Checked;
			General.Settings.ToolbarTesting = toolbar_testing.Checked;
			General.Settings.ShowTextureSizes = showtexturesizes.Checked;

            // ano - defaults tab
            General.Settings.SaveDefaultTexture = defaultTextureBox.Text;
            General.Settings.SaveDefaultFloorTexture = defaultFloorBox.Text;
            General.Settings.SaveDefaultCeilingTexture = defaultCeilingBox.Text;
            General.Settings.RandomizeDefaultSectors = randomizedefaultsBox.Checked;

            // Script font size
            int fontsize = 8;
			int.TryParse(scriptfontsize.Text, out fontsize);
			General.Settings.ScriptFontSize = fontsize;
			
			// Apply control keys to actions
			foreach(ListViewItem item in listactions.Items)
				General.Actions[item.Name].SetShortcutKey((int)item.SubItems[1].Tag);

			// Apply the colors
			// TODO: Make this automated by using the collection
			General.Colors.Background = colorbackcolor.Color;
			General.Colors.Vertices = colorvertices.Color;
			General.Colors.Linedefs = colorlinedefs.Color;
			General.Colors.Actions = colorspeciallinedefs.Color;
			General.Colors.Sounds = colorsoundlinedefs.Color;
			General.Colors.Highlight = colorhighlight.Color;
			General.Colors.Selection = colorselection.Color;
			General.Colors.Indication = colorindication.Color;
			General.Colors.Grid = colorgrid.Color;
			General.Colors.Grid64 = colorgrid64.Color;
			General.Colors.ScriptBackground = colorscriptbackground.Color;
			General.Colors.LineNumbers = colorlinenumbers.Color;
			General.Colors.PlainText = colorplaintext.Color;
			General.Colors.Comments = colorcomments.Color;
			General.Colors.Keywords = colorkeywords.Color;
			General.Colors.Literals = colorliterals.Color;
			General.Colors.Constants = colorconstants.Color;
			General.Colors.CreateAssistColors();
			General.Settings.BlackBrowsers = blackbrowsers.Checked;
			General.Settings.ClassicBilinear = classicbilinear.Checked;
			General.Settings.VisualBilinear = visualbilinear.Checked;
			General.Settings.QualityDisplay = qualitydisplay.Checked;
			
			// Paste options
			General.Settings.PasteOptions = pasteoptions.GetOptions();
			
			// Let the plugins know we're closing
			General.Plugins.OnClosePreferences(controller);
			
			// Close
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Let the plugins know
			controller.RaiseCancel();

			// Let the plugins know we're closing
			General.Plugins.OnClosePreferences(controller);

			// Close
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		#endregion

		#region ================== Tabs

		// This adds a tab page
		public void AddTabPage(TabPage tab)
		{
			tabs.TabPages.Add(tab);
		}

		// Tab changes
		private void tabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Enable/disable stuff with tabs
			if(tabs.SelectedTab != tabkeys)
			{
				this.AcceptButton = apply;
				this.CancelButton = cancel;
				apply.TabStop = true;
				cancel.TabStop = true;
				tabs.TabStop = true;
			}
			else
			{
				this.AcceptButton = null;
				this.CancelButton = null;
				apply.TabStop = false;
				cancel.TabStop = false;
				tabs.TabStop = false;
			}
			
			colorsgroup1.Visible = (tabs.SelectedTab == tabcolors);
			//colorsgroup2.Visible = (tabs.SelectedTab == tabcolors);
			colorsgroup3.Visible = (tabs.SelectedTab == tabcolors);
		}

		#endregion

		#region ================== Interface Panel

		private void previewsize_ValueChanged(object sender, EventArgs e)
		{
			int size = PreviewManager.PREVIEW_SIZES[previewsize.Value];
			previewsizelabel.Text = size.ToString() + " x " + size.ToString();
		}
		
		private void fieldofview_ValueChanged(object sender, EventArgs e)
		{
			int value = fieldofview.Value * 10;
			fieldofviewlabel.Text = value.ToString() + (char)176;
		}

		private void mousespeed_ValueChanged(object sender, EventArgs e)
		{
			int value = mousespeed.Value * 100;
			mousespeedlabel.Text = value.ToString();
		}

		private void movespeed_ValueChanged(object sender, EventArgs e)
		{
			int value = movespeed.Value * 100;
			movespeedlabel.Text = value.ToString();
		}

		private void viewdistance_ValueChanged(object sender, EventArgs e)
		{
			int value = viewdistance.Value * 200;
			viewdistancelabel.Text = value.ToString() + " mp";
		}

		private void scriptfontname_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateScriptFontPreview();
		}

		private void scriptfontsize_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateScriptFontPreview();
		}

		private void scriptfontbold_CheckedChanged(object sender, EventArgs e)
		{
			UpdateScriptFontPreview();
		}

		private void autoscrollspeed_ValueChanged(object sender, EventArgs e)
		{
			if(autoscrollspeed.Value == 0)
				autoscrollspeedlabel.Text = "Off";
			else
				autoscrollspeedlabel.Text = autoscrollspeed.Value.ToString() + "x";
		}

		private void zoomfactor_ValueChanged(object sender, EventArgs e)
		{
			zoomfactorlabel.Text = (zoomfactor.Value * 10).ToString() + "%";
		}

		// This updates the script font preview label
		private void UpdateScriptFontPreview()
		{
			if((scriptfontname.SelectedIndex > -1) &&
			   (scriptfontsize.SelectedIndex > -1))
			{
				scriptfontlabel.Text = scriptfontname.Text;
				scriptfontlabel.BackColor = General.Colors.ScriptBackground.ToColor();
				scriptfontlabel.ForeColor = General.Colors.PlainText.ToColor();
				FontFamily ff = new FontFamily(scriptfontname.Text);
				FontStyle style = FontStyle.Regular;
				if(scriptfontbold.Checked)
				{
					// Prefer bold over regular
					if(ff.IsStyleAvailable(FontStyle.Bold))
						style = FontStyle.Bold;
					else if(ff.IsStyleAvailable(FontStyle.Regular))
						style = FontStyle.Regular;
					else if(ff.IsStyleAvailable(FontStyle.Italic))
						style = FontStyle.Italic;
					else if(ff.IsStyleAvailable(FontStyle.Underline))
						style = FontStyle.Underline;
					else if(ff.IsStyleAvailable(FontStyle.Strikeout))
						style = FontStyle.Strikeout;
				}
				else
				{
					// Prefer regular over bold
					if(ff.IsStyleAvailable(FontStyle.Regular))
						style = FontStyle.Regular;
					else if(ff.IsStyleAvailable(FontStyle.Bold))
						style = FontStyle.Bold;
					else if(ff.IsStyleAvailable(FontStyle.Italic))
						style = FontStyle.Italic;
					else if(ff.IsStyleAvailable(FontStyle.Underline))
						style = FontStyle.Underline;
					else if(ff.IsStyleAvailable(FontStyle.Strikeout))
						style = FontStyle.Strikeout;
				}
				int fontsize = 8;
				int.TryParse(scriptfontsize.Text, out fontsize);
				if(ff.IsStyleAvailable(style))
					scriptfontlabel.Font = new Font(scriptfontname.Text, (float)fontsize, style);
			}
		}

		#endregion
		
		#region ================== Controls Panel
		
		// This updates the used keys info
		private void UpdateKeyUsedActions()
		{
			List<string> usedactions = new List<string>();
			
			// Anything selected?
			if(listactions.SelectedItems.Count > 0)
			{
				// Get info
				int thiskey = (int)listactions.SelectedItems[0].SubItems[1].Tag;
				if(thiskey != 0)
				{
					// Find actions with same key
					foreach(ListViewItem item in listactions.Items)
					{
						// Don't count the selected action
						if(item != listactions.SelectedItems[0])
						{
							Actions.Action a = General.Actions[item.Name];
							int akey = (int)item.SubItems[1].Tag;

							// Check if the key combination matches
							if((thiskey & a.ShortcutMask) == (akey & a.ShortcutMask))
								usedactions.Add(a.Title + "  (" + General.Actions.Categories[a.Category] + ")");
						}
					}
				}
			}
			
			// Update info
			if(usedactions.Count == 0)
			{
				keyusedlabel.Visible = false;
				keyusedlist.Visible = false;
				keyusedlist.Items.Clear();
			}
			else
			{
				keyusedlist.Items.Clear();
				foreach(string a in usedactions) keyusedlist.Items.Add(a);
				keyusedlabel.Visible = true;
				keyusedlist.Visible = true;
			}
		}
		
		// This fills the list of available controls for the specified action
		private void FillControlsList(Actions.Action a)
		{
			actioncontrol.Items.Clear();
			
			// Fill combobox with special controls
			if(a.AllowMouse)
			{
				actioncontrol.Items.Add(new KeyControl(Keys.LButton, "LButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.MButton, "MButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.RButton, "RButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton1, "XButton1"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton2, "XButton2"));
			}
			if(a.AllowScroll)
			{
				actioncontrol.Items.Add(new KeyControl(SpecialKeys.MScrollUp, "ScrollUp"));
				actioncontrol.Items.Add(new KeyControl(SpecialKeys.MScrollDown, "ScrollDown"));
			}
			if(a.AllowMouse && !a.DisregardShift)
			{
				actioncontrol.Items.Add(new KeyControl(Keys.LButton | Keys.Shift, "Shift+LButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.MButton | Keys.Shift, "Shift+MButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.RButton | Keys.Shift, "Shift+RButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton1 | Keys.Shift, "Shift+XButton1"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton2 | Keys.Shift, "Shift+XButton2"));
			}
			if(a.AllowScroll && !a.DisregardShift)
			{
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollUp | (int)Keys.Shift, "Shift+ScrollUp"));
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollDown | (int)Keys.Shift, "Shift+ScrollDown"));
			}
			if(a.AllowMouse && !a.DisregardControl)
			{
				actioncontrol.Items.Add(new KeyControl(Keys.LButton | Keys.Control, "Ctrl+LButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.MButton | Keys.Control, "Ctrl+MButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.RButton | Keys.Control, "Ctrl+RButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton1 | Keys.Control, "Ctrl+XButton1"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton2 | Keys.Control, "Ctrl+XButton2"));
			}
			if(a.AllowScroll && !a.DisregardControl)
			{
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollUp | (int)Keys.Control, "Ctrl+ScrollUp"));
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollDown | (int)Keys.Control, "Ctrl+ScrollDown"));
			}
			if(a.AllowMouse && !a.DisregardShift && !a.DisregardControl)
			{
				actioncontrol.Items.Add(new KeyControl(Keys.LButton | Keys.Shift | Keys.Control, "Ctrl+Shift+LButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.MButton | Keys.Shift | Keys.Control, "Ctrl+Shift+MButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.RButton | Keys.Shift | Keys.Control, "Ctrl+Shift+RButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton1 | Keys.Shift | Keys.Control, "Ctrl+Shift+XButton1"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton2 | Keys.Shift | Keys.Control, "Ctrl+Shift+XButton2"));
			}
			if(a.AllowScroll && !a.DisregardShift && !a.DisregardControl)
			{
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollUp | (int)Keys.Shift | (int)Keys.Control, "Ctrl+Shift+ScrollUp"));
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollDown | (int)Keys.Shift | (int)Keys.Control, "Ctrl+Shift+ScrollDown"));
			}
		}
		
		// Item selected
		private void listactions_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			Actions.Action action;
			KeyControl keycontrol;
			string disregardkeys = "";
			int key;

			// Anything selected?
			if(listactions.SelectedItems.Count > 0)
			{
				// Begin updating
				allowapplycontrol = false;

				// Get the selected action
				action = General.Actions[listactions.SelectedItems[0].Name];
				key = (int)listactions.SelectedItems[0].SubItems[1].Tag;
				disregardshift = action.DisregardShift;
				disregardcontrol = action.DisregardControl;
				
				// Enable panel
				actioncontrolpanel.Enabled = true;
				actiontitle.Text = action.Title;
				actiondescription.Text = action.Description;
				actioncontrol.SelectedIndex = -1;
				actionkey.Text = "";
				
				if(disregardshift && disregardcontrol)
					disregardkeys = "Shift and Control";
				else if(disregardshift)
					disregardkeys = "Shift";
				else if(disregardcontrol)
					disregardkeys = "Control";

				disregardshiftlabel.Text = disregardshiftlabel.Tag.ToString().Replace("%s", disregardkeys);
				disregardshiftlabel.Visible = disregardshift | disregardcontrol;
				
				// Fill special controls list
				FillControlsList(action);
				
				// See if the key is in the combobox
				for(int i = 0; i < actioncontrol.Items.Count; i++)
				{
					// Select it when the key is found here
					keycontrol = (KeyControl)actioncontrol.Items[i];
					if(keycontrol.key == key) actioncontrol.SelectedIndex = i;
				}

				// Otherwise display the key in the textbox
				if(actioncontrol.SelectedIndex == -1)
					actionkey.Text = Actions.Action.GetShortcutKeyDesc(key);
				
				// Show actions with same key
				UpdateKeyUsedActions();
				
				// Focus to the input box
				actionkey.Focus();

				// Done
				allowapplycontrol = true;
			}
		}

		// Key released
		private void listactions_KeyUp(object sender, KeyEventArgs e)
		{
			// Nothing selected?
			if(listactions.SelectedItems.Count == 0)
			{
				// Disable panel
				actioncontrolpanel.Enabled = false;
				actiontitle.Text = "(select an action from the list)";
				actiondescription.Text = "";
				actionkey.Text = "";
				actioncontrol.SelectedIndex = -1;
				disregardshiftlabel.Visible = false;
			}
			
			// Show actions with same key
			UpdateKeyUsedActions();
		}

		// Mouse released
		private void listactions_MouseUp(object sender, MouseEventArgs e)
		{
			listactions_KeyUp(sender, new KeyEventArgs(Keys.None));

			// Focus to the input box
			actionkey.Focus();
		}

		// Key combination pressed
		private void actionkey_KeyDown(object sender, KeyEventArgs e)
		{
			int key = (int)e.KeyData;
			e.SuppressKeyPress = true;

			// Leave when not allowed to update
			if(!allowapplycontrol) return;

			// Anything selected?
			if(listactions.SelectedItems.Count > 0)
			{
				// Begin updating
				allowapplycontrol = false;
				
				// Remove modifier keys from the key if needed
				if(disregardshift) key &= ~(int)Keys.Shift;
				if(disregardcontrol) key &= ~(int)Keys.Control;
				
				// Deselect anything from the combobox
				actioncontrol.SelectedIndex = -1;
				
				// Apply the key combination
				listactions.SelectedItems[0].SubItems[1].Text = Actions.Action.GetShortcutKeyDesc(key);
				listactions.SelectedItems[0].SubItems[1].Tag = key;
				actionkey.Text = Actions.Action.GetShortcutKeyDesc(key);
				
				// Show actions with same key
				UpdateKeyUsedActions();
				
				// Done
				allowapplycontrol = true;
			}
		}

		// Key combination displayed
		private void actionkey_TextChanged(object sender, EventArgs e)
		{
			// Cursor to the end
			actionkey.SelectionStart = actionkey.Text.Length;
			actionkey.SelectionLength = 0;
		}

		// Special key selected
		private void actioncontrol_SelectedIndexChanged(object sender, EventArgs e)
		{
			KeyControl key;

			// Leave when not allowed to update
			if(!allowapplycontrol) return;

			// Anything selected?
			if((actioncontrol.SelectedIndex > -1) && (listactions.SelectedItems.Count > 0))
			{
				// Begin updating
				allowapplycontrol = false;

				// Remove text from textbox
				actionkey.Text = "";

				// Get the key control
				key = (KeyControl)actioncontrol.SelectedItem;

				// Apply the key combination
				listactions.SelectedItems[0].SubItems[1].Text = Actions.Action.GetShortcutKeyDesc(key.key);
				listactions.SelectedItems[0].SubItems[1].Tag = key.key;
				
				// Show actions with same key
				UpdateKeyUsedActions();
				
				// Focus to the input box
				actionkey.Focus();

				// Done
				allowapplycontrol = true;
			}
		}

		// Clear clicked
		private void actioncontrolclear_Click(object sender, EventArgs e)
		{
			// Begin updating
			allowapplycontrol = false;

			// Clear textbox and combobox
			actionkey.Text = "";
			actioncontrol.SelectedIndex = -1;

			// Apply the key combination
			listactions.SelectedItems[0].SubItems[1].Text = "";
			listactions.SelectedItems[0].SubItems[1].Tag = (int)0;
			
			// Show actions with same key
			UpdateKeyUsedActions();
			
			// Focus to the input box
			actionkey.Focus();

			// Done
			allowapplycontrol = true;
		}

		#endregion

		#region ================== Colors Panel

		private void imagebrightness_ValueChanged(object sender, EventArgs e)
		{
			imagebrightnesslabel.Text = "+ " + imagebrightness.Value.ToString() + " y";
		}

		private void doublesidedalpha_ValueChanged(object sender, EventArgs e)
		{
			int percent = doublesidedalpha.Value * 10;
			doublesidedalphalabel.Text = percent.ToString() + "%";
		}

		#endregion

		// Help
		private void PreferencesForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			if(!actionkey.Focused)
			{
				General.ShowHelp("w_preferences.html");
				hlpevent.Handled = true;
			}
		}

        #region ================== Defaults Panel
        // ano
        private void randomizedefaultsBox_CheckedChanged(object sender, EventArgs e)
        {
            if (randomizedefaultsBox.Checked)
            {
                defaultTextureBox.Enabled = false;
                defaultFloorBox.Enabled = false;
                defaultCeilingBox.Enabled = false;
            }
            else
            {
                defaultTextureBox.Enabled = true;
                defaultFloorBox.Enabled = true;
                defaultCeilingBox.Enabled = true;
            }
        }
        #endregion
        /*
		// This writes all action help files using a template and some basic info from the actions.
		// Also writes actioncontents.txt with all files to be inserted into Contents.hhc.
		// Only used during development. Actual button to call this has been removed.
		private void gobutton_Click(object sender, EventArgs e)
		{
			string template = File.ReadAllText(Path.Combine(General.AppPath, "..\\Help\\a_template.html"));
			StringBuilder contents = new StringBuilder("\t<UL>\r\n");
			string filename;
			
			// Go for all actions
			Action[] actions = General.Actions.GetAllActions();
			foreach(Action a in actions)
			{
				StringBuilder actionhtml = new StringBuilder(template);
				actionhtml.Replace("ACTIONTITLE", a.Title);
				actionhtml.Replace("ACTIONDESCRIPTION", a.Description);
				actionhtml.Replace("ACTIONCATEGORY", General.Actions.Categories[a.Category]);
				filename = Path.Combine(General.AppPath, "..\\Help\\a_" + a.Name + ".html");
				File.WriteAllText(filename, actionhtml.ToString());
				
				contents.Append("\t\t<LI> <OBJECT type=\"text/sitemap\">\r\n");
				contents.Append("\t\t\t<param name=\"Name\" value=\"" + a.Title + "\">\r\n");
				contents.Append("\t\t\t<param name=\"Local\" value=\"a_" + a.Name + ".html\">\r\n");
				contents.Append("\t\t\t</OBJECT>\r\n");
			}
			
			contents.Append("\t</UL>\r\n");
			filename = Path.Combine(General.AppPath, "..\\Help\\actioncontents.txt");
			File.WriteAllText(filename, contents.ToString());
		}
		*/
    }
}