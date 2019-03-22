using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr
{
    // To be done:
    // Physical Combat:
    // float GetPhysicalMiss(int LevelDelta, bool DualWield)
    // float GetPhysicalDodge(int LevelDelta)
    // float GetPhysicalParry(int LevelDelta)
    // float GetPhysicalGlancing(int LevelDelta)
    // float[??] GetPhysicalCombatTable(int LevelDelta, float Hit, float Expertise, float Block)
    // float GetPhysicalMitigation(int TargetLevel)

/*
	public static class ArmorCalculations
	{
		//Added as part of the change to ArPen functionality; should integrate with StatConversion or something,
		//don't know exactly how, quite yet.
		public static float GetDamageReduction(int attackerLevel, float targetArmor, 
			float attackerArmorPenetration, float attackerArmorPenetrationRating)
		{
			float armorReductionPercent = (1f - attackerArmorPenetration) * (1f - attackerArmorPenetrationRating * 1.25f / 1539.529991f);
			float reducedArmor = (float)targetArmor * (armorReductionPercent);
			float damageReduction = (reducedArmor / ((467.5f * attackerLevel) + reducedArmor - 22167.5f));
			return damageReduction;
		}
	}
*/
    public static class StatConversion
    {   // Class only works for Level 80 Characters
        // Numbers reverse engineered by Whitetooth (hotdogee [at] gmail [dot] com)

        #region Character Constants

        public const float LEVEL_80_COMBATRATING_MODIFIER = 3.2789987789987789987789987789988f; // 82/52 * Math.Pow(131/63, ((80 - 70) / 10));
        public const float RATING_PER_ARMORPENETRATION = 1399.572614f; // 4.69512177f / (1.1/1.25f) * LEVEL_80_COMBATRATING_MODIFIER * 100f;
        public const float RATING_PER_BLOCK = 1639.499474f;
        public const float RATING_PER_DEFENSE = 4.918498039f; // 1.5f * LEVEL_80_COMBATRATING_MODIFIER;
        public const float RATING_PER_DODGE = 4525.018692f; // 12f * LEVEL_80_COMBATRATING_MODIFIER * 100f;
        public const float RATING_PER_EXPERTISE = 8.1974969f; // 2.5f * LEVEL_80_COMBATRATING_MODIFIER;
		public const float RATING_PER_PARRY = 4525.018692f; // 15f * LEVEL_80_COMBATRATING_MODIFIER * 100f;
        public const float RATING_PER_PHYSICALCRIT = 4590.598679f;
        public const float RATING_PER_PHYSICALHASTE = 3278.998947f;
        public const float RATING_PER_PHYSICALHIT = 3278.998947f;
        public const float RATING_PER_RESILIENCE = 9427.122498f; // 25f * LEVEL_80_COMBATRATING_MODIFIER * 100f;
        public const float RATING_PER_SPELLCRIT = 4590.598679f;
        public const float RATING_PER_SPELLHASTE = 3278.998947f;
        public const float RATING_PER_SPELLHIT = 2623.199272f;
        public const float RATING_PER_ARMOR = 2.00f; //2 Armor per 1 AGI;
        public const float RATING_PER_HEALTH = 10.00f; //10 Health per 1 STA;
        public const float RATING_PER_MANA = 15.00f; //15 Mana per 1 INT;
        public const float RATING_PER_DODGEPARRYREDUC = 0.0025f; //4 Exp per 1% Dodge/Parry Reduction;
        public const float DEFENSE_RATING_AVOIDANCE_MULTIPLIER = 0.04f;
        public const float BLOCKVALUE_PER_STR = 2.0f;

        // Attack Table for players attacking mobs                                        80       81        82      83
        public static readonly float[] WHITE_MISS_CHANCE_CAP            = new float[] { 0.0500f, 0.0520f, 0.0540f, 0.0800f };
        public static readonly float[] WHITE_MISS_CHANCE_CAP_DW         = new float[] { 0.2400f, 0.2420f, 0.2440f, 0.2700f }; //  WHITE_MISS_CHANCE_CAP + 19%
        //public static readonly float[] WHITE_MISS_CHANCE_CAP_BEHIND     = WHITE_MISS_CHANCE_CAP;
        //public static readonly float[] WHITE_MISS_CHANCE_CAP_DW_BEHIND  = WHITE_MISS_CHANCE_CAP_DW;
        public static readonly float[] YELLOW_MISS_CHANCE_CAP           = WHITE_MISS_CHANCE_CAP;
        //public static readonly float[] YELLOW_MISS_CHANCE_CAP_BEHIND    = WHITE_MISS_CHANCE_CAP_BEHIND;

        public static readonly float[] WHITE_DODGE_CHANCE_CAP           = new float[] { 0.0500f, 0.0550f, 0.0600f, 0.0650f }; //  6.5%
        //public static readonly float[] WHITE_DODGE_CHANCE_CAP_BEHIND    = WHITE_DODGE_CHANCE_CAP; // 6.5% Attacks from behind *can* be dodged
        public static readonly float[] YELLOW_DODGE_CHANCE_CAP          = WHITE_DODGE_CHANCE_CAP;
        //public static readonly float[] YELLOW_DODGE_CHANCE_CAP_BEHIND   = WHITE_DODGE_CHANCE_CAP_BEHIND;

        public static readonly float[] WHITE_PARRY_CHANCE_CAP           = new float[] { 0.0500f, 0.0550f, 0.0600f, 0.14f }; // 14%
        //public static readonly float[] WHITE_PARRY_CHANCE_CAP_BEHIND    = new float[] { 0.0000f, 0.0000f, 0.0000f, 0.0000f }; //  0% Attacks from behind can't be parried
        public static readonly float[] YELLOW_PARRY_CHANCE_CAP          = WHITE_PARRY_CHANCE_CAP;
        //public static readonly float[] YELLOW_PARRY_CHANCE_CAP_BEHIND   = WHITE_PARRY_CHANCE_CAP_BEHIND;

        public static readonly float[] WHITE_GLANCE_CHANCE_CAP          = new float[] { 0.1000f, 0.1500f, 0.2000f, 0.2400f }; // 25%
        //public static readonly float[] WHITE_GLANCE_CHANCE_CAP_BEHIND   = WHITE_GLANCE_CHANCE_CAP;
        public static readonly float[] YELLOW_GLANCE_CHANCE_CAP         = new float[] { 0.0000f, 0.0000f, 0.0000f, 0.0000f }; //  0% Yellows don't glance
        //public static readonly float[] YELLOW_GLANCE_CHANCE_CAP_BEHIND  = YELLOW_GLANCE_CHANCE_CAP;

        public static readonly float[] WHITE_BLOCK_CHANCE_CAP           = new float[] { 0.0500f, 0.0520f, 0.0540f, 0.0650f }; //  6.5%
        //public static readonly float[] WHITE_BLOCK_CHANCE_CAP_BEHIND    = new float[] { 0.0000f, 0.0000f, 0.0000f, 0.0000f }; //  0% Attacks from behind can't be blocked
        public static readonly float[] YELLOW_BLOCK_CHANCE_CAP          = WHITE_BLOCK_CHANCE_CAP;
        //public static readonly float[] YELLOW_BLOCK_CHANCE_CAP_BEHIND   = WHITE_BLOCK_CHANCE_CAP_BEHIND;

        /// <summary>
        /// You need to *add* this to your current crit value as it's a negative number.
        /// [80: 0, 81: -0.006, 82: -0.012, 83: -0.048]
        /// </summary>
        public static readonly float[] NPC_LEVEL_CRIT_MOD               = new float[] {-0.0000f,-0.0060f,-0.0120f,-0.0480f }; //  -4.8%

        public static readonly float[] NPC_ARMOR                        = new float[] { 9729f, 10034f, 10338f, 10643f };

        // Same for all classes
        public const float INT_PER_SPELLCRIT = 166.66667f;
        public const float REGEN_CONSTANT = 0.003345f;

        // Sigh, depends on class.
        public static float[] AGI_PER_PHYSICALCRIT = { 0.0f, // CharacterClass starts at 1
            62.50f,   // Warrior 1
            52.083f,  // Paladin 2
            83.333f,  // Hunter 3
            83.333f,  // Rogue 4
            52.083f,  // Priest 5
            62.50f,   // Death Knight 6
            83.333f,  // Shaman 7
            51.0204f, // Mage 8
            50.505f,  // Warlock 9
            0.0f,     // Empty 10
            83.333f,  // Druid 11
        };

        public static float[] AGI_PER_DODGE = { 0.0f, // Starts at 0
            84.74576271f, // Warrior 1
            59.88023952f, // Paladin 2
            86.20689655f, // Hunter 3
            47.84688995f, // Rogue 4
            59.88023952f, // Priest 5
            84.74576271f, // Death Knight 6
            59.88023952f, // Shaman 7
            58.82352941f, // Mage 8
            59.88023952f, // Warlock 9
            0.0f,         // Empty 10
            47.84688995f, // Druid 11
        };

        public static float[] DR_COEFFIENT = { 0.0f, // Starts at 0
            0.9560f, // Warrior 1
            0.9560f, // Paladin 2
            0.9880f, // Hunter 3
            0.9880f, // Rogue 4
            0.9530f, // Priest 5
            0.9560f, // Death Knight 6
            0.9880f, // Shaman 7
            0.9530f, // Mage 8
            0.9530f, // Warlock 9
            0.0f,    // Empty 10
            0.9720f, // Druid 11
        };

        // This is the cap value for DODGE PERCENTAGE.
        public static float[] CAP_DODGE = { 0.0f, // Starts at 0
            88.129021f,  // Warrior 1
            88.129021f,  // Paladin 2
            145.560408f, // Hunter 3
            145.560408f, // Rogue 4
            150.375940f, // Priest 5
            88.129021f,  // Death Knight 6
            145.560408f, // Shaman 7
            150.375940f, // Mage 8
            150.375940f, // Warlock 9
            0.0f,        // Empty 10
            116.890707f, // Druid 11
        };

        /// <summary>
        /// This is the 1/CAP_DODGE to cut down the ammount of math going on.
        /// </summary>
        public static float[] CAP_DODGE_INV = { 0.0f, // Starts at 0
            0.011347f, // Warrior 1
            0.011347f, // Paladin 2
            0.006870f, // Hunter 3
            0.006870f, // Rogue 4
            0.006650f, // Priest 5
            0.011347f, // Death Knight 6
            0.006870f, // Shaman 7
            0.006650f, // Mage 8
            0.006650f, // Warlock 9
            0.0f,      // Empty 10
            0.008555f, // Druid 11
        };

        // This is the cap value for PARRY PERCENTAGE.
        public static float[] CAP_PARRY = { 0.0f, // Starts at 0
            47.003525f,  // Warrior 1
            47.003525f,  // Paladin 2
            145.560408f, // Hunter 3
            145.560408f, // Rogue 4
            0f,          // Priest 5
            47.003525f,  // Death Knight 6
            145.560408f, // Shaman 7
            0f,          // Mage 8
            0f,          // Warlock 9
            0.0f,        // Empty 10
            0f,          // Druid 11
        };

        /// <summary>
        /// This is the 1/CAP_PARRY to cut down the amount of math going on.
        /// And prevent divide by 0 errors.
        /// </summary>
        public static float[] CAP_PARRY_INV = { 0.0f, // Starts at 0
            0.021275f, // Warrior 1
            0.021275f, // Paladin 2
            0.006870f, // Hunter 3
            0.006870f, // Rogue 4
            0f,        // Priest 5
            0.021275f, // Death Knight 6
            0.006870f, // Shaman 7
            0f,        // Mage 8
            0f,        // Warlock 9
            0.0f,      // Empty 10
            0f,        // Druid 11
        };

        //This is the cap value for MISS PERCENTAGE
        public static float[] CAP_MISSED = { 0.0f, // Starts at 0
            16f,  // Warrior 1
            16f,  // Paladin 2
            0f,   // Hunter 3
            0f,   // Rogue 4
            0f,   // Priest 5
            16f,  // Death Knight 6
            0f,   // Shaman 7
            0f,   // Mage 8
            0f,   // Warlock 9
            0.0f, // Empty 10
            0f,   // Druid 11
        };

        #endregion

        #region Functions for Plain Rating Conversions

        public static float GetArmorFromAgility(float Rating, CharacterClass Class) { return GetArmorFromAgility(Rating); }
        /// <summary>
        /// Returns a Value (2 = 2 extra Armor)
        /// </summary>
        /// <param name="Rating">Agility</param>
        /// <returns>A Value (2 = 2 extra Armor)</returns>
        public static float GetArmorFromAgility(float Rating) {
            return Rating * RATING_PER_ARMOR;
        }

        public static float GetHealthFromStamina(float Rating, CharacterClass Class) { return GetHealthFromStamina(Rating); }
        /// <summary>
        /// Returns a Value (1000 = 1000 extra Health)
        /// </summary>
        /// <param name="Rating">Stamina</param>
        /// <returns>A Value (1000 = 1000 extra Health)</returns>
        public static float GetHealthFromStamina(float Rating) {
            return Rating <= 20 ? Rating : (Rating - 20) * RATING_PER_HEALTH + 20; // first 20 stamina is 1 health
        }

        public static float GetManaFromIntellect(float Rating, CharacterClass Class) { return GetManaFromIntellect(Rating); }
        /// <summary>
        /// Returns a Value (1000 = 1000 extra Mana)
        /// </summary>
        /// <param name="Rating">Intellect</param>
        /// <returns>A Value (1000 = 1000 extra Mana)</returns>
        public static float GetManaFromIntellect(float Rating)
        {
            return Rating <= 20 ? Rating : (Rating - 20) * RATING_PER_MANA + 20; // first 20 intellect is 1 mana
        }

        public static float GetArmorPenetrationFromRating(float Rating, CharacterClass Class) { return GetArmorPenetrationFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% Armor Ignored)
        /// </summary>
        /// <param name="Rating">Armor Penetration Rating</param>
        /// <returns>A Percentage (0.05 = 5% Armor Ignored)</returns>
        public static float GetArmorPenetrationFromRating(float Rating) {
            return Rating / RATING_PER_ARMORPENETRATION;
        }
        public static float GetRatingFromArmorPenetration(float Rating, CharacterClass Class) { return GetRatingFromArmorPenetration(Rating); }
        /// <summary>
        /// Returns a Value (1 = 1 Armor Penetration Rating)
        /// </summary>
        /// <param name="Rating">Percent</param>
        /// <returns>A Value (1 = 1 Armor Penetration Rating)</returns>
        public static float GetRatingFromArmorPenetration(float Rating)
        {
            return Rating * RATING_PER_ARMORPENETRATION;
        }

        public static float GetBlockFromRating(float Rating, CharacterClass Class) { return GetBlockFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% added chance to Block)
        /// </summary>
        /// <param name="Rating">Block Rating</param>
        /// <returns>A Percentage (0.05 = 5% added chance to Block)</returns>
        public static float GetBlockFromRating(float Rating)
        {
            return Rating / RATING_PER_BLOCK;
        }

        public static float GetBlockValueFromStrength(float str, CharacterClass Class) { return GetBlockValueFromStrength(str); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% added chance to Block)
        /// </summary>
        /// <param name="Rating">Block Rating</param>
        /// <returns>A Percentage (0.05 = 5% added chance to Block)</returns>
        public static float GetBlockValueFromStrength(float str) { return str / BLOCKVALUE_PER_STR; }

        public static float GetDefenseFromRating(float Rating, CharacterClass Class) { return GetDefenseFromRating(Rating); }
        /// <summary>
        /// Returns a Value (5.4 = 5 extra Defense)
        /// </summary>
        /// <param name="Rating">Defense Rating</param>
        /// <returns>A Value (5.4 = 5 extra Defense)</returns>
        public static float GetDefenseFromRating(float Rating) { return (float)Math.Floor(Rating / RATING_PER_DEFENSE); }

        public static float GetDodgeFromRating(float Rating, CharacterClass Class) { return GetDodgeFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Dodge)
        /// </summary>
        /// <param name="Rating">Dodge Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra Dodge)</returns>
        public static float GetDodgeFromRating(float Rating)
        {
            return Rating / RATING_PER_DODGE;
        }

        public static float GetExpertiseFromRating(float Rating, CharacterClass Class) { return GetExpertiseFromRating(Rating); }
        /// <summary>
        /// Returns a Value (6.34 = 6.34 extra Expertise)
        /// </summary>
        /// <param name="Rating">Expertise Rating</param>
        /// <returns>A Value (6.34 = 6.34 extra Expertise)</returns>
        public static float GetExpertiseFromRating(float Rating) { return Rating / RATING_PER_EXPERTISE; }
        public static float GetRatingFromExpertise(float value, CharacterClass Class) { return GetRatingFromExpertise(value); }
        /// <summary>
        /// Returns a Value (6.34 = 6.34 extra Expertise)
        /// </summary>
        /// <param name="Rating">Expertise Rating</param>
        /// <returns>A Value (6.34 = 6.34 extra Expertise)</returns>
        public static float GetRatingFromExpertise(float value) { return value * RATING_PER_EXPERTISE; }

        public static float GetDodgeParryReducFromExpertise(float Rating, CharacterClass Class) { return GetDodgeParryReducFromExpertise(Rating); }
        /// <summary>
        /// Returns a Percentage (1.00 = 1% extra Dodge/Parry Reduction)
        /// </summary>
        /// <param name="Rating">Expertise</param>
        /// <returns>A Percentage (1.00 = 1% extra Dodge/Parry Reduction)</returns>
        public static float GetDodgeParryReducFromExpertise(float Rating) { return Rating * RATING_PER_DODGEPARRYREDUC; }
        public static float GetExpertiseFromDodgeParryReduc(float value, CharacterClass Class) { return GetExpertiseFromDodgeParryReduc(value); }
        /// <summary>
        /// Returns a Value (1 = 1 extra Expertise)
        /// </summary>
        /// <param name="Rating">DodgeParryReduc %</param>
        /// <returns>A Value (1 = 1 extra Expertise)</returns>
        public static float GetExpertiseFromDodgeParryReduc(float value) { return value / RATING_PER_DODGEPARRYREDUC; }

        public static float GetParryFromRating(float Rating, CharacterClass Class) { return GetParryFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Parry)
        /// </summary>
        /// <param name="Rating">Parry Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra Parry)</returns>
        public static float GetParryFromRating(float Rating) { return Rating / RATING_PER_PARRY; }

        public static float GetCritFromRating(float Rating, CharacterClass Class) { return GetPhysicalCritFromRating(Rating); }
        public static float GetCritFromRating(float Rating) { return GetPhysicalCritFromRating(Rating); }
        public static float GetPhysicalCritFromRating(float Rating, CharacterClass Class) { return GetPhysicalCritFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra chance to Crit)
        /// </summary>
        /// <param name="Rating">Crit Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra chance to Crit)</returns>
        public static float GetPhysicalCritFromRating(float Rating) { return Rating / RATING_PER_PHYSICALCRIT; }

        public static float GetRatingFromCrit(float Rating, CharacterClass Class) { return GetRatingFromPhysicalCrit(Rating); }
        public static float GetRatingFromCrit(float Rating) { return GetRatingFromPhysicalCrit(Rating); }
        public static float GetRatingFromPhysicalCrit(float Rating, CharacterClass Class) { return GetRatingFromPhysicalCrit(Rating); }
        /// <summary>
        /// Returns a Value (1 = 1 Crit Rating)
        /// </summary>
        /// <param name="Rating">Crit Percent</param>
        /// <returns>A Value (1 = 1 Crit Rating)</returns>
        public static float GetRatingFromPhysicalCrit(float Rating) { return Rating * RATING_PER_PHYSICALCRIT; }

        // Returns a Percentage
        public static float GetHasteFromRating(float Rating, CharacterClass Class) { return GetPhysicalHasteFromRating(Rating, Class); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Haste)
        /// </summary>
        /// <param name="Rating">Haste Rating</param>
        /// <param name="Class">CharacterClass</param>
        /// <returns>A Percentage (0.05 = 5% extra Haste)</returns>
        public static float GetPhysicalHasteFromRating(float Rating, CharacterClass Class) {
            if (Class == CharacterClass.DeathKnight
                || Class == CharacterClass.Druid
                || Class == CharacterClass.Paladin
                || Class == CharacterClass.Shaman)
                return Rating / RATING_PER_PHYSICALHASTE * 1.3f;    // Patch 3.1: Hybrids gain 30% more Phyiscal Haste from Haste Rating.
            return Rating / RATING_PER_PHYSICALHASTE;
        }

        public static float GetHitFromRating(float Rating, CharacterClass Class) { return GetPhysicalHitFromRating(Rating); }
        public static float GetHitFromRating(float Rating) { return GetPhysicalHitFromRating(Rating); }
        public static float GetPhysicalHitFromRating(float Rating, CharacterClass Class) { return GetPhysicalHitFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Hit)
        /// </summary>
        /// <param name="Rating">Hit Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra Hit)</returns>
        public static float GetPhysicalHitFromRating(float Rating){return Rating / RATING_PER_PHYSICALHIT;}
        public static float GetRatingFromHit(float value, CharacterClass Class) { return GetRatingFromHit(value); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Hit)
        /// </summary>
        /// <param name="Rating">Hit Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra Hit)</returns>
        public static float GetRatingFromHit(float value) { return value * RATING_PER_PHYSICALHIT; }

        // Returns a Percentage
        public static float GetCritReductionFromResilience(float Rating, CharacterClass Class) { return GetCritReductionFromResilience(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Resilience)
        /// </summary>
        /// <param name="Rating">Resilience Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra Resilience)</returns>
        public static float GetCritReductionFromResilience(float Rating)
        {
            return Rating / RATING_PER_RESILIENCE;
        }

        public static float GetSpellCritFromRating(float Rating, CharacterClass Class) { return GetSpellCritFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra chance to Crit)
        /// </summary>
        /// <param name="Rating">Crit Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra chance to Crit)</returns>
        public static float GetSpellCritFromRating(float Rating)
        {
            return Rating / RATING_PER_SPELLCRIT;
        }

        public static float GetSpellHasteFromRating(float Rating, CharacterClass Class) { return GetSpellHasteFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Haste)
        /// </summary>
        /// <param name="Rating">Haste Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra Haste)</returns>
        public static float GetSpellHasteFromRating(float Rating)
        {
            return Rating / RATING_PER_SPELLHASTE;
        }

        public static float GetSpellHitFromRating(float Rating, CharacterClass Class) { return GetSpellHitFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra chance to Hit)
        /// </summary>
        /// <param name="Rating">Hit Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra chance to Hit)</returns>
        public static float GetSpellHitFromRating(float Rating)
        {
            return Rating / RATING_PER_SPELLHIT;
        }

        public static float GetSpellCritFromIntellect(float Intellect, CharacterClass Class) { return GetSpellCritFromIntellect(Intellect); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra chance to Crit)
        /// </summary>
        /// <param name="Intellect">Intellect</param>
        /// <returns>A Percentage (0.05 = 5% extra chance to Crit)</returns>
        public static float GetSpellCritFromIntellect(float Intellect)
        {
            return Intellect / INT_PER_SPELLCRIT * 0.01f;
        }

        public static float GetCritFromAgility(float Agility, CharacterClass Class) { return GetPhysicalCritFromAgility(Agility, Class); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra chance to Crit)
        /// </summary>
        /// <param name="Agility">Agility</param>
        /// <param name="Class">CharacterClass</param>
        /// <returns>A Percentage (0.05 = 5% extra chance to Crit)</returns>
        public static float GetPhysicalCritFromAgility(float Agility, CharacterClass Class)
        {
            return Agility / AGI_PER_PHYSICALCRIT[(int)Class] * 0.01f;
        }

        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Dodge)
        /// </summary>
        /// <param name="Agility">Agility</param>
        /// <param name="Class">CharacterClass</param>
        /// <returns>A Percentage (0.05 = 5% extra Dodge)</returns>
        public static float GetDodgeFromAgility(float Agility, CharacterClass Class)
        {
            return Agility / AGI_PER_DODGE[(int)Class] * 0.01f;
        }

        public static float GetSpiritRegenSec(float Spirit, float Intellect, CharacterClass Class) { return GetSpiritRegenSec(Spirit, Intellect); }
        /// <summary>
        /// Returns a Number, How much mana is gained each Second. (Multiply by 5 to get MP5)
        /// </summary>
        /// <param name="Spirit">Spirit</param>
        /// <param name="Intellect">Intellect</param>
        /// <returns>A Number, How much mana is gained each Second. (Multiply by 5 to get MP5)</returns>
        public static float GetSpiritRegenSec(float Spirit, float Intellect)
        {
            return 0.001f + Spirit * REGEN_CONSTANT * (float)Math.Sqrt(Intellect);
        }

        public static float ApplyMultiplier(float baseValue, float multiplier)
        {
            return (baseValue * (1f + multiplier));
        }
        #endregion

        #region Functions for More complex things.

		// http://forums.worldofwarcraft.com/thread.html?topicId=16473618356&sid=1&pageNo=4 post 77.
		// Ghostcrawler vs theorycraft.
		/// <summary>
		/// Returns how much physical damage is reduced from Armor. (0.095 = 9.5% reduction)
		/// </summary>
		/// <param name="AttackerLevel">Level of Attacker</param>
		/// <param name="TargetLevel">Level of Target</param>
		/// <param name="TargetArmor">Armor of Target</param>
		/// <param name="ArmorIgnoreDebuffs">Armor reduction on target as result of Debuffs (Sunder/Fearie Fire) These are Multiplied.</param>
		/// <param name="ArmorIgnoreBuffs">Armor reduction buffs on player (Mace Spec, Battle Stance, etc) These are Added.</param>
		/// <param name="ArmorPenetrationRating">Penetration Rating (Can be added into ArmorIgnoreBuffs and then set this to 0)</param>
		/// <returns>How much physical damage is reduced from Armor. (0.095 = 9.5% reduction)</returns>
		public static float GetArmorDamageReduction(int AttackerLevel, float TargetArmor,
			float ArmorIgnoreDebuffs, float ArmorIgnoreBuffs, float ArmorPenetrationRating)
		{
			return GetArmorDamageReduction(AttackerLevel, 83, TargetArmor, ArmorIgnoreDebuffs, ArmorIgnoreBuffs, ArmorPenetrationRating);
		}

		// http://forums.worldofwarcraft.com/thread.html?topicId=16473618356&sid=1&pageNo=4 post 77.
		// Ghostcrawler vs theorycraft.
		/// <summary>
		/// Returns how much physical damage is reduced from Armor. (0.095 = 9.5% reduction)
		/// </summary>
		/// <param name="AttackerLevel">Level of Attacker</param>
		/// <param name="TargetLevel">Level of Target</param>
		/// <param name="TargetArmor">Armor of Target</param>
		/// <param name="ArmorIgnoreDebuffs">Armor reduction on target as result of Debuffs (Sunder/Fearie Fire) These are Multiplied.</param>
		/// <param name="ArmorIgnoreBuffs">Armor reduction buffs on player (Mace Spec, Battle Stance, etc) These are Added.</param>
		/// <param name="ArmorPenetrationRating">Penetration Rating (Can be added into ArmorIgnoreBuffs and then set this to 0)</param>
		/// <returns>How much physical damage is reduced from Armor. (0.095 = 9.5% reduction)</returns>
		public static float GetArmorDamageReduction(int AttackerLevel, int TargetLevel, float TargetArmor,
			float ArmorIgnoreDebuffs, float ArmorIgnoreBuffs, float ArmorPenetrationRating)
		{
			float ArmorConstant = 400 + 85 * TargetLevel + 4.5f * 85 * (TargetLevel - 59);
			TargetArmor *= (1f - ArmorIgnoreDebuffs);
			float ArPCap = Math.Min((TargetArmor + ArmorConstant) / 3f, TargetArmor);
			TargetArmor -= ArPCap * Math.Min(1f, GetArmorPenetrationFromRating(ArmorPenetrationRating) + ArmorIgnoreBuffs);

			return 1f - ArmorConstant / (ArmorConstant + TargetArmor);
		}

		/// <summary>
        /// Returns how much Armor Penetration rating gets you to a given Armor Reduction (inverse of GetArmorDamageReduction).
        /// For use with GrimToll-style procs
        /// </summary>
        /// <param name="AttackerLevel">Level of Attacker</param>
        /// <param name="TargetLevel">Level of Target</param>
        /// <param name="TargetArmor">Armor of Target</param>
        /// <param name="ArmorIgnoreDebuffs">Armor reduction on target as result of Debuffs (Sunder/Fearie Fire) -- These are Multiplied.</param>
        /// <param name="ArmorIgnoreBuffs">Armor reduction buffs on player (Mace Spec, Battle Stance, etc) -- These are Added.</param>
        /// <param name="TargetArmorReduction">How much Armor Reduction you are looking trying to find</param>
        /// <returns>How much Armor Penetration Rating will have GetArmorDamageReduction() return TargetArmorReduction</returns>
        public static float GetRatingFromArmorReduction(int AttackerLevel, float TargetArmor,
            float ArmorIgnoreDebuffs, float ArmorIgnoreBuffs, float TargetArmorReduction)
        {
            float ArmorConstant = 400 + 85 * AttackerLevel + 4.5f * 85 * (AttackerLevel - 59);
            TargetArmor *= (1f - ArmorIgnoreDebuffs);
            float ArPCap = Math.Min((TargetArmor + ArmorConstant) / 3f, TargetArmor);

            /*float ArpFromRating = (TargetArmor - ArPCap * ArmorIgnoreBuffs - TargetArmorReduction * ArmorConstant - TargetArmorReduction * TargetArmor + TargetArmor * ArPCap * ArmorIgnoreBuffs) /
                (ArPCap - TargetArmorReduction * ArPCap);*/
            float a = TargetArmorReduction, b = ArmorConstant, c = TargetArmor, d = ArPCap, e = ArmorIgnoreBuffs;

            /*
            TA2 = TA - ArPCap * (x + AIB)
            TAR = 1 - AC/(AC + TA2)
            TAR = 1 - AC/(AC+ TA - ArPCap * (x + AIB)) << base of formula

            a = 1 - b/(b+c-d*(x+e))
            b/(b+c-d*(x+e)) = (1 - a)
            b = (1 - a)(b+c-dx-de))
            0 = c-dx-de - ab - ac + adx + ade
            dx - adx = c - de - ab - ac + ade
            (d-ad)x = c - de - ab - ac + ade

            x = (c - de - ab - ac + ade) / (d-ad)   << gives loss of precision when dealing with some floats
              = (c*(1-a) + de(a-1) -ab) / (d(1-a))*/

            float ArpFromRating = (c * (1 - a) + d * e * (a - 1) - a * b) / (d * (1 - a));
            //if (/*first*/(  x  )/*br*/ - /*second*/(  (c * (1 - a) + d * e * (a - 1) - a * b) / (d * (1 - a))  ) <= 0.1)) return 0f;
            return (float)Math.Round(ArpFromRating * RATING_PER_ARMORPENETRATION,3);
        }

        /// <summary>
        /// Returns the chance to miss (0.09 = 9% chance to miss)
        /// </summary>
        /// <param name="LevelDelta">Attacker Level - Defender Level</param>
        /// <param name="bPvP">Set to True if Player vs Player combat</param>
        /// <returns>The chance to miss (0.09 = 9% chance to miss)</returns>
        public static float GetSpellMiss(int LevelDelta, bool bPvP)
        { //http://www.wow-dark-destiny.com/images/spellhit.png
            if (-LevelDelta <= 2)
                return (float)Math.Min(0.0f, - (LevelDelta + 4) * 0.01f);
            
            if (-LevelDelta > 2)
            //{
                if (bPvP)
                    return (float)Math.Min(0.62f, (-LevelDelta * 7 - 8) * 0.01f);
                //else
                    //break;
            //}
            return (float)Math.Min(0.94f, (-LevelDelta * 11 - 16) * 0.01f);
        }

        private static float AttackerResistancePenalty(int LevelDelta)
        {
            if (LevelDelta == 1)
                return 0f;
            else if (LevelDelta == 2)
                return 0f;
            else if (LevelDelta == 3)
                return 95f;
            return 0f;
        }

        /// <summary>
        /// Returns a Percent giving Average Magical Damage Resisted (0.16 = 16% Resisted)
        /// </summary>
        /// <param name="AttackerLevel">Level of the Attacker</param>
        /// <param name="TargetLevel">Level of the Target</param>
        /// <param name="TargetResistance">Targets Resistance</param>
        /// <param name="AttackerSpellPenetration">Attackers Spell Penetration</param>
        /// <returns>A Percent giving Average Magical Damage Resisted (0.16 = 16% Resisted)</returns>
        public static float GetAverageResistance(int AttackerLevel, int TargetLevel,
            float TargetResistance, float AttackerSpellPenetration)
        {
            float ActualResistance = (float)Math.Max(0f, TargetResistance - AttackerSpellPenetration);
            return ActualResistance / (AttackerLevel * 5f + AttackerResistancePenalty(AttackerLevel - TargetLevel) + ActualResistance) + 0.02f * (float)Math.Max(0, TargetLevel - AttackerLevel);
        }

        /// <summary>
        /// Returns a Table giving the chance to fall within a resistance slice cutoff.
        /// The table is float[11] Table, where Table[0] is 0% resisted, and Table[10] is 100% resisted.
        /// Each Table entry gives how much chance to roll into that slice.
        /// So if Table[1] contains 0.165, that means you have a 16.5% chance to resist 10% damage.
        /// </summary>
        /// <param name="AttackerLevel">Level of the Attacker</param>
        /// <param name="TargetLevel">Level of the Target</param>
        /// <param name="TargetResistance">Targets Resistance</param>
        /// <param name="AttackerSpellPenetration">Attackers Spell Penetration</param>
        /// <returns>A Table giving the chance to fall within a resistance slice cutoff.</returns>
        public static float[] GetResistanceTable(int AttackerLevel, int TargetLevel,
            float TargetResistance, float AttackerSpellPenetration)
        {                      //   00% 10% 20% 30% 40% 50% 60% 70% 80% 90% 100%
            float[] ResistTable = { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
            float AverageResistance = GetAverageResistance(AttackerLevel, TargetLevel, TargetResistance, AttackerSpellPenetration);

            for (int x = -1; x < 11; x++)
            {   // Build Table
                float ResistSlice = (float)Math.Max(0f, 0.5f - 2.5f * (float)Math.Abs(0.1f * x - AverageResistance));
                if (x == -1)
                {   // Adjust 0% and 10% for "negative" resists.
                    ResistTable[0] += 2f * ResistSlice;
                    ResistTable[1] -= 1f * ResistSlice;
                }
                else
                    ResistTable[x] += ResistSlice;
            }
            return ResistTable;
        }

        /// <summary>
        /// Returns a String version of the Table giving the chance to fall within a resistance slice cutoff.
        /// Useful as part of a tooltip
        /// </summary>
        /// <param name="AttackerLevel">Level of the Attacker</param>
        /// <param name="TargetLevel">Level of the Target</param>
        /// <param name="TargetResistance">Targets Resistance</param>
        /// <param name="AttackerSpellPenetration">Attackers Spell Penetration</param>
        /// <returns>A string version of a Table giving the chance to fall within a resistance slice cutoff.</returns>
        public static string GetResistanceTableString(int AttackerLevel, int TargetLevel,
            float TargetResistance, float AttackerSpellPenetration)
        {
            int count;
            string tipResist = string.Empty;
            tipResist = Math.Round(StatConversion.GetAverageResistance(AttackerLevel, TargetLevel, TargetResistance, AttackerSpellPenetration) * 100.0f, 2).ToString() + "% average resistance \n";
            tipResist += "% Resisted     Occurance";

            float[] ResistTable = StatConversion.GetResistanceTable(AttackerLevel, TargetLevel, TargetResistance, AttackerSpellPenetration);
            for (count = 0; count < 10; count++)
            {
                if (ResistTable[count] > 0)
                {
                    tipResist += "\n" + Math.Round(count * 10.0f, 1) + " % resisted   " + Math.Round(ResistTable[count] * 100.0f, 2) + "%";
                }
            }

            return tipResist;
        }

        /// <summary>
        /// Returns the Minimum amount of Spell Damage will be resisted. (0.2 = Anything below 20% is always resisted)
        /// If this returns 0.0, that means you will always have a chance to take full damage from spells.
        /// If this returns 0.1, that means you will never take full damage from spells, and minumum you will take is 10% reduction.
        /// </summary>
        /// <param name="AttackerLevel">Level of Attacker</param>
        /// <param name="TargetLevel">Level of Target</param>
        /// <param name="TargetResistance">Target Resistance</param>
        /// <param name="AttackerSpellPenetration">Attacker Spell Penetration</param>
        /// <returns>The Minimum amount of Spell Damage will be resisted. (0.2 = Anything below 20% is always resisted)</returns>
        public static float GetMinimumResistance(int AttackerLevel, int TargetLevel,
            float TargetResistance, float AttackerSpellPenetration)
        {
            float[] ResistTable = GetResistanceTable(AttackerLevel, TargetLevel, TargetResistance, AttackerSpellPenetration);

            for (int x = 0; x < 11; x++)
                if (ResistTable[x] > 0f)
                    return 0.1f * x;
            return 0f;
        }

        // Initial function taken from the ProtWarrior Module.
        // Then using table found on EJ:
        // http://elitistjerks.com/f31/t29453-combat_ratings_level_80_a/
        // creating updated Avoidance Chance w/ DR build in formula.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="character">Character in question.</param>
        /// <param name="stats">Stats object... total stats of the character.</param>
        /// <param name="avoidanceType">What type of hit is the target doing on the character?</param>
        /// <param name="TargetLevel">Level of the target being fought</param>
        /// <returns>A % value where .50 == 50%</returns>
        public static float GetDRAvoidanceChance(Character character, Stats stats, HitResult avoidanceType, int TargetLevel) { return GetDRAvoidanceChance(character, stats, avoidanceType, (uint)TargetLevel); }
        public static float GetDRAvoidanceChance(Character character, Stats stats, HitResult avoidanceType, uint TargetLevel)
        {
            float defSkill = stats.Defense;
            // Let's make sure we don't run off the bottom w/ a negative defense rating.
            stats.DefenseRating = Math.Max(stats.DefenseRating, 0f);
            float defSkillMod = (GetDefenseFromRating(stats.DefenseRating, character.Class) * DEFENSE_RATING_AVOIDANCE_MULTIPLIER);
            float baseAvoid = (defSkill - (TargetLevel * 5)) * DEFENSE_RATING_AVOIDANCE_MULTIPLIER;
            float modifiedAvoid = defSkillMod;
            float finalAvoid = 0f; // I know it breaks my lack of redundancy rule, but it helps w/ readability.
            int iClass = (int)character.Class;

            switch (avoidanceType)
            {
                case HitResult.Dodge:
                    baseAvoid += ((stats.Dodge + GetDodgeFromAgility(stats.BaseAgility, character.Class)) * 100f);
                    // Assuring we don't run off the bottom w/ negative dodge rating.
                    stats.DodgeRating = Math.Max(stats.DodgeRating, 0f);
                    modifiedAvoid += ((GetDodgeFromAgility((stats.Agility - stats.BaseAgility), character.Class) +
                                    GetDodgeFromRating(stats.DodgeRating)) * 100f);
                    modifiedAvoid = DRMath(CAP_DODGE_INV[iClass], DR_COEFFIENT[iClass], modifiedAvoid);
                    // Don't run off the bottom if we have negative dodge
                    finalAvoid = Math.Max(baseAvoid + modifiedAvoid, 0);
                    finalAvoid = Math.Min(finalAvoid, CAP_DODGE[iClass]);
                    break;
                case HitResult.Parry:
                    baseAvoid += stats.Parry * 100f;
                    // Assuring we don't run off the bottom w/ negative parry rating.
                    stats.ParryRating = Math.Max(stats.ParryRating, 0f);
                    modifiedAvoid += (GetParryFromRating(stats.ParryRating) * 100f);
                    modifiedAvoid = DRMath(CAP_PARRY_INV[iClass], DR_COEFFIENT[iClass], modifiedAvoid);
                    finalAvoid = (baseAvoid + modifiedAvoid);
                    finalAvoid = Math.Min(finalAvoid, CAP_PARRY[iClass]);
                    break;
                case HitResult.Miss:
                    // Base Miss rate according is 5%
                    // However, this can be talented up (e.g. Frigid Dreadplate, NE racial, etc.) 
                    baseAvoid += stats.Miss * 100f;
                    modifiedAvoid = DRMath( (1f/CAP_MISSED[iClass]), DR_COEFFIENT[iClass], modifiedAvoid );
                    // Factoring in the Miss Cap. 
                    modifiedAvoid = Math.Min(CAP_MISSED[iClass], modifiedAvoid);
                    finalAvoid = (baseAvoid + modifiedAvoid);
                    finalAvoid = Math.Min(finalAvoid, CAP_MISSED[iClass]);
                    break;
                case HitResult.Block:
                    // Base Block is 5%
                    baseAvoid += stats.Block * 100f;
                    // Assuring we don't run off the bottom w/ negative block rating.
                    stats.BlockRating = Math.Max(stats.BlockRating, 0f);

                    modifiedAvoid += (GetBlockFromRating(stats.BlockRating) * 100f);
                    finalAvoid = (baseAvoid + modifiedAvoid);
                    break;
                case HitResult.Crit:
                    modifiedAvoid += (GetCritReductionFromResilience(stats.Resilience) * 100f);
                    finalAvoid = (baseAvoid + modifiedAvoid);
                    break;
            }

            // Many of the base values are whole numbers, so need to get it back to decimal. 
            // May want to look at making this more consistant in the future.
            finalAvoid = finalAvoid / 100.0f;
            return finalAvoid;
        }
        /// <summary>
        /// StatPostDR =  1/(CAP_STAT_INV + COEF/StatPreDR)
        /// </summary>
        /// <param name="inv_cap">One of the CAP_STAT_INV values, appropriate for the class.</param>
        /// <param name="coefficient">One of the DR_COEF values, appropriate for the class.</param>
        /// <param name="valuePreDR">The value of the stat before DR are factored in.</param>
        /// <returns></returns>
        private static float DRMath(float inv_cap, float coefficient, float valuePreDR)
        {
            float DRValue = 0f;
            DRValue = 1f / (inv_cap + coefficient / valuePreDR);
            return DRValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="stats">Be sure to pass in the total character's stats.  Not just his gear.</param>
        /// <param name="TargetLevel">Level of the mob to be tanked.  Usually 82 or 83.</param>
        /// <returns></returns>
        public static float GetDefenseRatingNeeded(Character character, Stats stats, int TargetLevel) { return GetDefenseRatingNeeded(character, stats, (uint)TargetLevel); }
        public static float GetDefenseRatingNeeded(Character character, Stats stats, uint TargetLevel)
        {
            // Should be 689(DefRating) - stats.DefenseRating. vs. level 83 mob.
            // And/or 459.2(Resilience) - stats.ResilienceRating
            float fAvoidance = GetDRAvoidanceChance(character, stats, HitResult.Crit, TargetLevel);
            float critChance = 5.0f - (fAvoidance * 100);
            return (critChance / DEFENSE_RATING_AVOIDANCE_MULTIPLIER) * RATING_PER_DEFENSE;
        }

        // Not yet completed.
        public class CombatTable
        {
            public float Miss;
            public float Dodge;
            public float Parry;
            public float Glancing;
            public float Block;
            public float Crit;
            //public float Crush;
            public float Hit;

            public bool bPlayerTarget;
            public bool bBehind;

            public float Evasion()
            {
                return Miss + (bBehind ? (bPlayerTarget ? 0f : Dodge) : (Dodge + Parry));
            }

            public CombatTable()
            {
                Miss = Dodge = Parry = Glancing = Block = Crit = Hit = 0f;
                bPlayerTarget = bBehind = false;
            }
        }

        // http://elitistjerks.com/f31/t37032-faq_working_theories_raiding_level_80_a/

        /// <summary>
        /// Returns a Combat Table for an attack
        /// </summary>
        /// <param name="AttackerLevel">Your Level (80)</param>
        /// <param name="TargetLevel">The Mob's Level (80-83)</param>
        /// <param name="bDualWield">True = You are Dual Wielding</param>
        /// <param name="bWhiteorYellow">True = Yellow Attack</param>
        /// <param name="PercBehind">The percentage of time over Duration of Fight you are standing behind the mob. 0.50 = 50% = 3 min of 6 min fight</param>
        /// <param name="HitBonus">Your total bonus Hit Percentage</param>
        /// <param name="ExpertiseBonus">Your total bonus Dodge/Parry Reduction Percentage from Expertise</param>
        /// <param name="CritChance">Your total Crit chance Percentage</param>
        /// <returns></returns>
        /// <remarks>
        /// Need to add things that would affect the combat table outside Hit/Exp such as Talents and special gear bonuses
        /// Need to add handling for abilities that cannot be Dodged/Blocked/Parried (Warrior Overpower for example)
        /// </remarks>
        public static CombatTable GetCombatTablePlayer_vs_NPC(int AttackerLevel, int TargetLevel,
            bool bDualWield, bool bWhiteorYellow, float PercBehind, float HitBonus, float ExpertiseBonus, float CritChance)
        {
            CombatTable ct = new CombatTable();
            
            int DeltaLevel = TargetLevel - AttackerLevel;

            // Set the Baselines Up
            ct.Miss = bDualWield ?
                    (bWhiteorYellow ? YELLOW_MISS_CHANCE_CAP[DeltaLevel] : WHITE_MISS_CHANCE_CAP_DW[DeltaLevel])
                :
                    (bWhiteorYellow ? YELLOW_MISS_CHANCE_CAP[DeltaLevel] : WHITE_MISS_CHANCE_CAP[DeltaLevel]);

            ct.Glancing = bWhiteorYellow ? YELLOW_GLANCE_CHANCE_CAP[DeltaLevel] : WHITE_GLANCE_CHANCE_CAP[DeltaLevel];
            ct.Dodge    = bWhiteorYellow ? YELLOW_DODGE_CHANCE_CAP[DeltaLevel] : WHITE_DODGE_CHANCE_CAP[DeltaLevel];
            ct.Parry    = bWhiteorYellow ? YELLOW_PARRY_CHANCE_CAP[DeltaLevel] : WHITE_PARRY_CHANCE_CAP[DeltaLevel];
            ct.Block    = bWhiteorYellow ? YELLOW_BLOCK_CHANCE_CAP[DeltaLevel] : WHITE_BLOCK_CHANCE_CAP[DeltaLevel];
            ct.Crit     = NPC_LEVEL_CRIT_MOD[DeltaLevel];

            // Modify those baselines with your character's attributes
            ct.Miss     -= HitBonus;
            ct.Glancing -= 0f; // can't reduce glance with a stat
            ct.Dodge    -= ExpertiseBonus;
            ct.Parry    -= ExpertiseBonus;
            ct.Block    -= 0f; // can't reduce block with a stat
            ct.Crit     += CritChance;

            // Modify this one more time based upon situtations
            ct.Miss     -= 0f; // No mods
            ct.Glancing -= 0f; // No mods
            ct.Dodge    -= 0f; // No mods
            ct.Parry    *= PercBehind; // Can be reduced by standing behind the mob
            ct.Block    *= PercBehind; // Can be reduced by standing behind the mob
            ct.Crit     -= 0f; // No mods

            return ct;
        }
        // */

        #endregion
    }
}