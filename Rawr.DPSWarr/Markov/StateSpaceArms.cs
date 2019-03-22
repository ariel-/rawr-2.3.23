using System;
using System.Collections.Generic;
using Rawr.Base.Algorithms;
using System.Text;
using Rawr.Base;

namespace Rawr.DPSWarr.Markov
{
    public class ArmsGenerator : StateSpaceGenerator<Skills.Ability>
    {
        public ArmsGenerator(Character c, Stats s, CombatFactors cf, Skills.WhiteAttacks wa, CalculationOptionsDPSWarr co, BossOptions bo) {
            Char = c; Talents = c.WarriorTalents; StatS = s; combatFactors = cf; WhiteAtks = wa; CalcOpts = co; BossOpts = bo;// TalentsCata = c.WarriorTalentsCata;
            //
            Rot = new ArmsRotation(c, s, cf, wa, co, bo);
            Rot.Initialize();
            LatentGCD = 1.5 + co.FullLatency;
        }

        #region Variables
        protected double LatentGCD;
        public ArmsRotation Rot = null;
        Character Char;
        WarriorTalents Talents;
        WarriorTalentsCata TalentsCata;
        Stats StatS;
        CombatFactors combatFactors;
        Skills.WhiteAttacks WhiteAtks;
        CalculationOptionsDPSWarr CalcOpts;
        BossOptions BossOpts;
        #endregion

        public class StateArms : State<Skills.Ability>
        {
            public double Current_Rage;
            public double TimeTilNext_RendTickProcgTfB;
            public bool HaveBuff_OPTfB;
            public bool HaveBuff_SD;
            public double CDRem_MS;
            public bool AbilRdy_MS;
            public bool ThereAreMultipleMobs;
        }

        protected override State<Skills.Ability> GetInitialState()
        {
            return GetState(0, 6, false, false, 0, true, false);
            //return GetState(0, 6, false, false, 0, true, false);
        }

        private double WhiteRageForAGCD {
            get {
                return (Rot.WhiteAtks.whiteRageGenOverDur
                        * LatentGCD)
#if RAWR3 || SILVERLIGHT
                        / BossOpts.BerserkTimer;
#else
                        / CalcOpts.Duration;
#endif
            }
        }

