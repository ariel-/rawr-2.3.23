using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Rawr.Hunter {
    public class ShotPriority {
        #region Variables
        public ShotData[] priorities = new ShotData[10];

        public CalculationOptionsHunter CalcOpts;
        public BossOptions BossOpts;

        public bool chimeraRefreshesSerpent = false;
        public bool chimeraRefreshesViper = false;
        public float LALExplosiveFrequency = 0f;
        public float LALArcaneFrequency = 0f;
        public bool useKillShot = false; // TODO
        public float viperDamagePenalty = 0f;
        public float NoManaPenalty = 0f;

        public float specialShotsPerSecond = 0f;
        public float critSpecialShotsPerSecond = 0f;

        public float critsRatioSum = 0f;
        public float critsCompositeSum = 0f;

        public float DPS = 0f;
        public float MPS = 0f;
        #endregion

        public ShotPriority(CalculationOptionsHunter options) { this.CalcOpts = options; }

        public void validateShots(HunterTalents Talents) { 
            // check each shot in the priority is allowed to be there.
            // this means removing dupes and anything after steady shot.
            // it also means removing shots which have already had their cooldowns used

            bool removeAll = false;
            bool used_arcane_explosive = false;
            bool used_aimed_multi = false;
            bool used_black_immo = false;

            for (int i = 0; i < priorities.Length; i++) {
                // we've already seen steadyShot - remove everything else
                if (priorities[i] != null && removeAll) { priorities[i].FailReason_SteadyBefore = true; priorities[i] = null; }

                // this shot has been used aleady
                if (priorities[i] != null && priorities[i].FailReason_AlreadyUsed) priorities[i] = null;                

                // we've already used a shot which shares cooldown with this one
                if (priorities[i] != null && (priorities[i].Type == Shots.ArcaneShot && used_arcane_explosive)) { priorities[i].FailReason_SharedCooldownUsed = true; priorities[i] = null; }
                if (priorities[i] != null && (priorities[i].Type == Shots.ExplosiveShot && used_arcane_explosive)){ priorities[i].FailReason_SharedCooldownUsed = true; priorities[i] = null;}
                if (priorities[i] != null && (priorities[i].Type == Shots.AimedShot && used_aimed_multi)){ priorities[i].FailReason_SharedCooldownUsed = true; priorities[i] = null;}
                if (priorities[i] != null && (priorities[i].Type == Shots.MultiShot && used_aimed_multi)){ priorities[i].FailReason_SharedCooldownUsed = true; priorities[i] = null;}
                if (priorities[i] != null && (priorities[i].Type == Shots.BlackArrow && used_black_immo)){ priorities[i].FailReason_SharedCooldownUsed = true; priorities[i] = null;}
                if (priorities[i] != null && (priorities[i].Type == Shots.ImmolationTrap && used_black_immo)){ priorities[i].FailReason_SharedCooldownUsed = true; priorities[i] = null;}
                if (priorities[i] != null && (priorities[i].Type == Shots.ExplosiveTrap && used_black_immo)) { priorities[i].FailReason_SharedCooldownUsed = true; priorities[i] = null; }
                if (priorities[i] != null && (priorities[i].Type == Shots.FreezingTrap && used_black_immo)) { priorities[i].FailReason_SharedCooldownUsed = true; priorities[i] = null; }
                if (priorities[i] != null && (priorities[i].Type == Shots.FrostTrap && used_black_immo)) { priorities[i].FailReason_SharedCooldownUsed = true; priorities[i] = null; }
                //if (priorities[i] != null && (priorities[i].Type == Shots.Volley && used_black_immo)) { priorities[i].FailReason_SharedCooldownUsed = true; priorities[i] = null; }

                // Requires Multiple Targets
#if RAWR3 || SILVERLIGHT
                if (priorities[i] != null && (priorities[i].Type == Shots.Volley && (!BossOpts.MultiTargs || (BossOpts.MultiTargs && BossOpts.MultiTargsTime == 0)))) { priorities[i].FailReason_RequiresMultiTargs = true; priorities[i] = null; }
#else
                if (priorities[i] != null && (priorities[i].Type == Shots.Volley && (!CalcOpts.MultipleTargets || (CalcOpts.MultipleTargets && CalcOpts.MultipleTargetsPerc == 0)))) { priorities[i].FailReason_RequiresMultiTargs = true; priorities[i] = null; }
#endif

                // shots which require talents
                if (priorities[i] != null && priorities[i].Type == Shots.BlackArrow && Talents.BlackArrow == 0){ priorities[i].FailReason_LackTalent = true; priorities[i] = null; }
                if (priorities[i] != null && priorities[i].Type == Shots.ChimearaShot && Talents.ChimeraShot == 0) { priorities[i].FailReason_LackTalent = true; priorities[i] = null; }
                if (priorities[i] != null && priorities[i].Type == Shots.AimedShot && Talents.AimedShot == 0) { priorities[i].FailReason_LackTalent = true; priorities[i] = null; }
                if (priorities[i] != null && priorities[i].Type == Shots.ExplosiveShot && Talents.ExplosiveShot == 0){priorities[i].FailReason_LackTalent = true; priorities[i] = null;}
                if (priorities[i] != null && priorities[i].Type == Shots.BestialWrath && Talents.BestialWrath == 0) { priorities[i].FailReason_LackTalent = true; priorities[i] = null; }
                if (priorities[i] != null && priorities[i].Type == Shots.SilencingShot && Talents.SilencingShot == 0) { priorities[i].FailReason_LackTalent = true; priorities[i] = null; }

                if (priorities[i] != null)
                {
                    priorities[i].FailReason_AlreadyUsed = true;
                    if (priorities[i].Type == Shots.SteadyShot) removeAll = true;

                    if (priorities[i].Type == Shots.ArcaneShot) used_arcane_explosive = true;
                    if (priorities[i].Type == Shots.ExplosiveShot) used_arcane_explosive = true;
                    if (priorities[i].Type == Shots.AimedShot) used_aimed_multi = true;
                    if (priorities[i].Type == Shots.MultiShot) used_aimed_multi = true;
                    if (priorities[i].Type == Shots.BlackArrow) used_black_immo = true;
                    if (priorities[i].Type == Shots.ImmolationTrap) used_black_immo = true;
                    if (priorities[i].Type == Shots.ExplosiveTrap) used_black_immo = true;
                    if (priorities[i].Type == Shots.FreezingTrap) used_black_immo = true;
                    if (priorities[i].Type == Shots.FrostTrap) used_black_immo = true;
                    //if (priorities[i].Type == Shots.Volley) used_black_immo = true;
                }
            }
   
            // store some derived facts about the rotation for later use
            chimeraRefreshesSerpent = false;
            chimeraRefreshesViper = false;
            if (containsShot(Shots.ChimearaShot))
            {
                if (containsShot(Shots.SerpentSting)) chimeraRefreshesSerpent = true;
                if (containsShot(Shots.ViperSting)) chimeraRefreshesViper = true;
            }
        }

        public void initializeTimings()
        {
            for (int i = 0; i < priorities.Length; i++)
            {
                if (priorities[i] != null) priorities[i].initializeTimings(this);
            }
        }

        public bool containsShot(Shots aType)
        {
            for (int i=0; i<priorities.Length; i++)
            {
                if (priorities[i] != null)
                {
                    if (priorities[i].Type == aType) return true;
                }
            }

            return false;
        }

        public ShotData getShotInRotation(Shots aType)
        {
            for (int i = 0; i < priorities.Length; i++)
            {
                if (priorities[i] != null)
                {
                    if (priorities[i].Type == aType) return priorities[i];
                }
            }

            return null;
        }

        public void calculateFrequencies()
        {
            // This function calculates the frequencies for each shot.
            // We have already calculated all shot cooldowns and validated the rotation by this point.

            // THIS FUNCTION IS CALLED THREE TIMES - TRY TO REMEMBER THIS!
            // 1) When we have mana and basic timings
            // 2) When we have LAL proc info
            // 3) When we have steady shot speed

            ShotData PrevShot = null;

            for (int i = 0; i < priorities.Length; i++)
            {
                if (priorities[i] == null) continue;
                ShotData s = priorities[i];
                s.calculateTimings(this, PrevShot);

                PrevShot = s;
            }
        }

        public void recalculateRatios()
        {
            // this function is called when we're using the rotation test.
            // we call it once we know the cast time for steady shot, to 
            // get the correct ratio numbers.

            for (int i = 0; i < priorities.Length; i++)
            {
                if (priorities[i] == null) continue;
                priorities[i].finishCalculateTimings(this);
            }
        }

        public void calculateFrequencySums()
        {
            specialShotsPerSecond = 0f;
            critSpecialShotsPerSecond = 0f;
            critsRatioSum = 0f;

            // 31-10-2009 Drizz: Updated to follow 92b
            // I still don't understand the method of how critSpecialShotsPerSecond is calculated.

            for (int i = 0; i < priorities.Length; i++)
            {
                if (priorities[i] == null) continue;
                ShotData s = priorities[i];

                if (s.final_freq > 0) {
                    specialShotsPerSecond += 1f / s.final_freq;

                    if (s.Type == Shots.SerpentSting) { critSpecialShotsPerSecond += 1f / (s.final_freq / s.rotation_cooldown * 3f); }
                    else                              { critSpecialShotsPerSecond += 1f / (s.final_freq                           ); }
                }
                critsRatioSum += s.CritsRatio;
            }
        }

        public void calculateCrits()
        {
            // called after we find out shot crit rates.
            // we calculated frequencies and ratios in earlier calls.

            critsCompositeSum = 0f;

            for (int i = 0; i < priorities.Length; i++)
            {
                if (priorities[i] == null) continue;
                ShotData s = priorities[i];

                s.calculateComposites(this);
                critsCompositeSum += s.CritsComposite;
            }
        }

        public void calculateLALProcs(Character character)
        {
            float lal_trigger_freq = 0f;
            float lal_trigger_duration = 0f;

            ShotData proc_shot = null;
            if (proc_shot == null) proc_shot = getShotInRotation(Shots.BlackArrow);
            if (proc_shot == null) proc_shot = getShotInRotation(Shots.ImmolationTrap);
            if (proc_shot == null) proc_shot = getShotInRotation(Shots.ExplosiveTrap);
            if (proc_shot == null) proc_shot = getShotInRotation(Shots.FreezingTrap);
            if (proc_shot == null) proc_shot = getShotInRotation(Shots.FrostTrap);
            if (proc_shot == null) proc_shot = getShotInRotation(Shots.Volley);

            if (proc_shot != null)
            {
                lal_trigger_freq = proc_shot.lal_freq;
                lal_trigger_duration = proc_shot.Duration;
            }

            float lal_proc_chance = 0.02f * character.HunterTalents.LockAndLoad;
            //lal_proc_chance = 0.06; //isolation testing for LAL
            float lal_proc_freq = 0f;

            if (CalcOpts.UseRotationTest) {
                // the spreadsheet gets the LAL proc frequency from the rotation test,
                // but it doesn't really matter because it's only used to calculate some
                // stats we'll throw away (final_freq via lal_freq).
            } else {
                if (lal_trigger_freq > 0 && lal_proc_chance > 0)
                {
                    lal_proc_freq = (lal_trigger_duration > lal_trigger_freq ? 1f : lal_trigger_freq / lal_trigger_duration) * 3f / lal_proc_chance;
                }
            }

            ShotData lalExplosive = getShotInRotation(Shots.ExplosiveShot);
            ShotData lalArcane = getShotInRotation(Shots.ArcaneShot);

            float pre_lal_explosive_freq = (lalExplosive != null && CalcOpts.LALShotToUse == Shots.ExplosiveShot) ? lalExplosive.inbet_freq : 0;
            float pre_lal_arcane_freq = (lalArcane != null && CalcOpts.LALShotToUse == Shots.ArcaneShot) ? lalArcane.inbet_freq : 0;

            LALExplosiveFrequency = 0;
            if (lal_proc_freq > 0 && pre_lal_explosive_freq > 0) {
                LALExplosiveFrequency = 1f / (1f / (lal_proc_freq / (pre_lal_explosive_freq / (pre_lal_arcane_freq + pre_lal_explosive_freq) * CalcOpts.LALShotsReplaced)) + 1f / pre_lal_explosive_freq);
            }

            LALArcaneFrequency = 0;
            if (lal_proc_freq > 0 && pre_lal_arcane_freq > 0) {
                LALArcaneFrequency = 1f / (1f / (lal_proc_freq / (pre_lal_arcane_freq / (pre_lal_explosive_freq + pre_lal_arcane_freq) * CalcOpts.LALShotsReplaced)) + 1f / pre_lal_arcane_freq);
            }
        }

        public void calculateRotationMPS()
        {
            MPS = 0;
            for (int i = 0; i < priorities.Length; i++)
            {
                if (priorities[i] != null)
                {
                    ShotData s = priorities[i];

                    s.calculateMPS(this);

                    MPS += s.MPS;
                }
            }
        }

        public void calculateRotationDPS()
        {
            bool debug_shot_rotation = false;

            DPS = 0;

            for (int i=0; i<priorities.Length; i++)
            {
                if (priorities[i] != null){
                    ShotData s = priorities[i];

                    s.calculateDPS(this);

                    #region Spreadsheet Calculations

                    // the hidden shot calculations!
                    // G  (RotationCooldown)
                    // H  (Mana)
                    // I  (Damage)
                    // J  (TimeUsed)
                    // K  (Ratio) =IF(OR(AND(ChimeraRefreshesViper,E30="Viper Sting"),AND(ChimeraRefreshesSerpent,E30="Serpent Sting")),"Refreshed",AB30)
                    // N  (MPS) =IF(AND(NOT(UseRotationTest),OR(E30="Explosive Shot",E30="Arcane Shot)),V30,IF(L30="Refreshed","Refreshed",AD30))
                    // O  (StartFreq) =IF(AJ30=TRUE,0,IF(G30>J30,ROUNDUP((G30-J30)/(GCD+CastLag),0)*(GCD+CastLag)+J30,GCD+CastLag))*IF(VLOOKUP(E30,ShotsTable,8,FALSE)<>FALSE,1,0)
                    // P  (GCDLeft)
                    // Q  (GCDNeeded) =IF(AND(NOT(UseKillShot,E30="Kill Shot"),0,IF(OR(AND(ChimeraRefreshesViper,E30="Viper Sting"),AND(ChimeraRefreshesSerpent,E30="Serpent Sting")),0,IF(O30>0,IF(J30>O30,1,J30/O30),0)))
                    // R  (GCDUsed)
                    // S  (InBetFreq) =IF(AND(O30>0,R30>0),J30/R30,0)
                    // T  (InBetStings)
                    // W  (LALFreq) =IF(OR(O30=0,T30=2),0,IF(E30="Explosive Shot",IF(LALExplosiveFrequency>0,LALExplosiveFrequency,S30),IF(E30="Arcane Shot",IF(LALArcaneFrequency>0,LALArcaneFrequency,S30),S30)))
                    // X  (LALGCDLeft)
                    // Y  (LALGCDNeeded) =IF(AND(NOT(UseKillShot),E30="Kill Shot"),0,IF(AND(ChimeraRefreshesSerpent,E30="Serpent Sting"),0,IF(W30>0,IF(J30>W30,1,J30/W30),0)))
                    // Z  (LALGCDUsed)
                    // AA (ActualFreq) =IF(UseRotationTest,'Rotation Test'!D11,IF(AND(W30>0,Z30>0),J3/Z30,0))
                    // AB (ActualRatio) =IF(AND(NOT(UseKillShot),E30="Kill Shot"),0,IF(AND(ChimeraRefereshesSerpent,E30="Serpent Sting"),0,IF(AA30>0,IF(J30>AA30,1,J30/AA30),0)))
                    // AC (ActualDPS) =IF(OR(AND(ChimeraRefreshesViper,E30="Viper Sting"),AND(ChimeraRefreshesSerpent,E30="Serpent Sting")),I30/G30,IF(AA30>0,I30/AA30,0))
                    // AD (ActualMPS) =IF(OR(AND(ChimeraRefreshesViper,E30="Viper Sting"),AND(ChimeraRefreshesSerpent,E30="Serpent Sting")),0,IF(AA30>0,H30/AA30,0))
                    // AJ (InvalidShotPriority) =IF(VLOOKUP(E30,ShotPriorityList,2,FALSE)<>F30,TRUE,OR(IF(AND(E30="Arcane Shot",ExplosiveInRotation),IF(VLOOKUP("Explosive Shot",ShotPriorityList,2,FALSE)<F30,TRUE,FALSE),FALSE,IF(AND(

                    #endregion

                    DPS += s.DPS;

                    if (debug_shot_rotation) {
                        string col1 = String.Format("{0,6:0.00}", s.rotation_cooldown);
                        string col2 = String.Format("{0,3:0}", s.ManaCost);
                        string col3 = String.Format("{0,4:0}", s.Damage);
                        string col4 = String.Format("{0:0.00}", s.time_used);
                        string col5 = String.Format("{0,6:0.00}%", 100*s.ratio);
                        string col6 = String.Format("{0,6:0.00}", s.Freq);
                        string col7 = String.Format("{0,6:0.00}", s.DPS);
                        string col8 = String.Format("{0,6:0.00}", s.MPS);

                        Debug.WriteLine("Shot: |" + col1 + "|" + col2 + "|" + col3 + "|" + col4 + "|" + col5 + "|" + col6 + "|" + col7 + "|" + col8 + "|");
                    }
                } else {
                    if (debug_shot_rotation) {
                        Debug.WriteLine("None: |------|---|----|----|-------|------|  0.00|  0.00|");
                    }
                }
            }

            if (debug_shot_rotation) {
                string debug_dps = String.Format("{0,6:0.00}", DPS);
                string debug_mps = String.Format("{0,6:0.00}", MPS);

                Debug.WriteLine("TOTAL:                                    |"+debug_dps+"|"+debug_mps+"|");
            }
        }
    }
}
