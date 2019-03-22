using System;
using System.Collections.Generic;
using System.Text;
#if RAWR3
using System.Windows.Media;
#else
using System.Drawing;
#endif
using System.Xml.Serialization;

namespace Rawr.DPSDK
{
    [Rawr.Calculations.RawrModelInfo("DPSDK", "spell_deathknight_classicon", CharacterClass.DeathKnight)]
    public class CalculationsDPSDK : CalculationsBase
    {
        private Rotation.Type _rTypeOld = Rotation.Type.Unknown;

        public static double hawut = new Random().NextDouble() * DateTime.Now.ToOADate();
        public override List<GemmingTemplate> DefaultGemmingTemplates
        {
            get
            {
                ////Relevant Gem IDs for DPSDKs

                //Red
                int[] bold = { 39900, 39996, 40111, 42142 };
                int[] fractured = { 39909, 40002, 40117, 42153 };

                //Purple
                int[] sovereign = { 39934, 40022, 40129 };

                //Orange
                int[] inscribed = { 39947, 40037, 40142 };

                // Prismatic 
                int[] tear = { 42702, 42702, 49110 };

                //Meta
                int chaotic = 41285;
                int relentless = 41398;

                return new List<GemmingTemplate>()
				{
					new GemmingTemplate() { Model = "DPSDK", Group = "Rare", //Max Strength
						RedId = bold[1], YellowId = bold[1], BlueId = bold[1], PrismaticId = bold[1], MetaId = chaotic },
					new GemmingTemplate() { Model = "DPSDK", Group = "Rare", //Max Arp
						RedId = fractured[1], YellowId = fractured[1], BlueId = fractured[1], PrismaticId = fractured[1], MetaId = chaotic },
					new GemmingTemplate() { Model = "DPSDK", Group = "Rare", //Strength
						RedId = bold[1], YellowId = inscribed[1], BlueId = sovereign[1], PrismaticId = bold[1], MetaId = chaotic },
					new GemmingTemplate() { Model = "DPSDK", Group = "Rare", //Relentless
						RedId = bold[1], YellowId = bold[1], BlueId = bold[1], PrismaticId = tear[1], MetaId = relentless },
						
					new GemmingTemplate() { Model = "DPSDK", Group = "Epic", Enabled = true, //Max Strength
						RedId = bold[2], YellowId = bold[2], BlueId = bold[2], PrismaticId = bold[2], MetaId = chaotic },
					new GemmingTemplate() { Model = "DPSDK", Group = "Rare", //Max Arp
						RedId = fractured[2], YellowId = fractured[2], BlueId = fractured[2], PrismaticId = fractured[2], MetaId = chaotic },
					new GemmingTemplate() { Model = "DPSDK", Group = "Epic", Enabled = true, //Strength
						RedId = bold[2], YellowId = inscribed[2], BlueId = sovereign[2], PrismaticId = bold[2], MetaId = chaotic },
					new GemmingTemplate() { Model = "DPSDK", Group = "Epic", Enabled = true, //Relentless
						RedId = bold[2], YellowId = inscribed[2], BlueId = sovereign[2], PrismaticId = tear[2], MetaId = relentless },

					new GemmingTemplate() { Model = "DPSDK", Group = "Jeweler", //Max Strength
						RedId = bold[3], YellowId = bold[3], BlueId = bold[3], PrismaticId = bold[3], MetaId = chaotic },
					new GemmingTemplate() { Model = "DPSDK", Group = "Rare", //Max Arp
						RedId = fractured[3], YellowId = fractured[3], BlueId = fractured[3], PrismaticId = fractured[3], MetaId = chaotic },
					new GemmingTemplate() { Model = "DPSDK", Group = "Jeweler", //Strength
						RedId = bold[2], YellowId = bold[3], BlueId = bold[3], PrismaticId = bold[2], MetaId = chaotic },
				};
            }
        }
        public float BonusMaxRunicPower = 0f;

