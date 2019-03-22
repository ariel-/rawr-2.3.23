using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.DPSDK
{
    class FrostCycle
    {
        int currentTime;
        int GCDTime;
        int spellGCD;
        int meleeGCD;
        int MHFrequency;
        double KMChance;
        double RimeChance;
        Rotation occurence;
        Character character;
        CombatTable combatTable;
        DeathKnightTalents talents;
        Stats stats;
        CalculationOptionsDPSDK calcOpts;
        double RP;
        int horn;
        int BP, FF;
        int FrostRune1, FrostRune2, UnholyRune1, UnholyRune2, BloodRune1, BloodRune2, DeathRune1, DeathRune2;
        double PhysicalGCDMultiplier;
        double KMApplicationProbability;
        double RimeApplicationProbability;
        double SpellGCDMultiplier;
        AbilityHandler abilities;
        public FrostCycle(Character c, CombatTable t, Stats s, CalculationOptionsDPSDK opts, AbilityHandler a)
        {
            BP = 0;
            FF = 0;
            currentTime = 0;
            GCDTime = 0;
            occurence = new Rotation();
            //occurence.presence = calcOpts.rotation.presence;
            occurence.setRotation(Rotation.Type.Custom);
            occurence.NumDisease = 0f;
            occurence.DiseaseUptime = 100f;
            occurence.DeathCoil = 0f;
            occurence.IcyTouch = 0f;
            occurence.PlagueStrike = 0f;
            occurence.ScourgeStrike = 0f;
            occurence.ManagedRP = false;
            occurence.FrostStrike = 0f;
            occurence.HowlingBlast = 0f;
            occurence.Obliterate = 0f;
            occurence.BloodStrike = 0f;
            occurence.HeartStrike = 0f;
            occurence.DancingRuneWeapon = 0f;
            occurence.Horn = 0f;
            occurence.GargoyleDuration = 30f;
            occurence.DeathStrike = 0f;
            occurence.GhoulFrenzy = 0f;
            occurence.Pestilence = 0f;
            occurence.CurRotationDuration = 60f;
            KMApplicationProbability = 0;
            
            RP = 0;
            FrostRune1 = 0;
            FrostRune2 = 0;
            UnholyRune1 = 0;
            UnholyRune2 = 0;
            BloodRune1 = 0;
            BloodRune2 = 0;
            DeathRune1 = 100*60*1000 + 1;
            DeathRune2 = 100*60*1000 + 1;
            horn = 0;
            abilities = a;
            character = c;
            combatTable = t;
            stats = s;
            talents = c.DeathKnightTalents;
            KMChance = (1 / (60 / t.MH.baseSpeed)) * talents.KillingMachine * (1 - combatTable.totalMHMiss);
            calcOpts = opts;
            if (opts.CurrentPresence == CalculationOptionsDPSDK.Presence.Unholy)
            {
                meleeGCD = 1000;
                spellGCD = 1000;
            }
            else
            {
                meleeGCD = 1500;
                spellGCD = (int)(1500 / ((1 + (StatConversion.GetHasteFromRating(stats.HasteRating, CharacterClass.DeathKnight))) * (1d + stats.SpellHaste)));
                spellGCD = Math.Max(spellGCD, 1000);
            }

            PhysicalGCDMultiplier = (1 / (1 - (combatTable.dodgedSpecial + combatTable.missedSpecial)));
            SpellGCDMultiplier = (1 / (1 - (combatTable.spellResist)));

            MHFrequency = (int)(combatTable.MH.hastedSpeed * 1000);
            RimeChance = 0.05 * talents.Rime + (combatTable.DW ? (1 - talents.Rime * .05) * (talents.Rime * .05) : 0);
        }
        public Rotation GetDamage(int fightDuration)
        {


            bool Oblit = false;
            bool BS = false;
            bool IT = false;
            bool PS = false;
            bool pest = false;
            while (currentTime < fightDuration)
            {
                #region Priority Queue
                if (GCDTime <= 0)
                {
                    if (talents.GlyphofDisease)
                    {
                        #region GoD
                        if (((FF < meleeGCD * PhysicalGCDMultiplier || BP < meleeGCD * PhysicalGCDMultiplier) ||
                            (FF < BloodRune1 && FF < BloodRune2 && FF < DeathRune1 && FF > DeathRune2) ||
                            (FF < BloodRune1 && FF < BloodRune2 && FF > DeathRune1 && FF < DeathRune2) ||
                            (FF < BloodRune1 && FF > BloodRune2 && FF < DeathRune1 && FF < DeathRune2) ||
                            (FF > BloodRune1 && FF < BloodRune2 && FF < DeathRune1 && FF < DeathRune2) ||

                            (BP < BloodRune1 && BP < BloodRune2 && BP < DeathRune1 && BP > DeathRune2) ||
                            (BP < BloodRune1 && BP < BloodRune2 && BP > DeathRune1 && BP < DeathRune2) ||
                            (BP < BloodRune1 && BP > BloodRune2 && BP < DeathRune1 && BP < DeathRune2) ||
                            (BP > BloodRune1 && BP < BloodRune2 && BP < DeathRune1 && BP < DeathRune2))
                            && (FF > 0 && BP > 0))
                        {
                            if (BloodRune1 < 0)
                            {
                                BloodRune1 += 10000;
                                if (talents.BloodOfTheNorth == 5)
                                {
                                    DeathRune1 = BloodRune1;
                                    BloodRune1 = fightDuration;
                                }
                                pest = true;
                            }
                            else if (BloodRune2 < 0)
                            {
                                BloodRune2 += 10000;
                                if (talents.BloodOfTheNorth == 5)
                                {
                                    DeathRune2 = BloodRune2;
                                    BloodRune2 = fightDuration;
                                }
                                pest = true;
                            }
                            else if (DeathRune1 < 0)
                            {
                                BloodRune1 += 10000;
                                DeathRune1 = fightDuration + 1;
                                if (talents.BloodOfTheNorth == 5)
                                {
                                    DeathRune1 = BloodRune1;
                                    BloodRune1 = fightDuration;
                                }
                                pest = true;
                            }
                            else if (DeathRune2 < 0)
                            {
                                BloodRune2 += 10000;
                                DeathRune2 = fightDuration + 1;
                                if (talents.BloodOfTheNorth == 5)
                                {
                                    DeathRune2 = BloodRune2;
                                    BloodRune2 = fightDuration;
                                }
                                pest = true;
                            }
                        }
                        #endregion
                    }

                    if (RimeApplicationProbability > 0.5 && KMApplicationProbability > 0.5)
                    {
                        #region KMRIME
                        // HB
                        occurence.KMRime += KMApplicationProbability * RimeApplicationProbability;
                        occurence.HowlingBlast += (1 - KMApplicationProbability) * RimeApplicationProbability;
                        KMApplicationProbability = 0;
                        RimeApplicationProbability = 0;
                      //  KM = false;
                        GCDTime = (int)(spellGCD * RimeApplicationProbability);
                        RP += (talents.ChillOfTheGrave * 2.5d) * RimeApplicationProbability;
                       /* if (talents.GlyphofHowlingBlast)
                        {
                            FF = 15000 + talents.Epidemic * 3000 - 1;
                        }*/
                        #endregion
                    }
                    else if (KMApplicationProbability > 0.5 && RP >= (talents.GlyphofFrostStrike ? 32 : 40))
                    {
                        #region KM FS
                        // FS
                        occurence.KMFS += KMApplicationProbability;
                        occurence.FrostStrike += 1 - KMApplicationProbability;
                        KMApplicationProbability = 0;
                       // KM = false;
                        GCDTime = (int)(meleeGCD);
                        RP -= (talents.GlyphofFrostStrike ? 32 : 40);
                        #endregion
                    }
                    else if ((FF <= 0 && (FrostRune1 < 0 || FrostRune2 < 0 || DeathRune1 < 0 || DeathRune2 < 0)) ||
                            (FF < FrostRune1 && FF < FrostRune2 && FF < DeathRune1 && FF > DeathRune2) ||
                            (FF < FrostRune1 && FF < FrostRune2 && FF > DeathRune1 && FF < DeathRune2) ||
                            (FF < FrostRune1 && FF > FrostRune2 && FF < DeathRune1 && FF < DeathRune2) ||
                            (FF > FrostRune1 && FF < FrostRune2 && FF < DeathRune1 && FF < DeathRune2))
                    {
                        #region FF
                        if (FrostRune1 < 0)
                        {
                            // IT
                            IT = true;
                            FrostRune1 += 10000;
                        }
                        else if (FrostRune2 < 0)
                        {
                            // IT
                            IT = true;
                            FrostRune2 += 10000;
                        }
                        else if (DeathRune1 < 0)
                        {
                            // IT
                            IT = true;
                            BloodRune1 = DeathRune1 + 10000;
                            DeathRune1 = fightDuration;
                            IT = true;
                        }
                        else if (DeathRune2 < 0)
                        {
                            // IT
                            IT = true;
                            BloodRune2 = DeathRune2 + 10000;
                            DeathRune2 = fightDuration;
                        }
                        #endregion
                    }
                    else if ((BP <= 0 && (UnholyRune1 < 0 || UnholyRune2 < 0 || DeathRune1 < 0 || DeathRune2 < 0)) ||
                            (BP < UnholyRune1 && BP < UnholyRune2 && BP < DeathRune1 && BP > DeathRune2) ||
                            (BP < UnholyRune1 && BP < UnholyRune2 && BP > DeathRune1 && BP < DeathRune2) ||
                            (BP < UnholyRune1 && BP > UnholyRune2 && BP < DeathRune1 && BP < DeathRune2) ||
                            (BP > UnholyRune1 && BP < UnholyRune2 && BP < DeathRune1 && BP < DeathRune2))
                    {
                        #region BP
                        if (UnholyRune1 < 0)
                        {
                            // PS
                            PS = true;
                            UnholyRune1 += 10000;
                        }
                        else if (UnholyRune2 < 0)
                        {
                            // PS
                            PS = true;
                            UnholyRune2 += 10000;
                        }
                        else if (DeathRune1 < 0)
                        {
                            // PS
                            PS = true;
                            BloodRune1 = DeathRune1 + 10000;
                            DeathRune1 = fightDuration;
                        }
                        else if (DeathRune2 < 0)
                        {
                            // PS
                            PS = true;
                            BloodRune2 = DeathRune2 + 10000;
                            DeathRune2 = fightDuration;
                        }
                        #endregion
                    }
                    #region Obliterate
                    else if (FrostRune1 < 0 && UnholyRune1 < 0)
                    {
                        FrostRune1 += 10000;
                        UnholyRune1 += 10000;
                        Oblit = true;
                    }
                    else if (FrostRune1 < 0 && UnholyRune2 < 0)
                    {
                        FrostRune1 += 10000;
                        UnholyRune2 += 10000;
                        Oblit = true;
                    }
                    else if (FrostRune1 < 0 && DeathRune1 < 0)
                    {
                        FrostRune1 += 10000;
                        BloodRune1 = DeathRune1 + 10000;
                        DeathRune1 = fightDuration + 1;
                        Oblit = true;
                    }
                    else if (FrostRune1 < 0 && DeathRune2 < 0)
                    {
                        FrostRune1 += 10000;
                        BloodRune2 = DeathRune2 + 10000;
                        DeathRune2 = fightDuration + 1;
                        Oblit = true;
                    }
                    else if (FrostRune2 < 0 && UnholyRune1 < 0)
                    {
                        FrostRune2 += 10000;
                        UnholyRune1 += 10000;
                        Oblit = true;
                    }
                    else if (FrostRune2 < 0 && UnholyRune2 < 0)
                    {
                        FrostRune2 += 10000;
                        UnholyRune2 += 10000;
                        Oblit = true;
                    }
                    else if (FrostRune2 < 0 && DeathRune1 < 0)
                    {
                        FrostRune2 += 10000;
                        BloodRune1 = DeathRune1 + 10000;
                        DeathRune1 = fightDuration + 1;
                        Oblit = true;
                    }
                    else if (FrostRune2 < 0 && DeathRune2 < 0)
                    {
                        FrostRune2 += 10000;
                        BloodRune2 = DeathRune2 + 10000;
                        DeathRune2 = fightDuration + 1;
                        Oblit = true;
                    }

                    else if (DeathRune1 < 0 && UnholyRune1 < 0)
                    {
                        BloodRune1 = DeathRune1 + 10000;
                        DeathRune1 = fightDuration + 1;
                        UnholyRune1 += 10000;
                        Oblit = true;
                    }
                    else if (DeathRune1 < 0 && UnholyRune2 < 0)
                    {
                        BloodRune1 = DeathRune1 + 10000;
                        DeathRune1 = fightDuration + 1;
                        UnholyRune2 += 10000;
                        Oblit = true;
                    }
                    else if (DeathRune1 < 0 && DeathRune2 < 0)
                    {
                        BloodRune1 = DeathRune1 + 10000;
                        DeathRune1 = fightDuration + 1;
                        BloodRune2 = DeathRune2 + 10000;
                        DeathRune2 = fightDuration + 1;
                        Oblit = true;
                    }

                    #endregion

                    #region Blood Strike
                    else if (BloodRune1 < 0)
                    {
                        if (talents.BloodOfTheNorth == 5)
                        {
                            DeathRune1 = BloodRune1 + 10000;
                            BloodRune1 = fightDuration + 1;
                        }
                        else
                        {
                            BloodRune1 += 10000;
                        }
                        BS = true;
                    }
                    else if (BloodRune2 < 0)
                    {
                        if (talents.BloodOfTheNorth == 5)
                        {
                            DeathRune2 = BloodRune2 + 10000;
                            BloodRune2 = fightDuration + 1;
                        }
                        else
                        {
                            BloodRune2 += 10000;
                        }
                        BS = true;
                    }
                    #endregion

                    else if (RP > (talents.GlyphofFrostStrike ? 32 : 40))
                    {
                        #region FS
                        // FS
                        //occurence.FrostStrike++;
                        occurence.KMFS += KMApplicationProbability;
                        occurence.FrostStrike += 1 - KMApplicationProbability;
                        KMApplicationProbability = 0;
                        GCDTime = (int)(meleeGCD);
                        RP -= talents.GlyphofFrostStrike ? 32 : 40;
                        #endregion
                    }

                    else if (RimeApplicationProbability > 0)
                    {
                        #region Rime
                        // HB
                        occurence.KMRime += KMApplicationProbability * RimeApplicationProbability;
                        occurence.HowlingBlast += (1 - KMApplicationProbability) * RimeApplicationProbability;
                        KMApplicationProbability = 0;
                        RimeApplicationProbability = 0;
                        RP += (talents.ChillOfTheGrave * 2.5d) * RimeApplicationProbability;
                        GCDTime = (int)(spellGCD * RimeApplicationProbability);
                        /*
                        if (talents.GlyphofHowlingBlast)
                        {
                            FF = 15000 + talents.Epidemic * 3000 - 1;
                        }*/
                        #endregion
                    }
                    else if (horn <= 0)
                    {
                        horn = 20000;
                        occurence.Horn++;
                        GCDTime = spellGCD;
                        RP += 10;
                    }
                }
                #endregion
                if (Oblit)
                {
                    //Obliterate
                    GCDTime = (int)(meleeGCD * PhysicalGCDMultiplier);
                    RP += 15 + talents.ChillOfTheGrave * 2.5;
                    RimeApplicationProbability += (1 - RimeApplicationProbability) * RimeChance;
                    occurence.Obliterate++;
                    Oblit = false;
                    if (talents.Annihilation < 3)
                    {
                        BP = -1;
                        FF = -1;
                    }
                }
                if (BS)
                {
                    // Blood Strike
                    GCDTime = (int)(meleeGCD * PhysicalGCDMultiplier);
                    RP += 10;
                    occurence.BloodStrike++;
                    BS = false;
                }
                if (pest)
                {
                    FF = (FF % 3000) + 15000 + talents.Epidemic * 3000;
                    BP = (BP % 3000) + 15000 + talents.Epidemic * 3000;
                    GCDTime = (int)(spellGCD * SpellGCDMultiplier);
                    occurence.Pestilence++;
                    pest = false;
                    RP += 10;
                }
                if (IT)
                {
                    FF = 15000 + talents.Epidemic * 3000 - 1;
                    IT = false;
                    GCDTime = (int)(SpellGCDMultiplier * spellGCD);
                    occurence.IcyTouch++;
                    RP += 10 + talents.ChillOfTheGrave * 2.5;
                }
                if (PS)
                {
                    BP = 15000 + talents.Epidemic * 3000 - 1;
                    GCDTime = (int)(meleeGCD * PhysicalGCDMultiplier);
                    occurence.PlagueStrike++;
                    PS = false;
                    RP += 10 + talents.Dirge * 2.5;
                }
                // Kavan: in Rawr3 MHFrequency can be 0 when still loading weapon data from armory
                if (MHFrequency > 0 && currentTime % MHFrequency == 0)
                {
                    KMApplicationProbability += (1 - KMApplicationProbability) * KMChance;
                }
                if (FF % 3000 == 0 && FF >= 0)
                {
                    occurence.FFTick++;
                }
                if (BP % 3000 == 0 && BP >= 0)
                {
                    occurence.BPTick++;
                }
                if (currentTime % 5000 == 0)
                {
                    RP += talents.Butchery;
                }
                if (RP > 100 + talents.RunicPowerMastery * 15)
                {
                    RP = 100 + talents.RunicPowerMastery * 15;
                }

                #region time keeping
                currentTime++;
                FrostRune1--;
                FrostRune2--;
                UnholyRune1--;
                UnholyRune2--;
                BloodRune1--;
                BloodRune2--;
                DeathRune1--;
                DeathRune2--;
                horn--;
                GCDTime--;
                FF--;
                BP--;

                BloodRune1 = BloodRune1 < -2000 ? -2000 : BloodRune1;
                BloodRune2 = BloodRune2 < -2000 ? -2000 : BloodRune2;
                FrostRune1 = FrostRune1 < -2000 ? -2000 : FrostRune1;
                FrostRune2 = FrostRune2 < -2000 ? -2000 : FrostRune2;
                UnholyRune1 = UnholyRune1 < -2000 ? -2000 : UnholyRune1;
                UnholyRune2 = UnholyRune2 < -2000 ? -2000 : UnholyRune2;
                DeathRune1 = DeathRune1 < -2000 ? -2000 : DeathRune1;
                DeathRune2 = DeathRune2 < -2000 ? -2000 : DeathRune2;
                #endregion
            }
            return occurence;
        }
    }
}
