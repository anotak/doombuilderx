
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
	internal partial class ScriptEditorPanel : UserControl
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables
		
		private List<ScriptConfiguration> scriptconfigs;
		private List<CompilerError> compilererrors;

		// Find/Replace
		private ScriptFindReplaceForm findreplaceform;
		private FindReplaceOptions findoptions;
		
		#endregion
		
		#region ================== Properties
		
		public ScriptDocumentTab ActiveTab { get { return (tabs.SelectedTab as ScriptDocumentTab); } }
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public ScriptEditorPanel()
		{
			InitializeComponent();
		}
		
		// This initializes the control
		public void Initialize()
		{
			ToolStripMenuItem item;
			
			// Make list of script configs
			scriptconfigs = new List<ScriptConfiguration>(General.ScriptConfigs.Values);
			scriptconfigs.Add(new ScriptConfiguration());
			scriptconfigs.Sort();
			
			// Fill the list of new document types
			foreach(ScriptConfiguration cfg in scriptconfigs)
			{
				// Button for new script menu
				item = new ToolStripMenuItem(cfg.Description);
				//item.Image = buttonnew.Image;
				item.Tag = cfg;
				item.Click += new EventHandler(buttonnew_Click);
				buttonnew.DropDownItems.Add(item);
				
				// Button for script type menu
				item = new ToolStripMenuItem(cfg.Description);
				//item.Image = buttonnew.Image;
				item.Tag = cfg;
				item.Click += new EventHandler(buttonscriptconfig_Click);
				buttonscriptconfig.DropDownItems.Add(item);
			}
			
			// Setup supported extensions
			string filterall = "";
			string filterseperate = "";
			foreach(ScriptConfiguration cfg in scriptconfigs)
			{
				if(cfg.Extensions.Length > 0)
				{
					string exts = "*." + string.Join(";*.", cfg.Extensions);
					if(filterseperate.Length > 0) filterseperate += "|";
					filterseperate += cfg.Description + "|" + exts;
					if(filterall.Length > 0) filterall += ";";
					filterall += exts;
				}
			}
			openfile.Filter = "Script files|" + filterall + "|" + filterseperate + "|All files|*.*";

            int selectedIndex = 0;
			// Load the script lumps
			foreach(MapLumpInfo maplumpinfo in General.Map.Config.MapLumps.Values)
			{
				// Is this a script lump?
				if(maplumpinfo.script != null)
				{
					// Load this!
					ScriptLumpDocumentTab t = new ScriptLumpDocumentTab(this, maplumpinfo.name, maplumpinfo.script);
					tabs.TabPages.Add(t);
                    if (maplumpinfo.name.StartsWith("SCRIPTS"))
                    {
                        selectedIndex = tabs.TabCount - 1;
                    }
				}
			}

			// Load the files that were previously opened for this map
			foreach(String filename in General.Map.Options.ScriptFiles)
			{
				// Does this file exist?
				if(File.Exists(filename))
				{
					// Load this!
					OpenFile(filename);
				}
			}

			// Select the first tab
			if(tabs.TabPages.Count > 0) tabs.SelectedIndex = selectedIndex;
			
			// If the map has remembered any compile errors, then show them
			ShowErrors(General.Map.Errors);
			
			// Done
			UpdateToolbar(true);
		}
		
		// This applies user preferences
		public void ApplySettings()
		{
			// Apply settings
			//int panel2size = General.Settings.ReadSetting("scriptspanel.splitter", splitter.ClientRectangle.Height - splitter.SplitterDistance);
			//splitter.SplitterDistance = splitter.ClientRectangle.Height - panel2size;
			errorlist.Columns[0].Width = General.Settings.ReadSetting("scriptspanel.errorscolumn0width", errorlist.Columns[0].Width);
			errorlist.Columns[1].Width = General.Settings.ReadSetting("scriptspanel.errorscolumn1width", errorlist.Columns[1].Width);
			errorlist.Columns[2].Width = General.Settings.ReadSetting("scriptspanel.errorscolumn2width", errorlist.Columns[2].Width);
		}
		
		// This saves user preferences
		public void SaveSettings()
		{
			//General.Settings.WriteSetting("scriptspanel.splitter", splitter.ClientRectangle.Height - splitter.SplitterDistance);
			General.Settings.WriteSetting("scriptspanel.errorscolumn0width", errorlist.Columns[0].Width);
			General.Settings.WriteSetting("scriptspanel.errorscolumn1width", errorlist.Columns[1].Width);
			General.Settings.WriteSetting("scriptspanel.errorscolumn2width", errorlist.Columns[2].Width);
		}
		
		#endregion
		
		#region ================== Methods

		// Find Next
		public void FindNext(FindReplaceOptions options)
		{
			// Save the options
			findoptions = options;
			FindNext();
		}

		// Find Next with saved options
		public void FindNext()
		{
			if(!string.IsNullOrEmpty(findoptions.FindText) && (ActiveTab != null))
			{
				if(!ActiveTab.FindNext(findoptions))
				{
					General.MainWindow.DisplayStatus(StatusType.Warning, "Can't find any occurence of \"" + findoptions.FindText + "\".");
				}
			}
			else
			{
				General.MessageBeep(MessageBeepType.Default);
			}
		}
		
		// Replace if possible
		public void Replace(FindReplaceOptions options)
		{
			if(!string.IsNullOrEmpty(findoptions.FindText) && (options.ReplaceWith != null) && (ActiveTab != null))
			{
				if(string.Compare(ActiveTab.GetSelectedText(), options.FindText, !options.CaseSensitive) == 0)
				{
					// Replace selection
					ActiveTab.ReplaceSelection(options.ReplaceWith);
				}
			}
			else
			{
				General.MessageBeep(MessageBeepType.Default);
			}
		}
		
		// Replace all
		public void ReplaceAll(FindReplaceOptions options)
		{
			int replacements = 0;
			findoptions = options;
			if(!string.IsNullOrEmpty(findoptions.FindText) && (options.ReplaceWith != null) && (ActiveTab != null))
			{
				int firstfindpos = -1;
				int lastpos = -1;
				bool firstreplace = true;
				bool wrappedaround = false;
				int selectionstart = Math.Min(ActiveTab.SelectionStart, ActiveTab.SelectionEnd);
				
				// Continue finding and replacing until nothing more found
				while(ActiveTab.FindNext(findoptions))
				{
					int curpos = Math.Min(ActiveTab.SelectionStart, ActiveTab.SelectionEnd);
					if(curpos <= lastpos)
						wrappedaround = true;
					
					if(firstreplace)
					{
						// Remember where we started replacing
						firstfindpos = curpos;
					}
					else if(wrappedaround)
					{
						// Make sure we don't go past our start point, or we could be in an endless loop
						if(curpos >= firstfindpos)
							break;
					}
					
					Replace(findoptions);
					replacements++;
					firstreplace = false;

					lastpos = curpos;
				}

				// Restore selection
				ActiveTab.SelectionStart = selectionstart;
				ActiveTab.SelectionEnd = selectionstart;
				
				// Show result
				if(replacements == 0)
					General.MainWindow.DisplayStatus(StatusType.Warning, "Can't find any occurence of \"" + findoptions.FindText + "\".");
				else
					General.MainWindow.DisplayStatus(StatusType.Info, "Replaced " + replacements + " occurences of \"" + findoptions.FindText + "\" with \"" + findoptions.ReplaceWith + "\".");
			}
			else
			{
				General.MessageBeep(MessageBeepType.Default);
			}
		}
		
		// This closed the Find & Replace subwindow
		public void CloseFindReplace(bool closing)
		{
			if(findreplaceform != null)
			{
				if(!closing) findreplaceform.Close();
				findreplaceform = null;
			}
		}

		// This opens the Find & Replace subwindow
		public void OpenFindAndReplace()
		{
			if(findreplaceform == null)
				findreplaceform = new ScriptFindReplaceForm();

			try
			{
				if(findreplaceform.Visible)
					findreplaceform.Focus();
				else
					findreplaceform.Show(this.ParentForm);

				if(ActiveTab.SelectionEnd != ActiveTab.SelectionStart)
					findreplaceform.SetFindText(ActiveTab.GetSelectedText());
			}
			catch(Exception)
			{
				// If we can't pop up the find/replace form right now, thats just too bad.
			}
		}

		// This refreshes all settings
		public void RefreshSettings()
		{
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				t.RefreshSettings();
			}
		}

		// This clears all error marks and hides the errors list
		public void ClearErrors()
		{
			// Hide list
			splitter.Panel2Collapsed = true;
			errorlist.Items.Clear();

			// Clear marks
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				t.ClearMarks();
			}
		}
		
		// This shows the errors panel with the given errors
		// Also updates the scripts with markers for the given errors
		public void ShowErrors(IEnumerable<CompilerError> errors)
		{
			// Copy list
			if(errors != null)
				compilererrors = new List<CompilerError>(errors);
			else
				compilererrors = new List<CompilerError>();
			
			// Fill list
			errorlist.BeginUpdate();
			errorlist.Items.Clear();
			int listindex = 1;
			foreach(CompilerError e in compilererrors)
			{
				ListViewItem ei = new ListViewItem(listindex.ToString());
				ei.ImageIndex = 0;
				ei.SubItems.Add(e.description);
				if(e.filename.StartsWith("?"))
					ei.SubItems.Add(e.filename.Replace("?", "") + " (line " + e.linenumber.ToString() + ")");
				else
					ei.SubItems.Add(Path.GetFileName(e.filename) + " (line " + e.linenumber.ToString() + ")");
				ei.Tag = e;
				errorlist.Items.Add(ei);
				listindex++;
			}
			errorlist.EndUpdate();
			
			// Show marks on scripts
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				t.MarkScriptErrors(compilererrors);
			}
			
			// Show/hide panel
			splitter.Panel2Collapsed = (errorlist.Items.Count == 0);
		}
		
		// This writes all explicitly opened files to the configuration
		public void WriteOpenFilesToConfiguration()
		{
			List<string> files = new List<string>();
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				if(t.ExplicitSave) files.Add(t.Filename);
			}
			General.Map.Options.ScriptFiles = files;
		}
		
		// This asks to save files and returns the result
		public bool AskSaveAll()
		{
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				if(t.ExplicitSave)
				{
					if(!CloseScript(t, true)) return false;
				}
			}
			
			return true;
		}
		
		// This closes a script and returns true when closed
		private bool CloseScript(ScriptDocumentTab t, bool saveonly)
		{
			if(t.IsChanged)
			{
				// Ask to save
				DialogResult result = MessageBox.Show(this.ParentForm, "Do you want to save changes to " + t.Text + "?", "Close File", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if(result == DialogResult.Yes)
				{
					// Save file
					if(!SaveScript(t)) return false;
				}
				else if(result == DialogResult.Cancel)
				{
					// Cancel
					return false;
				}
			}
			
			if(!saveonly)
			{
				// Close file
				tabs.TabPages.Remove(t);
				t.Dispose();
			}
			return true;
		}
		
		// This returns true when any of the implicit-save scripts are changed
		public bool CheckImplicitChanges()
		{
			bool changes = false;
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				if(!t.ExplicitSave && t.IsChanged) changes = true;
			}
			return changes;
		}
		
		// This forces the focus to the script editor
		public void ForceFocus()
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			tabs.Focus();
			if(t != null) t.Focus();
		}
		
		// This does an implicit save on all documents that use implicit saving
		// Call this to save the lumps before disposing the panel!
		public void ImplicitSave()
		{
			// Save all scripts
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				if(!t.ExplicitSave) t.Save();
			}
			
			UpdateToolbar(false);
		}
		
		// This updates the toolbar for the current status
		private void UpdateToolbar(bool focuseditor)
		{
			int numscriptsopen = tabs.TabPages.Count;
			int explicitsavescripts = 0;
			ScriptDocumentTab t = null;
			
			// Any explicit save scripts?
			foreach(ScriptDocumentTab dt in tabs.TabPages)
				if(dt.ExplicitSave) explicitsavescripts++;
			
			// Get current script, if any are open
			if(numscriptsopen > 0)
				t = (tabs.SelectedTab as ScriptDocumentTab);
			
			// Enable/disable buttons
			buttonsave.Enabled = (t != null) && t.ExplicitSave;
			buttonsaveall.Enabled = (explicitsavescripts > 0);
			buttoncompile.Enabled = (t != null) && (t.Config.Compiler != null);
			buttonkeywordhelp.Enabled = (t != null) && !string.IsNullOrEmpty(t.Config.KeywordHelp);
			buttonscriptconfig.Enabled = (t != null) && t.IsReconfigurable;
			buttonundo.Enabled = (t != null);
			buttonredo.Enabled = (t != null);
			buttoncopy.Enabled = (t != null);
			buttoncut.Enabled = (t != null);
			buttonpaste.Enabled = (t != null);
			buttonclose.Enabled = (t != null) && t.IsClosable;
			
			if(t != null)
			{
				// Check the according script config in menu
				foreach(ToolStripMenuItem item in buttonscriptconfig.DropDownItems)
				{
					ScriptConfiguration config = (item.Tag as ScriptConfiguration);
					item.Checked = (config == t.Config);
				}
				
				// Focus to script editor
				if(focuseditor) ForceFocus();
			}
		}

		// This opens the given file, returns null when failed
		public ScriptFileDocumentTab OpenFile(string filename)
		{
			ScriptConfiguration foundconfig = new ScriptConfiguration();

			// Find the most suitable script configuration to use
			foreach(ScriptConfiguration cfg in scriptconfigs)
			{
				foreach(string ext in cfg.Extensions)
				{
					// Use this configuration if the extension matches
					if(filename.EndsWith("." + ext, true, CultureInfo.InvariantCulture))
					{
						foundconfig = cfg;
						break;
					}
				}
			}

			// Create new document
			ScriptFileDocumentTab t = new ScriptFileDocumentTab(this, foundconfig);
			if(t.Open(filename))
			{
				// Mark any errors this script may have
				if(compilererrors != null)
					t.MarkScriptErrors(compilererrors);

				// Add to tabs
				tabs.TabPages.Add(t);
				tabs.SelectedTab = t;

				// Done
				UpdateToolbar(true);
				return t;
			}
			else
			{
				// Failed
				return null;
			}
		}

		// This saves the current open script
		public void ExplicitSaveCurrentTab()
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			if((t != null) && t.ExplicitSave)
			{
				buttonsave_Click(this, EventArgs.Empty);
			}
			else
			{
				General.MessageBeep(MessageBeepType.Default);
			}
		}
		
		// This opens a script
		public void OpenBrowseScript()
		{
			buttonopen_Click(this, EventArgs.Empty);
		}
		
		#endregion
		
		#region ================== Events

		// Called when the window that contains this panel closes
		public void OnClose()
		{
			// Close the sub windows now
			if(findreplaceform != null) findreplaceform.Dispose();
		}

		// Keyword help requested
		private void buttonkeywordhelp_Click(object sender, EventArgs e)
		{
			// Get script
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.LaunchKeywordHelp();
		}

		// When the user changes the script configuration
		private void buttonscriptconfig_Click(object sender, EventArgs e)
		{
			// Get the tab and new script config
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			ScriptConfiguration scriptconfig = ((sender as ToolStripMenuItem).Tag as ScriptConfiguration);
			
			// Change script config
			t.ChangeScriptConfig(scriptconfig);

			// Done
			UpdateToolbar(true);
		}
		
		// When new script is clicked
		private void buttonnew_Click(object sender, EventArgs e)
		{
			// Get the script config to use
			ScriptConfiguration scriptconfig = ((sender as ToolStripMenuItem).Tag as ScriptConfiguration);
			
			// Create new document
			ScriptFileDocumentTab t = new ScriptFileDocumentTab(this, scriptconfig);
			tabs.TabPages.Add(t);
			tabs.SelectedTab = t;
			
			// Done
			UpdateToolbar(true);
		}
		
		// Open script clicked
		private void buttonopen_Click(object sender, EventArgs e)
		{
			// Show open file dialog
			if(openfile.ShowDialog(this.ParentForm) == DialogResult.OK)
			{
				// TODO: Make multi-select possible
				OpenFile(openfile.FileName);
			}
		}

		// Save script clicked
		private void buttonsave_Click(object sender, EventArgs e)
		{
			// Save the current script
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			SaveScript(t);
			UpdateToolbar(true);
		}

		// Save All clicked
		private void buttonsaveall_Click(object sender, EventArgs e)
		{
			// Save all scripts
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				// Use explicit save for this script?
				if(t.ExplicitSave)
				{
					if(!SaveScript(t)) break;
				}
			}
			
			UpdateToolbar(true);
		}

		// This is called by Save and Save All to save a script
		// Returns false when cancelled by the user
		private bool SaveScript(ScriptDocumentTab t)
		{
			// Do we have to do a save as?
			if(t.IsSaveAsRequired)
			{
				// Setup save dialog
				string scriptfilter = t.Config.Description + "|*." + string.Join(";*.", t.Config.Extensions);
				savefile.Filter = scriptfilter + "|All files|*.*";
				if(savefile.ShowDialog(this.ParentForm) == DialogResult.OK)
				{
					// Save to new filename
					t.SaveAs(savefile.FileName);
					return true;
				}
				else
				{
					// Cancelled
					return false;
				}
			}
			else
			{
				// Save to same filename
				t.Save();
				return true;
			}
		}
		
		// A tab is selected
		private void tabs_Selecting(object sender, TabControlCancelEventArgs e)
		{
			UpdateToolbar(true);
		}
		
		// This closes the current file
		private void buttonclose_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			CloseScript(t, false);
			UpdateToolbar(true);
		}
		
		// Compile Script clicked
		private void buttoncompile_Click(object sender, EventArgs e)
		{
			// First save all implicit scripts to the temporary wad file
			ImplicitSave();
			
			// Get script
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);

			// Check if it must be saved as a new file
			if(t.ExplicitSave && t.IsSaveAsRequired)
			{
				// Save the script first!
				if(MessageBox.Show(this.ParentForm, "You must save your script before you can compile it. Do you want to save your script now?", "Compile Script", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					if(!SaveScript(t)) return;
				}
				else
				{
					return;
				}
			}
			else
			{
				if(t.ExplicitSave && t.IsChanged)
				{
					// We can only compile when the script is saved
					if(!SaveScript(t)) return;
				}
			}

			// Compile now
			General.MainWindow.DisplayStatus(StatusType.Busy, "Compiling script " + t.Text + "...");
			Cursor.Current = Cursors.WaitCursor;
			t.Compile();

			// Show warning
			if((compilererrors != null) && (compilererrors.Count > 0))
				General.MainWindow.DisplayStatus(StatusType.Warning, compilererrors.Count.ToString() + " errors while compiling " + t.Text + "!");
			else
				General.MainWindow.DisplayStatus(StatusType.Info, "Script " + t.Text + " compiled without errors.");

			Cursor.Current = Cursors.Default;
			UpdateToolbar(true);
		}
		
		// Undo clicked
		private void buttonundo_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Undo();
			UpdateToolbar(true);
		}
		
		// Redo clicked
		private void buttonredo_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Redo();
			UpdateToolbar(true);
		}
		
		// Cut clicked
		private void buttoncut_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Cut();
			UpdateToolbar(true);
		}
		
		// Copy clicked
		private void buttoncopy_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Copy();
			UpdateToolbar(true);
		}

		// Paste clicked
		private void buttonpaste_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Paste();
			UpdateToolbar(true);
		}
		
		// Mouse released on tabs
		private void tabs_MouseUp(object sender, MouseEventArgs e)
		{
			ForceFocus();
		}
		
		// User double-clicks and error in the list
		private void errorlist_ItemActivate(object sender, EventArgs e)
		{
			// Anything selection?
			if(errorlist.SelectedItems.Count > 0)
			{
				// Get the compiler error
				CompilerError err = (CompilerError)errorlist.SelectedItems[0].Tag;
				
				// Show the tab with the script that matches
				bool foundscript = false;
				foreach(ScriptDocumentTab t in tabs.TabPages)
				{
					if(t.VerifyErrorForScript(err))
					{
						tabs.SelectedTab = t;
						t.MoveToLine(err.linenumber);
						foundscript = true;
						break;
					}
				}

				// If we don't have the script opened, see if we can find the file and open the script
				if(!foundscript && File.Exists(err.filename))
				{
					ScriptDocumentTab t = OpenFile(err.filename);
					if(t != null) t.MoveToLine(err.linenumber);
				}
				
				ForceFocus();
			}
		}
		
		#endregion
	}
}
