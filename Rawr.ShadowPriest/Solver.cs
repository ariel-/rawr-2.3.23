using System;
using System.Collections.Generic;
using System.Text;
//using System.Diagnostics;

namespace Rawr.ShadowPriest
{

    public class SolverBase
    {
        private static SpecialEffect[] _seSpiritTap = new SpecialEffect[] {
            null,
            new SpecialEffect(
                Trigger.SpellCrit, 
                new Stats { BonusSpiritMultiplier = 0.05f, SpellCombatManaRegeneration = 0.17f },
                8f, 0f),
            new SpecialEffect(
                Trigger.SpellCrit, 
                new Stats { BonusSpiritMultiplier = 0.10f, SpellCombatManaRegeneration = 0.33f },
                8f, 0f),
        };

        private static SpecialEffect[] _seGlyphOfShadow = new SpecialEffect[] {
            null,
            new SpecialEffect(
                Trigger.SpellHit,
                new Stats {},
                10f, 0f),
        };

        public List<Spell> SpellPriority { get; protected set; }
        public List<Spell> SpellSimulation { get; protected set; }
        public float OverallDamage { get; protected set; }
        public float DPS { get; protected set; }
        public float OverallMana { get; protected set; }
        public float SustainDPS { get; protected set; }
        public float MovementDamageLoss { get; protected set; }
        public Dictionary<float, Spell> Sequence { get; protected set; }

        public CalculationOptionsShadowPriest CalcOpts { get; set; }
        public BossOptions BossOpts { get; set; }
        public Stats PlayerStats { get; set; }
        public Character character { get; set; }
        public float HitChance { get; set; }
        public List<Trinket> Trinkets { get; set; }
        public List<ManaSource> ManaSources { get; set; }
        public SpecialEffect seSpiritTap, seGlyphofShadow;
        public float SpellDamageFromSpiritPercentage;

        public string Name { get; protected set; }
        public string Rotation { get; protected set; }

        public class Trinket
        {
            public float DamageBonus { get; set; }
            public float HasteBonus { get; set; }
            public float Cooldown { get; set; }
            public float CooldownTimer { get; set; }
            public float UpTime { get; set; }
            public float UpTimeTimer { get; set; }
        }

        public class ManaSource
        {
            public string Name { get; set; }
            public float Value { get; set; }

            public ManaSource(string name, float value)
            {
                Name = name; Value = value;
            }
        }

        public class SpellComparerDpCT: IComparer<Spell>
        {
            public int Compare(Spell a, Spell b)
            {
                if (a == null)
                {
                    if (b == null)
                        return 0;
                    return -1;
                }
                else if (b == null)
                    return 1;
                return (int)Math.Round(b.DpCT - a.DpCT);
            }
        }

        public class SpellComparerDpM : IComparer<Spell>
        {
            public int Compare(Spell a, Spell b)
            {
                if (a == null)
                {
                    if (b == null)
                        return 0;
                    return -1;
                }
                else if (b == null)
                    return 1;
                return (int)Math.Round((b.DpM - a.DpM) * 100f);
            }
        }

        protected float _GetDirectDamage(Stats stats)
        {
            return stats.ArcaneDamage
                + stats.FireDamage
                + stats.FrostDamage
                + stats.HolyDamage
                + stats.NatureDamage
                + stats.ShadowDamage
                + stats.ValkyrDamage;
        }

        public SolverBase(Stats playerStats, Character _char) 
        {
            character = _char;
            Name = "Base";
            Rotation = "None";
            SpellDamageFromSpiritPercentage = _char.PriestTalents.SpiritualGuidance * 0.05f + _char.PriestTalents.TwistedFaith * 0.04f;

            // USE trinkets & effects.
            Stats Twinkets = new Stats();
            foreach (SpecialEffect se in playerStats.SpecialEffects())
            {
                if (se.Stats.ManaRestore == 0 && se.Stats.Mp5 == 0 && se.Stats.SpellPower == 0)
                {
                    if (se.Trigger == Trigger.Use)
                        Twinkets += se.GetAverageStats();
                    else if (se.Trigger == Trigger.DamageSpellCast
                        || se.Trigger == Trigger.SpellCast)
                    {
                        if (se.Stats.HighestStat > 0)
                        {
                            float greatnessProc = se.GetAverageStats(2f, 1f).HighestStat;
                            if (playerStats.Spirit > playerStats.Intellect)
                                Twinkets.Spirit += greatnessProc;
                            else
                                Twinkets.Intellect += greatnessProc;
                        }
                    }
                }
            }

            Twinkets.Spirit = (float)Math.Round(Twinkets.Spirit * (1 + playerStats.BonusSpiritMultiplier));
            Twinkets.Intellect = (float)Math.Round(Twinkets.Intellect * (1 + playerStats.BonusIntellectMultiplier));
            Twinkets.SpellPower += (float)Math.Round(Twinkets.Spirit * SpellDamageFromSpiritPercentage);
            playerStats.Accumulate(Twinkets);

            CalcOpts = character.CalculationOptions as CalculationOptionsShadowPriest;
            BossOpts = character.BossOptions;

#if RAWR3 || SILVERLIGHT
            HitChance = playerStats.SpellHit * 100f + 100 - (StatConversion.GetSpellMiss(character.Level - BossOpts.Level, false) * 100);
#else
            HitChance = playerStats.SpellHit * 100f + 100 - (StatConversion.GetSpellMiss(-CalcOpts.TargetLevel, false) * 100);
#endif
            if (!character.ActiveBuffsConflictingBuffContains("Spell Hit Chance Taken"))
                HitChance += character.PriestTalents.Misery * 1f;
            if (character.Race == CharacterRace.Draenei && !character.ActiveBuffsContains("Heroic Presence"))
                HitChance += 1;
            HitChance = (float)Math.Min(100f, HitChance);

            Trinkets = new List<Trinket>();
            Sequence = new Dictionary<float, Spell>();
            ManaSources = new List<ManaSource>();

            // Spirit Tap
            if (character.PriestTalents.ImprovedSpiritTap > 0 && character.PriestTalents.ImprovedSpiritTap < 3)
                seSpiritTap = _seSpiritTap[character.PriestTalents.ImprovedSpiritTap];
            // Glyph of Shadow
            if (character.PriestTalents.GlyphofShadow)
                seGlyphofShadow = _seGlyphOfShadow[1];

            PlayerStats = playerStats;      
        }

        public virtual void Calculate(CharacterCalculationsShadowPriest calculatedStats)
        {
            DPS = 0;
            SustainDPS = 0;
        }

        public Spell GetCastSpell(float timer)
        {   // FIXME: Rewrite this freaking crappy shit.
            foreach (Spell spell in SpellPriority)
            {
                if ((spell.DebuffDuration > 0) && (spell.CastTime > 0) && (spell.SpellStatistics.CooldownReset < (spell.CastTime + timer)))
                    return spell;   // Special case for dots that have cast time (Holy Fire / Vampiric Touch)
                //if (spell.SpellStatistics.CooldownReset <= timer && spell.Cooldown > 0)
                if (spell.SpellStatistics.CooldownReset <= timer)
                    return spell;
                if (spell.SpellStatistics.CooldownReset > 0 
                    && (spell.SpellStatistics.CooldownReset - (spell.DebuffDuration > 0 ? spell.CastTime : 0) - timer < spell.GlobalCooldown * 0.2f))
                    return null;
                if (spell.SpellStatistics.CooldownReset <= timer)
                    return spell;
            }
            return null;
        }

        public float OldGetCastSpell(float timer, out Spell castSpell, Spell SWD)
        {   // FIXME: Rewrite this freaking crappy shit.
            castSpell = null;
            foreach (Spell spell in SpellPriority)
            {
                castSpell = spell;
                if ((spell.DebuffDuration > 0) && (spell.CastTime > 0) && (spell.SpellStatistics.CooldownReset < (spell.CastTime + timer)))
                    return 0;   // Special case for dots that have cast time (Holy Fire / Vampiric Touch)
                //if (spell.SpellStatistics.CooldownReset <= timer && spell.Cooldown > 0)
                if (spell.SpellStatistics.CooldownReset <= timer)
                    return 0;
                if (spell.SpellStatistics.CooldownReset > 0)
                {
                    float nextCast = spell.SpellStatistics.CooldownReset - (spell.DebuffDuration > 0 ? spell.CastTime : 0) - timer;
                    if (nextCast < 0.5f)
                        return nextCast;
                    /*if (nextCast <= spell.GlobalCooldown && SWD != null & SWD.SpellStatistics.CooldownReset <= timer)
                    {
                        castSpell = SWD;
                        return 0f;
                    }*/
                }
                if (spell.SpellStatistics.CooldownReset <= timer)
                    return 0;
            }
            castSpell = null;
            return 0;
        }

        
        public Spell NewGetCastSpell(float timer)
        {
            float[] nextRelativeCastTime = new float[SpellPriority.Count];
            
            int cnt = 0;
            foreach (Spell spell in SpellPriority)
            {
                if (spell.DebuffDuration > 0)
                    nextRelativeCastTime[cnt++] = spell.SpellStatistics.CooldownReset - spell.CastTime - timer;
                else if (spell.Cooldown > 0)
                    nextRelativeCastTime[cnt++] = spell.SpellStatistics.CooldownReset - timer;
                else
                {   // This is our filler.
                    nextRelativeCastTime[cnt++] = 0;
                    break;
                }
            }

            for (int x = 0; x < cnt; x++)
                if (nextRelativeCastTime[x] < 0)
                    return SpellPriority[x];

            Spell spellCastNext = SpellPriority[cnt - 1];
            float fillerDPS = spellCastNext.DpCT;
            float nextDelayCost = fillerDPS;
            for (int x = 0; x < cnt - 1; x++)
            {
                float delayCost = SpellPriority[x].DpCT - nextRelativeCastTime[x] * fillerDPS;
                if (delayCost > nextDelayCost)
                {
                    nextDelayCost = delayCost;
                    spellCastNext = SpellPriority[x];
                }
            }
            return spellCastNext;
        }

