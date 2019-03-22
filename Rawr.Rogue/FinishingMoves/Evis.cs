using System;
using Rawr.Rogue.ClassAbilities;

namespace Rawr.Rogue.FinishingMoves
{
#if (SILVERLIGHT == false)
    [Serializable]
#endif
    public class Evis : FinisherBase
    {
        /*public const string NAME = "Evis";

        public override char Id { get { return 'E'; } }

        public override string Name { get { return NAME; } }

        public override float EnergyCost(CombatFactors combatFactors, int rank)
        {
            float baseCost = 35f;
            float rsBonus = 25f * rank * Talents.RelentlessStrikes.Bonus;
            float missCost = (Talents.SurpriseAttacks.HasPoints) ? 0 : 35f * (1f - Talents.QuickRecovery.Bonus) * combatFactors.YellowMissChance;

            return baseCost - rsBonus + missCost;
        }

        public override float CalcFinisherDPS( CalculationOptionsRogue calcOpts, Stats stats, CombatFactors combatFactors, int rank, CycleTime cycleTime, WhiteAttacks whiteAttacks, CharacterCalculationsRogue displayValues )
        {
            var evisMod = stats.AttackPower * rank * 0.03f;
            var evisMin = 245f + (rank-1f)*185f + evisMod;
            var evisMax = 365f + (rank-1f)*185f + evisMod;

            var baseFinisherDmg = (evisMin + evisMax)/2f;
            baseFinisherDmg *= Talents.Add(Talents.ImprovedEviscerate, Talents.FindWeakness, Talents.Murder, Talents.Aggression, Talents.HungerForBlood.Damage).Multiplier;

            var critDamage = (baseFinisherDmg * 2f * CritChance(combatFactors, calcOpts));
            var nonCritDamage = baseFinisherDmg * ( 1f - CritChance(combatFactors, calcOpts) );

            var finisherDmg = critDamage + nonCritDamage;
            finisherDmg *= (1f - (combatFactors.WhiteMissChance));

            if (!Talents.SurpriseAttacks.HasPoints) {
                finisherDmg *= (1f - (combatFactors.MhDodgeChance));
            }

            finisherDmg *= combatFactors.MhDamageReduction;
            return finisherDmg / cycleTime.Duration;
        }

        private float CritChance(CombatFactors combatFactors, CalculationOptionsRogue calcOpts) {
            return (float)Math.Max(0f,Math.Min(1f,combatFactors.ProbMhCrit + (Glyphs.GlyphOfEviscerate ? 0.1f : 0f) + CritBonusFromTurnTheTables(calcOpts)));
        }*/
    }
}