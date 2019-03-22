using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

namespace Rawr.ProtWarr
{
    [Rawr.Calculations.RawrModelInfo("ProtWarr", "Ability_Warrior_DefensiveStance", CharacterClass.Warrior)]
    public class CalculationsProtWarr : CalculationsBase
    {
        public override List<GemmingTemplate> DefaultGemmingTemplates
        {
            get
            {
                // Relevant Gem IDs for ProtWarr
                // Red
                int[] bold = { 39900, 39996, 40111, 42142 };        // Strength
                int[] delicate = { 39905, 39997, 40112, 42143 };    // Agility
                int[] subtle = { 39907, 40000, 40115, 42151 };      // Dodge
                int[] flashing = { 39908, 40001, 40116, 42152 };    // Parry
                int[] precise = { 39910, 40003, 40118, 42154 };     // Expertise

                // Purple
                int[] shifting = { 39935, 40023, 40130, 40130 };    // Agility + Stamina
                int[] sovereign = { 39934, 40022, 40129, 40129 };   // Strength + Stamina
                int[] regal = { 39938, 40031, 40138, 40138 };       // Dodge + Stamina
                int[] defender = { 39939, 40032, 40139, 40139 };    // Parry + Stamina
                int[] guardian = { 39940, 40034, 40141, 40141 };    // Expertise + Stamina

                // Blue
                int[] solid = { 39919, 40008, 40119, 36767 };       // Stamina

                // Green
                int[] enduring = { 39976, 40089, 40167, 40167 };    // Defense + Stamina

                // Yellow
                int[] thick = { 39916, 40015, 40126, 42157 };       // Defense

                // Orange
                int[] etched = { 39948, 40038, 40143, 40143 };      // Strength + Hit
                int[] champion = { 39949, 40039, 40144, 40144 };    // Strength + Defense
                int[] stalwart = { 39964, 40056, 40160, 40160 };    // Dodge + Defense
                int[] glimmering = { 39965, 40057, 40161, 40161 };  // Parry + Defense
                int[] accurate = { 39966, 40058, 40162, 40162 };    // Expertise + Hit
                int[] resolute = { 39967, 40059, 40163, 40163 };    // Expertise + Defense

                // Meta
                int[] austere = { 41380 };

                string[] groupNames = new string[] { "Uncommon", "Rare", "Epic", "Jeweler" };
                int[,][] gemmingTemplates = new int[,][]
                {
                    //Red       Yellow      Blue        Prismatic   Meta
                    { thick,    thick,      thick,      thick,      austere },  // Max Avoidance (Defense)
                    { stalwart, enduring,   enduring,   enduring,   austere },  // Avoidance Heavy (Dodge/Defense)
                    { flashing, glimmering, defender,   flashing,   austere },  // Avoidance Heavy (Parry)
                    { subtle,   stalwart,   regal,      subtle,     austere },  // Avoidance Heavy (Dodge)
                    { defender, enduring,   solid,      solid,      austere },  // Balanced Avoidance (Parry)
                    { regal,    enduring,   solid,      solid,      austere },  // Balanced Avoidance (Dodge)
                    { guardian, enduring,   solid,      solid,      austere },  // Balanced TPS
                    { solid,    solid,      solid,      solid,      austere },  // Max Health
                };

                // Generate List of Gemming Templates
                // groupNames[] index starts at 1 to ignore Uncommon gems currently
                List<GemmingTemplate> gemmingTemplate = new List<GemmingTemplate>();
                for (int j = 1; j <= groupNames.GetUpperBound(0); j++)
                {
                    for (int i = 0; i <= gemmingTemplates.GetUpperBound(0); i++)
                    {
                        gemmingTemplate.Add(new GemmingTemplate()
                        {
                            Model = "ProtWarr",
                            Group = groupNames[j],
                            RedId = gemmingTemplates[i, 0][j],
                            YellowId = gemmingTemplates[i, 1][j],
                            BlueId = gemmingTemplates[i, 2][j],
                            PrismaticId = gemmingTemplates[i, 3][j],
                            MetaId = gemmingTemplates[i, 4][0],
                            Enabled = j == 2
                        });
                    }
                }
                return gemmingTemplate;
            }
        }

        #region Variables and Properties

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
                    _calculationOptionsPanel = new CalculationOptionsPanelProtWarr();
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
					"Base Stats:Health",
                    "Base Stats:Strength",
                    "Base Stats:Agility",
                    "Base Stats:Stamina",

                    "Defensive Stats:Armor",
                    "Defensive Stats:Defense",
                    "Defensive Stats:Dodge",
                    "Defensive Stats:Parry",
                    "Defensive Stats:Block",
                    "Defensive Stats:Miss",
                    "Defensive Stats:Block Value",
                    "Defensive Stats:Resilience",
                    "Defensive Stats:Chance to be Crit",
                    "Defensive Stats:Guaranteed Reduction",
					"Defensive Stats:Avoidance",
					"Defensive Stats:Total Mitigation",
                    "Defensive Stats:Attacker Speed",
					"Defensive Stats:Damage Taken",

                    "Offensive Stats:Weapon Speed",
                    "Offensive Stats:Attack Power",
                    "Offensive Stats:Hit",
					"Offensive Stats:Haste",
					"Offensive Stats:Armor Penetration",
					"Offensive Stats:Crit",
                    "Offensive Stats:Expertise",
                    // Never really used in current WoW itemization, just taking up space
					//"Offensive Stats:Weapon Damage",
                    "Offensive Stats:Missed Attacks",
                    "Offensive Stats:Total Damage/sec",
                    "Offensive Stats:Total Threat/sec",
                    "Offensive Stats:Rotation",

