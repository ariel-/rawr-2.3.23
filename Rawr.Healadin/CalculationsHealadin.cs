using System;
using System.Collections.Generic;

namespace Rawr.Healadin
{
    [Rawr.Calculations.RawrModelInfo("Healadin", "Spell_Holy_HolyBolt", CharacterClass.Paladin)]
    public class CalculationsHealadin : CalculationsBase
    {

        #region Model Properties
        public override List<GemmingTemplate> DefaultGemmingTemplates {
            get {
                // Relevant Gem IDs for Healadin
                // Red
                int[] runed = { 39911, 39998, 40113, 42144 };       // Spell Power

                // Purple
                int[] royal = { 39943, 40027, 40134, 40134 };       // Spell Power + MP5

                // Blue
                int[] lustrous = { 39927, 40010, 40121, 42146 };    // MP5

                // Green
                int[] dazzling = { 39984, 40094, 40175, 40175 };    // Intellect + MP5
                int[] energized = { 39989, 40105, 40179, 40179 };   // Haste + MP5
                int[] sundered = { 39985, 40096, 40176, 40176 };    // Crit + MP5

                // Yellow
                int[] brilliant = { 39912, 40012, 40123, 42148 };   // Intellect
                int[] quick = { 39918, 40017, 40128, 42150 };       // Haste
                int[] smooth = { 39914, 40013, 40124, 42149 };      // Crit

                // Orange
                int[] luminous = { 39946, 40047, 40151, 40151 };    // Spell Power + Intellect
                int[] potent = { 39956, 40048, 40152, 40152 };      // Spell Power + Crit
                int[] reckless = { 39959, 40051, 40155, 40155 };    // Spell Power + Haste

                // Prismatic
                int[] allStats = { 42701, 42702, 49110, 49110 };    // All Stats

                // Meta
                int insightful = 41401;
                int revitalizing = 41376;

                string[] qualityGroupNames = new string[] { "Uncommon", "Rare", "Epic", "Jeweler" };
                string[] typeGroupNames = new string[] { "MP5", "Spell Power", "Intellect", "Haste", "Critical" };

                int[] metaTemplates = new int[] { insightful, revitalizing };

                //    Red           Yellow      Blue        Prismatic
                int[,][] mp5Templates = new int[,][]
                { // MP5
                    { lustrous,     lustrous,   lustrous,   lustrous },
                    { royal,        dazzling,   lustrous,   lustrous },
                };

                int[,][] spellPowerTemplates = new int[,][]
                { // Spell Power
                    { runed,        runed,      runed,      runed },
                    { runed,        luminous,   royal,      runed },
                };

                int[,][] intellectTemplates = new int[,][]
                { // Intellect
                    { brilliant,    brilliant,  brilliant,  brilliant },
                    { allStats,     brilliant,  brilliant,  brilliant },
                    { brilliant,    allStats,   brilliant,  brilliant },
                    { brilliant,    brilliant,  allStats,   brilliant },
                    { brilliant,    brilliant,  brilliant,  allStats, },
                    { luminous,     brilliant,  dazzling,   brilliant },
                };

                int[,][] hasteTemplates = new int[,][]
                { // Haste
                    { quick,        quick,      quick,      quick },
                    { reckless,     quick,      energized,  quick },
                };

                int[,][] critTemplates = new int[,][]
                { // Crit
                    { smooth,       smooth,     smooth,     smooth },
                    { potent,       smooth,     sundered,   smooth },
                };

                int[][,][] gemmingTemplates = new int[][,][] { mp5Templates, spellPowerTemplates, intellectTemplates, hasteTemplates, critTemplates };

                // Generate List of Gemming Templates
                List<GemmingTemplate> gemmingTemplate = new List<GemmingTemplate>();
                for (int qualityId = 0; qualityId <= qualityGroupNames.GetUpperBound(0); qualityId++) {
                    for (int typeId = 0; typeId <= typeGroupNames.GetUpperBound(0); typeId++) {
                        for (int templateId = 0; templateId <= gemmingTemplates[typeId].GetUpperBound(0); templateId++) {
                            for (int metaId = 0; metaId <= metaTemplates.GetUpperBound(0); metaId++) {
                                gemmingTemplate.Add(new GemmingTemplate()
                                {
                                    Model = "Healadin",
                                    Group = string.Format("{0} - {1}", qualityGroupNames[qualityId], typeGroupNames[typeId]),
                                    RedId = gemmingTemplates[typeId][templateId, 0][qualityId],
                                    YellowId = gemmingTemplates[typeId][templateId, 1][qualityId],
                                    BlueId = gemmingTemplates[typeId][templateId, 2][qualityId],
                                    PrismaticId = gemmingTemplates[typeId][templateId, 3][qualityId],
                                    MetaId = metaTemplates[metaId],
                                    Enabled = qualityGroupNames[qualityId] == "Epic" && typeGroupNames[typeId] == "Intellect",
                                });
                            }
                        }
                    }
                }
                return gemmingTemplate;
            }
        }
        
