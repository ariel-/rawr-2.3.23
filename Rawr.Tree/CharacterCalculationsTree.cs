using System;
using System.Collections.Generic;

namespace Rawr.Tree {
    public class CharacterCalculationsTree : CharacterCalculationsBase {
        public Stats BasicStats { get; set; }
        public Stats CombatStats { get; set; }

        private float[] subPoints = new float[] { 0f, 0f, 0f };
        public override float[] SubPoints {
            get { return subPoints; }
            set { subPoints = value; }
        }
        public float SingleTargetPoints {
            get { return subPoints[0]; }
            set { subPoints[0] = value; }
        }
        public float SustainedPoints {
            get { return subPoints[1]; }
            set { subPoints[1] = value; }
        }
        public float SurvivalPoints {
            get { return subPoints[2]; }
            set { subPoints[2] = value; }
        } 
        public override float OverallPoints { get; set; }
        public SustainedResult Sustained;
        public SingleTargetBurstResult[] SingleTarget;
        public Character LocalCharacter { get; set; }
        public float SingleTargetHPS;
        public float SustainedHPS;

        string LifebloomMethod_ToString(int lbStack, LifeBloomType type) {
            string result;
            if (type == LifeBloomType.Fast)
            {
                switch (lbStack)
                {
                    case 0: result = "Unused"; break;
                    case 1: result = "Single blooms"; break;
                    case 2: result = "Fast Double blooms"; break;
                    case 3: result = "Fast Triple blooms"; break;
                    case 4:
                    default: result = "Stack"; break;
                }
            }
            else if (type == LifeBloomType.Slow)
            {
                switch (lbStack)
                {
                    case 0: result = "Unused"; break;
                    case 1: result = "Single blooms"; break;
                    case 2: result = "Slow Double blooms"; break;
                    case 3: result = "Slow Triple blooms"; break;
                    case 4:
                    default: result = "Stack"; break;
                }
            }
            else result = "Rolling stack";

            return result;
        }

