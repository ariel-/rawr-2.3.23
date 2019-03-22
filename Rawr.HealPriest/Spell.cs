using System;
using System.Collections.Generic;
#if RAWR3
using System.Windows.Media;
#else
using System.Drawing;
#endif
using System.Text;


namespace Rawr.HealPriest
{  
    public class Spell
    {
        public class SpellData
        {
            public int Rank { get; protected set; }
            public int Level { get; protected set; }
            public int MinHeal { get; protected set; }
            public int MaxHeal { get; protected set; }
            public float CastTime { get; protected set; }

            public SpellData(int rank, int level, int minHeal, int maxHeal, float castTime)
            {
                Rank = rank;
                Level = level;
                MinHeal = minHeal;
                MaxHeal = maxHeal;
                CastTime = castTime;
            }
        }

        public static readonly float SP2HP = (0.855f / 0.455f);      

        public string Name { get; protected set; }
        public int Rank { get; protected set; }

        public float MinHeal { get; protected set; }
        public float MaxHeal { get; protected set; }
        public float BaseMana { get; protected set; }
        public int ManaCost { get; protected set; }
        public float BaseCastTime { get; protected set; }
        public float CastTime { get; protected set; }
        public float HotDuration { get; protected set; }
        public float CritChance { get; protected set; }
        public float CritCoef { get; protected set; }

        public float HealingCoef { get; protected set; }
        public float RankCoef { get; protected set; }
        public int Range { get; protected set; }
        public int Targets { get; protected set; }

        public float GlobalCooldown { get; protected set; }
        public float Cooldown { get; protected set; }
        public Color GraphColor { get; protected set; }


        #region Properties

        public bool IsInstant
        {
            get { return CastTime == 0; }
        }

        public bool IsHot
        {
            get { return HotDuration > 0; }
        }

        public float AvgHeal
        {
            get
            {
                return (MinHeal + MaxHeal) / 2;
            }
        }

        public virtual float AvgTotHeal
        {
            get
            {
                return AvgHeal * (1 - CritChance) + AvgCrit * CritChance;
            }
        }

        public virtual float HpS
        {
            get
            {
                if (IsHot)
                    return AvgTotHeal / HotDuration;

                return AvgTotHeal / CastTime;
            }
        }

        public virtual float HpM
        {
            get
            {
                return AvgTotHeal / ManaCost;
            }
        }

        public float AvgCrit
        {
            get
            {
                return (MinHeal * CritCoef + MaxCrit)/2;
            }
        }

        public float MaxCrit
        {
            get
            {
                return MaxHeal * CritCoef;
            }
        }

        #endregion

        public Spell(string name, Stats stats, Character character, List<SpellData> SpellRankTable, int manaCost, float coef, Color col)
            : this(name, stats, character, SpellRankTable, manaCost, coef, 0, col)
        { }

        public Spell(string name, Stats stats, Character character, List<SpellData> SpellRankTable, int manaCost, float coef, float hotDuration, Color col)
        {
            foreach (SpellData sd in SpellRankTable)
                if (character.Level >= sd.Level)
                {
                    Rank = sd.Rank;
                    MinHeal = sd.MinHeal;
                    MaxHeal = sd.MaxHeal;
                    BaseCastTime = sd.CastTime;
                }
//            Name = string.Format("{0}, Rank {1}", name, Rank);
            Name = name;
            GlobalCooldown = Math.Max(1.5f / (1 + stats.SpellHaste), 1.0f);
            if (BaseCastTime == 0)
                CastTime = GlobalCooldown;
            else
                CastTime = Math.Max(BaseCastTime / (1 + stats.SpellHaste), 1.0f);
            Cooldown = CastTime;
            ManaCost = manaCost;
            HealingCoef = coef;
            HotDuration = hotDuration;
            GraphColor = col;
            CritChance = stats.SpellCrit;
            CritCoef = 1.5f * (1f + stats.BonusCritHealMultiplier);
            BaseMana = BaseStats.GetBaseStats(character.Level, character.Class, character.Race).Mana;
        }
      
        public override string ToString()
        {
            return String.Format("{0} *HpS: {1}\r\nHpM: {2}\r\nMin Heal: {3}\r\nMax Heal: {4}\r\nAvg Crit: {5}\r\nMax Crit: {6}\r\nCast: {7}\r\nCost: {8}\r\n{9}",
                AvgHeal.ToString("0"),
                HpS.ToString("0.00"),
                HpM.ToString("0.00"),
                MinHeal.ToString("0"),
                MaxHeal.ToString("0"),
                AvgCrit.ToString("0"),
                MaxCrit.ToString("0"),
                CastTime.ToString("0.00"),
                ManaCost.ToString("0"),
                Name);
        }
    }

    public class Renew : Spell
    {
        public float InstantHealEffect = 0f;

        private static readonly List<SpellData> SpellRankTable = new List<SpellData> (){   
        /*                Rank MinHeal MaxHeal ManaCost CastTime RankCoef HotDuration    */
            new SpellData( 1,  8,   45,   45, 0),
            new SpellData( 2, 14,  100,  100, 0),
            new SpellData( 3, 20,  175,  175, 0),
            new SpellData( 4, 26,  245,  245, 0),
            new SpellData( 5, 32,  315,  315, 0),
            new SpellData( 6, 38,  400,  400, 0),
            new SpellData( 7, 44,  510,  510, 0),
            new SpellData( 8, 50,  650,  650, 0),
            new SpellData( 9, 56,  810,  810, 0),
            new SpellData(10, 60,  970,  970, 0),
            new SpellData(11, 65, 1010, 1010, 0),
            new SpellData(12, 70, 1110, 1110, 0),
            new SpellData(13, 75, 1235, 1235, 0),
            new SpellData(14, 80, 1400, 1400, 0),
            };