        //public override List<GemmingTemplate> DefaultGemmingTemplates
        //{
        //    get
        //    {
        //        ////Relevant Gem IDs
        //        //Green
        //        int[] dazzling = { 39984, 40094, 40175 };

        //        //Yellow
        //        int[] brilliant = { 39912, 40012, 40123, 42148 };

        //        //Orange
        //        int[] luminous = { 39946, 40047, 40151 };

        //        //Meta
        //        int insightful = 41401;
        //        // int revitalizing = 41376;

        //        return new List<GemmingTemplate>()
        //        {
        //            new GemmingTemplate() { Model = "Healadin", Group = "Uncommon",
        //                RedId = brilliant[0], YellowId = brilliant[0], BlueId = brilliant[0], PrismaticId = brilliant[0], MetaId = insightful },
        //            new GemmingTemplate() { Model = "Healadin", Group = "Uncommon",
        //                RedId = luminous[0], YellowId = brilliant[0], BlueId = dazzling[0], PrismaticId = brilliant[0], MetaId = insightful },

        //            new GemmingTemplate() { Model = "Healadin", Group = "Rare",
        //                RedId = brilliant[1], YellowId = brilliant[1], BlueId = brilliant[1], PrismaticId = brilliant[1], MetaId = insightful },
        //            new GemmingTemplate() { Model = "Healadin", Group = "Rare",
        //                RedId = luminous[1], YellowId = brilliant[1], BlueId = dazzling[1], PrismaticId = brilliant[1], MetaId = insightful },
                        
        //            new GemmingTemplate() { Model = "Healadin", Group = "Epic", Enabled = true,
        //                RedId = brilliant[2], YellowId = brilliant[2], BlueId = brilliant[2], PrismaticId = brilliant[2], MetaId = insightful },
        //            new GemmingTemplate() { Model = "Healadin", Group = "Epic", Enabled = true,
        //                RedId = luminous[2], YellowId = brilliant[2], BlueId = dazzling[2], PrismaticId = brilliant[2], MetaId = insightful },
                        
        //            new GemmingTemplate() { Model = "Healadin", Group = "Jeweler",
        //                RedId = brilliant[2], YellowId = brilliant[3], BlueId = brilliant[2], PrismaticId = brilliant[2], MetaId = insightful },
        //            new GemmingTemplate() { Model = "Healadin", Group = "Jeweler",
        //                RedId = luminous[2], YellowId = brilliant[3], BlueId = dazzling[2], PrismaticId = brilliant[2], MetaId = insightful },
        //        };
        //    }
        //}

        public override void SetDefaults(Character character)
        {
            character.ActiveBuffsAdd(("Swift Retribution"));
            character.ActiveBuffsAdd(("Arcane Intellect"));
            character.ActiveBuffsAdd(("Judgements of the Wise"));
            character.ActiveBuffsAdd(("Blessing of Wisdom"));
            character.ActiveBuffsAdd(("Improved Blessing of Wisdom"));
            character.ActiveBuffsAdd(("Elemental Oath"));
            character.ActiveBuffsAdd(("Wrath of Air Totem"));
            character.ActiveBuffsAdd(("Totem of Wrath (Spell Power)"));
            character.ActiveBuffsAdd(("Power Word: Fortitude"));
            character.ActiveBuffsAdd(("Improved Power Word: Fortitude"));
            character.ActiveBuffsAdd(("Mark of the Wild"));
            character.ActiveBuffsAdd(("Improved Mark of the Wild"));
            character.ActiveBuffsAdd(("Blessing of Kings"));
            character.ActiveBuffsAdd(("Flask of the Frost Wyrm"));
            character.ActiveBuffsAdd(("Fish Feast"));
            character.ActiveBuffsAdd(("Tree of Life Aura"));
        }

