using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.ProtWarr
{
    public class ParryModel
    {
        private Character Character;
        private CalculationOptionsProtWarr Options;
        private Stats Stats;

        private AttackTable AttackTable;
        private DefendTable DefendTable;

        public float BossAttackSpeed { get; private set; }
        public float WeaponSpeed { get; private set; }

        private void Calculate()
        {
            float globalCooldownSpeed   = Lookup.GlobalCooldownSpeed(Character, true);
            float baseBossAttackSpeed   = Options.BossAttackSpeed * (1.0f - Stats.BossAttackSpeedMultiplier);
            float baseWeaponSpeed       = Lookup.WeaponSpeed(Character, Stats);
            float bossAttackHaste       = 0.0f;
            float weaponHaste           = 0.0f;
            
            BossAttackSpeed             = baseBossAttackSpeed;
            WeaponSpeed                 = baseWeaponSpeed;

            if (Options.UseParryHaste)
            {
                // Iterate on this a few times to get a 'stable' result
                for (int j = 0; j < 4; j++)
                {
                    weaponHaste = DefendTable.Parry * 0.24f * (WeaponSpeed / BossAttackSpeed);
                    bossAttackHaste = AttackTable.Parry * 0.24f * ((BossAttackSpeed / WeaponSpeed) + (BossAttackSpeed / globalCooldownSpeed));
                    
                    WeaponSpeed     = baseWeaponSpeed / (1.0f + weaponHaste);
                    BossAttackSpeed = baseBossAttackSpeed / (1.0f + bossAttackHaste);
                }
            }
            else
            {
                // Simple adjust to the defender's speed if the attacker isn't parry hasted
                WeaponSpeed /= (1.0f + (DefendTable.Parry * 0.24f * (WeaponSpeed / BossAttackSpeed)));
            }
        }

        public ParryModel(Character character, Stats stats, CalculationOptionsProtWarr options)
        {
            Character   = character;
            Stats       = stats;
            Options     = options;
            AttackTable = new AttackTable(character, stats, options);
            DefendTable = new DefendTable(character, stats, options);

            Calculate();
        }
    }
}
