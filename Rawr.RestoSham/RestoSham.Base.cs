using System.Collections.Generic;
#if RAWR3
using System.Windows.Media;
#else
using System.Drawing;
#endif

namespace Rawr.RestoSham
{
    /// <summary>
    /// Contains various lists of relevant stats, armor types, etc
    /// </summary>
    public static class Relevants
    {
        public static string[] RelevantStats = { 
            "Stamina",
            "Intellect",
            "SpellPower",
            "CritRating",
            "HasteRating",
            "SpellCrit",
            "SpellHaste",
            "Mana",
            "Mp5",
            "Earthliving",
            "ManacostReduceWithin15OnHealingCast",
            "ManaRestoreFromMaxManaPerSecond",
            //"BonusManaPotion",                            Mana pots are not currently modeled by RestoSham
            "WaterShieldIncrease",
            "CHHWHealIncrease",
            "CHCTDecrease",
            "RTCDDecrease",
            "TotemCHBaseHeal",
            "ManaRestoreFromMaxManaPerSecond",
            "TotemHWBaseCost",
            "TotemCHBaseCost",
            "TotemHWSpellpower",
            "TotemLHWSpellpower",
            "TotemThunderhead",
            "ManaRestore",
            "BonusCritHealMultiplier",
            "BonusHealingDoneMultiplier",
            "BonusManaMultiplier",
            "BonusIntellectMultiplier",
            "RestoSham2T9",
            "RestoSham4T9",
            "RestoSham2T10",
            "RestoSham4T10",
            "RestoShamRelicT9",
            "RestoShamRelicT10",
            "Healed",
            "Hp5",
            "MovementSpeed"
        };

        public static List<string> RelevantGlyphs = new List<string>(10) {
            "Glyph of Healing Wave",
            "Glyph of Water Shield",
            "Glyph of Water Mastery",
            "Glyph of Chain Heal",
            "Glyph of Earth Shield",
            "Glyph of Lesser Healing Wave",
            "Glyph of Earthliving Weapon",
            "Glyph of Mana Tide Totem",
            "Glyph of Healing Stream Totem",
            "Glyph of Riptide"
        };

        public static List<ItemType> RelevantItemTypes = new List<ItemType>(new ItemType[] {
            ItemType.None,
            ItemType.Cloth,
            ItemType.Leather,
            ItemType.Mail,
            ItemType.Totem,
            ItemType.OneHandMace,
            ItemType.OneHandAxe,
            ItemType.Shield,
            ItemType.Staff,
            ItemType.FistWeapon,
            ItemType.Dagger }
        );
    }

    /// <summary>
    /// Contains various static lists for the RestoSham module
    /// </summary>
    public static class RestoShamConfiguration
    {
        /// <summary>
        /// The Character Calculations Display configuration
        /// </summary>
        public static string[] CharacterDisplayCalculationLabels = new string[] {
            "Totals:HPS - Burst",
            "Totals:HPS - Sustained",
            "Totals:Survival",
            "Basic Stats:Health",
            "Basic Stats:Mana",
            "Basic Stats:Stamina",
            "Basic Stats:Intellect",
            "Basic Stats:Spell Power",
            "Basic Stats:MP5*Mana regeneration while casting",
            "Basic Stats:Mana Available*This is your total mana from all sources throughout the entire fight.",
            "Basic Stats:Heal Spell Crit*This includes all static talents including those that are not shown on the in-game character pane",
            "Basic Stats:Spell Haste",
            "Healing Style Breakdowns:Burst Sequence",
            "Healing Style Breakdowns:Sustained Sequence",
            "Healing Style Breakdowns:Mana Available per Second",
            "Healing Style Breakdowns:Mana Used per Second*This is the mana used per second by your chosen sustained sequence",
            "Healing Style Breakdowns:Healing Stream HPS",
            "Healing Style Breakdowns:Earth Shield HPS",
            "Healing Style Breakdowns:RT+HW HPS",
            "Healing Style Breakdowns:RT+LHW HPS",
            "Healing Style Breakdowns:RT+CH HPS",
            "Healing Style Breakdowns:HW Spam HPS",
            "Healing Style Breakdowns:LHW Spam HPS",
            "Healing Style Breakdowns:CH Spam HPS",
            "Average Cast Times:Global Cooldown",
            "Average Cast Times:Healing Wave*Normal / Tidal Waves",
            "Average Cast Times:Lesser Healing Wave",
            "Average Cast Times:Chain Heal",
            "Average Cast Times:Lightning Bolt*Aren't you busy healing people? ;)",
        };

        public static string[] OptimizableCalculationLabels = new string[] {
            "Haste %",
            "Crit %",
            "Mana Usable per Second",
        };

        public static Dictionary<string, Color> SubPointNameColors = new Dictionary<string, Color>()
        {
            {"Burst", Color.FromArgb(255, 255, 0, 0)},
            {"Sustained", Color.FromArgb(255, 0, 0, 255)},
            {"Survival", Color.FromArgb(255, 0, 128, 0)},
        };
    }
};