        private string[] _optimizableCalculationLabels = null;
        /// <summary>
        /// Labels of the stats available to the Optimizer 
        /// </summary>
        public override string[] OptimizableCalculationLabels
        {
            get
            {
                if (_optimizableCalculationLabels == null)
                    _optimizableCalculationLabels = new string[] {
                    "Health",
                    "Holy Light Cast Time",
                    "Holy Light HPS",
                    "Holy Light Time",
                    "Flash of Light Cast Time",
                    "Flash of Light HPS",
                    "Flash of Light Time",
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
            get
            {
                if (_calculationOptionsPanel == null)
                {
                    _calculationOptionsPanel = new CalculationOptionsPanelHealadin();
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
                    "Basic Stats:Health",
                    "Basic Stats:Mana",
                    "Basic Stats:Stamina",
                    "Basic Stats:Intellect",
                    "Basic Stats:Spell Power",
                    "Basic Stats:Mp5",
                    "Basic Stats:Spell Crit",
                    "Basic Stats:Spell Haste",

                    "Cycle Stats:Total Healed",
                    "Cycle Stats:Total Mana",
                    "Cycle Stats:Average Healing per sec",
                    "Cycle Stats:Average Healing per mana",

                    "Rotation Info:Holy Light Time",
                    "Rotation Info:Flash of Light Time",
                    "Rotation Info:Holy Shock Time",
                    "Rotation Info:Sacred Shield Time",
                    "Rotation Info:Beacon of Light Time",
                    "Rotation Info:Judgement Time",

                    "Healing Breakdown:Holy Light Healed",
                    "Healing Breakdown:Flash of Light Healed",
                    "Healing Breakdown:Holy Shock Healed",
                    "Healing Breakdown:Sacred Shield Healed",
                    "Healing Breakdown:Beacon of Light Healed",
                    "Healing Breakdown:Glyph of HL Healed",
                    "Healing Breakdown:Other Healed*From trinket procs",

                    "Spell Information:Holy Light",
                    "Spell Information:Flash of Light",
                    "Spell Information:Holy Shock",
                    "Spell Information:Sacred Shield",

                };
                return _characterDisplayCalculationLabels;
            }
        }

        private string[] _customChartNames = null;
        public override string[] CustomChartNames
        {
            get
            {
                if (_customChartNames == null)
                    _customChartNames = new string[] {
                    "Mana Pool Breakdown",
                    "Mana Usage Breakdown",
                    "Healing Breakdown",
                    "Rotation Breakdown",
                    };
                return _customChartNames;
            }
        }

#if RAWR3
        private Dictionary<string, System.Windows.Media.Color> _subPointNameColors = null;
        public override Dictionary<string, System.Windows.Media.Color> SubPointNameColors
        {
            get
            {
                if (_subPointNameColors == null)
                {
                    _subPointNameColors = new Dictionary<string, System.Windows.Media.Color>();
                    _subPointNameColors.Add("Fight Healing", System.Windows.Media.Colors.Red);
                    _subPointNameColors.Add("Burst Healing", System.Windows.Media.Colors.Cyan);
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
                    _subPointNameColors.Add("Fight Healing", System.Drawing.Color.FromArgb(255, 255, 0, 0));
                    _subPointNameColors.Add("Burst Healing", System.Drawing.Color.CornflowerBlue);
                }
                return _subPointNameColors;
            }
        }
#endif

        public override CharacterClass TargetClass { get { return CharacterClass.Paladin; } }
        public override ComparisonCalculationBase CreateNewComparisonCalculation() { return new ComparisonCalculationHealadin(); }
        public override CharacterCalculationsBase CreateNewCharacterCalculations() { return new CharacterCalculationsHealadin(); }

        public override ICalculationOptionBase DeserializeDataObject(string xml)
        {
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsHealadin));
            System.IO.StringReader reader = new System.IO.StringReader(xml);
            CalculationOptionsHealadin calcOpts = serializer.Deserialize(reader) as CalculationOptionsHealadin;
            return calcOpts;
        }
        #endregion

        public override CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, bool referenceCalculation, bool significantChange, bool needsDisplayCalculations)
        {
            if (character == null) return new CharacterCalculationsHealadin();

            Stats stats;
            CharacterCalculationsHealadin calc = null;
            PaladinTalents talents = character.PaladinTalents;

            for (int i = 0; i < 5; i++)
            {
                float oldBurst = 0;
                if (i > 0) oldBurst = calc.BurstPoints;

                stats = GetCharacterStats(character, additionalItem, true, calc);
                calc = new CharacterCalculationsHealadin();

                Rotation rot = new Rotation(character, stats);

                if (i > 0) calc.BurstPoints = oldBurst;
                else calc.BurstPoints = rot.CalculateBurstHealing(calc);
                calc.FightPoints = rot.CalculateFightHealing(calc);
                calc.OverallPoints = calc.BurstPoints + calc.FightPoints;
            }
            calc.BasicStats = GetCharacterStats(character, additionalItem, false, null);

            return calc;
        }

