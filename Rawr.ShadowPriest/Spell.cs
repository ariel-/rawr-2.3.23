using System;
using System.Collections.Generic;
#if RAWR3
using System.Windows.Media;
#else
using System.Drawing;
#endif
using System.Text;

namespace Rawr.ShadowPriest {
    public static class SpellFactory {
        public static Spell CreateSpell(string name, Stats stats, Character character, float baseMana, bool ptr) {
            PriestTalents talents = character.PriestTalents;
            switch (name) {
                case "Vampiric Embrace":    return (talents.VampiricEmbrace > 0 ? new VampiricEmbrace(stats, character, baseMana, ptr) : null);
                case "Vampiric Touch":      return (talents.VampiricTouch > 0 ? new VampiricTouch(stats, character, baseMana, ptr) : null);
                case "Shadow Word: Pain":   return new ShadowWordPain(stats, character, baseMana, ptr);
                case "Devouring Plague":    return new DevouringPlague(stats, character, baseMana, ptr);
                case "Mind Blast":          return new MindBlast(stats, character, baseMana, ptr);
                case "Shadow Word: Death":  return new ShadowWordDeath(stats, character, baseMana, ptr);
                case "Mind Flay":           return (talents.MindFlay > 0 ? new MindFlay(stats, character, baseMana, ptr) : null);
                case "Smite":               return new Smite(stats, character, baseMana, ptr);
                case "Holy Fire":           return new HolyFire(stats, character, baseMana, ptr);
                case "Penance":             return (talents.Penance > 0 ? new Penance(stats, character, baseMana, ptr) : null);
                default:                    return null;
            }
        }
    }
    public class SpellStatistics {
        public float CritCount { get; set; }
        public float MissCount { get; set; }
        public float HitCount { get; set; }
        public float CooldownReset { get; set; }
        public float DamageDone { get; set; }
        public float ManaUsed { get; set; }
        public void Reset()
        {
            CritCount = 0;
            MissCount = 0;
            HitCount = 0;
            CooldownReset = 0;
            DamageDone = 0;
            ManaUsed = 0;
        }
    }

    public class Spell {
        public class SpellData {
            public int Rank { get; protected set; }
            public int Level { get; protected set; }
            public int MinDamage { get; protected set; }
            public int MaxDamage { get; protected set; }

            public SpellData(int rank, int level, int minDamage, int maxDamage) {
                Rank = rank;
                Level = level;
                MinDamage = minDamage;
                MaxDamage = maxDamage;
            }
        }

        public static readonly List<string> ShadowSpellList = new List<string>() { "Vampiric Touch", "Devouring Plague", "Mind Blast", "Shadow Word: Pain", "Mind Flay", "Shadow Word: Death" };
        public static readonly List<string> HolySpellList = new List<string>() { "Penance", "Holy Fire", "Devouring Plague", "Shadow Word: Pain", "Mind Blast", "Shadow Word: Death", "Smite" };

        public string Name { get; protected set; }

        public MagicSchool MagicSchool { get; protected set; }
        public int Rank { get; protected set; }
        public float BaseMinDamage { get; protected set; }
        public float MinDamage { get; protected set; }
        public float BaseMaxDamage { get; protected set; }
        public float MaxDamage { get; protected set; }
        public float BaseDamageCoef { get; protected set; }
        public float DamageCoef { get; protected set; }
        public float BaseRange { get; protected set; }
        public int Range { get; protected set; }
        public int BaseManaCost { get; protected set; }
        public int ManaCost { get; protected set; }
        public float BaseCastTime { get; protected set; }
        public float CastTime { get; protected set; }
        public float CritChance { get; protected set; }
        public float BaseCritCoef { get; protected set; }
        public float CritCoef { get; protected set; }
        public float BaseDebuffDuration { get; protected set; }
        public float DebuffDuration { get; protected set; }
        public int BaseDebuffTicks { get; protected set; }
        public int DebuffTicks { get; protected set; }
        public int Targets { get; protected set; }

        public float Cooldown { get; protected set; }
        public float BaseCooldown { get; protected set; }
        public float GlobalCooldown { get; protected set; }
        public Color GraphColor { get; protected set; }

        public SpellStatistics SpellStatistics { get; protected set; }

        protected float BaseMana;
        protected bool Ptr;

        #region Properties
        public float AvgHit { get { return (MinDamage + MaxDamage) / 2f; } }
        public virtual float AvgDamage { get { return AvgHit * (1f - CritChance) + AvgCrit * CritChance; } }
        public virtual float DpCT { get { return AvgDamage / (CastTime > 0 ? CastTime: GlobalCooldown); } }
        public virtual float DpS { get { return AvgDamage / (DebuffDuration > 0 ? DebuffDuration : (CastTime > 0 ? CastTime : GlobalCooldown)); } }
        public virtual float DpM { get { return AvgDamage / ManaCost; } }
        public float AvgCrit { get { return (MinCrit + MaxCrit) / 2f; } }
        public float MaxCrit { get { return MaxDamage * CritCoef; } }
        public float MinCrit { get { return MinDamage * CritCoef; } }
        #endregion

        public Spell(string name, Stats stats, Character character, List<SpellData> SpellRankTable, int manaCost, float castTime, float critCoef, float dotDuration, float damageCoef, int range, float cooldown, Color col, MagicSchool magicSchool, float baseMana, bool ptr) {
            DamageCoef = damageCoef;
            DebuffDuration = dotDuration;
            GraphColor = col;
            foreach (SpellData sd in SpellRankTable){
                if (character.Level >= sd.Level) {
                    Rank = sd.Rank;
                    BaseMinDamage = MinDamage = sd.MinDamage;
                    BaseMaxDamage = MaxDamage = sd.MaxDamage;
                }
            }
            //Name = string.Format("{0}, Rank {1}", name, Rank);
            Name = name;
            BaseManaCost = ManaCost = manaCost;
            BaseCastTime = CastTime = castTime;
            BaseDebuffDuration = DebuffDuration = dotDuration;
            BaseDebuffTicks = DebuffTicks = (int)(dotDuration / 3f);
            BaseDamageCoef = DamageCoef = damageCoef;
            BaseRange = Range = range;
            CritChance = 0f;
            BaseCritCoef = CritCoef = critCoef;
            GlobalCooldown = (float)Math.Max(1f, 1.5f / (1 + stats.SpellHaste));
            BaseCooldown = Cooldown = cooldown;
            SpellStatistics = new SpellStatistics();
            MagicSchool = magicSchool;
            BaseMana = baseMana;
            Ptr = ptr;
        }
        public Spell(string name, Stats stats, Character character, List<SpellData> SpellRankTable, int manaCost, float castTime, float critCoef, float dotDuration, float damageCoef, int range, float cooldown, Color col, float baseMana, bool ptr) :
            this(name, stats, character, SpellRankTable, manaCost, castTime, critCoef, dotDuration, damageCoef, range, cooldown, col, MagicSchool.Shadow, baseMana, ptr) { }

