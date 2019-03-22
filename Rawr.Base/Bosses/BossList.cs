using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Rawr.Bosses;

namespace Rawr {
    public class BossList {
        // Constructors
        public BossList()
        {
            #region Instance Declarations
            #region ==== Tier 7 Content ====
            // Naxxramas
            AnubRekhan anubrekhan = new AnubRekhan();
            GrandWidowFaerlina grandwidow = new GrandWidowFaerlina();
            Maexxna maexxna = new Maexxna();
            NoththePlaguebringer noth = new NoththePlaguebringer();
            HeigantheUnclean heigantheUnclean = new HeigantheUnclean();
            Loatheb loatheb = new Loatheb();
            InstructorRazuvious instructorRazuvious = new InstructorRazuvious();
            GothiktheHarvester gothiktheHarvester = new GothiktheHarvester();
            FourHorsemen fourHorsemen = new FourHorsemen();
            Patchwerk patchwerk = new Patchwerk();
            Grobbulus grobbulus = new Grobbulus();
            Gluth gluth = new Gluth();
            Thaddius thaddius = new Thaddius();
            Sapphiron sapphiron = new Sapphiron();
            KelThuzad kelThuzad = new KelThuzad();
            // The Obsidian Sanctum
            Shadron shadron = new Shadron();
            Tenebron tenebron = new Tenebron();
            Vesperon vesperon = new Vesperon();
            Sartharion sartharion = new Sartharion();
            // The Vault of Archavon
            ArchavonTheStoneWatcher archavon = new ArchavonTheStoneWatcher();
            // The Eye of Eternity
            Malygos malygos = new Malygos();
            #endregion
            #region ==== Tier 8 Content ====
            // Ulduar
            IgnistheFurnaceMaster IgnistheFurnaceMaster = new IgnistheFurnaceMaster();
            Razorscale Razorscale = new Razorscale();
            XT002Deconstructor XT002Deconstructor = new XT002Deconstructor();
            AssemblyofIron AssemblyofIron = new AssemblyofIron();
            Kologarn Kologarn = new Kologarn();
            Auriaya Auriaya = new Auriaya();
            Mimiron Mimiron = new Mimiron();
            Freya Freya = new Freya();
            Thorim Thorim = new Thorim();
            Hodir Hodir = new Hodir();
            GeneralVezax GeneralVezax = new GeneralVezax();
            YoggSaron YoggSaron = new YoggSaron();
            AlgalontheObserver AlgalontheObserver = new AlgalontheObserver();
            // Vault of Archavon
            EmalonTheStormWatcher EmalonTheStormWatcher = new EmalonTheStormWatcher();
            #endregion
            #region ==== Tier 9 Content ====
            // The Vault of Archavon
            KoralonTheFlameWatcher KoralonTheFlameWatcher = new KoralonTheFlameWatcher();
            // Trial of the Crusader
            NorthrendBeasts NorthrendBeasts = new NorthrendBeasts();
            LordJaraxxus LordJaraxxus = new LordJaraxxus();
            FactionChampions FactionChampions = new FactionChampions();
            TwinValkyr TwinValkyr = new TwinValkyr();
            Anubarak Anubarak = new Anubarak();
            #endregion
            #region ==== Tier 10 Content ====
            // The Vault of Archavon
            ToravonTheIceWatcher ToravonTheIceWatcher = new ToravonTheIceWatcher();
            // Icecrown Citadel
            LordMarrowgar LordMarrowgar = new LordMarrowgar();
            LadyDeathwhisper LadyDeathwhisper = new LadyDeathwhisper();
            DeathbringerSaurfang DeathbringerSaurfang = new DeathbringerSaurfang();
            Festergut Festergut = new Festergut();
            Rotface Rotface = new Rotface();
            ProfessorPutricide ProfessorPutricide = new ProfessorPutricide();
            BloodPrinceCouncil BloodPrinceCouncil = new BloodPrinceCouncil();
            BloodQueenLanathel BloodQueenLanathel = new BloodQueenLanathel();
            ValathriaDreamwalker ValathriaDreamwalker = new ValathriaDreamwalker();
            Sindragosa Sindragosa = new Sindragosa();
            TheLichKing TheLichKing = new TheLichKing();
            // Ruby Sanctum
            Baltharus Baltharus = new Baltharus();
            SavianaRagefire SavianaRagefire = new SavianaRagefire();
            GeneralZarithrian GeneralZarithrian = new GeneralZarithrian();
            Halion Halion = new Halion();
            #endregion
            #endregion
            list = new BossHandler[] {
                #region ==== Tier 7 Content ====
                // Naxxramas
                anubrekhan[0],anubrekhan[1],
                grandwidow[0],grandwidow[1],
                maexxna[0],maexxna[1],
                noth[0],noth[1],
                heigantheUnclean[0],heigantheUnclean[1],
                loatheb[0],loatheb[1],
                instructorRazuvious[0],instructorRazuvious[1],
                gothiktheHarvester[0],gothiktheHarvester[1],
                fourHorsemen[0],fourHorsemen[1],
                patchwerk[0],patchwerk[1],
                grobbulus[0],grobbulus[1],
                gluth[0],gluth[1],
                thaddius[0],thaddius[1],
                sapphiron[0],sapphiron[1],
                kelThuzad[0],kelThuzad[1],
                // The Obsidian Sanctum
                shadron[0],shadron[1],
                tenebron[0],tenebron[1],
                vesperon[0],vesperon[1],
                sartharion[0],sartharion[1],
                // Vault of Archavon
                archavon[0],archavon[1],
                // The Eye of Eternity
                malygos[0],malygos[1],
                #endregion
                #region ==== Tier 8 Content ====
                // Ulduar
                IgnistheFurnaceMaster[0],IgnistheFurnaceMaster[1],
                Razorscale[0],Razorscale[1],
                XT002Deconstructor[0],XT002Deconstructor[1],
                AssemblyofIron[0],AssemblyofIron[1],
                Kologarn[0],Kologarn[1],
                Auriaya[0],Auriaya[1],
                Mimiron[0],Mimiron[1],
                Freya[0],Freya[1],
                Thorim[0],Thorim[1],
                Hodir[0],Hodir[1],
                GeneralVezax[0],GeneralVezax[1],
                YoggSaron[0],YoggSaron[1],
                AlgalontheObserver[0],AlgalontheObserver[1],
                // Vault of Archavon
                EmalonTheStormWatcher[0], EmalonTheStormWatcher[1],
                #endregion
                #region ==== Tier 9 Content ====
                // Vault of Archavon
                KoralonTheFlameWatcher[0],KoralonTheFlameWatcher[1],
                // Trial of the Crusader
                NorthrendBeasts[0],NorthrendBeasts[1],NorthrendBeasts[2],NorthrendBeasts[3],
                LordJaraxxus[0],LordJaraxxus[1],LordJaraxxus[2],LordJaraxxus[3],
                FactionChampions[0],FactionChampions[1],FactionChampions[2],FactionChampions[3],
                TwinValkyr[0],TwinValkyr[1],TwinValkyr[2],TwinValkyr[3],
                Anubarak[0],Anubarak[1],Anubarak[2],Anubarak[3],
                #endregion
                #region ==== Tier 10 Content ====
                // Vault of Archavon
                ToravonTheIceWatcher[0],ToravonTheIceWatcher[1],
                // Icecrown Citadel
                LordMarrowgar[0],LordMarrowgar[1],LordMarrowgar[2],LordMarrowgar[3],
                LadyDeathwhisper[0],LadyDeathwhisper[1],LadyDeathwhisper[2],LadyDeathwhisper[3],
                DeathbringerSaurfang[0], DeathbringerSaurfang[1], DeathbringerSaurfang[2], DeathbringerSaurfang[3],
                Festergut[0], Festergut[1], Festergut[2], Festergut[3],
                Rotface[0], Rotface[1], Rotface[2], Rotface[3],
                ProfessorPutricide[0], ProfessorPutricide[1], ProfessorPutricide[2], ProfessorPutricide[3],
                BloodPrinceCouncil[0], BloodPrinceCouncil[1], BloodPrinceCouncil[2], BloodPrinceCouncil[3],
                BloodQueenLanathel[0], BloodQueenLanathel[1], BloodQueenLanathel[2], BloodQueenLanathel[3],
                ValathriaDreamwalker[0], ValathriaDreamwalker[1], ValathriaDreamwalker[2], ValathriaDreamwalker[3],
                Sindragosa[0], Sindragosa[1], Sindragosa[2], Sindragosa[3],
                TheLichKing[0], TheLichKing[1], TheLichKing[2], TheLichKing[3],
                // Ruby Sanctum
                Baltharus[0],Baltharus[1],Baltharus[2],Baltharus[3],
                SavianaRagefire[0],SavianaRagefire[1],SavianaRagefire[2],SavianaRagefire[3],
                GeneralZarithrian[0],GeneralZarithrian[1],GeneralZarithrian[2],GeneralZarithrian[3],
                Halion[0],Halion[1],Halion[2],Halion[3],
                #endregion
            };
            TheEZModeBoss  = GenTheEZModeBoss(list);
            TheAvgBoss     = GenTheAvgBoss(list);
            TheHardestBoss = GenTheHardestBoss(list);
            // This one is for filtered lists, defaults to the full list above
            calledList = GenCalledList(FilterType.Content,"All");
        }
        #region Variables
        /// <summary>Global setting, the Character's level should always be 80 until the next expansion</summary>
        public const int NormCharLevel = 80;
        /// <summary>The primary list, this hold all bosses regardless of filters</summary>
        public BossHandler[] list;
        /// <summary>This is what modules actually see and is based upon current filters</summary>
        public BossHandler[] calledList;
        /// <summary>Variable for storing Damage Type (Physical, Nature, Holy, etc)</summary>
        private ItemDamageType[] dts = null;
        private ItemDamageType[] DamageTypes {
            get
            {
                return dts ?? (new ItemDamageType[] { 
                    ItemDamageType.Physical,
                    ItemDamageType.Frost,
                    ItemDamageType.Fire,
                    ItemDamageType.Nature,
                    ItemDamageType.Arcane,
                    ItemDamageType.Shadow,
                    ItemDamageType.Holy,
                });
            }
        }
        /// <summary>Checks all the bosses to find the easiest of each stat and combines them to a single boss. Does NOT pick the easiest boss in the list but MAKES a new one. This IS NOT affected by filters.</summary>
        public BossHandler TheEZModeBoss;
        /// <summary>Checks all the bosses to total up stats and average them out and combines them to a single boss, this is what most users should base their characters against. This IS NOT affected by filters.</summary>
        public BossHandler TheAvgBoss;
        /// <summary>Checks all the bosses to find the worst of each stat and combines them to a single boss. Does NOT pick the hardest boss in the list but MAKES a new one. This IS NOT affected by filters.</summary>
        public BossHandler TheHardestBoss;
        /// <summary>Checks all the bosses to find the easiest of each stat and combines them to a single boss. Does NOT pick the easiest boss in the list but MAKES a new one. This IS affected by filters.</summary>
        public BossHandler TheEZModeBoss_Called;
        /// <summary>Checks all the bosses to total up stats and average them out and combines them to a single boss, this is what most users should base their characters against. This IS affected by filters.</summary>
        public BossHandler TheAvgBoss_Called;
        /// <summary>Checks all the bosses to find the worst of each stat and combines them to a single boss. Does NOT pick the hardest boss in the list but MAKES a new one. This IS affected by filters.</summary>
        public BossHandler TheHardestBoss_Called;
        #endregion
        #region Functions
        // Called List Generation and Interaction
        public enum FilterType { Content=0, Instance, Version, Name }
        public BossHandler[] GenCalledList(FilterType ftype, string Filter) {
            if (Filter.Equals("All",StringComparison.OrdinalIgnoreCase)) {
                // Resets the calledList to the full, unfiltered List
                TheEZModeBoss_Called  = TheEZModeBoss;
                TheAvgBoss_Called     = TheAvgBoss;
                TheHardestBoss_Called = TheHardestBoss;
                return calledList = list;
            }
            // Generate a list based upon the specialized Filter, only 1 thing can be compared at a time though
            List<BossHandler> retList = new List<BossHandler>();
            foreach (BossHandler boss in list) {
                switch (ftype) {
                    case FilterType.Content:  { if (boss.ContentString.Equals( Filter,StringComparison.OrdinalIgnoreCase)) { retList.Add(boss); } break; }
                    case FilterType.Instance: { if (boss.Instance.Equals(Filter,StringComparison.OrdinalIgnoreCase)) { retList.Add(boss); } break; }
                    case FilterType.Version:  { if (boss.VersionString.Equals( Filter,StringComparison.OrdinalIgnoreCase)) { retList.Add(boss); } break; }
                    case FilterType.Name:     { if (boss.Name.Equals(    Filter,StringComparison.OrdinalIgnoreCase)) { retList.Add(boss); } break; }
                    default: { /*Invalid type, do nothing*/ break; }
                }
            }
            BossHandler[] retList2 = retList.ToArray();
            // Gen the special bosses based upon this filtered list
            TheEZModeBoss_Called  = GenTheEZModeBoss(retList2);
            TheAvgBoss_Called     = GenTheAvgBoss(retList2);
            TheHardestBoss_Called = GenTheHardestBoss(retList2);
            return calledList = retList2;
        }
        // Name Calling
        public List<string> GetBossNames() {
            List<string> names = new List<string>() { };
            names.Add(TheEZModeBoss.Name);
            names.Add(TheAvgBoss.Name);
            names.Add(TheHardestBoss.Name);
            foreach (BossHandler boss in calledList) {
                names.Add(boss.Name);
            }
            return names;
        }
        public string[] GetBossNamesAsArray() { return GetBossNames().ToArray(); }
        public List<string> GetBetterBossNames() {
            List<string> names = new List<string>() { };
            names.Add(TheEZModeBoss.Name);
            names.Add(TheAvgBoss.Name);
            names.Add(TheHardestBoss.Name);
            foreach (BossHandler boss in calledList) {
                string name = boss.ContentString + " : " + boss.Instance + " (" + boss.VersionString + ") " + boss.Name;
                names.Add(name);
            }
            return names;
        }
        public string[] GetBetterBossNamesAsArray() { return GetBetterBossNames().ToArray(); }
        public BossHandler GetBossFromName(string name) {
            BossHandler retBoss = new BossHandler();
            if      (TheEZModeBoss_Called.Name  == name) { retBoss = TheEZModeBoss_Called;  }
            else if (TheAvgBoss_Called.Name     == name) { retBoss = TheAvgBoss_Called;     }
            else if (TheHardestBoss_Called.Name == name) { retBoss = TheHardestBoss_Called; }
            else {
                foreach (BossHandler boss in calledList) {
                    if(boss.Name == name){
                        retBoss = boss;
                        break;
                    }
                }
            }
            return retBoss;
        }
        public BossHandler GetBossFromBetterName(string name) {
            BossHandler retBoss = new BossHandler();
            if      (TheEZModeBoss_Called.Name  == name) { retBoss = TheEZModeBoss_Called;  }
            else if (TheAvgBoss_Called.Name     == name) { retBoss = TheAvgBoss_Called;     }
            else if (TheHardestBoss_Called.Name == name) { retBoss = TheHardestBoss_Called; }
            else {
                foreach (BossHandler boss in calledList) {
                    string checkName = boss.ContentString + " : " + boss.Instance + " (" + boss.VersionString + ") " + boss.Name;
                    if(checkName == name){
                        retBoss = boss;
                        break;
                    }
                }
            }
            return retBoss;
        }
        public List<string> GetFilterList(FilterType ftype) {
            List<string> names = new List<string>() { };
            switch (ftype) {
                case FilterType.Content: {
                    if (!names.Contains(TheEZModeBoss_Called.ContentString)) { names.Add(TheEZModeBoss_Called.ContentString); }
                    if (!names.Contains(TheAvgBoss_Called.ContentString)) { names.Add(TheAvgBoss_Called.ContentString); }
                    if (!names.Contains(TheHardestBoss_Called.ContentString)) { names.Add(TheHardestBoss_Called.ContentString); }
                    foreach (BossHandler boss in calledList) { if (!names.Contains(boss.ContentString)) { names.Add(boss.ContentString); } }
                    break;
                }
                case FilterType.Instance: {
                    if (!names.Contains(TheEZModeBoss_Called.Instance)) { names.Add(TheEZModeBoss_Called.Instance); }
                    if (!names.Contains(TheAvgBoss_Called.Instance)) { names.Add(TheAvgBoss_Called.Instance); }
                    if (!names.Contains(TheHardestBoss_Called.Instance)) { names.Add(TheHardestBoss_Called.Instance); }
                    foreach (BossHandler boss in calledList) { if (!names.Contains(boss.Instance)) { names.Add(boss.Instance); } }
                    break;
                }
                case FilterType.Name: {
                    if (!names.Contains(TheEZModeBoss_Called.Name)) { names.Add(TheEZModeBoss_Called.Name); }
                    if (!names.Contains(TheAvgBoss_Called.Name)) { names.Add(TheAvgBoss_Called.Name); }
                    if (!names.Contains(TheHardestBoss_Called.Name)) { names.Add(TheHardestBoss_Called.Name); }
                    foreach (BossHandler boss in calledList) { if (!names.Contains(boss.Name)) { names.Add(boss.Name); } }
                    break;
                }
                case FilterType.Version: {
                    if (!names.Contains(TheEZModeBoss_Called.VersionString)) { names.Add(TheEZModeBoss_Called.VersionString); }
                    if (!names.Contains(TheAvgBoss_Called.VersionString)) { names.Add(TheAvgBoss_Called.VersionString); }
                    if (!names.Contains(TheHardestBoss_Called.VersionString)) { names.Add(TheHardestBoss_Called.VersionString); }
                    foreach (BossHandler boss in calledList) { if (!names.Contains(boss.VersionString)) { names.Add(boss.VersionString); } }
                    break;
                }
                default: { /*Invalid type, do nothing*/ break; }
            }
            return names;
        }
        public string[] GetFilterListAsArray(FilterType ftype) { return GetFilterList(ftype).ToArray(); }
        /// <summary>
        /// Generates a Fight Info description listing the stats of the fight as well as any comments listed for the boss
        /// </summary>
        /// <param name="bossName">The full name of the boss, can use BetterBossName</param>
        /// <param name="useBettername">Set to True if you are passing the BetterBossName version of the Boss Name for proper verification</param>
        /// <returns>The generated string</returns>
        public string GenInfoString(string bossName, bool useBettername) {
            BossHandler boss = useBettername ? GetBossFromBetterName(bossName) : GetBossFromName(bossName);
            string retVal = boss.GenInfoString();
            return retVal;
        }
        // The Special Bosses
        bool useGoodBoyAvg = true;
        public void ConvertList_Move(BossHandler[] passedList, BossHandler retboss) {
            BossHandler dummy = new BossHandler(); dummy.BerserkTimer = retboss.BerserkTimer;
            Impedance s;
            List<Impedance> moves = new List<Impedance>();
            //
            foreach (BossHandler boss in passedList)
            {
                s = boss.DynamicCompiler_Move;
                if (s.Frequency != -1f) {
                    moves.Add(s);
                } else if (useGoodBoyAvg) {
                    // Adds a move that doesn't actually occur
                    moves.Add(new Impedance()
                    {
                        Frequency = retboss.BerserkTimer,
                        Duration = 0f, //retboss.BerserkTimer,
                        Chance = 1f,
                        Breakable = true,
                    });
                }
            }
            dummy.Moves = moves;
            s = dummy.DynamicCompiler_Move;
            if (s.Frequency != -1) { retboss.Moves.Add(s); }
        }
        public void ConvertList_Stun(BossHandler[] passedList, BossHandler retboss) {
            BossHandler dummy = new BossHandler(); dummy.BerserkTimer = retboss.BerserkTimer;
            Impedance s;
            List<Impedance> stuns = new List<Impedance>();
            //
            foreach (BossHandler boss in passedList)
            {
                s = boss.DynamicCompiler_Stun;
                if (s.Frequency != -1f) {
                    stuns.Add(s);
                } else if(useGoodBoyAvg) {
                    // Adds a stun that doesn't actually occur
                    stuns.Add(new Impedance()
                    {
                        Frequency = retboss.BerserkTimer,
                        Duration = 0f, //retboss.BerserkTimer,
                        Chance = 1f,
                        Breakable = true,
                    });
                }
            }
            dummy.Stuns = stuns;
            s = dummy.DynamicCompiler_Stun;
            if (s.Frequency != -1) { retboss.Stuns.Add(s); }
        }
        public void ConvertList_Fear(BossHandler[] passedList, BossHandler retboss) {
            BossHandler dummy = new BossHandler(); dummy.BerserkTimer = retboss.BerserkTimer;
            Impedance s;
            List<Impedance> fears = new List<Impedance>();
            //
            foreach (BossHandler boss in passedList)
            {
                s = boss.DynamicCompiler_Fear;
                if (s.Frequency != -1f) {
                    fears.Add(s);
                } else if (useGoodBoyAvg) {
                    // Adds a fear that doesn't actually occur
                    fears.Add(new Impedance()
                    {
                        Frequency = retboss.BerserkTimer,
                        Duration = 0f, //retboss.BerserkTimer,
                        Chance = 1f,
                        Breakable = true,
                    });
                }
            }
            dummy.Fears = fears;
            s = dummy.DynamicCompiler_Fear;
            if (s.Frequency != -1) { retboss.Fears.Add(s); }
        }
        public void ConvertList_Root(BossHandler[] passedList, BossHandler retboss) {
            BossHandler dummy = new BossHandler(); dummy.BerserkTimer = retboss.BerserkTimer;
            Impedance s;
            List<Impedance> roots = new List<Impedance>();
            //
            foreach (BossHandler boss in passedList)
            {
                s = boss.DynamicCompiler_Root;
                if (s.Frequency != -1f) {
                    roots.Add(s);
                } else if (useGoodBoyAvg) {
                    // Adds a fear that doesn't actually occur
                    roots.Add(new Impedance()
                    {
                        Frequency = retboss.BerserkTimer,
                        Duration = 0f, //retboss.BerserkTimer,
                        Chance = 1f,
                        Breakable = true,
                    });
                }
            }
            dummy.Roots = roots;
            s = dummy.DynamicCompiler_Root;
            if (s.Frequency != -1) { retboss.Roots.Add(s); }
        }
        public void ConvertList_Dsrm(BossHandler[] passedList, BossHandler retboss) {
            BossHandler dummy = new BossHandler(); dummy.BerserkTimer = retboss.BerserkTimer;
            Impedance s;
            List<Impedance> disarms = new List<Impedance>();
            //
            foreach (BossHandler boss in passedList)
            {
                s = boss.DynamicCompiler_Disarm;
                if (s.Frequency != -1f) {
                    disarms.Add(s);
                } else if (useGoodBoyAvg) {
                    // Adds a fear that doesn't actually occur
                    disarms.Add(new Impedance()
                    {
                        Frequency = retboss.BerserkTimer,
                        Duration = 0f, //retboss.BerserkTimer,
                        Chance = 1f,
                        Breakable = true,
                    });
                }
            }
            dummy.Disarms = disarms;
            s = dummy.DynamicCompiler_Disarm;
            if (s.Frequency != -1) { retboss.Disarms.Add(s); }
        }
        private BossHandler GenTheEZModeBoss(BossHandler[] passedList) {
            useGoodBoyAvg = true;
            BossHandler retboss = new BossHandler();
            if (passedList.Length < 1) { return retboss; }
            float value = 0f;
            #region Info
            retboss.Name = "The Easiest Boss";
            value = (int)BossHandler.TierLevels.T10_9; foreach (BossHandler boss in passedList) { value = Math.Min(value, (int)boss.Content); } retboss.Content = (BossHandler.TierLevels)Math.Floor(value);
            // Instance, Version, Comment Skipped
            #endregion
            #region Basics
            value = passedList[0].Level; foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.Level); } retboss.Level = (int)value;
            value = passedList[0].Armor; foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.Armor); } retboss.Armor = (int)value;
            value = passedList[0].Health; foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.Health); } retboss.Health = (int)value;
            value = 1f; foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.BerserkTimer); } retboss.BerserkTimer = (int)Math.Ceiling(value);
            value = 1f; foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.SpeedKillTimer); } retboss.SpeedKillTimer = (int)Math.Ceiling(value);
            value = 0f; foreach (BossHandler boss in passedList) { value = Math.Max(value, (float)boss.InBackPerc_Melee); } retboss.InBackPerc_Melee = value;
            value = 0f; foreach (BossHandler boss in passedList) { value = Math.Max(value, (float)boss.InBackPerc_Ranged); } retboss.InBackPerc_Ranged = value;
            bool is25 = false;
            foreach (BossHandler boss in passedList) { if (boss.Version == BossHandler.Versions.V_25N || boss.Version == BossHandler.Versions.V_25H) { is25 = true; break; } }
            retboss.Max_Players = is25 ? 25 : 10;
            value = 0f; foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.Min_Tanks); } retboss.Min_Tanks = (int)Math.Ceiling(value);
            value = 0f; foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.Min_Healers); } retboss.Min_Healers = (int)Math.Ceiling(value);
            #endregion
            #region Offensive
            {   // Multi-targs
                float f = -1, d = -1, n = 1;
                value = 0; foreach (BossHandler boss in passedList) { if (boss.MultiTargsFreq <= 0) { value = 0f; break; } else { value = Math.Max(value, boss.MultiTargsFreq); } } f = (value >= retboss.BerserkTimer || value <= 0 ? 0000 : value);
                value = 5000; foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.MultiTargsDur); } d = (value >= retboss.BerserkTimer || value <= 0 ? 5000 : value);
                value = 0; foreach (BossHandler boss in passedList) { value = Math.Min(value, (float)boss.MultiTargsNum); } n = value;
                if (!(f <= 0 || d <= 0 || n <= 0)) { retboss.Targets.Add(new TargetGroup() { Frequency = f, Duration = d, Chance = 1f, NumTargs = n, NearBoss = true, }); }
            }
            #region Attacks
            {
                {
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList) {
                        attacks.AddRange(boss.GetFilteredAttackList(ATTACK_TYPES.AT_MELEE));
                    }
                    float perhit = 500000, numtrg = retboss.Max_Players, atkspd = 0f;
                    foreach (Attack a in attacks) {
                        if(a.Name != "Invalid"){
                            perhit = Math.Min(perhit, a.DamagePerHit);
                            numtrg = Math.Min(numtrg, a.MaxNumTargets);
                            atkspd = Math.Max(atkspd, a.AttackSpeed);
                        }
                    }
                    retboss.Attacks.Add(new Attack {
                        Name = "Avg Melee",
                        AttackType = ATTACK_TYPES.AT_MELEE,
                        DamageType = ItemDamageType.Physical,
                        DamagePerHit = perhit,
                        MaxNumTargets = numtrg,
                        AttackSpeed = atkspd,
                        IsTheDefaultMelee = true,
                    });
                }
                {
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList) {
                        attacks.AddRange(boss.GetFilteredAttackList(ATTACK_TYPES.AT_RANGED));
                    }
                    float perhit = 500000, numtrg = retboss.Max_Players, atkspd = 0f;
                    foreach (Attack a in attacks) {
                        if(a.Name != "Invalid"){
                            perhit = Math.Min(perhit, a.DamagePerHit);
                            numtrg = Math.Min(numtrg, a.MaxNumTargets);
                            atkspd = Math.Max(atkspd, a.AttackSpeed);
                        }
                    }
                    retboss.Attacks.Add(new Attack {
                        Name = "Avg Ranged",
                        AttackType = ATTACK_TYPES.AT_RANGED,
                        DamageType = ItemDamageType.Physical,
                        DamagePerHit = perhit,
                        MaxNumTargets = numtrg,
                        AttackSpeed = atkspd,
                    });
                }
                {
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList)
                    {
                        attacks.AddRange(boss.GetFilteredAttackList(ATTACK_TYPES.AT_AOE));
                    }
                    float perhit = 500000, numtrg = retboss.Max_Players, atkspd = 0f;
                    foreach (Attack a in attacks) {
                        if(a.Name != "Invalid"){
                            perhit = Math.Min(perhit, a.DamagePerHit);
                            numtrg = Math.Min(numtrg, a.MaxNumTargets);
                            atkspd = Math.Max(atkspd, a.AttackSpeed);
                        }
                    }
                    retboss.Attacks.Add(new Attack {
                        Name = "Avg AoE",
                        AttackType = ATTACK_TYPES.AT_AOE,
                        DamageType = ItemDamageType.Arcane,
                        DamagePerHit = perhit,
                        MaxNumTargets = numtrg,
                        AttackSpeed = atkspd,
                    });
                }
            }
            #endregion
            #endregion
            #region Defensive
            foreach (ItemDamageType t in DamageTypes) {
                value = (float)passedList[0].Resistance(t);
                foreach (BossHandler boss in passedList) {
                    value = (float)Math.Min(value, boss.Resistance(t));
                }
                retboss.Resistance(t, value);
            }
            #endregion
            #region Impedances
            {   // Move
                float f = -1, d = -1;
                value = 0;      foreach (BossHandler boss in passedList) { if (boss.MovingTargsFreq    <= 0) { value = 0f; break; } else { value = Math.Max(value, boss.MovingTargsFreq); } } f = (value >= retboss.BerserkTimer || value <= 0 ? 0000 : value);
                value = 5000;   foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.MovingTargsDur); } d = (value >= retboss.BerserkTimer || value <= 0 ? 5000 : value);
                if (!(f <= 0 || d <= 0)) { retboss.Moves.Add(new Impedance() { Frequency = f, Duration = d, Chance = 1f, Breakable = true, }); }
            }
            {   // Stun
                float f = -1, d = -1;
                value = 0;      foreach (BossHandler boss in passedList) { if (boss.StunningTargsFreq  <= 0) { value = 0f; break; } else { value = Math.Max(value, boss.StunningTargsFreq  ); } } f  = (value >= retboss.BerserkTimer || value <= 0 ? 0000 : value);
                value = 10*1000;foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.StunningTargsDur  ); } d   = (value >= retboss.BerserkTimer || value <= 0 ? 5000 : value);
                if (!(f <= 0 || d <= 0)) { retboss.Stuns.Add(new Impedance() { Frequency = f, Duration = d, Chance = 1f, Breakable = true, }); }
            }
            {   // Fear
                float f = -1, d = -1;
                value = 0;      foreach (BossHandler boss in passedList) { if (boss.FearingTargsFreq   <= 0) { value = 0f; break; } else { value = Math.Max(value, boss.FearingTargsFreq   ); } } f   = (value >= retboss.BerserkTimer || value <= 0 ? 0000 : value);
                value = 5000;   foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.FearingTargsDur   ); } d    = (value >= retboss.BerserkTimer || value <= 0 ? 5000 : value);
                if (!(f <= 0 || d <= 0)) { retboss.Fears.Add(new Impedance() { Frequency = f, Duration = d, Chance = 1f, Breakable = true, }); }
            }
            {   // Root
                float f = -1, d = -1;
                value = 0;      foreach (BossHandler boss in passedList) { if (boss.RootingTargsFreq   <= 0) { value = 0f; break; } else { value = Math.Max(value, boss.RootingTargsFreq   ); } } f = (value >= retboss.BerserkTimer || value <= 0 ? 0000 : value);
                value = 5000;   foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.RootingTargsDur   ); } d = (value >= retboss.BerserkTimer || value <= 0 ? 5000 : value);
                if (!(f <= 0 || d <= 0)) { retboss.Roots.Add(new Impedance() { Frequency = f, Duration = d, Chance = 1f, Breakable = true, }); }
            }
            {   // Disarm
                float f = -1, d = -1;
                value = 0;      foreach (BossHandler boss in passedList) { if (boss.DisarmingTargsFreq <= 0) { value = 0f; break; } else { value = Math.Max(value, boss.DisarmingTargsFreq ); } } f = (value >= retboss.BerserkTimer || value <= 0 ? 0000 : value);
                value = 5000;   foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.DisarmingTargsDur ); } d  = (value >= retboss.BerserkTimer || value <= 0 ? 5000 : value);
                if (!(f <= 0 || d <= 0)) { retboss.Disarms.Add(new Impedance() { Frequency = f, Duration = d, Chance = 1f, Breakable = true, }); }
            }
            #endregion
            //
            return retboss;
        }
        private BossHandler GenTheAvgBoss(BossHandler[] passedList) {
            useGoodBoyAvg = true;
            BossHandler retboss = new BossHandler();
            if (passedList.Length < 1) { return retboss; }
            float value = 0f;
            #region Info
            retboss.Name = "The Average Boss";
            value = (int)BossHandler.TierLevels.T7_0; foreach (BossHandler boss in passedList) { value += (int)boss.Content; } retboss.Content = (BossHandler.TierLevels)Math.Floor(value / (float)passedList.Length);
            // Instance, Version, Comment Skipped
            #endregion
            #region Basics
            value = 0f; foreach (BossHandler boss in passedList) { value += boss.Level; } value /= passedList.Length; retboss.Level = (int)value;
            value = 0f; foreach (BossHandler boss in passedList) { value += boss.Armor; } value /= passedList.Length; retboss.Armor = (int)value;
            value = 0f; foreach (BossHandler boss in passedList) { value += boss.BerserkTimer; } value /= passedList.Length; retboss.BerserkTimer = (int)Math.Floor(value);
            value = 0f; foreach (BossHandler boss in passedList) { value += boss.SpeedKillTimer; } value /= passedList.Length; retboss.SpeedKillTimer = (int)Math.Floor(value);
            value = 0f; foreach (BossHandler boss in passedList) { value += boss.Health; } value /= passedList.Length; retboss.Health = value;
            value = 0f; foreach (BossHandler boss in passedList) { value += (float)boss.InBackPerc_Melee; } value /= passedList.Length; retboss.InBackPerc_Melee = value;
            value = 0f; foreach (BossHandler boss in passedList) { value += (float)boss.InBackPerc_Ranged; } value /= passedList.Length; retboss.InBackPerc_Ranged = value;
            bool is25 = false;
            foreach (BossHandler boss in passedList) { if (boss.Version == BossHandler.Versions.V_25N || boss.Version == BossHandler.Versions.V_25H) { is25 = true; break; } }
            retboss.Max_Players = is25 ? 25 : 10;
            value = 0f; foreach (BossHandler boss in passedList) { value += boss.Min_Tanks; } value /= passedList.Length; retboss.Min_Tanks = (int)Math.Floor(value);
            value = 0f; foreach (BossHandler boss in passedList) { value += boss.Min_Healers; } value /= passedList.Length; retboss.Min_Healers = (int)Math.Floor(value);
            #endregion
            #region Offensive
            {   // Multi-targs
                float f = -1, d = -1, n = 1;
                value = 0f; foreach (BossHandler boss in passedList) { value += (boss.MultiTargsFreq > 0 && boss.MultiTargsFreq < boss.BerserkTimer) ? boss.MultiTargsFreq : retboss.BerserkTimer; } value /= passedList.Length; f = value;
                value = 0f; foreach (BossHandler boss in passedList) { value += boss.MultiTargsDur; } value /= passedList.Length; d = value;
                value = 0f; foreach (BossHandler boss in passedList) { value += (float)boss.MultiTargsNum; } value /= passedList.Length; n = value;
                if (f <= 0 || d == 0) {
                } else {
                    retboss.Targets.Add(new TargetGroup() { Frequency = f, Duration = d, Chance = 1f, NumTargs = n, NearBoss = true, });
                }
            }
            #region Attacks
            {
                {
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList) {
                        attacks.AddRange(boss.GetFilteredAttackList(ATTACK_TYPES.AT_MELEE));
                    }
                    if (attacks.Count > 0)
                    {
                        float perhit = 0f, numtrg = 0f, atkspd = 0f;
                        foreach (Attack a in attacks)
                        {
                            if (a.Name != "Invalid")
                            {
                                perhit += a.DamagePerHit;
                                numtrg += a.MaxNumTargets;
                                atkspd += a.AttackSpeed;
                            }
                        }
                        perhit /= (float)attacks.Count;
                        numtrg /= (float)attacks.Count;
                        atkspd /= (float)attacks.Count;
                        retboss.Attacks.Add(new Attack
                        {
                            Name = "Avg Melee",
                            AttackType = ATTACK_TYPES.AT_MELEE,
                            DamageType = ItemDamageType.Physical,
                            DamagePerHit = perhit,
                            MaxNumTargets = numtrg,
                            AttackSpeed = atkspd,
                            IsTheDefaultMelee = true,
                        });
                    }
                }
                {
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList) {
                        attacks.AddRange(boss.GetFilteredAttackList(ATTACK_TYPES.AT_RANGED));
                    }
                    if (attacks.Count > 0)
                    {
                        float perhit = 0f, numtrg = 0f, atkspd = 0f;
                        foreach (Attack a in attacks)
                        {
                            if (a.Name != "Invalid")
                            {
                                perhit += a.DamagePerHit;
                                numtrg += a.MaxNumTargets;
                                atkspd += a.AttackSpeed;
                            }
                        }
                        perhit /= (float)attacks.Count;
                        numtrg /= (float)attacks.Count;
                        atkspd /= (float)attacks.Count;
                        retboss.Attacks.Add(new Attack
                        {
                            Name = "Avg Ranged",
                            AttackType = ATTACK_TYPES.AT_RANGED,
                            DamageType = ItemDamageType.Physical,
                            DamagePerHit = perhit,
                            MaxNumTargets = numtrg,
                            AttackSpeed = atkspd,
                        });
                    }
                }
                {
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList)
                    {
                        attacks.AddRange(boss.GetFilteredAttackList(ATTACK_TYPES.AT_AOE));
                    }
                    if (attacks.Count > 0)
                    {
                        float perhit = 0f, numtrg = 0f, atkspd = 0f;
                        foreach (Attack a in attacks)
                        {
                            if (a.Name != "Invalid")
                            {
                                perhit += a.DamagePerHit;
                                numtrg += a.MaxNumTargets;
                                atkspd += a.AttackSpeed;
                            }
                        }
                        perhit /= (float)attacks.Count;
                        numtrg /= (float)attacks.Count;
                        atkspd /= (float)attacks.Count;
                        retboss.Attacks.Add(new Attack
                        {
                            Name = "Avg AoE",
                            AttackType = ATTACK_TYPES.AT_AOE,
                            DamageType = ItemDamageType.Arcane,
                            DamagePerHit = perhit,
                            MaxNumTargets = numtrg,
                            AttackSpeed = atkspd,
                        });
                    }
                }
            }
            #endregion
            #endregion
            #region Defensive
            foreach (ItemDamageType t in DamageTypes) {
                value = 0f;
                foreach (BossHandler boss in passedList) {
                    value += (float)boss.Resistance(t);
                }
                value /= passedList.Length;
                retboss.Resistance(t, value);
            }
            #endregion
            #region Impedances
            {
                // Move
                float f = -1, d = -1;
                value = 0f; foreach (BossHandler boss in passedList) { value += (boss.MovingTargsFreq > 0 && boss.MovingTargsFreq < boss.BerserkTimer) ? boss.MovingTargsFreq : retboss.BerserkTimer; } value /= passedList.Length; f = value;
                value = 0f; foreach (BossHandler boss in passedList) { value += boss.MovingTargsDur; } value /= passedList.Length; d = value;
                if (f <= 0 || d == 0) {
                } else {
                    retboss.Moves.Add(new Impedance() { Frequency = f, Duration = d, Chance = 1f, Breakable = true, });
                }
            }
            {
                // Stun
                float f = -1, d = -1;
                value = 0f; foreach (BossHandler boss in passedList) { value += (boss.StunningTargsFreq > 0 && boss.StunningTargsFreq < boss.BerserkTimer) ? boss.StunningTargsFreq : retboss.BerserkTimer; } value /= passedList.Length; f = value;
                value = 0f; foreach (BossHandler boss in passedList) { value += boss.StunningTargsDur; } value /= passedList.Length; d = value;
                if (f <= 0 || d == 0) {
                } else {
                    retboss.Stuns.Add(new Impedance() { Frequency = f, Duration = d, Chance = 1f, Breakable = true, });
                }
            }
            {
                // Fear
                float f = -1, d = -1;
                value = 0f; foreach (BossHandler boss in passedList) { value += (boss.FearingTargsFreq > 0 && boss.FearingTargsFreq < boss.BerserkTimer) ? boss.FearingTargsFreq : retboss.BerserkTimer; } value /= passedList.Length; f = value;
                value = 0f; foreach (BossHandler boss in passedList) { value += boss.FearingTargsDur; } value /= passedList.Length; d = value;
                if (f <= 0 || d == 0) {
                } else {
                    retboss.Fears.Add(new Impedance() { Frequency = f, Duration = d, Chance = 1f, Breakable = true, });
                }
            }
            {
                // Root
                float f = -1, d = -1;
                value = 0f; foreach (BossHandler boss in passedList) { value += (boss.RootingTargsFreq > 0 && boss.RootingTargsFreq < boss.BerserkTimer) ? boss.RootingTargsFreq : retboss.BerserkTimer; } value /= passedList.Length; f = value;
                value = 0f; foreach (BossHandler boss in passedList) { value += boss.RootingTargsDur; } value /= passedList.Length; d = value;
                if (f <= 0 || d == 0) {
                } else {
                    retboss.Roots.Add(new Impedance() { Frequency = f, Duration = d, Chance = 1f, Breakable = true, });
                }
            }
            {
                // Disarm
                float f = -1, d = -1;
                value = 0f; foreach (BossHandler boss in passedList) { value += (boss.DisarmingTargsFreq > 0 && boss.DisarmingTargsFreq < boss.BerserkTimer) ? boss.DisarmingTargsFreq : retboss.BerserkTimer; } value /= passedList.Length; f = value;
                value = 0f; foreach (BossHandler boss in passedList) { value += boss.DisarmingTargsDur; } value /= passedList.Length; d = value;
                if (f <= 0 || d == 0) {
                } else {
                    retboss.Disarms.Add(new Impedance() { Frequency = f, Duration = d, Chance = 1f, Breakable = true, });
                }
            }
            #endregion
            //
            return retboss;
        }
        private BossHandler GenTheHardestBoss(BossHandler[] passedList) {
            useGoodBoyAvg = false;
            BossHandler retboss = new BossHandler();
            if (passedList.Length < 1) { return retboss; }
            float value = 0f;
            #region Info
            retboss.Name = "The Hardest Boss";
            value = (int)BossHandler.TierLevels.T7_0; foreach (BossHandler boss in passedList) { value = Math.Max(value, (int)boss.Content); } retboss.Content = (BossHandler.TierLevels)Math.Ceiling(value);
            // Instance, Version, Comment Skipped
            #endregion
            #region Basics
            value = 0f; foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.Level); } retboss.Level = (int)value;
            value = 0f; foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.Armor); } retboss.Armor = (int)value;
            value = 0f; foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.Health); } retboss.Health = value;
            value = passedList[0].BerserkTimer; foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.BerserkTimer); } retboss.BerserkTimer = (int)Math.Floor(value);
            value = passedList[0].SpeedKillTimer; foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.SpeedKillTimer); } retboss.SpeedKillTimer = (int)Math.Floor(value);
            value = 1.00f;      foreach (BossHandler boss in passedList) { value = (float)Math.Min(value, boss.InBackPerc_Melee   ); } retboss.InBackPerc_Melee  = value;
            value = 1.00f;      foreach (BossHandler boss in passedList) { value = (float)Math.Min(value, boss.InBackPerc_Ranged  ); } retboss.InBackPerc_Ranged = value;
            bool is25 = false;
            foreach (BossHandler boss in passedList) { if (boss.Version == BossHandler.Versions.V_25N || boss.Version == BossHandler.Versions.V_25H) { is25 = true; break; } }
            retboss.Max_Players = is25 ? 25 : 10;
            value = 0; foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.Min_Tanks); } retboss.Min_Tanks = (int)value;
            value = 0; foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.Min_Healers); } retboss.Min_Healers = (int)value;
            #endregion
            #region Offensive
            {   // Multi-targs
                float f = -1, d = -1, n = 1;
                value = 0; foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.MultiTargsFreq > 0 ? boss.MultiTargsFreq : value); } f = value;
                value = 0; foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.MultiTargsDur); } d = value;
                value = 0; foreach (BossHandler boss in passedList) { value = Math.Max(value, (float)boss.MultiTargsNum); } n = value;
                if (!(f == 0 || d == 0)) { retboss.Targets.Add(new TargetGroup() { Frequency = f, Duration = d, Chance = 1f, NumTargs = n, NearBoss = false, }); }
            }
            #region Attacks
            {
                {
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList) {
                        attacks.AddRange(boss.GetFilteredAttackList(ATTACK_TYPES.AT_MELEE));
                    }
                    float perhit = 0f, numtrg = 0f, atkspd = 45f;
                    foreach (Attack a in attacks) {
                        if(a.Name != "Invalid"){
                            perhit = Math.Max(perhit, a.DamagePerHit);
                            numtrg = Math.Max(numtrg, a.MaxNumTargets);
                            atkspd = Math.Min(atkspd, a.AttackSpeed);
                        }
                    }
                    retboss.Attacks.Add(new Attack {
                        Name = "Avg Melee",
                        AttackType = ATTACK_TYPES.AT_MELEE,
                        DamageType = ItemDamageType.Physical,
                        DamagePerHit = perhit,
                        MaxNumTargets = numtrg,
                        AttackSpeed = atkspd,
                        IsTheDefaultMelee = true,
                    });
                }
                {
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList) {
                        attacks.AddRange(boss.GetFilteredAttackList(ATTACK_TYPES.AT_RANGED));
                    }
                    float perhit = 0f, numtrg = 0f, atkspd = 45f;
                    foreach (Attack a in attacks) {
                        if(a.Name != "Invalid"){
                            perhit = Math.Max(perhit, a.DamagePerHit);
                            numtrg = Math.Max(numtrg, a.MaxNumTargets);
                            atkspd = Math.Min(atkspd, a.AttackSpeed);
                        }
                    }
                    retboss.Attacks.Add(new Attack {
                        Name = "Avg Ranged",
                        AttackType = ATTACK_TYPES.AT_RANGED,
                        DamageType = ItemDamageType.Physical,
                        DamagePerHit = perhit,
                        MaxNumTargets = numtrg,
                        AttackSpeed = atkspd,
                    });
                }
                {
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList)
                    {
                        attacks.AddRange(boss.GetFilteredAttackList(ATTACK_TYPES.AT_AOE));
                    }
                    float perhit = 0f, numtrg = 0f, atkspd = 45f;
                    foreach (Attack a in attacks) {
                        if(a.Name != "Invalid"){
                            perhit = Math.Max(perhit, a.DamagePerHit);
                            numtrg = Math.Max(numtrg, a.MaxNumTargets);
                            atkspd = Math.Min(atkspd, a.AttackSpeed);
                        }
                    }
                    retboss.Attacks.Add(new Attack {
                        Name = "Avg AoE",
                        AttackType = ATTACK_TYPES.AT_AOE,
                        DamageType = ItemDamageType.Arcane,
                        DamagePerHit = perhit,
                        MaxNumTargets = numtrg,
                        AttackSpeed = atkspd,
                    });
                }
            }
            #endregion
            #endregion
            #region Defensive
            foreach (ItemDamageType t in DamageTypes) { value = 0f; foreach (BossHandler boss in passedList) { value = (float)Math.Max(value, boss.Resistance(t)); } retboss.Resistance(t, value); }
            #endregion
            #region Impedances
            {
                // Move
                float f = -1, d = -1;
                value = 19 * 20;    foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.MovingTargsFreq    > 0 ? boss.MovingTargsFreq    : value); } f    = Math.Max(20,value);
                value = 0f;         foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.MovingTargsDur     ); } d    = Math.Min(10*1000,value);
                if (f == 0 || d == 0) {
                } else {
                    retboss.Moves.Add(new Impedance() { Frequency = f, Duration = d, Chance = 1f, Breakable = true, });
                }
            }
            {
                // Stun
                float f = -1, d = -1;
                value = 19 * 20;    foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.StunningTargsFreq  > 0 ? boss.StunningTargsFreq  : value); } f  = Math.Max(20,value);
                value = 0f;         foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.StunningTargsDur   ); } d  = Math.Min(10*1000,value);
                if (f == 0 || d == 0) {
                } else {
                    retboss.Stuns.Add(new Impedance() { Frequency = f, Duration = d, Chance = 1f, Breakable = true, });
                }
            }
            {
                // Fear
                float f = -1, d = -1;
                value = 19 * 20;    foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.FearingTargsFreq   > 0 ? boss.FearingTargsFreq   : value); } f   = Math.Max(20,value);
                value = 0f;         foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.FearingTargsDur    ); } d   = Math.Min(10*1000,value);
                if (f == 0 || d == 0) {
                } else {
                    retboss.Fears.Add(new Impedance() { Frequency = f, Duration = d, Chance = 1f, Breakable = true, });
                }
            }
            {
                // Root
                float f = -1, d = -1;
                value = 19 * 20;    foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.RootingTargsFreq   > 0 ? boss.RootingTargsFreq   : value); } f   = Math.Max(20,value);
                value = 0f;         foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.RootingTargsDur    ); } d   = Math.Min(10*1000,value);
                if (f == 0 || d == 0) {
                } else {
                    retboss.Roots.Add(new Impedance() { Frequency = f, Duration = d, Chance = 1f, Breakable = true, });
                }
            }
            {
                // Disarm
                float f = -1, d = -1;
                value = 19 * 20;    foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.DisarmingTargsFreq < 0 ? boss.DisarmingTargsFreq : value); } f = Math.Max(20,value);
                value = 0f;         foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.DisarmingTargsDur  ); } d = Math.Min(10*1000,value);
                if (f == 0 || d == 0) {
                } else {
                    retboss.Disarms.Add(new Impedance() { Frequency = f, Duration = d, Chance = 1f, Breakable = true, });
                }
            }
            #endregion
            //
            return retboss;
        }
        #endregion
    }
}