        #region Stat Calculation
        public override Stats GetCharacterStats(Character character, Item additionalItem)
        {
            return GetCharacterStats(character, additionalItem, true, null);
        }

        public Stats GetCharacterStats(Character character, Item additionalItem, bool computeAverageStats, CharacterCalculationsHealadin calc)
        {
            PaladinTalents talents = character.PaladinTalents;
            
            CalculationOptionsHealadin calcOpts = character.CalculationOptions as CalculationOptionsHealadin;
            if (calcOpts == null) calcOpts = new CalculationOptionsHealadin();
            
#if (RAWR3)
            BossOptions bossOpts = character.BossOptions;
            if (bossOpts == null) bossOpts = new BossOptions();
#endif
            
#if (RAWR3)
            float fightLength = bossOpts.BerserkTimer * 60f;
#else
            float fightLength = calcOpts.Length * 60f;
#endif

            Stats statsRace = BaseStats.GetBaseStats(character.Level, CharacterClass.Paladin, character.Race);
            Stats statsBaseGear = GetItemStats(character, additionalItem);
            Stats statsBuffs = GetBuffsStats(character, calcOpts);
            Stats stats = statsBaseGear + statsBuffs + statsRace;

            ConvertRatings(stats, talents, calcOpts);
            if (computeAverageStats)
            {
                Stats statsAverage = new Stats();

                foreach (SpecialEffect effect in stats.SpecialEffects())
                {
                    float trigger = 0f;
                    if (calc == null)
                    {
                        trigger = 1.5f / calcOpts.Activity / (1f + stats.SpellHaste);
                        if (effect.Trigger == Trigger.HealingSpellCrit || effect.Trigger == Trigger.SpellCrit)
                            trigger *= stats.SpellCrit;
                        else if (effect.Trigger == Trigger.HolyShockCast)
                            trigger = 6f / calcOpts.HolyShock;
                    }
                    else
                    {
                        if (effect.Trigger == Trigger.HealingSpellCast || effect.Trigger == Trigger.HealingSpellHit)
                            trigger = 1f / Rotation.GetHealingCastsPerSec(calc);
                        else if (effect.Trigger == Trigger.HealingSpellCrit || effect.Trigger == Trigger.SpellCrit)
                            trigger = 1f / Rotation.GetHealingCritsPerSec(calc);
                        else if (effect.Trigger == Trigger.SpellCast || effect.Trigger == Trigger.SpellHit)
                            trigger = 1f / Rotation.GetSpellCastsPerSec(calc);
                        else if (effect.Trigger == Trigger.DamageOrHealingDone)
                            trigger = 1f / Rotation.GetHealingCastsPerSec(calc);
                        else if (effect.Trigger == Trigger.HolyShockCast)
                            trigger = 6f / calcOpts.HolyShock;
                        else if (effect.Trigger == Trigger.Use)
                        {
                            trigger = 0f;
                            foreach (SpecialEffect childEffect in effect.Stats.SpecialEffects())
                            {
                                float childTrigger = 0f;

                                if (effect.Trigger == Trigger.HealingSpellCast || effect.Trigger == Trigger.HealingSpellHit)
                                    childTrigger = 1f / Rotation.GetHealingCastsPerSec(calc);
                                else if (effect.Trigger == Trigger.HealingSpellCrit || effect.Trigger == Trigger.SpellCrit)
                                    childTrigger = 1f / Rotation.GetHealingCritsPerSec(calc);
                                else if (effect.Trigger == Trigger.SpellCast || effect.Trigger == Trigger.SpellHit)
                                    childTrigger = 1f / Rotation.GetSpellCastsPerSec(calc);

                                statsAverage.Accumulate(childEffect.Stats,
                                                        effect.GetAverageUptime(0.0f, 1.0f)
                                                            * childEffect.GetAverageStackSize(childTrigger, 1f, 1.5f, effect.Duration));
                            }
                        }
                        else if (effect.Trigger == Trigger.HolyLightCast) trigger = 1f / Rotation.GetHolyLightCastsPerSec(calc);
                        else continue;
                    }
                    statsAverage.Accumulate(effect.GetAverageStats(trigger, 1f, 1.5f, fightLength));
                }
                statsAverage.ManaRestore *= fightLength;
                statsAverage.Healed *= fightLength;

                stats = statsBaseGear + statsBuffs + statsRace + statsAverage;
                ConvertRatings(stats, talents, calcOpts);
            }

            return stats;
        }

