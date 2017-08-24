
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
using CodeImp.DoomBuilder.Config;
using System.IO;
using System.Diagnostics;

#endregion

namespace CodeImp.DoomBuilder.Types
{
	[TypeHandler(UniversalType.Integer, "Integer", true)]
	internal class IntegerHandler : TypeHandler
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private int value;
        private int defaultvalue; //mxd

        #endregion

        #region ================== Properties

        #endregion

        #region ================== Methods

        //mxd
        public override void SetupArgument(TypeHandlerAttribute attr, ArgumentInfo arginfo)
        {
            defaultvalue = (int)arginfo.DefaultValue;
            base.SetupArgument(attr, arginfo);
        }

        //mxd
        public override void ApplyDefaultValue()
        {
            value = defaultvalue;
        }

        public override void SetValue(object value)
		{
			int result;
			
			// Null?
			if(value == null)
			{
				this.value = 0;
			}
			// Compatible type?
			else if((value is int) || (value is float) || (value is bool))
			{
				// Set directly
				this.value = Convert.ToInt32(value);
			}
			else
			{
				// Try parsing as string
				if(int.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture, out result))
				{
					this.value = result;
				}
				else
				{
					this.value = 0;
				}
			}

			if(forargument)
			{
				this.value = General.Clamp(this.value, General.Map.FormatInterface.MinArgument, General.Map.FormatInterface.MaxArgument);
			}
		}

		public override object GetValue()
		{
			return this.value;
		}
		
		public override int GetIntValue()
		{
			return this.value;
		}

		public override string GetStringValue()
		{
			return this.value.ToString();
		}

        public override object GetDefaultValue()
        {
            return defaultvalue;
        }

        #endregion
    }
}
