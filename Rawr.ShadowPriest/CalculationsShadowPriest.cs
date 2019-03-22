using System;
using System.Collections.Generic;
#if RAWR3
using System.Windows.Media;
#else
using System.Drawing;
#endif
using Rawr;

namespace Rawr.ShadowPriest
{
    [Rawr.Calculations.RawrModelInfo("ShadowPriest", "Spell_Shadow_Shadowform", CharacterClass.Priest)]
    public class CalculationsShadowPriest : CalculationsBase 
    {
        public override List<GemmingTemplate> DefaultGemmingTemplates
        {
            get
            {
                // Interesting Gem Choices for a Shadow Priest
                // Red
                int[] runed = { 39911, 39998, 40113, 42144 }; // +spp

                // Orange
                int[] durable = { 39958, 40050, 40154 }; // +spp/+resilience
                int[] luminous = { 39946, 40047, 40151 }; // +spp/+int
                int[] potent = { 39956, 40048, 40152 }; // +spp/+crit
                int[] reckless = { 39959, 40051, 40155 }; // +spp/+haste
                int[] veiled = { 39957, 40049, 40153 }; // +spp/+hit

                // Yellow
                int[] brilliant = { 39912, 40012, 40123, 42148 }; // +int
                int[] mystic = { 39917, 40016, 40127, 42158 }; // +resilience
                int[] quick = { 39918, 40017, 40128, 42150 }; // +haste
                int[] rigid = { 39915, 40014, 40125, 42156 }; // +hit
                int[] smooth = { 39914, 40013, 40124, 42149 }; // +crit

                // Green
                int[] dazzling = { 39984, 40094, 40175 }; // +int/+mp5
                int[] energized = { 39989, 40105, 40179 }; // +haste/+mp5
                int[] forceful = { 39978, 40091, 40169 }; // +haste/+sta
                int[] intricate = { 39983, 40104, 40174 }; // +haste/+spi
                int[] jagged = { 39974, 40086, 40165 }; // +crit/+stamina
                int[] lambent = { 39986, 40100, 40177 }; // +hit/+mp5
                int[] misty = { 39980, 40095, 40171 }; // +crit/+spi
                int[] opaque = { 39988, 40103, 40178 }; // +resilience/+mp5
                int[] seers = { 39979, 40092, 40170 }; // +int/+spi 
                int[] shining = { 39981, 40099, 40172 }; // +hit/+spi
                int[] steady = { 39977, 40090, 40168 }; // +resilience/+stamina
                int[] sundered = { 39985, 40096, 40176 }; // +crit/+mp5
                int[] timeless = { 39968, 40085, 40164 }; // +int/+stamina
                int[] turbid = { 39982, 40102, 40173 }; // +resilience/+spi
                int[] vivid = { 39975, 40088, 40166 }; // +hit/+stamina

                // Blue
                int[] lustrous = { 39927, 40010, 40121, 42146 }; // +mp5
                int[] solid = { 39919, 40008, 40119, 36767 }; // +stamina
                int[] sparkling = { 39920, 40009, 40120, 42145 }; // +spirit

                // Purple
                int[] glowing = { 39936, 40025, 40132 }; // +spp/+stamina
                int[] purified = { 39941, 40026, 40133 }; // +spp/+spirit
                int[] royal = { 39943, 40027, 40134 }; // +spp/+mp5

                // Meta
                int[] chaotic = { 41285 }; // +spp/+3% crit damage
                int[] insightful = { 41401 }; // +int/manarestore
                int[] powerful = { 41397 }; // +stamina/-10% stun duration

                return new List<GemmingTemplate>() {
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Uncommon", // Forced Hit
                        RedId = rigid[0], YellowId = rigid[0], BlueId = rigid[0], PrismaticId = rigid[0], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Uncommon", // Balanced w/Hit
                        RedId = veiled[0], YellowId = rigid[0], BlueId = shining[0], PrismaticId = rigid[0], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Uncommon", // Power w/Hit
                        RedId = runed[0], YellowId = veiled[0], BlueId = purified[0], PrismaticId = runed[0], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Uncommon", // Full Power
                        RedId = runed[0], YellowId = runed[0], BlueId = runed[0], PrismaticId = runed[0], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Uncommon", // Power w/Haste
                        RedId = runed[0], YellowId = reckless[0], BlueId = purified[0], PrismaticId = runed[0], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Uncommon", // Power w/Crit
                        RedId = runed[0], YellowId = potent[0], BlueId = purified[0], PrismaticId = runed[0], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Uncommon", // Mana
                        RedId = luminous[0], YellowId = brilliant[0], BlueId = seers[0], PrismaticId = brilliant[0], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Uncommon", // PvP - Stamina
                        RedId = glowing[0], YellowId = steady[0], BlueId = solid[0], PrismaticId = solid[0], MetaId = powerful[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Uncommon", // PvP - Resilience
                        RedId = durable[0], YellowId = mystic[0], BlueId = steady[0], PrismaticId = mystic[0], MetaId = powerful[0] },

                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Rare", // Forced Hit
                        RedId = rigid[1], YellowId = rigid[1], BlueId = rigid[1], PrismaticId = rigid[1], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Rare", // Balanced w/Hit
                        RedId = veiled[1], YellowId = rigid[1], BlueId = shining[1], PrismaticId = rigid[1], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Rare", // Power w/Hit
                        RedId = runed[1], YellowId = veiled[1], BlueId = purified[1], PrismaticId = runed[1], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Rare", // Full Power
                        RedId = runed[1], YellowId = runed[1], BlueId = runed[1], PrismaticId = runed[1], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Rare", // Power w/Haste
                        RedId = runed[1], YellowId = reckless[1], BlueId = purified[1], PrismaticId = runed[1], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Rare", // Power w/Crit
                        RedId = runed[1], YellowId = potent[1], BlueId = purified[1], PrismaticId = runed[1], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Rare", // Mana
                        RedId = luminous[1], YellowId = brilliant[1], BlueId = seers[1], PrismaticId = brilliant[1], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Rare", // PvP - Stamina
                        RedId = glowing[1], YellowId = steady[1], BlueId = solid[1], PrismaticId = solid[1], MetaId = powerful[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Rare", // PvP - Resilience
                        RedId = durable[1], YellowId = mystic[1], BlueId = steady[1], PrismaticId = mystic[1], MetaId = powerful[0] },


                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Epic", // Forced Hit
                        RedId = rigid[2], YellowId = rigid[2], BlueId = rigid[2], PrismaticId = rigid[2], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Epic", Enabled = true, // Balanced w/Hit
                        RedId = veiled[2], YellowId = rigid[2], BlueId = shining[2], PrismaticId = rigid[2], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Epic", Enabled = true, // Power w/Hit
                        RedId = runed[2], YellowId = veiled[2], BlueId = purified[2], PrismaticId = runed[2], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Epic", Enabled = true, // Full Power
                        RedId = runed[2], YellowId = runed[2], BlueId = runed[2], PrismaticId = runed[2], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Epic", Enabled = true, // Power w/Haste
                        RedId = runed[2], YellowId = reckless[2], BlueId = purified[2], PrismaticId = runed[2], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Epic", Enabled = true, // Power w/Crit
                        RedId = runed[2], YellowId = potent[2], BlueId = purified[2], PrismaticId = runed[2], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Epic", // Mana
                        RedId = luminous[2], YellowId = brilliant[2], BlueId = seers[2], PrismaticId = brilliant[2], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Epic", // PvP - Stamina
                        RedId = glowing[2], YellowId = steady[2], BlueId = solid[2], PrismaticId = solid[2], MetaId = powerful[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Epic", // PvP - Resilience
                        RedId = durable[2], YellowId = mystic[2], BlueId = steady[2], PrismaticId = mystic[2], MetaId = powerful[0] },
                    

                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Rare", // Forced Hit
                        RedId = rigid[1], YellowId = rigid[3], BlueId = rigid[1], PrismaticId = rigid[1], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Rare", // Balanced w/Hit
                        RedId = veiled[1], YellowId = rigid[3], BlueId = shining[1], PrismaticId = rigid[1], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Rare", // Power w/Hit
                        RedId = runed[3], YellowId = veiled[1], BlueId = purified[1], PrismaticId = runed[1], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Rare", // Full Power
                        RedId = runed[3], YellowId = runed[3], BlueId = runed[3], PrismaticId = runed[3], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Rare", // Power w/Haste
                        RedId = runed[3], YellowId = reckless[1], BlueId = purified[1], PrismaticId = runed[1], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Rare", // Power w/Crit
                        RedId = runed[3], YellowId = potent[1], BlueId = purified[1], PrismaticId = runed[1], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Rare", // Mana
                        RedId = luminous[1], YellowId = brilliant[3], BlueId = seers[1], PrismaticId = brilliant[1], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Rare", // PvP - Stamina
                        RedId = glowing[1], YellowId = steady[1], BlueId = solid[3], PrismaticId = solid[1], MetaId = powerful[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Rare", // PvP - Resilience
                        RedId = durable[1], YellowId = mystic[3], BlueId = steady[1], PrismaticId = mystic[1], MetaId = powerful[0] },

                        
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Epic", // Forced Hit
                        RedId = rigid[2], YellowId = rigid[3], BlueId = rigid[2], PrismaticId = rigid[2], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Epic", // Balanced w/Hit
                        RedId = veiled[2], YellowId = rigid[3], BlueId = shining[2], PrismaticId = rigid[2], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Epic", // Power w/Hit
                        RedId = runed[3], YellowId = veiled[2], BlueId = purified[2], PrismaticId = runed[2], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Epic", // Full Power
                        RedId = runed[3], YellowId = runed[3], BlueId = runed[3], PrismaticId = runed[3], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Epic", // Power w/Haste
                        RedId = runed[3], YellowId = reckless[2], BlueId = purified[2], PrismaticId = runed[2], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Epic", // Power w/Crit
                        RedId = runed[3], YellowId = potent[2], BlueId = purified[2], PrismaticId = runed[2], MetaId = chaotic[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Epic", // Mana
                        RedId = luminous[2], YellowId = brilliant[3], BlueId = seers[2], PrismaticId = brilliant[2], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Epic", // PvP - Stamina
                        RedId = glowing[2], YellowId = steady[2], BlueId = solid[3], PrismaticId = solid[2], MetaId = powerful[0] },
                    new GemmingTemplate() { Model = "ShadowPriest", Group = "Jeweler Epic", // PvP - Resilience
                        RedId = durable[2], YellowId = mystic[3], BlueId = steady[2], PrismaticId = mystic[2], MetaId = powerful[0] },
                };
            }
        }

