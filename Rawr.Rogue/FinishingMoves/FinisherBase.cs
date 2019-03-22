using System;
using System.Xml.Serialization;
using Rawr.Rogue.ClassAbilities;

namespace Rawr.Rogue.FinishingMoves
{
#if (SILVERLIGHT == false)
    [Serializable]
    [XmlInclude(typeof(NoFinisher))]
    [XmlInclude(typeof(SnD))]
    [XmlInclude(typeof(Rupture))]
    [XmlInclude(typeof(Evis))]
    [XmlInclude(typeof(Envenom))]
#endif
    public abstract class FinisherBase
    {
        /*public abstract char Id { get; }
        public abstract string Name { get; }
        public abstract float EnergyCost(CombatFactors combatFactors, int rank);
        public abstract float CalcFinisherDPS( CalculationOptionsRogue calcOpts, Stats stats, CombatFactors combatFactors, int rank, CycleTime cycleTime, WhiteAttacks whiteAttacks, CharacterCalculationsRogue displayValues );

        protected float CritBonusFromTurnTheTables(CalculationOptionsRogue calcOpts)
        {
            return calcOpts.TurnTheTablesUptime * Talents.TurnTheTables.Bonus;
        }*/
    }
}