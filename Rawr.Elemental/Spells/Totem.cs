using System;

namespace Rawr.Elemental.Spells
{
    public class Totem : Spell
    {
        protected float averageUptime;

        public Totem() : base()
        {
        }

        protected override void SetBaseValues()
        {
            base.SetBaseValues();
            shortName = "Totem";
            castTime = 0f;
            gcd = 1f;
        }

        public override float CCCritChance
        {
            get
            {
                return 0f;
            }
        }

        public override void Initialize(ISpellArgs args)
        {
            base.Initialize(args);
        }

        public float AverageUptime
        {
            get
            {
                return averageUptime;
            }
            set
            {
                averageUptime = value;
            }
        }
    }
}