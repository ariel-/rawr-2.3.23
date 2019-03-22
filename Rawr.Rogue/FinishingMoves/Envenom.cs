using System;
using Rawr.Rogue.ClassAbilities;
using Rawr.Rogue.Poisons;

namespace Rawr.Rogue.FinishingMoves
{
#if (SILVERLIGHT == false)
    [Serializable]
#endif
    public class Envenom : FinisherBase
    {
        /*public const string NAME = "Envenom";
        public override char Id
        {
            get { return 'N'; }
        }

        public override string Name
        {
            get { return NAME; }
        }

        public override float EnergyCost( CombatFactors combatFactors, int rank )
        {
            float baseCost = 35f;
            float rsBonus = 25f * rank * Talents.RelentlessStrikes.Bonus;
            float missCost  = (Talents.SurpriseAttacks.HasPoints) ? 0 : 35f * (1f - Talents.QuickRecovery.Bonus) * combatFactors.YellowMissChance;

            return baseCost - rsBonus + missCost;
        }

        public override float CalcFinisherDPS( CalculationOptionsRogue calcOpts, Stats stats, CombatFactors combatFactors, int rank, CycleTime cycleTime, WhiteAttacks whiteAttacks, CharacterCalculationsRogue displayValues )
        {
            float dpAverageStackSize = CalcAverageStackSize(calcOpts, combatFactors, whiteAttacks, rank, cycleTime);
            float baseDamage = (215f + stats.AttackPower * 0.09f) * dpAverageStackSize;
            baseDamage *= Talents.Add(Talents.VilePoisons, Talents.FindWeakness, Talents.Murder, Talents.HungerForBlood.Damage).Multiplier * (1f + stats.BonusNatureDamageMultiplier) * 0.95f;

            float critDamage = baseDamage * combatFactors.BaseCritMultiplier;

            return (baseDamage * (1 - CritChance(combatFactors, calcOpts)) + critDamage * CritChance(combatFactors, calcOpts)) / cycleTime.Duration;
        }

         private static float CalcAverageStackSize(CalculationOptionsRogue calcOpts, CombatFactors combatFactors, WhiteAttacks whiteAttacks, int rank, CycleTime cycleTime)
        {
            float countEnve = 0;
            float timeEnvnProc = 0;
            float consumeDP = 0;

            foreach (CycleComponent comp in calcOpts.DpsCycle.Components)
            {
                if (comp.Finisher.Name == "Envenom")
                {
                    countEnve++;
                    timeEnvnProc += (float)comp.Rank + 1f;
                    consumeDP += (float)comp.Rank * (1f - Talents.MasterPoisoner.NotConsumeDeadlyPoison.Bonus);
                }
            }

            float mhPoisonHits = (calcOpts.TempMainHandEnchant.IsDeadlyPoison) ? cycleTime.Duration * whiteAttacks.MhSwingsPerSecond + calcOpts.CpGenerator.MhHitsNeeded(combatFactors,calcOpts) : 0;
            float ohPoisonHits = (calcOpts.TempOffHandEnchant.IsDeadlyPoison)  ? cycleTime.Duration * whiteAttacks.OhSwingsPerSecond : 0;

            float hits = (mhPoisonHits + ohPoisonHits) * (1f - combatFactors.PoisonMissChance);

            hits = hits * (cycleTime.Duration - timeEnvnProc) / cycleTime.Duration * DeadlyPoison.ChanceToApply(false) +
                   hits * timeEnvnProc / cycleTime.Duration * DeadlyPoison.ChanceToApply(true);

            float avgHitTime = cycleTime.Duration / hits;

            float sDP = (countEnve > 0) ? (5f - consumeDP / countEnve) : 5f;
            float eDP = 5f;

            float stacking = (sDP + eDP) * .5f * consumeDP * avgHitTime;
            float stacked = eDP * (cycleTime.Duration - consumeDP * avgHitTime);

            return Math.Min(rank, (stacking + stacked) / cycleTime.Duration);
        }

        private float CritChance(CombatFactors combatFactors, CalculationOptionsRogue calcOpts)
        {
            return combatFactors.ProbMhCrit + CritBonusFromTurnTheTables(calcOpts);
        }*/
    }
}
