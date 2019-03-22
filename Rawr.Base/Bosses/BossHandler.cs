using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Rawr.Bosses;

namespace Rawr {
    #region Subclasses
    /// <summary>The role of the player, will allow certain lists to return filtered to things that affect said role</summary>
    public enum PLAYER_ROLES { ROLE_MainTank = 0, ROLE_OffTank, ROLE_TertiaryTank, ROLE_MeleeDPS, ROLE_RangedDPS, ROLE_Healer }
    /// <summary>Enumerator for creating a list of possible values for the Level box</summary>
    public enum POSSIBLE_LEVELS { LVLP0 = 80, LVLP1, LVLP2, LVLP3, }
    /// <summary>Enumerator for attack types, this mostly is for raid members that aren't being directly attacked to know when AoE damage is coming from the boss</summary>
    public enum ATTACK_TYPES { AT_MELEE = 0, AT_RANGED, AT_AOE, }
    /// <summary>A single Attack of various types</summary>
    public partial class Attack {
        /// <summary>The Name of the Attack</summary>
        public string Name;
        /// <summary>The type of damage done, use the ItemDamageType enumerator to select</summary>
        public ItemDamageType DamageType;
        /// <summary>This is so you can pull the Default Melee Attack more easily</summary>
        public bool IsTheDefaultMelee = false;
        /// <summary>The Unmitigated Damage per Hit for this attack, 5000f is 5,000 Raw Unmitigated Damage. When DamageIsPerc is true DamagePerHit = 0.75f; would be 75% of Player's Health</summary>
        public virtual float DamagePerHit { get; set; }
        /// <summary>When set to True, DamagePerHit will be seen as a Percentage. DamagePerHit = 0.75f; would be 75% of Player's Health</summary>
        public bool DamageIsPerc = false;
        /// <summary>The maximum number of party/raid members this attack can hit</summary>
        public float MaxNumTargets;
        /// <summary>The frequency of this attack (in seconds)</summary>
        public float AttackSpeed;
        /// <summary>The Attack Type (for AoE vs. single-target Melee/Ranged)</summary>
        public ATTACK_TYPES AttackType;
        /// <summary>If the attack Parry Haste's, then the attacks that are parried will reset the swing timer.</summary>
        public bool UseParryHaste = false;
        /// <summary>if the attack is part of a Dual Wield, there is an additional 20% chance to Miss<para>This should flag the Buff... sort of</para></summary>
        public bool IsDualWielding = false;
        #region Player Avoidance
        /// <summary>Returns True if any of the Avoidance types are true</summary>
        public bool Avoidable { get { return Missable || Dodgable || Parryable || Blockable; } }
        /// <summary>Can this attack Miss the player</summary>
        public bool Missable = true;
        /// <summary>Can this attack be Dodged by the player</summary>
        public bool Dodgable = true;
        /// <summary>Can this attack be Parried by the player</summary>
        public bool Parryable = true;
        /// <summary>Can this attack be Blocked by the player</summary>
        public bool Blockable = true;
        #endregion
        #region Player Targeting
        /// <summary>Returns True if anyone gets ignored, serves as a primary flag to run the ignorance system in your model</summary>
        public bool IgnoresSomeone { get { return IgnoresSomeTanks || IgnoresSomeDPS || IgnoresHealers; } }
        // Tanks
        public bool IgnoresAllTanks { get { return IgnoresMTank && IgnoresOTank && IgnoresTTank; } }
        public bool IgnoresSomeTanks { get { return IgnoresMTank || IgnoresOTank || IgnoresTTank; } }
        public bool IgnoresMTank = false;
        public bool IgnoresOTank = false;
        public bool IgnoresTTank = false;
        // Heals
        public bool IgnoresHealers = false;
        // DPS
        public bool IgnoresAllDPS { get { return IgnoresMeleeDPS && IgnoresRangedDPS; } }
        public bool IgnoresSomeDPS { get { return IgnoresMeleeDPS || IgnoresRangedDPS; } }
        public bool IgnoresMeleeDPS = false;
        public bool IgnoresRangedDPS = false;
        #endregion
        #region Player Negation
        public bool Interruptable = false;
        #endregion

