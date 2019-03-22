using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.TankDK
{
    /// <summary>
    /// This class is the implmentation of the BloodBoil Ability based on the AbilityDK_Base class.
    /// </summary>
    class AbilityDK_BloodBoil : AbilityDK_Base
    {
        public AbilityDK_BloodBoil(CombatState CS)
        {
            this.CState = CS;
            this.szName = "Blood Boil";
            this.AbilityCost[(int)DKCostTypes.Blood] = 1;
            this.AbilityCost[(int)DKCostTypes.RunicPower] = -10;
            this.uMinDamage = 180;
            this.uMaxDamage = 220;
            this.tDamageType = ItemDamageType.Shadow;
            this.bWeaponRequired = false;
            this.fWeaponDamageModifier = 0;
            this.bTriggersGCD = true;
            this.uRange = 0;
            this.uArea = 10;
            this.bAOE = true;
        }

        private int _DamageAdditiveModifer = 95;
        /// <summary>
        /// Setup the modifier formula for a given ability.
        /// </summary>
        override public int DamageAdditiveModifer
        {
            get
            {
                // AP Scaling
                int AdditionalDamage = (int)(this.CState.m_Stats.AttackPower * 0.04);
                // TODO: Additional Disease Damage:
                // if (uNumDiseases > 0): 95 + (0.035 × Attack Power).
                return (int)(this.CState.m_Stats.AttackPower * 0.035) + this._DamageAdditiveModifer + AdditionalDamage;
            }
            set
            {
                _DamageAdditiveModifer = value;
            }
        }
        
    }
}
