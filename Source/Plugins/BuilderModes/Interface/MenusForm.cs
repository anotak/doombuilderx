
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class MenusForm : Form
	{
		#region ================== Variables

		// Menus list
		private ToolStripItem[] menus;

		// Buttons list
		private ToolStripItem[] buttons;

		#endregion

		#region ================== Properties

		public ToolStripMenuItem LinedefsMenu { get { return linedefsmenu; } }
		public ToolStripMenuItem SectorsMenu { get { return sectorsmenu; } }
		public ToolStripButton ViewSelectionNumbers { get { return buttonselectionnumbers; } }
		public ToolStripSeparator SeparatorSectors1 { get { return separatorsectors1; } }
		public ToolStripButton MakeGradientBrightness { get { return buttonbrightnessgradient; } }
		public ToolStripButton MakeGradientFloors { get { return buttonfloorgradient; } }
		public ToolStripButton MakeGradientCeilings { get { return buttonceilinggradient; } }
		public ToolStripButton FlipSelectionV { get { return buttonflipselectionv; } }
		public ToolStripButton FlipSelectionH { get { return buttonflipselectionh; } }
		public ToolStripButton CurveLinedefs { get { return buttoncurvelinedefs; } }
		public ToolStripButton CopyProperties { get { return buttoncopyproperties; } }
		public ToolStripButton PasteProperties { get { return buttonpasteproperties; } }
		public ToolStripSeparator SeparatorCopyPaste { get { return seperatorcopypaste; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public MenusForm()
		{
			// Initialize
			InitializeComponent();

			// Apply settings
			buttonselectionnumbers.Checked = BuilderPlug.Me.ViewSelectionNumbers;
			
			// List all menus
			menus = new ToolStripItem[menustrip.Items.Count];
			for(int i = 0; i < menustrip.Items.Count; i++) menus[i] = menustrip.Items[i];

			// List all buttons
			buttons = new ToolStripItem[globalstrip.Items.Count];
			for(int i = 0; i < globalstrip.Items.Count; i++) buttons[i] = globalstrip.Items[i];
		}
		
		#endregion

		#region ================== Methods

		// This registers with the core
		public void Register()
		{
			// Add the menus to the core
			foreach(ToolStripMenuItem m in menus)
				General.Interface.AddMenu(m);
			
			// Add the buttons to the core
			foreach(ToolStripItem b in buttons)
				General.Interface.AddButton(b);
		}

		// This unregisters from the core
		public void Unregister()
		{
			// Remove the menus from the core
			foreach(ToolStripMenuItem m in menus)
				General.Interface.RemoveMenu(m);

			// Remove the buttons from the core
			foreach(ToolStripItem b in buttons)
				General.Interface.RemoveButton(b);
		}

		// This hides all menus
		public void HideAllMenus()
		{
			foreach(ToolStripMenuItem m in menus) m.Visible = false;
		}
		
		// This hides all except one menu
		public void HideAllMenusExcept(ToolStripMenuItem showthis)
		{
			HideAllMenus();
			showthis.Visible = true;
		}
		
		// This shows the menu for the current editing mode
		public void ShowEditingModeMenu(EditMode mode)
		{
			Type sourcemode = typeof(object);
			if(mode != null)
			{
				sourcemode = mode.GetType();

				// When in a volatile mode, check against the last stable mode
				if(mode.Attributes.Volatile) sourcemode = General.Editing.PreviousStableMode;
			}
			
			// Final decision
			if(sourcemode == typeof(LinedefsMode)) HideAllMenusExcept(linedefsmenu);
			else if(sourcemode == typeof(SectorsMode)) HideAllMenusExcept(sectorsmenu);
			else HideAllMenus();
		}

		// This invokes an action from control event
		private void InvokeTaggedAction(object sender, EventArgs e)
		{
			General.Interface.InvokeTaggedAction(sender, e);
		}

		// View selection numbers clicked
		private void buttonselectionnumbers_Click(object sender, EventArgs e)
		{
			BuilderPlug.Me.ViewSelectionNumbers = buttonselectionnumbers.Checked;
			General.Interface.RedrawDisplay();
		}
		
		#endregion
	}
}