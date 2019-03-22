/**********
 * Owner: Ebs
 **********/
using System;
using System.Collections.Generic;
using System.Text;
using Rawr.DPSWarr.Skills;
using Rawr.Base;

namespace Rawr.DPSWarr
{
    public class FuryRotation : Rotation
    {
        public FuryRotation(Character character, Stats stats, CombatFactors cf, Skills.WhiteAttacks wa, CalculationOptionsDPSWarr co, BossOptions bo) {
            Char = character;
            StatS = stats;
            Talents = Char == null || Char.WarriorTalents == null ? new WarriorTalents() : Char.WarriorTalents;
            //TalentsCata = Char == null || Char.WarriorTalentsCata == null ? new WarriorTalentsCata() : Char.WarriorTalentsCata;
            CombatFactors = cf;
            CalcOpts = (co == null ? new CalculationOptionsDPSWarr() : co);
            BossOpts = (bo == null ? new BossOptions() : bo);
            WhiteAtks = wa;

            _cachedLatentGCD = 1.5f + CalcOpts.Latency + CalcOpts.AllowedReact;
            _cachedNumGCDs = CalcOpts.AllowFlooring ? (float)Math.Floor(FightDuration / LatentGCD) : FightDuration / LatentGCD;
            // Initialize();
        }

        #region FuryRotVariables
        
        private const float ROTATION_LENGTH = 8.0f;
        private const float FREE_GCDS = 1.0f;
        
        //float _bloodsurgeRPS;
        private float percHS, percCL;
        private float timeLostPerc = 0f;
        #endregion

