using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Healadin
{
    public class Rotation
    {


        private Character _character;
        public Character Character
        {
            get { return _character; }
            set
            {
                _character = value;
                _talents = _character.PaladinTalents;
                
                _calcOpts = _character.CalculationOptions as CalculationOptionsHealadin;
                if (_calcOpts == null) _calcOpts = new CalculationOptionsHealadin();

#if (RAWR3)
                _bossOpts = _character.BossOptions;
                if (_bossOpts == null) _bossOpts = new BossOptions();
#endif
            }
        }

        private PaladinTalents _talents;
        public PaladinTalents Talents
        {
            get { return _talents; }
        }
        private CalculationOptionsHealadin _calcOpts;
        public CalculationOptionsHealadin CalcOpts
        {
            get { return _calcOpts; }
        }
#if (RAWR3)
        private BossOptions _bossOpts;
        public BossOptions BossOpts
        {
            get { return _bossOpts; }
        }
#endif
        private Stats _stats;
        public Stats Stats
        {
            get { return _stats; }
            set { _stats = value; }
        }

        public float DivinePleas { get; set; }
        public float FightLength { get; set; }
        public float FoLCasts { get; set; }

        private FlashOfLight fol;
        private HolyLight hl;
        private HolyShock hs;
        private SacredShield ss;
        private JudgementsOfThePure jotp;
        private BeaconOfLight bol;

        public Rotation(Character character, Stats stats)
        {
            Character = character;
#if (RAWR3)
            FightLength = BossOpts.BerserkTimer * 60f;
#else
            FightLength = CalcOpts.Length * 60f;
#endif
            Stats = stats;
            fol = new FlashOfLight(this);
            hl = new HolyLight(this);
            hs = new HolyShock(this);
            ss = new SacredShield(this);
            jotp = new JudgementsOfThePure(this, CalcOpts.Judgement);
            bol = new BeaconOfLight(this);
        }

        public float ManaPool(CharacterCalculationsHealadin calc)
        {
            DivinePleas = (float)Math.Ceiling((FightLength - 60f) / (60f * CalcOpts.DivinePlea));

            calc.ManaBase = Stats.Mana;
            calc.ManaLayOnHands = 1950 * ((Talents.GlyphOfDivinity ? 1 : 0) + (CalcOpts.LoHSelf ? 1 : 0)) * (Talents.GlyphOfDivinity ? 2f : 1f);
            calc.ManaArcaneTorrent = (Character.Race == CharacterRace.BloodElf ? Stats.Mana * .06f * (float)Math.Ceiling(FightLength / 120f - .25f) : 0);
            calc.ManaDivinePlea = Stats.Mana * .25f * DivinePleas;
            calc.ManaMp5 = FightLength * Stats.Mp5 / 5f;
            calc.ManaReplenishment = Stats.ManaRestoreFromMaxManaPerSecond * Stats.Mana * FightLength * CalcOpts.Replenishment;
            calc.ManaOther += Stats.ManaRestore;
            if (Stats.HighestStat > 0)
            {
                float greatnessMana = Stats.HighestStat * StatConversion.RATING_PER_MANA;
                calc.ManaReplenishment += Stats.ManaRestoreFromMaxManaPerSecond * FightLength * greatnessMana * CalcOpts.Replenishment; // Replenishment
                calc.ManaDivinePlea += DivinePleas * greatnessMana * .25f; // Divine Plea
            }
            return calc.ManaBase + calc.ManaDivinePlea + calc.ManaMp5 + calc.ManaOther +
                calc.ManaReplenishment + calc.ManaLayOnHands;
        }

        public static float GetHolyLightCastsPerSec(CharacterCalculationsHealadin calc)
        {
            return (calc.RotationHL / calc.HL.CastTime()) / calc.FightLength;
        }

        public static float GetHealingCastsPerSec(CharacterCalculationsHealadin calc)
        {
            return (calc.RotationHL / calc.HL.CastTime()
                + calc.RotationFoL / calc.FoL.CastTime()
                + calc.RotationHS / calc.HS.CastTime()) / calc.FightLength;
        }

        public static float GetSpellCastsPerSec(CharacterCalculationsHealadin calc)
        {
            return GetHealingCastsPerSec(calc)
                + (calc.RotationBoL / calc.BoL.CastTime()
                + calc.RotationJotP / calc.JotP.CastTime()
                + calc.RotationSS / calc.SS.CastTime()) / calc.FightLength;
        }

        public static float GetSpellCritsPerSec(CharacterCalculationsHealadin calc) { return GetHealingCritsPerSec(calc); }
        public static float GetHealingCritsPerSec(CharacterCalculationsHealadin calc)
        {
            return (calc.RotationHL / calc.HL.CastTime() * calc.HL.ChanceToCrit()
                + calc.RotationFoL / calc.FoL.CastTime() * calc.FoL.ChanceToCrit()
                + calc.RotationHS / calc.HS.CastTime() * calc.HS.ChanceToCrit()) / calc.FightLength;
        }

        public float CalculateFightHealing(CharacterCalculationsHealadin calc)
        {
            float fol_BaseHealed = 0f;

            #region Copying Stuff to Calc
            calc.FightLength = FightLength;

            calc.HL = hl;
            calc.FoL = fol;
            calc.HS = hs;
            calc.SS = ss;
            calc.JotP = jotp;
            calc.BoL = bol;

            calc.RotationJotP = jotp.Time();
            calc.UsageJotP = jotp.Usage();

            calc.RotationSS = ss.Time();
            calc.UsageSS = ss.Usage();
            calc.HealedSS = ss.TotalAborb();

            calc.RotationBoL = bol.Time();
            calc.UsageBoL = bol.Usage();

            calc.RotationHS = hs.Time();
            calc.HealedHS = hs.Healed();
            calc.UsageHS = hs.Usage();
            #endregion

            #region Divine Illumination
            if (Talents.DivineIllumination > 0)
            {
                DivineIllumination di = new DivineIllumination(this);
                calc.RotationHL += di.Time();
                calc.UsageHL += di.Usage();
                calc.HealedHL += di.Healed();
            }
            #endregion

            #region Divine Favor
            if (Talents.DivineFavor > 0)
            {
                DivineFavor df = new DivineFavor(this);
                calc.RotationHL += df.Time();
                calc.UsageHL += df.Usage();
                calc.HealedHL += df.Healed();
            }
            #endregion

            #region Infusion of Light

            float iol_hlcasts = 0;
            float iol_folcasts = 0;
            if (CalcOpts.InfusionOfLight)
            {
                float iol_count = hs.Casts() * hs.ChanceToCrit();

                HolyLight hl_iol = new HolyLight(this) { ExtraCritChance = .1f * Talents.InfusionOfLight };
                if (Stats.HolyLightCastTimeReductionFromHolyShock > 0)
                {
                    hl_iol.CastTimeReductionFromHolyShock = true;
                }

                iol_hlcasts = hs.Casts() * CalcOpts.IoLHolyLight * hs.ChanceToCrit();

                calc.UsageHL += iol_hlcasts * hl_iol.AverageCost();
                calc.RotationHL += iol_hlcasts * hl_iol.CastTime();
                calc.HealedHL += iol_hlcasts * hl_iol.AverageHealed();

                iol_folcasts = hs.Casts() * (1f - CalcOpts.IoLHolyLight) * hs.ChanceToCrit();
            }

            #endregion

            float remainingMana = calc.TotalMana = ManaPool(calc);
            remainingMana -= calc.UsageJotP + calc.UsageBoL + calc.UsageHS + calc.UsageHL + calc.UsageFoL + calc.UsageSS;

            float remainingTime = FightLength * CalcOpts.Activity;
            remainingTime -= calc.RotationJotP + calc.RotationBoL + calc.RotationSS + calc.RotationHS + calc.RotationFoL + calc.RotationHL;

            FoLCasts = 0f;
            if (remainingMana > 0)
            {
                if (Stats.HolyLightCastTimeReductionFromHolyShock > 0) // Calculations for Holy Light with cost reduction from Holy Shock
                {
                    // Get the Holy Light model for cast time reduction from Holy Shock
                    HolyLight hl_hs = new HolyLight(this) { CastTimeReductionFromHolyShock = true };

                    // Calculate how much time we have available to cast Holy Lights after Holy Shock
                    float hl_hs_time_available = Math.Min(remainingTime, Math.Max(0, (remainingMana - (remainingTime * fol.MPS())) / (hl_hs.MPS() - fol.MPS())));

                    // Calculate the maximum number of casts available with the cast time reduction
                    float hs_hlcasts = hs.Casts() - iol_hlcasts;

                    // Calculate the amount of time needed for all of the casts
                    float hl_hs_time_needed = hs_hlcasts * hl_hs.CastTime();

                    float manaCost = 0f;
                    float timeCost = 0f;
                    if (hl_hs_time_available > 0 && hl_hs_time_available < hl_hs_time_needed) // We have enough time to cast the Holy Lights needed, cast them all
                    {
                        // Calculate mana and time cost
                        manaCost = hs_hlcasts * hl_hs.AverageCost();
                        timeCost = hs_hlcasts * hl_hs.CastTime();

                        // Update Holy Light stats
                        calc.UsageHL += manaCost;
                        calc.RotationHL += timeCost;
                        calc.HealedHL += hs_hlcasts * hl_hs.AverageHealed();
                    }
                    else if (hl_hs_time_available >= remainingTime) // There's not enough time to cast the Holy Lights needed, so we'll only cast what we can
                    {
                        // Calculate mana and time cost
                        float remainingCasts = remainingTime / hl_hs.CastTime();
                        manaCost = remainingCasts * hl_hs.AverageCost();
                        timeCost = remainingCasts * hl_hs.CastTime();

                        // Update Holy Light stats
                        calc.UsageHL += manaCost;
                        calc.RotationHL += timeCost;
                        calc.HealedHL += remainingCasts * hl_hs.AverageHealed();
                    }
                    // Update remaining mana and remaining time
                    remainingMana -= manaCost;
                    remainingTime -= timeCost;
                }

                // Calculate how much time we have available to cast regular Holy Lights
                float hl_time_available = Math.Min(remainingTime, Math.Max(0, (remainingMana - (remainingTime * fol.MPS())) / (hl.MPS() - fol.MPS())));

                // The rest of the time will be for Flash of Light
                float fol_time = remainingTime - hl_time_available;

                if (hl_time_available == 0) // If we didn't have any time available for Holy Lights, check to see when we run out of mana while casting Flash of Light
                {
                    fol_time = Math.Min(remainingTime, remainingMana / fol.MPS());
                }
                else // If we do have time available for Holy Lights, update the Holy Light stats
                {
                    calc.HealedHL += hl.HPS() * hl_time_available;
                    calc.UsageHL += hl.MPS() * hl_time_available;
                    calc.RotationHL += hl_time_available;
                }

                // Calculate Flash of Light data
                if (iol_folcasts > 0) // We are using Infusion of Light, so we must calculate it differently than normal
                {
                    // Get the Flash of Light model for Infusion of Light
                    FlashOfLight fol_iol = new FlashOfLight(this) { InfusionOfLight = true };

                    // If there is less time remaining than it would take to cast all of the
                    // Infusion of Light procs, we are forced to drop some of them.  Note
                    // that this should *never* happen, but is here for a sanity check.
                    if (fol_time <= iol_folcasts * fol_iol.CastTime())
                    {
                        // Determine how many total casts we are going to be able to do
                        FoLCasts = fol_time / fol_iol.CastTime();

                        // Adding Flash of Light with Infusion of Light stats
                        calc.UsageFoL += FoLCasts * fol_iol.AverageCost();
                        calc.RotationFoL += FoLCasts * fol_iol.CastTime();
                        calc.HealedFoL += FoLCasts * fol_iol.AverageHealed();

                        fol_BaseHealed += FoLCasts * fol_iol.BaseAverageHealed();
                    }
                    else
                    {
                        // Adding Flash of Light with Infusion of Light
                        calc.UsageFoL += iol_folcasts * fol_iol.AverageCost();
                        calc.RotationFoL += iol_folcasts * fol_iol.CastTime();
                        calc.HealedFoL += iol_folcasts * fol_iol.AverageHealed();

                        fol_BaseHealed += iol_folcasts * fol_iol.BaseAverageHealed();

                        // Determine how much time we have left to cast Flash of Light without the procs
                        fol_time = fol_time - iol_folcasts * fol.CastTime();

                        // Determine how many total casts, with and without Infusion of Light
                        float remaining_folcasts = fol_time / fol.CastTime();
                        FoLCasts = iol_folcasts + remaining_folcasts;

                        // Adding Flash of Light stats
                        calc.RotationFoL += fol_time;
                        calc.UsageFoL += fol.MPS() * fol_time;
                        calc.HealedFoL += fol.HPS() * fol_time;

                        fol_BaseHealed += remaining_folcasts * fol.BaseAverageHealed();
                    }
                }
                else // We don't use Infusion of Light, so just calculate normal data for Flash of Light
                {
                    // Determine how many total casts, with and without Infusion of Light
                    FoLCasts = fol_time / fol.CastTime();

                    // Adding Flash of Light stats
                    calc.RotationFoL += fol_time;
                    calc.UsageFoL += fol.MPS() * fol_time;
                    calc.HealedFoL += fol.HPS() * fol_time;

                    fol_BaseHealed += FoLCasts * fol.BaseAverageHealed();
                }
            }

            calc.TotalHealed = calc.HealedFoL + calc.HealedHL + calc.HealedHS;

            if (Talents.BeaconOfLight > 0)
            {
                calc.TotalHealed += calc.HealedBoL = bol.HealingDone(fol_BaseHealed + calc.HealedHL + calc.HealedHS);
            }

            calc.TotalHealed += calc.HealedGHL = hl.GlyphOfHolyLight(calc.HealedHL);
            
            calc.HealedOther = Stats.Healed;
            calc.HealedOther += calc.TotalHealed * Stats.ShieldFromHealed;

            calc.TotalHealed += calc.HealedOther;
            calc.TotalHealed += calc.HealedSS;

            calc.AvgHPS = calc.TotalHealed / FightLength;
            calc.AvgHPM = calc.TotalHealed / calc.TotalMana;

            return calc.AvgHPS * (1f - CalcOpts.BurstScale);
        }

        public float CalculateBurstHealing(CharacterCalculationsHealadin calc)
        {
            return hl.HPS() * CalcOpts.BurstScale;
        }

    }
}
