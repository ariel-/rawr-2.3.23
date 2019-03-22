using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.TankDK
{
    /// <summary>
    /// This class is the implmentation of the Heart Strike Ability based on the AbilityDK_Base class.
    /// </summary>
    class AbilityDK_HeartStrike : AbilityDK_Base
    {
        public AbilityDK_HeartStrike(CombatState CS)
        {
            this.CState = CS;
            this.wMH = CS.MH;
            this.wOH = CS.OH;
            this.szName = "Heart Strike";
            this.AbilityCost[(int)DKCostTypes.Blood] = 1;
            this.AbilityCost[(int)DKCostTypes.RunicPower] = -10;
            this.uBaseDamage = 736; // May need to adjust this.
            this.bWeaponRequired = true;
            this.fWeaponDamageModifier = .5f;
            this.bTriggersGCD = true;
            this.bAOE = true;
            // TODO: Multi Target spell - need to add in the damage for the 2nd target.
            // Override Damage method.
        }
    }
}