        public virtual void Calculate(Stats stats, Character character) { }

        public virtual void RecalcHaste(Stats stats, float addedHasteRating)
        {
            float newHaste = 1f + stats.SpellHaste;
            if (addedHasteRating > 0f)
            {
                newHaste /= (1f + StatConversion.GetSpellHasteFromRating(stats.HasteRating));
                newHaste *= (1f + StatConversion.GetSpellHasteFromRating(stats.HasteRating + addedHasteRating));
            }
            if (CastTime > 0f)
                CastTime = (float)Math.Max(1f, BaseCastTime / newHaste);
            GlobalCooldown = (float)Math.Max(1f, 1.5f / newHaste);
        }

        public float waitTime(float timer)
        {
            if (DebuffDuration > 0)
                return SpellStatistics.CooldownReset - CastTime - timer;
            return SpellStatistics.CooldownReset - timer;
        }

        public override string ToString() {
            if (DebuffDuration > 0f) {
                return String.Format("{0} *DpS: {1}\r\nDpCT: {2}\r\nDpM: {3}\r\nTick: {4}-{5}{6}\r\nDuration: {7}s\r\nCost: {8}\r\n{9}",
                          AvgDamage.ToString("0"),
                          DpS.ToString("0.00"),
                          DpCT.ToString("0.00"),
                          DpM.ToString("0.00"),
                          Math.Floor(AvgHit / DebuffDuration * 3).ToString("0"),
                          Math.Ceiling(AvgHit / DebuffDuration * 3).ToString("0"),
                          (CritCoef > 1f ? String.Format("\r\nTick Crit: {0}-{1}\r\nCrit Chance: {2}%", Math.Floor(AvgCrit / DebuffDuration * 3).ToString("0"), Math.Floor(AvgCrit / DebuffDuration * 3).ToString("0"), (CritChance * 100f).ToString("0.00")) : String.Empty),
                          DebuffDuration.ToString("0.00"),
                          ManaCost.ToString("0"),
                          Name);
            }
            return String.Format("{0} *DpS: {1}\r\nDpCT: {2}\r\nDpM: {3}\r\nHit: {4}-{5}, Avg {6}\r\nCrit: {7}-{8}, Avg {9}\r\nCrit Chance: {10}%\r\nCast: {11}\r\nCost: {12}\r\n{13}",
                AvgDamage.ToString("0"),
                DpS.ToString("0.00"),
                DpCT.ToString("0.00"),
                DpM.ToString("0.00"),
                MinDamage.ToString("0"), MaxDamage.ToString("0"), AvgHit.ToString("0"),
                MinCrit.ToString("0"), MaxCrit.ToString("0"), AvgCrit.ToString("0"),
                (CritChance * 100f).ToString("0.00"),
                CastTime.ToString("0.00"),
                ManaCost.ToString("0"),
                Name);
        }
    }