        public override Dictionary<string, string> GetCharacterDisplayCalculationValues() {
            Dictionary<string, string> dictValues = new Dictionary<string, string>();

            dictValues.Add("Single Target Points", SingleTargetPoints.ToString());
            dictValues.Add("Sustained Points", SustainedPoints.ToString());
            dictValues.Add("Survival Points", SurvivalPoints.ToString());
            dictValues.Add("Overall Points", OverallPoints.ToString());

            dictValues.Add("Base Health", BasicStats.Health.ToString());
            dictValues.Add("Base Armor", Math.Round(BasicStats.Armor, 0) + "*Reduces damage taken by " + Math.Round(StatConversion.GetArmorDamageReduction(83, BasicStats.Armor, 0, 0, 0) * 100.0f, 2) + "%");
            dictValues.Add("Base Mana", BasicStats.Mana.ToString());
            dictValues.Add("Base Stamina", BasicStats.Stamina.ToString());
            dictValues.Add("Base Intellect", BasicStats.Intellect.ToString());
            dictValues.Add("Base Spirit", BasicStats.Spirit.ToString());
            dictValues.Add("Base Spell Power", (BasicStats.SpellPower).ToString() + "*" + BasicStats.Spirit * LocalCharacter.DruidTalents.ImprovedTreeOfLife * 0.05f + " from Improved Tree of Life");
            dictValues.Add("Base Spell Crit", BasicStats.SpellCrit.ToString());

            float speed_from_hr = (1f + StatConversion.GetSpellHasteFromRating(BasicStats.HasteRating));
            float speed_from_sh = (1f + BasicStats.SpellHaste);
            float speed = speed_from_hr * speed_from_sh;
            float hard = (1.5f / (1f * speed_from_sh) - 1) * StatConversion.RATING_PER_SPELLHASTE;
            float untilcap = hard - BasicStats.HasteRating;

            dictValues.Add("Base Spell Haste", Math.Round((speed - 1.0f) * 100.0f, 2) + "%*" + Math.Round((speed_from_sh - 1.0f) * 100.0f, 2) + "% from spell effects and talents\n" + Math.Round((speed_from_hr - 1.0f) * 100.0f, 2) + "% from " + BasicStats.HasteRating + " haste rating");
            // Use Nourish cast time to equal normal GCD
            Spell spell = new Nourish(LocalCharacter, BasicStats);
            dictValues.Add("Base Global CD", Math.Round(spell.gcd, 2) + " sec*" + Math.Ceiling(untilcap).ToString() + " Haste Rating until hard gcd cap");

            dictValues.Add("Health", CombatStats.Health.ToString());
            dictValues.Add("Armor", Math.Round(CombatStats.Armor, 0) + "*Reduces damage taken by " + Math.Round(StatConversion.GetArmorDamageReduction(83, CombatStats.Armor, 0, 0, 0) * 100.0f, 2) + "%");
            dictValues.Add("Mana", CombatStats.Mana.ToString());
            dictValues.Add("Stamina", CombatStats.Stamina.ToString());
            dictValues.Add("Intellect", CombatStats.Intellect.ToString());
            dictValues.Add("Spirit", CombatStats.Spirit.ToString());
            dictValues.Add("Spell Power", (CombatStats.SpellPower).ToString() + "*" + CombatStats.Spirit * LocalCharacter.DruidTalents.ImprovedTreeOfLife * 0.05f + " from Improved Tree of Life");
            dictValues.Add("Spell Crit", CombatStats.SpellCrit.ToString());

            speed_from_hr = (1f + StatConversion.GetSpellHasteFromRating(CombatStats.HasteRating));
            speed_from_sh = (1f + CombatStats.SpellHaste);
            speed = speed_from_hr * speed_from_sh;
            hard = (1.5f / (1f * speed_from_sh) - 1) * StatConversion.RATING_PER_SPELLHASTE;
            untilcap = hard - CombatStats.HasteRating;

            dictValues.Add("Spell Haste", Math.Round((speed - 1.0f) * 100.0f, 2) + "%*" + Math.Round((speed_from_sh - 1.0f) * 100.0f, 2) + "% from spell effects and talents\n" + Math.Round((speed_from_hr - 1.0f) * 100.0f, 2) + "% from " + CombatStats.HasteRating + " haste rating");
            // Use Nourish cast time to equal normal GCD
            spell = Sustained.spellMix.nourish[0];
            dictValues.Add("Global CD", Math.Round(spell.gcd, 2) + " sec*" + Math.Ceiling(untilcap).ToString() + " Haste Rating until soft gcd cap");

            dictValues.Add("Total Time", Math.Round(Sustained.TotalTime, 2).ToString());
            dictValues.Add("Time until OOM (unreduced)", Math.Round(Sustained.TimeToOOM_unreduced, 2).ToString());
            dictValues.Add("Time until OOM", Math.Round(Sustained.TimeToOOM, 2).ToString());
            dictValues.Add("Total healing done", Math.Round(Sustained.TotalTime * SustainedHPS, 2).ToString()); // Has extra component from procs
            dictValues.Add("Sustained HPS", Math.Round(SustainedHPS, 2).ToString());
            dictValues.Add("Single Target HPS", Math.Round(SingleTargetHPS, 2).ToString());
            dictValues.Add("Mana regen per second", Math.Round(Sustained.ManaRegen, 2).ToString());
            dictValues.Add("Mana from innervates", Math.Round(Sustained.InnervateMana, 2).ToString());
            dictValues.Add("Average casts per minute", Math.Round(Sustained.spellMix.CastsPerMinute, 2).ToString());
            dictValues.Add("Average crits per minute", Math.Round(Sustained.spellMix.CritsPerMinute, 2).ToString());
            dictValues.Add("Average heals per minute", Math.Round(Sustained.spellMix.HealsPerMinute, 2).ToString());
            dictValues.Add("Rejuvenation casts per minute", Math.Round(Sustained.spellMix.RejuvCPM, 2).ToString());
            dictValues.Add("Rejuvenation average up", Math.Round(Sustained.spellMix.RejuvAvg, 2).ToString());
            dictValues.Add("Regrowth casts per minute", Math.Round(Sustained.spellMix.RegrowthCPM, 2).ToString());
            dictValues.Add("Regrowth average up", Math.Round(Sustained.spellMix.RegrowthAvg, 2).ToString());
            dictValues.Add("Lifebloom (stack) casts per minute", Math.Round(Sustained.spellMix.LifebloomStackCPM, 2).ToString());
            dictValues.Add("Lifebloom (stack) average up", Math.Round(Sustained.spellMix.LifebloomStackAvg, 2).ToString());
            dictValues.Add("Lifebloom (stack) method", LifebloomMethod_ToString(3, Sustained.rotSettings.lifeBloomType));
            dictValues.Add("Lifebloom casts per minute", Math.Round(Sustained.spellMix.LifebloomCPM, 2).ToString());
            dictValues.Add("Lifebloom average up", Math.Round(Sustained.spellMix.LifebloomAvg, 2).ToString());
            dictValues.Add("Nourish casts per minute", Math.Round(Sustained.spellMix.NourishCPM, 2).ToString());
            //dictValues.Add("Healing Touch casts per minute", Math.Round(, 2).ToString());
            dictValues.Add("Swiftmend casts per minute", Math.Round(Sustained.spellMix.SwiftmendCPM, 2).ToString());
            dictValues.Add("Wild Growth casts per minute", Math.Round(Sustained.spellMix.WildGrowthCPM, 2).ToString());
            dictValues.Add("Revitalize procs per minute", Math.Round(Sustained.spellMix.RevitalizeProcsPerMinute, 2).ToString());

            dictValues.Add("RJ Heal per tick", Math.Round(Sustained.spellMix.rejuvenate.PeriodicTickwithCrit, 2).ToString());
            dictValues.Add("RJ Tick time", Math.Round(Sustained.spellMix.rejuvenate.PeriodicTickTime, 2).ToString());
            dictValues.Add("RJ HPS", Math.Round(Sustained.spellMix.rejuvenate.HPS, 2).ToString());
            dictValues.Add("RJ HPM", Math.Round(Sustained.spellMix.rejuvenate.HPM, 2).ToString());
            dictValues.Add("RJ HPCT", Math.Round(Sustained.spellMix.rejuvenate.HPCT, 2).ToString());

            dictValues.Add("RG Heal", Sustained.spellMix.regrowth.ToString());
            dictValues.Add("RG Tick", Math.Round(Sustained.spellMix.regrowth.PeriodicTickwithCrit, 2).ToString());
            dictValues.Add("RG HPS", Math.Round(Sustained.spellMix.regrowth.HPS, 2).ToString());
            dictValues.Add("RG HPS (HoT only)", Math.Round(Sustained.spellMix.regrowth.HPS_HOT, 2).ToString());
            dictValues.Add("RG HPM", Math.Round(Sustained.spellMix.regrowth.HPM, 2).ToString());
            dictValues.Add("RG HPCT", Math.Round(Sustained.spellMix.regrowth.HPCT, 2).ToString());
            dictValues.Add("RG Heal (spam)", Sustained.spellMix.regrowthAgain.ToString());
            dictValues.Add("RG HPS (spam)", Math.Round(Sustained.spellMix.regrowthAgain.HPCT_DH, 2).ToString());
            dictValues.Add("RG HPM (spam)", Math.Round(Sustained.spellMix.regrowthAgain.HPM_DH, 2).ToString());
            dictValues.Add("RG HPCT (spam)", Math.Round(Sustained.spellMix.regrowthAgain.HPCT_DH, 2).ToString());

            dictValues.Add("LB Tick", Math.Round(Sustained.spellMix.lifebloom.PeriodicTick, 2).ToString());
            dictValues.Add("LB Bloom Heal", Math.Round(Sustained.spellMix.lifebloom.AverageHealingwithCrit, 2).ToString());
            dictValues.Add("LB HPS", Math.Round(Sustained.spellMix.lifebloom.HPS, 2).ToString());
            dictValues.Add("LB HPS (HoT only)", Math.Round(Sustained.spellMix.lifebloom.HPS_HOT, 2).ToString());
            dictValues.Add("LB HPM", Math.Round(Sustained.spellMix.lifebloom.HPM, 2).ToString());
            dictValues.Add("LB HPCT", Math.Round(Sustained.spellMix.lifebloom.HPCT, 2).ToString());

            dictValues.Add("LBx2 (fast stack) HPS", Math.Round(Sustained.spellMix.lifebloomFast2Stack.HPS, 2).ToString());
            dictValues.Add("LBx2 (fast stack) HPM", Math.Round(Sustained.spellMix.lifebloomFast2Stack.HPM, 2).ToString());
            dictValues.Add("LBx2 (fast stack) HPCT", Math.Round(Sustained.spellMix.lifebloomFast2Stack.HPCT, 2).ToString());
            dictValues.Add("LBx3 (fast stack) HPS", Math.Round(Sustained.spellMix.lifebloomFastStack.HPS, 2).ToString());
            dictValues.Add("LBx3 (fast stack) HPM", Math.Round(Sustained.spellMix.lifebloomFastStack.HPM, 2).ToString());
            dictValues.Add("LBx3 (fast stack) HPCT", Math.Round(Sustained.spellMix.lifebloomFastStack.HPCT, 2).ToString());
            dictValues.Add("LBx2 (slow stack) HPS", Math.Round(Sustained.spellMix.lifebloomSlow2Stack.HPS, 2).ToString());
            dictValues.Add("LBx2 (slow stack) HPM", Math.Round(Sustained.spellMix.lifebloomSlow2Stack.HPM, 2).ToString());
            dictValues.Add("LBx2 (slow stack) HPCT", Math.Round(Sustained.spellMix.lifebloomSlow2Stack.HPCT, 2).ToString());
            dictValues.Add("LBx3 (slow stack) HPS", Math.Round(Sustained.spellMix.lifebloomSlowStack.HPS, 2).ToString());
            dictValues.Add("LBx3 (slow stack) HPM", Math.Round(Sustained.spellMix.lifebloomSlowStack.HPM, 2).ToString());
            dictValues.Add("LBx3 (slow stack) HPCT", Math.Round(Sustained.spellMix.lifebloomSlowStack.HPCT, 2).ToString());
            dictValues.Add("LBx3 (rolling) Tick", Math.Round(Sustained.spellMix.lifebloomRollingStack.PeriodicTick, 2).ToString());
            dictValues.Add("LBx3 (rolling) HPS", Math.Round(Sustained.spellMix.lifebloomRollingStack.HPS, 2).ToString());
            dictValues.Add("LBx3 (rolling) HPM", Math.Round(Sustained.spellMix.lifebloomRollingStack.HPM, 2).ToString());
            dictValues.Add("LBx3 (rolling) HPCT", Math.Round(Sustained.spellMix.lifebloomRollingStack.HPCT, 2).ToString());

            dictValues.Add("HT Heal", Sustained.spellMix.healingTouch.ToString());
            dictValues.Add("HT HPS", Math.Round(Sustained.spellMix.healingTouch.HPCT_DH, 2).ToString());
            dictValues.Add("HT HPM", Math.Round(Sustained.spellMix.healingTouch.HPM_DH, 2).ToString());
            dictValues.Add("HT HPCT", Math.Round(Sustained.spellMix.healingTouch.HPCT_DH, 2).ToString());

            dictValues.Add("WG first Tick", Math.Round(Sustained.spellMix.wildGrowth.Tick[0], 2).ToString());
            dictValues.Add("WG HPS(single target)", Math.Round(Sustained.spellMix.wildGrowth.HPS, 2).ToString());
            dictValues.Add("WG HPM(single target)", Math.Round(Sustained.spellMix.wildGrowth.HPM, 2).ToString());
            dictValues.Add("WG HPS(max)", Math.Round(Sustained.spellMix.wildGrowth.HPS * Sustained.spellMix.wildGrowth.maxTargets, 2).ToString());
            dictValues.Add("WG HPM(max)", Math.Round(Sustained.spellMix.wildGrowth.HPM * Sustained.spellMix.wildGrowth.maxTargets, 2).ToString());

            dictValues.Add("N Heal",Sustained.spellMix.nourish[0].ToString());
            dictValues.Add("N HPM", Math.Round(Sustained.spellMix.nourish[0].HPM_DH, 2).ToString());
            dictValues.Add("N HPS", Math.Round(Sustained.spellMix.nourish[0].HPCT_DH, 2).ToString());
            dictValues.Add("N HPCT", Math.Round(Sustained.spellMix.nourish[0].HPCT_DH, 2).ToString());
            dictValues.Add("N (1 HoT) Heal", Sustained.spellMix.nourish[1].ToString());
            dictValues.Add("N (1 HoT) HPM", Math.Round(Sustained.spellMix.nourish[1].HPM_DH, 2).ToString());
            dictValues.Add("N (1 HoT) HPS", Math.Round(Sustained.spellMix.nourish[1].HPCT_DH, 2).ToString());
            dictValues.Add("N (1 HoT) HPCT", Math.Round(Sustained.spellMix.nourish[1].HPCT_DH, 2).ToString());
            dictValues.Add("N (2 HoTs) Heal", Sustained.spellMix.nourish[2].ToString());
            dictValues.Add("N (2 HoTs) HPM", Math.Round(Sustained.spellMix.nourish[2].HPM_DH, 2).ToString());
            dictValues.Add("N (2 HoTs) HPS", Math.Round(Sustained.spellMix.nourish[2].HPCT_DH, 2).ToString());
            dictValues.Add("N (2 HoTs) HPCT", Math.Round(Sustained.spellMix.nourish[2].HPCT_DH, 2).ToString());
            dictValues.Add("N (3 HoTs) Heal", Sustained.spellMix.nourish[3].ToString());
            dictValues.Add("N (3 HoTs) HPM", Math.Round(Sustained.spellMix.nourish[3].HPM_DH, 2).ToString());
            dictValues.Add("N (3 HoTs) HPS", Math.Round(Sustained.spellMix.nourish[3].HPCT_DH, 2).ToString());
            dictValues.Add("N (3 HoTs) HPCT", Math.Round(Sustained.spellMix.nourish[3].HPCT_DH, 2).ToString());
            dictValues.Add("N (4 HoTs) Heal", Sustained.spellMix.nourish[4].ToString());
            dictValues.Add("N (4 HoTs) HPM", Math.Round(Sustained.spellMix.nourish[4].HPM_DH, 2).ToString());
            dictValues.Add("N (4 HoTs) HPS", Math.Round(Sustained.spellMix.nourish[4].HPCT_DH, 2).ToString());
            dictValues.Add("N (4 HoTs) HPCT", Math.Round(Sustained.spellMix.nourish[4].HPCT_DH, 2).ToString());

            Swiftmend swift = new Swiftmend(LocalCharacter, CombatStats, new Rejuvenation(LocalCharacter, CombatStats), null);
            dictValues.Add("SM Rejuv Heal", swift.ToString());
            dictValues.Add("SM Rejuv HPM", Math.Round(swift.HPM, 2).ToString());
            dictValues.Add("SM Rejuv Lost Ticks", Math.Round(swift.rejuvTicksLost, 2).ToString());
            swift = new Swiftmend(LocalCharacter, BasicStats, null, new Regrowth(LocalCharacter, BasicStats));
            dictValues.Add("SM Regrowth Heal", swift.ToString());
            dictValues.Add("SM Regrowth HPM", Math.Round(swift.HPM, 2).ToString());
            dictValues.Add("SM Regrowth Lost Ticks", Math.Round(swift.regrowthTicksLost, 2).ToString());
            swift = new Swiftmend(LocalCharacter, BasicStats, new Rejuvenation(LocalCharacter, BasicStats), new Regrowth(LocalCharacter, BasicStats));
            dictValues.Add("SM Both Heal", swift.ToString());
            dictValues.Add("SM Both HPM", Math.Round(swift.HPM, 2).ToString());
            dictValues.Add("SM Both Rejuv Lost Ticks", Math.Round(swift.rejuvTicksLost, 2).ToString());
            dictValues.Add("SM Both Regrowth Lost Ticks", Math.Round(swift.regrowthTicksLost, 2).ToString());
            
            return dictValues;
        }
        public override float GetOptimizableCalculationValue(string calculation) {
            switch (calculation) {
                case "Mana": return CombatStats.Mana;
                case "MP5": return Sustained.MPSInFSR;
                case "Global CD": return (new Nourish(LocalCharacter, CombatStats)).gcd * 1000.0f;  // Use Nourish gcd to equal normal GCD
            }
            return 0f;
        }
    }
}
