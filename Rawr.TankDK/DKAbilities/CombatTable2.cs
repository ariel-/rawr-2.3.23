using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.TankDK
{
    enum RSState : int
    {
        Good = 0,
        RPStarved = 0x1,
        TimeStarved = 0x2,
    }

    class CombatTable2
    {
        #region Member Vars
        #region Basic setup variables
        private CombatState m_CState;
        private CharacterCalculationsTankDK m_Calcs;
        private CalculationOptionsTankDK m_Opts;
        public Rotation m_Rotation;
        #endregion

        // TODO: Find a way to balance 2h vs. DW.
        public List<AbilityDK_Base> ml_Rot;

        #region Output Values
        private float _TotalDamage;
        public float TotalDamage
        {
            get
            {
                return _TotalDamage;
            }
        }
        private float _TotalThreat;
        public float TotalThreat
        {
            get
            {
                return _TotalThreat;
            }
        }

        public float m_TPS // readonly
        {
            get
            {
                float tps = 0;
                if (m_RotationDuration > 0) 
                    tps = _TotalThreat / (m_RotationDuration / 1000);
                return tps;
            }
        }
        public float m_DPS // readonly
        {
            get
            {
                float dps = 0;
                if (m_RotationDuration > 0)
                    dps = _TotalDamage / (m_RotationDuration / 1000);
                return dps;
            }
        }

        private int[] _AbilityCost;
        public float m_RotationDuration // readonly
        {
            get 
            {
                // At the very least, the rotation duration is how long it takes to cast the 
                // Abilities selected in the rotations.
                int iRotDur = _AbilityCost[(int)DKCostTypes.CastTime] + (m_GCDs * 1500);
                // However, if the Cooldowns of the rotations take LONGER, 
                // then we need to extend the rotation Duration to be that length.
                iRotDur = Math.Max(iRotDur, _AbilityCost[(int)DKCostTypes.CooldownTime]);
                return iRotDur; 
            }
        }
        public float m_CooldownDuration // readonly
        {
            get { return _AbilityCost[(int)DKCostTypes.CooldownTime]; }
        }
        public float m_CastDuration // readonly
        {
            get { return _AbilityCost[(int)DKCostTypes.CastTime]; }
        }
        public float m_DurationDuration // readonly
        {
            get { return _AbilityCost[(int)DKCostTypes.DurationTime]; }
        }

        public int m_BloodRunes
        {
            get { return _AbilityCost[(int)DKCostTypes.Blood]; }
        }
        public int m_FrostRunes
        {
            get { return _AbilityCost[(int)DKCostTypes.Frost]; }
        }
        public int m_UnholyRunes
        {
            get { return _AbilityCost[(int)DKCostTypes.UnHoly]; }
        }
        public int m_DeathRunes
        {
            get { return _AbilityCost[(int)DKCostTypes.Death]; }
        }
        public int m_RunicPower
        {
            get { return _AbilityCost[(int)DKCostTypes.RunicPower]; }
        }
        private int _GCDs;
        public int m_GCDs
        {
            get { return _GCDs; }
        }

        public int m_iRSState = (int)RSState.Good;



        #endregion
        #endregion

        public CombatTable2(Character c, Stats stats, CharacterCalculationsTankDK calcs, CalculationOptionsTankDK calcOpts)
        {
            if (calcOpts.bExperimental)
            {
                this.m_CState = new CombatState();
                if (c != null)
                {
                    if (c.DeathKnightTalents == null)
                        c.DeathKnightTalents = new DeathKnightTalents();
                    this.m_CState.m_Talents = (DeathKnightTalents)c.DeathKnightTalents.Clone();
                }
                this.m_CState.m_Stats = stats.Clone();
                m_Calcs = calcs;
                m_Opts = calcOpts;
                this.m_CState.m_NumberOfTargets = (float)m_Opts.uNumberTargets;
                m_Rotation = calcOpts.m_Rotation;

                //TODO: Handle Expertise
                if (c.MainHand != null && c.MainHand.Item.Type != ItemType.None)
                {
                    m_CState.MH = new Weapon(c.MainHand.Item, m_CState.m_Stats, m_Opts, 0);
                    m_CState.OH = null;
                    if (c.MainHand.Slot != ItemSlot.TwoHand)
                    {
                        if (c.OffHand != null && c.OffHand.Item.Type != ItemType.None)
                            m_CState.OH = new Weapon(c.OffHand.Item, this.m_CState.m_Stats, m_Opts, 0);
                    }
                }
                else
                {
                    m_CState.MH = null;
                    m_CState.OH = null;
                }

                // Checking the rotation:
                if (m_Rotation.IcyTouch == 0
                    && m_Rotation.PlagueStrike == 0
                    && m_Rotation.BloodStrike == 0)
                {
                    // Then this is probably a null rotation, and
                    // so let's build one?
                    m_Rotation = new Rotation(this.m_CState.m_Talents);
                }

                BuildRotation();

                // TODO: move this out of the constructor
                CompileRotation(m_Rotation);
            }
        }

        /// <summary>
        /// Generate a rotation based on available resources.
        /// </summary>
        public void BuildRotation()
        {
            if (m_Opts.bExperimental)
            {
                // TODO: need to setup a CombatState object. 
                // Setup an instance of each ability.
                // Single Runes:
                AbilityDK_IcyTouch IT = new AbilityDK_IcyTouch(m_CState);
                AbilityDK_FrostFever FF = new AbilityDK_FrostFever(m_CState);
                AbilityDK_PlagueStrike PS = new AbilityDK_PlagueStrike(m_CState);
                AbilityDK_BloodPlague BP = new AbilityDK_BloodPlague(m_CState);
                AbilityDK_BloodStrike BS = new AbilityDK_BloodStrike(m_CState);
                AbilityDK_HeartStrike HS = new AbilityDK_HeartStrike(m_CState);
                AbilityDK_Pestilence Pest = new AbilityDK_Pestilence(m_CState);
                AbilityDK_BloodBoil BB = new AbilityDK_BloodBoil(m_CState);
                // Multi Runes:
                AbilityDK_DeathStrike DS = new AbilityDK_DeathStrike(m_CState);
                AbilityDK_HowlingBlast HB = new AbilityDK_HowlingBlast(m_CState);
                AbilityDK_Obliterate OB = new AbilityDK_Obliterate(m_CState);
                AbilityDK_ScourgeStrike SS = new AbilityDK_ScourgeStrike(m_CState);
                AbilityDK_DeathNDecay DnD = new AbilityDK_DeathNDecay(m_CState);
                // RP:
                AbilityDK_RuneStrike RS = new AbilityDK_RuneStrike(m_CState);
                AbilityDK_DeathCoil DC = new AbilityDK_DeathCoil(m_CState);
                AbilityDK_FrostStrike FS = new AbilityDK_FrostStrike(m_CState);

                // Build the sortable list of abilities
                List<AbilityDK_Base> l_RotRunes = new List<AbilityDK_Base>();
                List<AbilityDK_Base> l_RotRP = new List<AbilityDK_Base>();
                l_RotRunes.Add(IT);
                l_RotRunes.Add(PS);
                l_RotRunes.Add(BS);
                l_RotRunes.Add(HS);
                l_RotRunes.Add(BB);

                l_RotRunes.Add(DS);
                l_RotRunes.Add(HB);
                l_RotRunes.Add(OB);
                l_RotRunes.Add(SS);
                l_RotRunes.Add(DnD);

                l_RotRP.Add(RS);
                l_RotRP.Add(DC);
                l_RotRP.Add(FS);

                // The sorting functions may be causing the occasional crash on Vista 32.  
                // Test this before re-implementing.
                //            l_RotRunes.Sort(AbilityDK_Base.CompareThreatByRunes);
                //            l_RotRP.Sort(AbilityDK_Base.CompareByRP);

                // we now have lists that provide sorted by cost of # of runes 
                // and ammount of RP needed.
            }
        }

        /// <summary>
        /// Do the work for populating data based on the Rotation settings.
        /// </summary>
        public void CompileRotation(Rotation Rot)
        {
            if (m_Opts.bExperimental)
            {

                // Limitation w/ current setup - can't have partial strikes
                int i = 0;
                _AbilityCost = new int[(int)DKCostTypes.NumCostTypes];
                // Build the rotation into a list of abilities to play with.
                ml_Rot = new List<AbilityDK_Base>();
                #region Runes
                #region Frost
                if (Rot.IcyTouch > 0)
                {
                    for (i = (int)Rot.IcyTouch; i > 0; i--)
                    {
                        ml_Rot.Add(new AbilityDK_IcyTouch(m_CState));
                        ml_Rot.Add(new AbilityDK_FrostFever(m_CState));
                    }
                }
                #endregion
                #region Unholy
                if (Rot.PlagueStrike > 0)
                {
                    for (i = (int)Rot.PlagueStrike; i > 0; i--)
                    {
                        ml_Rot.Add(new AbilityDK_PlagueStrike(m_CState));
                        ml_Rot.Add(new AbilityDK_BloodPlague(m_CState));
                    }
                }
                #endregion
                #region Blood
                if (Rot.BloodStrike > 0)
                {
                    for (i = (int)Rot.BloodStrike; i > 0; i--)
                    {
                        ml_Rot.Add(new AbilityDK_BloodStrike(m_CState));
                        if (m_CState.m_Talents.BloodOfTheNorth > 0)
                        {
                            _AbilityCost[(int)DKCostTypes.Death] -= (m_CState.m_Talents.BloodOfTheNorth / 3);
                        }
                        if (m_CState.m_Talents.Reaping > 0)
                        {
                            _AbilityCost[(int)DKCostTypes.Death] -= (m_CState.m_Talents.Reaping / 3);
                        }
                    }
                }
                if (Rot.HeartStrike > 0)
                {
                    if (m_CState.m_Talents.HeartStrike > 0)
                    {
                        for (i = (int)Rot.HeartStrike; i > 0; i--)
                            ml_Rot.Add(new AbilityDK_HeartStrike(m_CState));
                    }
                    else
                    {
                        // Error
                        // Shot in rotation but not talented for.
                    }
                }
                if (Rot.BloodBoil > 0)
                {
                    for (i = (int)Rot.BloodBoil; i > 0; i--)
                        ml_Rot.Add(new AbilityDK_BloodBoil(m_CState));
                }
                if (Rot.Pestilence > 0)
                {
                    for (i = (int)Rot.Pestilence; i > 0; i--)
                    {
                        ml_Rot.Add(new AbilityDK_Pestilence(m_CState));
                        if (m_CState.m_Talents.BloodOfTheNorth > 0)
                        {
                            _AbilityCost[(int)DKCostTypes.Death] -= (m_CState.m_Talents.BloodOfTheNorth / 3);
                        }
                        if (m_CState.m_Talents.Reaping > 0)
                        {
                            _AbilityCost[(int)DKCostTypes.Death] -= (m_CState.m_Talents.Reaping / 3);
                        }
                    }
                }
                #endregion
                #region Frost+Unholy
                if (Rot.DeathStrike > 0)
                {
                    for (i = (int)Rot.DeathStrike; i > 0; i--)
                    {
                        ml_Rot.Add(new AbilityDK_DeathStrike(m_CState));
                        if (m_CState.m_Talents.DeathRuneMastery > 0)
                        {
                            _AbilityCost[(int)DKCostTypes.Death] -= 2 * (m_CState.m_Talents.DeathRuneMastery / 3);
                        }
                    }
                }
                if (Rot.HowlingBlast > 0)
                {
                    for (i = (int)Rot.HowlingBlast; i > 0; i--)
                    {
                        ml_Rot.Add(new AbilityDK_HowlingBlast(m_CState));
                        if (m_CState.m_Talents.GlyphofHowlingBlast)
                        {
                            ml_Rot.Add(new AbilityDK_FrostFever(m_CState));
                        }
                    }
                }
                if (Rot.Obliterate > 0)
                {
                    for (i = (int)Rot.Obliterate; i > 0; i--)
                    {
                        ml_Rot.Add(new AbilityDK_Obliterate(m_CState));
                        if (m_CState.m_Talents.DeathRuneMastery > 0)
                        {
                            _AbilityCost[(int)DKCostTypes.Death] -= 2 * (m_CState.m_Talents.DeathRuneMastery / 3);
                        }
                        // Oblit will consume the diseases... so we need to cut their duration short.
                        if (m_CState.m_Talents.Annihilation < 3)
                        {
                            float percDuration = m_CState.m_Talents.Annihilation / 3;
                            foreach (AbilityDK_Base a in ml_Rot)
                            {
                                if (a.szName == "Blood Plague"
                                    || a.szName == "Frost Fever"
                                    || a.szName == "Crypt Fever"
                                    || a.szName == "Ebon Plague")
                                {
                                    a.AbilityCost[(int)DKCostTypes.DurationTime] = (int)(a.AbilityCost[(int)DKCostTypes.DurationTime] * percDuration);
                                }
                            }
                        }
                    }
                }
                if (Rot.ScourgeStrike > 0)
                {
                    if (m_CState.m_Talents.ScourgeStrike > 0)
                    {
                        for (i = (int)Rot.ScourgeStrike; i > 0; i--)
                        {
                            ml_Rot.Add(new AbilityDK_ScourgeStrike(m_CState));
                        }
                    }
                    else
                    {
                        // Error
                        // Shot in rotation but not talented for.
                    }

                }
                #endregion
                #region BloodFrostUnholy
                if (Rot.DeathNDecay > 0)
                {
                    for (i = (int)Rot.DeathNDecay; i > 0; i--)
                        ml_Rot.Add(new AbilityDK_DeathNDecay(m_CState));
                }
                #endregion
                #endregion

                #region RunicPower
                if (Rot.RuneStrike > 0)
                {
                    for (i = (int)Rot.RuneStrike; i > 0; i--)
                        ml_Rot.Add(new AbilityDK_RuneStrike(m_CState));
                    // TODO: need to have these replace melee strikes.
                }
                if (Rot.DeathCoil > 0)
                {
                    for (i = (int)Rot.DeathCoil; i > 0; i--)
                        ml_Rot.Add(new AbilityDK_DeathCoil(m_CState));
                }
                if (Rot.FrostStrike > 0)
                {
                    if (m_CState.m_Talents.FrostStrike > 0)
                    {
                        for (i = (int)Rot.FrostStrike; i > 0; i--)
                            ml_Rot.Add(new AbilityDK_FrostStrike(m_CState));
                    }
                    else
                    {
                        // Error
                        // Shot in rotation, but not available via talents.
                    }
                }
                #endregion

                // Now that the list is built, setup the costs so that we know what the rotation looks like in runes.
                #region Sum of Costs
                _GCDs = 0;
                foreach (AbilityDK_Base ability in ml_Rot)
                {
                    for (int cost = 0; cost < (int)DKCostTypes.NumCostTypes; cost++)
                    {
                        _AbilityCost[cost] += ability.AbilityCost[cost];
                    }
                    // Setup the total GCD count
                    if (ability.bTriggersGCD)
                        ++_GCDs;
                }
                #endregion
                // TODO: Handle Disease/DoT Clipping

                // Now we have the total costs
                // Need to figure out how to factor in Death Runes
                SpendDeathRunes(_AbilityCost, 0);

                // Let's evaluate the rune cooldown cost to the GCD cost:
                // 2 runes every 10 secs 
                // in ms
                int BloodCD = _AbilityCost[(int)DKCostTypes.Blood] * 10 * 1000 / 2;
                int FrostCD = _AbilityCost[(int)DKCostTypes.Frost] * 10 * 1000 / 2;
                int UnHolyCD = _AbilityCost[(int)DKCostTypes.UnHoly] * 10 * 1000 / 2;
                int DeathCD = Math.Abs(_AbilityCost[(int)DKCostTypes.Death]) * 10 * 1000 / 2; // Assuming all DeathRune possible generating strikes actually generate death runes.

                int maxCD = Math.Max(BloodCD, FrostCD);
                maxCD = Math.Max(maxCD, UnHolyCD);

                // This sums up all Cooldowntime
                // Not really the best way of handling this since cooldowns can overlap.
                // TODO: Fix cooldown math.
                _AbilityCost[(int)DKCostTypes.CooldownTime] = Math.Max(maxCD, _AbilityCost[(int)DKCostTypes.CooldownTime]);


                #region White Swings
                float fWhiteSwings = 0;
                if (m_CState.MH != null && m_CState.MH.hastedSpeed != 0)
                {
                    fWhiteSwings = (float)m_RotationDuration / (int)(m_CState.MH.hastedSpeed * 1000);
                    // How many of the shots are parried?
                    // TODO: fWhiteSwings here may need to be # of BossSwings from BossHandler.
                    float fShotsParried = m_CState.m_Stats.Parry * fWhiteSwings;
                    // What's the average hasted speed of those shots?
                    float fParryHastedSpeed = (m_CState.MH.hastedSpeed * 1000) * (1f - 0.24f);
                    // How much of the rotation duration has hasted shots?
                    float fTimeHasted = fShotsParried * fParryHastedSpeed;
                    // The rest of that time, are normal shots
                    float fTimeNormal = m_RotationDuration - fTimeHasted;
                    // Get the final total.
                    fWhiteSwings = (fTimeNormal / (int)(m_CState.MH.hastedSpeed * 1000)) + fShotsParried;
                }
                // This will provide the opportunity to increase available RP by swings due to Talents.
                // Also get a swing count to balance out the RuneStrikes.
                bool bAvailableWhiteSwings = false;
                float fWhiteDamage = 0;
                if (0 < fWhiteSwings && fWhiteSwings < Rot.RuneStrike)
                {
                    // There's not enough time to get in all the RS's we have slated for
                    bAvailableWhiteSwings = false;
                    m_iRSState |= (int)RSState.TimeStarved;
                }
                else if (0 < fWhiteSwings)
                {
                    // Since there are spare white swings, let's figure out how much white damage is done
                    bAvailableWhiteSwings = true;
                    fWhiteSwings = (fWhiteSwings - Rot.RuneStrike);
                    fWhiteDamage = (m_CState.MH.damage * fWhiteSwings);
                    if (null != m_CState.OH)
                        fWhiteDamage += (m_CState.OH.damage * fWhiteSwings);
                }

                #endregion

                bool bSpareGCDs = false;
                bool bAvailableRP = false;
                if (maxCD > _AbilityCost[(int)DKCostTypes.CooldownTime])
                {
                    // We have spare GCDs waiting for Runes to come back.
                    bSpareGCDs = true;
                }

                // Are we starving the RSs?
                // We may be wanting to save RP for Mind Freeze or something.
                if (_AbilityCost[(int)DKCostTypes.RunicPower] > 0)
                {
                    // starving our RP based abilities in the rotation.
                    bAvailableRP = false;
                    // We need to adjust the number of RSs due to starvation.
                    m_iRSState |= (int)RSState.RPStarved;
                }
                else
                {
                    bAvailableRP = true;
                }

                #region Sum of DPS & Threat
                this._TotalDamage = 0;
                this._TotalThreat = 0;
                _TotalDamage += fWhiteDamage;
                _TotalThreat += fWhiteDamage * AbilityDK_Base.THREAT_FROST_PRESENCE;

                foreach (AbilityDK_Base ability in ml_Rot)
                {
                    this._TotalDamage += ability.GetTotalDamage();
                    this._TotalThreat += ability.GetTotalThreat();
                }
                #endregion


                // I need a way of setting up the rotation in such a way that I can get a state of the target at the time
                // the ability is used.  
            }
        }

        /// <summary>
        /// Pass in the AblityCost[] to process the DeathRunes
        /// </summary>
        /// <param name="AbilityCost"></param>
        public void SpendDeathRunes(int[] AbilityCost, int DRSpent)
        {
            // Need to figure out how to factor in Death Runes
            // Since each death rune replaces any other rune on the rotation,
            // for each death rune, cut the cost of the highest other rune by 1.
            // Do not run this if there are no DeathRunes to spend.
            if (Math.Abs(AbilityCost[(int)DKCostTypes.Death]) > DRSpent)
            {
                int iHighestCostAbilityIndex = 0;
                int iPreviousCostValue = 0;
                for (int t = 0; t < (int)DKCostTypes.Death; t++)
                {
                    // Is the cost higher than our previous checked value.
                    if (AbilityCost[t] > iPreviousCostValue)
                    {
                        // If so, save off the index of that ability.
                        iHighestCostAbilityIndex = t;
                    }
                    iPreviousCostValue = AbilityCost[t];
                }
                // After going through the full list, spend a death rune and 
                // then iterate through that list again. 
                AbilityCost[iHighestCostAbilityIndex] -= 1;
                // increment the death runes.
                DRSpent++;
                SpendDeathRunes(AbilityCost, DRSpent);
            }
        }
    }
}
