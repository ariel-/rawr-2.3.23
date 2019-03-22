using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Rawr.ProtWarr
{
    public class AttackModel
    {
        private Character Character;
        private CalculationOptionsProtWarr Options;
        private Stats Stats;
        private WarriorTalents Talents;
        //private WarriorTalentsCata TalentsCata;
        private DefendTable DefendTable;
        private ParryModel ParryModel;

        public AbilityModelList Abilities = new AbilityModelList();

        private AttackModelMode _attackModelMode;
        public AttackModelMode AttackModelMode
        {
            get { return _attackModelMode; }
            set { _attackModelMode = value; Calculate(); }
        }

        private RageModelMode _rageModelMode;
        public RageModelMode RageModelMode
        {
            get { return _rageModelMode; }
            set { _rageModelMode = value; Calculate(); }
        }

        public string Name { get; private set; }
        public string ShortName 
        {
            get { return Name.Replace("Sword and Board", "SnB").Replace("Revenge + Shockwave", "Rev + Shockwave"); }
        }
        public string Description { get; private set; }
        public float ThreatPerSecond { get; private set; }
        public float DamagePerSecond { get; private set; }
        public float WeaponAttacksPerSecond { get; private set; }
        public float HitsPerSecond { get; private set; }
        public float CritsPerSecond { get; private set; }
        public float AttackerSwingsPerSecond { get; private set; }
        public float AttackerHitsPerSecond { get; private set; }

        private void Calculate()
        {
            Dictionary<Ability, float> modelAbilities = new Dictionary<Ability, float>();
            float modelLength = 0.0f;
            float modelThreat = 0.0f;
            float modelDamage = 0.0f;
            float modelCrits = 0.0f;
            float modelHits = 0.0f;
            float modelWeaponAttacks = 0.0f;

            // Rotation Auto-Detection
            if (AttackModelMode == AttackModelMode.Optimal)
            {
                if (Talents.Shockwave == 1 && Talents.SwordAndBoard == 3)
                    if (Talents.ImprovedRevenge > 0)
                        AttackModelMode = AttackModelMode.FullProtectionRevenge;
                    else
                        AttackModelMode = AttackModelMode.FullProtection;
                else if(Talents.SwordAndBoard == 3)
                    if (Talents.ImprovedRevenge > 0)
                        AttackModelMode = AttackModelMode.SwordAndBoardRevenge;
                    else
                        AttackModelMode = AttackModelMode.SwordAndBoard;
                else if(Talents.Devastate == 1)
                    if (Talents.ImprovedRevenge > 0)
                        AttackModelMode = AttackModelMode.DevastateRevenge;
                    else
                        AttackModelMode = AttackModelMode.Devastate;
                else if(Talents.UnrelentingAssault == 2)
                    AttackModelMode = AttackModelMode.UnrelentingAssault;
                else
                    AttackModelMode = AttackModelMode.Basic;                   
            }

            switch (AttackModelMode)
            {
                case AttackModelMode.Basic:
                {
                    // Basic Rotation
                    // Shield Slam -> Revenge -> Sunder Armor -> Sunder Armor
                    Name        = "Basic Cycle";
                    Description = "Shield Slam -> Revenge -> Sunder Armor -> Sunder Armor";
                    modelLength = 6.0f;
                    modelAbilities.Add(Ability.ShieldSlam, 1.0f);
                    modelAbilities.Add(Ability.Revenge, 1.0f);
                    modelAbilities.Add(Ability.SunderArmor, 2.0f);
                    break;
                }
                case AttackModelMode.Devastate:
                {
                    // Devastate Rotation
                    // Shield Slam -> Devastate -> Devastate -> Devastate
                    Name = "Devastate";
                    Description = "Shield Slam -> Devastate -> Devastate -> Devastate";
                    modelLength = 6.0f;
                    modelAbilities.Add(Ability.ShieldSlam, 1.0f);
                    modelAbilities.Add(Ability.Devastate, 3.0f);
                    break;
                }
                case AttackModelMode.DevastateRevenge:
                {
                    // Devastate + Revenge Rotation
                    // Shield Slam -> Revenge -> Devastate -> Devastate
                    Name = "Devastate + Revenge";
                    Description = "Shield Slam -> Revenge -> Devastate -> Devastate";
                    modelLength = 6.0f;
                    modelAbilities.Add(Ability.ShieldSlam, 1.0f);
                    modelAbilities.Add(Ability.Revenge, 1.0f);
                    modelAbilities.Add(Ability.Devastate, 2.0f);
                    break;
                }
                case AttackModelMode.SwordAndBoard:
                case AttackModelMode.SwordAndBoardRevenge:
                case AttackModelMode.FullProtection:
                case AttackModelMode.FullProtectionRevenge:
                {
                    // Sword And Board Rotation
                    // Shield Slam > Revenge > Devastate
                    // The distribution of abilities in the model is as follows:
                    // 1.0 * Shield Slam + 0.73 * Revenge + 1.4596 * Devastate
                    // -or-
                    // Shield Slam > Revenge > Devastate @ 3s Shield Slam Cooldown > Shockwave > Devastate
                    // The distribution of abilities in the model is as follows:
                    // 1.0 * Shield Slam + 0.73 * Revenge + 1.133 * Devastate + 0.3266 * (Concussion Blow/Shockwave/Devastate)
                    // The cycle length is 4.7844s, abilities per cycle is 3.1896
                    Name = "Sword and Board";
                    Description = "Shield Slam > ";
                    if (AttackModelMode == AttackModelMode.SwordAndBoardRevenge || AttackModelMode == AttackModelMode.FullProtectionRevenge)
                    {
                        Name += " + Revenge";
                        Description += "Revenge > Devastate";
                    }
                    else
                        Description += "Devastate";
                    if (AttackModelMode == AttackModelMode.FullProtection || AttackModelMode == AttackModelMode.FullProtectionRevenge)
                    {
                        Name += " + Shockwave";
                        Description += "\n@ 1.5s Shield Slam Cooldown: Shockwave > Devastate";
                    }

                    modelLength = 4.7844f;
                    modelAbilities.Add(Ability.ShieldSlam, 1.0f);
                    modelAbilities.Add(Ability.Devastate, 1.133f);
                    
                    // Add Revenge, if applicable
                    if (AttackModelMode == AttackModelMode.SwordAndBoardRevenge || AttackModelMode == AttackModelMode.FullProtectionRevenge)
                        modelAbilities.Add(Ability.Revenge, 0.73f);
                    else
                        modelAbilities[Ability.Devastate] += 0.73f;
                    
                    // Add Shockwave, if applicable
                    if (AttackModelMode == AttackModelMode.FullProtection || AttackModelMode == AttackModelMode.FullProtectionRevenge)
                    {
                        modelAbilities.Add(Ability.Shockwave, (0.3266f / 3.0f));
                        modelAbilities[Ability.Devastate] += (0.3266f / 3.0f) * 2.0f;
                    }
                    else
                        modelAbilities[Ability.Devastate] += 0.3266f;

                    break;
                }
                case AttackModelMode.UnrelentingAssault:
                {
                    Name = "Unrelenting Assault";
                    Description = "Revenge Only";
                    modelLength = 1.0f;
                    modelAbilities.Add(Ability.Revenge, 1.0f);
                    break;
                }
            }

            // Accumulate base model statistics
            foreach (KeyValuePair<Ability, float> modelAbility in modelAbilities)
            {
                AbilityModel ability = Abilities[modelAbility.Key];
                modelThreat += ability.Threat * modelAbility.Value;
                modelDamage += ability.Damage * modelAbility.Value;
                if (ability.Damage > 0.0f)
                {
                    modelHits += ability.HitPercentage * modelAbility.Value;
                    modelCrits += ability.CritPercentage * modelAbility.Value;
                }
                if (ability.IsWeaponAttack)
                    modelWeaponAttacks += modelAbility.Value;
            }

            // Simple GCD-Based Latency Adjustment
            modelLength *= Lookup.GlobalCooldownSpeed(Character, true) / Lookup.GlobalCooldownSpeed(Character, false);

            // Weapon Swings/Heroic Strike
            float attackerSwings = (modelLength / ParryModel.BossAttackSpeed);
            float attackerHits = DefendTable.AnyHit * attackerSwings;
            float weaponSwings = modelLength / ParryModel.WeaponSpeed;
            float heroicStrikePercentage = Math.Max(0.0f, Math.Min(1.0f, Options.HeroicStrikeFrequency));

            AbilityModel heroicStrike = Abilities[Ability.HeroicStrike];
            modelThreat += heroicStrike.Threat * weaponSwings * heroicStrikePercentage;
            modelDamage += heroicStrike.Damage * weaponSwings * heroicStrikePercentage;
            modelHits   += heroicStrike.HitPercentage * weaponSwings * heroicStrikePercentage;
            modelCrits  += heroicStrike.CritPercentage * weaponSwings * heroicStrikePercentage;

            AbilityModel whiteSwing = Abilities[Ability.None];
            modelThreat += whiteSwing.Threat * weaponSwings * (1.0f - heroicStrikePercentage);
            modelDamage += whiteSwing.Damage * weaponSwings * (1.0f - heroicStrikePercentage);
            modelCrits  += whiteSwing.CritPercentage * weaponSwings * (1.0f - heroicStrikePercentage);
            modelHits   += whiteSwing.HitPercentage * weaponSwings * (1.0f - heroicStrikePercentage);

            modelWeaponAttacks += weaponSwings;

            // Damage Shield
            AbilityModel damageShield = Abilities[Ability.DamageShield];
            modelThreat += damageShield.Threat * attackerHits;
            modelDamage += damageShield.Damage * attackerHits;

            // Deep Wounds
            AbilityModel deepWounds = Abilities[Ability.DeepWounds];
            modelThreat += deepWounds.Threat * modelCrits;
            modelDamage += deepWounds.Damage * modelCrits;

            // Misc. Power Gains
            modelThreat += DefendTable.DodgeParryBlock * (modelLength / ParryModel.BossAttackSpeed) * 25.0f * 
                (Talents.ShieldSpecialization * 0.2f);
            modelThreat += DefendTable.DodgeParryBlock * (modelLength / ParryModel.BossAttackSpeed) * 1.0f * 
                Lookup.StanceThreatMultipler(Character, Stats) * (Talents.ImprovedDefensiveStance * 0.5f);

            // Vigilance, is already calculated as TPS
            if (Options.UseVigilance)
                modelThreat += Abilities[Ability.Vigilance].Threat * modelLength;

            // Final Per-Second Calculations
            ThreatPerSecond             = modelThreat / modelLength;
            DamagePerSecond             = modelDamage / modelLength;
            WeaponAttacksPerSecond      = modelWeaponAttacks / modelLength;
            HitsPerSecond               = modelHits / modelLength;
            CritsPerSecond              = modelCrits / modelLength;
            AttackerSwingsPerSecond     = attackerSwings / modelLength;
            AttackerHitsPerSecond       = attackerHits / modelLength;
        }

        public AttackModel(Character character, Stats stats, CalculationOptionsProtWarr options, AttackModelMode attackModelMode)
            : this(character, stats, options, attackModelMode, RageModelMode.Infinite)
        {
        }

        public AttackModel(Character character, Stats stats, CalculationOptionsProtWarr options, AttackModelMode attackModelMode, RageModelMode rageModelMode)
        {
            Character        = character;
            Stats            = stats;
            Options          = options;
            Talents          = Character.WarriorTalents;
            DefendTable      = new DefendTable(character, stats, options);
            ParryModel       = new ParryModel(character, stats, options);
            _attackModelMode = attackModelMode;
            _rageModelMode   = rageModelMode;

            Abilities.Add(Ability.None, character, stats, options);
            Abilities.Add(Ability.Cleave, character, stats, options);
            Abilities.Add(Ability.ConcussionBlow, character, stats, options);
            Abilities.Add(Ability.DamageShield, character, stats, options);
            Abilities.Add(Ability.DeepWounds, character, stats, options);
            Abilities.Add(Ability.Devastate, character, stats, options);
            Abilities.Add(Ability.HeroicStrike, character, stats, options);
            Abilities.Add(Ability.HeroicThrow, character, stats, options);
            Abilities.Add(Ability.MockingBlow, character, stats, options);
            Abilities.Add(Ability.Rend, character, stats, options);
            Abilities.Add(Ability.Revenge, character, stats, options);
            Abilities.Add(Ability.ShieldSlam, character, stats, options);
            Abilities.Add(Ability.Shockwave, character, stats, options);
            Abilities.Add(Ability.Slam, character, stats, options);
            Abilities.Add(Ability.SunderArmor, character, stats, options);
            Abilities.Add(Ability.ThunderClap, character, stats, options);
            Abilities.Add(Ability.Vigilance, character, stats, options);

            Calculate();
        }
    }
}