/**********
 * Owner: Jothay
 **********/
using System;
using System.Collections.Generic;
using System.Text;
using Rawr.DPSWarr.Skills;

namespace Rawr.DPSWarr {
    public class ArmsRotation : Rotation {
        public ArmsRotation(Character character, Stats stats, CombatFactors cf, Skills.WhiteAttacks wa, CalculationOptionsDPSWarr co, BossOptions bo) {
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
#if RAWR3 || SILVERLIGHT
            _cachedNumGCDsU20 = CalcOpts.AllowFlooring ? (float)Math.Floor(FightDuration / LatentGCD * (float)BossOpts.Under20Perc) : FightDuration / LatentGCD * (float)BossOpts.Under20Perc;
#else
            _cachedNumGCDsU20 = CalcOpts.AllowFlooring ? (float)Math.Floor(FightDuration / LatentGCD * CalcOpts.Under20Perc) : FightDuration / LatentGCD * CalcOpts.Under20Perc;
#endif
            // Initialize();
        }
        #region Variables
        // Ability Declarations
        public Skills.FakeWhite FW;
        #endregion
        #region Initialization
        public override void Initialize(CharacterCalculationsDPSWarr calcs) {
            base.Initialize(calcs);
            calcs.FW = FW;
        }
        /*protected override void initAbilities() {
            base.initAbilities();
            WW = new Skills.WhirlWind(Char, StatS, CombatFactors, WhiteAtks, CalcOpts);
            BLS = new Skills.Bladestorm(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, WW);
            MS = new Skills.MortalStrike(Char, StatS, CombatFactors, WhiteAtks, CalcOpts);
            RD = new Skills.Rend(Char, StatS, CombatFactors, WhiteAtks, CalcOpts);
            SS = new Skills.Swordspec(Char, StatS, CombatFactors, WhiteAtks, CalcOpts);
            OP = new Skills.OverPower(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, SS);
            TB = new Skills.TasteForBlood(Char, StatS, CombatFactors, WhiteAtks, CalcOpts);
            SD = new Skills.Suddendeath(Char, StatS, CombatFactors, WhiteAtks, CalcOpts, EX);
            FW = new Skills.FakeWhite(Char, StatS, CombatFactors, WhiteAtks, CalcOpts);
        }*/
        #endregion
        #region Rage Calcs
        /*protected override float RageNeededOverDur {
            get {
                float SweepingRage = SW.GetRageUseOverDur(_SW_GCDs);
                float BladestormRage = BLS.GetRageUseOverDur(_BLS_GCDs);
                float MSRage = MS.GetRageUseOverDur(_MS_GCDs);
                float RendRage = RD.GetRageUseOverDur(_RD_GCDs);
                float OPRage = OP.GetRageUseOverDur(_OP_GCDs);
                float TBRage = TB.GetRageUseOverDur(_TB_GCDs);
                float SDRage = SD.GetRageUseOverDur(_SD_GCDs);
                return base.RageNeededOverDur + SweepingRage + BladestormRage + MSRage + 
                    RendRage + OPRage + TBRage + SDRage;
            }
        }*/
        #endregion
        protected override void calcDeepWounds() {
            base.calcDeepWounds();
        }

