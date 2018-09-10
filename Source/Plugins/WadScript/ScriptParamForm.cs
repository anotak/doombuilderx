using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MoonSharp.Interpreter;

namespace CodeImp.DoomBuilder.DBXLua
{
    public partial class ScriptParamForm : Form
    {
        public ScriptParamForm()
        {
            InitializeComponent();
            Shown += ScriptParamForm_Shown;
            paramsview.KeyDown += ScriptParamForm_KeyDown;
        }

        public void ScriptParamForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                    e.Handled = true;
                }
            }
        }

        public void ScriptParamForm_Shown(object sender, EventArgs e)
        {
            // ano - focus kb properly
            if (paramsview.Rows.Count > 0)
            {
                //paramsview.Rows[0].Cells[0].Selected = false;

                //paramsview.Rows[0].Cells[1].Selected = true;
                paramsview.CurrentCell = paramsview.Rows[0].Cells[1];

                paramsview.Select();
            }
        }

        public static Dictionary<string,string> ShowParamsDialog(LuaUIParameters ui_parameters)
        {
            // ano - safer to assume data coming from a script is suspect
            if (ui_parameters.keys == null || ui_parameters.labels == null)
            {
                ScriptContext.context.Warn("Keys/labels were nil for taking script parameters. Possibly Lua API bug?");
                return null;
            }
            if (ui_parameters.keys.Count != ui_parameters.labels.Count)
            {
                ScriptContext.context.Warn("# keys must be equal to # labels for parameters dialog. Possibly Lua API bug?");
                return null;
            }

            // ano - really suspect
            if (ui_parameters.keys.Count > 384)
            {
                ScriptContext.context.Warn("# keys for script parameters can't be greater than 384");
                return null;
            }

            if (ui_parameters.defaultvalues == null)
            {
                ui_parameters.defaultvalues = new List<string>();
            }

            while (ui_parameters.defaultvalues.Count < ui_parameters.keys.Count)
            {
                ui_parameters.defaultvalues.Add(null);
            }

            ScriptParamForm form = new ScriptParamForm();
            for (int i = 0; i < ui_parameters.labels.Count; i++)
            {
                if (ui_parameters.defaultvalues[i] == null)
                {
                    form.paramsview.Rows.Add(ui_parameters.labels[i], "");
                }
                else
                {
                    form.paramsview.Rows.Add(ui_parameters.labels[i], ui_parameters.defaultvalues[i]);
                }

                if (ui_parameters.tooltips[i] != null && ui_parameters.tooltips[i].Length > 0)
                {
                    form.paramsview.Rows[i].Cells[0].ToolTipText = ui_parameters.tooltips[i];
                    form.paramsview.Rows[i].Cells[1].ToolTipText = ui_parameters.tooltips[i];
                }
            }

            DialogResult result = form.ShowDialog();

            if (result == DialogResult.OK)
            {
                Dictionary<string, string> output = new Dictionary<string, string>(ui_parameters.labels.Count);

                for (int i = 0; i < ui_parameters.labels.Count; i++)
                {
                    output.Add(ui_parameters.keys[i], form.paramsview.Rows[i].Cells[1].Value.ToString());
                }

                return output;
            }
            else
            {
                return null;
            }
        }

        private void okbutton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelbutton_Click(object sender, EventArgs e)
        {
            ScriptMode.CancelReason = eCancelReason.UserChoice;
            ScriptMode.bScriptCancelled = true;
            DialogResult = DialogResult.Cancel;
            Close();
        }
    } // class
} // ns
