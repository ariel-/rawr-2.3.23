using System;
using System.Collections.Generic;

namespace Rawr.Healadin
{

    public class CharacterCalculationsHealadin : CharacterCalculationsBase
    {

        private float _overallPoints = 0f;
        public override float OverallPoints
        {
            get { return _overallPoints; }
            set { _overallPoints = value; }
        }

        private float[] _subPoints = new float[] { 0f , 0f };
        public override float[] SubPoints
        {
            get { return _subPoints; }
            set { _subPoints = value; }
        }

        public float FightPoints
        {
            get { return _subPoints[0]; }
            set { _subPoints[0] = value; }
        }

        public float BurstPoints
        {
            get { return _subPoints[1]; }
            set { _subPoints[1] = value; }
        }

        public float AvgHPS { get; set; }
        public float AvgHPM { get; set; }
        public float TotalHealed { get; set; }
        public float TotalMana { get; set; }

        public float FightLength { get; set; }

        public FlashOfLight FoL { get; set; }
        public HolyLight HL { get; set; }
        public HolyShock HS { get; set; }
        public SacredShield SS { get; set; }
        public BeaconOfLight BoL { get; set; }
        public JudgementsOfThePure JotP { get; set; }

        public float RotationFoL { get; set; }
        public float RotationHL { get; set; }
        public float RotationHS { get; set; }
        public float RotationJotP { get; set; }
        public float RotationBoL { get; set; }
        public float RotationSS { get; set; }

        public float HealedFoL { get; set; }
        public float HealedHL { get; set; }
        public float HealedHS { get; set; }
        public float HealedGHL { get; set; }
        public float HealedBoL { get; set; }
        public float HealedSS { get; set; }
        public float HealedOther { get; set; }

        public float UsageFoL { get; set; }
        public float UsageHL { get; set; }
        public float UsageHS { get; set; }
        public float UsageBoL { get; set; }
        public float UsageSS { get; set; }
        public float UsageJotP { get; set; }

        public float ManaBase { get; set; }
        public float ManaMp5 { get; set; }
        public float ManaReplenishment { get; set; }
        public float ManaArcaneTorrent { get; set; }
        public float ManaDivinePlea { get; set; }
        public float ManaLayOnHands { get; set; }
        public float ManaOther { get; set; }

        private Stats _basicStats;
        public Stats BasicStats
        {
            get { return _basicStats; }
            set { _basicStats = value; }
        }

        public override Dictionary<string, string> GetCharacterDisplayCalculationValues()
        {
            Dictionary<string, string> dictValues = new Dictionary<string, string>();
            dictValues.Add("Status", string.Format("Overall: {0,-10} Fight: {1,-10} Burst: {2,-10}",
                OverallPoints.ToString("N0"),
                FightPoints.ToString("N0"),
                BurstPoints.ToString("N0")));

            //Basic Stats
            dictValues["Health"] = BasicStats.Health.ToString("N00");
            dictValues["Stamina"] = BasicStats.Stamina.ToString("N00");
            dictValues["Mana"] = BasicStats.Mana.ToString("N00");
            dictValues["Intellect"] = BasicStats.Intellect.ToString("N00");
            dictValues["Spell Power"] = BasicStats.SpellPower.ToString("N00");
            dictValues["Mp5"] = BasicStats.Mp5.ToString("N00");
            dictValues["Spell Crit"] = string.Format("{0}%*{1} Crit Rating", (BasicStats.SpellCrit * 100).ToString("N02"), BasicStats.CritRating);
            dictValues["Spell Haste"] = string.Format("{0}%*{1} Haste Rating", (BasicStats.SpellHaste * 100).ToString("N02"), BasicStats.HasteRating);

            // Cycle Stats
            dictValues["Total Healed"] = string.Format("{0} healing", TotalHealed.ToString("N00"));
            dictValues["Total Mana"] = string.Format("{0} mana", TotalMana.ToString("N00"));
            dictValues["Average Healing per sec"] = string.Format("{0} hps", AvgHPS.ToString("N00"));
            dictValues["Average Healing per mana"] = string.Format("{0} hpm", AvgHPM.ToString("N02"));

            // Rotation Info
            dictValues["Holy Light Time"] = string.Format("{0} sec", RotationHL.ToString("N1"));
            dictValues["Flash of Light Time"] = string.Format("{0} sec", RotationFoL.ToString("N1"));
            dictValues["Holy Shock Time"] = string.Format("{0} sec", RotationHS.ToString("N1"));
            dictValues["Sacred Shield Time"] = string.Format("{0} sec", RotationSS.ToString("N1"));
            dictValues["Beacon of Light Time"] = string.Format("{0} sec", RotationBoL.ToString("N1"));
            dictValues["Judgement Time"] = string.Format("{0} sec", RotationJotP.ToString("N1"));

            // Healing Breakdown
            dictValues["Holy Light Healed"] = string.Format("{0} healed", HealedHL.ToString("N00"));
            dictValues["Flash of Light Healed"] = string.Format("{0} healed", HealedFoL.ToString("N00"));
            dictValues["Holy Shock Healed"] = string.Format("{0} healed", HealedHS.ToString("N00"));
            dictValues["Sacred Shield Healed"] = string.Format("{0} healed", HealedSS.ToString("N00"));
            dictValues["Beacon of Light Healed"] = string.Format("{0} healed", HealedBoL.ToString("N00"));
            dictValues["Glyph of HL Healed"] = string.Format("{0} healed", HealedGHL.ToString("N00"));
            dictValues["Other Healed"] = string.Format("{0} healed", HealedOther.ToString("N00"));

            dictValues["Holy Light"] = "*" + HL.ToString();
            dictValues["Flash of Light"] = "*" + FoL.ToString();
            dictValues["Holy Shock"] = "*" + HS.ToString();
            dictValues["Sacred Shield"] = "*" + SS.ToString();

            return dictValues;
        }

        public override float GetOptimizableCalculationValue(string calculation)
		{
			switch (calculation)
			{
				case "Health": return BasicStats.Health;
				case "Holy Light Cast Time": return HL.CastTime();
                case "Holy Light HPS": return HL.HPS();
                case "Holy Light Time": return RotationHL;
				case "Flash of Light Cast Time": return FoL.CastTime();
                case "Flash of Light HPS": return FoL.HPS();
                case "Flash of Light Time": return RotationFoL;
			}
			return 0f;
		}
    }
}
