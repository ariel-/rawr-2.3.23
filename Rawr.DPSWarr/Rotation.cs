using System;
using System.Collections.Generic;
using System.Text;
using Rawr.DPSWarr.Skills;
using System.Threading;

namespace Rawr.DPSWarr {
    public class Rotation {
        public class AbilWrapper
        {
            public AbilWrapper(Skills.Ability abil) {
                ability = abil;
                _cachedIsDamaging = ability.DamageOverride + ability.DamageOnUseOverride > 0f;
            }
            public Skills.Ability ability { get; set; }
            public float numActivates { get; set; }
            public float Rage { get { return ability.GetRageUseOverDur(numActivates); } }
            public float DPS { get { return ability.GetDPS(numActivates); } }
            public float HPS { get { return ability.GetHPS(numActivates); } }
            public float numActivatesU20 { get; set; }
            public float RageU20 { get { return ability.GetRageUseOverDur(numActivatesU20); } }
            public float DPSU20 { get { return ability.GetDPS(numActivatesU20); } }
            public float HPSU20 { get { return ability.GetHPS(numActivatesU20); } }
            public float allNumActivates { get { return numActivates + numActivatesU20; } }
            public float allRage { get { return Rage + RageU20; } }
            public float allDPS { get { return DPS + DPSU20; } }
            public float allHPS { get { return HPS + HPSU20; } }
            protected bool _cachedIsDamaging = false;
            public bool isDamaging { get { return _cachedIsDamaging; } }
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

        public List<AbilWrapper> GetAbilityListThatGCDs()
        {
            return new List<AbilWrapper>(AbilityList.Values).FindAll(e => e.ability.UsesGCD);
        }

        public AbilWrapper GetWrapper<T>()
        {
            return AbilityList[typeof(T)];
        }

        public float _HPS_TTL;
        public float _DPS_TTL, _DPS_TTL_U20;
        public string GCDUsage = "";
        protected CharacterCalculationsDPSWarr calcs = null;
        
        public bool _needDisplayCalcs = true;
        
        //protected float FightDuration;
        protected float TimeLostGCDs;
        protected float RageGainedWhileMoving;
        public float TimesStunned = 0f;
        public float TimesFeared = 0f;
        public float TimesRooted = 0f;
        public float TimesDisarmed = 0f;
        
        public Skills.DeepWounds DW;

        #endregion
        #region Get/Set
        protected Character Char { get; set; }
        protected WarriorTalents Talents { get; set; }
        protected WarriorTalentsCata TalentsCata { get; set; }
        protected Stats StatS { get; set; }
        protected CombatFactors CombatFactors { get; set; }
        public Skills.WhiteAttacks WhiteAtks { get; protected set; }
        protected CalculationOptionsDPSWarr CalcOpts { get; set; }
        protected BossOptions BossOpts { get; set; }

        protected float _cachedLatentGCD = 1.5f;
        protected float LatentGCD { get { return _cachedLatentGCD; } }
        
        /// <summary>
        /// How many GCDs are in the rotation, based on fight duration and latency
        /// </summary>
        protected float NumGCDs { get { return _cachedNumGCDs; } }
        protected float _cachedNumGCDs = 0f;
        /// <summary>
        /// How many GCDs are in the rotation, based on fight duration and latency, under 20%
        /// </summary>
        protected float NumGCDsU20 { get { return _cachedNumGCDsU20; } }
        protected float _cachedNumGCDsU20 = 0f;
        
        /// <summary>
        /// How many GCDs have been used by the rotation
        /// </summary>
        protected float GCDsUsed
        {
            get
            {
                float gcds = 0f;
                foreach (AbilWrapper aw in GetAbilityListThatGCDs()/*GetAbilityList()*/)
                {
                    //if (aw.ability.UsesGCD)
                        gcds += aw.numActivates * aw.ability.UseTime / LatentGCD;
                }
                return gcds;
            }
        }
        /// <summary>
        /// How many GCDs have been used by the rotation
        /// </summary>
        protected float GCDsUsedU20
        {
            get
            {
                float gcds = 0f;
                foreach (AbilWrapper aw in GetAbilityListThatGCDs())
                {
                    gcds += aw.numActivatesU20 * aw.ability.UseTime / LatentGCD;
                }
                return gcds;
            }
        }


        /// <summary>
        /// How many GCDs are still available in the rotation
        /// </summary>
        protected float GCDsAvailable { get { return Math.Max(0f, NumGCDs - GCDsUsed - TimeLostGCDs); } }
        /// <summary>
        /// How many GCDs are still available in the rotation sub 20%
        /// </summary>
        protected float GCDsAvailableU20 { get { return Math.Max(0f, NumGCDsU20 - GCDsUsedU20 - TimeLostGCDs); } }
        
