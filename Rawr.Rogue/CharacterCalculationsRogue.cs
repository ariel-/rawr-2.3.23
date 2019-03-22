using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Rogue
{
    public class CharacterCalculationsRogue : CharacterCalculationsBase
    {
        private float _overallPoints = 0f;
        public override float OverallPoints
        {
            get { return _overallPoints; }
            set { _overallPoints = value; }
        }

        private float[] _subPoints = new float[] { 0f, 0f };
        public override float[] SubPoints
        {
            get { return _subPoints; }
            set { _subPoints = value; }
        }

        public float DPSPoints
        {
            get { return _subPoints[0]; }
            set { _subPoints[0] = value; }
        }

        public float SurvivabilityPoints
        {
            get { return _subPoints[1]; }
            set { _subPoints[1] = value; }
        }

        public Stats BasicStats { get; set; }
        public int TargetLevel { get; set; }

        public float AvoidedWhiteMHAttacks { get; set; }
        public float AvoidedWhiteOHAttacks { get; set; }
        public float AvoidedAttacks { get; set; }
        public float AvoidedFinisherAttacks { get; set; }
        public float AvoidedPoisonAttacks { get; set; }
        public float DodgedMHAttacks { get; set; }
        public float DodgedOHAttacks { get; set; }
        public float ParriedAttacks { get; set; }
        public float MissedWhiteAttacks { get; set; }
        public float MissedAttacks { get; set; }
        public float MissedPoisonAttacks { get; set; }
        public float CritChanceYellow { get; set; }
        public float CritChanceMHTotal { get; set; }
        public float CritChanceMH { get; set; }
        public float CritChanceOHTotal { get; set; }
        public float CritChanceOH { get; set; }
        public float MainHandSpeed { get; set; }
        public float OffHandSpeed { get; set; }
        public float ArmorMitigationMH { get; set; }
        public float ArmorMitigationOH { get; set; }
        public float Duration { get; set; }

        public RogueAbilityStats MainHandStats { get; set; }
        public RogueAbilityStats OffHandStats { get; set; }
        public RogueAbilityStats BackstabStats { get; set; }
        public RogueAbilityStats HemoStats { get; set; }
        public RogueAbilityStats SStrikeStats { get; set; }
        public RogueAbilityStats MutiStats { get; set; }
        public RogueAbilityStats RuptStats { get; set; }
        public RogueAbilityStats SnDStats { get; set; }
        public RogueAbilityStats EvisStats { get; set; }
        public RogueAbilityStats EnvenomStats { get; set; }
        public RogueAbilityStats IPStats { get; set; }
        public RogueAbilityStats DPStats { get; set; }
        public RogueAbilityStats WPStats { get; set; }
        public RogueAbilityStats APStats { get; set; }

        public RogueRotationCalculator.RogueRotationCalculation HighestDPSRotation { get; set; }
        public RogueRotationCalculator.RogueRotationCalculation CustomRotation { get; set; }

        public string Rotations { get; set; }

        public override Dictionary<string, string> GetCharacterDisplayCalculationValues()
        {
            Dictionary<string, string> dictValues = new Dictionary<string, string>();
            dictValues.Add("Overall Points", OverallPoints.ToString());
            dictValues.Add("DPS Points", DPSPoints.ToString());
            dictValues.Add("Survivability Points", SurvivabilityPoints.ToString());

            float baseMiss = StatConversion.WHITE_MISS_CHANCE_CAP_DW[TargetLevel - 80] - BasicStats.PhysicalHit;
            float baseYellowMiss = StatConversion.YELLOW_MISS_CHANCE_CAP[TargetLevel - 80] - BasicStats.PhysicalHit;
            float basePoisonMiss = StatConversion.GetSpellMiss(80 - TargetLevel, false) - BasicStats.SpellHit;
            float baseDodge = StatConversion.WHITE_DODGE_CHANCE_CAP[TargetLevel - 80] - StatConversion.GetDodgeParryReducFromExpertise(BasicStats.Expertise);
            float baseParry = 0f;// StatConversion.WHITE_PARRY_CHANCE_CAP[TargetLevel - 80] - StatConversion.GetDodgeParryReducFromExpertise(BasicStats.Expertise);
            float baseWhiteMHCrit = CritChanceMHTotal;
            float baseWhiteOHCrit = CritChanceOHTotal;
            float capMiss = (float)Math.Ceiling(baseMiss * 100f * 32.78998947f);
            float capYellowMiss = (float)Math.Ceiling(baseYellowMiss * 100f * 32.78998947f);
            float capPoisonMiss = (float)Math.Ceiling(basePoisonMiss * 100f * 26.23f);
            float capDodge = (float)Math.Ceiling(baseDodge * 100f * 32.78998947f);
            float capParry = (float)Math.Ceiling(baseParry * 100f * 32.78998947f); // TODO: Check this value
            float capWhiteMHCrit = 100 - StatConversion.WHITE_GLANCE_CHANCE_CAP[TargetLevel - 80] * 100 - MissedWhiteAttacks - DodgedMHAttacks;
            float capWhiteOHCrit = 100 - StatConversion.WHITE_GLANCE_CHANCE_CAP[TargetLevel - 80] * 100 - MissedWhiteAttacks - DodgedOHAttacks;

            string tipMiss = "*White: ";
            if (BasicStats.HitRating > capMiss)
                tipMiss += string.Format("Over the cap by {0} Hit Rating", BasicStats.HitRating - capMiss);
            else if (BasicStats.HitRating < capMiss)
                tipMiss += string.Format("Under the cap by {0} Hit Rating", capMiss - BasicStats.HitRating);
            else
                tipMiss += "Exactly at the cap";

            tipMiss += "\r\nYellow: ";
            if (BasicStats.HitRating > capYellowMiss)
                tipMiss += string.Format("Over the cap by {0} Hit Rating", BasicStats.HitRating - capYellowMiss);
            else if (BasicStats.HitRating < capYellowMiss)
                tipMiss += string.Format("Under the cap by {0} Hit Rating", capYellowMiss - BasicStats.HitRating);
            else
                tipMiss += "Exactly at the cap";

            tipMiss += "\r\nPoison: ";
            if (BasicStats.HitRating > capPoisonMiss)
                tipMiss += string.Format("Over the cap by {0} Hit Rating", BasicStats.HitRating - capPoisonMiss);
            else if (BasicStats.HitRating < capPoisonMiss)
                tipMiss += string.Format("Under the cap by {0} Hit Rating", capPoisonMiss - BasicStats.HitRating);
            else
                tipMiss += "Exactly at the cap";

            string tipDodge = string.Empty;
            if (BasicStats.ExpertiseRating > capDodge)
                tipDodge = string.Format("*Over the cap by {0} Expertise Rating", BasicStats.ExpertiseRating - capDodge);
            else if (BasicStats.ExpertiseRating < capDodge)
                tipDodge = string.Format("*Under the cap by {0} Expertise Rating", capDodge - BasicStats.ExpertiseRating);
            else
                tipDodge = "*Exactly at the cap";

            string tipCrit = string.Format("Mainhand: {0}, ", CritChanceMH);
            if (CritChanceMHTotal > capWhiteMHCrit)
                tipCrit += string.Format("over the Crit cap by {0}%", CritChanceMHTotal - capWhiteMHCrit);
            else if (CritChanceMHTotal < capWhiteMHCrit)
                tipCrit += string.Format("under the Crit cap by {0}%", capWhiteMHCrit - CritChanceMHTotal);
            else tipCrit += "exactly at the Crit cap";

            tipCrit += string.Format("\nOffhand: {0}, ", CritChanceOH);
            if (CritChanceOHTotal > capWhiteOHCrit)
                tipCrit += string.Format("over the Crit cap by {0}%", CritChanceOHTotal - capWhiteOHCrit);
            else if (CritChanceOHTotal < capWhiteOHCrit)
                tipCrit += string.Format("under the Crit cap by {0}%", capWhiteOHCrit - CritChanceOHTotal);
            else tipCrit += "exactly at the Crit cap";

            dictValues.Add("Health", BasicStats.Health.ToString());
            dictValues.Add("Attack Power", BasicStats.AttackPower.ToString());
            dictValues.Add("Agility", BasicStats.Agility.ToString());
            dictValues.Add("Strength", BasicStats.Strength.ToString());
            dictValues.Add("Crit Rating", BasicStats.CritRating.ToString());
            dictValues.Add("Hit Rating", BasicStats.HitRating.ToString() + tipMiss);
            dictValues.Add("Expertise Rating", BasicStats.ExpertiseRating.ToString() + tipDodge);
            dictValues.Add("Haste Rating", BasicStats.HasteRating.ToString());
            dictValues.Add("Armor Penetration Rating", BasicStats.ArmorPenetrationRating.ToString());
            dictValues.Add("Weapon Damage", "+" + BasicStats.WeaponDamage.ToString());

            dictValues.Add("Avoided White Attacks", string.Format("{0}% / {1}%*Mainhand: {2}% Dodged, {3}% Missed\n   Offhand: {4}% Dodged, {3}% Missed", AvoidedWhiteMHAttacks, AvoidedWhiteOHAttacks, DodgedMHAttacks, MissedWhiteAttacks, DodgedOHAttacks));
            dictValues.Add("Avoided Yellow Attacks", string.Format("{0}%*{1}% Dodged, {2}% Missed", AvoidedAttacks, DodgedMHAttacks, MissedAttacks));
            dictValues.Add("Avoided Poison Attacks", string.Format("{0}%*{1}% Missed", AvoidedPoisonAttacks, MissedPoisonAttacks));
            dictValues.Add("Crit Chance", CritChanceYellow.ToString() + "%*" + tipCrit);
            dictValues.Add("MainHand Speed", MainHandSpeed.ToString() + "s");
            dictValues.Add("OffHand Speed", OffHandSpeed.ToString() + "s");
            dictValues.Add("Armor Mitigation MainHand", ArmorMitigationMH.ToString() + "%");
            dictValues.Add("Armor Mitigation OffHand", ArmorMitigationOH.ToString() + "%");

            dictValues.Add("Optimal Rotation", HighestDPSRotation.ToString());
            dictValues.Add("Optimal Rotation DPS", HighestDPSRotation.DPS.ToString());
            dictValues.Add("Custom Rotation DPS", CustomRotation.DPS.ToString());


            float chanceWhiteMHNonAvoided = 1f - (AvoidedWhiteMHAttacks / 100f);
            float chanceWhiteOHNonAvoided = 1f - (AvoidedWhiteOHAttacks / 100f);
            float chanceNonAvoided = 1f - (AvoidedAttacks / 100f);
            float chancePoisonNonAvoided = 1f - (AvoidedPoisonAttacks / 100f);
            dictValues.Add("MainHand", MainHandStats.GetStatsTexts(HighestDPSRotation.MainHandCount, 0, HighestDPSRotation.TotalDamage, chanceWhiteMHNonAvoided, Duration));
            dictValues.Add("OffHand", OffHandStats.GetStatsTexts(HighestDPSRotation.OffHandCount, 0, HighestDPSRotation.TotalDamage, chanceWhiteOHNonAvoided, Duration));
            dictValues.Add("Backstab", BackstabStats.GetStatsTexts(HighestDPSRotation.BackstabCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Hemorrhage", HemoStats.GetStatsTexts(HighestDPSRotation.HemoCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Sinister Strike", SStrikeStats.GetStatsTexts(HighestDPSRotation.SStrikeCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Mutilate", MutiStats.GetStatsTexts(HighestDPSRotation.MutiCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Rupture", RuptStats.GetStatsTexts(HighestDPSRotation.RuptCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Slice and Dice", SnDStats.GetStatsTexts(HighestDPSRotation.SnDCount, HighestDPSRotation.SnDCP, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Eviscerate", EvisStats.GetStatsTexts(HighestDPSRotation.EvisCount, Math.Max(HighestDPSRotation.EvisCP, HighestDPSRotation.EnvenomCP), HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Envenom", EnvenomStats.GetStatsTexts(HighestDPSRotation.EnvenomCount, Math.Max(HighestDPSRotation.EvisCP, HighestDPSRotation.EnvenomCP), HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Instant Poison", IPStats.GetStatsTexts(HighestDPSRotation.IPCount, 0, HighestDPSRotation.TotalDamage, chancePoisonNonAvoided, Duration));
            dictValues.Add("Deadly Poison", DPStats.GetStatsTexts(HighestDPSRotation.DPCount, 0, HighestDPSRotation.TotalDamage, chancePoisonNonAvoided, Duration));
            dictValues.Add("Wound Poison", WPStats.GetStatsTexts(HighestDPSRotation.WPCount, 0, HighestDPSRotation.TotalDamage, chancePoisonNonAvoided, Duration));
            dictValues.Add("Anesthetic Poison", APStats.GetStatsTexts(HighestDPSRotation.APCount, 0, HighestDPSRotation.TotalDamage, chancePoisonNonAvoided, Duration));

            return dictValues;
        }

        public override float GetOptimizableCalculationValue(string calculation)
        {
            switch (calculation)
            {
                case "Avoided Yellow Attacks %": return AvoidedAttacks;
                case "Custom Rotation DPS": return CustomRotation.DPS;
            }
            return 0f;
        }
    }
}
