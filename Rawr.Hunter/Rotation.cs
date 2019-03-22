using System;
using System.Collections.Generic;
using System.Text;
using Rawr.Hunter.Skills;

namespace Rawr.Hunter {
    public class Rotation {
        public class AbilWrapper
        {
            public AbilWrapper(Skills.Ability abil) { ability = abil; }
            public Skills.Ability ability { get; set; }
            public float numActivates { get; set; }
            public float Mana { get { return ability.GetManaUseOverDur(numActivates); } }
            public float DPS { get { return ability.GetDPS(numActivates); } }
            public float HPS { get { return ability.GetHPS(numActivates); } }
            public bool isDamaging { get { return ability.DamageOverride > 0f; } }
        }
        // Constructors
        public Rotation()
        {
            Char = null;
            StatS = null;
            Talents = null;
            CombatFactors = null;
            CalcOpts = null;
            BossOpts = null;

            AbilityList = new Dictionary<Type,AbilWrapper>();
            InvalidateCache();
        }

        #region Variables

        private Dictionary<Type,AbilWrapper> AbilityList;

        public List<AbilWrapper> GetDamagingAbilities()
        {
            return new List<AbilWrapper>(AbilityList.Values).FindAll(e => e.isDamaging);
        }

        public List<AbilWrapper> GetMaintenanceAbilities()
        {
            return new List<AbilWrapper>(AbilityList.Values).FindAll(e => e.ability.isMaint);
        }

        public List<AbilWrapper> GetAbilityList()
        {
            return new List<AbilWrapper>(AbilityList.Values);
        }

        public AbilWrapper GetWrapper<T>()
        {
            return AbilityList[typeof(T)];
        }

        public float _HPS_TTL;
        public float _DPS_TTL;
        public string GCDUsage = "";
        protected CharacterCalculationsHunter calcs = null;
        
        public bool _needDisplayCalcs = true;
        
        protected float FightDuration;
        protected float TimeLostGCDs;
        protected float RageGainedWhileMoving;
        public float TimesStunned = 0f;
        public float TimesFeared = 0f;
        public float TimesRooted = 0f;
        public float TimesDisarmed = 0f;
        
        #region Abilities
        // Anti-DeBuff
        //public Skills.HeroicFury HF;
        //public Skills.EveryManForHimself EM;
        //public Skills.Charge CH;
        //public Skills.Intercept IN;
        //public Skills.Intervene IV;
        // Rage Generators
        //public Skills.SecondWind SndW;
        //public Skills.BerserkerRage BZ;
        //public Skills.Bloodrage BR;
        // Maintenance
        //public Skills.BattleShout BTS;
        //public Skills.CommandingShout CS;
        //public Skills.DemoralizingShout DS;
        //public Skills.SunderArmor SN;
        //public Skills.ThunderClap TH;
        //public Skills.Hamstring HMS;
        //public Skills.EnragedRegeneration ER;
        // Periodics
        //public Skills.ShatteringThrow ST;
        //public Skills.SweepingStrikes SW;
        //public Skills.DeathWish Death;
        //public Skills.Recklessness RK;
        // Shots
        public Skills.ExplosiveShot Explosive;
        public Skills.MultiShot Multi;
        public Skills.SteadyShot Steady;
        public Skills.AimedShot Aimed;
        public Skills.ArcaneShot Arcane;
        public Skills.SilencingShot Silencing;
        public Skills.Volley Volley;
        //public Skills.ChimeraShot ChimeraS;

        // Buffs.
        public Skills.BestialWrath Bestial;
        public Skills.RapidFire Rapid;
        public Skills.BlackArrowBuff BlackArrowB;
        public Skills.Readiness Ready;

        // DoTs.
        public Skills.PiercingShots Piercing;
        public Skills.BlackArrowDoT BlackArrowD;

        public Skills.SerpentSting Serpent;
        public Skills.ScorpidSting Scorpid;
        public Skills.ViperSting Viper;
        public Skills.ChimeraShot_Serpent Chimera;

        // Traps.
        public Skills.ImmolationTrap Immolation;
        public Skills.ExplosiveTrap ExplosiveT;
        public Skills.FreezingTrap Freezing;
        public Skills.FrostTrap Frost;

        public float _Steady_DPS = 0f, _Steady_HPS = 0f, _Steady_GCDs = 0f;
        public float _Multi_DPS = 0f, _Multi_HPS = 0f, _Multi_GCDs = 0f;
        
        // Generic
        //public Skills.Cleave CL;
        //public Skills.HeroicStrike HS;
        public Skills.KillShot Kill;
        #endregion

        #endregion
        #region Get/Set
        protected Character Char { get; set; }
        protected WarriorTalents Talents { get; set; }
        protected WarriorTalentsCata TalentsCata { get; set; }
        protected Stats StatS { get; set; }
        protected CombatFactors CombatFactors { get; set; }
        public Skills.WhiteAttacks WhiteAtks { get; protected set; }
        protected CalculationOptionsHunter CalcOpts { get; set; }
        protected BossOptions BossOpts { get; set; }
        
        protected float LatentGCD { get { return 1.5f + CalcOpts.Latency + CalcOpts.AllowedReact; } }
        
        /// <summary>
        /// How many GCDs are in the rotation, based on fight duration and latency
        /// </summary>
        protected float NumGCDs { get { return FightDuration / LatentGCD; } }
        
        /// <summary>
        /// How many GCDs have been used by the rotation
        /// </summary>
        protected float GCDsUsed {
            get {
                float gcds = 0f;
                foreach (AbilWrapper aw in GetAbilityList()) {
                    if (aw.ability.UsesGCD) {
                        gcds += aw.numActivates * aw.ability.UseTime / LatentGCD;
                    }
                }
                return gcds;
            }
        }