        public void RecalculateHaste(Stats stats, float addedHasteRating)
        {
            foreach (Spell spell in SpellPriority)
                spell.RecalcHaste(stats, addedHasteRating);
        }

        public Spell GetSpellByName(string name)
        {
            foreach (Spell spell in SpellPriority)
            {
                if (spell.Name.Contains(name))
                    return spell;
            }
            return null;
        }

        public void UpTrinket(float timer)
        {
            foreach (Trinket trinket in Trinkets)
                if (trinket.CooldownTimer <= timer)
                {
                    trinket.CooldownTimer = timer + trinket.Cooldown;
                    trinket.UpTimeTimer = timer + trinket.UpTime;
                }
        }

        public void GetTrinketBuff(float timer, Stats stats)
        {
            foreach (Trinket trinket in Trinkets)
                if (trinket.UpTimeTimer > timer)
                {
                    stats.HasteRating += trinket.HasteBonus;
                    stats.SpellPower += trinket.DamageBonus;
                }
        }
    }

    public class solverShadow : SolverBase
    {   // Models Full Rotation
        protected float ShadowHitChance { get; set; }
        protected ShadowWordPain  SWP { get; set; }
        protected MindFlay MF { get; set; }
        protected VampiricEmbrace VE { get; set; }
        protected ShadowWordDeath SWD { get; set; }
        protected MindBlast MB { get; set; }
        protected DevouringPlague DP { get; set; }
        protected bool bPnS { get; set; }

/*        public Spell GetCastSpell(float timer)
        {   // FIXME: Rewrite this freaking crappy shit.
            foreach (Spell spell in SpellPriority)
            {
                if ((spell.DebuffDuration > 0) && (spell.CastTime > 0) && (spell.SpellStatistics.CooldownReset < (spell.CastTime + timer)))
                    return spell;   // Special case for dots that have cast time (Holy Fire / Vampiric Touch)
                //if (spell.SpellStatistics.CooldownReset <= timer && spell.Cooldown > 0)
                if (spell.SpellStatistics.CooldownReset <= timer)
                    return spell;
                if (spell.SpellStatistics.CooldownReset > 0
                    && (spell.SpellStatistics.CooldownReset - (spell.DebuffDuration > 0 ? spell.CastTime : 0) - timer < spell.GlobalCooldown * 0.2f))
                    return null;
                if (spell.SpellStatistics.CooldownReset <= timer)
                    return spell;
            }
            return null;
        }*/

        public Spell NewGetCastSpell2(float timer)
        {
            bool bDoFlay = true;
            foreach (Spell spell in SpellPriority)
            {
                if (spell == MF)
                    break;
                if ((spell.DebuffDuration > 0) && (spell.CastTime > 0) && (spell.SpellStatistics.CooldownReset < (spell.CastTime + timer)))
                    return spell; // For Vampiric Touch, when VT is at or over time to cast
                if (spell.SpellStatistics.CooldownReset <= timer)
                    return spell; // For SWP, DP, MB
                // This is where we gotta check if MF is better than waiting.
                float needToBeCast = (spell.DebuffDuration > 0) ? 
                    (spell.SpellStatistics.CooldownReset - spell.CastTime - timer) : // SWP, DP, VT
                    spell.SpellStatistics.CooldownReset - timer;      // MB, SWD
                if (needToBeCast < MF.CastTime)
                {
                    // Calculate how much DPS we lose by doing absolutely nothing
                    float DPSLossDoNothing = spell.DpS * needToBeCast;
                    float DPSLossDoFillerTime = MF.CastTime - needToBeCast;
                    // Calculate how much DPS we gain by doing Mind Flay instead, postponing this spell
                    float DPSLossDoFiller = MF.DpS * DPSLossDoFillerTime - spell.DpS * DPSLossDoFillerTime;
                    // If we lose less DPS by doing nothing than doing a Mind Flay, then.. don't Mind Flay.
                    if (DPSLossDoNothing < DPSLossDoFiller)
                        bDoFlay = false;
                }
            }
            if (bDoFlay)
                return MF;
            else
                return null;
        }

        public float NewGetCastSpell3(float timer, out Spell castSpell)
        {
            castSpell = null;
            foreach (Spell spell in SpellPriority)
                if (spell.DebuffDuration == 0 && spell.Cooldown == 0)
                {
                    castSpell = spell;
                    break;
                }

            int idx = SpellPriority.IndexOf(castSpell);
            float spellDelay = 0f;

            for (int x = idx - 1; x >= 0; x--)
            {
                Spell thisSpell = SpellPriority[x];
                if ((thisSpell.DebuffDuration > 0) &&
                    (thisSpell.CastTime > 0) &&
                    (thisSpell.SpellStatistics.CooldownReset < (thisSpell.CastTime + timer)))
                    castSpell = thisSpell;
                else if (thisSpell.SpellStatistics.CooldownReset <= timer)
                    castSpell = thisSpell;
                else
                {
                    float nextTimeToCastThis = (thisSpell.DebuffDuration > 0) ?
                        (thisSpell.SpellStatistics.CooldownReset - thisSpell.CastTime - timer) :    // VT, SWP, DP
                        (thisSpell.SpellStatistics.CooldownReset - timer);                          // MB, SWD
                    
                    if (nextTimeToCastThis < MF.CastTime)
                    {
                        float completeTimeNext = (castSpell.CastTime > 0) ?
                            (castSpell.CastTime) :                                              // VT, MB, MF
                            (castSpell.GlobalCooldown)                                          // SWP, DP, SWD
                            + castSpell.SpellStatistics.CooldownReset;
                        float DPSLossDoNothing = castSpell.DpS * (float)Math.Max(castSpell.GlobalCooldown, nextTimeToCastThis);
                        float DPSLossNextSpellTime = completeTimeNext - nextTimeToCastThis;
                        float DPSLossDoNext = castSpell.DpS * DPSLossNextSpellTime - thisSpell.DpS * DPSLossNextSpellTime;
                        if (DPSLossDoNothing < DPSLossDoNext)
                        {
                            spellDelay = nextTimeToCastThis;
                            castSpell = thisSpell;
                        }
                    }
                }
            }

            return spellDelay;
        }

        public float NewGetCastSpell4(float timer, out Spell castSpell)
        {
            float spellWait = 0f;
            castSpell = null;
            foreach (Spell spell in SpellPriority)
                if (spell.DebuffDuration == 0 && spell.Cooldown == 0)
                {
                    castSpell = spell;
                    break;  // We just found us a filler.
                }

            int idx = SpellPriority.IndexOf(castSpell);
            float score = castSpell.GlobalCooldown;
            for (int x = 0; x < idx; x++)
            {
                Spell thisSpell = SpellPriority[x];

                float nextCast = 0f;
                if (thisSpell.DebuffDuration > 0)   // VT, SWP, DP
                    nextCast = thisSpell.SpellStatistics.CooldownReset - thisSpell.CastTime - timer;
                else if (thisSpell.Cooldown > 0)    // SWD, MB
                    nextCast = thisSpell.SpellStatistics.CooldownReset - timer;

                if (nextCast <= 0)
                {
                    castSpell = thisSpell;
                    return 0f;
                }
                else if (nextCast >= castSpell.CastTime)
                    continue;

                if (nextCast < score)
                {
                    castSpell = thisSpell;
                    score = nextCast;
                    spellWait = (float)Math.Max(0f, nextCast);
                }
            }
            return spellWait;
        }

        public float NewGetCastSpell5(float timer, out Spell castSpell)
        {
            castSpell = null;
            float castDpC = 0f;
            float spellWait = 0f;
            foreach (Spell spell in SpellPriority)
            {
                float intervalTime = 0, wait = 0;
                if (spell.DebuffDuration > 0)   // SWP, VT, DP
                {
                    intervalTime = spell.DebuffDuration;
                    wait = spell.SpellStatistics.CooldownReset - timer - spell.CastTime;
                }
                else if (spell.Cooldown > 0)
                {
                    if (spell.CastTime > 0)  // MB
                        intervalTime = spell.Cooldown + spell.CastTime;
                    else // SWD
                        intervalTime = spell.Cooldown;
                    wait = spell.SpellStatistics.CooldownReset - timer;                    
                }
                else // Mind Flay
                    intervalTime = spell.CastTime;


                float damagePerCooldown = spell.AvgDamage / intervalTime;
                
                if (castSpell == null)
                {
                    castSpell = spell;
                    spellWait = wait;
                    castDpC = damagePerCooldown;
                }
                else
                {

                }             
            }
            return spellWait;
        }

