using System;
using System.Collections.Generic;
using System.IO;
#if RAWR3
using System.Windows.Media;
#else
using System.Drawing;
#endif
using System.Xml.Serialization;

namespace Rawr.Tree {

    public class DiminishingReturns
    {
        private double C;
        private double D;
        private float multiplier;
        private float factor;
        public DiminishingReturns(float multiplier, int factor)
        {
            C = Math.Pow(multiplier / factor, factor / (factor - 1));
            D = Math.Pow(C, 1 / factor);
            this.multiplier = multiplier;
            this.factor = factor;
        }
        public float Cap(float value, float cap)
        {
            if (cap >= value) return value;
            return (float)(cap + multiplier * cap * (Math.Pow(value / cap - 1 + C, 1 / factor) - D));
        }
        // DiminishingReturns(multiplier,factor).Cap(value,cap) gives the same result as this function
        public static float Cap(float value, float cap, float multiplier, int factor)
        {
            if (cap >= value) return value;
            double C = Math.Pow(multiplier / factor, factor / (factor - 1));
            return (float)(cap + multiplier * cap * (Math.Pow(value / cap - 1 + C, 1 / factor) - Math.Pow(C, 1 / factor)));
        }
        // max > cap, adviced is cap < max <= 2*cap
        public static float CapWithMaximum(float value, float cap, float max)
        {
            if (cap >= value) return value;
            return (value * max - cap * cap) / (value + max - 2 * cap);
        }
        // with max = 2 * cap 
        // This is exactly the same as the one above, actually, but with 2*cap the formula is much simpler
        public static float CapWithMaximum2(float value, float cap)
        {
            if (cap >= value) return value;
            return cap * (2 - cap/value);
        }
        /*
         * Derivation of Cap:
         * we want to base on a function like "y = x^(1/n)"
         * parameters: cap, x, C and D (a constant tbd), n, f (a multiplier)
         * y'(cap) = 1
         * y(cap) = cap
         * y = cap + f * cap * ( (x/cap-1+C)^(1/n) - (C)^(1/n) ) + D
         * y' = f * (C + x/cap - 1)^(1/n - 1) / n = 1 when x=cap
         *   1   = f/n * (C)^(1/n-1)
         *   n/f = (C)^((1-n)/n) 
         *   C   = (n/f)^(n/(1-n))
         *   C   = (f/n)^(n/(n-1))
         * y(cap) = cap
         *   0   = f * cap * ((x/cap-1+C)^(1/n) - (C)^(1/n)) + D
         *   0   = f * cap * ((C)^(1/n) - (C)^(1/n)) + D
         *   0   = D
         *   
         * Derivation of CapWithMaximum:
         * we want to base on a function like "y = max - 1/x"
         * parameters: cap, max, A and B (a constant tbd)
         * y'(cap) = 1
         * y(cap) = cap
         * limit of x to infinity = max
         *  .... to be written out eventually
         * y = (x * max - cap^2) / (x + max - 2*cap)
         * y' = (max - cap)^2 / (max - 2 * cap + x)^2
         * y'(cap) = (max - cap)^2 / (max - cap)^2 = 1
         * y(cap) = (cap * max - cap*cap) / (cap + max - 2 * cap)
         *        = cap * (max - cap) / (max - cap) = 1
         * limit of x to infinity:
         * y = (x * max - cap^2) / (x + max - 2*cap) with x to infinity ==> differentiate top and bottom with respect to x
         * y = (max) / (1) = max
         */
    }

