using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Rawr.Warlock {

    public abstract class Pet {

        public static List<string> ALL_PETS = new List<String>();

        static Pet() {
            Type petType = Type.GetType("Rawr.Warlock.Pet");
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) {
                if (type.IsSubclassOf(petType)) {
                    ALL_PETS.Add(type.Name);
                }
            }
        }

        #region properties

        public CharacterCalculationsWarlock Mommy { get; protected set; }
        public Stats Stats { get; protected set; }

        public float BaseStamina { get; protected set; }
        public float StaminaCoef { get; protected set; }
        public float HealthPerStamina { get; protected set; }
        public float BaseHealth { get; protected set; }

        public float BaseIntellect { get; protected set; }
        public float IntellectCoef { get; protected set; }
        public float ManaPerIntellect { get; protected set; }
        public float BaseMana { get; protected set; }
        public float SpellCritPerIntellect { get; protected set; }
        public float BaseSpellCrit { get; protected set; }

        public float BaseSpellPower { get; protected set; }
        public float SpellPowerCoef { get; protected set; }

        public float BaseAttackPower { get; protected set; }
        public float AttackPowerCoef { get; protected set; }

        public float BaseMeleeDamage { get; protected set; }
        public float DamagePerAttackPower { get; protected set; }

        public float SpecialCooldown { get; protected set; }
        public float SpecialCastTime { get; protected set; }
        public float SpecialBaseDamage { get; protected set; }
        public float SpecialDamagePerSpellPower { get; protected set; }

        public SpellModifiers TotalModifiers { get; protected set; }
        public SpellModifiers MeleeModifiers { get; protected set; }
        public SpellModifiers SpecialModifiers { get; protected set; }

        #endregion

        #region init

        public Pet(
            CharacterCalculationsWarlock mommy,
            float baseHealth,
            float healthPerStamina) {

            Mommy = mommy;
            TotalModifiers = new SpellModifiers();
            MeleeModifiers = new SpellModifiers();
            SpecialModifiers = new SpellModifiers();

            BaseStamina = 328f;
            StaminaCoef = .75f;
            HealthPerStamina = healthPerStamina;
            BaseHealth = baseHealth;

            BaseIntellect = 150f;
            IntellectCoef = .3f;
            ManaPerIntellect = 11.55f;
            BaseMana = 1343f;
            BaseSpellCrit = .03333f;

            BaseSpellPower = 0f;
            SpellPowerCoef = .15f;

            BaseAttackPower = 294f;
            AttackPowerCoef = .57f;
        }

        public void CalcStats1() {

            // Stam & Int have to happen in this stage, so that Demonic
            // Knowledge is in effect when calculating the benefit of a
            // Demonic Pact proc.

            // Crit has to happen in this stage, so that Empowered Imp gives
            // the warlocks the right amount of crit.

            WarlockTalents talents = Mommy.Talents;
            float vitality = talents.FelVitality * .05f;
            float tacticsCrit
                = .02f * talents.DemonicTactics
                    + .1f
                        * talents.ImprovedDemonicTactics
                        * (Mommy.CalcSpellCrit()
                            - Mommy.Stats.SpellCritOnTarget);
            Stats = new Stats() {
                Stamina = BaseStamina + StaminaCoef * Mommy.CalcStamina(),
                Intellect
                    = BaseIntellect + IntellectCoef * Mommy.CalcIntellect(),
                Strength = 297f,
                Agility = 90f,
                BonusStaminaMultiplier = vitality,
                BonusIntellectMultiplier = vitality,
                SpellCrit
                    = BaseSpellCrit + tacticsCrit + Mommy.Stats.Warlock2T9,
                SpellPower = BaseSpellPower,
                AttackPower = BaseAttackPower,
                PhysicalCrit = .0329f + tacticsCrit + Mommy.Stats.Warlock2T9,
            };
            Stats.Accumulate(Mommy.PetBuffs);

            Mommy.Add4pT10(TotalModifiers);
            FinalizeModifiers();
            SpecialModifiers.Accumulate(TotalModifiers);
            MeleeModifiers.Accumulate(TotalModifiers);

            Stats.SpellHaste = GetHasteWithProcs(s => s.SpellHaste);
            Stats.PhysicalHaste = GetHasteWithProcs(s => s.PhysicalHaste);
        }

        private float GetHasteWithProcs(StatExtractor statExtractor) {

            Dictionary<int, float> periods = new Dictionary<int, float>();
            Dictionary<int, float> chances = new Dictionary<int, float>();
            periods.Add((int) Trigger.Use, 0f);
            chances.Add((int) Trigger.Use, 1f);
            WeightedStat[] hasteProcs
                = Mommy.GetUptimes(
                    Stats,
                    periods,
                    chances,
                    statExtractor,
                    (a, b, c, d, e, f, g, h)
                        => SpecialEffect
                                .GetAverageCombinedUptimeCombinationsMultiplicative(
                            a, b, c, d, e, f, g, h));
            float avgProcHaste = 0f;
            foreach (WeightedStat proc in hasteProcs) {
                avgProcHaste += proc.Chance * proc.Value;
            }
            return (1f + statExtractor(Stats)) * (1f + avgProcHaste) - 1f;
        }

        public void CalcStats2() {

            Stats.SpellPower += SpellPowerCoef * Mommy.CalcSpellPower();
            Stats.AttackPower += AttackPowerCoef * Mommy.CalcSpellPower();
        }

        protected virtual void FinalizeModifiers() {

            TotalModifiers.AddMultiplicativeMultiplier(
                Stats.BonusDamageMultiplier);
            MeleeModifiers.AddAdditiveMultiplier(
                .04f * Mommy.Talents.UnholyPower);
            MeleeModifiers.AddMultiplicativeMultiplier(
                Stats.BonusPhysicalDamageMultiplier);
            if (Mommy.Character.Race == CharacterRace.Orc) {
                TotalModifiers.AddAdditiveMultiplier(.05f);
            }
        }

        #endregion

        #region stats

        public float CalcStamina() {

            return StatUtils.CalcStamina(Stats);
        }

        public float CalcIntellect() {

            return StatUtils.CalcIntellect(Stats);
        }

        public float CalcHealth() {

            // sadly, pets do not have the same stamina->health conversion as
            // players, so have to write our own CalcHealth method.
            return (Stats.Health + HealthPerStamina * CalcStamina())
                * (1 + Stats.BonusHealthMultiplier);
        }

        public float CalcSpellPower() {

            return StatUtils.CalcSpellPower(Stats);
        }

        public float CalcSpellCrit() {

            return StatUtils.CalcSpellCrit(Stats);
        }

        public float CalcAttackPower() {

            return StatUtils.CalcAttackPower(Stats, 1f, 0f);
        }

        public float CalcMeleeCrit() {

            return StatUtils.CalcPhysicalCrit(
                Stats, .00019f, Mommy.Options.TargetLevel - 80);
        }

        public float CalcMeleeHaste() {

            return 1f + Stats.PhysicalHaste;
        }

        #endregion

        #region dps

        public float CalcMeleeSpeed() {

            return 2f / CalcMeleeHaste();
        }

        public float CalcMeleeHitChance() {

            // the warlock's miss rate, not including buff/debuff
            float miss
                = .17f
                    - Mommy.CalcSpellHit()
                    + Stats.SpellHit
                    - Mommy.Talents.Suppression * .01f;
           
            // adjust to melee miss rate
            miss *= 8f / 13f;

            // add in physical hit buff/debuffs
            miss -= Stats.PhysicalHit;

            return Math.Min(1f, 1f - miss);
        }

        public float CalcMeleeDamage() {

            return CalcMeleeDamage(true, 0f);
        }

        protected float CalcMeleeDamage(bool canGlance, float bonusDamage) {

            int level = Mommy.Options.TargetLevel;
            int levelDelta = level - 80;

            float characterSheetDamage
                = BaseMeleeDamage
                    + DamagePerAttackPower * CalcAttackPower()
                    + bonusDamage;
            float combatTableModifier
                = CalcMeleeHitChance()
                    + CalcMeleeCrit()
                    - StatConversion.WHITE_DODGE_CHANCE_CAP[levelDelta];
            if (canGlance) {
                combatTableModifier
                    -= .3f * StatConversion.WHITE_GLANCE_CHANCE_CAP[levelDelta];
            }
            float armorModifier
                = 1
                    - StatConversion.GetArmorDamageReduction(
                        level,
                        StatConversion.NPC_ARMOR[levelDelta],
                        Stats.ArmorPenetration, // arpen debuffs
                        0f, // arpen buffs
                        0f); // arpen rating
            return characterSheetDamage
                * combatTableModifier
                * armorModifier
                * MeleeModifiers.GetFinalDirectMultiplier();
        }

        public float CalcMeleeDps() {

            return CalcMeleeDamage() / CalcMeleeSpeed();
        }

        public float GetSpecialSpeed() {

            return SpecialCooldown
                + SpecialCastTime / StatUtils.CalcSpellHaste(Stats);
        }

        public virtual float GetSpecialDamage() {

            float resist
                = StatConversion.GetAverageResistance(
                    80, Mommy.Options.TargetLevel, 0f, 0f);
            float nonCrit
                = (SpecialBaseDamage
                        + SpecialDamagePerSpellPower * CalcSpellPower())
                    * SpecialModifiers.GetFinalDirectMultiplier()
                    * (1 - resist);
            float crit = nonCrit * SpecialModifiers.GetFinalCritMultiplier();
            float critChance = CalcSpellCrit();
            return Mommy.HitChance
                * Utilities.GetWeightedSum(
                    crit, critChance, nonCrit, 1 - critChance);
        }

        public float CalcSpecialDps() {

            float speed = GetSpecialSpeed();
            if (speed == 0) {
                return 0f;
            } else {
                return GetSpecialDamage() / GetSpecialSpeed();
            }
        }

        #endregion

        public float ApplyPactProcBenefit() {

            float benefit = GetPactProcBenefit();
            Stats.SpellPower += benefit;
            return benefit;
        }

        public float GetPactProcBenefit() {

            float pact = .02f * Mommy.Talents.DemonicPact;
            if (pact == 0) {
                return 0f;
            }

            float buff
                = StatUtils.GetBuffEffect(
                    Mommy.Character.ActiveBuffs,
                    Mommy.CalcSpellPower() * pact,
                    "Spell Power",
                    s => s.SpellPower);
            if (buff == 0) {
                return 0f;
            }

            SpecialEffect pactEffect
                = new SpecialEffect(0, null, 45f, 20f);
            float meleeRate;
            if (BaseMeleeDamage == 0) {
                meleeRate = 0f;
            } else {
                meleeRate = 1 / CalcMeleeSpeed();
            }
            float spellRate;
            if (SpecialDamagePerSpellPower == 0) {
                spellRate = 0f;
                float specialSpeed = GetSpecialSpeed();
                if (specialSpeed > 0) {
                    meleeRate += 1 / GetSpecialSpeed();
                }
            } else {
                spellRate = 1 / GetSpecialSpeed();
            }
            float triggerRate = 1 / (meleeRate + spellRate);
            float uprate = pactEffect.GetAverageUptime(
                triggerRate,
                Utilities.GetWeightedSum(
                    Mommy.HitChance * CalcSpellCrit(),
                    spellRate,
                    CalcMeleeCrit(),
                    meleeRate),
                triggerRate,
                Mommy.Options.Duration);

            return uprate * buff;
        }

        protected float GetEmpowermentCooldown() {

            return 60f * (1f - .1f * Mommy.Talents.Nemesis);
        }
    }

    public class Felguard : Pet {

        public Felguard(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                1820f, // baseHealth,
                11f) { // healthPerStamina

            BaseMeleeDamage = (410f + 627f) / 2f;
            DamagePerAttackPower = .187f;

            SpecialCooldown = 6f;
        }

        protected override void FinalizeModifiers() {
            
            base.FinalizeModifiers();

            WarlockTalents talents = Mommy.Talents;
            float apBonus = 1.5f + talents.DemonicBrutality * .1f;
            if (Mommy.Talents.GlyphFelguard) {
                apBonus *= 1.2f;
            }
            Stats.BonusAttackPowerMultiplier
                = (1f + Stats.BonusAttackPowerMultiplier) * apBonus - 1f;
            TotalModifiers.AddMultiplicativeMultiplier(
                talents.MasterDemonologist * .01f);

            if (talents.DemonicEmpowerment > 0) {
                Stats.AddSpecialEffect(
                    new SpecialEffect(
                        Trigger.Use,
                        new Stats() { PhysicalHaste = .2f },
                        15f,
                        GetEmpowermentCooldown()));
            }
        }

        public override float GetSpecialDamage() {

            return CalcMeleeDamage(false, 72f);
        }
    }

    public class Felhunter : Pet {

        public Felhunter(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                1671f, // baseHealth,
                9.5f) { // healthPerStamina,

            BaseMeleeDamage = (277f + 416f) / 2f;
            DamagePerAttackPower = .12f;

            SpecialBaseDamage = (112f + 159f) / 2f;
            SpecialDamagePerSpellPower = .5f;
            SpecialCooldown = 6f - Mommy.Talents.ImprovedFelhunter * 2f;
        }

        protected override void FinalizeModifiers() {

            base.FinalizeModifiers();

            WarlockTalents talents = Mommy.Talents;

            // multipliers go into SpecialModifiers
            SpecialModifiers.AddMultiplicativeMultiplier(
                Stats.BonusShadowDamageMultiplier);
            if (Mommy.CastSpells.ContainsKey("Corruption")) {
                SpecialModifiers.AddAdditiveMultiplier(.15f);
            }
            if (Mommy.CastSpells.ContainsKey("Curse Of Agony")) {
                SpecialModifiers.AddAdditiveMultiplier(.15f);
            }
            if (Mommy.CastSpells.ContainsKey("Immolate")) {
                SpecialModifiers.AddAdditiveMultiplier(.15f);
            }
            if (Mommy.CastSpells.ContainsKey("Unstable Affliction")) {
                SpecialModifiers.AddAdditiveMultiplier(.15f);
            }
        }
    }

    public class Imp : Pet {

        public Imp(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                2877f, // baseHealth,
                8.4f) { // healthPerStamina,

            BaseStamina = 118f;
            BaseIntellect = 368f;
            ManaPerIntellect = 4.95f;
            BaseMana = 1052f;
            BaseSpellCrit = .01f;

            SpecialBaseDamage = (213f + 239f) / 2f;
            SpecialDamagePerSpellPower = .79f;
            SpecialCastTime = 2.5f - Mommy.Talents.DemonicPower * .25f;
        }

        public float GetCritsPerSec() {

            return Mommy.HitChance * CalcSpellCrit() / GetSpecialSpeed();
        }

        protected override void FinalizeModifiers() {

            base.FinalizeModifiers();

            WarlockTalents talents = Mommy.Talents;
            float demonologist = talents.MasterDemonologist * .01f;

            // crit goes into the stats object
            Stats.SpellCrit += demonologist;
            if (talents.DemonicEmpowerment > 0) {
                Stats.SpellCrit += .2f * 30f / GetEmpowermentCooldown();
            }

            // multipliers go into SpecialModifiers
            SpecialModifiers.AddMultiplicativeMultiplier(
                Stats.BonusFireDamageMultiplier);
            SpecialModifiers.AddAdditiveMultiplier(.1f * talents.ImprovedImp);
            SpecialModifiers.AddAdditiveMultiplier(.04f * talents.UnholyPower);
            SpecialModifiers.AddMultiplicativeMultiplier(demonologist);
            SpecialModifiers.AddAdditiveMultiplier(.1f * talents.EmpoweredImp);
            SpecialModifiers.AddCritBonusMultiplier(.2f * talents.Ruin);
            if (talents.GlyphImp) {
                SpecialModifiers.AddAdditiveMultiplier(.2f);
            }
        }
    }

    public class Succubus : Pet {
        public Succubus(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                1671f, // baseHealth,
                9f) { // healthPerStamina,

            BaseMeleeDamage = (363f + 546f) / 2f;
            DamagePerAttackPower = .157f;

            SpecialBaseDamage = 248f;
            SpecialDamagePerSpellPower = .5f;
            SpecialCooldown = 12f - Mommy.Talents.DemonicPower * 3f;
        }

        protected override void FinalizeModifiers() {

            base.FinalizeModifiers();

            WarlockTalents talents = Mommy.Talents;
            float demonologist = talents.MasterDemonologist * .01f;

            // crit goes into the stats object
            Stats.SpellCrit += demonologist;

            // multipliers go into SpecialModifiers
            SpecialModifiers.AddMultiplicativeMultiplier(
                Stats.BonusShadowDamageMultiplier);
            SpecialModifiers.AddAdditiveMultiplier(demonologist);
        }
    }

    public class Voidwalker : Pet {
        public Voidwalker(CharacterCalculationsWarlock mommy)
            : base(
                mommy,
                1671f, // baseHealth,
                11f) { // healthPerStamina,

            BaseMeleeDamage = (297f + 448f) / 2f;
            DamagePerAttackPower = .13f;
        }
    }

    //public class Doomguard : Pet {
    //    public Doomguard()
    //        : base("Doomguard") {
    //        BaseHealth = 18000;
    //        BaseMana = 3000;
    //    }
    //}

    //public class Infernal : Pet {
    //    public Infernal()
    //        : base("Infernal") {
    //        BaseStrength = 331;
    //        BaseAgility = 113;
    //        BaseStamina = 361;
    //        BaseIntellect = 65;
    //        BaseSpirit = 109;
    //        BaseHealth = 20300;
    //        BaseMana = 0;
    //    }
    //}
}
