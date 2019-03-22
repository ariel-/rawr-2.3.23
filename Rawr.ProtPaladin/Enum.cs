using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.ProtPaladin
{
/*
    public enum Races
    {
        RACE_HUMAN          = 1,
        RACE_ORC            = 2,
        RACE_DWARF          = 3,
        RACE_NIGHTELF       = 4,
        RACE_UNDEAD_PLAYER  = 5,
        RACE_TAUREN         = 6,
        RACE_GNOME          = 7,
        RACE_TROLL          = 8,
        //RACE_GOBLIN         = 9,
        RACE_BLOODELF       = 10,
        RACE_DRAENEI        = 11
    };

    public enum Classes
    {
        CLASS_WARRIOR       = 1,
        CLASS_PALADIN       = 2,
        CLASS_HUNTER        = 3,
        CLASS_ROGUE         = 4,
        CLASS_PRIEST        = 5,
        CLASS_DEATH_KNIGHT  = 6,
        CLASS_SHAMAN        = 7,
        CLASS_MAGE          = 8,
        CLASS_WARLOCK       = 9,
        // CLASS_UNK2       = 10,unused
        CLASS_DRUID         = 11
    };

    public enum Stat
    {
        STAT_STRENGTH                      = 0,
        STAT_AGILITY                       = 1,
        STAT_STAMINA                       = 2,
        STAT_INTELLECT                     = 3,
        STAT_SPIRIT                        = 4
    };

    public enum Powers
    {
        POWER_MANA                          = 0,
        POWER_RAGE                          = 1,
        POWER_FOCUS                         = 2,
        POWER_ENERGY                        = 3,
        POWER_HAPPINESS                     = 4,
        POWER_RUNE                          = 5,
        POWER_RUNIC_POWER                   = 6,
        POWER_HEALTH                        = -2    // (-2 as signed value)
    };

    public enum SpellSchool
    {
        SPELL_SCHOOL_NORMAL                 = 0,
        SPELL_SCHOOL_HOLY                   = 1,
        SPELL_SCHOOL_FIRE                   = 2,
        SPELL_SCHOOL_NATURE                 = 3,
        SPELL_SCHOOL_FROST                  = 4,
        SPELL_SCHOOL_SHADOW                 = 5,
        SPELL_SCHOOL_ARCANE                 = 6
    };

    public enum SpellSchoolMask
    {
        SPELL_SCHOOL_MASK_NONE    = 0x00,     // not exist
        SPELL_SCHOOL_MASK_NORMAL  = (1 << 0), // PHYSICAL (Armor)
        SPELL_SCHOOL_MASK_HOLY    = (1 << 1),
        SPELL_SCHOOL_MASK_FIRE    = (1 << 2),
        SPELL_SCHOOL_MASK_NATURE  = (1 << 3),
        SPELL_SCHOOL_MASK_FROST   = (1 << 4),
        SPELL_SCHOOL_MASK_SHADOW  = (1 << 5),
        SPELL_SCHOOL_MASK_ARCANE  = (1 << 6),

        // unions

        // 124, not include normal and holy damage
        SPELL_SCHOOL_MASK_SPELL   = ( SPELL_SCHOOL_MASK_FIRE   |
                                     SPELL_SCHOOL_MASK_NATURE | SPELL_SCHOOL_MASK_FROST  |
                                     SPELL_SCHOOL_MASK_SHADOW | SPELL_SCHOOL_MASK_ARCANE ),
        // 126
        SPELL_SCHOOL_MASK_MAGIC   = ( SPELL_SCHOOL_MASK_HOLY | SPELL_SCHOOL_MASK_SPELL ),

        // 127
        SPELL_SCHOOL_MASK_ALL     = ( SPELL_SCHOOL_MASK_NORMAL | SPELL_SCHOOL_MASK_MAGIC )
    };

//    public static struct SpellSchools GetFirstSchoolInMask(SpellSchoolMask mask)
//    {
//        for(int i = 0; i < MAX_SPELL_SCHOOL; ++i)
//            if(mask & (1 << i))
//            return SpellSchools(i);
//
//        return SPELL_SCHOOL_NORMAL;
//    }

public enum SpellMissInfo
{
    SPELL_MISS_NONE                    = 0,
    SPELL_MISS_MISS                    = 1,
    SPELL_MISS_RESIST                  = 2,
    SPELL_MISS_DODGE                   = 3,
    SPELL_MISS_PARRY                   = 4,
    SPELL_MISS_BLOCK                   = 5,
    SPELL_MISS_EVADE                   = 6,
    SPELL_MISS_IMMUNE                  = 7,
    SPELL_MISS_IMMUNE2                 = 8,
    SPELL_MISS_DEFLECT                 = 9,
    SPELL_MISS_ABSORB                  = 10,
    SPELL_MISS_REFLECT                 = 11
};

public enum SpellHitType
{
    SPELL_HIT_TYPE_UNK1 = 0x00001,
    SPELL_HIT_TYPE_CRIT = 0x00002,
    SPELL_HIT_TYPE_UNK2 = 0x00004,
    SPELL_HIT_TYPE_UNK3 = 0x00008,
    SPELL_HIT_TYPE_UNK4 = 0x00020
};


public enum SpellDmgClass
{
    SPELL_DAMAGE_CLASS_NONE     = 0,
    SPELL_DAMAGE_CLASS_MAGIC    = 1,
    SPELL_DAMAGE_CLASS_MELEE    = 2,
    SPELL_DAMAGE_CLASS_RANGED   = 3
};

struct SpellImmune
{
    uint32 type;
    uint32 spellId;
};

typedef std::list<SpellImmune> SpellImmuneList;

enum UnitModifierType
{
    BASE_VALUE = 0,
    BASE_PCT = 1,
    TOTAL_VALUE = 2,
    TOTAL_PCT = 3,
    MODIFIER_TYPE_END = 4
};

enum WeaponDamageRange
{
    MINDAMAGE,
    MAXDAMAGE
};

enum DamageTypeToSchool
{
    RESISTANCE,
    DAMAGE_DEALT,
    DAMAGE_TAKEN
};

enum UnitMods
{
    UNIT_MOD_STAT_STRENGTH,                                 // UNIT_MOD_STAT_STRENGTH..UNIT_MOD_STAT_SPIRIT must be in existed order, it's accessed by index values of Stats enum.
    UNIT_MOD_STAT_AGILITY,
    UNIT_MOD_STAT_STAMINA,
    UNIT_MOD_STAT_INTELLECT,
    UNIT_MOD_STAT_SPIRIT,
    UNIT_MOD_HEALTH,
    UNIT_MOD_MANA,                                          // UNIT_MOD_MANA..UNIT_MOD_RUNIC_POWER must be in existed order, it's accessed by index values of Powers enum.
    UNIT_MOD_RAGE,
    UNIT_MOD_FOCUS,
    UNIT_MOD_ENERGY,
    UNIT_MOD_HAPPINESS,
    UNIT_MOD_RUNE,
    UNIT_MOD_RUNIC_POWER,
    UNIT_MOD_ARMOR,                                         // UNIT_MOD_ARMOR..UNIT_MOD_RESISTANCE_ARCANE must be in existed order, it's accessed by index values of SpellSchools enum.
    UNIT_MOD_RESISTANCE_HOLY,
    UNIT_MOD_RESISTANCE_FIRE,
    UNIT_MOD_RESISTANCE_NATURE,
    UNIT_MOD_RESISTANCE_FROST,
    UNIT_MOD_RESISTANCE_SHADOW,
    UNIT_MOD_RESISTANCE_ARCANE,
    UNIT_MOD_ATTACK_POWER,
    UNIT_MOD_ATTACK_POWER_RANGED,
    UNIT_MOD_DAMAGE_MAINHAND,
    UNIT_MOD_DAMAGE_OFFHAND,
    UNIT_MOD_DAMAGE_RANGED,
    UNIT_MOD_END,
    // synonyms
    UNIT_MOD_STAT_START = UNIT_MOD_STAT_STRENGTH,
    UNIT_MOD_STAT_END = UNIT_MOD_STAT_SPIRIT + 1,
    UNIT_MOD_RESISTANCE_START = UNIT_MOD_ARMOR,
    UNIT_MOD_RESISTANCE_END = UNIT_MOD_RESISTANCE_ARCANE + 1,
    UNIT_MOD_POWER_START = UNIT_MOD_MANA,
    UNIT_MOD_POWER_END = UNIT_MOD_RUNIC_POWER + 1
};

enum BaseModGroup
{
    CRIT_PERCENTAGE,
    RANGED_CRIT_PERCENTAGE,
    OFFHAND_CRIT_PERCENTAGE,
    SHIELD_BLOCK_VALUE,
    BASEMOD_END
};

enum WeaponAttackType
{
    BASE_ATTACK   = 0,
    OFF_ATTACK    = 1,
    RANGED_ATTACK = 2
};

#define MAX_ATTACK  3

enum CombatRating
{
    CR_WEAPON_SKILL             = 0,
    CR_DEFENSE_SKILL            = 1,
    CR_DODGE                    = 2,
    CR_PARRY                    = 3,
    CR_BLOCK                    = 4,
    CR_HIT_MELEE                = 5,
    CR_HIT_RANGED               = 6,
    CR_HIT_SPELL                = 7,
    CR_CRIT_MELEE               = 8,
    CR_CRIT_RANGED              = 9,
    CR_CRIT_SPELL               = 10,
    CR_HIT_TAKEN_MELEE          = 11,
    CR_HIT_TAKEN_RANGED         = 12,
    CR_HIT_TAKEN_SPELL          = 13,
    CR_CRIT_TAKEN_MELEE         = 14,
    CR_CRIT_TAKEN_RANGED        = 15,
    CR_CRIT_TAKEN_SPELL         = 16,
    CR_HASTE_MELEE              = 17,
    CR_HASTE_RANGED             = 18,
    CR_HASTE_SPELL              = 19,
    CR_WEAPON_SKILL_MAINHAND    = 20,
    CR_WEAPON_SKILL_OFFHAND     = 21,
    CR_WEAPON_SKILL_RANGED      = 22,
    CR_EXPERTISE                = 23,
    CR_ARMOR_PENETRATION        = 24
};

enum DamageEffectType
{
    DIRECT_DAMAGE           = 0,                            // used for normal weapon damage (not for class abilities or spells)
    SPELL_DIRECT_DAMAGE     = 1,                            // spell/class abilities damage
    DOT                     = 2,
    HEAL                    = 3,
    NODAMAGE                = 4,                            // used when damage applies to health but no channelInterruptFlags/etc
    SELF_DAMAGE             = 5
};

enum MeleeHitOutcome
{
    MELEE_HIT_EVADE, MELEE_HIT_MISS, MELEE_HIT_DODGE, MELEE_HIT_BLOCK, MELEE_HIT_PARRY,
    MELEE_HIT_GLANCING, MELEE_HIT_CRIT, MELEE_HIT_CRUSHING, MELEE_HIT_NORMAL
};

struct CleanDamage
{
    CleanDamage(uint32 _damage, WeaponAttackType _attackType, MeleeHitOutcome _hitOutCome) :
    damage(_damage), attackType(_attackType), hitOutCome(_hitOutCome) {}

    uint32 damage;
    WeaponAttackType attackType;
    MeleeHitOutcome hitOutCome;
};

// Struct for use in Unit::CalculateMeleeDamage
// Need create structure like in SMSG_ATTACKERSTATEUPDATE opcode
struct CalcDamageInfo
{
    Unit  *attacker;             // Attacker
    Unit  *target;               // Target for damage
    uint32 damageSchoolMask;
    uint32 damage;
    uint32 absorb;
    uint32 resist;
    uint32 blocked_amount;
    uint32 HitInfo;
    uint32 TargetState;
// Helper
    WeaponAttackType attackType; //
    uint32 procAttacker;
    uint32 procVictim;
    uint32 procEx;
    uint32 cleanDamage;          // Used only for rage calcultion
    MeleeHitOutcome hitOutCome;  // TODO: remove this field (use TargetState instead)
};

// Spell damage info structure based on structure sending in SMSG_SPELLNONMELEEDAMAGELOG opcode
struct SpellNonMeleeDamage{
 SpellNonMeleeDamage(Unit *_attacker, Unit *_target, uint32 _SpellID, uint32 _schoolMask) :
    attacker(_attacker), target(_target), SpellID(_SpellID), damage(0), schoolMask(_schoolMask),
    absorb(0), resist(0), phusicalLog(false), unused(false), blocked(0), HitInfo(0), cleanDamage(0) {}
 Unit   *target;
 Unit   *attacker;
 uint32 SpellID;
 uint32 damage;
 uint32 schoolMask;
 uint32 absorb;
 uint32 resist;
 bool   phusicalLog;
 bool   unused;
 uint32 blocked;
 uint32 HitInfo;
 // Used for help
 uint32 cleanDamage;
};

uint32 createProcExtendMask(SpellNonMeleeDamage *damageInfo, SpellMissInfo missCondition);

struct UnitActionBarEntry
{
    UnitActionBarEntry() : Raw(0) {}

    union
    {
        struct
        {
            uint16 SpellOrAction;
            uint16 Type;
        };
        struct
        {
            uint32 Raw;
        };
    };
};

enum CurrentSpellTypes
{
    CURRENT_MELEE_SPELL = 0,
    CURRENT_FIRST_NON_MELEE_SPELL = 1,                      // just counter
    CURRENT_GENERIC_SPELL = 1,
    CURRENT_AUTOREPEAT_SPELL = 2,
    CURRENT_CHANNELED_SPELL = 3,
    CURRENT_MAX_SPELL = 4                                   // just counter
};


public enum CreatureType
{
    CREATURE_TYPE_BEAST            = 1,
    CREATURE_TYPE_DRAGONKIN        = 2,
    CREATURE_TYPE_DEMON            = 3,
    CREATURE_TYPE_ELEMENTAL        = 4,
    CREATURE_TYPE_GIANT            = 5,
    CREATURE_TYPE_UNDEAD           = 6,
    CREATURE_TYPE_HUMANOID         = 7,
    CREATURE_TYPE_CRITTER          = 8,
    CREATURE_TYPE_MECHANICAL       = 9,
    CREATURE_TYPE_NOT_SPECIFIED    = 10,
    CREATURE_TYPE_TOTEM            = 11,
    CREATURE_TYPE_NON_COMBAT_PET   = 12,
    CREATURE_TYPE_GAS_CLOUD        = 13
};
*/
    public enum Ability
    {
        None,
        ShieldOfRighteousness,
        HammerOfTheRighteous,
        SealOfVengeance, 
        HolyVengeance,
        JudgementOfVengeance,
        SealOfRighteousness,
        JudgementOfRighteousness,
        Exorcism,
        HammerOfWrath,
        AvengersShield,
        HolyShield,
        RetributionAura,
        Consecration,
        RighteousDefense,
        HolyWrath,
        HandOfReckoning,
    }
    
    /*public enum HitResult
    {
        AnyMiss,
        AnyHit,
        Miss,
        Dodge,
        Parry,
        Block,
        Glance,
        Resist,
        Crit,
        Hit,
    }*/

    public enum AttackModelMode
    {
        BasicSoV,
        BasicSoR,
        
    }

    public enum DamageType
    {
        Physical,
        Holy,
        Fire,
        Frost,
        Arcane,
        Shadow,
        Nature,
    }

    public enum CreatureType
    {
        Unspecified,
        Humanoid,
        Undead,
        Demon,
        Giant,
        Elemental,
        Mechanical,
        Beast,
        Dragonkin,
    }

    public enum AttackType
    {
        Melee,
        Ranged,
        Spell,
        DOT,
    }
}