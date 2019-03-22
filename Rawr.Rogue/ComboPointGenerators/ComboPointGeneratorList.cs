using System;
using System.Collections.Generic;
using System.IO;

namespace Rawr.Rogue.ComboPointGenerators
{
    public class ComboPointGeneratorList : List<ComboPointGenerator>
    {
        /*public ComboPointGeneratorList()
        {
            Add(new Mutilate());
            Add(new Backstab());
            Add(new SinisterStrike());
            Add(new Hemo());
            Add(new HonorAmongThieves(0f,0f));
        }

        public static ComboPointGenerator Get(string name)
        {
            foreach (var cpGenerator in new ComboPointGeneratorList())
            {
                if (name == cpGenerator.Name)
                {
                    return cpGenerator;
                }
            }
            throw new Exception("Cannot find: " + name);
        }*/
    }
}