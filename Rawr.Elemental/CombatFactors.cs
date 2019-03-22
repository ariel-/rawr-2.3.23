using System;
using Rawr.Elemental.Spells;

namespace Rawr.Elemental
{
    /// <summary>
    /// This class holds all combat relevant information.
    /// It is used as argument to a Spell or Spellbox in order to Initialize its values.
    /// </summary>
    public class CombatFactors : ISpellArgs, IRotationOptions
    {

        public CombatFactors(ShamanTalents talents, Stats stats, int additionalTargets, float latencyCast, float latencyGcd) : this(talents, stats, additionalTargets, latencyCast, latencyGcd, true, true, false)
        {
        }

        public CombatFactors(ShamanTalents talents, Stats stats, int additionalTargets, float latencyCast, float latencyGcd, bool useFireNova, bool useChainLightning, bool useDpsFireTotem)
        {
            Talents = talents;
            Stats = stats;
            AdditionalTargets = additionalTargets;
            LatencyCast = latencyCast;
            LatencyGCD = latencyGcd;
            UseFireNova = useFireNova;
            UseChainLightning = useChainLightning;
            UseDpsFireTotem = useDpsFireTotem;
        }

        #region ISpellArgs Member

        public ShamanTalents Talents
        {
            get;
            set;
        }

        public Stats Stats
        {
            get;
            set;
        }

        public float LatencyCast
        {
            get;
            set;
        }

        public float LatencyGCD
        {
            get;
            set;
        }

        public int AdditionalTargets
        {
            get;
            set;
        }

        #endregion

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

        public float FightDuration
        {
            get;
            set;
        }

        #endregion
    }
}
