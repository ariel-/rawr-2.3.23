using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.ProtWarr
{
    public static class Lookup
    {
        public static float GlobalCooldownSpeed(Character character, bool withLatency)
        {
            float globalCooldownSpeed = 1.5f;
            
            if (character.WarriorTalents.UnrelentingAssault == 2)
                globalCooldownSpeed -= 0.5f;

            if (withLatency)
                globalCooldownSpeed += 0.1f;

            return globalCooldownSpeed;
        }

        public static float LevelModifier(Character character, int targetLevel)
        {
            return (targetLevel - character.Level) * 0.2f;
        }

        public static float TargetArmorReduction(Character character, Stats stats, int targetArmor)
        {
            float ignoreArmor = 0.0f;
            if (character.MainHand != null && (character.MainHand.Type == ItemType.OneHandMace))
                ignoreArmor += character.WarriorTalents.MaceSpecialization * 0.03f;

			return StatConversion.GetArmorDamageReduction(character.Level, targetArmor, stats.ArmorPenetration, ignoreArmor, stats.ArmorPenetrationRating);
        }

        public static float TargetCritChance(Character character, Stats stats, int targetLevel)
        {
            return Math.Max(0.0f, 0.05f - AvoidanceChance(character, stats, HitResult.Crit, targetLevel));
        }

        public static float TargetAvoidanceChance(Character character, Stats stats, HitResult avoidanceType, int targetLevel)
        {
            switch (avoidanceType)
            {
                case HitResult.Miss:
                    return StatConversion.WHITE_MISS_CHANCE_CAP[targetLevel - 80];
                case HitResult.Dodge:
                    return StatConversion.YELLOW_DODGE_CHANCE_CAP[targetLevel - 80] - (0.01f * character.WarriorTalents.WeaponMastery);
                case HitResult.Parry:
                    return StatConversion.YELLOW_PARRY_CHANCE_CAP[targetLevel - 80];
                case HitResult.Glance:
                    return 0.06f + ((targetLevel - character.Level) * 0.06f);
                case HitResult.Crit:
                    return -StatConversion.NPC_LEVEL_CRIT_MOD[targetLevel - character.Level]; // StatConversion returns as negative
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
        }

        public static float BonusExpertisePercentage(Character character, Stats stats)
        {
            return StatConversion.GetDodgeParryReducFromExpertise(stats.Expertise + 
                StatConversion.GetExpertiseFromRating(stats.ExpertiseRating), CharacterClass.Warrior);
        }

        public static float BonusHastePercentage(Character character, Stats stats)
        {
            return StatConversion.GetPhysicalHasteFromRating(stats.HasteRating, CharacterClass.Warrior) + stats.PhysicalHaste;
        }

        public static float BonusHitPercentage(Character character, Stats stats)
        {
            return StatConversion.GetPhysicalHitFromRating(stats.HitRating, CharacterClass.Warrior) + stats.PhysicalHit;
        }

        public static float BonusCritMultiplier(Character character, Stats stats, Ability ability)
        {
            if (ability == Ability.None)
                return (2.0f * (1.0f + stats.BonusCritMultiplier) - 1.0f);
            else
                return (2.0f * (1.0f + stats.BonusCritMultiplier) - 1.0f) * (1.0f + character.WarriorTalents.Impale * 0.1f);
        }

        public static float BonusCritPercentage(Character character, Stats stats, int targetLevel)
        {
            return Math.Max(0.0f, Math.Min(1.0f, 
                StatConversion.GetPhysicalCritFromRating(stats.CritRating, CharacterClass.Warrior) + 
                StatConversion.GetPhysicalCritFromAgility(stats.Agility, CharacterClass.Warrior) +
                stats.PhysicalCrit));
        }

        public static float BonusCritPercentage(Character character, Stats stats, Ability ability, int targetLevel)
        {
            // Grab base melee crit chance before adding ability-specific crit chance
            float abilityCritChance = BonusCritPercentage(character, stats, targetLevel);

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
                case Ability.DamageShield:
                case Ability.DeepWounds:
                case Ability.Rend:
                case Ability.ShieldBash:
                case Ability.SunderArmor:
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

        public static float GlancingReduction(Character character, int targetLevel)
        {
            return (Math.Min(0.91f, 1.3f - (0.05f * (targetLevel - character.Level) * 5.0f)) +
                    Math.Max(0.99f, 1.2f - (0.03f * (targetLevel - character.Level) * 5.0f))) / 2;
        }

        public static float ArmorReduction(Character character, Stats stats, int targetLevel)
        {
            return StatConversion.GetArmorDamageReduction(targetLevel, stats.Armor, 0.0f, 0.0f, 0.0f);
        }

        public static float MagicReduction(Character character, Stats stats, DamageType school, int targetLevel)
        {
            float totalResist = 0.0f;
            switch (school)
            {
                case DamageType.Arcane: totalResist = stats.ArcaneResistance; break;
                case DamageType.Fire: totalResist = stats.FireResistance; break;
                case DamageType.Frost: totalResist = stats.FrostResistance; break;
                case DamageType.Nature: totalResist = stats.NatureResistance; break;
                case DamageType.Shadow: totalResist = stats.ShadowResistance; break;
            }

            float damageReduction = Lookup.StanceDamageReduction(character, stats, school);
            float averageResistance = StatConversion.GetAverageResistance(targetLevel, character.Level, totalResist, 0.0f);

            return Math.Max(0.0f, (1.0f - averageResistance) * damageReduction);
        }

        public static float AvoidanceChance(Character character, Stats stats, HitResult avoidanceType, int targetLevel)
        {
            if(avoidanceType == HitResult.Crit)
                return StatConversion.GetDRAvoidanceChance(character, stats, avoidanceType, targetLevel);
            else
                return Math.Max(0.0f, StatConversion.GetDRAvoidanceChance(character, stats, avoidanceType, targetLevel));
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

        public static bool IsWeaponAttack(Ability ability)
        {
            switch (ability)
            {
                case Ability.None:
                case Ability.Cleave:
                case Ability.ConcussionBlow:
                case Ability.Devastate:
                case Ability.HeroicStrike:
                case Ability.MockingBlow:
                case Ability.Revenge:
                case Ability.ShieldSlam:
                case Ability.Shockwave:
                case Ability.Slam:
                    return true;
                default:
                    return false;
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
                case Ability.MockingBlow: return "Mocking Blow";
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