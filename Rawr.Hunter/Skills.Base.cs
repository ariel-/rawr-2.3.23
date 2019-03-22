using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Hunter.Skills
{
    public enum AttackTableSelector { Missed = 0, /*Dodged, Parried, Blocked,*/ Crit, /*Glance,*/ Hit }

    public class WhiteAttacks
    {
        // Constructors
        public WhiteAttacks(Character character, Stats stats, CombatFactors cf, CalculationOptionsHunter calcOpts, BossOptions bossOpts)
        {
            Char = character;
            StatS = stats;
            Talents = Char.HunterTalents == null ? new HunterTalents() : Char.HunterTalents;
            combatFactors = cf;
            CalcOpts = calcOpts;
            BossOpts = bossOpts;
            RWAtkTable = new AttackTable(Char, StatS, combatFactors, calcOpts, false, false);
#if RAWR3 || SILVERLIGHT
            FightDuration = BossOpts.BerserkTimer;
#else
            FightDuration = CalcOpts.Duration;
#endif
            //
            Targets = 1f;
            HSOverridesOverDur = 0f;
            CLOverridesOverDur = 0f;
            Steady_Freq = 0f;
        }
        public void InvalidateCache()
        {
            _RwDamageOnUse = -1f;
        }
        #region Variables
        private readonly Character Char;
        private Stats StatS;
        private readonly HunterTalents Talents;
        private readonly CombatFactors combatFactors;
        private CalculationOptionsHunter CalcOpts;
        private BossOptions BossOpts;
        private float TARGETS;
        public AttackTable RWAtkTable;
        private float OVDOVERDUR_HS;
        private float OVDOVERDUR_CL;
        private float FightDuration;
        private float Targets { get { return TARGETS; } set { TARGETS = value; } }
        private float AvgTargets {
            get {
#if RAWR3 || SIVLERLIGHT
                if (BossOpts.MultiTargs)
#else
                if (CalcOpts.MultipleTargets)
#endif
                {
                    //float extraTargetsHit = Math.Min(CalcOpts.MultipleTargetsMax, TARGETS) - 1f;
                    return 1f +
                        //(Math.Min(CalcOpts.MultipleTargetsMax, TARGETS) - 1f) *
#if RAWR3 || SIVLERLIGHT
                        (BossOpts.MultiTargsTime / BossOpts.BerserkTimer)  + StatS.BonusTargets;
#else
                        CalcOpts.MultipleTargetsPerc / 100f + StatS.BonusTargets;
#endif
                }
                else { return 1f; }
            }
        }
        // Get/Set
        public float HSOverridesOverDur { get { return OVDOVERDUR_HS; } set { OVDOVERDUR_HS = value; } }
        public float CLOverridesOverDur { get { return OVDOVERDUR_CL; } set { OVDOVERDUR_CL = value; } }
        public float Steady_Freq;
        #endregion
        // bah
        private float SlamFreqSpdMod { get { return 0f; } }// (Slam_Freq == 0f ? 0f : ((1.5f - (0.5f * Talents.ImprovedSlam)) * (Slam_Freq / FightDuration))); } }
        // Main Hand
        public float RwEffectiveSpeed { get { return combatFactors.RWSpeed + SlamFreqSpdMod; } }
        public float RwDamage
        {
            get
            {
                //float DamageBase = combatFactors.AvgMhWeaponDmgUnhasted;
                //float DamageBonus = 1f + 0f;
                return combatFactors.AvgRwWeaponDmgUnhasted * AvgTargets;
            }
        }
        private float _RwDamageOnUse = -1f;
        public float RwDamageOnUse
        {
            get
            {
                if (_RwDamageOnUse == -1f)
                {
                    float dmg = RwDamage;                  // Base Damage
                    dmg *= combatFactors.DamageBonus;      // Global Damage Bonuses
                    dmg *= combatFactors.DamageReduction;  // Global Damage Penalties

                    // Work the Attack Table
                    float dmgDrop = (1f
                        - RWAtkTable.Miss   // no damage when being missed
                        - RWAtkTable.Dodge  // no damage when being dodged
                        - RWAtkTable.Parry  // no damage when being parried
                        - RWAtkTable.Glance // glancing handled below
                        - RWAtkTable.Block  // blocked handled below
                        - RWAtkTable.Crit); // crits   handled below

                    float dmgGlance = dmg * RWAtkTable.Glance * combatFactors.ReducWhGlancedDmg;//Partial Damage when glancing
                    float dmgBlock = dmg * RWAtkTable.Block * combatFactors.ReducWhBlockedDmg;//Partial damage when blocked
                    float dmgCrit = dmg * RWAtkTable.Crit * (1f + combatFactors.BonusWhiteCritDmg);//Bonus Damage when critting

                    dmg *= dmgDrop;

                    dmg += dmgGlance + dmgBlock + dmgCrit;

                    _RwDamageOnUse = dmg;
                }
                return _RwDamageOnUse;
            }
        }
        public float AvgRwDamageOnUse { get { return RwDamageOnUse * RwActivates; } }
        public float RwActivates {
            get {
                if (RwEffectiveSpeed != 0)
                    return FightDuration / RwEffectiveSpeed - HSOverridesOverDur - CLOverridesOverDur;
                else return 0f;
            }
        }
        public float RwActivatesNoHS {
            get {
                if (RwEffectiveSpeed != 0)
                    return FightDuration / RwEffectiveSpeed;
                else return 0f;
            }
        }
        public float RwDPS { get { return AvgRwDamageOnUse / FightDuration; } }
        // Attacks Over Fight Duration
        public float LandedAtksOverDur { get { return LandedAtksOverDurRw; } }
        public float LandedAtksOverDurRw { get { return RwActivates * RWAtkTable.AnyLand; } }
        private float CriticalAtksOverDur { get { return CriticalAtksOverDurRW; } }
        public float CriticalAtksOverDurRW { get { return RwActivates * RWAtkTable.Crit; } }
        // Other
        public float ManaSlip(float abilInterval, float manaCost)
        {
            //float whiteAtkInterval = (MhActivates + OhActivates) / FightDuration;
            //return MHAtkTable.AnyNotLand / abilInterval / whiteAtkInterval * manaCost / MHSwingMana;
            //float whiteMod = (MhActivates * MHSwingMana + (combatFactors.useOH ? OhActivates * OHSwingMana : 0f)) / FightDuration;
            if (RwActivates <= 0f) { return 0f; }
            return (RWAtkTable.Miss * manaCost) / (abilInterval * ((RwActivates /* * (MHSwingMana + MHUWProcValue)*/) / FightDuration));
        }
        public virtual float GetXActs(AttackTableSelector i, float acts)
        {
            AttackTable table = RWAtkTable;
            float retVal = 0f;
            switch (i)
            {
                case AttackTableSelector.Missed:  { retVal = acts * table.Miss;   break; }
                //case AttackTableSelector.Dodged:  { retVal = acts * table.Dodge;  break; }
                //case AttackTableSelector.Parried: { retVal = acts * table.Parry;  break; }
                //case AttackTableSelector.Blocked: { retVal = acts * table.Block;  break; }
                case AttackTableSelector.Crit:    { retVal = acts * table.Crit;   break; }
                //case AttackTableSelector.Glance:  { retVal = acts * table.Glance; break; }
                case AttackTableSelector.Hit:     { retVal = acts * table.Hit;    break; }
                default: { break; }
            }
            return retVal;
        }
        public virtual string GenTooltip(float ttldpsRW, /*float ttldpsOH,*/ float ttldps)
        {
            // ==== MAIN HAND ====
            float acts = RwActivates;
            float misses = GetXActs(AttackTableSelector.Missed, acts), missesPerc = (acts == 0f ? 0f : misses / acts);
            //float dodges = GetXActs(AttackTableSelector.Dodged, acts), dodgesPerc = (acts == 0f ? 0f : dodges / acts);
            //float parrys = GetXActs(AttackTableSelector.Parried, acts), parrysPerc = (acts == 0f ? 0f : parrys / acts);
            //float blocks = GetXActs(AttackTableSelector.Blocked, acts), blocksPerc = (acts == 0f ? 0f : blocks / acts);
            float crits = GetXActs(AttackTableSelector.Crit, acts), critsPerc = (acts == 0f ? 0f : crits / acts);
            //float glncs = GetXActs(AttackTableSelector.Glance, acts), glncsPerc = (acts == 0f ? 0f : glncs / acts);
            float hits = GetXActs(AttackTableSelector.Hit, acts), hitsPerc = (acts == 0f ? 0f : hits / acts);

            bool showmisss = misses > 0f;
            //bool showdodge = dodges > 0f;
            //bool showparry = parrys > 0f;
            //bool showblock = blocks > 0f;
            bool showcrits = crits > 0f;

            string tooltip = "*" + "White Damage (Ranged Weapon)" +
                Environment.NewLine + "Cast Time: Instant"
                                    + ", CD: " + (RwEffectiveSpeed != -1 ? RwEffectiveSpeed.ToString("0.00") : "None")
                                    + //", Mana Generated: " + (MHSwingMana != -1 ? MHSwingMana.ToString("0.00") : "None") +
            Environment.NewLine + Environment.NewLine + acts.ToString("000.00") + " Activates over Attack Table:" +
            (showmisss ? Environment.NewLine + "- " + misses.ToString("000.00") + " : " + missesPerc.ToString("00.00%") + " : Missed " : "") +
            //(showdodge ? Environment.NewLine + "- " + dodges.ToString("000.00") + " : " + dodgesPerc.ToString("00.00%") + " : Dodged " : "") +
            //(showparry ? Environment.NewLine + "- " + parrys.ToString("000.00") + " : " + parrysPerc.ToString("00.00%") + " : Parried " : "") +
            //(showblock ? Environment.NewLine + "- " + blocks.ToString("000.00") + " : " + blocksPerc.ToString("00.00%") + " : Blocked " : "") +
            (showcrits ? Environment.NewLine + "- " + crits.ToString("000.00") + " : " + critsPerc.ToString("00.00%") + " : Crit " : "") +
                         //Environment.NewLine + "- " + glncs.ToString("000.00") + " : " + glncsPerc.ToString("00.00%") + " : Glanced " +
                         Environment.NewLine + "- " + hits.ToString("000.00") + " : " + hitsPerc.ToString("00.00%") + " : Hit " +
                Environment.NewLine +
                //Environment.NewLine + "Damage per Blocked|Hit|Crit: x|x|x" +
                Environment.NewLine + "Targets Hit: " + (Targets != -1 ? Targets.ToString("0.00") : "None") +
                Environment.NewLine + "DPS: " + (ttldpsRW > 0 ? ttldpsRW.ToString("0.00") : "None") +
                Environment.NewLine + "Percentage of Total DPS: " + (ttldpsRW > 0 ? (ttldpsRW / ttldps).ToString("00.00%") : "None");

            return tooltip;
        }
    }

    // Templated Base Classes
    public class Ability
    {
        // Constructors
        public Ability()
        {
            // Character related
            Char = null;
            Talents = null;
            StatS = null;
            combatFactors = null;
            RWAtkTable = null;
            Whiteattacks = null;
            CalcOpts = null;
            // Ability Related
            Name = "Invalid";
            ReqTalent = false;
            CanBeDodged = true;
            CanBeParried = true;
            CanBeBlocked = true;
            CanCrit = true;
            Talent2ChksValue = 0;
            AbilIterater = -1;
            ReqRangedWeap = false;
            ReqSkillsRange = false;
            ReqMultiTargs = false;
            Targets = 1f;
            MaxRange = 5f; // In Yards 
            CD = -1f; // In Seconds
            Duration = -1f; // In Seconds
            ManaCost = -1f;
            ManaCostisPerc = false;
            CastTime = -1f; // In Seconds
            UseReact = false;
            DamageBase = 0f;
            DamageBonus = 1f;
            HealingBase = 0f;
            HealingBonus = 1f;
            BonusCritChance = 0.00f;
            UseSpellHit = false;
        }
        public static Ability NULL = new NullAbility();
        #region Variables
        private string NAME;
        private float DAMAGEBASE;
        private float DAMAGEBONUS;
        private float HEALINGBASE;
        private float HEALINGBONUS;
        private float BONUSCRITCHANCE;
        private bool CANBEDODGED;
        private bool CANBEPARRIED;
        private bool CANBEBLOCKED;
        private bool CANCRIT;
        private bool REQTALENT;
        private int TALENT2CHKSVALUE;
        private bool REQRANGEDWEAP;
        private bool REQSKILLSRANGE;
        private bool REQMULTITARGS;
        private float TARGETS;
        private float MINRANGE; // In Yards 
        private float MAXRANGE; // In Yards
        private float CD; // In Seconds
        private float DURATION; // In Seconds
        private float MANACOST;
        private bool MANACOSTISPERC;
        private float CASTTIME; // In Seconds
        private bool USEREACT; // if this ability is used as a proc effect
        private Character CHARACTER;
        private HunterTalents TALENTS;
        private Stats STATS;
        private CombatFactors COMBATFACTORS;
        private AttackTable RWATTACKTABLE;
        private WhiteAttacks WHITEATTACKS;
        private CalculationOptionsHunter CALCOPTS;
        private BossOptions BOSSOPTS;
        private bool USESPELLHIT = false;
        private bool USEHITTABLE = true;
        public int AbilIterater;
        #endregion
        #region Get/Set
        public string Name { get { return NAME; } set { NAME = value; } }
        protected bool ReqTalent { get { return REQTALENT; } set { REQTALENT = value; } }
        protected int Talent2ChksValue { get { return TALENT2CHKSVALUE; } set { TALENT2CHKSVALUE = value; } }
        protected bool ReqRangedWeap { get { return REQRANGEDWEAP; } set { REQRANGEDWEAP = value; } }
        protected bool ReqSkillsRange { get { return REQSKILLSRANGE; } set { REQSKILLSRANGE = value; } }
        protected bool ReqMultiTargs { get { return REQMULTITARGS; } set { REQMULTITARGS = value; } }
        private float _AvgTargets = -1f;
        public float AvgTargets
        {
            get
            {
                //float extraTargetsHit = Math.Min(CalcOpts.MultipleTargetsMax, TARGETS) - 1f;
                if (_AvgTargets == -1f)
                {
#if RAWR3 || SILVERLIGHT
                    _AvgTargets = 1f +
                       (BossOpts.MultiTargs ?
                           StatS.BonusTargets +
                           (BossOpts.MultiTargsTime / BossOpts.BerserkTimer) // *
                           //(Math.Min(CalcOpts.MultipleTargetsMax, TARGETS) - 1f)
                           : 0f);
#else
                    _AvgTargets = 1f +
                       (CalcOpts.MultipleTargets ?
                           StatS.BonusTargets +
                           CalcOpts.MultipleTargetsPerc / 100f // *
                        //(Math.Min(CalcOpts.MultipleTargetsMax, TARGETS) - 1f)
                           : 0f);
#endif
                }
                return _AvgTargets;
            }
        }
        protected float Targets { get { return TARGETS; } set { TARGETS = value; } }
        public bool CanBeDodged { get { return CANBEDODGED; } set { CANBEDODGED = value; } }
        public bool CanBeParried { get { return CANBEPARRIED; } set { CANBEPARRIED = value; } }
        public bool CanBeBlocked { get { return CANBEBLOCKED; } set { CANBEBLOCKED = value; } }
        public bool CanCrit { get { return CANCRIT; } set { CANCRIT = value; } }
        public float MinRange { get { return MINRANGE; } set { MINRANGE = value; } } // In Yards 
        public float MaxRange { get { return MAXRANGE; } set { MAXRANGE = value; } } // In Yards
        public float Cd
        { // In Seconds
            get { return CD; }
            set
            {
                /*float AssignedCD = value;
                float LatentGCD = 1.5f + CalcOpts.GetLatency();
                float CDs2Pass = 0f;
                for (int count = 0; count < FightDuration; count++) {
                    CDs2Pass = count * LatentGCD;
                    if (CDs2Pass >= AssignedCD) { break; }
                }
                CD = CDs2Pass;
                //*/
                CD = value;
            }
        }
        public float Duration { get { return DURATION; } set { DURATION = value; } } // In Seconds
        public float ManaCost { get { return MANACOST; } set { MANACOST = value; } }
        public bool ManaCostisPerc { get { return MANACOSTISPERC; } set { MANACOSTISPERC = value; } }
        public float CastTime { get { return CASTTIME; } set { CASTTIME = value; } } // In Seconds
        /// <summary>Base Damage Value (500 = 500.00 Damage)</summary>
        protected float DamageBase { get { return DAMAGEBASE; } set { DAMAGEBASE = value; } }
        /// <summary>Percentage Based Damage Bonus (1.5 = 150% damage)</summary>
        protected float DamageBonus { get { return DAMAGEBONUS; } set { DAMAGEBONUS = value; } }
        protected float HealingBase { get { return HEALINGBASE; } set { HEALINGBASE = value; } }
        protected float HealingBonus { get { return HEALINGBONUS; } set { HEALINGBONUS = value; } }
        protected float BonusCritChance { get { return BONUSCRITCHANCE; } set { BONUSCRITCHANCE = value; } }
        protected bool UseReact { get { return USEREACT; } set { USEREACT = value; } }
        protected Character Char
        {
            get { return CHARACTER; }
            set
            {
                CHARACTER = value;
                if (CHARACTER != null)
                {
                    Talents = CHARACTER.HunterTalents;
                    //StatS = CalculationsHunter.GetCharacterStats(CHARACTER, null);
                    //combatFactors = new CombatFactors(CHARACTER, StatS);
                    //Whiteattacks = Whiteattacks;
                    //CalcOpts = CHARACTER.CalculationOptions as CalculationOptionsHunter;
                }
                else
                {
                    Talents = null;
                    StatS = null;
                    //combatFactors = null;
                    RWAtkTable = null;
                    Whiteattacks = null;
                    CalcOpts = null;
                }
            }
        }
        protected HunterTalents Talents { get { return TALENTS; } set { TALENTS = value; } }
        protected Stats StatS { get { return STATS; } set { STATS = value; } }
        protected CombatFactors combatFactors { get { return COMBATFACTORS; } set { COMBATFACTORS = value; } }
        public virtual AttackTable RWAtkTable { get { return RWATTACKTABLE; } protected set { RWATTACKTABLE = value; } }
        public WhiteAttacks Whiteattacks { get { return WHITEATTACKS; } set { WHITEATTACKS = value; } }
        protected CalculationOptionsHunter CalcOpts { get { return CALCOPTS; } set { CALCOPTS = value; } }
        protected BossOptions BossOpts { get { return BOSSOPTS; } set { BOSSOPTS = value; } }
        public virtual float ManaUseOverDur { get { return (!Validated ? 0f : Activates * ManaCost); } }
#if RAWR3 || SILVERLIGHT
        protected float FightDuration { get { return BossOpts.BerserkTimer; } }
#else
        protected float FightDuration { get { return CalcOpts.Duration; } }
#endif
        protected bool UseSpellHit { get { return USESPELLHIT; } set { USESPELLHIT = value; } }
        protected bool UseHitTable { get { return USEHITTABLE; } set { USEHITTABLE = value; } }
        public bool isMaint { get; protected set; }
        public bool UsesGCD { get; protected set; }
        public float GCDTime { get; protected set; } // In Seconds
        public float SwingsPerActivate { get; protected set; }
        public float UseTime { get { return CalcOpts.Latency + (UseReact ? CalcOpts.React / 1000f : CalcOpts.AllowedReact) + Math.Min(Math.Max(1.5f, CastTime), GCDTime); } }
        private bool? validatedSet = null;
        public virtual bool Validated
        {
            get
            {
                if (validatedSet != null)
                {
                    return (validatedSet == true);
                }

                /*if (Char == null || Char.MainHand == null || CalcOpts == null || Talents == null) {
                    validatedSet = false;
                } else */
                if (ReqTalent && Talent2ChksValue < 1)
                {
                    validatedSet = false;
                }
                else if (ReqRangedWeap && (Char.MainHand == null || Char.MainHand.MaxDamage <= 0))
                {
                    validatedSet = false;
                }
#if RAW3 || SILVERLIGHT
                else if (ReqMultiTargs && (!BossOpts.MultiTargs || BossOpts.MultiTargsTime == 0))
#else
                else if (ReqMultiTargs && (!CalcOpts.MultipleTargets || CalcOpts.MultipleTargetsPerc == 0))
#endif
                {
                    validatedSet = false;
                }
                else validatedSet = true;
                /*if (
                    // Null crap is bad
                   (Char == null || Char.MainHand == null || CalcOpts == null || Talents == null) ||
                // Rotational Changes (Options Panel) (Arms Only right now)
                    (AbilIterater != -1 && !CalcOpts.Maintenance[AbilIterater]) ||
                // Talent Requirements
                    (ReqTalent && Talent2ChksValue < 1) ||
                // Need a weapon
                   (ReqMeleeWeap && Char.MainHand.MaxDamage <= 0) ||
                // Need Multiple Targets or it's useless
                   (ReqMultiTargs && (!CalcOpts.MultipleTargets || CalcOpts.MultipleTargetsPerc == 0)) ||
                // Proper Stance
                   ((CalcOpts.FuryStance && !StanceOkFury)
                    || (!CalcOpts.FuryStance && !StanceOkArms)
                  )) { return false; } */

                return (validatedSet == true);
            }
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
                float LatentGCD = 1.5f + CalcOpts.Latency + (UseReact ? CalcOpts.AllowedReact : 0f);
                float GCDPerc = LatentGCD / ((Duration > Cd ? Duration : Cd) + CalcOpts.Latency + (UseReact ? CalcOpts.AllowedReact : 0f));
                //float Every = LatentGCD / GCDPerc;
                if (ManaCost > 0f)
                {
                    /*float manaSlip = (float)Math.Pow(Whiteattacks.MHAtkTable.AnyNotLand, Whiteattacks.AvoidanceStreak * Every);
                    float manaSlip2 = Whiteattacks.MHAtkTable.AnyNotLand / Every / Whiteattacks.AvoidanceStreak * ManaCost / Whiteattacks.MHSwingMana;
                    float ret = FightDuration / Every * (1f - manaSlip);
                    return ret;*/
                    return Math.Max(0f, FightDuration / (LatentGCD / GCDPerc) * (1f - Whiteattacks.ManaSlip(LatentGCD / GCDPerc, ManaCost)));
                }
                else return FightDuration / (LatentGCD / GCDPerc);
                /*double test = Math.Pow((double)Whiteattacks.MHAtkTable.AnyNotLand, (double)Whiteattacks.AvoidanceStreak * Every);
                return Math.Max(0f, FightDuration / Every * (1f - Whiteattacks.AvoidanceStreak));*/
            }
        }
        protected virtual float Healing { get { return !Validated ? 0f : HealingBase * HealingBonus; } }
        protected virtual float HealingOnUse
        {
            get
            {
                return Healing * combatFactors.HealthBonus;
            }
        }
        protected virtual float AvgHealingOnUse { get { return HealingOnUse * Activates; } }
        protected virtual float HPS { get { return AvgHealingOnUse / FightDuration; } }
        protected virtual float Damage { get { return !Validated ? 0f : DamageOverride; } }
        public virtual float DamageOverride { get { return Math.Max(0f, DamageBase * DamageBonus * AvgTargets); } }
        public virtual float DamageOnUse
        {
            get
            {
                float dmg = Damage; // Base Damage
                dmg *= combatFactors.DamageBonus; // Global Damage Bonuses
                dmg *= combatFactors.DamageReduction; // Global Damage Penalties

                // Work the Attack Table
                float dmgDrop = (1f
                    - RWAtkTable.Miss   // no damage when being missed
                    - RWAtkTable.Dodge  // no damage when being dodged
                    - RWAtkTable.Parry  // no damage when being parried
                    - RWAtkTable.Glance // glancing handled below
                    - RWAtkTable.Block  // blocked handled below
                    - RWAtkTable.Crit); // crits   handled below

                float dmgGlance = dmg * RWAtkTable.Glance * combatFactors.ReducWhGlancedDmg;//Partial Damage when glancing, this doesn't actually do anything since glance is always 0
                float dmgBlock = dmg * RWAtkTable.Block * combatFactors.ReducYwBlockedDmg;//Partial damage when blocked
                float dmgCrit = dmg * RWAtkTable.Crit * (1f + combatFactors.BonusYellowCritDmg);//Bonus   Damage when critting

                dmg *= dmgDrop;

                dmg += /*dmgGlance +*/ dmgBlock + dmgCrit;

                return dmg;
            }
        }
        protected virtual float DamageOnUseOverride { get { return DamageOnUse; } }
        protected virtual float AvgDamageOnUse { get { return DamageOnUse * Activates; } }
        public virtual float DPS { get { return AvgDamageOnUse / FightDuration; } }
        #endregion
        #region Functions
        protected void Initialize()
        {
            if (!UseSpellHit && UseHitTable && CanBeDodged && CanCrit && BonusCritChance == 0f) {
                RWAtkTable = combatFactors.AttackTableBasicRW;
            } else {
                RWAtkTable = new AttackTable(Char, StatS, combatFactors, CalcOpts, this, UseSpellHit, !UseHitTable);
            }
        }
        public virtual float GetManaUseOverDur(float acts)
        {
            if (!Validated) { return 0f; }
            return acts * ManaCost;
        }
        public virtual float GetHealing() { if (!Validated) { return 0f; } return 0f; }
        public virtual float GetAvgDamageOnUse(float acts)
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
        public virtual float GetDPS(float acts, float perc)
        {
            if (!Validated) { return 0f; }
            //float adou = GetAvgDamageOnUse(acts);
            return GetAvgDamageOnUse(acts) / (FightDuration * perc);
        }
        public virtual float GetAvgHealingOnUse(float acts)
        {
            if (!Validated) { return 0f; }
            return HealingOnUse * acts;
        }
        public virtual float GetHPS(float acts)
        {
            if (!Validated) { return 0f; }
            //float adou = GetAvgHealingOnUse(acts);
            return GetAvgHealingOnUse(acts) / FightDuration;
        }
        public virtual float ContainCritValue_RW { get { return Math.Min(1f, combatFactors._c_rwycrit + BonusCritChance); } }
        /*public virtual float ContainCritValue(bool IsMH) {
            //float BaseCrit = IsMH ? combatFactors._c_mhycrit : combatFactors._c_ohycrit;
            return Math.Min(1f, (IsMH ? combatFactors._c_mhycrit : combatFactors._c_ohycrit) + BonusCritChance);
        }*/
        protected virtual float GetXActs(AttackTableSelector i, float acts)
        {
            float retVal = 0f;
            switch (i)
            {
                case AttackTableSelector.Missed: { retVal = acts * RWAtkTable.Miss; break; }
                //case AttackTableSelector.Dodged: { retVal = acts * RWAtkTable.Dodge; break; }
                //case AttackTableSelector.Parried: { retVal = acts * RWAtkTable.Parry; break; }
                //case AttackTableSelector.Blocked: { retVal = acts * RWAtkTable.Block; break; }
                //case AttackTableSelector.Glance: { retVal = acts * RWAtkTable.Glance; break; }
                case AttackTableSelector.Crit: { retVal = acts * RWAtkTable.Crit; break; }
                case AttackTableSelector.Hit: { retVal = acts * RWAtkTable.Hit; break; }
                default: { break; }
            }
            return retVal;
        }
        public virtual string GenTooltip(float acts, float ttldpsperc)
        {
            float misses = GetXActs(AttackTableSelector.Missed, acts), missesPerc = (acts == 0f ? 0f : misses / acts);
            //float dodges = GetXActs(AttackTableSelector.Dodged, acts), dodgesPerc = (acts == 0f ? 0f : dodges / acts);
            //float parrys = GetXActs(AttackTableSelector.Parried, acts), parrysPerc = (acts == 0f ? 0f : parrys / acts);
            //float blocks = GetXActs(AttackTableSelector.Blocked, acts), blocksPerc = (acts == 0f ? 0f : blocks / acts);
            float crits = GetXActs(AttackTableSelector.Crit, acts), critsPerc = (acts == 0f ? 0f : crits / acts);
            float hits = GetXActs(AttackTableSelector.Hit, acts), hitsPerc = (acts == 0f ? 0f : hits / acts);

            bool showmisss = misses > 0f;
            //bool showdodge = CanBeDodged && dodges > 0f;
            //bool showparry = CanBeParried && parrys > 0f;
            //bool showblock = CanBeBlocked && blocks > 0f;
            bool showcrits = CanCrit && crits > 0f;

            string tooltip = "*" + Name +
                Environment.NewLine + "Cast Time: " + (CastTime != -1 ? CastTime.ToString() : "Instant")
                                    + ", CD: " + (Cd != -1 ? Cd.ToString() : "None")
                                    + ", ManaCost: " + (ManaCost != -1 ? ManaCost.ToString() : "None") +
            Environment.NewLine + Environment.NewLine + acts.ToString("000.00") + " Activates over Attack Table:" +
            (showmisss ? Environment.NewLine + "- " + misses.ToString("000.00") + " : " + missesPerc.ToString("00.00%") + " : Missed " : "") +
            //(showdodge ? Environment.NewLine + "- " + dodges.ToString("000.00") + " : " + dodgesPerc.ToString("00.00%") + " : Dodged " : "") +
            //(showparry ? Environment.NewLine + "- " + parrys.ToString("000.00") + " : " + parrysPerc.ToString("00.00%") + " : Parried " : "") +
            //(showblock ? Environment.NewLine + "- " + blocks.ToString("000.00") + " : " + blocksPerc.ToString("00.00%") + " : Blocked " : "") +
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
        public override AttackTable RWAtkTable {
            get {
                return (AttackTable)CombatTable.NULL;
            }
            protected set { ; }
        }
        public override float ManaUseOverDur { get { return 0; } }
        protected override float ActivatesOverride { get { return 0; } }
        protected override float DamageOnUseOverride { get { return 0; } }
        public override float DamageOverride { get { return 0; } }
        public override string GenTooltip(float acts, float ttldpsperc) { return String.Empty; }
        public override float GetManaUseOverDur(float acts) { return 0; }
        public override bool Validated { get { return false; } }
        public override float Activates { get { return 0; } }
        public override float GetDPS(float acts) { return 0; }
    }
    public class OnAttack : Ability
    {
        // Constructors
        public OnAttack() { OverridesOverDur = 0f; }
        // Variables
        private float OVERRIDESOVERDUR;
        // Get/Set
        public float OverridesOverDur { get { return OVERRIDESOVERDUR; } set { OVERRIDESOVERDUR = value; } }
        public virtual float FullManaCost { get { return ManaCost /*+ Whiteattacks.MHSwingMana - Whiteattacks.MHUWProcValue * RWAtkTable.AnyLand*/; } }
        // Functions
        public override float Activates
        {
            get
            {
                if (!Validated || OverridesOverDur <= 0f) { return 0f; }
                //return Acts * (1f - Whiteattacks.AvoidanceStreak);
                return OverridesOverDur * (1f - Whiteattacks.ManaSlip(FightDuration / OverridesOverDur, ManaCost));
            }
        }
    };
    public class DoT : Ability
    {
        // Constructors
        public DoT() { }
        // Variables
        private float TIMEBTWNTICKS; // In Seconds
        // Get/Set
        public float TimeBtwnTicks { get { return TIMEBTWNTICKS; } set { TIMEBTWNTICKS = value; } } // In Seconds
        // Functions
        public virtual float TickSize { get { return 0f; } }
        public virtual float TTLTickingTime { get { return Duration; } }
        public virtual float TickLength { get { return TimeBtwnTicks; } }
        public virtual float NumTicks { get { return TTLTickingTime / TickLength; } }
        public virtual float DmgOverTickingTime { get { return TickSize * NumTicks; } }
        public virtual float GetDmgOverTickingTime(float acts) { return TickSize * (NumTicks * acts); }
        public override float GetDPS(float acts)
        {
            //float dmgonuse = TickSize;
            //float numticks = NumTicks * acts;
            return GetDmgOverTickingTime(acts) / FightDuration;
            //return result;
        }
        public override float DPS { get { return TickSize / TickLength; } }
    }
    public class BuffEffect : Ability
    {
        // Constructors
        public BuffEffect()
        {
            EFFECT = null;
            EFFECT2 = null;
        }
        // Variables
        private SpecialEffect EFFECT;
        private SpecialEffect EFFECT2;
        protected float addMisses;
        // Get/Set
        public SpecialEffect Effect { get { return EFFECT; } set { EFFECT = value; } }
        public SpecialEffect Effect2 { get { return EFFECT2; } set { EFFECT2 = value; } }
        // Functions
        public virtual Stats AverageStats
        {
            get
            {
                if (!Validated) { return new Stats(); }
                Stats bonus = (Effect == null) ? new Stats() { AttackPower = 0f, } : Effect.GetAverageStats(0f, RWAtkTable.Hit + RWAtkTable.Crit, Whiteattacks.RwEffectiveSpeed, FightDuration);
                bonus += (Effect2 == null) ? new Stats() { AttackPower = 0f, } : Effect2.GetAverageStats(0f, RWAtkTable.Hit + RWAtkTable.Crit, Whiteattacks.RwEffectiveSpeed, FightDuration);
                return bonus;
            }
        }
    }
}

