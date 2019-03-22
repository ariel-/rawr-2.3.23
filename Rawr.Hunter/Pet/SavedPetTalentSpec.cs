using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Rawr.Hunter
{
    [GenerateSerializer]
    public class SavedPetTalentSpecList : List<SavedPetTalentSpec>
    {
        public SavedPetTalentSpecList() : base() { }
        public SavedPetTalentSpecList(int capacity) : base(capacity) {  }
    }

    public class SavedPetTalentSpec
    {
        public string Name { get; set; }
        public PetFamilyTree Class { get; set; }
        public string Spec { get; set; }

        public int Tree { get; set; }

        public SavedPetTalentSpec() : this("", null, PetFamilyTree.None, 0) { ; }

        public static SavedPetTalentSpecList AllSpecs { get; private set; }
        public static void Load(TextReader reader)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SavedPetTalentSpecList));
                AllSpecs = (SavedPetTalentSpecList)serializer.Deserialize(reader);
                reader.Close();
            }
            catch { }
            finally
            {
                reader.Close();
                if (AllSpecs == null) AllSpecs = new SavedPetTalentSpecList();
            }
        }

        public static void Save(TextWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SavedPetTalentSpecList));
            serializer.Serialize(writer, AllSpecs);
            writer.Close();
        }

        public static SavedPetTalentSpecList SpecsFor(PetFamilyTree petClass)
        {
            SavedPetTalentSpecList ret = new SavedPetTalentSpecList();
            foreach (SavedPetTalentSpec sts in AllSpecs)
            {
                if (sts.Class == petClass) ret.Add(sts);
            }
            return ret;
        }

#if RAWR3 || SILVERLIGHT
        public SavedPetTalentSpec(String name, PetTalentsBase talentSpec, PetFamilyTree tree, int pts)
#else
        public SavedPetTalentSpec(String name, PetTalentTreeData talentSpec, PetFamilyTree tree, int pts)
#endif
        {
            Name = name;
            Tree = pts;
            if (talentSpec != null)
            {
                Spec = talentSpec.ToString();
                Class = tree;
            }
        }

#if RAWR3 || SILVERLIGHT
        public PetTalents TalentSpec() {
            if (Spec == null) return null;
            PetTalents spec = new PetTalents(Spec);
            return spec;
        }
#else
        public PetTalentTreeData TalentSpec() {
            if (Spec == null) return null;
            PetTalentTreeData spec = new PetTalentTreeData(Spec);
            return spec;
        }
#endif

        public override string ToString()
        {
            //return string.Format("{0} ({1})", Name, Tree);
            string warning = "";
            // TODO: That 16 shouldn't be hard-coded, but I don't have a Character.Level here.
            // also need to factor in beast master talent for +4 points
            int pointsleft = 16 - (Tree);
            int bmpointsleft = 20 - (Tree);

            if (pointsleft > 0)                              warning = string.Format(" (Norm: {0} Left|BM: {1} Left)", Math.Abs(pointsleft), Math.Abs(bmpointsleft));
            else if ((pointsleft == 0 && bmpointsleft >  0)) warning = string.Format(" (Norm:" +" Even|BM: {0} Left)", Math.Abs(bmpointsleft));
            else if ((pointsleft <  0 && bmpointsleft >  0)) warning = string.Format(" (Norm: {0} Over|BM: {1} Left)", Math.Abs(pointsleft), Math.Abs(bmpointsleft));
            else if ((pointsleft <  0 && bmpointsleft == 0)) warning = string.Format(" (Norm: {0} Over|BM:" +" Even)", Math.Abs(pointsleft));
            else if ((pointsleft <  0 && bmpointsleft <  0)) warning = string.Format(" (Norm: {0} Over|BM: {1} Over)", Math.Abs(pointsleft), Math.Abs(bmpointsleft));

            return string.Format("{0} ({1}){2}", Name, Tree, warning);
        }

#if RAWR3 || SILVERLIGHT
        public bool Equals(PetTalents talents)
#else
        public bool Equals(PetTalentTreeData talents)
#endif
        {
            if (talents == null || Spec == null) return false;
            return talents.ToString().Equals(Spec) ;//&& Class == talents.GetClass();
        }
    }
}
