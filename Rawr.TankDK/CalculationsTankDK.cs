using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if RAWR3
using System.Windows.Media;
#else
using System.Drawing;
#endif
using System.Xml.Serialization;

namespace Rawr.TankDK {
	[Rawr.Calculations.RawrModelInfo("TankDK", "spell_deathknight_darkconviction", CharacterClass.DeathKnight)]
	public class CalculationsTankDK : CalculationsBase
	{
		public struct TankDKChar
		{
			public Character Char;
			public CalculationOptionsTankDK opts;
			//public CombatTable ct;
			public Rotation Rot;
		}

		#region Gems
		enum GemQuality {
			Uncommon,
			Rare, 
			Epic,
			Jewelcraft,

			NUM_Quality
		}
		public override List<GemmingTemplate> DefaultGemmingTemplates {
			get {
				////Relevant Gem IDs for TankDKs
				//Red
				//                    UC     Rare   Epic   JC
				int[] subtle =      { 39907, 40000, 40115, 42151 }; // +Dodge

				//Purple
				int[] regal =       { 39938, 40031, 40138, }; // +dodge, Stam

				//Blue
				int[] solid =       { 39919, 40008, 40119, 36767 }; // +Stam

				//Green
				int[] enduring =    { 39976, 40089, 40167,  }; // +Def +Stam

				//Yellow
				int[] thick =       { 39916, 40015, 40126, 42157 }; // +def

				//Orange
				int[] stalwart =    { 39964, 40056, 40160 }; // +Dodge +Def

				//Meta
				int austere = 41380;

				// Prismatic:
				int nightmare = 49110;

				return new List<GemmingTemplate>() {
					new GemmingTemplate() { Model = "TankDK", Group = "Uncommon", //Defense 
						RedId = stalwart[0], YellowId = thick[0], BlueId = enduring[0], PrismaticId = thick[0], MetaId = austere },
					new GemmingTemplate() { Model = "TankDK", Group = "Uncommon", //Dodge
						RedId = subtle[0], YellowId = stalwart[0], BlueId = regal[0], PrismaticId = subtle[0], MetaId = austere },
					new GemmingTemplate() { Model = "TankDK", Group = "Uncommon", //Max Stamina
						RedId = solid[0], YellowId = solid[0], BlueId = solid[0], PrismaticId = solid[0], MetaId = austere },
					new GemmingTemplate() { Model = "TankDK", Group = "Uncommon", //Stamina
						RedId = regal[0], YellowId = enduring[0], BlueId = solid[0], PrismaticId = solid[0], MetaId = austere },
						
					new GemmingTemplate() { Model = "TankDK", Group = "Rare", //Defense 
						RedId = stalwart[1], YellowId = thick[1], BlueId = enduring[1], PrismaticId = thick[1], MetaId = austere },
					new GemmingTemplate() { Model = "TankDK", Group = "Rare", //Dodge
						RedId = subtle[1], YellowId = stalwart[1], BlueId = regal[1], PrismaticId = subtle[1], MetaId = austere },
					new GemmingTemplate() { Model = "TankDK", Group = "Rare", //Max Stamina
						RedId = solid[1], YellowId = solid[1], BlueId = solid[1], PrismaticId = solid[1], MetaId = austere },
					new GemmingTemplate() { Model = "TankDK", Group = "Rare", //Stamina
						RedId = regal[1], YellowId = enduring[1], BlueId = solid[1], PrismaticId = solid[1], MetaId = austere },

					new GemmingTemplate() { Model = "TankDK", Group = "Epic", Enabled = true, //Defense 
						RedId = stalwart[2], YellowId = thick[2], BlueId = enduring[2], PrismaticId = thick[2], MetaId = austere },
					new GemmingTemplate() { Model = "TankDK", Group = "Epic", Enabled = true, //Dodge
						RedId = subtle[2], YellowId = stalwart[2], BlueId = regal[2], PrismaticId = subtle[2], MetaId = austere },
					new GemmingTemplate() { Model = "TankDK", Group = "Epic", Enabled = true, //Max Stamina
						RedId = solid[2], YellowId = solid[2], BlueId = solid[2], PrismaticId = solid[2], MetaId = austere },
					new GemmingTemplate() { Model = "TankDK", Group = "Epic",Enabled = true,  //Stamina
						RedId = regal[2], YellowId = enduring[2], BlueId = solid[2], PrismaticId = nightmare, MetaId = austere },

					new GemmingTemplate() { Model = "TankDK", Group = "Jeweler", //Max Defense
						RedId = thick[3], YellowId = thick[3], BlueId = thick[3], PrismaticId = thick[3], MetaId = austere },
					new GemmingTemplate() { Model = "TankDK", Group = "Jeweler", //Max Dodge
						RedId = subtle[3], YellowId = subtle[3], BlueId = subtle[3], PrismaticId = subtle[3], MetaId = austere },
					new GemmingTemplate() { Model = "TankDK", Group = "Jeweler", //Max Stamina
						RedId = solid[3], YellowId = solid[3], BlueId = solid[3], PrismaticId = solid[3], MetaId = austere },
				};
			}
		}
		#endregion

		#region Intermediate Values
		public float BonusMaxRunicPower = 0f;
		public bool m_bT9_4PC = false;
		#endregion 
		
		public static int HitResultCount = EnumHelper.GetCount(typeof(HitResult));

		#region SubPointColors
		private Dictionary<string, Color> _subPointNameColors = null;
		private Dictionary<string, Color> _subPointNameColors_SMT = new Dictionary<string, Color>();
		private Dictionary<string, Color> _subPointNameColors_Burst = new Dictionary<string, Color>();

		public override Dictionary<string, Color> SubPointNameColors
		{
			get
			{
				if (_subPointNameColors == null)
				{
					return _subPointNameColors_SMT;
				}
				return _subPointNameColors;
			}
		}

		public CalculationsTankDK()
		{
			_subPointNameColors_SMT.Add("Survival", Color.FromArgb(255, 0, 0, 255));
			_subPointNameColors_SMT.Add("Mitigation", Color.FromArgb(255, 255, 0, 0));
			_subPointNameColors_SMT.Add("Threat", Color.FromArgb(255, 0, 255, 0));

			_subPointNameColors_Burst.Add("BurstTime", Color.FromArgb(255, 0, 0, 255));
			_subPointNameColors_Burst.Add("ReactionTime", Color.FromArgb(255, 255, 0, 0));

			_subPointNameColors = _subPointNameColors_SMT;
		}
		#endregion

		private string[] _characterDisplayCalculationLabels = null;
		/// <summary>
		/// An array of strings which will be used to build the calculation display.
		/// Each string must be in the format of "Heading:Label". Heading will be used as the
		/// text of the group box containing all labels that have the same Heading.
		/// Label will be the label of that calculation, and may be appended with '*' followed by
		/// a description of that calculation which will be displayed in a tooltip for that label.
		/// Label (without the tooltip string) must be unique.
		/// 
		/// EXAMPLE:
		/// characterDisplayCalculationLabels = new string[]
		/// {
		///		"Basic Stats:Health",
		///		"Basic Stats:Armor",
		///		"Advanced Stats:Dodge",
		///		"Advanced Stats:Miss*Chance to be missed"
		/// };
		/// </summary>
		public override string[] CharacterDisplayCalculationLabels {
			get {
				if (_characterDisplayCalculationLabels == null) {
					List<string> labels = new List<string>(new string[] {
						@"Summary:Survival Points*Survival Points represents the total raw damage 
(pre-Mitigation) you can take before dying. Unlike 
Mitigation Points, you should not attempt to maximize this, 
but rather get 'enough' of it, and then focus on Mitigation. 
'Enough' can vary greatly by fight and by your healers, but 
keeping it roughly even with Mitigation Points is a good 
way to maintain 'enough' as you progress. If you find that 
you are being killed by burst damage, focus on Survival Points.",
						@"Summary:Mitigation Points*Mitigation Points represent the amount of damage you avoid, 
on average per second, through avoidance stats (Miss, Dodge, Parry) along 
with ways to improve survivablity, +heal or self healing, ability 
cooldowns.  It is directly relative to your Damage Taken. 
Ideally, you want to maximize Mitigation Points, while maintaining 
'enough' Survival Points (see Survival Points). If you find 
yourself dying due to healers running OOM, or being too busy 
healing you and letting other raid members die, then focus on 
Mitigation Points.  Represented in Damage per Second multiplied
by MitigationWeight (seconds).",
						@"Summary:Threat Points*Threat Points represent how much threat is capable for the current 
gear/talent/rotation setup.  Threat points are represented in Threat per second.",
						@"Summary:Overall Points*Overall Points are a sum of Mitigation, Survival and Threat Points. 
Overall is typically, but not always, the best way to rate gear. 
For specific encounters, closer attention to Mitigation or Survival 
Points individually may be important.",

						"Basic Stats:Strength*Should Match in-game value.",
						"Basic Stats:Agility*Should Match in-game value.",
						"Basic Stats:Stamina*Should Match in-game value.",
						"Basic Stats:Attack Power",
						"Basic Stats:Crit Rating",
						"Basic Stats:Hit Rating*Should Match in-game value.",
						"Basic Stats:Expertise",
						"Basic Stats:Haste Rating",
						"Basic Stats:Armor Penetration",
						"Basic Stats:Armor Penetration Rating",
						"Basic Stats:Health*Including Frost Presence",
						"Basic Stats:Armor*Including Frost Presence",

						@"Defense:Crit*Enemy's crit chance on you. When using the optimizer, set a secondary 
criteria to this <= 0 to ensure that you stay defense-soft capped.",
						"Defense:Defense Rating",
						"Defense:Defense",
						"Defense:Resilience",
						"Defense:Defense Rating needed*Including Resilience to ensure being uncrittable.",

						"Advanced Stats:Miss*After Diminishing Returns",
						"Advanced Stats:Dodge*After Diminishing Returns",
						"Advanced Stats:Parry*After Diminishing Returns. Includes Str bonus from Unbreakable Armor's average uptime.",
						"Advanced Stats:Total Avoidance*Miss + Dodge + Parry",
						"Advanced Stats:Armor Damage Reduction",
						"Advanced Stats:Magic Damage Reduction*Currently Magic Resistance Only.",
						"Advanced Stats:Reaction Time*The time healers have to react to a particularly high damage burst before the next potential burst.",
						"Advanced Stats:Burst Time*Enhanced time-to-live calculation that factors avoidance and survival.",

						"Threat Stats:Target Miss*Chance to miss the target",
						"Threat Stats:Target Dodge*Chance the target dodges",
						"Threat Stats:Target Parry*Chance the target parries",
						"Threat Stats:Total Threat*[experimental] Raw Total Threat Generated by the specified rotation",
						"Threat Stats:Threat*Threat Per Second: Total Threat / Rotation Duration",

						"Damage Data:DPS*[experimental] DPS done for given rotation",
						"Damage Data:Rotation Time*[experimental] Duration of the total rotation cycle",
						"Damage Data:RS Limited*[experimental] Is the number of runestrikes specified in the rotation limited in any way",
						"Damage Data:Blood*[experimental] Number of Runes consumed",
						"Damage Data:Frost*[experimental] Number of Runes consumed",
						"Damage Data:Unholy*[experimental] Number of Runes consumed",
						"Damage Data:Death*[experimental] Number of Runes consumed",
						"Damage Data:Runic Power*[experimental] Amount of Runic Power consumed.\nNegative values mean more generated than used.",

					});
					_characterDisplayCalculationLabels = labels.ToArray();
				}
				return _characterDisplayCalculationLabels;
			}
		}
		private string[] _customChartNames = null;
		/// <summary>
		/// The names of all custom charts provided by the model, if any.
		/// </summary>
		public override string[] CustomChartNames {
			get {
				if (_customChartNames == null) {
					_customChartNames = new string[] { };
				}
				return _customChartNames;
			}
		}
		

#if RAWR3
		private ICalculationOptionsPanel _calculationOptionsPanel = null;
		public override ICalculationOptionsPanel CalculationOptionsPanel
		{
			get { return _calculationOptionsPanel ?? (_calculationOptionsPanel = new CalculationOptionsPanelTankDK()); }
		}
#else
		private CalculationOptionsPanelBase _calculationOptionsPanel = null;
		/// <summary>
		/// A custom panel inheriting from CalculationOptionsPanelBase which contains controls for
		/// setting CalculationOptions for the model. CalculationOptions are stored in the Character,
		/// and can be used by multiple models. See comments on CalculationOptionsPanelBase for more details.
		/// </summary>
		public override CalculationOptionsPanelBase CalculationOptionsPanel {
			get { return _calculationOptionsPanel ?? (_calculationOptionsPanel = new CalculationOptionsPanelTankDK()); }
		}
#endif

		private List<ItemType> _relevantItemTypes = null;
		/// <summary>
		/// List<ItemType> containing all of the ItemTypes relevant to this model. Typically this
		/// means all types of armor/weapons that the intended class is able to use, but may also
		/// be trimmed down further if some aren't typically used. ItemType.None should almost
		/// always be included, because that type includes items with no proficiancy requirement, such
		/// as rings, necklaces, cloaks, held in off hand items, etc.

		/// </summary>
		public override List<ItemType> RelevantItemTypes {
			get {
				return _relevantItemTypes ?? (_relevantItemTypes = new List<ItemType>(new ItemType[]
					{
						ItemType.None,
						ItemType.Plate,
						ItemType.Sigil,
						ItemType.Polearm,
						ItemType.TwoHandAxe,
						ItemType.TwoHandMace,
						ItemType.TwoHandSword,
						ItemType.OneHandAxe,
						ItemType.OneHandMace,
						ItemType.OneHandSword
					}));
			}
		}
		/// <summary>
		/// Character class that this model is for.
		/// </summary>
		public override CharacterClass TargetClass { get { return CharacterClass.DeathKnight; } }
		/// <summary>
		/// Method to get a new instance of the model's custom ComparisonCalculation class.
		/// </summary>
		/// <returns>A new instance of the model's custom ComparisonCalculation class, 
		/// which inherits from ComparisonCalculationBase</returns>
		public override ComparisonCalculationBase CreateNewComparisonCalculation() { return new ComparisonCalculationTankDK(); }
		/// <summary>
		/// Method to get a new instance of the model's custom CharacterCalculations class.
		/// </summary>
		/// <returns>A new instance of the model's custom CharacterCalculations class, 
		/// which inherits from CharacterCalculationsBase</returns>
		public override CharacterCalculationsBase CreateNewCharacterCalculations() { return new CharacterCalculationsTankDK(); }
		/// <summary>
		/// An array of strings which define what calculations (in addition to the subpoint ratings)
		/// will be available to the optimizer
		/// </summary>
		public override string[] OptimizableCalculationLabels { 
			get {
				return new string[] {
					"Chance to be Crit",
					"Avoidance %",
					"Damage Reduction %",
					"% Chance to Hit",
					"Target Parry %",
					"Target Dodge %",
					"Armor",
					"Health",
					"Hit Rating",
					"Reaction Time",
					"Burst Time",
                    "Resilience",
                    "Spell Penetration"
				}; 
			} 
		}

