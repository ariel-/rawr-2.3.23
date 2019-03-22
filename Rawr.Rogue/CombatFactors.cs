using System;
using Rawr.Rogue.ClassAbilities;

namespace Rawr.Rogue {
    public class CombatFactors {
        public CombatFactors(Character character, Stats stats) {
            _stats = stats;
			_mainHand = character.MainHand == null ? new Knuckles() : character.MainHand.Item;
			_offHand = character.OffHand == null ? new Knuckles() : character.OffHand.Item;
            _calcOpts = character.CalculationOptions as CalculationOptionsRogue;
            _character = character;
        }

        private readonly Stats _stats;
        private readonly Item _mainHand;
        private readonly Item _offHand;
        private readonly CalculationOptionsRogue _calcOpts;
        private readonly Character _character;

        public Item MH { get { return _mainHand; } }
        public Item OH { get { return _offHand; } }

        #region Crit
        public float CritFromCritRating { get { return StatConversion.GetCritFromRating(_stats.CritRating,CharacterClass.Rogue); } }
        public float SpellCritFromCritRating { get { return StatConversion.GetSpellCritFromRating(_stats.CritRating,CharacterClass.Rogue); } }
        //  public float BonusWhiteCritDmg { get { return 1f + _stats.BonusCritMultiplier; } }
        public float BaseCritMultiplier { get { return 2f * (1f + Talents.PreyOnTheWeak.Bonus) * (1f + _stats.BonusCritMultiplier); } }
        public float BaseSpellCritMultiplier { get { return 1.5f * (1f + Talents.PreyOnTheWeak.Bonus) * (1f + _stats.BonusCritMultiplier); } }
        public float ProbMhCrit { get { return CalcCrit(MH); } }
        public float ProbOhCrit { get { return CalcCrit(OH); } }
        private float CalcCrit(Item weapon) {
            // Base + Rating
            float crit = _stats.PhysicalCrit + CritFromCritRating;
            // Talent and Racial Bonuses
            if (weapon.Type == ItemType.Dagger || weapon.Type == ItemType.FistWeapon) {
                crit += Talents.CloseQuartersCombat.Bonus;
            }
            // Contain it between 0% and 100%
            return (float)Math.Max(0f,Math.Min(1f,crit));
        }
        public float ProbPoisonCrit { get { return _stats.SpellCrit + _stats.SpellCritOnTarget + SpellCritFromCritRating; } }
        #endregion

        #region Expertise and Dodge
        public float BaseExpertise { get { return Talents.WeaponExpertise.Bonus + _stats.Expertise + StatConversion.GetExpertiseFromRating(_stats.ExpertiseRating, CharacterClass.Rogue); } }
        public float MhExpertise { get { return CalcExpertise(MH); } }
        public float OhExpertise { get { return CalcExpertise(OH); } }
        private float GetRacialExpertiseFromWeaponType(ItemType weapon) {
            CharacterRace r = _character.Race;
            if (weapon != ItemType.None) {
                if (r == CharacterRace.Human) {
                    if (weapon == ItemType.OneHandSword || weapon == ItemType.OneHandMace
                        || weapon == ItemType.TwoHandSword || weapon == ItemType.TwoHandMace)
                    {
                        return 3f;
                    }
                } else if (r == CharacterRace.Dwarf) {
                    if (weapon == ItemType.OneHandMace || weapon == ItemType.TwoHandMace) {
                        return 5f;
                    }
                } else if (r == CharacterRace.Orc) {
                    if (weapon == ItemType.OneHandAxe || weapon == ItemType.TwoHandAxe) {
                        return 5f;
                    }
                }
            }
            return 0f;
        }
        private float CalcExpertise(Item weapon) {
            // Base + Rating
            float baseExpertise = BaseExpertise;
            // Talent and Racial Bonuses
            float racial = GetRacialExpertiseFromWeaponType(weapon._type);
            //
            return baseExpertise + racial;
        }
        public float MhDodgeChance { get { return CalcDodgeChance(MhExpertise); } }
        public float OhDodgeChance { get { return CalcDodgeChance(OhExpertise); } }
        private static float CalcDodgeChance(float weaponExpertise) {
            float mhDodgeChance = Math.Max(0f, StatConversion.WHITE_DODGE_CHANCE_CAP[83 /*_calcOpts.TargetLevel*/ - 80]
                                               - StatConversion.GetDodgeParryReducFromExpertise(weaponExpertise));
            return mhDodgeChance;
        }
        #endregion

