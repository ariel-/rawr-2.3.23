using Rawr.Base.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Rawr.UnitTests
{
    /// <summary>
    ///This is a test class for MarkovProcessTest and is intended
    ///to contain all MarkovProcessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MarkovProcessTest
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

        public class Ability
        {
            public double Damage { get; set; }
            public double Duration { get; set; }
        }

        public class AbilitySequence : Ability
        {
            public AbilitySequence(params Ability[] abilities)
            {
                foreach (Ability ability in abilities)
                {
                    Damage += ability.Damage;
                    Duration += ability.Duration;
                }
            }
        }

        [TestMethod]
        public void MarkovProcessTest1()
        {
            //S0:
            //A => S0 0.5
            //  => S1 0.5
            //S1:
            //B => S0 1

            Ability A = new Ability() { Damage = 100, Duration = 2 };
            Ability B = new Ability() { Damage = 200, Duration = 1 };

            State<Ability> S0 = new State<Ability>() { Name = "S0" };
            State<Ability> S1 = new State<Ability>() { Name = "S1" };

            S0.Transitions = new List<StateTransition<Ability>>()
            {
                new StateTransition<Ability>() { Ability = A, TargetState = S0, TransitionDuration = A.Duration, TransitionProbability = 0.5 },
                new StateTransition<Ability>() { Ability = A, TargetState = S1, TransitionDuration = A.Duration, TransitionProbability = 0.5 },
            };
            S1.Transitions = new List<StateTransition<Ability>>()
            {
                new StateTransition<Ability>() { Ability = B, TargetState = S0, TransitionDuration = B.Duration, TransitionProbability = 1.0 },
            };
            List<State<Ability>> stateSpace = new List<State<Ability>>() { S0, S1 };

            MarkovProcess<Ability> mp = new MarkovProcess<Ability>(stateSpace);

            Assert.AreEqual(mp.StateWeight[0], 2.0 / 3.0, 0.000000000001, "S0");
            Assert.AreEqual(mp.StateWeight[1], 1.0 / 3.0, 0.000000000001, "S1");
            Assert.AreEqual(mp.AverageTransitionDuration, 5.0 / 3.0, 0.000000000001, "time");

            double averageDamage = 0.0;
            foreach (KeyValuePair<Ability, double> kvp in mp.AbilityWeight)
            {
                averageDamage += kvp.Key.Damage * kvp.Value;
            }
            Assert.AreEqual(averageDamage, 100.0 * 4.0 / 3.0, 0.000000000001, "damage");

            Assert.AreEqual(averageDamage / mp.AverageTransitionDuration, 80.0, 0.000000000001, "dps");
        }

        [TestMethod]
        public void MarkovProcessTest2()
        {
            //S0:
            //A  => S0 0.5
            //AB  => S0 0.5

            Ability A = new Ability() { Damage = 100, Duration = 2 };
            Ability B = new Ability() { Damage = 200, Duration = 1 };
            Ability AB = new AbilitySequence(A, B);

            State<Ability> S0 = new State<Ability>() { Name = "S0" };

            S0.Transitions = new List<StateTransition<Ability>>()
            {
                new StateTransition<Ability>() { Ability = A, TargetState = S0, TransitionDuration = A.Duration, TransitionProbability = 0.5 },
                new StateTransition<Ability>() { Ability = AB, TargetState = S0, TransitionDuration = AB.Duration, TransitionProbability = 0.5 },
            };
            List<State<Ability>> stateSpace = new List<State<Ability>>() { S0 };

            MarkovProcess<Ability> mp = new MarkovProcess<Ability>(stateSpace);

            Assert.AreEqual(mp.StateWeight[0], 1.0, 0.000000000001, "S0");
            Assert.AreEqual(mp.AverageTransitionDuration, 2.5, 0.000000000001, "time");

            double averageDamage = 0.0;
            foreach (KeyValuePair<Ability, double> kvp in mp.AbilityWeight)
            {
                averageDamage += kvp.Key.Damage * kvp.Value;
            }
            Assert.AreEqual(averageDamage, 200.0, "damage");

            Assert.AreEqual(averageDamage / mp.AverageTransitionDuration, 80.0, 0.000000000001, "dps");
        }

        [TestMethod]
        public void MarkovProcessTest3()
        {
            //S0:
            //AB-ABar1 => S0 0.8 * 0.8
            //         => S1 1 - 0.8 * 0.8
            //S1:
            //MBAM-ABar => S0 0.8
            //          => S1 0.2

            Ability AB = new Ability();
            Ability ABar1 = new Ability();
            Ability MBAM = new Ability();
            Ability ABar = new Ability();

            Ability ABABar1 = new AbilitySequence(AB, ABar1);
            Ability MBAMABar = new AbilitySequence(MBAM, ABar);

            State<Ability> S0 = new State<Ability>() { Name = "S0" };
            State<Ability> S1 = new State<Ability>() { Name = "S1" };

            S0.Transitions = new List<StateTransition<Ability>>()
            {
                new StateTransition<Ability>() { Ability = ABABar1, TargetState = S0, TransitionDuration = ABABar1.Duration, TransitionProbability = 0.8 * 0.8 },
                new StateTransition<Ability>() { Ability = ABABar1, TargetState = S1, TransitionDuration = ABABar1.Duration, TransitionProbability = 1.0 - 0.8 * 0.8 },
            };
            S1.Transitions = new List<StateTransition<Ability>>()
            {
                new StateTransition<Ability>() { Ability = MBAMABar, TargetState = S0, TransitionDuration = MBAMABar.Duration, TransitionProbability = 0.8 },
                new StateTransition<Ability>() { Ability = MBAMABar, TargetState = S1, TransitionDuration = MBAMABar.Duration, TransitionProbability = 0.2 },
            };
            List<State<Ability>> stateSpace = new List<State<Ability>>() { S0, S1 };

            MarkovProcess<Ability> mp = new MarkovProcess<Ability>(stateSpace);

            Assert.AreEqual(mp.StateWeight[0], 0.69, 0.01, "S0");
            Assert.AreEqual(mp.StateWeight[1], 0.31, 0.01, "S1");
        }
    }
}
