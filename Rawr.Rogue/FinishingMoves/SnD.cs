using System;
using System.Collections.Generic;
using Rawr.Rogue.ClassAbilities;

namespace Rawr.Rogue.FinishingMoves
{
#if (SILVERLIGHT == false)
    [Serializable]
#endif
    public class SnD : FinisherBase {
        /*public const string NAME = "SnD";
        private const float BASE_HASTE_BONUS = .4f;
        public override char Id { get { return 'S'; } }
        public override string Name { get { return NAME; } }
        public override float EnergyCost(CombatFactors combatFactors, int rank)
        {
            float baseCost = 25f;
            float rsBonus = 25f * rank * Talents.RelentlessStrikes.Bonus;

            return baseCost - rsBonus;
            //  return 25f - Talents.RelentlessStrikes.Bonus;
        }
        public override float CalcFinisherDPS(CalculationOptionsRogue calcOpts, Stats stats, CombatFactors combatFactors, int rank, CycleTime cycleTime, WhiteAttacks whiteAttacks, CharacterCalculationsRogue displayValues ) {
            return 0f;
        }
        public static float UpTime(CalculationOptionsRogue calcOpts, CycleTime cycleTime) {
            var upTime = DurationCalculatedFromCutToTheChase(calcOpts, cycleTime);
            
            if(!calcOpts.DpsCycle.Includes(SnD.NAME)) {
                return upTime;
            }

            if(cycleTime.Duration <= SndDuration(calcOpts)) {
                return 1f;
            }

            upTime += SndDuration(calcOpts) / cycleTime.Duration;
            return upTime > 1f ? 1f: upTime;
        }
        private static float DurationCalculatedFromCutToTheChase(CalculationOptionsRogue calcOpts, CycleTime cycleTime) {
            var finishersThatRefreshSnD = new List<CycleComponent>();
            finishersThatRefreshSnD.AddRange(calcOpts.DpsCycle.FindAll(Evis.NAME));
            finishersThatRefreshSnD.AddRange(calcOpts.DpsCycle.FindAll(Envenom.NAME));

            if(finishersThatRefreshSnD.Count > 0 && Talents.CutToTheChase.Bonus == 1f) {
                var durationEachComponentHasToCover = cycleTime.Duration / finishersThatRefreshSnD.Count;
                if (durationEachComponentHasToCover < SndDuration(5)) {
                    return 1f;
                }
            }

            var baseProbOfRefreshingSnD = SndDuration(5) / cycleTime.Duration;
            var probOfRefreshingSnd = 0f;

            foreach(var finisher in finishersThatRefreshSnD) {
                probOfRefreshingSnd += (baseProbOfRefreshingSnD - probOfRefreshingSnd) * Talents.CutToTheChase.Bonus;
            }

            return probOfRefreshingSnd;
        }

        private static float SndDuration(CalculationOptionsRogue calcOpts) {
            return SndDuration(calcOpts.DpsCycle.Find(NAME).Rank);
        }

        private static float SndDuration(int rank) {
            return (6f + (Glyphs.GlyphOfSliceandDice ? 3f : 0f) + (rank * 3f)) * Talents.ImprovedSliceAndDice.Multiplier; 
        }

        public static float CalcHasteBonus(CalculationOptionsRogue calcOpts, CycleTime cycleTime) {
            return BASE_HASTE_BONUS * UpTime(calcOpts, cycleTime);
        }*/
    }
}