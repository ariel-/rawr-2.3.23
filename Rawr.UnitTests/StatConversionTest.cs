using Rawr;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Rawr.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for StatConversionTest and is intended
    ///to contain all StatConversionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StatConversionTest
    {
        public static int HitResultCount = EnumHelper.GetCount(typeof(HitResult));

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
        /// Tests for GetDRAvoidanceChance
        /// http://www.tankspot.com/forums/f63/40003-diminishing-returns-avoidance.html
        /// The paper doll is correct. This can be verified very easily in game. So if dodge/parry are not matching up between 
        /// TankDK and in-game (with no buffs), then TankDK is incorrect.
        /// Important notes: Defense from Stoneskin Gargoyle is NOT subject to diminishing returns (meaning 1% dodge, 1% parry, 
        /// and 1% miss) in addition to 5% dodge from talents.
        /// This leaves Miss to be calculated separately: 5% base, 1% from SG rune, and 3% from talents are all NOT subject to 
        /// diminishing returns (9%), leaving only defense rating to contribute to DR miss. This means at 566 defense rating 
        /// (the DK cap with SG rune), you would expect to have 5.6% chance to miss from defense rating (+9% from talents/SG
        /// rune/base), or 14.6% chance to be missed. After diminishing returns, however, you would only have roughly 13.29% 
        /// total chance to be missed.
        /// NOTE: The miss cap has only been confirmed for Warriors, so diminishing returns on miss from defense rating for 
        /// DK's (and druids/paladins) is currently only able to be estimated based on the Warrior miss cap.
        /// Additional note: Night Elf racial 2% miss is also not subject to diminishing returns."
        /// "Slight accuracy correction: knock 0.6% off of the non-DR counts on all 3 stats due to the level difference on bosses.
        /// This brings 566 defense rating miss rate to roughly 13.1%, rather than 13.29%"
        /// </summary>
        [TestMethod()]
        public void GetDRAvoidanceChanceTest_DK_TestFromForums()
        {
            ItemInstance[] IIArray = new ItemInstance[1];
            Character toon = new Character("TestDK", "Malygos", CharacterRegion.US, CharacterRace.Human, new BossOptions(), IIArray, new System.Collections.Generic.List<Buff>(), "TankDK"); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(toon);
            //toon.Level = 80;  //Asumption here.
            toon.Class = CharacterClass.DeathKnight;

            Stats stats = new Stats();
            stats += BaseStats.GetBaseStats(toon);
            stats.Miss += .03f; // 3% Miss from talents
            stats.Defense = 425; // 25 Def from SSG rune
            stats.Dodge += .05f; // 5% dodge from talents.
            stats.DefenseRating = 566f;
            uint TargetLevel = 83;
            //float levelDiff = 0.006f;
            float[] expected = new float[HitResultCount];
            expected[(int)HitResult.Miss] = 0.121f;
            expected[(int)HitResult.Dodge] = 0.134f;
            expected[(int)HitResult.Parry] = 0.098f;
            expected[(int)HitResult.Block] = .05f;
            expected[(int)HitResult.Crit] = .05f;
            expected[(int)HitResult.AnyMiss] = 0;
            expected[(int)HitResult.AnyHit] = 0;
            expected[(int)HitResult.Glance] = .05f;
            expected[(int)HitResult.Resist] = .05f;
            expected[(int)HitResult.Hit] = .05f;

            // Iterate through the hit result types.
            for (HitResult i = 0; i < (HitResult)HitResultCount; i++)
            {
                float actual = (float)System.Math.Round((double)StatConversion.GetDRAvoidanceChance(toon, stats, i, TargetLevel), 3);
                Assert.AreEqual(expected[(int)i], actual, i.ToString());
            }
        }

        [TestMethod()]
        public void GetDRAvoidanceChanceTest_Warr_TestTableDef689()
        {
            const float testValue = 689f;
            ItemInstance[] IIArray = new ItemInstance[1];
            Character toon = new Character("TestWarrior", "Malygos", CharacterRegion.US, CharacterRace.Human, new BossOptions(), IIArray, new System.Collections.Generic.List<Buff>(), "ProtWar"); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(toon);
            //toon.Level = 80;  //Asumption here.
            toon.Class = CharacterClass.Warrior;

            Stats stats = new Stats();
            stats += BaseStats.GetBaseStats(toon);
            stats.Defense = 400;
            stats.DefenseRating = testValue;
            uint TargetLevel = 80;
            //float levelDiff = 0.006f;
            float[] expected = new float[HitResultCount];
            expected[(int)HitResult.Miss] = 0.0929f;
            expected[(int)HitResult.Dodge] = 0.1037f;
            expected[(int)HitResult.Parry] = 0.1021f;
            // Miss test
            float actual = (float)System.Math.Round((double)StatConversion.GetDRAvoidanceChance(toon, stats, HitResult.Miss, TargetLevel), 4);
            Assert.AreEqual(expected[(int)HitResult.Miss], actual, HitResult.Miss.ToString());

            actual = (float)System.Math.Round((double)StatConversion.GetDRAvoidanceChance(toon, stats, HitResult.Dodge, TargetLevel), 4);
            Assert.AreEqual(expected[(int)HitResult.Dodge], actual, HitResult.Dodge.ToString());

            actual = (float)System.Math.Round((double)StatConversion.GetDRAvoidanceChance(toon, stats, HitResult.Parry, TargetLevel), 4);
            Assert.AreEqual(expected[(int)HitResult.Parry], actual, HitResult.Parry.ToString());
        }

        [TestMethod()]
        public void GetDRAvoidanceChanceTest_Warr_TestTableDef10000()
        {
            const float testValue = 10000f;
            ItemInstance[] IIArray = new ItemInstance[1];
            Character toon = new Character("TestWarrior", "Malygos", CharacterRegion.US, CharacterRace.Human, new BossOptions(), IIArray, new System.Collections.Generic.List<Buff>(), "ProtWarr"); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(toon);
            //toon.Level = 80;  //Asumption here.
            toon.Class = CharacterClass.Warrior;

            Stats stats = new Stats();
            stats += BaseStats.GetBaseStats(toon);
            stats.Defense = 400;
            stats.DefenseRating = testValue;
            uint TargetLevel = 80;
            //float levelDiff = 0.006f;
            float[] expected = new float[HitResultCount];
            expected[(int)HitResult.Miss]  = 0.16f;
            expected[(int)HitResult.Dodge] = 0.7650f;
            expected[(int)HitResult.Parry] = 0.4700f;
            // Miss test
            float actual = (float)System.Math.Round((double)StatConversion.GetDRAvoidanceChance(toon, stats, HitResult.Miss, TargetLevel), 4);
            Assert.AreEqual(expected[(int)HitResult.Miss], actual, HitResult.Miss.ToString());
            // Dodge Test
            actual = (float)System.Math.Round((double)StatConversion.GetDRAvoidanceChance(toon, stats, HitResult.Dodge, TargetLevel), 4);
            Assert.AreEqual(expected[(int)HitResult.Dodge], actual, HitResult.Dodge.ToString());
            // Parry Test
            actual = (float)System.Math.Round((double)StatConversion.GetDRAvoidanceChance(toon, stats, HitResult.Parry, TargetLevel), 4);
            Assert.AreEqual(expected[(int)HitResult.Parry], actual, HitResult.Parry.ToString());
        }

        /// <summary>
        ///A test for GetDefenseFromRating
        ///</summary>
        [TestMethod()]
        public void GetDefenseFromRatingTest_zero()
        {
            float Rating = 0F; 
            float expected = 0F; 
            float actual;
            actual = StatConversion.GetDefenseFromRating(Rating);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetDefenseFromRating
        ///</summary>
        [TestMethod()]
        public void GetDefenseFromRatingTest_CritCap()
        {
            float Rating = 689F;
            float expected = 140F;
            float actual;
            actual = StatConversion.GetDefenseFromRating(Rating);
            Assert.AreEqual(expected, actual);
        }

        /* Commenting out the tests that aren't implemented yet.
         * 
         * 
        /// <summary>
        ///A test for GetCombatRatingModifier
        ///</summary>
        [TestMethod()]
        public void GetCombatRatingModifierTest()
        {
            // TODO:  (Shazear)
            // Setup an array that compares real values w/ expected values.
            uint iLevel = 0; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetCombatRatingModifier(iLevel);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetSpiritRegenSec
        ///</summary>
        [TestMethod()]
        public void GetSpiritRegenSecTest1()
        {
            float Spirit = 0F; // TODO: Initialize to an appropriate value
            float Intellect = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetSpiritRegenSec(Spirit, Intellect, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetSpiritRegenSec
        ///</summary>
        [TestMethod()]
        public void GetSpiritRegenSecTest()
        {
            float Spirit = 0F; // TODO: Initialize to an appropriate value
            float Intellect = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetSpiritRegenSec(Spirit, Intellect);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetSpellMiss
        ///</summary>
        [TestMethod()]
        public void GetSpellMissTest()
        {
            int LevelDelta = 0; // TODO: Initialize to an appropriate value
            bool bPvP = false; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetSpellMiss(LevelDelta, bPvP);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetSpellHitFromRating
        ///</summary>
        [TestMethod()]
        public void GetSpellHitFromRatingTest1()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetSpellHitFromRating(Rating);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetSpellHitFromRating
        ///</summary>
        [TestMethod()]
        public void GetSpellHitFromRatingTest()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetSpellHitFromRating(Rating, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetSpellHasteFromRating
        ///</summary>
        [TestMethod()]
        public void GetSpellHasteFromRatingTest1()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetSpellHasteFromRating(Rating, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetSpellHasteFromRating
        ///</summary>
        [TestMethod()]
        public void GetSpellHasteFromRatingTest()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetSpellHasteFromRating(Rating);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetSpellCritFromRating
        ///</summary>
        [TestMethod()]
        public void GetSpellCritFromRatingTest1()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetSpellCritFromRating(Rating, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetSpellCritFromRating
        ///</summary>
        [TestMethod()]
        public void GetSpellCritFromRatingTest()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetSpellCritFromRating(Rating);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetSpellCritFromIntellect
        ///</summary>
        [TestMethod()]
        public void GetSpellCritFromIntellectTest1()
        {
            float Intellect = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetSpellCritFromIntellect(Intellect);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetSpellCritFromIntellect
        ///</summary>
        [TestMethod()]
        public void GetSpellCritFromIntellectTest()
        {
            float Intellect = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetSpellCritFromIntellect(Intellect, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetResistanceTableString
        ///</summary>
        [TestMethod()]
        public void GetResistanceTableStringTest()
        {
            int AttackerLevel = 0; // TODO: Initialize to an appropriate value
            int TargetLevel = 0; // TODO: Initialize to an appropriate value
            float TargetResistance = 0F; // TODO: Initialize to an appropriate value
            float AttackerSpellPenetration = 0F; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = StatConversion.GetResistanceTableString(AttackerLevel, TargetLevel, TargetResistance, AttackerSpellPenetration);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetResistanceTable
        ///</summary>
        [TestMethod()]
        public void GetResistanceTableTest()
        {
            int AttackerLevel = 0; // TODO: Initialize to an appropriate value
            int TargetLevel = 0; // TODO: Initialize to an appropriate value
            float TargetResistance = 0F; // TODO: Initialize to an appropriate value
            float AttackerSpellPenetration = 0F; // TODO: Initialize to an appropriate value
            float[] expected = null; // TODO: Initialize to an appropriate value
            float[] actual;
            actual = StatConversion.GetResistanceTable(AttackerLevel, TargetLevel, TargetResistance, AttackerSpellPenetration);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetResilienceFromRating
        ///</summary>
        [TestMethod()]
        public void GetResilienceFromRatingTest1()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetResilienceFromRating(Rating, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetResilienceFromRating
        ///</summary>
        [TestMethod()]
        public void GetResilienceFromRatingTest()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetResilienceFromRating(Rating);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPhysicalHitFromRating
        ///</summary>
        [TestMethod()]
        public void GetPhysicalHitFromRatingTest1()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetPhysicalHitFromRating(Rating);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPhysicalHitFromRating
        ///</summary>
        [TestMethod()]
        public void GetPhysicalHitFromRatingTest()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetPhysicalHitFromRating(Rating, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPhysicalHasteFromRating
        ///</summary>
        [TestMethod()]
        public void GetPhysicalHasteFromRatingTest()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetPhysicalHasteFromRating(Rating, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPhysicalCritFromRating
        ///</summary>
        [TestMethod()]
        public void GetPhysicalCritFromRatingTest1()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetPhysicalCritFromRating(Rating);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPhysicalCritFromRating
        ///</summary>
        [TestMethod()]
        public void GetPhysicalCritFromRatingTest()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetPhysicalCritFromRating(Rating, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPhysicalCritFromAgility
        ///</summary>
        [TestMethod()]
        public void GetPhysicalCritFromAgilityTest()
        {
            float Agility = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetPhysicalCritFromAgility(Agility, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetParryFromRating
        ///</summary>
        [TestMethod()]
        public void GetParryFromRatingTest1()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetParryFromRating(Rating);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetParryFromRating
        ///</summary>
        [TestMethod()]
        public void GetParryFromRatingTest()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetParryFromRating(Rating, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetMinimumResistance
        ///</summary>
        [TestMethod()]
        public void GetMinimumResistanceTest()
        {
            int AttackerLevel = 0; // TODO: Initialize to an appropriate value
            int TargetLevel = 0; // TODO: Initialize to an appropriate value
            float TargetResistance = 0F; // TODO: Initialize to an appropriate value
            float AttackerSpellPenetration = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetMinimumResistance(AttackerLevel, TargetLevel, TargetResistance, AttackerSpellPenetration);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetHitFromRating
        ///</summary>
        [TestMethod()]
        public void GetHitFromRatingTest1()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetHitFromRating(Rating, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetHitFromRating
        ///</summary>
        [TestMethod()]
        public void GetHitFromRatingTest()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetHitFromRating(Rating);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetHasteFromRating
        ///</summary>
        [TestMethod()]
        public void GetHasteFromRatingTest()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetHasteFromRating(Rating, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetExpertiseFromRating
        ///</summary>
        [TestMethod()]
        public void GetExpertiseFromRatingTest1()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetExpertiseFromRating(Rating);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetExpertiseFromRating
        ///</summary>
        [TestMethod()]
        public void GetExpertiseFromRatingTest()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetExpertiseFromRating(Rating, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDodgeFromRating
        ///</summary>
        [TestMethod()]
        public void GetDodgeFromRatingTest1()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetDodgeFromRating(Rating);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDodgeFromRating
        ///</summary>
        [TestMethod()]
        public void GetDodgeFromRatingTest()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetDodgeFromRating(Rating, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDodgeFromAgility
        ///</summary>
        [TestMethod()]
        public void GetDodgeFromAgilityTest()
        {
            float Agility = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetDodgeFromAgility(Agility, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCritFromRating
        ///</summary>
        [TestMethod()]
        public void GetCritFromRatingTest1()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetCritFromRating(Rating);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCritFromRating
        ///</summary>
        [TestMethod()]
        public void GetCritFromRatingTest()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetCritFromRating(Rating, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCritFromAgility
        ///</summary>
        [TestMethod()]
        public void GetCritFromAgilityTest()
        {
            float Agility = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetCritFromAgility(Agility, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCombatRatingModifier
        ///</summary>
        [TestMethod()]
        public void GetCombatRatingModifierTest1()
        {
            uint iLevel = 0; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetCombatRatingModifier(iLevel);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetBlockFromRating
        ///</summary>
        [TestMethod()]
        public void GetBlockFromRatingTest1()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetBlockFromRating(Rating);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetBlockFromRating
        ///</summary>
        [TestMethod()]
        public void GetBlockFromRatingTest()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetBlockFromRating(Rating, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetAverageResistance
        ///</summary>
        [TestMethod()]
        public void GetAverageResistanceTest()
        {
            int AttackerLevel = 0; // TODO: Initialize to an appropriate value
            int TargetLevel = 0; // TODO: Initialize to an appropriate value
            float TargetResistance = 0F; // TODO: Initialize to an appropriate value
            float AttackerSpellPenetration = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetAverageResistance(AttackerLevel, TargetLevel, TargetResistance, AttackerSpellPenetration);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetArmorPenetrationFromRating
        ///</summary>
        [TestMethod()]
        public void GetArmorPenetrationFromRatingTest1()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetArmorPenetrationFromRating(Rating);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetArmorPenetrationFromRating
        ///</summary>
        [TestMethod()]
        public void GetArmorPenetrationFromRatingTest()
        {
            float Rating = 0F; // TODO: Initialize to an appropriate value
            CharacterClass Class = new CharacterClass(); // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetArmorPenetrationFromRating(Rating, Class);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetArmorDamageReduction
        ///</summary>
        [TestMethod()]
        public void GetArmorDamageReductionTest()
        {
            int AttackerLevel = 0; // TODO: Initialize to an appropriate value
            float TargetArmor = 0F; // TODO: Initialize to an appropriate value
            float ArmorIgnoreDebuffs = 0F; // TODO: Initialize to an appropriate value
            float ArmorIgnoreBuffs = 0F; // TODO: Initialize to an appropriate value
            float ArmorPenetrationRating = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion.GetArmorDamageReduction(AttackerLevel, TargetArmor, ArmorIgnoreDebuffs, ArmorIgnoreBuffs, ArmorPenetrationRating);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AttackerResistancePenalty
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Rawr.Base.dll")]
        public void AttackerResistancePenaltyTest()
        {
            int LevelDelta = 0; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = StatConversion_Accessor.AttackerResistancePenalty(LevelDelta);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
         * 
         */

    }
}

