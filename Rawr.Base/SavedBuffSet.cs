using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Rawr
{
    [GenerateSerializer]
    public class SavedBuffSetList : List<SavedBuffSet>
    {
        public SavedBuffSetList() : base() { }
        public SavedBuffSetList(int capacity) : base(capacity) { }
    }

    public class SavedBuffSet
    {

        public string Name { get; set; }
        public string SetAsString { get; set; }
        public List<Buff> BuffSet { get; set; }

        public SavedBuffSet() : this("", null) { ; }

        private static SavedBuffSetList _AllSets;
        public static SavedBuffSetList AllSets { get { return _AllSets ?? (_AllSets = new SavedBuffSetList()); } private set { _AllSets = value; } }

        public static void Load(TextReader reader)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SavedBuffSetList));
                AllSets = (SavedBuffSetList)serializer.Deserialize(reader);
                reader.Close();
            }
            catch { }
            finally
            {
                reader.Close();
                if (AllSets == null) AllSets = new SavedBuffSetList();
            }
        }

        public static void Save(TextWriter writer)
        {
            XmlSerializer serilizer = new XmlSerializer(typeof(SavedBuffSetList));
            serilizer.Serialize(writer, AllSets);
            writer.Close();
        }

        public SavedBuffSet(String name, List<Buff> buffSet)
        {
            Name = name;
            if (buffSet != null) {
                List<Buff> toSave = new List<Buff>();
                foreach (Buff b in buffSet) {
                    if (b == null || b.Group == null) continue;
                    // Remove these since they are tied to other stuff and will be auto-enforced by other means
                    // We will also not be looking at them when doing an Equals check
                    if (b.Group == "Set Bonuses" || b.Group == "Profession Buffs") { } else { toSave.Add(b); }
                }
                SetAsString = toSave.ToString();
                BuffSet = toSave;
            }
        }

        public override string ToString() { return Name; }

        /// <summary>
        /// This function is overridden to remove Set Bonuses and Professions Buffs. It will still compare the rest.
        /// </summary>
        /// <param name="otherBuffSet"></param>
        /// <returns></returns>
        public bool Equals(List<Buff> otherBuffSet)
        {
            // Fail on null sets
            if (otherBuffSet == null || SetAsString == null) { return false; }
            // Remove Buffs we don't want to verify in equality: Set Bonuses and Profession Buffs
            List<Buff> clonedotherBuffSet = new List<Buff>();
            List<Buff> toRemove = new List<Buff>();
            foreach (Buff b in /*cloned*/otherBuffSet) {
                if (b == null || b.Group == null) continue;
                if (b.Group == "Set Bonuses" || b.Group == "Profession Buffs") { } else { clonedotherBuffSet/*toRemove*/.Add(b); }
            }
            // Fail on not the same array size, this saves us some processing time when we already know it won't match
            if (BuffSet.Count != clonedotherBuffSet.Count) {
                return false;
            }
            // Replicate the arrays for easier direct comparison
            List<String> thisSetAsStrings = new List<String>(); foreach (Buff b in BuffSet) { thisSetAsStrings.Add(b.Name); } thisSetAsStrings.Sort();
            List<String> otherSetAsStrings = new List<String>(); foreach (Buff b in clonedotherBuffSet) { otherSetAsStrings.Add(b.Name); } otherSetAsStrings.Sort();
            // Check each array to see if the opposite one is missing a buff that the original contains
            bool noMatch = false;
            foreach (String t in thisSetAsStrings) {
                // Check other set to see if anything in this set isn't there
                if (!otherSetAsStrings.Contains(t)) { noMatch = true; break; }
            }
            if (noMatch) return false;
            foreach (String o in otherSetAsStrings) {
                // Check this set to see if anything in other set isn't there
                if (!thisSetAsStrings.Contains(o)) { noMatch = true; break; }
            }
            if (noMatch) return false;

            // If nothing else failed, return true
            return true;

            //return thisSetAsStrings.Equals(otherSetAsStrings);
        }
    }
}