    public class ShadowWordPain : Spell {
        static readonly List<SpellData> SpellRankTable = new List<SpellData>() {
            new SpellData(1, 4, 30, 30),
            new SpellData(2, 10, 60, 60),
            new SpellData(3, 18, 120, 120),
            new SpellData(4, 26, 210, 210),
            new SpellData(5, 34, 330, 330),
            new SpellData(6, 42, 462, 462),
            new SpellData(7, 50, 606, 606),
            new SpellData(8, 58, 768, 768),
            new SpellData(9, 65, 906, 906),
            new SpellData(10, 70, 1116, 1116),
            new SpellData(11, 75, 1176, 1176),
            new SpellData(12, 80, 1380, 1380)
        };
        public ShadowWordPain(Stats stats, Character character, float baseMana, bool ptr)
            //: base("Shadow Word: Pain", stats, character, SpellRankTable, 22, 0, 0, 18f, 18f / 15f / 1.1f, 30, 0f, Color.FromArgb(255, 255, 0, 0))
            // Testing shows coefficient to be between 1.097 and 1.099
            : base("Shadow Word: Pain", stats, character, SpellRankTable, 22, 0, 0, 18f, 1.098f, 30, 0f, Color.FromArgb(255, 255, 0, 0), baseMana, ptr)
        {
            Calculate(stats, character);
        }
        public override void Calculate(Stats stats, Character character) {

            float modifiers
                = (1f + character.PriestTalents.TwinDisciplines * 0.01f
                    + character.PriestTalents.Darkness * 0.02f
                    + character.PriestTalents.ImprovedShadowWordPain * 0.03f
                    + character.PriestTalents.FocusedPower * 0.02f) // FIXME: ASSUMED
                * (1f + ((character.PriestTalents.ShadowWeaving > 0) ? 0.1f : 0.0f))
                * (1f + character.PriestTalents.Shadowform * 0.15f);

            MinDamage = MaxDamage = (BaseMinDamage +
                (stats.SpellPower + stats.SpellShadowDamageRating) * DamageCoef)
                * modifiers;

            DebuffDuration = BaseDebuffDuration + stats.SWPDurationIncrease;
            MinDamage *= DebuffDuration / BaseDebuffDuration;
            MaxDamage = MinDamage;
            DebuffTicks = BaseDebuffTicks + (int)(stats.SWPDurationIncrease / 3f);
          
            ManaCost = (int)Math.Floor((BaseManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1f - character.PriestTalents.ShadowFocus * 0.02f)
                * (1f - character.PriestTalents.MentalAgility * 0.1f / 3f));


            if (character.PriestTalents.Shadowform > 0) {
                CritChance = stats.SpellCrit
                    + character.PriestTalents.MindMelt * 0.03f
                    + stats.PriestDPS_T10_2pc;
                foreach (SpecialEffect se in stats.SpecialEffects())
                {
                    if (se.Stats.CritRating > 0 && se.Trigger == Trigger.Use)
                    // Special Handling for Nevermelting Ice Crystal (and similar?)
                        CritChance += StatConversion.GetSpellCritFromRating(se.Stats.CritRating);
                }
                CritCoef = (1.5f * (1f + stats.BonusSpellCritMultiplier) - 1f) * 2f + 1f;
                // Apparently made SPriests too good.
                //    DebuffDuration /= (1f + stats.SpellHaste);
            }

            Range = (int)Math.Round(BaseRange * (1 + character.PriestTalents.ShadowReach * 0.1f));
        }
    }
    public class VampiricTouch : Spell {
        static readonly List<SpellData> SpellRankTable = new List<SpellData>() {
            new SpellData( 1, 50, 450, 450),
            new SpellData( 2, 60, 600, 600),
            new SpellData( 3, 70, 650, 650),
            new SpellData( 4, 75, 735, 735),
            new SpellData( 5, 80, 935, 935)
        };
        public VampiricTouch(Stats stats, Character character, float baseMana, bool ptr)
        //: base("Vampiric Touch", stats, character, SpellRankTable, 16, 1.5f, 0, 15f, 15f / 15f * 1.9f, 30, 0f, Color.FromArgb(255, 0, 0, 255))
            // Testing shows VT to be between 1.965 and 1.97
            : base("Vampiric Touch", stats, character, SpellRankTable, 16, 1.5f, 0, 15f, 1.9685f, 30, 0f, Color.FromArgb(255, 0, 0, 255), baseMana, ptr)
        {
            Calculate(stats, character);
        }
        public override void Calculate(Stats stats, Character character) {
            if (character.PriestTalents.VampiricTouch == 0) {
                MinDamage = MaxDamage = 0;
                return;
            }

            float modifiers
                = (1f + character.PriestTalents.Darkness * 0.02f)
//                    + character.PriestTalents.FocusedPower * 0.02f) // FIXME: Uh, can you get VT and FP?
                * (1f + ((character.PriestTalents.ShadowWeaving > 0) ? 0.1f : 0.0f))
                * (1f + character.PriestTalents.Shadowform * 0.15f);

            MinDamage = (BaseMinDamage + (stats.SpellPower + stats.SpellShadowDamageRating) * DamageCoef)
                * modifiers;

            DebuffDuration = BaseDebuffDuration + stats.PriestDPS_T9_2pc;
            DebuffTicks = BaseDebuffTicks + (int)(stats.PriestDPS_T9_2pc / 3f);

            MinDamage *= DebuffDuration / BaseDebuffDuration;
            MaxDamage = MinDamage;

            ManaCost = (int)Math.Floor((BaseManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1f - character.PriestTalents.ShadowFocus * 0.02f));

            CastTime = (float)Math.Max(1f, BaseCastTime / (1f + stats.SpellHaste));

            if (character.PriestTalents.Shadowform > 0) {
                CritChance = stats.SpellCrit
                    + character.PriestTalents.MindMelt * 0.03f
                    + stats.PriestDPS_T10_2pc;
                CritCoef = (1.5f * (1f + stats.BonusSpellCritMultiplier) - 1f) * 2f + 1f;
                DebuffDuration /= (1f + stats.SpellHaste);
            }

            Range = (int)Math.Round(BaseRange * (1f + character.PriestTalents.ShadowReach * 0.1f));
        }
    }
    public class MindBlast : Spell {
        static readonly List<SpellData> SpellRankTable = new List<SpellData>() {
            new SpellData( 1, 10,  39,  43),
            new SpellData( 2, 16,  72,  78),
            new SpellData( 3, 22, 116, 120),
            new SpellData( 4, 28, 167, 177),
            new SpellData( 5, 34, 217, 231),
            new SpellData( 6, 40, 279, 297),
            new SpellData( 7, 46, 346, 366),
            new SpellData( 8, 52, 425, 449),
            new SpellData( 9, 58, 503, 531),
            new SpellData(10, 63, 557, 587),
            new SpellData(11, 69, 711, 752),
            new SpellData(12, 74, 837, 883),
            new SpellData(13, 79, 992, 1048)
        };
        public MindBlast(Stats stats, Character character, float baseMana, bool ptr)
			: base("Mind Blast", stats, character, SpellRankTable, 17, 1.5f, 1.5f, 0, 1.5f / 3.5f, 30, 8f, Color.FromArgb(255, 255, 192, 0), baseMana, ptr)
        {
            Calculate(stats, character);
        }
        public override void Calculate(Stats stats, Character character) {
            PriestTalents talents = character.PriestTalents;

            float modifiers
                = (1f + talents.Darkness * 0.02f
                    + talents.FocusedPower * 0.02f) // FIXME: Check they stack like this
                    * (1f + ((talents.ShadowWeaving > 0) ? 0.1f : 0f))
                    * (1f + stats.BonusMindBlastMultiplier)
                    * (1f + talents.TwistedFaith * 0.02f)
                    * (1f + talents.Shadowform * 0.15f);
                    
                
            DamageCoef = BaseDamageCoef * (1f + talents.Misery * 0.05f);

            MinDamage = (BaseMinDamage + (stats.SpellPower + stats.SpellShadowDamageRating) * DamageCoef)
                * modifiers;

            MaxDamage = (BaseMaxDamage + (stats.SpellPower + stats.SpellShadowDamageRating) * DamageCoef)
                * modifiers;

            CastTime = (float)Math.Max(1f, BaseCastTime / (1 + stats.SpellHaste));

            CritCoef = (BaseCritCoef * (1f + stats.BonusSpellCritMultiplier) - 1f) * (1f + character.PriestTalents.ShadowPower * 0.2f) + 1f;

            CritChance = stats.SpellCrit + character.PriestTalents.MindMelt * 0.02f;

            Cooldown = BaseCooldown - talents.ImprovedMindBlast * 0.5f;

            ManaCost = (int)Math.Floor((BaseManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1f - character.PriestTalents.ShadowFocus * 0.02f)
                * (1f - character.PriestTalents.FocusedMind * 0.05f)
                * (1f - stats.MindBlastCostReduction));

            Range = (int)Math.Round(BaseRange * (1 + character.PriestTalents.ShadowReach * 0.1f));
        }
    }
    public class ShadowWordDeath : Spell {
        static readonly List<SpellData> SpellRankTable = new List<SpellData>() {
            new SpellData( 1, 62, 450, 522),
            new SpellData( 2, 70, 572, 664),
            new SpellData( 3, 75, 639, 741),
            new SpellData( 4, 80, 750, 870)
        };
        public ShadowWordDeath(Stats stats, Character character, float baseMana, bool ptr)
            : base("Shadow Word: Death", stats, character, SpellRankTable, 12, 0, 1.5f, 0, 1.5f / 3.5f, 30, 12f, Color.FromArgb(255, 255, 215, 0), baseMana, ptr)
        {
            Calculate(stats, character);
        }
        public override void Calculate(Stats stats, Character character) {
            float modifiers
                = (1f + character.PriestTalents.TwinDisciplines * 0.01f
                    + character.PriestTalents.Darkness * 0.02f
                    + character.PriestTalents.FocusedPower * 0.02f)     // FIXME: Doublecheck this
                * (1f + ((character.PriestTalents.ShadowWeaving > 0) ? 0.1f : 0.0f))
                * (1f + character.PriestTalents.Shadowform * 0.15f);

            MinDamage = (BaseMinDamage + (stats.SpellPower + stats.SpellShadowDamageRating) * DamageCoef)
                * modifiers;

            MaxDamage = (BaseMaxDamage + (stats.SpellPower + stats.SpellShadowDamageRating) * DamageCoef)
                * modifiers;

            CritChance = stats.SpellCrit + stats.ShadowWordDeathCritIncrease;

            CritCoef = (BaseCritCoef * (1f + stats.BonusSpellCritMultiplier) - 1f) * (1f + character.PriestTalents.ShadowPower * 0.2f) + 1f;

            ManaCost = (int)Math.Floor((BaseManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1f - character.PriestTalents.ShadowFocus * 0.02f)
                * (1f - character.PriestTalents.MentalAgility * 0.1f / 3f));

            Range = (int)Math.Round(BaseRange * (1 + character.PriestTalents.ShadowReach * 0.1f));
        }
    }
    public class MindFlay : Spell {
        static readonly List<SpellData> SpellRankTable = new List<SpellData>() {
            new SpellData( 1, 20,  45,  45),
            new SpellData( 2, 28, 108, 108),
            new SpellData( 3, 36, 159, 159),
            new SpellData( 4, 44, 222, 222),
            new SpellData( 5, 52, 282, 282),
            new SpellData( 6, 60, 363, 363),
            new SpellData( 7, 68, 450, 450),
            new SpellData( 8, 74, 492, 492),
            new SpellData( 9, 80, 588, 588)
        };
        public MindFlay(Stats stats, Character character, float baseMana, bool ptr)
            //: base("Mind Flay", stats, character, SpellRankTable, 9, 3f, 1.5f, 0, 3f / 3.5f, 20, 0f, Color.FromArgb(255, 173, 216, 230))
            // Testing shows MF to be about 0.771
            : base("Mind Flay", stats, character, SpellRankTable, 9, 3f, 1.5f, 0, 0.771f, 30, 0f, Color.FromArgb(255, 173, 216, 230), baseMana, ptr)
        {
            Calculate(stats, character);
            BaseDebuffTicks = DebuffTicks = (int)(BaseDebuffDuration);
        }

