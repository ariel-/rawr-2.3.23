using System;
using System.Collections.Generic;
#if RAWR3
using System.Windows.Media;
#endif
using System.Text;

namespace Rawr.Cat
{
	[Rawr.Calculations.RawrModelInfo("Cat", "Ability_Druid_CatForm", CharacterClass.Druid)]
	public class CalculationsCat : CalculationsBase
	{
		//my insides all turned to ash / so slow
		//and blew away as i collapsed / so cold
		public override List<GemmingTemplate> DefaultGemmingTemplates
		{
			get
			{
				// Relevant Gem IDs for Cats
				//Red
				int[] delicate  = { 39905, 39997, 40112, 42143 }; // Agi
				int[] fractured = { 39909, 40002, 40117, 42153 }; // ArP

				//Purple
				int[] shifting  = { 39935, 40023, 40130, 40130 }; // Agi/Sta
				int[] puissant  = { 39933, 40033, 40140, 40140 }; // ArP/Sta
				int[] forceful  = { 39978, 40091, 40169, 40169 }; // Haste/Sta

				//Blue

				//Green

				//Yellow

				//Orange
				int[] glinting = { 39953, 40044, 40148, 40148 }; // Agi/Hit
				int[] deadly   = { 39952, 40043, 40147, 40147 }; // Agi/Crit
				int[] deft     = { 39955, 40046, 40150, 40150 }; // Agi/Haste

				// Prismatic
				int[] nightmare = { 49110, 49110, 49110, 49110 };

				//Meta
				int relentless = 41398;

				List<GemmingTemplate> list = new List<GemmingTemplate>();
				for (int tier = 0; tier < 4; tier++)
				{
					list.AddRange(new GemmingTemplate[]
						{
							CreateCatGemmingTemplate(tier,	 fractured,	 fractured,	fractured,	fractured,	relentless), //Max ArP
							CreateCatGemmingTemplate(tier,	 fractured,	 deadly,	puissant,	fractured,	relentless), //ArP/Crit
							CreateCatGemmingTemplate(tier,	 fractured,	 deft,		puissant,	fractured,	relentless), //ArP/Haste
							CreateCatGemmingTemplate(tier,	 fractured,	 glinting,	puissant,	fractured,	relentless), //ArP/Hit
							CreateCatGemmingTemplate(tier,	 fractured,	 deadly,	nightmare,	fractured,	relentless), //ArP/Crit/Nightmare
							CreateCatGemmingTemplate(tier,	 fractured,	 deft,		nightmare,	fractured,	relentless), //ArP/Haste/Nightmare
							CreateCatGemmingTemplate(tier,	 fractured,	 glinting,	nightmare,	fractured,	relentless), //ArP/Hit/Nightmare
							CreateCatGemmingTemplate(tier,	 delicate,	 delicate,	delicate,	delicate,	relentless), //Max Agi
							CreateCatGemmingTemplate(tier,	 delicate,	 deadly,	shifting,	delicate,	relentless), //Agi/Crit
							CreateCatGemmingTemplate(tier,	 delicate,	 deft,		shifting,	delicate,	relentless), //Agi/Haste
							CreateCatGemmingTemplate(tier,	 delicate,	 glinting,	shifting,	delicate,	relentless), //Agi/Hit
							CreateCatGemmingTemplate(tier,	 delicate,	 deadly,	nightmare,	delicate,	relentless), //Agi/Crit/Nightmare
							CreateCatGemmingTemplate(tier,	 delicate,	 deft,		nightmare,	delicate,	relentless), //Agi/Haste/Nightmare
							CreateCatGemmingTemplate(tier,	 delicate,	 glinting,	nightmare,	delicate,	relentless), //Agi/Hit/Nightmare
						});
				}

				return list;
			}
		}

		private const int DEFAULT_GEMMING_TIER = 2;
		private GemmingTemplate CreateCatGemmingTemplate(int tier, int[] red, int[] yellow, int[] blue, int[] prismatic, int meta)
		{
			return new GemmingTemplate()
			{
				Model = "Cat",
				Group = (new string[] { "Uncommon", "Rare", "Epic", "Jeweler" })[tier],
				Enabled = (tier == DEFAULT_GEMMING_TIER),
				RedId = red[tier],
				YellowId = yellow[tier],
				BlueId = blue[tier],
				PrismaticId = prismatic[tier],
				MetaId = meta
			};
		}

#if RAWR3
		private ICalculationOptionsPanel _calculationOptionsPanel = null;
		public override ICalculationOptionsPanel CalculationOptionsPanel
#else
		private CalculationOptionsPanelBase _calculationOptionsPanel = null;
		public override CalculationOptionsPanelBase CalculationOptionsPanel
#endif
		{
			get
			{
				if (_calculationOptionsPanel == null)
				{
					_calculationOptionsPanel = new CalculationOptionsPanelCat();
				}
				return _calculationOptionsPanel;
			}
		}

		private string[] _characterDisplayCalculationLabels = null;
		public override string[] CharacterDisplayCalculationLabels
		{
			get
			{
				if (_characterDisplayCalculationLabels == null)
					_characterDisplayCalculationLabels = new string[] {
					"Summary:Overall Points*Sum of your DPS Points and Survivability Points",
					"Summary:DPS Points*DPS Points is your theoretical DPS.",
					"Summary:Survivability Points*One hundreth of your health.",

					"Basic Stats:Health",
					"Basic Stats:Attack Power",
					"Basic Stats:Agility",
					"Basic Stats:Strength",
					"Basic Stats:Crit Rating",
					"Basic Stats:Hit Rating",
					"Basic Stats:Expertise Rating",
					"Basic Stats:Haste Rating",
					"Basic Stats:Armor Penetration Rating",
					"Basic Stats:Weapon Damage",
					
					"Complex Stats:Avoided Attacks",
					"Complex Stats:Crit Chance",
					"Complex Stats:Attack Speed",
					"Complex Stats:Armor Mitigation",
					
					"Abilities:Optimal Rotation",
					"Abilities:Optimal Rotation DPS",
					"Abilities:Custom Rotation DPS",
					"Abilities:Melee",
					"Abilities:Mangle",
					"Abilities:Shred",
					"Abilities:Rake",
					"Abilities:Rip",
					"Abilities:Bite",
					//"Abilities:Melee Usage",
					//"Abilities:Melee Stats",
					//"Abilities:Mangle Usage",
					//"Abilities:Mangle Stats",
					//"Abilities:Shred Usage",
					//"Abilities:Shred Stats",
					//"Abilities:Rake Usage",
					//"Abilities:Rake Stats",
					//"Abilities:Rip Usage",
					//"Abilities:Rip Stats",
					//"Abilities:Bite Usage",
					//"Abilities:Bite Stats",
				};
				return _characterDisplayCalculationLabels;
			}
		}

		private string[] _optimizableCalculationLabels = null;
		public override string[] OptimizableCalculationLabels
		{
			get
			{
				if (_optimizableCalculationLabels == null)
					_optimizableCalculationLabels = new string[] {
					"Custom Rotation DPS",
					"Health",
					"Avoided Attacks %",
					"Nature Resist",
					"Fire Resist",
					"Frost Resist",
					"Shadow Resist",
					"Arcane Resist",
					};
				return _optimizableCalculationLabels;
			}
		}

