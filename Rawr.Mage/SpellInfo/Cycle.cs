using System;
using System.Collections.Generic;
#if RAWR3
using System.Linq;
#endif
using System.Text;

namespace Rawr.Mage
{
    public enum CycleId
    {
        None,
        ArcaneMissiles,
        Scorch,
        FrostboltFOF,
        Fireball,
        FrostfireBoltFOF,
        ArcaneBlastSpam,
        ABABarSc,
        ABABarCSc,
        ABAMABarSc,
        AB3AMABarSc,
        AB3ABarCSc,
        AB3MBAMABarSc,
        ABarAM,
        ABP,
        ABAM,
        ABABar,
        ABABar3C,
        ABABar2C,
        ABABar2MBAM,
        ABABar1MBAM,
        ABABar0MBAM,
        ABSpamMBAM,
        ABSpam03C,
        ABSpam3C,
        ABSpam03MBAM,
        ABSpam3MBAM,
        ABAMABar,
        AB2AMABar,
        AB32AMABar,
        AB3ABar3MBAM,
        AB2AM,
        AB3AM,
        AB3AM23MBAM,
        AB4AM234MBAM,
        AB3AM023MBAM,
        AB4AM0234MBAM,
        ABSpam04MBAM,
        ABSpam024MBAM,
        ABSpam034MBAM,
        ABSpam0234MBAM,
        ABSpam4MBAM,
        ABSpam24MBAM,
        ABSpam234MBAM,
        AB3AM2MBAM,
        ABABar0C,
        ABABar1C,
        ABABarY,
        AB2ABar,
        AB2ABarMBAM,
        AB2ABar2C,
        AB2ABar2MBAM,
        AB2ABar3C,
        AB3AMABar,
        AB3AMABar2C,
        AB3ABar,
        AB3ABar3C,
        AB3ABarX,
        AB3ABarY,
        FBABar,
        FrBABar,
        FFBABar,
        //ABAMP,
        //AB3AMSc,
        //ABAM3Sc,
        //ABAM3Sc2,
        //ABAM3FrB,
        //ABAM3FrB2,
        //ABFrB,
        //AB3FrB,
        //ABFrB3FrB,
        //ABFrB3FrB2,
        //ABFrB3FrBSc,
        //ABFB3FBSc,
        //AB3Sc,
        FB2ABar,
        FrB2ABar,
        ScLBPyro,
        FrBFB,
        FrBIL,
        FrBILFB,
        FrBDFFBIL,
        FrBDFFFB,
        FrBFBIL,
        FrBFFB,
        FBSc,
        FBFBlast,
        FBPyro,
        FBLBPyro,
        FFBLBPyro,
        FFBPyro,
        FBScPyro,
        FFBScPyro,
        FBScLBPyro,
        FFBScLBPyro,
        ABABarSlow,
        FBABarSlow,
        FrBABarSlow,
        /*ABAM3ScCCAM,
        ABAM3Sc2CCAM,
        ABAM3FrBCCAM,
        ABAM3FrBCCAMFail,
        ABAM3FrBScCCAM,
        ABAMCCAM,
        ABAM3CCAM,*/
        CustomSpellMix,
        ArcaneExplosion,
        FlamestrikeSpammed,
        FlamestrikeSingle,
        Blizzard,
        BlastWave,
        DragonsBreath,
        ConeOfCold
    }

    public class Cycle
    {
        public string Name;
        public CycleId CycleId;

        public override string ToString()
        {
            return Name;
        }

        public CastingState CastingState;

        private struct SpellData
        {
            public Spell Spell;
            public float Weight;
            public float DotUptime;
        }

        private List<SpellData> Spell;

        public Cycle()
        {
        }

        public Cycle(bool needsDisplayCalculations, CastingState castingState)
        {
            CastingState = castingState;
            if (needsDisplayCalculations)
            {
                Spell = new List<SpellData>();
            }
        }

        public static Cycle New(bool needsDisplayCalculations, CastingState castingState)
        {
            ArraySet arraySet = castingState.Solver.ArraySet;
            if (needsDisplayCalculations || arraySet == null)
            {
                return new Cycle(needsDisplayCalculations, castingState);
            }
            else
            {
                Cycle cycle = arraySet.NewCycle();
                cycle.Initialize(castingState);
                return cycle;
            }
        }

        public void AddCycle(bool needsDisplayCalculations, Cycle cycle, float weight)
        {
            if (needsDisplayCalculations)
            {
                AddSpellsFromCycle(cycle, weight);
            }
            CastTime += weight * cycle.CastTime;
            CastProcs += weight * cycle.CastProcs;
            CastProcs2 += weight * cycle.CastProcs2;
            NukeProcs += weight * cycle.NukeProcs;
            Ticks += weight * cycle.Ticks;
            HitProcs += weight * cycle.HitProcs;
            CritProcs += weight * cycle.CritProcs;
            IgniteProcs += weight * cycle.IgniteProcs;
            DotProcs += weight * cycle.DotProcs;
            TargetProcs += weight * cycle.TargetProcs;
            DamageProcs += weight * cycle.DamageProcs;
            Absorbed += weight * cycle.Absorbed;
            costPerSecond += weight * cycle.CastTime * cycle.costPerSecond;
            damagePerSecond += weight * cycle.CastTime * cycle.damagePerSecond;
            threatPerSecond += weight * cycle.CastTime * cycle.threatPerSecond;
            DpsPerSpellPower += weight * cycle.CastTime * cycle.DpsPerSpellPower;
        }

        private void AddSpellsFromCycle(Cycle cycle, float weight)
        {
            foreach (var spell in cycle.Spell)
            {
                Spell.Add(new SpellData() { Spell = spell.Spell, DotUptime = spell.DotUptime, Weight = weight * spell.Weight });
            }
        }

        public void AddSpell(bool needsDisplayCalculations, Spell spell, float weight)
        {
            if (needsDisplayCalculations)
            {
                Spell.Add(new SpellData() { Spell = spell, Weight = weight });
            }
            CastTime += weight * spell.CastTime;
            CastProcs += weight * spell.CastProcs;
            CastProcs2 += weight * spell.CastProcs2;
            NukeProcs += weight * spell.NukeProcs;
            Ticks += weight * spell.Ticks;
            HitProcs += weight * spell.HitProcs;
            CritProcs += weight * spell.CritProcs;
            IgniteProcs += weight * spell.IgniteProcs;
            DotProcs += weight * spell.DotProcs;
            TargetProcs += weight * spell.TargetProcs;
            DamageProcs += weight * (spell.HitProcs + spell.DotProcs);
            Absorbed += weight * spell.TotalAbsorb;
            costPerSecond += weight * spell.AverageCost;
            damagePerSecond += weight * spell.AverageDamage;
            threatPerSecond += weight * spell.AverageThreat;
            DpsPerSpellPower += weight * spell.DamagePerSpellPower;
        }