        public override float AvgTotHeal
        {
            get
            {
                return AvgHeal + InstantHealEffect * (1f - CritChance) + InstantHealEffect * CritCoef * CritChance;
            }
        }

        public Renew(Stats stats, Character character)
			: base("Renew", stats, character, SpellRankTable, 17, 15f / 15f, 15f, Color.FromArgb(255, 0, 128, 0))
        {
            Calculate(stats, character);
        }

		protected void Calculate(Stats stats, Character character)
        {
            MinHeal = (MinHeal +
                stats.SpellPower * (SP2HP * HealingCoef * (1 - RankCoef)
                + character.PriestTalents.EmpoweredRenew * 0.05f))
                * (1 + character.PriestTalents.ImprovedRenew * 0.05f)
                * (1 + character.PriestTalents.TwinDisciplines * 0.01f)
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f);

            if (stats.RenewDurationIncrease > 0)
            {
                MinHeal = MinHeal * (HotDuration + stats.RenewDurationIncrease) / HotDuration;
                HotDuration += stats.RenewDurationIncrease;
            }
            if (character.PriestTalents.GlyphofRenew)
                HotDuration -= 3;

            InstantHealEffect = character.PriestTalents.EmpoweredRenew * 0.05f * MinHeal * 
                (1f + stats.PriestHeal_T9_4pc);
            MaxHeal = MinHeal;

            ManaCost = (int)Math.Floor((ManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1 - character.PriestTalents.MentalAgility * 0.1f / 3f));
            CastTime = 0;
        }