        private void ConvertRatings(Stats stats, PaladinTalents talents, CalculationOptionsHealadin calcOpts)
        {
            stats.Stamina *= 1f + stats.BonusStaminaMultiplier;

            // Intellect is used to calculate initial mana pool.
            // To avoid temporary intellect from highest stat procs changing initial mana pool
            // we track temporary intellect separatly in HighestStat property and combine it with intellect
            // when needed.
            // TODO: However this doesn't help to deal with pure temporary intellect procs (if there are any).
            // NOTE: If we add highest stat to intellect after we calculate mana, the only visible change
            // will be the displayed intellect for the character.
            stats.Intellect *= (1f + stats.BonusIntellectMultiplier) * (1f + talents.DivineIntellect * .02f);
            stats.HighestStat *= (1f + stats.BonusIntellectMultiplier) * (1f + talents.DivineIntellect * .02f);

            stats.SpellPower += 0.04f * (stats.Intellect + stats.HighestStat) * talents.HolyGuidance;
            stats.SpellCrit = stats.SpellCrit + 
                StatConversion.GetSpellCritFromIntellect(
                    stats.Intellect + stats.HighestStat, 
                    CharacterClass.Paladin) + 
                StatConversion.GetSpellCritFromRating(stats.CritRating, CharacterClass.Paladin) + 
                talents.SanctityOfBattle * .01f + 
                talents.Conviction * .01f;

            stats.SpellHaste = (1f + talents.JudgementsOfThePure * (calcOpts.JotP ? .03f : 0f)) * 
                (1f + stats.SpellHaste) *
                (1f + StatConversion.GetSpellHasteFromRating(stats.HasteRating, CharacterClass.Paladin))
                - 1f;

            // GetManaFromIntellect/GetHealthFromStamina account for the fact 
            // that the first 20 Int/Sta only give 1 Mana/Health each.
            stats.Mana += StatConversion.GetManaFromIntellect(stats.Intellect, CharacterClass.Paladin) * 
                (1f + stats.BonusManaMultiplier);
            stats.Health += StatConversion.GetHealthFromStamina(stats.Stamina, CharacterClass.Paladin);

            stats.PhysicalHit += StatConversion.GetPhysicalHitFromRating(
                stats.HitRating, 
                CharacterClass.Paladin);
        }
        #endregion