        public void AddSpell(bool needsDisplayCalculations, Spell spell, float weight, float dotUptime)
        {
            if (needsDisplayCalculations)
            {
                Spell.Add(new SpellData() { Spell = spell, Weight = weight, DotUptime = dotUptime });
            }
            CastTime += weight * spell.CastTime;
            CastProcs += weight * spell.CastProcs;
            CastProcs2 += weight * spell.CastProcs2;
            NukeProcs += weight * spell.NukeProcs;
            Ticks += weight * spell.Ticks;
            HitProcs += weight * spell.HitProcs;
            CritProcs += weight * spell.CritProcs;
            IgniteProcs += weight * spell.IgniteProcs;
            DotProcs += weight * dotUptime * spell.DotProcs;
            TargetProcs += weight * spell.TargetProcs;
            DamageProcs += weight * (spell.HitProcs + dotUptime * spell.DotProcs);
            Absorbed += weight * spell.TotalAbsorb;
            costPerSecond += weight * spell.AverageCost;
            damagePerSecond += weight * (spell.AverageDamage + dotUptime * spell.DotAverageDamage);
            threatPerSecond += weight * (spell.AverageThreat + dotUptime * spell.DotAverageThreat);
            DpsPerSpellPower += weight * (spell.DamagePerSpellPower + dotUptime * spell.DotDamagePerSpellPower);
        }

        public void AddPause(float duration, float weight)
        {
            CastTime += weight * duration;
        }

        public void Calculate()
        {
            costPerSecond /= CastTime;
            damagePerSecond /= CastTime;
            threatPerSecond /= CastTime;
            DpsPerSpellPower /= CastTime;
        }

        public virtual void AddSpellContribution(Dictionary<string, SpellContribution> dict, float duration, float effectSpellPower)
        {
            foreach (var spell in Spell)
            {
                spell.Spell.AddSpellContribution(dict, spell.Weight * spell.Spell.CastTime / CastTime * duration, spell.DotUptime, effectSpellPower);
            }
        }

        public virtual void AddManaUsageContribution(Dictionary<string, float> dict, float duration)
        {
            foreach (var spell in Spell)
            {
                spell.Spell.AddManaUsageContribution(dict, spell.Weight * spell.Spell.CastTime / CastTime * duration);
            }
        }

        public void Initialize(CastingState castingState)
        {
            CastingState = castingState;
            calculated = false;
            damagePerSecond = 0;
            effectDamagePerSecond = 0;
            effectSpellPower = 0;
            threatPerSecond = 0;
            effectThreatPerSecond = 0;
            costPerSecond = 0;
            manaRegenPerSecond = 0;
            DpsPerSpellPower = 0;
            Absorbed = 0;

            ProvidesSnare = false;
            ProvidesScorch = false;

            AreaEffect = false;
            AoeSpell = null;

            HitProcs = 0;
            Ticks = 0;
            CastProcs = 0;
            CastProcs2 = 0;
            NukeProcs = 0;
            CritProcs = 0;
            IgniteProcs = 0;
            DotProcs = 0;
            CastTime = 0;
            TargetProcs = 0;
            DamageProcs = 0;
            OO5SR = 0;
        }

        private bool calculated;

        internal float damagePerSecond;
        internal float effectDamagePerSecond;
        internal float effectSpellPower;
        public float DamagePerSecond
        {
            get
            {
                CalculateEffects();
                return damagePerSecond + effectDamagePerSecond;
            }
        }

        public float GetDamagePerSecond(float manaAdeptBonus)
        {
            CalculateEffects();
            return damagePerSecond * (1 + manaAdeptBonus) + effectDamagePerSecond;
        }

        public float GetSpellDamagePerSecond()
        {
            CalculateEffects();
            return damagePerSecond;
        }

        internal float threatPerSecond;
        internal float effectThreatPerSecond;
        public float ThreatPerSecond
        {
            get
            {
                CalculateEffects();
                return threatPerSecond + effectThreatPerSecond;
            }
        }

        internal float costPerSecond;
        public float CostPerSecond
        {
            get
            {
                CalculateEffects();
                return costPerSecond;
            }
        }

        private float manaRegenPerSecond;
        public float ManaRegenPerSecond
        {
            get
            {
                CalculateEffects();
                return manaRegenPerSecond;
            }
        }

        public float ManaPerSecond
        {
            get
            {
                return CostPerSecond - ManaRegenPerSecond;
            }
        }

        public float Absorbed;
        public float DpsPerSpellPower;

        public bool ProvidesSnare;
        public bool ProvidesScorch;

        public bool AreaEffect;
        public Spell AoeSpell;

        public float HitProcs;
        public float Ticks;
        public float CastProcs;
        public float CastProcs2; // variant with only one proc from AM
        public float NukeProcs;
        public float CritProcs;
        public float DotProcs;
        public float IgniteProcs;
        public float CastTime;
        public float TargetProcs;
        public float DamageProcs;
        public float OO5SR;

        public void AddDamageContribution(Dictionary<string, SpellContribution> dict, float duration)
        {
            AddSpellContribution(dict, duration, effectSpellPower);
            AddEffectContribution(dict, duration);
        }

        private void CalculateEffects()
        {
            if (!calculated)
            {
                CalculateIgniteDamageProcs();
                CalculateManaRegen();
                CalculateEffectDamage();
                calculated = true;
            }
        }

        private void CalculateIgniteDamageProcs()
        {
            if (IgniteProcs > 0)
            {
                double rate = IgniteProcs / CastTime;
                double k = Math.Exp(-2 * rate);
                double ticks = k * (1 + k);
                DamageProcs += (float)(IgniteProcs * ticks);
                DotProcs += (float)(IgniteProcs * ticks);
            }
        }