        /// <summary>
        /// How many GCDs are still available in the rotation
        /// </summary>
        protected float GCDsAvailable { get { return Math.Max(0f, NumGCDs - GCDsUsed - TimeLostGCDs); } }
        
        #endregion
        #region Functions
        public virtual void Initialize(CharacterCalculationsHunter calcs) {
            this.calcs = calcs;
            StatS = calcs.AverageStats;

            initAbilities();
            //doIterations();

            // Whites
            calcs.Whites = WhiteAtks;
            // Anti-Debuff
            calcs.Explosive = Explosive;
            /*calcs.HF = HF;
            calcs.EM = EM;
            calcs.CH = CH;
            calcs.IN = IN;
            calcs.IV = IV;
            // Rage Generators
            calcs.SndW = SndW;
            calcs.BZ = BZ;
            calcs.BR = BR;
            // Maintenance
            calcs.BTS = BTS;
            calcs.CS = CS;
            calcs.DS = DS;
            calcs.SN = SN;
            calcs.TH = TH;
            calcs.HMS = HMS;
            calcs.ER = ER;
            // Periodics
            calcs.ST = ST;
            calcs.SW = SW;
            calcs.Death = Death;
            calcs.RK = RK;*/
            
            // Shared Instants
            calcs.Multi = Multi;
            calcs.Steady = Steady;
            calcs.Aimed = Aimed;
            calcs.Multi = Multi;
            calcs.Arcane = Arcane;
            // Arms
            
            // Generic
            //calcs.CL = CL;
            calcs.Piercing = Piercing;
            //calcs.HS = HS;
            calcs.Kill = Kill;
            calcs.Silencing = Silencing;
            calcs.Volley = Volley;

            calcs.Bestial = Bestial;
            calcs.Rapid = Rapid;
            calcs.BlackArrowB = BlackArrowB;
            calcs.Ready = Ready;

            calcs.Piercing = Piercing;
            calcs.BlackArrowD = BlackArrowD;
            calcs.Serpent = Serpent;
            calcs.Scorpid = Scorpid;
            calcs.Viper = Viper;
            calcs.Chimera = Chimera;

            calcs.Immolation = Immolation;
            calcs.ExplosiveT = ExplosiveT;
            calcs.Freezing = Freezing;
            calcs.Frost = Frost;
        }
        public virtual void Initialize() { initAbilities(); /*doIterations();*/ }

        public virtual void MakeRotationandDoDPS(bool setCalcs, bool needsDisplayCalculations) {
            _needDisplayCalcs = needsDisplayCalculations;
        }

        protected virtual void initAbilities() {
            AbilityList.Clear();
            WhiteAtks.InvalidateCache();
            // Whites
            //WhiteAtks = new Skills.WhiteAtks(Char, StatS, CombatFactors);
            // Anti-Debuff
            //AddAbility(new AbilWrapper(new Skills.HeroicFury(        Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.EveryManForHimself(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.Charge(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.Intercept(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.Intervene(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            // Rage Generators
            //AddAbility(new AbilWrapper(new Skills.SecondWind(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.BerserkerRage(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.Bloodrage(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            // Maintenance
            //AddAbility(new AbilWrapper(new Skills.BattleShout(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.CommandingShout(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.DemoralizingShout(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.SunderArmor(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.ThunderClap(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.Hamstring(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.EnragedRegeneration(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            // Periodics
            //AddAbility(new AbilWrapper(new Skills.ShatteringThrow(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.SweepingStrikes(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.DeathWish(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.Recklessness(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));

            // Slam used by Bloodsurge, WW used by Bladestorm, so they're shared
            SteadyShot Steady = new Skills.SteadyShot(Char, StatS, CombatFactors, WhiteAtks, CalcOpts);
            AddAbility(new AbilWrapper(Steady));
            Skills.Ability Multi = new Skills.MultiShot(Char, StatS, CombatFactors, WhiteAtks, CalcOpts);
            AddAbility(new AbilWrapper(Multi));
            
            //AddAbility(new AbilWrapper(new Skills.Cleave(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.HeroicStrike(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            Skills.Ability Kill = new Skills.KillShot(Char, StatS, CombatFactors, WhiteAtks, CalcOpts);
            AddAbility(new AbilWrapper(Kill));

            // Arms abilities
            //AddAbility(new AbilWrapper(new Skills.Bladestorm(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, WW)));
            //AddAbility(new AbilWrapper(new Skills.MortalStrike(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.Rend(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //Skills.Ability SS = new Skills.Swordspec(Char, StatS, CombatFactors, WhiteAtks, CalcOpts);
            //AddAbility(new AbilWrapper(SS));
            //AddAbility(new AbilWrapper(new Skills.OverPower(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, SS)));
            //AddAbility(new AbilWrapper(new Skills.TasteForBlood(Char, StatS, CombatFactors, WhiteAtks, CalcOpts)));
            //AddAbility(new AbilWrapper(new Skills.Suddendeath(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, EX)));
            
            // Fury abilities
            //Ability BT = new Skills.BloodThirst(Char, StatS, CombatFactors, WhiteAtks, CalcOpts);
            //AddAbility(new AbilWrapper(BT));
            //AddAbility(new AbilWrapper(new Skills.BloodSurge(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, SL, WW, BT)));

            Piercing = new Skills.PiercingShots(Char, StatS, CombatFactors, WhiteAtks, CalcOpts);

        }

        private void AddAbility(AbilWrapper abilWrapper)
        {
            AbilityList.Add(abilWrapper.ability.GetType(), abilWrapper);
        }

        public virtual void doIterations() { }

