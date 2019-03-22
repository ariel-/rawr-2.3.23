using System;
#if RAWR3
using System.Windows.Media;
#else
using System.Drawing;
#endif
using System.Collections.Generic;

namespace Rawr.RestoSham
{
    [Rawr.Calculations.RawrModelInfo("RestoSham", "Spell_Nature_Magicimmunity", CharacterClass.Shaman)]
    public class CalculationsRestoSham : CalculationsBase
    {
        //  Base mana at level 80
        const float BaseMana = 4396f;

        // Carry over calculations
        public float HealPerSec { get; set; }
        public float HealHitPerSec { get; set; }
        public float CritPerSec { get; set; }
        public float FightSeconds { get; set; }
        public float castingActivity { get; set; }

        #region Setup Character Defaults
        /// <summary>
        /// Sets the defaults for a RestoShaman character
        /// </summary>
        /// <param name="character">The character object to set the defaults for</param>
        public override void SetDefaults(Character character)
        {
            character.ActiveBuffsAdd(("Improved Moonkin Form"));
            character.ActiveBuffsAdd(("Tree of Life Aura"));
            character.ActiveBuffsAdd(("Arcane Intellect"));
            character.ActiveBuffsAdd(("Vampiric Touch"));
            character.ActiveBuffsAdd(("Mana Spring Totem"));
            character.ActiveBuffsAdd(("Restorative Totems"));
            character.ActiveBuffsAdd(("Moonkin Form"));
            character.ActiveBuffsAdd(("Wrath of Air Totem"));
            character.ActiveBuffsAdd(("Totem of Wrath (Spell Power)"));
            character.ActiveBuffsAdd(("Power Word: Fortitude"));
            character.ActiveBuffsAdd(("Improved Power Word: Fortitude"));
            character.ActiveBuffsAdd(("Mark of the Wild"));
            character.ActiveBuffsAdd(("Improved Mark of the Wild"));
            character.ActiveBuffsAdd(("Blessing of Kings"));
            character.ActiveBuffsAdd(("Flask of the Frost Wyrm"));
            character.ActiveBuffsAdd(("Spell Power Food"));
            character.ActiveBuffsAdd(("Earthliving Weapon"));
        }
        public override List<string> GetRelevantGlyphs()
        {
            return Relevants.RelevantGlyphs;
        }
        public override List<GemmingTemplate> DefaultGemmingTemplates
        {
            get
            {
                ////Relevant Gem IDs for Resto Shaman - Goes Normal, Rare, Epic, and JC
                //Red
                int[] Runed = { 39911, 39998, 40113, 42144 };

                //Purple
                int[] Royal = { 39943, 40027, 40134 };
                int[] Glowing = { 39936, 40025, 40132 };

                //Blue
                int[] Lustrous = { 39927, 40010, 40121, 42146 };

                //Green
                int[] Dazzling = { 39984, 40094, 40175 };
                int[] Energized = { 39989, 40105, 40179 };
                int[] Sundered = { 39985, 40096, 40176 };

                //Yellow
                int[] Brilliant = { 39912, 40012, 40123, 42148 };
                int[] Quick = { 39918, 40017, 40128, 42150 };
                int[] Smooth = { 39914, 40013, 40124, 42149 };

                //Orange
                int[] Luminous = { 39946, 40047, 40151 };
                int[] Reckless = { 39959, 40051, 40155 };
                int[] Potent = { 39956, 40048, 40152 };

                //Meta
                int Ember = 41333;
                int Insightful = 41401;
                int Revitalizing = 41376;

                return new List<GemmingTemplate>()
				{
					new GemmingTemplate() { Model = "RestoSham", Group = "Uncommon", //Max Spellpower
						RedId = Runed[0], YellowId = Runed[0], BlueId = Runed[0], PrismaticId = Runed[0], MetaId = Ember },
					new GemmingTemplate() { Model = "RestoSham", Group = "Uncommon", //SP Int
						RedId = Runed[0], YellowId = Luminous[0], BlueId = Glowing[0], PrismaticId = Runed[0], MetaId = Ember },
					new GemmingTemplate() { Model = "RestoSham", Group = "Uncommon", //Heavy MP5
						RedId = Royal[0], YellowId = Dazzling[0], BlueId = Dazzling[0], PrismaticId = Lustrous[0], MetaId = Insightful },
					new GemmingTemplate() { Model = "RestoSham", Group = "Uncommon", //SP MP5
						RedId = Runed[0], YellowId = Dazzling[0], BlueId = Royal[0], PrismaticId = Runed[0], MetaId = Insightful },
					new GemmingTemplate() { Model = "RestoSham", Group = "Uncommon", //SP Haste
						RedId = Runed[0], YellowId = Reckless[0], BlueId = Energized[0], PrismaticId = Quick[0], MetaId = Revitalizing },
					new GemmingTemplate() { Model = "RestoSham", Group = "Uncommon", //SP Crit
						RedId = Runed[0], YellowId = Potent[0], BlueId = Sundered[0], PrismaticId = Smooth[0], MetaId = Revitalizing },

					new GemmingTemplate() { Model = "RestoSham", Group = "Rare",
						RedId = Runed[1], YellowId = Runed[1], BlueId = Runed[1], PrismaticId = Runed[1], MetaId = Ember },
					new GemmingTemplate() { Model = "RestoSham", Group = "Rare",
						RedId = Runed[1], YellowId = Luminous[1], BlueId = Glowing[1], PrismaticId = Runed[1], MetaId = Ember },
					new GemmingTemplate() { Model = "RestoSham", Group = "Rare",
						RedId = Royal[1], YellowId = Dazzling[1], BlueId = Dazzling[1], PrismaticId = Lustrous[1], MetaId = Insightful },
					new GemmingTemplate() { Model = "RestoSham", Group = "Rare",
						RedId = Runed[1], YellowId = Dazzling[1], BlueId = Royal[1], PrismaticId = Runed[1], MetaId = Insightful },
					new GemmingTemplate() { Model = "RestoSham", Group = "Rare",
						RedId = Runed[1], YellowId = Reckless[1], BlueId = Energized[1], PrismaticId = Quick[1], MetaId = Revitalizing },
					new GemmingTemplate() { Model = "RestoSham", Group = "Rare",
						RedId = Runed[1], YellowId = Potent[1], BlueId = Sundered[1], PrismaticId = Smooth[1], MetaId = Revitalizing },

					new GemmingTemplate() { Model = "RestoSham", Group = "Epic", Enabled = true, //Max Haste + Insight
						RedId = Quick[2], YellowId = Quick[2], BlueId = Quick[2], PrismaticId = Quick[2], MetaId = Insightful },
					new GemmingTemplate() { Model = "RestoSham", Group = "Epic", Enabled = true, //SP Int + Insight
						RedId = Luminous[2], YellowId = Luminous[2], BlueId = Dazzling[2], PrismaticId = Luminous[2], MetaId = Insightful },
					new GemmingTemplate() { Model = "RestoSham", Group = "Epic", Enabled = true, //Heavy Haste + Insight
						RedId = Reckless[2], YellowId = Quick[2], BlueId = Energized[2], PrismaticId = Quick[2], MetaId = Insightful },
					new GemmingTemplate() { Model = "RestoSham", Group = "Epic", Enabled = true, //SP Crit + Insight
						RedId = Runed[2], YellowId = Potent[2], BlueId = Sundered[2], PrismaticId = Smooth[2], MetaId = Insightful },
					new GemmingTemplate() { Model = "RestoSham", Group = "Epic", Enabled = true, //Max Haste + Revitalizing
						RedId = Quick[2], YellowId = Quick[2], BlueId = Quick[2], PrismaticId = Quick[2], MetaId = Revitalizing },
					new GemmingTemplate() { Model = "RestoSham", Group = "Epic", Enabled = true, //SP Int + Revitalizing
						RedId = Luminous[2], YellowId = Luminous[2], BlueId = Dazzling[2], PrismaticId = Luminous[2], MetaId = Revitalizing },
					new GemmingTemplate() { Model = "RestoSham", Group = "Epic", Enabled = true, //Heavy Haste + Revitalizing
						RedId = Reckless[2], YellowId = Quick[2], BlueId = Energized[2], PrismaticId = Quick[2], MetaId = Revitalizing },
					new GemmingTemplate() { Model = "RestoSham", Group = "Epic", Enabled = true, //SP Crit + Revitalizing
						RedId = Runed[2], YellowId = Potent[2], BlueId = Sundered[2], PrismaticId = Smooth[2], MetaId = Revitalizing },
					new GemmingTemplate() { Model = "RestoSham", Group = "Epic", //SP MP5
						RedId = Runed[2], YellowId = Dazzling[2], BlueId = Royal[2], PrismaticId = Runed[2], MetaId = Insightful },
					new GemmingTemplate() { Model = "RestoSham", Group = "Epic", //Heavy MP5
						RedId = Royal[2], YellowId = Dazzling[2], BlueId = Dazzling[2], PrismaticId = Lustrous[2], MetaId = Insightful },

					new GemmingTemplate() { Model = "RestoSham", Group = "Jeweler", //Max Haste + Insight
						RedId = Quick[3], YellowId = Quick[3], BlueId = Quick[3], PrismaticId = Quick[3], MetaId = Insightful },
					new GemmingTemplate() { Model = "RestoSham", Group = "Jeweler", //SP Int + Insight
						RedId = Runed[3], YellowId = Brilliant[3], BlueId = Dazzling[2], PrismaticId = Luminous[2], MetaId = Insightful },
					new GemmingTemplate() { Model = "RestoSham", Group = "Jeweler", //Heavy Haste + Insight
						RedId = Reckless[2], YellowId = Quick[3], BlueId = Energized[2], PrismaticId = Quick[3], MetaId = Insightful },
					new GemmingTemplate() { Model = "RestoSham", Group = "Jeweler", //SP Crit + Insight
						RedId = Runed[3], YellowId = Smooth[3], BlueId = Sundered[2], PrismaticId = Smooth[3], MetaId = Insightful },
					new GemmingTemplate() { Model = "RestoSham", Group = "Jeweler", //Max Haste + Revitalizing
						RedId = Quick[3], YellowId = Quick[3], BlueId = Quick[3], PrismaticId = Quick[3], MetaId = Revitalizing },
					new GemmingTemplate() { Model = "RestoSham", Group = "Jeweler", //SP Int + Revitalizing
						RedId = Runed[3], YellowId = Brilliant[3], BlueId = Dazzling[2], PrismaticId = Luminous[2], MetaId = Revitalizing },
					new GemmingTemplate() { Model = "RestoSham", Group = "Jeweler", //Heavy Haste + Revitalizing
						RedId = Reckless[2], YellowId = Quick[3], BlueId = Energized[2], PrismaticId = Quick[3], MetaId = Revitalizing },
					new GemmingTemplate() { Model = "RestoSham", Group = "Jeweler", //SP Crit + Revitalizing
						RedId = Runed[3], YellowId = Smooth[3], BlueId = Sundered[2], PrismaticId = Smooth[3], MetaId = Revitalizing },
				};
            }
        }
        public override List<ItemType> RelevantItemTypes
        {
            get { return Relevants.RelevantItemTypes; }
        }
        #endregion

