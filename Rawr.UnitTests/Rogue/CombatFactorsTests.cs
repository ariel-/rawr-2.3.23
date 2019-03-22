//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Rawr.Rogue;
//using Rawr.Rogue.ClassAbilities;

//namespace Rawr.UnitTests.Rogue
//{
//    /// <summary>
//    /// Tests the Properties and Methods in the Rawr.Rogue.CombatFactors class
//    /// </summary>
//    [TestClass]
//    public class CombatFactorsTests
//    {
//        [TestMethod]
//        public void BaseEnergyRegen()
//        {
//            var combatFactors = CreateCombatFactors(new RogueTalents());
//            AssertRoundedValues(10f, combatFactors.BaseEnergyRegen);
//        }

//        [TestMethod]
//        public void RegenWithAdrenalineRush()
//        {
//            var combatFactors = CreateCombatFactors(new RogueTalents { AdrenalineRush = 1});
//            AssertRoundedValues(10.83333f, combatFactors.BaseEnergyRegen);
//        }

//        [TestMethod]
//        public void RegenWithAdrenalineRushGlyph()
//        {
//            var combatFactors = CreateCombatFactors(new RogueTalents {AdrenalineRush = 1, GlyphOfAdrenalineRush = true});
//            AssertRoundedValues(11.111111f, combatFactors.BaseEnergyRegen);
//        }

//        [TestMethod]
//        public void RegenWithVitality()
//        {
//            var combatFactors = CreateCombatFactors(new RogueTalents { Vitality = 3 });
//            AssertRoundedValues(12.5f, combatFactors.BaseEnergyRegen);
//        }

//        [TestMethod]
//        public void RegenWithBladeFlurry()
//        {
//            var combatFactors = CreateCombatFactors(new RogueTalents { BladeFlurry = 1}); 
//            AssertRoundedValues(9.7916667f, combatFactors.BaseEnergyRegen);
//        }

//        [TestMethod]
//        public void RegenWithBladeFlurryGlyph()
//        {
//            var combatFactors = CreateCombatFactors(new RogueTalents { BladeFlurry = 1, GlyphOfBladeFlurry = true });
//            AssertRoundedValues(10f, combatFactors.BaseEnergyRegen);
//        }

//        public void AssertRoundedValues(float expected, float actual)
//        {
//            Assert.AreEqual(Math.Round(expected, 4), Math.Round(actual, 4));
//        }

//        public CombatFactors CreateCombatFactors( RogueTalents talents )
//        {
//            var character = new RogueTestCharacter(talents);
//            TalentsAndGlyphs.Initialize(character.RogueTalents);
//            return new CombatFactors(character, new Stats());
//        }
//    }
//}