        protected virtual void calcDeepWounds() {
            // Ranged Weapon
            float rwActivates =
                /*Yellow  */CriticalYellowsOverDurMH + 
                /*White   */WhiteAtks.RwActivates * WhiteAtks.RWAtkTable.Crit;

            // Push to the Ability
            //DW.SetAllAbilityActivates(rwActivates); 
        }

        public void InvalidateCache()
        {
            for (int i = 0; i < 5; i++) for (int k = 0; k < 3; k++)
                _atkOverDurs[i,k] = -1f;
        }

        #region Attacks over Duration
        public enum ShotResult : int { Attempt=0, Land, Crit };
        public enum AttackType : int { Yellow=0, White, Both };
        private float[,] _atkOverDurs = new float[5,3];
        public float GetAttackOverDuration(ShotResult shotResult, AttackType attackType)
        {
            if (_atkOverDurs[(int)shotResult, (int)attackType] == -1f)
            {
                SetTable(shotResult, attackType);
            }
            return _atkOverDurs[(int)shotResult, (int)attackType];
        }
        private void SetTable(ShotResult sr, AttackType at)
        {
            float count = 0f;
            float mod;
            CombatTable table;

            if (at != AttackType.White) {
                foreach (AbilWrapper abil in GetDamagingAbilities())
                {
                    if (!abil.ability.Validated) { continue; }
                    table = abil.ability.RWAtkTable;
                    mod = GetTableFromSwingResult(sr, table);
                    count += abil.numActivates * abil.ability.AvgTargets * abil.ability.SwingsPerActivate * mod;
                }
            }
            if (at != AttackType.Yellow) {
                table = WhiteAtks.RWAtkTable;
                mod = GetTableFromSwingResult(sr, table);
                count += WhiteAtks.RwActivates * mod;
            }
            
            _atkOverDurs[(int)sr, (int)at] = count;
        }
        private float GetTableFromSwingResult(ShotResult sr, CombatTable table)
        {
            if (table == null) return 0f;
            switch (sr)
            {
                case ShotResult.Attempt: return 1f;
                case ShotResult.Crit: return table.Crit;
                //case ShotResult.Dodge: return table.Dodge;
                case ShotResult.Land: return table.AnyLand;
                //case ShotResult.Parry: return table.Parry;
                default: return 0f;
            }
        }

        public float AttemptedAtksOverDur { get { return GetAttackOverDuration(ShotResult.Attempt, AttackType.Both); } }
        public float AttemptedYellowsOverDur { get { return GetAttackOverDuration(ShotResult.Attempt, AttackType.Yellow); } }
        public float LandedAtksOverDur { get { return GetAttackOverDuration(ShotResult.Land, AttackType.Both); } }
        public float CriticalAtksOverDur { get { return GetAttackOverDuration(ShotResult.Crit, AttackType.Both); } }

        public float AttemptedAtksOverDurMH { get { return GetAttackOverDuration(ShotResult.Attempt, AttackType.Both); } }
        public float LandedAtksOverDurMH { get { return GetAttackOverDuration(ShotResult.Land, AttackType.Both); } }

        public float AttemptedAtksOverDurOH { get { return GetAttackOverDuration(ShotResult.Attempt, AttackType.Both); } }
        public float LandedAtksOverDurOH { get { return GetAttackOverDuration(ShotResult.Land, AttackType.Both); } }

        public float CriticalYellowsOverDur { get { return GetAttackOverDuration(ShotResult.Crit, AttackType.Both); } }
        public float LandedYellowsOverDur { get { return GetAttackOverDuration(ShotResult.Land, AttackType.Both); } }

        public float CriticalYellowsOverDurMH { get { return GetAttackOverDuration(ShotResult.Crit, AttackType.Yellow); } }
        public float CriticalYellowsOverDurOH { get { return GetAttackOverDuration(ShotResult.Crit, AttackType.Yellow); } }
        
        //public float DodgedYellowsOverDur { get { return GetAttackOverDuration(ShotResult.Dodge, AttackType.Yellow); } }
        //public float ParriedYellowsOverDur { get { return GetAttackOverDuration(ShotResult.Parry, AttackType.Yellow); } }

        public virtual float CritHsSlamOverDur {
            get {
                return 0f;
                /*AbilWrapper HS = GetWrapper<HeroicStrike>();
                AbilWrapper SL = GetWrapper<Slam>();
                AbilWrapper BS = GetWrapper<BloodSurge>();
                return HS.numActivates * HS.ability.RWAtkTable.Crit * HS.ability.AvgTargets +
                       SL.numActivates * SL.ability.RWAtkTable.Crit * SL.ability.AvgTargets +
                       BS.numActivates * BS.ability.RWAtkTable.Crit * BS.ability.AvgTargets;*/
            }
        }

        #endregion

        public float MaintainCDs { get {
            float cds = 0f;
            foreach (AbilWrapper aw in GetMaintenanceAbilities()) cds += aw.numActivates;
            return cds;
        } }
        #endregion
        #region Rage Calcs
        protected virtual float RageGenOverDur_IncDmg {
            get {
                // Invalidate bad things
                /*if (!CalcOpts.AoETargets || CalcOpts.AoETargetsFreq < 1 || CalcOpts.AoETargetsDMG < 1) { return 0f; }
                float RageMod = 2.5f / 453.3f;
                float damagePerSec = 0f;
                float freq = CalcOpts.AoETargetsFreq;
                float dmg = CalcOpts.AoETargetsDMG * (1f + StatS.DamageTakenMultiplier) + StatS.BossAttackPower / 14f;
                float acts = FightDuration / freq;
                // Add Berserker Rage's
                float zerkerMOD = 1f;
                if (CalcOpts.Maintenance[(int)Rawr.Hunter.CalculationOptionsHunter.Maintenances.BerserkerRage_]) {
                    SpecialEffect effect = new SpecialEffect(Trigger.Use,
                        new Stats() { BonusAgilityMultiplier = 1f }, // this is just so we can use a Perc Mod without having to make a new stat
                        10f, 30f);
                    float upTime = effect.GetAverageUptime(0, 1f, CombatFactors._c_rwItemSpeed, (CalcOpts.SE_UseDur ? FightDuration : 0f));
                    zerkerMOD *= (1f + upTime);
                }
                float dmgCap = 100f / (RageMod * zerkerMOD); // Can't get any more rage than 100 at any given time
                damagePerSec = (acts * Math.Min(dmgCap, dmg)) / FightDuration;
                
                return (damagePerSec * RageMod * zerkerMOD) * FightDuration;*/
                return 0;
            }
        }
#if RAWR3 || SILVERLIGHT
        protected virtual float RageGenOverDur_Anger { get { return (Talents.AngerManagement / 3.0f) * BossOpts.BerserkTimer; } }
#else
        protected virtual float RageGenOverDur_Anger { get { return (Talents.AngerManagement / 3.0f) * CalcOpts.Duration; } }
#endif

