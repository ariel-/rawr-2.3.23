using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Cat
{
	public class CharacterCalculationsCat : CharacterCalculationsBase
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

		public float AvoidedAttacks { get; set; }
		public float DodgedAttacks { get; set; }
		public float ParriedAttacks { get; set; }
		public float MissedAttacks { get; set; }
		public float CritChance { get; set; }
		public float AttackSpeed { get; set; }
		public float ArmorMitigation { get; set; }
		public float Duration { get; set; }

		//public float MeleeDamagePerHit { get; set; }
		//public float MangleDamagePerHit { get; set; }
		//public float ShredDamagePerHit { get; set; }
		//public float RakeDamagePerHit { get; set; }
		//public float RipDamagePerHit { get; set; }
		//public float BiteDamagePerHit { get; set; }

		//public float MeleeDamagePerSwing { get; set; }
		//public float MangleDamagePerSwing { get; set; }
		//public float ShredDamagePerSwing { get; set; }
		//public float RakeDamagePerSwing { get; set; }
		//public float RipDamagePerSwing { get; set; }
		//public float BiteDamagePerSwing { get; set; }

		public CatAbilityStats MeleeStats { get; set; }
		public CatAbilityStats MangleStats { get; set; }
		public CatAbilityStats ShredStats { get; set; }
		public CatAbilityStats RakeStats { get; set; }
		public CatAbilityStats RipStats { get; set; }
		public CatAbilityStats RoarStats { get; set; }
		public CatAbilityStats BiteStats { get; set; }

		public CatRotationCalculator.CatRotationCalculation HighestDPSRotation { get; set; }
		public CatRotationCalculator.CatRotationCalculation CustomRotation { get; set; }

		public string Rotations { get; set; }

		public override Dictionary<string, string> GetCharacterDisplayCalculationValues()
		{
			Dictionary<string, string> dictValues = new Dictionary<string, string>();
			dictValues.Add("Overall Points", OverallPoints.ToString());
			dictValues.Add("DPS Points", DPSPoints.ToString());
			dictValues.Add("Survivability Points", SurvivabilityPoints.ToString());

			float baseMiss = StatConversion.WHITE_MISS_CHANCE_CAP[TargetLevel - 80] - BasicStats.PhysicalHit;
			float baseDodge = StatConversion.WHITE_DODGE_CHANCE_CAP[TargetLevel - 80] - StatConversion.GetDodgeParryReducFromExpertise(BasicStats.Expertise);
			float baseParry = 0f;// StatConversion.WHITE_PARRY_CHANCE_CAP[TargetLevel - 80] - StatConversion.GetDodgeParryReducFromExpertise(BasicStats.Expertise);
			float capMiss = (float)Math.Ceiling(baseMiss * 100f * 32.78998947f);
			float capDodge = (float)Math.Ceiling(baseDodge * 100f * 32.78998947f);
			float capParry = (float)Math.Ceiling(baseParry * 100f * 32.78998947f); // TODO: Check this value

			string tipMiss = string.Empty;
			if (BasicStats.HitRating > capMiss)
				tipMiss = string.Format("*Over the cap by {0} Hit Rating", BasicStats.HitRating - capMiss);
			else if (BasicStats.HitRating < capMiss)
				tipMiss = string.Format("*Under the cap by {0} Hit Rating", capMiss - BasicStats.HitRating);
			else
				tipMiss = "*Exactly at the cap";

			string tipDodge = string.Empty;
			if (BasicStats.ExpertiseRating > capDodge)
				tipDodge = string.Format("*Over the cap by {0} Expertise Rating", BasicStats.ExpertiseRating - capDodge);
			else if (BasicStats.ExpertiseRating < capDodge)
				tipDodge = string.Format("*Under the cap by {0} Expertise Rating", capDodge - BasicStats.ExpertiseRating);
			else
				tipDodge = "*Exactly at the cap";


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

			dictValues.Add("Avoided Attacks", string.Format("{0}%*{1}% Dodged, {2}% Missed", AvoidedAttacks, DodgedAttacks, MissedAttacks));
			dictValues.Add("Crit Chance", CritChance.ToString() + "%");
			dictValues.Add("Attack Speed", AttackSpeed.ToString() + "s");
			dictValues.Add("Armor Mitigation", ArmorMitigation.ToString() + "%");

			dictValues.Add("Optimal Rotation", HighestDPSRotation.ToString());
			dictValues.Add("Optimal Rotation DPS", HighestDPSRotation.DPS.ToString());
			dictValues.Add("Custom Rotation DPS", CustomRotation.DPS.ToString());


			float chanceNonAvoided = 1f - (AvoidedAttacks / 100f);
			dictValues.Add("Melee", MeleeStats.GetStatsTexts(HighestDPSRotation.MeleeCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
			dictValues.Add("Mangle", MangleStats.GetStatsTexts(HighestDPSRotation.MangleCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
			dictValues.Add("Shred", ShredStats.GetStatsTexts(HighestDPSRotation.ShredCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
			dictValues.Add("Rake", RakeStats.GetStatsTexts(HighestDPSRotation.RakeCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
			dictValues.Add("Rip", RipStats.GetStatsTexts(HighestDPSRotation.RipCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
			dictValues.Add("Roar", RoarStats.GetStatsTexts(HighestDPSRotation.RoarCount, HighestDPSRotation.RoarCP, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
			dictValues.Add("Bite", BiteStats.GetStatsTexts(HighestDPSRotation.BiteCount, HighestDPSRotation.BiteCP, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
			

			//string[] abilityStats = MeleeStats.GetStatsTexts(HighestDPSRotation.MeleeCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration);
			//dictValues.Add("Melee Usage", abilityStats[0]);
			//dictValues.Add("Melee Stats", abilityStats[1]);
			//abilityStats = MangleStats.GetStatsTexts(HighestDPSRotation.MangleCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration);
			//dictValues.Add("Mangle Usage", abilityStats[0]);
			//dictValues.Add("Mangle Stats", abilityStats[1]);
			//abilityStats = ShredStats.GetStatsTexts(HighestDPSRotation.ShredCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration);
			//dictValues.Add("Shred Usage", abilityStats[0]);
			//dictValues.Add("Shred Stats", abilityStats[1]);
			//abilityStats = RakeStats.GetStatsTexts(HighestDPSRotation.RakeCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration);
			//dictValues.Add("Rake Usage", abilityStats[0]);
			//dictValues.Add("Rake Stats", abilityStats[1]);
			//abilityStats = RipStats.GetStatsTexts(HighestDPSRotation.RipCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration);
			//dictValues.Add("Rip Usage", abilityStats[0]);
			//dictValues.Add("Rip Stats", abilityStats[1]);
			//abilityStats = RoarStats.GetStatsTexts(HighestDPSRotation.RoarCount, HighestDPSRotation.RoarCP, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration);
			//dictValues.Add("Roar Usage", abilityStats[0]);
			//dictValues.Add("Roar Stats", abilityStats[1]);
			//abilityStats = BiteStats.GetStatsTexts(HighestDPSRotation.BiteCount, HighestDPSRotation.BiteCP, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration);
			//dictValues.Add("Bite Usage", abilityStats[0]);
			//dictValues.Add("Bite Stats", abilityStats[1]);

			//string attackFormat = "{0}%*Damage Per Hit: {1}, Damage Per Swing: {2}\r\n{0}% of Total Damage, {3} Damage Done";
			//dictValues.Add("Melee Damage", string.Empty);//.Format(attackFormat, 100f * HighestDPSRotation.MeleeDamageTotal / HighestDPSRotation.DamageTotal, MeleeDamagePerHit, MeleeDamagePerSwing, HighestDPSRotation.MeleeDamageTotal));
			//dictValues.Add("Mangle Damage", string.Empty);//.Format(attackFormat, 100f * HighestDPSRotation.MangleDamageTotal / HighestDPSRotation.DamageTotal, MangleDamagePerHit, MangleDamagePerSwing, HighestDPSRotation.MangleDamageTotal));
			//dictValues.Add("Shred Damage", string.Empty);//.Format(attackFormat, 100f * HighestDPSRotation.ShredDamageTotal / HighestDPSRotation.DamageTotal, ShredDamagePerHit, ShredDamagePerSwing, HighestDPSRotation.ShredDamageTotal));
			//dictValues.Add("Rake Damage", string.Empty);//.Format(attackFormat, 100f * HighestDPSRotation.RakeDamageTotal / HighestDPSRotation.DamageTotal, RakeDamagePerHit, RakeDamagePerSwing, HighestDPSRotation.RakeDamageTotal));
			//dictValues.Add("Rip Damage", string.Empty);//.Format(attackFormat, 100f * HighestDPSRotation.RipDamageTotal / HighestDPSRotation.DamageTotal, RipDamagePerHit, RipDamagePerSwing, HighestDPSRotation.RipDamageTotal));
			//dictValues.Add("Bite Damage", string.Empty);//.Format(attackFormat, 100f * HighestDPSRotation.BiteDamageTotal / HighestDPSRotation.DamageTotal, BiteDamagePerHit, BiteDamagePerSwing, HighestDPSRotation.BiteDamageTotal));

			//string rotationDescription = string.Empty;
			//try
			//{
			//    rotationDescription = string.Format("{0}*Keep {1}cp Savage Roar up.\r\n{2}{3}{4}{5}Use {6} for combo points.",
			//        HighestDPSRotation.Name.Replace(" + ", "+"), HighestDPSRotation.RoarCP,
			//        HighestDPSRotation.Name.Contains("Rake") ? "Keep Rake up.\r\n" : "",
			//        HighestDPSRotation.Name.Contains("Rip") ? "Keep 5cp Rip up.\r\n" : "",
			//        HighestDPSRotation.Name.Contains("Mangle") ? "Keep Mangle up.\r\n" : "",
			//        HighestDPSRotation.Name.Contains("Bite") ? string.Format("Use {0}cp Ferocious Bites to spend extra combo points.\r\n", HighestDPSRotation.BiteCP) : "",
			//        HighestDPSRotation.Name.Contains("Shred") ? "Shred" : "Mangle");
			//}
			//catch (Exception ex)
			//{
			//    ex.ToString();
			//}

			
			return dictValues;
		}

		public override float GetOptimizableCalculationValue(string calculation)
		{
			switch (calculation)
			{
				case "Health": return BasicStats.Health;
				case "Avoided Attacks %": return AvoidedAttacks;
				case "Nature Resist": return BasicStats.NatureResistance;
				case "Fire Resist": return BasicStats.FireResistance;
				case "Frost Resist": return BasicStats.FrostResistance;
				case "Shadow Resist": return BasicStats.ShadowResistance;
				case "Arcane Resist": return BasicStats.ArcaneResistance;
				case "Custom Rotation DPS": return CustomRotation.DPS;
			}
			return 0f;
		}
	}
}