        public override void RecalcHaste(Stats stats, float addedHasteRating)
        {
            float newHaste = 1f + stats.SpellHaste;
            if (addedHasteRating > 0f)
            {
                newHaste /= (1f + StatConversion.GetSpellHasteFromRating(stats.HasteRating));
                newHaste *= (1f + StatConversion.GetSpellHasteFromRating(stats.HasteRating + addedHasteRating));
            }
            if (CastTime > 0f)
                CastTime = (float)Math.Max(1f, (BaseCastTime - stats.PriestDPS_T10_4pc) / newHaste);
            GlobalCooldown = (float)Math.Max(1f, 1.5f / newHaste);
        }
        
        public override void Calculate(Stats stats, Character character) {
            PriestTalents talents = character.PriestTalents;

            if (talents.MindFlay == 0) {
                MinDamage = MaxDamage = 0;
                return;
            }

            float modifiers
                = (1f + talents.TwinDisciplines * 0.01f
                    + talents.Darkness * 0.02f
                    + talents.FocusedPower * 0.02f)     // FIXME: Doublecheck
                * (1f + ((talents.ShadowWeaving > 0) ? 0.1f : 0.0f))
                * (1f + character.PriestTalents.TwistedFaith * 0.02f
                    + (character.PriestTalents.GlyphofMindFlay ? 0.1f : 0.0f))
                * (1f + talents.Shadowform * 0.15f);

            // Coefficient penalty for snare effect.
            //DamageCoef = BaseDamageCoef * 0.9f * (1f + talents.Misery * 0.05f);
            DamageCoef = BaseDamageCoef * (1f + talents.Misery * 0.05f);

            MinDamage = MaxDamage = (BaseMinDamage + (stats.SpellPower + stats.SpellShadowDamageRating) * DamageCoef)
                * modifiers;

            ManaCost = (int)Math.Floor((BaseManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                //* (1f - talents.MentalAgility * 0.1f / 3f)
                * (1f - talents.ShadowFocus * 0.02f)
                * (1f - talents.FocusedMind * 0.05f));

            CritChance = stats.SpellCrit + talents.MindMelt * 0.02f + stats.PriestDPS_T9_4pc;

            CritCoef = (BaseCritCoef * (1f + stats.BonusSpellCritMultiplier) - 1f) * (1f + talents.ShadowPower * 0.2f) + 1f;

            CastTime = (float)Math.Max(1f, (BaseCastTime - stats.PriestDPS_T10_4pc) / (1 + stats.SpellHaste));
            Range = (int)Math.Round(BaseRange * (1f + talents.ShadowReach * 0.1f));
        }
        public override string ToString() {
            return String.Format("{0} *DpS: {1}\r\nDpCT: {2}\r\nDpM: {3}\r\nTick Hit: {4}-{5}\r\nTick Crit: {6}-{7}\r\nCrit Chance: {8}%\r\nCast: {9}\r\nCost: {10}\r\n{11}",
                          AvgDamage.ToString("0"),
                          DpS.ToString("0.00"),
                          DpCT.ToString("0.00"),
                          DpM.ToString("0.00"),
                          Math.Floor(AvgHit / 3).ToString("0"), Math.Ceiling(AvgHit / 3).ToString("0"),
                          Math.Floor(AvgCrit / 3).ToString("0"), Math.Ceiling(AvgCrit / 3).ToString("0"),
                          (CritChance * 100f).ToString("0.00"),
                          CastTime.ToString("0.00"),
                          ManaCost.ToString("0"),
                          Name);
        }
    }
    public class PowerWordShield : Spell {
        static readonly List<SpellData> SpellRankTable = new List<SpellData>() {
            new SpellData( 1,  6,   44,   44),
            new SpellData( 2, 12,   88,   88),
            new SpellData( 3, 18,  158,  158),
            new SpellData( 4, 24,  234,  234),
            new SpellData( 5, 30,  301,  301),
            new SpellData( 6, 36,  381,  381),
            new SpellData( 7, 42,  484,  484),
            new SpellData( 8, 48,  605,  605),
            new SpellData( 9, 54,  763,  763),
            new SpellData(10, 60,  942,  942),
            new SpellData(11, 65, 1125, 1125),
            new SpellData(12, 70, 1265, 1265),
            new SpellData(13, 75, 1920, 1920),
            new SpellData(14, 80, 2230, 2230)
        };
        public PowerWordShield(Stats stats, Character character, float baseMana, bool ptr)
            : base("Power Word: Shield", stats, character, SpellRankTable, 23, 0, 0, 0, 1.5f / 3.5f, 30, 4f, Color.FromArgb(255, 165, 42, 42), baseMana, ptr)
        {
            Calculate(stats, character);
        }
        public override void Calculate(Stats stats, Character character) {
            MinDamage = MaxDamage = (BaseMinDamage * (1 + character.PriestTalents.ImprovedPowerWordShield * 0.05f)
                + stats.SpellPower * 1.88f * DamageCoef
                + stats.SpellPower * character.PriestTalents.BorrowedTime * 0.04f)
                * (1f + character.PriestTalents.FocusedPower * 0.02f)
                * (1f + character.PriestTalents.TwinDisciplines * 0.01f)
                * (1f + character.PriestTalents.ImprovedPowerWordShield * 0.05f);

            ManaCost = (int)Math.Floor((BaseManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1f - character.PriestTalents.MentalAgility * 0.1f / 3f
                      - character.PriestTalents.SoulWarding * 0.15f));
        }
        public override string ToString() {
            return String.Format("{0} *Cost: {1}\r\n{2}",
                MinDamage.ToString("0"),
                ManaCost.ToString("0"),
                Name);
        }
    }
    public class VampiricEmbrace : Spell {
        static readonly List<SpellData> SpellRankTable = new List<SpellData>() {
            new SpellData(1, 30, 0, 0)
        };
        public float HealthConvertionCoef { get; protected set; }
        public VampiricEmbrace(Stats stats, Character character, float baseMana, bool ptr)
            : base("Vampiric Embrace", stats, character, SpellRankTable, 2, 0, 0, 60f, 0, 30, 10f, Color.FromArgb(255, 0, 128, 0), baseMana, ptr)
        {
            HealthConvertionCoef = 0.15f;
            Calculate(stats, character);
        }
        public override void Calculate(Stats stats, Character character) {
            if (character.PriestTalents.VampiricEmbrace == 0) {
                HealthConvertionCoef = 0;
                return;
            }
            HealthConvertionCoef *= (1 + character.PriestTalents.ImprovedVampiricEmbrace / 3f);
            ManaCost = (int)Math.Floor((BaseManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1f - character.PriestTalents.ShadowFocus * 0.02f)
                * (1f - character.PriestTalents.MentalAgility * 0.1f / 3f));
        }
        public override string ToString() {
            return String.Format("{0} *Cost: {1}\r\n{2}",
                (HealthConvertionCoef * 100).ToString("0") + "%",
                ManaCost.ToString("0"),
                Name);
        }
    }
    public class DevouringPlague : Spell
    {
        static readonly List<SpellData> SpellRankTable = new List<SpellData>() {
            new SpellData(1, 20, 152, 152),
            new SpellData(2, 28, 272, 272),
            new SpellData(3, 36, 400, 400),
            new SpellData(4, 44, 544, 544),
            new SpellData(5, 52, 712, 712),
            new SpellData(6, 60, 904, 904),
            new SpellData(7, 68, 1088, 1088),
            new SpellData(8, 73, 1144, 1144),
            new SpellData(9, 79, 1376, 1376)
        };

