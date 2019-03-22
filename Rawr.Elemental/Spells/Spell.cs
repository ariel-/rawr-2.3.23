using System;

namespace Rawr.Elemental.Spells
{
    public abstract class Spell
    {
        protected float baseMinDamage = 0f;
        protected float baseMaxDamage = 0f;
        protected float baseCastTime = 0f;
        protected float castTime = 0f;
        protected float periodicTick = 0f;
        protected float periodicTicks = 0f;
        protected float periodicTickTime = 3f;
        protected float manaCost = 0f;
        protected float gcd = 1.5f;
        protected float crit = 0f;
        protected float critModifier = 1f;
        protected float cooldown = 0f;
        protected float missChance = .17f;

        protected float totalCoef = 1f;
        protected float directCoefBonus = 0f;
        protected float baseCoef = 1f;
        protected float spCoef = 0f;
        protected float dotBaseCoef = 1f;
        protected float dotSpCoef = 0f;
        protected float dotCanCrit = 0f;
        protected float dotCritModifier = 1f;

        protected float spellPower = 0f;

        protected float latencyGcd = .15f;
        protected float latencyCast = .075f;

        /// <summary>
        /// This Constructor calls SetBaseValues.
        /// </summary>
        public Spell()
        {
            SetBaseValues();
        }

        protected virtual void SetBaseValues()
        {
            baseMinDamage = 0f;
            baseMaxDamage = 0f;
            baseCastTime = 0f;
            castTime = 0f;
            periodicTick = 0f;
            periodicTicks = 0f;
            periodicTickTime = 3f;
            manaCost = 0f;
            gcd = 1.5f;
            crit = 0f;
            critModifier = 1f;
            cooldown = 0f;
            missChance = .17f;

            totalCoef = 1f;
            baseCoef = 1f;
            spCoef = 0f;
            dotBaseCoef = 1f;
            dotSpCoef = 0f;
            dotCritModifier = 1f;
            directCoefBonus = 0f;
            dotCanCrit = 0f;
            spellPower = 0f;
        }

        public void Update(ISpellArgs args)
        {
            SetBaseValues();
            Initialize(args);
        }

        protected string shortName = "Spell";

        protected static void add(Spell sp1, Spell sp2, Spell nS)
        {
            nS.baseMinDamage = (sp1.baseMinDamage + sp2.baseMaxDamage);
            nS.baseMaxDamage = (sp1.baseMaxDamage + sp2.baseMaxDamage);
            nS.castTime = (sp1.castTime + sp2.castTime);
            nS.periodicTick = (sp1.periodicTick + sp2.periodicTick);
            nS.periodicTicks = (sp1.periodicTicks + sp2.periodicTicks);
            nS.periodicTickTime = (sp1.periodicTickTime + sp2.periodicTickTime);
            nS.manaCost = (sp1.manaCost + sp2.manaCost);
            nS.gcd = (sp1.gcd + sp2.gcd);
            nS.crit = (sp1.crit + sp2.crit);
            nS.critModifier = (sp1.critModifier + sp2.critModifier);
            nS.cooldown = (sp1.cooldown + sp2.cooldown);
            nS.missChance = (sp1.missChance + sp2.missChance);
            nS.totalCoef = (sp1.totalCoef + sp2.totalCoef);
            nS.directCoefBonus = (sp1.directCoefBonus + sp2.directCoefBonus);
            nS.baseCoef = (sp1.baseCoef + sp2.baseCoef);
            nS.spCoef = (sp1.spCoef + sp2.spCoef);
            nS.dotBaseCoef = (sp1.dotBaseCoef + sp2.dotBaseCoef);
            nS.dotSpCoef = (sp1.dotSpCoef + sp2.dotSpCoef);
            nS.dotCritModifier = (sp1.dotCritModifier + sp2.dotCritModifier);
            nS.spellPower = (sp1.spellPower + sp2.spellPower);
        }

        protected static void multiply(Spell sp1, float c, Spell nS)
        {
            nS.baseMinDamage = sp1.baseMinDamage * c;
            nS.baseMaxDamage = sp1.baseMaxDamage * c;
            nS.castTime = sp1.castTime * c;
            nS.periodicTick = sp1.periodicTick * c;
            nS.periodicTicks = sp1.periodicTicks * c;
            nS.periodicTickTime = sp1.periodicTickTime * c;
            nS.manaCost = sp1.manaCost * c;
            nS.gcd = sp1.gcd * c;
            nS.crit = sp1.crit * c;
            nS.critModifier = sp1.critModifier * c;
            nS.cooldown = sp1.cooldown * c;
            nS.missChance = sp1.missChance * c;
            nS.totalCoef = sp1.totalCoef * c;
            nS.baseCoef = sp1.baseCoef * c;
            nS.spCoef = sp1.spCoef * c;
            nS.dotBaseCoef = sp1.dotBaseCoef * c;
            nS.dotSpCoef = sp1.dotSpCoef * c;
            nS.directCoefBonus = sp1.directCoefBonus * c;
            nS.dotCritModifier = sp1.dotCritModifier * c;
            nS.spellPower = sp1.spellPower * c;
        }

        public virtual float MinHit
        { get { return (totalCoef + directCoefBonus) * (baseMinDamage * baseCoef + spellPower * spCoef); } }

        public float MinCrit
        { get { return MinHit * (1 + critModifier); } }

        public virtual float MaxHit
        { get { return (totalCoef + directCoefBonus) * (baseMaxDamage * baseCoef + spellPower * spCoef); } }

        public float MaxCrit
        { get { return MaxHit * (1 + critModifier); } }

        public float AvgHit
        { get { return (MinHit + MaxHit) / 2; } }

        public float AvgCrit
        { get { return (MinCrit + MaxCrit) / 2; } }

