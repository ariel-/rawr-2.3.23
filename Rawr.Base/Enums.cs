using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr
{
    public enum CharacterClass
    {
        Warrior = 1,
        Paladin = 2,
        Hunter = 3,
        Rogue = 4,
        Priest = 5,
        DeathKnight = 6,
        Shaman = 7,
        Mage = 8,
        Warlock = 9,
        Druid = 11,
    }

    public enum CharacterFaction
    {
        Alliance = 1,
        Horde = 2,
    }

    public enum CharacterRegion
    {
        US, EU, KR, TW, CN
    }

    public enum CharacterRace
    {
        Human = 1,
        Orc = 2,
        Dwarf = 3,
        NightElf = 4,
        Undead = 5,
        Tauren = 6,
        Gnome = 7,
        Troll = 8,
        BloodElf = 10,
        Draenei = 11
    }

    public enum CharacterSlot
    {
        None = -1,
        Projectile = 0,
        Head = 1,
        Neck = 2,
        Shoulders = 3,
        Chest = 4,
        Waist = 5,
        Legs = 6,
        Feet = 7,
        Wrist = 8,
        Hands = 9,
        Finger1 = 10,
        Finger2 = 11,
        Trinket1 = 12,
        Trinket2 = 13,
        Back = 14,
        MainHand = 15,
        OffHand = 16,
        Ranged = 17,
        ProjectileBag = 18,
        Tabard = 19,
        Shirt = 20,

        Gems = 100,
        Metas = 101,
        AutoSelect = 1000,
    }

    public enum ItemAvailability
    {
        NotAvailable,
        Available,
        AvailableWithEnchantRestrictions,
        RegemmingAllowed,
        RegemmingAllowedWithEnchantRestrictions
    }

    public enum ItemDamageType
    {
        Physical = 0,
        Holy,
        Fire,
        Nature,
        Frost,
        Shadow,
        Arcane
    }

    public enum ItemQuality
    {
        Temp = -1,
        Poor = 0,
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Artifact,
        Heirloom
    }

    public enum ItemFaction
    {
        Neutral = 0,
        Alliance = 1,
        Horde = 2,
    }

    public enum ItemType
    {
        None,

        Cloth,
        Leather,
        Mail,
        Plate,

        Dagger,
        FistWeapon,
        OneHandAxe,
        TwoHandAxe,
        OneHandMace,
        TwoHandMace,
        OneHandSword,
        TwoHandSword,
        Polearm,
        Staff,
        Shield,

        Bow,
        Crossbow,
        Gun,
        Wand,
        Thrown,

        Idol,
        Libram,
        Totem,
        Sigil,

        Arrow,
        Bullet,
        Quiver,
        AmmoPouch
    }

    public enum ItemSlot
    {
        None,

        Head,
        Neck,
        Shoulders,
        Back,
        Chest,
        Shirt,
        Tabard,
        Wrist,
        Hands,
        Waist,
        Legs,
        Feet,
        Finger,
        Trinket,
        OneHand,
        TwoHand,
        MainHand,
        OffHand,
        Ranged,
        Projectile,
        ProjectileBag,

        Meta,
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
        Purple,
        Prismatic

        //None = 0,
        //Head = 1,
        //Neck = 2,
        //Shoulders = 3,
        //Shirt = 4,
        //Chest = 5,
        //Waist = 6,
        //Legs = 7,
        //Feet = 8,
        //Wrist = 9,
        //Hands = 10,
        //Finger = 11,
        //Trinket = 12,
        //OneHand = 13,
        //OffHand = 14,
        //Ranged = 15,
        //Back = 16,
        //TwoHand = 17,

        //Tabard = 19,
        //Robe = 20,
        //MainHand = 21,
        //OffHandB = 22,


        //Thrown = 25,
        //Wand = 26,
        //Relic = 28,

        //Weapon = 97,

        //Meta = 101,
        //Red = 102,
        //Orange = 103,
        //Yellow = 104,
        //Green = 105,
        //Prismatic = 106,
        //Purple = 107,
        //Blue = 108
    }

    public enum BuffGroup
    {
        All = 0,
        Current,
        Food,
        ElixirsAndFlasks,
        Scrolls,
        Potions,
        RaidBuffs,
        RaidDeBuffs
    }

    public enum Profession
    { // values are based upon what the armory returns for those profession id's
        None = 0,
        Alchemy = 171,
        Enchanting = 333,
        Engineering = 202,
        Herbalism = 182,
        Inscription = 773,
        Jewelcrafting = 755,
        Leatherworking = 165,
        Skinning = 393,
        Tailoring = 197,
        Blacksmithing = 164,
        Mining = 186,
    }
}
