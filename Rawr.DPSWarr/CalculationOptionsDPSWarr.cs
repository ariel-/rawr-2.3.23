using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Rawr.DPSWarr {
#if !SILVERLIGHT
    [Serializable]
#endif
    public class CalculationOptionsDPSWarr : ICalculationOptionBase, INotifyPropertyChanged
    {
        #region Constructors
        public CalculationOptionsDPSWarr()
        {
            //FuryStance = true;
            AllowFlooring = false;
            SE_UseDur = true;
            UseMarkov = false;
            PTRMode = false;
            //HideBadItems = true;
            HideBadItems_Def = true;
            HideBadItems_Spl = true;
            HideBadItems_PvP = true;
            SurvScale = 1.0f;
            // Maintenance
            _Maintenance = new bool[] {
                true,  // == Rage Gen ==
                    true,  // Berserker Rage
                    true,  // Bloodrage
                false, // == Maintenance ==
                    false, // Shout Choice
                        false, // Battle Shout
                        false, // Commanding Shout
                    false, // Demoralizing Shout
                    false, // Sunder Armor
                    false, // Thunder Clap
                    false, // Hamstring
                true,  // == Periodics ==
                    true,  // Shattering Throw
                    true,  // Sweeping Strikes
                    true,  // DeathWish
                    true,  // Recklessness
                    false, // Enraged Regeneration
                true,  // == Damage Dealers ==
                    true,  // Fury
                        true,  // Whirlwind
                        true,  // Bloodthirst
                        true,  // Bloodsurge
                    true,  // Arms
                        true,  // Bladestorm
                        true,  // Mortal Strike
                        true,  // Rend
                        true,  // Overpower
                        true,  // Taste for Blood
                        true,  // Sudden Death
                        true,  // Slam
                    true,  // <20% Execute Spamming
                true,  // == Rage Dumps ==
                    true,  // Cleave
                    true   // Heroic Strike
            };
            // Latency
            Lag = 179f;
            React = 220f;
            // Boss Options
//#if !RAWR3 && !SILVERLIGHT
            FilterType = "Content";
            Filter = "All";
            BossName = "Custom";
            TargetLevel = 83;
            TargetArmor = StatConversion.NPC_ARMOR[TargetLevel - 80];
            TargetHP = 1000000f;
            Duration = 300f;
            Under20Perc = 0.17f;
            // Rotational Changes
            InBack           = true ; InBackPerc           = 100;
            MultipleTargets  = false; MultipleTargetsPerc  =  25; MultipleTargetsMax  =    3;
            MovingTargets    = false; 
            StunningTargets  = false; 
            FearingTargets   = false; 
            RootingTargets   = false; 
            DisarmingTargets = false; // nonfunctional
            AoETargets       = false; AoETargetsFreq       =  20; AoETargetsDMG       = 5000;
//#endif
        }
        #endregion
        #region Variables
        #region Basics
        /*private bool _FuryStance;
        public bool FuryStance
        {
            get { return _FuryStance; }
            set { _FuryStance = value; OnPropertyChanged("FuryStance"); }
        }*/
        private bool _AllowFlooring;
        public bool AllowFlooring
        {
            get { return _AllowFlooring; }
            set { _AllowFlooring = value; OnPropertyChanged("AllowFlooring"); }
        }
        private bool _SE_UseDur;
        public bool SE_UseDur
        {
            get { return _SE_UseDur; }
            set { _SE_UseDur = value; OnPropertyChanged("SE_UseDur"); }
        }
        private bool _UseMarkov;
        public bool UseMarkov
        {
            get { return _UseMarkov; }
            set { _UseMarkov = value; OnPropertyChanged("UseMarkov"); }
        }
        private bool _PTRMode;
        public bool PTRMode
        {
            get { return _PTRMode; }
            set { _PTRMode = value; OnPropertyChanged("PTRMode"); }
        }
        private bool _HideBadItems_Def;
        public bool HideBadItems_Def
        {
            get { return _HideBadItems_Def; }
            set { _HideBadItems_Def = value; OnPropertyChanged("HideBadItems_Def"); }
        }
        private bool _HideBadItems_Spl;
        public bool HideBadItems_Spl
        {
            get { return _HideBadItems_Spl; }
            set { _HideBadItems_Spl = value; OnPropertyChanged("HideBadItems_Spl"); }
        }
        private bool _HideBadItems_PvP;
        public bool HideBadItems_PvP
        {
            get { return _HideBadItems_PvP; }
            set { _HideBadItems_PvP = value; OnPropertyChanged("HideBadItems_PvP"); }
        }
        private float _SurvScale;
        public float SurvScale
        {
            get { return _SurvScale; }
            set { _SurvScale = value; OnPropertyChanged("SurvScale"); }
        }
        #endregion
        #region Stat Graph
        private bool[] _statsList = new bool[] { true, true, true, true, true, true, true, true, true, true };
        public bool[] StatsList
        {
            get { return _statsList; }
            set { _statsList = value; OnPropertyChanged("StatsList"); }
        }
        private int _StatsIncrement = 100;
        public int StatsIncrement
        {
            get { return _StatsIncrement; }
            set { _StatsIncrement = value; OnPropertyChanged("StatsIncrement"); }
        }
        private string _calculationToGraph = "DPS Rating";
        public string CalculationToGraph
        {
            get { return _calculationToGraph; }
            set { _calculationToGraph = value; OnPropertyChanged("CalculationToGraph"); }
        }
        #endregion
        #region Abilities to Maintain
        private bool[] _Maintenance;
        public enum Maintenances
        {
            _RageGen__ = 0,
            BerserkerRage_,
            Bloodrage_,
            _Maintenance__,
            ShoutChoice_,
            BattleShout_,
            CommandingShout_,
            DemoralizingShout_,
            SunderArmor_,
            ThunderClap_,
            Hamstring_,
            _Periodics__,
            ShatteringThrow_,
            SweepingStrikes_,
            DeathWish_,
            Recklessness_,
            EnragedRegeneration_,
            _DamageDealers__,
            Fury_,
            Whirlwind_,
            Bloodthirst_,
            Bloodsurge_,
            Arms_,
            Bladestorm_,
            MortalStrike_,
            Rend_,
            Overpower_,
            TasteForBlood_,
            SuddenDeath_,
            Slam_,
            ExecuteSpam_,
            _RageDumps__,
            Cleave_,
            HeroicStrike_
        };
        public bool[] Maintenance
        {
            get {
                return _Maintenance ??
                    (_Maintenance = new bool[] {
                        true,  // == Rage Gen ==
                            true,  // Berserker Rage
                            true,  // Bloodrage
                        false, // == Maintenance ==
                            false, // Shout Choice
                                false, // Battle Shout
                                false, // Commanding Shout
                            false, // Demoralizing Shout
                            false, // Sunder Armor
                            false, // Thunder Clap
                            false, // Hamstring
                        true,  // == Periodics ==
                            true,  // Shattering Throw
                            true,  // Sweeping Strikes
                            true,  // DeathWish
                            true,  // Recklessness
                            false, // Enraged Regeneration
                        true,  // == Damage Dealers ==
                            true,  // Fury
                                true,  // Whirlwind
                                true,  // Bloodthirst
                                true,  // Bloodsurge
                            true,  // Arms
                                true,  // Bladestorm
                                true,  // Mortal Strike
                                true,  // Rend
                                true,  // Overpower
                                true,  // Taste for Blood
                                true,  // Sudden Death
                                true,  // Slam
                            true,  // <20% Execute Spamming
                        true,  // == Rage Dumps ==
                            true,  // Cleave
                            true   // Heroic Strike
                    });
            }
            set { _Maintenance = value; OnPropertyChanged("Maintenance"); }
        }
        [XmlIgnore]
        public bool M_BerserkerRage
        {
            get { return Maintenance[(int)Maintenances.BerserkerRage_]; }
            set { Maintenance[(int)Maintenances.BerserkerRage_] = value; OnPropertyChanged("M_BerserkerRage"); }
        }
        [XmlIgnore]
        public bool M_Bloodrage
        {
            get { return Maintenance[(int)Maintenances.Bloodrage_]; }
            set { Maintenance[(int)Maintenances.Bloodrage_] = value; OnPropertyChanged("M_Bloodrage"); }
        }
        [XmlIgnore]
        public bool M_BattleShout
        {
            get { return Maintenance[(int)Maintenances.BattleShout_]; }
            set { Maintenance[(int)Maintenances.BattleShout_] = value; OnPropertyChanged("M_BattleShout"); }
        }
        [XmlIgnore]
        public bool M_CommandingShout
        {
            get { return Maintenance[(int)Maintenances.CommandingShout_]; }
            set { Maintenance[(int)Maintenances.CommandingShout_] = value; OnPropertyChanged("M_CommandingShout"); }
        }
        [XmlIgnore]
        public bool M_DemoralizingShout
        {
            get { return Maintenance[(int)Maintenances.DemoralizingShout_]; }
            set { Maintenance[(int)Maintenances.DemoralizingShout_] = value; OnPropertyChanged("M_DemoralizingShout"); }
        }
        [XmlIgnore]
        public bool M_SunderArmor
        {
            get { return Maintenance[(int)Maintenances.SunderArmor_]; }
            set { Maintenance[(int)Maintenances.SunderArmor_] = value; OnPropertyChanged("M_SunderArmor"); }
        }
        [XmlIgnore]
        public bool M_ThunderClap
        {
            get { return Maintenance[(int)Maintenances.ThunderClap_]; }
            set { Maintenance[(int)Maintenances.ThunderClap_] = value; OnPropertyChanged("M_ThunderClap"); }
        }
        [XmlIgnore]
        public bool M_Hamstring
        {
            get { return Maintenance[(int)Maintenances.Hamstring_]; }
            set { Maintenance[(int)Maintenances.Hamstring_] = value; OnPropertyChanged("M_Hamstring"); }
        }
        [XmlIgnore]
        public bool M_ShatteringThrow
        {
            get { return Maintenance[(int)Maintenances.ShatteringThrow_]; }
            set { Maintenance[(int)Maintenances.ShatteringThrow_] = value; OnPropertyChanged("M_ShatteringThrow"); }
        }
        [XmlIgnore]
        public bool M_SweepingStrikes
        {
            get { return Maintenance[(int)Maintenances.SweepingStrikes_]; }
            set { Maintenance[(int)Maintenances.SweepingStrikes_] = value; OnPropertyChanged("M_SweepingStrikes"); }
        }
        [XmlIgnore]
        public bool M_DeathWish
        {
            get { return Maintenance[(int)Maintenances.DeathWish_]; }
            set { Maintenance[(int)Maintenances.DeathWish_] = value; OnPropertyChanged("M_DeathWish"); }
        }
        [XmlIgnore]
        public bool M_Recklessness
        {
            get { return Maintenance[(int)Maintenances.Recklessness_]; }
            set { Maintenance[(int)Maintenances.Recklessness_] = value; OnPropertyChanged("M_Recklessness"); }
        }
        [XmlIgnore]
        public bool M_EnragedRegeneration
        {
            get { return Maintenance[(int)Maintenances.EnragedRegeneration_]; }
            set { Maintenance[(int)Maintenances.EnragedRegeneration_] = value; OnPropertyChanged("M_EnragedRegeneration"); }
        }
        [XmlIgnore]
        public bool M_Whirlwind
        {
            get { return Maintenance[(int)Maintenances.Whirlwind_]; }
            set { Maintenance[(int)Maintenances.Whirlwind_] = value; OnPropertyChanged("M_Whirlwind"); }
        }
        [XmlIgnore]
        public bool M_Bloodthirst
        {
            get { return Maintenance[(int)Maintenances.Bloodthirst_]; }
            set { Maintenance[(int)Maintenances.Bloodthirst_] = value; OnPropertyChanged("M_Bloodthirst"); }
        }
        [XmlIgnore]
        public bool M_Bloodsurge
        {
            get { return Maintenance[(int)Maintenances.Bloodsurge_]; }
            set { Maintenance[(int)Maintenances.Bloodsurge_] = value; OnPropertyChanged("M_Bloodsurge"); }
        }
        [XmlIgnore]
        public bool M_Bladestorm
        {
            get { return Maintenance[(int)Maintenances.Bladestorm_]; }
            set { Maintenance[(int)Maintenances.Bladestorm_] = value; OnPropertyChanged("M_Bladestorm"); }
        }
        [XmlIgnore]
        public bool M_MortalStrike
        {
            get { return Maintenance[(int)Maintenances.MortalStrike_]; }
            set { Maintenance[(int)Maintenances.MortalStrike_] = value; OnPropertyChanged("M_MortalStrike"); }
        }
        [XmlIgnore]
        public bool M_Rend
        {
            get { return Maintenance[(int)Maintenances.Rend_]; }
            set { Maintenance[(int)Maintenances.Rend_] = value; OnPropertyChanged("M_Rend"); }
        }
        [XmlIgnore]
        public bool M_Overpower
        {
            get { return Maintenance[(int)Maintenances.Overpower_]; }
            set { Maintenance[(int)Maintenances.Overpower_] = value; OnPropertyChanged("M_Overpower"); }
        }
        [XmlIgnore]
        public bool M_TasteForBlood
        {
            get { return Maintenance[(int)Maintenances.TasteForBlood_]; }
            set { Maintenance[(int)Maintenances.TasteForBlood_] = value; OnPropertyChanged("M_TasteForBlood"); }
        }
        [XmlIgnore]
        public bool M_SuddenDeath
        {
            get { return Maintenance[(int)Maintenances.SuddenDeath_]; }
            set { Maintenance[(int)Maintenances.SuddenDeath_] = value; OnPropertyChanged("M_SuddenDeath"); }
        }
        [XmlIgnore]
        public bool M_Slam
        {
            get { return Maintenance[(int)Maintenances.Slam_]; }
            set { Maintenance[(int)Maintenances.Slam_] = value; OnPropertyChanged("M_Slam"); }
        }
        [XmlIgnore]
        public bool M_ExecuteSpam
        {
            get { return Maintenance[(int)Maintenances.ExecuteSpam_]; }
            set { Maintenance[(int)Maintenances.ExecuteSpam_] = value; OnPropertyChanged("M_ExecuteSpam"); }
        }
        [XmlIgnore]
        public bool M_Cleave
        {
            get { return Maintenance[(int)Maintenances.Cleave_]; }
            set { Maintenance[(int)Maintenances.Cleave_] = value; OnPropertyChanged("M_Cleave"); }
        }
        [XmlIgnore]
        public bool M_HeroicStrike
        {
            get { return Maintenance[(int)Maintenances.HeroicStrike_]; }
            set { Maintenance[(int)Maintenances.HeroicStrike_] = value; OnPropertyChanged("M_HeroicStrike"); }
        }
        #endregion
        #region Latency
        private float _Lag;
        public float Lag
        {
            get { return _Lag; }
            set { _Lag = value; _cachedLatency = value / 1000f; OnPropertyChanged("Lag"); }
        }
        private float _React;
        public float React
        {
            get { return _React; }
            set { _React = value; _cachedAllowedReact = Math.Max(0f, (value - 200f) / 1000f); OnPropertyChanged("React"); }
        }
        [XmlIgnore]
        private float _cachedLatency = -1000000f;
        public float Latency { get { return _cachedLatency; } }
        [XmlIgnore]
        private float _cachedAllowedReact = -1000000f;
        public float AllowedReact { get { return _cachedAllowedReact; } }
        public float FullLatency { get { return AllowedReact + Latency; } }
        #endregion
        #region Boss Options
        private string _FilterType;
        public string FilterType
        {
            get { return _FilterType; }
            set { _FilterType = value; OnPropertyChanged("FilterType"); }
        }
        private string _Filter;
        public string Filter
        {
            get { return _Filter; }
            set { _Filter = value; OnPropertyChanged("Filter"); }
        }
        private string _BossName;
        public string BossName
        {
            get { return _BossName; }
            set { _BossName = value; OnPropertyChanged("BossName"); }
        }
        private int _TargetLevel;
        public int TargetLevel
        {
            get { return _TargetLevel; }
            set { _TargetLevel = value; OnPropertyChanged("TargetLevel"); }
        }
        private float _TargetArmor;
        public float TargetArmor
        {
            get { return _TargetArmor; }
            set { _TargetArmor = value; OnPropertyChanged("TargetArmor"); }
        }
        private float _TargetHP;
        public float TargetHP
        {
            get { return _TargetHP; }
            set { _TargetHP = value; OnPropertyChanged("TargetHP"); }
        }
        private float _Duration;
        public float Duration
        {
            get { return _Duration; }
            set { _Duration = value; OnPropertyChanged("Duration"); }
        }
        private float _Under20Perc;
        public float Under20Perc
        {
            get { return _Under20Perc; }
            set { _Under20Perc = value; OnPropertyChanged("Under20Perc"); }
        }
        private bool _InBack;
        public bool InBack
        {
            get { return _InBack; }
            set { _InBack = value; OnPropertyChanged("InBack"); }
        }
        private int _InBackPerc;
        public int InBackPerc
        {
            get { return _InBackPerc; }
            set { _InBackPerc = value; OnPropertyChanged("InBackPerc"); }
        }
        private bool _MultipleTargets;
        public bool MultipleTargets
        {
            get { return _MultipleTargets; }
            set { _MultipleTargets = value; OnPropertyChanged("MultipleTargets"); }
        }
        private int _MultipleTargetsPerc;
        public int MultipleTargetsPerc
        {
            get { return _MultipleTargetsPerc; }
            set { _MultipleTargetsPerc = value; OnPropertyChanged("MultipleTargetsPerc"); }
        }
        private float _MultipleTargetsMax;
        public float MultipleTargetsMax
        {
            get { return _MultipleTargetsMax; }
            set { _MultipleTargetsMax = value; OnPropertyChanged("MultipleTargetsMax"); }
        }
        private bool _MovingTargets;
        public bool MovingTargets
        {
            get { return _MovingTargets; }
            set { _MovingTargets = value; OnPropertyChanged("MovingTargets"); }
        }
        private bool _StunningTargets;
        public bool StunningTargets
        {
            get { return _StunningTargets; }
            set { _StunningTargets = value; OnPropertyChanged("StunningTargets"); }
        }
        private bool _FearingTargets;
        public bool FearingTargets
        {
            get { return _FearingTargets; }
            set { _FearingTargets = value; OnPropertyChanged("FearingTargets"); }
        }
        private bool _RootingTargets;
        public bool RootingTargets
        {
            get { return _RootingTargets; }
            set { _RootingTargets = value; OnPropertyChanged("RootingTargets"); }
        }
        private bool _DisarmingTargets;
        public bool DisarmingTargets
        {
            get { return _DisarmingTargets; }
            set { _DisarmingTargets = value; OnPropertyChanged("DisarmingTargets"); }
        }
        private bool _AoETargets;
        public bool AoETargets
        {
            get { return _AoETargets; }
            set { _AoETargets = value; OnPropertyChanged("AoETargets"); }
        }
        private int _AoETargetsFreq;
        public int AoETargetsFreq
        {
            get { return _AoETargetsFreq; }
            set { _AoETargetsFreq = value; OnPropertyChanged("AoETargetsFreq"); }
        }
        private float _AoETargetsDMG;
        public float AoETargetsDMG
        {
            get { return _AoETargetsDMG; }
            set { _AoETargetsDMG = value; OnPropertyChanged("AoETargetsDMG"); }
        }
        private List<Impedance> _stuns;
        public List<Impedance> Stuns
        {
            get { return _stuns ?? (_stuns = new List<Impedance>()); }
            set { _stuns = value; OnPropertyChanged("Stuns"); }
        }
        private List<Impedance> _moves;
        public List<Impedance> Moves
        {
            get { return _moves ?? (_moves = new List<Impedance>()); }
            set { _moves = value; OnPropertyChanged("Moves"); }
        }
        private List<Impedance> _fears;
        public List<Impedance> Fears
        {
            get { return _fears ?? (_fears = new List<Impedance>()); }
            set { _fears = value; OnPropertyChanged("Fears"); }
        }
        private List<Impedance> _roots;
        public List<Impedance> Roots
        {
            get { return _roots ?? (_roots = new List<Impedance>()); }
            set { _roots = value; OnPropertyChanged("Roots"); }
        }
        private List<Impedance> _disarms;
        public List<Impedance> Disarms
        {
            get { return _disarms ?? (_disarms = new List<Impedance>()); }
            set { _disarms = value; OnPropertyChanged("Disarms"); }
        }
        #endregion
        #endregion
        #region Functions
        public string GetXml() {
            var s = new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsDPSWarr));
            var xml = new StringBuilder();
            var sw = new System.IO.StringWriter(xml);
            s.Serialize(sw, this);
            return xml.ToString();
        }
        #endregion
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(property)); }
        }
        #endregion
    }
}
