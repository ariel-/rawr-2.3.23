using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Mage
{
    public abstract class SpellTemplate
    {
        public string Name;

        public bool Channeled;
        public bool Instant;
        public float BaseCastTime;
        public float BaseUntalentedCastTime;
        public float BaseCooldown;
        public float Cooldown;

        public bool AreaEffect;
        public int BaseCost;
        public MagicSchool MagicSchool;
        public float Ticks;
        public float CastProcs;
        public float CastProcs2;
        public float BaseMinDamage;
        public float BaseMaxDamage;
        public float SpellDamageCoefficient;
        public float BasePeriodicDamage;
        public float DotDamageCoefficient;
        public float DotDuration;

        public float BaseDirectDamageModifier;
        public float BaseDotDamageModifier;
        public float BaseCostModifier;
        public float BaseCostAmplifier;
        public float BaseInterruptProtection;

        public float BaseSpellModifier;
        public float BaseAdditiveSpellModifier;
        public float BaseCritRate;
        public float CritBonus;
        public float HitRate;
        public float ThreatMultiplier;
        public float Range;

        public float RealResistance;
        public float PartialResistFactor;

        // not initialized, but never changed
        public float DotTickInterval;
        public float NukeProcs;

        public bool Dirty = true;

        public static Dictionary<int, int> BaseMana = new Dictionary<int, int>();
        static SpellTemplate()
        {
            BaseMana[70] = 2241;
            BaseMana[71] = 2343;
            BaseMana[72] = 2446;
            BaseMana[73] = 2549;
            BaseMana[74] = 2652;
            BaseMana[75] = 2754;
            BaseMana[76] = 2857;
            BaseMana[77] = 2960;
            BaseMana[78] = 3063;
            BaseMana[79] = 3166;
            BaseMana[80] = 3268;
            BaseMana[81] = 4567;
            BaseMana[82] = 6382;
            BaseMana[83] = 6382;
            BaseMana[84] = 6382;
            BaseMana[85] = 6382;
        }

        public virtual Spell GetSpell(CastingState castingState)
        {
            if (AreaEffect)
            {
                AoeSpell spell = new AoeSpell(this);
                spell.Calculate(castingState);
                spell.CalculateDerivedStats(castingState);
                return spell;
            }
            else
            {
                Spell spell = Spell.New(this, castingState.Solver);
                spell.Calculate(castingState);
                spell.CalculateDerivedStats(castingState);
                return spell;
            }
        }

        public float GetEffectAverageDamage(CastingState castingState)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            float damagePerSpellPower;
            float igniteDamage;
            float igniteDamagePerSpellPower;           
            return spell.CalculateAverageDamage(castingState.Solver, 0, false, false, out damagePerSpellPower, out igniteDamage, out igniteDamagePerSpellPower);
        }

        protected SpellTemplate() { }

        public void InitializeCastTime(bool channeled, bool instant, float castTime, float cooldown)
        {
            Channeled = channeled;
            Instant = instant;
            BaseCastTime = castTime;
            BaseUntalentedCastTime = instant ? 1.5f : castTime;
            BaseCooldown = cooldown;
            Cooldown = cooldown;
        }

        public void InitializeDamage(Solver solver, bool areaEffect, int range, MagicSchool magicSchool, SpellData spellData) 
        {
            InitializeDamage(solver, areaEffect, range, magicSchool, spellData, 1, 1, 0);
        }

        public void InitializeDamage(Solver solver, bool areaEffect, int range, MagicSchool magicSchool, SpellData spellData, float hitProcs, float castProcs, float dotDuration)
        {
            Stats baseStats = solver.BaseStats;
            MageTalents mageTalents = solver.MageTalents;
            CalculationOptionsMage calculationOptions = solver.CalculationOptions;

            AreaEffect = areaEffect;
            BaseCost = spellData.Cost - (int)baseStats.SpellsManaReduction;
            MagicSchool = magicSchool;
            Ticks = hitProcs;
            CastProcs = castProcs;
            CastProcs2 = castProcs;
            BaseMinDamage = spellData.MinDamage;
            BaseMaxDamage = spellData.MaxDamage;
            SpellDamageCoefficient = spellData.SpellDamageCoefficient;
            BasePeriodicDamage = spellData.PeriodicDamage;
            DotDamageCoefficient = spellData.DotDamageCoefficient;
            DotDuration = dotDuration;

            BaseDirectDamageModifier = 1.0f;
            BaseDotDamageModifier = 1.0f;
            BaseCostModifier = 1.0f;

            float baseCostAmplifier = calculationOptions.EffectCostMultiplier;
            baseCostAmplifier *= (1.0f - 0.01f * mageTalents.Precision);
            if (mageTalents.FrostChanneling > 0) baseCostAmplifier *= (1.0f - 0.01f - 0.03f * mageTalents.FrostChanneling);
            if (MagicSchool == MagicSchool.Arcane) baseCostAmplifier *= (1.0f - 0.01f * mageTalents.ArcaneFocus);
            BaseCostAmplifier = baseCostAmplifier;

            float baseInterruptProtection = baseStats.InterruptProtection;
            if (MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire)
            {
                baseInterruptProtection += 0.35f * mageTalents.BurningSoul;
            }
            BaseInterruptProtection = baseInterruptProtection;

            float realResistance;
            switch (MagicSchool)
            {
                case MagicSchool.Arcane:
                    BaseSpellModifier = solver.BaseArcaneSpellModifier;
                    BaseAdditiveSpellModifier = solver.BaseArcaneAdditiveSpellModifier;
                    BaseCritRate = solver.BaseArcaneCritRate;
                    CritBonus = solver.BaseArcaneCritBonus;
                    HitRate = solver.BaseArcaneHitRate;
                    ThreatMultiplier = solver.ArcaneThreatMultiplier;
                    realResistance = calculationOptions.ArcaneResist;
                    if (range != 0)
                    {
                        Range = range + mageTalents.MagicAttunement * 3;
                    }
                    break;
                case MagicSchool.Fire:
                    BaseSpellModifier = solver.BaseFireSpellModifier;
                    BaseAdditiveSpellModifier = solver.BaseFireAdditiveSpellModifier;
                    BaseCritRate = solver.BaseFireCritRate;
                    CritBonus = solver.BaseFireCritBonus;
                    HitRate = solver.BaseFireHitRate;
                    ThreatMultiplier = solver.FireThreatMultiplier;
                    realResistance = calculationOptions.FireResist;
                    if (range != 0)
                    {
                        Range = range + mageTalents.FlameThrowing * 3;
                    }
                    break;
                case MagicSchool.FrostFire:
                    BaseSpellModifier = solver.BaseFrostFireSpellModifier;
                    BaseAdditiveSpellModifier = solver.BaseFrostFireAdditiveSpellModifier;
                    BaseCritRate = solver.BaseFrostFireCritRate;
                    CritBonus = solver.BaseFrostFireCritBonus;
                    HitRate = solver.BaseFrostFireHitRate;
                    ThreatMultiplier = solver.FrostFireThreatMultiplier;
                    if (calculationOptions.FireResist == -1)
                    {
                        realResistance = calculationOptions.FrostResist;
                    }
                    else if (calculationOptions.FrostResist == -1)
                    {
                        realResistance = calculationOptions.FireResist;
                    }
                    else
                    {
                        realResistance = Math.Min(calculationOptions.FireResist, calculationOptions.FrostResist);
                    }
                    Range = range;
                    break;
                case MagicSchool.Frost:
                    BaseSpellModifier = solver.BaseFrostSpellModifier;
                    BaseAdditiveSpellModifier = solver.BaseFrostAdditiveSpellModifier;
                    BaseCritRate = solver.BaseFrostCritRate;
                    CritBonus = solver.BaseFrostCritBonus;
                    HitRate = solver.BaseFrostHitRate;
                    ThreatMultiplier = solver.FrostThreatMultiplier;
                    realResistance = calculationOptions.FrostResist;
                    Range = range * (1 + mageTalents.ArcticReach * 0.1f);
                    break;
                case MagicSchool.Nature:
                    BaseSpellModifier = solver.BaseNatureSpellModifier;
                    BaseAdditiveSpellModifier = solver.BaseNatureAdditiveSpellModifier;
                    BaseCritRate = solver.BaseNatureCritRate;
                    CritBonus = solver.BaseNatureCritBonus;
                    HitRate = solver.BaseNatureHitRate;
                    ThreatMultiplier = solver.NatureThreatMultiplier;
                    realResistance = calculationOptions.NatureResist;
                    Range = range;
                    break;
                case MagicSchool.Shadow:
                    BaseSpellModifier = solver.BaseShadowSpellModifier;
                    BaseAdditiveSpellModifier = solver.BaseShadowAdditiveSpellModifier;
                    BaseCritRate = solver.BaseShadowCritRate;
                    CritBonus = solver.BaseShadowCritBonus;
                    HitRate = solver.BaseShadowHitRate;
                    ThreatMultiplier = solver.ShadowThreatMultiplier;
                    realResistance = calculationOptions.ShadowResist;
                    Range = range;
                    break;
                case MagicSchool.Holy:
                default:
                    BaseSpellModifier = solver.BaseHolySpellModifier;
                    BaseAdditiveSpellModifier = solver.BaseHolyAdditiveSpellModifier;
                    BaseCritRate = solver.BaseHolyCritRate;
                    CritBonus = solver.BaseHolyCritBonus;
                    HitRate = solver.BaseHolyHitRate;
                    ThreatMultiplier = solver.HolyThreatMultiplier;
                    realResistance = calculationOptions.HolyResist;
                    Range = range;
                    break;
            }

            int playerLevel = calculationOptions.PlayerLevel;
            int targetLevel;

            if (areaEffect)
            {
                targetLevel = calculationOptions.AoeTargetLevel;
                float hitRate = ((targetLevel <= playerLevel + 2) ? (0.96f - (targetLevel - playerLevel) * 0.01f) : (0.94f - (targetLevel - playerLevel - 2) * 0.11f)) + solver.BaseSpellHit;
                if (MagicSchool == MagicSchool.Arcane) hitRate += 0.01f * mageTalents.ArcaneFocus;
                if (hitRate > Spell.MaxHitRate) hitRate = Spell.MaxHitRate;
                HitRate = hitRate;
            }
            else
            {
                targetLevel = calculationOptions.TargetLevel;
            }

            RealResistance = realResistance;
            PartialResistFactor = (realResistance == -1) ? 0 : (1 - StatConversion.GetAverageResistance(playerLevel, targetLevel, realResistance, baseStats.SpellPenetration));
        }

        public void InitializeEffectDamage(Solver solver, MagicSchool magicSchool, float minDamage, float maxDamage)
        {
            Stats baseStats = solver.BaseStats;
            MageTalents mageTalents = solver.MageTalents;
            CalculationOptionsMage calculationOptions = solver.CalculationOptions;

            //AreaEffect = areaEffect;
            //BaseCost = cost - (int)baseStats.SpellsManaReduction;
            MagicSchool = magicSchool;
            BaseMinDamage = minDamage;
            BaseMaxDamage = maxDamage;
            //BasePeriodicDamage = periodicDamage;
            //SpellDamageCoefficient = spellDamageCoefficient;
            //Ticks = 1;
            //CastProcs = 0;
            //CastProcs2 = 0;
            //DotDamageCoefficient = dotDamageCoefficient;
            //DotDuration = dotDuration;

            BaseDirectDamageModifier = 1.0f;
            BaseDotDamageModifier = 1.0f;
            BaseCostModifier = 1.0f;

            //Range = range;
            
            /*float baseCostAmplifier = calculationOptions.EffectCostMultiplier;
            baseCostAmplifier *= (1.0f - 0.01f * mageTalents.Precision);
            if (mageTalents.FrostChanneling > 0) baseCostAmplifier *= (1.0f - 0.01f - 0.03f * mageTalents.FrostChanneling);
            if (MagicSchool == MagicSchool.Arcane) baseCostAmplifier *= (1.0f - 0.01f * mageTalents.ArcaneFocus);
            BaseCostAmplifier = baseCostAmplifier;*/

            /*float baseInterruptProtection = baseStats.InterruptProtection;
            if (MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire)
            {
                baseInterruptProtection += 0.35f * mageTalents.BurningSoul;
                AffectedByFlameCap = true;
            }
            BaseInterruptProtection = baseInterruptProtection;*/

            float realResistance;
            switch (MagicSchool)
            {
                case MagicSchool.Arcane:
                    BaseSpellModifier = solver.BaseArcaneSpellModifier;
                    BaseAdditiveSpellModifier = solver.BaseArcaneAdditiveSpellModifier;
                    BaseCritRate = solver.BaseArcaneCritRate;
                    CritBonus = solver.BaseArcaneCritBonus;
                    HitRate = solver.BaseArcaneHitRate;
                    ThreatMultiplier = solver.ArcaneThreatMultiplier;
                    realResistance = calculationOptions.ArcaneResist;
                    break;
                case MagicSchool.Fire:
                    BaseSpellModifier = solver.BaseFireSpellModifier;
                    BaseAdditiveSpellModifier = solver.BaseFireAdditiveSpellModifier;
                    BaseCritRate = solver.BaseFireCritRate;
                    CritBonus = solver.BaseFireCritBonus;
                    HitRate = solver.BaseFireHitRate;
                    ThreatMultiplier = solver.FireThreatMultiplier;
                    realResistance = calculationOptions.FireResist;
                    break;
                case MagicSchool.FrostFire:
                    BaseSpellModifier = solver.BaseFrostFireSpellModifier;
                    BaseAdditiveSpellModifier = solver.BaseFrostFireAdditiveSpellModifier;
                    BaseCritRate = solver.BaseFrostFireCritRate;
                    CritBonus = solver.BaseFrostFireCritBonus;
                    HitRate = solver.BaseFrostFireHitRate;
                    ThreatMultiplier = solver.FrostFireThreatMultiplier;
                    if (calculationOptions.FireResist == -1)
                    {
                        realResistance = calculationOptions.FrostResist;
                    }
                    else if (calculationOptions.FrostResist == -1)
                    {
                        realResistance = calculationOptions.FireResist;
                    }
                    else
                    {
                        realResistance = Math.Min(calculationOptions.FireResist, calculationOptions.FrostResist);
                    }
                    break;
                case MagicSchool.Frost:
                    BaseSpellModifier = solver.BaseFrostSpellModifier;
                    BaseAdditiveSpellModifier = solver.BaseFrostAdditiveSpellModifier;
                    BaseCritRate = solver.BaseFrostCritRate;
                    CritBonus = solver.BaseFrostCritBonus;
                    HitRate = solver.BaseFrostHitRate;
                    ThreatMultiplier = solver.FrostThreatMultiplier;
                    realResistance = calculationOptions.FrostResist;
                    break;
                case MagicSchool.Nature:
                    BaseSpellModifier = solver.BaseNatureSpellModifier;
                    BaseAdditiveSpellModifier = solver.BaseNatureAdditiveSpellModifier;
                    BaseCritRate = solver.BaseNatureCritRate;
                    CritBonus = solver.BaseNatureCritBonus;
                    HitRate = solver.BaseNatureHitRate;
                    ThreatMultiplier = solver.NatureThreatMultiplier;
                    realResistance = calculationOptions.NatureResist;
                    break;
                case MagicSchool.Shadow:
                    BaseSpellModifier = solver.BaseShadowSpellModifier;
                    BaseAdditiveSpellModifier = solver.BaseShadowAdditiveSpellModifier;
                    BaseCritRate = solver.BaseShadowCritRate;
                    CritBonus = solver.BaseShadowCritBonus;
                    HitRate = solver.BaseShadowHitRate;
                    ThreatMultiplier = solver.ShadowThreatMultiplier;
                    realResistance = calculationOptions.ShadowResist;
                    break;
                case MagicSchool.Holy:
                default:
                    BaseSpellModifier = solver.BaseHolySpellModifier;
                    BaseAdditiveSpellModifier = solver.BaseHolyAdditiveSpellModifier;
                    BaseCritRate = solver.BaseHolyCritRate;
                    CritBonus = solver.BaseHolyCritBonus;
                    HitRate = solver.BaseHolyHitRate;
                    ThreatMultiplier = solver.HolyThreatMultiplier;
                    realResistance = calculationOptions.HolyResist;
                    break;
            }

            int playerLevel = calculationOptions.PlayerLevel;
            int targetLevel = calculationOptions.TargetLevel;

            /*if (areaEffect)
            {
                targetLevel = calculationOptions.AoeTargetLevel;
                float hitRate = ((targetLevel <= playerLevel + 2) ? (0.96f - (targetLevel - playerLevel) * 0.01f) : (0.94f - (targetLevel - playerLevel - 2) * 0.11f)) + calculations.BaseSpellHit;
                if (MagicSchool == MagicSchool.Arcane) hitRate += 0.01f * mageTalents.ArcaneFocus;
                if (hitRate > Spell.MaxHitRate) hitRate = Spell.MaxHitRate;
                HitRate = hitRate;
            }
            else
            {
                targetLevel = calculationOptions.TargetLevel;
            }*/

            RealResistance = realResistance;
            PartialResistFactor = (realResistance == -1) ? 0 : (1 - StatConversion.GetAverageResistance(playerLevel, targetLevel, realResistance, baseStats.SpellPenetration));
        }

        public static float ProcBuffUp(float procChance, float buffDuration, float triggerInterval)
        {
            if (triggerInterval <= 0)
                return 0;
            else
                return 1 - (float)Math.Pow(1 - procChance, buffDuration / triggerInterval);
        }

        public float CalculateCastTime(CastingState castingState, float interruptProtection, float critRate, bool pom, float baseCastTime, out float channelReduction)
        {
            CalculationOptionsMage calculationOptions = castingState.CalculationOptions;
            float castingSpeed = castingState.CastingSpeed;
            float spellHasteRating = castingState.SpellHasteRating;
            float levelScalingFactor = calculationOptions.LevelScalingFactor;
            float hasteFactor = levelScalingFactor / 1000f;
            float rootCastingSpeed = castingSpeed / (1 + spellHasteRating * hasteFactor);

            float InterruptFactor = 0f;
            float maxPushback = 0f;
            if (calculationOptions.InterruptFrequency > 0f)
            {
                // interrupt factors of more than once per spell are not supported, so put a limit on it (up to twice is probably approximately correct)
                InterruptFactor = Math.Min(calculationOptions.InterruptFrequency, 2 * castingSpeed / baseCastTime);
                if (castingState.IcyVeins) interruptProtection = 1;
                maxPushback = 0.5f * Math.Max(0, 1 - interruptProtection);
                if (Channeled) maxPushback = 0.0f;
            }

            if (pom) baseCastTime = 0.0f;

            float globalCooldown = Math.Max(Spell.GlobalCooldownLimit, 1.5f / castingSpeed);
            float latency;
            if (baseCastTime <= 1.5f || Instant)
            {
                latency = calculationOptions.LatencyGCD;
            }
            else if (Channeled)
            {
                latency = calculationOptions.LatencyChannel;
            }
            else
            {
                latency = calculationOptions.LatencyCast;
            }
            float averageTicks = Ticks;
            float castTime = baseCastTime / castingSpeed;
            if (calculationOptions.Beta && Channeled)
            {
                float tickCastTime = castTime / Ticks;
                averageTicks = (float)Math.Floor(baseCastTime / tickCastTime);
                castTime = averageTicks * tickCastTime;
            }
            if (InterruptFactor > 0)
            {
                castTime = castTime * (1 + InterruptFactor * maxPushback) - (maxPushback * 0.5f) * maxPushback * InterruptFactor;
            }
            castTime += latency;
            float gcdcap = globalCooldown + calculationOptions.LatencyGCD;
            if (castTime < gcdcap) castTime = gcdcap;
            channelReduction = 0.0f;

            if (!calculationOptions.AdvancedHasteProcs)
            {
                for (int i = 0; i < castingState.Solver.HasteRatingEffectsCount; i++)
                {
                    SpecialEffect effect = castingState.Solver.HasteRatingEffects[i];
                    float procs = 0.0f;
                    int triggers = 0;
                    switch (effect.Trigger)
                    {
                        case Trigger.DamageSpellCast:
                        case Trigger.SpellCast:
                            procs = CastProcs;
                            triggers = (int)procs;
                            break;
                        case Trigger.DamageSpellCrit:
                        case Trigger.SpellCrit:
                            procs = critRate * averageTicks;
                            triggers = (int)averageTicks;
                            break;
                        case Trigger.DamageSpellHit:
                        case Trigger.SpellHit:
                            procs = HitRate * averageTicks;
                            triggers = (int)averageTicks;
                            break;
                    }
                    if (procs == 0.0f) continue;
                    float procHaste = effect.Stats.HasteRating;
                    if (effect.Cooldown >= effect.Duration)
                    {
                        // hasted casttime
                        float speed = rootCastingSpeed * (1 + (spellHasteRating + procHaste) * hasteFactor);
                        float gcd = Math.Max(Spell.GlobalCooldownLimit, 1.5f / speed);
                        float cast = baseCastTime / speed;
                        if (calculationOptions.Beta && Channeled)
                        {
                            float tickCastTime = cast / Ticks;
                            cast = (float)Math.Floor(baseCastTime / tickCastTime) * tickCastTime;
                        }
                        if (InterruptFactor > 0)
                        {
                            cast = cast * (1 + InterruptFactor * maxPushback) - (maxPushback * 0.5f) * maxPushback * InterruptFactor;
                        }
                        cast += latency;
                        gcdcap = gcd + calculationOptions.LatencyGCD;
                        if (cast < gcdcap) cast = gcdcap;

                        float castsAffected = 0;
                        if (triggers > 1)
                        {
                            // multi tick spell (i.e. AM)
                            for (int c = 0; c < triggers; c++) castsAffected += (float)Math.Ceiling((effect.Duration - c * castTime / triggers) / cast);
                            castsAffected /= triggers;
                        }
                        else
                        {
                            // single tick spell
                            castsAffected = (float)Math.Ceiling(effect.Duration / cast); // should the first proc be already hasted?
                        }
                        float effectiveDuration = castsAffected * cast;
                        // this isn't completely accurate, we should have made a separate SpecialEffect and change the actual duration
                        // but performance would hurt so this'll have to do
                        spellHasteRating += procHaste * (effectiveDuration / effect.Duration) * effect.GetAverageUptime(castTime / triggers, procs / triggers, 3.0f, calculationOptions.FightDuration);
                        //spellHasteRating += procHaste * castsAffected * cast / (effect.Cooldown + castTime / procs / effect.Chance);
                        //Haste += castingState.BasicStats.SpellHasteFor6SecOnCast_15_45 * 6f / (45f + CastTime / CastProcs / 0.15f);
                        castingSpeed = rootCastingSpeed * (1 + spellHasteRating * hasteFactor);

                        globalCooldown = Math.Max(Spell.GlobalCooldownLimit, 1.5f / castingSpeed);
                        castTime = baseCastTime / castingSpeed;
                        if (calculationOptions.Beta && Channeled)
                        {
                            float tickCastTime = castTime / Ticks;
                            averageTicks = (float)Math.Floor(baseCastTime / tickCastTime);
                            castTime = averageTicks * tickCastTime;
                        }
                        if (InterruptFactor > 0)
                        {
                            castTime = castTime * (1 + InterruptFactor * maxPushback) - (maxPushback * 0.5f) * maxPushback * InterruptFactor;
                        }
                        castTime += latency;
                        gcdcap = globalCooldown + calculationOptions.LatencyGCD;
                        if (castTime < gcdcap) castTime = gcdcap;
                    }
                    else if (effect.Cooldown == 0 && (effect.Trigger == Trigger.SpellCrit || effect.Trigger == Trigger.DamageSpellCrit))
                    {
                        float rawHaste = spellHasteRating;

                        castingSpeed /= (1 + spellHasteRating / 1000f * levelScalingFactor);
                        float proccedSpeed = castingSpeed * (1 + (rawHaste + procHaste) / 1000f * levelScalingFactor);
                        float proccedGcd = Math.Max(Spell.GlobalCooldownLimit, 1.5f / proccedSpeed);
                        float proccedCastTime = baseCastTime / proccedSpeed;
                        float proccedTicks = averageTicks;
                        if (calculationOptions.Beta && Channeled)
                        {
                            float tickCastTime = proccedCastTime / Ticks;
                            proccedTicks = (float)Math.Floor(baseCastTime / tickCastTime);
                            castTime = proccedTicks * tickCastTime;
                        }
                        if (InterruptFactor > 0)
                        {
                            proccedCastTime = proccedCastTime * (1 + InterruptFactor * maxPushback) - (maxPushback * 0.5f) * maxPushback * InterruptFactor;
                        }
                        proccedCastTime += latency;
                        if (proccedCastTime < proccedGcd + calculationOptions.LatencyGCD) proccedCastTime = proccedGcd + calculationOptions.LatencyGCD;
                        int chancesToProc = (int)(((int)Math.Floor(effect.Duration / proccedCastTime) + 1) * proccedTicks);
                        if (!(Instant || pom)) chancesToProc -= 1;
                        if (AreaEffect) chancesToProc *= calculationOptions.AoeTargets;
                        spellHasteRating = rawHaste + procHaste * (1 - (float)Math.Pow(1 - effect.Chance * critRate, chancesToProc));
                        //Haste = rawHaste + castingState.BasicStats.SpellHasteFor5SecOnCrit_50 * ProcBuffUp(1 - (float)Math.Pow(1 - 0.5f * CritRate, HitProcs), 5, CastTime);
                        castingSpeed *= (1 + spellHasteRating / 1000f * levelScalingFactor);
                        globalCooldown = Math.Max(Spell.GlobalCooldownLimit, 1.5f / castingSpeed);
                        castTime = baseCastTime / castingSpeed;
                        if (calculationOptions.Beta && Channeled)
                        {
                            float tickCastTime = castTime / Ticks;
                            averageTicks = (float)Math.Floor(baseCastTime / tickCastTime);
                            castTime = averageTicks * tickCastTime;
                        }
                        if (InterruptFactor > 0)
                        {
                            castTime = castTime * (1 + InterruptFactor * maxPushback) - (maxPushback * 0.5f + latency) * maxPushback * InterruptFactor;
                        }
                        castTime += latency;
                        if (castTime < globalCooldown + calculationOptions.LatencyGCD) castTime = globalCooldown + calculationOptions.LatencyGCD;
                    }
                }
                // on use stacking items
                for (int i = 0; i < castingState.Solver.StackingHasteEffectCooldownsCount; i++)
                {
                    EffectCooldown effectCooldown = castingState.Solver.StackingHasteEffectCooldowns[i];
                    if (castingState.EffectsActive(effectCooldown.Mask))
                    {
                        Stats stats = effectCooldown.SpecialEffect.Stats;
                        for (int j = 0; j < stats._rawSpecialEffectDataSize; j++)
                        {
                            SpecialEffect effect = stats._rawSpecialEffectData[j];
                            float procHaste = effect.Stats.HasteRating;
                            if (procHaste > 0)
                            {
                                float procs = 0.0f;
                                switch (effect.Trigger)
                                {
                                    case Trigger.DamageSpellCast:
                                    case Trigger.SpellCast:
                                        procs = CastProcs;
                                        break;
                                    case Trigger.DamageSpellCrit:
                                    case Trigger.SpellCrit:
                                        procs = critRate * averageTicks;
                                        break;
                                    case Trigger.DamageSpellHit:
                                    case Trigger.SpellHit:
                                        procs = HitRate * averageTicks;
                                        break;
                                }
                                if (procs == 0.0f) continue;
                                // until they put in some good trinkets with such effects just do a quick dirty calculation
                                float effectHasteRating;
                                if (procs > averageTicks)
                                {
                                    // some 100% on cast procs, happens because AM has 6 cast procs and only 5 ticks
                                    effectHasteRating = effect.GetAverageStackSize(castTime / procs, 1.0f, 3.0f, effectCooldown.SpecialEffect.Duration) * procHaste;
                                }
                                else
                                {
                                    effectHasteRating = effect.GetAverageStackSize(castTime / averageTicks, procs / averageTicks, 3.0f, effectCooldown.SpecialEffect.Duration) * procHaste;
                                }

                                castingSpeed /= (1 + spellHasteRating / 1000f * levelScalingFactor);
                                spellHasteRating += effectHasteRating;
                                castingSpeed *= (1 + spellHasteRating / 1000f * levelScalingFactor);

                                globalCooldown = Math.Max(Spell.GlobalCooldownLimit, 1.5f / castingSpeed);
                                castTime = baseCastTime / castingSpeed;
                                if (calculationOptions.Beta && Channeled)
                                {
                                    float tickCastTime = castTime / Ticks;
                                    averageTicks = (float)Math.Floor(baseCastTime / tickCastTime);
                                    castTime = averageTicks * tickCastTime;
                                }
                                if (InterruptFactor > 0)
                                {
                                    castTime = castTime * (1 + InterruptFactor * maxPushback) - (maxPushback * 0.5f) * maxPushback * InterruptFactor;
                                }
                                castTime += latency;
                                if (castTime < globalCooldown + calculationOptions.LatencyGCD) castTime = globalCooldown + calculationOptions.LatencyGCD;
                            }
                        }
                    }
                }
            }

            if (Channeled && calculationOptions.Beta)
            {
                channelReduction = 1 - averageTicks / Ticks;
            }

            // channeled pushback
            if (Channeled && InterruptFactor > 0)
            {
                int maxLostTicks = (int)Math.Ceiling(averageTicks * 0.25f * Math.Max(0, 1 - interruptProtection));
                // pushbacks that happen up to pushbackCastTime cut the cast time to pushbackCastTime
                // pushbacks that happen after just terminate the channel
                // [---|---X---|---|---]
                castTime -= latency;
                float tickFactor = 0;
                for (int i = 0; i < maxLostTicks; i++)
                {
                    tickFactor += InterruptFactor * castTime / averageTicks * (i + 1) / averageTicks;
                }
                tickFactor += InterruptFactor * (averageTicks - maxLostTicks) * castTime / averageTicks * maxLostTicks / averageTicks;
                if (calculationOptions.Beta)
                {
                    channelReduction = 1 - averageTicks * (1 - tickFactor) / Ticks;
                }
                else
                {
                    channelReduction = tickFactor;
                }
                castTime = castTime * (1 - tickFactor) + latency;
            }

            return castTime;
        }
    }
}