        #region Static SpecialEffects
        private static Dictionary<float, SpecialEffect[]> _SE_SpellDeflection = new Dictionary<float,SpecialEffect[]>();
        private static readonly SpecialEffect _SE_T10_4P = new SpecialEffect(Trigger.Use, new Stats() { DamageTakenMultiplier = -0.12f }, 10f, 60f);
        private static readonly SpecialEffect _SE_FC1 = new SpecialEffect(Trigger.DamageDone, new Stats() { BonusStrengthMultiplier = .15f }, 15f, 0f, -2f, 1);
        private static readonly SpecialEffect _SE_FC2 = new SpecialEffect(Trigger.DamageDone, new Stats() { HealthRestoreFromMaxHealth = .03f }, 0, 0f, -2f, 1);
        private static readonly SpecialEffect[][] _SE_VampiricBlood = new SpecialEffect[][] {
            new SpecialEffect[] { new SpecialEffect(Trigger.Use, null, 10 + 0 * 5, 60f - (false ? 10 : 0)), new SpecialEffect(Trigger.Use, null, 10 + 0 * 5, 60f - (true ? 10 : 0)),},
            new SpecialEffect[] { new SpecialEffect(Trigger.Use, null, 10 + 1 * 5, 60f - (false ? 10 : 0)), new SpecialEffect(Trigger.Use, null, 10 + 1 * 5, 60f - (true ? 10 : 0)),},
        };
        private static readonly SpecialEffect[] _SE_RuneTap = new SpecialEffect[] {
            new SpecialEffect(Trigger.Use, null, 0, 60f - 10 * 0),
            new SpecialEffect(Trigger.Use, null, 0, 60f - 10 * 1),
            new SpecialEffect(Trigger.Use, null, 0, 60f - 10 * 2),
            new SpecialEffect(Trigger.Use, null, 0, 60f - 10 * 3),
        };
        private static readonly SpecialEffect[] _SE_BloodyVengeance1 = new SpecialEffect[] {
            null,
            new SpecialEffect(Trigger.DamageSpellCrit, new Stats() { BonusPhysicalDamageMultiplier = .01f * 0 }, 30, 0, 1, 3),
            new SpecialEffect(Trigger.DamageSpellCrit, new Stats() { BonusPhysicalDamageMultiplier = .01f * 1 }, 30, 0, 1, 3),
            new SpecialEffect(Trigger.DamageSpellCrit, new Stats() { BonusPhysicalDamageMultiplier = .01f * 2 }, 30, 0, 1, 3),
            new SpecialEffect(Trigger.DamageSpellCrit, new Stats() { BonusPhysicalDamageMultiplier = .01f * 3 }, 30, 0, 1, 3),
        };
        private static readonly SpecialEffect[] _SE_BloodyVengeance2 = new SpecialEffect[] {
            null,
            new SpecialEffect(Trigger.MeleeCrit, new Stats() { BonusPhysicalDamageMultiplier = .01f * 0 }, 30, 0, 1, 3),
            new SpecialEffect(Trigger.MeleeCrit, new Stats() { BonusPhysicalDamageMultiplier = .01f * 1 }, 30, 0, 1, 3),
            new SpecialEffect(Trigger.MeleeCrit, new Stats() { BonusPhysicalDamageMultiplier = .01f * 2 }, 30, 0, 1, 3),
            new SpecialEffect(Trigger.MeleeCrit, new Stats() { BonusPhysicalDamageMultiplier = .01f * 3 }, 30, 0, 1, 3),
        };
        private static Dictionary<float, SpecialEffect[]> _SE_Bloodworms = new Dictionary<float, SpecialEffect[]>();
        private static readonly SpecialEffect[] _SE_WillOfTheNecropolis = new SpecialEffect[] {
            null,
            new SpecialEffect(Trigger.DamageTaken, new Stats() { DamageTakenMultiplier = -(0.05f * 1) }, 0, 0, 0.35f),
            new SpecialEffect(Trigger.DamageTaken, new Stats() { DamageTakenMultiplier = -(0.05f * 2) }, 0, 0, 0.35f),
            new SpecialEffect(Trigger.DamageTaken, new Stats() { DamageTakenMultiplier = -(0.05f * 3) }, 0, 0, 0.35f),
        };
        private static readonly SpecialEffect[] _SE_IcyTalons = new SpecialEffect[] {
            null,
            new SpecialEffect(Trigger.FrostFeverHit, new Stats() { PhysicalHaste = (0.04f * 1) }, 20f, 0f),
            new SpecialEffect(Trigger.FrostFeverHit, new Stats() { PhysicalHaste = (0.04f * 2) }, 20f, 0f),
            new SpecialEffect(Trigger.FrostFeverHit, new Stats() { PhysicalHaste = (0.04f * 3) }, 20f, 0f),
            new SpecialEffect(Trigger.FrostFeverHit, new Stats() { PhysicalHaste = (0.04f * 4) }, 20f, 0f),
            new SpecialEffect(Trigger.FrostFeverHit, new Stats() { PhysicalHaste = (0.04f * 5) }, 20f, 0f),
        };
        private static readonly SpecialEffect[][] _SE_UnbreakableArmor = new SpecialEffect[][] {
            new SpecialEffect[] {
                    new SpecialEffect(Trigger.Use, new Stats() { BonusStrengthMultiplier = 0.20f, BaseArmorMultiplier = .25f + (false ? .20f : 0f), BonusArmorMultiplier = .25f + (false ? .20f : 0f) }, 20f, 60f - 0 * 10f),
                    new SpecialEffect(Trigger.Use, new Stats() { BonusStrengthMultiplier = 0.20f, BaseArmorMultiplier = .25f + (true  ? .20f : 0f), BonusArmorMultiplier = .25f + (true  ? .20f : 0f) }, 20f, 60f - 0 * 10f),
            },
            new SpecialEffect[] {
                    new SpecialEffect(Trigger.Use, new Stats() { BonusStrengthMultiplier = 0.20f, BaseArmorMultiplier = .25f + (false ? .20f : 0f), BonusArmorMultiplier = .25f + (false ? .20f : 0f) }, 20f, 60f - 1 * 10f),
                    new SpecialEffect(Trigger.Use, new Stats() { BonusStrengthMultiplier = 0.20f, BaseArmorMultiplier = .25f + (true  ? .20f : 0f), BonusArmorMultiplier = .25f + (true  ? .20f : 0f) }, 20f, 60f - 1 * 10f),
            },
        };
        private static readonly SpecialEffect[] _SE_Acclimation = new SpecialEffect[] {
            null,
            new SpecialEffect(Trigger.DamageTakenMagical, new Stats() { FireResistance = 50f, FrostResistance = 50f, ArcaneResistance = 50f, ShadowResistance = 50f, NatureResistance = 50f, }, 18f, 0f, (0.10f * 1), 3),
            new SpecialEffect(Trigger.DamageTakenMagical, new Stats() { FireResistance = 50f, FrostResistance = 50f, ArcaneResistance = 50f, ShadowResistance = 50f, NatureResistance = 50f, }, 18f, 0f, (0.10f * 2), 3),
            new SpecialEffect(Trigger.DamageTakenMagical, new Stats() { FireResistance = 50f, FrostResistance = 50f, ArcaneResistance = 50f, ShadowResistance = 50f, NatureResistance = 50f, }, 18f, 0f, (0.10f * 3), 3),
        };
        private static readonly SpecialEffect _SE_AntiMagicZone = new SpecialEffect(Trigger.Use, new Stats() { SpellDamageTakenMultiplier = -0.75f }, 10f, 2f * 60f);
        #endregion

