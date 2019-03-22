using System;
using Rawr.Rogue.ClassAbilities;

namespace Rawr.Rogue.SpecialAbilities
{
#if (SILVERLIGHT == false)
    [Serializable]
#endif

    public class Feint
    {
        public Feint() : this(0f){}
        public Feint( float delay )
        {
            _delay = delay;
        }

        private readonly float _delay;

        public float EnergyCost()
        {
            var baseCost = 20 - (Glyphs.GlyphOfFeint ? 10 : 0);
            return _delay == 0f ? 0f : baseCost / _delay;
        }

        public bool IsNeedFeint()
        {
            return (_delay > 0) ? true : false;
    }

        public int Delay()
        {
            return (int)_delay;
        }
    }
}