        public override string ToString()
        {
            if (AttackSpeed <= 0) { return "None"; }
            return string.Format("Spd: {0:0.0#}sec D: {1:0}{2} #T: {3:0} {4}{5}",
                AttackSpeed,
                DamagePerHit, DamageIsPerc ? "%" : "",
                MaxNumTargets,
                UseParryHaste ? ": PH" : "",
                Name != "Dynamic" ? ": " + Name : "");
        }
    }
    /// <summary>A single Attack of type Damage over Time</summary>
    public partial class DoT : Attack {
        /// <summary>The initial damage of the dot, separate from the over time portion</summary>
        public override float DamagePerHit { get; set; }
        /// <summary>The over time damage of the dot, separate from the initial portion</summary>
        public float DamagePerTick;
        /// <summary>The total number of Ticks</summary>
        public float NumTicks;
        /// <summary>Interval of the ticks, 1 = 1 sec</summary>
        public float TickInterval;
        /// <summary>The full duration of the DoT, 2 sec Interval * 5 Ticks = 10 sec duration</summary>
        public float Duration { get { return TickInterval * NumTicks; } }
    }
    public class Impedance {
        #region Constructors
        public Impedance() {
            Frequency = 120f;
            Duration  = 5000f;
            Chance    = 1.00f;
            Breakable = true;
        }
        public Impedance(float f, float d, float c, bool b) {
            Frequency = f;
            Duration  = d;
            Chance    = c;
            Breakable = b;
        }
        public Impedance(Impedance i) {
            Impedance clone = (Impedance)i.MemberwiseClone();
            Frequency = clone.Frequency;
            Duration  = clone.Duration;
            Chance    = clone.Chance;
            Breakable = clone.Breakable;
        }
        #endregion
        #region Variables
        /// <summary>
        /// The amount of tim between activations of this Impedance, in sec
        /// <para>Eg- This Impedance occurs every 45 sec</para>
        /// <para>Frequency = 45f</para>
        /// </summary>
        public float Frequency;
        /// <summary>
        /// The amount of time spent in this state, in millisec
        /// <para>Eg- This Impedance lasts 4 seconds:</para>
        /// <para>Duration = 4f * 1000f</para>
        /// </summary>
        public float Duration;
        /// <summary>
        /// A Percentage, value range from 0% to 100% (0.00f to 1.00f)
        /// <para>Eg- An Impedance effects one random raid target:</para>
        /// <para>Chance = 1f / this.Max_Players</para>
        /// </summary>
        public float Chance;
        /// <summary>
        /// Flag which indicates whether the player can reduce or break the Duration of the Impedance
        /// <para>Eg- The player has to move for 4 seconds, movement can be broken by MovementSpeed bonus and Charge-like abilities:</para>
        /// <para>Breakable = true</para>
        /// <para>Eg- The player is stunned by Smite (Deadmines) for 4 seconds, this stun is scripted and cannot be broken:</para>
        /// <para>Breakable = false</para>
        /// </summary>
        public bool Breakable;
        #endregion
        #region Functions
        public override string ToString() {
            if (Frequency < 0) return "None";
            return string.Format("F: {0:0}s D: {1:0}ms C: {2:0.0%}{3}",
                Frequency, Duration, Chance, Breakable ? " : B" : "");
        }
        #endregion
    }
    public partial class TargetGroup
    {
        #region Constructors
        public TargetGroup() {
            Frequency = -1;
            Duration  = 20 * 1000f;
            Chance    = 1.00f;
            NumTargs  = 2;
            NearBoss  = false;
        }
        public TargetGroup(float f, float d, float c, float t, bool n) {
            Frequency = f;
            Duration  = d;
            Chance    = c;
            NumTargs  = t;
            NearBoss  = n;
        }
        public TargetGroup(TargetGroup i) {
            TargetGroup clone = (TargetGroup)i.MemberwiseClone();
            Frequency = clone.Frequency;
            Duration  = clone.Duration;
            Chance    = clone.Chance;
            NumTargs  = clone.NumTargs;
            NearBoss  = clone.NearBoss;
        }
        #endregion
        #region Variables
        /// <summary>In Seconds<para>Defaults to -1 as an 'invalid' flag</para></summary>
        public float Frequency = -1;
        /// <summary>In MilliSeconds (1/1000 of a second)<para>Defaults to 20 seconds</para></summary>
        public float Duration = 20 * 1000;
        /// <summary>Percentage, 0.50f = 50% Chance that this occurs<para>Defaults to and almost every time in usage this should be 1.00f=100%</para></summary>
        public float Chance = 1.00f;
        /// <summary>The Number of Targets in this Target Group<para>Defaults to 2</para></summary>
        public float NumTargs = 2;
        /// <summary>
        /// If the mobs are near boss then your MultiTargets calculations should also count the boss,
        /// otherwise this is a group that must be DPS'd by itself.
        /// <para>Defaults to false</para>
        /// </summary>
        public bool NearBoss = false;
        #endregion
        #region Functions
        public override string ToString()
        {
            if (Frequency <= 0) return "None";
            return string.Format("#T: {0:0.00} F: {1:0.0}s D: {2:0.00}s C: {3:0%}{4}",
                NumTargs, Frequency, Duration / 1000f, Chance, NearBoss ? " : NB" : "");
        }
        #endregion
    }
    #endregion

#if !RAWR3 && !SILVERLIGHT
    [Serializable]
#endif
    public partial class BossOptions : BossHandler
    {
        #region MyModelSupportsThis
        private static readonly Dictionary<string, bool> DefaultSupports = new Dictionary<string, bool>() {
            // Basics
            {"Level", true},
            {"Armor", true},
            {"Timers", true},
            {"Health", true},
            {"TimeSub35", true},
            {"TimeSub20", true},
            {"InBack_Melee", true},
            {"InBack_Ranged", true},
            {"RaidSize", true},
            // Offensive
            {"TargetGroups", true},
            {"Attacks", true},
            // Defensive
            {"Defensive", true},
            // Impedances
            {"Moves", true},
            {"Stuns", true},
            {"Fears", true},
            {"Roots", true},
            {"Disarms", true},
            {"Invulnerables", true}, // Not UI'd yet
        };
        protected static Dictionary<string, bool> DuplicateDefaultSupports() {
            Dictionary<string, bool> retVal = new Dictionary<string, bool>();
            foreach (string key in DefaultSupports.Keys)
            {
                retVal.Add(key, DefaultSupports[key]);
            }
            return retVal;
        }
        private static Dictionary<string, Dictionary<string, bool>> _MyModelSupportsThis = null;
        public static Dictionary<string, Dictionary<string, bool>> MyModelSupportsThis {
            get
            {
                if (_MyModelSupportsThis == null)
                {
                    _MyModelSupportsThis = new Dictionary<string, Dictionary<string, bool>>();
                    { // Melee DPS
                        Dictionary<string, bool> custom = DuplicateDefaultSupports();
                        custom["InBack_Ranged"] = false; // You aren't Ranged
                        _MyModelSupportsThis.Add("Cat", custom);
                        _MyModelSupportsThis.Add("DPSDK", custom);
                        _MyModelSupportsThis.Add("Enhance", custom);
                        _MyModelSupportsThis.Add("Rogue", custom);
                        _MyModelSupportsThis.Add("Retribution", custom);
                    }
                    { // Ranged DPS
                        Dictionary<string, bool> custom = DuplicateDefaultSupports();
                        custom["InBack_Melee"] = false; // You aren't melee
                        _MyModelSupportsThis.Add("Hunter", custom);
                        _MyModelSupportsThis.Add("Elemental", custom);
                        _MyModelSupportsThis.Add("Mage", custom);
                        _MyModelSupportsThis.Add("ShadowPriest", custom);
                        _MyModelSupportsThis.Add("Warlock", custom);
                    }
                    { // Tanks
                        Dictionary<string, bool> custom = DuplicateDefaultSupports();
                        custom["InBack_Melee"] = custom["InBack_Ranged"] = false; // The boss is focused on you
                        _MyModelSupportsThis.Add("Bear", custom);
                        _MyModelSupportsThis.Add("ProtWarr", custom);
                        _MyModelSupportsThis.Add("TankDK", custom);
                    }
                    { // Heals
                        Dictionary<string, bool> custom = DuplicateDefaultSupports();
                        custom["InBack_Melee"] = custom["InBack_Ranged"] = false; // Doesn't matter
                        custom["Level"] = false; // Your target isn't the boss
                        custom["Armor"] = false; // You don't damage anything, so there's nothing for the armor to mitigate
                        custom["Defensive"] = false; // Your target isn't the boss
                        custom["Invulnerables"] = false; // Your target isn't the boss
                        custom["TimeSub35"] = false; // No abilities tied to this
                        custom["TimeSub20"] = false; // No abilities tied to this
                        _MyModelSupportsThis.Add("HealPriest", custom);
                        _MyModelSupportsThis.Add("RestoSham", custom);
                        _MyModelSupportsThis.Add("Tree", custom);
                    }
                    #region DPSWarr
                    {
                        Dictionary<string, bool> custom = DuplicateDefaultSupports();
                        custom["InBack_Ranged"] = false; // Not Ranged
                        custom["TimeSub35"] = false; // No abilities tied to this
                        _MyModelSupportsThis.Add("DPSWarr", custom);
                    }
                    #endregion
                    #region ProtPaladin
                    {
                        // ProtPaladin needs a lot of work before it can support most of BossHandler
                        Dictionary<string, bool> custom = DuplicateDefaultSupports();
                        custom["Timers"] = false; // NYI
                        custom["Health"] = false; // NYI
                        custom["TimeSub35"] = false; // No abilities tied to this
                        custom["TimeSub20"] = false; // NYI
                        custom["InBack_Melee"] = custom["InBack_Ranged"] = false; // The boss is focused on you
                        custom["RaidSize"] = false; // NYI
                        custom["TargetGroups"] = false; // NYI
                        custom["Attacks"] = false; // NYI
                        custom["Defensive"] = false; // NYI
                        custom["Moves"] = false; // NYI
                        custom["Stuns"] = false; // NYI
                        custom["Fears"] = false; // NYI
                        custom["Roots"] = false; // NYI
                        custom["Disarms"] = false; // NYI
                        custom["Invulnerables"] = false; // NYI
                        _MyModelSupportsThis.Add("ProtPaladin", custom);
                    }
                    #endregion
                    #region Healadin
                    {
                        Dictionary<string, bool> custom = DuplicateDefaultSupports();
                        custom["Level"] = false; // Your target isn't the boss
                        custom["Armor"] = false; // You don't damage anything, so there's nothing for the armor to mitigate
                        custom["Health"] = false; // Your target isn't the boss
                        custom["TimeSub35"] = false; // No abilities tied to this
                        custom["TimeSub20"] = false; // No abilities tied to this
                        custom["InBack_Melee"] = false; // NYI
                        custom["InBack_Ranged"] = false; // You're not ranged
                        custom["RaidSize"] = false; // No abilities tied to this
                        custom["TargetGroups"] = false; // Your target isn't these groups
                        custom["Attacks"] = false; // We don't model damage taken by the player
                        custom["Defensive"] = false; // Your target isn't the boss
                        custom["Moves"] = false; // NYI
                        custom["Stuns"] = false; // NYI
                        custom["Fears"] = false; // NYI
                        custom["Roots"] = false; // NYI
                        custom["Disarms"] = false; // NYI
                        custom["Invulnerables"] = false; // Your target isn't the boss
                        _MyModelSupportsThis.Add("Healadin", custom);
                    }
                    #endregion
                    #region Moonkin
                    {
                        // Moonkin only needs to support a few features - we don't have any fancy executes or other cool stuff
                        Dictionary<string, bool> custom = DuplicateDefaultSupports();
                        custom["Health"] = false; // NYI
                        custom["TimeSub35"] = false; // Not applicable
                        custom["TimeSub20"] = false; // Not applicable
                        custom["InBack_Melee"] = custom["InBack_Ranged"] = false; // We're a caster, we don't care
                        custom["RaidSize"] = false; // NYI
                        custom["TargetGroups"] = false; // NYI
                        custom["Attacks"] = false; // NYI
                        custom["Defensive"] = false; // NYI
                        custom["Roots"] = false; // Not applicable
                        custom["Disarms"] = false; // Not applicable
                        _MyModelSupportsThis.Add("Moonkin", custom);
                    }
                    #endregion
                }
                return _MyModelSupportsThis;
            }
        }
        #endregion

        public BossOptions() { }
        public new BossOptions Clone() {
            BossOptions clone = (BossOptions)this.MemberwiseClone();
            return clone;
        }
        public BossOptions CloneThis(BossHandler toClone)
        {
            BossHandler clone = (BossHandler)toClone.Clone();
            // Info
            this.Name = clone.Name;
            this.Content = clone.Content;
            this.Instance = clone.Instance;
            this.Version = clone.Version;
            this.Comment = clone.Comment;
            // Basics
            this.Level = clone.Level;
            this.Armor = clone.Armor;
            this.Health = clone.Health;
            this.BerserkTimer = clone.BerserkTimer;
            this.SpeedKillTimer = clone.SpeedKillTimer;
            this.InBackPerc_Melee = clone.InBackPerc_Melee;
            this.InBackPerc_Ranged = clone.InBackPerc_Ranged;
            this.InBack = (this.InBackPerc_Melee + this.InBackPerc_Ranged) > 0f;
            this.Max_Players = clone.Max_Players;
            this.Min_Healers = clone.Min_Healers;
            this.Min_Tanks = clone.Min_Tanks;
            // Offensive
            this.Targets = clone.Targets; this.MultiTargs = this.Targets != null && this.Targets.Count > 0;
            this.DoTs = clone.DoTs;
            this.Attacks = clone.Attacks;
            this.DamagingTargs = (Attacks != null && Attacks.Count > 0);
            // Defensive
            this.Resist_Physical = clone.Resist_Physical;
            this.Resist_Frost = clone.Resist_Frost;
            this.Resist_Fire = clone.Resist_Fire;
            this.Resist_Nature = clone.Resist_Nature;
            this.Resist_Arcane = clone.Resist_Arcane;
            this.Resist_Shadow = clone.Resist_Shadow;
            this.Resist_Holy = clone.Resist_Holy;
            // Impedances
            this.Moves = clone.Moves; this.MovingTargs = this.Moves != null && this.Moves.Count > 0;
            this.Stuns = clone.Stuns; this.StunningTargs = this.Stuns != null && this.Stuns.Count > 0;
            this.Fears = clone.Fears; this.FearingTargs = this.Fears != null && this.Fears.Count > 0;
            this.Roots = clone.Roots; this.RootingTargs = this.Roots != null && this.Roots.Count > 0;
            this.Disarms = clone.Disarms; this.DisarmingTargs = this.Disarms != null && this.Disarms.Count > 0;
            this.TimeBossIsInvuln = clone.TimeBossIsInvuln;
            //
            return this;
        }
        #region Variables
        private bool INBACK = false;
        public bool InBack { get { return INBACK; } set { INBACK = value; OnPropertyChanged("InBack"); } }

