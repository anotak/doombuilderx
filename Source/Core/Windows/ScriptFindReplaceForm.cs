
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
	public partial class ScriptFindReplaceForm : DelayedForm
	{
		#region ================== Variables

		private bool appclose;

		#endregion

		#region ================== Properties
		
		#endregion

		#region ================== Constructor

		// Constructor
		public ScriptFindReplaceForm()
		{
			InitializeComponent();
		}

		#endregion

		#region ================== Methods

		// This makes the Find & Replace options
		private FindReplaceOptions MakeOptions()
		{
			FindReplaceOptions options = new FindReplaceOptions();
			options.FindText = findtext.Text;
			options.CaseSensitive = casesensitive.Checked;
			options.WholeWord = wordonly.Checked;
			options.ReplaceWith = replacetext.Text;
			return options;
		}

		// Close the window
		new public void Close()
		{
			appclose = true;
			base.Close();
		}

		// This sets the text to find
		public void SetFindText(string text)
		{
			findtext.Text = text;
			findtext.SelectAll();
		}

		#endregion

		#region ================== Events

		// Form is closing
		private void ScriptFindReplaceForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(!appclose)
			{
				General.Map.ScriptEditor.Editor.CloseFindReplace(true);
			}
		}

		// Find Next
		private void findnextbutton_Click(object sender, EventArgs e)
		{
			General.Map.ScriptEditor.Editor.FindNext(MakeOptions());
		}
		
		// Replace
		private void replacebutton_Click(object sender, EventArgs e)
		{
			FindReplaceOptions options = MakeOptions();
			
			General.Map.ScriptEditor.Editor.Replace(options);
			General.Map.ScriptEditor.Editor.FindNext(options);
		}
		
		// Replace All
		private void replaceallbutton_Click(object sender, EventArgs e)
		{
			General.Map.ScriptEditor.Editor.ReplaceAll(MakeOptions());
		}
		
		// Close
		private void closebutton_Click(object sender, EventArgs e)
		{
			General.Map.ScriptEditor.Editor.CloseFindReplace(false);
		}

		#endregion
	}
}