using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Elemental
{
    public class RotationOptions : IRotationOptions
    {
        public RotationOptions(bool useFN, bool useCL, bool useDpsFireTotem, bool useFireEle)
        {
            UseFireNova = useFN;
            UseChainLightning = useCL;
            UseDpsFireTotem = useDpsFireTotem;
            UseFireEle = useFireEle;
        }
        #region IRotationOptions Member

        public bool UseFireNova
        {
            get;
            set;
        }

        public bool UseChainLightning
        {
            get;
            set;
        }

        public bool UseDpsFireTotem
        {
            get;
            set;
        }

        public bool UseFireEle
        {
            get;
            set;
        }

        #endregion
    }
}
