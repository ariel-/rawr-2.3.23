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

        protected virtual void Calculate()
        {
        }

        protected void Initialize(Character character, Stats stats, Ability ability)
        {
            Character   = character;
            Options     = character.CalculationOptions as CalculationOptionsProtWarr;
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
            Miss = Math.Min(1.0f - tableSize, Lookup.AvoidanceChance(Character, Stats, HitResult.Miss));
            tableSize += Miss;
            // Dodge
            Dodge = Math.Min(1.0f - tableSize, Lookup.AvoidanceChance(Character, Stats, HitResult.Dodge));
            tableSize += Dodge;
            // Parry
            Parry = Math.Min(1.0f - tableSize, Lookup.AvoidanceChance(Character, Stats, HitResult.Parry));
            tableSize += Parry;
            // Block
            if (Character.OffHand != null && Character.OffHand.Type == ItemType.Shield)
            {
                Block = Math.Min(1.0f - tableSize, Lookup.AvoidanceChance(Character, Stats, HitResult.Block));
                tableSize += Block;
            }
            // Critical Hit
            Critical = Math.Min(1.0f - tableSize, Lookup.TargetCritChance(Character, Stats));
            tableSize += Critical;
            // Normal Hit
            Hit = Math.Max(0.0f, 1.0f - tableSize);
        }

        public DefendTable(Character character, Stats stats)
        {
            Initialize(character, stats, Ability.None);
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
            Miss = Math.Min(1.0f - tableSize, Math.Max(0.0f, Lookup.TargetAvoidanceChance(Character, Stats, HitResult.Miss) - bonusHit));
            tableSize += Miss;
            // Avoidance
            if (Lookup.IsAvoidable(Ability))
            {
            // Dodge
                Dodge = Math.Min(1.0f - tableSize, Math.Max(0.0f, Lookup.TargetAvoidanceChance(Character, Stats, HitResult.Dodge) - bonusExpertise));
                tableSize += Dodge;
            // Parry
                Parry = Math.Min(1.0f - tableSize, Math.Max(0.0f, Lookup.TargetAvoidanceChance(Character, Stats, HitResult.Parry) - bonusExpertise));
                tableSize += Parry;
            }
            // Glancing Blow
            if (Ability == Ability.None)
            {
                Glance = Math.Min(1.0f - tableSize, Math.Max(0.0f, Lookup.TargetAvoidanceChance(Character, Stats, HitResult.Glance)));
                tableSize += Glance;
            }
            // Critical Hit
            Critical = Math.Min(1.0f - tableSize, Lookup.BonusCritPercentage(Character, Stats, Ability));
            tableSize += Critical;
            // Normal Hit
            Hit = Math.Max(0.0f, 1.0f - tableSize);
        }

        public AttackTable(Character character, Stats stats)
        {
            Initialize(character, stats, Ability.None);
        }

        public AttackTable(Character character, Stats stats, Ability ability)
        {
            Initialize(character, stats, ability);
        }
    }
}
