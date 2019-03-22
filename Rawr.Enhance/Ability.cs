using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Enhance
{
    class Ability : IComparable<Ability>
    {
        private EnhanceAbility _abilityType;
        private string _name;
        private float _duration;
        private int _priority;
        private float _cooldownOver;
        private float _uses;
        private float _manacost;
        private float _gcd = 1.5f;
        private float timedrift = 0.1f;
        private bool _manaRegen = false;
        
        public Ability(EnhanceAbility abilityType, float duration, float gcd, float manacost, int priority, bool useBeforeCombat, bool manaRegen)
        {
            _abilityType = abilityType;
            _name = abilityType.ToString();
            _duration = duration;
            _priority = priority;
            _manacost = manacost; 
            _gcd = gcd;
            _manaRegen = manaRegen;
            //TODO initial cooldown on SR is when you are almost out of mana for now use its duration ie: 60 seconds
            if (useBeforeCombat)  // if ability is to be used before start of combat refresh is after first duration over (eg: totems)
                _cooldownOver = duration;
            else
                _cooldownOver = 0f;
            _uses = 0;
        }

        public EnhanceAbility AbilityType { get { return _abilityType; } }
        public string Name { get { return _name; } }
        public float Duration { get { return _duration; } }
        public float GCD { get { return _gcd; } }
        public float CooldownOver { get { return _cooldownOver; } }
        public float Uses { get { return _uses; } }
        public float ManaCost { get { return _manacost; } }
        public bool ManaRegen { get { return _manaRegen; } }
        public int Priority { get { return _priority; } }

        public void Use(float useTime)
        {
            _uses++;
            _cooldownOver = useTime + _duration;
        }

        public void AddUses(float uses)
        {
            _uses += uses > 0f ? uses : 0f;
        }

        public void RemovePossibleClashingUses(float clashChance, float fightLength)
        {   
            if (_abilityType != EnhanceAbility.FireElemental)
            {
                float possibleClashes = clashChance * (float)Math.Sqrt(_priority - 1) * fightLength / _duration;
                _uses -= possibleClashes;
            }
            if (_uses < 0) _uses = 0;
        }

        public void AverageUses(float iterations)
        {
            _uses /= iterations;
        }

        public void DeferAbility()
        {
            _cooldownOver += _gcd;
        }

        public void UpdateCooldown(float time)
        {
            _cooldownOver = time;
        }

        public bool MissedCooldown(float starttime)
        {
            return _cooldownOver < starttime;
        }

        public bool OffCooldown(float starttime)
        {
            return starttime >= _cooldownOver;
        }

        public int CompareTo(Ability other)
        {
            float diff = _cooldownOver - other._cooldownOver;
            if (diff < -timedrift)  // current ability is off cooldown earlier than other ability
                return -1;
            else if (diff > timedrift) // current ability is off cooldown later than other ability
                return 1;
            else  // off cooldown at same time so use highest priority ability
               return _priority.CompareTo(other._priority);
        }
    }

    public enum EnhanceAbility
    {
        None = 0,
        FeralSpirits = 1,
        LightningBolt = 2,
        StormStrike = 3,
        EarthShock = 4,
        FlameShock = 5,
        LavaLash = 6,
        LightningShield = 7,
        MagmaTotem = 8,
        SearingTotem = 9,
        RefreshTotems = 10,
        ShamanisticRage = 11,
        FireNova = 12,
        FireElemental = 13,
    }
}
