using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Enhance
{
    class Priorities
    {
        private CombatStats _cs;
        private CalculationOptionsEnhance _calcOpts;
        private Character _character;
        private Stats _stats;
        private ShamanTalents _talents;
        private List<Ability> _abilities;
        private float fightLength;

        public Priorities(CombatStats cs, CalculationOptionsEnhance calcOpts, Character character, Stats stats, ShamanTalents talents)
        {
            _cs = cs;
            _calcOpts = calcOpts;
            _character = character;
            _stats = stats;
            _talents = talents;
            fightLength = _calcOpts.FightLength * 60f;
            _abilities = SetupAbilities();
        }

        private List<Ability> SetupAbilities()
        {
            List<Ability> abilities = new List<Ability>();
            float convection = 1f - _talents.Convection * 0.02f;
            float baseMana = BaseStats.GetBaseStats(_character).Mana;
            float elementalFocus = (_talents.ElementalFocus == 1) ? .6f * _cs.ChanceSpellCrit : 1f;
            float ESMana = _talents.ShamanisticFocus == 1 ? baseMana * 0.55f * 0.18f : baseMana * 0.18f; // 45% reduction if Shamanistic Focus
            float FSMana = _talents.ShamanisticFocus == 1 ? baseMana * 0.55f * 0.17f : baseMana * 0.17f; // 45% reduction if Shamanistic Focus
            float fireElementalCD = _talents.GlyphofFireElementalTotem ? 300f : 600f;
            float gcd = Math.Max(1.0f, 1.5f / (1f + _stats.SpellHaste + StatConversion.GetSpellHasteFromRating(_stats.HasteRating)));

            int priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.ShamanisticRage);
            if (priority > 0 && _talents.ShamanisticRage == 1)
                abilities.Add(new Ability(EnhanceAbility.ShamanisticRage, 60f, 1.5f, 0f, priority, false, true));

            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.FeralSpirits);
            if (priority > 0 && _talents.FeralSpirit == 1)
                abilities.Add(new Ability(EnhanceAbility.FeralSpirits, 180f, gcd, 0.12f * baseMana, priority, false, false));

            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.LightningBolt);
            if (priority > 0 && _talents.MaelstromWeapon > 0)
                abilities.Add(new Ability(EnhanceAbility.LightningBolt, _cs.SecondsToFiveStack, gcd, 0.1f * baseMana * convection * elementalFocus, priority, false, false));

            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.FlameShock);
            if (priority > 0)
                if (_talents.GlyphofShocking)
                    abilities.Add(new Ability(EnhanceAbility.FlameShock, _cs.AverageFSDotTime, 1.0f, FSMana * convection * elementalFocus, priority, false, false));
                else
                    abilities.Add(new Ability(EnhanceAbility.FlameShock, _cs.AverageFSDotTime, gcd, FSMana * convection * elementalFocus, priority, false, false));

            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.StormStrike);
            if (priority > 0 && _talents.Stormstrike == 1)
                abilities.Add(new Ability(EnhanceAbility.StormStrike, 8f, 1.5f, 0.08f * baseMana, priority, false, true));

            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.EarthShock);
            if (priority > 0)
                if (_talents.GlyphofShocking)
                    abilities.Add(new Ability(EnhanceAbility.EarthShock, _cs.BaseShockSpeed, 1.0f, ESMana * convection * elementalFocus, priority, false, false));
                else
                    abilities.Add(new Ability(EnhanceAbility.EarthShock, _cs.BaseShockSpeed, gcd, ESMana * convection * elementalFocus, priority, false, false));

            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.LavaLash);
            if (priority > 0 && _talents.LavaLash == 1)
                abilities.Add(new Ability(EnhanceAbility.LavaLash, 6f, 1.5f, 0.04f * baseMana, priority, false, false));

            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.FireNova);
            if (priority > 0)
                abilities.Add(new Ability(EnhanceAbility.FireNova, _cs.BaseFireNovaSpeed, gcd, 0.22f * baseMana, priority, false, false));

            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.FireElemental);
            if (priority > 0 && _calcOpts.FireElemental)
                abilities.Add(new Ability(EnhanceAbility.FireElemental, fireElementalCD, 1.0f, 0.23f * baseMana, priority, false, false));

            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.MagmaTotem);
            if (priority > 0 && _calcOpts.Magma)
                abilities.Add(new Ability(EnhanceAbility.MagmaTotem, 20f, 1.0f, 0.27f * baseMana * elementalFocus, priority, false, false));

            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.SearingTotem);
            if (priority > 0 && !_calcOpts.Magma)
                abilities.Add(new Ability(EnhanceAbility.SearingTotem, 60f, 1.0f, 0.07f * baseMana * elementalFocus, priority, false, false));

            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.LightningShield);
            if (priority > 0 && _talents.StaticShock > 0)
                    abilities.Add(new Ability(EnhanceAbility.LightningShield, _cs.StaticShockAvDuration, gcd, 0f, priority, true, false));

            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.RefreshTotems);
            if (priority > 0)
                abilities.Add(new Ability(EnhanceAbility.RefreshTotems, 300f, 1.0f, 0.24f * baseMana, _calcOpts.GetAbilityPriorityValue(EnhanceAbility.RefreshTotems), true, false)); // patch 3.2 takes just 1 second GCD to refresh totems.

            abilities.Sort();
            return abilities;
        }

        public void CalculateAbilities()
        {
            float averageLag = _calcOpts.AverageLag / 1000f;
            float reactionTime = (_calcOpts.ReactionTime / 1000f) - 0.25f; // the 0.25 seconds before spell comes off gcd allows spells added to queue.
            if (reactionTime < 0f)
                reactionTime = 0f;
            // first calculate base abilities uses
            float totalTimeUsed = 0f;
            float totalManaUsed = 0f;
            float uses = 0f;
            float shockTime = 0f;
            foreach (Ability ability in _abilities)
            {
                if (ability.AbilityType == EnhanceAbility.EarthShock || ability.AbilityType == EnhanceAbility.FlameShock)
                {
                    uses = (fightLength - shockTime) / (ability.Duration + averageLag);
                    shockTime = uses * _cs.BaseShockSpeed;
                }
                else
                    uses = fightLength / (ability.Duration + averageLag);
                ability.AddUses(uses);
                totalTimeUsed += uses * (ability.GCD + reactionTime);
                totalManaUsed += uses * (ability.ManaCost - ability.GCD * _cs.ManaRegen);
                if (ability.AbilityType == EnhanceAbility.ShamanisticRage)
                    totalManaUsed -= uses * _cs.MaxMana;
                if (ability.AbilityType == EnhanceAbility.StormStrike)
                    totalManaUsed -= uses * _cs.ImpStormStrikeMana;
            }
        // fight length & mana calcs causing huge issues - disabling for now
//            if (totalTimeUsed > fightLength)
//                foreach (Ability ability in _abilities)
//                   ability.AverageUses(totalTimeUsed / fightLength); 
//            if (totalManaUsed > _cs.MaxMana && _calcOpts.UseMana)
//                foreach (Ability ability in _abilities)
//                    ability.AverageUses(totalManaUsed / _cs.MaxMana); 
            // at this stage have the upper bounds of number of uses - ie: no GCD interference
            foreach (Ability ability in _abilities)
            {
                ability.RemovePossibleClashingUses(0.075f, fightLength);
            }
        }       

        public float AbilityCooldown(EnhanceAbility abilityType)
        {
            foreach (Ability ability in _abilities)
            {
                if (ability.AbilityType == abilityType)
                    return ability.Uses == 0 ? ability.Duration : fightLength / ability.Uses;
            }
            return fightLength;
        }

        public float FireElementalUses
        {
            get
            {
                foreach (Ability ability in _abilities)
                    if (ability.AbilityType == EnhanceAbility.FireElemental)
                        return ability.Uses;
                return 0f;
            }
        }
    }

