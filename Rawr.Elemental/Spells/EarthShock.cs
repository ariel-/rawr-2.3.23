using System;

namespace Rawr.Elemental.Spells
{
public class EarthShock : Shock
    {
        public EarthShock() : base()
        {
        }

        protected override void SetBaseValues()
        {
            base.SetBaseValues();
            baseMinDamage = 854;
            baseMaxDamage = 900;
            spCoef = 1.5f / 3.5f * .9f;
            manaCost = .18f * Constants.BaseMana;
            cooldown = 6f;
            shortName = "ES";
        }

        public override void Initialize(ISpellArgs args)
        {
            totalCoef += .01f * args.Talents.Concussion;
            manaCost *= 1 - .02f * args.Talents.Convection;
            manaCost *= 1 - .45f * args.Talents.ShamanisticFocus;
            cooldown -= .2f * args.Talents.Reverberation;
            spellPower += args.Stats.SpellNatureDamageRating;
            totalCoef *= 1 + args.Stats.BonusNatureDamageMultiplier;
            if (args.Talents.GlyphofShocking)
                gcd -= .5f;

            base.Initialize(args);
        }

        public EarthShock(ISpellArgs args) : this()
        {
            Initialize(args);
        }

        public static EarthShock operator +(EarthShock A, EarthShock B)
        {
            EarthShock C = (EarthShock)A.MemberwiseClone();
            add(A, B, C);
            return C;
        }

        public static EarthShock operator *(EarthShock A, float b)
        {
            EarthShock C = (EarthShock)A.MemberwiseClone();
            multiply(A, b, C);
            return C;
        }

    }
}