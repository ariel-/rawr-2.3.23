using System;
using Rawr.Rogue.ClassAbilities;

namespace Rawr.Rogue.ComboPointGenerators
{
#if (SILVERLIGHT == false)
    [Serializable]
#endif
    public class Backstab : ComboPointGenerator
    {
        /*public const string NAME = "Backstab";

        public override string Name { get { return NAME; } }

        public override float EnergyCost(CombatFactors combatFactors, CalculationOptionsRogue calcOpts)
        {
            return 60f * combatFactors.Tier7FourPieceEnergyCostReduction
                - Talents.SlaughterFromTheShadows.BackstabAndAmbushEnergyCost.Bonus
                - (Crit(combatFactors, calcOpts) * Talents.FocusedAttacks.Bonus);  
        }

        public override float Crit( CombatFactors combatFactors, CalculationOptionsRogue calcOpts )
        {
            float baseCrit = combatFactors.ProbMhCrit + Talents.PuncturingWounds.Backstab.Bonus + CritBonusFromTurnTheTables(calcOpts) + combatFactors.T09x4BonusCPGCritChance;

            return Math.Min(combatFactors.ProbYellowHit, baseCrit);
        }

        public override float CalcCpgDps(CalculationOptionsRogue calcOpts, CombatFactors combatFactors, Stats stats, CycleTime cycleTime)
        {
            var baseDamage = BaseAttackDamage(combatFactors);
            baseDamage *= Talents.Add(Talents.FindWeakness, Talents.Murder, Talents.Aggression, Talents.BladeTwisting, Talents.SurpriseAttacks, Talents.Opportunity, Talents.DirtyDeeds.DamageSpecialAbilities, Talents.HungerForBlood.Damage).Multiplier;
            baseDamage *= combatFactors.MhDamageReduction;
            var critDamage = baseDamage * CriticalDamageMultiplier(combatFactors) * Crit(combatFactors, calcOpts);
            var nonCritDamage = baseDamage * Math.Max(combatFactors.ProbYellowHit - Crit(combatFactors, calcOpts), 0);

            return (critDamage + nonCritDamage) * calcOpts.ComboPointsNeededForCycle(combatFactors.T10x4ChanceOn3CPOnFinisher) / cycleTime.Duration;
        }

        private static float BaseAttackDamage(CombatFactors combatFactors)
        {
            if (combatFactors.MH.Type == ItemType.Dagger)
            {
            var attackDamage = combatFactors.MhNormalizedDamage;
            attackDamage += 465;
            attackDamage *= (1.5f + Talents.SinisterCalling.HemoAndBackstab.Bonus);
            return attackDamage;
        }
            else
            {
                return 0f;
            }
        }*/
    }
}