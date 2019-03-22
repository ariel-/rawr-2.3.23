using System;
using Rawr.Rogue.ClassAbilities;
using Rawr.Rogue.FinishingMoves;

namespace Rawr.Rogue.Poisons
{
#if (SILVERLIGHT == false)
    [Serializable]
#endif
    public class InstantPoison : PoisonBase
    {
        /*private const float PROCS_PER_MINUTE = 8.53f;

        public override string Name { get { return "Instant Poison"; } }

        public override bool IsDeadlyPoison { get { return false; } }

        /// <summary>Returns the average Damage from one Instant Poison hit/crit.</summary>
        /// <returns>Average Damage from one Instant Poison hit/crit</returns>
        public float CalcPoisonApplied(Stats stats, CalculationOptionsRogue calcOpts, CombatFactors combatFactors)
        {
            float baseDamage = (300f + .10f * stats.AttackPower);
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
            float baseDamage = (300f + .10f * stats.AttackPower);
            baseDamage *= (1f + stats.BonusNatureDamageMultiplier) * (1f + stats.BonusDamageMultiplier);
            baseDamage *= (1f + Talents.VilePoisons.Bonus) * (1f + Talents.HungerForBlood.Damage.Bonus) * (1f + Talents.Murder.Bonus);
            baseDamage *= (1f - combatFactors.PoisonDamageReduction);

            float critDamage = baseDamage * combatFactors.BaseSpellCritMultiplier;

            float timeEnvnProc = 0;

            foreach (CycleComponent comp in calcOpts.DpsCycle.Components)
            {
                if(comp.Finisher.Name == "Envenom") timeEnvnProc += (float)comp.Rank + 1f;
            }

            hits = hits * (cycleTime.Duration - timeEnvnProc) / cycleTime.Duration * ChanceToApply(weapon, false) +
                   hits * timeEnvnProc / cycleTime.Duration * ChanceToApply(weapon, true);

            float damage = hits * (1f - combatFactors.ProbPoisonCrit) * baseDamage;
            damage += hits * combatFactors.ProbPoisonCrit * critDamage;

            return damage / cycleTime.Duration;
        }

        private static float ChanceToApply(Item weapon, bool bEnvenom)
        {
            float BaseChanceToApply = Talents.ImprovedPoisons.InstantPoison.Multiplier * (PROCS_PER_MINUTE / (60 / weapon.Speed));

            if(bEnvenom)    BaseChanceToApply *= 1.75f;

            return (BaseChanceToApply > 1f) ? 1f : BaseChanceToApply;
        }*/
    }
}