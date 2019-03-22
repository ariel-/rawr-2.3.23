using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Tree
{
    public class SpellMix
    {
        private Character cacheCharacter;
        private Stats cacheStats;

        public SpellMix(SpellMix copy)
        {
            cacheCharacter = copy.cacheCharacter;
            cacheStats = copy.cacheStats;

            #region Setup Spells
            regrowth = new Regrowth(cacheCharacter, cacheStats, false, false);
            regrowthFresh = new Regrowth(cacheCharacter, cacheStats, false, false);
            regrowthAgain = new Regrowth(cacheCharacter, cacheStats, true, true);
            regrowthClipped = new Regrowth(cacheCharacter, cacheStats, false, true);
            lifebloom = new Lifebloom(cacheCharacter, cacheStats);
            lifebloomSlowStack = new Lifebloom(cacheCharacter, cacheStats, 3, false);
            lifebloomFastStack = new Lifebloom(cacheCharacter, cacheStats, 3, true);
            lifebloomSlow2Stack = new Lifebloom(cacheCharacter, cacheStats, 2, false);
            lifebloomFast2Stack = new Lifebloom(cacheCharacter, cacheStats, 2, true);
            lifebloomRollingStack = new LifebloomStack(cacheCharacter, cacheStats);

            rejuvenate = new Rejuvenation(cacheCharacter, cacheStats);
            nourish = new Nourish[5];
            nourish[0] = new Nourish(cacheCharacter, cacheStats, 0);
            nourish[1] = new Nourish(cacheCharacter, cacheStats, 1);
            nourish[2] = new Nourish(cacheCharacter, cacheStats, 2);
            nourish[3] = new Nourish(cacheCharacter, cacheStats, 3);
            nourish[4] = new Nourish(cacheCharacter, cacheStats, 4);
            healingTouch = new HealingTouch(cacheCharacter, cacheStats);
            wildGrowth = new WildGrowth(cacheCharacter, cacheStats);
            #endregion

            RejuvCF = copy.RejuvCF;
            ManagedRejuvCF = copy.ManagedRejuvCF;
            RegrowthChainCast = copy.RegrowthChainCast;
            ManagedRegrowthChainCast = copy.ManagedRegrowthChainCast;
            RegrowthCF = copy.RegrowthCF;
            ManagedRegrowthCF = copy.ManagedRegrowthCF;
            LifebloomCF = copy.LifebloomCF;
            NourishCF = copy.NourishCF;
            nourishRawHPCT = copy.nourishRawHPCT;

            LifebloomStackType = copy.LifebloomStackType;
            lifebloomStacks = copy.lifebloomStacks;
            lifebloomStackCF = copy.lifebloomStackCF;
            WildGrowthCF = copy.WildGrowthCF;

            ValAnyrShield = copy.ValAnyrShield;

            CreateSwiftmend();
            SwiftmendCPM = copy.SwiftmendCPM;
        }

        public SpellMix(Character character, Stats stats)
        {
            cacheCharacter = character;
            cacheStats = stats;

            #region Setup Spells
            regrowth = new Regrowth(character, stats, false, false);
            regrowthFresh = new Regrowth(character, stats, false, false);
            regrowthAgain = new Regrowth(character, stats, true, true);
            regrowthClipped = new Regrowth(character, stats, false, true);
            lifebloom = new Lifebloom(character, stats);
            lifebloomSlowStack = new Lifebloom(character, stats, 3, false);
            lifebloomFastStack = new Lifebloom(character, stats, 3, true);
            lifebloomSlow2Stack = new Lifebloom(character, stats, 2, false);
            lifebloomFast2Stack = new Lifebloom(character, stats, 2, true);
            lifebloomRollingStack = new LifebloomStack(character, stats);

            rejuvenate = new Rejuvenation(character, stats);
            nourish = new Nourish[5];
            nourish[0] = new Nourish(character, stats, 0);
            nourish[1] = new Nourish(character, stats, 1);
            nourish[2] = new Nourish(character, stats, 2);
            nourish[3] = new Nourish(character, stats, 3);
            nourish[4] = new Nourish(character, stats, 4);
            healingTouch = new HealingTouch(character, stats);
            wildGrowth = new WildGrowth(character, stats);
            #endregion

            RejuvCF = 0f;
            ManagedRejuvCF = 0f;
            RegrowthChainCast = false;
            RegrowthCF = 0f;
            ManagedRegrowthChainCast = false;
            ManagedRegrowthCF = 0f;
            LifebloomCF = 0f;
            NourishCF = 0f;
            nourishRawHPCT = 0f;

            LifebloomStackType = LifeBloomType.Slow;
            lifebloomStacks = 0f;
            lifebloomStackCF = 0f;
            WildGrowthCF = 0f;

            if (stats.ShieldFromHealed > 0) ValAnyrShield = stats.ShieldFromHealed;

            CreateSwiftmend();
            SwiftmendCPM = 0f;
            RevitalizeChance = stats.RevitalizeChance;
        }

        public void ApplyCombatStats(Stats stats)
        {
            cacheStats = stats;
            rejuvenate.applyStats(stats);
            regrowth.applyStats(stats);
            regrowthFresh.applyStats(stats);
            regrowthAgain.applyStats(stats);
            lifebloom.applyStats(stats);
            lifebloomSlowStack.applyStats(stats);
            lifebloomFastStack.applyStats(stats);
            lifebloomRollingStack.applyStats(stats);
            nourish[0].applyStats(stats);
            nourish[1].applyStats(stats);
            nourish[2].applyStats(stats);
            nourish[3].applyStats(stats);
            nourish[4].applyStats(stats);
            healingTouch.applyStats(stats);
            wildGrowth.applyStats(stats);
            CreateSwiftmend();
        }

        #region Spells
        public Rejuvenation rejuvenate;
        public Regrowth regrowthFresh;
        public Regrowth regrowthClipped;
        public Regrowth regrowthAgain;
        public Regrowth regrowth;
        public Lifebloom lifebloom;
        public Lifebloom lifebloomSlowStack;
        public Lifebloom lifebloomFastStack;
        public Lifebloom lifebloomSlow2Stack;
        public Lifebloom lifebloomFast2Stack;
        public LifebloomStack lifebloomRollingStack;

        public Nourish[] nourish;
        public HealingTouch healingTouch;
        public WildGrowth wildGrowth;
        public Swiftmend swiftmend = null;
        #endregion

        public float HPS { get { return HotsHPS + WildGrowthHPS + SwiftmendHPS + NourishHPS + ValAnyrHPS; } }
        public float HPM { get { return HPS / MPS; } }
        public float MPS { get { return HotsMPS + WildGrowthMPS + SwiftmendMPS + NourishMPS; } }
        public float CastsPerMinute { get { return HotsCastsPerMinute + WildGrowthCPM + SwiftmendCPM + NourishCPM; } }
        public float HealsPerMinute { get { return HotsHealsPerMinute + WildGrowthHealsPerMinute + SwiftmendHealsPerMinute + NourishHealsPerMinute; } }
        public float CritsPerMinute { get { return HotsCritsPerMinute + SwiftmendCritsPerMinute + NourishCritsPerMinute; } }

        #region Hots
        public float HotsCF { get { return RejuvCF + RegrowthCF + ManagedRejuvCF + ManagedRegrowthCF + LifebloomCF + LifebloomStackCF; } }
        public float HotsHPS { get { return RejuvHPS + RegrowthHPS + ManagedRejuvHPS + ManagedRegrowthHPS + LifebloomHPS + LifebloomStackHPS; } }
        public float HotsMPS { get { return RejuvMPS + RegrowthMPS + ManagedRejuvMPS + ManagedRegrowthMPS + LifebloomMPS + LifebloomStackMPS; } }
        public float HotsCastsPerMinute { get { return RejuvCPM + RegrowthCPM + ManagedRejuvCPM + ManagedRegrowthCPM + LifebloomCPM + LifebloomStackCPM; } }
        public float HotsHealsPerMinute { get { return RejuvHealsPerMinute + RegrowthHealsPerMinute + ManagedRegrowthHealsPerMinute + LifebloomHealsPerMinute + LifebloomStackHealsPerMinute; } }
        public float HotsCritsPerMinute { get { return RegrowthCritsPerMinute + ManagedRegrowthCritsPerMinute + LifebloomCritsPerMinute + LifebloomStackCritsPerMinute; } }
        #endregion

        #region Rejuvenation
        public float RejuvCF = 0f;
        public float RejuvCPS { get { return RejuvCF / rejuvenate.CastTime; } }
        public float RejuvCPM { get { return 60f * RejuvCF / rejuvenate.CastTime; } }
        public float RejuvAvg { get { return RejuvCPS * rejuvenate.Duration; } }
        public float RejuvHPS { get { return RejuvCF * rejuvenate.HPCT; } }
        public float RejuvMPS { get { return RejuvCPS * rejuvenate.ManaCost; } }
        public float RejuvHealsPerSecond { get { return RejuvCPS * rejuvenate.PeriodicTicks; } }
        public float RejuvCritsPerSecond { get { return RejuvCPS * rejuvenate.PeriodicTicks * rejuvenate.CritHoTPercent / 100f; } }

        public float ManagedRejuvCF = 0f;
        public float ManagedRejuvCPS { get { return ManagedRejuvCF / rejuvenate.CastTime; } }
        public float ManagedRejuvCPM { get { return 60f * ManagedRejuvCF / rejuvenate.CastTime; } }
        public float ManagedRejuvAvg { 
            get { return ManagedRejuvCPS * rejuvenate.Duration; }
            set { ManagedRejuvCF = value * rejuvenate.CastTime / rejuvenate.Duration; }
        }
        public float ManagedRejuvHPS { get { return ManagedRejuvCF * rejuvenate.HPCT; } }
        public float ManagedRejuvMPS { get { return ManagedRejuvCPS * rejuvenate.ManaCost; } }
        public float ManagedRejuvHealsPerSecond { get { return ManagedRejuvCPS * rejuvenate.PeriodicTicks; } }
        public float ManagedRejuvCritsPerSecond { get { return ManagedRejuvCPS * rejuvenate.PeriodicTicks * rejuvenate.CritHoTPercent / 100f; } }

        public float RejuvHealsPerMinute { get { return (ManagedRejuvCPM + RejuvCPM) * rejuvenate.PeriodicTicks; } }
        #endregion

        #region Regrowth
        public float RegrowthCF = 0f;
        private bool regrowthChainCast = false;
        public bool RegrowthChainCast { get { return regrowthChainCast; } set { regrowthChainCast = value; regrowthSpell = value ? regrowthAgain : regrowth; } }
        private Spell regrowthSpell = null;
        public Spell RegrowthSpell { get { return regrowthSpell; } }
        public float RegrowthCPS { get { return RegrowthCF / regrowthSpell.CastTime; } }
        public float RegrowthCPM { get { return 60f * RegrowthCF / regrowthSpell.CastTime; } }
        public float RegrowthAvg { get { return RegrowthCPS * regrowthSpell.Duration; } }
        public float RegrowthHPS { get { return RegrowthCF * (regrowthChainCast ? regrowthSpell.HPCT_DH : regrowthSpell.HPCT); } }
        public float RegrowthMPS { get { return RegrowthCPS * regrowthSpell.ManaCost; } }
        public float RegrowthHotTicksPerMinute { get { return RegrowthCPM * (regrowthChainCast ? 0 : regrowthSpell.PeriodicTicks); } }
        public float RegrowthHealsPerMinute { get { return RegrowthCPM + RegrowthHotTicksPerMinute; } }
        public float RegrowthCritsPerMinute { get { return RegrowthCPM * regrowthSpell.CritPercent / 100f; } }

        public float ManagedRegrowthCF = 0f;
        private bool managedRegrowthChainCast = false;
        public bool ManagedRegrowthChainCast { get { return managedRegrowthChainCast; } set { managedRegrowthChainCast = value; managedRegrowthSpell = value ? regrowthAgain : regrowthClipped; } }
        private Spell managedRegrowthSpell = null;
        public Spell ManagedRegrowthSpell { get { return managedRegrowthSpell; } }
        public float ManagedRegrowthCPS { get { return ManagedRegrowthCF / managedRegrowthSpell.CastTime; } }
        public float ManagedRegrowthCPM { get { return 60f * ManagedRegrowthCF / managedRegrowthSpell.CastTime; } }
        public float ManagedRegrowthAvg { 
            get { return ManagedRegrowthCPS * managedRegrowthSpell.Duration; } 
            set { ManagedRegrowthCF = value * managedRegrowthSpell.CastTime / managedRegrowthSpell.Duration; }
        }
        public float ManagedRegrowthHPS { get { return ManagedRegrowthCF * (managedRegrowthChainCast ? managedRegrowthSpell.HPCT_DH : managedRegrowthSpell.HPCT); } }
        public float ManagedRegrowthMPS { get { return ManagedRegrowthCPS * managedRegrowthSpell.ManaCost; } }
        public float ManagedRegrowthHotTicksPerMinute { get { return ManagedRegrowthCPM * (managedRegrowthChainCast ? 0 : managedRegrowthSpell.PeriodicTicks); } }
        public float ManagedRegrowthHealsPerMinute { get { return ManagedRegrowthCPM + ManagedRegrowthHotTicksPerMinute; } }
        public float ManagedRegrowthCritsPerMinute { get { return ManagedRegrowthCPM * managedRegrowthSpell.CritPercent / 100f; } }
        #endregion

        #region Lifebloom
        public float LifebloomCF = 0f;
        public float LifebloomCPS { get { return LifebloomCF / lifebloom.CastTime; } }
        public float LifebloomCPM { get { return 60f * LifebloomCF / lifebloom.CastTime; } }
        public float LifebloomAvg { get { return LifebloomCPS * lifebloom.Duration; } }
        public float LifebloomHPS { get { return LifebloomCF * lifebloom.HPCT; } }
        public float LifebloomMPS { get { return LifebloomCPS * lifebloom.ManaCost; } }
        public float LifebloomHotTicksPerMinute { get { return LifebloomCPM * lifebloom.PeriodicTicks; } }
        public float LifebloomHealsPerMinute { get { return LifebloomCPM + LifebloomHotTicksPerMinute; } }
        public float LifebloomCritsPerMinute { get { return LifebloomCPM * lifebloom.CritPercent / 100f; } }
        #endregion

        #region Nourish
        public float NourishCF = 0f;
        public float NourishHPS { get { return NourishCF * nourishRawHPCT; } }
        public float NourishMPS { get { return NourishCPS * nourish[0].ManaCost; } }
        public float NourishCPS { get { return NourishCF / nourish[0].CastTime; } }
        public float NourishCPM { get { return 60f * NourishCF / nourish[0].CastTime; } }
        public float NourishHealsPerMinute { get { return NourishCPM; } }
        public float NourishCritsPerMinute { get { return NourishHealsPerMinute * nourish[0].CritPercent / 100f; } }

        private float nourishRawHPCT = 0f;
        public void CalculateNourishHPS(float hots0, float hots1, float hots2, float hots3, float hots4)
        {
            nourishRawHPCT = (hots0 * nourish[0].HPCT_DH
                + hots1 * nourish[1].HPCT_DH
                + hots2 * nourish[2].HPCT_DH
                + hots3 * nourish[3].HPCT_DH
                + hots4 * nourish[4].HPCT_DH);
        }
        #endregion

        #region Lifebloom stacks
        private LifeBloomType lifebloomStackType;
        public LifeBloomType LifebloomStackType
        {
            get { return lifebloomStackType; }
            set
            {
                lifebloomStackType = value;
                switch (value)
                {
                    case LifeBloomType.Slow:
                        lifebloomStackSpell = lifebloomSlowStack;
                        lifebloomStackDuration = lifebloomStackSpell.Duration + 1f; // need a second before reapplying
                        break;
                    case LifeBloomType.Fast:
                        lifebloomStackSpell = lifebloomFastStack;
                        lifebloomStackDuration = lifebloomStackSpell.Duration + 1f; // need a second before reapplying
                        break;
                    case LifeBloomType.Slow2:
                        lifebloomStackSpell = lifebloomSlow2Stack;
                        lifebloomStackDuration = lifebloomStackSpell.Duration + 1f; // need a second before reapplying
                        break;
                    case LifeBloomType.Fast2:
                        lifebloomStackSpell = lifebloomFast2Stack;
                        lifebloomStackDuration = lifebloomStackSpell.Duration + 1f; // need a second before reapplying
                        break;
                    default:
                        lifebloomStackSpell = lifebloomRollingStack;
                        lifebloomStackDuration = lifebloomStackSpell.Duration;
                        break;
                }
                lifebloomStackCF = -1f;
            }
        }
        private Spell lifebloomStackSpell = null;
        public Spell LifebloomStackSpell { get { return lifebloomStackSpell; } } 
        private float lifebloomStackDuration;

        private float lifebloomStacks = 0;
        public float LifebloomStacks { get { return lifebloomStacks; } set { lifebloomStacks = value; lifebloomStackCF = -1f; } }

        private float lifebloomStackCF = -1f;
        public float LifebloomStackCF
        {
            get
            {
                if (lifebloomStackCF != -1f) return lifebloomStackCF;
                return lifebloomStackSpell.CastTime * LifebloomStacks / lifebloomStackDuration;
            }
            set { lifebloomStackCF = value; }
        }
        public float LifebloomStackCPS
        {
            get
            {
                return lifebloomStackSpell.NumberOfCasts * LifebloomStackCF / lifebloomStackSpell.CastTime;
            }
        }
        public float LifebloomStackCPM
        {
            get
            {
                return 60f * LifebloomStackCPS;
            }
        }
        public float LifebloomStackAvg
        {
            get
            {
                return LifebloomStackCPS / lifebloomStackSpell.NumberOfCasts * lifebloomStackSpell.Duration;
            }
        }
        public float LifebloomStackBPS
        {
            get
            {
                return lifebloomStackType == LifeBloomType.Rolling ? 0 : LifebloomStackCPS / lifebloomStackSpell.NumberOfCasts;
            }
        }
        public float LifebloomStackHPS
        {
            get
            {
                return LifebloomStackAvg * lifebloomStackSpell.HPS_HOT + LifebloomStackBPS * lifebloomStackSpell.AverageHealingwithCrit;
            }
        }
        public float LifebloomStackMPS
        {
            get
            {
                return LifebloomStackCPS * lifebloomStackSpell.ManaCost;
            }
        }
        public float LifebloomStackHotTicksPerMinute
        {
            get
            {
                return LifebloomStackAvg * 60f / lifebloomStackSpell.PeriodicTickTime;
            }
        }
        public float LifebloomStackHealsPerMinute
        {
            get
            {
                return 60f * LifebloomStackBPS + LifebloomStackHotTicksPerMinute;
            }
        }
        public float LifebloomStackCritsPerMinute
        {
            get
            {
                return lifebloomStackType == LifeBloomType.Rolling ? 0 :
                    60f * LifebloomStackBPS * lifebloomStackSpell.CritPercent / 100f;
            }
        }
        public float LifebloomStackHPS_DH { get { return LifebloomStackBPS * lifebloomStackSpell.AverageHealingwithCrit; } }
        public float LifebloomStackHPS_HOT { get { return LifebloomStackAvg * lifebloomStackSpell.HPS_HOT; } }
        #endregion

        #region Wild Growth
        private float wildgrowthCF = 0f;
        public float WildGrowthCF { get { return wildgrowthCF; } set { if (cacheCharacter.DruidTalents.WildGrowth > 0) wildgrowthCF = value; else wildgrowthCF = 0; } }
        public float WildGrowthCPS { get { return WildGrowthCF / wildGrowth.CastTime; } }
        public float WildGrowthCPM { get { return 60f * WildGrowthCPS; } set { WildGrowthCF = value * (float)wildGrowth.CastTime / 60f; } }
        public float WildGrowthAvg { get { return WildGrowthCPS * wildGrowth.Duration; } }
        public float WildGrowthHPS { get { return WildGrowthAvg * wildGrowth.maxTargets * wildGrowth.PeriodicTick; } }
        public float WildGrowthMPS { get { return WildGrowthCPS * wildGrowth.ManaCost; } }
        public float WildGrowthHealsPerMinute { get { return WildGrowthCPM * wildGrowth.maxTargets * wildGrowth.PeriodicTicks; } }
        #endregion

        #region Swiftmend
        public void CreateSwiftmend()
        {
            swiftmend = new Swiftmend(cacheCharacter, cacheStats, RejuvCF>0?rejuvenate:null, RegrowthCF>0?regrowth:null);
        }
        public float SwiftmendCF
        {
            get
            {
                if (swiftmend == null) return 0;
                float cf = SwiftmendCPS * swiftmend.CastTime;
                //cf += swiftmend.rejuvUseChance * swiftmend.rejuvTicksLost / rejuvenate.PeriodicTicks * rejuvenate.CastTime * SwiftmendCPS;
                //cf += swiftmend.regrowthUseChance * swiftmend.regrowthTicksLost / regrowth.PeriodicTicks * regrowth.CastTime * SwiftmendCPS;
                return cf;
            }
            /**/// COMMENT BLOCK
            set
            {
                SwiftmendCPM = 60f * value / swiftmend.CastTime;
            }
            /**/
        }
        public float SwiftmendCPS { get { return SwiftmendCPM / 60f; } }
        private float swiftmendCPM;
        public float SwiftmendCPM { get { return swiftmendCPM; } set { if (cacheCharacter.DruidTalents.Swiftmend > 0) swiftmendCPM = value; else swiftmendCPM = 0; } }
        public float SwiftmendAvg { get { return swiftmend == null ? 0 : SwiftmendCPS * swiftmend.Duration; } }
        public float SwiftmendHPS
        {
            get
            {
                if (swiftmend == null) return 0;
                float healing = swiftmend.TotalAverageHealing;
                healing -= swiftmend.regrowthUseChance * swiftmend.regrowthTicksLost * regrowth.PeriodicTick;
                healing -= swiftmend.rejuvUseChance * swiftmend.rejuvTicksLost * rejuvenate.PeriodicTick;
                return healing * SwiftmendCPS;
                //float hps = swiftmend.TotalAverageHealing * SwiftmendCPS;
                //hps += swiftmend.regrowthUseChance * swiftmend.regrowthTicksLost / regrowth.PeriodicTicks * regrowth.AverageHealingwithCrit * SwiftmendCPS;
                //return hps;
            }
        }

        public float SwiftmendMPS
        {
            get
            {
                if (swiftmend == null) return 0;
                float mps = SwiftmendCPS * swiftmend.ManaCost;
                mps += swiftmend.rejuvUseChance * swiftmend.rejuvTicksLost / rejuvenate.PeriodicTicks * rejuvenate.ManaCost * SwiftmendCPS;
                mps += swiftmend.regrowthUseChance * swiftmend.regrowthTicksLost / regrowth.PeriodicTicks * regrowth.ManaCost * SwiftmendCPS;
                return mps;
            }
        }
        public float SwiftmendHealsPerMinute { get { return SwiftmendCPM; } }
        public float SwiftmendCritsPerMinute { get { return swiftmend == null ? 0 : SwiftmendCPM * swiftmend.CritPercent / 100.0f; } }
        #endregion

        public float TotalCF { get { return (float)Math.Round(HotsCF + WildGrowthCF + SwiftmendCF + NourishCF, 4); } }
        public float IdleCF { get { return 1f - TotalCF; } }

        public float ValAnyrShield = 0;
        public float ValAnyrHPS { get { return ValAnyrShield * (HotsHPS + WildGrowthHPS + SwiftmendHPS + NourishHPS); } }

        public float RejuvenationHealsPerMinute { get { return RejuvHealsPerMinute; } }

        public float RevitalizeChance = 0f;
        public float RevitalizeProcsPerMinute { get { return RevitalizeChance * (RejuvCPM * rejuvenate.PeriodicTicks + WildGrowthCPM * wildGrowth.PeriodicTicks * wildGrowth.maxTargets); } }
    }

    public enum HealingSpell : int
    {
        Rejuv,
        Regrowth,
        Lifebloom,
        ManagedRejuv,
        ManagedRegrowth,
        ManagedLifebloom,
        WildGrowth,
        Swiftmend,
        Nourish,
        HealingTouch,
        NUM_HealingSpell // should always be last
    };

    public class CombatFactor
    {
        public float HealingPerSecond;
        public float ManaPerSecond;
        public float HotTicksPerSecond;
        public float DirectHealsPerSecond;
        public float HotCritsPerSecond;
        public float DirectCritsPerSecond;
        public float CritsPerSecond;
        public float HealsPerSecond;
        public float CastsPerSecond;
        public float CastFraction;

        public void Compute()
        {
            CritsPerSecond = DirectCritsPerSecond + HotCritsPerSecond;
            HealsPerSecond = DirectHealsPerSecond + HotTicksPerSecond;
        }

        public CombatFactor()
        {
            HealingPerSecond = 0;
            ManaPerSecond = 0;
            HotTicksPerSecond = 0;
            DirectHealsPerSecond = 0;
            HotCritsPerSecond = 0;
            DirectCritsPerSecond = 0;
            CritsPerSecond = 0;
            HealsPerSecond = 0;
            CastsPerSecond = 0;
            CastFraction = 0;
        }

        public CombatFactor Clone()
        {
            CombatFactor result = new CombatFactor();
            result.HealingPerSecond = HealingPerSecond;
            result.ManaPerSecond = ManaPerSecond;
            result.HotTicksPerSecond = HotTicksPerSecond;
            result.DirectHealsPerSecond = DirectHealsPerSecond;
            result.HotCritsPerSecond = HotCritsPerSecond;
            result.DirectCritsPerSecond = DirectCritsPerSecond;
            result.HealsPerSecond = HealsPerSecond;
            result.CritsPerSecond = CritsPerSecond;
            result.CastsPerSecond = CastsPerSecond;
            result.CastFraction = CastFraction;
            return result;
        }

        public void Accumulate(CombatFactor cf, float weight)
        {
            HealingPerSecond += cf.HealingPerSecond * weight;
            ManaPerSecond += cf.ManaPerSecond * weight;
            HotTicksPerSecond += cf.HotTicksPerSecond * weight;
            DirectHealsPerSecond += cf.DirectHealsPerSecond * weight;
            HotCritsPerSecond += cf.HotCritsPerSecond * weight;
            DirectCritsPerSecond += cf.DirectCritsPerSecond * weight;
            HealsPerSecond += cf.HealsPerSecond * weight;
            CritsPerSecond += cf.CritsPerSecond * weight;
            CastsPerSecond += cf.CastsPerSecond * weight;
            CastFraction += cf.CastFraction * weight;
        }
    }

    public class CombatFactors
    {
        public CombatFactor[] factors;

        public float HealingPerSecond;
        public float ManaPerSecond;
        public float HotTicksPerSecond;
        public float DirectHealsPerSecond;
        public float HotCritsPerSecond;
        public float DirectCritsPerSecond;
        public float CritsPerSecond;
        public float HealsPerSecond;
        public float CastsPerSecond;
        public float CastFraction;

        public float RejuvTicksPerSecond;
        public float HealingPerMana;

        public float TotalHealing;

        public CombatFactors()
        {
            int Count = EnumHelper.GetValues(typeof(HealingSpell)).Length;
            factors = new CombatFactor[Count];
            for (int i = 0; i < Count; i++)
            {
                factors[i] = new CombatFactor();
            }
            TotalHealing = 0;
        }

        public CombatFactors(CombatFactors copyOf)
        {
            int Count = EnumHelper.GetValues(typeof(HealingSpell)).Length;
            factors = new CombatFactor[Count];
            for (int i = 0; i < Count; i++)
            {
                factors[i] = copyOf.factors[i].Clone();
            }
            TotalHealing = 0;
        }

        public CombatFactors Clone()
        {
            return new CombatFactors(this);
        }

        public void Accumulate(CombatFactors cfs, float weight)
        {
            int Count = EnumHelper.GetValues(typeof(HealingSpell)).Length;
            for (int i = 0; i < Count; i++)
            {
                factors[i].Accumulate(cfs.factors[i], weight);
            }
            TotalHealing += cfs.TotalHealing * weight;
        }

        public void Compute()
        {
            HealingPerSecond = 0;
            ManaPerSecond = 0;
            HotTicksPerSecond = 0;
            DirectHealsPerSecond = 0;
            HotCritsPerSecond = 0;
            DirectCritsPerSecond = 0;
            CritsPerSecond = 0;
            HealsPerSecond = 0;
            CastsPerSecond = 0;
            CastFraction = 0;
            RejuvTicksPerSecond = 0;

            int Count = EnumHelper.GetValues(typeof(HealingSpell)).Length;
            for (int i = 0; i < Count; i++)
            {
                HealingPerSecond += factors[i].HealingPerSecond;
                ManaPerSecond += factors[i].ManaPerSecond;
                HotTicksPerSecond += factors[i].HotTicksPerSecond;
                DirectHealsPerSecond += factors[i].DirectHealsPerSecond;
                HotCritsPerSecond += factors[i].HotCritsPerSecond;
                DirectCritsPerSecond += factors[i].DirectCritsPerSecond;
                CritsPerSecond += factors[i].CritsPerSecond;
                HealsPerSecond += factors[i].HealsPerSecond;
                CastsPerSecond += factors[i].CastsPerSecond;
                CastFraction += factors[i].CastFraction;
                if (i == (int)HealingSpell.Rejuv || i == (int)HealingSpell.ManagedRejuv)
                {
                    RejuvTicksPerSecond += factors[i].HotTicksPerSecond;
                }
            }

            HealingPerMana = ManaPerSecond > 0 ? HealingPerSecond / ManaPerSecond : 0;
        }
    }

    public class SustainedResult
    {
        public SustainedResult(CharacterCalculationsTree calculatedStats, Stats stats)
        {
            spellMix = new SpellMix(calculatedStats.LocalCharacter, stats);

            #region Setup Mana regen
            SpiritMPS = StatConversion.GetSpiritRegenSec(stats.Intellect, stats.Spirit);
            replenishment = stats.ManaRestoreFromMaxManaPerSecond; 
            Mana = stats.Mana;
            GearMPS = stats.Mp5 / 5f;
            SpiritInCombatFraction = stats.SpellCombatManaRegeneration;
            ProcsMPS = stats.ManaRestore;
            #endregion
        }

        public void ApplyCombatStats(Stats stats) 
        {
            spellMix.ApplyCombatStats(stats);
        }

        public SpellMix spellMix;

        #region Passive Mana Regen
        public float Mana; // set by constructor
        public float SpiritMPS; // set by constructor
        public float GearMPS; // set by constructor
        public float SpiritInCombatFraction; // set by constructor
        private float replenishment; // set by constructor
        public float ReplenishmentUptime = 1f;
        public float ReplenishmentMPS { get { return Mana * replenishment * ReplenishmentUptime; } }
        public float RevitalizePPM = 0f;
        public float RevitalizeMPS { get { return RevitalizePPM * 0.01f * Mana; } }
        public float ProcsMPS; // set by constructor
        public float MPSInFSR { get { return RevitalizeMPS + ProcsMPS + GearMPS + ReplenishmentMPS + SpiritMPS * SpiritInCombatFraction; } }
        public float MPSOutFSR { get { return RevitalizeMPS + ProcsMPS + GearMPS + ReplenishmentMPS + SpiritMPS; } }
        public float OutOfCombatFraction = 0;
        public float ManaRegen { get { return OutOfCombatFraction * MPSOutFSR + (1f - OutOfCombatFraction) * MPSInFSR; } }
        #endregion

        #region Active Mana Regen
        public float ExtraMana { get { return InnervateMana; } }
        public float InnervateMana { get { return ManaPerInnervate * NumberOfInnervates; } }
        public float InnervateMPS { get { return InnervateMana / TotalTime; } }
        public float ManaPerInnervate = 0f;
        public float NumberOfInnervates = 0f;
        #endregion

        #region Saved unreduces cast fractions
        public float RejuvCF_unreduced = 0f;
        public float RegrowthCF_unreduced = 0f;
        public float ManagedRejuvCF_unreduced = 0f;
        public float ManagedRegrowthCF_unreduced = 0f;
        public float LifebloomCF_unreduced = 0f;
        public float LifebloomStackCF_unreduced = 0f;
        public float NourishCF_unreduced = 0f;
        public float WildGrowthCF_unreduced = 0f;
        public float SwiftmendCF_unreduced = 0f;
        public float IdleCF_unreduced = 1f;

        public float RejuvCF_unreducedOOM = 0f;
        public float RegrowthCF_unreducedOOM = 0f;
        public float ManagedRejuvCF_unreducedOOM = 0f;
        public float ManagedRegrowthCF_unreducedOOM = 0f;
        public float LifebloomCF_unreducedOOM = 0f;
        public float NourishCF_unreducedOOM = 0f;
        public float WildGrowthCF_unreducedOOM = 0f;
        public float IdleCF_unreducedOOM = 1f;
        #endregion

        public float TotalTime;
        public float TimeToOOM { get { return EffMPS > 0 ? Math.Min((ExtraMana + Mana) / EffMPS, TotalTime) : TotalTime; } }
        public float TimeToOOM_unreduced = 0f;
        public float TimeAfterOOM { get { return TotalTime - TimeToOOM; } }

        public float EffMPS { get { return spellMix.MPS - ManaRegen; } }

        public float TotalModifier { get { return TotalTime > TimeToOOM ? (TimeToOOM + BurnRegenFraction * TimeAfterOOM) / TotalTime : 1f; } }
        public float TimeToRegenAll { get { return Mana / MPSOutFSR; } }
        public float TimeToBurnAll { get { return Mana / EffMPS; } }
        public float BurnRegenFraction { get { return TimeToBurnAll / (TimeToRegenAll + TimeToBurnAll); } }

        public float TotalHealing { get { return spellMix.HPS * TotalTime * TotalModifier; } }
        public float TotalCastsPerMinute { get { return spellMix.CastsPerMinute * TotalModifier; } }
        public float TotalCritsPerMinute { get { return spellMix.CritsPerMinute * TotalModifier; } }
        public float TotalHealsPerMinute { get { return spellMix.HealsPerMinute * TotalModifier; } }

        public RotationSettings rotSettings;

        public CombatFactors getCombatFactors()
        {
            CombatFactors result = new CombatFactors();
            result.factors[(int)HealingSpell.Rejuv].HealingPerSecond = spellMix.RejuvHPS;
            result.factors[(int)HealingSpell.Rejuv].ManaPerSecond = spellMix.RejuvMPS;
            result.factors[(int)HealingSpell.Rejuv].HotTicksPerSecond = spellMix.RejuvHealsPerSecond;
            result.factors[(int)HealingSpell.Rejuv].DirectHealsPerSecond = 0;
            result.factors[(int)HealingSpell.Rejuv].HotCritsPerSecond = spellMix.RejuvCritsPerSecond;
            result.factors[(int)HealingSpell.Rejuv].DirectCritsPerSecond = 0;
            result.factors[(int)HealingSpell.Rejuv].CastsPerSecond = spellMix.RejuvCPS;
            result.factors[(int)HealingSpell.Rejuv].CastFraction = spellMix.RejuvCF;
            result.factors[(int)HealingSpell.Rejuv].Compute();

            result.factors[(int)HealingSpell.Regrowth].HealingPerSecond = spellMix.RegrowthHPS;
            result.factors[(int)HealingSpell.Regrowth].ManaPerSecond = spellMix.RegrowthMPS;
            result.factors[(int)HealingSpell.Regrowth].HotTicksPerSecond = spellMix.RegrowthHotTicksPerMinute / 60f;
            result.factors[(int)HealingSpell.Regrowth].DirectHealsPerSecond = spellMix.RegrowthCPS;
            result.factors[(int)HealingSpell.Regrowth].HotCritsPerSecond = 0;
            result.factors[(int)HealingSpell.Regrowth].DirectCritsPerSecond = spellMix.RegrowthCritsPerMinute / 60f;
            result.factors[(int)HealingSpell.Regrowth].CastsPerSecond = spellMix.RegrowthCPS;
            result.factors[(int)HealingSpell.Regrowth].CastFraction = spellMix.RegrowthCF;
            result.factors[(int)HealingSpell.Regrowth].Compute();

            result.factors[(int)HealingSpell.Lifebloom].HealingPerSecond = spellMix.LifebloomHPS;
            result.factors[(int)HealingSpell.Lifebloom].ManaPerSecond = spellMix.LifebloomMPS;
            result.factors[(int)HealingSpell.Lifebloom].HotTicksPerSecond = spellMix.LifebloomHotTicksPerMinute / 60f;
            result.factors[(int)HealingSpell.Lifebloom].DirectHealsPerSecond = spellMix.LifebloomCPS;
            result.factors[(int)HealingSpell.Lifebloom].HotCritsPerSecond = 0;
            result.factors[(int)HealingSpell.Lifebloom].DirectCritsPerSecond = spellMix.LifebloomCritsPerMinute / 60f;
            result.factors[(int)HealingSpell.Lifebloom].CastsPerSecond = spellMix.LifebloomCPS;
            result.factors[(int)HealingSpell.Lifebloom].CastFraction = spellMix.LifebloomCF;
            result.factors[(int)HealingSpell.Lifebloom].Compute();

            result.factors[(int)HealingSpell.ManagedRejuv].HealingPerSecond = spellMix.ManagedRejuvHPS;
            result.factors[(int)HealingSpell.ManagedRejuv].ManaPerSecond = spellMix.ManagedRejuvMPS;
            result.factors[(int)HealingSpell.ManagedRejuv].HotTicksPerSecond = spellMix.ManagedRejuvHealsPerSecond;
            result.factors[(int)HealingSpell.ManagedRejuv].DirectHealsPerSecond = 0;
            result.factors[(int)HealingSpell.ManagedRejuv].HotCritsPerSecond = spellMix.ManagedRejuvCritsPerSecond;
            result.factors[(int)HealingSpell.ManagedRejuv].DirectCritsPerSecond = 0;
            result.factors[(int)HealingSpell.ManagedRejuv].CastsPerSecond = spellMix.ManagedRejuvCPS;
            result.factors[(int)HealingSpell.ManagedRejuv].CastFraction = spellMix.ManagedRejuvCF;
            result.factors[(int)HealingSpell.ManagedRejuv].Compute();

            result.factors[(int)HealingSpell.ManagedRegrowth].HealingPerSecond = spellMix.ManagedRegrowthHPS;
            result.factors[(int)HealingSpell.ManagedRegrowth].ManaPerSecond = spellMix.ManagedRegrowthMPS;
            result.factors[(int)HealingSpell.ManagedRegrowth].HotTicksPerSecond = spellMix.ManagedRegrowthHotTicksPerMinute / 60f;
            result.factors[(int)HealingSpell.ManagedRegrowth].DirectHealsPerSecond = spellMix.ManagedRegrowthCPS;
            result.factors[(int)HealingSpell.ManagedRegrowth].HotCritsPerSecond = 0;
            result.factors[(int)HealingSpell.ManagedRegrowth].DirectCritsPerSecond = spellMix.ManagedRegrowthCritsPerMinute / 60f;
            result.factors[(int)HealingSpell.ManagedRegrowth].CastsPerSecond = spellMix.ManagedRegrowthCPS;
            result.factors[(int)HealingSpell.ManagedRegrowth].CastFraction = spellMix.ManagedRegrowthCF;
            result.factors[(int)HealingSpell.ManagedRegrowth].Compute();

            result.factors[(int)HealingSpell.ManagedLifebloom].HealingPerSecond = spellMix.LifebloomStackHPS;
            result.factors[(int)HealingSpell.ManagedLifebloom].ManaPerSecond = spellMix.LifebloomStackMPS;
            result.factors[(int)HealingSpell.ManagedLifebloom].HotTicksPerSecond = spellMix.LifebloomStackHotTicksPerMinute / 60f;
            result.factors[(int)HealingSpell.ManagedLifebloom].DirectHealsPerSecond = spellMix.LifebloomStackBPS;
            result.factors[(int)HealingSpell.ManagedLifebloom].HotCritsPerSecond = 0;
            result.factors[(int)HealingSpell.ManagedLifebloom].DirectCritsPerSecond = spellMix.LifebloomStackCritsPerMinute / 60f;
            result.factors[(int)HealingSpell.ManagedLifebloom].CastsPerSecond = spellMix.LifebloomStackCPS;
            result.factors[(int)HealingSpell.ManagedLifebloom].CastFraction = spellMix.LifebloomStackCF;
            result.factors[(int)HealingSpell.ManagedLifebloom].Compute();

            result.factors[(int)HealingSpell.WildGrowth].HealingPerSecond = spellMix.WildGrowthHPS;
            result.factors[(int)HealingSpell.WildGrowth].ManaPerSecond = spellMix.WildGrowthMPS;
            result.factors[(int)HealingSpell.WildGrowth].HotTicksPerSecond = spellMix.WildGrowthHealsPerMinute / 60f;
            result.factors[(int)HealingSpell.WildGrowth].DirectHealsPerSecond = 0;
            result.factors[(int)HealingSpell.WildGrowth].HotCritsPerSecond = 0;
            result.factors[(int)HealingSpell.WildGrowth].DirectCritsPerSecond = 0;
            result.factors[(int)HealingSpell.WildGrowth].CastsPerSecond = spellMix.WildGrowthCPS;
            result.factors[(int)HealingSpell.WildGrowth].CastFraction = spellMix.WildGrowthCF;
            result.factors[(int)HealingSpell.WildGrowth].Compute();

            result.factors[(int)HealingSpell.Swiftmend].HealingPerSecond = spellMix.SwiftmendHPS;
            result.factors[(int)HealingSpell.Swiftmend].ManaPerSecond = spellMix.SwiftmendMPS;
            result.factors[(int)HealingSpell.Swiftmend].HotTicksPerSecond = 0;
            result.factors[(int)HealingSpell.Swiftmend].DirectHealsPerSecond = spellMix.SwiftmendHealsPerMinute / 60f;
            result.factors[(int)HealingSpell.Swiftmend].HotCritsPerSecond = 0;
            result.factors[(int)HealingSpell.Swiftmend].DirectCritsPerSecond = spellMix.SwiftmendCritsPerMinute / 60f;
            result.factors[(int)HealingSpell.Swiftmend].CastsPerSecond = spellMix.SwiftmendCPS;
            result.factors[(int)HealingSpell.Swiftmend].CastFraction = spellMix.SwiftmendCF;
            result.factors[(int)HealingSpell.Swiftmend].Compute();

            result.factors[(int)HealingSpell.Nourish].HealingPerSecond = spellMix.NourishHPS;
            result.factors[(int)HealingSpell.Nourish].ManaPerSecond = spellMix.NourishMPS;
            result.factors[(int)HealingSpell.Nourish].HotTicksPerSecond = 0;
            result.factors[(int)HealingSpell.Nourish].DirectHealsPerSecond = spellMix.NourishHealsPerMinute / 60f;
            result.factors[(int)HealingSpell.Nourish].HotCritsPerSecond = 0;
            result.factors[(int)HealingSpell.Nourish].DirectCritsPerSecond = spellMix.NourishCritsPerMinute / 60f;
            result.factors[(int)HealingSpell.Nourish].CastsPerSecond = spellMix.NourishCPS;
            result.factors[(int)HealingSpell.Nourish].CastFraction = spellMix.NourishCF;
            result.factors[(int)HealingSpell.Nourish].Compute();

            result.factors[(int)HealingSpell.HealingTouch].HealingPerSecond = 0;
            result.factors[(int)HealingSpell.HealingTouch].ManaPerSecond = 0;
            result.factors[(int)HealingSpell.HealingTouch].HotTicksPerSecond = 0;
            result.factors[(int)HealingSpell.HealingTouch].DirectHealsPerSecond = 0;
            result.factors[(int)HealingSpell.HealingTouch].HotCritsPerSecond = 0;
            result.factors[(int)HealingSpell.HealingTouch].DirectCritsPerSecond = 0;
            result.factors[(int)HealingSpell.HealingTouch].CastsPerSecond = 0;
            result.factors[(int)HealingSpell.HealingTouch].CastFraction = 0;
            result.factors[(int)HealingSpell.HealingTouch].Compute();

            result.TotalHealing = TotalHealing;

            return result;
        }
    }

    public enum SingleTargetBurstRotations
    {
        AutoSelect = 0,
        AutoSelectForceSwiftmend,
        RjN,
        RjLbN,
        RjRgN,
        RjRgLbN,
        RjRg,
        RjLbRg,
        Rg,
        RgN,        
        N,
        RjN_s,
        RjLbN_s,
        RjRgN_s,
        RjRgLbN_s,
        RjRg_s,
        RjLbRg_s,
        Rg_s,
        RgN_s
    };

    public enum HealTargetTypes { TankHealing = 0, RaidHealing };

    public enum SpellList { HealingTouch = 0, Nourish, Regrowth, Rejuvenation };

    public enum LifeBloomType { Slow = 0, Fast, Rolling, Slow2, Fast2 };

    public class RotationSettings
    {
        public float averageLifebloomStacks, averageRejuv, averageRegrowth;
        public LifeBloomType lifeBloomType;
        public int SwiftmendPerMin, WildGrowthPerMin;
        public float RejuvFraction, RegrowthFraction, LifebloomFraction, NourishFraction;
        public float nourish0, nourish1, nourish2, nourish3, nourish4;
        public float livingSeedEfficiency;
        public HealTargetTypes healTarget;
    }

    public class SingleTargetBurstResult
    {
        public SpellMix spellMix;
        public float HPS { get { return spellMix.HPS; } }
        public float HPM { get { return spellMix.HPM; } }

        public SingleTargetBurstRotations rotation;

        public SingleTargetBurstResult(SingleTargetBurstRotations rotation, SpellMix spells, 
            float rjCF, float rgCF, float lbCF, float smCPM, float nCF, bool rgAgain)
        {
            spellMix = new SpellMix(spells);
            spellMix.RegrowthChainCast = rgAgain;

            spellMix.RejuvCF = rjCF;
            spellMix.RegrowthCF = rgCF;
            spellMix.LifebloomStackCF = lbCF;

            spellMix.CreateSwiftmend();
            spellMix.SwiftmendCPM = smCPM;

            spellMix.NourishCF = nCF;

            int numberOfHots = 0
                + (rjCF > 0 ? 1 : 0)
                + (rgCF > 0 ? 1 : 0)
                + (lbCF > 0 ? 1 : 0);
            if (numberOfHots == 0) spellMix.CalculateNourishHPS(1, 0, 0, 0, 0);
            if (numberOfHots == 1) spellMix.CalculateNourishHPS(0, 1, 0, 0, 0);
            if (numberOfHots == 2) spellMix.CalculateNourishHPS(0, 0, 1, 0, 0);
            if (numberOfHots == 3) spellMix.CalculateNourishHPS(0, 0, 0, 1, 0);
            if (numberOfHots == 4) spellMix.CalculateNourishHPS(0, 0, 0, 0, 1);

            this.rotation = rotation;
        }
    }

    public class Solver
    {
        public static SustainedResult SimulateHealing(CharacterCalculationsTree calculatedStats, Stats stats, CalculationOptionsTree calcOpts, RotationSettings rotSettings)
        {
            SpellProfile profile = calcOpts.Current;

            SustainedResult rot = new SustainedResult(calculatedStats, stats);

            #region Setup spell mix
            rot.spellMix.LifebloomStackType = rotSettings.lifeBloomType;
            rot.spellMix.LifebloomStacks = rotSettings.averageLifebloomStacks;
            rot.spellMix.RejuvCF = rotSettings.RejuvFraction;
            rot.spellMix.ManagedRejuvAvg = rotSettings.averageRejuv;
            rot.spellMix.RegrowthCF = rotSettings.RegrowthFraction;
            rot.spellMix.ManagedRegrowthAvg = rotSettings.averageRegrowth;
            rot.spellMix.LifebloomCF = rotSettings.LifebloomFraction;
            rot.spellMix.NourishCF = rotSettings.NourishFraction;
            rot.spellMix.WildGrowthCPM = rotSettings.WildGrowthPerMin;
            rot.spellMix.CreateSwiftmend();
            rot.spellMix.SwiftmendCPM = rotSettings.SwiftmendPerMin;
            rot.spellMix.CalculateNourishHPS(rotSettings.nourish0, rotSettings.nourish1,
                rotSettings.nourish2, rotSettings.nourish3, rotSettings.nourish4);
            #endregion

            rot.rotSettings = rotSettings;

            rot.TotalTime = profile.FightDuration;

            #region Mana regeneration
            rot.ReplenishmentUptime = profile.ReplenishmentUptime / 100f; 
            rot.OutOfCombatFraction = 0f;
            rot.RevitalizePPM = (float)profile.RevitalizePPM / 100f;
            #endregion

            #region Innervates
            // 3 min CD, takes 15 seconds to apply
            rot.NumberOfInnervates = (float)Math.Ceiling((profile.FightDuration - 15f) / 180f);
            rot.ManaPerInnervate = 2.25f * profile.Innervates;       // Use self innervate?
            if (calculatedStats.LocalCharacter.DruidTalents.GlyphOfInnervate) rot.ManaPerInnervate += 0.45f;
            rot.ManaPerInnervate *= Rawr.Tree.TreeConstants.BaseMana;
            #endregion

            #region Correct cast fractions 
            rot.RejuvCF_unreduced = rot.spellMix.RejuvCF;
            rot.RegrowthCF_unreduced = rot.spellMix.RegrowthCF;
            rot.ManagedRejuvCF_unreduced = rot.spellMix.ManagedRejuvCF;
            rot.ManagedRegrowthCF_unreduced = rot.spellMix.ManagedRegrowthCF;
            rot.LifebloomCF_unreduced = rot.spellMix.LifebloomCF;
            rot.NourishCF_unreduced = rot.spellMix.NourishCF;
            rot.WildGrowthCF_unreduced = rot.spellMix.WildGrowthCF;
            rot.SwiftmendCF_unreduced = rot.spellMix.SwiftmendCF;
            rot.LifebloomStackCF_unreduced = rot.spellMix.LifebloomStackCF;

            float IdleCF = profile.IdleCastTimePercent / 100f;
            rot.IdleCF_unreduced = IdleCF;

            for (int i = 0; i < 9 && rot.spellMix.TotalCF + IdleCF > 1f; i++)
            {
                List<float> gains = new List<float>();
                if (profile.AdjustTimeRejuvOrder == i) gains.Add(profile.AdjustTimeRejuv / 100f * rot.spellMix.RejuvCF);
                else gains.Add(0);
                if (profile.AdjustTimeRegrowthOrder == i) gains.Add(profile.AdjustTimeRegrowth / 100f * rot.spellMix.RegrowthCF);
                else gains.Add(0);
                if (profile.AdjustTimeNourishOrder == i) gains.Add(profile.AdjustTimeNourish / 100f * rot.spellMix.NourishCF);
                else gains.Add(0);
                if (profile.AdjustTimeLifebloomOrder == i) gains.Add(profile.AdjustTimeLifebloom / 100f * rot.spellMix.LifebloomCF);
                else gains.Add(0);
                if (profile.AdjustTimeSwiftmendOrder == i) gains.Add(rot.spellMix.SwiftmendCF);
                else gains.Add(0);
                if (profile.AdjustTimeWildGrowthOrder == i) gains.Add(profile.AdjustTimeWildGrowth / 100f * rot.spellMix.WildGrowthCF);
                else gains.Add(0);
                if (profile.AdjustTimeIdleOrder == i) gains.Add(profile.AdjustTimeIdle / 100f * IdleCF);
                else gains.Add(0);
                if (profile.AdjustTimeManagedRejuvOrder == i) gains.Add(profile.AdjustTimeManagedRejuv / 100f * rot.spellMix.ManagedRejuvCF);
                else gains.Add(0);
                if (profile.AdjustTimeManagedRegrowthOrder == i) gains.Add(profile.AdjustTimeManagedRegrowth / 100f * rot.spellMix.ManagedRegrowthCF);
                else gains.Add(0);
                if (profile.AdjustTimeManagedLifebloomStackOrder == i) gains.Add(profile.AdjustTimeManagedLifebloomStack / 100f * rot.spellMix.LifebloomStackCF);
                else gains.Add(0);
                
                float GainingThisRound = 0f;
                foreach (float ff in gains) GainingThisRound += ff;
                if (GainingThisRound == 0) continue;
                float Factor = Math.Min(GainingThisRound, rot.spellMix.TotalCF + IdleCF - 1f) / GainingThisRound;

                if (profile.AdjustTimeRejuvOrder == i)
                    rot.spellMix.RejuvCF = (float)Math.Round(rot.spellMix.RejuvCF - gains[0] * Factor, 6);
                if (profile.AdjustTimeRegrowthOrder == i)
                    rot.spellMix.RegrowthCF = (float)Math.Round(rot.spellMix.RegrowthCF - gains[1] * Factor, 6);
                if (profile.AdjustTimeNourishOrder == i)
                    rot.spellMix.NourishCF = (float)Math.Round(rot.spellMix.NourishCF - gains[2] * Factor, 6);
                if (profile.AdjustTimeLifebloomOrder == i)
                    rot.spellMix.LifebloomCF = (float)Math.Round(rot.spellMix.LifebloomCF - gains[3] * Factor, 6);
                if (profile.AdjustTimeSwiftmendOrder == i)
                {
                    // you can't just reduce cast fraction here, because 'recasting lost hots' is calculated
                    // alternative version would be to slower swiftmend's healing by the amount of healing expected lost...
                    // rot.spellMix.SwiftmendCPM = 0;
                    rot.spellMix.SwiftmendCF = (float)Math.Round(rot.spellMix.SwiftmendCF - gains[4] * Factor, 6);
                }
                if (profile.AdjustTimeWildGrowthOrder == i)
                    rot.spellMix.WildGrowthCF = (float)Math.Round(rot.spellMix.WildGrowthCF - gains[5] * Factor, 6);
                if (profile.AdjustTimeIdleOrder == i)
                    IdleCF = (float)Math.Round(IdleCF - gains[6] * Factor, 6);
                if (profile.AdjustTimeManagedRejuvOrder == i)
                    rot.spellMix.ManagedRejuvCF = (float)Math.Round(rot.spellMix.ManagedRejuvCF - gains[7] * Factor, 6);
                if (profile.AdjustTimeManagedRegrowthOrder == i)
                    rot.spellMix.ManagedRegrowthCF = (float)Math.Round(rot.spellMix.ManagedRegrowthCF - gains[8] * Factor, 6);
                if (profile.AdjustTimeManagedLifebloomStackOrder == i)
                    rot.spellMix.LifebloomStackCF = (float)Math.Round(rot.spellMix.LifebloomStackCF - gains[9] * Factor, 6);
            }

            if (Math.Round(rot.spellMix.TotalCF + IdleCF, 4) > 1f)
            {
                //float Factor = (1f - rot.spellMix.SwiftmendCF) / (rot.spellMix.TotalCF + IdleCF - rot.spellMix.SwiftmendCF);
                float Factor = 1f / (rot.spellMix.TotalCF + IdleCF);
                rot.spellMix.LifebloomStackCF *= Factor;
                rot.spellMix.RejuvCF *= Factor;
                rot.spellMix.RegrowthCF *= Factor;
                rot.spellMix.NourishCF *= Factor;
                rot.spellMix.LifebloomCF *= Factor;
                rot.spellMix.SwiftmendCF *= Factor;
                rot.spellMix.WildGrowthCF *= Factor;
                rot.spellMix.ManagedRejuvCF *= Factor;
                rot.spellMix.ManagedRegrowthCF *= Factor;
                rot.spellMix.LifebloomStackCF *= Factor;
                // Now, rot.MaxPrimaryCF *must* be 1f exactly.
            }
            /*
            if (Math.Round(rot.spellMix.TotalCF + IdleCF, 4) > 1f)
            {
                rot.spellMix.SwiftmendCPM = 0;
            }*/
            #endregion

            #region Correct going OOM
            rot.TimeToOOM_unreduced = rot.TimeToOOM;
            rot.IdleCF_unreducedOOM = rot.spellMix.IdleCF;
            rot.RejuvCF_unreducedOOM = rot.spellMix.RejuvCF;
            rot.RegrowthCF_unreducedOOM = rot.spellMix.RegrowthCF;
            rot.ManagedRejuvCF_unreducedOOM = rot.spellMix.ManagedRejuvCF;
            rot.ManagedRegrowthCF_unreducedOOM = rot.spellMix.ManagedRegrowthCF;
            rot.LifebloomCF_unreducedOOM = rot.spellMix.LifebloomCF;
            rot.NourishCF_unreducedOOM = rot.spellMix.NourishCF;
            rot.WildGrowthCF_unreducedOOM = rot.spellMix.WildGrowthCF;

            // If you cast less, you will also see less returns. This is calculated elsewhere
            // in the Special Effects. Repeated calculations are necessary.
            // We assume this effect is convergent.

            float f = Math.Max(0.0f, (rot.TimeToOOM * (rot.spellMix.MPS - rot.ManaRegen) + rot.TotalTime * rot.ManaRegen) / (rot.spellMix.MPS * rot.TotalTime));
            // We want our MPS to become f*MPS

            float MPStoGain = rot.spellMix.MPS * (1f - f);

            for (int i = 0; i < 5 && MPStoGain > 0; i++)
            {
                List<float> gains = new List<float>();
                if (profile.ReduceOOMRejuvOrder == i)
                {
                    float r = Math.Min(1f, (MPStoGain / rot.spellMix.rejuvenate.ManaCost * rot.spellMix.rejuvenate.CastTime));
                    float s = Math.Max(rot.spellMix.RejuvCF * (1f - profile.ReduceOOMRejuv / 100f), rot.spellMix.RejuvCF - r);
                    gains.Add(rot.spellMix.rejuvenate.ManaCost * (rot.RejuvCF_unreducedOOM - s) / rot.spellMix.rejuvenate.CastTime);
                }
                else gains.Add(0);
                if (profile.ReduceOOMRegrowthOrder == i)
                {
                    float r = Math.Min(1f, (MPStoGain / rot.spellMix.regrowth.ManaCost * rot.spellMix.regrowth.CastTime));
                    float s = Math.Max(rot.spellMix.RegrowthCF * (1f - profile.ReduceOOMRegrowth / 100f), rot.spellMix.RegrowthCF - r);
                    gains.Add(rot.spellMix.regrowth.ManaCost * (rot.RegrowthCF_unreducedOOM - s) / rot.spellMix.regrowth.CastTime);
                }
                else gains.Add(0); 
                if (profile.ReduceOOMLifebloomOrder == i)
                {
                    float r = Math.Min(1f, (MPStoGain / rot.spellMix.lifebloom.ManaCost * rot.spellMix.lifebloom.CastTime));
                    float s = Math.Max(rot.spellMix.LifebloomCF * (1f - profile.ReduceOOMLifebloom / 100f), rot.spellMix.LifebloomCF - r);
                    gains.Add(rot.spellMix.lifebloom.ManaCost * (rot.LifebloomCF_unreducedOOM - s) / rot.spellMix.lifebloom.CastTime);
                }
                else gains.Add(0); 
                if (profile.ReduceOOMNourishOrder == i)
                {
                    float r = Math.Min(1f, (MPStoGain / rot.spellMix.nourish[0].ManaCost * rot.spellMix.nourish[0].CastTime));
                    float s = Math.Max(rot.spellMix.NourishCF * (1f - profile.ReduceOOMNourish / 100f), rot.spellMix.NourishCF - r);
                    gains.Add(rot.spellMix.nourish[0].ManaCost * (rot.NourishCF_unreducedOOM - s) / rot.spellMix.nourish[0].CastTime);
                }
                else gains.Add(0); 
                if (profile.ReduceOOMWildGrowthOrder == i)
                {
                    float r = Math.Min(1f, (MPStoGain / rot.spellMix.wildGrowth.ManaCost * rot.spellMix.wildGrowth.CastTime));
                    float s = Math.Max(rot.spellMix.WildGrowthCF * (1f - profile.ReduceOOMWildGrowth / 100f), rot.spellMix.WildGrowthCF - r);
                    gains.Add(rot.spellMix.wildGrowth.ManaCost * (rot.WildGrowthCF_unreducedOOM - s) / rot.spellMix.wildGrowth.CastTime);
                }
                else gains.Add(0);

                float GainingThisRound = 0f;
                foreach (float ff in gains) GainingThisRound += ff;
                if (GainingThisRound == 0) continue;
                float Factor = Math.Min(GainingThisRound, MPStoGain) / GainingThisRound;
                MPStoGain -= Math.Min(GainingThisRound, MPStoGain);

                if (profile.ReduceOOMRejuvOrder == i)
                {
                    rot.spellMix.RejuvCF -= gains[0] * Factor / rot.spellMix.rejuvenate.ManaCost * rot.spellMix.rejuvenate.CastTime;
                    rot.spellMix.RejuvCF = (float)Math.Round(rot.spellMix.RejuvCF, 6);
                }
                if (profile.ReduceOOMRegrowthOrder == i)
                {
                    rot.spellMix.RegrowthCF -= gains[1] * Factor / rot.spellMix.regrowth.ManaCost * rot.spellMix.regrowth.CastTime;
                    rot.spellMix.RegrowthCF = (float)Math.Round(rot.spellMix.RegrowthCF, 6);
                }
                if (profile.ReduceOOMLifebloomOrder == i)
                {
                    rot.spellMix.LifebloomCF -= gains[2] * Factor / rot.spellMix.lifebloom.ManaCost * rot.spellMix.lifebloom.CastTime;
                    rot.spellMix.LifebloomCF = (float)Math.Round(rot.spellMix.LifebloomCF, 6);
                }
                if (profile.ReduceOOMNourishOrder == i)
                {
                    rot.spellMix.NourishCF -= gains[3] * Factor / rot.spellMix.nourish[0].ManaCost * rot.spellMix.nourish[0].CastTime;
                    rot.spellMix.NourishCF = (float)Math.Round(rot.spellMix.NourishCF, 6);
                }
                if (profile.ReduceOOMWildGrowthOrder == i)
                {
                    rot.spellMix.WildGrowthCF -= gains[4] * Factor / rot.spellMix.wildGrowth.ManaCost * rot.spellMix.wildGrowth.CastTime;
                    rot.spellMix.WildGrowthCF = (float)Math.Round(rot.spellMix.WildGrowthCF, 6);
                }
            }
            #endregion

            return rot;
        }

        public static String SingleTargetRotationToText(SingleTargetBurstRotations rotation) {
            switch (rotation) {
                case SingleTargetBurstRotations.AutoSelect:
                    return "Autoselect";
                case SingleTargetBurstRotations.AutoSelectForceSwiftmend:
                    return "Autoselect + Swiftmend";
                case SingleTargetBurstRotations.RjN:
                    return "RjN*";
                case SingleTargetBurstRotations.RjLbN:
                    return "RjLbN*";
                case SingleTargetBurstRotations.RjRgN:
                    return "RjRgN*";
                case SingleTargetBurstRotations.RjRgLbN:
                    return "RjRgLbN*";
                case SingleTargetBurstRotations.RjRg:
                    return "RjRg*";
                case SingleTargetBurstRotations.RjLbRg:
                    return "RjLbRg*";
                case SingleTargetBurstRotations.Rg:
                    return "Rg*";
                case SingleTargetBurstRotations.RgN:
                    return "RgN*";
                case SingleTargetBurstRotations.N:
                    return "N*";
                case SingleTargetBurstRotations.RjN_s:
                    return "RjN*+S";
                case SingleTargetBurstRotations.RjLbN_s:
                    return "RjLbN*+S";
                case SingleTargetBurstRotations.RjRgN_s:
                    return "RjRgN*+S";
                case SingleTargetBurstRotations.RjRgLbN_s:
                    return "RjRgLbN*+S";
                case SingleTargetBurstRotations.RjRg_s:
                    return "RjRg*+S";
                case SingleTargetBurstRotations.RjLbRg_s:
                    return "RjLbRg*+S";
                case SingleTargetBurstRotations.Rg_s:
                    return "Rg*+S";
                case SingleTargetBurstRotations.RgN_s:
                    return "RgN*+S";
                default:
                    return "Unknown";
            }
        }

        public static SingleTargetBurstRotations SingleTargetIndexToRotation(int index)
        {
            switch (index)
            {
                case 0:
                default:
                    return SingleTargetBurstRotations.AutoSelect;
                case 1:
                    return SingleTargetBurstRotations.AutoSelectForceSwiftmend;
                case 2:
                    return SingleTargetBurstRotations.RjN;
                case 3:
                    return SingleTargetBurstRotations.RjLbN;
                case 4:
                    return SingleTargetBurstRotations.RjRgN;
                case 5:
                    return SingleTargetBurstRotations.RjRgLbN;
                case 6:
                    return SingleTargetBurstRotations.RjRg;
                case 7:
                    return SingleTargetBurstRotations.RjLbRg;
                case 8:
                    return SingleTargetBurstRotations.Rg;
                case 9:
                    return SingleTargetBurstRotations.RgN;
                case 10:
                    return SingleTargetBurstRotations.N;
                case 11:
                    return SingleTargetBurstRotations.RjN_s;
                case 12:
                    return SingleTargetBurstRotations.RjLbN_s;
                case 13:
                    return SingleTargetBurstRotations.RjRgN_s;
                case 14:
                    return SingleTargetBurstRotations.RjRgLbN_s;
                case 15:
                    return SingleTargetBurstRotations.RjRg_s;
                case 16:
                    return SingleTargetBurstRotations.RjLbRg_s;
                case 17:
                    return SingleTargetBurstRotations.Rg_s;
                case 18:
                    return SingleTargetBurstRotations.RgN_s;
            }
        }

        public static SingleTargetBurstResult[] CalculateSingleTargetBurst(CharacterCalculationsTree calculatedStats, Stats stats, CalculationOptionsTree calcOpts, SingleTargetBurstRotations rotation)
        {
            SpellMix spellMix = new SpellMix(calculatedStats.LocalCharacter, stats);

            SpellProfile profile = calcOpts.Current;

            switch (profile.LifebloomStackType)
            {
                case 0:
                    spellMix.LifebloomStackType = LifeBloomType.Slow;
                    break;
                case 1:
                    spellMix.LifebloomStackType = LifeBloomType.Fast;
                    break;
                case 2:
                    spellMix.LifebloomStackType = LifeBloomType.Rolling ;
                    break;
                case 3:
                    spellMix.LifebloomStackType = LifeBloomType.Slow2;
                    break;
                case 4:
                    spellMix.LifebloomStackType = LifeBloomType.Fast2;
                    break;
                default:
                    // err...
                    spellMix.LifebloomStackType = LifeBloomType.Rolling;
                    break;
            }

            float RejuvFraction = spellMix.rejuvenate.CastTime / spellMix.rejuvenate.Duration;
            float RegrowthFraction = spellMix.regrowth.CastTime / spellMix.regrowth.Duration;
            float LifebloomFraction = spellMix.LifebloomStackSpell.CastTime / spellMix.LifebloomStackSpell.Duration;
            float SwiftFraction = spellMix.swiftmend.CastTime * 4f / 60f;

            // RJ + N1 
            SingleTargetBurstResult RjN1 = new SingleTargetBurstResult(SingleTargetBurstRotations.RjN,
                spellMix, RejuvFraction, 0, 0, 0, (1f - RejuvFraction), false);
            // RJ + LB + N2 
            SingleTargetBurstResult RjLbN2 = new SingleTargetBurstResult(SingleTargetBurstRotations.RjLbN,
                spellMix, RejuvFraction, 0, LifebloomFraction, 0, (1f - RejuvFraction - LifebloomFraction), false);
            // RJ + RG + N2
            SingleTargetBurstResult RjRgN2 = new SingleTargetBurstResult(SingleTargetBurstRotations.RjRgN,
                spellMix, RejuvFraction, RegrowthFraction, 0, 0, (1f - RejuvFraction - RegrowthFraction), false);
            // RJ + RG + LB + N3 
            SingleTargetBurstResult RjRgLbN3 = new SingleTargetBurstResult(SingleTargetBurstRotations.RjRgLbN,
                spellMix, RejuvFraction, RegrowthFraction, LifebloomFraction, 0,
                (1f - RejuvFraction - RegrowthFraction - LifebloomFraction), false);
            // RJ + RG 
            SingleTargetBurstResult RjRg = new SingleTargetBurstResult(SingleTargetBurstRotations.RjRg,
                spellMix, RejuvFraction, (1f - RejuvFraction), 0, 0, 0, true);
            // RJ + LB + RG
            SingleTargetBurstResult RjLbRg = new SingleTargetBurstResult(SingleTargetBurstRotations.RjLbRg,
                spellMix, RejuvFraction, (1f - RejuvFraction - LifebloomFraction), LifebloomFraction, 0, 0, true);
            // RG
            SingleTargetBurstResult Rg = new SingleTargetBurstResult(SingleTargetBurstRotations.Rg,
                spellMix, 0, 1f, 0, 0, 0, true);
            // RG + N1
            SingleTargetBurstResult RgN1 = new SingleTargetBurstResult(SingleTargetBurstRotations.RgN,
                spellMix, 0, RegrowthFraction, 0, 0, (1f - RegrowthFraction), false);
            // N0
            SingleTargetBurstResult N0 = new SingleTargetBurstResult(SingleTargetBurstRotations.N,
                spellMix, 0, 0, 0, 0, 1f, false);

            // RJ + N1 S
            SingleTargetBurstResult RjN1S = new SingleTargetBurstResult(SingleTargetBurstRotations.RjN_s,
                spellMix, RejuvFraction, 0, 0, 4, (1f - RejuvFraction - SwiftFraction), false);
            // RJ + LB + N2 S
            SingleTargetBurstResult RjLbN2S = new SingleTargetBurstResult(SingleTargetBurstRotations.RjLbN_s,
                spellMix, RejuvFraction, 0, LifebloomFraction, 4,
                (1f - RejuvFraction - LifebloomFraction - SwiftFraction), false);
            // RJ + RG + N2 S
            SingleTargetBurstResult RjRgN2S = new SingleTargetBurstResult(SingleTargetBurstRotations.RjRgN_s,
                spellMix, RejuvFraction, RegrowthFraction, 0, 4,
                (1f - RejuvFraction - RegrowthFraction - SwiftFraction), false);
            // RJ + RG + LB + N3 S
            SingleTargetBurstResult RjRgLbN3S = new SingleTargetBurstResult(SingleTargetBurstRotations.RjRgLbN_s,
                spellMix, RejuvFraction, RegrowthFraction, LifebloomFraction, 4,
                (1f - RejuvFraction - RegrowthFraction - LifebloomFraction - SwiftFraction), false);
            // RJ + RG S
            SingleTargetBurstResult RjRgS = new SingleTargetBurstResult(SingleTargetBurstRotations.RjRg_s,
                spellMix, RejuvFraction, (1f - RejuvFraction - SwiftFraction), 0, 4, 0, true);
            // RJ + LB + RG S
            SingleTargetBurstResult RjLbRgS = new SingleTargetBurstResult(SingleTargetBurstRotations.RjLbRg_s,
                spellMix, RejuvFraction, (1f - RejuvFraction - LifebloomFraction - SwiftFraction),
                LifebloomFraction, 4, 0, true);
            // RG S
            SingleTargetBurstResult RgS = new SingleTargetBurstResult(SingleTargetBurstRotations.Rg_s, 
                spellMix, 0, (1f - SwiftFraction), 0, 4, 0, true);
            // RG + N1 S
            SingleTargetBurstResult RgN1S = new SingleTargetBurstResult(SingleTargetBurstRotations.RgN_s,
                spellMix, 0, RegrowthFraction, 0, 4, (1f - RegrowthFraction - SwiftFraction), false);

            SingleTargetBurstResult result = new SingleTargetBurstResult(SingleTargetBurstRotations.AutoSelect, 
                spellMix, 0, 0, 0, 0, 0, false);

            if (rotation == SingleTargetBurstRotations.AutoSelect ||
                rotation == SingleTargetBurstRotations.AutoSelectForceSwiftmend)
            {
                if (rotation != SingleTargetBurstRotations.AutoSelectForceSwiftmend)
                {
                    if (RjN1.HPS > result.HPS) result = RjN1;
                    if (RjLbN2.HPS > result.HPS) result = RjLbN2;
                    if (RjRgN2.HPS > result.HPS) result = RjRgN2;
                    if (RjRgLbN3.HPS > result.HPS) result = RjRgLbN3;
                    if (RjRg.HPS > result.HPS) result = RjRg;
                    if (RjLbRg.HPS > result.HPS) result = RjLbRg;
                    if (Rg.HPS > result.HPS) result = Rg;
                    if (N0.HPS > result.HPS) result = N0;
                }

                if (RjN1S.HPS > result.HPS) result = RjN1S;
                if (RjLbN2S.HPS > result.HPS) result = RjLbN2S;
                if (RjRgN2S.HPS > result.HPS) result = RjRgN2S;
                if (RjRgLbN3S.HPS > result.HPS) result = RjRgLbN3S;
                if (RjRgS.HPS > result.HPS) result = RjRgS;
                if (RjLbRgS.HPS > result.HPS) result = RjLbRgS;
                if (RgS.HPS > result.HPS) result = RgS;
                if (RgN1S.HPS > result.HPS) result = RgN1S;
            }
            else
            {
                switch (rotation)
                {
                    case SingleTargetBurstRotations.RjN:
                        result = RjN1;
                        break;
                    case SingleTargetBurstRotations.RjLbN:
                        result = RjLbN2;
                        break;
                    case SingleTargetBurstRotations.RjRgN:
                        result = RjRgN2;
                        break;
                    case SingleTargetBurstRotations.RjRgLbN:
                        result = RjRgLbN3;
                        break;
                    case SingleTargetBurstRotations.RjRg:
                        result = RjRg;
                        break;
                    case SingleTargetBurstRotations.RjLbRg:
                        result = RjLbRg;
                        break;
                    case SingleTargetBurstRotations.Rg:
                        result = Rg;
                        break;
                    case SingleTargetBurstRotations.RgN:
                        result = RgN1;
                        break;
                    case SingleTargetBurstRotations.N:
                        result = N0;
                        break;
                    case SingleTargetBurstRotations.RjN_s:
                        result = RjN1S;
                        break;
                    case SingleTargetBurstRotations.RjLbN_s:
                        result = RjLbN2S;
                        break;
                    case SingleTargetBurstRotations.RjRgN_s:
                        result = RjRgN2S;
                        break;
                    case SingleTargetBurstRotations.RjRgLbN_s:
                        result = RjRgLbN3S;
                        break;
                    case SingleTargetBurstRotations.RjRg_s:
                        result = RjRgS;
                        break;
                    case SingleTargetBurstRotations.RjLbRg_s:
                        result = RjLbRgS;
                        break;
                    case SingleTargetBurstRotations.Rg_s:
                        result = RgS;
                        break;
                    case SingleTargetBurstRotations.RgN_s:
                        result = RgN1S;
                        break;
                    default:
                        break;
                }
            }

            List<SingleTargetBurstResult> rots = new List<SingleTargetBurstResult>();
            rots.Add(result);
            rots.Add(result);
            rots.Add(RjN1);
            rots.Add(RjLbN2);
            rots.Add(RjRgN2);
            rots.Add(RjRgLbN3);
            rots.Add(RjRg);
            rots.Add(RjLbRg);
            rots.Add(Rg);
            rots.Add(RgN1);
            rots.Add(N0);
            rots.Add(RjN1S);
            rots.Add(RjLbN2S);
            rots.Add(RjRgN2S);
            rots.Add(RjRgLbN3S);
            rots.Add(RjRgS);
            rots.Add(RjLbRgS);
            rots.Add(RgS);
            rots.Add(RgN1S);

            return rots.ToArray();
        }
    }

    public static class TreeConstants
    {
        // Master is now in Base.BaseStats and Base.StatConversion
        public static float BaseMana;  // Keep since this is more convenient reference than calling the whole BaseStats function every time // = 3496f;
    }
}
