using System;
using System.Collections.Generic;

namespace Rawr.ShadowPriest
{

    public class CharacterCalculationsShadowPriest : CharacterCalculationsBase
    {
        private Stats basicStats;
        private Character character;

        public float SpiritRegen { get; set; }
        public float RegenInFSR { get; set; }
        public float RegenOutFSR { get; set; }
        public CharacterRace Race { get; set; }

        public bool bHoly { get; set; }

        public Character Character
        {
            get { return character; }
            set { character = value; }
        }
        
        public Stats BasicStats
        {
            get { return basicStats; }
            set { basicStats = value; }
        }

        public override float OverallPoints
        {
            get
            {
                float f = 0f;
                foreach (float f2 in _subPoints)
                    f += f2;
                return f;
            }
            set { }
        }

        private float[] _subPoints = new float[] { 0f, 0f };
        public override float[] SubPoints
        {
            get { return _subPoints; }
            set { _subPoints = value; }
        }

        public float DpsPoints
        {
            get { return _subPoints[0]; }
            set { _subPoints[0] = value; }
        }

/*        public float SustainPoints
        {
            get { return _subPoints[1]; }
            set { _subPoints[1] = value; }
        }*/

        public float SurvivalPoints
        {
            get { return _subPoints[1]; }
            set { _subPoints[1] = value; }
        }

        public SolverBase GetSolver(Character character, Stats stats)
        {
            if (character.PriestTalents.MindFlay > 0)
                return new SolverShadow(stats, character);
            else
                return new SolverHoly(stats, character);
        }

