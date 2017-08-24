
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
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using System.Threading;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[ErrorChecker("Check unknown textures", true, 60)]
	public class CheckUnknownTextures : ErrorChecker
	{
		#region ================== Constants

		private int PROGRESS_STEP = 1000;

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public CheckUnknownTextures()
		{
			// Total progress is done when all lines are checked
			SetTotalProgress(General.Map.Map.Sidedefs.Count / PROGRESS_STEP);
		}

		#endregion

		#region ================== Methods

		// This runs the check
		public override void Run()
		{
			int progress = 0;
			int stepprogress = 0;

			// Go for all the sidedefs
			foreach(Sidedef sd in General.Map.Map.Sidedefs)
			{
				// Check upper texture
				if(sd.HighRequired() && ((sd.HighTexture.Length < 1) || (sd.HighTexture[0] != '-')))
				{
					if(!General.Map.Data.GetTextureExists(sd.LongHighTexture))
						SubmitResult(new ResultUnknownTexture(sd, SidedefPart.Upper));
				}

				// Check middle texture
				if(sd.MiddleRequired() && ((sd.MiddleTexture.Length < 1) || (sd.MiddleTexture[0] != '-')))
				{
					if(!General.Map.Data.GetTextureExists(sd.LongMiddleTexture))
						SubmitResult(new ResultUnknownTexture(sd, SidedefPart.Middle));
				}

				// Check lower texture
				if(sd.LowRequired() && ((sd.LowTexture.Length < 1) || (sd.LowTexture[0] != '-')))
				{
					if(!General.Map.Data.GetTextureExists(sd.LongLowTexture))
						SubmitResult(new ResultUnknownTexture(sd, SidedefPart.Lower));
				}
				
				// Handle thread interruption
				try { Thread.Sleep(0); }
				catch(ThreadInterruptedException) { return; }

				// We are making progress!
				if((++progress / PROGRESS_STEP) > stepprogress)
				{
					stepprogress = (progress / PROGRESS_STEP);
					AddProgress(1);
				}
			}
		}

		#endregion
	}
}
