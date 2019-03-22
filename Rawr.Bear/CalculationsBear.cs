/////////////////////////////////
//
// Written by Chadd Nervig (aka Astrylian) for Rawr. E-mail me at cnervig@hotmail.com.
//
// Rawr is a program for comparing and exploring gear for characters in the MMORPG, 
// World of Warcraft. It has been designed from the start to be fun to use, and helpful in 
// finding better combinations of gear, and what gear to obtain. (.NET, WinForm, C# 3.0, XML)
//
// Rawr is designed as a platform for hosting 'models', which implement the features and
// calculations needed by a specific class or specialization of WoW character.
//
// This file contains the majority of the classes and methods for the Rawr.Bear model,
// which models combat for the Druid Bear type of character.
//
// Please visit http://rawr.codeplex.com/ for the full source code of Rawr, or contact me
// for additional details.
//
/////////////////////////////////

using System;
using System.Collections.Generic;
#if RAWR3
using System.Windows.Media;
#endif
using System.Text;

namespace Rawr.Bear
{
    /// <summary>
    /// Core class representing the Rawr.Bear model
    /// </summary>
    [Rawr.Calculations.RawrModelInfo("Bear", "Ability_Racial_BearForm", CharacterClass.Druid)]
    public class CalculationsBear : CalculationsBase
    {
        #region Basic Model Properties and Methods
        /// <summary>
        /// GemmingTemplates to be used by default, when none are defined
        /// </summary>
        public override List<GemmingTemplate> DefaultGemmingTemplates
        {
            get
            {
                ////Relevant Gem IDs for Ferals
                //Red
                int[] bold = { 39900, 39996, 40111, 42142 };
                int[] delicate = { 39905, 39997, 40112, 42143 };
                
                //Purple
                int[] shifting = { 39935, 40023, 40130 };
                int[] sovereign = { 39934, 40022, 40129 };

                //Blue
                int[] solid = { 39919, 40008, 40119, 36767 };

                //Green
                int[] enduring = { 39976, 40089, 40167 };

                //Yellow
                int[] thick = { 39916, 40015, 40126, 42157 };

                //Orange
                int[] etched = { 39948, 40038, 40143 };
                int[] fierce = { 39951, 40041, 40146 };
                int[] glinting = { 39953, 40044, 40148 };
                int[] stalwart = { 39964, 40056, 40160 };

                //Meta
                int austere = 41380;
                // int relentless = 41398;

                return new List<GemmingTemplate>()
                {
                    new GemmingTemplate() { Model = "Bear", Group = "Uncommon", //Max Mitigation
                        RedId = delicate[0], YellowId = delicate[0], BlueId = delicate[0], PrismaticId = delicate[0], MetaId = austere },
                    new GemmingTemplate() { Model = "Bear", Group = "Uncommon", //Mitigation Heavier 
                        RedId = delicate[0], YellowId = stalwart[0], BlueId = shifting[0], PrismaticId = delicate[0], MetaId = austere },
                    new GemmingTemplate() { Model = "Bear", Group = "Uncommon", //Mitigation Heavy
                        RedId = delicate[0], YellowId = glinting[0], BlueId = shifting[0], PrismaticId = delicate[0], MetaId = austere },
                    new GemmingTemplate() { Model = "Bear", Group = "Uncommon", //Survivbility Heavy
                        RedId = shifting[0], YellowId = enduring[0], BlueId = solid[0], PrismaticId = solid[0], MetaId = austere },
                    new GemmingTemplate() { Model = "Bear", Group = "Uncommon", //Max Survivbility
                        RedId = solid[0], YellowId = solid[0], BlueId = solid[0], PrismaticId = solid[0], MetaId = austere },

                    new GemmingTemplate() { Model = "Bear", Group = "Rare", //Max Mitigation
                        RedId = delicate[1], YellowId = delicate[1], BlueId = delicate[1], PrismaticId = delicate[1], MetaId = austere },
                    new GemmingTemplate() { Model = "Bear", Group = "Rare", //Mitigation Heavier 
                        RedId = delicate[1], YellowId = stalwart[1], BlueId = shifting[1], PrismaticId = delicate[1], MetaId = austere },
                    new GemmingTemplate() { Model = "Bear", Group = "Rare", //Mitigation Heavy
                        RedId = delicate[1], YellowId = glinting[1], BlueId = shifting[1], PrismaticId = delicate[1], MetaId = austere },
                    new GemmingTemplate() { Model = "Bear", Group = "Rare", //Survivbility Heavy
                        RedId = shifting[1], YellowId = enduring[1], BlueId = solid[1], PrismaticId = solid[1], MetaId = austere },
                    new GemmingTemplate() { Model = "Bear", Group = "Rare", //Max Survivbility
                        RedId = solid[1], YellowId = solid[1], BlueId = solid[1], PrismaticId = solid[1], MetaId = austere },
                        
                    new GemmingTemplate() { Model = "Bear", Group = "Epic", Enabled = true, //Max Mitigation
                        RedId = delicate[2], YellowId = delicate[2], BlueId = delicate[2], PrismaticId = delicate[2], MetaId = austere },
                    new GemmingTemplate() { Model = "Bear", Group = "Epic", Enabled = true, //Mitigation Heavier 
                        RedId = delicate[2], YellowId = stalwart[2], BlueId = shifting[2], PrismaticId = delicate[2], MetaId = austere },
                    new GemmingTemplate() { Model = "Bear", Group = "Epic", Enabled = true, //Mitigation Heavy
                        RedId = delicate[2], YellowId = glinting[2], BlueId = shifting[2], PrismaticId = delicate[2], MetaId = austere },
                    new GemmingTemplate() { Model = "Bear", Group = "Epic", Enabled = true, //Survivbility Heavy
                        RedId = shifting[2], YellowId = enduring[2], BlueId = solid[2], PrismaticId = solid[2], MetaId = austere },
                    new GemmingTemplate() { Model = "Bear", Group = "Epic", Enabled = true, //Max Survivbility
                        RedId = solid[2], YellowId = solid[2], BlueId = solid[2], PrismaticId = solid[2], MetaId = austere },
                        
                    new GemmingTemplate() { Model = "Bear", Group = "Jeweler", //Max Mitigation
                        RedId = delicate[3], YellowId = delicate[3], BlueId = delicate[3], PrismaticId = delicate[3], MetaId = austere },
                    new GemmingTemplate() { Model = "Bear", Group = "Jeweler", //Mitigation Heavy
                        RedId = delicate[2], YellowId = delicate[3], BlueId = delicate[3], PrismaticId = delicate[2], MetaId = austere },
                    new GemmingTemplate() { Model = "Bear", Group = "Jeweler", //Survivbility Heavy
                        RedId = solid[3], YellowId = solid[3], BlueId = solid[2], PrismaticId = solid[2], MetaId = austere },
                    new GemmingTemplate() { Model = "Bear", Group = "Jeweler", //Max Survivbility
                        RedId = solid[3], YellowId = solid[3], BlueId = solid[3], PrismaticId = solid[3], MetaId = austere },
                };
            }
        }

#if RAWR3
        private ICalculationOptionsPanel _calculationOptionsPanel = null;
        /// <summary>
        /// Panel to be placed on the Options tab of the main form
        /// </summary>
        public override ICalculationOptionsPanel CalculationOptionsPanel
        {
            get
            {
                if (_calculationOptionsPanel == null)
                {
                    _calculationOptionsPanel = new CalculationOptionsPanelBear();
                }
                return _calculationOptionsPanel;
            }
        }
#else
        private CalculationOptionsPanelBase _calculationOptionsPanel = null;
        /// <summary>
        /// Panel to be placed on the Options tab of the main form
        /// </summary>
        public override CalculationOptionsPanelBase CalculationOptionsPanel
        {
            get
            {
                if (_calculationOptionsPanel == null)
                {
                    _calculationOptionsPanel = new CalculationOptionsPanelBear();
                }
                return _calculationOptionsPanel;
            }
        }
#endif

        private string[] _characterDisplayCalculationLabels = null;
        /// <summary>
        /// Labels of the stats to display on the Stats tab of the main form
        /// </summary>
        public override string[] CharacterDisplayCalculationLabels
        {
            get
            {
                if (_characterDisplayCalculationLabels == null)
                    _characterDisplayCalculationLabels = new string[] {
                    @"Summary:Overall Points*Overall Points are a sum of Mitigation and Survival Points. 
Overall is typically, but not always, the best way to rate gear. 
For specific encounters, closer attention to Mitigation and 
Survival Points individually may be important.",
                    @"Summary:Mitigation Points*Mitigation Points represent the amount of damage you mitigate, 
on average, through armor mitigation and avoidance. It is directly 
relational to your Damage Taken. Ideally, you want to maximize 
Mitigation Points, while maintaining 'enough' Survival Points 
(see Survival Points). If you find yourself dying due to healers 
running OOM, or being too busy healing you and letting other 
raid members die, then focus on Mitigation Points.",
                    @"Summary:Survival Points*Survival Points represents the total raw physical damage 
(pre-mitigation) you can take before dying. Unlike 
Mitigation Points, you should not attempt to maximize this, 
but rather get 'enough' of it, and then focus on Mitigation. 
'Enough' can vary greatly by fight and by your healers, but 
keeping it roughly even with Mitigation Points is a good 
way to maintain 'enough' as you progress. If you find that 
you are being killed by burst damage, focus on Survival Points.",
                    @"Summary:Threat Points*The TPS of your Highest TPS Rotation, multiplied by
the Threat Scale defined on the Options tab.",
                    
                    "Basic Stats:Armor",
                    "Basic Stats:Agility",
                    "Basic Stats:Stamina",
                    "Basic Stats:Strength",
                    "Basic Stats:Attack Power",
                    "Basic Stats:Crit Rating",
                    "Basic Stats:Hit Rating",
                    "Basic Stats:Expertise Rating",
                    "Basic Stats:Haste Rating",
                    "Basic Stats:Armor Penetration Rating",
                    "Basic Stats:Dodge Rating",
                    "Basic Stats:Defense Rating",
                    "Basic Stats:Resilience",
                    "Basic Stats:Nature Resist",
                    "Basic Stats:Fire Resist",
                    "Basic Stats:Frost Resist",
                    "Basic Stats:Shadow Resist",
                    "Basic Stats:Arcane Resist",
                    "Mitigation Stats:Dodge",
                    "Mitigation Stats:Miss",
                    "Mitigation Stats:Mitigation",
                    "Mitigation Stats:Avoidance PreDR",
                    "Mitigation Stats:Avoidance PostDR",
                    "Mitigation Stats:Savage Defense",
                    "Mitigation Stats:Total Mitigation",
                    "Mitigation Stats:Damage Taken",
                    "Survival Stats:Health",
                    "Survival Stats:Chance to be Crit",
                    "Survival Stats:Nature Survival",
                    "Survival Stats:Fire Survival",
                    "Survival Stats:Frost Survival",
                    "Survival Stats:Shadow Survival",
                    "Survival Stats:Arcane Survival",
                    "Threat Stats:Highest DPS Rotation",
                    "Threat Stats:Highest TPS Rotation",
                    "Threat Stats:Swipe Rotation",
                    "Threat Stats:Custom Rotation",
                    "Threat Stats:Melee",
                    "Threat Stats:Maul",
                    "Threat Stats:Mangle",
                    "Threat Stats:Swipe",
                    "Threat Stats:Faerie Fire",
                    "Threat Stats:Lacerate",
                    "Threat Stats:Lacerate DoT Tick",
                    "Threat Stats:Avoided Attacks",
                    };
                return _characterDisplayCalculationLabels;
            }
        }

        private string[] _optimizableCalculationLabels = null;
        /// <summary>
        /// Labels of the stats available to the Optimizer 
        /// </summary>
        public override string[]  OptimizableCalculationLabels
        {
            get
            {
                if (_optimizableCalculationLabels == null)
                    _optimizableCalculationLabels = new string[] {
                    "Health",
                    "Hit Rating",
                    "Expertise Rating",
                    "Haste Rating",
                    "Avoided Attacks %",
                    "Mitigation % from Armor",
                    "Avoidance %",
                    "% Chance to be Crit",
                    "Nature Survival",
                    "Fire Survival",
                    "Frost Survival",
                    "Shadow Survival",
                    "Arcane Survival",
                    "Nature Resist",
                    "Fire Resist",
                    "Frost Resist",
                    "Shadow Resist",
                    "Arcane Resist",
                    "Highest DPS",
                    "Highest TPS",
                    "Swipe DPS",
                    "Swipe TPS",
                    "Custom Rotation DPS",
                    "Custom Rotation TPS",
                    };
                return _optimizableCalculationLabels;
            }
        }

        private string[] _customChartNames = null;
        /// <summary>
        /// Names of the custom charts that Rawr.Bear provides
        /// </summary>
        public override string[] CustomChartNames
        {
            get
            {
                if (_customChartNames == null)
                    _customChartNames = new string[] {
                    "Combat Table",
                    //"Relative Stat Values",
                    "Rotation DPS",
                    "Rotation TPS"
                    };
                return _customChartNames;
            }
        }

#if RAWR3
        private Dictionary<string, Color> _subPointNameColors = null;
        /// <summary>
        /// Names and colors for the SubPoints that Rawr.Bear uses
        /// </summary>
        public override Dictionary<string, Color> SubPointNameColors
        {
            get
            {
                if (_subPointNameColors == null)
                {
                    _subPointNameColors = new Dictionary<string, Color>();
                    _subPointNameColors.Add("Mitigation", Colors.Red);
                    _subPointNameColors.Add("Survival", Colors.Blue);
                    _subPointNameColors.Add("Threat", Colors.Green);
                }
                return _subPointNameColors;
            }
        }
#else
        private Dictionary<string, System.Drawing.Color> _subPointNameColors = null;
        /// <summary>
        /// Names and colors for the SubPoints that Rawr.Bear uses
        /// </summary>
        public override Dictionary<string, System.Drawing.Color> SubPointNameColors
        {
            get
            {
                if (_subPointNameColors == null)
                {
                    _subPointNameColors = new Dictionary<string, System.Drawing.Color>();
                    _subPointNameColors.Add("Mitigation", System.Drawing.Color.FromArgb(255, 255, 0, 0));
                    _subPointNameColors.Add("Survival", System.Drawing.Color.FromArgb(255, 0, 0, 255));
                    _subPointNameColors.Add("Threat", System.Drawing.Color.FromArgb(255, 0, 128, 0));
                }
                return _subPointNameColors;
            }
        }
#endif