        public float NewGetCastSpell7(float timer, out Spell castSpell)
        {
            // estimates overall dps by estimating impact of collisions
            // one downside of this particular implementation is that it assumes if
            // collision is present in this case it'll be present on each cycle
            // see #9 for alternative
            castSpell = null;
            float bestWaitTime = 0;
            float bestScore = float.NegativeInfinity;
            // evaluate each spell
            foreach (Spell spell in SpellPriority)
            {
                // how long to cast it
                float castTime = spell.CastTime;
                if (castTime == 0)
                {
                    castTime = spell.GlobalCooldown;
                }
                // how long do we have to wait to cast this spell
                float timeShare = 1;
                float waitTime = 0;
                if (spell.DebuffDuration > 0)
                {
                    waitTime = Math.Max(0, spell.SpellStatistics.CooldownReset - spell.CastTime - timer);    // VT, SWP, DP
                    timeShare = (waitTime + castTime) / (spell.DebuffDuration);
                }
                else if (spell.Cooldown > 0)
                {
                    waitTime = Math.Max(0, spell.SpellStatistics.CooldownReset - timer);                          // MB, SWD
                    timeShare = (waitTime + castTime) / (spell.Cooldown + castTime);
                }
                // estimate overall dps
                float dps = 0;
                float timeShareLeft = 1;
                foreach (Spell s in SpellPriority)
                {
                    float ct = s.CastTime;
                    float realWait = 0;
                    if (ct == 0)
                    {
                        ct = s.GlobalCooldown;
                    }
                    float t = 1;
                    if (s == spell)
                    {
                        t = timeShare;
                        realWait = waitTime;
                    }
                    else if (s.DebuffDuration > 0)
                    {
                        float sWait = Math.Max(0, s.SpellStatistics.CooldownReset - s.CastTime - timer);    // VT, SWP, DP
                        float delay = Math.Max(0, waitTime + castTime - sWait);
                        t = ct / (s.DebuffDuration + delay);
                    }
                    else if (s.Cooldown > 0)
                    {
                        float sWait = Math.Max(0, s.SpellStatistics.CooldownReset - timer);    // VT, SWP, DP
                        float delay = Math.Max(0, waitTime + castTime - sWait);
                        t = ct / (s.Cooldown + delay + ct);
                    }
                    if (t > timeShareLeft)
                    {
                        t = timeShareLeft;
                    }
                    dps += t * s.AvgDamage / (ct + realWait);
                    timeShareLeft -= t;
                    if (timeShareLeft <= 0)
                    {
                        break;
                    }
                }
                // is it better than what we had so far?
                if (dps > bestScore)
                {
                    bestScore = dps;
                    bestWaitTime = waitTime;
                    castSpell = spell;
                }
                if (spell.DebuffDuration == 0 && spell.Cooldown == 0)
                {
                    break;
                }
            }
            return bestWaitTime;
        }

        public float NewGetCastSpell8(float timer, out Spell castSpell)
        {
            // this is same as #7, but extended for window > 1
            // this method only makes sense for windows that are small enough that it is smaller than all debuff durations/cooldowns
            // otherwise you need to complicate a lot more
            // NOTE: turns out in practice this is not much smoother than #7, so probably not worth the computational effort
            castSpell = null;
            float bestWaitTime = 0;
            float bestScore = float.NegativeInfinity;
            int N = 0;
            for (int i = 0; i < SpellPriority.Count; i++)
            {
                Spell spell = SpellPriority[i];
                if (spell.DebuffDuration == 0 && spell.Cooldown == 0)
                {
                    N = i + 1;
                    break;
                }
            }
            int j;
            const int window = 2;
            int[] sequence = new int[window];
            float[] waitTime = new float[window];
            float[] timeShare = new float[window];
            bool[] used = new bool[N];
            // evaluate each spell
            do
            {
                // is it a valid sequence?
                bool valid = true;
                Array.Clear(used, 0, N);
                for (int i = 0; i < window; i++)
                {
                    if (sequence[i] != N - 1 && used[sequence[i]])
                    {
                        valid = false;
                        break;
                    }
                    used[sequence[i]] = true;
                }
                if (valid)
                {
                    // calculate waits and delays on the spells in the window sequence
                    float miniTimer = timer;
                    for (int i = 0; i < window; i++)
                    {
                        Spell spell = SpellPriority[sequence[i]];
                        // how long to cast it
                        float castTime = spell.CastTime;
                        if (castTime == 0)
                        {
                            castTime = spell.GlobalCooldown;
                        }
                        // how long do we have to wait to cast this spell
                        timeShare[i] = 1;
                        waitTime[i] = 0;
                        if (spell.DebuffDuration > 0)
                        {
                            float minWaitTime = Math.Max(0, spell.SpellStatistics.CooldownReset - spell.CastTime - timer);
                            float delay = Math.Max(0, miniTimer - timer - minWaitTime);
                            waitTime[i] = Math.Max(0, spell.SpellStatistics.CooldownReset - spell.CastTime - miniTimer);    // VT, SWP, DP
                            timeShare[i] = (waitTime[i] + castTime) / (spell.DebuffDuration + delay);
                        }
                        else if (spell.Cooldown > 0)
                        {
                            float minWaitTime = Math.Max(0, spell.SpellStatistics.CooldownReset - timer);
                            float delay = Math.Max(0, miniTimer - timer - minWaitTime);
                            waitTime[i] = Math.Max(0, spell.SpellStatistics.CooldownReset - miniTimer);                          // MB, SWD
                            timeShare[i] = (waitTime[i] + castTime) / (spell.Cooldown + delay + castTime);
                        }
                        miniTimer += castTime + waitTime[i];
                    }
                    // estimate overall dps
                    float dps = 0;
                    float timeShareLeft = 1;
                    for (int i = 0; i < N; i++)
                    {
                        Spell s = SpellPriority[i];
                        float ct = s.CastTime;
                        float realWait = 0;
                        if (ct == 0)
                        {
                            ct = s.GlobalCooldown;
                        }
                        float t = 1;
                        int index = Array.IndexOf(sequence, i);
                        if (index >= 0)
                        {
                            t = timeShare[index];
                            realWait = waitTime[index];
                        }
                        else if (s.DebuffDuration > 0)
                        {
                            float sWait = Math.Max(0, s.SpellStatistics.CooldownReset - s.CastTime - timer);    // VT, SWP, DP
                            float delay = Math.Max(0, miniTimer - timer - sWait);
                            t = ct / (s.DebuffDuration + delay);
                        }
                        else if (s.Cooldown > 0)
                        {
                            float sWait = Math.Max(0, s.SpellStatistics.CooldownReset - timer);    // VT, SWP, DP
                            float delay = Math.Max(0, miniTimer - timer - sWait);
                            t = ct / (s.Cooldown + delay + ct);
                        }
                        if (t > timeShareLeft)
                        {
                            t = timeShareLeft;
                        }
                        dps += t * s.AvgDamage / (ct + realWait);
                        timeShareLeft -= t;
                        if (timeShareLeft <= 0)
                        {
                            break;
                        }
                    }
                    // is it better than what we had so far?
                    if (dps > bestScore)
                    {
                        bestScore = dps;
                        bestWaitTime = waitTime[0];
                        castSpell = SpellPriority[sequence[0]];
                    }
                }
                // increment spell combination
                j = window - 1;
                sequence[j]++;
                while (sequence[j] >= N)
                {
                    sequence[j] = 0;
                    j--;
                    if (j < 0)
                    {
                        break;
                    }
                    sequence[j]++;
                }
            } while (j >= 0);
            return bestWaitTime;
        }

