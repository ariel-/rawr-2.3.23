using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Rawr
{
    [GenerateSerializer]
    public class BuffList : List<Buff>
    {
        public BuffList() : base() { }
        public BuffList(IEnumerable<Buff> collection) : base(collection) { }
    }

    public class Buff {
        //early morning
        //summer soul and solace
        //the world is watching
        //vicious circle
        public string Name;
        public string Group;
        public Stats Stats = new Stats();
        private List<CharacterClass> _allowedClasses = null;
        public List<CharacterClass> AllowedClasses
        {
            get
            {
                return _allowedClasses ??
                    (_allowedClasses = new List<CharacterClass>(new CharacterClass[] {
                        CharacterClass.DeathKnight,
                        CharacterClass.Druid,
                        CharacterClass.Hunter,
                        CharacterClass.Mage,
                        CharacterClass.Paladin,
                        CharacterClass.Priest,
                        CharacterClass.Rogue,
                        CharacterClass.Shaman,
                        CharacterClass.Warlock,
                        CharacterClass.Warrior,
                    }));
            }
            set { _allowedClasses = value; }
        }
        private List<Profession> _professions = null;
        public List<Profession> Professions
        {
            get
            {
                return _professions ??
                    (_professions = new List<Profession>(new Profession[] {
                        Profession.None,
                        Profession.Alchemy,
                        Profession.Blacksmithing,
                        Profession.Enchanting,
                        Profession.Engineering,
                        Profession.Herbalism,
                        Profession.Inscription,
                        Profession.Jewelcrafting,
                        Profession.Leatherworking,
                        Profession.Mining,
                        Profession.Skinning,
                        Profession.Tailoring,
                    }));
            }
            set { _professions = value; }
        }
        public string SetName;
        public string Source;
        public int SetThreshold = 0;
        public List<Buff> Improvements = new List<Buff>();
        public bool IsTargetDebuff = false;
        public bool IsCustom = false;
        private List<string> _conflictingBuffs = null;
        public List<string> ConflictingBuffs 
        {
            get { return _conflictingBuffs ?? (_conflictingBuffs = new List<string>(new string[] { Group })); }
            set { _conflictingBuffs = value; }
        }

#if RAWR3
        public static void Save(TextWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BuffList));
            serializer.Serialize(writer, _allBuffs);
            writer.Close();
        }

        public static void Load(TextReader reader)
        {
            try
            {
                List<Buff> loadedBuffs = null;
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(BuffList));
                    loadedBuffs = (List<Buff>)serializer.Deserialize(reader);
                }
                catch { }
                finally
                {
                    reader.Close();
                    loadedBuffs = loadedBuffs ?? new List<Buff>();
                }
