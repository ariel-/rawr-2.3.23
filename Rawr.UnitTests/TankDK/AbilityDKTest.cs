using Rawr.TankDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.UnitTests.TankDK
{

    /// <summary>
    ///This is a test class for AbilityDKTest and is intended
    ///to contain all AbilityDKTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AbilityDKTest
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        }
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
        ///A test for AbilityDK Constructor Test
        ///</summary>
        [TestMethod()]
        public void AbilityDKConstructorTest()
        {
/*
 * AbilityDK_Base target = new AbilityDK_Base() as AbilityDK_Base;
            Assert.IsTrue(target.szName == "");
            Assert.IsTrue(target.AbilityCost[(int)DKCostTypes.Blood] == 0);
            Assert.IsTrue(target.AbilityCost[(int)DKCostTypes.Frost] == 0);
            Assert.IsTrue(target.AbilityCost[(int)DKCostTypes.UnHoly] == 0);
            Assert.IsTrue(target.AbilityCost[(int)DKCostTypes.RunicPower] == 0);
            Assert.IsTrue(target.uBaseDamage == 0);
            Assert.IsTrue(target.uRange == 5);
            Assert.IsTrue(target.tDamageType == ItemDamageType.Physical);
            Assert.IsTrue(target.Cooldown == 1500);
 */
        }

        /// <summary>
        /// A test for AbilityDK IcyTouch
        /// This test just creates IcyTouch Rank 5 and tests very basic assumptions about what's in the structure.
        /// This does *not* create Frost Fever.
        ///</summary>
        [TestMethod()]
        public void AbilityDKTest_IcyTouch_Basic()
        {   
            // TODO: Get this so we don't have to do this work every test.
            Stats characterStats = new Stats();
            characterStats = BaseStats.GetBaseStats(80, CharacterClass.DeathKnight, CharacterRace.Undead);

            CombatState state = new CombatState();
            state.m_Stats = characterStats;
            state.m_Talents = new DeathKnightTalents();
            
            // We're going to create IcyTouch and ensure that we can access everything for that 
            // ability that is used by a DK.  
            AbilityDK_IcyTouch IT = new AbilityDK_IcyTouch(state);
            
            // Frost Fever application. is skipped for the purposes of this test.
            Assert.IsTrue(IT.szName == "Icy Touch", "Name");
            Assert.AreEqual(IT.AbilityCost[(int)DKCostTypes.Blood], 0, "Blood Cost");
            Assert.AreEqual(IT.AbilityCost[(int)DKCostTypes.Frost], 1, "Frost Cost");
            Assert.AreEqual(IT.AbilityCost[(int)DKCostTypes.UnHoly], 0, "Unholy Cost");
            Assert.AreEqual(-10, IT.AbilityCost[(int)DKCostTypes.RunicPower], "RP Cost/Gain");
            Assert.AreEqual((uint)((245 + 227) / 2), IT.uBaseDamage, "Base Damage");
            Assert.AreEqual(20u, IT.uRange, "Range");
            Assert.AreEqual(IT.tDamageType, ItemDamageType.Frost, "Damage Type");
            Assert.AreEqual(IT.Cooldown, 1500u, "Cooldown");
            // With the addition of GetTotalDamage function we need to add it into the basic IT check.
            Assert.AreEqual((int)IT.uBaseDamage, IT.GetTotalDamage(), "GetTotalDamage");
        }

        /// <summary>
        /// A test for AbilityDK Frost Fever
        /// This test creates Frost Fever.
        ///</summary>
        [TestMethod()]
        public void AbilityDKTest_FF()
        {
            Stats FFTestStats = new Stats();
            FFTestStats.AttackPower = 100;

            CombatState state = new CombatState();
            state.m_Stats = FFTestStats;
            state.m_Talents = new DeathKnightTalents();

            // Frost Fever application.
            AbilityDK_FrostFever FF = new AbilityDK_FrostFever(state);
            // FF can be reapplied w/o any cooldown.
            // A disease dealing [0 + AP * 0.055 * 1.15] Frost damage every 3 sec and reducing the target's melee and ranged attack speed by 14% for 15 sec.  Caused by Icy Touch and other spells. 
            // Base damage 0
            // Bonus from attack power [AP * 0.055 * 1.15]
            // Needs AP passed in 
            // Needs to have base formula passed in as well.

            // Frost Fever Checking
            Assert.IsTrue(FF.szName == "Frost Fever");
            Assert.IsTrue(FF.AbilityCost[(int)DKCostTypes.Blood] == 0);
            Assert.IsTrue(FF.AbilityCost[(int)DKCostTypes.Frost] == 0);
            Assert.IsTrue(FF.AbilityCost[(int)DKCostTypes.UnHoly] == 0);
            Assert.IsTrue(FF.AbilityCost[(int)DKCostTypes.RunicPower] == 0);
            Assert.IsTrue(FF.uBaseDamage == 0);
            Assert.IsTrue(FF.tDamageType == ItemDamageType.Frost);
            Assert.IsTrue(FF.Cooldown == 0);
            Assert.IsTrue(FF.uDuration == 15000);
            Assert.IsTrue(FF.uTickRate == 3000);
            Assert.AreEqual((int)(FFTestStats.AttackPower * 0.055f * 1.15f), FF.GetTickDamage(), 0.1, "GetTickDamage");
            Assert.AreEqual((int)(FF.GetTickDamage() * (15 / 3)), FF.GetTotalDamage(), 0.1, "GetTotalDamage" );
        }

        /// <summary>
        /// A test for AbilityDK Plague Strike
        /// This test creates Plague Strike and Blood Plague.
        ///</summary>
        [TestMethod()]
        public void AbilityDKTest_PlagueStrike_BP()
        {
            // Needs AP passed in 
            Stats FFTestStats = new Stats();
            FFTestStats.AttackPower = 100;
            
            Item i = new Item("Test", ItemQuality.Common, ItemType.Dagger, 1, "", ItemSlot.MainHand, "", false, new Stats(), new Stats(), ItemSlot.None, ItemSlot.None, ItemSlot.None, 10, 20, ItemDamageType.Physical, 2, "");
            CalculationOptionsTankDK c = new CalculationOptionsTankDK();
            c.talents = new DeathKnightTalents();
            Weapon w = new Weapon(i, FFTestStats, c, 0f);

            CombatState combatState = new CombatState();
            combatState.m_Stats = FFTestStats;
            combatState.MH = w;
            combatState.m_Talents = c.talents;
            
            AbilityDK_PlagueStrike PS = new AbilityDK_PlagueStrike(combatState);

            // Blood Plauge application.
            AbilityDK_BloodPlague BP = new AbilityDK_BloodPlague(combatState);

            // A disease dealing [0 + AP * 0.055 * 1.15] Shadow damage every 3 sec . 
            // Base damage 0
            // Bonus from attack power [AP * 0.055 * 1.15]

            // Plague Strike Checking
            Assert.IsTrue(PS.szName == "Plague Strike", "Name");
            Assert.AreEqual(PS.AbilityCost[(int)DKCostTypes.Blood], 0, "Blood Rune");
            Assert.AreEqual(PS.AbilityCost[(int)DKCostTypes.Frost], 0, "Frost Rune");
            Assert.AreEqual(PS.AbilityCost[(int)DKCostTypes.UnHoly], 1, "UnHoly Rune");
            Assert.AreEqual(PS.AbilityCost[(int)DKCostTypes.RunicPower], -10, "RP");
            Assert.AreEqual(Math.Floor((378 + PS.wMH.damage) / 2), PS.uBaseDamage, "BaseDamage");
            Assert.AreEqual(Math.Floor((378 + PS.wMH.damage) / 2), PS.uBaseDamage, "Total Damage");
            Assert.AreEqual(PS.uRange, AbilityDK_Base.MELEE_RANGE, "Range");
            Assert.AreEqual(PS.tDamageType, ItemDamageType.Physical, "Damage Type");
            Assert.AreEqual(PS.Cooldown, 1500u, "Cooldown");

            // Blood Plague Checking
            Assert.IsTrue(BP.szName == "Blood Plague", "Name");
            Assert.AreEqual(BP.AbilityCost[(int)DKCostTypes.Blood], 0, "Blood Rune");
            Assert.AreEqual(BP.AbilityCost[(int)DKCostTypes.Frost], 0, "Frost Rune");
            Assert.AreEqual(BP.AbilityCost[(int)DKCostTypes.UnHoly], 0, "UnHoly Rune");
            Assert.AreEqual(BP.AbilityCost[(int)DKCostTypes.RunicPower], 0, "Runic Power");
            Assert.AreEqual(BP.uBaseDamage, 0u, "Damage");
            Assert.AreEqual(BP.tDamageType, ItemDamageType.Shadow, "Damage Type");
            // Not sure if this actually needs a Cooldown.
//            Assert.AreEqual(BP.Cooldown, 0u, "Cooldown");
            Assert.AreEqual(BP.uDuration, 15000u, "Duration");
            Assert.AreEqual(BP.uTickRate, 3000u, "TickRate");
            Assert.AreEqual((int)(FFTestStats.AttackPower * 0.055f * 1.15f), BP.GetTickDamage(), 0.1, "GetTickDamage");
            Assert.AreEqual((int)(BP.GetTickDamage() * (15 / 3)), BP.GetTotalDamage(), 0.1, "GetTotalDamage");
        }
    }
}
