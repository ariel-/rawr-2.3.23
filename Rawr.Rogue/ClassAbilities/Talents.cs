using System.Collections.Generic;

namespace Rawr.Rogue.ClassAbilities
{
    public abstract class Talents
    {
        private Talents(TalentDelegate talent)
        {
            _talent = talent;
        }

        private readonly TalentDelegate _talent;
        protected static RogueTalents _talents = new RogueTalents();
        protected delegate int TalentDelegate();
        protected delegate float CalculationDelegate(int talentPoints);

        //---------------------------------------------------------------------
        //Assassination Talents
        //---------------------------------------------------------------------
        public static readonly Talents ImprovedEviscerate = new TalentBonusPulledFromList(() => _talents.ImprovedEviscerate, .07f, .14f, .20f);
        //public static readonly Talents Malice = new TalentBonusPulledFromList(() => _talents.Malice, 0.01f, 0.02f, 0.03f, 0.04f, 0.05f);
        public static readonly Talents Ruthlessness = new TalentBonusPulledFromList(() => _talents.Ruthlessness, .2f, .4f, .6f);
        public static readonly Talents BloodSpatter = new TalentBonusPulledFromList(() => _talents.BloodSpatter, 0.15f, 0.30f);

        public class PuncturingWounds
        {
            public static readonly Talents Mutilate = new TalentBonusPulledFromList(() => _talents.PuncturingWounds, 0.05f, 0.10f, 0.15f);
            public static readonly Talents Backstab = new TalentBonusPulledFromList(() => _talents.PuncturingWounds, 0.1f, 0.2f, 0.3f);
        }

        public static readonly Talents Vigor = new TalentBonusPulledFromList(() => _talents.Vigor, 10f);
        public static readonly Talents ImprovedExposeArmor = new TalentBonusPulledFromList(() => _talents.ImprovedExposeArmor, 5f, 10f);
        public static readonly Talents Lethality = new TalentBonusPulledFromList(() => _talents.Lethality, 0.06f, 0.12f, 0.18f, 0.24f, 0.30f);
        public static readonly Talents VilePoisons = new TalentBonusPulledFromList(() => _talents.VilePoisons, 0.07f, 0.14f, 0.2f);
		
        public class ImprovedPoisons
        {
            public static readonly Talents DeadlyPoison = new TalentBonusPulledFromList(() => _talents.ImprovedPoisons, 0.04f, 0.08f, 0.12f, 0.16f, 0.20f);
            public static readonly Talents InstantPoison = new TalentBonusPulledFromList(() => _talents.ImprovedPoisons, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f);
        }
        //NEED:  Cold Blood
        public static readonly Talents QuickRecovery = new TalentBonusPulledFromList(() => _talents.QuickRecovery, 0.4f, .8f);
        public static readonly Talents SealFate = new TalentBonusPulledFromList(() => _talents.SealFate, .2f, .4f, .6f, .8f, 1f);
        public static readonly Talents Murder = new TalentBonusPulledFromList(() => _talents.Murder, .02f, .04f);
        public static readonly Talents DeadlyBrew = new TalentBonusPulledFromList(() => _talents.DeadlyBrew, 0.5f, 1.0f);

        public static readonly Talents FocusedAttacks = new TalentBonusPulledFromList(() => _talents.FocusedAttacks, .66f, 1.32f, 2f);//energy per rank (e.g. .33*2, .66*2, 1*2)

        public static readonly Talents FindWeakness = new TalentBonusPulledFromList(() => _talents.FindWeakness, .02f, .04f, .06f);

        public class MasterPoisoner
        {
            //public static readonly Talents Crit = new TalentBonusPulledFromList(() => _talents.MasterPoisoner, 0.01f, 0.02f, 0.03f);
            //public static readonly Talents DeadlyPoisonApplication = new TalentBonusPulledFromList(() => _talents.MasterPoisoner, .15f, .30f, .45f);
            public static readonly Talents NotConsumeDeadlyPoison = new TalentBonusPulledFromList(() => _talents.MasterPoisoner, .33f, .66f, 1.00f);
        }

        public static readonly Talents TurnTheTables = new TalentBonusPulledFromList(() => _talents.TurnTheTables, 0.02f, 0.04f, 0.06f);
        public static readonly Talents CutToTheChase = new TalentBonusPulledFromList(() => _talents.CutToTheChase, 0.20f, 0.40f, 0.60f, 0.80f, 1.00f);

        public class HungerForBlood
        {
            public static readonly Talents EnergyCost = new TalentBonusPulledFromList(() => _talents.HungerForBlood, 15f);
            public static readonly Talents Damage = new TalentBonusCalculatedFromMethod(() => _talents.HungerForBlood, HungerForBloodCalc);