        private bool MULTITARGS = false;
        public bool MultiTargs { get { return MULTITARGS; } set { MULTITARGS = value; OnPropertyChanged("MultiTargs"); } }

        private bool STUNNINGTARGS = false;
        public bool StunningTargs { get { return STUNNINGTARGS; } set { STUNNINGTARGS = value; OnPropertyChanged("StunningTargs"); } }
        private bool MOVINGTARGS = false;
        public bool MovingTargs { get { return MOVINGTARGS; } set { MOVINGTARGS = value; OnPropertyChanged("MovingTargs"); } }
        private bool FEARINGTARGS = false;
        public bool FearingTargs { get { return FEARINGTARGS; } set { FEARINGTARGS = value; OnPropertyChanged("FearingTargs"); } }
        private bool ROOTINGTARGS = false;
        public bool RootingTargs { get { return ROOTINGTARGS; } set { ROOTINGTARGS = value; OnPropertyChanged("RootingTargs"); } }
        private bool DISARMINGTARGS = false;
        public bool DisarmingTargs { get { return DISARMINGTARGS; } set { DISARMINGTARGS = value; OnPropertyChanged("DisarmingTargs"); } }
        private bool DAMAGINGTARGS = false;
        public bool DamagingTargs { get { return DAMAGINGTARGS; } set { DAMAGINGTARGS = value; OnPropertyChanged("DamagingTargs"); } }

        private double UNDER35PERC = 0.10d;
        public double Under35Perc { get { return UNDER35PERC; } set { UNDER35PERC = value; OnPropertyChanged("Under35Perc"); } }
        private double UNDER20PERC = 0.15d;
        public double Under20Perc { get { return UNDER20PERC; } set { UNDER20PERC = value; OnPropertyChanged("Under20Perc"); } }

        private BossList.FilterType FILTERTYPE = BossList.FilterType.Content;
        public BossList.FilterType FilterType { get { return FILTERTYPE; } set { FILTERTYPE = value; OnPropertyChanged("FilterType"); } }
        private string FILTER = "";
        public string Filter { get { return FILTER; } set { FILTER = value; OnPropertyChanged("Filter"); } }
        private string BOSSNAME = "";
        public string BossName { get { return BOSSNAME; } set { BOSSNAME = value; OnPropertyChanged("BossName"); } }
        #endregion
    }

#if !RAWR3 && !SILVERLIGHT
    [Serializable]
#endif
    public partial class BossHandler {
        public const int NormCharLevel = (int)POSSIBLE_LEVELS.LVLP0;
        public BossHandler() { }
        public BossHandler Clone() {
            BossHandler clone = (BossHandler)this.MemberwiseClone();
            //
            clone.Targets = new List<TargetGroup>(this.Targets);
            clone.Attacks = new List<Attack>(this.Attacks);
            clone.Moves = new List<Impedance>(this.Moves);
            clone.Stuns = new List<Impedance>(this.Stuns);
            clone.Fears = new List<Impedance>(this.Fears);
            clone.Roots = new List<Impedance>(this.Roots);
            clone.Disarms = new List<Impedance>(this.Disarms);
            //
            return clone;
        }

        #region Variables
        #region Enums/Convertors
        protected readonly static string[] BossTierStrings = new string[] {
            "Tier  7.0",
            "Tier  7.5",
            "Tier  8.0",
            "Tier  8.5",
            "Tier  9.0",
            "Tier  9.5",
            "Tier 10.0",
            "Tier 10.5",
            "Tier 10.9",
        };
        protected readonly static string[] BossVersionStrings = new string[] {
            "10 Man",
            "25 Man",
            "10 Man (H)",
            "25 Man (H)",
        };
        public enum Versions   : int { V_10N = 0, V_25N = 1, V_10H = 2, V_25H = 3, V_10 = 0, V_25 = 1 } // last two are for file compatibility between versions
        public enum TierLevels : int { T7_0 = 0, T7_5, T8_0, T8_5, T9_0, T9_5, T10_0, T10_5, T10_9 }
        public static readonly float[] StandardMeleePerHit = new float[] {
                 5000f*2f, //T7_0,
                10000f*2f, //T7_5,
                20000f*2f, //T8_0,
                30000f*2f, //T8_5,
                40000f*2f, //T9_0,
                50000f*2f, //T9_5,
                60000f*2f, //T10_0,
                70000f*2f, //T10_5,
                80000f*2f, //T10_9,
        };
        #endregion
        #region ==== Info ====
        private string NAME = "Generic",
            INSTANCE = "None",
            COMMENT = "No comments have been written for this Boss.";
        private TierLevels CONTENT = TierLevels.T7_0;
        private Versions VERSION = Versions.V_10N;
        #endregion
        #region ==== Basics ====
        private float HEALTH = 1000000f;
        private int BERSERKTIMER = 8 * 60,
                    SPEEDKILLTIMER = 3 * 60,
                    LEVEL = (int)POSSIBLE_LEVELS.LVLP3,
                    ARMOR = (int)StatConversion.NPC_ARMOR[3],
                    MAX_PLAYERS = 10,
                    MIN_HEALERS = 3,
                    MIN_TANKS = 2;
        private double INBACKPERC_MELEE = 0.00d,
                       INBACKPERC_RANGED = 0.00d;
        #endregion
        #region ==== Offensive ====
        public List<TargetGroup> Targets = new List<TargetGroup>();
        /// <summary>WARNING! This variable is not presently used!</summary>
        private List<DoT> DOTS = new List<DoT>();
        private List<Attack> ATTACKS = new List<Attack>();
        #endregion
        #region ==== Defensive ====
        private double RESISTANCE_PHYSICAL = 0,
                       RESISTANCE_FROST = 0,
                       RESISTANCE_FIRE = 0,
                       RESISTANCE_NATURE = 0,
                       RESISTANCE_ARCANE = 0,
                       RESISTANCE_SHADOW = 0,
                       RESISTANCE_HOLY = 0;
        #endregion
        #region ==== Impedances ====
        public List<Impedance> Moves = new List<Impedance>();
        public List<Impedance> Stuns = new List<Impedance>();
        public List<Impedance> Fears = new List<Impedance>();
        public List<Impedance> Roots = new List<Impedance>();
        public List<Impedance> Disarms = new List<Impedance>();
        private float TIMEBOSSISINVULN = 0;
        #endregion
        #endregion

