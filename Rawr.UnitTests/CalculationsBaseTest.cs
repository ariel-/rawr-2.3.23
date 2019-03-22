using Rawr;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;
using System.Drawing;

namespace Rawr.zzTestzz
{
    
    
    /// <summary>
    ///This is a test class for CalculationsBaseTest and is intended
    ///to contain all CalculationsBaseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CalculationsBaseTest
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
        ///A test for TargetClass
        ///</summary>
        [TestMethod()]
        public void TargetClassTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            CharacterClass actual;
            actual = target.TargetClass;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SupportsMultithreading
        ///</summary>
        [TestMethod()]
        public void SupportsMultithreadingTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.SupportsMultithreading;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SubPointNameColors
        ///</summary>
        [TestMethod()]
        public void SubPointNameColorsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Dictionary<string, Color> actual;
            actual = target.SubPointNameColors;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RelevantItemTypes
        ///</summary>
        [TestMethod()]
        public void RelevantItemTypesTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            List<ItemType> actual;
            actual = target.RelevantItemTypes;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for OptimizableCalculationLabels
        ///</summary>
        [TestMethod()]
        public void OptimizableCalculationLabelsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            string[] actual;
            actual = target.OptimizableCalculationLabels;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DefaultGemmingTemplates
        ///</summary>
        [TestMethod()]
        public void DefaultGemmingTemplatesTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            List<GemmingTemplate> actual;
            actual = target.DefaultGemmingTemplates;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CustomRenderedChartNames
        ///</summary>
        [TestMethod()]
        public void CustomRenderedChartNamesTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            string[] actual;
            actual = target.CustomRenderedChartNames;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CustomChartNames
        ///</summary>
        [TestMethod()]
        public void CustomChartNamesTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            string[] actual;
            actual = target.CustomChartNames;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CharacterDisplayCalculationLabels
        ///</summary>
        [TestMethod()]
        public void CharacterDisplayCalculationLabelsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            string[] actual;
            actual = target.CharacterDisplayCalculationLabels;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CanUseAmmo
        ///</summary>
        [TestMethod()]
        public void CanUseAmmoTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.CanUseAmmo;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CalculationOptionsPanel
        ///</summary>
        [TestMethod()]
        public void CalculationOptionsPanelTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            CalculationOptionsPanelBase actual;
            actual = target.CalculationOptionsPanel;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CachedCharacter
        ///</summary>
        [TestMethod()]
        public void CachedCharacterTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Character actual;
            actual = target.CachedCharacter;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetDefaults
        ///</summary>
        [TestMethod()]
        public void SetDefaultsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            target.SetDefaults(character);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for RenderCustomChart
        ///</summary>
        [TestMethod()]
        public void RenderCustomChartTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            string chartName = string.Empty; // TODO: Initialize to an appropriate value
            Graphics g = null; // TODO: Initialize to an appropriate value
            int width = 0; // TODO: Initialize to an appropriate value
            int height = 0; // TODO: Initialize to an appropriate value
            target.RenderCustomChart(character, chartName, g, width, height);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for RemoveConflictingBuffs
        ///</summary>
        [TestMethod()]
        public void RemoveConflictingBuffsTest()
        {
            List<Buff> activeBuffs = null; // TODO: Initialize to an appropriate value
            Buff buff = null; // TODO: Initialize to an appropriate value
            CalculationsBase.RemoveConflictingBuffs(activeBuffs, buff);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for PropertyValueIsContinuous
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Rawr.Base.dll")]
        public void PropertyValueIsContinuousTest()
        {
            Character character = null; // TODO: Initialize to an appropriate value
            float basePoints = 0F; // TODO: Initialize to an appropriate value
            PropertyInfo property = null; // TODO: Initialize to an appropriate value
            Item tagItem = null; // TODO: Initialize to an appropriate value
            float resolution = 0F; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = CalculationsBase_Accessor.PropertyValueIsContinuous(character, basePoints, property, tagItem, resolution);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ItemFitsInSlot
        ///</summary>
        [TestMethod()]
        public void ItemFitsInSlotTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Item item = null; // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            CharacterSlot slot = new CharacterSlot(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ItemFitsInSlot(item, character, slot);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsItemRelevant
        ///</summary>
        [TestMethod()]
        public void IsItemRelevantTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Item item = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsItemRelevant(item);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsEnchantRelevant
        ///</summary>
        [TestMethod()]
        public void IsEnchantRelevantTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Enchant enchant = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsEnchantRelevant(enchant);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsBuffRelevant
        ///</summary>
        [TestMethod()]
        public void IsBuffRelevantTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Buff buff = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsBuffRelevant(buff);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IncludeOffHandInCalculations
        ///</summary>
        [TestMethod()]
        public void IncludeOffHandInCalculationsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IncludeOffHandInCalculations(character);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for HasRelevantStats
        ///</summary>
        [TestMethod()]
        public void HasRelevantStatsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Stats stats = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.HasRelevantStats(stats);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetStatValueUpperChangePoint
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Rawr.Base.dll")]
        public void GetStatValueUpperChangePointTest()
        {
            Character character = null; // TODO: Initialize to an appropriate value
            float basePoints = 0F; // TODO: Initialize to an appropriate value
            PropertyInfo property = null; // TODO: Initialize to an appropriate value
            Item tagItem = null; // TODO: Initialize to an appropriate value
            float lowerBound = 0F; // TODO: Initialize to an appropriate value
            float upperBound = 0F; // TODO: Initialize to an appropriate value
            float resolution = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = CalculationsBase_Accessor.GetStatValueUpperChangePoint(character, basePoints, property, tagItem, lowerBound, upperBound, resolution);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetStatValueLowerChangePoint
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Rawr.Base.dll")]
        public void GetStatValueLowerChangePointTest()
        {
            Character character = null; // TODO: Initialize to an appropriate value
            float basePoints = 0F; // TODO: Initialize to an appropriate value
            PropertyInfo property = null; // TODO: Initialize to an appropriate value
            Item tagItem = null; // TODO: Initialize to an appropriate value
            float lowerBound = 0F; // TODO: Initialize to an appropriate value
            float upperBound = 0F; // TODO: Initialize to an appropriate value
            float resolution = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = CalculationsBase_Accessor.GetStatValueLowerChangePoint(character, basePoints, property, tagItem, lowerBound, upperBound, resolution);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetRelevantStats
        ///</summary>
        [TestMethod()]
        public void GetRelevantStatsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Stats stats = null; // TODO: Initialize to an appropriate value
            Stats expected = null; // TODO: Initialize to an appropriate value
            Stats actual;
            actual = target.GetRelevantStats(stats);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetRelevantGlyphs
        ///</summary>
        [TestMethod()]
        public void GetRelevantGlyphsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            List<string> expected = null; // TODO: Initialize to an appropriate value
            List<string> actual;
            actual = target.GetRelevantGlyphs();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetRelativeStatValues
        ///</summary>
        [TestMethod()]
        public void GetRelativeStatValuesTest()
        {
            Character character = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase[] expected = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase[] actual;
            actual = CalculationsBase.GetRelativeStatValues(character);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetRelativeStatValue
        ///</summary>
        [TestMethod()]
        public void GetRelativeStatValueTest()
        {
            Character character = null; // TODO: Initialize to an appropriate value
            PropertyInfo property = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase expected = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase actual;
            actual = CalculationsBase.GetRelativeStatValue(character, property);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetItemStats
        ///</summary>
        [TestMethod()]
        public void GetItemStatsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            Item additionalItem = null; // TODO: Initialize to an appropriate value
            Stats expected = null; // TODO: Initialize to an appropriate value
            Stats actual;
            actual = target.GetItemStats(character, additionalItem);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetItemCalculations
        ///</summary>
        [TestMethod()]
        public void GetItemCalculationsTest1()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Item additionalItem = null; // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            CharacterSlot slot = new CharacterSlot(); // TODO: Initialize to an appropriate value
            ComparisonCalculationBase expected = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase actual;
            actual = target.GetItemCalculations(additionalItem, character, slot);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetItemCalculations
        ///</summary>
        [TestMethod()]
        public void GetItemCalculationsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            ItemInstance item = null; // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            CharacterSlot slot = new CharacterSlot(); // TODO: Initialize to an appropriate value
            ComparisonCalculationBase expected = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase actual;
            actual = target.GetItemCalculations(item, character, slot);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetEnchantCalculations
        ///</summary>
        [TestMethod()]
        public void GetEnchantCalculationsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            ItemSlot slot = new ItemSlot(); // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase currentCalcs = null; // TODO: Initialize to an appropriate value
            List<ComparisonCalculationBase> expected = null; // TODO: Initialize to an appropriate value
            List<ComparisonCalculationBase> actual;
            actual = target.GetEnchantCalculations(slot, character, currentCalcs);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCustomChartData
        ///</summary>
        [TestMethod()]
        public void GetCustomChartDataTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            string chartName = string.Empty; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase[] expected = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase[] actual;
            actual = target.GetCustomChartData(character, chartName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterStatsString
        ///</summary>
        [TestMethod()]
        public void GetCharacterStatsStringTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetCharacterStatsString(character);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterStats
        ///</summary>
        [TestMethod()]
        public void GetCharacterStatsTest1()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            Item additionalItem = null; // TODO: Initialize to an appropriate value
            Stats expected = null; // TODO: Initialize to an appropriate value
            Stats actual;
            actual = target.GetCharacterStats(character, additionalItem);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterStats
        ///</summary>
        [TestMethod()]
        public void GetCharacterStatsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            Stats expected = null; // TODO: Initialize to an appropriate value
            Stats actual;
            actual = target.GetCharacterStats(character);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterComparisonCalculations
        ///</summary>
        [TestMethod()]
        public void GetCharacterComparisonCalculationsTest2()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            CharacterCalculationsBase calculations = null; // TODO: Initialize to an appropriate value
            string name = string.Empty; // TODO: Initialize to an appropriate value
            bool equipped = false; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase expected = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase actual;
            actual = target.GetCharacterComparisonCalculations(calculations, name, equipped);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterComparisonCalculations
        ///</summary>
        [TestMethod()]
        public void GetCharacterComparisonCalculationsTest1()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            CharacterCalculationsBase baseCalculations = null; // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            string name = string.Empty; // TODO: Initialize to an appropriate value
            bool equipped = false; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase expected = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase actual;
            actual = target.GetCharacterComparisonCalculations(baseCalculations, character, name, equipped);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterComparisonCalculations
        ///</summary>
        [TestMethod()]
        public void GetCharacterComparisonCalculationsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            CharacterCalculationsBase baseCalculation = null; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase newCalculation = null; // TODO: Initialize to an appropriate value
            string name = string.Empty; // TODO: Initialize to an appropriate value
            bool equipped = false; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase expected = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase actual;
            actual = target.GetCharacterComparisonCalculations(baseCalculation, newCalculation, name, equipped);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterCalculations
        ///</summary>
        [TestMethod()]
        public void GetCharacterCalculationsTest2()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            Item additionalItem = null; // TODO: Initialize to an appropriate value
            bool referenceCalculation = false; // TODO: Initialize to an appropriate value
            bool significantChange = false; // TODO: Initialize to an appropriate value
            bool needsDisplayCalculations = false; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase expected = null; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase actual;
            actual = target.GetCharacterCalculations(character, additionalItem, referenceCalculation, significantChange, needsDisplayCalculations);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterCalculations
        ///</summary>
        [TestMethod()]
        public void GetCharacterCalculationsTest1()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase expected = null; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase actual;
            actual = target.GetCharacterCalculations(character);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterCalculations
        ///</summary>
        [TestMethod()]
        public void GetCharacterCalculationsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            Item additionalItem = null; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase expected = null; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase actual;
            actual = target.GetCharacterCalculations(character, additionalItem);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetBuffsStats
        ///</summary>
        [TestMethod()]
        public void GetBuffsStatsTest1()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            List<string> buffs = null; // TODO: Initialize to an appropriate value
            Stats expected = null; // TODO: Initialize to an appropriate value
            Stats actual;
            actual = target.GetBuffsStats(buffs);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetBuffsStats
        ///</summary>
        [TestMethod()]
        public void GetBuffsStatsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            List<Buff> buffs = null; // TODO: Initialize to an appropriate value
            Stats expected = null; // TODO: Initialize to an appropriate value
            Stats actual;
            actual = target.GetBuffsStats(buffs);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetBuffCalculations
        ///</summary>
        [TestMethod()]
        public void GetBuffCalculationsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase currentCalcs = null; // TODO: Initialize to an appropriate value
            bool activeOnly = false; // TODO: Initialize to an appropriate value
            List<ComparisonCalculationBase> expected = null; // TODO: Initialize to an appropriate value
            List<ComparisonCalculationBase> actual;
            actual = target.GetBuffCalculations(character, currentCalcs, activeOnly);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EnchantFitsInSlot
        ///</summary>
        [TestMethod()]
        public void EnchantFitsInSlotTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Enchant enchant = null; // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            ItemSlot slot = new ItemSlot(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.EnchantFitsInSlot(enchant, character, slot);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeserializeDataObject
        ///</summary>
        [TestMethod()]
        public void DeserializeDataObjectTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            string xml = string.Empty; // TODO: Initialize to an appropriate value
            ICalculationOptionBase expected = null; // TODO: Initialize to an appropriate value
            ICalculationOptionBase actual;
            actual = target.DeserializeDataObject(xml);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateNewComparisonCalculation
        ///</summary>
        [TestMethod()]
        public void CreateNewComparisonCalculationTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            ComparisonCalculationBase expected = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase actual;
            actual = target.CreateNewComparisonCalculation();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateNewCharacterCalculations
        ///</summary>
        [TestMethod()]
        public void CreateNewCharacterCalculationsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            CharacterCalculationsBase expected = null; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase actual;
            actual = target.CreateNewCharacterCalculations();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ClearCache
        ///</summary>
        [TestMethod()]
        public void ClearCacheTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            target.ClearCache();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AccumulateItemStats
        ///</summary>
        [TestMethod()]
        public void AccumulateItemStatsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Stats stats = null; // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            Item additionalItem = null; // TODO: Initialize to an appropriate value
            target.AccumulateItemStats(stats, character, additionalItem);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AccumulateBuffsStats
        ///</summary>
        [TestMethod()]
        public void AccumulateBuffsStatsTest1()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Stats stats = null; // TODO: Initialize to an appropriate value
            List<Buff> buffs = null; // TODO: Initialize to an appropriate value
            target.AccumulateBuffsStats(stats, buffs);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        internal virtual CalculationsBase CreateCalculationsBase()
        {
            // TODO: Instantiate an appropriate concrete class.
            CalculationsBase target = null;
            return target;
        }

        /// <summary>
        ///A test for AccumulateBuffsStats
        ///</summary>
        [TestMethod()]
        public void AccumulateBuffsStatsTest()
        {
            CalculationsBase target = CreateCalculationsBase(); // TODO: Initialize to an appropriate value
            Stats stats = null; // TODO: Initialize to an appropriate value
            List<string> buffs = null; // TODO: Initialize to an appropriate value
            target.AccumulateBuffsStats(stats, buffs);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
         * 
         * 
         */
    }
}
