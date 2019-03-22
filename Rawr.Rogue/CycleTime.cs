using Rawr.Rogue.ClassAbilities;

namespace Rawr.Rogue
{
    public class CycleTime
    {
        /*public CycleTime(CalculationOptionsRogue calcOpts, CombatFactors combatFactors, WhiteAttacks whiteAttacks)
        {
            _calcOpts = calcOpts;
            _combatFactors = combatFactors;
            _whiteAttacks = whiteAttacks;

            EnergyRegen = CalcEnergyRegen();
            Duration = CalcCycleTime();
        }

        private readonly CalculationOptionsRogue _calcOpts;
        private readonly CombatFactors _combatFactors;
        private readonly WhiteAttacks _whiteAttacks;

        public float Duration { get; set; }
        public float EnergyRegen { get; set; }

        private float CalcCycleTime()
        {
            var cpgDuration = _calcOpts.CpGenerator.CalcDuration(_calcOpts, EnergyRegen, _combatFactors);

            var finisherEnergyCost = 0f;
            foreach (var component in _calcOpts.DpsCycle.Components)
            {
                finisherEnergyCost += component.Finisher.EnergyCost(_combatFactors, component.Rank);
            }

            return cpgDuration + (finisherEnergyCost / EnergyRegen);
        }

        public float CalcEnergyRegen()
        {
            var energyRegen = _combatFactors.BaseEnergyRegen;
            energyRegen += Talents.CombatPotency.Bonus * _whiteAttacks.OhHits;
            energyRegen += Talents.FocusedAttacks.Bonus * (_whiteAttacks.MhCrits + _whiteAttacks.OhCrits);

            if (_calcOpts.TempMainHandEnchant.IsDeadlyPoison || _calcOpts.TempOffHandEnchant.IsDeadlyPoison)
            {
                energyRegen += _combatFactors.Tier8TwoPieceEnergyBonus;
            }

            if (_calcOpts.DpsCycle.Includes("Rupture"))
            {
                //  http://www.wowhead.com/?spell=67209#comments, 2% per 2sec.
                energyRegen += _combatFactors.T09x2ReduceEnergyCostFromRupture * .01f;
            }

            return energyRegen;
        }*/
    }
}