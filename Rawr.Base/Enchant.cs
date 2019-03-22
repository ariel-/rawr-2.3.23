using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Rawr
{
    [GenerateSerializer]
    public class EnchantList : List<Enchant>
    {
    }

    /// <summary>
    /// An object representing an Enchantment to be placed on a slot on a character.
    /// </summary>
    public class Enchant
    {
        /// <summary>
        /// The ID of the enchant. This is determined by viewing the enchant spell on Wowhead, and
        /// noting the Enchant Item Permenant ID in the spell effect data.
        /// 
        /// EXAMPLE:
        /// Enchant Gloves - Superior Agility. This enchant is applied by spell 25080, which you can find
        /// by searching on Wowhead (http://www.wowhead.com/?spell=25080). In the spell Effect section, it
        /// says "Enchant Item Permanent (2564)". The Enchant ID is 2564.
        /// </summary>
        public int Id;

        /// <summary>
        /// The name of the enchant.
        /// </summary>
        public string Name;

        /// <summary>
        /// A shortened version of the enchant's Name.
        /// </summary>
        public string ShortName
        {
            get
            {
                string shortName = Name.Replace("Arcanum of the ", "").Replace("Arcanum of ", "").Replace("Inscription of the ", "")
                    .Replace("Inscription of ", "").Replace("Greater", "Great").Replace("Exceptional", "Excep").Replace("Defense", "Def")
                    .Replace("Armor Kit", "").Replace("Arcanum of ", "").Replace(" Leg Armor", "").Replace(" Scope", "")
					.Replace(" Spellthread", "").Replace("Lining - ", "").Replace("Strength", "Str").Replace("Agility", "Agi")
					.Replace("Stamina", "Sta").Replace("Intellect", "Int").Replace("Spirit", "Spr").Replace("Heavy", "Hvy")
					.Replace("Jormungar Leg Reinforcements", "Jorm Reinf").Replace(" Leg Reinforcements", "")
					.Replace("Powerful", "Power").Replace("Swiftness", "Swift").Replace("Knothide", "Knot")
					.Replace("Savage", "Sav").Replace("Mighty Armor", "Mighty Arm").Replace("Shadow Armor", "Shadow Arm")
					.Replace("Attack Power", "AP").Replace("Rune of the ", "").Replace(" Gargoyle", "")
					.Replace("speed Accelerators", "").Replace(" Mysteries", "").Replace(" Embroidery", "")
					.Replace("Mana Restoration", "Mp5").Replace("Restore Mana", "Mp5").Replace("Vengeance", "Veng.")
                    .Replace("Reticulated Armor ", "").Replace("Titanium Weapon Chain", "Titnm.W.Chn");
                return shortName.Substring(0, Math.Min(shortName.Length, 12));
            }
        }

        public string ReallyShortName
        {
            get
            {
                if (Id == 0) return "None";
                string[] split = Name.Split(' ');
                if (split.Length > 1) 
                {
                    string ret = "";
                    foreach (string s in split) { ret += s.Substring(0, 1); }
                    return ret;
                }
                return Name.Substring(0, 5);
            }
        }

        /// <summary>
        /// The slot that the enchant is applied to. If the enchant is available on multiple slots,
        /// define the enchant multiple times, once for each slot.
        /// 
        /// IMPORTANT: Shield enchants should be defined 
        /// </summary>
        public ItemSlot Slot = ItemSlot.Head;

        /// <summary>
        /// The stats that the enchant gives the character.
        /// </summary>
        public Stats Stats = new Stats();

        private static EnchantList _allEnchants;

#if !RAWR3
        private static readonly string _SaveFilePath;

        static Enchant()
        {
            _SaveFilePath = 
                Path.Combine(
                    Path.Combine(
                        Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), 
                        "Data"),
                    "EnchantCache.xml");
            LoadEnchants(); // moved to formmain constructor
            //SaveEnchants();
        }
#endif

        public Enchant() { }
        /// <summary>
        /// Creates a new Enchant, representing an enchant to a single slot.
        /// 
        /// EXAMPLE:
        /// new Enchant(2564, "Superior Agility", ItemSlot.Hands, new Stats() { Agility = 15 })
        /// </summary>
        /// <param name="id">The Enchant ID for the enchant. See the Id property for details of how to find this.</param>
        /// <param name="name">The name of the enchant.</param>
        /// <param name="slot">The slot that this instance of the enchant applies to. (Create multiple Enchant
        /// objects for enchants which may be applied to multiple slots)</param>
        /// <param name="stats">The stats that the enchant gives the character.</param>
        public Enchant(int id, string name, ItemSlot slot, Stats stats)
        {
            Id = id;
            Name = name;
            Slot = slot;
            Stats = stats;
        }

        public override string ToString()
        {
            string summary = Name + ": ";
            summary += Stats.ToString();
            summary = summary.TrimEnd(',', ' ', ':');
            return summary;
        }

        public override bool Equals(object obj)
        {
            Enchant temp = obj as Enchant;
            if (temp != null)
            {
                return Name.Equals(temp.Name) && Id == temp.Id && Slot == temp.Slot && Stats.Equals(temp.Stats);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (Id << 5) | (int)Slot;
        }

        public bool FitsInSlot(ItemSlot slot)
        {
            return (Slot == slot ||
                (Slot == ItemSlot.OneHand && (slot == ItemSlot.OffHand || slot == ItemSlot.MainHand || slot == ItemSlot.TwoHand)) ||
                (Slot == ItemSlot.TwoHand && (slot == ItemSlot.MainHand)));
        }

        public bool FitsInSlot(ItemSlot slot, Character character)
        {
            return Calculations.EnchantFitsInSlot(this, character, slot);
        }

        /// <summary>
        /// A List<Enchant> containing all known enchants relevant to all models.
        /// </summary>
        public static List<Enchant> AllEnchants
        {
            get
            {
                return _allEnchants;
            }
        }

        public static Enchant FindEnchant(int id, ItemSlot slot, Character character)
        {
            //List<ItemSlot> validSlots = new List<ItemSlot>();
            //if (slot != ItemSlot.MainHand)
            //    validSlots.Add(slot);
            //if (slot == ItemSlot.OffHand || slot == ItemSlot.MainHand || slot == ItemSlot.TwoHand)
            //    validSlots.Add(ItemSlot.OneHand);
            //if (slot == ItemSlot.MainHand)
            //    validSlots.Add(ItemSlot.TwoHand);
            return AllEnchants.Find(new Predicate<Enchant>(delegate(Enchant enchant)
            {
                return (enchant.Id == id) && (enchant.FitsInSlot(slot, character) ||
                  (enchant.Slot == ItemSlot.TwoHand && slot == ItemSlot.OneHand));
            })) ?? AllEnchants[0];
        }

        public static List<Enchant> FindEnchants(ItemSlot slot, Character character)
        {
            return FindEnchants(slot, character, Calculations.Instance);
        }

        public static List<Enchant> FindEnchants(ItemSlot slot, Character character, CalculationsBase model)
        {
            //List<ItemSlot> validSlots = new List<ItemSlot>();
            //if (slot != ItemSlot.MainHand)
            //    validSlots.Add(slot);
            //if (slot == ItemSlot.OffHand || slot == ItemSlot.MainHand || slot == ItemSlot.TwoHand)
            //    validSlots.Add(ItemSlot.OneHand);
            //if (slot == ItemSlot.MainHand)
            //    validSlots.Add(ItemSlot.TwoHand);
            return AllEnchants.FindAll(new Predicate<Enchant>(
                delegate(Enchant enchant)
                {
                    return enchant.Slot == ItemSlot.None ||
                        model.IsEnchantRelevant(enchant, character) &&
                        (slot == ItemSlot.None || enchant.FitsInSlot(slot, character));
                }
            ));
        }

        public static List<Enchant> FindAllEnchants(ItemSlot slot, Character character)
        {
            //List<ItemSlot> validSlots = new List<ItemSlot>();
            //if (slot != ItemSlot.MainHand)
            //    validSlots.Add(slot);
            //if (slot == ItemSlot.OffHand || slot == ItemSlot.MainHand || slot == ItemSlot.TwoHand)
            //    validSlots.Add(ItemSlot.OneHand);
            //if (slot == ItemSlot.MainHand)
            //    validSlots.Add(ItemSlot.TwoHand);
            return AllEnchants.FindAll(new Predicate<Enchant>(
                delegate(Enchant enchant)
                {
                    return enchant.Slot == ItemSlot.None ||
                        (slot == ItemSlot.None || enchant.FitsInSlot(slot, character));
                }
            ));
        }

        public static List<Enchant> FindEnchants(ItemSlot slot, Character character, List<string> availableIds)
        {
            return FindEnchants(slot, character, availableIds, Calculations.Instance);
        }

        public static List<Enchant> FindEnchants(ItemSlot slot, Character character, List<string> availableIds, CalculationsBase model)
        {
            return AllEnchants.FindAll(new Predicate<Enchant>(
                delegate(Enchant enchant)
                {
                    return enchant.Id == 0 || 
                        ((enchant.Slot == ItemSlot.None || 
                        model.IsEnchantRelevant(enchant, character) && (slot == ItemSlot.None || enchant.FitsInSlot(slot, character)))
                        && availableIds.Contains((-1 * (enchant.Id + (10000 * (int)enchant.Slot))).ToString()));
                }
            ));
        }

        public static List<Enchant> FindEnchants(ItemSlot slot, Character[] characters, List<string> availableIds, CalculationsBase[] models)
        {
            return AllEnchants.FindAll(new Predicate<Enchant>(
                delegate(Enchant enchant)
                {
                    bool isRelevant = false;
                    for (int i = 0; i < models.Length; i++)
                    {
                        if (models[i].IsEnchantRelevant(enchant, characters[i]) && (enchant.FitsInSlot(slot, characters[i]) || slot == ItemSlot.None))
                        {
                            isRelevant = true;
                            break;
                        }
                    }
                    return ((isRelevant || enchant.Slot == ItemSlot.None)
                        && availableIds.Contains((-1 * (enchant.Id + (10000 * (int)enchant.Slot))).ToString()))
                        || enchant.Id == 0;
                }
            ));
        }

#if RAWR3
        public static void Save(TextWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(EnchantList));
            serializer.Serialize(writer, _allEnchants);
            writer.Close();
        }

        public static void Load(TextReader reader)
        {
            _allEnchants = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(EnchantList));
                _allEnchants = (EnchantList)serializer.Deserialize(reader);
                reader.Close();
            }
            catch { }
            finally
            {
                reader.Close();
                _allEnchants = _allEnchants ?? new EnchantList();
            }
