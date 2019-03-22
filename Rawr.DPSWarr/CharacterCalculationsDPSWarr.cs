using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.DPSWarr {
    public class CharacterCalculationsDPSWarr : CharacterCalculationsBase {
        #region Variables
        public Stats AverageStats { get; set; }
        public Stats MaximumStats { get; set; }
        public Stats UnbuffedStats { get; set; }
        public Stats BuffedStats { get; set; }
        public Stats BuffsStats { get; set; } // The actual stats that come from Buffs
        public CombatFactors combatFactors { get; set; }
        public Rotation Rot { get; set; }
        #endregion

        #region Points
        private float _overallPoints = 0f;
        public override float OverallPoints { get { return _overallPoints; } set { _overallPoints = value; } }
        private float[] _subPoints = new float[] { 0f, 0f };
        public override float[] SubPoints { get { return _subPoints; } set { _subPoints = value; } }
        public float TotalDPS { get { return _subPoints[0]; } set { _subPoints[0] = value; } }
        public float TotalHPS;
        public float Survivability { get { return _subPoints[1]; } set { _subPoints[1] = value; } }
        #endregion

        #region Display Values
        public int TargetLevel { get; set; }
        public float Duration { get; set; }
        public string floorstring { get; set; }
#if (!RAWR3 && DEBUG)
        public long calculationTime { get; set; }
#endif
        #region Attack Table
        public float Miss { get; set; }
        public float HitRating { get; set; }
        public float ExpertiseRating { get; set; }
        public float Expertise { get; set; }
        public float MhExpertise { get; set; }
        public float OhExpertise { get; set; }
        public float WeapMastPerc { get; set; }
        public float AgilityCritBonus { get; set; }
        public float CritRating { get; set; }
        public float CritPercent { get; set; }
        public float MhCrit { get; set; }
        public float OhCrit { get; set; }
        #endregion
        #region Offensive
        public float BonusCritPercPoleAxeSpec { get; set; }
        public float ArmorPenetrationMaceSpec { get; set; }
        public float ArmorPenetrationStance { get; set; }
        public float ArmorPenetrationRating { get; set; }
        public float ArmorPenetrationRating2Perc { get; set; }
        public float ArmorPenetration { get; set; }
        public float MaxArmorPenetration { get; set; }
        public float buffedArmorPenetration { get; set; }
        public float HasteRating { get; set; }
        public float HastePercent { get; set; }
        public float WeaponSpeed { get; set; }
        public float TeethBonus { get; set; }
        #endregion
        #region DPS
        // White
        public Skills.WhiteAttacks Whites { get; set; }
        public float WhiteDPS { get; set; }
        public float WhiteDmg { get; set; }
        public float WhiteDPSMH { get; set; }
        public float WhiteDPSOH { get; set; }
        public float SpecProcDPS { get; set; }
        public float TotalDamagePerSecond { get; set; }
        #endregion
        #region Abilities
        // Markov Work
        public Skills.FakeWhite FW { get; set; }
        #endregion
        #region Neutral
        public float BaseHealth { get; set; }
        public float WhiteRage { get; set; }
        public float OtherRage { get; set; }
        public float NeedyRage { get; set; }
        public float FreeRage { get; set; }
        public float Stamina { get; set; }
        public float Health { get; set; }
        #endregion
        #region Defensive
        public float Armor { get; set; }
        public float CritReduction { get; set; }
        public float ArmorReduction { get; set; }
        public float GuaranteedReduction { get; set; }
        public float damageReduc { get; set; }
        public float MissedAttacks { get; set; }
        public float AvoidedAttacks { get; set; }
        public float DodgedAttacks { get; set; }
        public float ParriedAttacks { get; set; }
        public float BlockedAttacks { get; set; }
        #endregion
        #endregion

        private string GenFormattedString(string[] passiveContrs) { return GenFormattedString(passiveContrs, false, false, false); }
        private string GenFormattedString(string[] passiveContrs, bool hasCap, bool unprocisOverCap, bool procisOverCap)
        {
            int formIter = 2;
            string theFormat = "";

            theFormat += "{0:00.00%} : {1}*"; // Averaged % and Averaged Rating
            theFormat += "The Pane shows Averaged Values";
            theFormat += "\r\n";
            if (passiveContrs.Length > 0) {
                theFormat += "\r\n= Your Passive Contributions =";
                foreach (string passiveContr in passiveContrs) {
                    theFormat += "\r\n{" + formIter.ToString() + ":00.00%} : " + passiveContr;
                    formIter++;
                }
                theFormat += "\r\n";
            }
            theFormat += "\r\n= UnProc'd =";
            theFormat += "\r\n{" + formIter.ToString() + ":0.#} : Rating"; formIter++;
            theFormat += "\r\n{" + formIter.ToString() + ":00.00%} : Percent"; formIter++;
            if (hasCap) { theFormat += "\r\n" + (unprocisOverCap ? "{" + formIter.ToString() + ":0.#} Rating Over Cap"
                                                                 : "{" + formIter.ToString() + ":0.#} Rating Under Cap"); formIter++; }
            theFormat += "\r\n";
            theFormat += "\r\n= Proc'd =";
            theFormat += "\r\n{" + formIter.ToString() + ":0.#} : Rating"; formIter++;
            theFormat += "\r\n{" + formIter.ToString() + ":00.00%} : Percent"; formIter++;
            if (hasCap) { theFormat += "\r\n" + (procisOverCap ? "{" + formIter.ToString() + ":0.#} Rating Over Cap"
                                                               : "{" + formIter.ToString() + ":0.#} Rating Under Cap"); formIter++; }

            return theFormat;
        }
        private string GenFormattedString(string[] passiveContrs, bool hasCap,
            bool whunprocisOverCap, bool whprocisOverCap,
            bool ywunprocisOverCap, bool ywprocisOverCap)
        {
            int formIter = 2;
            string theFormat = "";

            theFormat += "{0:00.00%} : {1}*"; // Averaged % and Averaged Rating
            theFormat += "The Pane shows Averaged Values";
            theFormat += "\r\n";
            if (passiveContrs.Length > 0) {
                theFormat += "\r\n= Your Passive Contributions =";
                foreach (string passiveContr in passiveContrs) {
                    theFormat += "\r\n{" + formIter.ToString() + ":00.00%} : " + passiveContr;
                    formIter++;
                }
                theFormat += "\r\n";
            }
            theFormat += "\r\n= UnProc'd =";
            theFormat += "\r\n{" + formIter.ToString() + ":0.#} : Rating"; formIter++;
            theFormat += "\r\n{" + formIter.ToString() + ":00.00%} : Percent"; formIter++;
            if (hasCap) {
                theFormat += "\r\n{" + formIter.ToString() + ":0.#} Rating "
                                     + (whunprocisOverCap ? "Over White Cap" : "Under White Cap");
                formIter++;
            }
            theFormat += "\r\n{" + formIter.ToString() + ":00.00%} : Percent"; formIter++;
            if (hasCap) {
                theFormat += "\r\n{" + formIter.ToString() + ":0.#} Rating "
                                     + (ywunprocisOverCap ? "Over Yellow Cap" : "Under Yellow Cap");
                formIter++;
            }
            theFormat += "\r\n";
            theFormat += "\r\n= Proc'd =";
            theFormat += "\r\n{" + formIter.ToString() + ":0.#} : Rating"; formIter++;
            theFormat += "\r\n{" + formIter.ToString() + ":00.00%} : Percent"; formIter++;
            if (hasCap) {
                theFormat += "\r\n{" + formIter.ToString() + ":0.#} Rating "
                + (whprocisOverCap ? "Over White Cap" : "Under White Cap");
                formIter++;
            }
            theFormat += "\r\n{" + formIter.ToString() + ":00.00%} : Percent"; formIter++;
            if (hasCap) {
                theFormat += "\r\n{" + formIter.ToString() + ":0.#} Rating "
                    + (ywprocisOverCap ? "Over Yellow Cap" : "Under Yellow Cap");
                formIter++;
            }

            return theFormat;
        }

        public override Dictionary<string, string> GetCharacterDisplayCalculationValues() {
            Dictionary<string, string> dictValues = new Dictionary<string, string>();
            string format = "";
            int LevelDif = combatFactors.levelDif;

            // Base Stats
            dictValues.Add("Health and Stamina", string.Format("{0:##,##0} : {1:##,##0}*" +
                                "{2:00,000} : Base Health" +
                                "\r\n{3:00,000} : Stam Bonus",
                                AverageStats.Health, AverageStats.Stamina,
                                BaseHealth,
                                StatConversion.GetHealthFromStamina(AverageStats.Stamina)));
            dictValues.Add("Armor", string.Format("{0}*Increases Attack Power by {1}", Armor, TeethBonus));
            #region Strength
            {
                int formIter = 1;
                string theFormat = "";

                float[] passiveContrsVals = new float[] {
                    combatFactors.Char.WarriorTalents.StrengthOfArms * 0.02f,
                    (combatFactors.FuryStance ? combatFactors.Char.WarriorTalents.ImprovedBerserkerStance * 0.04f : 0f),
                    BuffsStats.Strength,
                    BuffsStats.BonusStrengthMultiplier,
                };

                string[] passiveContrs = new string[] {
                    "Strength of Arms",
                    "Improved Berserker Stance",
                    "Buffs : Simple",
                    "Buffs : Multi",
                };

                theFormat += "{0:0.#}*"; // Averaged % and Averaged Rating
                theFormat += "The Pane shows Averaged Values";
                theFormat += "\r\n";
                theFormat += "\r\n= Your Passive Contributions =";
                theFormat += "\r\n{" + formIter.ToString() + ":00.#%} : " + passiveContrs[0]; formIter++;
                theFormat += "\r\n{" + formIter.ToString() + ":00.#%} : " + passiveContrs[1]; formIter++;
                theFormat += "\r\n{" + formIter.ToString() + ":0.#} : "   + passiveContrs[2]; formIter++;
                theFormat += "\r\n{" + formIter.ToString() + ":00.#%} : " + passiveContrs[3]; formIter++;
                theFormat += "\r\n";
                theFormat += "\r\n= UnProc'd =";
                theFormat += "\r\nValue: {" + formIter.ToString() + ":0.#}"; formIter++;
                theFormat += "\r\nIncreases Attack Power by {" + formIter.ToString() + ":0.#}"; formIter++;
                theFormat += "\r\n";
                theFormat += "\r\n= Proc'd =";
                theFormat += "\r\nValue: {" + formIter.ToString() + ":0.#}"; formIter++;
                theFormat += "\r\nIncreases Attack Power by {" + formIter.ToString() + ":0.#}"; formIter++;

                dictValues.Add("Strength", string.Format(theFormat,
                    // Averaged Stats
                    AverageStats.Strength,
                    // Passive Contributions
                    passiveContrsVals[0], passiveContrsVals[1], passiveContrsVals[2], passiveContrsVals[3],
                    // UnProc'd Stats
                    BuffedStats.Strength,
                    BuffedStats.Strength * 2f,
                    // Proc'd Stats
                    MaximumStats.Strength,
                    MaximumStats.Strength * 2f
                    ));
            }
            #endregion
            dictValues.Add("Attack Power", string.Format("{0}*"+
                                "Increases White DPS by {1:0.0}\r\n"+
                                "\r\n"+
                                "Buffed: {2:0}\r\n"+
                                "Proc'd: {3:0}\r\n", (int)AverageStats.AttackPower, AverageStats.AttackPower / 14f,
                                BuffedStats.AttackPower, MaximumStats.AttackPower));
            #region Agility
            {
                int formIter = 1;
                string theFormat = "";

                float[] passiveContrsVals = new float[] {
                    BuffsStats.Agility,
                    BuffsStats.BonusAgilityMultiplier,
                };

                string[] passiveContrs = new string[] { "Buffs : Simple", "Buffs : Multi" };

                theFormat += "{0:0.#}*"; // Averaged % and Averaged Rating
                theFormat += "The Pane shows Averaged Values";
                theFormat += "\r\n";
                theFormat += "\r\n= Your Passive Contributions =";
                theFormat += "\r\n{" + formIter.ToString() + ":0.#} : " + passiveContrs[0]; formIter++;
                theFormat += "\r\n{" + formIter.ToString() + ":00.#%} : " + passiveContrs[1]; formIter++;
                theFormat += "\r\n";
                theFormat += "\r\n= UnProc'd =";
                theFormat += "\r\nIncreases Crit by {" + formIter.ToString() + ":0.#%}"; formIter++;
                theFormat += "\r\nIncreases Armor by {" + formIter.ToString() + ":0.#}"; formIter++;
                theFormat += "\r\n";
                theFormat += "\r\n= Proc'd =";
                theFormat += "\r\nIncreases Crit by {" + formIter.ToString() + ":0.#%}"; formIter++;
                theFormat += "\r\nIncreases Armor by {" + formIter.ToString() + ":0.#}"; formIter++;

                dictValues.Add("Agility", string.Format(theFormat,
                    // Averaged Stats
                    AverageStats.Agility,
                    // Passive Contributions
                    passiveContrsVals[0], passiveContrsVals[1],
                    // UnProc'd Stats
                    StatConversion.GetCritFromAgility(BuffedStats.Agility, CharacterClass.Warrior),
                    StatConversion.GetArmorFromAgility(BuffedStats.Agility),
                    // Proc'd Stats
                    StatConversion.GetCritFromAgility(MaximumStats.Agility, CharacterClass.Warrior),
                    StatConversion.GetArmorFromAgility(MaximumStats.Agility)
                    ));
            }
            #endregion
            #region Crit
            {
                // sub to add neg number as pos, for overcapping to compensate
                // for boss level on yellows (or whites, I dont remember which)
                // Whites clip crit cap with glances, dodges, parries, misses
                float WhCritCap = 1f;// +StatConversion.NPC_LEVEL_CRIT_MOD[LevelDif];
                //float YwCritCap = 1f + StatConversion.NPC_LEVEL_CRIT_MOD[LevelDif];
                if (combatFactors.useOH)
                {
                    WhCritCap -= (Whites.OHAtkTable.Glance + Whites.OHAtkTable.AnyNotLand);
                //    YwCritCap -= (new AttackTable(combatFactors.Char, BuffedStats, combatFactors, combatFactors.CalcOpts, FW, false, false, false)).AnyNotLand;
                }
                else
                {
                    WhCritCap -= (Whites.MHAtkTable.Glance + Whites.MHAtkTable.AnyNotLand);
                    // Yellows clip crit cap with dodges, parries, misses
                //    YwCritCap -= (new AttackTable(combatFactors.Char, BuffedStats, combatFactors, combatFactors.CalcOpts, FW, true, false, false)).AnyNotLand;
                }
                float useRamp = 0f;
                if (combatFactors.Char.WarriorTalents.Rampage > 0f) useRamp = 0.05f;
                float[] passiveContrsVals = new float[] {
                    0.03192f,
                    AgilityCritBonus,
                    StatConversion.GetCritFromRating(BuffedStats.CritRating + BuffedStats.DeathbringerProc),
                    combatFactors.Char.WarriorTalents.Cruelty * 0.01f,
                    (combatFactors.FuryStance ? 0.03f : 0f),
                    (combatFactors.FuryStance ? AverageStats.BonusWarrior_T9_2P_Crit : 0f),
                    BonusCritPercPoleAxeSpec,
                    BuffsStats.PhysicalCrit + useRamp,
                };
                float passiveContrsTtlVal = passiveContrsVals[0] + passiveContrsVals[1] + passiveContrsVals[2]
                                          + passiveContrsVals[3] + passiveContrsVals[4] + passiveContrsVals[5]
                                          + passiveContrsVals[6] + passiveContrsVals[7];
                string[] passiveContrs = new string[] { "Base Crit", "From Agility", "From Crit Rating", "Cruelty",
                                                        "Berserker Stance", "T9 2P Set Bonus", "Poleaxe Specialization",
                                                        "Buffs" };

                //float WhUnProcdCrit = StatConversion.GetCritFromRating(BuffedStats.CritRating + BuffedStats.DeathbringerProc);
                float WhProcdCrit = StatConversion.GetCritFromRating(MaximumStats.CritRating + MaximumStats.DeathbringerProc - BuffedStats.CritRating - BuffedStats.DeathbringerProc);
                bool isWhUnProcdOverCap = passiveContrsTtlVal > WhCritCap;
                bool isWhProcdOverCap = passiveContrsTtlVal + WhProcdCrit > WhCritCap;
                float amountWhUnProcdOverCap = Math.Abs(StatConversion.GetRatingFromCrit(WhCritCap - passiveContrsTtlVal));
                float amountWhProcdOverCap = Math.Abs(StatConversion.GetRatingFromCrit(WhCritCap - (passiveContrsTtlVal + WhProcdCrit)));

                float YwUnProcdCrit = StatConversion.GetCritFromRating(BuffedStats.CritRating + BuffedStats.DeathbringerProc);
                float YwProcdCrit = StatConversion.GetCritFromRating(MaximumStats.CritRating + MaximumStats.DeathbringerProc);
                //bool isYwUnProcdOverCap = passiveContrsTtlVal + YwUnProcdCrit > YwCritCap;
                //bool isYwProcdOverCap = passiveContrsTtlVal + YwProcdCrit > YwCritCap;
                //float amountYwUnProcdOverCap = Math.Abs(StatConversion.GetRatingFromCrit(YwCritCap - (passiveContrsTtlVal + YwUnProcdCrit)));
                //float amountYwProcdOverCap = Math.Abs(StatConversion.GetRatingFromCrit(YwCritCap - (passiveContrsTtlVal + YwProcdCrit)));

                string theFormat = GenFormattedString(passiveContrs, true,
                    isWhUnProcdOverCap, isWhProcdOverCap
                    //isYwUnProcdOverCap, isYwProcdOverCap
                    );

                dictValues.Add("Crit", string.Format(theFormat,
                    // Averaged Stats
                    CritPercent, AverageStats.CritRating,
                    // Passive Contributions
                    passiveContrsVals[0], passiveContrsVals[1], passiveContrsVals[2],
                    passiveContrsVals[3], passiveContrsVals[4], passiveContrsVals[5],
                    passiveContrsVals[6], passiveContrsVals[7],
                    // UnProc'd Stats
                    BuffedStats.CritRating + BuffedStats.DeathbringerProc,
                    Math.Min(WhCritCap, passiveContrsTtlVal), amountWhUnProcdOverCap,
                    //Math.Min(YwCritCap, passiveContrsTtlVal + YwUnProcdCrit), amountYwUnProcdOverCap,
                    // Proc'd Stats
                    MaximumStats.CritRating + MaximumStats.DeathbringerProc,
                    Math.Min(WhCritCap, passiveContrsTtlVal + WhProcdCrit), amountWhProcdOverCap
                    //Math.Min(YwCritCap, passiveContrsTtlVal + YwProcdCrit), amountYwProcdOverCap
                    ));
            }
            #endregion
            #region Armor Penetration
            {
                float ArPCap = 1.00f;
                float[] passiveContrsVals = new float[] {
                    (!combatFactors.FuryStance ? 0.10f : 0f),
                    (!combatFactors.FuryStance ? AverageStats.BonusWarrior_T9_2P_ArP : 0f),
                    ArmorPenetrationMaceSpec
                };
                float passiveContrsTtlVal = passiveContrsVals[0] + passiveContrsVals[1] + passiveContrsVals[2];
                string[] passiveContrs = new string[] { "Battle Stance", "T9 2P Set Bonus", "Mace Specialization" };
                float UnProcdArP = StatConversion.GetArmorPenetrationFromRating(BuffedStats.ArmorPenetrationRating);
                float ProcdArP = StatConversion.GetArmorPenetrationFromRating(MaximumStats.ArmorPenetrationRating);
                bool isUnProcdOverCap = passiveContrsTtlVal + UnProcdArP > ArPCap;
                bool isProcdOverCap = passiveContrsTtlVal + ProcdArP > ArPCap;
                float amountUnProcdOverCap = Math.Abs(StatConversion.GetRatingFromArmorPenetration(ArPCap - (passiveContrsTtlVal + UnProcdArP)));
                float amountProcdOverCap = Math.Abs(StatConversion.GetRatingFromArmorPenetration(ArPCap - (passiveContrsTtlVal + ProcdArP)));
                string theFormat = GenFormattedString(passiveContrs, true, isUnProcdOverCap, isProcdOverCap);
                dictValues.Add("Armor Penetration", string.Format(theFormat,
                    // Averaged Stats
                    ArmorPenetration, AverageStats.ArmorPenetrationRating,
                    // Passive Contributions
                    passiveContrsVals[0], passiveContrsVals[1], passiveContrsVals[2],
                    // UnProc'd Stats
                    BuffedStats.ArmorPenetrationRating,
                    Math.Min(ArPCap, passiveContrsTtlVal + UnProcdArP),
                    amountUnProcdOverCap,
                    // Proc'd Stats
                    MaximumStats.ArmorPenetrationRating,
                    Math.Min(ArPCap, passiveContrsTtlVal + ProcdArP),
                    amountProcdOverCap
                    ));
            }
            #endregion
            #region Haste
            {
                // Haste has no cap? Shouldn't there be a 100% cap or something?
                // We should also state the before/after effects of haste on white swings
                // Maybe a good point to show how much swing time is lost to Slams too?
                float heroism = 0f;
                if (BuffsStats._rawSpecialEffectData != null)
                {
                    foreach (SpecialEffect effect in BuffsStats._rawSpecialEffectData)
                    {
                        if (effect != null && effect.Stats.PhysicalHaste > 0) {
                            heroism = effect.GetAverageStats().PhysicalHaste;
                        }
                    }
                }
                float[] passiveContrsVals = new float[] {
                    combatFactors.Char.WarriorTalents.BloodFrenzy * 0.05f,
                    BuffsStats.PhysicalHaste,
                    heroism,
                };
                float passiveContrsTtlVal = (1f + passiveContrsVals[0])
                                          * (1f + passiveContrsVals[1])
                                          * (1f + passiveContrsVals[2])
                                          - 1f;
                string[] passiveContrs = new string[] { "Blood Frenzy", "Buffs", "Heroism (Averaged)" };
                float UnProcdHaste = StatConversion.GetHasteFromRating(BuffedStats.HasteRating, CharacterClass.Warrior);
                float ProcdHaste = StatConversion.GetHasteFromRating(MaximumStats.HasteRating, CharacterClass.Warrior);
                string theFormat = GenFormattedString(passiveContrs);

                dictValues.Add("Haste", string.Format(theFormat,
                    // Averaged Stats
                    HastePercent, AverageStats.HasteRating,
                    // Passive Contributions
                    passiveContrsVals[0], passiveContrsVals[1], passiveContrsVals[2],
                    // UnProc'd Stats
                    BuffedStats.HasteRating,
                    (1f + passiveContrsTtlVal) * (1f + UnProcdHaste) - 1f,
                    // Proc'd Stats
                    MaximumStats.HasteRating,
                    (1f + passiveContrsTtlVal) * (1f + ProcdHaste) - 1f
                    ));
            }
            #endregion
            #region Hit
            {
                // old
                float HitPercent = StatConversion.GetHitFromRating(HitRating);
                float HitPercBonus = AverageStats.PhysicalHit;
                // Hit Soft Cap ratings check, how far from it
                float capA1         = StatConversion.WHITE_MISS_CHANCE_CAP[LevelDif];
                float convcapA1     = (float)Math.Ceiling(StatConversion.GetRatingFromHit(capA1));
                float sec2lastNumA1 = (convcapA1 - StatConversion.GetRatingFromHit(HitPercent) - StatConversion.GetRatingFromHit(HitPercBonus)) * -1;
                //float lastNumA1    = StatConversion.GetRatingFromExpertise((convcapA1 - Math.Min(MhExpertise, (OhExpertise != 0 ? OhExpertise : MhExpertise))) * -1);
                // Hit Hard Cap ratings check, how far from it
                float capA2         = StatConversion.WHITE_MISS_CHANCE_CAP_DW[LevelDif];
                float convcapA2     = (float)Math.Ceiling(StatConversion.GetRatingFromHit(capA2));
                float sec2lastNumA2 = (convcapA2 - StatConversion.GetRatingFromHit(HitPercent) - StatConversion.GetRatingFromHit(HitPercBonus)) * -1;
                //float lastNumA2   = StatConversion.GetRatingFromExpertise((sec2lastNumA2 - Math.Min(MhExpertise, (OhExpertise != 0 ? OhExpertise : MhExpertise))) * -1);
                dictValues.Add("Hit",
                    string.Format("{0:00.00%} : {1}*" + "{2:0.00%} : From Other Bonuses" +
                                    "\r\n{3:0.00%} : Total Hit % Bonus" +
                                    "\r\n\r\nWhite Two-Hander Cap: " +
                                    (sec2lastNumA1 > 0 ? "You can free {4:0} Rating"
                                                       : "You need {4:0} more Rating") +
                                    "\r\nWhite Dual Wield Cap: " +
                                    (sec2lastNumA2 > 0 ? "You can free {5:0} Rating"
                                                       : "You need {5:0} more Rating"),
                                    StatConversion.GetHitFromRating(AverageStats.HitRating),
                                    AverageStats.HitRating,
                                    HitPercBonus,
                                    HitPercent + HitPercBonus,
                                    (sec2lastNumA1 > 0 ? sec2lastNumA1 : sec2lastNumA1 * -1),
                                    (sec2lastNumA2 > 0 ? sec2lastNumA2 : sec2lastNumA2 * -1)
                                ));
            }
            #endregion
            #region Expertise
            {
                // Dodge Cap ratings check, how far from it, uses lesser of MH and OH
                // Also factors in Weapon Mastery
                float capB1         = StatConversion.YELLOW_DODGE_CHANCE_CAP[LevelDif] - WeapMastPerc;
                float convcapB1     = (float)Math.Ceiling(StatConversion.GetExpertiseFromDodgeParryReduc(capB1));
                float sec2lastNumB1 = (convcapB1 - Math.Min(MhExpertise, (OhExpertise != 0 ? OhExpertise : MhExpertise))) * -1;
                float lastNumB1     = StatConversion.GetRatingFromExpertise((convcapB1 - WeapMastPerc - Math.Min(MhExpertise, (OhExpertise != 0 ? OhExpertise : MhExpertise))) * -1);
                // Parry Cap ratings check, how far from it, uses lesser of MH and OH
                float capB2         = StatConversion.YELLOW_PARRY_CHANCE_CAP[LevelDif];
                float convcapB2     = (float)Math.Ceiling(StatConversion.GetExpertiseFromDodgeParryReduc(capB2));
                float sec2lastNumB2 = (convcapB2 - Math.Min(MhExpertise, (OhExpertise != 0 ? OhExpertise : MhExpertise))) * -1;
                float lastNumB2     = StatConversion.GetRatingFromExpertise((convcapB2 - Math.Min(MhExpertise, (OhExpertise != 0 ? OhExpertise : MhExpertise))) * -1);
                dictValues.Add("Expertise",
                    string.Format("{0:00.00%} : {1:00.00} : {2}*" +
                    "Following includes Racial bonus and Strength of Arms" +
                    "\r\n{3:00.00%} Weapon Mastery (Dodge Only)" +
                    "\r\n{4:00.00%} : {5:00.00} : MH" +
                    "\r\n{6:00.00%} : {7:00.00} : OH" +
                    "\r\n\r\n" + "Dodge Cap: " +
                    (lastNumB1 > 0 ? "You can free {8:0} Expertise ({9:0} Rating)"
                                 : "You need {8:0} more Expertise ({9:0} Rating)") +
                    "\r\n" + "Parry Cap: " +
                    (lastNumB2 > 0 ? "You can free {10:0} Expertise ({11:0} Rating)"
                                 : "You need {10:0} more Expertise ({11:0} Rating)"),
                    StatConversion.GetDodgeParryReducFromExpertise(Expertise),
                    Expertise,
                    AverageStats.ExpertiseRating,
                    WeapMastPerc,
                    StatConversion.GetDodgeParryReducFromExpertise(MhExpertise), MhExpertise,
                    StatConversion.GetDodgeParryReducFromExpertise(OhExpertise), OhExpertise,
                    (sec2lastNumB1 > 0 ? sec2lastNumB1 : sec2lastNumB1 * -1), (lastNumB1 > 0 ? lastNumB1 : lastNumB1 * -1),
                    (sec2lastNumB2 > 0 ? sec2lastNumB2 : sec2lastNumB2 * -1), (lastNumB2 > 0 ? lastNumB2 : lastNumB2 * -1)
                ));
            }
            #endregion

            dictValues.Add("Description", string.Format("DPS : PerHit : #ActsD"));
            // DPS Abilities
            format = "{0:0000} : {1:00000} : {2:000.00}";
            if (TotalDPS < 0f) { TotalDPS = 0f; }
            foreach (Rawr.DPSWarr.Rotation.AbilWrapper aw in Rot.GetAbilityList()) {
                if (!aw.ability.Name.Equals("Invalid")) {
                    dictValues.Add(aw.ability.Name, string.Format(format, aw.allDPS, aw.ability.DamageOnUse, aw.allNumActivates)
                                                    + aw.ability.GenTooltip(aw.allNumActivates, aw.allDPS / TotalDPS));
                }
            }
            // DPS General
            dictValues.Add("White DPS",             string.Format("{0:0000} : {1:00000}", WhiteDPS, WhiteDmg) + Whites.GenTooltip(WhiteDPSMH, WhiteDPSOH, TotalDPS));
            dictValues.Add("Deep Wounds",           string.Format("{0:0000}*{1:00.0%} of DPS", Rot.DW.TickSize,Rot.DW.TickSize/TotalDPS));
            dictValues.Add("Special DMG Procs",     string.Format("{0:0000}*{1:00.0%} of DPS", SpecProcDPS, SpecProcDPS / TotalDPS));
            dictValues.Add("Total DPS",             string.Format("{0:#,##0} : {1:#,###,##0}*"+Rot.GCDUsage,TotalDPS,TotalDPS*Duration));
            // Rage
            format = "{0:0000}";
            dictValues.Add("Total Generated Rage",      string.Format("{0:00} = {1:0} + {2:0}", WhiteRage + OtherRage, WhiteRage, OtherRage));
            dictValues.Add("Needed Rage for Abilities", string.Format(format,NeedyRage));
            dictValues.Add("Available Free Rage",       string.Format(format,FreeRage ));
#if (!RAWR3 && DEBUG)
            dictValues.Add("Calculation Time", string.Format("{0}", calculationTime));
#endif
            return dictValues;
        }

        public override float GetOptimizableCalculationValue(string calculation) {
            switch (calculation) {
                case "Health": return AverageStats.Health;
                case "Armor": return AverageStats.Armor + AverageStats.BonusArmor;
                case "Strength": return AverageStats.Strength;
                case "Attack Power": return AverageStats.AttackPower;
                case "Agility": return AverageStats.Agility;
                case "Crit %": return combatFactors._c_mhycrit * 100f;
                case "Haste %": return combatFactors.TotalHaste * 100f;
                case "ArP %": return AverageStats.ArmorPenetration * 100f;
                case "% Chance to Miss (White)": return combatFactors._c_wmiss * 100f;
                case "% Chance to Miss (Yellow)": return combatFactors._c_ymiss * 100f;
                case "% Chance to be Dodged": return combatFactors._c_mhdodge * 100f;
                case "% Chance to be Parried": return combatFactors._c_mhparry * 100f;
                case "% Chance to be Avoided (Yellow/Dodge)":
                    return combatFactors._c_ymiss * 100f + combatFactors._c_mhdodge * 100f;
            }
            return 0.0f;
        }
    }
}
