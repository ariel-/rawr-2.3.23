using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Rogue
{
    public class RogueRotationCalculator
    {
        public Character Char { get; set; }
        public Stats Stats { get; set; }
        public CalculationOptionsRogue CalcOpts { get; set; }
        public float Duration { get; set; }
		public bool MaintainBleed { get; set; }
        public float MainHandSpeed { get; set; }
        public float OffHandSpeed { get; set; }
        public float MainHandSpeedNorm { get; set; }
        public float OffHandSpeedNorm { get; set; }
        public float ChanceExtraCPPerHit { get; set; }
        public float ChanceExtraCPPerMutiHit { get; set; }
        public float AvoidedWhiteMHAttacks { get; set; }
        public float AvoidedWhiteOHAttacks { get; set; }
        public float AvoidedMHAttacks { get; set; }
        public float AvoidedOHAttacks { get; set; }
        public float AvoidedFinisherAttacks { get; set; }
        public float AvoidedPoisonAttacks { get; set; }

        public RogueAbilityStats MainHandStats { get; set; }
        public RogueAbilityStats OffHandStats { get; set; }
        public RogueAbilityStats BackstabStats { get; set; }
        public RogueAbilityStats HemoStats { get; set; }
        public RogueAbilityStats MutiStats { get; set; }
        public RogueAbilityStats SStrikeStats { get; set; }
        public RogueAbilityStats RuptStats { get; set; }
        public RogueAbilityStats EnvenomStats { get; set; }
        public RogueAbilityStats EvisStats { get; set; }
        public RogueAbilityStats SnDStats { get; set; }
        public RogueAbilityStats IPStats { get; set; }
        public RogueAbilityStats DPStats { get; set; }
        public RogueAbilityStats WPStats { get; set; }
        public RogueAbilityStats APStats { get; set; }

        public float BonusDamageMultiplierHFB { get; set; }
        public float BonusEnergyRegen { get; set; }
        public float BonusEnergyRegenMultiplier { get; set; }
        public float BonusFlurryHaste { get; set; }
        public float BonusHemoDamageMultiplier { get; set; }
        public float BonusIPFrequencyMultiplier { get; set; }
        public float BonusMaxEnergy { get; set; }
        public float BonusStealthEnergyRegen { get; set; }
        public float ChanceOnCPOnSSCrit { get; set; }
        public float ChanceOnEnergyOnCrit { get; set; }
        public float ChanceOnEnergyOnOHAttack { get; set; }
        public float ChanceOnEnergyPerCPFinisher { get; set; }
        public float ChanceOnMHAttackOnSwordAxeHit { get; set; }
        public float ChanceOnNoDPConsumeOnEnvenom { get; set; }
        public float ChanceOnSnDResetOnEnvenom { get; set; }
        public float CPOnFinisher { get; set; }
        public float FlurryCostReduction { get; set; }
        public float ToTTCDReduction { get; set; }
        public float ToTTCostReduction { get; set; }
        public float VanishCDReduction { get; set; }

        private float[] _averageNormalCP = new float[6];
        private float[] _averageSStrikeCP = new float[6];
        private float[] _averageMutiCP = new float[6];
        private float[] _avgMutiNeeded = new float[6];

        public RogueRotationCalculator(Character character, Stats stats, CalculationOptionsRogue calcOpts, bool maintainBleed,
            float mainHandSpeed, float offHandSpeed, float mainHandSpeedNorm, float offHandSpeedNorm, float avoidedWhiteMHAttacks, float avoidedWhiteOHAttacks, float avoidedMHAttacks, float avoidedOHAttacks, float avoidedFinisherAttacks, float avoidedPoisonAttacks,
			float chanceExtraCPPerHit, float chanceExtraCPPerMutiHit, 
            RogueAbilityStats mainHandStats, RogueAbilityStats offHandStats, RogueAbilityStats backstabStats, RogueAbilityStats hemoStats, RogueAbilityStats sStrikeStats,
            RogueAbilityStats mutiStats, RogueAbilityStats ruptStats, RogueAbilityStats evisStats, RogueAbilityStats envenomStats, RogueAbilityStats snDStats, 
            RogueAbilityStats iPStats, RogueAbilityStats dPStats, RogueAbilityStats wPStats, RogueAbilityStats aPStats)
		{
            Char = character;
			Stats = stats;
            CalcOpts = calcOpts;
			Duration = CalcOpts.Duration;
			MaintainBleed = maintainBleed;
            MainHandSpeed = mainHandSpeed;
            OffHandSpeed = offHandSpeed;
            MainHandSpeedNorm = mainHandSpeedNorm;
            OffHandSpeedNorm = offHandSpeedNorm;
            AvoidedWhiteMHAttacks = avoidedWhiteMHAttacks;
            AvoidedWhiteOHAttacks = avoidedWhiteOHAttacks;
            AvoidedMHAttacks = avoidedMHAttacks;
            AvoidedOHAttacks = avoidedOHAttacks;
            AvoidedFinisherAttacks = avoidedFinisherAttacks;
            AvoidedPoisonAttacks = avoidedPoisonAttacks;
            ChanceExtraCPPerHit = chanceExtraCPPerHit;
            ChanceExtraCPPerMutiHit = chanceExtraCPPerMutiHit;

            MainHandStats = mainHandStats;
            OffHandStats = offHandStats;
            BackstabStats = backstabStats;
            HemoStats = hemoStats;
            MutiStats = mutiStats;
            SStrikeStats = sStrikeStats;
            RuptStats = ruptStats;
            EnvenomStats = envenomStats;
            EvisStats = evisStats;
            SnDStats = snDStats;
            IPStats = iPStats;
            DPStats = dPStats;
            WPStats = wPStats;
            APStats = aPStats;

            #region Talent bonuses
            BonusFlurryHaste = 0.2f * Char.RogueTalents.BladeFlurry;
            BonusEnergyRegen = (15 + (Char.RogueTalents.GlyphOfAdrenalineRush ? 5f : 0f)) * Char.RogueTalents.AdrenalineRush;
            BonusEnergyRegenMultiplier = 0.08f * Char.RogueTalents.Vitality;
            BonusHemoDamageMultiplier = 0.1f * Char.RogueTalents.SurpriseAttacks + 0.02f * Char.RogueTalents.SinisterCalling;
            BonusIPFrequencyMultiplier = 0.1f * Char.RogueTalents.ImprovedPoisons;
            BonusMaxEnergy = (10 + (Char.RogueTalents.GlyphOfVigor ? 10 : 0)) * Char.RogueTalents.Vigor;
            BonusStealthEnergyRegen = 0.3f * Char.RogueTalents.Overkill;
            ChanceOnCPOnSSCrit = Char.RogueTalents.GlyphOfSinisterStrike ? 0.5f : 0f;
            ChanceOnEnergyOnCrit = 2f * (Char.RogueTalents.FocusedAttacks > 2 ? 1f : (0.33f * Char.RogueTalents.FocusedAttacks));
            ChanceOnEnergyOnOHAttack = 3 * 0.2f * Char.RogueTalents.CombatPotency;
            ChanceOnEnergyPerCPFinisher = 0.04f * Char.RogueTalents.RelentlessStrikes;
            ChanceOnMHAttackOnSwordAxeHit = 0.01f * Char.RogueTalents.HackAndSlash;
            ChanceOnNoDPConsumeOnEnvenom = Char.RogueTalents.MasterPoisoner == 3 ? 1f : (0.33f * Char.RogueTalents.MasterPoisoner);
            ChanceOnSnDResetOnEnvenom = 0.2f * Char.RogueTalents.CutToTheChase;
            CPOnFinisher = 0.2f * Char.RogueTalents.Ruthlessness + 3f * Stats.ChanceOn3CPOnFinisher;
            FlurryCostReduction = Char.RogueTalents.GlyphOfBladeFlurry ? 25 : 0;
            ToTTCDReduction = 5 * Char.RogueTalents.FilthyTricks;
            ToTTCostReduction = 5 * Char.RogueTalents.FilthyTricks;
            VanishCDReduction = 30 * Char.RogueTalents.Elusiveness;
            #endregion

            #region Probability tables
            float c = ChanceExtraCPPerHit, h = (1f - c), f = CPOnFinisher, nf = (1f - f);
            _averageNormalCP[1] = 1 * (f + nf * h) + 2 * (nf * c);
            _averageNormalCP[2] = 2 * (f * h + nf * c + nf * h * h) + 3 * (f * c);
            _averageNormalCP[3] = 3 * (f * c + f * h * h + 2 * nf * c * h + nf * h * h * h) + 4 * (f * h * c + nf * c * c + nf * h * h * c);
            _averageNormalCP[4] = 4 * (2 * f * c * h + f * h * h * h + nf * c * c + 3 * nf * c * h * h + nf * h * h * h * h) + 5 * (f * c * c + f * h * h * c + 2 * nf * c * h * c + nf * h * h * h * c);
            _averageNormalCP[5] = 5 * (f * c * c + 3 * f * c * h * h + f * h * h * h * h + 3 * nf * c * c * h + 4 * nf * c * h * h * h + nf * h * h * h * h * h) + 6 * (2 * f * c * h * c + f * h * h * h * c + nf * c * c * c + 3 * nf * c * h * h * c + nf * h * h * h * h * c);
            
            c = ChanceExtraCPPerHit + ChanceOnCPOnSSCrit * SStrikeStats.CritChance; h = (1f - c);
            _averageSStrikeCP[1] = 1 * (f + nf * h) + 2 * (nf * c);
            _averageSStrikeCP[2] = 2 * (f * h + nf * c + nf * h * h) + 3 * (f * c + nf * h * c);
            _averageSStrikeCP[3] = 3 * (f * c + f * h * h + 2 * nf * c * h + nf * h * h * h) + 4 * (f * h * c + nf * c * c + nf * h * h * c);
            _averageSStrikeCP[4] = 4 * (2 * f * c * h + f * h * h * h + nf * c * c + 3 * nf * c * h * h + nf * h * h * h * h) + 5 * (f * c * c + f * h * h * c + 2 * nf * c * h * c + nf * h * h * h * c);
            _averageSStrikeCP[5] = 5 * (f * c * c + 3 * f * c * h * h + f * h * h * h * h + 3 * nf * c * c * h + 4 * nf * c * h * h * h + nf * h * h * h * h * h) + 6 * (2 * f * c * h * c + f * h * h * h * c + nf * c * c * c + 3 * nf * c * h * h * c + nf * h * h * h * h * c);
            
            c = ChanceExtraCPPerMutiHit; h = (1f - c);
            _averageMutiCP[1] = 1 * (f) + 2 * (nf * h) + 3 * (nf * c);
            _averageMutiCP[2] = 2 * (nf * h) + 3 * (f * h + nf * c) + 4 * (f * c);
            _averageMutiCP[3] = 3 * (f * h + nf * c) + 4 * (f * c + nf * h * h) + 5 * (nf * h * c);
            _averageMutiCP[4] = 4 * (f * c + nf * h * h) + 5 * (f * h * h + 2 * nf * h * c) + 5 * (f * h * c + nf * c * c);
            _averageMutiCP[5] = 5 * (f * h * h + 2 * nf * h * c) + 6 * (2 * f * h * c + nf * h * h * h + nf * c * c) + 7 * (f * c * c + nf * h * h * c);

            _avgMutiNeeded[1] = 1 * (nf);
            _avgMutiNeeded[2] = 1 * (f * h + f * c + nf * h + nf * c);
            _avgMutiNeeded[3] = 1 * (f * h + f * c + nf * c) + 2 * (nf * h);
            _avgMutiNeeded[4] = 1 * (f * c) + 2 * (f * h * h + f * h * c + nf * h * h + nf * h * c + nf * c);
            _avgMutiNeeded[5] = 2 * (f + nf * h * c + nf * c) + 3 * (nf * h * h);
            #endregion
        }

        public RogueRotationCalculation GetRotationCalculations(int CPG, bool useRupt, int finisher, int finisherCP, int snDCP, int mHPoison, int oHPoison, bool bleedIsUp, bool useTotT, bool PTRMode)
		{
            float energyRegen = 10f * (1f + BonusEnergyRegenMultiplier);
            float totalEnergyAvailable = 100f + BonusMaxEnergy +
                                         energyRegen * Duration +
                                         ((Duration - 20f) / (180f - VanishCDReduction)) * 20f * energyRegen * BonusStealthEnergyRegen +
                                         (useTotT ? (Stats.BonusToTTEnergy > 0 ? Stats.BonusToTTEnergy : (-15f + ToTTCostReduction)) * (Duration - 5f) / (30f - ToTTCDReduction) : 0f) +
                                         (useRupt ? 0.02f * (Duration / 2f) * Stats.ReduceEnergyCostFromRupture : 0f) +
                                         energyRegen * 2f * BonusEnergyRegen * (Duration / 180f) -
                                         (BonusFlurryHaste > 0 ? (25f - FlurryCostReduction) * Duration / 120f : 0f);
            float averageGCD = 1f / (1f - AvoidedMHAttacks);
            float averageFinisherGCD = 1f / (1f - AvoidedFinisherAttacks);
            float ruptDurationAverage = RuptStats.DurationAverage;
            float[] _averageCP = CPG == 0 ? _averageMutiCP : CPG == 1 ? _averageSStrikeCP : _averageNormalCP;
            float averageFinisherCP = _averageCP[5];
			
			#region Melee
			float whiteMHAttacks = Duration / MainHandSpeed + 0.5f * 0.5f * Stats.MoteOfAnger * Duration;
            float whiteOHAttacks = Duration / OffHandSpeed + 0.5f * 0.5f * Stats.MoteOfAnger * Duration;
            totalEnergyAvailable += whiteOHAttacks * (1f - AvoidedWhiteOHAttacks) * ChanceOnEnergyOnOHAttack +
                                    ChanceOnEnergyOnCrit * whiteMHAttacks * MainHandStats.CritChance +
                                    ChanceOnEnergyOnCrit * whiteOHAttacks * OffHandStats.CritChance;
			#endregion

            #region Combo Point Generator
            float cpgCount = 0f;
            float cpgEnergy = CPG == 2 ? BackstabStats.EnergyCost : CPG == 3 ? HemoStats.EnergyCost : CPG == 1 ? SStrikeStats.EnergyCost : MutiStats.EnergyCost;
            float CPPerCPG = CPG == 2 ? BackstabStats.CPPerSwing : CPG == 3 ? HemoStats.CPPerSwing : CPG == 1 ? SStrikeStats.CPPerSwing : MutiStats.CPPerSwing;
            #endregion

            #region Slice and Dice
            float averageSnDCP = _averageCP[snDCP];

            //Lose some time due to SnD/Rupt conflicts
            float snDRuptConflict = (1f / ruptDurationAverage) * 0.5f * (averageGCD * averageFinisherCP / CPPerCPG);

            float snDDuration = SnDStats.DurationAverage + 3f * Math.Min(5f, averageSnDCP)
                                - snDRuptConflict;
            float snDCount = Duration / snDDuration;
            snDCount = Math.Max(1f, (finisher == 2 ?  snDCount * (1f - ChanceOnSnDResetOnEnvenom): snDCount));
            float snDTotalEnergy = snDCount * (SnDStats.EnergyCost - 25f * ChanceOnEnergyPerCPFinisher * Math.Min(5f, averageSnDCP));
            float snDCPRequired = snDCount * (averageSnDCP - CPOnFinisher);
            float cpgToUse = snDCPRequired / CPPerCPG;
            cpgCount += cpgToUse;
            totalEnergyAvailable -= cpgToUse * cpgEnergy + snDTotalEnergy ;
            #endregion

            #region Damage Finishers
            float ruptCount = 0f;
            float evisCount = 0f;
            float envenomCount = 0f;
            if (useRupt)
            {
                #region Rupture
                //Lose GCDs at the start of the fight to get SnD up and enough CPGs to get 5CPG.
                float durationRuptable = Duration - 2f * averageGCD - (averageGCD * (averageFinisherCP / CPPerCPG));
                float ruptCountMax = durationRuptable / RuptStats.DurationAverage;
                float ruptCycleEnergy = ((averageFinisherCP - CPOnFinisher) / CPPerCPG) * cpgEnergy + RuptStats.EnergyCost - 25f * ChanceOnEnergyPerCPFinisher * averageFinisherCP;
                ruptCount = Math.Min(ruptCountMax, totalEnergyAvailable / ruptCycleEnergy);
                cpgCount += ((averageFinisherCP - CPOnFinisher) / CPPerCPG) * ruptCount;
                totalEnergyAvailable -= ruptCycleEnergy * ruptCount;
                #endregion
            }
            if (finisher == 1 && finisherCP > 0)
            {
                #region Eviscerate
                float averageEvisCP = _averageCP[finisherCP];
                float evisCycleEnergy = ((averageEvisCP - CPOnFinisher) / CPPerCPG) * cpgEnergy + EvisStats.EnergyCost - 25f * ChanceOnEnergyPerCPFinisher * averageEvisCP;
                evisCount = totalEnergyAvailable / evisCycleEnergy;
                cpgCount += evisCount * ((averageEvisCP - CPOnFinisher) / CPPerCPG);
                totalEnergyAvailable = 0f;
                #endregion
            }
            else if (finisher == 2 && finisherCP > 0)
            {
                #region Envenom
                float averageEnvenomCP = _averageCP[finisherCP];
                float envenomCycleEnergy = ((averageEnvenomCP - CPOnFinisher) / CPPerCPG) * cpgEnergy + EnvenomStats.EnergyCost - 25f * ChanceOnEnergyPerCPFinisher * averageEnvenomCP;
                envenomCount = totalEnergyAvailable / envenomCycleEnergy;
                cpgCount += envenomCount * ((averageEnvenomCP - CPOnFinisher) / CPPerCPG);
                totalEnergyAvailable = 0f;
                #endregion
            }
            #endregion

            #region Heartpierce proc
            float HP264PPS = 0;
            float HP277PPS = 0;
            float HP264TPS = 0;
            float HP277TPS = 0;
            if (MainHandStats.Weapon.Name == "Heartpierce")
                if (MainHandStats.Weapon.ItemLevel == 264)
                    HP264PPS += MainHandSpeedNorm / 60 * (whiteMHAttacks * (1f - AvoidedWhiteMHAttacks) + cpgCount + evisCount + envenomCount) / Duration;
                else if (MainHandStats.Weapon.ItemLevel == 277)
                    HP277PPS += MainHandSpeedNorm / 60 * (whiteMHAttacks * (1f - AvoidedWhiteMHAttacks) + cpgCount + evisCount + envenomCount) / Duration;
            if (OffHandStats.Weapon.Name == "Heartpierce")
                if (OffHandStats.Weapon.ItemLevel == 264)
                    HP264PPS += OffHandSpeedNorm / 60 * (whiteOHAttacks * (1f - AvoidedWhiteOHAttacks) + (cpgCount == 0 ? cpgCount : 0)) / Duration;
                else if (OffHandStats.Weapon.ItemLevel == 277)
                    HP277PPS += OffHandSpeedNorm / 60 * (whiteOHAttacks * (1f - AvoidedWhiteOHAttacks) + (cpgCount == 0 ? cpgCount : 0)) / Duration;
            if (HP264PPS > 0)
            {
                float ChanceVar264 = (float)Math.Exp(-2 * HP264PPS);
                HP264TPS = HP264PPS * ChanceVar264 * (1 - (float)Math.Pow(ChanceVar264, 5)) / (1 - ChanceVar264);
            }
            if (HP277PPS > 0)
            {
                float ChanceVar277 = (float)Math.Exp(-2 * HP277PPS);
                HP277TPS = HP277PPS * ChanceVar277 * (1 - (float)Math.Pow(ChanceVar277, 6)) / (1 - ChanceVar277);
            }
            totalEnergyAvailable += (HP264TPS + HP277TPS) * Duration * 4;
            #endregion

            #region Extra Energy turned into Combo Point Generators
            if (totalEnergyAvailable > 0)
            {
                cpgCount += totalEnergyAvailable / cpgEnergy;
                totalEnergyAvailable = 0f;
            }
            #endregion

            #region Extra Mainhand attacks from Hack and Slash
            if (MainHandStats.Weapon._type == ItemType.OneHandAxe || MainHandStats.Weapon._type == ItemType.OneHandSword)
            {
                whiteMHAttacks += ChanceOnMHAttackOnSwordAxeHit * (whiteMHAttacks * (1f - AvoidedWhiteMHAttacks) + cpgCount + evisCount + envenomCount);
            }
            if (OffHandStats.Weapon._type == ItemType.OneHandAxe || OffHandStats.Weapon._type == ItemType.OneHandSword)
            {
                whiteMHAttacks += ChanceOnMHAttackOnSwordAxeHit * (whiteOHAttacks * (1f - AvoidedWhiteOHAttacks) + (cpgCount == 0 ? cpgCount : 0));
            }
            #endregion

            #region Poisons
            float mHHitCount = whiteMHAttacks * (1f - AvoidedWhiteMHAttacks) + cpgCount + evisCount + envenomCount + snDCount;
            float oHHitCount = whiteOHAttacks * (1f - AvoidedWhiteOHAttacks) + (cpgCount == 0 ? cpgCount : 0);
            float iPCount = 0f;
            float dPCount = 0f;
            float wPCount = 0f;
            float aPCount = 0f;
            float iPProcRate = 0.2f * (1f + BonusIPFrequencyMultiplier) / 1.4f;
            float dPApplyChance = 0.3f + 0.04f * Char.RogueTalents.ImprovedPoisons;
            float envenomBuffTime = envenomCount * finisherCP + envenomCount;
            #region MainHand Poison
            if (mHPoison == 1)
                iPCount += mHHitCount * MainHandStats.Weapon._speed * iPProcRate * ((Duration - envenomBuffTime) / Duration +
                                                                 1.75f * envenomBuffTime / Duration);
            else if (mHPoison == 2 && oHPoison != 2)
            {
                float dPCountTemp = mHHitCount * dPApplyChance * (1f - AvoidedPoisonAttacks) *
                                    ((Duration - envenomBuffTime) / Duration + 1.15f * envenomBuffTime / Duration);
                dPCountTemp -= (1f - ChanceOnNoDPConsumeOnEnvenom) * envenomCount * 5 + 5;
                float dPStackTime = 5f / (dPApplyChance * (1f - AvoidedPoisonAttacks) * (1f - AvoidedWhiteMHAttacks)) * MainHandSpeed;
                dPCount = (Duration - dPStackTime - (1f - ChanceOnNoDPConsumeOnEnvenom) * envenomCount * dPStackTime) / 3 * 5 +
                          10 + (1f - ChanceOnNoDPConsumeOnEnvenom) * envenomCount * 10;
                if (oHPoison == 1)
                    iPCount += dPCountTemp;
                else if (oHPoison == 3)
                    wPCount += dPCountTemp;
                else if (oHPoison == 4)
                    aPCount += dPCountTemp;
            }
            else if (mHPoison == 3)
                wPCount += mHHitCount * MainHandStats.Weapon._speed * 21.43f / 60f;
            else if (mHPoison == 4)
                aPCount += mHHitCount * 0.5f;
            #endregion
            #region OffHand Poison
            if (oHPoison == 1)
                iPCount += oHHitCount * OffHandStats.Weapon._speed * iPProcRate * ((Duration - envenomBuffTime) / Duration +
                                                                1.75f * envenomBuffTime / Duration);
            else if (oHPoison == 2 && mHPoison != 2)
            {
                float dPCountTemp = oHHitCount * dPApplyChance * (1f - AvoidedPoisonAttacks) *
                                    ((Duration - envenomBuffTime) / Duration + 1.15f * envenomBuffTime / Duration);
                dPCountTemp -= (1f - ChanceOnNoDPConsumeOnEnvenom) * envenomCount * 10 + 10;
                float dPStackTime = 5f / (dPApplyChance * (1f - AvoidedPoisonAttacks) * (1f - AvoidedWhiteOHAttacks)) * OffHandSpeed;
                dPCount = (Duration - dPStackTime - (1f - ChanceOnNoDPConsumeOnEnvenom) * envenomCount * dPStackTime) / 3 * 5 +
                          10 + (1f - ChanceOnNoDPConsumeOnEnvenom) * envenomCount * 10;
                if (mHPoison == 1)
                    iPCount += dPCountTemp;
                else if (mHPoison == 3)
                    wPCount += dPCountTemp;
                else if (mHPoison == 4)
                    aPCount += dPCountTemp;
            }
            else if (oHPoison == 3)
                wPCount += oHHitCount * OffHandStats.Weapon._speed * 21.43f / 60f;
            else if (oHPoison == 4)
                aPCount += oHHitCount * 0.5f;
            #endregion
            iPCount *= (1f - AvoidedPoisonAttacks);
            wPCount *= (1f - AvoidedPoisonAttacks);
            aPCount *= (1f - AvoidedPoisonAttacks);
            #endregion

            #region Killing Spree
            float kSAttacks = 0;
            float kSDuration = 0;
            float kSDmgBonus = 1.2f;
            if (Char.RogueTalents.KillingSpree > 0)
            {
                float kSCount = Duration / (120f - (Char.RogueTalents.GlyphOfKillingSpree ? 45f : 0f));
                kSDuration = kSCount * 2.5f;
                kSAttacks = 5f * kSCount;
            }
            #endregion

            #region Damage Totals
            float mainHandDamageTotal = ((Duration - kSDuration) / Duration * (whiteMHAttacks - 0.5f * 0.5f * Stats.MoteOfAnger * Duration) +
                                        kSDmgBonus * kSDuration / Duration * (whiteMHAttacks - 0.5f * 0.5f * Stats.MoteOfAnger * Duration) +
                                        kSDmgBonus * kSAttacks) * MainHandStats.DamagePerSwing +
                                        0.5f * Stats.MoteOfAnger * Duration * 0.5f * MainHandStats.DamagePerSwing;
            float offHandDamageTotal = ((Duration - kSDuration) / Duration * (whiteOHAttacks - 0.5f * 0.5f * Stats.MoteOfAnger * Duration) +
                                       kSDmgBonus * kSDuration / Duration * (whiteOHAttacks - 0.5f * 0.5f * Stats.MoteOfAnger * Duration) +
                                       kSDmgBonus * kSAttacks) * OffHandStats.DamagePerSwing +
                                       0.5f * Stats.MoteOfAnger * Duration * 0.5f * OffHandStats.DamagePerSwing;
            float backstabDamageTotal = (CPG == 2 ? cpgCount : 0) * BackstabStats.DamagePerSwing;
            float hemoDamageTotal = (CPG == 3 ? cpgCount : 0) * HemoStats.DamagePerSwing;
            float sStrikeDamageTotal = (CPG == 1 ? cpgCount : 0) * SStrikeStats.DamagePerSwing;
            float mutiDamageTotal = (CPG == 0 ? cpgCount : 0) * MutiStats.DamagePerSwing;
            float ruptDamageTotal = ruptCount * RuptStats.DamagePerSwing * (RuptStats.DurationUptime / 16f);
            float evisDamageTotal = evisCount * (EvisStats.DamagePerSwing + EvisStats.DamagePerSwingPerCP * Math.Min(_averageCP[finisherCP], 5));
            float envenomDamageTotal = envenomCount * (EnvenomStats.DamagePerSwing + EnvenomStats.DamagePerSwingPerCP * Math.Min(_averageCP[finisherCP],5));
            float instantPoisonTotal = iPCount * IPStats.DamagePerSwing;
            float deadlyPoisonTotal = dPCount * DPStats.DamagePerSwing;
            float woundPoisonTotal = wPCount * WPStats.DamagePerSwing;
            float anestheticPoisonTotal = aPCount * APStats.DamagePerSwing;

            float damageTotal = mainHandDamageTotal + offHandDamageTotal + backstabDamageTotal + hemoDamageTotal + sStrikeDamageTotal + 
                                  mutiDamageTotal + ruptDamageTotal + evisDamageTotal + envenomDamageTotal + instantPoisonTotal + deadlyPoisonTotal + woundPoisonTotal + anestheticPoisonTotal;
            if (Char.RogueTalents.HungerForBlood > 0 && !CalcOpts.BleedIsUp && ruptCount == 0) damageTotal /= 1.05f + 0.03f * (Char.RogueTalents.GlyphOfHungerforBlood ? 1 : 0);
            #endregion

            return new RogueRotationCalculation()
            {
                DPS = damageTotal / Duration,
                TotalDamage = damageTotal,

                MainHandCount = whiteMHAttacks,
                OffHandCount = whiteOHAttacks,
                BackstabCount = (CPG == 2 ? cpgCount : 0),
                HemoCount = (CPG == 3 ? cpgCount : 0),
                SStrikeCount = (CPG == 1 ? cpgCount : 0),
                MutiCount = (CPG == 0 ? cpgCount : 0),
                RuptCount = ruptCount,
                EvisCount = evisCount,
                EnvenomCount = envenomCount,
                SnDCount = snDCount,
                IPCount = iPCount,
                DPCount = dPCount,
                WPCount = wPCount,
                APCount = aPCount,

                FinisherCP = finisherCP,
                EvisCP = (finisher == 1 ? Math.Min(_averageCP[finisherCP], 5) : 0),
                EnvenomCP = (finisher == 2 ? Math.Min(_averageCP[finisherCP], 5) : 0),
                SnDCP = snDCP,

                MHPoison = mHPoison,
                OHPoison = oHPoison,

                UseTotT = useTotT,
                CutToTheChase = ChanceOnSnDResetOnEnvenom,
            };
        }

        public class RogueRotationCalculation
        {
            public float DPS { get; set; }
            public float TotalDamage { get; set; }

            public float MainHandCount { get; set; }
            public float OffHandCount { get; set; }
            public float BackstabCount { get; set; }
            public float HemoCount { get; set; }
            public float SStrikeCount { get; set; }
            public float MutiCount { get; set; }
            public float RuptCount { get; set; }
            public float EvisCount { get; set; }
            public float EnvenomCount { get; set; }
            public float SnDCount { get; set; }
            public float IPCount { get; set; }
            public float DPCount { get; set; }
            public float WPCount { get; set; }
            public float APCount { get; set; }

            public float FinisherCP { get; set; }
            public float EvisCP { get; set; }
            public float EnvenomCP { get; set; }
            public int SnDCP { get; set; }

            public int MHPoison { get; set; }
            public int OHPoison { get; set; }

            public bool UseTotT { get; set; }
            public float CutToTheChase { get; set; }

            public override string ToString()
            {
                StringBuilder rotation = new StringBuilder();
                if (BackstabCount > 0) rotation.Append("BS ");
                if (HemoCount > 0) rotation.Append("He ");
                if (SStrikeCount > 0) rotation.Append("SS ");
                if (MutiCount > 0) rotation.Append("Mu ");
                if (RuptCount > 0) rotation.Append("Ru ");
                if (EvisCount > 0) rotation.AppendFormat("Ev{0} ", FinisherCP);
                if (EnvenomCount > 0) rotation.AppendFormat("En{0} ", FinisherCP);
                rotation.Append("SnD" + SnDCP.ToString());

                if (EnvenomCount > 0 && CutToTheChase == 1) rotation.AppendFormat("*Use {0}cp Slice and Dice, kept up with Envenom.\r\n", SnDCP);
                else if (EnvenomCount > 0 && CutToTheChase > 0) rotation.AppendFormat("*Use {0}cp Slice and Dice, partially kept up with Envenom.\r\n", SnDCP);
                else rotation.AppendFormat("*Keep {0}cp Slice and Dice up.\r\n", SnDCP);
                if (RuptCount > 0) rotation.Append("Keep 5cp Rupture up.\r\n");
                if (EvisCount > 0) rotation.AppendFormat("Use {0}cp Eviscerates to spend extra combo points.\r\n", FinisherCP);
                if (EnvenomCount > 0) rotation.AppendFormat("Use {0}cp Envenoms to spend extra combo points.\r\n", FinisherCP);
                if (BackstabCount > 0) rotation.Append("Use Backstab for combo points.\r\n");
                else if (HemoCount > 0) rotation.Append("Use Hemorrhage for combo points.\r\n");
                else if (SStrikeCount > 0) rotation.Append("Use Sinister Strike for combo points.\r\n");
                else if (MutiCount > 0) rotation.Append("Use Mutilate for combo points.\r\n");
                else rotation.Append("Error: no CPG used, please create an issue on rawr.codeplex.com including your char file.\r\n");
                if (MHPoison == 1) rotation.Append("Use Instant Poison on Mainhand.\r\n");
                else if (MHPoison == 2) rotation.Append("Use Deadly Poison on Mainhand.\r\n");
                else if (MHPoison == 3) rotation.Append("Use Wound Poison on Mainhand.\r\n");
                if (OHPoison == 1) rotation.Append("Use Instant Poison on Offhand.\r\n");
                else if (OHPoison == 2) rotation.Append("Use Deadly Poison on Offhand.\r\n");
                else if (OHPoison == 3) rotation.Append("Use Wound Poison on Offhand.\r\n");
                if (UseTotT) rotation.Append("Use Tricks of the Trade every cooldown.");

                return rotation.ToString();
            }
        }
    }
}
