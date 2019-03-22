using Rawr;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Rawr.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for SpecialEffectTest and is intended
    ///to contain all SpecialEffectTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SpecialEffectTest
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


        /// <summary>
        ///A test for GetAverageUptime
        ///</summary>
        [TestMethod()]
        public void GetAverageUptimeTest()
        {
            // test interpolation at proc chance = 100%
            Properties.GeneralSettings.Default.ProcEffectMode = 3;
            SpecialEffect.UpdateCalculationMode();
            SpecialEffect target = new SpecialEffect(Trigger.PhysicalCrit, new Stats() { AttackPower = 632 }, 10.0f, 45.0f, 1.0f); // TODO: Initialize to an appropriate value
            float triggerInterval = 1.39778626F;
            float triggerChance = 1.0F;
            float attackSpeed = 3.4F;
            float fightDuration = 300.0F;
            float expected = 70.0F / 300.0F;
            float actual;
            actual = target.GetAverageUptime(triggerInterval, triggerChance, attackSpeed, fightDuration);
            Assert.AreEqual(expected, actual);
        }
    }
}
