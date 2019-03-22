using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Elemental.Spells
{
    public interface ISpellArgs
    {
        ShamanTalents Talents { get; }
        Stats Stats { get; }
        float LatencyCast { get; }
        float LatencyGCD {get;}
        int AdditionalTargets {get;}
    }
}
