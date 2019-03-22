using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.TankDK
{
    class CombatTable
    {
        public bool m_bDW;
        public Character character;
//        public CharacterCalculationsTankDK calcs;
        public DeathKnightTalents talents;
        public Stats stats;
        public CalculationOptionsTankDK calcOpts;

        public int m_BloodRunes
        {
            get
            {
                return (int)this.calcOpts.m_Rotation.AbilityCost[(int)DKCostTypes.Blood];
            }
        }
        public int m_FrostRunes
        {
            get
            {
                return (int)this.calcOpts.m_Rotation.AbilityCost[(int)DKCostTypes.Frost];
            }
        }
        public int m_UnholyRunes
        {
            get
            {
                return (int)this.calcOpts.m_Rotation.AbilityCost[(int)DKCostTypes.UnHoly];
            }
        }
        public int m_DeathRunes
        {
            get
            {
                return (int)this.calcOpts.m_Rotation.AbilityCost[(int)DKCostTypes.Death];
            }
        }

        public Weapon MH, OH;

        public float combinedSwingTime;

        public float physCrits, 
            missedSpecial, dodgedSpecial, 
            spellCrits, spellResist, 
            totalMHMiss, totalOHMiss,
            realDuration, totalMeleeAbilities, totalParryableAbilities, 
            totalSpellAbilities, normalizationFactor,
            chanceAvoided, chanceMissed, chanceDodged, chanceParried;
        /// <summary>
        /// Used as Total GCD time - usually hasted, but need to confirm that.
        /// </summary>
        private float fDuration;
        public float m_fRSCount = 0f;
        public Rotation.Type rotType;

        public CombatTable(Character c, Stats stats, CalculationOptionsTankDK calcOpts) :
            this(c, new CharacterCalculationsTankDK(), stats, calcOpts)
        { }

        public CombatTable(Character c, CharacterCalculationsTankDK calcs, Stats stats, CalculationOptionsTankDK calcOpts)
        {
            m_bDW = false;
            this.character = c;
//            this.calcs = calcs;
            this.calcOpts = calcOpts;
            this.calcOpts.talents = character.DeathKnightTalents;
            this.talents = character.DeathKnightTalents;
            this.stats = stats;
            totalMeleeAbilities = 0f;
            totalSpellAbilities = 0f;

            totalMeleeAbilities = calcOpts.m_Rotation.PlagueStrike + calcOpts.m_Rotation.ScourgeStrike +
                calcOpts.m_Rotation.Obliterate + calcOpts.m_Rotation.BloodStrike + calcOpts.m_Rotation.HeartStrike +
                calcOpts.m_Rotation.FrostStrike + calcOpts.m_Rotation.DeathStrike;

            totalSpellAbilities = calcOpts.m_Rotation.DeathCoil + calcOpts.m_Rotation.IcyTouch + calcOpts.m_Rotation.HowlingBlast
                + calcOpts.m_Rotation.Pestilence + calcOpts.m_Rotation.HornOfWinter + calcOpts.m_Rotation.DeathNDecay
                + calcOpts.m_Rotation.BoneShield;

            totalParryableAbilities = calcOpts.m_Rotation.PlagueStrike + calcOpts.m_Rotation.ScourgeStrike +
                calcOpts.m_Rotation.Obliterate + calcOpts.m_Rotation.BloodStrike + calcOpts.m_Rotation.HeartStrike;

            Weapons();
            CritsAndResists();
            fDuration = calcOpts.m_Rotation.getGCDTime();

            rotType = calcOpts.m_Rotation.curRotationType;
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

                // Crit: Base .65%
                physCrits = .0065f;
                physCrits += stats.PhysicalCrit;

                chanceAvoided = 0.335f;

                chanceDodged = StatConversion.WHITE_DODGE_CHANCE_CAP[calcOpts.TargetLevel-character.Level];

                if (character.MainHand != null)
                {
                    chanceDodged = MH.chanceDodged;
                }

                if (character.OffHand != null)
                {
                    if (character.MainHand != null && character.MainHand.Slot != ItemSlot.TwoHand)
                    {
                        m_bDW = true;
                        chanceDodged += OH.chanceDodged;
                        chanceDodged /= 2;
                    }
                    else
                    {
                        chanceDodged = OH.chanceDodged;
                    }
                }

                float chanceMiss = StatConversion.WHITE_MISS_CHANCE_CAP[calcOpts.TargetLevel-character.Level];
                if (m_bDW)
                {
                      chanceMiss = StatConversion.WHITE_MISS_CHANCE_CAP_DW[calcOpts.TargetLevel-character.Level]; 
                }
                chanceMiss -= stats.PhysicalHit;
                chanceMiss = Math.Max(0f, chanceMiss);

                chanceAvoided = chanceDodged + chanceMiss;

                missedSpecial = chanceMiss;
                dodgedSpecial = chanceDodged;

                spellCrits = 0f;
                spellCrits += stats.SpellCrit;

                // Resists: Base 17%
                spellResist = .17f;
                spellResist -= stats.SpellHit;
                spellResist = Math.Max(0f, spellResist);

                // Total physical misses
                totalMHMiss = MH.chanceDodged + chanceMiss;
                if (OH != null)
                    totalOHMiss = OH.chanceDodged + chanceMiss;
                realDuration = calcOpts.m_Rotation.curRotationDuration;
                realDuration += ((totalMeleeAbilities - calcOpts.m_Rotation.FrostStrike) * chanceDodged * (1.5f)) +
                    ((totalMeleeAbilities - calcOpts.m_Rotation.FrostStrike) * chanceMiss * (1.5f)) +
                    ((calcOpts.m_Rotation.IcyTouch * spellResist * (((1.5f) / (1 + (StatConversion.GetHasteFromRating(stats.HasteRating, CharacterClass.DeathKnight)) + stats.SpellHaste)) <= 1.0f ? 1.0f : (((1.5f) / (1 + (StatConversion.GetHasteFromRating(stats.HasteRating, CharacterClass.DeathKnight)) + stats.SpellHaste)))))); 
                //still need to implement spellhaste here
            }
            #endregion
        }

        public void Weapons()
        {
            float MHExpertise = stats.Expertise;
            float OHExpertise = stats.Expertise;

            Item barehand = new Item();
            barehand.Speed = 2.0f;
            barehand.MaxDamage = 2;
            barehand.MinDamage = 1;

            MH = new Weapon(barehand, stats, calcOpts, 0f);
            OH = new Weapon(barehand, stats, calcOpts, 0f);

            // Set if we're DW or not in 1 place.  
            // Then use that member var to determine all other DW related issues.
            if (character.MainHand != null && character.MainHand.Slot != ItemSlot.TwoHand && character.OffHand != null)
            {
                m_bDW = true;
            }

            if (character.MainHand != null)
            {
                MH = new Weapon(character.MainHand.Item, stats, calcOpts, MHExpertise);
                if (character.MainHand.Slot == ItemSlot.TwoHand)
                {
                    normalizationFactor = 3.3f;
                    MH.damage *= 1f + .02f * talents.TwoHandedWeaponSpecialization;
                    combinedSwingTime = MH.hastedSpeed;
                }
                else normalizationFactor = 2.4f;
            }

            if (m_bDW)
            {
                OH = new Weapon(character.OffHand.Item, stats, calcOpts, OHExpertise);

                // Talent: Nerves of Cold Steel 
                float OHMult = .08f * (float)talents.NervesOfColdSteel;
                OH.damage *= (.5f + OHMult);

                //need this for weapon swing procs
                // in DW situations, weapon swing procs should happen MORE often, not less.
                combinedSwingTime = 1 / (1 / MH.hastedSpeed + 1 / OH.hastedSpeed);
            }

            if (character.MainHand == null && character.OffHand == null)
            {
                combinedSwingTime = 2f;
                normalizationFactor = 2.4f;
            }
        }

        public float GetTotalThreat()
        {
            //DPS Subgroups
            float fDamWhite = 0f;
            float fDamBCB = 0f;
            float fDamNecrosis = 0f;
            float fDamDeathCoil = 0f;
            float fDamIcyTouch = 0f;
            float fDamPlagueStrike = 0f;
            float fDamFrostFever = 0f;
            float fDamBloodPlague = 0f;
            float fDamScourgeStrike = 0f;
            float fDamUnholyBlight = 0f;
            float fDamFrostStrike = 0f;
            float fDamHowlingBlast = 0f;
            float fDamDeathNDecay = 0f;
            float fDamObliterate = 0f;
            float fDamDeathStrike = 0f;
            float fDamBloodStrike = 0f;
            float fDamHeartStrike = 0f;
            float fDamDancingRuneWeapon = 0f;
            float fDamWanderingPlague = 0f;
            float fDamWPFromFF = 0f;
            float fDamWPFromBP = 0f;
            float fDamOtherShadow = 0f;
            float fDamOtherArcane = 0f;
            float fDamOtherFrost = 0f;
            //float fDamBloodworms = 0f;
            float fDamRuneStrike = 0f;

            float missedSpecial = 0f;

            float fDamWhiteBeforeArmor = 0f;
            float fDamWhiteMinusGlancing = 0f;
            float fightDuration = calcOpts.FightLength * 60;
            float mitigation;
            float KMRatio = 0f;
            float CinderglacierMultiplier = 1f;

            float MHExpertise = stats.Expertise;
            float OHExpertise = stats.Expertise;

            //damage multipliers
            float spellPowerMult = 1f + stats.BonusSpellPowerMultiplier;
            float frostSpellPowerMult = 1f + stats.BonusSpellPowerMultiplier + Math.Max((stats.BonusFrostDamageMultiplier), 0f); //implement razorice here later
            float shadowSpellPowerMult = 1f + stats.BonusSpellPowerMultiplier + Math.Max((stats.BonusShadowDamageMultiplier), 0f); //implement razorice here later

            float physPowerMult = 1f + stats.BonusPhysicalDamageMultiplier;
            // Covers all % physical damage increases.  Blood Frenzy, FI.
            float partialResist = 0.94f; // Average of 6% damage lost to partial resists on spells

            /* Coefficients As of 3.1:
             * http://www.tankspot.com/forums/f200/41215-dk-attack-power-coefficients.html
             * Attack Power Coefficients table:
             * Ability 	Coefficient 
             * Blood Boil	0.04
                Blood Plague	0.055
                Bloodworms	0.006
                Corpse Explosion	0.0475
                Death and Decay	0.0475
                Death Coil	0.15
                Frost Fever	0.055
                Howling Blast	0.1
                Summon Gargoyle	0.4
                Icy Touch	0.1
                Pestilence	0.04
            */

            //spell AP multipliers, for diseases its per tick
//            float BloodBoilAPMult = .04f;
            float BloodPlagueAPMult = 0.055f;
//            float CorpseExplosionAPMult = .0475f;
            float DeathCoilAPMult = 0.15f;
            float DeathNDecayAPMult = .0475f;
            float FrostFeverAPMult = 0.055f;
            float GargoyleAPMult = 0.4f;
            float HowlingBlastAPMult = 0.1f;
            float IcyTouchAPMult = 0.1f;
            //float StrangulateAPMult = .06f;

            //for estimating rotation pushback

            calcOpts.m_Rotation.avgDiseaseMult = calcOpts.m_Rotation.numDisease * (calcOpts.m_Rotation.diseaseUptime / 100);
            float commandMult = 0f;

            if (calcOpts.m_Rotation.managedRP)
            {
                calcOpts.m_Rotation.getRP(talents, character);
            }

            /* Threat table as of 3.3.3a
             * http://www.tankspot.com/showthread.php?40485-Death-Knight-threat-values&p=397523#post397523
            Anti-Magic Shell ___________________  0
            *Anti-Magic Shell RP gain ___________  5 per RP (approx), unaffected by presence.  See * below.
            *Anti-Magic Zone ____________________  0
            *Abomination's Might ________________  0
            *Blood Aura _________________________  0
            *Blade Barrier ______________________  20, split between all mobs
            Blood Boil _________________________  damage
            *Blood Caked Blade __________________  damage
            Blood Plague _______________________  damage
            *Blood Presence reactive healing ____  0
            Blood Strike _______________________  damage
            *Blood Tap __________________________  0
            *Bloody Vengeance ___________________  0
            *Bloodworms _________________________  0 The DK gains no threat from BW's damage or healing.  See ** below.
            *Bone Shield ________________________  0
            *Butchery RP gains on kills _________  5 per RP, unaffected by presence
            Chains of Ice ______________________ 240
            *Chilblains (all ranks) _____________  2
            *Chill of the Grave RP gains ________  5 per RP, unaffected by presence
            *Corpse Explosion ___________________  damage
            *Crypt Fever ________________________  0
            *Dancing Rune Weapon ________________ damage
            Death and Decay ____________________  damage × 1.90
            Deathchill _________________________  55, split between all mobs
            Death Coil _________________________  damage
            Death Coil healing _________________  healing × 0.5 × presence, split between all mobs
            Death Grip _________________________  110
            Death Pact _________________________  0
            Death Strike _______________________  damage
            Death Strike healing _______________  healing × 0.5 × presence, split between all mobs
            *Desecration ________________________  0
            *Dirge RP gains _____________________  5 per RP, unaffected by presence
            *Ebon Plaguebringer _________________  0
            Empower Rune Weapon ________________  0
            Frost Fever ________________________  damage
            Frost Strike _______________________  damage
            Heart Strike _______________________  damage
            Horn of Winter _____________________  75 ÷ number of units buffed, split between mobs
            Howling Blast ______________________  damage
            *Hungering Cold _____________________  112, split between all mobs affected
            *Hysteria ___________________________  55 (the DK gains the threat, not the target)
            Icebound Fortitude _________________  0
            Icy Talons _________________________  0
            Improved Icy Talons ________________  0
            Icy Touch __________________________  damage x7
            Killing Maching ____________________  0
            *Lichborne __________________________  110, split between all mobs
            Mark of Blood ______________________  0
            Mind Freeze ________________________  0
            Obliterate _________________________  damage
            Pestilence _________________________ 0
            Plague Strike ______________________  damage
            *Raise Dead _________________________  57, split between mobs
            *Rime _______________________________  0
            Rune Strike ________________________  damage × 1.75
            *Rune Tap ___________________________  healing × 0.5 + 55, split between all mobs
            Scent of Blood _____________________  0
            Scent of Blood RP gains ____________  0
            *Scourge Strike _____________________  damage + 120
            Strangulate ________________________  138 on application
            *Unholy Blight ______________________  damage
            *Unholy Strength ____________________  healing × 0.5 × presence, split between all mobs
            *Unbreakable Armor __________________  0
            *Vampiric Blood _____________________  0
            *Vendetta healing ___________________  0
            *Wandering Plague ___________________  damage
            */
            #region Impurity Application
            {
                // TALENT: Impurity
                float impurityMult = 1f + (.05f * (float)talents.Impurity);

                HowlingBlastAPMult *= impurityMult;
                IcyTouchAPMult *= impurityMult;
                FrostFeverAPMult *= impurityMult;
                BloodPlagueAPMult *= impurityMult;
                DeathCoilAPMult *= impurityMult;
                GargoyleAPMult *= impurityMult;
            }
            #endregion

            #region racials
            {
                if (character.Race == CharacterRace.Orc)
                {
                    commandMult += .05f;
                }
            }
            #endregion

            #region Killing Machine
            {
                float KMPpM = (1f * talents.KillingMachine) * (1f + stats.PhysicalHaste); // KM Procs per Minute (Defined "1 per point" by Blizzard) influenced by Phys. Haste

                float KMPpR = KMPpM / (60 / calcOpts.m_Rotation.curRotationDuration);
                float totalAbilities = calcOpts.m_Rotation.FrostStrike + calcOpts.m_Rotation.IcyTouch + calcOpts.m_Rotation.HowlingBlast;
                if (totalAbilities > 0)
                {
                    KMRatio = KMPpR / totalAbilities;
                }
            }
            #endregion

            #region Cinderglacier
            {
                float shadowFrostAbilitiesPerSecond = ((calcOpts.m_Rotation.DeathCoil + calcOpts.m_Rotation.FrostStrike +
                        calcOpts.m_Rotation.ScourgeStrike + calcOpts.m_Rotation.IcyTouch + calcOpts.m_Rotation.HowlingBlast) /
                        this.realDuration);
                if (shadowFrostAbilitiesPerSecond > 0)
                {
                    CinderglacierMultiplier *= 1f + (0.2f / (shadowFrostAbilitiesPerSecond / stats.CinderglacierProc));
                }
            }
            #endregion

            #region Mitigation
            {
                float targetArmor = calcOpts.BossArmor;
                float totalArP = stats.ArmorPenetration;

                mitigation = 1f - StatConversion.GetArmorDamageReduction(character.Level, targetArmor, stats.ArmorPenetration, 0f, 0f);
                mitigation = Math.Max(0, mitigation);
//                calcs.EnemyMitigation = 1f - mitigation;
//                calcs.EffectiveArmor = mitigation;
            }
            #endregion

            // These white damage values need to carry forward beyond just the individaul white damage attacks.
            // BCB matters.
            float MHDam = 0f, OHDam = 0f;
            #region White Dmg
            {
                #region Main Hand
                {
                    float fDamMHglancing = (StatConversion.WHITE_GLANCE_CHANCE_CAP[calcOpts.TargetLevel-character.Level] * MH.damage) * 0.75f;
                    float fDamMHBeforeArmor = ((MH.damage * (1f - chanceAvoided - StatConversion.WHITE_GLANCE_CHANCE_CAP[calcOpts.TargetLevel - character.Level])) * (1f + physCrits)) + fDamMHglancing;
                    fDamWhiteMinusGlancing = fDamMHBeforeArmor - fDamMHglancing;
                    fDamWhiteBeforeArmor = fDamMHBeforeArmor;
                    MHDam = fDamMHBeforeArmor * mitigation;
                    // fDuration = total GCD time.
                    MHDam *= fDuration / MH.hastedSpeed;
                }
                #endregion

                #region Off Hand
                if (m_bDW)
                {
                    if (this.OH.damage > 0)
                    {
                        float fDamOHglancing = (StatConversion.WHITE_GLANCE_CHANCE_CAP[calcOpts.TargetLevel - character.Level] * this.OH.damage) * 0.75f;
                        float fDamOHBeforeArmor = ((this.OH.damage * (1f - chanceAvoided - StatConversion.WHITE_GLANCE_CHANCE_CAP[calcOpts.TargetLevel - character.Level])) * (1f + this.physCrits)) + fDamOHglancing;
                        fDamWhiteMinusGlancing += fDamOHBeforeArmor - fDamOHglancing;
                        fDamWhiteBeforeArmor += fDamOHBeforeArmor;
                        OHDam = fDamOHBeforeArmor * mitigation;
                        OHDam *= fDuration / OH.hastedSpeed;
                    }
                    else
                    {
                        OHDam = 0f;
                    }
                }
                #endregion

                fDamWhite = MHDam + OHDam;
            }
            #endregion

            #region Necrosis
            {
                fDamNecrosis = fDamWhiteMinusGlancing * (.04f * (float)talents.Necrosis); // doesn't proc off Glancings
            }
            #endregion

            #region Blood Caked Blade
            {
                float fDamMHBCB = 0f;
                float fDamOHBCB = 0f;
                if ((OHDam != 0) && m_bDW)
                {
                    fDamOHBCB = OHDam * (.25f + .125f * calcOpts.m_Rotation.avgDiseaseMult);
                }
                if (MHDam != 0)
                {
                    fDamMHBCB = MHDam * (.25f + .125f * calcOpts.m_Rotation.avgDiseaseMult);
                }
                fDamBCB = fDamMHBCB + fDamOHBCB;
                fDamBCB *= .1f * (float)talents.BloodCakedBlade;
            }
            #endregion

            #region Death Coil
            {
                if (calcOpts.m_Rotation.DeathCoil > 0f)
                {
                    float DCCount = calcOpts.m_Rotation.DeathCoil + (0.05f * (float)talents.SuddenDoom * (calcOpts.m_Rotation.HeartStrike + calcOpts.m_Rotation.BloodStrike));
                    float DCDmg = 443f + (DeathCoilAPMult * stats.AttackPower) + stats.BonusDeathCoilDamage;
                    // Glyph of Dark Death adds 15% damage to each DC.
                    DCDmg *= 1f + (talents.GlyphofDarkDeath ? .15f : 0f);
                    // Mobidity adds 5% per point of damage to each DC.
                    DCDmg *= 1f + (.05f * (float)talents.Morbidity);
                    // Unholy Blight causes additional 10% DC damage over 10 sec. 
                    DCDmg *= 1f + (talents.UnholyBlight * 0.1f);
                    // Since we're looking at total threat returned, do not divide the damage on a per-second basis.
                    fDamDeathCoil = DCDmg * DCCount;
                    float DCCritDmgMult = 1.5f * (1f + stats.BonusSpellCritMultiplier);
                    float DCCrit = 1f + ((this.spellCrits + stats.BonusDeathCoilCrit) * DCCritDmgMult);
                    fDamDeathCoil *= DCCrit;
                }
            }
            #endregion

            #region Icy Touch
            // this seems to handle crit strangely.
            // additionally, looks like it's missing some multipliers? maybe they're applied later
            {
                if (calcOpts.m_Rotation.IcyTouch > 0f)
                {
                    float addedCritFromKM = KMRatio;
                    float ITCount = calcOpts.m_Rotation.IcyTouch;
                    float ITDmg = 236f + (IcyTouchAPMult * stats.AttackPower) + stats.BonusIcyTouchDamage;
                    ITDmg *= 1f + (.1f * (float)talents.ImprovedIcyTouch);
                    // Total damage for IT is ITdam * # of ITs
                    fDamIcyTouch = ITDmg * ITCount;
                    float ITCritDmgMult = 1.5f * (1f + stats.BonusSpellCritMultiplier);
                    float ITCrit = 1f + ((this.spellCrits + addedCritFromKM + (.05f * (float)talents.Rime)) * ITCritDmgMult);
                    fDamIcyTouch *= ITCrit;
                    fDamIcyTouch *= 7;

                }
            }
            #endregion

            #region Plague Strike
            {
                if (calcOpts.m_Rotation.PlagueStrike > 0f)
                {
                    float PCCount = calcOpts.m_Rotation.PlagueStrike;
                    float PSDmg = (MHDam * .5f) + 189f;
                    PSDmg += (OHDam * (talents.ThreatOfThassarian / 3f) * 0.5f);
                    fDamPlagueStrike = PSDmg * PCCount;
                    float PSCritDmgMult = 2f * (1f + (.15f * (float)talents.ViciousStrikes) + stats.BonusCritMultiplier);
                    float PSCrit = 1f + ((this.physCrits + (.03f * (float)talents.ViciousStrikes) + stats.BonusPlagueStrikeCrit) * PSCritDmgMult);
                    fDamPlagueStrike *= PSCrit;

                    fDamPlagueStrike *= (talents.GlyphofPlagueStrike ? 1.2f : 1f);

                    fDamPlagueStrike *= 1f + (.1f * (float)talents.Outbreak);
                }
            }
            #endregion

            #region Frost Fever
            {
                if (calcOpts.m_Rotation.IcyTouch > 0f || (talents.GlyphofHowlingBlast && calcOpts.m_Rotation.HowlingBlast > 0f))
                {
                    // Frost Fever is renewed with every Icy Touch/HB and starts a new cd
                    // So how many applications of FF do we have through the rotation?
                    float FFCount = calcOpts.m_Rotation.IcyTouch + (talents.GlyphofHowlingBlast ? calcOpts.m_Rotation.HowlingBlast : 0f);
                    float FFCD = 3f / (calcOpts.m_Rotation.diseaseUptime / 100f);
                    int tempF = (int)Math.Floor(FFCount / FFCD);
                    FFCD = ((calcOpts.m_Rotation.IcyTouch - ((float)tempF * FFCD)) / ((float)tempF + 1f)) + FFCD;
                    // Patch 3.2: Incease Damage by 15%
                    float FFDmg = (FrostFeverAPMult * stats.AttackPower + 25.6f) * 1.15f;
                    // Patch 3.3: Glyph of IT increases damage by 20%
                    FFDmg *= 1f + (talents.GlyphofIcyTouch ? .2f : 0f);
                    if (fDuration > 0)
                    {
                        fDamFrostFever = FFDmg / FFCD * fDuration;
                        if (talents.GlyphofHowlingBlast && calcOpts.m_Rotation.HowlingBlast > 0f)
                        {
                            // If we're using the glyph of HB, then all targets get FF.
                            fDamFrostFever *= calcOpts.uNumberTargets;
                        }
                        fDamWPFromFF = fDamFrostFever * this.physCrits * fDuration;
                    }
                }
            }
            #endregion

            #region Blood Plague
            {
                if (calcOpts.m_Rotation.PlagueStrike > 0f)
                {
                    // Blood Plague is renewed with every Plague Strike and starts a new cd
                    float PSCD = calcOpts.m_Rotation.curRotationDuration / calcOpts.m_Rotation.PlagueStrike;
                    float BPCD = 3f / (calcOpts.m_Rotation.diseaseUptime / 100);
                    int tempF = (int)Math.Floor(PSCD / BPCD);
                    BPCD = ((PSCD - ((float)tempF * BPCD)) / ((float)tempF + 1f)) + BPCD;
                    // Patch 3.2: Increase damage by 15%
                    float BPDmg = (BloodPlagueAPMult * stats.AttackPower + 31.1f) * 1.15f;
                    if (fDuration > 0)
                    {
                        fDamBloodPlague = BPDmg / BPCD * fDuration;
                        fDamWPFromBP = fDamBloodPlague * this.physCrits * fDuration;
                    }
                }
            }
            #endregion

            #region Pestilence
            {
                if (calcOpts.m_Rotation.Pestilence > 0f )
                {
                    fDamBloodPlague *= calcOpts.uNumberTargets;
                    fDamFrostFever *= calcOpts.uNumberTargets;
                }
            }
            #endregion

            #region Wandering Plague
            {
                if ((float)talents.WanderingPlague > 0)
                {
                    fDamWanderingPlague = fDamWPFromBP + fDamWPFromFF;
                    fDamWanderingPlague *= (1f / 3f) * (float)talents.WanderingPlague;
                    // Since the damage is spread across all nearby targets, let's add that in.
                    fDamWanderingPlague *= calcOpts.uNumberTargets;
                }
                else
                {
                    fDamWanderingPlague = 0f;
                }
            }
            #endregion

            #region Scourge Strike
            {
                if (talents.ScourgeStrike > 0 && calcOpts.m_Rotation.ScourgeStrike > 0f)
                {
                    float SSCount = calcOpts.m_Rotation.ScourgeStrike;
                    float SSDmg = (MHDam * .70f) + 357.188f +
                        stats.BonusScourgeStrikeDamage;
                    SSDmg *= 1f + 0.12f * calcOpts.m_Rotation.avgDiseaseMult * (1f + stats.BonusPerDiseaseScourgeStrikeDamage);
                    SSDmg += 120; // Bonus threat
                    fDamScourgeStrike = SSDmg * SSCount;
                    float SSCritDmgMult = 2f * (1f + (.15f * (float)talents.ViciousStrikes) + stats.BonusCritMultiplier);
                    float SSCrit = 1f + ((this.physCrits + (.03f * (float)talents.ViciousStrikes) + (.03f * (float)talents.Subversion) + stats.BonusScourgeStrikeCrit) * SSCritDmgMult);
                    fDamScourgeStrike *= SSCrit;
                    fDamScourgeStrike *= 1f + ((2f/3f) * (float)talents.Outbreak);
                }
            }
            #endregion

            #region Frost Strike
            {
                if (talents.FrostStrike > 0 && calcOpts.m_Rotation.FrostStrike > 0f)
                {
                    float addedCritFromKM = KMRatio;
                    float FSCount = calcOpts.m_Rotation.FrostStrike;
                    // Patch 3.2: 60% Damage tuned to 55% weapon damage 
                    float FSDmg = (MHDam * .55f) +
                        150f + stats.BonusFrostStrikeDamage;
                    // Threat of Thassarian:
                    FSDmg = ((OHDam * .55f) + 150f + stats.BonusFrostStrikeDamage) * (talents.ThreatOfThassarian / 3f) * .5f;
                    fDamFrostStrike = FSDmg * FSCount;
                    float FSCritDmgMult = 2f * (1f + (.15f * (float)talents.GuileOfGorefiend) + stats.BonusCritMultiplier);
                    float FSCrit = 1f + ((this.physCrits + addedCritFromKM + stats.BonusFrostStrikeCrit) * FSCritDmgMult);
                    fDamFrostStrike *= FSCrit;
                    fDamFrostStrike *= 1f + .03f * talents.BloodOfTheNorth;
                }
            }
            #endregion

            #region Trinket direct-damage procs, razorice damage, etc
            {
                fDamOtherArcane = stats.ArcaneDamage;
                fDamOtherShadow = stats.ShadowDamage;

                if (this.MH != null) {
                    float dpsMHglancing = (StatConversion.WHITE_GLANCE_CHANCE_CAP[calcOpts.TargetLevel - character.Level] * MH.DPS) * 0.75f;
                    float dpsMHBeforeArmor = ((MH.DPS * (1f - chanceAvoided - StatConversion.WHITE_GLANCE_CHANCE_CAP[calcOpts.TargetLevel - character.Level])) * (1f + physCrits)) + dpsMHglancing;
                    fDamOtherFrost += (dpsMHBeforeArmor - dpsMHglancing);   // presumably doesn't proc off of glancings, like necrosis
                }

                if (this.OH != null) {
                    float dpsOHglancing = (StatConversion.WHITE_GLANCE_CHANCE_CAP[calcOpts.TargetLevel - character.Level] * OH.DPS) * 0.75f;
                    float dpsOHBeforeArmor = ((OH.DPS * (1f - chanceAvoided - StatConversion.WHITE_GLANCE_CHANCE_CAP[calcOpts.TargetLevel - character.Level])) * (1f + physCrits)) + dpsOHglancing;
                    fDamOtherFrost += (dpsOHBeforeArmor - dpsOHglancing);
                }

                fDamOtherFrost += fDamOtherFrost * stats.BonusFrostWeaponDamage;

                float OtherCritDmgMult = 1.5f * (1f + stats.BonusSpellCritMultiplier);
                float OtherCrit = 1f + ((this.spellCrits) * OtherCritDmgMult);
                fDamOtherArcane *= OtherCrit;
                fDamOtherShadow *= OtherCrit;
            }
            #endregion

            #region Howling Blast
            {
                if ((talents.HowlingBlast > 0) && (calcOpts.m_Rotation.HowlingBlast > 0f))
                {
                    float addedCritFromKM = KMRatio;
                    float HBCount = calcOpts.m_Rotation.HowlingBlast;
                    float HBDmg = 540f + stats.BonusHowlingBlastDamage + HowlingBlastAPMult * stats.AttackPower;
                    fDamHowlingBlast = HBDmg * HBCount;
                    float HBCritDmgMult = 1.5f * (1f + (.15f * (float)talents.GuileOfGorefiend) + stats.BonusSpellCritMultiplier);
                    float HBCrit = 1f + ((this.spellCrits + addedCritFromKM) * HBCritDmgMult);
                    fDamHowlingBlast *= HBCrit;
                    // Multiply this by the number of targets hit.
                    fDamHowlingBlast *= calcOpts.uNumberTargets;
                }
            }
            #endregion

            #region Death n Decay
            {
                if (calcOpts.m_Rotation.DeathNDecay > 0f)
                {
                    float DNDCD = 30f - (talents.Morbidity * 5f); // 30 sec cool down modified by Morbidity
                    float DNDDur = 10f; // 10 sec duration
                    float DNDDmg = (62 + DeathNDecayAPMult * stats.AttackPower * DNDDur) * (talents.GlyphofDeathandDecay ? 1.2f : 1f);
                    fDamDeathNDecay = DNDDmg * (Math.Min(calcOpts.m_Rotation.DeathNDecay, (fDuration / DNDCD))) ;
                    float DNDCritDmgMult = 1.5f * (1f + stats.BonusSpellCritMultiplier);
                    float DNDCrit = 1f + (this.spellCrits * DNDCritDmgMult );
                    fDamDeathNDecay *= DNDCrit;
                    // Threat: damage × 1.90
                    fDamDeathNDecay *= 1.9f;
                    // Multiply this by the number of targets hit.
                    fDamDeathNDecay *= calcOpts.uNumberTargets;
                }
            }
            #endregion

            #region Obliterate
            {
                if (calcOpts.m_Rotation.Obliterate > 0f)
                {
                    float OblitCount = calcOpts.m_Rotation.Obliterate;
                    float OblitDmg = (MHDam * 0.8f) + stats.BonusObliterateDamage;
                    // Threat of Thassarian:
                    OblitDmg += ((OHDam * 0.8f) + stats.BonusObliterateDamage) * (talents.ThreatOfThassarian / 3f) * 0.5f;
                    OblitDmg *= 1f + 0.125f * (float)calcOpts.m_Rotation.avgDiseaseMult * (1f + stats.BonusPerDiseaseObliterateDamage);
                    fDamObliterate = OblitDmg * OblitCount;
                    float OblitCritDmgMult = 2f * (1f + (.15f * (float)talents.GuileOfGorefiend) + stats.BonusCritMultiplier);
                    float OblitCrit = 1f + ((this.physCrits +
                        (.03f * (float)talents.Subversion) +
                        (0.05f * (float)talents.Rime) +
                        stats.BonusObliterateCrit) * OblitCritDmgMult);
                    fDamObliterate *= OblitCrit;
                    fDamObliterate *= (talents.GlyphofObliterate ? 1.2f : 1f);
                }
            }
            #endregion

            #region Death Strike
            {
                if (calcOpts.m_Rotation.DeathStrike > 0f)
                {
                    float DSCount = calcOpts.m_Rotation.DeathStrike;
                    float DSDmg = (MHDam * 0.75f) + 222.75f + stats.BonusDeathStrikeDamage;
                    // Threat of Thassarian:
                    DSDmg += (OHDam * .75f) * (talents.ThreatOfThassarian / 3f) * 0.5f;
                    DSDmg *= 1f + 0.15f * (float)talents.ImprovedDeathStrike;
                    DSDmg *= (talents.GlyphofDeathStrike ? 1.25f : 1f);
                    fDamDeathStrike = DSDmg * DSCount;
                    float DSCritDmgMult = 2f * (1f + (.15f * (float)talents.MightOfMograine) + stats.BonusCritMultiplier);
                    float DSCrit = 1f + ((this.physCrits +
                        (.03f * (float)talents.ImprovedDeathStrike) +
                        (0.05f * (float)talents.Rime) +
                        stats.BonusDeathStrikeCrit) * DSCritDmgMult);
                    // Factor in threat from damage
                    fDamDeathStrike *= DSCrit; 
                    // Add in threat from Healing.
                    fDamDeathStrike += ((stats.Health * .05f * calcOpts.m_Rotation.numDisease) / calcOpts.uNumberTargets / 2); // threat from Healing divided by number of targets.
                    stats.HealthRestoreFromMaxHealth += .05f * calcOpts.m_Rotation.numDisease;
                }
            }
            #endregion

            #region Blood Strike
            {
                if (calcOpts.m_Rotation.BloodStrike > 0f)
                {
                    float BSCount = calcOpts.m_Rotation.BloodStrike;
                    float BSDmg = (MHDam * 0.4f) + 305.6f + stats.BonusBloodStrikeDamage;
                    // Threat of Thassarian:
                    BSDmg += (OHDam * .4f) * (talents.ThreatOfThassarian / 3f) * 0.5f;
                    BSDmg *= 1f + 0.125f * (float)calcOpts.m_Rotation.avgDiseaseMult * (1f + stats.BonusPerDiseaseBloodStrikeDamage) ;
                    fDamBloodStrike = BSDmg * BSCount;
                    float BSCritDmgMult = 2f * (1f + (.15f * (float)talents.MightOfMograine) + (.15f * (float)talents.GuileOfGorefiend) + 
                        stats.BonusCritMultiplier);
                    float BSCrit = 1f + ((this.physCrits + (.03f * (float)talents.Subversion)) * BSCritDmgMult);
                    fDamBloodStrike *= BSCrit;
                    fDamBloodStrike *= 1f + (.03f * (float)talents.BloodOfTheNorth);
                    // Patch 3.2: From 15% to 5% per point.
                    fDamBloodStrike *= 1f + (.05f * (float)talents.BloodyStrikes);
                }
            }
            #endregion

            #region Heart Strike
            {
                if (talents.HeartStrike > 0 && calcOpts.m_Rotation.HeartStrike > 0f)
                {
                    float HSCount = calcOpts.m_Rotation.HeartStrike;
                    float HSDmg = ((this.MH.baseDamage + ((stats.AttackPower / 14f) * this.normalizationFactor)) *
                        0.5f) + 368f + stats.BonusHeartStrikeDamage;
                    HSDmg *= 1f + 0.1f * (float)calcOpts.m_Rotation.avgDiseaseMult * (1f + stats.BonusPerDiseaseHeartStrikeDamage) ;
                    fDamHeartStrike = HSDmg * HSCount;
                    float HSCritDmgMult = 2f * (1f + (.15f * (float)talents.MightOfMograine) + stats.BonusCritMultiplier);
                    float HSCrit = 1f + ((this.physCrits + (.03f * (float)talents.Subversion)) * HSCritDmgMult);
                    fDamHeartStrike *= HSCrit;
                    fDamHeartStrike += fDamHeartStrike * (0.5f * (Math.Min(2, calcOpts.uNumberTargets) - 1) );
                    // Patch 3.2: From 15% to 5% per point.
                    fDamHeartStrike *= 1f + (.05f * (float)talents.BloodyStrikes);
                }
            }
            #endregion

            #region Rune Strike
            {
                float RSDmg = (MHDam + (150 * stats.AttackPower * 10 / 10000));
                // Threat of Thassarian
                RSDmg += (OHDam +(150 * stats.AttackPower * 10 / 10000)) * (talents.ThreatOfThassarian / 3f) * 0.5f;
                RSDmg *= (1f + stats.BonusRuneStrikeMultiplier); // Two T8. 
                // what's the threat modifier?
                RSDmg *= (1.75f); // 3.3 update to RS Threat.
                float RSCritDmgMult = 2f * (1f + (.15f * (float)talents.MightOfMograine) + stats.BonusCritMultiplier + (talents.GlyphofRuneStrike ? .1f : 0f));
                float RSCrit = 1f + ((this.physCrits) * RSCritDmgMult);
                /*
                if (calcOpts.m_Rotation.RuneStrike == 0)
                {
                    // How many RS do we get?
                    // First off, we'll go w/ triggers off of the number of Boss Attacks over the duration.
                    // How many can we Dodge or Parry?
                    m_fRSCount = (fDuration / calcOpts.BossAttackSpeed) * (stats.Dodge + stats.Parry);
                    // Then make sure that we don't trigger more often than the number of white swings
                    // No more than the number of white swings.
                    m_fRSCount = Math.Min(m_fRSCount, (fDuration / MH.hastedSpeed));
                    // Then no more than the amount of RP available.
                    // However, RP may be 0 if Manage runic power is enabled, or if the user has their RP dumps tuned.
                    // Well, at least in this case, Manage RP is not enabled.  5
                    m_fRSCount = Math.Min(m_fRSCount, (calcOpts.m_Rotation.RP / 20f));
                    // No fewer than 0;
                    m_fRSCount = Math.Max(m_fRSCount, 0);
                }
                else
                {
                 */ 
                    m_fRSCount = calcOpts.m_Rotation.RuneStrike;
                
                //}
                fDamRuneStrike = RSDmg * m_fRSCount;
                fDamRuneStrike *= RSCrit;
            }
            #endregion

            float BCBMult = 1f;
            float BloodPlagueMult = 1f;
            float BloodStrikeMult = 1f + stats.BonusBloodStrikeDamageMultiplier;
            float DeathCoilMult = 1f;
            float DancingRuneWeaponMult = 1f;
            float FrostFeverMult = 1f;
            float FrostStrikeMult = 1f;
            float BloodwormsMult = 1f + commandMult;
            float HeartStrikeMult = 1f + stats.BonusHeartStrikeDamageMultiplier;
            float HowlingBlastMult = 1f;
            // Adding the multiplier from the T10 set bonus.
            float DeathNDecayMult = 1f + stats.TankDK_T10_2pc;
            float IcyTouchMult = 1f;
            float NecrosisMult = 1f;
            float ObliterateMult = 1f;
            float DeathStrikeMult = 1f;
            float PlagueStrikeMult = 1f;
            float ScourgeStrikeMult = 1f;
            float UnholyBlightMult = 1f;
            float WhiteMult = 1f;
            float WanderingPlagueMult = 1f;
            float otherShadowMult = 1f;
            float otherArcaneMult = 1f;
            float otherFrostMult = 1f;

            #region Apply Physical Mitigation
            {
                float physMit = mitigation;
                physMit *= 1f + (m_bDW ? 0f : .02f * talents.TwoHandedWeaponSpecialization);

                fDamBCB *= physMit;
                fDamBloodStrike *= physMit;
                fDamHeartStrike *= physMit;
                fDamObliterate *= physMit;
                fDamDeathStrike *= physMit;
                fDamPlagueStrike *= physMit;
                fDamRuneStrike *= physMit;

                WhiteMult += physPowerMult - 1f;
                BCBMult += physPowerMult - 1f;
                BloodStrikeMult += physPowerMult - 1f;
                HeartStrikeMult += physPowerMult - 1f;
                ObliterateMult += physPowerMult - 1f;
                DeathStrikeMult += physPowerMult - 1f;
                PlagueStrikeMult += physPowerMult - 1f;
            }
            #endregion

            #region Apply Elemental Strike Mitigation
            {
                float strikeMit = /*missedSpecial **/ partialResist;
                strikeMit *= (m_bDW ? 1f : 1f + .02f * talents.TwoHandedWeaponSpecialization);

                fDamScourgeStrike *= strikeMit;
                fDamFrostStrike *= strikeMit * (1f - missedSpecial);

                ScourgeStrikeMult += spellPowerMult - 1f;
                FrostStrikeMult += frostSpellPowerMult - 1f;
            }
            #endregion

            #region Apply Magical Mitigation
            {
                // some of this applies to necrosis, I wonder if it is ever accounted for
                float magicMit = partialResist /** combatTable.spellResist*/;
                // magicMit = 1f - magicMit;

                fDamNecrosis *= magicMit;
                fDamBloodPlague *= magicMit;
                fDamDeathCoil *= magicMit * (1 - this.spellResist);
                fDamFrostFever *= magicMit;
                fDamHowlingBlast *= magicMit * (1 - this.spellResist);
                fDamIcyTouch *= magicMit;
                fDamUnholyBlight *= magicMit * (1 - this.spellResist);


                NecrosisMult += spellPowerMult - 1f;
                BloodPlagueMult += spellPowerMult - 1f;
                DeathCoilMult += shadowSpellPowerMult - 1f;
                FrostFeverMult += frostSpellPowerMult - 1f;
                HowlingBlastMult += frostSpellPowerMult - 1f;
                IcyTouchMult += frostSpellPowerMult - 1f;
                UnholyBlightMult += shadowSpellPowerMult - 1f;
                otherShadowMult += shadowSpellPowerMult - 1f;
                otherArcaneMult += spellPowerMult - 1f;
                otherFrostMult += frostSpellPowerMult - 1f;
            }
            #endregion

            #region Cinderglacier multipliers
            {
                DeathCoilMult *= CinderglacierMultiplier;
                HowlingBlastMult *= CinderglacierMultiplier;
                IcyTouchMult *= CinderglacierMultiplier;
                ScourgeStrikeMult *= CinderglacierMultiplier;
                FrostStrikeMult *= CinderglacierMultiplier;
            }
            #endregion

            #region Apply Multi-Ability Talent Multipliers
            {
                float BloodyVengeanceMult = .03f * (float)talents.BloodyVengeance;
                BCBMult *= 1 + BloodyVengeanceMult;
                BloodStrikeMult *= 1 + BloodyVengeanceMult;
                HeartStrikeMult *= 1 + BloodyVengeanceMult;
                ObliterateMult *= 1 + BloodyVengeanceMult;
                DeathStrikeMult *= 1 + BloodyVengeanceMult;
                PlagueStrikeMult *= 1 + BloodyVengeanceMult;
                WhiteMult *= 1 + BloodyVengeanceMult;

                /*
                float HysteriaCoeff = .3f / 6f; // current uptime is 16.666...%
                float HysteriaMult = HysteriaCoeff * (float)talents.Hysteria;
                BCBMult *= 1f + HysteriaMult;
                BloodStrikeMult *= 1f + HysteriaMult;
                HeartStrikeMult *= 1f + HysteriaMult;
                ObliterateMult *= 1f + HysteriaMult;
                DeathStrikeMult *= 1f + HysteriaMult;
                PlagueStrikeMult *= 1f + HysteriaMult;
                WhiteMult *= 1f + HysteriaMult;
                */

                float BlackIceMult = .02f * (float)talents.BlackIce;
                FrostFeverMult *= 1 + BlackIceMult;
                HowlingBlastMult *= 1 + BlackIceMult;
                IcyTouchMult *= 1 + BlackIceMult;
                FrostStrikeMult *= 1 + BlackIceMult;
                DeathCoilMult *= 1 + BlackIceMult;
                ScourgeStrikeMult *= 1 + BlackIceMult;
                BloodPlagueMult *= 1 + BlackIceMult;
                otherShadowMult *= 1 + BlackIceMult;
                otherFrostMult *= 1 + BlackIceMult;

                float MercilessCombatMult = .315f * 0.06f * (float)talents.MercilessCombat;   // The last 35% of a Boss don't take 35% of the fight-time...say .315 (10% faster)
                ObliterateMult *= 1 + MercilessCombatMult;
                HowlingBlastMult *= 1 + MercilessCombatMult;
                IcyTouchMult *= 1 + MercilessCombatMult;
                FrostStrikeMult *= 1 + MercilessCombatMult;

                float GlacierRot = .0666666666666f * (float)talents.GlacierRot;
                HowlingBlastMult *= 1 + GlacierRot;
                IcyTouchMult *= 1 + GlacierRot;
                FrostStrikeMult *= 1 + GlacierRot;


                float CryptFeverMult = .1f * (float)talents.CryptFever;
                float CryptFeverBuff = stats.BonusDiseaseDamageMultiplier;
                CryptFeverMult = Math.Max(CryptFeverMult, CryptFeverBuff);
                FrostFeverMult *= 1 + CryptFeverMult;
                BloodPlagueMult *= 1 + CryptFeverMult;
                UnholyBlightMult *= 1 + CryptFeverMult;

                float DesolationMult = .01f * (float)talents.Desolation;  //the new desolation is basically a flat 1% per point
                BCBMult *= 1 + DesolationMult;
                BloodPlagueMult *= 1 + DesolationMult;
                BloodStrikeMult *= 1 + DesolationMult;
                DeathCoilMult *= 1 + DesolationMult;
                DancingRuneWeaponMult *= 1 + DesolationMult;
                FrostFeverMult *= 1 + DesolationMult;
                FrostStrikeMult *= 1 + DesolationMult;
                HeartStrikeMult *= 1 + DesolationMult;
                HowlingBlastMult *= 1 + DesolationMult;
                DeathNDecayMult *= 1 + DesolationMult;
                IcyTouchMult *= 1 + DesolationMult;
                NecrosisMult *= 1 + DesolationMult;
                ObliterateMult *= 1 + DesolationMult;
                DeathStrikeMult *= 1 + DesolationMult;
                PlagueStrikeMult *= 1 + DesolationMult;
                ScourgeStrikeMult *= 1 + DesolationMult;
                UnholyBlightMult *= 1 + DesolationMult;
                WhiteMult *= 1 + DesolationMult;
                otherShadowMult *= 1 + DesolationMult;
                otherArcaneMult *= 1 + DesolationMult;
                otherFrostMult *= 1 + DesolationMult;

                if ((float)talents.BoneShield >= 1f)
                {
                    float BoneMult = .02f;
                    BCBMult *= 1 + BoneMult;
                    BloodPlagueMult *= 1 + BoneMult;
                    BloodStrikeMult *= 1 + BoneMult;
                    DeathCoilMult *= 1 + BoneMult;
                    DancingRuneWeaponMult *= 1 + BoneMult;
                    FrostFeverMult *= 1 + BoneMult;
                    FrostStrikeMult *= 1 + BoneMult;
                    HeartStrikeMult *= 1 + BoneMult;
                    HowlingBlastMult *= 1 + BoneMult;
                    DeathNDecayMult *= 1 + BoneMult;
                    IcyTouchMult *= 1 + BoneMult;
                    NecrosisMult *= 1 + BoneMult;
                    ObliterateMult *= 1 + BoneMult;
                    DeathStrikeMult *= 1 + BoneMult;
                    PlagueStrikeMult *= 1 + BoneMult;
                    ScourgeStrikeMult *= 1 + BoneMult;
                    UnholyBlightMult *= 1 + BoneMult;
                    WhiteMult *= 1 + BoneMult;
                    otherShadowMult *= 1 + BoneMult;
                    otherArcaneMult *= 1 + BoneMult;
                    otherFrostMult *= 1 + BoneMult;
                }
            }
            #endregion

            //feed variables for output
            fDamBCB *= BCBMult;
            fDamBloodPlague *= BloodPlagueMult;
            fDamBloodStrike *= BloodStrikeMult;
            fDamDeathCoil *= DeathCoilMult;
            fDamDancingRuneWeapon *= DancingRuneWeaponMult;
            fDamFrostFever *= FrostFeverMult;
            fDamFrostStrike *= FrostStrikeMult;
            //fDamBloodworms *= BloodwormsMult;
            fDamHeartStrike *= HeartStrikeMult;
            fDamHowlingBlast *= HowlingBlastMult;
            fDamDeathNDecay *= DeathNDecayMult;
            fDamIcyTouch *= IcyTouchMult;
            fDamNecrosis *= NecrosisMult;
            fDamObliterate *= ObliterateMult;
            fDamDeathStrike *= DeathStrikeMult;
            fDamPlagueStrike *= PlagueStrikeMult;
            fDamScourgeStrike *= ScourgeStrikeMult;
            fDamUnholyBlight *= UnholyBlightMult;
            fDamWhite *= WhiteMult;
            fDamWanderingPlague *= WanderingPlagueMult;
            fDamRuneStrike += fDamOtherShadow * otherShadowMult +
                fDamOtherArcane * otherArcaneMult +
                fDamOtherFrost * otherFrostMult;

            float DPSPoints = 0;
            DPSPoints += fDamBCB;
            DPSPoints += fDamBloodPlague;
            DPSPoints += fDamBloodStrike;
            DPSPoints += fDamDeathCoil;
            DPSPoints += fDamDancingRuneWeapon;
            DPSPoints += fDamFrostFever;
            DPSPoints += fDamFrostStrike;
            DPSPoints += fDamHeartStrike;
            DPSPoints += fDamHowlingBlast;
            DPSPoints += fDamDeathNDecay;
            DPSPoints += fDamIcyTouch;
            DPSPoints += fDamNecrosis;
            DPSPoints += fDamObliterate;
            DPSPoints += fDamDeathStrike;
            DPSPoints += fDamPlagueStrike;
            DPSPoints += fDamScourgeStrike;
            DPSPoints += fDamUnholyBlight;
            DPSPoints += fDamWhite;
            DPSPoints += fDamWanderingPlague;
            DPSPoints += fDamRuneStrike;

            // Multiply by Frost Presence modifier:
            DPSPoints *= 2.0735f;
            DPSPoints *= 1 + stats.BonusDamageMultiplier;
            // Figure out how much damage may have been done when factoring in missrate.
            // This currenty is just physicalhit not spell hit.  Need to fix that.
            DPSPoints *= 1f - chanceMissed;

#if DEBUG
            if ( float.IsNaN(DPSPoints) )
            {
                throw new Exception("DPSPoints NaN");
            }
#endif 

            return DPSPoints;
        }
    }
}
