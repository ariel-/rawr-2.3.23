using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Retribution
{

    public enum SealOf
    {
        Blood = 0,
        Command,
        Righteousness,
        Vengeance,
        None
    }

    public enum Ability
    {
        Judgement = 0,
        CrusaderStrike,
        DivineStorm,
        Consecration,
        HammerOfWrath,
        Exorcism,

        // Abilities are mapped to integer numbers from 0 to (int)Ability.Last inclusive
        Last = Exorcism
    }

    public enum AbilityType
    {
        Melee = 0,
        Range,
        Spell
    }

    public enum DamageType
    {
        Physical = 0,
        Holy,
        Magic
    }

    public enum MobType
    {
        Undead = 0,
        Demon,
        Humanoid,
        Elemental,
        Other
    }

}