        /// <summary>
		/// GetCharacterCalculations is the primary method of each model, where a majority of the calculations
		/// and formulae will be used. GetCharacterCalculations should call GetCharacterStats(), and based on
		/// those total stats for the character, and any calculationoptions on the character, perform all the 
		/// calculations required to come up with the final calculations defined in 
		/// CharacterDisplayCalculationLabels, including an Overall rating, and all Sub ratings defined in 
		/// SubPointNameColors.
		/// </summary>
		/// <param name="character">The character to perform calculations for.</param>
		/// <param name="additionalItem">An additional item to treat the character as wearing.
		/// This is used for gems, which don't have a slot on the character to fit in, so are just
		/// added onto the character, in order to get gem calculations.</param>
		/// <returns>A custom CharacterCalculations object which inherits from CharacterCalculationsBase,
		/// containing all of the final calculations defined in CharacterDisplayCalculationLabels. See
		/// CharacterCalculationsBase comments for more details.</returns>
		public override CharacterCalculationsBase GetCharacterCalculations
			(
				Character character, 
				Item additionalItem, 
				bool referenceCalculation, 
				bool significantChange, 
				bool needsDisplayCalculations
			) 
		{
			#region Setup what we need and validate.
			// Since calcs is what we return at the end.  And the caller can't handle null value returns - 
			// Lets only return null if calcs is null, otherwise, let's return an empty calcs on other fails.
			CharacterCalculationsTankDK calcs = new CharacterCalculationsTankDK();
			if (null == calcs) 
			{ 
#if DEBUG
				throw new Exception("Could not generate new CharacterCalculationsTankDK.");
#else
				return null;
#endif
			}

			TankDKChar TDK = new TankDKChar();
			TDK.Char = character;
			// Ok, this is the initial gathering of our information... we haven't processed the multipliers or anything.
			Stats stats = GetCharacterStats(TDK.Char, additionalItem);
			// validate that we get a stats object;
			if (null == stats) { return calcs; }

			// Apply the Multipliers
			ProcessStatModifiers(stats, TDK.Char.DeathKnightTalents.BladedArmor);

			// Import the option values from the options tab on the UI.
			TDK.opts = character.CalculationOptions as CalculationOptionsTankDK;
			// Validate opts 
			if (null == TDK.opts) { return calcs; }
			// Get the shotrotation/combat model here.
			if (TDK.opts.m_Rotation == null) { return calcs; }

			// Level differences.
			int iTargetLevel = TDK.opts.TargetLevel;
			if (TDK.opts.bExperimental)
			{
				iTargetLevel = TDK.opts.hCurrentBoss.Level;
			}
			int iLevelDiff = iTargetLevel - character.Level;
			float fLevelDiffModifier = iLevelDiff * 0.2f;

			// Apply the ratings to actual stats.
			ProcessRatings(stats);

			// Need to save off the base avoidance stats before having their ratings applied to them.
			float fBaseDodge = stats.Dodge;
			float fBaseParry = stats.Parry;
			float fBaseDef = stats.Defense;
			float fBaseMiss = stats.Miss;

			ProcessAvoidance(stats, iTargetLevel);

			#endregion

			float fChanceToGetHit = 1f - (stats.Miss + stats.Dodge);
			if (TDK.Char.MainHand != null || TDK.Char.OffHand != null)
			{
				fChanceToGetHit -= stats.Parry;
			}

			#region TargetDodge/Parry/Miss & Expertise - finish populating totalstats.
			bool bDualWielding = false;
			float f2hWeaponDamageMultiplier = 0f;
			float hitChance = 0;
			float chanceTargetParry = StatConversion.WHITE_PARRY_CHANCE_CAP[iLevelDiff];
			float chanceTargetDodge = StatConversion.WHITE_DODGE_CHANCE_CAP[iLevelDiff];
			float chanceTargetMiss = StatConversion.WHITE_MISS_CHANCE_CAP[iLevelDiff];
			if (character.MainHand != null) 
			{
				// 2-hander weapon specialization.
				if (character.MainHand.Slot == ItemSlot.TwoHand)
				{
					f2hWeaponDamageMultiplier = (0.02f * TDK.Char.DeathKnightTalents.TwoHandedWeaponSpecialization);
				}
				else
				{
					// Toon is not using a 2h, meaning that he's DW if he's got something in his off hand.
					bDualWielding = (TDK.Char.OffHand != null && TDK.Char.MainHand != null);
				}
				// 8% default miss rate vs lvl 83
				chanceTargetMiss = Math.Max(0f, StatConversion.WHITE_MISS_CHANCE_CAP[iLevelDiff] - stats.PhysicalHit);
				if (bDualWielding) 
				{
					// Talent: Nerves of Cold Steel
					// +hit changes only.  See damage buff change further down.
					chanceTargetMiss = (StatConversion.WHITE_MISS_CHANCE_CAP_DW[iLevelDiff]
									 - (0.01f * TDK.Char.DeathKnightTalents.NervesOfColdSteel) - stats.PhysicalHit);
				}
				if (TDK.Char.Race == CharacterRace.Dwarf &&
					(TDK.Char.MainHand.Type == ItemType.TwoHandMace || TDK.Char.MainHand.Type == ItemType.OneHandMace)) 
				{
					stats.Expertise += 5;
				}
				if (TDK.Char.Race == CharacterRace.Human &&
					(TDK.Char.MainHand.Type == ItemType.TwoHandMace || TDK.Char.MainHand.Type == ItemType.OneHandMace ||
					TDK.Char.MainHand.Type == ItemType.TwoHandSword || TDK.Char.MainHand.Type == ItemType.OneHandSword)) 
				{
					stats.Expertise += 3;
				}
				if (TDK.Char.Race == CharacterRace.Orc &&
					(TDK.Char.MainHand.Type == ItemType.TwoHandAxe || TDK.Char.MainHand.Type == ItemType.OneHandAxe)) 
				{
					stats.Expertise += 5;
				}
			}

			chanceTargetParry = Math.Max(0.0f, StatConversion.WHITE_PARRY_CHANCE_CAP[iLevelDiff] - StatConversion.GetDodgeParryReducFromExpertise(stats.Expertise));
			chanceTargetDodge = Math.Max(0.0f, StatConversion.WHITE_DODGE_CHANCE_CAP[iLevelDiff] - StatConversion.GetDodgeParryReducFromExpertise(stats.Expertise));
			hitChance = 1.0f - (chanceTargetMiss + chanceTargetDodge + chanceTargetParry);
			// Can't have more than 100% hit chance.
			hitChance = Math.Min(1f, hitChance);
			
			#endregion

			// need to calculate the rotation after we have the DR values for Dodge/Parry/Miss/haste.
			TDK.opts.m_Rotation.m_fDodge = stats.Dodge;
			TDK.opts.m_Rotation.m_fParry = stats.Parry;
			TDK.opts.m_Rotation.m_fPhysicalHaste = stats.PhysicalHaste;

			if (character.DeathKnightTalents.SpellDeflection > 0) {
                float key = (float)Math.Round(stats.Parry, 5);
                if (!_SE_SpellDeflection.ContainsKey(key)) {
                    _SE_SpellDeflection.Add(key, new SpecialEffect[] {
                        null, // this array point will never get selected because the opening If statement prevents it
                        new SpecialEffect(Trigger.DamageSpellHit, new Stats() { SpellDamageTakenMultiplier = -0.15f * 1 }, 0f, 0f, stats.Parry),
                        new SpecialEffect(Trigger.DamageSpellHit, new Stats() { SpellDamageTakenMultiplier = -0.15f * 2 }, 0f, 0f, stats.Parry),
                        new SpecialEffect(Trigger.DamageSpellHit, new Stats() { SpellDamageTakenMultiplier = -0.15f * 3 }, 0f, 0f, stats.Parry),
                    });
                }
                stats.AddSpecialEffect(_SE_SpellDeflection[key][TDK.Char.DeathKnightTalents.SpellDeflection]);
			}

			// This is the point that SHOULD have the right values according to the paper-doll.
			Stats sPaperDoll = stats.Clone();

			CombatTable ct = new CombatTable(TDK.Char, calcs, stats, TDK.opts);
			// Setup for new combat table using the new ability objects.
			CombatTable2 ct2 = new CombatTable2(TDK.Char, stats, calcs, TDK.opts);
			// Now that we have the combat table, we should be able to integrate the Special effects.
			// However, the special effects will modify the incoming stats for all aspects, so we have 
			// ensure that as we iterate, we don't count whole sets of stats twice.

			#region T10 4PC
			// T10 4PC bonus:
			if (stats.TankDK_T10_4pc != 0)
			{
				// Blood Tap:
				// 6% of base health Instant 1 min cooldown
				// Turns a blood rune to Death rune
				// Blood Armor:
				// When you activate Blood Tap, you gain 12% damage reduction from all attacks for 10 sec.
				// For now, we're going to assume that Blood Tap is used at every opportunity.
                stats.AddSpecialEffect(_SE_T10_4P);
			}
			#endregion 

			// Filter out the duplicate Runes:
			if (character.OffHandEnchant == Enchant.FindEnchant(3368, ItemSlot.OneHand, character)
				&& character.MainHandEnchant == character.OffHandEnchant)
			{
				bool bFC1Found = false;
				bool bFC2Found = false;
				foreach (SpecialEffect se1 in stats.SpecialEffects())
				{
					// if we've already found them, and we're seeing them again, then remove these repeats.
					if (bFC1Found && se1.Equals(_SE_FC1))
						stats.RemoveSpecialEffect(se1);
					else if (bFC2Found && se1.Equals(_SE_FC2))
						stats.RemoveSpecialEffect(se1);
					else if (se1.Equals(_SE_FC1))
						bFC1Found = true;
					else if (se1.Equals(_SE_FC2))
						bFC2Found = true;
				}
			}

			#region Special Effects
			// For now we just factor them in once.
			StatsSpecialEffects sse = new StatsSpecialEffects(character, stats, ct);
			Stats statSE = new Stats();
			foreach (SpecialEffect e in stats.SpecialEffects())
			{
				// There are some multi-level special effects that need to be factored in.
				foreach (SpecialEffect ee in e.Stats.SpecialEffects())
				{
					e.Stats = sse.getSpecialEffects(TDK.opts, ee);
				}
				statSE.Accumulate(sse.getSpecialEffects(TDK.opts, e));
			}
			// Darkmoon card greatness procs
			if (statSE.HighestStat > 0 || statSE.Paragon > 0)
			{
				if (statSE.Strength >= statSE.Agility) { statSE.Strength += statSE.HighestStat + statSE.Paragon; }
				else if (statSE.Agility > statSE.Strength) { statSE.Agility += statSE.HighestStat + statSE.Paragon; }
				statSE.HighestStat = 0;
				statSE.Paragon = 0;
			}

			// Any Modifiers from stats need to be applied to statSE
			statSE.Strength = StatConversion.ApplyMultiplier(statSE.Strength, stats.BonusStrengthMultiplier);
			statSE.Agility = StatConversion.ApplyMultiplier(statSE.Agility, stats.BonusAgilityMultiplier);
			statSE.Stamina = StatConversion.ApplyMultiplier(statSE.Stamina, stats.BonusStaminaMultiplier);
//            statSE.Stamina = (float)Math.Floor(statSE.Stamina);
			statSE.Armor = StatConversion.ApplyMultiplier(statSE.Armor, stats.BaseArmorMultiplier);
			statSE.AttackPower = StatConversion.ApplyMultiplier(statSE.AttackPower, stats.BonusAttackPowerMultiplier);
			statSE.BonusArmor = StatConversion.ApplyMultiplier(statSE.BonusArmor, stats.BonusArmorMultiplier);

			float AgiArmor = StatConversion.GetArmorFromAgility(statSE.Agility); // Don't multiply the armor from agility.
			statSE.Armor += statSE.BonusArmor + AgiArmor;
			statSE.Health += StatConversion.GetHealthFromStamina(statSE.Stamina) + statSE.BattlemasterHealth;
			StatConversion.ApplyMultiplier(statSE.Health, stats.BonusHealthMultiplier);
			if (character.DeathKnightTalents.BladedArmor > 0)
			{
				statSE.AttackPower += (statSE.Armor / 180f) * (float)character.DeathKnightTalents.BladedArmor;
			}
			statSE.AttackPower += StatConversion.ApplyMultiplier((statSE.Strength * 2), stats.BonusAttackPowerMultiplier);
			statSE.ParryRating += statSE.Strength * 0.25f;

			// Any Modifiers from statSE need to be applied to stats
			stats.Strength = StatConversion.ApplyMultiplier(stats.Strength, statSE.BonusStrengthMultiplier);
			stats.Agility = StatConversion.ApplyMultiplier(stats.Agility, statSE.BonusAgilityMultiplier);
			stats.Stamina = StatConversion.ApplyMultiplier(stats.Stamina, statSE.BonusStaminaMultiplier);
//            stats.Stamina = (float)Math.Floor(stats.Stamina);
			stats.Armor = StatConversion.ApplyMultiplier(stats.Armor, statSE.BaseArmorMultiplier);
			stats.AttackPower = StatConversion.ApplyMultiplier(stats.AttackPower, statSE.BonusAttackPowerMultiplier);
			stats.BonusArmor = StatConversion.ApplyMultiplier(stats.BonusArmor, statSE.BonusArmorMultiplier);

			// Refresh the base avoidance values
			stats.Dodge = fBaseDodge;
			stats.Parry = fBaseParry;
			stats.Defense = fBaseDef;
			stats.Miss = fBaseMiss;

			stats.Accumulate(statSE);

			#endregion // Special effects 

			// refresh avoidance w/ the new stats.
			float[] fAvoidance = new float[HitResultCount];
			for (uint i = 0; i < HitResultCount; i++)
			{
				// GetDRAvoidanceChance returns a dec. percentage.
				// Since CurrentAvoidance is a percent, need to multiply by 100.
				fAvoidance[i] = (StatConversion.GetDRAvoidanceChance(character, stats, (HitResult)i, iTargetLevel));
			}

			// So let's populate the miss, dodge and parry values pulling them out of the avoidance number.
			fChanceToGetHit = 1f;
			stats.Miss = Math.Min((StatConversion.CAP_MISSED[(int)CharacterClass.DeathKnight]/100), fAvoidance[(int)HitResult.Miss]);
			stats.Miss = Math.Max(0, stats.Miss);
			fChanceToGetHit -= stats.Miss;
			// Dodge needs to be factored in here.
			stats.Dodge = Math.Min((StatConversion.CAP_DODGE[(int)CharacterClass.DeathKnight]/100), fAvoidance[(int)HitResult.Dodge]);
			stats.Dodge = Math.Max(stats.Dodge, 0);
			fChanceToGetHit -= stats.Dodge;
			// Pary factors
			stats.Parry = Math.Min((StatConversion.CAP_PARRY[(int)CharacterClass.DeathKnight]/100), fAvoidance[(int)HitResult.Parry]);
			stats.Parry = Math.Max(stats.Parry, 0);

			if (character.MainHand != null || character.OffHand != null) 
			{ 
				fChanceToGetHit -= stats.Parry; 
			}

			float fChanceToGetCrit = fAvoidance[(int)HitResult.Crit];
			// The next call expect Defense rating to NOT be factored into the defense stat
			calcs.DefenseRatingNeeded = StatConversion.GetDefenseRatingNeeded(character, stats, iTargetLevel);

			stats.Defense += StatConversion.GetDefenseFromRating(stats.DefenseRating, character.Class);

			// 5% + Level difference crit chance.
			// Level difference is already factored in above.
			float fBaseCritChance = .05f;
			// Be sure that we don't have a negative chance to get crit.
			fChanceToGetCrit = Math.Max(0.0f, (fBaseCritChance - fChanceToGetCrit));
			float fPercentCritMitigation = 1f - (fChanceToGetCrit / fBaseCritChance);

			// refresh Combat table w/ the new stats.
			ct = new CombatTable(character, calcs, stats, TDK.opts);
			if (TDK.opts.bExperimental)
			{
				// Setup for new combat table using the new ability objects.
				ct2 = new CombatTable2(character, stats, calcs, TDK.opts);
			}

			#region Talents with general reach that aren't already in stats.
			#region Talent: Bone Shield
			// Talent: Bone Shield 
			float bsDR = 0.0f;
			float bsUptime = 0f;
			if (character.DeathKnightTalents.BoneShield > 0) 
			{
				uint BSStacks = 3;  // The number of bones by default.
				if (character.DeathKnightTalents.GlyphofBoneShield == true) { BSStacks += 2; }

				float fBSCD = 60f;
				if (m_bT9_4PC) fBSCD -= 10f;

				bsUptime = Math.Min(1f,                         // Can't be up for longer than 100% of the time. 
							(BSStacks * 2f)                   // 2 sec internal cooldown on loosing bones so the DK can't get spammed to death. 
							/ (1 - fChanceToGetHit)   // Loose a bone every time we get hit.
							/ fBSCD);                          // 60 sec cooldown.
				// 20% damage reduction while active.
				bsDR = 0.2f * bsUptime;
			}
			stats.DamageTakenMultiplier -= bsDR;
			#endregion

			#region Talent: Vampiric Blood
			// Talent: Vampiric Blood
			if (character.DeathKnightTalents.VampiricBlood > 0)
			{
				Stats VBStats = new Stats() { Health = (stats.Health * 0.15f), HealingReceivedMultiplier = 0.35f, };
                float uptime = _SE_VampiricBlood[character.DeathKnightTalents.GlyphofVampiricBlood ? 1 : 0][m_bT9_4PC ? 1 : 0].GetAverageUptime(0f, 1f);
                stats.Accumulate(VBStats, uptime);
			}
			#endregion

			#region Talent: RuneTap
			// Talent: Rune Tap
			if (character.DeathKnightTalents.RuneTap > 0)
			{
                // Improved Rune Tap.
                // increases the health provided by RT by 33% per point. and lowers the CD by 10 sec per point
                Stats newStats = new Stats() { Healed = (stats.Health * 0.10f) * (1f + (character.DeathKnightTalents.ImprovedRuneTap / 3f)) };
                float uptime = _SE_RuneTap[character.DeathKnightTalents.ImprovedRuneTap].GetAverageUptime(0f, 1f);
                stats.Accumulate(newStats, uptime);
			}
			#endregion
			#endregion

			// Assuming the Boss has no ArPen
			// From http://www.skeletonjack.com/2009/05/14/dk-tanking-armor-cap/#comments
			// 75% armor cap.  Not sure if this is for DK or for all Tanks.  So I'm just going to handle it here.
			// I'll do more research and see if it needs to go into the general function.
			float ArmorDamageReduction = (float)Math.Min(0.75f, StatConversion.GetArmorDamageReduction(iTargetLevel, stats.Armor, 0f, 0f, 0f));

			#region Setup Fight parameters

			float fFightDuration = TDK.opts.FightLength;
			if (fFightDuration == 0f)
			{
				TDK.opts.FightLength = fFightDuration = 10f;
			}
			// Does the boss have parry haste?
			bool bParryHaste = TDK.opts.bParryHaste;

			// Get the values of each type of damage in %.
			// So first we get each type of damage in the same units: DPS.
			float fPhyDamageDPS = GetDPS(TDK.opts.IncomingDamage, TDK.opts.BossAttackSpeed * 1.14f);
			float fBleedDamageDPS = GetDPS(TDK.opts.IncomingBleedDamage, TDK.opts.BleedTickFrequency);
			float fMagicDamageDPS = GetDPS(TDK.opts.IncomingMagicDamage, TDK.opts.IncomingFromMagicFrequency);
			// Get the total DPS.
			float fTotalDPS = fPhyDamageDPS + fBleedDamageDPS + fMagicDamageDPS;
			// Factor the segments out.
			float fPhyDamPercent = fPhyDamageDPS / fTotalDPS;
			float fBleedDamPercent = fBleedDamageDPS / fTotalDPS;
			float fMagicDamPercent = fMagicDamageDPS / fTotalDPS;

			float fTotalMitigation = 0f;

			#endregion

			// We want to start getting the Boss Handler stuff going on.
			#region ***** Boss Handler *****
			// Setup initial Boss data.
			BossHandler hCurrentBoss = new BossHandler();
			if (TDK.opts.bExperimental)
			{
				hCurrentBoss = TDK.opts.hCurrentBoss;                
				// How much of what kind of damage does this boss deal with?
				#region ** Incoming Boss Damage **
				int uAttackCount = hCurrentBoss.Attacks.Count;
				if (uAttackCount <= 0)
				{
					// Error
				}
				else
				{
					fPhyDamageDPS = hCurrentBoss.GetDPSByType(ATTACK_TYPES.AT_MELEE, 0, 0, stats.Miss, stats.Dodge, stats.Parry, 0, 0);
					foreach (Attack a in hCurrentBoss.Attacks)
					{
						if (a.IgnoresAllTanks == false)
						{
							// Bleeds vs Magic vs Physical
							if (a.DamageType == ItemDamageType.Physical)
							{
								// Bleed or Physical
								// Need to figure out how to determine bleed vs. physical hits.
								// Also need to balance out the physical hits and balance the hit rate.
								if (a.Avoidable)
								{
									fPhyDamageDPS = GetDPS(a.DamagePerHit, a.AttackSpeed);
								}
								else
								{
									fBleedDamageDPS = GetDPS(a.DamagePerHit, a.AttackSpeed);
								}
							}
							else
							{
								// Magic
								fMagicDamageDPS = GetDPS(a.DamagePerHit, a.AttackSpeed);
							}
						}
					}
				}
				#endregion

				#region Fight Settings
				// Set the Fight Duration to no larger than the Berserk Timer
				// Question: What is the units for Berserk & Speed Timer? MS/S/M?
				fFightDuration = Math.Min(hCurrentBoss.BerserkTimer, fFightDuration);
				bParryHaste = hCurrentBoss.DefaultMeleeAttack != null ? hCurrentBoss.DefaultMeleeAttack.UseParryHaste : false;
				#endregion 
			}
			#endregion

			#region ***** Survival Rating *****
			// For right now Survival Rating == Effective Health will be HP + Armor/Resistance mitigation values.
			// Everything else is really mitigating damage based on RNG.

			// The health bonus from Frost presence is now include in the character by default.
			float fPhysicalSurvival = stats.Health;
			float fBleedSurvival = stats.Health;
			float fMagicalSurvival = stats.Health;
				
			// Physical damage:
			fPhysicalSurvival = GetEffectiveHealth(stats.Health, ArmorDamageReduction, fPhyDamPercent);

			// Bleed damage:
			fBleedSurvival = GetEffectiveHealth(stats.Health, 0, fBleedDamPercent);

			// Magical damage:
			// if there is a max resistance, then it's likely they are stacking for that resistance.  So factor in that Max resistance.
			float fMaxResist = Math.Max(stats.ArcaneResistance, stats.FireResistance);
			fMaxResist = Math.Max(fMaxResist, stats.FrostResistance);
			fMaxResist = Math.Max(fMaxResist, stats.NatureResistance);
			fMaxResist = Math.Max(fMaxResist, stats.ShadowResistance);

			float fMagicDR = StatConversion.GetAverageResistance(iTargetLevel, character.Level, fMaxResist, 0f);
			calcs.MagicDamageReduction = fMagicDR;
			fMagicalSurvival = GetEffectiveHealth(stats.Health, fMagicDR, fMagicDamPercent);

			float fEffectiveHealth = fPhysicalSurvival + fBleedSurvival + fMagicalSurvival;
			// EffHealth is used further down for Burst/Reaction Times.
			calcs.PhysicalSurvival = fPhysicalSurvival;
			calcs.BleedSurvival = fBleedSurvival;
			calcs.MagicSurvival = fMagicalSurvival;
			calcs.Survival = fEffectiveHealth;
			calcs.SurvivalWeight = TDK.opts.SurvivalWeight;
			#endregion

			#region ***** Threat Rating *****
			float fRotDuration = ct.calcOpts.m_Rotation.getRotationDuration();
			float DSperSec = 0;
			float fThreatTotal = 0f;
			float fThreatPS = 0f;

			fThreatTotal = ct.GetTotalThreat();
			if (TDK.opts.bExperimental)
			{
				// Setup for new combat table using the new ability objects.
				fThreatTotal = ct2.m_TPS;
				fRotDuration = (float)ct2.m_RotationDuration / 1000;
				calcs.RotationTime = ct2.m_RotationDuration;
				fThreatPS = ct2.m_TPS;
				calcs.DPS = ct2.m_DPS;
				DSperSec = ct2.m_Rotation.DeathStrike / fRotDuration;
			}
			else if (fRotDuration > 0)
			{
				calcs.RotationTime = fRotDuration * 1000;
				DSperSec = ct.calcOpts.m_Rotation.DeathStrike / fRotDuration;
				fThreatPS = fThreatTotal / fRotDuration;
			}

			calcs.Threat = fThreatPS;
			// Improved Blood Presence
			if (character.DeathKnightTalents.ImprovedBloodPresence > 0)
			{
				if (TDK.opts.bExperimental)
				{
					float fDamageDone = ct2.m_DPS;
					stats.Healed += (fDamageDone * 0.02f * character.DeathKnightTalents.ImprovedBloodPresence);
				}
				else
				{
					float fDamageDone = fThreatPS / 2.035f; // reducing the TPS by the multiplier for Frost presence for basic DPS number - not the most accurate, but it gets us closer.
					stats.Healed += (fDamageDone * 0.02f * character.DeathKnightTalents.ImprovedBloodPresence);
				}
			}

			// Factor in damage procs.
			float fDamageFromProcs = stats.ArcaneDamage + stats.FireDamage + stats.FrostDamage + stats.ShadowDamage + stats.NatureDamage + stats.HolyDamage;
			calcs.Threat += fDamageFromProcs;

			if (TDK.opts.bExperimental)
			{
				calcs.DPS += fDamageFromProcs;
				calcs.RSLimit = ct2.m_iRSState;

				calcs.Blood = ct2.m_BloodRunes;
				calcs.Frost = ct2.m_FrostRunes;
				calcs.Unholy = ct2.m_UnholyRunes;
				calcs.Death = ct2.m_DeathRunes;
				calcs.RP = ct2.m_RunicPower;
				calcs.TotalThreat = (int)ct2.TotalThreat;
			}
			else
			{
				calcs.Blood = ct.m_BloodRunes;
				calcs.Frost = ct.m_FrostRunes;
				calcs.Unholy = ct.m_UnholyRunes;
				calcs.Death = ct.m_DeathRunes;
			}

			// Threat buffs.
			calcs.Threat *= 1f + (stats.ThreatIncreaseMultiplier - stats.ThreatReductionMultiplier);
			calcs.ThreatWeight = TDK.opts.ThreatWeight;
			#endregion

			#region ***** Mitigation Rating *****
			float fSegmentMitigation = 0f;

			#region ** Crit Mitigation **
			// Crit mitigation:
			// Crit mitigation works for Magical as well as Physical damage so take care of that first.
			float fCritMultiplier = 1;
			// Bleeds can't crit.
			// Neither can spells from bosses.  (As per a Loading screen ToolTip.)
			float fCritDPS = (fPhyDamageDPS) * fCritMultiplier;
			fSegmentMitigation = (fCritDPS * fPercentCritMitigation);
			// Add in the value of crit mitigation.
			fTotalMitigation += fSegmentMitigation;
			// The max damage at this point needs to include crit.
			float fMaxIncDPS = fTotalDPS + fCritDPS - fSegmentMitigation;
			#endregion

			// How much damage per shot normal shot?
			float fPerShotPhysical = TDK.opts.IncomingDamage;

			#region ** Haste Mitigation **
			// Placeholder for comparing differing DPS values related to haste.
			float fNewIncPhysDPS = 0;
			// Let's just look at Imp Icy Touch 
			#region Improved Icy Touch
			// Get the new slowed AttackSpeed based on ImpIcyTouch
			// Factor in the base slow caused by FF (14% base).
			float fBossAverageAttackSpeed = TDK.opts.BossAttackSpeed * 1.14f * (1f + (.02f * character.DeathKnightTalents.ImprovedIcyTouch));
			// Figure out what the new Physical DPS should be based on that.
			fNewIncPhysDPS = GetDPS(fPerShotPhysical, fBossAverageAttackSpeed);
			// Send the difference to the Mitigation value.
			fSegmentMitigation = fPhyDamageDPS - fNewIncPhysDPS;
			fTotalMitigation += fSegmentMitigation;
			if (TDK.opts.AdditiveMitigation)
			{
				// Lets' remove the Damage that was avoided.
				fPhyDamageDPS -= fSegmentMitigation;
			}
			#endregion

			// we don't have to do this work unless we are working out parry haste since we already have the current DPS.
			#region Parry Haste
			if (bParryHaste)
			{
				if (fFightDuration == 0f)
				{
					TDK.opts.FightLength = fFightDuration = 10f;
				}
				float fNumRotations = 0f;

				// How many shots over the length of the fight?
				float fTotalBossAttacksPerFight = (fFightDuration * 60f) / fBossAverageAttackSpeed;
				// Integrate Expertise values to prevent additional physical damage coming in:
				// Each parry reducing swing timer by up to 40% so we'll average that damage increase out.
				// Each parry is factored by weapon speed - the faster the weapons, the more likely the boss can parry.
				// Figure out how many shots there are.  Right now, just calculating white damage.
				// How fast is a hasted shot? up to 40% faster.
				// average based on parry haste being equal to Math.Min(Math.Max(timeRemaining-0.4,0.2),timeRemaining)
				float fBossShotCountPerRot = 0f;
				if (fRotDuration > 0)
				{
					fNumRotations = (fFightDuration * 60f) / fRotDuration;
					// How many shots does the boss take over a given rotation period.
					fBossShotCountPerRot = fRotDuration / fBossAverageAttackSpeed;
					float fCharacterShotCount = 0f;
					if (character.MainHand != null && ct.MH.hastedSpeed > 0f)
					{
						fCharacterShotCount += (fRotDuration / ct.MH.hastedSpeed);
					}
					if (ct.m_bDW || character.MainHand == null && character.OffHand != null && ct.OH.hastedSpeed > 0f)
					{
						fCharacterShotCount += (fRotDuration / ct.OH.hastedSpeed);
					}
					fCharacterShotCount += ct.totalParryableAbilities;

					#region Max Parry-Hasted Damage
//                    fPhyDamageDPS = GetParryHastedDPS(StatConversion.WHITE_PARRY_CHANCE_CAP[iLevelDiff], fCharacterShotCount, fBossAverageAttackSpeed, fRotDuration, fPerShotPhysical);
					float fMaxHastedBossAttackSpeed = GetParryHastedAttackSpeed(StatConversion.WHITE_PARRY_CHANCE_CAP[iLevelDiff], fCharacterShotCount, fBossAverageAttackSpeed, fRotDuration);
					float fMaxPhyDamageDPS = GetDPS(fPerShotPhysical, fMaxHastedBossAttackSpeed);
					#endregion

					#region Actual Parry-haste for this character
					// Now, what's the actual expertise-based hasted damage?
//                    fNewIncPhysDPS = GetParryHastedDPS(chanceTargetParry, fCharacterShotCount, fBossAverageAttackSpeed, fRotDuration, fPerShotPhysical);
					fBossAverageAttackSpeed = GetParryHastedAttackSpeed(chanceTargetParry, fCharacterShotCount, fBossAverageAttackSpeed, fRotDuration);
					fNewIncPhysDPS = GetDPS(fPerShotPhysical, fBossAverageAttackSpeed);
					#endregion

					// Still need to translate this to how much is mitigated by Expertise.
					fSegmentMitigation = fMaxPhyDamageDPS - fNewIncPhysDPS;

					fTotalMitigation += fSegmentMitigation;
					if (TDK.opts.AdditiveMitigation)
					{
						// Lets' remove the Damage that was avoided.
						fPhyDamageDPS -= fSegmentMitigation;
					}
				}
			}
			#endregion
			#endregion

			#region ** Mark of Blood **
			// Mark of blood
			// Cast on the enemy
			// buff that lasts 20 secs or 20 hits
			// heals the target for 4% of max health for each damage dealing hit from that enemy to the target of that enemy.
			// 3 Min CD.
			if (character.DeathKnightTalents.MarkOfBlood > 0)
			{
				// Now that we have the Avg. Boss Attack speed, let's figure how many attacks in 20 secs.
				float AttacksFor20 = Math.Min(20f, 20f / fBossAverageAttackSpeed);
				float MOBhealing = stats.Health * .04f * (AttacksFor20 * fChanceToGetHit); // how many attacks get through avoidance.
				float MoB_HPS = (MOBhealing * Math.Max(1f, fFightDuration / 3)) / (fFightDuration * 60); // Fire it off every time we can and at least once per fight.
				stats.Healed += MoB_HPS;
			}
			#endregion

			#region ** Avoidance Mitigation **
			// Let's see how much damage was avoided.
			float fAvoidanceTotal = 1 - fChanceToGetHit;
			// Raise the total mitgation by that amount.
			fSegmentMitigation = fPhyDamageDPS * Math.Min(1f, fAvoidanceTotal);
			fTotalMitigation += fSegmentMitigation;
			if (TDK.opts.AdditiveMitigation)
			{
				// Lets' remove the Damage that was avoided.
				fPhyDamageDPS -= fSegmentMitigation;
			}
			#endregion

			#region ** Anti-Magic Shell **
			// Anti-Magic Shell. ////////////////////////////////////////////////////////
			// Talent: MagicSuppression increases AMS by 8/16/25% per point.
			// Glyph: GlyphofAntiMagicShell increases AMS by 2 sec.
			// AMS has a 45 sec CD.
			float amsDuration = (5f + (character.DeathKnightTalents.GlyphofAntiMagicShell == true ? 2f : 0f));
			float amsUptimePct = amsDuration / 45f;
			// AMS reduces damage taken by 75% up to a max of 50% health.
			float amsReduction = 0.75f * (1f + character.DeathKnightTalents.MagicSuppression * 0.08f + (character.DeathKnightTalents.MagicSuppression == 3 ? 0.01f : 0f));
			float amsReductionMax = stats.Health * 0.5f;
			// up to 50% of health means that the amdDRvalue equates to the raw damage points removed.  
			// This means that toon health and INC damage values from the options pane are going to affect this quite a bit.
			float amsDRvalue = (Math.Min(amsReductionMax, (fMagicDamageDPS * amsDuration) * amsReduction) * amsUptimePct);
			// Raise the TotalMitigation by that amount.
			fTotalMitigation += amsDRvalue;
			if (TDK.opts.AdditiveMitigation)
			{
				// lower the Magical DPS by the AMSDRValue
				fPhyDamageDPS -= amsDRvalue;
			}
			#endregion

			#region ** Armor Damage Mitigation **
			// For any physical only damage reductions. 
			// Factor in armor Damage Reduction
			fTotalMitigation += fPhyDamageDPS * ArmorDamageReduction;
			if (TDK.opts.AdditiveMitigation)
			{
				fPhyDamageDPS -= fPhyDamageDPS * ArmorDamageReduction;
			}
			#endregion

			#region ** Resistance Damage Mitigation **
			// For any physical only damage reductions. 
			// Factor in armor Damage Reduction
			fTotalMitigation += fMagicDamageDPS * fMagicDR;
			if (TDK.opts.AdditiveMitigation)
			{
				fMagicDamageDPS -= fMagicDamageDPS * fMagicDR;
			}
			#endregion

			// Four T8 : AMS grants 10% damage reduction.
			stats.DamageTakenMultiplier -= (stats.BonusAntiMagicShellDamageReduction * amsUptimePct);

			#region ** Damage Taken Mitigation **
			fSegmentMitigation = Math.Abs(fMagicDamageDPS * stats.SpellDamageTakenMultiplier);
			fSegmentMitigation += Math.Abs(fMagicDamageDPS * stats.DamageTakenMultiplier);
			fSegmentMitigation += Math.Abs(fBleedDamageDPS * stats.DamageTakenMultiplier);
			fSegmentMitigation += Math.Abs(fPhyDamageDPS * stats.DamageTakenMultiplier);
			fTotalMitigation += fSegmentMitigation;
			#endregion

			#region ** Damage Absorbed Mitigation **
			fTotalMitigation += stats.DamageAbsorbed;
			#endregion

			// Let's make sure we don't go into negative damage here
			fMagicDamageDPS = Math.Max(0f, fMagicDamageDPS);
			fPhyDamageDPS = Math.Max(0f, fPhyDamageDPS);

			#region ** Burst/Reaction Time **
			// The next 2 returns are in swing count.
			float fReactionSwingCount = GetReactionTime(fAvoidanceTotal);
			float fBurstSwingCount = GetBurstTime(fAvoidanceTotal, fEffectiveHealth, TDK.opts.IncomingDamage);

			// Get how long that actually will be on Average.
			calcs.ReactionTime = fReactionSwingCount * fBossAverageAttackSpeed;
			calcs.BurstTime = fBurstSwingCount * fBossAverageAttackSpeed;

			// Total damage avoided between bursts.
			//            float fBurstDamage = fBurstSwingCount * fPerShotPhysical;
			//            float fBurstDPS = fBurstDamage / fBossAverageAttackSpeed;
			//            float fReactionDamage = fReactionSwingCount * fPerShotPhysical;
			#endregion

			// Mitigation is the difference between what damage would have been before and what it is once you factor in mitigation effects.
			fSegmentMitigation = 0;
			fSegmentMitigation += StatConversion.ApplyMultiplier(stats.Healed, stats.HealingReceivedMultiplier);
			fSegmentMitigation += (StatConversion.ApplyMultiplier(stats.Hp5, stats.HealingReceivedMultiplier) / 5);
			fSegmentMitigation += StatConversion.ApplyMultiplier(stats.HealthRestore, stats.HealingReceivedMultiplier);
			// Health Returned by DS and other sources:
			if (DSperSec > 0)
				fSegmentMitigation += StatConversion.ApplyMultiplier((stats.HealthRestoreFromMaxHealth * stats.Health) * DSperSec, stats.HealingReceivedMultiplier);
			if (stats.HealthRestoreFromMaxHealth > 0)
				fSegmentMitigation += StatConversion.ApplyMultiplier((stats.HealthRestoreFromMaxHealth * stats.Health), stats.HealingReceivedMultiplier);
			fTotalMitigation += fSegmentMitigation;

			calcs.Mitigation = fTotalMitigation;
			calcs.MitigationWeight = TDK.opts.MitigationWeight;
			#endregion

			#region Key Data Validation
			if (float.IsNaN(calcs.Threat) ||
				float.IsNaN(calcs.Survival) ||
				float.IsNaN(calcs.Mitigation) ||
				float.IsNaN(calcs.BurstTime) ||
				float.IsNaN(calcs.ReactionTime) ||
				float.IsNaN(calcs.OverallPoints) )
			{
#if DEBUG
				throw new Exception("One of the Subpoints are Invalid.");
#endif
			}
			#endregion

			#region Display only work
//            if (needsDisplayCalculations)
//            {
			calcs.cType = TDK.opts.cType;
			if (TDK.opts.cType == CalculationType.Burst)
			{
				_subPointNameColors = _subPointNameColors_Burst;
			}
			else
			{
				_subPointNameColors = _subPointNameColors_SMT;
			}

				calcs.BasicStats = sPaperDoll;
				// The full character data.
				calcs.TargetLevel = iTargetLevel;

				calcs.Miss = stats.Miss * 100f;
				calcs.Dodge = stats.Dodge * 100f;
				calcs.Parry = stats.Parry * 100f;
				calcs.Crit = fChanceToGetCrit * 100f;

				calcs.DefenseRating = stats.DefenseRating;
				calcs.Defense = stats.Defense;
				calcs.Resilience = stats.Resilience;

				calcs.TargetDodge = chanceTargetDodge;
				calcs.TargetMiss = chanceTargetMiss;
				calcs.TargetParry = chanceTargetParry;
				calcs.Expertise = stats.Expertise;
//                calcs.BasicStats.ArmorPenetration = StatConversion.GetArmorPenetrationFromRating(sPaperDoll.ArmorPenetrationRating) * 100f;

				calcs.ArmorDamageReduction = ArmorDamageReduction;
//            }
			#endregion

			return calcs;
		}
		/// <summary>
		/// GetCharacterStats is the 2nd-most calculation intensive method in a model. Here the model will
		/// combine all of the information about the character, including race, gear, enchants, buffs,
		/// calculationoptions, etc., to form a single combined Stats object. Three of the methods below
		/// can be called from this method to help total up stats: GetItemStats(character, additionalItem),
		/// GetEnchantsStats(character), and GetBuffsStats(character.ActiveBuffs).
		/// </summary>
		/// <param name="character">The character whose stats should be totaled.</param>
		/// <param name="addition
		/// alItem">An additional item to treat the character as wearing.
		/// This is used for gems, which don't have a slot on the character to fit in, so are just
		/// added onto the character, in order to get gem calculations.</param>
		/// <returns>A Stats object containing the final totaled values of all character stats.</returns>
		public override Stats GetCharacterStats(Character character, Item additionalItem) {
			Stats statsTotal = new Stats();

			// Validate that character.CalculationOptions != NULL
			if (null == character.CalculationOptions) {
				// Possibly put some error text here.
				return statsTotal;
			}
			CalculationOptionsTankDK calcOpts = character.CalculationOptions as CalculationOptionsTankDK;

			// Filter out the duplicate Runes:
			if (character.MainHandEnchant == Enchant.FindEnchant(3368, ItemSlot.OneHand, character)
				&& character.MainHandEnchant == character.OffHandEnchant)
			{
				// Remove one of the enchants.
				character.OffHandEnchant = null;
			}

			// Start populating data w/ Basic racial & class baseline.
			statsTotal = BaseStats.GetBaseStats(character);
			statsTotal.BaseAgility = BaseStats.GetBaseStats(character).Agility;

			if (statsTotal.Defense < 400f)
				// Adding in the base 400 Defense skill all tanks are expected to have.  
				// There are too many places where this just kinda stuck in.  It should be attached to the toon.
				statsTotal.Defense = 400f;
			AccumulateItemStats(statsTotal, character, additionalItem);
			AccumulateBuffsStats(statsTotal, character.ActiveBuffs); // includes set bonuses.
			// Except the 4 piece T9 - improves CD of VB, UA, and BS by 10 sec.  That has to get handled elsewhere.
			if (character.ActiveBuffsContains("Thassarian's Plate 4 Piece Bonus") ||
				character.ActiveBuffsContains("Koltira's Plate 4 Piece Bonus"))
			{
				// Set the character as having the T9_4pc bonus
				m_bT9_4PC = true;
			}
			
			// Stack only the info we care about.
			AccumulateTalents(statsTotal, character);
			statsTotal = GetRelevantStats(statsTotal);
			AccumulateFrostPresence(statsTotal);

			/* At this point, we're combined all the data from gear and talents and all that happy jazz.
			 * However, we haven't applied any special effects nor have we applied any multipliers.
			 * Also many special effects are now getting dependant upon combat info (rotations).
			 */ 

			return (statsTotal);
		}