        protected virtual float RageGenOverDur_Other {
            get {
                    float mana = RageGenOverDur_Anger               // Anger Management Talent
                               //+ RageGenOverDur_IncDmg              // Damage from the bosses
                               + (100f * StatS.ManaorEquivRestore)  // 0.02f becomes 2f
                               + StatS.BonusRageGen;                // Bloodrage, Berserker Rage, Might Rage Pot

                    foreach (AbilWrapper aw in GetAbilityList())
                    {
                        if (aw.Mana < 0) 
                            mana += (-1f) * aw.Mana;
                    }

                    // 4pcT7
                    //if (StatS.BonusWarrior_T7_4P_RageProc != 0f) {
                    //    rage += (StatS.BonusWarrior_T7_4P_RageProc * 0.1f) * (Talents.DeepWounds > 0f ? 1f : 0f) * CalcOpts.Duration;
                    //    rage += (StatS.BonusWarrior_T7_4P_RageProc * 0.1f) * (CalcOpts.Maintenance[(int)CalculationOptionsHunter.Maintenances.Rend_] ? 1f / 3f : 0f) * CalcOpts.Duration;
                    //}
                    
                return mana;
            }
        }
        protected float RageNeededOverDur {
            get {
                float mana = 0f;
                foreach (AbilWrapper aw in GetAbilityList())
                {
                    if (aw.Mana > 0f) 
                        mana += aw.Mana;
                }
                return mana;
            }
        }
        #endregion

        #region AddAnItem(s)
        /// <summary>
        /// Adds every maintenance ability to the rotation
        /// </summary>
        /// <param name="totalPercTimeLost">Time lost due to Boss Handler options</param>
        /// <returns>Change in rage from these abilities</returns>
        protected float DoMaintenanceActivates(float totalPercTimeLost)
        {
            float netRage = 0f;

            foreach (AbilWrapper aw in GetMaintenanceAbilities())
            {
                netRage += AddMaintenanceAbility(totalPercTimeLost, aw);
            }
            if (netRage != 0f && _needDisplayCalcs) GCDUsage += Environment.NewLine;
            return netRage;
        }

        /// <summary>
        /// Adds a maintenance ability to the rotation if it's been validated
        /// </summary>
        /// <param name="totalPercTimeLost">Time lost due to stun/fear/movement</param>
        /// <param name="abil">The ability to add</param>
        /// <returns>The final result from Abil.GetManaUseOverDur</returns>
        private float AddMaintenanceAbility(float totalPercTimeLost, AbilWrapper aw)
        {
            if (!aw.ability.Validated) return 0f;

            float Abil_GCDs = Math.Min(GCDsAvailable, aw.ability.Activates * (1f - totalPercTimeLost));
            aw.numActivates = Abil_GCDs;
            //availGCDs -= Abil_GCDs;
            if (_needDisplayCalcs && Abil_GCDs > 0)
                GCDUsage += Abil_GCDs.ToString("000.00") + " : " + aw.ability.Name + (aw.ability.UsesGCD ? "\n" : "(Doesn't use GCDs)\n");

            _HPS_TTL += aw.HPS;
            _DPS_TTL += aw.DPS;
            return aw.ability.GetManaUseOverDur(Abil_GCDs);
        }
        #endregion

        #region Lost Time due to Combat Factors
        //private float /*_emActs/*, _emRecovery, _emRecoveryTotal*/;

        /// <summary>
        /// Calculates percentage of time lost due to moving, being rooted, etc
        /// </summary>
        /// <param name="MS">Placeholder right now for juggernaut handling.  Fury should pass null</param>
        /// <returns>Percentage of time lost as a float</returns>
        protected float CalculateTimeLost(Ability MS)
        {
            //_emActs = 0f; //_emRecovery = 0f; _emRecoveryTotal = 0f;
            TimeLostGCDs = 0;

            float percTimeInMovement = CalculateMovement(MS);
            float percTimeInFear = CalculateFear();
            float percTimeInStun = CalculateStun();
            float percTimeInRoot = CalculateRoot();
            return Math.Min(1f, percTimeInStun + percTimeInMovement + percTimeInFear + percTimeInRoot);
        }