        private void CalculateEffectDamage()
        {
            Stats baseStats = CastingState.BaseStats;
            float spellPower = 0;
            if (Ticks > 0)
            {
                for (int i = 0; i < CastingState.Solver.SpellPowerEffectsCount; i++)
                {
                    SpecialEffect effect = CastingState.Solver.SpellPowerEffects[i];
                    switch (effect.Trigger)
                    {
                        case Trigger.DamageSpellCrit:
                        case Trigger.SpellCrit:
                            spellPower += effect.Stats.SpellPower * effect.GetAverageUptime(CastTime / Ticks, CritProcs / Ticks, 3, CastingState.CalculationOptions.FightDuration);
                            break;
                        case Trigger.DamageSpellHit:
                        case Trigger.SpellHit:
                            spellPower += effect.Stats.SpellPower * effect.GetAverageUptime(CastTime / Ticks, HitProcs / Ticks, 3, CastingState.CalculationOptions.FightDuration);
                            break;
                        case Trigger.SpellMiss:
                            spellPower += effect.Stats.SpellPower * effect.GetAverageUptime(CastTime / Ticks, 1 - HitProcs / Ticks, 3, CastingState.CalculationOptions.FightDuration);
                            break;
                        case Trigger.DamageSpellCast:
                        case Trigger.SpellCast:
                            if (CastProcs > 0)
                            {
                                spellPower += effect.Stats.SpellPower * effect.GetAverageUptime(CastTime / CastProcs, 1, 3, CastingState.CalculationOptions.FightDuration);
                            }
                            break;
                        case Trigger.MageNukeCast:
                            if (NukeProcs > 0)
                            {
                                spellPower += effect.Stats.SpellPower * effect.GetAverageUptime(CastTime / NukeProcs, 1, 3, CastingState.CalculationOptions.FightDuration);
                            }
                            break;
                        case Trigger.DamageDone:
                        case Trigger.DamageOrHealingDone:
                            if (DamageProcs > 0)
                            {
                                spellPower += effect.Stats.SpellPower * effect.GetAverageUptime(CastTime / DamageProcs, 1, 3, CastingState.CalculationOptions.FightDuration);
                            }
                            break;
                        case Trigger.DoTTick:
                            if (DotProcs > 0)
                            {
                                spellPower += effect.Stats.SpellPower * effect.GetAverageUptime(CastTime / DotProcs, 1, 3, CastingState.CalculationOptions.FightDuration);
                            }
                            break;
                    }
                }
            }
            if (CastingState.MageTalents.IncantersAbsorption > 0)
            {
                //float incanterSpellPower = Math.Min((float)Math.Min(calculationOptions.AbsorptionPerSecond, calculationResult.IncomingDamageDps) * 0.05f * talents.IncantersAbsorption * 10, 0.05f * baseStats.Health);
                spellPower += Absorbed / CastTime * 0.05f * CastingState.MageTalents.IncantersAbsorption * 10;
                //spellPower += Math.Min((float)Math.Min(CastingState.CalculationOptions.AbsorptionPerSecond + Absorbed / CastTime, CastingState.Calculations.IncomingDamageDps) * 0.05f * CastingState.MageTalents.IncantersAbsorption * 10, 0.05f * baseStats.Health);
            }
            effectSpellPower = spellPower;
            effectDamagePerSecond += spellPower * DpsPerSpellPower;
            //effectThreatPerSecond += spellPower * TpsPerSpellPower; // do we really need more threat calculations???
            if (CastingState.WaterElemental)
            {
                Spell waterbolt = CastingState.GetSpell(SpellId.Waterbolt);
                effectDamagePerSecond += (waterbolt.AverageDamage + spellPower * waterbolt.DamagePerSpellPower) / waterbolt.CastTime;
            }
            if (CastingState.MirrorImage)
            {
                Spell mirrorImage = CastingState.GetSpell(SpellId.MirrorImage);
                effectDamagePerSecond += (mirrorImage.AverageDamage + spellPower * mirrorImage.DamagePerSpellPower) / mirrorImage.CastTime;
            }
            if (Ticks > 0)
            {
                for (int i = 0; i < CastingState.Solver.DamageProcEffectsCount; i++)
                {
                    SpecialEffect effect = CastingState.Solver.DamageProcEffects[i];
                    float chance = 0;
                    float interval = 0;
                    switch (effect.Trigger)
                    {
                        case Trigger.SpellCrit:
                        case Trigger.DamageSpellCrit:
                            chance = CritProcs / Ticks;
                            // aoe modifier
                            if (TargetProcs > HitProcs)
                            {
                                chance = 1f - (float)Math.Pow(1 - chance, TargetProcs / HitProcs);
                            }
                            interval = CastTime / Ticks;
                            break;
                        case Trigger.SpellHit:
                        case Trigger.DamageSpellHit:
                            chance = HitProcs / Ticks;
                            // aoe modifier
                            if (TargetProcs > HitProcs)
                            {
                                chance = 1f - (float)Math.Pow(1 - chance, TargetProcs / HitProcs);
                            }
                            interval = CastTime / Ticks;
                            break;
                        case Trigger.DamageDone:
                        case Trigger.DamageOrHealingDone:
                            chance = 1;
                            interval = CastTime / DamageProcs;
                            break;
                        case Trigger.DoTTick:
                            chance = 1;
                            interval = CastTime / DotProcs;
                            break;
                        case Trigger.SpellCast:
                        case Trigger.DamageSpellCast:
                            chance = 1;
                            if (effect.Stats.ValkyrDamage > 0)
                            {
                                interval = CastTime / CastProcs2;
                            }
                            else
                            {
                                interval = CastTime / CastProcs;
                            }
                            break;
                    }
                    float effectsPerSecond = effect.GetAverageProcsPerSecond(interval, chance, 3f, CastingState.CalculationOptions.FightDuration);
                    if (effect.Stats.ArcaneDamage > 0)
                    {
                        float boltDps = CastingState.ArcaneAverageDamage * effect.Stats.ArcaneDamage * effectsPerSecond;
                        effectDamagePerSecond += boltDps;
                        effectThreatPerSecond += boltDps * CastingState.ArcaneThreatMultiplier;
                    }
                    if (effect.Stats.FireDamage > 0)
                    {
                        float boltDps = CastingState.FireAverageDamage * effect.Stats.FireDamage * effectsPerSecond;
                        effectDamagePerSecond += boltDps;
                        effectThreatPerSecond += boltDps * CastingState.FireThreatMultiplier;
                    }
                    if (effect.Stats.FrostDamage > 0)
                    {
                        float boltDps = CastingState.FrostAverageDamage * effect.Stats.FrostDamage * effectsPerSecond;
                        effectDamagePerSecond += boltDps;
                        effectThreatPerSecond += boltDps * CastingState.FrostThreatMultiplier;
                    }
                    if (effect.Stats.ShadowDamage > 0)
                    {
                        float boltDps = CastingState.ShadowAverageDamage * effect.Stats.ShadowDamage * effectsPerSecond;
                        effectDamagePerSecond += boltDps;
                        effectThreatPerSecond += boltDps * CastingState.ShadowThreatMultiplier;
                    }
                    if (effect.Stats.NatureDamage > 0)
                    {
                        float boltDps = CastingState.NatureAverageDamage * effect.Stats.NatureDamage * effectsPerSecond;
                        effectDamagePerSecond += boltDps;
                        effectThreatPerSecond += boltDps * CastingState.NatureThreatMultiplier;
                    }
                    if (effect.Stats.HolyDamage > 0)
                    {
                        float boltDps = CastingState.HolyAverageDamage * effect.Stats.HolyDamage * effectsPerSecond;
                        effectDamagePerSecond += boltDps;
                        effectThreatPerSecond += boltDps * CastingState.HolyThreatMultiplier;
                    }
                    if (effect.Stats.ValkyrDamage > 0)
                    {
                        float boltDps = CastingState.ValkyrAverageDamage * effect.Stats.ValkyrDamage * effectsPerSecond;
                        effectDamagePerSecond += boltDps;
                        effectThreatPerSecond += boltDps * CastingState.HolyThreatMultiplier;
                    }
                }
            }
            /*if (baseStats.LightningCapacitorProc > 0)
            {
                //discrete model
                int hitsInsideCooldown = (int)(2.5f / (CastTime / Ticks));
                float avgCritsPerHit = CritProcs / Ticks * TargetProcs / HitProcs;
                float avgHitsToDischarge = 3f / avgCritsPerHit;
                if (avgHitsToDischarge < 1) avgHitsToDischarge = 1;
                float boltDps = CastingState.LightningBoltAverageDamage / ((CastTime / Ticks) * (hitsInsideCooldown + avgHitsToDischarge));
                effectDamagePerSecond += boltDps;
                effectThreatPerSecond += boltDps * CastingState.NatureThreatMultiplier;
                //continuous model
                //DamagePerSecond += LightningBolt.AverageDamage / (2.5f + 3f * CastTime / (CritRate * TargetProcs));
            }
            if (baseStats.ThunderCapacitorProc > 0)
            {
                //discrete model
                int hitsInsideCooldown = (int)(2.5f / (CastTime / Ticks));
                float avgCritsPerHit = CritProcs / Ticks * TargetProcs / HitProcs;
                float avgHitsToDischarge = 4f / avgCritsPerHit;
                if (avgHitsToDischarge < 1) avgHitsToDischarge = 1;
                float boltDps = CastingState.ThunderBoltAverageDamage / ((CastTime / Ticks) * (hitsInsideCooldown + avgHitsToDischarge));
                effectDamagePerSecond += boltDps;
                effectThreatPerSecond += boltDps * CastingState.NatureThreatMultiplier;
                //continuous model
                //DamagePerSecond += LightningBolt.AverageDamage / (2.5f + 4f * CastTime / (CritRate * TargetProcs));
            }*/
            /*if (baseStats.PendulumOfTelluricCurrentsProc > 0)
            {
                float boltDps = CastingState.PendulumOfTelluricCurrentsAverageDamage / (45f + CastTime / HitProcs / 0.15f);
                effectDamagePerSecond += boltDps;
                effectThreatPerSecond += boltDps * CastingState.ShadowThreatMultiplier;
            }*/
        }