        #region Hit Rating and Miss
        public float YellowMissChance { get { return Math.Max(0f, StatConversion.YELLOW_MISS_CHANCE_CAP[_calcOpts.TargetLevel - _character.Level] - HitPercent); } }
        public float WhiteMissChance { get { return Math.Max(0f, StatConversion.WHITE_MISS_CHANCE_CAP_DW[_calcOpts.TargetLevel - _character.Level] - HitPercent); } }
        public float PoisonMissChance { get { return Math.Max(0f, StatConversion.GetSpellMiss(_character.Level - _calcOpts.TargetLevel, false) - PoisonHitPercent); } }
        public float HitPercent { get { return _stats.PhysicalHit + StatConversion.GetHitFromRating(_stats.HitRating, CharacterClass.Rogue); } }
        public float PoisonHitPercent { get { return _stats.SpellHit + StatConversion.GetSpellHitFromRating(_stats.HitRating, CharacterClass.Rogue); } }
        public float ProbPoisonHit { get { return 1f + (PoisonHitPercent <= 0.17f ? PoisonHitPercent - 0.17f : 0f); } }
        #endregion

        public float TotalHaste {
            get {
                //  float totalHaste = 1f + _stats.PhysicalHaste; // All haste is calc'd into PhysicalHaste in GetCharacterStats
                float totalHaste = _stats.PhysicalHaste;
                //totalHaste      *= 1f + Talents.Flurry * 0.05f * FlurryUptime;
                return totalHaste;
            }
        }
        public float MHSpeed { get { return (MH == null ? 0f : MH.Speed / TotalHaste); } }
        public float OHSpeed { get { return (OH == null ? 0f : OH.Speed / TotalHaste); } }