        public float SettleAll(float totalPercTimeLost, float percTimeUnder20, float availRage, out float PercFailRage)
        {
            float percTimeOver20 = (1f - percTimeUnder20);
            float percTimeInDPS = (1f - totalPercTimeLost);
            float percTimeInDPSAndOver20 = percTimeOver20 * percTimeInDPS;
            availRage -= DoMaintenanceActivates(totalPercTimeLost) * percTimeOver20;
            //availGCDs = NumGCDs - GCDsused;
            /* The following are dependant on other attacks as they are proccing abilities or are the fallback item
             * We need to loop these until the activates are relatively unchanged
             * Heroic Strike, when there is rage to do so, handled by the Heroic Strike class
             * Alternate to Cleave is MultiTargs is active, but only to the perc of time where Targs is active
             * After iterating how many Overrides can be done and still do other abilities, then do the white dps
             *
             * Starting Assumptions:
             * No ability ever procs so Slam sucks up all the cooldowns (except under <20% with that active, where Exec sucks all of them)
             * Heroic Strike and Cleave won't be used at all
             * Sudden Death Free Rage is minimum cost, no extra rage available
             * Execute Free Rage is minimum cost, no extra rage available
             * 
             * Hoped Ending Results:
             * All abilities will have proc'd and abilities that can proc from other ones will have their activates settled
             * Heroic Strikes and Cleave will activate when there's enough rage to support them AND Executes
             * Sudden Death will get extra rage leftovers if there are any
             * Execute will get extra rage leftovers if there are any (since you won't use HS/CL <20%)
            */

            float preloopAvailGCDs = GCDsAvailable, preloopGCDsUsed = GCDsUsed, preloopAvailRage = availRage;

            float origNumGCDs = (CalcOpts.AllowFlooring ? (float)Math.Floor(FightDuration / LatentGCD) : FightDuration / LatentGCD) * percTimeOver20,
                  origavailGCDs = preloopAvailGCDs * percTimeOver20,
                  origGCDsused = preloopGCDsUsed * percTimeOver20;
            float oldBLSGCDs = 0f, oldMSGCDs = 0f,
                  oldRDGCDs = 0f,    oldOPGCDs = 0f,  oldTBGCDs = 0f,
                  oldSDGCDs = 0f,    oldEXGCDs = 0f,  oldSLGCDs = 0f,
                  oldSSActs = 0f;

            AbilWrapper SL = GetWrapper<Slam>();
            AbilWrapper SD = GetWrapper<Suddendeath>();
            AbilWrapper EX = GetWrapper<Execute>();
            AbilWrapper HS = GetWrapper<HeroicStrike>();
            AbilWrapper CL = GetWrapper<Cleave>();
            AbilWrapper MS = GetWrapper<MortalStrike>();
            AbilWrapper OP = GetWrapper<OverPower>();
            AbilWrapper BLS = GetWrapper<Bladestorm>();
            AbilWrapper RD = GetWrapper<Rend>();
            AbilWrapper TB = GetWrapper<TasteForBlood>();
            AbilWrapper SS = GetWrapper<Swordspec>();

            Execute EX_ability = EX.ability as Execute;
            Suddendeath SD_ability = SD.ability as Suddendeath;
            
            SL.numActivates = origavailGCDs;
            WhiteAtks.Slam_ActsOverDur = SL.numActivates;
            SD_ability.FreeRage = SD_ability.RageCost;
            EX_ability.FreeRage = EX_ability.RageCost;
            float oldHSActivates = 0f, RageForHS = 0f, newHSActivates = HS.numActivates = WhiteAtks.HSOverridesOverDur = 0f;
            float oldCLActivates = 0f, RageForCL = 0f, newCLActivates = CL.numActivates = WhiteAtks.CLOverridesOverDur = 0f;
            float origAvailRage = preloopAvailRage * percTimeOver20;
            bool hsok = CalcOpts.Maintenance[(int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.HeroicStrike_];
            bool clok =
#if RAWR3 || SILVERLIGHT
                BossOpts.MultiTargs && BossOpts.Targets != null && BossOpts.Targets.Count > 0
#else
                CalcOpts.MultipleTargets && CalcOpts.MultipleTargetsPerc > 0
#endif
                && CalcOpts.Maintenance[(int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.Cleave_];
            availRage += WhiteAtks.whiteRageGenOverDurNoHS * percTimeInDPS * percTimeOver20;
            availRage -= SL.Rage;
            float repassAvailRage = 0f;
            PercFailRage = 1f;

            int Iterator = 0;
            #region >20%
            // Run the loop for >20%
            float MSBaseCd = 6f - Talents.ImprovedMortalStrike / 3f;
            float MS_WeightedValue = MS.ability.DamageOnUse + DW.TickSize * MS.ability.MHAtkTable.Crit,
                  SD_WeightedValue = SD.ability.DamageOnUse + DW.TickSize * SD.ability.MHAtkTable.Crit,
                  SL_WeightedValue = SL.ability.DamageOnUse + DW.TickSize * SL.ability.MHAtkTable.Crit;
            float OnePt5Plus1 = LatentGCD + (OP.ability.Cd + CalcOpts.AllowedReact),
                  Two1pt5 = LatentGCD * 2f,
                  Two1pt0 = (OP.ability.Cd + CalcOpts.AllowedReact) * 2f;
            float TasteForBloodMOD = (Talents.TasteForBlood == 3 ? 1f / 6f : (Talents.TasteForBlood == 2 ? 0.144209288653733f : (Talents.TasteForBlood == 1 ? 0.104925207394343f : 0)));
            float OtherMOD = (MSBaseCd + CalcOpts.Latency);
            float SDMOD = 1f - 0.03f * Talents.SuddenDeath;
            float avoid = (1f - CombatFactors._c_mhdodge - CombatFactors._c_ymiss);
            float atleast1=0f, atleast2=0f, atleast3=0f, extLength1, extLength2, extLength3, averageTimeBetween,
                  OnePt5Plus1_Occurs, Two1pt5_Occurs, Two1PtZero_Occurs;
            float LeavingUntilNextMS_1, MSatExtra1, msNormally1, lengthFor1;
            float LeavingUntilNextMS_2, MSatExtra2, msNormally2, lengthFor2;
            float LeavingUntilNextMS_3, MSatExtra3, msNormally3, lengthFor3;
            float timeInBetween = MSBaseCd - 1.5f;
            float useExeifMSHasMoreThan, useSlamifMSHasMoreThan;
            string canUse1, canUse2, canUse3;
            float HPS;
            #region Abilities
            while (
                    Iterator < 50 &&
                    (
                     Math.Abs(BLS.numActivates - oldBLSGCDs) > 0.1f ||
                     Math.Abs(MS.numActivates - oldMSGCDs) > 0.1f ||
                     Math.Abs(RD.numActivates - oldRDGCDs) > 0.1f ||
                     Math.Abs(OP.numActivates - oldOPGCDs) > 0.1f ||
                     Math.Abs(TB.numActivates - oldTBGCDs) > 0.1f ||
                     Math.Abs(SD.numActivates - oldSDGCDs) > 0.1f ||
                     Math.Abs(SL.numActivates - oldSLGCDs) > 0.1f ||
                     (percTimeUnder20 > 0
                        && Math.Abs(EX.numActivates - oldEXGCDs) > 0.1f) ||
                     (Talents.SwordSpecialization > 0
                        && (CombatFactors._c_mhItemType == ItemType.TwoHandSword || CombatFactors._c_mhItemType == ItemType.OneHandSword)
                        && Math.Abs(SS.numActivates - oldSSActs) > 0.1f)
                    )
                  )
            {
                // Reset a couple of items so we can keep iterating
                //availGCDs = origavailGCDs;
                WhiteAtks.Slam_ActsOverDur = SL.numActivates;
                oldBLSGCDs = BLS.numActivates; oldMSGCDs = MS.numActivates; oldRDGCDs = RD.numActivates; oldOPGCDs = OP.numActivates; oldTBGCDs = TB.numActivates;
                oldSDGCDs = SD.numActivates; oldEXGCDs = EX.numActivates; oldSLGCDs = SL.numActivates; oldSSActs = SS.numActivates;
                BLS.numActivates = MS.numActivates = RD.numActivates = OP.numActivates = TB.numActivates = SD.numActivates = EX.numActivates = SL.numActivates = 0;
                availRage = origAvailRage;
                availRage += WhiteAtks.whiteRageGenOverDur * percTimeInDPS * percTimeOver20;

                float acts;
                float Abil_GCDs;
                float RDspace, BLSspace, MSspace, TFBspace, OPspace, SDspace, /*EXspace,*/ SLspace;
                // ==== Primary Ability Priorities ====
                // Rend
                if (RD.ability.Validated) {
                    acts = Math.Min(GCDsAvailable, RD.ability.Activates * percTimeInDPSAndOver20);
                    Abil_GCDs = CalcOpts.AllowFlooring ? (float)Math.Floor(acts) : acts;
                    RD.numActivates = Abil_GCDs;
                    //availGCDs = Math.Max(0f, origNumGCDs - GCDsused);
                    availRage -= RD.Rage;
                }
                RDspace = RD.numActivates / NumGCDs * RD.ability.UseTime / LatentGCD;

                // Reduc abilities due to lack of Rage for maintaining the rotation
                if (repassAvailRage < 0f || PercFailRage != 1f) {
                    // total the amount of rage you really need and turn it into a percentage that we failed
                    PercFailRage *= 1f + repassAvailRage / (availRage - repassAvailRage); // if repassAvailRage was -100 and availRage was 900, then this becomes 1 + (-100 / 900 - (-100)) = 1 - 100/1000 = 90%
                } else { PercFailRage = 1f; }
                
                // Bladestorm
                if (BLS.ability.Validated) {
                    acts = Math.Min(GCDsAvailable, BLS.ability.Activates * percTimeInDPSAndOver20 * PercFailRage);
                    Abil_GCDs = CalcOpts.AllowFlooring ? (float)Math.Floor(acts) : acts;
                    BLS.numActivates = (1f - RDspace) * Abil_GCDs;
                    //availGCDs = Math.Max(0f, origNumGCDs - GCDsused);
                    availRage -= BLS.Rage;
                }
                BLSspace = BLS.numActivates / NumGCDs * BLS.ability.UseTime / LatentGCD;
                // Mortal Strike
                if (MS.ability.Validated) {
                    // TODO: THIS WAS ADDED TO FIX A BUG, JOTHAY'S UNAVAILABLE FOR COMMENT
                    acts = Math.Min(GCDsAvailable, MS.ability.Activates * percTimeInDPSAndOver20 * PercFailRage);
                    #region Mortal Strike Delays
                    /* ===== Delays in MS from Slam or Execute =====
                     * This is a test for MS Delays (idea coming from Landsoul's sheet 2.502)
                     * Note: The numbers displayed are from a specific example, formula
                     * results in Rawr may differ
                     */
                    if (PercFailRage == 1f)
                    {
                        HPS = LandedAtksOverDur;

                        // In-Between MS is: 3.50 seconds
                        // moved up out of the loop
                        //use exe if MS more than 0.320164 sec
                        useExeifMSHasMoreThan = LatentGCD * MS_WeightedValue / (MSBaseCd * (SD_WeightedValue / LatentGCD + 0.03f * Talents.SuddenDeath * HPS * (SD_WeightedValue - SL_WeightedValue)) + MS_WeightedValue);
                        //use slam if MS more than 0.413178 sec
                        useSlamifMSHasMoreThan = LatentGCD * MS_WeightedValue / (MSBaseCd * (SL_WeightedValue / LatentGCD + 0.03f * Talents.SuddenDeath * HPS * (SD_WeightedValue - SL_WeightedValue)) + MS_WeightedValue);

                        //1.5 and 1.0 global is 2.60 seconds
                        // moved out of the loop
                        //leaving until next MS 0.90 seconds
                        LeavingUntilNextMS_1 = timeInBetween - OnePt5Plus1;
                        //Occurs 84.20% of the time
                        //can use exe or slam for 3rd gcd before next ms
                        canUse1 = (useSlamifMSHasMoreThan < LeavingUntilNextMS_1 ? "exe or slam" : (useExeifMSHasMoreThan < LeavingUntilNextMS_1 ? "exe" : "nothing"));
                        //puts MS at extra 0.65 length for 5.70
                        MSatExtra1 = LatentGCD - LeavingUntilNextMS_1;
                        lengthFor1 = (MSBaseCd + CalcOpts.AllowedReact + MSatExtra1);
                        //MS is normally at a length of 5.696
                        msNormally1 = (canUse1 == "exe or slam" ? lengthFor1 : MSBaseCd + CalcOpts.AllowedReact);
                        //Extended length is 100.00% of the time

                        //Two 1.5 globals are 3.10 seconds
                        // move up out of the loop
                        //leaving until next MS 0.40 seconds
                        LeavingUntilNextMS_2 = timeInBetween - Two1pt5;
                        //Occurs 15.52% of the time
                        //can use exe for 3rd gcd before next ms
                        canUse2 = (useSlamifMSHasMoreThan < LeavingUntilNextMS_2 ? "exe or slam" : (useExeifMSHasMoreThan < LeavingUntilNextMS_2 ? "exe" : "nothing"));
                        //puts MS at extra 1.15	length for 6.20
                        MSatExtra2 = LatentGCD - LeavingUntilNextMS_2;
                        lengthFor2 = (MSBaseCd + CalcOpts.AllowedReact + MSatExtra2);
                        //MS is normally at a length of 5.049
                        msNormally2 = (canUse2 == "exe or slam" ? lengthFor2 : MSBaseCd + CalcOpts.AllowedReact);
                        //Extended length is 19.25% of the time

                        //Two 1.0 globals are 2.10 seconds
                        // moved up out of the loop
                        //leaving until next MS 1.40 seconds
                        LeavingUntilNextMS_3 = timeInBetween - Two1pt0;
                        //Occurs 0.28% of the time
                        //can use exe or slam for last gcd before next ms
                        canUse3 = (useSlamifMSHasMoreThan < LeavingUntilNextMS_3 ? "exe or slam" : (useExeifMSHasMoreThan < LeavingUntilNextMS_3 ? "exe" : "nothing"));
                        //puts MS at extra 0.15	length for 5.20
                        MSatExtra3 = LatentGCD - LeavingUntilNextMS_3;
                        lengthFor3 = (MSBaseCd + CalcOpts.AllowedReact + MSatExtra3);
                        //MS is normally at a length of 5.196
                        msNormally3 = (canUse3 == "exe or slam" ? lengthFor3 : MSBaseCd + CalcOpts.AllowedReact);
                        //Extended length is 100.00% of the time

                        float Abilities = ((BLS.numActivates + RD.numActivates + SL.numActivates + acts + oldHSActivates + WhiteAtks.MhActivates + SD.numActivates) / FightDuration);

                        OnePt5Plus1_Occurs = 0f;
                        Two1PtZero_Occurs = TasteForBloodMOD
                                                * OtherMOD
                                                * (
                                                    (1f - 3f * TasteForBloodMOD)
                                                    * Abilities
                                                    * WhiteAtks.MHAtkTable.Dodge
                                                   )
                                                * OtherMOD;
                        OnePt5Plus1_Occurs = (1f - 3f * TasteForBloodMOD)
                                               * Abilities
                                               * WhiteAtks.MHAtkTable.Dodge
                                               * OtherMOD
                                             + TasteForBloodMOD
                                               * OtherMOD
                                             - Two1PtZero_Occurs;
                        Two1pt5_Occurs = 1f - OnePt5Plus1_Occurs - Two1PtZero_Occurs;

                        // Exec procs in MS
                        atleast1 = (1f - (float)Math.Pow(SDMOD, OnePt5Plus1_Occurs * ((canUse1 == "nothing" ? 1f : 2f) * avoid + 1f * (1f - CombatFactors._c_ymiss)) + Two1pt5_Occurs * (canUse2 == "niether" ? 2f : 3f) * avoid + Two1PtZero_Occurs * ((canUse2 == "nothing" ? 0f : 1f) * avoid + 2f * (1f - CombatFactors._c_ymiss)) + (MSBaseCd - (1.5f + CalcOpts.AllowedReact)) / CombatFactors._c_mhItemSpeed));
                        atleast2 = (1f - (float)Math.Pow(SDMOD, OnePt5Plus1_Occurs * ((canUse1 == "nothing" ? 1f : 2f) * avoid + 1f * (1f - CombatFactors._c_ymiss)) + Two1pt5_Occurs * (canUse2 == "nothing" ? 2f : 3f) * avoid + (MSBaseCd - (1.5f + CalcOpts.AllowedReact)) / CombatFactors._c_mhItemSpeed))
                                 * (1f - (float)Math.Pow(SDMOD, OnePt5Plus1_Occurs * ((canUse1 == "nothing" ? 0f : 1f) * avoid + 1.5f / CombatFactors._c_mhItemSpeed) + Two1pt5_Occurs * ((canUse2 == "nothing" ? 0f : 1f) * avoid + 1.5f / CombatFactors._c_mhItemSpeed)));
                        atleast3 = (1f - (float)Math.Pow(SDMOD, Two1pt5_Occurs * (canUse2 == "nothing" ? 2f : 3f) * avoid + (MSBaseCd - (1.5f + CalcOpts.AllowedReact)) / CombatFactors._c_mhItemSpeed))
                                 * (1f - (float)Math.Pow(SDMOD, Two1pt5_Occurs * ((canUse2 == "nothing" ? 0f : 1f) * avoid + 1.5f / CombatFactors._c_mhItemSpeed)))
                                 * (1f - (float)Math.Pow(SDMOD, Two1pt5_Occurs * ((canUse2 == "nothing" ? 0f : 1f) * avoid)));

                        extLength1 = (canUse1 == "exe" ? 0.5f * (atleast1 + atleast2 + atleast3) : (canUse1 == "exe or slam" ? 1f : 0f));
                        extLength2 = (canUse2 == "exe" ? 0.5f * (atleast1 + atleast2 + atleast3) : (canUse2 == "exe or slam" ? 1f : 0f));
                        extLength3 = (canUse3 == "exe" ? 0.5f * (atleast1 + atleast2 + atleast3) : (canUse3 == "exe or slam" ? 1f : 0f));

                        //for avg of 5.628472631 between MS'es, from crowding
                        averageTimeBetween = OnePt5Plus1_Occurs * (lengthFor1 * extLength1 + msNormally1 * (1f - extLength1))
                                           + Two1pt5_Occurs * (lengthFor2 * extLength2 + msNormally2 * (1f - extLength2))
                                           + Two1PtZero_Occurs * (lengthFor3 * extLength3 + msNormally3 * (1f - extLength3));
                        MS.ability.Cd = averageTimeBetween;
                        
                    }
                    #endregion
                    acts = Math.Min(GCDsAvailable, MS.ability.Activates * percTimeInDPSAndOver20 * PercFailRage);
                    Abil_GCDs = CalcOpts.AllowFlooring ? (float)Math.Floor(acts) : acts;
                    MS.numActivates = (1f - BLSspace) * Abil_GCDs;
                    //availGCDs = Math.Max(0f, origNumGCDs - GCDsused);
                    availRage -= MS.Rage;
                }
                MSspace = MS.numActivates / NumGCDs * MS.ability.UseTime / LatentGCD;
                // Taste for Blood
                float OPGCDReduc = (OP.ability.Cd < LatentGCD ? (OP.ability.Cd + CalcOpts.Latency) / LatentGCD : 1f);
                if (TB.ability.Validated)
                {
                    acts = Math.Min(GCDsAvailable, TB.ability.Activates * percTimeInDPSAndOver20 * PercFailRage);
                    Abil_GCDs = CalcOpts.AllowFlooring ? (float)Math.Floor(acts) : acts;
                    TB.numActivates = Abil_GCDs;
                    //availGCDs = Math.Max(0f, origNumGCDs - GCDsused);
                    availRage -= TB.Rage;
                }
                TFBspace = TB.numActivates / NumGCDs * TB.ability.UseTime / LatentGCD;
                // Overpower
                if (OP.ability.Validated) {
                    OverPower _OP = OP.ability as OverPower;
                    acts = Math.Min(GCDsAvailable, _OP.GetActivates(DodgedYellowsOverDur, ParriedYellowsOverDur, SS.numActivates) * percTimeInDPSAndOver20 * PercFailRage);
                    Abil_GCDs = CalcOpts.AllowFlooring ? (float)Math.Floor(acts) : acts;
                    OP.numActivates = Abil_GCDs * (1f - TFBspace - RDspace - BLSspace - MSspace);
                    //availGCDs = Math.Max(0f, origNumGCDs - GCDsused);
                    availRage -= OP.Rage;
                }
                OPspace = OP.numActivates / NumGCDs * OP.ability.UseTime / LatentGCD;
               
                // Sudden Death
                // the atleast (1 to 3) comes from MS Delays, this does already factor talent rate in
                if (SD.ability.Validated) {
                    #region Sudden Death Delays
                    if (false) {
                        //float execSpace = LatentGCD * (atleast1 + atleast2 + atleast3) / MS.Cd;
                        //float attemptspersec = execSpace / LatentGCD * (1f - 0f/*AB81 rage slip*/);
                        //acts = attemptspersec * FightDuration;
                        //acts *= percTimeInDPSAndOver20;
                    } else {
                        acts = Math.Min(GCDsAvailable, SD_ability.GetActivates(AttemptedAtksOverDur) * percTimeInDPSAndOver20 * PercFailRage);
                    }
                    #endregion
                    Abil_GCDs = CalcOpts.AllowFlooring ? (float)Math.Floor(acts) : acts;
                    SD.numActivates = (1f - MSspace) * Abil_GCDs;
                    SD_ability.FreeRage = 0f; // we will do Extra Rage later
                    //SD_ability.SetGCDTime(AttemptedAtksOverDur);
                    /*float rageuse = SD.GetRageUseOverDur(_SD_GCDs);
                    float extraRage = availRage - rageuse;
                    if        (availRage > 0f && extraRage >  0f) {
                        // There's Rage available and more than enough to support it
                    } else if (availRage > 0f && extraRage <= 0f) {
                        // There's Rage available but not enough to support it
                        // so we need to drop off some SD GCDs
                        float perc2Drop = availRage / rageuse;
                        _SD_GCDs *= perc2Drop;
                    } else if (availRage <= 0f) {
                        // There's no Rage available to support this ability
                        _SD_GCDs = 0f;
                    }*/
                    //availGCDs = Math.Max(0f, origNumGCDs - GCDsused);
                    availRage -= SD.Rage;
                }
                SDspace = (SD.numActivates / NumGCDs) * (SD.ability.UseTime / LatentGCD);

                // Slam for remainder of GCDs
                if (SL.ability.Validated && PercFailRage == 1f)
                {
                    acts = Math.Min(GCDsAvailable, GCDsAvailable/*SL.Activates*/ * percTimeInDPS);
                    Abil_GCDs = CalcOpts.AllowFlooring ? (float)Math.Floor(acts) : acts;
                    if (SL.ability.GetRageUseOverDur(Abil_GCDs) > availRage) Abil_GCDs = Math.Max(0f, availRage) / SL.ability.RageCost;
                    SL.numActivates = Abil_GCDs;
                    //availGCDs = Math.Max(0f, origNumGCDs - GCDsused);
                    availRage -= SL.Rage;
                } else { SL.numActivates = 0f; }
                WhiteAtks.Slam_ActsOverDur = SL.numActivates;
                SLspace = SL.numActivates / NumGCDs * SL.ability.UseTime / LatentGCD;

                // Sword Spec Procs
                if (SS.ability.Validated) {
                    Swordspec _SS = SS.ability as Swordspec;
                    SS.numActivates = _SS.GetActivates(LandedYellowsOverDur, WhiteAtks.HSOverridesOverDur, WhiteAtks.CLOverridesOverDur) * percTimeOver20;
                    availRage -= SS.Rage;
                }

                float TotalSpace = (RDspace + BLSspace + MSspace + OPspace + TFBspace + SDspace + SLspace);
                repassAvailRage = availRage; // check for not enough rage to maintain rotation
                InvalidateCache();
                Iterator++;
            }
            #endregion
            #region OnAttacks
            if (availRage > 0f && PercFailRage == 1f && (hsok || clok))
            { // We need extra rage beyond the rotation to HS/CL and we don't HS/CL when parts of our rotation were failing for lack of rage
                float savedAvailRage = availRage - WhiteAtks.whiteRageGenOverDurNoHS * percTimeInDPSAndOver20;
                float newSSActivates, oldSSActivates;
                Iterator = 0;
                do {
                    OnAttack _HS = HS.ability as OnAttack;
                    OnAttack _CL = CL.ability as OnAttack;

                    availRage = savedAvailRage + WhiteAtks.whiteRageGenOverDur * percTimeInDPSAndOver20;
                    oldHSActivates = HS.numActivates;
                    oldCLActivates = CL.numActivates;
                    oldSSActivates = SS.numActivates;

                    availRage += SD.Rage; // add back the non-extra rage using
                    float possibleFreeRage = availRage / (FightDuration * percTimeOver20);
                    SD_ability.FreeRage = possibleFreeRage;//50f;
                    availRage -= SD.Rage;

                    // Assign Rage to each ability
                    float RageForHSCL = availRage * percTimeOver20;
#if RAWR3 || SILVERLIGHT
                    //RageForCL = clok ? (!hsok ? RageForHSCL : RageForHSCL * ((float)BossOpts.MultiTargsPerc/* / 100f*/)) : 0f;
                    {
                        float time = 0;
                        foreach (TargetGroup tg in BossOpts.Targets)
                        {
                            if (tg.Chance <= 0 || tg.Frequency <= 0 || tg.Duration <= 0) continue;
                            time += tg.Frequency / BossOpts.BerserkTimer * tg.Duration / 1000f;
                        }
                        float perc = time / BossOpts.BerserkTimer;
                        RageForCL = clok ? (!hsok ? RageForHSCL : RageForHSCL * (Math.Max(0f, Math.Min(1f, perc)))) : 0f;
                    }
#else
                    RageForCL = clok ? (!hsok ? RageForHSCL : RageForHSCL * (CalcOpts.MultipleTargetsPerc / 100f)) : 0f;
#endif
                    RageForHS = hsok ? RageForHSCL - RageForCL : 0f;

                    float val1 = (RageForHS / _HS.FullRageCost), val2 = (RageForCL / _CL.FullRageCost);
                    if (CalcOpts.AllowFlooring) { val1 = (float)Math.Floor(val1); val2 = (float)Math.Floor(val2); }
                    HS.numActivates = WhiteAtks.HSOverridesOverDur = val1;
                    CL.numActivates = WhiteAtks.CLOverridesOverDur = val2;
                    availRage -= RageForHSCL;

                    if (SS.ability.Validated)
                    {
                        Swordspec _SS = SS.ability as Swordspec;
                        SS.numActivates = _SS.GetActivates(LandedYellowsOverDur, WhiteAtks.HSOverridesOverDur, WhiteAtks.CLOverridesOverDur);
                    }
                    
                    // Final Prep for Next iter
                    newHSActivates = HS.numActivates;
                    newCLActivates = CL.numActivates;
                    newSSActivates = SS.numActivates;

                    Iterator++;
                } while (Iterator < 50 && (
                        (hsok && Math.Abs(newHSActivates - oldHSActivates) > 0.01f) ||
                        (clok && Math.Abs(newCLActivates - oldCLActivates) > 0.01f) ||
                        (SS.ability.Validated && Math.Abs(newSSActivates - oldSSActivates) > 0.01f)));
            }
            #endregion
            #endregion
          
            float DPS_TTL = 0f;
            float rageNeeded = 0f, rageGenOther = 0f;
            foreach (AbilWrapper aw in GetAbilityList())
            {
                DPS_TTL += aw.DPS;
                _HPS_TTL += aw.HPS;
                if (aw.Rage > 0) rageNeeded += aw.Rage;
                else rageGenOther -= aw.Rage;
            }

            DPS_TTL += (WhiteAtks.MhDPS + (CombatFactors.useOH ? WhiteAtks.OhDPS : 0f)) * percTimeInDPSAndOver20;
           // InvalidateCache();
            return DPS_TTL;
        }

        public float SettleAll_U20(float totalPercTimeLost, float percTimeUnder20, float availRage, out float PercFailRageUnder20)
        {
            float percTimeOver20 = (1f - percTimeUnder20);
            float percTimeInDPS = (1f - totalPercTimeLost);
            float percTimeInDPSAndUnder20 = percTimeUnder20 * percTimeInDPS;
            availRage -= DoMaintenanceActivates(totalPercTimeLost) * percTimeUnder20;
            //availGCDs = NumGCDs - GCDsused;
            /* The following are dependant on other attacks as they are proccing abilities or are the fallback item
             * We need to loop these until the activates are relatively unchanged
             * Heroic Strike, when there is rage to do so, handled by the Heroic Strike class
             * Alternate to Cleave is MultiTargs is active, but only to the perc of time where Targs is active
             * After iterating how many Overrides can be done and still do other abilities, then do the white dps
             *
             * Starting Assumptions:
             * No ability ever procs so Slam sucks up all the cooldowns (except under <20% with that active, where Exec sucks all of them)
             * Heroic Strike and Cleave won't be used at all
             * Sudden Death Free Rage is minimum cost, no extra rage available
             * Execute Free Rage is minimum cost, no extra rage available
             * 
             * Hoped Ending Results:
             * All abilities will have proc'd and abilities that can proc from other ones will have their activates settled
             * Heroic Strikes and Cleave will activate when there's enough rage to support them AND Executes
             * Sudden Death will get extra rage leftovers if there are any
             * Execute will get extra rage leftovers if there are any (since you won't use HS/CL <20%)
            */

            float preloopAvailGCDs = GCDsAvailable, preloopGCDsUsed = GCDsUsed, preloopAvailRage = availRage;

            float origNumGCDs = (CalcOpts.AllowFlooring ? (float)Math.Floor(FightDuration / LatentGCD) : FightDuration / LatentGCD) * (/*1f -*/ percTimeUnder20),
                  origavailGCDs = preloopAvailGCDs * (/*1f -*/ percTimeUnder20),
                  origGCDsused = preloopGCDsUsed * (/*1f -*/ percTimeUnder20);
            float oldBLSGCDs = 0f, //oldMSGCDs = 0f,
                  oldRDGCDs = 0f, oldOPGCDs = 0f, oldTBGCDs = 0f,
                  oldSDGCDs = 0f, oldEXGCDs = 0f, //oldSLGCDs = 0f,
                  oldSSActs = 0f;

            AbilWrapper SL = GetWrapper<Slam>();
            AbilWrapper SD = GetWrapper<Suddendeath>();
            AbilWrapper EX = GetWrapper<Execute>();
            AbilWrapper HS = GetWrapper<HeroicStrike>();
            AbilWrapper CL = GetWrapper<Cleave>();
            AbilWrapper MS = GetWrapper<MortalStrike>();
            AbilWrapper OP = GetWrapper<OverPower>();
            AbilWrapper BLS = GetWrapper<Bladestorm>();
            AbilWrapper RD = GetWrapper<Rend>();
            AbilWrapper TB = GetWrapper<TasteForBlood>();
            AbilWrapper SS = GetWrapper<Swordspec>();

            Execute EX_ability = EX.ability as Execute;
            Suddendeath SD_ability = SD.ability as Suddendeath;

            EX.numActivatesU20 = origavailGCDs;
            WhiteAtks.Slam_ActsOverDur = 0f;// SL.numActivates;
            SD_ability.FreeRage = SD_ability.RageCost;
            EX_ability.FreeRage = EX_ability.RageCost;
            float oldHSActivates = 0f, RageForHS = 0f, newHSActivates = HS.numActivatesU20 /*= WhiteAtks.HSOverridesOverDur*/ = 0f;
            float oldCLActivates = 0f, RageForCL = 0f, newCLActivates = CL.numActivatesU20 /*= WhiteAtks.CLOverridesOverDur*/ = 0f;
            float origAvailRage = preloopAvailRage * (/*1f -*/ percTimeUnder20);
            bool hsok = false; // CalcOpts.Maintenance[(int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.HeroicStrike_];
            bool clok =
#if RAWR3 || SILVERLIGHT
                false; // BossOpts.MultiTargs && BossOpts.Targets != null && BossOpts.Targets.Count > 0
                //&& CalcOpts.Maintenance[(int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.Cleave_];
#else
                CalcOpts.MultipleTargets && CalcOpts.MultipleTargetsPerc > 0
                && CalcOpts.Maintenance[(int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.Cleave_];
#endif
            availRage += WhiteAtks.whiteRageGenOverDurNoHS * percTimeInDPS * (/*1f -*/ percTimeUnder20);
            availRage -= EX.RageU20;
            float repassAvailRage = 0f;
            PercFailRageUnder20 = 1f;

            int Iterator = 0;
            #region <20%
            // Run the loop for <20%
            //float MSBaseCd = 6f - Talents.ImprovedMortalStrike / 3f;
            /*float MS_WeightedValue = MS.ability.DamageOnUse + DW.TickSize * MS.ability.MHAtkTable.Crit,
                  SD_WeightedValue = SD.ability.DamageOnUse + DW.TickSize * SD.ability.MHAtkTable.Crit,
                  SL_WeightedValue = SL.ability.DamageOnUse + DW.TickSize * SL.ability.MHAtkTable.Crit;
            float OnePt5Plus1 = LatentGCD + (OP.ability.Cd + CalcOpts.AllowedReact),
                  Two1pt5 = LatentGCD * 2f,
                  Two1pt0 = (OP.ability.Cd + CalcOpts.AllowedReact) * 2f;*/
            //float TasteForBloodMOD = (Talents.TasteForBlood == 3 ? 1f / 6f : (Talents.TasteForBlood == 2 ? 0.144209288653733f : (Talents.TasteForBlood == 1 ? 0.104925207394343f : 0)));
            //float OtherMOD = (MSBaseCd + CalcOpts.Latency);
            //float SDMOD = 1f - 0.03f * Talents.SuddenDeath;
            //float avoid = (1f - CombatFactors._c_mhdodge - CombatFactors._c_ymiss);
            //float atleast1 = 0f, atleast2 = 0f, atleast3 = 0f, extLength1, extLength2, extLength3, averageTimeBetween, OnePt5Plus1_Occurs, Two1pt5_Occurs, Two1PtZero_Occurs;
            //float LeavingUntilNextMS_1, MSatExtra1, msNormally1, lengthFor1;
            //float LeavingUntilNextMS_2, MSatExtra2, msNormally2, lengthFor2;
            //float LeavingUntilNextMS_3, MSatExtra3, msNormally3, lengthFor3;
            //float timeInBetween = MSBaseCd - 1.5f;
            //float useExeifMSHasMoreThan, useSlamifMSHasMoreThan;
            //string canUse1, canUse2, canUse3;
            //float HPS;
            #region Abilities
            while (
                    Iterator < 50 &&
                    (
                     Math.Abs(BLS.numActivates - oldBLSGCDs) > 0.1f ||
                     //Math.Abs(MS.numActivates - oldMSGCDs) > 0.1f ||
                     Math.Abs(RD.numActivates - oldRDGCDs) > 0.1f ||
                     Math.Abs(OP.numActivates - oldOPGCDs) > 0.1f ||
                     Math.Abs(TB.numActivates - oldTBGCDs) > 0.1f ||
                     Math.Abs(SD.numActivates - oldSDGCDs) > 0.1f ||
                     //Math.Abs(SL.numActivates - oldSLGCDs) > 0.1f ||
                     (percTimeUnder20 > 0
                        && Math.Abs(EX.numActivates - oldEXGCDs) > 0.1f) ||
                     (Talents.SwordSpecialization > 0
                        && (CombatFactors._c_mhItemType == ItemType.TwoHandSword || CombatFactors._c_mhItemType == ItemType.OneHandSword)
                        && Math.Abs(SS.numActivates - oldSSActs) > 0.1f)
                    )
                  )
            {
                // Reset a couple of items so we can keep iterating
                //availGCDs = origavailGCDs;
                //WhiteAtks.Slam_Freq = SL.numActivates;
                oldBLSGCDs = BLS.numActivatesU20; /*oldMSGCDs = MS.numActivates;*/ oldRDGCDs = RD.numActivatesU20; oldOPGCDs = OP.numActivatesU20; oldTBGCDs = TB.numActivatesU20;
                oldSDGCDs = SD.numActivatesU20; oldEXGCDs = EX.numActivatesU20; /*oldSLGCDs = SL.numActivatesU20;*/ oldSSActs = SS.numActivatesU20;
                BLS.numActivatesU20 = /*MS.numActivatesU20 =*/ RD.numActivatesU20 = OP.numActivatesU20 = TB.numActivatesU20 = SD.numActivatesU20 = EX.numActivatesU20 = SL.numActivatesU20 = 0;
                availRage = origAvailRage;
                availRage += WhiteAtks.whiteRageGenOverDur * percTimeInDPS * (/*1f -*/ percTimeUnder20);

                float acts;
                float Abil_GCDs;
                float RDspace, BLSspace, /*MSspace,*/ TFBspace, OPspace, SDspace, EXspace/*, SLspace*/;
                // ==== Primary Ability Priorities ====
                // Rend
                if (RD.ability.Validated)
                {
                    acts = Math.Min(GCDsAvailableU20, RD.ability.Activates * percTimeInDPSAndUnder20);
                    Abil_GCDs = CalcOpts.AllowFlooring ? (float)Math.Floor(acts) : acts;
                    RD.numActivatesU20 = Abil_GCDs;
                    //availGCDs = Math.Max(0f, origNumGCDs - GCDsused);
                    availRage -= RD.RageU20;

                }
                RDspace = RD.numActivatesU20 / NumGCDs * RD.ability.UseTime / LatentGCD;

                // Reduc abilities due to lack of Rage for maintaining the rotation
                if (repassAvailRage < 0f || PercFailRageUnder20 != 1f)
                {
                    // total the amount of rage you really need and turn it into a percentage that we failed
                    PercFailRageUnder20 *= 1f + repassAvailRage / (availRage - repassAvailRage); // if repassAvailRage was -100 and availRage was 900, then this becomes 1 + (-100 / 900 - (-100)) = 1 - 100/1000 = 90%
                }
                else { PercFailRageUnder20 = 1f; }

                // Bladestorm
                if (BLS.ability.Validated)
                {
                    acts = Math.Min(GCDsAvailableU20, BLS.ability.Activates * percTimeInDPSAndUnder20 * PercFailRageUnder20);
                    Abil_GCDs = CalcOpts.AllowFlooring ? (float)Math.Floor(acts) : acts;
                    BLS.numActivatesU20 = (1f - RDspace) * Abil_GCDs;
                    //availGCDs = Math.Max(0f, origNumGCDs - GCDsused);
                    availRage -= BLS.RageU20;
                }
                BLSspace = BLS.numActivatesU20 / NumGCDs * BLS.ability.UseTime / LatentGCD;
                // Taste for Blood
                float OPGCDReduc = (OP.ability.Cd < LatentGCD ? (OP.ability.Cd + CalcOpts.Latency) / LatentGCD : 1f);
                if (TB.ability.Validated)
                {
                    acts = Math.Min(GCDsAvailableU20, TB.ability.Activates * percTimeInDPSAndUnder20 * PercFailRageUnder20);
                    Abil_GCDs = CalcOpts.AllowFlooring ? (float)Math.Floor(acts) : acts;
                    TB.numActivatesU20 = Abil_GCDs;
                    //availGCDs = Math.Max(0f, origNumGCDs - GCDsused);
                    availRage -= TB.RageU20;
                }
                TFBspace = TB.numActivatesU20 / NumGCDs * TB.ability.UseTime / LatentGCD;
                // Overpower
                if (OP.ability.Validated)
                {
                    OverPower _OP = OP.ability as OverPower;
                    acts = Math.Min(GCDsAvailableU20, _OP.GetActivates(DodgedYellowsOverDur, ParriedYellowsOverDur, SS.numActivatesU20) * percTimeInDPSAndUnder20 * PercFailRageUnder20);
                    Abil_GCDs = CalcOpts.AllowFlooring ? (float)Math.Floor(acts) : acts;
                    OP.numActivatesU20 = Abil_GCDs * (1f - TFBspace - RDspace - BLSspace /*- MSspace*/);
                    //availGCDs = Math.Max(0f, origNumGCDs - GCDsused);
                    availRage -= OP.RageU20;
                }
                OPspace = OP.numActivatesU20 / NumGCDs * OP.ability.UseTime / LatentGCD;

                // Sudden Death
                // the atleast (1 to 3) comes from MS Delays, this does already factor talent rate in
                if (SD.ability.Validated)
                {
                    #region Sudden Death Delays
                    if (false) {
                        //float execSpace = LatentGCD * (atleast1 + atleast2 + atleast3) / MS.Cd;
                        //float attemptspersec = execSpace / LatentGCD * (1f - 0f/*AB81 rage slip*/);
                        //acts = attemptspersec * FightDuration;
                        //acts *= percTimeInDPS * percTimeOver20;
                    } else {
                        acts = Math.Min(GCDsAvailableU20, SD_ability.GetActivates(AttemptedAtksOverDur) * percTimeInDPSAndUnder20 * PercFailRageUnder20);
                    }
                    #endregion
                    Abil_GCDs = CalcOpts.AllowFlooring ? (float)Math.Floor(acts) : acts;
                    SD.numActivatesU20 = (1f /*- MSspace*/) * Abil_GCDs;
                    SD_ability.FreeRage = 0f; // we will do Extra Rage later
                    //SD_ability.SetGCDTime(AttemptedAtksOverDur);
                    /*float rageuse = SD.GetRageUseOverDur(_SD_GCDs);
                    float extraRage = availRage - rageuse;
                    if        (availRage > 0f && extraRage >  0f) {
                        // There's Rage available and more than enough to support it
                    } else if (availRage > 0f && extraRage <= 0f) {
                        // There's Rage available but not enough to support it
                        // so we need to drop off some SD GCDs
                        float perc2Drop = availRage / rageuse;
                        _SD_GCDs *= perc2Drop;
                    } else if (availRage <= 0f) {
                        // There's no Rage available to support this ability
                        _SD_GCDs = 0f;
                    }*/
                    //availGCDs = Math.Max(0f, origNumGCDs - GCDsused);
                    availRage -= SD.RageU20;
                }
                SDspace = (SD.numActivatesU20 / NumGCDs) * (SD.ability.UseTime / LatentGCD);

                // Execute for remainder of GCDs
                if (EX.ability.Validated /*&& PercFailRage == 1f*/)
                {
                    acts = Math.Min(GCDsAvailableU20, GCDsAvailableU20/*SL.Activates*/ * percTimeInDPS);
                    Abil_GCDs = CalcOpts.AllowFlooring ? (float)Math.Floor(acts) : acts;
                    if (EX.ability.GetRageUseOverDur(Abil_GCDs) > availRage) Abil_GCDs = Math.Max(0f, availRage) / EX.ability.RageCost;
                    EX.numActivatesU20 = Abil_GCDs;
                    //availGCDs = Math.Max(0f, origNumGCDs - GCDsused);
                    availRage -= EX.RageU20;
                }
                else { EX.numActivatesU20 = 0f; }
                EXspace = EX.numActivatesU20 / NumGCDs * EX.ability.UseTime / LatentGCD;

                // Sword Spec Procs
                if (SS.ability.Validated)
                {
                    Swordspec _SS = SS.ability as Swordspec;
                    SS.numActivatesU20 = _SS.GetActivates(LandedYellowsOverDur, WhiteAtks.HSOverridesOverDur, WhiteAtks.CLOverridesOverDur) * percTimeUnder20;
                    availRage -= SS.RageU20;
                }

                float TotalSpace = (RDspace + BLSspace /*+ MSspace*/ + OPspace + TFBspace + SDspace /*+ SLspace*/ + EXspace);
                repassAvailRage = availRage; // check for not enough rage to maintain rotation
                InvalidateCache();
                Iterator++;
            }
            #endregion
            #region OnAttacks
            if (false && availRage > 0f && PercFailRageUnder20 == 1f && (hsok || clok))
            { // We need extra rage beyond the rotation to HS/CL and we don't HS/CL when parts of our rotation were failing for lack of rage
                float savedAvailRage = availRage - WhiteAtks.whiteRageGenOverDurNoHS;
                float newSSActivates, oldSSActivates;
                Iterator = 0;
                do
                {
                    OnAttack _HS = HS.ability as OnAttack;
                    OnAttack _CL = CL.ability as OnAttack;

                    availRage = savedAvailRage + WhiteAtks.whiteRageGenOverDur * percTimeInDPS * percTimeOver20; ;
                    oldHSActivates = HS.numActivates;
                    oldCLActivates = CL.numActivates;
                    oldSSActivates = SS.numActivates;

                    availRage += SD.RageU20; // add back the non-extra rage using
                    float possibleFreeRage = availRage / (FightDuration * percTimeOver20);
                    SD_ability.FreeRage = possibleFreeRage;//50f;
                    availRage -= SD.RageU20;

                    // Assign Rage to each ability
                    float RageForHSCL = availRage * percTimeOver20;
#if RAWR3 || SILVERLIGHT
                    {
                        float time = 0;
                        foreach (TargetGroup tg in BossOpts.Targets)
                        {
                            if (tg.Chance <= 0 || tg.Frequency <= 0 || tg.Duration <= 0) continue;
                            time += tg.Frequency / BossOpts.BerserkTimer * tg.Duration / 1000f;
                        }
                        float perc = time / BossOpts.BerserkTimer;
                        RageForCL = clok ? (!hsok ? RageForHSCL : RageForHSCL * (Math.Max(0f, Math.Min(1f, perc)))) : 0f;
                    }
#else
                    RageForCL = clok ? (!hsok ? RageForHSCL : RageForHSCL * (CalcOpts.MultipleTargetsPerc / 100f)) : 0f;
#endif
                    RageForHS = hsok ? RageForHSCL - RageForCL : 0f;

                    float val1 = (RageForHS / _HS.FullRageCost), val2 = (RageForCL / _CL.FullRageCost);
                    if (CalcOpts.AllowFlooring) { val1 = (float)Math.Floor(val1); val2 = (float)Math.Floor(val2); }
                    HS.numActivates = WhiteAtks.HSOverridesOverDur = val1;
                    CL.numActivates = WhiteAtks.CLOverridesOverDur = val2;
                    availRage -= RageForHSCL;

                    if (SS.ability.Validated)
                    {
                        Swordspec _SS = SS.ability as Swordspec;
                        SS.numActivates = _SS.GetActivates(LandedYellowsOverDur, WhiteAtks.HSOverridesOverDur, WhiteAtks.CLOverridesOverDur);
                    }

                    // Final Prep for Next iter
                    newHSActivates = HS.numActivates;
                    newCLActivates = CL.numActivates;
                    newSSActivates = SS.numActivates;

                    Iterator++;
                } while (Iterator < 50 && (
                        (hsok && Math.Abs(newHSActivates - oldHSActivates) > 0.01f) ||
                        (clok && Math.Abs(newCLActivates - oldCLActivates) > 0.01f) ||
                        (SS.ability.Validated && Math.Abs(newSSActivates - oldSSActivates) > 0.01f)));
            }
            #endregion
            #endregion

            float DPS_TTL = 0f;
            float rageNeeded = 0f, rageGenOther = 0f;
            foreach (AbilWrapper aw in GetAbilityList())
            {
                DPS_TTL += aw.DPSU20;
                _HPS_TTL += aw.HPSU20;
                if (aw.RageU20 > 0) rageNeeded += aw.RageU20;
                else rageGenOther -= aw.RageU20;
            }

            DPS_TTL += (WhiteAtks.MhDPS + (CombatFactors.useOH ? WhiteAtks.OhDPS : 0f)) * percTimeInDPSAndUnder20;
            // InvalidateCache();
            return DPS_TTL;
        }

        public void MakeRotationandDoDPS(bool setCalcs, float percTimeUnder20) {
            if (Char.MainHand == null) { return; }
            _HPS_TTL = 0f;
            if (_needDisplayCalcs) GCDUsage += NumGCDs.ToString("000") + " : Total GCDs\n\n";
            
            float TotalPercTimeLost = CalculateTimeLost(GetWrapper<MortalStrike>().ability);
            
            if (_needDisplayCalcs) { GCDUsage += (TotalPercTimeLost != 0f ? "\n" : ""); }

            // ==== Rage Generation Priorities ========
            float availRage = 0f;
            float PercFailRageUnder20 = 1f, PercFailRage = 1f;
            availRage += RageGenOverDur_Other + RageGainedWhileMoving;

            // ==== Standard Priorities ===============
            _DPS_TTL = SettleAll(TotalPercTimeLost, percTimeUnder20, availRage, out PercFailRage);
            if (percTimeUnder20 != 0) { _DPS_TTL_U20 = SettleAll_U20(TotalPercTimeLost, percTimeUnder20, availRage, out PercFailRageUnder20); }

            calcDeepWounds();
            _DPS_TTL += DW.TickSize;
            //_DPS_TTL += GetWrapper<Skills.SpellDamageEffect>().DPS;

            // Add each of the abilities' DPS and HPS values and other aesthetics
            if (_needDisplayCalcs)
            {
                if (PercFailRage != 1.0f || PercFailRageUnder20 != 1.0f)
                {
                    GCDUsage += (PercFailRage < 1.0f ? string.Format("WARNING! You are losing {0:0.0%} of your abilities due to Rage Starvation before Exec Spam.\n", (1f - PercFailRage)) : "");
                    GCDUsage += (PercFailRageUnder20 < 1.0f ? string.Format("WARNING! You are losing {0:0.0%} of your abilities due to Rage Starvation during Exec Spam.\n", (1f - PercFailRageUnder20)) : "");
                    GCDUsage += "\n";
                }
                foreach (AbilWrapper aw in GetDamagingAbilities())
                {
                    if (aw.allNumActivates > 0)
                        GCDUsage += aw.allNumActivates.ToString(CalcOpts.AllowFlooring ? "000" : "000.00") + " : " + aw.ability.Name + (aw.ability.UsesGCD ? "\n" : "(Doesn't use GCDs)\n");
                }
                GCDUsage += "\n" + (GCDsAvailable + GCDsAvailableU20).ToString("000") + " : Avail GCDs";
            }


            // Return result
            if (setCalcs) {
                this.calcs.TotalDPS = _DPS_TTL + _DPS_TTL_U20;
                this.calcs.WhiteDPS = WhiteAtks.MhDPS + WhiteAtks.OhDPS;
                this.calcs.WhiteDPSMH = WhiteAtks.MhDPS;
                this.calcs.WhiteDmg = this.WhiteAtks.MhDamageOnUse;

                this.calcs.WhiteRage = WhiteAtks.MHRageGenOverDur;
                this.calcs.OtherRage = this.RageGenOverDur_Other;
                this.calcs.NeedyRage = this.RageNeededOverDur;
                this.calcs.FreeRage = calcs.WhiteRage + calcs.OtherRage - calcs.NeedyRage;
            }
        }

        public override void MakeRotationandDoDPS(bool setCalcs, bool needsDisplayCalculations) {
            base.MakeRotationandDoDPS(setCalcs, needsDisplayCalculations);
            float PercTimeUnder20 = 0f;
            if(CalcOpts.Maintenance[(int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.ExecuteSpam_])
            {
#if RAWR3 || SILVERLIGHT
                PercTimeUnder20 = (float)BossOpts.Under20Perc;
#else
                PercTimeUnder20 = CalcOpts.Under20Perc;
#endif
            }
            MakeRotationandDoDPS(setCalcs, PercTimeUnder20);
        }
    }
}