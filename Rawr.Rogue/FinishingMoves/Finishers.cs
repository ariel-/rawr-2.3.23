using System.Collections.Generic;
using System;

namespace Rawr.Rogue.FinishingMoves
{
#if (SILVERLIGHT == false)
    [Serializable]
#endif
    public class Finishers : List<FinisherBase>
    {
        /*public Finishers()
        {
            Add(new NoFinisher());
            Add(new SnD());
            Add(new Rupture());
            Add(new Evis());
            Add(new Envenom());
        }

        public static FinisherBase Get(string name)
        {
            foreach (var finisher in new Finishers())
            {
                if (name == finisher.Name)
                {
                    return finisher;
                }
            }
            return new NoFinisher();
        }*/
    }
}