    [Rawr.Calculations.RawrModelInfo("Tree", "Ability_Druid_TreeofLife", CharacterClass.Druid)]
    public class CalculationsTree : CalculationsBase {
        private List<GemmingTemplate> _defaultGemmingTemplates = null;
        public override List<GemmingTemplate> DefaultGemmingTemplates {
            get {
                if (_defaultGemmingTemplates == null) {
                    // Meta
                    int ember = 41333;
                    int revitalizing = 41376;
                    int insightful = 41401;

                    // [0] uncommon
                    // [1] perfect uncommon
                    // [2] rare
                    // [3] epic
                    // [4] jewelcrafting

                    // Reds
                    int[] runed = { 39911, 41438, 39998, 40113, 42144 }; // ** spell power
                    // Blue
                    int[] lustrous = { 39927, 41440, 40010, 40121, 42146 }; // mp5 [pure]
                    int[] sparkling = { 39920, 41442, 40009, 40120, 42145 }; // spi [pure]
                    // Yellow
                    int[] brilliant = { 39912, 41444, 40012, 40123, 42148 }; // int [pure]
                    // Purple
                    int[] purified = { 39941, 41457, 40026, 40133 }; // ** spell power + spirit
                    int[] royal = { 39943, 41459, 40027, 40134 }; // spell power + mp5
                    // Green
                    int[] dazzling = { 39984, 41463, 40094, 40175 }; // int + mp5
                    int[] seers = { 39979, 41473, 40092, 40170 }; // int + spi
                    // Orange
                    int[] luminous = { 39946, 41494, 40047, 40151 }; // int + spell power
                    int[] reckless = { 39959, 41497, 40051, 40155 }; // ** spell power + haste

                    /*
                     * Gemmings
                     * No Crit, Spirit > MP5
                     * red: Runed, Purified, Luminous
                     * yellow: Brilliant, Seer's, Luminous
                     * blue: Sparkling, Purified, Seer's
                     * = runed, purified, luminous, brilliant, seer's, sparkling
                     * seems seer's is always better than sparkling
                     * Meta: use revitalizing (better than ember)
                     */

                    _defaultGemmingTemplates = new List<GemmingTemplate>();
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Uncommon (Revitalizing)", false, runed[0], purified[0], reckless[0], revitalizing);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Uncommon (Ember)", false, runed[0], purified[0], reckless[0], ember);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Uncommon (Insightful)", false, runed[0], purified[0], reckless[0], insightful);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Perfect (Revitalizing)", false, runed[1], purified[1], reckless[1], revitalizing);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Perfect (Ember)", false, runed[1], purified[1], reckless[1], ember);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Perfect (Insightful)", false, runed[1], purified[1], reckless[1], insightful);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Rare (Revitalizing)", false, runed[2], purified[2], reckless[2], revitalizing);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Rare (Ember)", false, runed[2], purified[2], reckless[2], ember);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Rare (Insightful)", false, runed[2], purified[2], reckless[2], insightful);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Epic (Revitalizing)", true, runed[3], purified[3], reckless[3], revitalizing);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Epic (Ember)", true, runed[3], purified[3], reckless[3], ember);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Epic (Insightful)", true, runed[3], purified[3], reckless[3], insightful);
                    AddJCGemmingTemplateGroup(_defaultGemmingTemplates, "Jewelcrafting (Revitalizing)", false, runed[4], revitalizing);
                    AddJCGemmingTemplateGroup(_defaultGemmingTemplates, "Jewelcrafting (Ember)", false, runed[4], ember);
                    AddJCGemmingTemplateGroup(_defaultGemmingTemplates, "Jewelcrafting (Insightful)", false, runed[4], insightful);
                }
                return _defaultGemmingTemplates;
            }
        }
        private void AddGemmingTemplateGroup(List<GemmingTemplate> list, string name, bool enabled, int runed, int purified, int reckless, int meta) {
            // Overrides, only "runed" and "seers"
            list.Add(new GemmingTemplate() { Model = "Tree", Group = name, RedId = runed, YellowId = runed, BlueId = runed, PrismaticId = runed, MetaId = meta, Enabled = enabled });
            list.Add(new GemmingTemplate() { Model = "Tree", Group = name, RedId = runed, YellowId = reckless, BlueId = purified, PrismaticId = runed, MetaId = meta, Enabled = enabled });
        }
        private void AddJCGemmingTemplateGroup(List<GemmingTemplate> list, string name, bool enabled, int runed, int meta) {
            list.Add(new GemmingTemplate() { Model = "Tree", Group = name, RedId = runed, YellowId = runed, BlueId = runed, PrismaticId = runed, MetaId = meta, Enabled = enabled });
        }

        private static List<string> _relevantGlyphs;
        public override List<string> GetRelevantGlyphs()
        {
            if (_relevantGlyphs == null)
            {
                _relevantGlyphs = new List<string>();
                _relevantGlyphs.Add("Glyph of Healing Touch");
                _relevantGlyphs.Add("Glyph of Innervate");
                _relevantGlyphs.Add("Glyph of Lifebloom");
                _relevantGlyphs.Add("Glyph of Nourish");
                _relevantGlyphs.Add("Glyph of Rebirth");
                _relevantGlyphs.Add("Glyph of Regrowth");
                _relevantGlyphs.Add("Glyph of Rejuvenation");
                _relevantGlyphs.Add("Glyph of Swiftmend");
                _relevantGlyphs.Add("Glyph of Wild Growth");
                _relevantGlyphs.Add("Glyph of Rapid Rejuvenation");
            }
            return _relevantGlyphs;
        }
        
        private Dictionary<string, Color> _subPointNameColors = null;
        private Dictionary<string, Color> _subPointNameColorsRating = null;
        private Dictionary<string, Color> _subPointNameColorsMPS = null;
        private Dictionary<string, Color> _subPointNameColorsHPS = null;
        private Dictionary<string, Color> _subPointNameColorsHPCT = null;
        private Dictionary<string, Color> _subPointNameColorsHPM = null;
        private Dictionary<string, Color> _subPointNameColorsCF = null;

        public CalculationsTree()
        {
            _subPointNameColorsRating = new Dictionary<string, Color>();
            _subPointNameColorsRating.Add("SingleTarget", Color.FromArgb(255, 255, 0, 0));
            _subPointNameColorsRating.Add("Sustained", Color.FromArgb(255, 0, 0, 255));
            _subPointNameColorsRating.Add("Survival", Color.FromArgb(255, 0, 128, 0));

            _subPointNameColorsMPS = new Dictionary<string, Color>();
            _subPointNameColorsMPS.Add("Mana per second", Color.FromArgb(128, 0, 0, 255));

            _subPointNameColorsHPS = new Dictionary<string, Color>();
            _subPointNameColorsHPS.Add("Healing per second", Color.FromArgb(128, 0, 255, 0));

            _subPointNameColorsHPCT = new Dictionary<string, Color>();
            _subPointNameColorsHPCT.Add("Healing per cast time", Color.FromArgb(128, 0, 255, 0));

            _subPointNameColorsHPM = new Dictionary<string, Color>();
            _subPointNameColorsHPM.Add("Healing per mana", Color.FromArgb(128, 0, 255, 255));

            _subPointNameColorsCF = new Dictionary<string, Color>();
            _subPointNameColorsCF.Add("Casting time percentage", Color.FromArgb(196, 0, 255, 0));
            _subPointNameColorsCF.Add("Mana reduction", Color.FromArgb(196, 255, 255, 0));
            _subPointNameColorsCF.Add("Time reduction", Color.FromArgb(196, 255, 0, 0));

            _subPointNameColors = _subPointNameColorsRating;
        }

        public override Dictionary<string, Color> SubPointNameColors {
            get {
                Dictionary<string, Color> ret = _subPointNameColors;
                _subPointNameColors = _subPointNameColorsRating;
                return ret;
            }
        }

        private string[] _characterDisplayCalculationLabels = null;
        public override string[] CharacterDisplayCalculationLabels {
            get {
                if (_characterDisplayCalculationLabels == null) {
                    _characterDisplayCalculationLabels = new string[] {
                        "Points:Single Target Points",
                        "Points:Sustained Points",
                        @"Points:Survival Points*Survival Points represents the total raw physical damage 
you can take before dying. Calculated based on Health
and Armor damage reduction. (Survival multiplier is 
applied and result is scaled down by 100)",
                        "Points:Overall Points",

                        "Base Stats:Base Health",
                        "Base Stats:Base Armor",
                        "Base Stats:Base Mana",
                        "Base Stats:Base Stamina",
                        "Base Stats:Base Intellect",
                        "Base Stats:Base Spirit",
                        "Base Stats:Base Spell Power",
                        "Base Stats:Base Spell Crit",
                        "Base Stats:Base Spell Haste",
                        "Base Stats:Base Global CD",

                        "Combat Stats:Health",
                        "Combat Stats:Armor",
                        "Combat Stats:Mana",
                        "Combat Stats:Stamina",
                        "Combat Stats:Intellect",
                        "Combat Stats:Spirit",
                        "Combat Stats:Spell Power",
                        "Combat Stats:Spell Crit",
                        "Combat Stats:Spell Haste",
                        "Combat Stats:Global CD",

                        "Model:Total Time",
                        "Model:Time until OOM (unreduced)",
                        "Model:Time until OOM",
                        "Model:Total healing done",
                        "Model:Sustained HPS",
                        "Model:Single Target HPS",
                        "Model:Mana regen per second",
                        "Model:Mana from innervates",
                        "Model:Average casts per minute",
                        "Model:Average crits per minute",
                        "Model:Average heals per minute",
                        "Model:Rejuvenation casts per minute",
                        "Model:Rejuvenation average up",
                        "Model:Regrowth casts per minute",
                        "Model:Regrowth average up",
                        "Model:Lifebloom (stack) casts per minute",
                        "Model:Lifebloom (stack) average up",
                        "Model:Lifebloom (stack) method",
                        "Model:Lifebloom casts per minute",
                        "Model:Lifebloom average up",
                        "Model:Nourish casts per minute",
                        //"Model:Healing Touch casts per minute",
                        "Model:Swiftmend casts per minute",
                        "Model:Wild Growth casts per minute",
                        "Model:Revitalize procs per minute",

                        "Rejuvenation:RJ Heal per tick",
                        "Rejuvenation:RJ Tick time",
                        "Rejuvenation:RJ HPS",
                        "Rejuvenation:RJ HPM",
                        "Rejuvenation:RJ HPCT",

                        "Regrowth:RG Heal",
                        "Regrowth:RG Tick",
                        "Regrowth:RG HPS",
                        "Regrowth:RG HPS (HoT only)",
                        "Regrowth:RG HPM",
                        "Regrowth:RG HPCT",
                        "Regrowth:RG Heal (spam)",
                        "Regrowth:RG HPS (spam)",
                        "Regrowth:RG HPM (spam)",
                        "Regrowth:RG HPCT (spam)",

                        "Lifebloom:LB Tick",
                        "Lifebloom:LB Bloom Heal",
                        "Lifebloom:LB HPS",
                        "Lifebloom:LB HPS (HoT only)",
                        "Lifebloom:LB HPM",
                        "Lifebloom:LB HPCT",

                        "Lifebloom Stacked Blooms:LBx2 (fast stack) HPS",
                        "Lifebloom Stacked Blooms:LBx2 (fast stack) HPM",
                        "Lifebloom Stacked Blooms:LBx2 (fast stack) HPCT",

                        "Lifebloom Stacked Blooms:LBx3 (fast stack) HPS",
                        "Lifebloom Stacked Blooms:LBx3 (fast stack) HPM",
                        "Lifebloom Stacked Blooms:LBx3 (fast stack) HPCT",

                        "Lifebloom Stacked Blooms:LBx2 (slow stack) HPS",
                        "Lifebloom Stacked Blooms:LBx2 (slow stack) HPM",
                        "Lifebloom Stacked Blooms:LBx2 (slow stack) HPCT",

                        "Lifebloom Stacked Blooms:LBx3 (slow stack) HPS",
                        "Lifebloom Stacked Blooms:LBx3 (slow stack) HPM",
                        "Lifebloom Stacked Blooms:LBx3 (slow stack) HPCT",

                        "Lifebloom Stacked Blooms:LBx3 (rolling) Tick",
                        "Lifebloom Stacked Blooms:LBx3 (rolling) HPS",
                        "Lifebloom Stacked Blooms:LBx3 (rolling) HPM",
                        "Lifebloom Stacked Blooms:LBx3 (rolling) HPCT",

                        "Healing Touch:HT Heal",
                        "Healing Touch:HT HPS",
                        "Healing Touch:HT HPM",
                        "Healing Touch:HT HPCT",

                        "Wild Growth:WG first Tick",
                        "Wild Growth:WG HPS(single target)",
                        "Wild Growth:WG HPM(single target)",
                        "Wild Growth:WG HPS(max)",
                        "Wild Growth:WG HPM(max)",

                        "Nourish:N Heal",
                        "Nourish:N HPM",
                        "Nourish:N HPS",
                        "Nourish:N HPCT",
                        "Nourish:N (1 HoT) Heal",
                        "Nourish:N (1 HoT) HPM",
                        "Nourish:N (1 HoT) HPS",
                        "Nourish:N (1 HoT) HPCT",
                        "Nourish:N (2 HoTs) Heal",
                        "Nourish:N (2 HoTs) HPM",
                        "Nourish:N (2 HoTs) HPS",
                        "Nourish:N (2 HoTs) HPCT",
                        "Nourish:N (3 HoTs) Heal",
                        "Nourish:N (3 HoTs) HPM",
                        "Nourish:N (3 HoTs) HPS",
                        "Nourish:N (3 HoTs) HPCT",
                        "Nourish:N (4 HoTs) Heal",
                        "Nourish:N (4 HoTs) HPM",
                        "Nourish:N (4 HoTs) HPS",
                        "Nourish:N (4 HoTs) HPCT",

                        "Swiftmend:SM Rejuv Heal",
                        "Swiftmend:SM Rejuv HPM",
                        "Swiftmend:SM Rejuv Lost Ticks",
                        "Swiftmend:SM Regrowth Heal",
                        "Swiftmend:SM Regrowth HPM",
                        "Swiftmend:SM Regrowth Lost Ticks",
                        "Swiftmend:SM Both Heal",
                        "Swiftmend:SM Both HPM",
                        "Swiftmend:SM Both Rejuv Lost Ticks",
                        "Swiftmend:SM Both Regrowth Lost Ticks",
                    };
                }
                return _characterDisplayCalculationLabels;
            }
        }
        private string[] _customChartNames = null;
        public override string[] CustomChartNames {
            get {
                if (_customChartNames == null) {
                    _customChartNames = new string[] {
                        "Single target spell mixes (HPS)",
                        "Single target spell mixes (HPM)",
                        "Single target spell mixes (MPS)",
                        "Healing per spell (single target)",
					    "Mana sources (sustained)",
                        "Mana usage per spell (sustained)",
                        "Total HPS per spell (sustained)",
                        "Casting time percentage per spell (sustained)",
                        "HPCT per spell",
                        "HPS per spell",
                        "HPM per spell",
					};
                }
                return _customChartNames;
            }
        }

#if !RAWR3
        // for RAWR3 include all charts in CustomChartNames
        private string[] _customRenderedChartNames = null;
        public override string[] CustomRenderedChartNames
        {
            get
            {
                if (_customRenderedChartNames == null)
                {
                    _customRenderedChartNames = new string[] {"Stats Graph"};
                }
                return _customRenderedChartNames;
            }
        }
#endif

        private string[] _optimizableCalculationLabels = null;
        public override string[] OptimizableCalculationLabels {
            get {
                if (_optimizableCalculationLabels == null)
                    _optimizableCalculationLabels = new string[] {
					    "Mana",
					    "MP5",
                        "Global CD",
					};
                return _optimizableCalculationLabels;
            }
        }
#if RAWR3
        private ICalculationOptionsPanel _calculationOptionsPanel = null;
        public override ICalculationOptionsPanel CalculationOptionsPanel
#else
        private CalculationOptionsPanelTree _calculationOptionsPanel = null;
        public override CalculationOptionsPanelBase CalculationOptionsPanel