        private float CalculateRoot()
        {
            return 0f; // Not yet implemetned in this model
            /*float percTimeInRoot = 0f;
            if (CalcOpts.RootingTargets && CalcOpts.Roots.Count > 0)
            {
                float timelostwhilerooted = 0f;
                float BaseRootDur = 0f, rootActs = 0f, reducedDur = 0f,
                      MaxTimeRegain = 0f,
                      ChanceYouAreRooted = 1f;
                /*AbilWrapper HF = GetWrapper<HeroicFury>();
                float HFMaxActs = HF.ability.Activates;
                float HFActualActs = 0f;*//*
                AbilWrapper EM = GetWrapper<EveryManForHimself>();
                float EMMaxActs = EM.ability.Activates - EM.numActivates;
                float EMOldActs = EM.numActivates;
                TimesRooted = 0f;
                foreach (Impedence r in CalcOpts.Roots)
                {
                    BaseRootDur = Math.Max(0f, (r.Duration / 1000f * (1f - StatS.SnareRootDurReduc)));
                    rootActs = FightDuration / r.Frequency;
                    if (rootActs > 0f)
                    {
                        TimesRooted += rootActs;
                        /*if (HFMaxActs - HFActualActs > 0f)
                        {
                            MaxTimeRegain = Math.Max(0f, (BaseRootDur - LatentGCD - CalcOpts.React / 1000f));
                            float BZNewActs = Math.Min(HFMaxActs - HFActualActs, rootActs);
                            HFActualActs += BZNewActs;
                            // Use up to the maximum, leaving a 0 boundary so we don't mess up later numbers
                            reducedDur = Math.Max(0f, BaseRootDur - MaxTimeRegain);
                            float percBZdVsUnBZd = BZNewActs / rootActs;
                            timelostwhilerooted += (reducedDur * rootActs * percBZdVsUnBZd * ChanceYouAreRooted)
                                                 + (BaseRootDur * rootActs * (1f - percBZdVsUnBZd) * ChanceYouAreRooted);
                        }
                        else*//* if (Char.Race == CharacterRace.Human && EMMaxActs - EM.numActivates > 0f)
                        {
                            MaxTimeRegain = Math.Max(0f, (BaseRootDur - LatentGCD - CalcOpts.React / 1000f));

                            float availEMacts = EM.ability.Activates - EM.numActivates;
                            float EMNewActs = Math.Min(rootActs, availEMacts);
                            EM.numActivates += EMNewActs;
                            _emActs = EM.numActivates;

                            //EMActualActs += EMNewActs;
                            // Use up to the maximum, leaving a 0 boundary so we don't mess up later numbers
                            reducedDur = Math.Max(0f, BaseRootDur - MaxTimeRegain);
                            float percEMdVsUnEMd = EMNewActs / rootActs;
                            timelostwhilerooted += (reducedDur * rootActs * percEMdVsUnEMd * ChanceYouAreRooted)
                                                 + (BaseRootDur * rootActs * (1f - percEMdVsUnEMd) * ChanceYouAreRooted);
                        }
                        else
                        {
                            timelostwhilerooted += BaseRootDur * rootActs * ChanceYouAreRooted;
                        }
                    }
                }
                //HF.numActivates = HFActualActs;
                if (_needDisplayCalcs)
                {
                    GCDUsage += (TimesFeared > 0 ? TimesFeared.ToString("000.00") + " x " + BaseRootDur.ToString("0.00") + "secs : Lost to Fears\n" : "");
                    //GCDUsage += (HF.numActivates > 0 ? HF.numActivates.ToString("000.00") + " x " + (BaseRootDur - reducedDur).ToString("0.00") + "secs : - " + HF.ability.Name + "\n" : "");
                    GCDUsage += (EM.numActivates - EMOldActs > 0 ? (EM.numActivates - EMOldActs).ToString("000.00") + " x " + (BaseRootDur - reducedDur).ToString("0.00") + "secs : - " + EM.ability.Name + "\n" : "");
                }
                TimeLostGDCs += Math.Min(NumGCDs, (BaseRootDur * TimesFeared) / LatentGCD);
                TimeLostGDCs -= Math.Min(TimeLostGDCs, (reducedDur * HF.numActivates) / LatentGCD);
                TimeLostGDCs -= Math.Min(TimeLostGDCs, (reducedDur * _emActs) / LatentGCD);

                timelostwhilerooted = timelostwhilerooted;
                percTimeInRoot = timelostwhilerooted / FightDuration;
            }
            SecondWind SndW = GetWrapper<SecondWind>().ability as SecondWind;
            SndW.NumStunsOverDur += TimesRooted;
            return percTimeInRoot;*/
        }

        private float CalculateStun()
        {
            return 0f; // Not yet implemented in this model
            /*
            float percTimeInStun = 0f;
            if (CalcOpts.StunningTargets && CalcOpts.Stuns.Count > 0)
            {
                float timelostwhilestunned = 0f;
                float BaseStunDur = 0f, stunActs = 0f, reducedDur = 0f,
                      MaxTimeRegain = 0f,
                      ChanceYouAreStunned = 1f;
                AbilWrapper EM = GetWrapper<EveryManForHimself>();
                float EMMaxActs = EM.ability.Activates - EM.numActivates;
                float EMOldActs = EM.numActivates;
                TimesFeared = 0f;
                foreach (Impedence s in CalcOpts.Stuns)
                {
                    BaseStunDur = Math.Max(0f, (s.Duration / 1000f * (1f - StatS.StunDurReduc)));
                    stunActs = FightDuration / s.Frequency;
                    if (stunActs > 0f)
                    {
                        TimesFeared += stunActs;
                        if (Char.Race == CharacterRace.Human && EMMaxActs - EM.numActivates > 0f) {
                            MaxTimeRegain = Math.Max(0f, (BaseStunDur - LatentGCD - CalcOpts.React / 1000f));

                            float availEMacts = EM.ability.Activates - EM.numActivates;
                            float EMNewActs = Math.Min(stunActs, availEMacts);
                            EM.numActivates += EMNewActs;
                            _emActs = EM.numActivates;

                            //EMActualActs += EMNewActs;
                            // Use up to the maximum, leaving a 0 boundary so we don't mess up later numbers
                            reducedDur = Math.Max(0f, BaseStunDur - MaxTimeRegain);
                            float percEMdVsUnEMd = EMNewActs / stunActs;
                            timelostwhilestunned += (reducedDur * stunActs * percEMdVsUnEMd * ChanceYouAreStunned)
                                                 + (BaseStunDur * stunActs * (1f - percEMdVsUnEMd) * ChanceYouAreStunned);
                        } else {
                            timelostwhilestunned += BaseStunDur * stunActs * ChanceYouAreStunned;
                        }
                    }
                }
                if (_needDisplayCalcs)
                {
                    GCDUsage += (TimesFeared > 0 ? TimesFeared.ToString("000.00") + " x " + BaseStunDur.ToString("0.00") + "secs : Lost to Fears\n" : "");
                    GCDUsage += (EM.numActivates - EMOldActs > 0 ? (EM.numActivates - EMOldActs).ToString("000.00") + " x " + (BaseStunDur - reducedDur).ToString("0.00") + "secs : - " + EM.ability.Name + "\n" : "");
                }
                TimeLostGDCs += Math.Min(NumGCDs, (BaseStunDur * TimesFeared) / LatentGCD);
                TimeLostGDCs -= Math.Min(TimeLostGDCs, (reducedDur * _emActs) / LatentGCD);

                timelostwhilestunned = timelostwhilestunned;
                percTimeInStun = timelostwhilestunned / FightDuration;

                SecondWind SndW = GetWrapper<SecondWind>().ability as SecondWind;
                SndW.NumStunsOverDur += stunActs;
            }
            
            return percTimeInStun;*/
        }

