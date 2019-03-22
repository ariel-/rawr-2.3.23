using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rawr
{
    public enum Trigger : int
    {
        /// <summary>The player activates the ability manually</summary>
        Use,

        #region General Spells
        /// <summary>Any spell (damage or heal) lands on the target (hit or crit)</summary>
        SpellHit,
        /// <summary>Any spell (damage or heal) crits on the target</summary>
        SpellCrit,
        /// <summary>Any spell (damage or heal) is cast, regardless of result</summary>
        SpellCast,
        /// <summary>Any spell (damage or heal) misses the target</summary>
        SpellMiss,
        #endregion
        #region Damage Spells
        /// <summary>Any Damage spell (non-heal) lands on the target (hit or crit)</summary>
        DamageSpellHit,
        /// <summary>Any Damage spell (non-heal) crits on the target</summary>
        DamageSpellCrit,
        /// <summary>Any Damage spell (non-heal) is cast, regardless of result</summary>
        DamageSpellCast,
        #endregion
        #region Healing Spells
        /// <summary>Any Heal spell (non-damage) lands on the target (hit or crit)</summary>
        HealingSpellHit,
        /// <summary>Any Heal spell (non-damage) crits on the target</summary>
        HealingSpellCrit,
        /// <summary>Any Heal spell (non-damage) is cast, regardless of result</summary>
        HealingSpellCast,
        #endregion

        #region General Physical Attacks
        /// <summary>Any physical damage (melee or ranged) lands on the target (hit or crit)</summary>
        PhysicalHit,
        /// <summary>Any physical damage (melee or ranged) crits on the target</summary>
        PhysicalCrit,
        #endregion
        #region Melee Physical Attacks
        /// <summary>Any melee lands on the target (hit or crit)</summary>
        MeleeHit,
        /// <summary>Any melee crits on the target</summary>
        MeleeCrit,
        /// <summary>Any melee attack lands (special trigger for 'Tiny Abobination in a Jar' (Icecrown Citadel) and Shadowmourne (Icecrown Citadel). Identical to Meleehit for some, may have additional procs happening for other classes (Retribution is one)</summary>
        MeleeAttack,
        #endregion
        #region Ranged Physical Attacks
        /// <summary>Any ranged lands on the target (hit or crit)</summary>
        RangedHit,
        /// <summary>Any ranged crits on the target</summary>
        RangedCrit,
        #endregion
        #region Single-Weapon Hits
        /// <summary>Hit or Crit with just the main-hand, useful for dual-wielding weapon procs/enchants</summary>
        MainHandHit,
        /// <summary>Hit or Crit with just the off-hand, useful for dual-wielding weapon procs/enchants</summary>
        OffHandHit,
        /// <summary>Hit or Crit with the hand that has the effect, useful for dual-weilding weapon procs/enchants that are specific to the weapon.</summary>
        CurrentHandHit,
        #endregion

        #region Damage, in or out
        /// <summary>Any damage taken, regardless of type</summary>
		DamageTaken,
        /// <summary>The player blocks, parries or dodges a melee attack</summary>
        DamageAvoided,
        /// <summary>The player deals any form of damage, regardless of where it comes from</summary>
        DamageDone,
        /// <summary>The tick of a player's DoT on the target</summary>
        DoTTick,
        /// <summary>Damage taken via magical means.</summary>
        DamageTakenMagical,
        /// <summary>Damage taken via physical/melee means.</summary>
        DamageTakenPhysical,
        #endregion

        /// <summary>The player deals any form of damage, regardless of where it comes from or performs a heal (on self or other).</summary>
        DamageOrHealingDone,

        // Class Specific
        #region Added by Death Knight
        BloodStrikeHit,
        HeartStrikeHit,
        ObliterateHit,
        DeathStrikeHit,
        ScourgeStrikeHit,
        BloodStrikeOrHeartStrikeHit, // Deprecated.
        PlagueStrikeHit,
        IcyTouchHit,
        RuneStrikeHit,
        /// <summary>When FrostFever is Applied - not when it TICKs</summary>
        FrostFeverHit,
        #endregion
        #region Added by Druid
        InsectSwarmTick,
        MoonfireCast,
        MoonfireTick,
        InsectSwarmOrMoonfireTick,
        RejuvenationTick,
        MangleCatHit,
        MangleBearHit,
		SwipeBearOrLacerateHit,
		MangleCatOrShredHit,
		MangleCatOrShredOrInfectedWoundsHit,
		RakeTick,
		LacerateTick,
        #endregion
        #region Added by Hunter
        /// <summary>The Hunter's Auto Shot lands on the target (hit or crit)</summary>
        HunterAutoShotHit,
        /// <summary>The Hunter's Steady Shot ability lands on the target (hit or crit)</summary>
        SteadyShotHit,
        /// <summary>The Hunter Pet's normal Damage ability (Claw, Bite or Smack) crits on the target</summary>
        PetClawBiteSmackCrit,
        /// <summary>The Hunter's Serpent Sting and Wyvern Sting abilities deal damage</summary>
        SerpentWyvernStingsDoDamage,
        #endregion
        #region Added by Mage
        ManaGem,
        MageNukeCast,
        #endregion
        #region Added by Paladin
        JudgementHit,
        HolyShield,
        HammeroftheRighteous,
        ShieldofRighteousness,
        DivinePlea,
        CrusaderStrikeHit,
        SealOfVengeanceTick,
        HolyLightCast,
        HolyShockCast,
        #endregion
        #region Added by Shaman
        ShamanLightningBolt,
        ShamanLavaLash,
        ShamanShock,
        ShamanFlameShockDoTTick,
        ShamanStormStrike,
        ShamanShamanisticRage,
        #endregion
        #region Added by Warrior
        /// <summary>The Warrior uses a Heroic Strike or Slam and it lands on the target (hit or crit)</summary>
        HSorSLHit,
        /// <summary>The Warrior's Deep Wounds ability ticks on the target</summary>
        DeepWoundsTick,
        #endregion
        #region Added by Warlock
        /// <summary>The Warlock's Corruption DoT ticks</summary>
        CorruptionTick,
        #endregion
    }

    public partial class SpecialEffect
    {
//        public static int numSpecEffectsCreatedEver = 0;

        public Trigger Trigger { get; set; }
        public Stats Stats { get; set; }
        public float Duration { get; set; }
        public float Cooldown { get; set; }
        public float Chance { get; set; }
        public int MaxStack { get; set; }

        private class UptimeInterpolator : Interpolator
        {
            protected SpecialEffect effect;

            public UptimeInterpolator(SpecialEffect effect, float fightDuration) : base(fightDuration, true) 
            {
                this.effect = effect;
            }

            protected override float Evaluate(float procChance, float interval)
            {
                /*if (fightDuration <= effect.Duration)
                {
                    double n = fightDuration / interval;
                    double p = procChance;
                    return (float)((n - (1 - p) / p * (1 - Math.Pow(1 - p, n))) / n);
                }
                else if (fightDuration <= effect.Cooldown)
                {
                    double D = effect.Duration / interval;
                    double n = fightDuration / interval - D;
                    double p = procChance;
                    return (float)((D - Math.Pow(1 - p, n) * (1 - p) / p * (1 - Math.Pow(1 - p, D))) / (fightDuration / interval));
                }
                else
                {
                    // X : number of intervals under proc effect
                    // Pr(X >= r*d + i) = Pr(NegBin(r,p) <= n - r*c - i) = I_p(r, n - r*c - i)
                    // E(X) = sum_i i * Pr(X == i) = sum_i i * (Pr(X >= i) - Pr(X >= i+1))
                    //      = sum_i Pr(X >= i) = sum_r sum_i=1..d I_p(r, n - r*c - i)
                    double d = effect.Duration / interval;
                    double n = fightDuration / interval;
                    double p = procChance;

                    double c = effect.Cooldown / interval;
                    if (discretizationCorrection)
                    {
                        c += 0.5;
                    }
                    if (c < 1.0) c = 1.0;
                    double x = n;

                    double averageUptime = 0.0;
                    int r = 1;
                    while (x > 0)
                    {
                        averageUptime += SpecialFunction.Ibeta(r, x, p) * Math.Min(d, x);
                        // TODO: consider replacing with sum/integral over d
                        r++;
                        x -= c;
                    }
                    return (float)(averageUptime / n);
                }*/

                double d = effect.Duration / interval;
                double d2 = d * 0.5;
                double n = fightDuration / interval;
                double p = procChance;

                double c = effect.Cooldown / interval;
                if (discretizationCorrection)
                {
                    c += 0.5;
                }
                if (c < 1.0) c = 1.0;
                double x = n;

                const double w1 = 5.0 / 9.0;
                const double w2 = 8.0 / 9.0;
                const double k = 0.77459666924148337703585307995648;  //Math.Sqrt(3.0 / 5.0);
                double dx = k * d2;

                double averageUptime = 0.0;
                int r = 1;
                while (x > 0)
                {
                    // integrate_t=(x-duration)..x Ibeta(r, t, p) dt
                    if (x - d > 0)
                    {
                        double tmid = x - d2;
                        averageUptime += (w1 * SpecialFunction.Ibeta(r, tmid - dx, p) + w2 * SpecialFunction.Ibeta(r, tmid, p) + w1 * SpecialFunction.Ibeta(r, tmid + dx, p)) * d2;
                    }
                    else //if (x > 0)
                    {
                        double tmid = x * 0.5;
                        double dt = k * tmid;
                        averageUptime += (w1 * SpecialFunction.Ibeta(r, tmid - dt, p) + w2 * SpecialFunction.Ibeta(r, tmid, p) + w1 * SpecialFunction.Ibeta(r, tmid + dt, p)) * tmid;
                    }
                    r++;
                    x -= c;
                }
                return (float)(averageUptime / n);
            }
        }

        private class ProcsPerSecondInterpolator : Interpolator
        {
            protected SpecialEffect effect;

            public ProcsPerSecondInterpolator(SpecialEffect effect, float fightDuration) : base(fightDuration, true) 
            {
                this.effect = effect;
            }

            protected override float Evaluate(float procChance, float interval)
            {
                double c = effect.Cooldown / interval;
                if (discretizationCorrection)
                {
                    c += 0.5;
                }
                if (c < 1.0) c = 1.0;
                double n = fightDuration / interval;
                double x = n;
                double p = procChance;

                double averageProcs = 0.0;
                int r = 1;
                while (x > 0)
                {
                    averageProcs += SpecialFunction.Ibeta(r, x, p);
                    r++;
                    x -= c;
                }
                return (float)(averageProcs / fightDuration);
            }
        }

        private Dictionary<float, Interpolator> interpolator = new Dictionary<float, Interpolator>();

        public enum CalculationMode
        {
            Simple,
            Advanced,
            Interpolation
        }

        private static CalculationMode Mode = CalculationMode.Interpolation;

        static SpecialEffect()
        {
            UpdateCalculationMode();
        }

        public static void UpdateCalculationMode()
        {
            switch (Properties.GeneralSettings.Default.ProcEffectMode)
            {
                case 0:
                    Mode = CalculationMode.Simple;
                    break;
                case 1:
                    Mode = CalculationMode.Advanced;
                    SpecialFunction.MACHEP = 5.9604645E-8;
                    break;
                case 2:
                    Mode = CalculationMode.Advanced;
                    SpecialFunction.MACHEP = 1.11022302462515654042E-16;
                    break;
                case 3:
                    Mode = CalculationMode.Interpolation;
                    SpecialFunction.MACHEP = 1.11022302462515654042E-16;
                    break;
            }
        }

        public SpecialEffect()
        {
            Chance = 1.0f;
            MaxStack = 1;
            //Interlocked.Increment(ref numSpecEffectsCreatedEver);
        }

        public SpecialEffect(Trigger trigger, Stats stats, float duration, float cooldown)
        {
            Chance = 1.0f;
            MaxStack = 1;
            Trigger = trigger;
            Duration = duration;
            Cooldown = cooldown;
            Stats = stats;
            //Interlocked.Increment(ref numSpecEffectsCreatedEver);
        }

        public SpecialEffect(Trigger trigger, Stats stats, float duration, float cooldown, float chance)
        {
            Chance = chance;
            MaxStack = 1;
            Trigger = trigger;
            Duration = duration;
            Cooldown = cooldown;
            Stats = stats;
            //Interlocked.Increment(ref numSpecEffectsCreatedEver);
        }

        public SpecialEffect(Trigger trigger, Stats stats, float duration, float cooldown, float chance, int maxStack)
        {
            Chance = chance;
            MaxStack = maxStack;
            Trigger = trigger;
            Duration = duration;
            Cooldown = cooldown;
            Stats = stats;
            //Interlocked.Increment(ref numSpecEffectsCreatedEver);
        }

        /// <summary>
        /// Computes average stats given the frequency of triggers.
        /// </summary>
        /// <param name="stats">Stats object into which the average stats will be accumulated.</param>
        public void AccumulateAverageStats(Stats stats)
        {
            AccumulateAverageStats(stats, 0.0f);
        }

        /// <summary>
        /// Computes average stats given the frequency of triggers.
        /// </summary>
        public Stats GetAverageStats()
        {
            return GetAverageStats(0.0f);
        }

        /// <summary>
        /// Computes average stats given the frequency of triggers.
        /// </summary>
        /// <param name="stats">Stats object into which the average stats will be accumulated.</param>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        public void AccumulateAverageStats(Stats stats, float triggerInterval)
        {
            AccumulateAverageStats(stats, triggerInterval, 1.0f, 3.0f, 0.0f);
        }

        /// <summary>
        /// Computes average stats given the frequency of triggers.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        public Stats GetAverageStats(float triggerInterval)
        {
            return GetAverageStats(triggerInterval, 1.0f, 3.0f, 0.0f);
        }

        /// <summary>
        /// Computes average stats given the frequency of triggers.
        /// </summary>
        /// <param name="stats">Stats object into which the average stats will be accumulated.</param>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between hits and set
        /// triggerChance to crit chance)</param>
        public void AccumulateAverageStats(Stats stats, float triggerInterval, float triggerChance)
        {
            AccumulateAverageStats(stats, triggerInterval, triggerChance, 3.0f, 0.0f); ;
        }

        /// <summary>
        /// Computes average stats given the frequency of triggers.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between hits and set
        /// triggerChance to crit chance)</param>
        public Stats GetAverageStats(float triggerInterval, float triggerChance)
        {
            return GetAverageStats(triggerInterval, triggerChance, 3.0f, 0.0f); ;
        }

        /// <summary>
        /// Computes average stats given the frequency of triggers.
        /// </summary>
        /// <param name="stats">Stats object into which the average stats will be accumulated.</param>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between hits and set
        /// triggerChance to crit chance)</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        public void AccumulateAverageStats(Stats stats, float triggerInterval, float triggerChance, float attackSpeed)
        {
            AccumulateAverageStats(stats, triggerInterval, triggerChance, attackSpeed, 0.0f); ;
        }

        /// <summary>
        /// Computes average stats given the frequency of triggers.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between hits and set
        /// triggerChance to crit chance)</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        public Stats GetAverageStats(float triggerInterval, float triggerChance, float attackSpeed)
        {
            return GetAverageStats(triggerInterval, triggerChance, attackSpeed, 0.0f); ;
        }

        /// <summary>
        /// Computes average stats given the frequency of triggers.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between hits and set
        /// triggerChance to crit chance)</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="fightDuration">Duration of fight in seconds.</param>
        public Stats GetAverageStats(float triggerInterval, float triggerChance, float attackSpeed, float fightDuration)
        {
            Stats stats = new Stats();
            AccumulateAverageStats(stats, triggerInterval, triggerChance, attackSpeed, fightDuration);
            return stats;
        }

        /// <summary>
        /// Computes average stats given the frequency of triggers.
        /// </summary>
        /// <param name="stats">Stats object into which the average stats will be accumulated.</param>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between hits and set
        /// triggerChance to crit chance)</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="fightDuration">Duration of fight in seconds.</param>
        public void AccumulateAverageStats(Stats stats, float triggerInterval, float triggerChance, float attackSpeed, float fightDuration)
        {
            Stats.GenerateSparseData();
            if (MaxStack > 1)
            {
                stats.Accumulate(Stats, GetAverageStackSize(triggerInterval, triggerChance, attackSpeed, fightDuration));
            }
            else if (Duration == 0f)
            {
                stats.Accumulate(Stats, GetAverageProcsPerSecond(triggerInterval, triggerChance, attackSpeed, fightDuration));
            }
            else
            {
                stats.Accumulate(Stats, GetAverageUptime(triggerInterval, triggerChance, attackSpeed, fightDuration));
            }
        }

        /// <summary>
        /// Computes average scaled stats given the frequency of triggers; also computers average effect
        /// </summary>
        /// <param name="stats">Stats object into which the average stats will be accumulated.</param>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between hits and set
        /// triggerChance to crit chance)</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="fightDuration">Duration of fight in seconds.</param>
        /// <param name="scale">Scale value of effect, used for secondary effects</param>
        public float AccumulateAverageStats(Stats stats, float triggerInterval, float triggerChance, float attackSpeed, float fightDuration, float scale)
        {
            Stats.GenerateSparseData();
            float factor;
            if (MaxStack > 1)
            {
                factor = GetAverageStackSize(triggerInterval, triggerChance, attackSpeed, fightDuration);
            }
            else if (Duration == 0f)
            {
                factor = GetAverageProcsPerSecond(triggerInterval, triggerChance, attackSpeed, fightDuration);
            }
            else
            {
                factor = GetAverageUptime(triggerInterval, triggerChance, attackSpeed, fightDuration);
            }
            factor *= scale;
            stats.Accumulate(Stats, factor);
            return factor;
        }

        /// <summary>
        /// Computes average factor given the frequency of triggers
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between hits and set
        /// triggerChance to crit chance)</param>
        /// <returns></returns>
        public float GetAverageFactor(float triggerInterval, float triggerChance)
        {
            return GetAverageFactor(triggerInterval, triggerChance, 3.0f, 0.0f, 1.0f);
        }

        /// <summary>
        /// Computes average factor given the frequency of triggers
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between hits and set
        /// triggerChance to crit chance)</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="fightDuration">Duration of fight in seconds.</param>
        /// <returns></returns>
        public float GetAverageFactor(float triggerInterval, float triggerChance, float attackSpeed, float fightDuration)
        {
            return GetAverageFactor(triggerInterval, triggerChance, attackSpeed, fightDuration, 1.0f);
        }

        /// <summary>
        /// Computes average factor given the frequency of triggers
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between hits and set
        /// triggerChance to crit chance)</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="fightDuration">Duration of fight in seconds.</param>
        /// <param name="scale">Scale value of effect, used for secondary effects</param>
        /// <returns></returns>
        public float GetAverageFactor(float triggerInterval, float triggerChance, float attackSpeed, float fightDuration, float scale)
        {
            float factor;
            if (MaxStack > 1)
            {
                factor = GetAverageStackSize(triggerInterval, triggerChance, attackSpeed, fightDuration);
            }
            else if (Duration == 0f)
            {
                factor = GetAverageProcsPerSecond(triggerInterval, triggerChance, attackSpeed, fightDuration);
            }
            else
            {
                factor = GetAverageUptime(triggerInterval, triggerChance, attackSpeed, fightDuration);
            }
            factor *= scale;
            return factor;
        }

        /// <summary>
        /// Computes average stack size given the frequency of triggers.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between hits and set
        /// triggerChance to crit chance)</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="fightDuration">Duration of fight in seconds.</param>
        public float GetAverageStackSize(float triggerInterval, float triggerChance, float attackSpeed, float fightDuration)
        {
            return GetAverageStackSize(triggerInterval, triggerChance, attackSpeed, fightDuration, 1);
        }

        /// <summary>
        /// Computes average stack size given the frequency of triggers.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between hits and set
        /// triggerChance to crit chance)</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="fightDuration">Duration of fight in seconds.</param>
        /// <param name="stackReset">Number of times the stack resets.</param>
        public float GetAverageStackSize(float triggerInterval, float triggerChance, float attackSpeed, float fightDuration, int stackReset)
        {
            float averageStack = 0;
            if ((MaxStack > 1) && (Cooldown == 0f))
            {
                float p = triggerChance * GetChance(attackSpeed);
                float q = 1.0f - p;
                float Q = (float)Math.Pow(q, Duration / triggerInterval);
                float probToStack = 1.0f - Q;                

                if (probToStack > 0.99f)
                {
                    // Simplified handling for stacking procs
                    // very high chance to stack, treat as remaining at max stacks after buildup
                    if (fightDuration == 0.0f)
                        averageStack = MaxStack;        // Infinite time, it will stack to Max
                    else
                    {
                        // For now, assume it stacks to max, if it can stack at all
                        float buildTime = triggerInterval / triggerChance * MaxStack * stackReset;
                        float value;
                        if (fightDuration >= buildTime)
                        {
                            value = buildTime * (MaxStack - 1) / 2 + (fightDuration - buildTime) * MaxStack;
                        }
                        else
                        {
                            float singleBuild = buildTime / stackReset;
                            int fullBuilds = (int)(fightDuration / singleBuild);
                            value = fullBuilds * singleBuild * (MaxStack - 1) / 2;
                            float remaining = fightDuration - fullBuilds * singleBuild;
                            float remainingStacks = remaining / (triggerInterval / triggerChance);
                            value += remaining * Math.Max(0, remainingStacks - 1) / 2;
                        }

                        averageStack = value / fightDuration;
                    }
                }
                else if (probToStack > 0)
                {
                    // nontrivial chance to drop stacks, treat as stationary distribution
                    // for now we don't have a nonstationary fixed duration solution

                    //Sij: i stacks, j ticks left

                    //S00:
                    //=> S1T p
                    //=> S00 1-p

                    //S1j:
                    //=> S2T p
                    //=> S1(j-1) 1-p

                    //S11:
                    //=> S2T p
                    //=> S00 1-p

                    //SMj:
                    //=> SMT p
                    //=> SM(j-1) 1-p

                    //SM1:
                    //=> SMT p
                    //=> S00 1-p

                    //S00 = q * (S00 + S11 + S21 + .. + SM1)

                    //S11 = q * S12
                    //S12 = q * S13
                    //..
                    //S1(T-1) = q * S1T

                    //=>

                    //S11 = q ^ (T-1) * S1T

                    //S1T = p * S00

                    //=>

                    //S11 = p * q ^ (T-1) * S00



                    //S21 = q * S22
                    //S22 = q * S23
                    //..
                    //S2(T-1) = q * S2T

                    //=>

                    //S21 = q ^ (T-1) * S2T

                    //S2T = p * (S11 + S12 + .. + S1T)

                    //S1j = p * q ^ (T-j) * S00

                    //S11 + S12 + .. + S1T = p * S00 * (q ^ (T-1) + q ^ (T-2) + ... + 1) = S00 * (1 - q ^ T)

                    //S2T = p * (1 - q ^ T) * S00

                    //S2j = q ^ (T-j) * S2T




                    //Sij = q ^ (T-j) * SiT

                    //Si1 + Si2 + .. + SiT = SiT * (q ^ (T-1) + q ^ (T-2) + ... + 1) = SiT * (1 - q ^ T) / p




                    //S31 = q * S32
                    //S32 = q * S33
                    //..
                    //S3(T-1) = q * S3T

                    //S31 = q ^ (T-1) * S3T

                    //S3T = p * (S21 + S22 + .. + S2T) = S2T * (1 - q ^ T)



                    //SMT = p * (S(M-1)1 + .. + S(M-1)T) + p * (SM1 + .. + SMT)
                    //    = S(M-1)T * (1 - q ^ T) + SMT * (1 - q ^ T)
                    //    = S00 * p * (1 - q ^ T) ^ (M-1) + SMT * (1 - q ^ T)

                    //Q := q ^ T
                    //P := 1 - Q

                    //SMT = S00 * p * P ^ (M-1) + SMT * P
                    //SMT = S00 * p * P ^ (M-1) / Q
                    //SMT = S00 * p * P ^ (M-1)*(1/Q+1-1)
                    //SMT = S00 * p * (P ^ (M-1) + P ^ (M-1)*(1/Q - Q/Q))
                    //SMT = S00 * p * (P ^ (M-1) + P ^ M / Q)

                    //S00 + (S11 + .. + S1T) + .. + (SM1 + .. + SMT) = 
                    //S00 + S1T * (1 - q ^ T) / p + S2T * (1 - q ^ T) / p + .. + SMT * (1 - q ^ T) / p = 
                    //S00 + (S1T + S2T + .. + SMT) * P / p =
                    //S00 + S00 * (1 + P + P^2 + .. + P^(M-1)/Q) * P = 
                    //S00 + S00 * (1 + P + P^2 + .. + P^(M-1) + P^M / Q) * P = 
                    //S00 + S00 * (1 - P ^ M + P ^ M) * P / Q =
                    //S00 + S00 * P / Q =
                    //S00 * (1 + P / Q) =
                    //S00 / Q == 1

                    //=>

                    //S00 = Q

                    //average stack size = 0 * S00 + 1 * (S11 + .. + S1T) + 2 * (S21 + .. + S2T) + .. + M * (SM1 + .. + SMT) =

                    //S1T * (1 - q ^ T) / p + 2 * S2T * (1 - q ^ T) / p + .. + M * SMT * (1 - q ^ T) / p =
                    //S00 * (P + 2 * P^2 + 3 * P^3 + .. + M * P^M + M * P^(M+1)/Q) =
                    //S00 * (((M*P - M - 1) * P^(M+1) + P)/Q^2 + M * P^(M+1)/Q) =
                    //S00 * ((-M*Q - 1+M*Q) * P^(M+1) + P) / Q^2 =
                    //S00 * (-P^(M+1) + P) / Q^2 =
                    //S00 * P * (1 - P^M) / Q^2 =
                    //P/Q * (1 - P^M)

                    // P == probToStack

                    float PM = (float)Math.Pow(probToStack, MaxStack);

                    averageStack = probToStack / Q * (1.0f - (float)Math.Pow(probToStack, MaxStack));
                }
                else
                {
                    // Handle it like single stack only for now
                    // Since no cooldown, assume it isn't stacking because Duration <  ( triggerInterval / triggerChance )
                    averageStack = Duration / (triggerInterval / triggerChance);       // Average Uptime
                }
                return averageStack;
            }
            else
                return GetAverageUptime(triggerInterval, triggerChance, attackSpeed, fightDuration);
        }

        /// <summary>
        /// Computes average uptime of the effect given the frequency of triggers.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between spell ticks and set
        /// triggerChance to crit chance)</param>
        public float GetAverageUptime(float triggerInterval, float triggerChance)
        {
            return GetAverageUptime(triggerInterval, triggerChance, 3f, 0f);
        }

        /// <summary>
        /// Computes average uptime of the effect given the frequency of triggers.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between spell ticks and set
        /// triggerChance to crit chance)</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        public float GetAverageUptime(float triggerInterval, float triggerChance, float attackSpeed)
        {
            return GetAverageUptime(triggerInterval, triggerChance, attackSpeed, 0f);
        }

        /// <summary>
        /// Computes average uptime of (atleast 1 stack of) the effect given the frequency of triggers.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between spell ticks and set
        /// triggerChance to crit chance)</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="fightDuration">Duration of fight in seconds.</param>
        public float GetAverageUptime(float triggerInterval, float triggerChance, float attackSpeed, float fightDuration)
        {
            bool discretizationCorrection = true;
            if (triggerChance == 0f || float.IsPositiveInfinity(triggerInterval))
            {
                return 0f;
            }

            if (Cooldown > Duration)
            {
                // fight duration is used only in this case, for the cooldown < duration case the other approximations are good enough
                if (fightDuration == 0f)
                {
                    if (discretizationCorrection)
                    {
                        return Duration / (Cooldown - triggerInterval / 2 + triggerInterval / triggerChance / GetChance(attackSpeed));
                    }
                    else
                    {
                        return Duration / (Cooldown - triggerInterval + triggerInterval / triggerChance / GetChance(attackSpeed));
                    }
                }
                else
                {
                    // activeTime(T) := average time the effect is up given we are out of cooldown and there is T time left
                    // activeTime(T) = 0 if T <= 0
                    // activeTime(T) = chance * (min(duration, T) + activeTime(T - cooldown)) + (1 - chance) * activeTime(T - interval)
                    // activeTime[T] = chance * (min(D, T) + activeTime[T - C]) + (1 - chance) * activeTime[T - 1]

                    // activeTime[0] = 0
                    // activeTime[1] = p
                    // activeTime[2] = p * 2 + (1 - p) * activeTime[1]
                    // activeTime[3] = p * 3 + (1 - p) * activeTime[2]
                    // ...
                    // activeTime[D] = p * D + (1 - p) * activeTime[D - 1]

                    // H[0] = 0
                    // H[n] = n * p + (1 - p) * H[n - 1]
                    // H[n] = 1 - 1/p + n + (1 - p)^n * (1/p - 1)
                    //      = n + (1 - p)/p * ((1 - p)^n - 1)
                    // activeTime[n] = n - (1 - p)/p * (1 - (1 - p)^n)
                    // activeTime[D] = D - (1 - p)/p * (1 - (1 - p)^D)

                    // activeTime[D + 1] = p * D + (1 - p) * activeTime[D]
                    // activeTime[D + 2] = p * D + (1 - p) * (p * D + (1 - p) * activeTime[D])
                    //                   = p * D + p * (1 - p) * D + (1 - p) ^ 2 * activeTime[D]
                    //                   = p * (2 - p) * D + (1 - p) ^ 2 * activeTime[D]
                    // activeTime[D + 3] = p * D + (1 - p) * [p * (2 - p) * D + (1 - p) ^ 2 * activeTime[D]]
                    //                   = p * D * (1 + (1 - p) * (2 - p)) + (1 - p) ^ 3 * activeTime[D]
                    // activeTime[D + 4] = p * D * (1 + (1 - p) * (1 + (1 - p) * (2 - p))) + (1 - p) ^ 4 * activeTime[D]

                    // K[1] = 1
                    // K[n] = 1 + (1 - p) * K[n - 1]
                    // K[n] = 1/p + (1-p)^(n-1) * (p - 1) / p
                    //      = (1 - (1-p) ^ n) / p

                    // activeTime[D + n] = p * D * (1 - (1-p) ^ n) / p + (1 - p) ^ n * activeTime[D]
                    //                   = D + (1 - p) ^ n * (activeTime[D] - D)
                    //                   = D - (1 - p) ^ n * (1 - p)/p * (1 - (1 - p)^D)

                    // activeTime[C] = activeTime[D + C - D] = D + (1 - p) ^ (C - D) * (activeTime[D] - D)
                    //               = D - (1 - p) ^ (C - D) * (1 - p)/p * (1 - (1 - p)^D)

                    // activeTime[C + 1] = p * (D + activeTime[1]) + (1 - p) * activeTime[C]
                    //                   = p * D + p * activeTime[1] + (1 - p) * activeTime[C]
                    // activeTime[C + 2] = p * D + p * activeTime[2] + (1 - p) * (p * D + p * activeTime[1] + (1 - p) * activeTime[C])
                    //                   = p * D * K[2] + p * activeTime[2] + (1 - p) * p * activeTime[1] + (1 - p) ^ 2 * activeTime[C]

                    // activeTime[2] - p * 2 = (1 - p) * activeTime[1]
                    // activeTime[C + 2] = p * D * K[2] + p * activeTime[2] + p * (activeTime[2] - p * 2) + (1 - p) ^ 2 * activeTime[C]
                    //                   = p * D * K[2] + 2 * p * activeTime[2] - 2 * p ^ 2 + (1 - p) ^ 2 * activeTime[C]
                    // activeTime[C + 3] = p * D * K[3] + p * activeTime[3] + (1 - p) * (2 * p * activeTime[2] - 2 * p ^ 2 + (1 - p) ^ 2 * activeTime[C])
                    //                   = p * D * K[3] + p * activeTime[3] + 2 * p * (activeTime[3] - 3 * p) - 2 * p ^ 2 * (1 - p) + (1 - p) ^ 3 * activeTime[C]
                    //                   = p * D * K[3] + 3 * p * activeTime[3] - 3 * 2 * p ^ 2 - 2 * p ^ 2 * (1 - p) + (1 - p) ^ 3 * activeTime[C]
                    //                   = p * D * K[3] + 3 * p * activeTime[3] - 2 * p ^ 2 * (4 - p) + (1 - p) ^ 3 * activeTime[C]

                    // activeTime[C + 1] = p * D + p * p + (1 - p) * activeTime[C]
                    // activeTime[C + 2] = p * D * K[2] + 2 * (p * (2 - p) - (1 - p) * (1 - (1 - p)^2)) + (1 - p) ^ 2 * activeTime[C]

                    // activeTime[C + n] = p * D * K[n] + p * H[n] + (1 - p) * (p * H[n - 1] + (1 - p) * (p * H[n - 2] + ...))
                    // p * H[n] = p * (n + (1 - p)/p * ((1 - p)^n - 1)) = p * n - (1 - p) * (1 - (1 - p)^n)
                    // sum_i=0..n p * (1 - p)^(i - 1) * H[i]
                    // p * (1 - p) ^ (i - 1) * H[i] = p * i * (1 - p) ^ (i - 1) - (1 - p) ^ i * (1 - (1 - p)^i)
                    // sum_i=0..n p * (1 - p)^(i - 1) * H[i] = p * n * (1 - p) ^ (n - 1) - (1 - p) ^ n * (1 - (1 - p)^n)    +    p * (n - 1) * (1 - p) ^ (n - 2) - (1 - p) ^ (n - 1) * (1 - (1 - p)^(n - 1)) + ...
                    // - (1 - p) ^ n * (1 - (1 - p)^n) + (1 - p) ^ (n - 1) * (p * n - 1 + (1 - p)^(n - 1)))

                    // activeTime[C + 4] = p * D * K[4] + p * activeTime[4] + (1 - p) * (3 * p * activeTime[3] - 2 * p ^ 2 * (4 - p)) + (1 - p) ^ 4 * activeTime[C]
                    // p * activeTime[4] + (1 - p) * (3 * p * activeTime[3] - 2 * p ^ 2 * (4 - p)) = 
                    // p * activeTime[4] + 3 * p * (1 - p) * activeTime[3] - 2 * p ^ 2 * (4 - p) * (1 - p) =
                    // p * activeTime[4] + 3 * p * (activeTime[4] - 4 * p) - 2 * p ^ 2 * (4 - p) * (1 - p) =
                    // 4 * p * activeTime[4] - p ^ 2 * (3 * 4 + (1 - p) * (3 * 2 + (1 - p) * (2 * 1 + (1 - p) * 1 * 0)))

                    // R[0] = 0
                    // R[n] = n * (n - 1) + (1 - p) * R[n - 1]
                    // R[n] = 2*p^(-2) * ((1 - p)^n - 1 - n)  +  2*p^(-3) * (1 - (1-p)^n)  +  n*(n + 1)/p
                    //      = [p^2*n*(n + 1) + 2*p*((1 - p)^n - 1 - n) + 2* (1 - (1-p)^n)] / p^3
                    //      = [p^2*n*(n + 1) - 2*p*n - 2*p*(1 - (1 - p)^n) + 2* (1 - (1-p)^n)] / p^3
                    //      = [p*n*(p*n + p - 2) + 2*(1 - (1-p)^n) * (1 - p)] / p^3

                    // S[n] := n * p * H[n] - p ^ 2 * R[n] =
                    // n * (p * n - (1 - p) * (1 - (1 - p)^n)) - [n*(p*n + p - 2) + 2/p*(1 - (1-p)^n) * (1 - p)]
                    // p * n^2 - n*(p*n + p - 2) - (2/p * (1 - p) + n * (1 - p)) * (1 - (1 - p)^n)
                    // n * (2 - p) - (2/p + n) * (1 - p) * (1 - (1 - p)^n)

                    // activeTime[C + n] = p * D * K[n] + n * (2 - p) - (2/p + n) * (1 - p) * (1 - (1 - p)^n) + (1 - p) ^ n * activeTime[C]
                    //                   = (D - (2/p + n) * (1 - p)) * (1 - (1-p) ^ n) + n * (2 - p) + (1 - p) ^ n * activeTime[C]
                    //                   = 
                    // activeTime[C + n] = p * D * K[n] + S[n] + (1 - p) ^ n * activeTime[C]

                    // activeTime[k*C+n] = p * (D + activeTime[(k-1)*C+n]) + (1 - p) * activeTime[k*C+n-1]

                    // p * T * D / (p * C + 1)

                    // activeTime[2*C+n] = p * (D + activeTime[C+n]) + (1 - p) * activeTime[2*C+n-1]
                    //                   = p * D + p * (p * D * K[n] + S[n] + (1 - p) ^ n * activeTime[C])


                    // advanced calcs based on findings from average procs

                    // Uptime(x) = sum_r=0..inf probability that (r+1)st proc happens between time x-duration and x
                    // Uptime(x) = sum_r=0..inf Pr(NegBin(p,r+1) <= x - r * cooldown - 1) - Pr(NegBin(p,r+1) <= x - duration - r * cooldown - 1)
                    // Uptime(x) = sum_r=0..inf Ibeta(r+1, x - r * cooldown, p) - Ibeta(r+1, x - duration - r * cooldown, p)

                    // AvgTotalUptime(x) = integrate_t=0..x Uptime(t) dt
                    // AvgTotalUptime(x) = integrate_t=0..x sum_r=0..inf Ibeta(r+1, t - r * cooldown, p) - Ibeta(r+1, t - duration - r * cooldown, p) dt
                    // AvgTotalUptime(x) = sum_r=0..inf [integrate_t=0..x Ibeta(r+1, t - r * cooldown, p) - Ibeta(r+1, t - duration - r * cooldown, p)] dt
                    // AvgTotalUptime(x) = sum_r=0..inf [integrate_w=0..(x-r * cooldown) Ibeta(r+1, w, p) dw - integrate_t=0..(x-duration - r * cooldown) Ibeta(r+1, w, p) dw]
                    // AvgTotalUptime(x) = sum_r=0..inf integrate_w=(x-duration - r * cooldown)..(x-r * cooldown) Ibeta(r+1, w, p) dw
                    // AvgTotalUptime(x) = sum_r=0..inf integrate_t=(x-duration)..x Ibeta(r+1, t - r*cooldown, p) dt

                    bool needsBeta = fightDuration < 10 * Cooldown;
                    if (triggerInterval > 0 && Mode == CalculationMode.Advanced && needsBeta)
                    {
                        // old calcs based on approximation from procs per second
                        /*if (fightDuration <= Duration)
                        {
                            double n = fightDuration / triggerInterval;
                            double p = triggerChance * GetChance(attackSpeed);
                            return (float)((n - (1 - p) / p * (1 - Math.Pow(1 - p, n))) / n);
                        }
                        else if (fightDuration <= Cooldown)
                        {
                            double D = Duration / triggerInterval;
                            double n = fightDuration / triggerInterval - D;
                            double p = triggerChance * GetChance(attackSpeed);
                            return (float)((D - Math.Pow(1 - p, n) * (1 - p) / p * (1 - Math.Pow(1 - p, D))) / (fightDuration / triggerInterval));
                        }
                        else
                        {
                            // X : number of intervals under proc effect
                            // Pr(X >= r*d + i) = Pr(NegBin(r,p) <= n - r*c - i) = I_p(r, n - r*c - i)
                            // E(X) = sum_i i * Pr(X == i) = sum_i i * (Pr(X >= i) - Pr(X >= i+1))
                            //      = sum_i Pr(X >= i) = sum_r sum_i=1..d I_p(r, n - r*c - i)
                            double d = Duration / triggerInterval;
                            double n = fightDuration / triggerInterval;
                            double p = triggerChance * GetChance(attackSpeed);

                            double c = Cooldown / triggerInterval;
                            if (discretizationCorrection)
                            {
                                c += 0.5;
                            }
                            if (c < 1.0) c = 1.0;
                            double x = n;

                            double averageUptime = 0.0;
                            int r = 1;
                            while (x > 0)
                            {
                                averageUptime += SpecialFunction.Ibeta(r, x, p) * Math.Min(d, x);
                                r++;
                                x -= c;
                            }
                            return (float)(averageUptime / n);
                        }*/

                        // new calcs based on gaussian integrator

                        double d = Duration / triggerInterval;
                        double d2 = d * 0.5;
                        double n = fightDuration / triggerInterval;
                        double p = triggerChance * GetChance(attackSpeed);

                        double c = Cooldown / triggerInterval;
                        if (discretizationCorrection)
                        {
                            c += 0.5;
                        }
                        if (c < 1.0) c = 1.0;
                        double x = n;

                        const double w1 = 5.0 / 9.0;
                        const double w2 = 8.0 / 9.0;
                        const double k = 0.77459666924148337703585307995648;  //Math.Sqrt(3.0 / 5.0);
                        double dx = k * d2;

                        double averageUptime = 0.0;
                        int r = 1;
                        while (x > 0)
                        {
                            // integrate_t=(x-duration)..x Ibeta(r, t, p) dt
                            if (x - d > 0)
                            {
                                double tmid = x - d2;
                                averageUptime += (w1 * SpecialFunction.Ibeta(r, tmid - dx, p) + w2 * SpecialFunction.Ibeta(r, tmid, p) + w1 * SpecialFunction.Ibeta(r, tmid + dx, p)) * d2;
                            }
                            else //if (x > 0)
                            {
                                double tmid = x * 0.5;
                                double dt = k * tmid;
                                averageUptime += (w1 * SpecialFunction.Ibeta(r, tmid - dt, p) + w2 * SpecialFunction.Ibeta(r, tmid, p) + w1 * SpecialFunction.Ibeta(r, tmid + dt, p)) * tmid;
                            }
                            r++;
                            x -= c;
                        }
                        return (float)(averageUptime / n);
                    }
                    else if (triggerInterval > 0 && Mode == CalculationMode.Interpolation && needsBeta)
                    {
                        lock (interpolator)
                        {
                            Interpolator i;
                            if (!interpolator.TryGetValue(fightDuration, out i))
                            {
                                i = new UptimeInterpolator(this, fightDuration);
                                interpolator[fightDuration] = i;
                            }
                            return i[triggerChance * GetChance(attackSpeed), triggerInterval];
                        }
                    }
                    else if (triggerInterval == 0.0f)
                    {
                        // this is a special case, meaning that it basically auto triggers on cooldown
                        float t = fightDuration;
                        float cc = float.IsPositiveInfinity(Cooldown) ? fightDuration : Cooldown;
                        float total = (float)Math.Floor(t / cc) * Duration;
                        t -= (float)Math.Floor(t / cc) * cc;
                        total += Math.Min(t, Duration);
                        return total / fightDuration;
                    }
                    else
                    {
                        double d = Duration / triggerInterval;
                        double n = fightDuration / triggerInterval;
                        double p = triggerChance * GetChance(attackSpeed);

                        double c = Cooldown / triggerInterval;
                        if (discretizationCorrection)
                        {
                            c += 0.5;
                        }
                        if (c < 1.0) c = 1.0;
                        // (1+cooldown/fightDuration*(cooldown-1)/(2*K)) * 1/K
                        double K = 1.0 / p + c - 1.0;
                        double ppt = (1 + c / n * (c - 1) / (2 * K)) / K;
                        return (float)(ppt * d);
                    }
                }
            }
            else if (Cooldown == 0.0f)
            {
                if (triggerInterval >= Duration) return Duration / triggerInterval * GetChance(attackSpeed) * triggerChance;
                else return 1.0f - (float)Math.Pow(1f - triggerChance * GetChance(attackSpeed), Duration / triggerInterval);
            }
            else
            {
                //P(E0) = P(E0|S0)*P(S0) + P(E0|!S0) * (1 - P(S0))
                //=1 - (1 - P(S0)) + P(E0|!S0) * (1 - P(S0))
                //=1 + P(!S0) * (1 - P(E0|!S0))
                //=1 + (1 - P0) * (1 - P(E0|!S0))

                //P(E0|!S0) = P(S1|!S0) + P(E0|!S0,!S1)*P(!S1|!S0)

                //P(S1|!S0) = P(!S0|S1)*P(S1)/P(!S0) = P0 / (1 - P0)
                //P(!S1|!S0) = P(!S0|!S1)*P(!S1)/P(!S0) = P(!S0|!S1)

                //P(E0|!S0) = P0 / (1 - P0) + P(E0|!S0,!S1)*P(!S0|!S1)

                //P(E0|!S0,!S1) = P(S2|!S0,!S1) + P(E0|!S0,!S1,!S2)*P(!S2|!S0,!S1)

                //P(S2|!S0,!S1) = P(!S0,S1)/P(!S0,!S1) = (P(!S0) - P(!S0,!S1))/P(!S0,!S1)

                //P(!S0,!S1) = P(!S0|!S1) * P(!S1) = P(!S0|!S1) * (1 - P0)



                //P(S0|!S1,!S2,...,!SC) = PT
                //P(S0|Sn) = 0, 0 < n <= C

                //P(S0) = P(S0|S1)*P(S1) + P(S0|!S1)*P(!S1)
                //=P(S0|!S1)*(1 - P(S0))

                //P(S0) * (1 + P(S0|!S1)) = P(S0|!S1)
                //P(S0) = P(S0|!S1) / (1 + P(S0|!S1))

                //P(S0|!S1) = P(S0|!S1,S2)*P(S2|!S1) + P(S0|!S1,!S2)*P(!S2|!S1)
                //=P(S0|!S1,S2)*P(S1|!S0) + P(S0|!S1,!S2)*P(!S1|!S0)
                //=P(S0|!S1,!S2)*(1 - P(S0|!S1))

                //P(S0|!S1) * (1 + P(S0|!S1,!S2)) = P(S0|!S1,!S2)
                //P(S0|!S1) = P(S0|!S1,!S2) / (1 + P(S0|!S1,!S2))


                //P(S0|!S1,!S2) = P(S0|!S1,!S2,S3)*P(S3|!S1,!S2) + P(S0|!S1,!S2,!S3)*P(!S3|!S1,!S2)
                //=P(S0|!S1,!S2,!S3)*P(!S3|!S1,!S2) = P(S0|!S1,!S2,!S3) * (1 - P(S0|!S1,!S2))

                //P(!S3|!S1,!S2) = P(!S2|!S0,!S1) = P(!S0|!S2,!S1)*P(!S1,!S0) / P(!S0,!S1) = P(!S0|!S2,!S1) = 1 - P(S0|!S1,!S2)

                //P(S0|!S1,!S2) = P(S0|!S1,!S2,!S3) / (1 + P(S0|!S1,!S2,!S3))

                //PC = PT / (1 + PT)
                //PC_1 = PC / (1 + PC) = PT / (1 + PT) / ((1 + 2 * PT) / (1 + PT)) = PT / (1 + 2 * PT)
                //PC_2 = PT / (1 + 2 * PT) / ((1 + 3 * PT) / (1 + 2 * PT)) = PT / (1 + 3 * PT)


                //P0 = PT / (1 + C * PT)



                //P0 / (1 - P0) = PT / (1 + C * PT) / ((1 + (C - 1) * PT) / (1 + C * PT)) = PT / (1 + (C - 1) * PT) = P1

                //Pk = PT / (1 + (C - k) * PT)

                //P(E0) = 1 + (1 - P0) * (1 - P(E0|!S0))

                //P(E0|!S0) = P0 / (1 - P0) + P(E0|!S0,!S1)*P(!S0|!S1)
                //=P1 + P(E0|!S0,!S1)*(1 - P1)


                //P(E0|!S0,!S1) = P(S2|!S0,!S1) + P(E0|!S0,!S1,!S2)*P(!S2|!S0,!S1)


                //P(S2|!S0,!S1) = P(!S0|S2,!S1) * P(S2,!S1) / P(!S0,!S1) = P(S1,!S0) / P(!S0,!S1) = P(!S0|S1) * P(S1) / [P(!S0|!S1) * P(!S1)]
                //=P0 / [(1 - P(S0|!S1)) * (1 - P0) = P0 / [(1 - P1) * (1 - P0)]
                //=P1 / (1 - P1) = P2


                //P(E0|!S0,!S1) = P2 + P(E0|!S0,!S1,!S2)*(1 - P2)


                //P(E0|!S0,!S1,!S2,...,!SD-1) = 0


                //PED = 0

                //PED_1 = PD_1

                //PED_2 = PD_2 + PD_1 * (1 - PD_2) = PD_2 + PD_1 - PD_1 * PD_2 =
                //[PT * (1 + (C - D + 1) * PT) + PT * (1 + (C - D + 2) * PT) - PT * PT] / [(1 + (C - D + 2) * PT) * (1 + (C - D + 1) * PT)]
                //PT * 2 * [(1 + (C - D + 1) * PT)] / [(1 + (C - D + 2) * PT) * (1 + (C - D + 1) * PT)]
                //2 * PT / (1 + (C - D + 2) * PT) = 2 * PD_2
                //PED_3 = 3 * PD_3
                //PE0 = D * P0, if D < C

                //P(E0|!S0,!S1,...,!SC-1) = PC + P(E0|!S0,!S1,!S2,...,!SC)*(1 - PC)


                //P(E0|!S0,!S1,!S2,...,!SC) = P(E0|!S0-C,SC+1)*P(SC+1|!S0-C) + P(E0|!S0-C,!SC+1)*P(!SC+1|!S0-C)
                //=P(SC+1|!S0-C) + P(E0|!S0-C,!SC+1)*P(!SC+1|!S0-C)


                //P(SC+1|!S0-C) = P(!S0-C,SC+1) / P(!S0-C) = P(!S0,!S1-C,SC+1) / P(!S0-C) = P(!S0|!S1-C,SC+1) * P(!S0-C-1,SC) / P(!S0-C) = 
                //(1 - PT) * P(SC|!S0-C-1) * P(!S0-C-1) / [P(!SC|!S0-C-1) * P(!S0-C-1)] = 
                //(1 - PT) * P(SC|!S0-C-1) / P(!SC|!S0-C-1)
                //= (1 - PT) * PT / (1 - PT) = PT


                //P(E0|!S0,!S1,!S2,...,!SC) = PT + P(E0|!S0-C,!SC+1) * (1 - PT)


                //PED = 0
                //PED_1 = PT

                //PED_2 = PT + PED_1 * (1 - PT) = PT + PT * (1 - PT) = 2 * PT - PT * PT = 1 - (1 - 2 * PT + PT * PT) = 1 - (1 - PT) ^ 2

                //PED_3 = PT + PED_2 * (1 - PT) = PT + [1 - (1 - PT) ^ 2] * (1 - PT) =
                //PT + [(1 - PT) - (1 - PT) ^ 3] = 1 - (1 - PT) ^ 3


                //PEC = 1 - (1 - PT) ^ (D - C)


                //PEC_1 = PC-1 + PEC*(1 - PC-1)

                //PEC_2 = PC-2 + PEC_1*(1 - PC-2) = PC-2 + [PC-1 + PEC*(1 - PC-1)]*(1 - PC-2) = PC_2 + PC_1 - PC_1 * PC_2 + PEC*(1 - PC_1)*(1 - PC_2)
                //= 1 - (1 - PC_1)*(1 - PC_2) + PEC * (1 - PC_1)*(1 - PC_2) = 1 - (1 - PC_1)*(1 - PC_2) * (1 - PEC)


                //(1 - PC_1)*(1 - PC_2) = 1 - [PC_2 + PC_1 - PC_1 * PC_2] = 1 - 2 * PC_2

                //PEC_2 = 1 - (1 - 2 * PC_2) * (1 - PEC)

                //PE0 = 1 - (1 - C * P0) * (1 - PEC)
                //= 1 - (1 - C * PT / (1 + C * PT)) * (1 - PEC)
                //= 1 - 1 / (1 + C * PT) * (1 - PEC)

                //= 1 - 1 / (1 + C * PT) * (1 - PT) ^ (D - C)
                return 1.0f - 1.0f / (1.0f + Cooldown / triggerInterval * triggerChance * GetChance(attackSpeed)) * (float)Math.Pow(1f - triggerChance * GetChance(attackSpeed), (Duration - Cooldown) / triggerInterval);
            }

        }

        /// <summary>
        /// Computes chance that proc is up at specific time assuming uniform trigger grid.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between spell ticks and set
        /// triggerChance to crit chance)</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="time">Time in seconds since fight start.</param>
        /// <returns>Returns chance that proc is up at specific time.</returns>
        public float GetUptimePlot(float triggerInterval, float triggerChance, float attackSpeed, float time)
        {
            if (triggerChance == 0f || float.IsPositiveInfinity(triggerInterval))
            {
                return 0f;
            }

            const bool discretizationCorrection = true;

            if (Cooldown > Duration)
            {
                if (triggerInterval > 0)
                {
                    double d = Duration / triggerInterval;
                    double n = time / triggerInterval;
                    double p = triggerChance * GetChance(attackSpeed);

                    double c = Cooldown / triggerInterval;
                    if (discretizationCorrection)
                    {
                        c += 0.5;
                    }
                    if (c < 1.0) c = 1.0;
                    double x = n;

                    double uptime = 0.0;
                    int r = 1;
                    while (x > 0)
                    {
                        uptime += SpecialFunction.Ibeta(r, x, p);
                        double xd = x - d;
                        if (xd > 0)
                        {
                            uptime -= SpecialFunction.Ibeta(r, xd, p);
                        }
                        r++;
                        x -= c;
                    }

                    return (float)uptime;
                }
                else
                {
                    // this is a special case, meaning that it basically auto triggers on cooldown
                    float t = time;
                    float cc = Cooldown;
                    float uptime = (t % cc <= Duration) ? 1.0f : 0.0f;
                    return uptime;
                }
            }
            else if (Cooldown == 0.0f)
            {
                // only stationary model so far
                if (triggerInterval >= Duration) return Duration / triggerInterval * GetChance(attackSpeed) * triggerChance;
                else return 1.0f - (float)Math.Pow(1f - triggerChance * GetChance(attackSpeed), Duration / triggerInterval);
            }
            else
            {
                // only stationary model so far
                return 1.0f - 1.0f / (1.0f + Cooldown / triggerInterval * triggerChance * GetChance(attackSpeed)) * (float)Math.Pow(1f - triggerChance * GetChance(attackSpeed), (Duration - Cooldown) / triggerInterval);
            }
        }

        /// <summary>
        /// Computes average number of procs per second given the frequency of triggers.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced (for example for
        /// SpellCrit trigger you would set triggerInterval to average time between spell ticks and set
        /// triggerChance to crit chance)</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="fightDuration">Duration of fight in seconds.</param>
        public float GetAverageProcsPerSecond(float triggerInterval, float triggerChance, float attackSpeed, float fightDuration)
        {
            // derivation from recurrence relation
            // Procs[n] := average number of procs given fight length n
            // p := proc rate
            // q := 1-p
            // C := Cooldown / triggerInterval (assume C = 1 if cooldown less than trigger interval)

            // Procs[n] = p + p * Procs[n-C] + q * Procs[n-1]
            // split the function definition in segments of size C and solve each segment separately
            // that way the recurrence is only nonhomogeneous first order
            // Procs[n,k] = p + p * Procs[n,k-1] + q * Procs[n-1,k]

            // for k = 0 solve
            // Procs[n,0] = p + q * Procs[n-1,0]
            // Procs[0,0] = 0

            // solution is
            // Procs[n;0] = 1 - q^n

            // for further segments let k be the previous segment and Procs(n)=Procs[n,k] the closed form
            // solution for previous segment
            // the homogeneous part has solution of form A + B * q^n
            // the particular solution is obtained using method of undetermined coefficient using the
            // form of n * P(n) * q^n, where P is polynomial of order k
            // we can more generally just set the form of Procs2(n)=Procs[n,k+1] as
            // Procs2(n):=q^n*sum(A[i] * n^i, i, 0, k+1)+sum(B[i]*n^i,i,0,k+1);
            // using the recurrence
            // eq: Procs2(n) = p + p * Procs(n) + q * Procs2(n-1);
            // we compute the missing coefficients
            // splitqn(eq):=makelist(ratcoef(eq,q^n,i),i,0,1);
            // spliteq(eq,var,order):=makelist(ratcoef(eq,var,i),i,0,order);
            // eqlist: flatten(map(splitqn,makelist(ratcoef(eq,n,i),i,0,k+1)));
            // ev (tellsimp (0^0, 1), simp: false);
            // solution: solve(append(eqlist,[Procs2(0)=Procs(C)]),append(makelist(A[i],i,0,k+1),makelist(B[i],i,0,k+1)))[1];
            // Procs2s : subst(solution,Procs2(n));
            // Procs2s : ratsubst(Q,q^C,facsum(subst(q,1-p,ratsimp(subst(1-p,q,Procs2s))),q^n));

            // for the first few k we obtain the following (Q := q^C)

            // Procs[n;0] = 1 - q^n
            // Procs[n;1] = 2 - (1 + Q + p * n) * q^n
            // Procs[n;2] = 3 - (1 + Q + Q^2 + p * (n + (C + n)*Q) + p^2 * n*(n+1)/2) * q^n
            // Procs[n;3] = 4 - (1 + Q + Q^2 + Q^3 + p * (n + (C + n)*Q + (2*C + n)*Q^2) + p^2 * ((C^2+(2*n+1)*C+n^2+n)*Q+n^2+n)/2  + p^3 * (n*(n+1)*(n+2))/3!) * q^n

            // analyzing the form we can obtain the following generalization
            // Procs(n,k):=k+1-sum(sum(binomial(C*j+n+i-1,i)*q^(C*j+n)*p^i,j,0,k-i),i,0,k);
            // which can be verified with the recurrence

            // rearranging the sumation
            // Procs(n,k):=k+1-sum(q^(C*j+n)*sum(binomial(C*j+n+i-1,i)*p^i,i,0,k-j),j,0,k);

            // we inspect the inner sum which has the following form
            // G(K,kj,p):=sum(binomial(K+i,i)*p^i*(1-p)^K,i,0,kj);
            // for K = 0 we get
            // sum(p^i,i,0,kj)
            // which can be evaluated to
            // G[0](kj,p):=(1-p^(kj-1))/(1-p);
            // we can express G in terms of G with lower K as follows
            // G[K+1](kj,p)=(1-p)^K*sum((1-p)*p^i*binomial(K+i+1,i),i,0,kj);
            // G[K+1](kj,p)=((1-p)^K/(K+1)!*(sum((p^i*(K+i+1)*(K+i)!)/i!,i,0,kj)-sum(i*(p^i*(K+i)!)/i!,i,1,kj+1)));
            // G[K+1](kj,p)=sum((1-p)^K/K!*(p^i*(K+i)!)/i!,i,0,kj)-(1-p)^K/(K+1)!*(kj+1)*(p^(kj+1)*(K+kj+1)!)/(kj+1)!;
            // G[K+1](kj,p)=G[K](kj,p)-(1-p)^K/(K+1)!*(p^(kj+1)*(K+kj+1)!)/kj!;
            // G[K+1](kj,p)=G[K](kj,p)-(1-p)^K*p^(kj+1)*binomial(K+kj+1,kj);
            
            // this gives us an alternative formulation of G
            // G2(K,kj,p):=(1-p^(kj+1))/(1-p)-sum((1-p)^i*p^(kj+1)*binomial(i+kj+1,kj),i,0,K-1);

            // we can now rephrase the Procs function using this to get
            // Procs3(n,k):=k+1-(1-p)*sum(G2(C*j+n-1,k-j,p),j,0,k);
            // =sum(p^(k-j+1),j,0,k)+sum(sum(binomial(k-j+i+1,k-j)*(1-p)^(i+1),i,0,j*C+n-2)*p^(k-j+1),j,0,k);

            // we observe that we can express the inner expression in terms of negative binomial
            // Procs3(n,k):=sum(sum(binomial(r-1+i,r-1)*(1-p)^(i),i,0,k*C-(r-1)*C+n-1)*p^r,r,1,k+1);
            // Procs3(n,k)=sum(sum(Pr(NegBin(r,p)=i),i,0,k*C-(r-1)*C+n-1),r,1,k+1);
            // using the cumulative probability density for negative binomial we can express Procs as

            // P(x):=sum(I[p](r+1,x-r*C),r,0,floor(x/C))
            // where I[p] is incomplete beta function

            if (triggerChance == 0f || float.IsPositiveInfinity(triggerInterval))
            {
                return 0f;
            }

            bool discretizationCorrection = true;

            if (fightDuration == 0.0f) {
                // this is special case, meaning that we have infinite fight duration, so we have
                // a proc on average every Cooldown + triggerInterval / p
                float pp = triggerChance * GetChance(attackSpeed);
                if (pp == 0.0) return 0.0f;
                if (discretizationCorrection)
                {
                    return 1.0f / (Cooldown - triggerInterval / 2 + triggerInterval / pp);
                }
                else
                {
                    return 1.0f / (Cooldown - triggerInterval + triggerInterval / pp);
                }
            }
            if (triggerInterval == 0.0f) {
                // this is a special case, meaning that it basically auto triggers on cooldown
                return (1.0f + (float)Math.Floor(fightDuration / Cooldown)) / fightDuration;
            }

            bool needsBeta = fightDuration < Cooldown * 10;
            if (Mode == CalculationMode.Advanced && triggerInterval > 0 && needsBeta) {
                double c = Cooldown / triggerInterval;
                if (discretizationCorrection)
                {
                    c += 0.5;
                }
                if (c < 1.0) c = 1.0;
                double n = fightDuration / triggerInterval;
                double x = n;
                double p = triggerChance * GetChance(attackSpeed);

                double averageProcs = 0.0;
                int r = 1;
                while (x > 0) {
                    averageProcs += SpecialFunction.Ibeta(r, x, p);
                    r++;
                    x -= c;
                }
                return (float)(averageProcs / fightDuration);
            } else if (Mode == CalculationMode.Interpolation && triggerInterval > 0 && needsBeta) {
                lock (interpolator) {
                    Interpolator i;
                    if (!interpolator.TryGetValue(fightDuration, out i))
                    {
                        i = new ProcsPerSecondInterpolator(this, fightDuration);
                        interpolator[fightDuration] = i;
                    }
                    return i[triggerChance * GetChance(attackSpeed), triggerInterval];
                }
            } else {
                double d = Duration / triggerInterval;
                double n = fightDuration / triggerInterval;
                double p = triggerChance * GetChance(attackSpeed);

                double c = Cooldown / triggerInterval;
                if (discretizationCorrection)
                {
                    c += 0.5;
                }
                if (c < 1.0) c = 1.0;
                // (1+cooldown/fightDuration*(cooldown-1)/(2*K)) * 1/K
                double K = 1.0 / p + c - 1.0;
                double ppt = (1 + c / n * (c - 1) / (2 * K)) / K;
                return (float)(ppt / triggerInterval);
            }
        }

        /// <summary>
        /// Computes the average crit rate in given duration for special effect that has a static crit bonus and on each crit adds additional
        /// stacking bonus up to maxStack stacks. All parameters are in crit rate (you have to convert the values if the actual effect works
        /// based on crit rating).
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds.</param>
        /// <param name="duration">Duration of the special effect.</param>
        /// <param name="hitRate">Hit rate if crit success depends on hitting (two-roll model).</param>
        /// <param name="baseCritRate">Base crit rate without the special effect.</param>
        /// <param name="bonusCritRate">Static crit rate bonus of the special effect.</param>
        /// <param name="stackCritRate">How much crit rate the bonus changes on each succesful crit.</param>
        /// <param name="maxStack">Maximum number of stacks.</param>
        /// <returns>Average crit rate of the special effect in given duration.</returns>
        public static float GetAverageStackingCritRate(float triggerInterval, float duration, float hitRate, float baseCritRate, float bonusCritRate, float stackCritRate, int maxStack)
        {
            // Si,j = chance that at step i we are at j stacks
            // Si,j = Si-1,j * (1 - (C + B + j*c)) + Si-1,j-1 * (C + B + (j-1)*c)
            // average crit bonus = sum_i sum_j Si,j * (B + j*c) / N
            if (maxStack <= 0)
            {
                return Math.Min(1.0f, baseCritRate + bonusCritRate) - baseCritRate;
            }

            float[] C = new float[maxStack + 1];
            float[] B = new float[maxStack + 1];
            float[] S = new float[maxStack + 1];
            S[0] = 1.0f;
            for (int j = 0; j <= maxStack; j++)
            {
                C[j] = hitRate * Math.Min(1.0f, baseCritRate + bonusCritRate + j * stackCritRate); // chance that crit happens at j stacks
                B[j] = Math.Min(1.0f, baseCritRate + bonusCritRate + j * stackCritRate) - baseCritRate; // crit value of special effect at j stacks
            }
            int N = (int)(duration / triggerInterval);
            float average = 0.0f;
            for (int i = 0; i < N; i++)
            {
                average += S[maxStack] * B[maxStack];
                S[maxStack] += S[maxStack - 1] * C[maxStack - 1];
                for (int j = maxStack - 1; j > 0; j--)
                {
                    average += S[j] * B[j];
                    S[j] = S[j] * (1.0f - C[j]) + S[j - 1] * C[j - 1];
                }
                average += S[0] * B[0];
                S[0] -= S[0] * C[0]; // S[0] = S[0] * (1.0f - C[0])
            }
            return average / N;
        }


        public bool UsesPPM()
        {
            return Chance < 0;
        }

        public float GetChance(float attackspeed)
        {
            if (Chance < 0)
            {
                return -Chance / 60f * attackspeed;
            }
            else return Chance;
        }

        private string StackString
        {
            get
            {
                return MaxStack + "x";
            }
        }

        private string CooldownString
        {
            get
            {
                if (float.IsPositiveInfinity(Cooldown)) { return "Once per Fight"; }
                int cooldown = (int)Cooldown;
                if (cooldown % 60 == 0)
                {
                    return (cooldown / 60).ToString() + " Min";
                }
                else
                {
                    return cooldown.ToString() + " Sec";
                }
            }
        }

        private string DurationString
        {
            get
            {
                return ((int)Duration).ToString() + " Sec";
            }
        }

        private string ChanceString
        {
            get
            {
                if (Chance < 0) return (-Chance).ToString("N2") + " PPM";
                else return (Chance * 100).ToString("N0") + "%";
            }
        }

        private string TriggerString
        {
            get
            {
                switch (Trigger)
                {
                    case Trigger.DamageSpellCast:
                        return "on Damaging Spell Cast";
                    case Trigger.DamageDone:
                        return "on Damage Dealt";
                    case Trigger.DamageSpellCrit:
                        return "on Damaging Spell Crit";
                    case Trigger.DamageSpellHit:
                        return "on Damaging Spell Hit";
                    case Trigger.HealingSpellCast:
                        return "on Healing Cast";
                    case Trigger.HealingSpellCrit:
                        return "on Healing Crit";
                    case Trigger.HealingSpellHit:
                        return "on Healing Hit";
                    case Trigger.MeleeCrit:
                        return "on Melee Crit";
                    case Trigger.MeleeHit:
                        return "on Melee Hit";
                    case Trigger.CurrentHandHit:
                        return "on Current Hand Melee Hit";
                    case Trigger.MainHandHit:
                        return "on Main Hand Melee Hit";
                    case Trigger.OffHandHit:
                        return "on Off Hand Melee Hit";
                    case Trigger.MeleeAttack:
                        return "on Melee Attack";
                    case Trigger.SpellCast:
                        return "on Spell Cast";
                    case Trigger.SpellCrit:
                        return "on Spell Crit";
                    case Trigger.SpellHit:
                        return "on Spell Hit";
                    case Trigger.SpellMiss:
                        return "on Spell Miss";
                    case Trigger.Use:
                        return "";
                    case Trigger.PhysicalHit:
                        return "on Physical Hit";
                    case Trigger.PhysicalCrit:
                        return "on Physical Crit";
                    case Trigger.ManaGem:
                        return "on Mana Gem";
                    case Trigger.MageNukeCast:
                        return "on Mage Nuke Cast";
                    case Trigger.JudgementHit:
                        return "on Judgement";
                    case Trigger.HolyShield:
                        return "on Holy Shield cast";
                    case Trigger.HammeroftheRighteous:
                        return "on HotR cast";
                    case Trigger.ShieldofRighteousness:
                        return "on ShoR cast";
                    case Trigger.CrusaderStrikeHit:
                        return "on Crusader Strike";
                    case Trigger.InsectSwarmTick :
                        return "on Insect Swarm Tick";
                    case Trigger.ShamanLightningBolt:
                        return "on Lightning Bolt Cast";
                    case Trigger.ShamanLavaLash:
                        return "on Lava Lash Hit";
                    case Trigger.ShamanShock:
                        return "on Shock Hit";
                    case Trigger.ShamanStormStrike:
                        return "on Stormstrike Hit";
                    case Trigger.BloodStrikeHit:
                        return "on Blood Strike";
                    case Trigger.HeartStrikeHit:
                        return "on Heart Strike";
                    case Trigger.BloodStrikeOrHeartStrikeHit:
                        return "on Blood Strike or Heart Strike";
                    case Trigger.IcyTouchHit:
                        return "on Icy Touch";
                    case Trigger.PlagueStrikeHit:
                        return "on Plague Strike";
                    case Trigger.RuneStrikeHit:
                        return "on Rune Strike";
                    default:
                        return Trigger.ToString();
                }
            }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            if (MaxStack > 1) { s.Append(StackString); s.Append(" "); }
            
            s.Append(Stats.ToString());
            s.Append(" (");

            bool needsSpace = false;
            if (Duration > 0 && !float.IsInfinity(Duration))
            {
                s.Append(DurationString);
                needsSpace = true;
            }
            if (Chance < 1)
            {
                if (needsSpace) s.Append(" ");
                s.Append(ChanceString);
                needsSpace = true;
            }

            if (Trigger != Trigger.Use)
            {
                if (needsSpace) s.Append(" ");
                s.Append(TriggerString);
                needsSpace = true;
            }
            if (Cooldown > 0)
            {
                if (needsSpace) s.Append("/");
                s.Append(CooldownString);
            }
            s.Append(")");
            return s.ToString();
        }
    }
}
