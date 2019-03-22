using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.TankDK
{
    /// <summary>
    /// The state of combat at the time an ability is used.
    /// This is a container for all the necessary stats that get passed around
    /// during rotation evaluation.
    /// </summary>
    class CombatState
    {
        public Stats m_Stats;
        // Why save a whole character when we only need the talents?
//        public Character m_Char;
        public DeathKnightTalents m_Talents;
        public Weapon MH, OH;
        public float m_NumberOfTargets;


    }
}
