using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Rawr.Mage
{
    public class Encounter
    {
        public List<DamageMultiplier> GlobalMultipliers { get; set; }
        public List<TargetGroup> TargetGroups { get; set; }

        public Encounter()
        {
            GlobalMultipliers = new List<DamageMultiplier>();
            TargetGroups = new List<TargetGroup>();
        }
    }

    public class DamageMultiplier
    {
        public float Multiplier { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public bool RelativeTime { get; set; }

        public DamageMultiplier()
        {
            RelativeTime = true;
            StartTime = 0.0f;
            EndTime = 1.0f;
            Multiplier = 1.0f;
        }
    }

    public class TargetGroup
    {
        //public int TargetLevel { get; set; }
        //public int Count { get; set; }
        public float EntranceTime { get; set; }
        public float ExitTime { get; set; }
        public bool RelativeTime { get; set; }
        //public float MoltenFury { get; set; }
        //public float ArcaneResist { get; set; }
        //public float FireResist { get; set; }
        //public float FrostResist { get; set; }
        //public float ShadowResist { get; set; }
        //public float NatureResist { get; set; }
        //public float HolyResist { get; set; }
        public List<DamageMultiplier> Multipliers { get; set; }

        public TargetGroup()
        {
            //TargetLevel = 83;
            //Count = 1;
            EntranceTime = 0.0f;
            ExitTime = 1.0f;
            RelativeTime = true;
            //MoltenFury = 0.3f;
            Multipliers = new List<DamageMultiplier>();
        }
    }
}
