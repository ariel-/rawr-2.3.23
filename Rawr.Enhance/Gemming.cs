using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Enhance
{
    class Gemming
    {
        ////Relevant Gem IDs for Enhancement Shamans
        //Red
        private int[] delicate = { 39905, 39997, 40112, 42143 };  // Agility
        private int[] bright = { 39906, 39999, 40114, 36766 };    // Attack Power
        private int[] precise = { 39910, 40003, 40118, 42154 };   // Expertise
        private int[] fractured = { 39909, 40002, 40117, 42153 }; // Armour Penetration

        //Yellow
        private int[] rigid = { 39915, 40014, 40125, 42156 };  // Hit
        private int[] smooth = { 39914, 40013, 40124, 42149 }; // Crit
        private int[] quick = { 39918, 40017, 40128, 42150 };  // Haste

        //Orange
        private int[] deadly = { 39952, 40043, 40147, 40147 };   // Agi/Crit
        private int[] glinting = { 39953, 40044, 40148, 40148 }; // Agi/Hit
        private int[] deft = { 39955, 40046, 40150, 40150 };     // Agi/Haste
        private int[] wicked = { 39960, 40052, 40156, 40156 };   // AP/Crit
        private int[] pristine = { 39961, 40053, 40157, 40157 }; // AP/Hit
        private int[] stark = { 39963, 40055, 40159, 40159 };    // AP/Haste
        private int[] accurate = { 39966, 40058, 40162, 40162 }; // Exp/Hit

        //Purple
        private int[] guardian = { 39940, 40034, 40141, 40141 }; // Exp/Sta
        private int[] balanced = { 39937, 40029, 40136, 40136 }; // Ap/Sta
        private int[] shifting = { 39935, 40023, 40130, 40130 }; // Agi/Sta
        private int[] puissant = { 39933, 40033, 40140, 40140 }; // ArP/Sta

        //Green
        private int[] vivid = { 39975, 40088, 40166, 40166 };    // Hit/Sta
        private int[] jagged = { 39974, 40086, 40165, 40165 };   // Crit/Sta
        private int[] forceful = { 39978, 40091, 40169, 40169 }; // Haste/Sta

        //Prismatic
        private int[] tear = { 34143, 42702, 49110, 49110 }; // +X to all stats

        public List<GemmingTemplate> addTemplates(String group, int rarity, int metagem, bool enabled)
        {
            return new List<GemmingTemplate>() { 
            	new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled, // Stats
					RedId = tear[rarity], YellowId = tear[rarity], BlueId = tear[rarity], PrismaticId = tear[rarity], MetaId = metagem },

            	new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled, //Max Expertise - colour match
					RedId = precise[rarity], YellowId = accurate[rarity], BlueId = guardian[rarity], PrismaticId = precise[rarity], MetaId = metagem },
            	new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled, //Max Expertise - no colour match
					RedId = precise[rarity], YellowId = precise[rarity], BlueId = precise[rarity], PrismaticId = precise[rarity], MetaId = metagem },

            	new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Hit - red exp
					RedId = accurate[rarity], YellowId = rigid[rarity], BlueId = vivid[rarity], PrismaticId = rigid[rarity], MetaId = metagem },
            	new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Hit - red agi
					RedId = glinting[rarity], YellowId = rigid[rarity], BlueId = vivid[rarity], PrismaticId = rigid[rarity], MetaId = metagem },
            	new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Hit - red ap
					RedId = pristine[rarity], YellowId = rigid[rarity], BlueId = vivid[rarity], PrismaticId = rigid[rarity], MetaId = metagem },
            	new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Hit - no colour match
					RedId = rigid[rarity], YellowId = rigid[rarity], BlueId = rigid[rarity], PrismaticId = rigid[rarity], MetaId = metagem },

                new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Attack Power - yellow hit
					RedId = bright[rarity], YellowId = pristine[rarity], BlueId = tear[rarity], PrismaticId = bright[rarity], MetaId = metagem },
                new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Attack Power - yellow haste
					RedId = bright[rarity], YellowId = stark[rarity], BlueId = tear[rarity], PrismaticId = bright[rarity], MetaId = metagem },
                new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Attack Power - yellow crit
					RedId = bright[rarity], YellowId = wicked[rarity], BlueId = tear[rarity], PrismaticId = bright[rarity], MetaId = metagem },
                new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Attack Power - no colour match
					RedId = bright[rarity], YellowId = bright[rarity], BlueId = bright[rarity], PrismaticId = bright[rarity], MetaId = metagem },

                new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Agility - yellow hit
					RedId = delicate[rarity], YellowId = glinting[rarity], BlueId = shifting[rarity], PrismaticId = delicate[rarity], MetaId = metagem },
                new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Agility - yellow haste
					RedId = delicate[rarity], YellowId = deft[rarity], BlueId = shifting[rarity], PrismaticId = delicate[rarity], MetaId = metagem },
                new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Agility - yellow crit
					RedId = delicate[rarity], YellowId = deadly[rarity], BlueId = shifting[rarity], PrismaticId = delicate[rarity], MetaId = metagem },
                new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Agility - no colour match
					RedId = delicate[rarity], YellowId = delicate[rarity], BlueId = delicate[rarity], PrismaticId = delicate[rarity], MetaId = metagem },

            	new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Crit - red agi
					RedId = deadly[rarity], YellowId = smooth[rarity], BlueId = jagged[rarity], PrismaticId = smooth[rarity], MetaId = metagem },
            	new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Crit - red ap
					RedId = wicked[rarity], YellowId = smooth[rarity], BlueId = jagged[rarity], PrismaticId = smooth[rarity], MetaId = metagem },
            	new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Crit - no colour match
					RedId = smooth[rarity], YellowId = smooth[rarity], BlueId = smooth[rarity], PrismaticId = smooth[rarity], MetaId = metagem },

                new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Haste - red agi
					RedId = deft[rarity], YellowId = quick[rarity], BlueId = forceful[rarity], PrismaticId = quick[rarity], MetaId = metagem },
                new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Haste - red ap
					RedId = stark[rarity], YellowId = quick[rarity], BlueId = forceful[rarity], PrismaticId = quick[rarity], MetaId = metagem },
                new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Haste - no colour match
					RedId = quick[rarity], YellowId = quick[rarity], BlueId = quick[rarity], PrismaticId = quick[rarity], MetaId = metagem },

                new GemmingTemplate() { Model = "Enhance", Group = group, Enabled = enabled,  //Max Armour Penetration - no colour match
					RedId = fractured[rarity], YellowId = fractured[rarity], BlueId = fractured[rarity], PrismaticId = fractured[rarity], MetaId = metagem },
            };
        }
    }
}