        #region Custom Charts
        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
        {
            if (chartName == "Mana Pool Breakdown")
            {
                CharacterCalculationsHealadin calc = GetCharacterCalculations(character) as CharacterCalculationsHealadin;
                if (calc == null) calc = new CharacterCalculationsHealadin();

                ComparisonCalculationHealadin Base = new ComparisonCalculationHealadin("Base");
                ComparisonCalculationHealadin Mp5 = new ComparisonCalculationHealadin("Mp5");
                ComparisonCalculationHealadin Replenishment = new ComparisonCalculationHealadin("Replenishment");
                ComparisonCalculationHealadin ArcaneTorrent = new ComparisonCalculationHealadin("Arcane Torrent");
                ComparisonCalculationHealadin DivinePlea = new ComparisonCalculationHealadin("Divine Plea");
                ComparisonCalculationHealadin LoH = new ComparisonCalculationHealadin("Lay on Hands");
                ComparisonCalculationHealadin Other = new ComparisonCalculationHealadin("Potion & Other");

                Base.OverallPoints = Base.ThroughputPoints = calc.ManaBase;
                Mp5.OverallPoints = Mp5.ThroughputPoints = calc.ManaMp5;
                LoH.OverallPoints = LoH.ThroughputPoints = calc.ManaLayOnHands;
                Replenishment.OverallPoints = Replenishment.ThroughputPoints = calc.ManaReplenishment;
                ArcaneTorrent.OverallPoints = ArcaneTorrent.ThroughputPoints = calc.ManaArcaneTorrent;
                DivinePlea.OverallPoints = DivinePlea.ThroughputPoints = calc.ManaDivinePlea;
                Other.OverallPoints = Other.ThroughputPoints = calc.ManaOther;

                return new ComparisonCalculationBase[] { Base, Mp5, Replenishment, LoH, ArcaneTorrent, DivinePlea, Other };
            }
            else if (chartName == "Mana Usage Breakdown")
            {
                CharacterCalculationsHealadin calc = GetCharacterCalculations(character) as CharacterCalculationsHealadin;
                if (calc == null) calc = new CharacterCalculationsHealadin();

                ComparisonCalculationHealadin FoL = new ComparisonCalculationHealadin("Flash of Light");
                ComparisonCalculationHealadin HL = new ComparisonCalculationHealadin("Holy Light");
                ComparisonCalculationHealadin HS = new ComparisonCalculationHealadin("Holy Shock");
                ComparisonCalculationHealadin JotP = new ComparisonCalculationHealadin("Judgements and Seals");
                ComparisonCalculationHealadin BoL = new ComparisonCalculationHealadin("Beacon of Light");
                ComparisonCalculationHealadin SS = new ComparisonCalculationHealadin("Sacred Shield");

                FoL.OverallPoints = FoL.ThroughputPoints = calc.UsageFoL;
                HL.OverallPoints = HL.ThroughputPoints = calc.UsageHL;
                HS.OverallPoints = HS.ThroughputPoints = calc.UsageHS;
                JotP.OverallPoints = JotP.ThroughputPoints = calc.UsageJotP;
                BoL.OverallPoints = BoL.ThroughputPoints = calc.UsageBoL;
                SS.OverallPoints = SS.ThroughputPoints = calc.UsageSS;

                return new ComparisonCalculationBase[] { FoL, HL, HS, JotP, BoL, SS };
            }
            else if (chartName == "Healing Breakdown")
            {
                CharacterCalculationsHealadin calc = GetCharacterCalculations(character) as CharacterCalculationsHealadin;
                if (calc == null) calc = new CharacterCalculationsHealadin();

                ComparisonCalculationHealadin FoL = new ComparisonCalculationHealadin("Flash of Light");
                ComparisonCalculationHealadin HL = new ComparisonCalculationHealadin("Holy Light");
                ComparisonCalculationHealadin HS = new ComparisonCalculationHealadin("Holy Shock");
                ComparisonCalculationHealadin GHL = new ComparisonCalculationHealadin("Glyph of Holy Light");
                ComparisonCalculationHealadin BoL = new ComparisonCalculationHealadin("Beacon of Light");
                ComparisonCalculationHealadin SS = new ComparisonCalculationHealadin("Sacred Shield");

                FoL.OverallPoints = FoL.ThroughputPoints = calc.HealedFoL / calc.FightLength;
                HL.OverallPoints = HL.ThroughputPoints = calc.HealedHL / calc.FightLength;
                HS.OverallPoints = HS.ThroughputPoints = calc.HealedHS / calc.FightLength;
                GHL.OverallPoints = GHL.ThroughputPoints = calc.HealedGHL / calc.FightLength;
                BoL.OverallPoints = BoL.ThroughputPoints = calc.HealedBoL / calc.FightLength;
                SS.OverallPoints = SS.ThroughputPoints = calc.HealedSS / calc.FightLength;

                return new ComparisonCalculationBase[] { FoL, HL, HS, GHL, BoL, SS };
            }
            else if (chartName == "Rotation Breakdown")
            {
                CharacterCalculationsHealadin calc = GetCharacterCalculations(character) as CharacterCalculationsHealadin;
                if (calc == null) calc = new CharacterCalculationsHealadin();

                ComparisonCalculationHealadin FoL = new ComparisonCalculationHealadin("Flash of Light");
                ComparisonCalculationHealadin HL = new ComparisonCalculationHealadin("Holy Light");
                ComparisonCalculationHealadin HS = new ComparisonCalculationHealadin("Holy Shock");
                ComparisonCalculationHealadin JotP = new ComparisonCalculationHealadin("Judgements and Seals");
                ComparisonCalculationHealadin BoL = new ComparisonCalculationHealadin("Beacon of Light");
                ComparisonCalculationHealadin SS = new ComparisonCalculationHealadin("Sacred Shield");

                FoL.OverallPoints = FoL.ThroughputPoints = calc.RotationFoL;
                HL.OverallPoints = HL.ThroughputPoints = calc.RotationHL;
                HS.OverallPoints = HS.ThroughputPoints = calc.RotationHS;
                JotP.OverallPoints = JotP.ThroughputPoints = calc.RotationJotP;
                BoL.OverallPoints = BoL.ThroughputPoints = calc.RotationBoL;
                SS.OverallPoints = SS.ThroughputPoints = calc.RotationSS;

                return new ComparisonCalculationBase[] { FoL, HL, HS, JotP, BoL, SS };
            }
            return new ComparisonCalculationBase[] {};
        }
        #endregion Custom Charts

        #region Relevancy Methods

        private static bool isSpiritIrrelevant = true;
        internal static bool IsSpiritIrrelevant
        {
            get { return isSpiritIrrelevant; }
            set { isSpiritIrrelevant = value; }
        }

