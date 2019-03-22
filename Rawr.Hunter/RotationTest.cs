using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Rawr.Hunter
{
    class RotationTest
    {
        // passed in by the caller
        private Character character;
        private CharacterCalculationsHunter calculatedStats;
        private CalculationOptionsHunter CalcOpts;
        private BossOptions BossOpts;

        // used for calculation
        private Dictionary<Shots, RotationShotInfo> shotData;
        private RotationShotInfo[] shotTable;
        private Random rand = new Random();

        // the output
        public float TestTimeElapsed = 0;
        public int LALProcCount = 0;
        public int ISSProcsAimed = 0;
        public int ISSProcsArcane = 0;
        public int ISSProcsChimera = 0;
        public float ISSAimedUptime = 0;
        public float ISSArcaneUptime = 0;
        public float ISSChimeraUptime = 0;
        public int IAotHProcs = 0;
        public float IAotHTime = 0;
        public float IAotHUptime = 0;
 
        public RotationTest(Character character, CharacterCalculationsHunter calculatedStats, CalculationOptionsHunter calcOpts, BossOptions bossOpts)
        {
            this.character = character;
            this.CalcOpts = calcOpts;
            this.BossOpts = bossOpts;
            this.calculatedStats = calculatedStats;
            // 091109 Drizz: Added
            this.calculatedStats.sequence = "Time  :Shot          :CastTime:Done   :CD Until" + Environment.NewLine;
        }

        // 091109 Drizz: Added to be able to pull infor from "outside"
        public RotationShotInfo[] getRotationTable()
        {
            return shotTable;
        }

        public void RunTest()
        {
#if RAWR3 || SILVERLIGHT
            float FightDuration = BossOpts.BerserkTimer;
#else
            float FightDuration = CalcOpts.Duration;
#endif
            float Latency = CalcOpts.Latency;
            float CDCutoff = CalcOpts.CDCutoff;
            float Longevity = character.HunterTalents.Longevity;
            float GCD = 1.5f;
            float BA = calculatedStats.blackArrow.Duration;
            float it = calculatedStats.immolationTrap.Duration;
            float et = calculatedStats.explosiveTrap.Duration;
            float ft = calculatedStats.freezingTrap.Duration;
            float frt = calculatedStats.frostTrap.Duration;
            float vly = calculatedStats.volley.Duration;
            float LALChance = character.HunterTalents.BlackArrow == 1 ? character.HunterTalents.LockAndLoad * CalcOpts.LALProcChance : -1;
            bool RandomProcs = CalcOpts.RandomizeProcs;
            int ISSfix = 0;
            int IAotHfix = 0;
            int LALfix = 0;
            float AutoShotSpeed = 2; // Speed at which we shoot auto-shots
            //float IAotHEffect = 1 + calculatedStats.quickShotsEffect; // haste increase during IAoTH
            float IAotHChance = character.HunterTalents.ImprovedAspectOfTheHawk > 0 ? 10 : -1;
            float WaitForESCS = CalcOpts.waitForCooldown;
            bool InterleaveLAL = CalcOpts.interleaveLAL;
            bool ArcAimedPrio = CalcOpts.prioritiseArcAimedOverSteady;
            float ISSTalent = character.HunterTalents.ImprovedSteadyShot * 5;
            float ISSDuration = -1;
            int ISSprocsAimed = 0;
            int ISSProcsArcane = 0;
            int ISSProcsChimera = 0;
            float BossHPPercentage = CalcOpts.BossHPPerc * 100;
#if RAWR3 || SILVERLIGHT
            float Sub20Time = (BossHPPercentage > 20) ? FightDuration - (float)BossOpts.Under20Perc * FightDuration : 0; // time *until* we hit sub-20
#else
            float Sub20Time = (BossHPPercentage > 20) ? FightDuration - CalcOpts.TimeSpentSub20 : 0; // time *until* we hit sub-20
#endif
            bool UseKillShot = calculatedStats.priorityRotation.containsShot(Shots.KillShot);
            bool BMSpec = character.HunterTalents.BestialWrath + character.HunterTalents.TheBeastWithin > 0;

            if (CalcOpts.PetFamily == PetFamily.None) BMSpec = false;

            int currentShot;
            float currentTime;
            //int selectedShot;
            float ElapsedTime;
            float BWTime;
            float RFTime;
            float VolleyTime;
            float BWCD;
            float RFCD;
            float VLYCD;
            float LALTimer;      // timer till current serpent sting expires
            float LALShots;      // amount of free shots left
            float LALDuration;   // time since last Explosive Shot while having L&L up
            float LastLALCheck;  // time since last check for proc
            float LastLALProc;   // time since last proc
            int LALProcs;      // Amount of procs
            Shots LALType;       // Type of L&L proc, i.e. Immolation trap or Black Arrow
            float IAotHDuration; // Time until buff expires
            float LastAutoShotCheck; // Last time we checked for IAotH procs
            int IAotHProcs;
            bool SerpentUsed;  // Have we used Serpent Sting yet?

            #region Initialize shotData & shotTable
            shotData = new Dictionary<Shots, RotationShotInfo>(); // the shot info (mana, cooldown, etc)
            shotTable = new RotationShotInfo[10]; // the rotation info
            
            for (int i=0; i<calculatedStats.priorityRotation.priorities.Length; i++) {
                ShotData shot = calculatedStats.priorityRotation.priorities[i];
                if (shot != null && shot.Type != Shots.None) {
                    shotData[shot.Type] = new RotationShotInfo(shot);
                    shotTable[i] = shotData[shot.Type];
                } else { shotTable[i] = null; }
            }
            #endregion
            #region Shot Timing Adjustments

            // we need to adjust rapid fire cooldown, since we accounted for readiness initially
            if (shotData.ContainsKey(Shots.RapidFire)) {
                shotData[Shots.RapidFire].cooldown = (5 - character.HunterTalents.RapidKilling) * 60f;
            }

            // set Steady Shot cast time so it's only static haste
            if (shotData.ContainsKey(Shots.SteadyShot)) {
                shotData[Shots.SteadyShot].castTime = 2f / (1f + calculatedStats.hasteStaticTotal);
            }

            // set Steady Shot cast time so it's only static haste
            if (shotData.ContainsKey(Shots.Volley)) {
                shotData[Shots.Volley].castTime = 6f;
            }

            // Set Auto Shot Speed to statically hasted value
            AutoShotSpeed = calculatedStats.autoShotStaticSpeed;

            #endregion
            #region Variable setup

            // these are used by both the frequency rotation and here in the rotation test
            bool chimeraRefreshesSerpent = containsShot(Shots.ChimearaShot) && containsShot(Shots.SerpentSting);

            currentShot = 1;
            currentTime = 0;
            ElapsedTime = 0;
    
            //Lock And Load variables
            LALTimer = -1;
            LALShots = -1;
            LALDuration = -1;
            LastLALCheck = -1;
            LastLALProc = -50;
            LALType = Shots.None;

            //IAotH variables
            // 021109 Drizz: Updating init variables to align with v92b spreadsheet
            LastAutoShotCheck = 0; // -50;
            IAotHDuration = 0;// -1;
            IAotHProcs = 0;

            SerpentUsed = false;

            // some things not initialized in the spreadsheet
            BWTime = 0;
            VolleyTime = 0;
            RFTime = 0;
            BWCD = 0;
            RFCD = 0;
            VLYCD = 0;
            currentTime = 0;
            LALProcs = 0;
            float IAotHUptime = 0;

            #endregion
            #region The Main Loop
            do {
                bool haveShot = false;
                Shots thisShot = Shots.None;

                #region Find shot - Non-GCD

                // Check shots that are off the GCD
                for (int i=0; i<shotTable.Length && !haveShot; i++)
                {
                    RotationShotInfo s = shotTable[i];
                    if (s != null)
                    {
                        if (s.type == Shots.BestialWrath && BWTime > currentTime) {
                            // do nothing if Rapid Fire is still active
                        } else if (s.type == Shots.RapidFire && RFTime > currentTime) {
                            // Check if TBW/BW CD is nearly up if we want to use Rapid Fire
                        } else if (BMSpec && s.type == Shots.Readiness && BWCD < (currentTime + CDCutoff)) {
                            // Wait for Rapid Fire if it's close too
                        } else if (s.type == Shots.Readiness && RFCD < (currentTime + CDCutoff)) {
                            // wait for Rapid Fire
                        } else if (s.type == Shots.BestialWrath && !BMSpec) {
                            // do nothing
                        //} else if (s.type == Shots.Volley && VLYCD < (currentTime + CDCutoff)) {
                            // do nothing, we're channelling
                        } else {
                            // check if the shot is off the GCD
                            if (s.no_gcd && s.time_until_off_cd <= currentTime) {
                                thisShot = s.type;
                                haveShot = true;
                            }
                        }
                    }
                }

                #endregion
                #region Find Shot - Any

                // Check all shots whether they are off CD
                for (int i=0; i<shotTable.Length && !haveShot; i++)
                {
                    RotationShotInfo s = shotTable[i]; // known as thisShot
                    if (s != null) {            
                        // During L&L proc Explosive Shot gets priority over everything else
                        // Do nothing if L&L just proc'ed and ES is still on CD
                        if (s.time_until_off_cd > currentTime && LALShots == 3 && s.type == Shots.ExplosiveShot)
                        {
                        }
                        //  First L&L proc shot
                        else if (s.time_until_off_cd < currentTime && LALShots == 3 && s.type == Shots.ExplosiveShot)
                        {
                            s.time_until_off_cd = 0;
                            LALShots--;
                            haveShot = true;
                            thisShot = s.type;
                            // Cells(row + 1, col + 5).value = Cells(row + 1, col + 5).value + "L&L #1"
                        }
                        // If We're interleaving, make sure the last ES was at least 2 seconds before this
                        else if (InterleaveLAL && LALShots < 3 && LALShots > 0 && LALDuration - currentTime > -2 && LALDuration - currentTime <= 0 && s.type == Shots.ExplosiveShot)
                        {
                            currentTime += 0.5f;
                            s.time_until_off_cd = 0;
                            haveShot = true;
                            thisShot = s.type;
                            if (LALShots == 2)
                            {
                                // Cells(row + 1, col + 5).value = Cells(row + 1, col + 5).value + "L&L #2"
                            }
                            else if (LALShots == 1)
                            {
                                // Cells(row + 1, col + 5).value = Cells(row + 1, col + 5).value + "L&L #3"
                            }                
                            LALShots--;
                        }
                        // If we're not interleaving other shots with L&L procs reset ES cooldown and advance timer by 0.5 seconds (to miss the last tick)
                        else if (!InterleaveLAL && LALShots < 3 && LALShots > 0 && s.type == Shots.ExplosiveShot)
                        {
                            // ******************************************************************************
                            // 29-10-2009 - Drizz: Commenting out this line to align with Spreadsheet (v92b)
                            // currentTime += 0.5;
                            // ******************************************************************************
                            s.time_until_off_cd = 0;
                            haveShot = true;
                            thisShot = s.type;
                
                            if (LALShots == 2)
                            {
                                // Cells(row + 1, col + 5).value = Cells(row + 1, col + 5).value + "L&L #2"
                            }
                            else if (LALShots == 1)
                            {
                                // Cells(row + 1, col + 5).value =  Cells(row + 1, col + 5).value + "L&L #3"
                            }
                            LALShots--;
                        }
            
                        float waitTime = (float)Math.Round(s.time_until_off_cd - currentTime, 2);

                        // Check if shots with CDs come off CD soon enough to wait for them, excluding stings
                        if (!haveShot && waitTime > 0 && s.time_until_off_cd > currentTime && s.time_until_off_cd <= currentTime + WaitForESCS
                                && s.type != Shots.SerpentSting && s.type != Shots.BlackArrow)
                        {
                            if (!ArcAimedPrio && (thisShot == Shots.AimedShot || thisShot == Shots.ArcaneShot || thisShot == Shots.MultiShot))
                            {
                                // do nothing if we don't want to prioritise Aimed/Arcane or Multi-Shot
                            }
                        // ************************************************************************
                        //29-10-2009 Drizz : Added one else if check to align with Spreadsheet v92b
                            else if (s.type==Shots.Readiness && RFCD < (currentTime+CDCutoff+waitTime))
                            {
                                // Do nothing if we Rapid Fire isn't soon enough of CD
                            }
                        // ************************************************************************
                            else if (s.check_gcd)
                            {
                                bool result = true;

                                // Check if other shots that are about to come off CD do more damage than this one
                                for (int j=i; j<shotTable.Length && result; j++)
                                {
                                    RotationShotInfo thatShot = shotTable[j];
                                    if (thatShot != null)
                                    {
                                        // 021109 Drizz: Removed the last && on this line (align with Worksheet 92b.
                                        if (thatShot.time_until_off_cd > currentTime && thatShot.time_until_off_cd <= currentTime + WaitForESCS) // && checkGCD(thisShot))
                                        {
                                            if (compareDamage(thisShot, thatShot.type, waitTime) != thisShot)
                                            {
                                                result = false;
                                                // Cells(row + 1, col + 5).value = Cells(row + 1, col + 5).value + "(Skipped " & thisShot & ") "
                                                //exit for loop
                                            }
                                        }
                                    }
                                }

                                if (result)
                                {
                                    currentTime = s.time_until_off_cd;
                                    thisShot = s.type;
                                    haveShot = true;
                                    // Cells(row + 1, col + 5).value = Cells(row + 1, col + 5).value + "(Waited " & waitTime & "s) "
                                }
                            }
                        }

                        // this is a horrible round hack, i know. the issue probably comes
                        // somewhere from a float->float cast.
                        // the issue is that we have currentTime at (e.g.) 65.35 and steady shot CD is at 65.3500000001
                        // so we skip forward by 0.1 seconds when we didn;t really need to.
                        if (!haveShot && s.time_until_off_cd <= currentTime+0.00001) {
                            if (s.type == Shots.SerpentSting && chimeraRefreshesSerpent && SerpentUsed) {
                                // do nothing for refreshed Serpent Sting except the first time
                            } else if (s.type == Shots.KillShot && (!UseKillShot || currentTime < Sub20Time)) {
                                // Do not use Kill Shot if Boss HP is above 20%
                            } else if (s.type == Shots.Volley && VolleyTime > currentTime) {
                                // do nothing if Volley is still active
                            } else if (s.type == Shots.BestialWrath && BWTime > currentTime) {
                                // do nothing if TBW or BW is still active
                            } else if (s.type == Shots.RapidFire && RFTime > currentTime) {
                                // do nothing if Rapid Fire is still active
                            } else if (BMSpec && s.type == Shots.Readiness && BWCD < (currentTime + CDCutoff)) {
                                // Check if TBW/BW CD is nearly up if we want to use Rapid Fire
                            } else if (s.type == Shots.Readiness &&  RFCD < (currentTime + CDCutoff)) {
                                // Wait for Rapid Fire if it's close too
                            } else if (s.type == Shots.SerpentSting && FightDuration - currentTime < s.cooldown) {
                                // do nothing if Rapid Fire is about to come off CD
                                // do nothing if we don't get all the ticks of Serpent Sting anyway
                            } else if (s.type == Shots.BlackArrow && FightDuration - currentTime < BA) {
                                // ditto for Black Arrow
                            } else if (s.type == Shots.BestialWrath && !BMSpec) {
                                // do nothing
                            } else {
                                thisShot = s.type;
                                haveShot = true;
                            }
                        }                    
                    }
                }

                #endregion
                #region Execute shot

                if (!haveShot)
                {
                    // If no shot, advance 100 ms
                    // This case should almost never happen unless Steady Shot isn't in the rotation
                    currentTime += 0.1f;
                    //Debug.WriteLine("no shot found - advancing time to "+currentTime);
                }
                else
                {
                    float currentAutoShotSpeed;
            
                    currentAutoShotSpeed = AutoShotSpeed;
                    RotationShotInfo thisShotInfo = shotData[thisShot];

                    if (thisShotInfo.type == Shots.SerpentSting && !SerpentUsed)
                    {
                        SerpentUsed = true;
                    }

                    // Adjust AutoShot Speed if Rapid Fire is active
                    if (RFTime > currentTime)
                    {
                        currentAutoShotSpeed *= 0.6f;
                    }

                    //  Adjust AutoShotSpeed if IAotH is active
                    /*if (IAotHDuration - currentTime > 0)
                    {
                        currentAutoShotSpeed *= (1f / IAotHEffect);
                    }*/

                    // Check if we had an auto shot since the last check
                    if (currentTime - LastAutoShotCheck > currentAutoShotSpeed)
                    {
                    // *******************************************************
                    // 021109 Drizz: Added if around LastAutoShotCheck, Updated from v92b (spreadsheet)
                        if (currentTime == 0)
                        LastAutoShotCheck = currentTime;
                        else
                            LastAutoShotCheck += currentAutoShotSpeed;
                    // *******************************************************
                    
                    // 021109 Drizz: Updating to use "LastAutoShotCheck" instead of currentTime (Align with v92b)
                        if (RandomProcs)
                        {
                            // 10% proc chance                    
                            if (randomProc(IAotHChance))
                            {
                                if (IAotHDuration - LastAutoShotCheck < 0)
                                {
                                        // No IAotH proc active
                                        IAotHUptime = IAotHUptime + 12;
                                }
                                else
                                {
                                    IAotHUptime = IAotHUptime - (IAotHDuration - LastAutoShotCheck) + 12;
                                }
                                IAotHDuration = LastAutoShotCheck + 12;
                                IAotHProcs = IAotHProcs + 1;
                            }
                        }
                        else if (IAotHChance > 0)
                        {
                            IAotHfix = IAotHfix + 1;
                            if (IAotHfix * IAotHChance > 100)
                            {
                                IAotHfix = 0; // reset counter
                                if (IAotHDuration - LastAutoShotCheck < 0)
                                {
                                    // No IAotH proc active
                                    IAotHUptime = IAotHUptime + 12;
                                }
                                else
                                {
                                    IAotHUptime = IAotHUptime - (IAotHDuration - LastAutoShotCheck) + 12;
                                }
                                IAotHDuration = LastAutoShotCheck + 12;
                                IAotHProcs = IAotHProcs + 1;
                            }
                        }
                    }
                

                    // count shots
                    currentShot++;
            
                    // advance row on the test page
                    //row++

                    thisShotInfo.countUsed++;

                    // Determine Steady Shot haste
                    float SShaste = 1f;
                    if (RFTime > currentTime) SShaste *= 1.4f;
                    //if (IAotHDuration > currentTime) SShaste *= IAotHEffect;


                    // Increment cooldown on the shot just fired
                    if (thisShot == Shots.SteadyShot && thisShotInfo.castTime * (1f / SShaste) < GCD + Latency)
                    {
                        thisShotInfo.time_until_off_cd = currentTime + GCD + Latency;
                    }
                    else if (thisShot == Shots.SteadyShot)
                    {
                        thisShotInfo.time_until_off_cd = currentTime + thisShotInfo.castTime * (1f / SShaste) + Latency;
                    }
                    else if (thisShot == Shots.Volley)
                    {
                        thisShotInfo.time_until_off_cd = currentTime + Math.Max(thisShotInfo.castTime, thisShotInfo.cooldown) + Latency;
                    }
                    else if (checkGCD(thisShot))
                    {
                        thisShotInfo.time_until_off_cd = currentTime + thisShotInfo.cooldown + Latency;
                    }
                    else
                    {
                        // Shots that are off the GCD do not incur latency
                        thisShotInfo.time_until_off_cd = currentTime + thisShotInfo.cooldown;
                    }


                    // Note down shot current time and shot used
                    // 091109 Drizz: If we now can get the debug info within Rawr, no need to go by debug option.
                    if (calculatedStats.collectSequence) 
                    {
                        float timeUsed = 0;
                        float castEnd = currentTime;
                        float onCDUntil = currentTime + thisShotInfo.cooldown;

                        // Note down cast time used, cast end time and time till CD
                        if (!checkGCD(thisShot))
                        {
                            // These do not cost time
                            // (we set these values just above)
                        }
                        else
                        {
                            // Steady Shot can fire faster than GCD so check
                            if (thisShot == Shots.SteadyShot)
                            {
                                timeUsed = thisShotInfo.castTime * (1 / SShaste) + Latency;
                                castEnd = currentTime + thisShotInfo.castTime * (1 / SShaste) + Latency;
                            }
                            else if (thisShot == Shots.Volley)
                            {
                                timeUsed = thisShotInfo.castTime + Latency;
                                castEnd = currentTime + thisShotInfo.castTime + Latency;
                            }
                            else
                            {
                                // Other shots fire at GCD + Latency
                                timeUsed = GCD + Latency;
                                castEnd = currentTime + GCD + Latency;
                            }
                            onCDUntil = thisShotInfo.time_until_off_cd;
                        }
                        // 091109 Drizz: Removing the Debug.Writeline
                        //Debug.WriteLine(" "+ currentTime + ": " + thisShot + " (" +timeUsed+"/"+castEnd+"/"+onCDUntil + ")");
                        calculatedStats.sequence += String.Format("{0,6:0.00}:{1,-13}" + " : {2,7:0.000}:{3,7:0.000}:{4,7:0.000}", currentTime, thisShot, timeUsed, castEnd, onCDUntil) + Environment.NewLine;
                        #if !RAWR3 && !SILVERLIGHT
                        Debug.Flush();
                        #endif
                    }

                    // Set L&L Timer after Black Arrow/Immolation Trap was used
                    if (thisShot == Shots.BlackArrow
                        || thisShot == Shots.ImmolationTrap
                        || thisShot == Shots.ExplosiveTrap
                        || thisShot == Shots.FreezingTrap
                        || thisShot == Shots.FrostTrap)
                    {
                        switch (thisShot) {
                            case Shots.BlackArrow: { LALTimer = currentTime + BA; break; }
                            case Shots.ImmolationTrap: { LALTimer = currentTime + it; break; }
                            case Shots.ExplosiveTrap: { LALTimer = currentTime + et; break; }
                            case Shots.FreezingTrap: { LALTimer = currentTime + ft; break; }
                            default: { LALTimer = currentTime + frt; break; } // Shots.FrostTrap
                        }
                        LALType = thisShot;
                        LastLALCheck = currentTime - 3;
                    }

                    
                    if (currentTime + thisShotInfo.cooldown > ElapsedTime) {
                        if (thisShotInfo.cooldown < GCD + Latency) {
                            ElapsedTime = currentTime + GCD + Latency;
                        } else {
                            ElapsedTime = currentTime + thisShotInfo.cooldown;
                        }
                    }

                    // If we have Improved Steady Shot and have just cast Steady Shot
                    if (ISSTalent > 0 && thisShot == Shots.SteadyShot) {
                        if (RandomProcs) {
                            if (randomProc(ISSTalent)) {
                                ISSDuration = currentTime + 12;
                            }
                        } else if (ISSTalent > 0) {
                            ISSfix++;
                            if (ISSfix * ISSTalent > 100) {
                                ISSfix = 0;
                                ISSDuration = currentTime + 12;
                            }
                        }
                    }

                    if (ISSDuration > currentTime) {
                        if (thisShot == Shots.AimedShot) {
                            ISSprocsAimed++;
                            ISSDuration = -1;
                            //Cells(row, col + 5).value = Cells(row, col + 5).value + "(ISS proc)"
                        } else if (thisShot == Shots.ArcaneShot) {
                            ISSProcsArcane++;
                            ISSDuration = -1;
                            //Cells(row, col + 5).value = Cells(row, col + 5).value + "(ISS proc)"
                        } else if (thisShot == Shots.ChimearaShot) {
                            ISSProcsChimera++;
                            ISSDuration = -1;
                            //Cells(row, col + 5).value = Cells(row, col + 5).value + "(ISS proc)"
                        }
                    }

                    // If we used Explosive Shot, set the duration of the debuff
                    if (thisShot == Shots.ExplosiveShot && LALShots > 0)
                    {
                        LALDuration = currentTime + 2;
                    }

                    // If we used Explosive Shot, set Arcane Shot on CD as well and likewise
                    if (thisShot == Shots.ArcaneShot && shotData.ContainsKey(Shots.ExplosiveShot)) {
                        shotData[Shots.ExplosiveShot].time_until_off_cd = currentTime + shotData[Shots.ExplosiveShot].cooldown + Latency;
                    }
                    if (thisShot == Shots.ExplosiveShot && shotData.ContainsKey(Shots.ArcaneShot)) {
                        shotData[Shots.ArcaneShot].time_until_off_cd = currentTime + shotData[Shots.ArcaneShot].cooldown + Latency;
                    }

                    // If we used Multi-Shot set Aimed Shot cooldown as well and vice-versa
                    if (thisShot == Shots.MultiShot && shotData.ContainsKey(Shots.AimedShot)) {
                        shotData[Shots.AimedShot].time_until_off_cd = currentTime + shotData[Shots.AimedShot].cooldown + Latency;
                    }
                    if (thisShot == Shots.AimedShot && shotData.ContainsKey(Shots.MultiShot)) {
                        shotData[Shots.MultiShot].time_until_off_cd = currentTime + shotData[Shots.MultiShot].cooldown + Latency;
                    }

                    // If Black Arrow is active (LALTimer) do check a proc every 3 seconds and if no LAL proc has occured within the last 22 seconds
                    if (LALTimer > currentTime && currentTime - LastLALCheck > 3 && LALDuration < currentTime && currentTime - LastLALProc > 22)
                    {
                        if (RandomProcs)
                        {
                            // And currentTime - LastLALProc > 30 Then --- currently no ICD                            
                            if (randomProc(LALChance))
                            {
                                //L&L procs
                                LALShots = 3;
                                LastLALProc = currentTime;
                                LALProcs++;
                                //Cells(row, col + 5).value = Cells(row, col + 5).value + "(Proc L&L after this shot)"
                            }
                        }
                        else if (LALChance > 0)
                        {
                            LALfix++;
                            if (LALfix * LALChance > 100)
                            {
                                //L&L procs
                                LALShots = 3;
                                LastLALProc = currentTime;
                                LALProcs++;
                                //Cells(row, col + 5).value = /Cells(row, col + 5).value + "(Proc L&L after this shot)"
                                LALfix = 0;
                            }
                        }
                        LastLALCheck = currentTime;
                    }

                    // If we used Beast Within, set duration to 18 seconds
                    if (thisShot == Shots.BestialWrath)
                    {
                        BWTime = currentTime + 10;
                        // also record the time when it comes off CD
                        BWCD = thisShotInfo.time_until_off_cd;
                    }

                    // If we used Volley, set duration to 6 seconds
                    if (thisShot == Shots.Volley)
                    {
                        VolleyTime = currentTime + thisShotInfo.castTime;
                        // also record the time when it comes off CD
                        VLYCD = thisShotInfo.time_until_off_cd;
                    }

                    if (thisShot == Shots.RapidFire)
                    {
                        RFTime = currentTime + 15;
                        // also record the time when it comes off CD
                        RFCD = thisShotInfo.time_until_off_cd;
                    }

                    // If Readiness is used, reset CD on other shots
                    if (thisShot == Shots.Readiness)
                    {
                        // Reset everything but Readiness and Serpent Sting and TBW
                        for (int i = 0; i < shotTable.Length; i++)
                        {
                            RotationShotInfo s = shotTable[i];
                            if (s != null)
                            {
                                if (s.type != Shots.Readiness
                                    && s.type != Shots.SerpentSting
                                    && s.type != Shots.BestialWrath
                                    && s.type != Shots.Volley)
                                {
                                    s.time_until_off_cd = 0;
                                }
                            }
                        }
                    }

                    if (LALTimer > currentTime)
                    {
                        //Cells(row, col + 5).value = "(" & LALType & ") " + Cells(row, col + 5).value
                    }

                    // Advance time by 0 if the shot is off the GCD, otherwise by GCD+Latency
                    if (!checkGCD(thisShot)) {
                        // do nothing
                    } else if (thisShot == Shots.SteadyShot && thisShotInfo.castTime * (1f / SShaste) < GCD + Latency) {
                        currentTime += GCD + Latency;
                    } else if (thisShot == Shots.SteadyShot) {
                        currentTime += thisShotInfo.castTime * (1f / SShaste) + Latency;
                    } else if (thisShot == Shots.Volley) {
                        currentTime += thisShotInfo.castTime + Latency;
                    } else {
                        currentTime += GCD + Latency;
                    }
                    // more here...
                }
                #endregion
            }
            while (currentTime < FightDuration - 1);

            #endregion
            #region Save Results

            foreach (var pair in shotData)
            {
                pair.Value.frequency = pair.Value.countUsed > 0 ? currentTime / pair.Value.countUsed : 0;
            }

            for (int i = 0; i < calculatedStats.priorityRotation.priorities.Length; i++)
            {
                ShotData s = calculatedStats.priorityRotation.priorities[i];
                if (s != null)
                {
                    s.setFrequency(calculatedStats.priorityRotation, shotData.ContainsKey(s.Type) ? shotData[s.Type].frequency : 0);
                }
            }

            this.TestTimeElapsed = currentTime;
            this.ISSProcsAimed = ISSprocsAimed;
            this.ISSProcsArcane = ISSProcsArcane;
            this.ISSProcsChimera = ISSProcsChimera;
            this.IAotHProcs = IAotHProcs;
            this.IAotHTime = IAotHUptime;
            this.LALProcCount = LALProcs;

            this.IAotHUptime      = this.TestTimeElapsed > 0 ? 1.0f * this.IAotHTime / this.TestTimeElapsed : 0;
            this.ISSAimedUptime   = this.ISSProcsAimed   > 0 ? 1.0f * this.ISSProcsAimed / shotData[Shots.AimedShot].countUsed : 0;
            this.ISSArcaneUptime  = this.ISSProcsArcane  > 0 ? 1.0f * this.ISSProcsArcane / shotData[Shots.ArcaneShot].countUsed : 0;
            this.ISSChimeraUptime = this.ISSProcsChimera > 0 ? 1.0f * this.ISSProcsChimera / shotData[Shots.ChimearaShot].countUsed : 0;

            #endregion
            /*
                ActiveWorkbook.Names.Add name:="RotationTestResultNames", RefersToR1C1:= _
                        "='Rotation Test'!R5C7:R" & row & "C7"
                ActiveWorkbook.Names.Add name:="RotationTestTable", RefersToR1C1:= _
                        "='Rotation Test'!R5C6:R" & row & "C" & col + 5
            */
        }

        private bool randomProc(float chance) {
            if (chance < 0) return false;
            // 021109 Drizz: If I understand the rand.Next function correctly it is always below the 2nd parameter so (0,100) is what we want.
            //return rand.Next(0,99) < chance;
            return rand.Next(0, 100) < chance;
        }

        private bool containsShot(Shots shot)
        {
            for (int i = 0; i < shotTable.Length; i++)
            {
                if (shotTable[i] != null && shotTable[i].type == shot) return true;
            }
            return false;
        }

        private bool checkGCD(Shots shot)
        {
            if (shotData.ContainsKey(shot)) return shotData[shot].check_gcd;
            return true;
        }

        private Shots compareDamage(Shots shot1, Shots shot2, float time)
        {
            // This function determines whether waiting for {shot1} for {time} seconds produces more dps than using {shot2} right now

            if (shot1 == Shots.None) return Shots.None;
            if (shot2 == Shots.None) return Shots.None;
            if (time <= 0) return Shots.None;

            float shot1damage = shotData.ContainsKey(shot1) ? shotData[shot1].damage : 0;
            float shot2damage = shotData.ContainsKey(shot2) ? shotData[shot2].damage : 0;

            float frac = (float)Math.Round(time / 1.5f, 3);

            if (shot1damage - shot2damage > shot2damage * frac) return shot1;
            return shot2;
        }
    }

    class RotationShotInfo
    {
        public bool no_gcd = false;             // noGCD()
        public bool check_gcd = true;           // CheckGCD()

        public Shots type;                      // shotTable 1
        public float time_until_off_cd = -1;   // shotTable 2
        public float cooldown = 0;             // shotTable 3
        public float unknownStat4 = -1;        // shotTable 4
        public float unknownStat5 = -1;        // shotTable 5
        public float castTime = 0;             // shotTable 6

        public float damage = 0;               // shotData 3

        public float countUsed = 0;
        public float frequency = 0;
        
        public RotationShotInfo(ShotData shot)
        {
            this.type = shot.Type;
            this.cooldown = shot.Cd;
            this.castTime = shot.Cd;
            this.check_gcd = shot.TriggersGCD;
            this.no_gcd = !shot.TriggersGCD && shot.Type != Shots.Readiness;
            this.damage = shot.Damage;
            if (shot.Type == Shots.SerpentSting) this.cooldown = shot.Duration;
        }
    }
/*
    class RotationShotInstance
    {
        public float startCast;
        public Shots shot;
        public float timeUsed;
        public float castEnd;
        public float onCooldownUntil;
        // these flags all show up in 1 column on the SS
        public bool flagBA;
    }
*/ 
}
