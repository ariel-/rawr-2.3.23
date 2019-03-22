using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Hunter.Skills
{
    #region Shots
    public class ExplosiveShot : Ability
    {
        /// <summary>
        /// <b>Explosive Shot</b>, 10% Base Mana, 5-35yd, Instant, 6 sec Cd
        /// <para>You fire an explosive charge into the enemy target, dealing
        /// [RAP * 0.14 + 386] - [RAP * 0.14 + 464] Fire Damage. The charge will
        /// blast the target every second for an additional 2 sec.</para>
        /// </summary>
        /// <TalentsAffecting>Explosive Shot (Requires Talent)</TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Explosive Shot [+4% crit chance]</GlyphsAffecting>
        public ExplosiveShot(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;
            //
            Name = "Explosive Shot";
            //AbilIterater = (int)CalculationOptionsHunter.Maintenances.MortalStrike_;
            ReqTalent = true;
            Talent2ChksValue = Talents.ExplosiveShot;
            ReqRangedWeap = true;
            ReqSkillsRange = true;
            //Targets += StatS.BonusTargets;
            Cd = 6f; // In Seconds
            ManaCostisPerc = true;
            ManaCost = 0.10f; // 10% of base mana, but higher shots are 7% so we need to verify
            DamageBase = (StatS.RangedAttackPower * 0.14f + ((386f + 464f) / 2f)) * 3f;
            BonusCritChance = Talents.GlyphOfExplosiveShot ? 0.04f : 0f;
            //
            Initialize();
        }
    }
    public class ChimeraShot : Ability
    {
        /// <summary>
        /// <b>Chimera Shot</b>, 12% Base Mana, 5-35yd, Instant, 10 sec Cd
        /// <para>You deal 125% weapon damage, refreshing the current Sting on your
        /// target and triggering an effect:</para>
        /// <para>Serpent Sting - Instantly deals 40% of the Damage done by your Serpent Sting.</para>
        /// <para>Viper Sting - Instantly restores mana to you equal to 60% of the total amount drained by your Viper Sting.</para>
        /// <para>Scorpid Sting - Attempts to Disarm the target for 10 sec. This affect cannot occur more than once per 1 minute.</para>
        /// </summary>
        /// <TalentsAffecting>Chimera Shot (Requires Talent)</TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Chimera Shot [-1 sec Cd]</GlyphsAffecting>
        public ChimeraShot(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;
            //
            Name = "Chimera Shot";
            //AbilIterater = (int)CalculationOptionsHunter.Maintenances.MortalStrike_;
            ReqTalent = true;
            Talent2ChksValue = Talents.ChimeraShot;
            ReqRangedWeap = true;
            ReqSkillsRange = true;
            //Targets += StatS.BonusTargets;
            Cd = 10f - (Talents.GlyphOfChimeraShot ? 1f : 0f); // In Seconds
            ManaCostisPerc = true;
            ManaCost = 0.12f; // 12% of base mana
            DamageBase = combatFactors.AvgRwWeaponDmgUnhasted * 1.25f;
            //
            Initialize();
        }
    }
    public class SteadyShot : Ability {
        /// <summary>
        /// <b>Steady Shot</b>, 5% Base Mana, 5-35yd, 1.5 sec cast, 10 sec Cd
        /// <para>A steady shot that causes unmodified weapon damage, plus ammo,
        /// plus [RAP * 0.1 + 252]. Causes an additional 175 against Dazed targets.</para>
        /// </summary>
        /// <TalentsAffecting>
        /// Improved Steady Shot [+3*5% chance to increase damage of next Aimed/Arcane/Chim
        /// shot by 15% and reduc Mana cost of next Aimed/Arcane/Chim shot by 20%]
        /// </TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Steady Shot [+10% DMG w/ Serpent Sting active]</GlyphsAffecting>
        public SteadyShot(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;
            //
            Name = "Steady Shot";
            //AbilIterater = (int)CalculationOptionsHunter.Maintenances.MortalStrike_;
            ReqRangedWeap = true;
            ReqSkillsRange = true;
            //Targets += StatS.BonusTargets;
            ManaCostisPerc = true;
            ManaCost = 0.05f; // 5% of base mana
            DamageBase = combatFactors.AvgRwWeaponDmgUnhasted
                       + (Char.Projectile.Item.MinDamage + Char.Projectile.Item.MaxDamage) / 2f
                       + StatS.RangedAttackPower * 0.1f + 252f;
            DamageBonus = 1f + (true ? 0.10f : 0f); // this should be a check for Serpent Sting on the target
            //
            Initialize();
        }
    }
    public class AimedShot : Ability {
        /// <summary>
        /// <b>Aimed Shot</b>, 8% Base Mana, 5-35yd, Instant, 10 sec Cd
        /// <para>An Aimed show that increases ranged damage by 408 and reduces
        /// healing done to that target by 50%. Lasts 10 sec.</para>
        /// </summary>
        /// <TalentsAffecting>Aimed Shot (Requires Talent)</TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Aimed Shot [-2 sec Cd]</GlyphsAffecting>
        public AimedShot(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;
            //
            Name = "Aimed Shot";
            //AbilIterater = (int)CalculationOptionsHunter.Maintenances.MortalStrike_;
            ReqTalent = true;
            Talent2ChksValue = Talents.AimedShot;
            ReqRangedWeap = true;
            ReqSkillsRange = true;
            //Targets += StatS.BonusTargets;
            Cd = 10f - (Talents.GlyphOfAimedShot ? 2f : 0f); // In Seconds
            Duration = 10f;
            ManaCostisPerc = true;
            ManaCost = 0.08f; // 8% of base mana
            DamageBase = combatFactors.NormalizedRwWeaponDmg + 408;
            //
            Initialize();
        }
    }
    public class MultiShot : Ability {
        /// <summary>
        /// <b>Multi-Shot</b>, 9% Base Mana, 5-35yd, Cast Time: Attack Speed, 10 sec Cd
        /// <para>Fires several missiles, hitting 3 targets for an additioal 408 damage.</para>
        /// </summary>
        /// <TalentsAffecting></TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Multi-Shot [-1 sec Cd]</GlyphsAffecting>
        public MultiShot(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;
            //
            Name = "Multi-Shot";
            //AbilIterater = (int)CalculationOptionsHunter.Maintenances.MortalStrike_;
            ReqRangedWeap = true;
            ReqSkillsRange = true;
            Targets = 3f;
            Cd = 10f - (Talents.GlyphOfMultiShot ? 1f : 0f); // In Seconds
            CastTime = cf.RWSpeed;
            ManaCostisPerc = true;
            ManaCost = 0.09f; // 9% of base mana
            DamageBase = cf.AvgRwWeaponDmgUnhasted + 408f;
            //
            Initialize();
        }
    }
    public class ArcaneShot : Ability {
        /// <summary>
        /// <b>Arcane Shot</b>, 5% Base Mana, 5-35yd, Instant, 6 sec Cd
        /// <para>An Instant shot that causes [RAP * 0.15 + 492] Arcane Damage</para>
        /// </summary>
        /// <TalentsAffecting>Aimed Shot (Requires Talent)</TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Arcane Shot [20% ManaCost refunded if a sting is active]</GlyphsAffecting>
        public ArcaneShot(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;
            //
            Name = "Arcane Shot";
            //AbilIterater = (int)CalculationOptionsHunter.Maintenances.MortalStrike_;
            ReqTalent = true;
            Talent2ChksValue = Talents.AimedShot;
            ReqRangedWeap = true;
            ReqSkillsRange = true;
            //Targets += StatS.BonusTargets;
            Cd = 6f; // In Seconds
            Duration = 10f;
            ManaCostisPerc = true;
            ManaCost = 0.05f; // 5% of base mana
            // -20% with a sting active
            DamageBase = StatS.RangedAttackPower * 0.15f + 492f;
            DamageBonus = 1f + Talents.ImprovedArcaneShot * 0.05f;
            //
            Initialize();
        }
    }
    public class KillShot : Ability {
        /// <summary>
        /// <b>Kill Shot</b>, 7% Base Mana, 45yd, Instant, 15 sec Cd
        /// <para>You attempt to finish the wounded target off, firing a long
        /// range attack dealing 200% weapon damage plus [RAP * 0.40 + 650].
        /// Kill Shot can only be used on enemies that have 20% or less health.</para>
        /// </summary>
        /// <TalentsAffecting></TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Kill Shot [-6 sec Cd]</GlyphsAffecting>
        public KillShot(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;
            //
            Name = "Kill Shot";
            //AbilIterater = (int)CalculationOptionsHunter.Maintenances.MortalStrike_;
            ReqTalent = true;
            Talent2ChksValue = Talents.AimedShot;
            ReqRangedWeap = true;
            ReqSkillsRange = true;
            //Targets += StatS.BonusTargets;
            Cd = 15f - (Talents.GlyphOfKillShot ? 6f : 0f); // In Seconds
            ManaCostisPerc = true;
            ManaCost = 0.07f; // 7% of base mana
            DamageBase = cf.AvgRwWeaponDmgUnhasted + StatS.RangedAttackPower * 0.40f + 650f;
            //
            Initialize();
        }
    }
    public class SilencingShot : Ability
    {
        /// <summary>
        /// <b>Silencing Shot</b>, 6% Base Mana, 5-35yd, Instant, 20 sec Cd
        /// <para>A shot that deals 50% weapon damage and Silences the target for 3 sec.</para>
        /// </summary>
        /// <TalentsAffecting></TalentsAffecting>
        /// <GlyphsAffecting></GlyphsAffecting>
        public SilencingShot(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;
            //
            Name = "Silencing Shot";
            //AbilIterater = (int)CalculationOptionsHunter.Maintenances.MortalStrike_;
            ReqTalent = true;
            Talent2ChksValue = Talents.ExplosiveShot;
            ReqRangedWeap = true;
            ReqSkillsRange = true;
            //Targets += StatS.BonusTargets;
            Cd = 20f; // In Seconds
            ManaCostisPerc = true;
            ManaCost = 0.06f; // 6% of base mana
            DamageBase = cf.AvgRwWeaponDmgUnhasted * 0.50f;
            //
            Initialize();
        }
    }
    public class Volley : Ability { }
    #endregion

    #region DoTs
    public class BlackArrowDoT : DoT
    {
        /// <summary>
        /// <b>Black Arrow</b>, 6% Base Mana, 5-35yd, Instant, 30 sec Cd
        /// <para>Fires a Black Arrow at the target, increasing all damage done by you
        /// to the target by 6%, and dealing [RAP * 0.1 + 2765] Shadow Damage over 15
        /// sec. Black Arrow shares a cooldown with Trap spells.</para>
        /// </summary>
        /// <TalentsAffecting>Black Arrow (Requires Talent)</TalentsAffecting>
        /// <GlyphsAffecting></GlyphsAffecting>
        public BlackArrowDoT(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;
            //
            Name = "Black Arrow";
            //AbilIterater = (int)CalculationOptionsHunter.Maintenances.MortalStrike_;
            ReqTalent = true;
            Talent2ChksValue = Talents.BlackArrow;
            ReqRangedWeap = true;
            ReqSkillsRange = true;
            //Targets += StatS.BonusTargets;
            Cd = 30f; // In Seconds
            Duration = 15f;
            TimeBtwnTicks = 1f; // In Seconds
            ManaCostisPerc = true;
            ManaCost = 0.06f; // 6% of base mana
            DamageBase = StatS.RangedAttackPower * 0.10f + 2765f;
            //
            Initialize();
        }
        public override float TickSize {
            get {
                if (!Validated) { return 0f; }

                float damage = Damage;
                /*float DmgBonusBase = (StatS.AttackPower * combatFactors._c_rwItemSpeed) / 14f
                                   + (combatFactors.RW.MaxDamage + combatFactors.RW.MinDamage) / 2f;
                float DmgBonusU75 = 0.75f * 1.00f;
                float DmgBonusO75 = 0.25f * 1.35f;
                float DmgMod = (1f + StatS.BonusBleedDamageMultiplier)
                             * (1f + StatS.BonusDamageMultiplier)
                             * DamageBonus;
                float GlyphMOD = Talents.GlyphOfRending ? 7f / 5f : 1f;

                float damageUnder75 = (DamageBase + DmgBonusBase) * DmgBonusU75;
                float damageOver75 = (DamageBase + DmgBonusBase) * DmgBonusO75;

                float TheDamage = (damageUnder75 + damageOver75) * DmgMod;

                float TickSize = (TheDamage * GlyphMOD) / NumTicks;*/

                return Damage * DamageBonus * (1f + StatS.BonusDamageMultiplier) * (1f + StatS.BonusShadowDamageMultiplier) / NumTicks;
            }
        }
        public override float GetDPS(float acts)
        {
            float dmgonuse = TickSize;
            float numticks = NumTicks * (acts /*- addMisses - addDodges - addParrys*/);
            float result = GetDmgOverTickingTime(acts) / FightDuration;
            return result;
        }
    }
    public class BlackArrowBuff : BuffEffect
    {
        /// <summary>
        /// <b>Black Arrow</b>, 6% Base Mana, 5-35yd, Instant, 30 sec Cd
        /// <para>Fires a Black Arrow at the target, increasing all damage done by you
        /// to the target by 6%, and dealing [RAP * 0.1 + 2765] Shadow Damage over 15
        /// sec. Black Arrow shares a cooldown with Trap spells.</para>
        /// </summary>
        /// <TalentsAffecting>Black Arrow (Requires Talent)</TalentsAffecting>
        /// <GlyphsAffecting></GlyphsAffecting>
        public BlackArrowBuff(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;
            //
            Name = "Black Arrow";
            //AbilIterater = (int)CalculationOptionsHunter.Maintenances.MortalStrike_;
            ReqTalent = true;
            Talent2ChksValue = Talents.BlackArrow;
            ReqRangedWeap = true;
            ReqSkillsRange = true;
            //Targets += StatS.BonusTargets;
            Cd = 30f; // In Seconds
            ManaCostisPerc = true;
            ManaCost = 0.06f; // 6% of base mana
            /*Effect = new SpecialEffect(Trigger.Use,
                new Stats() { BonusDamageMultiplier = 0.06f, },
                Duration, Cd);*/
            //
            Initialize();
        }
    }
    public class PiercingShots : DoT
    {
        /// <summary>
        /// <b>Piercing Shots</b>
        /// <para>Your critical Aimed, Steady and Chimera Shots cause the
        /// target to bleed for [Pts*10]% of the damage dealt over 8 sec.</para>
        /// </summary>
        /// <TalentsAffecting>Piercing Shots (Requires Talent)</TalentsAffecting>
        /// <GlyphsAffecting></GlyphsAffecting>
        public PiercingShots(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;
            //
            Name = "Piercing Shots";
            //AbilIterater = (int)CalculationOptionsHunter.Maintenances.MortalStrike_;
            ReqTalent = true;
            Talent2ChksValue = Talents.PiercingShots;
            ReqRangedWeap = true;
            ReqSkillsRange = true;
            //Targets += StatS.BonusTargets;
            //Cd = 30f; // In Seconds
            Duration = 8f;
            TimeBtwnTicks = 1f; // In Seconds
            ManaCostisPerc = true;
            ManaCost = 0.06f; // 6% of base mana
            DamageBase = StatS.RangedAttackPower * 0.10f + 2765f;
            //
            Initialize();
        }
        public override float TickSize
        {
            get
            {
                if (!Validated) { return 0f; }

                float damage = Damage;
                /*float DmgBonusBase = (StatS.AttackPower * combatFactors._c_rwItemSpeed) / 14f
                                   + (combatFactors.RW.MaxDamage + combatFactors.RW.MinDamage) / 2f;
                float DmgBonusU75 = 0.75f * 1.00f;
                float DmgBonusO75 = 0.25f * 1.35f;
                float DmgMod = (1f + StatS.BonusBleedDamageMultiplier)
                             * (1f + StatS.BonusDamageMultiplier)
                             * DamageBonus;
                float GlyphMOD = Talents.GlyphOfRending ? 7f / 5f : 1f;

                float damageUnder75 = (DamageBase + DmgBonusBase) * DmgBonusU75;
                float damageOver75 = (DamageBase + DmgBonusBase) * DmgBonusO75;

                float TheDamage = (damageUnder75 + damageOver75) * DmgMod;

                float TickSize = (TheDamage * GlyphMOD) / NumTicks;*/

                return Damage * DamageBonus * (1f + StatS.BonusDamageMultiplier) * (1f + StatS.BonusShadowDamageMultiplier) / NumTicks;
            }
        }
        public override float GetDPS(float acts)
        {
            float dmgonuse = TickSize;
            float numticks = NumTicks * (acts /*- addMisses - addDodges - addParrys*/);
            float result = GetDmgOverTickingTime(acts) / FightDuration;
            return result;
        }
    }
    #endregion

    #region Stings
    public class SerpentSting : DoT {
        /// <summary>
        /// <b>Serpent Sting</b>, 9% Base Mana, 5-35yd, Instant, No Cd
        /// <para>Stings the target, causing [RAP * 0.2 + 1210] Nature
        /// damage over 15 sec. Only one Sting per Hunter can be active
        /// on any one target.</para>
        /// </summary>
        /// <TalentsAffecting></TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Serpent Sting [+6 sec Dur]</GlyphsAffecting>
        public SerpentSting(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;
            //
            Name = "Serpent Sting";
            //AbilIterater = (int)CalculationOptionsHunter.Maintenances.MortalStrike_;
            ReqRangedWeap = true;
            ReqSkillsRange = true;
            //Targets += StatS.BonusTargets;
            TimeBtwnTicks = 3f; // In Seconds
            Duration = 15f + (Talents.GlyphOfSerpentSting ? 6f : 0f);
            ManaCostisPerc = true;
            ManaCost = 0.09f; // 9% of base mana
            DamageBase = StatS.RangedAttackPower * 0.2f + 1210f;
            //
            Initialize();
        }
        public override float TickSize
        {
            get
            {
                if (!Validated) { return 0f; }

                float damage = Damage;
                /*float DmgBonusBase = (StatS.AttackPower * combatFactors._c_rwItemSpeed) / 14f
                                   + (combatFactors.RW.MaxDamage + combatFactors.RW.MinDamage) / 2f;
                float DmgBonusU75 = 0.75f * 1.00f;
                float DmgBonusO75 = 0.25f * 1.35f;
                float DmgMod = (1f + StatS.BonusBleedDamageMultiplier)
                             * (1f + StatS.BonusDamageMultiplier)
                             * DamageBonus;
                float GlyphMOD = Talents.GlyphOfRending ? 7f / 5f : 1f;

                float damageUnder75 = (DamageBase + DmgBonusBase) * DmgBonusU75;
                float damageOver75 = (DamageBase + DmgBonusBase) * DmgBonusO75;

                float TheDamage = (damageUnder75 + damageOver75) * DmgMod;

                float TickSize = (TheDamage * GlyphMOD) / NumTicks;*/

                return Damage * DamageBonus * (1f + StatS.BonusDamageMultiplier) * (1f + StatS.BonusShadowDamageMultiplier) / NumTicks;
            }
        }
        public override float GetDPS(float acts)
        {
            float dmgonuse = TickSize;
            float numticks = NumTicks * (acts /*- addMisses - addDodges - addParrys*/);
            float result = GetDmgOverTickingTime(acts) / FightDuration;
            return result;
        }
    }
    public class ChimeraShot_Serpent : Ability { }
    public class ScorpidSting : Ability {
        /// <summary>
        /// <b>Scorpid Sting</b>, 11% Base Mana, 5-35yd, Instant, No Cd
        /// <para>Stings the target, reducing chance to hit with melee and
        /// ranged attacks by 3% for 20 sec. Only one Sting per Hunter can
        /// be active on any one target.</para>
        /// </summary>
        /// <TalentsAffecting></TalentsAffecting>
        /// <GlyphsAffecting></GlyphsAffecting>
        public ScorpidSting(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;
            //
            Name = "Scorpid Sting";
            //AbilIterater = (int)CalculationOptionsHunter.Maintenances.MortalStrike_;
            ReqRangedWeap = true;
            ReqSkillsRange = true;
            //Targets += StatS.BonusTargets;
            ManaCostisPerc = true;
            ManaCost = 0.11f; // 11% of base mana
            //
            Initialize();
        }
    }
    public class ViperSting : Ability {
        /// <summary>
        /// <b>Viper Sting</b>, 8% Base Mana, 5-35yd, Instant, 15 sec Cd
        /// <para>Stings the target, draining 4% mana over 8 sec (up to a
        /// maximum of 8% of the caster's maximum mana), and energizing
        /// the Hunter equal to 300% of the amount drained. Only one
        /// Sting per Hunter can be active on any one target.</para>
        /// </summary>
        /// <TalentsAffecting></TalentsAffecting>
        /// <GlyphsAffecting></GlyphsAffecting>
        public ViperSting(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;
            //
            Name = "Viper Sting";
            //AbilIterater = (int)CalculationOptionsHunter.Maintenances.MortalStrike_;
            ReqRangedWeap = true;
            ReqSkillsRange = true;
            //Targets += StatS.BonusTargets;
            Cd = 15f; // In Seconds
            ManaCostisPerc = true;
            ManaCost = 0.08f; // 8% of base mana
            //
            Initialize();
        }
    }
    #endregion

    #region Traps
    public class ImmolationTrap : Ability { }
    public class ExplosiveTrap : Ability { }
    public class FreezingTrap : Ability { }
    public class FrostTrap : Ability { }
    #endregion

    #region Special Abilities
    public class Readiness : Ability { }
    #endregion

    #region Buff Effects
    public class BestialWrath : BuffEffect { }
    public class RapidFire : BuffEffect
    {
        /// <summary>
        /// <b>Rapid Fire</b>, 3% Base Mana, , , Instant, 5 min Cd
        /// <para>Increases ranged attack speed by 40% for 15 sec.</para>
        /// </summary>
        /// <TalentsAffecting></TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Rapid Fire [+8% Haste Bonus]</GlyphsAffecting>
        public RapidFire(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;
            //
            Name = "Rapid Fire";
            //AbilIterater = (int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.DeathWish_;
            Cd = (5f - Talents.RapidKilling) * 60f; // In Seconds
            Duration = 15f;
            ManaCostisPerc = true;
            ManaCost = 0.03f;
            UseHitTable = false;
            //
            /*Effect = new SpecialEffect(Trigger.Use,
                    new Stats() { RangedHaste = 0.40f + (Talents.GlyphOfRapidFire ? 0.08f : 0f), },
                    Duration, Cd);
            if (Talents.RapidRecuperation > 0) {
                Effect2 = new SpecialEffect(Trigger.Use,
                        new Stats() { ManaRestore = StatS.Mana * (0.02f * Talents.RapidRecuperation) * Duration / 3f, },
                        Duration, Cd);
            } else { Effect2 = null; }*/
            //
            Initialize();
        }
    }
    #endregion
}
