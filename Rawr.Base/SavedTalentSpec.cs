using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Rawr
{
    [GenerateSerializer]
    public class SavedTalentSpecList : List<SavedTalentSpec>
    {
        public SavedTalentSpecList() : base() { }
        public SavedTalentSpecList(int capacity) : base(capacity) { }
    }

    public class SavedTalentSpec
    {
        public string Name { get; set; }
        public CharacterClass Class { get; set; }
        public string Spec { get; set; }

        public int Tree1 { get; set; }
        public int Tree2 { get; set; }
        public int Tree3 { get; set; }

        public SavedTalentSpec() : this("", null, 0, 0, 0) { ; }

        public static SavedTalentSpecList AllSpecs { get; private set; }
        public static void Load(TextReader reader)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SavedTalentSpecList));
                AllSpecs = (SavedTalentSpecList)serializer.Deserialize(reader);
                reader.Close();
            }
            catch { }
            finally
            {
                reader.Close();
                if (AllSpecs == null) AllSpecs = new SavedTalentSpecList();
            }
        }

        public static void Save(TextWriter writer)
        {
            XmlSerializer serilizer = new XmlSerializer(typeof(SavedTalentSpecList));
            serilizer.Serialize(writer, AllSpecs);
            writer.Close();
        }

        public static SavedTalentSpecList SpecsFor(CharacterClass charClass)
        {
            SavedTalentSpecList ret = new SavedTalentSpecList();
            foreach (SavedTalentSpec sts in AllSpecs)
            {
                if (sts.Class == charClass) ret.Add(sts);
            }
            return ret;
        }

        public SavedTalentSpec(String name, TalentsBase talentSpec, int tree1, int tree2, int tree3)
        {
            Name = name;
            Tree1 = tree1;
            Tree2 = tree2;
            Tree3 = tree3;
            if (talentSpec != null)
            {
                Spec = talentSpec.ToString();
                Class = talentSpec.GetClass();
            }
        }

        public TalentsBase TalentSpec()
        {
            if (Spec == null) return null;
            TalentsBase spec;
            if (Class == CharacterClass.DeathKnight) spec = new DeathKnightTalents(Spec);
            else if (Class == CharacterClass.Warrior) /*if(CataMode) { spec = new WarriorTalentsCata(Spec); } else {*/spec = new WarriorTalents(Spec);/*}*/
            else if (Class == CharacterClass.Paladin) spec = new PaladinTalents(Spec);
            else if (Class == CharacterClass.Shaman) spec = new ShamanTalents(Spec);
            else if (Class == CharacterClass.Hunter) spec = new HunterTalents(Spec);
            else if (Class == CharacterClass.Rogue) spec = new RogueTalents(Spec);
            else if (Class == CharacterClass.Druid) spec = new DruidTalents(Spec);
            else if (Class == CharacterClass.Warlock) spec = new WarlockTalents(Spec);
            else if (Class == CharacterClass.Priest) spec = new PriestTalents(Spec);
            else spec = new MageTalents(Spec);
            return spec;
        }

        public override string ToString()
        {
            //return string.Format("{0} ({1}/{2}/{3})", Name, Tree1, Tree2, Tree3);
            string warning = "";
            // TODO: That 71 shouldn't be hard-coded, but I don't have a Character.Level here.
            int pointsleft = 71 - (Tree1 + Tree2 + Tree3);

            if (pointsleft > 0)
                warning = string.Format(" ({0} Points Left)", pointsleft);
            else if (pointsleft < 0)
                warning = string.Format(" ({0} Points Over)", Math.Abs(pointsleft));

            return string.Format("{0} ({1}/{2}/{3}){4}", Name, Tree1, Tree2, Tree3, warning);
        }

        public bool Equals(TalentsBase talents)
        {
            if (talents == null || Spec == null) return false;
            return talents.ToString().Equals(Spec) && Class == talents.GetClass();
        }
    }
}
