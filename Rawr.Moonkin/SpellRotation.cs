using System;
using System.Collections.Generic;

namespace Rawr.Moonkin
{
    // Rotation information for display to the user.
    public class RotationData
    {
        public float BurstDPS = 0.0f;
        public float DPS = 0.0f;
        public float DPM = 0.0f;
        public float ManaUsed = 0.0f;
        public float ManaGained = 0.0f;
        public TimeSpan TimeToOOM = new TimeSpan(0, 0, 0);
    }

    // Our old friend the spell rotation.
    public class SpellRotation
    {
        public MoonkinSolver Solver { get; set; }
        public List<string> SpellsUsed;
        public RotationData RotationData = new RotationData();
        public string Name { get; set; }
        public float Duration { get; set; }
        public float ManaUsed { get; set; }
        public float ManaGained { get; set; }
        public float CastCount { get; set; }
        public float DotTicks { get; set; }
        public float WrathCount { get; set; }
        public float WrathAvgHit { get; set; }
        public float WrathAvgCast { get; set; }
        public float WrathNGCastTime { get; set; }
        public float WrathManaCost { get; set; }
        public float StarfireCount { get; set; }
        public float StarfireAvgHit { get; set; }
        public float StarfireAvgCast { get; set; }
        public float StarfireNGCastTime { get; set; }
        public float StarfireNonEclipseCrit { get; set; }
        public float StarfireEclipseCrit { get; set; }
        public float StarfireManaCost { get; set; }
        public float InsectSwarmTicks { get; set; }
        public float InsectSwarmCasts { get; set; }
        public float InsectSwarmAvgHit { get; set; }
        public float InsectSwarmDuration { get; set; }
        public float InsectSwarmCastTime { get; set; }
        public float InsectSwarmManaCost { get; set; }
        public float MoonfireCasts { get; set; }
        public float MoonfireTicks { get; set; }
        public float MoonfireAvgHit { get; set; }
        public float MoonfireDuration { get; set; }
        public float MoonfireCastTime { get; set; }
        public float MoonfireManaCost { get; set; }
        public float StarfallDamage { get; set; }
        public float StarfallStars { get; set; }

        // Calculate damage and casting time for a single, direct-damage spell.
        private void DoMainNuke(DruidTalents talents, CharacterCalculationsMoonkin calcs, ref Spell mainNuke, float spellPower, float spellHit, float spellCrit, float spellHaste)
        {
            float latency = calcs.Latency;
            int naturesGrace = talents.NaturesGrace;
            int starlightWrath = talents.StarlightWrath;

            float overallDamageModifier = mainNuke.AllDamageModifier * (1 + calcs.BasicStats.BonusSpellPowerMultiplier) * (1 + calcs.BasicStats.BonusDamageMultiplier);
            overallDamageModifier *= mainNuke.School == SpellSchool.Arcane ? (1 + calcs.BasicStats.BonusArcaneDamageMultiplier) : (1 + calcs.BasicStats.BonusNatureDamageMultiplier);
            overallDamageModifier *= 1 - 0.02f * (calcs.TargetLevel - 80);

            float gcd = 1.5f / (1.0f + spellHaste);
            float instantCast = (float)Math.Max(gcd, 1.0f) + latency;
            float ngGCD = (float)Math.Max(gcd / 1.2f, 1.0f);
            float instantCastNG = ngGCD + latency;

            mainNuke.CastTime = mainNuke.BaseCastTime - 0.1f * starlightWrath;
            float totalCritChance = spellCrit + mainNuke.CriticalChanceModifier;
            float normalCastTime = (float)Math.Max(mainNuke.CastTime / (1 + spellHaste), instantCast);
            mainNuke.NGCastTime = (float)Math.Max(mainNuke.CastTime / (1 + spellHaste) / (1 + 0.2f * naturesGrace / 3.0f), instantCastNG);
            float NGProcChance = totalCritChance * naturesGrace / 3.0f;
            float NGUptime = 1.0f - (float)Math.Pow(1.0f - NGProcChance, Math.Floor(3.0f / mainNuke.NGCastTime) + 1.0f);
            mainNuke.CastTime = (1 - NGUptime) * normalCastTime + NGUptime * mainNuke.NGCastTime;
            // Damage calculations
            float damagePerNormalHit = (mainNuke.BaseDamage + mainNuke.SpellDamageModifier * (spellPower + mainNuke.IdolExtraSpellPower)) * overallDamageModifier;
            float damagePerCrit = damagePerNormalHit * mainNuke.CriticalDamageModifier * (1 + calcs.BasicStats.MoonkinT10CritDot);
            mainNuke.DamagePerHit = (totalCritChance * damagePerCrit + (1 - totalCritChance) * damagePerNormalHit) * spellHit;
        }
        // Modified version of above function specifically for use in calculating Moonkin 4T8 proc.
        public void DoSpecialStarfire(CharacterCalculationsMoonkin calcs, ref Spell mainNuke, float spellPower, float spellHit, float spellCrit, float spellHaste)
        {
            float latency = calcs.Latency;
            float overallDamageModifier = mainNuke.AllDamageModifier * (1 + calcs.BasicStats.BonusSpellPowerMultiplier) * (1 + calcs.BasicStats.BonusDamageMultiplier) * (1 + calcs.BasicStats.BonusArcaneDamageMultiplier);
            overallDamageModifier *= 1 - 0.02f * (calcs.TargetLevel - 80);

            mainNuke.CastTime = mainNuke.BaseCastTime;
            float totalCritChance = spellCrit + mainNuke.CriticalChanceModifier;
            float normalCastTime = (float)Math.Max(1.0f, mainNuke.CastTime / (1 + spellHaste)) + latency;
            mainNuke.NGCastTime = (float)Math.Max(1.0f, mainNuke.CastTime / (1 + spellHaste) / (1 + 0.2f * Solver.NaturesGrace / 3.0f)) + latency;
            float NGProcChance = totalCritChance * Solver.NaturesGrace / 3.0f;
            float NGUptime = 1.0f - (float)Math.Pow(1.0f - NGProcChance, Math.Floor(3.0f / mainNuke.NGCastTime) + 1.0f);
            mainNuke.CastTime = (1 - NGUptime) * normalCastTime + NGUptime * mainNuke.NGCastTime;
            // Damage calculations
            float damagePerNormalHit = (mainNuke.BaseDamage + mainNuke.SpellDamageModifier * (spellPower + mainNuke.IdolExtraSpellPower)) * overallDamageModifier;
            float damagePerCrit = damagePerNormalHit * mainNuke.CriticalDamageModifier * (1 + calcs.BasicStats.MoonkinT10CritDot);
            mainNuke.DamagePerHit = (totalCritChance * damagePerCrit + (1 - totalCritChance) * damagePerNormalHit) * spellHit;
        }

