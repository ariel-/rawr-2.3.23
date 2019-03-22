using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.DPSDK
{
    class UnholyCycle
    {
    int currentTime;
        int GCDTime;
        int spellGCD;
        int meleeGCD;
        int MHFrequency;
        int RuneCD;
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
        double SpellGCDMultiplier;
        AbilityHandler abilities;
        public UnholyCycle(Character c, CombatTable t, Stats s, CalculationOptionsDPSDK opts, AbilityHandler a)
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
            DeathRune1 = 100*60*1000 + 1;
            DeathRune2 = 100*60*1000 + 1;
            horn = 0;
            abilities = a;
            character = c;
            combatTable = t;
            stats = s;
            talents = c.DeathKnightTalents;
            calcOpts = opts;
            if (opts.CurrentPresence == CalculationOptionsDPSDK.Presence.Unholy)
            {
                meleeGCD = 1000;
                spellGCD = 1000;
                RuneCD = (int)(10000 * (1 - talents.ImprovedUnholyPresence * .05));
            }
            else
            {
                meleeGCD = 1500;
                spellGCD = (int)(1500 / ((1 + (StatConversion.GetHasteFromRating(stats.HasteRating, CharacterClass.DeathKnight))) * (1d + stats.SpellHaste)));
                if (spellGCD < 1000) spellGCD = 1000;
                RuneCD = 10000;
            }

            PhysicalGCDMultiplier = (1 / (1 - (combatTable.dodgedSpecial + combatTable.missedSpecial)));
            SpellGCDMultiplier = (1 / (1 - (combatTable.spellResist)));

            MHFrequency = (int)(combatTable.MH.hastedSpeed * 1000);
        }
        public Rotation GetDamage(int fightDuration)
        {


            bool SS = false;
            bool BS = false;
            bool IT = false;
            bool PS = false;
            bool pest = false;
            int BPGoSS = 0;
            int FFGoSS = 0;
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
                                    BloodRune1 = RuneCD + BloodRune1;
                                    if (talents.Reaping == 3)
                                    {
                                        DeathRune1 = BloodRune1;
                                        BloodRune1 = fightDuration;
                                    }
                                pest = true;
                            }
                            else if (BloodRune2 < 0)
                            {
                                BloodRune2 += RuneCD;
                                if (talents.Reaping == 3)
                                {
                                    DeathRune2 = BloodRune2;
                                    BloodRune2 = fightDuration;                                    
                                }
                                pest = true;
                            }
                            else if (DeathRune1 < 0)
                            {
                                BloodRune1 = RuneCD + DeathRune1;
                                DeathRune1 = fightDuration + 1;
                                if (talents.Reaping == 3)
                                {
                                    DeathRune1 = BloodRune1;
                                    BloodRune1 = fightDuration;
                                    
                                }
                                pest = true;
                            }
                            else if (DeathRune2 < 0)
                            {
                                BloodRune2 = DeathRune2 + RuneCD;
                                DeathRune2 = fightDuration + 1;
                                if (talents.Reaping == 3)
                                {
                                    DeathRune2 = BloodRune2;
                                    BloodRune2 = fightDuration;
                                }
                                pest = true;
                            }
                        }
                        #endregion
                    }

                    if ((FF <= 0 && (FrostRune1 < 0 || FrostRune2 < 0 || DeathRune1 < 0 || DeathRune2 < 0)) ||
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
                            FrostRune1 += RuneCD;
                        }
                        else if (FrostRune2 < 0)
                        {
                            // IT
                            IT = true;
                            FrostRune2 += RuneCD;
                        }
                        else if (DeathRune1 < 0)
                        {
                            // IT
                            IT = true;
                            BloodRune1 = DeathRune1 + RuneCD;
                            DeathRune1 = fightDuration;
                            IT = true;
                        }
                        else if (DeathRune2 < 0)
                        {
                            // IT
                            IT = true;
                            BloodRune2 = DeathRune2 + RuneCD;
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
                            UnholyRune1 += RuneCD;
                        }
                        else if (UnholyRune2 < 0)
                        {
                            // PS
                            PS = true;
                            UnholyRune2 += RuneCD;
                        }
                        else if (DeathRune1 < 0)
                        {
                            // PS
                            PS = true;
                            BloodRune1 = DeathRune1 + RuneCD;
                            DeathRune1 = fightDuration;
                        }
                        else if (DeathRune2 < 0)
                        {
                            // PS
                            PS = true;
                            BloodRune2 = DeathRune2 + RuneCD;
                            DeathRune2 = fightDuration;
                        }
                        #endregion
                    }
                    #region Scourge Strike
                    else if (FrostRune1 < 0 && UnholyRune1 < 0)
                    {
                        FrostRune1 += RuneCD;
                        UnholyRune1 += RuneCD;
                        SS = true;
                    }
                    else if (FrostRune2 < 0 && UnholyRune2 < 0)
                    {
                        FrostRune2 += RuneCD;
                        UnholyRune2 += RuneCD;
                        SS = true;
                    }
                    else if (FrostRune1 < 0 && UnholyRune2 < 0)
                    {
                        FrostRune1 += RuneCD;
                        UnholyRune2 += RuneCD;
                        SS = true;
                    }
                    else if (FrostRune1 < 0 && DeathRune1 < 0)
                    {
                        FrostRune1 += RuneCD;
                        BloodRune1 = DeathRune1 + RuneCD;
                        DeathRune1 = fightDuration + 1;
                        SS = true;
                    }
                    else if (FrostRune1 < 0 && DeathRune2 < 0)
                    {
                        FrostRune1 += RuneCD;
                        BloodRune2 = DeathRune2 + RuneCD;
                        DeathRune2 = fightDuration + 1;
                        SS = true;
                    }
                    else if (FrostRune2 < 0 && UnholyRune1 < 0)
                    {
                        FrostRune2 += RuneCD;
                        UnholyRune1 += RuneCD;
                        SS = true;
                    }
                    else if (FrostRune2 < 0 && DeathRune1 < 0)
                    {
                        FrostRune2 += RuneCD;
                        BloodRune1 = DeathRune1 + RuneCD;
                        DeathRune1 = fightDuration + 1;
                        SS = true;
                    }
                    else if (FrostRune2 < 0 && DeathRune2 < 0)
                    {
                        FrostRune2 = RuneCD;
                        BloodRune2 = DeathRune2 + RuneCD;
                        DeathRune2 = fightDuration + 1;
                        SS = true;
                    }

                    else if (DeathRune1 < 0 && UnholyRune1 < 0)
                    {
                        BloodRune1 = RuneCD + DeathRune1;
                        DeathRune1 = fightDuration + 1;
                        UnholyRune1 += RuneCD;
                        SS = true;
                    }
                    else if (DeathRune1 < 0 && UnholyRune2 < 0)
                    {
                        BloodRune1 = DeathRune1 + RuneCD;
                        DeathRune1 = fightDuration + 1;
                        UnholyRune2 += RuneCD;
                        SS = true;
                    }
                    else if (DeathRune1 < 0 && DeathRune2 < 0)
                    {
                        BloodRune1 = DeathRune1 + RuneCD;
                        DeathRune1 = fightDuration + 1;
                        BloodRune2 = DeathRune2 + RuneCD;
                        DeathRune2 = fightDuration + 1;
                        SS = true;
                    }

                    #endregion

                    #region Blood Strike
                    else if (BloodRune1 < 0)
                    {
                        if (talents.BloodOfTheNorth == 5 || talents.Reaping == 3)
                        {
                            DeathRune1 = BloodRune1 + RuneCD;
                            BloodRune1 = fightDuration + 1;
                        }
                        else
                        {
                            BloodRune1 += RuneCD;
                        }
                        BS = true;
                    }
                    else if (BloodRune2 < 0)
                    {
                        if (talents.BloodOfTheNorth == 5 || talents.Reaping == 3)
                        {
                            DeathRune2 = BloodRune2 + RuneCD;
                            BloodRune2 = fightDuration + 1;
                        }
                        else
                        {
                            BloodRune2 += RuneCD;
                        }
                        BS = true;
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
                if (SS)
                {
                    //SS
                    GCDTime = (int)(meleeGCD * PhysicalGCDMultiplier);
                    RP += 15 + talents.Dirge * 2.5;
                    occurence.ScourgeStrike++;
                    SS = false;
                    if (talents.GlyphofScourgeStrike)
                    {
                        if (BPGoSS < 3 && BP > 0)
                        {
                            BP += 3000;
                            BPGoSS++;
                        }
                        if (FFGoSS < 3 && FF > 0)
                        {
                            FF += 3000;
                            FFGoSS++;
                        }
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
                    RP += 10;
                    occurence.Pestilence++;
                    pest = false;
                }
                if (IT)
                {
                    FF = 15000 + talents.Epidemic * 3000 - 1;
                    FFGoSS = 0;
                    IT = false;
                    RP += 10 + talents.ChillOfTheGrave * 2.5;
                    GCDTime = (int)(SpellGCDMultiplier * spellGCD);
                    occurence.IcyTouch++;
                }
                if (PS)
                {
                    BP = 15000 + talents.Epidemic * 3000 - 1;
                    BPGoSS = 0;
                    GCDTime = (int)(meleeGCD * PhysicalGCDMultiplier);
                    occurence.PlagueStrike++;
                    RP += 10 + talents.Dirge * 2.5;
                    PS = false;
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
