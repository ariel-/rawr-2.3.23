using System;
using System.Collections.Generic;

namespace Rawr.Moonkin
{
    // Define delegate types for proc effect public class
    // Enable and disable the effect of the proc.  These two delegates should perform exact opposite operations.
    public delegate void Activate(Character theChar, CharacterCalculationsMoonkin calcs, ref float spellPower, ref float spellHit, ref float spellCrit, ref float spellHaste);
    public delegate void Deactivate(Character theChar, CharacterCalculationsMoonkin calcs, ref float spellPower, ref float spellHit, ref float spellCrit, ref float spellHaste);
    // Calculate the uptime of the effect.  This will be used to weight the proc when calculating the rotational DPS.
    public delegate float UpTime(SpellRotation rotation, CharacterCalculationsMoonkin calcs);
    // Optional calculations for complicated proc effects like Eclipse or trinkets that proc additional damage.
    // The return value of this calculation will be ADDED to the rotational DPS.
    public delegate float CalculateDPS(SpellRotation rotation, CharacterCalculationsMoonkin calcs, float spellPower, float spellHit, float spellCrit, float spellHaste);
    // The return value of this calculation will be used to adjust the mana statistics of the rotation.
    public delegate float CalculateMP5(SpellRotation rotation, CharacterCalculationsMoonkin calcs, float spellPower, float spellHit, float spellCrit, float spellHaste);