        // Calculate damage and casting time for a damage-over-time effect.
        private void DoDotSpell(DruidTalents talents, CharacterCalculationsMoonkin calcs, ref Spell dotSpell, float spellPower, float spellHit, float spellCrit, float spellHaste)
        {
            if (dotSpell.Name == "MF")
                DoMoonfire(talents, calcs, ref dotSpell, spellPower, spellHit, spellCrit, spellHaste);
            else
                DoInsectSwarm(talents, calcs, ref dotSpell, spellPower, spellHit, spellCrit, spellHaste);
        }

        // Calculate damage and casting time for the Moonfire effect.
        private void DoMoonfire(DruidTalents talents, CharacterCalculationsMoonkin calcs, ref Spell dotSpell, float spellPower, float spellHit, float spellCrit, float spellHaste)
        {
            float latency = calcs.Latency;

            float naturesGrace = talents.NaturesGrace;

            float overallDamageModifier = dotSpell.AllDamageModifier * (1 + calcs.BasicStats.BonusSpellPowerMultiplier) * (1 + calcs.BasicStats.BonusDamageMultiplier) * (1 + calcs.BasicStats.BonusArcaneDamageMultiplier);
            overallDamageModifier *= 1 - 0.02f * (calcs.TargetLevel - 80);

            float dotEffectDamageModifier = dotSpell.DotEffect.AllDamageModifier * (1 + calcs.BasicStats.BonusSpellPowerMultiplier) * (1 + calcs.BasicStats.BonusDamageMultiplier) * (1 + calcs.BasicStats.BonusArcaneDamageMultiplier);
            dotEffectDamageModifier *= 1 - 0.02f * (calcs.TargetLevel - 80);

            float gcd = 1.5f / (1.0f + spellHaste);
            float instantCast = (float)Math.Max(gcd, 1.0f) + latency;
            float ngGCD = (float)Math.Max(gcd / 1.2f, 1.0f);
            float instantCastNG = ngGCD + latency;
            float normalCastTime = (float)Math.Max(dotSpell.CastTime / (1 + spellHaste), instantCast);
            float NGCastTime = (float)Math.Max(dotSpell.CastTime / (1 + spellHaste) / (1 + 0.2f * naturesGrace / 3.0f), instantCastNG);
            float NGProcChance = spellCrit * naturesGrace / 3.0f;
            float NGUptime = 1.0f - (float)Math.Pow(1.0f - NGProcChance, Math.Floor(3.0f / normalCastTime) + 1.0f);
            dotSpell.CastTime = (1 - NGUptime) * normalCastTime + NGUptime * NGCastTime;

            float mfDirectDamage = (dotSpell.BaseDamage + dotSpell.SpellDamageModifier * (spellPower + dotSpell.IdolExtraSpellPower)) * overallDamageModifier;
            float mfCritDamage = mfDirectDamage * dotSpell.CriticalDamageModifier;
            float totalCritChance = spellCrit + dotSpell.CriticalChanceModifier;
            dotSpell.DamagePerHit = (totalCritChance * mfCritDamage + (1 - totalCritChance) * mfDirectDamage) * spellHit;
            float normalDamagePerTick = dotSpell.DotEffect.TickDamage + dotSpell.DotEffect.SpellDamageModifierPerTick * spellPower;
            float damagePerTick = 0.0f;
            if (dotSpell.DotEffect.CanCrit)
            {
                float critDamagePerTick = normalDamagePerTick * dotSpell.CriticalDamageModifier;
                damagePerTick = (totalCritChance * critDamagePerTick + (1 - totalCritChance) * normalDamagePerTick) * dotEffectDamageModifier;
            }
            else
            {
                damagePerTick = normalDamagePerTick * dotEffectDamageModifier;
            }
            dotSpell.DotEffect.DamagePerHit = dotSpell.DotEffect.NumberOfTicks * damagePerTick * spellHit;
        }

