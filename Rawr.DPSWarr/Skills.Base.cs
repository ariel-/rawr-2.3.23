/**********
 * Owner: Shared
 **********/
using System;

namespace Rawr.DPSWarr.Skills
{
    #region Base
    public enum AttackTableSelector { Missed = 0, Dodged, Parried, Blocked, Crit, Glance, Hit }
    // White Damage + White Rage Generated
    public class WhiteAttacks
    {
        // Constructors
        public WhiteAttacks(Character character, Stats stats, CombatFactors cf, CalculationOptionsDPSWarr calcOpts, BossOptions bossOpts)
        {
            Char = character;
            StatS = stats;
            Talents = Char.WarriorTalents == null ? new WarriorTalents() : Char.WarriorTalents;
            //TalentsCata = Char.WarriorTalentsCata == null ? new WarriorTalentsCata() : Char.WarriorTalentsCata;
            combatFactors = cf;
            CalcOpts = calcOpts;
            BossOpts = bossOpts;
            MHAtkTable = new AttackTable(Char, StatS, combatFactors, calcOpts, bossOpts, true, false, false);
            OHAtkTable = new AttackTable(Char, StatS, combatFactors, calcOpts, bossOpts, false, false, false);
            FightDuration =
#if RAWR3 || SILVERLIGHT
                    BossOpts.BerserkTimer;
#else
                    CalcOpts.Duration;
#endif
            //
            HSOverridesOverDur = 0f;
            CLOverridesOverDur = 0f;
            Slam_ActsOverDur = 0f;
            _uwProcValue_mh = combatFactors._c_mhItemSpeed * Talents.UnbridledWrath / 20.0f;
            _uwProcValue_oh = combatFactors._c_ohItemSpeed * Talents.UnbridledWrath / 20.0f;
        }
        public void InvalidateCache()
        {
            _MhDamageOnUse = _MHSwingRage = _OhDamageOnUse = _OHSwingRage = -1f;
        }
        #region Variables
        private readonly Character Char;
        private Stats StatS;
        private readonly WarriorTalents Talents;
        private readonly WarriorTalentsCata TalentsCata;
        private readonly CombatFactors combatFactors;
        private CalculationOptionsDPSWarr CalcOpts;
        private BossOptions BossOpts;
        public AttackTable MHAtkTable { get; private set; }
        public AttackTable OHAtkTable { get; private set; }
        private float _uwProcValue_mh;
        private float _uwProcValue_oh;
        public float MHUWProcValue { get { return _uwProcValue_mh; } }
        public float OHUWProcValue { get { return _uwProcValue_oh; } }
        private float FightDuration;
        private float AvgTargets
        {
            get
            {
#if RAWR3 || SILVERLIGHT
                /*if (BossOpts.MultiTargs) {
                    return 1f + (Math.Min((float)BossOpts.MaxNumTargets, 1f) - 1f) * (float)BossOpts.MultiTargsPerc + StatS.BonusTargets;
                }*/
                if (BossOpts.MultiTargs && BossOpts.Targets != null && BossOpts.Targets.Count > 0)
                {
                    float value = 0;
                    foreach (TargetGroup tg in BossOpts.Targets) {
                        if (tg.Frequency <= 0 || tg.Chance <= 0) continue; // bad one, skip it
                        float upTime = (tg.Frequency / BossOpts.BerserkTimer * (tg.Duration/1000f) * tg.Chance) / BossOpts.BerserkTimer;
                        value += (Math.Max(10, tg.NumTargs - (tg.NearBoss ? 0 : 1) + StatS.BonusTargets)) * upTime;
                    }
                    return 1f + value;
                }
#else
                if (CalcOpts.MultipleTargets) { return 1f + (Math.Min(CalcOpts.MultipleTargetsMax, 1f) - 1f) * CalcOpts.MultipleTargetsPerc / 100f + StatS.BonusTargets; }
#endif
                else { return 1f; }
            }
        }
        // Get/Set
        public float HSOverridesOverDur { get; set; }
        public float CLOverridesOverDur { get; set; }
        public float Slam_ActsOverDur;
        #endregion
        // bah
        private float SlamFreqSpdMod { get { return (Slam_ActsOverDur == 0f ? 0f : ((1.5f - (0.5f * Talents.ImprovedSlam)) * (Slam_ActsOverDur / FightDuration))); } }
        // Main Hand
        public float MhEffectiveSpeed { get { return combatFactors.MHSpeed + SlamFreqSpdMod; } }
        public float MhDamage
        {
            get
            {
                return combatFactors.AvgMhWeaponDmgUnhasted * AvgTargets;
            }
        }
        private float _MhDamageOnUse = -1f;
        public float MhDamageOnUse
        {
            get
            {
                if (_MhDamageOnUse == -1f)
                {
                    float dmg = MhDamage;                  // Base Damage
                    dmg *= combatFactors.DamageBonus;      // Global Damage Bonuses
                    dmg *= combatFactors.DamageReduction;  // Global Damage Penalties

                    // Work the Attack Table
                    float dmgDrop = (1f
                        - MHAtkTable.Miss   // no damage when being missed
                        - MHAtkTable.Dodge  // no damage when being dodged
                        - MHAtkTable.Parry  // no damage when being parried
                        - MHAtkTable.Glance // glancing handled below
                        - MHAtkTable.Block  // blocked handled below
                        - MHAtkTable.Crit); // crits   handled below

                    float dmgGlance = dmg * MHAtkTable.Glance * combatFactors.ReducWhGlancedDmg;//Partial Damage when glancing
                    float dmgBlock = dmg * MHAtkTable.Block * combatFactors.ReducWhBlockedDmg;//Partial damage when blocked
                    float dmgCrit = dmg * MHAtkTable.Crit * (1f + combatFactors.BonusWhiteCritDmg);//Bonus Damage when critting

                    dmg *= dmgDrop;

                    dmg += dmgGlance + dmgBlock + dmgCrit;

                    _MhDamageOnUse = dmg;
                }
                return _MhDamageOnUse;
            }
        }
        public float AvgMhDamageOnUse { get { return MhDamageOnUse * MhActivates; } }
        public float MhActivates
        {
            get
            {
                if (MhEffectiveSpeed != 0)
                {
                    // floating point arithmetic fail, need to do it this way or we get negative numbers at 100% override
                    float f = FightDuration / MhEffectiveSpeed;
                    return f - HSOverridesOverDur - CLOverridesOverDur;
                }
                else return 0f;
            }
        }
        public float MhActivatesNoHS
        {
            get
            {
                if (MhEffectiveSpeed != 0)
                    return FightDuration / MhEffectiveSpeed;
                else return 0f;
            }
        }
        public float MhDPS { get { return AvgMhDamageOnUse / FightDuration; } }
        // Off Hand
        public float OhEffectiveSpeed { get { return combatFactors.OHSpeed + SlamFreqSpdMod; } }
        public float OhDamage
        {
            get
            {
                //float DamageBase = combatFactors.AvgOhWeaponDmgUnhasted;
                //float DamageBonus = 1f + 0f;
                return combatFactors.AvgOhWeaponDmgUnhasted * AvgTargets;
            }
        }
        private float _OhDamageOnUse = -1f;
        public float OhDamageOnUse
        {
            get
            {
                if (_OhDamageOnUse == -1f)
                {
                    float dmg = OhDamage;                  // Base Damage
                    dmg *= combatFactors.DamageBonus;      // Global Damage Bonuses
                    dmg *= combatFactors.DamageReduction;  // Global Damage Penalties

                    // Work the Attack Table
                    float dmgDrop = (1f
                        - OHAtkTable.Miss   // no damage when being missed
                        - OHAtkTable.Dodge  // no damage when being dodged
                        - OHAtkTable.Parry  // no damage when being parried
                        - OHAtkTable.Glance // glancing handled below
                        - OHAtkTable.Block  // blocked handled below
                        - OHAtkTable.Crit); // crits handled below

                    float dmgGlance = dmg * OHAtkTable.Glance * combatFactors.ReducWhGlancedDmg;//Partial Damage when glancing
                    float dmgBlock = dmg * OHAtkTable.Block * combatFactors.ReducWhBlockedDmg;//Partial damage when blocked
                    float dmgCrit = dmg * OHAtkTable.Crit * (1f + combatFactors.BonusWhiteCritDmg);//Bonus   Damage when critting

                    dmg *= dmgDrop;

                    dmg += dmgGlance + dmgBlock + dmgCrit;

                    _OhDamageOnUse = dmg;
                }
                return _OhDamageOnUse;
            }
        }
        public float AvgOhDamageOnUse { get { return OhDamageOnUse * OhActivates; } }
        public float OhActivates
        {
            get
            {
                if (OhEffectiveSpeed > 0f) return FightDuration / OhEffectiveSpeed;
                else return 0;
            }
        }
        public float OhDPS { get { return AvgOhDamageOnUse / FightDuration; } }
        // Rage Calcs
        private float _MHSwingRage = -1f;
        public float MHSwingRage
        {
            get
            {
                if (_MHSwingRage == -1f)
                {
                    // d = damage amount
                    // s = weapon speed
                    // f = hit factor
                    float s = combatFactors._c_mhItemSpeed;
                    float based = combatFactors.AvgMhWeaponDmgUnhasted * combatFactors.DamageBonus * combatFactors.DamageReduction;

                    _MHSwingRage = RageFormula(based, 3.5f * s) * (MHAtkTable.Hit + MHAtkTable.Dodge + MHAtkTable.Parry) +
                                   RageFormula(based * combatFactors.ReducWhGlancedDmg, 3.5f * s) * (MHAtkTable.Glance) +
                                   RageFormula(based * (1f + combatFactors.BonusWhiteCritDmg), 7.0f * s) * MHAtkTable.Crit;
                }
                return _MHSwingRage;
            }
        }
        private float _OHSwingRage = -1f;
        public float OHSwingRage
        {
            get
            {
                if (_OHSwingRage == -1f)
                {
                    // d = damage amount
                    // s = weapon speed
                    // f = hit factor
                    float s = combatFactors._c_ohItemSpeed;
                    float based = combatFactors.AvgOhWeaponDmgUnhasted * combatFactors.DamageBonus * combatFactors.DamageReduction;
                    _OHSwingRage = RageFormula(based, 1.75f * s) * (OHAtkTable.Hit + OHAtkTable.Dodge + OHAtkTable.Parry) +
                                   RageFormula(based * combatFactors.ReducWhGlancedDmg, 1.75f * s) * OHAtkTable.Glance +
                                   RageFormula(based * (1f + combatFactors.BonusWhiteCritDmg), 3.5f * s) * OHAtkTable.Crit;
                }
                return _OHSwingRage;
            }
        }
        public float MHRageGenOverDur { get { return MhActivates * (MHSwingRage + MHUWProcValue); } }
        public float MHRageGenOverDurNoHS { get { return MhActivatesNoHS * (MHSwingRage + MHUWProcValue); } }
        public float OHRageGenOverDur
        {
            get
            {
                if (combatFactors.useOH)
                {
                    return OhActivates * (OHSwingRage + OHUWProcValue);
                }
                return 0f;
            }
        }
        // Rage generated per second
        private float MHRageRatio
        {
            get
            {
                return MHRageGenOverDur / (MHRageGenOverDur + OHRageGenOverDur);
            }
        }
        public float whiteRageGenOverDur { get { return MHRageGenOverDur + OHRageGenOverDur; } }
        public float whiteRageGenOverDurNoHS { get { return MHRageGenOverDurNoHS + OHRageGenOverDur; } }