        public static float AddStatMultiplierStat(float statMultiplier, float newValue)
        {
            float updatedStatModifier = ((1 + statMultiplier) * (1 + newValue)) - 1f;
            return updatedStatModifier;
        }

        
        private Dictionary<string, Color> _subPointNameColors = null;
        /// <summary>
        /// Dictionary<string, Color> that includes the names of each rating which your model will use,
        /// and a color for each. These colors will be used in the charts.
        /// 
        /// EXAMPLE: 
        /// subPointNameColors = new Dictionary<string, System.Drawing.Color>();
        /// subPointNameColors.Add("Mitigation", System.Drawing.Colors.Red);
        /// subPointNameColors.Add("Survival", System.Drawing.Colors.Blue);
        /// </summary>
        public override Dictionary<string, Color> SubPointNameColors
        {
            get
            {
                if (_subPointNameColors == null)
                {
#if RAWR3
                    _subPointNameColors = new Dictionary<string, System.Windows.Media.Color>();
                    _subPointNameColors.Add("DPS", System.Windows.Media.Color.FromArgb(255,0,0,255));
#else
                    _subPointNameColors = new Dictionary<string, System.Drawing.Color>();
                    _subPointNameColors.Add("DPS", System.Drawing.Color.FromArgb(255, 0, 0, 255));
#endif
                }
                return _subPointNameColors;
            }
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
        /// </summary>
        public override string[] CharacterDisplayCalculationLabels {
            get {
                if (_characterDisplayCalculationLabels == null) {
                    List<string> labels = new List<string>(new string[] {
                        "Basic Stats:Health",
					    "Basic Stats:Strength",
					    "Basic Stats:Agility",
					    "Basic Stats:Attack Power",
					    "Basic Stats:Crit Rating",
					    "Basic Stats:Hit Rating",
					    "Basic Stats:Expertise",
					    "Basic Stats:Haste Rating",
					    "Basic Stats:Armor Penetration Rating",
                        "Basic Stats:Armor",
					    "Advanced Stats:Weapon Damage*Damage before misses and mitigation",
					    "Advanced Stats:Attack Speed",
					    "Advanced Stats:Crit Chance",
					    "Advanced Stats:Avoided Attacks",
					    "Advanced Stats:Enemy Mitigation",
                        "DPS Breakdown:White",
                        "DPS Breakdown:BCB*Blood Caked Blade",
                        "DPS Breakdown:Necrosis",
                        "DPS Breakdown:Death Coil",
                        "DPS Breakdown:Icy Touch",
                        "DPS Breakdown:Plague Strike",
                        "DPS Breakdown:Frost Fever",
                        "DPS Breakdown:Blood Plague",
                        "DPS Breakdown:Scourge Strike",
                        "DPS Breakdown:Unholy Blight",
                        "DPS Breakdown:Frost Strike",
                        "DPS Breakdown:Howling Blast",
                        "DPS Breakdown:Obliterate",
                        "DPS Breakdown:Death Strike",
                        "DPS Breakdown:Blood Strike",
                        "DPS Breakdown:Heart Strike",
                        "DPS Breakdown:DRW*Dancing Rune Weapon",
                        "DPS Breakdown:Gargoyle",
                        "DPS Breakdown:Wandering Plague",
                        "DPS Breakdown:Ghoul",
                        "DPS Breakdown:Bloodworms",
                        "DPS Breakdown:Other",
                        "DPS Breakdown:Total DPS",
                    });
                    _characterDisplayCalculationLabels = labels.ToArray();
                }
                return _characterDisplayCalculationLabels;
            }
        }

        private string[] _customChartNames = null;
        public override string[] CustomChartNames {
            get {
                if (_customChartNames == null) 
                { 
                    _customChartNames = new string[] 
                        { 
                            "Item Budget (10 point steps)", 
                            "Item Budget (25 point steps)", 
                            "Item Budget (50 point steps)", 
                            "Item Budget (100 point steps)",
                            "Presences"
                        }; }
                return _customChartNames;
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
            get { return _calculationOptionsPanel ?? (_calculationOptionsPanel = new CalculationOptionsPanelDPSDK()); }
        }

        private List<ItemType> _relevantItemTypes = null;
        public override List<ItemType> RelevantItemTypes {
            get {
                return _relevantItemTypes ?? (_relevantItemTypes = new List<ItemType>(new ItemType[] {
						ItemType.None,
                        ItemType.Leather,
                        ItemType.Mail,
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

        public override CharacterClass TargetClass { get { return CharacterClass.DeathKnight; } }
        public override ComparisonCalculationBase CreateNewComparisonCalculation() { return new ComparisonCalculationDPSDK(); }

        public override CharacterCalculationsBase CreateNewCharacterCalculations() { return new CharacterCalculationsDPSDK(); }

        public override ICalculationOptionBase DeserializeDataObject(string xml) {
            XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsDPSDK));
            System.IO.StringReader reader = new System.IO.StringReader(xml);
            CalculationOptionsDPSDK calcOpts = serializer.Deserialize(reader) as CalculationOptionsDPSDK;
            return calcOpts;
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
        public override CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, bool referenceCalculation, bool significantChange, bool needsDisplayCalculations) {
            CalculationOptionsDPSDK calcOpts = character.CalculationOptions as CalculationOptionsDPSDK;

//            if (calcOpts.m_bExperimental)
//                return GetCharacterCalculationsExp(character, additionalItem, referenceCalculation, significantChange, needsDisplayCalculations);

            int targetLevel = calcOpts.TargetLevel;

            Stats stats = new Stats();

            CharacterCalculationsDPSDK calcs = new CharacterCalculationsDPSDK();
            stats = GetCharacterStats(character, additionalItem);
            calcs.BasicStats = stats;
            calcs.ActiveBuffs = new List<Buff>(character.ActiveBuffs);
            calcs.Talents = character.DeathKnightTalents;

            CombatTable combatTable = new CombatTable(character, calcs, stats, calcOpts/*, additionalItem*/);
            
            //DPS Subgroups
            float dpsWhite = 0f;
            float dpsBCB = 0f;
            float dpsDancingRuneWeapon = 0f;
            float dpsGargoyle = 0f;
            float dpsGhoul = 0f;
            float dpsOtherShadow = 0f;
            float dpsOtherArcane = 0f;
            float dpsOtherFrost = 0f;
            float dpsOtherFire = 0f;
            float dpsBloodworms = 0f;

            //shared variables
            DeathKnightTalents talents = character.DeathKnightTalents;
            bool DW = combatTable.DW;
            float missedSpecial = 0f;

            float dpsWhiteBeforeArmor = 0f;
            float dpsWhiteMinusGlancing = 0f;
            float fightDuration = calcOpts.FightLength * 60;
            float mitigation;
            //float KMProcsPerRotation = 0f;
            float CinderglacierMultiplier = 1f;

            float MHExpertise = stats.Expertise;
            float OHExpertise = stats.Expertise;

            //damage multipliers
            float spellPowerMult = stats.BonusSpellPowerMultiplier;
            float frostSpellPowerMult = stats.BonusSpellPowerMultiplier + Math.Max((stats.BonusFrostDamageMultiplier - stats.BonusShadowDamageMultiplier), 0f);

            float physPowerMult = stats.BonusPhysicalDamageMultiplier;
            // Covers all % physical damage increases.  Blood Frenzy, FI.
            float partialResist = 0.94f; // Average of 6% damage lost to partial resists on spells
            physPowerMult *= 1f + stats.BonusDamageMultiplier;
            spellPowerMult *= 1f + stats.BonusDamageMultiplier;
            frostSpellPowerMult *= 1f + stats.BonusDamageMultiplier;

            // The Bonus Damage Multiplier needs to get applied to the stats object that gets passed into the Ability Handler.
            stats.BonusPhysicalDamageMultiplier = physPowerMult;
            stats.BonusSpellPowerMultiplier = spellPowerMult;
            stats.BonusFrostDamageMultiplier = frostSpellPowerMult;

            //spell AP multipliers, for diseases its per tick
            float HowlingBlastAPMult = 0.2f;
            float IcyTouchAPMult = 0.1f;
            float FrostFeverAPMult = 0.055f;
            float BloodPlagueAPMult = 0.055f;
            float DeathCoilAPMult = 0.15f;
            float UnholyBlightAPMult = 0.013f;
            float GargoyleAPMult = 0.4f;   // pre 3.0.8 == 0.42f...now probably ~0.3f
                                            // TESTED on june 27th 2009 and found to be 0.40f
            float BloodwormsAPMult = 0.006f;

            //for estimating rotation pushback

            calcOpts.rotation.AvgDiseaseMult = calcOpts.rotation.NumDisease * (calcOpts.rotation.DiseaseUptime / 100);
            float commandMult = 0f;
            Rotation r = new Rotation();
            bool bChangeRotation = false;
            Rotation.Type rType = r.GetRotationType(talents);
            if (rType != _rTypeOld)
                bChangeRotation = true;
            r.setRotation(rType);

            {
                float OHMult = 0.5f * (1f + (float)talents.NervesOfColdSteel * 0.083333333f);	//an OH multiplier that is useful sometimes
                //Boolean PTR = false; // enable and disable PTR things here

                #region Impurity Application
                {
                    float impurityMult = 1f + (.04f * (float)talents.Impurity);

                    HowlingBlastAPMult *= impurityMult;
                    IcyTouchAPMult *= impurityMult;
                    FrostFeverAPMult *= impurityMult;
                    BloodPlagueAPMult *= impurityMult;
                    DeathCoilAPMult *= impurityMult;
                    UnholyBlightAPMult *= impurityMult;
                    GargoyleAPMult *= impurityMult;
                }
                #endregion

                #region racials
                {
                    if (character.Race == CharacterRace.Orc) { commandMult += .05f; }
                }
                #endregion

               /* #region Killing Machine
                {
                    float KMPpM = (1f * talents.KillingMachine) * (1f + (StatConversion.GetHasteFromRating(stats.HasteRating, CharacterClass.DeathKnight))) * (1f + stats.PhysicalHaste); // KM Procs per Minute (Defined "1 per point" by Blizzard) influenced by Phys. Haste
                    KMPpM *= calcOpts.KMProcUsage;
                    KMPpM *= 1f - combatTable.totalMHMiss;
                    KMPpM += talents.Deathchill / 2f;

                    double KMPpR = KMPpM / (60f / temp.CurRotationDuration);
                    float totalAbilities = (float)(temp.FrostStrike + temp.IcyTouch + temp.HowlingBlast);
                    KMProcsPerRotation = (float)KMPpR;
                }
                #endregion*/

                

                #region Mitigation
                {
                    float targetArmor = calcOpts.BossArmor;

                    float arpenBuffs = talents.BloodGorged * 2f / 100;

                    mitigation = 1f - StatConversion.GetArmorDamageReduction(character.Level, targetArmor,
                                        stats.ArmorPenetration, arpenBuffs, Math.Max(0, stats.ArmorPenetrationRating));

                    calcs.EnemyMitigation = 1f - mitigation;
                    calcs.EffectiveArmor = mitigation;
                }
                #endregion

                #region White Dmg
                {
                    float MHDPS = 0f, OHDPS = 0f;
                    #region Main Hand
                    {
                        float dpsMHglancing = (StatConversion.WHITE_GLANCE_CHANCE_CAP[targetLevel - 80] * combatTable.MH.DPS) * 0.7f;
                        float dpsMHBeforeArmor = ((combatTable.MH.DPS * (1f - calcs.AvoidedAttacks - StatConversion.WHITE_GLANCE_CHANCE_CAP[targetLevel - 80])) * (1f + combatTable.physCrits)) + dpsMHglancing;
                        dpsWhiteMinusGlancing = dpsMHBeforeArmor - dpsMHglancing;
                        dpsWhiteBeforeArmor = dpsMHBeforeArmor;
                        MHDPS = dpsMHBeforeArmor * mitigation;
                    }
                    #endregion

                    #region Off Hand
                    if (DW || (character.MainHand == null && character.OffHand != null))
                    {
                        float dpsOHglancing = (StatConversion.WHITE_GLANCE_CHANCE_CAP[targetLevel - 80] * combatTable.OH.DPS) * 0.7f;
                        float dpsOHBeforeArmor = ((combatTable.OH.DPS * (1f - calcs.AvoidedAttacks - StatConversion.WHITE_GLANCE_CHANCE_CAP[targetLevel - 80])) * (1f + combatTable.physCrits)) + dpsOHglancing;
                        dpsOHBeforeArmor *= OHMult;
                        dpsWhiteMinusGlancing += dpsOHBeforeArmor - dpsOHglancing;
                        dpsWhiteBeforeArmor += dpsOHBeforeArmor;
                        OHDPS = dpsOHBeforeArmor * mitigation;

                    }
                    #endregion

                    dpsWhite = MHDPS + OHDPS;
                }
                #endregion

                AbilityHandler abilities = new AbilityHandler(stats, combatTable, character, calcOpts);

                if ((calcOpts.GetRefreshForReferenceCalcs && referenceCalculation)
                    || (calcOpts.GetRefreshForDisplayCalcs && needsDisplayCalculations) 
                    || (calcOpts.GetRefreshForSignificantChange && significantChange)
                    || bChangeRotation)
                {
                    // TODO: This gets very expensive.
                    if (rType == Rotation.Type.Blood)
                    {
                        BloodCycle cycle = new BloodCycle(character, combatTable, stats, calcOpts, abilities);
                        Rotation rot = cycle.GetDamage((int)(calcOpts.FightLength * 60 * 1000));
                        rot.AvgDiseaseMult = 2;
                        rot.NumDisease = 2;
                        rot.CurRotationDuration = calcOpts.FightLength * 60;
                        calcOpts.rotation.copyRotation(rot);
                        r.copyRotation(rot);
                    }
                    else if (rType == Rotation.Type.Unholy)
                    {
                        UnholyCycle cycle = new UnholyCycle(character, combatTable, stats, calcOpts, abilities);
                        Rotation rot = cycle.GetDamage((int)(calcOpts.FightLength * 60 * 1000));
                        rot.AvgDiseaseMult = 3;
                        rot.NumDisease = 3;
                        rot.CurRotationDuration = calcOpts.FightLength * 60;
                        calcOpts.rotation.copyRotation(rot);
                        r.copyRotation(rot);
                    }
                    else if (rType == Rotation.Type.Frost)
                    {
                        FrostCycle cycle = new FrostCycle(character, combatTable, stats, calcOpts, abilities);
                        Rotation rot = cycle.GetDamage((int)(calcOpts.FightLength * 60 * 1000));
                        rot.AvgDiseaseMult = 2;
                        rot.NumDisease = 2;
                        rot.CurRotationDuration = calcOpts.FightLength * 60;
                        calcOpts.rotation.copyRotation(rot);
                        r.copyRotation(rot);
                    }
                    else
                    {
                        r.copyRotation(calcOpts.rotation);
                    }
                }
                else
                {
                    r.copyRotation(calcOpts.rotation);
                }

                _rTypeOld = rType; 

                #region Blood Caked Blade
                {
                    float dpsMHBCB = 0f;
                    float dpsOHBCB = 0f;
                    if ((combatTable.OH.damage != 0) && (DW || combatTable.MH.damage == 0))
                    {
                        float OHBCBDmg = (float)(combatTable.OH.damage * (.25f + .125f * calcOpts.rotation.AvgDiseaseMult));
                        dpsOHBCB = OHBCBDmg / combatTable.OH.hastedSpeed;
                        dpsOHBCB *= OHMult;
                    }
                    if (combatTable.MH.damage != 0)
                    {
                        float MHBCBDmg = (float)(combatTable.MH.damage * (.25f + .125f * calcOpts.rotation.AvgDiseaseMult));
                        dpsMHBCB = MHBCBDmg / combatTable.MH.hastedSpeed;
                    }
                    dpsBCB = dpsMHBCB + dpsOHBCB;
                    dpsBCB *= .1f * (float)talents.BloodCakedBlade;
                }
                #endregion
                #region Bloodworms
                {
                    if (talents.Bloodworms > 0)
                    {
                        float BloodwormSwing = 50f + BloodwormsAPMult * stats.AttackPower;
                        float BloodwormSwingDPS = BloodwormSwing / 2.0f;    // any haste benefits?
                        float TotalBloodworms = ((fightDuration / combatTable.MH.hastedSpeed) + calcOpts.rotation.getMeleeSpecialsPerSecond() * fightDuration)
                            * (0.03f * talents.Bloodworms)
                            * 3f; // 3 bloodworms per proc
                        dpsBloodworms = ((TotalBloodworms * BloodwormSwingDPS * 20f) / fightDuration);
                    }
                }
                #endregion

                #region Trinket direct-damage procs, razorice damage, etc
                {
                    dpsOtherArcane = stats.ArcaneDamage;
                    dpsOtherShadow = stats.ShadowDamage;
                    dpsOtherFire = stats.FireDamage;
                    dpsOtherFrost = stats.FrostDamage;

                    // TODO: Differentiate between MH razorice and OH razorice. Hardly matters though, since razorice hits only based off of base damage
                    if (combatTable.MH.baseDamage != 0 && combatTable.MH.hastedSpeed != 0)
                    {
                        dpsOtherFrost += stats.BonusFrostWeaponDamage * combatTable.MH.baseDamage / combatTable.MH.hastedSpeed;
                    }
                    if (combatTable.OH.baseDamage != 0 && combatTable.OH.hastedSpeed != 0)
                    {
                        dpsOtherFrost += stats.BonusFrostWeaponDamage * combatTable.OH.baseDamage / combatTable.OH.hastedSpeed;
                    }
                    if (DW) dpsOtherFrost /= 2f; //razorice only actually effects the weapon its on, not both. this is closer than it would be otherwise.

                    float OtherCritDmgMult = .5f * (1f + stats.BonusCritMultiplier);
                    float OtherCrit = 1f + ((combatTable.spellCrits) * OtherCritDmgMult);
                    dpsOtherArcane *= OtherCrit;
                    //dpsOtherShadow *= OtherCrit;
                    dpsOtherFire *= OtherCrit;
                    //dpsOtherFrost *= OtherCrit;
                }
                #endregion
                    
                #region Gargoyle
                {
                    if (calcOpts.rotation.GargoyleDuration > 0f && talents.SummonGargoyle > 0f)
                    {
                        float GargoyleCastTime = 2.0f;  //2.0 second base cast time
                        GargoyleCastTime *= combatTable.MH.hastedSpeed / combatTable.MH.baseSpeed;
                        // benefits from all haste effects

                        float GargoyleStrike = 120f + GargoyleAPMult * stats.AttackPower;
                        float GargoyleStrikeCount = 30f / GargoyleCastTime;
                        float GargoyleDmg = GargoyleStrike * GargoyleStrikeCount;
                        GargoyleDmg *= 1f + (.5f * .05f);  // crit rate is uninfluenced by stats, but roughly crap
                        dpsGargoyle = GargoyleDmg / 180f;
                    }
                }
                #endregion

                #region Ghoul
                {
                    if (calcOpts.Ghoul)
                    {
                        float uptime = 1f;
                        if (talents.MasterOfGhouls == 0)
                        {
                            float timeleft = calcOpts.FightLength * 60;
                            float numCDs = timeleft / ((4f - .75f * talents.NightOfTheDead) * 60f);
                            float duration = numCDs * 60f;

                            uptime = duration / timeleft;
                        }

                        float GhoulBaseStrength = 331f;
                        float GhoulBaseStrengthMult = 0.7f;
                        float GhoulBaseAP = 836f;
                        float GhoulBaseSpeed = 2f;
                        float GhoulAPMult = 1f;                 // 1 Str = x AP     
                        float ClawCD = 4f;
                        float GhoulWeaponBaseDamage = 101.3f;   // I found these values to be fairly correct after a number of tests (v3.0.8)
                        float GhoulAPdivisor = 14f;           // 

                        float GlyphofGhoulValue = 0f;
                        if (talents.GlyphoftheGhoul) GlyphofGhoulValue = 0.4f;

                        float GhoulStrengthMult = GhoulBaseStrengthMult + (GhoulBaseStrengthMult * (0.2f * (float)talents.RavenousDead)) + GlyphofGhoulValue;
                        float GhoulStrength = GhoulBaseStrength + stats.Strength * GhoulStrengthMult;

                        Stats statsBuffs = GetBuffsStats(character.ActiveBuffs);

                        GhoulStrength *= 1f + statsBuffs.BonusStrengthMultiplier;
                        GhoulStrength += statsBuffs.Strength;

                        if (character.ActiveBuffs.Contains(Buff.GetBuffByName("Flask of Chromatic Wonder"))) GhoulStrength -= 18f;
                        if (character.ActiveBuffs.Contains(Buff.GetBuffByName("Flask of Chromatic Wonder (Mixology)"))) GhoulStrength -= 4f;
                        if (character.ActiveBuffs.Contains(Buff.GetBuffByName("Elixir of Mighty Strength"))) GhoulStrength -= 50f;
                        if (character.ActiveBuffs.Contains(Buff.GetBuffByName("Elixir of Mighty Strength (Mixology)"))) GhoulStrength -= 16f;
                        if (character.ActiveBuffs.Contains(Buff.GetBuffByName("Guru's Elixir"))) GhoulStrength -= 20f;
                        if (character.ActiveBuffs.Contains(Buff.GetBuffByName("Guru's Elixir (Mixology)"))) GhoulStrength -= 6f;
                        if (character.ActiveBuffs.Contains(Buff.GetBuffByName("Scroll of Strength VIII"))) GhoulStrength -= 30f;

                        float GhoulAP = GhoulBaseAP + GhoulStrength * GhoulAPMult;

                        GhoulAP *= 1f + statsBuffs.BonusAttackPowerMultiplier;
                        GhoulAP += statsBuffs.AttackPower;

                        if (character.ActiveBuffs.Contains(Buff.GetBuffByName("Flask of Endless Rage"))) GhoulAP -= 180f;
                        if (character.ActiveBuffs.Contains(Buff.GetBuffByName("Flask of Endless Rage (Mixology)"))) GhoulAP -= 64f;
                        if (character.ActiveBuffs.Contains(Buff.GetBuffByName("Wrath Elixir"))) GhoulAP -= 90f;
                        if (character.ActiveBuffs.Contains(Buff.GetBuffByName("Wrath Elixir (Mixology)"))) GhoulAP -= 32f;
                        if (character.ActiveBuffs.Contains(Buff.GetBuffByName("Attack Power Food"))) GhoulAP -= 80f;
                        if (character.ActiveBuffs.Contains(Buff.GetBuffByName("Fish Feast"))) GhoulAP -= 80f;

                        float GhoulhastedSpeed = 0f;
                        if (combatTable.MH != null)
                            GhoulhastedSpeed = ((combatTable.MH.hastedSpeed == 0 ? 1.0f : combatTable.MH.hastedSpeed) / (combatTable.MH.baseSpeed == 0 ? 1.0f : combatTable.MH.baseSpeed)) * GhoulBaseSpeed;
                        else
                            GhoulhastedSpeed = GhoulBaseSpeed;

                        if (talents.GhoulFrenzy > 0f)
                        {
                            float GhoulFrenzyHaste = (float)(0.25f * (calcOpts.rotation.GhoulFrenzy / combatTable.realDuration) * 30f);
                            GhoulhastedSpeed /= 1f + GhoulFrenzyHaste;
                        }
                        GhoulhastedSpeed /= 1f + statsBuffs.PhysicalHaste;

                        float dmgSwing = GhoulWeaponBaseDamage + (GhoulAP / GhoulAPdivisor) * GhoulBaseSpeed;
                        float dpsSwing = dmgSwing / GhoulhastedSpeed;
                        float dpsGhoulGlancing = (dpsSwing * 0.24f) * 0.75f;

                        float dpsClaw = (1.5f * dmgSwing) / ClawCD;

                        dpsSwing *= (1f - missedSpecial) - 0.24f;   // the Ghoul only has one weapon || Glancings added further down
                        dpsClaw *= 1f - missedSpecial;

                        dpsSwing *= 1f + .054f;// + stats.LotPCritRating / 4591f; // needs other crit modifiers, but doesn't inherit crit from master
                        dpsSwing += dpsGhoulGlancing;

                        dpsClaw *= 1f + .054f;// + stats.LotPCritRating / 4591f; // needs other crit modifiers, but doesn't inherit crit from master

                        dpsGhoul = dpsSwing + dpsClaw;

                        //dpsGhoul *= 1f + statsBuffs.BonusPhysicalDamageMultiplier; 
                        // commented out because ghoul doesn't benefit from most bonus physical damage multipliers (ie blood presence, bloody vengeance, etc)
                        int targetArmor = calcOpts.BossArmor;
                        float modArmor = 1f - StatConversion.GetArmorDamageReduction(character.Level, targetArmor,
                            0f/*stats.ArmorPenetration*/, 0f, 0f * stats.ArmorPenetrationRating);

                        dpsGhoul *= modArmor;
                        dpsGhoul *= 1f - (.0065f * missedSpecial);	// pet expertise now scales with player hit rating. Hopefully goes off of melee rather than spell hit.
                        dpsGhoul *= uptime;
                    }
                    else
                        dpsGhoul = 0;
                }
                #endregion

                float BCBMult = 1f;
                float DancingRuneWeaponMult = 1f;
                float GargoyleMult = 1f + commandMult;
                float GhoulMult = 1f + commandMult;
                float BloodwormsMult = 1f + commandMult;
                float WhiteMult = 1f;
                float otherShadowMult = 1f;
                float otherArcaneMult = 1f;
                float otherFrostMult = 1f;

                #region Apply Physical Mitigation
                {
                    float physMit = mitigation;
                    physMit *= 1f + (!DW ? .02f * talents.TwoHandedWeaponSpecialization : 0f);

                    dpsBCB *= physMit;
                    dpsBloodworms *= 1f - StatConversion.GetArmorDamageReduction(character.Level, calcOpts.BossArmor, 0f/*stats.ArmorPenetration*/, 0f, 0f);

                    WhiteMult *= physPowerMult * (1f + (!DW ? .02f * talents.TwoHandedWeaponSpecialization : 0f));
                    BCBMult *= physPowerMult * (1f + (!DW ? .02f * talents.TwoHandedWeaponSpecialization : 0f));
                }
                #endregion

                #region Apply Magical Mitigation
                {
                    // some of this applies to necrosis, I wonder if it is ever accounted for
                    float magicMit = partialResist /** combatTable.spellResist*/;
                    // magicMit = 1f - magicMit;

                    otherShadowMult *= spellPowerMult;
                    otherArcaneMult *= spellPowerMult;
                    otherFrostMult *= frostSpellPowerMult;
                }
                #endregion

                #region Cinderglacier multipliers
                {
                    #region Cinderglacier
                    {
                        if (stats.CinderglacierProc > 0f && combatTable.MH != null)
                        {
                            float shadowFrostAbilitiesPerSecond = (float)(
                                (r.DeathCoil 
                                + r.FrostStrike * (1 + talents.ThreatOfThassarian/3f) // FS hits once for each weapon.
                                + r.ScourgeStrike 
                                + r.IcyTouch 
                                + r.HowlingBlast)
                                / combatTable.realDuration);
                            float CGPercentChance = 1.5f / (60f / combatTable.MH.baseSpeed);
                            float CGPPM = ((60f / combatTable.MH.hastedSpeed) * CGPercentChance) * 1f - combatTable.totalMHMiss; // KM Procs per Minute (Defined "1 per point" by Blizzard) influenced by Phys. Haste
                            CGPPM += (1.5f / (60f / combatTable.MH.baseSpeed)) * ((combatTable.totalMeleeAbilities * (1f - combatTable.missedSpecial) + combatTable.totalSpellAbilities * (1f - combatTable.spellResist)) * 60f / combatTable.realDuration);
                            float ProcsPerAbility = CGPPM / (shadowFrostAbilitiesPerSecond * 60f);

                            CinderglacierMultiplier = 1f + 2f * 0.2f * (ProcsPerAbility);

                        }
                    }
                    #endregion
                    abilities.DC.DamageMod *= CinderglacierMultiplier;
                    abilities.HB.DamageMod *= CinderglacierMultiplier;
                    abilities.IT.DamageMod *= CinderglacierMultiplier;
                    abilities.FS.DamageMod *= CinderglacierMultiplier;
                    abilities.SS.DamageMod *= CinderglacierMultiplier;

                }
                #endregion

                #region Apply Multi-Ability Talent Multipliers
                {
                    float BloodyVengeanceMult = .03f * (float)talents.BloodyVengeance;
                    BCBMult *= 1 + BloodyVengeanceMult;
                    WhiteMult *= 1 + BloodyVengeanceMult;

                    float HysteriaCoeff = .2f / 6f; // current uptime is 16.666...%
                    float HysteriaMult = HysteriaCoeff * (float)talents.Hysteria;
                    BCBMult *= 1 + HysteriaMult;
                    WhiteMult *= 1 + HysteriaMult;

                    float BlackIceMult = .02f * (float)talents.BlackIce;
                    otherShadowMult *= 1 + BlackIceMult;
                    otherFrostMult *= 1 + BlackIceMult;

                    float DesecrationMult = .01f * (float)talents.Desolation;  	// the new desecration is basically a flat 1% per point
                    BCBMult *= 1 + DesecrationMult;
                    WhiteMult *= 1 + DesecrationMult;
                    otherShadowMult *= 1 + DesecrationMult;
                    otherArcaneMult *= 1 + DesecrationMult;
                    otherFrostMult *= 1 + DesecrationMult;

                    if ((float)talents.BoneShield >= 1f)
                    {
                        float BoneMult = .02f;
                        BCBMult *= 1 + BoneMult;
                        WhiteMult *= 1 + BoneMult;
                        otherShadowMult *= 1 + BoneMult;
                        otherArcaneMult *= 1 + BoneMult;
                        otherFrostMult *= 1 + BoneMult;
                    }
                }
                #endregion

                #region Pet uptime modifiers
                {
                    BloodwormsMult *= calcOpts.BloodwormsUptime;
                    GhoulMult *= calcOpts.GhoulUptime;
                }
                #endregion
                
                //feed variables for output
                calcs.BCBDPS = dpsBCB * BCBMult;
                calcs.DRWDPS = dpsDancingRuneWeapon * DancingRuneWeaponMult;
                calcs.GargoyleDPS = dpsGargoyle * GargoyleMult;
                calcs.GhoulDPS = dpsGhoul * GhoulMult;
                calcs.BloodwormsDPS = dpsBloodworms * BloodwormsMult;
                calcs.NecrosisDPS = dpsWhite * (.04f * talents.Necrosis);
                calcs.WhiteDPS = dpsWhite * WhiteMult;
                calcs.OtherDPS = dpsOtherShadow * otherShadowMult +
                    dpsOtherArcane * otherArcaneMult +
                    dpsOtherFire * otherArcaneMult +
                    dpsOtherFrost * otherFrostMult;

                // TODO: Re-work this to properly evaluate a base rotation and subrotations w/ special strikes.
                // First let's get the common stuff out of the ifs
                // Diseases & the abilities that cause them
                calcs.PlagueStrikeDPS = (float)((abilities.PS.Damage * r.PlagueStrike) / (calcOpts.FightLength * 60));
                calcs.BloodPlagueDPS = (float)((abilities.BP.Damage * r.BPTick) / (calcOpts.FightLength * 60));
                calcs.IcyTouchDPS = (float)((abilities.IT.Damage * r.IcyTouch) / (calcOpts.FightLength * 60));
                calcs.FrostFeverDPS = (float)((abilities.FF.Damage * r.FFTick) / (calcOpts.FightLength * 60));
                calcs.BloodStrikeDPS = (float)((abilities.BS.Damage * r.BloodStrike) / (calcOpts.FightLength * 60));
                // RP based
                calcs.DeathCoilDPS = (float)((abilities.DC.Damage * r.DeathCoil) / (calcOpts.FightLength * 60));

                if (rType == Rotation.Type.Frost)
                {
                    calcs.BloodStrikeDPS = (float)((abilities.BS.Damage * r.BloodStrike) / (calcOpts.FightLength * 60));
                    if ( talents.FrostStrike > 0 )
                        calcs.FrostStrikeDPS = (float)((abilities.FS.Damage * r.FrostStrike + abilities.FS.SecondaryDamage * r.KMFS) / (calcOpts.FightLength * 60));
                    else
                        calcs.DeathCoilDPS = (float)((abilities.DC.Damage * r.DeathCoil) / (calcOpts.FightLength * 60));

                    if (talents.HowlingBlast > 0)
                        calcs.HowlingBlastDPS = (float)((abilities.HB.Damage * r.HowlingBlast + abilities.HB.SecondaryDamage * r.KMRime) / (calcOpts.FightLength * 60));
                    calcs.ObliterateDPS = (float)((abilities.OB.Damage * r.Obliterate) / (calcOpts.FightLength * 60));
                }

                else if (rType == Rotation.Type.Blood)
                {
                    if (talents.HeartStrike > 0 )
                        calcs.HeartStrikeDPS = (float)((abilities.HS.Damage * r.HeartStrike) / (calcOpts.FightLength * 60));
                    else
                        calcs.BloodStrikeDPS = (float)((abilities.BS.Damage * r.BloodStrike) / (calcOpts.FightLength * 60));
                    calcs.DeathStrikeDPS = (float)((abilities.DS.Damage * r.DeathStrike) / (calcOpts.FightLength * 60));
                    calcs.DeathCoilDPS = (float)((abilities.DC.Damage * r.DeathCoil) / (calcOpts.FightLength * 60));
                }

                else if (rType == Rotation.Type.Unholy)
                {
                    if (talents.ScourgeStrike > 0)
                        calcs.ScourgeStrikeDPS = (float)(((abilities.SS.Damage + abilities.SS.SecondaryDamage) * r.ScourgeStrike) / (calcOpts.FightLength * 60));
                    else
                        calcs.ObliterateDPS = (float)((abilities.OB.Damage * r.Obliterate) / (calcOpts.FightLength * 60));
                    calcs.BloodStrikeDPS = (float)((abilities.BS.Damage * r.BloodStrike) / (calcOpts.FightLength * 60));
                    calcs.DeathCoilDPS = (float)((abilities.DC.Damage * r.DeathCoil) / (calcOpts.FightLength * 60));
                    if (talents.UnholyBlight > 0)
                    {
                        float modifier = (0.1f * (talents.GlyphofUnholyBlight ? 1.4f : 1));
                        calcs.UnholyBlightDPS = calcs.DeathCoilDPS * modifier;
                    }
                }

                calcs.DPSPoints = calcs.BCBDPS + calcs.BloodPlagueDPS + calcs.BloodStrikeDPS + calcs.DeathCoilDPS + calcs.FrostFeverDPS + calcs.FrostStrikeDPS +
                                  calcs.GargoyleDPS + calcs.GhoulDPS + calcs.WanderingPlagueDPS + calcs.HeartStrikeDPS + calcs.HowlingBlastDPS + calcs.IcyTouchDPS +
                                  calcs.NecrosisDPS + calcs.ObliterateDPS + calcs.DeathStrikeDPS + calcs.PlagueStrikeDPS + calcs.ScourgeStrikeDPS + calcs.UnholyBlightDPS +
                                  calcs.WhiteDPS + calcs.OtherDPS + calcs.BloodwormsDPS;

                #region Dancing Rune Weapon
                {
                    if (talents.DancingRuneWeapon > 0)
                    {
                        float dpsDRWMaximum = 0f;
                        String effectsStats = "";
                        Stats maxStats = GetCharacterStatsMaximum(character, additionalItem, 90f);
                        DRW drw;
                        Stats tempStats;

                        foreach (SpecialEffect effect in maxStats.SpecialEffects())
                        {
                            if (effect.Trigger != Trigger.Use)
                            {
                                tempStats = new Stats();
                                tempStats += maxStats;
                                tempStats += effect.Stats;



                                tempStats.Strength += tempStats.HighestStat + tempStats.Paragon;

                                tempStats.Agility = (float)Math.Floor(tempStats.Agility * (1 + tempStats.BonusAgilityMultiplier));
                                tempStats.Strength = (float)Math.Floor(tempStats.Strength * (1 + tempStats.BonusStrengthMultiplier));
                                tempStats.Stamina = (float)Math.Floor(tempStats.Stamina * (1 + tempStats.BonusStaminaMultiplier));
                                tempStats.Intellect = (float)Math.Floor(tempStats.Intellect * (1 + tempStats.BonusIntellectMultiplier));
                                tempStats.Spirit = (float)Math.Floor(tempStats.Spirit * (1 + tempStats.BonusSpiritMultiplier));
                                tempStats.Health = (float)Math.Floor(tempStats.Health + (tempStats.Stamina * 10f));
                                tempStats.Mana = (float)Math.Floor(tempStats.Mana + (tempStats.Intellect * 15f));
                                tempStats.AttackPower = (float)Math.Floor(tempStats.AttackPower + tempStats.Strength * 2);
                                // Copy from TankDK.
                                // tempStats.Armor = (float)Math.Floor((tempStats.Armor + tempStats.BonusArmor + 2f * tempStats.Agility) * 1f);
                                tempStats.Armor = (float)Math.Floor(StatConversion.GetArmorFromAgility(tempStats.Agility) +
                                                    StatConversion.ApplyMultiplier(tempStats.Armor, tempStats.BaseArmorMultiplier) +
                                                    StatConversion.ApplyMultiplier(tempStats.BonusArmor, tempStats.BonusArmorMultiplier));

                                tempStats.AttackPower += (tempStats.Armor / 180f) * (float)talents.BladedArmor;

                                tempStats.BonusSpellPowerMultiplier = tempStats.BonusShadowDamageMultiplier;

                                tempStats.AttackPower *= 1f + tempStats.BonusAttackPowerMultiplier;

                                drw = new DRW(combatTable, calcs, calcOpts, tempStats, character, talents);
                                if (drw.dpsDancingRuneWeapon > dpsDRWMaximum)
                                {
                                    dpsDRWMaximum = drw.dpsDancingRuneWeapon;
                                    effectsStats = effect.ToString();
                                }
                            }

                        }
                        dpsDancingRuneWeapon = dpsDRWMaximum;
                        float DRWUptime = (12f + (talents.GlyphofDancingRuneWeapon ? 5f : 0f)) / 90f;
                        dpsDancingRuneWeapon /= 90f;
                        calcs.DPSPoints += dpsDancingRuneWeapon;
                        calcs.DRWDPS = dpsDancingRuneWeapon;
                        calcs.DRWStats = effectsStats;
                    }
                }
                #endregion

                calcs.OverallPoints = calcs.DPSPoints;

                // TODO: Display rotation data:

            }

            return calcs;
        }

        public CharacterCalculationsBase GetCharacterCalculationsExp(Character character, Item additionalItem, bool referenceCalculation, bool significantChange, bool needsDisplayCalculations)
        {
            // Setup what we need.
            CalculationOptionsDPSDK calcOpts = character.CalculationOptions as CalculationOptionsDPSDK;
            int targetLevel = calcOpts.TargetLevel; // This needs to get from BossHandler.

            Stats stats = new Stats();

            CharacterCalculationsDPSDK calcs = new CharacterCalculationsDPSDK();

            CombatTable combatTable = new CombatTable(character, calcs, stats, calcOpts);

            // Build up Character Stats to do math

            // Perform the calculations

            // Output the results.

            return calcs;
        }

        private Stats GetRaceStats(Character character) 
        {
            return BaseStats.GetBaseStats(character.Level, CharacterClass.DeathKnight, character.Race);
        }

        private Stats GetPresenceStats(CalculationOptionsDPSDK.Presence p)
        {
            Stats PresenceStats = new Stats();
            switch(p)
            {
                case CalculationOptionsDPSDK.Presence.Blood:
                {
                    PresenceStats.BonusPhysicalDamageMultiplier = .15f;
                    PresenceStats.BonusSpellPowerMultiplier = .15f;
                    PresenceStats.BonusShadowDamageMultiplier = .15f;
                    PresenceStats.BonusFrostDamageMultiplier = .15f;
                    break;
                }
                case CalculationOptionsDPSDK.Presence.Unholy:
                {
                    PresenceStats.PhysicalHaste = 0.15f;
                    PresenceStats.SpellHaste = 0.15f;
                    break;
                }
                case CalculationOptionsDPSDK.Presence.Frost:
                {
                    PresenceStats.BonusStaminaMultiplier = 0.08f;
                    PresenceStats.BaseArmorMultiplier = .60f;
                    PresenceStats.DamageTakenMultiplier = -.08f;
                    break;
                }
            }
            return PresenceStats;
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
        public override Stats GetCharacterStats(Character character, Item additionalItem) {
            CalculationOptionsDPSDK calcOpts = character.CalculationOptions as CalculationOptionsDPSDK;
            DeathKnightTalents talents = character.DeathKnightTalents;

            Stats statsRace = GetRaceStats(character);
            Stats statsBaseGear = GetItemStats(character, additionalItem);

            // Filter out the duplicate Runes:
            if (character.OffHandEnchant == Enchant.FindEnchant(3368, ItemSlot.OneHand, character)
                && character.MainHandEnchant == character.OffHandEnchant)
            {
                bool bFC1Found = false;
                bool bFC2Found = false;
                foreach (SpecialEffect se1 in statsBaseGear.SpecialEffects())
                {
                    // if we've already found them, and we're seeing them again, then remove these repeats.
                    if (bFC1Found && _SE_FC1.Stats.Equals(se1.Stats) && _SE_FC1.Trigger.Equals(se1.Trigger))
                        statsBaseGear.RemoveSpecialEffect(se1);
                    else if (bFC2Found && _SE_FC2.Stats.Equals(se1.Stats) && _SE_FC2.Trigger.Equals(se1.Trigger))
                        statsBaseGear.RemoveSpecialEffect(se1);
                    else if (_SE_FC1.Stats.Equals(se1.Stats) && _SE_FC1.Trigger.Equals(se1.Trigger))
                        bFC1Found = true;
                    else if (_SE_FC2.Stats.Equals(se1.Stats) && _SE_FC2.Trigger.Equals(se1.Trigger))
                        bFC2Found = true;
                }
            }

            Stats statsBuffs = GetBuffsStats(character.ActiveBuffs);
            Stats statsPresence = GetPresenceStats(calcOpts.CurrentPresence);
            Stats statsTalents;
            if (calcOpts.m_bExperimental)
            {
                statsTalents = GetTalentStats(talents, character);
            }
            else
            {
                statsTalents = new Stats()
                {
                    // TODO: Expand this since I know in TankDK this was a much broader set of Talent->Stat adjustments.
                    BonusStrengthMultiplier = .01f * (float)(talents.AbominationsMight + talents.RavenousDead) + .02f * (float)(/*talents.ShadowOfDeath + */talents.VeteranOfTheThirdWar + talents.EndlessWinter),
                    BaseArmorMultiplier = .03f * (float)(talents.Toughness),
                    BonusStaminaMultiplier = .02f * (float)(/*talents.ShadowOfDeath + */talents.VeteranOfTheThirdWar),
                    Expertise = (float)(talents.TundraStalker + talents.RageOfRivendare) + 2f * (float)(talents.VeteranOfTheThirdWar),

                    //ArmorPenetration = talents.BloodGorged * 2f / 100,
                    // Improved Icy Talons:
                    PhysicalHaste = 0.04f * talents.IcyTalons + .05f * talents.ImprovedIcyTalons +
                        // IIT does not stack w/ Windfury Totem or Improved Icy Talons from the Buff pane.
                        ((character.ActiveBuffsContains("Improved Icy Talons") || character.ActiveBuffsContains("Windfury Totem")) ? 0 : .2f)
                };
                if (talents.UnbreakableArmor > 0)
                {
                    statsTalents.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BonusStrengthMultiplier = 0.2f }, 20f, 60f));
                }
            }


            Stats statsTotal = new Stats();
            statsTotal.Accumulate(statsBaseGear);
            statsTotal.Accumulate(statsBuffs);
            statsTotal.Accumulate(statsRace);
            statsTotal.Accumulate(statsPresence);
            statsTotal.Accumulate(statsTalents);

            statsTotal = GetRelevantStats(statsTotal);
            statsTotal.Expertise += (float)StatConversion.GetExpertiseFromRating(statsTotal.ExpertiseRating);

            StatsSpecialEffects se = new StatsSpecialEffects(character, statsTotal, new CombatTable(character, statsTotal, calcOpts));
            Stats statSE = new Stats();
            foreach (SpecialEffect e in statsTotal.SpecialEffects())
            {
                // There are some multi-level special effects that need to be factored in.
                foreach (SpecialEffect ee in e.Stats.SpecialEffects())
                {
                    e.Stats = se.getSpecialEffects(calcOpts, ee);
                }
                statSE.Accumulate(se.getSpecialEffects(calcOpts, e));
            }

            float tempCap = StatConversion.RATING_PER_ARMORPENETRATION * (1f - statsTotal.ArmorPenetration);

            foreach (SpecialEffect effect in statsTotal.SpecialEffects())
            {
                if (HasRelevantStats(effect.Stats))
                {
                    // TODO: This code is very expensive for some reason.  
                    // Need to isolate and fix what's going on here to improve performance
                    if (effect.Stats.ArmorPenetrationRating > 0f
                        && ((effect.Stats.ArmorPenetrationRating + statsTotal.ArmorPenetrationRating) > tempCap))
                    {
                        SpecialEffect tempEffect = new SpecialEffect(effect.Trigger, effect.Stats.Clone(), effect.Duration, effect.Cooldown, effect.Chance, effect.MaxStack);
                        tempEffect.Stats.ArmorPenetrationRating = Math.Max(tempCap - statsTotal.ArmorPenetrationRating, 0f);
                        se = new StatsSpecialEffects(character, statsTotal, new CombatTable(character, statsTotal, calcOpts));
                        statsTotal.Accumulate(se.getSpecialEffects(calcOpts, tempEffect));
                    }
                    else
                    {
                        se = new StatsSpecialEffects(character, statsTotal, new CombatTable(character, statsTotal, calcOpts));
                        statsTotal.Accumulate(se.getSpecialEffects(calcOpts, effect));
                    }
                }
            }

            statsTotal.Strength += statsTotal.HighestStat + statsTotal.Paragon + statsTotal.DeathbringerProc/3;
            statsTotal.HasteRating += statsTotal.DeathbringerProc/3;
            statsTotal.CritRating += statsTotal.DeathbringerProc/3;

            statsTotal.Agility = (float)Math.Floor(statsTotal.Agility * (1 + statsTotal.BonusAgilityMultiplier));
            statsTotal.Strength = (float)Math.Floor(statsTotal.Strength * (1 + statsTotal.BonusStrengthMultiplier));
            statsTotal.Stamina = (float)Math.Floor(statsTotal.Stamina * (1 + statsTotal.BonusStaminaMultiplier));
            statsTotal.Intellect = (float)Math.Floor(statsTotal.Intellect * (1 + statsTotal.BonusIntellectMultiplier));
            statsTotal.Spirit = (float)Math.Floor(statsTotal.Spirit * (1 + statsTotal.BonusSpiritMultiplier));
            statsTotal.Health = (float)Math.Floor(statsTotal.Health + (statsTotal.Stamina * 10f));
            statsTotal.Mana = (float)Math.Floor(statsTotal.Mana + (statsTotal.Intellect * 15f));
            statsTotal.AttackPower = (float)Math.Floor(statsTotal.AttackPower + statsTotal.Strength * 2);
            statsTotal.Armor = (float)Math.Floor(StatConversion.GetArmorFromAgility(statsTotal.Agility) +
                                StatConversion.ApplyMultiplier(statsTotal.Armor, statsTotal.BaseArmorMultiplier) +
                                StatConversion.ApplyMultiplier(statsTotal.BonusArmor, statsTotal.BonusArmorMultiplier));

            statsTotal.AttackPower += (statsTotal.Armor / 180f) * (float)talents.BladedArmor;

            statsTotal.BonusSpellPowerMultiplier++;
            statsTotal.BonusFrostDamageMultiplier++;
            statsTotal.BonusShadowDamageMultiplier++;

            statsTotal.AttackPower *= 1f + statsTotal.BonusAttackPowerMultiplier;

            statsTotal.BonusPhysicalDamageMultiplier++;

            if (calcOpts.CurrentPresence == CalculationOptionsDPSDK.Presence.Blood)  // a final, multiplicative component
            {
                statsTotal.BonusPhysicalDamageMultiplier *= 1.15f;
                statsTotal.BonusSpellPowerMultiplier *= 1.15f;
                statsTotal.BonusShadowDamageMultiplier *= 1.15f;
                statsTotal.BonusFrostDamageMultiplier *= 1.15f;
            }
            else if (calcOpts.CurrentPresence == CalculationOptionsDPSDK.Presence.Unholy)  // a final, multiplicative component
            {
                statsTotal.PhysicalHaste += 0.15f;
                statsTotal.SpellHaste += 0.15f;
            }

            return (statsTotal);
        }

        public Stats GetCharacterStatsMaximum(Character character, Item additionalItem, float abilityCooldown)
        {
            CalculationOptionsDPSDK calcOpts = character.CalculationOptions as CalculationOptionsDPSDK;
            DeathKnightTalents talents = character.DeathKnightTalents;
            Stats statsRace = GetRaceStats(character);
            Stats statsBaseGear = GetItemStats(character, additionalItem);
            Stats statsBuffs = GetBuffsStats(character.ActiveBuffs);
            Stats statsTalents;
            if (calcOpts.m_bExperimental)
            {
                statsTalents = GetTalentStats(talents, character);
            }
            else
            {
                statsTalents = new Stats()
                {
                    BonusStrengthMultiplier = .01f * (float)(talents.AbominationsMight + talents.RavenousDead) + .02f * (float)(/*talents.ShadowOfDeath + */talents.VeteranOfTheThirdWar),
                    BaseArmorMultiplier = .03f * (float)(talents.Toughness),
                    BonusStaminaMultiplier = .02f * (float)(/*talents.ShadowOfDeath + */talents.VeteranOfTheThirdWar),
                    Expertise = (float)(talents.TundraStalker + talents.RageOfRivendare) + 2f * (float)(talents.VeteranOfTheThirdWar),
                    BonusPhysicalDamageMultiplier = .02f * (float)(talents.BloodGorged + talents.RageOfRivendare) + 0.03f * talents.TundraStalker,
                    BonusSpellPowerMultiplier = .02f * (float)(talents.BloodGorged + talents.RageOfRivendare) + 0.03f * talents.TundraStalker,
                };
                if (talents.UnbreakableArmor > 0)
                {
                    statsTalents.AddSpecialEffect(_SE_UnbreakableArmor[character.DeathKnightTalents.GlyphofUnbreakableArmor ? 1 : 0][0]);
                }
            }
            Stats statsTotal = new Stats();
            Stats statsGearEnchantsBuffs = new Stats();

            statsGearEnchantsBuffs = statsBaseGear + statsBuffs + statsRace + statsTalents;

            statsTotal = GetRelevantStats(statsGearEnchantsBuffs);
            statsTotal.Expertise += (float)StatConversion.GetExpertiseFromRating(statsGearEnchantsBuffs.ExpertiseRating);

            StatsSpecialEffects se = new StatsSpecialEffects(character, statsTotal, new CombatTable(character, statsTotal, calcOpts));
            int temp = 0;
            Stats statsTemp = new Stats();
            foreach (SpecialEffect effect in statsTotal.SpecialEffects())
            {
                statsTemp.AddSpecialEffect(effect);
            }
            Stats tempStats;
            foreach (SpecialEffect effect in statsTemp.SpecialEffects())
            {
                if (HasRelevantStats(effect.Stats))
                {
                    if (effect.Trigger == Trigger.Use)
                    {
                        float uptimeMult = 0f;
                        if (effect.Cooldown > abilityCooldown)
                        {
                            for (int i = 0; i * effect.Cooldown < calcOpts.FightLength * 60f; i++)
                            {
                                if (i * effect.Cooldown % abilityCooldown == 0)
                                {
                                    uptimeMult++;
                                }
                            }
                            uptimeMult /= calcOpts.FightLength * 60f / abilityCooldown;
                        }
                        float tempCap = StatConversion.RATING_PER_ARMORPENETRATION * (1f - statsTotal.ArmorPenetration);
                        if (effect.Stats.ArmorPenetrationRating > 0f
                            && effect.Stats.ArmorPenetrationRating + statsTotal.ArmorPenetrationRating > tempCap)
                        {
                            tempStats = new Stats();
                            tempStats += effect.Stats;
                            tempStats.ArmorPenetrationRating =
                                (tempCap - statsTotal.ArmorPenetrationRating > 0f ? tempCap - statsTotal.ArmorPenetrationRating : 0f);
                            statsTotal += tempStats * uptimeMult;
                        }
                        else
                        {
                            statsTotal += effect.Stats * uptimeMult;
                        }
                    }
                    else
                    {
                        statsTotal.AddSpecialEffect(effect);
                        temp++;
                    }
                }
            }
            return (statsTotal);
        }

        public Stats GetTalentStats(DeathKnightTalents talents, Character c)
        {
            // TODO: THis will eventually be in the common area.
            Stats TalentStats = new Stats();

            AccumulateTalents(TalentStats, c);

            return TalentStats;
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
                if (!_SE_Bloodworms.ContainsKey(key))
                {
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
                FullCharacterStats.AddSpecialEffect(_SE_UnbreakableArmor[character.DeathKnightTalents.GlyphofUnbreakableArmor ? 1 : 0][0]);
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

        #region Custom Charts
        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
        {
            List<ComparisonCalculationBase> comparisonList = new List<ComparisonCalculationBase>();
            CharacterCalculationsDPSDK baseCalc, calc;
            ComparisonCalculationBase comparison;
            float[] subPoints;
            float fMultiplier = 1f;

            baseCalc = GetCharacterCalculations(character) as CharacterCalculationsDPSDK;

            string[] statList = new string[] 
            {
                "Strength",
                "Agility",
                "Attack Power",
                "Crit Rating",
                "Hit Rating",
                "Expertise Rating",
                "Haste Rating",
                "Armor Penetration Rating",
            };

            switch (chartName)
            {

                //"Item Budget (10 point steps)","Item Budget (25 point steps)","Item Budget(50 point steps)","Item Budget (100 point steps)"
                case "Item Budget (10 point steps)":
                    {
                        fMultiplier = 1f;
                        break;
                    }
                case "Item Budget (25 point steps)":
                    {
                        fMultiplier = 2.5f;
                        break;
                    }
                case "Item Budget (50 point steps)":
                    {
                        fMultiplier = 5f;
                        break;
                    }
                case "Item Budget (100 point steps)":
                    {
                        fMultiplier = 10f;
                        break;
                    }
                case "Presences":
                    {
                        string[] listPresence = new string[] {
                            "None",
                            "Blood",
                            "Unholy",
                            "Frost",
                        };

                        // Set this to have no presence enabled.
                        Character baseCharacter = character.Clone();
                        (baseCharacter.CalculationOptions as CalculationOptionsDPSDK).CurrentPresence = CalculationOptionsDPSDK.Presence.None;
                        // replacing pre-factored base calc since this is different than the Item budget lists. 
                        baseCalc = GetCharacterCalculations(baseCharacter, null, true, false, false) as CharacterCalculationsDPSDK;

                        // Set these to have the key presence enabled.
                        for (int index = 1; index < listPresence.Length; index++)
                        {
                            (character.CalculationOptions as CalculationOptionsDPSDK).CurrentPresence = (CalculationOptionsDPSDK.Presence)index;
                            
                            calc = GetCharacterCalculations(character, null, false, true, true) as CharacterCalculationsDPSDK;

                            comparison = CreateNewComparisonCalculation();
                            comparison.Name = listPresence[index];
                            comparison.Equipped = false;
                            comparison.OverallPoints = calc.OverallPoints - baseCalc.OverallPoints;
                            subPoints = new float[calc.SubPoints.Length];
                            for (int i = 0; i < calc.SubPoints.Length; i++)
                            {
                                subPoints[i] = calc.SubPoints[i] - baseCalc.SubPoints[i];
                            }
                            comparison.SubPoints = subPoints;

                            comparisonList.Add(comparison);
                        }
                        return comparisonList.ToArray();
                    }
                default:
                    return new ComparisonCalculationBase[0];
            }

            // Item Budget list. since it's used multiple times.
            Item[] itemList = new Item[] 
            {
                new Item() { Stats = new Stats() { Strength = 10 * fMultiplier } },
                new Item() { Stats = new Stats() { Agility = 10 * fMultiplier } },
                new Item() { Stats = new Stats() { AttackPower = 20 * fMultiplier } },
                new Item() { Stats = new Stats() { CritRating = 10 * fMultiplier } },
                new Item() { Stats = new Stats() { HitRating = 10 * fMultiplier } },
                new Item() { Stats = new Stats() { ExpertiseRating = 10 * fMultiplier } },
                new Item() { Stats = new Stats() { HasteRating = 10 * fMultiplier } },
                new Item() { Stats = new Stats() { ArmorPenetrationRating = 10 * fMultiplier } },
            };
            // Do the math.
            for (int index = 0; index < itemList.Length; index++)
            {
                calc = GetCharacterCalculations(character, itemList[index]) as CharacterCalculationsDPSDK;

                comparison = CreateNewComparisonCalculation();
                comparison.Name = statList[index];
                comparison.Equipped = false;
                comparison.OverallPoints = calc.OverallPoints - baseCalc.OverallPoints;
                subPoints = new float[calc.SubPoints.Length];
                for (int i = 0; i < calc.SubPoints.Length; i++)
                {
                    subPoints[i] = calc.SubPoints[i] - baseCalc.SubPoints[i];
                }
                comparison.SubPoints = subPoints;

                comparisonList.Add(comparison);
            }
            return comparisonList.ToArray();
        }
        #endregion

        #region Relevant Stats?
        public override bool IsItemRelevant(Item item)
        {
            if (item.Slot == ItemSlot.OffHand /*  ||
                (item.Slot == ItemSlot.Ranged && item.Type != ItemType.Sigil) */)
                return false;
            return base.IsItemRelevant(item);
        }

        public override Stats GetRelevantStats(Stats stats)
        {
            Stats s = new Stats()
            {
                // Core stats
                Strength = stats.Strength,
                Agility = stats.Agility,
                Stamina = stats.Stamina,
                ArmorPenetrationRating = stats.ArmorPenetrationRating,
                ExpertiseRating = stats.ExpertiseRating,
                AttackPower = stats.AttackPower,
                // Other Base Stats
                HasteRating = stats.HasteRating,
                HitRating = stats.HitRating,
                CritRating = stats.CritRating,
                Armor = stats.Armor,
                BonusArmor = stats.BonusArmor,
                Resilience = stats.Resilience,

                // Secondary Stats
                Health = stats.Health,
                ArmorPenetration = stats.ArmorPenetration,
                SpellHaste = stats.SpellHaste,
                PhysicalCrit = stats.PhysicalCrit,
                PhysicalHaste = stats.PhysicalHaste,
                PhysicalHit = stats.PhysicalHit,
                SpellCrit = stats.SpellCrit,
                SpellCritOnTarget = stats.SpellCritOnTarget,
                SpellHit = stats.SpellHit,
                SpellPenetration = stats.SpellPenetration,

                // Damage stats
                WeaponDamage = stats.WeaponDamage,
                PhysicalDamage = stats.PhysicalDamage,
                ShadowDamage = stats.ShadowDamage,
                ArcaneDamage = stats.ArcaneDamage,
                FireDamage = stats.FireDamage,
                FrostDamage = stats.FrostDamage,
                HolyDamage = stats.HolyDamage,
                NatureDamage = stats.NatureDamage,

                // Bonus to stat
                BonusStrengthMultiplier = stats.BonusStrengthMultiplier,
                BonusStaminaMultiplier = stats.BonusStaminaMultiplier,
                BonusAgilityMultiplier = stats.BonusAgilityMultiplier,
                BonusCritMultiplier = stats.BonusCritMultiplier,
                BonusAttackPowerMultiplier = stats.BonusAttackPowerMultiplier,

                // Bonus to Damage
                // *Damage
                BonusDamageMultiplier = stats.BonusDamageMultiplier,
                BonusPhysicalDamageMultiplier = stats.BonusPhysicalDamageMultiplier,
                BonusShadowDamageMultiplier = stats.BonusShadowDamageMultiplier,
                BonusFrostDamageMultiplier = stats.BonusFrostDamageMultiplier,
                BonusDiseaseDamageMultiplier = stats.BonusDiseaseDamageMultiplier,
                BonusRuneStrikeMultiplier = stats.BonusRuneStrikeMultiplier,
                BonusObliterateMultiplier = stats.BonusObliterateMultiplier,
                BonusHeartStrikeMultiplier = stats.BonusHeartStrikeMultiplier,
                BonusScourgeStrikeMultiplier = stats.BonusScourgeStrikeMultiplier,
                // +Damage
                BonusFrostWeaponDamage = stats.BonusFrostWeaponDamage,
                BonusScourgeStrikeDamage = stats.BonusScourgeStrikeDamage,
                BonusBloodStrikeDamage = stats.BonusBloodStrikeDamage,
                BonusDeathCoilDamage = stats.BonusDeathCoilDamage, 
                BonusDeathStrikeDamage =  stats.BonusDeathStrikeDamage,
                BonusFrostStrikeDamage = stats.BonusFrostStrikeDamage,
                BonusHeartStrikeDamage = stats.BonusHeartStrikeDamage,
                BonusIcyTouchDamage = stats.BonusIcyTouchDamage,
                BonusObliterateDamage = stats.BonusObliterateDamage,
                BonusPerDiseaseBloodStrikeDamage = stats.BonusPerDiseaseBloodStrikeDamage,
                BonusPerDiseaseHeartStrikeDamage = stats.BonusPerDiseaseHeartStrikeDamage,
                BonusPerDiseaseObliterateDamage = stats.BonusPerDiseaseObliterateDamage,
                BonusPerDiseaseScourgeStrikeDamage = stats.BonusPerDiseaseScourgeStrikeDamage,
                // Crit
                BonusDeathCoilCrit = stats.BonusDeathCoilCrit,
                BonusDeathStrikeCrit = stats.BonusDeathStrikeCrit,
                BonusFrostStrikeCrit = stats.BonusFrostStrikeCrit,
                BonusObliterateCrit = stats.BonusObliterateCrit,
                BonusPlagueStrikeCrit = stats.BonusPlagueStrikeCrit,
                BonusScourgeStrikeCrit = stats.BonusScourgeStrikeCrit,
                // RP
                BonusRPFromDeathStrike = stats.BonusRPFromDeathStrike,
                BonusRPFromObliterate = stats.BonusRPFromObliterate,
                BonusRPFromScourgeStrike = stats.BonusRPFromScourgeStrike, 
                // Other
                CinderglacierProc = stats.CinderglacierProc,
                DiseasesCanCrit = stats.DiseasesCanCrit,
                HighestStat = stats.HighestStat,
                Paragon = stats.Paragon,
                DeathbringerProc = stats.DeathbringerProc, 

            };

            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if (HasRelevantStats(effect.Stats))
                {
                    if (effect.Trigger == Trigger.DamageDone ||
                        effect.Trigger == Trigger.DamageOrHealingDone ||
                        effect.Trigger == Trigger.DamageSpellCast ||
                        effect.Trigger == Trigger.DamageSpellCrit ||
                        effect.Trigger == Trigger.DamageSpellHit ||
                        effect.Trigger == Trigger.SpellCast ||
                        effect.Trigger == Trigger.SpellCrit ||
                        effect.Trigger == Trigger.SpellHit ||
                        effect.Trigger == Trigger.DoTTick ||
                        effect.Trigger == Trigger.MeleeCrit ||
                        effect.Trigger == Trigger.MeleeHit ||
                        effect.Trigger == Trigger.CurrentHandHit ||
                        effect.Trigger == Trigger.MainHandHit ||
                        effect.Trigger == Trigger.OffHandHit ||
                        effect.Trigger == Trigger.PhysicalCrit ||
                        effect.Trigger == Trigger.PhysicalHit ||
                        effect.Trigger == Trigger.BloodStrikeHit ||
                        effect.Trigger == Trigger.HeartStrikeHit ||
                        effect.Trigger == Trigger.BloodStrikeOrHeartStrikeHit ||
                        effect.Trigger == Trigger.ScourgeStrikeHit ||
                        effect.Trigger == Trigger.ObliterateHit ||
                        effect.Trigger == Trigger.DeathStrikeHit ||
                        effect.Trigger == Trigger.IcyTouchHit ||
                        effect.Trigger == Trigger.PlagueStrikeHit ||
                        effect.Trigger == Trigger.RuneStrikeHit ||
                        effect.Trigger == Trigger.Use)
                    {
                        s.AddSpecialEffect(effect);
                    }
                }
            }

            return s;
        }

        public override bool HasRelevantStats(Stats stats)
        {
            bool bRelevant = false;
            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if (relevantStats(effect.Stats))
                {
                    if (effect.Trigger == Trigger.DamageDone ||
                        effect.Trigger == Trigger.DamageOrHealingDone ||
                        effect.Trigger == Trigger.DamageSpellCast ||
                        effect.Trigger == Trigger.DamageSpellCrit ||
                        effect.Trigger == Trigger.DamageSpellHit ||
                        effect.Trigger == Trigger.SpellCast ||
                        effect.Trigger == Trigger.SpellCrit ||
                        effect.Trigger == Trigger.SpellHit ||
                        effect.Trigger == Trigger.DoTTick ||
                        effect.Trigger == Trigger.MeleeCrit ||
                        effect.Trigger == Trigger.MeleeHit ||
                        effect.Trigger == Trigger.CurrentHandHit ||
                        effect.Trigger == Trigger.MainHandHit ||
                        effect.Trigger == Trigger.OffHandHit ||
                        effect.Trigger == Trigger.PhysicalCrit ||
                        effect.Trigger == Trigger.PhysicalHit ||
                        effect.Trigger == Trigger.BloodStrikeHit ||
                        effect.Trigger == Trigger.HeartStrikeHit ||
                        effect.Trigger == Trigger.BloodStrikeOrHeartStrikeHit ||
                        effect.Trigger == Trigger.ScourgeStrikeHit ||
                        effect.Trigger == Trigger.ObliterateHit ||
                        effect.Trigger == Trigger.DeathStrikeHit ||
                        effect.Trigger == Trigger.IcyTouchHit ||
                        effect.Trigger == Trigger.PlagueStrikeHit ||
                        effect.Trigger == Trigger.RuneStrikeHit ||
                        effect.Trigger == Trigger.Use)
                    {
                        foreach (SpecialEffect e in effect.Stats.SpecialEffects())
                        {
                            if (!bRelevant)
                                bRelevant = HasRelevantStats(e.Stats);
                        }
                        if (!bRelevant)
                            bRelevant = relevantStats(effect.Stats);
                        
                    }
                }
            }
            if (!bRelevant)
                bRelevant = relevantStats(stats);
            return bRelevant;
        }

        private bool relevantStats(Stats stats)
        {
            bool bResults = false;
            // Core stats
            bResults |= (stats.Strength != 0);
            bResults |= (stats.Agility != 0);
            bResults |= (stats.Stamina != 0);
            bResults |= (stats.ArmorPenetrationRating != 0);
            bResults |= (stats.ExpertiseRating != 0);
            bResults |= (stats.AttackPower != 0);
            bool bHasCore = bResults; // if the above stats are 0, lets make sure we're not bringing in caster gear below.
            // Other Base Stats
            bResults |= (stats.HasteRating != 0);
            bResults |= (stats.HitRating != 0);
            bResults |= (stats.CritRating != 0);
            bResults |= (stats.Armor != 0);
            bResults |= (stats.BonusArmor != 0);
            bResults |= (stats.Resilience != 0);

            // Secondary Stats
            bResults |= (stats.Health != 0);
            bResults |= (stats.ArmorPenetration != 0);
            bResults |= (stats.SpellHaste != 0);
            bResults |= (stats.PhysicalCrit != 0);
            bResults |= (stats.PhysicalHaste != 0);
            bResults |= (stats.PhysicalHit != 0);
            bResults |= (stats.SpellCrit != 0);
            bResults |= (stats.SpellCritOnTarget != 0);
            bResults |= (stats.SpellHit != 0);
            bResults |= (stats.SpellHaste != 0);
            bResults |= (stats.SpellPenetration != 0);

            // Damage stats
            bResults |= (stats.WeaponDamage != 0);
            bResults |= (stats.PhysicalDamage != 0);
            bResults |= (stats.ShadowDamage != 0);
            bResults |= (stats.ArcaneDamage != 0);
            bResults |= (stats.FireDamage != 0);
            bResults |= (stats.FrostDamage) != 0;
            bResults |= (stats.HolyDamage) != 0;
            bResults |= (stats.NatureDamage) != 0;

            // Bonus to stat
            bResults |= (stats.BonusStrengthMultiplier != 0);
            bResults |= ( stats.BonusStaminaMultiplier != 0);
            bResults |= ( stats.BonusAgilityMultiplier != 0);
            bResults |= ( stats.BonusCritMultiplier != 0);
            bResults |= (stats.BonusAttackPowerMultiplier != 0);

            // Bonus to Damage
            // *Damage
            bResults |= (stats.BonusDamageMultiplier != 0);
            bResults |= (stats.BonusPhysicalDamageMultiplier != 0);
            bResults |= ( stats.BonusShadowDamageMultiplier != 0);
            bResults |= ( stats.BonusFrostDamageMultiplier != 0);
            bResults |= ( stats.BonusDiseaseDamageMultiplier  != 0);
            bResults |= (stats.BonusRuneStrikeMultiplier != 0);
            bResults |= (stats.BonusObliterateMultiplier != 0);
            bResults |= (stats.BonusHeartStrikeMultiplier != 0);
            bResults |= (stats.BonusScourgeStrikeMultiplier != 0);
            // +Damage
            bResults |= (stats.BonusFrostWeaponDamage != 0);
            bResults |= (stats.BonusScourgeStrikeDamage != 0);
            bResults |= (stats.BonusBloodStrikeDamage != 0);
            bResults |= ( stats.BonusDeathCoilDamage != 0); 
            bResults |= ( stats.BonusDeathStrikeDamage != 0);  
            bResults |= ( stats.BonusFrostStrikeDamage   != 0); 
            bResults |= ( stats.BonusHeartStrikeDamage != 0);  
            bResults |= ( stats.BonusIcyTouchDamage != 0);  
            bResults |= ( stats.BonusObliterateDamage != 0);
            bResults |= (stats.BonusPerDiseaseBloodStrikeDamage != 0);
            bResults |= (stats.BonusPerDiseaseHeartStrikeDamage != 0);
            bResults |= (stats.BonusPerDiseaseObliterateDamage != 0);
            bResults |= (stats.BonusPerDiseaseScourgeStrikeDamage != 0);
            // Crit
            bResults |= (stats.BonusCritMultiplier != 0);
            bResults |= (stats.BonusDeathCoilCrit != 0);
            bResults |= ( stats.BonusDeathStrikeCrit != 0);
            bResults |= ( stats.BonusFrostStrikeCrit != 0); 
            bResults |= ( stats.BonusObliterateCrit != 0); 
            bResults |= ( stats.BonusPlagueStrikeCrit != 0);
            bResults |= (stats.BonusScourgeStrikeCrit != 0);
            // RP
            bResults |= ( stats.BonusRPFromDeathStrike != 0);
            bResults |= ( stats.BonusRPFromObliterate != 0);
            bResults |= ( stats.BonusRPFromScourgeStrike != 0); 
            // Other
            bResults |= ( stats.CinderglacierProc != 0); 
            bResults |= ( stats.DiseasesCanCrit != 0); 
            bResults |= ( stats.HighestStat != 0); 
            bResults |= ( stats.Paragon != 0); 
            bResults |= ( stats.DeathbringerProc != 0); 

            // Filter out caster gear:
            if (!bHasCore & bResults)
                // Let's make sure that if we've got some stats that may be interesting
            {
                bResults = !((stats.Intellect != 0)
                    || (stats.Spirit != 0)
                    || (stats.Mp5 != 0)
                    || (stats.SpellPower != 0)
                    || (stats.Mana != 0)
                    );
            }
            return bResults;
        }
        #endregion

        private string[] _optimizableCalculationLabels = null;
        public override string[] OptimizableCalculationLabels
        {
            get
            {
                if (_optimizableCalculationLabels == null)
                    _optimizableCalculationLabels = new string[] {
                        "Health",
                        "Crit Rating",
                        "Expertise Rating",
                        "Hit Rating",
                        "Haste Rating",
                        "Target Miss %",
                        "Target Dodge %",
                        "Resilience",
                        "Spell Penetration"
                    };

                return _optimizableCalculationLabels;
            }
        }

        #region Static SpecialEffects
        private static Dictionary<float, SpecialEffect[]> _SE_SpellDeflection = new Dictionary<float, SpecialEffect[]>();
        private static readonly SpecialEffect _SE_T10_4P = new SpecialEffect(Trigger.Use, new Stats() { DamageTakenMultiplier = -0.12f }, 10f, 60f);
        private static readonly SpecialEffect _SE_FC1 = new SpecialEffect(Trigger.DamageDone, new Stats() { BonusStrengthMultiplier = .15f }, 15f, 0f, -2f, 1);
        private static readonly SpecialEffect _SE_FC2 = new SpecialEffect(Trigger.DamageDone, new Stats() { HealthRestoreFromMaxHealth = .03f }, 0, 0f, -2f, 1);
        private static readonly SpecialEffect[][] _SE_VampiricBlood = new SpecialEffect[][] {
            new SpecialEffect[] { new SpecialEffect(Trigger.Use, null, 10 + 0 * 5, 60f - (false ? 0 : 10)), new SpecialEffect(Trigger.Use, null, 10 + 0 * 5, 60f - (true ? 0 : 10)),},
            new SpecialEffect[] { new SpecialEffect(Trigger.Use, null, 10 + 1 * 5, 60f - (false ? 0 : 10)), new SpecialEffect(Trigger.Use, null, 10 + 1 * 5, 60f - (true ? 0 : 10)),},
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
    }
}
