
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
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

#endregion

namespace CodeImp.DoomBuilder.Config
{
    internal sealed class FlatsWallsTextureSet : TextureSet, IFilledTextureSet
    {
        #region ================== Constants

        public const string NAME = "Flats";

        #endregion

        #region ================== Variables

        // Matching textures and flats
        private List<ImageData> flats;

        #endregion

        #region ================== Properties
        
        public ICollection<ImageData> Flats { get { return flats; } }
        public ICollection<ImageData> Textures { get { return flats; } }

        #endregion

        #region ================== Constructor / Destructor

        // New texture set constructor
        public FlatsWallsTextureSet()
        {
            this.name = NAME;
            this.flats = new List<ImageData>();
        }

        #endregion

        #region ================== Methods

        internal void AddFlat(ImageData image)
        {
            flats.Add(image);
        }

        #endregion
    }
}