        private void CalculateManaRegen()
        {
            if (CastingState.CalculationOptions.EffectDisableManaSources) return;
            Stats baseStats = CastingState.BaseStats;
            manaRegenPerSecond = CastingState.ManaRegen5SR + OO5SR * (CastingState.ManaRegen - CastingState.ManaRegen5SR);
            float fight = CastingState.CalculationOptions.FightDuration;
            for (int i = 0; i < CastingState.Solver.ManaRestoreEffectsCount; i++)
            {
                SpecialEffect effect = CastingState.Solver.ManaRestoreEffects[i];
                switch (effect.Trigger)
                {
                    case Trigger.Use:
                        manaRegenPerSecond += effect.Stats.ManaRestore * effect.GetAverageProcsPerSecond(0, 1, 3, fight);
                        break;
                    case Trigger.DamageSpellCast:
                    case Trigger.SpellCast:
                        if (CastProcs > 0)
                        {
                            manaRegenPerSecond += effect.Stats.ManaRestore * effect.GetAverageProcsPerSecond(CastTime / CastProcs, 1, 3, fight);
                        }
                        break;
                    case Trigger.DamageSpellCrit:
                    case Trigger.SpellCrit:
                        if (Ticks > 0)
                        {
                            manaRegenPerSecond += effect.Stats.ManaRestore * effect.GetAverageProcsPerSecond(CastTime / Ticks, CritProcs / Ticks, 3, fight);
                        }
                        break;
                    case Trigger.DamageSpellHit:
                    case Trigger.SpellHit:
                        if (Ticks > 0)
                        {
                            manaRegenPerSecond += effect.Stats.ManaRestore * effect.GetAverageProcsPerSecond(CastTime / Ticks, HitProcs / Ticks, 3, fight);
                        }
                        break;
                    case Trigger.DamageDone:
                    case Trigger.DamageOrHealingDone:
                        if (DamageProcs > 0)
                        {
                            manaRegenPerSecond += effect.Stats.ManaRestore * effect.GetAverageProcsPerSecond(CastTime / DamageProcs, 1, 3, fight);
                        }
                        break;
                    case Trigger.DoTTick:
                        if (DotProcs > 0)
                        {
                            manaRegenPerSecond += effect.Stats.ManaRestore * effect.GetAverageProcsPerSecond(CastTime / DotProcs, 1, 3, fight);
                        }
                        break;
                }
            }
            for (int i = 0; i < CastingState.Solver.Mp5EffectsCount; i++)
            {
                SpecialEffect effect = CastingState.Solver.Mp5Effects[i];
                switch (effect.Trigger)
                {
                    case Trigger.Use:
                        manaRegenPerSecond += effect.Stats.Mp5 / 5f * effect.GetAverageUptime(0f, 1f, 3, fight);
                        break;
                    case Trigger.DamageSpellCast:
                    case Trigger.SpellCast:
                        if (CastProcs > 0)
                        {
                            manaRegenPerSecond += effect.Stats.Mp5 / 5f * effect.GetAverageUptime(CastTime / CastProcs, 1f, 3, fight);
                        }
                        break;
                    case Trigger.DamageSpellCrit:
                    case Trigger.SpellCrit:
                        if (Ticks > 0)
                        {
                            manaRegenPerSecond += effect.Stats.Mp5 / 5f * effect.GetAverageUptime(CastTime / Ticks, CritProcs / Ticks, 3, fight);
                        }
                        break;
                    case Trigger.DamageSpellHit:
                    case Trigger.SpellHit:
                        if (Ticks > 0)
                        {
                            manaRegenPerSecond += effect.Stats.Mp5 / 5f * effect.GetAverageUptime(CastTime / Ticks, HitProcs / Ticks, 3, fight);
                        }
                        break;
                    case Trigger.DamageDone:
                    case Trigger.DamageOrHealingDone:
                        if (DamageProcs > 0)
                        {
                            manaRegenPerSecond += effect.Stats.Mp5 / 5f * effect.GetAverageUptime(CastTime / DamageProcs, 1f, 3, fight);
                        }
                        break;
                    case Trigger.DoTTick:
                        if (DotProcs > 0)
                        {
                            manaRegenPerSecond += effect.Stats.Mp5 / 5f * effect.GetAverageUptime(CastTime / DotProcs, 1f, 3, fight);
                        }
                        break;
                }
            }
            //threatPerSecond += (baseStats.ManaRestoreFromBaseManaPPM * 3268 / CastTime * HitProcs) * 0.5f * (1 + baseStats.ThreatIncreaseMultiplier) * (1 - baseStats.ThreatReductionMultiplier);
            // 3.2 mode Empowered Fire ignite return
            if (IgniteProcs > 0 && CastingState.MageTalents.EmpoweredFire > 0)
            {
                // on average we have IgniteProcs per CastTime
                double rate = IgniteProcs / CastTime;
                // using an exponential distribution approximation for time between ignite procs
                // we obtain chances for 0, 1 or 2 ignite ticks from each ignite
                // using cummulative distribution function
                // Pr(T <= 2) = 1 - e ^ -2*rate
                // Pr(T <= 4) = 1 - e ^ -4*rate
                // Pr(T >= 4) = e ^ -4*rate
                // number of ticks from an ignite proc is then
                // 2 * e ^ -4*rate + 1 * (1 - e ^ -4*rate - (1 - e ^ -2*rate))
                // = 2 * e ^ -4*rate + e ^ -2*rate - e ^ -4*rate
                // = e ^ -2*rate * (1 + e ^ -2*rate)
                // an alternative would be to use geometric distribution approximation instead
                // it is not clear which one more closely matches real data
                double k = Math.Exp(-2 * rate);
                double ticks = k * (1 + k);
                // we now obtain average number of ticks per second
                // as average number of ignite procs per second times average number of ticks per proc
                double ticksPerSecond = rate * ticks;
                // finally using the proc of the ability we get the mps bonus
                manaRegenPerSecond += 0.02f * 3268f * CastingState.MageTalents.EmpoweredFire / 3.0f * (float)ticksPerSecond;
            }
        }