        private List<ItemType> _relevantItemTypes = null;
        /// <summary>
        /// ItemTypes that are relevant to Rawr.Bear
        /// </summary>
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

        /// <summary>
        /// The class that Rawr.Bear is designed for (Druid)
        /// </summary>
        public override CharacterClass TargetClass { get { return CharacterClass.Druid; } }
        /// <summary>
        /// Creates a new ComparisonCalculationBear instance
        /// </summary>
        /// <returns>A new ComparisonCalculationBear instance</returns>
        public override ComparisonCalculationBase CreateNewComparisonCalculation() { return new ComparisonCalculationBear(); }
        /// <summary>
        /// Creates a new CharacterCalculationsBear instance
        /// </summary>
        /// <returns>A new CharacterCalculationsBear instance</returns>
        public override CharacterCalculationsBase CreateNewCharacterCalculations() { return new CharacterCalculationsBear(); }

        /// <summary>
        /// Deserializes the CalculationOptionsBear object contained in xml
        /// </summary>
        /// <param name="xml">The CalculationOptionsBear object, serialized as xml</param>
        /// <returns>The deserialized CalculationOptionsBear object</returns>
        public override ICalculationOptionBase DeserializeDataObject(string xml)
        {
            xml = xml.Replace("<CustomUseMaul xsi:nil=\"true\" />", "<CustomUseMaul>false</CustomUseMaul>");
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsBear));
            System.IO.StringReader reader = new System.IO.StringReader(xml);
            CalculationOptionsBear calcOpts = serializer.Deserialize(reader) as CalculationOptionsBear;
            return calcOpts;
        }