        public float NewGetCastSpell9(float timer, out Spell castSpell)
        {
            castSpell = null;
            float bestWaitTime = 0;
            float bestScore = float.NegativeInfinity;
            // evaluate each spell
            foreach (Spell spell in SpellPriority)
            {
                // how long to cast it
                float castTime = spell.CastTime;
                if (castTime == 0)
                {
                    castTime = spell.GlobalCooldown;
                }
                // how long do we have to wait to cast this spell
                float timeShare = 1;
                float waitTime = 0;
                float repeatInterval = 0;
                if (spell.DebuffDuration > 0)
                {
                    waitTime = Math.Max(0, spell.SpellStatistics.CooldownReset - spell.CastTime - timer);    // VT, SWP, DP
                    timeShare = (waitTime + castTime) / (spell.DebuffDuration);
                    repeatInterval = spell.DebuffDuration;
                }
                else if (spell.Cooldown > 0)
                {
                    waitTime = Math.Max(0, spell.SpellStatistics.CooldownReset - timer);                          // MB, SWD
                    timeShare = (waitTime + castTime) / (spell.Cooldown + castTime);
                    repeatInterval = spell.Cooldown + castTime;
                }
                // estimate overall dps
                float dps = 0;
                float timeShareLeft = 1;
                foreach (Spell s in SpellPriority)
                {
                    float ct = s.CastTime;
                    float realWait = 0;
                    if (ct == 0)
                    {
                        ct = s.GlobalCooldown;
                    }
                    float t = 1;
                    if (s == spell)
                    {
                        t = timeShare;
                        realWait = waitTime;
                    }
                    else if (s.DebuffDuration > 0)
                    {
                        float sWait = Math.Max(0, s.SpellStatistics.CooldownReset - s.CastTime - timer);    // VT, SWP, DP
                        float delay = Math.Max(0, waitTime + castTime - sWait);
                        // weight the delay by likelihood of collision
                        // very crude estimate, but helps discount collision of short cooldowns by long cooldowns
                        float collisionChance = (repeatInterval == 0) ? 1 : Math.Min(1, s.DebuffDuration / repeatInterval);
                        t = ct / (s.DebuffDuration + collisionChance * delay);
                    }
                    else if (s.Cooldown > 0)
                    {
                        float sWait = Math.Max(0, s.SpellStatistics.CooldownReset - timer);    // VT, SWP, DP
                        float delay = Math.Max(0, waitTime + castTime - sWait);
                        // weight the delay by likelihood of collision
                        // very crude estimate, but helps discount collision of short cooldowns by long cooldowns
                        float collisionChance = (repeatInterval == 0) ? 1 : Math.Min(1, (s.Cooldown + ct) / repeatInterval);
                        t = ct / (s.Cooldown + ct + collisionChance * delay);
                    }
                    if (t > timeShareLeft)
                    {
                        t = timeShareLeft;
                    }
                    dps += t * s.AvgDamage / (ct + realWait);
                    timeShareLeft -= t;
                    if (timeShareLeft <= 0)
                    {
                        break;
                    }
                }
                // is it better than what we had so far?
                if (dps > bestScore)
                {
                    bestScore = dps;
                    bestWaitTime = waitTime;
                    castSpell = spell;
                }
                if (spell.DebuffDuration == 0 && spell.Cooldown == 0)
                {
                    break;
                }
            }
            return bestWaitTime;
        }

        public class SpellInfo
        {
            public Spell Spell { get; protected set; }
            public float NextCast { get; protected set; }
            public SpellInfo(Spell spell, float timer)
            {
                Spell = spell;
                if (Spell.DebuffDuration > 0)
                    NextCast = Spell.SpellStatistics.CooldownReset - spell.CastTime - timer;
                else
                    NextCast = spell.SpellStatistics.CooldownReset - timer;
            }
        }

        public float NewGetCastSpell6(float timer, out Spell castSpell)
        {
            castSpell = null;
            float wait = 0f;
            List<SpellInfo> spellList = new List<SpellInfo>();

            foreach (Spell spell in SpellPriority)
            {
                if (spell.DebuffDuration > 0 || spell.Cooldown > 0)
                    spellList.Add(new SpellInfo(spell, timer));
                else
                {
                    castSpell = spell;
                    break;
                }
            }

            foreach (SpellInfo si in spellList)
            {
                if (si.NextCast > castSpell.CastTime)
                {
                    spellList.Remove(si);
                    continue;
                }
                else if (si.NextCast <= 0)
                {
                    castSpell = si.Spell;
                    return 0f;
                }
                else
                {

                }
            }

            return wait;
        }

        public solverShadow(Stats BasicStats, Character character)
            : base(BasicStats, character)
        {
            SpellPriority = new List<Spell>(CalcOpts.SpellPriority.Count);
            float baseMana = BaseStats.GetBaseStats(character).Mana;
            foreach (string spellname in CalcOpts.SpellPriority)
            {
                Spell spelltmp = SpellFactory.CreateSpell(spellname, PlayerStats, character, baseMana, CalcOpts.PTR);
                if (spelltmp != null) SpellPriority.Add(spelltmp);
            }

            SWP = GetSpellByName("Shadow Word: Pain") as ShadowWordPain;
            MF = GetSpellByName("Mind Flay") as MindFlay;
            VE = GetSpellByName("Vampiric Embrace") as VampiricEmbrace;
            SWD = GetSpellByName("Shadow Word: Death") as ShadowWordDeath;
            MB = GetSpellByName("Mind Blast") as MindBlast;
            DP = GetSpellByName("Devouring Plague") as DevouringPlague;

            if (VE != null)
            {   // Functional yet abysmal method of moving VE to bottom of priority.
                SpellPriority.Remove(VE);
                SpellPriority.Add(VE);
            }

            // With Pain and Suffering, Mind Flay has a 33/66/100% chance to refresh SW:P.
            // This mean we cast SW:P once, and reap the benefits later. Only use PnS if SWP is in the prio list.
            bPnS = (SWP != null) && (character.PriestTalents.PainAndSuffering > 0);
            if (bPnS)
            {   // Move SW:Pain to the end of the list in a very non-fashionable and hacky way.
                SpellPriority.Remove(SWP);
                SpellPriority.Add(SWP);
            }


            Name = "Shadow w/Mind Flay";
            Rotation = "Priority Based:";
            foreach (Spell spell in SpellPriority)
                Rotation += "\r\n- " + spell.Name;
            ShadowHitChance = (float)Math.Min(100f, HitChance + character.PriestTalents.ShadowFocus * 1f);
        }

        public override void Calculate(CharacterCalculationsShadowPriest calculatedStats)
        {
            Calculate(calculatedStats, true);
        }