            private static float HungerForBloodCalc(int talent)
            {
                return 0.15f + ( Glyphs.GlyphOfHungerforBlood ? .03f : 0f );
            }
        }

        //---------------------------------------------------------------------
        //Combat Talents
        //---------------------------------------------------------------------
        public static readonly Talents ImprovedSinisterStrike = new TalentBonusPulledFromList(() => _talents.ImprovedSinisterStrike, 3f, 5f);
        public static readonly Talents DualWieldSpecialization = new TalentBonusPulledFromList(() => _talents.DualWieldSpecialization, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f);
        public static readonly Talents ImprovedSliceAndDice = new TalentBonusPulledFromList(() => _talents.ImprovedSliceAndDice, 0.25f, 0.50f);
        public static readonly Talents Deflection = new TalentBonusPulledFromList(() => _talents.Deflection, 0.02f, 0.04f, 0.06f);
        public static readonly Talents Precision = new TalentBonusPulledFromList(() => _talents.Precision, 0.01f, 0.02f, 0.03f, 0.04f, 0.05f);
        //NEED  Endurance - is there a need for this one?
        //NEED: Riposte (might be another CPG class)
        public static readonly Talents CloseQuartersCombat = new TalentBonusPulledFromList(() => _talents.CloseQuartersCombat, 0.01f, 0.02f, 0.03f, 0.04f, 0.05f);
        public static readonly Talents Aggression = new TalentBonusPulledFromList(() => _talents.Aggression, 0.03f, 0.06f, 0.09f, 0.12f, 0.15f);
        
        public class LightningReflexes
        {
            public static readonly Talents Haste = new TalentBonusPulledFromList(() => _talents.LightningReflexes, 0.04f, 0.07f, 0.10f);
            public static readonly Talents Dodge = new TalentBonusPulledFromList(() => _talents.LightningReflexes, 0.02f, 0.04f, 0.06f);
        }

        public static readonly Talents MaceSpecialization = new TalentBonusPulledFromList(() => _talents.MaceSpecialization, .03f, .06f, .09f, .12f, .15f);
		
        public class BladeFlurry
        {
            public static readonly Talents Haste = new TalentBonusPulledFromList(() => _talents.BladeFlurry, 0.20f);
            public static readonly Talents EnergyCost = new TalentBonusCalculatedFromMethod(() => _talents.BladeFlurry, EnergyCostWithGlyph);   

            public static float EnergyCostWithGlyph(int points)
            {
                return Glyphs.GlyphOfBladeFlurry ? 0f : 25f/120f;
            }
        }

        public static readonly Talents HackAndSlash = new TalentBonusPulledFromList(() => _talents.HackAndSlash, 0.01f, 0.02f, 0.03f, 0.04f, 0.05f);
        public static readonly Talents WeaponExpertise = new TalentBonusPulledFromList(() => _talents.WeaponExpertise, 5, 10);
        public static readonly Talents BladeTwisting = new TalentBonusPulledFromList(() => _talents.BladeTwisting, 0.05f, 0.1f);
        public static readonly Talents Vitality = new TalentBonusPulledFromList(() => _talents.Vitality, .8f, 1.6f, 2.5f);

        public class AdrenalineRush
        {
            public static readonly Talents Energy = new TalentBonusCalculatedFromMethod(() => _talents.AdrenalineRush, EnergyBonusCalc);

            private static float EnergyBonusCalc(int talent)
            {
                return ( 150f + ( Glyphs.GlyphOfAdrenalineRush ? 50f : 0f ) ) / 180f;
            }
        }

        public static readonly Talents CombatPotency = new TalentBonusPulledFromList(() => _talents.CombatPotency, .6f, 1.2f, 1.8f, 2.4f, 3f); //energy per success

        //NEED: Unfair Advantage??
        public static readonly Talents SurpriseAttacks = new TalentBonusPulledFromList(() => _talents.SurpriseAttacks, 0.1f);

        public class SavageCombat
        {
            //public static readonly Talents AttackPower = new TalentBonusPulledFromList(() => _talents.SavageCombat, .02f, .04f);
            public static readonly Talents Damage = new TalentBonusPulledFromList(() => _talents.SavageCombat, .02f, .04f);
        }

        public static readonly Talents PreyOnTheWeak = new TalentBonusPulledFromList(() => _talents.PreyOnTheWeak, 0.04f, 0.08f, 0.12f, 0.16f, 0.20f);

        //NEED  Killing Spree (might be another CPG class, but generates 0 CPs).

