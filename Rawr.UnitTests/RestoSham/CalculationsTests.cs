using System;
using System.Text;
using System.Collections.Generic;
using Rawr.RestoSham;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rawr.UnitTests.RestoSham
{
    /// <summary>
    /// Tests to verify the operation of the calculation methods for the RestoSham module
    /// </summary>
    [TestClass]
    public class CalculationsTests
    {
        public CalculationsTests()
        {
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        //[TestMethod]
        //public void GetCharacterCalculations_StatModifierTest()
        //{
        //    float modifierChange = 50f;
        //    RestoShamTestCharacter character = new RestoShamTestCharacter();
        //    CalculationsRestoSham calc = new CalculationsRestoSham();

        //    foreach (string statName in RelevantStats.StatList)
        //    {
        //        Assert.IsFalse(String.IsNullOrEmpty(statName), "Invalid stat in the relevant stats list.");

        //        Stats modifier = new Stats();
        //        typeof(Stats).GetProperty(statName).SetValue(modifier, modifierChange, null);
        //        CharacterCalculationsRestoSham baseCalc = calc.GetCharacterCalculations(character) as CharacterCalculationsRestoSham;
        //        CharacterCalculationsRestoSham modifiedCalc = (CharacterCalculationsRestoSham)calc.GetCharacterCalculations(character, null, modifier);

        //        // Get the calculation results
        //        float baseValue = (float)typeof(Stats).GetProperty(statName).GetValue(baseCalc.BasicStats, null);
        //        float modValue = (float)typeof(Stats).GetProperty(statName).GetValue(modifiedCalc.BasicStats, null);
        //        bool changed = (modValue >= (baseValue + modifierChange));
                
        //        // Validate that the base stats changed
        //        string errorMessage = string.Format("Value for stat '{0}' did not change by the expected amount. (Base:{1} -> Modified:{2})",
        //            statName, baseValue, modValue);
        //        Assert.IsTrue(changed, errorMessage);

        //        // Validate that at least one of the derived calculations changed
        //        string[] styles = new string[] { "CHSpam", "HWSpam", "LHWSpam", "RTHW", "RTCH", "RTLHW" };

        //        // If the overall rating changed, skip checking the individual sequence calculations
        //        changed = (baseCalc.OverallPoints != modifiedCalc.OverallPoints);
        //        if (!changed)
        //        {
        //            foreach (string s in styles)
        //            {
        //                string mps = string.Format("{0}MPS", s);
        //                string hps = string.Format("{0}HPS", s);

        //                baseValue = (float)typeof(CharacterCalculationsRestoSham).GetProperty(mps).GetValue(baseCalc, null);
        //                modValue = (float)typeof(CharacterCalculationsRestoSham).GetProperty(mps).GetValue(modifiedCalc, null);
        //                if (baseValue != modValue)
        //                {
        //                    changed = true;
        //                    break;
        //                }

        //                baseValue = (float)typeof(CharacterCalculationsRestoSham).GetProperty(hps).GetValue(baseCalc, null);
        //                modValue = (float)typeof(CharacterCalculationsRestoSham).GetProperty(hps).GetValue(modifiedCalc, null);
        //                if (baseValue != modValue)
        //                {
        //                    changed = true;
        //                    break;
        //                }
        //            }
        //        }
                
        //        errorMessage = string.Format("Ratings while using '{0}' as a modifier did not change. (Overall: {1})",
        //            statName, modifiedCalc.OverallPoints);
        //        Assert.IsTrue(changed, errorMessage);
        //    }
        //}
    }
}