		private string[] _customChartNames = null;
		public override string[] CustomChartNames
		{
			get
			{
				if (_customChartNames == null)
					_customChartNames = new string[] {
						//"Hit Test",
					};
				return _customChartNames;
			}
		}

#if RAWR3
		private Dictionary<string, Color> _subPointNameColors = null;
		public override Dictionary<string, Color> SubPointNameColors
		{
			get
			{
				if (_subPointNameColors == null)
				{
					_subPointNameColors = new Dictionary<string, Color>();
					_subPointNameColors.Add("DPS", Color.FromArgb(255, 160, 0, 224));
					_subPointNameColors.Add("Survivability", Color.FromArgb(255, 64, 128, 32));
				}
				return _subPointNameColors;
			}
		}
#else
		private Dictionary<string, System.Drawing.Color> _subPointNameColors = null;
		public override Dictionary<string, System.Drawing.Color> SubPointNameColors
		{
			get
			{
				if (_subPointNameColors == null)
				{
					_subPointNameColors = new Dictionary<string, System.Drawing.Color>();
					_subPointNameColors.Add("DPS", System.Drawing.Color.FromArgb(160, 0, 224));
					_subPointNameColors.Add("Survivability", System.Drawing.Color.FromArgb(64, 128, 32));
				}
				return _subPointNameColors;
			}
		}
#endif

		private List<ItemType> _relevantItemTypes = null;
		public override List<ItemType> RelevantItemTypes
		{
			get
			{
				if (_relevantItemTypes == null)
				{
					_relevantItemTypes = new List<ItemType>(new ItemType[]
					{
						ItemType.None,
						ItemType.Leather,
						ItemType.Idol,
						ItemType.Staff,
						ItemType.TwoHandMace,
						ItemType.Polearm
					});
				}
				return _relevantItemTypes;
			}
		}

		public override CharacterClass TargetClass { get { return CharacterClass.Druid; } }
		public override ComparisonCalculationBase CreateNewComparisonCalculation() { return new ComparisonCalculationCat(); }
		public override CharacterCalculationsBase CreateNewCharacterCalculations() { return new CharacterCalculationsCat(); }

