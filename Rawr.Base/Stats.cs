using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Reflection.Emit;
#if RAWR3
using System.Linq;
#endif
using System.Xml.Serialization;

namespace Rawr {
    public enum AdditiveStat : int {
        Agility,
        ArcaneResistance,
        Armor,
        BonusArmor,
        ArmorPenetrationRating,
        AttackPower,
        BaseAgility,    // Improper use: Not a stat, used as a helper variable in StatConversion.GetDRAvoidanceChance(), needs fixing.

        Block,
        BlockRating,
        BlockValue,

        BossAttackPower,
        PhysicalCrit,
        CritRating,
        CritBonusDamage,
        DamageAbsorbed,
        Defense,
        DefenseRating,
        Dodge,
        DodgeRating,
        Expertise,     // Improper use: Not a stat, used as a helper variable in most Melee-based models, needs fixing. There is no Expertise on a naked character nor is there on any item/enchant/buff.
        ExpertiseRating,
        FireResistance,
        FrostResistance,
        HasteRating,
        Health,
        PhysicalHit,
        HitRating,
        Hp5,
        HealthRestore,
        HealthRestoreFromMaxHealth,
        InnervateCooldownReduction,
        InsectSwarmDmg,
        Intellect,
        InterruptProtection,
        Mana,
        ManaRestore,
        ManaRestoreOnCast_5_15,
        ManaRestoreFromBaseManaPPM,
        ManaRestoreFromMaxManaPerSecond,
        ManaorEquivRestore,
        MangleCostReduction,
        MasteryRating,
        Paragon,
        RipCostReduction,
        Miss,
        MoonfireDmg,
        MoonfireExtension,
        BladeWardProc,
        Mp5,
        NatureResistance,
        Parry,
        ParryRating,
        ExtractOfNecromanticPowerProc,
        DarkmoonCardDeathProc,
        PVPTrinket,
        RangedHitRating,
        RangedCritRating,
        RangedHasteRating,
        Resilience,
        ScopeDamage,
        ShadowResistance,
        SpellArcaneDamageRating,
        SpellCombatManaRegeneration,
        SpellCrit,
        SpellCritOnTarget,
        SpellDamageFromIntellectPercentage,
        SpellDamageFromSpiritPercentage,
        SpellDamageRating,
        SpellFireDamageRating,
        SpellFrostDamageRating,
        SpellHit,
        SpellNatureDamageRating,
        SpellPenetration,
        SpellShadowDamageRating,
        Spirit,
        Stamina,
        StarfireBonusWithDot,
        StarfireCritChance,
        StarfireDmg,
        Strength,
        TerrorProc,
        BonusHealingReceived,
        UnseenMoonDamageBonus,
        WeaponDamage,
        WindfuryAPBonus,
        WrathDmg,
        ManacostReduceWithin15OnHealingCast,
        SpellPower,
        BonusRageOnCrit,
		BonusFerociousBiteCrit,
		BonusRipCrit,
        BonusRakeCrit,
        BonusInsectSwarmDamage,
        BonusNukeCritChance,
        TigersFuryCooldownReduction,
        SpellReflectChance,
        MoteOfAnger,
        #region Added by Rawr.Feral
        BonusRakeDuration,
        BonusRipDamagePerCPPerTick,
        BonusRipDuration,
        BonusSavageRoarDuration,
        BonusShredDamage,
        CatFormStrength,
        ClearcastOnBleedChance,
        CritChanceReduction,    // Improper use: used as a talent intermediate, please cleanup
        #endregion
        #region Added by Rawr.HealPriest
        PriestInnerFire,
        RenewDurationIncrease,
        ManaGainOnGreaterHealOverheal,
        PrayerOfMendingExtraJumps,
        GreaterHealCostReduction,
        WeakenedSoulDurationDecrease,
        PrayerOfHealingExtraCrit,
        PWSBonusSpellPowerProc,
        PriestHeal_T9_2pc,
        PriestHeal_T9_4pc,
        PriestHeal_T10_2pc,
        PriestHeal_T10_4pc,
        #endregion
        #region Added by Rawr.ShadowPriest
        SWPDurationIncrease,
        MindBlastCostReduction,
        ShadowWordDeathCritIncrease,
        DevouringPlagueBonusDamage,
        MindBlastHasteProc,
        PriestDPS_T9_2pc,
        PriestDPS_T9_4pc,
        PriestDPS_T10_2pc,
        PriestDPS_T10_4pc,
        #endregion
        #region Added by Rawr.Mage
        BonusManaGem,
        MageIceArmor,
        MageMageArmor,
        MageMoltenArmor,
        Mage4T8,
        Mage2T9,
        Mage4T9,
        Mage2T10,
        Mage4T10,
        #endregion
        #region Added by Rawr.Tree
        LifebloomCostReduction, // T7 (2)
        NourishBonusPerHoT, // T7 (4)
        SwiftmendBonus, // T8 (2)
        RejuvenationInstantTick, // T8 (4)
        NourishCritBonus, // T9 (2)
        RejuvenationCrit, // T9 (4)
        WildGrowthLessReduction, // T10 (2)
        RejuvJumpChance, // T10 (4)
        NourishSpellpower, // Idol of the Flourishing Life
        RejuvenationHealBonus, // Idol of Pure Thoughts (lvl74)
        ReduceRejuvenationCost, // Idol of Awakening (lvl80) 
        LifebloomTickHealBonus, // Idol of Lush Mosh
        HealingTouchFinalHealBonus, // Idol of Health 
        SwiftmendCdReduc, // S7 PvP 4 Pc
        RevitalizeChance, // From talent
        #endregion
        #region Added by Rawr.Enhance
        BonusSSDamage,
        BonusWFAttackPower,
        Enhance2T7,
        Enhance4T7,
        Enhance2T8,
        Enhance4T8,
        Enhance2T9,
        Enhance4T9,
        Enhance2T10,
        Enhance4T10,
        #endregion
        #region Added by Rawr.Elemental
        BonusCritChance,
        BonusLavaBurstCritDamage,
        BonusFlameShockDoTDamage,
        BonusFlameShockDuration,
        LightningBoltDamageModifier,
        LightningBoltCostReduction,
        LightningSpellPower,
        LavaBurstBonus,
        LightningBoltCritDamageModifier,
        Elemental2T10,
        Elemental4T10,
        #endregion
        #region Added by Rawr.Restosham
        ManaSpringMp5Increase,
        WaterShieldIncrease,
        CHHWHealIncrease,
        CHManaReduction,
        CHHealIncrease,
        LHWManaReduction,
        RTCDDecrease,
        CHCTDecrease,
        Earthliving,
        TotemCHBaseHeal,
        TotemHWBaseCost,
        TotemCHBaseCost,
        TotemHWSpellpower,
        TotemLHWSpellpower,
        TotemThunderhead,
        RestoSham2T9,
        RestoSham4T9,
        RestoSham2T10,
        RestoSham4T10,
        RestoShamRelicT9,
        RestoShamRelicT10,
        #endregion
        #region Added by Rawr.Healadin
        FlashOfLightSpellPower,
        FlashOfLightMultiplier,
        FlashOfLightCrit,
        HolyLightManaReduction,
        HolyLightSpellPower,
        HolyLightPercentManaReduction,
        HolyLightCrit,
        SacredShieldICDReduction,
        HolyShockHoTOnCrit,
        HolyShockCrit,
        Heal1Min,
        Healed,
        SpellsManaReduction,        // Seems this applies before talents, so different from ManaRestore with 100% proc on SpellCast, initially used by Spark of Hope
        HealingOmenProc,            // Omen like proc from Soul Preserver and the like
        BonusJudgementDuration,
        FlashOfLightHoTMultiplier,
        DivineIlluminationHealingMultiplier,
        HolyLightCastTimeReductionFromHolyShock,
        #endregion
        #region Added by Rawr.Retribution
        DivineStormMultiplier,
        CrusaderStrikeMultiplier,
        ExorcismMultiplier,
        HammerOfWrathMultiplier,
        DivineStormCrit,
        CrusaderStrikeCrit,
        DivineStormDamage,
        CrusaderStrikeDamage,
        ConsecrationSpellPower,
        JudgementCDReduction,
        JudgementCrit,
        JudgementMultiplier,
        SealMultiplier,
        DivineStormRefresh,
        #endregion
        #region Added by Rawr.ProtPaladin
        BonusShieldOfRighteousnessDamage,
        DivineProtectionDurationBonus,
        HolyShieldBlockValue,
        JudgementBlockValue,
        ShieldOfRighteousnessBlockValue,        
        #endregion
        #region Added by Rawr.Moonkin
        StarfireProc,
        EclipseBonus,
        MoonfireDotCrit,
        BonusMoonkinNukeDamage,
        MoonkinT10CritDot,
        #endregion
        #region Added by Rawr.DPSWarr
        BonusTargets,
        BonusRageGen,
        #endregion
        #region Added by Rawr.Hunter
        RangedAttackPower,
        BonusHunter_T9_2P_SerpCanCrit,
        BonusHunter_T9_4P_SteadyShotPetAPProc,
        BonusHunter_PvP_4pc,
        BonusPetCritChance,
        PetStamina,
        PetStrength,
        PetSpirit,
        PetAttackPower,
        #endregion
        #region Added by Rawr.Warlock
        AttackPowerDemons,
        #endregion
        #region Added by DK*
        BonusObliterateDamage,
        BonusScourgeStrikeDamage,
        RPp5,
        #endregion
        #region Set Bonuses: Warlock
        Warlock4T7,
        #endregion
        #region Set Bonuses: Warrior
		DevastateCritIncrease,
        BonusWarrior_T7_4P_RageProc,
        BonusWarrior_T8_2P_HasteProc,
        BonusWarrior_T8_4P_MSBTCritIncrease,
        BonusWarrior_T9_2P_Crit,
        BonusWarrior_T9_2P_ArP,
        BonusWarrior_T9_4P_SLHSCritIncrease,
        BonusWarrior_T10_2P_DWAPProc,
        BonusWarrior_T10_4P_BSSDProcChange,
        BonusWarrior_PvP_4P_InterceptCDReduc,
        #endregion
        #region Set Bonuses: Rogue
        RuptureDamageBonus,
        ComboMoveEnergyReduction,
        BonusEnergyFromDP,
        RuptureCrit,
        ReduceEnergyCostFromRupture,
        BonusCPGCritChance,
        BonusToTTEnergy,
        ChanceOn3CPOnFinisher,
        #endregion
        #region DK Sigil Bonuses
        BonusBloodStrikeDamage,
        BonusDeathCoilDamage,
        BonusDeathStrikeDamage,
        BonusFrostStrikeDamage,
        BonusHeartStrikeDamage,
        BonusIcyTouchDamage,
        BonusHowlingBlastDamage,
        #endregion
        #region DK set bonuses
        DiseasesCanCrit,
        BonusAntiMagicShellDamageReduction,
        BonusIceboundFortitudeDuration,

        BonusRPFromDeathStrike,
        BonusRPFromObliterate,
        BonusRPFromScourgeStrike,
        TankDK_T10_2pc,
        TankDK_T10_4pc,
        #endregion
        #region Runeforges
        CinderglacierProc,
        #endregion
        #region Damage Procs
        PhysicalDamage,
        ArcaneDamage,
        HolyDamage,
        NatureDamage,
        ShadowDamage,
        FireDamage,
        FrostDamage,
        ValkyrDamage,
        #endregion
    }

    public enum MultiplicativeStat : int {
		BonusMangleBearThreat,
        BonusAgilityMultiplier,
        BonusArcaneDamageMultiplier,
        BaseArmorMultiplier,
        BonusArmorMultiplier,
        BonusBleedDamageMultiplier,

        BonusBlockValueMultiplier,

        BonusAttackPowerMultiplier,
        BonusCritMultiplier,
        BonusFireDamageMultiplier,
        BonusFrostDamageMultiplier,
        BonusIntellectMultiplier,
        BonusNatureDamageMultiplier,
        BonusPhysicalDamageMultiplier,
        BonusDamageMultiplier,
        BonusSpellCritMultiplier,
        BonusSpellPowerMultiplier,
        BonusSpellPowerDemonicPactMultiplier,
        BonusSpiritMultiplier,
        BonusHealthMultiplier,
        BonusManaMultiplier,
        BonusCritHealMultiplier,
        BonusStaminaMultiplier,
        BonusStrengthMultiplier,
		BonusBearSwipeDamageMultiplier,
        BonusShadowDamageMultiplier,
        BonusHolyDamageMultiplier,
        BonusDiseaseDamageMultiplier,
        ThreatIncreaseMultiplier,
        BonusManaregenWhileCastingMultiplier,
        PhysicalHaste,
        RangedHaste,
        SpellHaste,
        HealingReceivedMultiplier,
        BonusHealingDoneMultiplier,
        DamageTakenMultiplier,
        SpellDamageTakenMultiplier,
        PhysicalDamageTakenMultiplier,
        StunDurReduc,
        SnareRootDurReduc,
        FearDurReduc,
        #region Added by Rawr.TankDK
        BonusFrostWeaponDamage,

        BonusRuneStrikeMultiplier,
        BonusBloodStrikeDamageMultiplier, // T9_2p
        BonusHeartStrikeDamageMultiplier, // T9_2p
        BonusHeartStrikeMultiplier,
        BonusScourgeStrikeMultiplier,
        BonusObliterateMultiplier,

        BonusDeathCoilCrit,
        BonusDeathStrikeCrit,
        BonusFrostStrikeCrit,
        BonusObliterateCrit,
        BonusPlagueStrikeCrit,
        BonusScourgeStrikeCrit,

        BonusPerDiseaseBloodStrikeDamage,
        BonusPerDiseaseHeartStrikeDamage,
        BonusPerDiseaseObliterateDamage,
        BonusPerDiseaseScourgeStrikeDamage,

        #endregion
        #region Added by Rawr.Feral
        BonusLacerateDamageMultiplier,
        BonusRipDamageMultiplier,
        #endregion
        #region Added by Rawr.HealPriest
        BonusPoHManaCostReductionMultiplier,
        BonusGHHealingMultiplier,
        #endregion
        #region Added by Rawr.ShadowPriest
        BonusMindBlastMultiplier,
        #endregion
        #region Added by Rawr.Elemental
        BonusLavaBurstDamageMultiplier,
        #endregion      
        #region Added by Rawr.ProtPaladin
        BonusSealOfCorruptionDamageMultiplier,
        BonusSealOfRighteousnessDamageMultiplier,
        BonusSealOfVengeanceDamageMultiplier,
        HammerOfTheRighteousMultiplier,
        #endregion
        #region Added by Rawr.Hunter
        BonusRangedAttackPowerMultiplier,
        BonusPetAttackPowerMultiplier,
        BonusPetDamageMultiplier,
        BonusHunter_T7_4P_ViperSpeed,
        BonusHunter_T8_2P_SerpDmg,
        ManaCostPerc,
        #endregion
        #region Added by Rawr.Warlock
        Warlock2T7,
        Warlock2T8,
        Warlock4T8,
        Warlock2T9,
        Warlock4T9,
        Warlock2T10,
        Warlock4T10,
        #endregion
        #region Added by Rawr.Warrior
        BonusShieldSlamDamage,
        BonusDevastateDamage,
        BonusShockwaveDamage,
        BonusWarrior_T7_2P_SlamDamage,
        #endregion
        #region Boss Stats
        BossAttackSpeedMultiplier,
        #endregion
    }

    public enum InverseMultiplicativeStat : int {
		ArmorPenetration,
		ThreatReductionMultiplier,
    }

    public enum NonStackingStat : int {
        BonusManaPotion,
        ArcaneResistanceBuff,
        FireResistanceBuff,
        FrostResistanceBuff,
        NatureResistanceBuff,
        ShadowResistanceBuff,
        MovementSpeed,
        GreatnessProc,
        HighestStat,
        ShieldFromHealed,
        RighteousVengeanceCanCrit,
        DeathbringerProc,
        BattlemasterHealth,
        ZodProc,
    }

    // Pulling this from ProtWar/ProtPally so that it can be used in other common areas.
    public enum HitResult {
        AnyMiss,
        AnyHit,
        Miss,
        Dodge,
        Parry,
        Block,
        Glance,
        Resist,
        Crit,
        Hit,
    }