        public override void SetDefaults(Character character)
        {
            character.ActiveBuffsAdd(("Inner Fire"));
            character.ActiveBuffsAdd(("Improved Moonkin Form"));
            character.ActiveBuffsAdd(("Arcane Intellect"));
            character.ActiveBuffsAdd(("Vampiric Touch"));
            character.ActiveBuffsAdd(("Mana Spring Totem"));
                character.ActiveBuffsAdd(("Restorative Totems"));
            character.ActiveBuffsAdd(("Moonkin Form"));
            character.ActiveBuffsAdd(("Wrath of Air Totem"));
            character.ActiveBuffsAdd(("Totem of Wrath (Spell Power)"));
            character.ActiveBuffsAdd(("Divine Spirit"));
            character.ActiveBuffsAdd(("Power Word: Fortitude"));
                character.ActiveBuffsAdd(("Improved Power Word: Fortitude"));
            character.ActiveBuffsAdd(("Mark of the Wild"));
                character.ActiveBuffsAdd(("Improved Mark of the Wild"));
            character.ActiveBuffsAdd(("Blessing of Kings"));
            character.ActiveBuffsAdd(("Shadow Protection"));
            character.ActiveBuffsAdd(("Heart of the Crusader"));
            character.ActiveBuffsAdd(("Sanctified Retribution"));
            character.ActiveBuffsAdd(("Improved Scorch"));
            character.ActiveBuffsAdd(("Judgement of Wisdom"));
            character.ActiveBuffsAdd(("Earth and Moon"));
            character.ActiveBuffsAdd(("Misery"));
            character.ActiveBuffsAdd(("Crypt Fever"));
            character.ActiveBuffsAdd(("Flask of the Frost Wyrm"));
            character.ActiveBuffsAdd(("Spell Power Food"));
            character.ActiveBuffsAdd(("Heroism/Bloodlust"));
        }

