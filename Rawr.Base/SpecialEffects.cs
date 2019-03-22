using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Rawr {
    public static class SpecialEffects {
        public static void ProcessMetaGem(string line, Stats stats, bool bisArmory) {
            List<string> gemBonuses = new List<string>();
            string[] gemBonusStrings = line.Split(new string[] { " and ", " & ", ", " }, StringSplitOptions.None);
            foreach (string gemBonusString in gemBonusStrings) {
                if (gemBonusString.IndexOf('+') != gemBonusString.LastIndexOf('+')) {
                    gemBonuses.Add(gemBonusString.Substring(0, gemBonusString.IndexOf(" +")));
                    gemBonuses.Add(gemBonusString.Substring(gemBonusString.IndexOf(" +") + 1));
                } else{ gemBonuses.Add(gemBonusString); }
            }
            foreach (string gemBonus in gemBonuses) {
                if (gemBonus == "Spell Damage +6")
                {
                    stats.SpellPower = 6.0f;
                }
                else if (gemBonus == "Stamina +6")
                {
                    stats.Stamina = 6.0f;
                }
                else if (gemBonus == "Chance to restore mana on spellcast")
                {
                    stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast, new Stats() { ManaRestore = 600 }, 0f, 15f, .05f));
                    stats.ManaRestoreOnCast_5_15 = 600; // IED
                }
                else if (gemBonus == "2% Increased Armor Value from Items")
                {
                    stats.BaseArmorMultiplier = 0.02f; // IED
                }
                else if (gemBonus == "Chance on spellcast - next spell cast in half time" || gemBonus == "Chance to Increase Spell Cast Speed")
                {
                    stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast, new Stats() { HasteRating = 320 }, 6, 45, 0.15f));
                }
                else if (gemBonus == "+5% Shield Block Value")
                {
                    stats.BonusBlockValueMultiplier = 0.05f;
                }
                else if (gemBonus == "+2% Intellect")
                {
                    stats.BonusIntellectMultiplier = 0.02f;
                }
                else if (gemBonus == "+2% Mana")
                {
                    stats.BonusManaMultiplier = 0.02f;
                }
                else if (gemBonus.Contains("Reduce Spell Damage Taken by "))
                {
                    int bonus = int.Parse(gemBonus.Substring(gemBonus.Length - 3, 2));
                    stats.SpellDamageTakenMultiplier = (float)bonus / -100f;
                }
                else if (gemBonus == "3% Increased Critical Damage")
                {
                    stats.BonusCritMultiplier = 0.03f;
                    stats.BonusSpellCritMultiplier = 0.03f;
                }
                else if (gemBonus == "3% Increased Critical Healing Effect")
                {
                    stats.BonusCritHealMultiplier = 0.03f;
                }
                else if (gemBonus == "Minor Run Speed Increase")
                {
                    stats.MovementSpeed = 0.08f;
                }
                else if (gemBonus.Contains("Stun Duration Reduced by "))
                {
                    int bonus = int.Parse(gemBonus.Substring(gemBonus.Length - 3, 2));
                    stats.StunDurReduc = (float)bonus / 100f;
                }
                else if (gemBonus.Contains("Reduces Snare/Root Duration by "))
                {
                    int bonus = int.Parse(gemBonus.Substring(gemBonus.Length - 3, 2));
                    stats.SnareRootDurReduc = (float)bonus / 100f;
                }
                else if (gemBonus.Contains("Fear Duration Reduced by "))
                {
                    int bonus = int.Parse(gemBonus.Substring(gemBonus.Length - 3, 2));
                    stats.FearDurReduc = (float)bonus / 100f;
                }
                else if (gemBonus.Contains("Chance to Increase Melee/Ranged Attack Speed"))
                {
                    // 480 Haste Rating on 100% Chance to proc every 60 seconds for 6 seconds
                    stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageDone, new Stats() { HasteRating = 480f, }, 6f, 60f));
                }
                else if (gemBonus.Contains("% Spell Reflect"))
                {
                    int bonus = int.Parse(gemBonus.Substring(0, 2).Trim('%'));
                    stats.SpellReflectChance = (float)bonus / 100f;
                }
                else if (gemBonus == "Sometimes Heal on Your Crits")
                {
                    // this is supposed to be 2% of your total health healed when it procs, 50% chance to proc on crit
                    stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalCrit, new Stats() { HealthRestoreFromMaxHealth = 0.02f }, 0f, 0f, 0.50f));
                    stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCrit   , new Stats() { HealthRestoreFromMaxHealth = 0.02f }, 0f, 0f, 0.50f));
                }
                else if (gemBonus == "2% Reduced Threat")
                {
                    stats.ThreatReductionMultiplier = 0.02f;
                }
                else
                {
                    try
                    {
                        int gemBonusValue = int.Parse(gemBonus.Substring(0, gemBonus.IndexOf(' ')).Trim('+').Trim('%'));
                        switch (gemBonus.Substring(gemBonus.IndexOf(' ') + 1).Trim())
                        {
                            case "Resist All":
                                stats.ArcaneResistance = gemBonusValue;
                                stats.FireResistance = gemBonusValue;
                                stats.FrostResistance = gemBonusValue;
                                stats.NatureResistance = gemBonusValue;
                                stats.ShadowResistance = gemBonusValue;
                                break;
                            case "Increased Critical Damage":
                                stats.BonusCritMultiplier = (float)gemBonusValue / 100f;
                                stats.BonusSpellCritMultiplier = (float)gemBonusValue / 100f; // both melee and spell crit use the same text, would have to disambiguate based on id
                                break;
                            case "Agility":
                                stats.Agility = gemBonusValue;
                                break;
                            case "Stamina":
                                stats.Stamina = gemBonusValue;
                                break;
                            case "Dodge Rating":
                                stats.DodgeRating = gemBonusValue;
                                break;
                            case "Parry Rating":
                                stats.ParryRating = gemBonusValue;
                                break;
                            case "Block Rating":
                                stats.BlockRating = gemBonusValue;
                                break;
                            case "Defense Rating":
                                stats.DefenseRating = gemBonusValue;
                                break;
                            case "Hit Rating":
                                stats.HitRating = gemBonusValue;
                                break;
                            case "Haste Rating":
                                stats.HasteRating = gemBonusValue;
                                break;
                            case "Expertise Rating":
                                stats.ExpertiseRating = gemBonusValue;
                                break;
                            case "Armor Penetration Rating":
                                stats.ArmorPenetrationRating = gemBonusValue;
                                break;
                            case "Strength":
                                stats.Strength = gemBonusValue;
                                break;
                            case "Crit Rating":
                            case "Crit Strike Rating":
                            case "Critical Rating":
                            case "Critical Strike Rating":
                                stats.CritRating = gemBonusValue;
                                break;
                            case "Attack Power":
                                stats.AttackPower = gemBonusValue;
                                break;
                            case "Weapon Damage":
                                stats.WeaponDamage = gemBonusValue;
                                break;
                            case "Resilience":
                            case "Resilience Rating":
                                stats.Resilience = gemBonusValue;
                                break;
                            case "Spell Hit Rating":
                                stats.HitRating = gemBonusValue;
                                break;
                            case "Spell Haste Rating":
                                stats.HasteRating = gemBonusValue;
                                break;
                            case "Spell Damage":
                                // Ignore spell damage from gem if Healing has already been applied, as it might be a "9 Healing 3 Spell" gem. 
                                if (stats.SpellPower == 0)
                                    stats.SpellPower = gemBonusValue;
                                break;
                            case "Spell Damage and Healing":
                                stats.SpellPower = gemBonusValue;
                                break;
                            case "Healing":
                                stats.SpellPower = (float)Math.Round(gemBonusValue / 1.88f);
                                break;
                            case "Spell Power":
                                stats.SpellPower = gemBonusValue;
                                break;
                            case "Spell Crit":
                            case "Spell Crit Rating":
                            case "Spell Critical":
                            case "Spell Critical Rating":
                                stats.CritRating = gemBonusValue;
                                break;
                            case "Mana every 5 seconds":
                            case "Mana ever 5 Sec":
                            case "mana per 5 sec":
                            case "mana per 5 sec.":
                            case "Mana per 5 Seconds":
                                stats.Mp5 = gemBonusValue;
                                break;
                            case "Intellect":
                                stats.Intellect = gemBonusValue;
                                break;
                            case "Spirit":
                                stats.Spirit = gemBonusValue;
                                break;
                        }
                    }
                    catch { }
                }
            }
        }

        public static void ProcessEquipLine(string line, Stats stats, bool isArmory, int ilvl, int id) {
            Match match;
            #region Prep the line, if it needs it
            while (line.Contains("secs")) { line = line.Replace("secs", "sec"); }
            while (line.Contains("sec.")) { line = line.Replace("sec.", "sec"); }
            while (line.Contains("  ")) { line = line.Replace("  ", " "); }
            while (line.EndsWith(".")) { line = line.Substring(0, line.Length-1); }
            #endregion
            if (false) { /*Never run, this is just to make all the stuff below uniform with an 'else if' line start*/ }
            #region Class Specific
            #region Added by Druid: Bear/Cat
            else if ((match = new Regex(@"Your Mangle ability also grants you (?<amount>\d\d*) attack power for 10 sec").Match(line)).Success)
            {   // Gladiator's Idol of Resolve
                stats.AddSpecialEffect(new SpecialEffect(Trigger.MangleCatHit, new Stats() { AttackPower = (float)int.Parse(match.Groups["amount"].Value), }, 10, 0));
            }
            else if ((match = new Regex(@"While in Bear Form, your Lacerate and Swipe abilities have a chance to grant (?<amount1>\d\d*) dodge rating for (?<dur1>\d\d*) sec, and your Cat Form's Mangle and Shred abilities have a chance to grant (?<amount2>\d\d*) Agility for (?<dur2>\d\d*) sec").Match(line)).Success)
            {   // Idol of Mutilation
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SwipeBearOrLacerateHit, new Stats() { DodgeRating = (float)int.Parse(match.Groups["amount1"].Value) }, (float)int.Parse(match.Groups["dur1"].Value), 8f, .65f));
                stats.AddSpecialEffect(new SpecialEffect(Trigger.MangleCatOrShredHit, new Stats() { Agility = (float)int.Parse(match.Groups["amount2"].Value) }, (float)int.Parse(match.Groups["dur2"].Value), 8f, .85f));
            }
            else if ((match = new Regex(@"Increases periodic damage done by Rip by (?<amount>\d\d*) per combo point").Match(line)).Success)
            {   // Idol of Worship
                stats.BonusRipDamagePerCPPerTick += (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Your Mangle ability has a chance to grant (?<amount>\d\d*) agility for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Idol of the Corruptor | Idol of Terror
                float Dur = (float)int.Parse(match.Groups["dur"].Value);
                if (Dur == 10) { // Terror
                    stats.AddSpecialEffect(new SpecialEffect(Trigger.MangleCatHit, new Stats() { Agility = (float)int.Parse(match.Groups["amount"].Value), }, Dur, 0f, 0.65f));
                    stats.AddSpecialEffect(new SpecialEffect(Trigger.MangleBearHit, new Stats() { Agility = (float)int.Parse(match.Groups["amount"].Value), }, Dur, 0f, 0.45f));
                } else { // Corruptor
                    stats.AddSpecialEffect(new SpecialEffect(Trigger.MangleCatHit, new Stats() { Agility = (float)int.Parse(match.Groups["amount"].Value), }, Dur, 0f, 1f));
                    stats.AddSpecialEffect(new SpecialEffect(Trigger.MangleBearHit, new Stats() { Agility = (float)int.Parse(match.Groups["amount"].Value), }, Dur, 0f, 0.7f));
                }
            }
            else if ((match = new Regex(@"The periodic damage from your Lacerate and Rake abilities grants (?<amount>\d\d*) Agility for (?<dur>\d\d*) sec Stacks up to (?<stack>\d\d*) times").Match(line)).Success)
            {   // Idol of the Crying Moon
                stats.AddSpecialEffect(new SpecialEffect(Trigger.LacerateTick, new Stats() { Agility = (float)int.Parse(match.Groups["amount"].Value), }, (float)int.Parse(match.Groups["dur"].Value), 0f, 1f, int.Parse(match.Groups["stack"].Value)));
                stats.AddSpecialEffect(new SpecialEffect(Trigger.RakeTick, new Stats() { Agility = (float)int.Parse(match.Groups["amount"].Value), }, (float)int.Parse(match.Groups["dur"].Value), 0f, 1f, int.Parse(match.Groups["stack"].Value)));
            }
            else if ((match = new Regex(@"Increases the damage dealt by Shred by (?<amount>\d\d*)").Match(line)).Success)
            {   // Idol of the Ravenous Beast
                stats.BonusShredDamage += int.Parse(match.Groups["amount"].Value);
            }
            #endregion
            #region Added by Druid: Tree
            else if ((match = new Regex(@"Increases the spell power of the final healing value of your Lifebloom by (?<amount>\d\d*)").Match(line)).Success)
            {   // Gladiator's Idol of Tenacity
                // Note: This may need to go to another stat or drop this stat's value
                stats.LifebloomTickHealBonus += (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Reduces the mana cost of Rejuvenation by (?<amount>\d\d*)").Match(line)).Success)
            {   // Idol of Awakening
                stats.ReduceRejuvenationCost += (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Increases the spell power on the periodic portion of your Lifebloom by (?<amount>\d\d*)").Match(line)).Success)
            {   // Idol of Lush Mosh
                stats.LifebloomTickHealBonus += (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Each time your Rejuvenation spell deals periodic healing, you have a chance to gain (?<amount>\d+) spell power for (?<dur>\d+) sec").Match(line)).Success)
            {   // Idol of Flaring Growth
                // Not yet sure about cooldown and proc chance
                stats.AddSpecialEffect(new SpecialEffect(Trigger.RejuvenationTick,
                    new Stats() { SpellPower = int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 0, 0.7f));
            }
            else if ((match = new Regex(@"The periodic healing from your Rejuvenation spell grants (?<amount>\d+) spell power for (?<dur>\d+) sec Stacks up to (?<stacks>\d+) times").Match(line)).Success)
            {   // Idol of the Black Willow
                stats.AddSpecialEffect(new SpecialEffect(Trigger.RejuvenationTick,
                    new Stats() { SpellPower = int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 0, 1f, int.Parse(match.Groups["stacks"].Value)));
            }
            else if ((match = new Regex(@"Increases the spell power of your Nourish by (?<amount>\d+)").Match(line)).Success)
            {   // Idol of the Flourishing Life
                stats.NourishSpellpower += int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Increases the periodic healing of Rejuvenation by (?<amount>\d+)").Match(line)).Success)
            {
                // Idol of Pure Thoughts
                stats.RejuvenationHealBonus += int.Parse(match.Groups["amount"].Value);
            }
            #endregion
            #region Added by Druid: Moonkin
            else if ((match = new Regex(@"The periodic damage from your Insect Swarm and Moonfire spells grants (?<amount>\d\d*) critical strike rating for (?<dur>\d\d*) sec Stacks up to (?<stacks>\d\d*) times").Match(line)).Success)
            {   // Idol of the Lunar Eclipse
                stats.AddSpecialEffect(new SpecialEffect(Trigger.InsectSwarmOrMoonfireTick,
                    new Stats() { CritRating = (float)int.Parse(match.Groups["amount"].Value), },
                    (float)int.Parse(match.Groups["dur"].Value), 0, 1f, int.Parse(match.Groups["stacks"].Value)));
            }
            else if ((match = new Regex(@"Each time your Moonfire spell deals periodic damage, you have a chance to gain (?<amount>\d\d*) critical strike rating for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Idol of Lunar Fury
                stats.AddSpecialEffect(new SpecialEffect(Trigger.MoonfireTick,
                    new Stats() { CritRating = (float)int.Parse(match.Groups["amount"].Value), },
                    (float)int.Parse(match.Groups["dur"].Value), 0f, 0.7f));
            }
            else if ((match = new Regex(@"Increases the damage dealt by Wrath by (?<amount>\d\d*)").Match(line)).Success)
            {   // Idol of Steadfast Renewal
                stats.WrathDmg += (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Increases the spell power of your Insect Swarm by (?<amount>\d\d*)").Match(line)).Success)
            {   // Idol of the Crying Wind
                stats.InsectSwarmDmg += (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Increases the spell power of your Starfire spell by (?<amount>\d\d*)").Match(line)).Success)
            {   // Idol of the Shooting Star
                stats.StarfireDmg += (float)int.Parse(match.Groups["amount"].Value);
            }
            // Other procs to process (Unknown sources)
            else if ((match = new Regex(@"Increases the spell power of your Moonfire spell by (?<amount>\d\d*)").Match(line)).Success)
            {
                stats.MoonfireDmg += (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Your Moonfire spell grants (?<amount>\d\d*) spell power for 10 sec").Match(line)).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.MoonfireCast,
                    new Stats() { SpellPower = (float)int.Parse(match.Groups["amount"].Value), }, 10f, 0f));
            }
            else if (line.StartsWith("Your Moonfire ability has a chance to grant "))
            {
                line = line.Substring("Your Moonfire ability has a chance to grant ".Length);
                string spellPowerLine = line.Substring(0, line.IndexOf(" spell power for"));
                string durationLine = line.Substring(line.IndexOf("for") + 3, line.IndexOf(" sec") - line.IndexOf("for") - 3);
                float spellPower = float.Parse(spellPowerLine, System.Globalization.CultureInfo.InvariantCulture);
                float duration = float.Parse(durationLine, System.Globalization.CultureInfo.InvariantCulture);
                stats.AddSpecialEffect(new SpecialEffect(Trigger.MoonfireCast, new Stats() { SpellPower = spellPower, }, duration, 0f, 0.5f));
            }
            #endregion
            #region Added by DK: Tank/DPS
            // Note: Sigils code can be found at the bottom of this else statement
            #endregion
            #region Added by Shaman: Enhance/Elemental/Resto
            else if ((match = new Regex(@"Your Lava Lash ability also grants you (?<amount>\d\d*) attack power for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Gladiator's Totem of Indomitability
                stats.AddSpecialEffect(new SpecialEffect(Trigger.ShamanLavaLash,
                    new Stats() { AttackPower = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0f));
            }
            else if ((match = new Regex(@"Your Shock spells grant (?<amount>\d\d*) spellpower for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Gladiator's Totem of Survival
                stats.AddSpecialEffect(new SpecialEffect(Trigger.ShamanShock,
                    new Stats() { SpellPower = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0f));
            }
            else if ((match = new Regex(@"Increases spell power of Lesser Healing Wave by (?<amount>\d\d*)").Match(line)).Success)
            {   // Gladiator's Totem of the Third Wind | Totem of the Plains
                stats.TotemLHWSpellpower = (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"The periodic damage from your Flame Shock spell grants (?<amount>\d\d*) haste rating for (?<dur>\d\d*) sec Stacks up to (?<stacks>\d\d*) times").Match(line)).Success)
            {   // Bizuri's Totem of Shattered Ice
                stats.AddSpecialEffect(new SpecialEffect(Trigger.ShamanFlameShockDoTTick,
                    new Stats() { HasteRating = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0f, 1f, int.Parse(match.Groups["stacks"].Value)));
            }
            else if ((match = new Regex(@"Increases the base amount healed by your chain heal by (?<amount>\d\d*)").Match(line)).Success)
            {   // Steamcaller's Totem | Totem of the Bay
                stats.TotemCHBaseHeal = (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Increases the base damage of your Lava Burst by (?<amount>\d\d*)").Match(line)).Success)
            {   // Thunderfall Totem
                stats.LavaBurstBonus = (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Each time you cast Chain Heal, you have a chance to gain (?<amount>\d\d*) spell power for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Totem of Calming Tides
                stats.RestoShamRelicT9 = (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Your Storm Strike ability also grants you (?<amount>\d\d*) haste rating for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Totem of Dueling
                stats.AddSpecialEffect(new SpecialEffect(Trigger.ShamanStormStrike,
                    new Stats() { HasteRating = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0f));
            }
            else if ((match = new Regex(@"Each time you cast Lightning Bolt, you have a chance to gain (?<amount>\d\d*) haste rating for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Totem of Electrifying Wind
                stats.AddSpecialEffect(new SpecialEffect(Trigger.ShamanLightningBolt,
                    new Stats() { HasteRating = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0f, 0.70f));
            }
            else if ((match = new Regex(@"Reduces the base mana cost of Chain Heal by (?<amount>\d\d*)").Match(line)).Success)
            {   // Totem of Forest Growth
                stats.TotemCHBaseCost = (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Increases spell power of Chain Lightning and Lightning Bolt by (?<amount>\d\d*)").Match(line)).Success)
            {   // Totem of Hex
                stats.LightningSpellPower = (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Reduces the mana cost of Healing Wave by (?<amount>\d\d*)").Match(line)).Success)
            {   // Totem of Misery
                stats.TotemHWBaseCost = (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Each time you use your Lava Lash ability, you have a chance to gain (?<amount>\d\d*) attack power for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Totem of Quaking Earth
                stats.AddSpecialEffect(new SpecialEffect(Trigger.ShamanLavaLash,
                    new Stats() { AttackPower = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 9f, 0.80f));
            }
            else if ((match = new Regex(@"Increases the attack power bonus on Windfury Weapon attacks by (?<amount>\d\d*)").Match(line)).Success)
            {   // Totem of Astral Winds & Totem of Splintering
                stats.BonusWFAttackPower = (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Your Stormstrike ability grants (?<amount>\d\d*) attack power for (?<dur>\d\d*) sec Stacks up to (?<stacks>\d\d*) times").Match(line)).Success)
            {   // Totem of the Avalanche
                stats.AddSpecialEffect(new SpecialEffect(Trigger.ShamanStormStrike,
                    new Stats() { AttackPower = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0f, 1f, int.Parse(match.Groups["stacks"].Value)));
            }
            else if ((match = new Regex(@"Increases weapon damage when you use Stormstrike by (?<amount>\d\d*)").Match(line)).Success)
            {   // Totem of the Dancing Flame
                stats.BonusSSDamage += (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Your Lightning Bolt spell has a chance to grant (?<amount>\d\d*) haste rating for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Totem of the Elemental Plane
                //stats.LightningBoltHasteProc_15_45 += (float)int.Parse(match.Groups["amount"].Value);
                stats.AddSpecialEffect(new SpecialEffect(Trigger.ShamanLightningBolt,
                    new Stats() { HasteRating = int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 45f, 0.15f));
            }
            else if ((match = new Regex(@"Your Riptide spell grants (?<amount>\d\d*) spell power for (?<dur>\d\d*) sec Stacks up to (?<stacks>\d\d*) times").Match(line)).Success)
            {   // Totem of the Surging Sea
                // This needs to be remodeled as a SpecialEffect
                stats.RestoShamRelicT10 = int.Parse(match.Groups["amount"].Value) * int.Parse(match.Groups["stacks"].Value);
            }
            // Other
            else if (line.StartsWith("Increases spell power of Healing Wave by "))
            {   // Totem of Spontaneous Regrowth
                line = line.Replace(".", "");
                line = line.Substring("Increases spell power of Healing Wave by ".Length);
                stats.TotemHWSpellpower = float.Parse(line);
            }
            else if (line.StartsWith("Your Water Shield ability grants an additional "))
            {   // Totem of the Thunderhead, Possible Future totems
                stats.TotemThunderhead = 1f;
            }
            else if (line.StartsWith("Increases the damage dealt by your Lava Burst by "))
            {
                line = line.Replace(".", "");
                line = line.Substring("Increases the damage dealt by your Lava Burst by ".Length);
                stats.LavaBurstBonus = float.Parse(line);
            }
            else if (line == "Your Shock spells have a chance to grant 110 attack power for 10 sec")
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.ShamanShock, new Stats() { AttackPower = 110 }, 10f, 45f));
            }
            #endregion
            #region Added by Priest: HealPriest
            #endregion
            #region Added by Priest: Shadow
            #endregion
            #region Added by Warrior: ProtWarr
            #endregion
            #region Added by Warrior: DPSWarr
            #endregion
            #region Added by Paladin: Healadin
            else if ((match = new Regex(@"Each time you cast Holy Light, you have a chance to gain (?<amount>\d\d*) spell power for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Libram of Veracity
                stats.AddSpecialEffect(new SpecialEffect(Trigger.HolyLightCast,
                    new Stats() { SpellPower = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 10f, 0.70f));
            }
            else if ((match = new Regex(@"Your Holy Shock heals grant (?<amount>\d\d*) spell power for (?<dur>\d\d*) sec Stacks up to (?<stacks>\d\d*) times").Match(line)).Success)
            {   // Libram of Blinding Light
                stats.AddSpecialEffect(new SpecialEffect(Trigger.HolyShockCast,
                    new Stats() { SpellPower = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0f, 1f, int.Parse(match.Groups["stacks"].Value)));
            }
            else if ((match = new Regex(@"Reduces the mana cost of Holy Light by (?<amount>\d\d*)").Match(line)).Success)
            {   // Libram of Renewal
                stats.HolyLightManaCostReduction += (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Increases spell power of Holy Light by (?<amount>\d\d*)").Match(line)).Success)
            {   // Libram of Tolerance | the Resolute
                stats.HolyLightSpellPower += (float)int.Parse(match.Groups["amount"].Value);
            }
            
            // Other
            else if ((match = new Regex(@"Increases spell power of Flash of Light by (?<amount>\d\d*)").Match(line)).Success)
            {
                stats.FlashOfLightSpellPower += (float)int.Parse(match.Groups["amount"].Value);
            }
            #endregion
            #region Added by Paladin: Tankadin
            else if ((match = new Regex(@"Your Judgement ability also increases your shield block value by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Libram of Obstruction
                stats.AddSpecialEffect(new SpecialEffect(Trigger.JudgementHit,
                    new Stats() { JudgementBlockValue = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0f, 1f));
            }
            else if ((match = new Regex(@"Increases your block value by (?<amount>\d\d*) for (?<dur>\d\d*) sec each time you use Holy Shield").Match(line)).Success)
            {   // Libram of the Sacred Shield
                stats.AddSpecialEffect(new SpecialEffect(Trigger.HolyShield,
                    new Stats() { HolyShieldBlockValue = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0f, 1f));
            }
            else if ((match = new Regex(@"Each time you use your Hammer of The Righteous ability, you have a chance to gain (?<amount>\d\d*) dodge rating for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Libram of Defiance
                stats.AddSpecialEffect(new SpecialEffect(Trigger.HammeroftheRighteous,
                    new Stats() { DodgeRating = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 10f, 0.8f));
            }
            else if (line == "Increases the spell power of your Consecration spell by 141")
            {   // Libram of Resurgence
                stats.ConsecrationSpellPower = 141f;
            }
            else if ((match = new Regex(@"Your Shield of Righteousness ability grants (?<amount>\d\d*) dodge rating for (?<dur>\d\d*) sec Stacks up to (?<stacks>\d\d*) times").Match(line)).Success)
            {   // Libram of the Eternal Tower
                stats.AddSpecialEffect(new SpecialEffect(Trigger.ShieldofRighteousness,
                    new Stats() { DodgeRating = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0f, 1f, int.Parse(match.Groups["stacks"].Value)));
            }
            // Other
            else if ((match = new Regex(@"Your Shield of Righteousness deals an additional (?<amount>\d\d*) damage").Match(line)).Success)
            {
                stats.BonusShieldOfRighteousnessDamage = (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Your Holy Shield ability also grants you (?<amount>\d\d*) resilience rating for (?<dur>\d\d*) sec").Match(line)).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.HolyShield,
                    new Stats() { Resilience = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0f, 1f));
            }
            else if ((match = new Regex(@"Your Judgement ability also grants you (?<amount>\d\d*) resilience rating for (?<dur>\d\d*) sec").Match(line)).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.JudgementHit,
                    new Stats() { Resilience = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0f, 1f));
            }
            #endregion
            #region Added by Paladin: Retadin
            else if ((match = new Regex(@"Your Crusader Strike ability also grants you (?<amount>\d\d*) attack power for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Gladiator's Libram of Fortitude
                stats.AddSpecialEffect(new SpecialEffect(Trigger.CrusaderStrikeHit,
                    new Stats() { AttackPower = int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 0f));
            }
            else if ((match = new Regex(@"Increases the damage dealt by Crusader Strike by (?<amount>\d\d*)").Match(line)).Success)
            {   // Libram of Radiance
                stats.CrusaderStrikeDamage = 78.75f;//int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Your Crusader Strike ability grants (?<amount>\d\d*) Strength for (?<dur>\d\d*) sec Stacks up to (?<stacks>\d\d*) times").Match(line)).Success)
            {   // Libram of Three Truths
                stats.AddSpecialEffect(new SpecialEffect(Trigger.CrusaderStrikeHit,
                    new Stats() { Strength = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0f, 1f, int.Parse(match.Groups["stacks"].Value)));
            }
            else if ((match = new Regex(@"Increases the damage done by Divine Storm by (?<amount>\d\d*)").Match(line)).Success)
            {   // Libram of Discord
                stats.DivineStormDamage = (float)int.Parse(match.Groups["amount"].Value);
            }
            else if ((match = new Regex(@"Each time your Seal of Vengeance or Seal of Corruption ability deals periodic damage, you have a chance to gain (?<amount>\d\d*) Strength for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Libram of Valiance
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SealOfVengeanceTick,
                    new Stats() { Strength = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 6f, 0.70f));
            }



            // Other
            else if (line == "Increases the damage dealt by your Crusader Strike ability by 5%")
            {
                stats.CrusaderStrikeMultiplier = 0.05f;
            }
            else if (line.StartsWith("Causes your Divine Storm to increase your Critical Strike rating by 73 for 8 sec"))
            {
                stats.DivineStormDamage = 81;
            }
            else if (line.StartsWith("Causes your Judgements to increase your Critical Strike rating by 53 for 5 sec"))
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.JudgementHit, new Stats() { CritRating = 53 }, 5f, 0f, 1f));
            }
            else if (line.StartsWith("Causes your Judgements to increase your Critical Strike Rating by 61 for 5 sec"))
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.JudgementHit, new Stats() { CritRating = 61 }, 5f, 0f, 1f));
            }
            #endregion
            #region Added by Hunter
            #endregion
            #region Added by Warlock
            #endregion
            #region Added by Mage
            #endregion
            #region Added by Rogue
            #endregion
            #endregion
            #region Professions
            else if (line.StartsWith("Increases the effect that healing and mana potions have on the wearer by "))
            {
                // Alchemist's Stone
                line = line.Substring("Increases the effect that healing and mana potions have on the wearer by ".Length);
                line = line.Substring(0, line.IndexOf('%'));
                stats.BonusManaPotion += int.Parse(line) / 100f;
                stats.HealthRestore += int.Parse(line) / 100f;
            }
            #endregion
            #region General
            #region Armor Penetration Rating
            else if ((match = new Regex(@"Chance on melee (and|or) ranged critical strike to increase your armor penetration rating by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Needle-Encrusted Scorpion
                stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalCrit,
                    new Stats() { ArmorPenetrationRating = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 45,  0.10f));
            }
            else if ((match = new Regex(@"Your melee (and|or) ranged attacks have a chance to increase your armor penetration rating by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Mjolnir Runestone | Grim Toll
                stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalHit,
                    new Stats() { ArmorPenetrationRating = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 45, 0.15f));
            }
            #endregion
            #region Crit Rating
            else if ((match = new Regex(@"Chance on hit to increase your critical strike rating by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalHit,
                    new Stats() { CritRating = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 45, 0.10f));
            }
            else if ((match = new Regex(@"Your melee and ranged attacks have a chance to increase your critical strike rating by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalHit,
                    new Stats() { CritRating = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 45f, 0.15f));
            }
            #endregion
            #region Haste Rating
            else if ((match = new Regex(@"Your harmful spells have a chance to increase your haste rating by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Elemental Focus Stone
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellCast,
                    new Stats() { HasteRating = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 45f, 0.10f));
            }
            else if ((match = new Regex(@"Your melee and ranged attacks have a chance to increase your haste rating by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalHit,
                    new Stats() { HasteRating = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 45, 0.15f));
            }
            else if ((match = new Regex(@"Chance on melee and ranged critical strike to increase your haste rating by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalCrit,
                    new Stats() { HasteRating = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 45, 0.10f));
            }
            else if ((match = new Regex(@"Your spell critical strikes have a 50% chance to grant you (?<amount>\d\d*) spell haste rating for (?<dur>\d\d*) sec").Match(line)).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCrit,
                    new Stats() { HasteRating = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0, 0.50f));
            }
            else if ((match = new Regex(@"Your spells have a chance to increase your haste rating by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Embrace of the Spider
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast,
                    new Stats() { HasteRating = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 45, 0.10f));
            }
            else if ((match = new Regex(@"Your direct healing and heal over time spells have a chance to increase your haste rating by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // The Egg of Mortal Essence
                stats.AddSpecialEffect(new SpecialEffect(Trigger.HealingSpellCast,
                    new Stats() { HasteRating = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 45f, 0.10f));
            }
            #endregion
            #region Attack Power
            else if ((match = new Regex(@"Each time you deal melee or ranged damage to an opponent, you gain (?<amount>\d\d*) attack power for the next (?<dur>\d\d*) sec, stacking up to (?<stacks>\d\d*) times").Match(line)).Success)
            {   // Fury of the Five Flights
                stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalHit,
                    new Stats() { AttackPower = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0f, 1f, int.Parse(match.Groups["stacks"].Value)));
            }
            else if ((match = new Regex(@"Chance on critical hit to increase your attack power by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {
                int ap = int.Parse(match.Groups["amount"].Value);
                int duration = int.Parse(match.Groups["dur"].Value);
                stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalCrit, new Stats() { AttackPower = ap }, duration, 45f, 0.10f));
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCrit, new Stats() { AttackPower = ap }, duration, 45f, 0.10f));
            }
            else if ((match = new Regex(@"Chance on melee and ranged critical strike to increase your attack power by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalCrit,
                    new Stats() { AttackPower = (float)int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 50f, 0.10f));
            }
            else if ((match = new Regex(@"Chance on melee or ranged hit to increase your attack power by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalHit,
                    new Stats() { AttackPower = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 45f, 0.10f));
            }
            else if ((match = new Regex(@"Each time you hit with a melee or ranged attack, you have a chance to gain (?<amount>\d\d*) attack power for (?<dur>\d\d*) sec").Match(line)).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalHit,
                    new Stats() { AttackPower = int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 45f, 0.20f));
            }
            else if ((match = new Regex(@"Each time you deal melee or ranged damage to an opponent, you gain (?<amount>\d\d*) attack power for the next (?<dur>\d\d*) sec(|,) stacking up to (?<stacks>\d\d*) times").Match(line)).Success)
            {   // Herkuml War Token
                stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalHit,
                    new Stats() { AttackPower = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0f, 1f, int.Parse(match.Groups["stacks"].Value)));
            }
            else if ((match = new Regex(@"When you deal damage you have a chance to gain (?<amount>\d\d*) attack power for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Whispering Fanged Skull & Sharpened Twilight Scale
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageDone,
                    new Stats() { AttackPower = int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 45f, 0.35f));
            }
            else if ((match = new Regex(@"Chance on hit to increase your attack power by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Ashen Band of Vengeance - seems to be 60 sec iCD (wowhead)
                stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalHit,
                    new Stats() { AttackPower = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 60f, 0.10f));
            }
            #endregion
            #region Spell Power
            else if ((match = new Regex(@"Each time one of your spells deals periodic damage, you have a chance to gain (?<amount>\d\d*) spell power for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Phylactery of the Nameless Lich
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DoTTick,
                    new Stats() { SpellPower = int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 100f, 0.30f));
            }
            else if ((match = new Regex(@"Each time you deal spell damage to an opponent, you gain (?<amount>\d\d*) spell power for the next (?<dur>\d\d*) sec, stacking up to (?<stacks>\d\d*) times").Match(line)).Success)
            {   // Muradin's Spyglass
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellHit,
                    new Stats() { SpellPower = int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 0f, 1f, int.Parse(match.Groups["stacks"].Value)));
            }
            else if ((match = new Regex(@"Each time you cast a damaging or healing spell you gain (?<amount>\d\d*) spell power for the next (?<dur>\d\d*) sec, stacking up to (?<stacks>\d\d*) times").Match(line)).Success)
            {   // Illustration of the Dragon Soul | Eye of the Broodmother
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast,
                    new Stats() { SpellPower = int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 0f, 1f, int.Parse(match.Groups["stacks"].Value)));
            }
            else if ((match = new Regex(@"Your spells have a chance to increase your spell power by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Pandora's Plea
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast,
                    new Stats() { SpellPower = int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 45f, 0.10f));
            }
            else if ((match = new Regex(@"Each time you cast a harmful spell, you have a chance to gain (?<amount>\d\d*) spell power for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Abyssal Rune
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellCast,
                    new Stats() { SpellPower = int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 45f, 0.25f));
            }
            else if ((match = new Regex(@"Your spell casts have a chance to increase your spell power by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Flow of Knowledge
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast,
                    new Stats() { SpellPower = int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 45, 0.10f));
            }
            else if ((match = new Regex(@"Your spell critical strikes have a chance to increase your spell power by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCrit,
                    new Stats() { SpellPower = int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 45, 0.20f));
            }
            else if ((match = new Regex(@"Gives a chance when your harmful spells land to increase the damage of your spells and effects by up to (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellHit,
                    new Stats() { SpellPower = int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 45, 0.10f));
            }
            else if ((match = new Regex(@"Your offensive spells have a chance on hit to increase your spell power by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Ashen Band of Destruction - seems to be 60 sec iCD (wowhead)
                float internalCooldown = 45.0f;
                if (id == 50397 || id == 50398) { internalCooldown = 60.0f; }
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellHit,
                    new Stats() { SpellPower = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), internalCooldown, 0.10f));
            }
            else if ((match = new Regex(@"Your damaging and healing spells have a chance to increase your spell power by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Forge Ember
                // This is a nasty trick for compatibility = when designing a healer, please use this version:
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast,
                    new Stats() { SpellPower = int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 45, 0.10f));
            }
            else if ((match = new Regex(@"Your harmful spells have a chance to increase your spell power by (?<amount>\d+) for (?<dur>\d+) sec").Match(line)).Success)
            {   // Sundial of the Exiled (NOT FOR HEALERS) | Flare of the Heavens
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellCast,
                    new Stats() { SpellPower = int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 45, 0.10f));
            }
            #endregion
            #region Mp5
            else if ((match = new Regex(@"Each time you cast a spell, you gain (?<amount>\d\d*) mana per 5 sec for (?<dur>\d\d*) sec Stacks up to (?<stacks>\d\d*) times").Match(line)).Success)
            {   // Solace of the Defeated
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast,
                    new Stats() { Mp5 = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 0f, 1f, int.Parse(match.Groups["stacks"].Value)));
            }
            else if ((match = new Regex(@"Each time you cast a (damaging or healing |)spell, there is a chance you will gain up to (?<amount>\d\d*) mana per 5 for (?<dur>\d\d*) sec").Match(line)).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast,
                    new Stats() { Mp5 = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 45f, 0.10f));
            }
            else if ((match = new Regex(@"Your spell casts have a chance to grant (?<amount>\d\d*) mana per 5 sec for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Purified Lunar Dust
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast,
                    new Stats() { Mp5 = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 45f, 0.10f));
            }
            #endregion
            #region Damaging Procs
            else if ((match = new Regex(@"Sends a shadowy bolt at the enemy causing (?<min>\d\d*) to (?<max>\d\d*) Shadow damage.*").Match(line)).Success)
            {   // Raging Deathbringer / Empowered Deathbringer
                int min = int.Parse(match.Groups["min"].Value);
                int max = int.Parse(match.Groups["max"].Value);
                stats.AddSpecialEffect(new SpecialEffect(Trigger.CurrentHandHit,
                    new Stats() { ShadowDamage = (float)(min + max) / 2f, },
                    0f, 0f, 0.05f));  // proc chance is ~5% according to wowhead.  
            }

            else if ((match = new Regex(@"Your harmful spells have a chance to strike your enemy, dealing (?<min>\d\d*) to (?<max>\d\d*) shadow damage.*").Match(line)).Success)
            {   // Pendulum of Telluric Currents
                int min = int.Parse(match.Groups["min"].Value);
                int max = int.Parse(match.Groups["max"].Value);
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellHit,
                    new Stats() { ShadowDamage = (float)(min + max) / 2f, },
                    0f, 45f, 0.10f));
            }
            else if ((match = new Regex(@"Each time one of your spells deals periodic damage, there is a chance (?<min>\d\d*) to (?<max>\d\d*) additional damage will be dealt.*").Match(line)).Success)
            {   // Extract of Necromantic Power
                stats.ExtractOfNecromanticPowerProc += 1;
                int min = int.Parse(match.Groups["min"].Value);
                int max = int.Parse(match.Groups["max"].Value);
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DoTTick,
                    new Stats() { ShadowDamage = (float)(min + max) / 2f,},
                    0f, 45f, 0.10f));
            }
            else if ((match = new Regex(@"Each time you deal damage, you have a chance to do an additional (?<min>\d\d*) to (?<max>\d\d*) Shadow damage.*").Match(line)).Success)
            {   // Darkmoon Card: Death
                stats.DarkmoonCardDeathProc += 1;
                int min = int.Parse(match.Groups["min"].Value);
                int max = int.Parse(match.Groups["max"].Value);
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageDone,
                    new Stats() { ShadowDamage = (float)(min + max) / 2f, },
                    0f, 45f, 0.35f));
            }

            else if ((match = new Regex(@"Your melee and ranged attacks have a chance to strike your enemy, dealing (?<min>\d\d*) to (?<max>\d\d*) arcane damage.*").Match(line)).Success)
            {   // Bandit's Insignia
                int min = int.Parse(match.Groups["min"].Value);
                int max = int.Parse(match.Groups["max"].Value);
                stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalHit,
                    new Stats() { ArcaneDamage = (float)(min + max) / 2f, },
                    0f, 45f, 0.15f));
            }
            else if ((match = new Regex(@"Delivers a fatal wound for (?<min>\d\d*) damage").Match(line)).Success)
            {   // Tempered Vis'kag the bloodletter
                int min = int.Parse(match.Groups["min"].Value);
                stats.AddSpecialEffect(new SpecialEffect(Trigger.CurrentHandHit,
                    new Stats() { PhysicalDamage = (float)min, },
                    0f, 0f, 0.05f));
            }
            #endregion
            #region Spirit
            else if ((match = new Regex(@"Each time you cast a spell you gain (?<amount>\d\d*) Spirit for the next (?<dur>\d\d*) sec, stacking up to (?<stacks>\d\d*) times").Match(line)).Success)
            {   // Majestic Dragon Figurine
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast,
                    new Stats() { Spirit = int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 0f, 1f, int.Parse(match.Groups["stacks"].Value)));
            }
            #endregion
            #region Armor
            else if ((match = new Regex(@"Each time a melee attack strikes you, you have a chance to gain (?<amount>\d\d*) armor for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // The Black Heart
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageTakenPhysical,
                    new Stats() { BonusArmor = int.Parse(match.Groups["amount"].Value) },
                    int.Parse(match.Groups["dur"].Value), 45f, 0.25f));
            }
            #endregion
            #region Specialty Stat Procs
            else if ((match = new Regex(@"When you heal or deal damage you have a chance to gain Greatness").Match(line)).Success)
            {   // Darkmoon Card: Greatness
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageOrHealingDone, new Stats() { HighestStat = 300f }, 15f, 45f, .33f));
            }
            else if ((match = new Regex(@"Your harmful spells have a chance to cause you to summon a Val'kyr to fight by your side for 30 sec").Match(line)).Success)
            {   // Nibelung
                // source http://elitistjerks.com/f75/t37825-rawr_mage/p42/#post1517923
                // 5% crit rate, 1.5 crit multiplier, not affected by talents, affected only by target debuffs
                float damage = (id == 50648) ? 30000 : 27008; // enter value for heroic when it becomes known
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellCast, new Stats() { ValkyrDamage = damage }, 0f, 0f, 0.02f));
            }
            #endregion
            #region Misc
            else if (line.StartsWith("When struck in combat has a chance of shielding you in a protective barrier which will reduce damage from each attack by 140"))
            {   // Essence of Gossamer 
                // 140 damage reduced from EACH attack.
                // Lasts 10 secs.
                // So this isn't a normal X amount absorbed period over 10 sec. like a Corroded Skeleton Key or priest's bubble.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageTaken,
                                    new Stats() { DamageAbsorbed = 140f },
                                    10, 1, 0.05f));
            }
            else if (line.StartsWith("Protected from the cold.  Your Frost resistance is increased by 20"))
            {   // Mechanized Snow Goggles of the...
                stats.FrostResistance = 20;
            }
            else if ((match = Regex.Match(line, @"You gain (?:a|an) (?<buffName>[\w\s]+) each time you cause a (?<trigger>non-periodic|damaging)+ spell critical strike\.(?:\s|nbsp;)+When you reach (?<stackSize>\d+) [\w\s]+, they will release, firing (?<projectile>[\w\s]+) for (?<mindmg>\d+) to (?<maxdmg>\d+) damage\.(?:\s|nbsp;)+[\w\s]+ cannot be gained more often than once every (?<icd>\d+(?:\.\d+)?) sec")).Success)
            {
                // Capacitor like procs
                string buffName = match.Groups["buffName"].Value.TrimStart(' ');
                int stackSize = int.Parse(match.Groups["stackSize"].Value);
                string projectile = match.Groups["projectile"].Value.TrimStart(' ');
                int mindmg = int.Parse(match.Groups["mindmg"].Value);
                int maxdmg = int.Parse(match.Groups["maxdmg"].Value);
                float avgdmgperstack = (mindmg + maxdmg) / 2f / stackSize;
                float icd = float.Parse(match.Groups["icd"].Value, System.Globalization.CultureInfo.InvariantCulture);
                Trigger trigger = Trigger.SpellCrit;
                if (match.Groups["trigger"].Value.StartsWith("damaging"))
                    trigger = Trigger.DamageSpellCrit;
                Stats projectileStats = new Stats();

                if (projectile.StartsWith("a Lightning Bolt"))
                    projectileStats.NatureDamage = avgdmgperstack;
                else if (projectile.StartsWith("a Pillar of Flame"))
                    projectileStats.FireDamage = avgdmgperstack;
                stats.AddSpecialEffect(new SpecialEffect(trigger, projectileStats, 0f, icd));
            }
            // Shadowmourne
            // Your melee attacks have a chance to drain a Soul Fragment granting you 30 Strength.  When you have acquired 10 Soul Fragments you will unleash Chaos Bane, dealing 1900 to 2100 Shadow damage split between all enemies within 15 yards and granting you 270 Strength for 10 sec.
            else if ((match = Regex.Match(line, @"Your melee attacks have a chance to drain a Soul Fragment granting you 30 Strength")).Success)
            //            else if ((match = Regex.Match(line, @"Your melee attacks have a chance to drain a Soul Fragment granting you (?<StrBuff>\d+) Strength.  When you have acquired (?<stackSize>\d+) Soul Fragments you will unleash Chaos Bane, dealing (?<mindmg>\d+) to (?<maxdmg>\d+) Shadow damage split between all enemies within 15 yards and granting you (?<strBuff2>\d+) Strength for (?<icd>) sec")).Success)
            {
                // Capacitor like procs
                Stats BuffStats = new Stats();
                Stats BuffStats2 = new Stats();
                // How much strength per stack.
                BuffStats.Strength = 30;
                // Stack size
                int stackSize = 10;
                // Damage of the final proc.
                int mindmg = 1900;
                int maxdmg = 2100;
                float avgdmg = (mindmg + maxdmg) / 2f;
                // 2ndary Strength Buff
                BuffStats2.Strength = 270;
                BuffStats2.ShadowDamage = avgdmg;
                float icd = 10;
                Trigger trigger = Trigger.MeleeHit;
                // At 10 stacks, the stacks proc the 2nd effect, so you would never have 10 stacks.
                stats.AddSpecialEffect(new SpecialEffect(trigger, BuffStats, 60f, 0f, .2f, (stackSize - 1)));
                stats.AddSpecialEffect(new SpecialEffect(trigger, BuffStats2, icd, 0f, .02f));
            }

            #endregion
            #endregion

            else if (line.StartsWith("Increases your pet's critical strike chance by "))
            {
                string critChance = line.Substring("Increases your pet's critical strike chance by ".Length).Trim();
                if (critChance.EndsWith("%"))
                {
                    stats.BonusPetCritChance = float.Parse(critChance.Substring(0, critChance.Length - 2)) / 100f;
                }
            }
            else if (line.StartsWith("Increases damage dealt by your pet by "))
            {
                string critChance = line.Substring("Increases damage dealt by your pet by ".Length).Trim();
                if (critChance.EndsWith("%"))
                {
                    stats.BonusPetDamageMultiplier = float.Parse(critChance.Substring(0, critChance.Length - 2)) / 100f;
                }
            }
            else if (line.StartsWith("Your healing spells have a chance to make your next heal cast within 15 sec cost 800 less mana"))
            {
                // Soul Preserver
                // This is how RestoSham does it:
                stats.ManacostReduceWithin15OnHealingCast += 800;
                // And that is how Tree does it:
                stats.AddSpecialEffect(new SpecialEffect(Trigger.HealingSpellCast, new Stats() { HealingOmenProc = 800 }, 0, 0, 0.02f));
            }
            else if ((match = Regex.Match(line, @"Reduces the base mana cost of your spells by (?<amount>\d+)")).Success)
            {
                stats.SpellsManaReduction = int.Parse(match.Groups["amount"].Value);
            }
            else if (line.StartsWith("Your spell critical strikes have a chance to restore 900 mana"))
            {
                // Soul of the Dead
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCrit, new Stats() { ManaRestore = 900f }, 0f, 45f, .25f));
            }
            else if (line.StartsWith("Your direct healing spells have a chance to place a heal over time on your target"))
            {
                Regex r = new Regex("Your direct healing spells have a chance to place a heal over time on your target, healing (?<hot>\\d*) over (?<dur>\\d*) sec");
                Match m = r.Match(line);
                if (m.Success)
                {
                    float hot = int.Parse(m.Groups["hot"].Value);
                    float dur = int.Parse(m.Groups["dur"].Value);
                    // internal cooldown: 45 seconds
                    // 20% chance, so on average procs after 5 casts
                    stats.AddSpecialEffect(new SpecialEffect(Trigger.HealingSpellCast, new Stats() { Healed = hot }, 0f, 45f, .2f));
                }
            }
            else if ((match = Regex.Match(line, @"Grants the wielder (?<amount1>\d\d*) defense rating and (?<amount2>\d\d*) armor for (?<dur>\d\d*) sec")).Success)
            {
                // Quel'Serrar Sanctuary proc (all versions)
                stats.AddSpecialEffect(new SpecialEffect(Trigger.MeleeHit,
                    new Stats() { DefenseRating = int.Parse(match.Groups["amount1"].Value), BonusArmor = int.Parse(match.Groups["amount2"].Value) },
                    int.Parse(match.Groups["dur"].Value), 0f, -2f));
            }
            else if ((match = new Regex(@"Steals (?<amount1>\d\d*) to (?<amount2>\d\d*) life from target enemy.*").Match(line)).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.MeleeHit, new Stats()
                {
                    ShadowDamage = (int.Parse(match.Groups["amount1"].Value) + int.Parse(match.Groups["amount2"].Value)) / 2f,
                    //ProcdShadowDamageMin = int.Parse(match.Groups["amount1"].Value),
                    //ProcdShadowDamageMax = int.Parse(match.Groups["amount2"].Value),
                    // Stealing health means that some is restored to the user.
                    HealthRestore = (int.Parse(match.Groups["amount1"].Value) + int.Parse(match.Groups["amount2"].Value)) / 2f,

                }, 0f, 0f, -2f));
            }
            else if (line == "Your healing spells have a chance to cause Blessing of Ancient Kings for 15 sec allowing your heals to shield the target absorbing damage equal to 15% of the amount healed")
            {
                // Val'anyr, Hammer of Ancient Kings effect
                stats.AddSpecialEffect(new SpecialEffect(Trigger.HealingSpellHit, new Stats() { ShieldFromHealed = .15f }, 15f, 45f, .1f));
            }
            else if (line.EndsWith("Weapon Damage"))
            {
                line = line.Trim('+').Substring(0, line.IndexOf(" "));
                stats.WeaponDamage += int.Parse(line);
            }
            #region Special Armory Tags
            else if (isArmory && line.StartsWith("Increases spell power by"))
            {
                line = line.Substring("Increases spell power by".Length);
                line = line.Replace(".", "").Replace(" ", "");
                stats.SpellPower += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases damage done by Shadow spells and effects by up to"))
            {
                line = line.Substring("Increases damage done by Shadow spells and effects by up to".Length);
                line = line.Replace(".", "").Replace(" ", "");
                stats.SpellShadowDamageRating += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases damage done by Fire spells and effects by up to"))
            {
                line = line.Substring("Increases damage done by Fire spells and effects by up to".Length);
                line = line.Replace(".", "").Replace(" ", "");
                stats.SpellFireDamageRating += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases damage done by Frost spells and effects by up to"))
            {
                line = line.Substring("Increases damage done by Frost spells and effects by up to".Length);
                line = line.Replace(".", "").Replace(" ", "");
                stats.SpellFrostDamageRating += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases damage done by Arcane spells and effects by up to"))
            {
                line = line.Substring("Increases damage done by Arcane spells and effects by up to".Length);
                line = line.Replace(".", "").Replace(" ", "");
                stats.SpellArcaneDamageRating += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases damage done by Nature spells and effects by up to"))
            {
                line = line.Substring("Increases damage done by Nature spells and effects by up to".Length);
                line = line.Replace(".", "").Replace(" ", "");
                stats.SpellNatureDamageRating += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Improves haste rating by"))
            {
                line = line.Substring("Improves haste rating by".Length);
                line = line.Replace(".", "").Replace(" ", "");
                stats.HasteRating += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Improves critical strike rating by"))
            {
                line = line.Substring("Improves critical strike rating by".Length);
                line = line.Replace(".", "").Replace(" ", "");
                stats.CritRating += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases your spell penetration by"))
            {
                line = line.Substring("Increases your spell penetration by".Length);
                line = line.Replace(".", "").Replace(" ", "");
                stats.SpellPenetration += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases hit rating by "))
            {
                line = line.Substring("Increases hit rating by ".Length);
                line = line.Replace(".", "").Replace(" ", "");
                stats.HitRating += int.Parse(line);
            }
            // Restores 7 mana per 5 sec
            // Check to see if the desc contains the token 'mana'.  Items like Frostwolf Insignia
            // and Essense Infused Shroom Restore health.
            else if (isArmory && line.StartsWith("Restores ") && line.Contains("mana") && !line.Contains("kill a target"))
            {
                line = line.Substring("Restores ".Length);
                line = line.Substring(0, line.IndexOf(" mana"));
                stats.Mp5 += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases attack power by ") && !line.Contains("forms only"))
            {
                line = line.Substring("Increases attack power by ".Length);
                if (line.Contains(".")) line = line.Substring(0, line.IndexOf("."));
                if (line.Contains(" ")) line = line.Substring(0, line.IndexOf(" "));
                stats.AttackPower += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases defense rating by "))
            {
                line = line.Substring("Increases defense rating by ".Length);
                if (line.Contains(".")) line = line.Substring(0, line.IndexOf("."));
                if (line.Contains(" ")) line = line.Substring(0, line.IndexOf(" "));
                stats.DefenseRating += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases your dodge rating by "))
            {
                line = line.Substring("Increases your dodge rating by ".Length);
                if (line.Contains(".")) line = line.Substring(0, line.IndexOf("."));
                if (line.Contains(" ")) line = line.Substring(0, line.IndexOf(" "));
                stats.DodgeRating += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases your parry rating by "))
            {
                line = line.Substring("Increases your parry rating by ".Length);
                if (line.Contains(".")) line = line.Substring(0, line.IndexOf("."));
                if (line.Contains(" ")) line = line.Substring(0, line.IndexOf(" "));
                stats.ParryRating += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases the block value of your shield by "))
            {
                line = line.Substring("Increases the block value of your shield by ".Length);
                if (line.Contains(".")) line = line.Substring(0, line.IndexOf("."));
                if (line.Contains(" ")) line = line.Substring(0, line.IndexOf(" "));
                stats.BlockValue += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases your shield block rating by "))
            {
                line = line.Substring("Increases your shield block rating by ".Length);
                if (line.Contains(".")) line = line.Substring(0, line.IndexOf("."));
                if (line.Contains(" ")) line = line.Substring(0, line.IndexOf(" "));
                stats.BlockRating += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases your block rating by "))
            {
                line = line.Substring("Increases your block rating by ".Length);
                if (line.Contains(".")) line = line.Substring(0, line.IndexOf("."));
                if (line.Contains(" ")) line = line.Substring(0, line.IndexOf(" "));
                stats.BlockRating += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases your hit rating by "))
            {
                line = line.Substring("Increases your hit rating by ".Length);
                if (line.Contains(".")) line = line.Substring(0, line.IndexOf("."));
                if (line.Contains(" ")) line = line.Substring(0, line.IndexOf(" "));
                stats.HitRating += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases your armor penetration rating by "))
            {
                line = line.Substring("Increases your armor penetration rating by ".Length);
                if (line.Contains(".")) line = line.Substring(0, line.IndexOf("."));
                if (line.Contains(" ")) line = line.Substring(0, line.IndexOf(" "));
                stats.ArmorPenetrationRating += int.Parse(line);
            }
            else if (isArmory && line.StartsWith("Increases armor penetration rating by "))
            {
                line = line.Substring("Increases armor penetration rating by ".Length);
                if (line.Contains(".")) line = line.Substring(0, line.IndexOf("."));
                if (line.Contains(" ")) line = line.Substring(0, line.IndexOf(" "));
                stats.ArmorPenetrationRating += int.Parse(line);

            }
            #endregion
            #region 3.0.1 Trinkets
            else if ((match = Regex.Match(line, @"Chance on melee or ranged attack to enter Wracking Pains, during which your attacks will each grant 15 crit rating, stacking up to 10 times. Expires after 20 sec")).Success)
            {
                // Death Knight's Anguish
                SpecialEffect primary = new SpecialEffect(Trigger.PhysicalHit, new Stats() { }, 20f, 45f, 0.10f);
                SpecialEffect secondary = new SpecialEffect(Trigger.PhysicalHit, new Stats() { CritRating = 15, }, 20f, 0f, 1f, 10);
                primary.Stats.AddSpecialEffect(secondary);
                stats.AddSpecialEffect(primary);
            }
            #endregion
            #region 3.2 Trinkets
            else if ((match = Regex.Match(line.Replace("nbsp;", " "), @"When you deal damage you have a chance to gain Paragon, increasing your Strength or Agility by (?<amount>\d+) for 15 sec Your highest stat is always chosen")).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageDone, new Stats() { Paragon = int.Parse(match.Groups["amount"].Value) }, 15f, 45f, 0.35f));
            }
            else if ((match = Regex.Match(line, @"Each time you cast a helpful spell, you have a chance to gain (?<amount>\d+) mana")).Success)
            {
                // Ephemeral Snowflake
                stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast, new Stats() { ManaRestore = int.Parse(match.Groups["amount"].Value) }, 0f, 45f, 0.4f));
            }
            #endregion
            #region Icecrown Weapon Procs
            else if ((match = Regex.Match(line, @"Your melee attacks have a chance to grant you Necrotic Touch for 10 sec, causing all your melee attacks to deal an additional (?<amount>\d+)% damage as shadow damage")).Success)
            {
                // Black Bruise
                stats.AddSpecialEffect(new SpecialEffect(Trigger.MeleeHit, new Stats() { BonusPhysicalDamageMultiplier = int.Parse(match.Groups["amount"].Value) / 100f }, 10f, 0f, 0.033f));
            }
            else if ((match = new Regex("Your ranged attacks have a (?<amount1>\\d\\d*)% chance to cause you to instantly attack with this weapon for 50% weapon damage").Match(line)).Success)
            {
                // Zod, kneel before him
                stats.ZodProc = (ilvl == 277 ? 0.05f : 0.04f);
            }
            else if ((match = Regex.Match(line, @"Each time your spells heal a target you have a chance to cause the target of your heal to heal themselves and friends within 10 yards for (?<amount>\d+) each sec for 6 sec")).Success)
            {
                // Trauma; Procs on Healing Hit and HoTs; Hits up to 7 people
                stats.AddSpecialEffect(new SpecialEffect(Trigger.HealingSpellHit, new Stats() { Healed = int.Parse(match.Groups["amount"].Value) * 7f }, 6f, 0f, 0.01f, 0));
            }
            else if ((match = Regex.Match(line, @"Your melee attacks have a chance to grant you Blessing of Light, increasing your strength by (?<stramount>\d+) and your healing received by up to (?<healamount>\d+) for 10 sec")).Success)
            {
                // Last Word
                stats.AddSpecialEffect(new SpecialEffect(Trigger.HealingSpellHit, new Stats() { Strength = int.Parse(match.Groups["stramount"].Value), BonusHealingReceived = int.Parse(match.Groups["healamount"].Value) }, 10f, 0f, 0.37f, 1));
            }
            #endregion
            #region 3.3 Trinkets
            else if ((match = Regex.Match(line, @"Each time you are struck by a melee attack, you have a 60% chance to gain (?<stamina>\d+) stamina for the next 10 sec, stacking up to 10 times")).Success)
            {
                // Unidentifiable Organ
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageTakenPhysical, new Stats() { Stamina = int.Parse(match.Groups["stamina"].Value) }, 10.0f, 0.0f, 0.6f, 10));
            }
            else if (line.StartsWith("Your attacks have a chance to awaken the powers of the races of Northrend, temporarily transforming you and increasing your combat capabilities for 30 sec"))
            {
                // Deathbringer's Will
                stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalHit, new Stats() { DeathbringerProc = ilvl == 277 ? 700 : 600 }, 30f, 105f, 0.15f));
            }
            else if ((match = Regex.Match(line, @"Your melee attacks have a chance to grant you a Mote of Anger\. (nbsp;| )?When you reach (?<amount>\d+) Motes of Anger, they will release, causing you to instantly attack for 50% weapon damage with one of your melee weapons")).Success)
            {
                // Tiny Abomination Jar
                stats.AddSpecialEffect(new SpecialEffect(Trigger.MeleeAttack, new Stats() { MoteOfAnger = 0.5f }, 0f, 0f, 0.5f, int.Parse(match.Groups["amount"].Value)));
            }
            else if ((match = Regex.Match(line, @"You gain (?<mana>\d+) mana each time you heal a target with one of your spells")).Success)
            {
                // Epheremal Snowflake - assume iCD of 0.33 sec (wowhead)
                stats.AddSpecialEffect(new SpecialEffect(Trigger.HealingSpellHit, new Stats() { ManaRestore = int.Parse(match.Groups["mana"].Value) }, 0f, 0.33f, 1f));
            }
            else if ((match = new Regex(@"Each time your spells heal a target you have a chance to cause another nearby friendly target to be instantly healed for (?<min>\d\d*) to (?<max>\d\d*)").Match(line)).Success)
            {
                // Althor's Abacus
                float min = (float)int.Parse(match.Groups["min"].Value);
                float max = (float)int.Parse(match.Groups["max"].Value);
                stats.AddSpecialEffect(new SpecialEffect(Trigger.HealingSpellHit, new Stats() { Healed = min + (max - min) / 2f }, 0f, 45f, 0.3f));
            }
            else if ((match = new Regex(@"Your harmful spells have a chance to increase your spell power by (?<initialSP>\d+) and an additional (?<additionalSP>\d+) every (?<frequency>\d+) sec for (?<time>\d+) sec").Match(line)).Success)
            {
                // Dislodged Foreign Object - seems to be 45 sec iCD (wowhead)
                float initialSP = (float)int.Parse(match.Groups["initialSP"].Value);
                float additionalSP = (float)int.Parse(match.Groups["additionalSP"].Value);
                float frequency = (float)int.Parse(match.Groups["frequency"].Value);
                float time = (float)int.Parse(match.Groups["time"].Value);
                float min = initialSP;
                float max = initialSP + additionalSP * (time / frequency - 1f);
                float averageSP = (min + max) / 2f;
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellHit, new Stats() { SpellPower = averageSP }, time, 45f, 0.1f));
            }
            else if ((match = new Regex(@"Melee attacks which reduce you below 35% health cause you to gain (?<armor>\d+) armor for 10 sec").Match(line)).Success)
            {
                // Corpse Tongue Coin
                // Melee attacks which reduce you below 35% health cause you to gain 5712 armor for 10 sec.  Cannot occur more than once every 30 sec.
                float fArmor = (float)int.Parse(match.Groups["armor"].Value);;
                float fDuration = 10;
                float fICD = 30;
                // Assuming the target will be under 35% health for that amount of time.
                float fChance = .35f/2f;
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageTakenPhysical, new Stats() { BonusArmor = fArmor }, fDuration, fICD, fChance));
            }
            #endregion
            #region 3.3 rings
            else if ((match = new Regex(@"Your helpful spells have a chance to increase your spell power by (?<power>\d+) for (?<time>\d+) sec").Match(line)).Success)
            {
                // Ashen Band of Wisdom - seems to be 60 sec iCD (wowhead)
                float spellpower = (float)int.Parse(match.Groups["power"].Value);
                float time = (float)int.Parse(match.Groups["time"].Value);
                stats.AddSpecialEffect(new SpecialEffect(Trigger.HealingSpellCast, new Stats() { SpellPower = spellpower }, time, 60f, 0.1f));
            }
            else if ((match = new Regex(@"When struck in combat has a chance of increasing your armor by (?<armor>\d+) for (?<time>\d+) sec").Match(line)).Success)
            {
                // Ashen Band of Courage - seems to be 60 sec iCD (wowhead)
                // Apparently confirmed as 3% chance to proc -> http://maintankadin.failsafedesign.com/forum/index.php?f=21&t=27115&rb_v=viewtopic
                float armor = (float)int.Parse(match.Groups["armor"].Value);
                float time = (float)int.Parse(match.Groups["time"].Value);
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageTaken, new Stats() { BonusArmor = armor }, time, 60f, 0.03f));
            }


            #endregion
            #region 3.3.5 Trinkets
                // Note that Sharpened Twilight Scale already is modeled via Whispering Fanged Skull
            else if ((match = new Regex(@"Melee attacks which reduce you below 35% health cause you to gain (?<amount>\d+) dodge rating for 10 sec").Match(line)).Success)
            {
                // Petrified Twilight Scale
                // Melee attacks which reduce you below 35% health cause you to gain 733 dodge rating for 10 sec.  Cannot occur more than once every 45 sec.
                float fDodge = int.Parse(match.Groups["amount"].Value);
                float fDuration = 10;
                float fICD = 45;
                // Assuming the target will be under 35% health for that amount of time.
                float fChance = .35f/2f;
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageTakenPhysical, new Stats() { DodgeRating = fDodge }, fDuration, fICD, fChance));
            }
            else if ((match = new Regex(@"Your damaging spells have a chance to grant (?<spellPower>\d+) spell power for 15 sec").Match(line)).Success)
            {
                // Charred Twilight Scale
                float spellPower = int.Parse(match.Groups["spellPower"].Value);
                stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellHit, new Stats() { SpellPower = spellPower }, 15.0f, 45.0f, 0.1f));
            }
            #endregion
            else
            {
                #region Prototype Sigil Code:
                /* Prototype Sigil Code:

             Sigil of the Unfaltering Knight:
               Your Icy Touch will also increase your defense rating by 53.
             * Your [ability] will also increase your [stat] by [amount].
             Furious Gladiator's Sigil of Strife
               Your Plague Strike ability also grants you 144 attack power for 10 sec.
             * Your [ability] ability also grants you [amount] [stat] for [duration] sec.
             Sigil of Deflection
               Your Rune Strike ability grants 136 dodge rating for 5 sec.
             * Your [ability] ability also grants you [amount] [stat] for [duration] sec.
             Sigil of Insolence
               Each time you use your Rune Strike ability, you have a chance to gain 200 dodge rating for 20 sec.
             * Each time you use your [ability] ability, you have a chance to gain [amount] [stat] for [duration] sec.
             Sigil of the Vengeful Heart
               Increases the damage done by your Death Coil and Frost Strike abilities by 380.
             * Increases the damage done by your [ability] and [ability] abilities by [amount].
             Deadly Gladiator's Sigil of Strife
               Your Plague Strike ability also grants you 120 attack power for 10 sec.
             * Your [ability] ability also grants you [amount] [stat] for [duration] sec.
            Sigil of Awareness
                Increases the base damage dealt by your Scourge Strike by 189, your Obliterate by 336, and your Death Strike by 315.
             * Increases the base damage dealt by your [ability1] by [amount1], your [ability2] by [amount2], and your [ability3] by [amount3].
            Hateful Gladiator's Sigil of Strife
                Your Plague Strike ability also grants you 106 attack power for 6 sec.
             * Your [ability] ability also grants you [amount] [stat] for [duration] sec.
            Sigil of Haunted Dreams
                Your Blood Strike and Heart Strikes have a chance to grant 173 critical strike rating for 10 sec.
             * Your [ability] and [ability] have a chance to grant [amount] [stat] for [duration] sec.
             * Key point (chance) means proc rate factoring
            Savage Gladiator's Sigil of Strife
                Your Plague Strike ability also grants you 94 attack power for 6 sec.
             * Your [ability] ability also grants you [amount] [stat] for [duration] sec.
            Sigil of Arthritic Binding
                Increases the damage dealt by your Scourge Strike ability by 91.35.
             * Increases the damage dealt by your [ability] ability by [amount].
            Sigil of the Frozen Conscience
                Increases the damage dealt by your Icy Touch ability by 111.
             * Increases the damage dealt by your [ability] ability by [amount].
            Sigil of the Wild Buck
                Increases the damage dealt by your Death Coil ability by 80.
             * Increases the damage dealt by your [ability] ability by [amount].
            Sigil of the Dark Rider
                Increases the damage dealt by your Blood Strike and Heart Strike by 90.
             * Increases the damage dealt by your [ability] and [ability] by [amount].
            Sigil of the Bone Gryphon
                 * Your Rune Strike ability grants 44 dodge rating for 15 sec.  Stacks up to 5 times.
                 * Your Rune Strike ability grants [Amount] [stat] for [duration] sec.  Stacks up to [stack] times.
            */
                // Single Ability.
                Regex regex = new Regex(@"Your (?<ability>\w+\s*\w*) (ability )?(also )?grants (you )?(?<amount>\d*) (?<stat>\w+\s*\w*) for (?<duration>\d*) sec(\s*Stacks up to (?<stack>\d*) times)?");
                match = regex.Match(line);
                if (match.Success)
                {
                    string statName = match.Groups["stat"].Value;
                    float amount = int.Parse(match.Groups["amount"].Value);
                    float duration = int.Parse(match.Groups["duration"].Value);
                    string ability = match.Groups["ability"].Value;
                    int stack = 0;
                    if (match.Groups["stack"].Value != "")
                    {
                        stack = int.Parse(match.Groups["stack"].Value);
                    }

                    stats.AddSpecialEffect(EvalRegex(statName, amount, duration, ability, 0f, 1f, stack));
                }
                // ok... at this point with the way the code is written, this is really just for Sigil of the Unfaltering Knight, which grants a 
                // 30 sec buff of 53 Def. Since it's a triggered event, and all, it's better to go off the trigger rather than a straight 
                // 53 def all the time.  So adjusting this to be a special effect and assuming 30 secs.
                regex = new Regex(@"Your (?<ability>\w+\s*\w*) (ability )?(will )?(also )?increase (your )?(?<stat>\w+\s*\w*) by (?<amount>\d*)");
                match = regex.Match(line);
                if (match.Success)
                {
                    string statName = match.Groups["stat"].Value;
                    float amount = int.Parse(match.Groups["amount"].Value);
                    string ability = match.Groups["ability"].Value;

                    stats.AddSpecialEffect(EvalRegex(statName, amount, 30, ability, 0f));
                }

                regex = new Regex(@"Your (?<ability>\w+\s*\w*) and (?<ability2>\w+\s*\w*) (abilities )?have a chance to grant (?<amount>\d*) (?<stat>\w+[\s\w]*) for (?<duration>\d*) sec");
                match = regex.Match(line);
                if (match.Success)
                {
                    string statName = match.Groups["stat"].Value;
                    float amount = int.Parse(match.Groups["amount"].Value);
                    float duration = int.Parse(match.Groups["duration"].Value);
                    string ability = match.Groups["ability"].Value;
                    string ability2 = match.Groups["ability2"].Value;

                    SpecialEffect SE1 = EvalRegex(statName, amount, duration, ability, 0f, 0.15f);
                    stats.AddSpecialEffect(SE1);
                    SpecialEffect SE2 = EvalRegex(statName, amount, duration, ability2, 0f, 0.15f);
                    if (SE1.ToString() != SE2.ToString())
                    {
                        stats.AddSpecialEffect(SE2);
                    }
                }
                /*
                 * Sigil of the Hanged Man
                 * Equip: Your Obliterate, Scourge Strike, and Death Strike abilities grants 73 Strength for 15 sec.  Stacks up to 3 times.
                 * 		line	"Your Obliterate, Scourge Strike, and Death Strike abilities grants 73 Strength for 15 sec.  Stacks up to 3 times"	string
                 * */
                regex = new Regex(@"Your (?<ability>\w+\s*\w*), (?<ability2>\w+\s*\w*), and (?<ability3>\w+\s*\w*) (abilities )?grants (?<amount>\d*) (?<stat>\w+[\s\w]*) for (?<duration>\d*) sec Stacks up to (?<stacks>\d*) times");
                match = regex.Match(line);
                if (match.Success)
                {
                    string statName = match.Groups["stat"].Value;
                    float amount = int.Parse(match.Groups["amount"].Value);
                    float duration = int.Parse(match.Groups["duration"].Value);
                    int stacks = int.Parse(match.Groups["stacks"].Value);
                    string ability = match.Groups["ability"].Value;
                    string ability2 = match.Groups["ability2"].Value;
                    string ability3 = match.Groups["ability3"].Value;

                    SpecialEffect SE1 = EvalRegex(statName, amount, duration, ability, 0f, 1f, stacks);
                    stats.AddSpecialEffect(SE1);
                    SpecialEffect SE2 = EvalRegex(statName, amount, duration, ability2, 0f, 1f, stacks);
                    SpecialEffect SE3 = EvalRegex(statName, amount, duration, ability3, 0f, 1f, stacks);
                    if (SE1.ToString() != SE2.ToString())
                    {
                        stats.AddSpecialEffect(SE2);
                    }
                    if (SE3.ToString() != SE2.ToString() && SE3.ToString() != SE1.ToString())
                    {
                        stats.AddSpecialEffect(SE3);
                    }
                }

                regex = new Regex(@"Each time you use your (?<ability3>\w+\s*\w*), (?<ability>\w+\s*\w*), or (?<ability2>\w+\s*\w*) ability, you have a chance to gain (?<amount>\d*) (?<stat>\w+[\s\w]*) for (?<duration>\d*) sec");
                match = regex.Match(line);
                if (match.Success)
                {
                    string statName = match.Groups["stat"].Value;
                    float amount = int.Parse(match.Groups["amount"].Value);
                    float duration = int.Parse(match.Groups["duration"].Value);
                    string ability = match.Groups["ability"].Value;
                    string ability2 = match.Groups["ability2"].Value;
                    string ability3 = match.Groups["ability3"].Value;

                    SpecialEffect SE1 = EvalRegex(statName, amount, duration, ability, 10f, 0.8f);
                    stats.AddSpecialEffect(SE1);
                    SpecialEffect SE2 = EvalRegex(statName, amount, duration, ability2, 10f, 0.8f);
                    SpecialEffect SE3 = EvalRegex(statName, amount, duration, ability3, 10f, 0.8f);
                    if (SE1.ToString() != SE2.ToString())
                    {
                        stats.AddSpecialEffect(SE2);
                    }
                    if (SE3.ToString() != SE2.ToString())
                    {
                        stats.AddSpecialEffect(SE3);
                    }
                }

                // Each time you use your [ability] ability, you have a chance to gain [amount] [stat] for [duration] sec
                regex = new Regex(@"Each time you use your (?<ability>\w+\s*\w*) ability, you have a chance to gain (?<amount>\d*) (?<stat>\w+[\s\w]*) for (?<duration>\d*) sec");
                match = regex.Match(line);
                if (match.Success)
                {
                    string statName = match.Groups["stat"].Value;
                    float amount = int.Parse(match.Groups["amount"].Value);
                    float duration = int.Parse(match.Groups["duration"].Value);
                    string ability = match.Groups["ability"].Value;

                    SpecialEffect SE1 = EvalRegex(statName, amount, duration, ability, 0f, 0.8f);
                    stats.AddSpecialEffect(SE1);
                }

                // Single Ability damage increase.
                regex = new Regex(@"Increases the damage dealt by your (?<ability>\w+\s*\w*) (ability )?by (?<amount>\d+[.]*\d*)");
                match = regex.Match(line);
                if (match.Success)
                {
                    string statName = match.Groups["stat"].Value;
                    float amount = float.Parse(match.Groups["amount"].Value, System.Globalization.CultureInfo.InvariantCulture);
                    string ability = match.Groups["ability"].Value;
                    //string ability2 = match.Groups["ability2"].Value;

                    EvalAbility(ability, stats, amount);
                }
                // 2 Abilities damage increase.
                regex = new Regex(@"Increases the damage (dealt |done )?by your (?<ability>\w+\s*\w*) and (?<ability2>\w+\s*\w*) (abilities )?by (?<amount>\d+)");
                match = regex.Match(line);
                if (match.Success)
                {
                    string statName = match.Groups["stat"].Value;
                    float amount = int.Parse(match.Groups["amount"].Value);
                    string ability = match.Groups["ability"].Value;
                    string ability2 = match.Groups["ability2"].Value;

                    EvalAbility(ability, stats, amount);
                    EvalAbility(ability2, stats, amount);
                }
                // 2 Abilities with separate damage increases.
                regex = new Regex(@"Increases the (base )?damage (dealt |done )?by your (?<ability>\w+\s*\w*) ability by (?<amount>\d+) and by your (?<ability2>\w+\s*\w*) ability by (?<amount2>\d+)");
                match = regex.Match(line);
                if (match.Success)
                {
                    string statName = match.Groups["stat"].Value;
                    float amount = int.Parse(match.Groups["amount"].Value);
                    float amount2 = int.Parse(match.Groups["amount2"].Value);
                    string ability = match.Groups["ability"].Value;
                    string ability2 = match.Groups["ability2"].Value;

                    EvalAbility(ability, stats, amount);
                    EvalAbility(ability2, stats, amount2);
                }
                // 3 Abilities damage increase.
                regex = new Regex(@"Increases the (base )?damage (dealt |done )?by your (?<ability>\w+\s*\w*) by (?<amount>\d+), your (?<ability2>\w+\s*\w*) by (?<amount2>\d+), and your (?<ability3>\w+\s*\w*) by (?<amount3>\d+)");
                match = regex.Match(line);
                if (match.Success)
                {
                    string statName = match.Groups["stat"].Value;
                    float amount = int.Parse(match.Groups["amount"].Value);
                    float amount2 = int.Parse(match.Groups["amount2"].Value);
                    float amount3 = int.Parse(match.Groups["amount3"].Value);
                    string ability = match.Groups["ability"].Value;
                    string ability2 = match.Groups["ability2"].Value;
                    string ability3 = match.Groups["ability3"].Value;

                    EvalAbility(ability, stats, amount);
                    EvalAbility(ability2, stats, amount2);
                    EvalAbility(ability3, stats, amount3);
                }
                #endregion Prototype Sigil Code
            }
        }

        public static void ProcessUseLine(string line, Stats stats, bool isArmory, int id) {
            Regex regex = new Regex(@"Increases (your )?(?<stat>\w\w*( \w\w*)*) by (?<amount>\+?\d\d*)(nbsp;\<small\>.*\<\/small\>)?(<a.*q2.*>) for (?<duration>\d\d*) sec \((?<cooldown>\d\d*) Min Cooldown\)");
            /*#region Prep the line, if it needs it
            while (line.Contains("secs")) { line = line.Replace("secs", "sec"); }
            while (line.Contains("sec.")) { line = line.Replace("sec.", "sec"); }
            while (line.Contains("  ")) { line = line.Replace("  ", " "); }
            while (line.EndsWith(".")) { line = line.Substring(0, line.Length-1); }
            #endregion*/
            Match match = regex.Match(line);
            if (match.Success)
            {
                Stats s = new Stats();
                string statName = match.Groups["stat"].Value;
                float amount = int.Parse(match.Groups["amount"].Value);
                float duration = int.Parse(match.Groups["duration"].Value);
                float cooldown = int.Parse(match.Groups["cooldown"].Value) * 60;

                if (statName.Equals("attack power", StringComparison.InvariantCultureIgnoreCase)) { s.AttackPower = amount; }
                else if (statName.Equals("melee and ranged attack power", StringComparison.InvariantCultureIgnoreCase)) { s.AttackPower = amount; }
                else if (statName.Equals("haste rating", StringComparison.InvariantCultureIgnoreCase)) { s.HasteRating = amount; }
                else if (statName.Equals("spell power", StringComparison.InvariantCultureIgnoreCase)) { s.SpellPower = amount; }
                else if (statName.Equals("agility", StringComparison.InvariantCultureIgnoreCase)) { s.Agility = amount; }
                else if (statName.Equals("critical strike rating", StringComparison.InvariantCultureIgnoreCase)) { s.CritRating = amount; }
                else if (statName.Equals("dodge", StringComparison.InvariantCultureIgnoreCase)) { s.DodgeRating = amount; }
                else if (statName.Equals("dodge rating", StringComparison.InvariantCultureIgnoreCase)) { s.DodgeRating = amount; }
                else if (statName.Equals("parry", StringComparison.InvariantCultureIgnoreCase)) { s.ParryRating = amount; }
                else if (statName.Equals("parry rating", StringComparison.InvariantCultureIgnoreCase)) { s.ParryRating = amount; }
                else if (statName.Equals("spirit", StringComparison.InvariantCultureIgnoreCase)) { s.Spirit = amount; }
                else if (statName.Equals("armor penetration rating", StringComparison.InvariantCultureIgnoreCase)) { s.ArmorPenetrationRating = amount; }
                else if (statName.Equals("block value", StringComparison.InvariantCultureIgnoreCase)) { s.BlockValue = amount; }
                else if (statName.Equals("maximum health", StringComparison.InvariantCultureIgnoreCase)) { s.Health = amount; }
                else if (statName.Equals("armor", StringComparison.InvariantCultureIgnoreCase)) { s.BonusArmor = amount; }
                else if (statName.Equals("the block value of your shield", StringComparison.InvariantCultureIgnoreCase)) { s.BlockValue = amount; }
                else if (statName.Equals("master rating", StringComparison.InvariantCultureIgnoreCase)) { s.MasteryRating = amount; }
                
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, s, duration, cooldown));
            }
            // Victor's Call and Vengeance of the Forsaken (232 & 245)
            else if ((match = new Regex(@"Each time you strike an enemy.*, you gain (?<amount>\d+) attack power. Stacks up to 5 times. Entire effect lasts 20 sec.*").Match(line.Replace("  "," "))).Success)
            {
                SpecialEffect primary = new SpecialEffect(Trigger.Use, new Stats(), 20f, 2f * 60f);
                SpecialEffect secondary = new SpecialEffect(Trigger.MeleeHit, new Stats() { AttackPower = (float)int.Parse(match.Groups["amount"].Value) }, 20f, 0f, 1f, 5);
                primary.Stats.AddSpecialEffect(secondary);
                stats.AddSpecialEffect(primary);
            }
            else if ((match = new Regex(@"Increases attack power by (?<amount>\d\d*) for (?<dur>\d\d*) sec").Match(line)).Success)
            {   // Wrath Stone
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                    new Stats() { AttackPower = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["dur"].Value), 120f));
            }
            // Wrath: Eye of the Night
            else if (Regex.IsMatch(line, "Increases spell power by (\\d{2}) for all nearby party members."))
            {
                string[] inputs = Regex.Split(line, "Increases spell power by (\\d{2}) for all nearby party members.");
                // the full text reads: "Use: Increases spell power by 34 for all nearby party members.  Lasts 30 min. (1 Hour Cooldown)"
                // So in general you get the 34 spell power for 100% of a given boss fight.  Like most trinkets it is up to the player to
                // use it correctly, so we are going to give them the full power instead of half.
                float spell_power = float.Parse(inputs[1]);
                stats.SpellPower += spell_power;
            }
            // Medallion of Heroism
            else if ((match = new Regex(@"Heal self for (?<amount1>\d\d\d*) to (?<amount2>\d\d\d*) damage.*").Match(line)).Success)
            {
                int val1 = int.Parse(match.Groups["amount1"].Value);
                int val2 = int.Parse(match.Groups["amount2"].Value);
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { HealthRestore = (float)((val1 + val2) / 2f) }, 0f, 2 * 60));
            }
            // Increases spell power by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases spell power by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases spell power by (\\d{3}) for (\\d{2}) sec");
                float spell_power = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown is on the next line
                float cooldown_sec = 2.0f * 60.0f;
                // Vengeance of the Illidari (tooltip lies!)
                if (id == 28040) { cooldown_sec = 90.0f; }
                // we don't have a stat for this case so do the average
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { SpellPower = spell_power }, uptime, cooldown_sec));
            }
            // Increases spell power by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases your spell power by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases your spell power by (\\d{3}) for (\\d{2}) sec");
                float spell_power = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown is on the next line
                float cooldown_sec = 2.0f * 60.0f;
                // Twilight Serpent
                if (id == 42395) { cooldown_sec = 5.0f * 60.0f; }
                // Vengeance of the Illidari (tooltip lies!)
                if (id == 28040) { cooldown_sec = 90.0f; }
                // we don't have a stat for this case so do the average
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { SpellPower = spell_power }, uptime, cooldown_sec));
            }
            // Increases attack power by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases armor penetration rating by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases armor penetration rating by (\\d{3}) for (\\d{2}) sec");
                float armor_penetration = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { ArmorPenetrationRating = armor_penetration }, uptime, 120f));
            }
            // Increases attack power by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases your armor penetration rating by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases your armor penetration rating by (\\d{3}) for (\\d{2}) sec");
                float armor_penetration = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { ArmorPenetrationRating = armor_penetration }, uptime, 120f));
            }
            // Increases attack power by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases attack power by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases attack power by (\\d{3}) for (\\d{2}) sec");
                float attack_power = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { AttackPower = attack_power }, uptime, 120f));
            }
            // Increases attack power by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases your attack power by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases your attack power by (\\d{3}) for (\\d{2}) sec");
                float attack_power = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { AttackPower = attack_power }, uptime, 120f));
            }
            // Increases attack power by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases attack power by (\\d{4}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases attack power by (\\d{4}) for (\\d{2}) sec");
                float attack_power = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { AttackPower = attack_power }, uptime, 120f));
            }
            // Increases attack power by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases your attack power by (\\d{4}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases your attack power by (\\d{4}) for (\\d{2}) sec");
                float attack_power = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { AttackPower = attack_power }, uptime, 120f));
            }
            // Increases armor by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases armor by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases armor by (\\d{3}) for (\\d{2}) sec");
                float armor = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BonusArmor = armor }, uptime, 120f));
            }
            // Increases armor by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases your armor by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases your armor by (\\d{3}) for (\\d{2}) sec");
                float armor = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BonusArmor = armor }, uptime, 120f));
            }
            // Increases armor by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases armor by (\\d{4}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases armor by (\\d{4}) for (\\d{2}) sec");
                float armor = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BonusArmor = armor }, uptime, 120f));
            }
            // Increases armor by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases your armor by (\\d{4}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases your armor by (\\d{4}) for (\\d{2}) sec");
                float armor = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BonusArmor = armor }, uptime, 120f));
            }
            // Increases maximum health, shares cooldown with other Battlemaster's trinkets
            else if (line.Contains("Battlemaster\'s trinkets."))
            {
                Regex r = new Regex("Increases maximum health by (?<health>\\d*) for (?<duration>\\d*) sec");
                Match m = r.Match(line);
                if (m.Success)
                {
                    float health = float.Parse(m.Groups["health"].Value);
                    float duration = float.Parse(m.Groups["duration"].Value);
                    float cooldown = 3.0f * 60.0f; // Default to 3 minute cooldown, if not otherwise known

                    if (!isArmory)
                    {
                        Regex rCD = new Regex("\\((?<cooldown>\\d*) Min Cooldown\\)");
                        Match mCD = rCD.Match(line);
                        if (mCD.Success)
                            cooldown = int.Parse(mCD.Groups["cooldown"].Value) * 60.0f;
                    }

                    stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BattlemasterHealth = health }, duration, cooldown));
                }
            }
            // Increases maximum health by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases maximum health by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases maximum health by (\\d{3}) for (\\d{2}) sec");
                float health = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 3min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Health = health }, uptime, 180f));
            }
            // Increases maximum health by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases your maximum health by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases your maximum health by (\\d{3}) for (\\d{2}) sec");
                float health = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 3min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Health = health }, uptime, 180f));
            }
            // Increases maximum health by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases maximum health by (\\d{4}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases maximum health by (\\d{4}) for (\\d{2}) sec");
                float health = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 3min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Health = health }, uptime, 180f));
            }
            // Increases maximum health by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases your maximum health by (\\d{4}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases your maximum health by (\\d{4}) for (\\d{2}) sec");
                float health = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 3min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Health = health }, uptime, 180f));
            }
            // Increases dodge rating by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases dodge by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases dodge by (\\d{3}) for (\\d{2}) sec");
                float dodge_rating = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                float cooldown_sec = 120f;
                if (id == 44063) { cooldown_sec = 60.0f; } //Figurine - Monarch Crab
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { DodgeRating = dodge_rating }, uptime, cooldown_sec));
            }
            // Increases dodge rating by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases dodge rating by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases dodge rating by (\\d{3}) for (\\d{2}) sec");
                float dodge_rating = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                float cooldown_sec = 120f;
                if (id == 44063) { cooldown_sec = 60.0f; } //Figurine - Monarch Crab
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { DodgeRating = dodge_rating }, uptime, cooldown_sec));
            }
            // Increases dodge rating by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases your dodge rating by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases your dodge rating by (\\d{3}) for (\\d{2}) sec");
                float dodge_rating = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                float cooldown_sec = 120f;
                if (id == 44063) { cooldown_sec = 60.0f; } //Figurine - Monarch Crab
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { DodgeRating = dodge_rating }, uptime, cooldown_sec));
            }
            // Increases parry rating by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases parry rating by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases parry rating by (\\d{3}) for (\\d{2}) sec");
                float parry_rating = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { ParryRating = parry_rating }, uptime, 120f));
            }
            // Increases parry rating by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases your parry rating by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases your parry rating by (\\d{3}) for (\\d{2}) sec");
                float parry_rating = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { ParryRating = parry_rating }, uptime, 120f));
            }
            // Increases haste rating by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases haste rating by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases haste rating by (\\d{3}) for (\\d{2}) sec");
                float haste_rating = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { HasteRating = haste_rating }, uptime, 120f));
            }
            // Increases haste rating by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases your haste rating by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases your haste rating by (\\d{3}) for (\\d{2}) sec");
                float haste_rating = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { HasteRating = haste_rating }, uptime, 120f));
            }
            // Increases haste rating by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Increases the block value of your shield by (\\d{3}) for (\\d{2}) sec"))
            {
                string[] inputs = Regex.Split(line, "Increases the block value of your shield by (\\d{3}) for (\\d{2}) sec");
                float block = float.Parse(inputs[1]);
                float uptime = float.Parse(inputs[2]);
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BlockValue = block }, uptime, 120f));
            }
            // Increases haste rating by 183 for 20 sec.
            else if (Regex.IsMatch(line, "Instantly heal your current friendly target for (\\d{4})."))
            {
                string[] inputs = Regex.Split(line, "Instantly heal your current friendly target for (\\d{4}).");
                float heal = float.Parse(inputs[1]);
                // unfortunately for us the cooldown isn't listed, so guess 2min.
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Healed = heal }, 0, 60f));
            }
            else if (line.StartsWith("Increases your Spirit by "))
            {
                // Bangle of Endless Blessings, Earring of Soulful Meditation
                Regex r = new Regex("Increases your Spirit by \\+?(?<spi>\\d*) for (?<dur>\\d*) sec"); // \\(2 Min Cooldown\\)");
                Match m = r.Match(line);
                if (m.Success)
                {
                    int spi = int.Parse(m.Groups["spi"].Value);
                    int dur = int.Parse(m.Groups["dur"].Value);
                    stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Spirit = spi }, dur, 120f));
                }
            }
            else if (line.StartsWith("Increases speed by "))
            {
                // Figurine - Ruby Hare : "Increases speed by 30% for 6 sec. (3 Min Cooldown)"
                Regex r = new Regex("Increases speed by (?<speed>\\d*)% for (?<dur>\\d*) sec"); // \\((?<cd>\\d*) Min Cooldown\\)");
                Match m = r.Match(line);
                if (m.Success)
                {
                    int speed = int.Parse(m.Groups["speed"].Value);
                    int dur = int.Parse(m.Groups["dur"].Value);
                    // Test again with the cd... Available only on wowhead.
                    float cd;
                    Regex rCD = new Regex("Increases speed by (?<speed>\\d*)% for (?<dur>\\d*) sec \\((?<cd>\\d*) Min Cooldown\\)");
                    Match mCD = rCD.Match(line);
                    if (mCD.Success)
                    {
                        cd = int.Parse(mCD.Groups["cd"].Value) * 60.0f;
                    }
                    else
                    {
                        cd = 3.0f * 60.0f;
                    }
                    stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { MovementSpeed = speed/100f }, dur, cd));
                }
            }
            else if (Regex.IsMatch(line, "When the shield is removed by any means, you regain (\\d{4}) mana."))
            {
                string[] inputs = Regex.Split(line, "When the shield is removed by any means, you regain (\\d{4}) mana.");
                float restore = float.Parse(inputs[1]);
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { ManaRestore = restore }, 0f, 5 * 60f));
            }
            else if (line.StartsWith("For the next 20 sec, your direct heals increase healing received by their target by up to 58."))
            {
                // Talisman of Troll Divinity
                // For 20 seconds, direct healing adds a stack of 58 +healing for 10 seconds
                // Stacks 5 times, 2 minute cd
                // Direct heals: Nourish (1.5) HT (3) Regrowth (2)
                // Assumption: every 2 seconds, a direct healing spell is cast, so after 5 casts, full effect
                // That would mean: 10 seconds ramping up, then 20 seconds having the effect (assuming the stack is refreshed)
                // Average stack of 4 (24/30 * 5)
                // But remember that the spellpower will increase for others in the raid too!
                // stats.BonusHealingReceived = 58 * 4;                
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BonusHealingReceived = 58 * 4f }, 30, 120));
            }
            /*else if (line.StartsWith("Your heals each cost "))
            {
                // Lower City Prayerbook
                Regex r = new Regex("Your heals each cost (?<mana>\\d*) less mana for the next 15 sec");
                Match m = r.Match(line);
                if (m.Success)
                {
                    stats.ManacostReduceWithin15OnUse1Min += (float)int.Parse(m.Groups["mana"].Value);
                }
            }*/
            else if (line.StartsWith("Gain 250 mana each sec for "))
            {
                //stats.ManaregenFor8SecOnUse5Min += 250;
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Mp5 = 250 * 5 }, 8.0f, 300.0f));
            }
            else if (line.StartsWith("Conjures a Power Circle lasting for 15 sec.  While standing in this circle, the caster gains 320 spell power."))
            {
                // Shifting Naaru Sliver
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { SpellPower = 320 }, 15f, 90.0f));
            }
            else if ((match = Regex.Match(line, @"Each spell cast within 20 seconds will grant a stacking bonus of (?<mp5>\d+) mana regen per 5 sec.? Expires after (?<duration>\d+) seconds.*")).Success)
            {
                int mp5 = int.Parse(match.Groups["mp5"].Value);
                // Meteorite Crystal and Pendant of the Violet Eye
                // Estimate cast as every 2.0f seconds, average stack height is 0.5f of final value
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Mp5 = mp5 * int.Parse(match.Groups["duration"].Value) * 0.5f / 2.0f }, int.Parse(match.Groups["duration"].Value), 120));
            }
            else if ((match = Regex.Match(line, @"Each time you cast a helpful spell, you gain (?<amount>\d+) spell power. *Stacks up to (?<stacks>\d+) times. *Entire effect lasts (?<duration>\d+) sec.*")).Success)
            {   // Binding Light / Stone
                SpecialEffect primary = new SpecialEffect(Trigger.Use, new Stats(),
                    (float)int.Parse(match.Groups["duration"].Value), 2f * 60f);
                SpecialEffect secondary = new SpecialEffect(Trigger.HealingSpellCast,
                    new Stats() { SpellPower = (float)int.Parse(match.Groups["amount"].Value) },
                    (float)int.Parse(match.Groups["duration"].Value),
                    0f, 1f, int.Parse(match.Groups["stacks"].Value));
                primary.Stats.AddSpecialEffect(secondary);
                stats.AddSpecialEffect(primary);
            }
            else if ((match = Regex.Match(line,
@"Each time you cast a harmful spell, you gain (?<hasteRating>\d+) haste rating.? .*Stacks up to (?<stacks>\d+) times.? .*Entire effect lasts (?<duration>\d+) sec.?( \((?<cooldown>\d+) Min Cooldown\))?")).Success)
//@"Each time you cast a harmful spell, you gain 64                  haste rating.? Stacks up to 8 times. Entire effect lasts 20 sec (2 Min Cooldown)"
            {
                // Fetish of Volatile Power
                // armory doesn't give cooldown info
                int cooldown = 120;
                if (match.Groups["cooldown"].Success)
                {
                    cooldown = int.Parse(match.Groups["cooldown"].Value) * 60;
                }
                Stats childEffect = new Stats();
                childEffect.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast, new Stats() { HasteRating = int.Parse(match.Groups["hasteRating"].Value) }, float.PositiveInfinity, 0f, 1f, int.Parse(match.Groups["stacks"].Value)));
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, childEffect, int.Parse(match.Groups["duration"].Value), cooldown, 1f));
            }
            // Wowhead: "Each time you are struck by an attack, you gain 1265 armor. Stacks up to 5 times. Entire effect lasts 20 sec (2 Min Cooldown)"
            // Armory:  "Each time you are struck by an attack, you gain 1265 armor. Stacks up to 5 times. Entire effect lasts 20 sec."
            else if ((match = Regex.Match(line, @"Each time you are struck by an attack, you gain (?<bonusArmor>\d+) armor\.\s+Stacks up to (?<stacks>\d+) times\.\s+Entire effect lasts (?<duration>\d+) sec(\.\s+\((?<cooldown>\d+) Min Cooldown\))?")).Success)
            {
                // Fervor of the Frostborn / Eitrigg's Oath
                // armory doesn't give cooldown info
                int cooldown = 120;
                if (match.Groups["cooldown"].Success)
                {
                    cooldown = int.Parse(match.Groups["cooldown"].Value) * 60;
                }
                Stats childEffect = new Stats();
                childEffect.AddSpecialEffect(new SpecialEffect(Trigger.DamageTaken, new Stats() { BonusArmor = int.Parse(match.Groups["bonusArmor"].Value) }, float.PositiveInfinity, 0f, 1f, int.Parse(match.Groups["stacks"].Value)));
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, childEffect, int.Parse(match.Groups["duration"].Value), cooldown, 1f));
            }
            // Figurine - Talasite Owl, 5 min cooldown
            else if (line.StartsWith("Restores 900 mana over 12 sec."))
            {
                //if (stats.Mp5 == 18) // Figurine - Seaspray Albatross, 3 min cooldown
                //    stats.ManaregenOver12SecOnUse3Min += 900;
                //else if (stats.Mp5 == 14) // Figurine - Talasite Owl, 5 min cooldown
                //    stats.ManaregenOver12SecOnUse5Min += 900;
            }
            else if ((match = new Regex(@"Increases the block value of your shield by (?<amount>\d\d*) for 20 sec.").Match(line)).Success)
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BlockValue = (float)int.Parse(match.Groups["amount"].Value) }, 20.0f, 120.0f));
            }
            /*else if (line.StartsWith("Your heals each cost "))
            {
                // Lower City Prayerbook
                Regex r = new Regex("Your heals each cost (?<mana>\\d*) less mana for the next 15 sec");
                Match m = r.Match(line);
                if (m.Success)
                {
                    stats.ManacostReduceWithin15OnUse1Min += (float)int.Parse(m.Groups["mana"].Value);
                }
            }*/
            else if (line.StartsWith("Tap into the power of the skull, increasing haste rating by 175 for 20 sec"))
            {
                // The Skull of Gul'dan
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { HasteRating = 175 }, 20.0f, 120.0f));
            }
            else if (line.StartsWith("Each spell cast within 20 seconds will grant a stacking bonus of 21 mana regen per 5 sec. Expires after 20 seconds.  Abilities with no mana cost will not trigger this trinket.")
                  || line.StartsWith("Each spell cast within 20 seconds will grant a stacking bonus of 21 mana regen per 5 sec Expires after 20 seconds.  Abilities with no mana cost will not trigger this trinket."))
            {
                //stats.Mp5OnCastFor20SecOnUse2Min += 21;
            }
            // Mind Quickening Gem
            else if (line.StartsWith("Quickens the mind, increasing the Mage's haste rating by 330 for 20 sec"))
            {
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { HasteRating = 330 }, 20, 300));
            }
