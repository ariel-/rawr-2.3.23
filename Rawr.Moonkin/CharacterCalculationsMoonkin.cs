using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Moonkin
{
    public class CharacterCalculationsMoonkin : CharacterCalculationsBase
    {
        private float overallPoints = 0f;
        public override float OverallPoints { get { return overallPoints; } set { overallPoints = value; } }

        private float[] subPoints = new float[] { 0f, 0f };

        public override float[] SubPoints { get { return subPoints; } set { subPoints = value; } }

        public float SpellHit { get; set; }
        public float SpellCrit { get; set; }
        public float SpellHaste { get; set; }
        public float SpellPower { get; set; }
        public float ManaRegen { get; set; }
        public float ManaRegen5SR { get; set; }
        public float Latency { get; set; }
        public int TargetLevel { get; set; }
        public float FightLength { get; set; }
        public bool Scryer { get; set; }
        public SpellRotation SelectedRotation { get; set; }
        public SpellRotation BurstDPSRotation { get; set; }
        public string RotationName { get { return SelectedRotation.Name; } }
        public string DpsRotationName { get { return BurstDPSRotation.Name; } }
        public Dictionary<string, RotationData> Rotations { get; set; }
        private Stats baseStats;
        public Stats BasicStats { get { return baseStats; } set { baseStats = value; } }
        public float StarfallMana { get; set; }
        public float TreantDamage { get; set; }

        public override Dictionary<string, string> GetCharacterDisplayCalculationValues() {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            retVal.Add("Health", baseStats.Health.ToString());
            retVal.Add("Mana", baseStats.Mana.ToString());
            retVal.Add("Armor", baseStats.Armor.ToString());
            retVal.Add("Agility", baseStats.Agility.ToString());
            retVal.Add("Stamina", baseStats.Stamina.ToString());
            retVal.Add("Intellect", baseStats.Intellect.ToString());
            retVal.Add("Spirit", baseStats.Spirit.ToString());
            retVal.Add("Spell Power", SpellPower.ToString());
            retVal.Add("Spell Hit", String.Format("{0:F}%*{1} Hit Rating, {2:F}% Hit From Gear, {3} Rating To Cap",
                100 * SpellHit,
                baseStats.HitRating,
                100 * StatConversion.GetSpellHitFromRating(baseStats.HitRating),
                StatConversion.GetRatingFromHit(Math.Max(0, 0.17f - SpellHit))));
            retVal.Add("Spell Crit", String.Format("{0:F}%*{1} Crit Rating, {2:F}% Crit From Gear, {3:F}% Crit From Intellect",
                100 * SpellCrit,
                baseStats.CritRating,
                100 * StatConversion.GetSpellCritFromRating(baseStats.CritRating),
                100 * StatConversion.GetSpellCritFromIntellect(baseStats.Intellect)));
            retVal.Add("Spell Haste", String.Format("{0:F}%*{1} Haste Rating, {2:F}% Haste From Gear",
                100 * SpellHaste,
                baseStats.HasteRating,
                100 * StatConversion.GetSpellHasteFromRating(baseStats.HasteRating)));
            retVal.Add("MP5 Not Casting", String.Format("{0:F0}", ManaRegen * 5.0f));
            retVal.Add("MP5 While Casting", String.Format("{0:F0}", ManaRegen5SR * 5.0f));
            retVal.Add("Total Score", String.Format("{0:F2}", SubPoints[0]+SubPoints[1]));
            retVal.Add("Selected Rotation", SelectedRotation.Name);
            retVal.Add("Selected DPS", String.Format("{0:F2}", SelectedRotation.RotationData.DPS));
            retVal.Add("Selected Time To OOM", String.Format(SelectedRotation.RotationData.TimeToOOM > new TimeSpan(0, 0, 0) ? "{0} m {1} s" : "Not during fight", SelectedRotation.RotationData.TimeToOOM.Minutes, SelectedRotation.RotationData.TimeToOOM.Seconds));
            retVal.Add("Selected Cycle Length", String.Format("{0:F1} s", SelectedRotation.Duration));

            StringBuilder sb = new StringBuilder("*");
            float rotationDamage = SelectedRotation.RotationData.DPS * SelectedRotation.Duration;
            sb.AppendLine(String.Format("{0}: {1:F2}%, {2:F2} damage, {3:F0} count", "Starfire", 100 * SelectedRotation.StarfireAvgHit * SelectedRotation.StarfireCount / rotationDamage,
                SelectedRotation.StarfireAvgHit * SelectedRotation.StarfireCount, 
                SelectedRotation.StarfireCount));
            sb.AppendLine(String.Format("{0}: {1:F2}%, {2:F2} damage, {3:F0} count", "Moonfire", 100 * (SelectedRotation.MoonfireAvgHit) * SelectedRotation.MoonfireCasts / rotationDamage,
                (SelectedRotation.MoonfireAvgHit) * SelectedRotation.MoonfireCasts, 
                SelectedRotation.MoonfireCasts));
            sb.AppendLine(String.Format("{0}: {1:F2}%, {2:F2} damage, {3:F0} count", "Insect Swarm", 100 * SelectedRotation.InsectSwarmAvgHit * SelectedRotation.InsectSwarmCasts / rotationDamage,
                SelectedRotation.InsectSwarmAvgHit * (SelectedRotation.InsectSwarmCasts),
                SelectedRotation.InsectSwarmCasts));
            sb.AppendLine(String.Format("{0}: {1:F2}%, {2:F2} damage, {3:F0} count", "Wrath", 100 * SelectedRotation.WrathAvgHit * SelectedRotation.WrathCount / rotationDamage,
                SelectedRotation.WrathAvgHit * SelectedRotation.WrathCount, 
                SelectedRotation.WrathCount));

            retVal.Add("Selected Spell Breakdown", sb.ToString());
            retVal.Add("Burst Rotation", BurstDPSRotation.Name);
            retVal.Add("Burst DPS", String.Format("{0:F2}", BurstDPSRotation.RotationData.BurstDPS));
            retVal.Add("Burst Time To OOM", String.Format(BurstDPSRotation.RotationData.TimeToOOM > new TimeSpan(0, 0, 0) ? "{0} m {1} s" : "Not during fight", BurstDPSRotation.RotationData.TimeToOOM.Minutes, BurstDPSRotation.RotationData.TimeToOOM.Seconds));
            retVal.Add("Burst Cycle Length", String.Format("{0:F1} s", BurstDPSRotation.Duration));

            sb = new StringBuilder("*");
            rotationDamage = BurstDPSRotation.RotationData.DPS * BurstDPSRotation.Duration;
            sb.AppendLine(String.Format("{0}: {1:F2}%, {2:F2} damage, {3:F0} count", "Starfire", 100 * BurstDPSRotation.StarfireAvgHit * BurstDPSRotation.StarfireCount / rotationDamage,
                BurstDPSRotation.StarfireAvgHit * BurstDPSRotation.StarfireCount,
                BurstDPSRotation.StarfireCount));
            sb.AppendLine(String.Format("{0}: {1:F2}%, {2:F2} damage, {3:F0} count", "Moonfire", 100 * BurstDPSRotation.MoonfireAvgHit * SelectedRotation.MoonfireCasts / rotationDamage,
                (BurstDPSRotation.MoonfireAvgHit) * BurstDPSRotation.MoonfireCasts,
                BurstDPSRotation.MoonfireCasts));
            sb.AppendLine(String.Format("{0}: {1:F2}%, {2:F2} damage, {3:F0} count", "Insect Swarm", 100 * BurstDPSRotation.InsectSwarmAvgHit * BurstDPSRotation.InsectSwarmCasts / rotationDamage,
                BurstDPSRotation.InsectSwarmAvgHit * BurstDPSRotation.InsectSwarmCasts,
                BurstDPSRotation.InsectSwarmCasts));
            sb.AppendLine(String.Format("{0}: {1:F2}%, {2:F2} damage, {3:F0} count", "Wrath", 100 * BurstDPSRotation.WrathAvgHit * BurstDPSRotation.WrathCount / rotationDamage,
                BurstDPSRotation.WrathAvgHit * BurstDPSRotation.WrathCount, 
                BurstDPSRotation.WrathCount));

            retVal.Add("Burst Spell Breakdown", sb.ToString());
            retVal.Add("Starfire", String.Format("{0:F2} dps*{1:F2} s avg\n{2:F2} s w/NG\n{3:F2}% max non-Eclipse\n{4:F2}% max Eclipse\n{5:F2} avg hit\n{6:F0} avg mana",
                SelectedRotation.StarfireAvgHit / (SelectedRotation.StarfireAvgCast > 0 ? SelectedRotation.StarfireAvgCast : 1f),
                SelectedRotation.StarfireAvgCast,
                SelectedRotation.StarfireNGCastTime,
                100 * SelectedRotation.StarfireNonEclipseCrit,
                100 * SelectedRotation.StarfireEclipseCrit,
                SelectedRotation.StarfireAvgHit,
                SelectedRotation.StarfireManaCost));
            retVal.Add("Wrath", String.Format("{0:F2} dps*{1:F2} s avg\n{2:F2} s w/NG\n{3:F2} avg hit\n{4:F0} avg mana",
                SelectedRotation.WrathAvgHit / (SelectedRotation.WrathAvgCast > 0 ? SelectedRotation.WrathAvgCast : 1f),
                SelectedRotation.WrathAvgCast,
                SelectedRotation.WrathNGCastTime,
                SelectedRotation.WrathAvgHit,
                SelectedRotation.WrathManaCost));
            retVal.Add("Moonfire", String.Format("{0:F2} dps*{1:F2} s avg\n{2:F2} avg hit\n{3:F0} avg mana",
                SelectedRotation.MoonfireAvgHit / (SelectedRotation.MoonfireDuration > 0 ? SelectedRotation.MoonfireDuration : 1f),
                SelectedRotation.MoonfireCastTime,
                (SelectedRotation.MoonfireAvgHit),
                SelectedRotation.MoonfireManaCost));
            retVal.Add("Insect Swarm", String.Format("{0:F2} dps*{1:F2} s avg\n{2:F2} avg hit\n{3:F0} avg mana",
                SelectedRotation.InsectSwarmAvgHit / (SelectedRotation.InsectSwarmDuration > 0 ? SelectedRotation.InsectSwarmDuration : 1f),
                SelectedRotation.InsectSwarmCastTime,
                SelectedRotation.InsectSwarmAvgHit,
                SelectedRotation.InsectSwarmManaCost));
            retVal.Add("Starfall", String.Format("{0:F2} dps*{1:F2} avg per cast\n{2:F2} avg per star\n{3:F0} avg mana",
                SelectedRotation.StarfallDamage / 10.0f,
                SelectedRotation.StarfallDamage,
                SelectedRotation.StarfallDamage / (SelectedRotation.StarfallStars > 0 ? SelectedRotation.StarfallStars : 1f),
                StarfallMana));
            retVal.Add("Treants", String.Format("{0:F2} dps*{1:F2} avg per cast\n{2:F2} avg per tree",
                TreantDamage / 30.0f, TreantDamage, TreantDamage / 3.0f));

            return retVal;
        }

        public override float GetOptimizableCalculationValue(string calculation)
        {
            switch (calculation)
            {
                case "Hit Rating": return baseStats.HitRating;
                case "Haste Rating": return baseStats.HasteRating;
                case "Crit Rating": return baseStats.CritRating;
            }
            return 0;
        }
    }
}
