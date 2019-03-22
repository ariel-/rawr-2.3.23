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
        public string Description { get; private set; }
        public float ThreatPerSecond { get; private set; }
        public float DamagePerSecond { get; private set; }
        public float AttacksPerSecond { get; private set; }
        public float CritsPerSecond { get; private set; }
        public float AttackerHitsPerSecond { get; private set; }

        private void Calculate()
        {
            float modelLength = 0.0f;
            float modelThreat = 0.0f;
            float modelDamage = 0.0f;
            float modelCrits = 0.0f;
            float modelHits = 0.0f;

            switch (AttackModelMode)
            {
                case AttackModelMode.Basic:
                    {
                        // Basic Rotation
                        // Shield Slam -> Revenge -> Sunder Armor -> Sunder Armor
                        Name        = "Basic Cycle";
                        Description = "Shield Slam -> Revenge -> Sunder Armor -> Sunder Armor";
                        modelLength = 6.0f;
                        modelThreat = 
                            Abilities[Ability.ShieldSlam].Threat + 
                            Abilities[Ability.Revenge].Threat + 
                            Abilities[Ability.SunderArmor].Threat * 2;
                        modelDamage = 
                            Abilities[Ability.ShieldSlam].Damage +
                            Abilities[Ability.Revenge].Damage;
                        modelCrits  = 
                            Abilities[Ability.ShieldSlam].CritPercentage +
                            Abilities[Ability.Revenge].CritPercentage;
                        modelHits =
                            Abilities[Ability.ShieldSlam].HitPercentage +
                            Abilities[Ability.Revenge].HitPercentage;
                        break;
                    }
                case AttackModelMode.Devastate:
                    {
                        // Devastate Rotation
                        // Shield Slam -> Revenge -> Devastate -> Devastate
                        if (Character.WarriorTalents.Devastate == 1)
                        {
                            Name        = "Devastate";
                            Description = "Shield Slam -> Revenge -> Devastate -> Devastate";
                            modelLength = 6.0f;
                            modelThreat =
                                Abilities[Ability.ShieldSlam].Threat +
                                Abilities[Ability.Revenge].Threat +
                                Abilities[Ability.Devastate].Threat * 2;
                            modelDamage =
                                Abilities[Ability.ShieldSlam].Damage +
                                Abilities[Ability.Revenge].Damage +
                                Abilities[Ability.Devastate].Damage * 2;
                            modelCrits =
                                Abilities[Ability.ShieldSlam].CritPercentage +
                                Abilities[Ability.Revenge].CritPercentage +
                                Abilities[Ability.Devastate].CritPercentage * 2;
                            modelHits =
                               Abilities[Ability.ShieldSlam].HitPercentage +
                               Abilities[Ability.Revenge].HitPercentage +
                               Abilities[Ability.Devastate].HitPercentage * 2;
                        }
                        else
                            goto case AttackModelMode.Basic;
                        break;
                    }
                case AttackModelMode.SwordAndBoard:
                    {
                        // Sword And Board Rotation
                        // Requires 3 points in Sword and Board
                        // Shield Slam > Revenge > Devastate
                        // The distribution of abilities in the model is as follows:
                        // 1.0 * Shield Slam + 0.73 * Revenge + 1.4596 * Devastate
                        // The cycle length is 4.7844s, abilities per cycle is 3.1896
                        if (Character.WarriorTalents.SwordAndBoard == 3)
                        {
                            Name        = "Sword And Board";
                            Description = "Shield Slam > Revenge > Devastate";
                            modelLength = 4.7644f;
                            modelThreat =
                                (1.0f * Abilities[Ability.ShieldSlam].Threat) +
                                (0.73f * Abilities[Ability.Revenge].Threat) +
                                (1.4596f * Abilities[Ability.Devastate].Threat);
                            modelDamage = 
                                (1.0f * Abilities[Ability.ShieldSlam].Damage) +
                                (0.73f * Abilities[Ability.Revenge].Damage) +
                                (1.4596f * Abilities[Ability.Devastate].Damage);
                            modelCrits = 
                                (1.0f * Abilities[Ability.ShieldSlam].CritPercentage) +
                                (0.73f * Abilities[Ability.Revenge].CritPercentage) +
                                (1.4596f * Abilities[Ability.Devastate].CritPercentage);
                            modelHits =
                                (1.0f * Abilities[Ability.ShieldSlam].HitPercentage) +
                                (0.73f * Abilities[Ability.Revenge].HitPercentage) +
                                (1.4596f * Abilities[Ability.Devastate].HitPercentage);
                        }
                        else
                            goto case AttackModelMode.Basic;
                        break;
                    }
                case AttackModelMode.FullProtection:
                    {
                        // Sword And Board + Shockwave/Concussion Blow Rotation
                        // Requires 3 points in Sword and Board, Shockwave, and Concussion Blow
                        // Shield Slam > Revenge > Devastate @ 3s Shield Slam Cooldown > Concussion Blow > Shockwave > Devastate
                        // The distribution of abilities in the model is as follows:
                        // 1.0 * Shield Slam + 0.73 * Revenge + 1.133 * Devastate + 0.3266 * (Concussion Blow/Shockwave/Devastate)
                        // The cycle length is 4.7844s, abilities per cycle is 3.1896
                        if (Character.WarriorTalents.SwordAndBoard == 3 && Character.WarriorTalents.ConcussionBlow == 1 && Character.WarriorTalents.Shockwave == 1)
                        {
                            Name        = "Sword And Board + CB/SW";
                            Description = "Shield Slam > Revenge > Devastate\n@ 3s Shield Slam Cooldown: Concussion Blow > Shockwave > Devastate";
                            modelLength = 4.7644f;
                            modelThreat =
                                (1.0f * Abilities[Ability.ShieldSlam].Threat) +
                                (0.73f * Abilities[Ability.Revenge].Threat) +
                                (1.133f * Abilities[Ability.Devastate].Threat) +
                                (0.3266f * ((
                                    Abilities[Ability.ConcussionBlow].Threat + 
                                    Abilities[Ability.Shockwave].Threat + 
                                    Abilities[Ability.Devastate].Threat
                                    ) / 3));
                            modelDamage = 
                                (1.0f * Abilities[Ability.ShieldSlam].Damage) +
                                (0.73f * Abilities[Ability.Revenge].Damage) +
                                (1.133f * Abilities[Ability.Devastate].Damage) +
                                (0.3266f * ((
                                    Abilities[Ability.ConcussionBlow].Damage + 
                                    Abilities[Ability.Shockwave].Damage + 
                                    Abilities[Ability.Devastate].Damage
                                    ) / 3));
                            modelCrits = 
                                (1.0f * Abilities[Ability.ShieldSlam].CritPercentage) +
                                (0.73f * Abilities[Ability.Revenge].CritPercentage) +
                                (1.133f * Abilities[Ability.Devastate].CritPercentage) +
                                (0.3266f * ((
                                    Abilities[Ability.ConcussionBlow].CritPercentage + 
                                    Abilities[Ability.Shockwave].CritPercentage + 
                                    Abilities[Ability.Devastate].CritPercentage
                                    ) / 3));
                            modelHits =
                                (1.0f * Abilities[Ability.ShieldSlam].HitPercentage) +
                                (0.73f * Abilities[Ability.Revenge].HitPercentage) +
                                (1.133f * Abilities[Ability.Devastate].HitPercentage) +
                                (0.3266f * ((
                                    Abilities[Ability.ConcussionBlow].HitPercentage +
                                    Abilities[Ability.Shockwave].HitPercentage +
                                    Abilities[Ability.Devastate].HitPercentage
                                    ) / 3));
                        }
                        else
                            goto case AttackModelMode.Basic;
                        break;
                    }
                case AttackModelMode.UnrelentingAssault:
                    {
                        // Unrelenting Assault 'Protection' Build
                        // Requires 2 points in Unrelenting Assault
                        // Shield Slam -> Revenge -> Revenge -> Revenge
                        if (Character.WarriorTalents.UnrelentingAssault == 2)
                        {
                            Name        = "Unrelenting Assault";
                            Description = "Revenge";
                            modelLength = 1.0f;
                            modelThreat = Abilities[Ability.Revenge].Threat;
                            modelDamage = Abilities[Ability.Revenge].Damage;
                            modelCrits  = Abilities[Ability.Revenge].CritPercentage;
                            modelHits   = Abilities[Ability.Revenge].HitPercentage;
                        }
                        else
                            goto case AttackModelMode.Basic;
                        break;
                    }
            }

            // White Damage
            float weaponHits = modelLength / ParryModel.WeaponSpeed; //Lookup.WeaponSpeed(Character, Stats);
            if (RageModelMode == RageModelMode.Infinite)
            {
                // Convert all white hits to heroic strikes
                modelThreat += Abilities[Ability.HeroicStrike].Threat * weaponHits;
                modelDamage += Abilities[Ability.HeroicStrike].Damage * weaponHits;
                modelCrits  += Abilities[Ability.HeroicStrike].CritPercentage * weaponHits;
                modelHits   += Abilities[Ability.HeroicStrike].HitPercentage * weaponHits;
            }
            else
            {
                // Normal white hits if we aren't using infinite rage, add some logic for a hybrid system later...
                modelThreat += Abilities[Ability.None].Threat * weaponHits;
                modelDamage += Abilities[Ability.None].Damage * weaponHits;
                modelCrits  += Abilities[Ability.None].CritPercentage * weaponHits;
                modelHits   += Abilities[Ability.None].HitPercentage * weaponHits;
            }

            // Damage Shield
            float attackerHits = DefendTable.AnyHit * (modelLength / ParryModel.BossAttackSpeed); //Options.BossAttackSpeed;
            modelThreat += Abilities[Ability.DamageShield].Threat * attackerHits;
            modelDamage += Abilities[Ability.DamageShield].Damage * attackerHits;
            modelCrits  += Abilities[Ability.DamageShield].CritPercentage * attackerHits;

            // Deep Wounds
            modelThreat += Abilities[Ability.DeepWounds].Threat * modelCrits;
            modelDamage += Abilities[Ability.DeepWounds].Damage * modelCrits;

            // Vigilance
            if (Options.UseVigilance)
                modelThreat += Abilities[Ability.Vigilance].Threat * modelLength;

            ThreatPerSecond = modelThreat / modelLength;
            DamagePerSecond = modelDamage / modelLength;
            AttacksPerSecond = modelHits / modelLength;
            CritsPerSecond = modelCrits / modelLength;
            AttackerHitsPerSecond = attackerHits / modelLength;
        }

        public AttackModel(Character character, Stats stats, AttackModelMode attackModelMode)
            : this(character, stats, attackModelMode, RageModelMode.Infinite)
        {
        }

        public AttackModel(Character character, Stats stats, AttackModelMode attackModelMode, RageModelMode rageModelMode)
        {
            Character        = character;
            Options          = Character.CalculationOptions as CalculationOptionsProtWarr;
            Stats            = stats;
            DefendTable      = new DefendTable(character, stats);
            ParryModel       = new ParryModel(character, stats);
            _attackModelMode = attackModelMode;
            _rageModelMode   = rageModelMode;

            Abilities.Add(Ability.None, character, stats);
            Abilities.Add(Ability.Cleave, character, stats);
            Abilities.Add(Ability.ConcussionBlow, character, stats);
            Abilities.Add(Ability.DamageShield, character, stats);
            Abilities.Add(Ability.DeepWounds, character, stats);
            Abilities.Add(Ability.Devastate, character, stats);
            Abilities.Add(Ability.HeroicStrike, character, stats);
            Abilities.Add(Ability.HeroicThrow, character, stats);
            Abilities.Add(Ability.Rend, character, stats);
            Abilities.Add(Ability.Revenge, character, stats);
            Abilities.Add(Ability.ShieldSlam, character, stats);
            Abilities.Add(Ability.Shockwave, character, stats);
            Abilities.Add(Ability.Slam, character, stats);
            Abilities.Add(Ability.SunderArmor, character, stats);
            Abilities.Add(Ability.ThunderClap, character, stats);
            Abilities.Add(Ability.Vigilance, character, stats);

            Calculate();
        }
    }
}