        private static bool isHitIrrelevant = true;
        internal static bool IsHitIrrelevant
        {
            get { return isHitIrrelevant; }
            set { isHitIrrelevant = value; }
        }

        private List<ItemType> _relevantItemTypes = null;
        public override List<ItemType> RelevantItemTypes
        {
            get
            {
                if (_relevantItemTypes == null)
                {
                    _relevantItemTypes = new List<ItemType>(new ItemType[]
                    {
                        ItemType.Plate,
                        ItemType.Mail,
                        ItemType.Leather,
                        ItemType.Cloth,
                        ItemType.None,
                        ItemType.Shield,
                        ItemType.Libram,
                        ItemType.OneHandAxe,
                        ItemType.OneHandMace,
                        ItemType.OneHandSword
                    });
                }
                return _relevantItemTypes;
            }
        }

        private static List<string> _relevantGlyphs;
        public override List<string> GetRelevantGlyphs()
        {
            if (_relevantGlyphs == null)
            {
                _relevantGlyphs = new List<string>();
                _relevantGlyphs.Add("Glyph of Seal of Wisdom");
                _relevantGlyphs.Add("Glyph of Seal of Light");
                _relevantGlyphs.Add("Glyph of Holy Light");
                _relevantGlyphs.Add("Glyph of Flash of Light");
                _relevantGlyphs.Add("Glyph of Holy Shock");
                _relevantGlyphs.Add("Glyph of Divinity");
                _relevantGlyphs.Add("Glyph of Beacon of Light");
                _relevantGlyphs.Add("Glyph of the Wise");
                _relevantGlyphs.Add("Glyph of Lay on Hands");
            }
            return _relevantGlyphs;
        }

        public override bool EnchantFitsInSlot(Enchant enchant, Character character, ItemSlot slot)
        {
            // Filters out Non-Shield Offhand Enchants and Ranged Enchants
            if ((slot == ItemSlot.OffHand && enchant.Slot != ItemSlot.OffHand) || slot == ItemSlot.Ranged) return false;
            // Filters out Death Knight and Two-Hander Enchants
            if (enchant.Name.StartsWith("Rune of the") || enchant.Slot == ItemSlot.TwoHand) return false;

            return base.EnchantFitsInSlot(enchant, character, slot);
        }

        public override bool ItemFitsInSlot(Item item, Character character, CharacterSlot slot, bool ignoreUnique)
        {
            if (slot == CharacterSlot.OffHand && !(item.Type == ItemType.Shield || item.Type == ItemType.None)) return false;
            return base.ItemFitsInSlot(item, character, slot, ignoreUnique);
        }

        private bool IsTriggerRelevant(Trigger trigger)
        {
            return (
                trigger == Trigger.Use                 || trigger == Trigger.HolyLightCast     ||
                trigger == Trigger.SpellCast           || trigger == Trigger.SpellCrit         ||
                trigger == Trigger.SpellHit            || trigger == Trigger.HealingSpellCast  ||
                trigger == Trigger.HealingSpellCrit    || trigger == Trigger.HealingSpellHit   ||
                trigger == Trigger.DamageOrHealingDone || trigger == Trigger.HolyShockCast
            );
        }

