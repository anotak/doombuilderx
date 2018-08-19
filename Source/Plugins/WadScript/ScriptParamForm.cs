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
