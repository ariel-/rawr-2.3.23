using System;

namespace Rawr.Elemental.Spells
{
public class Thunderstorm : Spell
    {
        public Thunderstorm() : base()
        {
        }

        protected override void SetBaseValues()
        {
            base.SetBaseValues();

            baseMinDamage = 1450;
            baseMaxDamage = 1656;
            castTime = 0f;
            spCoef = 1.5f / 7f * .9f; // NOT CORRECT YET, assuming 1.5f/7f * 0.9f (aoe with additional effect)
            manaCost = 0f;
            cooldown = 45f;
            shortName = "TS";
        }

        public override void Initialize(ISpellArgs args)
        {
            totalCoef += .01f * args.Talents.Concussion;
            crit += .05f * args.Talents.CallOfThunder;
            crit += .01f * args.Talents.TidalMastery;
            spellPower += args.Stats.SpellNatureDamageRating;
            totalCoef *= 1 + args.Stats.BonusNatureDamageMultiplier;

            if (args.Talents.GlyphofThunder)
                cooldown -= 10f;

            base.Initialize(args);
        }

        public Thunderstorm(ISpellArgs args)
            : this()
        {
            Initialize(args);
        }

        public static float getProcsPerSecond(bool glyphed, int fightDuration)
        {
            if (glyphed)
            {
                if (manaRestoreGlyphed == null)
                    manaRestoreGlyphed = new SpecialEffect(Trigger.Use, new Stats { }, 0f, 35f, 1f);
                return manaRestoreGlyphed.GetAverageProcsPerSecond(35f, 1f, 1f, fightDuration);
            }
            else
            {
                if (manaRestoreUnglyphed == null)
                    manaRestoreUnglyphed = new SpecialEffect(Trigger.Use, new Stats { }, 0f, 45f, 1f);
                return manaRestoreUnglyphed.GetAverageProcsPerSecond(45f, 1f, 1f, fightDuration);
            }
        }

        private static SpecialEffect manaRestoreGlyphed = null;
        private static SpecialEffect manaRestoreUnglyphed = null;

        public static Thunderstorm operator +(Thunderstorm A, Thunderstorm B)
        {
            Thunderstorm C = (Thunderstorm)A.MemberwiseClone();
            add(A, B, C);
            return C;
        }

        public static Thunderstorm operator *(Thunderstorm A, float b)
        {
            Thunderstorm C = (Thunderstorm)A.MemberwiseClone();
            multiply(A, b, C);
            return C;
        }
    }
}