        public float MhNormalizedAttackSpeed { get { return MH.Type == ItemType.Dagger ? 1.7f : 2.4f; } }
        public float OhNormalizedAttackSpeed { get { return OH.Type == ItemType.Dagger ? 1.7f : 2.4f; } }
        //  public float DamageReduction { get { return ArmorDamageReduction; } }
        public float MhDamageReduction { get { return (MH.Type != ItemType.OneHandMace) ? ArmorDamageReduction : Math.Min(1f, ArmorDamageReduction + Talents.MaceSpecialization.Bonus); } }
        public float OhDamageReduction { get { return (OH.Type != ItemType.OneHandMace) ? ArmorDamageReduction : Math.Min(1f, ArmorDamageReduction + Talents.MaceSpecialization.Bonus); } }
        public float ArmorDamageReduction {
            get {
                float armorReduction;
                float arpenBuffs = 0f;
                if (_calcOpts == null) {
                    armorReduction = Math.Max(0f, 1f - StatConversion.GetArmorDamageReduction(_character.Level, (int)StatConversion.NPC_ARMOR[83 - _character.Level] - Talents.SerratedBlades.ArmorPenetration.Bonus, _stats.ArmorPenetration, arpenBuffs, _stats.ArmorPenetrationRating)); // default is vs raid boss
                } else {
                    armorReduction = Math.Max(0f, 1f - StatConversion.GetArmorDamageReduction(_character.Level, _calcOpts.TargetArmor - Talents.SerratedBlades.ArmorPenetration.Bonus, _stats.ArmorPenetration, arpenBuffs, _stats.ArmorPenetrationRating));
                }
                //return StatConversion.GetArmorDamageReduction(_character.Level, _calcOpts.TargetArmor - Talents.SerratedBlades.ArmorPenetration.Bonus, 0, 0, _stats.ArmorPenetrationRating);
                return armorReduction;
            }
        }
        public float GlanceReduction { get { return 0.75f; } }
        public float PoisonDamageReduction { get { return StatConversion.GetAverageResistance(_character.Level,_calcOpts.TargetLevel,0f,0f); } }
        public float AvgMhWeaponDmg { get { return CalcAverageWeaponDamage(MH, _stats); } }
        public float MhAvgDamage { get { return AvgMhWeaponDmg + (_stats.AttackPower / 14.0f) * MH.Speed; } }
        public float MhNormalizedDamage { get { return AvgMhWeaponDmg + (_stats.AttackPower / 14.0f) * MhNormalizedAttackSpeed; } }
        public float AvgOhWeaponDmg { get { return CalcAverageWeaponDamage(OH, _stats); } }
        public float OhAvgDamage { get { return (AvgOhWeaponDmg + (_stats.AttackPower / 14.0f) * OH.Speed) * OffHandDamagePenalty; } }
        public float OhNormalizedDamage { get { return (AvgOhWeaponDmg + (_stats.AttackPower/14.0f)*OhNormalizedAttackSpeed)*OffHandDamagePenalty; } }
        public float OffHandDamagePenalty { get { return 0.5f * (1f + Talents.DualWieldSpecialization.Bonus); } }
        public float ProbGlancingHit { get { return StatConversion.WHITE_GLANCE_CHANCE_CAP[_calcOpts.TargetLevel - _character.Level]; } }
        //assume parry & glance are always 0 for a yellow attack
        public float ProbYellowHit { get { return 1f - YellowMissChance - MhDodgeChance; } }
        public float ProbMhWhiteHit { get { return 1f - WhiteMissChance - MhDodgeChance - ProbGlancingHit - ProbMhCrit; } }
        public float ProbOhWhiteHit { get { return 1f - WhiteMissChance - OhDodgeChance - ProbGlancingHit - ProbOhCrit; } }
        public float BaseEnergyRegen {
            get {
                float energyRegen = 10f;
                energyRegen += Talents.Vitality.Bonus;
                energyRegen += Talents.AdrenalineRush.Energy.Bonus;
                energyRegen += _stats.BonusToTTEnergy / 30;                             // Assume ToTT every 30 sec
                energyRegen -= Talents.HungerForBlood.EnergyCost.Bonus / 55f;           // Assume reactive HungerForBlood every 55 sec
                energyRegen -= Talents.BladeFlurry.EnergyCost.Bonus;
                //energyRegen -= _calcOpts.Feint.EnergyCost();
                return energyRegen;
            }
        }
        //Need to Add 30% SnD   formerly:  totalHaste *= (1f + .3f * (1f + _stats.BonusSnDHaste));  //TODO:  change from assuming SnD has a 100% uptime
        //public float Haste { get { return _stats.PhysicalHaste; } set { _stats.PhysicalHaste = value; } }
        public float Tier7TwoPieceRuptureBonusDamage { get { return 1f + _stats.RogueT7TwoPieceBonus * 0.1f; } }
        public float Tier7FourPieceEnergyCostReduction { get{ return ( 1f - _stats.RogueT7FourPieceBonus * 0.05f);} }
        //1 energy per Deadly Poison tick, 1 tick every 3 seconds
        public float Tier8TwoPieceEnergyBonus { get { return _stats.RogueT8TwoPieceBonus == 1 ? 1f / 3f : 0f; } }
        public float Tier8FourPieceRuptureCrit { get { return 1 + (_stats.RogueT8FourPieceBonus == 1 ? ProbMhCrit : 0f); } }
        public float T09x2ReduceEnergyCostFromRupture { get { return _stats.ReduceEnergyCostFromRupture; } }
        public float T09x4BonusCPGCritChance { get { return _stats.BonusCPGCritChance; } }
        public float T10x2BonusToTTEnergy { get { return _stats.BonusToTTEnergy; } }
        public float T10x4ChanceOn3CPOnFinisher { get { return _stats.ChanceOn3CPOnFinisher; } }
        private static float CalcAverageWeaponDamage(Item weapon, Stats stats) { return stats.WeaponDamage + (weapon.MinDamage + weapon.MaxDamage) / 2.0f; }
        public class Knuckles : Item { public Knuckles() { Speed = 2f; } }
    }
}