        private static List<string> _relevantGlyphs;
        public override List<string> GetRelevantGlyphs()
        {
            if (_relevantGlyphs == null)
            {
                _relevantGlyphs = new List<string>();
                _relevantGlyphs.Add("Glyph of Dispel Magic");
                _relevantGlyphs.Add("Glyph of Dispersion");
                _relevantGlyphs.Add("Glyph of Inner Fire");
                _relevantGlyphs.Add("Glyph of Mass Dispel");
                _relevantGlyphs.Add("Glyph of Mind Flay");
                _relevantGlyphs.Add("Glyph of Mind Sear");
                _relevantGlyphs.Add("Glyph of Penance");
                _relevantGlyphs.Add("Glyph of Shadow");
                _relevantGlyphs.Add("Glyph of Shadow Word: Death");
                _relevantGlyphs.Add("Glyph of Shadow Word: Pain");
                _relevantGlyphs.Add("Glyph of Smite");
                _relevantGlyphs.Add("Glyph of Fading");
            }
            return _relevantGlyphs;
        }

        public override CharacterClass TargetClass { get { return CharacterClass.Priest; } }

        private string _currentChartName = null;
        private float _currentChartTotal = 0;

        private Dictionary<string, Color> _subPointNameColors = null;
        public override Dictionary<string, Color> SubPointNameColors
        {
            get
            {
                _subPointNameColors = new Dictionary<string, Color>();
                switch (_currentChartName)
                {
                    case "Mana Sources":
						_subPointNameColors.Add(string.Format("MP5 Sources ({0} Total)", _currentChartTotal.ToString("0")), Color.FromArgb(255, 0, 0, 255));
                        break;
                    case "DPS Sources":
						_subPointNameColors.Add(string.Format("DPS Sources ({0}% total)", _currentChartTotal.ToString("0")), Color.FromArgb(255, 255, 0, 0));
                        break;
                    case "Mana Usage":
						_subPointNameColors.Add(string.Format("Mana Usage ({0} total)", _currentChartTotal.ToString("0")), Color.FromArgb(255, 0, 0, 255));
                        break;
                    case "Haste Rating Gain":
						_subPointNameColors.Add(string.Format("DPS"), Color.FromArgb(255, 255, 0, 0));
//						_subPointNameColors.Add(string.Format("DPS-Sustained"), Color.FromArgb(255, 0, 0, 255));
                        break;
                    default:
						_subPointNameColors.Add("DPS", Color.FromArgb(255, 255, 0, 0));
//						_subPointNameColors.Add("DPS-Sustained", Color.FromArgb(255, 0, 0, 255));
						_subPointNameColors.Add("Survivability", Color.FromArgb(255, 0, 128, 0));
                        break;
                }
                _currentChartName = null;
                return _subPointNameColors;
            }
        }