		public Stats GetBuffsStats(Character character, CalculationOptionsTankDK calcOpts) {
			List<Buff> removedBuffs = new List<Buff>();
			List<Buff> addedBuffs = new List<Buff>();

			Stats statsBuffs = GetBuffsStats(character.ActiveBuffs);

			foreach (Buff b in removedBuffs) {
				character.ActiveBuffsAdd(b);
			}
			foreach (Buff b in addedBuffs) {
				character.ActiveBuffs.Remove(b);
			}

			return statsBuffs;
		}

		/// <summary>
		/// Process the Stat modifier values 
		/// </summary>
		/// <param name="statsTotal">[in/out] Stats object for the total character stats.</param>
		/// <param name="iBladedArmor">[in] character.talent.BladedArmor</param>
		private void ProcessStatModifiers( Stats statsTotal, int iBladedArmor )
		{
			statsTotal.Strength = StatConversion.ApplyMultiplier(statsTotal.Strength, statsTotal.BonusStrengthMultiplier);
			statsTotal.Agility = StatConversion.ApplyMultiplier(statsTotal.Agility, statsTotal.BonusAgilityMultiplier);
			// The stamina value is floor in game for the calculation
			statsTotal.Stamina = StatConversion.ApplyMultiplier(statsTotal.Stamina, statsTotal.BonusStaminaMultiplier);
			statsTotal.Stamina = (float)Math.Floor(statsTotal.Stamina);
			statsTotal.Armor = StatConversion.ApplyMultiplier(statsTotal.Armor, statsTotal.BaseArmorMultiplier);
			statsTotal.AttackPower = StatConversion.ApplyMultiplier(statsTotal.AttackPower, statsTotal.BonusAttackPowerMultiplier);
			statsTotal.BonusArmor = StatConversion.ApplyMultiplier(statsTotal.BonusArmor, statsTotal.BonusArmorMultiplier);

			float AgiArmor = StatConversion.GetArmorFromAgility(statsTotal.Agility); // Don't multiply the armor from agility.
			statsTotal.Armor += statsTotal.BonusArmor + AgiArmor;
			statsTotal.Health += StatConversion.GetHealthFromStamina(statsTotal.Stamina);

			StatConversion.ApplyMultiplier(statsTotal.Health, statsTotal.BonusHealthMultiplier);

			// Talent: BladedArmor //////////////////////////////////////////////////////////////
			if (iBladedArmor > 0)
			{
				statsTotal.AttackPower += (statsTotal.Armor / 180f) * (float)iBladedArmor;
			}
			// AP, crit, etc.  already being factored in w/ multiplier.
			statsTotal.AttackPower += StatConversion.ApplyMultiplier((statsTotal.Strength * 2), statsTotal.BonusAttackPowerMultiplier);

			// Parry from str. is only available to DKs.
			statsTotal.ParryRating += statsTotal.Strength * 0.25f;
		}

