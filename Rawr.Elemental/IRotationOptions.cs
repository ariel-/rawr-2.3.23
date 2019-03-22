using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Elemental
{
    public interface IRotationOptions
    {
        bool UseFireNova { get; }
        bool UseChainLightning { get; }
        bool UseDpsFireTotem { get; }
        bool UseFireEle { get; }
    }
}
