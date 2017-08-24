
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

namespace CodeImp.DoomBuilder.Types
{
	internal class NullHandler : TypeHandler
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private object value = (int)0;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Methods

		public override void SetValue(object value)
		{
			if(value != null)
				this.value = value;
			else
				this.value = (int)0;
		}

		public override object GetValue()
		{
			return this.value.ToString();
		}

		public override int GetIntValue()
		{
			int result;
			if(int.TryParse(this.value.ToString(), out result)) return result;
				else return 0;
		}
		
		public override string GetStringValue()
		{
			return this.value.ToString();
		}


        public override object GetDefaultValue()
        {
            return 0;
        }

        #endregion
    }
}