        #region Get/Set
        #region ==== Info ====
        protected string GetVersionString(Versions v) { return BossVersionStrings[(int)v]; }
        protected string GetContentString(TierLevels c) { return BossTierStrings[(int)c]; }
        public string Name               { get { return NAME;               } set { NAME               = value; OnPropertyChanged("Name"              ); } }
        public TierLevels Content        { get { return CONTENT;            } set { CONTENT            = value; OnPropertyChanged("Content"           ); } }
        public string ContentString      { get { return GetContentString(CONTENT); } }
        public string Instance           { get { return INSTANCE;           } set { INSTANCE           = value; OnPropertyChanged("Instance"          ); } }
        public Versions Version          { get { return VERSION;            } set { VERSION            = value; OnPropertyChanged("Version"           ); } }
        public string VersionString      { get { return GetVersionString(VERSION); } }
        public string Comment            { get { return COMMENT;            } set { COMMENT            = value; OnPropertyChanged("Comment"           ); } }
        #endregion
        #region ==== Basics ====
        public int    Level              { get { return LEVEL;              } set { LEVEL              = value; OnPropertyChanged("Level"             ); } }
        public int    Armor              { get { return ARMOR;              } set { ARMOR              = value; OnPropertyChanged("Armor"             ); } }
        public int    BerserkTimer       { get { return BERSERKTIMER;       } set { BERSERKTIMER       = value; OnPropertyChanged("BerserkTimer"      ); } }
        public int    SpeedKillTimer     { get { return SPEEDKILLTIMER;     } set { SPEEDKILLTIMER     = value; OnPropertyChanged("SpeedKillTimer"    ); } }
        public float  Health             { get { return HEALTH;             } set { HEALTH             = value; OnPropertyChanged("Health"            ); } }
        public double  InBackPerc_Melee  { get { return INBACKPERC_MELEE;   } set { INBACKPERC_MELEE   = CPd(value); OnPropertyChanged("InBackPerc_Melee"  ); } }
        public double  InBackPerc_Ranged { get { return INBACKPERC_RANGED;  } set { INBACKPERC_RANGED  = CPd(value); OnPropertyChanged("InBackPerc_Ranged" ); } }
        /// <summary>Example values: 5, 10, 25, 40</summary>
        public int Max_Players { get { return MAX_PLAYERS; } set { MAX_PLAYERS = value; OnPropertyChanged("Max_Players"); } }
        public int Min_Healers { get { return MIN_HEALERS; } set { MIN_HEALERS = value; OnPropertyChanged("Min_Healers"); } }
        public int Min_Tanks { get { return MIN_TANKS; } set { MIN_TANKS = value; OnPropertyChanged("Min_Tanks"); } }
        #endregion
        #region ==== Offensive ====
        // ==== Multiple Targets ====
        //public double  MultiTargsPerc { get { return MULTITARGSPERC; } set { MULTITARGSPERC = CPd(value); OnPropertyChanged("MultiTargsPerc"); } }
        //public double  MaxNumTargets  { get { return MAXNUMTARGS;    } set { MAXNUMTARGS    = value; OnPropertyChanged("MaxNumTargs"        ); } }
        public TargetGroup DynamicCompiler_MultiTargs {
            get {
                // Make one
                TargetGroup retVal = new TargetGroup();
                // Find the averaged _____
                float time = MultiTargsTime;
                float dur = MultiTargsDur;
                float acts = time / (dur / 1000f);
                float freq = BerserkTimer / acts;
                float chance = MultiTargsChance;
                double num = MultiTargsNum;
                bool near = MultiTargsNear;
                // Mark those into the retVal
                retVal.Frequency = freq;
                retVal.Duration = dur;
                retVal.Chance = chance;
                retVal.NearBoss = near;
                retVal.NumTargs = (float)num;
                // Double-check we aren't sending a bad one
                if (retVal.Frequency <= 0f || retVal.Chance <= 0f) {
                    retVal.Frequency = -1f; // if we are, use this as a flag
                }
                // Return results
                return retVal;
            }
        }
        public float MultiTargsFreq {
            get {
                if (Targets.Count > 0) {
                    // Adds up the total number of Moves and evens them out over the Berserk Timer
                    float numMultiTargsOverDur = 0;
                    foreach (TargetGroup s in Targets) {
                        numMultiTargsOverDur += (BerserkTimer / s.Frequency) * s.Chance;
                    }
                    float freq = BerserkTimer / numMultiTargsOverDur;
                    return freq;
                } else { return 0; }
            }
        }
        public float MultiTargsDur {
            get {
                if (Targets.Count > 0) {
                    // Averages out the MultiTarg Durations
                    float TotalMultiTargDur = 0;
                    foreach (TargetGroup s in Targets) { TotalMultiTargDur += s.Duration; }
                    float dur = TotalMultiTargDur / Targets.Count;
                    return dur;
                } else { return 0; }
            }
        }
        public double MultiTargsNum {
            get {
                if (Targets.Count > 0) {
                    // Averages out the MultiTarg Amounts
                    float TotalMultiTargAmt = 0;
                    foreach (TargetGroup s in Targets) { TotalMultiTargAmt += s.NumTargs; }
                    float dur = TotalMultiTargAmt / Targets.Count;
                    return dur;
                } else { return 0; }
            }
        }
        public float MultiTargsChance {
            get {
                if (Targets.Count > 0) {
                    // Averages out the MultiTarg Chances
                    float TotalChance = 0f;
                    foreach (TargetGroup s in Targets) { TotalChance += s.Chance; }
                    float chance = TotalChance / (float)Targets.Count;
                    return chance;
                } else { return 0; }
            }
        }
        public float MultiTargsTime {
            get {
                float time = 0f;
                float freq = MultiTargsFreq;
                float dur = MultiTargsDur;
                if (freq > 0f && freq < BerserkTimer) {
                    time = (BerserkTimer / freq) * (dur / 1000f);
                }
                return time;
            }
        }
        public bool MultiTargsNear {
            get {
                if (Targets.Count > 0) {
                    // Averages out the Move Durations
                    int countYes = 0;
                    int countNo = 0;
                    foreach (TargetGroup s in Targets) {
                        if (s.NearBoss) { countYes++; } else { countNo++; };
                    }
                    return countYes >= countNo;
                } else { return false; }
            }
        }
        #region Attacks
        // ==== Attacks ====
        public List<DoT> DoTs { get { return DOTS; } set { DOTS = value; } }// not actually used! Don't even try!
        public List<Attack> Attacks { get { return ATTACKS; } set { ATTACKS = value; } }
        public Attack DynamicCompiler_Attacks
        {
            get
            {
                // Make one
                Attack retVal = new Attack()
                {
                    // Basics
                    Name = "Dynamic",
                    DamageType = ItemDamageType.Physical,
                    DamagePerHit = 80f * 1000f,
                    DamageIsPerc = false,
                    MaxNumTargets = 1,
                    AttackSpeed = 2.0f,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    UseParryHaste = true,
                    Interruptable = false,
                    // Player Avoidances
                    Missable = true,
                    Dodgable = true,
                    Parryable = true,
                    Blockable = true,
                    // Targeting Ignores
                    IgnoresMTank = false,
                    IgnoresOTank = false,
                    IgnoresTTank = false,
                    IgnoresHealers = false,
                    IgnoresMeleeDPS = false,
                    IgnoresRangedDPS = false,
                };
                if (Attacks.Count <= 0) { retVal.AttackSpeed = -1; return retVal; }
                // Find the averaged _____
                int numTargs = 0;
                float speeds = 0;
                float dph = 0;
                foreach(Attack a in Attacks){
                    dph += a.DamagePerHit;
                    numTargs += (int)a.MaxNumTargets;
                    speeds += (int)a.AttackSpeed;
                }
                // Mark those into the retVal
                retVal.DamagePerHit = dph / (float)Attacks.Count;
                retVal.MaxNumTargets = (int)((float)numTargs / (float)Attacks.Count);
                retVal.AttackSpeed = (int)(speeds / (float)Attacks.Count);
                // Double-check we aren't sending a bad one
                if (retVal.AttackSpeed <= 0f)
                {
                    retVal.AttackSpeed = -1f; // if we are, use this as a flag
                }
                // Return results
                return retVal;
            }
        }
        public Attack DynamicCompiler_FilteredAttacks(List<Attack> atks)
        {
            // Make one
            Attack retVal = new Attack()
            {
                // Basics
                Name = "DynamicFiltered",
                DamageType = ItemDamageType.Physical,
                DamagePerHit = 80f * 1000f,
                DamageIsPerc = false,
                MaxNumTargets = 1,
                AttackSpeed = 2.0f,
                AttackType = ATTACK_TYPES.AT_MELEE,
                UseParryHaste = true,
                Interruptable = false,
                // Player Avoidances
                Missable = true,
                Dodgable = true,
                Parryable = true,
                Blockable = true,
                // Targeting Ignores
                IgnoresMTank = false,
                IgnoresOTank = false,
                IgnoresTTank = false,
                IgnoresHealers = false,
                IgnoresMeleeDPS = false,
                IgnoresRangedDPS = false,
            };
            if (atks.Count <= 0) { retVal.AttackSpeed = -1; return retVal; }
            // Find the averaged _____
            int numTargs = 0;
            float speeds = 0;
            float dph = 0;
            foreach (Attack a in atks)
            {
                dph += a.DamagePerHit;
                numTargs += (int)a.MaxNumTargets;
                speeds += (int)a.AttackSpeed;
            }
            // Mark those into the retVal
            retVal.DamagePerHit = dph / (float)atks.Count;
            retVal.MaxNumTargets = (int)((float)numTargs / (float)atks.Count);
            retVal.AttackSpeed = (int)(speeds / (float)atks.Count);
            // Double-check we aren't sending a bad one
            if (retVal.AttackSpeed <= 0f)
            {
                retVal.AttackSpeed = -1f; // if we are, use this as a flag
            }
            // Return results
            return retVal;
        }
        // ==== Methods for Pulling Boss DPS ==========
        public List<Attack> GetFilteredAttackList(ATTACK_TYPES type) {
            List<Attack> attacks = new List<Attack>();
            if (Attacks.Count <= 0) { return attacks; } // make sure there were some TO put in there
            foreach (Attack a in Attacks) { if (a.AttackType == type) { attacks.Add(a); } }
            return attacks;
        }
        /// <summary>Public function for the DPS Gets so we can re-use code. Includes a full player defend table.</summary>
        /// <param name="type">The type of attack to check: AT_MELEE, AT_RANGED, AT_AOE</param>
        /// <param name="BossDamageBonus">Perc value (0.10f = 110% Base damage)</param>
        /// <param name="BossDamagePenalty">Perc value (0.10f = 90% Base damage)</param>
        /// <param name="p_missPerc">Perc value (0.08f = 8% Chance for Boss to Miss Player)</param>
        /// <param name="p_dodgePerc">Perc value (0.201f = 20.10% Chance for Player to Dodge Boss Attack)</param>
        /// <param name="p_parryPerc">Perc value (0.1375f = 13.75% Chance for Player to Parry Boss Attack)</param>
        /// <param name="p_blockPerc">Perc value (0.065f = 6.5% Chance for Player to Block Boss Attack)</param>
        /// <param name="p_blockVal">How much Damage is absorbed by player's Shield in Block Value</param>
        /// <returns>The DPS value requested, returns zero if no Attacks have been created for the Boss or there are no Attacks of that Type.</returns>
        public float GetDPSByType(ATTACK_TYPES type, float BossDamageBonus, float BossDamagePenalty,
                                  float p_missPerc, float p_dodgePerc, float p_parryPerc, float p_blockPerc, float p_blockVal)
        {
            List<Attack> attacks = GetFilteredAttackList(type);
            if (attacks.Count <= 0) { return 0f; } // make sure there were some put in there

            float retDPS = 0f;

            foreach (Attack a in attacks) {
                float damage = a.DamagePerHit * (1f + BossDamageBonus) * (1f - BossDamagePenalty),
                      damageOnUse = damage * (1f - p_missPerc - p_dodgePerc - p_parryPerc - p_blockPerc), // takes out the player's defend table
                      swing = a.AttackSpeed;
                      damageOnUse += (damage - p_blockVal) * p_blockPerc; // Adds reduced damage from blocks back in
                float acts = BerserkTimer / swing,
                      avgDmg = damageOnUse * acts,
                      dps = avgDmg / BerserkTimer;
                retDPS += dps;
            }

            return retDPS;
        }
        /// <summary>Public function for the DPS Gets so we can re-use code.</summary>
        /// <param name="type">The type of attack to check: AT_MELEE, AT_RANGED, AT_AOE</param>
        /// <param name="BossDamageBonus">Perc value (0.10f = 110% Base damage)</param>
        /// <param name="BossDamagePenalty">Perc value (0.10f = 90% Base damage)</param>
        /// <param name="p_missPerc">Perc value (0.08f = 8% Chance for Boss to Miss Player)</param>
        /// <returns>The DPS value requested, returns zero if no Attacks have been created for the Boss or there are no Attacks of that Type.</returns>
        public float GetDPSByType(ATTACK_TYPES type, float BossDamageBonus, float BossDamagePenalty, float p_missPerc) {
            return GetDPSByType(type, BossDamageBonus, BossDamagePenalty, p_missPerc, 0, 0, 0, 0);
        }
        /// <summary>Public function for the DPS Gets so we can re-use code.</summary>
        /// <param name="type">The type of attack to check: AT_MELEE, AT_RANGED, AT_AOE</param>
        /// <param name="BossDamageBonus">Perc value (0.10f = 110% Base damage)</param>
        /// <param name="BossDamagePenalty">Perc value (0.10f = 90% Base damage)</param>
        /// <returns>The DPS value requested, returns zero if no Attacks have been created for the Boss or there are no Attacks of that Type.</returns>
        public float GetDPSByType(ATTACK_TYPES type, float BossDamageBonus, float BossDamagePenalty) {
            return GetDPSByType(type, BossDamageBonus, BossDamagePenalty, 0, 0, 0, 0, 0);
        }
        /// <summary>
        /// Gets Raw DPS of all attacks that are Melee type. DPS and Healing characters should not normally see this damage.
        /// Tanks will recieve this damage.
        /// </summary>
        public float DPS_SingleTarg_Melee { get { float dps = GetDPSByType(ATTACK_TYPES.AT_MELEE, 0, 0); return dps; } }
        /// <summary>
        /// Gets Raw DPS of all attacks that are Ranged type. DPS and Healing characters will use this
        /// to determine incoming damage to Raid, on specific targets. Tanks will recieve this damage in
        /// addition to the Melee single-target under chance methods.
        /// </summary>
        public float DPS_SingleTarg_Ranged { get { float dps = GetDPSByType(ATTACK_TYPES.AT_RANGED, 0, 0, 0); return dps; } }
        /// <summary>
        /// Gets Raw DPS of all attacks that are AoE type. DPS and Healing characters will use this
        /// to determine incoming damage to Raid. Tanks will recieve this damage in addition to the
        /// Melee single-target.
        /// </summary>
        public float DPS_AoE { get { float dps = GetDPSByType(ATTACK_TYPES.AT_AOE, 0, 0); return dps; } }
        /// <summary>
        /// Iterates though the attack list to find the Default Melee Attack.
        /// <para>If the Attack list is empty, this will return null</para>
        /// <para>If the Attack list doesn't have an attack listed as the Default Melee Attack, this will return null.</para>
        /// </summary>
        public Attack DefaultMeleeAttack {
            get
            {
                // There are no attacks
                if (Attacks.Count <= 0) { return null; }
                // Iterating the list to find it
                foreach (Attack a in Attacks) {
                    if (a.IsTheDefaultMelee) {
                        return a;
                    }
                }
                // We reached the end without finding it
                return null;
                
            }
        }
        // AoE Targets
        public float  AoETargsFreq  {
            get {
                List<Attack> attacks = GetFilteredAttackList(ATTACK_TYPES.AT_AOE);
                if (attacks.Count > 0) {
                    // Adds up the total number of AoEs and evens them out over the Berserk Timer
                    float numAoEsOverDur = 0;
                    foreach (Attack s in attacks) {
                        numAoEsOverDur += BerserkTimer / s.AttackSpeed;
                    }
                    float freq = BerserkTimer / numAoEsOverDur;
                    return freq;
                } else {
                    return BerserkTimer;
                }
            }
        }
        public float  AoETargsDMG   {
            get {
                List<Attack> attacks = GetFilteredAttackList(ATTACK_TYPES.AT_AOE);
                if (attacks.Count > 0) {
                    // Averages out the Root Durations
                    float TotalaAoEDmg = 0;
                    foreach (Attack s in attacks) { TotalaAoEDmg += s.DamagePerHit; }
                    float dur = TotalaAoEDmg / attacks.Count;
                    return dur;
                } else {
                    return 1500f;
                }
            }
        }
        #endregion
        #endregion
        #region ==== Defensive ====
        public double Resist_Physical { get { return RESISTANCE_PHYSICAL; } set { RESISTANCE_PHYSICAL = value; OnPropertyChanged("Resist_Physical"); } }
        public double Resist_Frost { get { return RESISTANCE_FROST; } set { RESISTANCE_FROST = value; OnPropertyChanged("Resist_Frost"); } }
        public double Resist_Fire { get { return RESISTANCE_FIRE; } set { RESISTANCE_FIRE = value; OnPropertyChanged("Resist_Fire"); } }
        public double Resist_Nature { get { return RESISTANCE_NATURE; } set { RESISTANCE_NATURE = value; OnPropertyChanged("Resist_Nature"); } }
        public double Resist_Arcane { get { return RESISTANCE_ARCANE; } set { RESISTANCE_ARCANE = value; OnPropertyChanged("Resist_Arcane"); } }
        public double Resist_Shadow { get { return RESISTANCE_SHADOW; } set { RESISTANCE_SHADOW = value; OnPropertyChanged("Resist_Shadow"); } }
        public double Resist_Holy { get { return RESISTANCE_HOLY; } set { RESISTANCE_HOLY = value; OnPropertyChanged("Resist_Holy"); } }
        /// <summary>A handler for Boss Damage Taken Reduction due to Resistance (Physical, Fire, etc). </summary>
        /// <returns>The Percentage of Damage to be removed (0.25 = 25% Damage Reduced, 100 Damage should become 75)</returns>
        public double Resistance(ItemDamageType type) {
            switch (type) {
                case ItemDamageType.Physical: return RESISTANCE_PHYSICAL;
                case ItemDamageType.Nature:   return RESISTANCE_NATURE;
                case ItemDamageType.Arcane:   return RESISTANCE_ARCANE;
                case ItemDamageType.Frost:    return RESISTANCE_FROST;
                case ItemDamageType.Fire:     return RESISTANCE_FIRE;
                case ItemDamageType.Shadow:   return RESISTANCE_SHADOW;
                case ItemDamageType.Holy:     return RESISTANCE_HOLY;
                default: break;
            }
            return 0f;
        }
        /// <summary>A handler for Boss Damage Taken Reduction due to Resistance (Physical, Fire, etc). This is the Set function</summary>
        /// <returns>The Percentage of Damage to be removed (0.25 = 25% Damage Reduced, 100 Damage should become 75)</returns>
        public double Resistance(ItemDamageType type, float newValue) {
            switch (type) {
                case ItemDamageType.Physical: return Resist_Physical = newValue;
                case ItemDamageType.Frost:    return Resist_Frost    = newValue;
                case ItemDamageType.Fire:     return Resist_Fire     = newValue;
                case ItemDamageType.Nature:   return Resist_Nature   = newValue;
                case ItemDamageType.Arcane:   return Resist_Arcane   = newValue;
                case ItemDamageType.Shadow:   return Resist_Shadow   = newValue;
                case ItemDamageType.Holy:     return Resist_Holy     = newValue;
                default: break;
            }
            return 0f;
        }
        #endregion
        #region ==== Impedances ====
        protected Impedance DynamicCompiler(List<Impedance> imps) {
            //if (imps == null || imps.Count <= 0) return null;
            // Make one
            Impedance retVal = new Impedance() {
                Frequency = 20f,
                Duration = 1f * 1000f,
                Chance = 1.00f,
                Breakable = true,
            };
            // Find the averaged _____
            float time = Time(imps);
            float dur = Dur(imps);
            float acts = time / (dur / 1000f);
            float freq = BerserkTimer / acts;
            float chance = Chance(imps);
            // Mark those into the retVal
            retVal.Frequency = freq;
            retVal.Duration = dur;
            retVal.Chance = chance;
            // Double-check we aren't sending a bad one
            if (retVal.Frequency <= 0f || retVal.Chance <= 0f)
            {
                retVal.Frequency = -1f; // if we are, use this as a flag
            }
            // Return results
            return retVal;
        }
        protected float Freq(List<Impedance> imps) {
            if (imps == null || imps.Count < 1) return -1;
            // Adds up the total number of impedences
            // and evens them out over the Berserk Timer
            float numImpsOverDur = 0f;
            foreach (Impedance imp in imps) {
                numImpsOverDur += (BerserkTimer / imp.Frequency) * imp.Chance;
            }
            return BerserkTimer / numImpsOverDur;
        }
        protected float Time(List<Impedance> imps) {
            if (imps == null || imps.Count < 1) return 0;
            float time = 0f;
            float freq = Freq(imps);
            float dur = Dur(imps) / 1000f;
            if (freq > 0f && freq < BerserkTimer) {
                time = (BerserkTimer / freq) * dur;
            } else if (freq >= BerserkTimer) {
                time = dur;
            }
            return time;
        }
        protected float Dur(List<Impedance> imps) {
            if (imps == null || imps.Count < 1) return 0;
            // Averages out the Move Durations
            float TotalDur = 0;
            foreach (Impedance s in imps) { TotalDur += s.Duration; }
            return TotalDur / (float)imps.Count;
        }
        protected float Chance(List<Impedance> imps) {
            if (imps == null || imps.Count < 1) return 0;
            // Averages out the Chances
            float TotalChance = 0f;
            foreach (Impedance s in imps) { TotalChance += s.Chance; }
            return TotalChance / (float)imps.Count;
        }
        // Moving Targets
        public Impedance DynamicCompiler_Move
        {
            get {
                // Make one
                Impedance retVal = new Impedance() {
                    Frequency = 20f,
                    Duration = 1f * 1000f,
                    Chance = 1.00f,
                    Breakable = true,
                };
                // Find the averaged _____
                float time = MovingTargsTime;
                float dur = MovingTargsDur;
                float acts = time / (dur / 1000f);
                float freq = BerserkTimer / acts;
                float chance = MovingTargsChance;
                // Mark those into the retVal
                retVal.Frequency = freq;
                retVal.Duration = dur;
                retVal.Chance = chance;
                // Double-check we aren't sending a bad one
                if (retVal.Frequency <= 0f || retVal.Chance <= 0f) {
                    retVal.Frequency = -1f; // if we are, use this as a flag
                }
                // Return results
                return retVal;
            }
        }
        public float MovingTargsFreq      { get { return Moves.Count > 0 ? Freq(Moves) : -1; } }
        public float MovingTargsDur       { get { return Moves.Count > 0 ? Dur(Moves) : 0; } }
        public float MovingTargsChance    { get { return Moves.Count > 0 ? Chance(Moves) : 0; } }
        public float MovingTargsTime      { get { return Moves.Count > 0 ? Time(Moves) : 0; } }
        // Stunning Targets
        public Impedance DynamicCompiler_Stun {
            get {
                // Make one
                Impedance retVal = new Impedance() {
                    Frequency = 20f,
                    Duration = 1f * 1000f,
                    Chance = 1.00f,
                    Breakable = true,
                };
                // Find the averaged _____
                float time = StunningTargsTime;
                float dur = StunningTargsDur;
                float acts = time / (dur / 1000f);
                float freq = BerserkTimer / acts;
                float chance = StunningTargsChance;
                // Mark those into the retVal
                retVal.Frequency = freq;
                retVal.Duration = dur;
                retVal.Chance = chance;
                // Double-check we aren't sending a bad one
                if (retVal.Frequency <= 0f || retVal.Chance <= 0f) {
                    retVal.Frequency = -1f; // if we are, use this as a flag
                }
                // Return results
                return retVal;
            }
        }
        public float StunningTargsFreq    { get { return Stuns.Count > 0 ? Freq(Stuns) : -1; } }
        public float StunningTargsDur     { get { return Stuns.Count > 0 ? Dur(Stuns) : 0; } }
        public float StunningTargsChance  { get { return Stuns.Count > 0 ? Chance(Stuns) : 0; } }
        public float StunningTargsTime    { get { return Stuns.Count > 0 ? Time(Stuns) : 0; } }
        // Fearing Targets
        public Impedance DynamicCompiler_Fear
        {
            get {
                // Make one
                Impedance retVal = new Impedance() {
                    Frequency = 20f,
                    Duration = 1f * 1000f,
                    Chance = 1.00f,
                    Breakable = true,
                };
                // Find the averaged _____
                float time = FearingTargsTime;
                float dur = FearingTargsDur;
                float acts = time / (dur / 1000f);
                float freq = BerserkTimer / acts;
                float chance = FearingTargsChance;
                // Mark those into the retVal
                retVal.Frequency = freq;
                retVal.Duration = dur;
                retVal.Chance = chance;
                // Double-check we aren't sending a bad one
                if (retVal.Frequency <= 0f || retVal.Chance <= 0f) {
                    retVal.Frequency = -1f; // if we are, use this as a flag
                }
                // Return results
                return retVal;
            }
        }
        public float FearingTargsFreq     { get { return Fears.Count > 0 ? Freq(Fears) : -1; } }
        public float FearingTargsDur      { get { return Fears.Count > 0 ? Dur(Fears) : 0; } }
        public float FearingTargsChance   { get { return Fears.Count > 0 ? Chance(Fears) : 0; } }
        public float FearingTargsTime     { get { return Fears.Count > 0 ? Time(Fears) : 0; } }
        // Rooting Targets
        public Impedance DynamicCompiler_Root
        {
            get {
                // Make one
                Impedance retVal = new Impedance()
                {
                    Frequency = 20f,
                    Duration = 1f * 1000f,
                    Chance = 1.00f,
                    Breakable = true,
                };
                // Find the averaged _____
                float time = RootingTargsTime;
                float dur = RootingTargsDur;
                float acts = time / (dur / 1000f);
                float freq = BerserkTimer / acts;
                float chance = RootingTargsChance;
                // Mark those into the retVal
                retVal.Frequency = freq;
                retVal.Duration = dur;
                retVal.Chance = chance;
                // Double-check we aren't sending a bad one
                if (retVal.Frequency <= 0f || retVal.Chance <= 0f) {
                    retVal.Frequency = -1f; // if we are, use this as a flag
                }
                // Return results
                return retVal;
            }
        }
        public float RootingTargsFreq     { get { return Roots.Count > 0 ? Freq(Roots) : -1; } }
        public float RootingTargsDur      { get { return Roots.Count > 0 ? Dur(Roots) : 0; } }
        public float RootingTargsChance   { get { return Roots.Count > 0 ? Chance(Roots) : 0; } }
        public float RootingTargsTime     { get { return Roots.Count > 0 ? Time(Roots) : 0; } }
        // Disarming Targets
        public Impedance DynamicCompiler_Disarm
        {
            get {
                // Make one
                Impedance retVal = new Impedance() {
                    Frequency = 20f,
                    Duration = 1f * 1000f,
                    Chance = 1.00f,
                    Breakable = true,
                };
                // Find the averaged _____
                float time = DisarmingTargsTime;
                float dur = DisarmingTargsDur;
                float acts = time / (dur / 1000f);
                float freq = BerserkTimer / acts;
                float chance = DisarmingTargsChance;
                // Mark those into the retVal
                retVal.Frequency = freq;
                retVal.Duration = dur;
                retVal.Chance = chance;
                // Double-check we aren't sending a bad one
                if (retVal.Frequency <= 0f || retVal.Chance <= 0f) {
                    retVal.Frequency = -1f; // if we are, use this as a flag
                }
                // Return results
                return retVal;
            }
        }
        public float DisarmingTargsFreq   { get { return Disarms.Count > 0 ? Freq(Disarms) : -1; } }
        public float DisarmingTargsDur    { get { return Disarms.Count > 0 ? Dur(Disarms) : 0; } }
        public float DisarmingTargsChance { get { return Disarms.Count > 0 ? Chance(Disarms) : 0; } }
        public float DisarmingTargsTime   { get { return Disarms.Count > 0 ? Time(Disarms) : 0; } }
        // Other
        public float TimeBossIsInvuln { get { return TIMEBOSSISINVULN; } set { TIMEBOSSISINVULN = value; OnPropertyChanged("TimeBossIsInvuln"); } }
        #endregion
        #endregion

