using Rawr;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Rawr.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for SpecialEffectsTest and is intended
    ///to contain all SpecialEffectsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SpecialEffectsTest
    {
        public static string[] m_TestLineArray = new string[13];
        public static Stats[] m_ExpectedArray = new Stats[13];

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
            int i = 0;
            Stats tempStat = new Stats();
            Stats elementStat = new Stats();
            //Sigil of the Dark Rider
            m_TestLineArray[i] = "Increases the damage dealt by your Blood Strike and Heart Strike by 90.";
            m_ExpectedArray[i] = new Stats();
            m_ExpectedArray[i].BonusBloodStrikeDamage = 90f;
            m_ExpectedArray[i].BonusHeartStrikeDamage = 90f;
            i++;

            // Sigil of the Unfaltering Knight:
            m_TestLineArray[i] = "Your Icy Touch will also increase your defense rating by 53.";
            tempStat = new Stats();
            elementStat = new Stats();
            elementStat.DefenseRating = 53;
            tempStat.AddSpecialEffect(new SpecialEffect(Trigger.IcyTouchHit, elementStat, 30f, 0));
            m_ExpectedArray[i] = tempStat;
            i++;

            // Furious Gladiator's Sigil of Strife
            m_TestLineArray[i] = "Your Plague Strike ability also grants you 144 attack power for 10 sec.";
            tempStat = new Stats();
            elementStat = new Stats();
            tempStat.AttackPower = 144;
            elementStat.AddSpecialEffect(new SpecialEffect(Trigger.PlagueStrikeHit, tempStat, 10f, 0));
            m_ExpectedArray[i] = elementStat;
            i++;

            // Sigil of Deflection
            m_TestLineArray[i] = "Your Rune Strike ability grants 136 dodge rating for 5 sec.";
            tempStat = new Stats();
            elementStat = new Stats();
            tempStat.DodgeRating = 136;
            elementStat.AddSpecialEffect(new SpecialEffect(Trigger.RuneStrikeHit, tempStat, 5f, 0));
            m_ExpectedArray[i] = elementStat;
            i++;

            // Sigil of the Vengeful Heart
            m_TestLineArray[i] = "Increases the damage done by your Death Coil and Frost Strike abilities by 380.";
            m_ExpectedArray[i] = new Stats();
            m_ExpectedArray[i].BonusDeathCoilDamage = 380f;
            m_ExpectedArray[i].BonusFrostStrikeDamage = 380f;
            i++;

            // Deadly Gladiator's Sigil of Strife
            m_TestLineArray[i] = "Your Plague Strike ability also grants you 120 attack power for 10 sec.";
            tempStat = new Stats();
            elementStat = new Stats();
            tempStat.AttackPower = 120;
            elementStat.AddSpecialEffect(new SpecialEffect(Trigger.PlagueStrikeHit, tempStat, 10f, 0));
            m_ExpectedArray[i] = elementStat;
            i++;

            //Sigil of Awareness
            m_TestLineArray[i] = "Increases the base damage dealt by your Scourge Strike by 189, your Obliterate by 336, and your Death Strike by 315.";
            m_ExpectedArray[i] = new Stats();
            m_ExpectedArray[i].BonusScourgeStrikeDamage = 189f;
            m_ExpectedArray[i].BonusObliterateDamage = 336f;
            m_ExpectedArray[i].BonusDeathStrikeDamage = 315f;
            i++;

            //Hateful Gladiator's Sigil of Strife
            m_TestLineArray[i] = "Your Plague Strike ability also grants you 106 attack power for 6 sec.";
            tempStat = new Stats();
            elementStat = new Stats();
            tempStat.AttackPower = 106;
            elementStat.AddSpecialEffect(new SpecialEffect(Trigger.PlagueStrikeHit, tempStat, 6f, 0));
            m_ExpectedArray[i] = elementStat;
            i++;

            //Sigil of Haunted Dreams
            m_TestLineArray[i] = "Your Blood Strike and Heart Strikes have a chance to grant 173 critical strike rating for 10 sec.";
            tempStat = new Stats();
            elementStat = new Stats();
            tempStat.CritRating = 173;
            elementStat.AddSpecialEffect(new SpecialEffect(Trigger.BloodStrikeHit, tempStat, 10f, 0f, 0.15f));
            elementStat.AddSpecialEffect(new SpecialEffect(Trigger.HeartStrikeHit, tempStat, 10f, 0f, 0.15f));
            m_ExpectedArray[i] = elementStat;
            i++;

            //Savage Gladiator's Sigil of Strife
            m_TestLineArray[i] = "Your Plague Strike ability also grants you 94 attack power for 6 sec.";
            tempStat = new Stats();
            elementStat = new Stats();
            tempStat.AttackPower = 94;
            elementStat.AddSpecialEffect(new SpecialEffect(Trigger.PlagueStrikeHit, tempStat, 6f, 0));
            m_ExpectedArray[i] = elementStat;
            i++;

            //Sigil of Arthritic Binding
            m_TestLineArray[i] = "Increases the damage dealt by your Scourge Strike ability by 91.35.";
            m_ExpectedArray[i] = new Stats();
            m_ExpectedArray[i].BonusScourgeStrikeDamage = 91.35f;
            i++;

            //Sigil of the Frozen Conscience
            m_TestLineArray[i] = "Increases the damage dealt by your Icy Touch ability by 111.";
            m_ExpectedArray[i] = new Stats();
            m_ExpectedArray[i].BonusIcyTouchDamage = 111f;
            i++;

            //Sigil of the Wild Buck
            m_TestLineArray[i] = "Increases the damage dealt by your Death Coil ability by 80.";
            m_ExpectedArray[i] = new Stats();
            m_ExpectedArray[i].BonusDeathCoilDamage = 80f;
            i++;
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
        ///A test for ProcessUseLine
        ///</summary>
/*
 *      [TestMethod()]
        public void ProcessUseLineTest()
        {
            string line = m_line; // TODO: Initialize to an appropriate value
            Stats stats = null; // TODO: Initialize to an appropriate value
            bool isArmory = false; // TODO: Initialize to an appropriate value
            int id = 0; // TODO: Initialize to an appropriate value
            SpecialEffects.ProcessUseLine(line, stats, isArmory, id);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ProcessMetaGem
        ///</summary>
        [TestMethod()]
        public void ProcessMetaGemTest()
        {
            string line = string.Empty; // TODO: Initialize to an appropriate value
            Stats stats = null; // TODO: Initialize to an appropriate value
            bool bisArmory = false; // TODO: Initialize to an appropriate value
            SpecialEffects.ProcessMetaGem(line, stats, bisArmory);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
        */

        /// <summary>
        ///A test for ProcessEquipLine
        ///</summary>
        [TestMethod()]
        public void ProcessEquipLineTest()
        {
            for(int m_i = 0; m_i < 13; m_i++)
            {
                string line = m_TestLineArray[m_i];
                Stats stats = new Stats();
                bool isArmory = false;
                SpecialEffects.ProcessEquipLine(line, stats, isArmory, 0, 0);
                string szExpected = m_ExpectedArray[m_i].ToString();
                string szStats = stats.ToString();
                Assert.AreEqual(szExpected, szStats, line);
            }
        }
    }
}
