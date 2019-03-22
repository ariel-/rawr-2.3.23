using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Rawr.Bosses {
    // ===== Naxxramas ================================
    // Spider Wing
    public class AnubRekhan : MultiDiffBoss
    {
        public AnubRekhan() {
            // If not listed here use values from defaults
            #region Info
            Name = "Anub'Rekhan";
            Instance = "Naxxramas";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 2231200, 6763325f, 0, 0 };
            Max_Players = new int[] { 10, 25,  0,  0 };
            Min_Tanks   = new int[] {  2,  3,  0,  0 };
            Min_Healers = new int[] {  2,  4,  0,  0 };
            #endregion
            #region Offensive
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                {
                    /* = Impale =
                     * Anub'Rekhan will target a random player and send a line of spikes out towards the
                     * player, hitting everyone in a straight line between him and his target. Players
                     * hit by these spikes will take 4,813 to 6,187 (Heroic: 5,688 to 7,312) physical
                     * damage and will be knocked into the air, suffering reduced fall damage when they land.
                     */
                    Attack a = new Attack {
                        Name = "Impale",
                        DamageType = ItemDamageType.Physical,
                        DamagePerHit = new float[] { (4813f + 6187f), (5688f + 7312f) }[i] / 2f,
                        MaxNumTargets = this[i].Max_Players,
                        AttackSpeed = 40.0f,
                        AttackType = ATTACK_TYPES.AT_AOE,
                        IgnoresMTank = true,
                    };
                    this[i].Attacks.Add(a);
                    // When he Impales, he turns around and faces the raid
                    // simming this by using the activates over fight and having him facing raid for 2 seconds
                    float time = (this[i].BerserkTimer / a.AttackSpeed) * 2f;
                    this[i].InBackPerc_Melee -= time / this[i].BerserkTimer;
                    this[i].InBackPerc_Ranged -= time / this[i].BerserkTimer;
                }
            }
            #endregion
            #region Impedances
            for (int i = 0; i < 2; i++)
            {
                Impedance M;
                /* = Locust Swarm =
                    * Every 80-120 seconds for 16 seconds you can't be on the target
                    * Note: Adding 4 seconds to the Duration for moving out before it
                    * starts and then back in after
                */
                this[i].Moves.Add(M = new Impedance()
                {
                    Frequency = (80f + 120f) / 2f,
                    Duration = (16f + 4f) * 1000f,
                    Chance = 1.00f,
                    Breakable = false, // Because he's being Kited and if you stay near him you die
                });
                // Every time he Locust Swarms he summons a Crypt Guard
                // Let's assume it's up for 10 seconds
                this[i].Targets.Add(new TargetGroup()
                {
                    Frequency = M.Frequency,
                    Duration = 10 * 1000f,
                    Chance = 1.00f,
                    NearBoss = false,
                    NumTargs = new float[] { 1 , 2 }[i],
                });
                // Every time he spawns a Crypt Guard and it dies, x seconds
                // after he summons 10 scarabs from it's body
                // Assuming they are up for 8 sec
                this[i].Targets.Add(new TargetGroup()
                {
                    Frequency = M.Frequency,
                    Duration = 8 * 1000f,
                    Chance = 1.00f,
                    NearBoss = false,
                    NumTargs = new float[] { 2, 4 }[i],
                });
            }
            #endregion
            /* TODO:
             * Damage from the Adds, minor but should still be there
             */
        }
    }
    public class GrandWidowFaerlina : MultiDiffBoss
    {
        public GrandWidowFaerlina() {
            // If not listed here use values from defaults
            #region Info
            Name = "Grand Widow Faerlina";
            Instance = "Naxxramas";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 2231200f, 6763325f, 0, 0 };
            BerserkTimer = new int[] { 19 * 60, 19 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 1.00f, 1.00f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 2, 0, 0 };
            Min_Healers = new int[] { 3, 4, 0, 0 };
            #endregion
            #region Offensive
            for (int i = 0; i < 2; i++) {
                this[i].Targets.Add(new TargetGroup { // Worshippers
                    Frequency = this[i].BerserkTimer - 1, // Once
                    Chance = 1.00f,
                    NearBoss = false,
                    Duration = new float[] { 4, 6 }[i] * 5 * 1000, // 4(+2) adds, 5 seconds each, used to derage her
                    NumTargs = 1
                });

                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                this[i].Attacks.Add(new DoT
                {
                    Name = "Poison Bolt Volley",
                    DamageType = ItemDamageType.Nature,
                    DamagePerHit = new float[] { (2625f + 3375f) / 2.0f, (3755f + 4125f) / 2.0f }[i],
                    DamagePerTick = new float[] { BossHandler.CalcADotTick(1480f, 1720f, 8f, 2f), BossHandler.CalcADotTick(1900f, 2100f, 8f, 2f) }[i],
                    TickInterval = 2f,
                    NumTicks = 4,
                    MaxNumTargets = 3,
                    AttackSpeed = (7.0f + 15.0f) / 2.0f,
                    AttackType = ATTACK_TYPES.AT_RANGED,
                });
                {
                    Attack a = new Attack
                    {
                        Name = "Rain of Fire",
                        DamageType = ItemDamageType.Fire,
                        DamagePerHit = new float[] { BossHandler.CalcADotTick(1750f, 2750f, 6f, 2f), BossHandler.CalcADotTick(3700f, 4300f, 6f, 2f) }[i],
                        MaxNumTargets = this[i].Max_Players,
                        AttackSpeed = (6.0f + 18.0f) / 2.0f,
                        AttackType = ATTACK_TYPES.AT_AOE,
                    };
                    this[i].Attacks.Add(a);
                    // For each Rain of Fire she has to be moved (assuming 3 seconds to move)
                    this[i].Moves.Add(new Impedance()
                    {
                        Frequency = a.AttackSpeed,
                        Duration = 3f * 1000f,
                        Chance = 1f,
                        Breakable = false
                    });
                }
            }
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
            //Moves;
            //Stuns;
            //Fears;
            //Roots;
            //Disarms;
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
             * Frenzy
             */
        }
    }
    public class Maexxna : MultiDiffBoss
    {
        public Maexxna() {
            // If not listed here use values from defaults
            #region Info
            Name = "Maexxna";
            Instance = "Naxxramas";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 2510000f, 7600000f, 0, 0 };
            BerserkTimer = new int[] { 19 * 60, 19 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 1.00f, 1.00f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 1, 1, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            for (int i = 0; i < 2; i++)
            {
                // 8 Adds every 40 seconds for 8 seconds (only 7300 HP each)
                this[i].Targets.Add(new TargetGroup
                {
                    Frequency = 40,
                    Chance = 1.00f,
                    Duration = 8 * 1000,
                    NumTargs = 8,
                    NearBoss = true,
                });

                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                {
                    /* = Web Wrap =
                     * Cast 20 seconds after engaging, and every 40 seconds after that.
                     * Sends 1 (Heroic: 2) player straight to the western web wall, encasing
                     * them in a Web Wrap cocoon and incapacitating them. When encased, the
                     * player takes 2,475 to 3,025 Nature damage every 2 seconds. The cocoon
                     * can be destroyed from the outside, freeing the player and causing them
                     * to take minimal falling damage when they land.
                     */
                    Attack a = new Attack
                    {
                        Name = "Web Wrap",
                        DamageType = ItemDamageType.Nature,
                        DamagePerHit = new float[] { (2925f + 3575f) / 2f, (2925f + 3575f) / 2f }[i],
                        MaxNumTargets = new int[] { 1, 2 }[i],
                        AttackSpeed = 40.0f,
                        AttackType = ATTACK_TYPES.AT_RANGED,
                    };
                    this[i].Attacks.Add(a);
                    float initial = 20f;
                    float freq = a.AttackSpeed;
                    float chance = 1f + a.MaxNumTargets / (this[i].Max_Players - this[i].Min_Tanks);
                    this[i].Stuns.Add(new Impedance()
                    {
                        Frequency = freq * (this[i].BerserkTimer / (this[i].BerserkTimer - initial)) * chance,
                        Duration = 5f * 1000f,
                        Chance = 1f / (this[i].Max_Players - this[i].Min_Tanks),
                        Breakable = false
                    });
                }
                {
                    /* = Web Spray =
                     * Cast every 40 seconds, incapacitating everyone for 6 seconds, and
                     * dealing 1,750 to 2,250 (Heroic: 5,225 to 5,775) Nature damage. This
                     * ability cannot be resisted.
                     */
                    Attack a = new Attack
                    {
                        Name = "Web Spray",
                        DamageType = ItemDamageType.Nature,
                        DamagePerHit = new float[] { (2188f + 2812f) / 2f, (5225f + 5775f) / 2f }[i],
                        MaxNumTargets = this[i].Max_Players,
                        AttackSpeed = 40.0f,
                        AttackType = ATTACK_TYPES.AT_AOE,
                    };
                    this[i].Attacks.Add(a);
                    float initial = 0f;
                    float freq = a.AttackSpeed;
                    float chance = 1f + a.MaxNumTargets / this[i].Max_Players;
                    this[i].Stuns.Add(new Impedance()
                    {
                        Frequency = freq * (this[i].BerserkTimer / (this[i].BerserkTimer - initial)),
                        Duration = 6f * 1000f,
                        Chance = 1f,
                        Breakable = false
                    });
                }
                {
                    /* = Poison Shock =
                     * Does 3500 to 4500 (Heroic: 4,550 to 5,850) Nature damage in a 15
                     * yard frontal cone.
                     */
                    Attack a = new Attack
                    {
                        Name = "Poison Shock",
                        DamageType = ItemDamageType.Nature,
                        DamagePerHit = new float[] { (3500f + 4500f) / 2f, (4550f + 5850f) / 2f }[i],
                        MaxNumTargets = 1,
                        AttackSpeed = 40.0f,
                        AttackType = ATTACK_TYPES.AT_MELEE,
                    };
                    this[i].Attacks.Add(a);
                }
            }
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
            //Moves;
            //Stuns;
            //Fears;
            //Roots;
            //Disarms;
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
             * Necrotic Poison - Poison effect that reduces healing taken by 90% for 30 seconds.
             * Frenzy - At 30% health, Maexxna will Frenzy, increasing attack speed and damage done by 50% (25 Player: damage by 75%).
             */
        }
    }
    // Plague Quarter
    public class NoththePlaguebringer : MultiDiffBoss {
        /// <summary>
        /// <para>Phase 1: Ground Phase</para>
        /// <para>o Curse of the Plaguebringer - Places a curse on 3 party members. After 10 seconds, every cursed target will cast Wrath of the
        ///     Plaguebringer, which will hit everybody in 30 yards range for 3,700 to 4,300 (25 Player: 5,550 to 6,450) Shadow damage and
        ///     additional 1,313 to 1687 (25 Player: 3,150 to 3,850) Shadow damage every 2 seconds, for 10 seconds.</para>
        /// <para>o Blink (25 Player) - Every 20-30 seconds Noth will Blink away from the tank, wiping all aggro and casting Cripple on everybody
        ///     around his old location.</para>
        /// <para>o Cripple - Noth will place a magic debuff on a few players in melee range, reducing their attack speed by 100% and movement
        ///     speed and Strength by 50%, for 15 seconds.</para>
        /// <para>o Plagued Warrior - Summoned every 30 seconds while in Phase 1. They Cleave, dealing 110% weapon damage to two targets.</para>
        /// <para>o Change Phase: Balcony - After 110 seconds on the ground, Noth will teleport to his balcony, entering Phase 2.</para>
        /// 
        /// <para>Phase 2: Balcony Phase</para>
        /// <para>o Plagued Champion - Summoned during Phase 2-1 and 2-2. They will use Mortal Strike, dealing 100% weapon damage and reducing
        ///     healing done by 50% and Shadow Shock, which hits nearby enemies for 2,313 to 2,687 (25 Player: 2,960 to 3,440) Shadow damage.</para>
        /// <para>o Plagued Guardian - Summoned during Phase 2-2 and 2-3. Their primary damaging ability is Arcane Explosion, which hits every in
        ///     30 yards range for 2,313 to 2,687 (25 Player: 2,590 to 3,010) Arcane damage.</para>
        /// <para>o Change Phase: Ground - After 70 seconds on the balcony, Noth will teleport back to the ground, re-entering Phase 1.</para>
        /// </summary>
        public NoththePlaguebringer() {
            // If not listed here use values from defaults
            #region Info
            Name = "Noth the Plaguebringer";
            Instance = "Naxxramas";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 2789000f, 8436725f, 0, 0 };
            float[] Phase1Length = { 110, 110, 0, 0 };
            float[] Phase2Length = {  70,  70, 0, 0 };
            BerserkTimer = new int[] { ((int)Phase1Length[0] + (int)Phase2Length[0]) * 3, ((int)Phase1Length[1] + (int)Phase2Length[1]) * 3, 0, 0 };
            SpeedKillTimer = new int[] { (int)Phase1Length[0], (int)Phase1Length[1], 0, 0 };
            float[] PercDurInPhase1 = { ((BerserkTimer[0] / Phase1Length[0]) * Phase2Length[0]) / BerserkTimer[0], ((BerserkTimer[1] / Phase1Length[1]) * Phase2Length[1]) / BerserkTimer[1] };
            float[] PercDurInPhase2 = { 1f - PercDurInPhase1[0], 1f - PercDurInPhase1[1] };
            InBackPerc_Melee = new double[] { 0.95f, 0.95f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 1, 2, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            for (int i = 0; i < 2; i++)
            {
                #region MultiTargs
                #region Phase 1
                // Every 30 seconds 2 adds will spawn with 100k HP each, simming their life-time to 20 seconds
                this[i].Targets.Add(new TargetGroup { // Plagued Warriors
                    Frequency = 30,
                    Chance = PercDurInPhase1[i],
                    Duration = 20 * 1000,
                    NumTargs = new int[] { 2, 3 }[i],
                    NearBoss = false,
                });
                #endregion
                #region Phase 2
                this[i].Targets.Add(new TargetGroup // Plagued Champions/Guardians
                {
                    Frequency = 30,
                    Chance = PercDurInPhase2[i],
                    Duration = 20 * 1000,
                    NumTargs = new int[] { 2, 4 }[i],
                    NearBoss = false,
                });
                #endregion
                #endregion

                #region Attacks
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));

                this[i].Attacks.Add(new Attack { // ToDo: Real DoT damage
                    Name = "Curse of the Plaguebringer",
                    AttackSpeed = 45,//unc
                    AttackType = ATTACK_TYPES.AT_RANGED,
                    DamagePerHit = new float[] { (3700f + 4300f), (5550f + 6450f) }[i] / 2f,
                    DamageType = ItemDamageType.Shadow,
                    MaxNumTargets = new float[] { 3, 10 }[i],
                    Missable = false, Dodgable = false, Parryable = false, Blockable = false,
                });
                #endregion
            }
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
                this[i].Moves.Add(new Impedance { // Blink
                    Frequency = (20f+30f)/2f,
                    Duration = 2 * 1000f,
                    Chance = PercDurInPhase1[i],
                    Breakable = true,
                });
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { PercDurInPhase2[0], PercDurInPhase2[1], 0, 0 };
            #endregion
            /* TODO:
             * Cripple
             * Add's Damage
             * Make the Default Melee limited to Phase 1
             */
        }
    }
    public class HeigantheUnclean : MultiDiffBoss {
        public HeigantheUnclean() {
            // If not listed here use values from defaults
            #region Info
            Name = "Heigan the Unclean";
            Instance = "Naxxramas";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 3067900f, 9273425f, 0, 0 };
            float[] Phase1Length = { 90, 90, 0, 0 };
            float[] Phase2Length = { 45, 45, 0, 0 };
            BerserkTimer = new int[] { 19 * 60, 19 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            float[] PercDurInPhase1 = { ((BerserkTimer[0] / Phase1Length[0]) * Phase2Length[0]) / BerserkTimer[0], ((BerserkTimer[1] / Phase1Length[1]) * Phase2Length[1]) / BerserkTimer[1] };
            float[] PercDurInPhase2 = { 1f - PercDurInPhase1[0], 1f - PercDurInPhase1[1] };
            InBackPerc_Melee = new double[] { 0.00f, 0.25f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 1, 1, 0, 0 };
            Min_Healers = new int[] { 3, 5, 0, 0 };
            #endregion
            #region Offensive
            for (int i = 0; i < 2; i++)
            {
                #region Attacks
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                this[i].Attacks.Add(new Attack // ToDo: An Actual DoT
                {
                    Name = "Decrepit Fever",
                    DamageType = ItemDamageType.Nature,
                    DamagePerHit = new float[] { 3000f / 3f * 21f, 4500f / 3f * 21f }[i],
                    MaxNumTargets = 1,
                    AttackSpeed = 30.0f,
                    AttackType = ATTACK_TYPES.AT_RANGED,
                });
                #endregion
            }
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
                // We are assuming you are using the corner trick so you don't have
                // to dance as much in 10 man
                // Every 90 seconds for 45 seconds you must do the safety dance
                // If you are good you can stop 4 times for 5 seconds each and do
                // something to the boss
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = 90f + 45f,
                    Duration = (45f - 4f * 5f) * 1000f,
                    Chance = new float[] { PercDurInPhase2[i], 1.00f }[i], // 10 man can corner cheat, 25 man has to dance
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
             * Decrepit Fever as DoT
             * Spell Disruption - Every few seconds Heigan will debuff everybody in 20 yards range with a debuff that increases casting time by 300% for 10 seconds.
             * Make the attacks limited to Phase 1
             */
        }
    }
    public class Loatheb : MultiDiffBoss {
        public Loatheb() {
            // If not listed here use values from defaults
            #region Info
            Name = "Loatheb";
            Instance = "Naxxramas";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 6693600f, 20220250f, 0, 0 };
            BerserkTimer = new int[] { 5 * 60, 5 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 1.00f, 1.00f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 1, 1, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            for (int i = 0; i < 2; i++)
            {
                #region Attacks
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                this[i].Attacks.Add(new Attack
                {
                    Name = "Deathbloom",
                    DamageType = ItemDamageType.Nature,
                    DamagePerHit = new float[] { (/*DoT*/200f / 1f * 6f) + (/*Bloom*/1200f), (/*DoT*/400f / 1f * 6f) + (/*Bloom*/1500f) }[i],
                    MaxNumTargets = 1,
                    AttackSpeed = 30.0f,
                    AttackType = ATTACK_TYPES.AT_RANGED,
                });
                this[i].Attacks.Add(new Attack
                {
                    Name = "Inevitable Doom",
                    DamageType = ItemDamageType.Shadow,
                    DamagePerHit = new float[] { 4000f / 30f * 120f, 5000f / 30f * 120f }[i],
                    MaxNumTargets = 10,
                    AttackSpeed = 120.0f,
                    AttackType = ATTACK_TYPES.AT_RANGED,
                });
                #endregion
            }
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
                // Initial 10 seconds to pop first Spore then every 3rd spore
                // after that (90 seconds respawn then 10 sec moving to/back)
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = 90f,
                    Duration = 10f * 1000f,
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
             * Necrotic Aura
             * Fungal Creep
             */
        }
    }
    // Military Quarter
    public class InstructorRazuvious : MultiDiffBoss {
        public InstructorRazuvious() {
            // If not listed here use values from defaults
            #region Info
            Name = "Instructor Razuvious";
            Instance = "Naxxramas";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 3349000f, 10110125, 0, 0 };
            BerserkTimer = new int[] { 19 * 60, 19 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.95f, 0.95f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 4, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            for (int i = 0; i < 2; i++)
            {
                #region Attacks
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                this[i].Attacks.Add(new Attack
                {
                    Name = "Disrupting Shout",
                    DamageType = ItemDamageType.Physical,
                    DamagePerHit = new float[] { (4275f + 4725f) / 2f, (7125f + 7825f) / 2f }[i],
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = 15.0f,
                    AttackType = ATTACK_TYPES.AT_AOE,
                });
                this[i].Attacks.Add(new Attack
                {
                    Name = "Jagged Knife",
                    DamageType = ItemDamageType.Physical,
                    DamagePerHit = new float[] { 5000 + (10000 / 5 * 5), 5000 + (10000 / 5 * 5) }[i],
                    MaxNumTargets = 1,
                    AttackSpeed = 10.0f,
                    AttackType = ATTACK_TYPES.AT_RANGED,
                });
                #endregion
            }
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
             * Unbalancing Strike
             * Using the Understudies
             */
        }
    }
    public class GothiktheHarvester : MultiDiffBoss {
        public GothiktheHarvester() {
            // If not listed here use values from defaults
            #region Info
            Name = "Gothik the Harvester";
            Instance = "Naxxramas";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 836700f, 2510100f, 0, 0 };
            float[] Phase1Length = { 274, 274, 0, 0 };
            float[] Phase2Length = { 45, 45, 0, 0 };
            BerserkTimer = new int[] { 19 * 60 - 274, 19 * 60 - 274, 0, 0 };
            SpeedKillTimer = new int[] { (int)Phase1Length[0] + (int)Phase2Length[0], (int)Phase1Length[0] + (int)Phase2Length[0], 0, 0 };
            float[] PercDurInPhase1 = { Phase1Length[0] / BerserkTimer[0], Phase1Length[1] / BerserkTimer[1] };
            float[] PercDurInPhase2 = { 1f - PercDurInPhase1[0], 1f - PercDurInPhase1[1] };
            InBackPerc_Melee = new double[] { 0.95f, 0.95f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 2, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            for (int i = 0; i < 2; i++)
            {
                #region MultiTargs
                // NOTE: I'm not duplicating these to the Undead side (on purpose)
                /* Unrelenting Trainee (Live side) - Total of 24 of those will spawn throughout the fight, always coming in pairs.
                 * They cast Death Plague, which does 85 (25 Player: 170) Nature damage per 3 seconds, stacking indefinitely. Will
                 * spawn Spectral Trainee.*/
                this[i].Targets.Add(new TargetGroup {
                    Frequency = Phase1Length[i] / (24/2),
                    Duration = 10 * 1000,
                    Chance = PercDurInPhase1[i],
                    NumTargs = 2,
                    NearBoss = false,
                });
                /* Unrelenting Death Knight (Live side) - Total of 7 of those will spawn throughout the fight. They cast Shadow Mark,
                 * which hits for ~3,500 (25 Player: ~5,000) on plate and applies a Shadow Mark debuff, which causes all Unrelenting
                 * Riders to be able to hit the debuffed players with shadow bolts. Will spawn Spectral Death Knight.*/
                this[i].Targets.Add(new TargetGroup {
                    Frequency = Phase1Length[i] / (7),
                    Duration = 10 * 1000,
                    Chance = 1.00f,
                    NumTargs = 1,
                    NearBoss = false,
                });
                /* Unrelenting Rider (Live side) - Total of 4 of those will spawn throughout the fight. Their Unholy Aura will hit
                 * everybody in line of sight for 350 (25 Player: 500) Shadow damage every 2 seconds. Shadow Bolt Volley will hit for
                 * 3,800 to 4,200 (25 Player: 5,700 to 6,300) Shadow damage, but will affect only people with the Mark of Shadow
                 * debuff. Will spawn Spectral Rider and Spectral Horse.*/
                this[i].Targets.Add(new TargetGroup {
                    Frequency = Phase1Length[i] / (4),
                    Duration = 10 * 1000,
                    Chance = PercDurInPhase1[i],
                    NumTargs = 1,
                    NearBoss = false,
                });
                #endregion

                #region Attacks
                this[i].Attacks.Add(new Attack
                {
                    Name = "Shadowbolt",
                    DamageType = ItemDamageType.Shadow,
                    DamagePerHit = new float[] { (2880f + 3520f) / 2f, (4500f + 5500f) / 2f }[i],
                    MaxNumTargets = 1,
                    AttackSpeed = 1.0f,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                });
                #endregion
            }
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
            TimeBossIsInvuln = new float[] { PercDurInPhase1[0], PercDurInPhase1[1], 0, 0 };
            #endregion
            /* TODO:
             * Harvest Soul
             * Phase 2 Blinking
             */
        }
    }
    public class FourHorsemen : MultiDiffBoss {
        public FourHorsemen() {
            // If not listed here use values from defaults
            #region Info
            Name = "Four Horsemen";
            Instance = "Naxxramas";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 781000f * 4f, 2370650f * 4f, 0, 0 };
            BerserkTimer = new int[] { 19 * 60, 19 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.75f, 0.75f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 3, 3, 0, 0 }; // simming 3rd to show that 2 dps have to OT the back
            Min_Healers = new int[] { 3, 4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                this[i].Attacks.Add(new Attack
                {
                    Name = "Korth'azz's Meteor",
                    DamageType = ItemDamageType.Fire,
                    DamagePerHit = new float[] { (13775f + 15225f) / 2f, (13775f + 15225f) / 2f }[i],
                    MaxNumTargets = 8,
                    AttackSpeed = 15.0f,
                    AttackType = ATTACK_TYPES.AT_AOE,
                });
                this[i].Attacks.Add(new Attack
                {
                    Name = "Rivendare's Unholy Shadow",
                    DamageType = ItemDamageType.Shadow,
                    DamagePerHit = new float[] { (2160f + 2640f) / 2f + (4800 / 2 * 4), (2160f + 2640f) / 2f + (4800 / 2 * 4) }[i],
                    MaxNumTargets = 8,
                    AttackSpeed = 15.0f,
                    AttackType = ATTACK_TYPES.AT_RANGED,
                });
                this[i].Attacks.Add(new Attack
                {
                    Name = "Blaumeux's Shadow Bolt",
                    DamageType = ItemDamageType.Shadow,
                    DamagePerHit = new float[] { (2357f + 2643f) / 2f, (2357f + 2643f) / 2f }[i],
                    MaxNumTargets = 1,
                    AttackSpeed = 2.0f,
                    AttackType = ATTACK_TYPES.AT_RANGED,
                });
                this[i].Attacks.Add(new Attack
                {
                    Name = "Zeliek's Holy Bolt",
                    DamageType = ItemDamageType.Holy,
                    DamagePerHit = new float[] { (2357f + 2643f) / 2f, (2357f + 2643f) / 2f }[i],
                    MaxNumTargets = 1,
                    AttackSpeed = 2.0f,
                    AttackType = ATTACK_TYPES.AT_RANGED,
                });
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
                // Swap 1st 2 mobs once: 15
                // Get to the back once: 10
                // Bounce back and forth in the back: Every 30 sec for 10 sec but for only 40% of the fight
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = 30f,
                    Duration = 10f * 1000f,
                    Chance = 0.40f,
                    Breakable = false
                });
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
             * Blaumeux's Void Zone
             */
        }
    }
    // Construct Quarter
    public class Patchwerk : MultiDiffBoss {
        public Patchwerk() {
            // If not listed here use values from defaults
            #region Info
            Name = "Patchwerk";
            Instance = "Naxxramas";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 4322950, 13038575, 0, 0 };
            BerserkTimer = new int[] { 6 * 60, 6 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 1.00f, 1.00f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 3, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            for (int i = 0; i < 2; i++)
            {
                #region Attacks
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                this[i].Attacks.Add(new Attack {
                    Name = "Hateful Strike",
                    DamageType = ItemDamageType.Physical,
                    DamagePerHit = new float[] { (19975f + 27025f) / 2f, (79000f + 81000f) / 2f }[i],
                    MaxNumTargets = 1f,
                    AttackSpeed = 1.0f,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                });
                #endregion
            }
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
             * Frenzy
             */
        }
    }
    public class Grobbulus : MultiDiffBoss {
        public Grobbulus() {
            // If not listed here use values from defaults
            #region Info
            Name = "Grobbulus";
            Instance = "Naxxramas";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 2928000f, 9552325, 0, 0 };
            BerserkTimer = new int[] { 12 * 60, 9 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.95f, 0.95f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 2, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            for (int i = 0; i < 2; i++)
            {
                #region MultiTargs
                this[i].Targets.Add(new TargetGroup { // Fallout Slimes
                     Frequency = 15,
                     Duration = 5*1000,
                     Chance = 1.00f,
                     NumTargs = 1,
                     NearBoss = false,
                });
                #endregion

                #region Attacks
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                #endregion
            }
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
                // Every 8 seconds for 3 seconds Grob has to be kited to
                // avoid Poison Cloud Farts. This goes on the entire fight
                // Dropping the Dur to 1 sec for usability
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = 8f,
                    Duration = 1f * 1000f,
                    Chance = 1f,
                    Breakable = false
                });
                // Every 20 seconds 1/10 chance to get hit with Mutating Injection
                // You have to run off for 10 seconds then run back for 4-5
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = 20f,
                    Duration = (10f + (4f + 5f) / 2f) * 1000f,
                    Chance = 1f / (this[i].Max_Players - 1f),
                    Breakable = false
                });
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
             * Slime Spray
             * Occasional Poins Cloud Ticks that are unavoidable
             */
        }
    }
    public class Gluth : MultiDiffBoss {
        public Gluth() {
            // If not listed here use values from defaults
            #region Info
            Name = "Gluth";
            Instance = "Naxxramas";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 2789000, 8436725, 0, 0 };
            BerserkTimer = new int[] { 8 * 60, 8 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 1.00f, 1.00f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 3, 0, 0 };
            Min_Healers = new int[] { 3, 4, 0, 0 };
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
             * Decimate
             * Enrage
             * Mortal Wound
             * Zombie Chows
             */
        }
    }
    public class Thaddius : MultiDiffBoss {
        public Thaddius() {
            // If not listed here use values from defaults
            #region Info
            Name = "Thaddius";
            Instance = "Naxxramas";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 3850000f + 838300f, 3834875 + 838300f, 0, 0 };// one player only deals with one of the add's total health + thadd's health
            float[] Phase1Length = { 1 * 60, 1 * 60, 0, 0 }; // Simming Feugen/Stalagg to 1 min since they die pretty easily for an at-content geared group
            float[] Phase2Length = { 6 * 60, 6 * 60, 0, 0 }; // Simming Thaddius to the rest, since he starts a zerk timer of 6 min at engagement
            BerserkTimer = new int[] { 7 * 60, 7 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            float[] PercDurInPhase1 = { ((BerserkTimer[0] / Phase1Length[0]) * Phase2Length[0]) / BerserkTimer[0], ((BerserkTimer[1] / Phase1Length[1]) * Phase2Length[1]) / BerserkTimer[1] };
            float[] PercDurInPhase2 = { 1f - PercDurInPhase1[0], 1f - PercDurInPhase1[1] };
            InBackPerc_Melee = new double[] { 1.00f * PercDurInPhase1[0] + 0.50f * PercDurInPhase2[0], 1.00f * PercDurInPhase1[1] + 0.50f * PercDurInPhase2[1], 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 2, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            for (int i = 0; i < 2; i++)
            {
                #region Attacks
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                this[i].Attacks.Add(new Attack {
                    Name = "Chain Lightning (Thaddius)",
                    DamageType = ItemDamageType.Nature,
                    DamagePerHit = new float[] { (3600f + 4400f) / 2f, (6938f + 8062f) / 2f }[i],
                    MaxNumTargets = 5f,
                    AttackSpeed = 15.0f,
                    AttackType = ATTACK_TYPES.AT_AOE,
                });
                this[i].Attacks.Add(new Attack {
                    Name = "Static Field (Feugen)",
                    DamageType = ItemDamageType.Nature,
                    DamagePerHit = new float[] { 2500f, 3500f }[i],
                    MaxNumTargets = 5f,
                    AttackSpeed = 15.0f,
                    AttackType = ATTACK_TYPES.AT_AOE,
                });
                #endregion
            }
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
                // Every 30 seconds, polarity shift, 3 sec move
                // 50% chance that your polarity will change
                this[i].Moves.Add(new Impedance() {
                    Frequency = 30f,
                    Duration = 3f * 1000f,
                    Chance = PercDurInPhase2[i] * 0.50f,
                    Breakable = true
                });
                // You have to run from Stal/Feug to Thaddius once
                this[i].Moves.Add(new Impedance() {
                    Frequency = BerserkTimer[i] - 1,
                    Duration = 5f * 1000f,
                    Chance = 1.00f,
                    Breakable = true
                });
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
             * Better handle of Feugen and Stalagg
             */
        }
    }
    // Frostwyrm Lair
    public class Sapphiron : MultiDiffBoss {
        public Sapphiron() {
            // If not listed here use values from defaults
            #region Info
            Name = "Sapphiron";
            Instance = "Naxxramas";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 4183500, 13038575, 0, 0 };
            BerserkTimer = new int[] { 15 * 60, 15 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.95f, 0.95f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 1, 1, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            for (int i = 0; i < 2; i++)
            {
                #region Attacks
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                this[i].Attacks.Add(new Attack
                {
                    Name = "Cleave",
                    DamageType = ItemDamageType.Physical,
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content] + 50,
                    MaxNumTargets = 10,
                    AttackSpeed = 10.0f,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                });
                this[i].Attacks.Add(new Attack
                {
                    Name = "Tail Sweep",
                    DamageType = ItemDamageType.Physical,
                    DamagePerHit = new float[] { 1500f + 2500f, 2188f + 2812f }[i] / 2f,
                    MaxNumTargets = 10,
                    AttackSpeed = 10.0f,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                });
                this[i].Attacks.Add(new Attack
                {
                    Name = "Frost Aura",
                    DamageType = ItemDamageType.Frost,
                    DamagePerHit = new float[]{1200,1600}[i],
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = 2.0f,
                    AttackType = ATTACK_TYPES.AT_AOE,
                });
                this[i].Attacks.Add(new Attack
                {
                    Name = "Life Drain",
                    DamageType = ItemDamageType.Shadow,
                    DamagePerHit = new float[]{(((4376f + 5624f) / 2f) * 3f) * 4f,(((4376f + 5624f) / 2f) * 3f) * 4f}[i],
                    MaxNumTargets = 2,
                    AttackSpeed = 24.0f,
                    AttackType = ATTACK_TYPES.AT_RANGED,
                });
                #endregion
            }
            #endregion
            #region Defensive
            Resist_Physical = new double[] { 0.00f, 0.00f, 0, 0 };
            Resist_Frost = new double[] { 0.75f, 0.75f, 0, 0 };
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
                // Every 45(+30) seconds for 30 seconds Sapph is in the air
                // He stops this at 10% hp
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = 45f + 30f,
                    Duration = 30f * 1000f,
                    Chance = 0.90f,
                    Breakable = false
                });
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
             * Chill (The Blizzard)
             * Ice Bolt
             */
        }
    }
    public class KelThuzad : MultiDiffBoss {
        public KelThuzad() {
            // If not listed here use values from defaults
            #region Info
            Name = "Kel'Thuzad";
            Instance = "Naxxramas";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 2230000f, 14000000f, 0, 0 };
            float[] Phase1Length = { 3 * 60 + 48, 3 * 60 + 48, 0, 0 };
            float[] Phase2Length = { 19 * 60 - Phase1Length[0], 19 * 60 - Phase1Length[1], 0, 0 };
            BerserkTimer = new int[] { ((int)Phase1Length[0] + (int)Phase2Length[0]), ((int)Phase1Length[1] + (int)Phase2Length[1]), 0, 0 };
            SpeedKillTimer = new int[] { (int)Phase1Length[0] + 3*60, (int)Phase1Length[1] + 3*60, 0, 0 };
            float[] PercDurInPhase1 = { ((BerserkTimer[0] / Phase1Length[0]) * Phase2Length[0]) / BerserkTimer[0], ((BerserkTimer[1] / Phase1Length[1]) * Phase2Length[1]) / BerserkTimer[1] };
            float[] PercDurInPhase2 = { 1f - PercDurInPhase1[0], 1f - PercDurInPhase1[1] };
            InBackPerc_Melee = new double[] { 1.00f, 1.00f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 3, 3, 0, 0 };
            Min_Healers = new int[] { 3, 4, 0, 0 };
            #endregion
            #region Offensive
            for (int i = 0; i < 2; i++)
            {
                #region Attacks
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                this[i].Attacks.Add(new Attack
                {
                    Name = "Frostbolt (Single)",
                    DamageType = ItemDamageType.Frost,
                    DamagePerHit = new float[] { (10063f + 12937f) / 2f, (29250f + 30750f) / 2f }[i],
                    MaxNumTargets = 1,
                    AttackSpeed = 40.0f,
                    AttackType = ATTACK_TYPES.AT_RANGED,
                });
                this[i].Attacks.Add(new Attack
                {
                    Name = "Frostbolt (Volley)",
                    DamageType = ItemDamageType.Frost,
                    DamagePerHit = new float[] { (4500f + 5500f) / 2f, (7200f + 8800f) / 2f }[i],
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = 40.0f,
                    AttackType = ATTACK_TYPES.AT_AOE,
                });
                this[i].Attacks.Add(new Attack
                {
                    Name = "Mana Detonation",
                    DamageType = ItemDamageType.Arcane,
                    DamagePerHit = new float[] { (10000f + 25000f), (12500f + 30000f) }[i] / 2f,
                    MaxNumTargets = 1,
                    AttackSpeed = 30.0f,
                    AttackType = ATTACK_TYPES.AT_RANGED,
                });
                this[i].Attacks.Add(new Attack
                {
                    Name = "Frost Blast",
                    DamageType = ItemDamageType.Frost,
                    DamagePerHit = 1.04f,DamageIsPerc = true,
                    MaxNumTargets = 1,
                    AttackSpeed = 45.0f,
                    AttackType = ATTACK_TYPES.AT_RANGED,
                    IgnoresMTank = new bool[] { true, false }[i],
                });
                #endregion
            }
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
                // Phase 2 & 3, gotta move out of Shadow Fissures periodically
                // We're assuming they pop every 15-20 seconds and you have to be
                // moved for 6 seconds and there's a 1/RaidSize chance he will select
                // you over someone else
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = (15f+20f)/2f,
                    Duration = 6f * 1000f,
                    Chance = 1f / this[i].Max_Players * PercDurInPhase2[i],
                    Breakable = false
                });
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            // Phase 1, no damage to KT
            TimeBossIsInvuln = new float[] { PercDurInPhase1[0], PercDurInPhase1[1], 0, 0 };
            #endregion
            /* TODO:
             * The Mobs in Phase 1
             */
        }
    }
    // ===== The Obsidian Sanctum =====================
    public class Shadron : MultiDiffBoss {
        public Shadron() {
            // If not listed here use values from defaults
            #region Info
            Name = "Shadron";
            Instance = "The Obsidian Sanctum";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 976150f, 2231200f, 0, 0 };
            BerserkTimer = new int[] { 19 * 60, 19 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.95f, 0.95f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 1, 2, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                this[i].Attacks.Add(new Attack
                {
                    Name = "Shadow Fissure",
                    DamageType = ItemDamageType.Shadow,
                    DamagePerHit = new float[] { (6188f + 8812f) / 2f, (6188f + 8812f) / 2f }[i],
                    MaxNumTargets = 1,
                    AttackSpeed = 40.0f,
                    AttackType = ATTACK_TYPES.AT_AOE,
                });
                this[i].Attacks.Add(new Attack
                {
                    Name = "Shadow Breath",
                    DamageType = ItemDamageType.Shadow,
                    DamagePerHit = new float[] { (6938f + 8062f) / 2f, (6938f + 8062f) / 2f }[i],
                    MaxNumTargets = 1,
                    AttackSpeed = 40.0f,
                    AttackType = ATTACK_TYPES.AT_AOE,
                });
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
                // Every 60 seconds for 20 seconds dps has to jump into the portal and kill the add
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = 60f + 20f,
                    Duration = 20f * 1000f,
                    Chance = 1f,
                    Breakable = false
                });
                // Every (Shadow Fissure Cd) seconds dps has to move out for 5 seconds then back in for 1
                // 1/10 chance he'll pick you
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = this[i].Attacks[1].AttackSpeed,
                    Duration = (5f + 1f) * 1000f,
                    Chance = 1f / this[i].Max_Players,
                    Breakable = false
                });
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
             * The Acolyte Add
             */
        }
    }
    public class Tenebron : MultiDiffBoss {
        public Tenebron() {
            // If not listed here use values from defaults
            #region Info
            Name = "Tenebron";
            Instance = "The Obsidian Sanctum";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 976150f, 2231200, 0, 0 };
            BerserkTimer = new int[] { 19 * 60, 19 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.95f, 0.95f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 2, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                this[i].Attacks.Add(new Attack
                {
                    Name = "Shadow Fissure",
                    DamageType = ItemDamageType.Shadow,
                    DamagePerHit = new float[] { (6188f + 8812f) / 2f, (9488f + 13512f) / 2f }[i],
                    MaxNumTargets = 1,
                    AttackSpeed = 40.0f,
                    AttackType = ATTACK_TYPES.AT_AOE,
                });
                this[i].Attacks.Add(new Attack
                {
                    Name = "Shadow Breath",
                    DamageType = ItemDamageType.Shadow,
                    DamagePerHit = new float[] { (6938f + 8062f) / 2f, (8788f + 10212f) / 2f }[i],
                    MaxNumTargets = 1,
                    AttackSpeed = 40.0f,
                    AttackType = ATTACK_TYPES.AT_AOE,
                });
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
                {
                    // Every 30 seconds for 20 seconds DPS has to jump onto the 6 adds that spawn
                    this[i].Targets.Add(new TargetGroup()
                    {
                        Frequency = 40,
                        Chance = 1.00f,
                        Duration = 16 * 1000,
                        NumTargs = 6,
                        NearBoss = false,
                    });
                    this[i].Moves.Add(new Impedance()
                    {
                        Frequency = 30f + 20f,
                        Duration = (2 + 16 + 2) * 1000f,
                        Chance = 1f,
                        Breakable = false
                    });
                    //this[i].MultiTargsPerc += (this[i].BerserkTimer / (30f + 20f)) * (20f) / this[i].BerserkTimer;
                    //this[i].MaxNumTargets = 6 + 1;
                }
                {
                    // Every (Shadow Fissure Cd) seconds DPS has to move out for 5 seconds then back in for 1
                    // 1/10 chance he'll pick you
                    this[i].Moves.Add(new Impedance()
                    {
                        Frequency = this[i].Attacks[1].AttackSpeed,
                        Duration = (5f + 1f) * 1000f,
                        Chance = 1f / this[i].Max_Players,
                        Breakable = false
                    });
                }
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
             * The Adds' abilities
             */
        }
    }
    public class Vesperon : MultiDiffBoss {
        public Vesperon() {
            // If not listed here use values from defaults
            #region Info
            Name = "Vesperon";
            Instance = "The Obsidian Sanctum";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 976150f, 2231200f, 0, 0 };
            BerserkTimer = new int[] { 19 * 60, 19 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.95f, 0.95f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 1, 1, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                this[i].Attacks.Add(new Attack
                {
                    Name = "Shadow Fissure",
                    DamageType = ItemDamageType.Shadow,
                    DamagePerHit = new float[] { (6188f + 8812f) / 2f, (9488f + 13512f) / 2f }[i],
                    MaxNumTargets = 1,
                    AttackSpeed = 40.0f,
                    AttackType = ATTACK_TYPES.AT_AOE,
                });
                this[i].Attacks.Add(new Attack
                {
                    Name = "Shadow Breath",
                    DamageType = ItemDamageType.Shadow,
                    DamagePerHit = new float[] { (6938f + 8062f) / 2f, (8788f + 10212f) / 2f }[i],
                    MaxNumTargets = 1,
                    AttackSpeed = 40.0f,
                    AttackType = ATTACK_TYPES.AT_AOE,
                });
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
                // Every (Shadow Fissure Cd) seconds dps has to move out for 5 seconds then back in for 1
                // 1/25 chance he'll pick you
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = this[i].Attacks[1].AttackSpeed,
                    Duration = (5f + 1f) * 1000f,
                    Chance = 1f / this[i].Max_Players,
                    Breakable = false
                });
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
             * The adds, which optimally you would ignore
             */
        }
    }
    public class Sartharion : MultiDiffBoss {
        public Sartharion() {
            // If not listed here use values from defaults
            #region Info
            Name = "Sartharion";
            Instance = "The Obsidian Sanctum";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 2510100f, 7669750f, 0, 0 };
            BerserkTimer = new int[] { 19 * 60, 19 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.95f, 0.95f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 2, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));
                this[i].Attacks.Add(new Attack
                {
                    Name = "Fire Breath",
                    DamageType = ItemDamageType.Fire,
                    DamagePerHit = new float[] { (8750f + 11250f) / 2f, (10938f + 14062f) / 2f }[i],
                    MaxNumTargets = 1,
                    AttackSpeed = 40.0f,
                    AttackType = ATTACK_TYPES.AT_AOE,
                });
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
                // Every 45 seconds for 10 seconds you gotta move for Lava Waves
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = 45f,
                    Duration = 10f * 1000f,
                    Chance = 1f,
                    Breakable = false
                });
                //Stuns;
                //Fears;
                //Roots;
                //Disarms;
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
             */
        }
    }
    // ===== The Vault of Archavon ====================
    public class ArchavonTheStoneWatcher : MultiDiffBoss {
        public ArchavonTheStoneWatcher() {
            // If not listed here use values from defaults
            #region Info
            Name = "Archavon The Stone Watcher";
            Instance = "The Vault of Archavon";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 2300925f, 9970675f, 0, 0 };
            BerserkTimer = new int[] { 5 * 60, 5 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.75f, 0.75f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 2, 0, 0 };
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
                // Every 30 seconds for 5 seconds you gotta catch up to him as he jumps around
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = 30f,
                    Duration = 5f * 1000f,
                    Chance = 3f / this[i].Max_Players,
                    Breakable = false
                });
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
            /* TODO:
             * Rock Shards
             * Crushing Leap
             * Stomp (this also stuns)
             * Impale (this also stuns)
             */
        }
    }
    // ===== The Eye of Eternity ======================
    public class Malygos : MultiDiffBoss {
        /// <summary>
        /// <para>* Phase 1</para>
        /// <para>The fight against Malygos consists of three phases. Phase 1 is only slightly more than a tank 'n' spank fight. The tank needs to have Malygos
        /// faced away from the raid to avoid the others being damaged by Arcane Breath. Throughout the phase, Power Sparks spawn off star light at the
        /// edges of the room and attempt to slowly approach Malygos. The best thing is killing them in a 5-10 yards distance to Malygos, so melee and
        /// ranged DPS get advantage of the damage increasing buff which otherwise would have been granted to the boss, stackingly increasing his damage.
        /// Crowd Control is an awesome tool. At least 20k health is needed to survive the vortices cast throughout the fight. Players will only be able
        /// to cast instant spells and suffer periodical damage. While in a vortex, players spin around the transform, being slowly dragged to it and fall.</para>
        ///     <para>o Arcane Breath - Deals 18,850 to 21,150 (25 Player: 28,275 to 31,725) Arcane damage to enemies in front of the caster. Also causes
        ///           the affected targets to explode after 5 seconds, dealing 9,425 to 10,575 (25 Player: 18,850 to 21,150) Arcane damage to nearby allies.</para>
        ///     <para>o Vortex - Causes the raid to spin around the platform, constantly dealing Arcane damage to them. At this point, players should use
        ///     damage-reducing cooldowns. They will be dragged in a spiral-like motion towards the platform, land there and suffer fall damage.
        ///     Only instant spells and abilities work, as the vortex forces to move.</para>
        ///     <para>o Power Spark - Spawned throughout the fight and slowly shift towards Malygos. Once they reach him, they buff him with Power Spark,
        ///     increasing the damage output by 50% for 10 seconds, stacking multiplicatively. If killed, they instead grant players in proximity
        ///     the same buff, Power Spark, which especially is a great buff for melee players close to Malygos.</para>
        /// 
        /// <para>* Phase 2</para>
        /// <para>Phase 2 begins when Malygos has lost 50% of his health. Slowly shifting in the air, players have approximately 10 seconds time to DPS him.
        /// In this Phase, two different NPC groups will spawn, namely Nexus Lords and Scions of Eternity. Protective Bubbles repeatedly spawn and become
        /// smaller over time so raid members are forced to move when they get small. They decrease Arcane damage taken by the players inside them. Flying
        /// Nexus Lords need to be tanked while standing in the bubbles. Once a Lord is dead, his disc will drop and a raid member can hop on it and kill
        /// the flying Scions of Eternity, whose discs also can pick up other players. Ranged DPS should focus on Scions. While standing on a disc,
        /// players do take damage, therefore they should fly high to avoid Malygos' Deep Breaths, also known as Arcane Pulse. This should be repeated
        /// until all NPCs are dead.</para>
        ///     <para>o Nexus Lord - They fly on discs and use Arcane Shock, causing 9,425 to 10,575 (25 Player: 14,138 to 15,862) Arcane damage. Also use
        ///     Haste, increasing their attack, casting and movement speed by 100% for 15 seconds. After killed, they drop off their discs which can
        ///     be used by players to kill remaining NPCs.</para>
        ///     <para>o Scion of Eternity - They also fly on discs, using Arcane Barrage, dealing Arcane damage.</para>
        ///     <para>o Arcane Pulse - Inflicts 28,275 to 31,725 Arcane damage to enemies within 30 yards, cast every 0.5 to 1 second. Preceded by a raid
        ///     warning "Malygos takes a deep breath..."</para>
        ///     <para>o Arcane Storm - Inflicts 9,425 to 10,575 (25 Player: 11,782 to 13,218) Arcane damage upon hitting them with missiles.</para>
        /// 
        /// <para>* Phase 3</para>
        /// <para>Once all NPCs are dead, Phase 3, a vehicle phase, will begin. Players are sent out flying on dragons, using special abilities. The raid needs
        /// to stack to get advantage of the healers' AoE heals which is pretty much the only thing to do. The dragons you will be riding have Rogue-like
        /// abilities. The optimal rotation for DPS is having two combo points and then use a finishing move, for healers using the lesser heals five
        /// times and then blow off the big one. Note that combo points earned through Flame Spikes cannot be used for the Burst of Life and respectively
        /// Revivify combo points not for Engulf in Flames. Malygos will occasionally cast Surge of Power, which will kill the player if no heals occur.
        /// Due to the group stacking this will not be a difficult problem. Static Fields occur throughout Phase 3 and the raid should move to one side
        /// to stay in group. Phase 3 can be practiced through the daily quest Aces High! given by Corastrasza.</para>
        ///     <para>o Dragons</para>
        ///         <para>Each player's dragon has 75,000 base health.</para>
        ///             <para>+ Flame Spike - Inflicts 943 to 1,057 Fire damage. Awards 1 combo point.</para>
        ///             <para>+ Engulf in Flames - Ignites the target with fire, causing Fire damage every 3 seconds. Should be used after two preceding
        ///             Flame Spikes.</para>
        /// 
        ///                 <para>1 point ---> 6  seconds (3,000 Fire damage)</para>
        ///                 <para>2 points --> 10 seconds (4,500 Fire damage)</para>
        ///                 <para>3 points --> 14 seconds (6,000 Fire damage)</para>
        ///                 <para>4 points --> 18 seconds (8,000 Fire damage)</para>
        ///                 <para>5 points --> 22 seconds (9,500 Fire damage)</para>
        /// 
        ///             <para>+ Revivify - Heals the target for 500 every second. Lasts 10 second, if not refreshed. Stacks up to 5 times. Should be used 5
        ///             times to take full advantage of Life Burst.</para>
        ///             <para>+ Life Burst - Instantly heals allies within 60 yards and increases the healing done by 50% for an amount of time. Should be
        ///             used after 5 preceding Revivify spells.</para>
        /// 
        ///                 <para>1 point ---> 5  seconds (5,000  healed)</para>
        ///                 <para>2 points --> 10 seconds (7,500  healed)</para>
        ///                 <para>3 points --> 15 seconds (10,000 healed)</para>
        ///                 <para>4 points --> 20 seconds (12,500 healed)</para>
        ///                 <para>5 points --> 25 seconds (15,000 healed)</para>
        /// 
        ///             <para>+ Flame Shield - Summons a shield protecting the caster and reducing damage suffered by 80% for an amount of time.</para>
        /// 
        ///                 <para>1 point ---> 2 seconds</para>
        ///                 <para>2 points --> 3 seconds</para>
        ///                 <para>3 points --> 4 seconds</para>
        ///                 <para>4 points --> 5 seconds</para>
        ///                 <para>5 points --> 6 seconds</para>
        /// 
        ///             <para>+ Blazing Speed - Increases the dragon's speed by 500% for 8 seconds.</para>
        ///     <para>o Static Field - Randomly summoned throughout Phase 3, dealing 9,425 to 10,575 Arcane damage every second to players within 30 yards
        ///     of the field. The raid should move to one side in order to stay in group.</para>
        ///     <para>o Surge of Power - Inflicts 5,000 (25 Player: 12,000 for 3 seconds) Arcane damage to nearby allies per second, for 5 seconds. Those
        ///     targeted receive a raid warning.</para>
        /// </summary>
        public Malygos() {
            // If not listed here use values from defaults
            #region Info
            Name = "Malygos";
            Instance = "The Eye of Eternity";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, BossHandler.TierLevels.T7_0, BossHandler.TierLevels.T7_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            Comment = "ToDo's:\r\n"
                    + "* We have no way to model Phase 3, and since it's not based on your actual DPS, who cares!\r\n"
                    + "* Need Buff tie-ins for the Power Sparks bonus damage zones in Phase 1\r\n"
                    + "* Arcane Pulse should only be occurring in Phase 2, but can't limit it\r\n";
            #endregion
            #region Basics
            Health = new float[] { 2230000f, 19523000f, 0, 0 };
            BerserkTimer = new int[] { 10 * 60, 10 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.95f, 0.95f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 2, 0, 0 };
            Min_Healers = new int[] { 2, 4, 0, 0 };
            #endregion
            #region Offensive
            for (int i = 0; i < 2; i++)
            {
                this[i].Targets.Add(new TargetGroup {
                    // {Phase 1: Sparks}
                    Frequency = 45, // Once every {unconfirmed} seconds
                    Duration = 10 * 1000f, // Lets say about 10 seconds of actually killing the little bastard
                    Chance =  1f / 3f, // Happens no matter what for 1 of 3 Phases
                    NearBoss = false, // You can't let them reach the boss
                    NumTargs = 1, // There's only one at a time
                });
                this[i].Targets.Add(new TargetGroup {
                    // {Phase 2: Scions}
                    Frequency = this[i].BerserkTimer - 1, // Once
                    Duration = this[i].BerserkTimer / 3f * 1000f, // 1/3 of the fight
                    Chance = 1.00f, // Happens no matter what
                    NearBoss = false, // Can't DPS boss in this phase
                    NumTargs = 2, // There are a bunch of targets, but you can really only DPS 1-2 at a time
                });

                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));

                this[i].Attacks.Add(new Attack {
                    Name = "Arcane Breath",
                    AttackSpeed = 40,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = new float[] { (18850f + 21150f), (28275f + 31725f) }[i] / 2f,
                    DamageType = ItemDamageType.Arcane,
                    Missable = false, Dodgable = false, Parryable = false, Blockable = false, 
                    IgnoresHealers = true, IgnoresMeleeDPS = true, IgnoresOTank = true, IgnoresTTank = true, IgnoresRangedDPS = true,
                    MaxNumTargets = this[i].Max_Players,
                });

                this[i].Attacks.Add(new Attack {
                    Name = "Vortex",
                    AttackSpeed = 40,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = new float[] { 4000, 6000 }[i], // unconfirmed
                    DamageType = ItemDamageType.Arcane,
                    Missable = false, Dodgable = false, Parryable = false, Blockable = false,
                    MaxNumTargets = this[i].Max_Players,
                });

                this[i].Attacks.Add(new Attack {
                    Name = "Arcane Pulse",
                    AttackSpeed = 1,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = new float[] { (28275f + 31725f), (28275f + 31725f) }[i] / 2f,
                    DamageType = ItemDamageType.Arcane,
                    Missable = false, Dodgable = false, Parryable = false, Blockable = false, 
                    MaxNumTargets = this[i].Max_Players,
                });
            }
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
            // Every 70-120 seconds for 16 seconds you can't be on the target (Vortex)
            // Adding 4 seconds to the Duration for moving out before starts and then back in after
            for (int i = 0; i < 2; i++) {
                this[i].Moves.Add(new Impedance() {
                    Frequency = (70f + 120f) / 2f,
                    Duration = (16f + 4f) * 1000f,
                    Chance = 1f / 3f, // 1/3 of the fight
                    Breakable = false,
                });
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0, 0 };
            #endregion
        }
    }
}