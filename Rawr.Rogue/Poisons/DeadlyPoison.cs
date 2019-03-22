using System;
using Rawr.Rogue.ClassAbilities;

namespace Rawr.Rogue.Poisons
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class DeadlyPoison : PoisonBase
    {
        /*public override string Name { get { return "Deadly Poison"; } }

        private const float STACK_SIZE = 5f;
        private const float DURATION = 12f;

        public override bool IsDeadlyPoison { get { return true; } }

        public override float CalcPoisonDps( Stats stats, CalculationOptionsRogue calcOpts, CombatFactors combatFactors, float hits, CycleTime cycleTime, Item weapon )
        {
            float baseDamage = (296f + .12f * stats.AttackPower);
            baseDamage *= (1f + stats.BonusNatureDamageMultiplier) * (1f + stats.BonusDamageMultiplier);
            baseDamage *= (1f + Talents.VilePoisons.Bonus) * (1f + Talents.HungerForBlood.Damage.Bonus) * (1f + Talents.Murder.Bonus);
            baseDamage *= (1f - combatFactors.PoisonDamageReduction);

            float countEnve = 0;// Number of Envenoms used
            float timeEnvnProc = 0;//Envenom uptime
            float consumeDP = 0;// Number of DP Stacks lost due to Envenom

            foreach (CycleComponent comp in calcOpts.DpsCycle.Components)
            {
                if (comp.Finisher.Name == "Envenom")
                {
                    countEnve++;
                    timeEnvnProc += (float)comp.Rank + 1f;
                    consumeDP += (float)comp.Rank * (1f - Talents.MasterPoisoner.NotConsumeDeadlyPoison.Bonus);
                }
            }

            hits = hits * (cycleTime.Duration - timeEnvnProc) / cycleTime.Duration * ChanceToApply(false) +
                   hits * timeEnvnProc / cycleTime.Duration * ChanceToApply(true);//number of dp applies

            float avgHitTime = cycleTime.Duration / hits;//time between dp applies

            float sDPPerSec = (countEnve > 0) ? (STACK_SIZE - consumeDP / countEnve) * baseDamage / DURATION : 0;// dp avg incomplete stack dps
            float eDPPerSec = STACK_SIZE * baseDamage / DURATION;// dp 5er stack dps

            float damageStacking = (sDPPerSec + eDPPerSec) * .5f * consumeDP * avgHitTime;
            float damageStacked = eDPPerSec * (cycleTime.Duration - consumeDP * avgHitTime);

            float otherHandPoisonDamage = 0f; // Damage dealt by other Poison if dp has 5 stacks & procs
            if (!calcOpts.TempMainHandEnchant.IsDeadlyPoison) //MH != dp
            {
                    otherHandPoisonDamage = OtherHandPoisonDamage( stats,  calcOpts,  combatFactors);
            }
            else
            {
                if (!calcOpts.TempOffHandEnchant.IsDeadlyPoison) //OH != dp
                {
                    otherHandPoisonDamage = OtherHandPoisonDamage(stats, calcOpts, combatFactors);
                }
            }
            float surplusDPStacks = hits - consumeDP - 5; // surplus dp applies, which will be used by other weapon poison
            float additionalPoisonDamage = surplusDPStacks * otherHandPoisonDamage; //additional Damage dealt by other weapon Poison if dp has 5 stacks & procs for the complete duration

            return (additionalPoisonDamage + damageStacking + damageStacked) / cycleTime.Duration;
        }

        /// <summary>Returns the average Damage one Poison hit/crit the other Poison than Deadly Poison deals.</summary>
        /// <returns> Average Damage from one Poison hit/crit</returns>
        public static float OtherHandPoisonDamage(Stats stats, CalculationOptionsRogue calcOpts, CombatFactors combatFactors )
        {
            if (calcOpts.TempMainHandEnchant.Name == "Instant Poison" || calcOpts.TempOffHandEnchant.Name == "Instant Poison")
            {
                InstantPoison poison = new InstantPoison();
                return poison.CalcPoisonApplied(stats, calcOpts, combatFactors);
            } 
            if (calcOpts.TempMainHandEnchant.Name == "Wound Poison" || calcOpts.TempOffHandEnchant.Name == "Wound Poison")
            {
                WoundPoison poison = new WoundPoison();
                return poison.CalcPoisonApplied(stats, calcOpts, combatFactors);
            }
            return 0f;
        }
        public static float ChanceToApply(bool bEnvenom)
        {
            float BaseChanceToApply = .3f + Talents.ImprovedPoisons.DeadlyPoison.Bonus;

            if (bEnvenom) BaseChanceToApply += 0.15f;

            return (BaseChanceToApply > 1f) ? 1f : BaseChanceToApply;
        }*/
    }
}