#endif
        {
            get {
                if (_calculationOptionsPanel == null) { _calculationOptionsPanel = new CalculationOptionsPanelTree(); }
                return _calculationOptionsPanel;
            }
        }
        private List<ItemType> _relevantItemTypes = null;
        public override List<ItemType> RelevantItemTypes {
            get {
                if (_relevantItemTypes == null) {
                    _relevantItemTypes = new List<ItemType>(new ItemType[]{
                        ItemType.None,
                        ItemType.Cloth,
                        ItemType.Leather,
                        ItemType.Dagger,
                        ItemType.FistWeapon,
                        ItemType.Idol,
                        ItemType.OneHandMace,
                        ItemType.TwoHandMace,
                        ItemType.Staff
                    });
                }
                return _relevantItemTypes;
            }
        }
        public override CharacterClass TargetClass { get { return CharacterClass.Druid; } }
        public override ComparisonCalculationBase CreateNewComparisonCalculation() { return new ComparisonCalculationTree(); }
        public override CharacterCalculationsBase CreateNewCharacterCalculations() { return new CharacterCalculationsTree(); }
        protected RotationSettings getRotationFromCalculationOptions(Stats stats, CalculationOptionsTree calcOpts, CharacterCalculationsTree calculatedStats) {
            RotationSettings settings = new RotationSettings();
            SpellProfile profile = calcOpts.Current;

            switch (profile.LifebloomStackType)
            {
                case 0:
                    settings.lifeBloomType = LifeBloomType.Slow;
                    break;
                case 1:
                    settings.lifeBloomType = LifeBloomType.Fast;
                    break;
                case 2:
                default:
                    settings.lifeBloomType = LifeBloomType.Rolling;
                    break;
                case 3:
                    settings.lifeBloomType = LifeBloomType.Slow2;
                    break;
                case 4:
                    settings.lifeBloomType = LifeBloomType.Fast2;
                    break;
            }

            settings.averageLifebloomStacks = (float)profile.LifebloomStackAmount;
            settings.averageRejuv = (float)profile.RejuvAmount;
            settings.averageRegrowth = (float)profile.RegrowthAmount;

            settings.SwiftmendPerMin = profile.SwiftmendPerMinute;
            settings.WildGrowthPerMin = profile.WildGrowthPerMinute;

            settings.RejuvFraction = (float)profile.RejuvFrac / 100.0f;
            settings.LifebloomFraction = (float)profile.LifebloomFrac / 100.0f;
            settings.RegrowthFraction = (float)profile.RegrowthFrac / 100.0f;
            settings.NourishFraction = (float)profile.NourishFrac / 100.0f;

            settings.nourish1 = (float)profile.Nourish1 / 100f;
            settings.nourish2 = (float)profile.Nourish2 / 100f;
            settings.nourish3 = (float)profile.Nourish3 / 100f;
            settings.nourish4 = (float)profile.Nourish4 / 100f;
            settings.nourish0 = 1.0f - (settings.nourish1 + settings.nourish2 + settings.nourish3 + settings.nourish4);

            settings.healTarget = HealTargetTypes.RaidHealing;

            settings.livingSeedEfficiency = (float)profile.LivingSeedEfficiency / 100f;

            return settings;
        }

        protected void CalculateTriggers(CombatFactors cf, Dictionary<Trigger, float> triggerIntervals, Dictionary<Trigger, float> triggerChances)
        {
            float CastInterval = 1f / cf.CastsPerSecond;
            float HealInterval = 1f / cf.HealsPerSecond;
            float CritsRatio = cf.CritsPerSecond / cf.CastsPerSecond;
            float RejuvInterval = 1f / cf.RejuvTicksPerSecond;
            
            triggerIntervals[Trigger.Use] = 0f;
            triggerChances[Trigger.Use] = 1f;

            triggerIntervals[Trigger.SpellCast] = CastInterval;
            triggerChances[Trigger.SpellCast] = 1f;

            triggerIntervals[Trigger.HealingSpellCast] = CastInterval;
            triggerChances[Trigger.HealingSpellCast] = 1f;

            triggerIntervals[Trigger.HealingSpellHit] = HealInterval;
            triggerChances[Trigger.HealingSpellHit] = 1f;

            triggerIntervals[Trigger.SpellCrit] = CastInterval;
            triggerChances[Trigger.SpellCrit] = CritsRatio;

            triggerIntervals[Trigger.HealingSpellCrit] = CastInterval;
            triggerChances[Trigger.HealingSpellCrit] = CritsRatio;

            triggerIntervals[Trigger.RejuvenationTick] = RejuvInterval;
            triggerChances[Trigger.RejuvenationTick] = 1f;
        }

        public enum ProcCalc
        {
            None,
            Average,
            Max
        };

        private void DoSpecialEffects(Character character, Stats stats, CombatFactors cf, float TotalTime, List<float> hasteRatings, List<float> hasteRatingUptimes, ProcCalc pc)
        {
            #region Initialize Triggers
            Dictionary<Trigger, float> triggerIntervals = new Dictionary<Trigger, float>(); ;
            Dictionary<Trigger, float> triggerChances = new Dictionary<Trigger, float>(); ;
            CalculateTriggers(cf, triggerIntervals, triggerChances);
            #endregion

            #region Haste Lists (seperately handled)
            List<SpecialEffect> tempHasteEffects = new List<SpecialEffect>();
            List<float> tempHasteEffectIntervals = new List<float>();
            List<float> tempHasteEffectChances = new List<float>();
            List<float> tempHasteEffectOffsets = new List<float>();
            List<float> tempHasteEffectScales = new List<float>();
            #endregion

            float offset = 0;

            List<SpecialEffect> effects = new List<SpecialEffect>();
            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                effect.Stats.GenerateSparseData();

                #region Filter out unhandled effects
                if (!triggerIntervals.ContainsKey(effect.Trigger)) continue;
                #endregion

                #region Filter out Haste effects
                if (effect.Stats.HasteRating > 0)
                {
                    tempHasteEffects.Add(effect);
                    // PATCH for use effects
                    if (effect.Trigger == Trigger.Use)
                    {
                        tempHasteEffectIntervals.Add(effect.Cooldown);
                        tempHasteEffectChances.Add(1f);
                        tempHasteEffectOffsets.Add(offset);
                        offset += effect.Duration;
                    }
                    else
                    {
                        tempHasteEffectIntervals.Add(triggerIntervals[effect.Trigger]);
                        tempHasteEffectChances.Add(triggerChances[effect.Trigger]);
                        tempHasteEffectOffsets.Add(0f);
                    }
                    tempHasteEffectScales.Add(1f);
                    continue;
                }
                #endregion

                effects.Add(effect);
            }

            #region Calculate Haste Breakdown
            if (tempHasteEffects.Count == 0 || pc == ProcCalc.None)
            {
                hasteRatings.Add(0.0f);
                hasteRatingUptimes.Add(1.0f);
            }
            else if (pc == ProcCalc.Max)
            {
                float hasteRating = 0;
                for (int i = 0; i < tempHasteEffects.Count; i++) hasteRating += tempHasteEffects[i].Stats.HasteRating;
                hasteRatings.Add(hasteRating);
                hasteRatingUptimes.Add(1f);
            }
            else if (tempHasteEffects.Count == 1)
            {   //Only one, add it to
                SpecialEffect effect = tempHasteEffects[0];
                float uptime = effect.GetAverageStackSize(tempHasteEffectIntervals[0], tempHasteEffectChances[0], 0, TotalTime);
                hasteRatings.Add(effect.Stats.HasteRating);
                hasteRatingUptimes.Add(uptime);
                hasteRatings.Add(0.0f);
                hasteRatingUptimes.Add(1.0f - uptime);
            }
            else if (tempHasteEffects.Count > 1)
            {
                WeightedStat[] HasteRatingWeights = SpecialEffect.GetAverageCombinedUptimeCombinations(tempHasteEffects.ToArray(), tempHasteEffectIntervals.ToArray(), tempHasteEffectChances.ToArray(), tempHasteEffectOffsets.ToArray(), tempHasteEffectScales.ToArray(), 0, TotalTime, AdditiveStat.HasteRating);
                for (int i = 0; i < HasteRatingWeights.Length; i++)
                {
                    hasteRatings.Add(HasteRatingWeights[i].Value);
                    hasteRatingUptimes.Add(HasteRatingWeights[i].Chance);
                }
            }
            #endregion

            /*
            #region Calculate average Haste from effects (capped)
            if (tempHasteRatings.Count > 0f)
            {
                float cap = (1.5f / (1f + stats.SpellHaste) - 1) * StatConversion.RATING_PER_SPELLHASTE - stats.HasteRating;

                float HasteRatingFromProcs = 0f;
                for (int i = 0; i < tempHasteRatings.Count; i++)
                {
                    float Haste = Math.Min(cap, tempHasteRatings[i]);
                    HasteRatingFromProcs += tempHasteRatingUptimes[i] * Haste;
                }

                stats.HasteRating += HasteRatingFromProcs;                
            }
            #endregion
            */

            AccumulateSpecialEffects(character, ref stats, TotalTime, triggerIntervals, triggerChances, effects, 1f, pc);

            #region Clear special effects from Stats
            for (int i = 0; i < stats._rawSpecialEffectDataSize; i++) stats._rawSpecialEffectData[i] = null;
            stats._rawSpecialEffectDataSize = 0;
            #endregion
        }

        protected void AccumulateSpecialEffects(Character character, ref Stats stats, float FightDuration, Dictionary<Trigger, float> triggerIntervals, Dictionary<Trigger, float> triggerChances, List<SpecialEffect> effects, float weight, ProcCalc pc) 
        {
            foreach (SpecialEffect effect in effects) 
            {
                Stats effectStats = effect.Stats;

                float upTime = 0f;

                #region Filter out Haste effects
                if (effect.Stats.HasteRating > 0f) continue;
                #endregion

                if (effect.Trigger == Trigger.Use)
                {
                    if (effect.Stats._rawSpecialEffectDataSize >= 1)
                    {
                        upTime = effect.GetAverageUptime(0f, 1f, 0, FightDuration);
                        List<SpecialEffect> nestedEffect = new List<SpecialEffect>(effect.Stats.SpecialEffects());
                        Stats _stats2 = effectStats.Clone();
                        AccumulateSpecialEffects(character, ref _stats2, effect.Duration, triggerIntervals, triggerChances, nestedEffect, upTime, pc);
                        effectStats = _stats2;
                    }
                    else
                    {
                        upTime = effect.GetAverageStackSize(0f, 1f, 0, FightDuration);
                    }
                }
                else if (effect.Duration == 0f)
                {
                    upTime = effect.GetAverageProcsPerSecond(triggerIntervals[effect.Trigger], triggerChances[effect.Trigger],
                                                             0, FightDuration);
                }
                else if (triggerIntervals.ContainsKey(effect.Trigger))
                {
                    upTime = effect.GetAverageStackSize(triggerIntervals[effect.Trigger], triggerChances[effect.Trigger],
                                                             0, FightDuration);
                }

                if (upTime > 0f)
                {
                    if (pc == ProcCalc.None && upTime < 0.5f)
                    {
                        Stats s = new Stats()
                        {
                            SpellPower = effectStats.SpellPower,
                            CritRating = effectStats.CritRating,
                            //HasteRating = effectStats.HasteRating,
                            SpellHaste = effectStats.SpellHaste
                        };
                        effectStats.SpellPower = 0;
                        effectStats.CritRating = 0;
                        //effectStats.HasteRating = 0;
                        effectStats.SpellHaste = 0;
                        stats.Accumulate(effectStats, upTime * weight);
                        effectStats.SpellPower = s.SpellPower;
                        effectStats.CritRating = s.CritRating;
                        //effectStats.HasteRating = s.HasteRating;
                        effectStats.SpellHaste = s.SpellHaste;
                    }
                    else if (pc == ProcCalc.Max && upTime < 0.5f)
                    {
                        Stats s = new Stats()
                        {
                            SpellPower = effectStats.SpellPower,
                            CritRating = effectStats.CritRating,
                            //HasteRating = effectStats.HasteRating,
                            SpellHaste = effectStats.SpellHaste
                        };
                        effectStats.SpellPower = 0;
                        effectStats.CritRating = 0;
                        //effectStats.HasteRating = 0;
                        effectStats.SpellHaste = 0;
                        stats.Accumulate(effectStats, upTime * weight);
                        effectStats.SpellPower = s.SpellPower;
                        effectStats.CritRating = s.CritRating;
                        //effectStats.HasteRating = s.HasteRating;
                        effectStats.SpellHaste = s.SpellHaste;
                        stats.Accumulate(s, weight);
                    }
                    else
                    {
                        stats.Accumulate(effectStats, upTime * weight);
                    }
                }
            }
        }

        public override CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, bool referenceCalculation, bool significantChange, bool needsDisplayCalculations)
        {
            CalculationOptionsTree calcOpts = (CalculationOptionsTree)character.CalculationOptions;
            SpellProfile profile = calcOpts.Current;

            CharacterCalculationsTree calculationResult = new CharacterCalculationsTree();
            calculationResult.LocalCharacter = character;
            calculationResult.BasicStats = GetCharacterStats(character, additionalItem);

            #region Rotations
            Stats stats = calculationResult.BasicStats;
            stats.ManaRestore /= profile.FightDuration;
            float replenish = stats.ManaRestoreFromMaxManaPerSecond >= 0.002f ? 0.002f : 0;
            stats.ManaRestore += (stats.ManaRestoreFromMaxManaPerSecond - replenish) * stats.Mana;
            stats.ManaRestoreFromMaxManaPerSecond = replenish;
            
            float ExtraHealing = 0f;
            RotationSettings settings = getRotationFromCalculationOptions(stats, calcOpts, calculationResult);

            // Initial run
            SustainedResult rot = Solver.SimulateHealing(calculationResult, stats, calcOpts, settings);

            List<float> hasteRatings = new List<float>();
            List<float> hasteRatingUptimes = new List<float>();

            ProcCalc pc = ProcCalc.None;
            switch (calcOpts.ProcType)
            {
                case 0:
                    pc = ProcCalc.None;
                    break;
                case 1:
                    pc = ProcCalc.Average;
                    break;
                case 2:
                    pc = ProcCalc.Max;
                    break;
            }

            CombatFactors cfs = rot.getCombatFactors();
            cfs.Compute();

            for (int k = 0; k < 2; k++) {
                // Create new stats instance with procs
                stats = GetCharacterStats(character, additionalItem);
                stats.ManaRestore /= profile.FightDuration;

                hasteRatings.Clear();
                hasteRatingUptimes.Clear();
                DoSpecialEffects(character, stats, cfs, rot.TotalTime, hasteRatings, hasteRatingUptimes, pc);

                // Add replenish
                replenish = stats.ManaRestoreFromMaxManaPerSecond >= 0.002f ? 0.002f : 0;
                stats.ManaRestore += (stats.ManaRestoreFromMaxManaPerSecond - replenish) * stats.Mana;
                stats.ManaRestoreFromMaxManaPerSecond = replenish;

                ExtraHealing = stats.Healed;

                if (false)//calcOpts.IgnoreAllHasteEffects)
                {
                    /*stats.SpellHaste = calculationResult.BasicStats.SpellHaste;
                    stats.HasteRating = calculationResult.BasicStats.HasteRating;

                    rot = Solver.SimulateHealing(calculationResult, stats, calcOpts, settings);
                    cfs = rot.getCombatFactors();
                    cfs.Compute();*/
                }
                else
                {
                    cfs = new CombatFactors();
                    float AverageHasteRating = 0;
                    for (int i = 0; i < hasteRatings.Count; i++)
                    {
                        stats.HasteRating += hasteRatings[i];
                        SustainedResult r = Solver.SimulateHealing(calculationResult, stats, calcOpts, settings);
                        cfs.Accumulate(r.getCombatFactors(), hasteRatingUptimes[i]);
                        stats.HasteRating -= hasteRatings[i];
                        AverageHasteRating += hasteRatings[i] * hasteRatingUptimes[i];
                        if (i == 0) rot = r;
                    }
                    cfs.Compute();
                    stats.HasteRating += AverageHasteRating; // For Burst
                }
            }
            calculationResult.Sustained = rot;
            calculationResult.CombatStats = stats;
            #endregion

            calculationResult.SingleTarget = Solver.CalculateSingleTargetBurst(calculationResult, stats, calcOpts, Solver.SingleTargetIndexToRotation(calcOpts.SingleTargetRotation));

            calculationResult.SingleTargetHPS = calculationResult.SingleTarget[0].HPS;
            calculationResult.SustainedHPS = cfs.TotalHealing / rot.TotalTime + ExtraHealing;

            #region Survival Points
            float DamageReduction = StatConversion.GetArmorDamageReduction(83, stats.Armor, 0, 0, 0);
            calculationResult.SurvivalPoints = stats.Health / (1f - DamageReduction) / 100f * calcOpts.SurvValuePer100;
            #endregion

            // Apply diminishing returns

            calculationResult.SingleTargetPoints = DiminishingReturns.CapWithMaximum2(calculationResult.SingleTargetHPS, calcOpts.SingleTarget);
            calculationResult.SustainedPoints = DiminishingReturns.CapWithMaximum2(calculationResult.SustainedHPS, calcOpts.SustainedTarget);

            calculationResult.OverallPoints = calculationResult.SingleTargetPoints + calculationResult.SustainedPoints + calculationResult.SurvivalPoints;

            return calculationResult;
        }
        private static readonly SpecialEffect[] _SE_NaturesGrace = new SpecialEffect[] {
            null,
            new SpecialEffect(Trigger.HealingSpellCrit, new Stats() { SpellHaste = 0.2f }, 3f, 0, 1 * 1f / 3f, 1),
            new SpecialEffect(Trigger.HealingSpellCrit, new Stats() { SpellHaste = 0.2f }, 3f, 0, 2 * 1f / 3f, 1),
            new SpecialEffect(Trigger.HealingSpellCrit, new Stats() { SpellHaste = 0.2f }, 3f, 0, 3 * 1f / 3f, 1),
        };
        public override Stats GetCharacterStats(Character character, Item additionalItem) { return GetCharacterStats(character, additionalItem, new Stats()); }
        public Stats GetCharacterStats(Character character, Item additionalItem, Stats statsProcs) {
            CalculationOptionsTree calcOpts = character.CalculationOptions as CalculationOptionsTree;
            DruidTalents talents = character.DruidTalents;
            SpellProfile profile = calcOpts.Current;

            Stats statsRace = BaseStats.GetBaseStats(character.Level, character.Class, character.Race); //GetRacialBaseStats(character.Race);
            TreeConstants.BaseMana = statsRace.Mana; // Setup TreeConstant

            Stats statsTalents = new Stats() {
                BonusAgilityMultiplier   = (1f + 0.01f * talents.SurvivalOfTheFittest * 2f) * (1f + 0.01f * talents.ImprovedMarkOfTheWild) - 1f,
                BonusIntellectMultiplier = (1f + 0.01f * talents.SurvivalOfTheFittest * 2f) * (1f + 0.01f * talents.ImprovedMarkOfTheWild) * (1f + 0.04f * talents.HeartOfTheWild) - 1f,
                BonusSpiritMultiplier    = (1f + 0.01f * talents.SurvivalOfTheFittest * 2f) * (1f + 0.01f * talents.ImprovedMarkOfTheWild) * (1f + 0.05f * talents.LivingSpirit) - 1f,
                BonusStaminaMultiplier   = (1f + 0.01f * talents.SurvivalOfTheFittest * 2f) * (1f + 0.01f * talents.ImprovedMarkOfTheWild) - 1f,
                BonusStrengthMultiplier  = (1f + 0.01f * talents.SurvivalOfTheFittest * 2f) * (1f + 0.01f * talents.ImprovedMarkOfTheWild) - 1f,
                BonusArmorMultiplier     = 0.80f * talents.ImprovedTreeOfLife,
                SpellHaste               = ((1f + 0.01f * talents.CelestialFocus) * (1f + 0.02f * talents.GiftOfTheEarthmother)) - 1f,
                SpellDamageFromSpiritPercentage = talents.ImprovedTreeOfLife * 0.05f,
                SpellCombatManaRegeneration = talents.Intensity * 0.5f / 3f,
                RevitalizeChance         = talents.Revitalize * 0.05f,
            };

            if (!calcOpts.IgnoreNaturesGrace && talents.NaturesGrace > 0) {
                statsTalents.AddSpecialEffect(_SE_NaturesGrace[talents.NaturesGrace]);
            }

            Stats statsBaseGear = GetItemStats(character, additionalItem);
            Stats statsBuffs = GetBuffsStats(character, calcOpts);
            Stats statsTotal = statsBaseGear + statsBuffs + statsRace + statsTalents + statsProcs;

            statsTotal.Agility   = (float)Math.Floor(statsTotal.Agility   * (1f + statsTotal.BonusAgilityMultiplier));
            statsTotal.Stamina   = (float)Math.Floor(statsTotal.Stamina   * (1f + statsTotal.BonusStaminaMultiplier));
            statsTotal.Intellect = (float)Math.Floor(statsTotal.Intellect * (1f + statsTotal.BonusIntellectMultiplier));
            statsTotal.Spirit    = (float)Math.Floor(statsTotal.Spirit    * (1f + statsTotal.BonusSpiritMultiplier));
            statsTotal.Armor     = (float)Math.Floor(statsTotal.Armor     * (1f + statsTotal.BonusArmorMultiplier));

            if (statsTotal.HighestStat > 0) {
                if (statsTotal.Spirit > statsTotal.Intellect) {
                    statsTotal.Spirit += (float)Math.Floor(statsTotal.HighestStat * (1f + statsTotal.BonusSpiritMultiplier));
                } else {
                    statsTotal.Intellect += (float)Math.Floor(statsTotal.HighestStat * (1f + statsTotal.BonusIntellectMultiplier));
                }
            }

            // Add spellpower from spirit, intellect and... agility :)
            statsTotal.SpellPower = (float)Math.Round( statsTotal.SpellPower
                                                    + (statsTotal.SpellDamageFromSpiritPercentage * statsTotal.Spirit)
                                                    + (statsTotal.Intellect * talents.LunarGuidance * 0.04)
                                                    + (talents.NurturingInstinct * 0.35f * statsTotal.Agility));

            statsTotal.Mana = statsTotal.Mana + StatConversion.GetManaFromIntellect(statsTotal.Intellect);
            statsTotal.Mana *= (1f + statsTotal.BonusManaMultiplier);

            statsTotal.Health = (float)Math.Round(statsTotal.Health + StatConversion.GetHealthFromStamina(statsTotal.Stamina));
            statsTotal.Mp5   += (float)Math.Floor(statsTotal.Intellect * (talents.Dreamstate > 0 ? talents.Dreamstate * 0.03f + 0.01f : 0f));

            statsTotal.SpellCrit = (float)Math.Round((StatConversion.GetSpellCritFromIntellect(statsTotal.Intellect)
                                                    + StatConversion.GetSpellCritFromRating(statsTotal.CritRating)
                                                    + (statsTotal.SpellCrit)
                                                    + 0.01f * talents.NaturalPerfection) * 100f, 2);

            return statsTotal;
        }
        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
        {
            List<ComparisonCalculationBase> comparisonList = new List<ComparisonCalculationBase>();

            CalculationOptionsTree calculationOptions = character.CalculationOptions as CalculationOptionsTree;
            DruidTalents talents = character.DruidTalents;
            CharacterCalculationsTree calculationResult = GetCharacterCalculations(character) as CharacterCalculationsTree;

            switch (chartName)
            {
                #region Mana sources (sustained)
                case "Mana sources (sustained)":
                    _subPointNameColors = _subPointNameColorsMPS;
                    ComparisonCalculationTree gear = new ComparisonCalculationTree()
                    {
                        Name = "MP5 from gear, buffs and MP5 procs",
                        Equipped = false,
                        OverallPoints = calculationResult.Sustained.GearMPS,
                        SubPoints = new float[] { calculationResult.Sustained.GearMPS }
                    };
                    comparisonList.Add(gear);
                    ComparisonCalculationTree procs = new ComparisonCalculationTree()
                    {
                        Name = "Mana from procs",
                        Equipped = false,
                        OverallPoints = calculationResult.Sustained.ProcsMPS,
                        SubPoints = new float[] { calculationResult.Sustained.ProcsMPS }
                    };
                    comparisonList.Add(procs);
                    ComparisonCalculationTree revitalize = new ComparisonCalculationTree()
                    {
                        Name = "Mana from Revitalize",
                        Equipped = false,
                        OverallPoints = calculationResult.Sustained.RevitalizeMPS,
                        SubPoints = new float[] { calculationResult.Sustained.RevitalizeMPS }
                    };
                    comparisonList.Add(revitalize);
                    ComparisonCalculationTree spiritIC = new ComparisonCalculationTree()
                    {
                        Name = "Spirit",
                        Equipped = false,
                        OverallPoints = (1f - calculationResult.Sustained.OutOfCombatFraction) * calculationResult.Sustained.SpiritMPS * calculationResult.Sustained.SpiritInCombatFraction,
                        SubPoints = new float[] { (1f - calculationResult.Sustained.OutOfCombatFraction) * calculationResult.Sustained.SpiritMPS * calculationResult.Sustained.SpiritInCombatFraction }
                    };
                    comparisonList.Add(spiritIC);
                    ComparisonCalculationTree replenishment = new ComparisonCalculationTree()
                    {
                        Name = "Replenishment",
                        Equipped = false,
                        OverallPoints = calculationResult.Sustained.ReplenishmentMPS,
                        SubPoints = new float[] { calculationResult.Sustained.ReplenishmentMPS }
                    };
                    comparisonList.Add(replenishment);
                    ComparisonCalculationTree innervates = new ComparisonCalculationTree()
                    {
                        Name = "Innervates",
                        Equipped = false,
                        OverallPoints = calculationResult.Sustained.InnervateMPS,
                        SubPoints = new float[] { calculationResult.Sustained.InnervateMPS }
                    };
                    comparisonList.Add(innervates);
                    
                    return comparisonList.ToArray();
                #endregion
                #region Casting time percentage per spell (sustained)
                case "Casting time percentage per spell (sustained)":
                    {
                        _subPointNameColors = _subPointNameColorsCF;
                        ComparisonCalculationTree rejuv = new ComparisonCalculationTree()
                        {
                            Name = "Rejuvenation",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.RejuvCF_unreduced * 100,
                            SubPoints = new float[] { 
                                calculationResult.Sustained.spellMix.RejuvCF * 100,
                                (calculationResult.Sustained.RejuvCF_unreducedOOM - calculationResult.Sustained.spellMix.RejuvCF) * 100f,
                                (calculationResult.Sustained.RejuvCF_unreduced - calculationResult.Sustained.RejuvCF_unreducedOOM) * 100
                            }
                        };
                        comparisonList.Add(rejuv);
                        ComparisonCalculationTree regrowth = new ComparisonCalculationTree()
                        {
                            Name = "Regrowth",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.RegrowthCF_unreduced * 100,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.RegrowthCF * 100,
                            (calculationResult.Sustained.RegrowthCF_unreducedOOM - calculationResult.Sustained.spellMix.RegrowthCF) * 100f,
                            (calculationResult.Sustained.RegrowthCF_unreduced - calculationResult.Sustained.RegrowthCF_unreducedOOM) * 100}
                        };
                        comparisonList.Add(regrowth);
                        ComparisonCalculationTree managedrejuv = new ComparisonCalculationTree()
                        {
                            Name = "Maintained Rejuvenation",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.ManagedRejuvCF_unreduced * 100,
                            SubPoints = new float[] { 
                                calculationResult.Sustained.spellMix.ManagedRejuvCF * 100,
                                (calculationResult.Sustained.ManagedRejuvCF_unreducedOOM - calculationResult.Sustained.spellMix.ManagedRejuvCF) * 100f,
                                (calculationResult.Sustained.ManagedRejuvCF_unreduced - calculationResult.Sustained.ManagedRejuvCF_unreducedOOM) * 100
                            }
                        };
                        comparisonList.Add(managedrejuv);
                        ComparisonCalculationTree managedregrowth = new ComparisonCalculationTree()
                        {
                            Name = "Maintained Regrowth",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.ManagedRegrowthCF_unreduced * 100,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.ManagedRegrowthCF * 100,
                            (calculationResult.Sustained.ManagedRegrowthCF_unreducedOOM - calculationResult.Sustained.spellMix.ManagedRegrowthCF) * 100f,
                            (calculationResult.Sustained.ManagedRegrowthCF_unreduced - calculationResult.Sustained.ManagedRegrowthCF_unreducedOOM) * 100}
                        };
                        comparisonList.Add(managedregrowth);
                        ComparisonCalculationTree lifebloom = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.LifebloomCF_unreduced * 100,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.LifebloomCF * 100,
                                (calculationResult.Sustained.LifebloomCF_unreducedOOM - calculationResult.Sustained.spellMix.LifebloomCF) * 100f,
                            (calculationResult.Sustained.LifebloomCF_unreduced - calculationResult.Sustained.LifebloomCF_unreducedOOM) * 100 }
                        };
                        comparisonList.Add(lifebloom);
                        ComparisonCalculationTree lifebloomStack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.LifebloomStackCF_unreduced * 100,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.LifebloomStackCF * 100, 0,
                            (calculationResult.Sustained.LifebloomStackCF_unreduced - calculationResult.Sustained.spellMix.LifebloomStackCF) * 100 }
                        };
                        comparisonList.Add(lifebloomStack);
                        ComparisonCalculationTree wildGrowth = new ComparisonCalculationTree()
                        {
                            Name = "Wild Growth",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.WildGrowthCF_unreduced * 100,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.WildGrowthCF * 100,
                                (calculationResult.Sustained.WildGrowthCF_unreducedOOM - calculationResult.Sustained.spellMix.WildGrowthCF) * 100f,
                            (calculationResult.Sustained.WildGrowthCF_unreduced - calculationResult.Sustained.WildGrowthCF_unreducedOOM) * 100 }
                        };
                        comparisonList.Add(wildGrowth);
                        ComparisonCalculationTree swiftmend = new ComparisonCalculationTree()
                        {
                            Name = "Swiftmend",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.SwiftmendCF_unreduced * 100,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.SwiftmendCF * 100, 0,
                            (calculationResult.Sustained.SwiftmendCF_unreduced - calculationResult.Sustained.spellMix.SwiftmendCF) * 100 }
                        };
                        comparisonList.Add(swiftmend);
                        ComparisonCalculationTree nourish = new ComparisonCalculationTree()
                        {
                            Name = "Nourish",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.NourishCF_unreduced * 100,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.NourishCF * 100,
                                (calculationResult.Sustained.NourishCF_unreducedOOM - calculationResult.Sustained.spellMix.NourishCF) * 100f,
                            (calculationResult.Sustained.NourishCF_unreduced - calculationResult.Sustained.NourishCF_unreducedOOM) * 100 }
                        };
                        comparisonList.Add(nourish);
                        /*ComparisonCalculationTree idle = new ComparisonCalculationTree()
                        {
                            Name = "Idle",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.IdleCF * 100,
                            SubPoints = new float[] { calculationResult.Sustained.IdleCF_unreduced * 100f,
                                (calculationResult.Sustained.spellMix.IdleCF - calculationResult.Sustained.IdleCF_unreducedOOM) * 100f,
                            (calculationResult.Sustained.IdleCF_unreducedOOM - calculationResult.Sustained.IdleCF_unreduced) * 100f }
                        };
                        comparisonList.Add(idle);*/
                    }
                    return comparisonList.ToArray();
                #endregion
                #region Mana usage per spell (sustained)
                case "Mana usage per spell (sustained)":
                    {
                        _subPointNameColors = _subPointNameColorsMPS;
                        ComparisonCalculationTree rejuv = new ComparisonCalculationTree()
                        {
                            Name = "Rejuvenation",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.RejuvMPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.RejuvMPS }
                        };
                        comparisonList.Add(rejuv);
                        ComparisonCalculationTree regrowth = new ComparisonCalculationTree()
                        {
                            Name = "Regrowth",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.RegrowthMPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.RegrowthMPS }
                        };
                        comparisonList.Add(regrowth);
                        ComparisonCalculationTree managedrejuv = new ComparisonCalculationTree()
                        {
                            Name = "Maintained Rejuvenation",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.ManagedRejuvMPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.ManagedRejuvMPS }
                        };
                        comparisonList.Add(managedrejuv);
                        ComparisonCalculationTree managedregrowth = new ComparisonCalculationTree()
                        {
                            Name = "Maintained Regrowth",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.ManagedRegrowthMPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.ManagedRegrowthMPS }
                        };
                        comparisonList.Add(managedregrowth);
                        ComparisonCalculationTree lifebloom = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.LifebloomMPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.LifebloomMPS }
                        };
                        comparisonList.Add(lifebloom);
                        ComparisonCalculationTree lifebloomStack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.LifebloomStackMPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.LifebloomStackMPS }
                        };
                        comparisonList.Add(lifebloomStack);
                        ComparisonCalculationTree wildGrowth = new ComparisonCalculationTree()
                        {
                            Name = "Wild Growth",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.WildGrowthMPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.WildGrowthMPS }
                        };
                        comparisonList.Add(wildGrowth);
                        ComparisonCalculationTree swiftmend = new ComparisonCalculationTree()
                        {
                            Name = "Swiftmend",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.SwiftmendMPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.SwiftmendMPS }
                        };
                        comparisonList.Add(swiftmend);
                        ComparisonCalculationTree primary = new ComparisonCalculationTree()
                        {
                            Name = "Nourish",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.NourishMPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.NourishMPS }
                        };
                        comparisonList.Add(primary);
                    }
                    return comparisonList.ToArray();
                #endregion
                #region Healing per spell (sustained)
                case "Total HPS per spell (sustained)":
                    {
                        _subPointNameColors = _subPointNameColorsHPS;
                        ComparisonCalculationTree rejuv = new ComparisonCalculationTree()
                        {
                            Name = "Rejuvenation",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.RejuvHPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.RejuvHPS }
                        };
                        comparisonList.Add(rejuv);
                        ComparisonCalculationTree regrowth = new ComparisonCalculationTree()
                        {
                            Name = "Regrowth",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.RegrowthHPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.RegrowthHPS }
                        };
                        comparisonList.Add(regrowth);
                        ComparisonCalculationTree managedrejuv = new ComparisonCalculationTree()
                        {
                            Name = "Maintained Rejuvenation",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.ManagedRejuvHPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.ManagedRejuvHPS }
                        };
                        comparisonList.Add(managedrejuv);
                        ComparisonCalculationTree managedregrowth = new ComparisonCalculationTree()
                        {
                            Name = "Maintained Regrowth",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.ManagedRegrowthHPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.ManagedRegrowthHPS }
                        };
                        comparisonList.Add(managedregrowth);
                        ComparisonCalculationTree regrowthDH = new ComparisonCalculationTree()
                        { // RegrowthAvg * regrowth.HPSHoT + RegrowthCPS * regrowth.AverageHealingwithCrit
                            Name = "Regrowth (DH)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.RegrowthCPS * calculationResult.Sustained.spellMix.RegrowthSpell.AverageHealingwithCrit,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.RegrowthCPS * calculationResult.Sustained.spellMix.RegrowthSpell.AverageHealingwithCrit }
                        };
                        comparisonList.Add(regrowthDH);
                        ComparisonCalculationTree regrowthHoT = new ComparisonCalculationTree()
                        {
                            Name = "Regrowth (HoT)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.RegrowthAvg * calculationResult.Sustained.spellMix.RegrowthSpell.HPS_HOT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.RegrowthAvg * calculationResult.Sustained.spellMix.RegrowthSpell.HPS_HOT }
                        };
                        comparisonList.Add(regrowthHoT);
                        ComparisonCalculationTree managedregrowthDH = new ComparisonCalculationTree()
                        { // RegrowthAvg * regrowth.HPSHoT + RegrowthCPS * regrowth.AverageHealingwithCrit
                            Name = "Maintained Regrowth (DH)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.ManagedRegrowthCPS * calculationResult.Sustained.spellMix.ManagedRegrowthSpell.AverageHealingwithCrit,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.ManagedRegrowthCPS * calculationResult.Sustained.spellMix.ManagedRegrowthSpell.AverageHealingwithCrit }
                        };
                        comparisonList.Add(managedregrowthDH);
                        ComparisonCalculationTree managedregrowthHoT = new ComparisonCalculationTree()
                        {
                            Name = "Maintained Regrowth (HoT)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.ManagedRegrowthAvg * calculationResult.Sustained.spellMix.ManagedRegrowthSpell.HPS_HOT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.ManagedRegrowthAvg * calculationResult.Sustained.spellMix.ManagedRegrowthSpell.HPS_HOT }
                        };
                        comparisonList.Add(managedregrowthHoT);
                        ComparisonCalculationTree lifebloom = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.LifebloomHPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.LifebloomHPS }
                        };
                        comparisonList.Add(lifebloom);
                        ComparisonCalculationTree lifebloomDH = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom (Bloom)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.LifebloomCPS * calculationResult.Sustained.spellMix.lifebloom.AverageHealingwithCrit,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.LifebloomCPS * calculationResult.Sustained.spellMix.lifebloom.AverageHealingwithCrit }
                        };
                        comparisonList.Add(lifebloomDH);
                        ComparisonCalculationTree lifebloomHoT = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom (HoT)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.LifebloomAvg * calculationResult.Sustained.spellMix.lifebloom.HPS_HOT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.LifebloomAvg * calculationResult.Sustained.spellMix.lifebloom.HPS_HOT }
                        };
                        comparisonList.Add(lifebloomHoT);
                        ComparisonCalculationTree lifebloomStack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.LifebloomStackHPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.LifebloomStackHPS }
                        };
                        comparisonList.Add(lifebloomStack);
                        ComparisonCalculationTree lifebloomStackDH = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Stack (Bloom)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.LifebloomStackHPS_DH,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.LifebloomStackHPS_DH }
                        };
                        comparisonList.Add(lifebloomStackDH);
                        ComparisonCalculationTree lifebloomStackHOT = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Stack (HoT)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.LifebloomStackHPS_HOT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.LifebloomStackHPS_HOT }
                        };
                        comparisonList.Add(lifebloomStackHOT);
                        ComparisonCalculationTree wildGrowth = new ComparisonCalculationTree()
                        {
                            Name = "Wild Growth",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.WildGrowthHPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.WildGrowthHPS }
                        };
                        comparisonList.Add(wildGrowth);
                        ComparisonCalculationTree wildGrowthSingle = new ComparisonCalculationTree()
                        {
                            Name = "Wild Growth (single target)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.WildGrowthHPS / calculationResult.Sustained.spellMix.wildGrowth.maxTargets,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.WildGrowthHPS / calculationResult.Sustained.spellMix.wildGrowth.maxTargets }
                        };
                        comparisonList.Add(wildGrowthSingle);
                        ComparisonCalculationTree swiftmend = new ComparisonCalculationTree()
                        {
                            Name = "Swiftmend",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.SwiftmendHPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.SwiftmendHPS }
                        };
                        comparisonList.Add(swiftmend);
                        ComparisonCalculationTree primary = new ComparisonCalculationTree()
                        {
                            Name = "Nourish",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.NourishHPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.NourishHPS }
                        };
                        comparisonList.Add(primary);
                    }
                    return comparisonList.ToArray();
                #endregion
                #region HPCT per spell
                case "HPCT per spell":
                    {
                        _subPointNameColors = _subPointNameColorsHPCT;
                        ComparisonCalculationTree rejuv = new ComparisonCalculationTree()
                        {
                            Name = "Rejuvenation",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.rejuvenate.HPCT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.rejuvenate.HPCT }
                        };
                        comparisonList.Add(rejuv);
                        ComparisonCalculationTree regrowth = new ComparisonCalculationTree()
                        {
                            Name = "Regrowth",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.regrowth.HPCT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.regrowth.HPCT }
                        };
                        comparisonList.Add(regrowth);
                        ComparisonCalculationTree regrowthAgain = new ComparisonCalculationTree()
                        {
                            Name = "Regrowth (chaincast)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.regrowthAgain.HPCT_DH,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.regrowthAgain.HPCT_DH }
                        };
                        comparisonList.Add(regrowthAgain);
                        ComparisonCalculationTree lifebloom = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloom.HPCT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloom.HPCT }
                        };
                        comparisonList.Add(lifebloom);
                        ComparisonCalculationTree lifebloomRollingStack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Rolling Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloomRollingStack.HPCT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloomRollingStack.HPCT }
                        };
                        comparisonList.Add(lifebloomRollingStack);
                        ComparisonCalculationTree lifebloomSlowStack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Slow Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloomSlowStack.HPCT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloomSlowStack.HPCT }
                        };
                        comparisonList.Add(lifebloomSlowStack);
                        ComparisonCalculationTree lifebloomFastStack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Fast Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloomFastStack.HPCT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloomFastStack.HPCT }
                        };
                        comparisonList.Add(lifebloomFastStack);
                        ComparisonCalculationTree lifebloomSlow2Stack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Slow x2 Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloomSlow2Stack.HPCT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloomSlow2Stack.HPCT }
                        };
                        comparisonList.Add(lifebloomSlow2Stack);
                        ComparisonCalculationTree lifebloomFast2Stack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Fast x2 Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloomFast2Stack.HPCT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloomFast2Stack.HPCT }
                        };
                        comparisonList.Add(lifebloomFast2Stack);
                        ComparisonCalculationTree wildGrowth = new ComparisonCalculationTree()
                        {
                            Name = "Wild Growth",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.wildGrowth.HPCT * calculationResult.Sustained.spellMix.wildGrowth.maxTargets,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.wildGrowth.HPCT * calculationResult.Sustained.spellMix.wildGrowth.maxTargets }
                        };
                        comparisonList.Add(wildGrowth);
                        ComparisonCalculationTree wildGrowthSingle = new ComparisonCalculationTree()
                        {
                            Name = "Wild Growth (single target)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.wildGrowth.HPCT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.wildGrowth.HPCT }
                        };
                        comparisonList.Add(wildGrowthSingle);
                        if (calculationResult.Sustained.spellMix.swiftmend != null)
                        {
                            ComparisonCalculationTree swiftmend = new ComparisonCalculationTree()
                            {
                                Name = "Swiftmend",
                                Equipped = false,
                                OverallPoints = calculationResult.Sustained.spellMix.swiftmend.HPCT,
                                SubPoints = new float[] { calculationResult.Sustained.spellMix.swiftmend.HPCT }
                            };
                            comparisonList.Add(swiftmend);
                        }
                        ComparisonCalculationTree nourish0 = new ComparisonCalculationTree()
                        {
                            Name = "Nourish (0)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.nourish[0].HPCT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.nourish[0].HPCT }
                        };
                        comparisonList.Add(nourish0);
                        ComparisonCalculationTree nourish1 = new ComparisonCalculationTree()
                        {
                            Name = "Nourish (1)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.nourish[1].HPCT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.nourish[1].HPCT }
                        };
                        comparisonList.Add(nourish1);
                        ComparisonCalculationTree nourish2 = new ComparisonCalculationTree()
                        {
                            Name = "Nourish (2)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.nourish[2].HPCT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.nourish[2].HPCT }
                        };
                        comparisonList.Add(nourish2);
                        ComparisonCalculationTree nourish3 = new ComparisonCalculationTree()
                        {
                            Name = "Nourish (3)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.nourish[3].HPCT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.nourish[3].HPCT }
                        };
                        comparisonList.Add(nourish3);
                        ComparisonCalculationTree nourish4 = new ComparisonCalculationTree()
                        {
                            Name = "Nourish (4)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.nourish[4].HPCT,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.nourish[4].HPCT }
                        };
                        comparisonList.Add(nourish4);
                    }
                    return comparisonList.ToArray();
                #endregion
                #region HPS per spell
                case "HPS per spell":
                    {
                        // YES, we use HPCTD here. That's TotalAverageHealing / max(CastTime, Duration)
                        // with CastTime, that means chaincasting a DH spell (Nourish/HT/SM)
                        // with Duration, that means refreshing a spell with a HoT component (RG/RJ/LB/WG)
                        _subPointNameColors = _subPointNameColorsHPS;
                        ComparisonCalculationTree rejuv = new ComparisonCalculationTree()
                        {
                            Name = "Rejuvenation",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.rejuvenate.HPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.rejuvenate.HPS }
                        };
                        comparisonList.Add(rejuv);
                        ComparisonCalculationTree regrowth = new ComparisonCalculationTree()
                        {
                            Name = "Regrowth",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.regrowth.HPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.regrowth.HPS }
                        };
                        comparisonList.Add(regrowth);
                        ComparisonCalculationTree regrowthAgain = new ComparisonCalculationTree()
                        {
                            Name = "Regrowth (chaincasting)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.regrowthAgain.HPCT_DH,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.regrowthAgain.HPCT_DH }
                        };
                        comparisonList.Add(regrowthAgain);
                        ComparisonCalculationTree lifebloom = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloom.HPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloom.HPS }
                        };
                        comparisonList.Add(lifebloom);
                        ComparisonCalculationTree lifebloomRollingStack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Rolling Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloomRollingStack.HPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloomRollingStack.HPS }
                        };
                        comparisonList.Add(lifebloomRollingStack);
                        ComparisonCalculationTree lifebloomSlowStack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Slow Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloomSlowStack.HPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloomSlowStack.HPS }
                        };
                        comparisonList.Add(lifebloomSlowStack);
                        ComparisonCalculationTree lifebloomFastStack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Fast Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloomFastStack.HPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloomFastStack.HPS }
                        };
                        comparisonList.Add(lifebloomFastStack);
                        ComparisonCalculationTree lifebloomSlow2Stack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Slow x2 Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloomSlow2Stack.HPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloomSlow2Stack.HPS }
                        };
                        comparisonList.Add(lifebloomSlow2Stack);
                        ComparisonCalculationTree lifebloomFast2Stack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Fast x2 Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloomFast2Stack.HPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloomFast2Stack.HPS }
                        };
                        comparisonList.Add(lifebloomFast2Stack);
                        ComparisonCalculationTree wildGrowth = new ComparisonCalculationTree()
                        {
                            Name = "Wild Growth",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.wildGrowth.HPS * calculationResult.Sustained.spellMix.wildGrowth.maxTargets,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.wildGrowth.HPS * calculationResult.Sustained.spellMix.wildGrowth.maxTargets }
                        };
                        comparisonList.Add(wildGrowth);
                        ComparisonCalculationTree wildGrowthSingle = new ComparisonCalculationTree()
                        {
                            Name = "Wild Growth (single target)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.wildGrowth.HPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.wildGrowth.HPS }
                        };
                        comparisonList.Add(wildGrowthSingle);
                        if (calculationResult.Sustained.spellMix.swiftmend != null)
                        {
                            ComparisonCalculationTree swiftmend = new ComparisonCalculationTree()
                            {
                                Name = "Swiftmend",
                                Equipped = false,
                                OverallPoints = calculationResult.Sustained.spellMix.swiftmend.HPS,
                                SubPoints = new float[] { calculationResult.Sustained.spellMix.swiftmend.HPS }
                            };
                            comparisonList.Add(swiftmend);
                        }
                        ComparisonCalculationTree nourish0 = new ComparisonCalculationTree()
                        {
                            Name = "Nourish (0)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.nourish[0].HPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.nourish[0].HPS }
                        };
                        comparisonList.Add(nourish0);
                        ComparisonCalculationTree nourish1 = new ComparisonCalculationTree()
                        {
                            Name = "Nourish (1)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.nourish[1].HPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.nourish[1].HPS }
                        };
                        comparisonList.Add(nourish1);
                        ComparisonCalculationTree nourish2 = new ComparisonCalculationTree()
                        {
                            Name = "Nourish (2)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.nourish[2].HPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.nourish[2].HPS }
                        };
                        comparisonList.Add(nourish2);
                        ComparisonCalculationTree nourish3 = new ComparisonCalculationTree()
                        {
                            Name = "Nourish (3)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.nourish[3].HPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.nourish[3].HPS }
                        };
                        comparisonList.Add(nourish3);
                        ComparisonCalculationTree nourish4 = new ComparisonCalculationTree()
                        {
                            Name = "Nourish (4)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.nourish[4].HPS,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.nourish[4].HPS }
                        };
                        comparisonList.Add(nourish4);
                    }
                    return comparisonList.ToArray();
