using System;
using Rawr.Rogue.ClassAbilities;

namespace Rawr.Rogue.ComboPointGenerators
{
#if (SILVERLIGHT == false)
    [Serializable]
#endif

    public class SinisterStrike : ComboPointGenerator
    {
        /*public override string Name { get { return "Sinister Strike"; } }

        public override float EnergyCost(CombatFactors combatFactors, CalculationOptionsRogue calcOpts)
        {
            return 45f * combatFactors.Tier7FourPieceEnergyCostReduction
                - Talents.ImprovedSinisterStrike.Bonus 
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
            baseDamage *= Talents.Add(Talents.FindWeakness, Talents.Murder, Talents.Aggression, Talents.BladeTwisting, Talents.SurpriseAttacks, Talents.DirtyDeeds.DamageSpecialAbilities, Talents.HungerForBlood.Damage).Multiplier;
			baseDamage *= combatFactors.MhDamageReduction;
            var critDamage = baseDamage * CriticalDamageMultiplier(combatFactors) * Crit(combatFactors, calcOpts);
            var nonCritDamage = baseDamage * Math.Max(combatFactors.ProbYellowHit - Crit(combatFactors, calcOpts), 0);

            return (critDamage + nonCritDamage) * calcOpts.ComboPointsNeededForCycle(combatFactors.T10x4ChanceOn3CPOnFinisher) / cycleTime.Duration;
        }

        private static float BaseAttackDamage(CombatFactors combatFactors)
        {
            var damage = combatFactors.MhNormalizedDamage;
            damage += 180f;
            return damage;
        }

        protected override float ComboPointsGeneratedPerAttack(CombatFactors combatFactors, CalculationOptionsRogue calcOpts)
        {
            float CpCriRate = Crit(combatFactors, calcOpts);
            return 1 + (Talents.SealFate.Bonus * CpCriRate) + (Glyphs.GlyphOfSinisterStrike ? .5f * CpCriRate : 0f);
        }*/
    }
}