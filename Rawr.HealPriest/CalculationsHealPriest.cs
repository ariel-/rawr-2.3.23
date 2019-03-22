using System;
using System.Collections.Generic;
#if RAWR3
using System.Windows.Media;
#else
using System.Drawing;
#endif
using Rawr;

namespace Rawr.HealPriest
{
    [Rawr.Calculations.RawrModelInfo("HealPriest", "Spell_Holy_Renew", CharacterClass.Priest)]
    public class CalculationsHealPriest : CalculationsBase 
    {
        public override List<GemmingTemplate> DefaultGemmingTemplates
        {
            get
            {
                // Interesting Gem Choices for a Holy & Discipline Priest
                // Red
                int[] runed = { 39911, 39998, 40113, 42144 }; // +spp

                // Orange
                int[] durable = { 39958, 40050, 40154 }; // +spp/+resilience
                int[] luminous = { 39946, 40047, 40151 }; // +spp/+int
                int[] potent = { 39956, 40048, 40152 }; // +spp/+crit
                int[] reckless = { 39959, 40051, 40155 }; // +spp/+haste
                //int[] veiled = { 39957, 40049, 40153 }; // +spp/+hit

                // Yellow
                int[] brilliant = { 39912, 40012, 40123, 42148 }; // +int
                int[] mystic = { 39917, 40016, 40127, 42158 }; // +resilience
                int[] quick = { 39918, 40017, 40128, 42150 }; // +haste
                // int[] rigid = { 39915, 40014, 40125, 42156 }; // +hit
                int[] smooth = { 39914, 40013, 40124, 42149 }; // +crit

                // Green
                int[] dazzling = { 39984, 40094, 40175 }; // +int/+mp5
                int[] energized = { 39989, 40105, 40179 }; // +haste/+mp5
                int[] forceful = { 39978, 40091, 40169 }; // +haste/+sta
                int[] intricate = { 39983, 40104, 40174 }; // +haste/+spi
                int[] jagged = { 39974, 40086, 40165 }; // +crit/+stamina
                // int[] lambent = { 39986, 40100, 40177 }; // +hit/+mp5
                int[] misty = { 39980, 40095, 40171 }; // +crit/+spi
                int[] opaque = { 39988, 40103, 40178 }; // +resilience/+mp5
                int[] seers = { 39979, 40092, 40170 }; // +int/+spi 
                // int[] shining = { 39981, 40099, 40172 }; // +hit/+spi
                int[] steady = { 39977, 40090, 40168 }; // +resilience/+stamina
                int[] sundered = { 39985, 40096, 40176 }; // +crit/+mp5
                int[] timeless = { 39968, 40085, 40164 }; // +int/+stamina
                int[] turbid = { 39982, 40102, 40173 }; // +resilience/+spi
                // int[] vivid = { 39975, 40088, 40166 }; // +hit/+stamina

                // Blue
                int[] lustrous = { 39927, 40010, 40121, 42146 }; // +mp5
                int[] solid = { 39919, 40008, 40119, 36767 }; // +stamina
                int[] sparkling = { 39920, 40009, 40120, 42145 }; // +spirit

                // Purple
                int[] glowing = { 39936, 40025, 40132 }; // +spp/+stamina
                int[] purified = { 39941, 40026, 40133 }; // +spp/+spirit
                int[] royal = { 39943, 40027, 40134 }; // +spp/+mp5

                // Meta
                int[] beaming = { 41389 }; // +crit rating/+2% mana
                int[] ember = { 41333 }; // +spp/+2% int
                int[] insightful = { 41401 }; // +int/manarestore
                int[] revitalizing = { 41376 }; // +mp5/+3% heal effect
                int[] powerful = { 41397 }; // +stamina/-10% stun duration
 
                return new List<GemmingTemplate>() {
                    new GemmingTemplate() { Model = "HealPriest", Group = "Uncommon", // Max Mana
                        RedId = luminous[0], YellowId = brilliant[0], BlueId = seers[0], PrismaticId = brilliant[0], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Uncommon", // Max Mana Regen
                        RedId = purified[0], YellowId = seers[0], BlueId = sparkling[0], PrismaticId = sparkling[0], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Uncommon", // Max SPP 
                        RedId = runed[0], YellowId = luminous[0], BlueId = purified[0], PrismaticId = runed[0], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Uncommon", // Max SPP w/ember
                        RedId = runed[0], YellowId = luminous[0], BlueId = purified[0], PrismaticId = runed[0], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Uncommon", // Max SPP w/revitalizing
                        RedId = runed[0], YellowId = luminous[0], BlueId = purified[0], PrismaticId = runed[0], MetaId = revitalizing[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Uncommon", // Max Haste
                        RedId = runed[0], YellowId = reckless[0], BlueId = intricate[0], PrismaticId = runed[0], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Uncommon", // Max Crit
                        RedId = runed[0], YellowId = potent[0], BlueId = misty[0], PrismaticId = runed[0], MetaId = beaming[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Uncommon", // Max Blast
                        RedId = runed[0], YellowId = runed[0], BlueId = runed[0], PrismaticId = runed[0], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Uncommon", // Max Blast w/ember
                        RedId = runed[0], YellowId = runed[0], BlueId = runed[0], PrismaticId = runed[0], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Uncommon", // PvP - Stamina
                        RedId = glowing[0], YellowId = steady[0], BlueId = solid[0], PrismaticId = solid[0], MetaId = powerful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Uncommon", // PvP - Resilience
                        RedId = durable[0], YellowId = mystic[0], BlueId = steady[0], PrismaticId = mystic[0], MetaId = powerful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Uncommon", // Max Mana w/MP5
                        RedId = luminous[0], YellowId = brilliant[0], BlueId = dazzling[0], PrismaticId = brilliant[0], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Uncommon", // Max Mana Regen
                        RedId = royal[0], YellowId = dazzling[0], BlueId = lustrous[0], PrismaticId = lustrous[0], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Uncommon", // Max SPP
                        RedId = runed[0], YellowId = luminous[0], BlueId = royal[0], PrismaticId = runed[0], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Uncommon", // Max SPP w/revitalizing
                        RedId = runed[0], YellowId = luminous[0], BlueId = royal[0], PrismaticId = runed[0], MetaId = revitalizing[0] },

                    new GemmingTemplate() { Model = "HealPriest", Group = "Rare", // Max Mana
                        RedId = luminous[1], YellowId = brilliant[1], BlueId = seers[1], PrismaticId = brilliant[1], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Rare", // Max Mana Regen
                        RedId = purified[1], YellowId = seers[1], BlueId = sparkling[1], PrismaticId = sparkling[1], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Rare", // Max SPP
                        RedId = runed[1], YellowId = luminous[1], BlueId = purified[1], PrismaticId = runed[1], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Rare", // Max SPP w/ember
                        RedId = runed[1], YellowId = luminous[1], BlueId = purified[1], PrismaticId = runed[1], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Rare", // Max SPP w/revitalizing
                        RedId = runed[1], YellowId = luminous[1], BlueId = purified[1], PrismaticId = runed[1], MetaId = revitalizing[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Rare", // Max Haste
                        RedId = runed[1], YellowId = reckless[1], BlueId = intricate[1], PrismaticId = runed[1], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Rare", // Max Crit
                        RedId = runed[1], YellowId = potent[1], BlueId = misty[1], PrismaticId = runed[1], MetaId = beaming[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Rare", // Max Blast
                        RedId = runed[1], YellowId = runed[1], BlueId = runed[1], PrismaticId = runed[1], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Rare", // Max Blast w/ember
                        RedId = runed[1], YellowId = runed[1], BlueId = runed[1], PrismaticId = runed[1], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Rare", // PvP - Stamina
                        RedId = glowing[1], YellowId = steady[1], BlueId = solid[1], PrismaticId = solid[1], MetaId = powerful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Rare", // PvP - Resilience
                        RedId = durable[1], YellowId = mystic[1], BlueId = steady[1], PrismaticId = mystic[1], MetaId = powerful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Rare", // Max Mana w/MP5
                        RedId = luminous[1], YellowId = brilliant[1], BlueId = dazzling[1], PrismaticId = brilliant[1], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Rare", // Max Mana Regen
                        RedId = royal[1], YellowId = dazzling[1], BlueId = lustrous[1], PrismaticId = lustrous[1], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Rare", // Max SPP
                        RedId = runed[1], YellowId = luminous[1], BlueId = royal[1], PrismaticId = runed[1], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Rare", // Max SPP w/revitalizing
                        RedId = runed[1], YellowId = luminous[1], BlueId = royal[1], PrismaticId = runed[1], MetaId = revitalizing[0] },


                    new GemmingTemplate() { Model = "HealPriest", Group = "Epic", Enabled = true, // Max Mana
                        RedId = luminous[2], YellowId = brilliant[2], BlueId = seers[2], PrismaticId = brilliant[2], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Epic", // Max Mana Regen
                        RedId = purified[2], YellowId = seers[2], BlueId = sparkling[2], PrismaticId = sparkling[2], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Epic", Enabled = true, // Max SPP
                        RedId = runed[2], YellowId = luminous[2], BlueId = purified[2], PrismaticId = runed[2], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Epic", Enabled = true, // Max SPP w/ember
                        RedId = runed[2], YellowId = luminous[2], BlueId = purified[2], PrismaticId = runed[2], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Epic", Enabled = true, // Max SPP w/revitalizing
                        RedId = runed[2], YellowId = luminous[2], BlueId = purified[2], PrismaticId = runed[2], MetaId = revitalizing[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Epic", // Max Haste
                        RedId = runed[2], YellowId = reckless[2], BlueId = intricate[2], PrismaticId = runed[2], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Epic", // Max Crit
                        RedId = runed[2], YellowId = potent[2], BlueId = misty[2], PrismaticId = runed[2], MetaId = beaming[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Epic", Enabled = true, // Max Blast
                        RedId = runed[2], YellowId = runed[2], BlueId = runed[2], PrismaticId = runed[2], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Epic", // Max Blast w/ember
                        RedId = runed[2], YellowId = runed[2], BlueId = runed[2], PrismaticId = runed[2], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Epic", // PvP - Stamina
                        RedId = glowing[2], YellowId = steady[2], BlueId = solid[2], PrismaticId = solid[2], MetaId = powerful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Epic", // PvP - Resilience
                        RedId = durable[2], YellowId = mystic[2], BlueId = steady[2], PrismaticId = mystic[2], MetaId = powerful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Epic", // Max Mana w/MP5
                        RedId = luminous[2], YellowId = brilliant[2], BlueId = dazzling[2], PrismaticId = brilliant[2], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Epic", // Max Mana Regen
                        RedId = royal[2], YellowId = dazzling[2], BlueId = lustrous[2], PrismaticId = lustrous[2], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Epic", // Max SPP
                        RedId = runed[2], YellowId = luminous[2], BlueId = royal[2], PrismaticId = runed[2], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Epic", // Max SPP w/revitalizing
                        RedId = runed[2], YellowId = luminous[2], BlueId = royal[2], PrismaticId = runed[2], MetaId = revitalizing[0] },


                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Rare", // Max Mana
                        RedId = luminous[1], YellowId = brilliant[3], BlueId = seers[1], PrismaticId = brilliant[1], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Rare", // Max Mana Regen
                        RedId = purified[1], YellowId = seers[1], BlueId = sparkling[3], PrismaticId = sparkling[1], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Rare", // Max SPP
                        RedId = runed[3], YellowId = luminous[1], BlueId = purified[1], PrismaticId = runed[1], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Rare", // Max SPP w/ember
                        RedId = runed[3], YellowId = luminous[1], BlueId = purified[1], PrismaticId = runed[1], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Rare", // Max SPP w/revitalizing
                        RedId = runed[3], YellowId = luminous[1], BlueId = purified[1], PrismaticId = runed[1], MetaId = revitalizing[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Rare", // Max Haste
                        RedId = runed[3], YellowId = reckless[1], BlueId = intricate[1], PrismaticId = runed[1], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Rare", // Max Crit
                        RedId = runed[3], YellowId = potent[1], BlueId = misty[1], PrismaticId = runed[1], MetaId = beaming[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Rare", // Max Blast
                        RedId = runed[3], YellowId = runed[3], BlueId = runed[3], PrismaticId = runed[3], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Rare", // Max Blast w/ember
                        RedId = runed[3], YellowId = runed[3], BlueId = runed[3], PrismaticId = runed[3], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Rare", // PvP - Stamina
                        RedId = glowing[1], YellowId = steady[1], BlueId = solid[3], PrismaticId = solid[1], MetaId = powerful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Rare", // PvP - Resilience
                        RedId = durable[1], YellowId = mystic[3], BlueId = steady[1], PrismaticId = mystic[1], MetaId = powerful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Rare", // Max Mana w/MP5
                        RedId = luminous[1], YellowId = brilliant[3], BlueId = dazzling[1], PrismaticId = brilliant[1], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Rare", // Max Mana Regen
                        RedId = royal[1], YellowId = dazzling[1], BlueId = lustrous[3], PrismaticId = lustrous[1], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Rare", // Max SPP
                        RedId = runed[3], YellowId = luminous[1], BlueId = royal[1], PrismaticId = runed[1], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Rare", // Max SPP w/revitalizing
                        RedId = runed[3], YellowId = luminous[1], BlueId = royal[1], PrismaticId = runed[1], MetaId = revitalizing[0] },


                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Epic", // Max Mana
                        RedId = luminous[2], YellowId = brilliant[3], BlueId = seers[2], PrismaticId = brilliant[2], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Epic", // Max Mana Regen
                        RedId = purified[2], YellowId = seers[2], BlueId = sparkling[3], PrismaticId = sparkling[2], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Epic", // Max SPP
                        RedId = runed[3], YellowId = luminous[2], BlueId = purified[2], PrismaticId = runed[2], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Epic", // Max SPP w/ember
                        RedId = runed[3], YellowId = luminous[2], BlueId = purified[2], PrismaticId = runed[2], MetaId = ember[0] }, 
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Epic", // Max SPP w/revitalizing
                        RedId = runed[3], YellowId = luminous[2], BlueId = purified[2], PrismaticId = runed[2], MetaId = revitalizing[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Epic", // Max Haste
                        RedId = runed[3], YellowId = reckless[2], BlueId = intricate[2], PrismaticId = runed[2], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Epic", // Max Crit
                        RedId = runed[3], YellowId = potent[2], BlueId = misty[2], PrismaticId = runed[2], MetaId = beaming[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Epic", // Max Blast
                        RedId = runed[3], YellowId = runed[3], BlueId = runed[3], PrismaticId = runed[3], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Epic", // Max Blast w/ember
                        RedId = runed[3], YellowId = runed[3], BlueId = runed[3], PrismaticId = runed[3], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Epic", // PvP - Stamina
                        RedId = glowing[2], YellowId = steady[2], BlueId = solid[3], PrismaticId = solid[2], MetaId = powerful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Epic", // PvP - Resilience
                        RedId = durable[2], YellowId = mystic[3], BlueId = steady[2], PrismaticId = mystic[2], MetaId = powerful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Epic", // Max Mana w/MP5
                        RedId = luminous[2], YellowId = brilliant[3], BlueId = dazzling[2], PrismaticId = brilliant[2], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Epic", // Max Mana Regen
                        RedId = royal[2], YellowId = dazzling[2], BlueId = lustrous[3], PrismaticId = lustrous[2], MetaId = insightful[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Epic", // Max SPP
                        RedId = runed[3], YellowId = luminous[2], BlueId = royal[2], PrismaticId = runed[2], MetaId = ember[0] },
                    new GemmingTemplate() { Model = "HealPriest", Group = "Jeweler Epic", // Max SPP w/revitalizing
                        RedId = runed[3], YellowId = luminous[2], BlueId = royal[2], PrismaticId = runed[2], MetaId = revitalizing[0] },
               
                };
            }
        }

        public override void SetDefaults(Character character)
        {
            character.ActiveBuffsAdd(("Inner Fire"));
            character.ActiveBuffsAdd(("Improved Moonkin Form"));
            character.ActiveBuffsAdd(("Tree of Life Aura"));
            character.ActiveBuffsAdd(("Arcane Intellect"));
            character.ActiveBuffsAdd(("Vampiric Touch"));
            character.ActiveBuffsAdd(("Mana Spring Totem"));
                character.ActiveBuffsAdd(("Restorative Totems"));
            character.ActiveBuffsAdd(("Moonkin Form"));
            character.ActiveBuffsAdd(("Wrath of Air Totem"));
            character.ActiveBuffsAdd(("Totem of Wrath (Spell Power)"));
            character.ActiveBuffsAdd(("Divine Spirit"));
            character.ActiveBuffsAdd(("Power Word: Fortitude"));
                character.ActiveBuffsAdd(("Improved Power Word: Fortitude"));
            character.ActiveBuffsAdd(("Mark of the Wild"));
                character.ActiveBuffsAdd(("Improved Mark of the Wild"));
            character.ActiveBuffsAdd(("Blessing of Kings"));
            character.ActiveBuffsAdd(("Shadow Protection"));
            character.ActiveBuffsAdd(("Flask of the Frost Wyrm"));
            character.ActiveBuffsAdd(("Spell Power Food"));
        }

        private static List<string> _relevantGlyphs;
        public override List<string> GetRelevantGlyphs()
        {
            if (_relevantGlyphs == null)
            {
                _relevantGlyphs = new List<string>();
                _relevantGlyphs.Add("Glyph of Circle of Healing");
                _relevantGlyphs.Add("Glyph of Flash Heal");
                _relevantGlyphs.Add("Glyph of Guardian Spirit");
                _relevantGlyphs.Add("Glyph of Holy Nova");
                _relevantGlyphs.Add("Glyph of Hymn of Hope");
                _relevantGlyphs.Add("Glyph of Inner Fire");
                _relevantGlyphs.Add("Glyph of Lightwell");
                _relevantGlyphs.Add("Glyph of Mass Dispel");
                _relevantGlyphs.Add("Glyph of Penance");
                _relevantGlyphs.Add("Glyph of Power Word: Shield");
                _relevantGlyphs.Add("Glyph of Prayer of Healing");
                _relevantGlyphs.Add("Glyph of Renew");
                _relevantGlyphs.Add("Glyph of Fading");

            }
            return _relevantGlyphs;
        }

        public override CharacterClass TargetClass { get { return CharacterClass.Priest; } }

        private string _currentChartName = null;
        private float _currentChartTotal = 0;
        
        private Dictionary<string, Color> _subPointNameColors = null;
        public override Dictionary<string, Color> SubPointNameColors
        {
            get
            {
                _subPointNameColors = new Dictionary<string, Color>();
                switch (_currentChartName)
                {
                    case "MP5 Sources":
                        _subPointNameColors.Add(string.Format("MP5 Sources ({0} total)", _currentChartTotal.ToString("0")), Color.FromArgb(255, 0, 0, 255));
                        break;
                    case "Spell HpS":
                        _subPointNameColors.Add("HpS", Color.FromArgb(255, 255, 0, 0));
                        break;
                    case "Spell HpM":
                        _subPointNameColors.Add("HpM", Color.FromArgb(255, 255, 0, 0));
                        break;
                    default:
                        _subPointNameColors.Add("HPS-Burst", Color.FromArgb(255, 255, 0, 0));
                        _subPointNameColors.Add("HPS-Sustained", Color.FromArgb(255, 0, 0, 255));
                        _subPointNameColors.Add("Survivability", Color.FromArgb(255, 0, 128, 0));
                        break;
                }
                _currentChartName = null;
                return _subPointNameColors;
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
                    "Basic Stats:Resilience",
                    "Basic Stats:Intellect",
                    "Basic Stats:Spirit",
                    "Basic Stats:Spell Power",
                    "Basic Stats:In FSR MP5",
                    "Basic Stats:Spell Crit",
                    "Basic Stats:Healing Crit",
                    "Basic Stats:Spell Haste",
                    "Basic Stats:Armor",
                    "Basic Stats:Resistance",
                    "Simulation:Role",
                    "Simulation:Burst*This is the HPS you are expected to have if you are not limited by Mana.\r\nIn Custom Role, this displays your HPS when you dump all spells in 1 stream.",
                    "Simulation:Sustained*This is the HPS are expected to have when restricted by Mana.\r\nIf this value is lower than your Burst HPS, you are running out of mana in the simulation.\r\nIn Custom Role, this displays your HPS over the length of the fight, adjusted by the amount of mana available.",
                    "Spells:Greater Heal",
                    "Spells:Flash Heal",
                    "Spells:Binding Heal",
                    "Spells:Renew",
                    "Spells:Prayer of Mending",
                    "Spells:Power Word Shield",
                    "Spells:PoH",
                    "Spells:Holy Nova",
                    "Spells:Lightwell",
                    "Spells:CoH",
                    "Spells:Penance",
                    "Spells:Gift of the Naaru",
                    "Spells:Divine Hymn",
                    "Spells:Resurrection",
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
                    "Resilience",
                    "Mana",
                    "InFSR Regen",
                    "OutFSR Regen",
                    "Haste Rating",
                    "Haste %",
                    "Crit Rating",
                    "Healing Crit %",
                    "PW:Shield",
                    "GHeal Avg",
                    "FHeal Avg",
                    "CoH Avg",
                    "Armor",
                    "Arcane Resistance",
                    "Fire Resistance",
                    "Frost Resistance",
                    "Nature Resistance",
                    "Shadow Resistance",
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
            get {
                if (_calculationOptionsPanel == null)
                {
                    _calculationOptionsPanel = new CalculationOptionsPanelHealPriest();
                }
                return _calculationOptionsPanel;
            }
        }

        private string[] _customChartNames = null;
        public override string[] CustomChartNames
        {
            get
            {
                if (_customChartNames == null)
                    _customChartNames = new string[] { "MP5 Sources", "Spell HpS", "Spell HpM", "Spell AoE HpS", "Spell AoE HpM"}; //, "Relative Stat Values" };
                return _customChartNames;
            }
        }

        public override ComparisonCalculationBase CreateNewComparisonCalculation() { return new ComparisonCalculationHealPriest(); }
        public override CharacterCalculationsBase CreateNewCharacterCalculations() { return new CharacterCalculationsHealPriest(); }

        private List<ItemType> _relevantItemTypes = null;
        public override List<ItemType> RelevantItemTypes
        {
            get {
                if (_relevantItemTypes == null) {
                    _relevantItemTypes = new List<ItemType>(new ItemType[]{
                        ItemType.None,
                        ItemType.Cloth,
                        ItemType.Dagger,
                        ItemType.Wand,
                        ItemType.OneHandMace,
                        ItemType.Staff
                    });
                }
                return _relevantItemTypes;
            }
        }

        public override bool EnchantFitsInSlot(Enchant enchant, Character character, ItemSlot slot)
        {
            if (slot == ItemSlot.OffHand || slot == ItemSlot.Ranged) return false;
            return base.EnchantFitsInSlot(enchant, character, slot);
        }

        public override bool ItemFitsInSlot(Item item, Character character, CharacterSlot slot, bool ignoreUnique)
        {
            if (slot == CharacterSlot.OffHand && item.Slot == ItemSlot.OneHand) return false;
            return base.ItemFitsInSlot(item, character, slot, ignoreUnique);
        }

        public override CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, bool referenceCalculation, bool significantChange, bool needsDisplayCalculations)
        {
            Stats stats = GetCharacterStats(character, additionalItem);
            Stats statsRace = BaseStats.GetBaseStats(character);  // GetRaceStats(character);
            CharacterCalculationsHealPriest calculatedStats = new CharacterCalculationsHealPriest();
            CalculationOptionsHealPriest calculationOptions = character.CalculationOptions as CalculationOptionsHealPriest;
            if (calculationOptions == null)
                return null;

            calculatedStats.Race = character.Race;
            calculatedStats.BasicStats = stats;
            calculatedStats.Character = character;

            calculatedStats.SpiritRegen = (float)Math.Floor(5 * StatConversion.GetSpiritRegenSec(calculatedStats.BasicStats.Spirit, calculatedStats.BasicStats.Intellect));
            calculatedStats.RegenInFSR = calculatedStats.SpiritRegen * calculatedStats.BasicStats.SpellCombatManaRegeneration;
            calculatedStats.RegenOutFSR = calculatedStats.SpiritRegen;

            BaseSolver solver;
            if (calculationOptions.Role == eRole.CUSTOM)
                solver = new AdvancedSolver(stats, character);
            else
                solver = new Solver(stats, character);
            solver.Calculate(calculatedStats);

            return calculatedStats;
        }

        public static float GetInnerFireSpellPowerBonus(Character character)
        {
            float InnerFireSpellPowerBonus = 0;
            if (character.Level >= 77)
                InnerFireSpellPowerBonus = 120;
            else if (character.Level >= 71)
                InnerFireSpellPowerBonus = 95;
            return InnerFireSpellPowerBonus * (1f + character.PriestTalents.ImprovedInnerFire * 0.15f);
        }

        public static float GetInnerFireArmorBonus(Character character)
        {
            float ArmorBonus = 2440 * (character.PriestTalents.GlyphofInnerFire ? 1.5f : 1f);

            return ArmorBonus * (1f + character.PriestTalents.ImprovedInnerFire * 0.15f);
        }

        public override Stats GetCharacterStats(Character character, Item additionalItem)
        {
            CalculationOptionsHealPriest calcOpts = character.CalculationOptions as CalculationOptionsHealPriest;
            Stats statsRace = BaseStats.GetBaseStats(character);
            Stats statsBaseGear = GetItemStats(character, additionalItem);
            //Stats statsEnchants = GetEnchantsStats(character);
            Stats statsBuffs = GetBuffsStats(character, calcOpts);

            Stats statsTalents = new Stats()
            {
                BonusStaminaMultiplier = character.PriestTalents.ImprovedPowerWordFortitude * 0.02f,
                BonusSpiritMultiplier = (1 + character.PriestTalents.Enlightenment * 0.02f) * (1f + character.PriestTalents.SpiritOfRedemption * 0.05f) - 1f,
                BonusIntellectMultiplier = character.PriestTalents.MentalStrength * 0.03f,
                SpellDamageFromSpiritPercentage = character.PriestTalents.SpiritualGuidance * 0.05f + character.PriestTalents.TwistedFaith * 0.02f,
                SpellHaste = character.PriestTalents.Enlightenment * 0.02f,
                SpellCombatManaRegeneration = character.PriestTalents.Meditation * 0.5f / 3f,
                SpellCrit = character.PriestTalents.FocusedWill * 0.01f,
            };

            Stats statsTotal = statsBaseGear + statsBuffs + statsRace + statsTalents;

            statsTotal.Stamina = (float)Math.Floor((statsTotal.Stamina) * (1 + statsTotal.BonusStaminaMultiplier));
            statsTotal.Intellect = (float)Math.Floor(statsTotal.Intellect * (1 + statsTotal.BonusIntellectMultiplier));
            statsTotal.Spirit = (float)Math.Floor((statsTotal.Spirit) * (1 + statsTotal.BonusSpiritMultiplier));
            statsTotal.SpellPower += statsTotal.SpellDamageFromSpiritPercentage * statsTotal.Spirit
                + (statsTotal.PriestInnerFire > 0 ? GetInnerFireSpellPowerBonus(character) : 0);
            statsTotal.Mana += StatConversion.GetManaFromIntellect(statsTotal.Intellect);
            statsTotal.Mana *= (1f + statsTotal.BonusManaMultiplier);
            statsTotal.Health += StatConversion.GetHealthFromStamina(statsTotal.Stamina);
            statsTotal.Health = (float)Math.Floor(statsTotal.Health * (1f + statsTotal.BonusHealthMultiplier));
            statsTotal.SpellCrit += StatConversion.GetSpellCritFromIntellect(statsTotal.Intellect)
                + StatConversion.GetSpellCritFromRating(statsTotal.CritRating);
            statsTotal.SpellHaste = (1f + statsTotal.SpellHaste) * (1f + StatConversion.GetSpellHasteFromRating(statsTotal.HasteRating)) - 1f;
            statsTotal.BonusArmor += statsTotal.Agility * 2f + (statsTotal.PriestInnerFire > 0 ? GetInnerFireArmorBonus(character) : 0);    
            return statsTotal;
        }

        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
        {
            List<ComparisonCalculationBase> comparisonList = new List<ComparisonCalculationBase>();
            ComparisonCalculationBase comparison;
            CharacterCalculationsHealPriest p;
            List<Spell> spellList;

            _currentChartTotal = 0;
            switch (chartName)
            {
                case "MP5 Sources":
                    _currentChartName = chartName;
                    CharacterCalculationsHealPriest mscalcs = GetCharacterCalculations(character) as CharacterCalculationsHealPriest;
                    BaseSolver mssolver;
                    if ((character.CalculationOptions as CalculationOptionsHealPriest).Role == eRole.CUSTOM)
                        mssolver = new AdvancedSolver(mscalcs.BasicStats, character);
                    else
                        mssolver = new Solver(mscalcs.BasicStats, character);
                    mssolver.Calculate(mscalcs);
                    foreach (Solver.ManaSource Source in mssolver.ManaSources)
                    {
                        comparison = CreateNewComparisonCalculation();
                        comparison.Name = Source.Name;
                        comparison.SubPoints[0] = Source.Value * 5;
                        _currentChartTotal += comparison.SubPoints[0];
                        comparison.OverallPoints = comparison.SubPoints[0];
                        comparison.Equipped = false;
                        comparisonList.Add(comparison);
                    }
                    return comparisonList.ToArray();
                case "Spell AoE HpS":
                    _currentChartName = chartName;
                    p = GetCharacterCalculations(character) as CharacterCalculationsHealPriest;
                    spellList = new List<Spell>();
                    spellList.Add(new Renew(p.BasicStats, character));
                    spellList.Add(new FlashHeal(p.BasicStats, character));
                    spellList.Add(new Heal(p.BasicStats, character));
                    spellList.Add(new PrayerOfHealing(p.BasicStats, character, 5));
                    spellList.Add(new BindingHeal(p.BasicStats, character));
                    spellList.Add(new PrayerOfMending(p.BasicStats, character, 5));
                    spellList.Add(new CircleOfHealing(p.BasicStats, character, 5));
                    spellList.Add(new HolyNova(p.BasicStats, character, 5));
                    spellList.Add(new Penance(p.BasicStats, character));
                    spellList.Add(new PowerWordShield(p.BasicStats, character));

                    foreach (Spell spell in spellList)
                    {
                        if(spell.AvgHeal == 0)
                            continue;

                        comparison = CreateNewComparisonCalculation();
                        comparison.Name = spell.Name;
                        comparison.Equipped = false;
                        comparison.SubPoints[0] = spell.HpS;
                        comparison.OverallPoints = comparison.SubPoints[0];
                        comparisonList.Add(comparison);
                    }

                    return comparisonList.ToArray();
                case "Spell AoE HpM":
                    _currentChartName = chartName;
                    p = GetCharacterCalculations(character) as CharacterCalculationsHealPriest;
                    spellList = new List<Spell>();
                    spellList.Add(new Renew(p.BasicStats, character));
                    spellList.Add(new FlashHeal(p.BasicStats, character));
                    spellList.Add(new Heal(p.BasicStats, character));
                    spellList.Add(new PrayerOfHealing(p.BasicStats, character, 5));
                    spellList.Add(new BindingHeal(p.BasicStats, character));
                    spellList.Add(new PrayerOfMending(p.BasicStats, character, 5));
                    spellList.Add(new CircleOfHealing(p.BasicStats, character, 5));
                    spellList.Add(new HolyNova(p.BasicStats, character, 5));
                    spellList.Add(new Penance(p.BasicStats, character));
                    spellList.Add(new PowerWordShield(p.BasicStats, character));

                    foreach (Spell spell in spellList)
                    {
                        if (spell.AvgHeal == 0)
                            continue;

                        comparison = CreateNewComparisonCalculation();
                        comparison.Name = spell.Name;
                        comparison.Equipped = false;
                        comparison.SubPoints[0] = spell.HpM;
                        comparison.OverallPoints = comparison.SubPoints[0];
                        comparisonList.Add(comparison);
                    }

                    return comparisonList.ToArray();
                case "Spell HpS":
                    _currentChartName = chartName;
                    p = GetCharacterCalculations(character) as CharacterCalculationsHealPriest;
                    spellList = new List<Spell>();
                    spellList.Add(new Renew(p.BasicStats, character));
                    spellList.Add(new FlashHeal(p.BasicStats, character));
                    spellList.Add(new Heal(p.BasicStats, character));
                    spellList.Add(new PrayerOfHealing(p.BasicStats, character, 1));
                    spellList.Add(new BindingHeal(p.BasicStats, character));
                    spellList.Add(new PrayerOfMending(p.BasicStats, character, 1));
                    spellList.Add(new CircleOfHealing(p.BasicStats, character, 1));
                    spellList.Add(new HolyNova(p.BasicStats, character, 1));
                    spellList.Add(new Penance(p.BasicStats, character));
                    spellList.Add(new PowerWordShield(p.BasicStats, character));

                    foreach (Spell spell in spellList)
                    {
                        if (spell.AvgHeal == 0)
                            continue;

                        comparison = CreateNewComparisonCalculation();
                        comparison.Name = spell.Name;
                        comparison.Equipped = false;
                        comparison.SubPoints[0] = spell.HpS;
                        comparison.OverallPoints = comparison.SubPoints[0];
                        comparisonList.Add(comparison);
                    }

                    return comparisonList.ToArray();
                case "Spell HpM":
                    _currentChartName = chartName;
                    p = GetCharacterCalculations(character) as CharacterCalculationsHealPriest;
                    spellList = new List<Spell>();
                    spellList.Add(new Renew(p.BasicStats, character));
                    spellList.Add(new FlashHeal(p.BasicStats, character));
                    spellList.Add(new Heal(p.BasicStats, character));
                    spellList.Add(new PrayerOfHealing(p.BasicStats, character, 1));
                    spellList.Add(new BindingHeal(p.BasicStats, character));
                    spellList.Add(new PrayerOfMending(p.BasicStats, character, 1));
                    spellList.Add(new CircleOfHealing(p.BasicStats, character, 1));
                    spellList.Add(new HolyNova(p.BasicStats, character, 1));
                    spellList.Add(new Penance(p.BasicStats, character));
                    spellList.Add(new PowerWordShield(p.BasicStats, character));

                    foreach (Spell spell in spellList)
                    {
                        if (spell.AvgHeal == 0)
                            continue;

                        comparison = CreateNewComparisonCalculation();
                        comparison.Name = spell.Name;
                        comparison.Equipped = false;
                        comparison.SubPoints[0] = spell.HpM;
                        comparison.OverallPoints = comparison.SubPoints[0];
                        comparisonList.Add(comparison);
                    }

                    return comparisonList.ToArray();

                case "Relative Stat Values":
                    _currentChartName = chartName;
                    CharacterCalculationsHealPriest calcsBase = GetCharacterCalculations(character) as CharacterCalculationsHealPriest;
                    CharacterCalculationsHealPriest calcsIntellect = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Intellect = 50 } }) as CharacterCalculationsHealPriest;
                    CharacterCalculationsHealPriest calcsSpirit = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Spirit = 50 } }) as CharacterCalculationsHealPriest;
                    CharacterCalculationsHealPriest calcsMP5 = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Mp5 = 50 } }) as CharacterCalculationsHealPriest;
                    CharacterCalculationsHealPriest calcsSpellPower = GetCharacterCalculations(character, new Item() { Stats = new Stats() { SpellPower = 50 } }) as CharacterCalculationsHealPriest;
                    CharacterCalculationsHealPriest calcsHaste = GetCharacterCalculations(character, new Item() { Stats = new Stats() { HasteRating = 50 } }) as CharacterCalculationsHealPriest;
                    CharacterCalculationsHealPriest calcsCrit = GetCharacterCalculations(character, new Item() { Stats = new Stats() { CritRating = 50 } }) as CharacterCalculationsHealPriest;
                    CharacterCalculationsHealPriest calcsSta = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Stamina = 50 } }) as CharacterCalculationsHealPriest;
                    CharacterCalculationsHealPriest calcsRes = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Resilience = 50 } }) as CharacterCalculationsHealPriest;

                    return new ComparisonCalculationBase[] {
                        new ComparisonCalculationHealPriest() { Name = "1 Intellect",
                            OverallPoints = (calcsIntellect.OverallPoints - calcsBase.OverallPoints) / 50,
                            HealPoints = (calcsIntellect.SubPoints[0] - calcsBase.SubPoints[0]) / 50,
                            RegenPoints = (calcsIntellect.SubPoints[1] - calcsBase.SubPoints[1]) / 50,
                            HastePoints = (calcsIntellect.SubPoints[2] - calcsBase.SubPoints[2]) / 50
                        },
                        new ComparisonCalculationHealPriest() { Name = "1 Spirit",
                            OverallPoints = (calcsSpirit.OverallPoints - calcsBase.OverallPoints) / 50,
                            HealPoints = (calcsSpirit.SubPoints[0] - calcsBase.SubPoints[0]) / 50,
                            RegenPoints = (calcsSpirit.SubPoints[1] - calcsBase.SubPoints[1]) / 50,
                            HastePoints = (calcsSpirit.SubPoints[2] - calcsBase.SubPoints[2]) / 50
                        },
                        new ComparisonCalculationHealPriest() { Name = "1 MP5",
                            OverallPoints = (calcsMP5.OverallPoints - calcsBase.OverallPoints) / 50,
                            HealPoints = (calcsMP5.SubPoints[0] - calcsBase.SubPoints[0]) / 50,
                            RegenPoints = (calcsMP5.SubPoints[1] - calcsBase.SubPoints[1]) / 50,
                            HastePoints = (calcsMP5.SubPoints[2] - calcsBase.SubPoints[2]) / 50
                        },
                        new ComparisonCalculationHealPriest() { Name = "1 Spell Power",
                            OverallPoints = (calcsSpellPower.OverallPoints - calcsBase.OverallPoints) / 50,
                            HealPoints = (calcsSpellPower.SubPoints[0] - calcsBase.SubPoints[0]) / 50,
                            RegenPoints = (calcsSpellPower.SubPoints[1] - calcsBase.SubPoints[1]) / 50,
                            HastePoints = (calcsSpellPower.SubPoints[2] - calcsBase.SubPoints[2]) / 50
                        },
                        new ComparisonCalculationHealPriest() { Name = "1 Haste",
                            OverallPoints = (calcsHaste.OverallPoints - calcsBase.OverallPoints) / 50,
                            HealPoints = (calcsHaste.SubPoints[0] - calcsBase.SubPoints[0]) / 50,
                            RegenPoints = (calcsHaste.SubPoints[1] - calcsBase.SubPoints[1]) / 50,
                            HastePoints = (calcsHaste.SubPoints[2] - calcsBase.SubPoints[2]) / 50
                        },
                        new ComparisonCalculationHealPriest() { Name = "1 Crit",
                            OverallPoints = (calcsCrit.OverallPoints - calcsBase.OverallPoints) / 50,
                            HealPoints = (calcsCrit.SubPoints[0] - calcsBase.SubPoints[0]) / 50,
                            RegenPoints = (calcsCrit.SubPoints[1] - calcsBase.SubPoints[1]) / 50,
                            HastePoints = (calcsCrit.SubPoints[2] - calcsBase.SubPoints[2]) / 50
                        },
                        new ComparisonCalculationHealPriest() { Name = "1 Stamina",
                            OverallPoints = (calcsSta.OverallPoints - calcsBase.OverallPoints) / 50,
                            HealPoints = (calcsSta.SubPoints[0] - calcsBase.SubPoints[0]) / 50,
                            RegenPoints = (calcsSta.SubPoints[1] - calcsBase.SubPoints[1]) / 50,
                            HastePoints = (calcsSta.SubPoints[2] - calcsBase.SubPoints[2]) / 50
                        },
                        new ComparisonCalculationHealPriest() { Name = "1 Resilience",
                            OverallPoints = (calcsRes.OverallPoints - calcsBase.OverallPoints) / 50,
                            HealPoints = (calcsRes.SubPoints[0] - calcsBase.SubPoints[0]) / 50,
                            RegenPoints = (calcsRes.SubPoints[1] - calcsBase.SubPoints[1]) / 50,
                            HastePoints = (calcsRes.SubPoints[2] - calcsBase.SubPoints[2]) / 50
                        }};
                default:
                    _currentChartName = null;
                    return new ComparisonCalculationBase[0];
            }
        }

        public override Stats GetRelevantStats(Stats stats)
        {
            Stats s = new Stats()
            {
                Stamina = stats.Stamina,
                Intellect = stats.Intellect,
                Spirit = stats.Spirit,
                Health = stats.Health,
                Mana = stats.Mana,
                Mp5 = stats.Mp5,
                SpellPower = stats.SpellPower,

                SpellHaste = stats.SpellHaste,
                SpellCrit = stats.SpellCrit,

                Resilience = stats.Resilience,
                CritRating = stats.CritRating,
                HasteRating = stats.HasteRating,
                
                BonusIntellectMultiplier = stats.BonusIntellectMultiplier,
                BonusSpiritMultiplier = stats.BonusSpiritMultiplier,
                BonusManaMultiplier = stats.BonusManaMultiplier,
                BonusCritHealMultiplier = stats.BonusCritHealMultiplier,

                SpellDamageFromSpiritPercentage = stats.SpellDamageFromSpiritPercentage,
                HealingReceivedMultiplier = stats.HealingReceivedMultiplier,
                BonusHealingDoneMultiplier = stats.BonusHealingDoneMultiplier,
                BonusHealthMultiplier = stats.BonusHealthMultiplier,
                BonusManaPotion = stats.BonusManaPotion,
                SpellCombatManaRegeneration = stats.SpellCombatManaRegeneration,
                ManaRestoreFromMaxManaPerSecond = stats.ManaRestoreFromMaxManaPerSecond,
                PriestInnerFire = stats.PriestInnerFire,
                Healed = stats.Healed,

                ManaRestore = stats.ManaRestore,
                SpellsManaReduction = stats.SpellsManaReduction,
                HighestStat = stats.HighestStat,
                ShieldFromHealed = stats.ShieldFromHealed,
                ManaGainOnGreaterHealOverheal = stats.ManaGainOnGreaterHealOverheal,
                RenewDurationIncrease = stats.RenewDurationIncrease,
                BonusPoHManaCostReductionMultiplier = stats.BonusPoHManaCostReductionMultiplier,
                BonusGHHealingMultiplier = stats.BonusGHHealingMultiplier,
                PrayerOfMendingExtraJumps = stats.PrayerOfMendingExtraJumps,
                GreaterHealCostReduction = stats.GreaterHealCostReduction,
                WeakenedSoulDurationDecrease = stats.WeakenedSoulDurationDecrease,
                PrayerOfHealingExtraCrit = stats.PrayerOfHealingExtraCrit,
                PWSBonusSpellPowerProc = stats.PWSBonusSpellPowerProc,
                PriestHeal_T9_2pc = stats.PriestHeal_T9_2pc,
                PriestHeal_T9_4pc = stats.PriestHeal_T9_4pc,
                PriestHeal_T10_2pc = stats.PriestHeal_T10_2pc,
                PriestHeal_T10_4pc = stats.PriestHeal_T10_4pc,
                /*
                ManaregenFor8SecOnUse5Min = stats.ManaregenFor8SecOnUse5Min,
                SpellPowerFor20SecOnUse2Min = stats.SpellPowerFor20SecOnUse2Min,
                SpellPowerFor15SecOnUse90Sec = stats.SpellPowerFor15SecOnUse90Sec,
                SpiritFor20SecOnUse2Min = stats.SpiritFor20SecOnUse2Min,
                HasteRatingFor20SecOnUse2Min = stats.HasteRatingFor20SecOnUse2Min,
                Mp5OnCastFor20SecOnUse2Min = stats.Mp5OnCastFor20SecOnUse2Min,
                ManaregenOver12SecOnUse3Min = stats.ManaregenOver12SecOnUse3Min,
                ManaregenOver12SecOnUse5Min = stats.ManaregenOver12SecOnUse5Min,
                ManacostReduceWithin15OnHealingCast = stats.ManacostReduceWithin15OnHealingCast,
                FullManaRegenFor15SecOnSpellcast = stats.FullManaRegenFor15SecOnSpellcast,
                BangleProc = stats.BangleProc,
                SpellHasteFor10SecOnCast_10_45 = stats.SpellHasteFor10SecOnCast_10_45,
                ManaRestoreOnCrit_25_45 = stats.ManaRestoreOnCrit_25_45,
                ManaRestoreOnCast_10_45 = stats.ManaRestoreOnCast_10_45,*/

                Armor = stats.Armor,
                BonusArmor = stats.BonusArmor,
                Agility = stats.Agility,
                ArcaneResistance = stats.ArcaneResistance,
                ArcaneResistanceBuff = stats.ArcaneResistanceBuff,
                FireResistance = stats.FireResistance,
                FireResistanceBuff = stats.FireResistanceBuff,
                FrostResistance = stats.FrostResistance,
                FrostResistanceBuff = stats.FrostResistanceBuff,
                NatureResistance = stats.NatureResistance,
                NatureResistanceBuff = stats.NatureResistanceBuff,
                ShadowResistance = stats.ShadowResistance,
                ShadowResistanceBuff = stats.ShadowResistanceBuff,
            };

            foreach (SpecialEffect se in stats.SpecialEffects())
                if (RelevantTrinket(se))
                    s.AddSpecialEffect(se);
            return s;
        }

        protected bool RelevantTrinket(SpecialEffect se)
        {
            if (se.Trigger == Trigger.HealingSpellCast
                || se.Trigger == Trigger.HealingSpellCrit
                || se.Trigger == Trigger.HealingSpellHit
                || se.Trigger == Trigger.SpellCast
                || se.Trigger == Trigger.SpellCrit
                || se.Trigger == Trigger.Use)
            {
                return _HasRelevantStats(se.Stats) || (se.Stats._rawSpecialEffectDataSize > 0 && _HasRelevantStats(se.Stats._rawSpecialEffectData[0].Stats));
            }
            return false;
        }

        // Trinket Status:
        // Correctly implemented:
        // http://www.wowhead.com/?item=40382 - Soul of the Dead
        
        // Wrongly implemented:
        // http://www.wowhead.com/?item=37835 - Je'Tze's Bell

        // Not implemented:
        // http://www.wowhead.com/?item=44253 - Darkmoon Card: Greatness (But for 90 spi/90 int versions, not yet known)
        // http://www.wowhead.com/?item=42988 - Darkmoon Card: Illusion
        // http://www.wowhead.com/?item=40258 - Forethought Talisman
        // http://www.wowhead.com/?item=40432 - Illustration of the Dragon Soul
        // http://www.wowhead.com/?item=40532 - Living Ice Crystals
        // http://www.wowhead.com/?item=40430 - Majestic Dragon Figurine

        protected bool _HasRelevantStats(Stats stats)
        {
            bool Yes = (
                stats.Intellect + stats.Spirit + stats.Mana + stats.Mp5 + stats.SpellPower
                + stats.SpellHaste + stats.SpellCrit
                + stats.HasteRating + stats.CritRating
                + stats.BonusIntellectMultiplier + stats.BonusSpiritMultiplier + stats.BonusManaMultiplier + stats.BonusCritHealMultiplier
                + stats.SpellDamageFromSpiritPercentage + stats.HealingReceivedMultiplier + stats.BonusHealingDoneMultiplier + stats.BonusManaPotion
                + stats.SpellCombatManaRegeneration + stats.ManaRestoreFromMaxManaPerSecond + stats.PriestInnerFire
                + stats.Healed

                + stats.ManaGainOnGreaterHealOverheal + stats.RenewDurationIncrease
                + stats.PrayerOfHealingExtraCrit + stats.PWSBonusSpellPowerProc

                + stats.BonusPoHManaCostReductionMultiplier + stats.BonusGHHealingMultiplier
                + stats.PrayerOfMendingExtraJumps + stats.GreaterHealCostReduction
                + stats.WeakenedSoulDurationDecrease
                + stats.PriestHeal_T9_2pc + stats.PriestHeal_T9_4pc
                + stats.PriestHeal_T10_2pc + stats.PriestHeal_T10_4pc

                + stats.ManaRestore + stats.SpellsManaReduction + stats.HighestStat
                + stats.ShieldFromHealed
                /*+ stats.ManaregenFor8SecOnUse5Min + stats.SpellPowerFor20SecOnUse2Min
                + stats.SpellPowerFor15SecOnUse90Sec + stats.SpiritFor20SecOnUse2Min + stats.HasteRatingFor20SecOnUse2Min
                + stats.Mp5OnCastFor20SecOnUse2Min + stats.ManaregenOver12SecOnUse3Min + stats.ManaregenOver12SecOnUse5Min
                + stats.ManacostReduceWithin15OnHealingCast + stats.FullManaRegenFor15SecOnSpellcast
                + stats.BangleProc + stats.SpellHasteFor10SecOnCast_10_45 + stats.ManaRestoreOnCrit_25_45
                + stats.ManaRestoreOnCast_10_45*/
            ) > 0;

            bool Maybe = (
                stats.Stamina + stats.Health + stats.Resilience
                + stats.Armor + stats.BonusArmor + stats.Agility +
                +stats.SpellHit + stats.HitRating
                + stats.ArcaneResistance + stats.ArcaneResistanceBuff
                + stats.FireResistance + stats.FireResistanceBuff
                + stats.FrostResistance + stats.FrostResistanceBuff
                + stats.NatureResistance + stats.NatureResistanceBuff
                + stats.ShadowResistance + stats.ShadowResistanceBuff
            ) > 0;

            bool No = (
                stats.Strength + stats.AttackPower
                + stats.ArmorPenetration + stats.ArmorPenetrationRating
                + stats.ExpertiseRating
                + stats.Dodge + stats.DodgeRating
                + stats.Parry + stats.ParryRating
                + stats.Defense + stats.DefenseRating
                + stats.PhysicalHit
            ) > 0;

            return Yes || (Maybe && !No);
        }

        public override bool HasRelevantStats(Stats stats)
        {
            bool isRelevant = _HasRelevantStats(stats);

            foreach (SpecialEffect se in stats.SpecialEffects())
                isRelevant |= RelevantTrinket(se);
            return isRelevant;
        }

        public Stats GetBuffsStats(Character character, CalculationOptionsHealPriest calcOpts) {
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

        public override ICalculationOptionBase DeserializeDataObject(string xml)
        {
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsHealPriest));
            System.IO.StringReader reader = new System.IO.StringReader(xml.Replace("CalculationOptionsPriest", "CalculationOptionsHealPriest").Replace("CalculationOptionsHolyPriest", "CalculationOptionsHealPriest"));
            CalculationOptionsHealPriest calcOpts = serializer.Deserialize(reader) as CalculationOptionsHealPriest;
            return calcOpts;
        }
    }
}
