
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
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Data;
using System.IO;
using System.Diagnostics;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class ArgumentInfo
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private string title;
		private bool used;
		private int type;
		private EnumList enumlist;
        private readonly object defaultvalue; //mxd

        #endregion

        #region ================== Properties

        public string Title { get { return title; } }
		public bool Used { get { return used; } }
		public int Type { get { return type; } }
		public EnumList Enum { get { return enumlist; } internal set { enumlist = value; } }
        public object DefaultValue { get { return defaultvalue; } } //mxd

        #endregion

        #region ================== Constructor / Disposer

        // Constructor for argument info from configuration
        internal ArgumentInfo(Configuration cfg, string argspath, int argindex, IDictionary<string, EnumList> enums)
		{
			// Read
			string istr = argindex.ToString(CultureInfo.InvariantCulture);
			this.used = cfg.SettingExists(argspath + ".arg" + istr);
			this.title = cfg.ReadSetting(argspath + ".arg" + istr + ".title", "Argument " + (argindex + 1));
			this.type = cfg.ReadSetting(argspath + ".arg" + istr + ".type", 0);
            this.defaultvalue = cfg.ReadSetting(argspath + ".arg" + istr + ".default", 0); //mxd

			// Determine enum type
			//EnumList enumlist = null;
			IDictionary argdic = cfg.ReadSetting(argspath + ".arg" + istr, new Hashtable());
            if (argdic.Contains("enum"))
            {
                // Enum fully specified?
                if (argdic["enum"] is IDictionary)
                {
                    // Create anonymous enum
                    this.enumlist = new EnumList(argdic["enum"] as IDictionary);
                }
                else
                {
                    // Check if referenced enum exists
                    if ((argdic["enum"].ToString().Length > 0) && enums.ContainsKey(argdic["enum"].ToString()))
                    {
                        // Get the enum list
                        this.enumlist = enums[argdic["enum"].ToString()] as EnumList;
                    }
                    else
                    {
                        General.ErrorLogger.Add(ErrorType.Warning, "'" + argspath + ".arg" + istr + "' references unknown enumeration '" + argdic["enum"] + "'.");
                    }
                }
            }
            else if(type == (int)Types.UniversalType.EnumOption || type == (int)Types.UniversalType.EnumStrings || type == (int)Types.UniversalType.EnumBits)
            {
                General.ErrorLogger.Add(ErrorType.Warning, "'" + argspath + ".arg" + istr + "' is missing enumeration type.");
                type = 0;
            }
		}

		// Constructor for unknown argument info
		internal ArgumentInfo(int argindex)
		{
			// Read
			this.used = false;
			this.title = "Argument " + (argindex + 1);
			this.type = 0;
			this.enumlist = new EnumList();
            this.defaultvalue = 0; //mxd
        }

		#endregion

		#region ================== Methods

		// This gets the description for an argument value
		public string GetValueDescription(int value)
		{
			// TODO: Use the registered type editor to get the description!

			return value.ToString();
		}

		#endregion
	}
}
