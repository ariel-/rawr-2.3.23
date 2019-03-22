using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.TankDK
{
    /// <summary>
    /// This class is the implmentation of the IcyTouch Ability based on the AbilityDK_Base class.
    /// </summary>
    class AbilityDK_IcyTouch : AbilityDK_Base
    {
        /// <summary>
        /// Chills the target for 227 to 245 Frost damage and  infects 
        /// them with Frost Fever, a disease that deals periodic damage 
        /// and reduces melee and ranged attack speed by 14% for 15 sec.
        /// </summary>
        public AbilityDK_IcyTouch(CombatState CS)
        {
            this.CState = CS;
            this.szName = "Icy Touch";
            this.AbilityCost[(int)DKCostTypes.Frost] = 1;
            this.AbilityCost[(int)DKCostTypes.RunicPower] = -10;
            this.uMaxDamage = 245;
            this.uMinDamage = 227;
            this.uRange = 20;
            this.tDamageType = ItemDamageType.Frost;
            this.bTriggersGCD = true;
            // 3.3.3 IT now hits alot harder for threat.
            this.ThreatMultiplier = 7;
            this.m_TriggeredAbility = new AbilityDK_FrostFever(CS);
        }
    }
}
