using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Rawr.Hunter
{
    public class CharacterCalculationsHunter : CharacterCalculationsBase
    {
        private float _overallPoints = 0f;
        private float[] _subPoints = new float[] { 0f, 0f, 0f, 0f };
        private Stats _basicStats;
        private float _baseAttackSpeed;
        private float _autoshotDPS;
        private float _BonusAttackProcsDPS;
        private float _wildQuiverDPS;
        public float SpecProcDPS;
        private float _customDPS;
        public Character character = null;
        public CalculationOptionsHunter CalcOpts = null;
        public BossOptions BossOpts = null;

        #region Skills
        public Skills.WhiteAttacks Whites { get; set; }
        public Skills.ExplosiveShot Explosive { get; set; }
        public Skills.SteadyShot Steady { get; set; }
        public Skills.AimedShot Aimed { get; set; }
        public Skills.MultiShot Multi { get; set; }
        public Skills.ArcaneShot Arcane { get; set; }
        public Skills.KillShot Kill { get; set; }
        public Skills.SilencingShot Silencing { get; set; }
        public Skills.Volley Volley { get; set; }
        public Skills.BlackArrowDoT BlackArrowD { get; set; }
        public Skills.BlackArrowBuff BlackArrowB { get; set; }
        public Skills.PiercingShots Piercing { get; set; }
        public Skills.SerpentSting Serpent { get; set; }
        public Skills.ChimeraShot_Serpent Chimera { get; set; }
        public Skills.ScorpidSting Scorpid { get; set; }
        public Skills.ViperSting Viper { get; set; }
        public Skills.ImmolationTrap Immolation { get; set; }
        public Skills.ExplosiveTrap ExplosiveT { get; set; }
        public Skills.FreezingTrap Freezing { get; set; }
        public Skills.FrostTrap Frost { get; set; }
        public Skills.Readiness Ready { get; set; }
        public Skills.BestialWrath Bestial { get; set; }
        public Skills.RapidFire Rapid { get; set; }
        #endregion

        private double _piercingShotsDPS;
        private double _piercingShotsDPSSteadyShot;
        private double _piercingShotsDPSAimedShot;
        private double _piercingShotsDPSChimeraShot;

        public float BaseHealth { get; set; }

        public PetCalculations pet { get; set; }

        #region Pet Stats
        public float petKillCommandDPS { get; set; }
        public float petKillCommandMPS { get; set; }
        public float petWhiteDPS { get; set; }
        public float petSpecialDPS { get; set; }
        public float petArmorDebuffs { get; set; }
        public float petTargetArmorReduction { get; set; }
        public float petTargetDodge { get; set; }
        public float ferociousInspirationDamageAdjust { get; set; }
        #endregion

        #region Pet Hit
        public float petHitTotal { get; set; }
        public float petHitSpellTotal { get; set; }
        #endregion

        #region Pet Crit
        public float petCritFromBase { get; set; }
        public float petCritFromAgility { get; set; }
        public float petCritFromSpidersBite { get; set; }
        public float petCritFromFerocity { get; set; }
        public float petCritFromGear { get; set; }
        public float petCritFromBuffs { get; set; }
        public float petCritFromTargetDebuffs { get; set; }
        public float petCritFromDepression { get; set; }
        public float petCritFromCobraStrikes { get; set; }
        public float petCritTotalMelee { get; set; }
        public float petCritTotalSpecials { get; set; }
        #endregion

        #region Pet AP
        public float petAPFromStrength { get; set; }
        public float petAPFromHunterVsWild { get; set; }
        public float petAPFromTier9 { get; set; }
        public float petAPFromBuffs { get; set; }
        public float petAPFromHunterRAP { get; set; }
        public float petAPFromRabidProc { get; set; }
        public float petAPFromSerenityDust { get; set; }
        public float petAPFromOutsideBuffs { get; set; }
        public float petAPFromAnimalHandler { get; set; }
        public float petAPFromAspectOfTheBeast { get; set; }
        #endregion

        #region Debuffs
        public float targetDebuffsCrit { get; set; }
        public float targetDebuffsArmor { get; set; }
        public float targetDebuffsNature { get; set; }
        public float targetDebuffsPetDamage { get; set; }
        #endregion

        #region Haste Stats
        public float hasteFromTalentsStatic { get; set; }
        public float hasteFromRapidFire { get; set; }
        public float hasteFromProcs { get; set; }
        public float hasteFromBase { get; set; }
        public float hasteFromRating { get; set; }
        public float hasteFromRangedBuffs { get; set; }
        public float hasteFromRacial { get; set; }
        public float hasteStaticTotal { get; set; }
        public float hasteDynamicTotal { get; set; }
        public float hasteEffectsTotal { get; set; }
        #endregion   		
        
        #region RAP Stats
        public float apTotal { get; set; }
        public float apSelfBuffed { get; set; }
        public float apFromBase { get; set; }
        public float apFromAGI { get; set; }
        public float apFromGear { get; set; }
        #endregion
        
        #region Hit Stats
        public float hitOverall {get; set;}
        public float hitFromBase { get; set; }
        public float hitFromRating { get; set; }
        public float hitFromTalents {get; set;}
        public float hitFromBuffs {get; set;}
        public float hitFromTargetDebuffs { get; set; }
        public float hitFromLevelAdjustment { get; set; }       
        #endregion        
        
        #region Crit Stats
        public float critRateOverall {get; set;}
        public float critBase { get; set; }
        public float critFromRacial { get; set; }
        public float critFromAgi { get; set; }
        public float critFromRating { get; set; }
        public float critFromProcRating { get; set; }
        public float critFromLethalShots { get; set; }
        public float critFromKillerInstincts { get; set; }
        public float critFromMasterMarksman { get; set; }
        public float critFromMasterTactician { get; set; }
        public float critFromBuffs { get; set; }
        public float critFromDepression { get; set; }
        #endregion

        #region Shots Per Second
        public float shotsPerSecondCritting { get; set; }
        #endregion

        public float damageReductionFromArmor { get; set; }
        public float baseMana { get; set; }

        // stuff for rotation test
        //public float autoShotSpeed { get; set; }
        public float autoShotStaticSpeed { get; set; }

        // new shots data
        public ShotData aimedShot = new ShotData(Shots.AimedShot, false, true, true);
        public ShotData arcaneShot = new ShotData(Shots.ArcaneShot, false, true, true);
        public ShotData multiShot = new ShotData(Shots.MultiShot, false, true, true);
        public ShotData serpentSting = new ShotData(Shots.SerpentSting, false, true, true);
        public ShotData scorpidSting = new ShotData(Shots.ScorpidSting, false, false, true);
        public ShotData viperSting = new ShotData(Shots.ViperSting, false, false, true);
        public ShotData silencingShot = new ShotData(Shots.SilencingShot, false, true, false);
        public ShotData steadyShot = new ShotData(Shots.SteadyShot, false, true, true);
        public ShotData killShot = new ShotData(Shots.KillShot, false, true, true);
        public ShotData explosiveShot = new ShotData(Shots.ExplosiveShot, false, true, true);
        public ShotData blackArrow = new ShotData(Shots.BlackArrow, false, false, true);
        public ShotData immolationTrap = new ShotData(Shots.ImmolationTrap, false, false, true);
        public ShotData explosiveTrap = new ShotData(Shots.ExplosiveTrap, false, false, true);
        public ShotData freezingTrap = new ShotData(Shots.FreezingTrap, true, false, true);
        public ShotData frostTrap = new ShotData(Shots.FrostTrap, true, false, true);
        public ShotData volley = new ShotData(Shots.Volley, false, true, true);
        public ShotData chimeraShot = new ShotData(Shots.ChimearaShot, false, true, true);
        public ShotData rapidFire = new ShotData(Shots.RapidFire, true, false, false);
        public ShotData readiness = new ShotData(Shots.Readiness, true, false, true);
        public ShotData bestialWrath = new ShotData(Shots.BestialWrath, true, false, false);

        public ShotPriority priorityRotation = null;

        //***************************************
        // 091109 Drizz: Added
        public bool collectSequence = false;
        public string sequence = "";
        //***************************************
        #region Mana Regen
        public float manaRegenGearBuffs {get; set;}
        public float manaRegenConstantViper { get; set; }
        public float manaRegenRoarOfRecovery { get; set; }
        public float manaRegenRapidRecuperation { get; set; }
        public float manaRegenChimeraViperProc { get; set; }
        public float manaRegenInvigoration { get; set; }
        public float manaRegenHuntingParty { get; set; }
        public float manaRegenTargetDebuffs { get; set; }
        public float manaRegenFromPots { get; set; }
        public float manaRegenTotal { get; set; }

        public float manaRegenViper { get; set; }
        #endregion

        #region Mana Usage
        public float manaUsageRotation { get; set; }
        public float manaUsageKillCommand { get; set; }
        public float manaUsageTotal { get; set; }

        public float manaChangeDuringViper { get; set; }
        public float manaChangeDuringNormal { get; set; }
        public float manaTimeToFull { get; set; }
        public float manaTimeToOOM { get; set; }
        #endregion

        #region Kill shot used sub-20%
        public float killShotSub20NewSteadyFreq { get; set; }
        public float killShotSub20NewDPS { get; set; }
        public float killShotSub20NewSteadyDPS { get; set; }
        public float killShotSub20Gain { get; set; }
        public float killShotSub20TimeSpent { get; set; }
        public float killShotSub20FinalGain { get; set; }
        #endregion

        #region Aspect uptime/penalties/bonuses
        public float aspectUptimeHawk { get; set; }
        public float aspectUptimeViper { get; set; }
        public float aspectUptimeBeast { get; set; }
        public float aspectBeastLostDPS { get; set; }
        public float aspectViperPenalty { get; set; }
        public float aspectBonusAPBeast { get; set; }
        public float NoManaDPSDownTimePerc { get; set; }
        #endregion

        public float BaseAttackSpeed
        {
            get { return _baseAttackSpeed; }
            set { _baseAttackSpeed = value; }
        }

        public float AutoshotDPS
        {
            get { return _autoshotDPS; }
            set { _autoshotDPS = value; }
        }

        public float BonusAttackProcsDPS
        {
            get { return _BonusAttackProcsDPS; }
            set { _BonusAttackProcsDPS = value; }
        }

        public float WildQuiverDPS
        {
            get { return _wildQuiverDPS; }
            set { _wildQuiverDPS = value; }
        }

        public double PiercingShotsDPS
        {
            get { return _piercingShotsDPS; }
            set { _piercingShotsDPS = value; }
        }

        public double PiercingShotsDPSSteadyShot
        {
            get { return _piercingShotsDPSSteadyShot; }
            set { _piercingShotsDPSSteadyShot = value; }
        }
        
        public double PiercingShotsDPSAimedShot
        {
            get { return _piercingShotsDPSAimedShot; }
            set { _piercingShotsDPSAimedShot = value; }
        }

        public double PiercingShotsDPSChimeraShot
        {
            get { return _piercingShotsDPSChimeraShot; }
            set { _piercingShotsDPSChimeraShot = value; }
        }

        public float CustomDPS
        {
            get { return _customDPS; }
            set { _customDPS = value; }
        }

        public override float OverallPoints
        {
            get { return _overallPoints; }
            set { _overallPoints = value; }
        }

        public override float[] SubPoints
        {
            get { return _subPoints; }
            set { _subPoints = value; }
        }

        public float HunterDpsPoints
        {
            get { return _subPoints[0]; }
            set { _subPoints[0] = value; }
        }

        public float PetDpsPoints
        {
            get { return _subPoints[1]; }
            set { _subPoints[1] = value; }
        }

        public float HunterSurvPoints
        {
            get { return _subPoints[2]; }
            set { _subPoints[2] = value; }
        }

        public float PetSurvPoints
        {
            get { return _subPoints[3]; }
            set { _subPoints[3] = value; }
        }

        public Stats BasicStats { get { return _basicStats; } set { _basicStats = value; } }
        public Stats AverageStats { get; set; }
        public Stats MaximumStats { get; set; }
        public Stats UnbuffedStats { get; set; }
        public Stats BuffedStats { get; set; }
        public Stats BuffsStats { get; set; } // The actual stats that come from Buffs

        public List<string> ActiveBuffs { get; set; }

        public override Dictionary<string, string> GetCharacterDisplayCalculationValues() {
            Dictionary<string, string> dictValues = new Dictionary<string, string>();
            //string format = "";
            CalcOpts = character.CalculationOptions as CalculationOptionsHunter;

            // Basic Stats
            dictValues.Add("Health and Stamina", string.Format("{0:##,##0} : {1:##,##0}*{2:00,000} : Base Health" +
                                "\r\n{3:00,000} : Stam Bonus",
                                BasicStats.Health, BasicStats.Stamina, BaseHealth, StatConversion.GetHealthFromStamina(BasicStats.Stamina)));
            dictValues.Add("Mana", BasicStats.Mana.ToString("F0"));
            dictValues.Add("Armor", BasicStats.Armor.ToString("F0"));
            dictValues.Add("Agility", BasicStats.Agility.ToString("F0"));
            dictValues.Add("Ranged Attack Power",string.Format("{0:0000}*Includes:" +
                            "\r\n{1:0000} : Base" +
                            "\r\n{2:0000} : Agility" +
                            "\r\n{3:0000} : Gear" +
                            "\r\nProcs were averaged out and added",
                            apTotal, apFromBase, apFromAGI, apFromGear));
            dictValues.Add("Intellect", BasicStats.Intellect.ToString("F0"));
            // old
            float HitPercent = StatConversion.GetHitFromRating(BasicStats.HitRating);
            float HitPercBonus = BasicStats.PhysicalHit - HitPercent;
            // Hit Soft Cap ratings check, how far from it
#if RAWR3 || SILVERLIGHT
            float capA1 = StatConversion.WHITE_MISS_CHANCE_CAP[BossOpts.Level - character.Level];
#else
            float capA1 = StatConversion.WHITE_MISS_CHANCE_CAP[CalcOpts.TargetLevel - character.Level];
#endif
            float convcapA1 = (float)Math.Ceiling(StatConversion.GetRatingFromHit(capA1));
            float sec2lastNumA1 = (convcapA1 - StatConversion.GetRatingFromHit(HitPercent) - StatConversion.GetRatingFromHit(HitPercBonus)) * -1;
            dictValues.Add("Hit",
                string.Format("{0:00.00%} : {1}*" + "{2:0.00%} : From Other Bonuses" +
                                Environment.NewLine + "{3:0.00%} : Total Hit % Bonus" +
                                Environment.NewLine + Environment.NewLine + "Ranged Cap: " +
                                (sec2lastNumA1 > 0 ? "You can free {4:0} Rating"
                                                   : "You need {4:0} more Rating"),
                                StatConversion.GetHitFromRating(BasicStats.HitRating),
                                BasicStats.HitRating,
                                HitPercBonus,
                                HitPercent + HitPercBonus,
                                (sec2lastNumA1 > 0 ? sec2lastNumA1 : sec2lastNumA1 * -1)
                            ));
            dictValues.Add("Crit", string.Format("{0:00.00%} : {1}*Includes:" +
                                "\r\n{2:00.00%} : Base Crit" +
                                "\r\n{3:00.00%} : Agility" +
                                "\r\n{4:00.00%} : Rating" +
                                "\r\n{5:00.00%} : Racial" +
                                "\r\n{6:00.00%} : Proc Effects" +
                                "\r\n{7:00.00%} : Lethal Shots" +
                                "\r\n{8:00.00%} : Killer Instincts" +
                                "\r\n{9:00.00%} : Master Marksman" +
                                "\r\n{10:00.00%} : Master Tactician" +
                                "\r\n{11:00.00%} : Buffs & Debuffs" +
                                "\r\n{12:00.00%} : Level Adjustment" +
                                "\r\n\r\nNote that individual Shots will handle their own crit caps",
                                critRateOverall, BasicStats.CritRating,
                                critBase, critFromAgi, critFromRating, critFromRacial,
                                critFromProcRating, critFromLethalShots, critFromKillerInstincts,
                                critFromMasterMarksman, critFromMasterTactician, critFromBuffs,
                                critFromDepression * -1f));
            dictValues.Add("Armor Penetration", string.Format("{0:00.00%} : {1}" + "*Enemy's Damage Reduction from armor: {2:00.00%}",
                                StatConversion.GetArmorPenetrationFromRating(BasicStats.ArmorPenetrationRating),
                                BasicStats.ArmorPenetrationRating,
                                damageReductionFromArmor));
            dictValues.Add("Haste", string.Format("{0:00.00%} : {1:0}*Includes:" +
                                "\r\n{2:00.00%} : Base" +
                                "\r\n{3:00.00%} : Rating" +
                                "\r\n{4:00.00%} : Serpent's Swiftness" +
                                "\r\n{5:00.00%} : Buffs" +
                                "\r\n{6:00.00%} : Rapid Fire" +
                                "\r\n{7:00.00%} : Proc Effects",
                                BasicStats.PhysicalHaste, BasicStats.HasteRating,
                                hasteFromBase, hasteFromRating, hasteFromTalentsStatic, hasteFromRangedBuffs,
                                hasteFromRapidFire, hasteFromProcs));
            dictValues.Add("Attack Speed", BaseAttackSpeed.ToString("F2"));
            
            // Pet Stats
            dictValues.Add("Pet Attack Power", pet.PetStats.AttackPower.ToString("F0") +
                                                string.Format("*Full Pet Stats:\r\n"
                                                              + "Strength: {0:0.0}\r\n"
                                                              + "Agility: {1:0.0}\r\n"
                                                              + "Hit: {2:0.00%}\r\n"
                                                              + "PhysCrit: {3:0.00%}\r\n"
                                                              + "PhysHaste: {4:0.00%}\r\n",
                                                            pet.PetStats.Strength,
                                                            pet.PetStats.Agility,
                                                            pet.PetStats.PhysicalHit,
                                                            pet.PetStats.PhysicalCrit,
                                                            pet.PetStats.PhysicalHaste));
            dictValues.Add("Pet Hit %", petHitTotal.ToString("P2"));
            dictValues.Add("Pet Dodge %", petTargetDodge.ToString("P2"));
            dictValues.Add("Pet Melee Crit %", petCritTotalMelee.ToString("P2") + "*includes:\n" +
                            petCritFromBase.ToString("P2") + " from base\n" +
                            petCritFromAgility.ToString("P2") + " from agility\n" +
                            petCritFromSpidersBite.ToString("P2") + " from Spider's Bite\n" +
                            petCritFromFerocity.ToString("P2") + " from Ferocity\n" +
                            petCritFromGear.ToString("P2") + " from gear\n" +
                            petCritFromBuffs.ToString("P2") + " from buffs\n" +
                            petCritFromTargetDebuffs.ToString("P2") + " from target debuffs\n" +
                            petCritFromDepression.ToString("P2") + " from depression");
            dictValues.Add("Pet Specials Crit %", petCritTotalSpecials.ToString("P2") + "*includes:\n" +
                            petCritTotalMelee.ToString("P2") + " from melee crit\n" +
                            petCritFromCobraStrikes.ToString("P2") + " from Cobra Strikes");
            dictValues.Add("Pet White DPS", petWhiteDPS.ToString("F2"));
            dictValues.Add("Pet Kill Command DPS", petKillCommandDPS.ToString("F2"));
            dictValues.Add("Pet Specials DPS", petSpecialDPS.ToString("F2") /*+ 
                string.Format("Breakout:\r\n"
                            + "Furious Howl: Use {0} DPS {1:0.00}"
                            + "Bite: Use {2} DPS {3:0.00}",
                            pet.priorityRotation.getSkillFrequency(PetAttacks.FuriousHowl), 0f,
                            pet.priorityRotation.getSkillFrequency(PetAttacks.Bite), pet.priorityRotation.dps - petWhiteDPS)*/);

            // Shot Stats
            dictValues.Add("Aimed Shot", aimedShot.GenTooltip());
            dictValues.Add("Arcane Shot", arcaneShot.GenTooltip());
            dictValues.Add("Multi Shot", multiShot.GenTooltip());
            dictValues.Add("Silencing Shot", silencingShot.GenTooltip());
            dictValues.Add("Steady Shot", steadyShot.GenTooltip());
            dictValues.Add("Kill Shot", killShot.GenTooltip());
            dictValues.Add("Explosive Shot", explosiveShot.GenTooltip());
            dictValues.Add("Black Arrow", blackArrow.GenTooltip());
            dictValues.Add("Volley", volley.GenTooltip());
            dictValues.Add("Chimera Shot", chimeraShot.GenTooltip());
            
            //dictValues.Add("Rapid Fire", rapidFire.GenTooltip());
            //dictValues.Add("Readiness", readiness.GenTooltip());
            //dictValues.Add("Bestial Wrath", bestialWrath.GenTooltip());

            // Sting Stats
            dictValues.Add("Serpent Sting", serpentSting.GenTooltip());
            dictValues.Add("Scorpid Sting", scorpidSting.GenTooltip());
            dictValues.Add("Viper Sting", viperSting.GenTooltip());

            // Trap Stats
            dictValues.Add("Immolation Trap", immolationTrap.GenTooltip());
            dictValues.Add("Explosive Trap", explosiveTrap.GenTooltip());
            dictValues.Add("Freezing Trap", freezingTrap.GenTooltip());
            dictValues.Add("Frost Trap", frostTrap.GenTooltip());

            // Mana
            dictValues.Add("Mana Usage Per Second", manaUsageTotal.ToString("F2") + "*includes:\n" +
                           manaUsageRotation.ToString("F2") + " from shot rotation\n" +
                           manaUsageKillCommand.ToString("F2") + " from Kill Command");
            dictValues.Add("Mana Regen Per Second", manaRegenTotal.ToString("F2") + "*includes:\n" +
                (manaRegenGearBuffs + manaRegenConstantViper + manaRegenViper + manaRegenRoarOfRecovery
                 + manaRegenRapidRecuperation + manaRegenChimeraViperProc + manaRegenInvigoration
                 + manaRegenHuntingParty + manaRegenTargetDebuffs + manaRegenFromPots > 0f ? 
                    (manaRegenGearBuffs != 0 ? manaRegenGearBuffs.ToString("F2") + " from Gear and Buffs\n" : "") +
                    (manaRegenConstantViper != 0 ? manaRegenConstantViper.ToString("F2") + " from Constant Aspect of the Viper\n" : "") +
                    (manaRegenViper != 0 ? manaRegenViper.ToString("F2") + " from Aspect of the Viper\n" : "") +
                    (manaRegenRoarOfRecovery != 0 ? manaRegenRoarOfRecovery.ToString("F2") + " from Roar of Recovery\n" : "") +
                    (manaRegenRapidRecuperation != 0 ? manaRegenRapidRecuperation.ToString("F2") + " from Rapid Recuperation\n" : "") +
                    (manaRegenChimeraViperProc != 0 ? manaRegenChimeraViperProc.ToString("F2") + " from Chimera Viper String Proc\n" : "") +
                    (manaRegenInvigoration != 0 ? manaRegenInvigoration.ToString("F2") + " from Invigoration\n" : "") +
                    (manaRegenHuntingParty != 0 ? manaRegenHuntingParty.ToString("F2") + " from Hunting Party\n" : "") +
                    (manaRegenTargetDebuffs != 0 ? manaRegenTargetDebuffs.ToString("F2") + " from Target Debuffs\n" : "") +
                    (manaRegenFromPots != 0 ? manaRegenFromPots.ToString("F2") + " from Pots" : "")
                : "Nothing to add")
            );
            dictValues.Add("Normal Change", manaChangeDuringNormal.ToString("F2"));
            dictValues.Add("Change during Viper", manaChangeDuringViper.ToString("F2"));
            dictValues.Add("Time to OOM", manaTimeToOOM.ToString("F2"));
            dictValues.Add("Time to Full", manaTimeToFull.ToString("F2"));
            dictValues.Add("Viper Damage Penalty", aspectViperPenalty.ToString("P2"));
            dictValues.Add("Viper Uptime", aspectUptimeViper.ToString("P2"));
            dictValues.Add("No Mana Damage Penalty", NoManaDPSDownTimePerc.ToString("P2"));

            // Hunter DPS
            dictValues.Add("Autoshot DPS", AutoshotDPS.ToString("F2"));
            dictValues.Add("Priority Rotation DPS", CustomDPS.ToString("F2"));
            dictValues.Add("Wild Quiver DPS", WildQuiverDPS.ToString("F2"));
            dictValues.Add("Kill Shot low HP gain", killShotSub20FinalGain.ToString("F2")+"*"+
                            "Kill Shot freq: "+killShot.Freq.ToString("F2")+" -> "+killShot.start_freq.ToString("F2")+"\n"+
                            "Steady Shot freq: "+steadyShot.Freq.ToString("F2")+" -> "+killShotSub20NewSteadyFreq.ToString("F2")+"\n"+
                            "Kill Shot DPS: "+killShot.DPS.ToString("F2")+" -> "+killShotSub20NewDPS.ToString("F2")+"\n"+
                            "Steady Shot DPS: "+steadyShot.DPS.ToString("F2")+" -> "+killShotSub20NewSteadyDPS.ToString("F2")+"\n"+
                            "DPS Gain when switched: " + killShotSub20Gain.ToString("F2")+"\n"+
                            "Time spent sub-20%: " + killShotSub20TimeSpent.ToString("P2"));
            dictValues.Add("Aspect Loss", aspectBeastLostDPS.ToString("F2") + "*" +
                            "Hawk Uptime: " + aspectUptimeHawk.ToString("P2") + "\n" + 
                            "Viper Uptime: " + aspectUptimeViper.ToString("P2") + "\n" + 
                            "Beast Uptime: " + aspectUptimeBeast.ToString("P2"));
            dictValues.Add("Piercing Shots DPS", PiercingShotsDPS.ToString("F2") + "*" +
                            "Steady Shot: " + PiercingShotsDPSSteadyShot.ToString("F2") + "\n" +
                            "Aimed Shot: " + PiercingShotsDPSAimedShot.ToString("F2") + "\n" +
                            "Chimera Shot: " + PiercingShotsDPSChimeraShot.ToString("F2") + "\n");
            dictValues.Add("Special DMG Procs DPS", SpecProcDPS.ToString("F2"));

            // Combined DPS
            string zod = (BonusAttackProcsDPS != 0 ? string.Format("*Includes:\r\nZod's Proc: {0:0.0}", BonusAttackProcsDPS) : "");
            dictValues.Add("Hunter DPS", HunterDpsPoints.ToString("F2") + zod);
            dictValues.Add("Pet DPS", PetDpsPoints.ToString("F2"));
            dictValues.Add("Total DPS", OverallPoints.ToString("F2"));

            return dictValues;
        }

        public override float GetOptimizableCalculationValue(string calculation)
        {
            switch (calculation)
            {
                case "Health": return BasicStats.Health;
                case "Mana": return BasicStats.Mana;
                case "Agility": return BasicStats.Agility;
                case "Crit %": return BasicStats.PhysicalCrit * 100f;
                case "Haste %": return BasicStats.PhysicalHaste * 100f;
                case "Attack Power": return BasicStats.AttackPower;
                case "Armor Penetration %": return BasicStats.ArmorPenetration * 100f;
#if RAWR3 || SILVERLIGHT
                case "% Chance to Miss (Yellow)": return (StatConversion.WHITE_MISS_CHANCE_CAP[BossOpts.Level - character.Level] - BasicStats.PhysicalHit) * 100f;
#else
                case "% Chance to Miss (Yellow)": return (StatConversion.WHITE_MISS_CHANCE_CAP[CalcOpts.TargetLevel - character.Level] - BasicStats.PhysicalHit) * 100f;
#endif
            }
            return 0;
        }
    }
}