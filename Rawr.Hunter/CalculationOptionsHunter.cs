using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Rawr.Hunter
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class CalculationOptionsHunter : ICalculationOptionBase, INotifyPropertyChanged
    {
        #region Hunter Tab
        // ==== Boss Settings ====
#if !RAWR3 && !SILVERLIGHT
        private int _TargetLevel = 83;
        public int TargetLevel
        {
            get { return _TargetLevel; }
            set { _TargetLevel = value; OnPropertyChanged("TargetLevel"); }
        }
        private float _TargetArmor = StatConversion.NPC_ARMOR[3];
        public float TargetArmor
        {
            get { return _TargetArmor; }
            set { _TargetArmor = value; OnPropertyChanged("TargetArmor"); }
        }
        private int _Duration = 300;
        public int Duration
        {
            get { return _Duration; }
            set { _Duration = value; OnPropertyChanged("Duration"); }
        }
        public int _TimeSpentSub20 = 72;
        public int TimeSpentSub20
        {
            get { return _TimeSpentSub20; }
            set { _TimeSpentSub20 = value; OnPropertyChanged("TimeSpentSub20"); }
        }
        public int _TimeSpent35To20 = 54;
        public int TimeSpent35To20
        {
            get { return _TimeSpent35To20; }
            set { _TimeSpent35To20 = value; OnPropertyChanged("TimeSpent35To20"); }
        }
        private bool _MultipleTargets = false;
        public bool MultipleTargets
        {
            get { return _MultipleTargets; }
            set { _MultipleTargets = value; OnPropertyChanged("MultipleTargets"); }
        }
        private float _MultipleTargetsPerc = 0;
        public float MultipleTargetsPerc
        {
            get { return _MultipleTargetsPerc; }
            set { _MultipleTargetsPerc = value; OnPropertyChanged("MultipleTargetsPerc"); }
        }
#endif
        // ==== Fight Settings ====
        #region Latency
        private float _Lag = 150;
        public float Lag
        {
            get { return _Lag; }
            set { _Lag = value; OnPropertyChanged("Lag"); }
        }
        private float _React = 200;
        public float React
        {
            get { return _React; }
            set { _React = value; OnPropertyChanged("React"); }
        }
        public float Latency { get { return Lag / 1000f; } }
        public float AllowedReact { get { return Math.Max(0f, (React - 250f) / 1000f); } }
        public float FullLatency { get { return AllowedReact + Latency; } }
        #endregion
        public float _BossHPPerc = 1.00f;
        public float BossHPPerc
        {
            get { return _BossHPPerc; }
            set { _BossHPPerc = value; OnPropertyChanged("BossHPPerc"); }
        }
        // ==== Hunter Settings ====
        public int _CDCutoff = 0;
        public int CDCutoff
        {
            get { return _CDCutoff; }
            set { _CDCutoff = value; OnPropertyChanged("CDCutoff"); }
        }
        private Aspect _SelectedAspect = Aspect.Dragonhawk;
        public Aspect SelectedAspect
        {
            get { return _SelectedAspect; }
            set { _SelectedAspect = value; OnPropertyChanged("SelectedAspect"); }
        }
        public AspectUsage _AspectUsage = AspectUsage.ViperToOOM;
        public AspectUsage AspectUsage
        {
            get { return _AspectUsage; }
            set { _AspectUsage = value; OnPropertyChanged("AspectUsage"); }
        }
        public bool _UseBeastDuringBestialWrath = true;
        public bool UseBeastDuringBestialWrath
        {
            get { return _UseBeastDuringBestialWrath; }
            set { _UseBeastDuringBestialWrath = value; OnPropertyChanged("UseBeastDuringBestialWrath"); }
        }
        public bool _UseRotationTest = true;
        public bool UseRotationTest
        {
            get { return _UseRotationTest; }
            set { _UseRotationTest = value; OnPropertyChanged("UseRotationTest"); }
        }
        public bool _RandomizeProcs = false;
        public bool RandomizeProcs
        {
            get { return _RandomizeProcs; }
            set { _RandomizeProcs = value; OnPropertyChanged("RandomizeProcs"); }
        }
        #endregion

        #region Rotations Tab
        public int _PriorityIndex1 = 0;
        public int PriorityIndex1
        {
            get { return _PriorityIndex1; }
            set { _PriorityIndex1 = value; OnPropertyChanged("PriorityIndex1"); }
        }
        public int _PriorityIndex2 = 0;
        public int PriorityIndex2
        {
            get { return _PriorityIndex2; }
            set { _PriorityIndex2 = value; OnPropertyChanged("PriorityIndex2"); }
        }
        public int _PriorityIndex3 = 0;
        public int PriorityIndex3
        {
            get { return _PriorityIndex3; }
            set { _PriorityIndex3 = value; OnPropertyChanged("PriorityIndex3"); }
        }
        public int _PriorityIndex4 = 0;
        public int PriorityIndex4
        {
            get { return _PriorityIndex4; }
            set { _PriorityIndex4 = value; OnPropertyChanged("PriorityIndex4"); }
        }
        public int _PriorityIndex5 = 0;
        public int PriorityIndex5
        {
            get { return _PriorityIndex5; }
            set { _PriorityIndex5 = value; OnPropertyChanged("PriorityIndex5"); }
        }
        public int _PriorityIndex6 = 0;
        public int PriorityIndex6
        {
            get { return _PriorityIndex6; }
            set { _PriorityIndex6 = value; OnPropertyChanged("PriorityIndex6"); }
        }
        public int _PriorityIndex7 = 0;
        public int PriorityIndex7
        {
            get { return _PriorityIndex7; }
            set { _PriorityIndex7 = value; OnPropertyChanged("PriorityIndex7"); }
        }
        public int _PriorityIndex8 = 0;
        public int PriorityIndex8
        {
            get { return _PriorityIndex8; }
            set { _PriorityIndex8 = value; OnPropertyChanged("PriorityIndex8"); }
        }
        public int _PriorityIndex9 = 0;
        public int PriorityIndex9
        {
            get { return _PriorityIndex9; }
            set { _PriorityIndex9 = value; OnPropertyChanged("PriorityIndex9"); }
        }
        public int _PriorityIndex10 = 0;
        public int PriorityIndex10
        {
            get { return _PriorityIndex10; }
            set { _PriorityIndex10 = value; OnPropertyChanged("PriorityIndex10"); }
        }
        [XmlIgnore]
        public int[] PriorityIndexes
        {
            get
            {
                int[] _PriorityIndexes = { PriorityIndex1, PriorityIndex2, PriorityIndex3, PriorityIndex4, PriorityIndex5, 
                                           PriorityIndex6, PriorityIndex7, PriorityIndex8, PriorityIndex9, PriorityIndex10 };
                return _PriorityIndexes;
            }
            set
            {
                int[] _PriorityIndexes = value;
                PriorityIndex1 = _PriorityIndexes[0];
                PriorityIndex2 = _PriorityIndexes[1];
                PriorityIndex3 = _PriorityIndexes[2];
                PriorityIndex4 = _PriorityIndexes[3];
                PriorityIndex5 = _PriorityIndexes[4];
                PriorityIndex6 = _PriorityIndexes[5];
                PriorityIndex7 = _PriorityIndexes[6];
                PriorityIndex8 = _PriorityIndexes[7];
                PriorityIndex9 = _PriorityIndexes[8];
                PriorityIndex10 = _PriorityIndexes[9];
            }
        }
        #endregion

        #region Pet Tab
        private int _petLevel = 80; // Not Editable
        public int PetLevel {
            get { return _petLevel; }
            set { _petLevel = value; OnPropertyChanged("PetLevel"); }
        }
        [XmlIgnore]
        public int _SelectedArmoryPet = 0;
        public int SelectedArmoryPet {
            get { return _SelectedArmoryPet; }
            set { _SelectedArmoryPet = value; OnPropertyChanged("SelectedArmoryPet"); }
        }
        private PetHappiness _petHappiness = PetHappiness.Happy; // Not Editable
        public PetHappiness PetHappinessLevel {
            get { return _petHappiness; }
            set { _petHappiness = value; OnPropertyChanged("PetHappinessLevel"); }
        }
#if RAWR3 || SILVERLIGHT
        [XmlIgnore]
        private PetTalents _PetTalents;
        public PetTalents PetTalents
        {
            get { return _PetTalents ?? (_PetTalents = new PetTalents()); }
            set { _PetTalents = value; OnPropertyChanged("PetTalents"); }
        }
#else
        [XmlIgnore]
        private PetTalentTreeData _PetTalents;
        [XmlIgnore]
        public PetTalentTreeData PetTalents {
            get { return _PetTalents ?? (_PetTalents = new PetTalentTreeData()); }
            set { _PetTalents = value; OnPropertyChanged("PetTalents"); }
        }
#endif
        private string _petTalents;
        public string petTalents {
            get {
                if ( String.IsNullOrEmpty(_petTalents) && _PetTalents != null) { _petTalents = _PetTalents.ToString(); }
                return _petTalents;
            }
            set { _petTalents = value; }
        }
        private PetFamily _petFamily = PetFamily.Cat;
        public PetFamily PetFamily {
            get { return _petFamily; }
            set { _petFamily = value; OnPropertyChanged("PetFamily"); }
        }
        #region Skill Priorities
        public PetAttacks _PetPriority1 = PetAttacks.Growl;
        public PetAttacks PetPriority1
        {
            get { return _PetPriority1; }
            set { _PetPriority1 = value; OnPropertyChanged("PetPriority1"); }
        }
        public PetAttacks _PetPriority2 = PetAttacks.Bite;
        public PetAttacks PetPriority2
        {
            get { return _PetPriority2; }
            set { _PetPriority2 = value; OnPropertyChanged("PetPriority2"); }
        }
        public PetAttacks _PetPriority3 = PetAttacks.None;
        public PetAttacks PetPriority3
        {
            get { return _PetPriority3; }
            set { _PetPriority3 = value; OnPropertyChanged("PetPriority3"); }
        }
        public PetAttacks _PetPriority4 = PetAttacks.None;
        public PetAttacks PetPriority4
        {
            get { return _PetPriority4; }
            set { _PetPriority4 = value; OnPropertyChanged("PetPriority4"); }
        }
        public PetAttacks _PetPriority5 = PetAttacks.None;
        public PetAttacks PetPriority5
        {
            get { return _PetPriority5; }
            set { _PetPriority5 = value; OnPropertyChanged("PetPriority5"); }
        }
        public PetAttacks _PetPriority6 = PetAttacks.None;
        public PetAttacks PetPriority6
        {
            get { return _PetPriority6; }
            set { _PetPriority6 = value; OnPropertyChanged("PetPriority6"); }
        }
        public PetAttacks _PetPriority7 = PetAttacks.None;
        public PetAttacks PetPriority7
        {
            get { return _PetPriority7; }
            set { _PetPriority7 = value; OnPropertyChanged("PetPriority7"); }
        }
        #endregion
        #region Buffs
        [XmlIgnore]
        private List<Buff> _petActiveBuffs;
        [XmlElement("petActiveBuffs")]
        public List<string> _petActiveBuffsXml = new List<string>();
        [XmlIgnore]
        public List<Buff> petActiveBuffs
        {
            get { return _petActiveBuffs ?? (_petActiveBuffs = new List<Buff>()); }
            set { _petActiveBuffs = value; OnPropertyChanged("PetActiveBuffs"); }
        }
        #endregion
        #endregion

        #region Misc Tab
        // ==== Misc ====
        private bool _HideBadItems_Spl = true;
        public bool HideBadItems_Spl
        {
            get { return _HideBadItems_Spl; }
            set { _HideBadItems_Spl = value; OnPropertyChanged("HideBadItems_Spl"); }
        }
        private bool _HideBadItems_PvP = true;
        public bool HideBadItems_PvP
        {
            get { return _HideBadItems_PvP; }
            set { _HideBadItems_PvP = value; OnPropertyChanged("HideBadItems_PvP"); }
        }
        private bool _PTRMode = false;
        public bool PTRMode
        {
            get { return _PTRMode; }
            set { _PTRMode = value; OnPropertyChanged("PTRMode"); }
        }
        private float _SurvScale = 1.0f;
        public float SurvScale
        {
            get { return _SurvScale; }
            set { _SurvScale = value; OnPropertyChanged("SurvScale"); }
        }
        private bool _SE_UseDur = true;
        public bool SE_UseDur
        {
            get { return _SE_UseDur; }
            set { _SE_UseDur = value; OnPropertyChanged("SE_UseDur"); }
        }
        // ==== Stats Graph ====
        private bool[] _statsList = new bool[] { true, true, true, true, true, true };
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
        private string _calculationToGraph = "Overall Rating";
        public string CalculationToGraph
        {
            get { return _calculationToGraph; }
            set { _calculationToGraph = value; OnPropertyChanged("CalculationToGraph"); }
        }
        #endregion

        #region Other Options not on the pane
        public float waitForCooldown = 0.8f; // not editable
        public bool interleaveLAL = false; // not editable
        public bool prioritiseArcAimedOverSteady = true; // not editable
        public bool debugShotRotation = false; // not editable

        private bool _useKillCommand = true;
        private float _gcdsToLayImmoTrap = 2.0f; // not editable
        private float _gcdsToLayExploTrap = 2.0f; // not editable
        private float _gcdsToVolley = 4.0f; // not editable, 6 seconds
        private Shots _LALShotToUse = Shots.ExplosiveShot; // not editable
        private int _LALShotsReplaced = 2; // not editable
        private float _LALProcChance = 2; // not editable

        public bool useKillCommand { get { return _useKillCommand; } set { _useKillCommand = value; OnPropertyChanged("useKillCommand"); } }
        public float gcdsToLayImmoTrap { get { return _gcdsToLayImmoTrap; } set { _gcdsToLayImmoTrap = value; OnPropertyChanged("gcdsToLayImmoTrap"); } }
        public float gcdsToLayExploTrap { get { return _gcdsToLayExploTrap; } set { _gcdsToLayExploTrap = value; OnPropertyChanged("gcdsToLayExploTrap"); } }
        public float gcdsToVolley { get { return _gcdsToVolley; } set { _gcdsToVolley = value; OnPropertyChanged("gcdsToVolley"); } }
        public Shots LALShotToUse { get { return _LALShotToUse; } set { _LALShotToUse = value; OnPropertyChanged("LALShotToUse"); } }
        public int LALShotsReplaced { get { return _LALShotsReplaced; } set { _LALShotsReplaced = value; OnPropertyChanged("LALShotsReplaced"); } }
        public float LALProcChance { get { return _LALProcChance; } set { _LALProcChance = value; OnPropertyChanged("LALProcChance"); } }
        #endregion

        #region Shots
        [XmlIgnore]
        public static Shot None = new Shot(0, "None");
        [XmlIgnore]
        public static Shot AimedShot = new Shot(1, "Aimed Shot");
        [XmlIgnore]
        public static Shot ArcaneShot = new Shot(2, "Arcane Shot");
        [XmlIgnore]
        public static Shot MultiShot = new Shot(3, "Multi-Shot");
        [XmlIgnore]
        public static Shot SerpentSting = new Shot(4, "Serpent Sting");
        [XmlIgnore]
        public static Shot ScorpidSting = new Shot(5, "Scorpid Sting");
        [XmlIgnore]
        public static Shot ViperSting = new Shot(6, "Viper Sting");
        [XmlIgnore]
        public static Shot SilencingShot = new Shot(7, "Silencing Shot");
        [XmlIgnore]
        public static Shot SteadyShot = new Shot(8, "Steady Shot");
        [XmlIgnore]
        public static Shot KillShot = new Shot(9, "Kill Shot");
        [XmlIgnore]
        public static Shot ExplosiveShot = new Shot(10, "Explosive Shot");
        [XmlIgnore]
        public static Shot BlackArrow = new Shot(11, "Black Arrow");
        [XmlIgnore]
        public static Shot ImmolationTrap = new Shot(12, "Immolation Trap");
        [XmlIgnore]
        public static Shot ExplosiveTrap = new Shot(13, "Explosive Trap");
        [XmlIgnore]
        public static Shot FreezingTrap = new Shot(14, "Freezing Trap");
        [XmlIgnore]
        public static Shot FrostTrap = new Shot(15, "Frost Trap");
        [XmlIgnore]
        public static Shot Volley = new Shot(16, "Volley");
        [XmlIgnore]
        public static Shot ChimeraShot = new Shot(17, "Chimera Shot");
        [XmlIgnore]
        public static Shot RapidFire = new Shot(18, "Rapid Fire");
        [XmlIgnore]
        public static Shot Readiness = new Shot(19, "Readiness");
        [XmlIgnore]
        public static Shot BeastialWrath = new Shot(20, "Beastial Wrath");
        [XmlIgnore]
        private static List<Shot> _ShotList = null;
        [XmlIgnore]
        public static List<Shot> ShotList
        {
            get
            {
                return _ShotList ?? (new List<Shot>() {
                        None,
                        AimedShot,
                        ArcaneShot,
                        MultiShot,
                        SerpentSting,
                        ScorpidSting,
                        ViperSting,
                        SilencingShot,
                        SteadyShot,
                        KillShot,
                        ExplosiveShot,
                        BlackArrow,
                        ImmolationTrap,
                        ExplosiveTrap,
                        FreezingTrap,
                        FrostTrap,
                        Volley,
                        ChimeraShot,
                        RapidFire,
                        Readiness,
                        BeastialWrath,
                    });
            }
        }
        #region Default Shot Groups
        [XmlIgnore]
        public static readonly ShotGroup Marksman = new ShotGroup("Marksman", new List<Shot>() {
                RapidFire,
                Readiness,
                SerpentSting,
                ChimeraShot,
                KillShot,
                AimedShot,
                SilencingShot,
                SteadyShot,
                None,
                None,
        });
        [XmlIgnore]
        public static readonly ShotGroup BeastMaster = new ShotGroup("Beast Master", new List<Shot>() {
                RapidFire,
                BeastialWrath,
                KillShot,
                AimedShot,
                ArcaneShot,
                SerpentSting,
                SteadyShot,
                None,
                None,
                None,
        });
        [XmlIgnore]
        public static readonly ShotGroup Survival = new ShotGroup("Survival", new List<Shot>() {
                RapidFire,
                KillShot,
                ExplosiveShot,
                SerpentSting,
                BlackArrow,
                AimedShot,
                SteadyShot,
                None,
                None,
                None,
        });
        #endregion
        #endregion

        #region Pet Functions
        public static PetFamilyTree PetFamilyToPetFamilyTree(PetFamily family)
        {
            switch (family)
            {
                case PetFamily.Bat:
                case PetFamily.BirdOfPrey:
                case PetFamily.Chimaera:
                case PetFamily.Dragonhawk:
                case PetFamily.NetherRay:
                case PetFamily.Ravager:
                case PetFamily.Serpent:
                case PetFamily.Silithid:
                case PetFamily.Spider:
                case PetFamily.SporeBat:
                case PetFamily.WindSerpent:
                    return PetFamilyTree.Cunning;

                case PetFamily.Bear:
                case PetFamily.Boar:
                case PetFamily.Crab:
                case PetFamily.Crocolisk:
                case PetFamily.Gorilla:
                case PetFamily.Rhino:
                case PetFamily.Scorpid:
                case PetFamily.Turtle:
                case PetFamily.WarpStalker:
                case PetFamily.Worm:
                    return PetFamilyTree.Tenacity;

                case PetFamily.CarrionBird:
                case PetFamily.Cat:
                case PetFamily.CoreHound:
                case PetFamily.Devilsaur:
                case PetFamily.Hyena:
                case PetFamily.Moth:
                case PetFamily.Raptor:
                case PetFamily.SpiritBeast:
                case PetFamily.Tallstrider:
                case PetFamily.Wasp:
                case PetFamily.Wolf:
                    return PetFamilyTree.Ferocity;
            }

            // hmmm!
            return PetFamilyTree.None;
        }
        #endregion

        /*private bool[] _Maintenance;
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
            get
            {
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
        }*/

        #region ICalculationOptionBase Members
        public string GetXml()
        {
            _petActiveBuffsXml = new List<string>(_petActiveBuffs.ConvertAll(buff => buff.Name));

            XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsHunter));
            StringBuilder xml = new StringBuilder();
            StringWriter writer = new StringWriter(xml);
            serializer.Serialize(writer, this);
            return xml.ToString();
        }
        #endregion
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
        #endregion
    }
    public class Shot
    {
        public Shot(int index, string name) { Index = index; Name = name; }

        public int Index = -1;
        public string Name = "Invalid";

        public override string ToString() { return Name; }
        public int ToInt() { return Index; }
    }
    public class ShotGroup
    {
        public ShotGroup(string name) { Name = name; }
        public ShotGroup(string name, ShotGroup sg) { Name = name; ShotList = sg.ShotList; }
        public ShotGroup(string name, List<Shot> shotList) { Name = name; ShotList = shotList; }
        public string Name = "Invalid";
        private List<Shot> _ShotList = null;
        public List<Shot> ShotList {
            get { return _ShotList ?? new List<Shot>() { }; }
            set { _ShotList = value; }
        }
        public bool Equals(List<Shot> shots2Compare) { return (ShotList == shots2Compare); }
    }
}