        protected override List<StateTransition<Skills.Ability>> GetStateTransitions(State<Skills.Ability> state)
        {
            StateArms s = state as StateArms;
            List<StateTransition<Skills.Ability>> list = new List<StateTransition<Skills.Ability>>();
            /*
            Rawr.DPSWarr.Rotation.AbilWrapper MS = Rot.GetWrapper<Skills.MortalStrike>();
            Rawr.DPSWarr.Rotation.AbilWrapper OP = Rot.GetWrapper<Skills.OverPower>();
            Rawr.DPSWarr.Rotation.AbilWrapper TB = Rot.GetWrapper<Skills.TasteForBlood>();
            Rawr.DPSWarr.Rotation.AbilWrapper SD = Rot.GetWrapper<Skills.Suddendeath>();
            Rawr.DPSWarr.Rotation.AbilWrapper SL = Rot.GetWrapper<Skills.Slam>();
            Skills.Suddendeath _SD = SD.ability as Skills.Suddendeath;


            if (s.CDRem_MS != 0 && s.CDRem_MS < LatentGCD)
            {
                // do nothing, don't want to reset GCD and delay MS
                // later we'll consider delaying for an extra Slam or Execute
                double dur = LatentGCD - s.CDRem_MS;
                list.Add(new StateTransition<Skills.Ability>()
                {
                    Ability = null,
                    TransitionDuration = dur,
                    TargetState = GetState(
                        Math.Min(100, s.Current_Rage + WhiteRageForAGCD * (dur / LatentGCD)),
                        s.TimeTilNext_RendTickProcgTfB > dur ? s.TimeTilNext_RendTickProcgTfB - dur : 6f + s.TimeTilNext_RendTickProcgTfB - dur,
                        s.HaveBuff_OPTfB || (Math.Max(0f, s.TimeTilNext_RendTickProcgTfB - dur) == 0),
                        s.HaveBuff_SD,
                        Math.Max(0f, s.CDRem_MS - LatentGCD),
                        true,
                        s.ThereAreMultipleMobs
                    ),
                    TransitionProbability = 1.0,
                });
            }
            else if (s.CDRem_MS == 0 && s.Current_Rage > MS.ability.RageCost)
            {
                // Time to MS!
                list.Add(new StateTransition<Skills.Ability>()
                {
                    Ability = MS.ability,
                    TransitionDuration = LatentGCD,
                    TargetState = GetState(
                        Math.Max(0, Math.Min(100, s.Current_Rage - MS.ability.RageCost + WhiteRageForAGCD)),
                        s.TimeTilNext_RendTickProcgTfB > LatentGCD ? s.TimeTilNext_RendTickProcgTfB - LatentGCD : 6f + s.TimeTilNext_RendTickProcgTfB - LatentGCD,
                        s.HaveBuff_OPTfB || (Math.Max(0f, s.TimeTilNext_RendTickProcgTfB - LatentGCD) == 0),
                        true,
                        MS.ability.Cd,
                        false,
                        s.ThereAreMultipleMobs
                    ),
                    TransitionProbability = 1.0 * (0.03 * Talents.SuddenDeath) * MS.ability.MHAtkTable.AnyLand,
                });
                list.Add(new StateTransition<Skills.Ability>()
                {
                    Ability = MS.ability,
                    TransitionDuration = LatentGCD,
                    TargetState = GetState(
                        Math.Max(0, Math.Min(100, s.Current_Rage - MS.ability.RageCost + WhiteRageForAGCD)),
                        s.TimeTilNext_RendTickProcgTfB > LatentGCD ? s.TimeTilNext_RendTickProcgTfB - LatentGCD : 6f + s.TimeTilNext_RendTickProcgTfB - LatentGCD,
                        s.HaveBuff_OPTfB || (Math.Max(0f, s.TimeTilNext_RendTickProcgTfB - LatentGCD) == 0),
                        s.HaveBuff_SD,
                        MS.ability.Cd,
                        false,
                        s.ThereAreMultipleMobs
                    ),
                    TransitionProbability = 1 - list[list.Count - 1].TransitionProbability,
                });
            }
            else if ((s.TimeTilNext_RendTickProcgTfB == 0 || s.HaveBuff_OPTfB) && s.Current_Rage > OP.ability.RageCost)
            {
                // TfB should proc now, we can use the ability after react time
                list.Add(new StateTransition<Skills.Ability>()
                {
                    Ability = s.TimeTilNext_RendTickProcgTfB == 0 ? (Skills.Ability)TB.ability : (Skills.Ability)OP.ability,
                    TransitionDuration = LatentGCD,
                    TargetState = GetState(
                        Math.Max(0, Math.Min(100, s.Current_Rage - TB.ability.RageCost + WhiteRageForAGCD)),
                        6f,
                        false,
                        true,
                        Math.Max(0f, s.CDRem_MS - LatentGCD),
                        s.AbilRdy_MS,
                        s.ThereAreMultipleMobs
                    ),
                    TransitionProbability = 1.0 * (0.03 * Talents.SuddenDeath) * TB.ability.MHAtkTable.AnyLand,
                });
                list.Add(new StateTransition<Skills.Ability>()
                {
                    Ability = s.TimeTilNext_RendTickProcgTfB == 0 ? (Skills.Ability)TB.ability : (Skills.Ability)OP.ability,
                    TransitionDuration = LatentGCD,
                    TargetState = GetState(
                        Math.Max(0, Math.Min(100, s.Current_Rage - TB.ability.RageCost + WhiteRageForAGCD)),
                        6f,
                        false,
                        s.HaveBuff_SD,
                        Math.Max(0f, s.CDRem_MS - LatentGCD),
                        s.AbilRdy_MS,
                        s.ThereAreMultipleMobs
                    ),
                    TransitionProbability = 1 - list[list.Count - 1].TransitionProbability,
                });
            }
            else if (s.HaveBuff_SD && s.Current_Rage > SD.ability.RageCost)
            {
                // Sudden death is active, we can execute
                list.Add(new StateTransition<Skills.Ability>()
                {
                    Ability = SD.ability,
                    TransitionDuration = LatentGCD,
                    TargetState = GetState(
                        Math.Max(0, Math.Min(100, s.Current_Rage - (SD.ability.RageCost + _SD.UsedExtraRage) + WhiteRageForAGCD)),
                        s.TimeTilNext_RendTickProcgTfB > LatentGCD ? s.TimeTilNext_RendTickProcgTfB - LatentGCD : 6f + s.TimeTilNext_RendTickProcgTfB - LatentGCD,
                        s.HaveBuff_OPTfB || (Math.Max(0f, s.TimeTilNext_RendTickProcgTfB - LatentGCD) == 0),
                        true, // procs off itself
                        Math.Max(0f, s.CDRem_MS - LatentGCD),
                        s.AbilRdy_MS,
                        s.ThereAreMultipleMobs
                    ),
                    TransitionProbability = 1.0 * (0.03 * Talents.SuddenDeath) * SD.ability.MHAtkTable.AnyLand,
                });
                list.Add(new StateTransition<Skills.Ability>()
                {
                    Ability = SD.ability,
                    TransitionDuration = LatentGCD,
                    TargetState = GetState(
                        Math.Max(0, Math.Min(100, s.Current_Rage - (SD.ability.RageCost + _SD.UsedExtraRage) + WhiteRageForAGCD)),
                        s.TimeTilNext_RendTickProcgTfB > LatentGCD ? s.TimeTilNext_RendTickProcgTfB - LatentGCD : 6f + s.TimeTilNext_RendTickProcgTfB - LatentGCD,
                        s.HaveBuff_OPTfB || (Math.Max(0f, s.TimeTilNext_RendTickProcgTfB - LatentGCD) == 0),
                        false, // didn't proc, current proc consumed
                        Math.Max(0f, s.CDRem_MS - LatentGCD),
                        s.AbilRdy_MS,
                        s.ThereAreMultipleMobs
                    ),
                    TransitionProbability = 1 - list[list.Count - 1].TransitionProbability,
                });
            }
            else if (s.Current_Rage > SL.ability.RageCost)
            {
                // do slam if nothing else
                list.Add(new StateTransition<Skills.Ability>()
                {
                    Ability = SL.ability,
                    TransitionDuration = LatentGCD,
                    TargetState = GetState(
                        Math.Max(0, Math.Min(100, s.Current_Rage - SL.ability.RageCost + WhiteRageForAGCD * (Talents.ImprovedSlam / 3))),
                        s.TimeTilNext_RendTickProcgTfB > LatentGCD ? s.TimeTilNext_RendTickProcgTfB - LatentGCD : 6f + s.TimeTilNext_RendTickProcgTfB - LatentGCD,
                        s.HaveBuff_OPTfB || (Math.Max(0f, s.TimeTilNext_RendTickProcgTfB - LatentGCD) == 0),
                        true,
                        Math.Max(0f, s.CDRem_MS - LatentGCD),
                        s.AbilRdy_MS,
                        s.ThereAreMultipleMobs
                    ),
                    TransitionProbability = 1.0 * (0.03 * Talents.SuddenDeath) * SL.ability.MHAtkTable.AnyLand,
                });
                list.Add(new StateTransition<Skills.Ability>()
                {
                    Ability = SL.ability,
                    TransitionDuration = LatentGCD,
                    TargetState = GetState(
                        Math.Max(0, Math.Min(100, s.Current_Rage - SL.ability.RageCost + WhiteRageForAGCD * (Talents.ImprovedSlam / 3))),
                        s.TimeTilNext_RendTickProcgTfB > LatentGCD ? s.TimeTilNext_RendTickProcgTfB - LatentGCD : 6f + s.TimeTilNext_RendTickProcgTfB - LatentGCD,
                        s.HaveBuff_OPTfB || (Math.Max(0f, s.TimeTilNext_RendTickProcgTfB - LatentGCD) == 0),
                        s.HaveBuff_SD,
                        Math.Max(0f, s.CDRem_MS - LatentGCD),
                        s.AbilRdy_MS,
                        s.ThereAreMultipleMobs
                    ),
                    TransitionProbability = 1 - list[list.Count - 1].TransitionProbability,
                });
            }
            else
            {
                // We don't have enough rage to do anything on this GCD
                list.Add(new StateTransition<Skills.Ability>()
                {
                    Ability = null,
                    TransitionDuration = LatentGCD,
                    TargetState = GetState(
                        Math.Min(100, s.Current_Rage + WhiteRageForAGCD),
                        s.TimeTilNext_RendTickProcgTfB > LatentGCD ? s.TimeTilNext_RendTickProcgTfB - LatentGCD : 6f + s.TimeTilNext_RendTickProcgTfB - LatentGCD,
                        s.HaveBuff_OPTfB || (Math.Max(0f, s.TimeTilNext_RendTickProcgTfB - LatentGCD) == 0),
                        s.HaveBuff_SD,
                        Math.Max(0f, s.CDRem_MS - LatentGCD),
                        s.AbilRdy_MS,
                        s.ThereAreMultipleMobs
                    ),
                    TransitionProbability = 1.0,
                });
            }*/
            return list;
        }
        private Dictionary<string, StateArms> stateDictionary = new Dictionary<string, StateArms>();
        public StateArms GetState(double _rage, double _timetillnextTfBproccingrendtick,
                                  bool _OverpowerTfBbuff, bool _SuddenDeathbuff,
                                  double _MortalStrikecooldownleft, bool _MortalStrikeReady,
                                  bool _ThereAreMultipleMobs)
        {
            string name = string.Format(
                "Rage {0:000.0000},TfBBuff {1},SDBuff {2},MSCdRem {3:0.0000},MM {4}",//GCD {1:0.0000},White {2:0.0000},TfB {3:0.0000},
                Math.Round(_rage, 0),
                //_timetillnextTfBproccingrendtick,
                _OverpowerTfBbuff ? "+" : "-",
                _SuddenDeathbuff ? "+" : "-",
                Math.Round(_MortalStrikecooldownleft,1),//_MortalStrikeReady ? "+" : "-",
                _ThereAreMultipleMobs ? "+" : "-");
            StateArms state;
            if (!stateDictionary.TryGetValue(name, out state))
            {
                state = new StateArms()
                {
                    Name = name,
                    Current_Rage = Math.Round(_rage, 4),
                    TimeTilNext_RendTickProcgTfB = Math.Round(_timetillnextTfBproccingrendtick, 4),
                    HaveBuff_OPTfB = _OverpowerTfBbuff,
                    HaveBuff_SD = _SuddenDeathbuff,
                    CDRem_MS = Math.Round(_MortalStrikecooldownleft, 4),
                    AbilRdy_MS = _MortalStrikeReady,
                    ThereAreMultipleMobs = _ThereAreMultipleMobs,
                };
                stateDictionary[name] = state;
            }
            return state;
        }
        public StateArms GetState(float _rage, float _timetillnextTfBproccingrendtick,
                                  bool _OverpowerTfBbuff, bool _SuddenDeathbuff,
                                  float _MortalStrikecooldownleft, bool _MortalStrikeReady,
                                  bool _ThereAreMultipleMobs)
        {
            return GetState((double)_rage, (double)_timetillnextTfBproccingrendtick,
                                   _OverpowerTfBbuff, _SuddenDeathbuff,
                                   (double)_MortalStrikecooldownleft, _MortalStrikeReady,
                                   _ThereAreMultipleMobs);
        }
    }

