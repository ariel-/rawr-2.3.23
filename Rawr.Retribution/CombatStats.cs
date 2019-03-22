using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Retribution
{
    public class CombatStats
    {

        public CombatStats(Character character, Stats stats)
        {
            _stats = stats;
            _character = character;
            _calcOpts = _character.CalculationOptions as CalculationOptionsRetribution;
            _talents = _character.PaladinTalents;
            UpdateCalcs();
        }

        protected Character _character = new Character();
        public Character Character { get { return _character; } }

        protected Stats _stats = new Stats();
        public Stats Stats { get { return _stats; } }

        protected CalculationOptionsRetribution _calcOpts = new CalculationOptionsRetribution();
        public CalculationOptionsRetribution CalcOpts { get { return _calcOpts; } }

        protected PaladinTalents _talents = new PaladinTalents();
        public PaladinTalents Talents { get { return _talents; } }

        public float WeaponDamage;
        public float BaseWeaponSpeed;
        public float AttackSpeed;
        public float NormalWeaponDamage;

        public float AvengingWrathMulti = 1f;
        public float ArmorReduction = 1f;
        public readonly float PartialResist = 0.94f;

        public float GetMeleeMissChance()    // Chance to miss a white/yellow
        {
            return (float)Math.Max(StatConversion.WHITE_MISS_CHANCE_CAP[_calcOpts.TargetLevel - 80] - _stats.PhysicalHit, 0f);
        }
        public float GetRangedMissChance()    // Chance to miss a ranged attack (HoW)
        {
            // Should be 'RangedHit' instead of PhysicalHit, pala's won't be gearing for specific ranged hit, and there's no RangedHit stat in Stats (only RangedHitRating).
            return (float)Math.Max(StatConversion.WHITE_MISS_CHANCE_CAP[_calcOpts.TargetLevel - 80] - _stats.PhysicalHit, 0f);
        }
        public float GetToBeParriedChance()    
        {
            return (float)Math.Max(StatConversion.WHITE_PARRY_CHANCE_CAP[_calcOpts.TargetLevel - 80] - StatConversion.GetDodgeParryReducFromExpertise(_stats.Expertise, CharacterClass.Paladin), 0f);
        }
        public float GetToBeDodgedChance()
        {
            return (float)Math.Max(StatConversion.WHITE_DODGE_CHANCE_CAP[_calcOpts.TargetLevel - 80] - StatConversion.GetDodgeParryReducFromExpertise(_stats.Expertise, CharacterClass.Paladin), 0f);
        }
        public float GetSpellMissChance()
        {
            return StatConversion.GetSpellMiss(_calcOpts.TargetLevel - 80, false);
        }

        public void UpdateCalcs()
        {
            float fightLength = _calcOpts.FightLength * 60f;

            float bloodlustUptime = ((float)Math.Floor(fightLength / 600f) * 40f + (float)Math.Min(fightLength % 600f, 40f)) / fightLength;
            float bloodlustHaste = 1f + (CalcOpts.Bloodlust ? (bloodlustUptime * .3f) : 0f);

            float awUptime = (float)Math.Ceiling((fightLength - 20f) / (180f - _talents.SanctifiedWrath * 30f)) * 20f / fightLength;
            AvengingWrathMulti = 1f + awUptime * .2f;

            float targetArmor = StatConversion.NPC_ARMOR[CalcOpts.TargetLevel - 80];

            float dr = StatConversion.GetArmorDamageReduction(Character.Level, targetArmor, Stats.ArmorPenetration, 0f, Stats.ArmorPenetrationRating);
            float drAW = dr * ((1 - awUptime) + (1 - .25f * _talents.SanctifiedWrath) * awUptime);
            float drNoAW = dr;
            ArmorReduction = 1f - drAW;

            BaseWeaponSpeed = (_character.MainHand == null || _character.MainHand.Speed == 0.0f) ? 3.5f : _character.MainHand.Speed; // NOTE by Kavan: added a check against speed == 0, it can happen when item data is still being downloaded
            float baseWeaponDamage = _character.MainHand == null ? 371.5f : (_character.MainHand.MinDamage + _character.MainHand.MaxDamage) / 2f;
            AttackSpeed = BaseWeaponSpeed / ((1f + _stats.PhysicalHaste) * bloodlustHaste);
            WeaponDamage = baseWeaponDamage + _stats.AttackPower * BaseWeaponSpeed / 14f;
            NormalWeaponDamage = baseWeaponDamage + _stats.AttackPower * 3.3f / 14f;
        }
    }
}