        public virtual void AddManaSourcesContribution(Dictionary<string, float> dict, float duration)
        {
            if (CastingState.CalculationOptions.EffectDisableManaSources) return;
            dict["Intellect/Spirit"] += duration * (CastingState.SpiritRegen * CastingState.BaseStats.SpellCombatManaRegeneration + OO5SR * (CastingState.SpiritRegen - CastingState.SpiritRegen * CastingState.BaseStats.SpellCombatManaRegeneration));
            dict["MP5"] += duration * CastingState.BaseStats.Mp5 / 5f;
            dict["Innervate"] += duration * (15732 * CastingState.CalculationOptions.Innervate / CastingState.CalculationOptions.FightDuration);
            dict["Mana Tide"] += duration * CastingState.CalculationOptions.ManaTide * 0.24f * CastingState.BaseStats.Mana / CastingState.CalculationOptions.FightDuration;
            dict["Replenishment"] += duration * CastingState.BaseStats.ManaRestoreFromMaxManaPerSecond * CastingState.BaseStats.Mana;
            //dict["Judgement of Wisdom"] += duration * CastingState.BaseStats.ManaRestoreFromBaseManaPPM * 3268 / CastTime * HitProcs;
            float fight = CastingState.CalculationOptions.FightDuration;
            for (int i = 0; i < CastingState.Solver.ManaRestoreEffectsCount; i++)
            {
                SpecialEffect effect = CastingState.Solver.ManaRestoreEffects[i];
                switch (effect.Trigger)
                {
                    case Trigger.Use:
                        dict["Other"] += duration * effect.Stats.ManaRestore * effect.GetAverageProcsPerSecond(0, 1, 3, fight);
                        break;
                    case Trigger.DamageSpellCast:
                    case Trigger.SpellCast:
                        if (CastProcs > 0)
                        {
                            dict["Other"] += duration * effect.Stats.ManaRestore * effect.GetAverageProcsPerSecond(CastTime / CastProcs, 1, 3, fight);
                        }
                        break;
                    case Trigger.DamageSpellCrit:
                    case Trigger.SpellCrit:
                        if (Ticks > 0)
                        {
                            dict["Other"] += duration * effect.Stats.ManaRestore * effect.GetAverageProcsPerSecond(CastTime / Ticks, CritProcs / Ticks, 3, fight);
                        }
                        break;
                    case Trigger.DamageSpellHit:
                    case Trigger.SpellHit:
                        if (Ticks > 0)
                        {
                            dict["Other"] += duration * effect.Stats.ManaRestore * effect.GetAverageProcsPerSecond(CastTime / Ticks, HitProcs / Ticks, 3, fight);
                        }
                        break;
                    case Trigger.DamageDone:
                    case Trigger.DamageOrHealingDone:
                        if (DamageProcs > 0)
                        {
                            dict["Other"] += duration * effect.Stats.ManaRestore * effect.GetAverageProcsPerSecond(CastTime / DamageProcs, 1, 3, fight);
                        }
                        break;
                    case Trigger.DoTTick:
                        if (DotProcs > 0)
                        {
                            dict["Other"] += duration * effect.Stats.ManaRestore * effect.GetAverageProcsPerSecond(CastTime / DotProcs, 1, 3, fight);
                        }
                        break;
                }
            }
            for (int i = 0; i < CastingState.Solver.Mp5EffectsCount; i++)
            {
                SpecialEffect effect = CastingState.Solver.Mp5Effects[i];
                switch (effect.Trigger)
                {
                    case Trigger.Use:
                        dict["Other"] += duration * effect.Stats.Mp5 / 5f * effect.GetAverageUptime(0f, 1f, 3, CastingState.CalculationOptions.FightDuration);
                        break;
                    case Trigger.DamageSpellCast:
                    case Trigger.SpellCast:
                        if (CastProcs > 0)
                        {
                            dict["Other"] += duration * effect.Stats.Mp5 / 5f * effect.GetAverageUptime(CastTime / CastProcs, 1f, 3, CastingState.CalculationOptions.FightDuration);
                        }
                        break;
                    case Trigger.DamageSpellCrit:
                    case Trigger.SpellCrit:
                        if (Ticks > 0)
                        {
                            dict["Other"] += duration * effect.Stats.Mp5 / 5f * effect.GetAverageUptime(CastTime / Ticks, CritProcs / Ticks, 3, CastingState.CalculationOptions.FightDuration);
                        }
                        break;
                    case Trigger.DamageSpellHit:
                    case Trigger.SpellHit:
                        if (Ticks > 0)
                        {
                            dict["Other"] += duration * effect.Stats.Mp5 / 5f * effect.GetAverageUptime(CastTime / Ticks, HitProcs / Ticks, 3, CastingState.CalculationOptions.FightDuration);
                        }
                        break;
                    case Trigger.DamageDone:
                    case Trigger.DamageOrHealingDone:
                        if (DamageProcs > 0)
                        {
                            dict["Other"] += duration * effect.Stats.Mp5 / 5f * effect.GetAverageUptime(CastTime / DamageProcs, 1f, 3, CastingState.CalculationOptions.FightDuration);
                        }
                        break;
                    case Trigger.DoTTick:
                        if (DotProcs > 0)
                        {
                            dict["Other"] += duration * effect.Stats.Mp5 / 5f * effect.GetAverageUptime(CastTime / DotProcs, 1f, 3, CastingState.CalculationOptions.FightDuration);
                        }
                        break;
                }
            }
            if (IgniteProcs > 0 && CastingState.MageTalents.EmpoweredFire > 0)
            {
                double rate = IgniteProcs / CastTime;
                double k = Math.Exp(-2 * rate);
                double ticks = k * (1 + k);
                double ticksPerSecond = rate * ticks;
                dict["Other"] += duration * 0.02f * 3268f * CastingState.MageTalents.EmpoweredFire / 3.0f * (float)ticksPerSecond;
            }
        }

