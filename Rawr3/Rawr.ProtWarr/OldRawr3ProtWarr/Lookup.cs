using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.ProtWarr
{
    public static class Lookup
    {
        public static float LevelModifier(Character character)
        {
            CalculationOptionsProtWarr calcOpts = character.CalculationOptions as CalculationOptionsProtWarr;
            return (calcOpts.TargetLevel - character.Level) * 0.2f;
        }

        public static float TargetArmorReduction(Character character, Stats stats)
        {
            CalculationOptionsProtWarr calcOpts = character.CalculationOptions as CalculationOptionsProtWarr;
			int targetArmor = calcOpts.TargetArmor;
            float ignoreArmor = 0.0f;
            if (character.MainHand != null && (character.MainHand.Type == ItemType.OneHandMace))
                ignoreArmor += character.WarriorTalents.MaceSpecialization * 0.03f;

			return StatConversion.GetArmorDamageReduction(character.Level, targetArmor, stats.ArmorPenetration, ignoreArmor, stats.ArmorPenetrationRating);
        }

        public static float TargetCritChance(Character character, Stats stats)
        {
            return Math.Max(0.0f, ((5.0f + Lookup.LevelModifier(character)) / 100.0f) - AvoidanceChance(character, stats, HitResult.Crit));
        }

        public static float TargetAvoidanceChance(Character character, Stats stats, HitResult avoidanceType)
        {
            CalculationOptionsProtWarr calcOpts = character.CalculationOptions as CalculationOptionsProtWarr;

            switch (avoidanceType)
            {
                case HitResult.Miss: 
                    return StatConversion.WHITE_MISS_CHANCE_CAP[calcOpts.TargetLevel - 80];
                case HitResult.Dodge:
                    return StatConversion.YELLOW_DODGE_CHANCE_CAP[calcOpts.TargetLevel - 80] - (0.01f * character.WarriorTalents.WeaponMastery);
                case HitResult.Parry:
                    return StatConversion.YELLOW_PARRY_CHANCE_CAP[calcOpts.TargetLevel - 80];
                case HitResult.Glance:
                    return 0.06f + ((calcOpts.TargetLevel - character.Level) * 0.06f);
                default:
                    return 0.0f;
            }
        }

        public static float StanceDamageMultipler(Character character, Stats stats)
        {
            // In Defensive Stance
            return (0.95f * (1.0f + character.WarriorTalents.ImprovedDefensiveStance * 0.05f) * (1.0f + stats.BonusDamageMultiplier));
        }

        public static float StanceThreatMultipler(Character character, Stats stats)
        {
            // In Defensive Stance
            return (2.0735f * (1.0f + stats.ThreatIncreaseMultiplier));
        }

        public static float StanceDamageReduction(Character character, Stats stats)
        {
            return StanceDamageReduction(character, stats, DamageType.Physical);
        }

        public static float StanceDamageReduction(Character character, Stats stats, DamageType damageType)
        {
            // In Defensive Stance
            float damageTaken = 0.9f * (1.0f + stats.DamageTakenMultiplier);
            
            switch (damageType)
            {
                case DamageType.Arcane:
                case DamageType.Fire:
                case DamageType.Frost:
                case DamageType.Nature:
                case DamageType.Shadow:
                case DamageType.Holy:
                    return damageTaken * (1.0f - character.WarriorTalents.ImprovedDefensiveStance * 0.03f);
                default:
                    return damageTaken;
            }
        }

        public static float BonusArmorPenetrationPercentage(Character character, Stats stats)
        {
            return StatConversion.GetArmorPenetrationFromRating(stats.ArmorPenetrationRating, CharacterClass.Warrior);
            //return ((stats.ArmorPenetrationRating * ProtWarr.ArPToArmorPenetration) / 100.0f);
        }

        public static float BonusExpertisePercentage(Character character, Stats stats)
        {
            return StatConversion.GetDodgeParryReducFromExpertise(stats.Expertise + 
                StatConversion.GetExpertiseFromRating(stats.ExpertiseRating), CharacterClass.Warrior);
            //return (((stats.ExpertiseRating * ProtWarr.ExpertiseRatingToExpertise) + stats.Expertise)
            //        * ProtWarr.ExpertiseToDodgeParryReduction) / 100.0f;
        }

        public static float BonusHastePercentage(Character character, Stats stats)
        {
            return StatConversion.GetPhysicalHasteFromRating(stats.HasteRating, CharacterClass.Warrior) + stats.PhysicalHaste;
            //return ((stats.HasteRating * ProtWarr.HasteRatingToHaste) / 100.0f) + stats.PhysicalHaste;
        }

        public static float BonusHitPercentage(Character character, Stats stats)
        {
            return StatConversion.GetPhysicalHitFromRating(stats.HitRating, CharacterClass.Warrior) + stats.PhysicalHit;
            //return ((stats.HitRating * ProtWarr.HitRatingToHit) / 100.0f) + stats.PhysicalHit;
        }

        public static float BonusCritMultiplier(Character character, Stats stats, Ability ability)
        {
            if (ability == Ability.None)
                return (2.0f * (1.0f + stats.BonusCritMultiplier) - 1.0f);
            else
                return (2.0f * (1.0f + stats.BonusCritMultiplier) - 1.0f) * (1.0f + character.WarriorTalents.Impale * 0.1f);
        }

        public static float BonusCritPercentage(Character character, Stats stats)
        {
            CalculationOptionsProtWarr calcOpts = character.CalculationOptions as CalculationOptionsProtWarr;

            return Math.Max(0.0f, Math.Min(1.0f, 
                StatConversion.GetPhysicalCritFromRating(stats.CritRating, CharacterClass.Warrior) + 
                StatConversion.GetPhysicalCritFromAgility(stats.Agility, CharacterClass.Warrior) + 
                stats.PhysicalCrit - StatConversion.NPC_LEVEL_CRIT_MOD[calcOpts.TargetLevel - character.Level]));

            /*
            if ((calcOpts.TargetLevel - character.Level) < 3)
                // This formula may or may not be accurate anymore, as the modifier on +1/2 mobs is untested
                return Math.Min(1.0f, (((stats.CritRating * ProtWarr.CritRatingToCrit) + (stats.Agility * ProtWarr.AgilityToCrit)
                                    - LevelModifier(character)) / 100.0f) + stats.PhysicalCrit);
            else
                // 4.8% critical hit supression as tested on bosses
                return Math.Min(1.0f, (((stats.CritRating * ProtWarr.CritRatingToCrit) + (stats.Agility * ProtWarr.AgilityToCrit)
                                    - 4.8f) / 100.0f) + stats.PhysicalCrit);
            */
        }

        public static float BonusCritPercentage(Character character, Stats stats, Ability ability)
        {
            // Grab base melee crit chance before adding ability-specific crit chance
            float abilityCritChance = BonusCritPercentage(character, stats);

            switch (ability)
            {
                case Ability.Cleave:
                    abilityCritChance += character.WarriorTalents.Incite * 0.05f;
                    break;
                case Ability.Devastate:
                    abilityCritChance += stats.DevastateCritIncrease;
                    break;
                case Ability.HeroicStrike:
                    abilityCritChance += character.WarriorTalents.Incite * 0.05f;
                    break;
                case Ability.ShieldSlam:
                    abilityCritChance += character.WarriorTalents.CriticalBlock * 0.05f;
                    break;
                case Ability.ThunderClap:
                    abilityCritChance += character.WarriorTalents.Incite * 0.05f;
                    break;
                case Ability.SunderArmor:
                case Ability.ShieldBash:
                case Ability.Rend:
                case Ability.DeepWounds:
                    abilityCritChance = 0.0f;
                    break;
            }

            return Math.Min(1.0f, abilityCritChance);
        }

        public static float WeaponDamage(Character character, Stats stats, bool normalized)
        {
            float weaponDamage = 1.0f;

            if (character.MainHand != null)
            {
                float weaponSpeed     = character.MainHand.Speed;
                float weaponMinDamage = character.MainHand.MinDamage;
                float weaponMaxDamage = character.MainHand.MaxDamage;
                float normalizedSpeed = 1.0f;
                if (character.MainHand.Type == ItemType.Dagger)
                    normalizedSpeed = 1.7f;
                else
                    normalizedSpeed = 2.4f;
            
                // Non-Normalized Hits
                if (!normalized)
                    weaponDamage = ((weaponMinDamage + weaponMaxDamage) / 2.0f + (weaponSpeed * stats.AttackPower / 14.0f)) + stats.WeaponDamage;
                // Normalized Hits
                else
                    weaponDamage = ((weaponMinDamage + weaponMaxDamage) / 2.0f + (normalizedSpeed * stats.AttackPower / 14.0f)) + stats.WeaponDamage;
            }

            return weaponDamage;
        }

        public static float WeaponSpeed(Character character, Stats stats)
        {
            if (character.MainHand != null)
                return Math.Max(1.0f, character.MainHand.Speed / (1.0f + BonusHastePercentage(character, stats)));
            else
                return 1.0f;
        }

        public static float GlancingReduction(Character character)
        {
            CalculationOptionsProtWarr calcOpts = character.CalculationOptions as CalculationOptionsProtWarr;
            return (Math.Min(0.91f, 1.3f - (0.05f * (calcOpts.TargetLevel - character.Level) * 5.0f)) +
                    Math.Max(0.99f, 1.2f - (0.03f * (calcOpts.TargetLevel - character.Level) * 5.0f))) / 2;
        }

        public static float ArmorReduction(Character character, Stats stats)
        {
            CalculationOptionsProtWarr calcOpts = character.CalculationOptions as CalculationOptionsProtWarr;
            return StatConversion.GetArmorDamageReduction(calcOpts.TargetLevel, stats.Armor, 0.0f, 0.0f, 0.0f);
            // return Math.Max(0.0f, Math.Min(0.75f, stats.Armor / (stats.Armor + (467.5f * calcOpts.TargetLevel - 22167.5f))));
        }

        public static float MagicReduction(Character character, Stats stats, DamageType school)
        {
            CalculationOptionsProtWarr calcOpts = character.CalculationOptions as CalculationOptionsProtWarr;
            float damageReduction = Lookup.StanceDamageReduction(character, stats, school);
            float totalResist = stats.AllResist;
            float resistScale = 0.0f;

            switch (school)
            {
                case DamageType.Arcane: totalResist += stats.ArcaneResistance; break;
                case DamageType.Fire: totalResist += stats.FireResistance; break;
                case DamageType.Frost: totalResist += stats.FrostResistance; break;
                case DamageType.Nature: totalResist += stats.NatureResistance; break;
                case DamageType.Shadow: totalResist += stats.ShadowResistance; break;
            }

            if ((calcOpts.TargetLevel - character.Level) < 3)
                resistScale = 400.0f;
            else
                // This number is still being tested by many and may be slightly higher
                resistScale = 510.0f;

            return Math.Max(0.0f, (1.0f - (totalResist / (resistScale + totalResist))) * damageReduction);
        }

        public static float AvoidanceChance(Character character, Stats stats, HitResult avoidanceType)
        {
            CalculationOptionsProtWarr calcOpts = character.CalculationOptions as CalculationOptionsProtWarr;
            return StatConversion.GetDRAvoidanceChance(character, stats, avoidanceType, calcOpts.TargetLevel);
            
            /*
            float defSkill = (float)Math.Floor(stats.DefenseRating * ProtWarr.DefenseRatingToDefense);
            float baseAvoid = 0.0f;
            float modifiedAvoid = 0.0f;

            switch (avoidanceType)
            {
                case HitResult.Dodge:
                    baseAvoid = stats.Dodge + (stats.BaseAgility * ProtWarr.AgilityToDodge) - LevelModifier(character);
                    modifiedAvoid = ((stats.Agility - stats.BaseAgility) * ProtWarr.AgilityToDodge) +
                                        (stats.DodgeRating * ProtWarr.DodgeRatingToDodge) + (defSkill * ProtWarr.DefenseToDodge);
                    modifiedAvoid = 1.0f / (1.0f / 88.129021f + 0.9560f / modifiedAvoid);
                    break;
                case HitResult.Parry:
                    baseAvoid = stats.Parry - LevelModifier(character);
                    modifiedAvoid = (stats.ParryRating * ProtWarr.ParryRatingToParry) + (defSkill * ProtWarr.DefenseToParry);
                    modifiedAvoid = 1.0f / (1.0f / 47.003525f + 0.9560f / modifiedAvoid);
                    break;
                case HitResult.Miss:
                    baseAvoid = stats.Miss * 100f - LevelModifier(character);
                    modifiedAvoid = (defSkill * ProtWarr.DefenseToMiss);
                    modifiedAvoid = 1.0f / (1.0f / 16.0f + 0.9560f / modifiedAvoid);
                    break;
                case HitResult.Block:
                    // The 5% base block should be moved into stats.Block as a base value like the others
                    baseAvoid = 5.0f + stats.Block - LevelModifier(character);
                    modifiedAvoid = (stats.BlockRating * ProtWarr.BlockRatingToBlock) + (defSkill * ProtWarr.DefenseToBlock);
                    break;
                case HitResult.Crit:
                    modifiedAvoid = (defSkill * ProtWarr.DefenseToCritReduction) + (stats.Resilience * ProtWarr.ResilienceRatingToCritReduction);
                    break;
            }

            // Many of the base values are whole numbers, so need to get it back to decimal. 
            // May want to look at making this more consistant in the future.
            return (baseAvoid + modifiedAvoid) / 100.0f;
            */
        }

        public static bool IsAvoidable(Ability ability)
        {
            switch (ability)
            {
                case Ability.DamageShield:
                case Ability.DeepWounds:
                case Ability.Shockwave:
                case Ability.ThunderClap:
                    return false;
                default:
                    return true;
            }
        }

        public static string Name(Ability ability)
        {
            switch (ability)
            {
                case Ability.None: return "Swing";
                case Ability.Cleave: return "Cleave";
                case Ability.ConcussionBlow: return "Concussion Blow";
                case Ability.DamageShield: return "Damage Shield";
                case Ability.DeepWounds: return "Deep Wounds";
                case Ability.Devastate: return "Devastate";
                case Ability.HeroicStrike: return "Heroic Strike";
                case Ability.HeroicThrow: return "Heroic Throw";
                case Ability.Rend: return "Rend";
                case Ability.Revenge: return "Revenge";
                case Ability.ShieldBash: return "Shield Bash";
                case Ability.ShieldSlam: return "Shield Slam";
                case Ability.Shockwave: return "Shockwave";
                case Ability.Slam: return "Slam";
                case Ability.SunderArmor: return "Sunder Armor";
                case Ability.ThunderClap: return "Thunder Clap";
                default: return "";
            }
        }
    }
}