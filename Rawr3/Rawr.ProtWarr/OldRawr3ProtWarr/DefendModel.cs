using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.ProtWarr
{
    class DefendModel
    {
        private Character Character;
        private CalculationOptionsProtWarr Options;
        private Stats Stats;

        public readonly ParryModel ParryModel;
        public readonly DefendTable DefendTable;

        public float AverageDamagePerAttack { get; set; }
        public float AverageDamagePerHit { get; set; }
        public float DamagePerHit { get; set; }
        public float DamagePerBlock { get; set; }
        public float DamagePerCrit { get; set; }
        public float DamagePerSecond { get; set; }
        public float GuaranteedReduction { get; set; }
        public float Mitigation { get; set; }
        public float TankPoints { get; set; }
        public float EffectiveHealth { get; set; }
        public float BurstTime { get; set; }

        public void Calculate()
        {
            float attackSpeed           = ParryModel.BossAttackSpeed; // Options.BossAttackSpeed;
            float armorReduction        = (1.0f - Lookup.ArmorReduction(Character, Stats));
            float baseDamagePerSecond   = Options.BossAttackValue / Options.BossAttackSpeed;
            float guaranteedReduction   = (Lookup.StanceDamageReduction(Character, Stats) * armorReduction);

            DamagePerHit    = Options.BossAttackValue * guaranteedReduction;
            DamagePerCrit   = 2.0f * DamagePerHit;
            DamagePerBlock  = 
                (Math.Max(0.0f, DamagePerHit - Stats.BlockValue) * (1.0f - Character.WarriorTalents.CriticalBlock * 0.1f)) +
                (Math.Max(0.0f, DamagePerHit - (Stats.BlockValue * 2.0f)) * (Character.WarriorTalents.CriticalBlock * 0.1f));

            AverageDamagePerHit =
                DamagePerHit * (DefendTable.Hit / DefendTable.AnyHit) +
                DamagePerCrit * (DefendTable.Critical / DefendTable.AnyHit) +
                DamagePerBlock * (DefendTable.Block / DefendTable.AnyHit);
            AverageDamagePerAttack = 
                DamagePerHit * DefendTable.Hit + 
                DamagePerCrit * DefendTable.Critical +
                DamagePerBlock * DefendTable.Block;

            DamagePerSecond     = AverageDamagePerAttack / attackSpeed;
            Mitigation          = (1.0f - (DamagePerSecond / baseDamagePerSecond));
            TankPoints          = (Stats.Health / (1.0f - Mitigation));
            EffectiveHealth     = (Stats.Health / guaranteedReduction);
            GuaranteedReduction = (1.0f - guaranteedReduction);

            double a = Convert.ToDouble(DefendTable.AnyMiss);
            double h = Convert.ToDouble(Stats.Health);
            double H = Convert.ToDouble(AverageDamagePerHit);
            double s = Convert.ToDouble(ParryModel.BossAttackSpeed / Options.BossAttackSpeed);
            BurstTime = Convert.ToSingle((1.0d / a) * ((1.0d / Math.Pow(1.0d - a, h / H)) - 1.0d) * s);
        }

        public DefendModel(Character character, Stats stats)
        {
            Character   = character;
            Stats       = stats;
            Options     = Character.CalculationOptions as CalculationOptionsProtWarr;
            ParryModel  = new ParryModel(character, stats);
            DefendTable = new DefendTable(character, stats); 
            Calculate();
        }
    }
}