        public void AddEffectContribution(Dictionary<string, SpellContribution> dict, float duration)
        {
            SpellContribution contrib;
            if (CastingState.WaterElemental)
            {
                Spell waterbolt = CastingState.GetSpell(SpellId.Waterbolt);
                if (!dict.TryGetValue(waterbolt.Name, out contrib))
                {
                    contrib = new SpellContribution() { Name = waterbolt.Name };
                    dict[waterbolt.Name] = contrib;
                }
                contrib.Hits += duration / waterbolt.CastTime;
                contrib.Damage += (waterbolt.AverageDamage + effectSpellPower * waterbolt.DamagePerSpellPower) / waterbolt.CastTime * duration;
            }
            if (CastingState.MirrorImage)
            {
                Spell mirrorImage = CastingState.GetSpell(SpellId.MirrorImage);
                if (!dict.TryGetValue("Mirror Image", out contrib))
                {
                    contrib = new SpellContribution() { Name = "Mirror Image" };
                    dict["Mirror Image"] = contrib;
                }
                contrib.Hits += 3 * (CastingState.MageTalents.GlyphOfMirrorImage ? 4 : 3) * duration / mirrorImage.CastTime;
                contrib.Damage += (mirrorImage.AverageDamage + effectSpellPower * mirrorImage.DamagePerSpellPower) / mirrorImage.CastTime * duration;
            }
            if (Ticks > 0)
            {
                for (int i = 0; i < CastingState.Solver.DamageProcEffectsCount; i++)
                {
                    SpecialEffect effect = CastingState.Solver.DamageProcEffects[i];
                    string name = null;
                    float chance = 0;
                    float interval = 0;
                    switch (effect.Trigger)
                    {
                        case Trigger.SpellCrit:
                        case Trigger.DamageSpellCrit:
                            chance = CritProcs / Ticks;
                            // aoe modifier
                            if (TargetProcs > HitProcs)
                            {
                                chance = 1f - (float)Math.Pow(1 - chance, TargetProcs / HitProcs);
                            }
                            interval = CastTime / Ticks;
                            break;
                        case Trigger.SpellHit:
                        case Trigger.DamageSpellHit:
                            chance = HitProcs / Ticks;
                            // aoe modifier
                            if (TargetProcs > HitProcs)
                            {
                                chance = 1f - (float)Math.Pow(1 - chance, TargetProcs / HitProcs);
                            }
                            interval = CastTime / Ticks;
                            break;
                        case Trigger.DamageDone:
                        case Trigger.DamageOrHealingDone:
                            chance = 1;
                            interval = CastTime / DamageProcs;
                            break;
                        case Trigger.DoTTick:
                            chance = 1;
                            interval = CastTime / DotProcs;
                            break;
                        case Trigger.SpellCast:
                        case Trigger.DamageSpellCast:
                            chance = 1;
                            if (effect.Stats.ValkyrDamage > 0)
                            {
                                interval = CastTime / CastProcs2;
                            }
                            else
                            {
                                interval = CastTime / CastProcs;
                            }
                            break;
                    }
                    float effectsPerSecond = effect.GetAverageProcsPerSecond(interval, chance, 3f, CastingState.CalculationOptions.FightDuration);
                    float boltDps = 0f;
                    if (effect.Stats.ArcaneDamage > 0)
                    {
                        boltDps = CastingState.ArcaneAverageDamage * effect.Stats.ArcaneDamage * effectsPerSecond;
                        name = "Arcane Damage Proc";
                    }
                    if (effect.Stats.FireDamage > 0)
                    {
                        boltDps = CastingState.FireAverageDamage * effect.Stats.FireDamage * effectsPerSecond;
                        name = "Fire Damage Proc";
                    }
                    if (effect.Stats.FrostDamage > 0)
                    {
                        boltDps = CastingState.FrostAverageDamage * effect.Stats.FrostDamage * effectsPerSecond;
                        name = "Frost Damage Proc";
                    }
                    if (effect.Stats.ShadowDamage > 0)
                    {
                        boltDps = CastingState.ShadowAverageDamage * effect.Stats.ShadowDamage * effectsPerSecond;
                        name = "Shadow Damage Proc";
                    }
                    if (effect.Stats.NatureDamage > 0)
                    {
                        boltDps = CastingState.NatureAverageDamage * effect.Stats.NatureDamage * effectsPerSecond;
                        name = "Nature Damage Proc";
                    }
                    if (effect.Stats.HolyDamage > 0)
                    {
                        boltDps = CastingState.HolyAverageDamage * effect.Stats.HolyDamage * effectsPerSecond;
                        name = "Holy Damage Proc";
                    }
                    if (effect.Stats.ValkyrDamage > 0)
                    {
                        boltDps = CastingState.ValkyrAverageDamage * effect.Stats.ValkyrDamage * effectsPerSecond;
                        name = "Val'kyr Damage";
                    }
                    if (!dict.TryGetValue(name, out contrib))
                    {
                        contrib = new SpellContribution() { Name = name };
                        dict[name] = contrib;
                    }
                    contrib.Hits += effectsPerSecond * duration;
                    contrib.Damage += boltDps * duration;
                }
            }
            /*if (CastingState.BaseStats.LightningCapacitorProc > 0)
            {
                if (!dict.TryGetValue("Lightning Bolt", out contrib))
                {
                    contrib = new SpellContribution() { Name = "Lightning Bolt" };
                    dict["Lightning Bolt"] = contrib;
                }
                //discrete model
                int hitsInsideCooldown = (int)(2.5f / (CastTime / Ticks));
                float avgCritsPerHit = CritProcs / Ticks * TargetProcs / HitProcs;
                float avgHitsToDischarge = 3f / avgCritsPerHit;
                if (avgHitsToDischarge < 1) avgHitsToDischarge = 1;
                float boltDps = CastingState.LightningBoltAverageDamage / ((CastTime / Ticks) * (hitsInsideCooldown + avgHitsToDischarge));
                contrib.Hits += duration / ((CastTime / Ticks) * (hitsInsideCooldown + avgHitsToDischarge));
                contrib.Damage += boltDps * duration;
            }
            if (CastingState.BaseStats.ThunderCapacitorProc > 0)
            {
                if (!dict.TryGetValue("Thunder Bolt", out contrib))
                {
                    contrib = new SpellContribution() { Name = "Thunder Bolt" };
                    dict["Thunder Bolt"] = contrib;
                }
                //discrete model
                int hitsInsideCooldown = (int)(2.5f / (CastTime / Ticks));
                float avgCritsPerHit = CritProcs / Ticks * TargetProcs / HitProcs;
                float avgHitsToDischarge = 4f / avgCritsPerHit;
                if (avgHitsToDischarge < 1) avgHitsToDischarge = 1;
                float boltDps = CastingState.ThunderBoltAverageDamage / ((CastTime / Ticks) * (hitsInsideCooldown + avgHitsToDischarge));
                contrib.Hits += duration / ((CastTime / Ticks) * (hitsInsideCooldown + avgHitsToDischarge));
                contrib.Damage += boltDps * duration;
            }*/
            /*if (CastingState.BaseStats.PendulumOfTelluricCurrentsProc > 0)
            {
                if (!dict.TryGetValue("Pendulum of Telluric Currents", out contrib))
                {
                    contrib = new SpellContribution() { Name = "Pendulum of Telluric Currents" };
                    dict["Pendulum of Telluric Currents"] = contrib;
                }
                float boltDps = CastingState.PendulumOfTelluricCurrentsAverageDamage / (45f + CastTime / HitProcs / 0.15f);
                contrib.Hits += duration / (45f + CastTime / HitProcs / 0.15f);
                contrib.Damage += boltDps * duration;
            }*/
            if (IgniteProcs > 0 && dict.TryGetValue("Ignite", out contrib))
            {
                double rate = IgniteProcs / CastTime;
                double k = Math.Exp(-2 * rate);
                double ticks = k * (1 + k);
                double ticksPerSecond = rate * ticks;
                contrib.Hits += duration * (float)ticksPerSecond;
            }
        }
    }

    public class SpellCustomMix : Cycle
    {
        public SpellCustomMix(bool needsDisplayCalculations, CastingState castingState)
            : base(needsDisplayCalculations, castingState)
        {
            Name = "Custom Mix";
            if (castingState.CalculationOptions.CustomSpellMix == null) return;
            for (int i = 0; i < castingState.CalculationOptions.CustomSpellMix.Count; i++)
            {
                SpellWeight spellWeight = castingState.CalculationOptions.CustomSpellMix[i];
                AddSpell(needsDisplayCalculations, castingState.GetSpell(spellWeight.Spell), spellWeight.Weight);
            }
            Calculate();
        }
    }

