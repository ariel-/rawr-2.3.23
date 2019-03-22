using System;
using System.Collections.Generic;
using System.Text;
#if RAWR3
using System.Windows.Media;
#else
using System.Drawing;
#endif
using Rawr.Elemental.Spells;

namespace Rawr.Elemental
{
    [Rawr.Calculations.RawrModelInfo("Elemental", "Spell_Nature_Lightning", CharacterClass.Shaman)]
    public class CalculationsElemental : CalculationsBase
    {
        private List<GemmingTemplate> _defaultGemmingTemplates = null;
        public override List<GemmingTemplate> DefaultGemmingTemplates
        {
            get
            {
                if (_defaultGemmingTemplates == null)
                {
                    // Meta
                    int chaotic = 41285;

                    // [0] uncommon
                    // [1] perfect uncommon
                    // [2] rare
                    // [3] epic
                    // [4] jewelcrafting

                    // Reds
                    int[] runed = { 39911, 41438, 39998, 40113, 42144 }; // spell power
                    // Blue
                    int[] lustrous = { 39927, 41440, 40010, 40121, 42146 }; // mp5
                    // Yellow
                    int[] quick = { 39918, 41446, 40017, 40128, 42150 }; // haste
                    int[] rigid = { 39915, 41447, 40014, 40125, 42156 }; // hit
                    // Purple
                    int[] royal = { 39943, 41459, 40027, 40134 }; // spell power + mp5
                    // Green
                    int[] dazzling = { 39984, 41463, 40094, 40175 }; // int + mp5
                    int[] lambent = { 39986, 41469, 40100, 40177 }; // hit + mp5
                    // Orange
                    int[] luminous = { 39946, 41494, 40047, 40151 }; // spell power + int
                    int[] reckless = { 39959, 41497, 40051, 40155 }; // spell power + haste
                    int[] potent = { 39956, 41495, 40048, 40152}; // spell power + crit
                    int[] veiled = { 39957, 41502, 40049, 40153 }; // spell power + hit

                    /*
                     * red: runed, royal
                     * yellow: reckless, quick
                     * blue: royal, dazzling(, lustrous)
                     */

                    _defaultGemmingTemplates = new List<GemmingTemplate>();
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Uncommon", false, runed[0], royal[0], reckless[0], quick[0], dazzling[0], rigid[0], veiled[0], lambent[0], chaotic);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Perfect", false, runed[1], royal[1], reckless[1], quick[1], dazzling[1], rigid[1], veiled[1], lambent[1], chaotic);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Rare", false, runed[2], royal[2], reckless[2], quick[2], dazzling[2], rigid[2], veiled[2], lambent[2], chaotic);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Epic", true, runed[3], royal[3], reckless[3], quick[3], dazzling[3], rigid[3], veiled[3], lambent[3], chaotic);
                    AddJCGemmingTemplateGroup(_defaultGemmingTemplates, "Jewelcrafting", false, runed[4], quick[4], lustrous[4], rigid[4], chaotic);
                }
                return _defaultGemmingTemplates;
            }
        }

        private void AddGemmingTemplateGroup(List<GemmingTemplate> list, string name, bool enabled, int runed, int royal, int reckless, int quick, int dazzling, int rigid, int veiled, int lambent, int meta)
        {
            // if no mana problem, runed and rigid are always very good
            list.Add(new GemmingTemplate() { Model = "Elemental", Group = name, RedId = runed, YellowId = runed, BlueId = runed, PrismaticId = runed, MetaId = meta, Enabled = enabled });
            list.Add(new GemmingTemplate() { Model = "Elemental", Group = name, RedId = rigid, YellowId = rigid, BlueId = rigid, PrismaticId = rigid, MetaId = meta, Enabled = enabled });
            list.Add(new GemmingTemplate() { Model = "Elemental", Group = name, RedId = veiled, YellowId = veiled, BlueId = veiled, PrismaticId = veiled, MetaId = meta, Enabled = enabled });
            // if mana problem, dazzling is often very good
            list.Add(new GemmingTemplate() { Model = "Elemental", Group = name, RedId = dazzling, YellowId = dazzling, BlueId = dazzling, PrismaticId = dazzling, MetaId = meta, Enabled = enabled });

            /*
             * red: runed, royal
             * yellow: reckless, quick
             * blue: royal, dazzling, lambent (, lustrous)
             */
            // +Power and +Crit
            list.Add(new GemmingTemplate() { Model = "Elemental", Group = name, 
                RedId = runed, YellowId = reckless, BlueId = royal, PrismaticId = runed, 
                MetaId = meta, Enabled = enabled });
            // +Hit
            list.Add(new GemmingTemplate() { Model = "Elemental", Group = name, 
                RedId = veiled, YellowId = rigid, BlueId = royal, PrismaticId = rigid, 
                MetaId = meta, Enabled = enabled });
            list.Add(new GemmingTemplate() { Model = "Elemental", Group = name, 
                RedId = veiled, YellowId = rigid, BlueId = lambent, PrismaticId = rigid, 
                MetaId = meta, Enabled = enabled });
            // MP5
            list.Add(new GemmingTemplate() { Model = "Elemental", Group = name, 
                RedId = veiled, YellowId = rigid, BlueId = dazzling, PrismaticId = veiled, 
                MetaId = meta, Enabled = enabled });
            list.Add(new GemmingTemplate() { Model = "Elemental", Group = name, 
                RedId = royal, YellowId = dazzling, BlueId = dazzling, PrismaticId = royal, 
                MetaId = meta, Enabled = enabled });
            list.Add(new GemmingTemplate() { Model = "Elemental", Group = name, 
                RedId = royal, YellowId = dazzling, BlueId = dazzling, PrismaticId = dazzling, 
                MetaId = meta, Enabled = enabled });
            list.Add(new GemmingTemplate() { Model = "Elemental", Group = name, 
                RedId = runed, YellowId = dazzling, BlueId = royal, PrismaticId = royal, 
                MetaId = meta, Enabled = enabled });
            list.Add(new GemmingTemplate() { Model = "Elemental", Group = name, 
                RedId = runed, YellowId = dazzling, BlueId = royal, PrismaticId = dazzling, 
                MetaId = meta, Enabled = enabled });
        }

        private void AddJCGemmingTemplateGroup(List<GemmingTemplate> list, string name, bool enabled, int runed, int quick, int lustrous, int rigid, int meta)
        {
            // Overrides, only "runed" and "seers"
            list.Add(new GemmingTemplate() { Model = "Elemental", Group = name, RedId = runed, YellowId = quick, BlueId = runed, PrismaticId = runed, MetaId = meta, Enabled = enabled });
            list.Add(new GemmingTemplate() { Model = "Elemental", Group = name, RedId = lustrous, YellowId = lustrous, BlueId = lustrous, PrismaticId = lustrous, MetaId = meta, Enabled = enabled });

            list.Add(new GemmingTemplate() { Model = "Elemental", Group = name, RedId = runed, YellowId = quick, BlueId = lustrous, PrismaticId = runed, MetaId = meta, Enabled = enabled });
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
                    _calculationOptionsPanel = new CalculationOptionsPanelElemental();
                }
                return _calculationOptionsPanel;
            }
        }

        public override bool ItemFitsInSlot(Item item, Character character, CharacterSlot slot, bool ignoreUnique)
        {
            if (
                    slot == CharacterSlot.OffHand
                    && character.ShamanTalents.DualWield < 1
                    && 
                    (
                        item.Type == ItemType.Dagger || 
                        item.Type == ItemType.FistWeapon ||
                        item.Type == ItemType.OneHandAxe ||
                        item.Type == ItemType.OneHandMace
                    )
                )
            { return false; }
            return base.ItemFitsInSlot(item, character, slot, ignoreUnique);
        }

        private string[] _characterDisplayCalculationLabels = null;
        public override string[] CharacterDisplayCalculationLabels
        {
            get
            {
                if (_characterDisplayCalculationLabels == null)
                    _characterDisplayCalculationLabels = new string[] {
                    "Summary:Overall Points*Sum of burst and sustained points",
                    "Summary:Burst Points*DPS until you go out of mana",
                    "Summary:Sustained Points*Total DPS of the fight",

                    "Basic Stats:Health",
                    "Basic Stats:Mana",
                    "Basic Stats:Stamina",
                    "Basic Stats:Intellect",
                    "Basic Stats:Spell Power",
                    "Basic Stats:Hit Rating",
                    "Basic Stats:Crit Rating",
                    "Basic Stats:Haste Rating",
                    "Basic Stats:Mana Regen",

                    "Combat Stats:Average Health",
                    "Combat Stats:Average Mana",
                    "Combat Stats:Average Stamina",
                    "Combat Stats:Average Intellect",
                    "Combat Stats:Average Spell Power",
                    "Combat Stats:Average Hit Rating",
                    "Combat Stats:Average Crit Rating",
                    "Combat Stats:Average Haste Rating",
                    "Combat Stats:Average Mana Regen",

                    "Attacks:Lightning Bolt",
                    "Attacks:Chain Lightning",
                    "Attacks:Lava Burst",
                    "Attacks:Flame Shock",
                    "Attacks:Earth Shock",
                    "Attacks:Frost Shock",
                    "Attacks:Fire Nova",
                    "Attacks:Searing Totem",
                    "Attacks:Magma Totem",
                    
                    "Simulation:Simulation",
                    "Simulation:Rotation",
                };
                return _characterDisplayCalculationLabels;
            }
        }

        public override void SetDefaults(Character character)
        {
            character.ActiveBuffsAdd(("Sanctified Retribution"));
            character.ActiveBuffsAdd(("Heroism/Bloodlust"));
            character.ActiveBuffsAdd(("Swift Retribution"));
            character.ActiveBuffsAdd(("Arcane Intellect"));
            character.ActiveBuffsAdd(("Hunting Party"));
            character.ActiveBuffsAdd(("Blessing of Wisdom"));
                character.ActiveBuffsAdd(("Improved Blessing of Wisdom"));
            character.ActiveBuffsAdd(("Moonkin Form"));
            character.ActiveBuffsAdd(("Wrath of Air Totem"));
            character.ActiveBuffsAdd(("Totem of Wrath (Spell Power)"));
            character.ActiveBuffsAdd(("Divine Spirit"));
            character.ActiveBuffsAdd(("Mark of the Wild"));
            character.ActiveBuffsAdd(("Improved Mark of the Wild"));
            character.ActiveBuffsAdd(("Blessing of Kings"));
            character.ActiveBuffsAdd(("Totem of Wrath"));
            character.ActiveBuffsAdd(("Judgement of Wisdom"));
            character.ActiveBuffsAdd(("Improved Shadow Bolt"));
            character.ActiveBuffsAdd(("Curse of the Elements"));
            character.ActiveBuffsAdd(("Improved Faerie Fire"));
            character.ActiveBuffsAdd(("Flask of the Frost Wyrm"));
            character.ActiveBuffsAdd(("Fish Feast"));
        }

        private Dictionary<string, Color> _subPointNameColors = null;
        public override Dictionary<string, Color> SubPointNameColors
        {
            get
            {
                if (_subPointNameColors == null)
                {
                    _subPointNameColors = new Dictionary<string, Color>();
                    _subPointNameColors.Add("Burst DPS", Color.FromArgb(255, 255, 0, 0));
                    _subPointNameColors.Add("Sustained DPS", Color.FromArgb(255, 0, 0, 255));
                }
                return _subPointNameColors;
            }
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
                        ItemType.None,
                        ItemType.Cloth,						
                        ItemType.Leather,
                        ItemType.Mail,
                        ItemType.Dagger,
                        ItemType.FistWeapon,
                        ItemType.OneHandAxe,
                        ItemType.OneHandMace,
                        ItemType.Shield,
                        ItemType.Staff,
                        ItemType.Totem,
                        ItemType.TwoHandAxe,
                        ItemType.TwoHandMace
                    });
                }
                return _relevantItemTypes;
            }
        }

        public override CharacterClass TargetClass { get { return CharacterClass.Shaman; } }
        public override ComparisonCalculationBase CreateNewComparisonCalculation() { return new ComparisonCalculationElemental(); }
        public override CharacterCalculationsBase CreateNewCharacterCalculations() { return new CharacterCalculationsElemental(); }

        private string[] _customChartNames = {};
        public override string[] CustomChartNames
        {
            get
            {
                return _customChartNames;
            }
        }

        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
        {
            return new ComparisonCalculationBase[0];
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
                    _customRenderedChartNames = new string[] { "Stats Graph" };
                }
                return _customRenderedChartNames;
            }
        }
