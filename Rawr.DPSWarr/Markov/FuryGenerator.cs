//#define TRANSTYPE
using System;
using System.Collections.Generic;
using Rawr.Base.Algorithms;
using System.Text;
using Rawr.Base;
using System.IO;

namespace Rawr.DPSWarr.Markov
{
    class FuryGenerator : StateSpaceGenerator<Skills.Ability>
    {
        const int NUM_DEC = 3;
        const string FORMAT = "0.000";
        public FuryGenerator(Character c, Stats s, CombatFactors cf, Skills.WhiteAttacks wa, CalculationOptionsDPSWarr co, BossOptions bo)
        {
            WarriorTalents Talents = c.WarriorTalents;
            //WarriorTalentsCata TalentsCata = c.WarriorTalentsCata; 
            //
            Rot = new FuryRotation(c, s, cf, wa, co, bo);
            Rot.Initialize();
            LatentGCD = Math.Round(1.5 + co.FullLatency, NUM_DEC);
            HastedGCD = Math.Round(1.0 + co.FullLatency, NUM_DEC);
            
            HSPerc = .8;
            
            BS1Proc = c.WarriorTalents.Bloodsurge / 15f; // 7/13/20%
            BS2Proc = 0f;
            
            if (s.BonusWarrior_T10_4P_BSSDProcChange > 0)
            {
                BS2Proc = 0.2;
            }

            BT = Rot.GetWrapper<Skills.BloodThirst>().ability;
            WW = Rot.GetWrapper<Skills.WhirlWind>().ability;
            BS = Rot.GetWrapper<Skills.BloodSurge>().ability;
            HS = Rot.GetWrapper<Skills.HeroicStrike>().ability;
            WhiteInterval = wa.MhEffectiveSpeed;

            SetChances();
        }

        #region Variables
        protected double LatentGCD;
        protected double HastedGCD;
        public double HSPerc;
        
        public FuryRotation Rot = null;

        protected Skills.Ability BT, WW, BS, HS;

        protected double BS1Proc;
        protected double BS2Proc;

        protected double HSProcChance;

        protected BloodSurgeMatrix /*HSMatrix,*/ WWMatrix, BTMatrix;

        protected double WhiteInterval;
        #endregion

        private void SetChances()
        {
            // HS
            //HSMatrix = new BloodSurgeMatrix(BS1Proc, BS2Proc);
            //HSMatrix.AddAbility(HS.MHAtkTable.AnyLand);

            // WW
            WWMatrix = new BloodSurgeMatrix(BS1Proc, BS2Proc);
            WWMatrix.AddAbility(WW.MHAtkTable.AnyLand);
            WWMatrix.AddAbility(WW.OHAtkTable.AnyLand);
            
            // BT
            BTMatrix = new BloodSurgeMatrix(BS1Proc, BS2Proc);
            BTMatrix.AddAbility(BT.MHAtkTable.AnyLand);

            HSProcChance = HS.MHAtkTable.AnyLand * BS1Proc;
        }

        public class StateFury : State<Skills.Ability>
        {
            public double BTCooldown;
            public double WWCooldown;
            public int BSProcced;
            public bool BSHasted;
#if TRANSTYPE
            public int transitionType;
#endif
        }

        public class DelegateStateTransition : StateTransition<Skills.Ability>
        {
            public delegate double newTransitionChance();
            
            public newTransitionChance myDel { get; set; }
            public override double TransitionProbability
            {
                get
                {
                    return myDel();
                }
            }
        }

        protected override State<Skills.Ability> GetInitialState()
        {
            return GetState(0, false, 0, 0, 0);
        }

