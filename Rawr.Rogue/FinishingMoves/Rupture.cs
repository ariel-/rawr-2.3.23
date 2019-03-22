using System;
using Rawr.Rogue.ClassAbilities;
using Rawr.Rogue.ComboPointGenerators;

namespace Rawr.Rogue.FinishingMoves
{
#if (SILVERLIGHT == false)
    [Serializable]
#endif
    public class Rupture : FinisherBase
    {
        /*public const string NAME = "Rupture";

        public override char Id { get { return 'R'; } }

        public override string Name { get { return NAME; } }

        public override float EnergyCost(CombatFactors combatFactors, int rank)
        {
            float baseCost = 25f;
            float rsBonus = 25f * rank * Talents.RelentlessStrikes.Bonus;
            float missCost = (Talents.SurpriseAttacks.HasPoints) ? 0 : 25f * (1f - Talents.QuickRecovery.Bonus) * combatFactors.YellowMissChance;

            return baseCost - rsBonus + missCost;
        }

        public override float CalcFinisherDPS( CalculationOptionsRogue calcOpts, Stats stats, CombatFactors combatFactors, int rank, CycleTime cycleTime, WhiteAttacks whiteAttacks, CharacterCalculationsRogue displayValues )
        {
            float baseDamage;
            float duration;

            switch (rank)
            {
                case 5:
                    baseDamage = 8f * (stats.AttackPower * 0.0375f + 217f);
                    duration = 16f;
                    break;
                case 4:
                    baseDamage = 7f * (stats.AttackPower * 0.03428571f + 199f);
                    duration = 14f;
                    break;
                case 3:
                    baseDamage = 6f * (stats.AttackPower * 0.03f + 181f);
                    duration = 12f;
                    break;
                case 2:
                    baseDamage = 5f * (stats.AttackPower * 0.024f + 163f);
                    duration = 10f;
                    break;
                default:
                    baseDamage = 4f * (stats.AttackPower * .015f + 145f);
                    duration = 8f;
                    break;
            }

            var baseDamagePerSecond = baseDamage / duration;
            duration += BonusGlyphOfRuptureDuration;
            duration += BonusDurationFromBackstab(calcOpts, combatFactors, cycleTime, duration);
            var actualDuration = Math.Min(duration, cycleTime.Duration);

            var finisherDmg = baseDamagePerSecond * actualDuration;
            finisherDmg *= Talents.Add(Talents.SerratedBlades.Rupture, Talents.BloodSpatter, Talents.FindWeakness, Talents.Murder, Talents.DirtyDeeds.DamageSpecialAbilities, Talents.HungerForBlood.Damage).Multiplier;
            finisherDmg *= ( 1f + stats.BonusBleedDamageMultiplier );
            finisherDmg *= ( 1f - combatFactors.YellowMissChance );
            finisherDmg *= combatFactors.Tier7TwoPieceRuptureBonusDamage;
            finisherDmg *= combatFactors.Tier8FourPieceRuptureCrit;
            if (!Talents.SurpriseAttacks.HasPoints)
                finisherDmg *= (1f - combatFactors.MhDodgeChance);
            return finisherDmg / cycleTime.Duration;
        }

        public float Duration(int CPs)
        {
            return 6 + 2 * CPs + BonusGlyphOfRuptureDuration;
        }

        public static float BonusDurationFromBackstab(CalculationOptionsRogue calcOpts, CombatFactors combatFactors, CycleTime cycleTime, float ruptureDuration)
        {
            if (!Glyphs.GlyphOfBackstab || (calcOpts.CpGenerator.Name != Backstab.NAME))
            {
                return 0f;
            }

            var delayBetweenBackstabs = new Backstab().EnergyCost(combatFactors, calcOpts) / cycleTime.EnergyRegen;

            if((delayBetweenBackstabs*3) < (ruptureDuration+4f))
                return 6f;

            if((delayBetweenBackstabs*2) < (ruptureDuration+2f))
                return 4f;

            if(delayBetweenBackstabs < ruptureDuration)
                return 2f;
            
            return 0f;
        }

        private static float BonusGlyphOfRuptureDuration
        {
            get { return Glyphs.GlyphOfRupture ? 4f : 0f; }
        }*/
    }
}