        private const float c_const = 0.016545334215751158173395102581072f; //7.5f / 453.3f;
        private const float c_const2 = 0.033090668431502316346790205162144f; // 2*c_const
        private const float c_const3 = 0.049636002647253474520185307743216f; // 3*c_const
        private float RageFormula(float d, float fs)
        {
            return (4f + Talents.EndlessRage) / 4f * ((fs > c_const3 * d) ? (c_const2 * d) : (c_const * d + fs) / 2.0f);
        }
        // Attacks Over Fight Duration
        public float LandedAtksOverDur { get { return LandedAtksOverDurMH + LandedAtksOverDurOH; } }
        public float LandedAtksOverDurMH { get { return MhActivates * MHAtkTable.AnyLand; } }
        public float LandedAtksOverDurOH { get { return (combatFactors.useOH ? OhActivates * OHAtkTable.AnyLand : 0f); } }
        private float CriticalAtksOverDur { get { return CriticalAtksOverDurMH + CriticalAtksOverDurOH; } }
        public float CriticalAtksOverDurMH { get { return MhActivates * MHAtkTable.Crit; } }
        public float CriticalAtksOverDurOH { get { return (combatFactors.useOH ? OhActivates * OHAtkTable.Crit : 0f); } }
        // Other
        public float RageSlip(float abilInterval, float rageCost) {
            if (!combatFactors.useOH && MhActivates <= 0f) { return 0f; }
            return (MHAtkTable.Miss * rageCost) / (abilInterval * ((MhActivates * (MHSwingRage + MHUWProcValue) + (combatFactors.useOH ? OhActivates * (OHSwingRage + OHUWProcValue) : 0f)) / FightDuration));
        }
        public virtual float GetXActs(AttackTableSelector i, float acts, bool isMH) {
            AttackTable table = (isMH ? MHAtkTable : OHAtkTable);
            float retVal = 0f;
            switch (i) {
                case AttackTableSelector.Missed: { retVal = acts * table.Miss; break; }
                case AttackTableSelector.Dodged: { retVal = acts * table.Dodge; break; }
                case AttackTableSelector.Parried: { retVal = acts * table.Parry; break; }
                case AttackTableSelector.Blocked: { retVal = acts * table.Block; break; }
                case AttackTableSelector.Crit: { retVal = acts * table.Crit; break; }
                case AttackTableSelector.Glance: { retVal = acts * table.Glance; break; }
                case AttackTableSelector.Hit: { retVal = acts * table.Hit; break; }
                default: { break; }
            }
            return retVal;
        }
        public virtual string GenTooltip(float ttldpsMH, float ttldpsOH, float ttldps)
        {
            // ==== MAIN HAND ====
            float acts = MhActivates;
            float misses = GetXActs(AttackTableSelector.Missed, acts, true), missesPerc = (acts == 0f ? 0f : misses / acts);
            float dodges = GetXActs(AttackTableSelector.Dodged, acts, true), dodgesPerc = (acts == 0f ? 0f : dodges / acts);
            float parrys = GetXActs(AttackTableSelector.Parried, acts, true), parrysPerc = (acts == 0f ? 0f : parrys / acts);
            float blocks = GetXActs(AttackTableSelector.Blocked, acts, true), blocksPerc = (acts == 0f ? 0f : blocks / acts);
            float crits = GetXActs(AttackTableSelector.Crit, acts, true), critsPerc = (acts == 0f ? 0f : crits / acts);
            float glncs = GetXActs(AttackTableSelector.Glance, acts, true), glncsPerc = (acts == 0f ? 0f : glncs / acts);
            float hits = GetXActs(AttackTableSelector.Hit, acts, true), hitsPerc = (acts == 0f ? 0f : hits / acts);

            bool showmisss = misses > 0f;
            bool showdodge = dodges > 0f;
            bool showparry = parrys > 0f;
            bool showblock = blocks > 0f;
            bool showcrits = crits > 0f;

            string tooltip = "*" + "White Damage (Main Hand)" +
                Environment.NewLine + "Cast Time: Instant"
                                    + ", CD: " + (MhEffectiveSpeed != -1 ? MhEffectiveSpeed.ToString("0.00") : "None")
                                    + ", Rage Generated: " + (MHSwingRage != -1 ? MHSwingRage.ToString("0.00") : "None") +
            Environment.NewLine + Environment.NewLine + acts.ToString("000.00") + " Activates over Attack Table:" +
            (showmisss ? Environment.NewLine + "- " + misses.ToString("000.00") + " : " + missesPerc.ToString("00.00%") + " : Missed " : "") +
            (showdodge ? Environment.NewLine + "- " + dodges.ToString("000.00") + " : " + dodgesPerc.ToString("00.00%") + " : Dodged " : "") +
            (showparry ? Environment.NewLine + "- " + parrys.ToString("000.00") + " : " + parrysPerc.ToString("00.00%") + " : Parried " : "") +
            (showblock ? Environment.NewLine + "- " + blocks.ToString("000.00") + " : " + blocksPerc.ToString("00.00%") + " : Blocked " : "") +
            (showcrits ? Environment.NewLine + "- " + crits.ToString("000.00") + " : " + critsPerc.ToString("00.00%") + " : Crit " : "") +
                         Environment.NewLine + "- " + glncs.ToString("000.00") + " : " + glncsPerc.ToString("00.00%") + " : Glanced " +
                         Environment.NewLine + "- " + hits.ToString("000.00") + " : " + hitsPerc.ToString("00.00%") + " : Hit " +
                Environment.NewLine +
                //Environment.NewLine + "Damage per Blocked|Hit|Crit: x|x|x" +
                Environment.NewLine + "Targets Hit: " + AvgTargets.ToString("0.00") +
                Environment.NewLine + "DPS: " + (ttldpsMH > 0 ? ttldpsMH.ToString("0.00") : "None") +
                Environment.NewLine + "Percentage of Total DPS: " + (ttldpsMH > 0 ? (ttldpsMH / ttldps).ToString("00.00%") : "None");

            if (combatFactors.useOH)
            {
                // ==== OFF HAND ====
                acts = OhActivates;
                misses = GetXActs(AttackTableSelector.Missed, acts, false); missesPerc = (acts == 0f ? 0f : misses / acts);
                dodges = GetXActs(AttackTableSelector.Dodged, acts, false); dodgesPerc = (acts == 0f ? 0f : dodges / acts);
                parrys = GetXActs(AttackTableSelector.Parried, acts, false); parrysPerc = (acts == 0f ? 0f : parrys / acts);
                blocks = GetXActs(AttackTableSelector.Blocked, acts, false); blocksPerc = (acts == 0f ? 0f : blocks / acts);
                crits = GetXActs(AttackTableSelector.Crit, acts, false); critsPerc = (acts == 0f ? 0f : crits / acts);
                glncs = GetXActs(AttackTableSelector.Glance, acts, false); glncsPerc = (acts == 0f ? 0f : glncs / acts);
                hits = GetXActs(AttackTableSelector.Hit, acts, false); hitsPerc = (acts == 0f ? 0f : hits / acts);

                showmisss = misses > 0f;
                showdodge = dodges > 0f;
                showparry = parrys > 0f;
                showblock = blocks > 0f;
                showcrits = crits > 0f;

                tooltip += Environment.NewLine + Environment.NewLine + "White Damage (Off Hand)" +
                    Environment.NewLine + "Cast Time: Instant"
                                        + ", CD: " + (OhEffectiveSpeed != -1 ? OhEffectiveSpeed.ToString("0.00") : "None")
                                        + ", Rage Generated: " + (OHSwingRage != -1 ? OHSwingRage.ToString("0.00") : "None") +
                Environment.NewLine + Environment.NewLine + acts.ToString("000.00") + " Activates over Attack Table:" +
                (showmisss ? Environment.NewLine + "- " + misses.ToString("000.00") + " : " + missesPerc.ToString("00.00%") + " : Missed " : "") +
                (showdodge ? Environment.NewLine + "- " + dodges.ToString("000.00") + " : " + dodgesPerc.ToString("00.00%") + " : Dodged " : "") +
                (showparry ? Environment.NewLine + "- " + parrys.ToString("000.00") + " : " + parrysPerc.ToString("00.00%") + " : Parried " : "") +
                (showblock ? Environment.NewLine + "- " + blocks.ToString("000.00") + " : " + blocksPerc.ToString("00.00%") + " : Blocked " : "") +
                (showcrits ? Environment.NewLine + "- " + crits.ToString("000.00") + " : " + critsPerc.ToString("00.00%") + " : Crit " : "") +
                             Environment.NewLine + "- " + glncs.ToString("000.00") + " : " + glncsPerc.ToString("00.00%") + " : Glanced " +
                             Environment.NewLine + "- " + hits.ToString("000.00") + " : " + hitsPerc.ToString("00.00%") + " : Hit " +
                    Environment.NewLine +
                    //Environment.NewLine + "Damage per Blocked|Hit|Crit: x|x|x" +
                    Environment.NewLine + "Targets Hit: " + AvgTargets.ToString("0.00") +
                    Environment.NewLine + "DPS: " + (ttldpsOH > 0 ? ttldpsOH.ToString("0.00") : "None") +
                    Environment.NewLine + "Percentage of Total DPS: " + (ttldpsOH > 0 ? (ttldpsOH / ttldps).ToString("00.00%") : "None");
            }
            return tooltip;
        }
    }