        private string[] _characterDisplayCalculationLabels = null;
        public override string[] CharacterDisplayCalculationLabels
        {
            get
            {
                if (_characterDisplayCalculationLabels == null)
                    _characterDisplayCalculationLabels = new string[] {
					"Basic Stats:Health",
                    "Basic Stats:Resilience",
					"Basic Stats:Mana",
					"Basic Stats:Stamina",
					"Basic Stats:Intellect",
					"Basic Stats:Spirit",
					"Basic Stats:Spell Power",
					"Basic Stats:Regen",
					"Basic Stats:Crit",
					"Basic Stats:Hit",
					"Basic Stats:Haste",
                    "Basic Stats:Armor",
                    "Basic Stats:Resistance",
                    "Simulation:Rotation",
                    "Simulation:Castlist",
                    "Simulation:DPS",
//                    "Simulation:SustainDPS",
                    "Shadow:Vampiric Touch",
                    "Shadow:SW Pain",
                    "Shadow:Devouring Plague",
                    "Shadow:Imp. Devouring Plague",
				    "Shadow:SW Death",
                    "Shadow:Mind Blast",
                    "Shadow:Mind Flay",
                    "Shadow:Shadowfiend",
                    "Holy:PW Shield",
                    "Holy:Smite",
                    "Holy:Holy Fire",
                    "Holy:Penance"
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
					"Health",
                    "Resilience",
                    "Mana",
					"Haste Rating",
                    "Haste %",
                    "Crit Rating",
                    "MB Crit %",
                    "Hit Rating",
                    "MF cast time (ms)",
                    "Armor",
                    "Arcane Resistance",
                    "Fire Resistance",
                    "Frost Resistance",
                    "Nature Resistance",
                    "Shadow Resistance",
					};
                return _optimizableCalculationLabels;
            }
        }

#if RAWR3
        private ICalculationOptionsPanel _calculationOptionsPanel = null;
		public override ICalculationOptionsPanel CalculationOptionsPanel
#else
		private CalculationOptionsPanelBase _calculationOptionsPanel = null;
		public override CalculationOptionsPanelBase CalculationOptionsPanel
#endif
        {
            get {
                if (_calculationOptionsPanel == null)
                {
                    _calculationOptionsPanel = new CalculationOptionsPanelShadowPriest();
                }
                return _calculationOptionsPanel;
            }
        }

        private string[] _customChartNames = null;
        public override string[] CustomChartNames
        {
            get
            {
                if (_customChartNames == null)
                    _customChartNames = new string[] { "Mana Sources", "DPS Sources", "Mana Usage", "Haste Rating Gain" };
                return _customChartNames;
            }
        }

        public override ComparisonCalculationBase CreateNewComparisonCalculation() { return new ComparisonCalculationShadowPriest(); }
        public override CharacterCalculationsBase CreateNewCharacterCalculations() { return new CharacterCalculationsShadowPriest(); }

        private List<ItemType> _relevantItemTypes = null;
        public override List<ItemType> RelevantItemTypes
        {
            get {
                if (_relevantItemTypes == null) {
                    _relevantItemTypes = new List<ItemType>(new ItemType[]{
                        ItemType.None,
                        ItemType.Cloth,
                        ItemType.Dagger,
                        ItemType.Wand,
                        ItemType.OneHandMace,
                        ItemType.Staff
                    });
                }
                return _relevantItemTypes;
            }
        }

        public override bool EnchantFitsInSlot(Enchant enchant, Character character, ItemSlot slot)
        {
            if (slot == ItemSlot.OffHand || slot == ItemSlot.Ranged) return false;
            return base.EnchantFitsInSlot(enchant, character, slot);
        }

