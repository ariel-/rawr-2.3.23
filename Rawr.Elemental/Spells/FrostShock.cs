using System;

namespace Rawr.Elemental.Spells
{
public class FrostShock : Shock
    {
        public FrostShock() : base()
        {
        }

        protected override void SetBaseValues()
        {
            base.SetBaseValues();

            baseMinDamage = 812;
            baseMaxDamage = 858;
            spCoef = 1.5f / 3.5f * .9f;
            manaCost = .18f * Constants.BaseMana;
            cooldown = 6f;
            shortName = "FrS";
        }

        public override void Initialize(ISpellArgs args)
        {
            totalCoef += .01f * args.Talents.Concussion;
            spCoef *= 1f + .1f * args.Talents.BoomingEchoes;
            manaCost *= 1 - .02f * args.Talents.Convection;
            manaCost *= 1 - .45f * args.Talents.ShamanisticFocus;
            cooldown -= .2f * args.Talents.Reverberation;
            cooldown -= 1f * args.Talents.BoomingEchoes;
            spellPower += args.Stats.SpellFrostDamageRating;
            totalCoef *= 1 + args.Stats.BonusFrostDamageMultiplier;
            if (args.Talents.GlyphofShocking)
                gcd -= .5f;

            base.Initialize(args);
        }

        public FrostShock(ISpellArgs args)
            : this()
        {
            Initialize(args);
        }

        public static FrostShock operator +(FrostShock A, FrostShock B)
        {
            FrostShock C = (FrostShock)A.MemberwiseClone();
            add(A, B, C);
            return C;
        }

        public static FrostShock operator *(FrostShock A, float b)
        {
            FrostShock C = (FrostShock)A.MemberwiseClone();
            multiply(A, b, C);
            return C;
        }
    }
}