        #region Functions
        /// <summary>Constrain Percent (float)</summary>
        /// <param name="value">Value to be constrained</param>
        /// <returns>value if it is between 0% and 100% or those limits</returns>
        public static float CPf(float value) { return Math.Max(0f, Math.Min(1f, value)); }
        /// <summary>Constrain Percent (double)</summary>
        /// <param name="value">Value to be constrained</param>
        /// <returns>value if it is between 0% and 100% or those limits</returns>
        public static double CPd(double value) { return Math.Max(0d, Math.Min(1d, value)); }

        public static float CalcADotTick(float mindmg, float maxdmg, float time, float interval) {
            return ((mindmg + maxdmg) / 2.0f) * time / interval;
        }
        public static float CalcADotTick(float mindmg, float maxdmg, float time)
        {
            return ((mindmg + maxdmg) / 2.0f) * time;
        }

        /// <summary>
        /// Generates a Fight Info description listing the stats of the fight as well as any comments listed for the boss
        /// </summary>
        /// <param name="BossDamageBonus">Perc value (0.10f = 110% Base damage)</param>
        /// <param name="BossDamagePenalty">Perc value (0.10f = 90% Base damage)</param>
        /// <param name="p_missPerc">Perc value (0.08f = 8% Chance for Boss to Miss Player)</param>
        /// <param name="p_dodgePerc">Perc value (0.201f = 20.10% Chance for Player to Dodge Boss Attack)</param>
        /// <param name="p_parryPerc">Perc value (0.1375f = 13.75% Chance for Player to Parry Boss Attack)</param>
        /// <param name="p_blockPerc">Perc value (0.065f = 6.5% Chance for Player to Block Boss Attack)</param>
        /// <param name="p_blockVal">How much Damage is absorbed by player's Shield in Block Value</param>
        /// <returns>The generated string</returns>
        public string GenInfoString(float BossDamageBonus, float BossDamagePenalty,
                                  float p_missPerc, float p_dodgePerc, float p_parryPerc, float p_blockPerc, float p_blockVal)
        {
            int room = Max_Players - Min_Healers - Min_Tanks;
            float TotalDPSNeeded = Health / (BerserkTimer - TimeBossIsInvuln - MovingTargsTime);
            float dpsdps = TotalDPSNeeded * ((float)room / ((float)Min_Tanks / 2f + (float)room)) / (float)room;
            float tankdps = (TotalDPSNeeded * ((float)Min_Tanks / ((float)Min_Tanks / 2f + (float)room)) / (float)Min_Tanks) / 2f;
            string retVal = "";
            //
            retVal += string.Format("Name: {0}\r\nContent: {1}\r\nInstance: {2} ({3})\r\nHealth: {4:#,##0}\r\n",
                Name, ContentString.Replace("  "," "), Instance, VersionString, Health);
            //
            retVal += string.Format("Enrage Timer: {0:mm} min {0:ss} sec\r\nSpeed Kill Timer: {1:mm} min {1:ss} sec\r\n\r\n",
                TimeSpan.FromSeconds(BerserkTimer), TimeSpan.FromSeconds(SpeedKillTimer));
            //
            retVal += string.Format("Raid Setup: {0:0} man ({1:0} Tanks {2:0} Healers {3:0} DPS)\r\n\r\n",
                Max_Players, Min_Tanks, Min_Healers, room);
            //
            retVal += string.Format("To beat the Enrage Timer you need {0:#,##0} Total DPS\r\n"
                + "{1:#,##0} from each Tank {2:#,##0} from each DPS\r\n\r\n",
                TotalDPSNeeded, tankdps, dpsdps);
            //
            TotalDPSNeeded = Health / (SpeedKillTimer - TimeBossIsInvuln - SpeedKillTimer * (MovingTargsTime / BerserkTimer));
            dpsdps = TotalDPSNeeded * ((float)room / ((float)Min_Tanks / 2f + (float)room)) / (float)room;
            tankdps = (TotalDPSNeeded * ((float)Min_Tanks / ((float)Min_Tanks / 2f + (float)room)) / (float)Min_Tanks) / 2f;
            //
            retVal += string.Format("To beat the Speed Kill Timer you need {0:#,##0} Total DPS\r\n"
                + "{1:#,##0} from each Tank {2:#,##0} from each DPS\r\n\r\n",
                TotalDPSNeeded, tankdps, dpsdps);
            //
            retVal += string.Format("This boss does the following Damage Per Second Amounts, factoring Armor, Resistance and Defend Tables where applicable:\r\n"
                + "Single Target Melee: {0:0.0}\r\n"
                + "Single Target Ranged: {1:0.0}\r\n"
                + "Raid AoE: {2:0.0}\r\n\r\n",
                GetDPSByType(ATTACK_TYPES.AT_MELEE, BossDamageBonus, BossDamagePenalty, p_missPerc, p_dodgePerc, p_parryPerc, p_blockPerc, p_blockVal),
                GetDPSByType(ATTACK_TYPES.AT_RANGED, BossDamageBonus, BossDamagePenalty, p_missPerc),
                GetDPSByType(ATTACK_TYPES.AT_AOE, BossDamageBonus, 0));
            //
            retVal += string.Format("Comment(s):\r\n{0}", Comment);
            //
            return retVal;
        }
        /// <summary>
        /// Generates a Fight Info description listing the stats of the fight as well as any comments listed for the boss
        /// </summary>
        /// <returns>The generated string</returns>
        public string GenInfoString() { return GenInfoString(0,0,0,0,0,0,0); }
        public string GenInfoString(Character character) {
            if (character == null || character.CurrentCalculations == null) return GenInfoString();
            Stats stats = character.CurrentCalculations.GetCharacterStats(character);
            return GenInfoString(stats.DamageTakenMultiplier, 0f,
                stats.Defense * StatConversion.DEFENSE_RATING_AVOIDANCE_MULTIPLIER,
                stats.Dodge, stats.Parry, stats.Block, stats.BlockValue);
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
        #endregion

        public override string ToString()
        {
            return Content.ToString() + " : " + Instance + " : (" + Version.ToString() + ") : " + Name;
        }
    }

