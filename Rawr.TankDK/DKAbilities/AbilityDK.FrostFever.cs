using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.TankDK
{
    /// <summary>
    /// This class is the implmentation of the Frost Fever Ability based on the AbilityDK_Base class.
    /// </summary>
    class AbilityDK_FrostFever : AbilityDK_Base
    {
        /// <summary>
        /// A disease dealing [0 + AP * 0.055 * 1.15] Frost damage every 3 sec and reducing the target's melee and ranged attack speed by 14% for 15 sec.  Caused by Icy Touch and other spells.
        /// Base damage 0
        /// Bonus from attack power [AP * 0.055 * 1.15]
        /// </summary>
        /// <param name="Epidemic">How many points into Epidemic?</param>
        public AbilityDK_FrostFever(CombatState CS)
        {
            this.CState = CS;
            this.szName = "Frost Fever";
            this.tDamageType = ItemDamageType.Frost;
            if (CS.m_Talents.Epidemic >= 3)
                // error
                this.uDuration = 15000;
            else
                this.uDuration = 15000 + ((uint)CS.m_Talents.Epidemic * 3000);
            this.uTickRate = 3 * 1000;
            this.uBaseDamage = 0;
            this.bTriggersGCD = false;
            this.Cooldown = 0;
            this.CastTime = 0;
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