    // Templated Base Classes
    public abstract class Ability
    {
        // Constructors
        public Ability()
        {
            // Character related
            Char = null;
            StatS = null;
            combatFactors = null;
            MHAtkTable = null;
            OHAtkTable = null;
            Whiteattacks = null;
            CalcOpts = null;
            BossOpts = null;
            // Ability Related
            Name = "Invalid";
            Description = "Invalid";
            ReqTalent = false;
            CanBeDodged = true;
            CanBeParried = true;
            CanBeBlocked = true;
            CanCrit = true;
            Talent2ChksValue = 0;
            AbilIterater = -1;
            ReqMeleeWeap = false;
            ReqMeleeRange = false;
            ReqMultiTargs = false;
            Targets = 1f;
            MaxRange = 5f; // In Yards 
            Cd = -1f; // In Seconds
            Duration = 0; // In Seconds
            RageCost = 0;
            CastTime = -1f; // In Seconds
            GCDTime = 1.5f; // default GCD size
            StanceOkFury = false;
            StanceOkArms = false;
            StanceOkDef = false;
            UseReact = false;
            DamageBase = 0f;
            DamageBonus = 1f;
            HealingBase = 0f;
            HealingBonus = 1f;
            BonusCritChance = 0.00f;
            UseSpellHit = false;
            UseHitTable = true;
            validatedSet = false;
            SwingsOffHand = false;
            SwingsPerActivate = 1f;
            UsesGCD = true;
        }
        public static Ability NULL = new NullAbility();
        #region Variables
        public int AbilIterater;
        #endregion
        #region Get/Set
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        protected bool ReqTalent { get; set; }
        protected int Talent2ChksValue { get; set; }
        public bool ReqMeleeWeap { get; set; }
        public bool ReqMeleeRange { get; set; }
        protected bool ReqMultiTargs { get; set; }
        private float _AvgTargets = -1f;
        public float AvgTargets {
            get {
                //float extraTargetsHit = Math.Min(CalcOpts.MultipleTargetsMax, TARGETS) - 1f;
                if (_AvgTargets == -1f)
                {
#if RAWR3 || SILVERLIGHT
                    //_AvgTargets = 1f + (BossOpts.MultiTargs ? StatS.BonusTargets + (float)BossOpts.MultiTargsPerc * (Math.Min((float)BossOpts.MaxNumTargets, Targets) - 1f) : 0f);
                    if (BossOpts.MultiTargs && BossOpts.Targets != null && BossOpts.Targets.Count > 0)
                    {
                        float value = 0;
                        foreach (TargetGroup tg in BossOpts.Targets)
                        {
                            if (tg.Frequency <= 0 || tg.Chance <= 0) continue; // bad one, skip it
                            float upTime = (tg.Frequency / BossOpts.BerserkTimer * (tg.Duration / 1000f) * tg.Chance) / BossOpts.BerserkTimer;
                            value += (Math.Max(10, tg.NumTargs - (tg.NearBoss ? 0 : 1) + StatS.BonusTargets)) * upTime;
                        }
                        _AvgTargets = 1f + value;
                    } else { _AvgTargets = 1f; }
#else
                    _AvgTargets = 1f + (CalcOpts.MultipleTargets ? StatS.BonusTargets + CalcOpts.MultipleTargetsPerc / 100f * (Math.Min(CalcOpts.MultipleTargetsMax, Targets) - 1f) : 0f);
#endif
                }
                return _AvgTargets;
            }
        }
        public float Targets { get; protected set; }
        public bool CanBeDodged { get; protected set; }
        public bool CanBeParried { get; protected set; }
        public bool CanBeBlocked { get; protected set; }
        public bool CanCrit { get; protected set; }
        public float MinRange { get; protected set; } // In Yards 
        public float MaxRange { get; protected set; } // In Yards
        public float Cd { get; set; }
        public float Duration { get; protected set; } // In Seconds
        public float RageCost { get; protected set; }
        protected float _cachedCastTime = -1f;
        public float CastTime // In Seconds
        {
            get { return _cachedCastTime; }
            protected set {
                _cachedCastTime = value;
                _cachedUseTime = (CalcOpts == null ? 0f : CalcOpts.Latency + (UseReact ? CalcOpts.React / 1000f : CalcOpts.AllowedReact))
                    + Math.Min(Math.Max(1.5f, value), _cachedGCDTime);
            }
        }
        protected float _cachedGCDTime = 1.5f;
        public float GCDTime // In Seconds
        {
            get { return _cachedGCDTime; }
            protected set
            {
                _cachedGCDTime = value;
                _cachedUseTime = (CalcOpts != null ? CalcOpts.Latency + (UseReact ? CalcOpts.React / 1000f : CalcOpts.AllowedReact) : 0f)
                    + Math.Min(Math.Max(1.5f, _cachedCastTime), value);
            }
        }
        protected float _cachedUseTime = 0;
        public float UseTime { get { return _cachedUseTime; } }
        /// <summary>Base Damage Value (500 = 500.00 Damage)</summary>
        public float DamageBase { get; set; }
        /// <summary>Percentage Based Damage Bonus (1.5 = 150% damage)</summary>
        protected float DamageBonus { get; set; }
        protected float HealingBase { get; set; }
        protected float HealingBonus { get; set; }
        public float BonusCritChance { get; set; }
        protected bool StanceOkFury { get; set; }
        protected bool StanceOkArms { get; set; }
        protected bool StanceOkDef { get; set; }
        protected bool UseReact { get; set; }
        protected Character Char { get; set; }
        protected WarriorTalents Talents { get { return Char.WarriorTalents; } }
        //protected WarriorTalentsCata TalentsCata { get { return Char.WarriorTalentsCata; } }
        protected Stats StatS { get; set; }
        protected CombatFactors combatFactors { get; set; }
        public virtual CombatTable MHAtkTable { get; protected set; }
        public virtual CombatTable OHAtkTable { get; protected set; }
        public WhiteAttacks Whiteattacks { get; protected set; }
        protected CalculationOptionsDPSWarr CalcOpts { get; set; }
        protected BossOptions BossOpts { get; set; }
        public virtual float RageUseOverDur { get { return (!Validated ? 0f : Activates * RageCost); } }
        public bool SwingsOffHand { get; protected set; }
        public float SwingsPerActivate { get; protected set; }
        protected float FightDuration {
            get {
                return
#if RAWR3 || SILVERLIGHT
                    BossOpts.BerserkTimer;
#else
                    CalcOpts.Duration;
#endif
            }
        }
        protected bool UseSpellHit { get; set; }
        protected bool UseHitTable { get; set; }
        public bool isMaint { get; protected set; }
        public bool UsesGCD { get; protected set; }
        private bool validatedSet = false;
        public virtual bool Validated { get { return validatedSet; } }
        private bool setValidation() {
            if (AbilIterater != -1 && !CalcOpts.Maintenance[AbilIterater])
            {
                validatedSet = false;
            }
            else if (ReqTalent && Talent2ChksValue < 1)
            {
                validatedSet = false;
            }
            else if (ReqMeleeWeap && (Char.MainHand == null || Char.MainHand.MaxDamage <= 0))
            {
                validatedSet = false;
            }
#if RAWR3 || SILVERLIGHT
                else if (ReqMultiTargs && (!BossOpts.MultiTargs || BossOpts.Targets == null || BossOpts.Targets.Count <= 0))
#else
            else if (ReqMultiTargs && (!CalcOpts.MultipleTargets || CalcOpts.MultipleTargetsPerc == 0))
#endif
            {
                validatedSet = false;
            }
            else if ((combatFactors.FuryStance && !StanceOkFury)
                || (!combatFactors.FuryStance && !StanceOkArms))
            {
                validatedSet = false;
            }
            else validatedSet = true;

            return validatedSet;
        }
        /// <summary>Number of times it can possibly be activated (# times actually used may be less or same).</summary>
        public virtual float Activates { get { return !Validated ? 0f : ActivatesOverride; } }
        /// <summary>
        /// Number of times it can possibly be activated (# times actually used may
        /// be less or same). This one does not check for stance/weapon info, etc.
        /// </summary>
        protected virtual float ActivatesOverride
        {
            get
            {
                float LatentGCD = 1.5f + CalcOpts.Latency + (UseReact ? CalcOpts.React / 1000f : CalcOpts.AllowedReact);
                float GCDPerc = LatentGCD / ((Duration > Cd ? Duration : Cd) + CalcOpts.Latency + (UseReact ? CalcOpts.React / 1000f : CalcOpts.AllowedReact));
                //float Every = LatentGCD / GCDPerc;
                if (RageCost > 0f)
                {
                    /*float rageSlip = (float)Math.Pow(Whiteattacks.MHAtkTable.AnyNotLand, Whiteattacks.AvoidanceStreak * Every);
                    float rageSlip2 = Whiteattacks.MHAtkTable.AnyNotLand / Every / Whiteattacks.AvoidanceStreak * RageCost / Whiteattacks.MHSwingRage;
                    float ret = FightDuration / Every * (1f - rageSlip);
                    return ret;*/
                    return Math.Max(0f, FightDuration / (LatentGCD / GCDPerc) * (1f - Whiteattacks.RageSlip(LatentGCD / GCDPerc, RageCost)));
                }
                else return FightDuration / (LatentGCD / GCDPerc);
                /*double test = Math.Pow((double)Whiteattacks.MHAtkTable.AnyNotLand, (double)Whiteattacks.AvoidanceStreak * Every);
                return Math.Max(0f, FightDuration / Every * (1f - Whiteattacks.AvoidanceStreak));*/
            }
        }
        protected float Healing { get { return !Validated ? 0f : HealingBase * HealingBonus; } }
        protected float HealingOnUse { get { return Healing * combatFactors.HealthBonus; } }
        //protected float AvgHealingOnUse { get { return HealingOnUse * Activates; } }
        protected float Damage { get { return !Validated ? 0f : DamageOverride; } }
        public virtual float DamageOverride { get { return Math.Max(0f, DamageBase * DamageBonus * AvgTargets); } }
        public float DamageOnUse { get { return (Validated ? DamageOnUseOverride : 0f); } }
        public virtual float DamageOnUseOverride
        {
            get
            {
                float dmg = Damage; // Base Damage
                dmg *= combatFactors.DamageBonus; // Global Damage Bonuses
                dmg *= combatFactors.DamageReduction; // Global Damage Penalties

                // Work the Attack Table
                float dmgDrop = (1f
                    - MHAtkTable.Miss   // no damage when being missed
                    - MHAtkTable.Dodge  // no damage when being dodged
                    - MHAtkTable.Parry  // no damage when being parried
                    - MHAtkTable.Glance // glancing handled below
                    - MHAtkTable.Block  // blocked handled below
                    - MHAtkTable.Crit); // crits   handled below

                float dmgGlance = dmg * MHAtkTable.Glance * combatFactors.ReducWhGlancedDmg;//Partial Damage when glancing, this doesn't actually do anything since glance is always 0
                float dmgBlock = dmg * MHAtkTable.Block * combatFactors.ReducYwBlockedDmg;//Partial damage when blocked
                float dmgCrit = dmg * MHAtkTable.Crit * (1f + combatFactors.BonusYellowCritDmg);//Bonus   Damage when critting

                dmg *= dmgDrop;

                dmg += /*dmgGlance +*/ dmgBlock + dmgCrit;

                return dmg;
            }
        }
        #endregion
        #region Functions
        protected void Initialize()
        {
            if (!UseSpellHit && UseHitTable && CanBeDodged && CanCrit && BonusCritChance == 0f)
            {
                MHAtkTable = combatFactors.AttackTableBasicMH;
                OHAtkTable = combatFactors.AttackTableBasicOH;
            }
            else
            {
                MHAtkTable = new AttackTable(Char, StatS, combatFactors, CalcOpts, BossOpts, this, true, UseSpellHit, !UseHitTable);
                OHAtkTable = new AttackTable(Char, StatS, combatFactors, CalcOpts, BossOpts, this, false, UseSpellHit, !UseHitTable);
            }
            setValidation();
        }
        public virtual float GetRageUseOverDur(float acts)
        {
            if (!Validated) { return 0f; }
            return acts * RageCost;
        }
        public float GetHealing() { if (!Validated) { return 0f; } return 0f; }
        public float GetAvgDamageOnUse(float acts)
        {
            if (!Validated) { return 0f; }
            return DamageOnUse * acts;
        }
        public virtual float GetDPS(float acts)
        {
            if (!Validated) { return 0f; }
            //float adou = GetAvgDamageOnUse(acts);
            return GetAvgDamageOnUse(acts) / FightDuration;
        }
        public float GetDPS(float acts, float perc)
        {
            if (!Validated) { return 0f; }
            //float adou = GetAvgDamageOnUse(acts);
            return GetAvgDamageOnUse(acts) / (FightDuration * perc);
        }
        public float GetAvgHealingOnUse(float acts)
        {
            if (!Validated) { return 0f; }
            return HealingOnUse * acts;
        }
        public float GetHPS(float acts)
        {
            if (!Validated) { return 0f; }
            //float adou = GetAvgHealingOnUse(acts);
            return GetAvgHealingOnUse(acts) / FightDuration;
        }
        //public virtual float ContainCritValue_MH { get { return Math.Min(1f, combatFactors._c_mhycrit + BonusCritChance); } }
        //public virtual float ContainCritValue_OH { get { return Math.Min(1f, combatFactors._c_ohycrit + BonusCritChance); } }
        /*public virtual float ContainCritValue(bool IsMH) {
            //float BaseCrit = IsMH ? combatFactors._c_mhycrit : combatFactors._c_ohycrit;
            return Math.Min(1f, (IsMH ? combatFactors._c_mhycrit : combatFactors._c_ohycrit) + BonusCritChance);
        }*/
        protected float GetXActs(AttackTableSelector i, float acts)
        {
            float retVal = 0f;
            switch (i)
            {
                case AttackTableSelector.Missed: { retVal = acts * MHAtkTable.Miss; break; }
                case AttackTableSelector.Dodged: { retVal = acts * MHAtkTable.Dodge; break; }
                case AttackTableSelector.Parried: { retVal = acts * MHAtkTable.Parry; break; }
                case AttackTableSelector.Blocked: { retVal = acts * MHAtkTable.Block; break; }
                case AttackTableSelector.Glance: { retVal = acts * MHAtkTable.Glance; break; }
                case AttackTableSelector.Crit: { retVal = acts * MHAtkTable.Crit; break; }
                case AttackTableSelector.Hit: { retVal = acts * MHAtkTable.Hit; break; }
                default: { break; }
            }
            return retVal;
        }
        public virtual string GenTooltip(float acts, float ttldpsperc)
        {
            float misses = GetXActs(AttackTableSelector.Missed, acts), missesPerc = (acts == 0f ? 0f : misses / acts);
            float dodges = GetXActs(AttackTableSelector.Dodged, acts), dodgesPerc = (acts == 0f ? 0f : dodges / acts);
            float parrys = GetXActs(AttackTableSelector.Parried, acts), parrysPerc = (acts == 0f ? 0f : parrys / acts);
            float blocks = GetXActs(AttackTableSelector.Blocked, acts), blocksPerc = (acts == 0f ? 0f : blocks / acts);
            float crits = GetXActs(AttackTableSelector.Crit, acts), critsPerc = (acts == 0f ? 0f : crits / acts);
            float hits = GetXActs(AttackTableSelector.Hit, acts), hitsPerc = (acts == 0f ? 0f : hits / acts);

            bool showmisss = misses > 0f;
            bool showdodge = CanBeDodged && dodges > 0f;
            bool showparry = CanBeParried && parrys > 0f;
            bool showblock = CanBeBlocked && blocks > 0f;
            bool showcrits = CanCrit && crits > 0f;

            string tooltip = "*" + Name +
                Environment.NewLine + "Cast Time: " + (CastTime != -1 ? CastTime.ToString() : "Instant")
                                    + ", CD: " + (Cd != -1 ? Cd.ToString() : "None")
                                    + ", RageCost: " + (RageCost != -1 ? RageCost.ToString() : "None") +
            Environment.NewLine + Environment.NewLine + acts.ToString("000.00") + " Activates over Attack Table:" +
            (showmisss ? Environment.NewLine + "- " + misses.ToString("000.00") + " : " + missesPerc.ToString("00.00%") + " : Missed " : "") +
            (showdodge ? Environment.NewLine + "- " + dodges.ToString("000.00") + " : " + dodgesPerc.ToString("00.00%") + " : Dodged " : "") +
            (showparry ? Environment.NewLine + "- " + parrys.ToString("000.00") + " : " + parrysPerc.ToString("00.00%") + " : Parried " : "") +
            (showblock ? Environment.NewLine + "- " + blocks.ToString("000.00") + " : " + blocksPerc.ToString("00.00%") + " : Blocked " : "") +
            (showcrits ? Environment.NewLine + "- " + crits.ToString("000.00") + " : " + critsPerc.ToString("00.00%") + " : Crit " : "") +
                         Environment.NewLine + "- " + hits.ToString("000.00") + " : " + hitsPerc.ToString("00.00%") + " : Hit " +
                Environment.NewLine +
                //Environment.NewLine + "Damage per Blocked|Hit|Crit: x|x|x" +
                Environment.NewLine + "Targets Hit: " + (Targets != -1 ? AvgTargets.ToString("0.00") : "None") +
                Environment.NewLine + "DPS: " + (GetDPS(acts) > 0 ? GetDPS(acts).ToString("0.00") : "None") +
                Environment.NewLine + "Percentage of Total DPS: " + (ttldpsperc > 0 ? ttldpsperc.ToString("00.00%") : "None");

            return tooltip;
        }
        #endregion
    }
    public class NullAbility : Ability
    {
        public override CombatTable MHAtkTable
        {
            get
            {
                return CombatTable.NULL;
            }
            protected set { ; }
        }
        public override CombatTable OHAtkTable
        {
            get
            {
                return CombatTable.NULL;
            }
            protected set { ; }
        }
        public override float RageUseOverDur { get { return 0; } }
        protected override float ActivatesOverride { get { return 0; } }
        public override float DamageOnUseOverride { get { return 0; } }
        public override float DamageOverride { get { return 0; } }
        public override string GenTooltip(float acts, float ttldpsperc) { return String.Empty; }
        public override float GetRageUseOverDur(float acts) { return 0; }
        public override bool Validated { get { return false; } }
        public override float Activates { get { return 0; } }
        public override float GetDPS(float acts) { return 0; }
    }
    public class OnAttack : Ability
    {
        // Constructors
        public OnAttack() { OverridesOverDur = 0f; UsesGCD = false; }
        // Get/Set
        public float OverridesOverDur { get; set; }
        public virtual float FullRageCost { get { return RageCost + Whiteattacks.MHSwingRage - Whiteattacks.MHUWProcValue * MHAtkTable.AnyLand; } }
        // Functions
        protected override float ActivatesOverride
        {
            get
            {
                if (!Validated || OverridesOverDur <= 0f) { return 0f; }
                //return Acts * (1f - Whiteattacks.AvoidanceStreak);
                return OverridesOverDur * (1f - Whiteattacks.RageSlip(FightDuration / OverridesOverDur, RageCost));
            }
        }
        public float DPS { get { return DamageOnUse * ActivatesOverride / FightDuration; } }
    };
    public class DoT : Ability
    {
        // Constructors
        public DoT() { }
        // Variables
        public float TimeBtwnTicks { get; set; } // In Seconds
        // Functions
        public virtual float TickSize { get { return 0f; } }
        public virtual float TTLTickingTime { get { return Duration; } }
        public virtual float TickLength { get { return TimeBtwnTicks; } }
        public virtual float NumTicks { get { return TTLTickingTime / TickLength; } }
        public virtual float DmgOverTickingTime { get { return TickSize * NumTicks; } }
        public virtual float GetDmgOverTickingTime(float acts) { return TickSize * (NumTicks * acts); }
        public override float GetDPS(float acts)
        {
            return GetDmgOverTickingTime(acts) / FightDuration;
        }
        public virtual float DPS { get { return TickSize / TickLength; } }
    }
    public class BuffEffect : Ability
    {
        // Constructors
        public BuffEffect()
        {
            isMaint = true;
            Effect = null;
            Effect2 = null;
        }
        // Variables
        protected float addMisses;
        protected float addDodges;
        protected float addParrys;
        // Get/Set
        public SpecialEffect Effect { get; set; }
        public SpecialEffect Effect2 { get; set; }
        // Functions
        public virtual Stats AverageStats
        {
            get
            {
                if (!Validated) { return new Stats(); }
                Stats bonus = (Effect == null) ? new Stats() { AttackPower = 0f, } : Effect.GetAverageStats(0f, MHAtkTable.Hit + MHAtkTable.Crit, Whiteattacks.MhEffectiveSpeed, FightDuration);
                bonus += (Effect2 == null) ? new Stats() { AttackPower = 0f, } : Effect2.GetAverageStats(0f, MHAtkTable.Hit + MHAtkTable.Crit, Whiteattacks.MhEffectiveSpeed, FightDuration);
                return bonus;
            }
        }
    }
    #endregion

    #region Unused
    public class VictoryRush : Ability
    {
        /// <summary>
        /// Instant, No Cd, No Rage, Melee Range, (Battle, Zerker)
        /// Instant attack the target causing 1424 damage. Can only be used within 25 sec after you
        /// kill an enemy that yields experience or honor. Damage is based on your attack power.
        /// </summary>
        /// <TalentsAffecting></TalentsAffecting>
        /// <GlyphsAffecting>
        /// Glyph of Victory Rush [+30% Crit Chance @ targs >70% HP]
        /// Glyph of Enduring Victory [+5 sec to length before ability wears off]
        /// </GlyphsAffecting>
    }
    public class HeroicThrow : Ability
    {
        /// <summary>
        /// Instant, 1 min Cd, 30 yd, Melee Weapon (Any)
        /// Throws your weapon at the enemy causing 1595 dmg (based upon attack power). This ability
        /// causes high threat.
        /// </summary>
        /// <TalentsAffecting></TalentsAffecting>
        /// <GlyphsAffecting></GlyphsAffecting>
        ///  - (Talents.FocusedRage * 1f)
    }
    #endregion
}
