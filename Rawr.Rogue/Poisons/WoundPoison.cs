using System;
using Rawr.Rogue.ClassAbilities;

namespace Rawr.Rogue.Poisons
{
#if (SILVERLIGHT == false)
    [Serializable]
#endif

    public class WoundPoison : PoisonBase
    {
        /*private const float PROCS_PER_MINUTE = 21.43f;

        public override string Name
        {
            get { return "Wound Poison"; }
        }

        public override bool IsDeadlyPoison
        {
            get { return false; }
        }

        /// <summary>Returns the average Damage from one Wound Poison hit/crit.</summary>
        /// <returns> Average Damage from one Wound Poison hit/crit</returns>
        public float CalcPoisonApplied(Stats stats, CalculationOptionsRogue calcOpts, CombatFactors combatFactors)
        {
            float baseDamage = (231f + 0.04f * stats.AttackPower);
            baseDamage *= (1f + stats.BonusNatureDamageMultiplier) * (1f + stats.BonusDamageMultiplier);
            baseDamage *= (1f + Talents.VilePoisons.Bonus) * (1f + Talents.HungerForBlood.Damage.Bonus) * (1f + Talents.Murder.Bonus);
            baseDamage *= (1f - combatFactors.PoisonDamageReduction);

            float critDamage = baseDamage * combatFactors.BaseSpellCritMultiplier;

            float damage = (1f - combatFactors.ProbPoisonCrit) * baseDamage;
            damage += combatFactors.ProbPoisonCrit * critDamage;

            return damage;
        }

        public override float CalcPoisonDps( Stats stats, CalculationOptionsRogue calcOpts, CombatFactors combatFactors, float hits, CycleTime cycleTime, Item weapon )
        {
            float baseDamage = (231f + 0.04f * stats.AttackPower);
            baseDamage *= (1f + stats.BonusNatureDamageMultiplier) * (1f + stats.BonusDamageMultiplier);
            baseDamage *= (1f + Talents.VilePoisons.Bonus) * (1f + Talents.HungerForBlood.Damage.Bonus) * (1f + Talents.Murder.Bonus);
            baseDamage *= (1f - combatFactors.PoisonDamageReduction);

            float critDamage = baseDamage * combatFactors.BaseSpellCritMultiplier;

            hits *= ChanceToApply(weapon);

            float damage = hits * (1f - combatFactors.ProbPoisonCrit) * baseDamage;
            damage += hits * combatFactors.ProbPoisonCrit * critDamage;

            return damage / cycleTime.Duration;
        }

        private static float ChanceToApply( Item weapon )
        {
            return BaseChanceToApply(weapon) > 1f ? 1f : BaseChanceToApply(weapon);
        }

        private static float BaseChanceToApply( Item weapon )
        {
            return PROCS_PER_MINUTE / ( 60 / weapon.Speed );
        }*/
    }
}