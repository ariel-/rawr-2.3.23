using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.ProtWarr
{
    public abstract class CombatTable
    {
        protected Character Character;
        protected CalculationOptionsProtWarr Options;
        protected Stats Stats;
        protected Ability Ability;

        public float Miss { get; protected set; }
        public float Dodge { get; protected set; }
        public float Parry { get; protected set; }
        public float Block { get; protected set; }
        public float Glance { get; protected set; }
        public float Critical { get; protected set; }
        public float Hit { get; protected set; }

        public float AnyHit
        {
            get { return (1.0f - (Miss + Dodge + Parry)); }
        }

        public float AnyMiss
        {
            get { return (Miss + Dodge + Parry); }
        }

        public float DodgeParryBlock
        {
            get { return (Dodge + Parry + Block); }
        }

        protected virtual void Calculate()
        {
        }

        protected void Initialize(Character character, Stats stats, CalculationOptionsProtWarr options, Ability ability)
        {
            Character   = character;
            Options     = options;
            Stats       = stats;
            Ability     = ability;
            Calculate();
        }
    }

    public class DefendTable : CombatTable
    {
        protected override void Calculate()
        {
            float tableSize = 0.0f;

            // Miss
            Miss = Math.Min(1.0f - tableSize, Lookup.AvoidanceChance(Character, Stats, HitResult.Miss, Options.TargetLevel));
            tableSize += Miss;
            // Dodge
            Dodge = Math.Min(1.0f - tableSize, Lookup.AvoidanceChance(Character, Stats, HitResult.Dodge, Options.TargetLevel));
            tableSize += Dodge;
            // Parry
            Parry = Math.Min(1.0f - tableSize, Lookup.AvoidanceChance(Character, Stats, HitResult.Parry, Options.TargetLevel));
            tableSize += Parry;
            // Block
            if (Character.OffHand != null && Character.OffHand.Type == ItemType.Shield)
            {
                Block = Math.Min(1.0f - tableSize, Lookup.AvoidanceChance(Character, Stats, HitResult.Block, Options.TargetLevel));
                tableSize += Block;
            }
            // Critical Hit
            Critical = Math.Min(1.0f - tableSize, Lookup.TargetCritChance(Character, Stats, Options.TargetLevel));
            tableSize += Critical;
            // Normal Hit
            Hit = Math.Max(0.0f, 1.0f - tableSize);
        }

        public DefendTable(Character character, Stats stats, CalculationOptionsProtWarr options)
        {
            Initialize(character, stats, options, Ability.None);
        }
    }

    public class AttackTable : CombatTable
    {
        protected override void Calculate()
        {
            float tableSize = 0.0f;
            float bonusHit = Lookup.BonusHitPercentage(Character, Stats);
            float bonusExpertise = Lookup.BonusExpertisePercentage(Character, Stats);

            // Miss
            Miss = Math.Min(1.0f - tableSize, Math.Max(0.0f, Lookup.TargetAvoidanceChance(Character, Stats, HitResult.Miss, Options.TargetLevel) - bonusHit));
            tableSize += Miss;
            // Avoidance
            if (Lookup.IsAvoidable(Ability))
            {
            // Dodge
                Dodge = Math.Min(1.0f - tableSize, Math.Max(0.0f, Lookup.TargetAvoidanceChance(Character, Stats, HitResult.Dodge, Options.TargetLevel) - bonusExpertise));
                tableSize += Dodge;
            // Parry
                Parry = Math.Min(1.0f - tableSize, Math.Max(0.0f, Lookup.TargetAvoidanceChance(Character, Stats, HitResult.Parry, Options.TargetLevel) - bonusExpertise));
                tableSize += Parry;
            }
            if (Ability == Ability.None)
            {
                // White Glancing Blow
                Glance = Math.Min(1.0f - tableSize, Math.Max(0.0f, Lookup.TargetAvoidanceChance(Character, Stats, HitResult.Glance, Options.TargetLevel)));
                tableSize += Glance;

                // White Critical Hits
                Critical = Math.Max(0.0f, Math.Min(1.0f - tableSize, Lookup.BonusCritPercentage(Character, Stats, Ability, Options.TargetLevel))
                            - Lookup.TargetAvoidanceChance(Character, Stats, HitResult.Crit, Options.TargetLevel));
                tableSize += Critical;               
            }
            else
            {
                // Yellow Critical Hits
                Critical = Math.Min(1.0f - tableSize, Math.Max(0.0f, Lookup.BonusCritPercentage(Character, Stats, Ability, Options.TargetLevel) 
                            - Lookup.TargetAvoidanceChance(Character, Stats, HitResult.Crit, Options.TargetLevel)));
                tableSize += Critical;
            }
            // Normal Hit
            Hit = Math.Max(0.0f, 1.0f - tableSize);
        }

        public AttackTable(Character character, Stats stats, CalculationOptionsProtWarr options)
        {
            Initialize(character, stats, options, Ability.None);
        }

        public AttackTable(Character character, Stats stats, CalculationOptionsProtWarr options, Ability ability)
        {
            Initialize(character, stats, options, ability);
        }
    }
}
