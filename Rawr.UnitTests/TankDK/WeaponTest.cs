using Rawr.TankDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rawr;

namespace Rawr.UnitTests.TankDK
{
    
    
    /// <summary>
    ///This is a test class for WeaponTest and is intended
    ///to contain all WeaponTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WeaponTest
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
        ///A test for Weapon Constructor
        ///</summary>
        [TestMethod()]
        public void WeaponConstructorTest()
        {
            // Weapon takes an item object.  
            // Refactor idea is for it to inherit from Item.
            Item i = new Item("Weapon Test", ItemQuality.Common, ItemType.OneHandMace, 101, null, ItemSlot.MainHand, null, false, null, null, ItemSlot.None, ItemSlot.None, ItemSlot.None, 10, 20, ItemDamageType.Physical, 2.0f, null);  
            Stats stats = new Stats();
            stats.Stamina = 100;
            CalculationOptionsTankDK calcOpts = new CalculationOptionsTankDK();
            calcOpts.talents = new DeathKnightTalents();
            float expertise = 0F; 
            Weapon target = new Weapon(i, stats, calcOpts, expertise);
            Assert.IsNotNull(target);
            Assert.AreNotEqual(0, target.baseDamage, "basedamage");
            Assert.AreNotEqual(0, target.damage, "adjusted damage");
            Assert.AreNotEqual(0, target.DPS, "DPS");
        }
    }
}