        public override Dictionary<string, string> GetCharacterDisplayCalculationValues()
        {
            Dictionary<string, string> dictValues = new Dictionary<string, string>();
            CalculationOptionsShadowPriest calcOpts = character.CalculationOptions as CalculationOptionsShadowPriest;
            BossOptions BossOpts = character.BossOptions;
            Stats baseStats = BaseStats.GetBaseStats(character.Level, character.Class, character.Race);
            bool Ptr = calcOpts.PTR;

            dictValues.Add("Health", BasicStats.Health.ToString());
            float ResilienceCap = 0.15f, ResilienceFromRating = StatConversion.GetCritReductionFromResilience(1);
            float Resilience = StatConversion.GetCritReductionFromResilience(BasicStats.Resilience);
            dictValues.Add("Resilience", string.Format("{0}*-{1}% Damage from DoT and Mana Drains\n\r-{1}% Chance to be crit\r\n-{2}% Damage from Crits.\r\n{3}",
                BasicStats.Resilience.ToString(),
                (Resilience * 100f).ToString("0.00"),
                (Resilience * 100f * 2.2f).ToString("0.00"),
                (Resilience > ResilienceCap) ? (string.Format("{0} rating above cap", ((float)Math.Floor((Resilience - ResilienceCap) / ResilienceFromRating)).ToString("0"))) : (string.Format("{0} rating below cap", ((float)Math.Ceiling((ResilienceCap - Resilience) / ResilienceFromRating)).ToString("0")))));
            dictValues.Add("Stamina", BasicStats.Stamina.ToString());
            dictValues.Add("Mana", BasicStats.Mana.ToString());
            dictValues.Add("Intellect", BasicStats.Intellect.ToString());
            dictValues.Add("Spirit", Math.Floor(BasicStats.Spirit).ToString("0"));
            dictValues.Add("Spell Power", String.Format("{0}*{1} Bonus Shadow\r\n{2} Bonus Holy\r\n{3} from Inner Fire", 
                Math.Floor(BasicStats.SpellPower), 
                Math.Floor(BasicStats.SpellPower + BasicStats.SpellShadowDamageRating), 
                Math.Floor(BasicStats.SpellPower /*+ BasicStats.SpellHolyDamageRating*/),
                BasicStats.PriestInnerFire * CalculationsShadowPriest.GetInnerFireSpellPowerBonus(character)));
            dictValues.Add("Regen", String.Format("{0}*MP5: {1}\r\nOutFSR: {2}" , RegenInFSR.ToString("0"), BasicStats.Mp5.ToString(), RegenOutFSR.ToString("0")));

            dictValues.Add("Crit", string.Format("{0}%*{1}% from {2} Spell Crit rating\r\n{3}% from Intellect\r\n{4}% from Focused Will\r\n{5}% from Base Crit\r\n{6}% from Buffs\r\n{7}% on Mind Blast, Mind Flay and Mind Sear.\r\n{8}% on VT, SW:P and DP\r\n{9}% on Smite, Holy Fire and Penance.",
                (BasicStats.SpellCrit * 100f).ToString("0.00"),
                (StatConversion.GetSpellCritFromRating(BasicStats.CritRating) * 100f).ToString("0.00"),
                BasicStats.CritRating.ToString("0"),
                (StatConversion.GetSpellCritFromIntellect(BasicStats.Intellect) * 100f).ToString("0.00"),
                character.PriestTalents.FocusedWill,
                (baseStats.SpellCrit * 100f).ToString("0.00"), 
                (BasicStats.SpellCrit * 100f - StatConversion.GetSpellCritFromRating(BasicStats.CritRating) * 100f - StatConversion.GetSpellCritFromIntellect(BasicStats.Intellect) * 100f - baseStats.SpellCrit * 100f - character.PriestTalents.FocusedWill).ToString("0.00"),
                (BasicStats.SpellCrit * 100f + character.PriestTalents.MindMelt * 2f).ToString("0.00"),
                (BasicStats.SpellCrit * 100f + character.PriestTalents.MindMelt * 3f).ToString("0.00"),
                (BasicStats.SpellCrit * 100f + character.PriestTalents.HolySpecialization * 1f).ToString("0.00")));

#if RAWR3 || SILVERLIGHT
            float Hit = 100 - (StatConversion.GetSpellMiss(character.Level - BossOpts.Level, false) * 100);
#else
            float Hit = 100 - (StatConversion.GetSpellMiss(-calcOpts.TargetLevel, false) * 100);
#endif
            float BonusHit = BasicStats.SpellHit * 100f;
            float RacialHit = 0;
            string RacialText = "";
            if (character.Race == CharacterRace.Draenei)
            {
                RacialHit = 1;
                RacialText = "1% from Draenei Racial\r\n";
                if (!character.ActiveBuffsContains("Heroic Presence"))
                    BonusHit += 1;
            }
            float DebuffHit = character.PriestTalents.Misery * 1f;
            if (character.ActiveBuffsConflictingBuffContains("Spell Hit Chance Taken"))
                DebuffHit = 3f;
            else
                BonusHit += DebuffHit;

            float RHitRating = 0.01f / StatConversion.GetSpellHitFromRating(1);
            float ShadowFocusHit = character.PriestTalents.ShadowFocus * 1f;
            float HitShadow = Hit + BonusHit + ShadowFocusHit;
            float HitHoly = Hit + BonusHit;
            dictValues.Add("Hit", string.Format("{0}%*{1}% from {2} Hit Rating\r\n{3}% from Buffs\r\n{4}% from {5} points in Misery\r\n{6}% from {7} points in Shadow Focus\r\n{8}{9}% Hit with Shadow spells, {10}\r\n{11}% Hit with Holy spells, {12}",
                BonusHit.ToString("0.00"),
                (StatConversion.GetSpellHitFromRating(BasicStats.HitRating) * 100f).ToString("0.00"), BasicStats.HitRating,
                (BonusHit - StatConversion.GetSpellHitFromRating(BasicStats.HitRating) * 100f - RacialHit - DebuffHit).ToString("0.00"),
                DebuffHit, character.PriestTalents.Misery,
                ShadowFocusHit, character.PriestTalents.ShadowFocus,
                RacialText,
                HitShadow.ToString("0.00"), (HitShadow > 100f) ? string.Format("{0} hit rating above cap", Math.Floor((HitShadow - 100f) * RHitRating)) : string.Format("{0} hit rating below cap", Math.Ceiling((100f - HitShadow) * RHitRating)),
                HitHoly.ToString("0.00"),  (HitHoly > 100f) ? string.Format("{0} hit rating above cap", Math.Floor((HitHoly - 100f) * RHitRating)) : string.Format("{0} hit rating below cap", Math.Ceiling((100f - HitHoly) * RHitRating))));

            dictValues.Add("Haste", string.Format("{0}%*{1}% from {2} Haste rating\r\n{3}% ({6}) points in Enlightenment\r\n{4}% from Buffs\r\n{5}s Global Cooldown",
                (BasicStats.SpellHaste * 100f).ToString("0.00"), (StatConversion.GetSpellHasteFromRating(BasicStats.HasteRating) * 100f).ToString("0.00"), BasicStats.HasteRating.ToString(), (character.PriestTalents.Enlightenment * 2).ToString("0"), (((1f + BasicStats.SpellHaste) / (1f + StatConversion.GetSpellHasteFromRating(BasicStats.HasteRating)) / (1f + character.PriestTalents.Enlightenment * 0.02f) - 1f) * 100f).ToString("0.00"), Math.Max(1.0f, 1.5f / (1 + BasicStats.SpellHaste)).ToString("0.00"), character.PriestTalents.Enlightenment));
            dictValues.Add("Armor", string.Format("{0}*{1}% Damage Reduction.",
                (BasicStats.Armor + BasicStats.BonusArmor).ToString("0"),
                (StatConversion.GetArmorDamageReduction(80, (BasicStats.Armor + BasicStats.BonusArmor), 0f, 0f, 0f) * 100f).ToString("0.00")));

            float[] Resistances = {
                0,
                BasicStats.ArcaneResistance + BasicStats.ArcaneResistanceBuff,
                BasicStats.FireResistance + BasicStats.FireResistanceBuff,
                BasicStats.FrostResistance + BasicStats.FrostResistanceBuff,
                BasicStats.NatureResistance + BasicStats.NatureResistanceBuff,
                BasicStats.ShadowResistance + BasicStats.ShadowResistanceBuff,
            };

            string[] ResistanceNames = {
                "None",
                "Arcane",
                "Fire",
                "Frost",
                "Nature",
                "Shadow",
            };

            string ResistanceString = "*Resistances:";

            float MaxResist = Resistances[0];
            int MaxResistIndex = 0;
            float AvgResist = 0f;
            for (int x = 1; x < Resistances.Length; x++)
            {
                AvgResist += Resistances[x];
                if (Resistances[x] > MaxResist)
                {
                    MaxResist = Resistances[x];
                    MaxResistIndex = x;
                }
                ResistanceString += string.Format("\r\n{0} : {1}", ResistanceNames[x], Resistances[x]);
            }
            AvgResist /= (Resistances.Length - 1);

            if (AvgResist == 0)
                ResistanceString = "None" + ResistanceString;
            else
            {
                string ResistanceName = (MaxResist == AvgResist) ? "All" : ResistanceNames[MaxResistIndex];
                ResistanceString = string.Format("{0} : {1}", ResistanceName, MaxResist.ToString("0")) + ResistanceString;
                ResistanceString += string.Format("\r\n\r\nResist ({0}):", ResistanceName);
                ResistanceString += string.Format("\r\n{0}", StatConversion.GetResistanceTableString(character.Level + 3, character.Level, MaxResist, 0));
            }

            dictValues.Add("Resistance", ResistanceString);

            SolverBase solver = GetSolver(character, BasicStats);
            solver.Calculate(this);

            dictValues.Add("Rotation", string.Format("{0}*{1}", solver.Name, solver.Rotation));
            if (solver.SpellSimulation != null)
            {
                String s = "Spell Cast List:";
                int i = 0;
                foreach (Spell spell in solver.SpellSimulation)
                {
                    if (i++ % 10 == 0)
                        s += "\r\n";
                    s += ", " + spell.Name;
                }
                s += "\r\n---Repeat---";
                dictValues.Add("Castlist", string.Format("{0}*{1}", solver.SpellSimulation.Count, s));
            }
            else
                dictValues.Add("Castlist", "Empty");
            dictValues.Add("DPS", string.Format("{0}*Damage Pr Second", solver.DPS.ToString("0")));
            //dictValues.Add("SustainDPS", string.Format("{0}*Mana restrained DPS", solver.SustainDPS.ToString("0")));

            float baseMana = BaseStats.GetBaseStats(character).Mana;

            dictValues.Add("SW Pain", new ShadowWordPain(BasicStats, character, baseMana, Ptr).ToString());
            DevouringPlague dp = new DevouringPlague(BasicStats, character, baseMana, Ptr);
            dictValues.Add("Devouring Plague", dp.ToString());
            if (dp.ImprovedDP != null)
                dictValues.Add("Imp. Devouring Plague", dp.ImprovedDP.ToString());
            else
                dictValues.Add("Imp. Devouring Plague", "- *No required talents");
            dictValues.Add("SW Death", new ShadowWordDeath(BasicStats, character, baseMana,Ptr).ToString());
            dictValues.Add("Mind Blast", new MindBlast(BasicStats, character, baseMana, Ptr).ToString());
            dictValues.Add("PW Shield", new PowerWordShield(BasicStats, character, baseMana, Ptr).ToString());

            if (character.PriestTalents.VampiricTouch > 0)
                dictValues.Add("Vampiric Touch", new VampiricTouch(BasicStats, character, baseMana, Ptr).ToString());
            else
                dictValues.Add("Vampiric Touch", "- *No required talents");

            if (character.PriestTalents.MindFlay > 0)
                dictValues.Add("Mind Flay", new MindFlay(BasicStats, character, baseMana, Ptr).ToString());
            else
                dictValues.Add("Mind Flay", "- *No required talents");

            dictValues.Add("Shadowfiend", new Shadowfiend(BasicStats, character, baseMana, Ptr).ToString());

            dictValues.Add("Smite", new Smite(BasicStats, character, baseMana, Ptr).ToString());
            dictValues.Add("Holy Fire", new HolyFire(BasicStats, character, baseMana, Ptr).ToString());
            if (character.PriestTalents.Penance > 0)
                dictValues.Add("Penance", new Penance(BasicStats, character, baseMana, Ptr).ToString());
            else
                dictValues.Add("Penance", "- *No required talents");

            return dictValues;
        }