#endif

#if !RAWR3
        public override void RenderCustomChart(Character character, string chartName, System.Drawing.Graphics g, int width, int height)
        {

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

                    Color[] statsColors = new Color[] { 
                        Color.FromArgb(255, 255, 0, 0), 
                        Color.DarkBlue, 
                        Color.FromArgb(255, 255, 165, 0), 
                        Color.Olive, 
                        Color.FromArgb(255, 154, 205, 50), 
                        Color.Aqua 
                    };

                    Base.Graph.RenderStatsGraph(g, width, height, character, statsList, statsColors, 200, "", "Sustained DPS", Base.Graph.Style.Mage);
                    break;
            }
        }
#endif

        public override ICalculationOptionBase DeserializeDataObject(string xml)
        {
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsElemental));
            System.IO.StringReader reader = new System.IO.StringReader(xml);
            CalculationOptionsElemental calcOpts = serializer.Deserialize(reader) as CalculationOptionsElemental;
            return calcOpts;
        }

        public override CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, bool referenceCalculation, bool significantChange, bool needsDisplayCalculations)
        {
            CalculationOptionsElemental calcOpts = character.CalculationOptions as CalculationOptionsElemental;
            if (calcOpts == null) calcOpts = new CalculationOptionsElemental();
            BossOptions bossOpts = character.BossOptions;
            if (bossOpts == null) bossOpts = new BossOptions();
            Stats stats = GetCharacterStats(character, additionalItem);

            CharacterCalculationsElemental calculatedStats = new CharacterCalculationsElemental();
            calculatedStats.BasicStats = stats;
            calculatedStats.LocalCharacter = character;
            calcOpts.calculatedStats = calculatedStats;

            Rawr.Elemental.Estimation.solve(calculatedStats, calcOpts, bossOpts);

            return calculatedStats;
            
        }

        //slowly being moved back into GetCharacterStats to reduce Stats
        public Stats GetTalentStats(ShamanTalents talents)
        {
            Stats statsTalents = new Stats()
            {
                #region Elemental
                SpellHit = .01f * talents.ElementalPrecision,
                #endregion
                #region Enhancement
                BonusIntellectMultiplier = .02f * talents.AncestralKnowledge,
                PhysicalCrit = .01f * talents.ThunderingStrikes,
                SpellCrit =0.01f * talents.ThunderingStrikes,
                #endregion
                #region Glyphs
                SpellPower = talents.GlyphofTotemofWrath ? talents.TotemOfWrath * 84 : 0
                #endregion
            };
            return statsTalents;
        }

        public override Stats GetCharacterStats(Character character, Item additionalItem)
        {
            CalculationOptionsElemental calcOpts = character.CalculationOptions as CalculationOptionsElemental;

            Stats statsRace = BaseStats.GetBaseStats(character);
            Stats statsItems = GetItemStats(character, additionalItem);
            Stats statsBuffs = GetBuffsStats(character, calcOpts);
            Stats statsTalents = GetTalentStats(character.ShamanTalents);

            Stats statsTotal = statsRace + statsItems + statsBuffs + statsTalents;

            if (statsTotal.HighestStat > 0)
            {
                if (statsTotal.Spirit > statsTotal.Intellect)
                {
                    statsTotal.Spirit += (statsTotal.HighestStat * 15f / 50f);
                }
                else
                {
                    statsTotal.Intellect += (statsTotal.HighestStat * 15f / 50f);
                }
            }

            statsTotal.Strength *= 1 + statsTotal.BonusStrengthMultiplier;
            statsTotal.Agility *= 1 + statsTotal.BonusAgilityMultiplier;
            statsTotal.Stamina *= 1 + statsTotal.BonusStaminaMultiplier;
            statsTotal.Intellect *= 1 + statsTotal.BonusIntellectMultiplier;
            statsTotal.Spirit *= 1 + statsTotal.BonusSpiritMultiplier;

            statsTotal.Strength = (float)Math.Floor(statsTotal.Strength);
            statsTotal.Agility = (float)Math.Floor(statsTotal.Agility);
            statsTotal.Stamina = (float)Math.Floor(statsTotal.Stamina);
            statsTotal.Intellect = (float)Math.Floor(statsTotal.Intellect);
            statsTotal.Spirit = (float)Math.Floor(statsTotal.Spirit);

            statsTotal.AttackPower += statsTotal.Strength + statsTotal.Agility;
            statsTotal.SpellPower += (float)Math.Round(statsTotal.AttackPower * .1f * character.ShamanTalents.MentalQuickness);

            statsTotal.Mana += StatConversion.GetManaFromIntellect(statsTotal.Intellect);
            statsTotal.Mana *= (float)Math.Round(1f + statsTotal.BonusManaMultiplier);

            statsTotal.Health += StatConversion.GetHealthFromStamina(statsTotal.Stamina);
            statsTotal.Health *= (float)Math.Round(1f + statsTotal.BonusHealthMultiplier);

            statsTotal.Mp5 += (float)Math.Floor(statsTotal.Intellect * .04f * character.ShamanTalents.UnrelentingStorm);

            statsTotal.SpellCrit += StatConversion.GetSpellCritFromRating(statsTotal.CritRating);
            statsTotal.SpellCrit += StatConversion.GetSpellCritFromIntellect(statsTotal.Intellect);
            statsTotal.SpellCrit += statsTotal.SpellCritOnTarget;
            statsTotal.SpellHit += StatConversion.GetSpellHitFromRating(statsTotal.HitRating);

            // Flametongue weapon assumed
            statsTotal.SpellPower += (float)Math.Floor(211 * (1f + character.ShamanTalents.ElementalWeapons * .1f));
            if (character.ShamanTalents.GlyphofFlametongueWeapon) statsTotal.SpellCrit += .02f;

            // Water shield assumed
            statsTotal.Mp5 += 100;
            if (character.ShamanTalents.GlyphofWaterMastery) statsTotal.Mp5 += 30;

            return statsTotal;
        }

        public override bool IsItemRelevant(Item item)
        {
            if ((item.Slot == ItemSlot.Ranged && item.Type != ItemType.Totem))
                return false;
            return base.IsItemRelevant(item);
        }

        public override bool IsBuffRelevant(Buff buff, Character character) {
            string name = buff.Name;
            if (!buff.AllowedClasses.Contains(CharacterClass.Shaman)) {
                return false;
            }
            return base.IsBuffRelevant(buff, character);
        }

        public override Stats GetRelevantStats(Stats stats)
        {
            Stats s = new Stats()
            {
                #region Basic stats
                Intellect = stats.Intellect,
                Mana = stats.Mana,
                Spirit= stats.Spirit,
                SpellCrit = stats.SpellCrit,
                SpellCritOnTarget = stats.SpellCritOnTarget,
                SpellHit = stats.SpellHit,
                SpellHaste = stats.SpellHaste,
                SpellPower = stats.SpellPower,
                CritRating = stats.CritRating,
                HasteRating = stats.HasteRating,
                HitRating = stats.HitRating,
                SpellFireDamageRating = stats.SpellFireDamageRating,
                SpellNatureDamageRating = stats.SpellNatureDamageRating,
                SpellFrostDamageRating = stats.SpellFrostDamageRating,
                Mp5 = stats.Mp5,
                ManaRestoreFromMaxManaPerSecond = stats.ManaRestoreFromMaxManaPerSecond,
                ManaRestore = stats.ManaRestore,
                ManaRestoreFromBaseManaPPM = stats.ManaRestoreFromBaseManaPPM,
                MovementSpeed = stats.MovementSpeed,
                #endregion
                #region Multipliers
                BonusIntellectMultiplier = stats.BonusIntellectMultiplier,
                BonusSpiritMultiplier = stats.BonusSpiritMultiplier,
                BonusSpellCritMultiplier = stats.BonusSpellCritMultiplier,
                BonusSpellPowerMultiplier = stats.BonusSpellPowerMultiplier,
                BonusFireDamageMultiplier = stats.BonusFireDamageMultiplier,
                BonusNatureDamageMultiplier = stats.BonusNatureDamageMultiplier,
                BonusFrostDamageMultiplier = stats.BonusFrostDamageMultiplier,
                BonusDamageMultiplier = stats.BonusDamageMultiplier,
                #endregion
                #region Totems
                LightningSpellPower = stats.LightningSpellPower,
                LavaBurstBonus = stats.LavaBurstBonus,
                #endregion
                #region Sets
                LightningBoltCostReduction = stats.LightningBoltCostReduction,
                LightningBoltDamageModifier = stats.LightningBoltDamageModifier,
                BonusLavaBurstCritDamage = stats.BonusLavaBurstCritDamage,
                BonusFlameShockDoTDamage = stats.BonusFlameShockDoTDamage,
                LightningBoltCritDamageModifier = stats.LightningBoltCritDamageModifier,
                BonusLavaBurstDamageMultiplier = stats.BonusLavaBurstDamageMultiplier,
                BonusFlameShockDuration = stats.BonusFlameShockDuration,
                Elemental2T10 = stats.Elemental2T10,
                Elemental4T10 = stats.Elemental4T10,
                #endregion
                #region Misc Damage
                NatureDamage = stats.NatureDamage,
                ArcaneDamage = stats.ArcaneDamage,
                FireDamage = stats.FireDamage,
                ShadowDamage = stats.ShadowDamage
                #endregion
            };
            #region Trinkets
            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if (effect.Trigger == Trigger.Use || 
                    effect.Trigger == Trigger.SpellCast || 
                    effect.Trigger == Trigger.SpellHit || 
                    effect.Trigger == Trigger.SpellCrit || 
                    effect.Trigger == Trigger.SpellMiss || 
                    effect.Trigger == Trigger.DamageSpellCast || 
                    effect.Trigger == Trigger.DamageSpellCrit || 
                    effect.Trigger == Trigger.DamageSpellHit || 
                    effect.Trigger == Trigger.DoTTick || 
                    effect.Trigger == Trigger.DamageDone ||
                    effect.Trigger == Trigger.DamageOrHealingDone || 
                    effect.Trigger == Trigger.ShamanLightningBolt ||
                    effect.Trigger == Trigger.ShamanFlameShockDoTTick || 
                    effect.Trigger == Trigger.ShamanShock)
                {
                    if (HasRelevantStats(effect.Stats))
                    {
                        s.AddSpecialEffect(effect);
                    }
                }
            }
            #endregion
            return s;
        }

        public override bool HasRelevantStats(Stats stats)
        {
            float elementalStats = 0;
            #region Basic stats
            elementalStats +=
                stats.Intellect +
                stats.Mana +
                stats.Spirit +
                stats.SpellCrit +
                stats.SpellCritOnTarget +
                stats.SpellHit +
                stats.SpellHaste +
                stats.SpellPower +
                stats.CritRating +
                stats.HasteRating +
                stats.HitRating +
                stats.SpellFireDamageRating +
                stats.SpellNatureDamageRating +
                stats.SpellFrostDamageRating +
                stats.Mp5 +
                stats.ManaRestoreFromMaxManaPerSecond +
                stats.ManaRestore +
                stats.ManaRestoreFromBaseManaPPM +
                stats.MovementSpeed;
            #endregion
            #region Multipliers
            elementalStats +=
                stats.BonusIntellectMultiplier +
                stats.BonusSpiritMultiplier +
                stats.BonusSpellCritMultiplier +
                stats.BonusSpellPowerMultiplier +
                stats.BonusFireDamageMultiplier +
                stats.BonusNatureDamageMultiplier +
                stats.BonusFrostDamageMultiplier +
                stats.BonusDamageMultiplier;
            #endregion
            #region Totems
            elementalStats +=
                stats.LightningSpellPower +
                stats.LavaBurstBonus;
            #endregion
            #region Sets
            elementalStats += 
                stats.BonusLavaBurstCritDamage +
                stats.LightningBoltCostReduction +
                stats.LightningBoltDamageModifier +
                stats.LightningBoltCritDamageModifier +
                stats.BonusFlameShockDoTDamage + 
                stats.BonusLavaBurstDamageMultiplier + 
                stats.BonusFlameShockDuration +
                stats.Elemental2T10 +
                stats.Elemental4T10;
            #endregion
            #region Misc Damage
            elementalStats +=
                stats.NatureDamage +
                stats.ArcaneDamage +
                stats.FireDamage +
                stats.ShadowDamage;
            #endregion
            bool relevant = (elementalStats > 0);
            #region Trinkets
            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if (effect.Trigger == Trigger.Use || 
                    effect.Trigger == Trigger.SpellCast || 
                    effect.Trigger == Trigger.SpellHit || 
                    effect.Trigger == Trigger.SpellCrit || 
                    effect.Trigger == Trigger.SpellMiss || 
                    effect.Trigger == Trigger.DamageSpellCast || 
                    effect.Trigger == Trigger.DamageSpellCrit || 
                    effect.Trigger == Trigger.DamageSpellHit || 
                    effect.Trigger == Trigger.DoTTick || 
                    effect.Trigger == Trigger.DamageDone ||
                    effect.Trigger == Trigger.DamageOrHealingDone || 
                    effect.Trigger == Trigger.ShamanLightningBolt ||
                    effect.Trigger == Trigger.ShamanFlameShockDoTTick || 
                    effect.Trigger == Trigger.ShamanShock)
                {
                    relevant |= HasRelevantStats(effect.Stats);
                    if (relevant) break;
                }
            }
            #endregion
            return relevant;
        }

        public Stats GetBuffsStats(Character character, CalculationOptionsElemental calcOpts) {
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

        public override bool EnchantFitsInSlot(Enchant enchant, Character character, ItemSlot slot)
        {
            // Filters out Non-Shield Offhand Enchants and Ranged Enchants
            if ((slot == ItemSlot.OffHand && enchant.Slot != ItemSlot.OffHand) || slot == ItemSlot.Ranged) return false;
            return base.EnchantFitsInSlot(enchant, character, slot);
        }
    }

    public class CharacterCalculationsElemental : CharacterCalculationsBase
    {

        #region Variable Declarations and Definitions

        public Stats BasicStats { get; set; }
        public Stats CombatStats { get; set; }
        public int TargetLevel { get; set; }

        public Spell LightningBolt;
        public Spell ChainLightning;
        public Spell LavaBurst;
        public Spell FlameShock;
        public Spell EarthShock;
        public Spell FrostShock;
        public Spell FireNova;
        public Spell SearingTotem;
        public Spell MagmaTotem;

        public float ManaRegenInFSR;
        public float ManaRegenOutFSR;
        public float ReplenishMP5;

        public float TimeToOOM;
        public float CastRegenFraction;
        public float RotationDPS;
        public float TotalDPS;
        public float RotationMPS;
        public float CastsPerSecond;
        public float CritsPerSecond;
        public float MissesPerSecond;

        public float LBPerSecond;
        public float LvBPerSecond;
        public float FSPerSecond;

        public float LatencyPerSecond;

        public float ClearCast_FlameShock;
        public float ClearCast_LavaBurst;
        public float ClearCast_LightningBolt;

        public string Rotation;
        public string RotationDetails;

        public Character LocalCharacter { get; set; }

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

        public float BurstPoints
        {
            get { return _subPoints[0]; }
            set { _subPoints[0] = value; }
        }

        public float SustainedPoints
        {
            get { return _subPoints[1]; }
            set { _subPoints[1] = value; }
        }

        #endregion

        public override Dictionary<string, string> GetCharacterDisplayCalculationValues()
        {
            Dictionary<string, string> dictValues = new Dictionary<string, string>();

            dictValues.Add("Overall Points", OverallPoints.ToString());
            dictValues.Add("Burst Points", BurstPoints.ToString());
            dictValues.Add("Sustained Points", SustainedPoints.ToString());

            float BasicUntilGCDCap = (float)Math.Ceiling((1.5f / (1f + BasicStats.SpellHaste) - 1) * StatConversion.RATING_PER_SPELLHASTE
                - BasicStats.HasteRating);
            float BasicUntilLBCap = (float)Math.Ceiling((2f / (1f + BasicStats.SpellHaste) - 1) * StatConversion.RATING_PER_SPELLHASTE
                - BasicStats.HasteRating);

            dictValues.Add("Health", BasicStats.Health.ToString());
            dictValues.Add("Mana", BasicStats.Mana.ToString());
            dictValues.Add("Stamina", BasicStats.Stamina.ToString());
            dictValues.Add("Intellect", BasicStats.Intellect.ToString());
            dictValues.Add("Spell Power", BasicStats.SpellPower.ToString());
            dictValues.Add("Hit Rating", BasicStats.HitRating.ToString());
            dictValues.Add("Crit Rating", BasicStats.CritRating.ToString());
            dictValues.Add("Haste Rating", BasicStats.HasteRating.ToString() + "*" + BasicUntilGCDCap + " Haste Rating until GCD cap\n" + BasicUntilLBCap + " Haste Rating until LB cap");
            dictValues.Add("Mana Regen", Math.Round(ManaRegenInFSR).ToString() + " / " + Math.Round(ManaRegenOutFSR) + " + " + Math.Round(ReplenishMP5).ToString() + "*All values are Mana per 5 seconds.\nMP5 while casting / MP5 while not casting + Replenishment");

            float CombatUntilGCDCap = (float)Math.Ceiling((1.5f / (1f + CombatStats.SpellHaste) - 1) * StatConversion.RATING_PER_SPELLHASTE 
                - CombatStats.HasteRating);
            float CombatUntilLBCap = (float)Math.Ceiling((2f / (1f + CombatStats.SpellHaste) - 1) * StatConversion.RATING_PER_SPELLHASTE 
                - CombatStats.HasteRating);

            dictValues.Add("Average Health", CombatStats.Health.ToString());
            dictValues.Add("Average Mana", CombatStats.Mana.ToString());
            dictValues.Add("Average Stamina", CombatStats.Stamina.ToString());
            dictValues.Add("Average Intellect", CombatStats.Intellect.ToString());
            dictValues.Add("Average Spell Power", CombatStats.SpellPower.ToString());
            dictValues.Add("Average Hit Rating", CombatStats.HitRating.ToString());
            dictValues.Add("Average Crit Rating", CombatStats.CritRating.ToString());
            dictValues.Add("Average Haste Rating", CombatStats.HasteRating.ToString() + "*" + CombatUntilGCDCap + " Haste Rating until GCD cap\n" + CombatUntilLBCap + " Haste Rating until LB cap");
            dictValues.Add("Average Mana Regen", Math.Round(ManaRegenInFSR).ToString() + " / " + Math.Round(ManaRegenOutFSR) + " + " + Math.Round(ReplenishMP5).ToString() + "*All values are Mana per 5 seconds.\nMP5 while casting / MP5 while not casting + Replenishment");

            dictValues.Add("Lightning Bolt", Math.Round(LightningBolt.MinHit).ToString() + "-" + Math.Round(LightningBolt.MaxHit).ToString() + " / " + Math.Round(LightningBolt.MinCrit).ToString() + "-" + Math.Round(LightningBolt.MaxCrit).ToString() + "*Mana cost: " + Math.Round(LightningBolt.ManaCost).ToString() + "\nCrit chance: " + Math.Round(100f * LightningBolt.CritChance, 2).ToString() + " %\nMiss chance: " + Math.Round(100f * LightningBolt.MissChance, 2).ToString() + " %\nCast time: " + Math.Round(LightningBolt.CastTime, 2) + " sec.\nClearcast uptime: " + Math.Round(100f * ClearCast_LightningBolt, 2).ToString() + " %");
            dictValues.Add("Chain Lightning", Math.Round(ChainLightning.MinHit).ToString() + "-" + Math.Round(ChainLightning.MaxHit).ToString() + " / " + Math.Round(ChainLightning.MinCrit).ToString() + "-" + Math.Round(ChainLightning.MaxCrit).ToString() + "*Targets: " + (((ChainLightning)ChainLightning).AdditionalTargets+1) + "\nMana cost: " + Math.Round(ChainLightning.ManaCost).ToString() + "\nCrit chance: " + Math.Round(100f * ChainLightning.CritChance, 2).ToString() + " %\nMiss chance: " + Math.Round(100f * ChainLightning.MissChance, 2).ToString() + " %\nCast time: " + Math.Round(ChainLightning.CastTime, 2) + " sec.");
            dictValues.Add("Lava Burst", Math.Round(LavaBurst.MinHit).ToString() + "-" + Math.Round(LavaBurst.MaxHit).ToString() + " / " + Math.Round(LavaBurst.MinCrit).ToString() + "-" + Math.Round(LavaBurst.MaxCrit).ToString() + "*Mana cost: " + Math.Round(LavaBurst.ManaCost).ToString() + "\nCrit chance: " + Math.Round(100f * LavaBurst.CritChance, 2).ToString() + " %\nMiss chance: " + Math.Round(100f * LavaBurst.MissChance, 2).ToString() + " %\nCast time: " + Math.Round(LavaBurst.CastTime, 2) + " sec.\nClearcast uptime: " + Math.Round(100f * ClearCast_LavaBurst, 2).ToString() + " %");
            dictValues.Add("Flame Shock", Math.Round(FlameShock.AvgHit).ToString() + " / " + Math.Round(FlameShock.AvgCrit).ToString() + " + " + Math.Round(FlameShock.PeriodicTick).ToString() + " (dot)*Duration: " + FlameShock.Duration + "\nTicks every " + Math.Round(FlameShock.PeriodicTickTime, 2).ToString()+ "s\nMana cost: " + Math.Round(FlameShock.ManaCost).ToString() + "\nCrit chance: " + Math.Round(100f * FlameShock.CritChance, 2).ToString() + " %\nMiss chance: " + Math.Round(100f * FlameShock.MissChance, 2).ToString() + " %\nGCD: " + Math.Round(FlameShock.CastTime, 2) + " sec.\nClearcast uptime: " + Math.Round(100f * ClearCast_FlameShock, 2).ToString() + " %");
            dictValues.Add("Earth Shock", Math.Round(EarthShock.MinHit).ToString() + "-" + Math.Round(EarthShock.MaxHit).ToString() + " / " + Math.Round(EarthShock.MinCrit).ToString() + "-" + Math.Round(EarthShock.MaxCrit).ToString() + "*Mana cost: " + Math.Round(EarthShock.ManaCost).ToString() + "\nCrit chance: " + Math.Round(100f * EarthShock.CritChance, 2).ToString() + " %\nMiss chance: " + Math.Round(100f * EarthShock.MissChance, 2).ToString() + " %\nGCD: " + Math.Round(EarthShock.CastTime, 2) + " sec.\n");
            dictValues.Add("Frost Shock", Math.Round(FrostShock.MinHit).ToString() + "-" + Math.Round(FrostShock.MaxHit).ToString() + " / " + Math.Round(FrostShock.MinCrit).ToString() + "-" + Math.Round(FrostShock.MaxCrit).ToString() + "*Mana cost: " + Math.Round(FrostShock.ManaCost).ToString() + "\nCrit chance: " + Math.Round(100f * FrostShock.CritChance, 2).ToString() + " %\nMiss chance: " + Math.Round(100f * FrostShock.MissChance, 2).ToString() + " %\nGCD: " + Math.Round(FrostShock.CastTime, 2) + " sec.\n");
            dictValues.Add("Fire Nova", Math.Round(FireNova.MinHit).ToString() + "-" + Math.Round(FireNova.MaxHit).ToString() + " / " + Math.Round(FireNova.MinCrit).ToString() + "-" + Math.Round(FireNova.MaxCrit).ToString() + "*Targets: " + (((FireNova)FireNova).AdditionalTargets+1) + "\nMana cost: " + Math.Round(FireNova.ManaCost).ToString() + "\nCrit chance: " + Math.Round(100f * FireNova.CritChance, 2).ToString() + " %\nMiss chance: " + Math.Round(100f * FireNova.MissChance, 2).ToString() + " %\nGCD: " + Math.Round(FireNova.CastTime, 2) + " sec.\n");
            dictValues.Add("Searing Totem", Math.Round(SearingTotem.PeriodicTick).ToString() + "*Mana cost: " + Math.Round(SearingTotem.ManaCost).ToString() + "\nCrit chance: " + Math.Round(100f*SearingTotem.CritChance, 2).ToString() + " %\nMiss chance: " + Math.Round(100f*SearingTotem.MissChance, 2).ToString() + " %\nGCD: " + Math.Round(SearingTotem.CastTime,2).ToString() + " sec.\n");
            dictValues.Add("Magma Totem", Math.Round(MagmaTotem.PeriodicTick).ToString() + "*Targets: " + (((MagmaTotem)MagmaTotem).AdditionalTargets+1) + "\nMana cost: " + Math.Round(MagmaTotem.ManaCost).ToString() + "\nCrit chance: " + Math.Round(100f * MagmaTotem.CritChance, 2).ToString() + " %\nMiss chance: " + Math.Round(100f * MagmaTotem.MissChance, 2).ToString() + " %\nGCD: " + Math.Round(MagmaTotem.CastTime, 2).ToString() + " sec.\n");

            dictValues.Add("Simulation", Math.Round(RotationDPS).ToString() + ((Math.Abs(RotationDPS - TotalDPS) >= 1) ? (" (" + Math.Round(TotalDPS).ToString() + ")") : "") + " DPS*OOM after " + Math.Round(TimeToOOM).ToString() + " sec.\nDPS until OOM: " + Math.Round(RotationDPS).ToString() + "\nMPS until OOM: " + Math.Round(RotationMPS).ToString() + "\nCast vs regen fraction after OOM: " + Math.Round(CastRegenFraction, 4).ToString() + "\n" + Math.Round(60f * CastsPerSecond, 1).ToString() + " casts per minute\n" + Math.Round(60f * CritsPerSecond, 1).ToString() + " crits per minute\n" + Math.Round(60f * MissesPerSecond, 1).ToString() + " misses per minute\n" + Math.Round(60f * LvBPerSecond, 1).ToString() + " Lava Bursts per minute\n" + Math.Round(60f * FSPerSecond, 1).ToString() + " Flame Shocks per minute\n" + Math.Round(60f * LBPerSecond, 1).ToString() + " Lightning Bolts per minute\n" + Math.Round(60f * LatencyPerSecond, 1).ToString() + " seconds lost to latency per minute\n");
            dictValues.Add("Rotation", Rotation + "*" + RotationDetails);

            return dictValues;
        }
    }

    public class ComparisonCalculationElemental : ComparisonCalculationBase
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

        public float BurstPoints
        {
            get { return _subPoints[0]; }
            set { _subPoints[0] = value; }
        }

        public float SustainedPoints
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
            return string.Format("{0}: ({1}O {2}Burst {3}Sustained)", Name, Math.Round(OverallPoints), Math.Round(BurstPoints), Math.Round(SustainedPoints));
        }
    }

    public static class Constants
    {
        // Source: http://www.wowwiki.com/Base_mana
        public static float BaseMana = 4396;
    }
}
