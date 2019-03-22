using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Rawr.Bosses
{
    #region The Vault of Archavon
    // ===== The Vault of Archavon ====================
    public class ToravonTheIceWatcher : MultiDiffBoss
    {
        public ToravonTheIceWatcher()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Toravon The Ice Watcher";
            Instance = "The Vault of Archavon";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 4601850f, 15060600f, 0, 0 };
            BerserkTimer = new int[] { 5 * 60, 5 * 60, 0, 0 };
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
                float frostmultiplierfromwhiteout = ( ( (BerserkTimer[i] - 25f) / 38f ) * .15f ) / 2f;
                this[i].Attacks.Add(GenAStandardMelee(this[i].Content));

                // Frozen Mallet - Applies debuff Frostbite
                // Frostbite - The Frostbite inflicts 1950 to 2050 Frost damage every 2 sec and reduces move speed by 5% for 20 sec.
                // Tanks generally take 4 stacks of this debuff
                this[i].Attacks.Add(new DoT
                {
                    Name = "Frostbite",
                    DamageType = ItemDamageType.Frost,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = this[i].Min_Tanks,
                    AttackSpeed = new float[] { (90 / (70 / 2)), (90 / (70 / 2)), (90 / (60 / 2)), (90 / (60 / 2)) }[i],

                    Dodgable = false,
                    Missable = false,
                    Parryable = false,
                    Blockable = false,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
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
    #endregion

    #region Icecrown Citadel
    // ===== Icecrown Citadel =========================
    #region The Lower Spire
    public class LordMarrowgar : MultiDiffBoss
    {
        public LordMarrowgar()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Lord Marrowgar";
            Instance = "Icecrown Citadel";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_9, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10H, BossHandler.Versions.V_25H, };
            #endregion
            #region Basics
            Health = new float[] { 6972500f, 23706500f, 10458750f, 31376250f };

            BerserkTimer = new int[] { 10 * 60, 10 * 60, 10 * 60, 10 * 60, };
            SpeedKillTimer = new int[] { 120, 180, 120, 150 };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 3, 2, 3 };
            Min_Healers = new int[] { 2, 5, 3, 5 };
            // 90 seconds behind the boss, 20/30 seconds without during bone storm
            //      110 seconds rotation during normal; 120 seconds rotation during heroic
            InBackPerc_Melee = new double[] { 70 / 90, 70 / 90, 60 / 90, 60 / 90 };
            InBackPerc_Ranged = new double[] { 70 / 90, 70 / 90, 60 / 90, 60 / 90 };

            #endregion
            #region Offensive
            #region Attacks
            for (int i = 0; i < 4; i++)
            {
                // Melee
                this[i].Attacks.Add(new Attack()
                {
                    Name = "Melee",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    // Hits for about 18k on normal 25
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = 1f,
                    // Only performs this attack during non-Bone Storm phases
                    AttackSpeed = new float[] { (90 / (70 / 2)), (90 / (70 / 2)), (90 / (60 / 2)), (90 / (60 / 2)) }[i],

                    Dodgable = true,
                    Missable = true,
                    Parryable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,

                    IsTheDefaultMelee = true,
                });

                // Saber Lash - Inflicts 200% weapon damage split evenly between the target and its 2 nearest allies.
                this[i].Attacks.Add(new Attack
                {
                    Name = "Saber Lash",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = this[i].Min_Tanks,
                    // Only performs this attack during non-Bone Storm phases
                    AttackSpeed = new float[] { (90 / (70 / 2)), (90 / (70 / 2)), (90 / (60 / 2)), (90 / (60 / 2)) }[i],

                    Dodgable = true,
                    Missable = true,
                    Parryable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });

                // Coldflame - Inflicts 6,000 Frost damage every 1 second for 3 seconds to anyone caught by the moving line of frost.
                this[i].Attacks.Add(new DoT
                {
                    Name = "Coldflame",
                    DamageType = ItemDamageType.Frost,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    NumTicks = 3,
                    TickInterval = 1,
                    DamagePerTick = new int[] { 6000, 8000, 9000, 11000 }[i],
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = 6.0f,
                });

                // Bone Spike Graveyard
                //   Hurls a spike at a random player, impaling all players between the boss and the target on Bone Spikes,
                //   inflicting 9,000 direct Physical damage, and additional 10% health as damage every 1 second until the
                //   spike is destroyed.
                // The player that is spiked is considered unbreakably stunned
                this[i].Attacks.Add(new DoT
                {
                    Name = "Bone Spike Graveyard",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_RANGED,
                    NumTicks = 3f, // Assuming you will be broken out within 3 sec
                    DamagePerHit = 9000f,
                    DamagePerTick = new float[] { 0.10f, 0.10f, 0.10f, 0.10f }[i],
                    DamageIsPerc = true,
                    MaxNumTargets = new int[] { 1, 1, 3, 3 }[i],
                    // Cooldown is every 18 seconds. Normal 10 or 25 does not get cast during bone storm,
                    //      Heroic 10 and 25 bone spike continues during bone storm
                    AttackSpeed = new float[] { (float)(90 / 4), (float)(90 / 4), 18, 18 }[i],
                    IgnoresMTank = true,
                    IgnoresOTank = true,
                    IgnoresTTank = true,
                });
                this[i].Targets.Add(new TargetGroup()
                {
                    Frequency = this[i].Attacks[this[i].Attacks.Count - 1].AttackSpeed,
                    Chance = this[i].Attacks[this[i].Attacks.Count - 1].MaxNumTargets / (this[i].Max_Players - this[i].Min_Tanks),
                    Duration = 3f * 1000f,
                    NumTargs = this[i].Attacks[this[i].Attacks.Count - 1].MaxNumTargets,
                    NearBoss = false,
                });
                this[i].Stuns.Add(new Impedance()
                {
                    Frequency = this[i].Attacks[this[i].Attacks.Count - 1].AttackSpeed,
                    Duration = 3f * 1000f,
                    Chance = this[i].Attacks[this[i].Attacks.Count - 1].MaxNumTargets / (this[i].Max_Players - this[i].Min_Tanks),
                    Breakable = false,
                });

                // Bone Storm
                //   Inflicts 6,000 Physical damage every 2 seconds to players caught in the Bone Storm.
                //   The entire storm lasts ~20 seconds.
                this[i].Attacks.Add(new DoT
                {
                    Name = "Bone Storm",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    NumTicks = new int[] { 20, 20, 30, 30 }[i] / 2f,
                    TickInterval = 2f,
                    DamagePerTick = new int[] { 6000, 12000, 12000, 14000 }[i] * 0.25f, // 6k per tick for 2 sec. 25% mod as people will be running away to take less damage
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = 90f,

                    Dodgable = true,
                    Missable = true,
                    Parryable = true,
                    Blockable = true,
                });
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = this[i].Attacks[this[i].Attacks.Count - 1].AttackSpeed,
                    Duration = new int[] { 20, 30, 20, 30 }[i] * 1000f,
                    Chance = 1f,
                    Breakable = false,
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
            //Moves;
            //Stuns;
            //Fears;
            //Roots;
            //Disarms;
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0.00f, 0.00f };
            #endregion
            /* TODO:
             */
        }
    }
    public class LadyDeathwhisper : MultiDiffBoss
    {
        public LadyDeathwhisper()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Lady Deathwhisper";
            Instance = "Icecrown Citadel";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_9, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10H, BossHandler.Versions.V_25H, };
            #endregion
            #region Basics
            // Health = Mana + Health
            // Mana Barrier, have to destroy mana before dps'g boss
            Health = new float[] { 3264800f + 3346800f, 11193600f + 13387200f, 3264800f + 6693600f, 13992000f + 26774400f };
            BerserkTimer = new int[] { 10 * 60, 10 * 60, 10 * 60, 10 * 60, };
            SpeedKillTimer = new int[] { 150, 300, 150, 300 };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 3, 2, 3 };
            Min_Healers = new int[] { 2, 5, 3, 5 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 2, 2, 2, 2 };
            #region Attacks
            for (int i = 0; i < 4; i++)
            {
                // Adds come 3 at a time, either 2 Adherents 1 Fanatic or 1 Adherent 2 Fanatics
                // 10 man - only 3 at a time
                // 25 man - 7 at a time (3 on left [2 fanatic, 1 adherent], 3 on right [1 fanatic, 2 adherent], 1 in the back [random between the two])
                // They will also randomly respawn as Reanimated or Mutated or Empowered
                this[i].Targets.Add(new TargetGroup()
                {
                    Frequency = new float[] { 60, 60, 45, 45 }[i],
                    Duration = 35 * 1000,
                    NumTargs = new float[] { 3, 7, 3, 7 }[i],
                    Chance = .5f, // phase 1 which lasts for about 50%
                    NearBoss = false,
                });
                // at least 1 add is reanimated to full health each round in Phase 1
                this[i].Targets.Add(new TargetGroup()
                {
                    Frequency = new float[] { 60, 60, 45, 45 }[i],
                    Duration = 35f * 1000,
                    NumTargs = 1f,
                    Chance = .5f * new float[] { 1 / 3, 1 / 7, 1 / 3, 1 / 7 }[i], // phase 1 which lasts for about 50%
                    NearBoss = false,
                });

                // Heroic version spawns adds (1 add on 10 man, 3 adds on 25 man); 50% of fight in Phase 2
                //          None of the adds are turned into Reanimated, Mutated, or Empowered mobs
                this[i].Targets.Add(new TargetGroup()
                {
                    Frequency = 45f, // Every 45 seconds and up for 35 sec before back on boss
                    Duration = 35f * 1000,
                    NumTargs = new float[] { 0, 0, 1, 3 }[i],
                    Chance = new float[] { 0, 0, .5f, .5f }[i], // only 1 phase has them and only in Heroic Mode
                    NearBoss = false,
                });

                // Melee attacks only in Phase 2
                this[i].Attacks.Add(new Attack()
                {
                    Name = "Melee",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    // hits for about 11k in Normal 25 
                    DamagePerHit = ((BossHandler.StandardMeleePerHit[(int)this[i].Content] / 1.3f) / 1.3f),
                    MaxNumTargets = 1f,
                    AttackSpeed = this[i].BerserkTimer / ((this[i].BerserkTimer * .5f) / 2f),

                    Dodgable = true,
                    Missable = true,
                    Parryable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });

                // Shadow Bolt - Inflicts 7,438 to 9,562 Shadow damage to the current target.
                this[i].Attacks.Add(new Attack()
                {
                    Name = "Shadow Bolt",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = new int[] { (7438 + 9562), (9188 + 11812), (9188 + 11812), (11375 + 14625) }[i] / 2f,
                    MaxNumTargets = 1f,
                    AttackSpeed = 4f,
                });

                // Frostbolt - Inflicts 40,950/44,850/50,700/58,500 to 43,050/47,150/53,330/61,500 Frost damage
                //           to the current target. Can be interrupted. 4 second Cast
                //      Only cast during Phase 2
                this[i].Attacks.Add(new Attack()
                {
                    Name = "Frostbolt",
                    DamageType = ItemDamageType.Frost,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    DamagePerHit = new int[] { (40950 + 43050), (50700 + 53330), (44850 + 47150), (58500 + 61500) }[i] / 2f,
                    MaxNumTargets = 1f,
                    AttackSpeed = this[i].BerserkTimer / ((this[i].BerserkTimer * .5f) / 4f),
                    Interruptable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });

                // Frostbolt Volley - Inflicts 8550/10800/14400 to 10450/13200/17600 Frost damage to nearby enemies, reducing their movement speed by 30% for 4 sec.
                //      Only cast during Phase 2
                this[i].Attacks.Add(new Attack()
                {
                    Name = "Frostbolt Volley",
                    DamageType = ItemDamageType.Frost,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = new int[] { (8550 + 10450), (10800 + 13200), (10800 + 13200), (14400 + 17600) }[i] / 2f,
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = this[i].BerserkTimer / ((this[i].BerserkTimer * .5f) / 20f),
                });

                // Death and Decay - Inflicts 4,500 Shadow damage every 1 second to enemies within the area of the spell. 
                //          The entire spell lasts 10 seconds.
                this[i].Attacks.Add(new DoT()
                {
                    Name = "Death and Decay",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    NumTicks = 10f,
                    TickInterval = 1f,
                    DamagePerTick = new int[] { 4500, 4500, 6000, 6000 }[i] * 0.25f, // 4.5k/6k per tick for 1 sec. 25% mod as people will be running away to take less damage
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = 45f,
                });
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = 45f,
                    Duration = 4f * 1000f, // assume 4 seconds to move out
                    Chance = 1 / this[i].Max_Players,
                    Breakable = false,
                });

                // **** Add information **** \\
                // Cult Adherent/Empowered Adherent/Reanimated Adherent
                // Deathchill Bolt - Deals 8788 to 10212 Shadowfrost damage to an enemy target. 2 sec cast. 11563-13437 on Heroic
                this[i].Attacks.Add(new Attack()
                {
                    Name = "Deathchill Bolt",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = new int[] { (6938 + 8062), (8788 + 10212), (8788 + 10212), (11563 + 13437) }[i] / 2f,
                    MaxNumTargets = 1f,
                    // Average uptime of the 2 second cast per round; * average number of casters per pull; for half the fight
                    AttackSpeed = ((new float[] { 60, 60, 45, 45 }[i] / (35f / 2f)) * new float[] { 1.5f, 3.5f, 1.5f, 3.5f }[i] * .5f) + ((new float[] { 60, 60, 45, 45 }[i] / (35f / 2f)) * new float[] { 0f, 0f, .5f, 1.5f }[i] * .5f),
                });

                // Cult Fanatic/Deformed Fanatic/Reanimated Fanatic
                // Necrotic Strike - Strikes an enemy with a cursed blade, dealing 70% of weapon damage to the target and inflicting
                //          a lasting malady that negates the next 14000 healing received. Instant. 20 sec duration. 20000 healing on Heroic 
                this[i].Attacks.Add(new Attack()
                {
                    Name = "Necrotic Strike",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    DamagePerHit = ((BossHandler.StandardMeleePerHit[(int)this[i].Content] / 1.3f) / 1.3f) * .7f,
                    MaxNumTargets = 1f,
                    // Average uptime of the 20 second cd per round; * average number of fanatic per pull; for half the fight
                    AttackSpeed = ((new float[] { 60, 60, 45, 45 }[i] / (35f / 20f)) * new float[] { 1.5f, 3.5f, 1.5f, 3.5f }[i] * .5f) + ((new float[] { 60, 60, 45, 45 }[i] / (35f / 20f)) * new float[] { 0f, 0f, .5f, 1.5f }[i] * .5f),

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });

                // Shadow Cleave - Inflicts 15913 to 17587 Shadow damage to enemies in front of the attacker. 19000-21000 on Heroic (6 sec cooldown)
                this[i].Attacks.Add(new Attack()
                {
                    Name = "Shadow Cleave",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    DamagePerHit = new int[] { (14250 + 15750), (15913 + 17587), (15913 + 17587), (19000 + 21000) }[i] / 2f,
                    MaxNumTargets = 1f,
                    // Average uptime of the 6 second cd per round; * average number of fanatic per pull; for half the fight
                    AttackSpeed = ((new float[] { 60, 60, 45, 45 }[i] / (35f / 6f)) * new float[] { 1.5f, 3.5f, 1.5f, 3.5f }[i] * .5f) + ((new float[] { 60, 60, 45, 45 }[i] / (35f / 6f)) * new float[] { 0f, 0f, .5f, 1.5f }[i] * .5f),

                    Dodgable = true,
                    Missable = true,
                    Parryable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });

                //  Vengeful Shades - Inflicts 19300 to 20700 Shadowfrost damage to an enemy. Normal; 1 shad in 10, 3 shades in 25; most raids don't move on normal
                //     in Heroic people will move since it hits multiple people within a 15/20 yard radius in 10 and 25 man respectfully.
                //     Dies within 10 seconds if it does not hit it's target.
                //     Cannot be targeted
                this[i].Targets.Add(new TargetGroup()
                {
                    Frequency = 30f,
                    Duration = 7f * 1000,
                    NumTargs = new float[] { 1, 3, 1, 3 }[i],
                    Chance = .5f, // phase 2 which lasts for about 50%
                    NearBoss = false,
                });
                // Vengeful Blast on Normal - Most raids don't have people run away from them since they only target 1 or 3 people in 10 or 25 man respectfully
                //    And just heal through the damage
                this[i].Attacks.Add(new Attack()
                {
                    Name = "Vengeful Blast on Normal",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = new int[] { (17853 + 19147), (19300 + 20700), 0, 0 }[i] / 2f,
                    MaxNumTargets = new float[] { 1, 3, 1, 3 }[i],
                    AttackSpeed = 30f,
                });
                // Vengeful Blast on Heroic - People run away from them on heroic since they damage all people within a 15 or 20 yard radius in 10 or 25 man respectfully
                //    if the vengeful spirit hits their intended target.
                this[i].Attacks.Add(new Attack()
                {
                    Name = "Vengeful Blast on Heroic",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = new int[] { 0, 0, (17370 + 18630), (23160 + 24840) }[i] / 2f,
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = 30f,
                    // Interruptable if the person is successfully able to kit the vengeful Spirit for 7 seconds
                    Interruptable = true,
                });
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = 30f,
                    Duration = 7f * 1000,
                    Chance = new int[] { 0, 0, 1, 3 }[i] / (this[i].Max_Players - this[i].Min_Tanks),
                    Breakable = false,
                });
            }
            #endregion
            #endregion
            #region Impedances
            //Moves;
            //Stuns;
            // Dominate Mind - 1 target in 10 man heroic, and 25 normal; 3 targets in 25 heroic. Lasts 12 seconds; 40 second Cooldown
            for (int i = 0; i < 4; i++)
            {
                this[i].Stuns.Add(new Impedance()
                {
                    // First dominate mind is after 30 seconds, then every 40 seconds there-after
                    Frequency = this[i].BerserkTimer / ((this[i].BerserkTimer - 30f) / 40f),
                    Duration = 12f * 1000f,
                    Chance = new int[] { 0, (1 / (this[i].Max_Players - this[i].Min_Tanks)), (1 / (this[i].Max_Players - this[i].Min_Tanks)), (3 / (this[i].Max_Players - this[i].Min_Tanks)) }[i],
                    Breakable = false,
                });
            }
            //Fears;
            //Roots;
            //Disarms;
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0.00f, 0.00f };
            #endregion
            /* TODO:
             * Dark Empowerment - Empowers a random Adherent or Fanatic, causing them to deal area damage with spells. Also makes the target immune to interrupts.
             * Curse of Torpor - Afflicts an enemy target with a curse that increases ability cooldowns by 15 seconds. (ie an instant cast becomes a 15 second cd)
             * Frost Fever  - A disease dealing Frost damage every 3 seconds and reducing the target's melee and ranged attack 
             *          speed by 14% for 15 seconds. Instant 
             * Deathchill Blast  - Deals 11563 to 13437 Shadowfrost damage to all enemies within 10 yards of the target. 2 sec 
             *          cast. Only cast while empowered. 11563-13437 on Heroic 
             *          If everyone is away from add people should not take damage from this
             * Necrotic Strike - Strikes an enemy with a cursed blade, inflicting a lasting malady that negates the next 14000 healing received. 
             *          lasts 20 seconds; 20000 healing on Heroic 
             */
        }
    }
    // - Gunship Event
    public class DeathbringerSaurfang : MultiDiffBoss
    {
        public DeathbringerSaurfang()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Deathbringer Saurfang";
            Instance = "Icecrown Citadel";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_9, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10H, BossHandler.Versions.V_25H, };
            #endregion
            #region Basics
            Health = new float[] { 8785350f, 31376250f, 12299490f, 43926752f };
            BerserkTimer = new int[] { 8 * 60, 8 * 60, 8 * 60, 8 * 60, };
            SpeedKillTimer = new int[] { 150, 180, 150, 180 };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 2, 2, 2 };
            Min_Healers = new int[] { 2, 5, 3, 5 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 1, 1 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 4; i++)
            {
                // Melee
                this[i].Attacks.Add(new Attack
                {
                    Name = "Melee",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    // Hits for about 18k on normal 25
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = 1f,
                    AttackSpeed = 1f,

                    Dodgable = true,
                    Missable = true,
                    Parryable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,

                    IsTheDefaultMelee = true,
                });

                // Boiling Blood - Inflicts 5,000 Physical damage every 3 seconds for 24 seconds. 
                //       Used on a random target. (Same amount in all three versions)
                this[i].Attacks.Add(new DoT()
                {
                    Name = "Boiling Blood",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_RANGED,
                    TickInterval = 3f,
                    NumTicks = 8f,
                    DamagePerTick = 5000f,
                    MaxNumTargets = new int[] { 1, 3, 1, 3 }[i],
                    AttackSpeed = 15.5f,
                });
                // Blood Nova - Inflicts 7,600/9,500 to 8,400/10,500 Physical damage 
                //       to a random target and all players within 12 yards.
                this[i].Attacks.Add(new Attack
                {
                    Name = "Blood Nova",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_RANGED,
                    DamagePerHit = new int[] { (7600 + 8400), (9500 + 10500), (7600 + 8400), (9500 + 10500) }[i] / 2f, // 1 person only assuming everyone is more than 12 yards apart
                    MaxNumTargets = 1f,
                    AttackSpeed = 20f,
                });
            }
            #endregion
            #endregion
            #region Impedances
            //Moves;
            //Stuns;
            //Fears;
            //Roots;
            //Disarms;
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0.00f, 0.00f };
            #endregion
            #region BossAdds
            /*
             * Name = Blood Beast (Summoned 2/5 mobs every 30 seconds)
             * Health = new float[] { 100000f, 120000f, 90997f, 140134f };
             * Resistant Skin - The skin of this creature is highly resistant. Damage from area of effect attacks is 
             *          reduced by 95% and damage from Diseases is reduced by 70%.
             * Scent of Blood - Saurfang buffs the Blood Beasts, causing them to reduce the movement speed of all nearby players by 80%,
             *          while increasing the Beasts' damage by 300%. Lasts 10 seconds. (Heroic 10 and 25 only)
             *          Normal, adds die in 7 seconds
             *          Heroic - about 10 seconds
             */
            #endregion
            /* TODO:
             * TO DO: Blood Power - Increases damage and size by 1%; Stacks up to 100%
             *        This is based on the amount of raid damage and how fast tanks can taunt.
             *        High end guilds can get 2-3 100% stacks out in a fight, 0 on normal;
             *        10man normal 0 - 100% stacks; 10man heroic 1-2 - 100% stacks
             * TO DO: Frenzy  - Increases Saurfang's attack speed by 30% and size by 20%.
             *       Used when Saurfang's health reaches 30%.
             * TO DO: Rune of Blood - Marks a target with Rune of Blood, causing Saurfang's melee attacks
             *       against that target to leech 5,100/5,950 to 6,900/8,050 health from them, and heal the 
             *       Deathbringer for 10 times that amount. Lasts 20 seconds.
             */
        }
    }
    #endregion

    #region The Plagueworks
    public class Festergut : MultiDiffBoss
    {
        public Festergut()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Festergut";
            Instance = "Icecrown Citadel";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_9, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10H, BossHandler.Versions.V_25H, };
            #endregion
            #region Basics
            Health = new float[] { 9412875f, 40440500f, 13666100f, 52293752f };
            BerserkTimer = new int[] { 5 * 60, 5 * 60, 5 * 60, 5 * 60, };
            SpeedKillTimer = new int[] { 150, 180, 180, 210 };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 2, 2, 2 };
            Min_Healers = new int[] { 3, 5, 3, 5 };
            #endregion
            #region Offensive
            float inhalerotationlength = (30f * 4f) + (3.5f * 3f) + 3f;
            for (int i = 0; i < 4; i++)
            {
                #region Attacks
                // Inhale rotation consists of 4 x 30 second Gaseous Blight rotations, 3 x 3.5 second Inhale cast times, and 1 x 3 second Pungent Blight cast time
                // Melee
                // 0 Inhales
                this[i].Attacks.Add(new Attack
                {
                    Name = "Melee with 0 Inhales",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    // hits for about 11k in 25 man
                    DamagePerHit = ((BossHandler.StandardMeleePerHit[(int)this[i].Content] / 1.3f) / 1.3f),
                    MaxNumTargets = 1f,
                    AttackSpeed = (inhalerotationlength / (30f / (2f / 1.1f))), // hits every 1.8 seconds for 30 seconds

                    Dodgable = true,
                    Missable = true,
                    Parryable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,

                    IsTheDefaultMelee = true,
                });
                // 1 Inhale
                this[i].Attacks.Add(new Attack
                {
                    Name = "Melee with 1 Inhales",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    // hits for about 14k in 25 man
                    DamagePerHit = (BossHandler.StandardMeleePerHit[(int)this[i].Content] / 1.3f),
                    MaxNumTargets = 1f,
                    AttackSpeed = (inhalerotationlength / (30f / (2f / 1.4f))), // hits every 1.45 seconds for 30 seconds

                    Dodgable = true,
                    Missable = true,
                    Parryable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });
                // 2 inhales
                this[i].Attacks.Add(new Attack
                {
                    Name = "Melee with 2 Inhales",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    // hits for about 19k in 25 man
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = 1f,
                    AttackSpeed = (inhalerotationlength / (30f / (2f / 1.7f))),  // hits 1.17 seconds for 30 seconds

                    Dodgable = true,
                    Missable = true,
                    Parryable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });
                // 3 inhales
                this[i].Attacks.Add(new Attack
                {
                    Name = "Melee with 3 Inhales",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    // hits for about 25k in 25 man
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content] * 1.3f,
                    MaxNumTargets = 1f,
                    AttackSpeed = (inhalerotationlength / 30f),  // hits every second for 30 seconds

                    Dodgable = true,
                    Missable = true,
                    Parryable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });

                // Gaseous Blight—The Gaseous Plague inflicts 4,388 to 4,612 Shadow damage to all nearby players.
                //              (lasts 30 seconds + 1st inhale cast time [3.5 seconds])
                this[i].Attacks.Add(new DoT()
                {
                    Name = "Gaseous Blight with 0 inhales",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    TickInterval = 1f,
                    NumTicks = 33.5f, //lasts 30 seconds + 1st inhale cast time [3.5 seconds]
                    DamagePerTick = new int[] { (2925 + 3075), (4388 + 4612), (4388 + 4612), (6338 + 6662) }[i] / 2f,
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = inhalerotationlength,
                });

                // Gaseous Blight—(After 1 inhale) The Gaseous Plague inflicts 2,925 to 3,075 Shadow damage to all nearby players.
                //             (lasts 30 seconds + 2st inhale cast time [3.5 seconds]) (lasts 30 seconds + 2st inhale cast time [3.5 seconds])
                this[i].Attacks.Add(new DoT()
                {
                    Name = "Gaseous Blight with 1 inhales",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    TickInterval = 1f,
                    NumTicks = 33.5f, //lasts 30 seconds + 2st inhale cast time [3.5 seconds]
                    DamagePerTick = (new int[] { (1950 + 2050), (2925 + 3075), (2925 + 3075), (4388 + 4612) }[i] / 2f) * .25f, // assume 25% shadow reduction
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = inhalerotationlength,
                });
                // Gaseous Blight—(After 2 inhale ) The Gaseous Plague inflicts 1,463 to 1,537 Shadow damage to all nearby players. 
                //             (lasts 30 seconds + Pungent Blight cast time [3 seconds]) (assume 50% shadow reduction)
                this[i].Attacks.Add(new DoT()
                {
                    Name = "Gaseous Blight with 2 inhales",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    TickInterval = 1f,
                    NumTicks = 33f, //lasts 30 seconds + Pungent Blight cast time [3 seconds]
                    DamagePerTick = (new int[] { (1463 + 1537), (1950 + 2050), (1950 + 2050), (2925 + 3075) }[i] / 2f) * .5f, // assume 50% shadow reduction
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = inhalerotationlength,
                });
                // Pungent Blight—Violently releases the Gaseous Blight, dealing 48,750 to 51,250 Shadow damage to all enemy players, 
                //                 releasing the deadly Blight back into the room. ( inhalerotationlength cd ) (assume 75% shadow reduction)
                this[i].Attacks.Add(new Attack
                {
                    Name = "Pungent Blight",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = (new int[] { (42900 + 45100), (63375 + 66625), (48750 + 51250), (73125 + 76875) }[i] / 2f) * .75f, // assume 75% shadow reduction
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = inhalerotationlength,
                });
                // Vile Gas—Inflicts a Vile plague in targeted area, inflicting 4,875 to 5,125 damage every 2 seconds for 6 seconds. The plague causes the infected targets to vomit uncontrollably, inflicting 3,900 to 4,100 damage to nearby allies. Does not affect melee if there are at least 3 ranged players (2 on 10-man; 3 on 25-man).
                this[i].Attacks.Add(new DoT()
                {
                    Name = "Vile Gas",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    TickInterval = 2f,
                    NumTicks = 3f, //damage every 2 seconds for 6 seconds
                    DamagePerTick = new int[] { (3900 + 4100), (4875 + 5125), (4875 + 5125), (6338 + 6662) }[i] / 2f, // assume everyone is spread out 8 yards and doesn't get hit with aoe from attack
                    MaxNumTargets = new float[] { 2f, 3f, 2f, 3f }[i],
                    AttackSpeed = 20f,
                    IgnoresMTank = true,
                    IgnoresOTank = true,
                    IgnoresMeleeDPS = true,
                });
                this[i].Stuns.Add(new Impedance()
                {
                    Frequency = this[i].Attacks[this[i].Attacks.Count - 1].AttackSpeed,
                    Duration = 6f * 1000f,
                    Chance = 1f / (this[i].Max_Players - this[i].Min_Tanks),
                    Breakable = false,
                });
                // Gas Spore - inflicting 1,950 to 2,050 Shadow damage every 1 second for 6 seconds to any players within 8 yards range. 
                //                      Players who survive the DoT will become Inoculated, which decreases Shadow damage taken by 25%, 
                //                      stacking up to 3 times. Pungent Blight resets the stack of Inoculated.
                this[i].Attacks.Add(new DoT
                {
                    Name = "Gas Spore",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    TickInterval = 1f,
                    NumTicks = 6f, //damage every 1 second for 6 seconds
                    DamagePerTick = new int[] { (1950 + 2050), (1950 + 2050), (1950 + 2050), (1950 + 2050) }[i] / 2f,
                    MaxNumTargets = 1f,
                    AttackSpeed = 40f, // 40 second cooldown, first spore after 12 seconds
                });
                // Some people will have to move, Tanks don't and some will not (aka melee).
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = this[i].Attacks[this[i].Attacks.Count - 1].AttackSpeed,
                    Duration = 3f * 1000f,
                    Chance = 1f / (this[i].Max_Players - this[i].Min_Tanks),
                    Breakable = true,
                });
                // Gastric Bloat—Inflicts 9,750 to 10,250 Nature damage to the target. Damage done by affected increased by 
                //               10% per application. Causes Gastric Explosion, immediately killing the player, if it reaches 10 stacks.
                this[i].Attacks.Add(new Attack
                {
                    Name = "Gastric Bloat",
                    DamageType = ItemDamageType.Nature,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    DamagePerHit = new int[] { (9750 + 10250), (12188 + 12812), (12188 + 12812), (14625 + 15375) }[i] / 2f,
                    MaxNumTargets = 1f,
                    AttackSpeed = 11f,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });
                // Gastric Explosion—Immediately kills the affected player, and does 29,250 to 30,750 shadow damage to all players
                //                    in a 10 yard range. 
                this[i].Attacks.Add(new Attack
                {
                    Name = "Gastric Explosion on Tank",
                    DamageType = ItemDamageType.Nature,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    DamagePerHit = 1f,
                    DamageIsPerc = true,
                    MaxNumTargets = 1f,
                    AttackSpeed = 100f,
                    Interruptable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });
                // Assuming the explosion does go off, it would most likely explode near melee
                this[i].Attacks.Add(new Attack
                {
                    Name = "Gastric Explosion on Melee",
                    DamageType = ItemDamageType.Nature,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    DamagePerHit = new int[] { (29250 + 30750), (29250 + 30750), (48750 + 51250), (48750 + 51250) }[i] / 2f,
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = 100f,
                    Interruptable = true,
                });
                // Malleable Goo - (Heroic Only) Inflicts 14625/19500 to 15375/20500 damage in a 5 yard radius and slows casting
                //                              and attack speed by 250% for 20 sec.
                this[i].Attacks.Add(new Attack
                {
                    Name = "Malleable Goo",
                    DamageType = ItemDamageType.Nature,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = new int[] { 0, (14625 + 15375), 0, (19500 + 20500) }[i] / 2f,
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = 10f,
                    Interruptable = true,
                });
                // run away if Malleable Goo targets the spot/melee that your in; estimate 2 seconds to move away 5 yards
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = this[i].Attacks[this[i].Attacks.Count - 1].AttackSpeed,
                    Duration = 2f * 1000f,
                    Chance = 1f / this[i].Max_Players,
                    Breakable = false,
                });
            }
                #endregion
            #endregion
            #region Impedances
            //Moves;
            //Stuns;
            //Fears;
            //Roots;
            //Disarms;
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0.00f, 0.00f };
            #endregion
            /* TODO:
             */
        }
    }
    public class Rotface : MultiDiffBoss
    {
        public Rotface()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Rotface";
            Instance = "Icecrown Citadel";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_9, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10H, BossHandler.Versions.V_25H, };
            #endregion
            #region Basics
            Health = new float[] { 7321125f, 36257000f, 10458750f, 47413000f };
            // Soft enrage is about 7 minutes in, Hard enrage is after 10 minutes (as shown in the DK soloing video)
            BerserkTimer = new int[] { 10 * 60, 10 * 60, 10 * 60, 10 * 60, };
            SpeedKillTimer = new int[] { 120, 150, 180, 210 };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 2, 2, 2 };
            Min_Healers = new int[] { 3, 5, 3, 5 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 1, 1 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 4; i++)
            {
                this[i].Attacks.Add(new Attack
                {
                    // Melee Attacks
                    Name = "Melee",
                    DamageType = ItemDamageType.Physical,
                    // Hits for about 18k on normal 25
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = 1f,
                    AttackSpeed = 2f,
                    AttackType = ATTACK_TYPES.AT_MELEE,

                    Dodgable = true,
                    Missable = true,
                    Parryable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,

                    IsTheDefaultMelee = true,
                });
            }
            #endregion
            #endregion
            #region Impedances
            //Moves;
            //Stuns;
            //Fears;
            //Roots;
            //Disarms;
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0.00f, 0.00f };
            #endregion
            /* TODO:
             */
        }
    }
    public class ProfessorPutricide : MultiDiffBoss
    {
        public ProfessorPutricide()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Professor Putricide";
            Instance = "Icecrown Citadel";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_9, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10H, BossHandler.Versions.V_25H, };
            #endregion
            #region Basics
            Health = new float[] { 9761500f, 41835000f, 13666100f, 50202000f };
            BerserkTimer = new int[] { 10 * 60, 10 * 60, 10 * 60, 10 * 60, };
            SpeedKillTimer = new int[] { 240, 270, 300, 330 };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 3, 2, 3 }; // One tank does not do much damage to the boss while in Abom, doesn't start tanking until Phase 3
            Min_Healers = new int[] { 3, 5, 3, 5 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 1, 1 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 4; i++)
            {
                this[i].Attacks.Add(new Attack
                {
                    // Melee Attacks
                    Name = "Melee",
                    DamageType = ItemDamageType.Physical,
                    // Hits for about 18k on normal 25
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = 1f,
                    AttackSpeed = 2f,
                    AttackType = ATTACK_TYPES.AT_MELEE,

                    Dodgable = true,
                    Missable = true,
                    Parryable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,

                    IsTheDefaultMelee = true,
                });
            }
            #endregion
            #endregion
            #region Impedances
            //Moves;
            //Stuns;
            //Fears;
            //Roots;
            //Disarms;
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0.00f, 0.00f };
            #endregion
            /* TODO:
             */
        }
    }
    #endregion

    #region The Crimson Hall
    public class BloodPrinceCouncil : MultiDiffBoss
    {
        public BloodPrinceCouncil()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Blood Prince Council";
            Instance = "Icecrown Citadel";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_9, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10H, BossHandler.Versions.V_25H, };
            #endregion
            #region Basics
            // Health is shared between Prince Valanar, Prince Keleseth, and Prince Taldaram
            Health = new float[] { 5647725f, 22590900f, 7624429f, 30497716f };
            BerserkTimer = new int[] { 10 * 60, 10 * 60, 10 * 60, 10 * 60, };
            SpeedKillTimer = new int[] { 150, 180, 180, 210 };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 3, 2, 3 }; // in 25 each boss is tanked separately.
            Min_Healers = new int[] { 3, 5, 3, 5 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 1, 1 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 4; i++)
            {
                this[i].Attacks.Add(new Attack
                {
                    // Melee Attacks
                    Name = "Melee",
                    DamageType = ItemDamageType.Physical,
                    // Hits for about 18k on normal 25
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = 1f,
                    AttackSpeed = 2f,
                    AttackType = ATTACK_TYPES.AT_MELEE,

                    Missable = true,
                    Parryable = true,
                    Dodgable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,

                    IsTheDefaultMelee = true,
                });
            }
            #endregion
            #endregion
            #region Impedances
            //Moves;
            //Stuns;
            //Fears;
            //Roots;
            //Disarms;
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0.00f, 0.00f };
            #endregion
            /* TODO:
             */
        }
    }
    public class BloodQueenLanathel : MultiDiffBoss
    {
        public BloodQueenLanathel()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Blood-Queen Lana'thel";
            Instance = "Icecrown Citadel";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_9, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10H, BossHandler.Versions.V_25H, };
            #endregion
            #region Basics
            Health = new float[] { 14154175f, 59419644f, 18899660f, 71300784f };
            // 5 minute, 20 second fight
            BerserkTimer = new int[] { 320, 320, 320, 320 };
            SpeedKillTimer = new int[] { 210, 210, 270, 270 };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 2, 2, 2 };
            Min_Healers = new int[] { 3, 5, 3, 5 };
            this.InBackPerc_Melee = new double[] { (320 - 12 - 12) / 320, (320 - 12 - 12) / 320, (320 - 12 - 12) / 320, (320 - 12 - 12) / 320 };
            this.InBackPerc_Ranged = new double[] { (320 - 4 - 4) / 320, (320 - 4 - 4) / 320, (320 - 4 - 4) / 320, (320 - 4 - 4) / 320 }; ;

            // Assume that by the time by the end of the second Air Phase, guilds should be at 35%
            //Under35Perc = new double[] { (320 - 124 - 100 - 3) / 320, (320 - 127 - 100 - 3) / 320, (320 - 124 - 100 - 3) / 320, (320 - 127 - 100 - 3) / 320 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 1, 1 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            int avgbitetargets10 = (1 + 2 + 4 + 8) / 4;      // average number of bites in 10 man
            int avgbitetargets25 = (1 + 2 + 4 + 8 + 16) / 5; // average number of bites in 25 man

            for (int i = 0; i < 4; i++)
            {
                this[i].Attacks.Add(new Attack
                {
                    // Melee Attacks
                    Name = "Melee",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    // Hits for about 18k on normal 25
                    // Heroic increases Lana'thel's damage by 10/5% in 10/25 man respecfully for each application of Essence of the Blood Queen
                    DamagePerHit = (1 + (new int[] { 0, avgbitetargets10, 0, avgbitetargets25 }[i] * new float[] { 0f, .10f, 0f, .05f }[i])) * BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = 1f,
                    AttackSpeed = 2f,

                    Missable = true,
                    Parryable = true,
                    Dodgable = true,
                    Blockable = true,

                    IgnoresOTank = true,
                    IgnoresTTank = true,
                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,

                    IsTheDefaultMelee = true,
                });

                // Blood Mirrored Melee attack from MT to OT as Shadow Damage
                this[i].Attacks.Add(new Attack
                {
                    // Melee Attacks
                    Name = "Off-Tank Melee Damage",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    // Hits for about 18k on normal 25
                    // Heroic increases Lana'thel's damage by 10/5% in 10/25 man respecfully for each application of Essence of the Blood Queen
                    DamagePerHit = (1 + (new int[] { 0, avgbitetargets10, 0, avgbitetargets25 }[i] * new float[] { 0f, .10f, 0f, .05f }[i])) * BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = 1f,
                    AttackSpeed = 2f,

                    Missable = true,
                    Parryable = true,
                    Dodgable = true,
                    Blockable = true,

                    IgnoresMTank = true,
                    IgnoresTTank = true,
                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });

                // Shroud of Sorrow - An aura of sorrow and despair emanates from the caster, 
                //        inflicting 4500 Shadow damage every 2 sec. to nearby enemies.
                this[i].Attacks.Add(new Attack
                {
                    Name = "Shroud of Sorrow",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = new int[] { 4000, 4500, 4500, 4500 }[i],
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = 2f,
                });

                // Delirious Slash - Inflicts 50% of weapon damage to an enemy and causes it 
                //        to bleed for 4500 to 5500 damage per application every 3 sec. for 15 sec.
                // Only goes to Off-Tank
                this[i].Attacks.Add(new DoT
                {
                    Name = "Delirious Slash",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    TickInterval = 3f,
                    NumTicks = 5f,
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content] * new float[] { .5f, .5f, .75f, .75f }[i],
                    DamagePerTick = new int[] { (4500 + 5500), (5250 + 6750), (6125 + 7875), (7000 + 9000) }[i] / 2f,
                    MaxNumTargets = 1f,
                    AttackSpeed = 20f,

                    Missable = true,
                    Parryable = true,
                    Dodgable = true,
                    Blockable = true,

                    IgnoresMTank = true,
                    IgnoresTTank = true,
                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });

                // Vampiric Bite - Inflicts 12,025 to 13,975 physical damage to a target, granting them Essence of the Blood Queen
                this[i].Attacks.Add(new Attack
                {
                    Name = "Vampiric Bite",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = new int[] { (8325 + 9675), (10175 + 11825), (8325 + 9675), (10175 + 11825) }[i] / 2f,
                    MaxNumTargets = new int[] { avgbitetargets10, avgbitetargets25, avgbitetargets10, avgbitetargets25 }[i],
                    // Bites are usable every 75/60 seconds in 10/25 respectfully
                    AttackSpeed = new int[] { 75, 60, 75, 60 }[i] + 5f, // Assuming biting within 5 seconds of the 15 seconds
                    // The OT COULD be targeted but only as a last resourt. The MT should NOT be targeted.
                    IgnoresMTank = true,
                });
                // Bitters or bittees need to move to get bitten
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = this[i].Attacks[this[i].Attacks.Count - 1].AttackSpeed,
                    Duration = 3f * 1000f, // Assume 3 seconds to move to the new biting target
                    Chance = 1f / new int[] { avgbitetargets10, avgbitetargets25, avgbitetargets10, avgbitetargets25 }[i],
                    Breakable = false,
                });

                // If the person doesn't bite a non-vampire player within 15 seconds
                // They go into a Uncontrollable Frenzy where they are Mind Controlled for 60 seconds then die after that period
                this[i].Attacks.Add(new Attack
                {
                    Name = "Uncontrollable Frenzy",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = 1f,
                    DamageIsPerc = true,
                    MaxNumTargets = new int[] { avgbitetargets10, avgbitetargets10, avgbitetargets25, avgbitetargets25 }[i],
                    // Bites are usable every 75/60 seconds in 10/25 respectfully
                    AttackSpeed = new int[] { 75, 60, 75, 60 }[i] + 5f, // Assuming biting within 5 seconds of the 15 seconds
                    IgnoresMTank = true,
                    Interruptable = true, // Can be avoided if biten correctly
                });
                // if biten incorrectly, the bitter is mind controlled
                this[i].Stuns.Add(new Impedance()
                {
                    Frequency = this[i].Attacks[this[i].Attacks.Count - 1].AttackSpeed,
                    Duration = 60f * 1000f, // 60 seconds of mind control before dying
                    Chance = 1f / new int[] { avgbitetargets10, avgbitetargets10, avgbitetargets25, avgbitetargets25 }[i],
                    Breakable = false,
                });

                // Twilight Bloodbolt - Inflicts 9,250 to 10,750 shadow damage to a target and other players within 6 yards.
                this[i].Attacks.Add(new Attack
                {
                    Name = "Twilight Bloodbolt",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = new int[] { (9250 + 10750), (11250 + 13750), (9250 + 10750), (11250 + 13750) }[i] / 2f, // assume range is spread out and doesn't do any splash damage
                    MaxNumTargets = new int[] { 2, 3, 2, 3 }[i], // assume range is spread out so that ther is no AOE
                    // TODO: Double Check attack Speed
                    AttackSpeed = 20f,
                    IgnoresMTank = true,
                    IgnoresOTank = true,
                    IgnoresMeleeDPS = true,
                });

                // Swarming Shadows - The affected player spawns a mass of shadows every 1 second for 6 seconds. Each mass inflicts 
                //         2,313 to 2,687 shadow damage every 1 second to all targets within it.
                this[i].Attacks.Add(new DoT
                {
                    Name = "Swarming Shadows",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    NumTicks = 6f,
                    TickInterval = 1f,
                    // TODO: Double Check Damage per tick
                    DamagePerTick = new int[] { (2313 + 2687), (2313 + 2687), (2313 + 2687), (2313 + 2687) }[i] / 2f, // assume range is spread out and doesn't do any splash damage
                    MaxNumTargets = new int[] { 1, 1, 1, 1 }[i],
                    // TODO: Double Check attack Speed
                    AttackSpeed = 30.5f,
                    IgnoresMTank = true,
                    IgnoresOTank = true,
                });
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = this[i].Attacks[this[i].Attacks.Count - 1].AttackSpeed,
                    Duration = 6f * 1000f, // move the shadows to the wall for 6 seconds
                    Chance = 1f / (this[i].Max_Players - this[i].Min_Tanks),
                    Breakable = false,
                });

                // Pact of the Darkfallen - Links a number of players together, causing them to inflict 
                //       3,500 Shadow damage every 1 second to non-linked players within 10 yards.
                //       This effect expires when all linked targets are within 5 yards of each other.
                this[i].Attacks.Add(new DoT
                {
                    Name = "Pact of the Darkfallen",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    NumTicks = 4f,  // assume 4 seconds to remove the debuff
                    TickInterval = 1f,
                    DamagePerTick = new int[] { 3500, 3500, 3500, 3500 }[i],
                    // Assume half of the raid get hit by by the debuff while people are running to get rid of the debuff
                    MaxNumTargets = this[i].Max_Players / 2f,
                    AttackSpeed = 30.5f,
                });
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = this[i].Attacks[this[i].Attacks.Count - 1].AttackSpeed,
                    Duration = 4f * 1000f, // assume 4 seconds to remove the debuff
                    Chance = new float[] { 2, 3, 2, 3 }[i] / (this[i].Max_Players - this[i].Min_Tanks),
                    Breakable = false,
                });

                // Bloodbolt Whirl - Shoots a lot of Twilight Bloodbolts to random players for 6 seconds.
                this[i].Attacks.Add(new Attack
                {
                    Name = "Bloodbolt Whirl",
                    DamageType = ItemDamageType.Shadow,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = new int[] { (9250 + 10750), (12025 + 13975), (14250 + 15750), (16150 + 17850) }[i] / 2f, // assume range is spread out and doesn't do any splash damage
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = (new int[] { 124, 127, 124, 127 }[i] + 100f + 100f) / 3f,
                });
                // Right before the Bloodbolt Whirl, the boss fears everyone for 4 seconds. This can be dispelled/ fear warded
                this[i].Fears.Add(new Impedance()
                {
                    Frequency = this[i].Attacks[this[i].Attacks.Count - 1].AttackSpeed - 6f,
                    Duration = 4f * 1000,
                    Chance = this[i].Max_Players / this[i].Max_Players,
                    Breakable = true,
                });
                // After the fear is gone, everyone has 3 seconds to spread out for the Bloodbolt whirl.
                //      If people are too close to each other, they cause splash damage to each other.
                //      Some people can find spots immediatly upon release of fear so it is "breakable."
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = this[i].Attacks[this[i].Attacks.Count - 1].AttackSpeed - 2f,
                    Duration = 3f * 1000,
                    Chance = 1f,
                    Breakable = true,
                });

                // TODO: Essence of the Blood Queen - Increases damage by 100%, and causes attacks to 
                //       heal the player for 15% of the damage done, as well as cause no threat.
                // Lasts 75 seconds on 10 man, 60 seconds on 25 man
            }
            #endregion
            #endregion
            #region Impedances
            //Moves;
            //Stuns;
            //Fears;
            //Roots;
            //Disarms;
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0.00f, 0.00f };
            #endregion
            /* TODO:
             */
        }
    }
    #endregion

    #region The Frostwing Hall
    public class ValathriaDreamwalker : MultiDiffBoss
    {
        public ValathriaDreamwalker()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Valathria Dreamwalker";
            Instance = "Icecrown Citadel";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_9, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10H, BossHandler.Versions.V_25H, };
            #endregion
            #region Basics
            // Health is half her total health, which you HEAL, not dps
            Health = new float[] { 12000007f / 2f, 36002816f / 2f, 12000007f / 2f, 35999996f / 2f };
            BerserkTimer = new int[] { 7 * 60, 7 * 60, 7 * 60, 7 * 60, };
            SpeedKillTimer = new int[] { 180, 240, 240, 240 };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 2, 2, 3 }; // 1-2 Tertiary Tanks to kite the Blistering Zombie on Heroic 25
            Min_Healers = new int[] { 3, 5, 3, 6 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 1, 1 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 4; i++)
            {
                // Valathria does not actually fight you, you fight a crap load of adds while healers heal Valathria back to full health
                // Heroic, Valathria does take damage
            }
            #endregion
            #endregion
            #region Impedances
            //Moves;
            //Stuns;
            //Fears;
            //Roots;
            //Disarms;
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0.00f, 0.00f };
            #endregion
            /* TODO:
             */
        }
    }
    public class Sindragosa : MultiDiffBoss
    {
        public Sindragosa()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Sindragosa";
            Instance = "Icecrown Citadel";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_9, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10H, BossHandler.Versions.V_25H, };
            #endregion
            #region Basics
            Health = new float[] { 11156000f, 38348752f, 13945000f, 46018500f };
            BerserkTimer = new int[] { 10 * 60, 10 * 60, 10 * 60, 10 * 60, };
            SpeedKillTimer = new int[] { 210, 210, 300, 270 };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 2, 2, 2 };
            Min_Healers = new int[] { 3, 5, 3, 5 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 1, 1 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 4; i++)
            {
                this[i].Attacks.Add(new Attack
                {
                    // Melee Attacks
                    Name = "Melee",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    // Hits for about 18k on normal 25
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = 1f,
                    AttackSpeed = 2f,

                    Dodgable = true,
                    Missable = true,
                    Parryable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,

                    IsTheDefaultMelee = true,
                });
            }
            #endregion
            #endregion
            #region Impedances
            //Moves;
            //Stuns;
            //Fears;
            //Roots;
            //Disarms;
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0.00f, 0.00f };
            #endregion
            /* TODO:
             */
        }
    }
    #endregion

    #region The Frozen Throne
    public class TheLichKing : MultiDiffBoss
    {
        public TheLichKing()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "The Lich King";
            Instance = "Icecrown Citadel";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_9, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10H, BossHandler.Versions.V_25H, };
            #endregion
            #region Basics
            // You have 15 minutes to bring him down to 10.4%. After which point it's a free kill with no berserker time.
            Health = new float[] { 17431250f * .896f, 61009376f * .896f, 29458812f * .896f, 103151168f * .896f };
            BerserkTimer = new int[] { 15 * 60, 15 * 60, 15 * 60, 15 * 60 };
            SpeedKillTimer = new int[] { 8 * 60, 13 * 60, 8 * 60, 13 * 60 };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 2, 2, 2 };
            Min_Healers = new int[] { 3, 5, 3, 5 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 1, 1 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 4; i++)
            {
                this[i].Attacks.Add(new Attack
                {
                    // Melee Attacks
                    Name = "Melee",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    // Hits for about 18k on normal 25
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = 1f,
                    AttackSpeed = 2f,

                    Dodgable = true,
                    Missable = true,
                    Parryable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,

                    IsTheDefaultMelee = true,
                });
            }
            #endregion
            #endregion
            #region Impedances
            //Moves;
            //Stuns;
            //Fears;
            //Roots;
            //Disarms;
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0.00f, 0.00f };
            #endregion
            /* TODO:
             */
        }
    }
    #endregion
    #endregion

    #region Ruby Sanctum
    // ===== Ruby Sanctum =========================
    public class Baltharus : MultiDiffBoss
    {
        public Baltharus()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Baltharus";
            Instance = "Ruby Sanctum";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_9, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10H, BossHandler.Versions.V_25H, };
            #endregion
            #region Basics
            // Normal and Heroic setting provide no difference to his health or abilities
            Health = new float[] { 3486000f, 11156000f, 0f, 0f };
            BerserkTimer = new int[] { 10 * 60, 10 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 120, 120, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 2, 0, 0 };
            Min_Healers = new int[] { 3, 5, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 1, 1 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                // 300 / (300 * (.5 * .5)) = 300 / 75 = 4
                int cloneattackspeed10man = (int)(BerserkTimer[i] / (BerserkTimer[i] * (.5f * .5f)));
                // 300 / ( ( 300 * (.66 * .5 ) ) + ( 300 * (.33 * .5) ) ) = 300 / ( 99 + 49.5 ) = 300 / 148.5 = 2.020202
                int cloneattackspeed25man = (int)(BerserkTimer[i] / ((BerserkTimer[i] * (.66f * .5f)) + (BerserkTimer[i] * (.33f * .5f))));

                this[i].Attacks.Add(new Attack
                {
                    // Melee Attacks
                    Name = "Melee",
                    DamageType = ItemDamageType.Physical,
                    // Hits for about 18k on normal 25
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = 1f,
                    // TODO: Double Check Enervating Brand's CD; minus 20 seconds; all divided by 2 seconds
                    AttackSpeed = 3f,
                    AttackType = ATTACK_TYPES.AT_MELEE,

                    Missable = true,
                    Parryable = true,
                    Dodgable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,

                    IsTheDefaultMelee = true,
                });

                // At 50% in 10man and 66% and 33% in 25 man, the boss will split himself up; the new add will be picked
                //    by the off-tank. DPS does not attack the adds and focus on the main boss
                //    The boss will then perform a knockback/stun when he does this ability that lasts for 4 seconds
                //    to everyone in melee range of him
                // Can be 3 tanked if needbe (1 OT and 1 TT) if the damage is too much for one off-tank in 25 man, 
                //              though not necessary
                this[i].Attacks.Add(new Attack
                {
                    Name = "Cloned Melee",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    DamagePerHit = new int[] { 1, (1 + 2) / 2, 0, 0 }[i] * BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = 1f,
                    AttackSpeed = new int[] { cloneattackspeed10man, cloneattackspeed25man, cloneattackspeed10man, cloneattackspeed25man }[i],

                    Missable = true,
                    Parryable = true,
                    Dodgable = true,
                    Blockable = true,

                    IgnoresMTank = true,
                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });

                // Enervating Brand: Brands an enemy, drawing energy from him and allies within 12 yards. 
                //     Stacks every 2 seconds, reducing the enemies' damage by 2% and increasing the caster's damage by 2%. Lasts 20 seconds.
                // Siphoned Might: Increases the caster's damage by 2% per applied brands.
                this[i].Attacks.Add(new Attack
                {
                    Name = "Siphoned Might",
                    DamageType = ItemDamageType.Arcane,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    // Assuming everyone is spread out
                    DamagePerHit = ((1f + (1f + (.02f * 20f / 2f))) / 2f) * BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = 1f,
                    // TODO: Double Check Enervating Brand's CD; minus normal attacks uptime; all divided by 2 seconds
                    AttackSpeed = (30f - 10f) / 3f,

                    IgnoresOTank = true,
                    IgnoresTTank = true,
                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });

                // Blade Tempest: Deals 70% of weapon damage to enemies in front of the attacker. 
                this[i].Attacks.Add(new Attack
                {
                    Name = "Blade Tempest",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content] * .7f,
                    MaxNumTargets = this[i].Min_Tanks,
                    AttackSpeed = 1.5f,

                    Missable = true,
                    Parryable = true,
                    Dodgable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });

                // Repelling Wave: Knocks nearby enemies back, inflicting 4,163 to 4,837 Fire damage, and stunning them for 3 seconds.
                //        Used while summoning a clone.
                this[i].Attacks.Add(new Attack
                {
                    Name = "Repelling Wave",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content] * .7f,
                    MaxNumTargets = this[i].Max_Players - this[i].Min_Healers,

                    IgnoresOTank = true,
                    IgnoresTTank = true,
                    IgnoresHealers = true,
                    IgnoresRangedDPS = true,
                });
                // Repelling wave stuns melee for 3 seconds. At the same time, the boss knocks Melee away from the boss
                // This stun is scripted and cannot be dispelled
                this[i].Stuns.Add(new Impedance()
                {
                    Frequency = this[i].Attacks[this[i].Attacks.Count - 1].AttackSpeed,
                    Duration = 3f * 1000f,
                    Chance = 1f / (this[i].Max_Players - this[i].Min_Healers),
                    Breakable = false,
                });
                // Allow 3 seconds for melee to run back to the boss after the stun.
                this[i].Moves.Add(new Impedance()
                {
                    Frequency = this[i].Attacks[this[i].Attacks.Count - 1].AttackSpeed,
                    Duration = 3f * 1000f,
                    Chance = 1f / (this[i].Max_Players - this[i].Min_Healers),
                    Breakable = false,
                });

                // Cleave: Inflicts 110% of normal melee damage to an enemy and its nearest allies, affecting up to 3 targets. 
                this[i].Attacks.Add(new Attack
                {
                    Name = "Cleave",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content] * 1.1f,
                    MaxNumTargets = 3f,
                    AttackSpeed = 20f,

                    Missable = true,
                    Parryable = true,
                    Dodgable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });
            }
            #endregion
            #endregion
            #region Impedances
            //Moves;
            //Stuns;
            //Fears;
            //Roots;
            //Disarms;
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0.00f, 0.00f };
            #endregion
            /* TODO:
             *  Enervating Brand: Brands an enemy, drawing energy from him and allies within 12 yards. 
             *        Stacks every 2 seconds, reducing the enemies' damage by 2%. Lasts 20 seconds.
             */
        }
    }
    public class SavianaRagefire : MultiDiffBoss
    {
        public SavianaRagefire()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Saviana Ragefire";
            Instance = "Ruby Sanctum";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_9, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10H, BossHandler.Versions.V_25H, };
            #endregion
            #region Basics
            // Normal and Heroic setting provide no difference to her health or abilities
            Health = new float[] { 4183000f, 13945000f, 0f, 0f };
            BerserkTimer = new int[] { 10 * 60, 10 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 120, 120, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 2, 0, 0 };
            Min_Healers = new int[] { 3, 5, 0, 0 };
            #endregion
            #region Offensive
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(new Attack
                {
                    // Melee Attacks
                    Name = "Melee",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    // Hits for about 18k on normal 25
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = 1f,
                    AttackSpeed = 2f,

                    Missable = true,
                    Parryable = true,
                    Dodgable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,

                    IsTheDefaultMelee = true,
                });

                // Flame Breath - Inflicts 24,500 to 31,500 Fire damage to enemies in a cone in front of Saviana.
                this[i].Attacks.Add(new Attack
                {
                    Name = "Flame Breath",
                    DamageType = ItemDamageType.Fire,
                    DamagePerHit = new int[] { (24500 + 31500), (33687 + 43312), 0, 0 }[i] / 2f,
                    MaxNumTargets = 1f,
                    // First attack is 12 seconds in, and every 25 seconds after
                    AttackSpeed = (this[i].BerserkTimer / ((this[i].BerserkTimer - 12f) / 25f)),
                    AttackType = ATTACK_TYPES.AT_MELEE,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });

                // Enrage - Decreases the time between your attacks by 150% for 10 sec.
                this[i].Attacks.Add(new DoT
                {
                    Name = "Enrage",
                    DamageType = ItemDamageType.Physical,
                    NumTicks = 10f / (2f / 1.5f),
                    TickInterval = (2f / 1.5f),
                    DamagePerTick = BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = this[i].Min_Tanks,
                    AttackSpeed = 30f,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    Interruptable = true,

                    Missable = true,
                    Parryable = true,
                    Dodgable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,
                });

                // Fire Nova - Inflicts 7,601 to 7,600 Fire damage to players within 100 yards.
                this[i].Attacks.Add(new Attack
                {
                    Name = "Fire Nova",
                    DamageType = ItemDamageType.Fire,
                    DamagePerHit = 7600f,
                    MaxNumTargets = this[i].Max_Players,
                    AttackSpeed = 15f,
                    AttackType = ATTACK_TYPES.AT_AOE,
                });
            }
            #endregion
            #endregion
            #region Impedances
            //Moves;
            //Stuns;
            for (int i = 0; i < 4; i++)
            {
                // Conflagration - Causes the affected players to temporarily do Fire damage to all nearby allies.
                //    Assume all players are spread out; Can target tanks; Stuns targets for 6 seconds
                this[i].Stuns.Add(new Impedance()
                {
                    Frequency = this[i].BerserkTimer / ((this[i].BerserkTimer - 32f) / 50f),
                    Duration = 6f * 1000f,
                    Chance = new float[] { 2, 5, 2, 5 }[i] / this[i].Max_Players,
                    Breakable = false,
                });
            }
            //Fears;
            //Roots;
            //Disarms;
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0.00f, 0.00f };
            #endregion
            /* TODO:
             */
        }
    }
    public class GeneralZarithrian : MultiDiffBoss
    {
        public GeneralZarithrian()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "General Zarithrian";
            Instance = "Ruby Sanctum";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_9, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10H, BossHandler.Versions.V_25H, };
            #endregion
            #region Basics
            // Normal and Heroic setting provide no difference to his health or abilities
            Health = new float[] { 4141000f, 14098395f, 0f, 0f };
            BerserkTimer = new int[] { 10 * 60, 10 * 60, 0, 0 };
            SpeedKillTimer = new int[] { 120, 120, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks = new int[] { 2, 2, 0, 0 };
            Min_Healers = new int[] { 3, 5, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 1, 1 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++)
            {
                this[i].Attacks.Add(new Attack
                {
                    // Melee Attacks
                    Name = "Melee",
                    DamageType = ItemDamageType.Physical,
                    AttackType = ATTACK_TYPES.AT_MELEE,
                    // Hits for about 18k on normal 25
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = 1f,
                    AttackSpeed = 2f,

                    Missable = true,
                    Parryable = true,
                    Dodgable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,

                    IsTheDefaultMelee = true,
                });

                // Summon Flamecaller - Summons 3 Onyx Flamecallers.
                this[i].Targets.Add(new TargetGroup()
                {
                    // First set of adds is after 15.5 seconds, then every 45.5 seconds after
                    Frequency = this[i].BerserkTimer / ((this[i].BerserkTimer - 15.5f) / 45.5f),
                    Chance = 1f,
                    Duration = 15f * 1000f, // Assume 15 seconds to kill adds; Can be ignored and just burn the boss
                    NearBoss = false,
                    NumTargs = 3f,
                });
                // Blast Nova - Inflicts 5688 to 7312 Fire damage to nearby enemies.
                this[i].Attacks.Add(new Attack()
                {
                    Name = "Blast Nova",
                    DamageType = ItemDamageType.Fire,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = new int[] { (5688 + 7312), (6563 + 8437), 0, 0 }[i] / 2f,
                    MaxNumTargets = this[i].Max_Players,
                    // Averaging Flamecallers frequency by their duration, assume that the normal cast of the spell is every 5 seconds; All devided by the number of targets
                    AttackSpeed = ((this[i].Targets[this[i].Targets.Count - 1].Frequency * 1000f) / (this[i].Targets[this[i].Targets.Count - 1].Duration / 5f)) / this[i].Targets[this[i].Targets.Count - 1].NumTargs,

                    Missable = true,
                });
                // Lava Gout - Inflicts 8483 to 9517 Fire damage to an enemy; Can be interrupted
                this[i].Attacks.Add(new Attack()
                {
                    Name = "Lava Gout",
                    DamageType = ItemDamageType.Fire,
                    AttackType = ATTACK_TYPES.AT_AOE,
                    DamagePerHit = new int[] { (8483 + 9517), (9897 + 9897), 0, 0 }[i] / 2f,
                    MaxNumTargets = this[i].Targets[this[i].Targets.Count - 1].NumTargs,
                    // Averaging Flamecallers frequency by their duration, assume that the normal cast of the spell is every 2 seconds; All devided by the number of targets
                    AttackSpeed = ((this[i].Targets[this[i].Targets.Count - 1].Frequency * 1000f) / (this[i].Targets[this[i].Targets.Count - 1].Duration / 2f)) / this[i].Targets[this[i].Targets.Count - 1].NumTargs,
                    Interruptable = true,

                    Missable = true,
                });
            }
            #endregion
            #endregion
            #region Impedances
            //Moves;
            //Stuns;
            //Fears;
            //Roots;
            //Disarms;
            for (int i = 0; i < 4; i++)
            {
                // Intimidating Roar - Causes all raid members to be stunned in fear for 4 seconds.
                //         This is a scripted event that cannot be broken
                this[i].Stuns.Add(new Impedance()
                {
                    // First stun is after 14 seconds, then every 33 seconds after
                    Frequency = this[i].BerserkTimer / ((this[i].BerserkTimer - 14f) / 33f),
                    Chance = 1f,
                    Duration = 4f * 1000f,
                    Breakable = false,
                });
            }
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0.00f, 0.00f };
            #endregion
            /* TODO:
             *    Add tank Armor reducing ability
             *    Cleave Armor - Reduces the target's [tanks] armor by 20% for 30 seconds. Stacks up to 5 times.
             *                 Tanks taunt after 3 stacks
             */
        }
    }
    public class Halion : MultiDiffBoss
    {
        public Halion()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Halion";
            Instance = "Ruby Sanctum";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T10_0, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_5, BossHandler.TierLevels.T10_9, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10H, BossHandler.Versions.V_25H, };
            #endregion
            #region Basics
            // Fight is split into 3 phases, first two everyone is DPSing him together, Phase 3 DPS is split up between live and shadow realm
            Health = new float[] { 11156000f, 40440500f, 15339900f, 58569000f };
            BerserkTimer = new int[] { 8 * 60, 8 * 60, 8 * 60, 8 * 60, };
            SpeedKillTimer = new int[] { 300, 300, 300, 300 };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 2, 2, 2 };
            Min_Healers = new int[] { 3, 5, 3, 5 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 1, 1 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 4; i++)
            {
                this[i].Attacks.Add(new Attack
                {
                    // Melee Attacks
                    Name = "Melee",
                    DamageType = ItemDamageType.Physical,
                    // Hits for about 18k on normal 25
                    DamagePerHit = BossHandler.StandardMeleePerHit[(int)this[i].Content],
                    MaxNumTargets = 1f,
                    AttackSpeed = 2f,
                    AttackType = ATTACK_TYPES.AT_MELEE,

                    Missable = true,
                    Parryable = true,
                    Dodgable = true,
                    Blockable = true,

                    IgnoresMeleeDPS = true,
                    IgnoresRangedDPS = true,
                    IgnoresHealers = true,

                    IsTheDefaultMelee = true,
                });
            }
            #endregion
            #endregion
            #region Impedances
            //Moves;
            //Stuns;
            //Fears;
            //Roots;
            //Disarms;
            TimeBossIsInvuln = new float[] { 0.00f, 0.00f, 0.00f, 0.00f };
            #endregion
            /* TODO:
             */
        }
    }
    #endregion
}