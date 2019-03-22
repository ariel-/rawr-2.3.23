using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.TankDK
{
    /// <summary>
    /// This class is the implmentation of the Death Coil Ability based on the AbilityDK_Base class.
    /// </summary>
    class AbilityDK_DeathCoil : AbilityDK_Base
    {
        public AbilityDK_DeathCoil(CombatState CS)
        {
            this.CState = CS;
            this.szName = "Death Coil";
            this.AbilityCost[(int)DKCostTypes.RunicPower] = 40;
            this.uBaseDamage = 443;
            this.bWeaponRequired = false;
            this.bTriggersGCD = true;
            this.uRange = 30;
        }
    }
}
