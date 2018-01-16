using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;

namespace CodeImp.DoomBuilder.Windows
{
	public partial class FlagsForm : DelayedForm
	{
		#region ================== Variables

		//private bool setup;
		private string value;
		private IDictionary<string, string> flagdefs;

		#endregion

		#region ================== Properties

		public string Value { get { return value; } }

		#endregion

		#region ================== Methods

		public FlagsForm()
		{
			InitializeComponent();
		}

		// Setup from EnumList
		public void Setup(string value, IDictionary<string, string> inflags)
		{
			//setup = true;
			this.value = value;
			flagdefs = inflags;

			// Fill flags list
			foreach (KeyValuePair<string, string> tf in flagdefs)
				flags.Add(tf.Value, tf.Key);

			// Parse the value string and check the boxes if necessary
			if (value.Trim() != "")
			{
				foreach (string s in value.Split(','))
				{
					string str = s.Trim();

					// Make sure the given flag actually exists
					if(!flagdefs.ContainsKey(str))
						continue;

					foreach (CheckBox c in flags.Checkboxes)
					{
						if (c.Text == flagdefs[str])
							c.Checked = true;
					}
				}
			}

			//setup = false;
		}

		// This shows the dialog
		// Returns the flags or the same flags when cancelled
		public static string ShowDialog(IWin32Window owner, string value, IDictionary<string, string> inflags)
		{
			string result = value;
			FlagsForm f = new FlagsForm();
			f.Setup(value, inflags);
			if (f.ShowDialog(owner) == DialogResult.OK) result = f.Value;
			f.Dispose();
			return result;
		}

		#endregion

		private void LinedefFlagsForm_Load(object sender, EventArgs e)
		{

		}

		private void apply_Click(object sender, EventArgs e)
		{
			value = "";

			foreach (CheckBox c in flags.Checkboxes)
			{
				if(c.Checked == false) continue;

				foreach (KeyValuePair<string, string> lf in flagdefs)
				{
					if (lf.Value == c.Text)
					{
						if (value != "") value += ",";
						value += lf.Key.ToString();
					}
				}
			}

			DialogResult = DialogResult.OK;
			this.Close();
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			// Close
			DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}
