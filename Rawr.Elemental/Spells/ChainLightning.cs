using System;

namespace Rawr.Elemental.Spells
{
public class ChainLightning : Spell, ILightningOverload
    {
        private int additionalTargets = 0;
        private float loCoef, lightningSpellpower = 0f, lspCoef;
        public ChainLightning() : base()
        {
        }

        protected override void SetBaseValues()
        {
            base.SetBaseValues();

            baseMinDamage = 973;
            baseMaxDamage = 1111;
            baseCastTime = 2f;
            castTime = 2f;
            spCoef = 2f / 3.5f;
            lspCoef = spCoef;
            loCoef = spCoef / 2f;
            manaCost = 0.26f * Constants.BaseMana;
            cooldown = 6f;
            lightningSpellpower = 0f;
        }

        public override void Initialize(ISpellArgs args)
        {
            additionalTargets = args.AdditionalTargets;
            // jumps
            if (additionalTargets < 0)
                additionalTargets = 0;
            if (additionalTargets > 3)
                additionalTargets = 3;
            shortName = "CL" + (1 + additionalTargets);
            if (!args.Talents.GlyphofChainLightning && additionalTargets > 2)
                additionalTargets = 2;
            totalCoef *= new float[] { 1f, 1.7f, 2.19f, 2.533f, 2.7731f }[additionalTargets];

            manaCost *= 1f - .02f * args.Talents.Convection;
            totalCoef += .01f * args.Talents.Concussion;
            crit += .05f * args.Talents.CallOfThunder;
            spCoef += .04f * args.Talents.Shamanism;
            loCoef += .04f * args.Talents.Shamanism;
            castTime -= .1f * args.Talents.LightningMastery;
            cooldown -= new float[] { 0, .75f, 1.5f, 2.5f }[args.Talents.StormEarthAndFire];
            crit += .01f * args.Talents.TidalMastery;
            spellPower += args.Stats.SpellNatureDamageRating;
            lightningSpellpower += args.Stats.LightningSpellPower;
            totalCoef *= 1 + args.Stats.BonusNatureDamageMultiplier;

            lightningOverload = args.Talents.LightningOverload;

            base.Initialize(args);
        }

        public ChainLightning(ISpellArgs args)
            : this()
        {
            Initialize(args);
        }

        public int AdditionalTargets
        {
            get { return additionalTargets; }
        }

        public static ChainLightning operator +(ChainLightning A, ChainLightning B)
        {
            ChainLightning C = (ChainLightning)A.MemberwiseClone();
            add(A, B, C);
            return C;
        }

        public static ChainLightning operator *(ChainLightning A, float b)
        {
            ChainLightning C = (ChainLightning)A.MemberwiseClone();
            multiply(A, b, C);
            return C;
        }

        private int lightningOverload = 0;

        public override float CCCritChance
        {
            get { return Math.Min(1f, CritChance * (1f + AdditionalTargets) * (1f + LOChance())); }
        }

        public override float MinHit
        {
            get { return totalCoef * (baseMinDamage * baseCoef + spellPower * spCoef + lightningSpellpower * lspCoef); }
        }

        public override float MaxHit
        {
            get { return totalCoef * (baseMaxDamage * baseCoef + spellPower * spCoef + lightningSpellpower * lspCoef); }
        }

        public float LOChance()
        {
            return .11f * lightningOverload / 3f * (1 + AdditionalTargets);
        }

        public override float TotalDamage
        {
            get { return base.TotalDamage + LOChance() * LightningOverloadDamage(); }
        }
        
        public override float DirectDpS
        {
            get { return (AvgDamage + LOChance() * LightningOverloadDamage()) / CastTime; }
        }

        public float LightningOverloadDamage()
        {
            return totalCoef * ((baseMinDamage + baseMaxDamage) / 4f * baseCoef + spellPower * loCoef + lightningSpellpower * lspCoef) * (1 + CritChance * critModifier);
        }
    }
}