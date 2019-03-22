using System.Collections.Generic;
#if RAWR3
using System.Linq;
#endif

namespace Rawr.Rogue
{
    public class DisplayValue
    {
        //---------------------------------------------------------------------
        //This class keeps the UI labels and calculations in sync in a single 
        //class.  By simply adding a new "public static readonly DisplayValue..." 
        //we automatically add it to the UI, and make it availble in our list 
        //of values to be displayed.
        //ALSO:  the order of the items in this list will be the order of 
        //appearance in the UI
        //---------------------------------------------------------------------
        private DisplayValue(string grouping, string name)            
        {
            Grouping = grouping;
            Name = name;
            _list.Add(this);
        }

        public readonly string Grouping;
        public readonly string Name;
        private static readonly List<DisplayValue> _list = new List<DisplayValue>();

        public static readonly DisplayValue Health = new DisplayValue("Base Stats", "Health");
        public static readonly DisplayValue Stamina = new DisplayValue("Base Stats", "Stamina");
        public static readonly DisplayValue Strength = new DisplayValue("Base Stats", "Strength");
        public static readonly DisplayValue Agility = new DisplayValue("Base Stats", "Agility");
        public static readonly DisplayValue AttackPower = new DisplayValue("Base Stats", "Attack Power");
        public static readonly DisplayValue Haste = new DisplayValue("Base Stats", "Haste");
        public static readonly DisplayValue CritRating = new DisplayValue("Base Stats", "Crit Rating");
        public static readonly DisplayValue ArmorDamageReduction = new DisplayValue("Base Stats", "Armor Damage Reduction");
        public static readonly DisplayValue MhWeaponDamage = new DisplayValue("Base Stats", "MH Weapon Damage");
        public static readonly DisplayValue OhWeaponDamage = new DisplayValue("Base Stats", "OH Weapon Damage");
        public static readonly DisplayValue HitRating = new DisplayValue("Base Stats", "Hit Rating");
        public static readonly DisplayValue BaseExpertise = new DisplayValue("Base Stats", "Base Expertise");
        public static readonly DisplayValue SndUptime = new DisplayValue("Base Stats", "SnD UpTime %");

        public static readonly DisplayValue EnergyRegen = new DisplayValue("DPS Breakdown", "Energy Regen");
        public static readonly DisplayValue CycleTime = new DisplayValue("DPS Breakdown", "Cycle Time (in seconds)");
        public static readonly DisplayValue Cpg = new DisplayValue("DPS Breakdown", "CPG");
        public static readonly DisplayValue CpgCrit = new DisplayValue("DPS Breakdown", "CPG Crit %");
        public static readonly DisplayValue CpgDps = new DisplayValue("DPS Breakdown", "CPG DPS");
        public static readonly DisplayValue FinisherDps = new DisplayValue("DPS Breakdown", "Finisher DPS");
        public static readonly DisplayValue SwordSpecDps = new DisplayValue("DPS Breakdown", "Sword Spec DPS");
        public static readonly DisplayValue PoisonDps = new DisplayValue("DPS Breakdown", "Poison DPS");
        public static readonly DisplayValue WhiteDps = new DisplayValue("DPS Breakdown", "White DPS");
        public static readonly DisplayValue TotalDps = new DisplayValue("DPS Breakdown", "Total DPS");

        public static readonly DisplayValue Dodge = new DisplayValue("Defenses", "Dodge");
        public static readonly DisplayValue Parry = new DisplayValue("Defenses", "Parry");

        public override string ToString()
        {
            return Name;
        }

        public static string[] GroupedList()
        {
            #if SILVERLIGHT
                return _list.ConvertAll<DisplayValue, string>(GroupedNameConverter).ToArray(); 
            #else
                return _list.ConvertAll<string>(GroupedNameConverter).ToArray();
            #endif
        }

        private static string GroupedNameConverter(DisplayValue displayValue)
        {
            return string.Format("{0}:{1}", displayValue.Grouping, displayValue.Name); 
        }
    }
}