        public override bool ItemFitsInSlot(Item item, Character character, CharacterSlot slot, bool ignoreUnique)
        {
            if (slot == CharacterSlot.OffHand && item.Slot == ItemSlot.OneHand) return false;
            return base.ItemFitsInSlot(item, character, slot, ignoreUnique);
        }
        
        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
        {
            List<ComparisonCalculationBase> comparisonList = new List<ComparisonCalculationBase>();
            ComparisonCalculationBase comparison;

            _currentChartTotal = 0;
            _currentChartName = chartName;

            switch (chartName)
            {
                case "Mana Sources":
                    CharacterCalculationsShadowPriest mscalcs = GetCharacterCalculations(character) as CharacterCalculationsShadowPriest;
                    SolverBase mssolver = mscalcs.GetSolver(character, mscalcs.BasicStats);
                    mssolver.Calculate(mscalcs);
                    foreach (SolverBase.ManaSource Source in mssolver.ManaSources)
                    {
                        comparison = CreateNewComparisonCalculation();
                        comparison.Name = Source.Name;
                        comparison.SubPoints[0] = Source.Value * 5;
                        _currentChartTotal += comparison.SubPoints[0];
                        comparison.OverallPoints = comparison.SubPoints[0];
                        comparison.Equipped = false;
                        comparisonList.Add(comparison);
                    }
                    return comparisonList.ToArray();
                case "DPS Sources":
                    CharacterCalculationsShadowPriest dpscalcs = GetCharacterCalculations(character) as CharacterCalculationsShadowPriest;
                    SolverBase dpssolver = dpscalcs.GetSolver(character, dpscalcs.BasicStats);
                    dpssolver.Calculate(dpscalcs);
                    float dpstotal = 0;
                    foreach (Spell spell in dpssolver.SpellPriority)
                        dpstotal += spell.SpellStatistics.DamageDone;
                    foreach (Spell spell in dpssolver.SpellPriority)
                    {
                        comparison = CreateNewComparisonCalculation();
                        comparison.Name = spell.Name;
                        comparison.SubPoints[0] = spell.SpellStatistics.DamageDone * 100f / dpstotal;
                        comparison.OverallPoints = comparison.SubPoints[0];
                        comparison.Equipped = false;
                        comparisonList.Add(comparison);
                    }
                    _currentChartTotal = 100f;
                    return comparisonList.ToArray();
                case "Mana Usage":
                    CharacterCalculationsShadowPriest mucalcs = GetCharacterCalculations(character) as CharacterCalculationsShadowPriest;
                    SolverBase musolver = mucalcs.GetSolver(character, mucalcs.BasicStats);
                    musolver.Calculate(mucalcs);
                    foreach (Spell spell in musolver.SpellPriority)
                    {
                        comparison = CreateNewComparisonCalculation();
                        comparison.Name = spell.Name;
                        comparison.SubPoints[0] = spell.SpellStatistics.ManaUsed * 5;
                        _currentChartTotal += comparison.SubPoints[0];
                        comparison.OverallPoints = comparison.SubPoints[0];
                        comparison.Equipped = false;
                        comparisonList.Add(comparison);
                    }
                    return comparisonList.ToArray();
                case "Haste Rating Gain":
                    CharacterCalculationsShadowPriest hrbase = GetCharacterCalculations(character) as CharacterCalculationsShadowPriest;
                    for (int x = 0; x < 100; x++)
                    {
                        CharacterCalculationsShadowPriest hrnew = GetCharacterCalculations(character, new Item() { Stats = new Stats() { HasteRating = x } }) as CharacterCalculationsShadowPriest;
                        comparison = CreateNewComparisonCalculation();
                        comparison.Name = string.Format("{0} Haste Rating", x);
                        comparison.SubPoints[0] = hrnew.DpsPoints - hrbase.DpsPoints;
//                        comparison.SubPoints[1] = hrnew.SustainPoints - hrbase.SustainPoints;
                        comparison.Equipped = false;
                        comparisonList.Add(comparison);
                    }
                    return comparisonList.ToArray();
                default:
                    //_customChartNames = null;
                    _currentChartName = null;
                    return new ComparisonCalculationBase[0];
            }
        }

        public override CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, bool referenceCalculation, bool significantChange, bool needsDisplayCalculations)
        {
            Stats stats = GetCharacterStats(character, additionalItem);
            Stats statsRace = BaseStats.GetBaseStats(character);
            CharacterCalculationsShadowPriest calculatedStats = new CharacterCalculationsShadowPriest();
            CalculationOptionsShadowPriest calculationOptions = character.CalculationOptions as CalculationOptionsShadowPriest;
            
            calculatedStats.Race = character.Race;
            calculatedStats.BasicStats = stats;
            calculatedStats.Character = character;

            calculatedStats.SpiritRegen = (float)Math.Floor(5f * StatConversion.GetSpiritRegenSec(calculatedStats.BasicStats.Spirit, calculatedStats.BasicStats.Intellect));
            calculatedStats.RegenInFSR = calculatedStats.SpiritRegen * calculatedStats.BasicStats.SpellCombatManaRegeneration + calculatedStats.BasicStats.Mp5;
            calculatedStats.RegenOutFSR = calculatedStats.SpiritRegen + calculatedStats.BasicStats.Mp5;

            SolverBase solver = calculatedStats.GetSolver(character, stats);
            solver.Calculate(calculatedStats);

            return calculatedStats;
        }

