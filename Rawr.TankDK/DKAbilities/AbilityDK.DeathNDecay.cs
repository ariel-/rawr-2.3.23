using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.TankDK
{
    /// <summary>
    /// This class is the implmentation of the Howling Blast Ability based on the AbilityDK_Base class.
    /// </summary>
    class AbilityDK_DeathNDecay : AbilityDK_Base
    {
        public AbilityDK_DeathNDecay(CombatState CS)
        {
            this.CState = CS;
            this.szName = "Death N Decay";
            this.AbilityCost[(int)DKCostTypes.Frost] = 1;
            this.AbilityCost[(int)DKCostTypes.UnHoly] = 1;
            this.AbilityCost[(int)DKCostTypes.Blood] = 1;
            this.AbilityCost[(int)DKCostTypes.RunicPower] = -15;
            this.uMinDamage = 62;
            this.uMaxDamage = 62;
            this.tDamageType = ItemDamageType.Shadow;
            this.bWeaponRequired = false;
            this.fWeaponDamageModifier = 0;
            this.bTriggersGCD = true;
            this.uRange = 30;
            this.uArea = 10;
            this.uTickRate = 1 * 1000;
            this.uDuration = 10 * 1000;
            this.Cooldown = 30 * 1000;
            this.bAOE = true;

        }
    }
}