#if !SILVERLIGHT
        [Serializable]
#endif
    public class Priority : IComparable<Priority>
    {
        private EnhanceAbility _abilityType;
        private string _priorityName;
        private string _description;
        private bool _inUse;
        private int _priority;
        private string _enhSimName;

        public Priority()
        {
            _abilityType = EnhanceAbility.None;
            _priorityName = "None";
            _description = "Empty Priority";
            _inUse = false;
            _priority = 0;
            _enhSimName = "XX";
        }

        public Priority(string priorityName, EnhanceAbility abilityType, string description, bool onByDefault, int priority, string enhSimName)
        {
            _priorityName = priorityName;
            _abilityType = abilityType;
            _description = description;
            _inUse = onByDefault;
            _priority = priority;
            _enhSimName = enhSimName;
        }

        public string PriorityName { get { return _priorityName; } set { _priorityName = value; } }
        public EnhanceAbility AbilityType { get { return _abilityType; } set { _abilityType = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public int PriorityValue { get { return _priority; } set { _priority = value; } }
        public bool Checked { get { return _inUse; } set { _inUse = value; } }
        public string EnhSimName { get { return _enhSimName; } }

        public override string ToString()
        {
            return _priorityName;
        }

        public int CompareTo(Priority other)
        {
            return this.PriorityValue.CompareTo(other.PriorityValue);
        }
    }
}