        // Calculate damage and casting time for the Insect Swarm effect.
        private void DoInsectSwarm(DruidTalents talents, CharacterCalculationsMoonkin calcs, ref Spell dotSpell, float spellPower, float spellHit, float spellCrit, float spellHaste)
        {
            float latency = calcs.Latency;

            float naturesGrace = talents.NaturesGrace;

            float dotEffectDamageModifier = dotSpell.DotEffect.AllDamageModifier * (1 + calcs.BasicStats.BonusSpellPowerMultiplier) * (1 + calcs.BasicStats.BonusDamageMultiplier) * (1 + calcs.BasicStats.BonusNatureDamageMultiplier);
            dotEffectDamageModifier *= 1 - 0.02f * (calcs.TargetLevel - 80);

            float gcd = 1.5f / (1.0f + spellHaste);
            float instantCast = (float)Math.Max(gcd, 1.0f) + latency;
            float ngGCD = (float)Math.Max(gcd / 1.2f, 1.0f);
            float instantCastNG = ngGCD + latency;
            float normalCastTime = (float)Math.Max(dotSpell.CastTime / (1 + spellHaste), instantCast);
            float NGCastTime = (float)Math.Max(dotSpell.CastTime / (1 + spellHaste) / (1 + 0.2f * naturesGrace / 3.0f), instantCastNG);
            float NGProcChance = spellCrit * naturesGrace / 3.0f;
            float NGUptime = 1.0f - (float)Math.Pow(1.0f - NGProcChance, Math.Floor(3.0f / normalCastTime) + 1.0f);
            dotSpell.CastTime = (1 - NGUptime) * normalCastTime + NGUptime * NGCastTime;
            float damagePerTick = (dotSpell.DotEffect.TickDamage + dotSpell.DotEffect.SpellDamageModifierPerTick * (spellPower + dotSpell.IdolExtraSpellPower)) * dotEffectDamageModifier;
            dotSpell.DotEffect.DamagePerHit = dotSpell.DotEffect.NumberOfTicks * damagePerTick * spellHit;
        }

