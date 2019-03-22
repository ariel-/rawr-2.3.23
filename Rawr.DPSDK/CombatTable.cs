using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.DPSDK
{
    public class CombatTable
    {
        public Character character;
        public CharacterCalculationsDPSDK calcs;
        public DeathKnightTalents talents;
        public Stats stats;
        public CalculationOptionsDPSDK calcOpts;

        public Weapon MH, OH;
        public bool DW;

        public float combinedSwingTime;

        public float physCrits, hitBonus, 
            missedSpecial, dodgedSpecial, 
            spellCrits, spellResist, 
            totalMHMiss, totalOHMiss,
            realDuration, totalMeleeAbilities,
            whiteMiss,
        totalSpellAbilities, normalizationFactor, physicalMitigation;

        public Item additionalItem;

        public CombatTable(Character c, Stats stats, CalculationOptionsDPSDK calcOpts) :
            this(c, new CharacterCalculationsDPSDK(), stats, calcOpts)
        {
        }

        public CombatTable(Character c, CharacterCalculationsDPSDK calcs, Stats stats, CalculationOptionsDPSDK calcOpts/*, Item additionalItem*/)
        {
            character = c;
            this.calcs = calcs;
            talents = character.DeathKnightTalents;
            this.calcOpts = calcOpts;
            this.stats = stats;
            this.additionalItem = null;
            totalMeleeAbilities = 0f;
            totalSpellAbilities = 0f;


/*#if RAWR3
            if (calcOpts.rotation == null)
            {
                calcOpts.rotation = new Rotation();
                calcOpts.rotation.setRotation(Rotation.Type.Blood);
            }
#endif*/
            float DWExtraHits = 1f;// +talents.ThreatOfThassarian / 3f;
            totalMeleeAbilities =(float)( calcOpts.rotation.PlagueStrike * DWExtraHits + calcOpts.rotation.ScourgeStrike +
                calcOpts.rotation.Obliterate * DWExtraHits + calcOpts.rotation.BloodStrike * DWExtraHits + calcOpts.rotation.HeartStrike +
                calcOpts.rotation.FrostStrike * DWExtraHits + calcOpts.rotation.DeathStrike * DWExtraHits);
            
            totalSpellAbilities = (float)(calcOpts.rotation.DeathCoil + calcOpts.rotation.IcyTouch + calcOpts.rotation.HowlingBlast + calcOpts.rotation.Pestilence + calcOpts.rotation.Horn + calcOpts.rotation.GhoulFrenzy);

            hitBonus = .01f * (float)talents.NervesOfColdSteel;
            Weapons();
            CritsAndResists();
        }

        public void CritsAndResists()
        {
            #region Crits, Resists
            {
                // Attack Rolltable (DW):
                // 27.0% miss     (8.0% with 2H)
                //  6.5% dodge
                // 24.0% glancing (75% hit-dmg)
                // xx.x% crit
                // remaining = hit

                float targetArmor = calcOpts.BossArmor, totalArP = stats.ArmorPenetration;

                float arpBuffs = talents.BloodGorged * 2f / 100;

                physicalMitigation = 1f - StatConversion.GetArmorDamageReduction(character.Level, targetArmor,
                    stats.ArmorPenetration, arpBuffs, stats.ArmorPenetrationRating);

                calcs.EnemyMitigation = 1f - physicalMitigation;
                calcs.EffectiveArmor = physicalMitigation;

                // Crit: Base .65%
                physCrits = .0065f;
                physCrits += StatConversion.GetPhysicalCritFromRating(stats.CritRating);
                physCrits += StatConversion.GetPhysicalCritFromAgility(stats.Agility, CharacterClass.DeathKnight);
                physCrits += .01f * (float)(talents.DarkConviction + talents.EbonPlaguebringer + talents.Annihilation);
                physCrits += stats.PhysicalCrit;
                calcs.CritChance = physCrits;

                float chanceAvoided = 0.335f;

                float chanceDodged = StatConversion.WHITE_DODGE_CHANCE_CAP[calcOpts.TargetLevel-80];

                calcs.DodgedMHAttacks = MH.chanceDodged;
                calcs.DodgedOHAttacks = OH.chanceDodged;

                if (character.MainHand != null) { chanceDodged = MH.chanceDodged; }

                if (character.OffHand != null) {
                    if (DW) 
                    {
                        chanceDodged += OH.chanceDodged;
                        chanceDodged /= 2;
                    } 
                    else if (character.MainHand == null ) 
                    {
                        chanceDodged = OH.chanceDodged;
                    }
                }

                calcs.DodgedAttacks = chanceDodged;
                // Process White hits:
                float chanceMiss = DW ? StatConversion.WHITE_MISS_CHANCE_CAP_DW[calcOpts.TargetLevel - 80] : StatConversion.WHITE_MISS_CHANCE_CAP[calcOpts.TargetLevel - 80];
                chanceMiss -= StatConversion.GetPhysicalHitFromRating(stats.HitRating);
                chanceMiss -= hitBonus;
                chanceMiss -= stats.PhysicalHit;
                // Cap the Miss rate at 0%
                chanceMiss = Math.Max(chanceMiss, 0f);
                calcs.MissedAttacks = chanceMiss;
                whiteMiss = chanceMiss;
                chanceAvoided = chanceDodged + chanceMiss;
                calcs.AvoidedAttacks = chanceDodged + chanceMiss;

                // Process Yellow hits
                chanceMiss = StatConversion.YELLOW_MISS_CHANCE_CAP[calcOpts.TargetLevel - 80];
                chanceMiss -= StatConversion.GetPhysicalHitFromRating(stats.HitRating);
                chanceMiss -= hitBonus;
                chanceMiss -= stats.PhysicalHit;
                chanceMiss = Math.Max(chanceMiss, 0f);
                chanceDodged = MH.chanceDodged;
                missedSpecial = chanceMiss;
                dodgedSpecial = chanceDodged;
                // calcs.MissedAttacks = chanceMiss           

                spellCrits = 0f;
                spellCrits += StatConversion.GetSpellCritFromRating(stats.CritRating);
                spellCrits += stats.SpellCrit + stats.SpellCritOnTarget;
                spellCrits += .01f * (float)(talents.DarkConviction + talents.EbonPlaguebringer);
                calcs.SpellCritChance = spellCrits;

                // Resists: Base 17%
                spellResist = .17f;
                spellResist -= StatConversion.GetSpellHitFromRating(stats.HitRating);
                spellResist -= .01f * talents.Virulence;
                spellResist -= stats.SpellHit;
                if (spellResist < 0f) spellResist = 0f;

                // Total physical misses
                totalMHMiss = calcs.DodgedMHAttacks + whiteMiss;
                totalOHMiss = calcs.DodgedOHAttacks + whiteMiss;
                double spellGCD = (calcOpts.rotation.presence == CalculationOptionsDPSDK.Presence.Blood ? 1.5d / ((1 + (StatConversion.GetHasteFromRating(stats.HasteRating, CharacterClass.DeathKnight))) * (1d + stats.SpellHaste)) < 1d ? 1d : 1.5d / ((1 + (StatConversion.GetHasteFromRating(stats.HasteRating, CharacterClass.DeathKnight))) * (1d + stats.SpellHaste)): 1d);
                double physicalGCD = (calcOpts.rotation.presence == CalculationOptionsDPSDK.Presence.Blood ? 1.5d : 1d);
                float minDuration = totalMeleeAbilities * (float) physicalGCD +
                    totalSpellAbilities * (float) spellGCD;

                realDuration = (float)Math.Max(minDuration, calcOpts.rotation.CurRotationDuration);

                float dodgeMissPerRotation = (float)(totalMeleeAbilities - calcOpts.rotation.FrostStrike);
                chanceAvoided = chanceDodged + chanceMiss;
                double GChanceAvoided = (1 / (1 - chanceAvoided)) - 1;
                double GSpellResist = (1 / (1 - spellResist)) - 1;
                double ProbableGCDLossPerRotation = dodgeMissPerRotation * physicalGCD * GChanceAvoided +
                                                    (calcOpts.rotation.IcyTouch + calcOpts.rotation.Pestilence) * spellGCD * GSpellResist;

                realDuration += (float)(((minDuration + ProbableGCDLossPerRotation) / realDuration < 1 ? (minDuration + ProbableGCDLossPerRotation) / realDuration : 1) * ProbableGCDLossPerRotation);
                // This last line is a bit hackish, but basically the extra GCD is more inconvenient the closer we are to having a GCD-capped rotation; once we're GCD-capped, they cost the full value.
                }
            #endregion
        }

        public void Weapons(){

            float MHExpertise = stats.Expertise;
            float OHExpertise = stats.Expertise;

            if (character.Race == CharacterRace.Dwarf)
            {
                if (character.MainHand != null &&
                    (character.MainHand.Item.Type == ItemType.OneHandMace ||
                     character.MainHand.Item.Type == ItemType.TwoHandMace))
                {
                    MHExpertise += 5f;
                }

                if (character.OffHand != null && character.OffHand.Item.Type == ItemType.OneHandMace)
                {
                    OHExpertise += 5f;
                }
            }
            else if (character.Race == CharacterRace.Orc)
            {
                if (character.MainHand != null &&
                    (character.MainHand.Item.Type == ItemType.OneHandAxe ||
                     character.MainHand.Item.Type == ItemType.TwoHandAxe))
                {
                    MHExpertise += 5f;
                }

                if (character.OffHand != null && character.OffHand.Item.Type == ItemType.OneHandAxe)
                {
                    OHExpertise += 5f;
                }
            }
            if (character.Race == CharacterRace.Human)
            {
                if (character.MainHand != null &&
                    (character.MainHand.Item.Type == ItemType.OneHandSword ||
                     character.MainHand.Item.Type == ItemType.TwoHandSword ||
                     character.MainHand.Item.Type == ItemType.OneHandMace ||
                     character.MainHand.Item.Type == ItemType.TwoHandMace))
                {
                    MHExpertise += 3f;
                }

                if (character.OffHand != null &&
                    (character.OffHand.Item.Type == ItemType.OneHandSword ||
                    character.OffHand.Item.Type == ItemType.OneHandMace))
                {
                    OHExpertise += 3f;
                }
            }


            MH = new Weapon(null, stats, calcOpts, talents, 0f);
            OH = new Weapon(null, null, null, talents, 0f);

            DW = character.MainHand != null && character.OffHand != null &&
                character.MainHand.Slot != ItemSlot.TwoHand;

            if (character.MainHand != null)
            {
                MH = new Weapon(character.MainHand.Item, stats, calcOpts, talents, MHExpertise);
                calcs.MHAttackSpeed = MH.hastedSpeed;
                calcs.MHWeaponDamage = MH.damage;
                calcs.MHExpertise = MH.effectiveExpertise;
            }

            if (character.OffHand != null && DW)
            {
                OH = new Weapon(character.OffHand.Item, stats, calcOpts, talents, OHExpertise);

               // float OHMult = .05f * (float)talents.NervesOfColdSteel;
               // OH.damage *= .5f + OHMult;

                calcs.OHAttackSpeed = OH.hastedSpeed;
                calcs.OHWeaponDamage = OH.damage;
                calcs.OHExpertise = OH.effectiveExpertise;
            }

            if (additionalItem != null && ((additionalItem.Slot == ItemSlot.MainHand) || (additionalItem.Slot == ItemSlot.TwoHand) || additionalItem.Slot == ItemSlot.OneHand))
            {
                MH = new Weapon(additionalItem, stats, calcOpts, talents, MHExpertise);
                calcs.MHAttackSpeed = MH.hastedSpeed;
                calcs.MHWeaponDamage = MH.damage;
                calcs.MHExpertise = MH.effectiveExpertise;
            }
            else if (additionalItem != null && (additionalItem.Slot == ItemSlot.OffHand || additionalItem.Slot == ItemSlot.OneHand))
            {
                OH = new Weapon(additionalItem, stats, calcOpts, talents, OHExpertise);
                calcs.OHAttackSpeed = OH.hastedSpeed;
                calcs.OHWeaponDamage = OH.damage;
                calcs.OHExpertise = OH.effectiveExpertise;
            }


            // MH-only
            if ((character.MainHand != null) && (! DW))
            {
                if (character.MainHand.Item.Type == ItemType.TwoHandAxe
                    || character.MainHand.Item.Type == ItemType.TwoHandMace
                    || character.MainHand.Item.Type == ItemType.TwoHandSword
                    || character.MainHand.Item.Type == ItemType.Polearm)
                {
                    normalizationFactor = 3.3f;
                    MH.damage *= 1f + .02f * talents.TwoHandedWeaponSpecialization;
                }
                else normalizationFactor = 2.4f;

                combinedSwingTime = MH.hastedSpeed;
                calcs.OHAttackSpeed = 0f;
                calcs.OHWeaponDamage = 0f;
                calcs.OHExpertise = 0f;
            }
            // DW or no MH
            else if (character.OffHand != null)
            {
                // need this for weapon swing procs
                // combinedSwingTime = 1f / MH.hastedSpeed + 1f / OH.hastedSpeed;
                // combinedSwingTime = 1f / combinedSwingTime;
                combinedSwingTime = (MH.hastedSpeed + OH.hastedSpeed) / 4;
                normalizationFactor = 2.4f;
            } 
            // Unarmed
            else if (character.MainHand == null && character.OffHand == null)
            {
                combinedSwingTime = 2f;
                normalizationFactor = 2.4f;
            }
        }
    }
}
