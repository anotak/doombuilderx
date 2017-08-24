using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeImp.DoomBuilder.Plugins.RejectEditor
{
	public struct RejectSet
	{
		// Members
		public int target;
		public RejectState state;

		// Constructor
		public RejectSet(int targetsector, RejectState rejectstate)
		{
			this.target = targetsector;
			this.state = rejectstate;
		}
	}
}