        public void Calculate(CharacterCalculationsShadowPriest calculatedStats, bool bVerbal)
        {
            if (SpellPriority.Count == 0)
                return;

            // Make sure calculations are properly reset.
            foreach (Spell spell in SpellPriority)
                spell.SpellStatistics.Reset();
            RecalculateHaste(PlayerStats, 0);

            Stats simStats = PlayerStats.Clone();
            bool bTwistedFaith = character.PriestTalents.TwistedFaith > 0;
            float timer = 0;
            float hasteProcTimer = 0;
            int sequence = SpellPriority.Count-1;
            List<Spell> CastList = new List<Spell>();
#if RAWR3 || SILVERLIGHT
            int FightLength = (int)(BossOpts.BerserkTimer / 60f);
#else
            int FightLength = (int)CalcOpts.FightLength;
#endif

            // Initial SW:Pain application
            if (bPnS)
            {
                SWP.SpellStatistics.CooldownReset = timer + SWP.DebuffDuration;
                SWP.SpellStatistics.HitCount++;
                SWP.SpellStatistics.ManaUsed = SWP.ManaCost;
            }

            if (VE != null)
            {
                VE.SpellStatistics.HitCount = FightLength;
                VE.SpellStatistics.ManaUsed = FightLength * VE.ManaCost;
            }

            if (DP != null && DP.ImprovedDP != null)
                SpellPriority.Add(DP.ImprovedDP);

            #region Pass 1: Create the cast sequence
            bool CleanBreak = false;
            float timeSequenceReset = 0f;
            //Debug.Write(string.Format("\r\n\r\n----\r\nHaste: {0}, HasteRating: {1}", PlayerStats.SpellHaste.ToString("0.00"), PlayerStats.HasteRating));
            while (timer < (60 * FightLength))
            {
                if (hasteProcTimer > 0 && hasteProcTimer < timer)
                {
                    hasteProcTimer = 0f;
                    RecalculateHaste(simStats, 0f);
                }
                Spell spell = null;
                float castWait = OldGetCastSpell(timer, out spell, SWD);
                //if (castWait > 0)
                //    Debug.Write(string.Format("\r\n{0} : Wait {1}", timer.ToString("0.00"), castWait.ToString("0.00")));
                timer += castWait;
                //Debug.Write(string.Format("\r\n{0} : {1}", timer.ToString("0.00"), spell.Name));
                if (spell == null)
                {
                    timer += 0.1f;
                    continue;
                }

                CastList.Add(spell);
                timer += CalcOpts.Delay / 1000f;
                spell.SpellStatistics.HitCount++;


                if (bPnS && spell == MF)
                    SWP.SpellStatistics.CooldownReset = timer + SWP.DebuffDuration;
                else if (spell.DebuffDuration > 0f || spell.Cooldown > 0f)
                    spell.SpellStatistics.CooldownReset = timer + (spell.DebuffDuration > spell.Cooldown ? spell.DebuffDuration : spell.Cooldown) + spell.CastTime;

                timer += (spell.CastTime > 0) ? spell.CastTime : spell.GlobalCooldown;

                if (simStats.MindBlastHasteProc > 0 && spell == MB)
                {
                    RecalculateHaste(simStats, simStats.MindBlastHasteProc);
                    hasteProcTimer = timer + 4f;
                }

                if (spell == SpellPriority[sequence])
                    sequence++;
                else
                {
                    sequence = 0;
                    timeSequenceReset = timer;
                }
                if (SpellPriority[sequence] == MF)
                {   // Spell sequence just reset, lets take advantage of that.
                    int i = SpellPriority.IndexOf(MF);
                    for (int x = CastList.Count - i; x < CastList.Count; x++)
                        CastList[x].SpellStatistics.HitCount--;
                    CastList.RemoveRange(CastList.Count - i, i);
                    CleanBreak = true;
                    timer = timeSequenceReset;
                    break;
                }
            }
            if (!CleanBreak)
            {   // Cut down on excess DoT damage.
                foreach (Spell spell in SpellPriority)
                {
                    if (spell == MF) break;
                    if (spell.DebuffDuration > 0)
                        spell.SpellStatistics.HitCount -= (spell.SpellStatistics.CooldownReset - timer) / spell.DebuffDuration;
                    else if (spell.Cooldown > 0)
                        spell.SpellStatistics.HitCount -= (spell.SpellStatistics.CooldownReset - timer) / spell.Cooldown;
                }
            }
            SpellSimulation = CastList;
            #endregion

            #region Pass 2: Calculate Statistics for Procs
            //timer = 0;
            float CastsPerSecond = 0, HitsPerSecond = 0, CritsPerSecond = 0;
            foreach (Spell spell in CastList)
            {
                //timer += (spell.CastTime > 0) ? spell.CastTime : spell.GlobalCooldown;
                //timer += CalculationOptions.Delay / 1000f;
                CastsPerSecond++;
                HitsPerSecond += ShadowHitChance / 100f;
                if (spell == SWD)
                    CritsPerSecond += SWD.CritChance;
                else if (spell == MB)
                    CritsPerSecond += MB.CritChance;
                else if (spell == MF)
                    CritsPerSecond += MF.CritChance * 3f * 0.5f;    // Only 50% chance to proc 
                //if (spell == MF)
                //    HitsPerSecond += 2;   // MF can hit 3 times / cast
            }
            CastsPerSecond /= timer;
            CritsPerSecond /= timer;
            HitsPerSecond /= timer;

            // Deal with Spirit Tap
            if (seSpiritTap != null)
            {
                float uptime = seSpiritTap.GetAverageUptime(1f / CritsPerSecond, 1f);
                float NewSpirit = simStats.Spirit * seSpiritTap.Stats.BonusSpiritMultiplier * uptime; 
                float NewSPP = NewSpirit * SpellDamageFromSpiritPercentage;
                simStats.Spirit += NewSpirit;
                simStats.SpellPower += NewSPP;
                if (bVerbal)
                    Rotation += string.Format("\r\nImp. Spirit Tap Uptime: {0}%", (uptime * 100f).ToString("0.0"));
            }
            if (seGlyphofShadow != null)
            {
                float uptime = seGlyphofShadow.GetAverageUptime(1f / CritsPerSecond, 1f);
                simStats.SpellPower += simStats.Spirit * 0.3f * uptime;
                if (bVerbal)
                    Rotation += string.Format("\r\nGlyph of Shadow Uptime: {0}%", (uptime * 100f).ToString("0.0"));
            }

            // Deal with Twinkets
            foreach (SpecialEffect se in simStats.SpecialEffects())
            {
                float SpellPower = se.Stats.SpellPower;
                if (SpellPower > 0)
                {
                    if (se.Trigger == Trigger.Use)
                        simStats.SpellPower += se.GetAverageFactor(se.Cooldown, 1f) * SpellPower;
                    else if (se.Trigger == Trigger.SpellCast
                        || se.Trigger == Trigger.DamageSpellCast)
                        simStats.SpellPower += se.GetAverageFactor(1f / CastsPerSecond, 1f) * SpellPower;
                    else if (se.Trigger == Trigger.SpellHit
                        || se.Trigger == Trigger.DamageSpellHit)
                        simStats.SpellPower += se.GetAverageFactor(1f / CastsPerSecond, ShadowHitChance / 100f) * SpellPower;
                    else if (se.Trigger == Trigger.SpellCrit
                        || se.Trigger == Trigger.DamageSpellCrit)
                        simStats.SpellPower += se.GetAverageFactor(1f / CastsPerSecond, simStats.SpellCrit) * SpellPower;
                    else if (se.Trigger == Trigger.DoTTick)
                    {
                        int dots = 0;
                        foreach (Spell spell in SpellPriority)
                            if ((spell.DebuffDuration > 0) && (spell.DpS > 0)) dots++;
                        simStats.SpellPower += se.GetAverageFactor((float)dots / 3, 1f) * SpellPower;
                    }
                }
            }
            #endregion

            #region Pass3 + Pass 4: Redo Spell Stats and Calculate Damage and Mana Usage

            foreach (Spell spell in SpellPriority)
            {
                spell.Calculate(simStats, character);   // Redo stats for spell (with new spell power as result of procs)
                float Damage = spell.AvgDamage;
                float Cost = spell.ManaCost;
                switch (spell.Name)
                {
                    case "Vampiric Touch":
                        // Reapplyable DoTs, a resist means you lose 1 GCD to reapply. (~= cost of 1 GCD worth of MF)
                        Damage -= MF.DpS * MF.GlobalCooldown * (1f - ShadowHitChance / 100f);
                        // Also invokes a mana penalty by needing to cast it again.
                        Cost *= (2f - ShadowHitChance / 100f);
                        break;
                    case "Mind Flay":
                        // Reapplyable DoTs, a resist means you lose 1 GCD to reapply. (~= cost of 1 GCD worth of MF)
                        Damage -= MF.DpS * MF.GlobalCooldown * (1f - ShadowHitChance / 100f);
                        // Also invokes a mana penalty by needing to cast it again.
                        Cost *= (2f - ShadowHitChance / 100f);
                        break;
                    case "Shadow Word: Pain":
                        // SW:Pain follows same rules as other reapplyable dots, but needs special handling with
                        // Pain and Suffering.
                        if (bPnS)
                            break;
                        Damage -= MF.DpS * MF.GlobalCooldown * (1f - ShadowHitChance / 100f);
                        Cost *= (2f - ShadowHitChance / 100f);
                        break;
                    case "Mind Blast":
                    case "Shadow Word: Death":
                        // Spells that have cooldowns, a resist means full loss of damage.
                        Damage *= ShadowHitChance / 100f;
                        break;
                    case "Devouring Plague":
                        DevouringPlague dp = spell as DevouringPlague;
                        //Damage *= (1f + simStats.BonusDiseaseDamageMultiplier);   // No longer workie.
                        if (dp.ImprovedDP != null)
                        {
                            float lDamage = dp.ImprovedDP.AvgDamage
                                * (1f + simStats.BonusShadowDamageMultiplier)
                                * (1f + simStats.BonusDamageMultiplier)
                                * (HitChance / 100f);       // Imp. DP might not receive shadow hit bonuses.
                            dp.ImprovedDP.SpellStatistics.DamageDone += lDamage;
                            OverallDamage += lDamage;
                        }
                        Damage *= ShadowHitChance / 100f;
                        break;
                    default:
                        break;
                }
                Damage *= spell.SpellStatistics.HitCount;
                Cost *= spell.SpellStatistics.HitCount;
                Damage *= (1f + simStats.BonusShadowDamageMultiplier) * (1f + simStats.BonusDamageMultiplier);
                spell.SpellStatistics.DamageDone += Damage;
                spell.SpellStatistics.ManaUsed += Cost;
                OverallMana += Cost;
            }
            #endregion



            if (bVerbal)
            {
                if (!CleanBreak)
                    Rotation += "\r\nWARNING: Did not find a clean rotation!\r\nThis may make Haste inaccurate!";
                Rotation += string.Format("\r\nRotation reset after {0} seconds.", timer.ToString("0.00"));
            }

            #region Pass 5: Do spell statistics & handle movement.
            foreach (Spell spell in SpellPriority)
            {
                spell.SpellStatistics.HitCount /= timer;
                spell.SpellStatistics.CritCount = spell.SpellStatistics.HitCount * spell.CritChance;
                spell.SpellStatistics.MissCount = spell.SpellStatistics.HitCount * (1 - ShadowHitChance / 100f);
                spell.SpellStatistics.ManaUsed /= timer;
                if (bPnS && spell == SWP)
                    spell.SpellStatistics.DamageDone = spell.DpS * (1f + simStats.BonusShadowDamageMultiplier) * (1f + simStats.BonusDamageMultiplier);
                else
                {
                    if (spell.CastTime > 0)
                    {
                        float realMoveDuration = CalcOpts.MoveDuration / (1f + simStats.MovementSpeed);
                        float spellWait = (spell.Cooldown + spell.DebuffDuration) * 0.5f;
                        if (realMoveDuration > spellWait)
                        {   // Apply movement penalty
                            float DPSLossTime = spell.CastTime + realMoveDuration - spellWait;
                            float DPSLoss = DPSLossTime / CalcOpts.MoveFrequency;
                            MovementDamageLoss += spell.SpellStatistics.DamageDone * DPSLoss;
                            spell.SpellStatistics.DamageDone *= (1f - DPSLoss);
                        }
                    }
                    OverallDamage += spell.SpellStatistics.DamageDone;
                    spell.SpellStatistics.DamageDone /= timer;
                }
            }
            MovementDamageLoss /= timer;
            if (bVerbal)
                Rotation += "\r\nDPS lost to movement: " + MovementDamageLoss.ToString("0.00");
            #endregion


            DPS = OverallDamage / timer + (bPnS ? SWP.DpS * (1f + simStats.BonusShadowDamageMultiplier) * (1f + simStats.BonusDamageMultiplier) : 0);


            // Finalize Trinkets
           foreach (SpecialEffect se in simStats.SpecialEffects())
           {
               float DD = _GetDirectDamage(se.Stats);
               if (DD > 0)
               {
                   float dpr = 0f;
                   if (se.Trigger == Trigger.Use)
                   {
                       float Factor = se.GetAverageFactor(se.Cooldown, 1f);
                       dpr += DD * Factor;
                   }
                   else if (se.Trigger == Trigger.DoTTick)
                   {
                       int dots = 0;
                       foreach (Spell spell in SpellPriority)
                           if ((spell.DebuffDuration > 0) && (spell.DpS > 0)) dots++;
                       float Factor = se.GetAverageFactor(dots / 3f, 1f);
                       dpr += DD * Factor; //se.GetAverageProcsPerSecond(dots / 3, 1f, 0f, CalculationOptions.FightLength * 60f);
                   }
                   else if (se.Trigger == Trigger.DamageSpellHit
                       || se.Trigger == Trigger.SpellHit)
                   {
                       float Factor = se.GetAverageFactor(1f / CastsPerSecond, 1f);
                       dpr += DD * Factor; //se.GetAverageProcsPerSecond(1f / CastsPerSecond, 1f, 0f, CalculationOptions.FightLength * 60f);
                   }
                   else if (se.Trigger == Trigger.DamageSpellCrit
                       || se.Trigger == Trigger.SpellCrit)
                   {
                       float Factor = se.GetAverageFactor(1f / CastsPerSecond, simStats.SpellCrit);
                       dpr += DD * Factor; //* se.GetAverageProcsPerSecond(1f / CastsPerSecond, simStats.SpellCrit, 0f, CalculationOptions.FightLength * 60f);
                   }
                   if (dpr > 0)
                   {
                       dpr *= (1f + simStats.SpellCrit * 0.5f);
                       if (se.Stats.ShadowDamage > 0)
                       {
                           dpr *= (character.PriestTalents.ShadowWeaving > 0 ? 1.1f : 1.0f)
                               * (1f + character.PriestTalents.Darkness * 0.02f)
                               * (1f + character.PriestTalents.Shadowform * 0.15f)
                               * (ShadowHitChance / 100f);
                       }
                       else if (se.Stats.ValkyrDamage > 0) { }
                       else
                           dpr *= (HitChance / 100f);
                       // Should technically be one BonusXDamageMultiplier pr effect but REALLY REALLY REALLY!
                       DPS += dpr * (1f + simStats.BonusDamageMultiplier) * (1f + simStats.BonusShadowDamageMultiplier);
                   }
               }
            }

            if (bVerbal)
                Rotation += "\r\n\r\nMana Buffs:";

            SustainDPS = DPS;
            float MPS = OverallMana / timer;
            float SpiritRegen = (float)Math.Floor(StatConversion.GetSpiritRegenSec(simStats.Spirit, simStats.Intellect));
            float regen = 0, tmpregen = 0;
            tmpregen = SpiritRegen * simStats.SpellCombatManaRegeneration * (CalcOpts.FSRRatio / 100f);
            if (tmpregen > 0f)
            {
                ManaSources.Add(new ManaSource("Meditation", tmpregen));
                regen += tmpregen;
            }
            tmpregen = SpiritRegen * (1f - CalcOpts.FSRRatio / 100f);
            if (tmpregen > 0f)
            {
                ManaSources.Add(new ManaSource("OutFSR", tmpregen));
                regen += tmpregen;
            }
            tmpregen = simStats.Mp5 / 5;
            ManaSources.Add(new ManaSource("MP5", tmpregen));
            regen += tmpregen;
            if (seSpiritTap != null)
            {
                tmpregen = SpiritRegen * seSpiritTap.GetAverageUptime(1f / CritsPerSecond, 1f) * seSpiritTap.Stats.SpellCombatManaRegeneration;
                if (tmpregen > 0f)
                {
                    ManaSources.Add(new ManaSource("Imp. Spirit Tap", tmpregen));
                    regen += tmpregen;
                }
            }
            tmpregen = simStats.Mana / (FightLength * 60f);
            ManaSources.Add(new ManaSource("Intellect", tmpregen));
            regen += tmpregen;
            tmpregen = simStats.Mana * 0.002f * (CalcOpts.Replenishment / 100f);
            ManaSources.Add(new ManaSource("Replenishment", tmpregen));
            regen += tmpregen;
            tmpregen = BaseStats.GetBaseStats(character).Mana * (simStats.ManaRestoreFromBaseManaPPM > 0 ? 0.02f * 0.25f : 0f) * HitsPerSecond * (CalcOpts.JoW / 100f);
            if (tmpregen > 0)
            {
                ManaSources.Add(new ManaSource("Judgement of Wisdom", tmpregen));
                regen += tmpregen;
            }
            if (character.PriestTalents.GlyphofShadowWordPain)
            {
                tmpregen = BaseStats.GetBaseStats(character).Mana * 0.01f / (SWP.DebuffDuration / SWP.DebuffTicks);
                if (tmpregen > 0)
                {
                    ManaSources.Add(new ManaSource("Glyph of SWP", tmpregen));
                    regen += tmpregen;
                }
            }
            foreach (SpecialEffect se in simStats.SpecialEffects())
            {
                float ManaRestore = se.Stats.ManaRestore;
                float Mp5 = se.Stats.Mp5;
                if (ManaRestore > 0 || Mp5 > 0)
                {
                    if (se.Trigger == Trigger.Use)
                        tmpregen = se.GetAverageUptime(se.Cooldown, 1f)
                            * (ManaRestore + Mp5 / 5);
                    else if (se.Trigger == Trigger.SpellCast
                        || se.Trigger == Trigger.DamageSpellCast)
                        tmpregen = se.GetAverageUptime(1f / CastsPerSecond, 1f)
                            * (ManaRestore + Mp5 / 5);
                    else if (se.Trigger == Trigger.SpellHit
                        || se.Trigger == Trigger.DamageSpellHit)
                        tmpregen = se.GetAverageUptime(1f / CastsPerSecond, ShadowHitChance / 100f)
                            * (ManaRestore + Mp5 / 5);
                    else if (se.Trigger == Trigger.SpellCrit
                        || se.Trigger == Trigger.DamageSpellCrit)
                        tmpregen = se.GetAverageUptime(1f / CastsPerSecond, simStats.SpellCrit)
                            * (ManaRestore + Mp5 / 5);
                    else
                        tmpregen = 0;
                    if (tmpregen > 0)
                    {
                        ManaSources.Add(new ManaSource(se.ToString(), tmpregen));
                        regen += tmpregen;
                    }
                }
            }

            if (MPS > regen && character.Race == CharacterRace.BloodElf)
            {   // Arcane Torrent is 6% max mana every 2 minutes.
                tmpregen = simStats.Mana * 0.06f / 120f;
                ManaSources.Add(new ManaSource("Arcane Torrent", tmpregen));
                regen += tmpregen;
                if (bVerbal)
                    Rotation += "\r\n- Used Arcane Torrent";
            }

            if (MPS > regen && CalcOpts.ManaAmt > 0)
            {   // Not enough mana, use Mana Potion
                tmpregen = CalcOpts.ManaAmt / (FightLength * 60f) * (1f + simStats.BonusManaPotion);
                ManaSources.Add(new ManaSource("Mana Potion", tmpregen));
                regen += tmpregen;
                if (bVerbal)
                    Rotation += "\r\n- Used Mana Potion";
            }

            if (MPS > regen)
            {   // Not enough mana, use Shadowfiend
                float sf_rat = (CalcOpts.Shadowfiend / 100f) / ((5f - character.PriestTalents.VeiledShadows * 1f) * 60f);
                tmpregen = simStats.Mana * 0.5f * sf_rat;
                ManaSources.Add(new ManaSource("Shadowfiend", tmpregen));
                regen += tmpregen;
                //SustainDPS -= MF.DpS * sf_rat; You will actually gain dps from using it, so no reason to do this anymore.
                if (bVerbal)
                    Rotation += "\r\n- Used Shadowfiend";
            }

            if (MPS > regen && character.PriestTalents.Dispersion > 0)
            {   // Not enough mana, so eat a Dispersion, remove DPS worth of 6 seconds of mindflay.
                float disp_rat = 6f / (60f * 3f - (character.PriestTalents.GlyphofDispersion ? 45 : 0));
                tmpregen = simStats.Mana * 0.06f * disp_rat;
                ManaSources.Add(new ManaSource("Dispersion", tmpregen));
                regen += tmpregen;
                SustainDPS -= MF.DpS * disp_rat;
                if (bVerbal)
                    Rotation += "\r\n- Used Dispersion";
            }

#if RAWR3 || SILVERLIGHT
            DPS *= (1f - StatConversion.GetAverageResistance(character.Level, BossOpts.Level, 0, 0)); // Level based Partial resists.
#else
            DPS *= (1f - StatConversion.GetAverageResistance(character.Level, character.Level + CalcOpts.TargetLevel, 0, 0)); // Level based Partial resists.
#endif
            //SustainDPS *= (1f - CalculationOptions.TargetLevel * 0.02f);

            SustainDPS = (MPS < regen) ? SustainDPS : (SustainDPS * regen / MPS);
        }
    }

