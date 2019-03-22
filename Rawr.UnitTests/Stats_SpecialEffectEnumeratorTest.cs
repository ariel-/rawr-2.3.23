using Rawr;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections;

namespace Rawr.zzTestzz
{
    
    
    /// <summary>
    ///This is a test class for Stats_SpecialEffectEnumeratorTest and is intended
    ///to contain all Stats_SpecialEffectEnumeratorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class Stats_SpecialEffectEnumeratorTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
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

        /* Commenting out the tests that aren't implemented yet.
         * 
         * 
        /// <summary>
        ///A test for System.Collections.IEnumerator.Current
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Rawr.Base.dll")]
        public void CurrentTest1()
        {
            IEnumerator target = new Stats.SpecialEffectEnumerator(); // TODO: Initialize to an appropriate value
            object actual;
            actual = target.Current;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Current
        ///</summary>
        [TestMethod()]
        public void CurrentTest()
        {
            Stats.SpecialEffectEnumerator target = new Stats.SpecialEffectEnumerator(); // TODO: Initialize to an appropriate value
            SpecialEffect actual;
            actual = target.Current;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for System.Collections.IEnumerator.Reset
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Rawr.Base.dll")]
        public void ResetTest()
        {
            IEnumerator target = new Stats.SpecialEffectEnumerator(); // TODO: Initialize to an appropriate value
            target.Reset();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for System.Collections.IEnumerable.GetEnumerator
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Rawr.Base.dll")]
        public void GetEnumeratorTest2()
        {
            IEnumerable target = new Stats.SpecialEffectEnumerator(); // TODO: Initialize to an appropriate value
            IEnumerator expected = null; // TODO: Initialize to an appropriate value
            IEnumerator actual;
            actual = target.GetEnumerator();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for System.Collections.Generic.IEnumerable<Rawr.SpecialEffect>.GetEnumerator
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Rawr.Base.dll")]
        public void GetEnumeratorTest1()
        {
            IEnumerable<SpecialEffect> target = new Stats.SpecialEffectEnumerator(); // TODO: Initialize to an appropriate value
            IEnumerator<SpecialEffect> expected = null; // TODO: Initialize to an appropriate value
            IEnumerator<SpecialEffect> actual;
            actual = target.GetEnumerator();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MoveNext
        ///</summary>
        [TestMethod()]
        public void MoveNextTest()
        {
            Stats.SpecialEffectEnumerator target = new Stats.SpecialEffectEnumerator(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.MoveNext();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetEnumerator
        ///</summary>
        [TestMethod()]
        public void GetEnumeratorTest()
        {
            Stats.SpecialEffectEnumerator target = new Stats.SpecialEffectEnumerator(); // TODO: Initialize to an appropriate value
            Stats.SpecialEffectEnumerator expected = new Stats.SpecialEffectEnumerator(); // TODO: Initialize to an appropriate value
            Stats.SpecialEffectEnumerator actual;
            actual = target.GetEnumerator();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            Stats.SpecialEffectEnumerator target = new Stats.SpecialEffectEnumerator(); // TODO: Initialize to an appropriate value
            target.Dispose();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
         * 
         * 
         */
    }
}
