using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Rawr.Mage
{
    public enum MagicSchool
    {
        Holy = 1,
        Fire = 2,
        Nature,
        Frost,
        Shadow,
        Arcane,
        FrostFire
    }

    public struct SpellData
    {
        public float MinDamage;
        public float MaxDamage;
        public float PeriodicDamage;
        public int Cost;
        public float SpellDamageCoefficient;
        public float DotDamageCoefficient;
    }

    public class SpellContribution : IComparable<SpellContribution>
    {
        public string Name;
        public float Hits;
        public float Crits;
        public float Ticks;
        public float Damage;
        public float HitDamage;
        public float CritDamage;
        public float TickDamage;
        public float DotUptime;
        public float Range;

        public int CompareTo(SpellContribution other)
        {
            return other.Damage.CompareTo(this.Damage);
        }
    }

    public class AoeSpell : Spell
    {
        public AoeSpell(SpellTemplate template) : base(template) { }

        public override void Calculate(CastingState castingState)
        {
            base.Calculate(castingState);
            // do not count debuffs for aoe effects, can't assume it will be up on all
            // do not include molten fury (molten fury relates to boss), instead amplify all by average
            if (castingState.MoltenFury)
            {
                SpellModifier /= (1 + 0.06f * castingState.MageTalents.MoltenFury);
            }
            if (castingState.MageTalents.MoltenFury > 0)
            {
                SpellModifier *= (1 + 0.06f * castingState.MageTalents.MoltenFury * castingState.CalculationOptions.MoltenFuryPercentage);
            }
        }

        public override void CalculateDerivedStats(CastingState castingState, bool outOfFiveSecondRule, bool pom, bool spammedDot, bool round, bool forceHit, bool forceMiss)
        {
            base.CalculateDerivedStats(castingState, outOfFiveSecondRule, pom, spammedDot, round, forceHit, forceMiss);
            TargetProcs *= castingState.CalculationOptions.AoeTargets;
        }

        public override float CalculateAverageDamage(Solver solver, float spellPower, bool spammedDot, bool forceHit, out float damagePerSpellPower, out float igniteDamage, out float igniteDamagePerSpellPower)
        {
            damagePerSpellPower = 0; // do we really need this for aoe?
            float baseAverage = (BaseMinDamage + BaseMaxDamage) / 2f + spellPower * SpellDamageCoefficient;
            float critMultiplier = 1 + (CritBonus - 1) * Math.Max(0, CritRate/* - castingState.ResilienceCritRateReduction*/);
            float resistMultiplier = (forceHit ? 1.0f : HitRate) * PartialResistFactor;
            int targets = solver.CalculationOptions.AoeTargets;
            float averageDamage = baseAverage * SpellModifier * DirectDamageModifier * targets * (forceHit ? 1.0f : HitRate);
            if (targets > 10)
            {
                averageDamage *= 10.0f / targets;
            }
            if (solver.NeedsDisplayCalculations && (MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire) && solver.MageTalents.Ignite > 0)
            {
                igniteDamage = averageDamage * PartialResistFactor * CritBonus * solver.IgniteFactor / (1 + solver.IgniteFactor) * Math.Max(0, CritRate);
                igniteDamagePerSpellPower = 0; // we're not applying effect spell power to aoe
            }
            else
            {
                igniteDamage = 0;
                igniteDamagePerSpellPower = 0;
            }
            averageDamage = averageDamage * critMultiplier * PartialResistFactor;            
            if (BasePeriodicDamage > 0.0f)
            {
                if (spammedDot)
                {
                    averageDamage += targets * (BasePeriodicDamage + DotDamageCoefficient * spellPower) * SpellModifier * DotDamageModifier * resistMultiplier * CastTime / DotDuration;
                }
                else
                {
                    averageDamage += targets * (BasePeriodicDamage + DotDamageCoefficient * spellPower) * SpellModifier * DotDamageModifier * resistMultiplier;
                }
            }
            return averageDamage;
        }
    }

    public class Spell
    {
        public SpellId SpellId; // set in CastingState.GetSpell
        private SpellTemplate template; // set in constructor/Intitialize

        public SpellTemplate SpellTemplate { get { return template; } }

        public void Initialize(SpellTemplate template)
        {
            this.template = template;
            cycle = null;
        }

        // Variables that have to be initialized in Calculate and can be modifier between Calculate and CalculateDerivedStats
        private CastingState castingState;
        public float BaseCastTime;
        public float CostModifier;
        public float CostAmplifier;
        public float SpellModifier;
        public float AdditiveSpellModifier;
        public float DirectDamageModifier;
        public float DotDamageModifier;
        public float CritRate;
        public float CritBonus;
        public float RawSpellDamage;
        public float InterruptProtection;

        // Variables that have to be initialized in CalculateDerivedStats and can be modified after
        //public float DamagePerSecond;
        //public float ThreatPerSecond;
        //public float CostPerSecond;
        public bool SpammedDot;
        public bool Pom;
        public float Ticks;
        public float CastProcs;
        public float HitProcs;
        public float CritProcs;
        public float IgniteProcs;
        public float TargetProcs;
        public float DotProcs;
        public float ChannelReduction;
        public float CastTime;
        public float OO5SR;
        public float AverageDamage;
        public float AverageThreat;
        public float AverageCost;
        public float Absorb; // max absorb on single impact
        public float TotalAbsorb; // total absorb with combined warding negates
        //public float IgniteDamagePerSecond;
        //public float IgniteDpsPerSpellPower;
        public float IgniteDamage;
        public float IgniteDamagePerSpellPower;
        //public float DpsPerSpellPower;
        public float DamagePerSpellPower;
        // stats valid for dot spells only
        public float DotAverageDamage;
        public float DotAverageThreat;
        public float DotDamagePerSpellPower;

        public float DamagePerSecond
        {
            get
            {
                return AverageDamage / CastTime;
            }
        }

        public float ThreatPerSecond
        {
            get
            {
                return AverageThreat / CastTime;
            }
        }

        public float CostPerSecond
        {
            get
            {
                return AverageCost / CastTime;
            }
        }

        public string Label { get; set; }

        // Properties pulling data directly from template
        public string Name { get { return template.Name; } }
        public bool AreaEffect { get { return template.AreaEffect; } }
        public bool Channeled { get { return template.Channeled; } }
        public float CastProcs2 { get { return template.CastProcs2; } }
        public float NukeProcs { get { return template.NukeProcs; } }
        public bool Instant { get { return template.Instant; } }
        public int BaseCost { get { return template.BaseCost; } }
        public float BaseCooldown { get { return template.BaseCooldown; } }
        public MagicSchool MagicSchool { get { return template.MagicSchool; } }
        public float BaseMinDamage { get { return template.BaseMinDamage; } }
        public float BaseMaxDamage { get { return template.BaseMaxDamage; } }
        public float BasePeriodicDamage { get { return template.BasePeriodicDamage; } }
        public float SpellDamageCoefficient { get { return template.SpellDamageCoefficient; } }
        public float DotDamageCoefficient { get { return template.DotDamageCoefficient; } }
        public float DotDuration { get { return template.DotDuration; } }
        public float DotTickInterval { get { return template.DotTickInterval; } }
        public float Range { get { return template.Range; } }
        public float RealResistance { get { return template.RealResistance; } }
        public float ThreatMultiplier { get { return template.ThreatMultiplier; } }
        public float HitRate { get { return template.HitRate; } }
        public float PartialResistFactor { get { return template.PartialResistFactor; } }
        public float Cooldown { get { return template.Cooldown; } }       

        public float Cost
        {
            get
            {
                return (float)Math.Floor(BaseCost * CostAmplifier * CostModifier);
            }
        }

        public float ABCost
        {
            get
            {
                return (float)Math.Floor(Math.Round(BaseCost * CostAmplifier) * CostModifier);
            }
        }

        public float Latency
        {
            get
            {
                if (BaseCastTime <= 1.5f || Instant)
                {
                    return castingState.CalculationOptions.LatencyGCD;
                }
                else if (Channeled)
                {
                    return castingState.CalculationOptions.LatencyChannel;
                }
                else
                {
                    return castingState.CalculationOptions.LatencyCast;
                }
            }
        }

        public float MinHitDamage
        {
            get
            {
                return (BaseMinDamage + RawSpellDamage * SpellDamageCoefficient) * SpellModifier * DirectDamageModifier / template.Ticks;
            }
        }

        public float MaxHitDamage
        {
            get
            {
                return (BaseMaxDamage + RawSpellDamage * SpellDamageCoefficient) * SpellModifier * DirectDamageModifier / template.Ticks;
            }
        }

        public float MinCritDamage
        {
            get
            {
                return MinHitDamage * CritBonus;
            }
        }

        public float MaxCritDamage
        {
            get
            {
                return MaxHitDamage * CritBonus;
            }
        }

        public float DotDamage
        {
            get
            {
                return (BasePeriodicDamage + DotDamageCoefficient * RawSpellDamage) * SpellModifier * DotDamageModifier;
            }
        }

        public const float GlobalCooldownLimit = 1.0f;
        public const float MaxHitRate = 1.0f;

        /*private class SpellCycle : Cycle
        {
            private Spell spell;

            public SpellCycle(Spell spell) : base(spell.castingState)
            {
                this.spell = spell;
                Name = spell.Name;
                Ticks = spell.Ticks;
                CastTime = spell.CastTime;
                HitProcs = spell.HitProcs;
                CastProcs = spell.CastProcs;
                CastProcs2 = spell.CastProcs2;
                NukeProcs = spell.NukeProcs;
                CritProcs = spell.CritProcs;
                IgniteProcs = spell.IgniteProcs;
                DotProcs = spell.DotProcs;
                TargetProcs = spell.TargetProcs;
                DamageProcs = spell.HitProcs + spell.DotProcs;
                damagePerSecond = spell.AverageDamage / spell.CastTime;
                threatPerSecond = spell.AverageDamage / spell.CastTime;
                costPerSecond = spell.AverageCost / spell.CastTime;
                OO5SR = spell.OO5SR;
                AreaEffect = spell.AreaEffect;
                DpsPerSpellPower = spell.DamagePerSpellPower / spell.CastTime;
                Absorbed = spell.TotalAbsorb;
                if (AreaEffect) AoeSpell = spell;
            }

            public override void AddSpellContribution(Dictionary<string, SpellContribution> dict, float duration, float effectSpellPower)
            {
                spell.AddSpellContribution(dict, spell.CastTime * duration / CastTime, effectSpellPower);
            }

            public override void AddManaUsageContribution(Dictionary<string, float> dict, float duration)
            {
                spell.AddManaUsageContribution(dict, spell.CastTime * duration / CastTime);
            }
        }*/

        private Cycle cycle;
        public static implicit operator Cycle(Spell spell)
        {
            if (spell.cycle == null)
            {
                bool needsDisplayCalculations = spell.castingState.Solver.NeedsDisplayCalculations;
                spell.cycle = Cycle.New(needsDisplayCalculations, spell.castingState);
                spell.cycle.AddSpell(needsDisplayCalculations, spell, 1.0f);
                spell.cycle.Calculate();
                if (spell.AreaEffect)
                {
                    spell.cycle.AreaEffect = true;
                    spell.cycle.AoeSpell = spell;
                }
            }
            return spell.cycle;
        }

        public Spell()
        {
        }

        public Spell(SpellTemplate template)
        {
            this.template = template;
        }

        public static Spell New(SpellTemplate template, Solver solver)
        {
            if (solver.NeedsDisplayCalculations || solver.ArraySet == null)
            {
                return new Spell(template);
            }
            else
            {
                Spell spell = solver.ArraySet.NewSpell();
                spell.Initialize(template);
                return spell;
            }
        }

        public static Spell NewFromReference(Spell reference, CastingState castingState)
        {
            Spell s = New(reference.template, castingState.Solver);
            s.castingState = castingState;

            s.BaseCastTime = reference.BaseCastTime;
            s.CostModifier = reference.CostModifier;
            s.CostAmplifier = reference.CostAmplifier;
            s.SpellModifier = reference.SpellModifier;
            s.AdditiveSpellModifier = reference.AdditiveSpellModifier;
            s.DirectDamageModifier = reference.DirectDamageModifier;
            s.DotDamageModifier = reference.DotDamageModifier;
            s.CritRate = reference.CritRate;
            s.CritBonus = reference.CritBonus;
            s.RawSpellDamage = reference.RawSpellDamage;
            s.InterruptProtection = reference.InterruptProtection;

            s.SpammedDot = reference.SpammedDot;
            s.Pom = reference.Pom;
            s.Ticks = reference.Ticks;
            s.CastProcs = reference.CastProcs;
            s.HitProcs = reference.HitProcs;
            s.CritProcs = reference.CritProcs;
            s.IgniteProcs = reference.IgniteProcs;
            s.TargetProcs = reference.TargetProcs;
            s.DotProcs = reference.DotProcs;
            s.ChannelReduction = reference.ChannelReduction;
            s.CastTime = reference.CastTime;
            s.OO5SR = reference.OO5SR;
            s.AverageDamage = reference.AverageDamage;
            s.AverageThreat = reference.AverageThreat;
            s.AverageCost = reference.AverageCost;
            s.IgniteDamage = reference.IgniteDamage;
            s.IgniteDamagePerSpellPower = reference.IgniteDamagePerSpellPower;
            s.DamagePerSpellPower = reference.DamagePerSpellPower;
            // absorb spells
            s.Absorb = reference.Absorb;
            s.TotalAbsorb = reference.TotalAbsorb;
            // dot spells
            s.DotAverageDamage = reference.DotAverageDamage;
            s.DotAverageThreat = reference.DotAverageThreat;
            s.DotDamagePerSpellPower = reference.DotDamagePerSpellPower;

            s.RecalculateCastTime(castingState);

            return s;
        }

        public virtual void Calculate(CastingState castingState)
        {
            this.castingState = castingState;

            BaseCastTime = template.BaseCastTime;
            CostModifier = template.BaseCostModifier;
            CostAmplifier = template.BaseCostAmplifier;
            DirectDamageModifier = template.BaseDirectDamageModifier;
            DotDamageModifier = template.BaseDotDamageModifier;
            if (castingState.PowerInfusion) CostModifier -= 0.2f; // don't have any information on this, going by best guess
            if (castingState.ArcanePower) CostModifier += 0.2f;
            InterruptProtection = template.BaseInterruptProtection;

            SpellModifier = template.BaseSpellModifier * castingState.StateSpellModifier;
            AdditiveSpellModifier = template.BaseAdditiveSpellModifier + castingState.StateAdditiveSpellModifier;
            CritBonus = template.CritBonus;
            CritRate = template.BaseCritRate + castingState.StateCritRate;
            if (castingState.Combustion && (MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire))
            {
                CritRate = 3 / castingState.CombustionDuration;
                if (MagicSchool == MagicSchool.Fire)
                {
                    CritBonus = castingState.Solver.CombustionFireCritBonus;
                }
                else if (MagicSchool == MagicSchool.FrostFire)
                {
                    CritBonus = castingState.Solver.CombustionFrostFireCritBonus;
                }
            }

            switch (MagicSchool)
            {
                case MagicSchool.Arcane:
                    RawSpellDamage = castingState.ArcaneSpellPower;
                    break;
                case MagicSchool.Fire:
                    RawSpellDamage = castingState.FireSpellPower;
                    break;
                case MagicSchool.FrostFire:
                    RawSpellDamage = castingState.FrostFireSpellPower;
                    break;
                case MagicSchool.Frost:
                    RawSpellDamage = castingState.FrostSpellPower;
                    break;
                case MagicSchool.Nature:
                    RawSpellDamage = castingState.NatureSpellPower;
                    break;
                case MagicSchool.Shadow:
                    RawSpellDamage = castingState.ShadowSpellPower;
                    break;
                case MagicSchool.Holy:
                    RawSpellDamage = castingState.HolySpellPower;
                    break;
            }
        }

        public void CalculateManualClearcasting(bool manualClearcasting, bool clearcastingAveraged, bool clearcastingActive)
        {
            if (manualClearcasting && !clearcastingAveraged)
            {
                CritRate -= 0.15f * 0.02f * castingState.MageTalents.ArcaneConcentration * castingState.MageTalents.ArcanePotency; // replace averaged arcane potency with actual % chance
                if (clearcastingActive) CritRate += 0.15f * castingState.MageTalents.ArcanePotency;
            }
        }

        public static float ProcBuffUp(float procChance, float buffDuration, float triggerInterval)
        {
            if (triggerInterval <= 0)
                return 0;
            else
                return 1 - (float)Math.Pow(1 - procChance, buffDuration / triggerInterval);
        }

        public void CalculateDerivedStats(CastingState castingState)
        {
            CalculateDerivedStats(castingState, false, false, true, false, false, false, false);
        }

        public void CalculateDerivedStats(CastingState castingState, bool outOfFiveSecondRule, bool pom, bool spammedDot)
        {
            CalculateDerivedStats(castingState, outOfFiveSecondRule, pom, spammedDot, false, false, false, false);
        }

        public virtual void CalculateDerivedStats(CastingState castingState, bool outOfFiveSecondRule, bool pom, bool spammedDot, bool round, bool forceHit, bool forceMiss)
        {
            CalculateDerivedStats(castingState, outOfFiveSecondRule, pom, spammedDot, round, forceHit, forceMiss, false);
        }

        public virtual void CalculateDerivedStats(CastingState castingState, bool outOfFiveSecondRule, bool pom, bool spammedDot, bool round, bool forceHit, bool forceMiss, bool dotUptime)
        {
            MageTalents mageTalents = castingState.MageTalents;
            Stats baseStats = castingState.BaseStats;
            CalculationOptionsMage calculationOptions = castingState.CalculationOptions;

            SpellModifier *= AdditiveSpellModifier;

            if (CritRate < 0.0f) CritRate = 0.0f;
            if (CritRate > 1.0f) CritRate = 1.0f;

            Ticks = template.Ticks;
            CastProcs = template.CastProcs;
            HitProcs = Ticks * HitRate;
            CritProcs = HitProcs * CritRate;
            if ((MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire) && mageTalents.Ignite > 0)
            {
                IgniteProcs = CritProcs;
            }
            else
            {
                IgniteProcs = 0;
            }
            TargetProcs = HitProcs;

            Pom = pom;
            if (Instant) InterruptProtection = 1;

            CastTime = template.CalculateCastTime(castingState, InterruptProtection, CritRate, pom, BaseCastTime, out ChannelReduction);

            // add crit rate for on use stacking crit effects (would be better if it was computed
            // on cycle level, but right now the architecture doesn't allow that too well)
            // we'd actually need some iterations of this as cast time can depend on crit etc, just ignore that for now
            for (int i = 0; i < castingState.Solver.StackingNonHasteEffectCooldownsCount; i++)
            {
                EffectCooldown effectCooldown = castingState.Solver.StackingNonHasteEffectCooldowns[i];
                if (castingState.EffectsActive(effectCooldown.Mask))
                {
                    Stats stats = effectCooldown.SpecialEffect.Stats;
                    for (int j = 0; j < stats._rawSpecialEffectDataSize; j++)
                    {
                        SpecialEffect effect = stats._rawSpecialEffectData[j];
                        if (effect.Chance == 1f && effect.Cooldown == 0f && (effect.Trigger == Trigger.DamageSpellCrit || effect.Trigger == Trigger.SpellCrit))
                        {
                            if (effect.Stats.CritRating < 0 && effectCooldown.SpecialEffect.Stats.CritRating > 0)
                            {
                                float critScale = castingState.CalculationOptions.LevelScalingFactor / 1400f;
                                CritRate += SpecialEffect.GetAverageStackingCritRate(CastTime, effectCooldown.SpecialEffect.Duration, HitRate, CritRate, effectCooldown.SpecialEffect.Stats.CritRating * critScale, effect.Stats.CritRating * critScale, effect.MaxStack);
                                if (CritRate > 1.0f) CritRate = 1.0f;
                            }
                        }
                    }
                }
            }

            if (DotTickInterval > 0)
            {
                if (spammedDot)
                {
                    DotProcs = (float)Math.Floor(Math.Min(CastTime, DotDuration) / DotTickInterval);
                }
                else
                {
                    DotProcs = DotDuration / DotTickInterval;
                }
            }
            else
            {
                DotProcs = 0;
            }

            SpammedDot = spammedDot;
            if (Ticks > 0 && !forceMiss)
            {
                if (dotUptime)
                {
                    AverageDamage = CalculateDirectAverageDamage(castingState.Solver, RawSpellDamage, forceHit, out DamagePerSpellPower, out IgniteDamage, out IgniteDamagePerSpellPower);
                    AverageThreat = AverageDamage * ThreatMultiplier;

                    DotAverageDamage = CalculateDotAverageDamage(baseStats, calculationOptions, RawSpellDamage, forceHit, out DotDamagePerSpellPower);
                    DotAverageThreat = DotAverageDamage * ThreatMultiplier;
                }
                else
                {
                    AverageDamage = CalculateAverageDamage(castingState.Solver, RawSpellDamage, spammedDot, forceHit, out DamagePerSpellPower, out IgniteDamage, out IgniteDamagePerSpellPower);
                    AverageThreat = AverageDamage * ThreatMultiplier;
                }
            }
            else
            {
                AverageDamage = 0;
                AverageThreat = 0;
                DamagePerSpellPower = 0;
                IgniteDamage = 0;
                IgniteDamagePerSpellPower = 0;
                if (dotUptime)
                {
                    DotAverageDamage = 0;
                    DotAverageThreat = 0;
                    DotDamagePerSpellPower = 0;
                }
            }
            if (ChannelReduction != 0)
            {
                Ticks *= (1 - ChannelReduction);
                HitProcs *= (1 - ChannelReduction);
                CritProcs *= (1 - ChannelReduction);
                TargetProcs *= (1 - ChannelReduction);
                CastProcs = CastProcs2 + (CastProcs - CastProcs2) * (1 - ChannelReduction);
                AverageDamage *= (1 - ChannelReduction);
                AverageThreat *= (1 - ChannelReduction);
                DamagePerSpellPower *= (1 - ChannelReduction);
            }
            AverageCost = CalculateCost(castingState.Solver, round);

            Absorb = 0;
            TotalAbsorb = 0;

            if (outOfFiveSecondRule)
            {
                OO5SR = 1;
            }
            else
            {
                OO5SR = 0;
            }
        }

        public virtual void RecalculateCastTime(CastingState castingState)
        {
            if (ChannelReduction != 0)
            {
                Ticks /= (1 - ChannelReduction);
                HitProcs /= (1 - ChannelReduction);
                CritProcs /= (1 - ChannelReduction);
                TargetProcs /= (1 - ChannelReduction);
                CastProcs = template.CastProcs;
                AverageDamage /= (1 - ChannelReduction);
                AverageThreat /= (1 - ChannelReduction);
                DamagePerSpellPower /= (1 - ChannelReduction);
            }
            CastTime = template.CalculateCastTime(castingState, InterruptProtection, CritRate, Pom, BaseCastTime, out ChannelReduction);
            if (ChannelReduction != 0)
            {
                Ticks *= (1 - ChannelReduction);
                HitProcs *= (1 - ChannelReduction);
                CritProcs *= (1 - ChannelReduction);
                TargetProcs *= (1 - ChannelReduction);
                CastProcs = template.CastProcs2 + (template.CastProcs - template.CastProcs2) * (1 - ChannelReduction);
                AverageDamage *= (1 - ChannelReduction);
                AverageThreat *= (1 - ChannelReduction);
                DamagePerSpellPower *= (1 - ChannelReduction);
            }
        }

        public virtual float CalculateAverageDamage(Solver solver, float spellPower, bool spammedDot, bool forceHit, out float damagePerSpellPower, out float igniteDamage, out float igniteDamagePerSpellPower)
        {
            float baseAverage = (BaseMinDamage + BaseMaxDamage) / 2f;
            float critMultiplier = 1 + (CritBonus - 1) * Math.Max(0, CritRate/* - castingState.ResilienceCritRateReduction*/);
            float resistMultiplier = (forceHit ? 1.0f : HitRate) * PartialResistFactor;
            float commonMultiplier = SpellModifier * resistMultiplier;
            float nukeMultiplier = commonMultiplier * DirectDamageModifier * critMultiplier;
            float averageDamage = baseAverage * nukeMultiplier;
            damagePerSpellPower = SpellDamageCoefficient * nukeMultiplier;
            if (solver.NeedsDisplayCalculations && (MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire) && solver.MageTalents.Ignite > 0)
            {
                float igniteMultiplier = commonMultiplier * DirectDamageModifier * CritBonus * solver.IgniteFactor / (1 + solver.IgniteFactor) * Math.Max(0, CritRate);
                igniteDamage = (baseAverage + SpellDamageCoefficient * spellPower) * igniteMultiplier;
                igniteDamagePerSpellPower = SpellDamageCoefficient * igniteMultiplier;
            }
            else
            {
                igniteDamage = 0;
                igniteDamagePerSpellPower = 0;
            }            
            if (BasePeriodicDamage > 0.0f)
            {
                float dotFactor = commonMultiplier * DotDamageModifier;
                if (spammedDot)
                {
                    dotFactor *= Math.Min((float)Math.Floor(CastTime / DotTickInterval) * DotTickInterval / DotDuration, 1.0f);
                }
                averageDamage += BasePeriodicDamage * dotFactor;
                damagePerSpellPower += DotDamageCoefficient * dotFactor;
            }
            return averageDamage + damagePerSpellPower * spellPower;
        }

        public virtual float CalculateDirectAverageDamage(Solver solver, float spellPower, bool forceHit, out float damagePerSpellPower, out float igniteDamage, out float igniteDamagePerSpellPower)
        {
            float baseAverage = (BaseMinDamage + BaseMaxDamage) / 2f;
            float critMultiplier = 1 + (CritBonus - 1) * Math.Max(0, CritRate/* - castingState.ResilienceCritRateReduction*/);
            float resistMultiplier = (forceHit ? 1.0f : HitRate) * PartialResistFactor;
            float commonMultiplier = SpellModifier * resistMultiplier;
            float nukeMultiplier = commonMultiplier * DirectDamageModifier * critMultiplier;
            float averageDamage = baseAverage * nukeMultiplier;
            damagePerSpellPower = SpellDamageCoefficient * nukeMultiplier;
            if (solver.NeedsDisplayCalculations && (MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire) && solver.MageTalents.Ignite > 0)
            {
                float igniteMultiplier = commonMultiplier * DirectDamageModifier * CritBonus * solver.IgniteFactor / (1 + solver.IgniteFactor) * Math.Max(0, CritRate);
                igniteDamage = (baseAverage + SpellDamageCoefficient * spellPower) * igniteMultiplier;
                igniteDamagePerSpellPower = SpellDamageCoefficient * igniteMultiplier;
            }
            else
            {
                igniteDamage = 0;
                igniteDamagePerSpellPower = 0;
            }
            return averageDamage + damagePerSpellPower * spellPower;
        }

        public virtual float CalculateDotAverageDamage(Stats baseStats, CalculationOptionsMage calculationOptions, float spellPower, bool forceHit, out float damagePerSpellPower)
        {
            float resistMultiplier = (forceHit ? 1.0f : HitRate) * PartialResistFactor;
            float commonMultiplier = SpellModifier * resistMultiplier;
            float averageDamage = 0.0f;
            damagePerSpellPower = 0.0f;
            if (BasePeriodicDamage > 0.0f)
            {
                float dotFactor = commonMultiplier * DotDamageModifier;
                averageDamage = BasePeriodicDamage * dotFactor;
                damagePerSpellPower = DotDamageCoefficient * dotFactor;
            }
            return averageDamage + damagePerSpellPower * spellPower;
        }

        protected float CalculateCost(Solver solver, bool round)
        {
            float cost;
            if (round)
            {
                cost = (float)Math.Floor(Math.Round(BaseCost * CostAmplifier) * CostModifier);
            }
            else
            {
                cost = (float)Math.Floor(BaseCost * CostAmplifier * CostModifier);
            }

            if (MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire) cost += CritRate * cost * 0.01f * solver.MageTalents.Burnout; // last I read Burnout works on final pre MOE cost

            cost *= (1 - 0.02f * solver.MageTalents.ArcaneConcentration);

            // from what I know MOE works on base cost
            // not tested, but I think if you get MOE proc on a spell while CC is active you still get mana return
            if (!solver.CalculationOptions.EffectDisableManaSources)
            {
                cost -= CritRate * BaseCost * 0.1f * solver.MageTalents.MasterOfElements;
                // Judgement of Wisdom
                // this is actually a PPM
                cost -= template.BaseUntalentedCastTime / 60f * solver.BaseStats.ManaRestoreFromBaseManaPPM * 3268;
            }
            return cost;
        }

        public void CalculateManualClearcastingCost(Solver solver, bool round, bool manualClearcasting, bool clearcastingAveraged, bool clearcastingActive)
        {
            float cost;
            if (round)
            {
                cost = (float)Math.Floor(Math.Round(BaseCost * CostAmplifier) * CostModifier);
            }
            else
            {
                cost = (float)Math.Floor(BaseCost * CostAmplifier * CostModifier);
            }

            if (MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire) cost += CritRate * cost * 0.01f * solver.MageTalents.Burnout; // last I read Burnout works on final pre MOE cost

            if (!manualClearcasting || clearcastingAveraged)
            {
                cost *= (1 - 0.02f * solver.MageTalents.ArcaneConcentration);
            }
            else if (clearcastingActive)
            {
                cost = 0;
            }

            // from what I know MOE works on base cost
            // not tested, but I think if you get MOE proc on a spell while CC is active you still get mana return
            if (!solver.CalculationOptions.EffectDisableManaSources)
            {
                cost -= CritRate * BaseCost * 0.1f * solver.MageTalents.MasterOfElements;
                // Judgement of Wisdom
                // this is actually a PPM
                cost -= template.BaseUntalentedCastTime / 60f * solver.BaseStats.ManaRestoreFromBaseManaPPM * 3268;
            }
            AverageCost = cost;
        }

        public void AddSpellContribution(Dictionary<string, SpellContribution> dict, float duration, float dotUptime, float effectSpellPower)
        {
            SpellContribution contrib;
            if (!dict.TryGetValue(Name, out contrib))
            {
                contrib = new SpellContribution() { Name = Name };
                dict[Name] = contrib;
            }
            float igniteContribution = 0;
            float critBonus = CritBonus;
            if (IgniteDamage > 0)
            {
                igniteContribution = (IgniteDamage + effectSpellPower * IgniteDamagePerSpellPower) / CastTime * duration;
                SpellContribution igniteContrib;
                if (!dict.TryGetValue("Ignite", out igniteContrib))
                {
                    igniteContrib = new SpellContribution() { Name = "Ignite" };
                    dict["Ignite"] = igniteContrib;
                }
                igniteContrib.Damage += igniteContribution;
                critBonus /= (1 + castingState.Solver.IgniteFactor);
            }
            contrib.Hits += (HitProcs - CritProcs) * duration / CastTime;
            contrib.Crits += CritProcs * duration / CastTime;
            float damage = (AverageDamage + effectSpellPower * DamagePerSpellPower) / CastTime * duration - igniteContribution;
            contrib.Damage += damage;
            if (dotUptime > 0)
            {
                float tickDamage = dotUptime * (DotAverageDamage + effectSpellPower * DotDamagePerSpellPower) / CastTime * duration;
                contrib.Damage += tickDamage;
                // dotUptime = DotProcs / (DotDuration / DotTickInterval)
                contrib.Ticks += dotUptime * (DotDuration / DotTickInterval) * duration / CastTime;
                contrib.TickDamage += tickDamage;

            }
            else
            {
                if (DotTickInterval > 0)
                {
                    contrib.Ticks += DotProcs * duration / CastTime;
                    float dotFactor = DotProcs / (DotDuration / DotTickInterval) * SpellModifier * DotDamageModifier * PartialResistFactor;
                    float tickDamage = dotFactor * (BasePeriodicDamage + (RawSpellDamage + effectSpellPower) * DotDamageCoefficient) / CastTime * duration;
                    contrib.TickDamage += tickDamage;
                    damage -= tickDamage;
                }
            }
            // damage = baseDamage * (1 + (CritBonus - 1) * CritRate)
            float baseDamage = damage / (1 + (critBonus - 1) * CritRate);
            contrib.HitDamage += baseDamage * (1 - CritRate);
            contrib.CritDamage += baseDamage * CritRate * critBonus;
            contrib.Range = Range;
        }

        public void AddManaUsageContribution(Dictionary<string, float> dict, float duration)
        {
            float contrib;
            dict.TryGetValue(Name, out contrib);
            contrib += AverageCost / CastTime * duration;
            dict[Name] = contrib;
        }
    }
}