    public class SolverShadow : SolverBase
    {   // A wrapper for solverShadow to handle haste properly.
        public class SolverInfo
        {
            public solverShadow Solver { get; protected set; }
            private float Duration;
            private float Interval;

            public SolverInfo(solverShadow solver, float duration, float interval)
            {
                Solver = solver;
                Duration = duration;
                Interval = interval;
            }

            public float Uptime(float totalTime)
            {
                if (Interval > 0f)
                {
                    float procs = (float)Math.Floor(totalTime / Interval) + 1f;
                    return Duration * procs / totalTime;
                }
                return 1.0f;
            }
        }
        List<SolverInfo> SSInfo = new List<SolverInfo>();

        public SolverShadow(Stats BasicStats, Character character)
            : base(BasicStats, character)
        {
            SolverInfo ssi;
            ssi = new SolverInfo(new solverShadow(BasicStats, character), 0f, 0f);
            SSInfo.Add(ssi);
            Name = ssi.Solver.Name;

            // Create an additional solver for each additional haste effect.
            foreach (SpecialEffect se in BasicStats.SpecialEffects())
            {
                bool bRelevant = se.Stats.HasteRating > 0 || se.Stats.SpellHaste > 0;
                foreach (SpecialEffect s in se.Stats.SpecialEffects())
                    bRelevant |= (s.Stats.HasteRating > 0 || s.Stats.SpellHaste > 0);
                if (bRelevant)
                {
                    if (float.IsInfinity(se.Duration))
                        continue;
                    if (se.Trigger == Trigger.Use)
                    {   // Heroism, Glove Enchants, Troll Racial + other Use trinkets.
                        Stats statsHasteUse = BasicStats.Clone();
                        foreach (SpecialEffect s in se.Stats.SpecialEffects())
                            if (s.Stats.HasteRating > 0)
                            {
                                float Factor = s.GetAverageFactor(1.5f / (1f + statsHasteUse.SpellHaste), 1f, 0f, se.Duration);
                                statsHasteUse.HasteRating += s.Stats.HasteRating * Factor;
                            }
                        statsHasteUse.HasteRating += se.Stats.HasteRating;
                        statsHasteUse.SpellHaste = (1f + statsHasteUse.SpellHaste)
                            / (1f + StatConversion.GetSpellHasteFromRating(BasicStats.HasteRating))
                            * (1f + StatConversion.GetSpellHasteFromRating(statsHasteUse.HasteRating))
                            * (1f + se.Stats.SpellHaste)
                            - 1f;
                        ssi = new SolverInfo(new solverShadow(statsHasteUse, character), se.Duration, se.Cooldown);
                        SSInfo.Add(ssi);
                    }
                    else if (se.Trigger == Trigger.SpellCast ||
                        se.Trigger == Trigger.SpellHit ||
                        se.Trigger == Trigger.DamageSpellCast ||
                        se.Trigger == Trigger.DamageSpellHit)
                    {   // Procs.
                        Stats statsHasteProc = BasicStats.Clone();
                        statsHasteProc.HasteRating += se.Stats.HasteRating;
                        statsHasteProc.SpellHaste = (1f + statsHasteProc.SpellHaste)
                            / (1f + StatConversion.GetSpellHasteFromRating(BasicStats.HasteRating))
                            * (1f + StatConversion.GetSpellHasteFromRating(statsHasteProc.HasteRating))                            
                            * (1f + se.Stats.SpellHaste)
                            - 1f;
                        float cooldown = se.Cooldown;
                        if (cooldown >= 44f)
                            cooldown += 10f;
                        else
                            cooldown += 15f;
                        ssi = new SolverInfo(new solverShadow(statsHasteProc, character), se.Duration, cooldown);
                        SSInfo.Add(ssi);
                    }
                }
            }
        }

