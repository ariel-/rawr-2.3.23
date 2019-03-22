using Rawr;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Rawr.zzTestzz
{
    
    
    /// <summary>
    ///This is a test class for StatsTest and is intended
    ///to contain all StatsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StatsTest
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

        /* Commenting out code that isn't implented yet.
         * 

        /// <summary>
        ///A test for Values
        ///</summary>
        [TestMethod()]
        public void ValuesTest()
        {
            Stats target = new Stats(); // TODO: Initialize to an appropriate value
            StatFilter filter = null; // TODO: Initialize to an appropriate value
            IDictionary<PropertyInfo, float> expected = null; // TODO: Initialize to an appropriate value
            IDictionary<PropertyInfo, float> actual;
            actual = target.Values(filter);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            Stats target = new Stats(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SpecialEffects
        ///</summary>
        [TestMethod()]
        public void SpecialEffectsTest1()
        {
            Stats target = new Stats(); // TODO: Initialize to an appropriate value
            Stats.SpecialEffectEnumerator expected = new Stats.SpecialEffectEnumerator(); // TODO: Initialize to an appropriate value
            Stats.SpecialEffectEnumerator actual;
            actual = target.SpecialEffects();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SpecialEffects
        ///</summary>
        [TestMethod()]
        public void SpecialEffectsTest()
        {
            Stats target = new Stats(); // TODO: Initialize to an appropriate value
            Predicate<SpecialEffect> match = null; // TODO: Initialize to an appropriate value
            Stats.SpecialEffectEnumerator expected = new Stats.SpecialEffectEnumerator(); // TODO: Initialize to an appropriate value
            Stats.SpecialEffectEnumerator actual;
            actual = target.SpecialEffects(match);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RemoveSpecialEffect
        ///</summary>
        [TestMethod()]
        public void RemoveSpecialEffectTest()
        {
            Stats target = new Stats(); // TODO: Initialize to an appropriate value
            SpecialEffect specialEffect = null; // TODO: Initialize to an appropriate value
            target.RemoveSpecialEffect(specialEffect);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

         * 
         * End of non-implented code
         * 
        */

#region Operator Tests

        /// <summary>
        ///A test for op_Subtraction
        ///</summary>
        [TestMethod()]
        public void op_SubtractionTest_DifferentAdditiveStat()
        {
            // Different stats.
            Stats a = new Stats();
            a.Agility = 100f;
            Stats s = new Stats();
            s.Strength = 100f;
            Stats expected = new Stats();
            expected.Agility = 100f;
            expected.Strength = -100f;
            Stats actual;
            // 100 Agi - 100 Str
            actual = (a - s);
            Assert.AreEqual(expected, actual);
            expected = new Stats();
            expected.Agility = -100f;
            expected.Strength = 100f;
            // 100 Str - 100 Agi
            actual = (s - a);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Subtraction
        ///</summary>
        [TestMethod()]
        public void op_SubtractionTest_SameAdditiveStat()
        {
            // Same stat.
            Stats a = new Stats();
            a.Agility = 100f;
            Stats b = new Stats();
            b.Agility = 100f;
            Stats expected = new Stats();
            expected.Agility = 0f;
            Stats actual;
            // 100 - 100 
            actual = (a - b);
            Assert.AreEqual(expected, actual);
            expected = new Stats();
            b.Agility = 50f;
            expected.Agility = 50f;
            // 100 - 50 
            actual = (a - b);
            Assert.AreEqual(expected, actual);
            expected.Agility = -50f;
            // 50 - 100 
            actual = (b - a);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Subtraction
        ///</summary>
        [TestMethod()]
        public void op_SubtractionTest_DifferentMultStat()
        {
            // Different stat.
            Stats a = new Stats();
            a.BaseArmorMultiplier = 1.5f;
            Stats b = new Stats();
            b.BonusArmorMultiplier = 2.0f;
            Stats expected = new Stats();
            expected.BaseArmorMultiplier = 1.5f;
            expected.BonusArmorMultiplier = ( -1f*(2f/3f) );
            Stats actual;
            // 1.5 baseArmor - 2.0 Bonus armor
            actual = (a - b);
            Assert.AreEqual(expected, actual);
            expected.BaseArmorMultiplier = -0.60f;  // Again... not sure if this value is accurate for these kinds of actions but don't have the comparative logic to know how to adjust it.
            expected.BonusArmorMultiplier = 2.0f;
            // 2.0 Bonus armor - 1.5 baseArmor 
            actual = (b - a);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Subtraction
        ///</summary>
        [TestMethod()]
        public void op_SubtractionTest_SameMultStat()
        {
            // Same stat.
            Stats a = new Stats();
            a.BaseArmorMultiplier = 0.25f;
            Stats b = new Stats();
            b.BaseArmorMultiplier = 0.25f;
            Stats expected = new Stats();
            expected.BaseArmorMultiplier = 0;
            Stats actual;
            actual = (a - b);
            Assert.AreEqual(expected, actual);
            // Same stat. Different values
            b.BaseArmorMultiplier = 0.1f;
            expected.BaseArmorMultiplier = 0.1363636363636363636363636363636f;
            actual = (a - b);
            Assert.AreEqual(expected, actual);
            // Same stat. Different values Different order
            expected.BaseArmorMultiplier = -0.12f;  // Negative Value? Why is that?
            actual = (b - a);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Multiply
        ///</summary>
        [TestMethod()]
        public void op_MultiplyTest_AdditiveStats()
        {
            // Single Stat Multiply by 0
            Stats a = new Stats();
            a.Agility = 100f;
            float b = 0F;
            Stats expected = new Stats();
            expected.Agility = 0f;
            Stats actual;
            actual = (a * b);
            Assert.AreEqual(expected, actual);
            // Multiple stats Multiply by 0
            a.Strength = 100f;
            expected.Agility = 0f;
            expected.Strength = 0f;
            actual = (a * b);
            Assert.AreEqual(expected, actual);

            // Single Stat Multiply by b < 1
            a = new Stats();
            expected = new Stats();
            a.Agility = 100f;
            b = 0.75F;
            expected.Agility = 75f;
            actual = (a * b);
            Assert.AreEqual(expected, actual);
            // Multiple stats  Multiply by b < 1
            a.Agility = 100f;
            a.Strength = 100f;
            expected.Agility = 75f;
            expected.Strength = 75f;
            actual = (a * b);
            Assert.AreEqual(expected, actual);

            // Single Stat Multiply by b > 1
            a = new Stats();
            expected = new Stats();
            a.Agility = 100f;
            b = 1.75F;
            expected.Agility = 175f;
            actual = (a * b);
            Assert.AreEqual(expected, actual);
            // Multiple stats  Multiply by b > 1
            a.Agility = 100f;
            a.Strength = 100f;
            expected.Agility = 175f;
            expected.Strength = 175f;
            actual = (a * b);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Multiply
        ///</summary>
        [TestMethod()]
        public void op_MultiplyTest_MultiplitiveStats()
        {
            // Single Stat Multiply by 0
            Stats a = new Stats();
            a.BaseArmorMultiplier = 1.5f;
            float b = 0F;
            Stats expected = new Stats();
            expected.BaseArmorMultiplier = 0f;
            Stats actual;
            actual = (a * b);
            Assert.AreEqual(expected, actual);
            // Multiple stats Multiply by 0
            a.BaseArmorMultiplier = 1.5f;
            a.BonusArmorMultiplier = 2.5f;
            expected.BaseArmorMultiplier = 0f;
            expected.BonusArmorMultiplier = 0f;
            actual = (a * b);
            Assert.AreEqual(expected, actual);

            // Single Stat Multiply by b < 1
            a = new Stats();
            expected = new Stats();
            a.BaseArmorMultiplier = 1.5f;
            b = 0.75F;
            expected.BaseArmorMultiplier = 1.125f;
            actual = (a * b);
            Assert.AreEqual(expected, actual);
            // Multiple stats  Multiply by b < 1
            a.BaseArmorMultiplier = 1.5f;
            a.BonusArmorMultiplier = 2.5f;
            expected.BaseArmorMultiplier = 1.125f;
            expected.BonusArmorMultiplier = 1.875f;
            actual = (a * b);
            Assert.AreEqual(expected, actual);

            // Single Stat Multiply by b > 1
            a = new Stats();
            expected = new Stats();
            a.BaseArmorMultiplier = 1.5f;
            b = 1.25F;
            expected.BaseArmorMultiplier = 1.875f;
            actual = (a * b);
            Assert.AreEqual(expected, actual);
            // Multiple stats  Multiply by b > 1
            a.BaseArmorMultiplier = 1.5f;
            a.BonusArmorMultiplier = 2.5f;
            expected.BaseArmorMultiplier = 1.875f;
            expected.BonusArmorMultiplier = 3.125f;
            actual = (a * b);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for op_LessThanOrEqual
        ///</summary>
        [TestMethod()]
        public void op_LessThanOrEqualTest_EmptyStats()
        {
            Stats x = new Stats();
            Stats y = new Stats();
            bool expected = true;
            bool actual;
            actual = (x <= y);
            Assert.AreEqual(expected, actual);
            actual = (y <= x);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_LessThanOrEqual
        ///</summary>
        [TestMethod()]
        public void op_LessThanOrEqualTest_SameStat()
        {
            // Same Stat, equal value.
            Stats x = new Stats();
            x.Strength = 15f;
            Stats y = new Stats();
            y.Strength = 15f;
            bool expected = true;
            bool actual;
            actual = (x <= y);
            Assert.AreEqual(expected, actual);
            actual = (y <= x);
            Assert.AreEqual(expected, actual);
            // Same Stat, inequal value.
            x.Strength = 10f;
            actual = (x <= y);
            Assert.AreEqual(expected, actual);
            expected = false;
            actual = (y <= x);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_LessThanOrEqual
        ///</summary>
        [TestMethod()]
        public void op_LessThanOrEqualTest_DifferentStat()
        {
            // Different Stat, equal value.
            Stats x = new Stats();
            x.Strength = 15f;
            Stats y = new Stats();
            y.Agility = 15f;
            bool expected = false;
            bool actual;
            actual = (x <= y);
            Assert.AreEqual(expected, actual);
            actual = (y <= x);
            Assert.AreEqual(expected, actual);
            // Different Stat, inequal value.
            x.Strength = 10f;
            actual = (x <= y);
            Assert.AreEqual(expected, actual);
            actual = (y <= x);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_LessThan
        ///</summary>
        [TestMethod()]
        public void op_LessThanTest_EmptyStats()
        {
            Stats x = new Stats();
            Stats y = new Stats();
            bool expected = false;
            bool actual;
            actual = (x < y);
            Assert.AreEqual(expected, actual);
            actual = (y < x);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_LessThan
        ///</summary>
        [TestMethod()]
        public void op_LessThanTest_SameStat()
        {
            // Same Stat, equal value.
            Stats x = new Stats();
            x.Strength = 15f;
            Stats y = new Stats();
            y.Strength = 15f;
            bool expected = false;
            bool actual;
            actual = (x < y);
            Assert.AreEqual(expected, actual);
            actual = (y < x);
            Assert.AreEqual(expected, actual);
            // Same Stat, inequal value.
            x.Strength = 10f;
            // (15 str < 10 str) == false
            actual = (y < x);
            Assert.AreEqual(expected, actual);
            expected = true;
            // (10 str < 15 str) == true
            actual = (x < y);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_LessThan
        ///</summary>
        [TestMethod()]
        public void op_LessThanTest_DifferentStat()
        {
            // Different Stat, equal value.
            Stats x = new Stats();
            x.Strength = 15f;
            Stats y = new Stats();
            y.Agility = 15f;
            bool expected = false;
            bool actual;
            actual = (x < y);
            Assert.AreEqual(expected, actual);
            actual = (y < x);
            Assert.AreEqual(expected, actual);
            // Different Stat, inequal value.
            x.Strength = 10f;
            actual = (x < y);
            Assert.AreEqual(expected, actual);
            actual = (y < x);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Inequality
        ///</summary>
        [TestMethod()]
        public void op_InequalityTest_Null()
        {
            // Null vs. Null
            Stats x = null;
            Stats y = null; 
            bool expected = false; 
            bool actual;
            actual = (x != y);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Inequality
        ///</summary>
        [TestMethod()]
        public void op_InequalityTest_notNull()
        {
            // Empty stats vs. Null
            Stats x = new Stats();
            Stats y = null;
            bool expected = true;
            bool actual;
            actual = (x != y);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Inequality
        ///</summary>
        [TestMethod()]
        public void op_InequalityTest_empty()
        {
            // Empty stats vs. empty stats
            Stats x = new Stats();
            Stats y = new Stats();
            bool expected = false;
            bool actual;
            actual = (x != y);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Inequality
        ///</summary>
        [TestMethod()]
        public void op_InequalityTest_DiffStats()
        {
            // different base stats.
            Stats x = new Stats();
            x.Stamina = 1f;
            Stats y = new Stats();
            y.Intellect = 1f; 
            bool expected = true;
            bool actual;
            actual = (x != y);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Inequality
        ///</summary>
        [TestMethod()]
        public void op_InequalityTest_SameStats()
        {
            // different base stats.
            Stats x = new Stats();
            x.Stamina = 1f;
            Stats y = new Stats();
            y.Stamina = 1f;
            bool expected = false;
            bool actual;
            actual = (x != y);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_GreaterThanOrEqual
        ///</summary>
        [TestMethod()]
        public void op_GreaterThanOrEqualTest_EmptyStat()
        {
            Stats x = new Stats();
            Stats y = new Stats();
            bool expected = true;
            bool actual;
            actual = (x >= y);
            Assert.AreEqual(expected, actual);
            actual = (y >= x);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_GreaterThanOrEqual
        ///</summary>
        [TestMethod()]
        public void op_GreaterThanOrEqualTest_SameStat()
        {
            // Same Stat, equal value.
            Stats x = new Stats();
            x.Strength = 15f;
            Stats y = new Stats();
            y.Strength = 15f;
            bool expected = true;
            bool actual;
            actual = (x >= y);
            Assert.AreEqual(expected, actual);
            actual = (y >= x);
            Assert.AreEqual(expected, actual);
            // Same Stat, inequal value.
            x.Strength = 10f;
            actual = (y >= x);
            Assert.AreEqual(expected, actual);
            expected = false;
            actual = (x >= y);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_GreaterThanOrEqual
        ///</summary>
        [TestMethod()]
        public void op_GreaterThanOrEqualTest_DifferentStat()
        {
            // Different Stat, equal value.
            Stats x = new Stats();
            x.Strength = 15f;
            Stats y = new Stats();
            y.Agility = 15f;
            bool expected = false;
            bool actual;
            actual = (x >= y);
            Assert.AreEqual(expected, actual);
            actual = (y >= x);
            Assert.AreEqual(expected, actual);
            // Different Stat, inequal value.
            x.Strength = 10f;
            actual = (y >= x);
            Assert.AreEqual(expected, actual);
            actual = (x >= y);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_GreaterThan
        ///</summary>
        [TestMethod()]
        public void op_GreaterThanTest_EmptyStat()
        {
            Stats x = new Stats();
            Stats y = new Stats();
            bool expected = false;
            bool actual;
            actual = (x > y);
            Assert.AreEqual(expected, actual);
            actual = (y > x);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_GreaterThan
        ///</summary>
        [TestMethod()]
        public void op_GreaterThanTest_SameStat()
        {
            // Same Stat, equal value.
            Stats x = new Stats();
            x.Strength = 15f;
            Stats y = new Stats();
            y.Strength = 15f;
            bool expected = false;
            bool actual;
            // 15 > 15 == false
            actual = (x > y);
            Assert.AreEqual(expected, actual);
            // 15 > 15 == false
            actual = (y > x);
            Assert.AreEqual(expected, actual);
            // Same Stat, inequal value.
            // 10 > 15 == false
            actual = (x > y);
            Assert.AreEqual(expected, actual);
            expected = true;
            x.Strength = 10f;
            // 15 > 10 == true
            actual = (y > x);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_GreaterThan
        ///</summary>
        [TestMethod()]
        public void op_GreaterThanTest_DifferentStat()
        {
            // Different Stat, equal value.
            Stats x = new Stats();
            x.Strength = 15f;
            Stats y = new Stats();
            y.Agility = 15f;
            bool expected = false;
            bool actual;
            actual = (x > y);
            Assert.AreEqual(expected, actual);
            actual = (y > x);
            Assert.AreEqual(expected, actual);
            // Different Stat, inequal value.
            x.Strength = 10f;
            // 10 agi > 15 str == false
            actual = (x > y);
            Assert.AreEqual(expected, actual);
            // 15 str > 10 agi == false
            actual = (y > x);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Equality
        ///</summary>
        [TestMethod()]
        public void op_EqualityTest_null()
        {
            Stats x = null; 
            Stats y = null; 
            bool expected = true; 
            bool actual;
            actual = (x == y);
            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for op_Equality
        ///</summary>
        [TestMethod()]
        public void op_EqualityTest_notNull()
        {
            Stats x = new Stats();
            Stats y = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = (x == y);
            Assert.AreEqual(expected, actual);
            actual = (y == x);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Equality
        ///</summary>
        [TestMethod()]
        public void op_EqualityTest_emptyStats()
        {
            Stats x = new Stats(); 
            Stats y = new Stats(); 
            bool expected = true; 
            bool actual;
            actual = (x == y);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Equality
        ///</summary>
        [TestMethod()]
        public void op_EqualityTest_basicStats()
        {
            Stats x = new Stats();
            Stats y = new Stats();
            // Different initial stats.
            x.Agility = 100f;
            y.Strength = 100f;
            bool expected = false;
            bool actual;
            actual = (x == y);
            Assert.AreEqual(expected, actual);
            // same basic stats.
            expected = true;
            x.Agility = 0f;
            x.Strength = 100f;
            actual = (x == y);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod()]
        public void op_AdditionTest_SameAdditiveStat()
        {
            // Same stat.
            Stats a = new Stats();
            a.Agility = 100f;
            Stats b = new Stats();
            b.Agility = 100f;
            Stats expected = new Stats();
            expected.Agility = 200f;
            Stats actual;
            actual = (a + b);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod()]
        public void op_AdditionTest_DifferentAdditiveStat()
        {
            // Different stats.
            Stats a = new Stats();
            a.Agility = 100f;
            Stats b = new Stats();
            b.Strength = 100f;
            Stats expected = new Stats();
            expected.Agility = 100f;
            expected.Strength = 100f;
            Stats actual;
            actual = (a + b);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod()]
        public void op_AdditionTest_SameMultStat()
        {
            // Same stat.
            Stats a = new Stats();
            a.BaseArmorMultiplier = 0.25f;
            Stats b = new Stats();
            b.BaseArmorMultiplier = 0.25f;
            Stats expected = new Stats();
            expected.BaseArmorMultiplier = 0.5625f;
            Stats actual;
            actual = (a + b);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod()]
        public void op_AdditionTest_DifferentMultStat()
        {
            // Different stat.
            Stats a = new Stats();
            a.BaseArmorMultiplier = 1.5f;
            Stats b = new Stats();
            b.BonusArmorMultiplier = 2.0f;
            Stats expected = new Stats();
            expected.BaseArmorMultiplier = 1.5f;
            expected.BonusArmorMultiplier = 2.0f;
            Stats actual;
            actual = (a + b);
            Assert.AreEqual(expected, actual);
        }

#endregion // Operator Stats

        /* Commenting out the tests that aren't implemented yet.
         * 
         * 
        /// <summary>
        ///A test for IsPercentage
        ///</summary>
        [TestMethod()]
        public void IsPercentageTest()
        {
            PropertyInfo info = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = Stats.IsPercentage(info);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InvalidateSparseData
        ///</summary>
        [TestMethod()]
        public void InvalidateSparseDataTest()
        {
            Stats target = new Stats(); // TODO: Initialize to an appropriate value
            target.InvalidateSparseData();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetHashCode
        ///</summary>
        [TestMethod()]
        public void GetHashCodeTest()
        {
            Stats target = new Stats(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.GetHashCode();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GenerateSparseData
        ///</summary>
        [TestMethod()]
        public void GenerateSparseDataTest()
        {
            Stats target = new Stats(); // TODO: Initialize to an appropriate value
            target.GenerateSparseData();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest1()
        {
            Stats target = new Stats(); // TODO: Initialize to an appropriate value
            object obj = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Equals(obj);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            Stats target = new Stats(); // TODO: Initialize to an appropriate value
            Stats other = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Equals(other);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EnsureSpecialEffectCapacity
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Rawr.Base.dll")]
        public void EnsureSpecialEffectCapacityTest()
        {
            Stats_Accessor target = new Stats_Accessor(); // TODO: Initialize to an appropriate value
            int min = 0; // TODO: Initialize to an appropriate value
            target.EnsureSpecialEffectCapacity(min);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ContainsSpecialEffect
        ///</summary>
        [TestMethod()]
        public void ContainsSpecialEffectTest1()
        {
            Stats target = new Stats(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ContainsSpecialEffect();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ContainsSpecialEffect
        ///</summary>
        [TestMethod()]
        public void ContainsSpecialEffectTest()
        {
            Stats target = new Stats(); // TODO: Initialize to an appropriate value
            Predicate<SpecialEffect> match = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ContainsSpecialEffect(match);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CompareTo
        ///</summary>
        [TestMethod()]
        public void CompareToTest()
        {
            Stats target = new Stats(); // TODO: Initialize to an appropriate value
            Stats other = null; // TODO: Initialize to an appropriate value
            ArrayUtils.CompareResult expected = new ArrayUtils.CompareResult(); // TODO: Initialize to an appropriate value
            ArrayUtils.CompareResult actual;
            actual = target.CompareTo(other);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Clone
        ///</summary>
        [TestMethod()]
        public void CloneTest()
        {
            Stats target = new Stats(); // TODO: Initialize to an appropriate value
            Stats expected = null; // TODO: Initialize to an appropriate value
            Stats actual;
            actual = target.Clone();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AllCompare
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Rawr.Base.dll")]
        public void AllCompareTest()
        {
            Stats x = null; // TODO: Initialize to an appropriate value
            Stats y = null; // TODO: Initialize to an appropriate value
            ArrayUtils.CompareOption comparison = new ArrayUtils.CompareOption(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = Stats_Accessor.AllCompare(x, y, comparison);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AddSpecialEffect
        ///</summary>
        [TestMethod()]
        public void AddSpecialEffectTest()
        {
            Stats target = new Stats(); // TODO: Initialize to an appropriate value
            SpecialEffect specialEffect = null; // TODO: Initialize to an appropriate value
            target.AddSpecialEffect(specialEffect);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
         * 
         * End of non-implemented section.
         * 
        */

    }
}
