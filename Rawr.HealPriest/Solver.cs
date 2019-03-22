using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.HealPriest
{

    public class BaseSolver
    {
        public Character character;
        public Stats stats;
        public CalculationOptionsHealPriest calculationOptions;

        public string Role { get; protected set; }
        public string ActionList { get; protected set; }

        public List<ManaSource> ManaSources = new List<ManaSource>();
        public class ManaSource
        {
            public string Name { get; set; }
            public float Value { get; set; }

            public ManaSource(string name, float value)
            {
                Name = name; Value = value;
            }
        }

        public float ProcInterval(float ProcChance, float ProcDelay, float ProcCooldown)
        {
            float ProcActual = 1f - (float)Math.Pow(1f - ProcChance, 1f / ProcChance);
            float EffCooldown = ProcCooldown + ProcDelay * 0.5f + (float)Math.Log(ProcChance) / (float)Math.Log(ProcActual) * ProcDelay / ProcActual;
            return EffCooldown;
        }

        public BaseSolver(Stats _stats, Character _char)
        {
            stats = _stats;
            character = _char;
            calculationOptions = character.CalculationOptions as CalculationOptionsHealPriest;

            Role = string.Empty;
            ActionList = "Cast List:";
        }

        public virtual void Calculate(CharacterCalculationsHealPriest calculatedStats)
        {
            ActionList += "- Virtual.";
            calculatedStats.HPSBurstPoints = calculatedStats.HPSSustainPoints = calculatedStats.SurvivabilityPoints = 0;
        }
    }

    public class Solver : BaseSolver
    {
        private eRole role;
      
        public Solver(Stats _stats, Character _char)
            : base(_stats, _char)
        {
            role = (eRole)calculationOptions.Role;

            if (role == eRole.AUTO_Tank) // OOOH MAGIC TANK ROTATION!!!
            {
                Role = "Auto ";
                if (character.PriestTalents.Penance > 0)
                {
                    if (character.PriestTalents.DivineFury < 5)
                        role = eRole.Disc_Tank_FH; // Disc-MT, Using Flash Heal instead of GH
                    else
                        role = eRole.Disc_Tank_GH; // Disc-MT
                }
                else
                    role = eRole.Holy_Tank; // Holy-MT
            }
            else if (role == eRole.AUTO_Raid) // Raid rotation
            {
                Role = "Auto ";
                if (character.PriestTalents.Penance > 0)
                    role = eRole.Disc_Raid; // Disc-Raid (PW:S/Penance/Flash)
                else if (character.PriestTalents.CircleOfHealing > 0)
                    role = eRole.Holy_Raid; // Holy-Raid (CoH/FH)
                else
                    role = eRole.Flash_Heal; // Fallback to Flash Heal raid.

            }
        }

        public override void Calculate(CharacterCalculationsHealPriest calculatedStats)
        {
            Stats simstats = calculatedStats.BasicStats.Clone();

            float valanyrProc = 0f;

            // Pre calc Procs (Power boosting Procs)
            Stats UseProcs = new Stats();
            if (calculationOptions.ModelProcs)
            {
                foreach (SpecialEffect se in simstats.SpecialEffects())
                {
                    if (se.Stats.ManaRestore == 0 && se.Stats.Mp5 == 0)
                    {   // We handle mana restoration stats later.
                        if (se.Trigger == Trigger.Use)
                        {
                            float Factor = se.GetAverageFactor(3f, 1f);
                            UseProcs += se.Stats * Factor;
                            foreach (SpecialEffect s in se.Stats.SpecialEffects())
                                UseProcs += s.Stats * Factor * s.MaxStack;
                        }
                        else if (se.Trigger == Trigger.SpellCast
                            || se.Trigger == Trigger.HealingSpellCast
                            || se.Trigger == Trigger.HealingSpellHit)
                        {
                            if (se.Stats.ShieldFromHealed > 0)
                            {
                                valanyrProc = se.GetAverageUptime(2f, 1f) * se.Stats.ShieldFromHealed;
                            }
                            if (se.Stats.HighestStat > 0)
                            {
                                float greatnessProc = se.GetAverageStats(2f, 1f).HighestStat;
                                if (simstats.Spirit > simstats.Intellect)
                                    UseProcs.Spirit += greatnessProc;
                                else
                                    UseProcs.Intellect += greatnessProc;
                            }
                            else
                                UseProcs += se.GetAverageStats(2f, 1f);
                        }
                    }
                }
                #region old stuff
                /*                if (simstats.SpiritFor20SecOnUse2Min > 0)
                    // Trinkets with Use: Increases Spirit with. (Like Earring of Soulful Meditation / Bangle of Endless blessings)
                    UseProcs.Spirit += simstats.SpiritFor20SecOnUse2Min * 20f / 120f;
                //                if (simstats.BangleProc > 0)
                // Bangle of Endless Blessings. Use: 130 spirit over 20 seconds. 120 sec cd.
                //UseProcs.Spirit += 130f * 20f / 120f;              
                if (simstats.SpellPowerFor15SecOnUse2Min > 0)
                    UseProcs.SpellPower += simstats.SpellPowerFor15SecOnUse2Min * 15f / 120f;
                if (simstats.SpellPowerFor15SecOnUse90Sec > 0)
                    UseProcs.SpellPower += simstats.SpellPowerFor15SecOnUse90Sec * 15f / 90f;
                if (simstats.SpellPowerFor20SecOnUse2Min > 0)
                    UseProcs.SpellPower += simstats.SpellPowerFor20SecOnUse2Min * 20f / 120f;
                if (simstats.HasteRatingFor20SecOnUse5Min > 0)
                    UseProcs.SpellHaste += StatConversion.GetSpellHasteFromRating(simstats.HasteRatingFor20SecOnUse5Min) * 20f / 300f;
                if (simstats.HasteRatingFor20SecOnUse2Min > 0)
                    UseProcs.SpellHaste += StatConversion.GetSpellHasteFromRating(simstats.HasteRatingFor20SecOnUse2Min) * 20f / 120f;*/
                #endregion

                // Juggle out the original spell haste and put in new.
                if (UseProcs.HasteRating > 0)
                {
                    simstats.SpellHaste = (1 + simstats.SpellHaste)
                        / (1 + StatConversion.GetSpellHasteFromRating(simstats.HasteRating))
                        * (1 + StatConversion.GetSpellHasteFromRating(UseProcs.HasteRating + simstats.HasteRating))
                        - 1;
                }
                UseProcs.Spirit = (float)Math.Round(UseProcs.Spirit * (1 + simstats.BonusSpiritMultiplier));
                UseProcs.Intellect = (float)Math.Round(UseProcs.Intellect * (1 + simstats.BonusIntellectMultiplier));
                UseProcs.SpellPower += (float)Math.Round(UseProcs.Spirit * simstats.SpellDamageFromSpiritPercentage);
                simstats += UseProcs;
            }

            float solchance = (character.PriestTalents.HolySpecialization * 0.01f + simstats.SpellCrit) * character.PriestTalents.SurgeOfLight * 0.25f;
            float sol5chance = 1f - (float)Math.Pow(1f - solchance, 5);
            float healmultiplier = (1 + character.PriestTalents.TestOfFaith * 0.04f * calculationOptions.TestOfFaith / 100f) * (1 + simstats.HealingReceivedMultiplier) * (1 + simstats.BonusHealingDoneMultiplier);

            // Add on Renewed Hope crit & Grace for Disc Maintank Rotation.
            if (role == eRole.Disc_Tank_FH
                || role == eRole.Disc_Tank_GH)
            {
                simstats.SpellCrit += character.PriestTalents.RenewedHope * 0.02f;
                healmultiplier *= (1 + character.PriestTalents.Grace * 0.045f);
            }

            //Spell spell;
            Heal gh = new Heal(simstats, character);
            FlashHeal fh = new FlashHeal(simstats, character);
            CircleOfHealing coh = new CircleOfHealing(simstats, character);
            Penance penance = new Penance(simstats, character);
            PowerWordShield pws = new PowerWordShield(simstats, character);
            PrayerOfMending prom_1 = new PrayerOfMending(simstats, character, 1);
            PrayerOfMending prom_4 = new PrayerOfMending(simstats, character, 4);
            //PrayerOfMending prom_max = new PrayerOfMending(simstats, character);
            Renew renew = new Renew(simstats, character);
            PrayerOfHealing proh_max = new PrayerOfHealing(simstats, character);

            // Surge of Light Flash Heal (cannot crit, is free)
            FlashHeal fh_sol = new FlashHeal(simstats, character);
            fh_sol.SurgeOfLight();

            // Serendipity haste
            //PrayerOfHealing proh_serendipity_1 = new PrayerOfHealing(simstats, character, 5, character.PriestTalents.Serendipity * 0.04f * 1);
            PrayerOfHealing proh_serendipity_2 = new PrayerOfHealing(simstats, character, 5, character.PriestTalents.Serendipity * 0.04f * 2);
            //PrayerOfHealing proh_serendipity_3 = new PrayerOfHealing(simstats, character, 5, character.PriestTalents.Serendipity * 0.04f * 3);

            // Borrowed Time Haste
            float oldSpellHaste = simstats.SpellHaste, oldSpellPower = simstats.SpellPower;
            simstats.SpellHaste = (1 + simstats.SpellHaste) * (1 + character.PriestTalents.BorrowedTime * 0.05f) - 1;
            if (simstats.PWSBonusSpellPowerProc > 0)
                simstats.SpellPower += simstats.PWSBonusSpellPowerProc;
            Heal gh_bt = new Heal(simstats, character);
            FlashHeal fh_bt = new FlashHeal(simstats, character);
            Penance penance_bt = new Penance(simstats, character);
            PowerWordShield pws_bt = new PowerWordShield(simstats, character);
            PrayerOfMending prom_1_bt = new PrayerOfMending(simstats, character, 1);
            PrayerOfMending prom_4_bt = new PrayerOfMending(simstats, character, 4);
            PrayerOfHealing proh_max_bt = new PrayerOfHealing(simstats, character);
            simstats.SpellHaste = oldSpellHaste;
            simstats.SpellPower = oldSpellPower;

            List<Spell> sr = new List<Spell>();
            switch (role)
            {
                case eRole.Greater_Heal:     // Greater Heal Spam
                    Role += "Greater Heal";
                    sr.Add(gh);
                    break;
                case eRole.Flash_Heal:     // Flash Heal Spam
                    Role += "Flash Heal";
                    sr.Add(fh);
                    break;
                case eRole.CoH_PoH:     // Circle of Healing/Prayer of Healing spam
                    if (character.PriestTalents.CircleOfHealing > 0)
                    {
                        Role += "CoH + PoH";
                        sr.Add(coh);
                        sr.Add(proh_max);
                        sr.Add(proh_max);
                    }
                    else if (character.PriestTalents.BorrowedTime > 0)
                    {
                        Role += "PWS + PoH";
                        sr.Add(pws);
                        sr.Add(proh_max_bt);
                    }
                    else
                    {
                        Role += "PoH";
                        sr.Add(proh_max);
                    }
                    break;
                case eRole.Holy_Tank:     // Holy MT Healing, renew + prom + ghx5 repeat
                    Role += "Holy Tank";
                    sr.Add(renew);      // 1.5s 1.5  -13.5 -??.?
                    sr.Add(prom_1);     // 1.5s 3.0  -12.0 -8.5
                    sr.Add(gh);         // 2.5s 5.5  -9.5  -6
                    sr.Add(gh);         // 2.5s 8.0  -7.0  -3.5
                    sr.Add(gh);         // 2.5s 10.5 -4.5  -1.0
                    sr.Add(gh);         // 2.5s 13.0 -2    -??
                    sr.Add(gh);         // 2.5s 15.5 -??   -??   Although, adjusted for haste and improved holy conc, this gets better and better.
                    break;
                case eRole.Holy_Raid:     // Holy Raid Healing, prom, coh, fhx2, proh, coh, fhx2, proh (2 haste stacks if serendipity)
                    Role += "Holy Raid";
                    sr.Add(prom_4);   // 1.5s 1.5 -8.5
                    sr.Add(coh);        // 1.5s 3.0 -7.0
                    sr.Add(fh);        // 1.5s 4.5 -5.5
                    sr.Add(fh);         // 1.5s 6.0 -4.0
                    sr.Add(proh_serendipity_2);
                    sr.Add(coh);
                    sr.Add(fh);
                    sr.Add(fh);
                    sr.Add(proh_serendipity_2);
                    // Repeat
                    break;
                case eRole.Disc_Tank_GH:     // Disc MT Healing, pws, penance, prom, gh, penance
                    Role += "Disc Tank w/Gheal";
                    sr.Add(pws);        
                    sr.Add(penance_bt); 
                    sr.Add(prom_1_bt);  
                    sr.Add(gh_bt);      
                    sr.Add(gh);         
                    sr.Add(gh);         
                    sr.Add(gh);         
                    // repeat
                    break;
                case eRole.Disc_Tank_FH:     // Disc MT Healing, pws, penance, prom, fh - Does not have Divine Fury.
                    Role += "Disc Tank w/Fheal";
                    sr.Add(pws);        
                    sr.Add(penance_bt); 
                    sr.Add(prom_1_bt);  
                    sr.Add(fh_bt);      
                    sr.Add(fh);         
                    sr.Add(fh);         
                    sr.Add(fh);         
                    sr.Add(fh);         
                    sr.Add(fh);         
                    // repeat
                    break;
                case eRole.Disc_Raid:     // Disc Raid Healing, pws, penance, prom, pw:s, fh, fh
                    Role += "Disc Raid";
                    sr.Add(pws);        // 1.5  1.5  -2.5  -??   -??
                    sr.Add(penance_bt); // 1.5  3.0  -1.0  -8.0  -??
                    sr.Add(prom_4_bt);  // 1.5  4.5  -??   -6.5  -8.5
                    sr.Add(pws_bt);     // 1.5  6.0  -2.5  -5.0  -7.0
                    sr.Add(fh_bt);      // 1.5  7.5  -1.0  -3.5  -5.5
                    // repeat
                    break;
                case eRole.Holy_Raid_Renew:    // Holy Raid healing with Renew. Almost no FH.
                    Role += "Holy Raid Renew";
                    sr.Add(prom_4);
                    sr.Add(coh);
                    sr.Add(renew);
                    sr.Add(renew);
                    sr.Add(renew);
                    sr.Add(renew);
                    sr.Add(renew);
                    sr.Add(prom_4);
                    sr.Add(coh);
                    sr.Add(renew);
                    sr.Add(renew);
                    sr.Add(renew);
                    sr.Add(renew);
                    sr.Add(fh);
                    break;
                default:
                    break;
            }

            foreach (Spell s in sr)
                ActionList += "\r\n- " + s.Name;

            float manacost = 0, cyclelen = 0, healamount = 0, solctr = 0, castctr = 0, castlandctr = 0, crittable = 0, holyconccast = 0, holyconccrit = 0, pwscasts = 0;
            float divineaegis = character.PriestTalents.DivineAegis * 0.1f * (1f + stats.PriestHeal_T9_4pc);
            for (int x = 0; x < sr.Count; x++)
            {
                float mcost = 0, absorb = 0, heal = 0, rheal = 0, clen = 0;
                if (sr[x] == gh || sr[x] == gh_bt)
                {   // Greater Heal (A Borrowed Time GHeal cannot also be improved Holy conc hasted, so this works)
                    clen = sr[x].CastTime;
                    rheal = sr[x].AvgTotHeal * healmultiplier;
                    absorb = sr[x].AvgCrit * healmultiplier * sr[x].CritChance * divineaegis;
                    solctr = 1f - (1f - solctr) * (1f - solchance);
                    mcost = sr[x].ManaCost;
                    mcost -= simstats.ManaGainOnGreaterHealOverheal * calculationOptions.Serendipity / 100f;
                    castctr++;
                    castlandctr++;
                    crittable += sr[x].CritChance;
                    holyconccast++;
                    holyconccrit += sr[x].CritChance;
                }
                else if (sr[x] == fh || sr[x] == fh_bt)
                {   // Flash Heal (Same applies to FH as GHeal with regards to borrowed time)
                    clen = sr[x].CastTime;
                    rheal = sr[x].AvgTotHeal * healmultiplier;
                    if (simstats.PriestHeal_T10_2pc > 0)
                        heal = rheal * 1 / 9;   // 33% chance of getting 33% of healed amount over 6s. 1/9 (11.11...%)
                    absorb = sr[x].AvgCrit * healmultiplier * sr[x].CritChance * divineaegis;
                    solctr = 1f - (1f - solctr) * (1f - solchance);
                    mcost = sr[x].ManaCost;
                    mcost -= mcost * solctr;
                    solctr = 0;
                    castctr++;
                    castlandctr++;
                    crittable += sr[x].CritChance;
                    holyconccast++;
                    holyconccrit += sr[x].CritChance;
                }
                else if (sr[x] == penance || sr[x] == penance_bt)
                {
                    clen = sr[x].CastTime;
                    rheal = sr[x].AvgTotHeal * healmultiplier;
                    absorb = sr[x].AvgCrit * healmultiplier * sr[x].CritChance * divineaegis;
                    mcost = sr[x].ManaCost;
                    castctr++;
                    castlandctr += 3f; // Penance counts as 3 casts for some purposes.
                    crittable += 1f - (float)Math.Pow(1f - sr[x].CritChance, 3f);
                }
                else if (sr[x] == coh)
                {   // Circle of Healing
                    clen = coh.GlobalCooldown;
                    heal = coh.AvgTotHeal * healmultiplier;
                    solctr = 1f - (1f - solctr) * (1f - sol5chance);
                    mcost = coh.ManaCost;
                    castctr++;
                    castlandctr += sr[x].Targets;
                    crittable += 1f - (float)Math.Pow(1f - sr[x].CritChance, sr[x].Targets);
                }
                else if (sr[x] == proh_max || sr[x] == proh_serendipity_2 || sr[x] == proh_max_bt)
                {
                    clen = sr[x].CastTime;
                    heal = sr[x].AvgTotHeal * healmultiplier;
                    solctr = 1f - (1f - solctr) * (1f - sol5chance);
                    absorb = sr[x].AvgCrit * sr[x].Targets * healmultiplier * sr[x].CritChance * divineaegis;
                    mcost = sr[x].ManaCost;
                    castctr++;
                    castlandctr += sr[x].Targets;
                    crittable += 1f - (float)Math.Pow(1f - sr[x].CritChance, sr[x].Targets);
                }
                else if (sr[x] == renew)
                {   // Renew
                    clen = renew.GlobalCooldown;
                    heal = renew.AvgTotHeal * healmultiplier;
                    mcost = renew.ManaCost;
                    castctr++;
                    castlandctr++;
                    if (character.PriestTalents.ImprovedRenew > 0)
                    {
                        holyconccast++;
                        holyconccrit += renew.CritChance;
                    }
                }
                else if (sr[x] == pws || sr[x] == pws_bt)
                {
                    clen = sr[x].GlobalCooldown;
                    absorb = sr[x].AvgTotHeal;
                    mcost = sr[x].ManaCost;
                    if (character.PriestTalents.GlyphofPowerWordShield)
                    {
                        heal = absorb * 0.2f * healmultiplier * (1 + 0.5f * (stats.SpellCrit + character.PriestTalents.HolySpecialization));   // Not entirely right, but close enough.
                        // Divine Aegis isn't yet entirely fixed for PWS
                        // absorb += heal * sr[x].CritChance * divineaegis;
                    }
                    castctr++;
                    castlandctr++;
                    pwscasts++;
                }
                else if (sr[x] == prom_1 || sr[x] == prom_4 || sr[x] == prom_1_bt || sr[x] == prom_4_bt)
                {
                    clen = sr[x].GlobalCooldown;
                    heal = sr[x].AvgTotHeal * healmultiplier;
                    absorb = sr[x].AvgCrit * sr[x].Targets * healmultiplier * sr[x].CritChance * divineaegis;
                    mcost = sr[x].ManaCost;
                    castctr++;
                    castlandctr += sr[x].Targets;
                    crittable += 1f - (float)Math.Pow(1f - sr[x].CritChance, sr[x].Targets);
                }
                absorb += valanyrProc * (heal + rheal);
                cyclelen += clen;
                healamount += heal + rheal + absorb;
                manacost += mcost;
                //metareductiontot += metaSpellCostReduction;
            }
         
            // Real Cyclelen also has time for FSR. To get 80% FSR, a cycle of 20 seconds needs to include:
            // (20 + 5) / 0.8 = 31.25 seconds. (31.25 - 5 - 20 = 6.25 / 31.25 = 0.2 seconds of FSR regen).
            //float realcyclelen = (cyclelen + 5f) / (calculationOptions.FSRRatio / 100f);
            // Extra fudge model: (As you approach 100% FSR, realcyclelen approaches cyclelen)
            //float realcyclelen = (cyclelen + 5f * (1f - (float)Math.Pow(calculationOptions.FSRRatio / 100f, 2f))) / (calculationOptions.FSRRatio / 100f);
            // Xtreme fudge model: Cast 25 seconds, 5 seconds no casting, then slap on FSR.
            // ((25 + 5) / FSR) / 25 * cyclelen = realcyclelen.
            float realcyclelen = cyclelen * ((25f + 5f) / (calculationOptions.FSRRatio / 100f)) / 25f;
            float avgcastlen = realcyclelen / castctr;
            float avgcastlandlen = realcyclelen / castlandctr;
            float avgcritcast = crittable / sr.Count;

            float periodicRegenOutFSR = StatConversion.GetSpiritRegenSec(simstats.Spirit, simstats.Intellect);
            // Add up all mana gains.
            float regen = 0, tmpregen = 0;

            // Spirit/Intellect based Regeneration and MP5
            tmpregen = periodicRegenOutFSR * (1f - calculationOptions.FSRRatio / 100f);
            if (tmpregen > 0f)
            {
                ManaSources.Add(new ManaSource("OutFSR", tmpregen));
                regen += tmpregen;
            }
            tmpregen = periodicRegenOutFSR * simstats.SpellCombatManaRegeneration * calculationOptions.FSRRatio / 100f;
            if (tmpregen > 0f)
            {
                ManaSources.Add(new ManaSource("Meditation", tmpregen));
                regen += tmpregen;
            }
            if (character.PriestTalents.HolyConcentration > 0)
            {
                float hceffect = character.PriestTalents.HolyConcentration * 0.5f / 3f;
                float hccastinterval = cyclelen / holyconccast;
                float hccritchance = holyconccrit / holyconccast;
                // Calculate chance that you crit within 8 seconds.
                float hcuptime = 1f - (float)Math.Pow(1f - hccritchance, 8f / hccastinterval);
                tmpregen = (periodicRegenOutFSR * (1f - calculationOptions.FSRRatio / 100f)
                    + periodicRegenOutFSR * simstats.SpellCombatManaRegeneration * calculationOptions.FSRRatio / 100f)
                    * hcuptime * hceffect;
                if (tmpregen > 0)
                {
                    ManaSources.Add(new ManaSource("Holy Concentration", tmpregen));
                    regen += tmpregen;
                }
            }
            if (character.PriestTalents.Rapture > 0 && pwscasts > 0)
            {   // New Rapture restores 1.5% - 2% - 2.5% of max mana every 12 seconds at best.
                float rapturereturn = 0.015f + (character.PriestTalents.Rapture - 1) * 0.005f;
                float maxrapture = simstats.Mana * rapturereturn / 12f;
                tmpregen = maxrapture * calculationOptions.Rapture / 100f;
                if (tmpregen > 0)
                {
                    ManaSources.Add(new ManaSource("Rapture", tmpregen));
                    regen += tmpregen;
                }
            }
            tmpregen = simstats.Mp5 / 5;
            ManaSources.Add(new ManaSource("MP5", tmpregen));
            regen += tmpregen;
            tmpregen = simstats.Mana / (calculationOptions.FightLengthSeconds);
            ManaSources.Add(new ManaSource("Intellect", tmpregen));
            regen += tmpregen;
            if (calculationOptions.ModelProcs)
            {
                float heal = 0f;
                foreach (SpecialEffect se in simstats.SpecialEffects())
                {
                    tmpregen = 0f;                  
                    if (se.Stats.ManaRestore > 0 || se.Stats.Mp5 > 0)
                    {
                        if (se.Trigger == Trigger.SpellCast
                            || se.Trigger == Trigger.HealingSpellCast)
                        {
                            tmpregen = se.GetAverageStats(avgcastlen, 1f, 0f, calculationOptions.FightLengthSeconds).ManaRestore
                                + se.GetAverageStats(avgcastlen, 1f, 0f, calculationOptions.FightLengthSeconds).Mp5 / 5;
                        }
                        else if (se.Trigger == Trigger.HealingSpellHit)
                        {
                            tmpregen = se.GetAverageStats(avgcastlandlen, 1f, 0f, calculationOptions.FightLengthSeconds).ManaRestore
                                + se.GetAverageStats(avgcastlandlen, 1f, 0f, calculationOptions.FightLengthSeconds).Mp5 / 5;
                        }
                        else if (se.Trigger == Trigger.SpellCrit
                            || se.Trigger == Trigger.HealingSpellCrit)
                        {
                            tmpregen = se.GetAverageStats(avgcastlen, avgcritcast / avgcastlen, 0f, calculationOptions.FightLengthSeconds).ManaRestore
                                + se.GetAverageStats(avgcastlen, avgcritcast / avgcastlen, 0f, calculationOptions.FightLengthSeconds).Mp5 / 5;
                        }
                        else if (se.Trigger == Trigger.Use)
                        {
                            tmpregen = se.GetAverageStats().ManaRestore
                                + se.GetAverageStats().Mp5 / 5;

                        }
                    }
                    if (tmpregen > 0)
                    {
                        ManaSources.Add(new ManaSource(se.ToString(), tmpregen));
                        regen += tmpregen;
                    }
                    if (se.Stats.Healed > 0f)
                    {
                        if (se.Trigger == Trigger.HealingSpellCast
                            || se.Trigger == Trigger.SpellCast)
                            heal += se.GetAverageStats(avgcastlen, 1f, 0f, calculationOptions.FightLengthSeconds).Healed;
                        else if (se.Trigger == Trigger.HealingSpellHit)
                            heal += se.GetAverageStats(avgcastlandlen, 1f, 0f, calculationOptions.FightLengthSeconds).Healed;
                        else if (se.Trigger == Trigger.SpellCrit
                            || se.Trigger == Trigger.HealingSpellCrit)
                            heal += se.GetAverageStats(avgcastlen, avgcritcast / avgcastlen, 0f, calculationOptions.FightLengthSeconds).Healed;
                        else if (se.Trigger == Trigger.Use)
                            heal += se.GetAverageStats().Healed;
                    }
                }
                healamount += heal * (1f + stats.SpellCrit * 0.5f) * healmultiplier * (1f + valanyrProc);
                #region old procs
                /*if (simstats.BangleProc > 0)
                {
                    float BangleLevelMod = 0.15f - (character.Level - 70f) / 200f;
                    tmpregen = periodicRegenOutFSR * BangleLevelMod * 15f / 60f * (1f - (float)Math.Pow(1f - BangleLevelMod, 15f / avgcastlen));
                    if (BangleLevelMod > 0f && tmpregen > 0f)
                    {
                        ManaSources.Add(new ManaSource("Bangle of Endless Blessings", tmpregen));
                        regen += tmpregen;
                    }
                }
                if (simstats.FullManaRegenFor15SecOnSpellcast > 0)
                {
                    // Blue Dragon. 2% chance to proc on cast, no known internal cooldown. calculate as the chance to have procced during its duration. 2% proc/cast.
                    tmpregen = periodicRegenOutFSR * (1f - simstats.SpellCombatManaRegeneration) * (1f - (float)Math.Pow(1f - 0.02f, 15f / avgcastlen));
                    if (tmpregen > 0f)
                    {
                        ManaSources.Add(new ManaSource("Darkmoon Card: Blue Dragon", tmpregen));
                        regen += tmpregen;
                    }
                }
                if (simstats.ManaRestoreOnCrit_25_45 > 0)
                {   // X mana back every 25%*critchance spell every 45seconds.
                    tmpregen = simstats.ManaRestoreOnCrit_25_45 / ProcInterval(0.25f * avgcritcast, avgcritcast, 45f);
                    if (tmpregen > 0f)
                    {
                        ManaSources.Add(new ManaSource("Soul of the Dead" , tmpregen));
                        regen += tmpregen;
                    }
                }
                if (simstats.ManaRestoreOnCast_10_45 > 0)
                {
                    tmpregen = simstats.ManaRestoreOnCast_10_45 / ProcInterval(0.1f, avgcastlen, 45f);
                    if (tmpregen > 0f)
                    {
                        if (simstats.ManaRestoreOnCast_10_45 == 300)
                            ManaSources.Add(new ManaSource("Je'Tze's Bell", tmpregen));
                        else if (simstats.ManaRestoreOnCast_10_45 == 528)
                            ManaSources.Add(new ManaSource("Spark of Life", tmpregen));
                        else if (simstats.ManaRestoreOnCast_10_45 == 228)
                            ManaSources.Add(new ManaSource("Memento of Tyrande", tmpregen));
                        else
                            ManaSources.Add(new ManaSource("MP5 Proc Trinket", tmpregen));
                        regen += tmpregen;
                    }
                }*/

                /*float trinketmp5 = 0;
                
                if (simstats.Mp5OnCastFor20SecOnUse2Min > 0)
                    trinketmp5 += (20f / avgcastlen) * 21f / 2f * 20f / 120f;
                if (simstats.ManacostReduceWithin15OnHealingCast > 0)
                    trinketmp5 += simstats.ManacostReduceWithin15OnHealingCast * (1f - (float)Math.Pow(1f - 0.02f, castctr)) * 5f / cyclelen;
                if (simstats.ManaregenFor8SecOnUse5Min > 0)
                    trinketmp5 += simstats.ManaregenFor8SecOnUse5Min * 8f / 300f * 5f;
                if (trinketmp5 > 0f)
                {
                    tmpregen = trinketmp5 / 5f;
                    ManaSources.Add(new ManaSource("Trinkets", tmpregen));
                    regen += tmpregen;
                }
                if (simstats.ManacostReduceWithin15OnUse1Min > 0)
                    manacost -= simstats.ManacostReduceWithin15OnUse1Min * (float)Math.Floor(15f / cyclelen * sr.Count) / 60f;
                 */
                #endregion
            }

            // External and Other mana sources.
            tmpregen = simstats.Mana * simstats.ManaRestoreFromMaxManaPerSecond * calculationOptions.Replenishment / 100f;
            if (tmpregen > 0f)
            {
                ManaSources.Add(new ManaSource("Replenishment", tmpregen));
                regen += tmpregen;
            }
          
            ActionList += "\r\n\r\nMana Options:";

            float mp1use = manacost / realcyclelen;

            if (mp1use > regen && character.Race == CharacterRace.BloodElf)
            {   // Arcane Torrent is 6% max mana every 2 minutes.
                tmpregen = simstats.Mana * 0.06f / 120f;
                ManaSources.Add(new ManaSource("Arcane Torrent", tmpregen));
                regen += tmpregen;
                ActionList += string.Format("\r\n- Used Arcane Torrent");
            }

            if (mp1use > regen && calculationOptions.ManaAmt > 0f)
            {
                float ManaPot = calculationOptions.ManaAmt * (1f + simstats.BonusManaPotion);
                tmpregen = ManaPot / (calculationOptions.FightLengthSeconds);
                ManaSources.Add(new ManaSource("Mana Potion", tmpregen));
                ActionList += string.Format("\r\n- Used Mana Potion ({0})", ManaPot.ToString("0"));
                regen += tmpregen;
            }
            if (mp1use > regen)
            {
                tmpregen = (simstats.Mana * 0.4f * calculationOptions.Shadowfiend / 100f)
                    / ((5f - character.PriestTalents.VeiledShadows * 1f) * 60f);
                ManaSources.Add(new ManaSource("Shadowfiend", tmpregen));
                ActionList += string.Format("\r\n- Used Shadowfiend");
                regen += tmpregen;
            }
            if (mp1use > regen)
            {   // Hymn of Hope increases max mana by 20% and restores 3% of total mana for 4 ticks, 5 if glyphed.
                float ticks = character.PriestTalents.GlyphofHymnofHope ? 5 : 4;
                tmpregen = (simstats.Mana * 1.2f * 0.03f * ticks)
                    / (6f * 60f);
                ManaSources.Add(new ManaSource("Hymn of Hope", tmpregen));
                ActionList += string.Format("\r\n- Used Hymn of Hope");
                regen += tmpregen;
            }

            if (mp1use > regen)
            {
                ActionList += string.Format("\r\n- {0} mp5 deficit. Could use more mana!", ((mp1use - regen) * 5).ToString("0"));
            }

            calculatedStats.HPSBurstPoints = healamount / cyclelen;
            // Sustained is limited by how much mana you regenerate over the time it would take to cast the spells, divided by the cost.
            if (regen > mp1use) // Regenerating more mana than we can use. Dont make user believe this is an upgrade.
                calculatedStats.HPSSustainPoints = calculatedStats.HPSBurstPoints;
            else
                calculatedStats.HPSSustainPoints = calculatedStats.HPSBurstPoints * regen / mp1use;

            // Lets just say that 15% of resilience scales all health by 150%.
            float Resilience = (float)Math.Min(15f, StatConversion.GetCritReductionFromResilience(simstats.Resilience) * 100f) / 15f;
            calculatedStats.SurvivabilityPoints = calculatedStats.BasicStats.Health * (Resilience * 1.5f + 1f) * calculationOptions.Survivability / 100f;
        }   

    }

    public class AdvancedSolver : BaseSolver
    {
        public AdvancedSolver(Stats _stats, Character _char)
            : base(_stats, _char)
        {
            Role = "Custom";
        }

        public override void Calculate(CharacterCalculationsHealPriest calculatedStats)
        {
            Stats simstats = calculatedStats.BasicStats.Clone();
            float valanyrProc = 0f;

            #region old old old
            /*
            // Pre calc Procs (Power boosting Procs)
            if (calculationOptions.ModelProcs)
            {
                if (simstats.SpiritFor20SecOnUse2Min > 0)
                    // Trinkets with Use: Increases Spirit with. (Like Earring of Soulful Meditation / Bangle of Endless blessings)
                    UseProcs.Spirit += simstats.SpiritFor20SecOnUse2Min * 20f / 120f;
                //                if (simstats.BangleProc > 0)
                // Bangle of Endless Blessings. Use: 130 spirit over 20 seconds. 120 sec cd.
                //UseProcs.Spirit += 130f * 20f / 120f;              
                if (simstats.SpellPowerFor15SecOnUse2Min > 0)
                    UseProcs.SpellPower += simstats.SpellPowerFor15SecOnUse2Min * 15f / 120f;
                if (simstats.SpellPowerFor15SecOnUse90Sec > 0)
                    UseProcs.SpellPower += simstats.SpellPowerFor15SecOnUse90Sec * 15f / 90f;
                if (simstats.SpellPowerFor20SecOnUse2Min > 0)
                    UseProcs.SpellPower += simstats.SpellPowerFor20SecOnUse2Min * 20f / 120f;
                if (simstats.HasteRatingFor20SecOnUse5Min > 0)
                    UseProcs.SpellHaste += StatConversion.GetSpellHasteFromRating(simstats.HasteRatingFor20SecOnUse5Min) * 20f / 300f;
                if (simstats.HasteRatingFor20SecOnUse2Min > 0)
                    UseProcs.SpellHaste += StatConversion.GetSpellHasteFromRating(simstats.HasteRatingFor20SecOnUse2Min) * 20f / 120f;
            }

            UseProcs.Spirit = (float)Math.Round(UseProcs.Spirit * (1 + simstats.BonusSpiritMultiplier));
            UseProcs.SpellPower += (float)Math.Round(UseProcs.Spirit * simstats.SpellDamageFromSpiritPercentage);

            simstats += UseProcs;*/
            #endregion
            Stats UseProcs = new Stats();
            if (calculationOptions.ModelProcs)
            {
                foreach (SpecialEffect se in simstats.SpecialEffects())
                {
                    if (se.Stats.ManaRestore == 0 && se.Stats.Mp5 == 0)
                    {   // We handle mana restoration stats later.
                        if (se.Trigger == Trigger.Use)
                            UseProcs += se.GetAverageStats(2f, 1f);
                        else if (se.Trigger == Trigger.SpellCast
                            || se.Trigger == Trigger.HealingSpellCast
                            || se.Trigger == Trigger.HealingSpellHit)
                        {
                            if (se.Stats.ShieldFromHealed > 0)
                            {
                                valanyrProc = se.GetAverageUptime(2f, 1f) * se.Stats.ShieldFromHealed;
                            }
                            if (se.Stats.HighestStat > 0)
                            {
                                float greatnessProc = se.GetAverageStats(2f, 1f).HighestStat;
                                if (simstats.Spirit > simstats.Intellect)
                                    UseProcs.Spirit += greatnessProc;
                                else
                                    UseProcs.Intellect += greatnessProc;
                            }
                            else
                                UseProcs += se.GetAverageStats(2f, 1f);
                        }
                    }
                }
                #region old stuff
                /*                if (simstats.SpiritFor20SecOnUse2Min > 0)
                    // Trinkets with Use: Increases Spirit with. (Like Earring of Soulful Meditation / Bangle of Endless blessings)
                    UseProcs.Spirit += simstats.SpiritFor20SecOnUse2Min * 20f / 120f;
                //                if (simstats.BangleProc > 0)
                // Bangle of Endless Blessings. Use: 130 spirit over 20 seconds. 120 sec cd.
                //UseProcs.Spirit += 130f * 20f / 120f;              
                if (simstats.SpellPowerFor15SecOnUse2Min > 0)
                    UseProcs.SpellPower += simstats.SpellPowerFor15SecOnUse2Min * 15f / 120f;
                if (simstats.SpellPowerFor15SecOnUse90Sec > 0)
                    UseProcs.SpellPower += simstats.SpellPowerFor15SecOnUse90Sec * 15f / 90f;
                if (simstats.SpellPowerFor20SecOnUse2Min > 0)
                    UseProcs.SpellPower += simstats.SpellPowerFor20SecOnUse2Min * 20f / 120f;
                if (simstats.HasteRatingFor20SecOnUse5Min > 0)
                    UseProcs.SpellHaste += StatConversion.GetSpellHasteFromRating(simstats.HasteRatingFor20SecOnUse5Min) * 20f / 300f;
                if (simstats.HasteRatingFor20SecOnUse2Min > 0)
                    UseProcs.SpellHaste += StatConversion.GetSpellHasteFromRating(simstats.HasteRatingFor20SecOnUse2Min) * 20f / 120f;*/
                #endregion

                // Juggle out the original spell haste and put in new.
                if (UseProcs.HasteRating > 0)
                {
                    simstats.SpellHaste = (1 + simstats.SpellHaste) 
                        / (1 + StatConversion.GetSpellHasteFromRating(simstats.HasteRating))
                        * (1 + StatConversion.GetSpellHasteFromRating(UseProcs.HasteRating + simstats.HasteRating))
                        - 1;
                }
                UseProcs.Spirit = (float)Math.Round(UseProcs.Spirit * (1 + simstats.BonusSpiritMultiplier));
                UseProcs.Intellect = (float)Math.Round(UseProcs.Intellect * (1 + simstats.BonusIntellectMultiplier));
                UseProcs.SpellPower += (float)Math.Round(UseProcs.Spirit * simstats.SpellDamageFromSpiritPercentage);
                simstats += UseProcs;
            }

            // Insightful Earthstorm Diamond.
            float healmultiplier = (1 + character.PriestTalents.TestOfFaith * 0.04f * calculationOptions.TestOfFaith / 100f) * (1 + character.PriestTalents.Grace * 0.045f) * (1 + simstats.HealingReceivedMultiplier) * (1 + simstats.BonusHealingDoneMultiplier);
            float divineaegis = character.PriestTalents.DivineAegis * 0.1f
                * (1f + stats.PriestHeal_T9_4pc);

            float solchance = (character.PriestTalents.HolySpecialization * 0.01f + simstats.SpellCrit) * character.PriestTalents.SurgeOfLight * 0.25f;
            float solbhchance = 1f - (float)Math.Pow(1f - solchance, 2);
            float solcohchance = 1f - (float)Math.Pow(1f - solchance, character.PriestTalents.GlyphofCircleOfHealing ? 6 : 5);
            float solpohhnchance = 1f - (float)Math.Pow(1f - solchance, 5);
            float solpromchance = 1f - (float)Math.Pow(1f - solchance, (calculationOptions.ProMCast == 0)?0:(calculationOptions.ProMTicks / calculationOptions.ProMCast));

            // Add on Renewed Hope crit for Disc Maintank Rotation, adjusted by uptime of Weakened Soul
            float WeakenedSoulUptime = (float)Math.Min(1f, calculationOptions.PWSCast * 15f / calculationOptions.FightLengthSeconds);
            simstats.SpellCrit += character.PriestTalents.RenewedHope * 0.02f * WeakenedSoulUptime;
            int instantCasts = calculationOptions.CoHCast + calculationOptions.DispelCast + calculationOptions.HolyNovaCast
                + calculationOptions.PenanceCast + calculationOptions.ProMCast + calculationOptions.PWSCast
                + calculationOptions.RenewCast;
            int nonInstantCasts = calculationOptions.BindingHealCast + calculationOptions.DivineHymnCast
                + calculationOptions.FlashHealCast + calculationOptions.GreaterHealCast + calculationOptions.MDCast
                + calculationOptions.PoHCast;

            float BorrowedTimeHaste = character.PriestTalents.BorrowedTime * 0.05f * (nonInstantCasts > 0 ? (float)Math.Min(calculationOptions.PWSCast / nonInstantCasts, 1f) : 1f);
            if (BorrowedTimeHaste > 0)
                simstats.SpellHaste = (1 + simstats.SpellHaste) * (1 + BorrowedTimeHaste) - 1;
            if (simstats.PWSBonusSpellPowerProc > 0 && calculationOptions.PWSCast > 0)
            {
                float timeBetweenShields = calculationOptions.FightLengthSeconds / calculationOptions.PWSCast;
                float bonusSpellPower = simstats.PWSBonusSpellPowerProc * (float)Math.Min(1f, 5 / timeBetweenShields);
                simstats.SpellPower += bonusSpellPower;
            }

            int TotalCasts = 0;
            float ManaUsed = 0f;
            float TimeUsed = 0f;
            float BaseTimeUsed = 0f;
            float DirectHeal = 0f;
            float OtherHeal = 0f;
            float AbsorbHeal = 0f;
            float CritCounter = 0f;
            float HCCritCounter = 0f;

            FlashHeal fh = new FlashHeal(simstats, character);
            BindingHeal bh = new BindingHeal(simstats, character);
            Heal gh = new Heal(simstats, character);
            Penance pen = new Penance(simstats, character);
            Renew renew = new Renew(simstats, character);
            PrayerOfMending prom = new PrayerOfMending(simstats, character, 1);
            PrayerOfMending prom_max = new PrayerOfMending(simstats, character);
            PrayerOfHealing proh = new PrayerOfHealing(simstats, character);
            PowerWordShield pws = new PowerWordShield(simstats, character);
            CircleOfHealing coh = new CircleOfHealing(simstats, character, 1);
            CircleOfHealing coh_max = new CircleOfHealing(simstats, character);
            HolyNova hn = new HolyNova(simstats, character, 1);
            DivineHymn dh = new DivineHymn(simstats, character);
            Dispel dispel = new Dispel(simstats, character);
            MassDispel md = new MassDispel(simstats, character);

            // Calculate how many of the flash heals are actually paid for.
            float FreeFlashes = calculationOptions.FlashHealCast * solchance
                + calculationOptions.BindingHealCast * solbhchance
                + calculationOptions.GreaterHealCast * solchance
                + calculationOptions.CoHCast * solcohchance
                + calculationOptions.ProMCast * solpromchance
                + calculationOptions.PoHCast * solpohhnchance
                + calculationOptions.HolyNovaCast * solpohhnchance;
            FreeFlashes = (float)Math.Min(calculationOptions.FlashHealCast, FreeFlashes);

            // Flash Heal
            if (calculationOptions.FlashHealCast > 0)
            {
                TotalCasts += calculationOptions.FlashHealCast;
                ActionList += String.Format("\r\n- {0} Flash Heal{1}",
                    calculationOptions.FlashHealCast,
                    (FreeFlashes > 0f) ? String.Format(", {0} Surge of Lights", FreeFlashes.ToString("0")) : String.Empty);
                float Cost = fh.ManaCost * (calculationOptions.FlashHealCast - FreeFlashes);
                ManaUsed += Cost;
                TimeUsed += fh.CastTime * calculationOptions.FlashHealCast;
                BaseTimeUsed += fh.BaseCastTime * calculationOptions.FlashHealCast;
                DirectHeal += fh.AvgTotHeal * healmultiplier * calculationOptions.FlashHealCast;
                if (simstats.PriestHeal_T10_2pc > 0)
                    OtherHeal += fh.AvgTotHeal * healmultiplier * calculationOptions.FlashHealCast * (1 / 9); // 33% chance to get 33% of healed over time. (~11.11...%)
                AbsorbHeal += fh.AvgCrit * fh.CritChance * healmultiplier * calculationOptions.FlashHealCast * divineaegis;
                CritCounter += fh.CritChance * calculationOptions.FlashHealCast;
                HCCritCounter += fh.CritChance * calculationOptions.FlashHealCast;
            }

            // Binding Heal
            if (calculationOptions.BindingHealCast > 0)
            {
                TotalCasts += calculationOptions.BindingHealCast;
                ActionList += String.Format("\r\n- {0} Binding Heal", calculationOptions.BindingHealCast);
                ManaUsed += bh.ManaCost * calculationOptions.BindingHealCast;
                TimeUsed += bh.CastTime * calculationOptions.BindingHealCast;
                BaseTimeUsed += bh.BaseCastTime * calculationOptions.BindingHealCast;
                OtherHeal += bh.AvgTotHeal * healmultiplier * calculationOptions.BindingHealCast;
                AbsorbHeal += bh.AvgCrit * 2 * bh.CritChance * healmultiplier * calculationOptions.BindingHealCast * divineaegis;
                CritCounter += bh.CritChance * 2 * calculationOptions.BindingHealCast;
                HCCritCounter += bh.CritChance * 2 * calculationOptions.BindingHealCast;
            }

            // Greater Heal
            if (calculationOptions.GreaterHealCast > 0)
            {
                TotalCasts += calculationOptions.GreaterHealCast;
                ActionList += String.Format("\r\n- {0} Greater Heal", calculationOptions.GreaterHealCast);
                float Cost = gh.ManaCost * calculationOptions.GreaterHealCast;
                ManaUsed += Cost;
                TimeUsed += gh.CastTime * calculationOptions.GreaterHealCast;
                BaseTimeUsed += gh.BaseCastTime * calculationOptions.GreaterHealCast;
                DirectHeal += gh.AvgTotHeal * healmultiplier * calculationOptions.GreaterHealCast;
                AbsorbHeal += gh.AvgCrit * gh.CritChance * healmultiplier * calculationOptions.GreaterHealCast * divineaegis;
                CritCounter += gh.CritChance * calculationOptions.GreaterHealCast;
                HCCritCounter += gh.CritChance * calculationOptions.GreaterHealCast;
            }

            // Penance
            if (calculationOptions.PenanceCast > 0 && character.PriestTalents.Penance > 0)
            {
                TotalCasts += calculationOptions.PenanceCast;
                ActionList += String.Format("\r\n- {0} Penance", calculationOptions.PenanceCast);
                ManaUsed += pen.ManaCost * calculationOptions.PenanceCast;
                TimeUsed += pen.CastTime * calculationOptions.PenanceCast;
                BaseTimeUsed += pen.BaseCastTime * calculationOptions.PenanceCast;
                DirectHeal += pen.AvgTotHeal * healmultiplier * calculationOptions.PenanceCast;
                AbsorbHeal += pen.AvgCrit * pen.CritChance * healmultiplier * calculationOptions.PenanceCast * divineaegis;
                CritCounter += pen.CritChance * 3f * calculationOptions.PenanceCast;
            }

            // Renew
            if (calculationOptions.RenewCast > 0)
            {
                TotalCasts += calculationOptions.RenewCast;
                ActionList += String.Format("\r\n- {0} Renew, {1} Ticks", calculationOptions.RenewCast, calculationOptions.RenewTicks);
                ManaUsed += renew.ManaCost * calculationOptions.RenewCast;
                TimeUsed += renew.GlobalCooldown * calculationOptions.RenewCast;
                BaseTimeUsed += 1.5f * calculationOptions.RenewCast;
                OtherHeal += renew.AvgHeal / (renew.HotDuration * 3) * healmultiplier * calculationOptions.RenewTicks;
                if (character.PriestTalents.EmpoweredRenew > 0)
                {
                    DirectHeal += (renew.AvgTotHeal - renew.AvgHeal) * healmultiplier;
                    CritCounter += renew.CritChance * calculationOptions.RenewCast;
                    HCCritCounter += renew.CritChance * calculationOptions.RenewCast;
                }
            }

            // Prayer of Mending
            if (calculationOptions.ProMCast > 0)
            {
                TotalCasts += calculationOptions.ProMCast;
                ActionList += String.Format("\r\n- {0} Prayer of Mending, {1} Procs", calculationOptions.ProMCast, calculationOptions.ProMTicks);
                ManaUsed += prom.ManaCost * calculationOptions.ProMCast;
                TimeUsed += prom.GlobalCooldown * calculationOptions.ProMCast;
                BaseTimeUsed += 1.5f * calculationOptions.ProMCast;
                DirectHeal += prom.AvgTotHeal * healmultiplier * calculationOptions.ProMTicks;
                AbsorbHeal += prom.AvgCrit * prom.CritChance * healmultiplier * calculationOptions.ProMTicks * divineaegis;
                CritCounter += prom.CritChance * calculationOptions.ProMTicks;
            }

            // Prayer of Healing
            if (calculationOptions.PoHCast > 0)
            {
                TotalCasts += calculationOptions.PoHCast;
                ActionList += String.Format("\r\n- {0} Prayer of Healing", calculationOptions.PoHCast);
                ManaUsed += proh.ManaCost * calculationOptions.PoHCast;
                TimeUsed += proh.CastTime * calculationOptions.PoHCast;
                BaseTimeUsed += proh.BaseCastTime * calculationOptions.PoHCast;
                DirectHeal += proh.AvgTotHeal * healmultiplier * calculationOptions.PoHCast;
                AbsorbHeal += proh.AvgCrit * proh.CritChance * healmultiplier * calculationOptions.PoHCast * divineaegis;
                CritCounter += proh.CritChance * 5 * calculationOptions.PoHCast;
            }

            // PW:Shield
            if (calculationOptions.PWSCast > 0)
            {
                TotalCasts += calculationOptions.PWSCast;
                ActionList += String.Format("\r\n- {0} Power Word: Shield", calculationOptions.PWSCast);
                ManaUsed += pws.ManaCost * calculationOptions.PWSCast;
                TimeUsed += pws.GlobalCooldown * calculationOptions.PWSCast;
                BaseTimeUsed += 1.5f * calculationOptions.PWSCast;
                AbsorbHeal += pws.AvgTotHeal * calculationOptions.PWSCast;
                if (character.PriestTalents.GlyphofPowerWordShield)
                {
                    float pwsglyphheal = pws.AvgTotHeal * 0.2f * healmultiplier * 0.2f;
                    OtherHeal += pwsglyphheal * (1f - simstats.SpellCrit) + pwsglyphheal * 1.5f * simstats.SpellCrit;
                    CritCounter += simstats.SpellCrit * calculationOptions.PWSCast;
                }
            }

            // Circle of Healing
            if (calculationOptions.CoHCast > 0)
            {
                TotalCasts += calculationOptions.CoHCast;
                ActionList += String.Format("\r\n- {0} Circle of Healing", calculationOptions.CoHCast);
                ManaUsed += coh_max.ManaCost * calculationOptions.CoHCast;
                TimeUsed += coh_max.GlobalCooldown * calculationOptions.CoHCast;
                BaseTimeUsed += 1.5f * calculationOptions.CoHCast;
                OtherHeal += coh_max.AvgTotHeal * calculationOptions.CoHCast;
                CritCounter += coh_max.CritChance * coh_max.Targets * calculationOptions.CoHCast;
            }

            // Holy Nova
            if (calculationOptions.HolyNovaCast > 0)
            {
                TotalCasts += calculationOptions.HolyNovaCast;
                ActionList += String.Format("\r\n- {0} Holy Nova", calculationOptions.HolyNovaCast);
                ManaUsed += hn.ManaCost * calculationOptions.HolyNovaCast;
                TimeUsed += hn.GlobalCooldown * calculationOptions.HolyNovaCast;
                BaseTimeUsed += 1.5f * calculationOptions.HolyNovaCast;
                DirectHeal += hn.AvgTotHeal * healmultiplier * calculationOptions.HolyNovaCast;
                AbsorbHeal += hn.AvgCrit * hn.CritChance * healmultiplier * calculationOptions.HolyNovaCast * divineaegis;
                CritCounter += hn.CritChance * 5 * calculationOptions.HolyNovaCast;
            }

            // Divine Hymn
            if (calculationOptions.DivineHymnCast > 0)
            {
                TotalCasts += calculationOptions.DivineHymnCast;
                ActionList += String.Format("\r\n- {0} Divine Hymn", calculationOptions.DivineHymnCast);
                ManaUsed += dh.ManaCost * calculationOptions.DivineHymnCast;
                TimeUsed += dh.CastTime * calculationOptions.DivineHymnCast;
                BaseTimeUsed += dh.BaseCastTime * calculationOptions.DivineHymnCast;
                DirectHeal += dh.AvgTotHeal * healmultiplier * calculationOptions.DivineHymnCast;
                AbsorbHeal += dh.AvgCrit * 3 * dh.CritChance * healmultiplier * calculationOptions.DivineHymnCast * divineaegis;
                CritCounter += dh.CritChance * 8/2*3 * calculationOptions.DivineHymnCast; // 12 total heals from Divine Hymn
            }

            // Dispel
            if (calculationOptions.DispelCast > 0)
            {
                TotalCasts += calculationOptions.DispelCast;
                ActionList += String.Format("\r\n- {0} Dispel", calculationOptions.DispelCast);
                ManaUsed += dispel.ManaCost * calculationOptions.DispelCast;
                TimeUsed += dispel.GlobalCooldown * calculationOptions.DispelCast;
                BaseTimeUsed += 1.5f * calculationOptions.DispelCast;
            }

            // Mass Dispel
            if (calculationOptions.MDCast > 0)
            {
                TotalCasts += calculationOptions.MDCast;
                ActionList += String.Format("\r\n- {0} Mass Dispel", calculationOptions.MDCast);
                ManaUsed += md.ManaCost * calculationOptions.MDCast;
                TimeUsed += md.CastTime * calculationOptions.MDCast;
                BaseTimeUsed += md.BaseCastTime * calculationOptions.MDCast;
            }

            ActionList += String.Format("\r\n- {0} Spells Cast", TotalCasts);

            if (TimeUsed > calculationOptions.FightLengthSeconds)
            {
                ActionList += "\r\n\r\nWARNING:\r\nFight Length is less than time needed to perform the actions listed!";
                calculatedStats.HPSBurstPoints = -1;
                calculatedStats.HPSSustainPoints = -1;
            }
            else
            {
                float mp1use = ManaUsed / calculationOptions.FightLengthSeconds;

                float periodicRegenOutFSR = StatConversion.GetSpiritRegenSec(simstats.Spirit, simstats.Intellect);

                // Add up all mana gains.
                float regen = 0, tmpregen = 0;

                // Spirit/Intellect based Regeneration and MP5
                tmpregen = periodicRegenOutFSR * (1f - calculationOptions.FSRRatio / 100f);
                if (tmpregen > 0f)
                {
                    ManaSources.Add(new ManaSource("OutFSR", tmpregen));
                    regen += tmpregen;
                }
                tmpregen = periodicRegenOutFSR * simstats.SpellCombatManaRegeneration * calculationOptions.FSRRatio / 100f;
                if (tmpregen > 0f)
                {
                    ManaSources.Add(new ManaSource("Meditation", tmpregen));
                    regen += tmpregen;
                }
                float holyconccast = calculationOptions.RenewCast + calculationOptions.FlashHealCast + calculationOptions.GreaterHealCast + calculationOptions.BindingHealCast * 2;
                if (character.PriestTalents.HolyConcentration > 0 && holyconccast > 0)
                {
                    float hceffect = character.PriestTalents.HolyConcentration * 0.5f / 3f;
                    float hccastinterval = calculationOptions.FightLengthSeconds / holyconccast;
                    float hccritchance = HCCritCounter / holyconccast;
                    // Calculate chance that you crit within 8 seconds.
                    float hcuptime = 1f - (float)Math.Pow(1f - hccritchance, 8f / hccastinterval);
                    tmpregen = (periodicRegenOutFSR * (1f - calculationOptions.FSRRatio / 100f) 
                        + periodicRegenOutFSR * simstats.SpellCombatManaRegeneration * calculationOptions.FSRRatio / 100f)
                        * hcuptime * hceffect;
                    if (tmpregen > 0)
                    {
                        ManaSources.Add(new ManaSource("Holy Concentration", tmpregen));
                        regen += tmpregen;
                    }
                }
                if (character.PriestTalents.Rapture > 0 && calculationOptions.PWSCast > 0)
                {   // New Rapture restores 1.5% - 2% - 2.5% of max mana.
                    float rapturereturn = 0.015f + (character.PriestTalents.Rapture - 1) * 0.005f;
                    float timebetweenshields = calculationOptions.FightLengthSeconds / calculationOptions.PWSCast;
                    float maxrapture = simstats.Mana * rapturereturn / timebetweenshields;
                    tmpregen = maxrapture * calculationOptions.Rapture / 100f;
                    if (tmpregen > 0)
                    {
                        ManaSources.Add(new ManaSource("Rapture", tmpregen));
                        regen += tmpregen;
                    }
                }

                tmpregen = simstats.Mp5 / 5;
                ManaSources.Add(new ManaSource("MP5", tmpregen));
                regen += tmpregen;
                tmpregen = simstats.Mana / (calculationOptions.FightLengthSeconds);
                ManaSources.Add(new ManaSource("Intellect", tmpregen));
                regen += tmpregen;

                if (calculationOptions.ModelProcs)
                {
                    float heal = 0f;
                    foreach (SpecialEffect se in simstats.SpecialEffects())
                    {
                        tmpregen = 0f;
                        if (se.Stats.ManaRestore > 0 || se.Stats.Mp5 > 0)
                        {
                            if (se.Trigger == Trigger.SpellCast
                                || se.Trigger == Trigger.HealingSpellCast
                                || se.Trigger == Trigger.HealingSpellHit)
                            {
                                tmpregen = se.GetAverageStats(calculationOptions.FightLengthSeconds / TotalCasts, 1f, 0f, calculationOptions.FightLengthSeconds).ManaRestore
                                    + se.GetAverageStats(calculationOptions.FightLengthSeconds / TotalCasts, 1f, 0f, calculationOptions.FightLengthSeconds).Mp5 / 5;
                            }
                            else if (se.Trigger == Trigger.SpellCrit
                                || se.Trigger == Trigger.HealingSpellCrit)
                            {
                                tmpregen = se.GetAverageStats(calculationOptions.FightLengthSeconds / TotalCasts, CritCounter / TotalCasts, 0f, calculationOptions.FightLengthSeconds).ManaRestore
                                    + se.GetAverageStats(calculationOptions.FightLengthSeconds / TotalCasts, CritCounter / TotalCasts, 0f, calculationOptions.FightLengthSeconds).Mp5 / 5;
                            }
                            else if (se.Trigger == Trigger.Use)
                            {
                                tmpregen = se.GetAverageStats().ManaRestore
                                    + se.GetAverageStats().Mp5 / 5;

                            }
                        }
                        if (tmpregen > 0)
                        {
                            ManaSources.Add(new ManaSource(se.ToString(), tmpregen));
                            regen += tmpregen;
                        }
                        if (se.Stats.Healed > 0f)
                        {
                            if (se.Trigger == Trigger.HealingSpellCast
                                || se.Trigger == Trigger.SpellCast
                                || se.Trigger == Trigger.HealingSpellHit)
                                heal += se.GetAverageStats(calculationOptions.FightLengthSeconds / TotalCasts, 1f, 0f, calculationOptions.FightLengthSeconds).Healed;
                            else if (se.Trigger == Trigger.SpellCrit
                                || se.Trigger == Trigger.HealingSpellCrit)
                                heal += se.GetAverageStats(calculationOptions.FightLengthSeconds / TotalCasts, CritCounter / TotalCasts, 0f, calculationOptions.FightLengthSeconds).Healed;
                            else if (se.Trigger == Trigger.Use)
                                heal += se.GetAverageStats().Healed;
                        }
                    }
                    heal *= (1f + stats.SpellCrit * 0.5f) * healmultiplier;
                    DirectHeal += heal;
                    AbsorbHeal += heal * valanyrProc;
                    #region old procs
                    /*
                    // TODO: Trinkets here.
                    if (simstats.ManaRestoreOnCrit_25_45 > 0)
                    {   // X mana back every 25%*critchance spell every 45seconds.
                        float avgcritcast = CritCounter / TotalCasts;
                        tmpregen = simstats.ManaRestoreOnCrit_25_45 / ProcInterval(0.25f * avgcritcast, avgcritcast, 45f);
                        if (tmpregen > 0f)
                        {
                            ManaSources.Add(new ManaSource("Soul of the Dead", tmpregen));
                            regen += tmpregen;
                        }
                    }
                    if (simstats.ManaRestoreOnCast_10_45 > 0)
                    {
                        tmpregen = simstats.ManaRestoreOnCast_10_45 / ProcInterval(0.1f, TimeUsed / TotalCasts, 45f);
                        if (tmpregen > 0f)
                        {
                            if (simstats.ManaRestoreOnCast_10_45 == 300)
                                ManaSources.Add(new ManaSource("Je'Tze's Bell", tmpregen));
                            else if (simstats.ManaRestoreOnCast_10_45 == 528)
                                ManaSources.Add(new ManaSource("Spark of Life", tmpregen));
                            else if (simstats.ManaRestoreOnCast_10_45 == 228)
                                ManaSources.Add(new ManaSource("Memento of Tyrande", tmpregen));
                            else
                                ManaSources.Add(new ManaSource("MP5 Proc Trinket", tmpregen));
                            regen += tmpregen;
                        }
                    }*/
                    #endregion
                }

                // External and Other mana sources.
                tmpregen = simstats.Mana * simstats.ManaRestoreFromMaxManaPerSecond * calculationOptions.Replenishment / 100f;
                if (tmpregen > 0f)
                {
                    ManaSources.Add(new ManaSource("Replenishment", tmpregen));
                    regen += tmpregen;
                }

                ActionList += "\r\n\r\nMana Options:";

                if (mp1use > regen && character.Race == CharacterRace.BloodElf)
                {   // Arcane Torrent is 6% max mana every 2 minutes.
                    tmpregen = simstats.Mana * 0.06f / 120f;
                    ManaSources.Add(new ManaSource("Arcane Torrent", tmpregen));
                    regen += tmpregen;
                    ActionList += string.Format("\r\n- Used Arcane Torrent");
                }

                if (mp1use > regen && calculationOptions.ManaAmt > 0f)
                {
                    float ManaPot = calculationOptions.ManaAmt * (1f + simstats.BonusManaPotion);
                    tmpregen = ManaPot / (calculationOptions.FightLengthSeconds);
                    ManaSources.Add(new ManaSource("Mana Potion", tmpregen));
                    ActionList += string.Format("\r\n- Used Mana Potion ({0})", ManaPot.ToString("0"));
                    regen += tmpregen;
                }
                if (mp1use > regen)
                {
                    tmpregen = (simstats.Mana * 0.4f * calculationOptions.Shadowfiend / 100f)
                        / ((5f - character.PriestTalents.VeiledShadows * 1f) * 60f);
                    ManaSources.Add(new ManaSource("Shadowfiend", tmpregen));
                    ActionList += string.Format("\r\n- Used Shadowfiend");
                    regen += tmpregen;
                }
                if (mp1use > regen)
                {   // 20% increased mana, 3% restored for 4 ticks unless Glyphed. Then its 5.
                    float ticks = character.PriestTalents.GlyphofHymnofHope ? 5 : 4;
                    tmpregen = (simstats.Mana * 1.2f * 0.03f * ticks)
                        / (6f * 60f);
                    ManaSources.Add(new ManaSource("Hymn of Hope", tmpregen));
                    ActionList += string.Format("\r\n- Used Hymn of Hope");
                    regen += tmpregen;
                }
                if (mp1use > regen)
                {
                    ActionList += string.Format("\r\n- {0} mp5 deficit. Could use more mana!", ((mp1use - regen) * 5).ToString("0"));
                }

                calculatedStats.HPSBurstPoints = (DirectHeal + AbsorbHeal + OtherHeal) / TimeUsed;
                calculatedStats.HPSSustainPoints = (DirectHeal + AbsorbHeal + OtherHeal) / calculationOptions.FightLengthSeconds;
                if (regen < mp1use)
                    calculatedStats.HPSSustainPoints *= regen / mp1use;
                /*if (regen > mp1use)
                    calculatedStats.HPSSustainPoints = calculatedStats.HPSBurstPoints;
                else
                    calculatedStats.HPSSustainPoints = calculatedStats.HPSBurstPoints * regen / mp1use;*/
            }

            // Lets just say that 15% of resilience scales all health by 150%.
            float Resilience = (float)Math.Min(15f, StatConversion.GetCritReductionFromResilience(simstats.Resilience) * 100f) / 15f;
            calculatedStats.SurvivabilityPoints = calculatedStats.BasicStats.Health * (Resilience * 1.5f + 1f) * calculationOptions.Survivability / 100f;
        }
    }
}
