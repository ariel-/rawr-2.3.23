using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Enhance
{
    /*
    class OldPriorities
    {
        private CombatStats _cs;
        private CalculationOptionsEnhance _calcOpts;
        private Character _character;
        private Stats _stats;
        private ShamanTalents _talents;
        private List<Ability> _abilities;
        private float fightLength;

        public OldPriorities(CombatStats cs, CalculationOptionsEnhance calcOpts, Character character, Stats stats, ShamanTalents talents)
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
            float gcd = Math.Max(1.0f, 1.5f * (1f - StatConversion.GetSpellHasteFromRating(_stats.HasteRating)));
            int priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.ShamanisticRage);
            if (priority > 0)
                if (_talents.ShamanisticRage == 1)
                    abilities.Add(new Ability(EnhanceAbility.ShamanisticRage, 60f, gcd, 0f, priority, false, true));
            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.FeralSpirits);
            if (priority > 0)
                if (_talents.FeralSpirit == 1)
                    abilities.Add(new Ability(EnhanceAbility.FeralSpirits, 180f, gcd, 0.12f * baseMana, priority, false, false));
            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.LightningBolt);
            if (priority > 0)
                if (_talents.MaelstromWeapon > 0)
                    abilities.Add(new Ability(EnhanceAbility.LightningBolt, _cs.SecondsToFiveStack, gcd, 0.1f * baseMana * convection * elementalFocus, priority, false, false));
            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.FlameShock);
            if (priority > 0)
                if (_talents.GlyphofShocking)
                    abilities.Add(new Ability(EnhanceAbility.FlameShock, 18f, 1.0f, FSMana * convection * elementalFocus, priority, false, false));
                else
                    abilities.Add(new Ability(EnhanceAbility.FlameShock, 18f, gcd, FSMana * convection * elementalFocus, priority, false, false));
            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.StormStrike);
            if (priority > 0)
                if (_talents.Stormstrike == 1)
                    abilities.Add(new Ability(EnhanceAbility.StormStrike, 8f, gcd, 0.08f * baseMana, priority, false, true));
            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.EarthShock);
            if (priority > 0)
                if (_talents.GlyphofShocking)
                    abilities.Add(new Ability(EnhanceAbility.EarthShock, _cs.BaseShockSpeed, 1.0f, ESMana * convection * elementalFocus, priority, false, false));
                else
                    abilities.Add(new Ability(EnhanceAbility.EarthShock, _cs.BaseShockSpeed, gcd, ESMana * convection * elementalFocus, priority, false, false));
            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.LavaLash);
            if (priority > 0)
                if (_talents.LavaLash == 1)
                    abilities.Add(new Ability(EnhanceAbility.LavaLash, 6f, gcd, 0.04f * baseMana, priority, false, false));
            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.LightningShield);
            if (priority > 0)
                if (_talents.StaticShock > 0)
                    abilities.Add(new Ability(EnhanceAbility.LightningShield, _cs.StaticShockAvDuration, gcd, 0f, priority, true, false));
            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.MagmaTotem);
            if (priority > 0 && _calcOpts.Magma)
                abilities.Add(new Ability(EnhanceAbility.MagmaTotem, 20f, 1.0f, 0.27f * baseMana * elementalFocus, priority, false, false));
            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.SearingTotem);
            if (priority > 0 && !_calcOpts.Magma)
                abilities.Add(new Ability(EnhanceAbility.SearingTotem, 60f, 1.0f, 0.07f * baseMana * elementalFocus, priority, false, false));
            priority = _calcOpts.GetAbilityPriorityValue(EnhanceAbility.RefreshTotems);
            if (priority > 0)
                abilities.Add(new Ability(EnhanceAbility.RefreshTotems, 300f, 1.0f, 0.24f * baseMana, _calcOpts.GetAbilityPriorityValue(EnhanceAbility.RefreshTotems), true, false)); // patch 3.2 takes just 1 second GCD to refresh totems.
            abilities.Sort();
            return abilities;
        }

        public void CalculateAbilities()
        {
            float fightLengthMultiplier = _calcOpts.FightLengthMultiplier;
            PriorityDataBlock db = new PriorityDataBlock(_calcOpts, _cs);
            PriorityQueue<Ability> queue = new PriorityQueue<Ability>();
            foreach (Ability ability in _abilities)
                queue.Enqueue(ability);
            while (queue.Count > 0)
            {
                Ability ability = queue.Dequeue();
                //                string name = "Skipped " + ability.Name;
                if (ability.MissedCooldown(db.CurrentTime)) // we missed a cooldown so set new cooldown to current time
                    ability.UpdateCooldown(db.CurrentTime);
                else
                {
                    // if we have chosen to wait a fraction of a second for next ability then we need
                    // to ensure that the current time starts when ability is actually off cooldown
                    if (ability.CooldownOver > db.CurrentTime)
                    {
                        db.AddManaRegen(ability.CooldownOver - db.CurrentTime);
                        db.CurrentTime = ability.CooldownOver;
                    }
                    // If this is a shock and previous shock is still on cooldown
                    // then we update the attempted shock's cooldown to when the shock is next available
                    if ((ability.AbilityType == EnhanceAbility.EarthShock || ability.AbilityType == EnhanceAbility.FlameShock) && db.CurrentTime < db.ShockOffCooldown)
                        ability.UpdateCooldown(db.ShockOffCooldown);
                    else
                    {
                        // all is ok so use the ability if mana available
                        if (db.ManaAvailable(ability))
                        {
                            db.UseAbility(ability);
                            //                            name = ability.Name;
                        }
                        else
                        {
                            ability.DeferAbility();
                            //                            name = "Deferred " + ability.Name;
                        }
                    }
                }
                if (ability.CooldownOver < fightLength * fightLengthMultiplier) // adds ability back into queue if its available again before end of fight
                    queue.Enqueue(ability);
                //  DebugPrint(_abilities, db.Timestamp, name, db.CurrentMana);
            }
            // at this stage abilities now contains the number of procs per fight for each ability as a whole number
            // to avoid big stepping problems work out the fraction of the ability use based on how long until next 
            // use beyond fight duration.
            foreach (Ability ability in _abilities)
            {
                float overrun = ability.Duration - (ability.CooldownOver - fightLength * fightLengthMultiplier);
                ability.AddUses(overrun / ability.Duration);
                ability.AverageUses(fightLengthMultiplier);
            }
            // DebugPrint(_abilities, timeElapsed - gcd - averageLag, "Final uses");
        }

        private void DebugPrint(List<Ability> abilities, float timeElapsed, string name, float currentMana)
        {
#if !RAWR3
            if (abilities.Count > 3)
                System.Diagnostics.Debug.Print(
                    "Time: {0} - {1}:{2}, {3} - {4}:{5}, {6} - {7}:{8}, {9} - {10}:{11}, {12} - {13}:{14}, {15} - {16}:{17}, {18} - {19}:{20}, {21} - {22} - {23}",
                   timeElapsed,
                   abilities[0].Name, abilities[0].Uses, abilities[0].CooldownOver,
                   abilities[1].Name, abilities[1].Uses, abilities[1].CooldownOver,
                   abilities[2].Name, abilities[2].Uses, abilities[2].CooldownOver,
                   abilities[3].Name, abilities[3].Uses, abilities[3].CooldownOver,
                   abilities[4].Name, abilities[4].Uses, abilities[4].CooldownOver,
                   abilities[5].Name, abilities[5].Uses, abilities[5].CooldownOver,
                   abilities[6].Name, abilities[6].Uses, abilities[6].CooldownOver, currentMana, name);
#endif
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
        #region Data Block
        private class PriorityDataBlock
        {
            float _gcd = 1.5f;
            float _currentTime = 0f;
            float _averageLag = 0f;
            float _shockOffCooldown = 0f;
            float _shockCooldown = 0f;
            float _currentMana = 0f;
            float _maxMana = 0f;
            float _minManaSR = 0f;
            float _impStormStrikeMana = 0f;
            float _manaRegen = 0f;
            bool _useMana = true;

            public PriorityDataBlock(CalculationOptionsEnhance calcOpts, CombatStats cs)
            {
                _averageLag = calcOpts.AverageLag / 1000f;
                _shockCooldown = cs.BaseShockSpeed;
                _maxMana = cs.MaxMana;
                _currentMana = cs.MaxMana;
                _minManaSR = calcOpts.MinManaSR;
                _impStormStrikeMana = cs.ImpStormStrikeMana;
                _useMana = calcOpts.UseMana;
                _manaRegen = cs.ManaRegen;
            }

            public void UseAbility(Ability ability)
            {
                ability.Use(_currentTime); // consider adding human delay factor to time elapsed as to when next comes off CD
                _gcd = ability.GCD;
                _currentTime += _gcd + _averageLag;
                _currentMana -= ability.ManaCost;
                AddManaRegen(_gcd);
                switch (ability.AbilityType)
                {
                    case EnhanceAbility.EarthShock:
                    case EnhanceAbility.FlameShock:
                        UpdateShockCooldown();
                        break;
                    case EnhanceAbility.ShamanisticRage:
                        _currentMana = _maxMana;
                        break;
                    case EnhanceAbility.StormStrike:
                        _currentMana += _impStormStrikeMana;
                        if (_currentMana > _maxMana)
                            _currentMana = _maxMana;
                        break;
                }
            }

            public void AddManaRegen(float timeElapsed)
            {
                _currentMana += _manaRegen * timeElapsed;
                if (_currentMana > _maxMana)
                    _currentMana = _maxMana;
            }

            public void UpdateShockCooldown()
            {
                _shockOffCooldown = _currentTime + _shockCooldown;
            }

            public bool ManaAvailable(Ability ability)
            {
                if (!_useMana) return true; // disable mana check if option to use mana not set
                if (ability.AbilityType == EnhanceAbility.ShamanisticRage)
                    return _currentMana <= _minManaSR;
                else
                    return ability.ManaCost <= _currentMana;
            }

            public float GCD { get { return _gcd; } set { _gcd = value; } }
            public float CurrentTime { get { return _currentTime; } set { _currentTime = value; } }
            public float CurrentMana { get { return _currentMana; } set { _currentMana = value; } }
            public float ShockCooldown { get { return _shockCooldown; } }
            public float ShockOffCooldown { get { return _shockOffCooldown; } }
            public float Timestamp { get { return _currentTime - _gcd - _averageLag; } }
        }
        #endregion
    }
     */
}