		/// <summary>
		/// Process All the ratings score to their base values.
		/// </summary>
		/// <param name="s"></param>
		private void ProcessRatings(Stats statsTotal)
		{
			statsTotal.PhysicalCrit = StatConversion.ApplyMultiplier(statsTotal.PhysicalCrit
										+ StatConversion.GetCritFromAgility(statsTotal.Agility, CharacterClass.DeathKnight)
										+ StatConversion.GetCritFromRating(statsTotal.CritRating), statsTotal.BonusCritMultiplier);
			statsTotal.SpellCrit = StatConversion.ApplyMultiplier(statsTotal.SpellCrit + statsTotal.SpellCritOnTarget
										+ StatConversion.GetCritFromRating(statsTotal.CritRating), statsTotal.BonusSpellCritMultiplier);

			statsTotal.PhysicalHit += StatConversion.GetHitFromRating(statsTotal.HitRating, CharacterClass.DeathKnight);
			statsTotal.SpellHit += StatConversion.GetSpellHitFromRating(statsTotal.HitRating);

			// Expertise Rating -> Expertise:
			statsTotal.Expertise += StatConversion.GetExpertiseFromRating(statsTotal.ExpertiseRating);

			statsTotal.ArmorPenetration += StatConversion.GetArmorPenetrationFromRating(statsTotal.ArmorPenetrationRating);

		}

		private void ProcessAvoidance(Stats statsTotal, int iTargetLevel)
		{
			// Get all the character avoidance numbers including deminishing returns.
			// Iterate through each hit type. and use fAvoidance array w/ the hitresult enum.
			float[] fAvoidance = new float[HitResultCount];
			Character c = new Character();
			c.Class = CharacterClass.DeathKnight;
			for (uint i = 0; i < HitResultCount; i++)
			{
				// GetDRAvoidanceChance returns a dec. percentage.
				// Since CurrentAvoidance is a percent, need to multiply by 100.
				fAvoidance[i] = (StatConversion.GetDRAvoidanceChance(c, statsTotal, (HitResult)i, iTargetLevel));
			}

			// So let's populate the miss, dodge and parry values for the UI display as well as pulling them out of the avoidance number.
			statsTotal.Miss = Math.Min((StatConversion.CAP_MISSED[(int)CharacterClass.DeathKnight]/100), fAvoidance[(int)HitResult.Miss]);
			statsTotal.Dodge = Math.Min((StatConversion.CAP_DODGE[(int)CharacterClass.DeathKnight]/100), fAvoidance[(int)HitResult.Dodge]);
			statsTotal.Parry = Math.Min((StatConversion.CAP_PARRY[(int)CharacterClass.DeathKnight] / 100), fAvoidance[(int)HitResult.Parry]);
		}

		/// <summary>
		/// Pass in the total stats object we're working with and add specific stat modifiers.
		/// </summary>
		/// <param name="s"></param>
		private void AccumulateFrostPresence(Stats s)
		{
			s.BaseArmorMultiplier = AddStatMultiplierStat(s.BaseArmorMultiplier, .6f); // Bonus armor for Frost Presence down from 80% to 60% as of 3.1.3
			s.BonusStaminaMultiplier = AddStatMultiplierStat(s.BonusStaminaMultiplier, .08f); // Bonus 8% Stamina
			s.DamageTakenMultiplier = AddStatMultiplierStat(s.DamageTakenMultiplier, -.08f);// Bonus of 8% damage reduced for frost presence. up from 5% for 3.2.2
//            s.ThreatIncreaseMultiplier += .45f; // Pulling this out since the threat bonus is normalized at 2.0735 as per multiple 
			// Tankspot and EJ conversations.
		}

