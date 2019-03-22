using System;

namespace Rawr.Elemental.Spells
{
    public class LightningBolt : Spell, ILightningOverload
    {
        public LightningBolt() : base()
        {
        }

        protected override void SetBaseValues()
        {
            base.SetBaseValues();

            baseMinDamage = 719;
            baseMaxDamage = 819;
            baseCastTime = 2.5f;
            castTime = 2.5f;
            spCoef = 2.5f / 3.5f;
            lspCoef = spCoef;
            loCoef = spCoef / 2f;
            manaCost = 0.1f * Constants.BaseMana;
            lightningSpellpower = 0f;
            shortName = "LB";
        }

        public override void Initialize(ISpellArgs args)
        {
            castTime -= .1f * args.Talents.LightningMastery;
            manaCost *= 1f - .02f * args.Talents.Convection;
            totalCoef += .01f * args.Talents.Concussion;
            crit += .05f * args.Talents.CallOfThunder;
            spCoef += .04f * args.Talents.Shamanism;
            loCoef += .04f * args.Talents.Shamanism;
            crit += .05f * args.Talents.TidalMastery;
            manaCost *= 1 - args.Stats.LightningBoltCostReduction / 100f; // T7 2 piece
            spellPower += args.Stats.SpellNatureDamageRating; // Nature SP
            lightningSpellpower += args.Stats.LightningSpellPower; // Totem (relic) is not affected by shamanism
            if (args.Talents.GlyphofLightningBolt) totalCoef += .04f;
            totalCoef *= 1 + args.Stats.BonusNatureDamageMultiplier;
            totalCoef *= 1 + args.Stats.LightningBoltDamageModifier / 100f; // T6 4 piece

            lightningOverload = args.Talents.LightningOverload;

            base.Initialize(args);
            critModifier *= 1 + args.Stats.LightningBoltCritDamageModifier; // T8 4 piece
        }

        public LightningBolt(ISpellArgs args)
            : this()
        {
            Initialize(args);
        }

        public static LightningBolt operator +(LightningBolt A, LightningBolt B)
        {
            LightningBolt C = (LightningBolt)A.MemberwiseClone();
            add(A, B, C);
            return C;
        }

        public static LightningBolt operator *(LightningBolt A, float b)
        {
            LightningBolt C = (LightningBolt)A.MemberwiseClone();
            multiply(A, b, C);
            return C;
        }

        private int lightningOverload = 0;
        private float loCoef, lightningSpellpower = 0f, lspCoef;

        public override float CCCritChance
        {
            get { return Math.Min(1f, CritChance * (1f + LOChance())); }
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
            return .11f * lightningOverload;
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
            return totalCoef * ((baseMinDamage + baseMaxDamage) / 4f * baseCoef + spellPower * loCoef + lightningSpellpower * lspCoef) * (1 + CritChance * critModifier);;
        }
    }
}