    // The proc effect public class itself.
    // NOTE: Adding constructor with special effect to allow efficient construction of the proc list in a loop.
    public class ProcEffect
    {
        public ProcEffect() { }
        public ProcEffect(SpecialEffect effect)
        {
            this.Effect = effect;
            // Shadow damage procs - most widely varied at the current moment
            if (effect.Stats.ShadowDamage > 0)
            {
                CalculateDPS = delegate(SpellRotation r, CharacterCalculationsMoonkin c, float sp, float sHi, float sc, float sHa)
                {
                    SpecialEffect e = Effect;
                    float specialDamageModifier = (1 + c.BasicStats.BonusSpellPowerMultiplier) * (1 + c.BasicStats.BonusShadowDamageMultiplier) * (1 + c.BasicStats.BonusDamageMultiplier);
                    float triggerInterval = 0.0f;
                    switch (e.Trigger)
                    {
                        case Trigger.DoTTick:       // Extract
                            triggerInterval = r.Duration / r.DotTicks;
                            break;
                        case Trigger.SpellHit:      // Pendulum
                            triggerInterval = r.Duration / r.CastCount;
                            break;
                        case Trigger.DamageDone:    // DMC: Death
                            triggerInterval = r.Duration / (r.CastCount + r.DotTicks);
                            break;
                        case Trigger.DamageOrHealingDone:    // DMC: Greatness
                            // Need to add Self-Heals
                            triggerInterval = r.Duration / (r.CastCount + r.DotTicks);
                            break;
                        default:
                            return 0.0f;
                    }
                    float procsPerSecond = e.GetAverageProcsPerSecond(triggerInterval, 1.0f, 3.0f, c.FightLength * 60.0f);
                    return e.Stats.ShadowDamage * specialDamageModifier * procsPerSecond;
                };
            }
            // Lightning Capacitor, Thunder Capacitor, Reign of the Unliving/Undead, Nibelung
            else if (effect.Stats.NatureDamage > 0 || effect.Stats.FireDamage > 0 || effect.Stats.ValkyrDamage > 0)
            {
                if (effect.Stats.NatureDamage > 0)
                {
                    CalculateDPS = delegate(SpellRotation r, CharacterCalculationsMoonkin c, float sp, float sHi, float sc, float sHa)
                    {
                        float specialDamageModifier = (1 + c.BasicStats.BonusSpellPowerMultiplier) * (1 + c.BasicStats.BonusNatureDamageMultiplier) * (1 + c.BasicStats.BonusDamageMultiplier);
                        float procsPerSecond = Effect.GetAverageProcsPerSecond(r.Duration / (r.CastCount * sc), 1.0f, 3.0f, c.FightLength * 60.0f);
                        return Effect.Stats.NatureDamage * specialDamageModifier * procsPerSecond;
                    };
                }
                else if (effect.Stats.FireDamage > 0)
                {
                    CalculateDPS = delegate(SpellRotation r, CharacterCalculationsMoonkin c, float sp, float sHi, float sc, float sHa)
                    {
                        float specialDamageModifier = (1 + c.BasicStats.BonusSpellPowerMultiplier) * (1 + c.BasicStats.BonusFireDamageMultiplier) * (1 + c.BasicStats.BonusDamageMultiplier);
                        float procsPerSecond = Effect.GetAverageProcsPerSecond(r.Duration / (r.CastCount * sc), 1.0f, 3.0f, c.FightLength * 60.0f);
                        return Effect.Stats.FireDamage * specialDamageModifier * procsPerSecond;
                    };
                }
                else
                {
                    CalculateDPS = delegate(SpellRotation r, CharacterCalculationsMoonkin c, float sp, float sHi, float sc, float sHa)
                    {
                        float specialDamageModifier = (1 + c.BasicStats.BonusSpellPowerMultiplier) * (1 + c.BasicStats.BonusDamageMultiplier);
                        float procsPerSecond = Effect.GetAverageProcsPerSecond(r.Duration / (r.CastCount * sc), 1.0f, 3.0f, c.FightLength * 60.0f);
                        return Effect.Stats.ValkyrDamage * specialDamageModifier * procsPerSecond;
                    };
                }
            }
            else if (effect.Stats.Mp5 > 0)
            {
                CalculateMP5 = delegate(SpellRotation r, CharacterCalculationsMoonkin c, float sp, float sHi, float sc, float sHa)
                {
                    SpecialEffect e = Effect;
                    float procsPerSecond = e.GetAverageProcsPerSecond(r.Duration / r.CastCount, 1.0f, 3.0f, c.FightLength * 60f);
                    return (e.Stats.Mp5 / 5.0f * e.Duration) * procsPerSecond * 5.0f;
                };
            }
            // Moonkin 4T8 set bonus (15% chance on IS tick to proc an instant-cast Starfire)
            else if (effect.Stats.StarfireProc == 1)
            {
                CalculateDPS = delegate(SpellRotation r, CharacterCalculationsMoonkin c, float sp, float sHi, float sc, float sHa)
                {
                    if (r.InsectSwarmTicks == 0) return 0.0f;
                    Spell newSF = new Spell()
                    {
                        AllDamageModifier = r.Solver.Starfire.AllDamageModifier,
                        BaseCastTime = 1.5f,
                        BaseDamage = r.Solver.Starfire.BaseDamage,
                        BaseManaCost = r.Solver.Starfire.BaseManaCost,
                        CriticalChanceModifier = r.Solver.Starfire.CriticalChanceModifier,
                        CriticalDamageModifier = r.Solver.Starfire.CriticalDamageModifier,
                        DotEffect = null,
                        IdolExtraSpellPower = r.Solver.Starfire.IdolExtraSpellPower,
                        Name = r.Solver.Starfire.Name,
                        School = r.Solver.Starfire.School,
                        SpellDamageModifier = r.Solver.Starfire.SpellDamageModifier
                    };
                    r.DoSpecialStarfire(c, ref newSF, sp, sHi, sc, sHa);
                    float timeBetweenProcs = r.Solver.InsectSwarm.DotEffect.TickLength / Effect.Chance;
                    float replaceWrathWithSFDPS = (newSF.DamagePerHit / newSF.CastTime) - (r.Solver.Wrath.DamagePerHit / r.Solver.Wrath.CastTime);
                    float replaceSFWithSFDPS = (newSF.DamagePerHit / newSF.CastTime) - (r.Solver.Starfire.DamagePerHit / r.Solver.Starfire.CastTime);
                    return (replaceWrathWithSFDPS * (r.WrathCount / (r.WrathCount + r.StarfireCount)) +
                        replaceSFWithSFDPS * (r.StarfireCount / (r.WrathCount + r.StarfireCount)))
                        / timeBetweenProcs;
                };
            }
            else if (Effect.Stats._rawSpecialEffectDataSize == 0 && 
                (Effect.Trigger == Trigger.DamageDone ||
                Effect.Trigger == Trigger.DamageOrHealingDone ||
                Effect.Trigger == Trigger.DamageSpellCast ||
                Effect.Trigger == Trigger.DamageSpellCrit ||
                Effect.Trigger == Trigger.DamageSpellHit ||
                Effect.Trigger == Trigger.SpellCast ||
                Effect.Trigger == Trigger.SpellCrit ||
                Effect.Trigger == Trigger.SpellHit ||
                Effect.Trigger == Trigger.SpellMiss ||
                Effect.Trigger == Trigger.Use ||
                Effect.Trigger == Trigger.MoonfireCast ||
                Effect.Trigger == Trigger.InsectSwarmOrMoonfireTick ||
                Effect.Trigger == Trigger.MoonfireTick ||
                Effect.Trigger == Trigger.InsectSwarmTick ||
				Effect.Trigger == Trigger.DoTTick) &&
                (Effect.Stats.HasteRating > 0 ||
                Effect.Stats.SpellHaste > 0  ||
                Effect.Stats.CritRating > 0 ||
                Effect.Stats.HighestStat > 0))
            {
                Activate = delegate(Character ch, CharacterCalculationsMoonkin c, ref float sp, ref float sHi, ref float sc, ref float sHa)
                {
                    SpecialEffect e = Effect;
                    int maxStack = e.MaxStack;
                    Stats st = e.Stats;
                    float critRating = st.CritRating;
                    float spellCrit = StatConversion.GetSpellCritFromRating(critRating * maxStack);
                    float hasteRating = st.HasteRating;
                    float spellHaste = StatConversion.GetSpellHasteFromRating(hasteRating * maxStack);
                    spellHaste += st.SpellHaste;
                    float highestStat = st.HighestStat;

                    if (critRating > 0)
                        sc += spellCrit;
                    if (spellHaste > 0)
                        sHa += spellHaste;
                    if (highestStat > 0)
                    {
                        if (c.BasicStats.Spirit > c.BasicStats.Intellect)
                        {
                            Stats s = c.BasicStats.Clone();
                            s.Spirit += highestStat;
                            CharacterCalculationsMoonkin cNew = CalculationsMoonkin.GetInnerCharacterCalculations(ch, s, null);
                            storedStats.SpellPower = cNew.SpellPower - c.SpellPower;
                            sp += storedStats.SpellPower;
                        }
                        else
                        {
                            Stats s = c.BasicStats.Clone();
                            s.Intellect += highestStat;
                            CharacterCalculationsMoonkin cNew = CalculationsMoonkin.GetInnerCharacterCalculations(ch, s, null);
                            storedStats.SpellPower = cNew.SpellPower - c.SpellPower;
                            storedStats.SpellCrit = cNew.SpellCrit - c.SpellCrit;
                            sp += storedStats.SpellPower;
                            sc += storedStats.SpellCrit;
                        }
                    }
                };
                Deactivate = delegate(Character ch, CharacterCalculationsMoonkin c, ref float sp, ref float sHi, ref float sc, ref float sHa)
                {
                    SpecialEffect e = Effect;
                    int maxStack = e.MaxStack;
                    Stats st = e.Stats;
                    float critRating = st.CritRating;
                    float spellCrit = StatConversion.GetSpellCritFromRating(critRating * maxStack);
                    float hasteRating = st.HasteRating;
                    float spellHaste = StatConversion.GetSpellHasteFromRating(hasteRating * maxStack);
                    spellHaste += st.SpellHaste;
                    float highestStat = st.HighestStat;

                    if (critRating > 0)
                        sc -= spellCrit;
                    if (spellHaste > 0)
                        sHa -= spellHaste;
                    if (highestStat > 0)
                    {
                        sp -= storedStats.SpellPower;
                        if (c.BasicStats.Intellect >= c.BasicStats.Spirit)
                            sc -= storedStats.SpellCrit;
                    }
                };
                UpTime = delegate(SpellRotation r, CharacterCalculationsMoonkin c)
                {
                    float upTime = 0.0f;
                    switch (Effect.Trigger)
                    {
                        case Trigger.Use:
                            upTime = Effect.GetAverageUptime(0f, 1f);
                            break;
                        case Trigger.SpellHit:
                        case Trigger.DamageSpellHit:
                            upTime = Effect.GetAverageUptime(r.Duration / r.CastCount, r.Solver.GetSpellHit(c));
                            break;
                        case Trigger.DamageSpellCrit:
                        case Trigger.SpellCrit:
                            upTime = Effect.GetAverageUptime(r.Duration / (r.CastCount - (r.InsectSwarmTicks / r.Solver.InsectSwarm.DotEffect.NumberOfTicks)), c.SpellCrit);
                            break;
                        case Trigger.SpellCast:
                        case Trigger.DamageSpellCast:
                            upTime = Effect.GetAverageUptime(r.Duration / r.CastCount, 1f);
                            break;
                        case Trigger.MoonfireCast:
                            upTime = Effect.GetAverageUptime(r.Duration / r.MoonfireCasts, 1f);
                            break;
                        case Trigger.InsectSwarmOrMoonfireTick:
                            upTime = Effect.GetAverageUptime(r.Duration / (r.InsectSwarmTicks + r.MoonfireTicks), 1f);
                            break;
                        case Trigger.MoonfireTick:
                            upTime = Effect.GetAverageUptime(r.Duration / r.MoonfireTicks, 1f);
                            break;
                        case Trigger.InsectSwarmTick:
                            upTime = Effect.GetAverageUptime(r.Duration / r.InsectSwarmTicks, 1f);
                            break;
                        case Trigger.DoTTick:
                            upTime = Effect.GetAverageUptime(r.Duration / (r.MoonfireTicks + r.InsectSwarmTicks), 1f);
                            break;
                        case Trigger.DamageDone:
                        case Trigger.DamageOrHealingDone:
                            upTime = Effect.GetAverageUptime(((r.Duration / r.CastCount) + (r.Duration / (r.MoonfireTicks + r.InsectSwarmTicks))) / 2.0f, 1f);
                            break;
                        default:
                            break;
                    }
                    return upTime;
                };
            }
        }
        private Stats storedStats = new Stats();
        public SpecialEffect Effect { get; set; }
        public Activate Activate { get; set; }
        public Deactivate Deactivate { get; set; }
        public UpTime UpTime { get; set; }
        public CalculateDPS CalculateDPS { get; set; }
        public CalculateMP5 CalculateMP5 { get; set; }
    }
    // Combination generator public class
    // Helper public class to generate all possible combinations of proc effects to mathematically account for interactions.
    public class CombinationGenerator
    {
        int[] a;
        int n;
        int r;
        long numLeft;
        long total;
        public CombinationGenerator(int n, int r)
        {
            if (r > n) throw new ArgumentException("r must be less than or equal to n");
            if (n < 1) throw new ArgumentException("n must be greater than or equal to 1");

            this.n = n;
            this.r = r;
            a = new int[r];
            long nFact = GetFactorial(n);
            long rFact = GetFactorial(r);
            long nMinusRFact = GetFactorial(n - r);
            total = nFact / (rFact * nMinusRFact);
            Reset();
        }

        public bool HasNext()
        {
            return numLeft > 0;
        }

        public int[] GetNext()
        {
            if (numLeft == total)
            {
                --numLeft;
                return a;
            }

            int i = r - 1;
            while (a[i] == n - r + i)
            {
                --i;
            }
            a[i]++;
            for (int j = i + 1; j < r; ++j)
            {
                a[j] = a[i] + j - i;
            }
            --numLeft;
            return a;
        }

        private void Reset()
        {
            for (int i = 0; i < a.Length; ++i)
            {
                a[i] = i;
            }
            numLeft = total;
        }

        private long GetFactorial(int p)
        {
            long fact = 1;
            for (int i = p; i > 1; --i)
            {
                fact *= i;
            }
            return fact;
        }
    }
}