		/// <summary>Build the talent special effects.</summary>
		private void AccumulateTalents(Stats FullCharacterStats, Character character)
		{
			Stats newStats = new Stats();
			float fDamageDone = 0f;

			#region Blood Talents
			// Butchery
			// 1RPp5 per Point
			// TODO: Implement Runic Regen info.
			if (character.DeathKnightTalents.Butchery > 0)
			{
				FullCharacterStats.RPp5 += 1 * character.DeathKnightTalents.Butchery;
			}

			// Subversion
			// Increase crit 3% per point of BS, HS, Oblit
			// 3.2.2: also SS
			if (character.DeathKnightTalents.Subversion > 0)
			{
				// implmented in CombatTable.cs
			}

			// Blade Barrier
			// Reduce damage by 1% per point for 10 sec.
			if (character.DeathKnightTalents.BladeBarrier > 0)
			{
				// If you don't have your Blood Runes on CD, you're doing it wrong. 
				FullCharacterStats.DamageTakenMultiplier -= (.01f * character.DeathKnightTalents.BladeBarrier);
			}

			// Bladed Armor
			// 1 AP per point per 180 Armor
			// Implmented after Frost Presence above.

			// Scent of Blood
			// 15% after Dodge, Parry or damage received causing 1 melee hit per point to generate 5 runic power.
			// TODO: setup RP gains.

			// 2H weapon spec.
			// 2% per point increased damage
			// Implmented in weapon section above.

			// Rune Tap
			// Convert 1 BR to 10% health.
			/*
			if (character.DeathKnightTalents.RuneTap > 0)
			{
				newStats = new Stats();
				float fCD = 60f;
				newStats.Healed = (GetCurrentHealth(FullCharacterStats) * .1f);
				// Improved Rune Tap.
				// increases the health provided by RT by 33% per point. and lowers the CD by 10 sec per point
				fCD -= (10f * character.DeathKnightTalents.ImprovedRuneTap);
				newStats.Healed += (newStats.Healed * (character.DeathKnightTalents.ImprovedRuneTap / 3f));
				FullCharacterStats.AddSpecialEffect(new SpecialEffect(Trigger.Use, newStats, 0, fCD));
			}
			*/
			// Dark Conviction 
			// Increase Crit w/ weapons, spells, and abilities by 1% per point.
			if (character.DeathKnightTalents.DarkConviction > 0)
			{
				FullCharacterStats.PhysicalCrit += (0.01f * character.DeathKnightTalents.DarkConviction);
				FullCharacterStats.SpellCrit += (0.01f * character.DeathKnightTalents.DarkConviction);
			}

			// Death Rune Mastery
			// Create death runes out of Frost & Unholy for each oblit/DS.
			// Implemented Death Runes in new CombatTable/Ability/Rotation

			// Spell Deflection
			// Parry chance of taking 15% less damage per point from direct damage spell
			// Implmented after Parry calc above.

			// Vendetta
			// Heals you for up to 2% per point on killing blow
			// Not important for tanking

			// Bloody Strikes
			// increases damage of BS and HS by 15% per point
			// increases damage of BB by 10% per point
			// Implemented in Combattable.cs

			// Veteran of the 3rd War
			// Patch 3.2 from 2% to 1% per point.
			// increases Str and Stam by 1% per point
			// increases expertise by 2 per point.
			if (character.DeathKnightTalents.VeteranOfTheThirdWar > 0)
			{
				FullCharacterStats.BonusStrengthMultiplier += (.02f * character.DeathKnightTalents.VeteranOfTheThirdWar);
				FullCharacterStats.BonusStaminaMultiplier += (.01f * character.DeathKnightTalents.VeteranOfTheThirdWar);
				FullCharacterStats.Expertise += (2f * character.DeathKnightTalents.VeteranOfTheThirdWar);
			}

			// Mark of blood
			// Cast on the enemy
			// buff that lasts 20 secs or 20 hits
			// heals the target for 4% of max health for each damage dealing hit from that enemy to the target of that enemy.
			// Implemented in Mitigation section above.

			// Bloody Vengence
			// 1% per point bonus to physical damage for 30 secs after a crit w/ up to 3 stacks.
			if (character.DeathKnightTalents.BloodyVengeance > 0)
			{
                newStats = new Stats() { BonusPhysicalDamageMultiplier = .01f * character.DeathKnightTalents.BloodyVengeance };
                FullCharacterStats.AddSpecialEffect(_SE_BloodyVengeance1[character.DeathKnightTalents.BloodyVengeance]);
				FullCharacterStats.AddSpecialEffect(_SE_BloodyVengeance2[character.DeathKnightTalents.BloodyVengeance]);
			}

			// Abominations Might
			// increase AP by 5%/10% of raid.
			// 1% per point increase to str.
			if (character.DeathKnightTalents.AbominationsMight > 0)
			{
				// This happens no matter what:
				FullCharacterStats.BonusStrengthMultiplier += (0.01f * character.DeathKnightTalents.AbominationsMight);
				// This happens only if there isn't Trueshot Aura available:
                if (!(character.ActiveBuffsContains("Trueshot Aura") || character.ActiveBuffsContains("Unleashed Rage") || character.ActiveBuffsContains("Abomination's Might")))
                {
                    FullCharacterStats.BonusAttackPowerMultiplier += (.05f * character.DeathKnightTalents.AbominationsMight);
                }
			}

			// Bloodworms
			// 3% chance per point per hit to cause the target to spawn 2-4 blood worms
			// Healing you 150% of the damage they do for 20 sec.
			if (character.DeathKnightTalents.Bloodworms > 0)
			{
				// TODO: figure out how much damage the worms do.
				fDamageDone = 100f;
				float fBWAttackSpeed = 2f;
				float fBWDuration = 20f;
                float key = (fDamageDone * fBWDuration / fBWAttackSpeed);
                // note, while this only creates one Dictionary entry and may seem like a waste
                // I left it open like this so that your above TODO for figuring out how much damage the worms do will make this part dynamic
                if (!_SE_Bloodworms.ContainsKey(key)) {
                    _SE_Bloodworms.Add(key, new SpecialEffect[] {
                        null,
                        new SpecialEffect(Trigger.PhysicalHit, new Stats() { Healed = ((fDamageDone * fBWDuration / fBWAttackSpeed) * 1.5f) }, fBWDuration, 0, .03f * 1),
                        new SpecialEffect(Trigger.PhysicalHit, new Stats() { Healed = ((fDamageDone * fBWDuration / fBWAttackSpeed) * 1.5f) }, fBWDuration, 0, .03f * 2),
                        new SpecialEffect(Trigger.PhysicalHit, new Stats() { Healed = ((fDamageDone * fBWDuration / fBWAttackSpeed) * 1.5f) }, fBWDuration, 0, .03f * 3),
                    });
                }
                FullCharacterStats.AddSpecialEffect(_SE_Bloodworms[key][character.DeathKnightTalents.Bloodworms]);
			}

			// Hysteria
			// Killy frenzy for 30 sec.
			// Increase physical damage by 20%
			// take damage 1% of max every sec.
			if (character.DeathKnightTalents.Hysteria > 0)
			{
				/*
				 * Pulling out the value of Hysteria since the target is rarely going to be the tank.
				float fDur = 30f;
				newStats = new Stats();
				newStats.BonusPhysicalDamageMultiplier += 0.2f;
				newStats.Healed -= (fHealth * 0.01f * fDur);
				FullCharacterStats.AddSpecialEffect(new SpecialEffect(Trigger.Use, newStats, fDur, 3f * 60f));
				 */
			}

			// Improved Blood Presence
			// while in frost or unholy, you retain the 2% per point healing from blood presence
			// Healing done to you is increased by 5% per point
			// Implemented above.

			// Improved Death Strike
			// increase damage of DS by 15% per point 
			// increase crit chance of DS by 3% per point
			// Implemented in CombatTable.cs

			// Sudden Doom
			// BS & HS have a 5% per point chance to launch a DC at target
			if (character.DeathKnightTalents.SuddenDoom > 0)
			{
				// Implmented in CombatTable.cs
			}

			// Vampiric Blood
			// temp 15% of max health and
			// increases health generated by 35% for 10 sec.
			// 1 min CD. as of 3.2.2
			/*
			if (character.DeathKnightTalents.VampiricBlood > 0)
			{
				// Also copy above, but it's commented out.
				newStats = new Stats();
				newStats.Health = (GetCurrentHealth(FullCharacterStats) * 0.15f);
				newStats.HealingReceivedMultiplier += 0.35f;

				float fVBCD = 60f;
				if (m_bT9_4PC) fVBCD -= 10f;
				float fVBDur = 10f;
				if (character.DeathKnightTalents.GlyphofVampiricBlood == true)
				{
					fVBDur += 5f;
				}
				FullCharacterStats.AddSpecialEffect(new SpecialEffect(Trigger.Use, newStats, fVBDur, fVBCD));
			}
			*/
			// Will of the Necropolis
			// Damage that takes you below 35% health or while at less than 35% is reduced by 5% per point.  
			if (character.DeathKnightTalents.WillOfTheNecropolis > 0)
			{
				// Need to factor in the damage taken aspect of the trigger.
				// Using the assumption that the tank will be at < 35% health about that % of the time.
				FullCharacterStats.AddSpecialEffect(_SE_WillOfTheNecropolis[character.DeathKnightTalents.WillOfTheNecropolis]);
			}

			// Heart Strike
			// 3.2.2: Secondary targets of HS take 1/2 as much damage
			// Implemented in CombatTable.cs

			// Might of Mograine
			// increase crit damage of BB, BS, DS, and HS by 15% per point
			// Implemented in CombatTable.cs

			// Blood Gorged
			// when above 75% health, you deal 2% more damage per point
			// when above 75% health, you receive 2% Armor Pen
			if (character.DeathKnightTalents.BloodGorged > 0)
			{
				// Damage done increase has to be in shot rotation.
				// Assuming a 50% up time 
				FullCharacterStats.ArmorPenetration += (0.02f * character.DeathKnightTalents.BloodGorged * 0.5f);
				FullCharacterStats.BonusDamageMultiplier += (0.02f * character.DeathKnightTalents.BloodGorged * 0.5f);
			}

			// Dancing Rune Weapon
			// not impl
			#endregion

			#region Frost Talents
			// Improved Icy Touch
			// 5% per point additional IT damage
			// 2% per point target haste reduction 
			if (character.DeathKnightTalents.ImprovedIcyTouch > 0)
			{
				FullCharacterStats.BonusIcyTouchDamage += (0.05f * character.DeathKnightTalents.ImprovedIcyTouch);
				// Haste Damage reduction added into Boss Attack speed.
				//                sReturn.DamageTakenMultiplier -= 0.02f * character.DeathKnightTalents.ImprovedIcyTouch;
			}

			// Runic Power Mastery
			// Increases Max RP by 15 per point
			if (character.DeathKnightTalents.RunicPowerMastery > 0)
			{
				BonusMaxRunicPower += 5 * character.DeathKnightTalents.RunicPowerMastery;
			}

			// Toughness
			// Patch 3.2: Increases Armor Value from items by 2% per point.
			// Reducing duration of all slowing effects by 6% per point.  
			if (character.DeathKnightTalents.Toughness > 0)
			{
				FullCharacterStats.BaseArmorMultiplier = AddStatMultiplierStat(FullCharacterStats.BaseArmorMultiplier, (.02f * character.DeathKnightTalents.Toughness)); // Patch 3.2
			}

			// Icy Reach
			// Increases range of IT & CoI and HB by 5 yards per point.

			// Black Ice
			// Increase Frost & shadow damage by 2% per point
			if (character.DeathKnightTalents.BlackIce > 0)
			{
				FullCharacterStats.BonusFrostDamageMultiplier += 0.02f * character.DeathKnightTalents.BlackIce;
				FullCharacterStats.BonusShadowDamageMultiplier += 0.02f * character.DeathKnightTalents.BlackIce;
			}

			// Nerves of Cold Steel
			// Increase hit w/ 1H weapons by 1% per point
			// Increase damage done by off hand weapons by 8/16/25% per point
			// Implement in combat shot roation

			// Icy Talons
			// Increase melee attack speed by 4% per point for the next 20 sec.
			if (character.DeathKnightTalents.IcyTalons > 0)
			{
				FullCharacterStats.AddSpecialEffect(_SE_IcyTalons[character.DeathKnightTalents.IcyTalons]);
			}

			// Lichborne
			// for 10 sec, immune to charm, fear, sleep
			// CD 2 Mins

			// Threat of Thassarian: 
			// New 3-point talent. When dual-wielding, your Death Strikes, Obliterates, Plague Strikes, 
			// Blood Strikes and Frost Strikes and Rune Strike (as of 3.2.2) have a 30/60/100% chance 
			// to also deal damage with your  off-hand weapon. Off-hand strikes are roughly one half the effect of the original strike. 

			// Annihilation
			// +1 % per point melee Crit chance 
			// 33% per point that oblit will not consume diseases
			if (character.DeathKnightTalents.Annihilation > 0)
			{
				FullCharacterStats.PhysicalCrit += (0.01f * character.DeathKnightTalents.Annihilation);
			}

			// Killing Machine
			// Melee attacks have a chance to make IT, HB, or FS a crit.
			// increased proc per point.

			// Chill of the Grave
			// CoI, HB, IT and Oblit generate 2.5 RP per point.

			// Endless Winter
			// removes FF from COI
			// Mind Freeze RP cost is reduced by 50% per point.
			if (character.DeathKnightTalents.EndlessWinter > 0)
			{
				FullCharacterStats.BonusStrengthMultiplier += (.02f * character.DeathKnightTalents.EndlessWinter);
			}

			// Frigid Dreadplate
			// Melee attacks against you will miss by +1% per point
			if (character.DeathKnightTalents.FrigidDreadplate > 0)
			{
				FullCharacterStats.Miss += 0.01f * character.DeathKnightTalents.FrigidDreadplate;
			}

			// Glacier Rot
			// Diseased enemies take 7%, 13% , 20% more damage from IT, HB, FS.
			if (character.DeathKnightTalents.GlacierRot > 0)
			{
				float fBonus = 0f;
				switch (character.DeathKnightTalents.GlacierRot)
				{
					case 1: fBonus = 0.07f; break;
					case 2: fBonus = 0.13f; break;
					case 3: fBonus = 0.20f; break;
				}
				FullCharacterStats.BonusIcyTouchDamage += fBonus;
				FullCharacterStats.BonusFrostStrikeDamage += fBonus;
				FullCharacterStats.BonusHowlingBlastDamage += fBonus;
			}

			// Deathchill
			// when active IT, HB, FS, Oblit will crit.

			// Improved Icy Talons
			// increases the melee haste of the group/raid by 20%
			// increases your haste by 5% all the time.
			if (character.DeathKnightTalents.ImprovedIcyTalons > 0)
			{
				FullCharacterStats.PhysicalHaste += 0.05f;
				// TODO: Factor in raid utility by improving raid haste by 20%
				// As per Blue Post Effect *does* stack w/ existing IcyTalons.
				// However, it will not stack if already included on Buffs tab.
				// Now passive - no longer procs.
				if (character.ActiveBuffsContains("Improved Icy Talons") != true 
					&& !character.ActiveBuffsContains("Windfury Totem"))
				{
					FullCharacterStats.PhysicalHaste += .2f;
				}
			}

			// Merciless Combat
			// addtional 6% per point damage for IT, HB, Oblit, and FS
			// on targets of less than 35% health.

			// Rime
			// increases crit chance of IT and Oblit by 5% per point
			// Oblit has a 5% per point to reset CD of HB and HB consumes no runes

			// Chilblains
			// FF victimes are movement reduced 15, 30, 50%

			// Hungering Cold
			// Spell that freezes all enemies w/ 10 yards.

			// Improved Frost Presence
			// retain the health bonus 5% per point when in non-Frost presence
			// Decrease damage done to you by 1% per point.
			if (character.DeathKnightTalents.ImprovedFrostPresence > 0)
			{
				FullCharacterStats.DamageTakenMultiplier -= (0.01f * character.DeathKnightTalents.ImprovedFrostPresence);
			}

			// Blood of the North
			// Patch 3.2: BS & FS damage +3% per point
			// Patch 3.2: BS & Pest create DeathRune from Blood 33% per point.
			if (character.DeathKnightTalents.BloodOfTheNorth > 0)
			{
				float fBonus = 0f;
				switch (character.DeathKnightTalents.BloodOfTheNorth)
				{
					case 1:
						fBonus = 0.03f;
						break;
					case 2:
						fBonus = 0.06f;
						break;
					case 3:
						fBonus = 0.1f;
						break;
				}
				FullCharacterStats.BonusFrostStrikeDamage += fBonus;
				FullCharacterStats.BonusBloodStrikeDamage += fBonus;
			}

			// Unbreakable Armor
			// Reinforces your armor with a thick coat of ice, Increasing Armor by 25% and increasing your Strength by 20% for 20 sec.
			if (character.DeathKnightTalents.UnbreakableArmor > 0)
			{
                // As per wowhead: GlyphofUnbreakableArmor
                // Effect: Apply Aura: Add % Modifier (3) Value: 20
                FullCharacterStats.AddSpecialEffect(_SE_UnbreakableArmor[character.DeathKnightTalents.GlyphofUnbreakableArmor?1:0][m_bT9_4PC ? 1 : 0]);
			}

			// Acclimation
			// When hit by a spell, 10% chance per point to boost resistance to that type of magic for 18 sec.  
			// up to 3 stacks.
			if (character.DeathKnightTalents.Acclimation > 0)
			{
                // TODO: SpellHit is not sufficient.  Need to have this be DamageTakenSpell (vs. DamageTakenPhysical)
				FullCharacterStats.AddSpecialEffect(_SE_Acclimation[character.DeathKnightTalents.Acclimation]);
			}

			// Frost Strike

			// Guile of Gorefiend
			// Increases CritStrike Damage of BS, FS, HB, Oblit by 15% per point.
			// Increases Duration of IBF by 2 sec per point.
			// HACK: Implenting IceBound Fortitude. ////////////////////////////////////////////////////////
			// Implmenting IBF here because it's affected by GoGF
			// Four T7 increases IBF by 3 sec.
			// Patch 3.2: IBF has a 120 sec CD. 
			float fIBFDur = (12.0f + character.DeathKnightTalents.GuileOfGorefiend * 2.0f + FullCharacterStats.BonusIceboundFortitudeDuration);
			// IBF reduces damage taken by 20% + 3% for each 28 defense over 400.
			float ibfDefense = StatConversion.GetDefenseFromRating(FullCharacterStats.DefenseRating, character.Class);
			float ibfReduction = 0.3f + (ibfDefense * 0.0015f);
			if (character.DeathKnightTalents.GlyphofIceboundFortitude)
			{
				// The glyph provides a MIN of 30% damage reduction, but doesn't help if your def takes you over that.
				ibfReduction = Math.Max(0.4f, ibfReduction);
			}
            FullCharacterStats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { DamageTakenMultiplier = -1f * ibfReduction }, fIBFDur, 120)); // Patch 3.2

