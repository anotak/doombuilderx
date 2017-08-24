
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
using System.IO;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class ScriptEditorForm : Form
	{
		#region ================== Variables

		// Position/size
		private Point lastposition;
		private Size lastsize;

		// Closing?
		private bool appclose;
		
		#endregion
		
		#region ================== Properties
		
		public ScriptEditorPanel Editor { get { return editor; } }
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public ScriptEditorForm()
		{
			InitializeComponent();
			editor.Initialize();
		}
		
		#endregion
		
		#region ================== Methods
		
		// This asks to save files and returns the result
		// Also does implicit saves
		// Returns false when cancelled by the user
		public bool AskSaveAll()
		{
			// Implicit-save the script lumps
			editor.ImplicitSave();
			
			// Save other scripts
			return editor.AskSaveAll();
		}
		
		// Close the window
		new public void Close()
		{
			appclose = true;
			base.Close();
		}

		#endregion
		
		#region ================== Events

		// Window is loaded
		private void ScriptEditorForm_Load(object sender, EventArgs e)
		{
			this.SuspendLayout();
			this.Location = new Point(General.Settings.ReadSetting("scriptswindow.positionx", this.Location.X),
									  General.Settings.ReadSetting("scriptswindow.positiony", this.Location.Y));
			this.Size = new Size(General.Settings.ReadSetting("scriptswindow.sizewidth", this.Size.Width),
								 General.Settings.ReadSetting("scriptswindow.sizeheight", this.Size.Height));
			this.WindowState = (FormWindowState)General.Settings.ReadSetting("scriptswindow.windowstate", (int)FormWindowState.Normal);
			this.ResumeLayout(true);

			// Normal windowstate?
			if(this.WindowState == FormWindowState.Normal)
			{
				// Keep last position and size
				lastposition = this.Location;
				lastsize = this.Size;
			}

			// Apply panel settings
			editor.ApplySettings();
		}
		
		// Window is shown
		private void ScriptEditorForm_Shown(object sender, EventArgs e)
		{
			// Focus to script editor
			editor.ForceFocus();
		}
		
		// Window is closing
		private void ScriptEditorForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			int windowstate;

			// Determine window state to save
			if(this.WindowState != FormWindowState.Minimized)
				windowstate = (int)this.WindowState;
			else
				windowstate = (int)FormWindowState.Normal;

			// Save window settings
			General.Settings.WriteSetting("scriptswindow.positionx", lastposition.X);
			General.Settings.WriteSetting("scriptswindow.positiony", lastposition.Y);
			General.Settings.WriteSetting("scriptswindow.sizewidth", lastsize.Width);
			General.Settings.WriteSetting("scriptswindow.sizeheight", lastsize.Height);
			General.Settings.WriteSetting("scriptswindow.windowstate", windowstate);
			editor.SaveSettings();
			
			// Only when closed by the user
			if(!appclose)
			{
				// Remember if scipts are changed
				General.Map.ApplyScriptChanged();
				
				// Ask to save scripts
				if(AskSaveAll())
				{
					// Let the general call close the editor
					General.Map.CloseScriptEditor(true);
				}
				else
				{
					// Cancel
					e.Cancel = true;
				}
			}

			// Not cancelling?
			if(!e.Cancel) editor.OnClose();
		}

		// Window resized
		private void ScriptEditorForm_ResizeEnd(object sender, EventArgs e)
		{
			// Normal windowstate?
			if(this.WindowState == FormWindowState.Normal)
			{
				// Keep last position and size
				lastposition = this.Location;
				lastsize = this.Size;
			}
		}

		// Window moved
		private void ScriptEditorForm_Move(object sender, EventArgs e)
		{
			// Normal windowstate?
			if(this.WindowState == FormWindowState.Normal)
			{
				// Keep last position and size
				lastposition = this.Location;
				lastsize = this.Size;
			}
		}

		// Help
		private void ScriptEditorForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_scripteditor.html");
			hlpevent.Handled = true;
		}
		
		#endregion
	}
}