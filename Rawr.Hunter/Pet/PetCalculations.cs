using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Rawr.Hunter
{   
    public class PetCalculations
    {
        #region Variables
        Character character;
        CharacterCalculationsHunter calculatedStats;
        CalculationOptionsHunter CalcOpts;
        BossOptions BossOpts;
#if RAWR3 || SILVERLIGHT
        PetTalents PetTalents;
#else
        PetTalentTreeData PetTalents;
#endif
        HunterTalents Talents;
        Stats HunterStats;
        Stats StatsPetBuffs;
        public Stats PetStats;
        public PetSkillPriorityRotation priorityRotation;

        public float ferociousInspirationUptime;
        private List<float> freqs = new List<float>();

        // things we save earlier for later DPS calcs
        private float killCommandCooldown;
        private float critSpecialsAdjust;

        private float PetChanceToMiss = StatConversion.YELLOW_MISS_CHANCE_CAP[83 - 80];
        private float PetChanceToSpellMiss = StatConversion.GetSpellMiss(83 - 80, false);
        private float PetChanceToBeDodged = StatConversion.YELLOW_DODGE_CHANCE_CAP[83 - 80];
        #endregion

        public PetCalculations(Character character, CharacterCalculationsHunter calculatedStats, CalculationOptionsHunter calcopts, BossOptions bossOpts,
            Stats hunterStats, Stats statsPetBuffs)
        {
            this.character = character;
            this.calculatedStats = calculatedStats;
            this.CalcOpts = calcopts;
            this.BossOpts = bossOpts;
            this.PetTalents = calcopts.PetTalents;
            this.Talents = character.HunterTalents;
            this.HunterStats = hunterStats;
            this.StatsPetBuffs = statsPetBuffs;

            PetStats = new Stats();
        }

        private Stats _basePetStats = null;
        private Stats BasePetStats {
            get {
                return _basePetStats ?? (_basePetStats = new Stats() {
                    Agility   = 113,
                    Strength  = 331,
                    Stamina   = 361,
                    Intellect =  65,
                    Spirit    =  10,
                });
            }
        }

        private void CalculateTimings()
        {
            #region Focus Regen
            float focusRegenBasePer4 = 20f;
            float focusRegenBestialDiscipline = focusRegenBasePer4 * 0.5f * Talents.BestialDiscipline;

            float critHitsPer4 = calculatedStats.shotsPerSecondCritting * calculatedStats.priorityRotation.critsCompositeSum * 4f;
            float goForTheThroatPerCrit = 25 * Talents.GoForTheThroat;
            float focusRegenGoForTheThroat = critHitsPer4 * goForTheThroatPerCrit;

            float focusRegenPerSecond = (focusRegenBasePer4 + focusRegenBestialDiscipline + focusRegenGoForTheThroat) / 4f;

            float owlsFocusEffect = 0f;
#if RAWR3 || SILVERLIGHT
            if (PetTalents.OwlsFocus > 0) { owlsFocusEffect = 1f / (1f / (PetTalents.OwlsFocus * 0.15f) + 1f); }
#else
            if (PetTalents.OwlsFocus.Value > 0) { owlsFocusEffect = 1f / (1f / (PetTalents.OwlsFocus.Value * 0.15f) + 1f); }
#endif
            #endregion

            if (priorityRotation == null) {
                priorityRotation = new PetSkillPriorityRotation(character, CalcOpts);
                priorityRotation.AddSkill(CalcOpts.PetPriority1);
                priorityRotation.AddSkill(CalcOpts.PetPriority2);
                priorityRotation.AddSkill(CalcOpts.PetPriority3);
                priorityRotation.AddSkill(CalcOpts.PetPriority4);
                priorityRotation.AddSkill(CalcOpts.PetPriority5);
                priorityRotation.AddSkill(CalcOpts.PetPriority6);
                priorityRotation.AddSkill(CalcOpts.PetPriority7);
            }

            priorityRotation.owlsFocus = owlsFocusEffect;
            priorityRotation.fpsGained = focusRegenPerSecond;

            priorityRotation.calculateTimings();
        }

        public Stats GetSpecialEffectsStats(Character Char,
            float[] attemptedAtkIntervals, 
            float[] hitRates, float[] critRates, float bleedHitInterval, float dmgDoneInterval,
            Stats statsTotal, Stats statsToProcess)
        {
            Stats statsProcs = new Stats();
#if RAWR3 || SILVERLIGHT
            float fightDuration = BossOpts.BerserkTimer;
#else
            float fightDuration = CalcOpts.Duration;
#endif
            float atkspeed = attemptedAtkIntervals[1];
            
            foreach (SpecialEffect effect in (statsToProcess != null ? statsToProcess.SpecialEffects() : statsTotal.SpecialEffects()))
            {
                switch (effect.Trigger) {
                    case Trigger.Use:
                        Stats _stats = new Stats();
                        if (effect.Stats._rawSpecialEffectDataSize == 1 && statsToProcess == null) {
                            float uptime = effect.GetAverageUptime(0f, 1f, atkspeed, fightDuration);
                            _stats.AddSpecialEffect(effect.Stats._rawSpecialEffectData[0]);
                            Stats _stats2 = GetSpecialEffectsStats(Char,
                                attemptedAtkIntervals,
                                hitRates, critRates, bleedHitInterval, dmgDoneInterval, statsTotal, _stats);
                            _stats = _stats2 * uptime;
                        } else {
                            _stats = effect.GetAverageStats(0f, 1f, atkspeed, fightDuration);
                        }
                        statsProcs += _stats;
                        break;
                    case Trigger.MeleeHit:
                    case Trigger.PhysicalHit:
                        if (attemptedAtkIntervals[0] > 0f) {
                            Stats add = effect.GetAverageStats(attemptedAtkIntervals[0], hitRates[0], atkspeed, fightDuration);
                            statsProcs += add;
                        }
                        break;
                    case Trigger.MeleeCrit:
                    case Trigger.PhysicalCrit:
                        if (attemptedAtkIntervals[0] > 0f) {
                            Stats add = effect.GetAverageStats(attemptedAtkIntervals[0], critRates[0], atkspeed, fightDuration);
                            statsProcs += add;
                        }
                        break;
                    case Trigger.DoTTick:
                        if (bleedHitInterval > 0f) { statsProcs += effect.GetAverageStats(bleedHitInterval, 1f, atkspeed, fightDuration); } // 1/sec DeepWounds, 1/3sec Rend
                        break;
                    case Trigger.DamageDone: // physical and dots
                        if (dmgDoneInterval > 0f) { statsProcs += effect.GetAverageStats(dmgDoneInterval, 1f, atkspeed, fightDuration); }
                        break;
                    case Trigger.PetClawBiteSmackCrit:
                        if (attemptedAtkIntervals[3] > 0f) {
                            Stats add = effect.GetAverageStats(attemptedAtkIntervals[3], critRates[1], atkspeed, fightDuration); // this needs to be fixed to read steady shot frequencies
                            statsProcs += add;
                        }
                        break;
                }
            }
            return statsProcs;
        }

        #region Speeds and Intervals
        private float attackSpeedEffective = 2f;
        private float compositeSpeed = 2f;
        public float PetClawBiteSmackInterval = 0f;
        protected float PetAttackSpeed(Stats currentStats) {
            float attackSpeedBase = CalcOpts.PetFamily == PetFamily.None ? 0.0f : 2.0f;
            float attackSpeedAdjust = (1f + currentStats.PhysicalHaste);
            float attackSpeedAdjusted = attackSpeedBase / attackSpeedAdjust;

            return attackSpeedAdjusted;
        }
        protected float GenPetFullAttackSpeed(Stats currentStats) {
            attackSpeedEffective = PetAttackSpeed(currentStats);
            CalculateTimings();
            float petSpecialFreq = (priorityRotation.petSpecialFrequency > 0 ? priorityRotation.petSpecialFrequency : 0);
            compositeSpeed = 1f / (1f / attackSpeedEffective + (petSpecialFreq > 0 ? 1f / petSpecialFreq : 0));
            return compositeSpeed;
        }
        public float PetWhiteInterval {
            get { return PetAttackSpeed(PetStats); }
        }
        public float PetYellowInterval {
            get {
                CalculateTimings();
                return (priorityRotation.petSpecialFrequency > 0 ? priorityRotation.petSpecialFrequency : 0);
            }
        }
        public float PetCompInterval {
            get { return GenPetFullAttackSpeed(PetStats); }
        }
        #endregion

        private PetAttackTable _whAtkTable;
        public PetAttackTable WhAtkTable {
            get { return _whAtkTable; }
            set { _whAtkTable = value; }
        }
        private PetAttackTable _ywAtkTable;
        public PetAttackTable YwAtkTable {
            get { return _ywAtkTable; }
            set { _ywAtkTable = value; }
        }

        SpecialEffect frenzy = null;

        public void GenPetStats()
        {
            // Initial Variables
#if RAWR3 || SILVERLIGHT
            int levelDiff = BossOpts.Level - character.Level;
#else
            int levelDiff = CalcOpts.TargetLevel - character.Level;
#endif
            Stats petStatsBase = BasePetStats;
            #region From Hunter
            Stats petStatsFromHunter = new Stats() {
#if RAWR3 || SILVERLIGHT
                AttackPower = HunterStats.RangedAttackPower * 0.22f * (1f + PetTalents.WildHunt * 0.15f),
                SpellPower = HunterStats.RangedAttackPower * 0.1287f * (1f + PetTalents.WildHunt * 0.15f),
                Stamina = HunterStats.Stamina * 0.45f * (1f + PetTalents.WildHunt * 0.20f)
#else
                AttackPower = HunterStats.RangedAttackPower * 0.22f * (1f + PetTalents.WildHunt.Value * 0.15f),
                SpellPower = HunterStats.RangedAttackPower * 0.1287f * (1f + PetTalents.WildHunt.Value * 0.15f),
                Stamina = HunterStats.Stamina * 0.45f * (1f + PetTalents.WildHunt.Value * 0.20f)
#endif
                        + HunterStats.PetStamina,
                Strength = HunterStats.PetStrength,
                Spirit = HunterStats.PetSpirit,
                ArcaneResistance = HunterStats.ArcaneResistance * 0.4f,
                FireResistance   = HunterStats.FireResistance   * 0.4f,
                NatureResistance = HunterStats.NatureResistance * 0.4f,
                ShadowResistance = HunterStats.ShadowResistance * 0.4f,
                FrostResistance  = HunterStats.FrostResistance  * 0.4f,
                Armor = HunterStats.Armor * 0.35f,
                SpellPenetration = HunterStats.SpellPenetration,
                Resilience = HunterStats.Resilience,
                BonusDamageMultiplier = (1f + (HunterStats.BonusDamageMultiplier /* / (1f + Talents.TheBeastWithin * 0.10f)*/))
                                      * (1f + (character.Race == CharacterRace.Orc ? 0.05f : 0f))
                                      - 1f,
                BonusPetDamageMultiplier = HunterStats.BonusPetDamageMultiplier,
                BonusBleedDamageMultiplier = HunterStats.BonusBleedDamageMultiplier,
                BonusPetAttackPowerMultiplier = HunterStats.BonusPetAttackPowerMultiplier,
                PetAttackPower = HunterStats.PetAttackPower,
            };
            #endregion
            #region From Buffs
            Stats petStatsBuffs = StatsPetBuffs;
            petStatsBuffs.Stamina  += petStatsBuffs.PetStamina;
            petStatsBuffs.Strength += petStatsBuffs.PetStrength;
            petStatsBuffs.Spirit   += petStatsBuffs.PetSpirit;
            #endregion
            #region From Talents (Pet or Hunter)
            Stats petStatsTalents = new Stats() {
#if RAWR3 || SILVERLIGHT
                BonusStaminaMultiplier = (1f + PetTalents.BloodOfTheRhino * 0.02f)
                                       * (1f + PetTalents.GreatStamina * 0.04f)
                                       - 1f,
                MovementSpeed = PetTalents.BoarsSpeed * 0.30f,
                PhysicalHaste = (1f + PetTalents.CobraReflexes * 0.15f)
                              * (1f + Talents.SerpentsSwiftness * 0.04f)
                              - 1f,
                PhysicalCrit = PetTalents.SpidersBite * 0.03f
                             + Talents.Ferocity * 0.02f,
                BaseArmorMultiplier = (1f + PetTalents.NaturalArmor * 0.05f)
                                    * (1f + PetTalents.PetBarding * 0.05f)
                                    - 1f,
                ArcaneResistance = PetTalents.GreatResistance * 0.05f,
                FireResistance = PetTalents.GreatResistance * 0.05f,
                NatureResistance = PetTalents.GreatResistance * 0.05f,
                ShadowResistance = PetTalents.GreatResistance * 0.05f,
                FrostResistance = PetTalents.GreatResistance * 0.05f,
                FearDurReduc = PetTalents.Lionhearted * 0.15f,
                StunDurReduc = PetTalents.Lionhearted * 0.15f,
                Dodge = Talents.CatlikeReflexes * 0.03f + PetTalents.PetBarding * 0.01f,
                BonusDamageMultiplier = (1f + Talents.UnleashedFury * 0.03f)
                                      * (1f + PetTalents.SpikedCollar * 0.03f)
                                      * (1f + Talents.KindredSpirits * 0.04f)
                                      * (1f + PetTalents.SharkAttack * 0.03f)
                                      - 1f,
#else
                BonusStaminaMultiplier = (1f + PetTalents.BloodOfTheRhino.Value * 0.02f)
                                       * (1f + PetTalents.GreatStamina.Value * 0.04f)
                                       - 1f,
                MovementSpeed = PetTalents.BoarsSpeed.Value * 0.30f,
                PhysicalHaste = (1f + PetTalents.CobraReflexes.Value * 0.15f)
                              * (1f + Talents.SerpentsSwiftness * 0.04f)
                              - 1f,
                PhysicalCrit = PetTalents.SpidersBite.Value * 0.03f
                             + Talents.Ferocity * 0.02f,
                BaseArmorMultiplier = (1f + PetTalents.NaturalArmor.Value * 0.05f)
                                    * (1f + PetTalents.PetBarding.Value   * 0.05f)
                                    - 1f,
                ArcaneResistance = PetTalents.GreatResistance.Value * 0.05f,
                FireResistance   = PetTalents.GreatResistance.Value * 0.05f,
                NatureResistance = PetTalents.GreatResistance.Value * 0.05f,
                ShadowResistance = PetTalents.GreatResistance.Value * 0.05f,
                FrostResistance  = PetTalents.GreatResistance.Value * 0.05f,
                FearDurReduc = PetTalents.Lionhearted.Value * 0.15f,
                StunDurReduc = PetTalents.Lionhearted.Value * 0.15f,
                Dodge = Talents.CatlikeReflexes * 0.03f + PetTalents.PetBarding.Value * 0.01f,
                BonusDamageMultiplier = (1f + Talents.UnleashedFury * 0.03f)
                                      * (1f + PetTalents.SpikedCollar.Value * 0.03f)
                                      * (1f + Talents.KindredSpirits * 0.04f)
                                      * (1f + PetTalents.SharkAttack.Value * 0.03f)
                                      - 1f,
#endif
                BonusAttackPowerMultiplier = (1f + Talents.AnimalHandler * 0.05f)
                                           * (1f + calculatedStats.aspectBonusAPBeast)
                                           - 1f,
                BonusHealthMultiplier = Talents.EnduranceTraining * 0.02f,
            };
            float LongevityCdAdjust = 1f - Talents.Longevity * 0.10f;
#if RAWR3 || SILVERLIGHT
            if (PetTalents.Rabid > 0) {
#else
            if (PetTalents.Rabid.Value > 0) {
#endif
                                                float rabidCooldown = 45f * LongevityCdAdjust;
                SpecialEffect primary = new SpecialEffect(Trigger.Use, new Stats() { }, 20f, rabidCooldown);
                SpecialEffect secondary = new SpecialEffect(Trigger.MeleeHit,
                    new Stats() { BonusAttackPowerMultiplier = 0.05f }, 20f, 0f, 0.50f, 5);
                primary.Stats.AddSpecialEffect(secondary);
                petStatsTalents.AddSpecialEffect(primary);
            }
            if (Talents.Frenzy > 0) {
                if (frenzy == null) {
                    frenzy = new SpecialEffect(Trigger.MeleeCrit, new Stats() { PhysicalHaste = 0.30f, }, 8f, 1f, Talents.Frenzy * 0.20f);
                }
                petStatsTalents.AddSpecialEffect(frenzy);
            }
#if RAWR3 || SILVERLIGHT
            if (PetTalents.LastStand > 0) {
#else
            if (PetTalents.LastStand.Value > 0) {
#endif
                SpecialEffect laststand = new SpecialEffect(Trigger.Use, new Stats() { BonusHealthMultiplier = 0.30f, }, 20f, (1f * 60f) * LongevityCdAdjust);
                petStatsTalents.AddSpecialEffect(laststand);
            }
            #endregion
            #region From Options
            Stats petStatsOptionsPanel = new Stats() {
                PhysicalCrit = StatConversion.NPC_LEVEL_CRIT_MOD[levelDiff],
                BonusStaminaMultiplier = 0.05f,
                BonusDamageMultiplier = (CalcOpts.PetHappinessLevel == PetHappiness.Happy ? 1.25f
                                        : CalcOpts.PetHappinessLevel == PetHappiness.Content ? 1f
                                        : 0.75f)
                                        - 1f,
            };
            CalculateTimings();
            if(priorityRotation.getSkillFrequency(PetAttacks.SerenityDust) > 0){
                // TODO: Need to make sure this freq actually works
                float freq = priorityRotation.getSkillFrequency(PetAttacks.SerenityDust);
                SpecialEffect serenitydust = new SpecialEffect(Trigger.Use,
                    new Stats() { BonusAttackPowerMultiplier = 0.10f, },
#if RAWR3 || SILVERLIGHT
                    15f, BossOpts.BerserkTimer / freq);
#else
                    15f, CalcOpts.Duration / freq);
#endif
                petStatsOptionsPanel.AddSpecialEffect(serenitydust);
#if RAWR3 || SILVERLIGHT
                petStatsOptionsPanel.HealthRestore += (BossOpts.BerserkTimer / freq) * 825f;
#else
                petStatsOptionsPanel.HealthRestore += (CalcOpts.Duration / freq) * 825f;
#endif
            }
            #endregion

            // Totals
            Stats petStatsGearEnchantsBuffs = new Stats();
            petStatsGearEnchantsBuffs.Accumulate(petStatsBuffs);
            Stats petStatsTotal = new Stats();
            petStatsTotal.Accumulate(petStatsBase);
            petStatsTotal.Accumulate(petStatsFromHunter);
            petStatsTotal.Accumulate(petStatsBuffs);
            petStatsTotal.Accumulate(petStatsTalents);
            petStatsTotal.Accumulate(petStatsOptionsPanel);
            Stats petStatsProcs = new Stats();

            #region Stamina & Health
            float totalBSTAM = petStatsTotal.BonusStaminaMultiplier;
            petStatsTotal.Stamina = (float)Math.Floor((1f + totalBSTAM) * petStatsTotal.Stamina);

            // Health | Pets dont get Health from their Base Stam, all pets have a 5% Bonus Stam
            petStatsTotal.Health += StatConversion.GetHealthFromStamina(petStatsTotal.Stamina - petStatsBase.Stamina);
            petStatsTotal.Health *= 1f + petStatsTotal.BonusHealthMultiplier;

#if RAWR3 || SILVERLIGHT
            if (PetTalents.Bloodthirsty > 0) {
#else
            if (PetTalents.Bloodthirsty.Value > 0) {
#endif
                SpecialEffect bloodthirsty = new SpecialEffect(Trigger.MeleeHit,
                    new Stats() { HealthRestore = petStatsTotal.Health * 0.05f },
#if RAWR3 || SILVERLIGHT
                    0f, 0f, PetTalents.Bloodthirsty * 0.10f);
#else
                    0f, 0f, PetTalents.Bloodthirsty.Value * 0.10f);
#endif
                petStatsTotal.AddSpecialEffect(bloodthirsty);
            }
            #endregion

            #region Strength
            float totalBSTRM = petStatsTotal.BonusStrengthMultiplier;
            petStatsTotal.Strength = /*(float)Math.Floor(*/(1f + totalBSTRM) * petStatsTotal.Strength/*)*/;
            #endregion

            #region Agility
            float totalBAGIM = petStatsTotal.BonusAgilityMultiplier;
            petStatsTotal.Agility = /*(float)Math.Floor(*/(1f + totalBAGIM) * petStatsTotal.Agility/*)*/;
            #endregion

            #region Attack Power
            petStatsTotal.BonusAttackPowerMultiplier *= (1f + petStatsTotal.BonusPetAttackPowerMultiplier);
            float totalBAPM    = petStatsTotal.BonusAttackPowerMultiplier;

            float apFromBase   = (1f + totalBAPM) * (petStatsTotal.AttackPower - petStatsFromHunter.AttackPower);
            float apFromBonus  = (1f + totalBAPM) * (petStatsTotal.PetAttackPower);

            float apFromHunter = (1f + totalBAPM) * (petStatsFromHunter.AttackPower);
            float apFromSTR    = (1f + totalBAPM) * (petStatsTotal.Strength - 10f) * 2f;
            float apFromHvW    = (1f + totalBAPM) * (HunterStats.Stamina * 0.10f * Talents.HunterVsWild);

            petStatsTotal.AttackPower = apFromBase + apFromBonus + apFromHunter + apFromSTR + apFromHvW;
            #endregion

            #region Haste
            /*if (StatsPetBuffs._rawSpecialEffectData != null && StatsPetBuffs._rawSpecialEffectData.Length > 0) {
                Stats add = StatsPetBuffs._rawSpecialEffectData[0].GetAverageStats();
                StatsPetBuffs.PhysicalHaste *= (1f + add.PhysicalHaste);
            }*/
            #endregion

            #region Armor
            petStatsTotal.Armor = (float)Math.Floor(petStatsTotal.Armor * (1f + petStatsTotal.BaseArmorMultiplier));
            petStatsTotal.BonusArmor += petStatsTotal.Agility * 2f;
            petStatsTotal.BonusArmor = (float)Math.Floor(petStatsTotal.BonusArmor * (1f + petStatsTotal.BonusArmorMultiplier));
            petStatsTotal.Armor += petStatsTotal.BonusArmor;
            #endregion

            #region Hit/Dodge Chances
            // This is tied directly to the Hunter's chance to miss
            PetChanceToMiss = Math.Max(0f, StatConversion.YELLOW_MISS_CHANCE_CAP[levelDiff] - HunterStats.PhysicalHit);
            PetChanceToSpellMiss = -1f * Math.Min(0f, StatConversion.GetSpellMiss(levelDiff, false) - HunterStats.SpellHit);

            calculatedStats.petHitTotal = HunterStats.PhysicalHit;
            calculatedStats.petHitSpellTotal = HunterStats.SpellHit;

            petStatsTotal.PhysicalHit = HunterStats.PhysicalHit;
            petStatsTotal.SpellHit = HunterStats.SpellHit;

            // If the Hunter is Hit Capped, the pet is also Exp Capped
            // If not, Pet is proportionately lower based on Hunter's Hit
            // Expertise itself doesn't factor in at all
            PetChanceToBeDodged = StatConversion.YELLOW_DODGE_CHANCE_CAP[levelDiff]
                                * Math.Min(1f, (PetChanceToMiss / StatConversion.YELLOW_MISS_CHANCE_CAP[levelDiff]));
            calculatedStats.petTargetDodge = PetChanceToBeDodged;

            float[] avoidChances = { PetChanceToMiss, PetChanceToSpellMiss, PetChanceToBeDodged };
            #endregion

            #region Crit Chance
            petStatsTotal.PhysicalCrit += StatConversion.GetCritFromAgility(petStatsTotal.Agility, CharacterClass.Warrior)
                                        + 0.032f
                                        + StatConversion.GetCritFromRating(petStatsTotal.CritRating);

            calculatedStats.petCritTotalMelee = petStatsTotal.PhysicalCrit;

            // Cobra Strikes
            calculatedStats.petCritFromCobraStrikes = 0;
            float cobraStrikesProc = Talents.CobraStrikes * 0.2f;
            if (cobraStrikesProc > 0)
            {
                float cobraStrikesCritFreqSteady = calculatedStats.steadyShot.Freq * (1f / calculatedStats.steadyShot.CritChance);
                float cobraStrikesCritFreqArcane = calculatedStats.arcaneShot.Freq * (1f / calculatedStats.arcaneShot.CritChance);
                float cobraStrikesCritFreqKill = calculatedStats.killShot.Freq * (1f / calculatedStats.killShot.CritChance);

                float cobraStrikesCritFreqSteadyInv = cobraStrikesCritFreqSteady > 0 ? 1f / cobraStrikesCritFreqSteady : 0;
                float cobraStrikesCritFreqArcaneInv = cobraStrikesCritFreqArcane > 0 ? 1f / cobraStrikesCritFreqArcane : 0;
                float cobraStrikesCritFreqKillInv = cobraStrikesCritFreqKill > 0 ? 1f / cobraStrikesCritFreqKill : 0;
                float cobraStrikesCritFreqInv = cobraStrikesCritFreqSteadyInv + cobraStrikesCritFreqArcaneInv + cobraStrikesCritFreqKillInv;
                float cobraStrikesCritFreq = cobraStrikesCritFreqInv > 0 ? 1f / cobraStrikesCritFreqInv : 0;
                float cobraStrikesProcAdjust = cobraStrikesCritFreq / cobraStrikesProc;

                float cobraStrikesFreqSteadyInv = calculatedStats.steadyShot.Freq > 0 ? 1f / calculatedStats.steadyShot.Freq : 0;
                float cobraStrikesFreqArcaneInv = calculatedStats.arcaneShot.Freq > 0 ? 1f / calculatedStats.arcaneShot.Freq : 0;
                float cobraStrikesFreqKillInv = calculatedStats.killShot.Freq > 0 ? 1f / calculatedStats.killShot.Freq : 0;
                float cobraStrikesFreqInv = cobraStrikesFreqSteadyInv + cobraStrikesFreqArcaneInv + cobraStrikesFreqKillInv;

                float cobraStrikesTwoSpecials = 2f * priorityRotation.petSpecialFrequency;
                float cobraStrikesUptime = 1f - (float)Math.Pow(1f - calculatedStats.steadyShot.CritChance * cobraStrikesProc, cobraStrikesFreqInv * cobraStrikesTwoSpecials);

                calculatedStats.petCritFromCobraStrikes = (cobraStrikesUptime + (1f - cobraStrikesUptime) * calculatedStats.petCritTotalMelee) - calculatedStats.petCritTotalMelee;
            }

            calculatedStats.petCritTotalSpecials = petStatsTotal.PhysicalCrit + calculatedStats.petCritFromCobraStrikes; // PetCritChance
            critSpecialsAdjust = calculatedStats.petCritTotalSpecials * 1.5f + 1f;
            #endregion

            #region Handle Special Effects
            CalculateTimings();
            WhAtkTable = new PetAttackTable(character, petStatsTotal, CalcOpts, avoidChances, false, false);
            YwAtkTable = new PetAttackTable(character, petStatsTotal, CalcOpts, avoidChances, PetAttacks.Claw, false, false);

            float AllAttemptedAtksInterval = GenPetFullAttackSpeed(petStatsTotal);
            float WhtAttemptedAtksInterval = PetAttackSpeed(petStatsTotal);
            float YlwAttemptedAtksInterval = AllAttemptedAtksInterval - WhtAttemptedAtksInterval;

            float[] hitRates  = { WhAtkTable.AnyLand,   // Whites
                                  YwAtkTable.AnyLand }; // Yellows
            float[] critRates = { WhAtkTable.Crit, // Whites
                                  YwAtkTable.Crit + calculatedStats.petCritFromCobraStrikes }; // Yellows

            float bleedHitInterval = 0f;
            float rakefreq = priorityRotation.getSkillFrequency(PetAttacks.Rake ); if (rakefreq > 0) { bleedHitInterval      += rakefreq; }

            float dmgDoneInterval = 1f; // Need to Fix this

            float clawbitesmackinterval = 0f;
            float clawfreq = priorityRotation.getSkillFrequency(PetAttacks.Claw ); if (clawfreq > 0) { clawbitesmackinterval += clawfreq; }
            float bitefreq = priorityRotation.getSkillFrequency(PetAttacks.Bite ); if (bitefreq > 0) { clawbitesmackinterval += bitefreq; }
            float smakfreq = priorityRotation.getSkillFrequency(PetAttacks.Smack); if (smakfreq > 0) { clawbitesmackinterval += smakfreq; }
            PetClawBiteSmackInterval = clawbitesmackinterval;

            float[] AttemptedAtkIntervals = {
                AllAttemptedAtksInterval, // All
                WhtAttemptedAtksInterval, // Whites
                YlwAttemptedAtksInterval, // Yellows
                PetClawBiteSmackInterval, // ClawBiteSmack
            };

            petStatsProcs += GetSpecialEffectsStats(character, AttemptedAtkIntervals, hitRates, critRates,
                                    bleedHitInterval, dmgDoneInterval, petStatsTotal, null);

            #region Stat Results of Special Effects
            // Base Stats
            petStatsProcs.Stamina  = (float)Math.Floor(petStatsProcs.Stamina  * (1f + totalBSTAM) * (1f + petStatsProcs.BonusStaminaMultiplier));
            petStatsProcs.Strength = (float)Math.Floor(petStatsProcs.Strength * (1f + totalBSTRM) * (1f + petStatsProcs.BonusStrengthMultiplier));
            petStatsProcs.Agility  = (float)Math.Floor(petStatsProcs.Agility  * (1f + totalBAGIM) * (1f + petStatsProcs.BonusAgilityMultiplier));
            petStatsProcs.Health  += (float)Math.Floor(petStatsProcs.Stamina  * 10f);
            petStatsProcs.Health  += (float)Math.Floor((petStatsProcs.Health + petStatsTotal.Health) * petStatsProcs.BonusHealthMultiplier);

            // Armor
            petStatsProcs.Armor = (float)Math.Floor(petStatsProcs.Armor * (1f + petStatsTotal.BaseArmorMultiplier + petStatsProcs.BaseArmorMultiplier));
            petStatsProcs.BonusArmor += petStatsProcs.Agility * 2f;
            petStatsProcs.BonusArmor = (float)Math.Floor(petStatsProcs.BonusArmor * (1f + petStatsTotal.BonusArmorMultiplier + petStatsProcs.BonusArmorMultiplier));
            petStatsProcs.Armor += petStatsProcs.BonusArmor;
            petStatsProcs.BonusArmor = 0; //it's been added to Armor so kill it

            // Attack Power
            petStatsProcs.BonusAttackPowerMultiplier *= (1f + petStatsProcs.BonusPetAttackPowerMultiplier);
            float totalBAPMProcs    = (1f + totalBAPM) * (1f + petStatsProcs.BonusAttackPowerMultiplier) - 1f;
            float apFromSTRProcs    = (1f + totalBAPMProcs) * (petStatsProcs.Strength * 2f);
            float apBonusOtherProcs = (1f + totalBAPMProcs) * (petStatsProcs.AttackPower + petStatsProcs.PetAttackPower);
            float apBonusFromBasetoNewMulti = (petStatsProcs.BonusAttackPowerMultiplier) * (petStatsTotal.AttackPower);
            petStatsProcs.AttackPower = (float)Math.Floor(apFromSTRProcs + apBonusOtherProcs + apBonusFromBasetoNewMulti);

            // Crit
            petStatsProcs.PhysicalCrit += StatConversion.GetCritFromAgility(petStatsProcs.Agility, CharacterClass.Warrior);
            //petStatsProcs.PhysicalCrit += StatConversion.GetCritFromRating(petStatsProcs.CritRating, CharacterClass.Warrior);

            #endregion
            petStatsTotal.Accumulate(petStatsProcs);

            GenPetFullAttackSpeed(petStatsTotal);
            CalculateTimings();
            clawbitesmackinterval = 0f;
            clawfreq = priorityRotation.getSkillFrequency(PetAttacks.Claw); if (clawfreq > 0) { clawbitesmackinterval += clawfreq; }
            bitefreq = priorityRotation.getSkillFrequency(PetAttacks.Bite); if (bitefreq > 0) { clawbitesmackinterval += bitefreq; }
            smakfreq = priorityRotation.getSkillFrequency(PetAttacks.Smack); if (smakfreq > 0) { clawbitesmackinterval += smakfreq; }
            PetClawBiteSmackInterval = clawbitesmackinterval;
            #endregion

            PetStats = petStatsTotal;

            #region Special Abilities Priority Rotation
            CalculateTimings();
            #endregion
            #region Kill Command MPS
            calculatedStats.petKillCommandMPS = 0;
            killCommandCooldown = 0;
            if (CalcOpts.useKillCommand) {
                float killCommandManaCost = 0.03f * calculatedStats.baseMana;

                float killCommandReadinessFactor = calculatedStats.priorityRotation.containsShot(Shots.Readiness) ? 1.0f / 180f : 0f;
                float killCommandCooldownBase = 1.0f / (60f - Talents.CatlikeReflexes * 10f);

                killCommandCooldown = 1.0f / (killCommandCooldownBase + killCommandReadinessFactor);

                calculatedStats.petKillCommandMPS = killCommandCooldown > 0 ? killCommandManaCost / killCommandCooldown : 0;
            }
            #endregion
            #region Target Debuffs
            float armorDebuffSporeCloud = 0;
            float sporeCloudFrequency = priorityRotation.getSkillFrequency(PetAttacks.SporeCloud);
            if (sporeCloudFrequency > 0)
            {
                float sporeCloudDuration = 9f;
                float sporeCloudUptime = sporeCloudDuration > sporeCloudFrequency ? 1f : sporeCloudDuration / sporeCloudFrequency;

                armorDebuffSporeCloud = sporeCloudUptime * 0.03f;
            }

            float armorDebuffAcidSpit = 0;
            float acidSpitFrequency = priorityRotation.getSkillFrequency(PetAttacks.AcidSpit); // AcidSpitEffectiveRate
            if (acidSpitFrequency > 0)
            {
                float acidSpitCalcFreq = priorityRotation.getSkillCooldown(PetAttacks.AcidSpit);
                float acidSpitDuration = 30;

                float acidSpitChanceToApply = 1f - PetChanceToMiss - PetChanceToBeDodged; // V45
                float acidSpitChancesToMaintain = (float)Math.Floor((acidSpitDuration - 1f) / acidSpitFrequency); // V46
                float acidSpitChanceToApplyFirst = acidSpitChancesToMaintain == 0 ? 0 : acidSpitChanceToApply; // V47
                float acidSpitChanceToStop = 1f - acidSpitChanceToApplyFirst; // AcidSpitChanceToStop
                float acidSpitAverageTimeToInc = acidSpitChanceToApplyFirst > 0 ? acidSpitFrequency : 0; // AcidSpitTimeToInc
                float acidSpitTimeSpentAtMax = acidSpitAverageTimeToInc + (acidSpitDuration * acidSpitChanceToStop); // AcidSpitAverageStackTime

                PetSkillStack[] stacks = new PetSkillStack[3];
                stacks[0] = new PetSkillStack();
                stacks[1] = new PetSkillStack();
                stacks[2] = new PetSkillStack();

                stacks[0].time_to_reach = 0;
                stacks[1].time_to_reach = acidSpitAverageTimeToInc * 1;
                stacks[2].time_to_reach = acidSpitAverageTimeToInc * 2;

                stacks[0].chance_to_max = 0;
                stacks[1].chance_to_max = acidSpitChanceToStop == 1 ? 0 : acidSpitChanceToStop;
                stacks[2].chance_to_max = acidSpitChanceToStop == 1 ? 0 : 1 - (stacks[0].chance_to_max + stacks[1].chance_to_max);

                stacks[0].time_spent = stacks[1].time_to_reach;
                stacks[1].time_spent = stacks[1].time_to_reach == 0 ? 0 : acidSpitTimeSpentAtMax * (1 - stacks[0].chance_to_max);
                stacks[2].time_spent = stacks[1].time_to_reach == 0 ? 0 : acidSpitChanceToStop == 0 ? 1 : 1 / acidSpitChanceToStop * acidSpitCalcFreq * (1-(stacks[0].chance_to_max + stacks[1].chance_to_max));

                float acidSpitTotalTime = stacks[0].time_spent + stacks[1].time_spent + stacks[2].time_spent;

                stacks[0].percent_time = acidSpitTotalTime == 0 ? 1 : stacks[0].time_spent / acidSpitTotalTime;
                stacks[1].percent_time = acidSpitTotalTime == 0 ? 1 : stacks[1].time_spent / acidSpitTotalTime;
                stacks[2].percent_time = acidSpitTotalTime == 0 ? 1 : stacks[2].time_spent / acidSpitTotalTime;

                stacks[0].total = stacks[0].percent_time * 0.0f;
                stacks[1].total = stacks[0].percent_time * 0.1f;
                stacks[2].total = stacks[0].percent_time * 0.2f;

                armorDebuffAcidSpit = stacks[0].total + stacks[1].total + stacks[2].total;
            }

            float armorDebuffSting = 0;
            float stingFrequency = priorityRotation.getSkillFrequency(PetAttacks.Sting);
            if (stingFrequency > 0) {
                float stingUseFreq = priorityRotation.getSkillFrequency(PetAttacks.Sting);
                float stingDuration = 20;
                float stingUptime = stingDuration > stingUseFreq ? 1 : stingDuration / stingUseFreq;

                armorDebuffSting = stingUptime * 0.05f;
            }

            // these local buffs can be overridden
            if (character.ActiveBuffsConflictingBuffContains("Sting")) {
                armorDebuffSporeCloud = 0;
                armorDebuffSting = 0;
            }
            if (character.ActiveBuffsConflictingBuffContains("Acid Spit")) {
                armorDebuffAcidSpit = 0;
            }

            calculatedStats.petArmorDebuffs = 0f - (1f - armorDebuffSporeCloud) * (1f - armorDebuffAcidSpit) * (1f - armorDebuffSting) + 1;

            #endregion
            #region Hunter Effects
            // Ferocious Inspiraion
            // (Same as above)
            calculatedStats.ferociousInspirationDamageAdjust = 1;
            if (character.HunterTalents.FerociousInspiration > 0) {
                if (CalcOpts.PetFamily != PetFamily.None) {
                    float ferociousInspirationSpecialsEffect = priorityRotation.petSpecialFrequency == 0 ? 0 : 10f / priorityRotation.petSpecialFrequency;
                    float ferociousInspirationUptime = 1f - (float)Math.Pow(1f - calculatedStats.petCritTotalSpecials, (10f / attackSpeedEffective) + ferociousInspirationSpecialsEffect);
                    float ferociousInspirationEffect = 0.01f * character.HunterTalents.FerociousInspiration;

                    calculatedStats.ferociousInspirationDamageAdjust = 1.0f + ferociousInspirationUptime * ferociousInspirationEffect;                    
                }
            }

            // Roar of Recovery
            calculatedStats.manaRegenRoarOfRecovery = 0;
            float roarOfRecoveryFreq = priorityRotation.getSkillFrequency(PetAttacks.RoarOfRecovery);
            if (roarOfRecoveryFreq > 0) {
#if RAWR3 || SILVERLIGHT
                float roarOfRecoveryUseCount = (float)Math.Ceiling(BossOpts.BerserkTimer / roarOfRecoveryFreq);
                float roarOfRecoveryManaRestored = HunterStats.Mana * 0.30f * roarOfRecoveryUseCount; // E129
                calculatedStats.manaRegenRoarOfRecovery = roarOfRecoveryUseCount > 0 ? roarOfRecoveryManaRestored / BossOpts.BerserkTimer : 0;
#else
                float roarOfRecoveryUseCount = (float)Math.Ceiling(CalcOpts.Duration / roarOfRecoveryFreq);
                float roarOfRecoveryManaRestored = HunterStats.Mana * 0.30f * roarOfRecoveryUseCount; // E129
                calculatedStats.manaRegenRoarOfRecovery = roarOfRecoveryUseCount > 0 ? roarOfRecoveryManaRestored / CalcOpts.Duration : 0;
#endif
            }

            //Invigoration
            //calculatedStats.manaRegenInvigoration = 0;
            float invigorationProcChance = Talents.Invigoration * 0.50f; // C32
            if (invigorationProcChance > 0) {
                float invigorationProcFreq = (priorityRotation.petSpecialFrequency / calculatedStats.petCritTotalSpecials) / invigorationProcChance; //C35
                float invigorationEffect = Talents.Invigoration > 0 ? 0.01f : 0;
                float invigorationManaGainedPercent = invigorationProcFreq > 0 ? 60f / invigorationProcFreq * invigorationEffect : 0; // C36
                float invigorationManaPerMinute = invigorationProcFreq > 0 ? 60f / invigorationProcFreq * invigorationEffect * HunterStats.Mana : 0; // C37
                calculatedStats.manaRegenInvigoration = invigorationManaPerMinute / 60f;
            }
            #endregion

            #region Target Armor Effect
            //31-10-2009 Drizz: added Armor effect
#if RAWR3 || SILVERLIGHT
            double petEffectiveArmor = BossOpts.Armor * (1f - calculatedStats.petArmorDebuffs);
            calculatedStats.petTargetArmorReduction = StatConversion.GetArmorDamageReduction(character.Level, BossOpts.Armor, calculatedStats.petArmorDebuffs, 0, 0);
#else
            double petEffectiveArmor = CalcOpts.TargetArmor * (1f - calculatedStats.petArmorDebuffs);
            calculatedStats.petTargetArmorReduction = StatConversion.GetArmorDamageReduction(character.Level, CalcOpts.TargetArmor, calculatedStats.petArmorDebuffs, 0, 0);
#endif
            //petEffectiveArmor/(petEffectiveArmor - 22167.5f + (467.5f*80f));
            #endregion
        }

        public void calculateDPS()
        {
            if (CalcOpts.PetFamily == PetFamily.None) return;

#if RAWR3 || SILVERLIGHT
            int levelDifference = BossOpts.Level - character.Level;
#else
            int levelDifference = CalcOpts.TargetLevel - character.Level;
#endif

            // setup
            #region Attack Power
            float damageBonusMeleeFromAP  = 0.07f * PetStats.AttackPower; // PetAPBonus
            float damageBonusSpellsFromAP = (1.5f / 35f) * PetStats.AttackPower; // PetSpellScaling * PetAP
            #endregion
            #region Spell Hit
            // Full Resists (Spell Hit)
            float levelResistFactor = 0.04f;
            if (levelDifference == 1) levelResistFactor = 0.05f;
            if (levelDifference == 2) levelResistFactor = 0.06f;
            if (levelDifference == 3) levelResistFactor = 0.08f;
            
            float fullResist = levelResistFactor - (1f - PetChanceToSpellMiss);
            if (fullResist < 0) fullResist = 0;
            float fullResistDamageAdjust = 1f - fullResist;

            // Partial Resists (Spell Hit)
#if RAWR3 || SILVERLIGHT
            float averageResist = (BossOpts.Level - character.Level) * 0.02f;
#else
            float averageResist = (CalcOpts.TargetLevel - character.Level) * 0.02f;
#endif
            float resist10 = 5.0f * averageResist;
            float resist20 = 2.5f * averageResist;
            float partialResistDamageAdjust = 1f - (resist10 * 0.1f + resist20 * 0.2f);
            #endregion
            #region Damage Adjustments
            // Dodge adjust can't help us, only hinder
            float damageAdjustDodge = Math.Min(1f, 1f - PetChanceToBeDodged);

            float damageAdjustHitCritMelee    = (2f * calculatedStats.petCritTotalMelee   ) + (1f - (PetChanceToMiss + PetChanceToBeDodged) - calculatedStats.petCritTotalMelee);
            float damageAdjustHitCritSpecials = (2f * calculatedStats.petCritTotalSpecials) + (1f - (PetChanceToMiss + PetChanceToBeDodged) - calculatedStats.petCritTotalSpecials); // CritAdjustments

            float damageAdjustFerociousInspiration = calculatedStats.ferociousInspirationDamageAdjust;
            float damageAdjustTargetDebuffs = calculatedStats.targetDebuffsPetDamage;
            float damageAdjustPetFamily = 1.05f;
            float damageAdjustMarkedForDeath = 1f + (Talents.MarkedForDeath * 0.02f);
#if RAWR3 || SILVERLIGHT
            float damageAdjustCobraReflexes = 1f - (PetTalents.CobraReflexes * 0.075f); // this is a negative effect!
#else
            float damageAdjustCobraReflexes = 1f - (PetTalents.CobraReflexes.Value * 0.075f); // this is a negative effect!
#endif

            // Feeding Frenzy
            float damageAdjustFeedingFrenzy = 1;
#if RAWR3 || SILVERLIGHT
            if (PetTalents.FeedingFrenzy > 0) {
                float feedingFrenzyTimeSpent = ((float)BossOpts.Under20Perc + (float)BossOpts.Under35Perc) * BossOpts.BerserkTimer;
                float feedingFrenzyUptime = feedingFrenzyTimeSpent > 0 ? feedingFrenzyTimeSpent / BossOpts.BerserkTimer : 0;
                damageAdjustFeedingFrenzy = 1f + feedingFrenzyUptime * PetTalents.FeedingFrenzy * 0.08f;
            }
#else
            if (PetTalents.FeedingFrenzy.Value > 0) {
                float feedingFrenzyTimeSpent = CalcOpts.TimeSpentSub20 + CalcOpts.TimeSpent35To20;
                float feedingFrenzyUptime = feedingFrenzyTimeSpent > 0 ? feedingFrenzyTimeSpent / CalcOpts.Duration : 0;
                damageAdjustFeedingFrenzy = 1f + feedingFrenzyUptime * PetTalents.FeedingFrenzy.Value * 0.08f;
            }
#endif

            // Glancing Blows
#if RAWR3 || SILVERLIGHT
            float glancingBlowsSkillDiff = (BossOpts.Level * 5) - (CalcOpts.PetLevel * 5); // F55
#else
            float glancingBlowsSkillDiff = (CalcOpts.TargetLevel * 5) - (CalcOpts.PetLevel * 5); // F55
#endif
            if (glancingBlowsSkillDiff < 0) glancingBlowsSkillDiff = 0;
            float glancingBlowsChance  = glancingBlowsSkillDiff > 15 ? 0.25f : 0.1f + glancingBlowsSkillDiff * 0.01f; // F56
            float glancingBlowsLowEnd  = (float)Math.Min(1.3f - 0.05f * glancingBlowsSkillDiff, 0.91f); // F57
            float glancingBlowsHighEnd = (float)Math.Min(1.2f - 0.03f * glancingBlowsSkillDiff, 0.99f); // F58
            float damageAdjustGlancingBlows = 1 - (glancingBlowsChance * (1 - ((glancingBlowsLowEnd + glancingBlowsHighEnd) / 2)));

            // Savage Rend
            float damageAdjustSavageRend = 1;
            float savageRendFrequency = priorityRotation.getSkillFrequency(PetAttacks.SavageRend);
            if (savageRendFrequency > 0)
            {
                float savageRendTimeBetweenProcs = savageRendFrequency * (1f / calculatedStats.petCritTotalSpecials);
                float savageRendUptime = savageRendTimeBetweenProcs > 0 ? 30f / savageRendTimeBetweenProcs : 0;
                damageAdjustSavageRend = 1f + 0.1f * savageRendUptime;
            }

            #region Monstrous Bite
            float damageAdjustMonstrousBite = 1;
            float monstrousBiteFrequency = priorityRotation.getSkillFrequency(PetAttacks.MonstrousBite);
            if (monstrousBiteFrequency > 0)
            {
                float monstrousBiteUseFreq = monstrousBiteFrequency;
                float monstrousBiteDuration = 12;

                float monstrousBiteChanceToApply = 1f - PetChanceToMiss - PetChanceToBeDodged;
                float monstrousBiteChancesToMaintain = 1;

                float monstrousBiteChanceToApplyFirst = monstrousBiteChancesToMaintain == 0 ? 0 : monstrousBiteChanceToApply;
                float monstrousBiteChanceToStop = 1f - monstrousBiteChanceToApplyFirst;

                float monstrousBiteAverageIncTime = 1f - monstrousBiteChanceToStop == 0 ? 0 : monstrousBiteUseFreq;
                float monstrousBiteAverageStackTime = monstrousBiteAverageIncTime + monstrousBiteDuration * monstrousBiteChanceToStop;

                PetSkillStack[] stacks = new PetSkillStack[4];
                stacks[0] = new PetSkillStack();
                stacks[1] = new PetSkillStack();
                stacks[2] = new PetSkillStack();
                stacks[3] = new PetSkillStack();

                stacks[0].time_to_reach = 0;
                stacks[1].time_to_reach = 1 * monstrousBiteAverageIncTime;
                stacks[2].time_to_reach = 2 * monstrousBiteAverageIncTime;
                stacks[3].time_to_reach = 3 * monstrousBiteAverageIncTime;

                stacks[0].chance_to_max = 0;
                stacks[1].chance_to_max = monstrousBiteChanceToStop == 1 ? 0 : monstrousBiteChanceToStop;
                stacks[2].chance_to_max = monstrousBiteChanceToStop == 1 ? 0 : monstrousBiteChanceToStop * (1 - stacks[1].chance_to_max);
                stacks[3].chance_to_max = monstrousBiteChanceToStop == 1 ? 0 : 1 - (stacks[0].chance_to_max + stacks[1].chance_to_max + stacks[2].chance_to_max);

                stacks[0].time_spent = stacks[3].chance_to_max == 1 ? 0 : stacks[1].time_to_reach;
                stacks[1].time_spent = stacks[3].chance_to_max == 1 ? 0 : (stacks[1].time_to_reach == 0 ? 0 : monstrousBiteAverageStackTime) * (1 - (stacks[0].chance_to_max));
                stacks[2].time_spent = stacks[3].chance_to_max == 1 ? 0 : (stacks[2].time_to_reach == 0 ? 0 : monstrousBiteAverageStackTime) * (1 - (stacks[0].chance_to_max + stacks[1].chance_to_max));
                stacks[3].time_spent = monstrousBiteChanceToStop == 0 ? 1 : stacks[3].time_to_reach == 0 ? 0 : 1 / monstrousBiteChanceToStop * monstrousBiteUseFreq * (1 - (stacks[0].chance_to_max + stacks[1].chance_to_max + stacks[2].chance_to_max));

                float monstrousBiteTotalTime = stacks[0].time_spent + stacks[1].time_spent + stacks[2].time_spent + stacks[3].time_spent;

                stacks[0].percent_time = monstrousBiteTotalTime == 0 ? 1 : stacks[0].time_spent / monstrousBiteTotalTime;
                stacks[1].percent_time = monstrousBiteTotalTime == 0 ? 0 : stacks[1].time_spent / monstrousBiteTotalTime;
                stacks[2].percent_time = monstrousBiteTotalTime == 0 ? 0 : stacks[2].time_spent / monstrousBiteTotalTime;
                stacks[3].percent_time = monstrousBiteTotalTime == 0 ? 0 : stacks[3].time_spent / monstrousBiteTotalTime;

                stacks[0].total = 0f;
                stacks[1].total = 0.03f * stacks[1].percent_time;
                stacks[2].total = 0.06f * stacks[1].percent_time;
                stacks[3].total = stacks[3].chance_to_max == 1f ? 0.09f : 0.09f * stacks[1].percent_time;

                float monstrousBiteProcEffect = stacks[0].total + stacks[1].total + stacks[2].total + stacks[3].total;

                damageAdjustMonstrousBite = 1f + monstrousBiteProcEffect;
            }
            #endregion
            float damageAdjustMangle = 1f + PetStats.BonusBleedDamageMultiplier;

            // Pets don't get ArP Ratings passed, spreadsheet agrees and no mention of it in the community
#if RAWR3 || SILVERLIGHT
            float damageAdjustMitigation = 1f - StatConversion.GetArmorDamageReduction(BossOpts.Level, BossOpts.Armor, StatsPetBuffs.ArmorPenetration, 0f, 0f);
#else
            float damageAdjustMitigation = 1f - StatConversion.GetArmorDamageReduction(CalcOpts.TargetLevel, CalcOpts.TargetArmor, StatsPetBuffs.ArmorPenetration, 0f, 0f);
#endif

            float damageAdjustBase = 1f
                    * (1f + PetStats.BonusDamageMultiplier)
                    * (1f + PetStats.BonusPetDamageMultiplier)
                    * damageAdjustFerociousInspiration
                    * damageAdjustMonstrousBite
                    * damageAdjustSavageRend
                    * damageAdjustFeedingFrenzy
                    * damageAdjustTargetDebuffs
                    * damageAdjustPetFamily;

            float damageAdjustDots     = damageAdjustBase; // BasePetModifier
            float damageAdjustWhite    = damageAdjustBase * damageAdjustHitCritMelee;
            float damageAdjustMelee    = damageAdjustBase * damageAdjustHitCritSpecials; // MeleeAttackAdjustment
            float damageAdjustSpecials = damageAdjustBase * damageAdjustHitCritSpecials * damageAdjustMarkedForDeath; // DamageAdjustment
            float damageAdjustMagic    = damageAdjustBase * damageAdjustMarkedForDeath / damageAdjustTargetDebuffs; // MagicDamageAdjustments

            #endregion

            // damage
            #region White Damage
            float whiteDamageBase = (52f + 78f) / 2f;
            float whiteDamageFromAP = (float)Math.Floor(PetStats.AttackPower / 14f * 2f);
            float whiteDamageNormal = whiteDamageBase + whiteDamageFromAP;
            float whiteDamageAdjust = damageAdjustWhite * damageAdjustCobraReflexes * damageAdjustMitigation * damageAdjustGlancingBlows;
            float whiteDamageReal = whiteDamageNormal * whiteDamageAdjust;

            float whiteDPS = whiteDamageReal / attackSpeedEffective;

            calculatedStats.petWhiteDPS = whiteDPS;
            #endregion
            #region Priority Rotation
            // loop over each skill, figuring out the damage value
            foreach (PetSkillInstance S in priorityRotation.skills) {
                S.damage = 0;

                #region Skill Groups
                if (S.skillData.type == PetSkillType.FocusDump) {
                    float focusDumpDamageAverage = ((118f + 168f) / 2f) + damageBonusMeleeFromAP;
                    S.damage = focusDumpDamageAverage * damageAdjustSpecials * damageAdjustMitigation;
                }
                if (S.skillData.type == PetSkillType.SpecialMelee) {
                    float meleeDamageAverage = S.skillData.average + damageBonusMeleeFromAP;
                    S.damage = meleeDamageAverage * damageAdjustSpecials * damageAdjustMitigation;
                }
                if (S.skillData.type == PetSkillType.SpecialSpell) {
                    float spellDamageAverage = S.skillData.average + damageBonusSpellsFromAP;
                    S.damage = spellDamageAverage * critSpecialsAdjust * fullResistDamageAdjust * partialResistDamageAdjust 
                                * damageAdjustMagic * calculatedStats.targetDebuffsNature;
                }
                #endregion
                #region Unique Skills
                if (S.skillType == PetAttacks.Rake) {
                    float rakeDamageFromAP = PetStats.AttackPower * 0.0175f;
                    float rakeAverageDamage = ((47  + 67) / 2) + rakeDamageFromAP;
                    float rakeAverageDamageDot = ((19 + 25) / 2) + rakeDamageFromAP;

                    float rakeInitialHitDamage = rakeAverageDamage * damageAdjustSpecials * damageAdjustMangle; // spreadsheet doesn't add armor mitigation, appears to be wow bug?
                    float rakeDotDamage = rakeAverageDamageDot * damageAdjustMangle * damageAdjustDots;
                    float rakeDots = S.frequency > 9 ? 3 : 2;

                    S.damage = rakeInitialHitDamage + rakeDotDamage * rakeDots;
                }else if(S.skillType == PetAttacks.FireBreath) {
                    float fireBreathDamageAverage = ((43 + 57) / 2) + damageBonusSpellsFromAP;
                    float fireBreathDamageInitial = fireBreathDamageAverage * fullResistDamageAdjust * partialResistDamageAdjust * damageAdjustMagic;
                    float fireBreathDamageDot = 50 + damageBonusSpellsFromAP;
                    S.damage = fireBreathDamageInitial + fireBreathDamageDot;
                }else if (S.skillType == PetAttacks.SavageRend) {
                    float savageRendAverageHitDamage = ((59 + 83) / 2) + damageBonusMeleeFromAP;
                    float savageRendAverageBleedDamage = ((21 + 27) / 2) + damageBonusMeleeFromAP;
                    
                    float savageRendHitDamage = savageRendAverageHitDamage * damageAdjustSpecials * damageAdjustMangle * damageAdjustMitigation;
                    float savageRendBleedDamage = savageRendAverageBleedDamage * damageAdjustDots * damageAdjustMangle;

                    S.damage = savageRendHitDamage + 3 * savageRendBleedDamage;
                }else if (S.skillType == PetAttacks.ScorpidPoison) {
                    float scorpidPoisionDamageAverage = ((100 + 130) / 2) + damageBonusSpellsFromAP;
                    float scorpidPoisionDamageNormal = scorpidPoisionDamageAverage * fullResistDamageAdjust * partialResistDamageAdjust
                                                        * damageAdjustMagic * calculatedStats.targetDebuffsNature;
                    float scorpidPoisionDamageTick = scorpidPoisionDamageNormal / 5;
                    float scorpidPoisionTicks = (float)Math.Floor(S.cooldown / 2f);
                    S.damage = scorpidPoisionDamageTick * scorpidPoisionTicks;
                }else if (S.skillType == PetAttacks.PoisonSpit) {
                    float poisonSpitDamageAverage = ((104 + 136) / 2) + damageBonusSpellsFromAP;
                    float poisonSpitDamageNormal = poisonSpitDamageAverage * fullResistDamageAdjust * partialResistDamageAdjust 
                                                    * damageAdjustMagic * calculatedStats.targetDebuffsNature;
                    float poisonSpitDamageTick = poisonSpitDamageNormal / 3;
                    float poisonSpitTicks = S.cooldown >= 9 ? 3 : 2;
                    S.damage = poisonSpitDamageTick * poisonSpitTicks;
                }else if (S.skillType == PetAttacks.VenomWebSpray) {
                    float venomWebSprayDamageAverage = ((46 + 68) / 2) + damageBonusSpellsFromAP;
                    float venomWebSprayDamageNormal = venomWebSprayDamageAverage * fullResistDamageAdjust * partialResistDamageAdjust
                                                        * damageAdjustMagic * calculatedStats.targetDebuffsNature;
                    S.damage = venomWebSprayDamageNormal * 4;
                }else if (S.skillType == PetAttacks.SpiritStrike) {
                    float spiritStrikeAdjust = fullResistDamageAdjust * partialResistDamageAdjust * damageAdjustMagic
                                                     * calculatedStats.targetDebuffsNature;

                    float spiritStrikeInitialDamageAverage = ((49 + 65) / 2) + damageBonusSpellsFromAP;
                    float spiritStrikeInitialDamageNormal = spiritStrikeInitialDamageAverage * spiritStrikeAdjust * damageAdjustHitCritSpecials;

                    float spiritStrikeDotDamageAverage = ((49 + 65) / 2) + damageBonusSpellsFromAP;
                    float spiritStrikeDotDamageNormal = spiritStrikeDotDamageAverage * spiritStrikeAdjust;

                    S.damage = spiritStrikeInitialDamageNormal + spiritStrikeDotDamageNormal;
                }else if (S.skillType == PetAttacks.SporeCloud) {
                    float spellDamageAverage = S.skillData.average + damageBonusSpellsFromAP;
                    S.damage = spellDamageAverage * fullResistDamageAdjust * partialResistDamageAdjust
                                    * damageAdjustMagic * calculatedStats.targetDebuffsNature;
                }else if (S.skillType == PetAttacks.AcidSpit) {
                    float acidSpitAverage = S.skillData.average + damageBonusSpellsFromAP;
                    S.damage = acidSpitAverage * (calculatedStats.petCritTotalSpecials * 1.5f + 1) * fullResistDamageAdjust * partialResistDamageAdjust * damageAdjustMagic * calculatedStats.targetDebuffsNature;
                }
                #endregion

                S.CalculateDPS();
            }
            // now add everything up...
            priorityRotation.calculateDPS();
            #endregion
            #region Kill Command

            float killCommandDPS = 0;

            if (killCommandCooldown > 0)
            {
                float killCommandDamagePerSpecial = priorityRotation.petSpecialFrequency > 0 ? priorityRotation.kc_dps / (1 / priorityRotation.petSpecialFrequency) : 0;

                float killCommandBonusDamage = killCommandDamagePerSpecial * 1.2f;

                float killCommandDPSOverCooldown = killCommandCooldown > 0 ? killCommandBonusDamage / killCommandCooldown : 0;

                float killCommandFocusedFireCritBonus = character.HunterTalents.FocusedFire * 0.1f;
                // 31-10-2009 Drizz: Remade the AdjustedBonus Calculation from the one below
                // float killCommandAdjustedBonus = killCommandFocusedFireCritBonus *  damageAdjustDodge * damageAdjustMitigation;
                float killCommandAdjustedBonus = killCommandFocusedFireCritBonus
                                               * (2f - 1f)
                                               * (1f - calculatedStats.petTargetArmorReduction);

                killCommandDPS = killCommandDPSOverCooldown * (1f + killCommandAdjustedBonus);
            }

            calculatedStats.petKillCommandDPS = killCommandDPS;

            #endregion

            calculatedStats.petSpecialDPS = priorityRotation.dps;

            calculatedStats.PetDpsPoints = (float)(calculatedStats.petWhiteDPS 
                                                + calculatedStats.petSpecialDPS 
                                                + calculatedStats.petKillCommandDPS);
        }
    }
}
