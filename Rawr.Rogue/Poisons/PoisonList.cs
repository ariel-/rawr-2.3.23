using System.Collections.Generic;

namespace Rawr.Rogue.Poisons
{
	public class PoisonList : List<PoisonBase>
    {
        /*public PoisonList()
        {
            Add(new NoPoison());
            Add(new DeadlyPoison());
            Add(new InstantPoison());
            Add(new WoundPoison());
        }

		public static PoisonBase Get(string poisonName)
        {
            foreach(var poison in new PoisonList())
            {
                if(poison.Name == poisonName)
                {
                    return poison;
                }
            }

            return new NoPoison();
        }*/
    }
}