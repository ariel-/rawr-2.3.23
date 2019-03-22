using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Retribution
{

    public abstract class Skill
    {

        protected Stats _stats;
        public Stats Stats { get { return _stats; } }

        private CalculationOptionsRetribution _calcOpts;
        public CalculationOptionsRetribution CalcOpts { get { return _calcOpts; } }

        private PaladinTalents _talents;
        public PaladinTalents Talents { get { return _talents; } }

        private CombatStats _combats;
        public CombatStats Combats
        {
            get { return _combats; }
            set
            {
                _combats = value;
                _stats = value.Stats;
                _calcOpts = value.CalcOpts;
                _talents = value.Talents;
            }
        }
        public AbilityType AbilityType { get; set; }
        public DamageType DamageType { get; set; }
        public bool UsesWeapon { get; set; }
        public bool RighteousVengeance { get; set; }

        public virtual bool UsableBefore20PercentHealth
        {
            get { return true; }
        }

        public virtual bool UsableAfter20PercentHealth
        {
            get { return true; }
        }

        public virtual Ability? RotationAbility
        {
            get { return null; }
        }


        public Skill(CombatStats combats, AbilityType abilityType, DamageType damageType, bool usesWeapon, bool righteousVengeance)
        {
            Combats = combats;
            AbilityType = abilityType;
            DamageType = damageType;
            UsesWeapon = usesWeapon;
            RighteousVengeance = righteousVengeance;
        }

        public virtual float AverageDamage()
        {
            return HitDamage() * ((1f - CritChance()) + CritChance() * CritBonus()) * ChanceToLand() * Targets();
        }

        public float CritChance()
        {
            if (AbilityType == AbilityType.Spell)
            {
                return (float)Math.Max(Math.Min(1f, Stats.SpellCrit + AbilityCritChance()), 0);
            }
            else
            {
                return (float)Math.Max(Math.Min(1f, Stats.PhysicalCrit + AbilityCritChance()), 0);
            }
        }

        public virtual float ChanceToLand()
        {
            if (AbilityType == AbilityType.Melee)
            {
                return 1 - (Combats.GetMeleeMissChance() + Combats.GetToBeDodgedChance() + Combats.GetToBeParriedChance() * _calcOpts.InFront);
            }
            else if (AbilityType == AbilityType.Range)
            {
                return 1 - Combats.GetRangedMissChance();
            }
            else // Spell
            {
                return 1 - Combats.GetSpellMissChance();
            }
        }

        public float ChanceToCrit() { return CritChance() * ChanceToLand(); }

        public float CritBonus()
        {
            float rightVen = 1;
            if (RighteousVengeance)
            {
                if (Stats.RighteousVengeanceCanCrit != 0)
                {
                    rightVen += .1f * Talents.RighteousVengeance * (1f + Stats.PhysicalCrit);
                }
                else
                {
                    rightVen += .1f * Talents.RighteousVengeance;
                }
            }
            if (AbilityType == AbilityType.Spell)
            {
                return 1.5f * (1f + Stats.BonusSpellCritMultiplier) * rightVen;
            }
            else
            {
                return 2f * (1f + Stats.BonusCritMultiplier) * rightVen;
            }
        }

        public float HitDamage()
        {
            float damage = AbilityDamage();
            if (DamageType == DamageType.Physical)
            {
                damage *= Combats.ArmorReduction;
                damage *= (1f + Stats.BonusPhysicalDamageMultiplier);
            }
            else // Holy Damage
            {
                damage *= Combats.PartialResist;
                damage *= (1f + Stats.BonusHolyDamageMultiplier);
            }
            damage *= 1f + Stats.BonusDamageMultiplier;
            if (DamageType != DamageType.Magic) damage *= 1f + .03f * Talents.Vengeance;
            if (UsesWeapon) damage *= 1f + .02f * Talents.TwoHandedWeaponSpecialization;
            damage *= (1f + .01f * Talents.Crusade);
            if (CalcOpts.Mob != MobType.Other) damage *= (1f + .01f * Talents.Crusade);
            damage *= Combats.AvengingWrathMulti;
            damage *= (Talents.GlyphOfSenseUndead && CalcOpts.Mob == MobType.Undead ? 1.01f : 1f);
            return damage;
        }

        public abstract float AbilityDamage();

        public virtual float AbilityCritChance() { return 0; }
        public virtual float Targets() { return 1f; }

        public virtual float TickCount()
        {
            return 1;
        }

        public override string ToString()
        {
            return string.Format("Average Damage: {0}\nAverage Hit: {1}\nCrit Chance: {2}%\nAvoid Chance: {3}%",
                AverageDamage().ToString("N0"),
                HitDamage().ToString("N0"),
                Math.Round(ChanceToCrit() * 100, 2),
                Math.Round((1f - ChanceToLand()) * 100, 2));
        }

    }

    public class JudgementOfCommand : Skill
    {

        public JudgementOfCommand(CombatStats combats) 
            : base(combats, AbilityType.Range, DamageType.Holy, true, true) { }


        public override Ability? RotationAbility
        {
            get { return Ability.Judgement; }
        }


        public override float AbilityDamage()
        {
            return (Combats.WeaponDamage * .19f + Stats.SpellPower * .13f + Stats.AttackPower * .08f)
                * (1f + .05f * Talents.TheArtOfWar + (Talents.GlyphOfJudgement ? 0.1f : 0f + Stats.JudgementMultiplier));
        }

        public override float AbilityCritChance()
        {
            return Talents.Fanaticism * .06f + Stats.JudgementCrit;
        }

    }

    public class JudgementOfRighteousness : Skill
    {

        public JudgementOfRighteousness(CombatStats combats) 
            : base(combats, AbilityType.Range, DamageType.Holy, true, true) { }


        public override Ability? RotationAbility
        {
            get { return Ability.Judgement; }
        }


        public override float AbilityDamage()
        {
            return (1f + Stats.SpellPower * .32f + Stats.AttackPower * .2f)
                * (1f + .05f * Talents.TheArtOfWar + .03f * Talents.SealsOfThePure
                + (Talents.GlyphOfJudgement ? 0.1f : 0f) + Stats.JudgementMultiplier);
        }

        public override float AbilityCritChance()
        {
            return Talents.Fanaticism * .06f + Stats.JudgementCrit;
        }

    }

    public class JudgementOfVengeance : Skill
    {

        public JudgementOfVengeance(CombatStats combats, float averageStack)
            : base(combats, AbilityType.Range, DamageType.Holy, true, true)
        {
            AverageStackSize = averageStack;
        }


        public override Ability? RotationAbility
        {
            get { return Ability.Judgement; }
        }

        public float AverageStackSize { get; private set; }


        public override float AbilityDamage()
        {
            return (1.0f + Stats.SpellPower * 0.22f + Stats.AttackPower * 0.14f) * (1f + 0.1f * AverageStackSize)
                * (1f + .05f * Talents.TheArtOfWar + .03f * Talents.SealsOfThePure
                + (Talents.GlyphOfJudgement ? 0.1f : 0f) + Stats.JudgementMultiplier);
        }

        public override float AbilityCritChance()
        {
            return Talents.Fanaticism * .06f + Stats.JudgementCrit;
        }

    }

    public class CrusaderStrike : Skill
    {

        public CrusaderStrike(CombatStats combats) 
            : base(combats, AbilityType.Melee, DamageType.Physical, true, true) { }


        public override Ability? RotationAbility
        {
            get { return Ability.CrusaderStrike; }
        }


        public override float AbilityDamage()
        {
            return (Combats.NormalWeaponDamage * .75f + Stats.CrusaderStrikeDamage)
                * (1f + .05f * Talents.SanctityOfBattle + .05f * Talents.TheArtOfWar + Stats.CrusaderStrikeMultiplier);
        }

        public override float AbilityCritChance()
        {
            return Stats.CrusaderStrikeCrit;
        }

    }

    public class NullCrusaderStrike : Skill
    {

        public NullCrusaderStrike(CombatStats combats)
            : base(combats, AbilityType.Melee, DamageType.Physical, true, true) { }


        public override Ability? RotationAbility
        {
            get { return Ability.CrusaderStrike; }
        }

        public override bool UsableAfter20PercentHealth
        {
            get { return false; }
        }

        public override bool UsableBefore20PercentHealth
        {
            get { return false; }
        }


        public override float AbilityDamage()
        {
            return 0;
        }

    }

    public class DivineStorm : Skill
    {

        public DivineStorm(CombatStats combats) 
            : base(combats, AbilityType.Melee, DamageType.Physical, true, true) { }


        public override Ability? RotationAbility
        {
            get { return Ability.DivineStorm; }
        }


        public override float AbilityDamage()
        {
            return (Combats.NormalWeaponDamage * 1.1f + Stats.DivineStormDamage)
                * (1f + .05f * Talents.TheArtOfWar + Stats.DivineStormMultiplier);
        }

        public override float AbilityCritChance()
        {
            return Stats.DivineStormCrit;
        }

        public override float Targets()
        {
            return (float)Math.Min(Combats.CalcOpts.Targets, 3f);
        }

    }

    public class NullDivineStorm : Skill
    {

        public NullDivineStorm(CombatStats combats)
            : base(combats, AbilityType.Melee, DamageType.Physical, true, true) { }


        public override Ability? RotationAbility
        {
            get { return Ability.DivineStorm; }
        }

        public override bool UsableAfter20PercentHealth
        {
            get { return false; }
        }

        public override bool UsableBefore20PercentHealth
        {
            get { return false; }
        }


        public override float AbilityDamage()
        {
            return 0;
        }

    }

    public class HammerOfWrath : Skill
    {

        public HammerOfWrath(CombatStats combats) 
            : base(combats, AbilityType.Range, DamageType.Holy, false, false) { }


        public override Ability? RotationAbility
        {
            get { return Ability.HammerOfWrath; }
        }

        public override bool UsableBefore20PercentHealth
        {
            get { return false; }
        }


        public override float AbilityDamage()
        {
            return (1198f + .15f * Stats.SpellPower + .15f * Stats.AttackPower)
                * (1f + Stats.HammerOfWrathMultiplier);
        }

        public override float AbilityCritChance()
        {
            return .25f * Talents.SanctifiedWrath;
        }

    }

    public class Exorcism : Skill
    {

        public Exorcism(CombatStats combats) 
            : base(combats, AbilityType.Spell, DamageType.Holy, false, false) { }


        public override Ability? RotationAbility
        {
            get { return Ability.Exorcism; }
        }


        public override float AbilityDamage()
        {
            return (1087f + .42f * Stats.SpellPower)
                * (1f + .05f * Talents.SanctityOfBattle + Stats.ExorcismMultiplier + (Talents.GlyphOfExorcism ? 0.2f : 0f));
        }

        public override float AbilityCritChance()
        {
            return (CalcOpts.Mob == MobType.Demon || CalcOpts.Mob == MobType.Undead) ? 1f : 0;
        }

    }

    public class Consecration : Skill
    {

        public Consecration(CombatStats combats) 
            : base(combats, AbilityType.Spell, DamageType.Holy, false, false) { }


        public override Ability? RotationAbility
        {
            get { return Ability.Consecration; }
        }


        public override float AbilityDamage()
        {
            return (113f + .04f * (Stats.SpellPower + Stats.ConsecrationSpellPower) + .04f * Stats.AttackPower)
                * TickCount() * (CalcOpts.ConsEff);
        }

        public override float AbilityCritChance()
        {
            return -1f; // -1 = can't crit.
        }

        public override float Targets()
        {
            return Combats.CalcOpts.Targets;
        }

        public override float TickCount()
        {
            // Every second for 8 seconds (10 seconds with glyph)
            return Talents.GlyphOfConsecration ? 10f : 8f;
        }

    }

    public class SealOfCommand : Skill
    {

        public SealOfCommand(CombatStats combats) : base(combats, AbilityType.Melee, DamageType.Holy, true, false) { }

        public override float AbilityDamage()
        {
            return (Combats.WeaponDamage * .36f) * (1f + Stats.SealMultiplier);
        }

        public override float Targets()
        {
            return (float)Math.Min(3f, Combats.CalcOpts.Targets);
        }

    }

    public class SealOfRighteousness : Skill
    {

        public SealOfRighteousness(CombatStats combats) : base(combats, AbilityType.Spell, DamageType.Holy, true, false) { }

        public override float AbilityDamage()
        {
            return Combats.BaseWeaponSpeed * (0.022f * Stats.AttackPower + 0.044f * Stats.SpellPower) * (1f
                + .03f * Talents.SealsOfThePure
                + (Talents.GlyphOfSealOfRighteousness ? 0.1f : 0f)
                + Stats.BonusSealOfRighteousnessDamageMultiplier)
                + Stats.SealMultiplier;
        }

        public override float AbilityCritChance() { return -1f; }
        public override float ChanceToLand() { return 1f; }

    }

    public class SealOfVengeanceDoT : Skill
    {

        public SealOfVengeanceDoT(CombatStats combats, float averageStack)
            : base(combats, AbilityType.Spell, DamageType.Holy, false, false)
        {
            AverageStackSize = averageStack;
        }

        public float AverageStackSize { get; private set; }

        public override float AbilityDamage()
        {
            return AverageStackSize * (Stats.SpellPower * 0.065f + Stats.AttackPower * 0.13f) / 5f * (1f
                + .03f * Talents.SealsOfThePure
                + Stats.BonusSealOfVengeanceDamageMultiplier
                + Stats.SealMultiplier);
        }

        public override float AbilityCritChance() { return -1f; }
        public override float ChanceToLand() { return 1f; }

    }

    public class SealOfVengeance : Skill
    {

        public SealOfVengeance(CombatStats combats, float averageStack)
            : base(combats, AbilityType.Melee, DamageType.Holy, true, false)
        {
            AverageStackSize = averageStack;
        }

        public float AverageStackSize { get; private set; }

        public override float AbilityDamage()
        {
            return (Combats.WeaponDamage * 0.066f * AverageStackSize) * (1f
                + Talents.SealsOfThePure * .03f
                + Stats.BonusSealOfVengeanceDamageMultiplier
                + Stats.SealMultiplier);
        }

        public override float ChanceToLand() { return 1f; }
    }  

    public class White : Skill
    {

        public White(CombatStats combats) : base(combats, AbilityType.Melee, DamageType.Physical, true, false) { }

        public override float AbilityDamage()
        {
            return Combats.WeaponDamage;
        }

        public override float AverageDamage()
        {
            const float glanceChance = .24f;
            const float glancingAmount = 1f - 0.25f;
            return HitDamage() *
                (glanceChance * glancingAmount +
                CritChance() * CritBonus() +
                (ChanceToLand() - CritChance() - glanceChance));
        }

        public float WhiteDPS()
        {
            return AverageDamage() / Combats.AttackSpeed + Stats.MoteOfAnger * AverageDamage();
        }

    }

    public class HandOfReckoning : Skill
    {

        public HandOfReckoning(CombatStats combats) : base(combats, AbilityType.Spell, DamageType.Holy, false, false) { }

        public override float AbilityDamage()
        {
            return 1f + Stats.AttackPower * 0.5f;
        }

    }

    public class NullSeal : Skill
    {

        public NullSeal(CombatStats combats) : base(combats, AbilityType.Melee, DamageType.Holy, true, false) { }

        public override float AbilityDamage()
        {
            return 0;
        }

    }

    public class NullSealDoT : Skill
    {

        public NullSealDoT(CombatStats combats) : base(combats, AbilityType.Melee, DamageType.Holy, true, false) { }

        public override float AbilityDamage()
        {
            return 0;
        }

    }

    public class NullJudgement : Skill
    {

        public NullJudgement(CombatStats combats) : base(combats, AbilityType.Melee, DamageType.Holy, true, false) { }


        public override Ability? RotationAbility
        {
            get { return Ability.Judgement; }
        }

        public override bool UsableBefore20PercentHealth
        {
            get { return false; }
        }

        public override bool UsableAfter20PercentHealth
        {
            get { return false; }
        }


        public override float AbilityDamage()
        {
            return 0;
        }

    }

    public class MagicDamage : Skill
    {
        private float amount;

        public MagicDamage(CombatStats combats, float amount)
            : base(combats, AbilityType.Spell, DamageType.Magic, false, false)
        {
            this.amount = amount;
        }

        public override float AbilityDamage()
        {
            return amount;
        }
    }

}