#endregion
                #region HPM per spell
                case "HPM per spell":
                    {
                        _subPointNameColors = _subPointNameColorsHPM;
                        ComparisonCalculationTree rejuv = new ComparisonCalculationTree()
                        {
                            Name = "Rejuvenation",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.rejuvenate.HPM,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.rejuvenate.HPM }
                        };
                        comparisonList.Add(rejuv);
                        ComparisonCalculationTree regrowth = new ComparisonCalculationTree()
                        {
                            Name = "Regrowth",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.regrowth.HPM,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.regrowth.HPM }
                        };
                        comparisonList.Add(regrowth);
                        ComparisonCalculationTree regrowthAgain = new ComparisonCalculationTree()
                        {
                            Name = "Regrowth (chaincast)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.regrowthAgain.HPM_DH,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.regrowthAgain.HPM_DH }
                        };
                        comparisonList.Add(regrowthAgain);
                        ComparisonCalculationTree lifebloom = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloom.HPM,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloom.HPM }
                        };
                        comparisonList.Add(lifebloom);
                        ComparisonCalculationTree lifebloomRollingStack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Rolling Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloomRollingStack.HPM,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloomRollingStack.HPM }
                        };
                        comparisonList.Add(lifebloomRollingStack);
                        ComparisonCalculationTree lifebloomSlowStack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Slow Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloomSlowStack.HPM,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloomSlowStack.HPM }
                        };
                        comparisonList.Add(lifebloomSlowStack);
                        ComparisonCalculationTree lifebloomFastStack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Fast Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloomFastStack.HPM,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloomFastStack.HPM }
                        };
                        comparisonList.Add(lifebloomFastStack);
                        ComparisonCalculationTree lifebloomSlow2Stack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Slow x2 Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloomSlow2Stack.HPM,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloomSlow2Stack.HPM }
                        };
                        comparisonList.Add(lifebloomSlow2Stack);
                        ComparisonCalculationTree lifebloomFast2Stack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Fast x2 Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.lifebloomFast2Stack.HPM,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.lifebloomFast2Stack.HPM }
                        };
                        comparisonList.Add(lifebloomFast2Stack);
                        ComparisonCalculationTree wildGrowth = new ComparisonCalculationTree()
                        {
                            Name = "Wild Growth",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.wildGrowth.HPM * calculationResult.Sustained.spellMix.wildGrowth.maxTargets,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.wildGrowth.HPM * calculationResult.Sustained.spellMix.wildGrowth.maxTargets }
                        };
                        comparisonList.Add(wildGrowth);
                        ComparisonCalculationTree wildGrowthSingle = new ComparisonCalculationTree()
                        {
                            Name = "Wild Growth (one target)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.wildGrowth.HPM,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.wildGrowth.HPM }
                        };
                        comparisonList.Add(wildGrowthSingle);
                        if (calculationResult.Sustained.spellMix.swiftmend != null)
                        {
                            ComparisonCalculationTree swiftmend = new ComparisonCalculationTree()
                            {
                                Name = "Swiftmend",
                                Equipped = false,
                                OverallPoints = calculationResult.Sustained.spellMix.swiftmend.HPM,
                                SubPoints = new float[] { calculationResult.Sustained.spellMix.swiftmend.HPM }
                            };
                            comparisonList.Add(swiftmend);
                        }
                        ComparisonCalculationTree nourish0 = new ComparisonCalculationTree()
                        {
                            Name = "Nourish (0)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.nourish[0].HPM,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.nourish[0].HPM }
                        };
                        comparisonList.Add(nourish0);
                        ComparisonCalculationTree nourish1 = new ComparisonCalculationTree()
                        {
                            Name = "Nourish (1)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.nourish[1].HPM,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.nourish[1].HPM }
                        };
                        comparisonList.Add(nourish1);
                        ComparisonCalculationTree nourish2 = new ComparisonCalculationTree()
                        {
                            Name = "Nourish (2)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.nourish[2].HPM,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.nourish[2].HPM }
                        };
                        comparisonList.Add(nourish2);
                        ComparisonCalculationTree nourish3 = new ComparisonCalculationTree()
                        {
                            Name = "Nourish (3)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.nourish[3].HPM,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.nourish[3].HPM }
                        };
                        comparisonList.Add(nourish3);
                        ComparisonCalculationTree nourish4 = new ComparisonCalculationTree()
                        {
                            Name = "Nourish (4)",
                            Equipped = false,
                            OverallPoints = calculationResult.Sustained.spellMix.nourish[4].HPM,
                            SubPoints = new float[] { calculationResult.Sustained.spellMix.nourish[4].HPM }
                        };
                        comparisonList.Add(nourish4);
                    }
                    return comparisonList.ToArray();