        private float CalculateFear()
        {
            return 0f; // Not yet implemented in this model
            /*
            float percTimeInFear = 0f;
            if (CalcOpts.FearingTargets && CalcOpts.Fears.Count > 0)
            {
                float timelostwhilefeared = 0f;
                float BaseFearDur = 0f, fearActs = 0f, reducedDur = 0f,
                      MaxTimeRegain = 0f,
                      ChanceYouAreFeared = 1f;
                AbilWrapper BZ = GetWrapper<BerserkerRage>();
                float BZMaxActs = BZ.ability.Activates;
                float BZActualActs = 0f;
                AbilWrapper EM = GetWrapper<EveryManForHimself>();
                float EMMaxActs = EM.ability.Activates - EM.numActivates;
                float EMOldActs = EM.numActivates;
                TimesFeared = 0f;
                foreach (Impedence f in CalcOpts.Fears)
                {
                    BaseFearDur = Math.Max(0f, (f.Duration / 1000f * (1f - StatS.FearDurReduc)));
                    fearActs = FightDuration / f.Frequency;
                    if (fearActs > 0f)
                    {
                        TimesFeared += fearActs;
                        if (BZMaxActs - BZActualActs > 0f)
                        {
                            MaxTimeRegain = Math.Max(0f, (BaseFearDur - LatentGCD - CalcOpts.React / 1000f));
                            float BZNewActs = Math.Min(BZMaxActs - BZActualActs, fearActs);
                            BZActualActs += BZNewActs;
                            // Use up to the maximum, leaving a 0 boundary so we don't mess up later numbers
                            reducedDur = Math.Max(0f, BaseFearDur - MaxTimeRegain);
                            float percBZdVsUnBZd = BZNewActs / fearActs;
                            timelostwhilefeared += (reducedDur * fearActs * percBZdVsUnBZd * ChanceYouAreFeared)
                                                 + (BaseFearDur * fearActs * (1f - percBZdVsUnBZd) * ChanceYouAreFeared);
                        }
                        else if (Char.Race == CharacterRace.Human && EMMaxActs - EM.numActivates > 0f)
                        {
                            MaxTimeRegain = Math.Max(0f, (BaseFearDur - LatentGCD - CalcOpts.React / 1000f));

                            float availEMacts = EM.ability.Activates - EM.numActivates;
                            float EMNewActs = Math.Min(fearActs, availEMacts);
                            EM.numActivates += EMNewActs;
                            _emActs = EM.numActivates;

                            //EMActualActs += EMNewActs;
                            // Use up to the maximum, leaving a 0 boundary so we don't mess up later numbers
                            reducedDur = Math.Max(0f, BaseFearDur - MaxTimeRegain);
                            float percEMdVsUnEMd = EMNewActs / fearActs;
                            timelostwhilefeared += (reducedDur * fearActs * percEMdVsUnEMd * ChanceYouAreFeared)
                                                 + (BaseFearDur * fearActs * (1f - percEMdVsUnEMd) * ChanceYouAreFeared);
                        }
                        else
                        {
                            timelostwhilefeared += BaseFearDur * fearActs * ChanceYouAreFeared;
                        }
                    }
                }
                BZ.numActivates = BZActualActs;
                if (_needDisplayCalcs)
                {
                    GCDUsage += (TimesFeared > 0 ? TimesFeared.ToString("000.00") + " x " + BaseFearDur.ToString("0.00") + "secs : Lost to Fears\n" : "");
                    GCDUsage += (BZ.numActivates > 0 ? BZ.numActivates.ToString("000.00") + " x " + (BaseFearDur - reducedDur).ToString("0.00") + "secs : - " + BZ.ability.Name + "\n" : "");
                    GCDUsage += (EM.numActivates - EMOldActs > 0 ? (EM.numActivates - EMOldActs).ToString("000.00") + " x " + (BaseFearDur - reducedDur).ToString("0.00") + "secs : - " + EM.ability.Name + "\n" : "");
                }
                TimeLostGDCs += Math.Min(NumGCDs, (BaseFearDur * TimesFeared) / LatentGCD);
                TimeLostGDCs -= Math.Min(TimeLostGDCs, (reducedDur * BZ.numActivates) / LatentGCD);
                TimeLostGDCs -= Math.Min(TimeLostGDCs, (reducedDur * _emActs) / LatentGCD);

                timelostwhilefeared = timelostwhilefeared;
                percTimeInFear = timelostwhilefeared / FightDuration;
            }
            
            return percTimeInFear;*/
        }

