using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Rawr.Bosses {
    // ===== The Vault of Archavon ====================
    public class EmalonTheStormWatcher : MultiDiffBoss {
        public EmalonTheStormWatcher() {
            // If not listed here use values from defaults
            #region Info
            Name = "Emalon the Storm Watcher";
            Instance = "The Vault of Archavon";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 2789000f, 11156000f, 0, 0 };
            BerserkTimer = new int[] { 6 * 6, 19 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 1.00f, 1.00f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 2, 0, 0 };
            Min_Healers = new int[] { 3, 5, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
            }
            #endregion
            #endregion
            #region Defensive
            Resist_Physical = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Frost = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Fire = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Nature = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Arcane = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Shadow = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Holy = new double[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            #region Impedances
            for (int i = 0; i < 2; i++)
            {
                //Moves;
                // Every 45 seconds for 18 seconds dps has to be on the overcharged add (it wipes the raid at 20 sec)
                // Adding 5 seconds to the Duration for moving out before starts and then 5 for back in after
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = 45f + 18f,
                    Duration = (18f + 5f + 5f) * 1000f,
                    Chance = 1f,
                    Breakable = false,
                });
                // Lightning Nova, usually happens a few seconds after the overcharged add dies
                // (right when most melee reaches the boss again) Simming 4 to run out and 4 to get back
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = 45f + 18f,
                    Duration = (4f + 4f) * 1000f,
                    Chance = 1f,
                    Breakable = false,
                });
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
             * Adds Damage
             * Chain Lightning Damage
             * Lightning Nova Damage
             */
        }
    }
    // ===== Ulduar ===================================
    /* JOTHAY NOTE: Most of these bosses were duplicated off
     * Auriya so they have a whole lot of values that they
     * aren't supposed to. They need to be set up before I
     * make 25 Man versions.*/
    // The Siege
    public class IgnistheFurnaceMaster : MultiDiffBoss {
        public IgnistheFurnaceMaster() {
            // If not listed here use values from defaults
            #region Info
            Name = "Ignis the Furnace Master";
            Instance = "Ulduar";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 5578000f, 6763325f, 0, 0 };
            BerserkTimer = new int[] { 19 * 60, 19 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.90f, 0.90f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 3, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                {
                    /* Flame Jets - Inflicts 5,655 to 6,345 (Heroic: 8,483 to 9,517)
                     *      Fire damage and interrupts spellcasting for 6 seconds. Affected
                     *      targets take additional 1,000 (Heroic: 2,000) Fire damage
                     *      every 1 second, for 6 seconds.
                     */
                    Attack a = new Attack()
                    {
                        Name = "Flame Jets",
                        AttackSpeed = 25f,
                        AttackType = ATTACK_TYPES.AT_AOE,
                        DamagePerHit = (5655f - 6345f) / 2f,
                        DamageType = ItemDamageType.Fire,
                        MaxNumTargets = this[i].Max_Players,
                    };
                   this[i].Attacks.Add(a);
                }
                {
                    /* Scorch - Inflicts 2,357 to 2,643 (Heroic: 3,770 to 4,230)
                     *  Fire damage every half-second to targets in front of the caster
                     *  within 30 yards. In addition, the ground within 13 yards becomes
                     *  scorched, inflicting 1,885 to 2,115 (Heroic: 3,016 to 3,384)
                     *  Fire damage every 1 second. Iron Constructs gain Heat while
                     *  standing on scorched ground.
                     */
                    Attack a = new Attack()
                    {
                        Name = "Scorch",
                        AttackSpeed = 25f,
                        AttackType = ATTACK_TYPES.AT_AOE,
                        DamagePerHit = (2357f - 2643f) / 2f,
                        DamageType = ItemDamageType.Fire,
                        MaxNumTargets = this[i].Max_Players,
                    };
                    this[i].Attacks.Add(a);
                    this[i].Moves.Add(new Impedance()
                    {
                        Frequency = a.AttackSpeed,
                        Duration = 5f * 1000f,
                        Chance = 1f,
                        Breakable = false,
                    });
                }
            }
            #endregion
            #endregion
            #region Defensive
            Resist_Physical = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Frost = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Fire = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Nature = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Arcane = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Shadow = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Holy = new double[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            #region Impedances
            for (int i = 0; i < 2; i++)
            {
                //Moves;
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            // TODO:
            /* Slag Pot - Charges and grabs a random target and incapacitates them,
             *   inflicting 4,500 (Heroic: 6,000) Fire damage every 1 second for 10
             *   seconds. If the target survives, they will gain 100% (Heroic: 150%)
             *   haste for 10 seconds.
             * Iron Constructs - Iron Constructs brought to ground scorched by
             *   Scorch, will gain stackable Heat buff, which increases their movement
             *   speed and haste by 5 per application. If Iron Constructs gain 10
             *   stacks of Heat, they will become Molten, increasing their haste by
             *   100% and inflicting 1,885 to 2,115 Fire damage every 1 second to
             *   enemies within 7 yards. Molten Iron Constructs can be brought to
             *   water, which causes them to become Brittle and unable to perform any
             *   action for 20 seconds. Critical strike chance against Brittle Iron
             *   Constructs is increased by 50%. If a single attack deals more than
             *   5,000 damage to a Brittle Iron Construct, it will Shatter, inflicting
             *   18,850 to 21,150 Physical damage not mitigated by armor, to enemies
             *   within 10 yards. Shattering the Construct causes Ignis to lose a
             *   stack of Strength of the Creator.
             * Strength of the Creator - For every Iron Construct active and alive,
             *   Ignis gains 15% damage, stacking up to 99 times.
             */
        }
    }
    public class Razorscale : MultiDiffBoss {
        public Razorscale() {
            // If not listed here use values from defaults
            #region Info
            Name = "Razorscale";
            Instance = "Ulduar";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 3555975f, 6763325f, 0, 0 };
            BerserkTimer = new int[] { 15 * 60, 15 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.90f, 0.90f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 3, 0, 0 };
            Min_Healers = new int[] { 3, 4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 5, 5, 0, 0 };// need to drop this down to only when the swarm is up
            //MultiTargsPerc = new double[] { 0.50d, 0.50d, 0.00d, 0.00d };// need to sim this out
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
            }
            #endregion
            #endregion
            #region Defensive
            Resist_Physical = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Frost = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Fire = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Nature = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Arcane = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Shadow = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Holy = new double[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            #region Impedances
            for (int i = 0; i < 2; i++)
            {
                //Moves;
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            // TODO:
            /* Phase 1: Air Phase
             * 1. (Spell #63014) - Inflicts 5,088 to 5,912 (Heroic: 7,863 to 9,137) Fire damage to a player, and additional 5,088 to 5,912 (Heroic: 7,863 to 9,137) Fire damage every 1 second to anyone within 6 yards of the initial impact, for 25 seconds.
             * 2. Fireball - Inflicts 6,660 to 7,740 (Heroic: 9,713 to 11,287) Fire damage.
             * 3. Flame Breath - Inflicts 13,125 to 16,875 (Heroic: 17,500 to 22,500) Fire damage to enemies in a cone in front of the caster. Used just before Razorscale goes back to the air.
             * 4. Wing Buffet - Knocks back enemies in 35 yards radius around the caster. Used right after Flame Breath.
             * 5. Dark Rune Adds - Adds will come in waves from mole machines. One mole can spawn a Dark Rune Watcher with 1-2 Guardians, or a lone Sentinel. Up to 4 mole machines can spawn adds at any given time.
             *     1. Dark Rune Watchers - Cast Lightning Bolt, inflicting 5,950 to 8,050 (Heroic: 8,075 to 10,925) Nature damage and Chain Lightning, inflicting 5,088 to 5,912 (Heroic: 8,788 to 10,212)
             *     2. Dark Rune Sentinels - Cast Whirlwind, hitting for ~9,000 Physical damage on plate.
             *     3. Dark Rune Guardians - Cast Stormstrike, which increases the Nature damage done to the target by 20% for 12 seconds.
             * Phase 2: Ground Phase
             * 1. Flame Buffet - Increases the Fire damage an enemy takes by 1,000 (Heroic: 1,500) for 1 minute, stacking up to 99 times.
             * 2. Wing Buffet - Knocks back enemies in 35 yards radius around the caster. Used in the beginning of the phase.
             * 3. Fuse Armor - Reduces armor, attack and movement speed by 20% for 20 seconds, stacking up to 5 times. 5 stacks of Fused Armor will stun the tank for several seconds.
             * 4. Flame Breath - Inflicts 13,125 to 16,875 (Heroic: 17,500 to 22,500) Fire damage to enemies in a cone in front of the caster. Affected by Flame Buffet.
             * 5. (Spell #63014) - Inflicts 5,088 to 5,912 (Heroic: 7,863 to 9,137) Fire damage to a player, and additional 5,088 to 5,912 (Heroic: 7,863 to 9,137) Fire damage every 1 second to anyone within 6 yards of the initial impact, for 25 seconds. Affected by Flame Buffet.
             */
        }
    }
    public class XT002Deconstructor : MultiDiffBoss {
        public XT002Deconstructor() {
            // If not listed here use values from defaults
            #region Info
            Name = "XT-002 Deconstructor";
            Instance = "Ulduar";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 5000008f, 6763325f, 0, 0 };
            BerserkTimer = new int[] { 10 * 60, 10 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 1.00f, 1.00f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 3, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                {
                    /* Gravity Bomb - Causes the target to spawn a Gravity Bomb
                     * after 9 seconds, which damages the target for 12,000
                     * (Heroic: 15,000) and pulls other players within 12 yards
                     * and inflicts them with 11,700 to 12,300 (Heroic: 14,625
                     * to 15,375) Shadow damage.*/
                    Attack a = new Attack
                    {
                        Name = "Gravity Bomb",
                        DamageType = ItemDamageType.Shadow,
                        DamagePerHit = 12000f,
                        MaxNumTargets = this[i].Max_Players,
                        AttackSpeed = 25,
                    };
                    this[i].Attacks.Add(a);
                    this[i].Moves.Add(new Impedance()
                    {
                        Frequency = a.AttackSpeed,
                        Duration = (9f + 5f) * 1000f,
                        Chance = 1f / this[i].Max_Players,
                        Breakable = false,
                    });
                }
                {
                    /* Searing Light - Causes the target to inflict 3,000
                     * (Heroic: 3,500) damage to the target and other players
                     * within 10 yards every 1 second, for 9 seconds.*/
                    Attack a = new Attack
                    {
                        Name = "Searing Light",
                        DamageType = ItemDamageType.Holy,
                        DamagePerHit = 3000f,
                        MaxNumTargets = this[i].Max_Players,
                        AttackSpeed = 25,
                    };
                    this[i].Attacks.Add(a);
                    this[i].Moves.Add(new Impedance()
                    {
                        Frequency = a.AttackSpeed,
                        Duration = (9f + 5f) * 1000f,
                        Chance = 1f / this[i].Max_Players,
                        Breakable = false,
                    });
                }
                {
                    /* Tympanic Tantrum - Damages everybody for 10% of their
                     * maximum hit points every 1 second, for 8 seconds. This
                     * is a channeled spell, which also dazes the affected
                     * targets for 2 seconds.*/
                    Attack a = new Attack
                    {
                        Name = "Tympanic Tantrum",
                        DamageType = ItemDamageType.Physical,
                        DamagePerHit = 22000f * 0.10f * 8f,
                        MaxNumTargets = this[i].Max_Players,
                        AttackSpeed = 25,
                    };
                    this[i].Attacks.Add(a);
                }
            }
            #endregion
            #endregion
            #region Defensive
            Resist_Physical = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Frost = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Fire = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Nature = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Arcane = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Shadow = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Holy = new double[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            #region Impedances
            for (int i = 0; i < 2; i++)
            {
                //Moves;
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
             * 
             * Heart Phase: At 75%, 50%, and 25% XT's Heart of the
             * Deconstructor will become exposed and attackable. It
             * will leak energy, which summons adds - Pummellers,
             * Scrapbots and Boombots. During this 30-second Phase XT
             * will take 100% extra damage through his heart and won't
             * attack.
             * o XM-024 Pummellers - Attack with Arcing Smash, Trample,
             *   and Uppercut.
             * o XS-013 Scrapbots - Run to XT and cause him to restore
             *   1% health per Scrapbot, through Scrap Repair.
             * o XE-321 Boombots - Explode on death, inflicting damage
             *   to nearby players and NPCs.*/
        }
    }
    // The Antechamber
    public class AssemblyofIron : MultiDiffBoss {
        public AssemblyofIron() {
            // If not listed here use values from defaults
            #region Info
            Name = "Assembly of Iron";
            Instance = "Ulduar";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 2998175f, 6763325f, 0, 0 };
            BerserkTimer = new int[] { 15 * 60, 15 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.75f, 0.75f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 3, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
            }
            #endregion
            #endregion
            #region Defensive
            Resist_Physical = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Frost = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Fire = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Nature = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Arcane = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Shadow = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Holy = new double[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            #region Impedances
            for (int i = 0; i < 2; i++)
            {
                //Moves;
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            // TODO:
            /* Steelbreaker
             * - High Voltage - An aura that inflicts 1,500 (Heroic: 3,000)
             *   to the whole raid every 3 seconds.
             * - Fusion Punch - Inflicts 18,850 to 21,150 (Heroic: 35,000)
             *   Nature damage to his target, and deals additional 15,000
             *   (Heroic: 20,000) Nature damage every 1 second, for 4 seconds.
             * - [1 Supercharge] Static Disruption - Inflicts 3,500 (Heroic:
             *   7,000) Nature damage to enemies in 6 yards radius area. Also
             *   increases Nature damage taken by 25% (Heroic: 75%) for 20
             *   seconds.
             * - [2 Supercharges] Overwhelming Power - Increases the size and
             *   damage done by his target by 200%, and affects it with
             *   Meltdown after 60 (Heroic: 30) seconds.
             * - [2 Supercharges] Meltdown - Causes the target to die, dealing
             *   29,250 to 30,750 Nature damage to friendly units within 15
             *   yards.
             * - [2 Supercharges] Electrical Charge - Increases Steelbreaker's
             *   damage by 25% and heals him for 20% of his maximum hit points
             *   whenever a player or a pet dies. Affected by healing
             *   reduction effects.*/
            /* Runemaster Molgeim
             * - Shield of Runes - Absorbs 20,000 (Heroic: 50,000) damage.
             *   When the Shield is broken by damage, it increases Molgeim's
             *   damage by 50% for 15 seconds. Using Spellsteal bypasses the
             *   damage increase.
             * - Rune of Power - Summons a Rune of Power under the feet of a
             *   random target. Rune of Power increases the damage of everybody
             *   who stands on it by 50%. Lasts 1 minute.
             * - [1 Supercharge] Rune of Death - Summons a Rune of Death
             *   under the feet of a random player. Rune of Death inflicts
             *   2,750 (Heroic: 3,500) Shadow damage to players within 13
             *   yards of it, every half-second for 30 seconds
             * - [2 Supercharges] Rune of Summoning - Summons a Rune of
             *   Summoning near a random player. Rune of Summoning periodically
             *   spawns Lightning Elementals, which will rush towards random
             *   players and cast Lightning Blast.
             * - [2 Supercharges] Lightning Blast - Inflicts 9,425 to 10,575
             *   (Heroic: 14,138 to 15,862) Nature damage to all players within
             *   30 yards of the caster. The Lightning Elemental dies after
             *   the spell goes off.*/
            /* Stormcaller Brundir
             * - Chain Lightning - Inflicts 4,163 to 4,837 (Heroic: 5,550 to
             *   6,450) Nature damage to its main target, and arcs to nearby
             *   players. Interruptible.
             * - Overload - Inflicts 20,000 (Heroic: 25,000) Nature damage to
             *   players within 30 yards of the caster, after 6 seconds of
             *   channeling. Also applies knockback.
             * - [1 Supercharge] Lightning Whirl - Inflicts 3,770 to 4,230
             *   (Heroic: 5,655 to 6,345) Nature damage to random players
             *   every 1 second for 5 seconds. Interruptible.
             * - [2 Supercharges] Lightning Tendrils - Chooses a random
             *   target and starts flying towards it. Players near Brundir
             *   take 3,000 (Heroic: 5,000) Nature damage every 1 second
             *   until they get away. Brundir cannot be stunned or interrupted
             *   while casting Lightning Tendrils.*/
        }
    }
    public class Kologarn : MultiDiffBoss {
        public Kologarn() {
            // If not listed here use values from defaults
            #region Info
            Name = "Kologarn";
            Instance = "Ulduar";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 3625700f, 6763325f, 0, 0 };
            BerserkTimer = new int[] { 19 * 60, 19 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 1.00f, 1.00f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 3, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
            }
            #endregion
            #endregion
            #region Defensive
            Resist_Physical = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Frost = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Fire = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Nature = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Arcane = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Shadow = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Holy = new double[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            #region Impedances
            for (int i = 0; i < 2; i++)
            {
                //Moves;
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
            * Kologarn
                 1. Focused Eyebeam - Inflicts 2,313 to 2,687 (Heroic: 3,238 to 3,762) Nature damage every 1 second. Two beams will spawn around a player, following by a raid warning. The beams will converge onto the player and start inflicting damage after a few seconds. They will follow their target.
                 2. Overhead Smash - Inflicts physical damage and applies stacking Crunch Armor, which reduces the target's armor by 20% for 6 seconds (Heroic: 25% for 45 seconds).
                 3. One-Armed Overhead Smash - Inflicts less Physical damage than the normal one, but still applies stackable Crunch Armor, which reduces the target's armor by 20% for 6 seconds (Heroic: 25% for 45 seconds). Cast if one of Kologarn's Arms is dead.
                 4. Stone Shout - Inflicts 1,084 to 1,216 Physical damage to the entire raid every 1 second. Cast if both of Kologarn's Arms are dead.
                 5. Petrifying Breath - Inflicts 14,063 to 15,937 (Heroic: 18,750 to 21,250) Nature damage every 1 second, and applies Brittle Skin, which increases damage taken by 20% for 8 seconds. Cast if there are no targets in melee range.
            * Right Arm
                 1. Stone Grip - Grabs 1 (Heroic: 3) target, incapacitating them and inflicting 2,925 to 3,075 (Heroic: 3,413 to 3,587) Physical damage every 1 second, until they are freed. The Arm can sustain 100,000 (Heroic: 480,000) damage before it releases its target.
                 2. Death - Killing the Right Arm decreases Kologarn's health by 15% for 30 seconds, until the Arm respawns. It also spawns 5 Rubbles, that cast Stone Nova, which inflicts 5,550 to 6,450 Physical damage to targets within 10 yards of the caster, increasing damage taken by 5% for 10 sec. Rubbles are stunnable.
            * Left Arm
                 1. Shockwave - Inflicts 8,788 to 10,212 (Heroic: 13,875 to 16,125) Nature damage to the whole raid.
                 2. Death - Killing the Right Arm decreases Kologarn's health by 15% for 30 seconds, until the Arm respawns. It also spawns 5 Rubbles, that cast Stone Nova, which inflicts 5,550 to 6,450 Physical damage to targets within 10 yards of the caster, increasing damage taken by 5% for 10 sec. Rubbles are stunnable.
             */
        }
    }
    public class Auriaya : MultiDiffBoss {
        public Auriaya() {
            // If not listed here use values from defaults
            #region Info
            Name = "Auriaya";
            Instance = "Ulduar";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 3137625f, 16734000f, 0, 0 };
            BerserkTimer = new int[] { 10 * 60, 10 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.00f, 0.00f, 0, 0 };// This is a boss where you CANNOT be behind her or she Fubar's the raid
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 2, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                // She summons extra targets a lot, most of the time, they are within melee range of persons on the boss
                // Guardian Swarm: Marks a player and summons a pack of 10 Swarming Guardians with low health around them soon after.
                // Feral Defender: If you leave him alone, he's up about 90% of the fight
                //this[i].MultiTargsPerc = 0.90f; // need to sim this out
                //this[i].MaxNumTargets = 10f; // need to drop this down to only when the swarm is up
            }
            #endregion
            #endregion
            #region Defensive
            Resist_Physical = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Frost = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Fire = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Nature = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Arcane = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Shadow = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Holy = new double[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            #region Impedances
            for (int i = 0; i < 2; i++)
            {
                //Moves;
                //Stuns;
                //Fears;
                // Terrifying Screech: Raid-wide fear for 5 seconds. Magic effect.
                // Going to assume the CD is 45 sec for now (cuz I know she doesnt do it every 8 sec)
                this[i].Fears.Add(new Impedance()
                {
                    Frequency = 45f,
                    Duration = 5f * 1000f,
                    Chance = 1f,
                    Breakable = true,
                });
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
             */
        }
    }
    // The Keepers
    public class Mimiron : MultiDiffBoss {
        public Mimiron() {
            // If not listed here use values from defaults
            #region Info
            Name = "Mimiron";
            Instance = "Ulduar";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 1742400f*3f, 6763325f, 0, 0 };
            BerserkTimer = new int[] { 15 * 60, 15 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.50f, 0.50f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 3, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
            }
            #endregion
            #endregion
            #region Defensive
            Resist_Physical = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Frost = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Fire = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Nature = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Arcane = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Shadow = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Holy = new double[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            #region Impedances
            for (int i = 0; i < 2; i++)
            {
                //Moves;
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
            * Phase 1: Leviathan MK II
                 1. Proximity Mines - Drops 8-10 Proximity Mines around MK II. After a small arming time, the mines will use Explosion, inflicting 9,000 (Heroic: 12,000) Fire damage to anyone who walks over them.
                 2. Napalm Shell - Inflicts 7,540 to 8,460 (Heroic: 9,425 to 10,575) Fire damage instantly and an additional 4,000 (Heroic: 6,000) every 1 second, for 8 seconds. This spell will always attempt to hit targets farther than 15 yards of MK II.
                 3. Plasma Blast - After a 3-second cast, MK II will channel a stream of plasma for 6 seconds, inflicting 20,000 (Heroic: 30,000) irresistible Fire damage every 1 second to his target.
                 4. Shock Blast - After a 4-second cast, MK II will inflict 100,000 Nature damage to all enemies within 15 yards of him. Used soon after a Plasma Blast.
            * Phase 2: VX-001
                 1. Rapid Burst - Inflicts 1,414 to 1,586 (Heroic: 1,885 to 2,115) Spellfire damage to a random target every half-second for 3 seconds.
                 2. P3Wx2 Laser Barrage - After a 4-second cast, VX-001 will shoot a laser in a random direction and spin slowly. The laser will inflict 20,000 Fire damage every 0.25 seconds, and will last 10 seconds.
                 3. Rocket Strike - VX-001 will fire a rocket that will travel to a location underneath one of the players. Reaching the location, it will inflict 5 million Fire damage to everyone in 3 yards radius.
                 4. Heat Wave - Inflicts 1,885 to 2,115 (Heroic: 2,828 to 3,172) Fire damage instantly and an additional 2,000 (Heroic: 3,000) Fire damage to the whole raid every 1 second, for 5 seconds.
            * Phase 3: Aerial Command Unit
                 1. Plasma Ball - Inflicts 9,425 to 10,575 (Heroic: 14,138 to 15,862) Spellfire damage to its current target.
                 2. Summon Adds - ACU will periodically summon Assault Bots, Bomb Bots, and Junk Bots.
                       1. Assault Bots - Cast Magnetic Field on their target, locking it in place and causing it to take 30% more damage for 6 seconds. When killed, Assault Bots will drop Magnetic Core, which can be used bring ACU to the ground, disable it and cause it to take 50% more damage for a few seconds. The Magnetic Core has to be used underneath the ACU. Assault Bots spawn off red beams.
                       2. Bomb Bots - Walk aimlessly and explode when they die, inflicting 9,425 to 10,575 damage. They spawn underneath ACU, after a short cast.
                       3. Junk Bots - Low on health and with weak damage, Junk Bots don't do anything special. They spawn off green beams.
            * Phase 4: V0-L7R-0N
                 1. Proximity Mines - Drops 8-10 Proximity Mines around V0-L7R-0N's MK II. After a small arming time, the mines will inflict 12,000 Fire damage to anyone who walks over them.
                 2. Shock Blast - After a 4-second cast, V0-L7R-0N's MK II will inflict 100,000 Nature damage to all enemies within 15 yards of him.
                 3. Hand Pulse - Inflicts 4,242 to 4,758 (Heroic: 6,598 to 7,402) Spellfire damage to a random target and all enemies between the caster and the target.
                 4. P3Wx2 Laser Barrage - After a 4-second cast, V0-L7R-0N's VX-001 will shoot a laser in a random direction and spin slowly. The laser will inflict 20,000 Fire damage every 0.25 seconds, and will last 10 seconds.
                 5. Rocket Strike - V0-L7R-0N's VX-001 will fire two rockets that will travel to a location underneath two of the players. Reaching the location, they will inflict 5 million Fire damage to everyone in 3 yards radius. During all the duration of the spell, the MK II component is stunned.
                 6. Plasma Ball - Inflicts 9,425 to 10,575 (Heroic: 14,138 to 15,862) Spellfire damage to its current target.
             */
        }
    }
    public class Freya : MultiDiffBoss {
        public Freya() {
            // If not listed here use values from defaults
            #region Info
            Name = "Freya";
            Instance = "Ulduar";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 1394500f, 6763325f, 0, 0 };
            BerserkTimer = new int[] { 10 * 60, 10 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 1.00f, 1.00f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 3, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
            }
            #endregion
            #endregion
            #region Defensive
            Resist_Physical = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Frost = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Fire = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Nature = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Arcane = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Shadow = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Holy = new double[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            #region Impedances
            for (int i = 0; i < 2; i++)
            {
                //Moves;
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
            Elders
                * Elder Brightleaf
                     1. Brightleaf Flux - Changes Elder Brightleaf's damage and healing randomly every 5 seconds.
                     2. Solar Flare - Inflicts 7,863 to 9,137 (Heroic: 12,950 to 15,050) Fire damage on a number of targets based on Brightleaf Flux.
                     3. Unstable Sun Beam - Increases damage and healing done by 5%, stacking up to 10 times. Canceled by Unstable Energy.
                     4. Unstable Energy - Inflicts 12,950 to 15,050 (Heroic: 16,650 to 19,350) Nature damage and cancels the effects of Unstable Sun Beam. Damage done is modified by the number of stacks of Unstable Sun Beam.
                     5. Photosynthesis - Heals Elder Brightleaf for 10,000 damage every 1 sec. Used while inside a Sun Beam.
                * Elder Ironbranch
                     1. Impale - Inflicts 16,650 to 19,350 (Heroic: 32,375 to 37,625) Physical damage every 1 second for 5 seconds. Stuns the target.
                     2. Iron Roots - Summons Iron Roots that root a player and inflict 5,550 to 6,450 (Heroic: 7,800 to 8,200) Nature damage every 2 seconds to them. Lasts until the roots are dead.
                     3. Thorn Swarm - Inflicts 8,325 to 9,675 (Heroic: 12,488 to 14,512) Nature damage to enemies within 6 yards of the target.
                * Elder Stonebark
                     1. Fists of Stone - Decreases movement speed by 20%, increases damage done by 250% and gives Elder Stonebark's attacks chance to cause Broken Bones, which renders the target unable to dodge, parry or block.
                     2. Ground Tremor - Inflicts 8,550 to 9,450 (Heroic: 11,400 to 12,600) Physical damage to the entire raid, and interrupts spellcasting for 8 seconds.
                     3. Petrified Bark - Causes melee attacks to damage the attacker as well. 60 charges (Heroic: 120).
            Freya
                * Sunbeam - Inflicts 5,088 to 5,912 (Heroic: 7,400 to 8,600) Nature damage to enemies within 8 yards of Freya's target.
                * Attuned to Nature - Increases healing done to Freya by 4%. Starts at 150 stacks, or 600%.
                * Touch of Eonar - Heals Freya for 6,000 (Heroic: 24,000) every 1 second.
                * Lifebinder's Gift Summon - Summons an Eonar's Gift, which uses Lifebinder's Gift after ~12 seconds, healing Freya and her allies for 30% (Heroic: 60%) of their maximum health. Eonar's Gift's Pheromones negates the effect of Conservator's Grip on nearby players.
                * Summon Allies of Nature - Summons one of the three following waves of adds every minute. Total of 6 waves will be summoned. Killing any of the adds reduces the number of stacks of Freya's Attuned to Nature.
                     1. Detonating Lashers - Each of the 12 Lashers reduces Freya's Attuned to Nature by 2 stacks. They will attack with Flame Lash and Detonate for 4,163 to 4,837 (Heroic: 6,825 to 7,175) Fire damage when they die.
                     2. Elemental Adds - each one of those reduces Freya's Attuned to Nature by 10 stacks.
                            o Ancient Water Spirit - Attacks with Tidal Wave, which inflicts Frost damage in a frontal cone.
                            o Storm Lasher - Attacks with Stormbolt for 6,013 to 6,987 (Heroic: 8,788 to 10,212) Nature damage. Also casts Lightning Lash for 5,088 to 5,912 (Heroic: 6,938 to 8,062), which hits up to 3 (Heroic: 5) targets.
                            o Snaplasher - Attacks against the Snaplasher trigger Hardened Bark, which causes attacks against it to increase its damage, but decrease its movement speed (Heroic: doesn't reduce movement speed)
                     3. Ancient Conservator - Reduces Freya's Attuned to Nature by 30 stacks. Applies Conservator's Grip when he spawns, preventing players from attacking or casting spells. Getting under one of the many Healthy Spores or an Eonar's Gift negates the effect. He will also debuff players with Nature's Fury, which cause the debuffed player to damage nearby allies for 4,163 to 4,837 (Heroic: 5,550 to 6,450) Nature damage every 2 seconds for 10 seconds.
                * Nature Bomb - Summons immobile Nature Bombs around the feet of random raid members. The bombs use Nature Bomb after a few seconds, inflicting 5,850 to 6,150 (Heroic: 8,775 to 9,225) Nature damage to enemies within 10 yards and knocking them back. Used frequently 6 minutes into the fight.             */
        }
    }
    public class Thorim : MultiDiffBoss {
        public Thorim() {
            // If not listed here use values from defaults
            #region Info
            Name = "Thorim";
            Instance = "Ulduar";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 1742400f, 6763325f, 0, 0 };
            BerserkTimer = new int[] { 5 * 60 * 2, 5 * 60 * 2, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 1.00f, 1.00f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 3, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
            }
            #endregion
            #endregion
            #region Defensive
            Resist_Physical = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Frost = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Fire = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Nature = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Arcane = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Shadow = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Holy = new double[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            #region Impedances
            for (int i = 0; i < 2; i++)
            {
                //Moves;
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
            * Pre-Phase Adds
                 1. Jormungar Behemoth - Attacks with Acid Breath (Heroic) and Sweep (Heroic)
                 2. Captured Mercenary Captain • Captured Mercenary Captain - Attacks with Devastate and Heroic Strike
                 3. Captured Mercenary Soldiers • Captured Mercenary Soldiers - Attack with Shoot, Barbed Shot and Wing Clip
            * Phase 1: Adds
                 1. Arena
                        o Dark Rune Acolyte - Uses Greater Heal to heal an ally for 40,500 to 49,500 (Heroic: 94,500 to 115,500); Renew for 9,263 to 9,737 (Heroic: 13,650 to 14,350) every 3 seconds for 15 seconds, and Holy Smite to attack for 4,950 to 6,050 (Heroic: 7,650 to 9,350) Holy damage.
                        o Dark Rune Champions - Attack with Mortal Strike, can Charge and Whirlwind.
                        o Dark Rune Commoners - Attack with Low Blow and can interrupt spellcasting with Pummel.
                        o Dark Rune Evokers - Attack with Runic Lightning (Heroic); they also protect their allies with Runic Mending (Heroic) and Runic Shield (Heroic).
                        o Dark Rune Warbringers - Attack with Runic Strike and have a passive Aura of Celerity
                 2. Hallway
                        o Iron Ring Guard - Can use Whirling Trip to stun the target, or Impale (Heroic) to damage it.
                        o Dark Rune Acolyte - Uses Greater Heal to heal an ally for 40,500 to 49,500 (Heroic: 94,500 to 115,500); Renew for 9,263 to 9,737 (Heroic: 13,650 to 14,350) every 3 seconds for 15 seconds, and Holy Smite to attack for 4,950 to 6,050 (Heroic: 7,650 to 9,350) Holy damage.
                        o Runic Colossus (Mini Boss) - Uses Smash, Runic Barrier and randomly does Charge, which hits for 4,625 to 5,375 (Heroic: 6,938 to 8,062) Physical damage.
                        o Iron Honor Guard - Attack with Cleave, occasionally Hamstring and can stun with Shield Smash (Heroic)
                        o Ancient Rune Giant (Mini Boss) - Buffs allies with Runic Fortification, uses Stomp (Heroic) and randomly casts Rune Detonation.

            * Phase 1: Thorim (inactive)
                 1. Sheath of Lightning - Reduces all damage done to Thorim by 99% for the duration of the Phase.
                 2. Stormhammer - Inflicts 2,451 to 2,551 Nature damage to a player, stuns them for 2 seconds and causes Deafening Thunder around them.
                 3. Deafening Thunder - Inflicts 4,625 to 5,375 Nature damage and increases casting speed by 75% for 8 seconds.
                 4. Charge Orb - Charges one of the four orbs in the room with lightning, causing it to strike with Lightning Shock, inflicting 2,831 to 3,169 Nature damage to any player nearby.
                 5. Berserk - 5 minutes after the raid defeats the jormungar and the captives, Thorim will empower all adds in both Arena and Hallway, making them almost impossible to kill.
                 6. Summon Lightning Orb - As soon as the Berserk occurs, Thorim will send a ball of lightning through the hallway, killing everyone inside.
            * Phase 2: Thorim
                 1. Touch of Dominion - Reduces Thorim's health and damage by 25%. This debuff only becomes active if Thorim is forced into the Arena 3 or more minutes after he is engaged.
                 2. Chain Lightning - Inflicts 3,700 to 4,300 (Heroic: 4,625 to 5,375) Nature damage on its main target and increases its damage by 100% (Heroic: 50%) each time it jumps.
                 3. Lightning Charge - Charges a 60-degrees cone at random direction with lightning sparks. The cone will cause Lightning Charge after a few seconds, inflicting 17,344 to 20,156 Nature damage to players in it, and increasing Thorim's attack speed by 15% and Nature damage by 10%.
                 4. Unbalancing Strike - Inflicts 200% weapon damage on the main target and reduces its defense skill by 200 for 15 seconds.             */
        }
    }
    public class Hodir : MultiDiffBoss {
        public Hodir() {
            // If not listed here use values from defaults
            #region Info
            Name = "Hodir";
            Instance = "Ulduar";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 8115990f, 32477904f, 0, 0 };
            BerserkTimer = new int[] { 8 * 60, 8 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.75f, 0.75f, 0, 0 };// he moves A LOT so it's hard to stay behind him at all times
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 1, 1, 0, 0 };
            Min_Healers = new int[] { 3, 5, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
            }
            #endregion
            #endregion
            #region Defensive
            Resist_Physical = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Frost = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Fire = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Nature = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Arcane = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Shadow = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Holy = new double[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            #region Impedances
            for (int i = 0; i < 2; i++)
            {
                //Moves;
                //Stuns;
                //Fears;
                //Roots;
                // Freeze: Inflicts 5,550 to 6,450 Frost damage to players within 10 yards. Also roots
                // the targets in place for 10 seconds. The rooting component of the spell is a magic debuff.
                // Going to assume the CD is 45 sec for now
                this[i].Roots.Add(new Impedance()
                {
                    Frequency = 45f,
                    Duration = 10f * 1000f,
                    Chance = 1.00f,
                    Breakable = true,
                });
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
            * Biting Cold - Applies exponentially increasing damage-over-time Frost damage effect every 1 second to stationary targets. 1 second of moving or a jump will reduce the stack by 1.
            * Freeze - Inflicts 5,550 to 6,450 Frost damage to players within 10 yards. Also roots the targets in place for 10 seconds. The rooting component of the spell is a magic debuff.
            * Flash Freeze - Encases the entire raid in tombs of ice with 35,000 (Heroic: 44,000) hit points. Kills targets already frozen. Flash Freeze has 9 seconds cast time and its effects can be negated by standing in a Snowdrift spawned by Icicle.
            * Icicle - Inflicts 14,000 Frost damage in a random area through Ice Shards. Larger Icicles will fall just as Hodir casts Flash Freeze - they leave Snowdrift, which protects against the Flash Freeze.
            * Frozen Blows - Reduces Hodir's physical damage by 70%, but grants each of his attacks 31,062 (Heroic: 40,000) additional Frost damage, for 20 seconds. Enemies take 4,000 additional Frost damage every 2 seconds for the duration of the spell.
            * Friendly NPCs - Hodir will engage with 4 (Heroic: 8) NPCs friendly to the raid enclosed in ice tombs. The raid can choose to free any number of them and benefit from their abilities.
                 1. Druids - Tor Greycloud • Kar Greycloud or Eivi Nightfeather • Ellie Nightfeather. They attack with Wrath, which does 1,913 to 2,587 Nature damage, and cast Starlight, which increases attack and casting speed of the affected targets by 100% for 1 minute.
                 2. Shamans - Spiritwalker Tara • Spiritwalker Yona or Elementalist Mahfuun • Elementalist Avuun. They attack with Lava Burst, which does 4,250 to 5,750 Fire damage, and cast Storm Cloud, which allows the affected player to buff up to 4 (Heroic: 6) allies with Storm Power, which increases critical strike damage by 135% for 30 seconds.
                 3. Mages - Amira Blazeweaver • Veesha Blazeweaver or Missy Flamecuffs • Sissy Flamecuffs. They attack with Fireball, which does 3,570 to 4,830 Fire damage and an additional 900 Fire damage every 3 second for 6 seconds. They also cast Conjure Toasty Fire, which prevents the effects of Biting Cold and Freeze, and Melt Ice, which attempts to free a target encased in a frost tomb over 10 seconds. Players standing near a Toasty Fire have a chance to apply Singed when hitting with spells.
                 4. Priests - Battle-Priest Eliza • Battle-Priest Gina or Field Medic Penny • Field Medic Jessy. They attack with Smite, which does 3,400 to 4,600 Holy damage and can use Great Heal, which heals for 19,000 to 21,000. They also cast Dispel Magic, which can free players from the immobilizing effect of Freeze.
             */
        }
    }
    // The Descent into Madness
    public class GeneralVezax : MultiDiffBoss {
        public GeneralVezax() {
            // If not listed here use values from defaults
            #region Info
            Name = "General Vezax";
            Instance = "Ulduar";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 6275250f, 6763325f, 0, 0 };
            BerserkTimer = new int[] { 10 * 60, 1 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 1.00f, 1.00f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 3, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
            }
            #endregion
            #endregion
            #region Defensive
            Resist_Physical = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Frost = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Fire = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Nature = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Arcane = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Shadow = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Holy = new double[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            #region Impedances
            for (int i = 0; i < 2; i++)
            {
                //Moves;
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
            * Aura of Despair - Prevents most mana regeneration methods. Melee attack speed is reduced by 20% for all raid members as well.
              Mana Regeneration

              Lifebloom, Aspect of the Viper, Shamanistic Rage, Judgements of the Wise (without the Replenishment component), Spiritual Attunement, Stormstrike/Improved Stormstrike all work at least partially.
              Hymn of Hope increases the mana pool of the targets, but does not regenerate mana.
            * Shadow Crash - Shoots a shadow missile towards a random target (preferring ranged), inflicting 11,310 to 12,690 Shadow damage to the target and anyone in 10 yd radius. A dark puddle spawns under the area of impact, increasing magical damage by 100%, decreasing casting speed by 100%, healing done and mana cost by 75%.
            * Mark of the Faceless - Applies a Mark of the Faceless to a random raid member (preferring ranged), causing them to drain 5,000 hit points every 1 second, for 10 seconds, from other nearby players. Vezax will heal for 20 times more health than he has drained (100,000 for every 5,000).
            * Searing Flames - Inflicts 13,875 to 16,125 Fire damage to everybody in the room, and reduces their armor by 75%. Can be interrupted.
            * Surge of Darkness - Increases Vezax's damage by 100%, but decreases his movement speed by 55%, for 10 seconds. Cast every 60-70 seconds.
            * Saronite Barrier - Reduces all damage taken by 99%. Cast if 6 Saronite Vapors spawn and not a single one of them is destroyed. Vezax will also summon a Saronite Animus. The Barrier disappears when the Animus dies.
            * Saronite Animus - Casts Profound Darkness frequently, which does 750 Shadow damage to the entire raid and increases the Shadow damage taken by 10%.
             */
        }
    }
    public class YoggSaron : MultiDiffBoss {
        public YoggSaron() {
            // If not listed here use values from defaults
            #region Info
            Name = "Yogg Saron";
            Instance = "Ulduar";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 10999997f, 6763325f, 0, 0 };
            BerserkTimer = new int[] { 15 * 60, 15 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 1.00f, 1.00f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 3, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
            }
            #endregion
            #endregion
            #region Defensive
            Resist_Physical = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Frost = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Fire = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Nature = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Arcane = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Shadow = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Holy = new double[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            #region Impedances
            for (int i = 0; i < 2; i++)
            {
                //Moves;
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
            * The Keepers of Ulduar
                 1. Freya - Buffs the raid with Resilience of Nature, which increases damage done by 10% and healing by 20%. Also provides Sanity Wells, which regenerate 10% Sanity every 2 seconds and reduce all damage taken by 50% to players standing in them, through Sanity Well.
                 2. Hodir - Buffs the raid with Fortitude of Frost, which increases damage done by 10% and reduces damage taken by 20%. He can also use Hodir's Protective Gaze to save a player from what would otherwise be a killing blow. This spell has ~25 seconds cooldown.
                 3. Mimiron - Buffs the raid with Speed of Invention, which increases damage done by 10% and movement speed by 20%. He can also use Destabilization Matrix to reduce the attack speed by 100% and casting speed by 300% of all Crusher and Corruptor Tentacles during Phase 2.
                 4. Thorim - Buffs the raid with Fury of the Storm, which increases damage done by 10% and hit points by 20%. He can also use Titanic Storm to kill Immortal Guardians of Yogg-Saron left at 1% hit points, during Phase 3.
            * Phase 1: Sara the Ally
                 1. Sara's Fervor - Increases damage done by 20% and damage taken by 100% for 15 seconds. Cast on a random raid member.
                 2. Sara's Blessing - Heals for 27,000 to 33,000 instantly, but causes 3,000 Shadow damage every 1 second, for 20 seconds (Total: 60,000). Cast on a random raid member.
                 3. Sara's Anger - Inflicts 12,500 Shadow damage every 3 seconds for 12 seconds (Total: 50,000), but increases physical damage done by 12,000. Cast on a random Guardian of Yogg-Saron.
                 4. Guardians of Yogg-Saron - Spawned slowly at first and more quickly as the fight goes on. Cast Dark Volley, which inflicts 8,500 to 11,500 Shadow damage to everyone in 35 yards range, and reduces the healing done to them by 25%, for 10 seconds. Guardians are also spawned if a player touches any of the clouds moving in the center of the room. They cast Shadow Nova as they die, inflicting 20,000 to 21,994 (Heroic: 25,000 to 27,500) Shadow damage to everyone in 15 yards range.[/b]
            * Phase 2: Sara the Enemy
                 1. Sanity - Indicates your overall level of Sanity. If Sanity reaches zero your mind will be permanently under the control of Yogg-Saron.
                 2. Psychosis - Inflicts 5,000 (Heroic: 7,500) Shadow damage and reduces Sanity by 12% (Heroic: 9%). Cast on a random raid member.
                 3. Malady of the Mind - Inflicts 5,000 Shadow damage, causes a fear effect for 4 seconds and attempts to jump on another player within 10 yards after the effect is over. Also reduces Sanity by 3%.
                 4. Brain Link - Connects two random targets with a beam, causing them to suffer 3,000 Shadow damage and 2% Sanity loss every 1 second they are more than 20 yards away from each other. Lasts 30 seconds, or until one of the players dies or enters a portal.
                 5. Death Ray - Summons 4 Death Orbs, each inflicting ~10,000 damage per second to players standing in them.
                 6. Descend Into Madness - Approximately every 90 seconds, 4 (Heroic: 10) portals will open to Brain of Yogg-Saron. Players taking them may be hit by Lunatic Gaze if they face the laughing skulls around the room.
                 7. The Brain Chamber - Players inside the illusions will need to quickly kill the fake NPCs (e.g. Suit of Armor) inside, which will open a passage to the brain room. Attacking the brain will cause Shattered Illusion, which stuns all tentacles outside the brain room. As soon as the portals open, Induce Madness is cast - players need to leave the brain room through portals in it before the cast is over.
            * Phase 2: Tentacles
                 1. Crusher Tentacles - Erupt as they spawn, inflicting 1,488 to 2,012 Physical damage,knocking players back. They channel Diminish Power, which reduces the damage of everybody in the raid by 20%. Diminish Power can be interrupted by a melee attack. Attacks against the Crushers buff them with Focused Anger, increasing their damage done and attack speed by 3% per application. Lasts 1 second unless refreshed.
                 2. Constrictor Tentacles - Squeeze a random target, inflicting 6,000 (Heroic: 7,500) Physical damage every 2 seconds. Immunity effects or the death of the tentacles free the target.
                 3. Corruptor Tentacle - Erupt as they spawn, inflicting 1,488 to 2,012 Physical damage, knocking players back. Cast Apathy, which reduces attack, casting and movement speed by 60% for 20 seconds (Magic effect); Black Plague, which stuns the target for 2 seconds every 3 seconds (Disease); Curse of Doom, which inflicts 20,000 Shadow damage after 12 seconds (Curse); Draining Poison, which inflicts 3,500 Nature damage and drains 1,400 mana every 3 seconds for 18 seconds.
            * Phase 3: An Old God
             */
        }
    }
    // Celestial Planetarium
    public class AlgalontheObserver : MultiDiffBoss {
        public AlgalontheObserver() {
            // If not listed here use values from defaults
            #region Info
            Name = "Algalon the Observer";
            Instance = "Ulduar";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, BossHandler.TierLevels.T8_0, BossHandler.TierLevels.T8_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 8367000f, 6763325f, 0, 0 };
            BerserkTimer = new int[] { 6 * 60, 6 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 1.00f, 1.00f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 3, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
            }
            #endregion
            #endregion
            #region Defensive
            Resist_Physical = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Frost = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Fire = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Nature = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Arcane = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Shadow = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Holy = new double[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            #region Impedances
            for (int i = 0; i < 2; i++)
            {
                //Moves;
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
            * Quantum Strike - Inflicts 15,675 to 17,325 (Heroic: 34,125 to 35,875) Physical damage.
            * Phase Punch - Inflicts 8,788 to 10,212 an applies a stacking debuff on the target. When the debuff reaches 5 stacks the target is affected by Phase Punch.
            * Phase Punch - Shifts the target to another plane for 10 seconds.
            * Cosmic Smash - Inflicts 41,438 to 43,562 (Heroic: 53,625 to 56,375) Fire damage to anyone under the area of impact, and less Fire damage to everyone, based on proximity.
            * Big Bang - Inflicts 76,313 to 88,687 (Heroic: 107,250 to 112,750) Physical damage to anyone not standing inside a Black Hole. Removes players from Black Holes.
            * Ascend to the Heavens - Inflicts 655,500 to 724,500 damage to all enemies. Cast 6 minutes after engaging him. Equivalent to Berserk.
            * Collapsing Stars - Summoned periodically during Phase 1. They lose 1% health each second and cast Black Hole Explosion as they die, which inflicts 16,088 to 16,912 (Heroic: 20,475 to 21,525) Shadow damage to the entire raid. They leave Black Holes on the ground as they die.
            * Black Holes - Spawned by dying Collapsing Star. Players standing in a Black Hole are shifted to Worm Hole dimension, taking 1,532 to 1,968 Arcane damage every 1 second spent shifted. Dark Matters populate the Worm Hole. If a Living Constellation enters a Black Hole, both despawn.
            * Living Constellation - Spawned in threes periodically during Phase 1. Attack with Arcane Barrage, which inflicts 4,163 to 4,837 Arcane damage. If a Living Constellation enters a Black Hole, both despawn.
            * Dark Matter - Spawned inside Worm Hole dimension during Phase 1. Attack any raid member who enters a Black Hole. Spawned periodically from Black Holes during Phase 2.
             */
        }
    }
}