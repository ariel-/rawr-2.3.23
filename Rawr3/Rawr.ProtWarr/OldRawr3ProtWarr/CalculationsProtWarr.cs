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
                    { subtle,   subtle,     subtle,     subtle,     austere },  // Max Avoidance
                    { subtle,   stalwart,   regal,      subtle,     austere },  // Avoidance Heavy
                    { regal,    enduring,   solid,      solid,      austere },  // Balanced Avoidance
                    { guardian, enduring,   solid,      solid,      austere },  // Balanced TPS
                    { solid,    solid,      solid,      solid,      austere },  // Max Health
                };

                // Generate List of Gemming Templates
                List<GemmingTemplate> gemmingTemplate = new List<GemmingTemplate>();
                for (int j = 0; j <= groupNames.GetUpperBound(0); j++)
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
                            Enabled = j == 1
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
                    "Offensive Stats:Limited Threat/sec",
                    "Offensive Stats:Unlimited Threat/sec*All white damage converted to Heroic Strikes.",

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
                    "Unlimited TPS",
                    "Limited TPS",
                    "% Total Mitigation",
					"% Guaranteed Reduction",
					"% Chance to Avoid Attacks",
					"% Chance to be Crit",
                    "% Chance to be Avoided", 
                    "% Chance to be Dodged",
                    "% Chance to Miss",
                    "Hit %",
                    "Expertise %",
                    "Burst Time", 
                    "TankPoints", 
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
                    _subPointNameColors.Add("Survival", System.Drawing.Color.Blue);
                    _subPointNameColors.Add("Mitigation", System.Drawing.Color.Red);
                    _subPointNameColors.Add("Threat", System.Drawing.Color.Green);
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
            character.ActiveBuffs.Add(Buff.GetBuffByName("Strength of Earth Totem"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Enhancing Totems (Agility/Strength)"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Devotion Aura"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Improved Devotion Aura (Armor)"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Inspiration"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Blessing of Might"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Improved Blessing of Might"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Unleashed Rage"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Sanctified Retribution"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Grace"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Swift Retribution"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Commanding Shout"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Commanding Presence (Health)"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Leader of the Pack"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Windfury Totem"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Improved Windfury Totem"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Power Word: Fortitude"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Improved Power Word: Fortitude"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Mark of the Wild"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Improved Mark of the Wild"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Blessing of Kings"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Sunder Armor"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Faerie Fire"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Trauma"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Heart of the Crusader"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Insect Swarm"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Flask of Stoneblood"));
            character.ActiveBuffs.Add(Buff.GetBuffByName("Fish Feast"));
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
            CalculationOptionsProtWarr calcOpts = character.CalculationOptions as CalculationOptionsProtWarr;
            Stats stats = GetCharacterStats(character, additionalItem);

            AttackModelMode amm = AttackModelMode.Basic;
            if (character.WarriorTalents.UnrelentingAssault > 0)
                amm = AttackModelMode.UnrelentingAssault;
            else if (character.WarriorTalents.Shockwave > 0)
                amm = AttackModelMode.FullProtection;
            else if (character.WarriorTalents.Devastate > 0)
                amm = AttackModelMode.Devastate;

            DefendModel dm = new DefendModel(character, stats);
            AttackModel am = new AttackModel(character, stats, amm);

            calculatedStats.BasicStats = stats;
            calculatedStats.TargetLevel = calcOpts.TargetLevel;
            calculatedStats.ActiveBuffs = new List<Buff>(character.ActiveBuffs);
            calculatedStats.Abilities = am.Abilities;

            calculatedStats.Miss = dm.DefendTable.Miss;
            calculatedStats.Dodge = dm.DefendTable.Dodge;
            calculatedStats.Parry = dm.DefendTable.Parry;
            calculatedStats.Block = dm.DefendTable.Block;

            calculatedStats.Defense = (float)Math.Floor(stats.Defense + StatConversion.GetDefenseFromRating(stats.DefenseRating, CharacterClass.Warrior));
            calculatedStats.BlockValue = stats.BlockValue;

            calculatedStats.DodgePlusMissPlusParry = calculatedStats.Dodge + calculatedStats.Miss + calculatedStats.Parry;
            calculatedStats.DodgePlusMissPlusParryPlusBlock = calculatedStats.Dodge + calculatedStats.Miss + calculatedStats.Parry + calculatedStats.Block;
            calculatedStats.CritReduction = Lookup.AvoidanceChance(character, stats, HitResult.Crit);
            calculatedStats.CritVulnerability = dm.DefendTable.Critical;
            calculatedStats.DefenseRatingNeeded = StatConversion.GetDefenseRatingNeeded(character, stats, calcOpts.TargetLevel);

            calculatedStats.ArmorReduction = Lookup.ArmorReduction(character, stats);
            calculatedStats.GuaranteedReduction = dm.GuaranteedReduction;
            calculatedStats.TotalMitigation = dm.Mitigation;
            calculatedStats.BaseAttackerSpeed = calcOpts.BossAttackSpeed;
            calculatedStats.AttackerSpeed = dm.ParryModel.BossAttackSpeed;
            calculatedStats.DamageTaken = dm.DamagePerSecond;
            calculatedStats.DamageTakenPerHit = dm.DamagePerHit;
            calculatedStats.DamageTakenPerBlock = dm.DamagePerBlock;
            calculatedStats.DamageTakenPerCrit = dm.DamagePerCrit;

            calculatedStats.ArcaneReduction = (1.0f - Lookup.MagicReduction(character, stats, DamageType.Arcane));
            calculatedStats.FireReduction = (1.0f - Lookup.MagicReduction(character, stats, DamageType.Fire));
            calculatedStats.FrostReduction = (1.0f - Lookup.MagicReduction(character, stats, DamageType.Frost));
            calculatedStats.NatureReduction = (1.0f - Lookup.MagicReduction(character, stats, DamageType.Nature));
            calculatedStats.ShadowReduction = (1.0f - Lookup.MagicReduction(character, stats, DamageType.Shadow));
            calculatedStats.ArcaneSurvivalPoints = stats.Health / Lookup.MagicReduction(character, stats, DamageType.Arcane);
            calculatedStats.FireSurvivalPoints = stats.Health / Lookup.MagicReduction(character, stats, DamageType.Fire);
            calculatedStats.FrostSurvivalPoints = stats.Health / Lookup.MagicReduction(character, stats, DamageType.Frost);
            calculatedStats.NatureSurvivalPoints = stats.Health / Lookup.MagicReduction(character, stats, DamageType.Nature);
            calculatedStats.ShadowSurvivalPoints = stats.Health / Lookup.MagicReduction(character, stats, DamageType.Shadow);

            calculatedStats.Hit = Lookup.BonusHitPercentage(character, stats);
            calculatedStats.Crit = Lookup.BonusCritPercentage(character, stats);
            calculatedStats.Expertise = Lookup.BonusExpertisePercentage(character, stats);
            calculatedStats.Haste = Lookup.BonusHastePercentage(character, stats);
            calculatedStats.ArmorPenetration = Lookup.BonusArmorPenetrationPercentage(character, stats);
            calculatedStats.AvoidedAttacks = am.Abilities[Ability.None].AttackTable.AnyMiss;
            calculatedStats.DodgedAttacks = am.Abilities[Ability.None].AttackTable.Dodge;
            calculatedStats.ParriedAttacks = am.Abilities[Ability.None].AttackTable.Parry;
            calculatedStats.MissedAttacks = am.Abilities[Ability.None].AttackTable.Miss;
            calculatedStats.WeaponSpeed = Lookup.WeaponSpeed(character, stats);
            calculatedStats.TotalDamagePerSecond = am.DamagePerSecond;

            calculatedStats.UnlimitedThreat = am.ThreatPerSecond;
            am.RageModelMode = RageModelMode.Limited;
            calculatedStats.LimitedThreat = am.ThreatPerSecond;
            calculatedStats.ThreatModel = am.Name + "\n" + am.Description;

            calculatedStats.TankPoints = dm.TankPoints;
            calculatedStats.BurstTime = dm.BurstTime;
            calculatedStats.RankingMode = calcOpts.RankingMode;
            calculatedStats.ThreatPoints = (calcOpts.ThreatScale * ((calculatedStats.LimitedThreat + calculatedStats.UnlimitedThreat) / 2.0f));
            switch (calcOpts.RankingMode)
            {
                case 2:
                    // Tank Points Mode
                    calculatedStats.SurvivalPoints = (dm.EffectiveHealth);
                    calculatedStats.MitigationPoints = (dm.TankPoints - dm.EffectiveHealth);
                    calculatedStats.ThreatPoints *= 3.0f;
                    break;
                case 3:
                    // Burst Time Mode
                    float threatScale = Convert.ToSingle(Math.Pow(Convert.ToDouble(calcOpts.BossAttackValue) / 25000.0d, 4));
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
                    calculatedStats.SurvivalPoints = (dm.EffectiveHealth);
                    calculatedStats.MitigationPoints = dm.Mitigation * calcOpts.BossAttackValue * calcOpts.MitigationScale * 100.0f;
                    break;
            }
            calculatedStats.OverallPoints = calculatedStats.MitigationPoints + calculatedStats.SurvivalPoints + calculatedStats.ThreatPoints;

            return calculatedStats;
        }

        public override Stats GetCharacterStats(Character character, Item additionalItem)
        {
            CalculationOptionsProtWarr calcOpts = character.CalculationOptions as CalculationOptionsProtWarr;
            WarriorTalents talents = character.WarriorTalents;

            Stats statsRace = BaseStats.GetBaseStats(character.Level, CharacterClass.Warrior, character.Race);
            Stats statsBuffs = GetBuffsStats(character.ActiveBuffs);
            Stats statsItems = GetItemStats(character, additionalItem);
            Stats statsTalents = new Stats()
            {
                Parry = talents.Deflection * 0.01f,
                Dodge = talents.Anticipation * 0.01f,
                Block = talents.ShieldSpecialization * 0.01f,
                BonusBlockValueMultiplier = talents.ShieldMastery * 0.15f,
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
                BonusStaminaMultiplier = talents.Vitality * 0.02f + talents.StrengthOfArms * 0.02f,
                BonusStrengthMultiplier = talents.Vitality * 0.02f + talents.StrengthOfArms * 0.02f,
                Expertise = talents.Vitality * 2.0f + talents.StrengthOfArms * 2.0f,
                BonusShieldSlamDamage = talents.GagOrder * 0.05f,
                DevastateCritIncrease = talents.SwordAndBoard * 0.05f,
                BaseArmorMultiplier = talents.Toughness * 0.02f,
                PhysicalHaste = talents.BloodFrenzy * 0.03f,
                PhysicalHit = talents.Precision * 0.01f,
            };
            Stats statsGlyphs = new Stats()
            {
                BonusBlockValueMultiplier = (talents.GlyphOfBlocking ? 0.1f : 0.0f),
            };
            Stats statsGearEnchantsBuffs = statsItems + statsBuffs;
            Stats statsTotal = statsRace + statsItems + statsBuffs + statsTalents + statsGlyphs;
            Stats statsProcs = new Stats();

            // Base Stats
            statsTotal.BaseAgility = statsRace.Agility + statsTalents.Agility;
            statsTotal.Stamina = (float)Math.Floor((statsRace.Stamina + statsTalents.Stamina) * (1.0f + statsTotal.BonusStaminaMultiplier));
            statsTotal.Stamina += (float)Math.Floor((statsItems.Stamina + statsBuffs.Stamina) * (1.0f + statsTotal.BonusStaminaMultiplier));
            statsTotal.Strength = (float)Math.Floor((statsRace.Strength + statsTalents.Strength) * (1.0f + statsTotal.BonusStrengthMultiplier));
            statsTotal.Strength += (float)Math.Floor((statsItems.Strength + statsBuffs.Strength) * (1.0f + statsTotal.BonusStrengthMultiplier));
            statsTotal.Agility = (float)Math.Floor((statsRace.Agility + statsTalents.Agility) * (1.0f + statsTotal.BonusAgilityMultiplier));
            statsTotal.Agility += (float)Math.Floor((statsItems.Agility + statsBuffs.Agility) * (1.0f + statsTotal.BonusAgilityMultiplier));
            statsTotal.Health += StatConversion.GetHealthFromStamina(statsTotal.Stamina, CharacterClass.Warrior);
            if (character.ActiveBuffsContains("Commanding Shout"))
                statsBuffs.Health += statsBuffs.BonusCommandingShoutHP;
            statsTotal.Health *= (float)Math.Floor(1.0f + statsTotal.BonusHealthMultiplier);

            // Defensive Stats
            statsTotal.Armor = (float)Math.Ceiling(statsTotal.Armor * (1.0f + statsTotal.BaseArmorMultiplier));
            statsTotal.BonusArmor += statsTotal.Agility * 2.0f;
            statsTotal.Armor += (float)Math.Ceiling(statsTotal.BonusArmor * (1.0f + statsTotal.BonusArmorMultiplier));

            statsTotal.BlockValue += (float)Math.Floor(StatConversion.GetBlockValueFromStrength(statsTotal.Strength, CharacterClass.Warrior) - 10.0f);
            statsTotal.BlockValue = (float)Math.Floor(statsTotal.BlockValue * (1 + statsTotal.BonusBlockValueMultiplier));

            statsTotal.NatureResistance += statsTotal.NatureResistanceBuff + statsTotal.AllResist;
            statsTotal.FireResistance += statsTotal.FireResistanceBuff + statsTotal.AllResist;
            statsTotal.FrostResistance += statsTotal.FrostResistanceBuff + statsTotal.AllResist;
            statsTotal.ShadowResistance += statsTotal.ShadowResistanceBuff + statsTotal.AllResist;
            statsTotal.ArcaneResistance += statsTotal.ArcaneResistanceBuff + statsTotal.AllResist;

            // Offensive Stats
            statsTotal.AttackPower += statsTotal.Strength * 2.0f + (float)Math.Floor(talents.ArmoredToTheTeeth * statsTotal.Armor / 108.0f);
            statsTotal.AttackPower *= (1 + statsTotal.BonusAttackPowerMultiplier);

            // Calculate Procs and Special Effects
            statsTotal += GetSpecialEffectStats(character, statsTotal);

            // Greatness/Highest Stat Effect
            statsTotal.Strength += (float)Math.Floor(statsTotal.HighestStat * (1 + statsTotal.BonusStrengthMultiplier));

            return statsTotal;
        }

        private Stats GetSpecialEffectStats(Character character, Stats stats)
        {
            Stats statsSpecialEffects = new Stats();

            float weaponSpeed = 1.0f;
            if (character.MainHand != null)
                weaponSpeed = character.MainHand.Speed;

            AttackModelMode amm = AttackModelMode.Basic;
            if (character.WarriorTalents.UnrelentingAssault > 0)
                amm = AttackModelMode.UnrelentingAssault;
            else if (character.WarriorTalents.Shockwave > 0)
                amm = AttackModelMode.FullProtection;
            else if (character.WarriorTalents.Devastate > 0)
                amm = AttackModelMode.Devastate;

            AttackModel am = new AttackModel(character, stats, amm);

            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                switch (effect.Trigger)
                {
                    case Trigger.Use:
                        statsSpecialEffects += effect.GetAverageStats(0.0f, 1.0f, weaponSpeed);
                        break;
                    case Trigger.MeleeHit:
                    case Trigger.PhysicalHit:
                        statsSpecialEffects += effect.GetAverageStats((1.0f / am.AttacksPerSecond), 1.0f, weaponSpeed);
                        break;
                    case Trigger.MeleeCrit:
                    case Trigger.PhysicalCrit:
                        statsSpecialEffects += effect.GetAverageStats((1.0f / am.CritsPerSecond), 1.0f, weaponSpeed);
                        break;
                    case Trigger.DoTTick:
                        if (character.WarriorTalents.DeepWounds > 0)
                            statsSpecialEffects += effect.GetAverageStats(2.0f, 1.0f, weaponSpeed);
                        break;
                    case Trigger.DamageDone:
                        statsSpecialEffects += effect.GetAverageStats((1.0f / am.AttacksPerSecond), 1.0f, weaponSpeed);
                        break;
                    case Trigger.DamageTaken:
                        statsSpecialEffects += effect.GetAverageStats((1.0f / am.AttackerHitsPerSecond), 1.0f, weaponSpeed);
                        break;
                }
            }

            // Base Stats
            statsSpecialEffects.Stamina = (float)Math.Floor(statsSpecialEffects.Stamina * (1.0f + stats.BonusStaminaMultiplier));
            statsSpecialEffects.Strength = (float)Math.Floor(statsSpecialEffects.Strength * (1.0f + stats.BonusStrengthMultiplier));
            statsSpecialEffects.Agility = (float)Math.Floor(statsSpecialEffects.Agility * (1.0f + stats.BonusAgilityMultiplier));
            statsSpecialEffects.Health += (float)Math.Floor(statsSpecialEffects.Stamina * 10.0f);

            // Defensive Stats
            statsSpecialEffects.Armor = (float)Math.Floor(statsSpecialEffects.Armor * (1f + stats.BaseArmorMultiplier + statsSpecialEffects.BaseArmorMultiplier));
            statsSpecialEffects.BonusArmor += statsSpecialEffects.Agility * 2.0f;
            statsSpecialEffects.BonusArmor = (float)Math.Floor(statsSpecialEffects.BonusArmor * (1.0f + stats.BonusArmorMultiplier + statsSpecialEffects.BonusArmorMultiplier));
            statsSpecialEffects.Armor += statsSpecialEffects.BonusArmor;

            statsSpecialEffects.BlockValue += (float)Math.Floor(StatConversion.GetBlockValueFromStrength(statsSpecialEffects.Strength, CharacterClass.Warrior));
            statsSpecialEffects.BlockValue = (float)Math.Floor(statsSpecialEffects.BlockValue * (1.0f + stats.BonusBlockValueMultiplier + statsSpecialEffects.BonusBlockValueMultiplier));

            // Offensive Stats
            statsSpecialEffects.AttackPower += statsSpecialEffects.Strength * 2.0f;
            statsSpecialEffects.AttackPower += (float)Math.Floor(character.WarriorTalents.ArmoredToTheTeeth * statsSpecialEffects.Armor / 108.0f);
            statsSpecialEffects.AttackPower = (float)Math.Floor(statsSpecialEffects.AttackPower * (1.0f + stats.BonusAttackPowerMultiplier + statsSpecialEffects.BonusAttackPowerMultiplier));

            return statsSpecialEffects;
        }

        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
        {
            CharacterCalculationsProtWarr calculations = GetCharacterCalculations(character) as CharacterCalculationsProtWarr;

            switch (chartName)
            {
                case "Ability Damage":
                case "Ability Threat":
                    {
                        ComparisonCalculationBase[] comparisons = new ComparisonCalculationBase[calculations.Abilities.Count];
                        int j = 0;
                        foreach (AbilityModel ability in calculations.Abilities)
                        {
                            if (ability.Ability != Ability.Vigilance)
                            {
                                ComparisonCalculationProtWarr comparison = new ComparisonCalculationProtWarr();

                                comparison.Name = ability.Name;
                                if (chartName == "Ability Damage")
                                    comparison.MitigationPoints = ability.Damage;
                                if (chartName == "Ability Threat")
                                    comparison.ThreatPoints = ability.Threat;

                                comparison.OverallPoints = comparison.SurvivalPoints + comparison.ThreatPoints + comparison.MitigationPoints;
                                comparisons[j] = comparison;
                                j++;
                            }
                        }
                        return comparisons;
                    }
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
                            calcHit.OverallPoints = calcHit.SurvivalPoints = (1.0f - (calculations.DodgePlusMissPlusParryPlusBlock + calculations.CritVulnerability)) * 100.0f;
                        }
                        return new ComparisonCalculationBase[] { calcMiss, calcDodge, calcParry, calcBlock, calcCrit, calcCrush, calcHit };
                    }
                case "Item Budget":
                    CharacterCalculationsProtWarr calcBaseValue = GetCharacterCalculations(character) as CharacterCalculationsProtWarr;
                    CharacterCalculationsProtWarr calcDodgeValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { DodgeRating = 10f } }) as CharacterCalculationsProtWarr;
                    CharacterCalculationsProtWarr calcParryValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { ParryRating = 10f } }) as CharacterCalculationsProtWarr;
                    CharacterCalculationsProtWarr calcBlockValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { BlockRating = 10f } }) as CharacterCalculationsProtWarr;
                    CharacterCalculationsProtWarr calcHasteValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { HasteRating = 10f } }) as CharacterCalculationsProtWarr;
                    CharacterCalculationsProtWarr calcExpertiseValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { ExpertiseRating = 10f } }) as CharacterCalculationsProtWarr;
                    CharacterCalculationsProtWarr calcHitValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { HitRating = 10f } }) as CharacterCalculationsProtWarr;
                    CharacterCalculationsProtWarr calcBlockValueValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { BlockValue = 10f / 0.65f } }) as CharacterCalculationsProtWarr;
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
                        new ComparisonCalculationProtWarr() { Name = "15.38 Block Value",
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
                default:
                    return new ComparisonCalculationBase[0];
            }
        }
        //Hide the ranged weapon enchants. None of them apply to melee damage at all.
        public override bool EnchantFitsInSlot(Enchant enchant, Character character, ItemSlot slot)
        {
            return enchant.Slot == ItemSlot.Ranged ? false : base.EnchantFitsInSlot(enchant, character, slot);
        }

        public override bool IsItemRelevant(Item item)
        {
            // Bone Fishing Pole
            if (item.Id == 45991)
                return false;

            return base.IsItemRelevant(item);
        }

        public override Stats GetRelevantStats(Stats stats)
        {
            Stats relevantStats = new Stats()
            {
                Armor = stats.Armor,
                BonusArmor = stats.BonusArmor,
                AverageArmor = stats.AverageArmor,
                Stamina = stats.Stamina,
                Agility = stats.Agility,
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
                BonusHealthMultiplier = stats.BonusHealthMultiplier,
                DamageTakenMultiplier = stats.DamageTakenMultiplier,
                Miss = stats.Miss,
                CritChanceReduction = stats.CritChanceReduction,
                AllResist = stats.AllResist,
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

                HighestStat = stats.HighestStat,
                BonusCommandingShoutHP = stats.BonusCommandingShoutHP,
                BonusShieldSlamDamage = stats.BonusShieldSlamDamage,
                DevastateCritIncrease = stats.DevastateCritIncrease
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
                    effect.Trigger == Trigger.DamageTaken) && HasRelevantStats(effect.Stats))
                {
                    relevantStats.AddSpecialEffect(effect);
                }
            }

            return relevantStats;
        }

        public override bool HasRelevantStats(Stats stats)
        {
            bool relevant =
                (stats.Agility + stats.Armor + stats.AverageArmor + stats.BonusArmor +
                    stats.BonusAgilityMultiplier + stats.BonusStrengthMultiplier +
                    stats.BonusAttackPowerMultiplier + stats.BonusArmorMultiplier +
                    stats.BonusStaminaMultiplier + stats.DefenseRating + stats.DodgeRating + stats.ParryRating +
                    stats.BlockRating + stats.BlockValue + stats.Health + stats.BonusHealthMultiplier +
                    stats.DamageTakenMultiplier + stats.Miss + stats.Resilience + stats.Stamina + stats.AllResist +
                    stats.ArcaneResistance + stats.NatureResistance + stats.FireResistance +
                    stats.FrostResistance + stats.ShadowResistance + stats.ArcaneResistanceBuff +
                    stats.NatureResistanceBuff + stats.FireResistanceBuff +
                    stats.FrostResistanceBuff + stats.ShadowResistanceBuff +
                    stats.Strength + stats.AttackPower + stats.CritRating + stats.HitRating + stats.HasteRating +
                    stats.PhysicalHit + stats.PhysicalHaste + stats.PhysicalCrit +
                    stats.ExpertiseRating + stats.ArmorPenetration + stats.ArmorPenetrationRating + stats.WeaponDamage +
                    stats.BonusCritMultiplier + stats.CritChanceReduction +
                    stats.ThreatIncreaseMultiplier + stats.BonusDamageMultiplier + stats.BonusBlockValueMultiplier +
                    stats.BonusBleedDamageMultiplier +
                    stats.HighestStat + stats.BonusCommandingShoutHP + stats.BonusShieldSlamDamage + stats.DevastateCritIncrease
                ) != 0;

            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if (effect.Trigger == Trigger.Use ||
                    effect.Trigger == Trigger.MeleeCrit ||
                    effect.Trigger == Trigger.MeleeHit ||
                    effect.Trigger == Trigger.PhysicalCrit ||
                    effect.Trigger == Trigger.PhysicalHit ||
                    effect.Trigger == Trigger.DoTTick ||
                    effect.Trigger == Trigger.DamageDone ||
                    effect.Trigger == Trigger.DamageTaken)
                {
                    relevant |= HasRelevantStats(effect.Stats);
                    if (relevant) break;
                }
            }

            return relevant;
        }

    }
}