        // Perform damage and mana calculations for all spells in the given rotation.  Returns damage done over the total duration.
        public float DamageDone(DruidTalents talents, CharacterCalculationsMoonkin calcs, float spellPower, float spellHit, float spellCrit, float spellHaste)
        {
            if (talents.Eclipse > 0)
            {
                return DoEclipseCalcs(talents, calcs, Solver, spellPower, spellHit, spellCrit, spellHaste);
            }
            float latency = calcs.Latency;

            float moonkinFormProc = (talents.MoonkinForm == 1) ? 0.02f * calcs.BasicStats.Mana : 0.0f;
            bool starfireGlyph = talents.GlyphOfStarfire;
            int impInsectSwarm = talents.ImprovedInsectSwarm;

            switch (SpellsUsed.Count)
            {
                // Nuke only
                case 1:
                    Spell mainNuke = Solver.FindSpell(SpellsUsed[0]);
                    DoMainNuke(talents, calcs, ref mainNuke, spellPower, spellHit, spellCrit, spellHaste);

                    float omenProcChance = talents.OmenOfClarity == 1 ? 0.06f : 0;
                    mainNuke.ManaCost = mainNuke.BaseManaCost - mainNuke.BaseCastTime / 60.0f * calcs.BasicStats.ManaRestoreFromBaseManaPPM * CalculationsMoonkin.BaseMana * spellHit - (spellCrit + mainNuke.CriticalChanceModifier) * moonkinFormProc - mainNuke.BaseManaCost * omenProcChance * spellHit;
                    Duration = mainNuke.CastTime;
                    RotationData.ManaUsed = ManaUsed = mainNuke.ManaCost;
                    RotationData.ManaGained = ManaGained = mainNuke.BaseManaCost - mainNuke.ManaCost;
                    RotationData.DPM = mainNuke.DamagePerHit / mainNuke.ManaCost;
                    CastCount = 1.0f;
                    DotTicks = 0.0f;
                    WrathCount = mainNuke.Name == "W" ? 1.0f : 0.0f;
                    StarfireCount = mainNuke.Name == "SF" ? 1.0f : 0.0f;

                    return mainNuke.DamagePerHit;
                // Nuke + 1 DotEffect
                case 2:
                    // Find the spells
                    Spell DotEffectSpell = Solver.FindSpell(SpellsUsed[0]);
                    mainNuke = Solver.FindSpell(SpellsUsed[1]);
                    // Do Starfire glyph calculations, if applicable; then do DoT spell calculations
                    if (starfireGlyph && mainNuke.Name == "SF" && DotEffectSpell.Name == "MF") DotEffectSpell.DotEffect.Duration += 9.0f;
                    DoDotSpell(talents, calcs, ref DotEffectSpell, spellPower, spellHit, spellCrit, spellHaste);

                    // Do iIS calculations, if applicable
                    if (impInsectSwarm > 0)
                    {
                        if (mainNuke.Name == "SF" && DotEffectSpell.Name == "MF")
                            mainNuke.CriticalChanceModifier += 0.01f * impInsectSwarm;
                        else if (mainNuke.Name == "W" && DotEffectSpell.Name == "IS")
                            mainNuke.AllDamageModifier *= 1 + 0.01f * impInsectSwarm;
                    }
                    // Calculate main nuke damage
                    DoMainNuke(talents, calcs, ref mainNuke, spellPower, spellHit, spellCrit, spellHaste);

                    // Set rotation duration
                    Duration = DotEffectSpell.DotEffect.Duration;

                    // Calculate mana usage and damage done for this rotation
                    float timeSpentCastingNuke = Duration - DotEffectSpell.CastTime;
                    float nukeDamageDone = mainNuke.DamagePerHit / mainNuke.CastTime * timeSpentCastingNuke;

                    float numNukeCasts = timeSpentCastingNuke / mainNuke.CastTime;
                    float nukeManaSpent = mainNuke.BaseManaCost * numNukeCasts;
                    float totalManaSpent = nukeManaSpent + DotEffectSpell.BaseManaCost;
                    CastCount = numNukeCasts + 1.0f;
                    WrathCount = mainNuke.Name == "W" ? numNukeCasts : 0.0f;
                    StarfireCount = mainNuke.Name == "SF" ? numNukeCasts : 0.0f;
                    DotTicks = DotEffectSpell.DotEffect.NumberOfTicks;
                    if (DotEffectSpell.Name == "IS")
                        InsectSwarmTicks = DotTicks;
                    else if (DotEffectSpell.Name == "MF")
                    {
                        MoonfireTicks = DotTicks;
                        MoonfireCasts = 1f;
                    }

                    float manaFromJoW = (mainNuke.ManaCost - mainNuke.BaseCastTime / 60.0f * calcs.BasicStats.ManaRestoreFromBaseManaPPM * CalculationsMoonkin.BaseMana) * numNukeCasts;
                    manaFromJoW += DotEffectSpell.ManaCost - 1.5f / 60.0f * calcs.BasicStats.ManaRestoreFromBaseManaPPM * CalculationsMoonkin.BaseMana;
                    float manaFromOoC = ((0.06f) * mainNuke.BaseManaCost
                        + (numNukeCasts - 1) * (0.06f) * mainNuke.BaseManaCost
                        + (0.06f) * DotEffectSpell.BaseManaCost) * spellHit;
                    float manaFromMoonkin = moonkinFormProc * spellHit * ((spellCrit + mainNuke.CriticalChanceModifier) * numNukeCasts + DotEffectSpell.CriticalChanceModifier);

                    float actualManaSpent = totalManaSpent - manaFromJoW - manaFromMoonkin - (talents.OmenOfClarity == 1 ? manaFromOoC : 0.0f);

                    RotationData.ManaUsed = ManaUsed = actualManaSpent;
                    RotationData.ManaGained = ManaGained = totalManaSpent - actualManaSpent;
                    RotationData.DPM = (nukeDamageDone + DotEffectSpell.DamagePerHit + DotEffectSpell.DotEffect.DamagePerHit) / totalManaSpent;

                    // Undo iIS, if applicable
                    if (impInsectSwarm > 0)
                    {
                        if (mainNuke.Name == "SF" && DotEffectSpell.Name == "MF")
                            mainNuke.CriticalChanceModifier -= 0.01f * impInsectSwarm;
                        else if (mainNuke.Name == "W" && DotEffectSpell.Name == "IS")
                            mainNuke.AllDamageModifier /= 1 + 0.01f * impInsectSwarm;
                    }
                    // Undo SF glyph, if applicable
                    if (starfireGlyph && mainNuke.Name == "SF" && DotEffectSpell.Name == "MF") DotEffectSpell.DotEffect.Duration -= 9.0f;
                    // Return the damage done per rotation
                    return nukeDamageDone + DotEffectSpell.DamagePerHit + DotEffectSpell.DotEffect.DamagePerHit;
                // Nuke + both DotEffects
                case 3:
                    // Find the spells
                    Spell moonFire = Solver.FindSpell(SpellsUsed[0]);
                    Spell insectSwarm = Solver.FindSpell(SpellsUsed[1]);
                    mainNuke = Solver.FindSpell(SpellsUsed[2]);
                    // Do Starfire glyph calculations, if applicable; then do DoT spell calculations
                    if (starfireGlyph && mainNuke.Name == "SF") moonFire.DotEffect.Duration += 9.0f;
                    DoDotSpell(talents, calcs, ref moonFire, spellPower, spellHit, spellCrit, spellHaste);
                    DoDotSpell(talents, calcs, ref insectSwarm, spellPower, spellHit, spellCrit, spellHaste);

                    // Do iIS calculations, if applicable
                    if (impInsectSwarm > 0)
                    {
                        if (mainNuke.Name == "SF")
                            mainNuke.CriticalChanceModifier += 0.01f * impInsectSwarm;
                        else if (mainNuke.Name == "W")
                            mainNuke.AllDamageModifier *= 1 + 0.01f * impInsectSwarm;
                    }
                    // Calculate main nuke damage
                    DoMainNuke(talents, calcs, ref mainNuke, spellPower, spellHit, spellCrit, spellHaste);

                    // Set rotation duration
                    Duration = moonFire.DotEffect.Duration;

                    // Calculate mana usage and damage done for this rotation
                    float timeSpentCastingIS = insectSwarm.CastTime * moonFire.DotEffect.Duration / insectSwarm.DotEffect.Duration;
                    float insectSwarmDamage = insectSwarm.DotEffect.DamagePerHit * moonFire.DotEffect.Duration / insectSwarm.DotEffect.Duration;
                    timeSpentCastingNuke = Duration - timeSpentCastingIS - moonFire.CastTime;
                    nukeDamageDone = mainNuke.DamagePerHit / mainNuke.CastTime * timeSpentCastingNuke;

                    numNukeCasts = timeSpentCastingNuke / mainNuke.CastTime;
                    float numISCasts = timeSpentCastingIS / insectSwarm.CastTime;
                    nukeManaSpent = mainNuke.BaseManaCost * numNukeCasts;
                    totalManaSpent = nukeManaSpent + moonFire.BaseManaCost + numISCasts * insectSwarm.BaseManaCost;
                    CastCount = numNukeCasts + numISCasts + 1.0f;
                    WrathCount = mainNuke.Name == "W" ? numNukeCasts : 0.0f;
                    StarfireCount = mainNuke.Name == "SF" ? numNukeCasts : 0.0f;
                    DotTicks = moonFire.DotEffect.NumberOfTicks + numISCasts * insectSwarm.DotEffect.NumberOfTicks;
                    InsectSwarmTicks = numISCasts * insectSwarm.DotEffect.NumberOfTicks;
                    MoonfireTicks = moonFire.DotEffect.NumberOfTicks;
                    MoonfireCasts = 1.0f;

                    manaFromJoW = numNukeCasts * (mainNuke.ManaCost - mainNuke.BaseCastTime / 60.0f * calcs.BasicStats.ManaRestoreFromBaseManaPPM * CalculationsMoonkin.BaseMana) +
                        (moonFire.ManaCost - 1.5f / 60.0f * calcs.BasicStats.ManaRestoreFromBaseManaPPM * CalculationsMoonkin.BaseMana) +
                        numISCasts * (insectSwarm.ManaCost - 1.5f / 60.0f * calcs.BasicStats.ManaRestoreFromBaseManaPPM * CalculationsMoonkin.BaseMana);
                    manaFromOoC = ((0.06f) * mainNuke.BaseManaCost
                        + (numNukeCasts - 1 - numISCasts) * (0.06f) * mainNuke.BaseManaCost
                        + (0.06f) * moonFire.BaseManaCost
                        + (0.06f) * numISCasts * insectSwarm.BaseManaCost
                        + (0.06f) * numISCasts * mainNuke.BaseManaCost) * spellHit;
                    manaFromMoonkin = moonkinFormProc * spellHit * ((spellCrit + mainNuke.CriticalChanceModifier) * numNukeCasts + moonFire.CriticalChanceModifier);

                    actualManaSpent = totalManaSpent - manaFromJoW - manaFromMoonkin - (talents.OmenOfClarity == 1 ? manaFromOoC : 0.0f);

                    RotationData.ManaUsed = ManaUsed = actualManaSpent;
                    RotationData.ManaGained = ManaGained = totalManaSpent - actualManaSpent;
                    RotationData.DPM = (nukeDamageDone + moonFire.DamagePerHit + moonFire.DotEffect.DamagePerHit + insectSwarm.DotEffect.DamagePerHit) / totalManaSpent;

                    // Undo iIS, if applicable
                    if (impInsectSwarm > 0)
                    {
                        if (mainNuke.Name == "SF")
                            mainNuke.CriticalChanceModifier -= 0.01f * impInsectSwarm;
                        else if (mainNuke.Name == "W")
                            mainNuke.AllDamageModifier /= 1 + 0.01f * impInsectSwarm;
                    }
                    // Undo SF glyph
                    if (starfireGlyph && mainNuke.Name == "SF") moonFire.DotEffect.Duration -= 9.0f;
                    // Return the damage done per rotation
                    return nukeDamageDone + moonFire.DamagePerHit + moonFire.DotEffect.DamagePerHit + insectSwarmDamage;
                default:
                    throw new Exception("Invalid rotation specified in rotation solver.");
            }
        }

