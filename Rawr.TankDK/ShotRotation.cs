using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.TankDK {
    public class Rotation {
        // I need FullCharacterStats
        // May need to pull these from the character rather than creating a new stats object here.
        // however these are the stats that are being saved in the XML under the TANKDK area.
        // I only need the Dodge & Parry & Haste Values.
        public float m_fDodge = 0f;
        public float m_fParry = 0f;
        public float m_fPhysicalHaste = 0f;
//        public Stats m_FullStats = new Stats();
        
        public Type curRotationType;
        
        public float curRotationDuration = 0f;  // rotation duration in seconds

        //disease info
        public float avgDiseaseMult = 0f;
        public float numDisease = 0f;
        public float diseaseUptime = 0f;
        
        //ability number of times per rotation used 
        public float DeathCoil = 0f;
        public float IcyTouch = 0f;
        public float PlagueStrike = 0f;
        public float FrostFever = 0f;
        public float BloodPlague = 0f;
        public float ScourgeStrike = 0f;
        public float FrostStrike = 0f;
        public float HowlingBlast = 0f;
        public float Obliterate = 0f;
        public float DeathStrike = 0f;
        public float BloodStrike = 0f;
        public float HeartStrike = 0f;
        public float HornOfWinter = 0f;
        public float Pestilence = 0f;
        public float BloodBoil = 0f;
        public float DeathNDecay = 0f;
        // Defensive Rotations
        public float BoneShield = 0f;
        public float IceBoundFortitude = 0f;
        public float UnbreakableArmor = 0f;
        public float RuneStrike = 0f;

        public Boolean fourT7 = false;
        public Boolean GlyphofIT = false;
        public Boolean GlyphofFS = false;
        public Boolean managedRP = true;
        public float GCDTime;
        public float RP;
        public float[] AbilityCost = new float[4];

        public DeathKnightTalents tTalents;

        public Rotation( ) 
        {
            this.setRotation(this.curRotationType); 
        }

        public Rotation(DeathKnightTalents t)
        {
            if (null != t)
            {
                tTalents = (DeathKnightTalents)t.Clone();
                //            int iHash = 0;
                this.GetRotationByTalents(tTalents);
            }
            else
                this.setRotation(this.curRotationType);

        }

        public enum Type { Custom, Blood, Frost, Unholy }

        public float getMeleeSpecialsPerSecond() 
        {
            float temp;
            temp = PlagueStrike + ScourgeStrike + FrostStrike + Obliterate + DeathStrike + BloodStrike + HeartStrike;
            // As per Bloodysorc, BCB should probably be included in this.
            temp = temp / curRotationDuration;
            return temp;
        }
        public float getSpellSpecialsPerSecond() 
        {
            float temp;
            temp = DeathCoil + IcyTouch + HowlingBlast + DeathNDecay + BloodBoil;
            temp = temp / curRotationDuration;
            return temp;
        }
        public float getRP(DeathKnightTalents talents, Character character)
        {
            fourT7 = character.ActiveBuffsContains("Scourgeborne Battlegear 4 Piece Bonus");
            this.GlyphofFS = talents.GlyphofFrostStrike;

            // Abilities that grant RP:
            RP = ((15 + (fourT7 ? 5 : 0) + 2.5f * talents.ChillOfTheGrave) * Obliterate) +
                 ((15 + (fourT7 ? 5 : 0) + 2.5f * talents.Dirge) * ScourgeStrike) +
                 ((15 + (fourT7 ? 5 : 0) + 2.5f * talents.Dirge) * DeathStrike) +
                 ((10 + 2.5f * talents.Dirge) * PlagueStrike) +
                 ((10 + 2.5f * talents.ChillOfTheGrave) * IcyTouch) +
                ((15 + 2.5f * talents.ChillOfTheGrave) * HowlingBlast) +
                  (15 * DeathNDecay) +
                  (10 * (BloodStrike + HeartStrike + BloodBoil + HornOfWinter)) +
                 ((curRotationDuration / 5f) * talents.Butchery);


            if (managedRP)
            {
                RP = manageRPDumping(talents, RP);
            } 
            else 
            {
                RP -= ((40 * DeathCoil)
                    + ((GlyphofFS ? 32 : 40) * FrostStrike) 
                    + (20 * RuneStrike));
            }
            return RP;
        }

        /// <summary>
        /// Use abilities that have Runic Power to dump the available RP per rotation.
        /// Primarily focused on RuneStrike.
        /// </summary>
        /// <param name="talents">What are the given talents for this character?</param>
        /// <param name="RP">How much RP generated per rotation?</param>
        /// <returns>Amount of RP remaining after use.</returns>
        public float manageRPDumping(DeathKnightTalents talents, float RP)
        {
            // We need to get the # of boss attacks from the mitigation section to 
            // figure out what is the chance the # of RS we can see.
            // PossibleRS = AverageBossAttacksPerRotation * (Dodge% + Parry%)
            // ActualRS = Math.Min(MaxRSBySpeed, PossibleRS);
            // RuneStrikes count is also based on Dodge & Parry chance:
            // RuneStrike *= (m_FullStats.Dodge + m_FullStats.Parry);
            // Remove the RS shots from the pool of available RP.
            // RP -= (RuneStrike * 20f);*/            

            #region Old Code: using FS/DC for RP dumps.
            /*
            if (talents.FrostStrike > 0f) 
            {
                FrostStrike = RP / (talents.GlyphofFrostStrike ? 32f : 40f);
                DeathCoil = 0f;
                RuneStrike = 0f;
                RP = 0f;
            } 
            else 
            {
                DeathCoil = RP / 40f;
                FrostStrike = 0f;
                RuneStrike = 0f;
                RP = 0f;
            }
            */
            #endregion

            // How many RS limited by RP.
            float fMax_ByRP = RP / 20f;
            // How many RS limited by weapon speed.
            // TODO: figure out best way to get weapon speed into the rotation.
            // float fMax_BySpeed = curRotationDuration / MH.Speed;

            // So for right now, we just setup how many RS's we get, and 
            // Zero out all the rest of the values.
            RuneStrike = fMax_ByRP;
            RP = 0;
            FrostStrike = 0;
            DeathCoil = 0;
            RP = 0;


            return RP;
        }

        public float getRotationDuration() 
        {
            float rotDuration = 20;
            float GCDTime = getGCDTime();
            float RuneTime = getRuneTime();
            float RPTime = getRPTime();

            return Math.Max(GCDTime, RuneTime); 
        }

        /// <summary>
        /// Get the total time spent in GCD for the rotation.
        /// </summary>
        /// <returns>float of total GCD in seconds</returns>
        public float getGCDTime() 
        {
            // Melee
            float meleeGCD = 1.5f;

            // Spells / effects
            float spellGCD = GetGCDHasted();
            GCDTime = spellGCD * (
                + DeathCoil // RP
                + IcyTouch // F
                + HowlingBlast // FU
                + HornOfWinter // none
                + DeathNDecay // BFU
                + BloodBoil // B
                + Pestilence // B 
                ) +
                meleeGCD * (
                PlagueStrike // U
                + ScourgeStrike // FU
                + FrostStrike // RP
                + Obliterate // FU 
                + DeathStrike // FU
                + BloodStrike // B
                + HeartStrike // B
                );
            return GCDTime;
        }

        public float getRuneTime()
        {
            float TimeSpentRuneCDs = 0;
            AbilityCost[(int)DKCostTypes.Frost] = (
                ScourgeStrike // FU
                + Obliterate // FU 
                + DeathStrike // FU
                + IcyTouch // F
                + HowlingBlast // FU
                + DeathNDecay // BFU
                );
            AbilityCost[(int)DKCostTypes.UnHoly] = (
                PlagueStrike // U
                + ScourgeStrike // FU
                + Obliterate // FU 
                + DeathStrike // FU
                + HowlingBlast // FU
                + DeathNDecay // BFU
                );
            AbilityCost[(int)DKCostTypes.Blood] = (
                + BloodStrike // B
                + HeartStrike // B
                + DeathNDecay // BFU
                + BloodBoil // B
                + Pestilence // B 
                );
            // Death Runes
            if (null != tTalents)
            {
                if (tTalents.BloodOfTheNorth > 0)
                {
                    AbilityCost[(int)DKCostTypes.Death] = (BloodStrike
                        + Pestilence) * (tTalents.BloodOfTheNorth / 3);
                }
                if (tTalents.Reaping > 0)
                {
                    AbilityCost[(int)DKCostTypes.Death] = (BloodStrike
                        + Pestilence) * (tTalents.Reaping / 3);
                }
                if (tTalents.DeathRuneMastery > 0)
                {
                    AbilityCost[(int)DKCostTypes.Death] += (DeathStrike
                        + Obliterate) * 2 * (tTalents.DeathRuneMastery / 3);
                }

                SpendDeathRunes(AbilityCost, 0);
            }

            float MaxRunes = Math.Max(AbilityCost[(int)DKCostTypes.Frost], AbilityCost[(int)DKCostTypes.Blood]);
            MaxRunes = Math.Max(MaxRunes, AbilityCost[(int)DKCostTypes.UnHoly]);
            MaxRunes = Math.Max(MaxRunes, AbilityCost[(int)DKCostTypes.Death]);

            // Each rune requires 10 secs. But there are 2 of them.
            TimeSpentRuneCDs = (MaxRunes * 10 / 2);

            return TimeSpentRuneCDs;
        }

        /// <summary>
        /// Pass in the AblityCost[] to process the DeathRunes
        /// Stolen from CombatTable2
        /// </summary>
        /// <param name="AbilityCost"></param>
        public void SpendDeathRunes(float[] AbilityCost, int DRSpent)
        {
            // Let's put some boundary conditions 'round here.
            if (AbilityCost == null)
                return;
            // 10 min fight w/ 2 runes available at Death runes every rune cooldown.
            if (Math.Abs(DRSpent) > 120)
                return;
            for (int i = 0; i <= (int)DKCostTypes.Death; i++)
            {
                if (Math.Abs(AbilityCost[i]) > 120)
                    return;
            }
            // Need to figure out how to factor in Death Runes
            // Since each death rune replaces any other rune on the rotation,
            // for each death rune, cut the cost of the highest other rune by 1.
            // Do not run this if there are no DeathRunes to spend.
            if (Math.Abs(AbilityCost[(int)DKCostTypes.Death]) > DRSpent)
            {
                int iHighestCostAbilityIndex = 0;
                float fPreviousCostValue = 0;
                for (int t = 0; t < (int)DKCostTypes.Death; t++)
                {
                    // Is the cost higher than our previous checked value.
                    if (AbilityCost[t] > fPreviousCostValue)
                    {
                        // If so, save off the index of that ability.
                        iHighestCostAbilityIndex = t;
                    }
                    fPreviousCostValue = AbilityCost[t];
                }
                // After going through the full list, spend a death rune and 
                // then iterate through that list again. 
                AbilityCost[iHighestCostAbilityIndex] -= 1;
                // increment the death runes.
                DRSpent++;
                SpendDeathRunes(AbilityCost, DRSpent);
            }
        }

        // How long are the GCDs for RP cost abilities?
        public float getRPTime()
        {
            // Melee
            float meleeGCD = 1.5f;

            // Spells / effects
            float spellGCD = GetGCDHasted();

            float GCDTimeforRPAbilties = spellGCD * (
                + DeathCoil // RP
                + HornOfWinter // none
                ) +
                meleeGCD * (
                + FrostStrike // RP
                );

            return GCDTimeforRPAbilties;
        }

        /// <summary>
        /// How fast is the GCD with the current character's stats?
        /// </summary>
        /// <param name="s">Total Stats for the character.</param>
        /// <returns>Duration of the GCD in seconds.</returns>
        public float GetGCDHasted() 
        {
            float fNormalGCD = 1.5f;
            float fHastedGCD = Math.Max( 1f, fNormalGCD/(1f + this.m_fPhysicalHaste) );
            return fHastedGCD;
        }

        // Rotations set by Skeleton Jack's 3.2 build and rotation notes.
        public void setRotation(Type t) 
        {
            curRotationType = t;
            managedRP = false;
            switch (curRotationType) {
                case Type.Blood:
                    numDisease = 2f;
                    diseaseUptime = 100f;
                    DeathCoil = 2f;
                    IcyTouch = 1f;
                    PlagueStrike = 1f;
                    ScourgeStrike = 0f;
                    FrostStrike = 0f;
                    HowlingBlast = 0f;
                    Obliterate = 0f;
                    DeathStrike = 2f;
                    BloodStrike = 0f;
                    HeartStrike = 6f;
                    Pestilence = 0f;
                    RuneStrike = 2f;
                    curRotationDuration = 20f;
                    break;
                case Type.Frost:
                    numDisease = 2f;
                    diseaseUptime = 100f;
                    DeathCoil = 0f;
                    IcyTouch = 2f;
                    PlagueStrike = 2f;
                    ScourgeStrike = 0f;
                    FrostStrike = 1f;
                    HowlingBlast = 0f;
                    Obliterate = 3f;
                    BloodStrike = 2f;
                    HeartStrike = 0f;
                    DeathStrike = 0f;
                    RuneStrike = 2f;
                    Pestilence = 0f;
                    curRotationDuration = 20f;
                    break;
                case Type.Unholy:
                    numDisease = 3f;
                    diseaseUptime = 100f;
                    DeathCoil = 0f;
                    IcyTouch = 1f;
                    PlagueStrike = 1f;
                    ScourgeStrike = 4f;
                    FrostStrike = 0f;
                    HowlingBlast = 0f;
                    Obliterate = 0f;
                    BloodStrike = 2f;
                    HeartStrike = 0f;
                    curRotationDuration = 20f;
                    DeathStrike = 0f;
                    RuneStrike = 2f;
                    Pestilence = 0f;
                    break;
                case Type.Custom:
                   /* numDisease = 0f;
                    diseaseUptime = 0f;
                    DeathCoil = 0f;
                    IcyTouch = 0f;
                    PlagueStrike = 0f;
                    ScourgeStrike = 0f;
                    UnholyBlight = 0f;
                    FrostStrike = 0f;
                    HowlingBlast = 0f;
                    Obliterate = 0f;
                    BloodStrike = 0f;
                    HeartStrike = 0f;
                    DancingRuneWeapon = 0f;
                    curRotationDuration = 0f;
                    GargoyleDuration = 0f;
                    */
                    break;
            }
        }

        /// <summary>
        /// Get the type of rotation we should be using based on talents.
        /// </summary>
        /// <param name="t">DeathKnight Talents to evaluate.</param>
        public void GetRotationByTalents(DeathKnightTalents t)
        {
            if (curRotationType == Type.Custom)
            {
                managedRP = false;
                // if talents are all 0, then setup a basic rotation:
                curRotationType = Type.Custom;
                // we're going to just spam a very basic rotation.
                // IT-PS-BS-BS-DS-DS-RP
                // Need to find a way to actually implement this properly.
                numDisease = 2f;
                diseaseUptime = 100f;
                IcyTouch = 2f;
                PlagueStrike = 2f;
                BloodStrike = 2f;
                Obliterate = 0f;
                DeathStrike = 2f;
                RuneStrike = 3f;
                Pestilence = 0f;

                DeathCoil = 2f;
                ScourgeStrike = 0f;
                FrostStrike = 0f;
                HowlingBlast = 0f;
                HeartStrike = 0f;
                curRotationDuration = 15f;
            }
            const int indexBlood = 0; // start index of Blood Talents.
            const int indexFrost = 28; // start index of Frost Talents.
            const int indexUnholy = indexFrost + 29; // start index of Unholy Talents.
            int[] TalentCounter = new int[4];
            int index = indexBlood;
            foreach (int i in t.Data)
            {
                if (i > 0)
                {
                    // Blood
                    if (index < indexFrost)
                        TalentCounter[(int)Type.Blood]++;
                    // Frost
                    else if ((indexFrost <= index) && (index < indexUnholy))
                    {
                        TalentCounter[(int)Type.Frost]++;
                    }
                    // Unholy
                    else if (index >= indexUnholy)
                    {
                        TalentCounter[(int)Type.Unholy]++;
                    }
                }
                index++;
            }
            if ((TalentCounter[(int)Type.Blood] > TalentCounter[(int)Type.Frost]) && (TalentCounter[(int)Type.Blood] > TalentCounter[(int)Type.Unholy]))
            {
                // Blood
                curRotationType = Type.Blood;
            }
            else if ((TalentCounter[(int)Type.Frost] > TalentCounter[(int)Type.Blood]) && (TalentCounter[(int)Type.Frost] > TalentCounter[(int)Type.Unholy]))
            {
                // Frost
                curRotationType = Type.Frost;
            }
            else if ((TalentCounter[(int)Type.Unholy] > TalentCounter[(int)Type.Frost]) && (TalentCounter[(int)Type.Unholy] > TalentCounter[(int)Type.Blood]))
            {
                // Unholy
                curRotationType = Type.Unholy;
            }

            managedRP = true;
            #region Frost Rotations
            if (curRotationType == Type.Frost)
            {
                numDisease = 2f;
                diseaseUptime = 100f;
                IcyTouch = 2f;
                PlagueStrike = 2f;
                ScourgeStrike = 0f;
                FrostStrike = 0f;
                if (t.FrostStrike > 0)
                {
                    DeathCoil = 0f;
                    FrostStrike = 2f;
                }
                else
                {
                    DeathCoil = 2f;
                    FrostStrike = 0f;
                }
                HowlingBlast = 0f;
                Obliterate = 3f;
                BloodStrike = 2f;
                HeartStrike = 0f;
                DeathStrike = 0f;
                RuneStrike = 3f;
                Pestilence = 0f;
                curRotationDuration = 15f;

                if (t.GlyphofHowlingBlast && t.HowlingBlast > 0)
                {
                    // Single Disease Glyphed HB rotation
                    // Means that we start w/ HB for the FF hit, and factor in PS later, while HB is on CD.
                    numDisease = 1f;
                    IcyTouch = 1f;
                    PlagueStrike = 1f;
                    HowlingBlast = 1f;
                }
                if (t.Rime > 0 && t.HowlingBlast > 0)
                {
                    // Additional HB proc based on Rime.
                    HowlingBlast += .5f;
                }
            }
            #endregion
            #region Blood Rotations
            else if (curRotationType == Type.Blood)
            {
                numDisease = 2f;
                diseaseUptime = 100f;
                DeathCoil = 2f;
                IcyTouch = 1f;
                PlagueStrike = 1f;
                ScourgeStrike = 0f;
                FrostStrike = 0f;
                HowlingBlast = 0f;
                Obliterate = 0f;
                DeathStrike = 2f;
                if (HeartStrike > 0)
                {
                    HeartStrike = 6f;
                    BloodStrike = 0f;
                }
                else
                {
                    HeartStrike = 0f;
                    BloodStrike = 6f;
                }
                Pestilence = 0f;
                RuneStrike = 2f;
                curRotationDuration = 15f;
            }
            #endregion
            #region Unholy Rotations
            else if (curRotationType == Type.Unholy)
            // UnHoly
            {
                numDisease = 3f;
                diseaseUptime = 100f;
                DeathCoil = 2f;
                IcyTouch = 1f;
                PlagueStrike = 1f;
                ScourgeStrike = 0f;
                if (t.ScourgeStrike > 0)
                    ScourgeStrike = 4f;
                else
                {
                    DeathStrike = 4f;
                }
                FrostStrike = 0f;
                HowlingBlast = 0f;
                Obliterate = 0f;
                BloodStrike = 2f;
                HeartStrike = 0f;
                curRotationDuration = 20f;
                RuneStrike = 2f;
                Pestilence = 0f;
            }
            #endregion

        }

    }
}