			// Tundra Stalker
			// Your spells & abilities deal 3% per point more damage to targets w/ FF
			// Increases Expertise by 1 per point
			if (character.DeathKnightTalents.TundraStalker > 0)
			{
				// Assuming FF is always up.
				FullCharacterStats.BonusDamageMultiplier += .03f * character.DeathKnightTalents.TundraStalker;
				FullCharacterStats.Expertise += 1f * character.DeathKnightTalents.TundraStalker;
			}

			// Howling Blast.

			#endregion

			#region UnHoly Talents
			// Vicious Strikes
			// Increases Crit chance by 3% per point of PS and SS
			// Increases Crit Strike Damage by 15% per point of PS and SS

			// Virulence
			// Increases Spell hit +1% per point
			if (character.DeathKnightTalents.Virulence > 0)
			{
				FullCharacterStats.SpellHit += 0.01f * character.DeathKnightTalents.Virulence;
			}

			// Anticipation
			// Increases dodge by 1% per point
			if (character.DeathKnightTalents.Anticipation > 0)
			{
				FullCharacterStats.Dodge += 0.01f * character.DeathKnightTalents.Anticipation;
			}

			// Epidemic
			// Increases Duration of BP and FF by 3 sec per point

			// Morbidity
			// increases dam & healing of DC by 5% per point
			// Decreases CD of DnD by 5 sec per point

			// Unholy Command
			// reduces CD of DG by 5 sec per point

			// Ravenous Dead
			// Increases Str +1% per point.
			// Increases contribution of your str & stam to ghoul by 20% per point
			if (character.DeathKnightTalents.RavenousDead > 0)
			{
				FullCharacterStats.BonusStrengthMultiplier += (0.01f * character.DeathKnightTalents.RavenousDead);
				// Ghouls don't help tank here.
			}

			// Outbreak
			// increases dam of PS by 10% per point
			// increases dam of SS by 7% per point

			// Necrosis
			// Autoattacks deal additional 4% shadow

			// Corpse Explosion
			// Does damage by blowing up a corpse to all targets in 10 yards

			// On a Pale Horse
			// Reduce dur of stun and fear by 10% per point
			// increase mount speed byh 10% per point

			// Blood-Caked Blade
			// 10% chance per point to cause Blood-Caked strike

			// Night of the Dead
			// Reduces CD of Raise Dead by 45 sec per point
			// Reduces CD of Army of the dead by 5 min per point

			// Unholy Blight
			// Shadow Damage done to all targets in 10 yards for 20 sec.

			// Impurity
			// Attack Power bonus to spells increased by 4% per point.

			// Dirge
			// DS, PS and SS generate 2.5 more runic power per point.

			// Magic Suppression
			// 2% per point less damage from all magic.
			// AMS absorbs additional 8, 16, 25% of spell damage.
			if (character.DeathKnightTalents.MagicSuppression > 0)
			{
				FullCharacterStats.SpellDamageTakenMultiplier -= 0.02f * character.DeathKnightTalents.MagicSuppression;
				// AMS modification factored in above.
			}

			// Reaping 
			// BS or Pest convert to DR.

			// Master of Ghouls
			// Reduces CD on Raise Dead by 60 sec.
			// Ghoul summoned is perm (pet). 

			// Desecration
			// PS and SS cause Desecrated Ground effect.
			// Targets are slowed by 10% per point
			// Patch 3.2: You don't cause 1% more damage 
			// Lasts 12 sec.
			/* Patch 3.2
			if (character.DeathKnightTalents.Desecration > 0)
			{
				newStats = new Stats();
				newStats.BonusDamageMultiplier += 0.01f * character.DeathKnightTalents.Desecration;
				// Gonna use an average CD of a rune at 10sec per rune divided by 2 runes == 5 sec.
				sReturn.AddSpecialEffect(new SpecialEffect(Trigger.SpellHit, newStats, 12f, 5f));
			}
			*/

			// AntiMagic Zone
			// Creates a zone where party/raid members take 75% less spell damage
			// Lasts 10 secs or X damage.  
			if (character.DeathKnightTalents.AntiMagicZone > 0)
			{
				FullCharacterStats.AddSpecialEffect(_SE_AntiMagicZone);
			}

			// Improved Unholy Presence
			// in Blood or Frost, retain movement speed (8%, 15%).
			// Runes finish CD 5% per point faster.

			// Ghoul Frenzy
			// Grants pet 25% haste for 30 sec and heals it for 60% health.

			// Crypt Fever
			// CF increases disease damage taken by target by 10% per point

			// Bone Shield
			// 3 Bones as of 3.2.2
			// Takes 20% less dmage from all sources
			// Does 2% more damage to target
			// Each damaging attack consumes a bone.
			// Lasts 5 mins

			// Ebon Plaguebringer
			// CF becomes EP - increases magic damage taken by targets 4, 9, 13% in addition to disease damage
			// Increases crit strike chance by 1% per point
			if (character.DeathKnightTalents.EbonPlaguebringer > 0)
			{
				FullCharacterStats.PhysicalCrit += 0.01f * character.DeathKnightTalents.EbonPlaguebringer;
				FullCharacterStats.SpellCrit += 0.01f * character.DeathKnightTalents.EbonPlaguebringer;
				if (!character.ActiveBuffsContains("Earth and Moon")
					&& !character.ActiveBuffsContains("Curse of the Elements")
					&& !character.ActiveBuffsContains("Ebon Plaguebringer"))
				{
					float fBonus = 0f;
					switch (character.DeathKnightTalents.EbonPlaguebringer)
					{
						case 1:
							fBonus = .04f;
							break;
						case 2:
							fBonus = .09f;
							break;
						case 3:
							fBonus = .13f;
							break;
						default:
							break;
					}
					FullCharacterStats.BonusArcaneDamageMultiplier += fBonus;
					FullCharacterStats.BonusFireDamageMultiplier += fBonus;
					FullCharacterStats.BonusFrostDamageMultiplier += fBonus;
					FullCharacterStats.BonusHolyDamageMultiplier += fBonus;
					FullCharacterStats.BonusNatureDamageMultiplier += fBonus;
					FullCharacterStats.BonusShadowDamageMultiplier += fBonus;
				}
			}

			// Sourge Strike

			// Rage of Rivendare
			// 2% per point more damage to targets w/ BP
			// Expertise +1 per point
			if (character.DeathKnightTalents.RageOfRivendare > 0)
			{
				FullCharacterStats.Expertise += character.DeathKnightTalents.RageOfRivendare;
				// Assuming BP is always on.
				FullCharacterStats.BonusDamageMultiplier += 0.02f * character.DeathKnightTalents.RageOfRivendare;
			}

			// Summon Gargoyle

			#endregion