#endregion

        #region Primary Calculation Methods
        /// <summary>
        /// Gets the results of the Character provided
        /// </summary>
        /// <param name="character">The Character to calculate resutls for</param>
        /// <param name="additionalItem">An additional item to grant the Character the stats of (as if it were worn)</param>
        /// <returns>The CharacterCalculationsBear containing the results of the calculations</returns>
        public override CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, bool referenceCalculation, bool significantChange, bool needsDisplayCalculations)
        {
            CalculationOptionsBear calcOpts = character.CalculationOptions as CalculationOptionsBear;
            if (calcOpts == null) calcOpts = new CalculationOptionsBear();
            int targetLevel = calcOpts.TargetLevel;
            int characterLevel = character.Level;
            StatsBear stats = GetCharacterStats(character, additionalItem) as StatsBear;
            float levelDifference = (targetLevel - characterLevel) * 0.002f;
            float TargAttackSpeed = calcOpts.TargetAttackSpeed * (1f - stats.BossAttackSpeedMultiplier);
            
            CharacterCalculationsBear calculatedStats = new CharacterCalculationsBear();
            calculatedStats.BasicStats = stats;
            calculatedStats.TargetLevel = targetLevel;

            float hasteBonus = StatConversion.GetHasteFromRating(stats.HasteRating, CharacterClass.Druid);
            float attackSpeed = ((2.5f) / (1f + hasteBonus)) / (1f + stats.PhysicalHaste);

            float hitBonus = StatConversion.GetPhysicalHitFromRating(stats.HitRating, CharacterClass.Druid) + stats.PhysicalHit;
            float expertiseBonus = StatConversion.GetDodgeParryReducFromExpertise(StatConversion.GetExpertiseFromRating(stats.ExpertiseRating, CharacterClass.Druid) + stats.Expertise, CharacterClass.Druid);
            float chanceDodge = Math.Max(0f, StatConversion.WHITE_DODGE_CHANCE_CAP[targetLevel - 80] - expertiseBonus);
            float chanceParry = Math.Max(0f, StatConversion.WHITE_PARRY_CHANCE_CAP[targetLevel - 80] - expertiseBonus);
            float chanceMiss  = Math.Max(0f, StatConversion.WHITE_MISS_CHANCE_CAP[ targetLevel - 80] - hitBonus);
            
            float chanceAvoided = chanceMiss + chanceDodge + chanceParry;

            float rawChanceCrit = StatConversion.GetPhysicalCritFromRating(stats.CritRating, CharacterClass.Druid)
                                + StatConversion.GetPhysicalCritFromAgility(stats.Agility, CharacterClass.Druid)
                                + stats.PhysicalCrit
                                + StatConversion.NPC_LEVEL_CRIT_MOD[targetLevel-80];
            float chanceCrit = rawChanceCrit * (1f - chanceAvoided);
            float chanceCritBleed = (character.DruidTalents.PrimalGore > 0 ? rawChanceCrit : 0f);
            attackSpeed = ((2.5f) / (1f + hasteBonus)) / (1f + stats.PhysicalHaste);
            
            float baseAgi = character.Race == CharacterRace.NightElf ? 87 : 77;
            
            //Calculate avoidance, considering diminishing returns
            float defSkill = (float)Math.Floor(StatConversion.GetDefenseFromRating(stats.DefenseRating, CharacterClass.Druid));
            float dodgeNonDR = stats.Dodge - levelDifference + StatConversion.GetDodgeFromAgility(baseAgi, CharacterClass.Druid);
            float missNonDR = stats.Miss - levelDifference;
            float dodgePreDR = StatConversion.GetDodgeFromAgility(stats.Agility - baseAgi, CharacterClass.Druid)
                               + StatConversion.GetDodgeFromRating(stats.DodgeRating, CharacterClass.Druid)
                               + defSkill * StatConversion.DEFENSE_RATING_AVOIDANCE_MULTIPLIER / 100f;
            float missPreDR   = (defSkill * StatConversion.DEFENSE_RATING_AVOIDANCE_MULTIPLIER / 100f);
            float dodgePostDR = 0.01f / (1f / 116.890707f + 0.00972f / dodgePreDR);
            float missPostDR  = 0.01f / (1f / 16f + 0.00972f / missPreDR);
            float dodgeTotal  = dodgeNonDR + dodgePostDR;
            float missTotal   = missNonDR + missPostDR;

            calculatedStats.Miss = missTotal;
            calculatedStats.Dodge = Math.Min(1f - calculatedStats.Miss, dodgeTotal);
            calculatedStats.ConstantDamageReduction = 1f - ((1f - Math.Min(0.75f, stats.Armor / (stats.Armor - 22167.5f + 467.5f * targetLevel))) * (1f + stats.DamageTakenMultiplier));
            calculatedStats.AvoidancePreDR = dodgeNonDR + dodgePreDR + missNonDR + missPreDR;
            calculatedStats.AvoidancePostDR = dodgeTotal + missTotal;
            calculatedStats.CritReduction = (defSkill * StatConversion.DEFENSE_RATING_AVOIDANCE_MULTIPLIER / 100f)
                                          + StatConversion.GetCritReductionFromResilience(stats.Resilience, CharacterClass.Druid)
                                          + stats.CritChanceReduction;
            calculatedStats.CappedCritReduction = Math.Min(0.05f + levelDifference, calculatedStats.CritReduction);

            float targetHitChance = 1f - calculatedStats.AvoidancePostDR;
            float autoSpecialAttacksPerSecond = 1f / 1.5f + 1f / attackSpeed;
            float lacerateTicksPerSecond = (character.DruidTalents.PrimalGore > 0 ? 1f : 0f) / 3f;
            float totalAttacksPerSecond = autoSpecialAttacksPerSecond + lacerateTicksPerSecond;
            float averageSDAttackCritChance = (chanceCrit * (autoSpecialAttacksPerSecond / totalAttacksPerSecond) + chanceCritBleed * (lacerateTicksPerSecond / totalAttacksPerSecond));
            float playerAttackSpeed = 1f / totalAttacksPerSecond;
            float blockChance = 1f - targetHitChance * ((float)Math.Pow(1f - averageSDAttackCritChance, TargAttackSpeed / playerAttackSpeed)) *
                1f / (1f - (1f - targetHitChance) * (float)Math.Pow(1f - averageSDAttackCritChance, TargAttackSpeed / playerAttackSpeed));
            float blockValue = stats.AttackPower * 0.25f;
            float blockedPercent = Math.Min(1f, (blockValue * blockChance) / ((1f - calculatedStats.ConstantDamageReduction) * calcOpts.TargetDamage));
            calculatedStats.SavageDefenseChance = (float)Math.Round(blockChance, 5);
            calculatedStats.SavageDefenseValue = (float)Math.Floor(blockValue);
            calculatedStats.SavageDefensePercent = (float)Math.Round(blockedPercent, 5);

            float parryHasteDamageMuliplier = 1.0f;
            if (calcOpts.TargetParryHastes)
            {
                float parryableAttacksPerSecond = 7f / 13.5f + 1f / attackSpeed; //In every 13.5sec period (9 GCDs), 2 should be FFF, so only 7 parryable instants per 13.5sec
                //Target parries within the first 40% of their swing timer advances their swing timer by 40%
                //...Within the next 40%, it advances their swing timer to 20% remaining.
                //So, we average that as a 60% window (since the last 40% only provides half as much haste on average), with a 40% swing advance
                float parryWindow = TargAttackSpeed * 0.6f; 
                float parryableAttacksPerParryWindow = parryableAttacksPerSecond * parryWindow;
                float chanceParryPerWindow = 1f - (float)Math.Pow(1f - chanceParry, parryableAttacksPerParryWindow);
                float parryBonusDPSMultiplier = 1f / 0.6f - 1f; //When a parry happens, boss DPS gets muliplied by 1/0.6 for that swing
                parryHasteDamageMuliplier = 1f + (parryBonusDPSMultiplier * chanceParryPerWindow);
            }

            //Out of 100 attacks, you'll take...
            float crits = Math.Min(Math.Max(0f, 1f - calculatedStats.AvoidancePostDR), (0.05f + levelDifference) - calculatedStats.CappedCritReduction);
            //float crushes = targetLevel == 73 ? Math.Max(0f, Math.Min(15f, 100f - (crits + calculatedStats.AvoidancePreDR)) - stats.CritChanceReduction) : 0f;
            float hits = Math.Max(0f, 1f - (crits + calculatedStats.AvoidancePostDR));
            //Apply armor and multipliers for each attack type...
            crits *= (1f - calculatedStats.ConstantDamageReduction) * 2f;
            //crushes *= (100f - calculatedStats.Mitigation) * .015f;
            hits *= (1f - calculatedStats.ConstantDamageReduction);
            calculatedStats.DamageTaken = (hits + crits) * (1f - blockedPercent) * parryHasteDamageMuliplier * (1f + stats.BossAttackSpeedMultiplier);
            calculatedStats.TotalMitigation = 1f - calculatedStats.DamageTaken;

            calculatedStats.SurvivalPointsRaw = (stats.Health / (1f - calculatedStats.ConstantDamageReduction));
            double survivalCap = (double)calcOpts.SurvivalSoftCap / 1000d;
            double survivalRaw = calculatedStats.SurvivalPointsRaw / 1000f;

            //Implement Survival Soft Cap
            if (survivalRaw <= survivalCap){
                calculatedStats.SurvivalPoints = 1000f * (float)survivalRaw;
            } else {
                double x = survivalRaw;
                double cap = survivalCap;
                double fourToTheNegativeFourThirds = Math.Pow(4d, -4d / 3d);
                double topLeft = Math.Pow(((x - cap) / cap) + fourToTheNegativeFourThirds, 1d / 4d);
                double topRight = Math.Pow(fourToTheNegativeFourThirds, 1d / 4d);
                double fracTop = topLeft - topRight;
                double fraction = fracTop / 2d;
                double y = (cap * fraction + cap);
                calculatedStats.SurvivalPoints = 1000f * (float)y;
            }

            calculatedStats.MitigationPoints = (17000f / calculatedStats.DamageTaken); // / (buffs.ShadowEmbrace ? 0.95f : 1f);

            #region WotLKResistance
            // Call new resistance formula and apply talent damage reduction
            // As for other survival, only use guaranteed reduction (MinimumResist), no luck
            calculatedStats.NatureSurvivalPoints = (float)(stats.Health / ((1f - StatConversion.GetMinimumResistance(targetLevel, character.Level, stats.NatureResistance, 0)) * (1f - 0.04 * character.DruidTalents.ProtectorOfThePack)));
            calculatedStats.FrostSurvivalPoints  = (float)(stats.Health / ((1f - StatConversion.GetMinimumResistance(targetLevel, character.Level, stats.FrostResistance, 0)) * (1f - 0.04 * character.DruidTalents.ProtectorOfThePack)));
            calculatedStats.FireSurvivalPoints   = (float)(stats.Health / ((1f - StatConversion.GetMinimumResistance(targetLevel, character.Level, stats.FireResistance, 0)) * (1f - 0.04 * character.DruidTalents.ProtectorOfThePack)));
            calculatedStats.ShadowSurvivalPoints = (float)(stats.Health / ((1f - StatConversion.GetMinimumResistance(targetLevel, character.Level, stats.ShadowResistance, 0)) * (1f - 0.04 * character.DruidTalents.ProtectorOfThePack)));
            calculatedStats.ArcaneSurvivalPoints = (float)(stats.Health / ((1f - StatConversion.GetMinimumResistance(targetLevel, character.Level, stats.ArcaneResistance, 0)) * (1f - 0.04 * character.DruidTalents.ProtectorOfThePack)));
            #endregion

            //Perform Threat calculations
            CalculateThreat(stats, targetLevel, calculatedStats, character);
            calculatedStats.OverallPoints = calculatedStats.MitigationPoints + 
                calculatedStats.SurvivalPoints + calculatedStats.ThreatPoints;
            return calculatedStats;
        }
        
        /// <summary>
        /// Calculates the threat properties of the Character
        /// </summary>
        /// <param name="stats">The total Stats of the character</param>
        /// <param name="targetLevel">The level of the target</param>
        /// <param name="calculatedStats">The CharacterCalculationsBear object to fill with results</param>
        /// <param name="character">The Character to calculate the threat properties of</param>
        private void CalculateThreat(StatsBear stats, int targetLevel, CharacterCalculationsBear calculatedStats, Character character)
        {
            CalculationOptionsBear calcOpts = character.CalculationOptions as CalculationOptionsBear;
            if (calcOpts == null) calcOpts = new CalculationOptionsBear();
            DruidTalents talents = character.DruidTalents;

            //Establish base multipliers and chances
            int targetArmor = calcOpts.TargetArmor;
            //float armorPenetrationPercent = stats.ArmorPenetration + stats.ArmorPenetrationRating / 1539.529991f;
            //float reducedArmor = targetArmor * (1f - armorPenetrationPercent);
            //float modArmor = 1f - (reducedArmor / ((467.5f * character.Level) + reducedArmor - 22167.5f));
            float modArmor = 1f - StatConversion.GetArmorDamageReduction(character.Level, targetArmor, stats.ArmorPenetration, 0f, stats.ArmorPenetrationRating);

            float critMultiplier = 2f * (1 + stats.BonusCritMultiplier);
            float spellCritMultiplier = 1.5f * (1 + stats.BonusCritMultiplier);

            float hasteBonus = StatConversion.GetPhysicalHasteFromRating(stats.HasteRating, CharacterClass.Druid);
            float attackSpeed = (2.5f) / (1f + hasteBonus);
            attackSpeed = attackSpeed / (1f + stats.PhysicalHaste);

            float hitBonus = StatConversion.GetPhysicalHitFromRating(stats.HitRating, CharacterClass.Druid) + stats.PhysicalHit;
            float expertiseBonus = StatConversion.GetDodgeParryReducFromExpertise(StatConversion.GetExpertiseFromRating(stats.ExpertiseRating, CharacterClass.Druid) + stats.Expertise, CharacterClass.Druid);

            float chanceDodge = Math.Max(0f, StatConversion.WHITE_DODGE_CHANCE_CAP[targetLevel-80] - expertiseBonus);
            float chanceParry = Math.Max(0f, StatConversion.WHITE_PARRY_CHANCE_CAP[targetLevel-80] - expertiseBonus);
            float chanceMiss  = Math.Max(0f, StatConversion.WHITE_MISS_CHANCE_CAP[ targetLevel-80] - hitBonus);
            /*if (targetLevel < 83) {
                chanceParry = Math.Max(0f, 0.05f + 0.005f * (targetLevel - 80f) - expertiseBonus);
                chanceMiss = Math.Max(0f, 0.05f + 0.005f * (targetLevel - 80f) - hitBonus);
            }*/
            float chanceGlance = StatConversion.WHITE_GLANCE_CHANCE_CAP[targetLevel-80]; //0.2335774f;
            float glanceMultiplier = 0.7f;
            float chanceAvoided = chanceMiss + chanceDodge + chanceParry;
            float chanceResisted = Math.Max(0f, ((new float[] { 0.04f, 0.05f, 0.06f, 0.17f })[targetLevel - 80]) -
                (StatConversion.GetSpellHitFromRating(stats.HitRating, CharacterClass.Druid) + stats.SpellHit));

            float rawChanceCrit = StatConversion.GetPhysicalCritFromRating(stats.CritRating, CharacterClass.Druid)
                                + StatConversion.GetPhysicalCritFromAgility(stats.Agility, CharacterClass.Druid)
                                + stats.PhysicalCrit
                                + StatConversion.NPC_LEVEL_CRIT_MOD[targetLevel-80];
            float chanceCrit = rawChanceCrit * (1f - chanceAvoided);
            float chanceCritBleed = character.DruidTalents.PrimalGore > 0 ? chanceCrit : 0f;
            float rawChanceCritSpell = StatConversion.GetSpellCritFromRating(stats.CritRating, CharacterClass.Druid)
                                + StatConversion.GetSpellCritFromIntellect(stats.Intellect, CharacterClass.Druid)
                                + stats.SpellCrit + stats.SpellCritOnTarget
                                + StatConversion.NPC_LEVEL_CRIT_MOD[targetLevel - 80];
            float chanceCritSpell = rawChanceCritSpell * (1f - chanceResisted);
            
            calculatedStats.DodgedAttacks = chanceDodge;
            calculatedStats.ParriedAttacks = chanceParry;
            calculatedStats.MissedAttacks = chanceMiss;

            float baseDamage = 137f + (stats.AttackPower / 14f) * 2.5f;
            float bearThreatMultiplier = (29f / 14f) * (1f + stats.ThreatIncreaseMultiplier);

            //Calculate the raw damage of each ability
            float meleeDamageRaw = baseDamage * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * modArmor;
            float maulDamageRaw = (baseDamage + 578) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + stats.BonusMaulDamageMultiplier) * (1f + stats.BonusBleedDamageMultiplier) * modArmor;
            float mangleDamageRaw = (baseDamage * 1.15f + 299) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + stats.BonusMangleDamageMultiplier) * modArmor;
            float swipeDamageRaw = (stats.AttackPower * 0.063f + 108f) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + stats.BonusSwipeDamageMultiplier) * (1f + stats.BonusBearSwipeDamageMultiplier) * modArmor;
            float faerieFireDamageRaw = (stats.AttackPower * 0.15f + 1f) * (1f + stats.BonusDamageMultiplier) * (1f + stats.BonusNatureDamageMultiplier);
            float lacerateDamageRaw = (stats.AttackPower * 0.01f + 88f) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * modArmor * (1f + stats.BonusLacerateDamageMultiplier);
            float lacerateDamageDotRaw = (stats.AttackPower * 0.01f + 64f) * 5f /*stack size*/ * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + stats.BonusBleedDamageMultiplier) * (1f + stats.BonusLacerateDamageMultiplier);

            //Calculate the average damage of each ability, including crits
            float meleeDamageAverage = (chanceCrit * (meleeDamageRaw * critMultiplier)) + //Damage from crits
                            (chanceGlance * (meleeDamageRaw * glanceMultiplier)) + //Damage from glances
                            ((1f - chanceCrit - chanceAvoided - chanceGlance) * (meleeDamageRaw)); //Damage from hits
            float maulDamageAverage = (chanceCrit * (maulDamageRaw * critMultiplier)) + ((1f - chanceCrit - chanceAvoided) * (maulDamageRaw));
            float mangleDamageAverage = (chanceCrit * (mangleDamageRaw * critMultiplier)) + ((1f - chanceCrit - chanceAvoided) * (mangleDamageRaw));
            float swipeDamageAverage = (chanceCrit * (swipeDamageRaw * critMultiplier)) + ((1f - chanceCrit - chanceAvoided) * (swipeDamageRaw));
            float faerieFireDamageAverage = (chanceCritSpell * (faerieFireDamageRaw * spellCritMultiplier)) + ((1f - chanceCritSpell - chanceResisted) * (faerieFireDamageRaw));
            float lacerateDamageAverage = (chanceCrit * (lacerateDamageRaw * critMultiplier)) + ((1f - chanceCrit - chanceAvoided) * (lacerateDamageRaw));
            float lacerateDamageDotAverage = (chanceCritBleed * (lacerateDamageDotRaw * critMultiplier)) + ((1f - chanceCritBleed) * (lacerateDamageDotRaw));

            //Calculate the raw threat of each attack
            float meleeThreatRaw = bearThreatMultiplier * meleeDamageRaw;
            float maulThreatRaw = bearThreatMultiplier * (maulDamageRaw + 424f / 1f); //NOTE! This assumes 1 target. If Maul hits 2 targets, replace 1 with 2.
            float mangleThreatRaw = bearThreatMultiplier * mangleDamageRaw * (1f + stats.BonusMangleBearThreat);
            float swipeThreatRaw = bearThreatMultiplier * swipeDamageRaw * 1.5f;
            float faerieFireThreatRaw = bearThreatMultiplier * (faerieFireDamageRaw + 632f);
            float lacerateThreatRaw = bearThreatMultiplier * (lacerateDamageRaw + 1031f) / 2f;
            float lacerateDotThreatRaw = bearThreatMultiplier * lacerateDamageDotRaw / 2f;
            
            //Calculate the average threat of each attack, including crits
            float meleeThreatAverage = bearThreatMultiplier * meleeDamageAverage;
            float maulThreatAverage = bearThreatMultiplier * (maulDamageAverage + (424f * (1f - chanceAvoided)) / 1f); //NOTE! This assumes 1 target. If Maul hits 2 targets, replace 1 with 2.
            float mangleThreatAverage = bearThreatMultiplier * mangleDamageAverage * (1f + stats.BonusMangleBearThreat);
            float swipeThreatAverage = bearThreatMultiplier * swipeDamageAverage * 1.5f;
            float faerieFireThreatAverage = bearThreatMultiplier * (faerieFireDamageAverage + (632f * (1f - chanceResisted)));
            float lacerateThreatAverage = bearThreatMultiplier * (lacerateDamageAverage + (1031f * (1f - chanceAvoided))) / 2f;
            float lacerateDotThreatAverage = bearThreatMultiplier * lacerateDamageDotAverage / 2f;
            
            //Calculate effective rage costs for the possible outcomes of each ability, and the average
            float meleeRageHit = meleeDamageRaw / 120f + 35f / 9f;
            float meleeRageGlance = (meleeDamageRaw * glanceMultiplier) / 120f + 35f / 9f;
            float meleeRageCrit = (meleeDamageRaw * critMultiplier) / 120f + 70f / 9f;
            float meleeRageDodgeParry = 9.5f;
            float meleeRageAverage = (chanceCrit * meleeRageCrit) + //Rage from crits
                            (chanceGlance * meleeRageGlance) + //Rage from glances
                            ((chanceDodge + chanceParry) * meleeRageDodgeParry) + //Rage from dodges/parries
                            ((1f - chanceCrit - chanceAvoided - chanceGlance) * meleeRageHit); //Rage from hits

            float maulRageHit = 15f - talents.Ferocity + meleeRageAverage;
            float maulRageCrit = 15f - talents.Ferocity - (2.5f * talents.PrimalFury) + meleeRageAverage;
            float maulRageAvoided = ((15f - talents.Ferocity) * 0.2f) + meleeRageAverage;
            float maulRageAverage = (chanceCrit * maulRageCrit) + ((1f - chanceCrit - chanceAvoided) * maulRageHit) + (chanceAvoided * maulRageAvoided);

            float mangleRageHit = 20f - talents.Ferocity;
            float mangleRageCrit = 20f - talents.Ferocity - (2.5f * talents.PrimalFury);
            float mangleRageDodgeParry = (20f - talents.Ferocity) * 0.2f;
            float mangleRageAverage = (chanceCrit * mangleRageCrit) + ((1f - chanceCrit - chanceDodge - chanceParry) * mangleRageHit) + ((chanceDodge + chanceParry) + mangleRageDodgeParry);
            
            float swipeRageHit = 20f - talents.Ferocity;
            float swipeRageCrit = 20f - talents.Ferocity - (2.5f * talents.PrimalFury);
            float swipeRageDodgeParry = (20f - talents.Ferocity) * 0.2f;
            float swipeRageAverage = (chanceCrit * swipeRageCrit) + ((1f - chanceCrit - chanceDodge - chanceParry) * swipeRageHit) + ((chanceDodge + chanceParry) + swipeRageDodgeParry);
            
            float lacerateRageHit = 15f - talents.ShreddingAttacks;
            float lacerateRageCrit = 15f - talents.ShreddingAttacks - (2.5f * talents.PrimalFury);
            float lacerateRageDodgeParry = (15f - talents.ShreddingAttacks) * 0.2f;
            float lacerateRageAverage = (chanceCrit * lacerateRageCrit) + ((1f - chanceCrit - chanceDodge - chanceParry) * lacerateRageHit) + ((chanceDodge + chanceParry) + lacerateRageDodgeParry);
            
            //Calculate the threat per rage of each ability, on average
            float maulTPR = maulThreatAverage / maulRageAverage;
            float maulDPR = maulDamageAverage / maulRageAverage;
            float mangleTPR = mangleThreatAverage / mangleRageAverage;
            float mangleDPR = mangleDamageAverage / mangleRageAverage;
            float swipeTPR = swipeThreatAverage / swipeRageAverage;
            float swipeDPR = swipeDamageAverage / swipeRageAverage;
            float lacerateTPR = lacerateThreatAverage / lacerateRageAverage;
            float lacerateDPR = lacerateDamageAverage / lacerateRageAverage;


            //Use the BearRotationCalculator to model the potential rotations, and get their results
            BearRotationCalculator rotationCalculator = new BearRotationCalculator(meleeDamageAverage, maulDamageAverage, mangleDamageAverage, swipeDamageAverage,
                faerieFireDamageAverage, lacerateDamageAverage, lacerateDamageDotAverage, meleeThreatAverage, maulThreatAverage, mangleThreatAverage, swipeThreatAverage,
                faerieFireThreatAverage, lacerateThreatAverage, lacerateDotThreatAverage, 6f - stats.MangleCooldownReduction, attackSpeed);

            BearRotationCalculator.BearRotationCalculation rotationCalculationDPS, rotationCalculationTPS;
            rotationCalculationDPS = rotationCalculationTPS = new BearRotationCalculator.BearRotationCalculation();

            //Loop through the potential rotations...
            for (int useMaul = 0; useMaul < 2; useMaul++)
                for (int useMangle = 0; useMangle < 2; useMangle++)
                    for (int useSwipe = 0; useSwipe < 2; useSwipe++)
                        for (int useFaerieFire = 0; useFaerieFire < 2; useFaerieFire++)
                            for (int useLacerate = 0; useLacerate < 2; useLacerate++)
                            {
                                //...and feed them to the BearRotationCalculator
                                BearRotationCalculator.BearRotationCalculation rotationCalculation = 
                                    rotationCalculator.GetRotationCalculations(useMaul == 1,
                                    useMangle == 1, useSwipe == 1, useFaerieFire == 1, useLacerate == 1);
                                //Keep track of the best rotation for both DPS and TPS
                                if (rotationCalculation.DPS > rotationCalculationDPS.DPS)
                                    rotationCalculationDPS = rotationCalculation;
                                if (rotationCalculation.TPS > rotationCalculationTPS.TPS)
                                    rotationCalculationTPS = rotationCalculation;
                            }

            float abomDPS = (stats.MoteOfAnger * meleeDamageAverage);
            float abomTPS = (stats.MoteOfAnger * meleeThreatAverage);
            
            //Fill the results into the CharacterCalculationsBear object
            calculatedStats.ThreatPoints = (rotationCalculationTPS.TPS + abomTPS) * calcOpts.ThreatScale;
            calculatedStats.HighestDPSRotation = rotationCalculationDPS;
            calculatedStats.HighestTPSRotation = rotationCalculationTPS;
            calculatedStats.SwipeRotation = rotationCalculator.GetRotationCalculations(null, false, true, false, false);
            calculatedStats.CustomRotation = rotationCalculator.GetRotationCalculations(calcOpts.CustomUseMaul,
                calcOpts.CustomUseMangle, calcOpts.CustomUseSwipe, calcOpts.CustomUseFaerieFire, calcOpts.CustomUseLacerate);

            calculatedStats.AttackSpeed = attackSpeed;
            calculatedStats.MangleCooldown = 6f - stats.MangleCooldownReduction;

            calculatedStats.MeleeDamageRaw = (float)Math.Round(meleeDamageRaw);
            calculatedStats.MeleeDamageAverage = (float)Math.Round(meleeDamageAverage);
            calculatedStats.MeleeThreatRaw = (float)Math.Round(meleeThreatRaw);
            calculatedStats.MeleeThreatAverage = (float)Math.Round(meleeThreatAverage);

            calculatedStats.MaulDamageRaw = (float)Math.Round(maulDamageRaw);
            calculatedStats.MaulDamageAverage = (float)Math.Round(maulDamageAverage);
            calculatedStats.MaulThreatRaw = (float)Math.Round(maulThreatRaw);
            calculatedStats.MaulThreatAverage = (float)Math.Round(maulThreatAverage);

            calculatedStats.MangleDamageRaw = (float)Math.Round(mangleDamageRaw);
            calculatedStats.MangleDamageAverage = (float)Math.Round(mangleDamageAverage);
            calculatedStats.MangleThreatRaw = (float)Math.Round(mangleThreatRaw);
            calculatedStats.MangleThreatAverage = (float)Math.Round(mangleThreatAverage);

            calculatedStats.SwipeDamageRaw = (float)Math.Round(swipeDamageRaw);
            calculatedStats.SwipeDamageAverage = (float)Math.Round(swipeDamageAverage);
            calculatedStats.SwipeThreatRaw = (float)Math.Round(swipeThreatRaw);
            calculatedStats.SwipeThreatAverage = (float)Math.Round(swipeThreatAverage);

            calculatedStats.FaerieFireDamageRaw = (float)Math.Round(faerieFireDamageRaw);
            calculatedStats.FaerieFireDamageAverage = (float)Math.Round(faerieFireDamageAverage);
            calculatedStats.FaerieFireThreatRaw = (float)Math.Round(faerieFireThreatRaw);
            calculatedStats.FaerieFireThreatAverage = (float)Math.Round(faerieFireThreatAverage);

            calculatedStats.LacerateDamageRaw = (float)Math.Round(lacerateDamageRaw);
            calculatedStats.LacerateDamageAverage = (float)Math.Round(lacerateDamageAverage);
            calculatedStats.LacerateThreatRaw = (float)Math.Round(lacerateThreatRaw);
            calculatedStats.LacerateThreatAverage = (float)Math.Round(lacerateThreatAverage);

            calculatedStats.LacerateDotDamageRaw = (float)Math.Round(lacerateDamageDotRaw);
            calculatedStats.LacerateDotDamageAverage = (float)Math.Round(lacerateDamageDotAverage);
            calculatedStats.LacerateDotThreatRaw = (float)Math.Round(lacerateDotThreatRaw);
            calculatedStats.LacerateDotThreatAverage = (float)Math.Round(lacerateDotThreatAverage);

            calculatedStats.MaulTPR = maulTPR;
            calculatedStats.MaulDPR = maulDPR;
            calculatedStats.MangleTPR = mangleTPR;
            calculatedStats.MangleDPR = mangleDPR;
            calculatedStats.SwipeTPR = swipeTPR;
            calculatedStats.SwipeDPR = swipeDPR;
            calculatedStats.LacerateTPR = lacerateTPR;
            calculatedStats.LacerateDPR = lacerateDPR;
        }

        /// <summary>
        /// Gets the total Stats of the Character
        /// </summary>
        /// <param name="character">The Character to get the total Stats of</param>
        /// <param name="additionalItem">An additional item to grant the Character the stats of (as if it were worn)</param>
        /// <returns>The total stats for the Character</returns>
        public override Stats GetCharacterStats(Character character, Item additionalItem)
        {
            CalculationOptionsBear calcOpts = character.CalculationOptions as CalculationOptionsBear;
            if (calcOpts == null) calcOpts = new CalculationOptionsBear();
            
            /* TODO:
             * Threat-only:
             * Ferocity (5: -5rage cost to Maul, Swipe, Mangle)
             --* Feral Instinct (3: 30% damage buff to Swipe)
             --* Savage Fury (2: 20% damage buff to Mangle, Maul)
             --* Sharpened Claws (3: 6% crit)
             * Shredding Attacks (2: -2rage cost to Lacerate)
             --* Predatory Strikes (3:  105 ap, plus 20% of weapon ap)
             --* Primal Fury (2: 5rage on crits)
             --* Primal Precision (2: 10 expertise)
             --* Leader of the Pack (1: 5% crit)
             --* Predatory Instincts (3: 15% damage buff to crits)
             --* King of the Jungle (3: 15% damage buff during Enrage)
             -* Mangle (1: Mangle)
             --* Improved Mangle (3: -1.5sec cooldown to Mangle)
             --* Rend and Tear (5: 20% damage buff to Maul)
             * Berserk (1: Berserk)
             --* Naturalist (5: 10% damage)
             * Omen of Clarity (1: Omen of Clarity)
             --* Master Shapeshifter (2: 4% damage)
             * 
             * Mitigation/Survival:
             --* Thick Hide (3: 10% armor buff)
             --* Feral Swiftness (2: 4% dodge)
             -* Natural Reaction (3: 6% dodge, 3 rage on dodges)
             --* Heart of the Wild (5: 20% sta)
             --* Survival of the Fittest (3: 6% stats, 6% anticrit)
             -* Mother Bear (3: 6% ap, 12% less damage taken)
             * 
            */

            DruidTalents talents = character.DruidTalents;
            
            StatsBear statsTotal = new StatsBear()
            {
                BonusMangleDamageMultiplier = (1f + 0.1f * talents.SavageFury) * (talents.GlyphOfMangle ? 1.1f : 1.0f) - 1f,
                BonusMaulDamageMultiplier = (1f + 0.1f * talents.SavageFury) * (1f + 0.04f * talents.RendAndTear) - 1f,
                BonusEnrageDamageMultiplier = 0.05f * talents.KingOfTheJungle,
                MangleCooldownReduction = (0.5f * talents.ImprovedMangle),
                BonusSwipeDamageMultiplier = 0.1f * talents.FeralInstinct,
                
                PhysicalCrit = 0.02f * talents.SharpenedClaws
                             + ((character.ActiveBuffsContains("Leader of the Pack")
                                 || character.ActiveBuffsContains("Rampage"))
                                ? 0f : 0.05f * talents.LeaderOfThePack),
                Dodge = 0.02f * talents.FeralSwiftness + 0.02f * talents.NaturalReaction,
                BonusStaminaMultiplier = (1f + 0.02f * talents.HeartOfTheWild) * (1f + 0.02f * talents.SurvivalOfTheFittest) * (1f + 0.01f * talents.ImprovedMarkOfTheWild) - 1f,
                BonusAgilityMultiplier = (1f + 0.02f * talents.SurvivalOfTheFittest) * (1f + 0.01f * talents.ImprovedMarkOfTheWild) - 1f,
                BonusStrengthMultiplier = (1f + 0.02f * talents.SurvivalOfTheFittest) * (1f + 0.01f * talents.ImprovedMarkOfTheWild) - 1f,
                CritChanceReduction = 0.02f * talents.SurvivalOfTheFittest,
                BonusAttackPowerMultiplier = 0.02f * talents.ProtectorOfThePack,
                BonusPhysicalDamageMultiplier = (1f + 0.02f * talents.Naturalist) * (1f + 0.02f * talents.MasterShapeshifter) - 1f,
                BonusRageOnCrit = (2.5f * talents.PrimalFury),
                Expertise = 5f * talents.PrimalPrecision,
                AttackPower = (character.Level / 2f) * talents.PredatoryStrikes,
                DamageTakenMultiplier = -0.04f * talents.ProtectorOfThePack,
                BonusBleedDamageMultiplier = (character.ActiveBuffsContains("Mangle") ||
                    character.ActiveBuffsContains("Trauma") ? 0f : 0.3f * talents.Mangle),
                BaseArmorMultiplier = 4.7f * (1f + 0.1f * talents.ThickHide / 3f) * (1f + 0.11f * talents.SurvivalOfTheFittest) - 1f,
            };
            statsTotal.Accumulate(BaseStats.GetBaseStats(80, character.Class, character.Race, BaseStats.DruidForm.Bear));
            statsTotal.Accumulate(GetItemStats(character, additionalItem));
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
            statsTotal.Strength = (float)Math.Floor(statsTotal.Strength * (1f + statsTotal.BonusStrengthMultiplier));
            statsTotal.Agility = (float)Math.Floor(statsTotal.Agility * (1f + statsTotal.BonusAgilityMultiplier));
            statsTotal.AttackPower += (float)Math.Floor(statsTotal.Strength) * 2f + fap + predatoryStrikesAP;
            statsTotal.AttackPower = (float)Math.Floor(statsTotal.AttackPower * (1f + statsTotal.BonusAttackPowerMultiplier));
            statsTotal.Health += ((statsTotal.Stamina - 20f) * 10f) + 20f;
            statsTotal.Health *= (1f + statsTotal.BonusHealthMultiplier);
            statsTotal.Armor *= 1f + statsTotal.BaseArmorMultiplier;
            statsTotal.Armor += 2f * (float)Math.Floor(statsTotal.Agility) + statsTotal.BonusArmor;
            statsTotal.Armor = (float)Math.Floor(statsTotal.Armor * (1f + statsTotal.BonusArmorMultiplier));
            statsTotal.NatureResistance += statsTotal.NatureResistanceBuff;
            statsTotal.FireResistance += statsTotal.FireResistanceBuff;
            statsTotal.FrostResistance += statsTotal.FrostResistanceBuff;
            statsTotal.ShadowResistance += statsTotal.ShadowResistanceBuff;
            statsTotal.ArcaneResistance += statsTotal.ArcaneResistanceBuff;

            int targetLevel = calcOpts.TargetLevel;

            float hasteBonus = StatConversion.GetHasteFromRating(statsTotal.HasteRating, CharacterClass.Druid);
            float attackSpeed = (2.5f / (1f + hasteBonus)) / (1f + statsTotal.PhysicalHaste);
            float meleeHitInterval = 1f / (1f / attackSpeed + 1f / 1.5f);

            float hitBonus = StatConversion.GetPhysicalHitFromRating(statsTotal.HitRating) + statsTotal.PhysicalHit;
            float expertiseBonus = StatConversion.GetDodgeParryReducFromExpertise(StatConversion.GetExpertiseFromRating(statsTotal.ExpertiseRating, CharacterClass.Druid) + statsTotal.Expertise, CharacterClass.Druid);
            float chanceDodge = Math.Max(0f, StatConversion.WHITE_DODGE_CHANCE_CAP[targetLevel-80] - expertiseBonus);
            float chanceParry = Math.Max(0f, StatConversion.WHITE_PARRY_CHANCE_CAP[targetLevel-80] - expertiseBonus);
            float chanceMiss  = Math.Max(0f, StatConversion.WHITE_MISS_CHANCE_CAP[ targetLevel-80] - hitBonus);
            float chanceAvoided = chanceMiss + chanceDodge + chanceParry;

            float rawChanceCrit = StatConversion.GetPhysicalCritFromRating(statsTotal.CritRating, CharacterClass.Druid)
                                + StatConversion.GetPhysicalCritFromAgility(statsTotal.Agility, CharacterClass.Druid)
                                + statsTotal.PhysicalCrit
                                + StatConversion.NPC_LEVEL_CRIT_MOD[targetLevel-80];
            float chanceCrit = rawChanceCrit * (1f - chanceAvoided);
            float chanceHit = 1f - chanceAvoided;

            float levelDifference = (targetLevel - character.Level) * 0.002f;
            float baseAgi = character.Race == CharacterRace.NightElf ? 87 : 77;

            float defSkill = (float)Math.Floor(StatConversion.GetDefenseFromRating(statsTotal.DefenseRating, CharacterClass.Druid));
            float dodgeNonDR = statsTotal.Dodge - levelDifference + StatConversion.GetDodgeFromAgility(baseAgi, CharacterClass.Druid);
            float missNonDR = statsTotal.Miss - levelDifference;
            float dodgePreDR = StatConversion.GetDodgeFromAgility(statsTotal.Agility - baseAgi, CharacterClass.Druid)
                             + StatConversion.GetDodgeFromRating(statsTotal.DodgeRating, CharacterClass.Druid)
                             + defSkill * StatConversion.DEFENSE_RATING_AVOIDANCE_MULTIPLIER / 100f;
            float missPreDR = (defSkill * StatConversion.DEFENSE_RATING_AVOIDANCE_MULTIPLIER / 100f);
            float dodgePostDR = 0.01f / (1f / 116.890707f + 0.00972f / dodgePreDR);
            float missPostDR = 0.01f / (1f / 16f + 0.00972f / missPreDR);
            float dodgeTotal = dodgeNonDR + dodgePostDR;
            float missTotal = missNonDR + missPostDR;

            float TargAttackSpeed = calcOpts.TargetAttackSpeed * (1f - statsTotal.BossAttackSpeedMultiplier);

            Stats statsProcs = new Stats();
            //float uptime;
            foreach (SpecialEffect effect in statsTotal.SpecialEffects())
            {
                switch (effect.Trigger)
                {
                    case Trigger.Use:
                        effect.AccumulateAverageStats(statsProcs, 0f, 1f, 2.5f);
                        break;
                    case Trigger.MeleeHit:
                    case Trigger.PhysicalHit:
                        effect.AccumulateAverageStats(statsProcs, meleeHitInterval, chanceHit, 2.5f);
                        break;
                    case Trigger.MeleeAttack:
                        if (effect.Stats.MoteOfAnger > 0)
                        {
                            // When in effect stats, MoteOfAnger is % of melee hits
                            // When in character stats, MoteOfAnger is average procs per second
                            statsProcs.MoteOfAnger = effect.Stats.MoteOfAnger * effect.GetAverageProcsPerSecond(meleeHitInterval, 1f, 2.5f, 300f) / effect.MaxStack;
                        }
                        else
                        {
                            effect.AccumulateAverageStats(statsProcs, meleeHitInterval, 1f, 2.5f);
                        }
                        break;
                    case Trigger.MeleeCrit:
                    case Trigger.PhysicalCrit:
                        effect.AccumulateAverageStats(statsProcs, meleeHitInterval, chanceCrit, 2.5f);
                        break;
                    case Trigger.DoTTick:
                        effect.AccumulateAverageStats(statsProcs, 3f, 1f, 2.5f);
                        break;
                    case Trigger.DamageDone:
                        effect.AccumulateAverageStats(statsProcs, meleeHitInterval / 2f, 1f, 2.5f);
                        break;
                    case Trigger.DamageOrHealingDone:
                        effect.AccumulateAverageStats(statsProcs, meleeHitInterval / 2f, 1f, 2.5f); // also needs healing
                        break;
                    case Trigger.MangleBearHit:
                        if (talents.Mangle > 0)
                            effect.AccumulateAverageStats(statsProcs, 6f - statsTotal.MangleCooldownReduction, chanceHit, 2.5f);
                        break;
                    case Trigger.MangleCatOrShredOrInfectedWoundsHit:
                        if (talents.Mangle > 0)
                            effect.AccumulateAverageStats(statsProcs, 1f /
                                (1f / (6f - statsTotal.MangleCooldownReduction) + //Mangles Per Second
                                1f / meleeHitInterval) //Mauls Per Second
                                , chanceHit, 2.5f);
                        break;
                    case Trigger.SwipeBearOrLacerateHit:
                        effect.AccumulateAverageStats(statsProcs, 2.25f, chanceHit, 2.5f);
                        break;
                    case Trigger.LacerateTick:
                        effect.AccumulateAverageStats(statsProcs, 3f, 1f, 2.5f);
                        break;
                    case Trigger.DamageTaken:
                        effect.AccumulateAverageStats(statsProcs, TargAttackSpeed * 0.8f, 1f - 0.8f * (dodgeTotal + missTotal)); //Assume you get hit by other things, like dots, aoes, etc, making you get targeted with damage 25% more often than the boss, and half the hits you take are unavoidable.
                        break;
                    case Trigger.DamageTakenPhysical:
                        effect.AccumulateAverageStats(statsProcs, TargAttackSpeed, 1f - (dodgeTotal + missTotal)); //Assume you get hit by other things, like dots, aoes, etc, making you get targeted with damage 25% more often than the boss, and half the hits you take are unavoidable.
                        break;
                }
            }

            statsProcs.Agility += statsProcs.HighestStat + statsProcs.Paragon;
            statsProcs.Stamina = (float)Math.Floor(statsProcs.Stamina * (1f + statsTotal.BonusStaminaMultiplier));
            statsProcs.Strength = (float)Math.Floor(statsProcs.Strength * (1f + statsTotal.BonusStrengthMultiplier));
            statsProcs.Agility = (float)Math.Floor(statsProcs.Agility * (1f + statsTotal.BonusAgilityMultiplier));
            statsProcs.AttackPower += statsProcs.Strength * 2f;
            statsProcs.AttackPower = (float)Math.Floor(statsProcs.AttackPower * (1f + statsTotal.BonusAttackPowerMultiplier));
            statsProcs.Health += (float)Math.Floor(statsProcs.Stamina * 10f) + (float)Math.Floor(statsProcs.BattlemasterHealth);
            statsProcs.Health *= (1f + statsProcs.BonusHealthMultiplier);
            statsProcs.Armor += 2f * (float)Math.Floor(statsProcs.Agility) + statsProcs.BonusArmor;
            statsProcs.Armor = (float)Math.Floor(statsProcs.Armor * (1f + statsTotal.BonusArmorMultiplier));
            statsTotal.Accumulate(statsProcs);

            return statsTotal;
        }


        //NOTE: This is currently unused, because it doesn't account for procs which are both mitigation and survival (ie armor and agility procs)
        private void AccumulateSpecialEffect(Stats statsProcs, Stats statsEffect, float effectUptime, float temporarySurvivalScale)
        {
            if (temporarySurvivalScale != 1f && statsEffect.Armor + statsEffect.Health + statsEffect.Stamina > 0f)
            {
                //Subject to Temporary Survival scaling
                if (temporarySurvivalScale < 1f)
                {
                    statsProcs.Accumulate(statsEffect, effectUptime * temporarySurvivalScale);
                }
                else
                {
                    statsProcs.Accumulate(statsEffect, 1f - ((2f - temporarySurvivalScale) * (1f - effectUptime)));
                }
            }
            else
            {
                statsProcs.Accumulate(statsEffect, effectUptime);
            }
        }

        /// <summary>
        /// Gets data for a custom chart that Rawr.Bear provides
        /// </summary>
        /// <param name="character">The Character to get the chart data for</param>
        /// <param name="chartName">The name of the custom chart to get data for</param>
        /// <returns>The data for the custom chart</returns>
        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
        {
            switch (chartName)
            {
                case "Combat Table":
                    CharacterCalculationsBear currentCalculationsBear = GetCharacterCalculations(character) as CharacterCalculationsBear;
                    ComparisonCalculationBear calcMiss = new ComparisonCalculationBear();
                    ComparisonCalculationBear calcDodge = new ComparisonCalculationBear();
                    ComparisonCalculationBear calcCrit = new ComparisonCalculationBear();
                    //ComparisonCalculationBear calcCrush = new ComparisonCalculationBear();
                    ComparisonCalculationBear calcHit = new ComparisonCalculationBear();
                    if (currentCalculationsBear != null)
                    {
                        calcMiss.Name = "    Miss    ";
                        calcDodge.Name = "   Dodge   ";
                        calcCrit.Name = "  Crit  ";
                        //calcCrush.Name = " Crush ";
                        calcHit.Name = "Hit";

                        float crits = 0.02f + (0.002f * (currentCalculationsBear.TargetLevel - character.Level)) - currentCalculationsBear.CappedCritReduction;
                        //float crushes = currentCalculationsBear.TargetLevel == 73 ? Math.Max(Math.Min(100f - (crits + (currentCalculationsBear.AvoidancePreDR)), 15f) - currentCalculationsBear.BasicStats.CritChanceReduction, 0f) : 0f;
                        float hits = Math.Max(1f - (Math.Max(0f, crits) + /*Math.Max(crushes, 0)*/ + (currentCalculationsBear.AvoidancePreDR)), 0f);
                        
                        calcMiss.OverallPoints = calcMiss.MitigationPoints = currentCalculationsBear.Miss * 100f;
                        calcDodge.OverallPoints = calcDodge.MitigationPoints = currentCalculationsBear.Dodge * 100f;
                        calcCrit.OverallPoints = calcCrit.SurvivalPoints = crits * 100f;
                        //calcCrush.OverallPoints = calcCrush.SurvivalPoints = crushes;
                        calcHit.OverallPoints = calcHit.SurvivalPoints = hits * 100f;
                    }
                    return new ComparisonCalculationBase[] { calcMiss, calcDodge, calcCrit, /*calcCrush,*/ calcHit };

                //Not used anymore
                //case "Relative Stat Values": 
                //    CharacterCalculationsBear calcBaseValue = GetCharacterCalculations(character) as CharacterCalculationsBear;
                //    //CharacterCalculationsBear calcAgiValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Agility = 10 } }) as CharacterCalculationsBear;
                //    //CharacterCalculationsBear calcACValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Armor = 10 } }) as CharacterCalculationsBear;
                //    //CharacterCalculationsBear calcStaValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Stamina = 10 } }) as CharacterCalculationsBear;
                //    //CharacterCalculationsBear calcDefValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { DefenseRating = 1 } }) as CharacterCalculationsBear;
                //    CharacterCalculationsBear calcDodgeValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { DodgeRating = 1 } }) as CharacterCalculationsBear;
                //    CharacterCalculationsBear calcHealthValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Health = 1 } }) as CharacterCalculationsBear;
                //    CharacterCalculationsBear calcResilValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Resilience = 1 } }) as CharacterCalculationsBear;
                //    //CharacterCalculationsBear calcStrengthValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Strength = 1 } }) as CharacterCalculationsBear;
                //    CharacterCalculationsBear calcAPValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { AttackPower = 1 } }) as CharacterCalculationsBear;
                //    CharacterCalculationsBear calcExpertiseValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { ExpertiseRating = 1 } }) as CharacterCalculationsBear;
                //    CharacterCalculationsBear calcHitValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { HitRating = 1 } }) as CharacterCalculationsBear;
                //    CharacterCalculationsBear calcPenValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { ArmorPenetration = 1 } }) as CharacterCalculationsBear;
                //    CharacterCalculationsBear calcHasteValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { HasteRating = 1 } }) as CharacterCalculationsBear;
                //    CharacterCalculationsBear calcDamageValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { WeaponDamage = 1 } }) as CharacterCalculationsBear;
                //    CharacterCalculationsBear calcCritValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { CritRating = 1 } }) as CharacterCalculationsBear;
                
                //    //Differential Calculations for Agi
                //    CharacterCalculationsBear calcAtAdd = calcBaseValue;
                //    float agiToAdd = 0f;
                //    while (calcBaseValue.OverallPoints == calcAtAdd.OverallPoints && agiToAdd < 2)
                //    {
                //        agiToAdd += 0.01f;
                //        calcAtAdd = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Agility = agiToAdd } }) as CharacterCalculationsBear;
                //    }

                //    CharacterCalculationsBear calcAtSubtract = calcBaseValue;
                //    float agiToSubtract = 0f;
                //    while (calcBaseValue.OverallPoints == calcAtSubtract.OverallPoints && agiToSubtract > -2)
                //    {
                //        agiToSubtract -= 0.01f;
                //        calcAtSubtract = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Agility = agiToSubtract } }) as CharacterCalculationsBear;
                //    }
                //    agiToSubtract += 0.01f;

                //    ComparisonCalculationBear comparisonAgi = new ComparisonCalculationBear() { 
                //        Name = string.Format("Agility ({0})", agiToAdd-agiToSubtract), 
                //        OverallPoints =     (calcAtAdd.OverallPoints - calcBaseValue.OverallPoints) / (agiToAdd - agiToSubtract),
                //        MitigationPoints =  (calcAtAdd.MitigationPoints - calcBaseValue.MitigationPoints) / (agiToAdd - agiToSubtract), 
                //        SurvivalPoints =    (calcAtAdd.SurvivalPoints - calcBaseValue.SurvivalPoints) / (agiToAdd - agiToSubtract),
                //        ThreatPoints =      (calcAtAdd.ThreatPoints - calcBaseValue.ThreatPoints) / (agiToAdd - agiToSubtract)};


                //    //Differential Calculations for Str
                //    calcAtAdd = calcBaseValue;
                //    float strToAdd = 0f;
                //    while (calcBaseValue.OverallPoints == calcAtAdd.OverallPoints && strToAdd < 2)
                //    {
                //        strToAdd += 0.01f;
                //        calcAtAdd = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Strength = strToAdd } }) as CharacterCalculationsBear;
                //    }

                //    calcAtSubtract = calcBaseValue;
                //    float strToSubtract = 0f;
                //    while (calcBaseValue.OverallPoints == calcAtSubtract.OverallPoints && strToSubtract > -2)
                //    {
                //        strToSubtract -= 0.01f;
                //        calcAtSubtract = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Strength = strToSubtract } }) as CharacterCalculationsBear;
                //    }
                //    strToSubtract += 0.01f;

                //    ComparisonCalculationBear comparisonStr = new ComparisonCalculationBear()
                //    {
                //        Name = string.Format("Strength ({0})", strToAdd-strToSubtract),
                //        OverallPoints = (calcAtAdd.OverallPoints - calcBaseValue.OverallPoints) / (strToAdd - strToSubtract),
                //        MitigationPoints = (calcAtAdd.MitigationPoints - calcBaseValue.MitigationPoints) / (strToAdd - strToSubtract),
                //        SurvivalPoints = (calcAtAdd.SurvivalPoints - calcBaseValue.SurvivalPoints) / (strToAdd - strToSubtract),
                //        ThreatPoints = (calcAtAdd.ThreatPoints - calcBaseValue.ThreatPoints) / (strToAdd - strToSubtract)
                //    };


                //    //Differential Calculations for Def
                //    calcAtAdd = calcBaseValue;
                //    float defToAdd = 0f;
                //    while (calcBaseValue.OverallPoints == calcAtAdd.OverallPoints && defToAdd < 20)
                //    {
                //        defToAdd += 0.01f;
                //        calcAtAdd = GetCharacterCalculations(character, new Item() { Stats = new Stats() { DefenseRating = defToAdd } }) as CharacterCalculationsBear;
                //    }

                //    calcAtSubtract = calcBaseValue;
                //    float defToSubtract = 0f;
                //    while (calcBaseValue.OverallPoints == calcAtSubtract.OverallPoints && defToSubtract > -20)
                //    {
                //        defToSubtract -= 0.01f;
                //        calcAtSubtract = GetCharacterCalculations(character, new Item() { Stats = new Stats() { DefenseRating = defToSubtract } }) as CharacterCalculationsBear;
                //    }
                //    defToSubtract += 0.01f;

                //    ComparisonCalculationBear comparisonDef = new ComparisonCalculationBear()
                //    {
                //        Name = string.Format("Defense Rating ({0})", defToAdd-defToSubtract),
                //        OverallPoints = (calcAtAdd.OverallPoints - calcBaseValue.OverallPoints) / (defToAdd - defToSubtract),
                //        MitigationPoints = (calcAtAdd.MitigationPoints - calcBaseValue.MitigationPoints) / (defToAdd - defToSubtract),
                //        SurvivalPoints = (calcAtAdd.SurvivalPoints - calcBaseValue.SurvivalPoints) / (defToAdd - defToSubtract),
                //        ThreatPoints = (calcAtAdd.ThreatPoints - calcBaseValue.ThreatPoints) / (defToAdd - defToSubtract)
                //    };


                //    //Differential Calculations for AC
                //    calcAtAdd = calcBaseValue;
                //    float acToAdd = 0f;
                //    while (calcBaseValue.OverallPoints == calcAtAdd.OverallPoints && acToAdd < 2)
                //    {
                //        acToAdd += 0.01f;
                //        calcAtAdd = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Armor = acToAdd } }) as CharacterCalculationsBear;
                //    }

                //    calcAtSubtract = calcBaseValue;
                //    float acToSubtract = 0f;
                //    while (calcBaseValue.OverallPoints == calcAtSubtract.OverallPoints && acToSubtract > -2)
                //    {
                //        acToSubtract -= 0.01f;
                //        calcAtSubtract = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Armor = acToSubtract } }) as CharacterCalculationsBear;
                //    }
                //    acToSubtract += 0.01f;

                //    ComparisonCalculationBear comparisonAC = new ComparisonCalculationBear() {
                //        Name = string.Format("Armor ({0})", acToAdd-acToSubtract),
                //        OverallPoints = (calcAtAdd.OverallPoints - calcBaseValue.OverallPoints) / (acToAdd - acToSubtract),
                //        MitigationPoints = (calcAtAdd.MitigationPoints - calcBaseValue.MitigationPoints) / (acToAdd - acToSubtract), 
                //        SurvivalPoints = (calcAtAdd.SurvivalPoints - calcBaseValue.SurvivalPoints) / (acToAdd - acToSubtract), 
                //        ThreatPoints = (calcAtAdd.ThreatPoints - calcBaseValue.ThreatPoints) / (acToAdd - acToSubtract)
                //    };


                //    //Differential Calculations for BonusAC
                //    calcAtAdd = calcBaseValue;
                //    float bacToAdd = 0f;
                //    while (calcBaseValue.OverallPoints == calcAtAdd.OverallPoints && bacToAdd < 2)
                //    {
                //        bacToAdd += 0.01f;
                //        calcAtAdd = GetCharacterCalculations(character, new Item() { Stats = new Stats() { BonusArmor = bacToAdd } }) as CharacterCalculationsBear;
                //    }

                //    calcAtSubtract = calcBaseValue;
                //    float bacToSubtract = 0f;
                //    while (calcBaseValue.OverallPoints == calcAtSubtract.OverallPoints && bacToSubtract > -2)
                //    {
                //        bacToSubtract -= 0.01f;
                //        calcAtSubtract = GetCharacterCalculations(character, new Item() { Stats = new Stats() { BonusArmor = bacToSubtract } }) as CharacterCalculationsBear;
                //    }
                //    bacToSubtract += 0.01f;

                //    ComparisonCalculationBear comparisonBAC = new ComparisonCalculationBear()
                //    {
                //        Name = string.Format("Bonus Armor ({0})", bacToAdd - bacToSubtract),
                //        OverallPoints = (calcAtAdd.OverallPoints - calcBaseValue.OverallPoints) / (bacToAdd - bacToSubtract),
                //        MitigationPoints = (calcAtAdd.MitigationPoints - calcBaseValue.MitigationPoints) / (bacToAdd - bacToSubtract),
                //        SurvivalPoints = (calcAtAdd.SurvivalPoints - calcBaseValue.SurvivalPoints) / (bacToAdd - bacToSubtract),
                //        ThreatPoints = (calcAtAdd.ThreatPoints - calcBaseValue.ThreatPoints) / (bacToAdd - bacToSubtract)
                //    };


                //    //Differential Calculations for Sta
                //    calcAtAdd = calcBaseValue;
                //    float staToAdd = 0f;
                //    while (calcBaseValue.OverallPoints == calcAtAdd.OverallPoints && staToAdd < 2)
                //    {
                //        staToAdd += 0.01f;
                //        calcAtAdd = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Stamina = staToAdd } }) as CharacterCalculationsBear;
                //    }

                //    calcAtSubtract = calcBaseValue;
                //    float staToSubtract = 0f;
                //    while (calcBaseValue.OverallPoints == calcAtSubtract.OverallPoints && staToSubtract > -2)
                //    {
                //        staToSubtract -= 0.01f;
                //        calcAtSubtract = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Stamina = staToSubtract } }) as CharacterCalculationsBear;
                //    }
                //    staToSubtract += 0.01f;

                //    ComparisonCalculationBear comparisonSta = new ComparisonCalculationBear() {
                //        Name = string.Format("Stamina ({0})", staToAdd-staToSubtract),
                //        OverallPoints = (calcAtAdd.OverallPoints - calcBaseValue.OverallPoints) / (staToAdd - staToSubtract),
                //        MitigationPoints = (calcAtAdd.MitigationPoints - calcBaseValue.MitigationPoints) / (staToAdd - staToSubtract), 
                //        SurvivalPoints = (calcAtAdd.SurvivalPoints - calcBaseValue.SurvivalPoints) / (staToAdd - staToSubtract), 
                //        ThreatPoints = (calcAtAdd.ThreatPoints - calcBaseValue.ThreatPoints) / (staToAdd - staToSubtract)};

                //    return new ComparisonCalculationBase[] { 
                //        comparisonAgi,
                //        comparisonAC,
                //        comparisonBAC,
                //        comparisonSta,
                //        comparisonDef,
                //        comparisonStr,
                //        new ComparisonCalculationBear() { Name = "Attack Power", 
                //            OverallPoints = (calcAPValue.OverallPoints - calcBaseValue.OverallPoints), 
                //            MitigationPoints = (calcAPValue.MitigationPoints - calcBaseValue.MitigationPoints), 
                //            SurvivalPoints = (calcAPValue.SurvivalPoints - calcBaseValue.SurvivalPoints), 
                //            ThreatPoints = (calcAPValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                //        new ComparisonCalculationBear() { Name = "Hit Rating", 
                //            OverallPoints = (calcHitValue.OverallPoints - calcBaseValue.OverallPoints), 
                //            MitigationPoints = (calcHitValue.MitigationPoints - calcBaseValue.MitigationPoints), 
                //            SurvivalPoints = (calcHitValue.SurvivalPoints - calcBaseValue.SurvivalPoints), 
                //            ThreatPoints = (calcHitValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                //        new ComparisonCalculationBear() { Name = "Crit Rating", 
                //            OverallPoints = (calcCritValue.OverallPoints - calcBaseValue.OverallPoints), 
                //            MitigationPoints = (calcCritValue.MitigationPoints - calcBaseValue.MitigationPoints), 
                //            SurvivalPoints = (calcCritValue.SurvivalPoints - calcBaseValue.SurvivalPoints), 
                //            ThreatPoints = (calcCritValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                //        new ComparisonCalculationBear() { Name = "Weapon Damage", 
                //            OverallPoints = (calcDamageValue.OverallPoints - calcBaseValue.OverallPoints), 
                //            MitigationPoints = (calcDamageValue.MitigationPoints - calcBaseValue.MitigationPoints), 
                //            SurvivalPoints = (calcDamageValue.SurvivalPoints - calcBaseValue.SurvivalPoints), 
                //            ThreatPoints = (calcDamageValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                //        new ComparisonCalculationBear() { Name = "Haste Rating", 
                //            OverallPoints = (calcHasteValue.OverallPoints - calcBaseValue.OverallPoints), 
                //            MitigationPoints = (calcHasteValue.MitigationPoints - calcBaseValue.MitigationPoints), 
                //            SurvivalPoints = (calcHasteValue.SurvivalPoints - calcBaseValue.SurvivalPoints), 
                //            ThreatPoints = (calcHasteValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                //        new ComparisonCalculationBear() { Name = "Expertise Rating", 
                //            OverallPoints = (calcExpertiseValue.OverallPoints - calcBaseValue.OverallPoints), 
                //            MitigationPoints = (calcExpertiseValue.MitigationPoints - calcBaseValue.MitigationPoints), 
                //            SurvivalPoints = (calcExpertiseValue.SurvivalPoints - calcBaseValue.SurvivalPoints), 
                //            ThreatPoints = (calcExpertiseValue.ThreatPoints - calcBaseValue.ThreatPoints)},

                //        new ComparisonCalculationBear() { Name = "Dodge Rating", 
                //            OverallPoints = (calcDodgeValue.OverallPoints - calcBaseValue.OverallPoints), 
                //            MitigationPoints = (calcDodgeValue.MitigationPoints - calcBaseValue.MitigationPoints), 
                //            SurvivalPoints = (calcDodgeValue.SurvivalPoints - calcBaseValue.SurvivalPoints), 
                //            ThreatPoints = (calcDodgeValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                //        new ComparisonCalculationBear() { Name = "Health", 
                //            OverallPoints = (calcHealthValue.OverallPoints - calcBaseValue.OverallPoints), 
                //            MitigationPoints = (calcHealthValue.MitigationPoints - calcBaseValue.MitigationPoints), 
                //            SurvivalPoints = (calcHealthValue.SurvivalPoints - calcBaseValue.SurvivalPoints), 
                //            ThreatPoints = (calcHealthValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                //        new ComparisonCalculationBear() { Name = "Resilience", 
                //            OverallPoints = (calcResilValue.OverallPoints - calcBaseValue.OverallPoints), 
                //            MitigationPoints = (calcResilValue.MitigationPoints - calcBaseValue.MitigationPoints), 
                //            SurvivalPoints = (calcResilValue.SurvivalPoints - calcBaseValue.SurvivalPoints), 
                //            ThreatPoints = (calcResilValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                //    };

                case "Rotation DPS":
                    CharacterCalculationsBear calcsDPS = GetCharacterCalculations(character) as CharacterCalculationsBear;
                    List<ComparisonCalculationBase> comparisonsDPS = new List<ComparisonCalculationBase>();

                    BearRotationCalculator rotationCalculatorDPS = new BearRotationCalculator(calcsDPS.MeleeDamageAverage,
                        calcsDPS.MaulDamageAverage, calcsDPS.MangleDamageAverage, calcsDPS.SwipeDamageAverage,
                        calcsDPS.FaerieFireDamageAverage, calcsDPS.LacerateDamageAverage, calcsDPS.LacerateDotDamageAverage, calcsDPS.MeleeThreatAverage,
                        calcsDPS.MaulThreatAverage, calcsDPS.MangleThreatAverage, calcsDPS.SwipeThreatAverage,
                        calcsDPS.FaerieFireThreatAverage, calcsDPS.LacerateThreatAverage, calcsDPS.LacerateDotThreatAverage,
                        6f - calcsDPS.BasicStats.MangleCooldownReduction, calcsDPS.AttackSpeed);

                    for (int useMaul = 0; useMaul < 3; useMaul++)
                        for (int useMangle = 0; useMangle < 2; useMangle++)
                            for (int useSwipe = 0; useSwipe < 2; useSwipe++)
                                for (int useFaerieFire = 0; useFaerieFire < 2; useFaerieFire++)
                                    for (int useLacerate = 0; useLacerate < 2; useLacerate++)
                                    {
                                        bool?[] useMaulValues = new bool?[] { null, false, true };
                                        BearRotationCalculator.BearRotationCalculation rotationCalculation =
                                            rotationCalculatorDPS.GetRotationCalculations(useMaulValues[useMaul],
                                            useMangle == 1, useSwipe == 1, useFaerieFire == 1, useLacerate == 1);

                                        comparisonsDPS.Add(new ComparisonCalculationBear()
                                        {
                                            CharacterItems = character.GetItems(),
                                            Name = rotationCalculation.Name.Replace("+", " + "),
                                            OverallPoints = rotationCalculation.DPS,
                                            ThreatPoints = rotationCalculation.DPS
                                        });
                                    }

                    return comparisonsDPS.ToArray();

                case "Rotation TPS":
                    CharacterCalculationsBear calcsTPS = GetCharacterCalculations(character) as CharacterCalculationsBear;
                    List<ComparisonCalculationBase> comparisonsTPS = new List<ComparisonCalculationBase>();

                    BearRotationCalculator rotationCalculatorTPS = new BearRotationCalculator(calcsTPS.MeleeDamageAverage,
                        calcsTPS.MaulDamageAverage, calcsTPS.MangleDamageAverage, calcsTPS.SwipeDamageAverage,
                        calcsTPS.FaerieFireDamageAverage, calcsTPS.LacerateDamageAverage, calcsTPS.LacerateDotDamageAverage, calcsTPS.MeleeThreatAverage,
                        calcsTPS.MaulThreatAverage, calcsTPS.MangleThreatAverage, calcsTPS.SwipeThreatAverage,
                        calcsTPS.FaerieFireThreatAverage, calcsTPS.LacerateThreatAverage, calcsTPS.LacerateDotThreatAverage,
                        6f - calcsTPS.BasicStats.MangleCooldownReduction, calcsTPS.AttackSpeed);

                    for (int useMaul = 0; useMaul < 3; useMaul++)
                        for (int useMangle = 0; useMangle < 2; useMangle++)
                            for (int useSwipe = 0; useSwipe < 2; useSwipe++)
                                for (int useFaerieFire = 0; useFaerieFire < 2; useFaerieFire++)
                                    for (int useLacerate = 0; useLacerate < 2; useLacerate++)
                                    {
                                        bool?[] useMaulValues = new bool?[] { null, false, true };
                                        BearRotationCalculator.BearRotationCalculation rotationCalculation =
                                            rotationCalculatorTPS.GetRotationCalculations(useMaulValues[useMaul],
                                            useMangle == 1, useSwipe == 1, useFaerieFire == 1, useLacerate == 1);

                                        comparisonsTPS.Add(new ComparisonCalculationBear()
                                        {
                                            CharacterItems = character.GetItems(),
                                            Name = rotationCalculation.Name.Replace("+", " + "),
                                            OverallPoints = rotationCalculation.TPS,
                                            ThreatPoints = rotationCalculation.TPS
                                        });
                                    }

                    return comparisonsTPS.ToArray();

                default:
                    return new ComparisonCalculationBase[0];
            }
        }
        #endregion

        #region Relevancy Methods
        public override bool IsItemRelevant(Item item)
        {
            if (item.Slot == ItemSlot.OffHand ||
                (item.Slot == ItemSlot.Ranged && item.Type != ItemType.Idol))
                return false;
            return base.IsItemRelevant(item);
        }

        public override Stats GetRelevantStats(Stats stats)
        {
            Stats s = new Stats()
            {
                Armor = stats.Armor,
                BonusArmor = stats.BonusArmor,
                Stamina = stats.Stamina,
                Agility = stats.Agility,
                DodgeRating = stats.DodgeRating,
                DefenseRating = stats.DefenseRating,
                Resilience = stats.Resilience,
                BonusAgilityMultiplier = stats.BonusAgilityMultiplier,
                BaseArmorMultiplier = stats.BaseArmorMultiplier,
                BonusArmorMultiplier = stats.BonusArmorMultiplier,
                BonusStaminaMultiplier = stats.BonusStaminaMultiplier,
                BonusStrengthMultiplier = stats.BonusStrengthMultiplier,
                Health = stats.Health,
                BattlemasterHealth = stats.BattlemasterHealth,
                BonusHealthMultiplier = stats.BonusHealthMultiplier,
                Miss = stats.Miss,
                CritChanceReduction = stats.CritChanceReduction,
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
                HighestStat = stats.HighestStat,
                Paragon = stats.Paragon,
                Dodge = stats.Dodge,

                Strength = stats.Strength,
                AttackPower = stats.AttackPower,
                CritRating = stats.CritRating,
                PhysicalCrit = stats.PhysicalCrit,
                HitRating = stats.HitRating,
                PhysicalHit = stats.PhysicalHit,
                MoteOfAnger = stats.MoteOfAnger,
                HasteRating = stats.HasteRating,
                PhysicalHaste = stats.PhysicalHaste,
                ExpertiseRating = stats.ExpertiseRating,
                ArmorPenetration = stats.ArmorPenetration,
                WeaponDamage = stats.WeaponDamage,
                BonusCritMultiplier = stats.BonusCritMultiplier,
                BonusMangleBearThreat = stats.BonusMangleBearThreat,
                BonusLacerateDamageMultiplier = stats.BonusLacerateDamageMultiplier,
                BonusBearSwipeDamageMultiplier = stats.BonusBearSwipeDamageMultiplier,
                BonusAttackPowerMultiplier = stats.BonusAttackPowerMultiplier,
                BonusDamageMultiplier = stats.BonusDamageMultiplier,
                DamageTakenMultiplier = stats.DamageTakenMultiplier,
                ArmorPenetrationRating = stats.ArmorPenetrationRating,
                BossAttackSpeedMultiplier = stats.BossAttackSpeedMultiplier,
                SpellCrit = stats.SpellCrit,
                SpellCritOnTarget = stats.SpellCritOnTarget,
                Intellect = stats.Intellect,
                BonusNatureDamageMultiplier = stats.BonusNatureDamageMultiplier,
                SpellHit = stats.SpellHit,
                ThreatIncreaseMultiplier = stats.ThreatIncreaseMultiplier
            };
            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if (effect.Trigger == Trigger.Use || effect.Trigger == Trigger.MeleeCrit || effect.Trigger == Trigger.MeleeHit || effect.Trigger == Trigger.MeleeAttack
                || effect.Trigger == Trigger.PhysicalCrit || effect.Trigger == Trigger.PhysicalHit || effect.Trigger == Trigger.DoTTick
                || effect.Trigger == Trigger.DamageDone || effect.Trigger == Trigger.MangleBearHit || effect.Trigger == Trigger.LacerateTick
                || effect.Trigger == Trigger.SwipeBearOrLacerateHit || effect.Trigger == Trigger.DamageTaken || effect.Trigger == Trigger.DamageTakenPhysical
                || effect.Trigger == Trigger.MangleCatOrShredOrInfectedWoundsHit || effect.Trigger == Trigger.DamageOrHealingDone)
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
            bool relevant = (stats.Agility + stats.Armor + stats.BonusArmor + stats.BonusAgilityMultiplier + stats.BonusArmorMultiplier +
                stats.BonusStaminaMultiplier + stats.DefenseRating + stats.DodgeRating + stats.Health + stats.BonusHealthMultiplier +
                stats.BattlemasterHealth + stats.Miss + stats.Resilience + stats.Stamina + stats.MoteOfAnger +
                stats.ArcaneResistance + stats.NatureResistance + stats.FireResistance + stats.Dodge +
                stats.FrostResistance + stats.ShadowResistance + stats.ArcaneResistanceBuff +
                stats.NatureResistanceBuff + stats.FireResistanceBuff + stats.PhysicalCrit +
                stats.FrostResistanceBuff + stats.ShadowResistanceBuff + stats.CritChanceReduction +
                stats.ArmorPenetrationRating + stats.PhysicalHaste + stats.BonusBearSwipeDamageMultiplier
                 + stats.Strength + stats.AttackPower + stats.CritRating + stats.HitRating + stats.HasteRating
                 + stats.ExpertiseRating + stats.ArmorPenetration + stats.WeaponDamage + stats.BonusCritMultiplier
                 + stats.BonusRipDuration + stats.HighestStat + stats.Paragon + stats.PhysicalHit
                 + stats.BonusMangleBearThreat + stats.BonusLacerateDamageMultiplier
                 + stats.BonusAttackPowerMultiplier + stats.BonusDamageMultiplier
                 + stats.DamageTakenMultiplier + stats.ArmorPenetrationRating + stats.BossAttackSpeedMultiplier
                 + stats.SpellCrit + stats.SpellCritOnTarget + stats.Intellect + stats.BonusNatureDamageMultiplier
                 + stats.SpellHit + stats.ThreatIncreaseMultiplier
                 ) != 0;

            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if (effect.Trigger == Trigger.Use || effect.Trigger == Trigger.MeleeCrit || effect.Trigger == Trigger.MeleeHit || effect.Trigger == Trigger.MeleeAttack
                    || effect.Trigger == Trigger.PhysicalCrit || effect.Trigger == Trigger.PhysicalHit
                    || effect.Trigger == Trigger.MangleBearHit || effect.Trigger == Trigger.SwipeBearOrLacerateHit
                    || effect.Trigger == Trigger.DamageTaken || effect.Trigger == Trigger.DamageTakenPhysical
                    || effect.Trigger == Trigger.MangleCatOrShredOrInfectedWoundsHit || effect.Trigger == Trigger.LacerateTick)
                {
                    relevant |= HasRelevantStats(effect.Stats);
                    if (relevant) break;
                }
            }
            return relevant;
        }

        #endregion

        public Stats GetBuffsStats(Character character, CalculationOptionsBear calcOpts) {
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
            character.ActiveBuffsAdd(("Devotion Aura"));
            character.ActiveBuffsAdd(("Inspiration"));
            character.ActiveBuffsAdd(("Battle Shout"));
            character.ActiveBuffsAdd(("Unleashed Rage"));
            character.ActiveBuffsAdd(("Improved Moonkin Form"));
            character.ActiveBuffsAdd(("Commanding Shout"));
            character.ActiveBuffsAdd(("Leader of the Pack"));
            character.ActiveBuffsAdd(("Improved Icy Talons"));
            character.ActiveBuffsAdd(("Power Word: Fortitude"));
            character.ActiveBuffsAdd(("Mark of the Wild"));
            character.ActiveBuffsAdd(("Blessing of Kings"));
            character.ActiveBuffsAdd(("Sunder Armor"));
            character.ActiveBuffsAdd(("Faerie Fire"));
            character.ActiveBuffsAdd(("Totem of Wrath"));
            character.ActiveBuffsAdd(("Flask of Stoneblood"));
            character.ActiveBuffsAdd(("Agility Food"));
            character.ActiveBuffsAdd(("Heroism/Bloodlust"));

            character.DruidTalents.GlyphOfMaul = true;
            character.DruidTalents.GlyphOfGrowl = true;
            character.DruidTalents.GlyphOfFrenziedRegeneration = true;
        }

        private static List<string> _relevantGlyphs;
        public override List<string> GetRelevantGlyphs()
        {
            if (_relevantGlyphs == null)
            {
                _relevantGlyphs = new List<string>();
                _relevantGlyphs.Add("Glyph of Maul");
                _relevantGlyphs.Add("Glyph of Growl");
                _relevantGlyphs.Add("Glyph of Frenzied Regeneration");
                _relevantGlyphs.Add("Glyph of Mangle");
                _relevantGlyphs.Add("Glyph of Berserk");
            }
            return _relevantGlyphs;
        }
    }

    /// <summary>
    /// Data container class for the results of calculations about a Character
    /// </summary>
    public class CharacterCalculationsBear : CharacterCalculationsBase
    {
        private float _overallPoints = 0f;
        public override float OverallPoints
        {
            get { return _overallPoints; }
            set { _overallPoints = value; }
        }

        private float[] _subPoints = new float[] { 0f, 0f, 0f };
        public override float[] SubPoints
        {
            get { return _subPoints; }
            set { _subPoints = value; }
        }

        public float MitigationPoints
        {
            get { return _subPoints[0]; }
            set { _subPoints[0] = value; }
        }

        public float SurvivalPoints
        {
            get { return _subPoints[1]; }
            set { _subPoints[1] = value; }
        }

        public float ThreatPoints
        {
            get { return _subPoints[2]; }
            set { _subPoints[2] = value; }
        }

        public StatsBear BasicStats { get; set; }
        public int TargetLevel { get; set; }
        public float Dodge { get; set; }
        public float Miss { get; set; }
        public float ConstantDamageReduction { get; set; }
        public float AvoidancePreDR { get; set; }
        public float AvoidancePostDR { get; set; }
        public float TotalMitigation { get; set; }
        public float SavageDefenseChance { get; set; }
        public float SavageDefenseValue { get; set; }
        public float SavageDefensePercent { get; set; }
        public float DamageTaken { get; set; }
        public float CritReduction { get; set; }
        public float CappedCritReduction { get; set; }
        public float SurvivalPointsRaw { get; set; }
        
        public float NatureSurvivalPoints { get; set; }
        public float FrostSurvivalPoints { get; set; }
        public float FireSurvivalPoints { get; set; }
        public float ShadowSurvivalPoints { get; set; }
        public float ArcaneSurvivalPoints { get; set; }

        public float AttackSpeed { get; set; }
        public float MangleCooldown { get; set; }
        
        public float MissedAttacks { get; set; }
        public float DodgedAttacks { get; set; }
        public float ParriedAttacks { get; set; }
        public float AvoidedAttacks { get { return MissedAttacks + DodgedAttacks + ParriedAttacks; } }

        public float MeleeThreatRaw { get; set; }
        public float MaulThreatRaw { get; set; }
        public float MangleThreatRaw { get; set; }
        public float SwipeThreatRaw { get; set; }
        public float FaerieFireThreatRaw { get; set; }
        public float LacerateThreatRaw { get; set; }
        public float LacerateDotThreatRaw { get; set; }

        public float MeleeThreatAverage { get; set; }
        public float MaulThreatAverage { get; set; }
        public float MangleThreatAverage { get; set; }
        public float SwipeThreatAverage { get; set; }
        public float FaerieFireThreatAverage { get; set; }
        public float LacerateThreatAverage { get; set; }
        public float LacerateDotThreatAverage { get; set; }

        public float MeleeDamageRaw { get; set; }
        public float MaulDamageRaw { get; set; }
        public float MangleDamageRaw { get; set; }
        public float SwipeDamageRaw { get; set; }
        public float FaerieFireDamageRaw { get; set; }
        public float LacerateDamageRaw { get; set; }
        public float LacerateDotDamageRaw { get; set; }

        public float MeleeDamageAverage { get; set; }
        public float MaulDamageAverage { get; set; }
        public float MangleDamageAverage { get; set; }
        public float SwipeDamageAverage { get; set; }
        public float FaerieFireDamageAverage { get; set; }
        public float LacerateDamageAverage { get; set; }
        public float LacerateDotDamageAverage { get; set; }

        public float MaulTPR { get; set; }
        public float MaulDPR { get; set; }
        public float MangleTPR { get; set; }
        public float MangleDPR { get; set; }
        public float SwipeTPR { get; set; }
        public float SwipeDPR { get; set; }
        public float LacerateTPR { get; set; }
        public float LacerateDPR { get; set; }

        public BearRotationCalculator.BearRotationCalculation HighestDPSRotation { get; set; }
        public BearRotationCalculator.BearRotationCalculation HighestTPSRotation { get; set; }
        public BearRotationCalculator.BearRotationCalculation SwipeRotation { get; set; }
        public BearRotationCalculator.BearRotationCalculation CustomRotation { get; set; }

        public override Dictionary<string, string> GetCharacterDisplayCalculationValues()
        {
            Dictionary<string, string> dictValues = new Dictionary<string, string>();

            float baseMiss  = StatConversion.WHITE_MISS_CHANCE_CAP[ TargetLevel - 80] - BasicStats.PhysicalHit;
            float baseDodge = StatConversion.WHITE_DODGE_CHANCE_CAP[TargetLevel - 80] - StatConversion.GetDodgeParryReducFromExpertise(BasicStats.Expertise);
            float baseParry = StatConversion.WHITE_PARRY_CHANCE_CAP[TargetLevel - 80] - StatConversion.GetDodgeParryReducFromExpertise(BasicStats.Expertise);
            float capMiss   = (float)Math.Ceiling(baseMiss  * StatConversion.RATING_PER_PHYSICALHIT);
            float capDodge  = (float)Math.Ceiling(baseDodge * 400f * StatConversion.RATING_PER_EXPERTISE);
            float capParry  = (float)Math.Ceiling(baseParry * 400f * StatConversion.RATING_PER_EXPERTISE);

            string tipMiss = string.Empty;
            if (BasicStats.HitRating > capMiss)
                tipMiss = string.Format("*Over the cap by {0} Hit Rating", BasicStats.HitRating - capMiss);
            else if (BasicStats.HitRating < capMiss)
                tipMiss = string.Format("*Under the cap by {0} Hit Rating", capMiss - BasicStats.HitRating);
            else
                tipMiss = "*Exactly at the cap";

            string tipDodgeParry = string.Empty;
            if (BasicStats.ExpertiseRating > capDodge)
                tipDodgeParry = string.Format("*Over the dodge cap by {0} Expertise Rating\r\n", BasicStats.ExpertiseRating - capDodge);
            else if (BasicStats.ExpertiseRating < capDodge)
                tipDodgeParry = string.Format("*Under the dodge cap by {0} Expertise Rating\r\n", capDodge - BasicStats.ExpertiseRating);
            else
                tipDodgeParry = "*Exactly at the dodge cap";

            if (BasicStats.ExpertiseRating > capParry)
                tipDodgeParry += string.Format("Over the parry cap by {0} Expertise Rating", BasicStats.ExpertiseRating - capParry);
            else if (BasicStats.ExpertiseRating < capParry)
                tipDodgeParry += string.Format("Under the parry cap by {0} Expertise Rating", capParry - BasicStats.ExpertiseRating);
            else
                tipDodgeParry += "Exactly at the parry cap";




            int armorCap = (int)Math.Ceiling((1402.5f * TargetLevel) - 66502.5f);
            float levelDifference = 0.002f * (TargetLevel - 80);
            float targetCritReduction = StatConversion.NPC_LEVEL_CRIT_MOD[TargetLevel-80];
            int defToCap = 0, resToCap = 0;
            if (CritReduction < targetCritReduction)
            {
                //while (((float)Math.Floor((BasicStats.DefenseRating + defToCap) / (123f / 52f)) * 0.04f)
                //+ BasicStats.Resilience / (2050f / 52f) + BasicStats.CritChanceReduction < targetCritReduction)
                //    defToCap++;
                //while (((float)Math.Floor(BasicStats.DefenseRating / (123f / 52f)) * 0.04f)
                //+ (BasicStats.Resilience + resToCap) / (2050f / 52f) + BasicStats.CritChanceReduction < targetCritReduction)
                //    resToCap++;
                while (((float)Math.Floor(StatConversion.GetDefenseFromRating(BasicStats.DefenseRating + defToCap)) * 0.0004f)
                + StatConversion.GetCritReductionFromResilience(BasicStats.Resilience) + BasicStats.CritChanceReduction < targetCritReduction)
                    defToCap++;
                while (((float)Math.Floor(StatConversion.GetDefenseFromRating(BasicStats.DefenseRating)) * 0.0004f)
                + StatConversion.GetCritReductionFromResilience(BasicStats.Resilience + resToCap) + BasicStats.CritChanceReduction < targetCritReduction)
                    resToCap++;
            }
            else if (CritReduction > targetCritReduction)
            {
                //while (((float)Math.Floor((BasicStats.DefenseRating + defToCap) / (123f / 52f)) * 0.04f)
                //+ BasicStats.Resilience / (2050f / 52f) + BasicStats.CritChanceReduction > targetCritReduction)
                //    defToCap--;
                //while (((float)Math.Floor(BasicStats.DefenseRating / (123f / 52f)) * 0.04f)
                //+ (BasicStats.Resilience + resToCap) / (2050f / 52f) + BasicStats.CritChanceReduction > targetCritReduction)
                //    resToCap--;
                
                while (((float)Math.Floor(StatConversion.GetDefenseFromRating(BasicStats.DefenseRating + defToCap)) * 0.0004f)
                + StatConversion.GetCritReductionFromResilience(BasicStats.Resilience) + BasicStats.CritChanceReduction > targetCritReduction)
                    defToCap--;
                while (((float)Math.Floor(StatConversion.GetDefenseFromRating(BasicStats.DefenseRating)) * 0.0004f)
                + StatConversion.GetCritReductionFromResilience(BasicStats.Resilience + resToCap) + BasicStats.CritChanceReduction > targetCritReduction)
                    resToCap--;
                defToCap++;
                resToCap++;
            }

            // Changed to not just give a resist rating, but a breakdown of the resulting resist values in the tooltip
            string tipResist = string.Empty;
            tipResist = StatConversion.GetResistanceTableString(TargetLevel, 80, BasicStats.NatureResistance, 0);
            dictValues.Add("Nature Resist", BasicStats.NatureResistance.ToString() + "*" + tipResist);
            tipResist = StatConversion.GetResistanceTableString(TargetLevel, 80, BasicStats.ArcaneResistance, 0);
            dictValues.Add("Arcane Resist", BasicStats.ArcaneResistance.ToString() + "*" + tipResist);
            tipResist = StatConversion.GetResistanceTableString(TargetLevel, 80, BasicStats.FrostResistance, 0);
            dictValues.Add("Frost Resist", BasicStats.FrostResistance.ToString() + "*" + tipResist);
            tipResist = StatConversion.GetResistanceTableString(TargetLevel, 80, BasicStats.FireResistance, 0);
            dictValues.Add("Fire Resist", BasicStats.FireResistance.ToString() + "*" + tipResist);
            tipResist = StatConversion.GetResistanceTableString(TargetLevel, 80, BasicStats.ShadowResistance, 0);
            dictValues.Add("Shadow Resist", BasicStats.ShadowResistance.ToString() + "*" + tipResist);

            dictValues.Add("Health", BasicStats.Health.ToString());
            dictValues.Add("Agility", BasicStats.Agility.ToString());
            dictValues.Add("Armor", BasicStats.Armor.ToString());
            dictValues.Add("Stamina", BasicStats.Stamina.ToString());
            dictValues.Add("Dodge Rating", BasicStats.DodgeRating.ToString());
            dictValues.Add("Defense Rating", BasicStats.DefenseRating.ToString());
            dictValues.Add("Resilience", BasicStats.Resilience.ToString());
            dictValues.Add("Dodge", Dodge.ToString("0.000%"));
            dictValues.Add("Miss", Miss.ToString("0.000%"));
            if (BasicStats.Armor == armorCap)
                dictValues.Add("Mitigation", ConstantDamageReduction.ToString("0.000%")
                    + string.Format("*Exactly at the armor cap against level {0} mobs.", TargetLevel));
            else if (BasicStats.Armor > armorCap)
                dictValues.Add("Mitigation", ConstantDamageReduction.ToString("0.000%")
                    + string.Format("*Over the armor cap by {0} armor.", BasicStats.Armor - armorCap));
            else
                dictValues.Add("Mitigation", ConstantDamageReduction.ToString("0.000%")
                    + string.Format("*Short of the armor cap by {0} armor.", armorCap - BasicStats.Armor));
            dictValues.Add("Avoidance PreDR", AvoidancePreDR.ToString("0.000%"));
            dictValues.Add("Avoidance PostDR", AvoidancePostDR.ToString("0.000%"));
            dictValues.Add("Total Mitigation", TotalMitigation.ToString("0.000%"));
            dictValues.Add("Damage Taken", DamageTaken.ToString("0.000%"));
            dictValues.Add("Savage Defense", string.Format(
                "{0}  ~ {1}*{0} chance to absorb incoming hit\r\n{1} absorbed per hit\r\n{2} of incoming damage absorbed", 
                SavageDefenseChance.ToString("0.000%"), SavageDefenseValue, SavageDefensePercent.ToString("0.000%")));
            dictValues.Add("Chance to be Crit", ((0.05f + levelDifference) - CritReduction).ToString("0.000%"));
            dictValues.Add("Overall Points", OverallPoints.ToString());
            dictValues.Add("Mitigation Points", MitigationPoints.ToString());
            dictValues.Add("Survival Points", string.Format("{0}*{1} Before Soft Cap", SurvivalPoints.ToString(), SurvivalPointsRaw.ToString()));
            dictValues.Add("Threat Points", ThreatPoints.ToString());

            dictValues["Nature Survival"] = NatureSurvivalPoints.ToString();
            dictValues["Frost Survival"] = FrostSurvivalPoints.ToString();
            dictValues["Fire Survival"] = FireSurvivalPoints.ToString();
            dictValues["Shadow Survival"] = ShadowSurvivalPoints.ToString();
            dictValues["Arcane Survival"] = ArcaneSurvivalPoints.ToString();


            dictValues["Strength"] = BasicStats.Strength.ToString();
            dictValues["Attack Power"] = BasicStats.AttackPower.ToString();
            dictValues["Crit Rating"] = BasicStats.CritRating.ToString();
            dictValues["Hit Rating"] = BasicStats.HitRating.ToString() + tipMiss;
            dictValues["Expertise Rating"] = BasicStats.ExpertiseRating.ToString() + tipDodgeParry;
            dictValues["Haste Rating"] = string.Format("{0}*{1}sec Attack Speed", BasicStats.HasteRating, AttackSpeed.ToString("0.000"));
            dictValues["Armor Penetration Rating"] = BasicStats.ArmorPenetrationRating.ToString();

            dictValues["Avoided Attacks"] = String.Format("{0}*{1} Missed\r\n{2} Dodged\r\n{3} Parried", 
                AvoidedAttacks.ToString("0.000%"), MissedAttacks.ToString("0.000%"), 
                DodgedAttacks.ToString("0.000%"), ParriedAttacks.ToString("0.000%"));
           
            string rotationFormat = "{0} DPS, {1} TPS*{2}";


            dictValues["Highest DPS Rotation"] = String.Format(rotationFormat, Math.Round(HighestDPSRotation.DPS), Math.Round(HighestDPSRotation.TPS), GetRotationTooltip(HighestDPSRotation.Name));
            dictValues["Highest TPS Rotation"] = String.Format(rotationFormat, Math.Round(HighestTPSRotation.DPS), Math.Round(HighestTPSRotation.TPS), GetRotationTooltip(HighestTPSRotation.Name));
            dictValues["Swipe Rotation"] = String.Format(rotationFormat, Math.Round(SwipeRotation.DPS), Math.Round(SwipeRotation.TPS), GetRotationTooltip(SwipeRotation.Name));
            dictValues["Custom Rotation"] = String.Format(rotationFormat, Math.Round(CustomRotation.DPS), Math.Round(CustomRotation.TPS), GetRotationTooltip(CustomRotation.Name));

            string attackFormat = "{0} Dmg, {1} Threat*Per Hit: {0} Damage, {1} Threat\r\nPer Average Swing: {2} Damage, {3} Threat";
            string attackFormatWithRage = attackFormat + "\r\nThreat Per Rage: {4}\r\nDamage Per Rage: {5}";
            dictValues["Melee"] = String.Format(attackFormat, MeleeDamageRaw, MeleeThreatRaw, MeleeDamageAverage, MeleeThreatAverage);
            dictValues["Maul"] = String.Format(attackFormatWithRage, MaulDamageRaw, MaulThreatRaw, MaulDamageAverage, MaulThreatAverage, MaulTPR, MaulDPR);
            dictValues["Mangle"] = String.Format(attackFormatWithRage, MangleDamageRaw, MangleThreatRaw, MangleDamageAverage, MangleThreatAverage, MangleTPR, MangleDPR);
            dictValues["Swipe"] = String.Format(attackFormatWithRage, SwipeDamageRaw, SwipeThreatRaw, SwipeDamageAverage, SwipeThreatAverage, SwipeTPR, SwipeDPR);
            dictValues["Faerie Fire"] = String.Format(attackFormat, FaerieFireDamageRaw, FaerieFireThreatRaw, FaerieFireDamageAverage, FaerieFireThreatAverage);
            dictValues["Lacerate"] = String.Format(attackFormatWithRage, LacerateDamageRaw, LacerateThreatRaw, LacerateDamageAverage, LacerateThreatAverage, LacerateTPR, LacerateDPR);
            dictValues["Lacerate DoT Tick"] = String.Format(attackFormat, LacerateDotDamageRaw, LacerateDotThreatRaw, LacerateDotDamageAverage, LacerateDotThreatAverage).Replace("Swing", "Tick");
            
            return dictValues;
        }

        private string GetRotationTooltip(string name)
        {
            StringBuilder tooltip = new StringBuilder("Ability Priority: \r\n");
            if (name.Contains("Maul")) tooltip.Append("Maul>");
            if (name.Contains("Mangle")) tooltip.Append("Mangle>");
            if (name.Contains("Faerie")) tooltip.Append("Faerie Fire>");
            if (name.Contains("Swipe") && name.Contains("Lacerate")) tooltip.Append("Lacerate(+)>Swipe\r\n(+) means to only stack and refresh the debuff.");
            else if (name.Contains("Swipe")) tooltip.Append("Swipe");
            else if (name.Contains("Lacerate")) tooltip.Append("Lacerate");
            
            return tooltip.ToString();
        }

        public override float GetOptimizableCalculationValue(string calculation)
        {
            switch (calculation)
            {
                case "Health": return BasicStats.Health;
                case "Hit Rating": return BasicStats.HitRating;
                case "Expertise Rating": return BasicStats.ExpertiseRating;
                case "Haste Rating": return BasicStats.HasteRating;
                case "Avoided Attacks %": return AvoidedAttacks * 100f;
                case "Mitigation % from Armor": return ConstantDamageReduction * 100f;
                case "Avoidance %": return AvoidancePostDR * 100f;
                case "% Chance to be Crit": return ((5f + (0.2f * (TargetLevel - 80))) - CritReduction * 100f);
                case "Nature Survival": return NatureSurvivalPoints;
                case "Fire Survival": return FireSurvivalPoints;
                case "Frost Survival": return FrostSurvivalPoints;
                case "Shadow Survival": return ShadowSurvivalPoints;
                case "Arcane Survival": return ArcaneSurvivalPoints;
                case "Nature Resist": return BasicStats.NatureResistance;
                case "Fire Resist": return BasicStats.FireResistance;
                case "Frost Resist": return BasicStats.FrostResistance;
                case "Shadow Resist": return BasicStats.ShadowResistance;
                case "Arcane Resist": return BasicStats.ArcaneResistance;
                case "Highest DPS": return HighestDPSRotation.DPS;
                case "Highest TPS": return HighestTPSRotation.TPS;
                case "Swipe DPS": return SwipeRotation.DPS;
                case "Swipe TPS": return SwipeRotation.TPS;
                case "Custom Rotation DPS": return CustomRotation.DPS;
                case "Custom Rotation TPS": return CustomRotation.TPS;
            }
            return 0f;
        }
    }

    /// <summary>
    /// Data container class for the results of a comparison of two CharacterCalculationsBear objects
    /// </summary>
    public class ComparisonCalculationBear : ComparisonCalculationBase
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

        private float[] _subPoints = new float[] { 0f, 0f, 0f };
        public override float[] SubPoints
        {
            get { return _subPoints; }
            set { _subPoints = value; }
        }

        public float MitigationPoints
        {
            get { return _subPoints[0]; }
            set { _subPoints[0] = value; }
        }

        public float SurvivalPoints
        {
            get { return _subPoints[1]; }
            set { _subPoints[1] = value; }
        }
        public float ThreatPoints
        {
            get { return _subPoints[2]; }
            set { _subPoints[2] = value; }
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
            return string.Format("{0}: ({1}O {2}M {3}S {4}T)", Name, Math.Round(OverallPoints), Math.Round(MitigationPoints), Math.Round(SurvivalPoints), Math.Round(ThreatPoints));
        }
    }
}