        #endregion
        #region Functions
        public virtual void Initialize(CharacterCalculationsDPSWarr calcs) {
            this.calcs = calcs;
            StatS = calcs.AverageStats;

            initAbilities();
            //doIterations();

            // Whites
            calcs.Whites = WhiteAtks;
            // Anti-Debuff
            /*
            calcs.HF = HF;
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
            calcs.RK = RK;
            
            // Shared Instants
            calcs.WW = WW;
            calcs.SL = SL;
            // Arms
            
            // Generic
            calcs.CL = CL;
            calcs.DW = DW;
            calcs.HS = HS;
            calcs.EX = EX;*/
            //calcs.DW = DW;
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
            AddAbility(new AbilWrapper(new Skills.HeroicFury(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.EveryManForHimself(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.Charge(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.Intercept(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.Intervene(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            // Rage Generators
            AddAbility(new AbilWrapper(new Skills.SecondWind(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.BerserkerRage(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.Bloodrage(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            // Maintenance
            AddAbility(new AbilWrapper(new Skills.BattleShout(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.CommandingShout(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.DemoralizingShout(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.SunderArmor(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.ThunderClap(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.Hamstring(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.EnragedRegeneration(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            // Periodics
            AddAbility(new AbilWrapper(new Skills.ShatteringThrow(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.SweepingStrikes(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.DeathWish(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.Recklessness(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));

            // Slam used by Bloodsurge, WW used by Bladestorm, so they're shared
            Slam SL = new Skills.Slam(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts);
            AddAbility(new AbilWrapper(SL));
            Skills.Ability WW = new Skills.WhirlWind(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts);
            AddAbility(new AbilWrapper(WW));

            AddAbility(new AbilWrapper(new Skills.Cleave(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.HeroicStrike(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            Skills.Ability EX = new Skills.Execute(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts);
            AddAbility(new AbilWrapper(EX));

            // Arms abilities
            AddAbility(new AbilWrapper(new Skills.Bladestorm(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts, WW)));
            AddAbility(new AbilWrapper(new Skills.MortalStrike(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.Rend(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            Skills.Ability SS = new Skills.Swordspec(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts);
            AddAbility(new AbilWrapper(SS));
            AddAbility(new AbilWrapper(new Skills.OverPower(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts, SS)));
            AddAbility(new AbilWrapper(new Skills.TasteForBlood(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts)));
            AddAbility(new AbilWrapper(new Skills.Suddendeath(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts, EX)));
            
            // Fury abilities
            Ability BT = new Skills.BloodThirst(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts);
            AddAbility(new AbilWrapper(BT));
            AddAbility(new AbilWrapper(new Skills.BloodSurge(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts, SL, WW, BT)));

            DW = new Skills.DeepWounds(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, BossOpts);

        }

        private void AddAbility(AbilWrapper abilWrapper)
        {
            try
            {
                AbilityList.Add(abilWrapper.ability.GetType(), abilWrapper);
            }
            catch (Exception ex) {
                Rawr.Base.ErrorBox eb = new Rawr.Base.ErrorBox("Error in adding an Ability Wrapper",
                    ex.Message, "AddAbility(...)", "No Additional Info", ex.StackTrace);
                eb.Show();
            }
        }

        public virtual void doIterations() { }

        protected virtual void calcDeepWounds() {
            // Main Hand
            float mhActivates =
                /*Yellow  */CriticalYellowsOverDurMH + 
                /*White   */WhiteAtks.MhActivates * WhiteAtks.MHAtkTable.Crit;

            // Off Hand
            float ohActivates = (CombatFactors.useOH ?
                // No OnAttacks for OH
                /*Yellow*/CriticalYellowsOverDurOH +
                /*White */WhiteAtks.OhActivates * WhiteAtks.OHAtkTable.Crit
                : 0f);

            // Push to the Ability
            DW.SetAllAbilityActivates(mhActivates, ohActivates); 
        }

        public void InvalidateCache()
        {
            for (int i = 0; i < 5; i++) for (int j = 0; j < 3; j++) for (int k = 0; k < 3; k++)
                _atkOverDurs[i,j,k] = -1f;
        }

        #region Attacks over Duration
        public enum SwingResult : int { Attempt=0, Land, Crit, Parry, Dodge };
        public enum Hand : int { MH=0, OH, Both };
        public enum AttackType : int { Yellow=0, White, Both };
        private float[,,] _atkOverDurs = new float[5, 3, 3];
        public float GetAttackOverDuration(SwingResult swingResult, Hand hand, AttackType attackType)
        {
            if (_atkOverDurs[(int)swingResult, (int)hand, (int)attackType] == -1f)
            {
                SetTable(swingResult, hand, attackType);
            }
            return _atkOverDurs[(int)swingResult, (int)hand, (int)attackType];
        }
        private void SetTable(SwingResult sr, Hand h, AttackType at)
        {
            float count = 0f;
            float mod;
            CombatTable table;

            if (at != AttackType.White) {
                foreach (AbilWrapper abil in GetDamagingAbilities())
                {
                    if (!abil.ability.Validated)
                    {
                        continue;
                    }
                    if (h != Hand.OH)
                    {
                        table = abil.ability.MHAtkTable;
                        mod = GetTableFromSwingResult(sr, table);
                        count += abil.allNumActivates * abil.ability.AvgTargets * abil.ability.SwingsPerActivate * mod;
                    }
                    if (h != Hand.MH && CombatFactors.useOH && abil.ability.SwingsOffHand)
                    {
                        table = abil.ability.OHAtkTable;
                        mod = GetTableFromSwingResult(sr, table);
                        count += abil.allNumActivates * abil.ability.AvgTargets * mod;
                    }
                }
            }
            if (at != AttackType.Yellow) {
                if (h != Hand.OH)
                {
                    table = WhiteAtks.MHAtkTable;
                    mod = GetTableFromSwingResult(sr, table);
                    count += WhiteAtks.MhActivates * mod;
                }
                if (h != Hand.MH && CombatFactors.useOH)
                {
                    table = WhiteAtks.OHAtkTable;
                    mod = GetTableFromSwingResult(sr, table);
                    count += WhiteAtks.OhActivates * mod;
                }
            }
            
            _atkOverDurs[(int)sr, (int)h, (int)at] = count;
        }
        private float GetTableFromSwingResult(SwingResult sr, CombatTable table)
        {
            if (table == null) return 0f;
            switch (sr)
            {
                case SwingResult.Attempt: return 1f;
                case SwingResult.Crit:    return table.Crit;
                case SwingResult.Dodge:   return table.Dodge;
                case SwingResult.Land:    return table.AnyLand;
                case SwingResult.Parry:   return table.Parry;
                default:                  return 0f;
            }
        }

        public float AttemptedAtksOverDur { get { return GetAttackOverDuration(SwingResult.Attempt, Hand.Both, AttackType.Both); } }
        public float AttemptedYellowsOverDur { get { return GetAttackOverDuration(SwingResult.Attempt, Hand.Both, AttackType.Yellow); } }
        public float LandedAtksOverDur { get { return GetAttackOverDuration(SwingResult.Land, Hand.Both, AttackType.Both); } }
        public float CriticalAtksOverDur { get { return GetAttackOverDuration(SwingResult.Crit, Hand.Both, AttackType.Both); } }

        public float AttemptedAtksOverDurMH { get { return GetAttackOverDuration(SwingResult.Attempt, Hand.MH, AttackType.Both); } }
        public float LandedAtksOverDurMH { get { return GetAttackOverDuration(SwingResult.Land, Hand.MH, AttackType.Both); } }

        public float AttemptedAtksOverDurOH { get { return GetAttackOverDuration(SwingResult.Attempt, Hand.OH, AttackType.Both); } }
        public float LandedAtksOverDurOH { get { return GetAttackOverDuration(SwingResult.Land, Hand.OH, AttackType.Both); } }

        public float CriticalYellowsOverDur { get { return GetAttackOverDuration(SwingResult.Crit, Hand.Both, AttackType.Both); } }
        public float LandedYellowsOverDur { get { return GetAttackOverDuration(SwingResult.Land, Hand.Both, AttackType.Both); } }

        public float CriticalYellowsOverDurMH { get { return GetAttackOverDuration(SwingResult.Crit, Hand.MH, AttackType.Yellow); } }
        public float CriticalYellowsOverDurOH { get { return GetAttackOverDuration(SwingResult.Crit, Hand.OH, AttackType.Yellow); } }
        
        public float DodgedYellowsOverDur { get { return GetAttackOverDuration(SwingResult.Dodge, Hand.Both, AttackType.Yellow); } }
        public float ParriedYellowsOverDur { get { return GetAttackOverDuration(SwingResult.Parry, Hand.Both, AttackType.Yellow); } }
        public virtual float CritHsSlamOverDur {
            get {
                AbilWrapper HS = GetWrapper<HeroicStrike>();
                AbilWrapper SL = GetWrapper<Slam>();
                AbilWrapper BS = GetWrapper<BloodSurge>();
                return HS.allNumActivates * HS.ability.MHAtkTable.Crit * HS.ability.AvgTargets +
                       SL.allNumActivates * SL.ability.MHAtkTable.Crit * SL.ability.AvgTargets +
                       BS.allNumActivates * BS.ability.MHAtkTable.Crit * BS.ability.AvgTargets;
            }
        }

        #endregion

        public float MaintainCDs {
            get {
                float cds = 0f;
                foreach (AbilWrapper aw in GetMaintenanceAbilities()) cds += aw.allNumActivates;
                return cds;
            }
        }
        #endregion
        #region Rage Calcs
        protected virtual float RageGenOverDur_IncDmg {
            get {
                // Invalidate bad things
#if RAWR3 || SILVERLIGHT
                List<Attack> aoeAtks = BossOpts.GetFilteredAttackList(ATTACK_TYPES.AT_AOE);
                Attack dynAoE = BossOpts.DynamicCompiler_FilteredAttacks(aoeAtks);
                if (aoeAtks.Count > 0 || dynAoE.AttackSpeed <= 0 || dynAoE.DamagePerHit <= 0) { return 0f; }
#else
                if (!CalcOpts.AoETargets || CalcOpts.AoETargetsFreq < 1 || CalcOpts.AoETargetsDMG < 1) { return 0f; }
#endif
                float RageMod = 2.5f / 453.3f;
                float damagePerSec = 0f;
                float freq =
#if RAWR3 || SILVERLIGHT
                    dynAoE.AttackSpeed;
#else
                    CalcOpts.AoETargetsFreq;
#endif
                float dmg =
#if RAWR3 || SILVERLIGHT
                    dynAoE.DamagePerHit
#else
                    CalcOpts.AoETargetsDMG
#endif
                    * (1f + StatS.DamageTakenMultiplier) + StatS.BossAttackPower / 14f;
                float acts = FightDuration / freq;
                // Add Berserker Rage's
                float zerkerMOD = 1f;
                if (CalcOpts.Maintenance[(int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.BerserkerRage_]) {
                    float upTime = _SE_ZERKERDUMMY.GetAverageUptime(0, 1f, CombatFactors._c_mhItemSpeed, (CalcOpts.SE_UseDur ? FightDuration : 0f));
                    zerkerMOD *= (1f + upTime);
                }
                float dmgCap = 100f / (RageMod * zerkerMOD); // Can't get any more rage than 100 at any given time
                damagePerSec = (acts * Math.Min(dmgCap, dmg)) / FightDuration;
                
                return (damagePerSec * RageMod * zerkerMOD) * FightDuration;
            }
        }
        
        protected virtual float RageGenOverDur_Anger {
            get {
                return (Talents.AngerManagement / 3.0f) * FightDuration;
            }
        }
        
        protected virtual float RageGenOverDur_Other {
            get {
                    float rage = RageGenOverDur_Anger               // Anger Management Talent
                               + RageGenOverDur_IncDmg              // Damage from the bosses
                               + (100f * StatS.ManaorEquivRestore)  // 0.02f becomes 2f
                               + StatS.BonusRageGen;                // Bloodrage, Berserker Rage, Might Rage Pot

                    foreach (AbilWrapper aw in GetAbilityList())
                    {
                        if (aw.allRage < 0) 
                            rage += (-1f) * aw.allRage;
                    }

                    // 4pcT7
                    if (StatS.BonusWarrior_T7_4P_RageProc != 0f) {
                        rage += (StatS.BonusWarrior_T7_4P_RageProc * 0.1f) * (Talents.DeepWounds > 0f ? 1f : 0f) * FightDuration;
                        rage += (StatS.BonusWarrior_T7_4P_RageProc * 0.1f) * (!CombatFactors.FuryStance && CalcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Rend_] ? 1f / 3f : 0f) * FightDuration;
                    }
                    
                return rage;
            }
        }
        public int FightDuration {
            get {
                return
#if RAWR3 || SILVERLIGHT
                    BossOpts.BerserkTimer;
#else
                    (int)CalcOpts.Duration;
#endif
            }
        }
        protected float RageNeededOverDur {
            get {
                float rage = 0f;
                foreach (AbilWrapper aw in GetAbilityList())
                {
                    if (aw.allRage > 0f) 
                        rage += aw.allRage;
                }
                return rage;
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
        /// <returns>The final result from Abil.GetRageUseOverDur</returns>
        private float AddMaintenanceAbility(float totalPercTimeLost, AbilWrapper aw)
        {
            if (!aw.ability.Validated) return 0f;

            float Abil_GCDs = Math.Min(GCDsAvailable, aw.ability.Activates * (1f - totalPercTimeLost));
            Abil_GCDs = CalcOpts.AllowFlooring ? (float)Math.Floor(Abil_GCDs) : Abil_GCDs;
            aw.numActivates = Abil_GCDs;
            //availGCDs -= Abil_GCDs;
            if (_needDisplayCalcs && Abil_GCDs > 0)
                GCDUsage += Abil_GCDs.ToString(CalcOpts.AllowFlooring ? "000" : "000.00") + " : " + aw.ability.Name + (aw.ability.UsesGCD ? "\n" : "(Doesn't use GCDs)\n");

            _HPS_TTL += aw.allHPS;
            _DPS_TTL += aw.allDPS;
            return aw.ability.GetRageUseOverDur(Abil_GCDs);
        }
        #endregion

        #region Lost Time due to Combat Factors
        private float _emActs/*, _emRecovery, _emRecoveryTotal*/;

        /// <summary>
        /// Calculates percentage of time lost due to moving, being rooted, etc
        /// </summary>
        /// <param name="MS">Placeholder right now for juggernaut handling.  Fury should pass null</param>
        /// <returns>Percentage of time lost as a float</returns>
        protected float CalculateTimeLost(Ability MS)
        {
            _emActs = 0f; //_emRecovery = 0f; _emRecoveryTotal = 0f;
            TimeLostGCDs = 0;
            RageGainedWhileMoving = 0; 

            float percTimeInMovement = CalculateMovement(MS);
            float percTimeInFear = CalculateFear();
            float percTimeInStun = CalculateStun();
            float percTimeInRoot = CalculateRoot();
            return Math.Min(1f, percTimeInStun + percTimeInMovement + percTimeInFear + percTimeInRoot);
        }

        private float CalculateRoot()
        {
            float percTimeInRoot = 0f;
#if RAWR3 || SILVERLIGHT
            if (BossOpts.RootingTargs && BossOpts.Roots.Count > 0)
#else
            if (CalcOpts.RootingTargets && CalcOpts.Roots.Count > 0)
#endif
            {
                float timelostwhilerooted = 0f;
                float BaseRootDur = 0f, rootActs = 0f, reducedDur = 0f,
                      MaxTimeRegain = 0f,
                      ChanceYouAreRooted = 1f;
                AbilWrapper HF = GetWrapper<HeroicFury>();
                float HFMaxActs = CalcOpts.AllowFlooring ? (float)Math.Floor(HF.ability.Activates) : HF.ability.Activates;
                float HFActualActs = 0f;
                AbilWrapper EM = GetWrapper<EveryManForHimself>();
                float EMMaxActs = (CalcOpts.AllowFlooring ? (float)Math.Floor(EM.ability.Activates) : EM.ability.Activates) - EM.allNumActivates;
                float EMOldActs = EM.allNumActivates;
                TimesRooted = 0f;
                foreach (Impedance r in
#if RAWR3 || SILVERLIGHT
                    BossOpts.Roots
#else
                    CalcOpts.Roots
#endif
)
                {
                    ChanceYouAreRooted = r.Chance;
                    BaseRootDur = Math.Max(0f, (r.Duration / 1000f * (1f - StatS.SnareRootDurReduc)));
                    rootActs = CalcOpts.AllowFlooring ? (float)Math.Ceiling(FightDuration / r.Frequency) : FightDuration / r.Frequency;
                    if (rootActs > 0f)
                    {
                        TimesRooted += rootActs;
                        if (HFMaxActs - HFActualActs > 0f)
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
                        else if (Char.Race == CharacterRace.Human && EMMaxActs - EM.allNumActivates > 0f)
                        {
                            MaxTimeRegain = Math.Max(0f, (BaseRootDur - LatentGCD - CalcOpts.React / 1000f));

                            float availEMacts = (CalcOpts.AllowFlooring ? (float)Math.Floor(EM.ability.Activates) : EM.ability.Activates) - EM.allNumActivates;
                            float EMNewActs = Math.Min(rootActs, availEMacts);
                            EM.numActivates += EMNewActs;
                            _emActs = EM.allNumActivates;

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
                HF.numActivates = HFActualActs;
                if (_needDisplayCalcs)
                {
                    GCDUsage += (TimesFeared > 0 ? TimesFeared.ToString(CalcOpts.AllowFlooring ? "000" : "000.00") + " x " + BaseRootDur.ToString("0.00") + "secs : Lost to Fears\n" : "");
                    GCDUsage += (HF.allNumActivates > 0 ? HF.allNumActivates.ToString(CalcOpts.AllowFlooring ? "000" : "000.00") + " x " + (BaseRootDur - reducedDur).ToString("0.00") + "secs : - " + HF.ability.Name + "\n" : "");
                    GCDUsage += (EM.allNumActivates - EMOldActs > 0 ? (EM.allNumActivates - EMOldActs).ToString(CalcOpts.AllowFlooring ? "000" : "000.00") + " x " + (BaseRootDur - reducedDur).ToString("0.00") + "secs : - " + EM.ability.Name + "\n" : "");
                }
                TimeLostGCDs += Math.Min(NumGCDs, (BaseRootDur * TimesFeared) / LatentGCD);
                TimeLostGCDs -= Math.Min(TimeLostGCDs, (reducedDur * HF.allNumActivates) / LatentGCD);
                TimeLostGCDs -= Math.Min(TimeLostGCDs, (reducedDur * _emActs) / LatentGCD);

                timelostwhilerooted = CalcOpts.AllowFlooring ? (float)Math.Ceiling(timelostwhilerooted) : timelostwhilerooted;
                percTimeInRoot = timelostwhilerooted / FightDuration;
            }
            SecondWind SndW = GetWrapper<SecondWind>().ability as SecondWind;
            SndW.NumStunsOverDur += TimesRooted;
            return percTimeInRoot;
        }

        private float CalculateStun()
        {
            float percTimeInStun = 0f;
#if RAWR3 || SILVERLIGHT
            if (BossOpts.StunningTargs && BossOpts.Stuns.Count > 0)
#else
            if (CalcOpts.StunningTargets && CalcOpts.Stuns.Count > 0)
#endif
            {
                float timelostwhilestunned = 0f;
                float BaseStunDur = 0f, stunActs = 0f, reducedDur = 0f,
                      MaxTimeRegain = 0f,
                      ChanceYouAreStunned = 1f;
                AbilWrapper EM = GetWrapper<EveryManForHimself>();
                float EMMaxActs = (CalcOpts.AllowFlooring ? (float)Math.Floor(EM.ability.Activates) : EM.ability.Activates) - EM.allNumActivates;
                float EMOldActs = EM.allNumActivates;
                TimesFeared = 0f;
#if RAWR3 || SILVERLIGHT
                foreach (Impedance s in BossOpts.Stuns)
#else
                foreach (Impedance s in CalcOpts.Stuns)
#endif
                {
                    ChanceYouAreStunned = s.Chance;
                    BaseStunDur = Math.Max(0f, (s.Duration / 1000f * (1f - StatS.StunDurReduc)));
                    stunActs = CalcOpts.AllowFlooring ? (float)Math.Ceiling(FightDuration / s.Frequency) : FightDuration / s.Frequency;
                    if (stunActs > 0f)
                    {
                        TimesFeared += stunActs;
                        if (Char.Race == CharacterRace.Human && EMMaxActs - EM.allNumActivates > 0f) {
                            MaxTimeRegain = Math.Max(0f, (BaseStunDur - LatentGCD - CalcOpts.React / 1000f));

                            float availEMacts = (CalcOpts.AllowFlooring ? (float)Math.Floor(EM.ability.Activates) : EM.ability.Activates) - EM.allNumActivates;
                            float EMNewActs = Math.Min(stunActs, availEMacts);
                            EM.numActivates += EMNewActs;
                            _emActs = EM.allNumActivates;

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
                    GCDUsage += (TimesFeared > 0 ? TimesFeared.ToString(CalcOpts.AllowFlooring ? "000" : "000.00") + " x " + BaseStunDur.ToString("0.00") + "secs : Lost to Fears\n" : "");
                    GCDUsage += (EM.allNumActivates - EMOldActs > 0 ? (EM.allNumActivates - EMOldActs).ToString(CalcOpts.AllowFlooring ? "000" : "000.00") + " x " + (BaseStunDur - reducedDur).ToString("0.00") + "secs : - " + EM.ability.Name + "\n" : "");
                }
                TimeLostGCDs += Math.Min(NumGCDs, (BaseStunDur * TimesFeared) / LatentGCD);
                TimeLostGCDs -= Math.Min(TimeLostGCDs, (reducedDur * _emActs) / LatentGCD);

                timelostwhilestunned = CalcOpts.AllowFlooring ? (float)Math.Ceiling(timelostwhilestunned) : timelostwhilestunned;
                percTimeInStun = timelostwhilestunned / FightDuration;

                SecondWind SndW = GetWrapper<SecondWind>().ability as SecondWind;
                SndW.NumStunsOverDur += stunActs;
            }
            
            return percTimeInStun;
        }

        private float CalculateFear()
        {
            float percTimeInFear = 0f;
#if RAWR3 || SILVERLIGHT
            if (BossOpts.FearingTargs && BossOpts.Fears.Count > 0)
#else
            if (CalcOpts.FearingTargets && CalcOpts.Fears.Count > 0)
#endif
            {
                float timelostwhilefeared = 0f;
                float BaseFearDur = 0f, fearActs = 0f, reducedDur = 0f,
                      MaxTimeRegain = 0f,
                      ChanceYouAreFeared = 1f;
                AbilWrapper BZ = GetWrapper<BerserkerRage>();
                float BZMaxActs = CalcOpts.AllowFlooring ? (float)Math.Floor(BZ.ability.Activates) : BZ.ability.Activates;
                float BZActualActs = 0f;
                AbilWrapper EM = GetWrapper<EveryManForHimself>();
                float EMMaxActs = (CalcOpts.AllowFlooring ? (float)Math.Floor(EM.ability.Activates) : EM.ability.Activates) - EM.allNumActivates;
                float EMOldActs = EM.allNumActivates;
                TimesFeared = 0f;
#if RAWR3 || SILVERLIGHT
                foreach (Impedance f in BossOpts.Fears)
#else
                foreach (Impedance f in CalcOpts.Fears)
#endif
                {
                    ChanceYouAreFeared = f.Chance;
                    BaseFearDur = Math.Max(0f, (f.Duration / 1000f * (1f - StatS.FearDurReduc)));
                    fearActs = CalcOpts.AllowFlooring ? (float)Math.Ceiling(FightDuration / f.Frequency) : FightDuration / f.Frequency;
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
                        else if (Char.Race == CharacterRace.Human && EMMaxActs - EM.allNumActivates > 0f)
                        {
                            MaxTimeRegain = Math.Max(0f, (BaseFearDur - LatentGCD - CalcOpts.React / 1000f));

                            float availEMacts = (CalcOpts.AllowFlooring ? (float)Math.Floor(EM.ability.Activates) : EM.ability.Activates) - EM.allNumActivates;
                            float EMNewActs = Math.Min(fearActs, availEMacts);
                            EM.numActivates += EMNewActs;
                            _emActs = EM.allNumActivates;

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
                    GCDUsage += (TimesFeared > 0 ? TimesFeared.ToString(CalcOpts.AllowFlooring ? "000" : "000.00") + " x " + BaseFearDur.ToString("0.00") + "secs : Lost to Fears\n" : "");
                    GCDUsage += (BZ.allNumActivates > 0 ? BZ.allNumActivates.ToString(CalcOpts.AllowFlooring ? "000" : "000.00") + " x " + (BaseFearDur - reducedDur).ToString("0.00") + "secs : - " + BZ.ability.Name + "\n" : "");
                    GCDUsage += (EM.allNumActivates - EMOldActs > 0 ? (EM.allNumActivates - EMOldActs).ToString(CalcOpts.AllowFlooring ? "000" : "000.00") + " x " + (BaseFearDur - reducedDur).ToString("0.00") + "secs : - " + EM.ability.Name + "\n" : "");
                }
                TimeLostGCDs += Math.Min(NumGCDs, (BaseFearDur * TimesFeared) / LatentGCD);
                TimeLostGCDs -= Math.Min(TimeLostGCDs, (reducedDur * BZ.allNumActivates) / LatentGCD);
                TimeLostGCDs -= Math.Min(TimeLostGCDs, (reducedDur * _emActs) / LatentGCD);

                timelostwhilefeared = CalcOpts.AllowFlooring ? (float)Math.Ceiling(timelostwhilefeared) : timelostwhilefeared;
                percTimeInFear = timelostwhilefeared / FightDuration;
            }
            
            return percTimeInFear;
        }

        private float CalculateMovement(Ability MS)
        {
            float percTimeInMovement = 0f;
#if RAWR3 || SILVERLIGHT
            if (BossOpts.MovingTargs && BossOpts.Moves.Count > 0)
#else
            if (CalcOpts.MovingTargets && CalcOpts.Moves.Count > 0)
#endif
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
                 */
                AbilWrapper HF = GetWrapper<HeroicFury>();
                AbilWrapper CH;
                if (CombatFactors.FuryStance) CH = GetWrapper<Intercept>();
                else CH = GetWrapper<Charge>();
                
                float MovementSpeed = 7f * (1f + StatS.MovementSpeed); // 7 yards per sec * 1.08 (if have bonus) = 7.56
                float BaseMoveDur = 0f, movedActs = 0f, reducedDur = 0f,
                      MinMovementTimeRegain = 0f, MaxMovementTimeRegain = 0f,
                      ChanceYouHaveToMove = 1f;
                float ChargeMaxActs = CalcOpts.AllowFlooring ? (float)Math.Floor(CH.ability.Activates) : CH.ability.Activates;
                if (CombatFactors.FuryStance && HF.ability.Validated)
                {
                    ChargeMaxActs += HF.ability.Activates - HF.allNumActivates;
                }
                float ChargeActualActs = 0f;
                float timelostwhilemoving = 0f;
                float moveGCDs = 0f;
#if RAWR3 || SILVERLIGHT
                foreach (Impedance m in BossOpts.Moves)
#else
                foreach (Impedance m in CalcOpts.Moves)
#endif
                {
                    ChanceYouHaveToMove = m.Chance;
                    BaseMoveDur = (m.Duration / 1000f * (1f - StatS.MovementSpeed));
                    moveGCDs += movedActs = CalcOpts.AllowFlooring ? (float)Math.Ceiling(FightDuration / m.Frequency) : FightDuration / m.Frequency;

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
                
                TimeLostGCDs += Math.Min(NumGCDs, (BaseMoveDur * moveGCDs) / LatentGCD);
                TimeLostGCDs -= Math.Min(TimeLostGCDs, (reducedDur * actsCharge) / LatentGCD);
                CH.numActivates = actsCharge;
                float actsHF = 0f;
                if (CH.allNumActivates > CH.ability.Activates) actsHF += (CH.allNumActivates - CH.ability.Activates);
                HF.numActivates += actsHF;
                if (_needDisplayCalcs)
                {
                    GCDUsage += (moveGCDs > 0 ? moveGCDs.ToString(CalcOpts.AllowFlooring ? "000" : "000.00") + " x " + BaseMoveDur.ToString("0.00") + "secs : Lost to Movement\n" : "");
                    GCDUsage += (actsCharge > 0 ? actsCharge.ToString(CalcOpts.AllowFlooring ? "000" : "000.00") + " x " + (BaseMoveDur - reducedDur).ToString("0.00") + "secs : - " + CH.ability.Name + "\n" : "");
                    GCDUsage += (actsHF > 0 ? actsHF.ToString(CalcOpts.AllowFlooring ? "000" : "000.00") + " activates of " + HF.ability.Name + " to refresh " + CH.ability.Name : "");
                }
                //RageGainedWhileMoving += CH.Rage;
                // Need to add the special effect from Juggernaut to Mortal Strike, not caring about Slam right now
                if (Talents.Juggernaut > 0 && MS != null && CH.ability is Charge)
                {
                    Stats stats = new Stats
                    {
                        BonusWarrior_T8_4P_MSBTCritIncrease = 0.25f *
                            (_SE_ChargeUse[Talents.Juggernaut][Talents.GlyphOfRapidCharge?1:0]).GetAverageUptime(FightDuration / actsCharge, 1f, CombatFactors._c_mhItemSpeed, FightDuration)
                    };
                    stats.Accumulate(StatS);
                    // I'm not sure if this is gonna work, but hell, who knows
                    MS.BonusCritChance = stats.BonusWarrior_T8_4P_MSBTCritIncrease;
                    //MS = new Skills.MortalStrike(Char, stats, CombatFactors, WhiteAtks, CalcOpts);
                }
                timelostwhilemoving = (CalcOpts.AllowFlooring ? (float)Math.Ceiling(timelostwhilemoving) : timelostwhilemoving);
                percTimeInMovement = timelostwhilemoving / FightDuration;
            }
            return percTimeInMovement;
        }
        #endregion

        #region Cached Special Effects
        #region Berserker Rage
        private static readonly SpecialEffect _SE_ZERKERDUMMY = new SpecialEffect(Trigger.Use,
            new Stats() { BonusAgilityMultiplier = 1f }, // this is just so we can use a Perc Mod without having to make a new stat
            10f, 30f);
        #endregion
        #region Charge
        /// <summary>
        /// This is a 2D array because both the Juggernaught talent and the Glyph of Rapid Charge affect this thing
        /// <para>No Jug,No Glyph | No Jug,Glyph</para>
        /// <para>   Jug,No Glyph |    Jug,Glyph</para>
        /// </summary>
        private static readonly SpecialEffect[][] _SE_ChargeUse = {
            new SpecialEffect[] { new SpecialEffect(Trigger.Use, null, 10, ((15f + 0 * 5f) * (1f - (false ? 0.07f : 0f)))), new SpecialEffect(Trigger.Use, null, 10, ((15f + 0 * 5f) * (1f - (true ? 0.07f : 0f)))) },
            new SpecialEffect[] { new SpecialEffect(Trigger.Use, null, 10, ((15f + 1 * 5f) * (1f - (false ? 0.07f : 0f)))), new SpecialEffect(Trigger.Use, null, 10, ((15f + 1 * 5f) * (1f - (true ? 0.07f : 0f)))) },
        };
        #endregion
        #region Battle Shout
        /// <summary>
        /// 3d Array, Commanding Presence 0-5, Glyph of Battle 0-1, Booming Voice 0-2
        /// </summary>
        private static readonly SpecialEffect[/*commPresence:0-5*/][/*Glyph:0-1*/][/*boomVoice:0-2*/] _SE_BattleShout = {
            new SpecialEffect[][] {
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 0 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 0 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 0 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 0 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 0 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 0 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
            },
            new SpecialEffect[][] {
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 1 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 1 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 1 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 1 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 1 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 1 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
            },
            new SpecialEffect[][] {
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 2 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 2 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 2 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 2 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 2 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 2 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
            },
            new SpecialEffect[][] {
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 3 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 3 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 3 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 3 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 3 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 3 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
            },
            new SpecialEffect[][] {
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 4 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 4 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 4 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 4 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 4 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 4 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
            },
            new SpecialEffect[][] {
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 5 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 5 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 5 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 5 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 5 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { AttackPower = (548f * (1f + 5 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
            },
        };
        #endregion
        #region Commanding Shout
        /// <summary>
        /// 3d Array, Commanding Presence 0-5, Glyph of Command 0-1, Booming Voice 0-2
        /// </summary>
        private static readonly SpecialEffect[/*commPresence:0-5*/][/*Glyph:0-1*/][/*boomVoice:0-2*/] _SE_CommandingShout = {
            new SpecialEffect[][] {
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 0 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 0 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 0 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 0 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 0 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 0 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
            },
            new SpecialEffect[][] {
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 1 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 1 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 1 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 1 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 1 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 1 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
            },
            new SpecialEffect[][] {
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 2 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 2 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 2 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 2 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 2 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 2 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
            },
            new SpecialEffect[][] {
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 3 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 3 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 3 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 3 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 3 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 3 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
            },
            new SpecialEffect[][] {
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 4 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 4 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 4 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 4 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 4 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 4 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
            },
            new SpecialEffect[][] {
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 5 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 5 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 5 * 0.05f)), }, ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (false ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
                new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 5 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 0 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 5 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 1 * 0.25f))),
                                      new SpecialEffect(Trigger.Use, new Stats() { Health = (2255f * (1f + 5 * 0.05f)), }, ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f)), ((2f + (true  ? 2f : 0f)) * 60f * (1f + 2 * 0.25f))) },
            },
        };
        #endregion
        #region Demoralizing Shout
        private static readonly SpecialEffect[][] _SE_DemoralizingShout = {
            new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 0 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 0), 30f * (1f + 0.25f * 0)), new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 0 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 1), 30f * (1f + 0.25f * 1)), new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 0 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 2), 30f * (1f + 0.25f * 2)) },
            new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 1 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 0), 30f * (1f + 0.25f * 0)), new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 1 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 1), 30f * (1f + 0.25f * 1)), new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 0 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 2), 30f * (1f + 0.25f * 2)) },
            new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 2 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 0), 30f * (1f + 0.25f * 0)), new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 2 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 1), 30f * (1f + 0.25f * 1)), new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 0 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 2), 30f * (1f + 0.25f * 2)) },
            new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 3 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 0), 30f * (1f + 0.25f * 0)), new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 3 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 1), 30f * (1f + 0.25f * 1)), new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 0 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 2), 30f * (1f + 0.25f * 2)) },
            new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 4 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 0), 30f * (1f + 0.25f * 0)), new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 4 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 1), 30f * (1f + 0.25f * 1)), new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 0 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 2), 30f * (1f + 0.25f * 2)) },
            new SpecialEffect[] { new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 5 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 0), 30f * (1f + 0.25f * 0)), new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 5 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 1), 30f * (1f + 0.25f * 1)), new SpecialEffect(Trigger.Use, new Stats() { BossAttackPower = 410f * (1f + 0 * 0.08f) * -1f, }, 30f * (1f + 0.25f * 2), 30f * (1f + 0.25f * 2)) },
        };
        #endregion
        #region Recklessness, Shattering Throw, ThunderClap, Sunder Armor, Sweeping Strikes
        private static Dictionary<float, SpecialEffect> _SE_Recklessness = new Dictionary<float, SpecialEffect>();
        private static Dictionary<float, SpecialEffect> _SE_ShatteringThrow = new Dictionary<float, SpecialEffect>();
        private static Dictionary<float, SpecialEffect[]> _SE_ThunderClap = new Dictionary<float, SpecialEffect[]>();
        private static Dictionary<float, SpecialEffect> _SE_SunderArmor = new Dictionary<float, SpecialEffect>();
        private static Dictionary<float, SpecialEffect> _SE_SweepingStrikes = new Dictionary<float, SpecialEffect>();
        #endregion
        #endregion

        public void AddValidatedSpecialEffects(Stats statsTotal, WarriorTalents talents)
        {
            Ability ST = GetWrapper<ShatteringThrow>().ability,
                    BTS = GetWrapper<BattleShout>().ability,
                    CS = GetWrapper<CommandingShout>().ability,
                    DS = GetWrapper<DemoralizingShout>().ability,
                    TH = GetWrapper<ThunderClap>().ability,
                    SN = GetWrapper<SunderArmor>().ability,
                    SW = GetWrapper<SweepingStrikes>().ability,
                    RK = GetWrapper<Recklessness>().ability;
            if (BTS.Validated) { statsTotal.AddSpecialEffect(_SE_BattleShout[talents.CommandingPresence][talents.GlyphOfBattle ? 0 : 1][talents.BoomingVoice]); }
            if (CS.Validated) { statsTotal.AddSpecialEffect(_SE_CommandingShout[talents.CommandingPresence][talents.GlyphOfCommand ? 0 : 1][talents.BoomingVoice]); }
            if (DS.Validated) { statsTotal.AddSpecialEffect(_SE_DemoralizingShout[talents.ImprovedDemoralizingShout][talents.BoomingVoice]); }
            if (ST.Validated) {
                float value = (float)Math.Round(ST.MHAtkTable.AnyLand, 3);
                if (!_SE_ShatteringThrow.ContainsKey(value)) {
                    _SE_ShatteringThrow.Add(value, new SpecialEffect(Trigger.Use, new Stats() { ArmorPenetration = 0.20f, }, ST.Duration, ST.Cd, ST.MHAtkTable.AnyLand));
                }
                statsTotal.AddSpecialEffect(_SE_ShatteringThrow[value]);
            }
            if (TH.Validated) {
                float value = (float)Math.Round(TH.MHAtkTable.AnyLand, 3);
                if (!_SE_ThunderClap.ContainsKey(value)) {
                    _SE_ThunderClap.Add(value, new SpecialEffect[] {
                        new SpecialEffect(Trigger.Use, new Stats() { BossAttackSpeedMultiplier = (-0.10f * (1f + 0 / 30f)), }, TH.Duration, TH.Cd, TH.MHAtkTable.AnyLand),
                        new SpecialEffect(Trigger.Use, new Stats() { BossAttackSpeedMultiplier = (-0.10f * (1f + 1 / 30f)), }, TH.Duration, TH.Cd, TH.MHAtkTable.AnyLand),
                        new SpecialEffect(Trigger.Use, new Stats() { BossAttackSpeedMultiplier = (-0.10f * (1f + 2 / 30f)), }, TH.Duration, TH.Cd, TH.MHAtkTable.AnyLand),
                        new SpecialEffect(Trigger.Use, new Stats() { BossAttackSpeedMultiplier = (-0.10f * (1f + 3 / 30f)), }, TH.Duration, TH.Cd, TH.MHAtkTable.AnyLand),
                    });
                }
                statsTotal.AddSpecialEffect(_SE_ThunderClap[value][talents.ImprovedThunderClap]);
            }
            if (SN.Validated) {
                float value = (float)Math.Round(SN.MHAtkTable.AnyLand, 3);
                if (!_SE_SunderArmor.ContainsKey(value)) {
                    _SE_SunderArmor.Add(value, new SpecialEffect(Trigger.Use, new Stats() { ArmorPenetration = 0.04f, }, SN.Duration, SN.Cd, SN.MHAtkTable.AnyLand, 5));
                }
                statsTotal.AddSpecialEffect(_SE_SunderArmor[value]);
            }
            float landedAtksInterval = LandedAtksOverDur / FightDuration;
            float critRate = CriticalAtksOverDur / AttemptedAtksOverDur;
            if (SW.Validated) {
                float interval = (float)Math.Round(landedAtksInterval * 5f, 3);
                if (!_SE_SweepingStrikes.ContainsKey(interval)) {
                    _SE_SweepingStrikes.Add(interval, new SpecialEffect(Trigger.Use, new Stats() { BonusTargets = 1f, }, landedAtksInterval * 5f, SW.Cd));
                }
                statsTotal.AddSpecialEffect(_SE_SweepingStrikes[interval]);
            }
            if (RK.Validated && CombatFactors.FuryStance) {
                float interval = (float)Math.Round(landedAtksInterval * 3f, 3);
                if (!_SE_Recklessness.ContainsKey(interval)) {
                    _SE_Recklessness.Add(interval, new SpecialEffect(Trigger.Use, new Stats() { PhysicalCrit = 1f /*- critRate*/ }, landedAtksInterval * 3f, RK.Cd));
                }
                statsTotal.AddSpecialEffect(_SE_Recklessness[interval]);
            }
            /*if (talents.Flurry > 0 && CalcOpts.FuryStance)
            { // NOTE!!!! This comment is still using the old method, we need to cache values like you see above
                //float value = talents.Flurry * 0.05f;
                SpecialEffect flurry = new SpecialEffect(Trigger.MeleeCrit,
                    new Stats() { PhysicalHaste = talents.Flurry * 0.05f, }, landedAtksInterval * 3f, 0f);
                statsTotal.AddSpecialEffect(flurry);
            }*/
        }

        internal void ResetHitTables()
        {
            foreach (AbilWrapper aw in GetAbilityList())
            {
                if (aw.ability.CanCrit)
                {
                    aw.ability.MHAtkTable.Reset();
                    aw.ability.OHAtkTable.Reset();
                }
            }
        }
    }
}