    public class CycleState
    {
        public List<CycleStateTransition> Transitions { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class CycleStateTransition
    {
        public CycleState TargetState { get; set; }
        public Cycle Cycle { get; set; }
        public Spell Spell { get; set; }
        public float Pause { get; set; }
        public virtual float TransitionProbability { get; set; }

        public override string ToString()
        {
            if (Spell != null)
            {
                return string.Format("{0} => {1} : {2:F}%", Spell.Name, TargetState, 100 * TransitionProbability);
            }
            else if (Cycle != null)
            {
                return string.Format("{0} => {1} : {2:F}%", Cycle.Name, TargetState, 100 * TransitionProbability);
            }
            else if (Pause > 0)
            {
                return string.Format("{0:F} sec => {1} : {2:F}%", Pause, TargetState, 100 * TransitionProbability);
            }
            return base.ToString();
        }
    }

    public class GenericCycle : Cycle
    {
        public List<CycleState> StateList;
        public double[] StateWeight;
        Dictionary<Spell, double> SpellWeight = new Dictionary<Spell, double>();
        Dictionary<Cycle, double> CycleWeight = new Dictionary<Cycle, double>();
        public string SpellDistribution;

#if SILVERLIGHT
        public GenericCycle(string name, CastingState castingState, List<CycleState> stateDescription, bool needsDisplayCalculations)
#else
        public unsafe GenericCycle(string name, CastingState castingState, List<CycleState> stateDescription, bool needsDisplayCalculations)
#endif
            : base(needsDisplayCalculations, castingState)
        {
            Name = name;

            StateList = stateDescription;
            for (int i = 0; i < StateList.Count; i++)
            {
                StateList[i].Index = i;
            }

            int size = StateList.Count + 1;

            ArraySet arraySet = ArrayPool.RequestArraySet(false);
            try
            {

                LU M = new LU(size, arraySet);

                StateWeight = new double[size];

#if SILVERLIGHT
            M.BeginSafe();

            Array.Clear(arraySet.LU_U, 0, size * size);

            //U[i * rows + j]

            foreach (CycleState state in StateList)
            {
                foreach (CycleStateTransition transition in state.Transitions)
                {
                    arraySet.LU_U[transition.TargetState.Index * size + state.Index] += transition.TransitionProbability;
                }
                arraySet.LU_U[state.Index * size + state.Index] -= 1.0;
            }

            for (int i = 0; i < size - 1; i++)
            {
                arraySet.LU_U[(size - 1) * size + i] = 1;
            }

            StateWeight[size - 1] = 1;

            M.Decompose();
            M.FSolve(StateWeight);

            M.EndUnsafe();            
#else
                fixed (double* U = arraySet.LU_U, x = StateWeight)
                fixed (double* sL = arraySet.LUsparseL, column = arraySet.LUcolumn, column2 = arraySet.LUcolumn2)
                fixed (int* P = arraySet.LU_P, Q = arraySet.LU_Q, LJ = arraySet.LU_LJ, sLI = arraySet.LUsparseLI, sLstart = arraySet.LUsparseLstart)
                {
                    M.BeginUnsafe(U, sL, P, Q, LJ, sLI, sLstart, column, column2);

                    Array.Clear(arraySet.LU_U, 0, size * size);

                    //U[i * rows + j]

                    foreach (CycleState state in StateList)
                    {
                        foreach (CycleStateTransition transition in state.Transitions)
                        {
                            U[transition.TargetState.Index * size + state.Index] += transition.TransitionProbability;
                        }
                        U[state.Index * size + state.Index] -= 1.0;
                    }

                    for (int i = 0; i < size - 1; i++)
                    {
                        U[(size - 1) * size + i] = 1;
                    }

                    x[size - 1] = 1;

                    M.Decompose();
                    M.FSolve(x);

                    M.EndUnsafe();
                }
#endif

                SpellWeight = new Dictionary<Spell, double>();
                CycleWeight = new Dictionary<Cycle, double>();

                foreach (CycleState state in StateList)
                {
                    double stateWeight = StateWeight[state.Index];
                    if (stateWeight > 0)
                    {
                        foreach (CycleStateTransition transition in state.Transitions)
                        {
                            float transitionProbability = transition.TransitionProbability;
                            if (transitionProbability > 0)
                            {
                                if (transition.Spell != null)
                                {
                                    double weight;
                                    SpellWeight.TryGetValue(transition.Spell, out weight);
                                    SpellWeight[transition.Spell] = weight + stateWeight * transitionProbability;
                                }
                                if (transition.Cycle != null)
                                {
                                    double weight;
                                    CycleWeight.TryGetValue(transition.Cycle, out weight);
                                    CycleWeight[transition.Cycle] = weight + stateWeight * transitionProbability;
                                }
                                if (transition.Pause > 0)
                                {
                                    AddPause(transition.Pause, (float)(stateWeight * transitionProbability));
                                }
                            }
                        }
                    }
                }

                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<Spell, double> kvp in SpellWeight)
                {
                    AddSpell(needsDisplayCalculations, kvp.Key, (float)kvp.Value);
                    if (kvp.Value > 0) sb.AppendFormat("{0}:\t{1:F}%\r\n", kvp.Key.Label ?? kvp.Key.SpellId.ToString(), 100.0 * kvp.Value);
                }
                foreach (KeyValuePair<Cycle, double> kvp in CycleWeight)
                {
                    AddCycle(needsDisplayCalculations, kvp.Key, (float)kvp.Value);
                    if (kvp.Value > 0) sb.AppendFormat("{0}:\t{1:F}%\r\n", kvp.Key.CycleId, 100.0 * kvp.Value);
                }

                Calculate();

                SpellDistribution = sb.ToString();
            }
            finally
            {
                ArrayPool.ReleaseArraySet(arraySet);
            }
        }
    }

    public class CycleControlledStateTransition : CycleStateTransition
    {
        private float rawProbability;
        private int controlIndex;
        private int controlValue;
        private int[] controlStates;

        public void SetControls(int controlIndex, int[] controlStates, int controlValue)
        {
            this.controlIndex = controlIndex;
            this.controlStates = controlStates;
            this.controlValue = controlValue;
        }

        public override float TransitionProbability
        {
            get
            {
                return (controlStates[controlIndex] == controlValue) ? rawProbability : 0.0f;
            }
            set
            {
                rawProbability = value;
            }
        }

        public override string ToString()
        {
            if (Spell != null)
            {
                return string.Format("{0} => {1} : {2:F}%", Spell.Name, TargetState, 100 * rawProbability);
            }
            else if (Cycle != null)
            {
                return string.Format("{0} => {1} : {2:F}%", Cycle.Name, TargetState, 100 * rawProbability);
            }
            else if (Pause > 0)
            {
                return string.Format("{0:F} sec => {1} : {2:F}%", Pause, TargetState, 100 * rawProbability);
            }
            return base.ToString();
        }
    }

    public abstract class CycleGenerator
    {
        public List<CycleState> StateList;
        public int[] ControlOptions;
        public int[] ControlValue;
        public int[] ControlIndex;
        public Dictionary<string, int>[] SpellMap;
        public virtual string StateDescription
        {
            get
            {
                return "";
            }
        }

