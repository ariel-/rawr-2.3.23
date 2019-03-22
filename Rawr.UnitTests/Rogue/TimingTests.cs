using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rawr.UnitTests.Rogue
{
    [TestClass]
    public class TimingTests
    {
        [TestMethod]
        public void ListTiming()
        {
            var additiveStats = new List<AdditiveStatClass>();
            var rng = new Random();

            var sw = new Stopwatch();
            sw.Start();

            for ( int index=0; index<1000000; index++)
            {
                additiveStats.Add(new AdditiveStatClass{ Value = (float)rng.NextDouble()});
            }

            Console.WriteLine("End of creating classes: {0}", sw.Elapsed);
            Console.WriteLine("items in list: {0}", additiveStats.Count);
            sw.Reset();
            sw.Start();

            float sum = 0;
            foreach(var stat in additiveStats)
            {
                sum += stat.Value;
            }

            Console.WriteLine("sum: {0}", sum);
            Console.WriteLine("End of summing: {0}", sw.Elapsed);
            sw.Stop();
            sw.Start();
        }

        [TestMethod, Ignore] //slow test.  no need to run for the moment
        public void DictionaryTiming()
        {
            //this method is slow because of the boxing/unboxing of the enum value (int) to (object)
            var rng = new Random();
            var statCollection = new Dictionary<AdditiveStat, AdditiveStatClass>();

            var sw = new Stopwatch();
            sw.Start();

            foreach (var stat in Helpers.List)
            {
                var statValue = new AdditiveStatClass { Value = (float)rng.NextDouble() };
                statCollection.Add(stat, statValue);
            } 
            
            Console.WriteLine("End of creating stats: " + sw.Elapsed);
            sw.Reset();
            sw.Start();

            for ( var i = 0; i < 100000; i++ )
            {
                foreach (var stat in Helpers.List)
                {
                    statCollection[stat].Value += (float)rng.NextDouble();
                }
            }

            var sum = 0f;

            foreach (var stat in Helpers.List)
            {
                sum += statCollection[stat].Value;
            }

            Console.WriteLine("sum: " + sum);
            Console.WriteLine("End of summing stats: " + sw.Elapsed);

            sw.Stop();
        }

        private const int POSSIBLE_ITEMS = 10000;

        [TestMethod]
        public void TimingsForStats()
        {
            var sw = new Stopwatch();
            sw.Start();

            var itemList = new List<SampleItem>(POSSIBLE_ITEMS);
            for ( var index = 0; index < POSSIBLE_ITEMS; index++ )
            {
                itemList.Add(new SampleItem());
            }

            Console.WriteLine("created items: " + sw.Elapsed);
            sw.Reset();
            sw.Start();

            var statSum = new SampleItem();
            foreach(var sampleItem in itemList)
            {
                foreach (var stat in Helpers.List)
                {
                    statSum.StatList[(int)stat].Value += sampleItem.StatList[(int)stat].Value;
                }
            }

            Console.WriteLine("sum time:" + sw.Elapsed);
            sw.Stop();
        }
    }

    internal class SampleItem
    {
        public SampleItem()
        {
            foreach ( var stat in Helpers.List )
            {
                StatList.Add(new AdditiveStatClass((float) _rng.NextDouble()));
            }
        }

        public readonly List<AdditiveStatClass> StatList = new List<AdditiveStatClass>(Helpers.List.Count);
        private static readonly Random _rng = new Random();
    }

    internal class AdditiveStatClass
    {
        public AdditiveStatClass():this(0f){}
        public AdditiveStatClass(float initialValue)
        {
            Value = initialValue;
        }

        public float Value { get; set; }
    }

    internal class Helpers
    {
        public static readonly List<AdditiveStat> List;

        static Helpers()
        {
            var stats = new List<AdditiveStat>();
            foreach (var item in Enum.GetValues(typeof(AdditiveStat)))
            {
                stats.Add((AdditiveStat)item);
            }
            List = stats;            
        }
    }

    // ReSharper disable InconsistentNaming
    enum AdditiveStat
    {
        Agility,
        AllResist,
        ArcaneResistance,
        Armor,
        BonusArmor,
        ArmorPenetrationRating,
        AshtongueTrinketProc,
        AttackPower,
        AverageAgility,
        AverageArmor,
        BaseAgility,
        Block,
        BlockRating,
        BlockValue,
        Bloodlust,
        BloodlustProc,
        BonusCommandingShoutHP,
        BonusLacerateDamageMultiplier,
        BonusMangleBearDamage,
        BonusMangleCatDamage,
        BonusRipDamagePerCPPerTick,
        BonusRipDuration,
        BonusSavageRoarDuration,
        BonusShredDamage,
        BonusStreadyShotCrit,
        ExtraSpiritWhileCasting,
        CatFormStrength,
        ClearcastOnBleedChance,
        PhysicalCrit,
        CritRating,
        CritMeleeRating,
        CritBonusDamage,
        CritChanceReduction,
        Defense,
        DefenseRating,
        Dodge,
        DodgeRating,
        DrumsOfBattle,
        DrumsOfWar,
        Expertise,
        ExpertiseRating,
        ExposeWeakness,
        FireResistance,
        FrostResistance,
        HasteRating,
        HasteRatingOnPhysicalAttack,
        Healing,
        Health,
        PhysicalHit,
        HitRating,
        Hp5,
        IdolCritRating,
        InnervateCooldownReduction,
        InsectSwarmDmg,
        Intellect,
        InterruptProtection,
        LightningCapacitorProc,
        LightweaveEmbroideryProc,
        ThunderCapacitorProc,
        LotPCritRating,
        WarlockFelArmor,
        WarlockDemonArmor,
        Mana,
        ManaRestore5min,
        ManaRestorePerCast,
        ManaRestoreOnCast_5_15,
        ManaRestoreOnCast_10_45,
        ManaRestoreFromBaseManaPerHit,
        ManaRestoreFromMaxManaPerSecond,
        ManaRestoreOnCrit_25_45,
        MangleCostReduction,
        RakeCostReduction,
        ShredCostReduction,
        Miss,
        MoonfireDmg,
        MoonfireExtension,
        BladeWardProc,
        Mp5,
        Mp5OnCastFor20SecOnUse2Min,
        NatureResistance,
        Parry,
        ParryRating,
        PendulumOfTelluricCurrentsProc,
        ExtractOfNecromanticPowerProc,
        DarkmoonCardDeathProc,
        PVPTrinket,
        RangedAttackPower,
        RangedHitRating,
        RangedCritRating,
        RangedHasteRating,
        Resilience,
        ScopeDamage,
        ShadowResistance,
        ShatteredSunAcumenProc,
        ShatteredSunMightProc,
        ShatteredSunRestoProc,
        SpellArcaneDamageRating,
        SpellCombatManaRegeneration,
        SpellCrit,
        SpellCritRating,
        SpellPowerFor10SecOnCrit_20_45,
        SpellPowerFor10SecOnHit_10_45,
        SpellPowerFor10SecOnHeal_10_45,
        SpellPowerFor10SecOnCast_10_45,
        SpellPowerFor10SecOnCast_15_45,
        SpellDamageFor10SecOnHit_5,
        SpellPowerFor10SecOnResist,
        SpellPowerFor15SecOnCrit_20_45,
        SpellPowerFor15SecOnCast_50_45,
        SpellPowerFor15SecOnUse90Sec,
        SpellPowerFor15SecOnUse2Min,
        SpellPowerFor20SecOnUse2Min,
        SpellPowerFor20SecOnUse5Min,
        SpellDamageFromIntellectPercentage,
        SpellDamageFromSpiritPercentage,
        SpellPowerFromAttackPowerPercentage,
        SpellDamageRating,
        SpellFireDamageRating,
        SpellFrostDamageRating,
        HasteRatingFor20SecOnUse2Min,
        HasteRatingFor20SecOnUse5Min,
        SpellHasteFor5SecOnCrit_50,
        SpellHasteFor6SecOnCast_15_45,
        SpellHasteFor10SecOnCast_10_45,
        SpellHasteFor10SecOnHeal_10_45,
        SpellHasteFor6SecOnHit_10_45,
        SpellHasteRating,
        SpellHit,
        SpellHitRating,
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
        TimbalsProc,
        BonusHealingReceived,
        UnseenMoonDamageBonus,
        WeaponDamage,
        WindfuryAPBonus,
        WrathDmg,
        DruidAshtongueTrinket,
        BonusPetCritChance,
        RegrowthExtraTicks,
        LifebloomFinalHealBonus,
        BonusHealingTouchMultiplier,
        TreeOfLifeAura,
        ReduceRejuvenationCost,
        ReduceRegrowthCost,
        ReduceHealingTouchCost,
        RejuvenationHealBonus,
        RejuvenationSpellpower,
        LifebloomTickHealBonus,
        HealingTouchFinalHealBonus,
        ManaregenFor8SecOnUse5Min,
        SpiritFor20SecOnUse2Min,
        ManaregenOver12SecOnUse3Min,
        ManaregenOver12SecOnUse5Min,
        ManacostReduceWithin15OnHealingCast,
        BangleProc,
        FullManaRegenFor15SecOnSpellcast,
        SpellPower,
        BonusRageOnCrit,
        BonusCPOnCrit,
        BonusEnergyOnTigersFury,
        FinisherEnergyOnAvoid,
        MangleCooldownReduction,
        BonusFerociousBiteCrit,
        BonusObliterateDamage,
        BonusScourgeStrikeDamage,
        BonusInsectSwarmDamage,
        BonusNukeCritChance,
        BonusHoTOnDirectHeals,
        TigersFuryCooldownReduction,
        #region Added by Rawr.HealPriest
        PriestInnerFire,
        RenewDurationIncrease,
        ManaGainOnGreaterHealOverheal,
        PrayerOfMendingExtraJumps,
        GreaterHealCostReduction,
        WeakenedSoulDurationDecrease,
        PrayerOfHealingExtraCrit,
        PWSBonusSpellPowerProc,
        #endregion
        #region Added by Rawr.ShadowPriest
        SWPDurationIncrease,
        MindBlastCostReduction,
        ShadowWordDeathCritIncrease,
        DevouringPlagueBonusDamage,
        MindBlastHasteProc,
        #endregion
        #region Added by Rawr.Mage
        AldorRegaliaInterruptProtection,
        ArcaneBlastBonus,
        BonusManaGem,
        EvocationExtension,
        MageAllResist,
        MageIceArmor,
        MageMageArmor,
        MageMoltenArmor,
        Mage4T8,
        #endregion
        #region Added by Rawr.Tree
        LifebloomCostReduction,
        NourishBonusPerHoT,
        NourishSpellpower,
        RejuvenationInstantTick,
        SwiftmendBonus,
        #endregion
        #region Added by Rawr.Enhance
        TotemLLAttackPower,
        TotemShockSpellPower,
        TotemShockAttackPower,
        TotemSSHaste,
        TotemSSDamage,
        TotemWFAttackPower,
        BonusFlurryHaste,
        BonusLSDamage,
        BonusLLSSDamage,
        BonusMWFreq,
        #endregion
        #region Added by Rawr.Elemental
        BonusCritChance,
        BonusThunderCritChance,
        BonusShamanHit,
        ManaRegenIntPer5,
        ShamanCastTimeReduction,
        LightningOverloadProc,
        BonusLavaBurstCritDamage,
        ChainLightningCooldownReduction,
        BonusFlameShockDoTDamage,
        BonusFlametongueDamage,
        ShockManaCostReduction,
        LightningBoltDamageModifier,
        LightningBoltCostReduction,
        LightningSpellPower,
        LightningBoltHasteProc_15_45,
        LavaBurstBonus,
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
        #endregion
        #region Rawr.Healadin
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
        ManaRestore,
        SpellsManaReduction,        // Seems this applies before talents, so different from ManaRestore with 100% proc on SpellCast
        #endregion
        #region Rawr.Retribution
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
        #endregion
        #region Added by Rawr.ProtPaladin
        DivineProtectionDurationBonus,
        JudgementBlockValue,
        ShieldOfRighteousnessBlockValue,
        #endregion
        #region Warlock set bonuses
        LifeTapBonusSpirit,
        #endregion
        #region Added by Rawr.Moonkin
        StarfireProc,
        EclipseBonus,
        #endregion
        #region Warrior set bonuses
        DevastateCritIncrease,
        DreadnaughtBonusRageProc,
        #endregion
        #region Rogue set bonuses
        BonusSnDDuration,
        BonusSnDHaste,
        BonusCPGDamage,
        BonusEvisEnvenomDamage,
        BonusFreeFinisher,
        CPOnFinisher,
        RogueComboMoveEnergyReduction,
        RogueRuptureDamageBonus,
        #endregion
        #region DK Sigil Bonuses
        BonusBloodStrikeDamage,
        BonusDeathCoilDamage,
        BonusDeathStrikeDamage,
        BonusFrostStrikeDamage,
        BonusHeartStrikeDamage,
        BonusIcyTouchDamage,
        #endregion
        #region DK set bonuses
        BonusAntiMagicShellDamageReduction,
        BonusDeathCoilCrit,
        BonusDeathStrikeCrit,
        BonusFrostStrikeCrit,
        BonusIceboundFortitudeDuration,
        BonusObliterateCrit,
        BonusPerDiseaseBloodStrikeDamage,
        BonusPerDiseaseHeartStrikeDamage,
        BonusPerDiseaseObliterateDamage,
        BonusPerDiseaseScourgeStrikeDamage,
        BonusPlagueStrikeCrit,
        BonusRPFromDeathStrike,
        BonusRPFromObliterate,
        BonusRPFromScourgeStrike,
        BonusRuneStrikeMultiplier,
        BonusScourgeStrikeCrit,
        #endregion
        #region Runeforges
        BonusFrostWeaponDamage,
        CinderglacierProc,
        #endregion
        ArcaneDamage,
        FireDamage,
        ShadowDamage,

        NUM_AdditiveStat // This should always be the last entry.
    }
    // ReSharper restore InconsistentNaming

}