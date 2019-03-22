using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.DPSDK
{
    class BloodCycle
{
        int currentTime;
        int GCDTime;
        int spellGCD;
        int meleeGCD;
        int MHFrequency;
        Rotation occurence;
        Character character;
        CombatTable combatTable;
        DeathKnightTalents talents;
        Stats stats;
        CalculationOptionsDPSDK calcOpts;
        double RP;
        int horn;
        int BP, FF;
        int FrostRune1, FrostRune2, UnholyRune1, UnholyRune2, BloodRune1, BloodRune2, DeathUnholyRune1, DeathUnholyRune2, DeathFrostRune1, DeathFrostRune2;
        double PhysicalGCDMultiplier;
        double SpellGCDMultiplier;
        public BloodCycle(Character c, CombatTable t, Stats s, CalculationOptionsDPSDK opts, AbilityHandler a)
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
            
            RP = 0;
            FrostRune1 = 0;
            FrostRune2 = 0;
            UnholyRune1 = 0;
            UnholyRune2 = 0;
            BloodRune1 = 0;
            BloodRune2 = 0;
            DeathUnholyRune1 = 100 * 60 * 1000 * 100;
            DeathUnholyRune2 = 100 * 60 * 1000 * 100;
            DeathFrostRune1 = 100 * 60 * 1000 * 100;
            DeathFrostRune2 = 100 * 60 * 1000 * 100;
            horn = 0;
            character = c;
            combatTable = t;
            stats = s;
            talents = c.DeathKnightTalents;
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
                if (spellGCD < 1000) spellGCD = 1000;
            }

            PhysicalGCDMultiplier = (1 / (1 - (combatTable.dodgedSpecial + combatTable.missedSpecial)));
            SpellGCDMultiplier = (1 / (1 - (combatTable.spellResist)));

            MHFrequency = (int)(combatTable.MH.hastedSpeed * 1000);
        }
        public Rotation GetDamage(int fightDuration)
        {


            bool DS = false;
            bool HS = false;
            bool pest = false;
            bool IT = false;
            bool PS = false;
            while (currentTime < fightDuration)
            {
                #region Priority Queue
                if (GCDTime <= 0)
                {
                    if (talents.GlyphofDisease)
                    {
                        #region GoD
                        if (((FF < meleeGCD * PhysicalGCDMultiplier || BP < meleeGCD * PhysicalGCDMultiplier) ||
                            (FF < BloodRune1 && FF < BloodRune2 && FF < DeathFrostRune1 && FF < DeathFrostRune2 && FF < DeathUnholyRune1 && FF > DeathUnholyRune2) ||
                            (FF < BloodRune1 && FF < BloodRune2 && FF < DeathFrostRune1 && FF < DeathFrostRune2 && FF > DeathUnholyRune1 && FF < DeathUnholyRune2) ||
                            (FF < BloodRune1 && FF < BloodRune2 && FF < DeathFrostRune1 && FF > DeathFrostRune2 && FF < DeathUnholyRune1 && FF < DeathUnholyRune2) ||
                            (FF < BloodRune1 && FF < BloodRune2 && FF > DeathFrostRune1 && FF < DeathFrostRune2 && FF < DeathUnholyRune1 && FF < DeathUnholyRune2) ||
                            (FF < BloodRune1 && FF > BloodRune2 && FF < DeathFrostRune1 && FF < DeathFrostRune2 && FF < DeathUnholyRune1 && FF < DeathUnholyRune2) ||
                            (FF > BloodRune1 && FF < BloodRune2 && FF < DeathFrostRune1 && FF < DeathFrostRune2 && FF < DeathUnholyRune1 && FF < DeathUnholyRune2) ||

                            (BP < BloodRune1 && BP < BloodRune2 && BP < DeathFrostRune1 && BP < DeathFrostRune2 && BP < DeathUnholyRune1 && BP > DeathUnholyRune2) ||
                            (BP < BloodRune1 && BP < BloodRune2 && BP < DeathFrostRune1 && BP < DeathFrostRune2 && BP > DeathUnholyRune1 && BP < DeathUnholyRune2) ||
                            (BP < BloodRune1 && BP < BloodRune2 && BP < DeathFrostRune1 && BP > DeathFrostRune2 && BP < DeathUnholyRune1 && BP < DeathUnholyRune2) ||
                            (BP < BloodRune1 && BP < BloodRune2 && BP > DeathFrostRune1 && BP < DeathFrostRune2 && BP < DeathUnholyRune1 && BP < DeathUnholyRune2) ||
                            (BP < BloodRune1 && BP > BloodRune2 && BP < DeathFrostRune1 && BP < DeathFrostRune2 && BP < DeathUnholyRune1 && BP < DeathUnholyRune2) ||
                            (BP > BloodRune1 && BP < BloodRune2 && BP < DeathFrostRune1 && BP < DeathFrostRune2 && BP < DeathUnholyRune1 && BP < DeathUnholyRune2))
                            && (FF > 0 && BP > 0))
                        {
                            if (BloodRune1 <= 0)
                            {
                                BloodRune1 += 10000;
                                pest = true;
                            }
                            else if (BloodRune2 <= 0)
                            {
                                BloodRune2 += 10000;
                                pest = true;
                            }
                            else if (DeathFrostRune1 <= 0)
                            {
                                FrostRune1 = DeathFrostRune1 + 10000;
                                DeathFrostRune1 = fightDuration + 1;
                                pest = true;
                            }
                            else if (DeathFrostRune2 <= 0)
                            {
                                FrostRune2 = DeathFrostRune2 + 10000;
                                DeathUnholyRune2 = fightDuration + 1;
                                pest = true;
                            }
                            else if (DeathUnholyRune1 <= 0)
                            {
                                UnholyRune1 = DeathUnholyRune1 + 10000;
                                DeathUnholyRune1 = fightDuration + 1;
                                pest = true;
                            }
                            else if (DeathUnholyRune2 <= 0)
                            {
                                UnholyRune2 = DeathUnholyRune2 + 10000;
                                DeathUnholyRune2 = fightDuration + 1;
                                pest = true;
                            }
                        }
                        #endregion

                        else if (FF < spellGCD * SpellGCDMultiplier && (FrostRune1 <= 0 || FrostRune2 <= 0 || DeathFrostRune1 <= 0 || DeathFrostRune2 <= 0 || DeathUnholyRune1 <= 0 || DeathUnholyRune2 <= 0))
                        {
                            #region FF
                            if (FrostRune1 <= 0)
                            {
                                IT = true;
                                FrostRune1 += 10000;
                            }
                            else if (FrostRune2 <= 0)
                            {
                                IT = true;
                                FrostRune2 += 10000;
                            }
                            else if (DeathFrostRune1 <= 0)
                            {
                                IT = true;
                                FrostRune1 = DeathFrostRune1 + 10000;
                                DeathFrostRune1 = fightDuration + 1;

                            }
                            else if (DeathFrostRune2 <= 0)
                            {
                                IT = true;
                                FrostRune2 = DeathFrostRune2 + 10000;
                                DeathFrostRune2 = fightDuration + 1;

                            }
                            else if (DeathUnholyRune1 <= 0)
                            {
                                IT = true;
                                UnholyRune1 = DeathUnholyRune2 + 10000;
                                DeathUnholyRune1 = fightDuration + 1;

                            }
                            else if (DeathUnholyRune2 <= 0)
                            {
                                IT = true;
                                UnholyRune2 = DeathUnholyRune2 + 10000;
                                DeathUnholyRune2 = fightDuration + 1;

                            }
                            #endregion
                        }
                        else if (BP < meleeGCD * PhysicalGCDMultiplier && (UnholyRune1 <= 0 || UnholyRune2 <= 0 || DeathFrostRune1 <= 0 || DeathFrostRune2 <= 0 || DeathUnholyRune1 <= 0 || DeathUnholyRune2 <= 0))
                        {
                            #region BP
                            if (UnholyRune1 <= 0)
                            {
                                PS = true;
                                UnholyRune1 += 10000;
                            }
                            else if (UnholyRune2 <= 0)
                            {
                                PS = true;
                                UnholyRune2 += 10000;
                            }
                            else if (DeathUnholyRune1 <= 0)
                            {
                                PS = true;
                                UnholyRune1 = DeathUnholyRune1 + 10000;
                                DeathUnholyRune1 = fightDuration + 1;
                            }
                            else if (DeathUnholyRune2 <= 0)
                            {
                                PS = true;
                                UnholyRune2 = DeathUnholyRune2 + 10000;
                                DeathUnholyRune2 = fightDuration + 1;
                            }
                            else if (DeathFrostRune1 <= 0)
                            {
                                PS = true;
                                FrostRune1 = DeathFrostRune1 + 10000;
                                DeathFrostRune1 = fightDuration + 1;
                            }
                            else if (DeathFrostRune2 <= 0)
                            {
                                PS = true;
                                FrostRune2 = DeathFrostRune2 + 10000;
                                DeathFrostRune2 = fightDuration + 1;
                            }

                            #endregion
                        }

                    }
                    else
                    {
                        if ((FF <= spellGCD * SpellGCDMultiplier && (FrostRune1 <= 0 || FrostRune2 <= 0 || DeathFrostRune1 <= 0 || DeathFrostRune2 <= 0 || DeathUnholyRune1 <= 0 || DeathUnholyRune2 <= 0)) ||
                            (FF < FrostRune1 && FF < FrostRune2 && FF < DeathFrostRune1 && FF < DeathFrostRune2 && FF < DeathUnholyRune1 && FF > DeathUnholyRune2) ||
                            (FF < FrostRune1 && FF < FrostRune2 && FF < DeathFrostRune1 && FF < DeathFrostRune2 && FF > DeathUnholyRune1 && FF < DeathUnholyRune2) ||
                            (FF < FrostRune1 && FF < FrostRune2 && FF < DeathFrostRune1 && FF > DeathFrostRune2 && FF < DeathUnholyRune1 && FF < DeathUnholyRune2) ||
                            (FF < FrostRune1 && FF < FrostRune2 && FF > DeathFrostRune1 && FF < DeathFrostRune2 && FF < DeathUnholyRune1 && FF < DeathUnholyRune2) ||
                            (FF < FrostRune1 && FF > FrostRune2 && FF < DeathFrostRune1 && FF < DeathFrostRune2 && FF < DeathUnholyRune1 && FF < DeathUnholyRune2) ||
                            (FF > FrostRune1 && FF < FrostRune2 && FF < DeathFrostRune1 && FF < DeathFrostRune2 && FF < DeathUnholyRune1 && FF < DeathUnholyRune2))
                        {
                            #region FF
                            if (FrostRune1 <= 0)
                            {
                                IT = true;
                                FrostRune1 += 10000;
                            }
                            else if (FrostRune2 <= 0)
                            {
                                IT = true;
                                FrostRune2 += 10000;
                            }
                            else if (DeathFrostRune1 <= 0)
                            {
                                IT = true;
                                FrostRune1 = DeathFrostRune1 + 10000;
                                DeathFrostRune1 = fightDuration + 1;

                            }
                            else if (DeathFrostRune2 <= 0)
                            {
                                IT = true;
                                FrostRune2 = DeathFrostRune2 + 10000;
                                DeathFrostRune2 = fightDuration + 1;

                            }
                            else if (DeathUnholyRune1 <= 0)
                            {
                                IT = true;
                                UnholyRune1 = DeathUnholyRune2 + 10000;
                                DeathUnholyRune1 = fightDuration + 1;

                            }
                            else if (DeathUnholyRune2 <= 0)
                            {
                                IT = true;
                                UnholyRune2 = DeathUnholyRune2 + 10000;
                                DeathUnholyRune2 = fightDuration + 1;

                            }
                            #endregion
                        }
                        else if ((BP <= meleeGCD * PhysicalGCDMultiplier && (UnholyRune1 <= 0 || UnholyRune2 <= 0 || DeathFrostRune1 <= 0 || DeathFrostRune2 <= 0 || DeathUnholyRune1 <= 0 || DeathUnholyRune2 <= 0)) ||
                            (BP < UnholyRune1 && BP < UnholyRune2 && BP < DeathFrostRune1 && BP < DeathFrostRune2 && BP < DeathUnholyRune1 && BP > DeathUnholyRune2) ||
                            (BP < UnholyRune1 && BP < UnholyRune2 && BP < DeathFrostRune1 && BP < DeathFrostRune2 && BP > DeathUnholyRune1 && BP < DeathUnholyRune2) ||
                            (BP < UnholyRune1 && BP < UnholyRune2 && BP < DeathFrostRune1 && BP > DeathFrostRune2 && BP < DeathUnholyRune1 && BP < DeathUnholyRune2) ||
                            (BP < UnholyRune1 && BP < UnholyRune2 && BP > DeathFrostRune1 && BP < DeathFrostRune2 && BP < DeathUnholyRune1 && BP < DeathUnholyRune2) ||
                            (BP < UnholyRune1 && BP > UnholyRune2 && BP < DeathFrostRune1 && BP < DeathFrostRune2 && BP < DeathUnholyRune1 && BP < DeathUnholyRune2) ||
                            (BP > UnholyRune1 && BP < UnholyRune2 && BP < DeathFrostRune1 && BP < DeathFrostRune2 && BP < DeathUnholyRune1 && BP < DeathUnholyRune2))
                        {
                            #region BP
                            if (UnholyRune1 <= 0)
                            {
                                PS = true;
                                UnholyRune1 += 10000;
                            }
                            else if (UnholyRune2 <= 0)
                            {
                                PS = true;
                                UnholyRune2 += 10000;
                            }
                            else if (DeathUnholyRune1 <= 0)
                            {
                                PS = true;
                                UnholyRune1 = DeathUnholyRune1 + 10000;
                                DeathUnholyRune1 = fightDuration + 1;
                            }
                            else if (DeathUnholyRune2 <= 0)
                            {
                                PS = true;
                                UnholyRune2 = DeathUnholyRune2 + 10000;
                                DeathUnholyRune2 = fightDuration + 1;
                            }
                            else if (DeathFrostRune1 <= 0)
                            {
                                PS = true;
                                FrostRune1 = DeathFrostRune1 + 10000;
                                DeathFrostRune1 = fightDuration + 1;
                            }
                            else if (DeathFrostRune2 <= 0)
                            {
                                PS = true;
                                FrostRune2 = DeathFrostRune2 + 10000;
                                DeathFrostRune2 = fightDuration + 1;
                            }

                            #endregion
                        }
                    }
                    #region HS
                    if (BloodRune1 <= 0)
                    {
                        HS = true;
                        BloodRune1 += 10000;
                    }
                    else if (BloodRune2 <= 0)
                    {
                        HS = true;
                        BloodRune2 += 10000;
                    }
                    else if (DeathFrostRune1 <= 0)
                    {
                        HS = true;
                        FrostRune1 = DeathFrostRune1 + 10000;
                        DeathFrostRune1 = fightDuration + 1;
                    }
                    else if (DeathFrostRune2 <= 0)
                    {
                        HS = true;
                        FrostRune2 = DeathFrostRune2 + 10000;
                        DeathFrostRune2 = fightDuration + 1;
                    }
                    else if (DeathUnholyRune1 <= 0)
                    {
                        HS = true;
                        UnholyRune1 = DeathUnholyRune1 + 10000;
                        DeathUnholyRune1 = fightDuration + 1;
                    }
                    else if (DeathUnholyRune2 <= 0)
                    {
                        HS = true;
                        UnholyRune2 = DeathUnholyRune2 + 10000;
                        DeathUnholyRune2 = fightDuration + 1;
                    }
                    #endregion
                    #region Death Strike
                    else if (FrostRune1 <= 0 && UnholyRune1 <= 0)
                    {
                        FrostRune1 += 10000;
                        UnholyRune1 += 10000;
                        if (talents.DeathRuneMastery == 3)
                        {
                            DeathFrostRune1 = FrostRune1;
                            FrostRune1 = fightDuration + 1;
                            DeathUnholyRune1 = UnholyRune1;
                            UnholyRune1 = fightDuration + 1;
                        }
                        DS = true;
                    }
                    else if (FrostRune1 <= 0 && UnholyRune2 <= 0)
                    {
                        FrostRune1 += 10000;
                        UnholyRune2 += 10000;
                        if (talents.DeathRuneMastery == 3)
                        {
                            DeathFrostRune1 = FrostRune1;
                            FrostRune1 = fightDuration + 1;
                            DeathUnholyRune2 = UnholyRune2;
                            UnholyRune2 = fightDuration + 1;
                        }
                        DS = true;
                    }
                    else if (FrostRune2 <= 0 && UnholyRune1 <= 0)
                    {
                        FrostRune2 += 10000;
                        UnholyRune1 += 10000;
                        if (talents.DeathRuneMastery == 3)
                        {
                            DeathFrostRune2 = FrostRune2;
                            FrostRune2 = fightDuration + 1;
                            DeathUnholyRune1 = UnholyRune1;
                            UnholyRune1 = fightDuration + 1;
                        }
                        DS = true;
                    }
                    else if (FrostRune2 <= 0 && UnholyRune2 <= 0)
                    {
                        FrostRune2 += 10000;
                        UnholyRune2 += 10000;
                        if (talents.DeathRuneMastery == 3)
                        {
                            DeathFrostRune2 = FrostRune2;
                            FrostRune2 = fightDuration + 1;
                            DeathUnholyRune2 = UnholyRune2;
                            UnholyRune2 = fightDuration + 1;
                        }
                        DS = true;
                    }

                    #endregion

                    else if (RP > 40)
                    {
                        #region DC
                        occurence.DeathCoil++;
                        GCDTime = (int)(spellGCD);
                        RP -= 40;
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
                if (pest)
                {
                    FF = (FF % 3000) + 15000 + talents.Epidemic * 3000;
                    BP = (BP % 3000) + 15000 + talents.Epidemic * 3000;
                    GCDTime = (int)(spellGCD * SpellGCDMultiplier);
                    occurence.Pestilence++;
                    pest = false;
                    RP += 10;
                }
                else if (IT)
                {
                    FF = 15000 + talents.Epidemic * 3000 - 1;
                    IT = false;
                    GCDTime = (int)(SpellGCDMultiplier * spellGCD);
                    occurence.IcyTouch++;
                    RP += 10 + talents.ChillOfTheGrave * 2.5;
                }
                else if (PS)
                {
                    BP = 15000 + talents.Epidemic * 3000 - 1;
                    GCDTime = (int)(meleeGCD * PhysicalGCDMultiplier);
                    occurence.PlagueStrike++;
                    RP += 10 + talents.Dirge;
                    PS = false;
                }
                else if (DS)
                {
                    //Death Strike
                    GCDTime = (int)(meleeGCD * PhysicalGCDMultiplier);
                    RP += 15 + talents.Dirge * 2.5;
                    occurence.DeathStrike++;
                    DS = false;
                }
                else if (HS)
                {
                    // Blood Strike
                    GCDTime = (int)(meleeGCD * PhysicalGCDMultiplier);
                    RP += 10;
                    occurence.HeartStrike++;
                    occurence.DeathCoil += talents.SuddenDoom * .05;
                    HS = false;
                }

                // Kavan: in Rawr3 MHFrequency can be 0 when still loading weapon data from armory, besides this doesn't do anything
                /*if (currentTime % MHFrequency == 0)
                {
                }*/
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
                DeathFrostRune1--;
                DeathFrostRune2--;
                DeathUnholyRune1--;
                DeathUnholyRune2--;
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
                DeathFrostRune1 = DeathFrostRune1 < -2000 ? -2000 : DeathFrostRune1;
                DeathFrostRune2 = DeathFrostRune2 < -2000 ? -2000 : DeathFrostRune2;
                DeathUnholyRune1 = DeathUnholyRune1 < -2000 ? -2000 : DeathUnholyRune1;
                DeathUnholyRune2 = DeathUnholyRune2 < -2000 ? -2000 : DeathUnholyRune2;
                #endregion
            }
            return occurence;
        }
    }
}