        public override string ToString()
        {
            return String.Format("{0} *HpS: {1}\r\nHpM: {2}\r\nTick: {3}\r\nInstant Heal: {4}\r\nCost: {5}\r\n{6}",
                          MinHeal.ToString("0"),
                          HpS.ToString("0.00"),
                          HpM.ToString("0.00"),
                          (MinHeal/HotDuration*3).ToString("0"),
                          InstantHealEffect.ToString("0"),
                          ManaCost.ToString("0"),
                          Name);
        }
    }

    public class FlashHeal : Spell
    {
        private static readonly List<SpellData> SpellRankTable = new List<SpellData>(){   
            new SpellData( 1, 20,  193,  237, 1.5f),
            new SpellData( 1, 26,  258,  314, 1.5f),
            new SpellData( 1, 32,  329,  393, 1.5f),
            new SpellData( 1, 38,  400,  478, 1.5f),
            new SpellData( 1, 44,  518,  616, 1.5f),
            new SpellData( 1, 50,  644,  764, 1.5f),
            new SpellData( 1, 56,  812,  958, 1.5f),
            new SpellData( 1, 61,  913, 1059, 1.5f),
            new SpellData( 1, 67, 1116, 1295, 1.5f),
            new SpellData( 1, 73, 1578, 1832, 1.5f),
            new SpellData( 1, 79, 1887, 2193, 1.5f)
        };

		public FlashHeal(Stats stats, Character character)
            : base("Flash Heal", stats, character, SpellRankTable, 18, 1.5f / 3.5f, Color.FromArgb(255, 154, 205, 50))
        {
            Calculate(stats, character);
        }

        protected void Calculate(Stats stats, Character character)
        {
            CalculationOptionsHealPriest calcOpts = character.CalculationOptions as CalculationOptionsHealPriest;

            MinHeal = (MinHeal +
                stats.SpellPower * ((1 - RankCoef) * HealingCoef * SP2HP
                + character.PriestTalents.EmpoweredHealing * 0.04f))
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f);

            MaxHeal = (MaxHeal +
                stats.SpellPower * ((1 - RankCoef) * HealingCoef * SP2HP
                + character.PriestTalents.EmpoweredHealing * 0.04f))
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f);

            CastTime = Math.Max(1.0f, BaseCastTime / (1 + stats.SpellHaste));

            CritChance = stats.SpellCrit + character.PriestTalents.HolySpecialization * 0.01f
                + character.PriestTalents.ImprovedFlashHeal * 0.04f * calcOpts.TestOfFaith / 100f;
            ManaCost = (int)Math.Floor((ManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1 - character.PriestTalents.ImprovedFlashHeal * 0.05f - (character.PriestTalents.GlyphofFlashHeal ? 0.1f : 0.0f)));
        }

        public void SurgeOfLight()
        {
            CritChance = 0f;
            CritCoef = 1.0f;
            ManaCost = 0;
            CastTime = 0;
       }

    }

    public class Heal : Spell
    {
        private static readonly List<SpellData> SpellRankTable = new List<SpellData>(){   
            new SpellData( 1,  1,   46,   56, 1.5f),
            new SpellData( 2,  4,   71,   85, 2.0f),
            new SpellData( 3, 10,  135,  157, 2.5f),
            new SpellData( 1, 16,  295,  341, 3.0f),
            new SpellData( 2, 22,  429,  491, 3.0f),
            new SpellData( 3, 28,  566,  642, 3.0f),
            new SpellData( 4, 34,  712,  804, 3.0f),
            new SpellData( 1, 40,  899, 1013, 3.0f),
            new SpellData( 2, 46, 1149, 1289, 3.0f),
            new SpellData( 3, 52, 1437, 1609, 3.0f),
            new SpellData( 4, 58, 1798, 2006, 3.0f),
            new SpellData( 5, 60, 1966, 2194, 3.0f),
            new SpellData( 6, 63, 2074, 2410, 3.0f),
            new SpellData( 7, 40, 2414, 2803, 3.0f),
            new SpellData( 8, 40, 3395, 3945, 3.0f),
            new SpellData( 9, 40, 3950, 4590, 3.0f)
        };

		public Heal(Stats stats, Character character)
            : base("Heal", stats, character, SpellRankTable, 32, 3f / 3.5f, Color.FromArgb(255, 143, 188, 143))
        {
            if (character.Level < 16)
                Name = "Lesser " + Name;
            else if (character.Level >= 40)
                Name = "Greater " + Name;
            Calculate(stats, character);
        }

        protected void Calculate(Stats stats, Character character)
        {
            MinHeal = (MinHeal +
                stats.SpellPower * ((1 - RankCoef) * HealingCoef * SP2HP
                + character.PriestTalents.EmpoweredHealing * 0.08f))
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f)
                * (1 + stats.BonusGHHealingMultiplier);

            MaxHeal = (MaxHeal +
                stats.SpellPower * ((1 - RankCoef) * HealingCoef * SP2HP
                + character.PriestTalents.EmpoweredHealing * 0.08f))
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f)
                * (1 + stats.BonusGHHealingMultiplier);

            ManaCost = (int)Math.Floor((ManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1 - character.PriestTalents.ImprovedHealing * 0.05f)
                * (1 - stats.GreaterHealCostReduction));

            CritChance = stats.SpellCrit + character.PriestTalents.HolySpecialization * 0.01f;
            CastTime = Math.Max(1.0f, (BaseCastTime - character.PriestTalents.DivineFury * 0.1f) 
                / (1 + stats.SpellHaste));
        }
    }

    public class PrayerOfHealing : Spell
    {
        private static readonly List<SpellData> SpellRankTable = new List<SpellData>(){   
            new SpellData(1, 30,  301,  321, 3.0f),
            new SpellData(2, 40,  444,  472, 3.0f),
            new SpellData(3, 50,  657,  695, 3.0f),
            new SpellData(4, 60,  939,  991, 3.0f),
            new SpellData(5, 60,  997, 1053, 3.0f),
            new SpellData(6, 68, 1251, 1322, 3.0f),
            new SpellData(7, 76, 2091, 2209, 3.0f),
        };

        private static readonly Color[] targetColors = new Color[]
                                           {
                                               Color.FromArgb(255, 255, 165, 0),
                                               Color.FromArgb(255, 255, 165, 0),
                                               Color.FromArgb(255, 255, 165, 0),
                                               Color.FromArgb(255, 255, 69, 0),
                                               Color.FromArgb(255, 255, 140, 0)
                                           };

        public override float AvgTotHeal
        {
            get
            {
                return (AvgHeal * (1 - CritChance) + AvgCrit * CritChance) * Targets;
            }
        }
        
        public override float HpS
        {
            get
            {
                return AvgTotHeal / CastTime;
            }
        }

        public override float HpM
        {
            get
            {
                return AvgTotHeal / ManaCost;
            }
        }

        public PrayerOfHealing(Stats stats, Character character, int targets, float haste)
            : base(string.Format("Prayer of Healing ({0} targets)", targets), stats, character, SpellRankTable, 48, 2f / 3.5f * 0.5f, targetColors[targets - 1])
        {
            Calculate(stats, character, targets, haste);
        }

        public PrayerOfHealing(Stats stats, Character character)
            : this(stats, character, 5, 0f)
        {}

        public PrayerOfHealing(Stats stats, Character character, int targets)
            : this(stats, character, targets, 0f)
        {}

        protected void Calculate(Stats stats, Character character, int targets, float haste)
        {
            Targets = targets;
            Range = 30;

            MinHeal = (MinHeal +
                stats.SpellPower * SP2HP * HealingCoef * (1 - RankCoef))
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.DivineProvidence * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f);
            MaxHeal = (MaxHeal +
                stats.SpellPower * SP2HP * HealingCoef * (1 - RankCoef))
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.DivineProvidence * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f);

            ManaCost = (int)Math.Floor((ManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1 - character.PriestTalents.HealingPrayers * 0.1f) 
                * (1 - stats.BonusPoHManaCostReductionMultiplier));

            CastTime = Math.Max(1.0f, (BaseCastTime * (1f - haste)) / (1 + stats.SpellHaste));
            CritChance = stats.SpellCrit
                + character.PriestTalents.HolySpecialization * 0.01f
                + stats.PrayerOfHealingExtraCrit;
            Range = (int)Math.Round(Range * (1 + character.PriestTalents.HolyReach * 0.1f));
        }

        public override string ToString()
        {
            return String.Format("{0} *HpS(3): {1}\r\nHpS(4): {10}\r\nHpS(5): {12}\r\nHpM(3): {2}\r\nHpM(4): {11}\r\nHpM(5): {13}\r\nMin Heal: {3}\r\nMax Heal: {4}\r\nAvg Crit: {5}\r\nMax Crit: {6}\r\nCast: {7}\r\nCost: {8}\r\nRange: {9}\r\n{10}",
                AvgHeal.ToString("0"),
                (AvgHeal * 3 / CastTime).ToString("0.00"),
                (AvgHeal * 3 / ManaCost).ToString("0.00"),
                MinHeal.ToString("0"),
                MaxHeal.ToString("0"),
                AvgCrit.ToString("0"),
                MaxCrit.ToString("0"),
                CastTime.ToString("0.00"),
                ManaCost.ToString("0"),
                Range,
                (AvgHeal * 4 / CastTime).ToString("0.00"),
                (AvgHeal * 4 / ManaCost).ToString("0.00"),
                (AvgHeal * 5 / CastTime).ToString("0.00"),
                (AvgHeal * 5 / ManaCost).ToString("0.00"),
                Name);
        }
    }

    public class CircleOfHealing : Spell
    {
        private static readonly List<SpellData> SpellRankTable = new List<SpellData>(){   
            new SpellData(1, 50,  343,  379, 0f),
            new SpellData(1, 56,  403,  445, 0f),
            new SpellData(1, 60,  458,  506, 0f),
            new SpellData(1, 65,  518,  572, 0f),
            new SpellData(1, 70,  572,  632, 0f),
            new SpellData(1, 75,  825,  911, 0f),
            new SpellData(1, 80,  958,  1058, 0f),
        };

        public override float AvgTotHeal
        {
            get
            {
                return (AvgHeal * (1 - CritChance) + AvgCrit * CritChance) * Targets;
            }
        }        
        
        public override float HpS
        {
            get
            {
                return AvgTotHeal / GlobalCooldown;
            }
        }

        public override float HpM
        {
            get
            {
                return AvgTotHeal / ManaCost;
            }
        }

        public CircleOfHealing(Stats stats, Character character, int targets)
            : base(string.Format("Circle of Healing ({0} targets)", targets), stats, character, SpellRankTable, 21, 1.5f / 3.5f * 0.5f, Color.FromArgb(255, 255, 215, 0))
        {
            Targets = targets;
            Calculate(stats, character);
        }

        public CircleOfHealing(Stats stats, Character character)
            : this(stats, character, character.PriestTalents.GlyphofCircleOfHealing ? 6 : 5)
        {}

        protected void Calculate(Stats stats, Character character)
        {
            if (character.PriestTalents.CircleOfHealing == 0)
            {
                MinHeal = MaxHeal = 0;
                return;
            }
            Range = 15;
            MinHeal = (MinHeal +
                stats.SpellPower * SP2HP * HealingCoef * (1 - RankCoef))
                * (1 + character.PriestTalents.TwinDisciplines * 0.01f)
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.DivineProvidence * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f)
                * (1 + stats.PriestHeal_T10_4pc * 2f);
            MaxHeal = (MaxHeal +
                stats.SpellPower * SP2HP * HealingCoef * (1 - RankCoef))
                * (1 + character.PriestTalents.TwinDisciplines * 0.01f)
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.DivineProvidence * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f)
                * (1 + stats.PriestHeal_T10_4pc * 2f);

            CritChance = stats.SpellCrit + character.PriestTalents.HolySpecialization * 0.01f;

            ManaCost = (int)Math.Floor((ManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1 - character.PriestTalents.MentalAgility * 0.1f / 3f));
            
            Range = (int)Math.Round(Range * (1 + character.PriestTalents.HolyReach * 0.1f));
            CastTime = 0;
        }

        public override string ToString()
        {
            return String.Format("{0} *HpS(3): {1}\r\nHpS(4): {9}\r\nHpS(5): {11}\r\nHpM(3): {2}\r\nHpM(4): {10}\r\nHpM(5): {12}\r\nMin Heal: {3}\r\nMax Heal: {4}\r\nAvg Crit: {5}\r\nMax Crit: {6}\r\nCost: {7}\r\nRange: {8}\r\n{9}",
                AvgHeal.ToString("0"),
                (AvgHeal * 3 / GlobalCooldown).ToString("0.00"),
                (AvgHeal * 3 / ManaCost).ToString("0.00"),
                MinHeal.ToString("0"),
                MaxHeal.ToString("0"),
                AvgCrit.ToString("0"),
                MaxCrit.ToString("0"),
                ManaCost.ToString("0"),
                Range,
                (AvgHeal * 4 / GlobalCooldown).ToString("0.00"),
                (AvgHeal * 4 / ManaCost).ToString("0.00"),
                (AvgHeal * 5 / GlobalCooldown).ToString("0.00"),
                (AvgHeal * 5 / ManaCost).ToString("0.00"),
                Name);
        }
    }

    public class HolyNova : Spell
    {
        private static readonly List<SpellData> SpellRankTable = new List<SpellData>(){   
            new SpellData(1, 20,  52,  60, 0f),
            new SpellData(2, 28,  86,  98, 0f),
            new SpellData(3, 36, 121, 139, 0f),
            new SpellData(4, 44, 161, 187, 0f),
            new SpellData(5, 52, 235, 271, 0f),
            new SpellData(6, 60, 302, 350, 0f),
            new SpellData(7, 68, 384, 446, 0f),
            new SpellData(8, 75, 611, 709, 0f),
            new SpellData(9, 80, 713, 827, 0f)
        };

        private static readonly Color[] targetColors = new Color[]
                                           {
                                               Color.FromArgb(255, 255, 127, 80),
                                               Color.FromArgb(255, 255, 127, 80),
                                               Color.FromArgb(255, 255, 127, 80),
                                               Color.FromArgb(255, 222, 184, 135),
                                               Color.FromArgb(255, 165, 42, 42)
                                           };

        public override float AvgTotHeal
        {
            get
            {
                return (AvgHeal * (1 - CritChance) + AvgCrit * CritChance) * Targets;
            }
        }
        
        public override float HpS
        {
            get
            {
                return AvgTotHeal / GlobalCooldown;
            }
        }

        public override float HpM
        {
            get
            {
                return AvgTotHeal / ManaCost;
            }
        }

        public HolyNova(Stats stats, Character character, int targets)
            : base(string.Format("Holy Nova ({0} targets)", targets), stats, character, SpellRankTable, 20, 0.16f, targetColors[targets - 1])
        {
            Targets = targets;
            Calculate(stats, character);
        }

        public HolyNova(Stats stats, Character character)
            : this(stats, character, 5)
        {}

        protected void Calculate(Stats stats, Character character)
        {
            Range = 10;
            
            MinHeal = (MinHeal +
                stats.SpellPower * SP2HP * HealingCoef * (1 - RankCoef))
                * (1 + character.PriestTalents.TwinDisciplines * 0.01f)
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.DivineProvidence * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f)
                * (character.PriestTalents.GlyphofHolyNova ? 1.4f : 1.0f);
            MaxHeal = (MaxHeal +
                stats.SpellPower * SP2HP * HealingCoef * (1 - RankCoef))
                * (1 + character.PriestTalents.TwinDisciplines * 0.01f)
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.DivineProvidence * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f)
                * (character.PriestTalents.GlyphofHolyNova ? 1.4f : 1.0f);

            CritChance = stats.SpellCrit + character.PriestTalents.HolySpecialization * 0.01f;

            ManaCost = (int)Math.Floor((ManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1 - character.PriestTalents.MentalAgility * 0.1f / 3f));
            
            Range = (int)Math.Round(Range * (1 + character.PriestTalents.HolyReach * 0.1f));
            CastTime = 0;
        }

        public override string ToString()
        {
            return String.Format("{0} *HpS(3): {1}\r\nHpS(4): {9}\r\nHpS(5): {11}\r\nHpM(3): {2}\r\nHpM(4): {10}\r\nHpM(5): {12}\r\nMin Heal: {3}\r\nMax Heal: {4}\r\nAvg Crit: {5}\r\nMax Crit: {6}\r\nCost: {7}\r\nRange: {8}\r\n{9}",
                AvgHeal.ToString("0"),
                (AvgHeal * 3 / GlobalCooldown).ToString("0.00"),
                (AvgHeal * 3 / ManaCost).ToString("0.00"),
                MinHeal.ToString("0"),
                MaxHeal.ToString("0"),
                AvgCrit.ToString("0"),
                MaxCrit.ToString("0"),
                ManaCost.ToString("0"),
                Range,
                (AvgHeal * 4 / GlobalCooldown).ToString("0.00"),
                (AvgHeal * 4 / ManaCost).ToString("0.00"),
                (AvgHeal * 5 / GlobalCooldown).ToString("0.00"),
                (AvgHeal * 5 / ManaCost).ToString("0.00"),
                Name);
        }
    }

    public class BindingHeal : Spell
    {
        private static readonly List<SpellData> SpellRankTable = new List<SpellData>(){   
        /*                Rank MinHeal MaxHeal ManaCost CastTime RankCoef    */
            new SpellData(1, 64, 1042, 1338, 1.5f),
            new SpellData(2, 72, 1619, 2081, 1.5f),
            new SpellData(3, 78, 1952, 2508, 1.5f)
            };

        public override float AvgTotHeal
        {
            get
            {
                return (AvgHeal * (1 - CritChance) + AvgCrit * CritChance) * 2;
            }
        }

        public override float HpS
        {
            get
            {
                return AvgTotHeal / CastTime;
            }
        }

        public override float HpM
        {
            get
            {
                return AvgTotHeal / ManaCost;
            }
        }

        public BindingHeal(Stats stats, Character character)
            : base("Binding Heal", stats, character, SpellRankTable, 27, 1.5f / 3.5f, Color.FromArgb(255, 128, 0, 128))
        {
            Calculate(stats, character);
        }

        protected void Calculate(Stats stats, Character character)
        {
            MinHeal = (MinHeal +
                stats.SpellPower * ((1 - RankCoef) * HealingCoef * SP2HP
                + character.PriestTalents.EmpoweredHealing * 0.04f))
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.DivineProvidence * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f);

            MaxHeal = (MaxHeal +
                stats.SpellPower * ((1 - RankCoef) * HealingCoef * SP2HP 
                + character.PriestTalents.EmpoweredHealing * 0.04f))
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.DivineProvidence * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f);

            ManaCost = (int)Math.Floor((ManaCost / 100f * BaseMana - stats.SpellsManaReduction));

            CritChance = stats.SpellCrit + character.PriestTalents.HolySpecialization * 0.01f;
            CastTime = Math.Max(1.0f, BaseCastTime / (1 + stats.SpellHaste));
        }

        public override string ToString()
        {
            return String.Format("{0} *HpS(2): {1}\r\nHpM (2): {2}\r\nMin Heal: {3}\r\nMax Heal: {4}\r\nAvg Crit: {5}\r\nMax Crit: {6}\r\nCast Time: {7}\r\nCost: {8}\r\n{9}",
                AvgHeal.ToString("0"),
                HpS.ToString("0.00"),
                HpM.ToString("0.00"),
                MinHeal.ToString("0"),
                MaxHeal.ToString("0"),
                AvgCrit.ToString("0"),
                MaxCrit.ToString("0"),
                CastTime.ToString("0.00"),
                ManaCost.ToString("0"),
                Name);
        }
    }

    public class PrayerOfMending : Spell
    {
        private static readonly List<SpellData> SpellRankTable = new List<SpellData>(){   
            new SpellData(1, 68,  800,  800, 0),
            new SpellData(2, 74,  905,  905, 0),
            new SpellData(3, 79, 1043, 1043, 0)
        };

        public override float AvgTotHeal
        {
            get
            {
                return (AvgHeal * (1 - CritChance) + AvgCrit * CritChance) * Targets;
            }
        }

        public override float HpS
        {
            get
            {
                return AvgTotHeal / GlobalCooldown;
            }
        }

        public override float HpM
        {
            get
            {
                return AvgTotHeal / ManaCost;
            }
        }

        public PrayerOfMending(Stats stats, Character character, int targets)
            : base(string.Format("Prayer of Mending ({0} targets)", targets), stats, character, SpellRankTable, 15, 1.5f / 3.5f, Color.FromArgb(255, 0, 255, 255))
        {
            Targets = targets;
            Calculate(stats, character);
        }

        public PrayerOfMending(Stats stats, Character character)
            : this(stats, character, 5 + (int)stats.PrayerOfMendingExtraJumps)
        {}

        protected void Calculate(Stats stats, Character character)
        {
            Range = 15;

            MinHeal = (MinHeal +
                stats.SpellPower * SP2HP * HealingCoef * (1 - RankCoef))
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.TwinDisciplines * 0.01f)
                * (1 + character.PriestTalents.DivineProvidence * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f)
                * (1 + stats.PriestHeal_T9_2pc);
            MaxHeal = (MaxHeal +
                stats.SpellPower * SP2HP * HealingCoef * (1 - RankCoef))
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.TwinDisciplines * 0.01f)
                * (1 + character.PriestTalents.DivineProvidence * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f)
                * (1 + stats.PriestHeal_T9_2pc);

            ManaCost = (int)Math.Floor((ManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1 - character.PriestTalents.HealingPrayers * 0.1f) 
                * (1 - character.PriestTalents.MentalAgility * 0.1f / 3f));
            Range = (int)Math.Round(Range * (1 + character.PriestTalents.HolyReach * 0.1f));

            CritChance = stats.SpellCrit + character.PriestTalents.HolySpecialization * 0.01f;
            CastTime = 0;
            Cooldown = 10.0f - (character.PriestTalents.DivineProvidence * 0.6f);
        }

        public override string ToString()
        {
            return String.Format("{0} *HpM(1): {1}\r\nHpM(2): {2}\r\nHpM 3): {3}\r\nHpM(4): {4}\r\nHpM(5): {5}\r\nCrit: {6}\r\nCost: {7}\r\nRange: {8}\r\n{9}",
                AvgHeal.ToString("0"),
                (AvgHeal / ManaCost).ToString("0.00"),
                (AvgHeal * 2 / ManaCost).ToString("0.00"),
                (AvgHeal * 3 / ManaCost).ToString("0.00"),
                (AvgHeal * 4 / ManaCost).ToString("0.00"),
                (AvgHeal * 5 / ManaCost).ToString("0.00"),
                AvgCrit.ToString("0"),
                ManaCost.ToString("0"),
                Range,
                Name);
        }
    }

    public class PowerWordShield : Spell
    {
        static readonly List<SpellData> SpellRankTable = new List<SpellData>() {
            new SpellData( 1,  6,   44,   44, 0f),
            new SpellData( 2, 12,   88,   88, 0f),
            new SpellData( 3, 18,  158,  158, 0f),
            new SpellData( 4, 24,  234,  234, 0f),
            new SpellData( 5, 30,  301,  301, 0f),
            new SpellData( 6, 36,  381,  381, 0f),
            new SpellData( 7, 42,  484,  484, 0f),
            new SpellData( 8, 48,  605,  605, 0f),
            new SpellData( 9, 54,  763,  763, 0f),
            new SpellData(10, 60,  942,  942, 0f),
            new SpellData(11, 65, 1125, 1125, 0f),
            new SpellData(12, 70, 1265, 1265, 0f),
            new SpellData(13, 75, 1920, 1920, 0f),
            new SpellData(14, 80, 2230, 2230, 0f)
        };


        public override float HpS
        {
            get
            {
                return AvgHeal / GlobalCooldown;
            }
        }

        public PowerWordShield(Stats stats, Character character)
            : base("Power Word Shield", stats, character, SpellRankTable, 23, 1.5f / 3.5f, Color.FromArgb(255, 112, 128, 144))
        {
            Calculate(stats, character);
        }

        protected void Calculate(Stats stats, Character character)
        {
            MinHeal = MaxHeal = (MinHeal //* (1 + character.PriestTalents.ImprovedPowerWordShield * 0.05f)
                + stats.SpellPower * SP2HP * HealingCoef * (1 - RankCoef)
                + stats.SpellPower * character.PriestTalents.BorrowedTime * 0.08f)
                * (1 + character.PriestTalents.TwinDisciplines * 0.01f
                    + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.ImprovedPowerWordShield * 0.05f)
                * (1 + stats.PriestHeal_T10_4pc);

            ManaCost = (int)Math.Floor((ManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1 - character.PriestTalents.MentalAgility * 0.1f / 3f 
                     - character.PriestTalents.SoulWarding * 0.15f) );

            CritChance = 0.0f;
            CritCoef = 1.0f;

            if (character.PriestTalents.SoulWarding > 0)
                Cooldown = 0.0f;
            else
                Cooldown = 4f;

            CastTime = 0.0f;
        }

        public override string ToString()
        {
            return String.Format("{0} *HpM: {1}\r\nCost: {2}\r\n{3}",
                MinHeal.ToString("0"),
                HpM.ToString("0.00"),
                ManaCost.ToString("0"),
                Name);
        }
    }

    public class Lightwell : Spell
    {
        private static readonly List<SpellData> SpellRankTable = new List<SpellData>(){   
            new SpellData(1, 40,  801,  801, 0.5f),
            new SpellData(2, 50, 1164, 1164, 0.5f),
            new SpellData(3, 60, 1599, 1599, 0.5f),
            new SpellData(4, 70, 2361, 2361, 0.5f),
            new SpellData(5, 75, 3915, 3915, 0.5f),
            new SpellData(6, 80, 4620, 4620, 0.5f)
        };

        public override float HpS
        {
            get
            {
                return AvgHeal / HotDuration;
            }
        }

        public Lightwell(Stats stats, Character character)
            : base("Lightwell", stats, character, SpellRankTable, 17, 1f, 6, Color.FromArgb(255, 128, 128, 128))
        {
            Calculate(stats, character);
        }

        protected void Calculate(Stats stats, Character character)
        {
            MinHeal = (MinHeal +
                stats.SpellPower * SP2HP * HealingCoef * (1 - RankCoef)) 
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (character.PriestTalents.GlyphofLightwell ? 1.2f : 1.0f);
            MaxHeal = MinHeal;

            CritChance = 0f;
            CritCoef = 1.0f;

            ManaCost =(int)Math.Floor((ManaCost / 100f * BaseMana - stats.SpellsManaReduction));

            CastTime = 0;
            Cooldown = 3.0f * 60.0f;
        }

        public override string ToString()
        {
            return String.Format("{0} *HpS: {1}\r\nHpM: {2}\r\nTick: {3}\r\nCost: {4}\r\n{5}",
                          MinHeal.ToString("0"),
                          HpS.ToString("0.00"),
                          HpM.ToString("0.00"),
                          (MinHeal / HotDuration * 2).ToString("0"),
                          ManaCost.ToString("0"),
                          Name);
        }
    }

    public class Penance : Spell
    {
        private static readonly List<SpellData> SpellRankTable = new List<SpellData>(){   
            new SpellData(1, 60,  670*3,  756*3, 2f),
            new SpellData(2, 70,  805*3,  909*3, 2f),
            new SpellData(3, 75, 1278*3, 1442*3, 2f),
            new SpellData(4, 80, 1484*3, 1676*3, 2f),
        };

        public Penance(Stats stats, Character character)
            : base("Penance", stats, character, SpellRankTable, 16, 3f / 3.5f, Color.FromArgb(255, 128, 128, 128))
        {
            Calculate(stats, character);
        }

        protected void Calculate(Stats stats, Character character)
        {
            if (character.PriestTalents.Penance == 0)
            {
                MinHeal = MaxHeal = 0;
                return;
            }

            MinHeal = (MinHeal +
                stats.SpellPower * SP2HP * (1 - RankCoef) * HealingCoef)
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.TwinDisciplines * 0.01f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f);

            MaxHeal = (MaxHeal +
                stats.SpellPower * SP2HP * (1 - RankCoef) * HealingCoef)
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.TwinDisciplines * 0.01f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f);


            ManaCost = (int)Math.Floor((ManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1 - character.PriestTalents.ImprovedHealing * 0.05f));

            CastTime = Math.Max(1.0f, BaseCastTime / (1 + stats.SpellHaste));

            CritChance = stats.SpellCrit + character.PriestTalents.HolySpecialization * 0.01f;
            Cooldown = (12.0f - (character.PriestTalents.GlyphofPenance ? 2 : 0)) * (1f - character.PriestTalents.Aspiration * 0.1f);
        }

        public override string ToString()
        {
            return String.Format("{0} *HpS: {1}\r\nHpM: {2}\r\nMin Tick: {3}\r\nMax Tick: {4}\r\nAvg Tick: {5}\r\nMax Crit Tick: {6}\r\nAvg Crit Tick: {7}\r\nAvg Heal: {8}\r\nMin Heal: {9}\r\nMax Heal: {10}\r\nAvg Crit: {11}\r\nMax Crit: {12}\r\nCast: {13}\r\nCost: {14}\r\n{15}",
                AvgHeal.ToString("0"),
                HpS.ToString("0.00"),
                HpM.ToString("0.00"),
                (MinHeal / 3).ToString("0.00"),
                (MaxHeal / 3).ToString("0.00"),
                (AvgHeal / 3).ToString("0.00"),
                (MaxCrit / 3).ToString("0.00"),
                (AvgCrit / 3).ToString("0.00"),
                AvgHeal.ToString("0.00"),
                MinHeal.ToString("0"),
                MaxHeal.ToString("0"),
                AvgCrit.ToString("0"),
                MaxCrit.ToString("0"),
                CastTime.ToString("0.00"),
                ManaCost.ToString("0"),
                Name);
        }
    }

    public class GiftOfTheNaaru : Spell
    {
        public GiftOfTheNaaru(Stats stats, Character character)
            : base("Gift of the Naaru", stats, character, new List<SpellData>() { new SpellData(1, 1, character.Level*15+35, character.Level*15+35, 1.5f) }, 0, 15f / 15f, 15f, Color.FromArgb(255, 0, 128, 0))
        {
            Calculate(stats, character);
        }

        protected void Calculate(Stats stats, Character character)
        {
            MinHeal = MaxHeal = (MinHeal +
                stats.SpellPower * SP2HP * HealingCoef)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f);

            ManaCost = (int)Math.Floor(ManaCost / 100f * BaseMana);

            CritChance = 0.0f;
            CritCoef = 1.0f;

            CastTime = Math.Max(1.0f, BaseCastTime / (1 + stats.SpellHaste));

            Cooldown = 3.0f * 60.0f;
        }

        public override string ToString()
        {
            return String.Format("{0} *HpS: {1}\r\nHpM: {2}\r\nTick: {3}\r\n{4}",
                          MinHeal.ToString("0"),
                          HpS.ToString("0.00"),
                          HpM.ToString("0.00"),
                          (MinHeal / HotDuration * 3).ToString("0"),
                          Name);
        }
    }

    public class DivineHymn : Spell
    {
        private static readonly List<SpellData> SpellRankTable = new List<SpellData>(){   
            new SpellData(1, 80,  3000*4, 3850*4, 8f),
        };

        public DivineHymn(Stats stats, Character character)
            : base("Divine Hymn", stats, character, SpellRankTable, 63, 8f / 3.5f, 8f, Color.FromArgb(255, 255, 255, 255))
        {
            Calculate(stats, character);
        }

        public override float AvgTotHeal
        {
            get
            {
                return (AvgHeal * (1 - CritChance) + AvgCrit * CritChance) * 3;
            }
        }

        protected void Calculate(Stats stats, Character character)
        {
            MinHeal = (MinHeal +
                stats.SpellPower * SP2HP * HealingCoef * (1 - RankCoef))
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.DivineProvidence * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f);
            MaxHeal = (MaxHeal +
                stats.SpellPower * SP2HP * HealingCoef * (1 - RankCoef))
                * (1 + character.PriestTalents.FocusedPower * 0.02f)
                * (1 + character.PriestTalents.DivineProvidence * 0.02f)
                * (1 + character.PriestTalents.SpiritualHealing * 0.02f)
                * (1 + character.PriestTalents.BlessedResilience * 0.01f);

            ManaCost = (int)Math.Floor((ManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1 - character.PriestTalents.MentalAgility * 0.1f / 3f));

            CastTime = Math.Max(1.0f, BaseCastTime / (1 + stats.SpellHaste));
            CritChance = stats.SpellCrit + character.PriestTalents.HolySpecialization * 0.01f;
            Range = (int)Math.Round(Range * (1 + character.PriestTalents.HolyReach * 0.1f));
        }

        public override string ToString()
        {
            return String.Format("{0} *HpS: {1}\r\nHpM: {2}\r\nTick: {3}\r\nCost: {4}\r\n{5}",
                          AvgTotHeal.ToString("0"),
                          HpS.ToString("0.00"),
                          HpM.ToString("0.00"),
                          (AvgTotHeal / HotDuration * 2 / 3).ToString("0"),
                          ManaCost.ToString("0"),
                          Name);
        }  
    }

    public class Dispel : Spell
    {
        public Dispel(Stats stats, Character character)
            : base("Dispel", stats, character, new List<SpellData>() { new SpellData(1, 30, 0, 0, 0) }, 12, 0f, Color.FromArgb(255, 128, 128, 128))
        {
            Calculate(stats, character);
        }

        protected void Calculate(Stats stats, Character character)
        {
            MinHeal = MaxHeal = 0;
            ManaCost = (int)Math.Floor((ManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1 - character.PriestTalents.MentalAgility * 0.1f / 3f)
                * (1 - character.PriestTalents.Absolution * 0.05f));
            CritChance = 0.0f;
            CritCoef = 1.0f;

            CastTime = 0.0f;

        }

        public override string ToString()
        {
            return String.Format("{0}",
                Name);
        }
    }

    public class MassDispel : Spell
    {
        public MassDispel(Stats stats, Character character)
            : base("MassDispel", stats, character, new List<SpellData>() { new SpellData(1, 30, 0, 0, 1.5f) }, 36, 0f, Color.FromArgb(255, 128, 128, 128))
        {
            Calculate(stats, character);
        }

        protected void Calculate(Stats stats, Character character)
        {
            MinHeal = MaxHeal = 0;
            ManaCost = (int)Math.Floor((ManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (character.PriestTalents.GlyphofMassDispel ? 0.7f : 1.0f) );
            CritChance = 0.0f;
            CritCoef = 1.0f;
        }

        public override string ToString()
        {
            return String.Format("{0}",
                Name);
        }
    }

    public class Resurrection : Spell
    {
        public Resurrection(Stats stats, Character character)
            : base("Resurrection", stats, character, new List<SpellData>() { new SpellData(1, 10, 0, 0, 10f) }, 60, 0, Color.FromArgb(255, 128, 128, 128))
        {
            Calculate(stats, character);
        }

        protected void Calculate(Stats stats, Character character)
        {
            MinHeal = MaxHeal = 0;
            ManaCost = (int)Math.Floor((ManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                );
            CritChance = 0.0f;
            CritCoef = 1.0f;
            CastTime = Math.Max(1.0f, BaseCastTime / (1 + stats.SpellHaste));
        }

        public override string ToString()
        {
            return String.Format("- *Cast Time: {0}\r\nCost: {1}\r\n{2}",
                CastTime.ToString("0.00"),
                ManaCost.ToString("0"),
                Name);
        }
    }
}
