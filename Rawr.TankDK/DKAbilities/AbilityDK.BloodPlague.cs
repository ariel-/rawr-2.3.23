using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.TankDK
{
    /// <summary>
    /// This class is the implmentation of the Blood Plague Ability based on the AbilityDK_Base class.
    /// </summary>
    class AbilityDK_BloodPlague : AbilityDK_Base
    {
        /// <summary>
        /// A disease dealing [0 + AP * 0.055 * 1.15] Shadow damage every 3 sec for 15 sec.  Caused by Plague Strike and other abilities.
        /// Base damage 0
        /// Bonus from attack power [AP * 0.055 * 1.15]
        /// </summary>
        public AbilityDK_BloodPlague(CombatState CS)
        {
            this.CState = CS;
            this.szName = "Blood Plague";
            this.tDamageType = ItemDamageType.Shadow;
            if (CState.m_Talents.Epidemic >= 3)
                // error
                this.uDuration = 15000;
            else
                this.uDuration = 15000 + ((uint)CState.m_Talents.Epidemic * 3000);
            this.uTickRate = 3 * 1000;
            this.uBaseDamage = 0;
            this.bTriggersGCD = false;
            this.CastTime = 0;
            this.Cooldown = 0;

        }

        private int _DamageAdditiveModifer = 0;
        /// <summary>
        /// Setup the modifier formula for a given ability.
        /// </summary>
        override public int DamageAdditiveModifer
        {
            get
            {
                //this.DamageAdditiveModifer = //[AP * 0.055 * 1.15]
                return (int)(this.CState.m_Stats.AttackPower * .055 * 1.15) + this._DamageAdditiveModifer;
            }
            set
            {
                _DamageAdditiveModifer = value;
            }
        }


    }
}