        public override void Initialize()
        {
            initAbilities();
            // doIterations();
            bool hsok = CalcOpts.Maintenance[(int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.HeroicStrike_];
            bool clok = 
#if RAWR3 || SILVERLIGHT
                BossOpts.MultiTargs && BossOpts.Targets != null && BossOpts.Targets.Count > 0
#else
                CalcOpts.MultipleTargets
#endif
                && CalcOpts.Maintenance[(int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.Cleave_];

            percHS = (hsok ? 1f : 0f);
            if (clok)
            {
#if RAWR3 || SILVERLIGHT
                //percHS -= (float)BossOpts.MultiTargsPerc;
                {
                    float time = 0;
                    foreach(TargetGroup tg in BossOpts.Targets) {
                        if (tg.Chance <= 0 || tg.Frequency <= 0 || tg.Duration <= 0) continue;
                        time += tg.Frequency / BossOpts.BerserkTimer * tg.Duration / 1000f;
                    }
                    float perc = time / BossOpts.BerserkTimer;
                    percHS -= Math.Max(0f, Math.Min(1f, perc)); 
                }
#else
                percHS -= CalcOpts.MultipleTargetsPerc / 100f;
#endif
            }
            percCL = (clok ? 1f - percHS : 0f);
        }

        public override void Initialize(CharacterCalculationsDPSWarr calcs)
        {
            base.Initialize(calcs);
            
            bool hsok = CalcOpts.Maintenance[(int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.HeroicStrike_];
            bool clok =
#if RAWR3 || SILVERLIGHT
                BossOpts.MultiTargs && BossOpts.Targets != null && BossOpts.Targets.Count > 0
#else
                CalcOpts.MultipleTargets
#endif
                && CalcOpts.Maintenance[(int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.Cleave_];
            BloodSurge _BS = GetWrapper<BloodSurge>().ability as BloodSurge;
            percHS = (hsok ? 1f : 0f);
            if (clok)
            {
#if RAWR3 || SILVERLIGHT
                //percHS -= (float)BossOpts.MultiTargsPerc;
                {
                    float time = 0;
                    foreach (TargetGroup tg in BossOpts.Targets)
                    {
                        if (tg.Chance <= 0 || tg.Frequency <= 0 || tg.Duration <= 0) continue;
                        time += tg.Frequency / BossOpts.BerserkTimer * tg.Duration / 1000f;
                    }
                    float perc = time / BossOpts.BerserkTimer;
                    percHS -= Math.Max(0f, Math.Min(1f, perc));
                }
#else
                percHS -= (float)CalcOpts.MultipleTargetsPerc / 100f;
#endif
            }
            percCL = (clok ? 1f - percHS : 0f);
            if (_BS != null) _BS.maintainActs = MaintainCDs;
        }

        protected void new_doIterations()
        {
            base.doIterations();
        /*    AbilWrapper HS = GetWrapper<HeroicStrike>();
            OnAttack _HS = HS.ability as OnAttack;
            HS.numActivates = 0f;
            WhiteAtks.HSOverridesOverDur = 0f;
            WhiteAtks.CLOverridesOverDur = 0f;

            //float bsBaseRage = BS.RageUseOverDur;
            float hsRageUsed = (FreeRageOverDur - BS.RageUseOverDur) / (1f + _HS.FullRageCost * (Talents.Bloodsurge * 0.20f / 3f));
          */  
        }
        protected override void calcDeepWounds()
        {
            // Main Hand
            float mhActivates =
                /*Yellow  */CriticalYellowsOverDurMH +
                /*White   */WhiteAtks.MhActivates * (1f - timeLostPerc) * WhiteAtks.MHAtkTable.Crit;

            // Off Hand
            float ohActivates = (CombatFactors.useOH ?
                // No OnAttacks for OH
                /*Yellow*/CriticalYellowsOverDurOH +
                /*White */WhiteAtks.OhActivates * (1f - timeLostPerc) * WhiteAtks.OHAtkTable.Crit
                : 0f);

            // Push to the Ability
            DW.SetAllAbilityActivates(mhActivates, ohActivates);
        }
        public override void doIterations() {
            try
            {
                base.doIterations();
                // Fury Iteration
                BloodSurge BS = GetWrapper<BloodSurge>().ability as BloodSurge;
                AbilWrapper HS = GetWrapper<HeroicStrike>();
                AbilWrapper CL = GetWrapper<Cleave>();
                OnAttack _HS = HS.ability as OnAttack;
                OnAttack _CL = CL.ability as OnAttack;

                float ovdRage, hsRageUsed, clRageUsed;
                float oldBS, oldHS, oldCL;
                do
                {
                    oldBS = BS.Activates;
                    oldHS = HS.allNumActivates;
                    oldCL = CL.allNumActivates;

                    ovdRage = FreeRageOverDur;
                    hsRageUsed = ovdRage * percHS;
                    clRageUsed = ovdRage * percCL;
                    WhiteAtks.HSOverridesOverDur = HS.numActivates = Math.Min(hsRageUsed / _HS.FullRageCost, WhiteAtks.MhActivatesNoHS);
                    WhiteAtks.CLOverridesOverDur = CL.numActivates = Math.Min(clRageUsed / _CL.FullRageCost, WhiteAtks.MhActivatesNoHS - WhiteAtks.HSOverridesOverDur);
                    BS.hsActivates = HS.allNumActivates;
                } while (Math.Abs(1f - (BS.Activates        != 0 ? oldBS / BS.Activates        : 1f)) > 0.005f ||
                         Math.Abs(1f - (HS.allNumActivates <= 0 ? oldHS / HS.allNumActivates : 1f)) > 0.005f ||
                         Math.Abs(1f - (CL.allNumActivates <= 0 ? oldCL / CL.allNumActivates : 1f)) > 0.005f);

            }
            catch (Exception ex)
            {
                ErrorBox eb = new ErrorBox("Error in performing Fury Iterations",
                    ex.Message, "doIterations()", "No Additional Info", ex.StackTrace);
                eb.Show();
            }
        }

        #region NewRotation
        /*private float bloodsurge_percUsed;  // Since we can only bloodsurge once every
                                            // 8secs, this keeps track of how many times we
                                            // can actually slam vs refresh an ability
        private float rotationSlipTime = 0f;// for when maint abilities fail
            
        private float gcdCounter;                  // where we are in the fight
        
        // maintenance
        private float numSunderGCDs = 0f, numThunderGCDs = 0f, numDemoShoutGCDs = 0f,
                      numBattleShoutGCDs = 0f, numCommandingShoutGCDs = 0f;
        
        // cooldowns on the GCD
        private float numDeathwishGCDs = 0f, numBerserkerRageGCDs = 0f;
        
        // dps
        private float numBloodthirstGCDs = 0f, numWhirlwindGCDs = 0f, numBloodsurgeGCDs = 0f;

        
        private void Preprocess(Skills.Ability ability) { Preprocess(ability, 1f); }
        /// <summary>
        /// Preprocesses the rotation variables for each maintenance ability
        /// </summary>
        /// <param name="ability">An ability that may be maintained</param>
        /// <param name="stacks">The number of stacks applied at the START of the fight (ex: Battle Shout would be 0, Sunder Armor would be 5, Demo Shout would be 1)</param>
        private void Preprocess(Skills.Ability ability, float stacks)
        {
            if (ability.Validated)
            {
                float slip = 1f / DS.MHAtkTable.AnyLand; // initial application
                float reapplyPeriod = (int)(DS.Duration / ROTATION_LENGTH);
                
                bloodsurge_percUsed -= 1f / reapplyPeriod; // keep it up
                rotationSlipTime += (1f - slip);// if I on average have to do
                                                // 1.08 demos to apply it, then
                                                // 8% of my rotations have a 1.5sec slip
                numDemoShoutGCDs += stacks * slip;
                gcdCounter += stacks * slip;
            }
        }

        public void new_MakeRotationandDoDPS(bool setCalcs)
        {
            float LatentGCD = 1.5f + CalcOpts.GetLatency();
            float NumGCDs = CalcOpts.Duration / LatentGCD;
            GCDUsage += "NumGCDs: " + NumGCDs.ToString() + "\n\n";
            
            // Maintenance abilities

            // First, apply initial debuffs
            bloodsurge_percUsed = 1f; // Since we can only bloodsurge once every 8secs, 
                                            // this keeps track of how many times we can actually slam vs refresh an ability
            
            // maintenance
            numSunderGCDs = 0f; numThunderGCDs = 0f; numDemoShoutGCDs = 0f;
            numBattleShoutGCDs = 0f; numCommandingShoutGCDs = 0f;
            // cooldowns on the GCD
            numDeathwishGCDs = 0f; numBerserkerRageGCDs = 0f;
            // dps
            numBloodthirstGCDs = 0f; numWhirlwindGCDs = 0f; numBloodsurgeGCDs = 0f;

            gcdCounter = 0f; // where we are in the fight

            //////////////////////////////////////////////////////////////////////////
            // The following code block does two things in one pass:                //
            // 1) Determines how long it takes to apply initial debuffs (stored in  //
            //         gcdCounter)                                                  //
            // 2) Determines how frequently we actually get to bloodsurge based on  //
            //         free GCDs (stored in bloodsurge_percUsed)                    //
            //////////////////////////////////////////////////////////////////////////
            Preprocess(SN, 5f);
            Preprocess(TH);
            Preprocess(DS);
            // Assuming these are already applied at the start of the fight
            Preprocess(BTS, 0f);
            Preprocess(CS, 0f);
            
            if (bloodsurge_percUsed < 0f)
                bloodsurge_percUsed = Math.Max(bloodsurge_percUsed, 0f);

        }
         */
        #endregion
        public override void MakeRotationandDoDPS(bool setCalcs, bool needsDisplayCalcs)
        {
            base.MakeRotationandDoDPS(setCalcs, needsDisplayCalcs);
            //new_MakeRotationandDoDPS(setCalcs);
            // Starting Numbers
            float DPS_TTL = 0f, HPS_TTL = 0f;
            float LatentGCD = 1.5f + CalcOpts.Latency;
            if (_needDisplayCalcs) GCDUsage += "NumGCDs: " + NumGCDs.ToString() + "\n\n";
            float GCDsused = 0f;
            float availGCDs = Math.Max(0f, NumGCDs - GCDsused);
            float availRage = 0f;
            float rageadd = 0f;
            //float timelostwhilestunned = 0f;
            //float percTimeInStun = 0f;

            if (Char.MainHand == null) { return; }
            //doIterations();

            // ==== Rage Generation Priorities ========
            availRage += RageGenOverDur_Other;
            
            /*Bloodrage         */
            //AddAnItem(ref availRage, percTimeInStun, ref _Blood_GCDs, ref HPS_TTL, ref _Blood_HPS, BR);
            /*Berserker Rage    */
            
            /*float Reck_GCDs = Math.Min(availGCDs, RK.Activates);
            _Reck_GCDs = Reck_GCDs;
            GCDsused += Math.Min(NumGCDs, Reck_GCDs);
            GCDUsage += RK.Name + ": " + Reck_GCDs.ToString() + "\n";
            availGCDs = Math.Max(0f, NumGCDs - GCDsused);
            rageadd = RK.GetRageUsePerSecond(Reck_GCDs);
            availRage -= rageadd;
            RageNeeded += rageadd;*/

            //doIterations();

            #region CalcOpts GCD Losses
            #region Stuns
            /*if (CalcOpts.StunningTargets && CalcOpts.Stuns.Count > 0f) 
            {
                float numStuns = FightDuration / CalcOpts.StunningTargetsFreq;
                if (!CalcOpts.SE_UseDur) numStuns = (float)Math.Floor(numStuns);
                // ways to counter, no GCD, just use reaction time
                if (Talents.HeroicFury > 0f)
                {
                    float numUses = Math.Min(numStuns, GetWrapper<HeroicFury>().ability.Activates);
                    numStuns -= numUses;
                    timeLost += numUses * (CalcOpts.React / 1000f + CalcOpts.Latency); // not using GetReact because it's not on the GCD
                    GetWrapper<HeroicFury>().numActivates += numUses;
                }
                if (Char.Race == CharacterRace.Human)
                {
                    float numUses = Math.Min(numStuns, GetWrapper<EveryManForHimself>().ability.Activates);
                    numStuns -= numUses;
                    timeLost += numUses * CalcOpts.React / 1000f + CalcOpts.Latency; // not using GetReact because it's not on the GCD
                    GetWrapper<EveryManForHimself>().numActivates += numUses;
                }

                if (numStuns > 0f)
                {
                    float stunDur = CalcOpts.StunningTargetsDur / 1000f * (1f - Talents.IronWill * 2f / 30f);
                    timeLost += stunDur * numStuns;
                }
            }*/
            #endregion
            float otherTimeLost = CalculateTimeLost(null);
            DoMaintenanceActivates(otherTimeLost);

            float timeLostPerc = otherTimeLost;
            #endregion

            AbilWrapper WW = GetWrapper<WhirlWind>();
            AbilWrapper BT = GetWrapper<BloodThirst>();
            AbilWrapper BS = GetWrapper<BloodSurge>();
            // Priority 1 : Whirlwind on every CD
            float WW_GCDs = Math.Min(availGCDs, WW.ability.Activates) * (1f - timeLostPerc);
            WW.numActivates = WW_GCDs;
            GCDsused += Math.Min(NumGCDs, WW_GCDs);
            if (_needDisplayCalcs) GCDUsage += WW.ability.Name + ": " + WW_GCDs.ToString() + "\n";
            availGCDs = Math.Max(0f, NumGCDs - GCDsused);
            DPS_TTL += WW.DPS;
            HPS_TTL += WW.HPS;
            rageadd = WW.allRage;
            availRage -= rageadd;
            
            // Priority 2 : Bloodthirst on every CD
            float BT_GCDs = Math.Min(availGCDs, BT.ability.Activates) * (1f - timeLostPerc);
            BT.numActivates = BT_GCDs;
            GCDsused += Math.Min(NumGCDs, BT_GCDs);
            if (_needDisplayCalcs) GCDUsage += BT.ability.Name + ": " + BT_GCDs.ToString() + "\n";
            availGCDs = Math.Max(0f, NumGCDs - GCDsused);
            DPS_TTL += BT.DPS;
            HPS_TTL += BT.HPS;
            rageadd = BT.allRage;
            availRage -= rageadd;
            
            doIterations();
            // Priority 3 : Bloodsurge Blood Proc (Do an Instant Slam) if available
            float BS_GCDs = Math.Min(availGCDs, BS.ability.Activates) * (1f - timeLostPerc);
            BS.numActivates = BS_GCDs;
            GCDsused += Math.Min(NumGCDs, BS_GCDs);
            if (_needDisplayCalcs) GCDUsage += BS.ability.Name + ": " + BS_GCDs.ToString() + "\n";
            availGCDs = Math.Max(0f, NumGCDs - GCDsused);
            DPS_TTL += BS.DPS;
            HPS_TTL += BS.HPS;
            rageadd = BS.allRage;
            availRage -= rageadd;

            InvalidateCache();
            //Sword Spec, Doesn't eat GCDs
            /*float SS_Acts = SS.GetActivates(GetLandedYellowsOverDur());
            _SS_Acts = SS_Acts;
            _SS_DPS = SS.GetDPS(SS_Acts);
            DPS_TTL += _SS_DPS;*/
            // TODO: Add Rage since it's a white hit

            //doIterations();
            // Priority 4 : Heroic Strike, when there is rage to do so, handled by the Heroic Strike class
            // Alternate to Cleave is MultiTargs is active
            // After iterating how many Overrides can be done and still do other abilities, then do the white dps

            availRage += WhiteAtks.MHRageGenOverDur + WhiteAtks.OHRageGenOverDur;

            bool HSok = CalcOpts.Maintenance[(int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.HeroicStrike_];
            bool CLok = 
#if RAWR3 || SILVERLIGHT
                BossOpts.MultiTargs && BossOpts.Targets != null && BossOpts.Targets.Count > 0
#else
                CalcOpts.MultipleTargets
#endif
                && CalcOpts.Maintenance[(int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.Cleave_];

            WhiteAtks.Slam_ActsOverDur = 0f;// _SL_GCDs;
 
                DPS_TTL += WhiteAtks.MhDPS * (1f - timeLostPerc) + WhiteAtks.OhDPS * (1f - timeLostPerc);
                DPS_TTL += GetWrapper<HeroicStrike>().DPS;
                DPS_TTL += GetWrapper<Cleave>().DPS;
 
            calcDeepWounds();
            DPS_TTL += DW.DPS;
            if (_needDisplayCalcs) GCDUsage += "\nAvail: " + availGCDs.ToString();

            // Return result
            _HPS_TTL = HPS_TTL;

            if (setCalcs)
            {
                this.calcs.TotalDPS = DPS_TTL;
                this.calcs.WhiteDPS = (WhiteAtks.MhDPS + WhiteAtks.OhDPS) * (1f - timeLostPerc);

                this.calcs.WhiteRage  = WhiteAtks.MHRageGenOverDur + WhiteAtks.OHRageGenOverDur;
                this.calcs.OtherRage  = RageGenOverDur_Other;
                this.calcs.NeedyRage  = RageNeededOverDur;
                this.calcs.FreeRage   = calcs.WhiteRage + calcs.OtherRage - calcs.NeedyRage;
                this.calcs.WhiteDPSMH = this.WhiteAtks.MhDPS;
                this.calcs.WhiteDPSOH = this.WhiteAtks.OhDPS;
                this.calcs.WhiteDmg   = this.WhiteAtks.MhDamageOnUse + this.WhiteAtks.OhDamageOnUse;
            }
        }

        protected float FreeRageOverDur
        {
            get
            {
                /*if (Char.MainHand == null) { return 0f; }
                float white = WHITEATTACKS.whiteRageGenOverDurNoHS * (1f - timeLostPerc);
                //float sword = SS.GetRageUseOverDur(_SS_Acts);
                float other = RageGenOverDur_Other;
                float needy = RageNeededOverDur;
                return white + other - needy;*/
                return WhiteAtks.whiteRageGenOverDurNoHS * (1f - timeLostPerc) +
                       RageGenOverDur_Other -
                       RageNeededOverDur;
            }
        }
    }
}