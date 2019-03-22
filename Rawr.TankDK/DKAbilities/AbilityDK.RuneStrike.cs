using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.TankDK
{
    /// <summary>
    /// This class is the implmentation of the Rune Strike Ability based on the AbilityDK_Base class.
    /// Strike the target for 150% weapon damage plus [150 * AP * 10 / 10000].  Only usable after the Death Knight dodges or parries.  Can't be dodged, blocked, or parried.  This attack causes a high amount of threat.
    /// </summary>
    class AbilityDK_RuneStrike : AbilityDK_Base
    {
        public AbilityDK_RuneStrike(CombatState CS)
        {
            this.CState = CS;
            this.wMH = CS.MH;
            this.wOH = CS.OH;
            this.szName = "Rune Strike";
            this.AbilityCost[(int)DKCostTypes.RunicPower] = 20;
            this.uBaseDamage = 736; // May need to adjust this.
            this.bWeaponRequired = true;
            this.fWeaponDamageModifier = 1.5f;
            this.DamageAdditiveModifer = 150 * (int)CState.m_Stats.AttackPower * 10 / 10000;
            this.bTriggersGCD = false;
            this.Cooldown = 0;
            m_iToT = CState.m_Talents.ThreatOfThassarian;
        }

        private int m_iToT = 0;

        /// <summary>
        /// Get the average value between Max and Min damage
        /// For DOTs damage is on a per-tick basis.
        /// </summary>
        override public uint uBaseDamage
        {
            get
            {
                uint WDam = base.uBaseDamage;
                // Off-hand damage is only effective if we have Threat of Thassaurian
                // And only for specific strikes as defined by the talent.
                if (m_iToT > 0 && null != this.wOH) // DW
                {
                    float iToTMultiplier = 0;
                    if (m_iToT == 1)
                        iToTMultiplier = .30f;
                    if (m_iToT == 2)
                        iToTMultiplier = .60f;
                    if (m_iToT == 3)
                        iToTMultiplier = 1f;
                    WDam += (uint)(this.wOH.damage * iToTMultiplier * this.fWeaponDamageModifier);
                }
                return WDam;
            }
            set
            {
                // Setup so that we can just set a base damage w/o having to 
                // manually set Min & Max all the time.
                uMaxDamage = uMinDamage = value;
            }
        }
    }
}