#else
        private static readonly string _savedFilePath;
        static Buff() 
        {
            _savedFilePath = 
                Path.Combine(
                    Path.Combine(
                        Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), 
                        "Data"),
                    "BuffCache.xml");
            LoadBuffs();
            //SaveBuffs(); moved to background save after load is complete
        }
        //washing virgin halo
        public Buff() { }

        public static void SaveBuffs()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_savedFilePath, false, Encoding.UTF8))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(BuffList));
                    serializer.Serialize(writer, _allBuffs);
                    writer.Close();
                }
            } catch (Exception) {
                // Log.Write(ex.Message);
                // Log.Write(ex.StackTrace);
            }
        }

        public static void LoadBuffs() 
        {
            try {
                List<Buff> loadedBuffs = new List<Buff>();
                try {
                    if (File.Exists(_savedFilePath)) {
                        using (StreamReader reader = new StreamReader(_savedFilePath, Encoding.UTF8)) {
                            XmlSerializer serializer = new XmlSerializer(typeof(BuffList));
                            loadedBuffs = (List<Buff>)serializer.Deserialize(reader);
                            reader.Close();
                        }
                    }
                } catch (System.Exception) {
                    //Log.Write(ex.Message);
#if !DEBUG
                    Log.Show("The current BuffCache.xml file was made with a previous version of Rawr, which is incompatible with the current version. It will be replaced with buff data included in the current version.");//, "Incompatible BuffCache.xml", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //The designer really doesn't like loading the stuff from a file
#endif
                }
            
#endif
                //the serializer doens't throw an exception in the designer, just sets the value null, have to move this outside the try cactch
                loadedBuffs = loadedBuffs ?? new List<Buff>();
                List<Buff> defaultBuffs = GetDefaultBuffs();
                Dictionary<string, Buff> allBuffs = new Dictionary<string, Buff>();
                foreach (Buff loadedBuff in loadedBuffs)
                    if (loadedBuff.IsCustom)
                        allBuffs.Add(loadedBuff.Name, loadedBuff);
                foreach (Buff defaultBuff in defaultBuffs)
                    if (!allBuffs.ContainsKey(defaultBuff.Name))
                        allBuffs.Add(defaultBuff.Name, defaultBuff);
                Buff[] allBuffArray = new Buff[allBuffs.Count];
                allBuffs.Values.CopyTo(allBuffArray, 0);
                _allBuffs = new BuffList(allBuffs.Values);
                _allBuffsByName = null;
                _allSetBonuses = null;
                _relevantBuffs = null;
                _relevantSetBonuses = null;
                CacheSetBonuses(); // cache it at the start because we don't like on demand caching with multithreading
            } catch { }
        }

        private static void CacheSetBonuses() {
            Dictionary<string, List<Buff>> listDict = new Dictionary<string, List<Buff>>();
            foreach (Buff buff in AllBuffs) 
            {
                string setName = buff.SetName;
                if (!string.IsNullOrEmpty(setName)) 
                {
                    List<Buff> setBonuses;
                    if (!listDict.TryGetValue(setName, out setBonuses))
                    {
                        setBonuses = new List<Buff>();
                        listDict[setName] = setBonuses;
                    }
                    setBonuses.Add(buff);
                }
            }
            foreach (var kvp in listDict)
            {
                setBonusesByName[kvp.Key] = kvp.Value.ToArray();
            }
        }

        //you're in agreement
        public override string ToString() {
            string summary = Name + ": ";
            summary += Stats.ToString();
            summary = summary.TrimEnd(',', ' ', ':');
            return summary;
        }

        //you can understand
        public static Buff GetBuffByName(string name) {
            /*foreach (Buff buff in AllBuffs)
                if (buff.Name == name)
                    return buff;
            return null;*/
            Buff buff;
            AllBuffsByName.TryGetValue(name, out buff);
            return buff;
        }

        //enter static
        private static string _cachedModel = "";
        private static List<Buff> _relevantBuffs = new List<Buff>();
        // returns relevant buffs, but not filtered for professions
        public static List<Buff> RelevantBuffs {
            get {
                if (Calculations.Instance == null || _cachedModel != Calculations.Instance.ToString() || _relevantBuffs == null) {
                    if (Calculations.Instance != null) {
                        _cachedModel = Calculations.Instance.ToString();
                        _relevantBuffs = AllBuffs.FindAll(buff => Calculations.IsBuffRelevant(buff, null));
                    } else { _relevantBuffs = new List<Buff>(); }
                }
                return _relevantBuffs;
            }
        }

        public static void InvalidateBuffs() { _relevantBuffs = null; }

        private static List<Buff> _allSetBonuses = null;
        public static List<Buff> AllSetBonuses {
            get {
                if (_allSetBonuses == null) {
                    _allSetBonuses = AllBuffs.FindAll(buff => !string.IsNullOrEmpty(buff.SetName));
                }
                return _allSetBonuses;
            }
        }

        private static Dictionary<string, Buff[]> setBonusesByName = new Dictionary<string, Buff[]>();
        public static Buff[] GetSetBonuses(string setName) {
            Buff[] setBonuses;
            // if it's not cached we know we don't have any
            setBonusesByName.TryGetValue(setName, out setBonuses);
            return setBonuses;
        }

        private static List<Buff> _relevantSetBonuses = null;
        public static List<Buff> RelevantSetBonuses {
            get {
                if (Calculations.Instance == null || _cachedModel != Calculations.Instance.ToString() || _relevantSetBonuses == null) {
                    if (Calculations.Instance != null) {
                        _cachedModel = Calculations.Instance.ToString();
                        _relevantSetBonuses = AllBuffs.FindAll(buff =>
                            Calculations.HasRelevantStats(buff.GetTotalStats()) && !string.IsNullOrEmpty(buff.SetName));
                    } else { _relevantSetBonuses = new List<Buff>(); }
                }
                return _relevantSetBonuses;
            }
        }

        private static Dictionary<string, Buff> _allBuffsByName = null;
        private static Dictionary<string, Buff> AllBuffsByName {
            get {
                if (_allBuffsByName == null) {
                    _allBuffsByName = new Dictionary<string, Buff>();
                    foreach (Buff buff in AllBuffs) {
                        if (!_allBuffsByName.ContainsKey(buff.Name)) {
                            _allBuffsByName.Add(buff.Name, buff);
                            foreach (Buff improvement in buff.Improvements) {
                                if (!_allBuffsByName.ContainsKey(improvement.Name)) {
                                    _allBuffsByName.Add(improvement.Name, improvement);
                                }
                            }
                        }
                    }
                }
                return _allBuffsByName;
            }
        }

        public Stats GetTotalStats() {
            Stats ret = this.Stats.Clone();
            foreach (Buff buff in Improvements)
                ret.Accumulate(buff.Stats);
            return ret;
        }

        //a grey mistake
        private static BuffList _allBuffs = null;
        public static List<Buff> AllBuffs
        {
            get { return _allBuffs; }
        }

        private static List<Buff> GetDefaultBuffs() {
            List<Buff> defaultBuffs = new List<Buff>();
            Buff buff;

            #region Buffs

            #region Agility and Strength
            defaultBuffs.Add(new Buff
            {
                Name = "Strength of Earth Totem",
                Source = "Shaman",
                Group = "Agility and Strength",
                Stats = { Strength = 155, Agility = 155 },
                ConflictingBuffs = { "Agility", "Strength" },
                Improvements = { 
                    new Buff { Name = "Enhancing Totems (Agility/Strength)", Stats = { Strength = (float)Math.Floor(155f * 0.15f), Agility = (float)Math.Floor(155f * 0.15f) } }
                }
            });

            defaultBuffs.Add(new Buff
            {
                Name = "Horn of Winter",
                Source = "Death Knight",
                Group = "Agility and Strength",
                Stats = { Strength = 155, Agility = 155 },
                ConflictingBuffs = { "Agility", "Strength" }
            });
            #endregion

            #region Armor
            defaultBuffs.Add(new Buff
            {
                Name = "Stoneskin Totem",
                Source = "Shaman",
                Group = "Armor (Stackable)",
                Stats = { BonusArmor = 1150f },
                Improvements = { 
                    new Buff {
                        Name = "Improved Stoneskin Totem (Armor)",
                        Source = "Shaman",
                        Stats = { BonusArmor = (float)Math.Floor(1150f * 0.20f) }
                    }
                }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Devotion Aura",
                Source = "Paladin",
                Group = "Armor",
                Stats = { BonusArmor = 1205f },
                Improvements = { 
                    new Buff {
                        Name = "Improved Devotion Aura (Armor)",
                        Source = "Prot Paladin",
                        Stats = { BonusArmor = (float)Math.Floor(1205f * 0.5f) }
                    }
                }
            });
            #endregion

            #region Damage Reduction (Major %)
            defaultBuffs.Add(new Buff
            {
                Name = "Ancestral Healing",
                Source = "Resto Shaman",
                Group = "Damage Reduction (Major %)",
                Stats = { DamageTakenMultiplier = -0.1f }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Inspiration",
                Source = "Holy Priest",
                Group = "Damage Reduction (Major %)",
                Stats = { DamageTakenMultiplier = -0.1f }
            });
            #endregion

            #region Attack Power
            defaultBuffs.Add(new Buff
            {
                Name = "Battle Shout",
                Source = "Warrior",
                Group = "Attack Power",
                Stats = { AttackPower = 548 },
                Improvements = { 
                    new Buff { Name = "Commanding Presence (Attack Power)", Stats = { AttackPower = (float)Math.Floor(548f * 0.25f) } }
                }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Blessing of Might",
                Source = "Paladin",
                Group = "Attack Power",
                Stats = { AttackPower = 550 },
                Improvements = { 
                    new Buff { Name = "Improved Blessing of Might", Source = "Ret Paladin", Stats = { AttackPower = (float)Math.Floor(550f * 0.25f) } }
                }
            });
            #endregion

            #region Attack Power (%)
            defaultBuffs.Add(new Buff
            {
                Name = "Trueshot Aura",
                Source = "MM Hunter",
                Group = "Attack Power (%)",
                Stats = { BonusAttackPowerMultiplier = 0.1f }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Unleashed Rage",
                Source = "Enhance Shaman",
                Group = "Attack Power (%)",
                Stats = { BonusAttackPowerMultiplier = 0.1f }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Abomination's Might",
                Source = "Blood Death Knight",
                Group = "Attack Power (%)",
                Stats = { BonusAttackPowerMultiplier = 0.1f }
            });
            #endregion

            #region Damage (%)
            defaultBuffs.Add(new Buff
            {
                Name = "Ferocious Inspiration",
                Source = "BM Hunter",
                Group = "Damage (%)",
                Stats = { BonusDamageMultiplier = 0.03f }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Sanctified Retribution",
                Source = "Ret Paladin",
                Group = "Damage (%)",
                Stats = { BonusDamageMultiplier = 0.03f }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Arcane Empowerment",
                Source = "Arcane Mage",
                Group = "Damage (%)",
                Stats = { BonusDamageMultiplier = 0.03f }
            });
            #endregion

            #region Damage Reduction (%)
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Blessing of Sanctuary",
                Source = "Prot Paladin",
                Group = "Damage Reduction (Minor %)",
                Stats = { DamageTakenMultiplier = -0.03f },
                Improvements = {
                    new Buff {
                        Name = "Blessing of Sanctuary (Str/Sta Bonus)",
                        Source = "Prot Paladin",
                        Group = "Stat Multiplier",
                        Stats = {
                            BonusStaminaMultiplier = 0.10f,
                            BonusStrengthMultiplier = 0.10f
                        },
                        ConflictingBuffs = new List<string>(new string[] { "SanctuaryKings", }),
                    }
                }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageAvoided, new Stats() { ManaorEquivRestore = 0.02f }, 0f, 0f));
            defaultBuffs.Add(new Buff
            {
                Name = "Renewed Hope",
                Source = "Disc Priest",
                Group = "Damage Reduction (Minor %)",
                Stats = { DamageTakenMultiplier = -0.03f }
            });
            #endregion

            #region Haste (%)
            defaultBuffs.Add(new Buff
            {
                Name = "Improved Moonkin Form",
                Source = "Moonkin Druid",
                Group = "Haste (%)",
                Stats = { PhysicalHaste = 0.03f, RangedHaste = 0.03f, SpellHaste = 0.03f }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Swift Retribution",
                Source = "Ret Paladin",
                Group = "Haste (%)",
                Stats = { PhysicalHaste = 0.03f, RangedHaste = 0.03f, SpellHaste = 0.03f }
            });
            #endregion

            #region Healing Received (%)
            defaultBuffs.Add(new Buff
            {
                Name = "Improved Devotion Aura (Healing Received %)",
                Source = "Prot Paladin",
                Group = "Healing Received (%)",
                Stats = { HealingReceivedMultiplier = 0.06f }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Tree of Life Aura",
                Source = "Resto Druid",
                Group = "Healing Received (%)",
                Stats = { HealingReceivedMultiplier = 0.06f }
            });
            #endregion

            #region Health
            defaultBuffs.Add(new Buff
            {
                Name = "Blood Pact",
                Source = "Warlock Imp",
                Group = "Health",
                Stats = { Health = 1330 },
                Improvements = { 
                    new Buff { Name = "Improved Imp", Source = "Warlock Imp", Stats = { Health = (float)Math.Floor(1330f * 0.30f) } }
                }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Commanding Shout",
                Source = "Warrior",
                Group = "Health",
                Stats = { Health = 2250 },
                Improvements = { 
                    new Buff { Name = "Commanding Presence (Health)", Source = "Warrior", Stats = { Health = (float)Math.Floor(2250f * 0.25f) } }
                }
            });
            #endregion

            #region Intellect
            defaultBuffs.Add(new Buff
            {
                Name = "Arcane Intellect",
                Source = "Mage",
                Group = "Intellect",
                Stats = { Intellect = 60 },
                ConflictingBuffs = new List<string>(new string[] { "Intellect" }),
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Fel Intelligence (Intellect)",
                Source = "Warlock",
                Group = "Intellect",
                Stats = { Intellect = 48 },
                Improvements = { 
                    new Buff { Name = "Improved Felhunter", Source = "Afflic Warlock", Stats = { Intellect = (float)Math.Floor(48f * 0.1f) } }
                },
                ConflictingBuffs = new List<string>(new string[] { "Intellect" }),
            });
            #endregion

            #region Physical Critical Strike Chance
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Leader of the Pack",
                Source = "Feral Druid",
                Group = "Physical Critical Strike Chance",
                Stats = { PhysicalCrit = 0.05f },
                Improvements = new List<Buff>(){
                    new Buff() {
                        Name = "Improved Leader of the Pack",
                        Source = "Feral Druid",
                        Stats = new Stats(),
                    }
                }
            });
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalCrit,
                new Stats() { HealthRestoreFromMaxHealth = 0.04f, ManaorEquivRestore = 0.08f }, 0f, 6f));
            defaultBuffs.Add(new Buff
            {
                Name = "Rampage",
                Source = "Fury Warrior",
                Group = "Physical Critical Strike Chance",
                Stats = { PhysicalCrit = 0.05f }
            });
            #endregion

            #region Physical Haste
            defaultBuffs.Add(new Buff
            {
                Name = "Windfury Totem",
                Source = "Shaman",
                Group = "Physical Haste",
                Stats = { PhysicalHaste = 0.16f },
                Improvements = { 
                    new Buff { Name = "Improved Windfury Totem", Source = "Enhance Shaman", Stats = { PhysicalHaste = (1.2f/1.16f) - 1f } }
                }
            });

            defaultBuffs.Add(new Buff
            {
                Name = "Improved Icy Talons",
                Source = "Death Knight",
                Group = "Physical Haste",
                Stats = { PhysicalHaste = 0.2f }
            });
            #endregion

            #region Replenishment
            defaultBuffs.Add(new Buff
            {
                Name = "Hunting Party",
                Source = "SV Hunter",
                Group = "Replenishment",
                Stats = { ManaRestoreFromMaxManaPerSecond = 0.002f }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Judgements of the Wise",
                Source = "Ret Paladin",
                Group = "Replenishment",
                Stats = { ManaRestoreFromMaxManaPerSecond = 0.002f }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Vampiric Touch",
                Source = "Shadow Priest",
                Group = "Replenishment",
                Stats = { ManaRestoreFromMaxManaPerSecond = 0.002f }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Improved Soul Leech",
                Source = "Dest Warlock",
                Group = "Replenishment",
                Stats = { ManaRestoreFromMaxManaPerSecond = 0.002f }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Enduring Winter",
                Source = "Frost Mage",
                Group = "Replenishment",
                Stats = { ManaRestoreFromMaxManaPerSecond = 0.002f }
            });
            #endregion

            #region Mana Regeneration
            defaultBuffs.Add(new Buff
            {
                Name = "Mana Spring Totem",
                Source = "Shaman",
                Group = "Mana Regeneration",
                Stats = { Mp5 = 91 },
                Improvements = { 
                    new Buff { Name = "Restorative Totems", Source = "Resto Shaman", Stats = { Mp5 = (float)Math.Floor(91f * 0.20f) } }
                }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Blessing of Wisdom",
                Source = "Paladin",
                Group = "Mana Regeneration",
                Stats = { Mp5 = 91 },
                Improvements = { 
                    new Buff { Name = "Improved Blessing of Wisdom", Source = "Holy Paladin", Stats = { Mp5 = (float)Math.Floor(91 * 0.2f) } }
                }
            });
            #endregion

            #region Burst Mana Regeneration
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Hymn of Hope",
                Source = "Holy Priest",
                Group = "Burst Mana Regeneration",
                Stats = new Stats() { },
                Improvements = {
                    new Buff {
                        Name = "Glyphed Hymn of Hope",
                        Source = "Holy Priest",
                        Stats = new Stats() { },
                    }
                },
                ConflictingBuffs = new List<string>() { }, // preventing this group from conflicting with itself
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                new Stats(){// 3% every 2 sec and 20% Bonus Mana for 8 sec
                    ManaRestoreFromMaxManaPerSecond = (0.03f / 2f),
                    BonusManaMultiplier = 0.20f
                },
                8f, 6 * 60)
            );
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                new Stats()
                {// Glyph adds 2 sec to the Duration
                    ManaRestoreFromMaxManaPerSecond = (0.03f / 2f),
                    BonusManaMultiplier = 0.20f
                },
                2f, 6 * 60)
            );

            defaultBuffs.Add(buff = new Buff {
                Name = "Mana Tide Totem",
                Source = "Resto Shaman",
                Group = "Burst Mana Regeneration",
                Stats = new Stats() { },
                Improvements = {
                    new Buff {
                        Name = "Glyphed Mana Tide Totem",
                        Source = "Resto Shaman",
                        Stats = new Stats() { },
                    }
                },
                ConflictingBuffs = new List<string>() { }, // preventing this group from conflicting with itself
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                new Stats()
                {// 6% Mana every 3 sec for 8 sec
                    ManaRestoreFromMaxManaPerSecond = (0.06f / 3f),
                },
                12f, 5 * 60)
            );
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                new Stats()
                {// Glyph adds 1% sec to the Restoration
                    ManaRestoreFromMaxManaPerSecond = (0.01f / 2f),
                },
                12f, 5 * 60)
            );
            #endregion

            #region Spell Critical Strike Chance
            defaultBuffs.Add(new Buff
            {
                Name = "Elemental Oath",
                Source = "Elem Shaman",
                Group = "Spell Critical Strike Chance",
                Stats = { SpellCrit = 0.05f }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Moonkin Form",
                Source = "Moonkin Druid",
                Group = "Spell Critical Strike Chance",
                Stats = { SpellCrit = 0.05f }
            });
            #endregion

            #region Focus Magic, Spell Critical Strike Chance
            defaultBuffs.Add(new Buff()
            {
                Name = "Focus Magic",
                Source = "Mage",
                Group = "Focus Magic, Spell Critical Strike Chance",
                Stats = { SpellCrit = 0.03f }
            });
            #endregion

            #region Spell Haste
            defaultBuffs.Add(new Buff
            {
                Name = "Wrath of Air Totem",
                Source = "Shaman",
                Group = "Spell Haste",
                Stats = { SpellHaste = 0.05f }
            });
            #endregion

            #region Spell Power
            defaultBuffs.Add(new Buff
            {
                Name = "Demonic Pact",
                Source = "Demo Warlock",
                Group = "Spell Power",
                Stats = { BonusSpellPowerDemonicPactMultiplier = 0.10f, }
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Flametongue Totem",
                Source = "Shaman",
                Group = "Spell Power",
                Stats = { SpellPower = 144f },
                Improvements = {
                    new Buff { Name = "Enhancing Totems (Spell Power)", Stats = { SpellPower = (float)Math.Floor(144f * 0.15f) } }
                },
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Totem of Wrath (Spell Power)",
                Source = "Elem Shaman",
                Group = "Spell Power",
                Stats = { SpellPower = 280f }
            });
           #endregion

            #region Spell Sensitivity
            defaultBuffs.Add(new Buff
            {
                Name = "Amplify Magic (on target, not self)",
                Source = "Mage",
                Group = "Spell Sensitivity",
                Stats = { SpellPower = 255 },
                Improvements = { 
                    new Buff { Name = "Magic Attunement", Stats = { SpellPower = (float)Math.Floor(255f * 0.5f) } } 
                }
            });
            #endregion

            #region Spirit
            defaultBuffs.Add(new Buff
            {
                Name = "Fel Intelligence (Spirit)",
                Source = "Warlock",
                Group = "Spirit",
                Stats = { Spirit = 64f },
                Improvements = { 
                    new Buff { Name = "Improved Felhunter", Source = "Afflic Warlock", Stats = { Spirit = (float)Math.Floor(64f * 0.1f) } }
                },
                ConflictingBuffs = new List<string>(new string[] { "Spirit" }),
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Divine Spirit",
                Source = "Priest",
                Group = "Spirit",
                Stats = { Spirit = 80f },
                ConflictingBuffs = new List<string>(new string[] { "Spirit" }),
            });
            #endregion

            #region Stamina
            defaultBuffs.Add(new Buff
            {
                Name = "Power Word: Fortitude",
                Source = "Priest",
                Group = "Stamina",
                Stats = { Stamina = 165f },
                Improvements = { 
                    new Buff { Name = "Improved Power Word: Fortitude", Source = "Priest", Stats = { Stamina = (float)Math.Floor(165f * 0.3f) } } 
                },
                ConflictingBuffs = new List<string>(new string[] { "Stamina" }),
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Runescroll of Fortitude",
                Source = "Inscription",
                Group = "Stamina",
                Stats = { Stamina = 165f },
                ConflictingBuffs = new List<string>(new string[] { "Stamina" }),
            });
            #endregion

            #region Stat Add
            defaultBuffs.Add(new Buff
            {
                Name = "Mark of the Wild",
                Source = "Druid",
                Group = "Stat Add",
                Stats =
                {
                    BonusArmor = 750,
                    Strength = 37,
                    Agility = 37,
                    Stamina = 37,
                    Intellect = 37,
                    Spirit = 37,
                    ArcaneResistanceBuff = 54,
                    FireResistanceBuff = 54,
                    FrostResistanceBuff = 54,
                    NatureResistanceBuff = 54,
                    ShadowResistanceBuff = 54
                },
                Improvements = { 
                    new Buff { Name = "Improved Mark of the Wild", Source = "Druid", Stats = {
                    BonusArmor = (float)Math.Floor(750f * 0.4f),
                    Strength = (float)Math.Floor(37f * 0.4f),
                    Agility = (float)Math.Floor(37f * 0.4f),
                    Stamina = (float)Math.Floor(37f * 0.4f),
                    Intellect = (float)Math.Floor(37f * 0.4f),
                    Spirit = (float)Math.Floor(37f * 0.4f),
                    ArcaneResistanceBuff = (float)Math.Floor(54f * 1.4f), // it is 1.4 because it's non stacking buff, it takes max
                    FireResistanceBuff = (float)Math.Floor(54f * 1.4f),
                    FrostResistanceBuff = (float)Math.Floor(54f * 1.4f),
                    NatureResistanceBuff = (float)Math.Floor(54f * 1.4f),
                    ShadowResistanceBuff = (float)Math.Floor(54f * 1.4f)} 
                    } 
                },
                ConflictingBuffs = new List<string>(new string[] { "StatArmor", "Mark of the Wild" })
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Drums of the Wild",
                Source = "Leatherworking",
                Group = "Stat Add",
                Stats =
                {
                    BonusArmor = 750,
                    Strength = 37,
                    Agility = 37,
                    Stamina = 37,
                    Intellect = 37,
                    Spirit = 37,
                    ArcaneResistanceBuff = 54,
                    FireResistanceBuff = 54,
                    FrostResistanceBuff = 54,
                    NatureResistanceBuff = 54,
                    ShadowResistanceBuff = 54
                },
                ConflictingBuffs = new List<string>(new string[] { "Mark of the Wild" })
            });
            #endregion

            #region Stat Multiplier
            defaultBuffs.Add(new Buff
            {
                Name = "Blessing of Kings",
                Source = "Paladin",
                Group = "Stat Multiplier",
                Stats = {
                    BonusAgilityMultiplier = 0.10f,
                    BonusIntellectMultiplier = 0.10f,
                    BonusSpiritMultiplier = 0.10f
                },
                Improvements = {
                    new Buff {
                        Name = "Blessing of Kings (Str/Sta Bonus)",
                        Source = "Paladin",
                        Group = "Stat Multiplier",
                        Stats = {
                            BonusStaminaMultiplier = 0.10f,
                            BonusStrengthMultiplier = 0.10f,
                        },
                        ConflictingBuffs = new List<string>(new string[] { "SanctuaryKings", }),
                    }
                },
                ConflictingBuffs = new List<string>(new string[] { "Blessing of Kings" })
            });

            defaultBuffs.Add(new Buff
            {
                Name = "Drums of Forgotten Kings",
                Source = "Leatherworking",
                Group = "Stat Multiplier", 
                Stats = {
                    BonusAgilityMultiplier = 0.08f,
                    BonusIntellectMultiplier = 0.08f,
                    BonusSpiritMultiplier = 0.08f,
                },
                Improvements = {
                    new Buff {
                        Name = "Drums of Forgotten Kings (Str/Sta Bonus)",
                        Source = "Leatherworking",
                        Group = "Stat Multiplier",
                        Stats = {
                            BonusStaminaMultiplier = 0.08f,
                            BonusStrengthMultiplier = 0.08f,
                        },
                        ConflictingBuffs = new List<string>(new string[] { "SanctuaryKings", }),
                    }
                },
                ConflictingBuffs = new List<string>(new string[] { "Blessing of Kings", "Blessing of Kings (Str/Sta Bonus)" })
            });
            #endregion

            #region Resistance
            defaultBuffs.Add(new Buff()
            {
                Name = "Shadow Protection",
                Source = "Priest",
                Group = "Resistance",
                ConflictingBuffs = new List<string>(new string[] { "Shadow Resistance Buff" }),
                Stats = { ShadowResistanceBuff = 130 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Shadow Resistance Aura",
                Source = "Paladin",
                Group = "Resistance",
                ConflictingBuffs = new List<string>(new string[] { "Shadow Resistance Buff" }),
                Stats = { ShadowResistanceBuff = 130 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Aspect of the Wild",
                Source = "Hunter",
                Group = "Resistance",
                ConflictingBuffs = new List<string>(new string[] { "Nature Resistance Buff" }),
                Stats = { NatureResistanceBuff = 130 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Nature Resistance Totem",
                Source = "Shaman",
                Group = "Resistance",
                ConflictingBuffs = new List<string>(new string[] { "Nature Resistance Buff" }),
                Stats = { NatureResistanceBuff = 130 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Frost Resistance Aura",
                Source = "Paladin",
                Group = "Resistance",
                ConflictingBuffs = new List<string>(new string[] { "Frost Resistance Buff" }),
                Stats = { FrostResistanceBuff = 130 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Frost Resistance Totem",
                Source = "Shaman",
                Group = "Resistance",
                ConflictingBuffs = new List<string>(new string[] { "Frost Resistance Buff" }),
                Stats = { FrostResistanceBuff = 130 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Fire Resistance Aura",
                Source = "Paladin",
                Group = "Resistance",
                ConflictingBuffs = new List<string>(new string[] { "Fire Resistance Buff" }),
                Stats = { FireResistanceBuff = 130 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Fire Resistance Totem",
                Source = "Shaman",
                Group = "Resistance",
                ConflictingBuffs = new List<string>(new string[] { "Fire Resistance Buff" }),
                Stats = { FireResistanceBuff = 130 }
            });
            #endregion

            #region Pushback Protection
            defaultBuffs.Add(new Buff()
            {
                Name = "Concentration Aura",
                Source = "Paladin",
                Group = "Pushback Protection",
                Stats = { InterruptProtection = 0.35f },
                Improvements = { new Buff { Name = "Improved Concentration Aura", Stats = { InterruptProtection = 0.15f } } }
            });
            #endregion

            #region Class Buffs
            defaultBuffs.Add(new Buff()
            {
                Name = "Mage Armor",
                Group = "Class Buffs",
                Stats = { MageMageArmor = 1f },
                ConflictingBuffs = new List<string>(new string[] { "Mage Class Armor" }),
                AllowedClasses = new List<CharacterClass> { CharacterClass.Mage }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Molten Armor",
                Group = "Class Buffs",
                Stats = { MageMoltenArmor = 1f },
                ConflictingBuffs = new List<string>(new string[] { "Mage Class Armor" }),
                AllowedClasses = new List<CharacterClass> { CharacterClass.Mage }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Ice Armor",
                Group = "Class Buffs",
                Stats = { MageIceArmor = 1f },
                ConflictingBuffs = new List<string>(new string[] { "Mage Class Armor" }),
                AllowedClasses = new List<CharacterClass> { CharacterClass.Mage }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Inner Fire",
                Group = "Class Buffs",
                Stats = { PriestInnerFire = 1f },
                ConflictingBuffs = new List<string>(new string[] { }),
                AllowedClasses = new List<CharacterClass> { CharacterClass.Priest }
            });

            #endregion

            #region Racial Buffs
            defaultBuffs.Add(new Buff
            {
                Name = "Heroic Presence",
                Source = "Draenei", 
                Group = "Racial Buffs",
                Stats = { SpellHit = 0.01f, PhysicalHit = 0.01f }
            });
            #endregion

            #region Temp Power Boosts
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Tricks of the Trade",
                Source = "Rogue",
                Group = "Temp Power Boost",
                ConflictingBuffs = new List<string>() { "Tricks" },
                Stats = new Stats(),
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BonusDamageMultiplier = 0.15f, }, 6f, 30f));
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Tricks of the Trade (Glyphed)",
                Source = "Rogue",
                Group = "Temp Power Boost",
                ConflictingBuffs = new List<string>() { "Tricks" },
                Stats = new Stats(),
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BonusDamageMultiplier = 0.15f, }, 6f + 4f, 30f));
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Heroism/Bloodlust",
                Source = "Shaman",
                Group = "Temp Power Boost",
                ConflictingBuffs = new List<string>() { "Heroism" },
                Stats = new Stats(),
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { PhysicalHaste = 0.30f, RangedHaste = 0.30f, SpellHaste = 0.30f }, 40f, 10f * 60f));
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Shattering Throw",
                Source = "Arms Warrior",
                Group = "Temp Power Boost",
                Stats = new Stats(),
                ConflictingBuffs = new List<string>() { "Shattering" },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { ArmorPenetration = 0.20f }, 10f, 5 * 60f));
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Hysteria",
                Source = "Death Knight",
                Group = "Temp Power Boost",
                Stats = new Stats(),
                ConflictingBuffs = new List<string>() { "Hysteria" },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BonusPhysicalDamageMultiplier = 0.20f, HealthRestoreFromMaxHealth = -0.01f * 30f }, 30f, 3 * 60f));
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Power Infusion",
                Source = "Disc Priest",
                Group = "Temp Power Boost",
                Stats = new Stats(),
                ConflictingBuffs = new List<string>() { "PowerInfusion" },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { SpellHaste = 0.20f, ManaCostPerc = -0.20f }, 15f, 2 * 60f));
            defaultBuffs.Add(buff = new Buff {
                Name = "Power Infusion (w/ Aspiration)",
                Source = "Disc Priest",
                Group = "Temp Power Boost",
                Stats = new Stats(),
                ConflictingBuffs = new List<string>() { "PowerInfusion" },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { SpellHaste = 0.20f, ManaCostPerc = -0.20f }, 15f, (2 * 60f) * 0.80f));

            #endregion

            #endregion

            #region Debuffs

            #region Armor (Major)
            defaultBuffs.Add(new Buff
            {
                Name = "Acid Spit",
                Source = "BM Hunter (Worm pet)",
                Group = "Armor (Major)",
                Stats = { ArmorPenetration = 0.2f },
                IsTargetDebuff = true,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Expose Armor",
                Source = "Rogue",
                Group = "Armor (Major)",
                Stats = { ArmorPenetration = 0.2f },
                IsTargetDebuff = true,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Sunder Armor",
                Source = "Warrior",
                Group = "Armor (Major)",
                Stats = { ArmorPenetration = 0.2f },
                IsTargetDebuff = true,
            });
            #endregion

            #region Armor (Minor)
            defaultBuffs.Add(new Buff
            {
                Name = "Curse of Weakness",
                Source = "Warlock",
                Group = "Armor (Minor)",
                Stats = { ArmorPenetration = 0.05f },
                IsTargetDebuff = true,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Faerie Fire",
                Source = "Druid",
                Group = "Armor (Minor)",
                Stats = { ArmorPenetration = 0.05f },
                IsTargetDebuff = true,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Sting",
                Source = "BM Hunter (Wasp pet)",
                Group = "Armor (Minor)",
                Stats = { ArmorPenetration = 0.05f },
                IsTargetDebuff = true,
            });
            #endregion

            #region Ranged Attack Power
            defaultBuffs.Add(new Buff()
            {
                Name = "Hunter's Mark",
                Source = "Hunter",
                Group = "Ranged Attack Power",
                Stats = { RangedAttackPower = 500f },
                Improvements = { 
                    new Buff {
                        Name = "Glyphed Hunter's Mark",
                        Stats = { RangedAttackPower = 100f },
                        ConflictingBuffs = { "Hunter's Mark Improvements" } 
                    },
                    new Buff { 
                        Name = "Improved Hunter's Mark", 
                        Stats = { RangedAttackPower = 150f }, 
                        ConflictingBuffs = { "Hunter's Mark Improvements" } 
                    },
                    new Buff { 
                        Name = "Improved and Glyphed Hunter's Mark", 
                        Stats = { RangedAttackPower = 250f }, 
                        ConflictingBuffs = { "Hunter's Mark Improvements" } 
                    }
                },
                IsTargetDebuff = true,
            });
            #endregion

            #region Target Attack Power
            defaultBuffs.Add(new Buff() {
                Name = "Demoralizing Shout",
                Group = "Boss Attack Power",
                Source = "Warrior",
                Stats = { BossAttackPower = -410f }, // Base Demo Shout
                Improvements = { new Buff { Name = "Improved Demoralizing Shout", Source = "Warrior", Stats = new Stats() { BossAttackPower = -410f * 0.40f } } }, // Fully Talented Demo Shout
                IsTargetDebuff = true,
            });
            #endregion

            #region Expose Weakness
            //defaultBuffs.Add(new Buff()
            //{
            //    Name = "Expose Weakness",
            //    Group = "Expose Weakness",
            //    Stats = { ExposeWeakness = 1 }
            //});
            #endregion

            #region Bleed Damage
            defaultBuffs.Add(new Buff
            {
                Name = "Mangle",
                Source = "Feral Druid",
                Group = "Bleed Damage",
                Stats = { BonusBleedDamageMultiplier = 0.3f },
                IsTargetDebuff = true,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Trauma",
                Source = "Arms Warrior",
                Group = "Bleed Damage",
                Stats = { BonusBleedDamageMultiplier = 0.3f },
                IsTargetDebuff = true,
            });
            #endregion

            #region Critical Strike Chance Taken
            defaultBuffs.Add(new Buff
            {
                Name = "Heart of the Crusader",
                Source = "Ret Paladin",
                Group = "Critical Strike Chance Taken",
                Stats = { PhysicalCrit = 0.03f, SpellCritOnTarget = 0.03f },
                IsTargetDebuff = true,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Master Poisoner",
                Source = "Assasin Rogue",
                Group = "Critical Strike Chance Taken",
                Stats = { PhysicalCrit = 0.03f, SpellCritOnTarget = 0.03f },
                IsTargetDebuff = true,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Totem of Wrath",
                Source = "Elem Shaman",
                Group = "Critical Strike Chance Taken",
                Stats = { PhysicalCrit = 0.03f, SpellCritOnTarget = 0.03f },
                IsTargetDebuff = true,
            });
            #endregion

            #region Mana Restore
            defaultBuffs.Add(new Buff
            {
                Name = "Judgement of Wisdom",
                Source = "Paladin",
                Group = "Mana Restore",
                Stats = { ManaRestoreFromBaseManaPPM = 0.24f },
                IsTargetDebuff = true,
            });
            #endregion

            #region Melee Hit Chance Reduction
            defaultBuffs.Add(new Buff
            {
                Name = "Insect Swarm",
                Source = "Moonkin Druid",
                Group = "Melee Hit Chance Reduction",
                Stats = { Miss = 0.03f },
                IsTargetDebuff = true,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Scorpid Sting",
                Source = "Hunter",
                Group = "Melee Hit Chance Reduction",
                Stats = { Miss = 0.03f },
                IsTargetDebuff = true,
            });
            #endregion

            #region Physical Vulnerability
            defaultBuffs.Add(new Buff
            {
                Name = "Blood Frenzy",
                Source = "Arms Warrior",
                Group = "Physical Vulnerability",
                Stats = { BonusPhysicalDamageMultiplier = 0.04f },
                IsTargetDebuff = true,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Savage Combat",
                Source = "Combat Rogue",
                Group = "Physical Vulnerability",
                Stats = { BonusPhysicalDamageMultiplier = 0.04f },
                IsTargetDebuff = true,
            });
            #endregion

            #region Spell Critical Strike Chance
            defaultBuffs.Add(new Buff
            {
                Name = "Winter's Chill",
                Source = "Frost Mage",
                Group = "Spell Critical Strike Taken",
                Stats = { SpellCritOnTarget = 0.05f },
                IsTargetDebuff = true,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Improved Scorch",
                Source = "Fire Mage",
                Group = "Spell Critical Strike Taken",
                Stats = { SpellCritOnTarget = 0.05f },
                IsTargetDebuff = true,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Improved Shadow Bolt",
                Source = "Warlock",
                Group = "Spell Critical Strike Taken",
                Stats = { SpellCritOnTarget = 0.05f },
                IsTargetDebuff = true,
            });
            #endregion

            #region Spell Damage Taken
            defaultBuffs.Add(new Buff
            {
                Name = "Curse of the Elements",
                Source = "Warlock",
                Group = "Spell Damage Taken",
                Stats =
                {
                    BonusFireDamageMultiplier = 0.13f,
                    BonusFrostDamageMultiplier = 0.13f,
                    BonusArcaneDamageMultiplier = 0.13f,
                    BonusShadowDamageMultiplier = 0.13f,
                    BonusHolyDamageMultiplier = 0.13f,
                    BonusNatureDamageMultiplier = 0.13f
                },
                IsTargetDebuff = true,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Earth and Moon",
                Source = "Moonkin Druid",
                Group = "Spell Damage Taken",
                Stats =
                {
                    BonusFireDamageMultiplier = 0.13f,
                    BonusFrostDamageMultiplier = 0.13f,
                    BonusArcaneDamageMultiplier = 0.13f,
                    BonusShadowDamageMultiplier = 0.13f,
                    BonusNatureDamageMultiplier = 0.13f,
                    BonusHolyDamageMultiplier = 0.13f
                },
                IsTargetDebuff = true,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Ebon Plaguebringer",
                Source = "Unholy Death Knight",
                Group = "Spell Damage Taken",
                Stats =
                {
                    BonusFireDamageMultiplier = 0.13f,
                    BonusFrostDamageMultiplier = 0.13f,
                    BonusArcaneDamageMultiplier = 0.13f,
                    BonusShadowDamageMultiplier = 0.13f,
                    BonusHolyDamageMultiplier = 0.13f,
                    BonusNatureDamageMultiplier = 0.13f
                },
                IsTargetDebuff = true,
            });
           /* defaultBuffs.Add(new Buff
            {
                Name = "Rune of Razorice",
                Source = "Death Knight",
                Group = "Spell Damage Taken",
                ConflictingBuffs = new List<string>(),
                Stats =
                {
                    BonusFrostDamageMultiplier = 0.05f,
                }
            }); */

            #endregion

            #region Disease Damage Taken
            defaultBuffs.Add(new Buff
            {
                Name = "Crypt Fever",
                Source = "Unholy Death Knight",
                Group = "Disease Damage Taken",
                Stats = { BonusDiseaseDamageMultiplier = 0.3f },
                IsTargetDebuff = true,
            });
            #endregion

            #region Spell Hit Chance Taken
            defaultBuffs.Add(new Buff
            {
                Name = "Improved Faerie Fire",
                Source = "Moonkin Druid",
                Group = "Spell Hit Chance Taken",
                Stats = { SpellHit = 0.03f },
                IsTargetDebuff = true,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Misery",
                Source = "Shadow Priest",
                Group = "Spell Hit Chance Taken",
                Stats = { SpellHit = 0.03f },
                IsTargetDebuff = true,
            });
            #endregion

            #region Boss Attack Speed Reduction
            defaultBuffs.Add(new Buff()
            {
                Name = "Judgements of the Just",
                Group = "Boss Attack Speed",
                Source = "Prot Paladin",
                Stats = { BossAttackSpeedMultiplier = -0.2f },
                IsTargetDebuff = true,
            });

            defaultBuffs.Add(new Buff()
            {
                Name = "Infected Wounds",
                Group = "Boss Attack Speed",
                Source = "Cat/Bear Druids",
                Stats = { BossAttackSpeedMultiplier = -0.2f },
                IsTargetDebuff = true,
            });

            defaultBuffs.Add(new Buff()
            {
                Name = "Frost Fever",
                Group = "Boss Attack Speed",
                Source = "Frost DK",
                Stats = { BossAttackSpeedMultiplier = -0.14f },
                Improvements = { new Buff { Name = "Improved Icy Touch", Stats = { BossAttackSpeedMultiplier = -(1.0f - (0.8f / 0.86f)) } } },
                IsTargetDebuff = true,
            });

            defaultBuffs.Add(new Buff()
            {
                Name = "Thunder Clap",
                Group = "Boss Attack Speed",
                Source = "Prot/Arms Warrior",
                Stats = { BossAttackSpeedMultiplier = -0.10f },
                Improvements = { new Buff { Name = "Improved Thunder Clap", Stats = { BossAttackSpeedMultiplier = -(1.0f - (0.8f/0.9f)) } } },
                IsTargetDebuff = true,
            });
            #endregion

            #endregion

            #region Consumables

            #region Elixirs and Flasks
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of Fortification",
                Group = "Elixirs and Flasks",
                Stats = { Health = 500, DefenseRating = 10 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
                Improvements = { new Buff { 
                    Name = "Flask of Fortification (Mixology)", 
                    Stats = { Health = 221, DefenseRating = 5 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of Chromatic Wonder",
                Group = "Elixirs and Flasks",
                Stats =
                {
                    Agility = 18,
                    Strength = 18,
                    Stamina = 18,
                    Intellect = 18,
                    Spirit = 18,
                    ArcaneResistance = 35,
                    FireResistance = 35,
                    FrostResistance = 35,
                    ShadowResistance = 35,
                    NatureResistance = 35
                },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Flask of Chromatic Wonder (Mixology)", Stats = {
                    Agility = 4, Strength = 4, Stamina = 4, Intellect = 4, Spirit = 4, 
                    ArcaneResistance = 4, FireResistance = 4, FrostResistance = 4, ShadowResistance = 4, NatureResistance = 4 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Lesser Flask of Resistance",
                Group = "Elixirs and Flasks",
                Stats =
                {
                    ArcaneResistance = 50,
                    FireResistance = 50,
                    FrostResistance = 50,
                    ShadowResistance = 50,
                    NatureResistance = 50
                },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Lesser Flask of Resistance (Mixology)", Stats = {
                    ArcaneResistance = 40, FireResistance = 40, FrostResistance = 40, ShadowResistance = 40, NatureResistance = 40 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of Distilled Wisdom",
                Group = "Elixirs and Flasks",
                Stats = { Intellect = 65 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Flask of Distilled Wisdom (Mixology)", Stats = { Intellect = 20 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of Endless Rage",
                Group = "Elixirs and Flasks",
                Stats = { AttackPower = 180 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Flask of Endless Rage (Mixology)", Stats = { AttackPower = 80 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of Pure Mojo",
                Group = "Elixirs and Flasks",
                Stats = { Mp5 = 45 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Flask of Pure Mojo (Mixology)", Stats = { Mp5 = 20 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of Stoneblood",
                Group = "Elixirs and Flasks",
                Stats = { Health = 1300 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Flask of Stoneblood (Mixology)", Stats = { Health = 650 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of the Frost Wyrm",
                Group = "Elixirs and Flasks",
                Stats = { SpellPower = 125 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Flask of the Frost Wyrm (Mixology)", Stats = { SpellPower = 47 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of the North - Attack Power",
                Group = "Elixirs and Flasks",
                Source = "Alchemy",
                Stats = { AttackPower = 80 },
                Professions = new List<Profession>() { Profession.Alchemy },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of the North - Spell Power",
                Group = "Elixirs and Flasks",
                Source = "Alchemy",
                Stats = { SpellPower = 47 },
                Professions = new List<Profession>() { Profession.Alchemy },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of the North - Strength",
                Group = "Elixirs and Flasks",
                Source = "Alchemy",
                Stats = { Strength = 40 },
                Professions = new List<Profession>() { Profession.Alchemy },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Major Agility",
                Group = "Elixirs and Flasks",
                Stats = { Agility = 30, CritRating = 12 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Major Agility (Mixology)", Stats = { Agility = 10, CritRating = 4 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Demonslaying",
                Group = "Elixirs and Flasks",
                Stats = { AttackPowerDemons = 265 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Demonslaying (Mixology)", Stats = { AttackPowerDemons = 88 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Adept's Elixir",
                Group = "Elixirs and Flasks",
                Stats = { SpellPower = 24, CritRating = 24 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Adept's Elixir (Mixology)", Stats = { SpellPower = 9, CritRating = 9 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Healing Power",
                Group = "Elixirs and Flasks",
                Stats = { SpellPower = 24f, Spirit = 24 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Healing Power (Mixology)", Stats = { SpellPower = 9, Spirit = 9 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Accuracy",
                Group = "Elixirs and Flasks",
                Stats = { HitRating = 45 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Accuracy (Mixology)", Stats = { HitRating = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Armor Piercing",
                Group = "Elixirs and Flasks",
                Stats = { ArmorPenetrationRating = 45 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Armor Piercing (Mixology)", Stats = { ArmorPenetrationRating = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Deadly Strikes",
                Group = "Elixirs and Flasks",
                Stats = { CritRating = 45 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Deadly Strikes (Mixology)", Stats = { CritRating = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Expertise",
                Group = "Elixirs and Flasks",
                Stats = { ExpertiseRating = 45 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Expertise (Mixology)", Stats = { ExpertiseRating = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Lightning Speed",
                Group = "Elixirs and Flasks",
                Stats = { HasteRating = 45 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Lightning Speed (Mixology)", Stats = { HasteRating = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Mighty Agility",
                Group = "Elixirs and Flasks",
                Stats = { Agility = 45 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Mighty Agility (Mixology)", Stats = { Agility = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Mighty Strength",
                Group = "Elixirs and Flasks",
                Stats = { Strength = 50 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Mighty Strength (Mixology)", Stats = { Strength = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Guru's Elixir",
                Group = "Elixirs and Flasks",
                Stats = { Stamina = 20, Intellect = 20, Spirit = 20, Strength = 20, Agility = 20 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Guru's Elixir (Mixology)", Stats = { 
                    Stamina = 8, Intellect = 8, Spirit = 8, Strength = 8, Agility = 8},
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Spellpower Elixir",
                Group = "Elixirs and Flasks",
                Stats = { SpellPower = 58 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Spellpower Elixir (Mixology)", Stats = { SpellPower = 19 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Wrath Elixir",
                Group = "Elixirs and Flasks",
                Stats = { AttackPower = 90 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Wrath Elixir (Mixology)", Stats = { AttackPower = 32 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });

            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Ironskin",
                Group = "Elixirs and Flasks",
                Stats = { Resilience = 30 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Ironskin (Mixology)", Stats = { Resilience = 10 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Draenic Wisdom",
                Group = "Elixirs and Flasks",
                Stats = { Intellect = 30, Spirit = 30 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Draenic Wisdom (Mixology)", Stats = { Intellect = 8, Spirit = 8 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Mighty Defense",
                Group = "Elixirs and Flasks",
                Stats = { DefenseRating = 45 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Mighty Defense (Mixology)", Stats = { DefenseRating = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Mighty Mageblood",
                Group = "Elixirs and Flasks",
                Stats = { Mp5 = 24 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Mighty Mageblood (Mixology)", Stats = { Mp5 = 6 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Protection",
                Group = "Elixirs and Flasks",
                Stats = { BonusArmor = 800 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Protection (Mixology)", Stats = { BonusArmor = 224 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Mighty Thoughts",
                Group = "Elixirs and Flasks",
                Stats = { Intellect = 45 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Mighty Thoughts (Mixology)", Stats = { Intellect = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Mighty Fortitude",
                Group = "Elixirs and Flasks",
                Stats = { Health = 350, Hp5 = 20 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Mighty Fortitude (Mixology)", Stats = { Health = 116, Hp5 = 6 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Spirit",
                Group = "Elixirs and Flasks",
                Stats = { Spirit = 50 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Spirit (Mixology)", Stats = { Spirit = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Empowerment",
                Group = "Elixirs and Flasks",
                Stats = { SpellPenetration = 30 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Empowerment (Mixology)", Stats = { SpellPenetration = 10 },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            #endregion

            #region Potion
            // potions set to be 1 hr cooldown to ensure its treated as once per combat.
            // Jothay: Changed to 20 Minutes to give a higher value for the fight while
            // keeping it outside the chance of using it twice during same fight
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Potion of Speed",
                Group = "Potion",
                Stats = new Stats(),
                Improvements = { new Buff { Name = "Potion of Speed (Double Pot Trick)", Stats = new Stats() } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { HasteRating = 500f }, 15f, float.PositiveInfinity /*20f * 60f*/));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { HasteRating = 500f }, 15f - 1f, float.PositiveInfinity /*20f * 60f*/));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Swiftness Potion",
                Group = "Potion",
                Stats = new Stats(),
                Improvements = { new Buff { Name = "Swiftness Potion (Double Pot Trick)", Stats = new Stats() } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { MovementSpeed = 0.50f }, 15f, float.PositiveInfinity /*20f * 60f*/));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { MovementSpeed = 0.50f }, 15f - 1f, float.PositiveInfinity /*20f * 60f*/));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Potion of Wild Magic",
                Group = "Potion",
                Stats = new Stats(),
                Improvements = { new Buff { Name = "Potion of Wild Magic (Double Pot Trick)", Stats = new Stats() } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { CritRating = 200f, SpellPower = 200f }, 15f, float.PositiveInfinity /*20f * 60f*/));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { CritRating = 200f, SpellPower = 200f }, 15f - 1f, float.PositiveInfinity /*20f * 60f*/));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Heroic Potion",
                Group = "Potion",
                Stats = new Stats(),
                Improvements = { new Buff { Name = "Heroic Potion (Double Pot Trick)", Stats = new Stats() } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Strength = 70f, Health = 700f }, 15f, float.PositiveInfinity /*20f * 60f*/));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Strength = 70f, Health = 700f }, 15f - 1f, float.PositiveInfinity /*20f * 60f*/));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Insane Strength Potion",
                Group = "Potion",
                Stats = new Stats(),
                Improvements = { new Buff { Name = "Insane Strength Potion (Double Pot Trick)", Stats = new Stats() } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Strength = 120f, DefenseRating = -75f }, 15f, float.PositiveInfinity /*20f * 60f*/));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Strength = 120f, DefenseRating = -75 }, 15f - 1f, float.PositiveInfinity /*20f * 60f*/));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Indestructible Potion",
                Group = "Potion",
                Stats = new Stats(),
                Improvements = { new Buff { Name = "Indestructible Potion (Double Pot Trick)", Stats = new Stats() } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BonusArmor = 3500f }, 2f * 60f, float.PositiveInfinity /*20f * 60f*/));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BonusArmor = 3500f }, 2f * 60f - 1f, float.PositiveInfinity /*20f * 60f*/));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Mighty Rage Potion",
                Group = "Potion",
                Stats = new Stats() { BonusRageGen = (45f + 75) / 2f, },
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, CharacterClass.Druid, },
                Improvements = {
                    new Buff {
                        Name = "Mighty Rage Potion (Double Pot Trick)",
                        Stats = new Stats() { BonusRageGen = (45f + 75) / 2f, },
                        AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, CharacterClass.Druid, },
                    }
                }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Strength = 60f, }, 20f, float.PositiveInfinity /*20f * 60f*/));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Strength = 60f, }, 20f - 1f, float.PositiveInfinity /*20f * 60f*/));
            defaultBuffs.Add(new Buff()
            {
                Name = "Runic Mana Potion",
                Group = "Potion",
                Stats = new Stats() { ManaRestore = (4200f + 4400f) / 2f, },
                Improvements = {
                        new Buff {
                            Name = "Runic Mana Potion (Alch Stone Bonus)",
                            Professions = new List<Profession>() { Profession.Alchemy },
                            Stats = new Stats() { ManaRestore = ((4200f + 4400f) / 2f) * 0.40f, }
                        }
                    }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Runic Mana Injector",
                Group = "Potion",
                Stats = new Stats() { ManaRestore = (4200f + 4400f) / 2f, },
                Improvements = {
                        new Buff {
                            Name = "Runic Mana Injector (Engineer Bonus)",
                            Professions = new List<Profession>() { Profession.Engineering },
                            Stats = new Stats() { ManaRestore = ((4200f + 4400f) / 2f) * 0.25f, }
                        }
                    }
            });
             defaultBuffs.Add(new Buff()
             {
                Name = "Endless Mana Potion",
                Group = "Potion",
                Source = "Alchemy",
                Professions = new List<Profession>() { Profession.Alchemy },
                Stats = new Stats() { ManaRestore = (1800f + 3000f) / 2f, },
                Improvements = {
                        new Buff {
                            Name = "Endless Mana Potion (Alch Stone Bonus)",
                            Professions = new List<Profession>() { Profession.Alchemy },
                            Stats = new Stats() { ManaRestore = ((1800f + 3000f) / 2f) * 0.40f, }
                        }
                    }
            });
             defaultBuffs.Add(new Buff()
             {
                Name = "Endless Healing Potion",
                Group = "Potion",
                Source = "Alchemy",
                Professions = new List<Profession>() { Profession.Alchemy },
                Stats = new Stats() { HealthRestore = (1500f + 2500f) / 2f, },
                Improvements = {
                        new Buff {
                            Name = "Endless Healing Potion (Alch Stone Bonus)",
                            Professions = new List<Profession>() { Profession.Alchemy },
                            Stats = new Stats() { HealthRestore = ((1500f + 2500f) / 2f) * 0.40f, }
                        }
                    }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Runic Healing Potion",
                Group = "Potion",
                Stats = new Stats() { HealthRestore = (2700f + 4500f) / 2f, },
                Improvements = {
                        new Buff {
                            Name = "Runic Healing Potion (Alch Stone Bonus)",
                            Professions = new List<Profession>() { Profession.Alchemy },
                            Stats = new Stats() { HealthRestore = ((2700f + 4500f) / 2f) * 0.40f, }
                        }
                    }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Runic Healing Injector",
                Group = "Potion",
                Stats = new Stats() { HealthRestore = (2700f + 4500f) / 2f, },
                Improvements = {
                        new Buff {
                            Name = "Runic Healing Injector (Engineer Bonus)",
                            Professions = new List<Profession>() { Profession.Engineering },
                            Stats = new Stats() { HealthRestore = ((2700f + 4500f) / 2f) * 0.25f, }
                        }
                    }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Powerful Rejuvenation Potion",
                Group = "Potion",
                Stats = new Stats() {
                    ManaRestore   = (2475f + 4125f) / 2f,
                    HealthRestore = (2475f + 4125f) / 2f,
                },
                Improvements = {
                        new Buff {
                            Name = "Powerful Rejuvenation Potion (Alch Stone Bonus)",
                            Professions = new List<Profession>() { Profession.Alchemy },
                            Stats = new Stats() {
                                ManaRestore   = ((2475f + 4125f) / 2f) * 0.40f,
                                HealthRestore = ((2475f + 4125f) / 2f) * 0.40f,
                            }
                        }
                    }
            });
            /* This potion has several effects that could individually randomly proc on use:
             * 
             * Wild Magic - Spell critical rating increased by 60 and spell power increased by 180. 15 seconds
             * Nightmare Slumber - like Dreamless Sleep potion (health and mana back over 6 seconds, ~5k both)
             * Healing Potion - like a healing potion
             * Nothing - Does nothing
             * Haste - Increased haste rating by 500 for 15 seconds
             * Nightmare slumber:
             *   Puts an undispellable debuff on you that regens
             *   2520 mana and health every second for 3 seconds (with the alchemist stone on)
             *   This thing always seems to happen when you are lower on health and mana then
             *   when on full health and mana.
             */
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Crazy Alchemist's Potion (proc not included)",
                Group = "Potion",
                Source = "Alchemy",
                Professions = new List<Profession>() { Profession.Alchemy },
                Stats = new Stats() {
                    ManaRestore   = (4200f + 4400f) / 2f,
                    HealthRestore = (3100f + 3500f) / 2f,
                },
                Improvements = {
                        new Buff {
                            Name = "Crazy Alchemist's Potion (Alch Stone Bonus)",
                            Professions = new List<Profession>() { Profession.Alchemy },
                            Stats = new Stats() {
                                ManaRestore   = ((4200f + 4400f) / 2f) * 0.40f,
                                HealthRestore = ((3100f + 3500f) / 2f) * 0.40f,
                            }
                        }
                    }
            });
            #endregion

            #region Food
            defaultBuffs.Add(new Buff()
            {
                Name = "Spirit Food",
                Group = "Food",
                Stats = { Stamina = 40, Spirit = 40 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Agility Food",
                Group = "Food",
                Stats = { Agility = 40, Stamina = 40 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Strength Food",
                Group = "Food",
                Stats = { Strength = 40, Stamina = 40 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Armor Pen Food",
                Group = "Food",
                Stats = { ArmorPenetrationRating = 40, Stamina = 40 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Expertise Food",
                Group = "Food",
                Stats = { ExpertiseRating = 40, Stamina = 40 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Hit Food",
                Group = "Food",
                Stats = { HitRating = 40, Stamina = 40 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Spell Power Food",
                Group = "Food",
                Stats = { SpellPower = 46, Stamina = 40 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Haste Food",
                Group = "Food",
                Stats = { HasteRating = 40, Stamina = 40 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Attack Power Food",
                Group = "Food",
                Stats = { AttackPower = 80, Stamina = 40 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Strength Food",
                Group = "Food",
                Stats = { Strength = 40, Stamina = 40 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Mp5 Food",
                Group = "Food",
                Stats = { Mp5 = 20, Stamina = 40 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Crit Food",
                Group = "Food",
                Stats = { CritRating = 40, Stamina = 40 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Fish Feast",
                Group = "Food",
                Stats = { AttackPower = 80, SpellPower = 46, Stamina = 40 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Dodge Food",
                Group = "Food",
                Stats = { DodgeRating = 40, Stamina = 40 }
            });
            #endregion
            #region Pet Food
            defaultBuffs.Add(new Buff()
            {
                Name = "Kibler's Bits",
                Group = "Pet Food",
                Stats = { PetStrength = 20, PetStamina = 20 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Spiced Mammoth Treats",
                Group = "Pet Food",
                Stats = { PetStrength = 30, PetStamina = 30 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Sporeling Snack",
                Group = "Pet Food",
                Stats = { PetSpirit = 20, PetStamina = 20 }
            });
            #endregion

            #region Scrolls
            defaultBuffs.Add(new Buff()
            {
                Name = "Scroll of Agility VIII",
                Group = "Scrolls",
                ConflictingBuffs = new List<string>(new string[] { "Scroll of Agility", "Agility" }),
                Stats = { Agility = 30 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Scroll of Strength VIII",
                Group = "Scrolls",
                ConflictingBuffs = new List<string>(new string[] { "Scroll of Strength", "Strength" }),
                Stats = { Strength = 30 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Scroll of Intellect VIII",
                Group = "Scrolls",
                ConflictingBuffs = new List<string>(new string[] { "Scroll of Intellect", "Intellect" }),
                Stats = { Intellect = 48 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Scroll of Stamina VIII",
                Group = "Scrolls",
                ConflictingBuffs = new List<string>(new string[] { "Scroll of Stamina", "Stamina" }),
                Stats = { Stamina = 132 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Scroll of Spirit VIII",
                Group = "Scrolls",
                ConflictingBuffs = new List<string>(new string[] { "Scroll of Spirit", "Spirit" }),
                Stats = { Spirit = 64 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Scroll of Protection VII",
                Group = "Scrolls",
                ConflictingBuffs = new List<string>(new string[] { "Scroll of Protection", "StatArmor" }),
                Stats = { BonusArmor = 340 }
            });
            #endregion

            #region Temporary Weapon Enchantment
            //defaultBuffs.Add(new Buff()
            //{
            //    Name = "Adamantite Weightstone",
            //    Group = "Temporary Weapon Enchantment",
            //    Stats = { WeaponDamage = 12, CritRating = 14 }
            //});
            defaultBuffs.Add(new Buff()
            {
                Name = "Earthliving Weapon",
                Group = "Temporary Weapon Enchantment",
                Stats = { Earthliving = 1 }
            });
            //defaultBuffs.Add(new Buff()
            //{
            //    Name = "Righteous Weapon Coating",
            //    Group = "Temporary Weapon Enchantment",
            //    Stats = { AttackPower = 60 }
            //});
            //defaultBuffs.Add(new Buff()
            //{
            //    Name = "Elemental Sharpening Stone",
            //    Group = "Temporary Weapon Enchantment",
            //    Stats = { CritRating = 28 }
            //});
            //defaultBuffs.Add(new Buff()
            //{
            //    Name = "Superior Wizard Oil",
            //    Group = "Temporary Weapon Enchantment",
            //    Stats = { SpellPower = 42 }
            //});
            //defaultBuffs.Add(new Buff()
            //{
            //    Name = "Brilliant Wizard Oil",
            //    Group = "Temporary Weapon Enchantment",
            //    Stats = { SpellPower = 36, CritRating = 14 }
            //});
            //defaultBuffs.Add(new Buff()
            //{
            //    Name = "Superior Mana Oil",
            //    Group = "Temporary Weapon Enchantment",
            //    Stats = { Mp5 = 14 }
            //});
            //defaultBuffs.Add(new Buff()
            //{
            //    Name = "Brilliant Mana Oil",
            //    Group = "Temporary Weapon Enchantment",
            //    Stats = { Mp5 = 12, SpellPower = 13 }
            //});
            #endregion

            #endregion

            #region Set Bonuses
            
            #region Death Knight
            #region WotLK
            #region Tier  7 | Scourgeborne
            #region Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Scourgeborne Battlegear (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    BonusDeathStrikeCrit = 0.05f,
                    BonusObliterateCrit = 0.05f,
                    BonusScourgeStrikeCrit = 0.05f
                },
                SetName = "Scourgeborne Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Scourgeborne Battlegear (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    BonusRPFromDeathStrike = 5f,
                    BonusRPFromObliterate = 5f,
                    BonusRPFromScourgeStrike = 5f
                },
                SetName = "Scourgeborne Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            #endregion
            #region Plate
            defaultBuffs.Add(new Buff()
            {
                Name = "Scourgeborne Plate (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusPlagueStrikeCrit = 0.10f },
                SetName = "Scourgeborne Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Scourgeborne Plate (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusIceboundFortitudeDuration = 3f },
                SetName = "Scourgeborne Plate",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            #endregion
            #endregion
            #region Tier  8 | Darkruned
            #region Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Darkruned Battlegear (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    BonusDeathCoilCrit = 0.08f,
                    BonusFrostStrikeCrit = 0.08f
                },
                SetName = "Darkruned Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Darkruned Battlegear (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    BonusPerDiseaseBloodStrikeDamage = 0.20f,
                    BonusPerDiseaseHeartStrikeDamage = 0.20f,
                    BonusPerDiseaseObliterateDamage = 0.20f,
                    BonusPerDiseaseScourgeStrikeDamage = 0.20f
                },
                SetName = "Darkruned Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            #endregion
            #region Plate
            defaultBuffs.Add(new Buff()
            {
                Name = "Darkruned Plate (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusRuneStrikeMultiplier = 0.10f },
                SetName = "Darkruned Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Darkruned Plate (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusAntiMagicShellDamageReduction = 0.10f },
                SetName = "Darkruned Plate",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            #endregion
            #endregion
            #region Tier  9 | Koltira/Kolitra/Thassaurian
            #region Battlegear
            Stats DK2T9 = new Stats();
            DK2T9.AddSpecialEffect(new SpecialEffect(Trigger.BloodStrikeHit, new Stats() { Strength = 180f }, 15f, 45f, .5f));
            DK2T9.AddSpecialEffect(new SpecialEffect(Trigger.HeartStrikeHit, new Stats() { Strength = 180f }, 15f, 45f, .5f));
            defaultBuffs.Add(new Buff()
            {
                Name = "Thassarian's Battlegear (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = DK2T9,
                SetName = "Thassarian's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Thassarian's Battlegear (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    DiseasesCanCrit = 1f
                },
                SetName = "Thassarian's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            #endregion
            #region Plate
            defaultBuffs.Add(new Buff()
            {
                // Decreases the cooldown on your Dark Command ability by 2 sec and increases the damage done by your Blood Strike and Heart Strike abilities by 5%.
                Name = "Koltira's Plate (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { 
                    BonusBloodStrikeDamageMultiplier = .05f,
                    BonusHeartStrikeDamageMultiplier = .05f,
                },
                SetName = "Koltira's Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(new Buff()
            {
                // Decreases the cooldown on your Unbreakable Armor, Vampiric Blood, and Bone Shield abilities by 10 sec.
                Name = "Koltira's Plate (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Koltira's Plate",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });

            defaultBuffs.Add(new Buff()
            {
                Name = "Thassarian's Plate (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    BonusBloodStrikeDamageMultiplier = .05f,
                    BonusHeartStrikeDamageMultiplier = .05f,
                },
                SetName = "Thassarian's Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Thassarian's Plate (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Thassarian's Plate",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            #endregion
            #endregion
            #region Tier 10 | Scourgelord's
            #region Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Scourgelord's Battlegear (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    BonusHeartStrikeMultiplier = .07f,
                    BonusScourgeStrikeMultiplier = .1f,
                    BonusObliterateMultiplier = .1f
                },
                SetName = "Scourgelord's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Scourgelord's Battlegear (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    BonusPhysicalDamageMultiplier = .03f,
                    BonusSpellPowerMultiplier = .03f
                },
                SetName = "Scourgelord's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            #endregion
            #region Plate
            defaultBuffs.Add(new Buff()
            {
                Name = "Scourgelord's Plate (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = {
                    TankDK_T10_2pc = .20f,                    
                },
                SetName = "Scourgelord's Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Scourgelord's Plate (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { 
                    TankDK_T10_4pc = .12f,
                },
                SetName = "Scourgelord's Plate",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            #endregion
            #endregion
            #endregion
            #endregion

            #region Druid
            #region TBC
            /*
            #region Tier 4 | Malorne
            #region Harness
            defaultBuffs.Add(new Buff()
            {
                Name = "Malorne Harness 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BloodlustProc = 0.8f },
                SetName = "Malorne Harness",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Malorne Harness 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusArmor = 1400, CatFormStrength = 30 },
                SetName = "Malorne Harness",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid },
            });
            #endregion
            #region Raiment
            defaultBuffs.Add( buff = new Buff()
            {
                Name = "Malorne Raiment 2 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ManaRestoreOnCast_5_15 = 120 },
                SetName = "Malorne Raiment",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.HealingSpellCast, new Stats() { ManaRestore = 120f }, 0f, 0f, 0.05f));
            #endregion
            #region Regalia
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Malorne Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Malorne Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellCast, new Stats() { Mana = 120f }, 0f, 0f, 0.1f, 1));
            defaultBuffs.Add(new Buff()
            {
                Name = "Malorne Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { InnervateCooldownReduction = 48.0f },
                SetName = "Malorne Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #endregion
            #region Tier 5 | Nordrassil
            #region Harness
            defaultBuffs.Add(new Buff()
            {
                Name = "Nordrassil Harness 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusShredDamage = 75 }, //, BonusLacerateDamage = 15/5},
                SetName = "Nordrassil Harness",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid },
            });
            #endregion
            #region Raiment
            defaultBuffs.Add(new Buff()
            {
                Name = "Nordrassil Raiment 2 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RegrowthExtraTicks = 2 },
                SetName = "Nordrassil Raiment",
                SetThreshold = 2
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Nordrassil Raiment 4 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { LifebloomFinalHealBonus = 150 },
                SetName = "Nordrassil Raiment",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Nordrassil Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { StarfireBonusWithDot = 0.1f },
                SetName = "Nordrassil Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #endregion
            #region Tier 6 | Thunderheart
            #region Harness
            defaultBuffs.Add(new Buff()
            {
                Name = "Thunderheart Harness 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { MangleCatCostReduction = 5, BonusMangleBearThreat = 0.15f },
                SetName = "Thunderheart Harness",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Thunderheart Harness 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusRipDamageMultiplier = .15f, BonusFerociousBiteDamageMultiplier = .15f, BonusSwipeDamageMultiplier = .15f },
                SetName = "Thunderheart Harness",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid },
            });
            #endregion
            #region Raiment
            defaultBuffs.Add(new Buff()
            {
                Name = "Thunderheart Raiment 4 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusHealingTouchMultiplier = 0.05f },
                SetName = "Thunderheart Raiment",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Thunderheart Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { MoonfireExtension = 3.0f },
                SetName = "Thunderheart Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Thunderheart Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { StarfireCritChance = 0.05f },
                SetName = "Thunderheart Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #endregion
 */
            #endregion
            #region WotLK
            #region Tier  7 | Dreamwalker
            #region Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreamwalker Battlegear (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusRipDuration = 4f, BonusLacerateDamageMultiplier = 0.05f },
                SetName = "Dreamwalker Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreamwalker Battlegear (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { TigersFuryCooldownReduction = 3f, WeaponDamage = 1.778f /*Increased Barkskin Duration*/ },
                SetName = "Dreamwalker Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreamwalker Garb (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusInsectSwarmDamage = 0.1f },
                SetName = "Dreamwalker Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreamwalker Garb (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusNukeCritChance = 0.05f },
                SetName = "Dreamwalker Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreamwalker Regalia (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { LifebloomCostReduction = 0.05f },
                SetName = "Dreamwalker Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreamwalker Regalia (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { NourishBonusPerHoT = 0.05f },
                SetName = "Dreamwalker Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #endregion
            #region Tier  8 | Nightsong
            #region Battlegear
            defaultBuffs.Add(new Buff() //TODO TODO TODO TODO
            {
                Name = "Nightsong Battlegear (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ClearcastOnBleedChance = 0.02f },
                SetName = "Nightsong Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff() //TODO TODO TODO TODO
            {
                Name = "Nightsong Battlegear (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusSavageRoarDuration = 8f },
                SetName = "Nightsong Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Nightsong Garb (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { EclipseBonus = 0.07f },
                SetName = "Nightsong Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Nightsong Garb (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats(),
                SetName = "Nightsong Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.InsectSwarmTick, new Stats() { StarfireProc = 1f }, 0f, 0f, 0.08f, 1));
            #endregion
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Nightsong Regalia (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { SwiftmendBonus = 0.1f },
                SetName = "Nightsong Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });

            defaultBuffs.Add(new Buff()
            {
                Name = "Nightsong Regalia (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RejuvenationInstantTick = 0.5f },
                SetName = "Nightsong Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #endregion
            #region Tier  9 | Malfurion's
            #region Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Malfurion's Battlegear (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusRakeDuration = 3f, BonusLacerateDamageMultiplier = 0.05f },
                SetName = "Malfurion's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Malfurion's Battlegear (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusRipCrit = 0.05f, BonusFerociousBiteCrit = 0.05f },
                SetName = "Malfurion's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Malfurion's Regalia (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { MoonfireDotCrit = 1f },
                SetName = "Malfurion's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Malfurion's Regalia (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusMoonkinNukeDamage = 0.04f },
                SetName = "Malfurion's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Malfurion's Garb (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { NourishCritBonus = 0.05f },
                SetName = "Malfurion's Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Malfurion's Garb (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RejuvenationCrit = 1.0f },
                SetName = "Malfurion's Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #endregion
            #region Tier 10 | Lasherweave
            #region Battlegear
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Lasherweave Battlegear (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RipCostReduction = 10f, BonusLacerateDamageMultiplier = 0.20f, BonusBearSwipeDamageMultiplier = 0.20f },
                SetName = "Lasherweave Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Lasherweave Battlegear (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusRakeCrit = 1f, DamageTakenMultiplier = -0.02f},
                SetName = "Lasherweave Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Regalia
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Lasherweave Regalia (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats(),
                SetName = "Lasherweave Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellCast, new Stats() { BonusArcaneDamageMultiplier = 0.15f, BonusNatureDamageMultiplier = 0.15f }, 6.0f, 0f, 0.06f, 1));
            defaultBuffs.Add(new Buff()
            {
                Name = "Lasherweave Regalia (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { MoonkinT10CritDot = 0.07f },
                SetName = "Lasherweave Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Garb
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Lasherweave Garb (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { WildGrowthLessReduction = 0.3f },
                SetName = "Lasherweave Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Lasherweave Garb (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RejuvJumpChance = 0.02f },
                SetName = "Lasherweave Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #endregion
            #endregion
            #region PvP
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator Sanctuary (PvP) 2 Piece Set Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 100, SpellPower = 29 },
                SetName = "Gladiator's Sanctuary",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator Sanctuary (PvP) 4 Piece Set Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { SpellPower = 88, /*SwiftmendCdReduc = 2*/ },
                SetName = "Gladiator's Sanctuary",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #endregion

            #region Hunter
            #region WotLK
            #region Tier 7 | Cryptstalker
            defaultBuffs.Add(new Buff()
            {
                Name = "Cryptstalker Battlegear (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Cryptstalker Battlegear",
                Stats = { BonusPetDamageMultiplier = 0.05f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Cryptstalker Battlegear (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Cryptstalker Battlegear",
                Stats = { BonusHunter_T7_4P_ViperSpeed = 0.2f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            #endregion
            #region Tier 8 | Scourgestalker
            defaultBuffs.Add(new Buff()
            {
                Name = "Scourgestalker Battlegear (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Scourgestalker Battlegear",
                Stats = { BonusHunter_T8_2P_SerpDmg = 0.1f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Scourgestalker Battlegear (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Scourgestalker Battlegear",
                Stats = new Stats(), //{ BonusHunter_T8_4P_SteadyShotAPProc = 1 },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.SteadyShotHit,
                new Stats() { AttackPower = 600f, },
                15f, 45f, 0.10f));
            #endregion
            #region Tier 9 | Windrunner's
            defaultBuffs.Add(new Buff()
            {
                Name = "Windrunner's Battlegear (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Windrunner's Battlegear",
                Stats = { BonusHunter_T9_2P_SerpCanCrit = 1 },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Windrunner's Battlegear (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Windrunner's Battlegear",
                Stats = new Stats() { },// { BonusHunter_T9_4P_SteadyShotPetAPProc = 1 },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.RangedHit,
                new Stats() { PetAttackPower = 600f, },
                15f, 45f, 0.35f));
            #endregion
            #region Tier 10 | Ahn'Kahar Blood Hunter's
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Ahn'Kahar Blood Hunter's Battlegear (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Ahn'Kahar Blood Hunter's Battlegear",
                Stats = new Stats() { },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.HunterAutoShotHit,
                new Stats() { BonusDamageMultiplier = 0.15f },
                10f, 0f, 0.05f
            ));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Ahn'Kahar Blood Hunter's Battlegear (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Ahn'Kahar Blood Hunter's Battlegear",
                Stats = new Stats() { },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.SerpentWyvernStingsDoDamage,
                new Stats() { BonusAttackPowerMultiplier = 0.20f },
                10f, 0f, 0.05f
            ));
            #endregion
            #endregion
            #region PvP
            #region Gladiator's Pursuit (Seasons 5-7)
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Pursuit 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 50 },
                SetName = "Gladiator's Pursuit",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Pursuit 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusHunter_PvP_4pc = 2 },
                SetName = "Gladiator's Pursuit",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            #endregion
            #endregion
            #endregion

            #region Mage
            #region TBC
/*
            #region Tier 4 | Aldor
            defaultBuffs.Add(new Buff()
            {
                Name = "Aldor Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { AldorRegaliaInterruptProtection = 1 },
                SetName = "Aldor Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            #endregion
            #region Tier 5 | Tirisfal
            defaultBuffs.Add(new Buff()
            {
                Name = "Tirisfal Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ArcaneBlastBonus = .05f },
                SetName = "Tirisfal Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Tirisfal Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats(),
                SetName = "Tirisfal Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCrit, new Stats() { SpellPower = 70f }, 6.0f, 0.0f));
            #endregion
            #region Tier 6 | Tempest
            defaultBuffs.Add(new Buff()
            {
                Name = "Tempest Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { EvocationExtension = 2f },
                SetName = "Tempest Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Tempest Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusMageNukeMultiplier = 0.05f },
                SetName = "Tempest Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            #endregion
*/
            #endregion
            #region WotLK
            #region Tier 7 | Frostfire
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Frostfire Garb 2 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusManaGem = 0.25f },
                SetName = "Frostfire Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.ManaGem, new Stats() { SpellPower = 225f }, 15f, 0f));
            defaultBuffs.Add(new Buff()
            {
                Name = "Frostfire Garb 4 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { CritBonusDamage = 0.05f },
                SetName = "Frostfire Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            #endregion
            #region Tier 8 | Kirin Tor
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Kirin Tor Garb 2 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Kirin Tor Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.MageNukeCast, new Stats() { SpellPower = 350f }, 15f, 45f, 0.25f));
            defaultBuffs.Add(new Buff()
            {
                Name = "Kirin Tor Garb 4 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Mage4T8 = 1 },
                SetName = "Kirin Tor Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            #endregion
            #region Tier 9 | Khadgar's Regalia
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Khadgar's Regalia 2 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Mage2T9 = 1 },
                SetName = "Khadgar's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Khadgar's Regalia 4 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Mage4T9 = 1 },
                SetName = "Khadgar's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            #endregion
            #region Tier 10 | Bloodmage's Regalia
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Bloodmage's Regalia 2 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Mage2T10 = 1 },
                SetName = "Bloodmage's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Bloodmage's Regalia 4 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Mage4T10 = 1 },
                SetName = "Bloodmage's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            #endregion
            #endregion
            #region PvP
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 100, SpellPower = 29 },
                SetName = "Gladiator's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { SpellPower = 88 },
                SetName = "Gladiator's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            #endregion
            #endregion

            #region Paladin
            #region TBC
/*          
            #region Tier 4 |
            #endregion
            #region Tier 5 |
            #endregion
            #region Tier 6 | Lightbringer
            defaultBuffs.Add(new Buff()
            {
                Name = "Lightbringer Raiment 4 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { HolyLightCrit = .05f },
                SetName = "Lightbringer Raiment",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Lightbringer Raiment 2 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { FlashOfLightMultiplier = .05f },
                SetName = "Lightbringer Raiment",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            #endregion
 */
            #endregion
            #region WotLK
            #region Tier  7 | Redemption
            #region Regalia (Holy)
            defaultBuffs.Add(new Buff()
            {
                Name = "Redemption Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { HolyShockCrit = .1f },
                SetName = "Redemption Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Redemption Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { HolyLightPercentManaReduction = .05f },
                SetName = "Redemption Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            #endregion
            #region Plate (Protection)
            defaultBuffs.Add(new Buff()
            {
                Name = "Redemption Plate 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusHammerOfTheRighteousMultiplier = .1f },
                SetName = "Redemption Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Redemption Plate 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { DivineProtectionDurationBonus = 3f },
                SetName = "Redemption Plate",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            #endregion
            #region Battlegear (Retribution)
            defaultBuffs.Add(new Buff()
            {
                Name = "Redemption Battlegear 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { DivineStormMultiplier = .1f },
                SetName = "Redemption Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Redemption Battlegear 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { JudgementCDReduction = 1f },
                SetName = "Redemption Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            #endregion
            #endregion
            #region Tier  8 | Aegis
            #region Regalia (Holy)
            defaultBuffs.Add(new Buff()
            {
                Name = "Aegis Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { HolyShockHoTOnCrit = .15f },
                SetName = "Aegis Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Aegis Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { SacredShieldICDReduction = 2f },
                SetName = "Aegis Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            #endregion
            #region Plate (Protection)
            defaultBuffs.Add(new Buff()
            {
                Name = "Aegis Plate 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = 
                {
                    BonusSealOfVengeanceDamageMultiplier = .1f,
                    BonusSealOfCorruptionDamageMultiplier = .1f,
                    BonusSealOfRighteousnessDamageMultiplier = .1f,
                },
                SetName = "Aegis Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Aegis Plate 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats(),
                SetName = "Aegis Plate",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.ShieldofRighteousness, new Stats() { ShieldOfRighteousnessBlockValue = 225f }, 6.0f, 0.0f, 1.0f));
            #endregion
            #region Battlegear (Retribution)
            defaultBuffs.Add(new Buff()
            {
                Name = "Aegis Battlegear 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ExorcismMultiplier = .1f, HammerOfWrathMultiplier = .1f },
                SetName = "Aegis Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Aegis Battlegear 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { CrusaderStrikeCrit = .1f, DivineStormCrit = .1f },
                SetName = "Aegis Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            #endregion
            #endregion
            #region Tier  9 | Turalyon's
            #region Garb (Holy)

            defaultBuffs.Add(new Buff()
            {
                Name = "Turalyon's Garb 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusJudgementDuration = 10f },
                SetName = "Turalyon's Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });

            defaultBuffs.Add(new Buff()
            {
                Name = "Turalyon's Garb 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { FlashOfLightHoTMultiplier = 1.00f },
                SetName = "Turalyon's Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });

            #endregion
            #region Plate (Protection)
            defaultBuffs.Add(new Buff()
            {
                Name = "Turalyon's Plate 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    BonusHammerOfTheRighteousMultiplier = .05f,
                    // HandOfReckoningCooldownReduction = 2f // Hand of Reckoning is currently unmodeled.
                },
                SetName = "Turalyon's Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            /* Divine Protection and Forbearance are currently unmodeled.
            defaultBuffs.Add(new Buff()
            {
                Name = "Turalyon's Plate 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    DivineProtectionCooldownReduction = 30f,
                    ForbearanceDurationReduction = 30f,
                },
                SetName = "Turalyon's Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });*/
            #endregion
            #region Battlegear (Retribution)
            defaultBuffs.Add(new Buff()
            {
                Name = "Turalyon's Battlegear 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RighteousVengeanceCanCrit = 1f },
                SetName = "Turalyon's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Turalyon's Battlegear 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { JudgementCrit = .05f },
                SetName = "Turalyon's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            #endregion
            #endregion
            #region Tier 10 | Lightsworn
            #region Garb (Holy)
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Lightsworn Garb 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { DivineIlluminationHealingMultiplier = 0.35f },
                SetName = "Lightsworn Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });

            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Lightsworn Garb 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { HolyLightCastTimeReductionFromHolyShock = 0.3f },
                SetName = "Lightsworn Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });

            #endregion
            #region Plate (Protection)
            defaultBuffs.Add(new Buff()
            {
                Name = "Lightsworn Plate 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    BonusHammerOfTheRighteousMultiplier = .2f
                },
                SetName = "Lightsworn Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Lightsworn Plate 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats(),
                SetName = "Lightsworn Plate",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.DivinePlea, new Stats() { Dodge = .12f }, 10.0f, 0.0f, 1.0f));
            #endregion
            #region Battlegear (Retribution)
            defaultBuffs.Add(new Buff()
            {
                Name = "Lightsworn Battlegear 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    DivineStormRefresh = .4f
                },
                SetName = "Lightsworn Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Lightsworn Battlegear 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    SealMultiplier = .1f,
                    JudgementMultiplier = .1f
                },
                SetName = "Lightsworn Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            #endregion
            #endregion
            #endregion
            #region PvP
            #endregion
            #endregion

            #region Priest
            #region TBC
/*
            #region Tier 4 |
            #endregion
            #region Tier 5 | Avatar Raiment
            defaultBuffs.Add(new Buff()
            {
                Name = "Avatar Raiment 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ManaGainOnGreaterHealOverheal = 100f },
                SetName = "Avatar Raiment",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Avatar Raiment 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RenewDurationIncrease = 3f },
                SetName = "Avatar Raiment",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #region Tier 6 | Absolution
            defaultBuffs.Add(new Buff()
            {
                Name = "Vestments of Absolution 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusPoHManaCostReductionMultiplier = 0.1f },
                SetName = "Vestments of Absolution",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Vestments of Absolution 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusGHHealingMultiplier = 0.05f },
                SetName = "Vestments of Absolution",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Absolution Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { SWPDurationIncrease = 3f },
                SetName = "Absolution Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Absolution Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusMindBlastMultiplier = 0.1f },
                SetName = "Absolution Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
*/
            #endregion
            #region WotLK
            #region Tier 7 | Garb of Faith
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Regalia of Faith 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PrayerOfMendingExtraJumps = 1 },
                SetName = "Regalia of Faith",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Regalia of Faith 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { GreaterHealCostReduction = 0.05f },
                SetName = "Regalia of Faith",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Garb of Faith 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { MindBlastCostReduction = 0.1f },
                SetName = "Garb of Faith",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Garb of Faith 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ShadowWordDeathCritIncrease = 0.1f },
                SetName = "Garb of Faith",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #endregion
            #region Tier 8 | Sanctification
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Sanctification Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PrayerOfHealingExtraCrit = 0.1f },
                SetName = "Sanctification Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Sanctification Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PWSBonusSpellPowerProc = 250 },
                SetName = "Sanctification Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Sanctification Garb 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { DevouringPlagueBonusDamage = 0.15f },
                SetName = "Sanctification Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Sanctification Garb 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { MindBlastHasteProc = 240 },
                SetName = "Sanctification Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #endregion
            #region Tier 9 | Velen's
            #region Raiment
            defaultBuffs.Add(new Buff()
            {
                Name = "Velen's Raiment 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PriestHeal_T9_2pc = 0.2f },
                SetName = "Velen's Raiment",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Velen's Raiment 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PriestHeal_T9_4pc = 0.1f },
                SetName = "Velen's Raiment",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Velen's Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PriestDPS_T9_2pc = 6 },
                SetName = "Velen's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Velen's Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PriestDPS_T9_4pc = 0.05f },
                SetName = "Velen's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #endregion
            #region Tier 10 | Crimson Acolyte's
            #region Raiment
            defaultBuffs.Add(new Buff()
            {
                Name = "Crimson Acolyte's Raiment 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PriestHeal_T10_2pc = 0.33f },
                SetName = "Crimson Acolyte's Raiment",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Crimson Acolyte's Raiment 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PriestHeal_T10_4pc = 0.05f },
                SetName = "Crimson Acolyte's Raiment",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Crimson Acolyte's Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PriestDPS_T10_2pc = 0.05f },
                SetName = "Crimson Acolyte's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Crimson Acolyte's Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PriestDPS_T10_4pc = 0.51f },
                SetName = "Crimson Acolyte's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #endregion

            #endregion
            #region PvP
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Investiture 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 50f, SpellPower = 29 },
                SetName = "Gladiator's Investiture",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Investiture 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { WeakenedSoulDurationDecrease = 2f, SpellPower = 88 },
                SetName = "Gladiator's Investiture",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Raiment 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 50f, SpellPower = 29 },
                SetName = "Gladiator's Raiment",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Raiment 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { WeakenedSoulDurationDecrease = 2f, SpellPower = 88 },
                SetName = "Gladiator's Raiment",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #endregion

            #region Rogue
            #region TBC
/*
            #region Tier 4 | Netherblade
            defaultBuffs.Add(new Buff()
            {
                Name = "Netherblade 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusSnDDuration = 3f },
                SetName = "Netherblade",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Netherblade 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { CPOnFinisher = .15f },
                SetName = "Netherblade",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            #endregion
            #region Tier 5 | Deathmantle
            defaultBuffs.Add(new Buff()
            {
                Name = "Deathmantle 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusEvisEnvenomDamage = 40f },
                SetName = "Deathmantle",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Deathmantle 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusFreeFinisher = 1f },
                SetName = "Deathmantle",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            #endregion
            #region Tier 6 | Slayer
            defaultBuffs.Add(new Buff()
            {
                Name = "Slayer's Armor 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusSnDHaste = .05f },
                SetName = "Slayer's Armor",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Slayer's Armor 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusCPGDamage = .06f },
                SetName = "Slayer's Armor",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            #endregion
*/
            #endregion
            #region WotLK
            #region Tier 7 | Bonescythe
            defaultBuffs.Add(new Buff()
            {
                Name = "Bonescythe Battlegear 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RuptureDamageBonus = 0.1f },
                SetName = "Bonescythe Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Bonescythe Battlegear 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ComboMoveEnergyReduction = .05f },
                SetName = "Bonescythe Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            #endregion
            #region Tier 8 | Terrorblade
            defaultBuffs.Add(new Buff()
            {
                Name = "Terrorblade Battlegear 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusEnergyFromDP = 1f },
                SetName = "Terrorblade Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Terrorblade Battlegear 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RuptureCrit = 1f },
                SetName = "Terrorblade Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            #endregion
            #region Tier 9 | VanCleef/Garona
            defaultBuffs.Add(new Buff()
            {
                Name = "VanCleef's Battlegear 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ReduceEnergyCostFromRupture = 40f },
                SetName = "VanCleef's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "VanCleef's Battlegear 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusCPGCritChance = .05f },
                SetName = "VanCleef's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Garona's Battlegear 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ReduceEnergyCostFromRupture = 40f },
                SetName = "Garona's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Garona's Battlegear 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusCPGCritChance = .05f },
                SetName = "Garona's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            #endregion
            #region Tier 10 | Shadowblade's
            defaultBuffs.Add(new Buff()
            {
                Name = "Shadowblade's Battlegear (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusToTTEnergy = 30 },
                SetName = "Shadowblade's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Shadowblade's Battlegear (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ChanceOn3CPOnFinisher = 0.13f },
                SetName = "Shadowblade's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            #endregion
            #endregion
            #region PvP
            #endregion
            #endregion

            #region Shaman
            #region TBC
            /*
            #region Tier 4 | Cyclone
            defaultBuffs.Add(new Buff()
            {
                Name = "Cyclone Raiment 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Cyclone Raiment",
                Stats = { ManaSpringMp5Increase = 7.5f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Tier 5 | Cataclysm
            defaultBuffs.Add(new Buff()
            {
                Name = "Cataclysm Raiment 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Cataclysm Raiment",
                Stats = { LHWManaReduction = .05f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Tier 6 | Skyshatter
            defaultBuffs.Add(new Buff()
            {
                Name = "Skyshatter Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Skyshatter Regalia",
                Stats = { Mp5 = 15f, CritRating = 35f, SpellPower = 45f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Skyshatter Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Skyshatter Regalia",
                Stats = { LightningBoltDamageModifier = 5f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Skyshatter Raiment 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Skyshatter Raiment",
                Stats = { CHManaReduction = .1f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Skyshatter Raiment 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Skyshatter Raiment",
                Stats = { CHHealIncrease = .05f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
*/
            #endregion
            #region WotLK
            #region Tier 7 | Earthshatter
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Earthshatter Regalia (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Earthshatter Regalia",
                Stats = { WaterShieldIncrease = .1f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Earthshatter Regalia (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Earthshatter Regalia",
                Stats = { CHHWHealIncrease = .05f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Earthshatter Battlegear (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Earthshatter Battlegear",
                Stats = { Enhance2T7 = 0.1f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Earthshatter Battlegear (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Earthshatter Battlegear",
                Stats = { Enhance4T7 = 0.05f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Earthshatter Garb (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Earthshatter Garb",
                Stats = { LightningBoltCostReduction = 5f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Earthshatter Garb (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Earthshatter Garb",
                Stats = { BonusLavaBurstCritDamage = 10f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #endregion
            #region Tier 8 | Worldbreaker
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Worldbreaker Regalia (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Worldbreaker Regalia",
                Stats = { RTCDDecrease = 1f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Worldbreaker Regalia (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Worldbreaker Regalia",
                Stats = { CHCTDecrease = .2f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Worldbreaker Battlegear (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Worldbreaker Battlegear",
                Stats = { Enhance2T8 = 1f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Worldbreaker Battlegear (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Worldbreaker Battlegear",
                Stats = { Enhance4T8 = 1f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Worldbreaker Garb (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Worldbreaker Garb",
                Stats = { BonusFlameShockDoTDamage = .2f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Worldbreaker Garb (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Worldbreaker Garb",
                Stats = { LightningBoltCritDamageModifier = 0.08f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #endregion
            #region Tier 9 | Nobundo's
            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Thrall's/Nobundo's Garb (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Nobundo's Garb",
                Stats = { RestoSham2T9 = 1f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Thrall's/Nobundo's Garb (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Nobundo's Garb",
                Stats = { RestoSham4T9 = 1f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Thrall's/Nobundo's Regalia (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Nobundo's Regalia",
                Stats = { BonusFlameShockDuration = 9f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Thrall's/Nobundo's Regalia (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Nobundo's Regalia",
                Stats = { BonusLavaBurstDamageMultiplier = 0.1f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Battlegear
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Nobundo's Battlegear (T9) 2 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Enhance2T9 = 1 },
                SetName = "Nobundo's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Nobundo's Battlegear (T9) 4 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Enhance4T9 = 1 },
                SetName = "Nobundo's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #endregion
            #region Tier 10 | Frost Witch's
            #region Battlegear
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Frost Witch's Battlegear (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Enhance2T10 = 1 },
                SetName = "Frost Witch's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Frost Witch's Battlegear (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Enhance4T10 = 1 },
                SetName = "Frost Witch's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Frost Witch's Garb (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Frost Witch's Garb",
                Stats = { RestoSham2T10 = 1f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Frost Witch's Garb (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Frost Witch's Garb",
                Stats = { RestoSham4T10 = 1f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Regalia
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Frost Witch's Regalia (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Elemental2T10 = 1 },
                SetName = "Frost Witch's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Frost Witch's Regalia (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Elemental4T10 = 1 },
                SetName = "Frost Witch's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #endregion
            #endregion
            #region PvP
            #endregion
            #endregion

            #region Warlock
            #region TBC
            /*
            #region Tier 4 |
            #endregion
            #region Tier 5 |
            #endregion
            #region Tier 6 |
            #endregion
*/
            #endregion
            #region WotLK
            #region Tier 7 | Plagueheart
            defaultBuffs.Add(new Buff()
            {
                Name = "Plagueheart Garb 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Warlock2T7 = 0.10f },
                SetName = "Plagueheart Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Plagueheart Garb 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Warlock4T7 = 300f },
                SetName = "Plagueheart Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            #endregion
            #region Tier 8 | Deathbringer
            defaultBuffs.Add(new Buff()
            {
                Name = "Deathbringer Garb 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Warlock2T8 = 0.2f },
                SetName = "Deathbringer Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Deathbringer Garb 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Warlock4T8 = 0.05f },
                SetName = "Deathbringer Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            #endregion
            #region Tier 9 | Kel'Thuzad's Regalia
            //alliance [<3] set
            defaultBuffs.Add(new Buff()
            {
                Name = "Kel'Thuzad's Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Warlock2T9 = 0.10f },
                SetName = "Kel'Thuzad's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Kel'Thuzad's Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Warlock4T9 = 0.10f },
                SetName = "Kel'Thuzad's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            #endregion
            #region Tier 10 | Dark Coven's Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Dark Coven's Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Warlock2T10 = 0.05f },
                SetName = "Dark Coven's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Dark Coven's Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Warlock4T10 = 0.15f },
                SetName = "Dark Coven's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            #endregion
            #region PvP
            #endregion
            #endregion
            #endregion

            #region Warrior
            #region WotLK
            #region Tier  7 | Dreadnaught
            #region Plate
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreadnaught Plate (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusShieldSlamDamage = 0.1f },
                SetName = "Dreadnaught Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreadnaught Battlegear (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusWarrior_T7_2P_SlamDamage = 0.1f },
                SetName = "Dreadnaught Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #region Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreadnaught Battlegear (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusWarrior_T7_4P_RageProc = 5f },
                SetName = "Dreadnaught Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #endregion
            #region Tier  8 | Siegebreaker
            #region Plate
            defaultBuffs.Add(new Buff()
            {
                Name = "Siegebreaker Plate (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { DevastateCritIncrease = 0.1f },
                SetName = "Siegebreaker Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #region Battlegear
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Siegebreaker Battlegear (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { /*BonusWarrior_T8_2P_HasteProc = 1,*/ },
                SetName = "Siegebreaker Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.HSorSLHit, new Stats() { HasteRating = 150f, }, 5f, 0f, 0.40f));
            defaultBuffs.Add(new Buff()
            {
                Name = "Siegebreaker Battlegear (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusWarrior_T8_4P_MSBTCritIncrease = 0.1f },
                SetName = "Siegebreaker Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #endregion
            #region Tier  9 | Wrynn's
            #region Plate
            defaultBuffs.Add(new Buff()
            {
                Name = "Wrynn's Plate (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusDevastateDamage = 0.05f },
                SetName = "Wrynn's Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #region Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Wrynn's Battlegear (T9) 2 Piece Fury Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusWarrior_T9_2P_Crit = 0.02f },
                SetName = "Wrynn's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Wrynn's Battlegear (T9) 2 Piece Arms Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusWarrior_T9_2P_ArP = 0.06f },
                SetName = "Wrynn's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Wrynn's Battlegear (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusWarrior_T9_4P_SLHSCritIncrease = 0.05f },
                SetName = "Wrynn's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Wrynn's Plate (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusDevastateDamage = 0.05f },
                SetName = "Wrynn's Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #endregion
            #region Tier 10 | Ymirjar Lord's
            #region Plate
            defaultBuffs.Add(new Buff()
            {
                Name = "Ymirjar Lord's Plate 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusShieldSlamDamage = 0.2f, BonusShockwaveDamage = 0.2f },
                SetName = "Ymirjar Lord's Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #region Battlegear
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Ymirjar Lord's Battlegear (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusWarrior_T10_2P_DWAPProc = 1f, },
                SetName = "Ymirjar Lord's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.DeepWoundsTick,
                new Stats() { BonusAttackPowerMultiplier = 0.16f, },
                10f, 0f, 0.03f));
            defaultBuffs.Add(new Buff()
            {
                Name = "Ymirjar Lord's Battlegear (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusWarrior_T10_4P_BSSDProcChange = 1f, },
                SetName = "Ymirjar Lord's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #endregion
            #endregion
            #region PvP
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Battlegear (PvP) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 100, AttackPower = 50, },
                SetName = "Gladiator's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Battlegear (PvP) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusWarrior_PvP_4P_InterceptCDReduc = 5, AttackPower = 150, },
                SetName = "Gladiator's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #endregion
            #endregion // Set Bonuses

            #region Onyxia Drops set bonuses
            {
                Stats setEffect = new Stats() { SpellPower = 222f };
                // Guess at cooldown and proc chance, to be updated when more info available
                setEffect.AddSpecialEffect(new SpecialEffect(Trigger.HealingSpellHit, new Stats() { Healed = 2521 }, 0, 45, 0.2f));
                setEffect.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellHit, new Stats()
                { 
                    FireDamage = 2862,
                }, 0, 45, 0.2f));
                // Not sure what the stat for this DoT effect would be for damage classes
                //setEffect.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellHit, new Stats() {  }, 0, 45, 0.2f));
                defaultBuffs.Add(new Buff()
                {
                    Name = "Purified Shard of the Gods 2 Piece Bonus (10 man version)",
                    Group = "Set Bonuses",
                    ConflictingBuffs = new List<string>(new string[] { }),
                    Stats = setEffect ,
                    SetName = "Purified Shard of the Gods",
                    SetThreshold = 2
                });
            }
            {
                Stats setEffect = new Stats() { SpellPower = 250f };
                // Guess at cooldown and proc chance, to be updated when more info available
                setEffect.AddSpecialEffect(new SpecialEffect(Trigger.HealingSpellHit, new Stats() { Healed = 2811 }, 0, 45, 0.2f));
                setEffect.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellHit, new Stats()
                {
                    FireDamage = 3192,
                }, 0, 45, 0.2f));
                // Not sure what the stat for this DoT effect would be for damage classes
                //setEffect.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellHit, new Stats() {  }, 0, 45, 0.2f));
                defaultBuffs.Add(new Buff()
                {
                    Name = "Shiny Shard of the Gods 2 Piece Bonus (25 man version)",
                    Group = "Set Bonuses",
                    ConflictingBuffs = new List<string>(new string[] { }),
                    Stats = setEffect,
                    SetName = "Shiny Shard of the Gods",
                    SetThreshold = 2
                });
            }
            #endregion

            #region Profession Buffs
            #region Tailoring
            #region TBC
            /*defaultBuffs.Add(new Buff() {
                Name = "Spellfire 3 Piece Bonus",
                Group = "Set Bonuses",
                Source = "Spellfire Tailoring",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { SpellDamageFromIntellectPercentage = 0.07f },
                SetName = "Wrath of Spellfire",
                SetThreshold = 3
            });
            defaultBuffs.Add(buff = new Buff() {
                Name = "Spellstrike 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Spellstrike Infusion",
                SetThreshold = 2
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellHit, new Stats() { SpellPower = 92 }, 10, 0, 0.05f));
            defaultBuffs.Add(new Buff() {
                Name = "Primal Mooncloth 3 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { SpellCombatManaRegeneration = 0.05f },
                SetName = "Primal Mooncloth",
                SetThreshold = 3
            });*/
            #endregion
            #endregion
            #region Leatherworking
            #region TBC
            /*defaultBuffs.Add(new Buff() {
                Name = "Primalstrike 3 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { AttackPower = 40 },
                SetName = "Primal Intent",
                SetThreshold = 3
            });
            defaultBuffs.Add(new Buff() {
                Name = "Fel Leather 3 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { DodgeRating = 20 },
                SetName = "Fel Skin",
                SetThreshold = 3
            });
            defaultBuffs.Add(new Buff() {
                Name = "Heavy Clefthoof 3 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Strength = 20 },
                SetName = "Strength of the Clefthoof",
                SetThreshold = 3
            });
            defaultBuffs.Add(new Buff() {
                Name = "Windhawk Armor",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Mp5 = 8.0f },
                SetName = "Windhawk Armor",
                SetThreshold = 3
            });*/
            #endregion
            #endregion
            defaultBuffs.Add(new Buff() {
                Name = "Toughness",
                Group = "Profession Buffs",
                Source = "Mining [450]",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Stamina = 60f },
                Professions = new List<Profession>() { Profession.Mining },
            });
            defaultBuffs.Add(new Buff() {
                Name = "Master of Anatomy",
                Group = "Profession Buffs",
                Source = "Skinning [450]",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { CritRating = 40f },
                Professions = new List<Profession>() { Profession.Skinning },
            });
            #endregion

            #region Temporary Buffs
            defaultBuffs.Add(new Buff()
            {
                Name = "Dual Wielding Mob",
                Group = "Temporary Buffs",
                ConflictingBuffs = new List<string>(new string[] { "Dual Wielding Mob" }),
                Stats = { Miss = 0.2f }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Unbalancing Strike",
                Group = "Temporary Buffs",
                Stats = { Defense = -200.0f },
                ConflictingBuffs = new List<string>(new string[] { })
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Power of Vesperon",
                Group = "Temporary Buffs",
                Stats = { BonusHealthMultiplier = -0.25f },
                ConflictingBuffs = new List<string>(new string[] { })
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Chill of the Throne",
                Group = "Temporary Buffs",
                Stats = { Dodge = -0.20f },
                ConflictingBuffs = new List<string>(new string[] { })
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Luck of the Draw",
                Group = "Temporary Buffs",
                Source = "Dungeon Finder Groups",
                Stats = {
                    BonusHealthMultiplier = .05f,
                    BonusDamageMultiplier = .05f,
                    BonusHealingDoneMultiplier = .05f,
                },
                ConflictingBuffs = new List<string>(new string[] { "Strength of Wrynn" }),
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Strength of Wrynn/Hellscream's Warsong (1)",
                Group = "Temporary Buffs",
                Source = "Icecrown Citadel Raid",
                Stats = {
                    BonusHealthMultiplier = .05f,
                    BonusDamageMultiplier = .05f,
                    BonusHealingDoneMultiplier = .05f,
                },
                ConflictingBuffs = new List<string>(new string[] { "Strength of Wrynn" }),
            });
            defaultBuffs.Add(new Buff() {
                Name = "Strength of Wrynn/Hellscream's Warsong (2)",
                Group = "Temporary Buffs",
                Source = "Icecrown Citadel Raid",
                Stats = {
                    BonusHealthMultiplier = .10f,
                    BonusDamageMultiplier = .10f,
                    BonusHealingDoneMultiplier = .10f,
                },
                ConflictingBuffs = new List<string>(new string[] { "Strength of Wrynn" }),
            });
            defaultBuffs.Add(new Buff() {
                Name = "Strength of Wrynn/Hellscream's Warsong (3)",
                Group = "Temporary Buffs",
                Source = "Icecrown Citadel Raid",
                Stats = {
                    BonusHealthMultiplier = .15f,
                    BonusDamageMultiplier = .15f,
                    BonusHealingDoneMultiplier = .15f,
                },
                ConflictingBuffs = new List<string>(new string[] { "Strength of Wrynn" }),
            });
            defaultBuffs.Add(new Buff() {
                Name = "Strength of Wrynn/Hellscream's Warsong (4)",
                Group = "Temporary Buffs",
                Source = "Icecrown Citadel Raid",
                Stats = {
                    BonusHealthMultiplier = .20f,
                    BonusDamageMultiplier = .20f,
                    BonusHealingDoneMultiplier = .20f,
                },
                ConflictingBuffs = new List<string>(new string[] { "Strength of Wrynn" }),
            });
            defaultBuffs.Add(new Buff() {
                Name = "Strength of Wrynn/Hellscream's Warsong (5)",
                Group = "Temporary Buffs",
                Source = "Icecrown Citadel Raid",
                Stats = {
                    BonusHealthMultiplier = .25f,
                    BonusDamageMultiplier = .25f,
                    BonusHealingDoneMultiplier = .25f,
                },
                ConflictingBuffs = new List<string>(new string[] { "Strength of Wrynn" }),
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Strength of Wrynn/Hellscream's Warsong (6)",
                Group = "Temporary Buffs",
                Source = "Icecrown Citadel Raid",
                Stats = {
                    BonusHealthMultiplier = .30f,
                    BonusDamageMultiplier = .30f,
                    BonusHealingDoneMultiplier = .30f,
                },
                ConflictingBuffs = new List<string>(new string[] { "Strength of Wrynn" }),
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Expose Weakness",
                Group = "Temporary Buffs",
                Stats = { PhysicalDamageTakenMultiplier = 2.25f },
                ConflictingBuffs = new List<string>(new string[] { })
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Improved Lay On Hands",
                Group = "Temporary Buffs",
                Stats = { PhysicalDamageTakenMultiplier = -0.2f },
                ConflictingBuffs = new List<string>(new string[] { })
            });
            // Nightmare Seeds no longer drop from Nightmare Vine; Confirmed with Blue Post
            // "Sorry for the late response, but I just confirmed that Nightmare Seeds were intentionally removed. You will no longer be able to get them in game. Thanks!"
            // ~Ujumqin - http://forums.worldofwarcraft.com/thread.html?topicId=25170440756&pageNo=1&sid=1
            //defaultBuffs.Add(new Buff()
            //{
            //    Name = "Nightmare Seed",
            //    Group = "Temporary Buffs",
            //    Stats = { Health = 2000 },
            //    ConflictingBuffs = new List<string>(new string[] { })
            //});
            #endregion

            return defaultBuffs;
        }
    }
}
//1963...
