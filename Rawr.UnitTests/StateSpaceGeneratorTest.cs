using Rawr.Base.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Rawr.UnitTests
{
    /// <summary>
    ///This is a test class for StateSpaceGeneratorTest and is intended
    ///to contain all StateSpaceGeneratorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StateSpaceGeneratorTest
    {
        private TestContext testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        // tests from http://elitistjerks.com/f75/t40398-mathematics_dynamic_cycles/

        // Next lets look at ABx3-ABar. We have to be very specific when specifying the spell selection.
        // For this cycle if ABar procs we always follow it with MBAM. If first AB procs we cast MBAM as 
        // soon as we notice the proc (that is after second AB) and follow MBAM with ABar. Similar if second 
        // AB procs we cast MBAM-ABar after the third AB. In both cases if the ABar after MBAM procs we follow 
        // with another MBAM. If third AB procs we won't notice it until after ABar.

        public class Ability
        {
            public double Damage { get; set; }
            public double Duration { get; set; }
        }

        public class AB3ABarGenerator : StateSpaceGenerator<Ability>
        {
            public class State : State<Ability>
            {
                public int ABStacks { get; set; }
                public bool MBProcced { get; set; }
                public bool MBRegistered { get; set; }
                public bool ABarFollowup { get; set; }
            }

            public Ability AB0 = new Ability();
            public Ability AB1 = new Ability();
            public Ability AB2 = new Ability();
            public Ability ABar = new Ability();
            public Ability ABar3 = new Ability();
            public Ability MBAM = new Ability();
            public Ability MBAM3 = new Ability();
            public Ability MBAM2 = new Ability();

            protected override State<Ability> GetInitialState()
            {
                return GetState(0, false, false, false);
            }

            protected override List<StateTransition<Ability>> GetStateTransitions(State<Ability> state)
            {
                State s = state as State;
                List<StateTransition<Ability>> list = new List<StateTransition<Ability>>();
                if (s.ABarFollowup)
                {
                    list.Add(new StateTransition<Ability>()
                    {
                        Ability = ABar,
                        TargetState = GetState(0, true, true, false),
                        TransitionDuration = ABar.Duration,
                        TransitionProbability = 0.2,
                    });
                    list.Add(new StateTransition<Ability>()
                    {
                        Ability = ABar,
                        TargetState = GetState(0, s.MBProcced, s.MBProcced, false),
                        TransitionDuration = ABar.Duration,
                        TransitionProbability = 0.8,
                    });
                }
                else if (s.ABStacks == 0 && s.MBRegistered)
                {
                    list.Add(new StateTransition<Ability>()
                    {
                        Ability = MBAM,
                        TargetState = GetState(0, false, false, false),
                        TransitionDuration = MBAM.Duration,
                        TransitionProbability = 1.0,
                    });
                }
                else if (s.ABStacks == 0 && !s.MBRegistered)
                {
                    list.Add(new StateTransition<Ability>()
                    {
                        Ability = AB0,
                        TargetState = GetState(1, true, s.MBProcced, false),
                        TransitionDuration = AB0.Duration,
                        TransitionProbability = 0.2,
                    });
                    list.Add(new StateTransition<Ability>()
                    {
                        Ability = AB0,
                        TargetState = GetState(1, s.MBProcced, s.MBProcced, false),
                        TransitionDuration = AB0.Duration,
                        TransitionProbability = 0.8,
                    });
                }
                else if (s.ABStacks == 1)
                {
                    list.Add(new StateTransition<Ability>()
                    {
                        Ability = AB1,
                        TargetState = GetState(2, true, s.MBProcced, false),
                        TransitionDuration = AB1.Duration,
                        TransitionProbability = 0.2,
                    });
                    list.Add(new StateTransition<Ability>()
                    {
                        Ability = AB1,
                        TargetState = GetState(2, s.MBProcced, s.MBProcced, false),
                        TransitionDuration = AB1.Duration,
                        TransitionProbability = 0.8,
                    });
                }
                else if (s.ABStacks == 2 && s.MBRegistered)
                {
                    list.Add(new StateTransition<Ability>()
                    {
                        Ability = MBAM2,
                        TargetState = GetState(0, false, false, true),
                        TransitionDuration = MBAM2.Duration,
                        TransitionProbability = 1.0,
                    });
                }
                else if (s.ABStacks == 2 && !s.MBRegistered)
                {
                    list.Add(new StateTransition<Ability>()
                    {
                        Ability = AB2,
                        TargetState = GetState(3, true, s.MBProcced, false),
                        TransitionDuration = AB2.Duration,
                        TransitionProbability = 0.2,
                    });
                    list.Add(new StateTransition<Ability>()
                    {
                        Ability = AB2,
                        TargetState = GetState(3, s.MBProcced, s.MBProcced, false),
                        TransitionDuration = AB2.Duration,
                        TransitionProbability = 0.8,
                    });
                }
                else if (s.ABStacks == 3 && s.MBRegistered)
                {
                    list.Add(new StateTransition<Ability>()
                    {
                        Ability = MBAM3,
                        TargetState = GetState(0, false, false, true),
                        TransitionDuration = MBAM3.Duration,
                        TransitionProbability = 1.0,
                    });
                }
                else if (s.ABStacks == 3 && !s.MBRegistered)
                {
                    list.Add(new StateTransition<Ability>()
                    {
                        Ability = ABar3,
                        TargetState = GetState(0, true, true, false),
                        TransitionDuration = ABar3.Duration,
                        TransitionProbability = 0.2,
                    });
                    list.Add(new StateTransition<Ability>()
                    {
                        Ability = ABar3,
                        TargetState = GetState(0, s.MBProcced, s.MBProcced, false),
                        TransitionDuration = ABar3.Duration,
                        TransitionProbability = 0.8,
                    });
                }
                return list;
            }

            private Dictionary<string, State> stateDictionary = new Dictionary<string, State>();

            private State GetState(int abStack, bool mbProcced, bool mbRegistered, bool abarFollowup)
            {
                string name = string.Format("AB{0}MB{1}{2}ABar{3}", abStack, mbProcced ? "+" : "-", mbRegistered ? "+" : "-", abarFollowup ? "+" : "-");
                State state;
                if (!stateDictionary.TryGetValue(name, out state))
                {
                    state = new State() { ABStacks = abStack, MBProcced = mbProcced, MBRegistered = mbRegistered, ABarFollowup = abarFollowup };
                    stateDictionary[name] = state;
                }
                return state;
            }
        }

        [TestMethod()]
        public void StateSpaceGeneratorTest1()
        {
            AB3ABarGenerator gen = new AB3ABarGenerator();

            var stateSpace = gen.GenerateStateSpace();

            MarkovProcess<Ability> mp = new MarkovProcess<Ability>(stateSpace);
            double unit = mp.AbilityWeight[gen.AB0];

            Assert.AreEqual(mp.AbilityWeight[gen.AB0] / unit, 1.0, 0.000001, "AB0");
            Assert.AreEqual(mp.AbilityWeight[gen.AB1] / unit, 1.0, 0.000001, "AB1");
            Assert.AreEqual(mp.AbilityWeight[gen.AB2] / unit, 0.8, 0.000001, "AB2");
            Assert.AreEqual(mp.AbilityWeight[gen.ABar] / unit, 0.36, 0.000001, "ABar");
            Assert.AreEqual(mp.AbilityWeight[gen.ABar3] / unit, 0.64, 0.000001, "ABar3");
            Assert.AreEqual(mp.AbilityWeight[gen.MBAM2] / unit, 0.2, 0.000001, "MBAM2");
            Assert.AreEqual(mp.AbilityWeight[gen.MBAM3] / unit, 0.16, 0.000001, "MBAM3");
            Assert.AreEqual(mp.AbilityWeight[gen.MBAM] / unit, 0.3024, 0.000001, "MBAM");
        }
    }
}