#endregion
                #region Healing per spell (single target)
                case "Healing per spell (single target)":
                    {
                        _subPointNameColors = _subPointNameColorsHPS;
                        ComparisonCalculationTree rejuv = new ComparisonCalculationTree()
                        {
                            Name = "Rejuvenation",
                            Equipped = false,
                            OverallPoints = calculationResult.SingleTarget[0].spellMix.RejuvHPS,
                            SubPoints = new float[] { calculationResult.SingleTarget[0].spellMix.RejuvHPS }
                        };
                        comparisonList.Add(rejuv);
                        ComparisonCalculationTree regrowth = new ComparisonCalculationTree()
                        {
                            Name = "Regrowth",
                            Equipped = false,
                            OverallPoints = calculationResult.SingleTarget[0].spellMix.RegrowthHPS,
                            SubPoints = new float[] { calculationResult.SingleTarget[0].spellMix.RegrowthHPS }
                        };
                        comparisonList.Add(regrowth);
                        ComparisonCalculationTree lifebloomStack = new ComparisonCalculationTree()
                        {
                            Name = "Lifebloom Stack",
                            Equipped = false,
                            OverallPoints = calculationResult.SingleTarget[0].spellMix.LifebloomStackHPS,
                            SubPoints = new float[] { calculationResult.SingleTarget[0].spellMix.LifebloomStackHPS }
                        };
                        comparisonList.Add(lifebloomStack);
                        if (calculationResult.Sustained.spellMix.swiftmend != null)
                        {
                            ComparisonCalculationTree swiftmend = new ComparisonCalculationTree()
                            {
                                Name = "Swiftmend",
                                Equipped = false,
                                OverallPoints = calculationResult.SingleTarget[0].spellMix.SwiftmendHPS,
                                SubPoints = new float[] { calculationResult.SingleTarget[0].spellMix.SwiftmendHPS }
                            };
                            comparisonList.Add(swiftmend);
                        }
                        ComparisonCalculationTree primary = new ComparisonCalculationTree()
                        {
                            Name = "Nourish",
                            Equipped = false,
                            OverallPoints = calculationResult.SingleTarget[0].spellMix.NourishHPS,
                            SubPoints = new float[] { calculationResult.SingleTarget[0].spellMix.NourishHPS }
                        };
                        comparisonList.Add(primary);
                    }
                    return comparisonList.ToArray();
                    #endregion
                #region Single Target spell mixes (HPS)
                case "Single target spell mixes (HPS)":
                    {
                        _subPointNameColors = _subPointNameColorsHPS;
                        for (int i = 2; i < 19; i++)
                        {

                            ComparisonCalculationTree spell = new ComparisonCalculationTree()
                            {
                                Name = Solver.SingleTargetRotationToText(calculationResult.SingleTarget[i].rotation),
                                Equipped = false,
                                OverallPoints = calculationResult.SingleTarget[i].HPS,
                                SubPoints = new float[] { calculationResult.SingleTarget[i].HPS }
                            };
                            comparisonList.Add(spell);
                        }
                    }
                    return comparisonList.ToArray();
                #endregion
                #region Single Target spell mixes (HPM)
                case "Single target spell mixes (HPM)":
                    {
                        _subPointNameColors = _subPointNameColorsHPM;
                        for (int i = 2; i < 19; i++)
                        {

                            ComparisonCalculationTree spell = new ComparisonCalculationTree()
                            {
                                Name = Solver.SingleTargetRotationToText(calculationResult.SingleTarget[i].rotation),
                                Equipped = false,
                                OverallPoints = calculationResult.SingleTarget[i].HPM,
                                SubPoints = new float[] { calculationResult.SingleTarget[i].HPM }
                            };
                            comparisonList.Add(spell);
                        }
                    }
                    return comparisonList.ToArray();
                #endregion
                #region Single Target spell mixes (MPS)
                case "Single target spell mixes (MPS)":
                    {
                        _subPointNameColors = _subPointNameColorsMPS;
                        for (int i = 2; i < 19; i++)
                        {

                            ComparisonCalculationTree spell = new ComparisonCalculationTree()
                            {
                                Name = Solver.SingleTargetRotationToText(calculationResult.SingleTarget[i].rotation),
                                Equipped = false,
                                OverallPoints = calculationResult.SingleTarget[i].spellMix.MPS,
                                SubPoints = new float[] { calculationResult.SingleTarget[i].spellMix.MPS }
                            };
                            comparisonList.Add(spell);
                        }
                    }
                    return comparisonList.ToArray();
                #endregion
                default:
                    return new ComparisonCalculationBase[0];
            }
        }