        protected override List<StateTransition<Skills.Ability>> GetStateTransitions(State<Skills.Ability> state)
        {
            StateFury s = state as StateFury;
            List<StateTransition<Skills.Ability>> list = new List<StateTransition<Skills.Ability>>();

            Rawr.DPSWarr.Rotation.AbilWrapper BT = Rot.GetWrapper<Skills.BloodThirst>();
            Rawr.DPSWarr.Rotation.AbilWrapper WW = Rot.GetWrapper<Skills.WhirlWind>();
            Rawr.DPSWarr.Rotation.AbilWrapper BS = Rot.GetWrapper<Skills.BloodSurge>();
            Rawr.DPSWarr.Rotation.AbilWrapper HS = Rot.GetWrapper<Skills.HeroicStrike>();

            /*bool forceDelay = false;
            if (s.WWCooldown != 0 && s.BTCooldown != 0 && s.BSProcced > 0)
            {
                double slamDuration = (s.BSHasted ? HastedGCD : LatentGCD);
                if (s.WWCooldown < slamDuration)
                {
                    double WWtimeDelayed = slamDuration - s.WWCooldown;
                    double WWpercentLost = WWtimeDelayed / WW.ability.Cd;
                    double dmgLost = WW.ability.DamageOnUse * WWpercentLost;
                    double numHSesOnDelay = s.WWCooldown / WhiteInterval * HSPerc;
                    
                    if (s.BTCooldown < s.WWCooldown + LatentGCD) // if delaying lets us WW>BT
                    {
                        numHSesOnDelay = (s.WWCooldown + LatentGCD) / WhiteInterval * HSPerc;

                        double BTtimeDelayed = slamDuration + LatentGCD - s.BTCooldown;
                        double BTpercentLost = BTtimeDelayed / BT.ability.Cd;
                        dmgLost += BT.ability.DamageOnUse * BTpercentLost;    
                    }

                    double slamDmgGained = 0;
                }
            }*/

            if (s.WWCooldown <= 0)
            {
                #region Whirlwind
                double transDuration = LatentGCD;
                double newBTCD = Math.Max(0, s.BTCooldown - transDuration);
                double newWWCD = WW.ability.Cd - transDuration;
                
                double numHSes = transDuration / WhiteInterval;

                BloodSurgeMatrix _mat = new BloodSurgeMatrix(this.BS1Proc, this.BS2Proc);
                _mat.SetBaseChance(WWMatrix.ProcChances(1) + WWMatrix.ProcChances(2));
                _mat.AddAbility(HS.ability.MHAtkTable.AnyLand * this.HSPerc, numHSes);

                double[] actual = { _mat.ProcChances(0), _mat.ProcChances(1), _mat.ProcChances(2) };
                double chanceProc = (1 - WW.ability.MHAtkTable.AnyLand * BS1Proc) * (1 - WW.ability.OHAtkTable.AnyLand * BS1Proc);

                double[] test = { chanceProc * Math.Pow(1 - HS.ability.MHAtkTable.AnyLand * BS1Proc * HSPerc, numHSes),
                                 (1 - chanceProc * Math.Pow(1 - HS.ability.MHAtkTable.AnyLand * BS1Proc * HSPerc, numHSes)) * (1 - BS2Proc),
                                 (1 - chanceProc * Math.Pow(1 - HS.ability.MHAtkTable.AnyLand * BS1Proc * HSPerc, numHSes)) * BS2Proc,
                                };

                DelegateStateTransition.newTransitionChance del0 = () =>
                {
                    return chanceProc * Math.Pow(1 - HSProcChance * HSPerc, numHSes);
                };
                DelegateStateTransition.newTransitionChance del1 = () =>
                {
                    return (1 - chanceProc * Math.Pow(1 - HSProcChance * HSPerc, numHSes)) * (1 - BS2Proc);
                };
                DelegateStateTransition.newTransitionChance del2 = () =>
                {
                    return (1 - chanceProc * Math.Pow(1 - HSProcChance * HSPerc, numHSes)) * BS2Proc;
                };
                
                //double WWProc = (1 - BS1Proc * WW.ability.MHAtkTable.AnyLand) * (1 - BS1Proc * WW.ability.OHAtkTable.AnyLand);
                //double HSProc = BS1Proc * HS.ability.MHAtkTable.AnyLand;
                //double both = WWProc * Math.Pow(1 - HSProc, numHSes);
                            
                // if WW is up, use it

                list.Add(new DelegateStateTransition()
                {
                    Ability = WW.ability,
                    TransitionDuration = transDuration,
                    TargetState = GetState(2, true, newBTCD, newWWCD, 2),
                    myDel = del2,
                });
                
                // if BS procs 1
                list.Add(new DelegateStateTransition()
                {
                    Ability = WW.ability,
                    TransitionDuration = transDuration,
                    TargetState = GetState((s.BSHasted ? 2 : 1), s.BSHasted, newBTCD, newWWCD, 1),
                    myDel = del1,
                });

                // if BS doesn't proc
                list.Add(new DelegateStateTransition()
                {
                    Ability = WW.ability,
                    TransitionDuration = transDuration,
                    TargetState = GetState(s.BSProcced, s.BSHasted, newBTCD, newWWCD, 0),
                    myDel = del0
                });
                #endregion
            }
            else if (s.BTCooldown <= 0)
            {
                #region BloodThirst
                double transDuration = LatentGCD;
                double newBTCD = BT.ability.Cd - transDuration;
                double newWWCD = Math.Max(0, s.WWCooldown - transDuration);

                double numHSes = transDuration / WhiteInterval;

                double chanceProc = (1 - BT.ability.MHAtkTable.AnyLand * BS1Proc);
                double[] test = { chanceProc * Math.Pow(1 - HS.ability.MHAtkTable.AnyLand * BS1Proc, HSPerc),
                                  chanceProc * Math.Pow(1 - HS.ability.MHAtkTable.AnyLand * BS1Proc, HSPerc) * (1 - BS2Proc),
                                  chanceProc * Math.Pow(1 - HS.ability.MHAtkTable.AnyLand * BS1Proc, HSPerc) * BS2Proc
                                };
                DelegateStateTransition.newTransitionChance del0 = () =>
                {
                    return chanceProc * Math.Pow(1 - HSProcChance * HSPerc, numHSes);
                };
                DelegateStateTransition.newTransitionChance del1 = () =>
                {
                    return (1 - chanceProc * Math.Pow(1 - HSProcChance * HSPerc, numHSes)) * (1 - BS2Proc);
                };
                DelegateStateTransition.newTransitionChance del2 = () =>
                {
                    return (1 - chanceProc * Math.Pow(1 - HSProcChance * HSPerc, numHSes)) * BS2Proc;
                };

                
                // if BS procs 2
                
                list.Add(new DelegateStateTransition()
                {
                    Ability = BT.ability,
                    TransitionDuration = transDuration,
                    TargetState = GetState(2, true, newBTCD, newWWCD, 2),
                    myDel = del2
                });
                
                // if BS procs 1
                list.Add(new DelegateStateTransition()
                {
                    Ability = BT.ability,
                    TransitionDuration = transDuration,
                    TargetState = GetState((s.BSHasted ? 2 : 1), s.BSHasted, newBTCD, newWWCD, 1),
                    myDel = del1
                });
                // if BS doesn't proc
                list.Add(new DelegateStateTransition()
                {
                    Ability = BT.ability,
                    TransitionDuration = transDuration,
                    TargetState = GetState(s.BSProcced, s.BSHasted, newBTCD, newWWCD, 0),
                    myDel = del0
                });
                #endregion
            }

            else if (s.BSProcced > 0)
            {
                #region Slam

                double transDuration = (s.BSHasted ? HastedGCD : LatentGCD);
                double newBTCD = Math.Max(0, s.BTCooldown - transDuration);
                double newWWCD = Math.Max(0, s.WWCooldown - transDuration);

                double numHSes = transDuration / WhiteInterval;

                double chanceProc = 1;
                double[] test = { Math.Pow(1 - HS.ability.MHAtkTable.AnyLand * BS1Proc, HSPerc),
                                  Math.Pow(1 - HS.ability.MHAtkTable.AnyLand * BS1Proc, HSPerc) * (1 - BS2Proc),
                                  Math.Pow(1 - HS.ability.MHAtkTable.AnyLand * BS1Proc, HSPerc) * BS2Proc
                                };
                DelegateStateTransition.newTransitionChance del0 = () =>
                {
                    return chanceProc * Math.Pow(1 - HSProcChance * HSPerc, numHSes);
                };
                DelegateStateTransition.newTransitionChance del1 = () =>
                {
                    return (1 - chanceProc * Math.Pow(1 - HSProcChance * HSPerc, numHSes)) * (1 - BS2Proc);
                };
                DelegateStateTransition.newTransitionChance del2 = () =>
                {
                    return (1 - chanceProc * Math.Pow(1 - HSProcChance * HSPerc, numHSes)) * BS2Proc;
                };

                // if HS procs 2
                list.Add(new DelegateStateTransition()
                  {
                      Ability = BS.ability,
                      TransitionDuration = transDuration,
                      TargetState = GetState(2, true, newBTCD, newWWCD, 2),
                      myDel = del2
                  });
                

                // if HS procs 1
                list.Add(new DelegateStateTransition()
                {
                    Ability = BS.ability,
                    TransitionDuration = transDuration,
                    TargetState = GetState(s.BSProcced, // if we had 2 and then procced, we're at 2.  If we had 1 and then procced, we're at 1
                                           (s.BSProcced == 2), // if we're at 2 before, we're still hasted.  If we're at 1 before, we lost the haste before we procced
                                           newBTCD, newWWCD, 1),
                    myDel = del1
                });

                // if HS doesn't proc
                list.Add(new DelegateStateTransition()
                {
                    Ability = BS.ability,
                    TransitionDuration = transDuration,
                    TargetState = GetState(s.BSProcced - 1,    // num procs decreases
                                           (s.BSProcced == 2), // haste falls off if we were at 1
                                           newBTCD, newWWCD, 0),
                    myDel = del0
                });
                #endregion
            }
            else
            {
                double transDuration = Math.Min(s.BTCooldown, s.WWCooldown);

                double numHSes = transDuration / WhiteInterval;
                double chanceProc = 1;
                double[] test = { Math.Pow(1 - HS.ability.MHAtkTable.AnyLand * BS1Proc, HSPerc),
                                  Math.Pow(1 - HS.ability.MHAtkTable.AnyLand * BS1Proc, HSPerc) * (1 - BS2Proc),
                                  Math.Pow(1 - HS.ability.MHAtkTable.AnyLand * BS1Proc, HSPerc) * BS2Proc
                                };
                DelegateStateTransition.newTransitionChance del0 = () =>
                {
                    return chanceProc * Math.Pow(1 - HSProcChance * HSPerc, numHSes);
                };
                DelegateStateTransition.newTransitionChance del1 = () =>
                {
                    return (1 - chanceProc * Math.Pow(1 - HSProcChance * HSPerc, numHSes)) * (1 - BS2Proc);
                };
                DelegateStateTransition.newTransitionChance del2 = () =>
                {
                    return (1 - chanceProc * Math.Pow(1 - HSProcChance * HSPerc, numHSes)) * BS2Proc;
                };

                list.Add(new DelegateStateTransition()
                    {
                        Ability = null,
                        TransitionDuration = transDuration,
                        TargetState = GetState(2, true, s.BTCooldown - transDuration, s.WWCooldown - transDuration, 2),
                        myDel = del2
                    });

                // if HS procs 1
                list.Add(new DelegateStateTransition()
                {
                    Ability = null,
                    TransitionDuration = transDuration,
                    TargetState = GetState((s.BSHasted ? 2 : 1), s.BSHasted, s.BTCooldown - transDuration, s.WWCooldown - transDuration, 1),
                    myDel = del1
                });

                // if HS doesn't proc
                list.Add(new DelegateStateTransition()
                {
                    Ability = null,
                    TransitionDuration = transDuration,
                    TargetState = GetState(s.BSProcced, s.BSHasted, s.BTCooldown - transDuration, s.WWCooldown - transDuration, 0),
                    myDel = del0
                });
            }

            // Test to make sure probabilities aren't messed up
            double prob = 0;
            foreach (StateTransition<Skills.Ability> st in list)
            {
                prob += st.TransitionProbability;
                StateFury sf = (StateFury)st.TargetState;
                if (s.BSHasted == sf.BSHasted &&
                    s.BSProcced == sf.BSProcced &&
                    s.BTCooldown == sf.BTCooldown &&
                    //s.WhiteCooldown == sf.WhiteCooldown &&
                    s.WWCooldown == sf.WWCooldown)
                {
                    //int j = 0; //break
                }
            }
            if (prob != 1)
            {
                //int j = 0; //break
            }

            return list;
        }
        private Dictionary<string, StateFury> stateDictionary = new Dictionary<string, StateFury>();
        public StateFury GetState(int BSProc, bool BSHasted, double BTCD, double WWCD, int transition)
        {
            string name = string.Format(
#if TRANSTYPE
                "BSProc {0}{1},BTCD {2:"+FORMAT+"},WWCD {3:"+FORMAT+"}, TransitionType{4}",
#else
                "BSProc {0}{1},BTCD {2:"+FORMAT+"},WWCD {3:"+FORMAT+"}",
#endif
                BSProc,
                BSHasted ? "+" : "-",
                BTCD,
#if TRANSTYPE
                WWCD,
                transition);
#else
                WWCD);
#endif
            StateFury state;
            if (!stateDictionary.TryGetValue(name, out state))
            {
                state = new StateFury()
                {
                    Name = name,
                    BTCooldown = BTCD,
                    WWCooldown = WWCD,
                    BSHasted = BSHasted,
                    BSProcced = BSProc,
#if TRANSTYPE
                    transitionType = transition
#endif
                };
                stateDictionary[name] = state;
            }
            return state;
        }
        public StateFury GetState(int BSProc, bool BSHasted, float BTCD, float WWCD, int transition)
        {
            return GetState(BSProc, BSHasted, (double)BTCD, (double)WWCD, transition);
        }

