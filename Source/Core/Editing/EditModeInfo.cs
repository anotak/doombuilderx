
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
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Plugins;
using System.Drawing;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	internal class EditModeInfo : IComparable<EditModeInfo>
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Mode type
		private Plugin plugin;
		private Type type;
		private EditModeAttribute attribs;
		
		// Mode switching
		private BeginActionAttribute switchactionattr = null;
		private ActionDelegate switchactiondel = null;

		// Mode button
		private Stream buttonimagestream = null;
		private Image buttonimage = null;
		private string buttondesc = null;
		private int buttonorder = int.MaxValue;
		
		#endregion

		#region ================== Properties

		public Plugin Plugin { get { return plugin; } }
		public Type Type { get { return type; } }
		public bool IsOptional { get { return ((switchactionattr != null) || (buttonimage != null)) && attribs.Optional; } }
		public BeginActionAttribute SwitchAction { get { return switchactionattr; } }
		public Image ButtonImage { get { return buttonimage; } }
		public string ButtonDesc { get { return buttondesc; } }
		public EditModeAttribute Attributes { get { return attribs; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public EditModeInfo(Plugin plugin, Type type, EditModeAttribute attr)
		{
			// Initialize
			this.plugin = plugin;
			this.type = type;
			this.attribs = attr;
			
			// Make switch action info
			if((attribs.SwitchAction != null) && (attribs.SwitchAction.Length > 0))
				switchactionattr = new BeginActionAttribute(attribs.SwitchAction);
			
			// Make button info
			if(attr.ButtonImage != null)
			{
				buttonimagestream = plugin.GetResourceStream(attr.ButtonImage);
				if(buttonimagestream != null)
				{
					buttonimage = Image.FromStream(buttonimagestream);
					buttondesc = attr.DisplayName;
					buttonorder = attr.ButtonOrder;
				}
			}
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public void Dispose()
		{
			// Dispose
			UnbindSwitchAction();
			buttonimage.Dispose();
			buttonimagestream.Dispose();

			// Clean up
			plugin = null;
		}
		
		#endregion

		#region ================== Methods
		
		// This binds the action to switch to this editing mode
		public void BindSwitchAction()
		{
			if((switchactiondel == null) && (switchactionattr != null))
			{
				switchactiondel = new ActionDelegate(UserSwitchToMode);
				General.Actions.BindBeginDelegate(plugin.Assembly, switchactiondel, switchactionattr);
			}
		}
		
		// This unbind the switch action
		public void UnbindSwitchAction()
		{
			if(switchactiondel != null)
			{
				General.Actions.UnbindBeginDelegate(plugin.Assembly, switchactiondel, switchactionattr);
				switchactiondel = null;
			}
		}
		
		// This switches to the mode by user command
		// (when user presses shortcut key)
		public void UserSwitchToMode()
		{
			EditMode newmode;
			
			// Only when a map is opened
			if(General.Map != null)
			{
				// Switching from volatile mode to volatile mode?
				if((General.Editing.Mode != null) && General.Editing.Mode.Attributes.Volatile && this.attribs.Volatile)
				{
					// First cancel previous volatile mode
					General.Editing.CancelVolatileMode();
				}
				
				// When in VisualMode and switching to the same VisualMode, then we switch back to the previous classic mode
				if((General.Editing.Mode is VisualMode) && (type == General.Editing.Mode.GetType()))
				{
					// Switch back to last classic mode
					General.Editing.ChangeMode(General.Editing.PreviousClassicMode.Name);
				}
				else
				{
					// Create instance
					newmode = plugin.CreateObject<EditMode>(type);
					
					// Switch mode
					General.Editing.ChangeMode(newmode);
				}
			}
		}
		
		// This switches to the mode
		public void SwitchToMode()
		{
			EditMode newmode;
			
			// Only when a map is opened
			if(General.Map != null)
			{
				// Create instance
				newmode = plugin.CreateObject<EditMode>(type);

				// Switch mode
				General.Editing.ChangeMode(newmode);
			}
		}

		// This switches to the mode with arguments
		public void SwitchToMode(object[] args)
		{
			EditMode newmode;

			// Only when a map is opened
			if(General.Map != null)
			{
				// Create instance
				newmode = plugin.CreateObjectA<EditMode>(type, args);

				// Switch mode
				if(!General.Editing.ChangeMode(newmode))
				{
					// When cancelled, dispose mode
					newmode.Dispose();
				}
			}
		}
		
		// String representation
		public override string ToString()
		{
			return attribs.DisplayName;
		}

		// Compare by button order
		public int CompareTo(EditModeInfo other)
		{
			if(this.buttonorder > other.buttonorder) return 1;
			else if(this.buttonorder < other.buttonorder) return -1;
			else return 0;
		}
		
		#endregion
	}
}
