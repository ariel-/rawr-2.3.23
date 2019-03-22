using System;
using System.Xml.Serialization;

namespace Rawr.Rogue.Poisons
{
#if (SILVERLIGHT == false)
    [Serializable]
    [XmlInclude(typeof(NoPoison))]
    [XmlInclude(typeof(DeadlyPoison))]
    [XmlInclude(typeof(InstantPoison))]
    [XmlInclude(typeof(WoundPoison))]
#endif
    public abstract class PoisonBase
    {
        /*public abstract string Name { get; }
        public abstract bool IsDeadlyPoison { get; }
        public abstract float CalcPoisonDps(Stats stats, CalculationOptionsRogue calcOpts, CombatFactors combatFactors, float hits, CycleTime cycleTime, Item weapon);

        public static float CalcPoisonDps(CalculationOptionsRogue calcOpts, CombatFactors combatFactors, Stats stats, WhiteAttacks whiteAttacks, CharacterCalculationsRogue calculatedStats, CycleTime cycleTime)
        {
            if (calcOpts.TempMainHandEnchant.IsDeadlyPoison && calcOpts.TempOffHandEnchant.IsDeadlyPoison)
            {
                float mohPoisonHits = cycleTime.Duration * (whiteAttacks.MhSwingsPerSecond + whiteAttacks.OhSwingsPerSecond) + calcOpts.CpGenerator.MhHitsNeeded(combatFactors, calcOpts);
                mohPoisonHits *= (1f - combatFactors.PoisonMissChance);

                var mhPoisonDps = calcOpts.TempMainHandEnchant.CalcPoisonDps(stats,calcOpts,combatFactors,mohPoisonHits,cycleTime,combatFactors.MH);

                calculatedStats.AddToolTip(DisplayValue.PoisonDps, "Deadly Poison DPS: " + Math.Round(mhPoisonDps, 2));

                return mhPoisonDps;
            }
            else
            {
                float mhPoisonHits = cycleTime.Duration * whiteAttacks.MhSwingsPerSecond + calcOpts.CpGenerator.MhHitsNeeded(combatFactors,calcOpts);
                mhPoisonHits *= (1f - combatFactors.PoisonMissChance);

                var mhPoisonDps = calcOpts.TempMainHandEnchant.CalcPoisonDps(stats,calcOpts,combatFactors,mhPoisonHits,cycleTime,combatFactors.MH);
                
                float ohPoisonHits = cycleTime.Duration * whiteAttacks.OhSwingsPerSecond;
                ohPoisonHits *= (1f - combatFactors.PoisonMissChance);

                var ohPoisonDps = calcOpts.TempOffHandEnchant.CalcPoisonDps(stats,calcOpts,combatFactors,ohPoisonHits,cycleTime,combatFactors.OH);

                var totalPoisonDps = mhPoisonDps + ohPoisonDps;

                if (calcOpts.TempMainHandEnchant.Name == calcOpts.TempOffHandEnchant.Name)
                {
                    calculatedStats.AddToolTip(DisplayValue.PoisonDps, calcOpts.TempMainHandEnchant.Name + " DPS: " + Math.Round(totalPoisonDps, 2));
                }
                else
                {
                    calculatedStats.AddToolTip(DisplayValue.PoisonDps, calcOpts.TempMainHandEnchant.Name + " DPS: " + Math.Round(mhPoisonDps, 2));
                    calculatedStats.AddToolTip(DisplayValue.PoisonDps, calcOpts.TempOffHandEnchant.Name + " DPS: " + Math.Round(ohPoisonDps, 2));
                }

                return totalPoisonDps;
            }
        }*/
    }
}