        public ImprovedDevouringPlague ImprovedDP;

        public DevouringPlague(Stats stats, Character character, float baseMana, bool ptr)
            //: base("Devouring Plague", stats, character, SpellRankTable, 25, 0, 0, 24, 24f / 15f * 0.925f, 30, 0, Color.FromArgb(255, 128, 0, 128))
            // Testing shows coefficient to be between 1.479 and 1.480
            : base("Devouring Plague", stats, character, SpellRankTable, 25, 0, 0, 24, 1.48f, 30, 0, Color.FromArgb(255, 128, 0, 128), baseMana, ptr)
        {
            if (character.PriestTalents.ImprovedDevouringPlague > 0)
                ImprovedDP = new ImprovedDevouringPlague(stats, character, SpellRankTable, baseMana, ptr);

            Calculate(stats, character);
        }
        public override void Calculate(Stats stats, Character character)
        {
            if (ImprovedDP != null)
                ImprovedDP.Calculate(stats, character);

            float modifiers
                = (1f + character.PriestTalents.TwinDisciplines * 0.01f
                    + character.PriestTalents.Darkness * 0.02f
                    + character.PriestTalents.FocusedPower * 0.02f      // FIXME: Doublecheck
                    + character.PriestTalents.ImprovedDevouringPlague * 0.05f
                    + stats.DevouringPlagueBonusDamage)
                * (1f + ((character.PriestTalents.ShadowWeaving > 0) ? 0.1f : 0f))
                * (1f + stats.BonusDiseaseDamageMultiplier)
                * (1f + character.PriestTalents.Shadowform * 0.15f);

            MinDamage = MaxDamage = (BaseMinDamage +
                (stats.SpellPower + stats.SpellShadowDamageRating) * DamageCoef)
                * modifiers;

            DebuffDuration = BaseDebuffDuration;
            DebuffTicks = BaseDebuffTicks;

            ManaCost = (int)Math.Floor((BaseManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1f - character.PriestTalents.ShadowFocus * 0.02f)
                * (1f - character.PriestTalents.MentalAgility * 0.1f / 3f));

            if (character.PriestTalents.Shadowform > 0)
            {
                CritChance = stats.SpellCrit
                    + character.PriestTalents.MindMelt * 0.03f
                    + stats.PriestDPS_T10_2pc;
                CritCoef = (1.5f * (1f + stats.BonusSpellCritMultiplier) - 1f) * 2f + 1f;
                DebuffDuration /= (1f + stats.SpellHaste);
            }

            Range = (int)Math.Round(BaseRange * (1 + character.PriestTalents.ShadowReach * 0.1f));
        }
        public override string ToString()
        {
            return String.Format("{0} *DpS: {1}\r\nDpCT: {2}\r\nDpM: {3}\r\nTick: {4}-{5}{6}\r\nDuration: {7}s\r\nCost: {8}\r\n{9}",
                AvgDamage.ToString("0"),
                DpS.ToString("0.00"),
                DpCT.ToString("0.00"),
                DpM.ToString("0.00"),
                Math.Floor(AvgHit / DebuffDuration * 3).ToString("0"),
                Math.Ceiling(AvgHit / DebuffDuration * 3).ToString("0"),
                (CritCoef > 1f ? String.Format("\r\nTick Crit: {0}-{1}\r\nCrit Chance: {2}%", Math.Floor(AvgCrit / DebuffDuration * 3).ToString("0"), Math.Floor(AvgCrit / DebuffDuration * 3).ToString("0"), (CritChance * 100f).ToString("0.00")) : String.Empty),
                DebuffDuration.ToString("0.00"),
                ManaCost.ToString("0"),
                Name);
        }
    }
    public class ImprovedDevouringPlague : Spell
    {
        public ImprovedDevouringPlague(Stats stats, Character character, List<SpellData> SpellRankTable, float baseMana, bool ptr)
            : base("Imp. Devouring Plague", stats, character, SpellRankTable, 25, 0, 0, 24, 1.48f, 30, 24, Color.FromArgb(255, 128, 0, 128), baseMana, ptr)
        {
            Calculate(stats, character);
        }
        public override void Calculate(Stats stats, Character character)
        {

            float modifiers
                = (1f + character.PriestTalents.TwinDisciplines * 0.01f
                    + character.PriestTalents.Darkness * 0.02f
                    + character.PriestTalents.FocusedPower * 0.02f      // FIXME: Doublecheck
                    + character.PriestTalents.ImprovedDevouringPlague * 0.1f)
                * (1f + ((character.PriestTalents.ShadowWeaving > 0) ? 0.05f : 0f))
                * (1f + stats.BonusDiseaseDamageMultiplier)
                * (1f + character.PriestTalents.Shadowform * 0.15f);

            MinDamage = MaxDamage = (BaseMinDamage +
                (stats.SpellPower + stats.SpellShadowDamageRating) * DamageCoef)
                * (character.PriestTalents.ImprovedDevouringPlague * 0.1f)
                * modifiers
                * (1f + stats.DevouringPlagueBonusDamage);

            CritChance = stats.SpellCrit;
            CritCoef = 1.5f * (1f + stats.BonusSpellCritMultiplier) ;

            ManaCost = 0;
            DebuffDuration = 0f;

            Range = 0;
        }
        public override string ToString()
        {
            return String.Format("{0} *Hit: {1}\r\nCrit: {2}\r\nCritChance: {3}%",
                AvgDamage.ToString("0"),
                AvgHit.ToString("0"),
                AvgCrit.ToString("0"),
                (CritChance * 100f).ToString("0.00"),
                Name);
        }
    }
    public class TimbalProc : Spell
    {
        static readonly List<SpellData> SpellRankTable = new List<SpellData>() {
            new SpellData(1, 0, 285, 475)
        };
        public TimbalProc(Stats stats, Character character, float baseMana, bool ptr)
            : base("TimbalProc", stats, character, SpellRankTable, 0, 0f, 1.5f, 0, 0f, 0, 0f, Color.FromArgb(255, 0, 0, 0), baseMana, ptr)
        {
            Calculate(stats, character);
        }
        public override void Calculate(Stats stats, Character character) {
            MinDamage = BaseMinDamage
                * (1f + character.PriestTalents.FocusedPower * 0.02f)
                * (1f + character.PriestTalents.Darkness * 0.02f)
                * (1f + ((character.PriestTalents.ShadowWeaving > 0) ? 0.1f : 0f))
                * (1f + character.PriestTalents.Shadowform * 0.15f);

            MaxDamage = BaseMaxDamage
                * (1f + character.PriestTalents.FocusedPower * 0.02f)
                * (1f + character.PriestTalents.Darkness * 0.02f)
                * (1f + ((character.PriestTalents.ShadowWeaving > 0) ? 0.1f : 0f))
                * (1f + character.PriestTalents.Shadowform * 0.15f);

            //CritCoef += character.PriestTalents.ShadowPower * 0.1f;
            CritChance = stats.SpellCrit;

            Range = (int)Math.Round(BaseRange * (1 + character.PriestTalents.ShadowReach * 0.1f));
        }
    }
    public class ExtractProc : Spell {
        static readonly List<SpellData> SpellRankTable = new List<SpellData>() {
            new SpellData(1, 0, 788, 1312)
        };
        public ExtractProc(Stats stats, Character character, float baseMana, bool ptr)
            : base("ExtractProc", stats, character, SpellRankTable, 0, 0f, 1.5f, 0, 0f, 0, 0f, Color.FromArgb(255, 0, 0, 0), baseMana, ptr)
        {
            Calculate(stats, character);
        }
        public override void Calculate(Stats stats, Character character) {
            MinDamage = BaseMinDamage
                * (1f + character.PriestTalents.FocusedPower * 0.02f)
                * (1f + character.PriestTalents.Darkness * 0.02f)
                * (1f + ((character.PriestTalents.ShadowWeaving > 0) ? 0.1f : 0.0f))
                * (1f + character.PriestTalents.Shadowform * 0.15f);

            MaxDamage = BaseMaxDamage
                * (1f + character.PriestTalents.FocusedPower * 0.02f)
                * (1f + character.PriestTalents.Darkness * 0.02f)
                * (1f + ((character.PriestTalents.ShadowWeaving > 0) ? 0.1f : 0.0f))
                * (1f + character.PriestTalents.Shadowform * 0.15f);

            //CritCoef += character.PriestTalents.ShadowPower * 0.1f;
            CritChance = stats.SpellCrit;

            Range = (int)Math.Round(BaseRange * (1 + character.PriestTalents.ShadowReach * 0.1f));
        }
    }
    public class PendulumProc : Spell {
        static readonly List<SpellData> SpellRankTable = new List<SpellData>() {
            new SpellData(1, 0, 1168, 1752)
        };
        public PendulumProc(Stats stats, Character character, float baseMana, bool ptr)
            : base("PendulumProc", stats, character, SpellRankTable, 0, 0f, 1.5f, 0, 0f, 0, 0f, Color.FromArgb(255, 0, 0, 0), baseMana, ptr)
        {
            Calculate(stats, character);
        }
        public override void Calculate(Stats stats, Character character) {
            MinDamage = BaseMinDamage
                * (1f + character.PriestTalents.FocusedPower * 0.02f)
                * (1f + character.PriestTalents.Darkness * 0.02f)
                * (1f + ((character.PriestTalents.ShadowWeaving > 0) ? 0.1f : 0.0f))
                * (1f + character.PriestTalents.Shadowform * 0.15f);

            MaxDamage = BaseMaxDamage
                * (1f + character.PriestTalents.FocusedPower * 0.02f)
                * (1f + character.PriestTalents.Darkness * 0.02f)
                * (1f + ((character.PriestTalents.ShadowWeaving > 0) ? 0.1f : 0.0f))
                * (1f + character.PriestTalents.Shadowform * 0.15f);

            //CritCoef += character.PriestTalents.ShadowPower * 0.1f;
            CritChance = stats.SpellCrit;

            Range = (int)Math.Round(BaseRange * (1f + character.PriestTalents.ShadowReach * 0.1f));
        }
    }
    public class Smite : Spell {
        static readonly List<SpellData> SpellRankTable = new List<SpellData>() {
            new SpellData( 1,  1,  13,  17),
            new SpellData( 2,  6,  25,  31),
            new SpellData( 3, 14,  54,  62),
            new SpellData( 4, 22,  91, 104),
            new SpellData( 5, 30, 150, 170),
            new SpellData( 6, 38, 212, 240),
            new SpellData( 7, 46, 287, 323),
            new SpellData( 8, 54, 371, 415),
            new SpellData( 9, 61, 405, 453),
            new SpellData(10, 69, 549, 616),
            new SpellData(11, 74, 604, 676),
            new SpellData(12, 79, 707, 793)
        };
        public Smite(Stats stats, Character character, float baseMana, bool ptr)
            : base("Smite", stats, character, SpellRankTable, 15, 2.5f, 1.5f, 0, 2.5f / 3.5f, 30, 0f, Color.FromArgb(255, 255, 255, 0), baseMana, ptr)
        {
            Calculate(stats, character);
        }
        public override void Calculate(Stats stats, Character character) {
            MinDamage = (BaseMinDamage + (stats.SpellPower + stats.SpellShadowDamageRating) * DamageCoef)
                * (1f + character.PriestTalents.FocusedPower * 0.02f)
                * (1f + character.PriestTalents.SearingLight * 0.05f);

            MaxDamage = (BaseMaxDamage + (stats.SpellPower + stats.SpellShadowDamageRating) * DamageCoef)
                * (1f + character.PriestTalents.FocusedPower * 0.02f)
                * (1f + character.PriestTalents.SearingLight * 0.05f);

            CastTime = (float)Math.Max(1.0f, (BaseCastTime - character.PriestTalents.DivineFury * 0.1f) / (1 + stats.SpellHaste));

            CritCoef = BaseCritCoef * (1f + stats.BonusSpellCritMultiplier);

            CritChance = stats.SpellCrit + character.PriestTalents.HolySpecialization * 0.01f;

            ManaCost = (int)Math.Floor((BaseManaCost / 100f * BaseMana - stats.SpellsManaReduction));

            Range = (int)Math.Round(BaseRange * (1f + character.PriestTalents.HolyReach * 0.1f));
        }
    }
    public class HolyFire : Spell {
        class SpellDataDot : SpellData {
            public int DotDamage { get; protected set; }
            public SpellDataDot(int rank, int level, int minDamage, int maxDamage, int dotDamage)
                : base(rank, level, minDamage, maxDamage)
            {
                DotDamage = dotDamage;
            }
        }
        static readonly List<SpellData> SpellRankTable = new List<SpellData>() {
            new SpellDataDot( 1, 20,  102,  128,  21),
            new SpellDataDot( 2, 24,  137,  173,  28),
            new SpellDataDot( 3, 30,  200,  252,  42),
            new SpellDataDot( 4, 36,  267,  339,  56),
            new SpellDataDot( 5, 42,  348,  440,  70),
            new SpellDataDot( 6, 48,  430,  546,  91),
            new SpellDataDot( 7, 54,  529,  671, 112),
            new SpellDataDot( 8, 60,  639,  811, 126),
            new SpellDataDot( 9, 66,  705,  895, 147),
            new SpellDataDot(10, 72,  732,  928, 287),
            new SpellDataDot(11, 78,  890, 1130, 350)
        };
        public float BaseDotDamage { get; protected set; }
        public float DotDamage { get; protected set; }
        public override float AvgDamage { get { return AvgHit * (1f - CritChance) + AvgCrit * CritChance + DotDamage; } }
        public override float DpCT { get { return AvgDamage / CastTime; } }
        public override float DpS { get { return AvgDamage / CastTime; } }
        public HolyFire(Stats stats, Character character, float baseMana, bool ptr)
            : base("Holy Fire", stats, character, SpellRankTable, 11, 2.0f, 1.5f, 7f, 2f / 3.5f, 30, 10f, Color.FromArgb(255, 255, 215, 0), baseMana, ptr)
        {
            Calculate(stats, character);
            foreach (SpellData sd in SpellRankTable) {
                if (character.Level > sd.Level) {
                    BaseDotDamage = DotDamage = ((SpellDataDot)sd).DotDamage;
                }
            }

        }
        public override void Calculate(Stats stats, Character character) {
            MinDamage = (BaseMinDamage + (stats.SpellPower + stats.SpellShadowDamageRating) * DamageCoef)
                * (1f + character.PriestTalents.FocusedPower * 0.02f)
                * (1f + character.PriestTalents.SearingLight * 0.05f);

            MaxDamage = (BaseMaxDamage + (stats.SpellPower + stats.SpellShadowDamageRating) * DamageCoef)
                * (1f + character.PriestTalents.FocusedPower * 0.02f)
                * (1f + character.PriestTalents.SearingLight * 0.05f);


            DotDamage = (BaseDotDamage + stats.SpellPower * 1f / 6f)
                * (1f + character.PriestTalents.SearingLight * 0.05f);

            CastTime = (float)Math.Max(1f, (BaseCastTime - character.PriestTalents.DivineFury * 0.1f) / (1f + stats.SpellHaste));

            CritCoef = BaseCritCoef * (1f + stats.BonusSpellCritMultiplier);

            CritChance = stats.SpellCrit + character.PriestTalents.HolySpecialization * 0.01f;

            ManaCost = (int)Math.Floor((BaseManaCost / 100f * BaseMana - stats.SpellsManaReduction));

            Range = (int)Math.Round(BaseRange * (1 + character.PriestTalents.HolyReach * 0.1f));
        }
        public override string ToString() {
            return String.Format("{0} *DpS: {1}\r\nDpCT: {2}\r\nDpM: {3}\r\nHit: {4}-{5}, Avg {6}\r\nCrit: {7}-{8}, Avg {9}\r\nTick: {10}-{11}\r\nCrit Chance: {12}%\r\nCast: {13}\r\nCost: {14}\r\n{15}",
                AvgDamage.ToString("0"),
                DpS.ToString("0.00"),
                DpCT.ToString("0.00"),
                DpM.ToString("0.00"),
                MinDamage.ToString("0"), MaxDamage.ToString("0"), AvgHit.ToString("0"),
                MinCrit.ToString("0"), MaxCrit.ToString("0"), AvgCrit.ToString("0"),
                Math.Floor(DotDamage / 7f).ToString("0"), Math.Ceiling(DotDamage / 7f).ToString("0"),
                (CritChance * 100f).ToString("0.00"),
                CastTime.ToString("0.00"),
                ManaCost.ToString("0"),
                Name);
        }
    }
    public class Penance : Spell {
        static readonly List<SpellData> SpellRankTable = new List<SpellData>() {
            new SpellData(1, 60, 552, 552),
            new SpellData(2, 70, 672, 672),
            new SpellData(3, 75, 768, 768),
            new SpellData(4, 80, 864, 864)
        };
        public Penance(Stats stats, Character character, float baseMana, bool ptr)
            : base("Penance", stats, character, SpellRankTable, 16, 2f, 1.5f, 0, 2.4f / 3.5f, 30, 10f, Color.FromArgb(255, 255, 165, 0), baseMana, ptr)
        {
            Calculate(stats, character);
        }
        public override void Calculate(Stats stats, Character character) {
            if (character.PriestTalents.Penance == 0) {
                MinDamage = MaxDamage = 0;
                return;
            }

            MinDamage = MaxDamage = (BaseMinDamage + (stats.SpellPower + stats.SpellShadowDamageRating) * DamageCoef)
                * (1f + character.PriestTalents.TwinDisciplines * 0.01f)
                * (1f + character.PriestTalents.FocusedPower * 0.02f)
                * (1f + character.PriestTalents.SearingLight * 0.05f);


            CastTime = (float)Math.Max(1.0f, BaseCastTime / (1 + stats.SpellHaste));

            CritCoef = BaseCritCoef * (1f + stats.BonusSpellCritMultiplier);

            CritChance = stats.SpellCrit + character.PriestTalents.HolySpecialization * 0.01f;

            Cooldown = (BaseCooldown - (character.PriestTalents.GlyphofPenance ? 2 : 0)) * (1f - character.PriestTalents.Aspiration * 0.1f);

            ManaCost = (int)Math.Floor((BaseManaCost / 100f * BaseMana - stats.SpellsManaReduction)
                * (1f - character.PriestTalents.ImprovedHealing * 0.05f));

            Range = (int)Math.Round(BaseRange * (1 + character.PriestTalents.HolyReach * 0.1f));
        }
        public override string ToString() {
            return String.Format("{0} *DpS: {1}\r\nDpCT: {2}\r\nDpM: {3}\r\nTick Hit: {4}-{5}, Total {6}\r\nTick Crit: {7}-{8}, Total {9}\r\nCrit Chance: {10}%\r\nCast: {11}\r\nCost: {12}\r\n{13}",
                AvgDamage.ToString("0"),
                DpS.ToString("0.00"),
                DpCT.ToString("0.00"),
                DpM.ToString("0.00"),
                Math.Floor(MinDamage/3).ToString("0"), Math.Ceiling(MinDamage/3).ToString("0"), AvgHit.ToString("0"),
                Math.Floor(MinCrit/3).ToString("0"), Math.Ceiling(MinCrit/3).ToString("0"), AvgCrit.ToString("0"),
                (CritChance * 100f).ToString("0.00"),
                CastTime.ToString("0.00"),
                ManaCost.ToString("0"),
                Name);
        }
    }
    public class Shadowfiend : Spell {
        static readonly List<SpellData> SpellRankTable = new List<SpellData>() {
            new SpellData(1, 66, 181, 220),
        };
        float Swings = 10f;
        public override float DpCT { get { return AvgDamage * Swings / GlobalCooldown; } }
        public override float DpS { get { return AvgDamage * Swings / DebuffDuration; } }
        public Shadowfiend(Stats stats, Character character, float baseMana, bool ptr)
            : base("Shadowfiend", stats, character, SpellRankTable, 0, 0f, 2.0f, 10f, 1.5f / 3.5f * 0.9f, 30, (5f - character.PriestTalents.VeiledShadows * 1f) * 60f, Color.FromArgb(255, 0, 0, 0), baseMana, ptr)
        {   // Coefficient is Spell Power * 15 seconds over 3.5 seconds, with a 10% penalty.
            Calculate(stats, character);
        }
        public override void Calculate(Stats stats, Character character) {
            // ShadowCrawl has a 5 second duration every 6 seconds, meaning you can keep it up 13 seconds of the 15 seconds.
            float ShadowCrawl = 0.15f * 13f / 15f;

            MinDamage = (BaseMinDamage + (stats.SpellPower + stats.SpellShadowDamageRating) * DamageCoef) * (1 + ShadowCrawl);

            MaxDamage = (BaseMaxDamage + (stats.SpellPower + stats.SpellShadowDamageRating) * DamageCoef) * (1 + ShadowCrawl);

            CritChance = 0.05f;

            ManaCost = 0;

            Range = (int)Math.Round(BaseRange * (1 + character.PriestTalents.ShadowReach * 0.1f));
        }
        public override string ToString() {
            return String.Format("{0} *DpS: {1}\r\nDpCT: {2}\r\nSwing: {3}\r\nSwing Crit: {4}\r\nCrit Chance: {5}%\r\n{6}",
                (AvgDamage * Swings).ToString("0"),
                DpS.ToString("0.00"),
                DpCT.ToString("0.00"),
                AvgHit.ToString("0.00"),
                AvgCrit.ToString("0.00"),
                (CritChance * 100f).ToString("0.00"),
                Name);
        }
    }
}