//			else if (line.StartsWith("Increases the block value of your shield by 200 for 20 sec"))
//			{
//				stats.BlockValue += (float)Math.Round(200f * (20f / 120f));
//			}
            else if (line.StartsWith("Removes all movement impairing effects and all effects which cause loss of control of your character."))
            {
                stats.PVPTrinket += 1f;
            }
            else if (line.StartsWith("Increases the damage dealt by your Scourge Strike and Obliterate abilities by 420."))
            {
                stats.BonusObliterateDamage += 420;
                stats.BonusScourgeStrikeDamage += 420;
            }
            else if (line.StartsWith("Restores 2340 mana over 12 sec"))
            {
                // Figurine - Sapphire Owl
                //stats.ManaRestore5min = 2340;
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { ManaRestore = 2340 }, 0f, 300f));
            }
            else if (line == "Instantly heal your current friendly target for 2710. (1 Min Cooldown)")
            {
                // Living Ice Crystals
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Healed = 2710 }, 0f, 60f));
            }
            else if ((match = new Regex(@"Grants (?<amount>\d\d*) haste rating for 20 sec").Match(line)).Success) {
                // Ephemeral Snowflake
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { HasteRating = (float)int.Parse(match.Groups["amount"].Value) }, 20f, 120f));
            }
            else if ((match = new Regex(@"Restores (?<amount>\d+) mana.").Match(line)).Success)
            {
                // Sliver of Pure Ice
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { ManaRestore = (float)int.Parse(match.Groups["amount"].Value) }, 0f, 120f));
            }
            else if ((match = Regex.Match(line, @"Every time one of your non-periodic spells deals a critical strike, the bonus is reduced by 184 critical strike rating.")).Success)
            {
                // Nevermelting Ice Crystal - Yes when armory is fixed this needs update
                Stats childEffect = new Stats();
                childEffect.CritRating = 920f;
                childEffect.AddSpecialEffect(new SpecialEffect(Trigger.SpellCrit, new Stats() { CritRating =-184f }, float.PositiveInfinity, 0f, 1f, 5));
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, childEffect, 20f, 180f, 1f));
            }
            else if ((match = new Regex(@"Generates a bolt of lightning to strike an enemy for (?<min>\d\d*) to (?<max>\d\d*) Nature damage*").Match(line)).Success)
            {   // Gnomish Lightning Generator
                int min = int.Parse(match.Groups["min"].Value);
                int max = int.Parse(match.Groups["max"].Value);
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                    new Stats() { NatureDamage = (float)(min + max) / 2f, },
                    0f, 60f, 1f));
            }
            // TODO: Update this so it can have a wider use.
            else if ((match = new Regex(@"Absorbs 6400 damage").Match(line)).Success)
            {   
                // Corroded Skeleton Key
                // Absorbs 6400 damage.  Lasts 10 sec. (2 Min Cooldown)
                float DamageAbsorb = 6400;
                float fDuration = 10;
                float fCD = 120; // 2 min * 60 sec
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                    new Stats() { DamageAbsorbed = DamageAbsorb },
                    fDuration, fCD));
            }
            else if ((match = new Regex(@"Increases resistance to Arcane, Fire, Frost, Nature, and Shadow spells by (?<amount>\d\d*) for (?<duration>\d\d*) sec.").Match(line)).Success)
            {
                // Sindragosa's Flawless Fang
                // Increases resistance to Arcane, Fire, Frost, Nature, and Shadow spells by 239 for 10 sec. (1 Min Cooldown)
                int amount = int.Parse(match.Groups["amount"].Value);
                float fDuration = float.Parse(match.Groups["duration"].Value);
                float fCD = 60; // 1 min * 60 sec
                stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                    new Stats() { ArcaneResistance = amount,
                    FireResistance = amount,
                    FrostResistance = amount, 
                    NatureResistance = amount,
                    ShadowResistance = amount,
                    },
                    fDuration, fCD));
            } else if ((match = Regex.Match(line, @"For the next 15 sec, each time your direct healing spells heal a target you cause the target of your heal to heal themselves and friends within 10 yards for (?<amount>\d+) each sec for (?<seconds>\d+) sec")).Success) {
                // Glowing Twilight Scale proc (Eyes of Twilight); Procs on Healing Hit, NOT HoTs; Hits average of 5 people;
                int amount = int.Parse(match.Groups["amount"].Value);
                int seconds = int.Parse(match.Groups["seconds"].Value);
                SpecialEffect primary = new SpecialEffect(Trigger.Use, new Stats(), 15f, 2f * 60f);
                SpecialEffect secondary = new SpecialEffect(Trigger.HealingSpellCast, new Stats() { Healed = amount * 5 }, seconds, 0f);
                primary.Stats.AddSpecialEffect(secondary);
                stats.AddSpecialEffect(primary);
            }
        }

        #region Specialized Items that need Extra Handling
        #region DeathBringer's Will (264 & 277)
        /// <summary>
        /// Special Handler for Deathbringer's Will to be call from INSIDE the model.
        /// </summary>
        /// <param name="Class">The class of the character</param>
        /// <param name="value">The current stats.DeathBringerProc value</param>
        /// <returns>List of Special Effects relevant to your class. Will be a list of 3 items or 0 if passing an invalid class.</returns>
        public static List<SpecialEffect> GetDeathBringerEffects(CharacterClass Class, float value) {
            List<SpecialEffect> retVal = new List<SpecialEffect>();
            float dur = 30f, cd = 105 * 3, ch = 0.15f;

            SpecialEffect procSTR   = new SpecialEffect(Trigger.PhysicalHit, new Stats { Strength               = value }, dur, cd, ch);
            SpecialEffect procCrit  = new SpecialEffect(Trigger.PhysicalHit, new Stats { CritRating             = value }, dur, cd, ch);
            SpecialEffect procArP   = new SpecialEffect(Trigger.PhysicalHit, new Stats { ArmorPenetrationRating = value }, dur, cd, ch);
            SpecialEffect procHaste = new SpecialEffect(Trigger.PhysicalHit, new Stats { HasteRating            = value }, dur, cd, ch);
            SpecialEffect procAGI   = new SpecialEffect(Trigger.PhysicalHit, new Stats { Agility                = value }, dur, cd, ch);
            SpecialEffect procAP    = new SpecialEffect(Trigger.PhysicalHit, new Stats { AttackPower            = value * 2 }, dur, cd, ch);

            switch (Class) {
                case CharacterClass.Warrior:     retVal.Add(procSTR); retVal.Add(procArP);   retVal.Add(procCrit);  break;
                case CharacterClass.Rogue:       retVal.Add(procAGI); retVal.Add(procArP);   retVal.Add(procAP);    break;
                case CharacterClass.Paladin:     retVal.Add(procSTR); retVal.Add(procHaste); retVal.Add(procCrit);  break;
                case CharacterClass.Hunter:      retVal.Add(procAP);  retVal.Add(procAGI);   retVal.Add(procCrit);  break;
                case CharacterClass.DeathKnight: retVal.Add(procSTR); retVal.Add(procCrit);  retVal.Add(procHaste); break;
                case CharacterClass.Druid:       retVal.Add(procArP); retVal.Add(procSTR);   retVal.Add(procAGI);   break;
                case CharacterClass.Shaman:      retVal.Add(procAGI); retVal.Add(procAP);    retVal.Add(procArP);   break;
                default: break; // None
            }

            return retVal;
        }
        #endregion
        #endregion

        #region EvalRegex
        public static SpecialEffect EvalRegex(string statName, float amount, float duration, string ability, float cooldown) { return EvalRegex(statName, amount, duration, ability, cooldown, 1f); }

        /// <summary>
        /// For those objects that have a special effect trigger (As opposed to just straight stat upgrades)
        /// This allows you to pass in a statName, amount, duration, trigger type, and cooldown and get a SpecialEffect back.
        /// </summary>
        /// <param name="statName">What stat does the special effect target</param>
        /// <param name="amount">By what amount?</param>
        /// <param name="duration">How long in seconds?</param>
        /// <param name="ability">What is the spell triggering? For right now, this only effects the Sigils.  So we're going to use the ability to setup the trigger.</param>
        /// <param name="cooldown">How long in seconds?</param>
        /// <param name="chance">What is the percent chance? (0-1)</param>
        /// <returns>A new SpecialEffect instance that can be used in Stats.AddSpecialEffect()</returns>
        public static SpecialEffect EvalRegex(string statName, float amount, float duration, string ability, float cooldown, float chance)
        {
            Stats s = new Stats();

            if (statName.Equals("attack power", StringComparison.InvariantCultureIgnoreCase)) { s.AttackPower = amount; }
            else if (statName.Equals("agility", StringComparison.InvariantCultureIgnoreCase)) { s.Agility = amount; }
            else if (statName.Equals("armor", StringComparison.InvariantCultureIgnoreCase)) { s.BonusArmor = amount; }
            else if (statName.Equals("armor penetration rating", StringComparison.InvariantCultureIgnoreCase)) { s.ArmorPenetrationRating = amount; }
            else if (statName.Equals("block value", StringComparison.InvariantCultureIgnoreCase)) { s.BlockValue = amount; }
            else if (statName.Equals("critical strike rating", StringComparison.InvariantCultureIgnoreCase)) { s.CritRating = amount; }
            else if (statName.Equals("defense rating", StringComparison.InvariantCultureIgnoreCase)) { s.DefenseRating = amount; }
            else if (statName.Equals("dodge", StringComparison.InvariantCultureIgnoreCase)) { s.DodgeRating = amount; }
            else if (statName.Equals("dodge rating", StringComparison.InvariantCultureIgnoreCase)) { s.DodgeRating = amount; }
            else if (statName.Equals("melee and ranged attack power", StringComparison.InvariantCultureIgnoreCase)) { s.AttackPower = amount; }
            else if (statName.Equals("haste rating", StringComparison.InvariantCultureIgnoreCase)) { s.HasteRating = amount; }
            else if (statName.Equals("maximum health", StringComparison.InvariantCultureIgnoreCase)) { s.Health = amount; }
            else if (statName.Equals("parry", StringComparison.InvariantCultureIgnoreCase)) { s.ParryRating = amount; }
            else if (statName.Equals("parry rating", StringComparison.InvariantCultureIgnoreCase)) { s.ParryRating = amount; }
            else if (statName.Equals("spell power", StringComparison.InvariantCultureIgnoreCase)) { s.SpellPower = amount; }
            else if (statName.Equals("spirit", StringComparison.InvariantCultureIgnoreCase)) { s.Spirit = amount; }
            else if (statName.Equals("strength", StringComparison.InvariantCultureIgnoreCase)) { s.Strength = amount; }
            Trigger trigger = new Trigger();

            switch (ability)
            {
                case "Icy Touch":
                    trigger = Trigger.IcyTouchHit;
                    break;
                case "Plague Strike":
                    trigger = Trigger.PlagueStrikeHit;
                    break;
                case "Rune Strike":
                    trigger = Trigger.RuneStrikeHit;
                    break;
                case "Blood Strike":
                    trigger = Trigger.BloodStrikeHit;
                    break;
                case "Heart Strike":
                case "Heart Strikes":
                    trigger = Trigger.HeartStrikeHit;
                    break;
                case "Obliterate":
                    trigger = Trigger.ObliterateHit;
                    break;
                case "Scourge Strike":
                    trigger = Trigger.ScourgeStrikeHit;
                    break;
                case "Death Strike":
                    trigger = Trigger.DeathStrikeHit;
                    break;
                default:
                    trigger = Trigger.SpellHit;
                    break;
            }

            return new SpecialEffect(trigger, s, duration, cooldown, chance);

        }

        /// <summary>
        /// For those objects that have a special effect trigger (As opposed to just straight stat upgrades)
        /// This allows you to pass in a statName, amount, duration, trigger type, and cooldown and get a SpecialEffect back.
        /// </summary>
        /// <param name="statName">What stat does the special effect target</param>
        /// <param name="amount">By what amount?</param>
        /// <param name="duration">How long in seconds?</param>
        /// <param name="ability">What is the spell triggering? For right now, this only effects the Sigils.  So we're going to use the ability to setup the trigger.</param>
        /// <param name="cooldown">How long in seconds?</param>
        /// <param name="chance">What is the percent chance? (0-1)</param>
        /// <param name="stacks"> How many stacks?</param>
        /// <returns>A new SpecialEffect instance that can be used in Stats.AddSpecialEffect()</returns>
        public static SpecialEffect EvalRegex(string statName, float amount, float duration, string ability, float cooldown, float chance, int stacks)
        {
            Stats s = new Stats();

            if (statName.Equals("attack power", StringComparison.InvariantCultureIgnoreCase)) { s.AttackPower = amount; }
            else if (statName.Equals("agility", StringComparison.InvariantCultureIgnoreCase)) { s.Agility = amount; }
            else if (statName.Equals("armor", StringComparison.InvariantCultureIgnoreCase)) { s.BonusArmor = amount; }
            else if (statName.Equals("armor penetration rating", StringComparison.InvariantCultureIgnoreCase)) { s.ArmorPenetrationRating = amount; }
            else if (statName.Equals("block value", StringComparison.InvariantCultureIgnoreCase)) { s.BlockValue = amount; }
            else if (statName.Equals("critical strike rating", StringComparison.InvariantCultureIgnoreCase)) { s.CritRating = amount; }
            else if (statName.Equals("defense rating", StringComparison.InvariantCultureIgnoreCase)) { s.DefenseRating = amount; }
            else if (statName.Equals("dodge", StringComparison.InvariantCultureIgnoreCase)) { s.DodgeRating = amount; }
            else if (statName.Equals("dodge rating", StringComparison.InvariantCultureIgnoreCase)) { s.DodgeRating = amount; }
            else if (statName.Equals("melee and ranged attack power", StringComparison.InvariantCultureIgnoreCase)) { s.AttackPower = amount; }
            else if (statName.Equals("haste rating", StringComparison.InvariantCultureIgnoreCase)) { s.HasteRating = amount; }
            else if (statName.Equals("maximum health", StringComparison.InvariantCultureIgnoreCase)) { s.Health = amount; }
            else if (statName.Equals("parry", StringComparison.InvariantCultureIgnoreCase)) { s.ParryRating = amount; }
            else if (statName.Equals("parry rating", StringComparison.InvariantCultureIgnoreCase)) { s.ParryRating = amount; }
            else if (statName.Equals("spell power", StringComparison.InvariantCultureIgnoreCase)) { s.SpellPower = amount; }
            else if (statName.Equals("spirit", StringComparison.InvariantCultureIgnoreCase)) { s.Spirit = amount; }
            else if (statName.Equals("strength", StringComparison.InvariantCultureIgnoreCase)) { s.Strength = amount; }
            Trigger trigger = new Trigger();

            switch (ability)
            {
                case "Icy Touch":
                    trigger = Trigger.IcyTouchHit;
                    break;
                case "Plague Strike":
                    trigger = Trigger.PlagueStrikeHit;
                    break;
                case "Rune Strike":
                    trigger = Trigger.RuneStrikeHit;
                    break;
                case "Blood Strike":
                    trigger = Trigger.BloodStrikeHit;
                    break;
                case "Heart Strike":
                case "Heart Strikes":
                    trigger = Trigger.HeartStrikeHit;
                    break;
                case "Obliterate":
                    trigger = Trigger.ObliterateHit;
                    break;
                case "Scourge Strike":
                    trigger = Trigger.ScourgeStrikeHit;
                    break;
                case "Death Strike":
                    trigger = Trigger.DeathStrikeHit;
                    break;
                default:
                    trigger = Trigger.SpellHit;
                    break;
            }

            return new SpecialEffect(trigger, s, duration, cooldown, chance, stacks);

        }

        /// <summary>
        /// Using the Regex functions above, update the passed in stat value of a given statName by the amount appropriate.
        /// </summary>
        /// <param name="s">the Stats instance to be updated.</param>
        /// <param name="statName">The stat to update.</param>
        /// <param name="amount">The amount to update the stat by.</param>
        public static void EvalRegex(Stats s, string statName, float amount) {
            if (statName.Equals("attack power", StringComparison.InvariantCultureIgnoreCase)) { s.AttackPower = amount; }
            else if (statName.Equals("agility", StringComparison.InvariantCultureIgnoreCase)) { s.Agility = amount; }
            else if (statName.Equals("armor", StringComparison.InvariantCultureIgnoreCase)) { s.BonusArmor = amount; }
            else if (statName.Equals("armor penetration rating", StringComparison.InvariantCultureIgnoreCase)) { s.ArmorPenetrationRating = amount; }
            else if (statName.Equals("block value", StringComparison.InvariantCultureIgnoreCase)) { s.BlockValue = amount; }
            else if (statName.Equals("critical strike rating", StringComparison.InvariantCultureIgnoreCase)) { s.CritRating = amount; }
            else if (statName.Equals("defense rating", StringComparison.InvariantCultureIgnoreCase)) { s.DefenseRating = amount; }
            else if (statName.Equals("dodge", StringComparison.InvariantCultureIgnoreCase)) { s.DodgeRating = amount; }
            else if (statName.Equals("dodge rating", StringComparison.InvariantCultureIgnoreCase)) { s.DodgeRating = amount; }
            else if (statName.Equals("melee and ranged attack power", StringComparison.InvariantCultureIgnoreCase)) { s.AttackPower = amount; }
            else if (statName.Equals("haste rating", StringComparison.InvariantCultureIgnoreCase)) { s.HasteRating = amount; }
            else if (statName.Equals("maximum health", StringComparison.InvariantCultureIgnoreCase)) { s.Health = amount; }
            else if (statName.Equals("parry", StringComparison.InvariantCultureIgnoreCase)) { s.ParryRating = amount; }
            else if (statName.Equals("parry rating", StringComparison.InvariantCultureIgnoreCase)) { s.ParryRating = amount; }
            else if (statName.Equals("spell power", StringComparison.InvariantCultureIgnoreCase)) { s.SpellPower = amount; }
            else if (statName.Equals("spirit", StringComparison.InvariantCultureIgnoreCase)) { s.Spirit = amount; }
        }

        /// <summary>
        /// Eval the Bonus damage values of given sigils and assign the bonus damage.
        /// This is specifically for DKs, but could be expanded as necessary.
        /// Hell, it probably could use a re-write once the whole Stats class is restructured.
        /// </summary>
        /// <param name="ability">What ability is having it's damage boosted?</param>
        /// <param name="s">The Stats instance to be updated.</param>
        /// <param name="amount">The amount by which the bonus should be applied.</param>
        public static void EvalAbility(string ability, Stats s, float amount) {
            switch (ability) {
                case "Blood Strike"  : { s.BonusBloodStrikeDamage   += amount; break; }
                case "Heart Strike"  : { s.BonusHeartStrikeDamage   += amount; break; }
                case "Death Coil"    : { s.BonusDeathCoilDamage     += amount; break; }
                case "Frost Strike"  : { s.BonusFrostStrikeDamage   += amount; break; }
                case "Obliterate"    : { s.BonusObliterateDamage    += amount; break; }
                case "Scourge Strike": { s.BonusScourgeStrikeDamage += amount; break; }
                case "Death Strike"  : { s.BonusDeathStrikeDamage   += amount; break; }
                case "Icy Touch"     : { s.BonusIcyTouchDamage      += amount; break; }
                default: { break; } // Error.
            }
        }
        #endregion
    }
}
