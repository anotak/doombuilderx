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
    public partial class ScriptMessageForm : Form
    {
        public ScriptMessageForm()
        {
            InitializeComponent();
        }
        
        public static void ShowMessage(string text)
        {
            ScriptMessageForm box = new ScriptMessageForm();
            box.acceptButton.Focus();
            box.acceptButton.Select();
            // i hate this, it's ugly, but it's too much of a pain to do it otherwise
            text = text.Replace("\n", Environment.NewLine);
            box.messageTextBox.Clear();
            box.messageTextBox.AppendText(text);

            box.ShowDialog();
        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