        public override float GetOptimizableCalculationValue(string calculation)
        {
            switch (calculation)
            {
                case "Health": return basicStats.Health;
                case "Resilience": return basicStats.Resilience;
                case "Mana": return basicStats.Mana;
                case "Haste Rating": return basicStats.HasteRating;
                case "Haste %": return basicStats.SpellHaste * 100f;
                case "Crit Rating": return basicStats.CritRating;
                case "MB Crit %": return new MindBlast(basicStats, character, BaseStats.GetBaseStats(character).Mana, (character.CalculationOptions as CalculationOptionsShadowPriest).PTR).CritChance * 100f;
                case "Hit Rating": return basicStats.HitRating;
                case "MF cast time (ms)": return new MindFlay(basicStats, character, BaseStats.GetBaseStats(character).Mana, (character.CalculationOptions as CalculationOptionsShadowPriest).PTR).CastTime * 1000f;
                case "Armor": return basicStats.Armor + basicStats.BonusArmor;
                case "Arcane Resistance": return basicStats.ArcaneResistance + basicStats.ArcaneResistanceBuff;
                case "Fire Resistance": return basicStats.FireResistance + basicStats.FireResistanceBuff;
                case "Frost Resistance": return basicStats.FrostResistance + basicStats.FrostResistance;
                case "Nature Resistance": return basicStats.NatureResistance + basicStats.NatureResistanceBuff;
                case "Shadow Resistance": return basicStats.ShadowResistance + basicStats.ShadowResistanceBuff;
            }
            return 0f;
        }
    }
}