        private float CalculateMovement(Ability MS)
        {
            return 0f; // Not yet handled by this model
            /*
            float percTimeInMovement = 0f;
            if (CalcOpts.MovingTargets && CalcOpts.Moves.Count > 0)
            {
                /* = Movement Speed =
                 * According to a post I found on WoWWiki, Standard (Run) Movement
                 * Speed is 7 yards per 1 sec.
                 * Cat's Swiftness (and similar) bring this to 7.56 (7x1.08)
                 * If you are moving for 5 seconds, this is 35 yards (37.8 w/ bonus)
                 * All the movement effects have a min 8 yards, so you have to be
                 * moving for 1.142857 seconds (1.08 seconds w/ bonus) before Charge
                 * would be viable. If you had to be moving more than Charge's Max
                 * Range (25 yards, editable by certain bonuses) then we'd benefit
                 * again from move speed bonuses, etc.
                 * 
                 * Charge Max = 25
                 * that's 25/7.00 = 3.571428571428571 seconds at 7.00 yards per sec
                 * that's 25/7.56 = 3.306878306873070 seconds at 7.56 yards per sec
                 * Charge (Glyph of Charge) Max = 25+5=30
                 * that's 30/7.00 = 4.285714285714286 seconds at 7.00 yards per sec
                 * that's 30/7.56 = 3.968253968253968 seconds at 7.56 yards per sec
                 * 
                 * = Now let's try and get some of those GCDs back =
                 * Let's assume that if the movement duration is longer
                 * than the before mentioned (1.142857|1.08) seconds,
                 * you are far enough away that you can use a Movement
                 * Ability (Charge, Intercept or  Intervene)
                 * Since some of these abilities are usable in combat
                 * only by talents, we have to make those checks first
                 * Since some stuff is kind of weird, we're going to
                 * enforce an ~3 sec est minimum move time before activating
                 * Charge
                 *//*
                AbilWrapper HF = GetWrapper<HeroicFury>();
                AbilWrapper CH;
                if (CalcOpts.FuryStance) CH = GetWrapper<Intercept>();
                else CH = GetWrapper<Charge>();
                
                float MovementSpeed = 7f * (1f + StatS.MovementSpeed); // 7 yards per sec * 1.08 (if have bonus) = 7.56
                float BaseMoveDur = 0f, movedActs = 0f, reducedDur = 0f,
                      MinMovementTimeRegain = 0f, MaxMovementTimeRegain = 0f,
                      ChanceYouHaveToMove = 1f;
                float ChargeMaxActs = CH.ability.Activates;
                if (CalcOpts.FuryStance && HF.ability.Validated)
                {
                    ChargeMaxActs += HF.ability.Activates - HF.numActivates;
                }
                float ChargeActualActs = 0f;
                float timelostwhilemoving = 0f;
                float moveGCDs = 0f;
                foreach (Impedence m in CalcOpts.Moves)
                {
                    BaseMoveDur = (m.Duration / 1000f * (1f - StatS.MovementSpeed));
                    moveGCDs += movedActs = FightDuration / m.Frequency;

                    if ((ChargeMaxActs - ChargeActualActs > 0f) && (movedActs > 0f))
                    {
                        MaxMovementTimeRegain = Math.Max(0f,
                            Math.Min((BaseMoveDur - CalcOpts.React / 1000f),
                                     (CH.ability.MaxRange / MovementSpeed - CalcOpts.React / 1000f)));
                        MinMovementTimeRegain = Math.Max(0f,
                            Math.Min((BaseMoveDur - CalcOpts.React / 1000f),
                                     (CH.ability.MinRange / MovementSpeed - CalcOpts.React / 1000f)));
                        if (BaseMoveDur >= MinMovementTimeRegain)
                        {
                            float ChargeNewActs = Math.Min(ChargeMaxActs - ChargeActualActs, movedActs);
                            ChargeActualActs += ChargeNewActs;
                            // Use up to the maximum, leaving a 0 boundary so we don't mess up later numbers
                            reducedDur = Math.Max(0f, BaseMoveDur - MaxMovementTimeRegain);
                            float percChargedVsUncharged = ChargeNewActs / movedActs;
                            timelostwhilemoving += (reducedDur * movedActs * percChargedVsUncharged * ChanceYouHaveToMove)
                                                 + (BaseMoveDur * movedActs * (1f - percChargedVsUncharged) * ChanceYouHaveToMove);
                        }
                    }
                    else if (movedActs > 0f)
                    {
                        timelostwhilemoving += BaseMoveDur * movedActs * ChanceYouHaveToMove;
                    }
                }
                float actsCharge = ChargeActualActs;
                
                TimeLostGDCs += Math.Min(NumGCDs, (BaseMoveDur * moveGCDs) / LatentGCD);
                TimeLostGDCs -= Math.Min(TimeLostGDCs, (reducedDur * actsCharge) / LatentGCD);
                CH.numActivates = actsCharge;
                float actsHF = 0f;
                if (CH.numActivates > CH.ability.Activates) actsHF += (CH.numActivates - CH.ability.Activates);
                HF.numActivates += actsHF;
                if (_needDisplayCalcs)
                {
                    GCDUsage += (moveGCDs > 0 ? moveGCDs.ToString("000.00") + " x " + BaseMoveDur.ToString("0.00") + "secs : Lost to Movement\n" : "");
                    GCDUsage += (actsCharge > 0 ? actsCharge.ToString("000.00") + " x " + (BaseMoveDur - reducedDur).ToString("0.00") + "secs : - " + CH.ability.Name + "\n" : "");
                    GCDUsage += (actsHF > 0 ? actsHF.ToString("000.00") + " activates of " + HF.ability.Name + " to refresh " + CH.ability.Name : "");
                }
                //RageGainedWhileMoving += CH.Rage;
                // Need to add the special effect from Juggernaut to Mortal Strike, not caring about Slam right now
                if (Talents.Juggernaut > 0 && MS != null && CH.ability is Charge)
                {
                    Stats stats = new Stats
                    {
                        BonusWarrior_T8_4P_MSBTCritIncrease = 0.25f *
                            (new SpecialEffect(Trigger.Use, null, 10, CH.ability.Cd)
                             ).GetAverageUptime(FightDuration / actsCharge, 1f, CombatFactors._c_rwItemSpeed, FightDuration)
                    };
                    stats.Accumulate(StatS);
                    // I'm not sure if this is gonna work, but hell, who knows
                    MS.BonusCritChance = stats.BonusWarrior_T8_4P_MSBTCritIncrease;
                    //MS = new Skills.MortalStrike(Char, stats, CombatFactors, WhiteAtks, CalcOpts);
                }
                timelostwhilemoving = timelostwhilemoving;
                percTimeInMovement = timelostwhilemoving / FightDuration;
            }
            return percTimeInMovement;*/
        }
        #endregion

