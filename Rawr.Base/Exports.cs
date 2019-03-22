using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Rawr
{
    public static class Exports
    {
        public static string GetLootRankURL(Character character)
        {
            StringBuilder lootrank = new System.Text.StringBuilder();
            lootrank.Append("http://www.lootrank.com/wow/wr.asp?"); // add URL header
            lootrank.Append(getLootRankClassAndArmour(character.Class));
            lootrank.Append("Max=10&n1=1&i4=0&Gem=3&"); // Gem=3 rare Gem=4 epic change when new epic gems available
            lootrank.Append("maxlv=" + character.Level + "&");
            lootrank.Append(getLootRankWeightFilter(character));
            return lootrank.ToString();
        }

        public static void CopyPawnString(Character character)
        {
            StringBuilder pawn = new System.Text.StringBuilder();
            pawn.Append("( Pawn: v1: \"Rawr\": "); // adds pawn header
            pawn.Append(getPawnWeightFilter(character));
            pawn.AppendLine(" )"); // adds pawn footer
			try
			{
				Clipboard.SetText(pawn.ToString());
			}
			catch { }
        }

        private static string getLootRankClassAndArmour(CharacterClass className)
        {
            switch (className)
            {
                case CharacterClass.DeathKnight :
                    return "Cla=2048&Art=1&";
                case CharacterClass.Druid :
                    return "Cla=1024&Art=4&";
                case CharacterClass.Hunter :
                    return "Cla=4&Art=2&";
                case CharacterClass.Mage :
                    return "Cla=128&Art=8&";
                case CharacterClass.Paladin :
                    return "Cla=2&Art=1&";
                case CharacterClass.Priest :
                    return "Cla=16&Art=8&";
                case CharacterClass.Rogue :
                    return "Cla=8&Art=4&";
                case CharacterClass.Shaman :
                    return "Cla=64&Art=2&";
                case CharacterClass.Warlock :
                    return "Cla=256&Art=8&";
                case CharacterClass.Warrior :
                    return "Cla=1&Art=1&";
            }
            return string.Empty;
        }

        private static string getLootRankWeightFilter(Character character)
        {
            StringBuilder wtf = new StringBuilder();
            ComparisonCalculationBase[] statValues = CalculationsBase.GetRelativeStatValues(character);
            foreach (ComparisonCalculationBase ccb in statValues)
            {
                string stat = getLootrankStatID(ccb.Name);
                if (!stat.Equals(string.Empty))
                    wtf.Append(stat + "=" + ccb.OverallPoints.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + "&");
            }
            if (character.CurrentModel == "Enhance")
                wtf.Append("s1=2.4&s3=2.4&dps=6&odps=3&");
            if (wtf.Length == 0)
                return string.Empty;
            else
                return wtf.ToString();
        }

        private static string getLootrankStatID(string Name)
        {
            switch (Name)
            {
                case " Strength": return "Str";
                case " Agility": return "Agi";
                case " Stamina": return "Sta";
                case " Intellect": return "Int";
                case " Spirit": return "Spi";

                case " Health": return string.Empty;
                case " Mana": return string.Empty;
                case " Health per 5 sec": return string.Empty;
                case " Mana per 5 sec": return "mp5";

                case " Armor": return "Arm";
                case " Defense Rating": return "Def";
                case " Block Value": return "blv";
                case " Block Rating": return "blr";
                case " Dodge Rating": return "Dod";
                case " Parry Rating": return "par";
                case " Bonus Armor": return "bar";
                case " Resilience": return "res";

                case " Attack Power": return "map";
                case " Spell Power": return "spd";
                case " Expertise Rating": return "Exp";
                case " Hit Rating": return "mhit";
                case " Crit Rating": return "mcr";
                case " Haste Rating": return "mh";
                case " Melee Crit": return string.Empty;

                case " Feral Attack Power": return "fap";
                case " Spell Crit Rating": return string.Empty;
                case " Spell Arcane Damage": return string.Empty;
                case " Spell Fire Damage": return string.Empty;
                case " Spell Nature Damage": return string.Empty;
                case " Spell Shadow Damage": return string.Empty;
                case " Armor Penetration Rating": return "arp";
            }
            return string.Empty;
        }

        private static string getPawnWeightFilter(Character character)
        {
            StringBuilder wtf = new StringBuilder();
            ComparisonCalculationBase[] statValues = CalculationsBase.GetRelativeStatValues(character);
            foreach (ComparisonCalculationBase ccb in statValues)
            {
                string stat = getPawnStatID(ccb.Name);
                if (!stat.Equals(string.Empty))
                    wtf.Append(stat + "=" + ccb.OverallPoints.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + ", ");
            }
            if (wtf.Length == 0)
                return string.Empty;
            else
                return wtf.ToString().Substring(0,wtf.Length-2); // remove trailing comma
        }

        private static string getPawnStatID(string Name)
        {
            switch (Name)
            {
                case " Strength": return "Strength";
                case " Agility": return "Agility";
                case " Stamina": return "Stamina";
                case " Intellect": return "Intellect";
                case " Spirit": return "Spirit";

                case " Health": return "Health";
                case " Mana": return "Mana";
                case " Health per 5 sec": return "Hp5";
                case " Mana per 5 sec": return "Mp5";

                case " Armor": return "Armor";
                case " Defense Rating": return "DefenseRating";
                case " Block Value": return "BlockValue";
                case " Block Rating": return "BlockRating";
                case " Dodge Rating": return "DodgeRating";
                case " Parry Rating": return "ParryRating";
                case " Bonus Armor": return string.Empty;
                case " Resilience": return "ResilienceRating";

                case " Attack Power": return "Ap";
                case " Spell Power": return "SpellPower";
                case " Expertise Rating": return "ExpertiseRating";
                case " Hit Rating": return "HitRating";
                case " Crit Rating": return "CritRating";
                case " Haste Rating": return "HasteRating";
                case " Melee Crit": return string.Empty;

                case " Feral Attack Power": return string.Empty;
                case " Spell Crit Rating": return string.Empty;
                case " Spell Arcane Damage": return "ArcaneSpellDamage";
                case " Spell Fire Damage": return "FireSpellDamage";
                case " Spell Nature Damage": return "NatureSpellDamage";
                case " Spell Shadow Damage": return "ShadowSpellDamage";
                case " Armor Penetration Rating": return "ArmorPenetration";
            }
            return string.Empty;
        }
    }
}
