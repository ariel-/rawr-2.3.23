using System.Diagnostics;
using Rawr.Rogue.ClassAbilities;

namespace Rawr.Rogue {
    public class WhiteAttacks {
        public WhiteAttacks(Character character, Stats stats,CombatFactors combatFactors) {
            _stats = stats;
            _calcOpts = character.CalculationOptions as CalculationOptionsRogue;
            _combatFactors = combatFactors;
        }
        
        private readonly CombatFactors _combatFactors;
        private readonly Stats _stats;
        private readonly CalculationOptionsRogue _calcOpts;

        public float MhHits { get { return MhSwingsPerSecond * (_combatFactors.ProbMhWhiteHit + _combatFactors.ProbMhCrit + _combatFactors.ProbGlancingHit); } }
        public float OhHits { get { return OhSwingsPerSecond * (_combatFactors.ProbOhWhiteHit + _combatFactors.ProbOhCrit + _combatFactors.ProbGlancingHit); } }
        public float MhCrits { get { return MhSwingsPerSecond * _combatFactors.ProbMhCrit; } }
        public float OhCrits { get { return OhSwingsPerSecond * _combatFactors.ProbOhCrit; } }
        public float MhSwingsPerSecond { get { return _combatFactors.MH.Speed == 0 ? 0 : (1f / _combatFactors.MHSpeed); } }
        public float OhSwingsPerSecond { get { return _combatFactors.OH.Speed == 0 ? 0 : (1f / _combatFactors.OHSpeed); } }
        public float MhHitDamage { get { return CalcBaseDamage(_combatFactors.MhAvgDamage * _combatFactors.MhDamageReduction, _calcOpts, _stats); } }
        public float OhHitDamage { get { return CalcBaseDamage(_combatFactors.OhAvgDamage * _combatFactors.OhDamageReduction, _calcOpts, _stats); } }

        public float CalcMhWhiteDps()
        {
            float baseDamage = MhHitDamage;
            float avgDamage = baseDamage * _combatFactors.GlanceReduction * _combatFactors.ProbGlancingHit +
                              baseDamage * _combatFactors.ProbMhWhiteHit +
                              baseDamage * _combatFactors.BaseCritMultiplier * _combatFactors.ProbMhCrit;

            return avgDamage * MhSwingsPerSecond;
        }
        public float CalcOhWhiteDps() {
            float baseDamage = OhHitDamage;
            float avgDamage = baseDamage * _combatFactors.GlanceReduction * _combatFactors.ProbGlancingHit +
                              baseDamage * _combatFactors.ProbOhWhiteHit +
                              baseDamage * _combatFactors.BaseCritMultiplier * _combatFactors.ProbOhCrit;

            return avgDamage * OhSwingsPerSecond;
       }

        private float CalcBaseDamage(float avgDamage, CalculationOptionsRogue calcOpts, Stats stats)
        {
            float baseDamage = avgDamage;
            baseDamage *= (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier);
            baseDamage *= (1f + Talents.HungerForBlood.Damage.Bonus) * (1f + Talents.Murder.Bonus);

            return baseDamage;
        }
    }
}