        //---------------------------------------------------------------------
        //Subtlety Talents
        //---------------------------------------------------------------------
        public static readonly Talents RelentlessStrikes = new TalentBonusPulledFromList(() => _talents.RelentlessStrikes, 0.04f, 0.08f, 0.12f, 0.16f, 0.20f);  //Average energy returned per combo point
        public static readonly Talents Opportunity = new TalentBonusPulledFromList(() => _talents.Opportunity, 0.1f, 0.2f);
        //NEED:  Ghostly Strike (might be another CPG class)

        public class SerratedBlades
        {
            public static readonly Talents ArmorPenetration = new TalentBonusPulledFromList(() => _talents.SerratedBlades, 213.333333f, 426.666666f, 640f);  //NEEDS UPDATING:  Armor Pen bonus is 0 on Rawr and the website
            public static readonly Talents Rupture = new TalentBonusPulledFromList(() => _talents.SerratedBlades, 0.1f, 0.2f, 0.3f);  //NEEDS UPDATING:  Armor Pen bonus is 0 on Rawr and the website
        }

        public class DirtyDeeds {
            public static readonly Talents EnergyCost = new TalentBonusPulledFromList(() => _talents.DirtyDeeds, 0.1f, 0.2f);
            public static readonly Talents DamageSpecialAbilities = new TalentBonusPulledFromList(() => _talents.DirtyDeeds, 0.1f, 0.2f);
        }
 
        public static readonly Talents Deadliness = new TalentBonusPulledFromList(() => _talents.Deadliness, 0.02f, 0.04f, 0.06f, 0.08f, 0.10f);

        //NEED Premeditation
        public class SinisterCalling
        {
            //public static readonly Talents Agility = new TalentBonusPulledFromList(() => _talents.SinisterCalling, 0.03f, 0.06f, 0.09f, 0.12f, 0.15f);
            public static readonly Talents HemoAndBackstab = new TalentBonusPulledFromList(() => _talents.SinisterCalling, 0.02f, 0.04f, 0.06f, 0.08f, 0.10f);
        }

        //NEED  Shadowstep
        //NEED  Filthy Tricks

        public class SlaughterFromTheShadows
        {
            public static readonly Talents BackstabAndAmbushEnergyCost = new TalentBonusPulledFromList(() => _talents.SlaughterFromTheShadows, 3f, 6f, 9f, 12f, 15f);
            public static readonly Talents HemoEnergyCost = new TalentBonusPulledFromList(() => _talents.SlaughterFromTheShadows, 1f, 2f, 3f, 4f, 5f);
        }

        //---------------------------------------------------------------------
        //Class Initializers and Methods
        //---------------------------------------------------------------------
        public static void Initialize(RogueTalents talents)
        {
            _talents = talents;
        }
        public abstract float Bonus { get; }

        public float Multiplier
        {
            get { return 1f + Bonus; }
        }

        public virtual bool HasPoints
        {
            get { return _talent() > 0; }
        }

        public static Talents Add(params Talents[] talents)
        {
            return new AddedTalents(talents);
        }

        //---------------------------------------------------------------------
        // Sub Classes
        //---------------------------------------------------------------------
        protected class TalentBonusPulledFromList : Talents
        {
            public TalentBonusPulledFromList(TalentDelegate talent, params float[] multipliers)
                : base(talent)
            {
                _bonuses = new float[multipliers.Length + 1];
                _bonuses[0] = 0f;
                multipliers.CopyTo(_bonuses, 1);
            }

            protected float[] _bonuses;

            public override float Bonus
            {
                get { return _bonuses[_talent()]; }
            }

            public void SetBonusValues(float[] bonuses)
            {
                _bonuses = bonuses;
            }
        }

        protected class TalentBonusCalculatedFromMethod : Talents
        {
            public TalentBonusCalculatedFromMethod(TalentDelegate talent, CalculationDelegate calculation)
                : base(talent)
            {
                _calculation = calculation;
            }

            private readonly CalculationDelegate _calculation;

            public override float Bonus
            {
                get { return _talent() == 0 ? 0f : _calculation(_talent()); }
            }
        }

        protected class AddedTalents : Talents
        {
            public AddedTalents(params Talents[] talents) : base(null)
            {
                _talentList.AddRange(talents);
            }

            private readonly List<Talents> _talentList  = new List<Talents>();

            public override float Bonus
            {
                get
                {
                    var dmg = 0f;
                    foreach (var talent in _talentList)
                    {
                        dmg += talent.Bonus;
                    }
                    return dmg;
                }
            }

            public override bool HasPoints
            {
                get { return true; }
            }
        }
    }
}