    public class StateSpaceGeneratorArmsTest {
        public void StateSpaceGeneratorArmsTest1(Character c, Stats s, CombatFactors cf, Skills.WhiteAttacks wa, CalculationOptionsDPSWarr co, BossOptions bo)
        {
            ArmsGenerator gen = new ArmsGenerator(c, s, cf, wa, co, bo);
            var stateSpace = gen.GenerateStateSpace();
            string output = "";
            foreach (State<Rawr.DPSWarr.Skills.Ability> a in stateSpace) {
                output += a.ToString() + "\n";
            }
            output += "\ndone";
            try {
                MarkovProcess<Skills.Ability> mp = new MarkovProcess<Skills.Ability>(stateSpace);

                double averageDamage = 0.0;
                foreach (KeyValuePair<Skills.Ability, double> kvp in mp.AbilityWeight) {
                    averageDamage += kvp.Key.DamageOnUse * kvp.Value;
                }

                double dps = averageDamage / mp.AverageTransitionDuration;
                dps += gen.Rot.WhiteAtks.MhDPS;
            } catch (Exception ex) {
                new ErrorBox("Error in creating Arms Markov Calculations",
                    ex.Message, "StateSpaceGeneratorArmsTest1()",
                    "StateSpace Count: " + stateSpace.Count.ToString(),
                    ex.StackTrace);
            }
        }
    }
}
