using System;
using Rawr.Rogue.ClassAbilities;

namespace Rawr.Rogue.ComboPointGenerators
{
#if (SILVERLIGHT == false)
    [Serializable]
#endif

    public class Hemo : ComboPointGenerator
    {
        /*public override string Name { get { return "Hemo"; } }

        public override float EnergyCost(CombatFactors combatFactors, CalculationOptionsRogue calcOpts)
        {
            return 35f * combatFactors.Tier7FourPieceEnergyCostReduction
                - Talents.SlaughterFromTheShadows.HemoEnergyCost.Bonus 
                - (Crit(combatFactors, calcOpts) * Talents.FocusedAttacks.Bonus); 
        }

        public override float Crit( CombatFactors combatFactors, CalculationOptionsRogue calcOpts )
        {
            float baseCrit = combatFactors.ProbMhCrit + CritBonusFromTurnTheTables(calcOpts) + combatFactors.T09x4BonusCPGCritChance;

            return Math.Min(combatFactors.ProbYellowHit, baseCrit);
		}

        public override float CalcCpgDps(CalculationOptionsRogue calcOpts, CombatFactors combatFactors, Stats stats, CycleTime cycleTime)
        {
            var baseDamage = BaseAttackDamage(combatFactors);
            baseDamage *= Talents.Add(Talents.DirtyDeeds.DamageSpecialAbilities, Talents.Murder, Talents.FindWeakness, Talents.SurpriseAttacks, Talents.HungerForBlood.Damage).Multiplier;
            baseDamage *= combatFactors.MhDamageReduction;

            var critDamage = baseDamage * CriticalDamageMultiplier(combatFactors) * Crit(combatFactors, calcOpts);
            var nonCritDamage = baseDamage * Math.Max(combatFactors.ProbYellowHit - Crit(combatFactors, calcOpts), 0);

            return (critDamage + nonCritDamage) * calcOpts.ComboPointsNeededForCycle(combatFactors.T10x4ChanceOn3CPOnFinisher) / cycleTime.Duration;
        }

        private static float BaseAttackDamage(CombatFactors combatFactors)
        {
            var damage = combatFactors.MhNormalizedDamage;
            damage *= (1.1f + Talents.SinisterCalling.HemoAndBackstab.Bonus);
            return damage;
        }*/
    }
}