using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace Rawr.Warlock {

    public class Spell {

        public enum SpellTree { Affliction, Demonology, Destruction }

        public static List<string> ALL_SPELLS = new List<String>();

        static Spell() {

            Type spellType = Type.GetType("Rawr.Warlock.Spell");
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) {
                if (type.IsSubclassOf(spellType)) {
                    string name = type.Name;
                    for (int i = 1; i < name.Length; ++i) {
                        if (char.IsUpper(name[i])) {
                            name = name.Insert(i, " ");
                            ++i;
                        } else if (name[i] == '_') {
                            name = name.Replace("_", " (");
                            name = name.Insert(name.Length, ")");
                            i += 2;
                        }
                    }
                    ALL_SPELLS.Add(name);
                }
            }
        }

        public static float CalcUprate(
            float procRate, float duration, float triggerPeriod) {

            return 1f
                - (float) Math.Pow(1f - procRate, duration / triggerPeriod);
        }

        public static float GetTimeUsed(
            float baseCastTime,
            float gcdBonus,
            List<WeightedStat> haste,
            float lag) {

            float minGCD = 1f - gcdBonus;
            float unhasted = Math.Max(baseCastTime, 1.5f - gcdBonus);

            float avgHasted = 0f;
            foreach (WeightedStat h in haste) {
                avgHasted += h.Chance * Math.Max(minGCD, unhasted / h.Value);
            }
            return avgHasted + lag;
        }

        #region properties

        // set via constructor
        public CharacterCalculationsWarlock Mommy { get; protected set; }
        public MagicSchool MagicSchool { get; protected set; }
        public SpellTree MySpellTree { get; protected set; }
        public float ManaCost { get; protected set; }
        public float BaseCastTime { get; protected set; }
        public float GCDBonus { get; protected set; }
        public bool CanMiss { get; protected set; }
        public float BaseDamage { get; protected set; }
        public float DirectCoefficient { get; protected set; }
        public float BaseTickDamage { get; protected set; }
        public float TickCoefficient { get; protected set; }
        public bool CanTickCrit { get; protected set; }

        // set via constructor but only used for internal optimizations
        public bool RecordMissesSeparately = false;

        // set via constructor but sometimes modified via RecordCollisionDelay()
        public float Cooldown { get; protected set; }
        public float RecastPeriod { get; protected set; }
        public float NumTicks { get; protected set; }
        public SpellModifiers SpellModifiers { get; protected set; }

        // set via RecordCollisionDelay()
        public Dictionary<string, SimulatedStat> SimulatedStats {
            get;
            protected set;
        }

        // set via SetDamageStats()
        public float AvgDirectHit { get; protected set; }
        public float AvgDirectCrit { get; protected set; }
        public float AvgTickHit { get; protected set; }
        public float AvgTickCrit { get; protected set; }
        public float AvgDamagePerCast { get; protected set; }

        // cached values (can be determined from the values in other props)
        protected float NumCasts;
        protected float AvgDelay;

        #endregion


        #region Non-Damage Constructor
        public Spell(
            CharacterCalculationsWarlock mommy,
            MagicSchool magicSchool,
            SpellTree spellTree,
            float percentBaseMana,
            float baseCastTime,
            float cooldown,
            float recastPeriod,
            bool canMiss)
            : this(
                mommy,
                magicSchool,
                spellTree,
                percentBaseMana,
                baseCastTime,
                cooldown,
                recastPeriod,
                canMiss,
                0f, // lowDirectDamage,
                0f, // highDirectDamage,
                0f, // directCoefficient,
                0f, // addedDirectMultiplier,
                0f, // baseTickDamage,
                0f, // numTicks,
                0f, // tickCoefficient,
                0f, // addedTickMultiplier,
                false, // canTickCrit,
                0f, // bonusCritChance,
                0f) { } // bonusCritMultiplier,
        #endregion

        #region Direct Damage Constructor
        public Spell(
            CharacterCalculationsWarlock mommy,
            MagicSchool magicSchool,
            SpellTree spellTree,
            float percentBaseMana,
            float baseCastTime,
            float cooldown,
            float recastPeriod,
            float lowDirectDamage,
            float highDirectDamage,
            float directCoefficient,
            float addedDirectMultiplier,
            float bonusCritChance,
            float bonusCritMultiplier)
            : this(
                mommy,
                magicSchool,
                spellTree,
                percentBaseMana,
                baseCastTime,
                cooldown,
                recastPeriod,
                true,
                lowDirectDamage,
                highDirectDamage,
                directCoefficient,
                addedDirectMultiplier,
                0f, // baseTickDamage,
                0f, // numTicks,
                0f, // tickCoefficient,
                0f, // addedTickMultiplier,
                false, // canTickCrit,
                bonusCritChance,
                bonusCritMultiplier) { }
        #endregion

        #region DoT Constructor
        public Spell(
            CharacterCalculationsWarlock mommy,
            MagicSchool magicSchool,
            SpellTree spellTree,
            float percentBaseMana,
            float baseCastTime,
            float cooldown,
            float recastPeriod,
            float baseTickDamage,
            float numTicks,
            float tickCoefficient,
            float addedTickMultiplier,
            bool canTickCrit,
            float bonusCritChance,
            float bonusCritMultiplier)
            : this(
                mommy,
                magicSchool,
                spellTree,
                percentBaseMana,
                baseCastTime,
                cooldown,
                recastPeriod,
                true,
                0f, // direct low damage
                0f, // direct high damage
                0f, // direct coefficient
                0f, // addedDirectMultiplier
                baseTickDamage,
                numTicks,
                tickCoefficient,
                addedTickMultiplier,
                canTickCrit,
                bonusCritChance,
                bonusCritMultiplier) { }
        #endregion

        #region Kitchen Sink Constructor
        public Spell(
            CharacterCalculationsWarlock mommy,
            MagicSchool magicSchool,
            SpellTree spellTree,
            float percentBaseMana,
            float baseCastTime,
            float cooldown,
            float recastPeriod,
            bool canMiss,
            float lowDirectDamage,
            float highDirectDamage,
            float directCoefficient,
            float addedDirectMultiplier,
            float baseTickDamage,
            float numTicks,
            float tickCoefficient,
            float addedTickMultiplier,
            bool canTickCrit,
            float bonusCritChance,
            float bonusCritMultiplier) {

            Mommy = mommy;
            MagicSchool = magicSchool;
            MySpellTree = spellTree;
            ManaCost = mommy.BaseMana * percentBaseMana;
            BaseCastTime = baseCastTime;
            BaseDamage = (lowDirectDamage + highDirectDamage) / 2f;
            Cooldown = cooldown;
            RecastPeriod = recastPeriod;
            CanMiss = canMiss;
            DirectCoefficient = directCoefficient;
            BaseTickDamage = baseTickDamage;
            NumTicks = numTicks;
            TickCoefficient = tickCoefficient;
            CanTickCrit = canTickCrit;

            SimulatedStats = new Dictionary<string, SimulatedStat>();

            WarlockTalents talents = mommy.Talents;
            SpellModifiers = new SpellModifiers();
            SpellModifiers.AddAdditiveDirectMultiplier(
                addedDirectMultiplier);
            SpellModifiers.AddAdditiveTickMultiplier(
                addedTickMultiplier);
            SpellModifiers.AddCritChance(bonusCritChance);
            SpellModifiers.AddCritBonusMultiplier(bonusCritMultiplier);
            if (spellTree == SpellTree.Destruction) {
                float[] talentAffects = { 0f, .04f, .07f, .1f };
                ManaCost *= (1 - talentAffects[Mommy.Talents.Cataclysm]);
            } else if (spellTree == SpellTree.Affliction) {
                ManaCost *= (1 - Mommy.Talents.Suppression * .01f);
            }
        }
        #endregion


        protected Rotation GetRotation() {

            return Mommy.Options.GetActiveRotation();
        }

        protected void ApplyImprovedSoulLeech() {

            float reductionOnProc
                = Mommy.CalcMana() * Mommy.Talents.ImprovedSoulLeech * .01f;
            ManaCost -= .3f * reductionOnProc;
        }

        public virtual bool IsCastable() {

            return true;
        }

        public string GetToolTip() {

            float numCasts = GetNumCasts();
            float castTime = GetAvgTimeUsed() - Mommy.Options.Latency;

            string toolTip;
            if (AvgDamagePerCast > 0) {
                float dps
                    = numCasts * AvgDamagePerCast / Mommy.Options.Duration;
                toolTip
                    = string.Format(
                        "{0:0.0} dps ({1:0.0%})*",
                        dps,
                        dps / Mommy.PersonalDps);
            } else {
                toolTip = string.Format("{0:0.0} casts*", numCasts);
            }
            if (AvgDirectHit > 0) {
                toolTip
                    += string.Format(
                        "{0:0.0}\tAverage Hit\r\n"
                            + "{1:0.0}\tAverage Crit\r\n",
                        AvgDirectHit,
                        AvgDirectCrit);
            }
            if (AvgTickHit > 0) {
                toolTip
                    += string.Format(
                        "{0:0.0}\tAverage Tick\r\n"
                            + "{1:0.0}\tAverage Tick Crit\r\n"
                            + "{2:0.0}\tTicks Per Cast\r\n",
                        AvgTickHit,
                        AvgTickCrit,
                        NumTicks);
            }
            if (AvgDamagePerCast > 0) {
                toolTip += "\r\n";
            }
            if (AvgDirectHit > 0) {
                toolTip
                    += string.Format(
                        "{0:0.0}%\tDirect Coefficient\r\n",
                        100 * DirectCoefficient);
            }
            if (AvgTickHit > 0) {
                toolTip
                    += string.Format(
                        "{0:0.0}%\tPer Tick Coefficient\r\n",
                        100 * TickCoefficient);
            }
            if (AvgDirectHit > 0) {
                toolTip
                    += string.Format(
                        "{0:0.0}%\tDirect Multiplier\r\n",
                        100 * SpellModifiers.GetFinalDirectMultiplier());
            }
            if (AvgTickHit > 0) {
                toolTip
                    += string.Format(
                        "{0:0.0}%\tPer Tick Multiplier\r\n",
                        100 * SpellModifiers.GetFinalTickMultiplier());
            }
            if (AvgDirectCrit > AvgDirectHit || AvgTickCrit > AvgTickHit) {
                toolTip
                    += string.Format(
                        "{0:0.0}%\tCrit Chance\r\n"
                            + "{1:0}%\tCrit Multiplier\r\n",
                        100 * SpellModifiers.CritChance,
                        100 * SpellModifiers.GetFinalCritMultiplier());
            }
            if (AvgDamagePerCast > 0) {
                toolTip
                    += string.Format(
                        "{0:0.0}%\tResisted\r\n\r\n", 100 * GetResist());
            }
            toolTip
                += string.Format(
                    "{0:0.00}s\tCast Time\r\n"
                        + "{1:0.0}\tMana\r\n",
                    castTime,
                    ManaCost);
            if (Cooldown > 0) {
                toolTip += string.Format("{0:0.0}s\tCooldown\r\n", Cooldown);
            }
            toolTip
                += string.Format(
                    "{0:0.0}s\tBetween Casts (Average)\r\n",
                    GetAvgTimeBetweenCasts());
            if (AvgDamagePerCast > 0) {
                toolTip
                    += string.Format(
                        "\r\n"
                            + "{0:0.0}\tDPC\r\n"
                            + "{1:0.0}\tDPCT\r\n"
                            + "{2:0.0}\tDPM\r\n"
                            + "{3:0.0}\tCasts",
                        AvgDamagePerCast,
                        AvgDamagePerCast / castTime,
                        AvgDamagePerCast / ManaCost,
                        numCasts);
            }
            return toolTip;
        }


        #region setting NumCasts

        public void Spam(float timeRemaining) {

            Debug.Assert(Cooldown == 0 && RecastPeriod == 0);
            NumCasts += timeRemaining / GetAvgTimeUsed();
            if (CanGetPyroclasm()) {
                Spell conflagrate = Mommy.CastSpells["Conflagrate"];
                CastingState fillerState = new CastingState(Mommy, null, 1f);
                RecordSimulatedStat(
                    "Pyroclasm Uprate",
                    conflagrate.GetUprate(fillerState, this),
                    1f);
            }
        }

        public virtual float GetNumCasts() {

            if (NumCasts == 0) {
                float duration = Mommy.Options.Duration;
                if (!IsCastDuringExecute()) {
                    duration *= 1 - Mommy.GetExecutePercentage();
                }
                float delay = SimulatedStats["delay"].GetValue();
                NumCasts = duration / (GetAvgRequeueTime() + delay);
            }
            return NumCasts;
        }

        public virtual bool IsCastDuringExecute() {

            return true;
        }

        private bool IsBinary() {

            return false;
        }

        public virtual float GetQueueProbability(CastingState state) {

            if (state.Cooldowns.ContainsKey(this)) {
                if (state.Cooldowns[this] <= 0) {
                    return 1f;
                } else {
                    return 0f;
                }
            } else {
                float maxQueued = state.GetMaxTimeQueued(this);
                float unqueuable = state.Elapsed - maxQueued;
                return Math.Min(
                    maxQueued / (GetAvgRequeueTime() - unqueuable), 1f);
            }
        }

        public float GetAvgRequeueTime() {

            // TODO this should really not be averaged (for the cases this
            // method is used).  Instead it should return all possible cooldowns
            // and their probabilities (and calling methods should be modified
            // accordingly).

            float period = Math.Max(RecastPeriod, Cooldown);
            float hitChance = Mommy.HitChance;
            if (IsBinary()) {
                hitChance -= GetResist();
            }
            if (CanMiss && hitChance < 1 && Cooldown < period) {

                // If a spell misses, and it can be recast sooner than it
                // normally would otherwise, it will instead wait for either its
                // cooldown (if it has one), or for one spell to be cast
                // inbetween (to allow for travel time + reaction time).
                float missDelay
                    = Math.Max(Cooldown + GetCastTime(null), Mommy.AvgTimeUsed);
                period
                    = Utilities.GetWeightedSum(
                        period, hitChance, missDelay, 1 - hitChance);
            }
            return period;
        }

        public virtual List<CastingState> SimulateCast(
            CastingState stateBeforeCast, float chanceOfCast) {

            // record stats about this spellcast
            float p = chanceOfCast * stateBeforeCast.Probability;
            float timeUsed = GetTimeUsed(stateBeforeCast);
            RecordSimulatedStat(
                "delay", stateBeforeCast.GetMaxTimeQueued(this) / 2f, p);
            RecordSimulatedStat("time used", timeUsed, p);
            if (CanGetPyroclasm()) {
                Spell conflagrate = Mommy.CastSpells["Conflagrate"];
                RecordSimulatedStat(
                    "Pyroclasm Uprate",
                    conflagrate.GetUprate(stateBeforeCast, this),
                    p);
            }

            // construct the casting state(s) that can result from this cast
            List<CastingState> results = new List<CastingState>();
            float hitChance = Mommy.HitChance;
            if (IsBinary()) {
                hitChance -= GetResist();
            }
            float newCooldown
                = Cooldown - timeUsed + GetCastTime(stateBeforeCast);
            if (CanMiss
                && hitChance < 1f
                && (RecordMissesSeparately || Cooldown < RecastPeriod)) {

                // state when spell hits
                PopulateNextState(
                    results,
                    stateBeforeCast,
                    timeUsed,
                    Math.Max(RecastPeriod - timeUsed, newCooldown),
                    hitChance * chanceOfCast,
                    true);

                // state when spell misses
                if (newCooldown <= 0) {

                    // ensure at least 1 spell is cast before this one is
                    // requeued, to allow for travel time + reaction time for
                    // the player to detect the miss
                    newCooldown = .0001f;
                }
                PopulateNextState(
                    results,
                    stateBeforeCast,
                    timeUsed,
                    newCooldown,
                    (1 - hitChance) * chanceOfCast,
                    false);
            } else {
                PopulateNextState(
                    results,
                    stateBeforeCast,
                    timeUsed,
                    Math.Max(newCooldown, RecastPeriod - timeUsed),
                    chanceOfCast,
                    true);
            }

            return results;
        }

        public void RecordSimulatedStat(
            string statName, float value, float weight) {

            if (!SimulatedStats.ContainsKey(statName)) {
                SimulatedStats[statName] = new SimulatedStat();
            }
            SimulatedStats[statName].AddSample(value, weight);
        }

        private void PopulateNextState(
            List<CastingState> results,
            CastingState stateBeforeCast,
            float timeUsed,
            float cooldownAfterAdvance,
            float p,
            bool isHit) {

            CastingState nextState = new CastingState(stateBeforeCast);
            nextState.Probability *= p;
            nextState.AddSpell(this, timeUsed, isHit);
            nextState.Cooldowns[this] = cooldownAfterAdvance;

            if (MySpellTree == SpellTree.Destruction
                && nextState.BackdraftCharges > 0) {

                --nextState.BackdraftCharges;
            }

            results.Add(nextState);
        }

        public float GetTimeUsed(CastingState state) {

            return MaybeApplyBackdraft(
                GetTimeUsed(
                    BaseCastTime, GCDBonus, Mommy.Haste, Mommy.Options.Latency),
                state);
        }

        public float GetCastTime(CastingState state) {

            if (BaseCastTime == 0f) {
                return 0f;
            }

            float avg = 0f;
            foreach (WeightedStat h in Mommy.Haste) {
                avg += h.Chance * BaseCastTime / h.Value;
            }
            return MaybeApplyBackdraft(avg, state);
        }

        private float MaybeApplyBackdraft(float time, CastingState state) {

            if (state != null
                && state.BackdraftCharges > 0
                && MySpellTree == SpellTree.Destruction) {

                time /= 1f + Mommy.Talents.Backdraft * .1f;
            }
            return time;
        }

        public float GetAvgTimeUsed() {

            if (SimulatedStats.ContainsKey("time used")) {
                return SimulatedStats["time used"].GetValue();
            } else {
                return GetTimeUsed(
                    BaseCastTime, GCDBonus, Mommy.Haste, Mommy.Options.Latency);
            }
        }

        public float GetTotalWeight() {

            return SimulatedStats["delay"].GetTotalWeight();
        }

        public float GetAvgTimeBetweenCasts() {

            return Mommy.Options.Duration / GetNumCasts();
        }

        /// <summary>
        /// Assumes the effect == requeue time.
        /// </summary>
        public float GetUprate(CastingState state, Spell spell) {

            // if this method is going to be called, be sure to enable
            // RecordMissesSeparately
            Debug.Assert(RecordMissesSeparately);

            float castTime = spell.GetCastTime(state);
            if (state.Cooldowns.ContainsKey(this)) {
                float cooldown = state.Cooldowns[this] - castTime;
                if (cooldown <= 0 || !state.LastCastHit(this)) {
                    return 0f;
                } else {
                    return 1f;
                }
            }

            float maxQueued = castTime;
            if (Mommy.IsPriorityOrdered(spell, this)) {
                maxQueued += state.GetMaxTimeQueued(this);
            }
            float unqueuable = (state.Elapsed + castTime) - maxQueued;
            float chanceQueued
                = Math.Min(
                    maxQueued / (GetAvgRequeueTime() - unqueuable), 1f);
            return 1 - chanceQueued;
        }

        public virtual void AdjustAfterCastingIsSet() {

            // here for subclasses to override if desired.
        }

        #endregion


        #region setting avg damage

        public virtual void FinalizeSpellModifiers() {

            SpellModifiers.Accumulate(Mommy.SpellModifiers);
            if (Mommy.Options.Imbue.Equals("Grand Spellstone")) {
                if (!GetType().Name.StartsWith("Curse")) {
                    SpellModifiers.AddAdditiveTickMultiplier(.01f);
                }
            } else {
                Debug.Assert(Mommy.Options.Imbue.Equals("Grand Firestone"));
                SpellModifiers.AddAdditiveDirectMultiplier(.01f);
            }
            switch (MagicSchool) {
                case MagicSchool.Shadow:
                    Mommy.AddShadowModifiers(SpellModifiers);
                    break;
                case MagicSchool.Fire:
                    Mommy.AddFireModifiers(SpellModifiers);
                    break;
            }
            if (MySpellTree == SpellTree.Destruction) {
                SpellModifiers.AddCritBonusMultiplier(Mommy.Talents.Ruin * .2f);
                SpellModifiers.AddCritChance(Mommy.Talents.Devastation * .05f);
                float[] talentAffects = { 0f, .04f, .07f, .1f };
            }
            if (CanGetPyroclasm()) {
                Spell conflagrate = Mommy.CastSpells["Conflagrate"];
                SpellModifiers.AddMultiplicativeMultiplier(
                    conflagrate.SpellModifiers.CritChance
                        * SimulatedStats["Pyroclasm Uprate"].GetValue()
                        * Mommy.Talents.Pyroclasm * .02f);
            }
            if (Mommy.Talents.EmpoweredImp > 0 && Mommy.Pet is Imp) {
                SpellModifiers.AddCritChance(
                    GetAvgTimeUsed()
                        * ((Imp) Mommy.Pet).GetCritsPerSec()
                        * Mommy.Talents.EmpoweredImp
                        / 3f);
            }
        }

        public virtual void SetDamageStats(float baseSpellPower) {

            AvgDirectHit
                = (BaseDamage + DirectCoefficient * baseSpellPower)
                    * SpellModifiers.GetFinalDirectMultiplier()
                    * (1 - GetResist());
            AvgTickHit
                = (BaseTickDamage + TickCoefficient * baseSpellPower)
                    * SpellModifiers.GetFinalTickMultiplier()
                    * (1 - GetResist());

            // crit direct damage
            float critChance = SpellModifiers.CritChance;
            float critMultiplier = SpellModifiers.GetFinalCritMultiplier();

            // crit direct damage
            AvgDirectCrit = AvgDirectHit * critMultiplier;
            float avgDirectDamage
                    = Utilities.GetWeightedSum(
                        AvgDirectCrit,
                        critChance,
                        AvgDirectHit,
                        1 - critChance);

            // crit tick damage (if possible)
            float avgTickDamage;
            if (CanTickCrit) {
                AvgTickCrit = AvgTickHit * critMultiplier;
                avgTickDamage
                    = Utilities.GetWeightedSum(
                        AvgTickHit, 1 - critChance, AvgTickCrit, critChance);
            } else {
                AvgTickCrit = AvgTickHit;
                avgTickDamage = AvgTickHit;
            }

            // overall damage
            AvgDamagePerCast
                = Mommy.HitChance
                    * (avgDirectDamage + NumTicks * avgTickDamage);
        }

        protected float GetResist() {

            return StatConversion.GetAverageResistance(
                80, Mommy.Options.TargetLevel, 0f, 0f);
        }

        private bool CanGetPyroclasm() {

            return Mommy.Talents.Pyroclasm > 0
                && !(this is Conflagrate)
                && (MagicSchool == MagicSchool.Fire
                    || MagicSchool == MagicSchool.Shadow)
                && Mommy.CastSpells.ContainsKey("Conflagrate");
        }

        #endregion

    }

    public class ChaosBolt : Spell {

        public ChaosBolt(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                MagicSchool.Fire,
                SpellTree.Destruction,
                .07f, // percentBaseMana,
                2.5f - mommy.Talents.Bane * .1f, // baseCastTime,
                mommy.Talents.GlyphChaosBolt ? 10f : 12f, // cooldown,
                0f, // recastPeriod,
                1429f, // lowDirectDamage,
                1813f, // highDirectDamage,
                (1 + mommy.Talents.ShadowAndFlame * .04f)
                    * .7142f, // directCoefficient,
                0f, // addedDirectMultiplier,
                0f, // bonusCritChance,
                0f) { // bonus crit multiplier

            ApplyImprovedSoulLeech();
        }

        public override bool IsCastable() {

            return Mommy.Talents.ChaosBolt > 0;
        }

        public override List<CastingState> SimulateCast(
            CastingState stateBeforeCast, float chanceOfCast) {

            if (Mommy.Talents.FireAndBrimstone > 0) {
                ((Immolate) Mommy.GetSpell("Immolate"))
                    .RecordUpChance(this, stateBeforeCast);
            }
            return base.SimulateCast(stateBeforeCast, chanceOfCast);
        }

        public override void FinalizeSpellModifiers() {

            base.FinalizeSpellModifiers();
            if (Mommy.Talents.FireAndBrimstone > 0) {
                float uprate = SimulatedStats["immolate up-chance"].GetValue();
                float fullBonus = Mommy.Talents.FireAndBrimstone * .02f;
                SpellModifiers.AddMultiplicativeMultiplier(uprate * fullBonus);
            }
        }
    }

    public class Conflagrate : Spell {

        public const float COOLDOWN = 10f;

        public static bool WillBeCast(CharacterCalculationsWarlock mommy) {

            return mommy.Options.GetActiveRotation().Contains("Conflagrate")
                && mommy.Talents.Conflagrate > 0;
        }

        public Conflagrate(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                MagicSchool.Fire,
                SpellTree.Destruction,
                .16f, // percentBaseMana,
                0f, // baseCastTime,
                COOLDOWN, // cooldown,
                6f, // recastPeriod,
                true, // canMiss,
                .6f * 785f, // lowDirectDamage
                .6f * 785f, // highDirectDamage
                .6f * .2f * 5f, // directCoefficient,
                0f, // addedDirectMultiplier, modified in SetDamageStats
                .4f * 785f / 3f, // baseTickDamage
                3f, // numTicks,
                .4f * .2f * 5f / 3f, // tickCoefficient,
                0f, // addedTickMultiplier, modified in SetDamageStats
                true, // canTickCrit,
                mommy.Talents.FireAndBrimstone * .05f, // bonusCritChance,
                0f) { // bonusCritMultiplier) {

            ApplyImprovedSoulLeech();
            RecordMissesSeparately = true;
        }

        public override bool IsCastable() {

            return Mommy.Talents.Conflagrate > 0;
        }

        public override void FinalizeSpellModifiers() {

            base.FinalizeSpellModifiers();

            // For some reason I may never understand, firestone's 1% bonus gets
            // 1% of Shadow and Flame and 1% of Emberstorm subtracted from it.
            // Also, that modifier becomes multiplicitive instead of additive.
            float direct = SpellModifiers.AdditiveDirectMultiplier;
            SpellModifiers.AddAdditiveDirectMultiplier(-direct);
            direct -= direct * Mommy.Talents.Emberstorm * .03f;
            SpellModifiers.AddMultiplicativeDirectMultiplier(direct);

            // Also account for improvements to immolate, which in turn improve
            // conflagrate
            SpellModifiers.AddAdditiveMultiplier(
                Mommy.Talents.ImprovedImmolate * .1f
                    + (Mommy.Talents.GlyphImmolate ? .1f : 0f)
                    + Mommy.Talents.Aftermath * .03f);
            if (Mommy.Stats.Warlock2T8 > 0) {
                SpellModifiers.AddAdditiveMultiplier(.1f);
            }
        }

        public override List<CastingState> SimulateCast(
            CastingState stateBeforeCast, float chanceOfCast) {

            List<CastingState> states
                = base.SimulateCast(stateBeforeCast, chanceOfCast);

            CastingState stateOnHit = states[0];
            if (Mommy.Talents.Backdraft > 0) {
                stateOnHit.BackdraftCharges = 3;
            }
            if (!Mommy.Talents.GlyphConflag) {
                foreach (CastingState state in states) {
                    state.Cooldowns[Mommy.GetSpell("Immolate")]
                        = -GetTimeUsed(stateBeforeCast);
                }
            }
            return states;
        }
    }

    public class Corruption : Spell {

        public static float GetTickPeriod(CharacterCalculationsWarlock mommy) {

            float period = 3.1f; // total guess
            if (mommy.Talents.GlyphQuickDecay) {
                period /= mommy.AvgHaste;
            }
            return period;
        }

        public Corruption(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                MagicSchool.Shadow, // magic school
                SpellTree.Affliction, // spell tree
                .14f, // percent base mana
                0f, // cast time
                0f, // cooldown
                18f, // recast period
                1080f / 6f, // damage per tick
                6f, // num ticks
                (1.2f + mommy.Talents.EmpoweredCorruption * .12f) / 6f
                    + mommy.Talents.EverlastingAffliction
                        * .01f, // tick coefficient
                mommy.Talents.ImprovedCorruption * .02f
                    + mommy.Talents.Contagion * .01f
                    + mommy.Talents.SiphonLife * .05f, // addedTickMultiplier
                mommy.Talents.Pandemic > 0, // canTickCrit
                (mommy.Talents.Malediction * .03f + mommy.Stats.Warlock2T10)
                    * mommy.Talents.Pandemic, // bonus crit chance
                mommy.Talents.Pandemic) { // bonus crit multiplier

            WarlockTalents talents = Mommy.Talents;
            if (talents.GlyphQuickDecay) {
                RecastPeriod /= Mommy.AvgHaste;
            }

            #region rolling corruption
            // Malficus solved the average duration of a rolling Corruption, see
            // http://rawr.codeplex.com/Thread/View.aspx?ThreadId=203628
            //
            // D = the non-rolling duration
            // T = the time between reset triggers
            // P = the chance a trigger will actually reset the duration
            // TC = the time between corruption ticks
            float d = RecastPeriod;
            float t = GuessRollingTriggerFrequency();
            float p = Mommy.HitChance * talents.EverlastingAffliction * .2f;
            float tc = d / 6f;
            if (p == 1 && t <= d) {
                RecastPeriod = Mommy.Options.Duration;
            } else if (p > 0 && t > 0) {
                float fightLen = Mommy.Options.Duration;
                int maxTriggers = (int) (fightLen * t);
                int maxTicks = (int) (fightLen / tc);

                RecastPeriod
                    = tc
                        * maleficusDuration(
                            new float[maxTicks + 1, maxTriggers + 1],
                            tc,
                            p,
                            1 / t,
                            (int) (fightLen / tc),
                            6,
                            0);
            }
            NumTicks = RecastPeriod / tc;
            #endregion
        }

        public override void FinalizeSpellModifiers() {

            base.FinalizeSpellModifiers();
            SpellModifiers.AddMultiplicativeMultiplier(Mommy.Stats.Warlock4T9);
        }

        public override void SetDamageStats(float baseSpellPower) {

            // stack crit trinkets at the front of a rolling corruption
            if (NumCasts < 2) {
                SpellModifiers.AddCritChance(Mommy.ExtraCritAtMax);
            }
            
            base.SetDamageStats(baseSpellPower);
        }

        private static float maleficusDuration(
            float[,] cache,
            float TC,
            float P,
            float T,
            int maxTicks,
            int accumTicks,
            int triggerIndex) {

            if (accumTicks >= maxTicks)
                return maxTicks;
            double now = triggerIndex * T;
            double curLen = accumTicks * TC;
            if (now > curLen)
                return accumTicks;

            if (cache[accumTicks, triggerIndex] > 0) {
                return cache[accumTicks, triggerIndex];
            }

            float procDuration =
                maleficusDuration(
                cache,
                TC,
                P,
                T,
                maxTicks,
                (int) (now / TC) + 7,
                triggerIndex + 1);
            float nonProcDuration =
                maleficusDuration(
                cache,
                TC,
                P,
                T,
                maxTicks,
                accumTicks,
                triggerIndex + 1);
            float res = procDuration * P + nonProcDuration * (1 - P);

            cache[accumTicks, triggerIndex] = res;
            return res;
        }

        private float GuessRollingTriggerFrequency() {

            float freq = 0f;
            if (GetRotation().Contains("Shadow Bolt")) {

                // assume about 1/2 the time will be spent spamming shadow bolt
                freq
                    += .5f
                        / GetTimeUsed(
                            ShadowBolt.GetBaseCastTime(Mommy),
                            0f,
                            Mommy.Haste,
                            Mommy.Options.Latency);
            }
            if (GetRotation().Contains("Haunt") && Mommy.Talents.Haunt > 0) {

                // assume 11 seconds between haunt casts, on average
                freq += 1f / 11f;
            }
            return freq;
        }
    }

    public class CurseOfAgony : Spell {

        public CurseOfAgony(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                MagicSchool.Shadow, // magic school
                SpellTree.Affliction, // spell tree
                .1f, // percent base mana
                0f, // cast time
                0f, // cooldown
                mommy.Talents.GlyphCoA ? 28f : 24f, // recast period

                // Glyph of Curse of Agony raises the *average* tick to
                // 8/7 its unglyphed value.

                1740f / 12f
                    * (mommy.Talents.GlyphCoA
                        ? 8f / 7f : 1f), // damage per tick
                mommy.Talents.GlyphCoA ? 14f : 12f, // num ticks
                1.2f / 12f, // tick coefficient

                // Notice that spellstone is NOT applied to Curse of Agony.
                // Why?  Ask Blizzard, I have no idea.

                mommy.Talents.ImprovedCurseOfAgony * .05f
                    + mommy.Talents.Contagion * .01f, // addedTickMultiplier
                false, // canTickCrit
                0f, // bonus crit chance
                0f) { // bonus crit multiplier

            GCDBonus = mommy.Talents.AmplifyCurse * .5f;
        }
    }

    public class CurseOfDoom : Spell {

        public CurseOfDoom(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                MagicSchool.Shadow, // magic school
                SpellTree.Affliction, // spell tree
                .15f, // percent base mana
                0f, // baseCastTime,
                60f, // cooldown,
                0f, // recastPeriod,
                7300f, // baseTickDamage,
                1f, // numTicks,
                2f, // tickCoefficient,
                0f, // addedTickMultiplier,
                false, //    canTickCrit,
                0f, // bonusCritChance,
                0f) { // bonusCritMultiplier)

            GCDBonus = mommy.Talents.AmplifyCurse * .5f;
        }
    }

    public class CurseOfTheElements : Spell {

        public CurseOfTheElements(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                0, // magic school
                SpellTree.Affliction, // spell tree
                .1f, // percent base mana
                0f, // cast time
                0f, // cooldown
                300f, // recast period
                true) { // can miss

            GCDBonus = mommy.Talents.AmplifyCurse * .5f;
        }

        public override bool IsCastable() {

            return Mommy.Stats.BonusShadowDamageMultiplier == 0;
        }
    }

    public class Haunt : Spell {

        private float AvgBonus;

        public Haunt(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                MagicSchool.Shadow,
                SpellTree.Affliction,
                .12f, // percent base mana
                1.5f, // cast time
                8f, // cooldown
                0f, // recast period
                645f, // low direct damage
                753f, // high direct damage
                .4266f, // direct coefficient
                0f, // bonus direct multiplier
                0f, // bonus crit chance
                mommy.Talents.Pandemic) { } // bonus crit multiplier

        public override bool IsCastable() {

            return Mommy.Talents.Haunt > 0;
        }

        public float GetAvgTickBonus() {

            if (AvgBonus > 0) {
                return AvgBonus;
            }

            // Consider the window from cast time until the next (actual, not
            // planned) cast time.  This can be broken into three cases, when
            // it hits, when it misses & the previous cast hit, and when it
            // misses & the previous cast missed. Calculate the average uprate
            // in each case, then combine them with a weighted average to get
            // the overall uprate.

            float uprate;
            float tolerance = 12f - RecastPeriod;
            SimulatedStat delayStats = SimulatedStats["delay"];
            SimulatedStat hitUprate = new SimulatedStat();
            SimulatedStat missFollowingHitUprate = new SimulatedStat();
            for (int i = delayStats.Values.Count; --i >= 0; ) {
                float collisionDelay = delayStats.Values[i];
                float probability = delayStats.Weights[i];

                // CASE 1: this cast hits.
                // uprate = duration / window
                // duration = 12
                // window = RecastPeriod + collisionDelay
                uprate = Math.Min(1f, 12f / (RecastPeriod + collisionDelay));
                hitUprate.AddSample(uprate, probability);

                // CASE 2: this cast misses, previous cast hit.
                // uprate = leftoverUptime / window
                // leftoverUptime = tolerance - collisionDelay
                // window = Cooldown + collisionDelay
                uprate
                    = Math.Max(tolerance - collisionDelay, 0)
                        / (Cooldown + delayStats.GetValue());
                missFollowingHitUprate.AddSample(uprate, probability);

                // CASE 3: this cast misses, previous cast missed.
                // This case will always yeild zero uptime/uprate.
            }

            // average them all together for the overall uprate
            float hitChance = Mommy.HitChance;
            float missChance = 1 - hitChance;
            uprate = Utilities.GetWeightedSum(
                hitUprate.GetValue(),
                hitChance,
                missFollowingHitUprate.GetValue(),
                missChance * hitChance,
                0f,
                missChance * missChance);

            AvgBonus = (Mommy.Talents.GlyphHaunt ? .23f : .2f) * uprate;
            return AvgBonus;
        }
    }

    public class Immolate : Spell {

        public static float GetRecastPeriod(
            CharacterCalculationsWarlock mommy) {

            return 15f + mommy.Talents.MoltenCore * 3f;
        }

        public static bool IsClippedByConflagrate(
            CharacterCalculationsWarlock mommy) {

            return Conflagrate.WillBeCast(mommy) && !mommy.Talents.GlyphConflag;
        }

        public Immolate(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                MagicSchool.Fire,
                SpellTree.Destruction,
                .17f, // percentBaseMana,
                2f - mommy.Talents.Bane * .1f, // baseCastTime,
                0f, // cooldown,
                GetRecastPeriod(mommy), // recastPeriod,
                true, // canMiss,
                460f, // lowDirectDamage,
                460f, // highDirectDamage,
                .2f, // directCoefficient,
                mommy.Talents.ImprovedImmolate * .1f, // addedDirectMultiplier,
                785f / 5f, // baseTickDamage,
                5f + mommy.Talents.MoltenCore, // numTicks,
                .2f, // tickCoefficient,
                (mommy.Talents.GlyphImmolate ? .1f : 0f)
                    + mommy.Talents.ImprovedImmolate * .1f
                    + mommy.Talents.Aftermath * .03f, // addedTickMultiplier,
                true, // canTickCrit,
                0f, // bonusCritChance,
                0f) { // bonusCritMultiplier) {

            RecordMissesSeparately = true;
        }

        public override float GetQueueProbability(CastingState state) {

            if (IsClippedByConflagrate(Mommy)
                && !state.Cooldowns.ContainsKey(this)) {

                return 0f;
            } else {
                return base.GetQueueProbability(state);
            }
        }

        public override List<CastingState> SimulateCast(
            CastingState stateBeforeCast, float chanceOfCast) {

            List<CastingState> states
                = base.SimulateCast(stateBeforeCast, chanceOfCast);
            if (IsClippedByConflagrate(Mommy)) {
                RecordSimulatedStat(
                    "downtime",
                    -stateBeforeCast.Cooldowns[this],
                    stateBeforeCast.Probability);
            }
            return states;
        }

        public override float GetNumCasts() {

            if (IsClippedByConflagrate(Mommy)) {
                return Mommy.CastSpells["Conflagrate"].GetNumCasts();
            } else {
                return base.GetNumCasts();
            }
        }

        public override void AdjustAfterCastingIsSet() {

            if (IsClippedByConflagrate(Mommy)) {
                float downtime = SimulatedStats["downtime"].GetValue();
                float uptime = GetAvgTimeBetweenCasts() - downtime;
                NumTicks = uptime / 3f;
            }
        }

        public override void FinalizeSpellModifiers() {
            
            base.FinalizeSpellModifiers();
            SpellModifiers.AddMultiplicativeMultiplier(Mommy.Stats.Warlock4T9);
            if (Mommy.Stats.Warlock2T8 > 0) {
                SpellModifiers.AddAdditiveMultiplier(.1f);
            }
        }

        /// <summary>
        /// Records the chance that a given spell is cast while immolate is on
        /// the target.
        /// </summary>
        public void RecordUpChance(Spell spell, CastingState state) {

            float chance;
            if (!state.Cooldowns.ContainsKey(this)
                && IsClippedByConflagrate(Mommy)) {

                // this does not actually garuntee immolate is up.  For example,
                // if the priority list starts: CoD > Corr > Chaos > Immo > Conf
                // then it is possible for immolate to fall off while casting
                // CoD + Corr so that the upchance on Chaos < 1.  But even then
                // it's a rare case, and that is not a priority list anyone is
                // actually going to use, so we'll just stick with this
                // approximation.
                chance = 1f;
            } else {
                chance = GetUprate(state, spell);
            }
            spell.RecordSimulatedStat(
                "immolate up-chance", chance, state.Probability);
        }
    }

    public class ImmolationAura : Spell {

        public ImmolationAura(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                MagicSchool.Fire,
                SpellTree.Demonology,
                .64f, // percentBaseMana,
                0f, // baseCastTime,
                180f * (1f - mommy.Talents.Nemesis * .1f), // cooldown,
                0f, // recastPeriod,
                251f, // baseTickDamage,
                15f, // numTicks,
                2.14f / 15f, // tickCoefficient,
                0f, // addedTickMultiplier,
                false, // canTickCrit,
                0f, // bonusCritChance,
                0f) { } // bonusCritMultiplier)

        public override bool IsCastable() {
            
            return Mommy.Talents.Metamorphosis > 0;
        }

        public override void FinalizeSpellModifiers() {
            
            base.FinalizeSpellModifiers();

            // immolation aura always gets the full metamorphosis bonus
            SpellModifiers.AddMultiplicativeDirectMultiplier(
                1.2f / (1 + Mommy.GetMetamorphosisBonus()) - 1);
        }
    }

    public class Incinerate : Spell {

        public Incinerate(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                MagicSchool.Fire,
                SpellTree.Destruction,
                .14f, // percentBaseMana,
                2.5f - mommy.Talents.Emberstorm * .05f, // baseCastTime,
                0f, // cooldown,
                0f, // recastPeriod,
                582f, // lowDirectDamage,
                676f, // highDirectDamage,
                (1 + mommy.Talents.ShadowAndFlame * .04f)
                    * .7143f, // directCoefficient,
                mommy.Talents.GlyphIncinerate
                    ? .05f : 0f, // addedDirectMultiplier,
                mommy.Stats.Warlock2T10
                    + mommy.Stats.Warlock4T8, // bonusCritChance,
                0f) { // bonus crit multiplier

            ApplyImprovedSoulLeech();
        }

        public override void FinalizeSpellModifiers() {

            base.FinalizeSpellModifiers();
            float immoUp = GetImmolateUpRate();
            BaseDamage *= 1f + (.25f * immoUp);
            if (Mommy.Talents.FireAndBrimstone > 0) {
                float fullBonus = Mommy.Talents.FireAndBrimstone * .02f;
                SpellModifiers.AddMultiplicativeMultiplier(immoUp * fullBonus);
            }
        }

        protected virtual float GetImmolateUpRate() {

            if (Immolate.IsClippedByConflagrate(Mommy)) {
                return 1f;
            } else {
                return 1 - GetCastTime(null) / Immolate.GetRecastPeriod(Mommy);
            }
        }
    }

    public class Incinerate_UnderBackdraft : Incinerate {

        public Incinerate_UnderBackdraft(CharacterCalculationsWarlock mommy)
            : base(mommy) { }

        public override bool IsCastable() {

            return Mommy.Talents.Backdraft > 0 && Conflagrate.WillBeCast(Mommy);
        }

        public override float GetQueueProbability(CastingState state) {

            if (state.BackdraftCharges > 0) {
                return 1f;
            } else {
                return 0f;
            }
        }

        public override float GetNumCasts() {

            if (NumCasts == 0) {
                Spell conflagrate = Mommy.CastSpells["Conflagrate"];
                float castsPerConf
                    = GetTotalWeight() / conflagrate.GetTotalWeight();
                NumCasts = castsPerConf * conflagrate.GetNumCasts();
            }
            return NumCasts;
        }

        public override List<CastingState> SimulateCast(
            CastingState stateBeforeCast, float chanceOfCast) {

            if (Mommy.Talents.FireAndBrimstone > 0) {
                ((Immolate) Mommy.GetSpell("Immolate"))
                    .RecordUpChance(this, stateBeforeCast);
            }
            return base.SimulateCast(stateBeforeCast, chanceOfCast);
        }

        protected override float GetImmolateUpRate() {
            if (!SimulatedStats.ContainsKey("immolate up-chance")) return 0f;
            return SimulatedStats["immolate up-chance"].GetValue();
        }
    }

    public class Incinerate_UnderMoltenCore : Incinerate {

        public Incinerate_UnderMoltenCore(CharacterCalculationsWarlock mommy)
            : base(mommy) {

            BaseCastTime /= 1f + Mommy.Talents.MoltenCore * .1f;

            float procChance = Mommy.Talents.MoltenCore * .04f;
            float tickPeriod = Corruption.GetTickPeriod(mommy);
            Cooldown = tickPeriod / procChance;
        }

        public override bool IsCastable() {

            return Mommy.Talents.MoltenCore > 0
                && GetRotation().Contains("Corruption");
        }

        public override bool IsCastDuringExecute() {
            
            return true;
        }

        public override float GetQueueProbability(CastingState state) {

            if (state.MoltenCoreCharges > 0) {
                return 1f;
            } else {
                return base.GetQueueProbability(state);
            }
        }

        public override List<CastingState> SimulateCast(CastingState stateBeforeCast, float chanceOfCast) {
            
            List<CastingState> states
                = base.SimulateCast(stateBeforeCast, chanceOfCast);
            foreach (CastingState state in states) {
                state.Cooldowns.Remove(this);
                if (state.MoltenCoreCharges > 0) {
                    --state.MoltenCoreCharges;
                } else {
                    state.MoltenCoreCharges = 2;
                }
            }
            return states;
        }

        public override void FinalizeSpellModifiers() {
            
            base.FinalizeSpellModifiers();
            SpellModifiers.AddMultiplicativeMultiplier(
                Mommy.Talents.MoltenCore * .06f);
        }
    }

    public class LifeTap : Spell {

        // This exists as a spell object only for the case when we would have
        // enough mana to never life tap, but still need to keep up the gyph. So
        // make it's cooldown 37s (40s duration of the glyph minus just about
        // enough time to make cooldown collisions & such don't let it drop
        // off).

        public LifeTap(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                0, // magic school
                SpellTree.Affliction, // spell tree
                0f, // percent base mana (overwritten below)
                0f, // cast time
                0f, // cooldown
                37f, // recast period
                false) { // can miss

            ManaCost
                = (-2000f - mommy.CalcSpellPower() * .5f)
                    * (1f + mommy.Talents.ImprovedLifeTap * .1f);
        }

        public override bool IsCastable() {

            return Mommy.Talents.GlyphLifeTap;
        }

        public float AddCastsForRegen(
            float timeRemaining, float manaRemaining, Spell spammedSpell) {

            // getting our cast time is not safe until the backdraft multilpier
            // has been set.  fortunetly that's easy to "calculate"
            //BackdraftMultiplier = 1f;

            // The number of needed lifetaps is obtained by solving this
            // system of equations:
            //
            // spamCasts * spamMana + ltCasts * ltMana = manaRemaining
            // spamCasts * spamCast + ltCasts * ltCast = timeRemaining

            float latency = Mommy.Options.Latency;
            float a = spammedSpell.ManaCost;
            float b = ManaCost;
            float c = manaRemaining;
            float d = spammedSpell.GetAvgTimeUsed();
            float e = GetAvgTimeUsed();
            float f = timeRemaining;
            float toAdd = Math.Max(0f, (c * d - a * f) / (b * d - a * e));

            NumCasts += toAdd;

            if (toAdd > 0 && !Mommy.CastSpells.ContainsKey("Life Tap")) {
                Mommy.CastSpells.Add("Life Tap", this);
            }

            return toAdd;
        }

        public float GetAvgBonusSpellPower() {

            if (!Mommy.Talents.GlyphLifeTap) {
                return 0f;
            }

            float uprate
                = Math.Min(
                    1f, (NumCasts + .8f) * 40f / Mommy.Options.Duration);
            return uprate * Mommy.CalcSpirit() * .2f;
        }
    }

    public class ShadowBolt : Spell {

        public static float GetBaseCastTime(
            CharacterCalculationsWarlock mommy) {

            return 3f - mommy.Talents.Bane * .1f;
        }

        public ShadowBolt(CharacterCalculationsWarlock mommy)
            : base(
                mommy, // options
                MagicSchool.Shadow, // magic school
                SpellTree.Destruction, // spell tree
                .17f, // percent base mana
                GetBaseCastTime(mommy), // cast time
                0f, // cooldown
                0f, // recast period
                694f, // low base
                775f, // high base
                (1 + mommy.Talents.ShadowAndFlame * .04f)
                    * .8571f, // direct coefficient
                mommy.Talents.ImprovedShadowBolt
                    * .01f, // addedDirectMultiplier
                mommy.Stats.Warlock2T10
                    + mommy.Stats.Warlock4T8, // bonus crit chance
                0f) { // bonus crit multiplier

            ApplyImprovedSoulLeech();
        }
    }

    public class ShadowBolt_Instant : ShadowBolt {

        public static bool IsCastable(
            WarlockTalents talents, List<string> priorities) {

            return priorities.Contains("Corruption")
                && (talents.GlyphCorruption || talents.Nightfall > 0);
        }

        public ShadowBolt_Instant(CharacterCalculationsWarlock mommy)
            : base(mommy) {

            BaseCastTime = 0f;

            // Currently modeled as a spell on a cooldown equal to the
            // average time between procs.  This lengthens the time between
            // casts according to the rules for cooldown collision, which is not
            // completely accurate, but close enough.  To be accurate it
            // should instead factor in the probability that it will proc twice
            // (or more) while casting higher priority spells.
            float procChance = Mommy.Talents.Nightfall * .02f;
            if (Mommy.Talents.GlyphCorruption) {
                procChance += .04f;
            }
            float tickPeriod = Corruption.GetTickPeriod(mommy);
            Cooldown = tickPeriod / procChance;
        }

        public override bool IsCastable() {

            return IsCastable(Mommy.Talents, GetRotation().SpellPriority);
        }
    }

    public class SoulFire : Spell {

        public SoulFire(CharacterCalculationsWarlock mommy) 
            : base(
                mommy,
                MagicSchool.Fire, // magicSchool,
                SpellTree.Destruction, // spellTree,
                .09f, // percentBaseMana,
                (6f - mommy.Talents.Bane * .4f)
                    * (1 - mommy.Talents.Decimation * .2f), // baseCastTime,
                0f, // cooldown,
                0f, // recastPeriod,
                1323f, // lowDirectDamage,
                1657f, // highDirectDamage,
                1.15f, // directCoefficient,
                0f, // addedDirectMultiplier,
                0f, // bonusCritChance,
                0f) { } // bonusCritMultiplier) { }

        public override bool IsCastable() {
            
            return Mommy.Talents.Decimation > 0;
        }
    }

    public class UnstableAffliction : Spell {

        public UnstableAffliction(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                MagicSchool.Shadow,
                SpellTree.Affliction,
                .15f, // percent base mana
                mommy.Talents.GlyphUA ? 1.3f : 1.5f, // cast time
                0f, // cooldown
                15f, // recast period
                1150f / 5f, // tick damage
                5f, // num ticks
                mommy.Talents.EverlastingAffliction * .01f
                    + 1f / 5f, // tick coefficient
                mommy.Talents.SiphonLife * .05f, // addedTickMultiplier
                mommy.Talents.Pandemic > 0, // canTickCrit
                mommy.Talents.Malediction * .03f
                    * mommy.Talents.Pandemic, // bonus crit chance
                mommy.Talents.Pandemic) { // bonus crit multiplier

            if (mommy.Talents.GlyphUA) {
                GCDBonus = .2f;
            }
        }

        public override bool IsCastable() {

            return Mommy.Talents.UnstableAffliction > 0;
        }

        public override void FinalizeSpellModifiers() {

            base.FinalizeSpellModifiers();
            SpellModifiers.AddMultiplicativeMultiplier(Mommy.Stats.Warlock4T9);
            if (Mommy.Stats.Warlock2T8 > 0) {
                SpellModifiers.AddAdditiveMultiplier(.2f);
            }
        }
    }
}
//3456789 223456789 323456789 423456789 523456789 623456789 723456789 8234567890
