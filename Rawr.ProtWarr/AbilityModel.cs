using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Rawr.ProtWarr
{
    public class AbilityModel
    {
        private Character Character;
        private Stats Stats;
        private WarriorTalents Talents;
        //private WarriorTalentsCata TalentsCata;
        private CalculationOptionsProtWarr Options;

        public readonly AttackTable AttackTable;

        public Ability Ability { get; private set; }
        public string Name { get; private set; }
        public float Damage { get; private set; }
        public float Threat { get; private set; }
        public float DamageMultiplier { get; private set; }
        public float ArmorReduction { get; private set; }
        public float CritPercentage
        {
            get { return AttackTable.Critical; }
        }
        public float HitPercentage
        {
            get { return AttackTable.AnyHit; }
        }
        public bool IsAvoidable { get; private set; }
        public bool IsWeaponAttack { get; private set; }

        private void CalculateDamage()
        {
            float baseDamage        = 0.0f;
            float critMultiplier    = 1.0f + Lookup.BonusCritMultiplier(Character, Stats, Ability);

            switch (Ability)
            {
                // White Damage
                case Ability.None:
                    baseDamage = Lookup.WeaponDamage(Character, Stats, false);
                    break;
                case Ability.Cleave:
                    baseDamage = Lookup.WeaponDamage(Character, Stats, false) + (222.0f * (1.0f + Talents.ImprovedCleave * 0.4f));
                    break;
                case Ability.ConcussionBlow:
                    baseDamage = Stats.AttackPower * 0.38f;
                    break;
                case Ability.DamageShield:
                    baseDamage = Stats.BlockValue * (Talents.DamageShield * 0.1f);
                    break;
                case Ability.DeepWounds:
                    baseDamage = Lookup.WeaponDamage(Character, Stats, false) * (Talents.DeepWounds * 0.16f);
                    DamageMultiplier *= (1.0f + Stats.BonusBleedDamageMultiplier);
                    ArmorReduction = 0.0f;
                    break;
                case Ability.Devastate:
                    // Assumes 5 stacks of Sunder Armor debuff
                    baseDamage = (Lookup.WeaponDamage(Character, Stats, true) + (202.0f * 5.0f)) * 1.2f;
                    DamageMultiplier *= (1.0f + Stats.BonusDevastateDamage) ;
                    break;
                case Ability.HeroicStrike:
                    baseDamage = Lookup.WeaponDamage(Character, Stats, false) + 495.0f;
                    break;
                case Ability.HeroicThrow:
                    baseDamage = 12.0f + (Stats.AttackPower * 0.5f);
                    break;
                case Ability.MockingBlow:
                    baseDamage = Lookup.WeaponDamage(Character, Stats, true);
                    if(Talents.GlyphOfMockingBlow)
                        DamageMultiplier *= 1.25f;
                    break;
                case Ability.Rend:
                    baseDamage = 380.0f + Lookup.WeaponDamage(Character, Stats, false);
                    if (Talents.GlyphOfRending)
                        baseDamage *= 1.4f;
                    DamageMultiplier *= (1.0f + Talents.ImprovedRend * 0.2f) * (1.0f + Stats.BonusBleedDamageMultiplier);
                    ArmorReduction = 0.0f;
                    break;
                case Ability.Revenge:
                    baseDamage = (1816.5f * (1.0f + Talents.ImprovedRevenge * 0.3f)) + (Stats.AttackPower * 0.31f);
                    DamageMultiplier *= (1.0f + Talents.UnrelentingAssault * 0.1f);
                    break;
                case Ability.ShieldSlam:
                    float softCap = 24.5f * Character.Level;
                    float hardCap = 39.5f * Character.Level;
                    if (Stats.BlockValue < softCap)
                        baseDamage = 1015.0f + Stats.BlockValue;
                    else
                        baseDamage = 1015.0f + softCap 
                            + (0.98f * (Math.Min(Stats.BlockValue, hardCap) - softCap)) 
                            - (0.00073885f * (float)Math.Pow((double)(Math.Min(Stats.BlockValue, hardCap) - softCap), 2.0d));
                    DamageMultiplier *= (1.0f + Stats.BonusShieldSlamDamage);
                    break;
                case Ability.Shockwave:
                    baseDamage = Stats.AttackPower * 0.75f;
                    DamageMultiplier *= (1.0f + Stats.BonusShockwaveDamage);
                    break;
                case Ability.Slam:
                    baseDamage = Lookup.WeaponDamage(Character, Stats, false) + 250.0f;
                    break;
                case Ability.ThunderClap:
                    baseDamage = 300.0f + (Stats.AttackPower * 0.12f);
                    DamageMultiplier *= (1.0f + Talents.ImprovedThunderClap * 0.1f);
                    break;
                case Ability.Vigilance:
                    baseDamage = 0.0f;
                    break;
            }

            // All damage multipliers
            baseDamage *= DamageMultiplier;
            // Armor reduction
            baseDamage *= (1.0f - ArmorReduction);
            // Combat table adjustments
            baseDamage *= 
                AttackTable.Hit + 
                AttackTable.Critical * critMultiplier + 
                AttackTable.Glance * Lookup.GlancingReduction(Character, Options.TargetLevel);

            Damage = baseDamage;
        }

        private void CalculateThreat()
        {
            // Base threat is always going to be the damage of the ability, if it is damaging
            float abilityThreat = Damage;

            switch (Ability)
            {
                case Ability.Cleave:
                    abilityThreat += 225.0f;
                    break;
                case Ability.ConcussionBlow:
                    abilityThreat *= 2.0f;
                    break;
                case Ability.Devastate:
                    // Glyph of Devastate doubles bonus threat
                    if(Talents.GlyphOfDevastate)
                        abilityThreat += (315 + (Stats.AttackPower * 0.05f)) * 2.0f;
                    else
                        abilityThreat += 315 + (Stats.AttackPower * 0.05f);
                    break;
                case Ability.HeroicStrike:
                    abilityThreat += 259.0f;
                    break;
                case Ability.HeroicThrow:
                    abilityThreat *= 1.5f;
                    break;
                case Ability.MockingBlow:
                    if(Talents.GlyphOfBarbaricInsults)
                        abilityThreat *= 6.0f;
                    else
                        abilityThreat *= 3.0f;
                    break;
                case Ability.Revenge:
                    abilityThreat += 121.0f;
                    break;
                case Ability.ShieldBash:
                    abilityThreat += 36.0f;
                    break;
                case Ability.ShieldSlam:
                    abilityThreat += 770.0f;
                    abilityThreat *= 1.3f;
                    break;
                case Ability.Slam:
                    abilityThreat += 140.0f;
                    break;
                case Ability.SunderArmor:
                    abilityThreat += 345.0f + (Stats.AttackPower * 0.05f);
                    break;
                case Ability.ThunderClap:
                    abilityThreat *= 1.85f;
                    break;
                case Ability.Vigilance:
                    if (Talents.GlyphOfVigilance)
                        abilityThreat = (Options.VigilanceValue * 0.15f) * Talents.Vigilance;
                    else
                        abilityThreat = (Options.VigilanceValue * 0.1f) * Talents.Vigilance;
                    break;
            }

            // All abilities other than Vigilance are affected by Defensive Stance
            if(Ability != Ability.Vigilance)
                abilityThreat *= Lookup.StanceThreatMultipler(Character, Stats);

            Threat = abilityThreat;
        }

        public AbilityModel(Character character, Stats stats, CalculationOptionsProtWarr options, Ability ability)
        {
            Character   = character;
            Stats       = stats;
            Ability     = ability;
            Options     = options;
            Talents     = Character.WarriorTalents;
            AttackTable = new AttackTable(character, stats, options, ability);

            Name                = Lookup.Name(Ability);
            ArmorReduction      = Lookup.TargetArmorReduction(Character, Stats, Options.TargetArmor);
            DamageMultiplier    = Lookup.StanceDamageMultipler(Character, Stats);
            IsAvoidable         = Lookup.IsAvoidable(Ability);
            IsWeaponAttack      = Lookup.IsWeaponAttack(Ability);

            CalculateDamage();
            CalculateThreat();
        }
    }

    public class AbilityModelList : KeyedCollection<Ability, AbilityModel>
    {
        protected override Ability GetKeyForItem(AbilityModel abilityModel)
        {
            return abilityModel.Ability;
        }

        public void Add(Ability ability, Character character, Stats stats, CalculationOptionsProtWarr options)
        {
            this.Add(new AbilityModel(character, stats, options, ability));
        }
    }
}
