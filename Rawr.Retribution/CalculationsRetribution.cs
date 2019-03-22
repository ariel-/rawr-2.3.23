using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Rawr.Retribution
{
    [Rawr.Calculations.RawrModelInfo("Retribution", "Spell_Holy_CrusaderStrike", CharacterClass.Paladin)]
    public class CalculationsRetribution : CalculationsBase
    {

        private const decimal simulationTime = 20000m;
        private const decimal allRotationsSimulationTime = 1000m;
        private const string rotationsChartName = "Rotations";
        private const string allRotationsChartName = "All Rotations (less precise)";


        #region Model Properties
        public override List<GemmingTemplate> DefaultGemmingTemplates
        {
            get
            {
                ////Relevant Gem IDs

                //Prismatic
                int[] tear = { 42701, 42702, 49110 }; //+stats

                //Yellow
                int[] rigid = { 39915, 40014, 40125, 42156 };  // +hit
                int[] quick = { 39918, 40017, 40128, 42150 };  // +haste

                //Red
                int[] bold = { 39900, 39996, 40111, 42142 };  // +str
                int[] precise = { 39910, 40003, 40118, 42154 }; // +exp

                //Orange (dual color gems don't have a JC variant)
                int[] inscribed = { 39947, 40037, 40142 };  // +str,+crit
                int[] etched = { 39948, 40038, 40143 };  // +str,+hit
                int[] fierce = { 39951, 40041, 40146 };  // +str,+haste
                // int[] expcrit = { ????, ????, ????, } // +exp,+crit  Doesn't exist
                int[] accurate = { 39966, 40058, 40162 };  // +exp,+hit
                // int[] exphaste = { ????, ????, ????, } // +exp,+haste  Doesn't exist

                //Purple
                int[] sovereign = { 39934, 40022, 40129 }; // +str,+stam
                int[] guardians = { 39940, 40034, 40141 }; // +exp, +stam

                //Green
                int[] vivid = { 39975, 40088, 40166 }; // +hit,+stam

                //Meta
                int relentless = 41398;
                //int chaotic = 41285;

                List<GemmingTemplate> retval = new List<GemmingTemplate>();

                string[] groupName = { "Uncommon", "Rare", "Epic" };
                for (int i = 0; i < 3; i++)
                {
                    retval.Add(new GemmingTemplate()    // Strength every slot - Don't match colors
                    {
                        Model = "Retribution",
                        Group = groupName[i],
                        RedId = bold[i],
                        YellowId = bold[i],
                        BlueId = bold[i],
                        PrismaticId = bold[i],
                        MetaId = relentless,
                        Enabled = i == 2
                    });
                    retval.Add(new GemmingTemplate()    // Strength in every slot - Match colors (Blue socket is least desired and gets the (unique) nightmare tear).
                    {
                        Model = "Retribution",
                        Group = groupName[i],
                        RedId = bold[i],
                        YellowId = inscribed[i],
                        BlueId = tear[i],
                        PrismaticId = bold[i],
                        MetaId = relentless,
                        Enabled = i == 2
                    });
                    retval.Add(new GemmingTemplate()    // Strength in every slot - Match colors (This is a catch in case we don't have blue sockets, in that case, yellow gets the tear to activate the meta).  Putting sovereign here in blue is just filler.
                    {
                        Model = "Retribution",
                        Group = groupName[i],
                        RedId = bold[i],
                        YellowId = tear[i],
                        BlueId = sovereign[i],
                        PrismaticId = bold[i],
                        MetaId = relentless,
                        Enabled = i == 2
                    });
                    retval.Add(new GemmingTemplate()    // Strength in every slot - match colors - no nightmare tear (in case we need to activate a 2nd bonus dependant on a blue)
                    {
                        Model = "Retribution",
                        Group = groupName[i],
                        RedId = bold[i],
                        YellowId = inscribed[i],
                        BlueId = sovereign[i],
                        PrismaticId = bold[i],
                        MetaId = relentless,
                        Enabled = i == 2
                    });
                    retval.Add(new GemmingTemplate()    // Hit in every slot - Match colors (catch for hit deficiency)
                    {
                        Model = "Retribution",
                        Group = groupName[i],
                        RedId = etched[i],
                        YellowId = rigid[i],
                        BlueId = vivid[i],
                        PrismaticId = rigid[i],
                        MetaId = relentless,
                        Enabled = i == 2
                    });
                    retval.Add(new GemmingTemplate()    // Expertise in every slot - Match colors (catch for expertise deficiency)
                    {
                        Model = "Retribution",
                        Group = groupName[i],
                        RedId = precise[i],
                        YellowId = accurate[i],         // Fill with hit, no other (useful) yellow sockets have expertise.
                        BlueId = guardians[i],
                        PrismaticId = precise[i],
                        MetaId = relentless,
                        Enabled = i == 2
                    });
                }

                retval.Add(new GemmingTemplate()        // Strength in every slot - Don't match colors, Bold JC gems everywhere.
                {
                    Model = "Retribution",
                    Group = "Jeweler",
                    RedId = bold[3],
                    YellowId = bold[3],
                    BlueId = bold[3],
                    PrismaticId = bold[3],
                    MetaId = relentless
                });
                retval.Add(new GemmingTemplate()        // Strength in every slot - match colors (Only red/prismatic gets JC gems)
                {
                    Model = "Retribution",
                    Group = "Jeweler",
                    RedId = bold[3],
                    YellowId = inscribed[2],
                    BlueId = tear[2],
                    PrismaticId = bold[3],
                    MetaId = relentless
                });

                return retval;
            }
        }

        /// <summary>
        /// Buffs that will be enabled by default in the given character object
        /// </summary>
        public override void SetDefaults(Character character)
        {
            character.ActiveBuffsAdd("Strength of Earth Totem");
            character.ActiveBuffsAdd("Blessing of Might");
            character.ActiveBuffsAdd("Improved Blessing of Might");
            character.ActiveBuffsAdd("Unleashed Rage");
            character.ActiveBuffsAdd("Sanctified Retribution");
            character.ActiveBuffsAdd("Swift Retribution");
            character.ActiveBuffsAdd("Arcane Intellect");
            character.ActiveBuffsAdd("Commanding Shout");
            character.ActiveBuffsAdd("Leader of the Pack");
            character.ActiveBuffsAdd("Windfury Totem");
            character.ActiveBuffsAdd("Elemental Oath");
            character.ActiveBuffsAdd("Power Word: Fortitude");
            character.ActiveBuffsAdd("Improved Power Word: Fortitude");
            character.ActiveBuffsAdd("Mark of the Wild");
            character.ActiveBuffsAdd("Improved Mark of the Wild");
            character.ActiveBuffsAdd("Sunder Armor");
            character.ActiveBuffsAdd("Faerie Fire");
            character.ActiveBuffsAdd("Heart of the Crusader");
            character.ActiveBuffsAdd("Blood Frenzy");
            character.ActiveBuffsAdd("Improved Scorch");
            character.ActiveBuffsAdd("Curse of the Elements");
            character.ActiveBuffsAdd("Misery");
            character.ActiveBuffsAdd("Blessing of Kings");
            character.ActiveBuffsAdd("Totem of Wrath (Spell Power)");
            character.ActiveBuffsAdd("Blessing of Kings (Str/Sta Bonus)");
            character.ActiveBuffsAdd("Flask of Endless Rage");
            character.ActiveBuffsAdd("Fish Feast");
        }

        private static List<string> _relevantGlyphs;
        /// <summary>
        /// List of glyphs that will be available in the Glyph subtab of the Talents tab.
        /// </summary>
        public override List<string> GetRelevantGlyphs()
        {
            if (_relevantGlyphs == null)
            {
                _relevantGlyphs = new List<string>();
                _relevantGlyphs.Add("Glyph of Judgement");
                _relevantGlyphs.Add("Glyph of Exorcism");
                _relevantGlyphs.Add("Glyph of Sense Undead");
                _relevantGlyphs.Add("Glyph of Consecration");
                _relevantGlyphs.Add("Glyph of Seal of Blood");
                _relevantGlyphs.Add("Glyph of Seal of Command");
                _relevantGlyphs.Add("Glyph of Seal of Vengeance");
                _relevantGlyphs.Add("Glyph of Seal of Righteousness");
                _relevantGlyphs.Add("Glyph of Crusader Strike");
                _relevantGlyphs.Add("Glyph of Hammer of Wrath");
                _relevantGlyphs.Add("Glyph of Avenging Wrath");
            }
            return _relevantGlyphs;
        }

        private string[] _optimizableCalculationLabels = null;
        /// <summary>
        /// Labels of the stats available to the Optimizer.
        /// </summary>
        /// The list of labels listed here needs to match with the list in GetOptimizableCalculationValue override in CharacterCalculationsRetribution.cs
        public override string[] OptimizableCalculationLabels
        {
            get
            {
                if (_optimizableCalculationLabels == null)
                    _optimizableCalculationLabels = new string[] {
					"Health",
                    "% Chance to Miss (Melee)",
                    "% Chance to Miss (Spells)",
                    "% Chance to be Dodged",
                    "% Chance to be Parried",
                    "% Chance to be Avoided (Melee/Dodge)",
					};
                return _optimizableCalculationLabels;
            }
        }

        /// <summary>
        /// Dictionary<string, Color> that includes the names of each rating which your model will use,
        /// and a color for each. These colors will be used in the charts.
        /// 
        /// EXAMPLE: 
        /// subPointNameColors = new Dictionary<string, System.Drawing.Color>();
        /// subPointNameColors.Add("Mitigation", System.Drawing.Colors.Red);
        /// subPointNameColors.Add("Survival", System.Drawing.Colors.Blue);
        /// </summary>
#if RAWR3
        private Dictionary<string, System.Windows.Media.Color> _subPointNameColors = null;
        public override Dictionary<string, System.Windows.Media.Color> SubPointNameColors
        {
            get
            {
                if (_subPointNameColors == null)
                {
                    _subPointNameColors = new Dictionary<string, System.Windows.Media.Color>();
                    _subPointNameColors.Add("DPS", System.Windows.Media.Colors.Red);
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
                    _subPointNameColors.Add("DPS", System.Drawing.Color.Red);
                }
                return _subPointNameColors;
            }
        }
#endif

        /// <summary>
        /// Creates the CalculationOptionPanel
        /// </summary>
#if RAWR3
        private ICalculationOptionsPanel _calculationOptionsPanel = null;
        public override ICalculationOptionsPanel CalculationOptionsPanel
#else
        private CalculationOptionsPanelBase _calculationOptionsPanel = null;
        public override CalculationOptionsPanelBase CalculationOptionsPanel
#endif
        {
            get { return _calculationOptionsPanel ?? (_calculationOptionsPanel = new CalculationOptionsPanelRetribution()); }
        }

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
        /// 
        /// Values used here need to be defined via the GetCharacterDisplayCalculationValues() member
        /// in CharacterCalculationsRetribution.cs
        /// </summary>
        public override string[] CharacterDisplayCalculationLabels
        {
            get
            {
                if (_characterDisplayCalculationLabels == null)
                {
                    List<string> labels = new List<string>(new string[]
                    {
                        "Basic Stats:Health",
                        "Basic Stats:Mana",
                        "Basic Stats:Strength",
                        "Basic Stats:Agility",
                        "Basic Stats:Attack Power",
                        "Basic Stats:Crit Chance",
                        "Basic Stats:Miss Chance",
                        "Basic Stats:Dodge Chance",
                        "Basic Stats:Melee Haste",
                        "Basic Stats:Weapon Damage",
                        "Basic Stats:Attack Speed",
                        "DPS Breakdown:Total DPS",
                        "DPS Breakdown:White",
                        "DPS Breakdown:Seal",
                        "DPS Breakdown:Crusader Strike",
                        "DPS Breakdown:Judgement",
                        "DPS Breakdown:Consecration",
                        "DPS Breakdown:Exorcism",
                        "DPS Breakdown:Divine Storm",
                        "DPS Breakdown:Hammer of Wrath",
                        "DPS Breakdown:Hand of Reckoning",
                        "DPS Breakdown:Other*From trinket procs",
                        "Rotation Info:Chosen Rotation",
                        "Rotation Info:Average SoV Stack",
                        "Rotation Info:SoV Overtake*How long you need to dps a target for SoV to do more DPS then SoR",
                        "Rotation Info:Crusader Strike CD",
                        "Rotation Info:Judgement CD",
                        "Rotation Info:Consecration CD",
                        "Rotation Info:Exorcism CD",
                        "Rotation Info:Divine Storm CD",
                        "Rotation Info:Hammer of Wrath CD",
                    });
                    _characterDisplayCalculationLabels = labels.ToArray();
                }
                return _characterDisplayCalculationLabels;
            }
        }
        #endregion

        public override CharacterClass TargetClass { get { return CharacterClass.Paladin; } }
        public override ComparisonCalculationBase CreateNewComparisonCalculation()
        {
            return new ComparisonCalculationRetribution();
        }

        public override CharacterCalculationsBase CreateNewCharacterCalculations()
        {
            return new CharacterCalculationsRetribution();
        }

        public override ICalculationOptionBase DeserializeDataObject(string xml)
        {
            System.Xml.Serialization.XmlSerializer serializer =
            new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsRetribution));
            System.IO.StringReader reader = new System.IO.StringReader(xml);
            CalculationOptionsRetribution calcOpts = serializer.Deserialize(reader) as CalculationOptionsRetribution;
            return calcOpts;
        }

        /// <summary>
        /// Processing of the Experimental flags. Experimental string is being set from 
        /// CalculationOptionPanelRetribution.cs. And is being parsed and split into relevant
        /// flags here.
        /// </summary>
        //private static bool Experimental_OldRelevancy = false;
        internal static string Experimental
        {
            set
            {
                // Revert all settings to default
                //Experimental_OldRelevancy = false;

                // And apply parameters
                string[] Experiments = value.Split(';');
                foreach (string Experiment in Experiments)
                {
                    int iOpen = Experiment.IndexOf('(');
                    int iClose = Experiment.IndexOf(')');
                    if (iOpen >= 0 && iClose > iOpen)
                    {
                        string Command = Experiment.Substring(0, iOpen).Trim();
                        string[] Parms = Experiment.Substring(iOpen + 1, iClose - 1 - iOpen).Split(',');
                        string Rest = Experiment.Substring(iClose + 1).Trim();

                        if (Command.Length > 0 && Rest.Length == 0)
                        {
                            /*if (Command == "SetOldRelevancy" && Parms.Length == 1 && Parms[0].Trim().Length == 0)
                            {
                                Experimental_OldRelevancy = true;
                            }*/
                        }
                    }
                }
            }
        }

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
        public override CharacterCalculationsBase GetCharacterCalculations(
            Character character, 
            Item additionalItem, 
            bool referenceCalculation, 
            bool significantChange, 
            bool needsDisplayCalculations)
        {
            return GetCharacterCalculations(
                character,
                additionalItem,
                GetCharacterRotation(character, additionalItem));
        }

        public CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, Rotation rot)
        {
            CalculationOptionsRetribution calcOpts = character.CalculationOptions as CalculationOptionsRetribution;
            float fightLength = calcOpts.FightLength * 60f;
            PaladinTalents talents = character.PaladinTalents;
            CombatStats combats = rot.Combats;
            Stats stats = combats.Stats;

            CharacterCalculationsRetribution calc = new CharacterCalculationsRetribution();
            calc.BasicStats = GetCharacterStats(character, additionalItem, false, null, 0);

            calc.AttackSpeed = combats.AttackSpeed;
            calc.WeaponDamage = combats.WeaponDamage;
            calc.ToMiss = combats.GetMeleeMissChance();
            calc.ToBeDodged = combats.GetToBeDodgedChance();
            calc.ToBeParried = combats.GetToBeParriedChance();
            calc.ToBeResisted = combats.GetSpellMissChance();

            calc.OtherDPS = new MagicDamage(combats, stats.ArcaneDamage).AverageDamage()
                          + new MagicDamage(combats, stats.FireDamage).AverageDamage()
                          + new MagicDamage(combats, stats.ShadowDamage).AverageDamage()
                          + new MagicDamage(combats, stats.FrostDamage).AverageDamage()
                          + new MagicDamage(combats, stats.NatureDamage).AverageDamage()
                          + new MagicDamage(combats, stats.HolyDamage).AverageDamage();
            rot.SetDPS(calc);
            calc.OverallPoints = calc.DPSPoints;

            return calc;
        }

        public Rotation GetCharacterRotation(Character character, Item additionalItem)
        {
            CalculationOptionsRetribution options = 
                (CalculationOptionsRetribution)character.CalculationOptions;

            if (!options.SimulateRotation)
                return GetCharacterRotation(character, additionalItem, null, 0);

            if (options.Rotations.Count == 0)
                return GetCharacterRotation(
                    character, 
                    additionalItem,
                    SimulatorParameters.DefaultRotation(),
                    simulationTime);

            return FindBestRotation(
                character,
                additionalItem,
                options.Rotations, 
                simulationTime);
        }

        private Rotation FindBestRotation(
            Character character,
            Item additionalItem,
            IEnumerable<Ability[]> rotations,
            decimal simulationTime)
        {
            float maxDPS = 0;
            Rotation bestRotation = null;
            foreach (Ability[] rotation in rotations)
            {
                Rotation currentRotation = GetCharacterRotation(character, additionalItem, rotation, simulationTime);
                float currentDPS = currentRotation.DPS();
                if (currentDPS > maxDPS)
                {
                    maxDPS = currentDPS;
                    bestRotation = currentRotation;
                }
            }

            return bestRotation;
        }

        public Rotation GetCharacterRotation(
            Character character,
            Item additionalItem,
            Ability[] rotation,
            decimal simulationTime)
        {
            return CreateRotation(
                new CombatStats(
                    character, 
                    GetCharacterStats(character, additionalItem, true, rotation, simulationTime)),
                rotation,
                simulationTime);
        }

        /// <summary>
        /// GetCharacterStats is the 2nd-most calculation intensive method in a model. Here the model will
        /// combine all of the information about the character, including race, gear, enchants, buffs,
        /// calculationoptions, etc., to form a single combined Stats object. Three of the methods below
        /// can be called from this method to help total up stats: GetItemStats(character, additionalItem),
        /// GetEnchantsStats(character), and GetBuffsStats(character.ActiveBuffs).
        /// </summary>
        /// <param name="character">The character whose stats should be totaled.</param>
        /// <param name="additionalItem">An additional item to treat the character as wearing.
        /// This is used for gems, which don't have a slot on the character to fit in, so are just
        /// added onto the character, in order to get gem calculations.</param>
        /// <returns>A Stats object containing the final totaled values of all character stats.</returns>
        public override Stats GetCharacterStats(Character character, Item additionalItem)
        {
            return GetCharacterRotation(character, additionalItem).Combats.Stats;
        }

        public Rotation CreateRotation(CombatStats combats, Ability[] rotation, decimal simulationTime)
        {
            return combats.CalcOpts.SimulateRotation ?
                (Rotation)new Simulator(combats, rotation, simulationTime) :
                (Rotation)new EffectiveCooldown(combats);
        }

        // rotation and simulationTime are only required when computeAverageStats == true
        public Stats GetCharacterStats(
            Character character, 
            Item additionalItem, 
            bool computeAverageStats,
            Ability[] rotation,
            decimal simulationTime)
        {
            PaladinTalents talents = character.PaladinTalents;
            CalculationOptionsRetribution calcOpts = 
                character.CalculationOptions as CalculationOptionsRetribution;

            Stats statsRace = BaseStats.GetBaseStats(character.Level, CharacterClass.Paladin, character.Race);
            Stats statsBaseGear = GetItemStats(character, additionalItem);
            Stats statsBuffs = GetBuffsStats(character, calcOpts);

            // Adjust expertise for racial passive and for using Seal of Vengeance combined with the SoV glyph.
            statsRace.Expertise += BaseStats.GetRacialExpertise(character, ItemSlot.MainHand);
            if (talents.GlyphOfSealOfVengeance && calcOpts.Seal == SealOf.Vengeance)
                statsRace.Expertise += 10f;

            // Combine stats
            Stats stats = statsBaseGear + statsBuffs + statsRace;
            // If wanted, Average out any Proc and OnUse effects into the stats
            if (computeAverageStats)
            {
                Stats statsTmp = stats.Clone();
                ConvertRatings(statsTmp, talents, calcOpts.TargetLevel);                // Convert ratings so we have right value for haste, weaponspeed and talents etc.

                float fightLength = calcOpts.FightLength * 60f;
                CombatStats combats = new CombatStats(character, statsTmp);
                Rotation rot = CreateRotation(combats, rotation, simulationTime);

                // Average out proc effects, and add to global stats.
                Stats statsAverage = new Stats();
                foreach (SpecialEffect effect in stats.SpecialEffects())
                    statsAverage.Accumulate(
                        ProcessSpecialEffect(
                            effect, 
                            rot, 
                            calcOpts.Seal, 
                            combats.BaseWeaponSpeed, 
                            fightLength, 
                            calcOpts.StackTrinketReset));
                stats.Accumulate(statsAverage);

                // Death's Verdict/Vengeance (TOTC)
                // Known issue: We haven't yet accounted for bonus multipliers on str and agi
                if (stats.Strength > stats.Agility)
                    stats.Strength += stats.HighestStat + stats.Paragon;
                else
                    stats.Agility += stats.HighestStat + stats.Paragon;

                // Deathbringer's Will (ICC, Saurfang)
                // Paladins get str, hasterating and crit procs.
                // They're all equally likely so we'll divide the proc by 3
                stats.Strength += stats.DeathbringerProc / 3f;
                stats.HasteRating += stats.DeathbringerProc / 3f;
                stats.CritRating += stats.DeathbringerProc / 3f;
            }

            // No negative values (from possible charts)
            if (stats.Strength < 0)
                stats.Strength = 0;
            if (stats.Agility < 0)
                stats.Agility = 0;
            if (stats.AttackPower < 0)
                stats.AttackPower = 0;
            if (stats.ArmorPenetrationRating < 0)
                stats.ArmorPenetrationRating = 0;
            if (stats.ExpertiseRating < 0)
                stats.ExpertiseRating = 0;
            if (stats.HitRating < 0)
                stats.HitRating = 0;
            if (stats.CritRating < 0)
                stats.CritRating = 0;
            if (stats.HasteRating < 0)
                stats.HasteRating = 0;
            if (stats.SpellPower < 0)
                stats.SpellPower = 0;
        
            // ConvertRatings needs to be done AFTER accounting for the averaged stats, since stat multipliers 
            // should affect the averaged stats also.
            ConvertRatings(stats, talents, calcOpts.TargetLevel);

            return stats;
        }

        private Stats ProcessSpecialEffect(
            SpecialEffect effect, 
            Rotation rot, 
            SealOf seal, 
            float baseWeaponSpeed, 
            float fightLength, 
            int stackTrinketReset)
        {
            float trigger = 0f; 
            float procChance = 1f;

            switch (effect.Trigger)
            {
                case Trigger.MeleeCrit:
                    trigger = 1f / rot.GetMeleeCritsPerSec();
                    break;

                case Trigger.MeleeHit:
                    trigger = 1f / rot.GetMeleeAttacksPerSec();
                    break;

                // Experimental.
                case Trigger.MeleeAttack:   // [Tiny Abomination in a Jar] and [Shadowmourne]
                    float MeleeAttackPerSec =   rot.GetMeleeAttacksPerSec() +                   // Meleehit
                                                rot.SealProcsPerSec(rot.Seal) +                 // Seal hit
                                                rot.GetAbilityCritsPerSecond(rot.CS) +          // -+
                                                rot.GetAbilityCritsPerSecond(rot.DS) +          //  +- Righteous Vengeance application/refresh (100% on CS/DS/Judge crit)
                                                rot.GetAbilityCritsPerSecond(rot.Judge) +       // -+
                                                rot.GetAbilityHitsPerSecond(rot.Judge);         // Judgement debuf application (100% on J Hit)
                    if (seal == SealOf.Vengeance)
                        MeleeAttackPerSec += rot.White.ChanceToLand() / rot.Combats.AttackSpeed; // Holy Vengeance (100% on melee hit when using SoV)

                    trigger = 1f / MeleeAttackPerSec;                            
                    break;

                case Trigger.PhysicalCrit:
                    trigger = 1f / rot.GetPhysicalCritsPerSec();
                    break;

                case Trigger.PhysicalHit:
                    trigger = 1f / rot.GetPhysicalAttacksPerSec();
                    break;

                case Trigger.DamageDone:
                    trigger = 1f / rot.GetAttacksPerSec();
                    break;

                case Trigger.SpellHit:                  // This technically counts heals also, but we aren't modelling ret doing heals
                case Trigger.DamageSpellHit:
                    trigger = 1f / rot.GetSpellAttacksPerSec();
                    break;

                case Trigger.SpellCrit:                 // This technically counts heals also, but we aren't modelling ret doing heals
                case Trigger.DamageSpellCrit:
                    trigger = 1f / rot.GetSpellCritsPerSec(); 
                    break;

                case Trigger.DamageOrHealingDone:
                    // Need to add Self-heals
                    trigger = 1f / rot.GetAttacksPerSec();
                    break;

                case Trigger.CrusaderStrikeHit:
                    trigger = rot.GetCrusaderStrikeCD();
                    procChance = rot.CS.ChanceToLand();
                    break;

                case Trigger.JudgementHit:
                    trigger = rot.GetJudgementCD();
                    procChance = rot.Judge.ChanceToLand();
                    break;

                case Trigger.DoTTick:                   // SoV is also a dot tick.
                case Trigger.SealOfVengeanceTick:
                    if (seal == SealOf.Vengeance)
                    {
                        trigger = 3f;
                        procChance = 1f;
                    }
                    else
                    {
                        return new Stats();
                    }
                    break;

                case Trigger.Use:
                    trigger = 0f;
                    procChance = 1f;
                    break;

                default:
                    return new Stats();
            }

            if (effect.Trigger == Trigger.Use && effect.Stats._rawSpecialEffectData != null)
            {
                // Run Recursive Effects (like Victor's Call)
                Stats SubStats = ProcessSpecialEffect(effect.Stats._rawSpecialEffectData[0],rot,seal,baseWeaponSpeed,fightLength,stackTrinketReset);
                float upTime = effect.GetAverageUptime(trigger,procChance);
                Stats retVal = new Stats();
                retVal.Accumulate(SubStats, upTime);
                return retVal;
            }
            else if (effect.MaxStack > 1)
            {
                if (effect.Stats.MoteOfAnger > 0)
                {
                    // When in effect stats, MoteOfAnger is % of melee hits
                    // When in character stats, MoteOfAnger is average procs per second
                    return new Stats() { MoteOfAnger = effect.Stats.MoteOfAnger * effect.GetAverageProcsPerSecond(trigger, procChance, baseWeaponSpeed, fightLength) / effect.MaxStack };
                }
                else
                {
                    Stats tempStats = null;
                    foreach (SpecialEffect subeffect in effect.Stats.SpecialEffects())
                    {
                        tempStats = ProcessSpecialEffect(subeffect, rot, seal, baseWeaponSpeed, effect.Duration, 0);
                    }

                    if (tempStats != null) 
                        return tempStats * effect.GetAverageStackSize(trigger, procChance, baseWeaponSpeed, fightLength, stackTrinketReset);
                    else 
                        return effect.Stats * effect.GetAverageStackSize(trigger, procChance, baseWeaponSpeed, fightLength, stackTrinketReset);
                }
            }
            else return effect.GetAverageStats(trigger, procChance, baseWeaponSpeed, fightLength);
        }

        // Combine talents and buffs into primary and secondary stats.
        // Convert ratings into their percentage values.
        private void ConvertRatings(Stats stats, PaladinTalents talents, int targetLevel)
        {
            // Primary stats
            stats.Strength *= (1 + stats.BonusStrengthMultiplier) * (1f + talents.DivineStrength * .03f);
            stats.Agility *= (1 + stats.BonusAgilityMultiplier);
            stats.Stamina *= (1 + stats.BonusStaminaMultiplier) * 
                (1f + talents.SacredDuty * .04f) * 
                (1f + talents.CombatExpertise * .02f);
            stats.Intellect *= (1 + stats.BonusIntellectMultiplier) * (1f + talents.DivineIntellect * .02f);

            // Secondary stats
            // GetManaFromIntellect/GetHealthFromStamina account for the fact 
            // that the first 20 Int/Sta only give 1 Mana/Health each.
            stats.Mana += StatConversion.GetManaFromIntellect(stats.Intellect, CharacterClass.Paladin) * (1f + stats.BonusManaMultiplier);
            stats.Health += StatConversion.GetHealthFromStamina(stats.Stamina, CharacterClass.Paladin);
            stats.AttackPower = (stats.AttackPower + stats.Strength * 2) * (1 + stats.BonusAttackPowerMultiplier);

            // Combat ratings
            stats.Expertise += talents.CombatExpertise * 2 + StatConversion.GetExpertiseFromRating(stats.ExpertiseRating, CharacterClass.Paladin);
            stats.PhysicalHit += StatConversion.GetPhysicalHitFromRating( stats.HitRating, CharacterClass.Paladin);
            stats.SpellHit += StatConversion.GetSpellHitFromRating(stats.HitRating, CharacterClass.Paladin);

            float talentCrit = 
                talents.CombatExpertise * .02f + 
                talents.Conviction * .01f + 
                talents.SanctityOfBattle * .01f;
            stats.PhysicalCrit += 
                StatConversion.GetPhysicalCritFromRating(stats.CritRating, CharacterClass.Paladin) + 
                StatConversion.GetPhysicalCritFromAgility(stats.Agility, CharacterClass.Paladin) + 
                talentCrit + 
                StatConversion.NPC_LEVEL_CRIT_MOD[targetLevel - 80]; // Mob crit suppression
            stats.SpellCrit += stats.SpellCritOnTarget + 
                StatConversion.GetSpellCritFromRating(stats.CritRating, CharacterClass.Paladin) + 
                StatConversion.GetSpellCritFromIntellect(stats.Intellect, CharacterClass.Paladin) + 
                talentCrit +
                StatConversion.NPC_LEVEL_CRIT_MOD[targetLevel - 80]; // Mob crit suppression

            stats.PhysicalHaste = (1f + stats.PhysicalHaste) * 
                (1f + StatConversion.GetPhysicalHasteFromRating(stats.HasteRating, CharacterClass.Paladin))
                - 1f;
            stats.SpellHaste = (1f + stats.SpellHaste) * 
                (1f + StatConversion.GetSpellHasteFromRating(stats.HasteRating, CharacterClass.Paladin))
                - 1f;

            stats.SpellPower += 
                stats.Strength * talents.TouchedByTheLight * .2f + 
                stats.AttackPower * talents.SheathOfLight * .1f;
        }

        public Stats GetBuffsStats(Character character, CalculationOptionsRetribution calcOpts) 
        {
            List<Buff> buffs = new List<Buff>(character.ActiveBuffs);

            var buffStats = GetBuffsStats(buffs);

            // If the character itself has any rank of Swift Retribution.
            // Improved Moonkin Form and different ranks of Swift Retribution don't stack.
            // Only the strongest one must be in ActiveBuffs
            if ((character.PaladinTalents.SwiftRetribution != 0) &&
                !character.ActiveBuffs.Contains(Buff.GetBuffByName("Swift Retribution")) &&
                !character.ActiveBuffs.Contains(Buff.GetBuffByName("Improved Moonkin Form")))
            {
                Stats additionalStats = new Stats();
                additionalStats.PhysicalHaste = character.PaladinTalents.SwiftRetribution * 0.01f;
                additionalStats.RangedHaste = character.PaladinTalents.SwiftRetribution * 0.01f;
                additionalStats.SpellHaste = character.PaladinTalents.SwiftRetribution * 0.01f;

                buffStats += additionalStats;
            }

            if ((character.PaladinTalents.HeartOfTheCrusader != 0) &&
                !character.ActiveBuffs.Contains(Buff.GetBuffByName("Heart of the Crusader")) &&
                !character.ActiveBuffs.Contains(Buff.GetBuffByName("Master Poisoner")) &&
                !character.ActiveBuffs.Contains(Buff.GetBuffByName("Totem of Wrath")))
            {
                Stats additionalStats = new Stats();
                additionalStats.PhysicalCrit = character.PaladinTalents.HeartOfTheCrusader * 0.01f;
                additionalStats.SpellCritOnTarget = character.PaladinTalents.HeartOfTheCrusader * 0.01f;

                buffStats += additionalStats;
            }

            if ((character.PaladinTalents.SanctifiedRetribution != 0) &&
                !character.ActiveBuffs.Contains(Buff.GetBuffByName("Sanctified Retribution")) &&
                !character.ActiveBuffs.Contains(Buff.GetBuffByName("Arcane Empowerment")) &&
                !character.ActiveBuffs.Contains(Buff.GetBuffByName("Ferocious Inspiration")))
            {
                Stats additionalStats = new Stats();
                additionalStats.BonusDamageMultiplier = character.PaladinTalents.SanctifiedRetribution * 0.03f;

                buffStats += additionalStats;
            }

            return buffStats;
        }


        #region Relevancy Methods

        /// <summary>
        /// List of itemtypes that are relevant for retribution
        /// </summary>
        private List<ItemType> _relevantItemTypes = null;
        public override List<ItemType> RelevantItemTypes
        {
            get
            {
                return _relevantItemTypes ?? (_relevantItemTypes = new List<ItemType>(new ItemType[]
                {
                    ItemType.None,
                    ItemType.Leather,
                    ItemType.Mail,
                    ItemType.Plate,
                    ItemType.Libram,
                    ItemType.Polearm,
                    ItemType.TwoHandAxe,
                    ItemType.TwoHandMace,
                    ItemType.TwoHandSword
                }));
            }
        }

        /// <summary>
        /// List of SpecialEffect Triggers that are relevant for retribution model
        /// Ever trigger listed here needs an implementation in ProcessSpecialEffects()
        /// A trigger not listed here should not appear in ProcessSpecialEffects()
        /// </summary>
        internal static List<Trigger> _RelevantTriggers = null;
        internal static List<Trigger> RelevantTriggers
        {
            get
            {
                return _RelevantTriggers ?? (_RelevantTriggers = new List<Trigger>() {
                            Trigger.Use,
                            Trigger.SpellCrit,        
                            Trigger.SpellHit,             
                            Trigger.DamageSpellCrit,        
                            Trigger.DamageSpellHit,         // Black magic enchant ?
                            Trigger.PhysicalCrit,
                            Trigger.PhysicalHit,
                            Trigger.MeleeCrit,
                            Trigger.MeleeHit,
                            Trigger.MeleeAttack,
                            Trigger.DamageDone,
                            Trigger.DamageOrHealingDone,    // Darkmoon Card: Greatness
                            Trigger.DoTTick,
                            //Trigger.DamageTaken,
                            //Trigger.DamageAvoided,
                            Trigger.JudgementHit,
                            Trigger.CrusaderStrikeHit,
                            Trigger.SealOfVengeanceTick,
                        });
            }
            //set { _RelevantTriggers = value; }
        }


        public override bool IsItemRelevant(Item item)
        {
            // First we let normal rules (profession, class, relevant stats) decide
            bool relevant = base.IsItemRelevant(item);

            // Next we use our special stat relevancy filtering.
            if (relevant)
                relevant = HasPrimaryStats(item.Stats) || (HasSecondaryStats(item.Stats) && !HasUnwantedStats(item.Stats));

            return relevant;
        }

        public override bool IsBuffRelevant(Buff buff, Character character)
        {
            // First we let normal rules (profession, class, relevant stats) decide
            bool relevant = base.IsBuffRelevant(buff, character);

            // Temporary FIX (?): buf.AllowedClasses is not currently being tested as part of base.IsBuffRelevant(). So we'll do it ourselves.
            if (relevant && !buff.AllowedClasses.Contains(CharacterClass.Paladin))
                relevant = false;

            // Next we use our special stat relevancy filtering on consumables. (party buffs only need filtering on relevant stats)
            if (relevant && (buff.Group == "Elixirs and Flasks" || buff.Group == "Potion" || buff.Group == "Food" || buff.Group == "Scrolls" || buff.Group == "Temporary Buffs"))
                relevant = HasPrimaryStats(buff.Stats) || (HasSecondaryStats(buff.Stats) && !HasUnwantedStats(buff.Stats));

            // Remove bloodlust, we have our own processing for it.
            if (relevant && buff.Name == "Heroism/Bloodlust")
                relevant = false;

            return relevant;
        }

        public override bool IsEnchantRelevant(Enchant enchant, Character character)
        {
            // First we let the normal rules (profession, class, relevant stats) decide
            bool relevant = base.IsEnchantRelevant(enchant, character);

            // Next we use our special stat relevancy filtering.
            if (relevant)
                relevant = HasPrimaryStats(enchant.Stats) || (HasSecondaryStats(enchant.Stats) && !HasUnwantedStats(enchant.Stats));

            return relevant;
        }

        public override Stats GetRelevantStats(Stats stats)
        {
            Stats s = new Stats()
            {
                Health = stats.Health,
                Mana = stats.Mana,
                Strength = stats.Strength,
                Agility = stats.Agility,
                Intellect = stats.Intellect,
                Stamina = stats.Stamina,
                AttackPower = stats.AttackPower,
                HitRating = stats.HitRating,
                CritRating = stats.CritRating,
                ArmorPenetration = stats.ArmorPenetration,
                ArmorPenetrationRating = stats.ArmorPenetrationRating,
                ExpertiseRating = stats.ExpertiseRating,
                HasteRating = stats.HasteRating,
                SpellCrit = stats.SpellCrit,
                SpellCritOnTarget = stats.SpellCritOnTarget,
                PhysicalCrit = stats.PhysicalCrit,
                PhysicalHaste = stats.PhysicalHaste,
                PhysicalHit = stats.PhysicalHit,
                SpellHit = stats.SpellHit,
                Expertise = stats.Expertise,
                Paragon = stats.Paragon,
                SpellPower = stats.SpellPower,
                BonusStrengthMultiplier = stats.BonusStrengthMultiplier,
                BonusStaminaMultiplier = stats.BonusStaminaMultiplier,
                BonusAgilityMultiplier = stats.BonusAgilityMultiplier,
                BonusIntellectMultiplier = stats.BonusIntellectMultiplier,
                BonusAttackPowerMultiplier = stats.BonusAttackPowerMultiplier,
                BonusCritMultiplier = stats.BonusCritMultiplier,
                BonusSpellCritMultiplier = stats.BonusSpellCritMultiplier,
                BonusPhysicalDamageMultiplier = stats.BonusPhysicalDamageMultiplier,
                BonusHolyDamageMultiplier = stats.BonusHolyDamageMultiplier,
                BonusDamageMultiplier = stats.BonusDamageMultiplier,
                DivineStormMultiplier = stats.DivineStormMultiplier,
                BonusSealOfCorruptionDamageMultiplier = stats.BonusSealOfCorruptionDamageMultiplier,
                BonusSealOfRighteousnessDamageMultiplier = stats.BonusSealOfRighteousnessDamageMultiplier,
                BonusSealOfVengeanceDamageMultiplier = stats.BonusSealOfVengeanceDamageMultiplier,
                CrusaderStrikeDamage = stats.CrusaderStrikeDamage,
                ConsecrationSpellPower = stats.ConsecrationSpellPower,
                JudgementCDReduction = stats.JudgementCDReduction,
                DivineStormDamage = stats.DivineStormDamage,
                DivineStormCrit = stats.DivineStormCrit,
                CrusaderStrikeCrit = stats.CrusaderStrikeCrit,
                ExorcismMultiplier = stats.ExorcismMultiplier,
                HammerOfWrathMultiplier = stats.HammerOfWrathMultiplier,
                CrusaderStrikeMultiplier = stats.CrusaderStrikeMultiplier,
                JudgementCrit = stats.JudgementCrit,
                RighteousVengeanceCanCrit = stats.RighteousVengeanceCanCrit,
                SealMultiplier = stats.SealMultiplier,
                JudgementMultiplier = stats.JudgementMultiplier,
                DivineStormRefresh = stats.DivineStormRefresh,
                DeathbringerProc = stats.DeathbringerProc,
                MoteOfAnger = stats.MoteOfAnger,
                FireDamage = stats.FireDamage,
                FrostDamage = stats.FrostDamage,
                ArcaneDamage = stats.ArcaneDamage,
                ShadowDamage = stats.ShadowDamage,
                NatureDamage = stats.NatureDamage,
                HolyDamage = stats.HolyDamage,
                MovementSpeed = stats.MovementSpeed,
            };
            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if (RelevantTriggers.Contains(effect.Trigger) && HasRelevantStats(effect.Stats))
                    s.AddSpecialEffect(effect);
            }
            return s;
        }

        public override bool HasRelevantStats(Stats stats)
        {
            // These 3 calls should amount to the same list of stats as used in GetRelevantStats()
            return HasPrimaryStats(stats) || HasSecondaryStats(stats) || HasExtraStats(stats);
        }

        /// <summary>
        /// HasPrimaryStats() should return true if the Stats object has any stats that define the item
        /// as being 'for your class/spec'. For melee classes this is typical melee stats like Strength, 
        /// Agility, AP, Expertise... For casters it would be spellpower, intellect, ...
        /// As soon as an item/enchant/buff has any of the stats listed here, it will be assumed to be 
        /// relevant unless explicitely filtered out.
        /// Stats that could be usefull for both casters and melee such as HitRating, CritRating and Haste
        /// don't belong here, but are SecondaryStats. Specific melee versions of these do belong here 
        /// for melee, spell versions would fit here for casters.
        /// </summary>
        public bool HasPrimaryStats(Stats stats)
        {
            bool PrimaryStats = // Base stats
                                stats.Strength > 0 ||
                                stats.Agility > 0 ||
                                stats.AttackPower > 0 ||
                                stats.ArmorPenetration > 0 ||
                                stats.Expertise > 0 ||//?
                                // Combat ratings
                                stats.ArmorPenetrationRating > 0 ||
                                stats.ExpertiseRating > 0 ||
                                stats.PhysicalHit > 0 ||
                                stats.PhysicalCrit > 0 ||
                                stats.PhysicalHaste > 0 ||
                                // Stat and damage multipliers
                                stats.BonusStrengthMultiplier > 0 ||
                                stats.BonusAgilityMultiplier > 0 ||
                                stats.BonusAttackPowerMultiplier > 0 ||
                                stats.BonusPhysicalDamageMultiplier > 0 ||
                                stats.BonusHolyDamageMultiplier > 0 ||
                                stats.BonusDamageMultiplier > 0 ||
                                // Paladin specific stats (set bonusses)
                                stats.DivineStormMultiplier > 0 ||
                                stats.BonusSealOfCorruptionDamageMultiplier > 0 ||
                                stats.BonusSealOfRighteousnessDamageMultiplier > 0 ||
                                stats.BonusSealOfVengeanceDamageMultiplier > 0 ||
                                stats.CrusaderStrikeDamage > 0 ||
                                stats.ConsecrationSpellPower > 0 ||
                                stats.JudgementCDReduction > 0 ||
                                stats.DivineStormDamage > 0 ||
                                stats.DivineStormCrit > 0 ||
                                stats.CrusaderStrikeCrit > 0 ||
                                stats.ExorcismMultiplier > 0 ||
                                stats.HammerOfWrathMultiplier > 0 ||
                                stats.CrusaderStrikeMultiplier > 0 ||
                                stats.JudgementCrit > 0 ||
                                stats.RighteousVengeanceCanCrit > 0 ||
                                stats.SealMultiplier > 0 ||
                                stats.JudgementMultiplier > 0 ||
                                stats.DivineStormRefresh > 0 ||
                                // Item proc effects
                                stats.Paragon > 0 ||            // Highest of Str or Agi. (Death's Verdict, TotC25/TotGC25)
                                stats.DeathbringerProc > 0 ||   // Chance to proc one of several subeffects. Paladins can get Str/Haste/Crit. (Deathbringer's Will, ICC25)
                                stats.MoteOfAnger > 0;           // Stacking buf, causes a weapon swing when full. (Tiny Abomination in a Jar, ICC25)

            if (!PrimaryStats)
            {
                foreach (SpecialEffect effect in stats.SpecialEffects())
                {
                    if (RelevantTriggers.Contains(effect.Trigger) && HasPrimaryStats(effect.Stats))
                    {
                        PrimaryStats = true;
                        break;
                    }
                }
            }

            return PrimaryStats;
        }

        /// <summary>
        /// HasSecondaryStats() should return true if the Stats object has any stats that are relevant for the 
        /// model but only to a smaller degree, so small that you wouldn't typically consider the item.
        /// Stats that are usefull to both melee and casters (HitRating, CritRating & Haste) fit in here also.
        /// An item/enchant/buff having these stats would be considered only if it doesn't have any of the 
        /// unwanted stats.  Group/Party buffs are slighly different, they would be considered regardless if 
        /// they have unwanted stats.
        /// Note that a stat may be listed here since it impacts the model, but may also be listed as an unwanted stat.
        /// </summary>
        public bool HasSecondaryStats(Stats stats)
        {
            bool SecondaryStats = // Caster stats
                                  stats.Intellect > 0 ||                // Intellect increases spellcrit, so it contributes to DPS.
                                  stats.SpellCrit > 0 ||                // Exorcism can crit
                                  stats.SpellCritOnTarget > 0 ||        // Exorcism
                                  stats.SpellHit > 0 ||                 // Exorcism & Consecration (1st tick)
                                  stats.SpellPower > 0 ||               // All holy damage effects benefit from spellpower
                                  stats.BonusIntellectMultiplier > 0 || // See intellect
                                  stats.BonusSpellCritMultiplier > 0 || // See spellcrit
                                  // Generic DPS stats, useful for casters and melee.
                                  stats.HitRating > 0 ||
                                  stats.CritRating > 0 ||
                                  stats.HasteRating > 0 ||
                                  stats.BonusCritMultiplier > 0 ||
                                  // Damage procs
                                  stats.FireDamage > 0 ||
                                  stats.FrostDamage > 0 ||
                                  stats.ArcaneDamage > 0 ||
                                  stats.ShadowDamage > 0 ||
                                  stats.NatureDamage > 0 ||
                                  stats.HolyDamage > 0 ||
                                  // Special (unmodelled)
                                  stats.MovementSpeed > 0;


            if (!SecondaryStats)
            {
                foreach (SpecialEffect effect in stats.SpecialEffects())
                {
                    if (RelevantTriggers.Contains(effect.Trigger) && HasSecondaryStats(effect.Stats))
                    {
                        SecondaryStats = true;
                        break;
                    }
                }
            }

            return SecondaryStats;
        }

        /// <summary>
        /// Return true if the Stats object has any stats that don't influence the model but that you do want 
        /// to display in tooltips and in calculated summary values.
        /// </summary>
        public bool HasExtraStats(Stats stats)
        {
            bool ExtraStats =   stats.Health > 0 ||
                                stats.Mana > 0 ||
                                stats.Stamina > 0 ||
                                stats.BonusStaminaMultiplier > 0;

            if (!ExtraStats)
            {
                foreach (SpecialEffect effect in stats.SpecialEffects())
                {
                    if (RelevantTriggers.Contains(effect.Trigger) && HasExtraStats(effect.Stats))
                    {
                        ExtraStats = true;
                        break;
                    }
                }
            }

            return ExtraStats;
        }

        /// <summary>
        /// Return true if the Stats object contains any stats that are making the item undesired.
        /// Any item having only Secondary stats would be removed if it also has one of these.
        /// </summary>
        public bool HasUnwantedStats(Stats stats)
        {
            /// List of stats that will filter out some buffs (Flasks, Elixirs & Scrolls), Enchants and Items.
            bool UnwantedStats = stats.SpellPower > 0 ||
                                 stats.Intellect > 0 ||
                                 stats.Spirit > 0 ||
                                 stats.Mp5 > 0 ||
                                 stats.DefenseRating > 0 ||
                                 stats.ParryRating > 0 ||
                                 stats.DodgeRating > 0 ||
                                 stats.BlockRating > 0 ||
                                 stats.BlockValue > 0;

            if (!UnwantedStats)
            {
                foreach (SpecialEffect effect in stats.SpecialEffects())
                {
                    if (/*RelevantTriggers.Contains(effect.Trigger) && */HasUnwantedStats(effect.Stats))    // An unwanted stat could be behind a trigger we don't model.
                    {
                        UnwantedStats = true;
                        break;
                    }
                }
            }

            return UnwantedStats;
        }

        #endregion

        #region Custom Charts
        // Custom charts are extra charts which can be defined per model.
        // The charts are available via the "Slot" menu of the righthand Rawr chart panel.

        private string[] _customChartNames = null;
        public override string[] CustomChartNames
        {
            get
            {
                if (_customChartNames == null)
                {
                    _customChartNames = new string[] 
                    { 
                        "Seals", 
                        "Weapon Speed", 
                        rotationsChartName, 
                        allRotationsChartName 
                    };
                }
                return _customChartNames;
            }
        }

        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
        {
            if (chartName == "Seals")
            {
                CalculationOptionsRetribution initOpts = character.CalculationOptions as CalculationOptionsRetribution;

                Character baseChar = character.Clone();
                CalculationOptionsRetribution baseOpts = initOpts.Clone();
                baseChar.CalculationOptions = baseOpts;
                baseOpts.Seal = SealOf.None;
                CharacterCalculationsBase baseCalc = Calculations.GetCharacterCalculations(baseChar);

                Character deltaChar = character.Clone();
                CalculationOptionsRetribution deltaOpts = initOpts.Clone();
                deltaChar.CalculationOptions = deltaOpts;

                ComparisonCalculationBase Command;
                deltaOpts.Seal = SealOf.Command;
                Command = Calculations.GetCharacterComparisonCalculations(baseCalc, deltaChar, "Seal of Command", initOpts.Seal == SealOf.Command);
                Command.Item = null;

                ComparisonCalculationBase Righteousness;
                deltaOpts.Seal = SealOf.Righteousness;
                Righteousness = Calculations.GetCharacterComparisonCalculations(baseCalc, deltaChar, "Seal of Righteousness", initOpts.Seal == SealOf.Righteousness);
                Righteousness.Item = null;

                ComparisonCalculationBase Vengeance;
                deltaOpts.Seal = SealOf.Vengeance;
                Vengeance = Calculations.GetCharacterComparisonCalculations(baseCalc, deltaChar, "Seal of Vengeance", initOpts.Seal == SealOf.Vengeance);
                Vengeance.Item = null;

                return new ComparisonCalculationBase[] { Command, Righteousness, Vengeance };
            }
            bool isRotationsChart = chartName == rotationsChartName;
            bool isAllRotationsChart = chartName == allRotationsChartName;
            if (isRotationsChart || isAllRotationsChart)
            {
                List<ComparisonCalculationBase> comparisons = new List<ComparisonCalculationBase>();
                Character baseChar = character.Clone();
                CalculationOptionsRetribution options = 
                    (CalculationOptionsRetribution)character.CalculationOptions;

                Ability[] selectedRotation = 
                    ((CharacterCalculationsRetribution)Calculations.GetCharacterCalculations(character))
                        .Rotation;

                foreach (Ability[] rotation in 
                    isAllRotationsChart ? Rotation.GetAllRotations() : options.Rotations)
                {
                    ComparisonCalculationBase compare = Calculations.GetCharacterComparisonCalculations(
                        GetCharacterCalculations(
                            baseChar, 
                            null, 
                            GetCharacterRotation(
                                baseChar, 
                                null,
                                rotation, 
                                isAllRotationsChart ? allRotationsSimulationTime : simulationTime)),
                        SimulatorParameters.RotationString(rotation), 
                        Utilities.AreArraysEqual(rotation, selectedRotation));
                    compare.Item = null;
                    comparisons.Add(compare);
                }

                return comparisons.ToArray();
            }
            if (chartName == "Weapon Speed")
            {
                if (character.MainHand == null)
                    return new ComparisonCalculationBase[] { };

                return new ComparisonCalculationBase[] 
                { 
                    GetWeaponSpeedComparison(character, 3.3f),
                    GetWeaponSpeedComparison(character, 3.4f),
                    GetWeaponSpeedComparison(character, 3.5f),
                    GetWeaponSpeedComparison(character, 3.6f),
                    GetWeaponSpeedComparison(character, 3.7f)
                };
            }
            else
            {
                return new ComparisonCalculationBase[0];
            }

        }


        private ComparisonCalculationBase GetWeaponSpeedComparison(Character character, float speed)
        {
            Character adjustedCharacter = character.Clone();
            adjustedCharacter.IsLoading = true;
            adjustedCharacter.MainHand = new ItemInstance(
                AdjustWeaponSpeed(character.MainHand.Item, speed), 
                character.MainHand.Gem1, 
                character.MainHand.Gem2, 
                character.MainHand.Gem3, 
                character.MainHand.Enchant);

            var comparison = Calculations.GetCharacterComparisonCalculations(
                Calculations.GetCharacterCalculations(character), 
                adjustedCharacter, 
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0:0.0} Speed", 
                    speed),
                character.MainHand.Item.Speed == speed);
            comparison.Item = null;

            return comparison;
        }

        private Item AdjustWeaponSpeed(Item weapon, float speed)
        {
            Item adjustedWeapon = weapon.Clone();
            adjustedWeapon.MinDamage = (int)Math.Round(weapon.MinDamage / weapon.Speed * speed);
            adjustedWeapon.MaxDamage = (int)Math.Round(weapon.MaxDamage / weapon.Speed * speed);
            adjustedWeapon.Speed = speed;

            return adjustedWeapon;
        }

        #endregion
    }
}
