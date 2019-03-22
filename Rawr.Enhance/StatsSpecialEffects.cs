using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Enhance
{
    class StatsSpecialEffects
    {
        private Character _character;
        private Stats _stats;
        private CombatStats _cs;
        float trigger = 0f;
        float chance = 1f;
        float unhastedAttackSpeed = 3f;
        SpecialEffect mainHandEnchant = null;
        SpecialEffect offHandEnchant = null;
        bool mhProcessed = false;
        bool ohProcessed = false;

        public StatsSpecialEffects(Character character, Stats stats, CalculationOptionsEnhance calcOpts)
        {
            _character = character;
            _stats = stats;
            _cs = new CombatStats(_character, _stats, calcOpts);
            if (character.MainHandEnchant != null)
            { 
                Stats.SpecialEffectEnumerator mhEffects = character.MainHandEnchant.Stats.SpecialEffects();
                if (mhEffects.MoveNext()) { mainHandEnchant = mhEffects.Current; }
            }
            if (_character.ShamanTalents.DualWield == 1 && character.OffHandEnchant != null)
            {
                Stats.SpecialEffectEnumerator ohEffects = character.OffHandEnchant.Stats.SpecialEffects();
                if (ohEffects.MoveNext()) { offHandEnchant = ohEffects.Current; }
            }
        }

        public Stats getSpecialEffects()
        {
            Stats statsAverage = new Stats();
            foreach (SpecialEffect effect in _stats.SpecialEffects())
            {
                statsAverage.Accumulate(getSpecialEffects(effect));
            }
            AddParagon(statsAverage);
            AddHighestStat(statsAverage);
            AddDeathBringerProc(statsAverage);
            return statsAverage;
        }

        public Stats getSpecialEffects(SpecialEffect effect)
        {
            Stats statsAverage = new Stats();
            if (effect == mainHandEnchant || effect == offHandEnchant)
            {
                if (mainHandEnchant != null && !mhProcessed)
                {
                    statsAverage.Accumulate(mainHandEnchant.Stats, GetMHUptime());
                    mhProcessed = true; 
                }
                else if (offHandEnchant != null && !ohProcessed)
                {
                    statsAverage.Accumulate(offHandEnchant.Stats, GetOHUptime());
                    ohProcessed = true; 
                }
            }
            else if (effect.Trigger == Trigger.Use)
            {
                effect.AccumulateAverageStats(statsAverage);
                foreach (SpecialEffect e in effect.Stats.SpecialEffects())
                    statsAverage.Accumulate(this.getSpecialEffects(e) * (effect.Duration / effect.Cooldown));
            }
            else
            {
                SetTriggerChanceAndSpeed(effect);
                foreach (SpecialEffect e in effect.Stats.SpecialEffects())  // deal with secondary effects
                {
                    statsAverage.Accumulate(this.getSpecialEffects(e));
                }
                if (effect.MaxStack > 1)
                {
                    if (effect.Stats.MoteOfAnger > 0)
                    {
                        // When in effect stats, MoteOfAnger is % of melee hits
                        // When in character stats, MoteOfAnger is average procs per second
                        statsAverage.Accumulate(new Stats() { MoteOfAnger = effect.Stats.MoteOfAnger * effect.GetAverageProcsPerSecond(trigger, chance, unhastedAttackSpeed, 0f) / effect.MaxStack });
                    }
                    else
                    {
                        float timeToMax = (float)Math.Min(_cs.FightLength, effect.GetChance(unhastedAttackSpeed) * trigger * effect.MaxStack);
                        float buffDuration = _cs.FightLength;
                        if (effect.Stats.AttackPower == 250f || effect.Stats.AttackPower == 215f || effect.Stats.HasteRating == 57f || effect.Stats.HasteRating == 64f)
                        {
                            buffDuration = 20f;
                        }
                        if (timeToMax * .5f > buffDuration)
                        {
                            timeToMax = 2 * buffDuration;
                        }
                        statsAverage.Accumulate(effect.Stats * (effect.MaxStack * (((buffDuration) - .5f * timeToMax) / (buffDuration))));
                    }
                }
                else
                {
                    effect.AccumulateAverageStats(statsAverage, trigger, chance, unhastedAttackSpeed);
                }
            }
            return statsAverage;
        }

        private void SetTriggerChanceAndSpeed(SpecialEffect effect)
        {
            trigger = 0f;
            chance = 1f;
            unhastedAttackSpeed = 3f;
            switch (effect.Trigger)
            {
                case Trigger.DamageDone:
                    trigger = (_cs.HastedMHSpeed + 1f / _cs.GetSpellAttacksPerSec()) / 2f;
                    chance = (float)Math.Min(1.0f, _cs.AverageWhiteHitChance + _cs.ChanceSpellHit); // limit to 100% chance
                    unhastedAttackSpeed = _cs.UnhastedMHSpeed;
                    break;
                case Trigger.DamageOrHealingDone:
                    // Need to add Self Heals
                    trigger = (_cs.HastedMHSpeed + 1f / _cs.GetSpellAttacksPerSec()) / 2f;
                    chance = (float)Math.Min(1.0f, _cs.AverageWhiteHitChance + _cs.ChanceSpellHit); // limit to 100% chance
                    unhastedAttackSpeed = _cs.UnhastedMHSpeed;
                    break;
                case Trigger.MeleeCrit:
                case Trigger.PhysicalCrit:
                    trigger = _cs.HastedMHSpeed;
                    chance = _cs.AverageWhiteCritChance;
                    unhastedAttackSpeed = _cs.UnhastedMHSpeed;
                    break;
                case Trigger.MeleeHit:
                case Trigger.PhysicalHit:
                    trigger = _cs.HastedMHSpeed;
                    chance = _cs.AverageWhiteHitChance;
                    unhastedAttackSpeed = _cs.UnhastedMHSpeed;
                    break;
                case Trigger.MeleeAttack:
                    if (_character.ShamanTalents.DualWield == 1)
                    {
                        trigger = (_cs.HastedMHSpeed + _cs.HastedOHSpeed) / 2;
                        chance = 1f;
                        unhastedAttackSpeed = (_cs.UnhastedMHSpeed + _cs.UnhastedOHSpeed) / 2;
                    } else {
                        trigger = _cs.HastedMHSpeed;
                        chance = 1f;
                        unhastedAttackSpeed = _cs.UnhastedMHSpeed;
                    }
                    break;                
                case Trigger.DamageSpellCast:
                case Trigger.SpellCast:
                    trigger = 1f / _cs.GetSpellCastsPerSec();
                    chance = 1f;
                    break;
                case Trigger.DamageSpellHit:
                case Trigger.SpellHit:
                    trigger = 1f / _cs.GetSpellAttacksPerSec();
                    chance = _cs.ChanceSpellHit;
                    break;
                case Trigger.DamageSpellCrit:
                case Trigger.SpellCrit:
                    trigger = 1f / _cs.GetSpellCritsPerSec();
                    chance = _cs.ChanceSpellCrit;
                    break;
                case Trigger.SpellMiss:
                    trigger = 1f / _cs.GetSpellMissesPerSec();
                    chance = 1 - _cs.ChanceSpellHit;
                    break;
                case Trigger.ShamanLightningBolt:
                    trigger = 1f / _cs.AbilityCooldown(EnhanceAbility.LightningBolt);
                    chance = _cs.ChanceSpellHit;
                    break;
                case Trigger.ShamanStormStrike:
                    trigger = 1f / _cs.AbilityCooldown(EnhanceAbility.StormStrike);
                    chance = _cs.ChanceYellowHitMH;
                    break;
                case Trigger.ShamanShock:
                    trigger = 1f / _cs.AbilityCooldown(EnhanceAbility.EarthShock);
                    chance = _cs.ChanceSpellHit;
                    break;
                case Trigger.ShamanLavaLash:
                    trigger = 1f / _cs.AbilityCooldown(EnhanceAbility.LavaLash);
                    chance = _cs.ChanceYellowHitOH;
                    unhastedAttackSpeed = _cs.UnhastedOHSpeed;
                    break;
                case Trigger.ShamanShamanisticRage:
                    trigger = 1f / _cs.AbilityCooldown(EnhanceAbility.ShamanisticRage);
                    chance = 1f;
                    break;
                case Trigger.ShamanFlameShockDoTTick:
                case Trigger.DoTTick:
                    trigger = 1f / _cs.AbilityCooldown(EnhanceAbility.FlameShock);
                    chance = 1f;
                    break;
            }
        }

        public float GetUptime(ItemInstance item)
        {
            float uptime = 0;
            if (item != null)
                foreach (SpecialEffect effect in item.GetTotalStats().SpecialEffects())
                {
                    SetTriggerChanceAndSpeed(effect);
                    uptime = effect.GetAverageUptime(trigger, chance, unhastedAttackSpeed);
                }
            return uptime;
        }

        public float GetMHUptime()
        {
            if (mainHandEnchant != null && mainHandEnchant.Trigger == Trigger.SpellHit)
                return mainHandEnchant.GetAverageUptime(1f / _cs.GetSpellAttacksPerSec(), _cs.ChanceSpellHit, _cs.UnhastedMHSpeed, _cs.FightLength);
            return mainHandEnchant == null ? 0f : mainHandEnchant.GetAverageUptime(_cs.HastedMHSpeed, _cs.ChanceWhiteHitMH, _cs.UnhastedMHSpeed, _cs.FightLength);
        }

        public float GetOHUptime()
        {
            if (offHandEnchant != null && offHandEnchant.Trigger == Trigger.SpellHit)
                return offHandEnchant.GetAverageUptime(1f / _cs.GetSpellAttacksPerSec(), _cs.ChanceSpellHit, _cs.UnhastedOHSpeed, _cs.FightLength);
            return offHandEnchant == null ? 0f : offHandEnchant.GetAverageUptime(_cs.HastedOHSpeed, _cs.ChanceWhiteHitOH, _cs.UnhastedOHSpeed, _cs.FightLength);
        }

        // Handling for Paragon trinket procs
        private void AddParagon(Stats statsAverage)
        {
            if (statsAverage.Paragon > 0)
            {
                float paragon = 0f;
                if (_stats.Strength > _stats.Agility)
                {
                    paragon = statsAverage.Paragon * (1 + _stats.BonusStrengthMultiplier);
                    statsAverage.Strength += paragon;
                    statsAverage.AttackPower += paragon;
                }
                else
                {
                    paragon = statsAverage.Paragon * (1 + _stats.BonusAgilityMultiplier);
                    statsAverage.Agility += paragon;
                    statsAverage.AttackPower += paragon;
                }
            }
        }

        private void AddHighestStat(Stats statsAverage)
        {
            //trinket procs
            if (statsAverage.HighestStat > 0)
            {
                float intfromAP = _character.ShamanTalents.MentalDexterity / 3;
                // Highest stat
                float highestStat = 0f;
                if (_stats.Agility > _stats.Strength)
                {
                    if (_stats.Agility > _stats.Intellect)
                    {
                        highestStat = statsAverage.HighestStat * (1 + _stats.BonusAgilityMultiplier);
                        statsAverage.Agility += highestStat;
                        statsAverage.AttackPower += highestStat;
                    }
                    else
                    {
                        highestStat = statsAverage.HighestStat * (1 + _stats.BonusIntellectMultiplier);
                        statsAverage.Intellect += highestStat;
                        statsAverage.AttackPower += intfromAP * highestStat;
                    }
                }
                else
                {
                    if (_stats.Strength > _stats.Intellect)
                    {
                        highestStat = statsAverage.HighestStat * (1 + _stats.BonusStrengthMultiplier);
                        statsAverage.Strength += highestStat;
                        statsAverage.AttackPower += highestStat;
                    }
                    else
                    {
                        highestStat = statsAverage.HighestStat * (1 + _stats.BonusIntellectMultiplier);
                        statsAverage.Intellect += highestStat;
                        statsAverage.AttackPower += intfromAP * highestStat;
                    }
                }
            }
        }

        private void AddDeathBringerProc(Stats statsAverage)
        {
            if (statsAverage.DeathbringerProc > 0)
            {
                statsAverage.Agility += (statsAverage.DeathbringerProc * (1 + _stats.BonusAgilityMultiplier)) / 3f;
                statsAverage.AttackPower += 2f * statsAverage.DeathbringerProc / 3f; // AP is double budget
                statsAverage.HasteRating += statsAverage.DeathbringerProc / 3f;
            }
        }
    }
}
