using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Warlock {

    public delegate float StatExtractor(Stats stats);

    public delegate WeightedStat[] UptimeCombiner(
        SpecialEffect[] effects,
        float[] triggerInterval,
        float[] triggerChance,
        float[] offset, float[] scale,
        float attackSpeed,
        float fightDuration,
        float[] value);

    public class StatUtils {

        public static float GetBuffEffect(
            List<Buff> activeBuffs,
            float candidateBuff,
            string group,
            StatExtractor extractor) {

            float active = GetActiveBuff(activeBuffs, group, extractor);
            return Math.Max(0f, candidateBuff - active);
        }

        public static float GetActiveBuff(
            List<Buff> activeBuffs,
            string group,
            StatExtractor extractor) {

            float active = 0f;
            foreach (Buff buff in activeBuffs) {
                if (buff.ConflictingBuffs.Contains(group)) {
                    active += extractor(buff.Stats);
                    foreach (Buff improvement in buff.Improvements) {
                        if (activeBuffs.Contains(improvement)) {
                            active += extractor(improvement.Stats);
                        }
                    }
                }
            }
            return active;
        }

        public static float CalcStamina(Stats stats) {

            return stats.Stamina * (1f + stats.BonusStaminaMultiplier);
        }

        public static float CalcIntellect(Stats stats) {

            return stats.Intellect * (1f + stats.BonusIntellectMultiplier);
        }

        public static float CalcSpirit(Stats stats) {

            return stats.Spirit * (1f + stats.BonusSpiritMultiplier);
        }

        public static float CalcHealth(Stats stats) {

            return (stats.Health
                    + StatConversion.GetHealthFromStamina(CalcStamina(stats)))
                * (1 + stats.BonusHealthMultiplier);
        }

        public static float CalcMana(Stats stats) {

            return (1 + stats.BonusManaMultiplier)
                * (stats.Mana
                    + StatConversion.GetManaFromIntellect(
                        CalcIntellect(stats)));
        }

        public static float CalcUsableMana(Stats stats, float fightLen) {

            float mps
                = stats.Mp5 / 5f
                    + stats.Mana * stats.ManaRestoreFromMaxManaPerSecond;
            return CalcMana(stats) + stats.ManaRestore + mps * fightLen;
        }

        public static float CalcSpellCrit(Stats stats) {

            return stats.SpellCrit
                + StatConversion.GetSpellCritFromIntellect(CalcIntellect(stats))
                + StatConversion.GetSpellCritFromRating(stats.CritRating)
                + stats.BonusCritChance
                + stats.SpellCritOnTarget;
        }

        public static float CalcSpellHit(Stats stats) {

            return stats.SpellHit
                + StatConversion.GetSpellHitFromRating(stats.HitRating);
        }

        public static float CalcSpellPower(Stats stats) {

            return (stats.SpellPower
                    + stats.SpellDamageFromSpiritPercentage * CalcSpirit(stats))
                * (1f + stats.BonusSpellPowerMultiplier);
        }

        public static float CalcSpellHaste(Stats stats) {

            return (1f + stats.SpellHaste)
                * (1f
                    + StatConversion.GetSpellHasteFromRating(
                        stats.HasteRating));
        }

        public static float CalcStrength(Stats stats) {

            return stats.Strength * (1 + stats.BonusStrengthMultiplier);
        }

        public static float CalcAgility(Stats stats) {

            return stats.Agility * (1 + stats.BonusAgilityMultiplier);
        }

        public static float CalcAttackPower(
            Stats stats, float apPerStrength, float apPerAgility) {

            return (1 + stats.BonusAttackPowerMultiplier)
                * (stats.AttackPower
                    + CalcStrength(stats) * apPerStrength
                    + CalcAgility(stats) * apPerAgility);
        }

        public static float CalcPhysicalCrit(
            Stats stats, float critPerAgility, int levelDelta) {

            return stats.PhysicalCrit
                + CalcAgility(stats) * critPerAgility
                + StatConversion.NPC_LEVEL_CRIT_MOD[levelDelta];
        }
    }

    public class SpellModifiers {

        public float AdditiveMultiplier { get; private set; }
        public float AdditiveDirectMultiplier { get; private set; }
        public float AdditiveTickMultiplier { get; private set; }
        public float MultiplicativeMultiplier { get; private set; }
        public float MultiplicativeDirectMultiplier { get; private set; }
        public float MultiplicativeTickMultiplier { get; private set; }
        public float CritChance { get; private set; }
        public float CritOverallMultiplier { get; private set; }
        public float CritBonusMultiplier { get; private set; }

        public SpellModifiers() { }

        public void Accumulate(SpellModifiers other) {

            AddAdditiveMultiplier(other.AdditiveMultiplier);
            AddAdditiveDirectMultiplier(other.AdditiveDirectMultiplier);
            AddAdditiveTickMultiplier(other.AdditiveTickMultiplier);
            AddMultiplicativeMultiplier(other.MultiplicativeMultiplier);
            AddMultiplicativeDirectMultiplier(
                other.MultiplicativeDirectMultiplier);
            AddMultiplicativeTickMultiplier(
                other.MultiplicativeTickMultiplier);
            AddCritChance(other.CritChance);
            AddCritOverallMultiplier(other.CritOverallMultiplier);
            AddCritBonusMultiplier(other.CritBonusMultiplier);
        }

        public float GetFinalDirectMultiplier() {

            return (1 + AdditiveMultiplier + AdditiveDirectMultiplier)
                * (1 + MultiplicativeMultiplier)
                * (1 + MultiplicativeDirectMultiplier);
        }

        public float GetFinalTickMultiplier() {

            return (1 + AdditiveMultiplier + AdditiveTickMultiplier)
                * (1 + MultiplicativeMultiplier)
                * (1 + MultiplicativeTickMultiplier);
        }

        public float GetFinalCritMultiplier() {

            float bonus = 1.5f * (1 + CritOverallMultiplier) - 1;
            return 1f + bonus * (1 + CritBonusMultiplier);
        }

        public void AddAdditiveMultiplier(float val) {

            AdditiveMultiplier += val;
        }

        public void AddAdditiveDirectMultiplier(float val) {

            AdditiveDirectMultiplier += val;
        }

        public void AddAdditiveTickMultiplier(float val) {

            AdditiveTickMultiplier += val;
        }

        public void AddMultiplicativeMultiplier(float val) {

            MultiplicativeMultiplier
                = (1 + MultiplicativeMultiplier) * (1 + val) - 1;
        }

        public void AddMultiplicativeDirectMultiplier(float val) {

            MultiplicativeDirectMultiplier
                = (1 + MultiplicativeDirectMultiplier) * (1 + val) - 1;
        }

        public void AddMultiplicativeTickMultiplier(float val) {

            MultiplicativeTickMultiplier
                = (1 + MultiplicativeTickMultiplier) * (1 + val) - 1;
        }

        public void AddCritChance(float val) {

            CritChance = Math.Min(1, CritChance + val);
        }

        public void AddCritOverallMultiplier(float val) {

            CritOverallMultiplier = (1 + CritOverallMultiplier) * (1 + val) - 1;
        }

        public void AddCritBonusMultiplier(float val) {

            CritBonusMultiplier = (1 + CritBonusMultiplier) * (1 + val) - 1;
        }
    }
}
