using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.TankDK
{
    enum DKCostTypes : int
    {
        Blood = 0,
        Frost,
        UnHoly,
        Death, // Have abilities that convert runes to death runes provide a negative value here just like RP.
        RunicPower,
        // TIME is in ms to keep the int structure.
        CastTime, // How long does it cost to activate the ability?
        CooldownTime, // How long until we can use the ability again.  This is ability specific CD.  Not counting Rune CD. Whatever solver we use will have to keep track of Rune CDs.
        DurationTime, // How long does the ability last?

        NumCostTypes
    }

    /// <summary>
    /// This class will be the base class for abilities.
    /// Each ability will inherit from this class and have to implement their own functions
    /// for the various spells/abilities.
    /// Then we will want to be able to aggregate all the data to have a solid picture of what's
    /// going on when the abilities are used.
    /// EG. IT * 2 should give us the values of 2 ITs in cost, damage, time, etc.
    /// </summary>
    abstract class AbilityDK_Base
    {
        // TODO: Setup a sub AbilityDK_Base object that contains any proc'd events.
        // This needs to then be calculated whenever someone calls for the value of a given ability.
        // Similar to the way special effects are handled w/ stats.

        #region Constants
        public const uint MIN_GCD_MS = 1000;
        public const uint INSTANT = 1;
        public const uint MELEE_RANGE = 5;
        public const float THREAT_FROST_PRESENCE = 2.0735f;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor for the DK's abilities.
        /// </summary>
        public AbilityDK_Base()
        {
            this.szName = "";
            this.AbilityCost[(int)DKCostTypes.Blood] = 0;
            this.AbilityCost[(int)DKCostTypes.Frost] = 0;
            this.AbilityCost[(int)DKCostTypes.UnHoly] = 0;
            this.AbilityCost[(int)DKCostTypes.RunicPower] = 0;
            this.uBaseDamage = 0;
            this.uRange = MELEE_RANGE;
            this.tDamageType = ItemDamageType.Physical;
            this.Cooldown = 1500; // GCD
            this.CastTime = INSTANT;
            this.uTickRate = INSTANT;
            this.uDuration = INSTANT;
        }
        public AbilityDK_Base(string Name)
        {
            this.szName = Name;
            this.AbilityCost[(int)DKCostTypes.Blood] = 0;
            this.AbilityCost[(int)DKCostTypes.Frost] = 0;
            this.AbilityCost[(int)DKCostTypes.UnHoly] = 0;
            this.AbilityCost[(int)DKCostTypes.RunicPower] = 0;
            this.uBaseDamage = 0;
            this.uRange = MELEE_RANGE;
            this.tDamageType = ItemDamageType.Physical;
            this.Cooldown = 1500; // GCD
            this.CastTime = INSTANT;
            this.uTickRate = INSTANT;
            this.uDuration = INSTANT;
        }
        public AbilityDK_Base(CombatState CS)
        {
            this.CState = CS;
            this.szName = "";
            this.AbilityCost[(int)DKCostTypes.Blood] = 0;
            this.AbilityCost[(int)DKCostTypes.Frost] = 0;
            this.AbilityCost[(int)DKCostTypes.UnHoly] = 0;
            this.AbilityCost[(int)DKCostTypes.RunicPower] = 0;
            this.uBaseDamage = 0;
            this.uRange = MELEE_RANGE;
            this.tDamageType = ItemDamageType.Physical;
            this.Cooldown = 1500; // GCD
            this.CastTime = INSTANT;
            this.uTickRate = INSTANT;
            this.uDuration = INSTANT;
        }
        #endregion 

        /// <summary>
        /// The state of combat at the time the ability is uesd.
        /// </summary>
        protected CombatState CState;

        /// <summary>
        /// Any DK ability triggered by this ability.  
        /// Should not be recursive.
        /// This would mean FF when using IT or Glyphed HB.
        /// </summary>
        public AbilityDK_Base m_TriggeredAbility;

        /// <summary>
        ///  Name of the ability.
        /// </summary>
        public string szName { get; set; }
        
        /// <summary>
        /// What is the cost of the ability?
        /// 6 INTs representing the 3 Rune Types & Runic Power & Time
        /// Use enum (int)DKCostTypes for placement.
        /// Negative costs mean they grant that item.
        /// </summary>
        public int[] AbilityCost = new int[(int)DKCostTypes.NumCostTypes];

        // TODO: Apply Damage types.
        // Maybe Create a Damage object since we have abilities that return 2 types of damage.
        /// <summary>
        /// What min damage does the ability cause?
        /// For DOTs damage is on a per-tick basis.
        /// </summary>
        public uint uMinDamage { get; set; }
        /// <summary>
        /// What max damage does the ability cause?
        /// For DOTs damage is on a per-tick basis.
        /// </summary>
        public uint uMaxDamage { get; set; }
        /// <summary>
        /// Get the average value between Max and Min damage
        /// For DOTs damage is on a per-tick basis.
        /// </summary>
        virtual public uint uBaseDamage { 
            get
            {
                uint AvgDam = (this.uMinDamage + this.uMaxDamage) / 2;
                uint WDam = 0;
                // Handle non-weapon based effects:
                if (null != this.wMH)
                {
                    WDam = (uint)(this.wMH.damage * this.fWeaponDamageModifier);
                }
                // Average out the min & max damage, then add in baseDamage from the weapon.
                // Factor in miss rate based on HIT
                float chanceMiss = StatConversion.WHITE_MISS_CHANCE_CAP[3];
                chanceMiss -= CState.m_Stats.PhysicalHit;
                chanceMiss = Math.Max(0f, chanceMiss);

                return (uint)((AvgDam + WDam) * (1 - chanceMiss));
            }
            set 
            {
                // Setup so that we can just set a base damage w/o having to 
                // manually set Min & Max all the time.
                uMaxDamage = uMinDamage = value;
            }
        }
        /// <summary>
        /// What's the range of a given ability?
        /// The idea is that we want to quantify the range buffing talents.
        /// </summary>
        public uint uRange { get; set; }
        /// <summary>
        /// What's the size of the area of a given ability?
        /// The idea is that we want to quantify the area buffing talents.
        /// </summary>
        public uint uArea { get; set; }

        /// <summary>
        /// Is this an AOE ability?
        /// </summary>
        public bool bAOE { get; set; }

        /// <summary>
        /// What type of damage does this ability do?
        /// </summary>
        public ItemDamageType tDamageType  { get; set; }

        #region Time Based Items
        ///////////////////////////////////////////////////////
        // Time based items.  
        // These will all be effected by haste.
        // Haste effects GCD to a max hasted of 1.5 sec to 1 sec.
        /// <summary>
        /// How long does it take to cast in ms?
        /// 1 == instant
        /// </summary>
        public uint CastTime 
        {
            get
            {
                return (uint)this.AbilityCost[(int)DKCostTypes.CastTime];
            }
            set
            {
                AbilityCost[(int)DKCostTypes.CastTime] = (int)value;
            }
        }
        /// <summary>
        /// Cooldown in seconds
        /// Default = 1500 millisecs == Global Cooldown
        /// GCD min == 1000 millisecs.
        /// </summary>
        public uint Cooldown 
        { 
            get 
            {
                if (this.bTriggersGCD)
                    return Math.Max(MIN_GCD_MS, (uint)AbilityCost[(int)DKCostTypes.CooldownTime]);
                return (uint)AbilityCost[(int)DKCostTypes.CooldownTime];
            } 
            set
            { 
                AbilityCost[(int)DKCostTypes.CooldownTime] = (int)value; 
            }
        }

        /// <summary>
        /// Does this ability trigger the GCD?
        /// </summary>
        public bool bTriggersGCD { get; set; }
        /// <summary>
        /// How long does the effect last?
        /// This is in millisecs.
        /// </summary>
        public uint uDuration
        {
            get
            {
                return Math.Max(INSTANT, (uint)AbilityCost[(int)DKCostTypes.DurationTime]);
            }
            set
            {
                AbilityCost[(int)DKCostTypes.DurationTime] = (int)Math.Max(INSTANT, value);
            }
        }

        private uint _uTickRate;
        /// <summary>
        /// How often does the effect proc for?
        /// Tick rate is millisecs.
        /// Ensure that we don't have a 0 value.  
        /// 1 ms == instant.
        /// </summary>
        public uint uTickRate 
        {
            get
            {
                return Math.Max(INSTANT, _uTickRate);
            }
            set
            {
                _uTickRate = Math.Max(INSTANT, value);
            }
        }
        #endregion 

        #region Weapon related Items
        /////////////////////////////////////////////////
        // Weapon related items.
        public bool bWeaponRequired { get; set; }
        public Weapon wMH, wOH;
        public float fWeaponDamageModifier { get; set; }
        #endregion 

        /// <summary>
        /// Get the single instance damage of this ability.
        /// </summary>
        /// <returns>Int that represents a fully buffed single instance of this ability.</returns>
        public int GetTickDamage()
        {
            // Start w/ getting the base damage values.
            int iDamage = (int)this.uBaseDamage;
            // Apply modifiers.
            iDamage += this.DamageAdditiveModifer;
            iDamage = (int)Math.Floor((float)iDamage * (1 + DamageMultiplierModifer));
            return iDamage;
        }

        /// <summary>
        /// Get the full effect over the lifetime of the ability.
        /// </summary>
        /// <returns>int that is TickDamage * duration</returns>
        virtual public int GetTotalDamage()
        {
            // Start w/ getting the base damage values.
            int iDamage = this.GetTickDamage();
            // Assuming full duration, or standard impact.
            // But I want this in whole numbers.
            // Also need to decide if I want this to be whole ticks, or if partial ticks will be allowed.
            float fDamageCount = (float)(this.uDuration / this.uTickRate);
            // To prevent divide by 0 errors.
            if (float.IsNaN(fDamageCount))
            {
#if DEBUG
//                throw new Exception("fDamageCount NaN");
#endif
                // Ensure that the Damage counts off at least once.
                fDamageCount = 1;
            }
            iDamage = (int)((float)iDamage * fDamageCount);
            if (bAOE == true)
            {
                iDamage = (int)((float)iDamage * this.CState.m_NumberOfTargets);
            }
            return iDamage;
        }

        private int _DamageAdditiveModifer;
        /// <summary>
        /// Setup the modifier formula for a given ability.
        /// </summary>
        virtual public int DamageAdditiveModifer
        {
            get 
            {
                return _DamageAdditiveModifer;
            }
            set 
            {
                _DamageAdditiveModifer = value;
            }
        }

        private float _DamageMultiplierModifer;
        /// <summary>
        /// Setup the modifier formula for a given ability.
        /// </summary>
        virtual public float DamageMultiplierModifer
        {
            get
            {
                return _DamageMultiplierModifer;
            }
            set
            {
                _DamageMultiplierModifer = value;
            }
        }

        /// <summary>
        /// How much to multiply the damage by to generate threat.
        /// </summary>
        private float _ThreatMultiplier;
        public float ThreatMultiplier
        {
            get
            {
                return _ThreatMultiplier;
            }
            set
            {
                _ThreatMultiplier = value;
            }
        }

        private float _ThreatAdditiveModifier;
        /// <summary>
        /// Get the full effect of threat over the lifetime of the ability.
        /// </summary>
        /// <returns>float that is (GetTotalDamage * ThreatModifiers) * Threat For Frost Presence</returns>
        public float GetTotalThreat() { return TotalThreat; } 
        public float TotalThreat
        {
            get
            {
                return (StatConversion.ApplyMultiplier(GetTotalDamage(), ThreatMultiplier) + _ThreatAdditiveModifier) * THREAT_FROST_PRESENCE;
            }
            set
            {
                _ThreatAdditiveModifier = value;
            }
        }

        #region Comparisons
        public static int CompareByThreatPerCost(AbilityDK_Base a, AbilityDK_Base b, DKCostTypes t)
        {
            int ic = 0;
            float aRunes = 1;
            float bRunes = 1;

            // Sum of cost:
            switch (t)
            {
                case DKCostTypes.Blood:
                    aRunes = a.AbilityCost[(int)DKCostTypes.Blood];
                    bRunes = b.AbilityCost[(int)DKCostTypes.Blood];
                    break;
                case DKCostTypes.Frost:
                    aRunes = a.AbilityCost[(int)DKCostTypes.Frost];
                    bRunes = b.AbilityCost[(int)DKCostTypes.Frost];
                    break;
                case DKCostTypes.UnHoly:
                    aRunes = a.AbilityCost[(int)DKCostTypes.UnHoly];
                    bRunes = b.AbilityCost[(int)DKCostTypes.UnHoly];
                    break;
                case DKCostTypes.Death:
                    aRunes = a.AbilityCost[(int)DKCostTypes.Blood] + a.AbilityCost[(int)DKCostTypes.Frost] + a.AbilityCost[(int)DKCostTypes.UnHoly];
                    bRunes = b.AbilityCost[(int)DKCostTypes.Blood] + b.AbilityCost[(int)DKCostTypes.Frost] + b.AbilityCost[(int)DKCostTypes.UnHoly];
                    break;
                case DKCostTypes.CastTime:
                case DKCostTypes.CooldownTime:
                case DKCostTypes.DurationTime:
                    aRunes = a.AbilityCost[(int)t] / 1000;
                    bRunes = b.AbilityCost[(int)t] / 1000;
                    break;
                case DKCostTypes.RunicPower:
                    aRunes = a.AbilityCost[(int)t];
                    bRunes = b.AbilityCost[(int)t];
                    break;
                default:
                    aRunes = 1;
                    bRunes = 1;
                    break;

            }
            if (aRunes != 0 || bRunes != 0)
            {
                if (aRunes != 0 && bRunes != 0)
                {
                    float avalue = a.GetTotalThreat() / aRunes;
                    float bvalue = b.GetTotalThreat() / bRunes;
                    if (avalue != bvalue)
                    {
                        // This is setup where we want a descending order.
                        if (avalue > bvalue)
                            ic = -1;
                        else
                            ic = 1;
                    }
                }
                else // one of them are 0
                {
                    if (aRunes > bRunes )
                        ic = -1;
                    else
                        ic = 1;
                }
            }
            return ic;
        }

        public static int CompareByTotalThreat(AbilityDK_Base a, AbilityDK_Base b)
        {
            return CompareByThreatPerCost(a, b, DKCostTypes.NumCostTypes);            
        }

        public static int CompareThreatByCooldown(AbilityDK_Base a, AbilityDK_Base b)
        {
            return CompareByThreatPerCost(a, b, DKCostTypes.CooldownTime);
        }

        public static int CompareThreatByRunes(AbilityDK_Base a, AbilityDK_Base b)
        {
            return CompareByThreatPerCost(a, b, DKCostTypes.Death);
        }

        public static int CompareByRP(AbilityDK_Base a, AbilityDK_Base b)
        {
            return CompareByThreatPerCost(a, b, DKCostTypes.RunicPower);
        }

        #endregion
    }
}
