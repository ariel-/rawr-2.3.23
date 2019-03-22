using System;
using System.Xml.Serialization;
using Rawr.Rogue.ClassAbilities;

namespace Rawr.Rogue.ComboPointGenerators {
#if (SILVERLIGHT == false)
    [Serializable]
    [XmlInclude(typeof(Mutilate))]
    [XmlInclude(typeof(Backstab))]
    [XmlInclude(typeof(SinisterStrike))]
    [XmlInclude(typeof(Hemo))]
    [XmlInclude(typeof(HonorAmongThieves))]
#endif
    public abstract class ComboPointGenerator {
 /*       public abstract string Name { get; }
        public abstract float CalcCpgDps(CalculationOptionsRogue calcOpts, CombatFactors combatFactors, Stats stats, CycleTime cycleTime);
        public abstract float Crit( CombatFactors combatFactors, CalculationOptionsRogue calcOpts );
        public abstract float EnergyCost(CombatFactors combatFactors, CalculationOptionsRogue calcOpts);

        public virtual float CalcDuration(CalculationOptionsRogue calcOpts, float regen, CombatFactors combatFactors) {
            return MhHitsNeeded(combatFactors, calcOpts) * EnergyCost(combatFactors, calcOpts) / regen;
        }
        public virtual float MhHitsNeeded(CombatFactors combatFactors, CalculationOptionsRogue calcOpts) {
            return calcOpts.ComboPointsNeededForCycle(combatFactors.T10x4ChanceOn3CPOnFinisher) / ComboPointsGeneratedPerAttack(combatFactors, calcOpts);    
        }
        public virtual float OhHitsNeeded(CombatFactors combatFactors, CalculationOptionsRogue calcOpts) {
            return 0f;
        }
        protected virtual float ComboPointsGeneratedPerAttack( CombatFactors combatFactors, CalculationOptionsRogue calcOpts ) {
            return 1 + (Talents.SealFate.Bonus * Crit(combatFactors, calcOpts)); 
        }
        protected float CritBonusFromTurnTheTables(CalculationOptionsRogue calcOpts) {
            return calcOpts.TurnTheTablesUptime * Talents.TurnTheTables.Bonus;
        }
        protected float CriticalDamageMultiplier(CombatFactors combatFactors)
        {
            return combatFactors.BaseCritMultiplier * (1f + Talents.Lethality.Bonus);
        }*/
    }
}