        public override void Calculate(CharacterCalculationsShadowPriest calculatedStats)
        {
#if RAWR3 || SILVERLIGHT
            float fightLen = (int)(SSInfo[0].Solver.BossOpts.BerserkTimer / 60f) * 60f;
#else
            float fightLen = SSInfo[0].Solver.CalcOpts.FightLength * 60f;
#endif
            foreach (SolverInfo si in SSInfo)
                si.Solver.Calculate(calculatedStats);

            float normalUptime = 1.0f;
            for (int x = 1; x < SSInfo.Count; x++)
            {
                SolverInfo si = SSInfo[x];
                float uptime = si.Uptime(fightLen);
                normalUptime -= uptime;
                if (normalUptime < 0)
                {
                    uptime += normalUptime;
                    normalUptime = 0;
                }
                DPS += si.Solver.DPS * uptime;
                SustainDPS += si.Solver.SustainDPS * uptime;
            }
            DPS += SSInfo[0].Solver.DPS * normalUptime;
            SustainDPS += SSInfo[0].Solver.SustainDPS * normalUptime;

            SpellSimulation = SSInfo[0].Solver.SpellSimulation;
//            Debug.Write(string.Format("\r\n\r\nDPS: {0}, Casts: {1}", DPS, SpellSimulation.Count));
//            foreach (Spell spell in SpellSimulation)
//                Debug.Write(string.Format("\r\n- {0}", spell.Name));
//            Debug.Write("\r\n-------------------");

            SpellPriority = SSInfo[0].Solver.SpellPriority;
            Rotation = SSInfo[0].Solver.Rotation;
            ManaSources = SSInfo[0].Solver.ManaSources;

            calculatedStats.DpsPoints = SustainDPS;
//            calculatedStats.SustainPoints = SustainDPS;

            // Lets just say that 15% of resilience scales all health by 150%.
            float Resilience = (float)Math.Min(15f, StatConversion.GetCritReductionFromResilience(PlayerStats.Resilience) * 100f) / 15f;
            calculatedStats.SurvivalPoints = calculatedStats.BasicStats.Health * (Resilience * 1.5f + 1f) * CalcOpts.Survivability / 100f;
        }
    }
   
    public class SolverHoly : SolverBase
    {   // Models Full Rotation
        protected Spell SWP { get; set; }
        protected Spell SWD { get; set; }
        protected Spell MB { get; set; }
        protected Spell DP { get; set; }
        protected Spell SM { get; set; }
        protected Spell PE { get; set; }

        public SolverHoly(Stats BasicStats, Character character)
            : base(BasicStats, character)
        {

            SpellPriority = new List<Spell>(Spell.HolySpellList.Count);
            SpellComparerDpCT _scDpCT = new SpellComparerDpCT();
            SpellComparerDpM _scDpM = new SpellComparerDpM();
            float baseMana = BaseStats.GetBaseStats(character).Mana;
            foreach (string spellname in Spell.HolySpellList)
            {
                Spell spelltmp = SpellFactory.CreateSpell(spellname, PlayerStats, character, baseMana, CalcOpts.PTR);
                if (spelltmp != null) SpellPriority.Add(spelltmp);
            }

            SWP = GetSpellByName("Shadow Word: Pain");
            SWD = GetSpellByName("Shadow Word: Death");
            MB = GetSpellByName("Mind Blast");
            DP = GetSpellByName("Devouring Plague");
            SM = GetSpellByName("Smite");
            PE = GetSpellByName("Penance");

            // Sort spells and remove any spell with worse mana efficiency than smite.
            SpellPriority.Sort(_scDpM);
            int i = SpellPriority.IndexOf(SM) + 1;
            SpellPriority.RemoveRange(i, SpellPriority.Count - i);

            // Sort spells, and remove any spell with worse damage pr cast time than smite.
            SpellPriority.Sort(_scDpCT);
            i = SpellPriority.IndexOf(SM) + 1; 
            SpellPriority.RemoveRange(i, SpellPriority.Count - i);

            if (character.PriestTalents.Penance > 0)
                Name = "Holy Smite w/Penance";
            else
                Name = "Holy Smite";
            Rotation = "Priority Based:";
            foreach (Spell spell in SpellPriority)
                Rotation += "\r\n- " + spell.Name;
            Rotation += "\r\n\r\nMana Buffs:";
        }

