using System;

namespace Rawr.Elemental.Spells
{
    public class Shock : Spell
    {
        public Shock() : base()
        {
        }

        protected override void SetBaseValues()
        {
            base.SetBaseValues();
            shortName = "Shock";
        }
    }
}