#else
        public static void SaveEnchants()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_SaveFilePath, false, Encoding.UTF8))
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(EnchantList));
                    serializer.Serialize(writer, _allEnchants);
                    writer.Close();
                }
            }
            catch { }
        }

        public static void LoadEnchants()
        {

            try
            {
                if (File.Exists(_SaveFilePath))
                {
                    string xml = System.IO.File.ReadAllText(_SaveFilePath);
                    xml = xml.Replace("<Slot>Weapon</Slot", "<Slot>MainHand</Slot>");
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(EnchantList));
                    System.IO.StringReader reader = new System.IO.StringReader(xml);
                    _allEnchants = (EnchantList)serializer.Deserialize(reader);
                    reader.Close();
                }
            }
            catch (Exception) { /* should ignore exception if there is a problem with the file */ }
            finally
            {
                if (_allEnchants == null)
                {
                    _allEnchants = new EnchantList();
                }
            }
#endif
            List<Enchant> defaultEnchants = GetDefaultEnchants();
            for (int defaultEnchantIndex = 0; defaultEnchantIndex < defaultEnchants.Count; defaultEnchantIndex++)
            {
                bool found = false;
                for (int allEnchantIndex = 0; allEnchantIndex < _allEnchants.Count; allEnchantIndex++)
                {
                    if (defaultEnchants[defaultEnchantIndex].Id == _allEnchants[allEnchantIndex].Id &&
                        defaultEnchants[defaultEnchantIndex].Slot == _allEnchants[allEnchantIndex].Slot &&
                        defaultEnchants[defaultEnchantIndex].Name == _allEnchants[allEnchantIndex].Name
                        )
                    {
                        if (defaultEnchants[defaultEnchantIndex].Stats != _allEnchants[allEnchantIndex].Stats)
                        {
                            if (defaultEnchants[defaultEnchantIndex].Stats == null)
                            {
                                _allEnchants.RemoveAt(allEnchantIndex);
                            }
                            else
                            {
                                _allEnchants[allEnchantIndex].Stats = defaultEnchants[defaultEnchantIndex].Stats;
                            }
                        }
                        found = true;
                        break;
                    }
                }
                if (!found && defaultEnchants[defaultEnchantIndex].Stats != null)
                {
                    _allEnchants.Add(defaultEnchants[defaultEnchantIndex]);
                }
            }
        }

        private static List<Enchant> GetDefaultEnchants()
        {
            List<Enchant> defaultEnchants = new List<Enchant>();
            defaultEnchants.Add(new Enchant(0, "No Enchant", ItemSlot.None, new Stats()));
            defaultEnchants.Add(new Enchant(2999, "Arcanum of the Defender", ItemSlot.Head, new Stats() { DefenseRating = 16, DodgeRating = 17 }));
            defaultEnchants.Add(new Enchant(3818, "Arcanum of the Stalwart Protector", ItemSlot.Head, new Stats() { Stamina = 37, DefenseRating = 20 }));
            defaultEnchants.Add(new Enchant(2991, "Greater Inscription of the Knight", ItemSlot.Shoulders, new Stats() { DefenseRating = 15, DodgeRating = 10 }));
            defaultEnchants.Add(new Enchant(2990, "Inscription of the Knight", ItemSlot.Shoulders, new Stats() { DefenseRating = 13 }));
            defaultEnchants.Add(new Enchant(2978, "Greater Inscription of Warding", ItemSlot.Shoulders, new Stats() { DodgeRating = 15, DefenseRating = 10 }));
            defaultEnchants.Add(new Enchant(2977, "Inscription of Warding", ItemSlot.Shoulders, new Stats() { DodgeRating = 13 }));
            defaultEnchants.Add(new Enchant(368, "Greater Agility", ItemSlot.Back, new Stats() { Agility = 12 }));
            defaultEnchants.Add(new Enchant(983, "Superior Agility", ItemSlot.Back, new Stats() { Agility = 16 }));
            defaultEnchants.Add(new Enchant(1099, "Major Agility", ItemSlot.Back, new Stats() { Agility = 22 }));
            defaultEnchants.Add(new Enchant(3256, "Shadow Armor", ItemSlot.Back, new Stats() { Agility = 10 }));
            defaultEnchants.Add(new Enchant(3825, "Speed", ItemSlot.Back, new Stats() { HasteRating = 15 }));
            defaultEnchants.Add(new Enchant(3831, "Greater Speed", ItemSlot.Back, new Stats() { HasteRating = 23 }));
            defaultEnchants.Add(new Enchant(2662, "Major Armor", ItemSlot.Back, new Stats() { BonusArmor = 120 }));
            defaultEnchants.Add(new Enchant(3294, "Mighty Armor", ItemSlot.Back, new Stats() { BonusArmor = 225 }));
            defaultEnchants.Add(new Enchant(2622, "Dodge", ItemSlot.Back, new Stats() { DodgeRating = 12 }));
            defaultEnchants.Add(new Enchant(2659, "Exceptional Health", ItemSlot.Chest, new Stats() { Health = 150 }));
            defaultEnchants.Add(new Enchant(3236, "Mighty Health", ItemSlot.Chest, new Stats() { Health = 200 }));
            defaultEnchants.Add(new Enchant(3297, "Super Health", ItemSlot.Chest, new Stats() { Health = 275 }));
            defaultEnchants.Add(new Enchant(2661, "Exceptional Stats", ItemSlot.Chest, new Stats() { Agility = 6, Strength = 6, Stamina = 6, Intellect = 6, Spirit = 6 }));
            defaultEnchants.Add(new Enchant(3252, "Super Stats", ItemSlot.Chest, new Stats() { Agility = 8, Strength = 8, Stamina = 8, Intellect = 8, Spirit = 8 }));
            defaultEnchants.Add(new Enchant(3832, "Powerful Stats", ItemSlot.Chest, new Stats() { Agility = 10, Strength = 10, Stamina = 10, Intellect = 10, Spirit = 10 }));
            defaultEnchants.Add(new Enchant(2933, "Major Resilience", ItemSlot.Chest, new Stats() { Resilience = 15 }));
            defaultEnchants.Add(new Enchant(3245, "Exceptional Resilience", ItemSlot.Chest, new Stats() { Resilience = 20 }));
            defaultEnchants.Add(new Enchant(1950, "Major Defense", ItemSlot.Chest, new Stats() { DefenseRating = 15 }));
            defaultEnchants.Add(new Enchant(1953, "Greater Defense", ItemSlot.Chest, new Stats() { DefenseRating = 22 }));
            defaultEnchants.Add(new Enchant(2649, "Fortitude", ItemSlot.Wrist, new Stats() { Stamina = 12 }));
            defaultEnchants.Add(new Enchant(1886, "Superior Stamina", ItemSlot.Wrist, new Stats() { Stamina = 9 }));
            defaultEnchants.Add(new Enchant(2648, "Major Defense", ItemSlot.Wrist, new Stats() { DefenseRating = 12 }));
            defaultEnchants.Add(new Enchant(1891, "Stats", ItemSlot.Wrist, new Stats() { Agility = 4, Strength = 4, Stamina = 4, Intellect = 4, Spirit = 4 }));
            defaultEnchants.Add(new Enchant(2661, "Greater Stats", ItemSlot.Wrist, new Stats() { Agility = 6, Strength = 6, Stamina = 6, Intellect = 6, Spirit = 6 }));
            defaultEnchants.Add(new Enchant(2564, "Superior Agility", ItemSlot.Hands, new Stats() { Agility = 15 }));
            defaultEnchants.Add(new Enchant(3222, "Major Agility", ItemSlot.Hands, new Stats() { Agility = 20 }));
            defaultEnchants.Add(new Enchant(3011, "Clefthide Leg Armor", ItemSlot.Legs, new Stats() { Agility = 10, Stamina = 30 }));
            defaultEnchants.Add(new Enchant(3013, "Nethercleft Leg Armor", ItemSlot.Legs, new Stats() { Agility = 12, Stamina = 40 }));
            defaultEnchants.Add(new Enchant(3325, "Jormungar Leg Armor", ItemSlot.Legs, new Stats() { Agility = 15, Stamina = 45 }));
            defaultEnchants.Add(new Enchant(3822, "Frosthide Leg Armor", ItemSlot.Legs, new Stats() { Agility = 22, Stamina = 55 }));
            defaultEnchants.Add(new Enchant(2939, "Cat's Swiftness", ItemSlot.Feet, new Stats() { Agility = 6, MovementSpeed = 0.08f }));
            defaultEnchants.Add(new Enchant(2940, "Boar's Speed", ItemSlot.Feet, new Stats() { Stamina = 9, MovementSpeed = 0.08f }));
            defaultEnchants.Add(new Enchant(3232, "Tuskarr's Vitality", ItemSlot.Feet, new Stats() { Stamina = 15, MovementSpeed = 0.08f }));
            defaultEnchants.Add(new Enchant(2657, "Dexterity", ItemSlot.Feet, new Stats() { Agility = 12 }));
            defaultEnchants.Add(new Enchant(983, "Superior Agility", ItemSlot.Feet, new Stats() { Agility = 16 }));
            defaultEnchants.Add(new Enchant(3824, "Assault", ItemSlot.Feet, new Stats() { AttackPower = 24 }));
            defaultEnchants.Add(new Enchant(1597, "Greater Assault", ItemSlot.Feet, new Stats() { AttackPower = 32 }));
            defaultEnchants.Add(new Enchant(2649, "Fortitude", ItemSlot.Feet, new Stats() { Stamina = 12 }));
            defaultEnchants.Add(new Enchant(1075, "Greater Fortitude", ItemSlot.Feet, new Stats() { Stamina = 22 }));
            defaultEnchants.Add(new Enchant(2931, "Stats", ItemSlot.Finger, new Stats() { Agility = 4, Strength = 4, Stamina = 4, Intellect = 4, Spirit = 4 }));
            defaultEnchants.Add(new Enchant(3791, "Stamina", ItemSlot.Finger, new Stats() { Stamina = 30 }));
            defaultEnchants.Add(new Enchant(2670, "Major Agility", ItemSlot.TwoHand, new Stats() { Agility = 35 }));
            defaultEnchants.Add(new Enchant(2792, "Knothide Armor Kit", ItemSlot.Chest, new Stats() { Stamina = 8 }));
            defaultEnchants.Add(new Enchant(2792, "Knothide Armor Kit", ItemSlot.Legs, new Stats() { Stamina = 8 }));
            defaultEnchants.Add(new Enchant(2792, "Knothide Armor Kit", ItemSlot.Hands, new Stats() { Stamina = 8 }));
            defaultEnchants.Add(new Enchant(2792, "Knothide Armor Kit", ItemSlot.Feet, new Stats() { Stamina = 8 }));
            defaultEnchants.Add(new Enchant(3329, "Borean Armor Kit", ItemSlot.Chest, new Stats() { Stamina = 12 }));
            defaultEnchants.Add(new Enchant(3329, "Borean Armor Kit", ItemSlot.Legs, new Stats() { Stamina = 12 }));
            defaultEnchants.Add(new Enchant(3329, "Borean Armor Kit", ItemSlot.Hands, new Stats() { Stamina = 12 }));
            defaultEnchants.Add(new Enchant(3329, "Borean Armor Kit", ItemSlot.Feet, new Stats() { Stamina = 12 }));
            defaultEnchants.Add(new Enchant(3330, "Heavy Borean Armor Kit", ItemSlot.Chest, new Stats() { Stamina = 18 }));
            defaultEnchants.Add(new Enchant(3330, "Heavy Borean Armor Kit", ItemSlot.Legs, new Stats() { Stamina = 18 }));
            defaultEnchants.Add(new Enchant(3330, "Heavy Borean Armor Kit", ItemSlot.Hands, new Stats() { Stamina = 18 }));
            defaultEnchants.Add(new Enchant(3330, "Heavy Borean Armor Kit", ItemSlot.Feet, new Stats() { Stamina = 18 }));
            defaultEnchants.Add(new Enchant(3330, "Heavy Borean Armor Kit", ItemSlot.Head, new Stats() { Stamina = 18 }));
            defaultEnchants.Add(new Enchant(3330, "Heavy Borean Armor Kit", ItemSlot.Shoulders, new Stats() { Stamina = 18 }));
            defaultEnchants.Add(new Enchant(2793, "Vindicator's Armor Kit", ItemSlot.Chest, new Stats() { DefenseRating = 8 }));
            defaultEnchants.Add(new Enchant(2793, "Vindicator's Armor Kit", ItemSlot.Hands, new Stats() { DefenseRating = 8 }));
            defaultEnchants.Add(new Enchant(2793, "Vindicator's Armor Kit", ItemSlot.Legs, new Stats() { DefenseRating = 8 }));
            defaultEnchants.Add(new Enchant(2793, "Vindicator's Armor Kit", ItemSlot.Feet, new Stats() { DefenseRating = 8 }));
            defaultEnchants.Add(new Enchant(2841, "Heavy Knothide Armor Kit", ItemSlot.Head, new Stats() { Stamina = 10 }));
            defaultEnchants.Add(new Enchant(2841, "Heavy Knothide Armor Kit", ItemSlot.Chest, new Stats() { Stamina = 10 }));
            defaultEnchants.Add(new Enchant(2841, "Heavy Knothide Armor Kit", ItemSlot.Shoulders, new Stats() { Stamina = 10 }));
            defaultEnchants.Add(new Enchant(2841, "Heavy Knothide Armor Kit", ItemSlot.Legs, new Stats() { Stamina = 10 }));
            defaultEnchants.Add(new Enchant(2841, "Heavy Knothide Armor Kit", ItemSlot.Hands, new Stats() { Stamina = 10 }));
            defaultEnchants.Add(new Enchant(2841, "Heavy Knothide Armor Kit", ItemSlot.Feet, new Stats() { Stamina = 10 }));
            defaultEnchants.Add(new Enchant(3260, "Glove Reinforcements", ItemSlot.Hands, new Stats() { BonusArmor = 240 }));
            defaultEnchants.Add(new Enchant(1441, "Greater Shadow Resistance", ItemSlot.Back, new Stats() { ShadowResistance = 15 }));

            defaultEnchants.Add(new Enchant(2543, "Arcanum of Rapidity", ItemSlot.Head, new Stats() { HasteRating = 10 }));
            defaultEnchants.Add(new Enchant(3003, "Arcanum of Ferocity", ItemSlot.Head, new Stats() { AttackPower = 34, HitRating = 16 }));
            defaultEnchants.Add(new Enchant(3817, "Arcanum of Torment", ItemSlot.Head, new Stats() { AttackPower = 50, CritRating = 20 }));
            defaultEnchants.Add(new Enchant(3096, "Arcanum of the Outcast", ItemSlot.Head, new Stats() { Strength = 17, Intellect = 16 }));
            defaultEnchants.Add(new Enchant(2716, "Fortitude of the Scourge", ItemSlot.Shoulders, new Stats() { Stamina = 16, BonusArmor = 100 }));
            defaultEnchants.Add(new Enchant(2717, "Might of the Scourge", ItemSlot.Shoulders, new Stats() { AttackPower = 26, CritRating = 14 }));
            defaultEnchants.Add(new Enchant(2583, "Presence of Might", ItemSlot.Head, new Stats() { Stamina = 10, DefenseRating = 10, BlockValue = 30 }));
            defaultEnchants.Add(new Enchant(2583, "Presence of Might", ItemSlot.Legs, new Stats() { Stamina = 10, DefenseRating = 10, BlockValue = 30 }));
            defaultEnchants.Add(new Enchant(2997, "Greater Inscription of the Blade", ItemSlot.Shoulders, new Stats() { AttackPower = 20, CritRating = 15 }));
            defaultEnchants.Add(new Enchant(2986, "Greater Inscription of Vengeance", ItemSlot.Shoulders, new Stats() { AttackPower = 30, CritRating = 10 }));
            defaultEnchants.Add(new Enchant(2996, "Inscription of the Blade", ItemSlot.Shoulders, new Stats() { CritRating = 13 }));
            defaultEnchants.Add(new Enchant(2983, "Inscription of Vengeance", ItemSlot.Shoulders, new Stats() { AttackPower = 26 }));
            defaultEnchants.Add(new Enchant(2606, "Zandalar Signet of Might", ItemSlot.Shoulders, new Stats() { AttackPower = 30 }));
            defaultEnchants.Add(new Enchant(2647, "Brawn", ItemSlot.Wrist, new Stats() { Strength = 12 }));
            defaultEnchants.Add(new Enchant(684, "Major Strength", ItemSlot.Hands, new Stats() { Strength = 15 }));
            defaultEnchants.Add(new Enchant(931, "Minor Haste", ItemSlot.Hands, new Stats() { HasteRating = 10 }));
            defaultEnchants.Add(new Enchant(3012, "Nethercobra Leg Armor", ItemSlot.Legs, new Stats() { AttackPower = 50, CritRating = 12 }));
            defaultEnchants.Add(new Enchant(3010, "Cobrahide Leg Armor", ItemSlot.Legs, new Stats() { AttackPower = 40, CritRating = 10 }));
            defaultEnchants.Add(new Enchant(3823, "Icescale Leg Armor", ItemSlot.Legs, new Stats() { AttackPower = 75, CritRating = 22 }));
            defaultEnchants.Add(new Enchant(3326, "Nerubian Leg Armor", ItemSlot.Legs, new Stats() { AttackPower = 55, CritRating = 15 }));
            defaultEnchants.Add(new Enchant(2658, "Surefooted", ItemSlot.Feet, new Stats() { CritRating = 10, HitRating = 10 }));
            defaultEnchants.Add(new Enchant(3826, "Icewalker", ItemSlot.Feet, new Stats() { CritRating = 12, HitRating = 12 }));
            defaultEnchants.Add(new Enchant(2929, "Striking", ItemSlot.Finger, new Stats() { WeaponDamage = 2 }));
            defaultEnchants.Add(new Enchant(3839, "Assault", ItemSlot.Finger, new Stats() { AttackPower = 40 }));
            defaultEnchants.Add(new Enchant(2667, "Savagery", ItemSlot.TwoHand, new Stats() { AttackPower = 70 }));
            defaultEnchants.Add(new Enchant(3828, "Greater Savagery", ItemSlot.TwoHand, new Stats() { AttackPower = 85 }));
            defaultEnchants.Add(new Enchant(3827, "Massacre", ItemSlot.TwoHand, new Stats() { AttackPower = 110 }));
            defaultEnchants.Add(new Enchant(1593, "Assault", ItemSlot.Wrist, new Stats() { AttackPower = 24 }));
            defaultEnchants.Add(new Enchant(1600, "Striking", ItemSlot.Wrist, new Stats() { AttackPower = 38 }));
            defaultEnchants.Add(new Enchant(3845, "Greater Assault", ItemSlot.Wrist, new Stats() { AttackPower = 50 }));
            defaultEnchants.Add(new Enchant(3231, "Expertise", ItemSlot.Wrist, new Stats() { ExpertiseRating = 15 }));
            defaultEnchants.Add(new Enchant(3829, "Greater Assault", ItemSlot.Hands, new Stats() { AttackPower = 35 }));
            defaultEnchants.Add(new Enchant(1594, "Assault", ItemSlot.Hands, new Stats() { AttackPower = 26 }));
            defaultEnchants.Add(new Enchant(1603, "Crusher", ItemSlot.Hands, new Stats() { AttackPower = 44 }));
            defaultEnchants.Add(new Enchant(3231, "Expertise", ItemSlot.Hands, new Stats() { ExpertiseRating = 15 }));
            defaultEnchants.Add(new Enchant(3222, "Greater Agility", ItemSlot.OneHand, new Stats() { Agility = 20 }));
            defaultEnchants.Add(new Enchant(1103, "Exceptional Agility", ItemSlot.OneHand, new Stats() { Agility = 26 }));
            defaultEnchants.Add(new Enchant(2668, "Potency", ItemSlot.OneHand, new Stats() { Strength = 20 }));
            defaultEnchants.Add(new Enchant(1606, "Greater Potency", ItemSlot.OneHand, new Stats() { AttackPower = 50 }));
            defaultEnchants.Add(new Enchant(3833, "Superior Potency", ItemSlot.OneHand, new Stats() { AttackPower = 65 }));
            defaultEnchants.Add(new Enchant(3731, "Titanium Weapon Chain", ItemSlot.OneHand, new Stats() { HitRating = 28 }));
            defaultEnchants.Add(new Enchant(3223, "Adamantite Weapon Chain", ItemSlot.OneHand, new Stats() { ParryRating = 15 }));
            defaultEnchants.Add(new Enchant(2722, "Adamantite Scope", ItemSlot.Ranged, new Stats() { ScopeDamage = 10 }));
            defaultEnchants.Add(new Enchant(2723, "Khorium Scope", ItemSlot.Ranged, new Stats() { ScopeDamage = 12 }));
            defaultEnchants.Add(new Enchant(3843, "Diamond-cut Refractor Scope", ItemSlot.Ranged, new Stats() { ScopeDamage = 15 }));
            defaultEnchants.Add(new Enchant(3608, "Heartseeker Scope", ItemSlot.Ranged, new Stats() { RangedCritRating = 40 }));
            defaultEnchants.Add(new Enchant(2724, "Stabilized Eternium Scope", ItemSlot.Ranged, new Stats() { RangedCritRating = 28 }));
            defaultEnchants.Add(new Enchant(3607, "Sun Scope", ItemSlot.Ranged, new Stats() { RangedHasteRating = 40 }));

            defaultEnchants.Add(new Enchant(2621, "Subtlety", ItemSlot.Back, new Stats() { ThreatReductionMultiplier = 0.02f }));
            defaultEnchants.Add(new Enchant(3296, "Wisdom", ItemSlot.Back, new Stats() { ThreatReductionMultiplier = 0.02f, Spirit = 10 }));
            defaultEnchants.Add(new Enchant(2613, "Threat", ItemSlot.Hands, new Stats() { ThreatIncreaseMultiplier = 0.02f }));
            defaultEnchants.Add(new Enchant(3253, "Armsman", ItemSlot.Hands, new Stats() { ThreatIncreaseMultiplier = 0.02f, ParryRating = 10 }));

            //spell stuff
            defaultEnchants.Add(new Enchant(2650, "Spellpower", ItemSlot.Wrist, new Stats() { SpellPower = 15 }));
            defaultEnchants.Add(new Enchant(2332, "Superior Spellpower", ItemSlot.Wrist, new Stats() { SpellPower = 30 }));
            defaultEnchants.Add(new Enchant(2326, "Greater Spellpower", ItemSlot.Wrist, new Stats() { SpellPower = 23 }));
            defaultEnchants.Add(new Enchant(2937, "Major Spellpower", ItemSlot.Hands, new Stats() { SpellPower = 20 }));
            defaultEnchants.Add(new Enchant(3246, "Exceptional Spellpower", ItemSlot.Hands, new Stats() { SpellPower = 28 }));
            defaultEnchants.Add(new Enchant(2935, "Precise Strikes", ItemSlot.Hands, new Stats() { HitRating = 15 }));
            defaultEnchants.Add(new Enchant(3234, "Precision", ItemSlot.Hands, new Stats() { HitRating = 20 }));
            defaultEnchants.Add(new Enchant(2928, "Spellpower", ItemSlot.Finger, new Stats() { SpellPower = 12 }));
            defaultEnchants.Add(new Enchant(3840, "Greater Spellpower", ItemSlot.Finger, new Stats() { SpellPower = 23 }));
            defaultEnchants.Add(new Enchant(2669, "Major Spellpower", ItemSlot.OneHand, new Stats() { SpellPower = 40 }));
            defaultEnchants.Add(new Enchant(3830, "Exceptional Spellpower", ItemSlot.OneHand, new Stats() { SpellPower = 50 }));
            defaultEnchants.Add(new Enchant(3834, "Mighty Spellpower", ItemSlot.OneHand, new Stats() { SpellPower = 63 }));
            defaultEnchants.Add(new Enchant(3788, "Accuracy", ItemSlot.OneHand, new Stats() { CritRating = 25, HitRating = 25 }));
            defaultEnchants.Add(new Enchant(2666, "Major Intellect", ItemSlot.OneHand, new Stats() { Intellect = 30 }));
            defaultEnchants.Add(new Enchant(3844, "Exceptional Spirit", ItemSlot.OneHand, new Stats() { Spirit = 45 }));
            defaultEnchants.Add(new Enchant(2679, "Restore Mana Prime", ItemSlot.Wrist, new Stats() { Mp5 = 8 }));
            defaultEnchants.Add(new Enchant(1147, "Major Spirit", ItemSlot.Wrist, new Stats() { Spirit = 18 }));
            defaultEnchants.Add(new Enchant(3872, "Sanctified Spellthread", ItemSlot.Legs, new Stats() { SpellPower = 50, Spirit = 20 }));
            defaultEnchants.Add(new Enchant(2748, "Runic Spellthread", ItemSlot.Legs, new Stats() { SpellPower = 35, Stamina = 20 }));
            defaultEnchants.Add(new Enchant(2747, "Mystic Spellthread", ItemSlot.Legs, new Stats() { SpellPower = 25, Stamina = 15 }));
            defaultEnchants.Add(new Enchant(3719, "Brilliant Spellthread", ItemSlot.Legs, new Stats() { SpellPower = 50, Spirit = 20 }));
            defaultEnchants.Add(new Enchant(3721, "Sapphire Spellthread", ItemSlot.Legs, new Stats() { SpellPower = 50, Stamina = 30 }));
            defaultEnchants.Add(new Enchant(3718, "Shining Spellthread", ItemSlot.Legs, new Stats() { SpellPower = 35, Spirit = 12 }));
            defaultEnchants.Add(new Enchant(3720, "Azure Spellthread", ItemSlot.Legs, new Stats() { SpellPower = 35, Stamina = 20 }));
            defaultEnchants.Add(new Enchant(2982, "Greater Inscription of Discipline", ItemSlot.Shoulders, new Stats() { SpellPower = 18, CritRating = 10 }));
            defaultEnchants.Add(new Enchant(2995, "Greater Inscription of the Orb", ItemSlot.Shoulders, new Stats() { SpellPower = 12, CritRating = 15 }));
            defaultEnchants.Add(new Enchant(2981, "Inscription of Discipline", ItemSlot.Shoulders, new Stats() { SpellPower = 15 }));
            defaultEnchants.Add(new Enchant(2994, "Inscription of the Orb", ItemSlot.Shoulders, new Stats() { CritRating = 13 }));
            defaultEnchants.Add(new Enchant(3002, "Arcanum of Power", ItemSlot.Head, new Stats() { SpellPower = 22, HitRating = 14 }));
            defaultEnchants.Add(new Enchant(3820, "Arcanum of Burning Mysteries", ItemSlot.Head, new Stats() { SpellPower = 30, CritRating = 20 }));
            defaultEnchants.Add(new Enchant(3797, "Arcanum of Dominance", ItemSlot.Head, new Stats() { SpellPower = 29, Resilience = 20 }));
            defaultEnchants.Add(new Enchant(2671, "Sunfire", ItemSlot.OneHand, new Stats() { SpellFireDamageRating = 50, SpellArcaneDamageRating = 50 }));
            defaultEnchants.Add(new Enchant(2672, "Soulfrost", ItemSlot.OneHand, new Stats() { SpellFrostDamageRating = 54, SpellShadowDamageRating = 54 }));
            defaultEnchants.Add(new Enchant(2938, "Spell Penetration", ItemSlot.Back, new Stats() { SpellPenetration = 20 }));
            defaultEnchants.Add(new Enchant(3243, "Spell Piercing", ItemSlot.Back, new Stats() { SpellPenetration = 35 }));
            defaultEnchants.Add(new Enchant(3244, "Greater Vitality", ItemSlot.Feet, new Stats() { Mp5 = 7f, Hp5 = 7f }));
            defaultEnchants.Add(new Enchant(2656, "Vitality", ItemSlot.Feet, new Stats() { Mp5 = 5f, Hp5 = 5f }));
            defaultEnchants.Add(new Enchant(369, "Major Intellect", ItemSlot.Wrist, new Stats() { Intellect = 12 }));
            defaultEnchants.Add(new Enchant(1119, "Exceptional Intellect", ItemSlot.Wrist, new Stats() { Intellect = 16 }));
            defaultEnchants.Add(new Enchant(1144, "Major Spirit", ItemSlot.Chest, new Stats() { Spirit = 15 }));
            defaultEnchants.Add(new Enchant(851, "Spirit", ItemSlot.Feet, new Stats() { Spirit = 5 }));
            defaultEnchants.Add(new Enchant(1147, "Greater Spirit", ItemSlot.Feet, new Stats() { Spirit = 18 }));

            // Healing enchants (add spell damage too)
            defaultEnchants.Add(new Enchant(3001, "Arcanum of Renewal", ItemSlot.Head, new Stats() { SpellPower = 19, Mp5 = 9 }));
            defaultEnchants.Add(new Enchant(3819, "Arcanum of Blissful Mending", ItemSlot.Head, new Stats() { SpellPower = 30, Mp5 = 10 }));

            defaultEnchants.Add(new Enchant(3150, "Restore Mana Prime", ItemSlot.Chest, new Stats() { Mp5 = 7f }));
            defaultEnchants.Add(new Enchant(2381, "Greater Mana Restoration", ItemSlot.Chest, new Stats() { Mp5 = 10f }));
            defaultEnchants.Add(new Enchant(3233, "Exceptional Mana", ItemSlot.Chest, new Stats() { Mana = 250 }));

            defaultEnchants.Add(new Enchant(2980, "Greater Inscription of Faith", ItemSlot.Shoulders, new Stats() { SpellPower = 18, Mp5 = 5 }));
            defaultEnchants.Add(new Enchant(2993, "Greater Inscription of the Oracle", ItemSlot.Shoulders, new Stats() { SpellPower = 12, Mp5 = 8 }));
            defaultEnchants.Add(new Enchant(2979, "Inscription of Faith", ItemSlot.Shoulders, new Stats() { SpellPower = 15 }));
            defaultEnchants.Add(new Enchant(2992, "Inscription of the Oracle", ItemSlot.Shoulders, new Stats() { Mp5 = 6 }));
            defaultEnchants.Add(new Enchant(2604, "Zandalar Signet of Serenity", ItemSlot.Shoulders, new Stats() { SpellPower = 18 }));

            defaultEnchants.Add(new Enchant(2322, "Major Healing", ItemSlot.Hands, new Stats() { SpellPower = 19 }));
            defaultEnchants.Add(new Enchant(2934, "Blasting", ItemSlot.Hands, new Stats() { CritRating = 10 }));
            defaultEnchants.Add(new Enchant(3249, "Greater Blasting", ItemSlot.Hands, new Stats() { CritRating = 16 }));

            defaultEnchants.Add(new Enchant(2746, "Golden Spellthread", ItemSlot.Legs, new Stats() { SpellPower = 35, Stamina = 20 }));
            defaultEnchants.Add(new Enchant(2745, "Silver Spellthread", ItemSlot.Legs, new Stats() { SpellPower = 25, Stamina = 15 }));

            defaultEnchants.Add(new Enchant(2794, "Magister's Armor Kit", ItemSlot.Chest, new Stats() { Mp5 = 4 }));
            defaultEnchants.Add(new Enchant(2794, "Magister's Armor Kit", ItemSlot.Feet, new Stats() { Mp5 = 4 }));
            defaultEnchants.Add(new Enchant(2794, "Magister's Armor Kit", ItemSlot.Legs, new Stats() { Mp5 = 4 }));
            defaultEnchants.Add(new Enchant(2794, "Magister's Armor Kit", ItemSlot.Hands, new Stats() { Mp5 = 4 }));

            defaultEnchants.Add(new Enchant(2930, "Healing Power", ItemSlot.Finger, new Stats() { SpellPower = 12 }));

            defaultEnchants.Add(new Enchant(2343, "Major Healing", ItemSlot.OneHand, new Stats() { SpellPower = 40 }));

            defaultEnchants.Add(new Enchant(2654, "Intellect", ItemSlot.OffHand, new Stats() { Intellect = 12 }));
            defaultEnchants.Add(new Enchant(1128, "Greater Intellect", ItemSlot.OffHand, new Stats() { Intellect = 25 }));

            defaultEnchants.Add(new Enchant(2617, "Superior Healing", ItemSlot.Wrist, new Stats() { SpellPower = 15 }));

            defaultEnchants.Add(new Enchant(2648, "Steelweave", ItemSlot.Back, new Stats() { DefenseRating = 12 }));
            defaultEnchants.Add(new Enchant(1951, "Titanweave", ItemSlot.Back, new Stats() { DefenseRating = 16 }));
            defaultEnchants.Add(new Enchant(3004, "Arcanum of the Gladiator", ItemSlot.Head, new Stats() { Stamina = 18, Resilience = 20 }));
            defaultEnchants.Add(new Enchant(3842, "Arcanum of the Savage Gladiator", ItemSlot.Head, new Stats() { Stamina = 30, Resilience = 25 }));
            defaultEnchants.Add(new Enchant(3795, "Arcanum of Triumph", ItemSlot.Head, new Stats() { AttackPower = 50, Resilience = 20 }));

            defaultEnchants.Add(new Enchant(3793, "Inscription of Triumph", ItemSlot.Shoulders, new Stats() { AttackPower = 40, Resilience = 15 }));
            defaultEnchants.Add(new Enchant(3794, "Inscription of Dominance", ItemSlot.Shoulders, new Stats() { SpellPower = 23, Resilience = 15 }));

            Stats bladeWard = new Stats();
            bladeWard.AddSpecialEffect(new SpecialEffect(Trigger.MeleeHit, new Stats() { ParryRating = 200f }, 10f, 0f, -1f));
            defaultEnchants.Add(new Enchant(3869, "Blade Ward", ItemSlot.OneHand, bladeWard));

            Stats berserking = new Stats();
            berserking.AddSpecialEffect(new SpecialEffect(Trigger.MeleeHit, new Stats() { AttackPower = 400f, BonusArmorMultiplier = -.05f }, 15f, 0f, -1f));
            defaultEnchants.Add(new Enchant(3789, "Berserking", ItemSlot.OneHand, berserking));

            Stats blackMagic = new Stats();
            blackMagic.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellHit, new Stats() { HasteRating = 250f }, 10f, 35f, 0.35f));
			blackMagic.AddSpecialEffect(new SpecialEffect(Trigger.MangleCatOrShredOrInfectedWoundsHit, new Stats() { HasteRating = 250f }, 10f, 35f, 0.35f));
            defaultEnchants.Add(new Enchant(3790, "Black Magic", ItemSlot.OneHand, blackMagic));

            Stats executioner = new Stats();
            executioner.AddSpecialEffect(new SpecialEffect(Trigger.MeleeHit, new Stats() { ArmorPenetrationRating = 120f }, 15f, 0f, -1f));
            defaultEnchants.Add(new Enchant(3225, "Executioner", ItemSlot.OneHand, executioner));
            
            Stats mongoose = new Stats();
            mongoose.AddSpecialEffect(new SpecialEffect(Trigger.MeleeHit, new Stats() { Agility = 120, HasteRating = 30f }, 15f, 0f, -1f));
            defaultEnchants.Add(new Enchant(2673, "Mongoose", ItemSlot.OneHand, mongoose));

			// Once at 35% Health, your melee Hits restores 400 health per stack
            Stats blood_drain_a = new Stats();
            blood_drain_a.AddSpecialEffect(new SpecialEffect(Trigger.MeleeHit, new Stats() { HealthRestore = ( 360f + 440f ) / 2f }, 20f, 0f, 1f, 5));
            Stats blood_drain = new Stats();
            blood_drain.AddSpecialEffect(new SpecialEffect(Trigger.DamageTaken, blood_drain_a, 0f, 0f, .35f));
            defaultEnchants.Add(new Enchant(3870, "Blood Draining", ItemSlot.OneHand, blood_drain));
            
            Stats icebreaker = new Stats();
            icebreaker.AddSpecialEffect(new SpecialEffect(Trigger.MeleeHit,
                new Stats() { // the .85 and .15 are simming spell hits vs spell crits, obsolete with new special dmg proc method
                    //FireDamage = ((209f + 234f) / 2f) * 0.85f + ((317f + 340f) / 2f) * 0.15f,
                    FireDamage = 200f,
                    //ProcdFireDamageMin = 185f,
                    //ProcdFireDamageMax = 215f,
                },
                0f, 0f, -3)); // 3 PPM = 9% Chance to proc
            defaultEnchants.Add(new Enchant(3239, "Icebreaker", ItemSlot.OneHand, icebreaker));

            defaultEnchants.Add(new Enchant(2984, "Shadow Armor Kit", ItemSlot.Hands, new Stats() { ShadowResistance = 8 }));
            defaultEnchants.Add(new Enchant(2984, "Shadow Armor Kit", ItemSlot.Feet, new Stats() { ShadowResistance = 8 }));
            defaultEnchants.Add(new Enchant(2984, "Shadow Armor Kit", ItemSlot.Chest, new Stats() { ShadowResistance = 8 }));
            defaultEnchants.Add(new Enchant(2984, "Shadow Armor Kit", ItemSlot.Legs, new Stats() { ShadowResistance = 8 }));
            defaultEnchants.Add(new Enchant(2985, "Flame Armor Kit", ItemSlot.Hands, new Stats() { FireResistance = 8 }));
            defaultEnchants.Add(new Enchant(2985, "Flame Armor Kit", ItemSlot.Feet, new Stats() { FireResistance = 8 }));
            defaultEnchants.Add(new Enchant(2985, "Flame Armor Kit", ItemSlot.Chest, new Stats() { FireResistance = 8 }));
            defaultEnchants.Add(new Enchant(2985, "Flame Armor Kit", ItemSlot.Legs, new Stats() { FireResistance = 8 }));
            defaultEnchants.Add(new Enchant(2987, "Frost Armor Kit", ItemSlot.Hands, new Stats() { FrostResistance = 8 }));
            defaultEnchants.Add(new Enchant(2987, "Frost Armor Kit", ItemSlot.Feet, new Stats() { FrostResistance = 8 }));
            defaultEnchants.Add(new Enchant(2987, "Frost Armor Kit", ItemSlot.Chest, new Stats() { FrostResistance = 8 }));
            defaultEnchants.Add(new Enchant(2987, "Frost Armor Kit", ItemSlot.Legs, new Stats() { FrostResistance = 8 }));
            defaultEnchants.Add(new Enchant(2988, "Nature Armor Kit", ItemSlot.Hands, new Stats() { NatureResistance = 8 }));
            defaultEnchants.Add(new Enchant(2988, "Nature Armor Kit", ItemSlot.Feet, new Stats() { NatureResistance = 8 }));
            defaultEnchants.Add(new Enchant(2988, "Nature Armor Kit", ItemSlot.Chest, new Stats() { NatureResistance = 8 }));
            defaultEnchants.Add(new Enchant(2988, "Nature Armor Kit", ItemSlot.Legs, new Stats() { NatureResistance = 8 }));
            defaultEnchants.Add(new Enchant(2989, "Arcane Armor Kit", ItemSlot.Hands, new Stats() { ArcaneResistance = 8 }));
            defaultEnchants.Add(new Enchant(2989, "Arcane Armor Kit", ItemSlot.Feet, new Stats() { ArcaneResistance = 8 }));
            defaultEnchants.Add(new Enchant(2989, "Arcane Armor Kit", ItemSlot.Chest, new Stats() { ArcaneResistance = 8 }));
            defaultEnchants.Add(new Enchant(2989, "Arcane Armor Kit", ItemSlot.Legs, new Stats() { ArcaneResistance = 8 }));
            defaultEnchants.Add(new Enchant(3005, "Arcanum of Nature Warding", ItemSlot.Head, new Stats() { NatureResistance = 20 }));
            defaultEnchants.Add(new Enchant(3813, "Arcanum of Toxic Warding", ItemSlot.Head, new Stats() { NatureResistance = 25, Stamina = 30 }));
            defaultEnchants.Add(new Enchant(3006, "Arcanum of Arcane Warding", ItemSlot.Head, new Stats() { ArcaneResistance = 20 }));
            defaultEnchants.Add(new Enchant(3815, "Arcanum of the Eclipsed Moon", ItemSlot.Head, new Stats() { ArcaneResistance = 25, Stamina = 30 }));
            defaultEnchants.Add(new Enchant(3007, "Arcanum of Fire Warding", ItemSlot.Head, new Stats() { FireResistance = 20 }));
            defaultEnchants.Add(new Enchant(3816, "Arcanum of the Flame's Soul", ItemSlot.Head, new Stats() { FireResistance = 25, Stamina = 30 }));
            defaultEnchants.Add(new Enchant(3008, "Arcanum of Frost Warding", ItemSlot.Head, new Stats() { FrostResistance = 20 }));
            defaultEnchants.Add(new Enchant(3812, "Arcanum of the Frosty Soul", ItemSlot.Head, new Stats() { FrostResistance = 25, Stamina = 30 }));
            defaultEnchants.Add(new Enchant(3009, "Arcanum of Shadow Warding", ItemSlot.Head, new Stats() { ShadowResistance = 20 }));
            defaultEnchants.Add(new Enchant(3814, "Arcanum of the Fleeing Shadow", ItemSlot.Head, new Stats() { ShadowResistance = 25, Stamina = 30 }));
            defaultEnchants.Add(new Enchant(2998, "Inscription of Endurance", ItemSlot.Shoulders, new Stats() { NatureResistance = 4, ArcaneResistance = 4, FireResistance = 4, FrostResistance = 4, ShadowResistance = 4 }));
            defaultEnchants.Add(new Enchant(2664, "Major Resistance", ItemSlot.Back, new Stats() { NatureResistance = 7, ArcaneResistance = 7, FireResistance = 7, FrostResistance = 7, ShadowResistance = 7 }));
            defaultEnchants.Add(new Enchant(2619, "Greater Fire Resistance", ItemSlot.Back, new Stats() { FireResistance = 15 }));
            defaultEnchants.Add(new Enchant(1262, "Superior Arcane Resistance", ItemSlot.Back, new Stats() { ArcaneResistance = 20 }));
            defaultEnchants.Add(new Enchant(1446, "Superior Shadow Resistance", ItemSlot.Back, new Stats() { ShadowResistance = 20 }));
            defaultEnchants.Add(new Enchant(1354, "Superior Fire Resistance", ItemSlot.Back, new Stats() { FireResistance = 20 }));
            defaultEnchants.Add(new Enchant(3230, "Superior Frost Resistance", ItemSlot.Back, new Stats() { FrostResistance = 20 }));
            defaultEnchants.Add(new Enchant(1400, "Superior Nature Resistance", ItemSlot.Back, new Stats() { NatureResistance = 20 }));
            defaultEnchants.Add(new Enchant(1505, "Lesser Arcanum of Resilience", ItemSlot.Head, new Stats() { FireResistance = 20 }));
            defaultEnchants.Add(new Enchant(1505, "Lesser Arcanum of Resilience", ItemSlot.Legs, new Stats() { FireResistance = 20 }));

            defaultEnchants.Add(new Enchant(1071, "Major Stamina", ItemSlot.OffHand, new Stats() { Stamina = 18 }));
            defaultEnchants.Add(new Enchant(2655, "Shield Block", ItemSlot.OffHand, new Stats() { BlockRating = 15 }));
            defaultEnchants.Add(new Enchant(2653, "Tough Shield", ItemSlot.OffHand, new Stats() { BlockValue = 36 }));
            defaultEnchants.Add(new Enchant(1952, "Defense", ItemSlot.OffHand, new Stats() { DefenseRating = 20 }));
            defaultEnchants.Add(new Enchant(3229, "Resilience", ItemSlot.OffHand, new Stats() {Resilience = 12 }));

            //scopes
            defaultEnchants.Add(new Enchant(2723, "Khorium Scope", ItemSlot.Ranged, new Stats() { ScopeDamage = 12 }));
            defaultEnchants.Add(new Enchant(2722, "Adamantite Scope", ItemSlot.Ranged, new Stats() { ScopeDamage = 10 }));
            defaultEnchants.Add(new Enchant(2523, "Biznicks 247x128 Accurascope", ItemSlot.Ranged, new Stats() { RangedHitRating = 30 }));
            defaultEnchants.Add(new Enchant(2724, "Stabilized Eternium Scope", ItemSlot.Ranged, new Stats() { RangedCritRating = 28 }));

            //Sapphiron Enchants
            defaultEnchants.Add(new Enchant(2721, "Power of the Scourge", ItemSlot.Shoulders, new Stats() { CritRating = 14, SpellPower = 15 }));
            defaultEnchants.Add(new Enchant(2716, "Fortitude of the Scourge", ItemSlot.Shoulders, new Stats() { Stamina = 16, BonusArmor = 100 }));
            defaultEnchants.Add(new Enchant(2715, "Resilience of the Scourge", ItemSlot.Shoulders, new Stats() { SpellPower = 16, Mp5 = 5 }));
            defaultEnchants.Add(new Enchant(2717, "Might of the Scourge", ItemSlot.Shoulders, new Stats() { CritRating = 14, AttackPower = 26 }));

            //Hodir Enchants
            defaultEnchants.Add(new Enchant(3808, "Greater Inscription of the Axe", ItemSlot.Shoulders, new Stats() { AttackPower = 40, CritRating = 15 }));
            defaultEnchants.Add(new Enchant(3875, "Lesser Inscription of the Axe", ItemSlot.Shoulders, new Stats() { AttackPower = 30, CritRating = 10 }));
            defaultEnchants.Add(new Enchant(3809, "Greater Inscription of the Crag", ItemSlot.Shoulders, new Stats() { SpellPower = 24, Mp5 = 8 }));
            defaultEnchants.Add(new Enchant(3807, "Lesser Inscription of the Crag", ItemSlot.Shoulders, new Stats() { SpellPower = 18, Mp5 = 5 }));
            defaultEnchants.Add(new Enchant(3811, "Greater Inscription of the Pinnacle", ItemSlot.Shoulders, new Stats() { DodgeRating = 20, DefenseRating = 15 }));
            defaultEnchants.Add(new Enchant(3876, "Lesser Inscription of the Pinnacle", ItemSlot.Shoulders, new Stats() { DodgeRating = 15, DefenseRating = 10 }));
            defaultEnchants.Add(new Enchant(3810, "Greater Inscription of the Storm", ItemSlot.Shoulders, new Stats() { SpellPower = 24, CritRating = 15 }));
            defaultEnchants.Add(new Enchant(3806, "Lesser Inscription of the Storm", ItemSlot.Shoulders, new Stats() { SpellPower = 18, CritRating = 10 }));

            //Inscriber enchants
            defaultEnchants.Add(new Enchant(3835, "Master's Inscription of the Axe", ItemSlot.Shoulders, new Stats() { CritRating = 15, AttackPower = 120 }));
            defaultEnchants.Add(new Enchant(3836, "Master's Inscription of the Crag", ItemSlot.Shoulders, new Stats() { Mp5 = 6, SpellPower = 70 }));
            defaultEnchants.Add(new Enchant(3837, "Master's Inscription of the Pinnacle", ItemSlot.Shoulders, new Stats() { DodgeRating = 60, DefenseRating = 15 }));
            defaultEnchants.Add(new Enchant(3838, "Master's Inscription of the Storm", ItemSlot.Shoulders, new Stats() { CritRating = 15, SpellPower = 70 }));

            //Leatherworking enchants
            defaultEnchants.Add(new Enchant(3756, "Fur Lining - Attack Power", ItemSlot.Wrist, new Stats() { AttackPower = 130 }));
            defaultEnchants.Add(new Enchant(3757, "Fur Lining - Stamina", ItemSlot.Wrist, new Stats() { Stamina = 102 }));
            defaultEnchants.Add(new Enchant(3758, "Fur Lining - Spell Power", ItemSlot.Wrist, new Stats() { SpellPower = 76 }));
            defaultEnchants.Add(new Enchant(3759, "Fur Lining - Fire Resist", ItemSlot.Wrist, new Stats() { FireResistance = 70 }));
            defaultEnchants.Add(new Enchant(3760, "Fur Lining - Frost Resist", ItemSlot.Wrist, new Stats() { FrostResistance = 70 }));
            defaultEnchants.Add(new Enchant(3761, "Fur Lining - Shadow Resist", ItemSlot.Wrist, new Stats() { ShadowResistance = 70 }));
            defaultEnchants.Add(new Enchant(3762, "Fur Lining - Nature Resist", ItemSlot.Wrist, new Stats() { NatureResistance = 70 }));
            defaultEnchants.Add(new Enchant(3763, "Fur Lining - Arcane Resist", ItemSlot.Wrist, new Stats() { ArcaneResistance = 70 }));
            defaultEnchants.Add(new Enchant(3327, "Jormungar Leg Reinforcements", ItemSlot.Legs, new Stats() { Stamina = 55, Agility = 22 }));
            defaultEnchants.Add(new Enchant(3328, "Nerubian Leg Reinforcements", ItemSlot.Legs, new Stats() { AttackPower = 75, CritRating = 22 }));


            //Death Knight Rune Enchants
            Stats razorice = new Stats() { BonusFrostWeaponDamage = .02f };
            // Updated Razorice for patch 3.3.3
            razorice.AddSpecialEffect(new SpecialEffect(Trigger.MeleeHit, new Stats() { BonusFrostDamageMultiplier = 0.02f }, 20f, 0f, 1f, 5));
            Stats RotFC = new Stats();
            RotFC.AddSpecialEffect(new SpecialEffect(Trigger.DamageDone, new Stats() { BonusStrengthMultiplier = .15f }, 15f, 0f, -2f, 1));
            RotFC.AddSpecialEffect(new SpecialEffect(Trigger.DamageDone, new Stats() { HealthRestoreFromMaxHealth = .03f }, 0, 0f, -2f, 1));
            Stats Cinderglacier = new Stats();
            Cinderglacier.AddSpecialEffect(new SpecialEffect(Trigger.DamageDone, new Stats() { CinderglacierProc = 2f }, 0f, 0f, -1.5f));
            defaultEnchants.Add(new Enchant(3368, "Rune of the Fallen Crusader", ItemSlot.OneHand, RotFC));
            defaultEnchants.Add(new Enchant(3370, "Rune of Razorice", ItemSlot.OneHand, razorice));
            defaultEnchants.Add(new Enchant(3369, "Rune of Cinderglacier", ItemSlot.OneHand, Cinderglacier));
            defaultEnchants.Add(new Enchant(3365, "Rune of Swordshattering", ItemSlot.TwoHand, new Stats() { Parry = 0.04f }));
            defaultEnchants.Add(new Enchant(3594, "Rune of Swordbreaking", ItemSlot.OneHand, new Stats() { Parry = 0.02f }));
            defaultEnchants.Add(new Enchant(3847, "Rune of the Stoneskin Gargoyle", ItemSlot.TwoHand, new Stats() { Defense = 25.0f, BonusStaminaMultiplier = 0.02f }));
            defaultEnchants.Add(new Enchant(3883, "Rune of the Nerubian Carapace", ItemSlot.OneHand, new Stats() { Defense = 13f, BonusStaminaMultiplier = 0.01f }));

            // Engineering enchant
            Stats hyper = new Stats();
            hyper.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { HasteRating = 340f }, 12f, 60f));
            defaultEnchants.Add(new Enchant(3604, "Hyperspeed Accelerators", ItemSlot.Hands, hyper));
            defaultEnchants.Add(new Enchant(3860, "Reticulated Armor Webbing", ItemSlot.Hands, new Stats() { BonusArmor = 885f }));
            Stats nitro = new Stats() { CritRating = 24f, };
            nitro.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { MovementSpeed = 1.50f, }, 5f, 3*60f));
            defaultEnchants.Add(new Enchant(3606, "Nitro Boosts", ItemSlot.Feet, nitro));

            // Engineering Head enchant
            defaultEnchants.Add(new Enchant(3878, "Mind Amplification Dish", ItemSlot.Head, new Stats() { Stamina = 45f }));
            
            // Engineering cloak enchant
            defaultEnchants.Add(new Enchant(3859, "Springy Arachnoweave", ItemSlot.Back, new Stats() { SpellPower = 27f }));
            defaultEnchants.Add(new Enchant(3605, "Flexweave Underlay", ItemSlot.Back, new Stats() { Agility = 23f }));

            // Tailoring enchant
            Stats stats = new Stats() { Spirit = 1 };
            stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast, new Stats() { SpellPower = 295 }, 15, 60, 0.35f));
            defaultEnchants.Add(new Enchant(3722, "Lightweave Embroidery", ItemSlot.Back, stats));

            stats = new Stats();
            stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalHit, new Stats() { AttackPower = 400 }, 15, 45, 0.25f));
            defaultEnchants.Add(new Enchant(3730, "Swordguard Embroidery", ItemSlot.Back, stats));

            Stats darkglow = new Stats();
            darkglow.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast, new Stats() { ManaRestore = 400 }, 1f, 60f, 0.35f));
            defaultEnchants.Add(new Enchant(3728, "Darkglow Embroidery", ItemSlot.Back, darkglow));

            defaultEnchants.Add(new Enchant(3873, "Master's Spellthread", ItemSlot.Legs, new Stats() { SpellPower = 50f, Stamina = 30f }));

            //3.0.8 enchants
            defaultEnchants.Add(new Enchant(3850, "Major Stamina", ItemSlot.Wrist, new Stats() { Stamina = 40 }));
            defaultEnchants.Add(new Enchant(3849, "Titanium Plating", ItemSlot.OffHand, new Stats() { BlockValue = 81 }));
            defaultEnchants.Add(new Enchant(3852, "Greater Inscription of the Gladiator", ItemSlot.Shoulders, new Stats() { Stamina = 30, Resilience = 15 }));

			//3.1 enchants
			defaultEnchants.Add(new Enchant(3854, "Greater Spellpower (Staff)", ItemSlot.TwoHand, new Stats() { SpellPower = 81 }));
			defaultEnchants.Add(new Enchant(3855, "Spellpower (Staff)", ItemSlot.TwoHand, new Stats() { SpellPower = 69 }));

            Stats rockets = new Stats();
            rockets.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats()
            { 
                FireDamage = 1837f,
            }, 0f, 45f));
            defaultEnchants.Add(new Enchant(3603, "Hand-Mounted Pyro Rocket", ItemSlot.Hands, rockets));
            return defaultEnchants;
        }
    }
}
