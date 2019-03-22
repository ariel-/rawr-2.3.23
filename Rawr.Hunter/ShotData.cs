using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Rawr.Hunter
{
    public class ShotData
    {
        public const float GCD = 1.5f;

        public Shots Type = Shots.None;
        public float Damage = 0f;
        public float DamageNormal = 0f;
        public float DamageOnCrit = 0f;
        public float ManaCost = 0f;
        public float Cd = 0f;
        public float Duration = 0f;
        public bool  TriggersGCD = false;
        public bool  CanCrit = false;
        private float _critChance = 0f;
        public float CritChance {
            get { return CanCrit ? Math.Min(1f, Math.Max(0f, _critChance)) : 0f; }
            set { _critChance = value; }
        }

        public CombatTable AttackTable;

        public bool DoesntDoDamage = false;

        public bool FailReason_SteadyBefore       = false; // used in tooltip display
        public bool FailReason_SharedCooldownUsed = false; // used in tooltip display
        public bool FailReason_LackTalent         = false; // used in tooltip display
        public bool FailReason_RequiresMultiTargs = false; // used in tooltip display
        public bool FailReason_AlreadyUsed        = false; // for de-duping the priority list

        public float rotation_cooldown = 0f;
        public float time_used = 0f;
        public float ratio = 0f;
        public bool is_refreshed = false;
        public float Freq = 0f;
        public float DPS = 0f;
        public float MPS = 0f;

        // used in the 4 intermediate stages of calculations (start, inbet, lal, final)
        public float start_freq = 0f;
        protected float start_gcd_left = 0f;
        protected float start_gcd_needed = 0f;
        protected float start_gcd_used = 0f;

        public float inbet_freq = 0f;
        protected int sting_count = 0;

        public float lal_freq = 0;
        protected float lal_gcd_left = 0f;
        protected float lal_gcd_needed = 0f;
        protected float lal_gcd_used = 0f;

        public float final_freq = 0f; // the rotation test can set this
        protected float final_ratio = 0f;

        public float CritsPerSec = 0f;
        public float CritsRatio = 0f;
        public float CritsComposite = 0f;

        public ShotData(Shots aType, bool doesntdodamage, bool aCritProcs, bool aGcd
            //,Character character, Stats stats, CombatFactors cf, CalculationOptionsHunter opts
            ) {
            DoesntDoDamage = doesntdodamage;
            Type = aType;
            CanCrit = aCritProcs;
            TriggersGCD = aGcd;
            //AttackTable = new AttackTable(character, stats, cf, opts, false, false);
        }

        public void Dump(string label)
        {
            Debug.WriteLine(label + " damage = " + Damage);
            Debug.WriteLine(label + " mana cost = " + ManaCost);
            Debug.WriteLine(label + " cooldown = " + Cd);
            Debug.WriteLine(label + " duration = " + Duration);
            Debug.WriteLine(label + " critProcs = " + CanCrit);
            Debug.WriteLine(label + " gcd = " + TriggersGCD);
        }

        public void initializeTimings(ShotPriority Priority) {
            float CastLag = Priority.CalcOpts.Latency;

            // rotation cooldown
            rotation_cooldown = Cd;
            if (Type == Shots.SerpentSting) { rotation_cooldown = Duration; }

            // time used
            switch (Type) {
                case Shots.None:           { time_used = 0f;                                        break; }
                case Shots.ImmolationTrap: { time_used = Priority.CalcOpts.gcdsToLayImmoTrap * GCD; break; }
                case Shots.ExplosiveTrap:  { time_used = Priority.CalcOpts.gcdsToLayImmoTrap * GCD; break; }
                case Shots.FreezingTrap:   { time_used = Priority.CalcOpts.gcdsToLayImmoTrap * GCD; break; }
                case Shots.FrostTrap:      { time_used = Priority.CalcOpts.gcdsToLayImmoTrap * GCD; break; }
                case Shots.Volley:         { time_used = Priority.CalcOpts.gcdsToVolley      * GCD; break; }
                case Shots.SteadyShot:     { time_used = Cd < GCD ? GCD : Cd;                       break; }
                default:                   { time_used = TriggersGCD ? GCD : 0f;                    break; }
            }

            if (Type != Shots.None) {
                float lag = TriggersGCD ? CastLag : 0.001f; // non GCD = 1ms
                time_used += lag;
            }
        }

        public void calculateTimings(ShotPriority Priority, ShotData PrevShot)
        {
            float CastLag = Priority.CalcOpts.Latency;

            #region Starting Calculations

            start_freq = (rotation_cooldown > time_used) ? (float)Math.Ceiling((rotation_cooldown - time_used) / (GCD + CastLag)) * (GCD + CastLag) + time_used : GCD + CastLag;

            start_gcd_needed = (start_freq > 0) ? (time_used > start_freq ? 1f : time_used / start_freq) : 0f;

            if (Priority.chimeraRefreshesViper && Type == Shots.ViperSting) start_gcd_needed = 0f;
            if (Priority.chimeraRefreshesSerpent && Type == Shots.SerpentSting) start_gcd_needed = 0f;
            if (!Priority.useKillShot && Type == Shots.KillShot) start_gcd_needed = 0f;

            start_gcd_left = 1f;
            if (PrevShot != null) {
                start_gcd_left = PrevShot.start_gcd_left - PrevShot.start_gcd_used;
            }

            start_gcd_used = 1f - (start_gcd_left - start_gcd_needed);
            if (PrevShot != null) {
                start_gcd_used = start_gcd_left > start_gcd_needed ? start_gcd_needed : start_gcd_left;
            }

            //Debug.WriteLine("Start Freq is " + start_freq);
            //Debug.WriteLine("GCD Left is " + start_gcd_left);
            //Debug.WriteLine("GCD Needed is " + start_gcd_needed);
            //Debug.WriteLine("GCD Used is " + start_gcd_used);

            #endregion
            #region In-Between Calculations

            inbet_freq = (start_freq > 0 && start_gcd_used > 0) ? time_used / start_gcd_used : 0;

            if (PrevShot != null) {
                if (Type == Shots.SerpentSting || Type == Shots.ScorpidSting) {
                    sting_count = PrevShot.sting_count < 2 ? PrevShot.sting_count + 1 : PrevShot.sting_count;
                } else {
                    sting_count = PrevShot.sting_count == 2 ? 3 : PrevShot.sting_count;
                }
            } else {
                sting_count = (Type == Shots.SerpentSting || Type == Shots.ScorpidSting) ? 1 : 0;
            }

            //Debug.WriteLine("Pre-LAL freq = "+inbet_freq);
            //Debug.WriteLine("Pre-LAL sting count = " + sting_count);

            #endregion
            #region Lock-and-Load Calculations

            lal_freq = inbet_freq;
            if (Type == Shots.ExplosiveShot && Priority.LALExplosiveFrequency > 0f) lal_freq = Priority.LALExplosiveFrequency;
            if (Type == Shots.ArcaneShot && Priority.LALArcaneFrequency > 0f) lal_freq = Priority.LALArcaneFrequency;
            if (start_freq == 0f || sting_count == 2f) lal_freq = 0f;

            lal_gcd_left = 1;
            if (PrevShot != null) {
                lal_gcd_left = PrevShot.lal_gcd_left - PrevShot.lal_gcd_used;
            }

            lal_gcd_needed = lal_freq > 0f ? (time_used > lal_freq ? 1f : time_used / lal_freq) : 0f;
            if (!Priority.useKillShot && Type == Shots.KillShot) lal_gcd_needed = 0f;
            if (Priority.chimeraRefreshesSerpent && Type == Shots.SerpentSting) lal_gcd_needed = 0f;

            lal_gcd_used = 1f - (lal_gcd_left - lal_gcd_needed);
            if (PrevShot != null) {
                lal_gcd_used = lal_gcd_left > lal_gcd_needed ? lal_gcd_needed : lal_gcd_left;
            }

            #endregion

            // RotationTest may come in later and override this value,
            // hence the split-function that we call here
            final_freq = (lal_freq > 0 && lal_gcd_used > 0) ? time_used / lal_gcd_used : 0;

            finishCalculateTimings(Priority);
        }

        public void finishCalculateTimings(ShotPriority Priority)
        {
            final_ratio = final_freq > 0f ? (time_used > final_freq ? 1f : time_used / final_freq) : 0f;
            if (!Priority.useKillShot && Type == Shots.KillShot) final_ratio = 0;
            if (Priority.chimeraRefreshesSerpent && Type == Shots.SerpentSting) final_ratio = 0f;
            
            #region Output Calculations

            is_refreshed = false;
            ratio = 0;
            Freq = 0;

            if (Priority.chimeraRefreshesViper && Type == Shots.ViperSting) is_refreshed = true;
            if (Priority.chimeraRefreshesSerpent && Type == Shots.SerpentSting) is_refreshed = true;

            if (!is_refreshed) {
                ratio = final_ratio;
                Freq = final_freq;
            }

            CritsPerSec = (CanCrit && final_freq > 0f) ? 1f / final_freq : 0f;
            CritsRatio = (CanCrit) ? ratio : 0f;

            #endregion
        }

        public void setFrequency(ShotPriority Priority, float new_freq) {
            // called by the RotationTest after it calculates real frequencies
            start_freq = new_freq;
            final_freq = new_freq;
            finishCalculateTimings(Priority);
        }

        public void calculateComposites(ShotPriority Priority)
        {
            if (CritsRatio == 0)
            {
                CritsComposite = 0;
                return;
            }
            CritsComposite = CritsRatio > 0 ? CritChance * (CritsRatio / Priority.critsRatioSum) : 0;
        }

        public void calculateMPS(ShotPriority Priority)
        {
            float inbet_mps = inbet_freq > 0 ? ManaCost / inbet_freq : 0;

            float final_mps = final_freq > 0 ? ManaCost / final_freq : 0;

            if (Priority.chimeraRefreshesViper && Type == Shots.ViperSting) final_mps = 0;
            if (Priority.chimeraRefreshesSerpent && Type == Shots.SerpentSting) final_mps = 0;

            if (!Priority.CalcOpts.UseRotationTest && (Type == Shots.ExplosiveShot || Type == Shots.ArcaneShot)) {
                MPS = inbet_mps;
            } else {
                MPS = is_refreshed ? 0 : final_mps;
            }
        }

        public void calculateDPS(ShotPriority Priority)
        {
            #region In-Between Calculations

            float inbet_dps = inbet_freq > 0 ? Damage / inbet_freq : 0;

            if (Priority.chimeraRefreshesViper && Type == Shots.ViperSting) inbet_dps = Damage / start_freq;
            if (Priority.chimeraRefreshesSerpent && Type == Shots.SerpentSting) inbet_dps = Damage / start_freq;

            //Debug.WriteLine("InBet DPS is " + inbet_dps);

            #endregion
            #region Final/Actual Calculations

            float final_dps = final_freq > 0 ? Damage / final_freq : 0;
            if (Priority.chimeraRefreshesViper && Type == Shots.ViperSting) final_dps = Damage / rotation_cooldown;
            if (Priority.chimeraRefreshesSerpent && Type == Shots.SerpentSting) final_dps = Damage / rotation_cooldown;

            //Debug.WriteLine("Final DPS is " + final_dps);

            #endregion
            #region Output Calculations

            DPS = final_dps;
            if (Priority.chimeraRefreshesSerpent && Type == Shots.SerpentSting) {
                // immune to AspectOfViper
            } else {
                DPS *= (1f - Priority.viperDamagePenalty);
            }
            DPS *= (1f - Priority.NoManaPenalty);

            //Debug.WriteLine("DPS is " + dps);

            #endregion
        }

        public string GenTooltip() {
            string ret = "";
            if (!DoesntDoDamage) { ret += Damage.ToString("00000") + " : " + DPS.ToString("0000.00") + " : "; }
            ret += Freq.ToString("00.00") + "*";

            if (!DoesntDoDamage) { ret += "Damage: " + Damage.ToString("F2") + "\n"; }
            ret += "Mana: " + ManaCost.ToString("F2") + "\n";
            ret += "Cooldown: " + Cd.ToString("F2") + "\n";
            if (Duration > 0) { ret += "Duration: " + Duration.ToString("F2") + "\n"; }
            if (!DoesntDoDamage && CanCrit) { ret += "Crit Chance: " + CritChance.ToString("P2") + "\n"; }

            if (Freq > 0) {
                ret += "Rotation Freqency: " + Freq.ToString("F2") + "\n";
                if (!DoesntDoDamage) { ret += "Rotation DPS: " + DPS.ToString("F2") + "\n"; }
                ret += "Rotation MPS: " + MPS.ToString("F2");
            } else if (is_refreshed) {
                ret += "Rotation Freqency: " + Freq.ToString("F2") + " (Refreshed by Chimera)\n";
                if (!DoesntDoDamage) { ret += "Rotation DPS: " + DPS.ToString("F2") + "\n"; }
                ret += "Rotation MPS: " + MPS.ToString("F2") + " (Refreshed by Chimera)";
            } else {
                ret += FailReason_SharedCooldownUsed ? "Not being used in rotation:\n- Shares a cooldown with a higher priority shot" : 
                       FailReason_LackTalent         ? "Not being used in rotation:\n- You lack the needed talent" :
                       FailReason_SteadyBefore       ? "Not being used in rotation:\n- Steady shot has a higher priority" :
                       FailReason_RequiresMultiTargs ? "Not being used in rotation:\n- Multiple Targets isn't set" :
                       "(Not in rotation)";
            }

            return ret;
        }
    }
}
