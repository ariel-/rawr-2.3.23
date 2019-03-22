using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Healadin
{
    public abstract class Heal
    {
        private Rotation _rotation;
        public Rotation Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                _stats = _rotation.Stats;
                _talents = _rotation.Talents;
            }
        }

        private Stats _stats;
        protected Stats Stats { get { return _stats; } }

        private PaladinTalents _talents;
        protected PaladinTalents Talents { get { return _talents; } }

        public Heal(Rotation rotation)
        {
            Rotation = rotation;
        }

        public float HPS() { return AverageHealed() / CastTime(); }
        public float MPS() { return AverageCost() / CastTime(); }
        public float HPM() { return AverageHealed() / AverageCost(); }
        public float CastTime() { return (float)Math.Max(1f, (BaseCastTime - AbilityCastTimeReduction()) / (1f + Stats.SpellHaste));}

        public float AverageCost()
        {
            return (float)Math.Floor((BaseMana * (DivineIllumination ? 0.5f : 1f) - CostReduction())
                * (AbilityCostMultiplier() - (Talents.GlyphOfSealOfWisdom ? .05f : 0f)))
                - BaseMana * .06f * Talents.Illumination * ChanceToCrit();
        }

        public float AverageHealed()
        {
            return BaseHealed() * (1f - ChanceToCrit()) + CritHealed() * ChanceToCrit();
        }

        public float CritHealed() { return BaseHealed() * 1.5f * (1f + Stats.BonusCritHealMultiplier) * AbilityCritMultiplier(); }

        public float BaseHealed()
        {
            float heal = AbilityHealed();

            heal *= Talents.GlyphOfSealOfLight ? 1.05f : 1;
            heal *= 1f + Stats.HealingReceivedMultiplier;
            heal *= 1f - Rotation.DivinePleas * 15f / Rotation.FightLength * .5f;
            heal *= 1f + .01f * Talents.Divinity;
            heal *= AbilityHealedMultiplier();
            heal *= (1f + Talents.HealingLight * .04f);
            heal *= (1f + Stats.BonusHealingDoneMultiplier);
            if (DivineIllumination) {
                heal *= (1f + Stats.DivineIlluminationHealingMultiplier);
            }

            return heal;
        }

        public float ChanceToCrit()
        {
            return (float)Math.Max(0f, (float)Math.Min(1f, Stats.SpellCrit + AbilityCritChance() + ExtraCritChance + Talents.HolyPower * .01f));
        }
        public float CostReduction() { return Stats.SpellsManaReduction + AbilityCostReduction(); }

        public float ExtraCritChance { get; set; }
        public bool DivineIllumination { get; set; }

        protected abstract float AbilityHealed();
        protected virtual float AbilityCritChance() { return 0f; }
        protected virtual float AbilityCostReduction() { return 0f; }
        protected virtual float AbilityCastTimeReduction() { return 0f; }
        protected virtual float AbilityCostMultiplier() { return 1f; }
        protected virtual float AbilityHealedMultiplier() { return 1f; }
        protected virtual float AbilityCritMultiplier() { return 1f; }

        public abstract float BaseCastTime { get; }
        public abstract float BaseMana { get; }

        public override string ToString()
        {
            return string.Format("Average Heal: {0:N0}\nAverage Cost: {1:N0}\nHPS: {2:N0}\nHPM: {3:N2}\nCast Time: {4:N2} sec\nCrit Chance: {5:N2}%",
                AverageHealed(),
                AverageCost(),
                HPS(),
                HPM(),
                CastTime(),
                ChanceToCrit() * 100);
        }
    }

    public class FlashOfLight : Heal
    {
        public FlashOfLight(Rotation rotation)
            : base(rotation)
        { }

        public override float BaseCastTime { get { return InfusionOfLight ? 1.5f - 0.75f * Talents.InfusionOfLight : 1.5f; } }
        public override float BaseMana { get { return 307f; } }
        public bool InfusionOfLight { get; set; }

        protected override float AbilityCritChance()
        {
            return (Talents.GlyphOfFlashOfLight ? .05f : 0f) + Stats.FlashOfLightCrit;
        }

        protected override float AbilityHealedMultiplier()
        {
            return 1f + Stats.FlashOfLightMultiplier;
        }

        public float BaseAverageHealed()
        {
            return AverageHealed() * (1f - ssPercentage);
        }
        
        private float ssPercentage = 0f;
        private float folHoTUptime = 0f;
        protected override float AbilityHealed()
        {
            const float fol_coef = 1.5f / 3.5f * 66f / 35f * 1.25f;
            float baseHealed = (835.5f + (Stats.SpellPower + Stats.FlashOfLightSpellPower) * fol_coef);

            // Infusion of Light's Flash of Light HoT for Sacred Shield
            // Max HoT Uptime is 12 seconds.  If user is casting FoL more often than every 12 seconds, the HoT gets overridden
            // so we must calculate how long the HoT will be active.
            
            folHoTUptime = Rotation.FoLCasts == 0f ? 12f : Math.Min(Rotation.FightLength / (Rotation.FoLCasts * Rotation.CalcOpts.FoLOnTank), 12f);
            float ssHealed = baseHealed * (0.5f * Talents.InfusionOfLight) * (folHoTUptime / 12f) * (1f + Stats.FlashOfLightHoTMultiplier) * Rotation.CalcOpts.SSUptime * Rotation.CalcOpts.FoLOnTank;
            ssPercentage = ssHealed / (baseHealed + ssHealed);
            return baseHealed + ssHealed;
        }

        public override string ToString()
        {
            if (Talents.InfusionOfLight > 0 && Rotation.CalcOpts.SSUptime > 0)
            {
                float averageHealed = AverageHealed();
                float averageSSHealed = averageHealed * ssPercentage;
                return string.Format("Average Heal: {0:N0}\nAverage Base Heal: {6:N0}\nAverage Sacred Shield HoT Uptime: {7:N2} sec\nAverage Sacred Shield Total HoT: {8:N0}\nAverage Cost: {1:N0}\nHPS: {2:N0}\nHPM: {3:N2}\nCast Time: {4:N2} sec\nCrit Chance: {5:N2}%",
                    averageHealed,
                    AverageCost(),
                    HPS(),
                    HPM(),
                    CastTime(),
                    ChanceToCrit() * 100,
                    averageHealed - averageSSHealed,
                    folHoTUptime,
                    averageSSHealed);
            }
            else
            {
                return base.ToString();
            }
        }
    }

    public class HolyLight : Heal
    {
        public HolyLight(Rotation rotation)
            : base(rotation)
        { }

        public override float BaseCastTime { get { return CastTimeReductionFromHolyShock ? 2.5f - Stats.HolyLightCastTimeReductionFromHolyShock : 2.5f; } }
        public override float BaseMana { get { return 1274f; } }
        public bool CastTimeReductionFromHolyShock { get; set; }

        protected override float AbilityCritChance()
        {
            return Talents.SanctifiedLight * .02f + Stats.HolyLightCrit;
        }

        protected override float AbilityCostReduction()
        {
            return Stats.HolyLightManaCostReduction;
        }

        protected override float AbilityCostMultiplier()
        {
            return 1f - Stats.HolyLightPercentManaReduction;
        }

        protected override float AbilityCastTimeReduction()
        {
            return .5f / 3 * Talents.LightsGrace;
        }

        protected override float AbilityHealed()
        {
            const float hl_coef = 2.5f / 3.5f * 66f / 35f * 1.25f;
            return (5166f + (Stats.HolyLightSpellPower + Stats.SpellPower) * hl_coef);
        }

        public float GlyphOfHolyLight(float hlHealed)
        {
            return hlHealed * (Talents.GlyphOfHolyLight ? .1f * Rotation.CalcOpts.GHL_Targets : 0f);
        }

    }

    public class HolyShock : Heal
    {
        public HolyShock(Rotation rotation)
            : base(rotation)
        { }

        public override float BaseCastTime { get { return 1.5f; } }
        public override float BaseMana { get { return 790f; } }

        protected override float AbilityCritChance()
        {
            return Talents.SanctifiedLight * .02f + Stats.HolyShockCrit;
        }

        protected override float AbilityCostMultiplier()
        {
            return 1f - .02f * Talents.Benediction;
        }

        protected override float AbilityHealed()
        {
            const float hs_coef = 1.5f / 3.5f * 66f / 35f;
            return (2500f + Stats.SpellPower * hs_coef);
        }

        public float Usage()
        {
            return Casts() * AverageCost();
        }

        public float Casts()
        {
            return Rotation.FightLength * Rotation.CalcOpts.HolyShock / Cooldown();
        }

        public float Time()
        {
            return Casts() * CastTime();
        }

        public float Healed()
        {
            return Casts() * AverageHealed();
        }

        public float Cooldown()
        {
            return Talents.GlyphOfHolyShock ? 5f : 6f;
        }

        protected override float AbilityCritMultiplier()
        {
             return 1f + Stats.HolyShockHoTOnCrit;
        }

    }

    public abstract class Spell
    {

        private Rotation _rotation;
        public Rotation Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                _stats = _rotation.Stats;
                _talents = _rotation.Talents;
            }
        }

        private Stats _stats;
        protected Stats Stats { get { return _stats; } }

        private PaladinTalents _talents;
        protected PaladinTalents Talents { get { return _talents; } }

        public Spell(Rotation rotation)
        {
            Rotation = rotation;
        }

        public float Uptime { get; set; }
        public float Duration { get; set; }
        public float BaseCost { get; set; }

        public float Cost()
        {
            return (BaseCost - Stats.SpellsManaReduction) * (1f - .02f * Talents.Benediction);
        }

        public float Casts()
        {
            return (float)Math.Ceiling(Uptime / Duration);
        }

        public virtual float CastTime()
        {
            return (float)Math.Max(1f, 1.5f / (1f + Stats.SpellHaste));
        }

        public virtual float Time()
        {
            return Casts() * CastTime();
        }

        public virtual float Usage()
        {
            return Casts() * Cost();
        }

    }

    public class SacredShield : Spell
    {
        public SacredShield(Rotation rotation)
            : base(rotation)
        {
            Duration = 30f + Talents.DivineGuardian * 15f;
            Uptime = Rotation.FightLength * Rotation.CalcOpts.SSUptime;
            BaseCost = 527f;         
        }

        public float ICD()
        {
            return 6f - Stats.SacredShieldICDReduction;
        }

        public float ProcAbsorb()
        {
            return (500f + Stats.SpellPower * (Stats.SacredShieldICDReduction > 0 ? 0.85f : 0.75f) ) * (1f + Talents.DivineGuardian * .1f);
        }

        public float TotalAborb()
        {
            return Uptime / ICD() * ProcAbsorb();
        }

        public float CastAborb()
        {
            return Duration / ICD() * ProcAbsorb();
        }

        public float HPM()
        {
            return ProcAbsorb() * Duration / ICD() / Cost(); 
        }

        public float HPS()
        {
            return TotalAborb() / Time();
        }

        public override string ToString()
        {
            return string.Format("Proc Absorb: {0:N0}\nCast Absorb: {1:N0}\nAverage Cost: {2:N0}\nHPS: {3:N0}\nHPM: {4:N2}\n",
                ProcAbsorb(),
                CastAborb(),
                Cost(),
                HPS(),
                HPM());
        }

    }

    public class BeaconOfLight : Spell
    {
        public BeaconOfLight(Rotation rotation)
            : base(rotation)
        {
            Duration = 60f + (Talents.GlyphOfBeaconOfLight ? 30f : 0f);
            Uptime = Rotation.FightLength * Rotation.CalcOpts.BoLUp;
            BaseCost = 1537f;
        }

        public float HealingDone(float procableHealing)
        {
            return procableHealing * Rotation.CalcOpts.BoLUp;
        }

    }

    public class JudgementsOfThePure : Spell
    {
        public JudgementsOfThePure(Rotation rotation, bool MaintainDebuff)
            : base(rotation)
        {
            Duration = MaintainDebuff ? (20f + Stats.BonusJudgementDuration) : 60f;
            Uptime = Rotation.CalcOpts.JotP ? Rotation.FightLength : 0f;
            BaseCost = 219f;
        }

        public override float CastTime()
        {
            return 1.5f;
        }

    }

    public class DivineIllumination : Spell
    {

        public HolyLight HL_DI { get; set; }

        public DivineIllumination(Rotation rotation)
            : base(rotation)
        {
            Duration = 180f;
            Uptime = Rotation.FightLength;
            BaseCost = 0f;
            HL_DI = new HolyLight(rotation) { DivineIllumination = true };
        }

        public float Healed()
        {
            return HL_DI.HPS() * Time();
        }

        public override float Time()
        {
            return 15f * Casts() * Rotation.CalcOpts.Activity;
        }

        public override float Usage()
        {
            return Time() * HL_DI.MPS();
        }

    }

    public class DivineFavor : Spell
    {

        public HolyLight HL_DF { get; set; }

        public DivineFavor(Rotation rotation)
            : base(rotation)
        {
            Duration = 120f;
            Uptime = Rotation.FightLength;
            BaseCost = 130f;
            HL_DF = new HolyLight(rotation) { ExtraCritChance = 1f };
        }

        public float Healed()
        {
            return HL_DF.AverageHealed() * Casts();
        }

        public override float Time()
        {
            return HL_DF.CastTime() * Casts();
        }

        public override float Usage()
        {
            return Casts() * (Cost() + HL_DF.AverageCost());
        }

    }
}
