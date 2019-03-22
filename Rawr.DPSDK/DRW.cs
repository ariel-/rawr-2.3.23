using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.DPSDK
{
    class DRW
    {

        public float dpsDancingRuneWeapon = 0f;

        public DRW(CombatTable combatTable, CharacterCalculationsDPSDK calcs, CalculationOptionsDPSDK calcOpts, Stats stats, 
                                Character character, DeathKnightTalents talents)
        {
            bool DW = false;

            float dpsWhite = 0f;
            float dpsBCB = 0f;
            float dpsNecrosis = 0f;
            float dpsDeathCoil = 0f;
            float dpsIcyTouch = 0f;
            float dpsPlagueStrike = 0f;
            float dpsFrostFever = 0f;
            float dpsBloodPlague = 0f;
            float dpsDeathStrike = 0f;
            float dpsHeartStrike = 0f;
            float dpsWPFromFF = 0f;
            float dpsWPFromBP = 0f;
            float dpsWhiteMinusGlancing = 0f;
            float dpsWhiteBeforeArmor = 0f;

            float IcyTouchAPMult = 0.1f;
            float FrostFeverAPMult = 0.055f;
            float BloodPlagueAPMult = 0.055f;
            float DeathCoilAPMult = 0.15f;

            float OHMult = 0.5f;

            int targetLevel = calcOpts.TargetLevel;
            float arpBuffs = talents.BloodGorged * 2f / 100;
            float mitigation = StatConversion.GetArmorDamageReduction(character.Level, calcOpts.BossArmor, stats.ArmorPenetration, arpBuffs, stats.ArmorPenetrationRating);
            mitigation = 1 - mitigation;
            float MHDPS = 0f, OHDPS = 0f;

            #region White Dmg
            {
                
                #region Main Hand
                {
                    
                    float dpsMHglancing = (StatConversion.WHITE_GLANCE_CHANCE_CAP[targetLevel - 80] * combatTable.MH.DPS) * 0.7f;
                    float dpsMHBeforeArmor = ((combatTable.MH.DPS * (1f - calcs.AvoidedAttacks - StatConversion.WHITE_GLANCE_CHANCE_CAP[targetLevel - 80])) * (1f + combatTable.physCrits)) + dpsMHglancing;
                    dpsWhiteMinusGlancing = dpsMHBeforeArmor - dpsMHglancing;
                    dpsWhiteBeforeArmor = dpsMHBeforeArmor;
                    MHDPS = dpsMHBeforeArmor * mitigation;
                }
                #endregion

                #region Off Hand
                if (DW || (character.MainHand == null && character.OffHand != null))
                {
                    float dpsOHglancing = (StatConversion.WHITE_GLANCE_CHANCE_CAP[targetLevel - 80] * combatTable.OH.DPS) * 0.7f;
                    float dpsOHBeforeArmor = ((combatTable.OH.DPS * (1f - calcs.AvoidedAttacks - StatConversion.WHITE_GLANCE_CHANCE_CAP[targetLevel - 80])) * (1f + combatTable.physCrits)) + dpsOHglancing;
                    dpsOHBeforeArmor *= OHMult;
                    dpsWhiteMinusGlancing += dpsOHBeforeArmor - dpsOHglancing;
                    dpsWhiteBeforeArmor += dpsOHBeforeArmor;
                    OHDPS = dpsOHBeforeArmor * mitigation;

                }
                #endregion

                dpsWhite = MHDPS + OHDPS;
            }
            #endregion

            #region Necrosis
            {
                dpsNecrosis = dpsWhiteMinusGlancing * (.04f * (float)talents.Necrosis); // doesn't proc off Glancings
            }
            #endregion

            #region Blood Caked Blade
            {
                float dpsMHBCB = 0f;
                if (combatTable.MH.damage != 0)
                {
                    float MHBCBDmg = (float)(combatTable.MH.damage * (.25f + .125f * calcOpts.rotation.AvgDiseaseMult));
                    dpsMHBCB = MHBCBDmg / combatTable.MH.hastedSpeed;
                }
                dpsBCB = dpsMHBCB;
                dpsBCB *= .1f * (float)talents.BloodCakedBlade;
            }
            #endregion

            #region Death Coil
            {
                if (calcOpts.rotation.DeathCoil > 0f)
                {
                    float DCCD = 1;
                    float DCDmg = 443f + (DeathCoilAPMult * stats.AttackPower);
                    dpsDeathCoil = DCDmg / DCCD;
                    float DCCritDmgMult = .5f * (2f + stats.BonusCritMultiplier);
                    float DCCrit = 1f + ((combatTable.spellCrits + stats.BonusDeathCoilCrit) * DCCritDmgMult);
                    dpsDeathCoil *= DCCrit;

                    dpsDeathCoil *= 1f + (.05f * (float)talents.Morbidity);
                }
            }
            #endregion

            #region Icy Touch
            {
                if (calcOpts.rotation.IcyTouch > 0f)
                {
                    float ITCD = 1;
                    float ITDmg = 236f + (IcyTouchAPMult * stats.AttackPower) + stats.BonusIcyTouchDamage;
                    ITDmg *= 1f + .1f * (float)talents.ImprovedIcyTouch;
                    dpsIcyTouch = ITDmg / ITCD;
                    float ITCritDmgMult = .5f * (2f + stats.CritBonusDamage + stats.BonusCritMultiplier);
                    float ITCrit = 1f + (Math.Min((combatTable.spellCrits + (.05f * (float)talents.Rime)), 1f) * ITCritDmgMult);
                    dpsIcyTouch *= ITCrit;
                }
            }
            #endregion

            #region Plague Strike
            {
                if (calcOpts.rotation.PlagueStrike > 0f)
                {
                    float PSCD = 1;
                    float PSDmg = (combatTable.MH.baseDamage + ((stats.AttackPower / 14f) *
                                    combatTable.normalizationFactor)) * 0.5f + 189f;
                    dpsPlagueStrike = PSDmg / PSCD;
                    float PSCritDmgMult = 1f + (.15f * (float)talents.ViciousStrikes) + stats.BonusCritMultiplier;
                    float PSCrit = 1f + ((combatTable.physCrits + (.03f * (float)talents.ViciousStrikes) +
                                        stats.BonusPlagueStrikeCrit) * PSCritDmgMult);

                    dpsPlagueStrike *= PSCrit;
                    dpsPlagueStrike *= 1f + (.1f * (float)talents.Outbreak);
                }

            }
            #endregion

            #region Frost Fever
            {
                if (calcOpts.rotation.IcyTouch > 0f ||
                (talents.GlyphofHowlingBlast && calcOpts.rotation.HowlingBlast > 0f) ||
                (talents.GlyphofDisease && calcOpts.rotation.Pestilence > 0f))
                {
                    float FFDmg = FrostFeverAPMult * stats.AttackPower + 25.6f;
                    dpsFrostFever = FFDmg;
                    dpsFrostFever *= 1.15f;	// Patch 3.2: Diseases hit 15% harder.
                    dpsWPFromFF = dpsFrostFever * combatTable.physCrits;
                }
            }
            #endregion

            #region Blood Plague
            {
                if (calcOpts.rotation.PlagueStrike > 0f || talents.GlyphofDisease)
                {
                    float BPDmg = BloodPlagueAPMult * stats.AttackPower + 31.1f;
                    dpsBloodPlague = BPDmg;
                    dpsBloodPlague *= 1.15f; // Patch 3.2: Diseases hit 15% harder.
                    dpsWPFromBP = dpsBloodPlague * combatTable.physCrits;
                }
            }
            #endregion

            #region Death Strike
            {
                if (calcOpts.rotation.DeathStrike > 0f)
                {
                    float DSCD = 1;
                    // TODO: This should be changed to make use of the new glyph stats:
                    float DSDmg = ((combatTable.MH.baseDamage + ((stats.AttackPower / 14f) * combatTable.normalizationFactor)) * 0.75f) + 222.75f;
                    DSDmg *= 1f + 0.15f * (float)talents.ImprovedDeathStrike;
                    dpsDeathStrike = DSDmg / DSCD;
                    float DSCritDmgMult = 1f + (.15f * (float)talents.MightOfMograine) + stats.BonusCritMultiplier;
                    float DSCrit = 1f + ((combatTable.physCrits +
                        (.03f * (float)talents.ImprovedDeathStrike) +
                        stats.BonusDeathStrikeCrit) * DSCritDmgMult);
                    dpsDeathStrike *= DSCrit;
                }
            }
            #endregion

            #region Heart Strike
            {
                if (talents.HeartStrike > 0 && calcOpts.rotation.HeartStrike > 0f)
                {
                    float HSCD = 1;
                    float HSDmg = ((combatTable.MH.baseDamage + ((stats.AttackPower / 14f) * combatTable.normalizationFactor)) *
                        0.5f) + 368f;
                    HSDmg *= 1f + 0.1f * (float)calcOpts.rotation.AvgDiseaseMult * (1f + stats.BonusPerDiseaseHeartStrikeDamage);
                    dpsHeartStrike = HSDmg / HSCD;
                    //float HSCrit = 1f + combatTable.physCrits + ( .03f * (float)talents.Subversion );
                    float HSCritDmgMult = 1f + (.15f * (float)talents.MightOfMograine) + stats.BonusCritMultiplier;
                    float HSCrit = 1f + ((combatTable.physCrits + (.03f * (float)talents.Subversion)) * HSCritDmgMult);
                    dpsHeartStrike = (dpsHeartStrike) * HSCrit;
                    dpsHeartStrike *= 1f + (.15f * (float)talents.BloodyStrikes);
                }
            }
            #endregion

            float BCBMult = 1f;
            float BloodPlagueMult = 1f;
            float DeathCoilMult = 1f;
            float FrostFeverMult = 1f;
            float HeartStrikeMult = 1f;
            float IcyTouchMult = 1f;
            float NecrosisMult = 1f;
            float DeathStrikeMult = 1f;
            float PlagueStrikeMult = 1f;
            float WhiteMult = 1f;

            float spellPowerMult = 1f + stats.BonusSpellPowerMultiplier;
            float frostSpellPowerMult = 1f + stats.BonusSpellPowerMultiplier + Math.Max((stats.BonusFrostDamageMultiplier - stats.BonusShadowDamageMultiplier), 0f);
            float partialResist = 0.94f;
            float physPowerMult = 1f;

            #region Apply Physical Mitigation
            {
                float physMit = mitigation;
                physMit *= 1f + (!DW ? .02f * talents.TwoHandedWeaponSpecialization : 0f);

                dpsBCB *= physMit;
                dpsHeartStrike *= physMit;
                dpsDeathStrike *= physMit;
                dpsPlagueStrike *= physMit;
                WhiteMult += physPowerMult - 1f;
                BCBMult += physPowerMult - 1f;
                HeartStrikeMult += physPowerMult - 1f;
                DeathStrikeMult += physPowerMult - 1f;
                PlagueStrikeMult += physPowerMult - 1f;
            }
            #endregion

            #region Apply Magical Mitigation
            {
                // some of this applies to necrosis, I wonder if it is ever accounted for
                float magicMit = partialResist /** combatTable.spellResist*/;
                // magicMit = 1f - magicMit;

                dpsNecrosis *= magicMit;
                dpsBloodPlague *= magicMit;
                dpsDeathCoil *= magicMit * (1f - combatTable.spellResist);
                dpsFrostFever *= magicMit;
                dpsIcyTouch *= magicMit;


                NecrosisMult += spellPowerMult - 1f;
                BloodPlagueMult += spellPowerMult - 1f;
                DeathCoilMult += spellPowerMult - 1f;
                FrostFeverMult += frostSpellPowerMult - 1f;
                IcyTouchMult += frostSpellPowerMult - 1f;
            }
            #endregion


            #region Apply Multi-Ability Talent Multipliers
            {
                float BloodyVengeanceMult = .03f * (float)talents.BloodyVengeance;
                BCBMult *= 1 + BloodyVengeanceMult;
                HeartStrikeMult *= 1 + BloodyVengeanceMult;
                DeathStrikeMult *= 1 + BloodyVengeanceMult;
                PlagueStrikeMult *= 1 + BloodyVengeanceMult;
                WhiteMult *= 1 + BloodyVengeanceMult;

                float HysteriaCoeff = .2f / 2f; // current uptime is 50% for now
                float HysteriaMult = HysteriaCoeff * (float)talents.Hysteria;
                BCBMult *= 1 + HysteriaMult;
                HeartStrikeMult *= 1 + HysteriaMult;
                DeathStrikeMult *= 1 + HysteriaMult;
                PlagueStrikeMult *= 1 + HysteriaMult;
                WhiteMult *= 1 + HysteriaMult;

                float BlackIceMult = .02f * (float)talents.BlackIce;
                FrostFeverMult *= 1 + BlackIceMult;
                IcyTouchMult *= 1 + BlackIceMult;
                DeathCoilMult *= 1 + BlackIceMult;
                BloodPlagueMult *= 1 + BlackIceMult;

                float CryptFeverMult = .1f * (float)talents.CryptFever;
                float CryptFeverBuff = stats.BonusDiseaseDamageMultiplier;
                CryptFeverMult = Math.Max(CryptFeverMult, CryptFeverBuff);
                FrostFeverMult *= 1 + CryptFeverMult;
                BloodPlagueMult *= 1 + CryptFeverMult;

            }
            #endregion



            dpsDancingRuneWeapon = dpsWhite * WhiteMult * 17f + 
                dpsBCB * BCBMult * 17f + 
                dpsNecrosis * NecrosisMult * 17f + 
                dpsDeathCoil * DeathCoilMult + 
                dpsIcyTouch * IcyTouchMult + 
                dpsPlagueStrike * PlagueStrikeMult + 
                dpsFrostFever * (5f + talents.Epidemic) * FrostFeverMult + 
                dpsBloodPlague * (5f + talents.Epidemic) * BloodPlagueMult + 
                dpsDeathStrike * DeathStrikeMult * (talents.GlyphofDancingRuneWeapon ? 1f : 0f) + 
                dpsHeartStrike * HeartStrikeMult * (5f + (talents.GlyphofDancingRuneWeapon ? 2f : 0f));
            dpsDancingRuneWeapon *= 0.5f;
        }
    }
}