        #region Labels and Charts Overrides
        public override Dictionary<string, Color> SubPointNameColors
        {
            get { return RestoShamConfiguration.SubPointNameColors; }
        }
        public override string[] CharacterDisplayCalculationLabels
        {
            get { return RestoShamConfiguration.CharacterDisplayCalculationLabels; }
        }
        public override string[] OptimizableCalculationLabels
        {
            get { return RestoShamConfiguration.OptimizableCalculationLabels; }
        }
        public override string[] CustomChartNames
        {
            get { return CustomCharts.CustomChartNames; }
        }
#if !RAWR3
        // for RAWR3 include all charts in CustomChartNames
        public override string[] CustomRenderedChartNames
        {
            get { return CustomCharts.CustomRenderedChartNames; }
        }
#endif
        #endregion

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
                    _calculationOptionsPanel = new CalculationOptionsPanelRestoSham();
                return _calculationOptionsPanel;
            }
        }
        public override bool ItemFitsInSlot(Item item, Character character, CharacterSlot slot, bool ignoreUnique)
        {
            if (slot == CharacterSlot.OffHand && item.Slot == ItemSlot.OneHand) return false;
            return base.ItemFitsInSlot(item, character, slot, ignoreUnique);
        }
        
        #region Model Verification and prepare Calculations
        //
        // This model is for shammies!
        //
        public override CharacterClass TargetClass
        {
            get { return CharacterClass.Shaman; }
        }


        //
        // Get instances of our calculation classes:
        //
        public override ComparisonCalculationBase CreateNewComparisonCalculation()
        {
            return new ComparisonCalculationRestoSham();
        }

        public override CharacterCalculationsBase CreateNewCharacterCalculations()
        {
            return new CharacterCalculationsRestoSham();
        }
        #endregion
        #region Calculations Begin
        public override CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, bool referenceCalculation, bool significantChange, bool needsDisplayCalculations)
        {
            return GetCharacterCalculations(character, additionalItem, null);
        }
        public CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, Stats statModifier)
        {
            Stats stats = GetCharacterStats(character, additionalItem, statModifier);
            CharacterCalculationsRestoSham calcStats = new CharacterCalculationsRestoSham();
            calcStats.BasicStats = stats;
            CalculationOptionsRestoSham options = character.CalculationOptions as CalculationOptionsRestoSham;
            if (options == null)
                return calcStats;

            FightSeconds = options.FightLength * 60f;
            castingActivity = options.ActivityPerc * .01f;
            #region Spell Power and Haste Based Calcs
            stats.SpellPower += stats.Earthliving * ((1 + character.ShamanTalents.ElementalWeapons * .1f) * 150f);
            if (options.SustStyle.Contains("CH"))
                stats.SpellPower += stats.RestoShamRelicT9;
            if (options.SustStyle.Contains("RT"))
                stats.SpellPower += stats.RestoShamRelicT10;
            stats.SpellHaste = (1 + StatConversion.GetSpellHasteFromRating(stats.HasteRating)) * (1 + stats.SpellHaste) - 1;
            //  FIXME: you can't really model Hero/BL as a flat haste increase since with decent amounts of haste from gear/buffs you'll begin hitting the GCD cap, which will skew a lot of computations
            /*
            if (options.Heroism.Equals("Me") || options.Heroism.Equals("Yes"))
                stats.SpellHaste = (1 + stats.SpellHaste) * (1 + .3f / FightSeconds) - 1;
            if (options.Heroism.Equals("Me"))
                stats.Mp5 -= .19f * BaseMana * 5 / FightSeconds;
             * */
            calcStats.SpellHaste = stats.SpellHaste;
            float Healing = 1.88f * stats.SpellPower;
            #endregion
            #region Overhealing/Latency/Interval/CH jumps values
            float GcdLatency = options.Latency / 1000;
            float Latency = Math.Max(Math.Min(GcdLatency, 0.275f) - 0.14f, 0) + Math.Max(GcdLatency - 0.275f, 0);
#if true
            float ESOverheal = 0;
            float ELWOverheal = 0;
            float CHOverheal = 0;
            float RTOverheal = 0;
            float HWOverheal = 0;
            float HWSelfOverheal = 0;
            float LHWOverheal = 0;
            float AAOverheal = 0;
            float HSTOverheal = 0;
            float ESInterval = 0;
#else
            float ESOverheal = 0.1f;
            float ELWOverheal = 0.7f;
            float CHOverheal = 0.7f;
            float RTOverheal = 0.8f;
            float HWOverheal = 0.4f;
            float HWSelfOverheal = 0.9f;
            float LHWOverheal = 0.75f;
            float AAOverheal = 0.6f;
            float HSTOverheal = 0.9f;
            float ESInterval = 6;
#endif
            bool TankHeal = options.Targets == "Tank";
            bool RaidHeal = options.Targets == "Heavy Raid";
            bool SelfHeal = RaidHeal || options.Targets == "Self";
            float ELWOverwriteScale = RaidHeal ? 0.875f : TankHeal ? 0.5f : 0.6f;
            float CHRTConsumption = RaidHeal ? 0.07f : TankHeal ? 0.5f : 0.19f;
            float CHJumps = character.ShamanTalents.GlyphofChainHeal ?
                (RaidHeal ? 4 : SelfHeal ? 1.73f : TankHeal ? 1.86f : 2.5f) :
                (RaidHeal ? 3 : SelfHeal ? 1.3f : TankHeal ? 1.41f : 1.8f);
            float HSTTargets = RaidHeal ? 5f : 1f;
            #endregion
            #region Intellect and MP5 Based Calcs
            float CritPenalty = 1f - (((CHOverheal + RTOverheal + HWOverheal + HWSelfOverheal + LHWOverheal + AAOverheal) / 6f) / 2f);
            stats.Mp5 += (float)Math.Round((stats.Intellect * ((character.ShamanTalents.UnrelentingStorm / 3) * .1f)), 0);
            stats.SpellCrit = .022f + StatConversion.GetSpellCritFromIntellect(stats.Intellect)
                + StatConversion.GetSpellCritFromRating(stats.CritRating) + stats.SpellCrit +
                (.01f * (character.ShamanTalents.TidalMastery + character.ShamanTalents.ThunderingStrikes +
                (character.ShamanTalents.BlessingOfTheEternals * 2)));
            calcStats.SpellCrit = stats.SpellCrit;
            float CriticalScale = 1.5f * (1 + stats.BonusCritHealMultiplier);
            float CriticalChance = calcStats.SpellCrit;
            float Critical = 1f + ((CriticalChance * Math.Min(CriticalScale - 1, 1)) * (CritPenalty));  //  The penalty is set to ensure that while no crit will ever be valued less then a full heal, it will however be reduced more so due to overhealing.  The average currently works out close to current HEP reports and combat logs.
            float ChCritical = 1f + (((CriticalChance + (stats.RestoSham4T9 * .05f)) * Math.Min(CriticalScale - 1, 1)) * (CritPenalty));

            #endregion
            #region Healing Bonuses and scales
            //  Cost scale
            float CostScale = 1f - character.ShamanTalents.TidalFocus * .01f;
            //  Healing scale from Purification
            float PurificationScale = 1 + character.ShamanTalents.Purification * .02f;
            //  AA scale
            float AAScale = CriticalScale * character.ShamanTalents.AncestralAwakening * .1f * PurificationScale;
            //  TW chance
            float TWChance = character.ShamanTalents.TidalWaves * 0.2f;
            #endregion
            #region Water Shield and Mana Calculations
            //// Code Flag: Dodger = Re-cast not needed in 3.2
            //float WSC = (float)Math.Max((1.5 * (1 - (calcStats.SpellHaste))), 1f) / 4;
            float Orb;
            if (options.WaterShield)
            {
                stats.Mp5 += (character.ShamanTalents.GlyphofWaterMastery ? 130 : 100) +
                    stats.TotemThunderhead * 2 +
                    100f * stats.WaterShieldIncrease;
                Orb = (428 * (1 + (character.ShamanTalents.ImprovedShields * .05f))) * (1 + stats.WaterShieldIncrease) +
                    stats.TotemThunderhead * 27;
                Orb = Orb * character.ShamanTalents.ImprovedWaterShield / 3;
            }
            else
                Orb = 0;
            #endregion
            #region Earthliving Weapon Calculations
            //  ELW bonus healing = spell power
            float ELWBonusHealing = stats.SpellPower;
            //  ... * generic healing scale * HoT scale
            ELWBonusHealing *= 1.88f * 0.3636f;
            //  ELW healing scale = purification scale
            float ELWHealingScale = PurificationScale;
            float ELWHPS = ((652 + ELWBonusHealing) * ELWHealingScale / 12) * (1 + stats.BonusHealingDoneMultiplier);
            float ELWChance = stats.Earthliving * (0.2f + (character.ShamanTalents.GlyphofEarthlivingWeapon ? .05f : 0)) * ELWOverwriteScale;
            #endregion
            #region Earth Shield Calculations
            bool UseES = (options.EarthShield && character.ShamanTalents.EarthShield > 0); // Wether or not to use ES at all - Make sure the option and the talent are on.
            float ESBonusHealing = stats.SpellPower;  //  ES bonus healing = spell power
            ESBonusHealing *= 1.88f * (1f / 3.5f);  //  ... * generic healing scale * HoT scale
            float ESHealingScale = PurificationScale;  //  ES healing scale = purification scale
            //  ... + 5%/10% Improved Earth Shield + 5%/10%/15% Improved Shields
            ESHealingScale *= 1 + character.ShamanTalents.ImprovedEarthShield * 0.05f + character.ShamanTalents.ImprovedShields * 0.05f;
            //  ... + 20% if Glyphed
            if (character.ShamanTalents.GlyphofEarthShield)
                ESHealingScale *= 1.2f;
            float ESChargeHeal = (337 + ESBonusHealing) * ESHealingScale * (UseES ? 1 : 0); //  Heal per ES Charge
            float ESHeal = ESChargeHeal * (6 + character.ShamanTalents.ImprovedEarthShield);  //  ES if all charges heal
            float ESCost = (float)Math.Round(.15 * BaseMana) * CostScale * (UseES ? 1 : 0);
            float ESTimer = (6 + character.ShamanTalents.ImprovedEarthShield) * Math.Max(ESInterval, 4);
            calcStats.ESHPS = ((ESHeal * Critical) / ESTimer) * (1 + stats.BonusHealingDoneMultiplier);
            float ESMPS = ESCost / ESTimer;
            #endregion
            #region Base Variables ( Heals per sec and Crits per sec )
            float RTPerSec = 0;
            float RTTicksPerSec = 0;
            float HWPerSec = 0;
            float CHPerSec = 0;
            float CHHitsPerSec = 0;
            float LHWPerSec = 0;
            float LHWCPerSec = 0f;
            float CHCPerSec = 0f;
            float CHCHitsPerSec = 0;
            float HWCPerSec = 0f;
            float RTCPerSec = 0f;
            float AAsPerSec = 0f;
            float ELWTicksPerSec = 0;
            #endregion
            #region Base Speeds ( Hasted / RTCast / LHWCast / HWCast / CHCast )
            float HasteScale = 1f / (1f + calcStats.SpellHaste);
            float RTHaste = stats.RestoSham2T10 * 0.2f;
            float RTCast = (float)Math.Max(1.5f * HasteScale + Latency, 1f + GcdLatency);
            float RTCD = 6 - stats.RTCDDecrease;
            float RTCDCast = RTCD + GcdLatency;
            float RTDuration = 15 + (character.ShamanTalents.GlyphofRiptide ? 6 : 0);
            float ELWDuration = stats.Earthliving * 12;
            float HWCastBase = 3.0f - (character.ShamanTalents.ImprovedHealingWave * .1f);
            calcStats.RealHWCast = HWCastBase * HasteScale;
            float HWCast = (float)Math.Max(HWCastBase * HasteScale + Latency, 1f + GcdLatency);
            float HWCastTWLatency = (Latency * 0.25f + GcdLatency * 0.75f) * TWChance + (Latency * 0.5f + GcdLatency * 0.5f) * (1 - TWChance);
            float HWCastTW = (float)Math.Max(HWCastBase * HasteScale * 0.7f + HWCastTWLatency, 1f + GcdLatency);
            float HWCast_RT = (float)Math.Max(HWCastBase / (1f + calcStats.SpellHaste + RTHaste), 1f) + GcdLatency;
            float HWCastTW_RT = (float)Math.Max(HWCastBase / (1f + calcStats.SpellHaste + RTHaste) * 0.7f + HWCastTWLatency, 1f + GcdLatency);
            calcStats.RealLHWCast = 1.5f * HasteScale;
            float LHWCast = (float)Math.Max(1.5f * HasteScale, 1f) + GcdLatency;
            float LHWCast_RT = (float)Math.Max(1.5f / (1f + calcStats.SpellHaste + RTHaste), 1f) + GcdLatency;
            float CHCastBase = 2.5f - stats.CHCTDecrease;
            calcStats.RealCHCast = CHCastBase * HasteScale;
            float CHCast = (float)Math.Max(CHCastBase * HasteScale + Latency, 1f + GcdLatency);
            float CHCast_RT = (float)Math.Max(CHCastBase / (1f + calcStats.SpellHaste + RTHaste), 1f) + GcdLatency;
            // This totally heals the boss backwards! Yeah! :D
            // Don't worry about this messing with procs or anything, it's just to show on the stats page. :)
            calcStats.LBCast = (float)Math.Max((2.5f - 0.1 * character.ShamanTalents.LightningMastery) * HasteScale, 1f);
            #endregion
            #region Base Spells ( TankCH / RTHeal / LHWHeal / HWHeal / CHHeal )
            //  RT bonus healing = spell power
            float RTBonusHealing = stats.SpellPower;
            //  ... * generic healing scale * HoT scale
            RTBonusHealing *= 1.88f * (1f / 3.5f);
            //  RT healing scale = purification scale
            float RTHealingScale = PurificationScale;
            //  ... + 20% 2pc T9 bonus
            RTHealingScale *= 1 + stats.RestoSham2T9 * .2f;
            //  ... set to zero if RT talent is not taken
            float RTHeal = (1670 + RTBonusHealing) * RTHealingScale * character.ShamanTalents.Riptide;
            //  RT HoT bonus healing = spell power
            float RTHotBonusHealing = stats.SpellPower;
            //  ... * generic healing scale * HoT scale
            RTHotBonusHealing *= 1.88f * 0.5f;
            float RTHotHeal = (1670 + RTHotBonusHealing) * RTHealingScale * character.ShamanTalents.Riptide;
            float RTHotTickHeal = RTHotHeal / 5;
            RTHotHeal = RTDuration / 3 * RTHotTickHeal;
            float RTHotHPS = RTHotTickHeal / 3;
            //  LHW bonus healing = spell power + totem spell power bonus
            float LHWBonusHealing = stats.SpellPower + stats.TotemLHWSpellpower;
            //  ... * generic healing scale + bonus from TW
            LHWBonusHealing *= 1.88f * (1.5f / 3.5f) + character.ShamanTalents.TidalWaves * .02f;
            //  LHW healing scale = purification scale
            float LHWHealingScale = PurificationScale;
            //  ... + 20% if Glyphed, tank healing and ES is enabled
            if (character.ShamanTalents.GlyphofLesserHealingWave && TankHeal && UseES)
                LHWHealingScale *= 1.2f;
            float LHWHeal = (1720 + LHWBonusHealing) * LHWHealingScale;
            //  HW bonus healing = spell power + totem spell power bonus
            float HWBonusHealing = stats.SpellPower + stats.TotemHWSpellpower;
            //  ... * generic healing scale + bonus from TW
            HWBonusHealing *= 1.88f * (3.0f / 3.5f) + character.ShamanTalents.TidalWaves * .04f;
            //  HW healing scale = purification scale
            float HWHealingScale = PurificationScale;
            //  ... + 8%/16%/25% Healing Way bonus
            HWHealingScale *= 1 + character.ShamanTalents.HealingWay * .25f / 3f;
            //  ... + 5% 4pc T7 bonus
            HWHealingScale *= 1 + stats.CHHWHealIncrease;
            float HWHeal = (3250 + HWBonusHealing) * HWHealingScale;
            //  HW self-healing scale = 20% if w/Glyph (no longer benefits from Purification since patch 3.2)
            float HWSelfHealingScale = SelfHeal && character.ShamanTalents.GlyphofHealingWave ? 0.2f : 0;
            //      * correction due to the fact it's just not smart to use GoHW for self-healing if you're _really_ hammered down
            HWSelfHealingScale *= 1f / HWHealingScale;
            //  CH bonus healing = spell power + totem spell power bonus
            float CHBonusHealing = stats.SpellPower + stats.TotemCHBaseHeal;
            //  ... * generic healing scale
            CHBonusHealing *= 1.88f * (2.5f / 3.5f);
            //  CH healing scale = purification scale
            float CHHealingScale = PurificationScale;
            //  ... + 5/10/15% Improved Chain Heal
            CHHealingScale *= 1f + character.ShamanTalents.ImprovedChainHeal * .1f;
            //  ... + 5% 4pc T7 + HoT 4pc T10
            CHHealingScale *= 1f + stats.CHHWHealIncrease + .25f * CriticalChance * stats.RestoSham4T10;
            //  NOTE: stats.CHHealIncrease isn't handled since WotLK items don't have it
            float CHHeal = (1130 + CHBonusHealing) * CHHealingScale;
            float CHJumpHeal = 0;
            float scale = 1f;
            int jump;
            for (jump = 0; jump < CHJumps; jump++)
            {
                CHJumpHeal += scale;
                scale *= 0.6f;
            }
            CHJumpHeal += scale * (CHJumps - jump);
            CHJumpHeal *= CHHeal;
            //  HST bonus healing = spell power
            float HSTBonusHealing = stats.SpellPower;
            //      * generic healing scale * HoT scale
            HSTBonusHealing *= 1.88f * 0.044f;
            //  HST healing scale = purification scale
            float HSTHealingScale = PurificationScale;
            //      + 15%/30%/45% Restorative Totems + 20% w/Glyph
            HSTHealingScale *= 1 + character.ShamanTalents.RestorativeTotems * 0.15f + (character.ShamanTalents.GlyphofHealingStreamTotem ? 0.2f : 0f);
            #endregion
            #region Base Costs ( Preserve / RTCost / LHWCost / CHCost )
            float Preserve = stats.ManacostReduceWithin15OnHealingCast * .02f;
            float RTCost = ((float)Math.Round(BaseMana * .18f) - Preserve) * CostScale;
            float LHWCost = ((float)Math.Round(BaseMana * .15f) - Preserve) * CostScale;
            float HWCost = ((float)Math.Round(BaseMana * .25f) - Preserve - stats.TotemHWBaseCost) * CostScale;
            float DecurseCost = ((float)Math.Round(BaseMana * .07f) - Preserve) * CostScale;
            //  NOTE: stats.CHManaReduction isn't handled since WotLK items don't have it
            float CHCost = ((float)Math.Round(BaseMana * .19f) - Preserve - stats.TotemCHBaseCost) * CostScale;
            #endregion
            #region RT + LHW Rotation (RTLHWMPS / RTLHWHPS / RTLHWTime)  (Adjusted based on Casting Activity)
            if (character.ShamanTalents.Riptide != 0)
            {
                float RTLHWTime = RTCast;
                float RTLHWRemainingTime = RTCDCast - RTLHWTime;
                int RTLHWLHWCasts = 0;
                if (RTLHWRemainingTime > GcdLatency)
                {
                    RTLHWRemainingTime -= LHWCast_RT;
                    RTLHWTime += LHWCast_RT;
                    ++RTLHWLHWCasts;
                    if (RTLHWRemainingTime > GcdLatency)
                    {
                        int RTLHWLHWRemainingCasts = (int)Math.Ceiling((RTLHWRemainingTime - GcdLatency) / LHWCast);
                        RTLHWTime += RTLHWLHWRemainingCasts * LHWCast;
                        RTLHWLHWCasts += RTLHWLHWRemainingCasts;
                    }
                }
                float RTLHWLHWCrits = Math.Min(2, RTLHWLHWCasts) * Math.Min(CriticalChance + TWChance * 0.25f, 1) + (RTLHWLHWCasts - 2) * CriticalChance;
                float RTLHWRTHeal = RTHeal * Critical;
                float RTLHWLHWCritHeal = LHWHeal * Critical;
                float RTLHWLHWHeal = (LHWHeal * (RTLHWLHWCasts - RTLHWLHWCrits)) + (RTLHWLHWCrits * RTLHWLHWCritHeal);
                float RTLHWAA = (RTHeal * CriticalChance + LHWHeal * RTLHWLHWCrits) * AAScale;
                float RTTargets = TankHeal ? 1 : RTDuration / RTLHWTime;
                float RTLHWELWTargets = ELWChance * (TankHeal ? 1 : RTLHWLHWCasts * ELWDuration / RTLHWTime);
                calcStats.RTLHWHPS = (((RTLHWRTHeal * (1 - RTOverheal) + RTLHWLHWHeal * (1 - LHWOverheal) + RTLHWAA * (1 - AAOverheal)) / RTLHWTime + RTTargets * RTHotHPS * (1 - RTOverheal) + RTLHWELWTargets * ELWHPS * (1 - ELWOverheal)) * castingActivity) * (1 + stats.BonusHealingDoneMultiplier);
                calcStats.RTLHWMPS = ((RTCost + (LHWCost * RTLHWLHWCasts)) / RTLHWTime) * castingActivity;
                if (options.SustStyle.Equals("RT+LHW"))
                {
                    RTPerSec = 1f / RTLHWTime;
                    RTTicksPerSec = RTTargets / 3;
                    LHWPerSec = RTLHWLHWCasts / RTLHWTime;
                    RTCPerSec = RTPerSec * CriticalChance;
                    LHWCPerSec = RTLHWLHWCrits / RTLHWTime;
                    AAsPerSec += (CriticalChance + RTLHWLHWCrits) / RTLHWTime;
                    ELWTicksPerSec += RTLHWELWTargets;
                }
            }
            #endregion
            #region RT + HW Rotation (RTHWMPS / RTHWHPS / RTHWTime) (Adjusted based on Casting Activity)
            if (character.ShamanTalents.Riptide != 0)
            {
                float RTHWTime = RTCast;
                float RTHWRemainingTime = RTCDCast - RTHWTime;
                int RTHWHWCasts = 0;
                if (RTHWRemainingTime > GcdLatency)
                {
                    float RTHWHWCastTW_RT = HWCastTW_RT * TWChance + HWCast_RT * (1 - TWChance);
                    RTHWRemainingTime -= RTHWHWCastTW_RT;
                    RTHWTime += RTHWHWCastTW_RT;
                    ++RTHWHWCasts;
                    if (RTHWRemainingTime > GcdLatency)
                    {
                        float RTHWHWCastTW = HWCastTW * TWChance + HWCast * (1 - TWChance);
                        RTHWRemainingTime -= RTHWHWCastTW;
                        RTHWTime += RTHWHWCastTW;
                        ++RTHWHWCasts;
                        if (RTHWRemainingTime > GcdLatency)
                        {
                            int RTHWHWRemainingCasts = (int)Math.Ceiling((RTHWRemainingTime - GcdLatency) / HWCast);
                            RTHWTime += RTHWHWRemainingCasts * HWCast;
                            RTHWHWCasts += RTHWHWRemainingCasts;
                        }
                    }
                }
                float RTHWRTHeal = RTHeal * Critical;
                float RTHWHWHCrits = RTHWHWCasts * CriticalChance;
                float RTHWHWHeal = HWHeal * RTHWHWCasts * Critical;
                float RTHWHWSelfHeal = RTHWHWHeal * HWSelfHealingScale * Critical;
                float RTHWAA = (RTHeal * CriticalChance + HWHeal * RTHWHWHCrits) * AAScale;
                //  Multi-target ELW handling not in yet due to low priority
                float RTTargets = TankHeal ? 1 : RTDuration / RTHWTime;
                float RTHWELWTargets = ELWChance * (TankHeal ? 1 : RTHWHWCasts * ELWDuration / RTHWTime);
                calcStats.RTHWHPS = (((RTHWRTHeal * (1 - RTOverheal) + RTHWHWHeal * (1 - HWOverheal) + RTHWHWSelfHeal * (1 - HWSelfOverheal) + RTHWAA * (1 - AAOverheal)) / RTHWTime + RTTargets * RTHotHPS * (1 - RTOverheal) + RTHWELWTargets * ELWHPS * (1 - ELWOverheal)) * castingActivity) * (1 + stats.BonusHealingDoneMultiplier);
                calcStats.RTHWMPS = ((RTCost + (HWCost * RTHWHWCasts)) / RTHWTime) * castingActivity;
                if (options.SustStyle.Equals("RT+HW"))
                {
                    RTPerSec = 1f / RTHWTime;
                    RTTicksPerSec = RTTargets / 3;
                    HWPerSec = RTHWHWCasts / RTHWTime;
                    RTCPerSec = RTPerSec * CriticalChance;
                    HWCPerSec = RTHWHWHCrits / RTHWTime;
                    AAsPerSec += (CriticalChance + RTHWHWHCrits) / RTHWTime;
                    ELWTicksPerSec += RTHWELWTargets;
                }
            }
            #endregion
            #region RT + CH Rotation (RTCHMPS / RTCHHPS / RTCHTime / TankCH) (Adjusted based on Casting Activity)
            if (character.ShamanTalents.Riptide != 0)
            {
                float RTCHTime = RTCast;
                float RTCHRemainingTime = RTCDCast - RTCHTime;
                int RTCHCHCasts = 0;
                if (RTCHRemainingTime > GcdLatency)
                {
                    RTCHRemainingTime -= CHCast_RT;
                    RTCHTime += CHCast_RT;
                    ++RTCHCHCasts;
                    if (RTCHRemainingTime > GcdLatency)
                    {
                        int RTCHCHRemainingCasts = (int)Math.Ceiling((RTCHRemainingTime - GcdLatency) / CHCast);
                        RTCHTime += RTCHCHRemainingCasts * CHCast;
                        RTCHCHCasts += RTCHCHRemainingCasts;
                    }
                }
                float RTCHRTHeal = RTHeal * Critical;
                float RTCHCHHeal = (CHJumpHeal + CHHeal * CHRTConsumption * .25f) * RTCHCHCasts * ChCritical;
                float RTCHAA = RTHeal * CriticalChance * AAScale;
                float RTTargets = TankHeal ? Math.Max(RTDuration / RTCHTime - CHRTConsumption, 0) : (RTCast + (RTDuration - RTCast) * (1 - CHRTConsumption)) / RTCHTime;
                float RTCHELWTargets = ELWChance * (CHJumps * RTCHCHCasts + RTTargets) * ELWDuration / RTCHTime;
                calcStats.RTCHHPS = (((RTCHRTHeal * (1 - RTOverheal) + RTCHCHHeal * (1 - CHOverheal) + RTCHAA * (1 - AAOverheal)) / RTCHTime + RTTargets * RTHotHPS * (1 - RTOverheal) + RTCHELWTargets * ELWHPS * (1 - ELWOverheal)) * castingActivity) * (1 + stats.BonusHealingDoneMultiplier);
                calcStats.RTCHMPS = ((RTCost + (CHCost * RTCHCHCasts)) / RTCHTime) * castingActivity;
                if (options.SustStyle.Equals("RT+CH"))
                {
                    RTPerSec = 1f / RTCHTime;
                    RTCPerSec = RTPerSec * CriticalChance;
                    RTTicksPerSec = RTTargets / 3;
                    CHPerSec = RTCHCHCasts / RTCHTime;
                    CHHitsPerSec = CHPerSec * CHJumps;
                    CHCPerSec = RTCHCHCasts * (CriticalChance + (stats.RestoSham4T9 * .05f)) / RTCHTime;
                    CHCHitsPerSec = CHCPerSec * CHJumps;
                    AAsPerSec += CriticalChance / RTCHTime;
                    ELWTicksPerSec += RTCHELWTargets;
                }
            }
            #endregion
            #region CH Spam (CHHPS / CHMPS)
            float CHELWTargets = ELWChance * CHJumps * ELWDuration / CHCast;
            calcStats.CHSpamHPS = ((CHJumpHeal * ChCritical * (1 - CHOverheal) / CHCast + CHELWTargets * ELWHPS * (1 - ELWOverheal)) * castingActivity) * (1 + stats.BonusHealingDoneMultiplier);
            calcStats.CHSpamMPS = (CHCost / CHCast) * castingActivity;
            if (options.SustStyle.Equals("CH Spam"))
            {
                CHPerSec = 1f / CHCast;
                CHHitsPerSec = CHPerSec * CHJumps;
                CHCPerSec = (CriticalChance + (stats.RestoSham4T9 * .05f)) / CHCast;
                CHCHitsPerSec = CHCPerSec * CHJumps;
                ELWTicksPerSec += CHELWTargets;
            }
            #endregion
            #region LHW Spam (LHWHPS / LHWMPS)
            float LHWLHWHeal = LHWHeal * Critical;
            float LHWAA = LHWHeal * CriticalChance * AAScale;
            float LHWELWTargets = ELWChance * ELWDuration / LHWCast;
            calcStats.LHWSpamHPS = (((LHWLHWHeal * (1 - LHWOverheal) + LHWAA * (1 - AAOverheal)) / LHWCast + LHWELWTargets * ELWHPS * (1 - ELWOverheal)) * castingActivity) * (1 + stats.BonusHealingDoneMultiplier);
            calcStats.LHWSpamMPS = (LHWCost / LHWCast) * castingActivity;
            if (options.SustStyle.Equals("LHW Spam"))
            {
                LHWPerSec = 1f / LHWCast;
                LHWCPerSec = CriticalChance / LHWCast;
                ELWTicksPerSec += LHWELWTargets;
            }
            #endregion
            #region HW Spam (HWHPS / HWMPS)
            float HWHWHeal = HWHeal * Critical;
            float HWHWSelfHeal = HWHWHeal * HWSelfHealingScale * Critical;
            float HWAA = HWHeal * CriticalChance * AAScale;
            float HWELWTargets = ELWChance * ELWDuration / HWCast;
            calcStats.HWSpamHPS = (((HWHWHeal * (1 - HWOverheal) + HWHWSelfHeal * (1 - HWSelfOverheal) + HWAA * (1 - AAOverheal)) / HWCast + HWELWTargets * ELWHPS * (1 - ELWOverheal)) * castingActivity) * (1 + stats.BonusHealingDoneMultiplier);
            calcStats.HWSpamMPS = (HWCost / HWCast) * castingActivity;
            if (options.SustStyle.Equals("HW Spam"))
            {
                HWPerSec = 1f / HWCast;
                HWCPerSec = CriticalChance / HWCast;
                ELWTicksPerSec += HWELWTargets;
            }
            #endregion
            #region Variables if Riptide not taken in talents
            if (character.ShamanTalents.Riptide == 0)
            {
                calcStats.RTLHWHPS = calcStats.LHWSpamHPS;
                calcStats.RTLHWMPS = calcStats.LHWSpamMPS;
                calcStats.RTHWHPS = calcStats.HWSpamHPS;
                calcStats.RTHWMPS = calcStats.HWSpamMPS;
                calcStats.RTCHHPS = calcStats.CHSpamHPS;
                calcStats.RTCHMPS = calcStats.CHSpamMPS;
            }
            #endregion
            #region Create Final calcs via spell cast (HealPerSec/HealHitPerSec/CritPerSec)
            HealPerSec = (RTPerSec + LHWPerSec + HWPerSec + CHPerSec) * castingActivity;
            HealHitPerSec = (RTPerSec + RTTicksPerSec + LHWPerSec + HWPerSec + CHHitsPerSec + AAsPerSec + ELWTicksPerSec) * castingActivity;
            CritPerSec = (RTCPerSec + LHWCPerSec + HWCPerSec + CHCPerSec) * castingActivity;
            #endregion
            #region Proc Handling for Mana Restore only
            Stats statsProcs2 = new Stats();
            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                switch (effect.Trigger)
                {
                    case (Trigger.HealingSpellCast):
                        if (HealPerSec != 0)
                            effect.AccumulateAverageStats(statsProcs2, (1f / HealPerSec), 1f, 0f, FightSeconds);
                        break;
                    case (Trigger.HealingSpellHit):
                        if (HealHitPerSec != 0)
                            effect.AccumulateAverageStats(statsProcs2, (1f / HealHitPerSec), 1f, 0f, FightSeconds);
                        break;
                    case (Trigger.HealingSpellCrit):
                        if (CritPerSec != 0)
                            effect.AccumulateAverageStats(statsProcs2, (1f / CritPerSec), 1f, 0f, FightSeconds);
                        break;
                    case (Trigger.SpellCast):
                        if (HealPerSec != 0)
                            effect.AccumulateAverageStats(statsProcs2, (1f / HealPerSec), 1f, 0f, FightSeconds);
                        break;
                    case (Trigger.SpellHit):
                        if (HealHitPerSec != 0)
                            effect.AccumulateAverageStats(statsProcs2, (1f / HealHitPerSec), 1f, 0f, FightSeconds);
                        break;
                    case (Trigger.SpellCrit):
                        if (CritPerSec != 0)
                            effect.AccumulateAverageStats(statsProcs2, (1f / CritPerSec), 1f, 0f, FightSeconds);
                        break;
                    case Trigger.Use:
                        effect.AccumulateAverageStats(statsProcs2, 0f, 1f, 0f, FightSeconds);
                        break;
                }
            }
            #endregion
            #region Calculate Sequence HPS/MPS
            float HSTHPS = (25 + HSTBonusHealing) * HSTHealingScale / 2f * (1 - HSTOverheal);
            calcStats.HSTHeals = HSTHPS * HSTTargets;

            // Burst
            calcStats.BurstSequence = options.BurstStyle;
            switch (options.BurstStyle)
            {
                case "CH Spam":
                    calcStats.BurstHPS = calcStats.CHSpamHPS;
                    break;
                case "HW Spam":
                    calcStats.BurstHPS = calcStats.HWSpamHPS;
                    break;
                case "LHW Spam":
                    calcStats.BurstHPS = calcStats.LHWSpamHPS;
                    break;
                case "RT+HW":
                    calcStats.BurstHPS = calcStats.RTHWHPS;
                    break;
                case "RT+LHW":
                    calcStats.BurstHPS = calcStats.RTLHWHPS;
                    break;
                case "RT+CH":
                    calcStats.BurstHPS = calcStats.RTCHHPS;
                    break;
            }

            // Sustained
            calcStats.SustainedSequence = options.SustStyle;
            float SustHPS = 0;
            switch (options.SustStyle)
            {
                case "CH Spam":
                    SustHPS = calcStats.CHSpamHPS;
                    calcStats.MUPS = calcStats.CHSpamMPS;
                    break;
                case "HW Spam":
                    SustHPS = calcStats.HWSpamHPS;
                    calcStats.MUPS = calcStats.HWSpamMPS;
                    break;
                case "LHW Spam":
                    SustHPS = calcStats.LHWSpamHPS;
                    calcStats.MUPS = calcStats.LHWSpamMPS;
                    break;
                case "RT+HW":
                    SustHPS = calcStats.RTHWHPS;
                    calcStats.MUPS = calcStats.RTHWMPS;
                    break;
                case "RT+LHW":
                    SustHPS = calcStats.RTLHWHPS;
                    calcStats.MUPS = calcStats.RTLHWMPS;
                    break;
                case "RT+CH":
                    SustHPS = calcStats.RTCHHPS;
                    calcStats.MUPS = calcStats.RTCHMPS;
                    break;
            }

            calcStats.BurstHPS += calcStats.HSTHeals;
            SustHPS += calcStats.HSTHeals;
            calcStats.MUPS += (DecurseCost * options.Decurse) / FightSeconds;
            #endregion
            #region Final Stats
            float ESUsage = UseES ? (float)Math.Round((FightSeconds / ESTimer), 0) : 0;
            float ESDowntime = (FightSeconds - (RTCast * ESUsage) - 3) / FightSeconds;  // Rip tide cast time is used to simulate ES cast time, as they are exactly the same.  The 3 Simulates the time of two full totem drops.
            calcStats.MAPS = ((stats.Mana) / (FightSeconds))
                + (stats.ManaRestore / FightSeconds)
                + ((((((float)Math.Floor(options.FightLength / 5.025f) + 1) * ((stats.Mana * (1 + stats.BonusManaMultiplier)) * (.24f + ((character.ShamanTalents.GlyphofManaTideTotem ? 0.04f : 0)))))) * (options.ManaTideEveryCD && character.ShamanTalents.ManaTideTotem > 0 ? 1 : 0)) / FightSeconds)
                + ((stats.ManaRestoreFromMaxManaPerSecond * stats.Mana) * (options.ReplenishmentPercentage * .01f))
                + (stats.Mp5 / 5f)
                + (options.Innervates * 7866f / FightSeconds)
                + statsProcs2.ManaRestore
                + ((RTCPerSec * Orb) * castingActivity * ESDowntime)
                + ((LHWCPerSec * Orb * .6f) * castingActivity * ESDowntime)
                + ((HWCPerSec * Orb) * castingActivity * ESDowntime)
                + ((CHCHitsPerSec * Orb * .3f) * castingActivity * ESDowntime)
                - ESMPS;
            if (options.WSPops > 0)
                calcStats.MAPS += ((options.WSPops * Orb) / 60);
            calcStats.ManaUsed = calcStats.MAPS * FightSeconds;
            float MAPSConvert = (float)Math.Min((calcStats.MAPS / ((calcStats.MUPS * castingActivity) * ESDowntime)), 1);
            //  FIXME: some Healed effects can crit and some are not affected by Purification, this should be taken into account
            float HealedHPS = (stats.Healed * PurificationScale) * (1 + stats.BonusHealingDoneMultiplier);
            calcStats.BurstHPS = (calcStats.BurstHPS * ESDowntime) + calcStats.ESHPS * (1 - ESOverheal) + HealedHPS;
            calcStats.SustainedHPS = (SustHPS * MAPSConvert) + calcStats.ESHPS * (1 - ESOverheal) + HealedHPS;
            calcStats.Survival = (calcStats.BasicStats.Health + calcStats.BasicStats.Hp5) * (options.SurvivalPerc * .01f);
            calcStats.OverallPoints = calcStats.BurstHPS + calcStats.SustainedHPS + calcStats.Survival;
            calcStats.SubPoints[0] = calcStats.BurstHPS;
            calcStats.SubPoints[1] = calcStats.SustainedHPS;
            calcStats.SubPoints[2] = calcStats.Survival;

            return calcStats;
            #endregion
        }
        #endregion
        #region Character Stats and other Final Stats
        public override Stats GetCharacterStats(Character character, Item additionalItem)
        {
            return GetCharacterStats(character, additionalItem, null);
        }
        public Stats GetCharacterStats(Character character, Item additionalItem, Stats statModifier)
        {
            CalculationOptionsRestoSham calcOpts = character.CalculationOptions as CalculationOptionsRestoSham;
            Stats statsRace = GetRaceStats(character.Race);
            #region Other Final Stats
            Stats statsBaseGear = GetItemStats(character, additionalItem);
            Stats statsBuffs = GetBuffsStats(character);
            Stats statsTotal = statsBaseGear + statsBuffs + statsRace;
            if (statModifier != null)
                statsTotal += statModifier;
            statsTotal.Accumulate(GetProcStats(statsTotal.SpecialEffects()));
            statsTotal.Stamina = (float)Math.Round((statsTotal.Stamina) * (1 + statsTotal.BonusStaminaMultiplier));

            float IntMultiplier = (1f + statsTotal.BonusIntellectMultiplier) * (1f + character.ShamanTalents.AncestralKnowledge * 2f / 100);
            if (IntMultiplier > 1)
            {
                statsTotal.Intellect = (float)Math.Floor((statsRace.Intellect) * IntMultiplier) +
                    (float)Math.Floor((statsBaseGear.Intellect + statsBuffs.Intellect/* + statsEnchants.Intellect*/) * IntMultiplier);
                if (statModifier != null && statModifier.Intellect > 0)
                    statsTotal.Intellect += (float)Math.Floor((statModifier.Intellect) * IntMultiplier);
            }

            statsTotal.SpellPower = (float)Math.Floor(statsTotal.SpellPower) + (float)Math.Floor(statsTotal.Intellect * 0.05f * character.ShamanTalents.NaturesBlessing);
            statsTotal.Mana = statsTotal.Mana + 20 + ((statsTotal.Intellect - 20) * 15);
            statsTotal.Health = (statsTotal.Health + 20 + ((statsTotal.Stamina - 20) * 10f)) * (1f + statsTotal.BonusHealthMultiplier);
            #endregion
            return statsTotal;
        }
        #region Basic Stats Methods
        
        /// <summary>
        /// Filters a Stats object to just the stats relevant to the model.
        /// </summary>
        /// <param name="stats">A complete Stats object containing all stats.</param>
        /// <returns>
        /// A filtered Stats object containing only the stats relevant to the model.
        /// </returns>
        public override Stats GetRelevantStats(Stats stats)
        {
            Stats relevantStats = new Stats();
            Type statsType = typeof(Stats);

            foreach (string relevantStat in Relevants.RelevantStats)
            {
                float v = (float)statsType.GetProperty(relevantStat).GetValue(stats, null);
                if (v > 0)
                {
                    statsType.GetProperty(relevantStat).SetValue(relevantStats, v, null);
                }
            }

            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if (effect.Trigger == Trigger.HealingSpellCast ||
                    effect.Trigger == Trigger.HealingSpellCrit ||
                    effect.Trigger == Trigger.HealingSpellHit ||
                    effect.Trigger == Trigger.SpellCast ||
                    effect.Trigger == Trigger.SpellCrit ||
                    effect.Trigger == Trigger.SpellHit ||
                    effect.Trigger == Trigger.Use)
                {
                    if (HasRelevantStats(effect.Stats))
                        relevantStats.AddSpecialEffect(effect);
                }
            }

            return relevantStats;
        }
        
        /// <summary>
        /// Tests whether there are positive relevant stats in the Stats object.
        /// </summary>
        /// <param name="stats">The complete Stats object containing all stats.</param>
        /// <returns>
        /// True if any of the positive stats in the Stats are relevant.
        /// </returns>
        public override bool HasRelevantStats(Stats stats)
        {
            // Accumulate the "base" stats with the special effect stats.
            Stats comparison = new Stats();
            comparison.Accumulate(stats);
            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                comparison.Accumulate(effect.Stats);
            }

            float statTotal = 0f;

            // Loop over each relevant stat and get its value
            Type statsType = typeof(Stats);
            foreach (string relevantStat in Relevants.RelevantStats)
            {
                float v = (float)statsType.GetProperty(relevantStat).GetValue(comparison, null);
                if (v > 0)
                    statTotal += v;
            }

            // if statTotal > 0 then we have relevant stats
            return statTotal > 0;
        }
        
        /// <summary>
        /// Gets the stats of the buffs currently active on a character
        /// </summary>
        /// <param name="character">The character to evaluate</param>
        /// <returns>Stats object containing the stats of all the active buffs on the character</returns>
        private Stats GetBuffsStats(Character character)
        {
            return GetBuffsStats(character.ActiveBuffs);
        }
        
        /// <summary>
        /// Gets the stats of procs or special effects
        /// </summary>
        /// <param name="specialEffects">List of special effects to aggregate</param>
        /// <returns>Stats object containing the accumulated stats from the provided special effects</returns>
        private Stats GetProcStats(Stats.SpecialEffectEnumerator specialEffects)
        {
            Stats statsProcs = new Stats();
            foreach (SpecialEffect effect in specialEffects)
            {
                statsProcs.Accumulate(GetProcStats_Inner(effect));
            }
            return statsProcs;
        }

        private Stats GetProcStats_Inner(SpecialEffect effect) {
            Stats procStats = new Stats();
            switch (effect.Trigger)
            {
                case (Trigger.HealingSpellCast):
                    if (HealPerSec != 0)
                        procStats = effect.GetAverageStats((1f / HealPerSec), 1f, 0f, FightSeconds);
                    break;
                case (Trigger.HealingSpellHit):
                    if (HealHitPerSec != 0)
                        procStats = effect.GetAverageStats((1f / HealHitPerSec), 1f, 0f, FightSeconds);
                    break;
                case (Trigger.HealingSpellCrit):
                    if (CritPerSec != 0)
                        procStats = effect.GetAverageStats((1f / CritPerSec), 1f, 0f, FightSeconds);
                    break;
                case (Trigger.SpellCast):
                    if (HealPerSec != 0)
                        procStats = effect.GetAverageStats((1f / HealPerSec), 1f, 0f, FightSeconds);
                    break;
                case (Trigger.SpellHit):
                    if (HealHitPerSec != 0)
                        procStats = effect.GetAverageStats((1f / HealHitPerSec), 1f, 0f, FightSeconds);
                    break;
                case (Trigger.SpellCrit):
                    if (CritPerSec != 0)
                        procStats = effect.GetAverageStats((1f / CritPerSec), 1f, 0f, FightSeconds);
                    break;
                case Trigger.Use:
                    if (effect.Stats._rawSpecialEffectData != null) {
                        // Handles Recursive Effects
                        Stats SubStats = GetProcStats_Inner(effect.Stats._rawSpecialEffectData[0]);
                        float upTime = effect.GetAverageUptime(0f, 1f, 0f, FightSeconds);
                        procStats.Accumulate(SubStats,upTime);
                    } else {
                        procStats = effect.GetAverageStats(0f, 1f, 0f, FightSeconds);
                    }
                    break;
            }
            return procStats;
        }
        
        /// <summary>
        /// Create the base stats for a given character based on their race.
        /// </summary>
        /// <param name="race">The race of the character</param>
        /// <returns>Stats object containing the base race stats</returns>
        private static Stats GetRaceStats(CharacterRace race)
        {
            #region Create the statistics for a given character
            Stats statsRace;
            switch (race)
            {
                case CharacterRace.Draenei:
                    statsRace = new Stats() { Health = 6485, Mana = BaseMana, Stamina = 135, Intellect = 141, Spirit = 145 };
                    break;

                case CharacterRace.Tauren:
                    statsRace = new Stats() { Health = 6485, Mana = BaseMana, Stamina = 138, Intellect = 135, Spirit = 145 };
                    statsRace.BonusHealthMultiplier = 0.05f;
                    break;

                case CharacterRace.Orc:
                    statsRace = new Stats() { Health = 6485, Mana = BaseMana, Stamina = 138, Intellect = 137, Spirit = 146 };
                    break;

                case CharacterRace.Troll:
                    statsRace = new Stats() { Health = 6485, Mana = BaseMana, Stamina = 137, Intellect = 124, Spirit = 144 };
                    break;

                default:
                    statsRace = new Stats();
                    break;
            }
            return statsRace;
            #endregion
        }
        #endregion
        #endregion

        #region Chart data area
        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
        {
#if !RAWR3
            ChartCalculator chartCalc = CustomCharts.GetChartCalculator(chartName);
            if (chartCalc == null)
                return new ComparisonCalculationBase[0];

            ICollection<ComparisonCalculationBase> list = chartCalc(character, this);
            ComparisonCalculationBase[] retVal = new ComparisonCalculationBase[list.Count];
            if (list.Count > 0)
                list.CopyTo(retVal, 0);
            return retVal;
#else
			return new ComparisonCalculationBase[0];
#endif
        }
#if !RAWR3
        public override void RenderCustomChart(Character character, string chartName, Graphics g, int width, int height)
        {
            string calc = chartName.Substring(0, chartName.IndexOf("Stats Graph") - 1);
            Base.Graph.RenderStatsGraph(g, width, height, character,
                CustomCharts.StatsGraphStatsList, CustomCharts.StatsGraphColors,
                200, "", calc, Base.Graph.Style.DpsWarr);
        }
#endif

        #endregion

        #region Retrieve our options from XML:
        public override ICalculationOptionBase DeserializeDataObject(string xml)
        {
            System.Xml.Serialization.XmlSerializer serializer =
                            new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsRestoSham));
            System.IO.StringReader reader = new System.IO.StringReader(xml);
            CalculationOptionsRestoSham calcOpts = serializer.Deserialize(reader) as CalculationOptionsRestoSham;
            return calcOpts;
        }
        #endregion
    }
}
