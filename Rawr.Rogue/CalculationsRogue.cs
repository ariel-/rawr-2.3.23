using System;
using System.Collections.Generic;
#if RAWR3
using System.Windows.Media;
#else
using System.Drawing;
#endif
using System.Text;

namespace Rawr.Rogue
{
    [Rawr.Calculations.RawrModelInfo("Rogue", "Ability_Rogue_SliceDice", CharacterClass.Rogue)]
    public class CalculationsRogue : CalculationsBase
    {
        #region Variables and Properties

        public override List<GemmingTemplate> DefaultGemmingTemplates {
            get {
                // Relevant Gem IDs for Rogues
                //Red
                int[] delicate = { 39905, 39997, 40112, 42143 }; // Agi
                int[] fractured = { 39909, 40002, 40117, 42153 }; // ArP
                int[] precise = { 39910, 40003, 40118, 42154 }; // Exp
                int[] bright = { 39906, 39999, 40114, 36766 }; // AP

                //Purple
                int[] shifting = { 39935, 40023, 40130, 40130 }; // Agi/Sta
                int[] puissant = { 39933, 40033, 40140, 40140 }; // ArP/Sta
                int[] balanced = { 39937, 40029, 40136, 40136 }; // AP/Sta
                int[] guardian = { 39940, 40034, 40141, 40141 }; // Exp/Sta

                //Blue
                int[] solid = { 39919, 40008, 40119, 36767 }; // Sta

                //Green
                int[] forceful = { 39978, 40091, 40169, 40169 }; // Haste/Sta
                int[] vivid = { 39975, 40088, 40166, 40166 }; // Hit/Sta

                //Yellow
                int[] quick = { 39918, 40017, 40128, 42150 }; // Haste
                int[] rigid = { 39915, 40014, 40125, 40156 }; // Hit

                //Orange
                int[] glinting = { 39953, 40044, 40148, 40148 }; // Agi/Hit
                int[] deadly = { 39952, 40043, 40147, 40147 }; // Agi/Crit
                int[] deft = { 39955, 40046, 40150, 40150 }; // Agi/Haste
                int[] pristine = { 39961, 40053, 40157, 40157 }; // AP/Hit
                int[] wicked = { 39960, 40052, 40156, 40156 }; // AP/Crit
                int[] stark = { 39963, 40055, 40159, 40159 }; // AP/Haste
                int[] accurate = { 39966, 40058, 40162, 40162 }; // Exp/Hit

                // Prismatic
                int[] nightmare = { 49110, 49110, 49110, 49110 }; // All stats

                //Meta
                int relentless = 41398; // Agi/Crit dmg

                return new List<GemmingTemplate>()
				{
					new GemmingTemplate() { Model = "Rogue", Group = "Uncommon", //Max ArP
						RedId = fractured[0], YellowId = fractured[0], BlueId = fractured[0], PrismaticId = fractured[0], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Uncommon", //ArP/Crit
						RedId = fractured[0], YellowId = quick[0], BlueId = puissant[0], PrismaticId = fractured[0], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Uncommon", //ArP/Haste
						RedId = fractured[0], YellowId = quick[0], BlueId = forceful[0], PrismaticId = fractured[0], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Uncommon", //Max Agility
						RedId = delicate[0], YellowId = delicate[0], BlueId = delicate[0], PrismaticId = delicate[0], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Uncommon", //Agi/Crit
						RedId = delicate[0], YellowId = deadly[0], BlueId = shifting[0], PrismaticId = delicate[0], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Uncommon", //Agi/Haste
						RedId = delicate[0], YellowId = quick[0], BlueId = forceful[0], PrismaticId = delicate[0], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Uncommon", //Max Attack Power
						RedId = bright[0], YellowId = bright[0], BlueId = bright[0], PrismaticId = bright[0], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Uncommon", //AP/Crit
						RedId = bright[0], YellowId = wicked[0], BlueId = balanced[0], PrismaticId = bright[0], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Uncommon", //AP/Haste
						RedId = bright[0], YellowId = quick[0], BlueId = forceful[0], PrismaticId = bright[0], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Uncommon", //Max Expertise
						RedId = precise[0], YellowId = precise[0], BlueId = precise[0], PrismaticId = precise[0], MetaId = relentless },

					new GemmingTemplate() { Model = "Rogue", Group = "Uncommon", //Agi/Hit
					    RedId = delicate[0], YellowId = glinting[0], BlueId = shifting[0], PrismaticId = delicate[0], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Uncommon", //AP/Hit
					    RedId = bright[0], YellowId = pristine[0], BlueId = balanced[0], PrismaticId = bright[0], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Uncommon", //Exp+/Hit
					    RedId = precise[0], YellowId = accurate[0], BlueId = guardian[0], PrismaticId = precise[0], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Uncommon", //Exp/Hit+
					    RedId = accurate[0], YellowId = rigid[0], BlueId = vivid[0], PrismaticId = rigid[0], MetaId = relentless },

					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //Max ArP
						RedId = fractured[1], YellowId = fractured[1], BlueId = fractured[1], PrismaticId = fractured[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //ArP/Crit
						RedId = fractured[1], YellowId = quick[1], BlueId = puissant[1], PrismaticId = fractured[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //ArP/Haste
						RedId = fractured[1], YellowId = quick[1], BlueId = forceful[1], PrismaticId = fractured[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //Max Agility
						RedId = delicate[1], YellowId = delicate[1], BlueId = delicate[1], PrismaticId = delicate[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //Agi/Crit 
						RedId = delicate[1], YellowId = deadly[1], BlueId = shifting[1], PrismaticId = delicate[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //Agi/Haste 
						RedId = delicate[1], YellowId = quick[1], BlueId = forceful[1], PrismaticId = delicate[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //Max Attack Power
						RedId = bright[1], YellowId = bright[1], BlueId = bright[1], PrismaticId = bright[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //AP/Crit 
						RedId = bright[1], YellowId = wicked[1], BlueId = balanced[1], PrismaticId = bright[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //AP/Haste 
						RedId = bright[1], YellowId = quick[1], BlueId = forceful[1], PrismaticId = bright[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //Max Expertise
						RedId = precise[1], YellowId = precise[1], BlueId = precise[1], PrismaticId = precise[1], MetaId = relentless },

					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //Max ArP
						RedId = fractured[1], YellowId = fractured[1], BlueId = nightmare[1], PrismaticId = fractured[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //ArP/Crit
						RedId = fractured[1], YellowId = quick[1], BlueId = nightmare[1], PrismaticId = fractured[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //ArP/Haste
						RedId = fractured[1], YellowId = quick[1], BlueId = nightmare[1], PrismaticId = fractured[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //Max Agility
						RedId = delicate[1], YellowId = delicate[1], BlueId = nightmare[1], PrismaticId = delicate[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //Agi/Crit 
						RedId = delicate[1], YellowId = deadly[1], BlueId = nightmare[1], PrismaticId = delicate[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //Agi/Haste 
						RedId = delicate[1], YellowId = quick[1], BlueId = nightmare[1], PrismaticId = delicate[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //Max Attack Power
						RedId = bright[1], YellowId = bright[1], BlueId = nightmare[1], PrismaticId = bright[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //AP/Crit 
						RedId = bright[1], YellowId = wicked[1], BlueId = nightmare[1], PrismaticId = bright[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //AP/Haste 
						RedId = bright[1], YellowId = quick[1], BlueId = nightmare[1], PrismaticId = bright[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //Max Expertise
						RedId = precise[1], YellowId = precise[1], BlueId = nightmare[1], PrismaticId = precise[1], MetaId = relentless },

					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //Agi/Hit
					    RedId = delicate[1], YellowId = glinting[1], BlueId = shifting[1], PrismaticId = delicate[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //AP/Hit
					    RedId = bright[1], YellowId = pristine[1], BlueId = balanced[1], PrismaticId = bright[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //Exp+/Hit
					    RedId = precise[1], YellowId = accurate[1], BlueId = guardian[1], PrismaticId = precise[1], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Rare", //Exp/Hit+
					    RedId = accurate[1], YellowId = rigid[1], BlueId = vivid[1], PrismaticId = rigid[1], MetaId = relentless },
						
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //Max ArP
						RedId = fractured[2], YellowId = fractured[2], BlueId = fractured[2], PrismaticId = fractured[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //ArP/Crit
						RedId = fractured[2], YellowId = quick[2], BlueId = puissant[2], PrismaticId = fractured[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //ArP/Haste
						RedId = fractured[2], YellowId = quick[2], BlueId = forceful[2], PrismaticId = fractured[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //Max Agility
						RedId = delicate[2], YellowId = delicate[2], BlueId = delicate[2], PrismaticId = delicate[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //Agi/Crit 
						RedId = delicate[2], YellowId = deadly[2], BlueId = shifting[2], PrismaticId = delicate[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //Agi/Haste 
						RedId = delicate[2], YellowId = quick[2], BlueId = forceful[2], PrismaticId = delicate[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //Max Attack Power
						RedId = bright[2], YellowId = bright[2], BlueId = bright[2], PrismaticId = bright[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //AP/Crit 
						RedId = bright[2], YellowId = wicked[2], BlueId = balanced[2], PrismaticId = bright[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //AP/Haste 
						RedId = bright[2], YellowId = quick[2], BlueId = forceful[2], PrismaticId = bright[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //Max Expertise
						RedId = precise[2], YellowId = precise[2], BlueId = precise[2], PrismaticId = precise[2], MetaId = relentless },

					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //Max ArP
						RedId = fractured[2], YellowId = fractured[2], BlueId = nightmare[2], PrismaticId = fractured[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //ArP/Crit
						RedId = fractured[2], YellowId = quick[2], BlueId = nightmare[2], PrismaticId = fractured[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //ArP/Haste
						RedId = fractured[2], YellowId = quick[2], BlueId = nightmare[2], PrismaticId = fractured[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //Max Agility
						RedId = delicate[2], YellowId = delicate[2], BlueId = nightmare[2], PrismaticId = delicate[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //Agi/Crit 
						RedId = delicate[2], YellowId = deadly[2], BlueId = nightmare[2], PrismaticId = delicate[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //Agi/Haste 
						RedId = delicate[2], YellowId = quick[2], BlueId = nightmare[2], PrismaticId = delicate[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //Max Attack Power
						RedId = bright[2], YellowId = bright[2], BlueId = nightmare[2], PrismaticId = bright[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //AP/Crit 
						RedId = bright[2], YellowId = wicked[2], BlueId = nightmare[2], PrismaticId = bright[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //AP/Haste 
						RedId = bright[2], YellowId = quick[2], BlueId = nightmare[2], PrismaticId = bright[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //Max Expertise
						RedId = precise[2], YellowId = precise[2], BlueId = nightmare[2], PrismaticId = precise[2], MetaId = relentless },

					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //Agi/Hit
					    RedId = delicate[2], YellowId = glinting[2], BlueId = shifting[2], PrismaticId = delicate[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //AP/Hit
					    RedId = bright[2], YellowId = pristine[2], BlueId = balanced[2], PrismaticId = bright[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //Exp+/Hit
					    RedId = precise[2], YellowId = accurate[2], BlueId = guardian[2], PrismaticId = precise[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Epic", Enabled = true, //Exp/Hit+
					    RedId = accurate[2], YellowId = rigid[2], BlueId = vivid[2], PrismaticId = rigid[2], MetaId = relentless },
						
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Max ArP
						RedId = fractured[3], YellowId = fractured[3], BlueId = fractured[3], PrismaticId = fractured[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //ArP/Crit
						RedId = fractured[3], YellowId = quick[3], BlueId = puissant[2], PrismaticId = fractured[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //ArP/Haste
						RedId = fractured[3], YellowId = quick[3], BlueId = forceful[2], PrismaticId = fractured[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Max Agility
						RedId = delicate[3], YellowId = delicate[3], BlueId = delicate[3], PrismaticId = delicate[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Agi/Crit
						RedId = delicate[3], YellowId = quick[3], BlueId = delicate[3], PrismaticId = delicate[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Agi/Haste
						RedId = delicate[3], YellowId = quick[3], BlueId = forceful[3], PrismaticId = delicate[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Max Attack Power
						RedId = bright[3], YellowId = bright[3], BlueId = bright[3], PrismaticId = bright[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //AP/Crit
						RedId = bright[3], YellowId = quick[3], BlueId = balanced[3], PrismaticId = bright[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //AP/Haste
						RedId = bright[3], YellowId = quick[3], BlueId = forceful[3], PrismaticId = bright[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Max Expertise
						RedId = precise[3], YellowId = precise[3], BlueId = precise[3], PrismaticId = precise[3], MetaId = relentless },

					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Max ArP
						RedId = fractured[3], YellowId = fractured[3], BlueId = nightmare[3], PrismaticId = fractured[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //ArP/Crit
						RedId = fractured[3], YellowId = quick[3], BlueId = nightmare[3], PrismaticId = fractured[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //ArP/Haste
						RedId = fractured[3], YellowId = quick[3], BlueId = nightmare[3], PrismaticId = fractured[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Max Agility
						RedId = delicate[3], YellowId = delicate[3], BlueId = nightmare[3], PrismaticId = delicate[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Agi/Crit
						RedId = delicate[3], YellowId = quick[3], BlueId = nightmare[3], PrismaticId = delicate[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Agi/Haste
						RedId = delicate[3], YellowId = quick[3], BlueId = nightmare[3], PrismaticId = delicate[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Max Attack Power
						RedId = bright[3], YellowId = bright[3], BlueId = nightmare[3], PrismaticId = bright[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //AP/Crit
						RedId = bright[3], YellowId = quick[3], BlueId = nightmare[3], PrismaticId = bright[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //AP/Haste
						RedId = bright[3], YellowId = quick[3], BlueId = nightmare[3], PrismaticId = bright[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Max Expertise
						RedId = precise[3], YellowId = precise[3], BlueId = nightmare[3], PrismaticId = precise[3], MetaId = relentless },

					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Agility Heavy
						RedId = delicate[2], YellowId = delicate[3], BlueId = delicate[3], PrismaticId = delicate[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Attack Power Heavy
						RedId = bright[2], YellowId = bright[3], BlueId = bright[3], PrismaticId = bright[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Expertise Heavy
						RedId = precise[2], YellowId = precise[3], BlueId = precise[3], PrismaticId = precise[2], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Exp+/Hit
					    RedId = precise[3], YellowId = accurate[3], BlueId = guardian[3], PrismaticId = precise[3], MetaId = relentless },
					new GemmingTemplate() { Model = "Rogue", Group = "Jeweler", //Exp/Hit+
					    RedId = accurate[3], YellowId = rigid[3], BlueId = vivid[3], PrismaticId = rigid[3], MetaId = relentless },
			    };
            }
        }

        private CalculationOptionsPanelRogue _calculationOptionsPanel = null;
        #if RAWR3
        public override ICalculationOptionsPanel CalculationOptionsPanel
        #else
        public override CalculationOptionsPanelBase CalculationOptionsPanel
        #endif
        {
            get {
                if (_calculationOptionsPanel == null) { _calculationOptionsPanel = new CalculationOptionsPanelRogue(); }
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
					"Summary:Overall Points*Sum of your DPS Points and Survivability Points.",
					"Summary:DPS Points*DPS Points is your theoretical DPS.",
					"Summary:Survivability Points*One hundreth of your health.",

					"Basic Stats:Health",
					"Basic Stats:Attack Power",
					"Basic Stats:Agility",
					"Basic Stats:Strength",
					"Basic Stats:Crit Rating",
					"Basic Stats:Hit Rating",
					"Basic Stats:Expertise Rating",
					"Basic Stats:Haste Rating",
					"Basic Stats:Armor Penetration Rating",
					"Basic Stats:Weapon Damage",
					
					"Complex Stats:Avoided White Attacks",
					"Complex Stats:Avoided Yellow Attacks",
					"Complex Stats:Avoided Poison Attacks",
					"Complex Stats:Crit Chance",
					"Complex Stats:MainHand Speed",
					"Complex Stats:OffHand Speed",
					"Complex Stats:Armor Mitigation MainHand",
					"Complex Stats:Armor Mitigation OffHand",
					
					"Abilities:Optimal Rotation",
					"Abilities:Optimal Rotation DPS",
					"Abilities:Custom Rotation DPS",
					"Abilities:MainHand",
					"Abilities:OffHand",
					"Abilities:Backstab",
					"Abilities:Hemorrhage",
					"Abilities:Sinister Strike",
					"Abilities:Mutilate",
					"Abilities:Slice and Dice",
					"Abilities:Rupture",
					"Abilities:Eviscerate",
					"Abilities:Envenom",
                    "Abilities:Instant Poison",
                    "Abilities:Deadly Poison",
                    "Abilities:Wound Poison",
                    "Abilities:Anesthetic Poison",
				};
                return _characterDisplayCalculationLabels;
            }
        }

        private string[] _optimizableCalculationLabels = null;
        public override string[] OptimizableCalculationLabels {
            get {
                if (_optimizableCalculationLabels == null)
                    _optimizableCalculationLabels = new string[] {
                        "Health",
                        "Avoided Yellow Attacks %",
                        "Avoided Poison Attacks %",
                        "Avoided White Attacks %",
                        "Custom Rotation DPS",
					};
                return _optimizableCalculationLabels;
            }
        }

        private string[] _customChartNames = null;
        public override string[] CustomChartNames {
            get {
                if (_customChartNames == null) {
                    _customChartNames = new string[] {
                        "Combat Table",
                    };
                }
                return _customChartNames;
            }
        }

        private Dictionary<string, Color> _subPointNameColors = null;
        public override Dictionary<string, Color> SubPointNameColors
        {
            get
            {
                if (_subPointNameColors == null)
                {
                    _subPointNameColors = new Dictionary<string, Color>();
                    _subPointNameColors.Add("DPS", Color.FromArgb(255, 255, 0, 0));
                    _subPointNameColors.Add("Survivability", Color.FromArgb(255, 64, 128, 32));
                }
                return _subPointNameColors;
            }
        }

        private List<ItemType> _relevantItemTypes = null;
        public override List<ItemType> RelevantItemTypes {
            get {
                if (_relevantItemTypes == null) {
                    _relevantItemTypes = new List<ItemType>(new[] {
                        ItemType.None,
                        ItemType.Leather,
                        ItemType.Bow,
                        ItemType.Crossbow,
                        ItemType.Gun,
                        ItemType.Thrown,
                        ItemType.Dagger,
                        ItemType.FistWeapon,
                        ItemType.OneHandAxe,
                        ItemType.OneHandMace,
                        ItemType.OneHandSword
                    });
                }
                return _relevantItemTypes;
            }
        }

        public override CharacterClass TargetClass { get { return CharacterClass.Rogue; } }
        public override ComparisonCalculationBase CreateNewComparisonCalculation() { return new ComparisonCalculationsRogue(); }
        public override CharacterCalculationsBase CreateNewCharacterCalculations() { return new CharacterCalculationsRogue(); }
        public bool PTRMode = false;
        
        public override ICalculationOptionBase DeserializeDataObject(string xml) {
            System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsRogue));
            System.IO.StringReader sr = new System.IO.StringReader(xml);
            CalculationOptionsRogue calcOpts = s.Deserialize(sr) as CalculationOptionsRogue;
            return calcOpts;
        }
        #endregion

        public override CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, bool referenceCalculation, bool significantChange, bool needsDisplayCalculations)
        {
            CalculationOptionsRogue calcOpts = character.CalculationOptions as CalculationOptionsRogue;
            if (calcOpts == null) calcOpts = new CalculationOptionsRogue();
            PTRMode = calcOpts.PTRMode;
            int targetLevel = calcOpts.TargetLevel;
            float targetArmor = calcOpts.TargetArmor;
            bool targetPoisonable = calcOpts.TargetPoisonable;
            bool maintainBleed = false;
            WeightedStat[] arPenUptimes, critRatingUptimes;
            Stats stats = GetCharacterStatsWithTemporaryEffects(character, additionalItem, out arPenUptimes, out critRatingUptimes);
            float levelDifference = (targetLevel - 80f) * 0.2f;
            CharacterCalculationsRogue calculatedStats = new CharacterCalculationsRogue();
            calculatedStats.BasicStats = stats;
            calculatedStats.TargetLevel = targetLevel;
            Item mainHand = character.MainHand == null ? new Knuckles() : character.MainHand.Item;
            Item offHand = character.OffHand == null ? new Knuckles() : character.OffHand.Item;

            #region Basic Chances and Constants
            #region Constants from talents
            float ambushBackstabCostReduction = 4 * character.RogueTalents.SlaughterFromTheShadows;
            float armorReduction = 0.03f * character.RogueTalents.SerratedBlades;
            float bonusBackstabCrit = 0.1f * character.RogueTalents.PuncturingWounds;
            float bonusBackstabDamageMultiplier = 0.03f * character.RogueTalents.Aggression + 0.05f * character.RogueTalents.BladeTwisting + 0.1f * character.RogueTalents.SurpriseAttacks + 0.1f * character.RogueTalents.Opportunity + 0.02f * character.RogueTalents.SinisterCalling;
            float bonusCPGCritDamageMultiplier = 0.06f * character.RogueTalents.Lethality;
            float bonusCPGCrit = 0.02f * character.RogueTalents.TurnTheTables;
            float bonusEnvenomDamageMultiplier = 0.07f * character.RogueTalents.VilePoisons;
            float bonusEvisCrit = character.RogueTalents.GlyphOfEviscerate ? 0.1f : 0f;
            float bonusEvisDamageMultiplier = (character.RogueTalents.ImprovedEviscerate == 3 ? 0.2f : 0.07f * character.RogueTalents.ImprovedEviscerate) + 0.03f * character.RogueTalents.Aggression;
            float bonusFlurryHaste = 0.2f * character.RogueTalents.BladeFlurry;
            float bonusHemoDamageMultiplier = 0.1f * character.RogueTalents.SurpriseAttacks + 0.02f * character.RogueTalents.SinisterCalling;
            float bonusMaceArP = 0.03f * character.RogueTalents.MaceSpecialization;
            float bonusMainHandCrit = (character.MainHand != null) ? ((character.MainHand.Type == ItemType.Dagger || character.MainHand.Type == ItemType.FistWeapon) ? 0.01f * character.RogueTalents.CloseQuartersCombat : 0f) : 0f;
            float bonusMutiCrit = 0.05f * character.RogueTalents.PuncturingWounds;
            float bonusMutiDamageMultiplier = 0.1f * character.RogueTalents.Opportunity;
            float bonusOffHandCrit = (character.OffHand != null) ? ((character.OffHand.Type == ItemType.Dagger || character.OffHand.Type == ItemType.FistWeapon) ? 0.01f * character.RogueTalents.CloseQuartersCombat : 0f) : 0f;
            float bonusOffHandDamageMultiplier = 0.1f * character.RogueTalents.DualWieldSpecialization;
            float bonusPoisonDamageMultiplier = character.RogueTalents.VilePoisons == 3 ? 0.2f : 0.07f * character.RogueTalents.VilePoisons;
            float bonusRuptDamageMultiplier = 0.15f * character.RogueTalents.BloodSpatter + 0.1f * character.RogueTalents.SerratedBlades;
            float bonusRuptDuration = character.RogueTalents.GlyphOfRupture ? 4 : 0;
            float bonusSnDDuration = character.RogueTalents.GlyphOfSliceandDice ? 3 : 0;
            float bonusSnDDurationMultiplier = 0.25f * character.RogueTalents.ImprovedSliceAndDice;
            float bonusSStrikeDamageMultiplier = 0.03f * character.RogueTalents.Aggression + 0.05f * character.RogueTalents.BladeTwisting + 0.1f * character.RogueTalents.SurpriseAttacks;
            float bonusYellowDamageMultiplier = 0.02f * character.RogueTalents.FindWeakness + 0.35f * 0.2f * character.RogueTalents.DirtyDeeds;
            float chanceOnEnergyOnCrit = 2f * (character.RogueTalents.FocusedAttacks > 2 ? 1f : (0.33f * character.RogueTalents.FocusedAttacks));
            float garrCostReduction = 10 * character.RogueTalents.DirtyDeeds;
            float hemoCostReduction = 1 * character.RogueTalents.SlaughterFromTheShadows;
            float mutiCostReduction = character.RogueTalents.GlyphOfMutilate ? 5 : 0;
            float sStrikeCostReduction = 3 * character.RogueTalents.ImprovedSinisterStrike;
            #endregion

            float modArmor = 0f;
            for (int i = 0; i < arPenUptimes.Length; i++)
            {
                modArmor += arPenUptimes[i].Chance * StatConversion.GetArmorDamageReduction(character.Level, calcOpts.TargetArmor, 1f - (1f - stats.ArmorPenetration) * (1f - armorReduction) * (mainHand.Type == ItemType.OneHandMace ? 1f - bonusMaceArP : 1), 0f, stats.ArmorPenetrationRating + arPenUptimes[i].Value);
            }
            float mainHandModArmor = 1f - modArmor;
            modArmor = 0f;
            for (int i = 0; i < arPenUptimes.Length; i++)
            {
                modArmor += arPenUptimes[i].Chance * StatConversion.GetArmorDamageReduction(character.Level, calcOpts.TargetArmor, 1f - (1f - stats.ArmorPenetration) * (1f - armorReduction) * (offHand.Type == ItemType.OneHandMace ? 1f - bonusMaceArP : 1), 0f, stats.ArmorPenetrationRating + arPenUptimes[i].Value);
            }
            float offHandModArmor = 1f - modArmor;
            float critMultiplier = 2f * (1f + stats.BonusCritMultiplier);
            float critMultiplierBleed = 2f * (1f + stats.BonusCritMultiplier);
            float critMultiplierPoison = 1.5f * (1f + stats.BonusCritMultiplier);
            float hasteBonus = StatConversion.GetPhysicalHasteFromRating(stats.HasteRating, CharacterClass.Rogue);
            hasteBonus = (1f + hasteBonus) * (1f + bonusFlurryHaste * 15f / 120f) - 1f;
            float speedModifier = 1f / (1f + hasteBonus) / (1f + stats.PhysicalHaste);
            float mainHandSpeed = mainHand == null ? 0f : mainHand._speed * speedModifier;
            float offHandSpeed = offHand == null ? 0f : offHand._speed * speedModifier;

            float mainHandSpeedNorm = mainHand.Type == ItemType.Dagger ? 1.7f : 2.4f;
            float offHandSpeedNorm = mainHand.Type == ItemType.Dagger ? 1.7f : 2.4f;

            float hitBonus = stats.PhysicalHit + StatConversion.GetPhysicalHitFromRating(stats.HitRating, CharacterClass.Rogue);
            float spellHitBonus = stats.SpellHit + StatConversion.GetSpellHitFromRating(stats.HitRating, CharacterClass.Rogue);
            float expertiseMHBonus = ((character.Race == CharacterRace.Human && (mainHand.Type == ItemType.OneHandSword || mainHand.Type == ItemType.OneHandMace)) ? 3 : 0) +
                                     ((character.Race == CharacterRace.Dwarf && (mainHand.Type == ItemType.OneHandMace)) ? 5 : 0) +
                                     ((character.Race == CharacterRace.Orc && (mainHand.Type == ItemType.OneHandAxe || mainHand.Type == ItemType.FistWeapon)) ? 5 : 0);
            float expertiseOHBonus = ((character.Race == CharacterRace.Human && (offHand.Type == ItemType.OneHandSword || offHand.Type == ItemType.OneHandMace)) ? 3 : 0) +
                                     ((character.Race == CharacterRace.Dwarf && (offHand.Type == ItemType.OneHandMace)) ? 5 : 0) +
                                     ((character.Race == CharacterRace.Orc && (offHand.Type == ItemType.OneHandAxe || offHand.Type == ItemType.FistWeapon)) ? 5 : 0);
            expertiseMHBonus = StatConversion.GetDodgeParryReducFromExpertise(StatConversion.GetExpertiseFromRating(stats.ExpertiseRating, CharacterClass.Rogue) + stats.Expertise + expertiseMHBonus, CharacterClass.Rogue);
            expertiseOHBonus = StatConversion.GetDodgeParryReducFromExpertise(StatConversion.GetExpertiseFromRating(stats.ExpertiseRating, CharacterClass.Rogue) + stats.Expertise + expertiseOHBonus, CharacterClass.Rogue);

            float chanceMHDodge = Math.Max(0f, StatConversion.WHITE_DODGE_CHANCE_CAP[targetLevel - 80] - expertiseMHBonus);
            float chanceOHDodge = Math.Max(0f, StatConversion.WHITE_DODGE_CHANCE_CAP[targetLevel - 80] - expertiseOHBonus);
            float chanceParry = 0f; //Math.Max(0f, StatConversion.WHITE_PARRY_CHANCE_CAP[targetLevel - 80] - expertiseBonus);
            float chanceWhiteMiss = Math.Max(0f, StatConversion.WHITE_MISS_CHANCE_CAP_DW[targetLevel - 80] - hitBonus);
            float chanceMiss = Math.Max(0f, StatConversion.YELLOW_MISS_CHANCE_CAP[targetLevel - 80] - hitBonus);
            float chancePoisonMiss = Math.Max(0f, StatConversion.GetSpellMiss(80 - targetLevel, false) - spellHitBonus);

            float glanceMultiplier = 0.75f;
            float chanceWhiteMHAvoided = chanceWhiteMiss + chanceMHDodge + chanceParry;
            float chanceWhiteOHAvoided = chanceWhiteMiss + chanceOHDodge + chanceParry;
            float chanceMHAvoided = chanceMiss + chanceMHDodge + chanceParry;
            float chanceOHAvoided = chanceMiss + chanceOHDodge + chanceParry;
            float chanceFinisherAvoided = chanceMiss + chanceMHDodge * (1f - character.RogueTalents.SurpriseAttacks) + chanceParry;
            float chancePoisonAvoided = chancePoisonMiss;
            float chanceWhiteMHNonAvoided = 1f - chanceWhiteMHAvoided;
            float chanceWhiteOHNonAvoided = 1f - chanceWhiteOHAvoided;
            float chanceMHNonAvoided = 1f - chanceMHAvoided;
            float chanceOHNonAvoided = 1f - chanceOHAvoided;
            float chancePoisonNonAvoided = 1f - chancePoisonAvoided;

            ////Crit Chances
            float chanceCritYellow = 0f;
            float chanceHitYellow = 0f;
            float chanceCritBackstab = 0f;
            float chanceHitBackstab = 0f;
            float cpPerBackstab = 0f;
            float chanceCritMuti = 0f;
            float chanceHitMuti = 0f;
            float cpPerMuti = 0f;
            float chanceCritSStrike = 0f;
            float chanceHitSStrike = 0f;
            float cpPerSStrike = 0f;
            float chanceCritHemo = 0f;
            float chanceHitHemo = 0f;
            float cpPerHemo = 0f;
            float chanceCritEvis = 0f;
            float chanceHitEvis = 0f;
            //float chanceCritBleed = 0f;
            float chanceGlance = StatConversion.WHITE_GLANCE_CHANCE_CAP[targetLevel - 80];
            float chanceCritWhiteMainTotal = 0f;
            float chanceCritWhiteMain = 0f;
            float chanceHitWhiteMain = 0f;
            float chanceCritWhiteOffTotal = 0f;
            float chanceCritWhiteOff = 0f;
            float chanceHitWhiteOff = 0f;
            float chanceCritPoison = 0f;
            float chanceHitPoison = 0f;

            for (int i = 0; i < critRatingUptimes.Length; i++)
            { //Sum up the weighted chances for each crit value
                WeightedStat iStat = critRatingUptimes[i];
                //Yellow - 2 Roll, so total of X chance to avoid, total of 1 chance to crit and hit when not avoided
                float chanceCritYellowTemp = Math.Min(1f, StatConversion.GetCritFromRating(stats.CritRating + iStat.Value, CharacterClass.Rogue)
                    + StatConversion.GetCritFromAgility(stats.Agility - 10f, CharacterClass.Rogue)
                    + stats.PhysicalCrit
                    + StatConversion.NPC_LEVEL_CRIT_MOD[targetLevel - 80]
                    + bonusMainHandCrit);
                float chanceHitYellowTemp = 1f - chanceCritYellowTemp;
                float chanceCritPoisonTemp = Math.Min(1f, StatConversion.GetSpellCritFromRating(stats.CritRating + iStat.Value)
                    + stats.SpellCrit
                    + stats.SpellCritOnTarget
                    + StatConversion.NPC_LEVEL_CRIT_MOD[targetLevel - 80]);
                float chanceHitPoisonTemp = 1f - chanceCritPoisonTemp;

                //Backstab - Identical to Yellow, with higher crit chance
                float chanceCritBackstabTemp = Math.Min(1f, chanceCritYellowTemp + bonusBackstabCrit + stats.BonusCPGCritChance + bonusCPGCrit);
                float chanceHitBackstabTemp = 1f - chanceCritBackstabTemp;
                float cpPerBackstabTemp = (chanceHitBackstabTemp + chanceCritBackstabTemp * (1f + 0.2f * character.RogueTalents.SealFate));

                //Mutilate - Identical to Yellow, with higher crit chance
                float chanceCritMutiTemp = Math.Min(1f, chanceCritYellowTemp + bonusMutiCrit + stats.BonusCPGCritChance + bonusCPGCrit);
                float chanceHitMutiTemp = 1f - chanceCritMutiTemp;
                float cpPerMutiTemp = (1 + chanceHitMutiTemp * chanceHitMutiTemp + (1 - chanceHitMutiTemp * chanceHitMutiTemp) * (1f + 0.2f * character.RogueTalents.SealFate));

                //Sinister Strike - Identical to Yellow, with higher crit chance
                float chanceCritSStrikeTemp = Math.Min(1f, chanceCritYellowTemp + stats.BonusCPGCritChance + bonusCPGCrit);
                float chanceHitSStrikeTemp = 1f - chanceCritSStrikeTemp;
                float cpPerSStrikeTemp = (chanceHitSStrikeTemp + chanceCritSStrikeTemp * (1f + 0.2f * character.RogueTalents.SealFate + (character.RogueTalents.GlyphOfSinisterStrike ? 0.5f : 0f)));

                //Hemorrhage - Identical to Yellow, with higher crit chance
                float chanceCritHemoTemp = Math.Min(1f, chanceCritYellowTemp + stats.BonusCPGCritChance + bonusCPGCrit);
                float chanceHitHemoTemp = 1f - chanceCritHemoTemp;
                float cpPerHemoTemp = (chanceHitHemoTemp + chanceCritHemoTemp * (1f + 0.2f * character.RogueTalents.SealFate));

                //Eviscerate - Identical to Yellow, with higher crit chance
                float chanceCritEvisTemp = Math.Min(1f, chanceCritYellowTemp + bonusEvisCrit);
                float chanceHitEvisTemp = 1f - chanceCritEvisTemp;

                //White Mainhand
                float chanceCritWhiteMainTotalTemp = chanceCritYellowTemp;
                float chanceCritWhiteMainTemp = Math.Min(chanceCritYellowTemp, 1f - chanceGlance - chanceWhiteMHAvoided);
                float chanceHitWhiteMainTemp = 1f - chanceCritWhiteMainTemp - chanceWhiteMHAvoided - chanceGlance;
                //White Offhand
                float chanceCritWhiteOffTotalTemp = chanceCritYellowTemp - bonusMainHandCrit + bonusOffHandCrit;
                float chanceCritWhiteOffTemp = Math.Min(chanceCritYellowTemp - bonusMainHandCrit + bonusOffHandCrit, 1f - chanceGlance - chanceWhiteOHAvoided);
                float chanceHitWhiteOffTemp = 1f - chanceCritWhiteOffTemp - chanceWhiteOHAvoided - chanceGlance;

                chanceCritYellow += iStat.Chance * chanceCritYellowTemp;
                chanceHitYellow += iStat.Chance * chanceHitYellowTemp;
                chanceCritBackstab += iStat.Chance * chanceCritBackstabTemp;
                chanceHitBackstab += iStat.Chance * chanceHitBackstabTemp;
                cpPerBackstab += iStat.Chance * cpPerBackstabTemp;
                chanceCritMuti += iStat.Chance * chanceCritMutiTemp;
                chanceHitMuti += iStat.Chance * chanceHitMutiTemp;
                cpPerMuti += iStat.Chance * cpPerMutiTemp;
                chanceCritSStrike += iStat.Chance * chanceCritSStrikeTemp;
                chanceHitSStrike += iStat.Chance * chanceHitSStrikeTemp;
                cpPerSStrike += iStat.Chance * cpPerSStrikeTemp;
                chanceCritHemo += iStat.Chance * chanceCritHemoTemp;
                chanceHitHemo += iStat.Chance * chanceHitHemoTemp;
                cpPerHemo += iStat.Chance * cpPerHemoTemp;
                chanceCritEvis += iStat.Chance * chanceCritEvisTemp;
                chanceHitEvis += iStat.Chance * chanceHitEvisTemp;
                chanceCritWhiteMainTotal += iStat.Chance * chanceCritWhiteMainTotalTemp;
                chanceCritWhiteMain += iStat.Chance * chanceCritWhiteMainTemp;
                chanceHitWhiteMain += iStat.Chance * chanceHitWhiteMainTemp;
                chanceCritWhiteOffTotal += iStat.Chance * chanceCritWhiteOffTotalTemp;
                chanceCritWhiteOff += iStat.Chance * chanceCritWhiteOffTemp;
                chanceHitWhiteOff += iStat.Chance * chanceHitWhiteOffTemp;
                chanceCritPoison += iStat.Chance * chanceCritPoisonTemp;
                chanceHitPoison += iStat.Chance * chanceHitPoisonTemp;
            }

            calculatedStats.DodgedMHAttacks = chanceMHDodge * 100f;
            calculatedStats.DodgedOHAttacks = chanceOHDodge * 100f;
            calculatedStats.ParriedAttacks = chanceParry * 100f;
            calculatedStats.MissedWhiteAttacks = chanceWhiteMiss * 100f;
            calculatedStats.MissedAttacks = chanceMiss * 100f;
            calculatedStats.MissedPoisonAttacks = chancePoisonMiss * 100f;

            float timeToReapplyDebuffs = 1f / (1f - chanceMHAvoided) - 1f;
            float lagVariance = (float)calcOpts.LagVariance / 1000f;
            float ruptDurationUptime = 16f + bonusRuptDuration;
            float ruptDurationAverage = ruptDurationUptime + timeToReapplyDebuffs + lagVariance;
            float snDBonusDuration = bonusSnDDuration - lagVariance;
            #endregion

            #region Attack Damages
            float baseDamage = (mainHand == null ? 0f : mainHand._speed) * stats.AttackPower / 14f + stats.WeaponDamage + (mainHand.MinDamage + mainHand.MaxDamage) / 2f;
            float baseDamageNorm = mainHandSpeedNorm * stats.AttackPower / 14f + stats.WeaponDamage + (mainHand.MinDamage + mainHand.MaxDamage) / 2f;
            float baseOffDamage = (((offHand == null ? 0f : offHand._speed) * stats.AttackPower / 14f + stats.WeaponDamage + (offHand.MinDamage + offHand.MaxDamage) / 2f)) * (0.5f * (1f + bonusOffHandDamageMultiplier));
            float baseOffDamageNorm = (offHandSpeedNorm * stats.AttackPower / 14f + stats.WeaponDamage + (offHand.MinDamage + offHand.MaxDamage) / 2f) * (0.5f * (1f + bonusOffHandDamageMultiplier));
            float meleeDamageRaw = baseDamage * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * mainHandModArmor;
            float meleeOffDamageRaw = baseOffDamage * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * offHandModArmor;
            float backstabDamageRaw = (baseDamageNorm * 1.5f + 465f) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + bonusBackstabDamageMultiplier + bonusYellowDamageMultiplier) * mainHandModArmor;
            backstabDamageRaw *= (mainHand._type == ItemType.Dagger ? 1f : 0f);
            float hemoDamageRaw = (baseDamageNorm * 1.1f) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + bonusHemoDamageMultiplier + bonusYellowDamageMultiplier) * mainHandModArmor;
            hemoDamageRaw *= (mainHand._type == ItemType.Dagger ? 1.6f/1.1f : 1f);
            hemoDamageRaw *= character.RogueTalents.Hemorrhage > 0 ? 1f : 0f;
            float sStrikeDamageRaw = (baseDamageNorm * 1f + 180f) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + bonusSStrikeDamageMultiplier + bonusYellowDamageMultiplier) * mainHandModArmor;
            float mutiDamageMainRaw = (baseDamageNorm * 1f + 181f) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + bonusMutiDamageMultiplier + bonusYellowDamageMultiplier) * (1f + (targetPoisonable ? 0.2f : 0f)) * mainHandModArmor;
            float mutiDamageOffRaw = (baseOffDamageNorm * 1f + 181f * (1f + bonusOffHandDamageMultiplier)) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + bonusMutiDamageMultiplier + bonusYellowDamageMultiplier) * (1f + (targetPoisonable ? 0.2f : 0f)) * offHandModArmor;
            mutiDamageMainRaw *= (character.RogueTalents.Mutilate > 0 && mainHand._type == ItemType.Dagger && offHand._type == ItemType.Dagger ? 1f : 0f);
            mutiDamageOffRaw *= (character.RogueTalents.Mutilate > 0 && mainHand._type == ItemType.Dagger && offHand._type == ItemType.Dagger ? 1f : 0f);
            float mutiDamageRaw = mutiDamageMainRaw + mutiDamageOffRaw;
            float ruptDamageRaw = (127f + 18f * 5f + 0.06f * stats.AttackPower * 5f / (3f + 5f)) * (3f + 5f + bonusRuptDuration / 2f) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + bonusRuptDamageMultiplier + bonusYellowDamageMultiplier) * (1f + stats.BonusBleedDamageMultiplier);
            float evisBaseDamageRaw = (127f + 381f) / 2f * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + bonusEvisDamageMultiplier + bonusYellowDamageMultiplier) * mainHandModArmor;
            float evisCPDamageRaw = (370f + stats.AttackPower * 0.07f) * (1f + stats.BonusPhysicalDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + bonusEvisDamageMultiplier + bonusYellowDamageMultiplier) * mainHandModArmor;
            float envenomBaseDamageRaw = 0f;
            float envenomCPDamageRaw = (215f + stats.AttackPower * 0.09f) * (1f + stats.BonusNatureDamageMultiplier) * (1f + stats.BonusDamageMultiplier) * (1f + bonusEnvenomDamageMultiplier + bonusYellowDamageMultiplier);
            float iPDamageRaw = ((300f + 400f) / 2f + stats.AttackPower * 0.09f) * (1f + bonusPoisonDamageMultiplier) * (1f + stats.BonusNatureDamageMultiplier) * (1f + stats.BonusDamageMultiplier);
            float dPDamageRaw = (296f + stats.AttackPower * 0.108f) * (1f + bonusPoisonDamageMultiplier) * (1f + stats.BonusNatureDamageMultiplier) * (1f + stats.BonusDamageMultiplier) / 4;
            float wPDamageRaw = ((78f + 231f) / 2f + stats.AttackPower * 0.036f) * (1f + bonusPoisonDamageMultiplier) * (1f + stats.BonusNatureDamageMultiplier) * (1f + stats.BonusDamageMultiplier);
            float aPDamageRaw = ((218f + 280f) / 2f) + (1f + bonusPoisonDamageMultiplier) * (1f + stats.BonusNatureDamageMultiplier) * (1f + stats.BonusDamageMultiplier);

            float meleeDamageAverage = (chanceGlance * glanceMultiplier + chanceCritWhiteMain * critMultiplier + chanceHitWhiteMain) * meleeDamageRaw;
            float meleeOffDamageAverage = (chanceGlance * glanceMultiplier + chanceCritWhiteOff * critMultiplier + chanceHitWhiteOff) * meleeOffDamageRaw;
            float backstabDamageAverage = (1f - chanceCritBackstab) * backstabDamageRaw + chanceCritBackstab * backstabDamageRaw * (critMultiplier + bonusCPGCritDamageMultiplier);
            float hemoDamageAverage = (1f - chanceCritHemo) * hemoDamageRaw + chanceCritHemo * hemoDamageRaw * (critMultiplier + bonusCPGCritDamageMultiplier);
            float sStrikeDamageAverage = (1f - chanceCritSStrike) * sStrikeDamageRaw + chanceCritSStrike * sStrikeDamageRaw * (critMultiplier + bonusCPGCritDamageMultiplier);
            float mutiDamageAverage = chanceHitMuti * chanceHitMuti * mutiDamageRaw +
                                      chanceCritMuti * chanceHitMuti * (mutiDamageMainRaw * (critMultiplier + bonusCPGCritDamageMultiplier) + mutiDamageOffRaw) +
                                      chanceHitMuti * chanceCritMuti * (mutiDamageMainRaw + mutiDamageOffRaw * (critMultiplier + bonusCPGCritDamageMultiplier)) +
                                      chanceCritMuti * chanceCritMuti * (mutiDamageMainRaw + mutiDamageOffRaw) * (critMultiplier + bonusCPGCritDamageMultiplier);
            float ruptDamageAverage = ((1f - chanceCritYellow) * ruptDamageRaw + chanceCritYellow * ruptDamageRaw * critMultiplierBleed);
            float evisBaseDamageAverage = (1f - chanceCritEvis) * evisBaseDamageRaw + chanceCritEvis * evisBaseDamageRaw * critMultiplier;
            float evisCPDamageAverage = (1f - chanceCritEvis) * evisCPDamageRaw + chanceCritEvis * evisCPDamageRaw * critMultiplier;
            float envenomBaseDamageAverage = (1f - chanceCritYellow) * envenomBaseDamageRaw + chanceCritYellow * envenomBaseDamageRaw * critMultiplier;
            float envenomCPDamageAverage = (1f - chanceCritYellow) * envenomCPDamageRaw + chanceCritYellow * envenomCPDamageRaw * critMultiplier;
            float iPDamageAverage = (1f - chanceCritPoison) * iPDamageRaw + chanceCritPoison * iPDamageRaw * critMultiplierPoison;
            float dPDamageAverage = dPDamageRaw;
            float wPDamageAverage = (1f - chanceCritPoison) * wPDamageRaw + chanceCritPoison * wPDamageRaw * critMultiplierPoison;
            float aPDamageAverage = (1f - chanceCritPoison) * aPDamageRaw + chanceCritPoison * aPDamageRaw * critMultiplierPoison;

            //if (needsDisplayCalculations)
            //{
                //Console.WriteLine("White:    {0:P} Avoided, {1:P} Glance,      {2:P} Hit, {3:P} Crit - Swing: {4}", chanceWhiteAvoided, chanceGlance, chanceHitWhiteMain, chanceCritWhiteMain, meleeDamageAverage);
                //Console.WriteLine("Yellow:   {0:P} Avoided, {1:P} NonAvoided,  {2:P} Hit, {3:P} Crit - Swing: {4}", chanceAvoided, chanceNonAvoided, 1f - chanceCritYellow, chanceCritYellow, sStrikeDamageAverage);
                //Console.WriteLine("SStrike:  {0:P} Avoided, {1:P} NonAvoided,  {2:P} Hit, {3:P} Crit - Swing: {4}", chanceAvoided, chanceNonAvoided, 1f - chanceCritSStrike, chanceCritSStrike, sStrikeDamageAverage);
                //    Console.WriteLine("Bite:     {0:P} Avoided, {1:P} NonAvoided,  {2:P} Hit, {3:P} Crit - Swing: {4}", chanceAvoided, chanceNonAvoided, 1f - chanceCritBite, chanceCritBite, biteBaseDamageAverage);
            //    Console.WriteLine("RipBleed:                                      {0:P} Hit, {1:P} Crit - Swing: {2}", 1f - chanceCritRip, chanceCritRip, ripDamageAverage);
            //    Console.WriteLine();
            //}
            #endregion

            #region Energy Costs
            float ambushEnergyRaw = 60f - ambushBackstabCostReduction - chanceOnEnergyOnCrit * chanceCritYellow;
            float garrEnergyRaw = 50f - garrCostReduction;
            float backstabEnergyRaw = 60f - ambushBackstabCostReduction - chanceOnEnergyOnCrit * chanceCritBackstab;
            float hemoEnergyRaw = 35f - hemoCostReduction - chanceOnEnergyOnCrit * chanceCritHemo;
            float sStrikeEnergyRaw = 45f - sStrikeCostReduction - chanceOnEnergyOnCrit * chanceCritSStrike;
            float mutiEnergyRaw = 60f - mutiCostReduction - 2 * chanceOnEnergyOnCrit * chanceCritMuti;
            float ruptEnergyRaw = 25f;
            float evisEnergyRaw = 35f - chanceOnEnergyOnCrit * chanceCritEvis;
            float envenomEnergyRaw = 35f - chanceOnEnergyOnCrit * chanceCritYellow;
            float snDEnergyRaw = 25f;

            //[rawCost + ((1/chance_to_land) - 1) * rawCost/5] 
            float cpgEnergyCostMultiplier = 1f + ((1f / chanceMHNonAvoided) - 1f) * 0.2f;
            float finisherEnergyCostMultiplier = 1f + ((1f / chanceMHNonAvoided) - 1f) * (1f - 0.4f * character.RogueTalents.QuickRecovery);
            float backstabEnergyAverage = backstabEnergyRaw * cpgEnergyCostMultiplier;
            float hemoEnergyAverage = hemoEnergyRaw * cpgEnergyCostMultiplier;
            float sStrikeEnergyAverage = sStrikeEnergyRaw * cpgEnergyCostMultiplier;
            float mutiEnergyAverage = mutiEnergyRaw * cpgEnergyCostMultiplier;
            float ruptEnergyAverage = ruptEnergyRaw * finisherEnergyCostMultiplier;
            float evisEnergyAverage = evisEnergyRaw * finisherEnergyCostMultiplier;
            float envenomEnergyAverage = envenomEnergyRaw * finisherEnergyCostMultiplier;
            float snDEnergyAverage = snDEnergyRaw * finisherEnergyCostMultiplier;
            #endregion

            #region Ability Stats
            RogueAbilityStats mainHandStats = new RogueMHStats()
            {
                DamagePerHit = meleeDamageRaw,
                DamagePerSwing = meleeDamageAverage,
                Weapon = mainHand,
                CritChance = chanceCritWhiteMain,
            };
            RogueAbilityStats offHandStats = new RogueOHStats()
            {
                DamagePerHit = meleeOffDamageRaw,
                DamagePerSwing = meleeOffDamageAverage,
                Weapon = offHand,
                CritChance = chanceCritWhiteOff,
            };
            RogueAbilityStats backstabStats = new RogueBackstabStats()
            {
                DamagePerHit = backstabDamageRaw,
                DamagePerSwing = backstabDamageAverage,
                EnergyCost = backstabEnergyAverage,
                CritChance = chanceCritBackstab,
                CPPerSwing = cpPerBackstab,
            };
            RogueAbilityStats hemoStats = new RogueHemoStats()
            {
                DamagePerHit = hemoDamageRaw,
                DamagePerSwing = hemoDamageAverage,
                EnergyCost = hemoEnergyAverage,
                CritChance = chanceCritYellow,
                CPPerSwing = cpPerHemo,
            };
            RogueAbilityStats sStrikeStats = new RogueSStrikeStats()
            {
                DamagePerHit = sStrikeDamageRaw,
                DamagePerSwing = sStrikeDamageAverage,
                EnergyCost = sStrikeEnergyAverage,
                CritChance = chanceCritYellow,
                CPPerSwing = cpPerSStrike,
            };
            RogueAbilityStats mutiStats = new RogueMutiStats()
            {
                DamagePerHit = mutiDamageRaw,
                DamagePerSwing = mutiDamageAverage,
                EnergyCost = mutiEnergyAverage,
                CritChance = chanceCritMuti,
                CPPerSwing = cpPerMuti,
            };
            RogueAbilityStats ruptStats = new RogueRuptStats()
            {
                DamagePerHit = ruptDamageRaw,
                DamagePerSwing = ruptDamageAverage,
                DurationUptime = ruptDurationUptime,
                DurationAverage = ruptDurationAverage,
                DurationPerCP = 2f,
                EnergyCost = ruptEnergyAverage,
            };
            RogueAbilityStats evisStats = new RogueEvisStats()
            {
                DamagePerHit = evisBaseDamageRaw,
                DamagePerSwing = evisBaseDamageAverage,
                DamagePerHitPerCP = evisCPDamageRaw,
                DamagePerSwingPerCP = evisCPDamageAverage,
                EnergyCost = evisEnergyAverage,
                CritChance = chanceCritEvis,
            };
            RogueAbilityStats envenomStats = new RogueEnvenomStats()
            {
                DamagePerHit = envenomBaseDamageRaw,
                DamagePerSwing = envenomBaseDamageAverage,
                DamagePerHitPerCP = envenomCPDamageRaw,
                DamagePerSwingPerCP = envenomCPDamageAverage,
                EnergyCost = envenomEnergyAverage,
                CritChance = chanceCritYellow,
            };
            RogueAbilityStats snDStats = new RogueSnDStats()
            {
                DurationUptime = snDBonusDuration * (1f + bonusSnDDurationMultiplier),
                DurationAverage = (6f + snDBonusDuration) * (1f + bonusSnDDurationMultiplier),
                EnergyCost = snDEnergyAverage,
                DurationPerCP = 3f,
            };
            RogueAbilityStats iPStats = new RogueIPStats()
            {
                DamagePerHit = iPDamageRaw,
                DamagePerSwing = iPDamageAverage,
            };
            RogueAbilityStats dPStats = new RogueDPStats()
            {
                DamagePerHit = dPDamageRaw,
                DamagePerSwing = dPDamageAverage,
            };
            RogueAbilityStats wPStats = new RogueWPStats()
            {
                DamagePerHit = wPDamageRaw,
                DamagePerSwing = wPDamageAverage,
            };
            RogueAbilityStats aPStats = new RogueAPStats()
            {
                DamagePerHit = aPDamageRaw,
                DamagePerSwing = aPDamageAverage,
            };
            #endregion

            #region Rotations
            RogueRotationCalculator rotationCalculator = new RogueRotationCalculator(character, stats, calcOpts, 
                maintainBleed, mainHandSpeed, offHandSpeed, mainHandSpeedNorm, offHandSpeedNorm,
                chanceWhiteMHAvoided, chanceWhiteOHAvoided, chanceMHAvoided, chanceOHAvoided, chanceFinisherAvoided, chancePoisonAvoided, chanceCritYellow * 0.2f * character.RogueTalents.SealFate, (1f - chanceHitMuti * chanceHitMuti) * 0.2f * character.RogueTalents.SealFate,
                mainHandStats, offHandStats, backstabStats, hemoStats, sStrikeStats, mutiStats,
                ruptStats, evisStats, envenomStats, snDStats, iPStats, dPStats, wPStats, aPStats);
            RogueRotationCalculator.RogueRotationCalculation rotationCalculationDPS = new RogueRotationCalculator.RogueRotationCalculation();

            bool bleedIsUp = calcOpts.BleedIsUp;
            if (calcOpts.ForceCustom == false)
                for (int snDCP = 1; snDCP < 6; snDCP++)
                    for (int finisher = 1; finisher < 3; finisher++)
                    {
                        if ((finisher == 1 && calcOpts.EnableEvis == false) ||
                           (finisher == 2 && calcOpts.EnableEnvenom == false)) continue;
                        for (int finisherCP = 1; finisherCP < 6; finisherCP++)
                            for (int CPG = 0; CPG < 4; CPG++)
                            {
                                if ((CPG == 0 && (calcOpts.EnableMuti == false || mutiStats.DamagePerSwing == 0)) ||
                                    (CPG == 1 && calcOpts.EnableSS == false) ||
                                    (CPG == 2 && (calcOpts.EnableBS == false || backstabStats.DamagePerSwing == 0)) ||
                                    (CPG == 3 && (calcOpts.EnableHemo == false || hemoStats.DamagePerSwing == 0))) continue;
                                for (int mHPoison = 0; mHPoison < 5; mHPoison++)
                                {
                                    if (!targetPoisonable || mainHand == null) break;
                                    if ((mHPoison == 1 && (calcOpts.EnableIP == false || character.RogueTalents.ImprovedPoisons < 4)) ||
                                        (mHPoison == 2 && calcOpts.EnableDP == false) ||
                                        (mHPoison == 3 && (calcOpts.EnableWP == false || character.RogueTalents.ImprovedPoisons > 3))) continue;
                                    for (int oHPoison = 0; oHPoison < 5; oHPoison++)
                                    {
                                        if (!targetPoisonable || offHand == null) break;
                                        if ((oHPoison == 1 && (calcOpts.EnableIP == false || character.RogueTalents.ImprovedPoisons < 4)) ||
                                            (oHPoison == 2 && calcOpts.EnableDP == false) ||
                                            (oHPoison == 3 && (calcOpts.EnableWP == false || character.RogueTalents.ImprovedPoisons > 3))) continue;
                                        for (int useRupt = 0; useRupt < 2; useRupt++)
                                        {
                                            if (useRupt == 1 && calcOpts.EnableRupt == false) continue;
                                            bool useTotT = stats.BonusToTTEnergy > 0;
                                            RogueRotationCalculator.RogueRotationCalculation rotationCalculation =
                                                rotationCalculator.GetRotationCalculations(
                                                CPG, useRupt == 1, finisher, finisherCP, snDCP, mHPoison, oHPoison, bleedIsUp, useTotT, PTRMode);
                                            if (rotationCalculation.DPS > rotationCalculationDPS.DPS)
                                                rotationCalculationDPS = rotationCalculation;
                                        }
                                    }
                                }
                            }
                    }

            calculatedStats.CustomRotation = rotationCalculator.GetRotationCalculations(
                calcOpts.CustomCPG, calcOpts.CustomUseRupt, calcOpts.CustomFinisher, calcOpts.CustomCPFinisher, calcOpts.CustomCPSnD, calcOpts.CustomMHPoison, calcOpts.CustomOHPoison, bleedIsUp, calcOpts.CustomUseTotT, PTRMode);
            calculatedStats.HighestDPSRotation = calcOpts.ForceCustom == false ? rotationCalculationDPS : calculatedStats.CustomRotation;

            if (character.RogueTalents.GlyphOfBackstab && rotationCalculationDPS.BackstabCount > 0)
            {
                ruptStats.DurationUptime += 6f;
                ruptStats.DurationAverage += 6f;
            }
            ruptStats.DamagePerHit *= ruptStats.DurationUptime / 16f;
            ruptStats.DamagePerSwing *= ruptStats.DurationUptime / 16f;
            #endregion

            calculatedStats.AvoidedWhiteMHAttacks = chanceWhiteMHAvoided * 100f;
            calculatedStats.AvoidedWhiteOHAttacks = chanceWhiteOHAvoided * 100f;
            calculatedStats.AvoidedAttacks = chanceMHAvoided * 100f;
            calculatedStats.AvoidedFinisherAttacks = chanceFinisherAvoided * 100f;
            calculatedStats.AvoidedPoisonAttacks = chancePoisonAvoided * 100f;
            calculatedStats.DodgedMHAttacks = chanceMHDodge * 100f;
            calculatedStats.ParriedAttacks = chanceParry * 100f;
            calculatedStats.MissedAttacks = chanceMiss * 100f;
            calculatedStats.CritChanceYellow = chanceCritYellow * 100f;
            calculatedStats.CritChanceMHTotal = chanceCritWhiteMainTotal * 100f;
            calculatedStats.CritChanceMH = chanceCritWhiteMain * 100f;
            calculatedStats.CritChanceOHTotal = chanceCritWhiteOffTotal * 100f;
            calculatedStats.CritChanceOH = chanceCritWhiteOff * 100f;
            calculatedStats.MainHandSpeed = mainHandSpeed;
            calculatedStats.OffHandSpeed = offHandSpeed;
            calculatedStats.ArmorMitigationMH = (1f - mainHandModArmor) * 100f;
            calculatedStats.ArmorMitigationOH = (1f - offHandModArmor) * 100f;
            calculatedStats.Duration = calcOpts.Duration;

            calculatedStats.MainHandStats = mainHandStats;
            calculatedStats.OffHandStats = offHandStats;
            calculatedStats.BackstabStats = backstabStats;
            calculatedStats.HemoStats = hemoStats;
            calculatedStats.SStrikeStats = sStrikeStats;
            calculatedStats.MutiStats = mutiStats;
            calculatedStats.RuptStats = ruptStats;
            calculatedStats.SnDStats = snDStats;
            calculatedStats.EvisStats = evisStats;
            calculatedStats.EnvenomStats = envenomStats;
            calculatedStats.IPStats = iPStats;
            calculatedStats.DPStats = dPStats;
            calculatedStats.WPStats = wPStats;
            calculatedStats.APStats = aPStats;

            float magicDPS = (stats.ShadowDamage + stats.ArcaneDamage) * (1f + chanceCritYellow);
            calculatedStats.DPSPoints = calculatedStats.HighestDPSRotation.DPS + magicDPS;
            calculatedStats.SurvivabilityPoints = stats.Health / 100f;
            calculatedStats.OverallPoints = calculatedStats.DPSPoints + calculatedStats.SurvivabilityPoints;
            return calculatedStats;
        }

        private Stats GetCharacterStatsWithTemporaryEffects(Character character, Item additionalItem, out WeightedStat[] armorPenetrationUptimes, out WeightedStat[] critRatingUptimes)
        {
            CalculationOptionsRogue calcOpts = character.CalculationOptions as CalculationOptionsRogue;
            int targetLevel = calcOpts.TargetLevel;
            bool targetPoisonable = calcOpts.TargetPoisonable;

            Stats statsRace = BaseStats.GetBaseStats(80, character.Class, character.Race);
            statsRace.PhysicalHaste = 0.4f; //Slice and Dice

            Stats statsItems = GetItemStats(character, additionalItem);
            Stats statsBuffs = GetBuffsStats(character, calcOpts);

            RogueTalents talents = character.RogueTalents;

            Stats statsTalents = new Stats()
            {
                BonusAgilityMultiplier = 0.03f * talents.SinisterCalling,
                BonusAttackPowerMultiplier = (1f + 0.02f * talents.SavageCombat) * (1f + 0.02f * talents.Deadliness) - 1f,
                BonusCritMultiplier = 0.04f * talents.PreyOnTheWeak,
                BonusDamageMultiplier = (1f + 0.02f * talents.Murder) * 
                                        (1f + 0.01f * talents.SlaughterFromTheShadows) * 
                                        (1f + (0.05f + (talents.GlyphOfHungerforBlood ? 0.03f : 0f)) * talents.HungerForBlood) - 1f,
                BonusStaminaMultiplier = 0.02f * talents.Endurance,
                Dodge = 0.02f * talents.LightningReflexes,
                Expertise = 5 * talents.WeaponExpertise,
                PhysicalCrit = 0.01f * talents.Malice + ((targetPoisonable &&
                                                          !(character.ActiveBuffsContains("Master Poisoner") || character.ActiveBuffsContains("Heart of the Crusader") || character.ActiveBuffsContains("Totem of Wrath")))
                                                         ? 0.01f * talents.MasterPoisoner : 0),
                PhysicalHit = 0.01f * talents.Precision,
                PhysicalHaste = (talents.LightningReflexes == 1 ? 0.04f : (talents.LightningReflexes == 2 ? 0.07f : (talents.LightningReflexes == 3 ? 0.1f : 0))),
                SpellCrit = 0.01f * talents.Malice + ((targetPoisonable &&
                                                      !(character.ActiveBuffsContains("Master Poisoner") || character.ActiveBuffsContains("Heart of the Crusader") || character.ActiveBuffsContains("Totem of Wrath")))
                                                      ? 0.01f * talents.MasterPoisoner : 0),
                SpellHit = 0.01f * talents.Precision,
            };

            Stats statsGearEnchantsBuffs = statsItems + statsBuffs;
            Stats statsTotal = statsRace + statsItems;
            statsTotal.Accumulate(statsBuffs);
            statsTotal.Accumulate(statsTalents);

            statsTotal.Stamina = (float)Math.Floor(statsTotal.Stamina * (1f + statsTotal.BonusStaminaMultiplier));
            statsTotal.Strength = (float)Math.Floor(statsTotal.Strength * (1f + statsTotal.BonusStrengthMultiplier));
            statsTotal.Agility = (float)Math.Floor(statsTotal.Agility * (1f + statsTotal.BonusAgilityMultiplier));
            statsTotal.AttackPower += statsTotal.Strength + statsTotal.Agility;
            statsTotal.AttackPower = (float)Math.Floor(statsTotal.AttackPower * (1f + statsTotal.BonusAttackPowerMultiplier));
            statsTotal.Health += (float)Math.Floor((statsTotal.Stamina - 20f) * 10f + 20f);
            statsTotal.Armor += 2f * statsTotal.Agility;
            statsTotal.Armor = (float)Math.Floor(statsTotal.Armor * (1f + statsTotal.BonusArmorMultiplier));
            statsTotal.NatureResistance += statsTotal.NatureResistanceBuff;
            statsTotal.FireResistance += statsTotal.FireResistanceBuff;
            statsTotal.FrostResistance += statsTotal.FrostResistanceBuff;
            statsTotal.ShadowResistance += statsTotal.ShadowResistanceBuff;
            statsTotal.ArcaneResistance += statsTotal.ArcaneResistanceBuff;

            float hasteBonus = StatConversion.GetPhysicalHasteFromRating(statsTotal.HasteRating, CharacterClass.Rogue);
            hasteBonus = (1f + hasteBonus) * (1f + statsTotal.PhysicalHaste) - 1f;
            float meleeHitInterval = 1f / ((character.MainHand == null ? 2 : character.MainHand.Speed / hasteBonus) + (character.OffHand == null ? 2 : character.OffHand.Speed / hasteBonus));

            //To calculate the Poison hit interval only white attacks are taken into account, IP is assumed on mainhand and DP on offhand
            float iPPS = 8.53f * (1f + 0.1f * talents.ImprovedPoisons) / 60f;
            float dPPS = calcOpts.Duration / (character.OffHand == null ? 2 : character.OffHand.Speed / hasteBonus) * 0.3f;
            float poisonHitInterval = 1 / (iPPS + dPPS);
            
            float hitBonus = StatConversion.GetPhysicalHitFromRating(statsTotal.HitRating) + statsTotal.PhysicalHit;
            float spellHitBonus = statsTotal.SpellHit + StatConversion.GetHitFromRating(statsTotal.HitRating, CharacterClass.Rogue);
            float expertiseBonus = StatConversion.GetDodgeParryReducFromExpertise(StatConversion.GetExpertiseFromRating(statsTotal.ExpertiseRating, CharacterClass.Rogue) + statsTotal.Expertise, CharacterClass.Rogue);
            float chanceDodge = Math.Max(0f, StatConversion.WHITE_DODGE_CHANCE_CAP[targetLevel - 80] - expertiseBonus);
            float chanceParry = 0f; //Math.Max(0f, StatConversion.WHITE_PARRY_CHANCE_CAP[targetLevel - 80] - expertiseBonus);
            float chanceMiss = Math.Max(0f, StatConversion.WHITE_MISS_CHANCE_CAP[targetLevel - 80] - hitBonus);
            float chanceAvoided = chanceMiss + chanceDodge + chanceParry;
            float chancePoisonAvoided = Math.Max(0f, StatConversion.GetSpellMiss(80 - targetLevel, false) - spellHitBonus);

            float rawChanceCrit = StatConversion.GetPhysicalCritFromRating(statsTotal.CritRating)
                                + StatConversion.GetPhysicalCritFromAgility(statsTotal.Agility, CharacterClass.Rogue)
                                + statsTotal.PhysicalCrit
                                + StatConversion.NPC_LEVEL_CRIT_MOD[targetLevel - 80];
            float chanceCrit = rawChanceCrit * (1f - chanceAvoided);
            float chanceHit = 1f - chanceAvoided;

            Dictionary<Trigger, float> triggerIntervals = new Dictionary<Trigger, float>();
            Dictionary<Trigger, float> triggerChances = new Dictionary<Trigger, float>();
            triggerIntervals[Trigger.Use] = 0f;
            triggerIntervals[Trigger.MeleeHit] = meleeHitInterval;
            triggerIntervals[Trigger.PhysicalHit] = meleeHitInterval;
            triggerIntervals[Trigger.MeleeAttack] = meleeHitInterval;
            triggerIntervals[Trigger.MeleeCrit] = meleeHitInterval;
            triggerIntervals[Trigger.PhysicalCrit] = meleeHitInterval;
            triggerIntervals[Trigger.DoTTick] = 0f;
            triggerIntervals[Trigger.DamageDone] = meleeHitInterval / 2f;
            triggerIntervals[Trigger.DamageOrHealingDone] = meleeHitInterval / 2f; // Need to add Self-heals
            triggerIntervals[Trigger.SpellHit] = poisonHitInterval;
            triggerChances[Trigger.Use] = 1f;
            triggerChances[Trigger.MeleeHit] = Math.Max(0f, chanceHit);
            triggerChances[Trigger.PhysicalHit] = Math.Max(0f, chanceHit);
            triggerChances[Trigger.MeleeAttack] = Math.Max(0f, chanceHit);
            triggerChances[Trigger.MeleeCrit] = Math.Max(0f, chanceCrit);
            triggerChances[Trigger.PhysicalCrit] = Math.Max(0f, chanceCrit);
            triggerChances[Trigger.DoTTick] = 1f;
            triggerChances[Trigger.DamageDone] = 1f - chanceAvoided / 2f;
            triggerChances[Trigger.DamageOrHealingDone] = 1f - chanceAvoided / 2f; // Need to add Self-heals
            triggerChances[Trigger.SpellHit] = Math.Max(0f, 1f - chancePoisonAvoided);

            // Handle Trinket procs
            Stats statsProcs = new Stats();
            foreach (SpecialEffect effect in statsTotal.SpecialEffects(se => triggerIntervals.ContainsKey(se.Trigger)))
            {
                // JOTHAY's NOTE: The following is an ugly hack to add Recursive Effects to Cat
                // so Victor's Call and similar trinkets can be given more appropriate value
                if (effect.Trigger == Trigger.Use && effect.Stats._rawSpecialEffectDataSize == 1
                    && triggerIntervals.ContainsKey(effect.Stats._rawSpecialEffectData[0].Trigger))
                {
                    float upTime = effect.GetAverageUptime(triggerIntervals[effect.Trigger],
                        triggerChances[effect.Trigger], 1f, calcOpts.Duration);
                    statsProcs.Accumulate(effect.Stats._rawSpecialEffectData[0].GetAverageStats(
                        triggerIntervals[effect.Stats._rawSpecialEffectData[0].Trigger],
                        triggerChances[effect.Stats._rawSpecialEffectData[0].Trigger], 1f, calcOpts.Duration),
                        upTime);
                }
                else if (effect.Stats.MoteOfAnger > 0)
                {
                    // When in effect stats, MoteOfAnger is % of melee hits
                    // When in character stats, MoteOfAnger is average procs per second
                    statsProcs.MoteOfAnger = effect.Stats.MoteOfAnger * effect.GetAverageProcsPerSecond(triggerIntervals[effect.Trigger],
                        triggerChances[effect.Trigger], 1f, calcOpts.Duration) / effect.MaxStack;
                }
                else
                {
                    statsProcs.Accumulate(effect.GetAverageStats(triggerIntervals[effect.Trigger],
                        triggerChances[effect.Trigger], 1f, calcOpts.Duration),
                        effect.Stats.DeathbringerProc > 0 ? 1f / 3f : 1f);
                }
            }

            statsProcs.Agility += statsProcs.HighestStat + statsProcs.Paragon + statsProcs.DeathbringerProc;
            statsProcs.Strength += statsProcs.DeathbringerProc;
            statsProcs.Stamina = (float)Math.Floor(statsProcs.Stamina * (1f + statsTotal.BonusStaminaMultiplier));
            statsProcs.Strength = (float)Math.Floor(statsProcs.Strength * (1f + statsTotal.BonusStrengthMultiplier));
            statsProcs.Agility = (float)Math.Floor(statsProcs.Agility * (1f + statsTotal.BonusAgilityMultiplier));
            statsProcs.AttackPower += statsProcs.Strength * 2f + statsProcs.Agility;
            statsProcs.AttackPower = (float)Math.Floor(statsProcs.AttackPower * (1f + statsTotal.BonusAttackPowerMultiplier));
            statsProcs.HasteRating += statsProcs.DeathbringerProc;
            statsProcs.Health += (float)Math.Floor(statsProcs.Stamina * 10f);
            statsProcs.Armor += 2f * statsProcs.Agility;
            statsProcs.Armor = (float)Math.Floor(statsProcs.Armor * (1f + statsTotal.BonusArmorMultiplier));

            //Agility is only used for crit from here on out; we'll be converting Agility to CritRating, 
            //and calculating CritRating separately, so don't add any Agility or CritRating from procs here.
            //Also calculating ArPen separately, so don't add that either.
            statsProcs.CritRating = statsProcs.Agility = statsProcs.ArmorPenetrationRating = 0;
            statsTotal.Accumulate(statsProcs);

            //Handle Crit procs
            critRatingUptimes = new WeightedStat[0];
            List<SpecialEffect> tempCritEffects = new List<SpecialEffect>();
            List<float> tempCritEffectIntervals = new List<float>();
            List<float> tempCritEffectChances = new List<float>();
            List<float> tempCritEffectScales = new List<float>();

            foreach (SpecialEffect effect in statsTotal.SpecialEffects(se => triggerIntervals.ContainsKey(se.Trigger) && (se.Stats.CritRating + se.Stats.Agility + se.Stats.DeathbringerProc + se.Stats.HighestStat + se.Stats.Paragon) > 0))
            {
                tempCritEffects.Add(effect);
                tempCritEffectIntervals.Add(triggerIntervals[effect.Trigger]);
                tempCritEffectChances.Add(triggerChances[effect.Trigger]);
                tempCritEffectScales.Add(effect.Stats.DeathbringerProc > 0 ? 1f / 3f : 1f);
            }

            if (tempCritEffects.Count == 0)
            {
                critRatingUptimes = new WeightedStat[] { new WeightedStat() { Chance = 1f, Value = 0f } };
            }
            else if (tempCritEffects.Count == 1)
            { //Only one, add it to
                SpecialEffect effect = tempCritEffects[0];
                float uptime = effect.GetAverageUptime(triggerIntervals[effect.Trigger], triggerChances[effect.Trigger], 1f, calcOpts.Duration) * tempCritEffectScales[0];
                float totalAgi = (effect.Stats.Agility + effect.Stats.DeathbringerProc + effect.Stats.HighestStat + effect.Stats.Paragon) * (1f + statsTotal.BonusAgilityMultiplier);
                critRatingUptimes = new WeightedStat[] { new WeightedStat() { Chance = uptime, Value = 
						effect.Stats.CritRating + StatConversion.GetCritFromAgility(totalAgi - 10,
						CharacterClass.Rogue) * StatConversion.RATING_PER_PHYSICALCRIT },
					new WeightedStat() { Chance = 1f - uptime, Value = 0f }};
            }
            else if (tempCritEffects.Count > 1)
            {
                List<float> tempCritEffectsValues = new List<float>();
                foreach (SpecialEffect effect in tempCritEffects)
                {
                    float totalAgi = (float)effect.MaxStack * (effect.Stats.Agility + effect.Stats.DeathbringerProc + effect.Stats.HighestStat + effect.Stats.Paragon) * (1f + statsTotal.BonusAgilityMultiplier);
                    tempCritEffectsValues.Add(effect.Stats.CritRating +
                        StatConversion.GetCritFromAgility(totalAgi - 10, CharacterClass.Rogue) *
                        StatConversion.RATING_PER_PHYSICALCRIT);
                }

                float[] intervals = new float[tempCritEffects.Count];
                float[] chances = new float[tempCritEffects.Count];
                float[] offset = new float[tempCritEffects.Count];
                for (int i = 0; i < tempCritEffects.Count; i++)
                {
                    intervals[i] = triggerIntervals[tempCritEffects[i].Trigger];
                    chances[i] = triggerChances[tempCritEffects[i].Trigger];
                }
                if (tempCritEffects.Count >= 2)
                {
                    offset[0] = calcOpts.TrinketOffset;
                }
                WeightedStat[] critWeights = SpecialEffect.GetAverageCombinedUptimeCombinations(tempCritEffects.ToArray(), intervals, chances, offset, tempCritEffectScales.ToArray(), 1f, calcOpts.Duration, tempCritEffectsValues.ToArray());
                critRatingUptimes = critWeights;
            }

            //Handle ArPen procs
            armorPenetrationUptimes = new WeightedStat[0];
            List<SpecialEffect> tempArPenEffects = new List<SpecialEffect>();
            List<float> tempArPenEffectIntervals = new List<float>();
            List<float> tempArPenEffectChances = new List<float>();
            List<float> tempArPenEffectScales = new List<float>();

            foreach (SpecialEffect effect in statsTotal.SpecialEffects(se => triggerIntervals.ContainsKey(se.Trigger) && se.Stats.ArmorPenetrationRating > 0))
            {
                tempArPenEffects.Add(effect);
                tempArPenEffectIntervals.Add(triggerIntervals[effect.Trigger]);
                tempArPenEffectChances.Add(triggerChances[effect.Trigger]);
                tempArPenEffectScales.Add(effect.Stats.DeathbringerProc > 0 ? 1f / 3f : 1f);
            }

            if (tempArPenEffects.Count == 0)
            {
                armorPenetrationUptimes = new WeightedStat[] { new WeightedStat() { Chance = 1f, Value = 0f } };
            }
            else if (tempArPenEffects.Count == 1)
            { //Only one, add it to
                SpecialEffect effect = tempArPenEffects[0];
                float uptime = effect.GetAverageUptime(triggerIntervals[effect.Trigger], triggerChances[effect.Trigger], 1f, calcOpts.Duration) * tempArPenEffectScales[0];
                armorPenetrationUptimes = new WeightedStat[] { new WeightedStat() { Chance = uptime, Value = effect.Stats.ArmorPenetrationRating + effect.Stats.DeathbringerProc },
					new WeightedStat() { Chance = 1f - uptime, Value = 0f }};
            }
            else if (tempArPenEffects.Count > 1)
            {
                List<float> tempArPenEffectValues = new List<float>();
                foreach (SpecialEffect effect in tempArPenEffects)
                {
                    tempArPenEffectValues.Add(effect.Stats.ArmorPenetrationRating);
                }

                float[] intervals = new float[tempArPenEffects.Count];
                float[] chances = new float[tempArPenEffects.Count];
                float[] offset = new float[tempArPenEffects.Count];
                for (int i = 0; i < tempArPenEffects.Count; i++)
                {
                    intervals[i] = triggerIntervals[tempArPenEffects[i].Trigger];
                    chances[i] = triggerChances[tempArPenEffects[i].Trigger];
                }
                if (tempArPenEffects.Count >= 2)
                {
                    offset[0] = calcOpts.TrinketOffset;
                }
                WeightedStat[] arPenWeights = SpecialEffect.GetAverageCombinedUptimeCombinations(tempArPenEffects.ToArray(), intervals, chances, offset, tempArPenEffectScales.ToArray(), 1f, calcOpts.Duration, tempArPenEffectValues.ToArray());
                armorPenetrationUptimes = arPenWeights;
            }

            return statsTotal;
        }

        public override Stats GetCharacterStats(Character character, Item additionalItem)
        {
            WeightedStat[] armorPenetrationUptimes, critRatingUptimes;
            return GetCharacterStatsWithTemporaryEffects(character, additionalItem, out armorPenetrationUptimes, out critRatingUptimes);
        }

        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName) {
            switch (chartName) {
                case "Combat Table":
                    var currentCalculationsRogue = GetCharacterCalculations(character) as CharacterCalculationsRogue;
                    var calcMiss = new ComparisonCalculationsRogue();
                    var calcDodge = new ComparisonCalculationsRogue();
                    var calcParry = new ComparisonCalculationsRogue();
                    var calcBlock = new ComparisonCalculationsRogue();
                    var calcGlance = new ComparisonCalculationsRogue();
                    var calcCrit = new ComparisonCalculationsRogue();
                    var calcHit = new ComparisonCalculationsRogue();

                    if (currentCalculationsRogue != null)
                    {
                        calcMiss.Name = "    Miss    ";
                        calcDodge.Name = "   Dodge   ";
                        calcGlance.Name = " Glance ";
                        calcCrit.Name = "  Crit  ";
                        calcHit.Name = "Hit";

                        calcMiss.OverallPoints = 0f;
                        calcDodge.OverallPoints = 0f;
                        calcParry.OverallPoints = 0f;
                        calcBlock.OverallPoints = 0f;
                        calcGlance.OverallPoints = 0f;
                        calcCrit.OverallPoints = 0f;
                        calcHit.OverallPoints = 0f;
                    }
                    return new ComparisonCalculationBase[] {calcMiss, calcDodge, calcParry, calcGlance, calcBlock, calcCrit, calcHit};

                default:
                    return new ComparisonCalculationBase[0];
            }
        }

        public override Stats GetRelevantStats(Stats stats) {
            Stats relevantStats = new Stats {
               Agility = stats.Agility,
               Strength = stats.Strength,
               AttackPower = stats.AttackPower,
               CritRating = stats.CritRating,
               HitRating = stats.HitRating,
               Stamina = stats.Stamina,
               HasteRating = stats.HasteRating,
               ExpertiseRating = stats.ExpertiseRating,
               ArmorPenetration = stats.ArmorPenetration,
               ArmorPenetrationRating = stats.ArmorPenetrationRating,
               WeaponDamage = stats.WeaponDamage,
               BonusAgilityMultiplier = stats.BonusAgilityMultiplier,
               BonusAttackPowerMultiplier = stats.BonusAttackPowerMultiplier,
               BonusCritMultiplier = stats.BonusCritMultiplier,
               BonusDamageMultiplier = stats.BonusDamageMultiplier,
               BonusStaminaMultiplier = stats.BonusStaminaMultiplier,
               BonusStrengthMultiplier = stats.BonusStrengthMultiplier,
               Health = stats.Health,
               ThreatReductionMultiplier = stats.ThreatReductionMultiplier,
               PhysicalHaste = stats.PhysicalHaste,
               PhysicalHit = stats.PhysicalHit,
               PhysicalCrit = stats.PhysicalCrit,
               HighestStat = stats.HighestStat,
               MoteOfAnger = stats.MoteOfAnger,
               
               /*
               ArcaneResistance = stats.ArcaneResistance,
               NatureResistance = stats.NatureResistance,
               FireResistance = stats.FireResistance,
               FrostResistance = stats.FrostResistance,
               ShadowResistance = stats.ShadowResistance,
               ArcaneResistanceBuff = stats.ArcaneResistanceBuff,
               NatureResistanceBuff = stats.NatureResistanceBuff,
               FireResistanceBuff = stats.FireResistanceBuff,
               FrostResistanceBuff = stats.FrostResistanceBuff,
               ShadowResistanceBuff = stats.ShadowResistanceBuff,*/

               RuptureDamageBonus = stats.RuptureDamageBonus,
               ComboMoveEnergyReduction = stats.ComboMoveEnergyReduction,
               BonusEnergyFromDP = stats.BonusEnergyFromDP,
               RuptureCrit = stats.RuptureCrit,
               ReduceEnergyCostFromRupture = stats.ReduceEnergyCostFromRupture,
               BonusCPGCritChance = stats.BonusCPGCritChance,
               BonusToTTEnergy = stats.BonusToTTEnergy,
               ChanceOn3CPOnFinisher = stats.ChanceOn3CPOnFinisher,

               BonusPhysicalDamageMultiplier = stats.BonusPhysicalDamageMultiplier,
               BonusBleedDamageMultiplier = stats.BonusBleedDamageMultiplier,
               SpellHit = stats.SpellHit,
               SpellCrit = stats.SpellCrit,
               SpellCritOnTarget = stats.SpellCritOnTarget,

                // Damage Procs
               ShadowDamage = stats.ShadowDamage,
               ArcaneDamage = stats.ArcaneDamage,
               HolyDamage = stats.HolyDamage,
               NatureDamage = stats.NatureDamage,
               FrostDamage = stats.FrostDamage,
               FireDamage = stats.FireDamage,
               BonusShadowDamageMultiplier = stats.BonusShadowDamageMultiplier,
               BonusArcaneDamageMultiplier = stats.BonusArcaneDamageMultiplier,
               BonusHolyDamageMultiplier = stats.BonusHolyDamageMultiplier,
               BonusNatureDamageMultiplier = stats.BonusNatureDamageMultiplier,
               BonusFrostDamageMultiplier = stats.BonusFrostDamageMultiplier,
               BonusFireDamageMultiplier = stats.BonusFireDamageMultiplier,
            };

            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if (effect.Trigger == Trigger.Use || effect.Trigger == Trigger.MeleeCrit || effect.Trigger == Trigger.MeleeHit
                || effect.Trigger == Trigger.PhysicalCrit || effect.Trigger == Trigger.PhysicalHit || effect.Trigger == Trigger.DoTTick
                    || effect.Trigger == Trigger.DamageDone || effect.Trigger == Trigger.DamageOrHealingDone || effect.Trigger == Trigger.SpellHit
                     || effect.Trigger == Trigger.MeleeAttack)
                {
                    if (HasRelevantStats(effect.Stats))
                    {
                        relevantStats.AddSpecialEffect(effect);
                    }
                }
            }

            return relevantStats;
        }
        public override bool HasRelevantStats(Stats stats) {
            bool relevant = (
                    stats.Agility +
                    stats.Strength +
                    stats.AttackPower +
                    stats.CritRating +
                    stats.HitRating +
                    //stats.Stamina +
                    stats.HasteRating +
                    stats.ExpertiseRating +
                    stats.ArmorPenetration +
                    stats.ArmorPenetrationRating +
                    stats.WeaponDamage +
                    stats.BonusAgilityMultiplier +
                    stats.BonusAttackPowerMultiplier +
                    stats.BonusCritMultiplier +
                    stats.BonusDamageMultiplier +
                    stats.BonusStaminaMultiplier +
                    stats.BonusStrengthMultiplier +
                    //stats.Health +
                    stats.ThreatReductionMultiplier +
                    stats.PhysicalHaste +
                    stats.PhysicalHit +
                    stats.PhysicalCrit +
                    stats.HighestStat +

                    /*
                    stats.ArcaneResistance +
                    stats.NatureResistance +
                    stats.FireResistance +
                    stats.FrostResistance +
                    stats.ShadowResistance +
                    stats.ArcaneResistanceBuff +
                    stats.NatureResistanceBuff +
                    stats.FireResistanceBuff +
                    stats.FrostResistanceBuff +
                    stats.ShadowResistanceBuff +*/

                    stats.RuptureDamageBonus +
                    stats.ComboMoveEnergyReduction +
                    stats.BonusEnergyFromDP +
                    stats.RuptureCrit +
                    stats.ReduceEnergyCostFromRupture +
                    stats.BonusCPGCritChance +
                    stats.BonusToTTEnergy +
                    stats.ChanceOn3CPOnFinisher +

                    stats.BonusPhysicalDamageMultiplier +
                    stats.BonusBleedDamageMultiplier + 
                    stats.SpellHit +
                    stats.SpellCrit +
                    stats.SpellCritOnTarget +

                    // Trinket Procs
                    stats.Paragon +
                    stats.DeathbringerProc +
                    stats.MoteOfAnger +

                    // Damage Procs
                    stats.ShadowDamage +
                    stats.ArcaneDamage +
                    stats.HolyDamage +
                    stats.NatureDamage +
                    stats.FrostDamage +
                    stats.FireDamage +
                    stats.BonusShadowDamageMultiplier +
                    stats.BonusArcaneDamageMultiplier +
                    stats.BonusHolyDamageMultiplier +
                    stats.BonusNatureDamageMultiplier +
                    stats.BonusFrostDamageMultiplier +
                    stats.BonusFireDamageMultiplier
                ) > 0 || (stats.Stamina > 0 && stats.SpellPower == 0);

            foreach (SpecialEffect effect in stats.SpecialEffects()) {
                if (effect.Trigger == Trigger.Use
                    || effect.Trigger == Trigger.MeleeHit
                    || effect.Trigger == Trigger.MeleeCrit
                    || effect.Trigger == Trigger.MeleeAttack
                    || effect.Trigger == Trigger.PhysicalHit
                    || effect.Trigger == Trigger.PhysicalCrit
                    || effect.Trigger == Trigger.DoTTick
                    || effect.Trigger == Trigger.DamageDone
                    || effect.Trigger == Trigger.DamageOrHealingDone
                    || effect.Trigger == Trigger.SpellHit) // For Poison Hits
                {
                    relevant |= HasRelevantStats(effect.Stats);
                    if (relevant) { break; }
                }
            }
            return relevant;
        }

        public override bool IsItemRelevant(Item item)
        {
            if (item.Slot == ItemSlot.OffHand && item.Type == ItemType.None)
                return false;
            return base.IsItemRelevant(item);
        }
        
        public Stats GetBuffsStats(Character character, CalculationOptionsRogue calcOpts)
        {
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

            foreach (Buff b in removedBuffs)
            {
                character.ActiveBuffsAdd(b);
            }
            foreach (Buff b in addedBuffs)
            {
                character.ActiveBuffs.Remove(b);
            }

            return statsBuffs;
        }
        public override void SetDefaults(Character character)
        {
            character.ActiveBuffsAdd(("Horn of Winter"));
            character.ActiveBuffsAdd(("Battle Shout"));
            character.ActiveBuffsAdd(("Unleashed Rage"));
            character.ActiveBuffsAdd(("Improved Moonkin Form"));
            character.ActiveBuffsAdd(("Leader of the Pack"));
            character.ActiveBuffsAdd(("Improved Icy Talons"));
            character.ActiveBuffsAdd(("Power Word: Fortitude"));
            character.ActiveBuffsAdd(("Mark of the Wild"));
            character.ActiveBuffsAdd(("Blessing of Kings"));
            character.ActiveBuffsAdd(("Sunder Armor"));
            character.ActiveBuffsAdd(("Faerie Fire"));
            character.ActiveBuffsAdd(("Totem of Wrath"));
            character.ActiveBuffsAdd(("Flask of Endless Rage"));
            character.ActiveBuffsAdd(("Agility Food"));
            character.ActiveBuffsAdd(("Heroism/Bloodlust"));

            if (character.PrimaryProfession == Profession.Alchemy ||
                character.SecondaryProfession == Profession.Alchemy)
                character.ActiveBuffsAdd(("Flask of Endless Rage (Mixology)"));
        }

        private static List<string> _relevantGlyphs = null;
        public override List<string> GetRelevantGlyphs() {
            if (_relevantGlyphs == null) {
                _relevantGlyphs = new List<string>();
                _relevantGlyphs.Add("Glyph of Backstab");
                _relevantGlyphs.Add("Glyph of Eviscerate");
                _relevantGlyphs.Add("Glyph of Mutilate");
                _relevantGlyphs.Add("Glyph of Hunger For Blood");
                _relevantGlyphs.Add("Glyph of Sinister Strike");
                _relevantGlyphs.Add("Glyph of Slice and Dice");
                _relevantGlyphs.Add("Glyph of Feint");
                _relevantGlyphs.Add("Glyph of Rupture");
                _relevantGlyphs.Add("Glyph of Blade Flurry");
                _relevantGlyphs.Add("Glyph of Adrenaline Rush");
                _relevantGlyphs.Add("Glyph of Killing Spree");
                _relevantGlyphs.Add("Glyph of Vigor");
                _relevantGlyphs.Add("Glyph of Fan of Knives");
                _relevantGlyphs.Add("Glyph of Expose Armor");
                _relevantGlyphs.Add("Glyph of Ghostly Strike");
                _relevantGlyphs.Add("Glyph of Tricks of the Trade");
            }
            return _relevantGlyphs;
        }
    }
    public class Knuckles : Item { public Knuckles() { Speed = 2f; } }

    public class ComparisonCalculationsRogue : ComparisonCalculationBase
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

        public float DPSPoints
        {
            get { return _subPoints[0]; }
            set { _subPoints[0] = value; }
        }

        public float SurvivabilityPoints
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
            return string.Format("{0}: ({1}O {2}DPS)", Name, Math.Round(OverallPoints), Math.Round(DPSPoints));
        }
    }
}