                    "Resistances:Nature Resist",
					"Resistances:Fire Resist",
					"Resistances:Frost Resist",
					"Resistances:Shadow Resist",
					"Resistances:Arcane Resist",
                    "Complex Stats:Ranking Mode*The currently selected ranking mode. Ranking modes can be changed in the Options tab.",
					@"Complex Stats:Overall Points*Overall Points are a sum of Mitigation and Survival Points. 
Overall is typically, but not always, the best way to rate gear. 
For specific encounters, closer attention to Mitigation and 
Survival Points individually may be important.",
					@"Complex Stats:Mitigation Points*Mitigation Points represent the amount of damage you mitigate, 
on average, through armor mitigation and avoidance. It is directly 
relational to your Damage Taken. Ideally, you want to maximize 
Mitigation Points, while maintaining 'enough' Survival Points 
(see Survival Points). If you find yourself dying due to healers 
running OOM, or being too busy healing you and letting other 
raid members die, then focus on Mitigation Points.",
					@"Complex Stats:Survival Points*Survival Points represents the total raw physical damage 
(pre-avoidance/block) you can take before dying. Unlike 
Mitigation Points, you should not attempt to maximize this, 
but rather get 'enough' of it, and then focus on Mitigation. 
'Enough' can vary greatly by fight and by your healers.
If you find that you are being killed by burst damage,
focus on Survival Points.",
                    @"Complex Stats:Threat Points*Threat Points represents the average between unlimited
threat and limited threat scaled by the threat scale.",
                    "Complex Stats:Nature Survival",
                    "Complex Stats:Fire Survival",
                    "Complex Stats:Frost Survival",
                    "Complex Stats:Shadow Survival",
                    "Complex Stats:Arcane Survival",
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
                    "% Total Mitigation",
					"% Guaranteed Reduction",
                    "% Chance to be Crit",
                    "% Avoidance",
                    "% Avoidance+Block",
                    "Block Value",
                    "Threat/sec",
                    "% Chance to be Avoided", 
                    "% Chance to be Dodged",
                    "% Chance to be Parried",
                    "% Chance to Miss",
                    "Burst Time", 
                    "Nature Survival",
                    "Fire Survival",
                    "Frost Survival",
                    "Shadow Survival",
                    "Arcane Survival",
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
                    "Ability Damage",
                    "Ability Threat",
                    //"Rotation Damage",
                    //"Rotation Threat",
					"Combat Table",
					"Item Budget",
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
                    _subPointNameColors.Add("Survival", System.Windows.Media.Colors.Blue);
                    _subPointNameColors.Add("Mitigation", System.Windows.Media.Colors.Red);
                    _subPointNameColors.Add("Threat", System.Windows.Media.Colors.Green);
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
                    _subPointNameColors.Add("Survival", System.Drawing.Color.FromArgb(255, 0, 0, 255));
                    _subPointNameColors.Add("Mitigation", System.Drawing.Color.FromArgb(255, 255, 0, 0));
                    _subPointNameColors.Add("Threat", System.Drawing.Color.FromArgb(255, 0, 128, 0));
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
                        ItemType.Plate,
                        ItemType.Bow,
                        ItemType.Crossbow,
                        ItemType.Gun,
                        ItemType.Thrown,
                        ItemType.Arrow,
                        ItemType.Bullet,
                        ItemType.FistWeapon,
                        ItemType.Dagger,
                        ItemType.OneHandAxe,
                        ItemType.OneHandMace,
                        ItemType.OneHandSword,
                        ItemType.Shield
					});
                }
                return _relevantItemTypes;
            }
        }

        public override void SetDefaults(Character character)
        {
            character.ActiveBuffsAdd(("Strength of Earth Totem"));
            character.ActiveBuffsAdd(("Enhancing Totems (Agility/Strength)"));
            character.ActiveBuffsAdd(("Devotion Aura"));
            character.ActiveBuffsAdd(("Improved Devotion Aura (Armor)"));
            character.ActiveBuffsAdd(("Inspiration"));
            character.ActiveBuffsAdd(("Blessing of Might"));
            character.ActiveBuffsAdd(("Improved Blessing of Might"));
            character.ActiveBuffsAdd(("Unleashed Rage"));
            character.ActiveBuffsAdd(("Sanctified Retribution"));
            character.ActiveBuffsAdd(("Renewed Hope"));
            character.ActiveBuffsAdd(("Swift Retribution"));
            character.ActiveBuffsAdd(("Commanding Shout"));
            character.ActiveBuffsAdd(("Commanding Presence (Health)"));
            character.ActiveBuffsAdd(("Leader of the Pack"));
            character.ActiveBuffsAdd(("Windfury Totem"));
            character.ActiveBuffsAdd(("Improved Windfury Totem"));
            character.ActiveBuffsAdd(("Power Word: Fortitude"));
            character.ActiveBuffsAdd(("Improved Power Word: Fortitude"));
            character.ActiveBuffsAdd(("Mark of the Wild"));
            character.ActiveBuffsAdd(("Improved Mark of the Wild"));
            character.ActiveBuffsAdd(("Blessing of Kings"));
            character.ActiveBuffsAdd(("Blessing of Kings (Str/Sta Bonus)"));
            character.ActiveBuffsAdd(("Sunder Armor"));
            character.ActiveBuffsAdd(("Faerie Fire"));
            character.ActiveBuffsAdd(("Trauma"));
            character.ActiveBuffsAdd(("Heart of the Crusader"));
            character.ActiveBuffsAdd(("Insect Swarm"));
            character.ActiveBuffsAdd(("Thunder Clap"));
            character.ActiveBuffsAdd(("Improved Thunder Clap"));
            character.ActiveBuffsAdd(("Flask of Stoneblood"));
            if (character.PrimaryProfession == Profession.Alchemy || character.SecondaryProfession == Profession.Alchemy)
                character.ActiveBuffsAdd(("Flask of Stoneblood (Mixology)"));
            character.ActiveBuffsAdd(("Fish Feast"));
        }

        private static List<string> _relevantGlyphs;
        public override List<string> GetRelevantGlyphs()
        {
            if (_relevantGlyphs == null)
            {
                _relevantGlyphs = new List<string>();
                _relevantGlyphs.Add("Glyph of Barbaric Insults");
                _relevantGlyphs.Add("Glyph of Blocking");
                _relevantGlyphs.Add("Glyph of Cleaving");
                _relevantGlyphs.Add("Glyph of Devastate");
                _relevantGlyphs.Add("Glyph of Enraged Regeneration");
                _relevantGlyphs.Add("Glyph of Heroic Strike");
                _relevantGlyphs.Add("Glyph of Intervene");
                _relevantGlyphs.Add("Glyph of Last Stand");
                _relevantGlyphs.Add("Glyph of Rapid Charge");
                _relevantGlyphs.Add("Glyph of Rending");
                _relevantGlyphs.Add("Glyph of Resonating Power");
                _relevantGlyphs.Add("Glyph of Revenge");
                _relevantGlyphs.Add("Glyph of Shield Wall");
                _relevantGlyphs.Add("Glyph of Shockwave");
                _relevantGlyphs.Add("Glyph of Spell Reflection");
                _relevantGlyphs.Add("Glyph of Sunder Armor");
                _relevantGlyphs.Add("Glyph of Taunt");
                _relevantGlyphs.Add("Glyph of Vigilance");

                _relevantGlyphs.Add("Glyph of Battle");
                _relevantGlyphs.Add("Glyph of Bloodrage");
                _relevantGlyphs.Add("Glyph of Charge");
                _relevantGlyphs.Add("Glyph of Command");
                _relevantGlyphs.Add("Glyph of Mocking Blow");
                _relevantGlyphs.Add("Glyph of Thunder Clap");
            }
            return _relevantGlyphs;
        }

        public override CharacterClass TargetClass { get { return CharacterClass.Warrior; } }
        public override ComparisonCalculationBase CreateNewComparisonCalculation() { return new ComparisonCalculationProtWarr(); }
        public override CharacterCalculationsBase CreateNewCharacterCalculations() { return new CharacterCalculationsProtWarr(); }

        public override ICalculationOptionBase DeserializeDataObject(string xml)
        {
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsProtWarr));
            System.IO.StringReader reader = new System.IO.StringReader(xml);
            CalculationOptionsProtWarr calcOpts = serializer.Deserialize(reader) as CalculationOptionsProtWarr;
            return calcOpts;
        }
        #endregion

        public override CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, bool referenceCalculation, bool significantChange, bool needsDisplayCalculations)
        {
            CharacterCalculationsProtWarr calculatedStats = new CharacterCalculationsProtWarr();
            CalculationOptionsProtWarr options = character.CalculationOptions as CalculationOptionsProtWarr;
            Stats stats = GetCharacterStats(character, additionalItem, options);

            DefendModel dm = new DefendModel(character, stats, options);
            AttackModel am = new AttackModel(character, stats, options, AttackModelMode.Optimal);

            if (needsDisplayCalculations)
            {
                calculatedStats.Defense = (float)Math.Floor(stats.Defense + StatConversion.GetDefenseFromRating(stats.DefenseRating, CharacterClass.Warrior));
                calculatedStats.CritReduction = Lookup.AvoidanceChance(character, stats, HitResult.Crit, options.TargetLevel);
                calculatedStats.DefenseRatingNeeded = StatConversion.GetDefenseRatingNeeded(character, stats, options.TargetLevel);
                calculatedStats.ArmorReduction = Lookup.ArmorReduction(character, stats, options.TargetLevel);

                calculatedStats.BaseAttackerSpeed = options.BossAttackSpeed;
                calculatedStats.AttackerSpeed = dm.ParryModel.BossAttackSpeed;
                calculatedStats.DamageTakenPerHit = dm.DamagePerHit;
                calculatedStats.DamageTakenPerBlock = dm.DamagePerBlock;
                calculatedStats.DamageTakenPerCritBlock = dm.DamagePerCritBlock;
                calculatedStats.DamageTakenPerCrit = dm.DamagePerCrit;
                calculatedStats.DamageTaken = dm.DamagePerSecond;

                calculatedStats.ArcaneReduction = (1.0f - Lookup.MagicReduction(character, stats, DamageType.Arcane, options.TargetLevel));
                calculatedStats.FireReduction = (1.0f - Lookup.MagicReduction(character, stats, DamageType.Fire, options.TargetLevel));
                calculatedStats.FrostReduction = (1.0f - Lookup.MagicReduction(character, stats, DamageType.Frost, options.TargetLevel));
                calculatedStats.NatureReduction = (1.0f - Lookup.MagicReduction(character, stats, DamageType.Nature, options.TargetLevel));
                calculatedStats.ShadowReduction = (1.0f - Lookup.MagicReduction(character, stats, DamageType.Shadow, options.TargetLevel));

                calculatedStats.Crit = am.Abilities[Ability.None].AttackTable.Critical;
                calculatedStats.Expertise = Lookup.BonusExpertisePercentage(character, stats);
                calculatedStats.Haste = Lookup.BonusHastePercentage(character, stats);
                calculatedStats.ArmorPenetration = Lookup.BonusArmorPenetrationPercentage(character, stats);
                calculatedStats.WeaponSpeed = Lookup.WeaponSpeed(character, stats);
            }

            calculatedStats.TargetLevel = options.TargetLevel;
            calculatedStats.ActiveBuffs = new List<Buff>(character.ActiveBuffs);
            calculatedStats.Abilities = am.Abilities;
            calculatedStats.BasicStats = stats;

            calculatedStats.Miss = dm.DefendTable.Miss;
            calculatedStats.Dodge = dm.DefendTable.Dodge;
            calculatedStats.Parry = dm.DefendTable.Parry;
            calculatedStats.Block = dm.DefendTable.Block;
            calculatedStats.DodgePlusMissPlusParry = calculatedStats.Dodge + calculatedStats.Miss + calculatedStats.Parry;
            calculatedStats.DodgePlusMissPlusParryPlusBlock = calculatedStats.DodgePlusMissPlusParry + calculatedStats.Block;
            calculatedStats.BlockValue = stats.BlockValue;
            calculatedStats.CritVulnerability = dm.DefendTable.Critical;
            calculatedStats.GuaranteedReduction = dm.GuaranteedReduction;
            calculatedStats.TotalMitigation = dm.Mitigation;

            calculatedStats.ArcaneSurvivalPoints = stats.Health / Lookup.MagicReduction(character, stats, DamageType.Arcane, options.TargetLevel);
            calculatedStats.FireSurvivalPoints = stats.Health / Lookup.MagicReduction(character, stats, DamageType.Fire, options.TargetLevel);
            calculatedStats.FrostSurvivalPoints = stats.Health / Lookup.MagicReduction(character, stats, DamageType.Frost, options.TargetLevel);
            calculatedStats.NatureSurvivalPoints = stats.Health / Lookup.MagicReduction(character, stats, DamageType.Nature, options.TargetLevel);
            calculatedStats.ShadowSurvivalPoints = stats.Health / Lookup.MagicReduction(character, stats, DamageType.Shadow, options.TargetLevel);

            calculatedStats.Hit = Lookup.BonusHitPercentage(character, stats);
            calculatedStats.AvoidedAttacks = am.Abilities[Ability.None].AttackTable.AnyMiss;
            calculatedStats.DodgedAttacks = am.Abilities[Ability.None].AttackTable.Dodge;
            calculatedStats.ParriedAttacks = am.Abilities[Ability.None].AttackTable.Parry;
            calculatedStats.MissedAttacks = am.Abilities[Ability.None].AttackTable.Miss;

            calculatedStats.HeroicStrikeFrequency = options.HeroicStrikeFrequency;
            calculatedStats.ThreatPerSecond = am.ThreatPerSecond;
            calculatedStats.ThreatModelName = am.ShortName;
            calculatedStats.ThreatModel = am.Name + "\n" + am.Description;
            calculatedStats.TotalDamagePerSecond = am.DamagePerSecond;

            calculatedStats.TankPoints = dm.TankPoints;
            calculatedStats.BurstTime = dm.BurstTime;
            calculatedStats.RankingMode = options.RankingMode;
            calculatedStats.ThreatPoints = (options.ThreatScale * am.ThreatPerSecond);
            switch (options.RankingMode)
            {
                case 2:
                    // Tank Points Mode
                    calculatedStats.SurvivalPoints = (dm.EffectiveHealth);
                    calculatedStats.MitigationPoints = (dm.TankPoints - dm.EffectiveHealth);
                    calculatedStats.ThreatPoints *= 3.0f;
                    break;
                case 3:
                    // Burst Time Mode
                    float threatScale = Convert.ToSingle(Math.Pow(Convert.ToDouble(options.BossAttackValue) / 25000.0d, 4));
                    calculatedStats.SurvivalPoints = (dm.BurstTime * 100.0f);
                    calculatedStats.MitigationPoints = 0.0f;
                    calculatedStats.ThreatPoints = (calculatedStats.ThreatPoints / threatScale) * 2.0f;
                    break;
                case 4:
                    // Damage Output Mode
                    calculatedStats.SurvivalPoints = 0.0f;
                    calculatedStats.MitigationPoints = 0.0f;
                    calculatedStats.ThreatPoints = calculatedStats.TotalDamagePerSecond;
                    break;
                default:
                    // Mitigation Scale Mode
                    calculatedStats.SurvivalPoints = (dm.EffectiveHealth) / 10.0f;
                    calculatedStats.MitigationPoints = dm.Mitigation * options.BossAttackValue * options.MitigationScale * 10.0f;
                    calculatedStats.ThreatPoints /= 10.0f;
                    break;
            }
            //calculatedStats.OverallPoints = calculatedStats.MitigationPoints + calculatedStats.SurvivalPoints + calculatedStats.ThreatPoints;

            return calculatedStats;
        }

        public override Stats GetCharacterStats(Character character, Item additionalItem)
        {
            CalculationOptionsProtWarr options = character.CalculationOptions as CalculationOptionsProtWarr;
            return GetCharacterStats(character, additionalItem, options);
        }

        public Stats GetCharacterStats(Character character, Item additionalItem, CalculationOptionsProtWarr options)
        {
            WarriorTalents talents = character.WarriorTalents;
            //WarriorTalentsCata talentsCata = character.WarriorTalentsCata;
            Stats statsTotal = new Stats();
            
            // Items and Buffs
            Stats statsItemsBuffs = new Stats();
            AccumulateItemStats(statsItemsBuffs, character, additionalItem);
            AccumulateBuffsStats(statsItemsBuffs, character.ActiveBuffs);
            statsTotal.Accumulate(statsItemsBuffs);

            // Race Stats
            Stats statsRace  = BaseStats.GetBaseStats(character.Level, CharacterClass.Warrior, character.Race);
            statsRace.Expertise += BaseStats.GetRacialExpertise(character, ItemSlot.MainHand);
            statsTotal.Accumulate(statsRace);

            // Talents
            Stats statsTalents = new Stats()
            {
                Parry = talents.Deflection * 0.01f,
                Dodge = talents.Anticipation * 0.01f,
                Block = talents.ShieldSpecialization * 0.01f,
                BonusBlockValueMultiplier = talents.ShieldMastery * 0.15f + (talents.GlyphOfBlocking ? 0.1f : 0.0f),
                PhysicalCrit = talents.Cruelty * 0.01f +
                    ((character.MainHand != null && (character.MainHand.Type == ItemType.OneHandAxe))
                        ? talents.PoleaxeSpecialization * 0.01f : 0.0f),
                BonusCritMultiplier =
                    ((character.MainHand != null && (character.MainHand.Type == ItemType.OneHandAxe))
                        ? talents.PoleaxeSpecialization * 0.01f : 0.0f),
                BonusDamageMultiplier =
                    ((character.MainHand != null &&
                        (character.MainHand.Type == ItemType.OneHandAxe ||
                        character.MainHand.Type == ItemType.OneHandMace ||
                        character.MainHand.Type == ItemType.OneHandSword ||
                        character.MainHand.Type == ItemType.Dagger ||
                        character.MainHand.Type == ItemType.FistWeapon))
                            ? talents.OneHandedWeaponSpecialization * 0.02f : 0f),
                BonusStaminaMultiplier = talents.Vitality * 0.03f + talents.StrengthOfArms * 0.02f,
                BonusStrengthMultiplier = talents.Vitality * 0.02f + talents.StrengthOfArms * 0.02f,
                Expertise = talents.Vitality * 2.0f + talents.StrengthOfArms * 2.0f,
                BonusShieldSlamDamage = talents.GagOrder * 0.05f,
                DevastateCritIncrease = talents.SwordAndBoard * 0.05f,
                BaseArmorMultiplier = talents.Toughness * 0.02f,
                PhysicalHaste = talents.BloodFrenzy * 0.03f,
                PhysicalHit = talents.Precision * 0.01f,

            };
            statsTotal.Accumulate(statsTalents);

            // Base Stats
            statsTotal.BaseAgility = statsRace.Agility + statsTalents.Agility;
            statsTotal.Stamina = (float)Math.Floor((statsRace.Stamina + statsTalents.Stamina) * (1.0f + statsTotal.BonusStaminaMultiplier));
            statsTotal.Stamina += (float)Math.Floor(statsItemsBuffs.Stamina * (1.0f + statsTotal.BonusStaminaMultiplier));
            statsTotal.Strength = (float)Math.Floor((statsRace.Strength + statsTalents.Strength) * (1.0f + statsTotal.BonusStrengthMultiplier));
            statsTotal.Strength += (float)Math.Floor(statsItemsBuffs.Strength * (1.0f + statsTotal.BonusStrengthMultiplier));
            statsTotal.Agility = (float)Math.Floor((statsRace.Agility + statsTalents.Agility) * (1.0f + statsTotal.BonusAgilityMultiplier));
            statsTotal.Agility += (float)Math.Floor(statsItemsBuffs.Agility * (1.0f + statsTotal.BonusAgilityMultiplier));
            statsTotal.Health += StatConversion.GetHealthFromStamina(statsTotal.Stamina, CharacterClass.Warrior);
            statsTotal.Health = (float)Math.Floor(statsTotal.Health * (1.0f + statsTotal.BonusHealthMultiplier));

            // Calculate Procs and Special Effects
            statsTotal.Accumulate(GetSpecialEffectStats(character, statsTotal, options));

            // Defensive Stats
            statsTotal.Armor = (float)Math.Ceiling(statsTotal.Armor * (1.0f + statsTotal.BaseArmorMultiplier));
            statsTotal.BonusArmor += statsTotal.Agility * 2.0f;
            statsTotal.Armor += (float)Math.Ceiling(statsTotal.BonusArmor * (1.0f + statsTotal.BonusArmorMultiplier));

            statsTotal.BlockValue += (float)Math.Floor(StatConversion.GetBlockValueFromStrength(statsTotal.Strength, CharacterClass.Warrior) - 10.0f);
            statsTotal.BlockValue = (float)Math.Floor(statsTotal.BlockValue * (1 + statsTotal.BonusBlockValueMultiplier));

            statsTotal.NatureResistance += statsTotal.NatureResistanceBuff;
            statsTotal.FireResistance += statsTotal.FireResistanceBuff;
            statsTotal.FrostResistance += statsTotal.FrostResistanceBuff;
            statsTotal.ShadowResistance += statsTotal.ShadowResistanceBuff;
            statsTotal.ArcaneResistance += statsTotal.ArcaneResistanceBuff;

            // Highest Stat Effects
            if(statsTotal.Strength > statsTotal.Agility)
                statsTotal.Strength += (float)Math.Floor((statsTotal.HighestStat + statsTotal.Paragon) * (1.0f + statsTotal.BonusStrengthMultiplier));
            else
                statsTotal.Agility += (float)Math.Floor((statsTotal.HighestStat + statsTotal.Paragon) * (1.0f + statsTotal.BonusAgilityMultiplier));

            // Final Attack Power
            statsTotal.AttackPower += statsTotal.Strength * 2.0f + (float)Math.Floor(talents.ArmoredToTheTeeth * statsTotal.Armor / 108.0f);
            statsTotal.AttackPower = (float)Math.Floor(statsTotal.AttackPower * (1.0f + statsTotal.BonusAttackPowerMultiplier));

            return statsTotal;
        }

        private Stats GetSpecialEffectStats(Character character, Stats stats, CalculationOptionsProtWarr options)
        {
            Stats statsSpecialEffects = new Stats();

            float weaponSpeed = 1.0f;
            if (character.MainHand != null)
                weaponSpeed = character.MainHand.Speed;

            AttackModel am = new AttackModel(character, stats, options, AttackModelMode.Optimal);

            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                switch (effect.Trigger)
                {
                    case Trigger.Use:
                        effect.AccumulateAverageStats(statsSpecialEffects, 0.0f, 1.0f, weaponSpeed);
                        // Trial of the Crusader Stacking Use Effect Trinkets
                        foreach (SpecialEffect childEffect in effect.Stats.SpecialEffects())
                        {
                            if (childEffect.Trigger == Trigger.DamageTaken)
                            {
                                statsSpecialEffects.Accumulate(childEffect.Stats * (effect.GetAverageUptime(0.0f, 1.0f) *
                                    childEffect.GetAverageStackSize((1.0f / am.AttackerSwingsPerSecond), (am.AttackerHitsPerSecond / am.AttackerSwingsPerSecond), weaponSpeed, effect.Duration)));
                            }
                        }
                        break;
                    case Trigger.MeleeHit:
                    case Trigger.PhysicalHit:
                        effect.AccumulateAverageStats(statsSpecialEffects, (1.0f / am.WeaponAttacksPerSecond), (am.HitsPerSecond / am.WeaponAttacksPerSecond), weaponSpeed);
                        break;
                    case Trigger.MeleeCrit:
                    case Trigger.PhysicalCrit:
                        effect.AccumulateAverageStats(statsSpecialEffects, (1.0f / am.WeaponAttacksPerSecond), (am.CritsPerSecond / am.WeaponAttacksPerSecond), weaponSpeed);
                        break;
                    case Trigger.DoTTick:
                        if (character.WarriorTalents.DeepWounds > 0)
                            effect.AccumulateAverageStats(statsSpecialEffects, 2.0f, 1.0f, weaponSpeed);
                        break;
                    case Trigger.DamageDone:
                    case Trigger.DamageOrHealingDone:
                        effect.AccumulateAverageStats(statsSpecialEffects, (1.0f / am.WeaponAttacksPerSecond), (am.HitsPerSecond / am.WeaponAttacksPerSecond), weaponSpeed);
                        break;
                    case Trigger.DamageTaken:
                        effect.AccumulateAverageStats(statsSpecialEffects, (1.0f / am.AttackerSwingsPerSecond), (am.AttackerHitsPerSecond / am.AttackerSwingsPerSecond));
                        break;
                }
            }

            // Base Stats
            statsSpecialEffects.Stamina = (float)Math.Floor(statsSpecialEffects.Stamina * (1.0f + stats.BonusStaminaMultiplier));
            statsSpecialEffects.Strength = (float)Math.Floor(statsSpecialEffects.Strength * (1.0f + stats.BonusStrengthMultiplier));
            statsSpecialEffects.Agility = (float)Math.Floor(statsSpecialEffects.Agility * (1.0f + stats.BonusAgilityMultiplier));
            statsSpecialEffects.Health += (statsSpecialEffects.Stamina * 10.0f) + statsSpecialEffects.BattlemasterHealth;
            statsSpecialEffects.Health = (float)Math.Floor(statsSpecialEffects.Health * (1.0f + stats.BonusHealthMultiplier));

            return statsSpecialEffects;
        }

        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
        {
            CharacterCalculationsProtWarr calculations = GetCharacterCalculations(character) as CharacterCalculationsProtWarr;

            switch (chartName)
            {
                /*
                #region Rotation Damage and Threat
                case "Rotation Damage":
                case "Rotation Threat":
                    {
                        CalculationOptionsProtWarr options = character.CalculationOptions as CalculationOptionsProtWarr;
                        string[] rotations = Enum.GetNames(typeof(AttackModelMode));
                        // Optimal rotation will need to be ignored as it is outside the scope of this
                        ComparisonCalculationBase[] comparisons = new ComparisonCalculationBase[rotations.Length - 1];

                        int j = 0;
                        for (int i = 0; i < rotations.Length; i++)
                        {
                            if (rotations[i] != "Optimal")
                            {
                                AttackModel am = new AttackModel(character, calculations.BasicStats, options, (AttackModelMode)Enum.Parse(typeof(AttackModelMode), rotations[i]));
                                ComparisonCalculationProtWarr comparison = new ComparisonCalculationProtWarr();
                                comparison.Name = am.Name;

                                if (chartName == "Rotation Damage")
                                    comparison.ThreatPoints = am.DamagePerSecond;
                                else
                                    comparison.ThreatPoints = am.ThreatPerSecond;

                                comparisons[j] = comparison;
                                j++;
                            }
                        }
                        return comparisons;
                    }
                #endregion
                */
                #region Ability Damage and Threat
                case "Ability Damage":
                case "Ability Threat":
                    {
                        // Vigilance will need to be ignores as it is outside the scope of this
                        ComparisonCalculationBase[] comparisons = new ComparisonCalculationBase[calculations.Abilities.Count - 1];
                        // Index Deep Wounds for including in everything that can crit
                        AbilityModel DeepWounds = calculations.Abilities[Ability.DeepWounds];

                        int j = 0;
                        foreach (AbilityModel ability in calculations.Abilities)
                        {
                            if (ability.Ability != Ability.Vigilance)
                            {
                                ComparisonCalculationProtWarr comparison = new ComparisonCalculationProtWarr();
                                comparison.Name = ability.Name;

                                // Deep Wounds gets colored red to make the integrated parts easier to understand
                                if (ability.Ability == Ability.DeepWounds)
                                {
                                    if (chartName == "Ability Damage")
                                        comparison.MitigationPoints = ability.Damage;
                                    else
                                        comparison.MitigationPoints = ability.Threat;
                                }
                                else
                                {
                                    if (chartName == "Ability Damage")
                                    {
                                        comparison.ThreatPoints = ability.Damage;
                                        comparison.MitigationPoints = ability.CritPercentage * DeepWounds.Damage;
                                    }
                                    else
                                    {
                                        comparison.ThreatPoints = ability.Threat;
                                        comparison.MitigationPoints = ability.CritPercentage * DeepWounds.Threat;
                                    }
                                }

                                comparison.OverallPoints = comparison.SurvivalPoints + comparison.ThreatPoints + comparison.MitigationPoints;
                                comparisons[j] = comparison;
                                j++;
                            }
                        }
                        return comparisons;
                    }
                #endregion
                #region Combat Table
                case "Combat Table":
                    {
                        ComparisonCalculationProtWarr calcMiss = new ComparisonCalculationProtWarr();
                        ComparisonCalculationProtWarr calcDodge = new ComparisonCalculationProtWarr();
                        ComparisonCalculationProtWarr calcParry = new ComparisonCalculationProtWarr();
                        ComparisonCalculationProtWarr calcBlock = new ComparisonCalculationProtWarr();
                        ComparisonCalculationProtWarr calcCrit = new ComparisonCalculationProtWarr();
                        ComparisonCalculationProtWarr calcCrush = new ComparisonCalculationProtWarr();
                        ComparisonCalculationProtWarr calcHit = new ComparisonCalculationProtWarr();
                        if (calculations != null)
                        {
                            calcMiss.Name = "Miss";
                            calcDodge.Name = "Dodge";
                            calcParry.Name = "Parry";
                            calcBlock.Name = "Block";
                            calcCrit.Name = "Crit";
                            calcCrush.Name = "Crush";
                            calcHit.Name = "Hit";

                            calcMiss.OverallPoints = calcMiss.MitigationPoints = calculations.Miss * 100.0f;
                            calcDodge.OverallPoints = calcDodge.MitigationPoints = calculations.Dodge * 100.0f;
                            calcParry.OverallPoints = calcParry.MitigationPoints = calculations.Parry * 100.0f;
                            calcBlock.OverallPoints = calcBlock.MitigationPoints = calculations.Block * 100.0f;
                            calcCrit.OverallPoints = calcCrit.SurvivalPoints = calculations.CritVulnerability * 100.0f;
                            calcHit.OverallPoints = calcHit.SurvivalPoints = (1.0f - calculations.DodgePlusMissPlusParry - calculations.Block - calculations.CritVulnerability) * 100.0f;
                        }
                        return new ComparisonCalculationBase[] { calcMiss, calcDodge, calcParry, calcBlock, calcCrit, calcCrush, calcHit };
                    }
                #endregion
                #region Item Budget
                case "Item Budget":
                    CharacterCalculationsProtWarr calcBaseValue = GetCharacterCalculations(character) as CharacterCalculationsProtWarr;
                    CharacterCalculationsProtWarr calcDodgeValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { DodgeRating = 10f } }) as CharacterCalculationsProtWarr;
                    CharacterCalculationsProtWarr calcParryValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { ParryRating = 10f } }) as CharacterCalculationsProtWarr;
                    CharacterCalculationsProtWarr calcBlockValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { BlockRating = 10f } }) as CharacterCalculationsProtWarr;
                    CharacterCalculationsProtWarr calcHasteValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { HasteRating = 10f } }) as CharacterCalculationsProtWarr;
                    CharacterCalculationsProtWarr calcExpertiseValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { ExpertiseRating = 10f } }) as CharacterCalculationsProtWarr;
                    CharacterCalculationsProtWarr calcHitValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { HitRating = 10f } }) as CharacterCalculationsProtWarr;
                    CharacterCalculationsProtWarr calcBlockValueValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { BlockValue = (10f / 0.65f) * 2.0f } }) as CharacterCalculationsProtWarr;
                    CharacterCalculationsProtWarr calcHealthValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Health = (10f * 10f) / 0.667f } }) as CharacterCalculationsProtWarr;
                    CharacterCalculationsProtWarr calcResilValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Resilience = 10f } }) as CharacterCalculationsProtWarr;

                    //Differential Calculations for Agi
                    CharacterCalculationsProtWarr calcAtAdd = calcBaseValue;
                    float agiToAdd = 0f;
                    while (calcBaseValue.OverallPoints == calcAtAdd.OverallPoints && agiToAdd < 2)
                    {
                        agiToAdd += 0.01f;
                        calcAtAdd = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Agility = agiToAdd } }) as CharacterCalculationsProtWarr;
                    }

                    CharacterCalculationsProtWarr calcAtSubtract = calcBaseValue;
                    float agiToSubtract = 0f;
                    while (calcBaseValue.OverallPoints == calcAtSubtract.OverallPoints && agiToSubtract > -2)
                    {
                        agiToSubtract -= 0.01f;
                        calcAtSubtract = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Agility = agiToSubtract } }) as CharacterCalculationsProtWarr;
                    }
                    agiToSubtract += 0.01f;

                    ComparisonCalculationProtWarr comparisonAgi = new ComparisonCalculationProtWarr()
                    {
                        Name = "10 Agility",
                        OverallPoints = 10 * (calcAtAdd.OverallPoints - calcBaseValue.OverallPoints) / (agiToAdd - agiToSubtract),
                        MitigationPoints = 10 * (calcAtAdd.MitigationPoints - calcBaseValue.MitigationPoints) / (agiToAdd - agiToSubtract),
                        SurvivalPoints = 10 * (calcAtAdd.SurvivalPoints - calcBaseValue.SurvivalPoints) / (agiToAdd - agiToSubtract),
                        ThreatPoints = 10 * (calcAtAdd.ThreatPoints - calcBaseValue.ThreatPoints) / (agiToAdd - agiToSubtract)
                    };

                    //Differential Calculations for Str
                    calcAtAdd = calcBaseValue;
                    float strToAdd = 0f;
                    while (calcBaseValue.OverallPoints == calcAtAdd.OverallPoints && strToAdd < 2)
                    {
                        strToAdd += 0.01f;
                        calcAtAdd = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Strength = strToAdd } }) as CharacterCalculationsProtWarr;
                    }

                    calcAtSubtract = calcBaseValue;
                    float strToSubtract = 0f;
                    while (calcBaseValue.OverallPoints == calcAtSubtract.OverallPoints && strToSubtract > -2)
                    {
                        strToSubtract -= 0.01f;
                        calcAtSubtract = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Strength = strToSubtract } }) as CharacterCalculationsProtWarr;
                    }
                    strToSubtract += 0.01f;

                    ComparisonCalculationProtWarr comparisonStr = new ComparisonCalculationProtWarr()
                    {
                        Name = "10 Strength",
                        OverallPoints = 10 * (calcAtAdd.OverallPoints - calcBaseValue.OverallPoints) / (strToAdd - strToSubtract),
                        MitigationPoints = 10 * (calcAtAdd.MitigationPoints - calcBaseValue.MitigationPoints) / (strToAdd - strToSubtract),
                        SurvivalPoints = 10 * (calcAtAdd.SurvivalPoints - calcBaseValue.SurvivalPoints) / (strToAdd - strToSubtract),
                        ThreatPoints = 10 * (calcAtAdd.ThreatPoints - calcBaseValue.ThreatPoints) / (strToAdd - strToSubtract)
                    };


                    //Differential Calculations for Def
                    calcAtAdd = calcBaseValue;
                    float defToAdd = 0f;
                    while (calcBaseValue.OverallPoints == calcAtAdd.OverallPoints && defToAdd < 20)
                    {
                        defToAdd += 0.01f;
                        calcAtAdd = GetCharacterCalculations(character, new Item() { Stats = new Stats() { DefenseRating = defToAdd } }) as CharacterCalculationsProtWarr;
                    }

                    calcAtSubtract = calcBaseValue;
                    float defToSubtract = 0f;
                    while (calcBaseValue.OverallPoints == calcAtSubtract.OverallPoints && defToSubtract > -20)
                    {
                        defToSubtract -= 0.01f;
                        calcAtSubtract = GetCharacterCalculations(character, new Item() { Stats = new Stats() { DefenseRating = defToSubtract } }) as CharacterCalculationsProtWarr;
                    }
                    defToSubtract += 0.01f;

                    ComparisonCalculationProtWarr comparisonDef = new ComparisonCalculationProtWarr()
                    {
                        Name = "10 Defense Rating",
                        OverallPoints = 10 * (calcAtAdd.OverallPoints - calcBaseValue.OverallPoints) / (defToAdd - defToSubtract),
                        MitigationPoints = 10 * (calcAtAdd.MitigationPoints - calcBaseValue.MitigationPoints) / (defToAdd - defToSubtract),
                        SurvivalPoints = 10 * (calcAtAdd.SurvivalPoints - calcBaseValue.SurvivalPoints) / (defToAdd - defToSubtract),
                        ThreatPoints = 10 * (calcAtAdd.ThreatPoints - calcBaseValue.ThreatPoints) / (defToAdd - defToSubtract)
                    };


                    //Differential Calculations for AC
                    calcAtAdd = calcBaseValue;
                    float acToAdd = 0f;
                    while (calcBaseValue.OverallPoints == calcAtAdd.OverallPoints && acToAdd < 2)
                    {
                        acToAdd += 0.01f;
                        calcAtAdd = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Armor = acToAdd } }) as CharacterCalculationsProtWarr;
                    }

                    calcAtSubtract = calcBaseValue;
                    float acToSubtract = 0f;
                    while (calcBaseValue.OverallPoints == calcAtSubtract.OverallPoints && acToSubtract > -2)
                    {
                        acToSubtract -= 0.01f;
                        calcAtSubtract = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Armor = acToSubtract } }) as CharacterCalculationsProtWarr;
                    }
                    acToSubtract += 0.01f;

                    ComparisonCalculationProtWarr comparisonAC = new ComparisonCalculationProtWarr()
                    {
                        Name = "100 Armor",
                        OverallPoints = 100f * (calcAtAdd.OverallPoints - calcBaseValue.OverallPoints) / (acToAdd - acToSubtract),
                        MitigationPoints = 100f * (calcAtAdd.MitigationPoints - calcBaseValue.MitigationPoints) / (acToAdd - acToSubtract),
                        SurvivalPoints = 100f * (calcAtAdd.SurvivalPoints - calcBaseValue.SurvivalPoints) / (acToAdd - acToSubtract),
                        ThreatPoints = 100f * (calcAtAdd.ThreatPoints - calcBaseValue.ThreatPoints) / (acToAdd - acToSubtract),
                    };


                    //Differential Calculations for Sta
                    calcAtAdd = calcBaseValue;
                    float staToAdd = 0f;
                    while (calcBaseValue.OverallPoints == calcAtAdd.OverallPoints && staToAdd < 2)
                    {
                        staToAdd += 0.01f;
                        calcAtAdd = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Stamina = staToAdd } }) as CharacterCalculationsProtWarr;
                    }

                    calcAtSubtract = calcBaseValue;
                    float staToSubtract = 0f;
                    while (calcBaseValue.OverallPoints == calcAtSubtract.OverallPoints && staToSubtract > -2)
                    {
                        staToSubtract -= 0.01f;
                        calcAtSubtract = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Stamina = staToSubtract } }) as CharacterCalculationsProtWarr;
                    }
                    staToSubtract += 0.01f;

                    ComparisonCalculationProtWarr comparisonSta = new ComparisonCalculationProtWarr()
                    {
                        Name = "15 Stamina",
                        OverallPoints = (10f / 0.667f) * (calcAtAdd.OverallPoints - calcBaseValue.OverallPoints) / (staToAdd - staToSubtract),
                        MitigationPoints = (10f / 0.667f) * (calcAtAdd.MitigationPoints - calcBaseValue.MitigationPoints) / (staToAdd - staToSubtract),
                        SurvivalPoints = (10f / 0.667f) * (calcAtAdd.SurvivalPoints - calcBaseValue.SurvivalPoints) / (staToAdd - staToSubtract),
                        ThreatPoints = (10f / 0.667f) * (calcAtAdd.ThreatPoints - calcBaseValue.ThreatPoints) / (staToAdd - staToSubtract)
                    };

                    return new ComparisonCalculationBase[] { 
                        comparisonStr,
						comparisonAgi,
						comparisonAC,
						comparisonSta,
						comparisonDef,
						new ComparisonCalculationProtWarr() { Name = "10 Dodge Rating",
                            OverallPoints = (calcDodgeValue.OverallPoints - calcBaseValue.OverallPoints), 
							MitigationPoints = (calcDodgeValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcDodgeValue.SurvivalPoints - calcBaseValue.SurvivalPoints),
                            ThreatPoints = (calcDodgeValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                        new ComparisonCalculationProtWarr() { Name = "10 Parry Rating",
                            OverallPoints = (calcParryValue.OverallPoints - calcBaseValue.OverallPoints), 
							MitigationPoints = (calcParryValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcParryValue.SurvivalPoints - calcBaseValue.SurvivalPoints),
                            ThreatPoints = (calcParryValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                        new ComparisonCalculationProtWarr() { Name = "10 Block Rating",
                            OverallPoints = (calcBlockValue.OverallPoints - calcBaseValue.OverallPoints), 
							MitigationPoints = (calcBlockValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcBlockValue.SurvivalPoints - calcBaseValue.SurvivalPoints),
                            ThreatPoints = (calcBlockValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                        new ComparisonCalculationProtWarr() { Name = "10 Haste Rating",
                            OverallPoints = (calcHasteValue.OverallPoints - calcBaseValue.OverallPoints), 
							MitigationPoints = (calcHasteValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcHasteValue.SurvivalPoints - calcBaseValue.SurvivalPoints),
                            ThreatPoints = (calcHasteValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                        new ComparisonCalculationProtWarr() { Name = "10 Expertise Rating",
                            OverallPoints = (calcExpertiseValue.OverallPoints - calcBaseValue.OverallPoints), 
							MitigationPoints = (calcExpertiseValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcExpertiseValue.SurvivalPoints - calcBaseValue.SurvivalPoints),
                            ThreatPoints = (calcExpertiseValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                        new ComparisonCalculationProtWarr() { Name = "10 Hit Rating",
                            OverallPoints = (calcHitValue.OverallPoints - calcBaseValue.OverallPoints), 
							MitigationPoints = (calcHitValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcHitValue.SurvivalPoints - calcBaseValue.SurvivalPoints),
                            ThreatPoints = (calcHitValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                        new ComparisonCalculationProtWarr() { Name = "30.76 Block Value",
                            OverallPoints = (calcBlockValueValue.OverallPoints - calcBaseValue.OverallPoints), 
							MitigationPoints = (calcBlockValueValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcBlockValueValue.SurvivalPoints - calcBaseValue.SurvivalPoints),
                            ThreatPoints = (calcBlockValueValue.ThreatPoints - calcBaseValue.ThreatPoints)},
						new ComparisonCalculationProtWarr() { Name = "150 Health",
                            OverallPoints = (calcHealthValue.OverallPoints - calcBaseValue.OverallPoints), 
							MitigationPoints = (calcHealthValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcHealthValue.SurvivalPoints - calcBaseValue.SurvivalPoints),
                            ThreatPoints = (calcHealthValue.ThreatPoints - calcBaseValue.ThreatPoints)},
						new ComparisonCalculationProtWarr() { Name = "10 Resilience",
                            OverallPoints = (calcResilValue.OverallPoints - calcBaseValue.OverallPoints), 
							MitigationPoints = (calcResilValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcResilValue.SurvivalPoints - calcBaseValue.SurvivalPoints),
                            ThreatPoints = (calcResilValue.ThreatPoints - calcBaseValue.ThreatPoints)},
					};
                #endregion
                default:
                    return new ComparisonCalculationBase[0];
            }
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
            // Only Displays Shields in Off-Hand Slot as Devastate is Shield-Only Post-3.2
            if (slot == CharacterSlot.OffHand && item.Type != ItemType.Shield)
                return false;

            return base.ItemFitsInSlot(item, character, slot, ignoreUnique);
        }

        public override bool IsItemRelevant(Item item)
        {
            // Fishing Poles, they love to muck up the list!
            if (item.Id == 45991 || item.Id == 25978)
                return false;

            return base.IsItemRelevant(item);
        }

        public override bool IsBuffRelevant(Buff buff, Character character)
        {
            bool NotClassSetBonus = 
                ((buff.Group == "Set Bonuses") && !(
                    buff.Name.StartsWith("Dreadnaught") || 
                    buff.Name.StartsWith("Siegebreaker") || 
                    buff.Name.StartsWith("Wrynn") ||
                    buff.Name.StartsWith("Ymirjar") ||
                    buff.Name.Contains("Gladiator")
                ));

            return base.IsBuffRelevant(buff, character) && !NotClassSetBonus;
        }

        public override Stats GetRelevantStats(Stats stats)
        {
            Stats relevantStats = new Stats()
            {
                Armor = stats.Armor,
                BonusArmor = stats.BonusArmor,
                Stamina = stats.Stamina,
                Agility = stats.Agility,
                Dodge = stats.Dodge,
                DodgeRating = stats.DodgeRating,
                ParryRating = stats.ParryRating,
                BlockRating = stats.BlockRating,
                BlockValue = stats.BlockValue,
                DefenseRating = stats.DefenseRating,
                Resilience = stats.Resilience,
                BonusAgilityMultiplier = stats.BonusAgilityMultiplier,
                BonusStrengthMultiplier = stats.BonusStrengthMultiplier,
                BonusAttackPowerMultiplier = stats.BonusAttackPowerMultiplier,
                BaseArmorMultiplier = stats.BaseArmorMultiplier,
                BonusArmorMultiplier = stats.BonusArmorMultiplier,
                BonusStaminaMultiplier = stats.BonusStaminaMultiplier,
                Health = stats.Health,
                BattlemasterHealth = stats.BattlemasterHealth,
                BonusHealthMultiplier = stats.BonusHealthMultiplier,
                DamageTakenMultiplier = stats.DamageTakenMultiplier,
                PhysicalDamageTakenMultiplier = stats.PhysicalDamageTakenMultiplier,
                Miss = stats.Miss,
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

                Strength = stats.Strength,
                AttackPower = stats.AttackPower,
                CritRating = stats.CritRating,
                PhysicalCrit = stats.PhysicalCrit,
                HitRating = stats.HitRating,
                PhysicalHit = stats.PhysicalHit,
                HasteRating = stats.HasteRating,
                PhysicalHaste = stats.PhysicalHaste,
                ExpertiseRating = stats.ExpertiseRating,
                ArmorPenetration = stats.ArmorPenetration,
                ArmorPenetrationRating = stats.ArmorPenetrationRating,
                WeaponDamage = stats.WeaponDamage,
                BonusCritMultiplier = stats.BonusCritMultiplier,
                ThreatIncreaseMultiplier = stats.ThreatIncreaseMultiplier,
                BonusDamageMultiplier = stats.BonusDamageMultiplier,
                BonusBlockValueMultiplier = stats.BonusBlockValueMultiplier,
                BonusBleedDamageMultiplier = stats.BonusBleedDamageMultiplier,
                BossAttackSpeedMultiplier = stats.BossAttackSpeedMultiplier,

                HighestStat = stats.HighestStat,
                Paragon = stats.Paragon,
                DeathbringerProc = stats.DeathbringerProc,
                BonusShieldSlamDamage = stats.BonusShieldSlamDamage,
                BonusShockwaveDamage = stats.BonusShockwaveDamage,
                DevastateCritIncrease = stats.DevastateCritIncrease,
                BonusDevastateDamage = stats.BonusDevastateDamage
            };

            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if ((effect.Trigger == Trigger.Use ||
                    effect.Trigger == Trigger.MeleeCrit ||
                    effect.Trigger == Trigger.MeleeHit ||
                    effect.Trigger == Trigger.PhysicalCrit ||
                    effect.Trigger == Trigger.PhysicalHit ||
                    effect.Trigger == Trigger.DoTTick ||
                    effect.Trigger == Trigger.DamageDone ||
                    effect.Trigger == Trigger.DamageOrHealingDone ||
                    effect.Trigger == Trigger.DamageTaken) && HasRelevantStats(effect.Stats))
                {
                    relevantStats.AddSpecialEffect(effect);
                }
            }

            return relevantStats;
        }

        public override bool HasRelevantStats(Stats stats)
        {
            // Stats that will automatically mark the item as relevant
            bool superRelevant =
                (
                    stats.BonusArmor + stats.BonusArmorMultiplier + 
                    stats.BonusStaminaMultiplier + stats.DefenseRating + stats.Dodge + stats.DodgeRating + stats.ParryRating +
                    stats.BlockRating + stats.BonusHealthMultiplier +
                    stats.DamageTakenMultiplier + stats.PhysicalDamageTakenMultiplier + stats.Miss +
                    stats.ArcaneResistance + stats.NatureResistance + stats.FireResistance +
                    stats.FrostResistance + stats.ShadowResistance + stats.ArcaneResistanceBuff +
                    stats.NatureResistanceBuff + stats.FireResistanceBuff + stats.FrostResistanceBuff + stats.ShadowResistanceBuff +
                    stats.ThreatIncreaseMultiplier + stats.BonusBlockValueMultiplier +
                    stats.BonusShieldSlamDamage + stats.BonusShockwaveDamage + stats.DevastateCritIncrease + stats.BonusDevastateDamage
                ) != 0;

            // Stats which are potentially relevant
            bool relevant =
                (stats.Agility + stats.Armor +
                    stats.BonusAgilityMultiplier + stats.BonusStrengthMultiplier + stats.BonusAttackPowerMultiplier +
                    stats.Health + stats.BattlemasterHealth + stats.Stamina + stats.Resilience + stats.BlockValue +
                    stats.Strength + stats.AttackPower + stats.CritRating + stats.HitRating + stats.HasteRating +
                    stats.PhysicalHit + stats.PhysicalHaste + stats.PhysicalCrit +
                    stats.ExpertiseRating + stats.ArmorPenetration + stats.ArmorPenetrationRating + stats.WeaponDamage +
                    stats.BonusCritMultiplier + 
                    stats.BonusDamageMultiplier +
                    stats.BonusBleedDamageMultiplier + stats.BossAttackSpeedMultiplier + 
                    stats.HighestStat + stats.Paragon + stats.DeathbringerProc +
                    stats.MovementSpeed + stats.FearDurReduc + stats.StunDurReduc + stats.SnareRootDurReduc
                ) != 0;

            // Stats which mark the item as irrelevant if not super-relevant
            bool notRelevant =
                (stats.Mp5 + stats.SpellPower + stats.Mana + stats.SpellPenetration +
                    stats.BonusSpiritMultiplier + stats.BonusIntellectMultiplier + stats.BonusManaMultiplier
                ) != 0;

            if (!superRelevant) // No reason to check effects if it's already known as relevant
            {
                foreach (SpecialEffect effect in stats.SpecialEffects())
                {
                    if (effect.Trigger == Trigger.Use ||
                        effect.Trigger == Trigger.MeleeCrit ||
                        effect.Trigger == Trigger.MeleeHit ||
                        effect.Trigger == Trigger.PhysicalCrit ||
                        effect.Trigger == Trigger.PhysicalHit ||
                        effect.Trigger == Trigger.DoTTick ||
                        effect.Trigger == Trigger.DamageDone ||
                        effect.Trigger == Trigger.DamageOrHealingDone ||
                        effect.Trigger == Trigger.DamageTaken)
                    {
                        relevant |= HasRelevantStats(effect.Stats);
                        if (relevant) break;
                    }
                }
            }

            return (superRelevant || (relevant && !notRelevant));
        }

    }
}
