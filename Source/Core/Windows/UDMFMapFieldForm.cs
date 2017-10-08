using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Types;

namespace CodeImp.DoomBuilder.Windows
{
    public partial class UDMFMapFieldForm : DelayedForm
    {
        UniFields fields;

        public UDMFMapFieldForm()
        {
            InitializeComponent();

            fields = new UniFields();

            if (General.Map.Map.UnidentifiedUDMF != null)
            {
                fieldslist.Setup("map");

                fieldslist.ClearFields();

                foreach (UniversalEntry u in General.Map.Map.UnidentifiedUDMF)
                {
                    if (!(u.Value is UniversalCollection))
                    {
                        int type = (int)UniversalType.Integer;

                        if (u.Value.GetType() == typeof(float)) type = (int)UniversalType.Float;
                        else if (u.Value.GetType() == typeof(bool)) type = (int)UniversalType.Boolean;
                        else if (u.Value.GetType() == typeof(string)) type = (int)UniversalType.String;

                        fields.Add(u.Key,new UniValue(type, u.Value));
                    }
                }
                // Custom fields
                fieldslist.SetValues(fields, true);
            }
        }

        private void apply_Click(object sender, EventArgs e)
        {
            General.Map.UndoRedo.CreateUndo("Edit Map UDMF Info");

            fieldslist.Apply(fields);

            UniversalCollection collection = new UniversalCollection();

            foreach (KeyValuePair<string, UniValue> f in fields)
            {
                collection.Add(f.Key, f.Value.Value);
            }

            // preserve collections
            foreach (UniversalEntry u in General.Map.Map.UnidentifiedUDMF)
            {
                if (u.Value is UniversalCollection)
                {
                    collection.Add(u);
                }
            }

            General.Map.Map.UnidentifiedUDMF = collection;

            General.Map.IsChanged = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            // Just close
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    } //class
} // ns