        public override Stats GetRelevantStats(Stats stats)
        {
            Stats s = new Stats()
            {
                #region These aren't really relevant stats, but they should still appear in the tooltip

                Stamina = stats.Stamina,
                Health = stats.Health,
                Spirit = stats.Spirit,
                HitRating = stats.HitRating,

                #endregion

                Intellect = stats.Intellect,
                Mp5 = stats.Mp5,
                SpellPower = stats.SpellPower,
                CritRating = stats.CritRating,
                HasteRating = stats.HasteRating,
                Mana = stats.Mana,
                ManaRestore = stats.ManaRestore,
                BonusIntellectMultiplier = stats.BonusIntellectMultiplier,
                BonusManaPotion = stats.BonusManaPotion,
                SpellCrit = stats.SpellCrit,
                SpellHaste = stats.SpellHaste,
                FlashOfLightCrit = stats.FlashOfLightCrit,
                FlashOfLightSpellPower = stats.FlashOfLightSpellPower,
                FlashOfLightMultiplier = stats.FlashOfLightMultiplier,
                HolyShockCrit = stats.HolyShockCrit,
                HolyLightSpellPower = stats.HolyLightSpellPower,
                HolyLightCrit = stats.HolyLightCrit,
                HolyLightManaCostReduction = stats.HolyLightManaCostReduction,
                HolyLightPercentManaReduction = stats.HolyLightPercentManaReduction,
                HealingReceivedMultiplier = stats.HealingReceivedMultiplier,
                BonusHealingDoneMultiplier = stats.BonusHealingDoneMultiplier,
                BonusJudgementDuration = stats.BonusJudgementDuration,
                FlashOfLightHoTMultiplier = stats.FlashOfLightHoTMultiplier,
                DivineIlluminationHealingMultiplier = stats.DivineIlluminationHealingMultiplier,
                HolyLightCastTimeReductionFromHolyShock = stats.HolyLightCastTimeReductionFromHolyShock,
                MovementSpeed = stats.MovementSpeed,
                ShieldFromHealed = stats.ShieldFromHealed,

                // Gear Procs
                ManaRestoreFromMaxManaPerSecond = stats.ManaRestoreFromMaxManaPerSecond,
                BonusManaMultiplier = stats.BonusManaMultiplier,
                BonusCritHealMultiplier = stats.BonusCritHealMultiplier,
                SpellsManaReduction = stats.SpellsManaReduction,
                SacredShieldICDReduction = stats.SacredShieldICDReduction,
                HolyShockHoTOnCrit = stats.HolyShockHoTOnCrit,

                // Ony Shiny Shard of the Flame
                Healed = stats.Healed,
            };
            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if (IsTriggerRelevant(effect.Trigger) && HasRelevantStats(effect.Stats))
                {
                    s.AddSpecialEffect(effect);
                }
            }
            return s;
        }

        bool useIrrelevancy = false;
        public override bool IsItemRelevant(Item item)
        {
            if (!item.IsGem) { useIrrelevancy = true; }
            bool result = base.IsItemRelevant(item);
            useIrrelevancy = false;
            return result;
        }

        public override bool HasRelevantStats(Stats stats)
        {
            if (useIrrelevancy)
            {
                if (isSpiritIrrelevant && stats.Spirit > 0) return false;
                if (isHitIrrelevant && stats.HitRating > 0) return false;
            }

            bool relevant = (
                stats.Intellect +
                stats.Mp5 +
                stats.SpellPower +
                stats.CritRating +
                stats.HasteRating +
                stats.Mana +
                stats.ManaRestore +
                stats.BonusIntellectMultiplier +
                stats.BonusManaPotion +
                stats.SpellCrit +
                stats.SpellHaste +
                stats.FlashOfLightCrit +
                stats.FlashOfLightSpellPower +
                stats.FlashOfLightMultiplier +
                stats.HolyShockCrit +
                stats.HolyLightSpellPower +
                stats.HolyLightCrit +
                stats.HolyLightManaCostReduction +
                stats.HolyLightPercentManaReduction +
                stats.HealingReceivedMultiplier +
                stats.BonusHealingDoneMultiplier +
                stats.BonusJudgementDuration +
                stats.FlashOfLightHoTMultiplier +
                stats.DivineIlluminationHealingMultiplier +
                stats.HolyLightCastTimeReductionFromHolyShock +
                stats.MovementSpeed +
                stats.ShieldFromHealed +
                stats.HighestStat +

                stats.ManaRestoreFromMaxManaPerSecond +
                stats.BonusManaMultiplier +
                stats.BonusCritHealMultiplier +
                stats.SpellsManaReduction +
                stats.SacredShieldICDReduction +
                stats.HolyShockHoTOnCrit +

                stats.Healed
                ) != 0;

            bool hasTrigger = false;
            bool hasRelevantTrigger = false;
            bool triggerHasRelevantStats;

            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                hasTrigger = true;
                if (IsTriggerRelevant(effect.Trigger))
                {
                    triggerHasRelevantStats = HasRelevantStats(effect.Stats);
                    relevant |= triggerHasRelevantStats;
                    hasRelevantTrigger |= triggerHasRelevantStats;
                }
            }
            return relevant && (!hasTrigger || hasRelevantTrigger);
        }

        public override bool IsBuffRelevant(Buff buff, Character character)
        {
            Stats stats = buff.Stats;
            bool hasRelevantBuffStats = HasRelevantStats(stats);

            bool NotClassSetBonus = buff.Group == "Set Bonuses" && (buff.AllowedClasses.Count != 1 || (buff.AllowedClasses.Count == 1 && !buff.AllowedClasses.Contains(CharacterClass.Paladin)));

            bool relevant = hasRelevantBuffStats && !NotClassSetBonus;
            return relevant;
        }

        public Stats GetBuffsStats(Character character, CalculationOptionsHealadin calcOpts) {
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
        #endregion
    }
}