        public void GenerateStateDescription()
        {
            List<CycleState> remainingStates = new List<CycleState>();
            List<CycleState> processedStates = new List<CycleState>();
            remainingStates.Add(GetInitialState());

            while (remainingStates.Count > 0)
            {
                CycleState state = remainingStates[remainingStates.Count - 1];
                remainingStates.RemoveAt(remainingStates.Count - 1);

                List<CycleControlledStateTransition> transitions = GetStateTransitions(state);
#if SILVERLIGHT
                state.Transitions = transitions.ConvertAll(transition => (CycleStateTransition)transition).ToList();
#else
                state.Transitions = transitions.ConvertAll(transition => (CycleStateTransition)transition);
#endif
                foreach (CycleControlledStateTransition transition in transitions)
                {
                    if (transition.TargetState != state && !processedStates.Contains(transition.TargetState) && !remainingStates.Contains(transition.TargetState))
                    {
                        remainingStates.Add(transition.TargetState);
                    }
                }

                processedStates.Add(state);
            }

            StateList = processedStates;
            for (int i = 0; i < StateList.Count; i++)
            {
                StateList[i].Index = i;
            }

            ControlIndex = new int[StateList.Count];
            List<CycleState> controlledStates = new List<CycleState>();
            foreach (CycleState state in StateList)
            {
                int controlIndex = -1;
                foreach (CycleState controlledState in controlledStates)
                {
                    if (!CanStatesBeDistinguished(state, controlledState))
                    {
                        controlIndex = ControlIndex[controlledState.Index];
                        break;
                    }
                }
                if (controlIndex == -1)
                {
                    controlIndex = controlledStates.Count;
                    controlledStates.Add(state);
                }
                ControlIndex[state.Index] = controlIndex;
            }

            ControlOptions = new int[controlledStates.Count];
            ControlValue = new int[controlledStates.Count];

            SpellMap = new Dictionary<string, int>[controlledStates.Count];

            foreach (CycleState state in StateList)
            {
                int controlIndex = ControlIndex[state.Index];
                if (SpellMap[controlIndex] == null)
                {
                    SpellMap[controlIndex] = new Dictionary<string, int>();
                }
                foreach (CycleControlledStateTransition transition in state.Transitions)
                {
                    string n;
                    if (transition.Spell != null)
                    {
                        n = transition.Spell.Name;
                    }
                    else
                    {
                        n = "Pause";
                    }
                    int controlValue;
                    if (!SpellMap[controlIndex].TryGetValue(n, out controlValue))
                    {
                        controlValue = SpellMap[controlIndex].Keys.Count;
                        SpellMap[controlIndex][n] = controlValue;
                    }
                    transition.SetControls(controlIndex, ControlValue, controlValue);
                }
            }

            for (int i = 0; i < ControlOptions.Length; i++)
            {
                ControlOptions[i] = SpellMap[i].Keys.Count;
            }
        }

        public GenericCycle GenerateCycle(string name, CastingState castingState)
        {
            return new GenericCycle(name, castingState, StateList, false);
        }

        public List<Cycle> Analyze(CastingState castingState, Cycle wand)
        {
            return Analyze(castingState, wand, null);
        }

        public List<Cycle> Analyze(CastingState castingState, Cycle wand, System.ComponentModel.BackgroundWorker worker)
        {
            Dictionary<string, Cycle> cycleDict = new Dictionary<string, Cycle>();
            int j;
            // reset
            for (int i = 0; i < ControlValue.Length; i++)
            {
                ControlValue[i] = 0;
            }
            // count total cycles
            int total = 0;
            do
            {
                total++;
                j = ControlValue.Length - 1;
                ControlValue[j]++;
                while (ControlValue[j] >= ControlOptions[j])
                {
                    ControlValue[j] = 0;
                    j--;
                    if (j < 0)
                    {
                        break;
                    }
                    ControlValue[j]++;
                }
            } while (j >= 0);
            // reset
            for (int i = 0; i < ControlValue.Length; i++)
            {
                ControlValue[i] = 0;
            }
            int count = 0;
            do
            {
                if (worker != null && worker.CancellationPending)
                {
                    break;
                }
                if (worker != null && count % 100 == 0)
                {
                    worker.ReportProgress((100 * count) / total, count + "/" + total);
                }
                count++;
                string name = "";
                for (int i = 0; i < ControlValue.Length; i++)
                {
                    name += ControlValue[i].ToString();
                }
                GenericCycle generic = new GenericCycle(name, castingState, StateList, false);
                if (!cycleDict.ContainsKey(generic.SpellDistribution))
                {
                    cycleDict.Add(generic.SpellDistribution, generic);
                }
                // increment control
                j = ControlValue.Length - 1;
                ControlValue[j]++;
                while (ControlValue[j] >= ControlOptions[j])
                {
                    ControlValue[j] = 0;
                    j--;
                    if (j < 0)
                    {
                        break;
                    }
                    ControlValue[j]++;
                }
            } while (j >= 0);

            if (wand != null)
            {
                cycleDict["Wand"] = wand;
            }

            List<Cycle> cyclePalette = new List<Cycle>();

            double maxdps = 0;
            Cycle maxdpsCycle = null;
            foreach (Cycle cycle in cycleDict.Values)
            {
                if (cycle.DamagePerSecond > maxdps)
                {
                    maxdpsCycle = cycle;
                    maxdps = cycle.DamagePerSecond;
                }
            }

            cyclePalette.Add(maxdpsCycle);

            Cycle mindpmCycle;
            do
            {
                Cycle highdpsCycle = cyclePalette[cyclePalette.Count - 1];
            RESTART:
                mindpmCycle = null;
                double mindpm = double.PositiveInfinity;
                foreach (Cycle cycle in cycleDict.Values)
                {
                    double dpm = (cycle.DamagePerSecond - highdpsCycle.DamagePerSecond) / (cycle.ManaPerSecond - highdpsCycle.ManaPerSecond);
                    if (dpm > 0 && dpm < mindpm && cycle.ManaPerSecond < highdpsCycle.ManaPerSecond)
                    {
                        mindpm = dpm;
                        mindpmCycle = cycle;
                    }
                }
                if (mindpmCycle != null)
                {
                    // validate cycle pair theory
                    foreach (Cycle cycle in cycleDict.Values)
                    {
                        double dpm = (cycle.DamagePerSecond - mindpmCycle.DamagePerSecond) / (cycle.ManaPerSecond - mindpmCycle.ManaPerSecond);
                        if (cycle != highdpsCycle && cycle.DamagePerSecond > mindpmCycle.DamagePerSecond && dpm > mindpm + 0.000001)
                        {
                            highdpsCycle = cycle;
                            goto RESTART;
                        }
                    }
                    cyclePalette.Add(mindpmCycle);
                }
            } while (mindpmCycle != null);
            return cyclePalette;
        }

        protected abstract CycleState GetInitialState();
        // the transition probabilities should be set as given the spell/pause is executed 100%
        // the transitions should all be spell transitions and at most one can be a state changing pause
        protected abstract List<CycleControlledStateTransition> GetStateTransitions(CycleState state);
        // the states must form equivalence classes
        protected abstract bool CanStatesBeDistinguished(CycleState state1, CycleState state2);
    }
}
