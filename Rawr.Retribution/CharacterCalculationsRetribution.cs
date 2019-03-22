using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Retribution
{
    public class CharacterCalculationsRetribution : CharacterCalculationsBase
    {
        private float _overallPoints = 0f;
        public override float OverallPoints { get { return _overallPoints; } set { _overallPoints = value; } }
        private float[] _subPoints = new float[] { 0f };
        public override float[] SubPoints { get { return _subPoints; } set { _subPoints = value; } }
        public float DPSPoints { get { return _subPoints[0]; } set { _subPoints[0] = value; } }

        public RotationSolution Solution { get; set; } // TODO: Remove dependancy, This should be obtained out of the base Rotation class, not the Sim specific solution.
        public Ability[] Rotation { get; set; }

        public float WhiteDPS { get; set; }
        public float SealDPS { get; set; }
        public float CrusaderStrikeDPS { get; set; }
        public float DivineStormDPS { get; set; }
        public float JudgementDPS { get; set; }
        public float ConsecrationDPS { get; set; }
        public float ExorcismDPS { get; set; }
        public float HandOfReckoningDPS { get; set; }
        public float HammerOfWrathDPS { get; set; }
        public float OtherDPS { get; set; }

        public Skill WhiteSkill { get; set; }
        public Skill SealSkill { get; set; }
        public Skill CrusaderStrikeSkill { get; set; }
        public Skill DivineStormSkill { get; set; }
        public Skill JudgementSkill { get; set; }
        public Skill ConsecrationSkill { get; set; }
        public Skill ExorcismSkill { get; set; }
        public Skill HandOfReckoningSkill { get; set; }
        public Skill HammerOfWrathSkill { get; set; }

        public float ToMiss { get; set; }
        public float ToBeDodged { get; set; }
        public float ToBeParried { get; set; }
        public float ToBeResisted { get; set; }

        public float AverageSoVStack { get; set; }
        public float SoVOvertake { get; set; }
        public float WeaponDamage { get; set; }
        public float AttackSpeed { get; set; }
        public Stats BasicStats { get; set; }

        // Add calculated values to the values dictionary.
        // These values are then available for display via the CharacterDisplayCalculationLabels
        // member defined in CalculationsRetribution.cs
        // While possible, there's little reason to add values to the dictionary that are not being
        // used by the CharacterDisplayCalculationLabels.
        public override Dictionary<string, string> GetCharacterDisplayCalculationValues()
        {
            Dictionary<string, string> dictValues = new Dictionary<string, string>();

            // Status text
            dictValues["Status"] = string.Format("{0} dps", DPSPoints.ToString("N0"));

            // Basic stats
            dictValues["Health"] = BasicStats.Health.ToString("N0");
            dictValues["Mana"] = BasicStats.Mana.ToString("N0");
            dictValues["Strength"] = BasicStats.Strength.ToString("N0");
            dictValues["Agility"] = string.Format("{0:0}", BasicStats.Agility);
            dictValues["Attack Power"] = BasicStats.AttackPower.ToString("N0");
            dictValues["Crit Chance"] = string.Format("{0:P}*{1:0} crit rating", BasicStats.PhysicalCrit, BasicStats.CritRating);
            dictValues["Miss Chance"] = string.Format("{0:P}*{1:P} hit ({2:0} rating)\n", ToMiss, BasicStats.PhysicalHit, BasicStats.HitRating);
            dictValues["Dodge Chance"] = string.Format("{0:P}*{1:P} expertise ({2:0} rating)", ToBeDodged, BasicStats.Expertise * .0025f, BasicStats.ExpertiseRating);
            dictValues["Melee Haste"] = string.Format("{0:P}*{1:0} haste rating", BasicStats.PhysicalHaste, BasicStats.HasteRating);
            dictValues["Weapon Damage"] = WeaponDamage.ToString("N2");
            dictValues["Attack Speed"] = AttackSpeed.ToString("N2");

            // DPS Breakdown
            dictValues["Total DPS"] = OverallPoints.ToString("N0");
            dictValues["White"] = string.Format("{0}*{1}", WhiteDPS.ToString("N0"), WhiteSkill.ToString());
            dictValues["Seal"] = string.Format("{0}*{1}", SealDPS.ToString("N0"), SealSkill.ToString());
            dictValues["Crusader Strike"] = string.Format("{0}*{1}", CrusaderStrikeDPS.ToString("N0"), CrusaderStrikeSkill.ToString());
            dictValues["Judgement"] = string.Format("{0}*{1}", JudgementDPS.ToString("N0"), JudgementSkill.ToString());
            dictValues["Consecration"] = string.Format("{0}*{1}", ConsecrationDPS.ToString("N0"), ConsecrationSkill.ToString());
            dictValues["Exorcism"] = string.Format("{0}*{1}", ExorcismDPS.ToString("N0"), ExorcismSkill.ToString());
            dictValues["Divine Storm"] = string.Format("{0}*{1}", DivineStormDPS.ToString("N0"), DivineStormSkill.ToString());
            dictValues["Hammer of Wrath"] = string.Format("{0}*{1}", HammerOfWrathDPS.ToString("N0"), HammerOfWrathSkill.ToString());
            dictValues["Hand of Reckoning"] = string.Format("{0}*{1}", HandOfReckoningDPS.ToString("N0"), HandOfReckoningSkill.ToString());
            dictValues["Other"] = OtherDPS.ToString("N0");

            // Rotation Info:
            dictValues["Chosen Rotation"] = Rotation == null ? 
                "n/a" :
                SimulatorParameters.ShortRotationString(Rotation);  // TODO: Remove dependancy on SimulatorParameters.
            dictValues["Average SoV Stack"] = AverageSoVStack.ToString("N2");
            dictValues["SoV Overtake"] = string.Format("{0} sec", SoVOvertake.ToString("N2"));
            dictValues["Crusader Strike CD"] = 
                Solution.GetAbilityEffectiveCooldown(Ability.CrusaderStrike).ToString("N2");
            dictValues["Judgement CD"] = 
                Solution.GetAbilityEffectiveCooldown(Ability.Judgement).ToString("N2");
            dictValues["Consecration CD"] = 
                Solution.GetAbilityEffectiveCooldown(Ability.Consecration).ToString("N2");
            dictValues["Exorcism CD"] = 
                Solution.GetAbilityEffectiveCooldown(Ability.Exorcism).ToString("N2");
            dictValues["Divine Storm CD"] = 
                Solution.GetAbilityEffectiveCooldown(Ability.DivineStorm).ToString("N2");
            dictValues["Hammer of Wrath CD"] = 
                Solution.GetAbilityEffectiveCooldown(Ability.HammerOfWrath).ToString("N2");

            return dictValues;
        }

        /// <summary>
        /// Obtain optimizable values.
        /// </summary>
        /// <param name="calculation"></param>
        /// <returns></returns>
        /// The list of labels listed here needs to match with the list in OptimizableCalculationLabels override in CalculationsRetribution.cs
        public override float GetOptimizableCalculationValue(string calculation)
        {
            switch (calculation)
            {
                case "Health": return BasicStats.Health;
                case "% Chance to Miss (Melee)": return ToMiss * 100f;  // White and Melee hit for ret are identical since we can't dual wield.
                case "% Chance to Miss (Spells)": return ToBeResisted * 100f;
                case "% Chance to be Dodged": return ToBeDodged * 100f;
                case "% Chance to be Parried": return ToBeParried * 100f;
                case "% Chance to be Avoided (Yellow/Dodge)" : return (ToMiss + ToBeDodged) * 100f;
            }
            return 0f;
        }
    }
}