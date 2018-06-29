using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.DBXLua
{
    public partial class ScriptTimeoutForm : Form
    {
        public Timer time;

        public ScriptTimeoutForm()
        {
            InitializeComponent();
            time = new Timer();
            time.Interval = 80;
            time.Tick += timer_event;
            Shown += shown_event;
        }

        public static DialogResult ShowTimeout()
        {
            ScriptTimeoutForm form = new ScriptTimeoutForm();
            
            if (ScriptMode.bScriptDone)
            {
                return DialogResult.OK;
            }
            form.time.Start();
            return form.ShowDialog();
        }

        private void shown_event(object sender, EventArgs e)
        {
            time.Start();
        }

        private void timer_event(object sender, EventArgs e)
        {
            if (ScriptMode.bScriptDone)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void timeoutButton_click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            if (!ScriptMode.bScriptDone)
            {
                ScriptContext.context.Kill();
                ScriptMode.CancelReason = eCancelReason.Timeout;
                ScriptMode.bScriptCancelled = true;
                DialogResult = DialogResult.Cancel;
            }
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (time != null)
            {
                time.Dispose();
            }
            base.OnClosed(e);
        }
    } // class
}// ns
