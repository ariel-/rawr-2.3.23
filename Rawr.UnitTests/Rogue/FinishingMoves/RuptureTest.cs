//using Rawr.Rogue.ClassAbilities;
//using Rawr.Rogue.ComboPointGenerators;
//using Rawr.Rogue.FinishingMoves;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Rawr.Rogue;

//namespace Rawr.UnitTests.Rogue.FinishingMoves
//{
//    [TestClass]
//    public class RuptureTests
//    {
//        //Tests assume 60 energy per backstab, regen of 10 energy per second, so there
//        // is a 6 second delay between backstabs

//        [TestMethod]
//        public void ShortRupture()
//        {
//            TestBonusDuration(0f, 0f);
//            TestBonusDuration(6f, 0f);
//        }

//        [TestMethod]
//        public void MediumRupture()
//        {
//            TestBonusDuration(6.1f, 2f);
//            TestBonusDuration(10f, 2f);
//        }

//        [TestMethod]
//        public void LongRupture()
//        {
//            TestBonusDuration(10.1f, 4f);
//            TestBonusDuration(14f, 4f);
//        }

//        [TestMethod]
//        public void ReallyLongRupture()
//        {
//            TestBonusDuration(14.1f, 6f);
//            TestBonusDuration(100f, 6f);
//        }

//        private static void TestBonusDuration(float ruptureDuration, float expectedBonus)
//        {
//            var calcOpts = new CalculationOptionsRogue { CpGenerator = new Backstab() };
//            var character = new RogueTestCharacter(new RogueTalents { GlyphOfBackstab = true });
//            TalentsAndGlyphs.Initialize(character.RogueTalents);

//            var cycle = new Cycle();
//            var rupture = new CycleComponent(4, new Rupture());
//            cycle.Components.Add(rupture);
//            calcOpts.DpsCycle = cycle;

//            var combatFactors = new CombatFactors(character, new Stats());
//            var cycleTime = new CycleTime(calcOpts, combatFactors, new WhiteAttacks(character, new Stats(), combatFactors)) { EnergyRegen = 10f };

//            var actual = Rupture.BonusDurationFromBackstab(calcOpts, combatFactors, cycleTime, ruptureDuration);
//            Assert.AreEqual(expectedBonus, actual);            
//        }
//    }
//}