#if !RAWR3
        public override void RenderCustomChart(Character character, string chartName, System.Drawing.Graphics g, int width, int height)
        {
            string[] statNames = new string[] { "11.7 Spell Power", "4 Mana per 5 sec", "10 Crit Rating", "10 Haste Rating", "10 Intellect", "10 Spirit" };
            Color[] statColors = new Color[] { Color.FromArgb(255, 255, 0, 0), Color.DarkBlue, Color.FromArgb(255, 255, 165, 0), Color.Olive, Color.FromArgb(255, 154, 205, 50), Color.Aqua };


            height -= 2;
            switch (chartName)
            {
                case "Stats Graph":
                    Stats[] statsList = new Stats[] {
                        new Stats() { SpellPower = 1 },
                        new Stats() { Mp5 = 1 },
                        new Stats() { CritRating = 1 },
                        new Stats() { HasteRating = 1 },
                        new Stats() { Intellect = 1 },
                        new Stats() { Spirit = 1 },
                    };

                    Base.Graph.RenderStatsGraph(g, width, height, character, statsList, statColors, 200, "", "Sustained Rating", Base.Graph.Style.Mage);
                    break;
            }
        }
#endif


        public override Stats GetRelevantStats(Stats stats) {
            Stats s = new Stats() {
                #region Base Stats
                //Stamina = stats.Stamina,
                Intellect = stats.Intellect,
                Spirit = stats.Spirit,
                SpellPower = stats.SpellPower,
                CritRating = stats.CritRating,
                HasteRating = stats.HasteRating,
                SpellHaste = stats.SpellHaste,
                SpellCrit = stats.SpellCrit,
                //Health = stats.Health,
                Mana = stats.Mana,
                Mp5 = stats.Mp5,
                Armor = stats.Armor,
                Stamina = stats.Stamina,
                ManaRestoreFromMaxManaPerSecond = stats.ManaRestoreFromMaxManaPerSecond,
                MovementSpeed = stats.MovementSpeed,
                BonusHealingDoneMultiplier = stats.BonusHealingDoneMultiplier,
                #endregion
                #region Trinkets
                HighestStat = stats.HighestStat,
                SpellsManaReduction = stats.SpellsManaReduction,
                HealingOmenProc = stats.HealingOmenProc, 
                #endregion
                #region Idols and Sets
                LifebloomCostReduction = stats.LifebloomCostReduction,  //T7 (2) Bonus
                NourishBonusPerHoT = stats.NourishBonusPerHoT,          //T7 (4) Bonus
                SwiftmendBonus = stats.SwiftmendBonus,                   //T8 (2) Bonus
                RejuvenationInstantTick = stats.RejuvenationInstantTick, //T8 (4) Bonus
                NourishCritBonus = stats.NourishCritBonus,              // T9 (2) Bonus
                RejuvenationCrit = stats.RejuvenationCrit,              // T9 (4) Bonus
                WildGrowthLessReduction = stats.WildGrowthLessReduction, // T10 (2) Bonus
                RejuvJumpChance = stats.RejuvJumpChance,                 // T10 (4) Bonus
                NourishSpellpower = stats.NourishSpellpower, // Idol of the Flourishing Life
                RejuvenationHealBonus = stats.RejuvenationHealBonus, // Idol of Pure Thoughts (lvl74)
                ReduceRejuvenationCost = stats.ReduceRejuvenationCost, // Idol of Awakening (lvl80) 
                LifebloomTickHealBonus = stats.LifebloomTickHealBonus, // Idol of Lush Mosh
                HealingTouchFinalHealBonus = stats.HealingTouchFinalHealBonus, // Idol of Health                 
                SwiftmendCdReduc = stats.SwiftmendCdReduc, // S7 PvP 4 Pc
                #endregion
                #region Gems
                BonusCritHealMultiplier = stats.BonusCritHealMultiplier,
                BonusManaMultiplier = stats.BonusManaMultiplier,
                BonusIntellectMultiplier = stats.BonusIntellectMultiplier,
                #endregion
            };
            foreach (Rawr.SpecialEffect effect in stats.SpecialEffects()) {
                if (effect.Trigger == Trigger.Use || 
                    effect.Trigger == Trigger.SpellCast || effect.Trigger == Trigger.SpellCrit || effect.Trigger == Trigger.SpellHit
                    || effect.Trigger == Trigger.HealingSpellCast || effect.Trigger == Trigger.HealingSpellCrit || effect.Trigger == Trigger.HealingSpellHit
                    || effect.Trigger == Trigger.RejuvenationTick  // Idol of Flaring Growth
                    )
                {
                    if (HasRelevantSpecialEffectStats(effect.Stats)) {
                        s.AddSpecialEffect(effect);
                    } else {
                        // Trigger seems relevant, but effect not. Sounds weird, probably DPS bonus on Use or general SpellCast ...
                    }
                }
            }
            return s;
        }
        public bool HasRelevantSpecialEffectStats(Stats stats) {
            foreach (Rawr.SpecialEffect effect in stats.SpecialEffects())
            {
                if (effect.Trigger == Trigger.Use ||
                    effect.Trigger == Trigger.SpellCast || effect.Trigger == Trigger.SpellCrit
                    || effect.Trigger == Trigger.HealingSpellCast || effect.Trigger == Trigger.HealingSpellCrit || effect.Trigger == Trigger.HealingSpellHit
                    || effect.Trigger == Trigger.RejuvenationTick  // Idol of Flaring Growth
                    )
                {
                    if (HasRelevantSpecialEffectStats(effect.Stats)) return true;
                }
            }
            return (stats.Intellect + stats.Spirit + stats.SpellPower + stats.CritRating + stats.HasteRating + stats.ManaRestore
                   + stats.Mp5 + stats.Healed + stats.HighestStat + stats.BonusHealingReceived + stats.HealingOmenProc
                   + stats.ShieldFromHealed + stats.ManaRestoreFromMaxManaPerSecond) > 0;
        }
        public override bool HasRelevantStats(Stats stats) {
            if (HasRelevantSpecialEffectStats(stats)) return true;

            if (stats.Intellect + stats.Spirit + stats.Mp5 + stats.SpellPower + stats.Mana + stats.CritRating + stats.SpellCrit
                + stats.HasteRating + stats.SpellHaste + stats.BonusSpellPowerMultiplier
                + stats.BonusSpiritMultiplier + stats.BonusIntellectMultiplier + stats.BonusStaminaMultiplier
                + stats.BonusCritHealMultiplier + stats.BonusManaMultiplier
                /*+ stats.Armor + stats.Stamina*/ + stats.ManaRestoreFromMaxManaPerSecond
                + stats.MovementSpeed + stats.SpellCombatManaRegeneration // Bangle of nerfed - might be useful in future
                + stats.BonusHealingDoneMultiplier
                #region Trinkets
                + stats.HighestStat + stats.SpellsManaReduction + stats.HealingOmenProc
                #endregion
                #region Idols and Sets
                + stats.LifebloomCostReduction + stats.NourishBonusPerHoT // T7
                + stats.SwiftmendBonus + stats.RejuvenationInstantTick // T8
                + stats.NourishCritBonus + stats.RejuvenationCrit // T9
                + stats.WildGrowthLessReduction + stats.RejuvJumpChance // T10
                + stats.NourishSpellpower // Idol of the Flourishing Life
                + stats.RejuvenationHealBonus // Idol of Pure Thoughts (lvl74)
                + stats.ReduceRejuvenationCost // Idol of Awakening (lvl80) 
                + stats.LifebloomTickHealBonus // Idol of Lush Mosh
                + stats.HealingTouchFinalHealBonus // Idol of Health       
                + stats.SwiftmendCdReduc // S7 PvP 4 Pc
                #endregion
                > 0)
                return true;

            return false;
        }
        public Stats GetBuffsStats(Character character, CalculationOptionsTree calcOpts) {
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
            }*/
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
        // Wildebees 20090407 : Overload base function to disable all enchants on OffHand for tree druids
        public override bool EnchantFitsInSlot(Enchant enchant, Character character, ItemSlot slot) {
            if (slot == ItemSlot.OffHand) { return false; }
            return base.EnchantFitsInSlot(enchant, character, slot);
        }

        public override ICalculationOptionBase DeserializeDataObject(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsTree));
            StringReader reader = new StringReader(xml);
            CalculationOptionsTree calcOpts = serializer.Deserialize(reader) as CalculationOptionsTree;
            return calcOpts;
        }
    }

}
