using Rawr;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Drawing;

namespace Rawr.zzTestzz
{
    
    
    /// <summary>
    ///This is a test class for CalculationsTest and is intended
    ///to contain all CalculationsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CalculationsTest
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
            CharacterClass actual;
            actual = Calculations.TargetClass;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SupportsMultithreading
        ///</summary>
        [TestMethod()]
        public void SupportsMultithreadingTest()
        {
            bool actual;
            actual = Calculations.SupportsMultithreading;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SubPointNameColors
        ///</summary>
        [TestMethod()]
        public void SubPointNameColorsTest()
        {
            Dictionary<string, Color> actual;
            actual = Calculations.SubPointNameColors;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for OptimizableCalculationLabels
        ///</summary>
        [TestMethod()]
        public void OptimizableCalculationLabelsTest()
        {
            string[] actual;
            actual = Calculations.OptimizableCalculationLabels;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Models
        ///</summary>
        [TestMethod()]
        public void ModelsTest()
        {
            SortedList<string, Type> actual;
            actual = Calculations.Models;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ModelIcons
        ///</summary>
        [TestMethod()]
        public void ModelIconsTest()
        {
            Dictionary<string, string> actual;
            actual = Calculations.ModelIcons;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ModelClasses
        ///</summary>
        [TestMethod()]
        public void ModelClassesTest()
        {
            Dictionary<string, CharacterClass> actual;
            actual = Calculations.ModelClasses;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Instance
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Rawr.Base.dll")]
        public void InstanceTest()
        {
            CalculationsBase expected = null; // TODO: Initialize to an appropriate value
            CalculationsBase actual;
            Calculations_Accessor.Instance = expected;
            actual = Calculations_Accessor.Instance;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CustomRenderedChartNames
        ///</summary>
        [TestMethod()]
        public void CustomRenderedChartNamesTest()
        {
            string[] actual;
            actual = Calculations.CustomRenderedChartNames;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CustomChartNames
        ///</summary>
        [TestMethod()]
        public void CustomChartNamesTest()
        {
            string[] actual;
            actual = Calculations.CustomChartNames;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CharacterDisplayCalculationLabels
        ///</summary>
        [TestMethod()]
        public void CharacterDisplayCalculationLabelsTest()
        {
            string[] actual;
            actual = Calculations.CharacterDisplayCalculationLabels;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CanUseAmmo
        ///</summary>
        [TestMethod()]
        public void CanUseAmmoTest()
        {
            bool actual;
            actual = Calculations.CanUseAmmo;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CalculationOptionsPanel
        ///</summary>
        [TestMethod()]
        public void CalculationOptionsPanelTest()
        {
            CalculationOptionsPanelBase actual;
            actual = Calculations.CalculationOptionsPanel;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ValidModel
        ///</summary>
        [TestMethod()]
        public void ValidModelTest()
        {
            string model = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = Calculations.ValidModel(model);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RenderCustomChart
        ///</summary>
        [TestMethod()]
        public void RenderCustomChartTest()
        {
            Character character = null; // TODO: Initialize to an appropriate value
            string chartName = string.Empty; // TODO: Initialize to an appropriate value
            Graphics g = null; // TODO: Initialize to an appropriate value
            int width = 0; // TODO: Initialize to an appropriate value
            int height = 0; // TODO: Initialize to an appropriate value
            Calculations.RenderCustomChart(character, chartName, g, width, height);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnModelChanging
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Rawr.Base.dll")]
        public void OnModelChangingTest()
        {
            Calculations_Accessor.OnModelChanging();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnModelChanged
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Rawr.Base.dll")]
        public void OnModelChangedTest()
        {
            Calculations_Accessor.OnModelChanged();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for LoadModel
        ///</summary>
        [TestMethod()]
        public void LoadModelTest1()
        {
            CalculationsBase model = null; // TODO: Initialize to an appropriate value
            Calculations.LoadModel(model);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for LoadModel
        ///</summary>
        [TestMethod()]
        public void LoadModelTest()
        {
            Type type = null; // TODO: Initialize to an appropriate value
            Calculations.LoadModel(type);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ItemFitsInSlot
        ///</summary>
        [TestMethod()]
        public void ItemFitsInSlotTest()
        {
            Item item = null; // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            CharacterSlot slot = new CharacterSlot(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = Calculations.ItemFitsInSlot(item, character, slot);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsItemRelevant
        ///</summary>
        [TestMethod()]
        public void IsItemRelevantTest()
        {
            Item item = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = Calculations.IsItemRelevant(item);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsEnchantRelevant
        ///</summary>
        [TestMethod()]
        public void IsEnchantRelevantTest()
        {
            Enchant enchant = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = Calculations.IsEnchantRelevant(enchant);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsBuffRelevant
        ///</summary>
        [TestMethod()]
        public void IsBuffRelevantTest()
        {
            Buff buff = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = Calculations.IsBuffRelevant(buff);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IncludeOffHandInCalculations
        ///</summary>
        [TestMethod()]
        public void IncludeOffHandInCalculationsTest()
        {
            Character character = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = Calculations.IncludeOffHandInCalculations(character);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for HasRelevantStats
        ///</summary>
        [TestMethod()]
        public void HasRelevantStatsTest()
        {
            Stats stats = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = Calculations.HasRelevantStats(stats);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetRelevantStats
        ///</summary>
        [TestMethod()]
        public void GetRelevantStatsTest()
        {
            Stats stats = null; // TODO: Initialize to an appropriate value
            Stats expected = null; // TODO: Initialize to an appropriate value
            Stats actual;
            actual = Calculations.GetRelevantStats(stats);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetModel
        ///</summary>
        [TestMethod()]
        public void GetModelTest1()
        {
            string model = string.Empty; // TODO: Initialize to an appropriate value
            CalculationsBase expected = null; // TODO: Initialize to an appropriate value
            CalculationsBase actual;
            actual = Calculations.GetModel(model);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetModel
        ///</summary>
        [TestMethod()]
        public void GetModelTest()
        {
            Type type = null; // TODO: Initialize to an appropriate value
            CalculationsBase expected = null; // TODO: Initialize to an appropriate value
            CalculationsBase actual;
            actual = Calculations.GetModel(type);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetItemStats
        ///</summary>
        [TestMethod()]
        public void GetItemStatsTest()
        {
            Character character = null; // TODO: Initialize to an appropriate value
            Item additionalItem = null; // TODO: Initialize to an appropriate value
            Stats expected = null; // TODO: Initialize to an appropriate value
            Stats actual;
            actual = Calculations.GetItemStats(character, additionalItem);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetItemCalculations
        ///</summary>
        [TestMethod()]
        public void GetItemCalculationsTest1()
        {
            ItemInstance item = null; // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            CharacterSlot slot = new CharacterSlot(); // TODO: Initialize to an appropriate value
            ComparisonCalculationBase expected = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase actual;
            actual = Calculations.GetItemCalculations(item, character, slot);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetItemCalculations
        ///</summary>
        [TestMethod()]
        public void GetItemCalculationsTest()
        {
            Item item = null; // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            CharacterSlot slot = new CharacterSlot(); // TODO: Initialize to an appropriate value
            ComparisonCalculationBase expected = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase actual;
            actual = Calculations.GetItemCalculations(item, character, slot);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetEnchantCalculations
        ///</summary>
        [TestMethod()]
        public void GetEnchantCalculationsTest()
        {
            ItemSlot slot = new ItemSlot(); // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase currentCalcs = null; // TODO: Initialize to an appropriate value
            List<ComparisonCalculationBase> expected = null; // TODO: Initialize to an appropriate value
            List<ComparisonCalculationBase> actual;
            actual = Calculations.GetEnchantCalculations(slot, character, currentCalcs);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCustomChartData
        ///</summary>
        [TestMethod()]
        public void GetCustomChartDataTest()
        {
            Character character = null; // TODO: Initialize to an appropriate value
            string chartName = string.Empty; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase[] expected = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase[] actual;
            actual = Calculations.GetCustomChartData(character, chartName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterStatsString
        ///</summary>
        [TestMethod()]
        public void GetCharacterStatsStringTest()
        {
            Character character = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = Calculations.GetCharacterStatsString(character);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterStats
        ///</summary>
        [TestMethod()]
        public void GetCharacterStatsTest1()
        {
            Character character = null; // TODO: Initialize to an appropriate value
            Item additionalItem = null; // TODO: Initialize to an appropriate value
            Stats expected = null; // TODO: Initialize to an appropriate value
            Stats actual;
            actual = Calculations.GetCharacterStats(character, additionalItem);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterStats
        ///</summary>
        [TestMethod()]
        public void GetCharacterStatsTest()
        {
            Character character = null; // TODO: Initialize to an appropriate value
            Stats expected = null; // TODO: Initialize to an appropriate value
            Stats actual;
            actual = Calculations.GetCharacterStats(character);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterComparisonCalculations
        ///</summary>
        [TestMethod()]
        public void GetCharacterComparisonCalculationsTest2()
        {
            CharacterCalculationsBase baseCalculations = null; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase newCalculation = null; // TODO: Initialize to an appropriate value
            string name = string.Empty; // TODO: Initialize to an appropriate value
            bool equipped = false; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase expected = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase actual;
            actual = Calculations.GetCharacterComparisonCalculations(baseCalculations, newCalculation, name, equipped);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterComparisonCalculations
        ///</summary>
        [TestMethod()]
        public void GetCharacterComparisonCalculationsTest1()
        {
            CharacterCalculationsBase calculations = null; // TODO: Initialize to an appropriate value
            string name = string.Empty; // TODO: Initialize to an appropriate value
            bool equipped = false; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase expected = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase actual;
            actual = Calculations.GetCharacterComparisonCalculations(calculations, name, equipped);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterComparisonCalculations
        ///</summary>
        [TestMethod()]
        public void GetCharacterComparisonCalculationsTest()
        {
            CharacterCalculationsBase baseCalculations = null; // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            string name = string.Empty; // TODO: Initialize to an appropriate value
            bool equipped = false; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase expected = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase actual;
            actual = Calculations.GetCharacterComparisonCalculations(baseCalculations, character, name, equipped);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterCalculations
        ///</summary>
        [TestMethod()]
        public void GetCharacterCalculationsTest1()
        {
            Character character = null; // TODO: Initialize to an appropriate value
            Item additionalItem = null; // TODO: Initialize to an appropriate value
            bool referenceCalculation = false; // TODO: Initialize to an appropriate value
            bool significantChange = false; // TODO: Initialize to an appropriate value
            bool needsDisplayCalculations = false; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase expected = null; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase actual;
            actual = Calculations.GetCharacterCalculations(character, additionalItem, referenceCalculation, significantChange, needsDisplayCalculations);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCharacterCalculations
        ///</summary>
        [TestMethod()]
        public void GetCharacterCalculationsTest()
        {
            Character character = null; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase expected = null; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase actual;
            actual = Calculations.GetCharacterCalculations(character);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetBuffsStats
        ///</summary>
        [TestMethod()]
        public void GetBuffsStatsTest()
        {
            List<string> buffs = null; // TODO: Initialize to an appropriate value
            Stats expected = null; // TODO: Initialize to an appropriate value
            Stats actual;
            actual = Calculations.GetBuffsStats(buffs);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetBuffCalculations
        ///</summary>
        [TestMethod()]
        public void GetBuffCalculationsTest()
        {
            Character character = null; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase currentCalcs = null; // TODO: Initialize to an appropriate value
            bool activeOnly = false; // TODO: Initialize to an appropriate value
            List<ComparisonCalculationBase> expected = null; // TODO: Initialize to an appropriate value
            List<ComparisonCalculationBase> actual;
            actual = Calculations.GetBuffCalculations(character, currentCalcs, activeOnly);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EnchantFitsInSlot
        ///</summary>
        [TestMethod()]
        public void EnchantFitsInSlotTest()
        {
            Enchant item = null; // TODO: Initialize to an appropriate value
            Character character = null; // TODO: Initialize to an appropriate value
            ItemSlot slot = new ItemSlot(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = Calculations.EnchantFitsInSlot(item, character, slot);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeserializeDataObject
        ///</summary>
        [TestMethod()]
        public void DeserializeDataObjectTest()
        {
            string xml = string.Empty; // TODO: Initialize to an appropriate value
            ICalculationOptionBase expected = null; // TODO: Initialize to an appropriate value
            ICalculationOptionBase actual;
            actual = Calculations.DeserializeDataObject(xml);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateNewComparisonCalculation
        ///</summary>
        [TestMethod()]
        public void CreateNewComparisonCalculationTest()
        {
            ComparisonCalculationBase expected = null; // TODO: Initialize to an appropriate value
            ComparisonCalculationBase actual;
            actual = Calculations.CreateNewComparisonCalculation();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateNewCharacterCalculations
        ///</summary>
        [TestMethod()]
        public void CreateNewCharacterCalculationsTest()
        {
            CharacterCalculationsBase expected = null; // TODO: Initialize to an appropriate value
            CharacterCalculationsBase actual;
            actual = Calculations.CreateNewCharacterCalculations();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ClearCache
        ///</summary>
        [TestMethod()]
        public void ClearCacheTest()
        {
            Calculations.ClearCache();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Calculations Constructor
        ///</summary>
        [TestMethod()]
        public void CalculationsConstructorTest()
        {
            Calculations target = new Calculations();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
         * 
         */
    }
}
