using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.ProtPaladin
{
    class DefendModel
    {
        private Character Character;
        private CalculationOptionsProtPaladin CalcOpts;
#if (RAWR3)
        private BossOptions BossOpts;
#endif
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
        public float DamageTaken { get; set; }
        public float[] ResistanceTable { get; set; }
        public float TankPoints { get; set; }
        public float EffectiveHealth { get; set; }
        public float BurstTime { get; set; }

        public void Calculate()
        {
            float attackSpeed           = ParryModel.BossAttackSpeed; // Options.BossAttackSpeed;
#if (RAWR3)
            float armorReduction        = (1.0f - Lookup.ArmorReduction(Character, Stats, BossOpts.Level));
#else
            float armorReduction        = (1.0f - Lookup.ArmorReduction(Character, Stats, CalcOpts.TargetLevel));
#endif
            float baseDamagePerSecond   = CalcOpts.BossAttackValue / CalcOpts.BossAttackSpeed;
            float guaranteedReduction   = (Lookup.StanceDamageReduction(Character, Stats) * armorReduction);
            float absorbed = Stats.DamageAbsorbed;

            DamagePerHit    = (CalcOpts.BossAttackValue * guaranteedReduction) - absorbed;
            DamagePerCrit   = (2.0f * DamagePerHit);
            DamagePerBlock  = Math.Max(0.0f, DamagePerHit - Lookup.ActiveBlockReduction(Character, Stats));

            AverageDamagePerHit =
                DamagePerHit * (DefendTable.Hit / DefendTable.AnyHit) +
                DamagePerCrit * (DefendTable.Critical / DefendTable.AnyHit) +
                DamagePerBlock * (DefendTable.Block / DefendTable.AnyHit);
            AverageDamagePerAttack = 
                DamagePerHit * DefendTable.Hit + 
                DamagePerCrit * DefendTable.Critical +
                DamagePerBlock * DefendTable.Block;

            float reductionAD = 1.0f - Lookup.ArdentDefenderReduction(Character);
            float healthAD = (0.65f + (0.35f / reductionAD)) * Stats.Health;

            DamagePerSecond     = AverageDamagePerAttack / attackSpeed;
            DamageTaken         = DamagePerSecond / baseDamagePerSecond;
            Mitigation          = (1.0f - (DamagePerSecond / baseDamagePerSecond));
            TankPoints          = (healthAD / (1.0f - Mitigation));
            EffectiveHealth     = (healthAD / guaranteedReduction);
            GuaranteedReduction = (1.0f - guaranteedReduction);

            
            double a = Convert.ToDouble(DefendTable.AnyMiss);
            double h = Convert.ToDouble(healthAD);
            double H = Convert.ToDouble(AverageDamagePerHit);
            double s = Convert.ToDouble(ParryModel.BossAttackSpeed / CalcOpts.BossAttackSpeed);
            BurstTime = Convert.ToSingle((1.0d / a) * ((1.0d / Math.Pow(1.0d - a, h / H)) - 1.0d) * s);
            /*
            // Attempt to make a different TTL:
            float damageTaken = Options.BossAttackValue; // for TTL(EH)
            float health = EffectiveHealth;
            float anyHit = 1.0f; // worst case, you get hit every swing
            float attacksToKill = (float)Math.Ceiling(health / DamageTaken); // So 10 health / 4 damage = 2.5 attacks = 3 attacks.
            float timeToDie = attacksToKill * attackSpeed; // time in seconds
            float chanceToDie = Convert.ToSingle((float)Math.Pow(anyHit, attacksToKill)); // (= 1^attacksToKill)
            float timeToLive = timeToDie * chanceToDie;
            BurstTime = timeToLive;
            */
        }

#if (RAWR3)
        public DefendModel(Character character, Stats stats, CalculationOptionsProtPaladin calcOpts, BossOptions bossOpts, bool useHolyShield)
#else
        public DefendModel(Character character, Stats stats, CalculationOptionsProtPaladin calcOpts, bool useHolyShield)
#endif
        {
            Character   = character;
            Stats       = stats;
            CalcOpts    = calcOpts;
#if (RAWR3)
            BossOpts    = bossOpts;
#endif

#if (RAWR3)
            ParryModel = new ParryModel(character, stats, calcOpts, bossOpts);
            DefendTable = new DefendTable(character, stats, calcOpts, bossOpts, useHolyShield);
#else
            ParryModel = new ParryModel(character, stats, calcOpts);
            DefendTable = new DefendTable(character, stats, calcOpts, useHolyShield); 
#endif
            Calculate();
        }
    }
}
