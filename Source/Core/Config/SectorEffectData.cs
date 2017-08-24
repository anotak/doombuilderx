using System.Collections.Generic;

// gzdb cross compat

namespace CodeImp.DoomBuilder.Config
{
    public class SectorEffectData
    {
        public int Effect;
        public HashSet<int> GeneralizedBits;

        public SectorEffectData()
        {
            GeneralizedBits = new HashSet<int>();
        }
    }
}