		public override ICalculationOptionBase DeserializeDataObject(string xml)
		{
			System.Xml.Serialization.XmlSerializer serializer =
				new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsCat));
			System.IO.StringReader reader = new System.IO.StringReader(xml);
			CalculationOptionsCat calcOpts = serializer.Deserialize(reader) as CalculationOptionsCat;
			return calcOpts;
		}

		public override CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, bool referenceCalculation, bool significantChange, bool needsDisplayCalculations)
		{
			CalculationOptionsCat calcOpts = character.CalculationOptions as CalculationOptionsCat;
			if (calcOpts == null) calcOpts = new CalculationOptionsCat();
			int targetLevel = calcOpts.TargetLevel;
			float targetArmor = calcOpts.TargetArmor;
			StatsCat stats = GetCharacterStats(character, additionalItem) as StatsCat;
			float levelDifference = (targetLevel - 80f) * 0.2f;
			CharacterCalculationsCat calculatedStats = new CharacterCalculationsCat();
			calculatedStats.BasicStats = stats;
			calculatedStats.TargetLevel = targetLevel;
			bool maintainMangle = stats.BonusBleedDamageMultiplier == 0f;
			stats.BonusBleedDamageMultiplier = 0.3f;

			#region Basic Chances and Constants
			float modArmor = 0f;
			for (int i = 0; i < stats.TemporaryArPenRatingUptimes.Length; i++)
			{
				modArmor += stats.TemporaryArPenRatingUptimes[i].Chance * StatConversion.GetArmorDamageReduction(character.Level, calcOpts.TargetLevel, calcOpts.TargetArmor, stats.ArmorPenetration, 0f, stats.ArmorPenetrationRating + stats.TemporaryArPenRatingUptimes[i].Value);
			}

			modArmor = 1f - modArmor;
			float critMultiplier = 2f * (1f + stats.BonusCritMultiplier);
			float critMultiplierBleed = 2f * (1f + stats.BonusCritMultiplier);
			float hasteBonus = StatConversion.GetPhysicalHasteFromRating(stats.HasteRating, CharacterClass.Druid);
			float attackSpeed = 1f / (1f + hasteBonus);
			attackSpeed = attackSpeed / (1f + stats.PhysicalHaste);

			float hitBonus = stats.PhysicalHit + StatConversion.GetPhysicalHitFromRating(stats.HitRating, CharacterClass.Druid);
			float expertiseBonus = StatConversion.GetDodgeParryReducFromExpertise(StatConversion.GetExpertiseFromRating(stats.ExpertiseRating, CharacterClass.Druid) + stats.Expertise, CharacterClass.Druid);

			float chanceDodge = Math.Max(0f, StatConversion.WHITE_DODGE_CHANCE_CAP[targetLevel-80] - expertiseBonus);
			float chanceParry = 0f; //Math.Max(0f, StatConversion.WHITE_PARRY_CHANCE_CAP[targetLevel - 80] - expertiseBonus);
			float chanceMiss  = Math.Max(0f, StatConversion.WHITE_MISS_CHANCE_CAP[ targetLevel-80] - hitBonus);
			
			float glanceMultiplier = 0.7f;
			float chanceAvoided = chanceMiss + chanceDodge + chanceParry;
			float chanceNonAvoided = 1f - chanceAvoided;

			////Crit Chances
			float chanceCritYellow = 0f;
			float chanceHitYellow = 0f;
			float cpPerCPG = 0f;
			float chanceCritBite = 0f;
			float chanceHitBite = 0f;
			float chanceCritBleed = 0f;
			float chanceCritRip = 0f;
			float chanceCritRake = 0f;
			float chanceGlance = 0f;
			float chanceCritWhite = 0f;
			float chanceHitWhite = 0f;

			for (int i = 0; i < stats.TemporaryCritRatingUptimes.Length; i++)
			{ //Sum up the weighted chances for each crit value
				WeightedStat iStat = stats.TemporaryCritRatingUptimes[i];
				//Yellow - 2 Roll, so total of X chance to avoid, total of 1 chance to crit and hit when not avoided
				float chanceCritYellowTemp = Math.Min(1f, StatConversion.GetCritFromRating(stats.CritRating + iStat.Value, CharacterClass.Druid)
					+ StatConversion.GetCritFromAgility(stats.Agility, CharacterClass.Druid)
					+ stats.PhysicalCrit
					+ StatConversion.NPC_LEVEL_CRIT_MOD[targetLevel - 80]);
				float chanceHitYellowTemp = 1f - chanceCritYellowTemp;
				float cpPerCPGTemp = chanceHitYellowTemp + chanceCritYellowTemp * (1f + stats.BonusCPOnCrit);

				//Bite - Identical to Yellow, with higher crit chance
				float chanceCritBiteTemp = Math.Min(1f, chanceCritYellowTemp + stats.BonusFerociousBiteCrit);
				float chanceHitBiteTemp = 1f - chanceCritBiteTemp;

				//Bleeds - 1 Roll, no avoidance, total of 1 chance to crit and hit
				float chanceCritBleedTemp = character.DruidTalents.PrimalGore > 0 ? chanceCritYellowTemp : 0f;
				float chanceCritRipTemp = Math.Min(1f, chanceCritBleedTemp > 0f ? chanceCritBleedTemp + stats.BonusRipCrit : 0f);
				float chanceCritRakeTemp = stats.BonusRakeCrit > 0 ? chanceCritBleedTemp : 0;

				//White
				float chanceGlanceTemp = StatConversion.WHITE_GLANCE_CHANCE_CAP[targetLevel - 80];
				float chanceCritWhiteTemp = Math.Min(chanceCritYellowTemp, 1f - chanceGlanceTemp - chanceAvoided);
				float chanceHitWhiteTemp = 1f - chanceCritWhiteTemp - chanceAvoided - chanceGlanceTemp;

				chanceCritYellow += iStat.Chance * chanceCritYellowTemp;
				chanceHitYellow += iStat.Chance * chanceHitYellowTemp;
				cpPerCPG += iStat.Chance * cpPerCPGTemp;
				chanceCritBite += iStat.Chance * chanceCritBiteTemp;
				chanceHitBite += iStat.Chance * chanceHitBiteTemp;
				chanceCritBleed += iStat.Chance * chanceCritBleedTemp;
				chanceCritRip += iStat.Chance * chanceCritRipTemp;
				chanceCritRake += iStat.Chance * chanceCritRakeTemp;
				chanceGlance += iStat.Chance * chanceGlanceTemp;
				chanceCritWhite += iStat.Chance * chanceCritWhiteTemp;
				chanceHitWhite += iStat.Chance * chanceHitWhiteTemp;
			}
			
			calculatedStats.DodgedAttacks = chanceDodge * 100f;
			calculatedStats.ParriedAttacks = chanceParry * 100f;
			calculatedStats.MissedAttacks = chanceMiss * 100f;

			float timeToReapplyDebuffs = 1f / (1f - chanceAvoided) - 1f;
			float lagVariance = (float)calcOpts.LagVariance / 1000f;
			float mangleDurationUptime = 60f;// (character.DruidTalents.GlyphOfMangle ? 18f : 12f);
			float mangleDurationAverage = mangleDurationUptime - timeToReapplyDebuffs - lagVariance;
			float rakeDurationUptime = 9f + stats.BonusRakeDuration;
			float rakeDurationAverage = rakeDurationUptime + timeToReapplyDebuffs + lagVariance;
			float ripDurationUptime = 12f + stats.BonusRipDuration; //Doesn't include Glyph of Shred
			float ripDurationAverage = ripDurationUptime + timeToReapplyDebuffs + lagVariance; //Doesn't include Glyph of Shred
			float roarBonusDuration = stats.BonusSavageRoarDuration - lagVariance;
			float berserkDuration = character.DruidTalents.Berserk > 0 ? (character.DruidTalents.GlyphOfBerserk ? 20f : 15f) : 0f;
			#endregion

			#region Attack Damages
			float baseDamage = 55f + (stats.AttackPower / 14f) + stats.WeaponDamage;
			float meleeDamageRaw = (baseDamage) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * modArmor;
			float mangleDamageRaw = (baseDamage * 2f + 566f) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + stats.BonusMangleDamageMultiplier) * modArmor;
			float shredDamageRaw = (baseDamage * 2.25f + 666f + stats.BonusShredDamage) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + stats.BonusShredDamageMultiplier) * (1f + stats.BonusBleedDamageMultiplier) * modArmor;
			float rakeDamageRaw = (176f + stats.AttackPower * 0.01f) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + stats.BonusRakeDamageMultiplier) * (1f + stats.BonusBleedDamageMultiplier);
			float rakeDamageDot = (1074f + stats.AttackPower * 0.18f) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + stats.BonusRakeDamageMultiplier) * (1f + stats.BonusBleedDamageMultiplier) * ((9f + stats.BonusRakeDuration) / 9f);
			float ripDamageRaw = (3006f + stats.AttackPower * 0.3f + (stats.BonusRipDamagePerCPPerTick * 5f * 6f)) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + stats.BonusRipDamageMultiplier) * (1f + stats.BonusBleedDamageMultiplier);
			float biteBaseDamageRaw = 190f * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + stats.BonusFerociousBiteDamageMultiplier) * modArmor;
			float biteCPDamageRaw = (290f + stats.AttackPower * 0.07f) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + stats.BonusFerociousBiteDamageMultiplier) * modArmor;

			float meleeDamageAverage =	chanceGlance * meleeDamageRaw * glanceMultiplier +
										chanceCritWhite * meleeDamageRaw * critMultiplier +
										chanceHitWhite * meleeDamageRaw;
			float mangleDamageAverage = (1f - chanceCritYellow) * mangleDamageRaw + chanceCritYellow * mangleDamageRaw * critMultiplier;
			float shredDamageAverage = (1f - chanceCritYellow) * shredDamageRaw + chanceCritYellow * shredDamageRaw * critMultiplier;
			float rakeDamageAverage = ((1f - chanceCritYellow) * rakeDamageRaw + chanceCritYellow * rakeDamageRaw * critMultiplier) + ((1f - chanceCritRake) * rakeDamageDot + chanceCritRake * rakeDamageDot * critMultiplierBleed);
			float ripDamageAverage = ((1f - chanceCritRip) * ripDamageRaw + chanceCritRip * ripDamageRaw * critMultiplierBleed);
			float biteBaseDamageAverage = (1f - chanceCritBite) * biteBaseDamageRaw + chanceCritBite * biteBaseDamageRaw * critMultiplier;
			float biteCPDamageAverage = (1f - chanceCritBite) * biteCPDamageRaw + chanceCritBite * biteCPDamageRaw * critMultiplier;

			//if (needsDisplayCalculations)
			//{
			//    Console.WriteLine("White:    {0:P} Avoided, {1:P} Glance,      {2:P} Hit, {3:P} Crit - Swing: {4}", chanceAvoided, chanceGlance, chanceHitWhite, chanceCritWhite, meleeDamageAverage);
			//    Console.WriteLine("Yellow:   {0:P} Avoided, {1:P} NonAvoided,  {2:P} Hit, {3:P} Crit - Swing: {4}", chanceAvoided, chanceNonAvoided, 1f - chanceCritYellow, chanceCritYellow, mangleDamageAverage);
			//    Console.WriteLine("Bite:     {0:P} Avoided, {1:P} NonAvoided,  {2:P} Hit, {3:P} Crit - Swing: {4}", chanceAvoided, chanceNonAvoided, 1f - chanceCritBite, chanceCritBite, biteBaseDamageAverage);
			//    Console.WriteLine("RipBleed:                                      {0:P} Hit, {1:P} Crit - Swing: {2}", 1f - chanceCritRip, chanceCritRip, ripDamageAverage);
			//    Console.WriteLine();
			//}
			#endregion

			#region Energy Costs
			float mangleEnergyRaw = 45f - stats.MangleCatCostReduction;
			float shredEnergyRaw = 60f - stats.ShredCostReduction;
			float rakeEnergyRaw = 40f - stats.RakeCostReduction;
			float ripEnergyRaw = 30f - stats.RipCostReduction;
			float biteEnergyRaw = 35f; //Assuming no wasted energy
			float roarEnergyRaw = 25f;

			//[rawCost + ((1/chance_to_land) - 1) * rawCost/5] 
			float cpgEnergyCostMultiplier = 1f + ((1f / chanceNonAvoided) - 1f) * 0.2f;
			float finisherEnergyCostMultiplier = 1f + ((1f / chanceNonAvoided) - 1f) * (1f - stats.FinisherEnergyOnAvoid);
			float mangleEnergyAverage = mangleEnergyRaw * cpgEnergyCostMultiplier;
			float shredEnergyAverage = shredEnergyRaw * cpgEnergyCostMultiplier;
			float rakeEnergyAverage = rakeEnergyRaw * cpgEnergyCostMultiplier;
			float ripEnergyAverage = ripEnergyRaw * finisherEnergyCostMultiplier;
			float biteEnergyAverage = biteEnergyRaw * finisherEnergyCostMultiplier;
			float roarEnergyAverage = roarEnergyRaw;
			#endregion

			#region Ability Stats
			CatAbilityStats meleeStats = new CatMeleeStats()
			{
				DamagePerHit = meleeDamageRaw,
				DamagePerSwing = meleeDamageAverage,
			};
			CatAbilityStats mangleStats = new CatMangleStats()
			{
				DamagePerHit = mangleDamageRaw,
				DamagePerSwing = mangleDamageAverage,
				DurationUptime = mangleDurationUptime,
				DurationAverage = mangleDurationAverage,
				EnergyCost = mangleEnergyAverage,
			};
			CatAbilityStats shredStats = new CatShredStats()
			{
				DamagePerHit = shredDamageRaw,
				DamagePerSwing = shredDamageAverage,
				EnergyCost = shredEnergyAverage,
			};
			CatAbilityStats rakeStats = new CatRakeStats()
			{
				DamagePerHit = rakeDamageRaw + rakeDamageDot,
				DamagePerSwing = rakeDamageAverage,
				DurationUptime = rakeDurationUptime,
				DurationAverage = rakeDurationAverage,
				EnergyCost = rakeEnergyAverage,
			};
			CatAbilityStats ripStats = new CatRipStats()
			{
				DamagePerHit = ripDamageRaw ,
				DamagePerSwing = ripDamageAverage,
				DurationUptime = ripDurationUptime,
				DurationAverage = ripDurationAverage,
				EnergyCost = ripEnergyAverage,
			};
			CatAbilityStats biteStats = new CatBiteStats()
			{
				DamagePerHit = biteBaseDamageRaw,
				DamagePerSwing = biteBaseDamageAverage,
				DamagePerHitPerCP = biteCPDamageRaw,
				DamagePerSwingPerCP = biteCPDamageAverage,
				EnergyCost = biteEnergyAverage,
			};
			CatAbilityStats roarStats = new CatRoarStats()
			{
				DurationUptime = roarBonusDuration,
				DurationAverage = 9f + roarBonusDuration,
				EnergyCost = roarEnergyAverage,
				DurationPerCP = 5f,
			};
			#endregion

			#region Rotations
			CatRotationCalculator rotationCalculator = new CatRotationCalculator(stats, calcOpts.Duration, cpPerCPG,
				maintainMangle, berserkDuration, attackSpeed, character.DruidTalents.OmenOfClarity > 0, 
				character.DruidTalents.GlyphOfShred, chanceAvoided, chanceCritYellow * stats.BonusCPOnCrit,
				cpgEnergyCostMultiplier, stats.ClearcastOnBleedChance, meleeStats, mangleStats, shredStats,
				rakeStats, ripStats, biteStats, roarStats);
			CatRotationCalculator.CatRotationCalculation rotationCalculationDPS = new CatRotationCalculator.CatRotationCalculation();

			for (int roarCP = 1; roarCP < 6; roarCP++)
				for (int biteCP = 0; biteCP < 6; biteCP++)
					for (int useRake = 0; useRake < 2; useRake++)
						for (int useShred = 0; useShred < 2; useShred++)
							for (int useRip = 0; useRip < 2; useRip++)
							{
								CatRotationCalculator.CatRotationCalculation rotationCalculation =
									rotationCalculator.GetRotationCalculations(
									useRake == 1, useShred == 1, useRip == 1, biteCP, roarCP);
								if (rotationCalculation.DPS > rotationCalculationDPS.DPS)
									rotationCalculationDPS = rotationCalculation;
							}

			calculatedStats.HighestDPSRotation = rotationCalculationDPS;
			calculatedStats.CustomRotation = rotationCalculator.GetRotationCalculations(
				calcOpts.CustomUseRake, calcOpts.CustomUseShred, calcOpts.CustomUseRip, calcOpts.CustomCPFerociousBite, calcOpts.CustomCPSavageRoar);
			
			if (character.DruidTalents.GlyphOfShred && rotationCalculationDPS.ShredCount > 0)
			{
				ripStats.DurationUptime += 6f;
				ripStats.DurationAverage += 6f;
			}
			ripStats.DamagePerHit *= ripStats.DurationUptime / 12f;
			ripStats.DamagePerSwing *= ripStats.DurationUptime / 12f;
			#endregion

			calculatedStats.AvoidedAttacks = chanceAvoided * 100f;
			calculatedStats.DodgedAttacks = chanceDodge * 100f;
			calculatedStats.ParriedAttacks = chanceParry * 100f;
			calculatedStats.MissedAttacks = chanceMiss * 100f;
			calculatedStats.CritChance = chanceCritYellow * 100f;
			calculatedStats.AttackSpeed = attackSpeed;
			calculatedStats.ArmorMitigation = (1f - modArmor) * 100f;
			calculatedStats.Duration = calcOpts.Duration;

			calculatedStats.MeleeStats = meleeStats;
			calculatedStats.MangleStats = mangleStats;
			calculatedStats.ShredStats = shredStats;
			calculatedStats.RakeStats = rakeStats;
			calculatedStats.RipStats = ripStats;
			calculatedStats.RoarStats = roarStats;
			calculatedStats.BiteStats = biteStats;

			float magicDPS = (stats.ShadowDamage + stats.ArcaneDamage) * (1f + chanceCritYellow);
			float abomDPS = (stats.MoteOfAnger * meleeDamageAverage);
			calculatedStats.DPSPoints = calculatedStats.HighestDPSRotation.DPS + magicDPS + abomDPS;
			calculatedStats.SurvivabilityPoints = stats.Health / 100f;
			calculatedStats.OverallPoints = calculatedStats.DPSPoints + calculatedStats.SurvivabilityPoints;
			return calculatedStats;
		}

		public override Stats GetCharacterStats(Character character, Item additionalItem)
		{
			CalculationOptionsCat calcOpts = character.CalculationOptions as CalculationOptionsCat;
			int targetLevel = calcOpts.TargetLevel;

			
			
			Stats statsItems = GetItemStats(character, additionalItem);
			statsItems.Armor *= new float[] { 1f, 1.04f, 1.07f, 1.1f }[character.DruidTalents.ThickHide];

			DruidTalents talents = character.DruidTalents;
			StatsCat statsTotal = new StatsCat()
			{
				PhysicalCrit = 0.02f * talents.SharpenedClaws
							 + 0.02f * talents.MasterShapeshifter
							 + ((character.ActiveBuffsContains("Leader of the Pack") ||
								 character.ActiveBuffsContains("Rampage"))
								 ? 0 : 0.05f * talents.LeaderOfThePack),
				Dodge = 0.02f * talents.FeralSwiftness,
				BonusStaminaMultiplier = (1f + 0.02f * talents.SurvivalOfTheFittest) * (1f + 0.01f * talents.ImprovedMarkOfTheWild) - 1f,
				BonusAgilityMultiplier = (1f + 0.02f * talents.SurvivalOfTheFittest) * (1f + 0.01f * talents.ImprovedMarkOfTheWild) - 1f,
				BonusStrengthMultiplier = (1f + 0.02f * talents.SurvivalOfTheFittest) * (1f + 0.01f * talents.ImprovedMarkOfTheWild) - 1f,
				BonusAttackPowerMultiplier = 0.02f * talents.HeartOfTheWild,
				CritChanceReduction = 0.02f * talents.SurvivalOfTheFittest,
				BonusPhysicalDamageMultiplier = (1f + 0.02f * talents.Naturalist) * (talents.GlyphOfSavageRoar ? 1.33f : 1.3f) - 1f,
				BonusMangleDamageMultiplier = (1f + 0.1f * talents.SavageFury) * (talents.GlyphOfMangle ? 1.1f : 1.0f) - 1f,
				BonusRakeDamageMultiplier = 0.1f * talents.SavageFury,
				BonusShredDamageMultiplier = 0.04f * talents.RendAndTear,
				BonusFerociousBiteCrit = 0.05f * talents.RendAndTear,
				BonusEnergyOnTigersFury = 20f * talents.KingOfTheJungle,
				MangleCatCostReduction = 1f * talents.Ferocity + 2f * talents.ImprovedMangle,
				RakeCostReduction = 1f * talents.Ferocity,
				ShredCostReduction = 9f * talents.ShreddingAttacks,
				BonusCPOnCrit = 0.5f * talents.PrimalFury,
				Expertise = 5f * talents.PrimalPrecision,
				FinisherEnergyOnAvoid = 0.4f * talents.PrimalPrecision,
				AttackPower = (character.Level / 2f) * talents.PredatoryStrikes,
				BonusCritMultiplier = 0.1f * ((float)talents.PredatoryInstincts / 3f),
				BonusFerociousBiteDamageMultiplier = 0.03f * talents.FeralAggression,
				BonusRipDuration = talents.GlyphOfRip ? 4f : 0f,
			};
			statsTotal.Accumulate(BaseStats.GetBaseStats(80, character.Class, character.Race, BaseStats.DruidForm.Cat));
			statsTotal.Accumulate(statsItems);
			statsTotal.Accumulate(GetBuffsStats(character, calcOpts));

			float predatoryStrikesAP = 0f;
			float fap = 0f;
			if (character.MainHand != null)
			{
				fap = Math.Max(0f, (character.MainHand.Item.DPS - 54.8f) * 14f); //TODO Find a more accurate number for this?
				predatoryStrikesAP = (fap + character.MainHand.Item.Stats.AttackPower) * 0.2f * (talents.PredatoryStrikes / 3f);
				if (character.MainHand.Enchant != null)
				{
					predatoryStrikesAP += character.MainHand.Enchant.Stats.AttackPower * 0.2f * (talents.PredatoryStrikes / 3f);
				}
			}

			statsTotal.Stamina = (float)Math.Floor(statsTotal.Stamina * (1f + statsTotal.BonusStaminaMultiplier));
			statsTotal.Strength = (float)Math.Floor((statsTotal.Strength + statsTotal.CatFormStrength) * (1f + statsTotal.BonusStrengthMultiplier));
			statsTotal.Agility = (float)Math.Floor(statsTotal.Agility * (1f + statsTotal.BonusAgilityMultiplier));
			statsTotal.AttackPower += statsTotal.Strength * 2f + statsTotal.Agility + fap + predatoryStrikesAP;
			statsTotal.AttackPower = (float)Math.Floor(statsTotal.AttackPower * (1f+ statsTotal.BonusAttackPowerMultiplier));
			statsTotal.Health += (float)Math.Floor((statsTotal.Stamina - 20f) * 10f + 20f);
			statsTotal.Health = (float)Math.Floor(statsTotal.Health * (1f + statsTotal.BonusHealthMultiplier));
			statsTotal.Armor += 2f * statsTotal.Agility;
			statsTotal.Armor = (float)Math.Floor(statsTotal.Armor * (1f + statsTotal.BonusArmorMultiplier));
			statsTotal.NatureResistance += statsTotal.NatureResistanceBuff;
			statsTotal.FireResistance += statsTotal.FireResistanceBuff;
			statsTotal.FrostResistance += statsTotal.FrostResistanceBuff;
			statsTotal.ShadowResistance += statsTotal.ShadowResistanceBuff;
			statsTotal.ArcaneResistance += statsTotal.ArcaneResistanceBuff;
			statsTotal.WeaponDamage += 16f; //Tiger's Fury

			float hasteBonus = StatConversion.GetPhysicalHasteFromRating(statsTotal.HasteRating, CharacterClass.Druid);
			hasteBonus = (1f + hasteBonus) * (1f + statsTotal.PhysicalHaste) - 1f;
			float meleeHitInterval = 1f / ((1f + hasteBonus) + 1f / 3.5f);
			float hitBonus = StatConversion.GetPhysicalHitFromRating(statsTotal.HitRating) + statsTotal.PhysicalHit;
			float expertiseBonus = StatConversion.GetDodgeParryReducFromExpertise(StatConversion.GetExpertiseFromRating(statsTotal.ExpertiseRating, CharacterClass.Druid) + statsTotal.Expertise, CharacterClass.Druid);
			float chanceDodge = Math.Max(0f, StatConversion.WHITE_DODGE_CHANCE_CAP[targetLevel-80] - expertiseBonus);
			float chanceParry = 0f;// Math.Max(0f, StatConversion.WHITE_PARRY_CHANCE_CAP[targetLevel - 80] - expertiseBonus);
			float chanceMiss  = Math.Max(0f, StatConversion.WHITE_MISS_CHANCE_CAP[ targetLevel-80] - hitBonus);
			float chanceAvoided = chanceMiss + chanceDodge + chanceParry;

			float rawChanceCrit = StatConversion.GetPhysicalCritFromRating(statsTotal.CritRating)
								+ StatConversion.GetPhysicalCritFromAgility(statsTotal.Agility, CharacterClass.Druid)
								+ statsTotal.PhysicalCrit
								+ StatConversion.NPC_LEVEL_CRIT_MOD[targetLevel - 80];
			float chanceCrit = rawChanceCrit * (1f - chanceAvoided);
			float chanceHit = 1f - chanceAvoided;
			bool usesMangle = (talents.Mangle > 0 && !character.ActiveBuffsContains("Mangle") && !character.ActiveBuffsContains("Trauma"));
			

			Dictionary<Trigger, float> triggerIntervals = new Dictionary<Trigger, float>();
			Dictionary<Trigger, float> triggerChances = new Dictionary<Trigger, float>();
			triggerIntervals[Trigger.Use] = 0f;
			triggerIntervals[Trigger.MeleeAttack] = meleeHitInterval;
			triggerIntervals[Trigger.MeleeHit] = meleeHitInterval;
			triggerIntervals[Trigger.PhysicalHit] = meleeHitInterval;
			triggerIntervals[Trigger.MeleeCrit] = meleeHitInterval;
			triggerIntervals[Trigger.PhysicalCrit] = meleeHitInterval;
			triggerIntervals[Trigger.DoTTick] = 1.5f;
			triggerIntervals[Trigger.DamageDone] = meleeHitInterval / 2f;
			triggerIntervals[Trigger.DamageOrHealingDone] = meleeHitInterval / 2f; // Need to Add Self-Heals
			triggerIntervals[Trigger.RakeTick] = 3f + (float)calcOpts.LagVariance / 3000f;
			if (usesMangle)
				triggerIntervals[Trigger.MangleCatHit] = 60f;
			triggerIntervals[Trigger.MangleCatOrShredHit] = usesMangle ? 3.76f : 3.87f;
			triggerIntervals[Trigger.MangleCatOrShredOrInfectedWoundsHit] = triggerIntervals[Trigger.MangleCatOrShredHit] / ((talents.InfectedWounds > 0) ? 2f : 1f);
			triggerChances[Trigger.Use] = 1f;
			triggerChances[Trigger.MeleeAttack] = 1f;
			triggerChances[Trigger.MeleeHit] = Math.Max(0f, chanceHit);
			triggerChances[Trigger.PhysicalHit] = Math.Max(0f, chanceHit);
			triggerChances[Trigger.MeleeCrit] = Math.Max(0f, chanceCrit);
			triggerChances[Trigger.PhysicalCrit] = Math.Max(0f, chanceCrit);
			triggerChances[Trigger.DoTTick] = 1f;
			triggerChances[Trigger.DamageDone] = 1f - chanceAvoided / 2f;
			triggerChances[Trigger.DamageOrHealingDone] = 1f - chanceAvoided / 2f; // Need to Add Self-Heals
			triggerChances[Trigger.RakeTick] = 1f;
			if (talents.Mangle > 0 && !character.ActiveBuffsContains("Mangle") && !character.ActiveBuffsContains("Trauma"))
				triggerChances[Trigger.MangleCatHit] = chanceHit;
			triggerChances[Trigger.MangleCatOrShredHit] = chanceHit;
			triggerChances[Trigger.MangleCatOrShredOrInfectedWoundsHit] = chanceHit;

			// Handle Trinket procs
			Stats statsProcs = new Stats();
			foreach (SpecialEffect effect in statsTotal.SpecialEffects(se => triggerIntervals.ContainsKey(se.Trigger)))
			{
				// JOTHAY's NOTE: The following is an ugly hack to add Recursive Effects to Cat
				// so Victor's Call and similar trinkets can be given more appropriate value
				if (effect.Trigger == Trigger.Use && effect.Stats._rawSpecialEffectDataSize == 1
					&& triggerIntervals.ContainsKey(effect.Stats._rawSpecialEffectData[0].Trigger))
				{
					float upTime = effect.GetAverageUptime(triggerIntervals[effect.Trigger],
						triggerChances[effect.Trigger], 1f, calcOpts.Duration);
					statsProcs.Accumulate(effect.Stats._rawSpecialEffectData[0].GetAverageStats(
						triggerIntervals[effect.Stats._rawSpecialEffectData[0].Trigger],
						triggerChances[effect.Stats._rawSpecialEffectData[0].Trigger], 1f, calcOpts.Duration),
						upTime);
				}
				else if (effect.Stats.MoteOfAnger > 0)
				{
					// When in effect stats, MoteOfAnger is % of melee hits
					// When in character stats, MoteOfAnger is average procs per second
					statsProcs.MoteOfAnger = effect.Stats.MoteOfAnger * effect.GetAverageProcsPerSecond(triggerIntervals[effect.Trigger],
						triggerChances[effect.Trigger], 1f, calcOpts.Duration) / effect.MaxStack;
				}
				else
				{
					statsProcs.Accumulate(effect.GetAverageStats(triggerIntervals[effect.Trigger],
						triggerChances[effect.Trigger], 1f, calcOpts.Duration),
						effect.Stats.DeathbringerProc > 0 ? 1f / 3f : 1f);
				}
			}

			statsProcs.Agility += statsProcs.HighestStat + statsProcs.Paragon + statsProcs.DeathbringerProc;
			statsProcs.Strength += statsProcs.DeathbringerProc;
			statsProcs.Stamina = (float)Math.Floor(statsProcs.Stamina * (1f + statsTotal.BonusStaminaMultiplier));
			statsProcs.Strength = (float)Math.Floor(statsProcs.Strength * (1f + statsTotal.BonusStrengthMultiplier));
			statsProcs.Agility = (float)Math.Floor(statsProcs.Agility * (1f + statsTotal.BonusAgilityMultiplier));
			statsProcs.AttackPower += statsProcs.Strength * 2f + statsProcs.Agility;
			statsProcs.AttackPower = (float)Math.Floor(statsProcs.AttackPower * (1f + statsTotal.BonusAttackPowerMultiplier));
			statsProcs.HasteRating += statsProcs.DeathbringerProc;
			statsProcs.Health += (float)Math.Floor(statsProcs.Stamina * 10f);
			statsProcs.Armor += 2f * statsProcs.Agility;
			statsProcs.Armor = (float)Math.Floor(statsProcs.Armor * (1f + statsTotal.BonusArmorMultiplier));
			
			//Agility is only used for crit from here on out; we'll be converting Agility to CritRating, 
			//and calculating CritRating separately, so don't add any Agility or CritRating from procs here.
			//Also calculating ArPen separately, so don't add that either.
			statsProcs.CritRating = statsProcs.Agility = statsProcs.ArmorPenetrationRating = 0;
			statsTotal.Accumulate(statsProcs);

			//Handle Crit procs
			statsTotal.TemporaryCritRatingUptimes = new WeightedStat[0];
			List<SpecialEffect> tempCritEffects = new List<SpecialEffect>();
			List<float> tempCritEffectIntervals = new List<float>();
			List<float> tempCritEffectChances = new List<float>();
			List<float> tempCritEffectScales = new List<float>();

			foreach (SpecialEffect effect in statsTotal.SpecialEffects(se => triggerIntervals.ContainsKey(se.Trigger) && (se.Stats.CritRating + se.Stats.Agility + se.Stats.DeathbringerProc + se.Stats.HighestStat + se.Stats.Paragon) > 0))
			{
				tempCritEffects.Add(effect);
				tempCritEffectIntervals.Add(triggerIntervals[effect.Trigger]);
				tempCritEffectChances.Add(triggerChances[effect.Trigger]);
				tempCritEffectScales.Add(effect.Stats.DeathbringerProc > 0 ? 1f / 3f : 1f);
			}

			if (tempCritEffects.Count == 0)
			{
				statsTotal.TemporaryCritRatingUptimes = new WeightedStat[] { new WeightedStat() { Chance = 1f, Value = 0f } };
			}
			else if (tempCritEffects.Count == 1)
			{ //Only one, add it to
				SpecialEffect effect = tempCritEffects[0];
				float uptime = effect.GetAverageUptime(triggerIntervals[effect.Trigger], triggerChances[effect.Trigger], 1f, calcOpts.Duration) * tempCritEffectScales[0];
				float totalAgi = (float)effect.MaxStack * (effect.Stats.Agility + effect.Stats.DeathbringerProc + effect.Stats.HighestStat + effect.Stats.Paragon) * (1f + statsTotal.BonusAgilityMultiplier);
				statsTotal.TemporaryCritRatingUptimes = new WeightedStat[] { new WeightedStat() { Chance = uptime, Value = 
						effect.Stats.CritRating + StatConversion.GetCritFromAgility(totalAgi,
						CharacterClass.Druid) * StatConversion.RATING_PER_PHYSICALCRIT },
					new WeightedStat() { Chance = 1f - uptime, Value = 0f }};
			}
			else if (tempCritEffects.Count > 1)
			{
				List<float> tempCritEffectsValues = new List<float>();
				foreach (SpecialEffect effect in tempCritEffects)
				{
					float totalAgi = (float)effect.MaxStack * (effect.Stats.Agility + effect.Stats.DeathbringerProc + effect.Stats.HighestStat + effect.Stats.Paragon) * (1f + statsTotal.BonusAgilityMultiplier);
					tempCritEffectsValues.Add(effect.Stats.CritRating +
						StatConversion.GetCritFromAgility(totalAgi, CharacterClass.Druid) * 
						StatConversion.RATING_PER_PHYSICALCRIT);
				}
				
				float[] intervals = new float[tempCritEffects.Count];
				float[] chances = new float[tempCritEffects.Count];
				float[] offset = new float[tempCritEffects.Count];
				for (int i = 0; i < tempCritEffects.Count; i++)
				{
					intervals[i] = triggerIntervals[tempCritEffects[i].Trigger];
					chances[i] = triggerChances[tempCritEffects[i].Trigger];
				}
				if (tempCritEffects.Count >= 2)
				{
					offset[0] = calcOpts.TrinketOffset;
				}
				WeightedStat[] critWeights = SpecialEffect.GetAverageCombinedUptimeCombinations(tempCritEffects.ToArray(), intervals, chances, offset, tempCritEffectScales.ToArray(), 1f, calcOpts.Duration, tempCritEffectsValues.ToArray());
				statsTotal.TemporaryCritRatingUptimes = critWeights;
			}

			//Handle ArPen procs
			statsTotal.TemporaryArPenRatingUptimes = new WeightedStat[0];
			List<SpecialEffect> tempArPenEffects = new List<SpecialEffect>();
			List<float> tempArPenEffectIntervals = new List<float>();
			List<float> tempArPenEffectChances = new List<float>();
			List<float> tempArPenEffectScales = new List<float>();

			foreach (SpecialEffect effect in statsTotal.SpecialEffects(se => triggerIntervals.ContainsKey(se.Trigger) && se.Stats.ArmorPenetrationRating > 0))
			{
				tempArPenEffects.Add(effect);
				tempArPenEffectIntervals.Add(triggerIntervals[effect.Trigger]);
				tempArPenEffectChances.Add(triggerChances[effect.Trigger]);
				tempArPenEffectScales.Add(effect.Stats.DeathbringerProc > 0 ? 1f / 3f : 1f);
			}

			if (tempArPenEffects.Count == 0)
			{
				statsTotal.TemporaryArPenRatingUptimes = new WeightedStat[] { new WeightedStat() { Chance = 1f, Value = 0f } };
			}
			else if (tempArPenEffects.Count == 1)
			{ //Only one, add it to
				SpecialEffect effect = tempArPenEffects[0];
				float uptime = effect.GetAverageUptime(triggerIntervals[effect.Trigger], triggerChances[effect.Trigger], 1f, calcOpts.Duration) * tempArPenEffectScales[0];
				statsTotal.TemporaryArPenRatingUptimes = new WeightedStat[] { new WeightedStat() { Chance = uptime, Value = effect.Stats.ArmorPenetrationRating },
					new WeightedStat() { Chance = 1f - uptime, Value = 0f }};
			}
			else if (tempArPenEffects.Count > 1)
			{
				List<float> tempArPenEffectValues = new List<float>();
				foreach (SpecialEffect effect in tempArPenEffects)
				{
					tempArPenEffectValues.Add(effect.Stats.ArmorPenetrationRating);
				}

				float[] intervals = new float[tempArPenEffects.Count];
				float[] chances = new float[tempArPenEffects.Count];
				float[] offset = new float[tempArPenEffects.Count];
				for (int i = 0; i < tempArPenEffects.Count; i++)
				{
					intervals[i] = triggerIntervals[tempArPenEffects[i].Trigger];
					chances[i] = triggerChances[tempArPenEffects[i].Trigger];
				}
				if (tempArPenEffects.Count >= 2)
				{
					offset[0] = calcOpts.TrinketOffset;
				}
				WeightedStat[] arPenWeights = SpecialEffect.GetAverageCombinedUptimeCombinations(tempArPenEffects.ToArray(), intervals, chances, offset, tempArPenEffectScales.ToArray(), 1f, calcOpts.Duration, tempArPenEffectValues.ToArray());
				statsTotal.TemporaryArPenRatingUptimes = arPenWeights;
			}

			return statsTotal;
		}

		public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
		{
			switch (chartName)
			{
				case "White Combat Table":
					CharacterCalculationsCat calcs = (CharacterCalculationsCat)GetCharacterCalculations(character);
					float[] ct = null;//calcs.MeleeStats.CombatTable;
					return new ComparisonCalculationBase[]
					{
						new ComparisonCalculationCat() { Name = "Miss", OverallPoints = ct[0], DPSPoints = ct[0]},
						new ComparisonCalculationCat() { Name = "Dodge", OverallPoints = ct[1], DPSPoints = ct[1]},
						new ComparisonCalculationCat() { Name = "Parry", OverallPoints = ct[2], DPSPoints = ct[2]},
						new ComparisonCalculationCat() { Name = "Glance", OverallPoints = ct[3], DPSPoints = ct[3]},
						new ComparisonCalculationCat() { Name = "Hit", OverallPoints = ct[4], DPSPoints = ct[4]},
						new ComparisonCalculationCat() { Name = "Crit", OverallPoints = ct[5], DPSPoints = ct[5]},
					};

				default:
					return new ComparisonCalculationBase[0];
			}
		}

		public override bool IsItemRelevant(Item item)
		{
			if (item.Slot == ItemSlot.OffHand || 
				(item.Slot == ItemSlot.Ranged && item.Type != ItemType.Idol) ||
				item.Stats.SpellPower > 0) 
				return false;
			foreach (var effect in item.Stats.SpecialEffects(s => s.Stats.SpellPower > 0))
				return false;
			return base.IsItemRelevant(item);
		}

		public override Stats GetRelevantStats(Stats stats)
		{
			Stats s = new Stats()
				{
					Agility = stats.Agility,
					Strength = stats.Strength,
					CatFormStrength = stats.CatFormStrength,
					AttackPower = stats.AttackPower,
					CritRating = stats.CritRating,
					HitRating = stats.HitRating,
					Stamina = stats.Stamina,
					HasteRating = stats.HasteRating,
					ExpertiseRating = stats.ExpertiseRating,
					ArmorPenetration = stats.ArmorPenetration,
					ArmorPenetrationRating = stats.ArmorPenetrationRating,
					BonusShredDamage = stats.BonusShredDamage,
					BonusRipDamagePerCPPerTick = stats.BonusRipDamagePerCPPerTick,
					WeaponDamage = stats.WeaponDamage,
					BonusAgilityMultiplier = stats.BonusAgilityMultiplier,
					BonusAttackPowerMultiplier = stats.BonusAttackPowerMultiplier,
					BonusCritMultiplier = stats.BonusCritMultiplier,
					BonusDamageMultiplier = stats.BonusDamageMultiplier,
					BonusHealthMultiplier = stats.BonusHealthMultiplier,
					BonusRipDamageMultiplier = stats.BonusRipDamageMultiplier,
					BonusStaminaMultiplier = stats.BonusStaminaMultiplier,
					BonusStrengthMultiplier = stats.BonusStrengthMultiplier,
					Health = stats.Health,
					MangleCatCostReduction = stats.MangleCatCostReduction,
					TigersFuryCooldownReduction = stats.TigersFuryCooldownReduction,
					ThreatReductionMultiplier = stats.ThreatReductionMultiplier,
					PhysicalHaste = stats.PhysicalHaste,
					PhysicalHit = stats.PhysicalHit,
					BonusBleedDamageMultiplier = stats.BonusBleedDamageMultiplier,
					PhysicalCrit = stats.PhysicalCrit,
					BonusSavageRoarDuration = stats.BonusSavageRoarDuration,
					ClearcastOnBleedChance = stats.ClearcastOnBleedChance,
					ArcaneDamage = stats.ArcaneDamage,
					ShadowDamage = stats.ShadowDamage,
					HighestStat = stats.HighestStat,
					Paragon = stats.Paragon,
					DeathbringerProc = stats.DeathbringerProc,
					BonusRakeDuration = stats.BonusRakeDuration,
					BonusRipCrit = stats.BonusRipCrit,
					BonusRakeCrit = stats.BonusRakeCrit,
					RipCostReduction = stats.RipCostReduction,
					MoteOfAnger = stats.MoteOfAnger,

					ArcaneResistance = stats.ArcaneResistance,
					NatureResistance = stats.NatureResistance,
					FireResistance = stats.FireResistance,
					FrostResistance = stats.FrostResistance,
					ShadowResistance = stats.ShadowResistance,
					ArcaneResistanceBuff = stats.ArcaneResistanceBuff,
					NatureResistanceBuff = stats.NatureResistanceBuff,
					FireResistanceBuff = stats.FireResistanceBuff,
					FrostResistanceBuff = stats.FrostResistanceBuff,
					ShadowResistanceBuff = stats.ShadowResistanceBuff,
					BonusRipDuration = stats.BonusRipDuration,
				};
			foreach (SpecialEffect effect in stats.SpecialEffects())
			{
				if (effect.Trigger == Trigger.Use || effect.Trigger == Trigger.MeleeCrit || effect.Trigger == Trigger.MeleeHit || effect.Trigger == Trigger.MeleeAttack
					|| effect.Trigger == Trigger.PhysicalCrit || effect.Trigger == Trigger.PhysicalHit || effect.Trigger == Trigger.DoTTick
					|| effect.Trigger == Trigger.DamageDone || effect.Trigger == Trigger.MangleCatHit || effect.Trigger == Trigger.RakeTick
					|| effect.Trigger == Trigger.MangleCatOrShredHit || effect.Trigger == Trigger.MangleCatOrShredOrInfectedWoundsHit || effect.Trigger == Trigger.DamageOrHealingDone)
				{
					if (HasRelevantStats(effect.Stats))
					{
						s.AddSpecialEffect(effect);
					}
				}
			}
			return s;
		}

		public override bool HasRelevantStats(Stats stats)
		{
			bool relevant = (stats.Agility + stats.ArmorPenetration + stats.AttackPower + stats.PhysicalCrit +
				stats.BonusAgilityMultiplier + stats.BonusAttackPowerMultiplier + stats.BonusCritMultiplier +
				stats.ClearcastOnBleedChance + stats.BonusSavageRoarDuration + stats.BonusRakeCrit + stats.RipCostReduction +
				stats.BonusDamageMultiplier + stats.BonusRipDamageMultiplier + stats.BonusShredDamage +
				stats.BonusStaminaMultiplier + stats.BonusStrengthMultiplier + stats.CritRating + stats.ExpertiseRating +
				stats.HasteRating + stats.Health + stats.HitRating + stats.MangleCatCostReduction + /*stats.Stamina +*/
				stats.Strength + stats.CatFormStrength + stats.WeaponDamage + stats.DeathbringerProc +
				stats.PhysicalHit + stats.BonusRipDamagePerCPPerTick + stats.BonusRipCrit + stats.MoteOfAnger +
				stats.PhysicalHaste + stats.ArmorPenetrationRating + stats.BonusRipDuration + stats.BonusRakeDuration +
				stats.ThreatReductionMultiplier + stats.ArcaneDamage + stats.ShadowDamage +
				stats.ArcaneResistance + stats.NatureResistance + stats.FireResistance + stats.BonusBleedDamageMultiplier + stats.Paragon +
				stats.FrostResistance + stats.ShadowResistance + stats.ArcaneResistanceBuff + stats.TigersFuryCooldownReduction + stats.HighestStat +
				stats.NatureResistanceBuff + stats.FireResistanceBuff + stats.BonusPhysicalDamageMultiplier +
				stats.FrostResistanceBuff + stats.ShadowResistanceBuff) > 0 || (stats.Stamina > 0 && stats.SpellPower == 0);

			foreach (SpecialEffect effect in stats.SpecialEffects())
			{
				if (effect.Trigger == Trigger.Use || effect.Trigger == Trigger.MeleeCrit || effect.Trigger == Trigger.MeleeHit || effect.Trigger == Trigger.MeleeAttack
					|| effect.Trigger == Trigger.PhysicalCrit || effect.Trigger == Trigger.PhysicalHit || effect.Trigger == Trigger.RakeTick
					|| effect.Trigger == Trigger.MangleCatHit || effect.Trigger == Trigger.MangleCatOrShredHit 
					|| effect.Trigger == Trigger.MangleCatOrShredOrInfectedWoundsHit || effect.Trigger == Trigger.DamageOrHealingDone)
				{
					relevant |= HasRelevantStats(effect.Stats);
					if (relevant) break;
				}
			}
			return relevant;
		}

		public Stats GetBuffsStats(Character character, CalculationOptionsCat calcOpts) {
			List<Buff> removedBuffs = new List<Buff>();
			List<Buff> addedBuffs = new List<Buff>();

			//float hasRelevantBuff;

			#region Passive Ability Auto-Fixing
			// Removes the Trueshot Aura Buff and it's equivalents Unleashed Rage and Abomination's Might if you are
			// maintaining it yourself. We are now calculating this internally for better accuracy and to provide
			// value to relevant talents
			/*{
				hasRelevantBuff = character.HunterTalents.TrueshotAura;
				Buff a = Buff.GetBuffByName("Trueshot Aura");
				Buff b = Buff.GetBuffByName("Unleashed Rage");
				Buff c = Buff.GetBuffByName("Abomination's Might");
				if (hasRelevantBuff > 0)
				{
					if (character.ActiveBuffs.Contains(a)) { character.ActiveBuffs.Remove(a); removedBuffs.Add(a); }
					if (character.ActiveBuffs.Contains(b)) { character.ActiveBuffs.Remove(b); removedBuffs.Add(b); }
					if (character.ActiveBuffs.Contains(c)) { character.ActiveBuffs.Remove(c); removedBuffs.Add(c); }
				}
			}*/
			#endregion

			Stats statsBuffs = GetBuffsStats(character.ActiveBuffs);

			foreach (Buff b in removedBuffs) {
				character.ActiveBuffsAdd(b);
			}
			foreach (Buff b in addedBuffs) {
				character.ActiveBuffs.Remove(b);
			}

			return statsBuffs;
		}
		public override void SetDefaults(Character character)
		{
			character.ActiveBuffsAdd(("Horn of Winter"));
			character.ActiveBuffsAdd(("Battle Shout"));
			character.ActiveBuffsAdd(("Unleashed Rage"));
			character.ActiveBuffsAdd(("Improved Moonkin Form"));
			character.ActiveBuffsAdd(("Leader of the Pack"));
			character.ActiveBuffsAdd(("Improved Icy Talons"));
			character.ActiveBuffsAdd(("Power Word: Fortitude"));
			character.ActiveBuffsAdd(("Mark of the Wild"));
			character.ActiveBuffsAdd(("Blessing of Kings"));
			character.ActiveBuffsAdd(("Sunder Armor"));
			character.ActiveBuffsAdd(("Faerie Fire"));
			character.ActiveBuffsAdd(("Totem of Wrath"));
			character.ActiveBuffsAdd(("Flask of Endless Rage"));
			character.ActiveBuffsAdd(("Agility Food"));
			character.ActiveBuffsAdd(("Heroism/Bloodlust"));

			if (character.PrimaryProfession == Profession.Alchemy ||
				character.SecondaryProfession == Profession.Alchemy)
				character.ActiveBuffsAdd(("Flask of Endless Rage (Mixology)"));

			character.DruidTalents.GlyphOfSavageRoar = true;
			character.DruidTalents.GlyphOfShred = true;
			character.DruidTalents.GlyphOfRip = true;
		}

		private static List<string> _relevantGlyphs;
		public override List<string> GetRelevantGlyphs()
		{
			if (_relevantGlyphs == null)
			{
				_relevantGlyphs = new List<string>();
				_relevantGlyphs.Add("Glyph of Mangle");
				_relevantGlyphs.Add("Glyph of Shred");
				_relevantGlyphs.Add("Glyph of Rip");
				_relevantGlyphs.Add("Glyph of Berserk");
				_relevantGlyphs.Add("Glyph of Savage Roar");
			}
			return _relevantGlyphs;
		}
	}

	public class ComparisonCalculationCat : ComparisonCalculationBase
	{
		private string _name = string.Empty;
		public override string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private string _desc = string.Empty;
		public override string Description
		{
			get { return _desc; }
			set { _desc = value; }
		}

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

		private Item _item = null;
		public override Item Item
		{
			get { return _item; }
			set { _item = value; }
		}

		private ItemInstance _itemInstance = null;
		public override ItemInstance ItemInstance
		{
			get { return _itemInstance; }
			set { _itemInstance = value; }
		}

		private bool _equipped = false;
		public override bool Equipped
		{
			get { return _equipped; }
			set { _equipped = value; }
		}
        public override bool PartEquipped { get; set; }

		public override string ToString()
		{
			return string.Format("{0}: ({1}O {2}DPS)", Name, Math.Round(OverallPoints), Math.Round(DPSPoints));
		}
	}
}
