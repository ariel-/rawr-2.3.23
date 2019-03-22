using System;

namespace Rawr.Rogue.FinishingMoves
{
    #if (SILVERLIGHT == false)
    [Serializable]
    #endif
    public class NoFinisher : FinisherBase
    {
        /*public override char Id { get { return 'Z'; } }
        public override string Name { get { return "None"; } }
        public override float EnergyCost(CombatFactors combatFactors, int rank) { return 0f; }
        public override float CalcFinisherDPS( CalculationOptionsRogue calcOpts, Stats stats, CombatFactors combatFactors, int rank, CycleTime cycleTime, WhiteAttacks whiteAttacks, CharacterCalculationsRogue displayValues )
        {
            return 0f;
        }*/
    }
}