        public float AvgDamage
        { get { return (1f - CritChance) * AvgHit + CritChance * AvgCrit; } }

        public float MinDamage
        { get { return (1f - CritChance) * MinHit + CritChance * MinCrit; } }

        public float MaxDamage
        { get { return (1f - CritChance) * MaxHit + CritChance * MaxCrit; } }

        /// <summary>
        /// This is to ensure that the constraints on the GCD are met on abilities that have Gcd
        /// </summary>
        public float Gcd
        { get { return (gcd >= 1 ? gcd : 1); } }

        /// <summary>
        /// The effective Cast Time. Taking GCD and latency into account.
        /// </summary>
        public float CastTime
        {

            get
            {
                if (gcd == 0 && castTime == 0)
                    return 0;
                if (castTime > Gcd)
                    return castTime + Latency;
                else
                    return Gcd + Latency;
            }
        }

        /// <summary>
        /// The effective Latency of this spell effecting the start cast time of the next one.
        /// </summary>
        public float Latency
        {
            get
            {
                if (gcd == 0 && castTime == 0)
                    return 0;
                if (castTime >= gcd)
                    return latencyCast;
                else
                    return latencyGcd;
            }
        }

        public float BaseCastTime
        {
            get
            {
                return Math.Max(baseCastTime, 1.5f);
            }
        }

        public float CastTimeWithoutGCD
        { get { return castTime; } }

        public float CritChance
        { get { return Math.Min(1f, crit); } }

        /// <summary>
        /// Crit chance for all kind of proc triggers (e.g. Clearcasting). This exists seperately because of Lightning Overload.
        /// </summary>
        public virtual float CCCritChance
        { get { return CritChance; } }

        public float MissChance
        { get { return missChance; } }

        public float HitChance
        { get { return 1f - missChance; } }

        public float DamageFromSpellPower
        { get { return spellPower * spCoef * totalCoef; } }

        public float PeriodicTick
        { get { return totalCoef * (periodicTick * dotBaseCoef + spellPower * dotSpCoef) * (1 + dotCanCrit * dotCritModifier * CritChance); } }

        public float PeriodicTicks
        { 
            get { return periodicTicks; }
            set
            {
                periodicTicks = value;
                if (periodicTicks < 0)
                    periodicTicks = 0;
            }
        }

        public float PeriodicDamage()
        {
            return PeriodicDamage(Duration);
        }

        public float PeriodicDamage(float duration)
        {
            if (PeriodicTickTime <= 0 || duration <= 0)
                return 0;
            int effectiveTicks = (int)Math.Floor(Math.Min(duration, Duration) / PeriodicTickTime);
            return PeriodicTick * effectiveTicks;
        }

        public virtual float TotalDamage
        { get { return AvgDamage + PeriodicDamage(); } }

        public virtual float DirectDpS
        { get { return AvgDamage / CastTime; } }

        public float PeriodicDpS
        { get { return PeriodicTick / periodicTickTime; } }

        public float PeriodicTickTime
        { get { return periodicTickTime; } }

        public float DpM
        { get { return TotalDamage / manaCost; } }

        public float DpCT
        { get { return TotalDamage / CastTime; } }

        public float DpPR
        { get { return TotalDamage / PeriodicRefreshTime; } }

        public float DpCDR
        { get { return TotalDamage / CDRefreshTime; } }

        public float CTpDuration
        { get { return Duration > 0 ? CastTime / Duration : 1f; } }

        public float Duration
        { get { return periodicTicks * periodicTickTime; } }

        public float Cooldown
        { get { return cooldown; } }

        public float PeriodicRefreshTime
        { get { return (Duration > CDRefreshTime ? Duration : CDRefreshTime); } }

        public float CDRefreshTime
        { get { return cooldown > CastTime ? cooldown + castTime : CastTime; } }
        //{ get { return cooldown + CastTime; } }

        public float ManaCost
        { get { return manaCost; } }

        public virtual void Initialize(ISpellArgs args)
        {
            float Speed = (1f + args.Stats.SpellHaste) * (1f + StatConversion.GetSpellHasteFromRating(args.Stats.HasteRating));
            gcd = (float)Math.Round(gcd / Speed, 4);
            castTime = (float)Math.Round(castTime / Speed, 4);
            latencyGcd = args.LatencyGCD;
            latencyCast = args.LatencyCast;
            critModifier += .2f * args.Talents.ElementalFury;
            critModifier *= (float)Math.Round(1.5f * (1f + args.Stats.BonusSpellCritMultiplier) - 1f, 6);
            dotCritModifier += .2f * args.Talents.ElementalFury;
            dotCritModifier *= (float)Math.Round(1.5f * (1f + args.Stats.BonusSpellCritMultiplier) - 1f, 6);
            //critModifier += 1f;
            spellPower += args.Stats.SpellPower;
            crit += args.Stats.SpellCrit;
            missChance -= args.Stats.SpellHit;
            totalCoef *= 1 + args.Stats.BonusDamageMultiplier; //ret + bm buff
            if (missChance < 0) missChance = 0;
            manaCost = (float)Math.Floor(manaCost);
            //base resistance by level
            totalCoef *= 1f - StatConversion.GetAverageResistance(80, 83, 0, 0);
        }

        public void ApplyDotHaste(ISpellArgs args)
        {
            float Speed = (1f + args.Stats.SpellHaste) * (1f + StatConversion.GetSpellHasteFromRating(args.Stats.HasteRating));
            periodicTickTime = (float)Math.Round(periodicTickTime / Speed, 4);
        }

        public void ApplyEM(float modifier)
        {
            throw new NotImplementedException();
        }

        public virtual Spell Clone()
        {
            return (Spell)this.MemberwiseClone();
        }

        public override string ToString()
        {
            return shortName;
        }
    }
}