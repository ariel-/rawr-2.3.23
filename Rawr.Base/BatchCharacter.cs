using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
#if SILVERLIGHT
using System.Collections.ObjectModel;
#endif

namespace Rawr
{
    public class BatchCharacter : INotifyPropertyChanged
    {
        private string relativePath;
        public string RelativePath
        {
            get
            {
                return relativePath;
            }
            set
            {
                if (relativePath != value)
                {
                    relativePath = value;
#if !SILVERLIGHT
                    character = null;

                    string curDir = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                    absolutePath = Path.GetFullPath(relativePath);
                    Directory.SetCurrentDirectory(curDir);

                    score = Optimizer.ItemInstanceOptimizer.GetOptimizationValue(Character, Model, true);

                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Score"));
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Character"));
#endif
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        private string absolutePath = "";
        [XmlIgnore]
        public string AbsolutePath
        {
            get
            {
                return absolutePath;
            }
        }

        [XmlIgnore]
        public string Name
        {
            get
            {
                if (relativePath == null) return null;
                string name = Path.GetFileNameWithoutExtension(relativePath);
                if (unsavedChanges) name += " *";
                return name;
            }
        }

        private Character character;
        [XmlIgnore]
        public Character Character
        {
            get
            {
                if (character == null && absolutePath != null)
                {
#if !RAWR3
                    character = Character.Load(absolutePath);
#else
                    using (StreamReader reader = new StreamReader(absolutePath))
                    {
                        character = Character.LoadFromXml(reader.ReadToEnd());
                    }
#endif
                    character.CalculationsInvalidated += new EventHandler(character_CalculationsInvalidated);
                }
                return character;
            }
#if SILVERLIGHT
            set
            {
                // Silverlight does not support loading via file name, so we need to load it directly
                character = value;
                originalCharacter = character.Clone();
                character.CalculationsInvalidated += new EventHandler(character_CalculationsInvalidated);
                score = Optimizer.ItemInstanceOptimizer.GetOptimizationValue(character, Model, true);

                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Score"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Character"));
            }
#endif
        }

#if SILVERLIGHT
        // in Silverlight there's no way to load clean version unless we make a clone
        private Character originalCharacter;
        public Character OriginalCharacter
        {
            get
            {
                return originalCharacter;
            }
        }
#endif

        void character_CalculationsInvalidated(object sender, EventArgs e)
        {
            unsavedChanges = true;
            newScore = Optimizer.ItemInstanceOptimizer.GetOptimizationValue(Character, Model, true);

            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("NewScore"));
        }

        private CalculationsBase model;
        [XmlIgnore]
        public CalculationsBase Model
        {
            get
            {
                if (model == null || model.Name != Character.CurrentModel)
                {
                    model = Calculations.GetModel(Character.CurrentModel);
                }
                return model;
            }
        }

        private bool unsavedChanges;
        [XmlIgnore]
        public bool UnsavedChanges
        {
            get
            {
                return unsavedChanges;
            }
            set
            {
                if (unsavedChanges != value)
                {
                    if (!value)
                    {
                        if (newScore != null)
                        {
                            score = (float)newScore;
                            newScore = null;
                        }
                    }
                    unsavedChanges = value;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Score"));
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("NewScore"));
                }
            }
        }

        private float weight = 1f;
        public float Weight
        {
            get
            {
                return weight;
            }
            set
            {
                if (weight != value)
                {
                    weight = value;
                }
            }
        }

        public bool Locked { get; set; }

        private float score;
        [XmlIgnore]
        public float Score
        {
            get
            {
                return score;
            }
        }

        private float? newScore;
        [XmlIgnore]
        public float? NewScore
        {
            get
            {
                return newScore;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [GenerateSerializer]
#if SILVERLIGHT
    public class BatchCharacterList : ObservableCollection<BatchCharacter>
#else
    public class BatchCharacterList : BindingList<BatchCharacter>
#endif
    {
        public static BatchCharacterList Load(string path)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(BatchCharacterList));
            System.IO.StreamReader reader = new System.IO.StreamReader(path);
            BatchCharacterList list = (BatchCharacterList)serializer.Deserialize(reader);
            reader.Close();
            return list;
        }

        public void Save(string path)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(BatchCharacterList));
            System.IO.StreamWriter writer = new System.IO.StreamWriter(path);
            serializer.Serialize(writer, this);
            writer.Close();
        }

        public void Save(Stream stream)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(BatchCharacterList));
            System.IO.StreamWriter writer = new System.IO.StreamWriter(stream);
            serializer.Serialize(writer, this);
            writer.Close();
        }

        public List<KeyValuePair<Character, float>> GetBatchOptimizerList()
        {
            List<KeyValuePair<Character, float>> list = new List<KeyValuePair<Character, float>>();
            foreach (BatchCharacter batchCharacter in this)
            {
                list.Add(new KeyValuePair<Character, float>(batchCharacter.Character, batchCharacter.Weight));
            }
            return list;
        }

        public float NewScore
        {
            get
            {
                float score = 0.0f;
                foreach (BatchCharacter character in this)
                {
                    score += character.Weight * (character.NewScore ?? character.Score);
                }
                return score;
            }
        }

        public float Score
        {
            get
            {
                float score = 0.0f;
                foreach (BatchCharacter character in this)
                {
                    score += character.Weight * character.Score;
                }
                return score;
            }
        }
    }
}
