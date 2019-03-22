using System;

namespace Rawr.Elemental.Spells
{
public class ElementalMastery : Spell
    {
        public ElementalMastery() : base()
        {
        }

        protected override void SetBaseValues()
        {
            base.SetBaseValues();
            missChance = 0;
            cooldown = 180f;
            gcd = 0; // no global cooldown ;)
            shortName = "EM";
        }

        public override void Initialize(ISpellArgs args)
        {
            if (args.Talents.GlyphofElementalMastery)
                cooldown -= 30f;

            base.Initialize(args);
        }

        public ElementalMastery(ISpellArgs args)
            : this()
        {
            Initialize(args);
        }

        public static float getAverageUptime(bool glyphed, int fightDuration)
        {
            if (glyphed)
            {
                if (uptimeGlyphed == null)
                    uptimeGlyphed = new SpecialEffect(Trigger.Use, new Stats { SpellCrit = 0.2f }, 15f, 150f);
                return uptimeGlyphed.GetAverageUptime(150f, 1f, 1f, fightDuration);
            }
            else
            {
                if (uptimeUnglyphed == null)
                    uptimeUnglyphed = new SpecialEffect(Trigger.Use, new Stats { SpellCrit = 0.2f }, 15f, 180f);
                return uptimeUnglyphed.GetAverageUptime(180f, 1f, 1f, fightDuration);
            }
        }

        private static SpecialEffect uptimeGlyphed = null;
        private static SpecialEffect uptimeUnglyphed = null;


        public static ElementalMastery operator +(ElementalMastery A, ElementalMastery B)
        {
            ElementalMastery C = (ElementalMastery)A.MemberwiseClone();
            add(A, B, C);
            return C;
        }

        public static ElementalMastery operator *(ElementalMastery A, float b)
        {
            ElementalMastery C = (ElementalMastery)A.MemberwiseClone();
            multiply(A, b, C);
            return C;
        }
    }
}