        public override void Calculate(CharacterCalculationsShadowPriest calculatedStats)
        {
            if (SpellPriority.Count == 0)
                return;

            Stats simStats = PlayerStats.Clone();
            float timer = 0;
            int sequence = SpellPriority.Count - 1;
            List<Spell> CastList = new List<Spell>();
#if RAWR3 || SILVERLIGHT
            int FightLength = (int)(BossOpts.BerserkTimer / 60f) * 60;
#else
            int FightLength = (int)CalcOpts.FightLength;
#endif

            while (timer < (FightLength * 60f))
            {
                Spell spell = GetCastSpell(timer);
                if (spell == null)
                {
                    timer += 0.1f;
                    continue;
                }

                CastList.Add(spell);

                if (spell.DebuffDuration > 0f || spell.Cooldown > 0f)
                    spell.SpellStatistics.CooldownReset = timer + (spell.DebuffDuration > spell.Cooldown ? spell.DebuffDuration : spell.Cooldown);

                timer += (spell.CastTime > 0) ? spell.CastTime : spell.GlobalCooldown;

                if (spell == SpellPriority[sequence])
                    sequence++;
                else
                    sequence = 0;
                if (SpellPriority[sequence] == SM)
                {   // Spell sequence just reset, lets take advantage of that.
                    int i = SpellPriority.IndexOf(SM);
                    CastList.RemoveRange(CastList.Count - i, i);
                    break;
                }
            }

            float ShadowHitChance = (float)Math.Min(HitChance + character.PriestTalents.ShadowFocus * 0.01f, 1f);
            timer = 0;
            foreach (Spell spell in CastList)
            {
                timer += (spell.CastTime > 0) ? spell.CastTime : spell.GlobalCooldown;
                float Damage = spell.AvgDamage;
                float Cost = spell.ManaCost;
                if (spell.Name == "Smite")
                {
                    Damage -= SM.DpS * SM.GlobalCooldown * (1f - HitChance / 100f);
                    Cost *= (2f - HitChance / 100f);
                    if (character.PriestTalents.GlyphofSmite)
                        Damage *= (1f + 0.2f * 7 / 10);
                }
                else if (spell.Name == "Shadow Word: Pain")
                {
                    Damage -= SM.DpS * SM.GlobalCooldown * (1f - ShadowHitChance / 100f);
                    Cost *= (2f - ShadowHitChance / 100f);
                }
                else if (spell.Name == "Holy Fire")
                {
                    Damage *= HitChance / 100f;
                }
                else if (spell.Name == "Mind Blast")
                {
                    Damage *= ShadowHitChance / 100f;
                }
                else if (spell.Name == "Shadow Word: Death")
                {
                    Damage *= ShadowHitChance / 100f;
                }
                else if (spell.Name == "Penance")
                {
                    Damage *= HitChance / 100f;
                }
                else if (spell.Name == "Devouring Plague")
                {
                    Damage *= ShadowHitChance / 100f;
                    Damage *= (1f + simStats.BonusDiseaseDamageMultiplier);
                }

                Damage *= (1f + simStats.BonusShadowDamageMultiplier) * (1f + simStats.BonusDamageMultiplier); // If there is shadow bonus damage there is holy bonus damage.
                spell.SpellStatistics.DamageDone += Damage;
                spell.SpellStatistics.ManaUsed += Cost;
                OverallDamage += Damage;
                OverallMana += Cost;
            }

            DPS = OverallDamage / timer;

            SustainDPS = DPS;
            float MPS = OverallMana / timer;
            float TimeInFSR = 1f;
            float regen = (calculatedStats.RegenInFSR * TimeInFSR + calculatedStats.RegenOutFSR * (1f - TimeInFSR)) / 5;
            regen += simStats.Mana / (FightLength * 60f);
            regen += simStats.Mana * 0.002f * (CalcOpts.Replenishment / 100f);

            if (MPS > regen && CalcOpts.ManaAmt > 0)
            {   // Not enough mana, use Mana Potion
                regen += CalcOpts.ManaAmt / (FightLength * 60f) * (1f + simStats.BonusManaPotion);
                Rotation += "\r\n- Used Mana Potion";
            }

            if (MPS > regen)
            {   // Not enough mana, use Shadowfiend
                float sf_rat = (CalcOpts.Shadowfiend / 100f) / ((5f - character.PriestTalents.VeiledShadows * 1f) * 60f);
                regen += simStats.Mana * 0.4f * sf_rat;
                SustainDPS -= SM.DpS * sf_rat;
                Rotation += "\r\n- Used Shadowfiend";
            }

            if (MPS > regen)
            {   // Not enough still, use Hymn of Hope
                float hh_rat = 8f / (60f * 5f);
                regen += simStats.Mana * 0.02f * hh_rat;
                SustainDPS -= SM.DpS * hh_rat;
                Rotation += "\r\n- Used Hymn of Hope";
            }

#if RAWR3 || SILVERLIGHT
            DPS *= (1f - (BossOpts.Level - character.Level) * 0.02f); // Level based Partial resists.
            SustainDPS *= (1f - (BossOpts.Level - character.Level) * 0.02f);
#else
            DPS *= (1f - CalcOpts.TargetLevel * 0.02f); // Level based Partial resists.
            SustainDPS *= (1f - CalcOpts.TargetLevel * 0.02f);
#endif

            SustainDPS = (MPS < regen) ? SustainDPS : (SustainDPS * regen / MPS);

            calculatedStats.DpsPoints = SustainDPS;
//            calculatedStats.SustainPoints = SustainDPS;
            // If opponent has 25% crit, each 39.42308044 resilience gives -1% damage from dots and -1% chance to be crit. Also reduces crits by 2%.
            // This effectively means you gain 12.5% extra health from removing 12.5% dot and 12.5% crits at resilience cap (492.5 (39.42308044*12.5))
            // In addition, the remaining 12.5% crits are reduced by 25% (12.5%*200%damage*75% = 18.75%)
            // At resilience cap I'd say that your hp's are scaled by 1.125*1.1875 = ~30%. Probably wrong but good enough.
            calculatedStats.SurvivalPoints = calculatedStats.BasicStats.Health * CalcOpts.Survivability / 100f * (1 + 0.3f * calculatedStats.BasicStats.Resilience / 492.7885f);
        }
    }


// Decrepated

    public class Solver : SolverBase
    {
        protected float ShadowHitChance { get; set; }
        protected float ShadowCritChance { get; set; }

        public Solver(Stats BasicStats, Character character)
            : base(BasicStats, character)
        {

            ShadowHitChance = (float)Math.Min(100f, HitChance + character.PriestTalents.ShadowFocus * 1f);
        }

        public override void Calculate(CharacterCalculationsShadowPriest calculatedStats)
        {/*
            if(SpellPriority.Count == 0)
                return;

            Stats currentStats;
            Random rnd = new Random(DateTime.Now.Millisecond);
            int castCount = 0, critableCount = 0, ssCount = 0;
            float timer = 0.0f, drumsTimer = 0.0f, drumsUpTimer = 0.0f;
            
            while (timer < CalculationOptions.FightLength * 60f)
            {
                timer += CalculationOptions.Lag/1000.0f;
                timer += CalculationOptions.WaitTime/1000.0f;
                
                if(CalculationOptions.DrumsCount > 0 && drumsTimer < timer)
                {
                    drumsTimer = timer + 120.0f;
                    drumsUpTimer = timer + CalculationOptions.DrumsCount * 30.0f;
                    if(CalculationOptions.UseYourDrum)
                    {
                        timer += SpellPriority[0].GlobalCooldown;
                        continue;
                    }
                }

                UpTrinket(timer);
                
                currentStats = PlayerStats.Clone();
                if (CalculationOptions.DrumsCount > 0 && drumsUpTimer > timer)
                    currentStats.HasteRating += 80.0f;
                GetTrinketBuff(timer, currentStats);

                Spell spell = GetCastSpell(timer);
                if (spell == null)
                {
                    timer += 0.1f;
                    continue;
                }

                Spell seqspell = SpellFactory.CreateSpell(spell.Name, currentStats, character);
                if (spell.CritCoef > 1.0f)
                    critableCount++;
                if (spell.MagicSchool != MagicSchool.Shadow)
                    ssCount++;
                if (spell.DebuffDuration > 0.0f || spell.Cooldown > 0.0f)
                    spell.SpellStatistics.CooldownReset = timer + (spell.DebuffDuration > spell.Cooldown ? spell.DebuffDuration : spell.Cooldown);

                spell.SpellStatistics.HitCount++;
                spell.SpellStatistics.ManaUsed += seqspell.ManaCost;
                Sequence.Add(timer, seqspell);
                timer += seqspell.CastTime > 0.0f ? seqspell.CastTime : seqspell.GlobalCooldown;
                castCount++;
            }

            int missCount = (int)Math.Round((Sequence.Count - ssCount) * (100.0f - ShadowHitChance) / 100.0f);
            int mbmissCount = (int)Math.Round((double)missCount / 2.0f);
            int critCount = (int)Math.Round(critableCount * new MindBlast(PlayerStats, character).CritChance / 100.0f);
            int mbcritCount = (int)Math.Round((double)critCount / 2.0f);
            int ssmissCount = (int)Math.Round(ssCount * (100.0f - HitChance) / 100.0f);
            foreach (Spell spell in SpellPriority)
            {
                if(spell.CritCoef > 1.0f)
                {
                    if (spell.Name == "Mind Blast")
                    {
                        spell.SpellStatistics.HitCount -= mbmissCount;
                        spell.SpellStatistics.MissCount = mbmissCount;
                        spell.SpellStatistics.CritCount = mbcritCount;
                    }
                    else
                    {
                        spell.SpellStatistics.HitCount -= (missCount - mbmissCount);
                        spell.SpellStatistics.MissCount = (missCount - mbmissCount);
                        spell.SpellStatistics.CritCount = (critCount - mbcritCount);
                    }

                    spell.SpellStatistics.DamageDone += spell.SpellStatistics.CritCount * spell.AvgCrit;
                }

                if (spell.MagicSchool != MagicSchool.Shadow)
                {
                    spell.SpellStatistics.HitCount -= ssmissCount;
                    spell.SpellStatistics.MissCount = ssmissCount;
                }

                spell.SpellStatistics.DamageDone += (spell.SpellStatistics.HitCount - spell.SpellStatistics.CritCount) * spell.AvgDamage;

                OverallDamage += spell.SpellStatistics.DamageDone;
                OverallMana += spell.SpellStatistics.ManaUsed;
            }

            DPS = OverallDamage / (CalculationOptions.FightLength * 60f);
            SustainDPS = OverallMana / (CalculationOptions.FightLength * 60f); // Meeehh*/
        }
    }
}
