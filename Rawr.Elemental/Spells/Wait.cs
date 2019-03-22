using System;

namespace Rawr.Elemental.Spells
{
    public class Wait : Spell
    {
        public Wait() : base()
        {
        }

        protected override void SetBaseValues()
        {
            base.SetBaseValues();

            gcd = 0f;
            missChance = 0f;
            totalCoef = 0f;
            spCoef = 0f;
        }

        public void Initialize(float duration)
        {
            castTime = duration;
            baseCastTime = duration;
            latencyCast = 0;
            latencyGcd = 0;
        }

        public Wait(float duration)
            : this()
        {
            Initialize(duration);
        }

        public override string ToString()
        {
            return "W(" + Math.Round(castTime,2) + ")";
        }
    }
}