using System;
using Rawr.Rogue.ClassAbilities;
using Rawr.Rogue.Poisons;

namespace Rawr.Rogue.ComboPointGenerators
{
#if (SILVERLIGHT == false)
    [Serializable]
#endif
    public class Mutilate : ComboPointGenerator
    {
        /*public const string NAME = "Mutilate";

        public override string Name { get { return NAME; } }

        public override float EnergyCost(CombatFactors combatFactors, CalculationOptionsRogue calcOpts)
		{ 
			//Assume Mutiliate can only crit once, so Focused Attacks can only return 2 energy, and
			//not 2 energy for a MH crit, and another 2 energy (total 4) for a MH and OH crit
            return 60f * combatFactors.Tier7FourPieceEnergyCostReduction
                - (Crit(combatFactors, calcOpts) * Talents.FocusedAttacks.Bonus)
                - GlyphOfMutilateBonus; 
		}

        private static float GlyphOfMutilateBonus { get { return Glyphs.GlyphOfMutilate ? 5f : 0f; } }

        public override float Crit( CombatFactors combatFactors, CalculationOptionsRogue calcOpts )
        {
            float baseCrit = combatFactors.ProbMhCrit + Talents.PuncturingWounds.Mutilate.Bonus + CritBonusFromTurnTheTables(calcOpts) + combatFactors.T09x4BonusCPGCritChance;

            return Math.Min(combatFactors.ProbYellowHit, baseCrit);
		}

        protected override float ComboPointsGeneratedPerAttack(CombatFactors combatFactors, CalculationOptionsRogue calcOpts)
		{
            float BothNoCri = (1f - Crit(combatFactors,calcOpts)) * (1f - Crit(combatFactors,calcOpts));
            return 2f + (Talents.SealFate.Bonus * (1f - BothNoCri));
		}

        public override float CalcCpgDps(CalculationOptionsRogue calcOpts, CombatFactors combatFactors, Stats stats, CycleTime cycleTime)
        {
            var baseDamage = BaseAttackDamage(combatFactors);
            baseDamage *= Talents.Add(Talents.FindWeakness, Talents.Murder, Talents.Opportunity, Talents.DirtyDeeds.DamageSpecialAbilities, Talents.HungerForBlood.Damage).Multiplier;
            baseDamage *= BonusIfTargetIsPoisoned(calcOpts);
            baseDamage *= combatFactors.MhDamageReduction;

            var critDamage = baseDamage * CriticalDamageMultiplier(combatFactors) * Crit(combatFactors, calcOpts);
            var nonCritDamage = baseDamage * Math.Max(combatFactors.ProbYellowHit - Crit(combatFactors, calcOpts), 0);

            return (critDamage + nonCritDamage) * (calcOpts.ComboPointsNeededForCycle(combatFactors.T10x4ChanceOn3CPOnFinisher) / ComboPointsGeneratedPerAttack(combatFactors, calcOpts)) / cycleTime.Duration;
        }

        private static float BaseAttackDamage(CombatFactors combatFactors)
        {
            if (combatFactors.MH.Type == ItemType.Dagger && combatFactors.OH.Type == ItemType.Dagger)
            {
            var damage = combatFactors.MhNormalizedDamage + 181;
                damage += combatFactors.OhNormalizedDamage + (181 * 2 * combatFactors.OffHandDamagePenalty);
            return damage;
        }
            else
            {
                return 0f;
            }
        }

        private static float BonusIfTargetIsPoisoned(CalculationOptionsRogue calcOpts)
        {
            if (calcOpts.TempMainHandEnchant.IsDeadlyPoison
                || calcOpts.TempOffHandEnchant.IsDeadlyPoison
                || calcOpts.TempMainHandEnchant.Name == new WoundPoison().Name
                || calcOpts.TempOffHandEnchant.Name == new WoundPoison().Name)
            {
                return 1.2f;
            }

            return 1f;
        }

        public override float OhHitsNeeded(CombatFactors combatFactors, CalculationOptionsRogue calcOpts)
        {
            return MhHitsNeeded(combatFactors, calcOpts);
        }*/
    }
}