/**********
 * Owner: Jothay
 **********/
using System;

namespace Rawr.DPSWarr.Skills
{
    public class ShieldSlam : Ability
    {
        /// <summary>
        /// Instant, 6 sec cd, 20 Rage, Melee Range, Shields (Any)
        /// Slam the target with your shield, causing 990 to 1040 damage, modified by you shield block
        /// value, and dispels 1 magic effect on the target. Also causes a high amount of threat.
        /// </summary>
        /// <TalentsAffecting>
        /// Focused Rage [-(Talents.FocusedRage * 1f) RageCost],
        /// Gag Order [+(5*Pts)% Damage],
        /// OneHandedWeaponSpecialization [+(2*Pts)% Damage]
        /// </TalentsAffecting>
        /// <GlyphsAffecting></GlyphsAffecting>
    }
    public class Revenge : Ability
    {
        /// <summary>
        /// Instant, 1 sec cd, 5 Rage, Melee Range, Melee Weapon (Def)
        /// Instantly counterattack the enemy for 2399 to 2787 damage. Revenge is only usable after the
        /// warrior blocks, dodges or parries an attack.
        /// </summary>
        /// <TalentsAffecting></TalentsAffecting>
        /// <GlyphsAffecting></GlyphsAffecting>
        ///  -(Talents.FocusedRage * 1f) RageCost
        ///  +(10*Pts)% Damage
    }
    public class ConcussionBlow : Ability
    {
        /// <summary>
        /// Instant, 30 sec cd, 12 Rage, Melee Range, Melee Weapon (Any)
        /// Stuns the opponent for 5 sec and deals 2419 damage (based upon attack power).
        /// </summary>
        /// <TalentsAffecting>Concussion Blow [Requires Talent], Focused Rage [-(Talents.FocusedRage * 1f ) Ragecost]</TalentsAffecting>
        /// <GlyphsAffecting></GlyphsAffecting>
    }
    public class Devastate : Ability
    {
        /// <summary>
        /// Instant, No Cd, 12 Rage, Melee Range, 1h Melee Weapon (Any)
        /// Sunder the target's armor causing the Sunder Armor effect. In addition, causes 50% of weapon
        /// damage plus 101 for each application of Sunder Armor on the target. The Sunder Armor effect
        /// can stack up to 5 times.
        /// </summary>
        /// <TalentsAffecting>
        /// Devastate [Requires Talent]
        /// Focused Rage [-(Talents.FocusedRage * 1f) RageCost]
        /// Puncture [-(Talents.Puncture * 1f) RageCost]
        /// Sword and Board [+(5*Pts)% Crit Chance]
        /// </TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Devastate [+1 stack of Sunder Armor]</GlyphsAffecting>
    }
    public class Shockwave : Ability
    {
        /// <summary>
        /// Instant, 20 sec Cd, 12 Rage, (Any)
        /// Sends a wave of force in front of the warrior, causing 2419 damage (based upon attack power)
        /// and stunning all enemy targets within 10 yards in a frontal cone for 4 sec.
        /// </summary>
        /// <TalentsAffecting>Shockwave [Requires Talent], Focused Rage [-(Talents.FocusedRage*1f) RageCost]</TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Shockwave [-3 sec Cd]</GlyphsAffecting>
    }
    public class MockingBlow : Ability
    {
        /// <summary>
        /// Instant, 1 min Cooldown, 10 Rage, Melee Range, Melee Weapon, (Battle/Zerker)
        /// A mocking attack that causes weapon damage, a moderate amount of threat and forces the
        /// target to focus attacks on you for 6 sec.
        /// </summary>
        /// <TalentsAffecting>
        /// Focused Rage [-(Talents.FocusedRage*1f) RageCost]
        /// </TalentsAffecting>
        /// <GlyphsAffecting>
        /// Glyph of Barbaric Insults [+100% Threat]
        /// Glyph of Mocking Blow [+25% Damage]
        /// </GlyphsAffecting>
    }
}