        internal void WalkStateSpace(List<State<Rawr.DPSWarr.Skills.Ability>> stateSpace)
        {
            double WWProc = (1 - BS1Proc * WW.MHAtkTable.AnyLand) * (1 - BS1Proc * WW.OHAtkTable.AnyLand);
            double BTProc = (1 - BS1Proc * BT.MHAtkTable.AnyLand);
            double HSconst = BS1Proc * HS.MHAtkTable.AnyLand;

            double transChanceBoth, innerParenth;

            bool caughtYet = false;
            foreach (State<Skills.Ability> s in stateSpace)
            {
                foreach (StateTransition<Skills.Ability> t in s.Transitions)
                {
                    if (t.TargetState.Name.EndsWith("2") && t.Ability == WW && !caughtYet)
                    {
                        caughtYet = true;
                        System.Console.WriteLine(t.TransitionProbability);
                    }

                    if (t.TargetState.Name.EndsWith("2"))
                    {
                        transChanceBoth = t.TransitionProbability / BS2Proc;

                    }
                    else if (t.TargetState.Name.EndsWith("1"))
                    {
                        transChanceBoth = t.TransitionProbability / (1 - BS2Proc);
                    }
                    else
                    {
                        transChanceBoth = 1 - t.TransitionProbability;
                    }

                    if (t.Ability == WW)
                    {
                        innerParenth = (1 - transChanceBoth) / WWProc;
                    }
                    else if (t.Ability == BT)
                    {
                        innerParenth = (1 - transChanceBoth) / BTProc;
                    }
                    else if (t.Ability == BS)
                    {
                        innerParenth = 1 - transChanceBoth;
                    }
                    else
                    {
                        innerParenth = 1 - transChanceBoth;
                    }
                    double perc = (1 - Math.Pow(innerParenth, WhiteInterval / t.TransitionDuration)) / HSconst;
                    //Console.WriteLine(perc);
                }
            }
        }
    }

    public class StateSpaceGeneratorFuryTest
    {
        public void StateSpaceGeneratorFuryTest1(Character c, Stats s, CombatFactors cf, Skills.WhiteAttacks wa, CalculationOptionsDPSWarr co, BossOptions bo, bool showOutput)
        {
            FuryGenerator gen = new FuryGenerator(c, s, cf, wa, co, bo);
#if !SILVERLIGHT
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
#endif
            List<State<Skills.Ability>> stateSpace = gen.GenerateStateSpace();
#if !SILVERLIGHT
            sw.Stop();
            Console.WriteLine("GenStateSpace: " + sw.ElapsedTicks);
#endif
            if (showOutput)
            {
                try
                {
                    int numIterations = 0;
                    gen.HSPerc = 1;
                    double oldDPS = 0;
                    while (numIterations < 2)
                    {
                        double averageDamage = 0;
                        double rageNeededNoHS = 0f;

#if !SILVERLIGHT
                        sw = System.Diagnostics.Stopwatch.StartNew();
#endif
                        MarkovProcess<Skills.Ability> mp2 = new MarkovProcess<Rawr.DPSWarr.Skills.Ability>(stateSpace);
#if !SILVERLIGHT
                        sw.Stop();
                        Console.WriteLine("MarkovProcess._ctor: " + sw.ElapsedTicks);
#endif
                        foreach (KeyValuePair<Skills.Ability, double> kvp in mp2.AbilityWeight)
                        {
#if RAWR3 || SILVERLIGHT
                            rageNeededNoHS += kvp.Key.RageCost * (kvp.Value * bo.BerserkTimer / mp2.AverageTransitionDuration);
#else
                            rageNeededNoHS += kvp.Key.RageCost * (kvp.Value * co.Duration / mp2.AverageTransitionDuration);
#endif
                            averageDamage += kvp.Key.DamageOnUse * kvp.Value;
                        }

                        double hsdps = gen.Rot.GetWrapper<Skills.HeroicStrike>().ability.DamageOnUse / wa.MhEffectiveSpeed * gen.HSPerc;
                        double mhdps = wa.MhDamageOnUse / wa.MhEffectiveSpeed * (1 - gen.HSPerc);
                        double ohdps = gen.Rot.WhiteAtks.OhDPS;

                        double dps = averageDamage / mp2.AverageTransitionDuration;
                        dps += hsdps + mhdps + ohdps;
                        System.Console.WriteLine(String.Format("DPS: {0} || HSPerc: {1}", dps, gen.HSPerc));

                        double rageNeeded = rageNeededNoHS;
                        Skills.HeroicStrike HS = gen.Rot.GetWrapper<Skills.HeroicStrike>().ability as Skills.HeroicStrike;
#if RAWR3 || SILVERLIGHT
                        rageNeeded += HS.FullRageCost * (bo.BerserkTimer / wa.MhEffectiveSpeed * gen.HSPerc);
                        double rageGenerated = wa.MHSwingRage * (bo.BerserkTimer / wa.MhEffectiveSpeed) +
                                               wa.OHSwingRage * (bo.BerserkTimer / wa.OhEffectiveSpeed);
#else
                        rageNeeded += HS.FullRageCost * (co.Duration / wa.MhEffectiveSpeed * gen.HSPerc);
                        double rageGenerated = wa.MHSwingRage * (co.Duration / wa.MhEffectiveSpeed) +
                                               wa.OHSwingRage * (co.Duration / wa.OhEffectiveSpeed);
#endif


                        double HsRage = rageNeeded - rageNeededNoHS;
                        double hsRageNeeded = rageGenerated - rageNeededNoHS;
                        gen.HSPerc = Math.Min((hsRageNeeded / HS.FullRageCost) /
#if RAWR3 || SILVERLIGHT
                                     (bo.BerserkTimer / wa.MhEffectiveSpeed), 1); // Needed HS Activates / White activates
#else
                                     (co.Duration / wa.MhEffectiveSpeed), 1); // Needed HS Activates / White activates
#endif

                        oldDPS = dps;
                        numIterations++;
                    }


                    /*MarkovProcess<Skills.Ability> mp = new MarkovProcess<Skills.Ability>(stateSpace);

                    double averageDamage2 = 0.0;
                    foreach (KeyValuePair<Skills.Ability, double> kvp in mp.AbilityWeight)
                    {
                        if (showOutput) System.Console.WriteLine("{0} - {1}", kvp.Key.Name, kvp.Key.DamageOnUse * kvp.Value / mp.AverageTransitionDuration);
                        averageDamage2 += kvp.Key.DamageOnUse * kvp.Value;
                    }
                    double hsdps2 = gen.Rot.GetWrapper<Skills.HeroicStrike>().ability.DamageOnUse / wa.MhEffectiveSpeed * gen.HSPerc;
                    double mhdps2 = wa.MhDamageOnUse / wa.MhEffectiveSpeed * (1 - gen.HSPerc);
                    double ohdps2 = gen.Rot.WhiteAtks.OhDPS;
                    if (showOutput)
                    {
                        System.Console.WriteLine("HS - {0}", hsdps2);
                        System.Console.WriteLine("MH - {0}", mhdps2);
                        System.Console.WriteLine("OH - {0}", ohdps2);
                    }
                    double dps2 = averageDamage2 / mp.AverageTransitionDuration;
                    dps2 += hsdps2 + mhdps2 + ohdps2;*/
                }
                catch (Exception ex)
                {
                    new ErrorBox("Error in creating Arms Markov Calculations",
                        ex.Message, "StateSpaceGeneratorArmsTest1()",
                        "StateSpace Count: " + stateSpace.Count.ToString(),
                        ex.StackTrace);
                }
            }
        }
    }
}