    public enum MagicSchool {
        Fire = 2,
        Nature,
        Frost,
        Shadow,
        Arcane,
    }

#if SILVERLIGHT
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DisplayNameAttribute : Attribute
    {
        public DisplayNameAttribute(string name)
        {
            _name = name;
        }

        private readonly string _name;

        public string DisplayName { get { return _name; } }
    }
#endif

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class GenerateSerializerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class GenerateArraySerializerAttribute : Attribute
    {
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public sealed class CommonStat : System.Attribute {
        public static float GetCommonStatMinimumRange(PropertyInfo property) {
            foreach (System.Attribute attribute in property.GetCustomAttributes(false))
            {
                CommonStat commonStat = attribute as CommonStat;
                if (commonStat != null)
                {
                    return commonStat.MinRange;
                }
            }
            return -1f;
        }

        public float MinRange = 0f;
    }

    /// <summary>
    /// A Stats object represents a collection of stats on an object, such as an Item, Enchant, or Buff.
    /// </summary>
#if SILVERLIGHT
    public class Stats
#else    
    public unsafe class Stats
#endif
    {
        internal float[] _rawAdditiveData = new float[AdditiveStatCount];
        internal float[] _rawMultiplicativeData = new float[MultiplicativeStatCount];
        internal float[] _rawInverseMultiplicativeData = new float[InverseMultiplicativeStatCount];
        internal float[] _rawNoStackData = new float[NonStackingStatCount];
        [System.ComponentModel.DefaultValueAttribute(null)]
        [XmlArray("SpecialEffects")]
        [XmlArrayItem(IsNullable = false)]
        public SpecialEffect[] _rawSpecialEffectData = null;

        //private static SpecialEffect[] _emptySpecialEffectData = new SpecialEffect[0];
        private const int _defaultSpecialEffectDataCapacity = 4;
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [XmlElement("SpecialEffectCount")]
        public int _rawSpecialEffectDataSize;

        //internal float[] _sparseData;
        internal int[] _sparseIndices;
        internal int _sparseAdditiveCount;
        internal int _sparseMultiplicativeCount;
        internal int _sparseInverseMultiplicativeCount;
        internal int _sparseNoStackCount;

        public void Clear()
        {
            Array.Clear(_rawAdditiveData, 0, _rawAdditiveData.Length);
            Array.Clear(_rawMultiplicativeData, 0, _rawMultiplicativeData.Length);
            Array.Clear(_rawInverseMultiplicativeData, 0, _rawInverseMultiplicativeData.Length);
            Array.Clear(_rawNoStackData, 0, _rawNoStackData.Length);
            _rawSpecialEffectDataSize = 0;
        }

        public void InvalidateSparseData() {
            //_sparseData = null;
            _sparseIndices = null;
        }

        public void GenerateSparseData() {
            //List<float> data = new List<float>();
            if (_sparseIndices != null) return;
            lock (_rawAdditiveData)
            {
                if (_sparseIndices == null)
                {
                    List<int> indices = new List<int>();
                    _sparseAdditiveCount = 0;
                    for (int i = 0; i < _rawAdditiveData.Length; i++)
                    {
                        if (_rawAdditiveData[i] != 0.0f)
                        {
                            _sparseAdditiveCount++;
                            //data.Add(_rawAdditiveData[i]);
                            indices.Add(i);
                        }
                    }
                    _sparseMultiplicativeCount = 0;
                    for (int i = 0; i < _rawMultiplicativeData.Length; i++)
                    {
                        if (_rawMultiplicativeData[i] != 0.0f)
                        {
                            _sparseMultiplicativeCount++;
                            //data.Add(_rawMultiplicativeData[i]);
                            indices.Add(i);
                        }
                    }
                    _sparseInverseMultiplicativeCount = 0;
                    for (int i = 0; i < _rawInverseMultiplicativeData.Length; i++)
                    {
                        if (_rawInverseMultiplicativeData[i] != 0.0f)
                        {
                            _sparseInverseMultiplicativeCount++;
                            //data.Add(_rawInverseMultiplicativeData[i]);
                            indices.Add(i);
                        }
                    }
                    _sparseNoStackCount = 0;
                    for (int i = 0; i < _rawNoStackData.Length; i++)
                    {
                        if (_rawNoStackData[i] != 0.0f)
                        {
                            _sparseNoStackCount++;
                            //data.Add(_rawNoStackData[i]);
                            indices.Add(i);
                        }
                    }
                    //_sparseData = data.ToArray();
                    _sparseIndices = indices.ToArray();
                }
            }
        }

        /// <summary>
        /// The properties for each stat. In order to add additional stats for Rawr to track,
        /// first add properties here, for each stat. Apply a Category attribute to assign it to
        /// a category in the item editor. Optionally, apply a DisplayName attribute. If no
        /// DisplayName attribute is applied, the property name will be used, with spaces between
        /// each word, detected by capitalization (AttackPower becomes "Attack Power"). If the
        /// stat is a multiplier, add the Multiplicative attribute.
        /// </summary>
        #region Stat Properties

        #region Base Stats
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [CommonStat(MinRange = 80f)]
        public float Armor
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Armor]; }
            set { _rawAdditiveData[(int)AdditiveStat.Armor] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [CommonStat(MinRange = 80f)]
        public float BonusArmor
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusArmor]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusArmor] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [CommonStat]
        public float Health
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Health]; }
            set { _rawAdditiveData[(int)AdditiveStat.Health] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        public float Mana
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Mana]; }
            set { _rawAdditiveData[(int)AdditiveStat.Mana] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [CommonStat(MinRange = 10f)]
        public float Agility
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Agility]; }
            set { _rawAdditiveData[(int)AdditiveStat.Agility] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Misc")]
        public float BaseAgility
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BaseAgility]; }
            set { _rawAdditiveData[(int)AdditiveStat.BaseAgility] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [CommonStat]
        public float Stamina
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Stamina]; }
            set { _rawAdditiveData[(int)AdditiveStat.Stamina] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [CommonStat(MinRange = 20f)]
        public float AttackPower
        {
            get { return _rawAdditiveData[(int)AdditiveStat.AttackPower]; }
            set { _rawAdditiveData[(int)AdditiveStat.AttackPower] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Misc")]
        public float AttackPowerDemons
        {
            get { return _rawAdditiveData[(int)AdditiveStat.AttackPowerDemons]; }
            set { _rawAdditiveData[(int)AdditiveStat.AttackPowerDemons] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [CommonStat(MinRange = 20f)]
        public float Strength
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Strength]; }
            set { _rawAdditiveData[(int)AdditiveStat.Strength] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Buffs / Debuffs")]
        public float BossAttackPower
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BossAttackPower]; }
            set { _rawAdditiveData[(int)AdditiveStat.BossAttackPower] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Misc")]
        public float WeaponDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.WeaponDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.WeaponDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Misc")]
        [CommonStat]
        public float ScopeDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ScopeDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.ScopeDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [DisplayName("Armor Penetration Rating")]
        [CommonStat]
        public float ArmorPenetrationRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ArmorPenetrationRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.ArmorPenetrationRating] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [CommonStat(MinRange = 10f)]
        public float Intellect
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Intellect]; }
            set { _rawAdditiveData[(int)AdditiveStat.Intellect] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [CommonStat(MinRange = 10f)]
        public float Spirit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Spirit]; }
            set { _rawAdditiveData[(int)AdditiveStat.Spirit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [DisplayName("Spell Power")]
        [CommonStat]
        public float SpellPower
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SpellPower]; }
            set { _rawAdditiveData[(int)AdditiveStat.SpellPower] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [DisplayName("Spell Penetration")]
        public float SpellPenetration
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SpellPenetration]; }
            set { _rawAdditiveData[(int)AdditiveStat.SpellPenetration] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [DisplayName("Crit Rating")]
        [CommonStat]
        public float CritRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.CritRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.CritRating] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [DisplayName("Hit Rating")]
        [CommonStat]
        public float HitRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.HitRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.HitRating] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [DisplayName("Mastery Rating")]
        [CommonStat]
        public float MasteryRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.MasteryRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.MasteryRating] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [DisplayName("Dodge Rating")]
        [CommonStat]
        public float DodgeRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.DodgeRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.DodgeRating] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [DisplayName("Parry Rating")]
        [CommonStat]
        public float ParryRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ParryRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.ParryRating] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [DisplayName("Block Rating")]
        [CommonStat]
        public float BlockRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BlockRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.BlockRating] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [DisplayName("Block Value")]
        [CommonStat]
        public float BlockValue
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BlockValue]; }
            set { _rawAdditiveData[(int)AdditiveStat.BlockValue] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [DisplayName("Defense Rating")]
        [CommonStat]
        public float DefenseRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.DefenseRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.DefenseRating] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [DisplayName("Resilience")]
        [CommonStat]
        public float Resilience
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Resilience]; }
            set { _rawAdditiveData[(int)AdditiveStat.Resilience] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [DisplayName("Expertise Rating")]
        [CommonStat]
        public float ExpertiseRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ExpertiseRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.ExpertiseRating] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [DisplayName("Haste Rating")]
        [CommonStat(MinRange = 10f)]
        public float HasteRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.HasteRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.HasteRating] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [DisplayName("Mana per 5 sec")]
        [CommonStat]
        public float Mp5
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Mp5]; }
            set { _rawAdditiveData[(int)AdditiveStat.Mp5] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Base Stats")]
        [DisplayName("Health per 5 sec")]
        [CommonStat]
        public float Hp5
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Hp5]; }
            set { _rawAdditiveData[(int)AdditiveStat.Hp5] = value; }
        }
        #endregion

        #region Resistances
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Resistances")]
        [DisplayName("Frost Res")]
        public float FrostResistance
        {
            get { return _rawAdditiveData[(int)AdditiveStat.FrostResistance]; }
            set { _rawAdditiveData[(int)AdditiveStat.FrostResistance] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Resistances")]
        [DisplayName("Nature Res")]
        public float NatureResistance
        {
            get { return _rawAdditiveData[(int)AdditiveStat.NatureResistance]; }
            set { _rawAdditiveData[(int)AdditiveStat.NatureResistance] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Resistances")]
        [DisplayName("Fire Res")]
        public float FireResistance
        {
            get { return _rawAdditiveData[(int)AdditiveStat.FireResistance]; }
            set { _rawAdditiveData[(int)AdditiveStat.FireResistance] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Resistances")]
        [DisplayName("Shadow Res")]
        public float ShadowResistance
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ShadowResistance]; }
            set { _rawAdditiveData[(int)AdditiveStat.ShadowResistance] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Resistances")]
        [DisplayName("Arcane Res")]
        public float ArcaneResistance
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ArcaneResistance]; }
            set { _rawAdditiveData[(int)AdditiveStat.ArcaneResistance] = value; }
        }
        #endregion

        #region Buffs / Debuffs

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Healing Received")]
        public float HealingReceivedMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.HealingReceivedMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.HealingReceivedMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Healing Done")]
        public float BonusHealingDoneMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusHealingDoneMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusHealingDoneMultiplier] = value; }
        }

        /// <summary>From Judgement of Wisdom</summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Base Mana / Minute (PPM)")]
        [Category("Buffs / Debuffs")]
        public float ManaRestoreFromBaseManaPPM
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ManaRestoreFromBaseManaPPM]; }
            set { _rawAdditiveData[(int)AdditiveStat.ManaRestoreFromBaseManaPPM] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Max Mana / Sec")]
        [Category("Buffs / Debuffs")]
        public float ManaRestoreFromMaxManaPerSecond
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ManaRestoreFromMaxManaPerSecond]; }
            set { _rawAdditiveData[(int)AdditiveStat.ManaRestoreFromMaxManaPerSecond] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Fire Damage")]
        public float BonusFireDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusFireDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusFireDamageMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Shadow Damage")]
        public float BonusShadowDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusShadowDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusShadowDamageMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Holy Damage")]
        public float BonusHolyDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusHolyDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusHolyDamageMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Arcane Damage")]
        public float BonusArcaneDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusArcaneDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusArcaneDamageMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Nature Damage")]
        public float BonusNatureDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusNatureDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusNatureDamageMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Frost Damage")]
        public float BonusFrostDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusFrostDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusFrostDamageMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Disease Damage")]
        public float BonusDiseaseDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusDiseaseDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusDiseaseDamageMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Spirit")]
        public float BonusSpiritMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusSpiritMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusSpiritMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Crit Dmg")]
        public float BonusCritMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusCritMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusCritMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Spell Crit Dmg")]
        public float BonusSpellCritMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusSpellCritMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusSpellCritMultiplier] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Physical Dmg")]
        public float BonusPhysicalDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusPhysicalDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusPhysicalDamageMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Dmg")]
        public float BonusDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusDamageMultiplier] = value; }
        }
        #endregion

        /// <summary>
        /// -50% = Abilities cost 50% less than they normally would. An Ability with a Mana Cost of "10% of base mana" would become 5%.
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% of Base Ability Mana Cost")]
        public float ManaCostPerc
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.ManaCostPerc]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.ManaCostPerc] = value; }
        }

        #region Combat Values
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Combat Values")]
        [DisplayName("% Armor Penetration")]
        public float ArmorPenetration
        {
            get { return _rawInverseMultiplicativeData[(int)InverseMultiplicativeStat.ArmorPenetration]; }
            set { _rawInverseMultiplicativeData[(int)InverseMultiplicativeStat.ArmorPenetration] = value; }
        }

        [Percentage]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% Spell Crit")]
        [Category("Combat Values")]
        public float SpellCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SpellCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.SpellCrit] = value; }
        }
        [Percentage]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% Spell Crit on target")]
        [Category("Combat Values")]
        public float SpellCritOnTarget
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SpellCritOnTarget]; }
            set { _rawAdditiveData[(int)AdditiveStat.SpellCritOnTarget] = value; }
        }
        [Percentage]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Combat Values")]
        [DisplayName("% Spell Hit")]
        public float SpellHit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SpellHit]; }
            set { _rawAdditiveData[(int)AdditiveStat.SpellHit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Combat Values")]
        [DisplayName("% Spell Haste")]
        public float SpellHaste
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.SpellHaste]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.SpellHaste] = value; }
        }

        // percentage mana generation while casting
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Combat Values")]
        [DisplayName("Combat Mana Regeneration")]
        public float SpellCombatManaRegeneration
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SpellCombatManaRegeneration]; }
            set { _rawAdditiveData[(int)AdditiveStat.SpellCombatManaRegeneration] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Combat Values")]
        [DisplayName("% Physical Crit")]
        public float PhysicalCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PhysicalCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.PhysicalCrit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Combat Values")]
        [DisplayName("% Parry")]
        public float Parry
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Parry]; }
            set { _rawAdditiveData[(int)AdditiveStat.Parry] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Combat Values")]
        [Percentage]
        [DisplayName("% Block")]
        public float Block
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Block]; }
            set { _rawAdditiveData[(int)AdditiveStat.Block] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Hit")]
        [Category("Combat Values")]
        public float PhysicalHit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PhysicalHit]; }
            set { _rawAdditiveData[(int)AdditiveStat.PhysicalHit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Dodge")]
        [Category("Combat Values")]
        public float Dodge
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Dodge]; }
            set { _rawAdditiveData[(int)AdditiveStat.Dodge] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Combat Values")]
        public float Defense
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Defense]; }
            set { _rawAdditiveData[(int)AdditiveStat.Defense] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Combat Values")]
        public float Expertise
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Expertise]; }
            set { _rawAdditiveData[(int)AdditiveStat.Expertise] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Combat Values")]
        [DisplayName("% Physical Haste")]
        public float PhysicalHaste
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.PhysicalHaste]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.PhysicalHaste] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Combat Values")]
        [DisplayName("% Ranged Haste")]
        public float RangedHaste
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.RangedHaste]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.RangedHaste] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Combat Values")]
        [DisplayName("% Miss")]
        public float Miss
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Miss]; }
            set { _rawAdditiveData[(int)AdditiveStat.Miss] = value; }
        }
        #endregion

        #region Spell Combat Ratings
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Spell Combat Ratings")]
        [DisplayName("Spell Shadow Damage")]
        [CommonStat]
        public float SpellShadowDamageRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SpellShadowDamageRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.SpellShadowDamageRating] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Spell Combat Ratings")]
        [DisplayName("Spell Fire Damage")]
        [CommonStat]
        public float SpellFireDamageRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SpellFireDamageRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.SpellFireDamageRating] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Spell Combat Ratings")]
        [DisplayName("Spell Frost Damage")]
        [CommonStat]
        public float SpellFrostDamageRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SpellFrostDamageRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.SpellFrostDamageRating] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Spell Combat Ratings")]
        [DisplayName("Spell Arcane Damage")]
        [CommonStat]
        public float SpellArcaneDamageRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SpellArcaneDamageRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.SpellArcaneDamageRating] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Spell Combat Ratings")]
        [DisplayName("Spell Nature Damage")]
        [CommonStat]
        public float SpellNatureDamageRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SpellNatureDamageRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.SpellNatureDamageRating] = value; }
        }
        #endregion

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Equipment Effects")]
        [DisplayName("% Crit Bonus Damage")]
        public float CritBonusDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.CritBonusDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.CritBonusDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Misc")]
        public float InterruptProtection
        {
            get { return _rawAdditiveData[(int)AdditiveStat.InterruptProtection]; }
            set { _rawAdditiveData[(int)AdditiveStat.InterruptProtection] = value; }
        }

        #region Rogue Set Bonuses
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Rogue")]
        public float RuptureDamageBonus
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RuptureDamageBonus]; }
            set { _rawAdditiveData[(int)AdditiveStat.RuptureDamageBonus] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Rogue")]
        public float ComboMoveEnergyReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ComboMoveEnergyReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.ComboMoveEnergyReduction] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Rogue")]
        public float BonusEnergyFromDP
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusEnergyFromDP]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusEnergyFromDP] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Rogue")]
        public float RuptureCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RuptureCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.RuptureCrit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Rogue")]
        public float ReduceEnergyCostFromRupture
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ReduceEnergyCostFromRupture]; }
            set { _rawAdditiveData[(int)AdditiveStat.ReduceEnergyCostFromRupture] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Rogue")]
        public float BonusCPGCritChance
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusCPGCritChance]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusCPGCritChance] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Rogue")]
        public float BonusToTTEnergy
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusToTTEnergy]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusToTTEnergy] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Rogue")]
        public float ChanceOn3CPOnFinisher
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ChanceOn3CPOnFinisher]; }
            set { _rawAdditiveData[(int)AdditiveStat.ChanceOn3CPOnFinisher] = value; }
        }
        #endregion

        #region Death Knight Bonuses
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Runic Power per 5 Seconds")]
        public float RPp5
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RPp5]; }
            set { _rawAdditiveData[(int)AdditiveStat.RPp5] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Bonus Obliterate Damage")]
        public float BonusObliterateDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusObliterateDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusObliterateDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Diseases Can Crit")]
        public float DiseasesCanCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.DiseasesCanCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.DiseasesCanCrit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Bonus Heart Strike Damage Multiplier")]
        public float BonusHeartStrikeMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusHeartStrikeMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusHeartStrikeMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Bonus Scourge Strike Damage Multiplier")]
        public float BonusScourgeStrikeMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusScourgeStrikeMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusScourgeStrikeMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Bonus Obliterate Damage Multiplier")]
        public float BonusObliterateMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusObliterateMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusObliterateMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Bonus Scourge Strike Damage")]
        public float BonusScourgeStrikeDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusScourgeStrikeDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusScourgeStrikeDamage] = value; }
        }
        
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Bonus Blood Strike Damage")]
        /// This is NOT a multiplier, this is additive raw damage bonus.
        public float BonusBloodStrikeDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusBloodStrikeDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusBloodStrikeDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Bonus Heart Strike Damage")]
        /// This is NOT a multiplier, this is additive raw damage bonus.
        public float BonusHeartStrikeDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusHeartStrikeDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusHeartStrikeDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Bonus Death Coil Damage")]
        public float BonusDeathCoilDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusDeathCoilDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusDeathCoilDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Bonus Frost Strike Damage")]
        public float BonusFrostStrikeDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusFrostStrikeDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusFrostStrikeDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Bonus Howling Blast Damage")]
        public float BonusHowlingBlastDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusHowlingBlastDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusHowlingBlastDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% White Damage as Frost Damage")]
        public float BonusFrostWeaponDamage
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusFrostWeaponDamage]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusFrostWeaponDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Cinderglacier Proc")]
        public float CinderglacierProc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.CinderglacierProc]; }
            set { _rawAdditiveData[(int)AdditiveStat.CinderglacierProc] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Bonus Death Strike Damage")]
        public float BonusDeathStrikeDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusDeathStrikeDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusDeathStrikeDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Bonus Icy Touch Damage")]
        public float BonusIcyTouchDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusIcyTouchDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusIcyTouchDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Bonus AMS Damage Reduction")]
        public float BonusAntiMagicShellDamageReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusAntiMagicShellDamageReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusAntiMagicShellDamageReduction] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Bonus Death Coil Crit")]
        public float BonusDeathCoilCrit
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusDeathCoilCrit]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusDeathCoilCrit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Bonus Death Strike Crit")]
        public float BonusDeathStrikeCrit
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusDeathStrikeCrit]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusDeathStrikeCrit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Bonus Frost Strike Crit")]
        public float BonusFrostStrikeCrit
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusFrostStrikeCrit]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusFrostStrikeCrit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Bonus IBF Duration")]
        public float BonusIceboundFortitudeDuration
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusIceboundFortitudeDuration]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusIceboundFortitudeDuration] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Obliterate Crit")]
        public float BonusObliterateCrit
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusObliterateCrit]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusObliterateCrit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Bonus Damage Per Disease For Blood Strike")]
        public float BonusPerDiseaseBloodStrikeDamage
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusPerDiseaseBloodStrikeDamage]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusPerDiseaseBloodStrikeDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Bonus Damage Per Disease For Heart Strike")]
        public float BonusPerDiseaseHeartStrikeDamage
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusPerDiseaseHeartStrikeDamage]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusPerDiseaseHeartStrikeDamage] = value; }
        }

        
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Bonus Damage Per Disease For Obliterate")]
        public float BonusPerDiseaseObliterateDamage
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusPerDiseaseObliterateDamage]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusPerDiseaseObliterateDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Bonus Damage Per Disease For Scourge Strike")]
        public float BonusPerDiseaseScourgeStrikeDamage
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusPerDiseaseScourgeStrikeDamage]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusPerDiseaseScourgeStrikeDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Bonus Plague Strike Crit")]
        public float BonusPlagueStrikeCrit
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusPlagueStrikeCrit]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusPlagueStrikeCrit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Bonus RP From Death Strike")]
        public float BonusRPFromDeathStrike
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusRPFromDeathStrike]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusRPFromDeathStrike] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Bonus RP From Obliterate")]
        public float BonusRPFromObliterate
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusRPFromObliterate]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusRPFromObliterate] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Death Knight")]
        [DisplayName("Bonus RP From Scourge Strike")]
        public float BonusRPFromScourgeStrike
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusRPFromScourgeStrike]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusRPFromScourgeStrike] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Rune Strike Damage Multiplier")]
        public float BonusRuneStrikeMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusRuneStrikeMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusRuneStrikeMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Blood Strike Damage Multiplier")]
        public float BonusBloodStrikeDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusBloodStrikeDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusBloodStrikeDamageMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Heart Strike Damage Multiplier")]
        public float BonusHeartStrikeDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusHeartStrikeDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusHeartStrikeDamageMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Bonus Scourge Strike Crit")]
        public float BonusScourgeStrikeCrit
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusScourgeStrikeCrit]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusScourgeStrikeCrit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Increase damage by DnD")]
        public float TankDK_T10_2pc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.TankDK_T10_2pc]; }
            set { _rawAdditiveData[(int)AdditiveStat.TankDK_T10_2pc] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Death Knight")]
        [DisplayName("% Damage Reduction by Blood Tap")]
        public float TankDK_T10_4pc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.TankDK_T10_4pc]; }
            set { _rawAdditiveData[(int)AdditiveStat.TankDK_T10_4pc] = value; }
        }
        #endregion

        #region Feral Bonuses
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Feral")]
        public float TerrorProc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.TerrorProc]; }
            set { _rawAdditiveData[(int)AdditiveStat.TerrorProc] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Feral")]
        public float MangleCatCostReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.MangleCostReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.MangleCostReduction] = value; }
        }

		//[System.ComponentModel.DefaultValueAttribute(0f)]
		//[Category("Feral")]
		//public float RakeCostReduction
		//{
		//    get { return _rawAdditiveData[(int)AdditiveStat.RakeCostReduction]; }
		//    set { _rawAdditiveData[(int)AdditiveStat.RakeCostReduction] = value; }
		//}

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Feral")]
        public float RipCostReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RipCostReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.RipCostReduction] = value; }
        }
        
		//[System.ComponentModel.DefaultValueAttribute(0f)]
		//[Category("Feral")]
		//public float ShredCostReduction
		//{
		//    get { return _rawAdditiveData[(int)AdditiveStat.ShredCostReduction]; }
		//    set { _rawAdditiveData[(int)AdditiveStat.ShredCostReduction] = value; }
		//}

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Feral")]
        public float BonusShredDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusShredDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusShredDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Bonus Rip Damage Per Combo Point Per Tick")]
        [Category("Feral")]
        public float BonusRipDamagePerCPPerTick
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusRipDamagePerCPPerTick]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusRipDamagePerCPPerTick] = value; }
		}

		[System.ComponentModel.DefaultValueAttribute(0f)]
		[DisplayName("Bonus Rake Duration")]
		[Category("Feral")]
		public float BonusRakeDuration
		{
			get { return _rawAdditiveData[(int)AdditiveStat.BonusRakeDuration]; }
			set { _rawAdditiveData[(int)AdditiveStat.BonusRakeDuration] = value; }
		}

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Bonus Rip Duration")]
        [Category("Feral")]
        public float BonusRipDuration
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusRipDuration]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusRipDuration] = value; }
		}

		[System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Bonus Savage Roar Duration")]
        [Category("Feral")]
		public float BonusSavageRoarDuration
		{
			get { return _rawAdditiveData[(int)AdditiveStat.BonusSavageRoarDuration]; }
			set { _rawAdditiveData[(int)AdditiveStat.BonusSavageRoarDuration] = value; }
		}

		[System.ComponentModel.DefaultValueAttribute(0f)]
		[Percentage]
        [DisplayName("% chance to proc Clearcast On Bleed Damage")]
        [Category("Feral")]
		public float ClearcastOnBleedChance
		{
			get { return _rawAdditiveData[(int)AdditiveStat.ClearcastOnBleedChance]; }
			set { _rawAdditiveData[(int)AdditiveStat.ClearcastOnBleedChance] = value; }
        }
        #endregion

        #region Shaman Bonuses
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Windfury")]
        [Category("Deprecated")]
        public float WindfuryAPBonus
        {
            get { return _rawAdditiveData[(int)AdditiveStat.WindfuryAPBonus]; }
            set { _rawAdditiveData[(int)AdditiveStat.WindfuryAPBonus] = value; }
        }
        #endregion

        #region Misc
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Misc")]
        public float SpellDamageFromIntellectPercentage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SpellDamageFromIntellectPercentage]; }
            set { _rawAdditiveData[(int)AdditiveStat.SpellDamageFromIntellectPercentage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Misc")]
        public float SpellDamageFromSpiritPercentage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SpellDamageFromSpiritPercentage]; }
            set { _rawAdditiveData[(int)AdditiveStat.SpellDamageFromSpiritPercentage] = value; }
        }
        #endregion

        #region Deprecated
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Deprecated")]
        public float CritChanceReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.CritChanceReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.CritChanceReduction] = value; }
        }
        #endregion

        #region Equipment Effects
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% Mana or Equivalent Restore")]
        [Category("Equipment Effects")]
        [Percentage]
        public float ManaorEquivRestore
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ManaorEquivRestore]; }
            set { _rawAdditiveData[(int)AdditiveStat.ManaorEquivRestore] = value; }
        }
        /// <summary>
        /// This stat stores Health restorations such as Runic Healing Potion
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Health Restored")]
        [Category("Equipment Effects")]
        public float HealthRestore
        {
            get { return _rawAdditiveData[(int)AdditiveStat.HealthRestore]; }
            set { _rawAdditiveData[(int)AdditiveStat.HealthRestore] = value; }
        }
        /// <summary>
        /// This stat stores Health restorations such as Invigorating Earthsiege Diamond's
        /// 'Sometimes Heal on your Crits' (2% of Health Restored)
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% of Max Health Restored")]
        [Category("Equipment Effects")]
        [Percentage]
        public float HealthRestoreFromMaxHealth
        {
            get { return _rawAdditiveData[(int)AdditiveStat.HealthRestoreFromMaxHealth]; }
            set { _rawAdditiveData[(int)AdditiveStat.HealthRestoreFromMaxHealth] = value; }
        }
        /// <summary>
        /// This stat stores Mana restorations such as Runic Mana Potion
        /// or Mana Restore procs like Figurine - Talasite Owl's Use Effect
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Mana Restored")]
        [Category("Equipment Effects")]
        public float ManaRestore
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ManaRestore]; }
            set { _rawAdditiveData[(int)AdditiveStat.ManaRestore] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Equipment Effects")]
        [DisplayName("PvP Trinket")]
        public float PVPTrinket
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PVPTrinket]; }
            set { _rawAdditiveData[(int)AdditiveStat.PVPTrinket] = value; }
        }

        [DisplayName("Bonus Healing Received")]
        [Category("Equipment Effects")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float BonusHealingReceived
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusHealingReceived]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusHealingReceived] = value; }
        }

        [DisplayName("Extract of Necromantic Power proc")]
        [Category("Equipment Effects")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float ExtractOfNecromanticPowerProc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ExtractOfNecromanticPowerProc]; }
            set { _rawAdditiveData[(int)AdditiveStat.ExtractOfNecromanticPowerProc] = value; }
        }

        [DisplayName("Darkmoon Card: Death proc")]
        [Category("Equipment Effects")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float DarkmoonCardDeathProc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.DarkmoonCardDeathProc]; }
            set { _rawAdditiveData[(int)AdditiveStat.DarkmoonCardDeathProc] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% Shield from Heal Amount")]
        [Percentage]
        [Category("Equipment Effects")]
        public float ShieldFromHealed
        {
            get { return _rawNoStackData[(int)NonStackingStat.ShieldFromHealed]; }
            set { _rawNoStackData[(int)NonStackingStat.ShieldFromHealed] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Equipment Effects")]
        public float DeathbringerProc
        {
            get { return _rawNoStackData[(int)NonStackingStat.DeathbringerProc]; }
            set { _rawNoStackData[(int)NonStackingStat.DeathbringerProc] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Equipment Effects")]
        [DisplayName("% Chance to proc an extra half damage shot")]
        [Percentage]
        public float ZodProc
        {
            get { return _rawNoStackData[(int)NonStackingStat.ZodProc]; }
            set { _rawNoStackData[(int)NonStackingStat.ZodProc] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Equipment Effects")]
        [DisplayName("Temporary Health")]
        public float BattlemasterHealth
        {
            get { return _rawNoStackData[(int)NonStackingStat.BattlemasterHealth]; }
            set { _rawNoStackData[(int)NonStackingStat.BattlemasterHealth] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Equipment Effects")]
        [DisplayName("% Chance to Reflect Spell")]
        [Percentage]
        public float SpellReflectChance {
            get { return _rawAdditiveData[(int)AdditiveStat.SpellReflectChance]; }
            set { _rawAdditiveData[(int)AdditiveStat.SpellReflectChance] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Highest Stat")]
        [Category("Equipment Effects")]
        public float HighestStat
        {
            get { return _rawNoStackData[(int)NonStackingStat.HighestStat]; }
            set { _rawNoStackData[(int)NonStackingStat.HighestStat] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Strength or Agility")]
        [Category("Equipment Effects")]
        public float Paragon
        {
            get { return _rawAdditiveData[(int) AdditiveStat.Paragon]; }
            set { _rawAdditiveData[(int)AdditiveStat.Paragon] = value; }
        }
        /// <summary>
        /// This stores Healing values for targets, not self, like Forethought Talisman's Passive Effect
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Healed")]
        [Category("Equipment Effects")]
        public float Healed
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Healed]; }
            set { _rawAdditiveData[(int)AdditiveStat.Healed] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Equipment Effects")]
        [DisplayName("Damage Absorbed")]
        [CommonStat]
        public float DamageAbsorbed
        {
            get { return _rawAdditiveData[(int)AdditiveStat.DamageAbsorbed]; }
            set { _rawAdditiveData[(int)AdditiveStat.DamageAbsorbed] = value; }
        }

        #region Proc'd Damage Types
        // Physical
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Physical Damage")]
        [Category("Equipment Effects")]
        public float PhysicalDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PhysicalDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.PhysicalDamage] = value; }
        }

        // Bandit's Insignia
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Arcane Damage")]
        [Category("Equipment Effects")]
        public float ArcaneDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ArcaneDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.ArcaneDamage] = value; }
        }

        // Bandit's Insignia
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Shadow Damage")]
        [Category("Equipment Effects")]
        public float ShadowDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ShadowDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.ShadowDamage] = value; }
        }

        // Hand Mounted Pyro Rocket
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Fire Damage")]
        [Category("Equipment Effects")]
        public float FireDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.FireDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.FireDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Frost Damage")]
        [Category("Equipment Effects")]
        public float FrostDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.FrostDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.FrostDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Holy Damage")]
        [Category("Equipment Effects")]
        public float HolyDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.HolyDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.HolyDamage] = value; }
        }

        // Thunder Capacitor
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Nature Damage")]
        [Category("Equipment Effects")]
        public float NatureDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.NatureDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.NatureDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Valkyr Damage")]
        [Category("Equipment Effects")]
        public float ValkyrDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ValkyrDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.ValkyrDamage] = value; }
        }
        #endregion

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Spells Mana Cost Reduction")]
        [Category("Equipment Effects")]
        public float SpellsManaReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SpellsManaReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.SpellsManaReduction] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Next Healing Spell Mana Cost Reduction")]
        [Category("Equipment Effects")]
        public float HealingOmenProc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.HealingOmenProc]; }
            set { _rawAdditiveData[(int)AdditiveStat.HealingOmenProc] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Equipment Effects")]
        [DisplayName("% Weapon Swing")]
        [Percentage]
        public float MoteOfAnger
        {
            get { return _rawAdditiveData[(int)AdditiveStat.MoteOfAnger]; }
            set { _rawAdditiveData[(int)AdditiveStat.MoteOfAnger] = value; }
        }

        #endregion

        #region Old Equipment Procs
        // 5% proc rate, 15 sec internal cooldown
        [DisplayName("Mana Restore On Cast (5%)")]
        [Category("Old Equipment Procs")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float ManaRestoreOnCast_5_15
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ManaRestoreOnCast_5_15]; }
            set { _rawAdditiveData[(int)AdditiveStat.ManaRestoreOnCast_5_15] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Mana cost of next spell reduce (within 15 sec.)")]
        [Category("Old Equipment Procs")]
        public float ManacostReduceWithin15OnHealingCast
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ManacostReduceWithin15OnHealingCast]; }
            set { _rawAdditiveData[(int)AdditiveStat.ManacostReduceWithin15OnHealingCast] = value; }
        }
        #endregion

        #region Added by Rawr.DPSWarr
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Warrior")]
        [DisplayName("Bonus Targets")]
        public float BonusTargets
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusTargets]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusTargets] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Warrior")]
        [DisplayName("Bonus Rage Generated")]
        public float BonusRageGen
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusRageGen]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusRageGen] = value; }
        }
        #endregion
        #region Added by Rawr.Mage

        [DisplayName("Kirin Tor Garb 4 Piece Bonus")]
        [Category("Mage")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float Mage4T8
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Mage4T8]; }
            set { _rawAdditiveData[(int)AdditiveStat.Mage4T8] = value; }
        }

        [DisplayName("Khadgar's Regalia 2 Piece Bonus")]
        [Category("Mage")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float Mage2T9
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Mage2T9]; }
            set { _rawAdditiveData[(int)AdditiveStat.Mage2T9] = value; }
        }

        [DisplayName("Khadgar's Regalia 4 Piece Bonus")]
        [Category("Mage")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float Mage4T9
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Mage4T9]; }
            set { _rawAdditiveData[(int)AdditiveStat.Mage4T9] = value; }
        }

        [DisplayName("Mage T10 2 Piece Bonus")]
        [Category("Mage")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float Mage2T10
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Mage2T10]; }
            set { _rawAdditiveData[(int)AdditiveStat.Mage2T10] = value; }
        }

        [DisplayName("Mage T10 4 Piece Bonus")]
        [Category("Mage")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float Mage4T10
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Mage4T10]; }
            set { _rawAdditiveData[(int)AdditiveStat.Mage4T10] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Mana Gem Effect")]
        [Category("Mage")]
        public float BonusManaGem
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusManaGem]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusManaGem] = value; }
        }

        [DisplayName("Ice Armor")]
        [Category("Mage")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float MageIceArmor
        {
            get { return _rawAdditiveData[(int)AdditiveStat.MageIceArmor]; }
            set { _rawAdditiveData[(int)AdditiveStat.MageIceArmor] = value; }
        }

        [DisplayName("Mage Armor")]
        [Category("Mage")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float MageMageArmor
        {
            get { return _rawAdditiveData[(int)AdditiveStat.MageMageArmor]; }
            set { _rawAdditiveData[(int)AdditiveStat.MageMageArmor] = value; }
        }

        [DisplayName("Molten Armor")]
        [Category("Mage")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float MageMoltenArmor
        {
            get { return _rawAdditiveData[(int)AdditiveStat.MageMoltenArmor]; }
            set { _rawAdditiveData[(int)AdditiveStat.MageMoltenArmor] = value; }
        }
        #endregion
        #region Added by Rawr.Feral
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Feral")]
        [DisplayName("Strength in Cat Form")]
        public float CatFormStrength
        {
            get { return _rawAdditiveData[(int)AdditiveStat.CatFormStrength]; }
            set { _rawAdditiveData[(int)AdditiveStat.CatFormStrength] = value; }
        }

        #endregion
        #region Added by Rawr.Retribution
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% Seal Damage")]
        [Category("Retribution")]
        [Percentage]
        public float SealMultiplier
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SealMultiplier]; }
            set { _rawAdditiveData[(int)AdditiveStat.SealMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% Judgement Damage")]
        [Category("Retribution")]
        [Percentage]
        public float JudgementMultiplier
        {
            get { return _rawAdditiveData[(int)AdditiveStat.JudgementMultiplier]; }
            set { _rawAdditiveData[(int)AdditiveStat.JudgementMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% DS CD Reset on Melee")]
        [Category("Retribution")]
        [Percentage]
        public float DivineStormRefresh
        {
            get { return _rawAdditiveData[(int)AdditiveStat.DivineStormRefresh]; }
            set { _rawAdditiveData[(int)AdditiveStat.DivineStormRefresh] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Crusader Strike Damage")]
        [Category("Retribution")]
        public float CrusaderStrikeDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.CrusaderStrikeDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.CrusaderStrikeDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Consecration Spell Power")]
        [Category("Retribution")]
        public float ConsecrationSpellPower
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ConsecrationSpellPower]; }
            set { _rawAdditiveData[(int)AdditiveStat.ConsecrationSpellPower] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Divine Storm Damage")]
        [Category("Retribution")]
        public float DivineStormDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.DivineStormDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.DivineStormDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Divine Storm Damage")]
        [Category("Retribution")]
        public float DivineStormMultiplier
        {
            get { return _rawAdditiveData[(int)AdditiveStat.DivineStormMultiplier]; }
            set { _rawAdditiveData[(int)AdditiveStat.DivineStormMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Crusader Strike Damage")]
        [Category("Retribution")]
        public float CrusaderStrikeMultiplier
        {
            get { return _rawAdditiveData[(int)AdditiveStat.CrusaderStrikeMultiplier]; }
            set { _rawAdditiveData[(int)AdditiveStat.CrusaderStrikeMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Exorcism Damage")]
        [Category("Retribution")]
        public float ExorcismMultiplier
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ExorcismMultiplier]; }
            set { _rawAdditiveData[(int)AdditiveStat.ExorcismMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Hammer of Wrath Damage")]
        [Category("Retribution")]
        public float HammerOfWrathMultiplier
        {
            get { return _rawAdditiveData[(int)AdditiveStat.HammerOfWrathMultiplier]; }
            set { _rawAdditiveData[(int)AdditiveStat.HammerOfWrathMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Crusader Strike Crit")]
        [Category("Retribution")]
        public float CrusaderStrikeCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.CrusaderStrikeCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.CrusaderStrikeCrit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Divine Storm Crit")]
        [Category("Retribution")]
        public float DivineStormCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.DivineStormCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.DivineStormCrit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("sec Judgement CD Reduction")]
        [Category("Retribution")]
        public float JudgementCDReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.JudgementCDReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.JudgementCDReduction] = value; }
        }
        
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Righteous Vengeance Can Crit")]
        [Category("Retribution")]
        public float RighteousVengeanceCanCrit
        {
            get { return _rawNoStackData[(int)NonStackingStat.RighteousVengeanceCanCrit]; }
            set { _rawNoStackData[(int)NonStackingStat.RighteousVengeanceCanCrit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% Judgement Crit")]
        [Category("Retribution")]
        [Percentage]
        public float JudgementCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.JudgementCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.JudgementCrit] = value; }
        }        
        #endregion
        #region Added by Rawr.Healadin
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Flash of Light Spell Power")]
        [Category("Healadin")]
        public float FlashOfLightSpellPower
        {
            get { return _rawAdditiveData[(int)AdditiveStat.FlashOfLightSpellPower]; }
            set { _rawAdditiveData[(int)AdditiveStat.FlashOfLightSpellPower] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("secs off Sacred Shield ICD")]
        [Category("Healadin")]
        public float SacredShieldICDReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SacredShieldICDReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.SacredShieldICDReduction] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% HoT on Holy Shock Crit")]
        [Percentage]
        [Category("Healadin")]
        public float HolyShockHoTOnCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.HolyShockHoTOnCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.HolyShockHoTOnCrit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Flash of Light Healing")]
        [Category("Healadin")]
        public float FlashOfLightMultiplier
        {
            get { return _rawAdditiveData[(int)AdditiveStat.FlashOfLightMultiplier]; }
            set { _rawAdditiveData[(int)AdditiveStat.FlashOfLightMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Flash of Light Crit")]
        [Category("Healadin")]
        public float FlashOfLightCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.FlashOfLightCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.FlashOfLightCrit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Holy Light Spell Power")]
        [Category("Healadin")]
        public float HolyLightSpellPower
        {
            get { return _rawAdditiveData[(int)AdditiveStat.HolyLightSpellPower]; }
            set { _rawAdditiveData[(int)AdditiveStat.HolyLightSpellPower] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Holy Light Cost Reduction")]
        [Category("Healadin")]
        public float HolyLightPercentManaReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.HolyLightPercentManaReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.HolyLightPercentManaReduction] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Holy Light Mana Cost Reduction")]
        [Category("Healadin")]
        public float HolyLightManaCostReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.HolyLightManaReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.HolyLightManaReduction] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Holy Light Crit")]
        [Category("Healadin")]
        public float HolyLightCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.HolyLightCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.HolyLightCrit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Holy Shock Crit")]
        [Category("Healadin")]
        public float HolyShockCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.HolyShockCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.HolyShockCrit] = value; }
        }

        /// <summary>
        /// Tier 9 2-piece bonus
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("sec Increased Judgement Duration")]
        [Category("Healadin")]
        public float BonusJudgementDuration
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusJudgementDuration]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusJudgementDuration] = value; }
        }

        /// <summary>
        /// Tier 9 4-piece bonus
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Flash of Light HoT")]
        [Category("Healadin")]
        public float FlashOfLightHoTMultiplier
        {
            get { return _rawAdditiveData[(int)AdditiveStat.FlashOfLightHoTMultiplier]; }
            set { _rawAdditiveData[(int)AdditiveStat.FlashOfLightHoTMultiplier] = value; }
        }

        /// <summary>
        /// Tier 10 2-piece bonus
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Healing with Divine Illumination Active")]
        [Category("Healadin")]
        public float DivineIlluminationHealingMultiplier
        {
            get { return _rawAdditiveData[(int)AdditiveStat.DivineIlluminationHealingMultiplier]; }
            set { _rawAdditiveData[(int)AdditiveStat.DivineIlluminationHealingMultiplier] = value; }
        }

        /// <summary>
        /// Tier 10 4-piece bonus
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("sec Holy Light Cast Time Reduction after Holy Shock")]
        [Category("Healadin")]
        public float HolyLightCastTimeReductionFromHolyShock
        {
            get { return _rawAdditiveData[(int)AdditiveStat.HolyLightCastTimeReductionFromHolyShock]; }
            set { _rawAdditiveData[(int)AdditiveStat.HolyLightCastTimeReductionFromHolyShock] = value; }
        }

        #endregion
        #region Added by Rawr.Warlock
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Bonus Spirit after Life Tap")]
        [Category("Warlock")]
        public float Warlock4T7
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Warlock4T7]; }
            set { _rawAdditiveData[(int)AdditiveStat.Warlock4T7] = value; }
        }
        #endregion
        #region Added by Rawr.Hunter
        #region Common Stats
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Hunter")]
        [DisplayName("Ranged Crit Rating")]
        [CommonStat]
        public float RangedCritRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RangedCritRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.RangedCritRating] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Hunter")]
        [DisplayName("Ranged Hit Rating")]
        [CommonStat]
        public float RangedHitRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RangedHitRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.RangedHitRating] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Hunter")]
        [DisplayName("Ranged Haste Rating")]
        [CommonStat]
        public float RangedHasteRating
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RangedHasteRating]; }
            set { _rawAdditiveData[(int)AdditiveStat.RangedHasteRating] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Hunter")]
        [DisplayName("Ranged Attack Power")]
        [CommonStat]
        public float RangedAttackPower
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RangedAttackPower]; }
            set { _rawAdditiveData[(int)AdditiveStat.RangedAttackPower] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Hunter")]
        [CommonStat]
        public float BonusRangedAttackPowerMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusRangedAttackPowerMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusRangedAttackPowerMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Hunter")]
        [CommonStat]
        public float BonusPetAttackPowerMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusPetAttackPowerMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusPetAttackPowerMultiplier] = value; }
        }
        #endregion
        #region Set Bonuses
        /// <summary>Your ranged attack speed is increased by 20% while Aspect of the Viper is active</summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Ranged Attack Speed Increase while Aspect of the Viper is active")]
        [Category("Hunter")]
        public float BonusHunter_T7_4P_ViperSpeed
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusHunter_T7_4P_ViperSpeed]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusHunter_T7_4P_ViperSpeed] = value; }
        }
        /// <summary>Increases the damage done by your Serpent Sting ability by 10%.</summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Damage increase for Serpent Sting")]
        [Category("Hunter")]
        public float BonusHunter_T8_2P_SerpDmg
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusHunter_T8_2P_SerpDmg]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusHunter_T8_2P_SerpDmg] = value; }
        }
        /// <summary>All Trap cooldowns reduced by 2 seconds</summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("second cooldown reduction for Traps")]
        [Category("Hunter")]
        public float BonusHunter_PvP_4pc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusHunter_PvP_4pc]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusHunter_PvP_4pc] = value; }
        }
        /// <summary>Your Serpent Stings can now Critical Strike</summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Your Serpent Stings can now crit")]
        [Category("Hunter")]
        public float BonusHunter_T9_2P_SerpCanCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusHunter_T9_2P_SerpCanCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusHunter_T9_2P_SerpCanCrit] = value; }
        }
        /// <summary>Steady shot has a chance to increase pet's attack power by 600</summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Steady shot has a chance to increase pet's attack power")]
        [Category("Hunter")]
        public float BonusHunter_T9_4P_SteadyShotPetAPProc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusHunter_T9_4P_SteadyShotPetAPProc]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusHunter_T9_4P_SteadyShotPetAPProc] = value; }
        }
        #endregion
        #region Pet Bonuses
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Extra Pet Crit Chance")]
        [Category("Hunter")]
        public float BonusPetCritChance
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusPetCritChance]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusPetCritChance] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Pet Stamina")]
        [Category("Hunter")]
        public float PetStamina
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PetStamina]; }
            set { _rawAdditiveData[(int)AdditiveStat.PetStamina] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Pet Strength")]
        [Category("Hunter")]
        public float PetStrength
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PetStrength]; }
            set { _rawAdditiveData[(int)AdditiveStat.PetStrength] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Pet Attack Power")]
        [Category("Hunter")]
        public float PetAttackPower
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PetAttackPower]; }
            set { _rawAdditiveData[(int)AdditiveStat.PetAttackPower] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Pet Spirit")]
        [Category("Hunter")]
        public float PetSpirit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PetSpirit]; }
            set { _rawAdditiveData[(int)AdditiveStat.PetSpirit] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Bonus Pet Damage")]
        [Category("Hunter")]
        public float BonusPetDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusPetDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusPetDamageMultiplier] = value; }
        }
        #endregion
        #endregion
        #region Added by Rawr.Tree
        // Tier 7 (2)
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Lifebloom cost reduction")]
        [Category("Tree")]
        public float LifebloomCostReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.LifebloomCostReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.LifebloomCostReduction] = value; }
        }

        // Tier 7 (4)
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Nourish bonus per HoT")]
        [Category("Tree")]
        public float NourishBonusPerHoT
        {
            get { return _rawAdditiveData[(int)AdditiveStat.NourishBonusPerHoT]; }
            set { _rawAdditiveData[(int)AdditiveStat.NourishBonusPerHoT] = value; }
        }

        // Tier 8 2 Set bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Bonus Healing by Swiftmend")]
        [Category("Tree")]
        public float SwiftmendBonus
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SwiftmendBonus]; }
            set { _rawAdditiveData[(int)AdditiveStat.SwiftmendBonus] = value; }
        }

        // Tier 8 4 Set bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Bonus Instant Tick by Rejuvenation")]
        [Category("Tree")]
        public float RejuvenationInstantTick
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RejuvenationInstantTick]; }
            set { _rawAdditiveData[(int)AdditiveStat.RejuvenationInstantTick] = value; }
        }

        // Tier 9 2 Set bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Bonus Crit Chance for Nourish")]
        [Category("Tree")]
        public float NourishCritBonus
        {
            get { return _rawAdditiveData[(int)AdditiveStat.NourishCritBonus]; }
            set { _rawAdditiveData[(int)AdditiveStat.NourishCritBonus] = value; }
        }

        // Tier 9 4 Set Bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Rejuvenation Crit Multiplier")]
        [Category("Tree")]
        public float RejuvenationCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RejuvenationCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.RejuvenationCrit] = value; }
        }

        // Tier 10 2 Set Bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Wild Growth Reduction over Time Reduction")]
        [Category("Tree")]
        public float WildGrowthLessReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.WildGrowthLessReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.WildGrowthLessReduction] = value; }
        }

        // Tier 10 4 Set Bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Rejuvenation jump chance")]
        [Category("Tree")]
        public float RejuvJumpChance
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RejuvJumpChance]; }
            set { _rawAdditiveData[(int)AdditiveStat.RejuvJumpChance] = value; }
        }

        // Idol of Pure Thoughts (WotLK blue)
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Healing by Rejuvenation")]
        [Category("Tree")]
        public float RejuvenationHealBonus
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RejuvenationHealBonus]; }
            set { _rawAdditiveData[(int)AdditiveStat.RejuvenationHealBonus] = value; }
        }

        // Idol of Awakening (WotLK purple)
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Reduced Rejuvenation Mana Cost")]
        [Category("Tree")]
        public float ReduceRejuvenationCost
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ReduceRejuvenationCost]; }
            set { _rawAdditiveData[(int)AdditiveStat.ReduceRejuvenationCost] = value; }
        }

        // Idol of Lush Mosh (WotLK)
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Healing by Lifebloom Ticks")]
        [Category("Tree")]
        public float LifebloomTickHealBonus
        {
            get { return _rawAdditiveData[(int)AdditiveStat.LifebloomTickHealBonus]; }
            set { _rawAdditiveData[(int)AdditiveStat.LifebloomTickHealBonus] = value; }
        }

        // Idol of Health (WotLK)
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Healing Touch Final Heal")]
        [Category("Tree")]
        public float HealingTouchFinalHealBonus
        {
            get { return _rawAdditiveData[(int)AdditiveStat.HealingTouchFinalHealBonus]; }
            set { _rawAdditiveData[(int)AdditiveStat.HealingTouchFinalHealBonus] = value; }
        }

        // Idol of the Flourishing Life (WotLK)
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Nourish spell power")]
        [Category("Tree")]
        public float NourishSpellpower
        {
            get { return _rawAdditiveData[(int)AdditiveStat.NourishSpellpower]; }
            set { _rawAdditiveData[(int)AdditiveStat.NourishSpellpower] = value; }
        }

        // S7 PvP 4 Pc
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Swiftmend Cooldown Reduction")]
        [Category("Tree")]
        public float SwiftmendCdReduc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SwiftmendCdReduc]; }
            set { _rawAdditiveData[(int)AdditiveStat.SwiftmendCdReduc] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Chance to proc Revitalize")]
        [Category("Tree")]
        public float RevitalizeChance
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RevitalizeChance]; }
            set { _rawAdditiveData[(int)AdditiveStat.RevitalizeChance] = value; }
        }

        #endregion
        #region Added by Rawr.Enhance

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Enhance")]
        public float BonusSSDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusSSDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusSSDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Enhance")]
        public float BonusWFAttackPower
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusWFAttackPower]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusWFAttackPower] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Enhance T7 4 Piece Bonus")]
        [Category("Enhance")]
        public float Enhance4T7
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Enhance4T7]; }
            set { _rawAdditiveData[(int)AdditiveStat.Enhance4T7] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Enhance T7 2 Piece Bonus")]
        [Category("Enhance")]
        public float Enhance2T7
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Enhance2T7]; }
            set { _rawAdditiveData[(int)AdditiveStat.Enhance2T7] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Enhance T8 2 Piece Bonus")]
        [Category("Enhance")]
        public float Enhance2T8
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Enhance2T8]; }
            set { _rawAdditiveData[(int)AdditiveStat.Enhance2T8] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Enhance T8 4 Piece Bonus")]
        [Category("Enhance")]
        public float Enhance4T8
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Enhance4T8]; }
            set { _rawAdditiveData[(int)AdditiveStat.Enhance4T8] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Enhance T9 2 Piece Bonus")]
        [Category("Enhance")]
        public float Enhance2T9
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Enhance2T9]; }
            set { _rawAdditiveData[(int)AdditiveStat.Enhance2T9] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Enhance T9 4 Piece Bonus")]
        [Category("Enhance")]
        public float Enhance4T9
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Enhance4T9]; }
            set { _rawAdditiveData[(int)AdditiveStat.Enhance4T9] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Enhance T10 2 Piece Bonus")]
        [Category("Enhance")]
        public float Enhance2T10
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Enhance2T10]; }
            set { _rawAdditiveData[(int)AdditiveStat.Enhance2T10] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Enhance T10 4 Piece Bonus")]
        [Category("Enhance")]
        public float Enhance4T10
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Enhance4T10]; }
            set { _rawAdditiveData[(int)AdditiveStat.Enhance4T10] = value; }
        }

        #endregion
        #region Added by Rawr.Elemental
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Elemental")]
        public float BonusCritChance
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusCritChance]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusCritChance] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% bonus critical strike damage for Lava Burst")]
        [Category("Elemental")]
        public float BonusLavaBurstCritDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusLavaBurstCritDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusLavaBurstCritDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Elemental")]
        public float BonusFlameShockDuration
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusFlameShockDuration]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusFlameShockDuration] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Elemental")]
        public float BonusFlameShockDoTDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusFlameShockDoTDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusFlameShockDoTDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Elemental")]
        public float LightningBoltCostReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.LightningBoltCostReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.LightningBoltCostReduction] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Elemental")]
        public float LightningBoltDamageModifier
        {
            get { return _rawAdditiveData[(int)AdditiveStat.LightningBoltDamageModifier]; }
            set { _rawAdditiveData[(int)AdditiveStat.LightningBoltDamageModifier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Elemental")]
        public float LightningSpellPower
        {
            get { return _rawAdditiveData[(int)AdditiveStat.LightningSpellPower]; }
            set { _rawAdditiveData[(int)AdditiveStat.LightningSpellPower] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Lava Burst Bonus Base Damage")]
        [Category("Elemental")]
        public float LavaBurstBonus
        {
            get { return _rawAdditiveData[(int)AdditiveStat.LavaBurstBonus]; }
            set { _rawAdditiveData[(int)AdditiveStat.LavaBurstBonus] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Elemental")]
        public float LightningBoltCritDamageModifier
        {
            get { return _rawAdditiveData[(int)AdditiveStat.LightningBoltCritDamageModifier]; }
            set { _rawAdditiveData[(int)AdditiveStat.LightningBoltCritDamageModifier] = value; }
        }

        #endregion
        #region Added by Rawr.RestoSham
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Mp5 increase for Mana Spring")]
        [Category("RestoSham")]
        public float ManaSpringMp5Increase
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ManaSpringMp5Increase]; }
            set { _rawAdditiveData[(int)AdditiveStat.ManaSpringMp5Increase] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("second off Riptide Cooldown")]
        [Category("RestoSham")]
        public float RTCDDecrease
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RTCDDecrease]; }
            set { _rawAdditiveData[(int)AdditiveStat.RTCDDecrease] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("tenths of a second cast time off Chain Heal")]
        [Category("RestoSham")]
        public float CHCTDecrease
        {
            get { return _rawAdditiveData[(int)AdditiveStat.CHCTDecrease]; }
            set { _rawAdditiveData[(int)AdditiveStat.CHCTDecrease] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Instances of Earthliving Weapon active")]
        [Category("RestoSham")]
        public float Earthliving
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Earthliving]; }
            set { _rawAdditiveData[(int)AdditiveStat.Earthliving] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Base Healing to CH")]
        [Category("RestoSham")]
        public float TotemCHBaseHeal
        {
            get { return _rawAdditiveData[(int)AdditiveStat.TotemCHBaseHeal]; }
            set { _rawAdditiveData[(int)AdditiveStat.TotemCHBaseHeal] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("HW Cost Reduction:")]
        [Category("RestoSham")]
        public float TotemHWBaseCost
        {
            get { return _rawAdditiveData[(int)AdditiveStat.TotemHWBaseCost]; }
            set { _rawAdditiveData[(int)AdditiveStat.TotemHWBaseCost] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("CH Cost Reduction:")]
        [Category("RestoSham")]
        public float TotemCHBaseCost
        {
            get { return _rawAdditiveData[(int)AdditiveStat.TotemCHBaseCost]; }
            set { _rawAdditiveData[(int)AdditiveStat.TotemCHBaseCost] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("HW Spellpower added:")]
        [Category("RestoSham")]
        public float TotemHWSpellpower
        {
            get { return _rawAdditiveData[(int)AdditiveStat.TotemHWSpellpower]; }
            set { _rawAdditiveData[(int)AdditiveStat.TotemHWSpellpower] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("LHW Spellpower added:")]
        [Category("RestoSham")]
        public float TotemLHWSpellpower
        {
            get { return _rawAdditiveData[(int)AdditiveStat.TotemLHWSpellpower]; }
            set { _rawAdditiveData[(int)AdditiveStat.TotemLHWSpellpower] = value; }
        }
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Thunderhead Effect")]
        [Category("RestoSham")]
        public float TotemThunderhead
        {
            get { return _rawAdditiveData[(int)AdditiveStat.TotemThunderhead]; }
            set { _rawAdditiveData[(int)AdditiveStat.TotemThunderhead] = value; }
        }
        #endregion
        #region Added by Rawr.ProtPaladin

        /// <summary>
        /// Venture Co. Libram of Protection bonus
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Bonus ShoR Damage")]
        [Category("ProtPaladin")]
        public float BonusShieldOfRighteousnessDamage {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusShieldOfRighteousnessDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusShieldOfRighteousnessDamage] = value; }
        }

        /// <summary>
        /// Tier 7 4-piece bonus
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Divine Protection Duration Bonus")]
        [Category("ProtPaladin")]
        public float DivineProtectionDurationBonus {
            get { return _rawAdditiveData[(int)AdditiveStat.DivineProtectionDurationBonus]; }
            set { _rawAdditiveData[(int)AdditiveStat.DivineProtectionDurationBonus] = value; }
        }

        /// <summary>
        /// Libram of the Sacred Shield bonus
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Holy Shield Block Value")]
        [Category("ProtPaladin")]
        public float HolyShieldBlockValue {
            get { return _rawAdditiveData[(int)AdditiveStat.HolyShieldBlockValue]; }
            set { _rawAdditiveData[(int)AdditiveStat.HolyShieldBlockValue] = value; }
        }

        /// <summary>
        /// Libram of Obstruction bonus
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Judgement Block Value")]
        [Category("ProtPaladin")]
        public float JudgementBlockValue
        {
            get { return _rawAdditiveData[(int)AdditiveStat.JudgementBlockValue]; }
            set { _rawAdditiveData[(int)AdditiveStat.JudgementBlockValue] = value; }
        }
        
        /// <summary>
        /// Tier 8 4-piece bonus
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("ShoR Block Value")]
        [Category("ProtPaladin")]
        public float ShieldOfRighteousnessBlockValue
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ShieldOfRighteousnessBlockValue]; }
            set { _rawAdditiveData[(int)AdditiveStat.ShieldOfRighteousnessBlockValue] = value; }
        }
        
        #endregion
        #region Added by Rawr.Moonkin
        // Starfire idol
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Starfire damage bonus")]
        [Category("Moonkin")]
        public float StarfireDmg
        {
            get { return _rawAdditiveData[(int)AdditiveStat.StarfireDmg]; }
            set { _rawAdditiveData[(int)AdditiveStat.StarfireDmg] = value; }
        }
        // Unseen Moon idol
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Spell Damage (10 sec on Moonfire)")]
        [Category("Moonkin")]
        public float UnseenMoonDamageBonus
        {
            get { return _rawAdditiveData[(int)AdditiveStat.UnseenMoonDamageBonus]; }
            set { _rawAdditiveData[(int)AdditiveStat.UnseenMoonDamageBonus] = value; }
        }

        // Insect Swarm idol
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Insect Swarm damage bonus")]
        [Category("Moonkin")]
        public float InsectSwarmDmg
        {
            get { return _rawAdditiveData[(int)AdditiveStat.InsectSwarmDmg]; }
            set { _rawAdditiveData[(int)AdditiveStat.InsectSwarmDmg] = value; }
        }

        // Moonfire idol
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Moonfire damage bonus")]
        [Category("Moonkin")]
        public float MoonfireDmg
        {
            get { return _rawAdditiveData[(int)AdditiveStat.MoonfireDmg]; }
            set { _rawAdditiveData[(int)AdditiveStat.MoonfireDmg] = value; }
        }

        // Wrath idol
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Wrath damage bonus")]
        [Category("Moonkin")]
        public float WrathDmg
        {
            get { return _rawAdditiveData[(int)AdditiveStat.WrathDmg]; }
            set { _rawAdditiveData[(int)AdditiveStat.WrathDmg] = value; }
        }

        // Moonkin 4-piece T4 bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Innervate CD Reduction")]
        [Category("Moonkin")]
        public float InnervateCooldownReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.InnervateCooldownReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.InnervateCooldownReduction] = value; }
        }

        // Moonkin 4-piece T5 bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% Starfire Crit Bonus")]
        [Percentage]
        [Category("Moonkin")]
        public float StarfireBonusWithDot
        {
            get { return _rawAdditiveData[(int)AdditiveStat.StarfireBonusWithDot]; }
            set { _rawAdditiveData[(int)AdditiveStat.StarfireBonusWithDot] = value; }
        }

        // Moonkin 2-piece T6 bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Moonfire Extension")]
        [Category("Moonkin")]
        public float MoonfireExtension
        {
            get { return _rawAdditiveData[(int)AdditiveStat.MoonfireExtension]; }
            set { _rawAdditiveData[(int)AdditiveStat.MoonfireExtension] = value; }
        }
        // Moonkin 4-piece T6 bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% Starfire Crit")]
        [Percentage]
        [Category("Moonkin")]
        public float StarfireCritChance
        {
            get { return _rawAdditiveData[(int)AdditiveStat.StarfireCritChance]; }
            set { _rawAdditiveData[(int)AdditiveStat.StarfireCritChance] = value; }
        }

        // Moonkin 2-piece T7 bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% Insect Swarm Damage")]
        [Percentage]
        [Category("Moonkin")]
        public float BonusInsectSwarmDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusInsectSwarmDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusInsectSwarmDamage] = value; }
        }

        // Moonkin 4-piece T7 bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% SF/W Crit")]
        [Percentage]
        [Category("Moonkin")]
        public float BonusNukeCritChance
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusNukeCritChance]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusNukeCritChance] = value; }
        }

        // Moonkin 2-piece T8 bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% Eclipse Bonus")]
        [Percentage]
        [Category("Moonkin")]
        public float EclipseBonus
        {
            get { return _rawAdditiveData[(int)AdditiveStat.EclipseBonus]; }
            set { _rawAdditiveData[(int)AdditiveStat.EclipseBonus] = value; }
        }
        // Moonkin 4-piece T8 bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Starfire Proc")]
        [Category("Moonkin")]
        public float StarfireProc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.StarfireProc]; }
            set { _rawAdditiveData[(int)AdditiveStat.StarfireProc] = value; }
        }
        // Moonkin 2-piece T9 bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Moonfire Dot Crit")]
        [Category("Moonkin")]
        public float MoonfireDotCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.MoonfireDotCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.MoonfireDotCrit] = value; }
        }
        // Moonkin 4-piece T9 bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% SF/W Damage")]
        [Percentage]
        [Category("Moonkin")]
        public float BonusMoonkinNukeDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusMoonkinNukeDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusMoonkinNukeDamage] = value; }
        }
        // Moonkin 4-piece T10 bonus
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% Crit Damage As Dot")]
        [Percentage]
        [Category("Moonkin")]
        public float MoonkinT10CritDot
        {
            get { return _rawAdditiveData[(int)AdditiveStat.MoonkinT10CritDot]; }
            set { _rawAdditiveData[(int)AdditiveStat.MoonkinT10CritDot] = value; }
        }
        #endregion
        #endregion

        #region MultiplicativeStats
        // threat dealt is damage * (1 + ThreatIncreaseMultiplier) * (1 - ThreatReductionMultiplier)
        [Percentage]
        [DisplayName("% Threat Increase")]
        [Category("Buffs / Debuffs")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float ThreatIncreaseMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.ThreatIncreaseMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.ThreatIncreaseMultiplier] = value; }
        }

        [Percentage]
        [DisplayName("% Boss Attack Speed Reduction")]
        [Category("Buffs / Debuffs")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float BossAttackSpeedMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BossAttackSpeedMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BossAttackSpeedMultiplier] = value; }
        }

        [Percentage]
        [Category("Equipment Effects")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% Mana")]
        public float BonusManaMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusManaMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusManaMultiplier] = value; }
        }

        [Percentage]
        [Category("Equipment Effects")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% Crit Heal")]
        public float BonusCritHealMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusCritHealMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusCritHealMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Bonus Bleed Damage Multiplier")]
        [Category("Buffs / Debuffs")]
        public float BonusBleedDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusBleedDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusBleedDamageMultiplier] = value; }
        }

        #region Buffs/Debuffs
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Agility")]
        public float BonusAgilityMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusAgilityMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusAgilityMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Strength")]
        public float BonusStrengthMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusStrengthMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusStrengthMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Stamina")]
        public float BonusStaminaMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusStaminaMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusStaminaMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Health")]
        public float BonusHealthMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusHealthMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusHealthMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Int")]
        public float BonusIntellectMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusIntellectMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusIntellectMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Armor")]
        public float BonusArmorMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusArmorMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusArmorMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Base Armor")]
        public float BaseArmorMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BaseArmorMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BaseArmorMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Block Value")]
        public float BonusBlockValueMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusBlockValueMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusBlockValueMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% AP")]
        public float BonusAttackPowerMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusAttackPowerMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusAttackPowerMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% SP")]
        public float BonusSpellPowerMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusSpellPowerMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusSpellPowerMultiplier] = value; }
        }
        #endregion

        #region Added by Rawr.Warlock
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% increase to the critical strike chance of your next Shadowbolt or Incinerate spell")]
        [Category("Warlock")]
        public float Warlock2T7
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.Warlock2T7]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.Warlock2T7] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% damage bonus on Unstable Affliction and 10% on Immolate")]
        [Category("Warlock")]
        public float Warlock2T8
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.Warlock2T8]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.Warlock2T8] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% crit chance on Shadowbolt and Incinerate")]
        [Category("Warlock")]
        public float Warlock4T8
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.Warlock4T8]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.Warlock4T8] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% increase to your pet's critical strike chance with its abilities")]
        [Category("Warlock")]
        public float Warlock2T9
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.Warlock2T9]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.Warlock2T9] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% increase to damage done by your Immolate, Corruption and Unstable Affliction spells")]
        [Category("Warlock")]
        public float Warlock4T9
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.Warlock4T9]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.Warlock4T9] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% increased critical strike chance to your Shadowbolt, Incinerate, Soulfire & Corruption spells")]
        [Category("Warlock")]
        public float Warlock2T10
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.Warlock2T10]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.Warlock2T10] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% chance for you & your pet to gain 10% increased damage for 10 seconds\r\neach time your Immolate and Unstable Affliction spells deals periodic damage.")]
        [Category("Warlock")]
        public float Warlock4T10
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.Warlock4T10]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.Warlock4T10] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Warlock")]
        [DisplayName("% Warlock Spell Power")]
        public float BonusSpellPowerDemonicPactMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusSpellPowerDemonicPactMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusSpellPowerDemonicPactMultiplier] = value; }
        }
        #endregion
        #region Added by Rawr.DPSWarr
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Bonus Rage on Crit")]
        [Category("Warrior")]
        public float BonusRageOnCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusRageOnCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusRageOnCrit] = value; }
        }
        
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Shield Slam Damage")]
        [Category("Warrior")]
        public float BonusShieldSlamDamage
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusShieldSlamDamage]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusShieldSlamDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Warrior")]
        [DisplayName("% increased critical strike chance on Devastate")]
        public float DevastateCritIncrease
        {
            get { return _rawAdditiveData[(int)AdditiveStat.DevastateCritIncrease]; }
            set { _rawAdditiveData[(int)AdditiveStat.DevastateCritIncrease] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Warrior")]
        [DisplayName("% Devastate Damage")]
        public float BonusDevastateDamage
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusDevastateDamage]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusDevastateDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Warrior")]
        [DisplayName("% Shockwave Damage")]
        public float BonusShockwaveDamage
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusShockwaveDamage]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusShockwaveDamage] = value; }
        }

        /// <summary>Increases the Damage of your Slam by 10%</summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Warrior")]
        [DisplayName("% Slam Damage")]
        public float BonusWarrior_T7_2P_SlamDamage
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusWarrior_T7_2P_SlamDamage]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusWarrior_T7_2P_SlamDamage] = value; }
        }

        /// <summary>Your Bleed periodic effects have a chance to make your next ability cost 5 less rage.</summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Warrior")]
        [DisplayName("Rage Cost Reduction Proc")]
        public float BonusWarrior_T7_4P_RageProc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusWarrior_T7_4P_RageProc]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusWarrior_T7_4P_RageProc] = value; }
        }

        /// <summary>Heroic Strike and Slam critical strikes have a chance to grant you 150 haste rating for 5 sec.</summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Warrior")]
        [DisplayName("bonus haste rating chance on Heroic Strike and Slam crits")]
        public float BonusWarrior_T8_2P_HasteProc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusWarrior_T8_2P_HasteProc]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusWarrior_T8_2P_HasteProc] = value; }
        }

        /// <summary>Increases the critical strike chance of Mortal Strike and Bloodthirst by 10%</summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Warrior")]
        [DisplayName("% increased critical strike chance on Mortal Strike and Bloodthirst")]
        public float BonusWarrior_T8_4P_MSBTCritIncrease
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusWarrior_T8_4P_MSBTCritIncrease]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusWarrior_T8_4P_MSBTCritIncrease] = value; }
        }

        /// <summary>Berserker Stance grants an additional 2% crit chance.</summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Warrior")]
        [DisplayName("% increased Berserker Stance Crit Bonus")]
        public float BonusWarrior_T9_2P_Crit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusWarrior_T9_2P_Crit]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusWarrior_T9_2P_Crit] = value; }
        }

        /// <summary>Battle Stance grants an additional 6% armor penetration.</summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Warrior")]
        [DisplayName("% increased Battle Stance ArP Bonus")]
        public float BonusWarrior_T9_2P_ArP
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusWarrior_T9_2P_ArP]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusWarrior_T9_2P_ArP] = value; }
        }

        /// <summary>Increases the critical strike chance of your Slam and Heroic Strike abilities by 5%.</summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Warrior")]
        [DisplayName("% increased critical strike chance on Slam and Heroic Strike")]
        public float BonusWarrior_T9_4P_SLHSCritIncrease
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusWarrior_T9_4P_SLHSCritIncrease]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusWarrior_T9_4P_SLHSCritIncrease] = value; }
        }

        /// <summary>When your Deep Wounds ability deals damage you have a 3% chance to gain 16% attack power for 10 sec.</summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Warrior")]
        [DisplayName(" 3% chance on Deep Wounds damage to grant 16% Bonus Attack Power for 10 sec.")]
        public float BonusWarrior_T10_2P_DWAPProc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusWarrior_T10_2P_DWAPProc]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusWarrior_T10_2P_DWAPProc] = value; }
        }

        /// <summary>
        /// You have a 20% chance for your Bloodsurge and Sudden Death talents to grant 2
        /// charges of their effect instead of 1, reduce the global cooldown on Execute or
        /// Slam by 0.5 sec, and for the duration of the effect to be increased by 100%.
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Warrior")]
        [DisplayName("extra Sudden Death or Blood Surge charge per proc and 0.5 GCD reduction on Slams/Executes and double effect SD/BS durations")]
        public float BonusWarrior_T10_4P_BSSDProcChange
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusWarrior_T10_4P_BSSDProcChange]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusWarrior_T10_4P_BSSDProcChange] = value; }
        }

        /// <summary>Your Intercept abilities cooldown is reduced by 5 sec.</summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Warrior")]
        [DisplayName("second Intercept Cooldown Reduction")]
        public float BonusWarrior_PvP_4P_InterceptCDReduc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusWarrior_PvP_4P_InterceptCDReduc]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusWarrior_PvP_4P_InterceptCDReduc] = value; }
        }

        #endregion
        #region Added by Rawr.Feral
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% Mangle (Bear) Threat")]
        [Category("Feral")]
        public float BonusMangleBearThreat
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusMangleBearThreat]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusMangleBearThreat] = value; }
		}

		[System.ComponentModel.DefaultValueAttribute(0f)]
		[Percentage]
		[DisplayName("% Lacerate Dmg")]
		[Category("Feral")]
		public float BonusLacerateDamageMultiplier
		{
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusLacerateDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusLacerateDamageMultiplier] = value; }
		}

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Feral")]
        [DisplayName("% Rip Dmg")]
        public float BonusRipDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusRipDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusRipDamageMultiplier] = value; }
        }

		//[System.ComponentModel.DefaultValueAttribute(0f)]
		//[Percentage]
		//[Category("Feral")]
		//[DisplayName("% Ferocious Bite Dmg")]
		//public float BonusFerociousBiteDamageMultiplier
		//{
		//    get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusFerociousBiteDamageMultiplier]; }
		//    set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusFerociousBiteDamageMultiplier] = value; }
		//}

		//[System.ComponentModel.DefaultValueAttribute(0f)]
		//[Percentage]
		//[Category("Feral")]
		//[DisplayName("% Swipe Dmg")]
		//public float BonusSwipeDamageMultiplier
		//{
		//    get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusSwipeDamageMultiplier]; }
		//    set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusSwipeDamageMultiplier] = value; }
		//}

		//[System.ComponentModel.DefaultValueAttribute(0f)]
		//[Percentage]
		//[Category("Feral")]
		//[DisplayName("% Mangle Dmg")]
		//public float BonusMangleDamageMultiplier
		//{
		//    get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusMangleDamageMultiplier]; }
		//    set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusMangleDamageMultiplier] = value; }
		//}

		//[System.ComponentModel.DefaultValueAttribute(0f)]
		//[Percentage]
		//[Category("Feral")]
		//[DisplayName("% Maul Dmg")]
		//public float BonusMaulDamageMultiplier
		//{
		//    get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusMaulDamageMultiplier]; }
		//    set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusMaulDamageMultiplier] = value; }
		//}

		//[System.ComponentModel.DefaultValueAttribute(0f)]
		//[Percentage]
		//[Category("Feral")]
		//[DisplayName("% Rake Dmg")]
		//public float BonusRakeDamageMultiplier
		//{
		//    get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusRakeDamageMultiplier]; }
		//    set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusRakeDamageMultiplier] = value; }
		//}

		//[System.ComponentModel.DefaultValueAttribute(0f)]
		//[Percentage]
		//[Category("Feral")]
		//[DisplayName("% Shred Dmg")]
		//public float BonusShredDamageMultiplier
		//{
		//    get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusShredDamageMultiplier]; }
		//    set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusShredDamageMultiplier] = value; }
		//}

		//[System.ComponentModel.DefaultValueAttribute(0f)]
		//[Percentage]
		//[Category("Feral")]
		//[DisplayName("% Enrage Dmg")]
		//public float BonusEnrageDamageMultiplier
		//{
		//    get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusEnrageDamageMultiplier]; }
		//    set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusEnrageDamageMultiplier] = value; }
		//}

		[System.ComponentModel.DefaultValueAttribute(0f)]
		[DisplayName("Bonus Rip Crit Chance")]
		[Category("Feral")]
		public float BonusRipCrit
		{
			get { return _rawAdditiveData[(int)AdditiveStat.BonusRipCrit]; }
			set { _rawAdditiveData[(int)AdditiveStat.BonusRipCrit] = value; }
		}

		[System.ComponentModel.DefaultValueAttribute(0f)]
		[Percentage]
		[Category("Feral")]
		[DisplayName("% Swipe (Bear) Dmg")]
		public float BonusBearSwipeDamageMultiplier
		{
			get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusBearSwipeDamageMultiplier]; }
			set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusBearSwipeDamageMultiplier] = value; }
		}

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Bonus Rake Bleed Can Crit")]
        [Category("Feral")]
        public float BonusRakeCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusRakeCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusRakeCrit] = value; }
        }
        
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Bonus Ferocious Bite Crit Chance")]
        [Category("Feral")]
        public float BonusFerociousBiteCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.BonusFerociousBiteCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.BonusFerociousBiteCrit] = value; }
        }

		//[System.ComponentModel.DefaultValueAttribute(0f)]
		//[Category("Feral")]
		//[DisplayName("Bonus CP on Crit")]
		//public float BonusCPOnCrit
		//{
		//    get { return _rawAdditiveData[(int)AdditiveStat.BonusCPOnCrit]; }
		//    set { _rawAdditiveData[(int)AdditiveStat.BonusCPOnCrit] = value; }
		//}

		//[System.ComponentModel.DefaultValueAttribute(0f)]
		//[Category("Feral")]
		//[DisplayName("Mangle Cooldown Reduction")]
		//public float MangleCooldownReduction
		//{
		//    get { return _rawAdditiveData[(int)AdditiveStat.MangleCooldownReduction]; }
		//    set { _rawAdditiveData[(int)AdditiveStat.MangleCooldownReduction] = value; }
		//}

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Feral")]
        [DisplayName("Tiger's Fury Cooldown Reduction")]
        public float TigersFuryCooldownReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.TigersFuryCooldownReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.TigersFuryCooldownReduction] = value; }
        }

		//[System.ComponentModel.DefaultValueAttribute(0f)]
		//[DisplayName("Bonus Energy On Tiger's Fury")]
		//[Category("Feral")]
		//public float BonusEnergyOnTigersFury
		//{
		//    get { return _rawAdditiveData[(int)AdditiveStat.BonusEnergyOnTigersFury]; }
		//    set { _rawAdditiveData[(int)AdditiveStat.BonusEnergyOnTigersFury] = value; }
		//}

		//[System.ComponentModel.DefaultValueAttribute(0f)]
		//[DisplayName("Finisher Energy On Avoid")]
		//[Category("Feral")]
		//public float FinisherEnergyOnAvoid
		//{
		//    get { return _rawAdditiveData[(int)AdditiveStat.FinisherEnergyOnAvoid]; }
		//    set { _rawAdditiveData[(int)AdditiveStat.FinisherEnergyOnAvoid] = value; }
		//}
        #endregion
        #region Added by Rawr.HealPriest
        /* See SpellCombatManaRegeneration, a stat that does exactly what the Primal Mooncloth set does
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Multiplicative]
        [DisplayName("% Bonus Manaregen While Casting")]
        public float BonusManaregenWhileCastingMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusManaregenWhileCastingMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusManaregenWhileCastingMultiplier] = value; }
        }*/

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Inner Fire (Priest)")]
        [Category("Priest")]
        public float PriestInnerFire
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PriestInnerFire]; }
            set { _rawAdditiveData[(int)AdditiveStat.PriestInnerFire] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("seconds increased duration of Shadow Word: Pain (Priest)")]
        [Category("Priest")]
        public float SWPDurationIncrease
        {
            get { return _rawAdditiveData[(int)AdditiveStat.SWPDurationIncrease]; }
            set { _rawAdditiveData[(int)AdditiveStat.SWPDurationIncrease] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Priest")]
        [DisplayName("% increased damage from Mind Blast (Priest)")]
        public float BonusMindBlastMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusMindBlastMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusMindBlastMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Priest")]
        [DisplayName("% reduced cost on Mind Blast (Priest)")]
        public float MindBlastCostReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.MindBlastCostReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.MindBlastCostReduction] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Priest")]
        [DisplayName("% increased critical hit chance on Shadow Word: Death (Priest)")]
        public float ShadowWordDeathCritIncrease
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ShadowWordDeathCritIncrease]; }
            set { _rawAdditiveData[(int)AdditiveStat.ShadowWordDeathCritIncrease] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Priest")]
        [DisplayName("% increased damage on Devouring Plague. (Priest)")]
        public float DevouringPlagueBonusDamage
        {
            get { return _rawAdditiveData[(int)AdditiveStat.DevouringPlagueBonusDamage]; }
            set { _rawAdditiveData[(int)AdditiveStat.DevouringPlagueBonusDamage] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Priest")]
        [DisplayName("Extra haste rating for 4 seconds after casting Mind Blast. (Priest)")]
        public float MindBlastHasteProc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.MindBlastHasteProc]; }
            set { _rawAdditiveData[(int)AdditiveStat.MindBlastHasteProc] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Priest")]
        [DisplayName(" seconds increased duration of Vampiric Touch. (Priest)")]
        public float PriestDPS_T9_2pc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PriestDPS_T9_2pc]; }
            set { _rawAdditiveData[(int)AdditiveStat.PriestDPS_T9_2pc] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Priest")]
        [DisplayName("% increased critical hit chance on Mind Flay. (Priest)")]
        public float PriestDPS_T9_4pc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PriestDPS_T9_4pc]; }
            set { _rawAdditiveData[(int)AdditiveStat.PriestDPS_T9_4pc] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Priest")]
        [DisplayName("% increased critical hit chance on Shadow Word: Pain, Vampiric Touch and Devouring Plague. (Priest)")]
        public float PriestDPS_T10_2pc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PriestDPS_T10_2pc]; }
            set { _rawAdditiveData[(int)AdditiveStat.PriestDPS_T10_2pc] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Priest")]
        [DisplayName(" seconds reduced cast time on Mind Flay. (Priest)")]
        public float PriestDPS_T10_4pc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PriestDPS_T10_4pc]; }
            set { _rawAdditiveData[(int)AdditiveStat.PriestDPS_T10_4pc] = value; }
        }


        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Priest")]
        [DisplayName("% reduce the mana cost of PoH (Priest)")]
        public float BonusPoHManaCostReductionMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusPoHManaCostReductionMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusPoHManaCostReductionMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("Priest")]
        [DisplayName("% increased healing from Greater Heal (Priest)")]
        public float BonusGHHealingMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusGHHealingMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusGHHealingMultiplier] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("seconds increased duration of Renew (Priest)")]
        [Category("Priest")]
        public float RenewDurationIncrease
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RenewDurationIncrease]; }
            set { _rawAdditiveData[(int)AdditiveStat.RenewDurationIncrease] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Mana gained on Greater Heal Overheals (Priest)")]
        [Category("Priest")]
        public float ManaGainOnGreaterHealOverheal
        {
            get { return _rawAdditiveData[(int)AdditiveStat.ManaGainOnGreaterHealOverheal]; }
            set { _rawAdditiveData[(int)AdditiveStat.ManaGainOnGreaterHealOverheal] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName(" extra jumps on Prayer of Mending (Priest)")]
        [Category("Priest")]
        public float PrayerOfMendingExtraJumps
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PrayerOfMendingExtraJumps]; }
            set { _rawAdditiveData[(int)AdditiveStat.PrayerOfMendingExtraJumps] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% reduced cost of Greater Heal (Priest)")]
        [Category("Priest")]
        [Percentage]
        public float GreaterHealCostReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.GreaterHealCostReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.GreaterHealCostReduction] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("seconds reduced duration of Weakened Soul (Priest)")]
        [Category("Priest")]
        public float WeakenedSoulDurationDecrease
        {
            get { return _rawAdditiveData[(int)AdditiveStat.WeakenedSoulDurationDecrease]; }
            set { _rawAdditiveData[(int)AdditiveStat.WeakenedSoulDurationDecrease] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% increased critical chance on Prayer of Healing. (Priest)")]
        [Category("Priest")]
        [Percentage]
        public float PrayerOfHealingExtraCrit
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PrayerOfHealingExtraCrit]; }
            set { _rawAdditiveData[(int)AdditiveStat.PrayerOfHealingExtraCrit] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Bonus spell power for 5 seconds after casting Power Word: Shield. (Priest)")]
        [Category("Priest")]
        public float PWSBonusSpellPowerProc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PWSBonusSpellPowerProc]; }
            set { _rawAdditiveData[(int)AdditiveStat.PWSBonusSpellPowerProc] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% increased healing on Prayer of Mending. (Priest)")]
        [Category("Priest")]
        public float PriestHeal_T9_2pc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PriestHeal_T9_2pc]; }
            set { _rawAdditiveData[(int)AdditiveStat.PriestHeal_T9_2pc] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% increased effect on Divine Aegis and Empowered Renew. (Priest)")]
        [Category("Priest")]
        public float PriestHeal_T9_4pc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PriestHeal_T9_4pc]; }
            set { _rawAdditiveData[(int)AdditiveStat.PriestHeal_T9_4pc] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% chance of causing to heal for 33% of healed amount over 9 sec. (Priest)")]
        [Category("Priest")]
        public float PriestHeal_T10_2pc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PriestHeal_T10_2pc]; }
            set { _rawAdditiveData[(int)AdditiveStat.PriestHeal_T10_2pc] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% increased effect from Renew and Power Word: Shield. (Priest)")]
        [Category("Priest")]
        public float PriestHeal_T10_4pc
        {
            get { return _rawAdditiveData[(int)AdditiveStat.PriestHeal_T10_4pc]; }
            set { _rawAdditiveData[(int)AdditiveStat.PriestHeal_T10_4pc] = value; }
        }


        #endregion
        #region Added by Rawr.Elemental
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [DisplayName("% bonus damage for Lava Burst")]
        [Category("Elemental")]
        public float BonusLavaBurstDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusLavaBurstDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusLavaBurstDamageMultiplier] = value; }
        }
        [System.ComponentModel.DefaultValue(0f)]
        [Category("Elemental")]
        [DisplayName("Your Lightning Bolt and Chain Lightning spells reduce the remaining cooldown on your Elemental Mastery talent by 2 sec.")]
        public float Elemental2T10
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Elemental2T10]; }
            set { _rawAdditiveData[(int)AdditiveStat.Elemental2T10] = value; }
        }

        [System.ComponentModel.DefaultValue(0f)]
        [Category("Elemental")]
        [DisplayName("Your Lava Burst spell causes your Flame Shock effect on the target to deal at least two additional periodic damage ticks before expiring.")]
        public float Elemental4T10
        {
            get { return _rawAdditiveData[(int)AdditiveStat.Elemental4T10]; }
            set { _rawAdditiveData[(int)AdditiveStat.Elemental4T10] = value; }
        }

        #endregion
        #region Added by Rawr.Restosham
        // Tier 7 Shaman Set
        [System.ComponentModel.DefaultValue(0f)]
        [Category("RestoSham")]
        [DisplayName("Increases the healing done by your Chain Heal and Healing Wave by 5%")]
        [Percentage]
        public float CHHWHealIncrease
        {
            get { return _rawAdditiveData[(int)AdditiveStat.CHHWHealIncrease]; }
            set { _rawAdditiveData[(int)AdditiveStat.CHHWHealIncrease] = value; }
        }
        [System.ComponentModel.DefaultValue(0f)]
        [Category("RestoSham")]
        [DisplayName("Your Water Shield is 10% stronger")]
        [Percentage]
        public float WaterShieldIncrease
        {
            get { return _rawAdditiveData[(int)AdditiveStat.WaterShieldIncrease]; }
            set { _rawAdditiveData[(int)AdditiveStat.WaterShieldIncrease] = value; }
        }

        [System.ComponentModel.DefaultValue(0f)]
        [Category("RestoSham")]
        [DisplayName("Reduce Lesser Healing Wave mana cost by 5%")]
        public float LHWManaReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.LHWManaReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.LHWManaReduction] = value; }
        }

        [System.ComponentModel.DefaultValue(0f)]
        [Category("RestoSham")]
        [DisplayName("Reduce Chain Heal mana cost by 10%")]
        public float CHManaReduction
        {
            get { return _rawAdditiveData[(int)AdditiveStat.CHManaReduction]; }
            set { _rawAdditiveData[(int)AdditiveStat.CHManaReduction] = value; }
        }

        [System.ComponentModel.DefaultValue(0f)]
        [Category("RestoSham")]
        [DisplayName("Increase healing done by Chain Heal by 5%")]
        public float CHHealIncrease
        {
            get { return _rawAdditiveData[(int)AdditiveStat.CHHealIncrease]; }
            set { _rawAdditiveData[(int)AdditiveStat.CHHealIncrease] = value; }
        }

        [System.ComponentModel.DefaultValue(0f)]
        [Category("RestoSham")]
        [DisplayName("Increases the healing done by your Riptide spell by 20%.")]
        public float RestoSham2T9
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RestoSham2T9]; }
            set { _rawAdditiveData[(int)AdditiveStat.RestoSham2T9] = value; }
        }

        [System.ComponentModel.DefaultValue(0f)]
        [Category("RestoSham")]
        [DisplayName("Increases the critical strike chance of your Chain Heal spell by 5%.")]
        public float RestoSham4T9
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RestoSham4T9]; }
            set { _rawAdditiveData[(int)AdditiveStat.RestoSham4T9] = value; }
        }

        [System.ComponentModel.DefaultValue(0f)]
        [Category("RestoSham")]
        [DisplayName("Your Riptide spell grants 20% spell haste for your next spellcast within 10 sec.")]
        public float RestoSham2T10
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RestoSham2T10]; }
            set { _rawAdditiveData[(int)AdditiveStat.RestoSham2T10] = value; }
        }

        [System.ComponentModel.DefaultValue(0f)]
        [Category("RestoSham")]
        [DisplayName("Your Chain Heal critical strikes cause the target to heal for 25% of the healed amount over 9 sec.")]
        public float RestoSham4T10
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RestoSham4T10]; }
            set { _rawAdditiveData[(int)AdditiveStat.RestoSham4T10] = value; }
        }

        [System.ComponentModel.DefaultValue(0f)]
        [Category("RestoSham")]
        [DisplayName("Each time you cast Chain Heal, you have a chance to gain 234 spell power for 15 sec.")]
        public float RestoShamRelicT9
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RestoShamRelicT9]; }
            set { _rawAdditiveData[(int)AdditiveStat.RestoShamRelicT9] = value; }
        }

        [System.ComponentModel.DefaultValue(0f)]
        [Category("RestoSham")]
        [DisplayName("Your Riptide spell grants 85 spell power for 15 sec.  Stacks up to 3 times.")]
        public float RestoShamRelicT10
        {
            get { return _rawAdditiveData[(int)AdditiveStat.RestoShamRelicT10]; }
            set { _rawAdditiveData[(int)AdditiveStat.RestoShamRelicT10] = value; }
        }
        
        #endregion
        #region Added by Rawr.ProtPaladin

        /// <summary>
        /// Tier 7, 9, and 10 2-piece bonus
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("ProtPaladin")]
        [DisplayName("% Hammer of the Righteous Damage")]
        public float BonusHammerOfTheRighteousMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.HammerOfTheRighteousMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.HammerOfTheRighteousMultiplier] = value; }
        }
        
        /// <summary>
        /// Tier 8 2-piece bonus
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("ProtPaladin")]
        [DisplayName("% Seal of Corruption Damage")]
        public float BonusSealOfCorruptionDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusSealOfCorruptionDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusSealOfCorruptionDamageMultiplier] = value; }
        }

        /// <summary>
        /// Tier 8 2-piece bonus
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("ProtPaladin")]
        [DisplayName("% Seal of Righteousness Damage")]
        public float BonusSealOfRighteousnessDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusSealOfRighteousnessDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusSealOfRighteousnessDamageMultiplier] = value; }
        }

        /// <summary>
        /// Tier 8 2-piece bonus
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        [Category("ProtPaladin")]
        [DisplayName("% Seal of Vengeance Damage")]
        public float BonusSealOfVengeanceDamageMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.BonusSealOfVengeanceDamageMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.BonusSealOfVengeanceDamageMultiplier] = value; }
        }
        
        #endregion

        #endregion

        #region InverseMultiplicativeStats
        // threat dealt is damage * (1 + ThreatIncreaseMultiplier) * (1 - ThreatReductionMultiplier)
        [Percentage]
        [Category("Buffs / Debuffs")]
        [DisplayName("% Threat Reduction")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float ThreatReductionMultiplier
        {
            get { return _rawInverseMultiplicativeData[(int)InverseMultiplicativeStat.ThreatReductionMultiplier]; }
            set { _rawInverseMultiplicativeData[(int)InverseMultiplicativeStat.ThreatReductionMultiplier] = value; }
        }

        [Percentage]
        [DisplayName("% Damage Taken")]
        [Category("Buffs / Debuffs")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float DamageTakenMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.DamageTakenMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.DamageTakenMultiplier] = value; }
        }

        [Percentage]
        [DisplayName("% Spell Damage Taken")]
        [Category("Buffs / Debuffs")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float SpellDamageTakenMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.SpellDamageTakenMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.SpellDamageTakenMultiplier] = value; }
        }

        [Percentage]
        [DisplayName("% Physical Damage Taken")]
        [Category("Buffs / Debuffs")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        public float PhysicalDamageTakenMultiplier
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.PhysicalDamageTakenMultiplier]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.PhysicalDamageTakenMultiplier] = value; }
        }

        [DisplayName("% Stun Duration Reduction")]
        [Category("Buffs / Debuffs")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        public float StunDurReduc
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.StunDurReduc]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.StunDurReduc] = value; }
        }

        [DisplayName("% Snare/Root Duration Reduction")]
        [Category("Buffs / Debuffs")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        public float SnareRootDurReduc
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.SnareRootDurReduc]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.SnareRootDurReduc] = value; }
        }

        [DisplayName("% Fear Duration Reduction")]
        [Category("Buffs / Debuffs")]
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Percentage]
        public float FearDurReduc
        {
            get { return _rawMultiplicativeData[(int)MultiplicativeStat.FearDurReduc]; }
            set { _rawMultiplicativeData[(int)MultiplicativeStat.FearDurReduc] = value; }
        }
        #endregion

        #region NoStackStats
        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("% Increased Mana Potion")]
        [Percentage]
        [Category("Equipment Effects")]
        public float BonusManaPotion
        {
            get { return _rawNoStackData[(int)NonStackingStat.BonusManaPotion]; }
            set { _rawNoStackData[(int)NonStackingStat.BonusManaPotion] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Frost Res")]
        [Category("Resistances")]
        public float FrostResistanceBuff
        {
            get { return _rawNoStackData[(int)NonStackingStat.FrostResistanceBuff]; }
            set { _rawNoStackData[(int)NonStackingStat.FrostResistanceBuff] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Nature Res")]
        [Category("Resistances")]
        public float NatureResistanceBuff
        {
            get { return _rawNoStackData[(int)NonStackingStat.NatureResistanceBuff]; }
            set { _rawNoStackData[(int)NonStackingStat.NatureResistanceBuff] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Fire Res")]
        [Category("Resistances")]
        public float FireResistanceBuff
        {
            get { return _rawNoStackData[(int)NonStackingStat.FireResistanceBuff]; }
            set { _rawNoStackData[(int)NonStackingStat.FireResistanceBuff] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Shadow Res")]
        [Category("Resistances")]
        public float ShadowResistanceBuff
        {
            get { return _rawNoStackData[(int)NonStackingStat.ShadowResistanceBuff]; }
            set { _rawNoStackData[(int)NonStackingStat.ShadowResistanceBuff] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [DisplayName("Arcane Res")]
        [Category("Resistances")]
        public float ArcaneResistanceBuff
        {
            get { return _rawNoStackData[(int)NonStackingStat.ArcaneResistanceBuff]; }
            set { _rawNoStackData[(int)NonStackingStat.ArcaneResistanceBuff] = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(0f)]
        [Category("Misc")]
        [DisplayName("% Increased Movement Speed")]
        [Percentage]
        public float MovementSpeed
        {
            get { return _rawNoStackData[(int)NonStackingStat.MovementSpeed]; }
            set { _rawNoStackData[(int)NonStackingStat.MovementSpeed] = value; }
        }

        #endregion

        private void EnsureSpecialEffectCapacity(int min)
        {
            if (_rawSpecialEffectData == null || _rawSpecialEffectData.Length < min)
            {
                int num = (_rawSpecialEffectData == null || _rawSpecialEffectData.Length == 0) ? _defaultSpecialEffectDataCapacity : (_rawSpecialEffectData.Length * 2);
                if (num < min)
                {
                    num = min;
                }
                SpecialEffect[] destinationArray = new SpecialEffect[num];
                if (_rawSpecialEffectDataSize > 0)
                {
                    Array.Copy(_rawSpecialEffectData, 0, destinationArray, 0, _rawSpecialEffectDataSize);
                }
                _rawSpecialEffectData = destinationArray;
            }
        }

        /// <summary>
        /// Adds together two stats, when using a + operator. When adding additional stats for
        /// Rawr to track, after adding the stat property, also add a line for it to this method,
        /// to properly combine the stat, as appropriate.
        /// </summary>
        /// <param name="a">The first Stats object to combine.</param>
        /// <param name="b">The second Stats object to combine.</param>
        /// <returns>The combined Stats object.</returns>
        public static Stats operator +(Stats a, Stats b)
        {
            Stats c = new Stats();

            int i = c._rawAdditiveData.Length;
            while (--i >= 0)
            {
                c._rawAdditiveData[i] = a._rawAdditiveData[i] + b._rawAdditiveData[i];
            }
            i = c._rawMultiplicativeData.Length;
            while (--i >= 0)
            {
                c._rawMultiplicativeData[i] = (1 + a._rawMultiplicativeData[i]) * (1 + b._rawMultiplicativeData[i]) - 1;
            }
            i = c._rawInverseMultiplicativeData.Length;
            while (--i >= 0)
            {
                c._rawInverseMultiplicativeData[i] = 1 - (1 - a._rawInverseMultiplicativeData[i]) * (1 - b._rawInverseMultiplicativeData[i]);
            }

            i = c._rawNoStackData.Length;
            while (--i >= 0)
            {
                c._rawNoStackData[i] = Math.Max(a._rawNoStackData[i], b._rawNoStackData[i]);
            }
            int specialEffectCount = a._rawSpecialEffectDataSize + b._rawSpecialEffectDataSize;
            if (specialEffectCount > 0)
            {
                c._rawSpecialEffectData = new SpecialEffect[specialEffectCount];
                c._rawSpecialEffectDataSize = specialEffectCount;
                if (a._rawSpecialEffectDataSize > 0)
                {
                    Array.Copy(a._rawSpecialEffectData, c._rawSpecialEffectData, a._rawSpecialEffectDataSize);
                }
                if (b._rawSpecialEffectDataSize > 0)
                {
                    Array.Copy(b._rawSpecialEffectData, 0, c._rawSpecialEffectData, a._rawSpecialEffectDataSize, b._rawSpecialEffectDataSize);
                }
            }
            return c;
        }

        /// <summary>
        /// Adds together two stats, when using a + operator. When adding additional stats for
        /// Rawr to track, after adding the stat property, also add a line for it to this method,
        /// to properly combine the stat, as appropriate.
        /// </summary>
        /// <param name="a">The first Stats object to combine.</param>
        /// <param name="b">The second Stats object to combine.</param>
        /// <returns>The combined Stats object.</returns>
        public static Stats operator -(Stats a, Stats b)
        {
            Stats c = new Stats();

            int i = c._rawAdditiveData.Length;
            while (--i >= 0)
            {
                c._rawAdditiveData[i] = a._rawAdditiveData[i] - b._rawAdditiveData[i];
            }
            i = c._rawMultiplicativeData.Length;
            while (--i >= 0)
            {
                c._rawMultiplicativeData[i] = (1 + a._rawMultiplicativeData[i]) / (1 + b._rawMultiplicativeData[i]) - 1;
            }
            i = c._rawInverseMultiplicativeData.Length;
            while (--i >= 0)
            {
                c._rawInverseMultiplicativeData[i] = 1 - (1 - a._rawInverseMultiplicativeData[i]) / (1 - b._rawInverseMultiplicativeData[i]);
            }

            //i = c._rawNoStackData.Length;
            //while (--i >= 0)
            //{
            //    c._rawNoStackData[i] = Math.Max(a._rawNoStackData[i], b._rawNoStackData[i]);
            //}
            //int specialEffectCount = a._rawSpecialEffectDataSize + b._rawSpecialEffectDataSize;
            //if (specialEffectCount > 0)
            //{
            //    c._rawSpecialEffectData = new SpecialEffect[specialEffectCount];
            //    c._rawSpecialEffectDataSize = specialEffectCount;
            //    if (a._rawSpecialEffectDataSize > 0)
            //    {
            //        Array.Copy(a._rawSpecialEffectData, c._rawSpecialEffectData, a._rawSpecialEffectDataSize);
            //    }
            //    if (b._rawSpecialEffectDataSize > 0)
            //    {
            //        Array.Copy(b._rawSpecialEffectData, 0, c._rawSpecialEffectData, a._rawSpecialEffectDataSize, b._rawSpecialEffectDataSize);
            //    }
            //}
            return c;
        }

        public void AddSpecialEffect(SpecialEffect specialEffect)
        {
            EnsureSpecialEffectCapacity(_rawSpecialEffectDataSize + 1);
            _rawSpecialEffectData[_rawSpecialEffectDataSize] = specialEffect;
            _rawSpecialEffectDataSize++;
        }


        public void RemoveSpecialEffect(SpecialEffect specialEffect)
        {
            bool found = false;
            for (int i = 0; i < _rawSpecialEffectDataSize; i++)
            {
                if (found)
                {
                    _rawSpecialEffectData[i - 1] = _rawSpecialEffectData[i];
                }
                else if (specialEffect == _rawSpecialEffectData[i])
                {
                    found = true;
                }
            }
            if (found)
            {
                _rawSpecialEffectDataSize--;
                _rawSpecialEffectData[_rawSpecialEffectDataSize] = null;
            }
        }


        public struct SpecialEffectEnumerator : IEnumerator<SpecialEffect>, IDisposable, System.Collections.IEnumerator, IEnumerable<SpecialEffect>
        {
            internal Stats stats;
            private int index;
            private SpecialEffect current;
            internal Predicate<SpecialEffect> match;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                while (index < stats._rawSpecialEffectDataSize)
                {
                    current = stats._rawSpecialEffectData[index];
                    index++;
                    if (match == null || match(current)) return true;
                }
                index = stats._rawSpecialEffectDataSize + 1;
                current = null;
                return false;
            }

            public SpecialEffect Current
            {
                get
                {
                    return current;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    if ((index == 0) || (index == (stats._rawSpecialEffectDataSize + 1)))
                    {
                        throw new InvalidOperationException();
                    }
                    return Current;
                }
            }

            void System.Collections.IEnumerator.Reset()
            {
                index = 0;
                current = null;
            }

            public SpecialEffectEnumerator GetEnumerator()
            {
                return this;
            }

            IEnumerator<SpecialEffect> System.Collections.Generic.IEnumerable<SpecialEffect>.GetEnumerator()
            {
                return this;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this;
            }
        }

        public SpecialEffectEnumerator SpecialEffects()
        {
            return new SpecialEffectEnumerator() { stats = this };
        }

        public SpecialEffectEnumerator SpecialEffects(Predicate<SpecialEffect> match)
        {
            return new SpecialEffectEnumerator() { stats = this, match = match };
        }

        public bool ContainsSpecialEffect(Predicate<SpecialEffect> match)
        {
            for (int i = 0; i < _rawSpecialEffectDataSize; i++)
            {
                SpecialEffect effect = _rawSpecialEffectData[i];
                if (match(effect)) return true;
            }
            return false;
        }

        public bool ContainsSpecialEffect()
        {
            return _rawSpecialEffectDataSize > 0;
        }

        /// <summary>
        /// Multiplies every stat in a stats by a float, when using a * operator.
        /// </summary>
        /// <param name="a">The Stats object to multiply.</param>
        /// <param name="b">The float by which to multiply every stat.</param>
        /// <returns>The new Stats object.</returns>
        public static Stats operator *(Stats a, float b)
        {
            Stats c = new Stats();

            int i = c._rawAdditiveData.Length;
            while (--i >= 0)
            {
                c._rawAdditiveData[i] = a._rawAdditiveData[i] * b;
            }
            i = c._rawMultiplicativeData.Length;
            while (--i >= 0)
            {
                c._rawMultiplicativeData[i] = a._rawMultiplicativeData[i] * b;
            }
            i = c._rawInverseMultiplicativeData.Length;
            while (--i >= 0)
            {
                c._rawInverseMultiplicativeData[i] = a._rawInverseMultiplicativeData[i] * b;
            }

            i = c._rawNoStackData.Length;
            while (--i >= 0)
            {
                c._rawNoStackData[i] = a._rawNoStackData[i] * b;
            }
            // undefined for special effects
            return c;
        }

        public void Accumulate(Stats data)
        {
            if (data._sparseIndices != null)
            {
                int i = 0;
                for (int a = 0; a < data._sparseAdditiveCount; a++, i++)
                {
                    int index = data._sparseIndices[i];
                    _rawAdditiveData[index] += data._rawAdditiveData[index];
                }
                for (int a = 0; a < data._sparseMultiplicativeCount; a++, i++)
                {
                    int index = data._sparseIndices[i];
                    _rawMultiplicativeData[index] = (1 + _rawMultiplicativeData[index]) * (1 + data._rawMultiplicativeData[index]) - 1;
                }
                for (int a = 0; a < data._sparseInverseMultiplicativeCount; a++, i++)
                {
                    int index = data._sparseIndices[i];
                    _rawInverseMultiplicativeData[index] = 1 - (1 - _rawInverseMultiplicativeData[index]) * (1 - data._rawInverseMultiplicativeData[index]);
                }
                for (int a = 0; a < data._sparseNoStackCount; a++, i++)
                {
                    int index = data._sparseIndices[i];
                    float value = data._rawNoStackData[index];
                    if (value > _rawNoStackData[index]) _rawNoStackData[index] = value;
                }
            }
            else
            {
                float[] add = data._rawAdditiveData;
                for (int i = 0; i < _rawAdditiveData.Length; i++)
                {
                    _rawAdditiveData[i] += add[i];
                }
                add = data._rawMultiplicativeData;
                for (int i = 0; i < _rawMultiplicativeData.Length; i++)
                {
                    _rawMultiplicativeData[i] = (1 + _rawMultiplicativeData[i]) * (1 + add[i]) - 1;
                }
                add = data._rawInverseMultiplicativeData;
                for (int i = 0; i < _rawInverseMultiplicativeData.Length; i++)
                {
                    _rawInverseMultiplicativeData[i] = 1 - (1 - _rawInverseMultiplicativeData[i]) * (1 - add[i]);
                }
                add = data._rawNoStackData;
                for (int i = 0; i < _rawNoStackData.Length; i++)
                {
                    if (add[i] > _rawNoStackData[i]) _rawNoStackData[i] = add[i];
                }
            }
            if (data._rawSpecialEffectDataSize > 0)
            {
                EnsureSpecialEffectCapacity(_rawSpecialEffectDataSize + data._rawSpecialEffectDataSize);
                Array.Copy(data._rawSpecialEffectData, 0, _rawSpecialEffectData, _rawSpecialEffectDataSize, data._rawSpecialEffectDataSize);
                _rawSpecialEffectDataSize += data._rawSpecialEffectDataSize;
            }
        }

        public void Accumulate(Stats data, float weight)
        {
            if (data._sparseIndices != null)
            {
                int i = 0;
                for (int a = 0; a < data._sparseAdditiveCount; a++, i++)
                {
                    int index = data._sparseIndices[i];
                    _rawAdditiveData[index] += weight * data._rawAdditiveData[index];
                }
                for (int a = 0; a < data._sparseMultiplicativeCount; a++, i++)
                {
                    int index = data._sparseIndices[i];
                    _rawMultiplicativeData[index] = (1 + _rawMultiplicativeData[index]) * (1 + weight * data._rawMultiplicativeData[index]) - 1;
                }
                for (int a = 0; a < data._sparseInverseMultiplicativeCount; a++, i++)
                {
                    int index = data._sparseIndices[i];
                    _rawInverseMultiplicativeData[index] = 1 - (1 - _rawInverseMultiplicativeData[index]) * (1 - weight * data._rawInverseMultiplicativeData[index]);
                }
                for (int a = 0; a < data._sparseNoStackCount; a++, i++)
                {
                    int index = data._sparseIndices[i];
                    float value = weight * data._rawNoStackData[index];
                    if (value > _rawNoStackData[index]) _rawNoStackData[index] = value;
                }
            }
            else
            {
                float[] add = data._rawAdditiveData;
                for (int i = 0; i < _rawAdditiveData.Length; i++)
                {
                    _rawAdditiveData[i] += weight * add[i];
                }
                add = data._rawMultiplicativeData;
                for (int i = 0; i < _rawMultiplicativeData.Length; i++)
                {
                    _rawMultiplicativeData[i] = (1 + _rawMultiplicativeData[i]) * (1 + weight * add[i]) - 1;
                }
                add = data._rawInverseMultiplicativeData;
                for (int i = 0; i < _rawInverseMultiplicativeData.Length; i++)
                {
                    _rawInverseMultiplicativeData[i] = 1 - (1 - _rawInverseMultiplicativeData[i]) * (1 - weight * add[i]);
                }
                add = data._rawNoStackData;
                for (int i = 0; i < _rawNoStackData.Length; i++)
                {
                    if (weight * add[i] > _rawNoStackData[i]) _rawNoStackData[i] = weight * add[i];
                }
            }
            if (data._rawSpecialEffectDataSize > 0)
            {
                EnsureSpecialEffectCapacity(_rawSpecialEffectDataSize + data._rawSpecialEffectDataSize);
                Array.Copy(data._rawSpecialEffectData, 0, _rawSpecialEffectData, _rawSpecialEffectDataSize, data._rawSpecialEffectDataSize);
                _rawSpecialEffectDataSize += data._rawSpecialEffectDataSize;
            }
        }

#if SILVERLIGHT
        public void AccumulateUnsafe(Stats data)
        {
            Accumulate(data);
        }

        public void AccumulateUnsafe(Stats data, bool generateSparseIfNeeded)
        {
            if (generateSparseIfNeeded && data._sparseIndices == null) data.GenerateSparseData();
            Accumulate(data);
        }
#else
        private float* pRawAdditiveData;
        private float* pRawMultiplicativeData;
        private float* pRawNoStackData;

        public void BeginUnsafe(float* pRawAdditiveData, float* pRawMultiplicativeData, float* pRawNoStackData)
        {
            this.pRawAdditiveData = pRawAdditiveData;
            this.pRawMultiplicativeData = pRawMultiplicativeData;
            this.pRawNoStackData = pRawNoStackData;
        }

        public void EndUnsafe()
        {
            pRawAdditiveData = null;
            pRawMultiplicativeData = null;
            pRawNoStackData = null;
        }

        public void AccumulateUnsafe(Stats data)
        {
            AccumulateUnsafe(data, false);
        }

        public void AccumulateUnsafe(Stats data, bool generateSparseIfNeeded)
        {
            if (generateSparseIfNeeded && data._sparseIndices == null) data.GenerateSparseData();
            if (data._sparseIndices != null)
            {
                int i = 0;
                float* pRawAdditiveData = this.pRawAdditiveData;
                int limit = data._sparseAdditiveCount;
                for (; i < limit; i++)
                {
                    int index = data._sparseIndices[i];
                    pRawAdditiveData[index] += data._rawAdditiveData[index];
                }
                float* pRawMultiplicativeData = this.pRawMultiplicativeData;
                limit += data._sparseMultiplicativeCount;
                for (; i < limit; i++)
                {
                    int index = data._sparseIndices[i];
                    float* pa = pRawMultiplicativeData + index;
                    *pa = (1 + *pa) * (1 + data._rawMultiplicativeData[index]) - 1;
                }
                float[] _rawInverseMultiplicativeData = this._rawInverseMultiplicativeData;
                limit += data._sparseInverseMultiplicativeCount;
                for (; i < limit; i++)
                {
                    int index = data._sparseIndices[i];
                    _rawInverseMultiplicativeData[index] = 1 - (1 - _rawInverseMultiplicativeData[index]) * (1 - data._rawInverseMultiplicativeData[index]);
                }
                float* pRawNoStackData = this.pRawNoStackData;
                limit += data._sparseNoStackCount;
                for (; i < limit; i++)
                {
                    int index = data._sparseIndices[i];
                    float* pa = pRawNoStackData + index;
                    float value = data._rawNoStackData[index];
                    if (value > *pa) *pa = value;
                }
            }
            else
            {
                AccumulateUnsafeDense(data);
            }
            if (data._rawSpecialEffectDataSize > 0)
            {
                EnsureSpecialEffectCapacity(_rawSpecialEffectDataSize + data._rawSpecialEffectDataSize);
                if (data._rawSpecialEffectDataSize == 1)
                {
                    // special case for majority case
                    _rawSpecialEffectData[_rawSpecialEffectDataSize++] = data._rawSpecialEffectData[0];
                }
                else
                {
                    Array.Copy(data._rawSpecialEffectData, 0, _rawSpecialEffectData, _rawSpecialEffectDataSize, data._rawSpecialEffectDataSize);
                    _rawSpecialEffectDataSize += data._rawSpecialEffectDataSize;
                }
            }
        }

        private void AccumulateUnsafeDense(Stats data)
        {
            fixed (float* add = data._rawAdditiveData)
            {
                float* pa = pRawAdditiveData;
                float* paend = pa + AdditiveStatCount;
                float* pa2 = add;
                for (; pa < paend; pa++, pa2++)
                {
                    *pa += *pa2;
                }
            }
            fixed (float* add = data._rawMultiplicativeData)
            {
                float* pa = pRawMultiplicativeData;
                float* paend = pa + MultiplicativeStatCount;
                float* pa2 = add;
                for (; pa < paend; pa++, pa2++)
                {
                    *pa = (1 + *pa) * (1 + *pa2) - 1;
                }
            }
            float[] arr = data._rawInverseMultiplicativeData;
            for (int i = 0; i < _rawInverseMultiplicativeData.Length; i++)
            {
                _rawInverseMultiplicativeData[i] = 1 - (1 - _rawInverseMultiplicativeData[i]) * (1 - arr[i]);
            }
            fixed (float* add = data._rawNoStackData)
            {
                float* pa = pRawNoStackData;
                float* paend = pa + NonStackingStatCount;
                float* pa2 = add;
                for (; pa < paend; pa++, pa2++)
                {
                    if (*pa2 > *pa) *pa = *pa2;
                }
            }
        }
#endif

        public bool Equals(Stats other)
        {
            return this == other;
        }
        public ArrayUtils.CompareResult CompareTo(Stats other)
        {
            if (ReferenceEquals(other, null))
                return 0;
            if (this._rawSpecialEffectDataSize > 0 || other._rawSpecialEffectDataSize > 0)
            {
                return ArrayUtils.CompareResult.Unequal; // not sure if we need to actually go through and compare each
            }
            if (this._sparseIndices != null && other._sparseIndices != null)
            {
                bool haveGreaterThan = false, haveLessThan = false;
                int j = 0;
                int i = 0;
                int b = 0;
                for (int a = 0; a < other._sparseAdditiveCount; a++, i++)
                {
                    int index = other._sparseIndices[i];
                    while (b < _sparseAdditiveCount && _sparseIndices[j] < index)
                    {
                        b++;
                        j++;
                        haveGreaterThan = true;
                        if (haveLessThan)
                        {
                            return ArrayUtils.CompareResult.Unequal;
                        }
                    }
                    if (b < _sparseAdditiveCount && _sparseIndices[j] == index)
                    {
                        int val = _rawAdditiveData[index].CompareTo(other._rawAdditiveData[index]);
                        if (val < 0)
                        {
                            haveLessThan = true;
                            if (haveGreaterThan)
                            {
                                return ArrayUtils.CompareResult.Unequal;
                            }
                        }
                        else if (val > 0)
                        {
                            haveGreaterThan = true;
                            if (haveLessThan)
                            {
                                return ArrayUtils.CompareResult.Unequal;
                            }
                        }
                        b++;
                        j++;
                    }
                    else
                    {
                        haveLessThan = true;
                        if (haveGreaterThan)
                        {
                            return ArrayUtils.CompareResult.Unequal;
                        }
                    }
                }
                while (b < _sparseAdditiveCount)
                {
                    b++;
                    j++;
                    haveGreaterThan = true;
                    if (haveLessThan)
                    {
                        return ArrayUtils.CompareResult.Unequal;
                    }
                }
                b = 0;
                for (int a = 0; a < other._sparseMultiplicativeCount; a++, i++)
                {
                    int index = other._sparseIndices[i];
                    while (b < _sparseMultiplicativeCount && _sparseIndices[j] < index)
                    {
                        b++;
                        j++;
                        haveGreaterThan = true;
                        if (haveLessThan)
                        {
                            return ArrayUtils.CompareResult.Unequal;
                        }
                    }
                    if (b < _sparseMultiplicativeCount && _sparseIndices[j] == index)
                    {
                        int val = _rawMultiplicativeData[index].CompareTo(other._rawMultiplicativeData[index]);
                        if (val < 0)
                        {
                            haveLessThan = true;
                            if (haveGreaterThan)
                            {
                                return ArrayUtils.CompareResult.Unequal;
                            }
                        }
                        else if (val > 0)
                        {
                            haveGreaterThan = true;
                            if (haveLessThan)
                            {
                                return ArrayUtils.CompareResult.Unequal;
                            }
                        }
                        b++;
                        j++;
                    }
                    else
                    {
                        haveLessThan = true;
                        if (haveGreaterThan)
                        {
                            return ArrayUtils.CompareResult.Unequal;
                        }
                    }
                }
                while (b < _sparseMultiplicativeCount)
                {
                    b++;
                    j++;
                    haveGreaterThan = true;
                    if (haveLessThan)
                    {
                        return ArrayUtils.CompareResult.Unequal;
                    }
                }
                b = 0;
                for (int a = 0; a < other._sparseInverseMultiplicativeCount; a++, i++)
                {
                    int index = other._sparseIndices[i];
                    while (b < _sparseInverseMultiplicativeCount && _sparseIndices[j] < index)
                    {
                        b++;
                        j++;
                        haveGreaterThan = true;
                        if (haveLessThan)
                        {
                            return ArrayUtils.CompareResult.Unequal;
                        }
                    }
                    if (b < _sparseInverseMultiplicativeCount && _sparseIndices[j] == index)
                    {
                        int val = _rawInverseMultiplicativeData[index].CompareTo(other._rawInverseMultiplicativeData[index]);
                        if (val < 0)
                        {
                            haveLessThan = true;
                            if (haveGreaterThan)
                            {
                                return ArrayUtils.CompareResult.Unequal;
                            }
                        }
                        else if (val > 0)
                        {
                            haveGreaterThan = true;
                            if (haveLessThan)
                            {
                                return ArrayUtils.CompareResult.Unequal;
                            }
                        }
                        b++;
                        j++;
                    }
                    else
                    {
                        haveLessThan = true;
                        if (haveGreaterThan)
                        {
                            return ArrayUtils.CompareResult.Unequal;
                        }
                    }
                }
                while (b < _sparseInverseMultiplicativeCount)
                {
                    b++;
                    j++;
                    haveGreaterThan = true;
                    if (haveLessThan)
                    {
                        return ArrayUtils.CompareResult.Unequal;
                    }
                }
                b = 0;
                for (int a = 0; a < other._sparseNoStackCount; a++, i++)
                {
                    int index = other._sparseIndices[i];
                    while (b < _sparseNoStackCount && _sparseIndices[j] < index)
                    {
                        b++;
                        j++;
                        haveGreaterThan = true;
                        if (haveLessThan)
                        {
                            return ArrayUtils.CompareResult.Unequal;
                        }
                    }
                    if (b < _sparseNoStackCount && _sparseIndices[j] == index)
                    {
                        int val = _rawNoStackData[index].CompareTo(other._rawNoStackData[index]);
                        if (val < 0)
                        {
                            haveLessThan = true;
                            if (haveGreaterThan)
                            {
                                return ArrayUtils.CompareResult.Unequal;
                            }
                        }
                        else if (val > 0)
                        {
                            haveGreaterThan = true;
                            if (haveLessThan)
                            {
                                return ArrayUtils.CompareResult.Unequal;
                            }
                        }
                        b++;
                        j++;
                    }
                    else
                    {
                        haveLessThan = true;
                        if (haveGreaterThan)
                        {
                            return ArrayUtils.CompareResult.Unequal;
                        }
                    }
                }
                while (b < _sparseNoStackCount)
                {
                    b++;
                    j++;
                    haveGreaterThan = true;
                    if (haveLessThan)
                    {
                        return ArrayUtils.CompareResult.Unequal;
                    }
                }
                if (haveGreaterThan && haveLessThan) return ArrayUtils.CompareResult.Unequal;
                else if (haveGreaterThan) return ArrayUtils.CompareResult.GreaterThan;
                else if (haveLessThan) return ArrayUtils.CompareResult.LessThan;
                else return ArrayUtils.CompareResult.Equal;
            }
            else
            {
                return ArrayUtils.And(ArrayUtils.AllCompare(this._rawAdditiveData, other._rawAdditiveData), ArrayUtils.And(
                    ArrayUtils.AllCompare(this._rawMultiplicativeData, other._rawMultiplicativeData),
                    ArrayUtils.AllCompare(this._rawNoStackData, other._rawNoStackData)));
            }
        }
        //int IComparable.CompareTo(object other)
        //{
        //    return CompareTo(other as Stats);
        //}


        public override int GetHashCode()
        {
            return _rawAdditiveData.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this == (obj as Stats);
        }
        public static bool operator ==(Stats x, Stats y)
        {
            if (ReferenceEquals(x, y) || (ReferenceEquals(x, null) && ReferenceEquals(y, null)))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            if (x._rawSpecialEffectDataSize > 0 || y._rawSpecialEffectDataSize > 0) return false;
            return ArrayUtils.AllEqual(x._rawAdditiveData, y._rawAdditiveData) &&
                ArrayUtils.AllEqual(x._rawMultiplicativeData, y._rawMultiplicativeData) &&
                ArrayUtils.AllEqual(x._rawNoStackData, y._rawNoStackData);
        }
        public static bool operator !=(Stats x, Stats y)
        {
            return !(x == y);
        }
        public static bool operator >(Stats x, Stats y)
        {
			return x >= y && x != y;
        }
        public static bool operator >=(Stats x, Stats y)
        {
            return AllCompare(x, y, ArrayUtils.CompareOption.GreaterThan | ArrayUtils.CompareOption.Equal);
        }
        public static bool operator <(Stats x, Stats y)
        {
			return x <= y && x != y;
        }
        public static bool operator <=(Stats x, Stats y)
        {
            return AllCompare(x, y, ArrayUtils.CompareOption.LessThan | ArrayUtils.CompareOption.Equal);
        }
        private static bool AllCompare(Stats x, Stats y, ArrayUtils.CompareOption comparison)
        {
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                throw new ArgumentNullException();
            if (x._rawSpecialEffectDataSize > 0 || y._rawSpecialEffectDataSize > 0) return false;
            return ArrayUtils.AllCompare(x._rawAdditiveData, y._rawAdditiveData, comparison)
                && ArrayUtils.AllCompare(x._rawMultiplicativeData, y._rawMultiplicativeData, comparison)
                && ArrayUtils.AllCompare(x._rawInverseMultiplicativeData, y._rawInverseMultiplicativeData, comparison)
                && ArrayUtils.AllCompare(x._rawNoStackData, y._rawNoStackData, comparison);
        }


        //as the ocean opens up to swallow you
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (PropertyInfo info in PropertyInfoCache)
            {
                float value = (float)info.GetValue(this, null);
                if (value != 0)
                {
                    if (IsPercentage(info))
                    {
                        value *= 100;
                    }

                    value = (float)Math.Round(value * 100f) / 100f;

                    sb.AppendFormat("{0}{1}, ", value, Extensions.DisplayName(info));
                }
            }
            foreach (SpecialEffect effect in SpecialEffects())
            {
                sb.AppendFormat("{0}, ", effect);
            }

            return sb.ToString().TrimEnd(' ', ',');
        }

        public Stats Clone()
        {
            Stats clone = (Stats)this.MemberwiseClone();
            clone._rawAdditiveData = (float[])clone._rawAdditiveData.Clone();
            clone._rawMultiplicativeData = (float[])clone._rawMultiplicativeData.Clone();
            clone._rawInverseMultiplicativeData = (float[])clone._rawInverseMultiplicativeData.Clone();
            clone._rawNoStackData = (float[])clone._rawNoStackData.Clone();
            if (_rawSpecialEffectData != null) clone._rawSpecialEffectData = (SpecialEffect[])_rawSpecialEffectData.Clone();
            return clone;
        }

		//public void ConvertStatsToWotLKEquivalents()
		//{
		//    HitRating = Math.Max(HitRating, SpellHitRating);
		//    CritRating = Math.Max(CritRating, SpellCritRating);
		//    HasteRating = Math.Max(HasteRating, SpellHasteRating);
		//    SpellPower = Math.Max(SpellPower, Math.Max(SpellDamageRating, (float)Math.Floor(Healing / 1.88f)));
		//    ArmorPenetrationRating = Math.Max(ArmorPenetrationRating, (float)Math.Floor(ArmorPenetration / 7f));
		//    SpellHitRating = SpellCritRating = SpellHasteRating = SpellDamageRating = Healing = ArmorPenetration = 0;
		//}

        #region Multiplicative Handling
        private static PropertyInfo[] _propertyInfoCache = null;
        private static List<PropertyInfo> _percentageProperties = new List<PropertyInfo>();
        private static int AdditiveStatCount = 0;
        private static int MultiplicativeStatCount = 0;
        private static int InverseMultiplicativeStatCount = 0;
        private static int NonStackingStatCount = 0;

        static Stats()
        {
            List<PropertyInfo> items = new List<PropertyInfo>();

            foreach (PropertyInfo info in typeof(Stats).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.ExactBinding))
            {
                if (info.PropertyType.IsAssignableFrom(typeof(float)))
                {
                    items.Add(info);
                }
            }
            _propertyInfoCache = items.ToArray();

            foreach (PropertyInfo info in _propertyInfoCache)
            {
                if (info.GetCustomAttributes(typeof(PercentageAttribute), false).Length > 0)
                {
                    _percentageProperties.Add(info);
                }
            }

            AdditiveStatCount = EnumHelper.GetValues(typeof(AdditiveStat)).Length;
            MultiplicativeStatCount = EnumHelper.GetValues(typeof(MultiplicativeStat)).Length;
            InverseMultiplicativeStatCount = EnumHelper.GetValues(typeof(InverseMultiplicativeStat)).Length;
            NonStackingStatCount = EnumHelper.GetValues(typeof(NonStackingStat)).Length;
        }

        public static PropertyInfo[] PropertyInfoCache
        {
            get
            {
                return _propertyInfoCache;
            }
        }

        public static bool IsPercentage(PropertyInfo info)
        {
            return _percentageProperties.Contains(info);
        }
        #endregion

        private class PropertyComparer : IComparer<PropertyInfo>
        {
            public int Compare(PropertyInfo x, PropertyInfo y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public static String[] StatNames
        {
            get
            {
                String[] names = new string[PropertyInfoCache.Length];
                for (int i = 0; i < PropertyInfoCache.Length; i++)
                {
                    names[i] = Extensions.DisplayName(PropertyInfoCache[i]);
                }
                Array.Sort(names);
                return names;
            }
        }

        public IDictionary<PropertyInfo, float> Values(StatFilter filter)
        {
#if SILVERLIGHT
            Dictionary<PropertyInfo, float> dict = new Dictionary<PropertyInfo, float>();
#else
            SortedList<PropertyInfo, float> dict = new SortedList<PropertyInfo, float>(new PropertyComparer());
#endif
            foreach (PropertyInfo info in PropertyInfoCache)
            {
                if (info.PropertyType.IsAssignableFrom(typeof(float)))
                {
                    float value = (float)info.GetValue(this, null);
                    if (filter(value))
                    {
                        dict[info] = value;
                    }
                }
            }
#if SILVERLIGHT
            dict.OrderBy(kvp => kvp.Key, new PropertyComparer());
#endif
            return dict;
        }
    }

    public delegate bool StatFilter(float value);

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PercentageAttribute : Attribute
    {
    }

    public static class Extensions {
        // requires .net 3.5 public static string LongName(this PropertyInfo info)
        // allows it to be called like
        //   info.LongName()
        // instead of
        //   Extensions.LongName(info)

        public static PropertyInfo UnDisplayName(string displayName) {
            foreach (PropertyInfo info in Stats.PropertyInfoCache) {
                if (DisplayName(info).Trim() == displayName.Trim())
                    return info;
            }
            return null;
        }

        public static string DisplayName(PropertyInfo info) {
            string prettyName = null;

            object[] attributes = info.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            if (attributes.Length == 1 && attributes[0] is DisplayNameAttribute && (attributes[0] as DisplayNameAttribute).DisplayName != null) {
                prettyName = (attributes[0] as DisplayNameAttribute).DisplayName;
            } else {
                prettyName = SpaceCamel(info.Name);
            }
            if (!prettyName.StartsWith("%"))
                prettyName = " " + prettyName;
            return prettyName;
        }
        public static string SpaceCamel(String name) {
            return System.Text.RegularExpressions.Regex.Replace(
                    name,
                    "([A-Z])",
                    " $1"
#if SILVERLIGHT
                    ).Trim();
#else
                    ,System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
#endif
        }
        public static string UnSpaceCamel(String name) {
            return System.Text.RegularExpressions.Regex.Replace(
                    name,
                    "( )",
                    ""
#if SILVERLIGHT
                    ).Trim();
#else
                    ,System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
#endif
        }
    }
}