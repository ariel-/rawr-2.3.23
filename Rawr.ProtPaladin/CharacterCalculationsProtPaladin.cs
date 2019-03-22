using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.ProtPaladin
{
    public class CharacterCalculationsProtPaladin : CharacterCalculationsBase
    {
        public Stats BasicStats { get; set; }
        public List<Buff> ActiveBuffs { get; set; }
        public AbilityModelList Abilities { get; set; }

        private float _overallPoints = 0f;
        public override float OverallPoints
        {
            get { return _overallPoints; }
            set { _overallPoints = value; }
        }

        private float[] _subPoints = new float[] { 0f, 0f, 0f };
        public override float[] SubPoints
        {
            get { return _subPoints; }
            set { _subPoints = value; }
        }

        public float SurvivalPoints
        {
            get { return _subPoints[0]; }
            set { _subPoints[0] = value; }
        }

        public float MitigationPoints
        {
            get { return _subPoints[1]; }
            set { _subPoints[1] = value; }
        }

        public float ThreatPoints
        {
            get { return _subPoints[2]; }
            set { _subPoints[2] = value; }
        }

        // Target Info
        public int TargetLevel { get; set; }
        public int TargetArmor { get; set; }
        public float EffectiveTargetArmor { get; set; }
        public float TargetArmorDamageReduction { get; set; }
        public float EffectiveTargetArmorDamageReduction { get; set; }

        // Ranking Modes
        public int RankingMode { get; set; }
        public float ThreatScale { get; set; }

        //Basic Tank Defensive Stats
        public float Defense { get; set; }
        public float DefenseRating { get; set; }
        public float Dodge { get; set; }
        public float Parry { get; set; }
        public float Miss { get; set; }
        public float CritReduction { get; set; }
        public float CritVulnerability { get; set; }
        public float CritVulnerabilityWithoutHolyShield { get; set; }
        public bool UsingHolyShield { get; set; }
        public float ArmorReduction { get; set; }
        public float GuaranteedReduction { get; set; }
        public float DodgePlusMissPlusParry { get; set; }
        public float TotalMitigation { get; set; }
        public float BaseAttackerSpeed { get; set; }
        public float AttackerSpeed { get; set; }
        public float DamageTaken { get; set; }
        public float DPSTaken { get; set; }
        public float DamageTakenPerHit { get; set; }
        public float DamageTakenPerCrit { get; set; }
		
        public float TankPoints { get; set; }
        public float BurstTime { get; set; }

        // Shield Tank Defensive Stats
        public float Block { get; set; }
        public float ActiveBlockValue { get; set; }
        public float StaticBlockValue { get; set; }
        public float DodgePlusMissPlusParryPlusBlock { get; set; }
        public float DamageTakenPerBlock { get; set; }

        // Magic Defensive Stats
        public float[] ResistanceTable { get; set; }
        public float NatureReduction { get; set; }
        public float FrostReduction { get; set; }
        public float FireReduction { get; set; }
        public float ShadowReduction { get; set; }
        public float ArcaneReduction { get; set; }
        public float NatureSurvivalPoints { get; set; }
        public float FrostSurvivalPoints { get; set; }
        public float FireSurvivalPoints { get; set; }
        public float ShadowSurvivalPoints { get; set; }
        public float ArcaneSurvivalPoints { get; set; }

        // Basic Offensive Stats
        public float MissedAttacks { get; set; }
        public float AvoidedAttacks { get; set; }
        public float DodgedAttacks { get; set; }
        public float ParriedAttacks { get; set; }
        public float GlancingAttacks { get; set; }
        public float GlancingReduction { get; set; }
        public float BlockedAttacks { get; set; }
        public float Hit { get; set; }
        public float SpellHit { get; set; }
        public float Crit { get; set; }
        public float SpellCrit { get; set; }
        public float Expertise { get; set; }
        public float PhysicalHaste { get; set; }
        public float SpellHaste { get; set; }
        public float ArmorPenetration { get; set; }
        public float ArmorPenetrationFromRating { get; set; }
        public float EffectiveArmorPenetration { get; set; }
        public float EffectiveArmorPenetrationRating { get; set; }
        public float ArmorPenetrationCap { get; set; }
        public float WeaponSpeed { get; set; }
        public float TotalDamagePerSecond { get; set; }

        public float ThreatPerSecond { get; set; }
        //public float UnlimitedThreat { get; set; }		
        public string ThreatModel { get; set; }

        public override Dictionary<string, string> GetCharacterDisplayCalculationValues()
        {
            Dictionary<string, string> dictValues = new Dictionary<string, string>();

            dictValues.Add("Health", BasicStats.Health.ToString());
            dictValues.Add("Mana", BasicStats.Mana.ToString());
            dictValues.Add("Strength", BasicStats.Strength.ToString());
            dictValues.Add("Agility", BasicStats.Agility.ToString());
            dictValues.Add("Stamina", string.Format("{0}*Increases Health by {1}", BasicStats.Stamina, (BasicStats.Stamina - 20f) * 10f + 20f));
            dictValues.Add("Intellect", BasicStats.Intellect.ToString());
            dictValues.Add("Armor", string.Format("{0}*Reduces physical damage taken by {1:0.00%}" + Environment.NewLine +
                                                  "Armor Damage Reduction depends on Attacker Level.",
                                                  BasicStats.Armor, ArmorReduction));
            dictValues.Add("Defense", Defense.ToString() + string.Format("*Defense Rating {0}", BasicStats.DefenseRating));
            dictValues.Add("Dodge", string.Format("{0:0.0000%}*Dodge Rating {1}", Dodge, BasicStats.DodgeRating));
            dictValues.Add("Parry", string.Format("{0:0.0000%}*Parry Rating {1}", Parry, BasicStats.ParryRating));
            dictValues.Add("Block", string.Format("{0:0.0000%}*Block Rating {1}", Block, BasicStats.BlockRating));
            dictValues.Add("Miss", string.Format("{0:0.0000%}", Miss));
            dictValues.Add("Block Value", string.Format("{0}*{1} Active Block Value (Libram etc)", StaticBlockValue, ActiveBlockValue));
            dictValues.Add("Guaranteed Reduction", string.Format("{0:0.00%}", GuaranteedReduction));
            dictValues.Add("Avoidance", string.Format("{0:0.0000%}*Avoidance Points {1}", DodgePlusMissPlusParry, (DodgePlusMissPlusParry * 10000f)));
            dictValues.Add("Avoidance + Block", string.Format("{0:0.0000%}", DodgePlusMissPlusParryPlusBlock));
            dictValues.Add("Total Mitigation", string.Format("{0:0.00%}", TotalMitigation));

            if (AttackerSpeed == BaseAttackerSpeed)
                dictValues.Add("Attacker Speed", string.Format("{0:0.00}s", AttackerSpeed));
            else
                dictValues.Add("Attacker Speed", string.Format("{0:0.00}s*Base speed of {1:0.00}s (reduced by parry haste)", AttackerSpeed, BaseAttackerSpeed));

            dictValues.Add("Damage Taken",
                string.Format("{0:0.0} DPS*{1:0} damage per normal attack" + Environment.NewLine +
                                "{2:0} damage per blocked attack" + Environment.NewLine +
                                "{3:0} damage per critical attack" + Environment.NewLine +
                                "On Average, you take" + Environment.NewLine +
                                "{4:0.00%} of incoming damage", DPSTaken, DamageTakenPerHit, DamageTakenPerBlock, DamageTakenPerCrit, DamageTaken));

            dictValues.Add("Resilience",
                string.Format(@"{0}*Reduces periodic damage and chance to be critically hit by {1:0.00%}." + Environment.NewLine +
                                "Reduces the effect of mana-drains and the damage of critical strikes by {2:0.00%}.",
                                BasicStats.Resilience,
                                StatConversion.GetCritReductionFromResilience(BasicStats.Resilience,CharacterClass.Paladin),
                                StatConversion.GetCritReductionFromResilience(BasicStats.Resilience,CharacterClass.Paladin)*2f));

            if (CritVulnerability > 0.0001f)
            {
                double defenseNeeded = Math.Ceiling((100 * CritVulnerability / StatConversion.DEFENSE_RATING_AVOIDANCE_MULTIPLIER) * StatConversion.RATING_PER_DEFENSE);
                double resilienceNeeded = Math.Ceiling(CritVulnerability * StatConversion.RATING_PER_RESILIENCE);
                dictValues.Add("Chance to be Crit",
                    string.Format("{0:0.00%}*CRITTABLE! Short by approximately {1:0} defense rating or approximately {2:0} resilience rating to be uncrittable.",
                                    CritVulnerability, defenseNeeded, resilienceNeeded));
            }
            else
                dictValues.Add("Chance to be Crit", string.Format("{0:0.00%}*Chance to be crit reduced by {1:0.00%}", CritVulnerability, CritReduction));

            if (UsingHolyShield)
            {
                if (CritVulnerabilityWithoutHolyShield > 0.0001f) {
                    double defenseNeeded = Math.Ceiling((100 * CritVulnerabilityWithoutHolyShield / StatConversion.DEFENSE_RATING_AVOIDANCE_MULTIPLIER) * StatConversion.RATING_PER_DEFENSE);
                    double resilienceNeeded = Math.Ceiling(CritVulnerability * StatConversion.RATING_PER_RESILIENCE);
                    dictValues.Add("...Without Holy Shield",
                        string.Format("{0:0.00%}*CRITTABLE! Short by approximately {1:0} defense rating or approximately {2:0} resilience rating to be uncrittable without holy shield.",
                                        CritVulnerabilityWithoutHolyShield, defenseNeeded, resilienceNeeded));
                }
                else
                    dictValues.Add("...Without Holy Shield", string.Format("{0:0.00%}*Chance to be crit reduced by {1:0.00%}", CritVulnerabilityWithoutHolyShield, CritReduction));
            }
            else
            {
                dictValues.Add("...Without Holy Shield", "N/A*Not applicable");
            }

            dictValues.Add("Nature Resist", string.Format("{0:0}*{1:0.00%} Total Reduction", BasicStats.NatureResistance, NatureReduction));
            dictValues.Add("Arcane Resist", string.Format("{0:0}*{1:0.00%} Total Reduction", BasicStats.ArcaneResistance, ArcaneReduction));
            dictValues.Add("Frost Resist", string.Format("{0:0}*{1:0.00%} Total Reduction", BasicStats.FrostResistance, FrostReduction));
            dictValues.Add("Fire Resist", string.Format("{0:0}*{1:0.00%} Total Reduction", BasicStats.FireResistance, FireReduction));
            dictValues.Add("Shadow Resist", string.Format("{0:0}*{1:0.00%} Total Reduction", BasicStats.ShadowResistance, ShadowReduction));
            dictValues["Nature Survival"] = NatureSurvivalPoints.ToString();
            dictValues["Frost Survival"] = FrostSurvivalPoints.ToString();
            dictValues["Fire Survival"] = FireSurvivalPoints.ToString();
            dictValues["Shadow Survival"] = ShadowSurvivalPoints.ToString();
            dictValues["Arcane Survival"] = ArcaneSurvivalPoints.ToString();

            dictValues.Add("Weapon Speed", string.Format("{0:0.00}*{1:0.00%} Haste", WeaponSpeed, PhysicalHaste));
            dictValues.Add("Attack Power", string.Format("{0}", BasicStats.AttackPower));
            dictValues.Add("Spell Power", string.Format("{0}", BasicStats.SpellPower));
            dictValues.Add("Hit", string.Format("{0:0.00%}*Hit Rating {1}" + Environment.NewLine + "Against a Target of Level {2}", Hit, BasicStats.HitRating, TargetLevel));
            dictValues.Add("Spell Hit", string.Format("{0:0.00%}*Hit Rating {1}" + Environment.NewLine + "Against a Target of Level {2}",
                                                      SpellHit, BasicStats.HitRating, TargetLevel));
            dictValues.Add("Expertise",
                string.Format("{0:0.00}*Expertise Rating {1}" + Environment.NewLine + "Reduces chance to be dodged or parried by {2:0.00%}.",
                                         StatConversion.GetExpertiseFromRating(BasicStats.ExpertiseRating,CharacterClass.Paladin) + BasicStats.Expertise,
                                         BasicStats.ExpertiseRating, Expertise));
            dictValues.Add("Physical Haste", string.Format("{0:0.00%}*Haste Rating {1:0.00}", PhysicalHaste, BasicStats.HasteRating));
            dictValues.Add("Effective Target Armor", 
                string.Format("{0}*Reduces the physical damage you deal by {1:0.00%}" + Environment.NewLine + Environment.NewLine +
                              "Effective Armor depends on Armor ignore debuffs," + Environment.NewLine +
                              "your Armor Penetration buffs and talents." + Environment.NewLine +
                              "Enemy Damage Reduction depends on your Level.",
                              (float)Math.Floor(EffectiveTargetArmor), EffectiveTargetArmorDamageReduction));
            
            dictValues.Add("Effective Armor Penetration",
                string.Format("{0:0.00%}*Armor Penetration Rating {1} = {2:0.00%}." + Environment.NewLine + "" + Environment.NewLine +
                              "Your Armor Penetration Cap is {3} Armor, this is" + Environment.NewLine + 
                              "the highest amount your Arp Rating will ignore." + Environment.NewLine + 
                              "Your Rating currently reduces Enemy Armor by {4}." + Environment.NewLine + "" + Environment.NewLine + 
                              "Your Arp Rating is {5:0.00%} effective (GC)."+ Environment.NewLine +
                              "Note that debuffs like Sunder reduce the Cap.",
                              EffectiveArmorPenetration, BasicStats.ArmorPenetrationRating, ArmorPenetrationFromRating, 
                              (float)Math.Floor(ArmorPenetrationCap), 
                              (float)Math.Floor(ArmorPenetrationCap * ArmorPenetrationFromRating), 
                              EffectiveArmorPenetrationRating));
            
            dictValues.Add("Crit", string.Format("{0:0.00%}*Crit Rating {1}" + Environment.NewLine + "Against a Target of Level {2}",
                                                 Crit, BasicStats.CritRating, TargetLevel));
            dictValues.Add("Spell Crit", string.Format("{0:0.00%}*Crit Rating {1}" + Environment.NewLine + "Against a Target of Level {2}",
                                                       SpellCrit, BasicStats.CritRating, TargetLevel));
            dictValues.Add("Weapon Damage", string.Format("{0:0.00}*As average damage per {1}" + Environment.NewLine + "Before Armor", 
                                                          BasicStats.WeaponDamage, Lookup.Name(Ability.None)));
            dictValues.Add("Missed Attacks",
                string.Format("{0:0.00%}*Attacks Missed: {1:0.00%}" + Environment.NewLine + "Attacks Dodged: {2:0.00%}" + Environment.NewLine +
                                "Attacks Parried: {3:0.00%}", AvoidedAttacks, MissedAttacks, DodgedAttacks, ParriedAttacks));
            dictValues.Add("Glancing Attacks", string.Format("{0:0.00%}*{1:0.00%} Reduction" + Environment.NewLine + 
                                "Against a Target of Level {2}", GlancingAttacks, 1.0f - GlancingReduction, TargetLevel));
            dictValues.Add("Total Damage/sec", string.Format("{0:0.0}", TotalDamagePerSecond) + "*" + ThreatModel);
            dictValues.Add("Threat/sec", string.Format("{0:0.0}", ThreatPerSecond) + "*" + ThreatModel);
            //dictValues.Add("Unlimited Threat/sec", string.Format("{0:0.0}", UnlimitedThreat) + "*" + ThreatModel);

            switch (RankingMode)
            {
                case 2:
                    dictValues.Add("Ranking Mode", "TankPoints*The average amount of unmitigated damage which can be taken before dying");
                    dictValues.Add("Survival Points", string.Format("{0:0}*Effective Health", SurvivalPoints));
                    break;
                case 3:
                    dictValues.Add("Ranking Mode", "Burst Time*The average amount of time between events which have a chance to result in a burst death");
                    dictValues.Add("Survival Points", string.Format("{0:0}*{1:0.00} seconds between events", SurvivalPoints, SurvivalPoints / 100.0f));
                    break;
                case 4:
                    dictValues.Add("Ranking Mode", "Damage Output*The average amount of DPS which can be produced");
                    dictValues.Add("Survival Points", string.Format("{0:0}*Survival is not weighted in this mode", SurvivalPoints, SurvivalPoints / 100.0f));
                    break;
                case 5:
                    dictValues.Add("Ranking Mode", "ProtWarr Mode*Average mitigated damage vs Effective Health");
                    dictValues.Add("Survival Points", string.Format("{0:0}*Effective Health", SurvivalPoints));
                    break;
                case 6:
                    dictValues.Add("Ranking Mode", "Damage Taken*Average taken damage vs. Effective Health");
                    dictValues.Add("Survival Points", string.Format("{0:0}*Effective Health", SurvivalPoints));
                    break;
                case 7:
                    dictValues.Add("Ranking Mode", "Ranking Mode 7*Dummy Placeholder");
                    dictValues.Add("Survival Points", string.Format("{0:0}*Survival Placeholder", SurvivalPoints));
                    break;
                case 8:
                    dictValues.Add("Ranking Mode", "Ranking Mode 8*Dummy Placeholder");
                    dictValues.Add("Survival Points", string.Format("{0:0}*Survival Placeholder", SurvivalPoints));
                    break;
                default:
                    dictValues.Add("Ranking Mode", "Mitigation Scale*Customizable scale which allows you to weight mitigation vs. effective health.");
                    dictValues.Add("Survival Points", string.Format("{0:0}*Effective Health", SurvivalPoints));
                    break;
            }
            dictValues.Add("Overall Points", string.Format("{0:0}", OverallPoints));
            dictValues.Add("Mitigation Points", string.Format("{0:0}", MitigationPoints));
            dictValues.Add("Threat Points", string.Format("{0:0}", ThreatPoints));

            return dictValues;
        }

        public override float GetOptimizableCalculationValue(string calculation)
        {
            switch (calculation)
            {
                case "Health": return BasicStats.Health;
                case "% Guaranteed Reduction": return GuaranteedReduction * 100.0f;
                case "% Total Mitigation": return TotalMitigation * 100.0f;
                case "Avoidance Points": return DodgePlusMissPlusParry * 10000.0f;
                case "% Avoid + Block Attacks": return DodgePlusMissPlusParryPlusBlock * 100.0f;
                case "% Chance to be Crit": return ((float)Math.Round(CritVulnerability * 100.0f, 2));
                case "% Chance to be Crit Without Holy Shield": return ((float)Math.Round(CritVulnerabilityWithoutHolyShield * 100.0f, 2));
                case "Block Value": return StaticBlockValue;
                case "% Block Chance": return Block * 100.0f;
                case "% Chance to be Avoided": return AvoidedAttacks * 100.0f;
                case "% Chance to be Missed": return MissedAttacks * 100.0f;
                case "% Chance to be Dodged": return DodgedAttacks * 100.0f;
                case "% Chance to be Parried": return ParriedAttacks * 100.0f;
                // case "% Hit Chance": return Hit * 100.0f;
                case "% Spell Hit Chance": return SpellHit * 100.0f;
                case "Burst Time": return BurstTime;
                case "TankPoints": return TankPoints;
                case "Nature Survival": return NatureSurvivalPoints;
                case "Fire Survival": return FireSurvivalPoints;
                case "Frost Survival": return FrostSurvivalPoints;
                case "Shadow Survival": return ShadowSurvivalPoints;
                case "Arcane Survival": return ArcaneSurvivalPoints;
                case "Threat Per Second": return ThreatPerSecond;
            }
            return 0.0f;
        }
    }
}