        public override Stats GetCharacterStats(Character character, Item additionalItem)
        {
            CalculationOptionsShadowPriest calcOpts = character.CalculationOptions as CalculationOptionsShadowPriest;
            Stats statsRace = BaseStats.GetBaseStats(character);
            Stats statsBaseGear = GetItemStats(character, additionalItem);
            Stats statsBuffs = GetBuffsStats(character, calcOpts);
            float SpellDamageFromSpiritPercentage = character.PriestTalents.SpiritualGuidance * 0.05f + character.PriestTalents.TwistedFaith * 0.04f;

            Stats statsTotal = new Stats()
            {
                BonusStaminaMultiplier = character.PriestTalents.ImprovedPowerWordFortitude * 0.02f,
                BonusSpiritMultiplier = (1 + character.PriestTalents.Enlightenment * 0.02f) * (1f + character.PriestTalents.SpiritOfRedemption * 0.05f) - 1f,
                BonusIntellectMultiplier = character.PriestTalents.MentalStrength * 0.03f,
                SpellHaste = character.PriestTalents.Enlightenment * 0.02f,
                SpellCombatManaRegeneration = character.PriestTalents.Meditation * 0.5f / 3f,
                SpellCrit = character.PriestTalents.FocusedWill * 0.01f,
            };

            statsTotal.Accumulate(statsBaseGear);
            statsTotal.Accumulate(statsBuffs);
            statsTotal.Accumulate(statsRace);

            statsTotal.Stamina = (float)Math.Floor((statsTotal.Stamina) * (1 + statsTotal.BonusStaminaMultiplier));
            statsTotal.Intellect = (float)Math.Floor(statsTotal.Intellect * (1 + statsTotal.BonusIntellectMultiplier));
            statsTotal.Spirit = (float)Math.Floor((statsTotal.Spirit) * (1 + statsTotal.BonusSpiritMultiplier));
            statsTotal.SpellPower += SpellDamageFromSpiritPercentage * statsTotal.Spirit
                + (statsTotal.PriestInnerFire > 0 ? GetInnerFireSpellPowerBonus(character) : 0);
            statsTotal.Mana += StatConversion.GetManaFromIntellect(statsTotal.Intellect);
            statsTotal.Health += StatConversion.GetHealthFromStamina(statsTotal.Stamina);
            statsTotal.SpellCrit += StatConversion.GetSpellCritFromIntellect(statsTotal.Intellect)
                + StatConversion.GetSpellCritFromRating(statsTotal.CritRating) + statsTotal.SpellCritOnTarget;
            statsTotal.SpellHaste = (1f + statsTotal.SpellHaste) * (1f + StatConversion.GetSpellHasteFromRating(statsTotal.HasteRating)) - 1f;
            statsTotal.SpellHit += StatConversion.GetSpellHitFromRating(statsTotal.HitRating);
            statsTotal.BonusArmor += statsTotal.Agility * 2f + (statsTotal.PriestInnerFire > 0 ? GetInnerFireArmorBonus(character) : 0);

            return statsTotal;
        }

