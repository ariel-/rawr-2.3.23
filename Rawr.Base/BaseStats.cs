using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr
{
    public static class BaseStats
    {   // Only returns level 80 characters.
        public enum DruidForm { Caster, Bear, Cat, Moonkin }

        #region Cache Variables
        // Caching to reduce load.
        private static Stats _lastStats;
        private static int _lastLevel;
        private static CharacterClass _lastClass;
        private static CharacterRace _lastRace;
        private static DruidForm _lastForm;
        private static readonly object syncLock = new object();
        #endregion

        public static Stats GetBaseStats(Character character) { return GetBaseStats(character.Level, character.Class, character.Race, DruidForm.Caster); }
        public static Stats GetBaseStats(int level, CharacterClass characterClass, CharacterRace characterRace) { return GetBaseStats(level, characterClass, characterRace, DruidForm.Caster); }
        public static Stats GetBaseStats(int level, CharacterClass characterClass, CharacterRace characterRace, DruidForm characterForm)
        {   // Health, Mana and some other things are same for every race.
            lock (syncLock)
            {
                #region Cache
                if (level == _lastLevel
                    && characterClass == _lastClass
                    && characterRace == _lastRace
                    && characterForm == _lastForm)
                    return _lastStats.Clone();
                _lastLevel = level;
                _lastClass = characterClass;
                _lastRace = characterRace;
                _lastForm = characterForm;
                #endregion

                Stats S = new Stats();

                #region Base Stats
                #region All Classes
                S.Miss = 0.05f;
                S.Block = 0.0f;
                S.Parry = 0.0f;
                #endregion
                switch (characterClass)
                {
                    #region Death Knight
                    case CharacterClass.DeathKnight:
                        // Blood Elf, Draenei, Dwarf, Gnome, Human, Night Elf, Orc, Tauren, Troll, Undead
                        S.Mana = 0;
                        S.Health = 8121;
                        S.Armor = 0;
                        S.AttackPower = 220;
                        S.PhysicalCrit = 0.0319f;
                        S.Dodge = 0.0362500f;
                        S.Parry = 0.05f;
                        S.SpellPower = 0;
                        S.SpellCrit = 0.0f;
                        switch (characterRace)
                        {
                            case CharacterRace.BloodElf:
                                S.Strength = 172;
                                S.Agility = 114;
                                S.Stamina = 158;
                                S.Intellect = 39;
                                S.Spirit = 58;
                                break;
                            case CharacterRace.Draenei:
                                S.Strength = 176;
                                S.Agility = 109;
                                S.Stamina = 162;
                                S.Intellect = 36;
                                S.Spirit = 61;
                                break;
                            case CharacterRace.Dwarf:
                                S.Strength = 177;
                                S.Agility = 108;
                                S.Stamina = 163;
                                S.Intellect = 34;
                                S.Spirit = 58;
                                break;
                            case CharacterRace.Gnome:
                                S.Strength = 170;
                                S.Agility = 115;
                                S.Stamina = 159;
                                S.Intellect = 37;   // 39/1.05 = 37.14 ~37
                                S.Spirit = 59;
                                break;
                            case CharacterRace.Human:
                                S.Strength = 175;
                                S.Agility = 112;
                                S.Stamina = 160;
                                S.Intellect = 35;
                                S.Spirit = 58;  // 60/1.03 = 58.25 ~58.
                                break;
                            case CharacterRace.NightElf:
                                S.Strength = 172;
                                S.Agility = 117;
                                S.Stamina = 159;
                                S.Intellect = 35;
                                S.Spirit = 59;
                                break;
                            case CharacterRace.Orc:
                                S.Strength = 178;
                                S.Agility = 109;
                                S.Stamina = 162;
                                S.Intellect = 32;
                                S.Spirit = 62;
                                break;
                            case CharacterRace.Tauren:
                                S.Strength = 180;
                                S.Agility = 107;
                                S.Stamina = 162;
                                S.Intellect = 30;
                                S.Spirit = 61;
                                break;
                            case CharacterRace.Troll:
                                S.Strength = 176;
                                S.Agility = 114;
                                S.Stamina = 165;
                                S.Intellect = 31;
                                S.Spirit = 60;
                                break;
                            case CharacterRace.Undead:
                                S.Strength = 174;
                                S.Agility = 110;
                                S.Stamina = 161;
                                S.Intellect = 33;
                                S.Spirit = 64;
                                break;
                            default:
                                break;
                        }
                        break;
                    #endregion
                    #region Druid
                    case CharacterClass.Druid:
                        // NightElf, Tauren
                        S.Mana = 3496;
                        S.Health = 7417;
                        S.Armor = 0;
                        S.SpellPower = 0;
                        S.SpellCrit = 0.0185f;
                        switch (characterForm)
                        {
                            case DruidForm.Moonkin:
                            case DruidForm.Caster:
                                S.AttackPower = -10;
                                S.PhysicalCrit = 0.0743f;
                                S.Dodge = 0.0556970f; //??
                                break;
                            case DruidForm.Bear:
                                S.AttackPower = 220;
                                S.PhysicalCrit = 0.05f;
                                S.Dodge = 0.0556970f;
                                S.BonusStaminaMultiplier = 0.25f;
                                break;
                            case DruidForm.Cat:
                                S.AttackPower = 140;
                                S.PhysicalCrit = 0.07476f;
                                S.Dodge = 0.0556970f;
                                break;
                            default:
                                break;
                        }
                        if (characterRace == CharacterRace.NightElf)
                        {
                            S.Strength = 85;
                            S.Agility = 86;
                            S.Stamina = 97;
                            S.Intellect = 143;
                            S.Spirit = 159;
                        }
                        else if (characterRace == CharacterRace.Tauren)
                        {
                            S.Strength = 94;
                            S.Agility = 77;
                            S.Stamina = 100;
                            S.Intellect = 140;
                            S.Spirit = 185;
                        }
                        break;
                    #endregion
                    #region Hunter
                    case CharacterClass.Hunter:
                        // Blood Elf, Draenei, Dwarf, Night Elf, Orc, Tauren, Troll

                        S.Mana = 5046;
                        S.Armor = 0;
                        S.AttackPower = 140;
                        S.RangedAttackPower = 150;
                        S.PhysicalCrit = -0.0153f;
                        S.Dodge = -0.0412730f;
                        S.Parry = 0.05f;
                        S.SpellPower = 0;
                        S.SpellCrit = 0.0360f;
                        switch (characterRace)
                        {
                            case CharacterRace.BloodElf:
                                S.Strength = 71;
                                S.Agility = 183;
                                S.Stamina = 126;
                                S.Intellect = 94;
                                S.Spirit = 96;
                                S.Health = 7324;
                                break;
                            case CharacterRace.Draenei:
                                S.Strength = 75;
                                S.Agility = 178;
                                S.Stamina = 129;
                                S.Intellect = 91;
                                S.Spirit = 99;
                                S.Health = 7400;
                                break;
                            case CharacterRace.Dwarf:
                                S.Strength = 76;
                                S.Agility = 177;
                                S.Stamina = 131;
                                S.Intellect = 89;
                                S.Spirit = 96;
                                S.Health = 7412;
                                break;
                            case CharacterRace.NightElf:
                                S.Strength = 71;
                                S.Agility = 184;
                                S.Stamina = 127;
                                S.Intellect = 90;
                                S.Spirit = 97;
                                S.Health = 7324;
                                break;
                            case CharacterRace.Orc:
                                S.Strength = 77;
                                S.Agility = 178;
                                S.Stamina = 130;
                                S.Intellect = 87;
                                S.Spirit = 100;
                                S.Health = 7324;
                                break;
                            case CharacterRace.Tauren:
                                S.Strength = 79;
                                S.Agility = 176;
                                S.Stamina = 130;
                                S.Intellect = 85;
                                S.Spirit = 99;
                                S.Health = 7400;
                                break;
                            case CharacterRace.Troll:
                                S.Strength = 75;
                                S.Agility = 183;
                                S.Stamina = 129;
                                S.Intellect = 86;
                                S.Spirit = 98;
                                S.Health = 7324;
                                break;
                            default:
                                break;
                        }
                        break;
                    #endregion
                    #region Mage
                    case CharacterClass.Mage:
                        // Blood Elf, Draenei, Gnome, Human, Troll, Undead
                        S.Mana = 3268;
                        S.Health = 6963;
                        S.Armor = 0;
                        S.AttackPower = -10;
                        S.PhysicalCrit = 0.03f;
                        S.Dodge = 0.0361870f;
                        S.SpellPower = 0;
                        S.SpellCrit = 0.0091f;
                        switch (characterRace)
                        {
                            case CharacterRace.BloodElf:
                                S.Strength = 33;
                                S.Agility = 45;
                                S.Stamina = 57;
                                S.Intellect = 185;
                                S.Spirit = 173;
                                break;
                            case CharacterRace.Draenei:
                                S.Strength = 37;
                                S.Agility = 40;
                                S.Stamina = 58;
                                S.Intellect = 182;
                                S.Spirit = 176;
                                break;
                            case CharacterRace.Gnome:
                                S.Strength = 31;
                                S.Agility = 46;
                                S.Stamina = 58;
                                S.Intellect = 184; // 193/1.05 = 183.8 ~184.
                                S.Spirit = 174;
                                break;
                            case CharacterRace.Human:
                                S.Strength = 36;
                                S.Agility = 43;
                                S.Stamina = 59;
                                S.Intellect = 181;
                                S.Spirit = 174; // 179/1.03 = 173.8 ~174
                                break;
                            case CharacterRace.Troll:
                                S.Strength = 37;
                                S.Agility = 45;
                                S.Stamina = 60;
                                S.Intellect = 177;
                                S.Spirit = 175;
                                break;
                            case CharacterRace.Undead:
                                S.Strength = 35;
                                S.Agility = 41;
                                S.Stamina = 60;
                                S.Intellect = 179;
                                S.Spirit = 179;
                                break;
                            default:
                                break;
                        }
                        break;
                    #endregion
                    #region Paladin
                    case CharacterClass.Paladin:
                        // Blood Elf, Draenei, Dwarf, Human
                        S.Mana = 4394;
                        S.Health = 6934;
                        S.Armor = 0;
                        S.AttackPower = 240;
                        S.PhysicalCrit = 0.0327f;
                        S.SpellPower = 0;
                        S.SpellCrit = 0.03336f;
                        S.Dodge = 0.0349430f;
                        S.Parry = 0.05f;
                        S.Block = 0.05f;
                        S.Defense = 400.0f;
                        switch (characterRace)
                        {
                            case CharacterRace.BloodElf:
                                S.Strength = 148;
                                S.Agility = 92;
                                S.Stamina = 143;
                                S.Intellect = 101;
                                S.Spirit = 103;
                                break;
                            case CharacterRace.Draenei:
                                S.Strength = 152;
                                S.Agility = 87;
                                S.Stamina = 143;
                                S.Intellect = 98;
                                S.Spirit = 107;
                                break;
                            case CharacterRace.Dwarf:
                                S.Strength = 153;
                                S.Agility = 86;
                                S.Stamina = 146;
                                S.Intellect = 97;
                                S.Spirit = 104;
                                break;
                            case CharacterRace.Human:
                                S.Strength = 151;
                                S.Agility = 90;
                                S.Stamina = 143;
                                S.Intellect = 98;
                                S.Spirit = 108;
                                break;
                            default:
                                break;
                        }
                        break;
                    #endregion
                    #region Priest
                    case CharacterClass.Priest:
                        // Blood Elf, Draenei, Dwarf, Human, Night Elf, Troll, Undead
                        S.Mana = 3863;
                        S.Health = 6960;
                        S.Armor = 0;
                        S.AttackPower = -10;
                        S.PhysicalCrit = 0.027f;
                        S.Dodge = 0.0337780f;
                        S.SpellPower = 0;
                        S.SpellCrit = 0.0124f;
                        switch (characterRace)
                        {
                            case CharacterRace.BloodElf:
                                S.Strength = 40;
                                S.Agility = 53;
                                S.Stamina = 65;
                                S.Intellect = 178;
                                S.Spirit = 180;
                                break;
                            case CharacterRace.Draenei:
                                S.Strength = 44;
                                S.Agility = 48;
                                S.Stamina = 66;
                                S.Intellect = 175;
                                S.Spirit = 183;
                                break;
                            case CharacterRace.Dwarf:
                                S.Strength = 45;
                                S.Agility = 47;
                                S.Stamina = 70;
                                S.Intellect = 176;
                                S.Spirit = 180;
                                break;
                            case CharacterRace.Human:
                                S.Strength = 43;
                                S.Agility = 51;
                                S.Stamina = 67;
                                S.Intellect = 174;
                                S.Spirit = 181;     // 186/1.03 = 180.5->181
                                break;
                            case CharacterRace.NightElf:
                                S.Strength = 40;
                                S.Agility = 56;
                                S.Stamina = 66;
                                S.Intellect = 174;
                                S.Spirit = 181;
                                break;
                            case CharacterRace.Troll:
                                S.Strength = 44;
                                S.Agility = 53;
                                S.Stamina = 68;
                                S.Intellect = 170;
                                S.Spirit = 182;
                                break;
                            case CharacterRace.Undead:
                                S.Strength = 42;
                                S.Agility = 49;
                                S.Stamina = 68;
                                S.Intellect = 172;
                                S.Spirit = 186;
                                break;
                            default:
                                break;
                        }
                        break;
                    #endregion
                    #region Rogue
                    case CharacterClass.Rogue:
                        // Blood Elf, Dwarf, Gnome, Human, Night Elf, Orc, Troll, Undead
                        S.Mana = 0;
                        S.Health = 7604;
                        S.Armor = 0;
                        S.AttackPower = 140;
                        S.PhysicalCrit = -0.00295f;
                        S.Dodge = 0.0205570f;
                        S.Parry = 0.05f;
                        S.SpellPower = 0;
                        S.SpellCrit = 0f;
                        switch (characterRace)
                        {
                            case CharacterRace.BloodElf:
                                S.Strength = 110;
                                S.Agility = 191;
                                S.Stamina = 103;
                                S.Intellect = 47;
                                S.Spirit = 66;
                                break;
                            case CharacterRace.Dwarf:
                                S.Strength = 115;
                                S.Agility = 185;
                                S.Stamina = 108;
                                S.Intellect = 42;
                                S.Spirit = 66;
                                break;
                            case CharacterRace.Gnome:
                                S.Strength = 108;
                                S.Agility = 192;
                                S.Stamina = 104;
                                S.Intellect = 46;   // 48/1.05 = 45.71 ~46
                                S.Spirit = 67;
                                break;
                            case CharacterRace.Human:
                                S.Strength = 113;
                                S.Agility = 189;
                                S.Stamina = 105;
                                S.Intellect = 43;
                                S.Spirit = 67;  // 69/1.03 = 66.99 ~67
                                break;
                            case CharacterRace.NightElf:
                                S.Strength = 110;
                                S.Agility = 194;
                                S.Stamina = 104;
                                S.Intellect = 43;
                                S.Spirit = 67;
                                break;
                            case CharacterRace.Orc:
                                S.Strength = 116;
                                S.Agility = 186;
                                S.Stamina = 107;
                                S.Intellect = 40;
                                S.Spirit = 70;
                                break;
                            case CharacterRace.Troll:
                                S.Strength = 114;
                                S.Agility = 191;
                                S.Stamina = 108;
                                S.Intellect = 39;
                                S.Spirit = 68;
                                break;
                            case CharacterRace.Undead:
                                S.Strength = 112;
                                S.Agility = 187;
                                S.Stamina = 105;
                                S.Intellect = 41;
                                S.Spirit = 72;
                                break;
                            default:
                                break;
                        }
                        break;
                    #endregion
                    #region Shaman
                    case CharacterClass.Shaman:
                        // Draenei, Orc, Tauren, Troll
                        S.Mana = 4396;
                        S.Health = 6960;
                        S.Armor = 0;
                        S.AttackPower = 140;
                        S.PhysicalCrit = 0.0292f;
                        S.Dodge = 0.0206800f;
                        S.Block = 0.05f;
                        S.SpellPower = 0;
                        S.SpellCrit = 0.022f;
                        switch (characterRace)
                        {
                            case CharacterRace.Draenei:
                                S.Strength = 121;
                                S.Agility = 71;
                                S.Stamina = 136;
                                S.Intellect = 128;
                                S.Spirit = 145;
                                break;
                            case CharacterRace.Orc:
                                S.Strength = 123;
                                S.Agility = 71;
                                S.Stamina = 137;
                                S.Intellect = 125;
                                S.Spirit = 145;
                                break;
                            case CharacterRace.Tauren:
                                S.Strength = 125;
                                S.Agility = 70;
                                S.Stamina = 137;
                                S.Intellect = 124;
                                S.Spirit = 145;
                                break;
                            case CharacterRace.Troll:
                                S.Strength = 121;
                                S.Agility = 76;
                                S.Stamina = 137;
                                S.Intellect = 124;
                                S.Spirit = 144;
                                break;
                            default:
                                break;
                        }
                        break;
                    #endregion
                    #region Warlock
                    case CharacterClass.Warlock:
                        // Blood Elf, Gnome, Human, Orc, Undead
                        S.Mana = 3856;
                        S.Health = 7164;
                        S.Armor = 0;
                        S.AttackPower = -10;
                        S.PhysicalCrit = 0.028f;
                        S.Dodge = 0.0238110f;
                        S.SpellPower = 0;
                        S.SpellCrit = 0.01701f;
                        switch (characterRace)
                        {
                            case CharacterRace.BloodElf:
                                S.Strength = 56;
                                S.Agility = 69;
                                S.Stamina = 87;
                                S.Intellect = 163;
                                S.Spirit = 165;
                                break;
                            case CharacterRace.Gnome:
                                S.Strength = 54;
                                S.Agility = 70;
                                S.Stamina = 88;
                                S.Intellect = 162;  // 170/1.05 = 161.9 ~162
                                S.Spirit = 166;
                                break;
                            case CharacterRace.Human:
                                S.Strength = 59;
                                S.Agility = 67;
                                S.Stamina = 89;
                                S.Intellect = 159;
                                S.Spirit = 165; // 170/1.03 = 165.05 ~165
                                break;
                            case CharacterRace.Orc:
                                S.Strength = 62;
                                S.Agility = 64;
                                S.Stamina = 91;
                                S.Intellect = 156;
                                S.Spirit = 169;
                                break;
                            case CharacterRace.Undead:
                                S.Strength = 58;
                                S.Agility = 65;
                                S.Stamina = 90;
                                S.Intellect = 157;
                                S.Spirit = 171;
                                break;
                            default:
                                break;
                        }
                        break;
                    #endregion
                    #region Warrior
                    case CharacterClass.Warrior:
                        // Draenei, Dwarf, Gnome, Human, Night Elf, Orc, Tauren, Troll, Undead
                        S.Mana = 0f;
                        S.Health = 8121f;
                        S.Armor = 0f;
                        S.AttackPower = 220f;
                        S.PhysicalCrit = 0.03192f; // This + the base agi makes 4.88%
                        S.Dodge = 0.03664f;
                        S.Parry = 0.05f;
                        S.Block = 0.05f;
                        S.SpellPower = 0f;
                        S.SpellCrit = 0.0f;
                        S.Defense = 400.0f;
                        switch (characterRace)
                        {
                            case CharacterRace.Draenei:
                                S.Strength = 182;
                                S.Agility = 110;
                                S.Stamina = 166;
                                S.Intellect = 37;
                                S.Spirit = 61;
                                break;
                            case CharacterRace.Dwarf:
                                S.Strength = 176;
                                S.Agility = 109;
                                S.Stamina = 162;
                                S.Intellect = 35;
                                S.Spirit = 58;
                                break;
                            case CharacterRace.Gnome:
                                S.Strength = 169;
                                S.Agility = 116;
                                S.Stamina = 158;
                                S.Intellect = 38;   // 40/1.05 = 38.095 ~38.
                                S.Spirit = 59;
                                break;
                            case CharacterRace.Human:
                                S.Strength = 174f;
                                S.Agility = 113f;
                                S.Stamina = 159f;
                                S.Intellect = 36f;
                                S.Spirit = 58f;  // 60/1.03 = 58.25 ~58
                                break;
                            case CharacterRace.NightElf:
                                S.Strength = 171;
                                S.Agility = 118;
                                S.Stamina = 158;
                                S.Intellect = 36;
                                S.Spirit = 59;
                                break;
                            case CharacterRace.Orc:
                                S.Strength = 177;
                                S.Agility = 110;
                                S.Stamina = 161;
                                S.Intellect = 33;
                                S.Spirit = 62;
                                break;
                            case CharacterRace.Tauren:
                                S.Strength = 179;
                                S.Agility = 109;
                                S.Stamina = 161;
                                S.Intellect = 31;
                                S.Spirit = 61;
                                break;
                            case CharacterRace.Troll:
                                S.Strength = 185;
                                S.Agility = 115;
                                S.Stamina = 162;
                                S.Intellect = 32;
                                S.Spirit = 60;
                                break;
                            case CharacterRace.Undead:
                                S.Strength = 183;
                                S.Agility = 111;
                                S.Stamina = 169;
                                S.Intellect = 34;
                                S.Spirit = 64;
                                break;
                            default:
                                break;
                        }
                        break;
                    #endregion
                    #region No Class
                    default:
                        break;
                    #endregion
                }
                #endregion

                #region Racials
                if (characterRace == CharacterRace.Gnome)
                    S.BonusIntellectMultiplier = 0.05f;
                else if (characterRace == CharacterRace.Human)
                    S.BonusSpiritMultiplier = 0.03f;
                else if (characterRace == CharacterRace.NightElf)
                    S.Miss += 0.02f;
                else if (characterRace == CharacterRace.Tauren)
                    S.Health = S.Health * 1.05f;
                else if (characterRace == CharacterRace.Troll)
                {
                    if (characterClass == CharacterClass.DeathKnight || characterClass == CharacterClass.Warrior || characterClass == CharacterClass.Rogue)
                        S.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { PhysicalHaste = 0.2f }, 10f, 180f));
                    else
                        S.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { SpellHaste = 0.2f, PhysicalHaste = 0.2f }, 10f, 180f));
                }
                else if (characterRace == CharacterRace.Orc)
                {
                    if (characterClass == CharacterClass.Shaman)
                        S.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { AttackPower = 2 + (level * 4), SpellPower = 3 + (level * 2) }, 15f, 120f));
                    else if (characterClass == CharacterClass.Warlock)
                        S.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { SpellPower = 3 + (level * 2) }, 15f, 120f));
                    else
                        S.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { AttackPower = 2 + (level * 4) }, 15f, 120f));
                }
                #endregion

                _lastStats = S.Clone();
                return S;
            }
        }

        public static float GetRacialExpertise(Character character, ItemSlot weaponSlot)
        {
            ItemType weaponType;
            CharacterRace characterRace = character.Race;

            if (weaponSlot == ItemSlot.MainHand && character.MainHand != null)
                weaponType = character.MainHand.Item.Type;
            else if (weaponSlot == ItemSlot.OffHand && character.OffHand != null)
                weaponType = character.OffHand.Item.Type;
            else
                return 0.0f;

            switch (characterRace)
            {
                case CharacterRace.Human:
                    if (weaponType == ItemType.OneHandSword || weaponType == ItemType.TwoHandSword
                        || weaponType == ItemType.OneHandMace || weaponType == ItemType.TwoHandMace)
                    {
                        return 3.0f;
                    }
                    break;
                case CharacterRace.Dwarf:
                    if (weaponType == ItemType.OneHandMace || weaponType == ItemType.TwoHandMace)
                    {
                        return 5.0f;
                    }
                    break;
                case CharacterRace.Orc:
                    if (weaponType == ItemType.OneHandAxe || weaponType == ItemType.TwoHandAxe)
                    {
                        return 5.0f;
                    }
                    break;
            }

            return 0.0f;
        }
    }
}