			//            return sReturn;
		}

		private float GetCurrentHealth(Stats FullCharacterStats)
		{
			return StatConversion.ApplyMultiplier((FullCharacterStats.Health + StatConversion.GetHealthFromStamina(FullCharacterStats.Stamina)), FullCharacterStats.BonusHealthMultiplier);
		}

		/// <summary>
		/// Gets data to fill a custom chart, based on the chart name, as defined in CustomChartNames.
		/// </summary>
		/// <param name="character">The character to build the chart for.</param>
		/// <param name="chartName">The name of the custom chart to get data for.</param>
		/// <returns>The data for the custom chart.</returns>
		public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName) 
		{
			return new ComparisonCalculationBase[0];
		}

		#region Relevant Stats
		/// <summary>
		/// Filters a Stats object to just the stats relevant to the model.
		/// </summary>
		/// <param name="stats">A complete Stats object containing all stats.</param>
		/// <returns>A filtered Stats object containing only the stats relevant to the model.</returns>
		public override Stats GetRelevantStats(Stats stats) {
			Stats s = new Stats() {
				Strength = stats.Strength,
				Agility = stats.Agility,
				BaseAgility = stats.BaseAgility,
				Stamina = stats.Stamina,
				Armor = stats.Armor,
				BonusArmor = stats.BonusArmor,
				Health = stats.Health,
				BattlemasterHealth = stats.BattlemasterHealth,

				HighestStat = stats.HighestStat,
				Paragon = stats.Paragon,
				DeathbringerProc = stats.DeathbringerProc,

				DefenseRating = stats.DefenseRating,
				ParryRating = stats.ParryRating,
				DodgeRating = stats.DodgeRating,

				Defense = stats.Defense,
				Dodge = stats.Dodge,
				Parry = stats.Parry,
				Miss = stats.Miss,

				Resilience = stats.Resilience,
                SpellPenetration = stats.SpellPenetration,

				DamageAbsorbed = stats.DamageAbsorbed,
				AttackPower = stats.AttackPower,
				HitRating = stats.HitRating,
				CritRating = stats.CritRating,
				ArmorPenetration = stats.ArmorPenetration,
				ArmorPenetrationRating = stats.ArmorPenetrationRating,
				ExpertiseRating = stats.ExpertiseRating,
				Expertise = stats.Expertise,
				HasteRating = stats.HasteRating,
				WeaponDamage = stats.WeaponDamage,
				PhysicalCrit = stats.PhysicalCrit,
				PhysicalHaste = stats.PhysicalHaste,
				PhysicalHit = stats.PhysicalHit,
				SpellHit = stats.SpellHit,

				Healed = stats.Healed,
				HealthRestore = stats.HealthRestore,
				HealthRestoreFromMaxHealth = stats.HealthRestoreFromMaxHealth,
				Hp5 = stats.Hp5,

				BonusHealthMultiplier = stats.BonusHealthMultiplier,
				BonusStrengthMultiplier = stats.BonusStrengthMultiplier,
				BonusStaminaMultiplier = stats.BonusStaminaMultiplier,
				BonusAgilityMultiplier = stats.BonusAgilityMultiplier,
				BonusAttackPowerMultiplier = stats.BonusAttackPowerMultiplier,
				BonusCritMultiplier = stats.BonusCritMultiplier,
				BonusSpellCritMultiplier = stats.BonusSpellCritMultiplier,
				BonusDamageMultiplier = stats.BonusDamageMultiplier,
				BonusPhysicalDamageMultiplier = stats.BonusPhysicalDamageMultiplier,
				BaseArmorMultiplier = stats.BaseArmorMultiplier,
				BonusArmorMultiplier = stats.BonusArmorMultiplier,
				DamageTakenMultiplier = stats.DamageTakenMultiplier,
 
				ThreatIncreaseMultiplier = stats.ThreatIncreaseMultiplier,
				ThreatReductionMultiplier = stats.ThreatReductionMultiplier,

				// General Damage Mods.
				BonusShadowDamageMultiplier = stats.BonusShadowDamageMultiplier,
				BonusFrostDamageMultiplier = stats.BonusFrostDamageMultiplier,
				BonusDiseaseDamageMultiplier = stats.BonusDiseaseDamageMultiplier,
				BonusRuneStrikeMultiplier = stats.BonusRuneStrikeMultiplier,
				BonusHeartStrikeDamageMultiplier = stats.BonusHeartStrikeDamageMultiplier,
				BonusBloodStrikeDamageMultiplier = stats.BonusBloodStrikeDamageMultiplier,

				// Ability mods.
				BonusBloodStrikeDamage = stats.BonusBloodStrikeDamage,
				BonusDeathCoilDamage = stats.BonusDeathCoilDamage,
				BonusDeathStrikeDamage = stats.BonusDeathStrikeDamage,
				BonusFrostStrikeDamage = stats.BonusFrostStrikeDamage,
				BonusHeartStrikeDamage = stats.BonusHeartStrikeDamage,
				BonusIcyTouchDamage = stats.BonusIcyTouchDamage,
				BonusObliterateDamage = stats.BonusObliterateDamage,
				BonusScourgeStrikeDamage = stats.BonusScourgeStrikeDamage,
				BonusHowlingBlastDamage = stats.BonusHowlingBlastDamage,
				BonusFrostWeaponDamage = stats.BonusFrostWeaponDamage,

				BonusPerDiseaseBloodStrikeDamage = stats.BonusPerDiseaseBloodStrikeDamage,
				BonusPerDiseaseHeartStrikeDamage = stats.BonusPerDiseaseHeartStrikeDamage,
				BonusPerDiseaseObliterateDamage = stats.BonusPerDiseaseObliterateDamage,
				BonusPerDiseaseScourgeStrikeDamage = stats.BonusPerDiseaseScourgeStrikeDamage,
			
				BonusDeathCoilCrit = stats.BonusDeathCoilCrit,
				BonusDeathStrikeCrit = stats.BonusDeathStrikeCrit,
				BonusFrostStrikeCrit = stats.BonusFrostStrikeCrit,
				BonusObliterateCrit = stats.BonusObliterateCrit,
				BonusPlagueStrikeCrit = stats.BonusPlagueStrikeCrit,
				BonusScourgeStrikeCrit = stats.BonusScourgeStrikeCrit,

				BonusIceboundFortitudeDuration = stats.BonusIceboundFortitudeDuration,
				BonusAntiMagicShellDamageReduction = stats.BonusAntiMagicShellDamageReduction,

				BonusHealingReceived = stats.BonusHealingReceived,
				RPp5 = stats.RPp5,
				TankDK_T10_2pc = stats.TankDK_T10_2pc,
				TankDK_T10_4pc = stats.TankDK_T10_4pc,

				// Resistances
				ArcaneResistance = stats.ArcaneResistance,
				FireResistance = stats.FireResistance,
				FrostResistance = stats.FrostResistance,
				NatureResistance = stats.NatureResistance,
				ShadowResistance = stats.ShadowResistance,
				
				// Damage Procs
				ArcaneDamage = stats.ArcaneDamage,
				FireDamage = stats.FireDamage,
				FrostDamage = stats.FrostDamage,
				ShadowDamage = stats.ShadowDamage,
				HolyDamage = stats.HolyDamage,
				NatureDamage = stats.NatureDamage,
			};

			// Also bringing in the trigger events from DPSDK - 
			// Since I'm going to move the +Def bonus for the Sigil of the Unfaltering Knight
			// To a special effect.  Also there are alot of OnUse and OnEquip special effects
			// That probably aren't being taken into effect.
			foreach (SpecialEffect effect in stats.SpecialEffects()) {
				if (HasRelevantStats(effect.Stats)) {
					s.AddSpecialEffect(effect);
				}
			}
			return s;
		}
 
		/// <summary>
		/// Tests whether there are positive relevant stats in the Stats object.
		/// </summary>
		/// <param name="stats">The complete Stats object containing all stats.</param>
		/// <returns>True if any of the non-Zero stats in the Stats are relevant.  
		/// I realize that there aren't many stats that have negative values, but for completeness.</returns>
		public override bool HasRelevantStats(Stats stats) {
			foreach (SpecialEffect effect in stats.SpecialEffects()) {
				if (relevantStats(effect.Stats)) {
					if (effect.Trigger == Trigger.DamageDone ||
						effect.Trigger == Trigger.DamageOrHealingDone ||
						effect.Trigger == Trigger.DamageTaken ||
						effect.Trigger == Trigger.DamageTakenMagical ||
						effect.Trigger == Trigger.DamageTakenPhysical ||
						effect.Trigger == Trigger.DamageSpellCast ||
						effect.Trigger == Trigger.DamageSpellCrit ||
						effect.Trigger == Trigger.DamageSpellHit ||
						effect.Trigger == Trigger.SpellCast ||
						effect.Trigger == Trigger.SpellCrit ||
						effect.Trigger == Trigger.SpellHit ||
						effect.Trigger == Trigger.DoTTick ||
						effect.Trigger == Trigger.MeleeCrit ||
						effect.Trigger == Trigger.MeleeHit ||
                        effect.Trigger == Trigger.OffHandHit ||
                        effect.Trigger == Trigger.CurrentHandHit ||
                        effect.Trigger == Trigger.MainHandHit ||
                        effect.Trigger == Trigger.PhysicalCrit ||
						effect.Trigger == Trigger.PhysicalHit ||
						effect.Trigger == Trigger.Use)
					{
						return relevantStats(effect.Stats);
					}
					// if it has a DK specific trigger, then just return true.
					else if (
						effect.Trigger == Trigger.BloodStrikeHit ||
						effect.Trigger == Trigger.HeartStrikeHit ||
						effect.Trigger == Trigger.BloodStrikeOrHeartStrikeHit ||
						effect.Trigger == Trigger.IcyTouchHit ||
						effect.Trigger == Trigger.PlagueStrikeHit ||
						effect.Trigger == Trigger.RuneStrikeHit ||
						effect.Trigger == Trigger.ObliterateHit ||
						effect.Trigger == Trigger.ScourgeStrikeHit)
					{
						return true;
					}

				}
			}
			return relevantStats(stats);
		}

		/// <summary>
		/// Helper function for HasRelevantStats() function of the base class.
		/// </summary>
		/// <param name="stats"></param>
		/// <returns>true == the stats object has interesting things for this model.</returns>
		private bool relevantStats(Stats stats) {
			bool bResults = false;
			// Core stats
			bResults |= (stats.Strength != 0);
			bResults |= (stats.Agility != 0);
			bResults |= (stats.Stamina != 0);
			bResults |= (stats.Armor != 0);
			bResults |= (stats.BonusArmor != 0);
			bResults |= (stats.Health != 0);
			bResults |= (stats.BattlemasterHealth != 0);

			bResults |= (stats.HighestStat != 0);
			bResults |= (stats.Paragon != 0);
			bResults |= (stats.DeathbringerProc != 0);

			// Defense stats
			bResults |= (stats.DodgeRating != 0);
			bResults |= (stats.DefenseRating != 0);
			bResults |= (stats.ParryRating != 0);

			bResults |= (stats.Dodge != 0);
			bResults |= (stats.Parry != 0);
			bResults |= (stats.Miss != 0);
			bResults |= (stats.Defense != 0);
			bResults |= (stats.DamageAbsorbed != 0);

			bResults |= (stats.Resilience != 0);
            bResults |= (stats.SpellPenetration != 0);

			// Offense stats
			bResults |= (stats.AttackPower != 0);
			bResults |= (stats.HitRating != 0);
			bResults |= (stats.CritRating != 0);
			bResults |= (stats.ArmorPenetration != 0);
			bResults |= (stats.ArmorPenetrationRating != 0);
			bResults |= (stats.ExpertiseRating != 0);
			bResults |= (stats.Expertise != 0);
			bResults |= (stats.HasteRating != 0);
			bResults |= (stats.WeaponDamage != 0);
			bResults |= (stats.PhysicalCrit != 0);
			bResults |= (stats.PhysicalHaste != 0);
			bResults |= (stats.PhysicalHit != 0);
			bResults |= (stats.SpellHit != 0);

			bResults |= (stats.Healed != 0);
			bResults |= (stats.HealthRestore != 0);
			bResults |= (stats.HealthRestoreFromMaxHealth != 0);
			bResults |= (stats.Hp5 != 0);

			// Bonus to stats
			bResults |= (stats.BonusArmorMultiplier != 0);
			bResults |= (stats.BaseArmorMultiplier != 0);
			bResults |= (stats.BonusHealthMultiplier != 0);
			bResults |= (stats.BonusStrengthMultiplier != 0);
			bResults |= (stats.BonusStaminaMultiplier != 0);
			bResults |= (stats.BonusAgilityMultiplier != 0);
			bResults |= (stats.BonusCritMultiplier != 0);
			bResults |= (stats.BonusSpellCritMultiplier != 0);
			bResults |= (stats.BonusAttackPowerMultiplier != 0);
			bResults |= (stats.BonusPhysicalDamageMultiplier != 0);
			bResults |= (stats.BonusDamageMultiplier != 0);
			bResults |= (stats.DamageTakenMultiplier != 0);
			bResults |= (stats.ThreatIncreaseMultiplier != 0);
			bResults |= (stats.ThreatReductionMultiplier != 0);

			// Damage Multipliers:
			bResults |= (stats.BonusShadowDamageMultiplier != 0);
			bResults |= (stats.BonusFrostDamageMultiplier != 0);
			bResults |= (stats.BonusDiseaseDamageMultiplier != 0);
			bResults |= (stats.BonusRuneStrikeMultiplier != 0);
			bResults |= (stats.BonusBloodStrikeDamageMultiplier != 0);
			bResults |= (stats.BonusHeartStrikeDamageMultiplier != 0);

			// Bulk Damage:
			bResults |= (stats.BonusBloodStrikeDamage != 0);
			bResults |= (stats.BonusDeathCoilDamage != 0);
			bResults |= (stats.BonusDeathStrikeDamage != 0);
			bResults |= (stats.BonusFrostStrikeDamage != 0);
			bResults |= (stats.BonusHeartStrikeDamage != 0);
			bResults |= (stats.BonusIcyTouchDamage != 0);
			bResults |= (stats.BonusObliterateDamage != 0);
			bResults |= (stats.BonusScourgeStrikeDamage != 0);
			bResults |= (stats.BonusHowlingBlastDamage != 0);
			bResults |= (stats.BonusFrostWeaponDamage != 0);

			bResults |= (stats.BonusPerDiseaseBloodStrikeDamage != 0);
			bResults |= (stats.BonusPerDiseaseHeartStrikeDamage != 0);
			bResults |= (stats.BonusPerDiseaseObliterateDamage != 0);
			bResults |= (stats.BonusPerDiseaseScourgeStrikeDamage != 0);

			// Others
			bResults |= (stats.BonusDeathCoilCrit != 0);
			bResults |= (stats.BonusDeathStrikeCrit != 0);
			bResults |= (stats.BonusFrostStrikeCrit != 0);
			bResults |= (stats.BonusObliterateCrit != 0);
			bResults |= (stats.BonusPlagueStrikeCrit != 0);
			bResults |= (stats.BonusScourgeStrikeCrit != 0);
			bResults |= (stats.BonusIceboundFortitudeDuration != 0);
			bResults |= (stats.BonusAntiMagicShellDamageReduction != 0);
			bResults |= (stats.BonusHealingReceived != 0);
			bResults |= (stats.RPp5 != 0);
			bResults |= (stats.TankDK_T10_2pc != 0);
			bResults |= (stats.TankDK_T10_4pc != 0);

			// Resistances
			bResults |= (stats.ArcaneResistance != 0);
			bResults |= (stats.FireResistance != 0);
			bResults |= (stats.FrostResistance != 0);
			bResults |= (stats.NatureResistance != 0);
			bResults |= (stats.ShadowResistance != 0);

			// Damage Procs
			bResults |= (stats.ArcaneDamage != 0);
			bResults |= (stats.FireDamage != 0);
			bResults |= (stats.FrostDamage != 0);
			bResults |= (stats.ShadowDamage != 0);
			bResults |= (stats.HolyDamage != 0);
			bResults |= (stats.NatureDamage != 0);

			// Filter out caster gear:
/*			if (bResults && stats.Strength == 0)
			{
				bResults = !((stats.Intellect != 0)
					|| (stats.Spirit != 0)
					|| (stats.Mp5 != 0)
					|| (stats.SpellPower != 0)
					|| (stats.Mana != 0)
					);
			}*/

			return bResults;
		}

		public override bool IsItemRelevant(Item item) {
			if (item.Slot == ItemSlot.Ranged && item.Type != ItemType.Sigil) { return false; }
			return base.IsItemRelevant(item);
		}
		#endregion

		#region Evaluations And Ratings
		/// <summary>Evaluate how many swings until the tank is next hit.</summary>
		/// <param name="PercAvoidance">a float that is a 0-1 value for % of total avoidance (Dodge + Parry + Miss)</param>
		/// <returns>Float of how many swings until the next hit. Should be > 1</returns>
		private float GetReactionTime(float PercAvoidance) {
			float fReactionTime = 0f;
			// check args.
			if (PercAvoidance < 0f || PercAvoidance > 1f) { return 0f; }// error
			fReactionTime = 1f / (1f - PercAvoidance);
			return fReactionTime;
		}

		/// <summary>
		/// Evaluate how many swings until the tank dies.
		/// </summary>
		/// <param name="PercAvoidance">a float values of Total avoidance 0-1 as a decimal percentage.</param>
		/// <param name="EffectiveHealth">Survival score</param>
		/// <param name="RawPerHit">What's the raw unmitigated damage coming in.</param>
		/// <returns>the number of hits until death.</returns>
		private float GetBurstTime(float PercAvoidance, float EffectiveHealth, float RawPerHit) {
			float fBurstTime = 0f;
			// check args.
			if (PercAvoidance < 0 || PercAvoidance > 1) { return 0f; } // error

			float fHvH = (EffectiveHealth / RawPerHit);

			fBurstTime = (1f / PercAvoidance) * ((1f / (float)Math.Pow((1f - PercAvoidance), fHvH)) - 1f);

			return fBurstTime;
		}

		/// <summary>
		/// Get the value for a sub-component of Survival
		/// </summary>
		/// <param name="fHealth">Current HP</param>
		/// <param name="fDR">Damage Reduction rate</param>
		/// <param name="PercValue">% value of the survival rank. valid range 0-1</param>
		/// <returns></returns>
		private float GetEffectiveHealth(float fHealth, float fDR, float PercValue)
		{
			// TotalSurvival == sum(Survival for each school)
			// Survival = (Health / (1 - DR)) * % damage inc from that school
			if (0f <= PercValue && PercValue <= 1f && fDR < 1f)
				return (fHealth / (1 - fDR)) * PercValue;
			else
				return 0;
		}

		/// <summary>
		/// Get the MitigationRating of the current setup.
		/// </summary>
		/// <returns>the value of the mitigation subpoint</returns>
		private float GetMitigationRating()
		{
			return 0f;
		}

		/// <summary>
		/// Get the Threat Rating of the current setup.
		/// </summary>
		/// <returns>the value of Threat per second.</returns>
		private float GetThreatRating()
		{
			return 0f;
		}
		#endregion 

		private float GetDPS(float fPerUnitDamage, float fDamFrequency)
		{
			if (fDamFrequency > 0)
				return fPerUnitDamage / fDamFrequency;
			return 0f;
		}
 
		public static float AddStatMultiplierStat(float statMultiplier, float newValue)
		{
			float updatedStatModifier = ((1 + statMultiplier) * (1 + newValue)) - 1f;
			return updatedStatModifier;
		}

		private float GetParryHastedDPS(float fParryChance, float fCharacterShotCount, float fBossAverageAttackSpeed, float fRotDuration, float fPerShotPhysical)
		{
			float fPhyDamageDPS = 0f;
			// What was the max POTENTIAL hasted damage?
			// The number of shots taken * the chance to be parried.
			// can't be higher than cap.
			float localParryChance = Math.Min(fParryChance, StatConversion.WHITE_PARRY_CHANCE_CAP[83-80]);
			// can't be lower than 0
			localParryChance = Math.Max(localParryChance, 0);
			float fShotsParried = fParryChance * fCharacterShotCount;
			float fBossParryHastedSpeed = fBossAverageAttackSpeed * (1f - 0.24f);

			float fTimeHasted = fShotsParried * fBossParryHastedSpeed;
			float fTimeNormal = fRotDuration - fTimeHasted;
			// Update the shot count w/ the new # of normal shots + the number of hasted shots.
			float fBossShotCountPerRot = (fTimeNormal / fBossAverageAttackSpeed) + fShotsParried;
			// How much DPS is the hasted amount?
			fPhyDamageDPS = (float)Math.Floor((fBossShotCountPerRot * fPerShotPhysical) / fRotDuration);
			return fPhyDamageDPS;
		}

		private float GetParryHastedAttackSpeed(float fParryChance, float fCharacterShotCount, float fBossAverageAttackSpeed, float fRotDuration)
		{
			// What was the max POTENTIAL hasted damage?
			// The number of shots taken * the chance to be parried.
			// can't be higher than cap.
			float localParryChance = Math.Min(fParryChance, StatConversion.WHITE_PARRY_CHANCE_CAP[83 - 80]);
			// can't be lower than 0
			localParryChance = Math.Max(localParryChance, 0);
			float fShotsParried = fParryChance * fCharacterShotCount;
			float fBossParryHastedSpeed = fBossAverageAttackSpeed * (1f - 0.24f);

			float fTimeHasted = fShotsParried * fBossParryHastedSpeed;
			float fTimeNormal = fRotDuration - fTimeHasted;
			// Update the shot count w/ the new # of normal shots + the number of hasted shots.
			float fBossShotCountPerRot = (fTimeNormal / fBossAverageAttackSpeed) + fShotsParried;
			// How much DPS is the hasted amount?
			return (fRotDuration / fBossShotCountPerRot);
		}

		/// <summary>Deserializes the model's CalculationOptions data object from xml</summary>
		/// <param name="xml">The serialized xml representing the model's CalculationOptions data object.</param>
		/// <returns>The model's CalculationOptions data object.</returns>
		public override ICalculationOptionBase DeserializeDataObject(string xml) {
			XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsTankDK));
			StringReader reader = new StringReader(xml);
			CalculationOptionsTankDK calcOpts = serializer.Deserialize(reader) as CalculationOptionsTankDK;
			return calcOpts;
		}
	}
}
