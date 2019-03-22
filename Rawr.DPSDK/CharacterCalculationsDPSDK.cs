using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.DPSDK
{
    public class CharacterCalculationsDPSDK : CharacterCalculationsBase
    {
        private float _overallPoints = 0f;
        public override float OverallPoints
        {
            get { return _overallPoints; }
            set { _overallPoints = value; }
        }

        private float[] _subPoints = new float[] { 0f };
        public override float[] SubPoints
        {
            get { return _subPoints; }
            set { _subPoints = value; }
        }

        public float DPSPoints
        {
            get { return _subPoints[0]; }
            set { _subPoints[0] = value; }
        }

        private float _whiteDPS;
        public float WhiteDPS
        {
            get { return _whiteDPS; }
            set { _whiteDPS = value; }
        }

        private float _NecrosisDPS;
        public float NecrosisDPS
        {
            get { return _NecrosisDPS; }
            set { _NecrosisDPS = value; }
        }

        private float _BCBDPS;
        public float BCBDPS
        {
            get { return _BCBDPS; }
            set { _BCBDPS = value; }
        }

        private float _DeathCoilDPS;
        public float DeathCoilDPS
        {
            get { return _DeathCoilDPS; }
            set { _DeathCoilDPS = value; }
        }

        private float _IcyTouchDPS;
        public float IcyTouchDPS
        {
            get { return _IcyTouchDPS; }
            set { _IcyTouchDPS = value; }
        }

        private float _PlagueStrikeDPS;
        public float PlagueStrikeDPS
        {
            get { return _PlagueStrikeDPS; }
            set { _PlagueStrikeDPS = value; }
        }

        private float _FrostFeverDPS;
        public float FrostFeverDPS
        {
            get { return _FrostFeverDPS; }
            set { _FrostFeverDPS = value; }
        }

        private float _BloodPlagueDPS;
        public float BloodPlagueDPS
        {
            get { return _BloodPlagueDPS; }
            set { _BloodPlagueDPS = value; }
        }

        private float _ScourgeStrikeDPS;
        public float ScourgeStrikeDPS
        {
            get { return _ScourgeStrikeDPS; }
            set { _ScourgeStrikeDPS = value; }
        }

        private float _UnholyBlightDPS;
        public float UnholyBlightDPS
        {
            get { return _UnholyBlightDPS; }
            set { _UnholyBlightDPS = value; }
        }

        private float _BloodwormsDPS;
        public float BloodwormsDPS
        {
            get { return _BloodwormsDPS; }
            set { _BloodwormsDPS = value; }
        }

        private float _OtherDPS;
        public float OtherDPS
        {
            get { return _OtherDPS; }
            set { _OtherDPS = value; }
        }

        private float _FrostStrikeDPS;
        public float FrostStrikeDPS
        {
            get { return _FrostStrikeDPS; }
            set { _FrostStrikeDPS = value; }
        }

        private float _HowlingBlastDPS;
        public float HowlingBlastDPS
        {
            get { return _HowlingBlastDPS; }
            set { _HowlingBlastDPS = value; }
        }

        private float _ObliterateDPS;
        public float ObliterateDPS
        {
            get { return _ObliterateDPS; }
            set { _ObliterateDPS = value; }
        }

        private float _DeathStrikeDPS;
        public float DeathStrikeDPS
        {
            get { return _DeathStrikeDPS; }
            set { _DeathStrikeDPS = value; }
        }

        private float _BloodStrikeDPS;
        public float BloodStrikeDPS
        {
            get { return _BloodStrikeDPS; }
            set { _BloodStrikeDPS = value; }
        }

        private float _HeartStrikeDPS;
        public float HeartStrikeDPS
        {
            get { return _HeartStrikeDPS; }
            set { _HeartStrikeDPS = value; }
        }

        private float _GargoyleDPS;
        public float GargoyleDPS
        {
            get { return _GargoyleDPS; }
            set { _GargoyleDPS = value; }
        }

        private float _DRWDPS;
        public float DRWDPS
        {
            get { return _DRWDPS; }
            set { _DRWDPS = value; }
        }

        private float _WanderingPlagueDPS;
        public float WanderingPlagueDPS
        {
            get { return _WanderingPlagueDPS; }
            set { _WanderingPlagueDPS = value; }
        }

        private int _targetLevel;
        public int TargetLevel
        {
            get { return _targetLevel; }
            set { _targetLevel = value; }
        }

        private float _MHweaponDamage;
        public float MHWeaponDamage
        {
            get { return _MHweaponDamage; }
            set { _MHweaponDamage = value; }
        }

        private float _OHWeaponDamage;
        public float OHWeaponDamage
        {
            get { return _OHWeaponDamage; }
            set { _OHWeaponDamage = value; }
        }

        private float _MHattackSpeed;
        public float MHAttackSpeed
        {
            get { return _MHattackSpeed; }
            set { _MHattackSpeed = value; }
        }

        private float _OHattackSpeed;
        public float OHAttackSpeed
        {
            get { return _OHattackSpeed; }
            set { _OHattackSpeed = value; }
        }

        private float _critChance;
        public float CritChance
        {
            get { return _critChance; }
            set { _critChance = value; }
        }

        private float _SpellCritChance;
        public float SpellCritChance
        {
            get { return _SpellCritChance; }
            set { _SpellCritChance = value; }
        }

        private float _avoidedAttacks;
        public float AvoidedAttacks
        {
            get { return _avoidedAttacks; }
            set { _avoidedAttacks = value; }
        }

        private float _dodgedMHAttacks;
        public float DodgedMHAttacks
        {
            get { return _dodgedMHAttacks; }
            set { _dodgedMHAttacks = value; }
        }

        private float _dodgedOHAttacks;
        public float DodgedOHAttacks
        {
            get { return _dodgedOHAttacks; }
            set { _dodgedOHAttacks = value; }
        }

        private float _dodgedAttacks;
        public float DodgedAttacks
        {
            get { return _dodgedAttacks; }
            set { _dodgedAttacks = value; }
        }

        private float _missedAttacks;
        public float MissedAttacks
        {
            get { return _missedAttacks; }
            set { _missedAttacks = value; }
        }

        private float _enemyMitigation;
        public float EnemyMitigation
        {
            get { return _enemyMitigation; }
            set { _enemyMitigation = value; }
        }
        private float _effectiveArmor;
        public float EffectiveArmor
        {
            get { return _effectiveArmor; }
            set { _effectiveArmor = value; }
        }
        private float _MHExpertise;
        public float MHExpertise
        {
            get { return _MHExpertise; }
            set { _MHExpertise = value; }
        }

        private float _OHExpertise;
        public float OHExpertise
        {
            get { return _OHExpertise; }
            set { _OHExpertise = value; }
        }

        private float _GhoulDPS;
        public float GhoulDPS
        {
            get { return _GhoulDPS; }
            set { _GhoulDPS = value; }
        }

        private String _DRWStats;
        public String DRWStats
        {
            get { return _DRWStats; }
            set { _DRWStats = value; }
        }

        private Stats _basicStats;
        public Stats BasicStats
        {
            get { return _basicStats; }
            set { _basicStats = value; }
        }

        private DeathKnightTalents _talents;
        public DeathKnightTalents Talents
        {
            get { return _talents; }
            set { _talents = value; }
        }


        public List<Buff> ActiveBuffs { get; set; }

        public override Dictionary<string, string> GetCharacterDisplayCalculationValues()
        {
            float critRating = BasicStats.CritRating;

            float hitRating = BasicStats.HitRating;

            float armorPenetrationRating = BasicStats.ArmorPenetrationRating;

            float attackPower = BasicStats.AttackPower;
            if (ActiveBuffs.Contains(Buff.GetBuffByName("Improved Hunter's Mark")))
                attackPower -= 110f * (1f + BasicStats.BonusAttackPowerMultiplier);

            Dictionary<string, string> dictValues = new Dictionary<string, string>();
            dictValues.Add("Health",            BasicStats.Health.ToString("N0"));
            dictValues.Add("Strength",          BasicStats.Strength.ToString("N0"));
            dictValues.Add("Agility",           string.Format("{0:0}*Provides {1:P} crit chance", BasicStats.Agility, StatConversion.GetCritFromAgility(BasicStats.Agility, CharacterClass.DeathKnight)));
            dictValues.Add("Attack Power",      attackPower.ToString("N0"));
            dictValues.Add("Crit Rating",       string.Format("{0:0}*Provides {1:P} crit chance", critRating, StatConversion.GetCritFromRating(critRating, CharacterClass.DeathKnight)));
            dictValues.Add("Hit Rating",        string.Format("{0:0}*Negates {1:P} melee miss / {2:P} spell miss", hitRating, StatConversion.GetPhysicalHitFromRating(hitRating, CharacterClass.DeathKnight), StatConversion.GetSpellHitFromRating(hitRating,CharacterClass.DeathKnight)));
            dictValues.Add("Expertise",         string.Format("{0:0.00} / {1:0.00}*Negates {2:P} / {3:P} dodge chance", MHExpertise, OHExpertise, StatConversion.GetDodgeParryReducFromExpertise(MHExpertise), StatConversion.GetDodgeParryReducFromExpertise(OHExpertise)));
            dictValues.Add("Haste Rating",      string.Format("{0:0}*Increases attack speed by {1:P}", BasicStats.HasteRating, StatConversion.GetHasteFromRating(BasicStats.HasteRating, CharacterClass.DeathKnight)));
            dictValues.Add("Armor Penetration Rating", armorPenetrationRating.ToString("N0"));
            dictValues.Add("Armor",             BasicStats.Armor.ToString("N0"));
            dictValues.Add("Resilience",        BasicStats.Resilience.ToString("F0"));

            dictValues.Add("Weapon Damage",     MHWeaponDamage.ToString("N2") + " / " + OHWeaponDamage.ToString("N2"));
            dictValues.Add("Attack Speed",      MHAttackSpeed.ToString("N2") + " / " + OHAttackSpeed.ToString("N2"));
            dictValues.Add("Crit Chance",       string.Format("{0:P}", CritChance));
            dictValues.Add("Avoided Attacks",   string.Format("{0:P}*{1:P} Dodged, {2:P} Missed", AvoidedAttacks, DodgedAttacks, MissedAttacks));
            dictValues.Add("Enemy Mitigation",  string.Format("{0:P}*{1:0} effective enemy armor", EnemyMitigation, EffectiveArmor));

            dictValues.Add("BCB",               string.Format("{0:N2}*{1:P}", BCBDPS, (float)BCBDPS/DPSPoints));
            dictValues.Add("Blood Plague",      string.Format("{0:N2}*{1:P}", BloodPlagueDPS, (float)BloodPlagueDPS/DPSPoints));
            dictValues.Add("Blood Strike",      string.Format("{0:N2}*{1:P}", BloodStrikeDPS, (float)BloodStrikeDPS/DPSPoints));
            dictValues.Add("Death Coil",        string.Format("{0:N2}*{1:P}", DeathCoilDPS, (float)DeathCoilDPS / DPSPoints));
            dictValues.Add("DRW",               string.Format("{0:N2}*{1:P}, wait for "+DRWStats+" proc", DRWDPS, (float)DRWDPS/DPSPoints));
            dictValues.Add("Frost Fever",       string.Format("{0:N2}*{1:P}", FrostFeverDPS, (float)FrostFeverDPS/DPSPoints));
            dictValues.Add("Frost Strike",      string.Format("{0:N2}*{1:P}", FrostStrikeDPS, (float)FrostStrikeDPS/DPSPoints));
            dictValues.Add("Gargoyle",          string.Format("{0:N2}*{1:P}", GargoyleDPS, (float)GargoyleDPS / DPSPoints));
            dictValues.Add("Heart Strike",      string.Format("{0:N2}*{1:P}", HeartStrikeDPS, (float)HeartStrikeDPS / DPSPoints));
            dictValues.Add("Howling Blast",     string.Format("{0:N2}*{1:P}", HowlingBlastDPS, (float)HowlingBlastDPS / DPSPoints));
            dictValues.Add("Icy Touch",         string.Format("{0:N2}*{1:P}", IcyTouchDPS, (float)IcyTouchDPS / DPSPoints));
            dictValues.Add("Necrosis",          string.Format("{0:N2}*{1:P}", NecrosisDPS, (float)NecrosisDPS / DPSPoints));
            dictValues.Add("Obliterate",        string.Format("{0:N2}*{1:P}", ObliterateDPS, (float)ObliterateDPS / DPSPoints));
            dictValues.Add("Death Strike",      string.Format("{0:N2}*{1:P}", DeathStrikeDPS, (float)DeathStrikeDPS / DPSPoints));
            dictValues.Add("Plague Strike",     string.Format("{0:N2}*{1:P}", PlagueStrikeDPS, (float)PlagueStrikeDPS / DPSPoints));
            dictValues.Add("Scourge Strike",    string.Format("{0:N2}*{1:P}", ScourgeStrikeDPS, (float)ScourgeStrikeDPS / DPSPoints));
            dictValues.Add("Unholy Blight",     string.Format("{0:N2}*{1:P}", UnholyBlightDPS, (float)UnholyBlightDPS / DPSPoints));
            dictValues.Add("Wandering Plague",  string.Format("{0:N2}*{1:P}", WanderingPlagueDPS, (float)WanderingPlagueDPS / DPSPoints));
            dictValues.Add("White",             string.Format("{0:N2}*{1:P}", WhiteDPS, (float)WhiteDPS / DPSPoints));
            dictValues.Add("Ghoul",             string.Format("{0:N2}*{1:P}", GhoulDPS, (float)GhoulDPS / DPSPoints));
            dictValues.Add("Bloodworms",        string.Format("{0:N2}*{1:P}", BloodwormsDPS, (float)BloodwormsDPS / DPSPoints));
            dictValues.Add("Other",             string.Format("{0:N2}*{1:P}", OtherDPS, (float)OtherDPS / DPSPoints));
            dictValues.Add("Total DPS",         DPSPoints.ToString("N2"));


            return dictValues;
        }
        public override float GetOptimizableCalculationValue(string calculation)
        {
            switch (calculation)
            {
                case "Health":
                    return BasicStats.Health;
                case "Nature Resistance":
                    return BasicStats.NatureResistance;
                case "Fire Resistance":
                    return BasicStats.FireResistance;
                case "Frost Resistance":
                    return BasicStats.FrostResistance;
                case "Shadow Resistance":
                    return BasicStats.ShadowResistance;
                case "Arcane Resistance":
                    return BasicStats.ArcaneResistance;
                case "Crit Rating":
                    return BasicStats.CritRating;
                case "Expertise Rating":
                    return BasicStats.ExpertiseRating;
                case "Hit Rating":
                    return BasicStats.HitRating;
                case "Haste Rating":
                    return BasicStats.HasteRating;
                case "Target Miss %":
                    return MissedAttacks * 100f;
                case "Target Dodge %":
                    return DodgedAttacks * 100f;
                case "Resilience": return BasicStats.Resilience;
                case "Spell Penetration": return BasicStats.SpellPenetration;

            }
            return 0;
        }
    }
}