        private float DoEclipseCalcs(DruidTalents talents, CharacterCalculationsMoonkin calcs, MoonkinSolver solver, float spellPower, float spellHit, float spellCrit, float spellHaste)
        {
            float latency = calcs.Latency;

            float omenOfClarityProcChance = talents.OmenOfClarity * 0.06f;
            float moonkinFormProc = (talents.MoonkinForm == 1) ? 0.02f * calcs.BasicStats.Mana : 0.0f;
            bool starfireGlyph = talents.GlyphOfStarfire;
            int impInsectSwarm = talents.ImprovedInsectSwarm;

            float moonfireCasts = SpellsUsed.Contains("MF") ? 1.0f : 0.0f;
            float insectSwarmCasts = SpellsUsed.Contains("IS") ? 1.0f : 0.0f;

            Spell moonfire = moonfireCasts > 0 ? solver.FindSpell("MF") : null;
            Spell insectSwarm = insectSwarmCasts > 0 ? solver.FindSpell("IS") : null;

            // Do SF glyph
            if (starfireGlyph && moonfire != null) moonfire.DotEffect.Duration += 9.0f;

            float eclipseMultiplier = 0.4f + calcs.BasicStats.EclipseBonus;

            float eclipseDuration = 15.0f;
            //float eclipseCooldown = 30.0f;

            Spell preLunarCast = solver.FindSpell("W");
            // Do improved Insect Swarm
            if (insectSwarm != null)
                preLunarCast.AllDamageModifier *= 1 + 0.01f * impInsectSwarm;

            Spell solarEclipseCast = new Spell(preLunarCast);

            // Eclipse bonus and improved Insect Swarm
            // NOTE: Eclipse bonus additive with Moonfury and 4T9; multiplicative with everything else
            solarEclipseCast.AllDamageModifier = 1 + (float)Math.Floor(talents.Moonfury * 10 / 3.0f) / 100.0f + calcs.BasicStats.BonusMoonkinNukeDamage + eclipseMultiplier;
            if (insectSwarm != null)
                solarEclipseCast.AllDamageModifier *= 1 + 0.01f * impInsectSwarm;

            Spell preSolarCast = solver.FindSpell("SF");
            if (moonfire != null)
                preSolarCast.CriticalChanceModifier += 0.01f * impInsectSwarm;

            Spell lunarEclipseCast = new Spell(preSolarCast);
            lunarEclipseCast.CriticalChanceModifier = (float)Math.Min(1.0f - spellCrit, lunarEclipseCast.CriticalChanceModifier + eclipseMultiplier);

            DoMainNuke(talents, calcs, ref preSolarCast, spellPower, spellHit, spellCrit, spellHaste);
            DoMainNuke(talents, calcs, ref solarEclipseCast, spellPower, spellHit, spellCrit, spellHaste);
            DoMainNuke(talents, calcs, ref preLunarCast, spellPower, spellHit, spellCrit, spellHaste);
            DoMainNuke(talents, calcs, ref lunarEclipseCast, spellPower, spellHit, spellCrit, spellHaste);

            float moonfireRatio = 0.0f;
            float insectSwarmRatio = 0.0f;
            if (moonfire != null)
            {
                DoDotSpell(talents, calcs, ref moonfire, spellPower, spellHit, spellCrit, spellHaste);
                moonfireRatio = moonfire.CastTime / moonfire.DotEffect.Duration;
            }
            if (insectSwarm != null)
            {
                DoDotSpell(talents, calcs, ref insectSwarm, spellPower, spellHit, spellCrit, spellHaste);
                insectSwarmRatio = insectSwarm.CastTime / insectSwarm.DotEffect.Duration;
            }

            float castRatio = moonfireRatio + (1 - moonfireRatio) * insectSwarmRatio;

            float lunarProcChance = (spellCrit + preLunarCast.CriticalChanceModifier) * spellHit * talents.Eclipse / 3.0f * 0.6f;
            float castsToProcLunar = 1.0f / lunarProcChance;
            float timeToProcLunar = preLunarCast.CastTime * (castsToProcLunar - 0.5f);

            float solarProcChance = (spellCrit + preSolarCast.CriticalChanceModifier) * spellHit * talents.Eclipse / 3.0f;
            float castsToProcSolar = 1.0f / solarProcChance;
            float timeToProcSolar = preSolarCast.CastTime * (castsToProcSolar - 0.5f);

            float preLunarTime = timeToProcLunar + (preLunarCast.CastTime * 0.5f) + preLunarCast.NGCastTime * 1.5f;
            float preLunarDPS = preLunarCast.DamagePerHit / preLunarCast.CastTime;
            float preLunarManaUsed = preLunarCast.BaseManaCost / preLunarCast.CastTime * preLunarTime;
            float preLunarManaGained = (preLunarCast.BaseManaCost * omenOfClarityProcChance) +
                ((spellCrit + preLunarCast.CriticalChanceModifier) * spellHit * moonkinFormProc) +
                (preLunarCast.BaseCastTime / 60.0f * calcs.BasicStats.ManaRestoreFromBaseManaPPM * CalculationsMoonkin.BaseMana);

            float lunarTime = (eclipseDuration - (preLunarCast.NGCastTime * 1.5f) - lunarEclipseCast.CastTime * 0.5f) * (1.0f - castRatio);
            float lunarDPS = lunarEclipseCast.DamagePerHit / lunarEclipseCast.CastTime;
            float lunarManaUsed = lunarEclipseCast.BaseManaCost / lunarEclipseCast.CastTime * lunarTime;
            float lunarManaGained = (lunarEclipseCast.BaseManaCost * omenOfClarityProcChance) +
                ((spellCrit + lunarEclipseCast.CriticalChanceModifier) * spellHit * moonkinFormProc) +
                (lunarEclipseCast.BaseCastTime / 60.0f * calcs.BasicStats.ManaRestoreFromBaseManaPPM * CalculationsMoonkin.BaseMana);

            float preSolarTime = timeToProcSolar + (lunarEclipseCast.CastTime * 0.5f) + preSolarCast.NGCastTime;
            float preSolarDPS = preSolarCast.DamagePerHit / preSolarCast.CastTime;
            float preSolarManaUsed = preSolarCast.BaseManaCost / preSolarCast.CastTime * preSolarTime;
            float preSolarManaGained = (preSolarCast.BaseManaCost * omenOfClarityProcChance) +
                ((spellCrit + preSolarCast.CriticalChanceModifier) * spellHit * moonkinFormProc) +
                (preSolarCast.BaseCastTime / 60.0f * calcs.BasicStats.ManaRestoreFromBaseManaPPM * CalculationsMoonkin.BaseMana);

            float solarTime = (eclipseDuration - (preSolarCast.NGCastTime) - (preLunarCast.CastTime * 0.5f)) * (1.0f - castRatio);
            float solarDPS = solarEclipseCast.DamagePerHit / solarEclipseCast.CastTime;
            float solarManaUsed = solarEclipseCast.BaseManaCost / solarEclipseCast.CastTime * solarTime;
            float solarManaGained = (solarEclipseCast.BaseManaCost * omenOfClarityProcChance) +
                ((spellCrit + solarEclipseCast.CriticalChanceModifier) * spellHit * moonkinFormProc) +
                (solarEclipseCast.BaseCastTime / 60.0f * calcs.BasicStats.ManaRestoreFromBaseManaPPM * CalculationsMoonkin.BaseMana);

            float rotationLength = (solarTime + lunarTime + preSolarTime + preLunarTime) / (1 - castRatio);

            float etSpentOnMFMFPriority = rotationLength * moonfireRatio;
            float etSpentOnISMFPriority = rotationLength * castRatio - etSpentOnMFMFPriority;
            float etSpentOnISISPriority = rotationLength * insectSwarmRatio;
            float etSpentOnMFISPriority = rotationLength * castRatio - etSpentOnISISPriority;

            float moonfireDamagePerCastTime = moonfireRatio > 0 ? (moonfire.DamagePerHit + moonfire.DotEffect.DamagePerHit) / moonfire.CastTime : 0.0f;
            float insectSwarmDamagePerCastTime = insectSwarmRatio > 0 ? insectSwarm.DotEffect.DamagePerHit / insectSwarm.CastTime : 0.0f;

            float dotDamageMoonfirePriority = etSpentOnMFMFPriority * moonfireDamagePerCastTime + etSpentOnISMFPriority * insectSwarmDamagePerCastTime;
            float dotDamageInsectSwarmPriority = etSpentOnISISPriority * insectSwarmDamagePerCastTime + etSpentOnMFISPriority * moonfireDamagePerCastTime;

            float totalDotDamage = 0.0f;

            if (dotDamageMoonfirePriority > dotDamageInsectSwarmPriority)
            {
                moonfireCasts = moonfireRatio > 0 ? etSpentOnMFMFPriority / moonfire.CastTime : 0.0f;
                insectSwarmCasts = insectSwarmRatio > 0 ? etSpentOnISMFPriority / insectSwarm.CastTime : 0.0f;
                totalDotDamage = dotDamageMoonfirePriority;
            }
            else
            {
                moonfireCasts = moonfireRatio > 0 ? etSpentOnMFISPriority / moonfire.CastTime : 0.0f;
                insectSwarmCasts = insectSwarmRatio > 0 ? etSpentOnISISPriority / insectSwarm.CastTime : 0.0f;
                totalDotDamage = dotDamageInsectSwarmPriority;
            }

            float moonfireTicks = moonfire != null ? moonfireCasts * moonfire.DotEffect.NumberOfTicks : 0.0f;
            float insectSwarmTicks = insectSwarm != null ? insectSwarmCasts * insectSwarm.DotEffect.NumberOfTicks : 0.0f;

            float moonfireManaUsed = moonfire != null ? moonfireCasts * moonfire.BaseManaCost : 0.0f;
            float insectSwarmManaUsed = insectSwarm != null ? insectSwarmCasts * insectSwarm.BaseManaCost : 0.0f;

            float damageDone = preSolarTime * preSolarDPS + solarTime * solarDPS + preLunarTime * preLunarDPS + lunarTime * lunarDPS +
                totalDotDamage;

            Duration = rotationLength;
            DotTicks = moonfireTicks + insectSwarmTicks;
            InsectSwarmTicks = insectSwarmTicks;
            MoonfireTicks = moonfireTicks;
            MoonfireCasts = moonfireCasts;
            InsectSwarmCasts = insectSwarmCasts;
            CastCount = castsToProcLunar + (lunarTime / lunarEclipseCast.CastTime) + castsToProcSolar + (solarTime / solarEclipseCast.CastTime) + moonfireCasts + insectSwarmCasts;

            WrathCount = castsToProcLunar + (solarTime / solarEclipseCast.CastTime);
            StarfireCount = castsToProcSolar + (lunarTime / lunarEclipseCast.CastTime);

            ManaUsed = preSolarManaUsed + solarManaUsed + preLunarManaUsed + lunarManaUsed + moonfireManaUsed + insectSwarmManaUsed;
            ManaGained = castsToProcSolar * preSolarManaGained + (solarTime / solarEclipseCast.CastTime) * solarManaGained + castsToProcLunar * preLunarManaGained + (lunarTime / lunarEclipseCast.CastTime) * lunarManaGained;

            if (spellCrit + lunarEclipseCast.CriticalChanceModifier > StarfireEclipseCrit)
            {
                StarfireEclipseCrit = spellCrit + lunarEclipseCast.CriticalChanceModifier;
            }
            if (spellCrit + preSolarCast.CriticalChanceModifier > StarfireNonEclipseCrit)
            {
                StarfireNonEclipseCrit = spellCrit + preSolarCast.CriticalChanceModifier;
            }

            InsectSwarmDuration = insectSwarm != null ? insectSwarm.DotEffect.Duration : 0.0f;
            MoonfireDuration = moonfire != null ? moonfire.DotEffect.Duration : 0.0f;

            float mfSavingsFromOoC = moonfire != null ? (moonfire.BaseManaCost - (moonfire.BaseManaCost *
                (1 - StarfireCount / WrathCount * 0.06f - (1 - StarfireCount / WrathCount) * 0.06f))) : 0.0f;
            float isSavingsFromOoC = insectSwarm != null ? (insectSwarm.BaseManaCost - (insectSwarm.BaseManaCost *
                (1 - StarfireCount / WrathCount * 0.06f - (1 - StarfireCount / WrathCount) * 0.06f))) : 0.0f;
            ManaGained += moonfire != null ? (moonfireCasts * (mfSavingsFromOoC +
                ((spellCrit + moonfire.CriticalChanceModifier) * moonkinFormProc * spellHit)
                + 1.5f / 60.0f * calcs.BasicStats.ManaRestoreFromBaseManaPPM * CalculationsMoonkin.BaseMana)) : 0.0f;
            ManaGained += insectSwarm != null ? (insectSwarmCasts * (isSavingsFromOoC + 1.5f / 60.0f * calcs.BasicStats.ManaRestoreFromBaseManaPPM * CalculationsMoonkin.BaseMana)) : 0.0f;

            RotationData.ManaGained = ManaGained;
            RotationData.DPM = damageDone / ManaUsed;
            ManaUsed -= ManaGained;
            RotationData.ManaUsed = ManaUsed;

            // Undo SF glyph
            if (starfireGlyph && moonfire != null) moonfire.DotEffect.Duration -= 9.0f;

            // Undo improved Insect Swarm
            if (insectSwarm != null)
            {
                preLunarCast.AllDamageModifier /= 1 + 0.01f * impInsectSwarm;
            }
            if (moonfire != null)
            {
                preSolarCast.CriticalChanceModifier -= 0.01f * impInsectSwarm;
            }

            return damageDone;
        }
    }
}
