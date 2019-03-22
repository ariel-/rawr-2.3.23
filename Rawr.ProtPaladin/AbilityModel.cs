using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.ProtPaladin {
    public class AbilityModel {
        private Ability Ability;
        //private DamageType DamageType;
        //private AttackType AttackType;
        private Character Character;
        private Stats Stats;
        private PaladinTalents Talents;
        private CalculationOptionsProtPaladin CalcOpts;
#if (RAWR3)
        private BossOptions BossOpts;
#endif

        public readonly AttackTable AttackTable;

        public string Name { get; private set; }
        public float Damage { get; private set; }
        public float Threat { get; private set; }
        public float DamageMultiplier { get; private set; }
        public float ArmorReduction { get; private set; }
        public float CritPercentage { get { return AttackTable.Critical; } }

        private void CalculateDamage() {
            float baseDamage = 0.0f;
            float critMultiplier = 0.0f;
            float duration = 0.0f;
            float AP = Stats.AttackPower;
            float SP = Stats.SpellPower;

#if (RAWR3)
            int targetLevel = BossOpts.Level;
#else
            int targetLevel = CalcOpts.TargetLevel;
#endif

            #region Ability Base Damage
            switch (Ability) {
                // White Damage
                case Ability.None:
                    baseDamage        = Stats.WeaponDamage;

                    DamageMultiplier *= (1.0f + Stats.BonusPhysicalDamageMultiplier)
                                      * (1.0f - (Lookup.GlancingReduction(Character, targetLevel) * AttackTable.Glance))
                                      * (1.0f - ArmorReduction);

                    critMultiplier    = 1.0f;
                    break;               
                case Ability.ShieldOfRighteousness:
                    float blockValue  = Stats.BlockValue + Stats.ShieldOfRighteousnessBlockValue + Stats.JudgementBlockValue + Stats.HolyShieldBlockValue;

                    float blockValueDRStart = 30 * Character.Level;

                    if (blockValue < blockValueDRStart) {
                        baseDamage    = blockValue;
                    } else if (blockValue < 39.5 * Character.Level) {
                        baseDamage    = blockValueDRStart + (0.95f * (blockValue - blockValueDRStart)) - (0.000625f * (float)Math.Pow(blockValue - blockValueDRStart, 2));
                    } else {
                        baseDamage    = blockValueDRStart + (0.95f * 9.5f * Character.Level) - (0.000625f * (float)Math.Pow(9.5 * Character.Level, 2));
                    }

                    baseDamage       += 520f
                                      + Stats.BonusShieldOfRighteousnessDamage;

                    DamageMultiplier *= (1.0f + Stats.BonusHolyDamageMultiplier);
                    critMultiplier    = 1.0f;
                    break;
                case Ability.HammerOfTheRighteous:
                    if (Talents.HammerOfTheRighteous == 0 || Character.MainHand == null)
                    {
                        Damage = 0.0f;
                        return;
                    }

                    baseDamage        = (Stats.WeaponDamage / Character.MainHand.Speed) * 4.0f;

                    DamageMultiplier *= (1.0f + Stats.BonusHolyDamageMultiplier)
                                      * (1.0f + Stats.BonusHammerOfTheRighteousMultiplier);

                    critMultiplier    = 1.0f;
                    break;
                // Seal of Vengeance is the tiny damage that applies on each swing; Holy Vengeance is the DoT
                // While trivial threat and damage, it's modeled for compatibility with Seal of Righteousness
                case Ability.SealOfVengeance:
                    baseDamage        = Stats.WeaponDamage * 0.33f;

                    DamageMultiplier *= (1.0f + Stats.BonusHolyDamageMultiplier)
                                      * (1.0f + 0.03f * Talents.SealsOfThePure)
                                      * (1.0f + Stats.BonusSealOfVengeanceDamageMultiplier);

                    critMultiplier    = 1.0f;
                    break;
                // 5 stacks of Holy Vengeance are assumed
                // TODO: implement stacking mechanic for beginning-of-fight TPS
                case Ability.HolyVengeance:
                    baseDamage        = 5f * (0.016f * SP + 0.032f * AP);

                    DamageMultiplier *= (1.0f + Stats.BonusHolyDamageMultiplier)
                                      * (1.0f + 0.03f * Talents.SealsOfThePure)
                                      * (1.0f + Stats.BonusSealOfVengeanceDamageMultiplier);

                    critMultiplier    = 0.0f;
                    break;
                // Judgement of Vengeance assumes 5 stacks of Holy Vengeance
                case Ability.JudgementOfVengeance:
                    float holyVengeanceStacks = 5;

                    baseDamage        = (0.22f * SP + 0.14f * AP) * (1.0f + 0.1f * holyVengeanceStacks);

                    DamageMultiplier *= (1.0f + Stats.BonusHolyDamageMultiplier)
                                      * (1.0f + 0.03f * Talents.SealsOfThePure)
                                      * (Talents.GlyphOfJudgement ? 1.1f : 1.0f);

                    critMultiplier    = 1.0f;
                    break;
                case Ability.SealOfRighteousness:
                    baseDamage        = Lookup.WeaponSpeed(Character, Stats) * ((0.022f * AP) + (0.044f * SP));

                    DamageMultiplier *= (1.0f + Stats.BonusHolyDamageMultiplier)
                                      * (1.0f + 0.03f * Talents.SealsOfThePure)
                                      * (1.0f + Stats.BonusSealOfRighteousnessDamageMultiplier)
                                      * (Talents.GlyphOfSealOfRighteousness ? 1.1f : 1.0f);

                    critMultiplier    = 0.0f;
                    break;
                case Ability.JudgementOfRighteousness:
                    baseDamage        = 1.0f + (0.2f * AP) + (0.32f * SP);

                    DamageMultiplier *= (1.0f + Stats.BonusHolyDamageMultiplier)
                                      * (1.0f + 0.03f * Talents.SealsOfThePure)
                                      * (Talents.GlyphOfJudgement ? 1.1f : 1.0f);

                    critMultiplier    = 1.0f;
                    break;
                case Ability.HolyShield:
                    if (Talents.HolyShield == 0)
                    {
                        Damage = 0.0f;
                        return;
                    }

                    baseDamage        = (211f + (0.056f * AP) + (0.09f * SP)) * 1.3f;
                    DamageMultiplier *= (1.0f + Stats.BonusHolyDamageMultiplier);
                    critMultiplier    = 0.0f;
                    break;
                case Ability.Consecration:
                    baseDamage        = 113f + (0.04f * (SP + Stats.ConsecrationSpellPower)) + (0.04f * AP);
                    duration          = (Talents.GlyphOfConsecration ? 10.0f : 8.0f);
                    DamageMultiplier *= (1.0f + Stats.BonusHolyDamageMultiplier);
                    critMultiplier    = 0.0f;
                    break;
                case Ability.Exorcism:
                    baseDamage        = 1087f + (0.15f * SP) + (0.15f * AP);

                    DamageMultiplier *= (1.0f + Stats.BonusHolyDamageMultiplier)
                                      * (1.0f + Talents.SanctityOfBattle * 0.05f)
                                      * (Talents.GlyphOfExorcism ? 1.2f : 1.0f);

                    critMultiplier    = 0.5f;
                    break;
                case Ability.AvengersShield:
                    baseDamage        = 1222f + (0.07f * SP) + (0.07f * AP);

                    DamageMultiplier *= (1.0f + Stats.BonusHolyDamageMultiplier)
                                      * (Talents.GlyphOfAvengersShield ? 2.0f : 1.0f);

                    critMultiplier    = 1.0f;
                    break;
                case Ability.HolyWrath:
                    baseDamage        = 1142f + (AP * 0.07f) + (SP * 0.07f); 
                    DamageMultiplier *= (1.0f + Stats.BonusHolyDamageMultiplier);
                    critMultiplier    = 0.5f;
                    break;
                case Ability.HammerOfWrath:
                    baseDamage        = 1198f + (AP * 0.15f) + (SP * 0.15f);
                    DamageMultiplier *= (1.0f + Stats.BonusHolyDamageMultiplier);
                    critMultiplier    = 1.0f;
                    break;
                case Ability.RetributionAura:
                    baseDamage        = 112f + (SP * 0.0666f);

                    DamageMultiplier *= (1.0f + Stats.BonusHolyDamageMultiplier)
                                      * (1.0f + Talents.SanctifiedRetribution * 0.5f);

                    critMultiplier    = 0.0f;
                    break;
            }
            #endregion

            // All damage multipliers, 1HWS, Armor etc...do we need to split buff/debuff ?
            baseDamage *= DamageMultiplier;

            #region Miss Chance, Avoidance Chance
            if (Lookup.IsSpell(Ability))
            {
                if (Ability == Ability.Consecration)
                    // Probability calculation, since each tick can be resisted individually.
                    baseDamage = Lookup.GetConsecrationTickChances(duration, baseDamage, AttackTable.Miss);
                else
                    // Missed spell attacks
                    // TODO: expand Ability Model to include a check for damage type, not only spell.
                    baseDamage *= (1.0f - AttackTable.Miss);
            } else {
                // Missed attacks
                if (Lookup.IsAvoidable(Ability))
                    baseDamage *= (1.0f - AttackTable.AnyMiss);
                else
                    baseDamage *= (1.0f - AttackTable.Miss);
            }
            #endregion

            #region Partial Resists
            // Partial Resists
            if (Lookup.HasPartials(Ability))
            {
                // Detailed table of Partial slices.
                float[] partialChanceTable = StatConversion.GetResistanceTable(Character.Level, targetLevel, 0.0f, Stats.SpellPenetration);

                // Here goes nothing, Damage averaged over the different partial slices that can happen.
                float partialDamage = 0.0f;
                
                for (int i = 0; i < 11; i++)
                    partialDamage += partialChanceTable[i] * (1.0f - 0.1f * (float)i) * baseDamage;
                
                baseDamage = partialDamage;
            }
            #endregion

            // Average critical strike bonuses
            if (Lookup.CanCrit(Ability))
                baseDamage += baseDamage * critMultiplier * AttackTable.Critical;

            // Final Damage the Ability deals
            Damage = baseDamage;
        }

        private void CalculateThreat() {
            // Base threat is always going to be the damage of the ability, if it is damaging
            float abilityThreat = Damage;
            float holyThreatModifier = 1.8f;

            switch (Ability) {
                case Ability.ShieldOfRighteousness:
                case Ability.HammerOfTheRighteous:
                case Ability.SealOfVengeance: 
                case Ability.HolyVengeance:
                case Ability.JudgementOfVengeance:
                case Ability.SealOfRighteousness:
                case Ability.JudgementOfRighteousness:
                case Ability.Exorcism:
                case Ability.HammerOfWrath:
                case Ability.AvengersShield:
                case Ability.HolyShield:
                case Ability.RetributionAura:
                case Ability.Consecration:
                case Ability.HolyWrath:
                    abilityThreat *= holyThreatModifier;
                    break;
            }

            abilityThreat *= Lookup.StanceThreatMultipler(Character, Stats);

            Threat = abilityThreat;
        }

#if (RAWR3)
        public AbilityModel(Character character, Stats stats, Ability ability, CalculationOptionsProtPaladin calcOpts, BossOptions bossOpts) {
#else   
        public AbilityModel(Character character, Stats stats, Ability ability, CalculationOptionsProtPaladin calcOpts) {
#endif
            Character   = character;
            Stats       = stats;
            Ability     = ability;
            CalcOpts    = calcOpts;
#if (RAWR3)
            BossOpts    = bossOpts;
#endif
            
            Talents     = Character.PaladinTalents;
#if (RAWR3)
            AttackTable = new AttackTable(character, stats, ability, CalcOpts, BossOpts);
#else
            AttackTable = new AttackTable(character, stats, ability, CalcOpts);
#endif

            if (!Lookup.IsSpell(Ability))
#if (RAWR3)
                ArmorReduction  = Lookup.EffectiveTargetArmorReduction(Character, Stats, BossOpts.Armor, BossOpts.Level);
#else
                ArmorReduction  = Lookup.EffectiveTargetArmorReduction(Character, Stats, CalcOpts.TargetArmor, CalcOpts.TargetLevel);
#endif

            Name                = Lookup.Name(Ability);
            DamageMultiplier    = Lookup.StanceDamageMultipler(Character, Stats);
            DamageMultiplier   *= Lookup.CreatureTypeDamageMultiplier(Character, CalcOpts.TargetType);

            CalculateDamage();
            CalculateThreat();
        }
    }

    public class AbilityModelList : Dictionary<Ability, AbilityModel>
    {
#if (RAWR3)
        public void Add(Ability ability, Character character, Stats stats, CalculationOptionsProtPaladin calcOpts, BossOptions bossOpts)
#else
        public void Add(Ability ability, Character character, Stats stats, CalculationOptionsProtPaladin calcOpts)
#endif
        {
#if (RAWR3)
            this.Add(ability, new AbilityModel(character, stats, ability, calcOpts, bossOpts));
#else
            this.Add(ability, new AbilityModel(character, stats, ability, calcOpts));
#endif
        }
    }
}