        public void AddValidatedSpecialEffects(Stats statsTotal, WarriorTalents talents)
        {
            /*Ability ST = GetWrapper<ShatteringThrow>().ability,
                    BTS = GetWrapper<BattleShout>().ability,
                    CS = GetWrapper<CommandingShout>().ability,
                    DS = GetWrapper<DemoralizingShout>().ability,
                    TH = GetWrapper<ThunderClap>().ability,
                    SN = GetWrapper<SunderArmor>().ability,
                    SW = GetWrapper<SweepingStrikes>().ability,
                    RK = GetWrapper<Recklessness>().ability;
            if (ST.Validated)
            {
                SpecialEffect shatt = new SpecialEffect(Trigger.Use,
                    new Stats() { ArmorPenetration = 0.20f, },
                    ST.Duration, ST.Cd,
                    ST.RWAtkTable.AnyLand);
                statsTotal.AddSpecialEffect(shatt);
            }
            if (BTS.Validated)
            {
                SpecialEffect bs = new SpecialEffect(Trigger.Use,
                    new Stats() { AttackPower = (548f * (1f + talents.CommandingPresence * 0.05f)), },
                    BTS.Duration, BTS.Cd);
                statsTotal.AddSpecialEffect(bs);
            }
            if (CS.Validated)
            {
                //float value = (2255f * (1f + talents.CommandingPresence * 0.05f));
                SpecialEffect cs = new SpecialEffect(Trigger.Use,
                    new Stats() { Health = 2255f * (1f + talents.CommandingPresence * 0.05f), },
                    CS.Duration, CS.Cd);
                statsTotal.AddSpecialEffect(cs);
            }
            if (DS.Validated)
            {
                //float value = (410f * (1f + talents.ImprovedDemoralizingShout * 0.08f));
                SpecialEffect ds = new SpecialEffect(Trigger.Use,
                    new Stats() { BossAttackPower = 410f * (1f + talents.ImprovedDemoralizingShout * 0.08f) * -1f, },
                    DS.Duration, DS.Cd);
                statsTotal.AddSpecialEffect(ds);
            }
            if (TH.Validated)
            {
                //float value = (0.10f * (1f + (float)Math.Ceiling(talents.ImprovedThunderClap * 10f / 3f) / 100f));
                SpecialEffect tc = new SpecialEffect(Trigger.Use,
                    new Stats() { BossAttackSpeedMultiplier = (-0.10f * (1f + talents.ImprovedThunderClap / 30f)), },
                    TH.Duration, TH.Cd, TH.RWAtkTable.AnyLand);
                statsTotal.AddSpecialEffect(tc);
            }
            if (SN.Validated)
            {
                //float value = 0.04f;
                SpecialEffect sn = new SpecialEffect(Trigger.Use,
                    new Stats() { ArmorPenetration = 0.04f, },
                    SN.Duration, SN.Cd, SN.RWAtkTable.AnyLand, 5);
                statsTotal.AddSpecialEffect(sn);
            }
            float landedAtksInterval = LandedAtksOverDur / CalcOpts.Duration;
            float critRate = CriticalAtksOverDur / AttemptedAtksOverDur;
            if (SW.Validated)
            {
                SpecialEffect sweep = new SpecialEffect(Trigger.Use,
                    new Stats() { BonusTargets = 1f, },
                    landedAtksInterval * 5f, SW.Cd);
                statsTotal.AddSpecialEffect(sweep);
            }
            /*if (RK.Validated && CalcOpts.FuryStance)
            {
                SpecialEffect reck = new SpecialEffect(Trigger.Use,
                    new Stats() { PhysicalCrit = 1f - critRate },
                    landedAtksInterval * 3f, RK.Cd);
                statsTotal.AddSpecialEffect(reck);
            }*/
            /*if (talents.Flurry > 0 && CalcOpts.FuryStance)
            {
                //float value = talents.Flurry * 0.05f;
                SpecialEffect flurry = new SpecialEffect(Trigger.MeleeCrit,
                    new Stats() { PhysicalHaste = talents.Flurry * 0.05f, }, landedAtksInterval * 3f, 0f);
                statsTotal.AddSpecialEffect(flurry);
            }*/
        }

        internal void ResetHitTables() { foreach (AbilWrapper aw in GetAbilityList()) { if (aw.ability.CanCrit) { aw.ability.RWAtkTable.Reset(); } } }
    }
}
