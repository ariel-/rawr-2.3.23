using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.TankDK
{
    class StatsSpecialEffects
    {
        public Character character;
        public Stats stats;
        public CombatTable combatTable;
        public StatsSpecialEffects(Character c, Stats s, CombatTable t)
        {
            // It doesn't actually use the character or stats object being passed in.
            character = c;
            stats = s;
            combatTable = t;
        }

        public Stats getSpecialEffects(CalculationOptionsTankDK calcOpts, SpecialEffect effect)
        {
            Stats statsAverage = new Stats();
            Rotation rRotation = calcOpts.m_Rotation;
            if (effect.Trigger == Trigger.Use)
            {
                if (calcOpts.bUseOnUseAbilities == true)
                    statsAverage.Accumulate(effect.GetAverageStats());
            }
            else
            {
                float trigger = 0f;
                float chance = effect.Chance;
                float duration = effect.Duration;
                float unhastedAttackSpeed = 2f;
                switch (effect.Trigger)
                {
                    case Trigger.MeleeCrit:
                    case Trigger.PhysicalCrit:
                        trigger = (1f / rRotation.getMeleeSpecialsPerSecond()) + (combatTable.combinedSwingTime != 0 ? 1f / combatTable.combinedSwingTime : 0.5f);
                        chance = combatTable.physCrits * effect.Chance;
                        unhastedAttackSpeed = (combatTable.MH != null ? combatTable.MH.baseSpeed : 2.0f);
                        break;
                    case Trigger.MeleeHit:
                    case Trigger.PhysicalHit:
                        trigger = (1f / (rRotation.getMeleeSpecialsPerSecond() * (combatTable.m_bDW ? 2 : 1))) + (combatTable.combinedSwingTime != 0 ? 1f / combatTable.combinedSwingTime : 0.5f);
                        chance = effect.Chance * (1f - (combatTable.missedSpecial + combatTable.dodgedSpecial));
                        unhastedAttackSpeed = (combatTable.MH != null ? combatTable.MH.baseSpeed : 2.0f);
                        break;
                    case Trigger.CurrentHandHit:
                    case Trigger.MainHandHit:
                        trigger = (1f / rRotation.getMeleeSpecialsPerSecond()) + (combatTable.MH.hastedSpeed != 0 ? 1f / combatTable.MH.hastedSpeed : 0.5f);
                        chance = effect.Chance * (1f - (combatTable.missedSpecial + combatTable.dodgedSpecial));
                        unhastedAttackSpeed = (combatTable.MH != null ? combatTable.MH.baseSpeed : 2.0f);
                        break;
                    case Trigger.OffHandHit:
                        trigger = (1f / rRotation.getMeleeSpecialsPerSecond()) + (combatTable.OH.hastedSpeed != 0 ? 1f / combatTable.OH.hastedSpeed : 0.5f);
                        chance = effect.Chance * (1f - (combatTable.missedSpecial + combatTable.dodgedSpecial));
                        unhastedAttackSpeed = (combatTable.MH != null ? combatTable.MH.baseSpeed : 2.0f);
                        break;
                    case Trigger.DamageDone:
                    case Trigger.DamageOrHealingDone:
                        trigger = (1f / rRotation.getMeleeSpecialsPerSecond()) + (combatTable.combinedSwingTime != 0 ? 1f / combatTable.combinedSwingTime : 0.5f);
                        unhastedAttackSpeed = (combatTable.MH != null ? combatTable.MH.baseSpeed : 2.0f);
                        chance = effect.Chance * (1f - (combatTable.missedSpecial + combatTable.dodgedSpecial));
                        break;
                    case Trigger.DamageSpellCast:
                    case Trigger.SpellCast:
                    case Trigger.DamageSpellHit:
                    case Trigger.SpellHit:
                        trigger = 1f / rRotation.getSpellSpecialsPerSecond();
                        chance = 1f - combatTable.spellResist;
                        break;
                    case Trigger.DamageSpellCrit:
                    case Trigger.SpellCrit:
                        trigger = 1f / rRotation.getSpellSpecialsPerSecond();
                        chance = combatTable.spellCrits * effect.Chance;
                        break;
                    case Trigger.DoTTick:
                        trigger = (rRotation.BloodPlague + rRotation.FrostFever) / 3;
                        break;
                    case Trigger.DamageTaken:
                    case Trigger.DamageTakenPhysical:
                        trigger = calcOpts.BossAttackSpeed;
                        chance *= 1f - (stats.Dodge + stats.Parry + stats.Miss);
                        unhastedAttackSpeed = calcOpts.BossAttackSpeed;
                        break;
                    case Trigger.DamageTakenMagical:
                        trigger = calcOpts.IncomingFromMagicFrequency;
                        break;
                    //////////////////////////////////
                    // DK specific triggers:
                    case Trigger.BloodStrikeHit:
                    case Trigger.HeartStrikeHit:
                        trigger = rRotation.curRotationDuration / (rRotation.BloodStrike + rRotation.HeartStrike);
                        break;
                    case Trigger.PlagueStrikeHit:
                        trigger = rRotation.curRotationDuration / rRotation.PlagueStrike;
                        break;
                    case Trigger.RuneStrikeHit:
                        trigger = rRotation.curRotationDuration / rRotation.RuneStrike;
                        break;
                    case Trigger.IcyTouchHit:
                        trigger = rRotation.curRotationDuration / rRotation.IcyTouch;
                        break;
                    case Trigger.DeathStrikeHit:
                        trigger = rRotation.curRotationDuration / rRotation.DeathStrike;
                        break;
                    case Trigger.ObliterateHit:
                        trigger = rRotation.curRotationDuration / rRotation.Obliterate;
                        break;
                    case Trigger.ScourgeStrikeHit:
                        trigger = rRotation.curRotationDuration / rRotation.ScourgeStrike;
                        break;
                    case Trigger.FrostFeverHit:
                        // Icy Talons triggers off this.
                        trigger = rRotation.curRotationDuration / rRotation.IcyTouch;
                        if (character.DeathKnightTalents.GlyphofHowlingBlast)
                            trigger += rRotation.curRotationDuration / rRotation.HowlingBlast;
                        break;
                }
                if (!float.IsInfinity(trigger) && !float.IsNaN(trigger))
                {
                    if (effect.UsesPPM())
                    {
                        // If effect.chance < 0 , then it's using PPM.
                        // Let's get the duration * how many times it procs per min:
                        float UptimePerMin = 0;
                        float fWeight = 0; 
                        if (duration == 0) // Duration of 0 means that it's a 1 time effect that procs every time the proc comes up.
                        {
                            fWeight = Math.Abs(effect.Chance) / 60 ;
                        }
                        else
                        {
                            UptimePerMin = duration * Math.Abs(effect.Chance);
                            fWeight = UptimePerMin / 60;
                        }
                        statsAverage.Accumulate(effect.Stats, fWeight);
                    }
                    else
                    {
                        effect.AccumulateAverageStats(statsAverage, trigger, chance, unhastedAttackSpeed, calcOpts.FightLength * 60);
                    }
                }
            }
            return statsAverage;
        }
    }
}
