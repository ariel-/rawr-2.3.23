using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.TankDK
{
    /// <summary>
    /// This class is the implmentation of the Blood Strike Ability based on the AbilityDK_Base class.
    /// </summary>
    class AbilityDK_BloodStrike : AbilityDK_Base
    {
        public AbilityDK_BloodStrike(CombatState CS)
        {
            this.CState = CS;
            this.wMH = CS.MH;
            this.wOH = CS.OH;
            this.szName = "Blood Strike";
            this.AbilityCost[(int)DKCostTypes.Blood] = 1;
            this.AbilityCost[(int)DKCostTypes.RunicPower] = -10;
            this.uBaseDamage = 306;
            this.fWeaponDamageModifier = 0.4f;
            this.bWeaponRequired = true;
            this.bTriggersGCD = true;
            m_iToT = CState.m_Talents.ThreatOfThassarian;

        }

        // TODO: Damage increased by 12.5% per disease on target.
        private float _DamageMultiplierModifer;
        /// <summary>
        /// Setup the modifier formula for a given ability.
        /// </summary>
        override public float DamageMultiplierModifer
        {
            get
            {
//                return .125 * uNumDiseases;
                return _DamageMultiplierModifer;
            }
            set
            {
                _DamageMultiplierModifer = value;
            }
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
