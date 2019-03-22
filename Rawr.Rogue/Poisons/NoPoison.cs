using System;

namespace Rawr.Rogue.Poisons
{
#if (SILVERLIGHT == false)
    [Serializable]
#endif
    public class NoPoison : PoisonBase
    {
        /*public override string Name { get { return "None"; } }

        public override bool IsDeadlyPoison { get { return false; } }

        public override float CalcPoisonDps( Stats stats, CalculationOptionsRogue calcOpts, CombatFactors combatFactors, float hits, CycleTime cycleTime, Item weapon )
        {
            return 0f;
        }*/
    }
}