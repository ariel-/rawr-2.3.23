using System;

namespace Rawr.Elemental.Spells
{
    public class LavaBurst : Spell
    {
        private float fs = 0f;
        private bool elementalT10 = false;

        public LavaBurst() : base()
        {
        }

        protected override void SetBaseValues()
        {
            base.SetBaseValues();

            baseMinDamage = 1192;
            baseMaxDamage = 1518;
            baseCastTime = 2f;
            castTime = 2f;
            spCoef = 2f / 3.5f;
            manaCost = .1f * Constants.BaseMana;
            cooldown = 8f;
            shortName = "LvB";
        }

        public override void Initialize(ISpellArgs args)
        {
            Initialize(args, fs);
        }

        public void Initialize(ISpellArgs args, float fs)
        {
            manaCost *= 1f - .02f * args.Talents.Convection;
            totalCoef += .01f * args.Talents.Concussion;
            totalCoef += .02f * args.Talents.CallOfFlame;
            totalCoef += args.Stats.BonusLavaBurstDamageMultiplier; // t9 4 piece
            castTime -= .1f * args.Talents.LightningMastery;
            spCoef += .05f * args.Talents.Shamanism;
            critModifier += new float[] { 0f, 0.06f, 0.12f, 0.24f }[args.Talents.LavaFlows];
            critModifier += args.Stats.BonusLavaBurstCritDamage / 100f; // t7 4 piece
            baseMinDamage += args.Stats.LavaBurstBonus; // Totem (relic)
            baseMaxDamage += args.Stats.LavaBurstBonus; // Totem (relic) 
            spellPower += args.Stats.SpellFireDamageRating;
            if (args.Talents.GlyphofLava)
                spCoef += .1f;
            totalCoef *= 1 + args.Stats.BonusFireDamageMultiplier;
            if (args.Stats.Elemental4T10 > 0) elementalT10 = true;

            base.Initialize(args);

            this.fs = fs;
            crit = (1 - fs) * crit + fs;
        }

        public LavaBurst(ISpellArgs args, float fs)
            : this()
        {
            this.fs = fs;
            Initialize(args, fs);
        }

        public static LavaBurst operator +(LavaBurst A, LavaBurst B)
        {
            LavaBurst C = (LavaBurst)A.MemberwiseClone();
            add(A, B, C);
            return C;
        }

        public static LavaBurst operator *(LavaBurst A, float b)
        {
            LavaBurst C = (LavaBurst)A.MemberwiseClone();
            multiply(A, b, C);
            return C;
        }

        /// <summary>
        /// Adds the closest amount of hasted FS ticks to 6s to the already running FS.
        /// </summary>
        public bool ElementalT10
        {
            get { return elementalT10; }
        }

    }
}