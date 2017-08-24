#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal enum RenderLayers : int
	{
		None = 0,
		Background = 1,
		Plotter = 2,
		Things = 3,
		Overlay = 4,
		Surface = 5
	}
}
