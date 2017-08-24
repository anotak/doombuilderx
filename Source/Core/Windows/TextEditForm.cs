
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
	public partial class TextEditForm : DelayedForm
	{
		// Properties
		public string Value { get { return textbox.Text; } set { textbox.Text = value; } }

		// Constructor
		public TextEditForm()
		{
			// Initialize
			InitializeComponent();
		}

		// This shows the dialog, returns the same value when cancelled
		public static string ShowDialog(IWin32Window owner, string value)
		{
			TextEditForm f = new TextEditForm();
			f.Value = value;
			if(f.ShowDialog(owner) == DialogResult.OK) value = f.Value;
			f.Dispose();
			return value;
		}

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Done
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clciked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Be gone
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Window activated
		private void TextEditForm_Activated(object sender, EventArgs e)
		{
			// Focus to textbox
			textbox.Focus();
		}
	}
}