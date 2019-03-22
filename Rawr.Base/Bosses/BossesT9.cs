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
    public class KoralonTheFlameWatcher : MultiDiffBoss {
        public KoralonTheFlameWatcher() {
            // If not listed here use values from defaults
            #region Info
            Name = "Koralon the Flame Watcher";
            Instance = "The Vault of Archavon";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T9_0, BossHandler.TierLevels.T9_5, BossHandler.TierLevels.T9_0, BossHandler.TierLevels.T9_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 4183500f, 4183500f, 0, 0 };
            BerserkTimer = new int[] { 19*60, 19*60, 0, 0 };
            SpeedKillTimer = new int[] { 3*60, 3*60, 0, 0 };
            InBackPerc_Melee = new double[] { 0.95f, 0.95f, 0, 0 };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0, 0 };
            Max_Players = new int[] { 10, 25, 0, 0 };
            Min_Tanks   = new int[] {  2,  2, 0, 0 };
            Min_Healers = new int[] {  2,  4, 0, 0 };
            #endregion
            #region Offensive
            //MaxNumTargets = new double[] { 1, 1, 0, 0 };
            //MultiTargsPerc = new double[] { 0.00d, 0.00d, 0.00d, 0.00d };
            #region Attacks
            for (int i = 0; i < 2; i++) {
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
            for (int i = 0; i < 2; i++) {
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

    #region Trial of the Crusader
    // ===== Trial of the Crusader ====================
    public class NorthrendBeasts : MultiDiffBoss
    {
        public NorthrendBeasts()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Northrend Beasts";
            Instance = "Trial of the Crusader";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T9_0, BossHandler.TierLevels.T9_5, BossHandler.TierLevels.T9_0, BossHandler.TierLevels.T9_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            // Gormok the Impaler (a magnataur), Acidmaw and Dreadscale (a pair of jormungar) and Icehowl (a yeti).
            Health = new float[] { (2230000 + 1255050 + 1255050 + 3486250), (8924800 + 5020200 + 5020200 + 13247750), (2789000 + 1673400 + 1673400 + 4648300), (11853250 + 6693600 + 6693600 + 18128500) };
            BerserkTimer = new int[] { 19 * 60, 19 * 60, 10 * 60, 10 * 60 };
            SpeedKillTimer = new int[] { 5 * 60, 5 * 60, 5 * 60, 5 * 60 };
            InBackPerc_Melee = new double[] { 0.95f, 0.95f, .95f, .95f };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0.00f, 0.00f };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 2, 2, 2 };
            Min_Healers = new int[] { 2, 4, 2, 5 };
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
                    DamagePerHit = BossHandler.StandardMeleePerHit[ (int)this[i].Content ],
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
            for (int i = 0; i < 4; i++)
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
             *  - Get actual Acidmaw and Dreadscale Health for 10man Heroic (currently estimated based on the health increase for 25 normal and heroic)
             *  - Get actual Icehowl Health for 10man Heroic (currently estimated based on the health increase for 25 normal and heroic)
             */
        }
    }

    public class LordJaraxxus : MultiDiffBoss
    {
        public LordJaraxxus()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Lord Jaraxxus";
            Instance = "Trial of the Crusader";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T9_0, BossHandler.TierLevels.T9_5, BossHandler.TierLevels.T9_0, BossHandler.TierLevels.T9_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 4183000, 20200000, 5300000, 26500000 };
            BerserkTimer = new int[] { 10 * 60, 10 * 60, 10 * 60, 10 * 60 };
            SpeedKillTimer = new int[] { 5 * 60, 5 * 60, 5 * 60, 5 * 60 };
            InBackPerc_Melee = new double[] { 0.95f, 0.95f, .95f, .95f };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0.00f, 0.00f };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 2, 2, 2 };
            Min_Healers = new int[] { 2, 4, 2, 5 };
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
            for (int i = 0; i < 4; i++)
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

    public class FactionChampions : MultiDiffBoss
    {
        public FactionChampions()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Faction Champions";
            Instance = "Trial of the Crusader";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T9_0, BossHandler.TierLevels.T9_5, BossHandler.TierLevels.T9_0, BossHandler.TierLevels.T9_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            // Since clothies have lower health than non-clothies, I'm averaging out everyone's health combined then multiplying by the number of champions
            // Clothies (4) - Mage, Warlock, Shadow Priest, Disc Priest
            // Non-Clothies (10) - DK, Boomkin, Resto Druid, Hunter, Holy Pally, Ret Pally, Rogue, Resto Sham, Enh Sham, Warrior
            Health = new float[] { ((322560 * 4) + (403200 * 10)) * 6 / 14, ((1935360 * 4) + (2419000 * 10)) * 10 / 14, ((483840 * 4) + (604800 * 10)) * 6 / 14, ((2580480 * 4) + (3225600 * 10)) * 10 / 14 };
            BerserkTimer = new int[] { 19 * 60, 19 * 60, 19 * 60, 19 * 60 };
            SpeedKillTimer = new int[] { 5 * 60, 5 * 60, 5 * 60, 5 * 60 };
            InBackPerc_Melee = new double[] { 0.95f, 0.95f, .95f, .95f };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0.00f, 0.00f };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 2, 2, 2 };
            Min_Healers = new int[] { 2, 4, 2, 5 };
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
            for (int i = 0; i < 4; i++)
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
             *  - Double Check clothies 10man Heroic Health
             */
        }
    }

    public class TwinValkyr : MultiDiffBoss
    {
        public TwinValkyr()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Twin Val'kyr";
            Instance = "Trial of the Crusader";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T9_0, BossHandler.TierLevels.T9_5, BossHandler.TierLevels.T9_0, BossHandler.TierLevels.T9_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            // Composed of Eydis Darkbane and Combat Fjola Lightbane; both share the same health pool 
            Health = new float[] { 5790000, 27980000, 8376000, 39000000 };
            BerserkTimer = new int[] { 10 * 60, 10 * 60, 10 * 60, 10 * 60 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 3 * 60, 3 * 60 };
            InBackPerc_Melee = new double[] { 0.95f, 0.95f, .95f, .95f };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0.00f, 0.00f };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 2, 2, 2 };
            Min_Healers = new int[] { 2, 4, 2, 5 };
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
            for (int i = 0; i < 4; i++)
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

    public class Anubarak : MultiDiffBoss
    {
        public Anubarak()
        {
            // If not listed here use values from defaults
            #region Info
            Name = "Anub'arak";
            Instance = "Trial of the Crusader";
            Content = new BossHandler.TierLevels[] { BossHandler.TierLevels.T9_0, BossHandler.TierLevels.T9_5, BossHandler.TierLevels.T9_0, BossHandler.TierLevels.T9_5, };
            Version = new BossHandler.Versions[] { BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, BossHandler.Versions.V_10N, BossHandler.Versions.V_25N, };
            #endregion
            #region Basics
            Health = new float[] { 4180000, 20910000, 5440000, 27200000 };
            BerserkTimer = new int[] { 10 * 60, 10 * 60, 10 * 60, 10 * 60 };
            SpeedKillTimer = new int[] { 3 * 60, 3 * 60, 3 * 60, 3 * 60 };
            InBackPerc_Melee = new double[] { 0.75f, 0.75f, .75f, .75f };
            InBackPerc_Ranged = new double[] { 0.00f, 0.00f, 0.00f, 0.00f };
            Max_Players = new int[] { 10, 25, 10, 25 };
            Min_Tanks = new int[] { 2, 2, 2, 2 };
            Min_Healers = new int[] { 2, 4, 2, 5 };
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
            for (int i = 0; i < 4; i++)
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
}