        public Stats GetBuffsStats(Character character, CalculationOptionsShadowPriest calcOpts) {
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
            }
            // Removes the Hunter's Mark Buff and it's Children 'Glyphed', 'Improved' and 'Both' if you are
            // maintaining it yourself. We are now calculating this internally for better accuracy and to provide
            // value to relevant talents
            {
                hasRelevantBuff =  character.HunterTalents.ImprovedHuntersMark
                                + (character.HunterTalents.GlyphOfHuntersMark ? 1 : 0);
                Buff a = Buff.GetBuffByName("Hunter's Mark");
                Buff b = Buff.GetBuffByName("Glyphed Hunter's Mark");
                Buff c = Buff.GetBuffByName("Improved Hunter's Mark");
                Buff d = Buff.GetBuffByName("Improved and Glyphed Hunter's Mark");
                // Since we are doing base Hunter's mark ourselves, we still don't want to double-dip
                if (character.ActiveBuffs.Contains(a)) { character.ActiveBuffs.Remove(a); /*removedBuffs.Add(a);*//* }
                // If we have an enhanced Hunter's Mark, kill the Buff
                if (hasRelevantBuff > 0) {
                    if (character.ActiveBuffs.Contains(b)) { character.ActiveBuffs.Remove(b); /*removedBuffs.Add(b);*//* }
                    if (character.ActiveBuffs.Contains(c)) { character.ActiveBuffs.Remove(c); /*removedBuffs.Add(c);*//* }
                    if (character.ActiveBuffs.Contains(d)) { character.ActiveBuffs.Remove(d); /*removedBuffs.Add(c);*//* }
                }
            }
            /* [More Buffs to Come to this method]
             * Ferocious Inspiration | Sanctified Retribution
             * Hunting Party | Judgements of the Wise, Vampiric Touch, Improved Soul Leech, Enduring Winter
             * Acid Spit | Expose Armor, Sunder Armor (requires BM & Worm Pet)
             */
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

        public static float GetInnerFireSpellPowerBonus(Character character)
        {
            float InnerFireSpellPowerBonus = 0;
            if (character.Level >= 77)
                InnerFireSpellPowerBonus = 120;
            else if (character.Level >= 71)
                InnerFireSpellPowerBonus = 95;
            return InnerFireSpellPowerBonus * (1f + character.PriestTalents.ImprovedInnerFire * 0.15f);
        }

        public static float GetInnerFireArmorBonus(Character character)
        {
            float ArmorBonus = 2440 * (character.PriestTalents.GlyphofInnerFire ? 1.5f : 1);

            return ArmorBonus * (1f + character.PriestTalents.ImprovedInnerFire * 0.15f);
        }

        public override Stats GetRelevantStats(Stats stats)
        {
            Stats s = new Stats()
            {
                Stamina = stats.Stamina,
                Health = stats.Health,
                Resilience = stats.Resilience,
                Intellect = stats.Intellect,
                Mana = stats.Mana,
                Spirit = stats.Spirit,
                Mp5 = stats.Mp5,
                SpellPower = stats.SpellPower,
                SpellShadowDamageRating = stats.SpellShadowDamageRating,
                CritRating = stats.CritRating,
                SpellCrit = stats.SpellCrit,
                SpellCritOnTarget = stats.SpellCritOnTarget,
                HitRating = stats.HitRating,
                SpellHit = stats.SpellHit,
                SpellHaste = stats.SpellHaste,
                HasteRating = stats.HasteRating,
                BonusSpiritMultiplier = stats.BonusSpiritMultiplier,
                BonusIntellectMultiplier = stats.BonusIntellectMultiplier,
                BonusManaPotion = stats.BonusManaPotion,
                ThreatReductionMultiplier = stats.ThreatReductionMultiplier,
                BonusDamageMultiplier = stats.BonusDamageMultiplier,
                BonusShadowDamageMultiplier = stats.BonusShadowDamageMultiplier,
                BonusHolyDamageMultiplier = stats.BonusHolyDamageMultiplier,
                BonusDiseaseDamageMultiplier = stats.BonusDiseaseDamageMultiplier,
                PriestInnerFire = stats.PriestInnerFire,
                MovementSpeed = stats.MovementSpeed,
                SWPDurationIncrease = stats.SWPDurationIncrease,
                BonusMindBlastMultiplier = stats.BonusMindBlastMultiplier,
                MindBlastCostReduction = stats.MindBlastCostReduction,
                ShadowWordDeathCritIncrease = stats.ShadowWordDeathCritIncrease,
                WeakenedSoulDurationDecrease = stats.WeakenedSoulDurationDecrease,
                DevouringPlagueBonusDamage = stats.DevouringPlagueBonusDamage,
                MindBlastHasteProc = stats.MindBlastHasteProc,
                PriestDPS_T9_2pc = stats.PriestDPS_T9_2pc,
                PriestDPS_T9_4pc = stats.PriestDPS_T9_4pc,
                PriestDPS_T10_2pc = stats.PriestDPS_T10_2pc,
                PriestDPS_T10_4pc = stats.PriestDPS_T10_4pc,
                BonusSpellCritMultiplier = stats.BonusSpellCritMultiplier,
                ManaRestore = stats.ManaRestore,
                SpellsManaReduction = stats.SpellsManaReduction,
                HighestStat = stats.HighestStat,
                ArcaneDamage = stats.ArcaneDamage,
                FireDamage = stats.FireDamage,
                FrostDamage = stats.FrostDamage,
                HolyDamage = stats.HolyDamage,
                NatureDamage = stats.NatureDamage,
                ShadowDamage = stats.ShadowDamage,
                ValkyrDamage = stats.ValkyrDamage,

                /*ManaRestoreFromBaseManaPerHit = stats.ManaRestoreFromBaseManaPerHit,
                SpellPowerFor15SecOnUse90Sec = stats.SpellPowerFor15SecOnUse90Sec,
                SpellPowerFor15SecOnUse2Min = stats.SpellPowerFor15SecOnUse2Min,
                SpellPowerFor20SecOnUse2Min = stats.SpellPowerFor20SecOnUse2Min,
                HasteRatingFor20SecOnUse2Min = stats.HasteRatingFor20SecOnUse2Min,
                HasteRatingFor20SecOnUse5Min = stats.HasteRatingFor20SecOnUse5Min,
                SpellPowerFor10SecOnCast_15_45 = stats.SpellPowerFor10SecOnCast_15_45,
                SpellPowerFor10SecOnHit_10_45 = stats.SpellPowerFor10SecOnHit_10_45,
                SpellHasteFor10SecOnCast_10_45 = stats.SpellHasteFor10SecOnCast_10_45,
                TimbalsProc = stats.TimbalsProc,
                PendulumOfTelluricCurrentsProc = stats.PendulumOfTelluricCurrentsProc,
                ExtractOfNecromanticPowerProc = stats.ExtractOfNecromanticPowerProc,
                */

                Armor = stats.Armor,
                BonusArmor = stats.BonusArmor,
                Agility = stats.Agility,
                ArcaneResistance = stats.ArcaneResistance,
                ArcaneResistanceBuff = stats.ArcaneResistanceBuff,
                FireResistance = stats.FireResistance,
                FireResistanceBuff = stats.FireResistanceBuff,
                FrostResistance = stats.FrostResistance,
                FrostResistanceBuff = stats.FrostResistanceBuff,
                NatureResistance = stats.NatureResistance,
                NatureResistanceBuff = stats.NatureResistanceBuff,
                ShadowResistance = stats.ShadowResistance,
                ShadowResistanceBuff = stats.ShadowResistanceBuff,
            };

            foreach (SpecialEffect se in stats.SpecialEffects())
                if (RelevantTrinket(se))
                    s.AddSpecialEffect(se);

            return s;
        }

        protected bool RelevantTrinket(SpecialEffect se)
        {
            if (se.Trigger == Trigger.DamageSpellCrit
                || se.Trigger == Trigger.DamageSpellHit
                || se.Trigger == Trigger.DamageSpellCast
                || se.Trigger == Trigger.DoTTick
                || se.Trigger == Trigger.SpellHit
                || se.Trigger == Trigger.SpellCast
                || se.Trigger == Trigger.SpellCrit
                || se.Trigger == Trigger.SpellMiss
                || se.Trigger == Trigger.Use)
            {
                bool bRelevant = _HasRelevantStats(se.Stats);
                foreach (SpecialEffect s in se.Stats.SpecialEffects())
                    bRelevant |= RelevantTrinket(s);
                return bRelevant;
            }
            return false;
        }

        protected bool _HasRelevantStats(Stats stats)
        {
            bool Yes = (
                stats.Intellect + stats.Mana + stats.Spirit + stats.Mp5 + stats.SpellPower
                + stats.SpellShadowDamageRating + stats.CritRating
                + stats.SpellCrit + stats.HitRating + stats.SpellHit + stats.SpellCritOnTarget
                + stats.SpellHaste + stats.HasteRating

                + stats.BonusSpiritMultiplier
                + stats.BonusIntellectMultiplier + stats.BonusManaPotion
                + stats.ThreatReductionMultiplier + stats.BonusDamageMultiplier
                + stats.BonusShadowDamageMultiplier + stats.BonusHolyDamageMultiplier
                + stats.BonusDiseaseDamageMultiplier
                + stats.PriestInnerFire + stats.MovementSpeed

                + stats.SWPDurationIncrease + stats.BonusMindBlastMultiplier
                + stats.MindBlastCostReduction + stats.ShadowWordDeathCritIncrease
                + stats.WeakenedSoulDurationDecrease
                + stats.DevouringPlagueBonusDamage + stats.MindBlastHasteProc
                + stats.PriestDPS_T9_2pc + stats.PriestDPS_T9_4pc
                + stats.PriestDPS_T10_2pc + stats.PriestDPS_T10_4pc
                + stats.ManaRestoreFromBaseManaPPM + stats.BonusSpellCritMultiplier
                + stats.ManaRestore + stats.SpellsManaReduction + stats.HighestStat
                + stats.ArcaneDamage + stats.FireDamage + stats.FrostDamage
                + stats.HolyDamage + stats.NatureDamage + stats.ShadowDamage
                + stats.ValkyrDamage

                /*+ stats.SpellPowerFor15SecOnUse90Sec
                + stats.SpellPowerFor15SecOnUse2Min + stats.SpellPowerFor20SecOnUse2Min
                + stats.HasteRatingFor20SecOnUse2Min + stats.HasteRatingFor20SecOnUse5Min
                + stats.SpellPowerFor10SecOnCast_15_45 + stats.SpellPowerFor10SecOnHit_10_45
                + stats.SpellHasteFor10SecOnCast_10_45 + stats.TimbalsProc
                + stats.PendulumOfTelluricCurrentsProc + stats.ExtractOfNecromanticPowerProc*/                
            ) > 0;

            bool Maybe = (
                stats.Stamina + stats.Health + stats.Resilience
                + stats.Armor + stats.BonusArmor + stats.Agility
                + stats.ArcaneResistance + stats.ArcaneResistanceBuff
                + stats.FireResistance + stats.FireResistanceBuff
                + stats.FrostResistance + stats.FrostResistanceBuff
                + stats.NatureResistance + stats.NatureResistanceBuff
                + stats.ShadowResistance + stats.ShadowResistanceBuff
            ) > 0;

            bool No = (
                stats.Strength + stats.AttackPower
                + stats.ArmorPenetration + stats.ArmorPenetrationRating
                + stats.ExpertiseRating
                + stats.Dodge + stats.DodgeRating
                + stats.Parry + stats.ParryRating
                + stats.Defense + stats.DefenseRating
            ) > 0;

            return Yes || (Maybe && !No);
        }

        public override bool HasRelevantStats(Stats stats)
        {
            bool isRelevant = _HasRelevantStats(stats);

            foreach (SpecialEffect se in stats.SpecialEffects()) {
                isRelevant |= RelevantTrinket(se);
                if (isRelevant) { break; }
            }
            return isRelevant;
        }

        public override ICalculationOptionBase DeserializeDataObject(string xml)
        {
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsShadowPriest));
            System.IO.StringReader reader = new System.IO.StringReader(xml);
            CalculationOptionsShadowPriest calcOpts = serializer.Deserialize(reader) as CalculationOptionsShadowPriest;
            return calcOpts;
        }
    }
}
