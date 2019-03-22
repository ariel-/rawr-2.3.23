using Rawr.Rogue.ClassAbilities;

namespace Rawr.Rogue.SpecialAbilities {
    public class SwordSpec {
        /*public float CalcDps(CalculationOptionsRogue calcOpts, CombatFactors combatFactors, WhiteAttacks whiteAttacks, CycleTime cycleTime) {
            float ssHits = 0f;
            if (combatFactors.MH.Type == ItemType.OneHandSword
                || combatFactors.MH.Type == ItemType.OneHandAxe)
            {
                //MH hits + CPG + finisher
                ssHits += whiteAttacks.MhHits * Talents.HackAndSlash.Bonus;
                ssHits += calcOpts.CpGenerator.MhHitsNeeded(combatFactors, calcOpts) * Talents.HackAndSlash.Bonus * combatFactors.ProbMhWhiteHit;
                ssHits += 1f / cycleTime.Duration * Talents.HackAndSlash.Bonus * combatFactors.ProbMhWhiteHit;
            }
            if (combatFactors.OH.Type == ItemType.OneHandSword
                || combatFactors.OH.Type == ItemType.OneHandAxe)
            {
                ssHits += whiteAttacks.OhHits * Talents.HackAndSlash.Bonus;
            }

            float ssDPS = (ssHits * combatFactors.MhAvgDamage) * (1 - combatFactors.ProbMhCrit) + (ssHits * combatFactors.MhAvgDamage * combatFactors.BaseCritMultiplier) * combatFactors.ProbMhCrit;
            ssDPS *= combatFactors.MhDamageReduction;
            return ssDPS;
        }*/
    }
}