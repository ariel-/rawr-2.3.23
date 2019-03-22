using System;
using System.Collections.Generic;
using System.Text;
#if RAWR3
using System.Windows.Media;
#else
using System.Drawing;
#endif
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;

namespace Rawr.Warlock {

    /// <summary>
    /// Core class representing the Rawr.Warlock model
    /// </summary>
    [Rawr.Calculations.RawrModelInfo(
        "Warlock",
        "Spell_Nature_FaerieFire",
        CharacterClass.Warlock)]
    public class CalculationsWarlock : CalculationsBase {

        #region Related Classes

        /// <summary>
        /// The class that Rawr.Warlock is designed for (Warlock)
        /// </summary>
        public override CharacterClass TargetClass {
            get { return CharacterClass.Warlock; }
        }

        private CalculationOptionsPanelWarlock _calculationOptionsPanel = null;
        /// <summary>
        /// Panel to be placed on the Options tab of the main form
        /// </summary>
#if RAWR3
        public override ICalculationOptionsPanel CalculationOptionsPanel {
#else
        public override CalculationOptionsPanelBase CalculationOptionsPanel {
#endif
            get {
                if (_calculationOptionsPanel == null) {
                    _calculationOptionsPanel
                        = new CalculationOptionsPanelWarlock();
                }
                return _calculationOptionsPanel;
            }
        }

        /// <summary>
        /// Creates a new ComparisonCalculationWarlock instance
        /// </summary>
        /// <returns>
        /// A new ComparisonCalculationWarlock instance
        /// </returns>
        public override ComparisonCalculationBase
            CreateNewComparisonCalculation() {

            return new ComparisonCalculationWarlock();
        }

        /// <summary>
        /// Creates a new CharacterCalculationsWarlock instance
        /// </summary>
        /// <returns>
        /// A new CharacterCalculationsWarlock instance
        /// </returns>
        public override CharacterCalculationsBase
            CreateNewCharacterCalculations() {

            return new CharacterCalculationsWarlock();
        }

        #endregion


        #region Basic Model Properties and Methods

        public const float AVG_UNHASTED_CAST_TIME = 2f; // total SWAG

        public override void SetDefaults(Character character) {

            character.ActiveBuffsAdd("Fel Armor");
            character.ActiveBuffsAdd("Grand Spellstone");
        }

        private string[] _characterDisplayCalculationLabels = null;
        /// <summary>
        /// Labels of the stats to display on the Stats tab of the main form
        /// </summary>
        public override string[] CharacterDisplayCalculationLabels {
            get {
                if (_characterDisplayCalculationLabels == null)
                    _characterDisplayCalculationLabels = new string[] {
                        "Simulation:Personal DPS",
                        "Simulation:Pet DPS",
                        "Simulation:Total DPS",
                        "Basic Stats:Health",
                        "Basic Stats:Mana",
                        "Basic Stats:Spirit",
                        "Basic Stats:Bonus Damage",
                        "Basic Stats:Hit Rating",
                        "Basic Stats:Crit Chance",
                        "Basic Stats:Average Haste",
                        "Pet Stats:Pet Stamina",
                        "Pet Stats:Pet Intellect",
                        "Pet Stats:Pet Health",
                        "Affliction:Corruption",
                        "Affliction:Curse Of Agony",
                        "Affliction:Curse Of Doom",
                        "Affliction:Curse Of The Elements",
                        "Affliction:Haunt",
                        "Affliction:Life Tap",
                        "Affliction:Unstable Affliction",
                        "Demonology:Immolation Aura",
                        "Destruction:Chaos Bolt",
                        "Destruction:Conflagrate",
                        "Destruction:Immolate",
                        "Destruction:Incinerate",
                        "Destruction:Incinerate (Under Backdraft)",
                        "Destruction:Incinerate (Under Molten Core)",
                        "Destruction:Soul Fire",
                        "Destruction:Shadow Bolt",
                        "Destruction:Shadow Bolt (Instant)" };
                return _characterDisplayCalculationLabels;
            }
        }

        private string[] _optimizableCalculationLabels = null;
        /// <summary>
        /// Labels of the additional constraints available to the Optimizer 
        /// </summary>
        public override string[] OptimizableCalculationLabels {
            get {
                if (_optimizableCalculationLabels == null)
                    _optimizableCalculationLabels = new string[] {
					    "Miss Chance"};
                return _optimizableCalculationLabels;
            }
        }

        private string[] _customChartNames = null;
        /// <summary>
        /// Names of the custom charts that Rawr.Warlock provides
        /// </summary>
        public override string[] CustomChartNames {
            get {
                if (_customChartNames == null)
                    _customChartNames = new string[] { };
                return _customChartNames;
            }
        }

        private Dictionary<string, Color>
            _subPointNameColors = null;
        /// <summary>
        /// Names and colors for the SubPoints that Rawr.Warlock uses
        /// </summary>
        public override Dictionary<string, Color>
            SubPointNameColors {
            get {
                if (_subPointNameColors == null) {
                    _subPointNameColors = new Dictionary<string, Color>();
                    _subPointNameColors.Add(
                        "DPS", Color.FromArgb(255, 255, 0, 0));
                    _subPointNameColors.Add(
                        "Pet DPS", Color.FromArgb(255, 0, 0, 255));
                    _subPointNameColors.Add(
                        "Raid Buff", Color.FromArgb(255, 0, 255, 0));
                }
                return _subPointNameColors;
            }
        }

        /// <summary>
        /// Deserializes the CalculationOptionsBear object contained 
        /// in xml
        /// </summary>
        /// <param name="xml">
        /// The CalculationOptionsBear object, serialized as xml
        /// </param>
        /// <returns>
        /// The deserialized CalculationOptionsBear object
        /// </returns>
        public override ICalculationOptionBase
            DeserializeDataObject(string xml) {

            XmlSerializer serializer = new XmlSerializer(
                typeof(CalculationOptionsWarlock));
            StringReader reader = new StringReader(xml);
            CalculationOptionsWarlock calcOpts
                = serializer.Deserialize(reader)
                    as CalculationOptionsWarlock;
            return calcOpts;
        }

        #endregion


        #region Primary Calculation Methods

        /// <summary>
        /// Gets the results of the Character provided
        /// </summary>
        /// <param name="character">
        /// The Character to calculate resutls for
        /// </param>
        /// <param name="additionalItem">
        /// An additional item to grant the Character the stats of (as if it
        /// were worn)
        /// </param>
        /// <returns>
        /// The CharacterCalculationsWarlock containing the results of the
        /// calculations
        /// </returns>
        public override CharacterCalculationsBase
            GetCharacterCalculations(
                Character character,
                Item additionalItem,
                bool referenceCalculation,
                bool significantChange,
                bool needsDisplayCalculations) {

            return new CharacterCalculationsWarlock(
                character,
                GetCharacterStats(character, additionalItem),
                GetPetBuffStats(character));
        }

        private Stats GetPetBuffStats(Character character) {

            List<Buff> buffs = new List<Buff>();
            foreach (Buff buff in character.ActiveBuffs) {
                string group = buff.Group;
                if (group != "Profession Buffs"
                    && group != "Set Bonuses"
                    && group != "Food"
                    && group != "Potion"
                    && group != "Elixirs and Flasks"
                    && group != "Focus Magic, Spell Critical Strike Chance") {

                    buffs.Add(buff);
                }
            }
            Stats stats = GetBuffsStats(buffs);
            ApplyPetsRaidBuff(
                stats,
                ((CalculationOptionsWarlock) character.CalculationOptions).Pet,
                character.WarlockTalents,
                character.ActiveBuffs);
            return stats;
        }

        /// <summary>Gets the total Stats of the Character</summary>
        /// <param name="character">
        /// The Character to get the total Stats of
        /// </param>
        /// <param name="additionalItem">
        /// An additional item to grant the Character the stats of (as if it
        /// were worn)
        /// </param>
        /// <returns>The total stats for the Character</returns>
        public override Stats GetCharacterStats(
            Character character, Item additionalItem) {

            WarlockTalents talents = character.WarlockTalents;
            CalculationOptionsWarlock options
                = character.CalculationOptions as CalculationOptionsWarlock;
            Stats stats = BaseStats.GetBaseStats(character);

            // Items
            AccumulateItemStats(stats, character, additionalItem);

            // Buffs
            AccumulateBuffsStats(stats, character.ActiveBuffs);
            if (options.Imbue.Equals("Grand Spellstone")) {
                stats.HasteRating += 60f * (1f + talents.MasterConjuror * 1.5f);
            } else {
                Debug.Assert(options.Imbue.Equals("Grand Firestone"));
                stats.CritRating += 49f * (1f + talents.MasterConjuror * 1.5f);
            }
            ApplyPetsRaidBuff(
                stats, options.Pet, talents, character.ActiveBuffs);
            float aegis = 1f + talents.DemonicAegis * 0.10f;
            stats.SpellPower += 180f * aegis; // fel armor
            stats.SpellDamageFromSpiritPercentage += .3f * aegis; // fel armor

            // Talents
            float[] talentValues = { 0f, .04f, .07f, .1f };
            Stats statsTalents = new Stats {

                //Demonic Embrace: increases your stamina by 4/7/10%
                BonusStaminaMultiplier = talentValues[talents.DemonicEmbrace],

                //Fel Vitality: increases your maximum Health & Mana by 1/2/3%
                BonusHealthMultiplier = talents.FelVitality * 0.01f,
                BonusManaMultiplier = talents.FelVitality * 0.01f,

                //Suppression: increases your chance to hit with spells by
                //1/2/3%
                SpellHit = (talents.Suppression * 0.01f),

                //Demonic Tactics: increases your spell crit chance by
                //2/4/6/8/10%
                //Backlash: increases your spell crit chance by 1/2/3%
                BonusCritChance
                    = talents.DemonicTactics * 0.02f + talents.Backlash * 0.01f
            };
            if (talents.Eradication > 0) {
                talentValues = new float[] { 0f, .06f, .12f, .20f };
                statsTalents.AddSpecialEffect(
                    new SpecialEffect(
                        Trigger.CorruptionTick,
                        new Stats() {
                            SpellHaste = talentValues[talents.Eradication]
                        },
                        6f,
                        0f,
                        .06f));
            }
            stats.Accumulate(statsTalents);
            stats.ManaRestoreFromMaxManaPerSecond
                = Math.Max(
                    stats.ManaRestoreFromMaxManaPerSecond,
                    .002f
                        * Spell.CalcUprate(
                            talents.ImprovedSoulLeech * .5f,
                            15f,
                            options.Duration * 1.1f));

            return stats;
        }

        private void ApplyPetsRaidBuff(
            Stats stats,
            string pet,
            WarlockTalents talents,
            List<Buff> activeBuffs) {

            stats.Health += CalcPetHealthBuff(pet, talents, activeBuffs);
            stats.Intellect += CalcPetIntBuff(pet, talents, activeBuffs);
            stats.Spirit += CalcPetSpiBuff(pet, talents, activeBuffs);
        }

        public static float CalcPetHealthBuff(
            string pet, WarlockTalents talents, List<Buff> activeBuffs) {

            if (!pet.Equals("Imp")) {
                return 0f;
            }

            return StatUtils.GetBuffEffect(
                activeBuffs,
                1330f * (1 + talents.ImprovedImp * .1f),
                "Health",
                s => s.Health);
        }

        public static float CalcPetIntBuff(
            string pet, WarlockTalents talents, List<Buff> activeBuffs) {

            if (!pet.Equals("Felhunter")) {
                return 0f;
            }

            return StatUtils.GetBuffEffect(
                activeBuffs,
                48f * (1 + talents.ImprovedFelhunter * .05f),
                "Intellect",
                s => s.Intellect);
        }

        public static float CalcPetSpiBuff(
            string pet, WarlockTalents talents, List<Buff> activeBuffs) {

            if (!pet.Equals("Felhunter")) {
                return 0f;
            }

            return StatUtils.GetBuffEffect(
                activeBuffs,
                64f * (1 + talents.ImprovedFelhunter * .05f),
                "Spirit",
                s => s.Spirit);
        }

        /// <summary>
        /// Gets data for a custom chart that Rawr.Warlock provides
        /// </summary>
        /// <param name="character">
        /// The Character to get the chart data for
        /// </param>
        /// <param name="chartName">
        /// The name of the custom chart to get data for
        /// </param>
        /// <returns>The data for the custom chart</returns>
        public override ComparisonCalculationBase[]
            GetCustomChartData(Character character, string chartName) {

            return new ComparisonCalculationBase[0];
        }

        #endregion


        #region Relevancy Methods

        private List<ItemType> _relevantItemTypes = null;
        /// <summary>
        /// ItemTypes that are relevant to Rawr.Warlock
        /// </summary>
        public override List<ItemType> RelevantItemTypes {
            get {
                if (_relevantItemTypes == null) {
                    _relevantItemTypes = new List<ItemType>(
                        new ItemType[] {
                            ItemType.None,
                            ItemType.Cloth,
                            ItemType.Dagger,
                            ItemType.Wand,
                            ItemType.OneHandSword,
                            ItemType.Staff });
                }
                return _relevantItemTypes;
            }
        }

        /// <summary>
        /// GemmingTemplates to be used by default, when none are defined
        /// </summary>
        public override List<GemmingTemplate> DefaultGemmingTemplates {
            get {
                //Relevant Gem IDs for Warlocks
                //Red
                int[] runed = { 39911, 39998, 40113, 42144 };

                //Purple
                int[] purified = { 39941, 40026, 40133 };

                //Orange
                int[] reckless = { 39959, 40051, 40155 };
                int[] veiled = { 39957, 40049, 40153 };

                //Meta
                const int ember = 41333;
                const int chaotic = 41285;

                return new List<GemmingTemplate>
                {
                    #region uncommon
                    new GemmingTemplate
                    {
                        Model = "Warlock", Group = "Uncommon", //Max SP - Ember
                        RedId = runed[0], YellowId = runed[0], BlueId = runed[0], PrismaticId = runed[0], MetaId = ember
                    },
                    new GemmingTemplate
                    {
                        Model = "Warlock", Group = "Uncommon", //SP/Hit - Ember
                        RedId = runed[0], YellowId = veiled[0], BlueId = purified[0], PrismaticId = runed[0], MetaId = ember
                    },
                    new GemmingTemplate
                    {
                        Model = "Warlock", Group = "Uncommon", //SP/Haste - Ember
                        RedId = runed[0], YellowId = reckless[0], BlueId = purified[0], PrismaticId = runed[0], MetaId = ember
                    },
                    new GemmingTemplate
                    {
                        Model = "Warlock", Group = "Uncommon", //Max SP - Chaotic
                        RedId = runed[0], YellowId = runed[0], BlueId = runed[0], PrismaticId = runed[0], MetaId = chaotic
                    },
                    new GemmingTemplate
                    {
                        Model = "Warlock", Group = "Uncommon", //SP/Hit - Chaotic
                        RedId = runed[0], YellowId = veiled[0], BlueId = purified[0], PrismaticId = runed[0], MetaId = chaotic
                    },
                    new GemmingTemplate
                    {
                        Model = "Warlock", Group = "Uncommon", //SP/Haste - Chaotic
                        RedId = runed[0], YellowId = reckless[0], BlueId = purified[0], PrismaticId = runed[0], MetaId = chaotic
                    },
                    #endregion

                    #region rare
				    new GemmingTemplate
				    {
                        Model = "Warlock", Group = "Rare", //Max SP - Ember
				        RedId = runed[1], YellowId = runed[1], BlueId = runed[1], PrismaticId = runed[1], MetaId = ember
                    },
				    new GemmingTemplate
				    {
                        Model = "Warlock", Group = "Rare", //SP/Hit - Ember
				        RedId = runed[1], YellowId = veiled[1], BlueId = purified[1], PrismaticId = runed[1], MetaId = ember
                    },
				    new GemmingTemplate
				    {
                        Model = "Warlock", Group = "Rare", //SP/Haste - Ember
				        RedId = runed[1], YellowId = reckless[1], BlueId = purified[1], PrismaticId = runed[1], MetaId = ember
                    },
				    new GemmingTemplate
				    {
                        Model = "Warlock", Group = "Rare", //Max SP - Chaotic
				        RedId = runed[1], YellowId = runed[1], BlueId = runed[1], PrismaticId = runed[1], MetaId = chaotic
                    },
				    new GemmingTemplate
				    {
                        Model = "Warlock", Group = "Rare", //SP/Hit - Chaotic
				        RedId = runed[1], YellowId = veiled[1], BlueId = purified[1], PrismaticId = runed[1], MetaId = chaotic
                    },
				    new GemmingTemplate
				    {
                        Model = "Warlock", Group = "Rare", //SP/Haste - Chaotic
				        RedId = runed[1], YellowId = reckless[1], BlueId = purified[1], PrismaticId = runed[1], MetaId = chaotic
                    },
                    #endregion

                    #region epic
                    new GemmingTemplate
				    {
                        Model = "Warlock", Group = "Epic", Enabled = true, //Max SP - Ember
				        RedId = runed[2], YellowId = runed[2], BlueId = runed[2], PrismaticId = runed[2], MetaId = ember
                    },
				    new GemmingTemplate
				    {
                        Model = "Warlock", Group = "Epic", Enabled = true, //SP/Hit - Ember
				        RedId = runed[2], YellowId = veiled[2], BlueId = purified[2], PrismaticId = runed[2], MetaId = ember
                    },
				    new GemmingTemplate
				    {
                        Model = "Warlock", Group = "Epic", Enabled = true, //SP/Haste - Ember
				        RedId = runed[2], YellowId = reckless[2], BlueId = purified[2], PrismaticId = runed[2], MetaId = ember
                    },
				    new GemmingTemplate
				    {
                        Model = "Warlock", Group = "Epic", Enabled = true, //Max SP - Chaotic
				        RedId = runed[2], YellowId = runed[2], BlueId = runed[2], PrismaticId = runed[2], MetaId = chaotic
                    },
				    new GemmingTemplate
				    {
                        Model = "Warlock", Group = "Epic", Enabled = true, //SP/Hit - Chaotic
				        RedId = runed[2], YellowId = veiled[2], BlueId = purified[2], PrismaticId = runed[2], MetaId = chaotic
                    },
				    new GemmingTemplate
				    {
                        Model = "Warlock", Group = "Epic", Enabled = true, //SP/Haste - Chaotic
				        RedId = runed[2], YellowId = reckless[2], BlueId = purified[2], PrismaticId = runed[2], MetaId = chaotic
                    },
                    #endregion

                    #region jeweler
				    new GemmingTemplate
				    {
                        Model = "Warlock", Group = "Jeweler", //Max SP - Ember
				        RedId = runed[3], YellowId = runed[3], BlueId = runed[3], PrismaticId = runed[3], MetaId = ember
                    },
				    new GemmingTemplate
				    {
                        Model = "Warlock", Group = "Jeweler", //SP/Hit - Ember
				        RedId = runed[2], YellowId = runed[3], BlueId = runed[3], PrismaticId = runed[2], MetaId = ember
                    },
				    new GemmingTemplate
				    {
                        Model = "Warlock", Group = "Jeweler", //Max SP - Chaotic
				        RedId = runed[3], YellowId = runed[3], BlueId = runed[3], PrismaticId = runed[3], MetaId = chaotic
                    },
				    new GemmingTemplate
				    {
                        Model = "Warlock", Group = "Jeweler", //SP/Hit - Chaotic
				        RedId = runed[2], YellowId = runed[3], BlueId = runed[3], PrismaticId = runed[2], MetaId = chaotic
                    },
                    #endregion
                };
            }
        }

        public override Stats GetRelevantStats(Stats stats) {
            Stats s = new Stats {

                //primary stats
                SpellPower = stats.SpellPower,
                Intellect = stats.Intellect,
                Spirit = stats.Spirit,
                HitRating = stats.HitRating,
                SpellHit = stats.SpellHit,
                HasteRating = stats.HasteRating,
                SpellHaste = stats.SpellHaste,
                CritRating = stats.CritRating,
                SpellCrit = stats.SpellCrit,
                SpellCritOnTarget = stats.SpellCritOnTarget,

                ShadowDamage = stats.ShadowDamage,
                SpellShadowDamageRating = stats.SpellShadowDamageRating,
                FireDamage = stats.FireDamage,
                SpellFireDamageRating = stats.SpellFireDamageRating,

                BonusIntellectMultiplier = stats.BonusIntellectMultiplier,
                BonusSpiritMultiplier = stats.BonusSpiritMultiplier,
                BonusSpellCritMultiplier = stats.BonusSpellCritMultiplier,
                BonusDamageMultiplier = stats.BonusDamageMultiplier,
                BonusShadowDamageMultiplier = stats.BonusShadowDamageMultiplier,
                BonusFireDamageMultiplier = stats.BonusFireDamageMultiplier,
                SpellDamageFromSpiritPercentage = stats.SpellDamageFromSpiritPercentage,

                //set bonuses
                Warlock2T7 = stats.Warlock2T7,
                Warlock4T7 = stats.Warlock4T7,
                Warlock2T8 = stats.Warlock2T8,
                Warlock4T8 = stats.Warlock4T8,
                Warlock2T9 = stats.Warlock2T9,
                Warlock4T9 = stats.Warlock4T9,
                Warlock2T10 = stats.Warlock2T10,
                Warlock4T10 = stats.Warlock4T10,

                //These stats can be used by warlocks, but they dont affect our dps calculations at all.
                //Included for display purposes only.
                Stamina = stats.Stamina,
                Health = stats.Health,
                Mana = stats.Mana,
                Mp5 = stats.Mp5,

                //The following are custom stat properties belonging to buffs, items (or procs) that can be used/applied to warlocks.
                HighestStat = stats.HighestStat,                                    //trinket - darkmoon card: greatness
                ManaRestoreFromBaseManaPPM = stats.ManaRestoreFromBaseManaPPM,      //paladin buff: judgement of wisdom
                ManaRestoreFromMaxManaPerSecond = stats.ManaRestoreFromMaxManaPerSecond,    //replenishment
                BonusManaPotion = stats.BonusManaPotion,                            //triggered when a mana pot is consumed
                ThreatReductionMultiplier = stats.ThreatReductionMultiplier,        //Bracing Eathsiege Diamond (metagem) effect
                ManaRestore = stats.ManaRestore,                                    //quite a few items that restore mana on spell cast or crit. Also used to model replenishment.
                // SpellsManaReduction = stats.SpellsManaReduction,                    //spark of hope -> http://www.wowhead.com/?item=45703
            };

            foreach (SpecialEffect effect in stats.SpecialEffects()) {
                if (RelevantTrinket(effect)) {
                    s.AddSpecialEffect(effect);
                }
            }
            return s;
        }

        public override bool IsBuffRelevant(Buff buff, Character character) {

            if (!buff.AllowedClasses.Contains(CharacterClass.Warlock)
                || buff.Group.Equals("Spell Sensitivity")) {

                return false;
            }
            if (character != null
                && Rawr.Properties.GeneralSettings.Default.HideProfEnchants
                && !character.HasProfession(buff.Professions)) {

                return false;
            }
            Stats stats = buff.GetTotalStats();
            return HasRelevantStats(stats)
                || stats.Strength > 0
                || stats.Agility > 0
                || stats.AttackPower > 0
                || stats.BonusAttackPowerMultiplier > 0
                || stats.PhysicalCrit > 0
                || stats.PhysicalHaste > 0
                || stats.ArmorPenetration > 0
                || stats.BonusPhysicalDamageMultiplier > 0;
        }

        protected bool RelevantTrinket(SpecialEffect effect) {

            if (effect.Trigger == Trigger.Use ||
                effect.Trigger == Trigger.DamageSpellCast ||
                effect.Trigger == Trigger.DamageSpellCrit ||
                effect.Trigger == Trigger.DamageSpellHit ||
                effect.Trigger == Trigger.SpellCast ||
                effect.Trigger == Trigger.SpellCrit ||
                effect.Trigger == Trigger.SpellHit ||
                effect.Trigger == Trigger.SpellMiss ||
                effect.Trigger == Trigger.DoTTick ||
                effect.Trigger == Trigger.DamageDone ||
                effect.Trigger == Trigger.DamageOrHealingDone) {
                return _HasRelevantStats(effect.Stats);
            }
            return false;
        }

        public override bool HasRelevantStats(Stats stats) {

            bool isRelevant = _HasRelevantStats(stats);
            foreach (SpecialEffect se in stats.SpecialEffects()) {
                isRelevant |= RelevantTrinket(se);
            }
            return isRelevant;
        }

        protected bool _HasRelevantStats(Stats stats) {
            bool yes = (

                //our primary stats
                stats.SpellPower
                + stats.Intellect
                + stats.Spirit
                + stats.HitRating + stats.SpellHit
                + stats.HasteRating + stats.SpellHaste
                + stats.CritRating + stats.SpellCrit + stats.SpellCritOnTarget
                + stats.ShadowDamage + stats.SpellShadowDamageRating
                + stats.FireDamage + stats.SpellFireDamageRating

                //multipliers
                + stats.BonusIntellectMultiplier
                + stats.BonusSpiritMultiplier + stats.SpellDamageFromSpiritPercentage
                + stats.BonusSpellCritMultiplier
                + stats.BonusDamageMultiplier + stats.BonusShadowDamageMultiplier + stats.BonusFireDamageMultiplier

                //set bonuses
                + stats.Warlock2T7
                + stats.Warlock4T7
                + stats.Warlock2T8
                + stats.Warlock4T8
                + stats.Warlock2T9
                + stats.Warlock4T9
                + stats.Warlock2T10
                + stats.Warlock4T10
            ) > 0;

            bool maybe = (
                stats.Stamina + stats.Health
                + stats.Mana + stats.Mp5

                //miscellaneous stats belonging to items (or trinket procs) that can be used/applied to warlocks
                //these stats are listed here so that those items (which supply them) can be listed
                //(these are terrible trinkets anyway - I should probably just remove them...)
                + stats.HighestStat                     //darkmoon card: greatness
                + stats.SpellsManaReduction             //spark of hope -> http://www.wowhead.com/?item=45703

                + stats.BonusManaPotion                 //triggered when a mana pot is consumed
                + stats.ManaRestoreFromBaseManaPPM      //judgement of wisdom
                + stats.ManaRestoreFromMaxManaPerSecond //replenishment sources
                + stats.ManaRestore                     //quite a few items that restore mana on spell cast or crit. Also used to model replenishment.
            ) > 0;

            bool no = (
                //ignore items with any of these stats
                + stats.Resilience
                + stats.Armor + stats.BonusArmor + stats.Agility
                + stats.ArmorPenetration + stats.ArmorPenetrationRating
                + stats.Strength + stats.AttackPower
                + stats.Expertise + stats.ExpertiseRating
                + stats.Dodge + stats.DodgeRating
                + stats.Parry + stats.ParryRating
                + stats.Defense + stats.DefenseRating
                + stats.ThreatReductionMultiplier       //bracing earthsiege diamond (metagem) effect
            ) > 0;

            return yes || (maybe && !no);
        }

        public override bool EnchantFitsInSlot(Enchant enchant, Character character, ItemSlot slot) {
            if (slot == ItemSlot.OffHand || slot == ItemSlot.Ranged) { return false; }
            return base.EnchantFitsInSlot(enchant, character, slot);
        }

        public override bool ItemFitsInSlot(Item item, Character character, CharacterSlot slot, bool ignoreUnique) {
            if (slot == CharacterSlot.OffHand && item.Slot == ItemSlot.OneHand)
                return false;
            return base.ItemFitsInSlot(item, character, slot, ignoreUnique);
        }

        private static List<string> _relevantGlyphs;
        public override List<string> GetRelevantGlyphs() {
            if (_relevantGlyphs == null) {
                _relevantGlyphs
                    = new List<string>{
                        "Glyph of Metamorphosis",
                        "Glyph of Quick Decay",
                        "Glyph of Corruption",
                        "Glyph of Life Tap",
                        "Glyph of Curse of Agony",
                        "Glyph of Unstable Affliction",
                        "Glyph of Haunt",
                        "Glyph of Chaos Bolt",
                        "Glyph of Immolate",
                        "Glyph of Incinerate",
                        "Glyph of Conflagrate",
                        "Glyph of Imp",
                        "Glyph of Felguard" };
            }
            return _relevantGlyphs;
        }

        #endregion
    }
}
//3456789 223456789 323456789 423456789 523456789 623456789 723456789 8234567890