    public class MultiDiffBoss : List<BossHandler>
    {
        public MultiDiffBoss()
        {
            // Initialize
            //this = new List<BossHandler>() { };
            this.Add(new BossHandler());
            this.Add(new BossHandler());
            this.Add(new BossHandler());
            this.Add(new BossHandler());
            // Basic Setups we don't want to repeat over and over again
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_9 };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10H, BossHandler.Versions.V_25H };
            // Fight Requirements
            Min_Tanks = new int[] { 2, 2, 2, 2 };
            Min_Healers = new int[] { 2, 5, 2, 5 };
        }
        #region Variable Convenience Overrides
        // Info
        public string Name
        {
            get { return this[0].Name; }
            set
            {
                this[0].Name = value;
                this[1].Name = value;
                this[2].Name = value;
                this[3].Name = value;
            }
        }
        public string Instance
        {
            get { return this[0].Instance; }
            set
            {
                this[0].Instance = value;
                this[1].Instance = value;
                this[2].Instance = value;
                this[3].Instance = value;
            }
        }
        public BossHandler.TierLevels[] Content
        {
            get
            {
                return new BossHandler.TierLevels[] {
                    this[0].Content,
                    this[1].Content,
                    this[2].Content,
                    this[3].Content,
                };
            }
            set
            {
                int i = 0;
                this[i].Content = value[i]; i++;
                this[i].Content = value[i]; i++;
                this[i].Content = value[i]; i++;
                this[i].Content = value[i];
            }
        }
        public BossHandler.Versions[] Version
        {
            get
            {
                return new BossHandler.Versions[] {
                    this[0].Version,
                    this[1].Version,
                    this[2].Version,
                    this[3].Version,
                };
            }
            set
            {
                int i = 0;
                this[i].Version = value[i]; i++;
                this[i].Version = value[i]; i++;
                this[i].Version = value[i]; i++;
                this[i].Version = value[i];
            }
        }
        public string Comment
        {
            get { return this[0].Comment; }
            set
            {
                this[0].Comment = value;
                this[1].Comment = value;
                this[2].Comment = value;
                this[3].Comment = value;
            }
        }
        // Basics
        public float[] Health
        {
            get
            {
                return new float[] {
                    this[0].Health,
                    this[1].Health,
                    this[2].Health,
                    this[3].Health,
                };
            }
            set
            {
                int i = 0;
                this[i].Health = value[i]; i++;
                this[i].Health = value[i]; i++;
                this[i].Health = value[i]; i++;
                this[i].Health = value[i];
            }
        }
        public int[] BerserkTimer
        {
            get
            {
                return new int[] {
                    this[0].BerserkTimer,
                    this[1].BerserkTimer,
                    this[2].BerserkTimer,
                    this[3].BerserkTimer,
                };
            }
            set
            {
                int i = 0;
                this[i].BerserkTimer = value[i]; i++;
                this[i].BerserkTimer = value[i]; i++;
                this[i].BerserkTimer = value[i]; i++;
                this[i].BerserkTimer = value[i];
            }
        }
        public int[] SpeedKillTimer
        {
            get
            {
                return new int[] {
                    this[0].SpeedKillTimer,
                    this[1].SpeedKillTimer,
                    this[2].SpeedKillTimer,
                    this[3].SpeedKillTimer,
                };
            }
            set
            {
                int i = 0;
                this[i].SpeedKillTimer = value[i]; i++;
                this[i].SpeedKillTimer = value[i]; i++;
                this[i].SpeedKillTimer = value[i]; i++;
                this[i].SpeedKillTimer = value[i];
            }
        }
        public double[] InBackPerc_Melee
        {
            get
            {
                return new double[] {
                    this[0].InBackPerc_Melee,
                    this[1].InBackPerc_Melee,
                    this[2].InBackPerc_Melee,
                    this[3].InBackPerc_Melee,
                };
            }
            set
            {
                int i = 0;
                this[i].InBackPerc_Melee = value[i]; i++;
                this[i].InBackPerc_Melee = value[i]; i++;
                this[i].InBackPerc_Melee = value[i]; i++;
                this[i].InBackPerc_Melee = value[i];
            }
        }
        public double[] InBackPerc_Ranged
        {
            get
            {
                return new double[] {
                    this[0].InBackPerc_Ranged,
                    this[1].InBackPerc_Ranged,
                    this[2].InBackPerc_Ranged,
                    this[3].InBackPerc_Ranged,
                };
            }
            set
            {
                int i = 0;
                this[i].InBackPerc_Ranged = value[i]; i++;
                this[i].InBackPerc_Ranged = value[i]; i++;
                this[i].InBackPerc_Ranged = value[i]; i++;
                this[i].InBackPerc_Ranged = value[i];
            }
        }
        public int[] Max_Players
        {
            get
            {
                return new int[] {
                    this[0].Max_Players,
                    this[1].Max_Players,
                    this[2].Max_Players,
                    this[3].Max_Players,
                };
            }
            set
            {
                int i = 0;
                this[i].Max_Players = value[i]; i++;
                this[i].Max_Players = value[i]; i++;
                this[i].Max_Players = value[i]; i++;
                this[i].Max_Players = value[i];
            }
        }
        public int[] Min_Tanks
        {
            get
            {
                return new int[] {
                    this[0].Min_Tanks,
                    this[1].Min_Tanks,
                    this[2].Min_Tanks,
                    this[3].Min_Tanks,
                };
            }
            set
            {
                int i = 0;
                this[i].Min_Tanks = value[i]; i++;
                this[i].Min_Tanks = value[i]; i++;
                this[i].Min_Tanks = value[i]; i++;
                this[i].Min_Tanks = value[i];
            }
        }
        public int[] Min_Healers
        {
            get
            {
                return new int[] {
                    this[0].Min_Healers,
                    this[1].Min_Healers,
                    this[2].Min_Healers,
                    this[3].Min_Healers,
                };
            }
            set
            {
                int i = 0;
                this[i].Min_Healers = value[i]; i++;
                this[i].Min_Healers = value[i]; i++;
                this[i].Min_Healers = value[i]; i++;
                this[i].Min_Healers = value[i];
            }
        }
        // Offensive
        /*public double[] MaxNumTargets
        {
            get
            {
                return new double[] {
                    this[0].MaxNumTargets,
                    this[1].MaxNumTargets,
                    this[2].MaxNumTargets,
                    this[3].MaxNumTargets,
                };
            }
            set
            {
                int i = 0;
                this[i].MaxNumTargets = value[i]; i++;
                this[i].MaxNumTargets = value[i]; i++;
                this[i].MaxNumTargets = value[i]; i++;
                this[i].MaxNumTargets = value[i];
            }
        }
        public double[] MultiTargsPerc
        {
            get
            {
                return new double[] {
                    this[0].MultiTargsPerc,
                    this[1].MultiTargsPerc,
                    this[2].MultiTargsPerc,
                    this[3].MultiTargsPerc,
                };
            }
            set
            {
                int i = 0;
                this[i].MultiTargsPerc = value[i]; i++;
                this[i].MultiTargsPerc = value[i]; i++;
                this[i].MultiTargsPerc = value[i]; i++;
                this[i].MultiTargsPerc = value[i];
            }
        }*/
        // Defensive
        public double[] Resist_Physical
        {
            get
            {
                return new double[] {
                    this[0].Resist_Physical,
                    this[1].Resist_Physical,
                    this[2].Resist_Physical,
                    this[3].Resist_Physical,
                };
            }
            set
            {
                int i = 0;
                this[i].Resist_Physical = value[i]; i++;
                this[i].Resist_Physical = value[i]; i++;
                this[i].Resist_Physical = value[i]; i++;
                this[i].Resist_Physical = value[i];
            }
        }
        public double[] Resist_Frost
        {
            get
            {
                return new double[] {
                    this[0].Resist_Frost,
                    this[1].Resist_Frost,
                    this[2].Resist_Frost,
                    this[3].Resist_Frost,
                };
            }
            set
            {
                int i = 0;
                this[i].Resist_Frost = value[i]; i++;
                this[i].Resist_Frost = value[i]; i++;
                this[i].Resist_Frost = value[i]; i++;
                this[i].Resist_Frost = value[i];
            }
        }
        public double[] Resist_Fire
        {
            get
            {
                return new double[] {
                    this[0].Resist_Fire,
                    this[1].Resist_Fire,
                    this[2].Resist_Fire,
                    this[3].Resist_Fire,
                };
            }
            set
            {
                int i = 0;
                this[i].Resist_Fire = value[i]; i++;
                this[i].Resist_Fire = value[i]; i++;
                this[i].Resist_Fire = value[i]; i++;
                this[i].Resist_Fire = value[i];
            }
        }
        public double[] Resist_Nature
        {
            get
            {
                return new double[] {
                    this[0].Resist_Nature,
                    this[1].Resist_Nature,
                    this[2].Resist_Nature,
                    this[3].Resist_Nature,
                };
            }
            set
            {
                int i = 0;
                this[i].Resist_Nature = value[i]; i++;
                this[i].Resist_Nature = value[i]; i++;
                this[i].Resist_Nature = value[i]; i++;
                this[i].Resist_Nature = value[i];
            }
        }
        public double[] Resist_Arcane
        {
            get
            {
                return new double[] {
                    this[0].Resist_Arcane,
                    this[1].Resist_Arcane,
                    this[2].Resist_Arcane,
                    this[3].Resist_Arcane,
                };
            }
            set
            {
                int i = 0;
                this[i].Resist_Arcane = value[i]; i++;
                this[i].Resist_Arcane = value[i]; i++;
                this[i].Resist_Arcane = value[i]; i++;
                this[i].Resist_Arcane = value[i];
            }
        }
        public double[] Resist_Shadow
        {
            get
            {
                return new double[] {
                    this[0].Resist_Shadow,
                    this[1].Resist_Shadow,
                    this[2].Resist_Shadow,
                    this[3].Resist_Shadow,
                };
            }
            set
            {
                int i = 0;
                this[i].Resist_Shadow = value[i]; i++;
                this[i].Resist_Shadow = value[i]; i++;
                this[i].Resist_Shadow = value[i]; i++;
                this[i].Resist_Shadow = value[i];
            }
        }
        public double[] Resist_Holy
        {
            get
            {
                return new double[] {
                    this[0].Resist_Holy,
                    this[1].Resist_Holy,
                    this[2].Resist_Holy,
                    this[3].Resist_Holy,
                };
            }
            set
            {
                int i = 0;
                this[i].Resist_Holy = value[i]; i++;
                this[i].Resist_Holy = value[i]; i++;
                this[i].Resist_Holy = value[i]; i++;
                this[i].Resist_Holy = value[i];
            }
        }
        // Impedances
        public float[] TimeBossIsInvuln
        {
            get
            {
                return new float[] {
                    this[0].TimeBossIsInvuln,
                    this[1].TimeBossIsInvuln,
                    this[2].TimeBossIsInvuln,
                    this[3].TimeBossIsInvuln,
                };
            }
            set
            {
                int i = 0;
                this[i].TimeBossIsInvuln = value[i]; i++;
                this[i].TimeBossIsInvuln = value[i]; i++;
                this[i].TimeBossIsInvuln = value[i]; i++;
                this[i].TimeBossIsInvuln = value[i];
            }
        }
        // Methods
        public BossHandler BossByVersion(BossHandler.Versions v) { return this[(int)v]; }
        public Attack GenAStandardMelee(BossHandler.TierLevels content) {
            return new Attack {
                Name = "Melee",
                DamageType = ItemDamageType.Physical,
                DamagePerHit = BossHandler.StandardMeleePerHit[(int)content],
                MaxNumTargets = 1f,
                AttackSpeed = 2.0f,
                AttackType = ATTACK_TYPES.AT_MELEE,

                IgnoresMeleeDPS = true,
                IgnoresRangedDPS = true,
                IgnoresHealers = true,

                IsTheDefaultMelee = true,
            };
        }
        #endregion
    }
}
