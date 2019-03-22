using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Rawr //O O . .
{
    [GenerateSerializer]
    public class Character
    {
        [XmlElement("Name")]
        public string _name;
        [XmlElement("Realm")]
        public string _realm;
        [XmlElement("Region")]
        public CharacterRegion _region = CharacterRegion.US;
        [XmlElement("Race")]
        public CharacterRace _race = CharacterRace.NightElf;
        [XmlElement("Faction")]
        private CharacterFaction _faction = CharacterFaction.Alliance;
        [XmlElement("Class")]
        public CharacterClass _class = CharacterClass.Druid;
        [XmlIgnore]
        public List<Buff> _activeBuffs;
        [XmlElement("ActiveBuffs")]
        public List<string> _activeBuffsXml;
        public const int SlotCount = 21;
        public const int OptimizableSlotCount = 19;
        [XmlIgnore]
        internal ItemInstance[] _item;

        [XmlIgnore]
        public List<ArmoryPet> ArmoryPets;
        [XmlElement("ArmoryPets")]
        public List<string> ArmoryPetsXml;

        public ItemInstance[] GetItems()
        {
            return (ItemInstance[])_item.Clone();
        }

        public void SetItems(ItemInstance[] items)
        {
            SetItems(items, true);
        }

        public void SetItems(ItemInstance[] items, bool invalidate)
        {
            int max = Math.Min(OptimizableSlotCount, items.Length);
            for (int slot = 0; slot < max; slot++)
            {
                _item[slot] = items[slot] == null ? null : items[slot].Clone();
            }
            // when called from optimizer we never want to invalidate since that causes creation of new item instances
            // and causes us to lose stats cache
            if (invalidate)
            {
                OnCalculationsInvalidated();
            }
        }

        public void SetItems(Character character)
        {
            SetItems(character, false, true);
        }

        public void SetItems(Character character, bool allSlots, bool invalidate)
        {
            int max = allSlots ? SlotCount : OptimizableSlotCount;
            for (int slot = 0; slot < max; slot++)
            {
                _item[slot] = character._item[slot] == null ? null : character._item[slot].Clone();
            }
            if (invalidate)
            {
                OnCalculationsInvalidated();
            }
        }

        #region Gem Slots
        private string GetGemmedId(CharacterSlot slot)
        {
            ItemInstance item = this[slot];
            if ((object)item == null) return null;
            return item.GemmedId;
        }
        private void SetGemmedId(CharacterSlot slot, string gemmedId)
        {
            if (string.IsNullOrEmpty(gemmedId)) _item[(int)slot] = null;
            else _item[(int)slot] = new ItemInstance(gemmedId); // don't call invalidations all the time while loading character
        }
        [XmlElement("Head")]
        public string _head { get { return GetGemmedId(CharacterSlot.Head); } set { SetGemmedId(CharacterSlot.Head, value); } }
        [XmlElement("Neck")]
        public string _neck { get { return GetGemmedId(CharacterSlot.Neck); } set { SetGemmedId(CharacterSlot.Neck, value); } }
        [XmlElement("Shoulders")]
        public string _shoulders { get { return GetGemmedId(CharacterSlot.Shoulders); } set { SetGemmedId(CharacterSlot.Shoulders, value); } }
        [XmlElement("Back")]
        public string _back { get { return GetGemmedId(CharacterSlot.Back); } set { SetGemmedId(CharacterSlot.Back, value); } }
        [XmlElement("Chest")]
        public string _chest { get { return GetGemmedId(CharacterSlot.Chest); } set { SetGemmedId(CharacterSlot.Chest, value); } }
        [XmlElement("Shirt")]
        public string _shirt { get { return GetGemmedId(CharacterSlot.Shirt); } set { SetGemmedId(CharacterSlot.Shirt, value); } }
        [XmlElement("Tabard")]
        public string _tabard { get { return GetGemmedId(CharacterSlot.Tabard); } set { SetGemmedId(CharacterSlot.Tabard, value); } }
        [XmlElement("Wrist")]
        public string _wrist { get { return GetGemmedId(CharacterSlot.Wrist); } set { SetGemmedId(CharacterSlot.Wrist, value); } }
        [XmlElement("Hands")]
        public string _hands { get { return GetGemmedId(CharacterSlot.Hands); } set { SetGemmedId(CharacterSlot.Hands, value); } }
        [XmlElement("Waist")]
        public string _waist { get { return GetGemmedId(CharacterSlot.Waist); } set { SetGemmedId(CharacterSlot.Waist, value); } }
        [XmlElement("Legs")]
        public string _legs { get { return GetGemmedId(CharacterSlot.Legs); } set { SetGemmedId(CharacterSlot.Legs, value); } }
        [XmlElement("Feet")]
        public string _feet { get { return GetGemmedId(CharacterSlot.Feet); } set { SetGemmedId(CharacterSlot.Feet, value); } }
        [XmlElement("Finger1")]
        public string _finger1 { get { return GetGemmedId(CharacterSlot.Finger1); } set { SetGemmedId(CharacterSlot.Finger1, value); } }
        [XmlElement("Finger2")]
        public string _finger2 { get { return GetGemmedId(CharacterSlot.Finger2); } set { SetGemmedId(CharacterSlot.Finger2, value); } }
        [XmlElement("Trinket1")]
        public string _trinket1 { get { return GetGemmedId(CharacterSlot.Trinket1); } set { SetGemmedId(CharacterSlot.Trinket1, value); } }
        [XmlElement("Trinket2")]
        public string _trinket2 { get { return GetGemmedId(CharacterSlot.Trinket2); } set { SetGemmedId(CharacterSlot.Trinket2, value); } }
        [XmlElement("MainHand")]
        public string _mainHand { get { return GetGemmedId(CharacterSlot.MainHand); } set { SetGemmedId(CharacterSlot.MainHand, value); } }
        [XmlElement("OffHand")]
        public string _offHand { get { return GetGemmedId(CharacterSlot.OffHand); } set { SetGemmedId(CharacterSlot.OffHand, value); } }
        [XmlElement("Ranged")]
        public string _ranged { get { return GetGemmedId(CharacterSlot.Ranged); } set { SetGemmedId(CharacterSlot.Ranged, value); } }
        [XmlElement("Projectile")]
        public string _projectile { get { return GetGemmedId(CharacterSlot.Projectile); } set { SetGemmedId(CharacterSlot.Projectile, value); } }
        [XmlElement("ProjectileBag")]
        public string _projectileBag { get { return GetGemmedId(CharacterSlot.ProjectileBag); } set { SetGemmedId(CharacterSlot.ProjectileBag, value); } }
        #endregion

        [XmlElement("AvailableItems")]
        public List<string> _availableItems;
        [XmlElement("CurrentModel")]
        public string _currentModel;
        [XmlElement("EnforceMetagemRequirements")]
        public bool _enforceMetagemRequirements = false;
        public int Level { get { return 80; } }

        #region Gemming Templates
        public List<GemmingTemplate> CustomGemmingTemplates { get; set; }
        public List<GemmingTemplate> GemmingTemplateOverrides { get; set; }

        private string gemmingTemplateModel;
        private List<GemmingTemplate> currentGemmingTemplates;


        [XmlIgnore]
        public List<GemmingTemplate> CurrentGemmingTemplates
        {
            get
            {
                if (currentGemmingTemplates == null || CurrentModel != gemmingTemplateModel)
                {
                    SaveGemmingTemplateOverrides();
                    GenerateGemmingTemplates();
                }
                return currentGemmingTemplates;
            }
        }

        private void SaveGemmingTemplateOverrides()
        {
            if (currentGemmingTemplates == null) return;
            List<GemmingTemplate> defaults = GemmingTemplate.AllTemplates[gemmingTemplateModel];
            GemmingTemplateOverrides.RemoveAll(template => template.Model == gemmingTemplateModel);
            foreach (GemmingTemplate template in defaults)
            {
                foreach (GemmingTemplate overrideTemplate in currentGemmingTemplates)
                {
                    if (template.Group == overrideTemplate.Group && template.BlueId == overrideTemplate.BlueId && template.MetaId == overrideTemplate.MetaId && template.Model == overrideTemplate.Model && template.PrismaticId == overrideTemplate.PrismaticId && template.RedId == overrideTemplate.RedId && template.YellowId == overrideTemplate.YellowId)
                    {
                        if (template.Enabled != overrideTemplate.Enabled)
                        {
                            GemmingTemplateOverrides.Add(overrideTemplate);
                            break;
                        }
                    }
                }
            }
        }

        private void GenerateGemmingTemplates()
        {
            List<GemmingTemplate> defaults = GemmingTemplate.CurrentTemplates;
            currentGemmingTemplates = new List<GemmingTemplate>();
            foreach (GemmingTemplate template in defaults)
            {
                GemmingTemplate toCopy = template;
                foreach (GemmingTemplate overrideTemplate in GemmingTemplateOverrides)
                {
                    if (template.Group == overrideTemplate.Group && template.BlueId == overrideTemplate.BlueId && template.MetaId == overrideTemplate.MetaId && template.Model == overrideTemplate.Model && template.PrismaticId == overrideTemplate.PrismaticId && template.RedId == overrideTemplate.RedId && template.YellowId == overrideTemplate.YellowId)
                    {
                        toCopy = overrideTemplate;
                        break;
                    }
                }
                currentGemmingTemplates.Add(new GemmingTemplate()
                {
                    BlueId = toCopy.BlueId,
                    Enabled = toCopy.Enabled,
                    Group = toCopy.Group,
                    MetaId = toCopy.MetaId,
                    Model = toCopy.Model,
                    PrismaticId = toCopy.PrismaticId,
                    RedId = toCopy.RedId,
                    YellowId = toCopy.YellowId,
                });
            }
            gemmingTemplateModel = CurrentModel;
        }
        #endregion

        #region Item Filter
        public List<ItemFilterEnabledOverride> ItemFilterEnabledOverride { get; set; }

        private void SaveItemFilterEnabledOverride()
        {
            ItemFilterEnabledOverride = new List<ItemFilterEnabledOverride>();
            foreach (var itemFilter in ItemFilter.FilterList)
            {
                SaveItemFilterEnabledOverride(itemFilter, ItemFilterEnabledOverride);
            }
            ItemFilterEnabledOverride.Add(new ItemFilterEnabledOverride() { Name = "Other", Enabled = ItemFilter.OtherEnabled });
        }

        private void SaveItemFilterEnabledOverride(ItemFilterRegex itemFilter, List<ItemFilterEnabledOverride> list)
        {
            ItemFilterEnabledOverride filterOverride = new ItemFilterEnabledOverride();
            filterOverride.Name = itemFilter.Name;
            filterOverride.Enabled = itemFilter.Enabled;
            if (itemFilter.RegexList.Count > 0)
            {
                filterOverride.SubFilterOverride = new List<ItemFilterEnabledOverride>();
                foreach (var subFilter in itemFilter.RegexList)
                {
                    SaveItemFilterEnabledOverride(subFilter, filterOverride.SubFilterOverride);
                }
                filterOverride.SubFilterOverride.Add(new ItemFilterEnabledOverride() { Name = "Other", Enabled = itemFilter.OtherRegexEnabled });                
            }
            list.Add(filterOverride);
        }

        public bool LoadItemFilterEnabledOverride()
        {
            bool triggerEvent = false;
            if (ItemFilterEnabledOverride == null || ItemFilterEnabledOverride.Count == 0) return false;
            ItemFilter.IsLoading = true;
            foreach (var filterOverride in ItemFilterEnabledOverride)
            {
                if (filterOverride.Name != "Other")
                {
                    LoadItemFilterEnabledOverride(filterOverride, ItemFilter.FilterList, ref triggerEvent);
                }
                else
                {
                    if (ItemFilter.OtherEnabled != filterOverride.Enabled)
                    {
                        ItemFilter.OtherEnabled = (bool)filterOverride.Enabled;
                        triggerEvent = true;
                    }
                }
            }
            ItemFilter.IsLoading = false;
            return triggerEvent;
        }

        private void LoadItemFilterEnabledOverride(ItemFilterEnabledOverride filterOverride, ItemFilterRegexList list, ref bool triggerEvent)
        {
            foreach (ItemFilterRegex itemFilter in list)
            {
                if (itemFilter.Name == filterOverride.Name)
                {
                    if (itemFilter.Enabled != filterOverride.Enabled)
                    {
                        itemFilter.Enabled = filterOverride.Enabled;
                        triggerEvent = true;
                    }
                    if (filterOverride.SubFilterOverride != null && filterOverride.SubFilterOverride.Count > 0)
                    {
                        foreach (var subOverride in filterOverride.SubFilterOverride)
                        {
                            if (subOverride.Name != "Other")
                            {
                                LoadItemFilterEnabledOverride(subOverride, itemFilter.RegexList, ref triggerEvent);
                            }
                            else
                            {
                                if (itemFilter.OtherRegexEnabled != subOverride.Enabled)
                                {
                                    itemFilter.OtherRegexEnabled = (bool)subOverride.Enabled;
                                    triggerEvent = true;
                                }
                            }
                        }
                    }
                    return;
                }
            }
        }
        #endregion

        public string CalculationToOptimize { get; set; }

        public List<OptimizationRequirement> OptimizationRequirements { get; set; }

        [XmlElement("CalculationOptions")]
        public SerializableDictionary<string, string> _serializedCalculationOptions;

        private Dictionary<string, ICalculationOptionBase> _calculationOptions;
        [XmlIgnore]
        public ICalculationOptionBase CalculationOptions {
            get 
            {
                ICalculationOptionBase ret;
                if (_calculationOptions.TryGetValue(CurrentModel, out ret))
                {
                    return ret;
                }
                else
                {
                    return LoadCalculationOptions();
                }
            }
            set { _calculationOptions[CurrentModel] = value; }
        }

        private ICalculationOptionBase LoadCalculationOptions()
        {
            if (_serializedCalculationOptions != null && _serializedCalculationOptions.ContainsKey(CurrentModel))
            {
                ICalculationOptionBase ret = Calculations.GetModel(CurrentModel)
                    .DeserializeDataObject(_serializedCalculationOptions[CurrentModel]);

                // set parent Character for models that need backward link
                ICharacterCalculationOptions characterCalculationOptions =
                    ret as ICharacterCalculationOptions;
                if (characterCalculationOptions != null)
                    characterCalculationOptions.Character = this;

                _calculationOptions[CurrentModel] = ret;
                return ret;
            }
            return null;
        }

        [XmlElement("Boss")]
        public BossOptions SerializableBoss {
            get { return BossOptions; }
            set { BossOptions = value.Clone(); }
        }
        [XmlIgnore]
        private BossOptions _bossOptions = null;
        [XmlIgnore]
        public BossOptions BossOptions
        {
            get { return _bossOptions ?? (_bossOptions = new BossOptions()); }
            set { _bossOptions = value; }
        }

        #region Talents
        [XmlElement("WarriorTalents")]
        public string SerializableWarriorTalents { get { return WarriorTalents.ToString(); } set { WarriorTalents = new WarriorTalents(value); } }
        //[XmlElement("WarriorTalentsCata")]
        //public string SerializableWarriorTalentsCata { get { return WarriorTalentsCata.ToString(); } set { WarriorTalentsCata = new WarriorTalentsCata(value); } }
        [XmlElement("PaladinTalents")]
        public string SerializablePaladinTalents { get { return PaladinTalents.ToString(); } set { PaladinTalents = new PaladinTalents(value); } }
        [XmlElement("HunterTalents")]
        public string SerializableHunterTalents { get { return HunterTalents.ToString(); }  set { HunterTalents = new HunterTalents(value); } }
        [XmlElement("RogueTalents")]
        public string SerializableRogueTalents { get { return RogueTalents.ToString(); }  set { RogueTalents = new RogueTalents(value); } }
        [XmlElement("PriestTalents")]
        public string SerializablePriestTalents { get { return PriestTalents.ToString(); } set { PriestTalents = new PriestTalents(value); } }
        [XmlElement("ShamanTalents")]
        public string SerializableShamanTalents { get { return ShamanTalents.ToString(); } set { ShamanTalents = new ShamanTalents(value); } }
        [XmlElement("MageTalents")]
        public string SerializableMageTalents { get { return MageTalents.ToString(); } set { MageTalents = new MageTalents(value); } }
        [XmlElement("WarlockTalents")]
        public string SerializableWarlockTalents { get { return WarlockTalents.ToString(); } set { WarlockTalents = new WarlockTalents(value); } }
        [XmlElement("DruidTalents")]
        public string SerializableDruidTalents { get { return DruidTalents.ToString(); } set { DruidTalents = new DruidTalents(value); } }
        [XmlElement("DeathKnightTalents")]
        public string SerializableDeathKnightTalents { get { return DeathKnightTalents.ToString(); } set { DeathKnightTalents = new DeathKnightTalents(value); } }

        [XmlIgnore]
        private WarriorTalents _warriorTalents = null;
        //[XmlIgnore]
        //private WarriorTalentsCata _warriorTalentsCata = null;
        [XmlIgnore]
        private PaladinTalents _paladinTalents = null;
        [XmlIgnore]
        private HunterTalents _hunterTalents = null;
        [XmlIgnore]
        private RogueTalents _rogueTalents = null;
        [XmlIgnore]
        private PriestTalents _priestTalents = null;
        [XmlIgnore]
        private ShamanTalents _shamanTalents = null;
        [XmlIgnore]
        private MageTalents _mageTalents = null;
        [XmlIgnore]
        private WarlockTalents _warlockTalents = null;
        [XmlIgnore]
        private DruidTalents _druidTalents = null;
        [XmlIgnore]
        private DeathKnightTalents _deathKnightTalents = null;

        [XmlIgnore]
        public WarriorTalents WarriorTalents { get { return _warriorTalents ?? (_warriorTalents = new WarriorTalents()); } set { _warriorTalents = value; } }
        //[XmlIgnore]
        //public WarriorTalentsCata WarriorTalentsCata { get { return _warriorTalentsCata ?? (_warriorTalentsCata = new WarriorTalentsCata()); } set { _warriorTalentsCata = value; } }
        [XmlIgnore]
        public PaladinTalents PaladinTalents { get { return _paladinTalents ?? (_paladinTalents = new PaladinTalents()); } set { _paladinTalents = value; } }
        [XmlIgnore]
        public HunterTalents HunterTalents { get { return _hunterTalents ?? (_hunterTalents = new HunterTalents()); } set { _hunterTalents = value; } }
        [XmlIgnore]
        public RogueTalents RogueTalents { get { return _rogueTalents ?? (_rogueTalents = new RogueTalents()); } set { _rogueTalents = value; } }
        [XmlIgnore]
        public PriestTalents PriestTalents { get { return _priestTalents ?? (_priestTalents = new PriestTalents()); } set { _priestTalents = value; } }
        [XmlIgnore]
        public ShamanTalents ShamanTalents { get { return _shamanTalents ?? (_shamanTalents = new ShamanTalents()); } set { _shamanTalents = value; } }
        [XmlIgnore]
        public MageTalents MageTalents { get { return _mageTalents ?? (_mageTalents = new MageTalents()); } set { _mageTalents = value; } }
        [XmlIgnore]
        public WarlockTalents WarlockTalents { get { return _warlockTalents ?? (_warlockTalents = new WarlockTalents()); } set { _warlockTalents = value; } }
        [XmlIgnore]
        public DruidTalents DruidTalents { get { return _druidTalents ?? (_druidTalents = new DruidTalents()); } set { _druidTalents = value; } }
        [XmlIgnore]
        public DeathKnightTalents DeathKnightTalents { get { return _deathKnightTalents ?? (_deathKnightTalents = new DeathKnightTalents()); } set { _deathKnightTalents = value; } }

        [XmlIgnore]
        public TalentsBase CurrentTalents
        {
            get
            {
                switch (Class)
                {
                    case CharacterClass.Warrior: return WarriorTalents;
                    case CharacterClass.Paladin: return PaladinTalents;
                    case CharacterClass.Hunter: return HunterTalents;
                    case CharacterClass.Rogue: return RogueTalents;
                    case CharacterClass.Priest: return PriestTalents;
                    case CharacterClass.Shaman: return ShamanTalents;
                    case CharacterClass.Mage: return MageTalents;
                    case CharacterClass.Warlock: return WarlockTalents;
                    case CharacterClass.Druid: return DruidTalents;
                    case CharacterClass.DeathKnight: return DeathKnightTalents;
                    default: return DruidTalents;
                }
            }
            set
            {
                switch (Class)
                {
                    case CharacterClass.Warrior: WarriorTalents = value as WarriorTalents; break;
                    case CharacterClass.Paladin: PaladinTalents = value as PaladinTalents; break;
                    case CharacterClass.Hunter: HunterTalents = value as HunterTalents; break;
                    case CharacterClass.Rogue: RogueTalents = value as RogueTalents; break;
                    case CharacterClass.Priest: PriestTalents = value as PriestTalents; break;
                    case CharacterClass.Shaman: ShamanTalents = value as ShamanTalents; break;
                    case CharacterClass.Mage: MageTalents = value as MageTalents; break;
                    case CharacterClass.Warlock: WarlockTalents = value as WarlockTalents; break;
                    case CharacterClass.Druid: DruidTalents = value as DruidTalents; break;
                    case CharacterClass.DeathKnight: DeathKnightTalents = value as DeathKnightTalents; break;
                    default: DruidTalents = value as DruidTalents; break;
                }
            }
        }
        /// <summary>
        /// This function will return a Cata talent tree if there is one, otherwise returns the Wotlk tree
        /// </summary>
        [XmlIgnore]
        public TalentsBase CurrentTalentsCata
        {
            get
            {
                switch (Class)
                {
                    //case CharacterClass.Warrior: return WarriorTalentsCata;
                    case CharacterClass.Paladin: return PaladinTalents;
                    case CharacterClass.Hunter: return HunterTalents;
                    case CharacterClass.Rogue: return RogueTalents;
                    case CharacterClass.Priest: return PriestTalents;
                    case CharacterClass.Shaman: return ShamanTalents;
                    case CharacterClass.Mage: return MageTalents;
                    case CharacterClass.Warlock: return WarlockTalents;
                    case CharacterClass.Druid: return DruidTalents;
                    case CharacterClass.DeathKnight: return DeathKnightTalents;
                    default: return DruidTalents;
                }
            }
            set
            {
                switch (Class)
                {
                    //case CharacterClass.Warrior: WarriorTalentsCata = value as WarriorTalentsCata; break;
                    case CharacterClass.Paladin: PaladinTalents = value as PaladinTalents; break;
                    case CharacterClass.Hunter: HunterTalents = value as HunterTalents; break;
                    case CharacterClass.Rogue: RogueTalents = value as RogueTalents; break;
                    case CharacterClass.Priest: PriestTalents = value as PriestTalents; break;
                    case CharacterClass.Shaman: ShamanTalents = value as ShamanTalents; break;
                    case CharacterClass.Mage: MageTalents = value as MageTalents; break;
                    case CharacterClass.Warlock: WarlockTalents = value as WarlockTalents; break;
                    case CharacterClass.Druid: DruidTalents = value as DruidTalents; break;
                    case CharacterClass.DeathKnight: DeathKnightTalents = value as DeathKnightTalents; break;
                    default: DruidTalents = value as DruidTalents; break;
                }
            }
        }
        #endregion

        // set to true to suppress ItemsChanged event
        [XmlIgnore]
        public bool IsLoading { get; set; }
        
        [XmlIgnore]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [XmlIgnore]
        public string Realm
        {
            get { return _realm; }
            set { _realm = value; }
        }
        [XmlIgnore]
        public CharacterRegion Region
        {
            get { return _region; }
            set { _region = value; }
        }
        [XmlIgnore]
        public int RegionIndex
        {
            get { return (int)Region; }
            set { Region = (CharacterRegion)value; }
        }
        [XmlIgnore]
        public CharacterRace Race
        {
            get { return _race; }
            set
            {
                if (_race != value)
                {
                    _race = value;
                    SetFaction();
                    OnRaceChanged();
                    OnCalculationsInvalidated();
                }
            }
        }

        [XmlIgnore]
        public int RaceIndex
        {
            get { return (int)Race; }
            set { Race = (CharacterRace)value; }
        }

        [XmlIgnore]
        public int PriProfIndex
        {
            get { return Profs.ProfessionToIndex(PrimaryProfession); }
            set { PrimaryProfession = Profs.IndexToProfession(value); }
        }

        [XmlIgnore]
        public int SecProfIndex
        {
            get { return Profs.ProfessionToIndex(SecondaryProfession); }
            set { SecondaryProfession = Profs.IndexToProfession(value); }
        }

        [XmlIgnore]
        public CharacterFaction Faction
        {
            get { return _faction; }
        }
        [XmlIgnore]
        public CharacterClass Class
        {
            get { return _class; }
            set
            {
                _class = value;
                OnClassChanged();
            }
        }

        [XmlIgnore]
        public int ClassIndex
        {
            get { return (int)Class; }
            set { Class = (CharacterClass)value; }
        }

        [XmlIgnore]
        public List<Buff> ActiveBuffs
        {
            get { return _activeBuffs; }
            set { _activeBuffs = value; ValidateActiveBuffs(); }
        }

        public void ActiveBuffsAdd(Buff buff)
        {
            if (buff != null)
                ActiveBuffs.Add(buff);
        }

        public void ActiveBuffsAdd(string buffName)
        {
            Buff buff = Buff.GetBuffByName(buffName);
            if (buff != null && !ActiveBuffs.Contains(buff))
            {
                ActiveBuffs.Add(buff);
            }
        }

        public bool ActiveBuffsContains(string buff)
        {
            if (_activeBuffs == null)
                return false;
            return _activeBuffs.FindIndex(x => x.Name == buff) >= 0;
        }

        public bool ActiveBuffsConflictingBuffContains(string conflictingBuff)
        {
            return _activeBuffs.FindIndex(x => x.ConflictingBuffs.Contains(conflictingBuff)) >= 0;
        }

        /// <summary>
        /// This function forces any duplicate buffs off the current buff list
        /// and enforces buffs that should be in there due to race/profession
        /// </summary>
        public void ValidateActiveBuffs() {
            // First let's check for Duplicate Buffs and remove them
            Buff cur = null;
            for (int i = 0; i < ActiveBuffs.Count;/*no default iter*/)
            {
                cur = ActiveBuffs[i];
                if (cur == null) { ActiveBuffs.RemoveAt(i); continue; } // don't iterate
                int count = 0;
                foreach (Buff iter in ActiveBuffs) {
                    if (iter.Name == cur.Name) count++;
                }
                if (count > 1) { ActiveBuffs.RemoveAt(i); continue; } // remove this first one, we'll check the other one(s) again later, don't iterate
                // At this point, we didn't fail so we can move on to the next one
                i++;
            }

            // Next let's check for Heroic Presence. If you are a Draenei and don't have it, you need it
            if (Race == CharacterRace.Draenei && !ActiveBuffsContains("Heroic Presence")) { ActiveBuffsAdd("Heroic Presence"); }
            // If you are Horde, you will never have this so let's take it out
            if (Faction == CharacterFaction.Horde && ActiveBuffsContains("Heroic Presence")) ActiveBuffs.Remove(Buff.GetBuffByName("Heroic Presence"));

            // Finally, let's check Profession Buffs that should be automatically applied
            // Toughness buff from Mining
            if (HasProfession(Profession.Mining) && !ActiveBuffsContains("Toughness")) { ActiveBuffsAdd("Toughness"); }
            else if (!HasProfession(Profession.Mining) && ActiveBuffsContains("Toughness")) { ActiveBuffs.Remove(Buff.GetBuffByName("Toughness")); }
            // Master of Anatomy from Skinning
            if (HasProfession(Profession.Skinning) && !ActiveBuffsContains("Master of Anatomy")) { ActiveBuffsAdd("Master of Anatomy"); }
            else if (!HasProfession(Profession.Skinning) && ActiveBuffsContains("Master of Anatomy")) { ActiveBuffs.Remove(Buff.GetBuffByName("Master of Anatomy")); }

            // Force a recalc, this will also update the Buffs tab since it's designed to react to that
            OnCalculationsInvalidated();
        }

        #region Items in Slots
        [XmlIgnore]
        public ItemInstance Head { get { return this[CharacterSlot.Head]; } set { this[CharacterSlot.Head] = value; } }
        [XmlIgnore]
        public ItemInstance Neck { get { return this[CharacterSlot.Neck]; } set { this[CharacterSlot.Neck] = value; } }
        [XmlIgnore]
        public ItemInstance Shoulders { get { return this[CharacterSlot.Shoulders]; } set { this[CharacterSlot.Shoulders] = value; } }
        [XmlIgnore]
        public ItemInstance Back { get { return this[CharacterSlot.Back]; } set { this[CharacterSlot.Back] = value; } }
        [XmlIgnore]
        public ItemInstance Chest { get { return this[CharacterSlot.Chest]; } set { this[CharacterSlot.Chest] = value; } }
        [XmlIgnore]
        public ItemInstance Shirt { get { return this[CharacterSlot.Shirt]; } set { this[CharacterSlot.Shirt] = value; } }
        [XmlIgnore]
        public ItemInstance Tabard { get { return this[CharacterSlot.Tabard]; } set { this[CharacterSlot.Tabard] = value; } }
        [XmlIgnore]
        public ItemInstance Wrist { get { return this[CharacterSlot.Wrist]; } set { this[CharacterSlot.Wrist] = value; } }
        [XmlIgnore]
        public ItemInstance Hands { get { return this[CharacterSlot.Hands]; } set { this[CharacterSlot.Hands] = value; } }
        [XmlIgnore]
        public ItemInstance Waist { get { return this[CharacterSlot.Waist]; } set { this[CharacterSlot.Waist] = value; } }
        [XmlIgnore]
        public ItemInstance Legs { get { return this[CharacterSlot.Legs]; } set { this[CharacterSlot.Legs] = value; } }
        [XmlIgnore]
        public ItemInstance Feet { get { return this[CharacterSlot.Feet]; } set { this[CharacterSlot.Feet] = value; } }
        [XmlIgnore]
        public ItemInstance Finger1 { get { return this[CharacterSlot.Finger1]; } set { this[CharacterSlot.Finger1] = value; } }
        [XmlIgnore]
        public ItemInstance Finger2 { get { return this[CharacterSlot.Finger2]; } set { this[CharacterSlot.Finger2] = value; } }
        [XmlIgnore]
        public ItemInstance Trinket1 { get { return this[CharacterSlot.Trinket1]; } set { this[CharacterSlot.Trinket1] = value; } }
        [XmlIgnore]
        public ItemInstance Trinket2 { get { return this[CharacterSlot.Trinket2]; } set { this[CharacterSlot.Trinket2] = value; } }
        [XmlIgnore]
        public ItemInstance MainHand { get { return this[CharacterSlot.MainHand]; } set { this[CharacterSlot.MainHand] = value; } }
        [XmlIgnore]
        public ItemInstance OffHand { get { return this[CharacterSlot.OffHand]; } set { this[CharacterSlot.OffHand] = value; } }
        [XmlIgnore]
        public ItemInstance Ranged { get { return this[CharacterSlot.Ranged]; } set { this[CharacterSlot.Ranged] = value; } }
        [XmlIgnore]
        public ItemInstance Projectile { get { return this[CharacterSlot.Projectile]; } set { this[CharacterSlot.Projectile] = value; } }
        [XmlIgnore]
        public ItemInstance ProjectileBag { get { return this[CharacterSlot.ProjectileBag]; } set { this[CharacterSlot.ProjectileBag] = value; } }
        //[XmlIgnore]
        //public Item ExtraWristSocket { get { return this[CharacterSlot.ExtraWristSocket]; } set { this[CharacterSlot.ExtraWristSocket] = value; } }
        //[XmlIgnore]
        //public Item ExtraHandsSocket { get { return this[CharacterSlot.ExtraHandsSocket]; } set { this[CharacterSlot.ExtraHandsSocket] = value; } }
        //[XmlIgnore]
        //public Item ExtraWaistSocket { get { return this[CharacterSlot.ExtraWaistSocket]; } set { this[CharacterSlot.ExtraWaistSocket] = value; } }

        // leave in for now to reduce rebinding needed
        [XmlIgnore]
        public Enchant HeadEnchant { get { return GetEnchantBySlot(CharacterSlot.Head); } set { SetEnchantBySlot(CharacterSlot.Head, value); } }
        [XmlIgnore]
        public Enchant ShouldersEnchant  { get { return GetEnchantBySlot(CharacterSlot.Shoulders); } set { SetEnchantBySlot(CharacterSlot.Shoulders, value); } }
        [XmlIgnore]
        public Enchant BackEnchant { get { return GetEnchantBySlot(CharacterSlot.Back); } set { SetEnchantBySlot(CharacterSlot.Back, value); } }
        [XmlIgnore]
        public Enchant ChestEnchant { get { return GetEnchantBySlot(CharacterSlot.Chest); } set { SetEnchantBySlot(CharacterSlot.Chest, value); } }
        [XmlIgnore]
        public Enchant WristEnchant { get { return GetEnchantBySlot(CharacterSlot.Wrist); } set { SetEnchantBySlot(CharacterSlot.Wrist, value); } }
        [XmlIgnore]
        public Enchant HandsEnchant { get { return GetEnchantBySlot(CharacterSlot.Hands); } set { SetEnchantBySlot(CharacterSlot.Hands, value); } }
        [XmlIgnore]
        public Enchant LegsEnchant { get { return GetEnchantBySlot(CharacterSlot.Legs); } set { SetEnchantBySlot(CharacterSlot.Legs, value); } }
        [XmlIgnore]
        public Enchant FeetEnchant { get { return GetEnchantBySlot(CharacterSlot.Feet); } set { SetEnchantBySlot(CharacterSlot.Feet, value); } }
        [XmlIgnore]
        public Enchant Finger1Enchant { get { return GetEnchantBySlot(CharacterSlot.Finger1); } set { SetEnchantBySlot(CharacterSlot.Finger1, value); } }
        [XmlIgnore]
        public Enchant Finger2Enchant { get { return GetEnchantBySlot(CharacterSlot.Finger2); } set { SetEnchantBySlot(CharacterSlot.Finger2, value); } }
        [XmlIgnore]
        public Enchant MainHandEnchant { get { return GetEnchantBySlot(CharacterSlot.MainHand); } set { SetEnchantBySlot(CharacterSlot.MainHand, value); } }
        [XmlIgnore]
        public Enchant OffHandEnchant { get { return GetEnchantBySlot(CharacterSlot.OffHand); } set { SetEnchantBySlot(CharacterSlot.OffHand, value); } }
        [XmlIgnore]
        public Enchant RangedEnchant { get { return GetEnchantBySlot(CharacterSlot.Ranged); } set { SetEnchantBySlot(CharacterSlot.Ranged, value); } }
        #endregion

        [XmlIgnore]
        public string CurrentModel
        {
            get
            {
                if (string.IsNullOrEmpty(_currentModel))
                {
                    foreach (KeyValuePair<string, Type> kvp in Calculations.Models)
                        if (kvp.Value == Calculations.Instance.GetType())
                            _currentModel = kvp.Key;
                }
                return _currentModel;
            }
            set
            {
                _currentModel = value;
            }
        }

        [XmlIgnore]
        public CalculationsBase CurrentCalculations
        {
            get
            {
                return Calculations.GetModel(CurrentModel);
            }
        }

        [XmlIgnore]
        public bool EnforceGemRequirements
        {
            get { return _enforceMetagemRequirements; }
            set
            {
                _enforceMetagemRequirements = value;
                OnCalculationsInvalidated();
            }
        }

        [XmlIgnore]
        public bool DisableBuffAutoActivation { get; set; }

        public void InvalidateItemInstances()
        {
            if (_relevantItems != null)
            {
                _relevantItems.Clear();
                _relevantItemInstances.Clear();
            }
            if (!IsLoading)
            {
                for (int i = 0; i < _item.Length; i++)
                {
                    if (_item[i] != null)
                    {
                        _item[i] = new ItemInstance(_item[i].Id, _item[i].Gem1Id, _item[i].Gem2Id, _item[i].Gem3Id, _item[i].EnchantId);
                    }
                }
            }
        }

        public void InvalidateItemInstances(CharacterSlot slot)
        {
            _relevantItemInstances.Remove(slot);
            int i = (int)slot;
            _item[i] = new ItemInstance(_item[i].Id, _item[i].Gem1Id, _item[i].Gem2Id, _item[i].Gem3Id, _item[i].EnchantId);
        }

        [XmlIgnore]
        private List<ItemInstance> _customItemInstances;

        public List<ItemInstance> CustomItemInstances
        {
            get
            {
                return _customItemInstances;
            }
            set
            {
                _customItemInstances = value;
                InvalidateItemInstances();
            }
        }

        private bool waistBSSocket;
        public bool WaistBlacksmithingSocketEnabled { 
            get 
            { 
                return waistBSSocket; 
            }            
            set
            {
                waistBSSocket = value;
                OnCalculationsInvalidated();
            }
        }

        private bool handsBSSocket;
        public bool HandsBlacksmithingSocketEnabled
        {
            get
            {
                return handsBSSocket;
            }
            set
            {
                handsBSSocket = value;
                OnCalculationsInvalidated();
            }
        }

        private bool wristBSSocket;
        public bool WristBlacksmithingSocketEnabled 
        {
            get
            {
                return wristBSSocket;
            }
            set
            {
                wristBSSocket = value;
                OnCalculationsInvalidated();
            }
        }

        private Profession _primaryProfession = Profession.None;
        public Profession PrimaryProfession { 
            get { return _primaryProfession; }
            set
            {
                if (_primaryProfession != value)
                {
                    _primaryProfession = value;
                    Calculations.UpdateProfessions(this);
                    ValidateActiveBuffs();
                }
            }
        }
        private Profession _secondaryProfession = Profession.None;
        public Profession SecondaryProfession
        {
            get { return _secondaryProfession; }
            set
            {
                if (_secondaryProfession != value)
                {
                    _secondaryProfession = value;
                    Calculations.UpdateProfessions(this);
                    ValidateActiveBuffs();
                }
            }
        }

        public bool HasProfession(Profession p)
        {
            if (PrimaryProfession == p) { return true; }
            if (SecondaryProfession == p) { return true; }
            return false;
        }

        public bool HasProfession(List<Profession> list)
        {
            foreach (Profession p in list)
            {
                if (HasProfession(p))
                    return true;
            }
            return false;
        }

        public static event EventHandler RaceChanged;
        protected static void OnRaceChanged()
        {
            if (RaceChanged != null)
                RaceChanged(null, EventArgs.Empty);
        }

        private void SetFaction()
        {
            if (_race == CharacterRace.Draenei || _race == CharacterRace.Dwarf || _race == CharacterRace.Gnome || _race == CharacterRace.Human || _race == CharacterRace.NightElf)
                _faction = CharacterFaction.Alliance;
            else
            {
                _faction = CharacterFaction.Horde;

                // horde don't get heroic presence muahahaha
                ActiveBuffs.RemoveAll(b => b.Name == "Heroic Presence");
            }
        }
         
        [XmlIgnore]
        private Dictionary<CharacterSlot, List<ItemInstance>> _relevantItemInstances;

        [XmlIgnore]
        private Dictionary<CharacterSlot, List<Item>> _relevantItems;

        public List<ItemInstance> GetRelevantItemInstances(CharacterSlot slot)
        {
            bool blacksmithingSocket = false;
            if ((slot == CharacterSlot.Waist && WaistBlacksmithingSocketEnabled) || (slot == CharacterSlot.Hands && HandsBlacksmithingSocketEnabled) || (slot == CharacterSlot.Wrist && WristBlacksmithingSocketEnabled))
            {
                blacksmithingSocket = true;
            }
            List<ItemInstance> items;
            if (!_relevantItemInstances.TryGetValue(slot, out items))
            {
                Dictionary<int, bool> itemChecked = new Dictionary<int, bool>();
                items = new List<ItemInstance>();
                foreach (Item item in ItemCache.RelevantItems)
                {
                    if (item.FitsInSlot(slot, this) && item.FitsFaction(Race))
                    {
                        itemChecked[item.Id] = true;
                        List<ItemInstance> itemInstances = new List<ItemInstance>();
                        foreach (GemmingTemplate template in CurrentGemmingTemplates)
                        {
                            if (template.Enabled)
                            {
                                ItemInstance instance = template.GetItemInstance(item, GetEnchantBySlot(slot), blacksmithingSocket);
                                if (!itemInstances.Contains(instance)) itemInstances.Add(instance);
                            }
                        }
                        foreach (GemmingTemplate template in CustomGemmingTemplates)
                        {
                            if (template.Enabled && template.Model == CurrentModel)
                            {
                                ItemInstance instance = template.GetItemInstance(item, GetEnchantBySlot(slot), blacksmithingSocket);
                                if (!itemInstances.Contains(instance)) itemInstances.Add(instance);
                            }
                        }
                        items.AddRange(itemInstances);
                    }
                }
                // add custom instances
                foreach (ItemInstance item in CustomItemInstances)
                {
                    if (item.Item != null && item.Item.FitsInSlot(slot, this)) // item.Item can be null if you're loading character with custom items that are not present on this install
                    {
                        // if it's already in make sure to set force visible to true
                        int index = items.IndexOf(item);
                        if (index >= 0)
                        {
                            items[index] = item;
                        }
                        else
                        {
                            items.Add(item);
                        }
                    }
                }
                // add available instances
                foreach (string availableItem in AvailableItems)
                {
                    string[] ids = availableItem.Split('.');
                    if (ids.Length == 1 || ids[1] == "*")
                    {
                        // we have an available item that might be filtered out
                        Item item = ItemCache.FindItemById(int.Parse(ids[0]));
                        if (item != null)
                        {
                            if (item.FitsInSlot(slot, this))
                            {
                                bool check = itemChecked.ContainsKey(item.Id);
                                Enchant enchant;
                                if (ids.Length < 5 || ids[4] == "*")
                                {
                                    if (check)
                                    {
                                        // we've already processed this one
                                        continue;
                                    }
                                    enchant = GetEnchantBySlot(slot);
                                }
                                else
                                {
                                    Enchant currentEnchant = GetEnchantBySlot(slot);
                                    int currentId = currentEnchant != null ? currentEnchant.Id : 0;
                                    if (check && int.Parse(ids[4]) == currentId)
                                    {
                                        continue;
                                    }
                                    enchant = Enchant.FindEnchant(int.Parse(ids[4]), item.Slot, this);
                                }
                                List<ItemInstance> itemInstances = new List<ItemInstance>();
                                foreach (GemmingTemplate template in CurrentGemmingTemplates)
                                {
                                    if (template.Enabled)
                                    {
                                        ItemInstance instance = template.GetItemInstance(item, enchant, blacksmithingSocket);
                                        if (!itemInstances.Contains(instance)) itemInstances.Add(instance);
                                    }
                                }
                                foreach (GemmingTemplate template in CustomGemmingTemplates)
                                {
                                    if (template.Enabled && template.Model == CurrentModel)
                                    {
                                        ItemInstance instance = template.GetItemInstance(item, enchant, blacksmithingSocket);
                                        if (!itemInstances.Contains(instance)) itemInstances.Add(instance);
                                    }
                                }
                                if (check)
                                {
                                    foreach (ItemInstance instance in itemInstances)
                                    {
                                        if (!items.Contains(instance)) items.Add(instance);
                                    }
                                }
                                else
                                {
                                    items.AddRange(itemInstances);
                                }
                            }
                            itemChecked[item.Id] = true;
                        }
                    }
                }
                foreach (string availableItem in AvailableItems)
                {
                    // only have to worry about items with gems, others should be visible already
                    string[] ids = availableItem.Split('.');
                    if (ids.Length > 1 && ids[1] != "*")
                    {
                        Item item = ItemCache.FindItemById(int.Parse(ids[0]));
                        if (item.FitsInSlot(slot, this))
                        {
                            Enchant enchant = GetEnchantBySlot(slot) ?? new Enchant();
                            ItemInstance instance = new ItemInstance(int.Parse(ids[0]), int.Parse(ids[1]), int.Parse(ids[2]), int.Parse(ids[3]), (ids[4] == "*") ? enchant.Id : int.Parse(ids[4]));
                            instance.ForceDisplay = true;
                            // we want to force display even if it's already present (might be lower then top N)
                            int index = items.IndexOf(instance);
                            if (index < 0)
                            {
                                items.Add(instance);
                            }
                            else
                            {
                                items[index] = instance;
                            }
                        }
                    }
                } 
                _relevantItemInstances[slot] = items;
            }
            return items;
        }

        public void ClearRelevantGems() { _relevantItems.Remove(CharacterSlot.Gems); }
        public List<Item> GetRelevantItems(CharacterSlot slot) { return GetRelevantItems(slot, ItemSlot.None); }
        public List<Item> GetRelevantItems(CharacterSlot slot, ItemSlot gemColour)
        {
            List<Item> items;
            if (!_relevantItems.TryGetValue(slot, out items))
            {
                items = new List<Item>();
                foreach (Item item in ItemCache.RelevantItems)
                {
                    if (item.FitsInSlot(slot, this))
                    {
                        if ((gemColour == ItemSlot.None) ||
                            (gemColour == ItemSlot.Red && item.IsRedGem) ||
                            (gemColour == ItemSlot.Yellow && item.IsYellowGem) ||
                            (gemColour == ItemSlot.Blue && item.IsBlueGem))
                        {
                            items.Add(item);
                        }
                    }
                }
                _relevantItems[slot] = items;
            }
            return items;
        }

        public void AssignAllTalentsFromCharacter(Character character, bool clone)
        {
            if (clone)
            {
                WarriorTalents = (WarriorTalents)character.WarriorTalents.Clone();
                //WarriorTalentsCata = (WarriorTalentsCata)character.WarriorTalentsCata.Clone();
                PaladinTalents = (PaladinTalents)character.PaladinTalents.Clone();
                HunterTalents = (HunterTalents)character.HunterTalents.Clone();
                RogueTalents = (RogueTalents)character.RogueTalents.Clone();
                PriestTalents = (PriestTalents)character.PriestTalents.Clone();
                ShamanTalents = (ShamanTalents)character.ShamanTalents.Clone();
                MageTalents = (MageTalents)character.MageTalents.Clone();
                WarlockTalents = (WarlockTalents)character.WarlockTalents.Clone();
                DruidTalents = (DruidTalents)character.DruidTalents.Clone();
                DeathKnightTalents = (DeathKnightTalents)character.DeathKnightTalents.Clone();
            }
            else
            {
                _warriorTalents = character._warriorTalents;
                //_warriorTalentsCata = character._warriorTalentsCata;
                _paladinTalents = character._paladinTalents;
                _hunterTalents = character._hunterTalents;
                _rogueTalents = character._rogueTalents;
                _priestTalents = character._priestTalents;
                _shamanTalents = character._shamanTalents;
                _mageTalents = character._mageTalents;
                _warlockTalents = character._warlockTalents;
                _druidTalents = character._druidTalents;
                _deathKnightTalents = character._deathKnightTalents;
            }
        }

        //[XmlIgnore]
        //public TalentTree Talents
        //{
        //    get { return _talents; }
        //    set { _talents = value; }
        //}

        // list of 5-tuples itemid.gem1id.gem2id.gem3id.enchantid, itemid is required, others can use * for wildcard
        // for backward compatibility use just itemid instead of itemid.*.*.*.*
        // -id represents enchants
        [XmlIgnore]
        public List<string> AvailableItems
        {
            get { return _availableItems; }
            set
            {
                _availableItems = value;
                OnAvailableItemsChanged();
            }
        }

        public bool IsEquipped(ItemInstance itemToBeChecked)
        {
            CharacterSlot slot = Character.GetCharacterSlotByItemSlot(itemToBeChecked.Slot);
            if (slot == CharacterSlot.Finger1)
                return IsEquipped(itemToBeChecked, CharacterSlot.Finger1) || IsEquipped(itemToBeChecked, CharacterSlot.Finger2);
            else if (itemToBeChecked.Slot == Rawr.ItemSlot.OneHand)
                return IsEquipped(itemToBeChecked, CharacterSlot.MainHand) || IsEquipped(itemToBeChecked, CharacterSlot.OffHand);
            else if (itemToBeChecked.Slot == Rawr.ItemSlot.Trinket)
                return IsEquipped(itemToBeChecked, CharacterSlot.Trinket1) || IsEquipped(itemToBeChecked, CharacterSlot.Trinket2);
            else
                return IsEquipped(itemToBeChecked, slot);
        }
        public bool IsEquipped(ItemInstance itemToBeChecked, CharacterSlot slot)
        {
            return itemToBeChecked == this[slot];
        }

        public bool IsEquipped(Item itemToBeChecked)
        {
            CharacterSlot slot = Character.GetCharacterSlotByItemSlot(itemToBeChecked.Slot);
            if (slot == CharacterSlot.Finger1)
                return IsEquipped(itemToBeChecked, CharacterSlot.Finger1) || IsEquipped(itemToBeChecked, CharacterSlot.Finger2);
            else if (itemToBeChecked.Slot == Rawr.ItemSlot.OneHand)
                return IsEquipped(itemToBeChecked, CharacterSlot.MainHand) || IsEquipped(itemToBeChecked, CharacterSlot.OffHand);
            else if (itemToBeChecked.Slot == Rawr.ItemSlot.Trinket)
                return IsEquipped(itemToBeChecked, CharacterSlot.Trinket1) || IsEquipped(itemToBeChecked, CharacterSlot.Trinket2);
            else
                return IsEquipped(itemToBeChecked, slot);
        }
        
        public bool IsEquipped(Item itemToBeChecked, CharacterSlot slot)
        {
            return (object)this[slot] != null && itemToBeChecked.Id == this[slot].Id;
        }

        public static CharacterSlot GetCharacterSlotByItemSlot(ItemSlot slot)
        {
            
            //note: When converting ItemSlot.Finger and ItemSlot.Trinket, this will ALWAYS
            //place them in Slot 1 of the 2 possibilities. Items listed as OneHand or TwoHand 
            //in their Itemslot profile, will be parsed into the MainHand CharacterSlot.
            
            switch (slot)
            {
               
                case Rawr.ItemSlot.Projectile: return CharacterSlot.Projectile;
                case Rawr.ItemSlot.Head: return CharacterSlot.Head;
                case Rawr.ItemSlot.Neck: return CharacterSlot.Neck;
                case Rawr.ItemSlot.Shoulders: return CharacterSlot.Shoulders;
                case Rawr.ItemSlot.Chest: return CharacterSlot.Chest;
                case Rawr.ItemSlot.Waist: return CharacterSlot.Waist;
                case Rawr.ItemSlot.Legs: return CharacterSlot.Legs;
                case Rawr.ItemSlot.Feet: return CharacterSlot.Feet;
                case Rawr.ItemSlot.Wrist: return CharacterSlot.Wrist;
                case Rawr.ItemSlot.Hands: return CharacterSlot.Hands;
                case Rawr.ItemSlot.Finger: return CharacterSlot.Finger1;
                //case Rawr.ItemSlot.Finger: return CharacterSlot.Finger2;
                case Rawr.ItemSlot.Trinket: return CharacterSlot.Trinket1;
                //case Rawr.ItemSlot.Trinket: return CharacterSlot.Trinket2;
                case Rawr.ItemSlot.Back: return CharacterSlot.Back;
                case Rawr.ItemSlot.OneHand: return CharacterSlot.MainHand;
                case Rawr.ItemSlot.TwoHand: return CharacterSlot.MainHand;
                case Rawr.ItemSlot.MainHand: return CharacterSlot.MainHand;
                case Rawr.ItemSlot.OffHand: return CharacterSlot.OffHand;
                case Rawr.ItemSlot.Ranged: return CharacterSlot.Ranged;
                case Rawr.ItemSlot.ProjectileBag: return CharacterSlot.ProjectileBag;
                case Rawr.ItemSlot.Tabard: return CharacterSlot.Tabard;
                case Rawr.ItemSlot.Shirt: return CharacterSlot.Shirt;
                case Rawr.ItemSlot.Red: return CharacterSlot.Gems;
                case Rawr.ItemSlot.Orange: return CharacterSlot.Gems;
                case Rawr.ItemSlot.Yellow: return CharacterSlot.Gems;
                case Rawr.ItemSlot.Green: return CharacterSlot.Gems;
                case Rawr.ItemSlot.Blue: return CharacterSlot.Gems;
                case Rawr.ItemSlot.Purple: return CharacterSlot.Gems;
                case Rawr.ItemSlot.Prismatic: return CharacterSlot.Gems;
                case Rawr.ItemSlot.Meta: return CharacterSlot.Metas;
                default: return CharacterSlot.None;
            }
        }

        public ItemAvailability GetItemAvailability(Item item)
        {
            return GetItemAvailability(item.Id.ToString(), item.Id.ToString() + ".0.0.0", item.Id.ToString() + ".0.0.0.0");
        }

        public ItemAvailability GetItemAvailability(ItemInstance itemInstance)
        {
            return GetItemAvailability(itemInstance.Id.ToString(), string.Format("{0}.{1}.{2}.{3}", itemInstance.Id, itemInstance.Gem1Id, itemInstance.Gem2Id, itemInstance.Gem3Id), itemInstance.GemmedId);
        }

        private ItemAvailability GetItemAvailability(string id, string gemId, string fullId)
        {
            string anyGem = id + ".*.*.*";
            List<string> list = _availableItems.FindAll(x => x.StartsWith(id, StringComparison.Ordinal));
            if (list.Contains(gemId + ".*"))
            {
                return ItemAvailability.Available;
            }
            else if (list.FindIndex(x => x.StartsWith(gemId, StringComparison.Ordinal)) >= 0)
            {
                return ItemAvailability.AvailableWithEnchantRestrictions;
            }
            if (list.Contains(id))
            {
                return ItemAvailability.RegemmingAllowed;
            }
            else if (list.FindIndex(x => x.StartsWith(anyGem, StringComparison.Ordinal)) >= 0)
            {
                return ItemAvailability.RegemmingAllowedWithEnchantRestrictions;
            }
            else
            {
                return ItemAvailability.NotAvailable;
            }
        }

        public void ToggleItemAvailability(Item item, bool regemmingAllowed)
        {
            string id = item.Id.ToString();
            string anyGem = id + ".*.*.*";

            if (id.StartsWith("-", StringComparison.Ordinal) || regemmingAllowed || item.IsGem)
            {
                // all enabled toggle
                if (_availableItems.Contains(id) || _availableItems.FindIndex(x => x.StartsWith(anyGem, StringComparison.Ordinal)) >= 0)
                {
                    _availableItems.Remove(id);
                    _availableItems.RemoveAll(x => x.StartsWith(anyGem, StringComparison.Ordinal));
                }
                else
                {
                    _availableItems.Add(id);
                }
            }
            OnAvailableItemsChanged();
        }

        public void ToggleItemAvailability(ItemInstance item, bool regemmingAllowed)
        {
            string id = item.Id.ToString();
            string anyGem = id + ".*.*.*";
            string gemId = string.Format("{0}.{1}.{2}.{3}", item.Id, item.Gem1Id, item.Gem2Id, item.Gem3Id);

            if (id.StartsWith("-", StringComparison.Ordinal) || regemmingAllowed)
            {
                // all enabled toggle
                if (_availableItems.Contains(id) || _availableItems.FindIndex(x => x.StartsWith(anyGem, StringComparison.Ordinal)) >= 0)
                {
                    _availableItems.Remove(id);
                    _availableItems.RemoveAll(x => x.StartsWith(anyGem, StringComparison.Ordinal));
                }
                else
                {
                    _availableItems.Add(id);
                }
            }
            else
            {
                // enabled toggle
                if (_availableItems.FindIndex(x => x.StartsWith(gemId, StringComparison.Ordinal)) >= 0)
                {
                    _availableItems.RemoveAll(x => x.StartsWith(gemId, StringComparison.Ordinal));
                }
                else
                {
                    _availableItems.Add(gemId + ".*");
                }
            }
            OnAvailableItemsChanged();
        }

        public void ToggleAvailableItemEnchantRestriction(ItemInstance item, Enchant enchant)
        {
            string id = item.Id.ToString();
            string anyGem = id + ".*.*.*";
            string gemId = string.Format("{0}.{1}.{2}.{3}", item.Id, item.Gem1Id, item.Gem2Id, item.Gem3Id);
            ItemAvailability availability = GetItemAvailability(item);
            switch (availability)
            {
                case ItemAvailability.Available:
                    if (enchant != null)
                    {
                        _availableItems.RemoveAll(x => x.StartsWith(gemId, StringComparison.Ordinal));
                        _availableItems.Add(gemId + "." + enchant.Id.ToString());
                    }
                    else
                    {
                        // any => all
                        _availableItems.RemoveAll(x => x.StartsWith(gemId, StringComparison.Ordinal));
                        foreach (Enchant e in Enchant.FindEnchants(item.Slot, this))
                        {
                            _availableItems.Add(gemId + "." + e.Id.ToString());
                        }
                    }
                    break;
                case ItemAvailability.AvailableWithEnchantRestrictions:
                    if (enchant != null)
                    {
                        if (_availableItems.Contains(gemId + "." + enchant.Id.ToString()))
                        {
                            _availableItems.Remove(gemId + "." + enchant.Id.ToString());
                        }
                        else
                        {
                            _availableItems.Add(gemId + "." + enchant.Id.ToString());
                        }
                    }
                    else
                    {
                        _availableItems.RemoveAll(x => x.StartsWith(gemId, StringComparison.Ordinal));
                        _availableItems.Add(gemId + ".*");
                    }
                    break;
                case ItemAvailability.RegemmingAllowed:
                    if (enchant != null)
                    {
                        _availableItems.RemoveAll(x => x.StartsWith(id, StringComparison.Ordinal));
                        _availableItems.Add(anyGem + "." + enchant.Id.ToString());
                    }
                    else
                    {
                        // any => all
                        _availableItems.RemoveAll(x => x.StartsWith(id, StringComparison.Ordinal));
                        foreach (Enchant e in Enchant.FindEnchants(item.Slot, this))
                        {
                            _availableItems.Add(anyGem + "." + e.Id.ToString());
                        }
                    }
                    break;
                case ItemAvailability.RegemmingAllowedWithEnchantRestrictions:
                    if (enchant != null)
                    {
                        if (_availableItems.Contains(anyGem + "." + enchant.Id.ToString()))
                        {
                            _availableItems.Remove(anyGem + "." + enchant.Id.ToString());
                        }
                        else
                        {
                            _availableItems.Add(anyGem + "." + enchant.Id.ToString());
                        }
                    }
                    else
                    {
                        _availableItems.RemoveAll(x => x.StartsWith(id, StringComparison.Ordinal));
                        _availableItems.Add(id);
                    }
                    break;
                case ItemAvailability.NotAvailable:
                    if (enchant != null)
                    {
                        _availableItems.Add(anyGem + "." + enchant.Id.ToString());
                    }
                    else
                    {
                        _availableItems.Add(id);
                    }
                    break;
            }
            OnAvailableItemsChanged();
        }

        public void SerializeCalculationOptions()
        {
            if (CalculationOptions != null)
            {
                if (_serializedCalculationOptions == null)
                {
                    _serializedCalculationOptions = new SerializableDictionary<string, string>();
                }
                _serializedCalculationOptions[CurrentModel] = CalculationOptions.GetXml();
            }
        }

        public Enchant GetEnchantBySlot(ItemSlot slot)
        {
            switch (slot)
            {
                case Rawr.ItemSlot.Head:
                    return HeadEnchant;
                case Rawr.ItemSlot.Shoulders:
                    return ShouldersEnchant;
                case Rawr.ItemSlot.Back:
                    return BackEnchant;
                case Rawr.ItemSlot.Chest:
                    return ChestEnchant;
                case Rawr.ItemSlot.Wrist:
                    return WristEnchant;
                case Rawr.ItemSlot.Hands:
                    return HandsEnchant;
                case Rawr.ItemSlot.Legs:
                    return LegsEnchant;
                case Rawr.ItemSlot.Feet:
                    return FeetEnchant;
                case Rawr.ItemSlot.Finger:
                    return Finger1Enchant;
                case Rawr.ItemSlot.MainHand:
                case Rawr.ItemSlot.OneHand:
                case Rawr.ItemSlot.TwoHand:
                    return MainHandEnchant;
                case Rawr.ItemSlot.OffHand:
                    return OffHandEnchant;
                case Rawr.ItemSlot.Ranged:
                    return RangedEnchant;
                default:
                    return null;
            }
        }

        //private static ItemSlot[] characterSlot2ItemSlot = new ItemSlot[] { ItemSlot.Projectile, ItemSlot.Head, ItemSlot.Neck, ItemSlot.Shoulders, ItemSlot.Chest, ItemSlot.Waist, ItemSlot.Legs, ItemSlot.Feet, ItemSlot.Wrist, ItemSlot.Hands, ItemSlot.Finger, ItemSlot.Finger, ItemSlot.Trinket, ItemSlot.Trinket, ItemSlot.Back, ItemSlot.MainHand, ItemSlot.OffHand, ItemSlot.Ranged, ItemSlot.ProjectileBag, ItemSlot.Tabard, ItemSlot.Shirt };
        public Enchant GetEnchantBySlot(CharacterSlot slot)
        {
            ItemInstance item = this[slot];
            if ((object)item == null) return null;
            return item.Enchant;
        }

        public bool IsEnchantable(CharacterSlot slot)
        {
            switch (slot)
            {
                case CharacterSlot.Head:
                case CharacterSlot.Shoulders:
                case CharacterSlot.Back:
                case CharacterSlot.Chest:
                case CharacterSlot.Wrist:
                case CharacterSlot.Hands:
                case CharacterSlot.Legs:
                case CharacterSlot.Feet:
                case CharacterSlot.Finger1:
                case CharacterSlot.Finger2:
                case CharacterSlot.MainHand:
                case CharacterSlot.OffHand:
                case CharacterSlot.Ranged:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsEnchantable(ItemSlot slot)
        {
            switch (slot)
            {
                case Rawr.ItemSlot.Head:
                case Rawr.ItemSlot.Shoulders:
                case Rawr.ItemSlot.Back:
                case Rawr.ItemSlot.Chest:
                case Rawr.ItemSlot.Wrist:
                case Rawr.ItemSlot.Hands:
                case Rawr.ItemSlot.Legs:
                case Rawr.ItemSlot.Feet:
                case Rawr.ItemSlot.Finger:
                case Rawr.ItemSlot.TwoHand:
                case Rawr.ItemSlot.MainHand:
                case Rawr.ItemSlot.OneHand:
                case Rawr.ItemSlot.OffHand:
                case Rawr.ItemSlot.Ranged:
                    return true;
                default:
                    return false;
            }
        }

        public void SetEnchantBySlot(ItemSlot slot, Enchant enchant)
        {
            switch (slot)
            {
                case Rawr.ItemSlot.Head:
                    HeadEnchant = enchant;
                    break;
                case Rawr.ItemSlot.Shoulders:
                    ShouldersEnchant = enchant;
                    break;
                case Rawr.ItemSlot.Back:
                    BackEnchant = enchant;
                    break;
                case Rawr.ItemSlot.Chest:
                    ChestEnchant = enchant;
                    break;
                case Rawr.ItemSlot.Wrist:
                    WristEnchant = enchant;
                    break;
                case Rawr.ItemSlot.Hands:
                    HandsEnchant = enchant;
                    break;
                case Rawr.ItemSlot.Legs:
                    LegsEnchant = enchant;
                    break;
                case Rawr.ItemSlot.Feet:
                    FeetEnchant = enchant;
                    break;
                case Rawr.ItemSlot.Finger:
                    Finger1Enchant = enchant;
                    break;
                case Rawr.ItemSlot.MainHand:
                case Rawr.ItemSlot.OneHand:
                case Rawr.ItemSlot.TwoHand:
                    MainHandEnchant = enchant;
                    break;
                case Rawr.ItemSlot.OffHand:
                    OffHandEnchant = enchant;
                    break;
                case Rawr.ItemSlot.Ranged:
                    RangedEnchant = enchant;
                    break;
            }
        }

        public void SetEnchantBySlot(CharacterSlot slot, Enchant enchant)
        {
            int i = (int)slot;
            if (i < 0 || i >= SlotCount) return;
            ItemInstance item = this[slot];
            if ((object)item != null) item.Enchant = enchant;
            OnCalculationsInvalidated();
        }

        private static CharacterSlot[] _characterSlots;
        public static CharacterSlot[] CharacterSlots
        {
            get
            {
                if (_characterSlots == null)
                {
#if SILVERLIGHT
                    _characterSlots = EnumHelper.GetValues<CharacterSlot>();
#else
                    _characterSlots = (CharacterSlot[])Enum.GetValues(typeof(CharacterSlot));
#endif
                }
                return _characterSlots;
            }
        }

        // cache gem counts as this takes the most time of accumulating item stats
        // this becomes invalid when items on character change, invalidate in OnItemsChanged
        private bool gemCountValid;
        private int redGemCount;
        private int yellowGemCount;
        private int blueGemCount;
        private int jewelersGemCount;
        private int gemRequirementsInvalid;
        private int nonjewelerGemRequirementsInvalid;

        public int RedGemCount
        {
            get
            {
                ComputeGemCount();
                return redGemCount;
            }
        }

        public int YellowGemCount
        {
            get
            {
                ComputeGemCount();
                return yellowGemCount;
            }
        }

        public int BlueGemCount
        {
            get
            {
                ComputeGemCount();
                return blueGemCount;
            }
        }

        public int JewelersGemCount
        {
            get
            {
                ComputeGemCount();
                return jewelersGemCount;
            }
        }

        public int GemRequirementsInvalid
        {
            get
            {
                ComputeGemCount();
                return gemRequirementsInvalid;
            }
        }

        public int NonjewelerGemRequirementsInvalid
        {
            get
            {
                ComputeGemCount();
                return nonjewelerGemRequirementsInvalid;
            }
        }


        public bool IsMetaGemActive
        {
            get
            {
                ItemInstance head = _item[1];
                if (head == null) return true;
                Item metagem = head.Gem1;
                if (metagem == null) return true;
                return metagem.MeetsRequirements(this);
            }
        }

        private void ComputeGemCount()
        {
            if (!gemCountValid)
            {
                redGemCount = 0;
                yellowGemCount = 0;
                blueGemCount = 0;
                jewelersGemCount = 0;
                Dictionary<int, bool> uniqueMap = null;
                gemRequirementsInvalid = 0;
                nonjewelerGemRequirementsInvalid = 0;
                for (int slot = 0; slot < OptimizableSlotCount; slot++)
                {
                    if (slot != (int)CharacterSlot.OffHand || CurrentCalculations.IncludeOffHandInCalculations(this))
                    {
                        ItemInstance item = _item[slot];
                        if (item == null) continue;
                        for (int gemIndex = 1; gemIndex <= 3; gemIndex++)
                        {
                            Item gem = item.GetGem(gemIndex);
                            if (gem != null)
                            {
                                if (gem.IsRedGem) redGemCount++;
                                if (gem.IsYellowGem) yellowGemCount++;
                                if (gem.IsBlueGem) blueGemCount++;
                                if (gem.IsJewelersGem) jewelersGemCount++;
                                else if (gem.Unique) // needs else, it seems jewelers gems are marked as unique
                                {
                                    if (uniqueMap == null)
                                    {
                                        uniqueMap = new Dictionary<int, bool>(); // this is a rare case, only create dictionary when really needed
                                    }
                                    if (uniqueMap.ContainsKey(gem.Id))
                                    {
                                        gemRequirementsInvalid++;
                                        nonjewelerGemRequirementsInvalid++;
                                    }
                                    else
                                    {
                                        uniqueMap[gem.Id] = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (jewelersGemCount > 3)
                {
                    gemRequirementsInvalid += jewelersGemCount - 3;
                }

                gemCountValid = true;
            }
        }

        public bool IsUniqueGemEquipped(Item testGem)
        {
            for (int slot = 0; slot < OptimizableSlotCount; slot++)
            {
                if (slot != (int)CharacterSlot.OffHand || CurrentCalculations.IncludeOffHandInCalculations(this))
                {
                    ItemInstance item = _item[slot];
                    if (item == null) continue;
                    for (int gemIndex = 1; gemIndex <= 3; gemIndex++)
                    {
                        Item gem = item.GetGem(gemIndex);
                        if (gem != null && !gem.IsJewelersGem)
                        {
                            if (gem.Unique && gem == testGem) 
                                return true;
                        }
                    }
                }
            }
            return false;
        }
        
        private int GetItemGemIdCount(ItemInstance item, int id)
        {
            int count = 0;
            if ((object)item != null)
            {
                if (item.Gem1 != null && item.Gem1.Id == id) count++;
                if (item.Gem2 != null && item.Gem2.Id == id) count++;
                if (item.Gem3 != null && item.Gem3.Id == id) count++;
            }
            return count;
        }

        public int GetGemIdCount(int id)
        {
            int count = 0;
            for (int slot = 0; slot < SlotCount; slot++)
            {
                count += GetItemGemIdCount(_item[slot], id);
            }
            return count;
        }
        
        public event EventHandler AvailableItemsChanged;
        public void OnAvailableItemsChanged() {
            if (AvailableItemsChanged != null)
                AvailableItemsChanged(this, EventArgs.Empty);
        }
        
        public event EventHandler TalentChangedEvent;
        public void OnTalentChange() {
            if (TalentChangedEvent != null)
                TalentChangedEvent(this, EventArgs.Empty);
        }

        public event EventHandler CalculationsInvalidated;
        public void OnCalculationsInvalidated()
        {
#if DEBUG
            if (CalculationsInvalidated != null) System.Diagnostics.Debug.WriteLine("Starting CalculationsInvalidated");
            DateTime start = DateTime.Now;
#endif
            gemCountValid = false; // invalidate gem counts
            InvalidateItemInstances();
            if (IsLoading) return;
            RecalculateSetBonuses();
            RecalculatePassiveBonuses();

            if (CalculationsInvalidated != null)
            {
                CalculationsInvalidated(this, EventArgs.Empty);
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Finished CalculationsInvalidated: Total " + DateTime.Now.Subtract(start).TotalMilliseconds.ToString() + "ms");
#endif
            }
        }

        public event EventHandler ClassChanged;
        public void OnClassChanged()
        {
            if (ClassChanged != null)
                ClassChanged(this, EventArgs.Empty);
        }

        [XmlIgnore]
        public Dictionary<string, int> SetBonusCount { get; private set; }

        public void RecalculateSetBonuses()
        {
            if (SetBonusCount == null)
            {
                SetBonusCount = new Dictionary<string, int>();
            }
            else
            {
                SetBonusCount.Clear();
            }
            //Compute Set Bonuses
            for (int slot = 0; slot < _item.Length; slot++)
            {
                ItemInstance item = _item[slot];
                if ((object)item != null && !string.IsNullOrEmpty(item.Item.SetName))
                {
                    int count;
                    SetBonusCount.TryGetValue(item.Item.SetName, out count);
                    SetBonusCount[item.Item.SetName] = count + 1;
                }
            }

            // eliminate searching in active buffs: first remove all set bonuses, then add active ones
            ActiveBuffs.RemoveAll(buff => !string.IsNullOrEmpty(buff.SetName));
            foreach (KeyValuePair<string, int> pair in SetBonusCount)
            {
                Buff[] setBonuses = Buff.GetSetBonuses(pair.Key);
                if (setBonuses != null)
                {
                    foreach (Buff buff in setBonuses)
                    {
                        if (pair.Value >= buff.SetThreshold)
                        {
                            ActiveBuffs.Add(buff);
                        }
                    }
                }
            }
        }

        public void RecalculatePassiveBonuses()
        {
            if (Race == CharacterRace.Draenei)
            {
                ActiveBuffsAdd("Heroic Presence");
            }
        }

        [XmlIgnore]
        public ItemInstance this[CharacterSlot slot]
        {
            get
            {
                int i = (int)slot;
                if (i < 0 || i >= SlotCount) return null;
                return _item[i];
            }
            set
            {
                int i = (int)slot;
                if (i < 0 || i >= SlotCount) return;
                // should we track id changes? for now assume assume we don't have to
                _item[i] = value;
                OnCalculationsInvalidated();
                //if (value == null || _item[i] != value.GemmedId) 
                //{
                //    _item[i] = value != null ? value.GemmedId : null;
                //    if (_itemCached[i] != null && _trackEquippedItemChanges) _itemCached[i].IdsChanged -= new EventHandler(_itemCached_IdsChanged);
                //    _itemCached[i] = value;
                //    if (_itemCached[i] != null && _trackEquippedItemChanges) _itemCached[i].IdsChanged += new EventHandler(_itemCached_IdsChanged);
                //    OnCalculationsInvalidated();
                //}
            }
        }

        public string[] GetAllEquippedAndAvailableGearIds()
        {
            Dictionary<string, bool> _ids = new Dictionary<string, bool>();
            if (_back != null) _ids[_back] = true;
            if (_chest != null) _ids[_chest] = true;
            if (_feet != null) _ids[_feet] = true;
            if (_finger1 != null) _ids[_finger1] = true;
            if (_finger2 != null) _ids[_finger2] = true;
            if (_hands != null) _ids[_hands] = true;
            if (_head != null) _ids[_head] = true;
            if (_legs != null) _ids[_legs] = true;
            if (_mainHand != null) _ids[_mainHand] = true;
            if (_neck != null) _ids[_neck] = true;
            if (_offHand != null) _ids[_offHand] = true;
            if (_projectile != null) _ids[_projectile] = true;
            if (_projectileBag != null) _ids[_projectileBag] = true;
            if (_ranged != null) _ids[_ranged] = true;
            if (_shirt != null) _ids[_shirt] = true;
            if (_shoulders != null) _ids[_shoulders] = true;
            if (_tabard != null) _ids[_tabard] = true;
            if (_trinket1 != null) _ids[_trinket1] = true;
            if (_trinket2 != null) _ids[_trinket2] = true;
            if (_waist != null) _ids[_waist] = true;
            if (_wrist != null) _ids[_wrist] = true;
            foreach (string xid in AvailableItems)
            {
                if (!xid.StartsWith("-", StringComparison.Ordinal))
                {
                    int dot = xid.LastIndexOf('.');
                    _ids[(dot >= 0) ? xid.Substring(0, dot).Replace(".*.*.*", "") : xid] = true;
                }
            }
            return new List<string>(_ids.Keys).ToArray();
        }

        public CharacterSlot[] GetEquippedSlots(ItemInstance item)
        {
            List<CharacterSlot> listSlots = new List<CharacterSlot>();
            foreach (CharacterSlot slot in CharacterSlots)
                if (this[slot] == item)
                    listSlots.Add(slot);
            return listSlots.ToArray();
        }

        public static CharacterSlot GetCharacterSlotFromId(int slotId)
        {
            CharacterSlot cslot = CharacterSlot.None;
            switch (slotId)
            {
                case -1:
                    cslot = CharacterSlot.None;
                    break;
                case 1:
                    cslot = CharacterSlot.Head;
                    break;
                case 2:
                    cslot = CharacterSlot.Neck;
                    break;
                case 3:
                    cslot = CharacterSlot.Shoulders;
                    break;
                case 15:
                    cslot = CharacterSlot.Back;
                    break;
                case 5:
                    cslot = CharacterSlot.Chest;
                    break;
                case 4:
                    cslot = CharacterSlot.Shirt;
                    break;
                case 19:
                    cslot = CharacterSlot.Tabard;
                    break;
                case 9:
                    cslot = CharacterSlot.Wrist;
                    break;
                case 10:
                    cslot = CharacterSlot.Hands;
                    break;
                case 6:
                    cslot = CharacterSlot.Waist;
                    break;
                case 7:
                    cslot = CharacterSlot.Legs;
                    break;
                case 8:
                    cslot = CharacterSlot.Feet;
                    break;
                case 11:
                    cslot = CharacterSlot.Finger1;
                    break;
                case 12:
                    cslot = CharacterSlot.Finger2;
                    break;
                case 13:
                    cslot = CharacterSlot.Trinket1;
                    break;
                case 14:
                    cslot = CharacterSlot.Trinket2;
                    break;
                case 16:
                    cslot = CharacterSlot.MainHand;
                    break;
                case 17:
                    cslot = CharacterSlot.OffHand;
                    break;
                case 18:
                    cslot = CharacterSlot.Ranged;
                    break;
                case 0:
                    cslot = CharacterSlot.Projectile;
                    break;
                case 102:
                    cslot = CharacterSlot.ProjectileBag;
                    break;
            }
            return cslot;
        }

        private void Initialize()
        {
            // common initialization used by constructors
            // avoid inline instantiation of fields as not all constructors want/need the overhead
            _item = new ItemInstance[SlotCount];
            _availableItems = new List<string>();
            _calculationOptions = new SerializableDictionary<string, ICalculationOptionBase>();
            _customItemInstances = new List<ItemInstance>();
            CustomGemmingTemplates = new List<GemmingTemplate>();
            GemmingTemplateOverrides = new List<GemmingTemplate>();
            _relevantItemInstances = new Dictionary<CharacterSlot, List<ItemInstance>>();
            _relevantItems = new Dictionary<CharacterSlot, List<Item>>();
        }

        public Character() 
        {
            Initialize();
            _activeBuffs = new List<Buff>();
        }

        public Character(string name, string realm, CharacterRegion region, CharacterRace race, BossOptions boss,
            string head, string neck, string shoulders, string back, string chest, string shirt, string tabard,
                string wrist, string hands, string waist, string legs, string feet, string finger1, string finger2, 
            string trinket1, string trinket2, string mainHand, string offHand, string ranged, string projectile, 
            string projectileBag/*, string extraWristSocket, string extraHandsSocket, string extraWaistSocket,
            int enchantHead, int enchantShoulders, int enchantBack, int enchantChest, int enchantWrist, 
            int enchantHands, int enchantLegs, int enchantFeet, int enchantFinger1, int enchantFinger2, 
            int enchantMainHand, int enchantOffHand, int enchantRanged*/)
        {
            Initialize();
            IsLoading = true;
            _name = name;
            _realm = realm;
            _region = region;
            _race = race;
            _head = head;
            _neck = neck;
            _shoulders = shoulders;
            _back = back;
            _chest = chest;
            _shirt = shirt;
            _tabard = tabard;
            _wrist = wrist;
            _hands = hands;
            _waist = waist;
            _legs = legs;
            _feet = feet;
            _finger1 = finger1;
            _finger2 = finger2;
            _trinket1 = trinket1;
            _trinket2 = trinket2;
            _mainHand = mainHand;
            _offHand = offHand;
            _ranged = ranged;
            _projectile = projectile;
            _projectileBag = projectileBag;

            EnforceGemRequirements = true;
            WaistBlacksmithingSocketEnabled = true;
            _activeBuffs = new List<Buff>();
            SetFaction();
            IsLoading = false;
            RecalculateSetBonuses();

            BossOptions = boss.Clone();
        }

        public Character(string name, string realm, CharacterRegion region, CharacterRace race, BossOptions boss,
            ItemInstance head, ItemInstance neck, ItemInstance shoulders, ItemInstance back, ItemInstance chest, ItemInstance shirt, ItemInstance tabard,
                ItemInstance wrist, ItemInstance hands, ItemInstance waist, ItemInstance legs, ItemInstance feet, ItemInstance finger1, ItemInstance finger2,
            ItemInstance trinket1, ItemInstance trinket2, ItemInstance mainHand, ItemInstance offHand, ItemInstance ranged, ItemInstance projectile,
            ItemInstance projectileBag/*, Item extraWristSocket, Item extraHandsSocket, Item extraWaistSocket,
            Enchant enchantHead, Enchant enchantShoulders, Enchant enchantBack, Enchant enchantChest, 
            Enchant enchantWrist, Enchant enchantHands, Enchant enchantLegs, Enchant enchantFeet, 
            Enchant enchantFinger1, Enchant enchantFinger2, Enchant enchantMainHand, Enchant enchantOffHand,
            Enchant enchantRanged, bool trackEquippedItemChanges*/
                                                                  )
        {
            Initialize();
            //_trackEquippedItemChanges = trackEquippedItemChanges;
            IsLoading = true;
            _name = name;
            _realm = realm;
            _region = region;
            _race = race;
            Head = head;
            Neck = neck;
            Shoulders = shoulders;
            Back = back;
            Chest = chest;
            Shirt = shirt;
            Tabard = tabard;
            Wrist = wrist;
            Hands = hands;
            Waist = waist;
            Legs = legs;
            Feet = feet;
            Finger1 = finger1;
            Finger2 = finger2;
            Trinket1 = trinket1;
            Trinket2 = trinket2;
            MainHand = mainHand;
            OffHand = offHand;
            Ranged = ranged;
            Projectile = projectile;
            ProjectileBag = projectileBag;
            _activeBuffs = new List<Buff>();
            SetFaction();
            IsLoading = false;
            RecalculateSetBonuses();
            BossOptions = boss.Clone();
        }

        // the following are special contructors used by optimizer, they assume the cached items/enchant are always used, and the underlying gemmedid/enchantid are never used
        public Character(string name, string realm, CharacterRegion region, CharacterRace race, BossOptions boss,
            ItemInstance head, ItemInstance neck, ItemInstance shoulders, ItemInstance back, ItemInstance chest, ItemInstance shirt, ItemInstance tabard,
                ItemInstance wrist, ItemInstance hands, ItemInstance waist, ItemInstance legs, ItemInstance feet, ItemInstance finger1, ItemInstance finger2, 
            ItemInstance trinket1, ItemInstance trinket2, ItemInstance mainHand, ItemInstance offHand, ItemInstance ranged, ItemInstance projectile,
            ItemInstance projectileBag, List<Buff> activeBuffs, string model)
        {
            Initialize();
            IsLoading = true;
            _name = name;
            _realm = realm;
            _region = region;
            _race = race;
            _item[(int)CharacterSlot.Head] = head;
            _item[(int)CharacterSlot.Neck] = neck;
            _item[(int)CharacterSlot.Shoulders] = shoulders;
            _item[(int)CharacterSlot.Back] = back;
            _item[(int)CharacterSlot.Chest] = chest;
            _item[(int)CharacterSlot.Shirt] = shirt;
            _item[(int)CharacterSlot.Tabard] = tabard;
            _item[(int)CharacterSlot.Wrist] = wrist;
            _item[(int)CharacterSlot.Hands] = hands;
            _item[(int)CharacterSlot.Waist] = waist;
            _item[(int)CharacterSlot.Legs] = legs;
            _item[(int)CharacterSlot.Feet] = feet;
            _item[(int)CharacterSlot.Finger1] = finger1;
            _item[(int)CharacterSlot.Finger2] = finger2;
            _item[(int)CharacterSlot.Trinket1] = trinket1;
            _item[(int)CharacterSlot.Trinket2] = trinket2;
            _item[(int)CharacterSlot.MainHand] = mainHand;
            _item[(int)CharacterSlot.OffHand] = offHand;
            _item[(int)CharacterSlot.Ranged] = ranged;
            _item[(int)CharacterSlot.Projectile] = projectile;
            _item[(int)CharacterSlot.ProjectileBag] = projectileBag;
            IsLoading = false;
            ActiveBuffs = new List<Buff>(activeBuffs);
            SetFaction();
            CurrentModel = model;
            RecalculateSetBonuses();

            BossOptions = boss.Clone();
        }

        /// <summary>
        /// This overload is used from optimizer and is optimized for performance, do not modify
        /// </summary>
        public Character(Character baseCharacter, object[] items, int count)
        {
            IsLoading = true;
            _name = baseCharacter._name;
            _realm = baseCharacter._realm;
            _region = baseCharacter._region;
            _race = baseCharacter._race;
            _currentModel = baseCharacter._currentModel;
            _calculationOptions = baseCharacter._calculationOptions;
            _primaryProfession = baseCharacter._primaryProfession;
            _secondaryProfession = baseCharacter._secondaryProfession;
            _class = baseCharacter._class;
            AssignAllTalentsFromCharacter(baseCharacter, false);
            CalculationToOptimize = baseCharacter.CalculationToOptimize;
            OptimizationRequirements = baseCharacter.OptimizationRequirements;
            _enforceMetagemRequirements = baseCharacter._enforceMetagemRequirements;
            _bossOptions = baseCharacter._bossOptions;
            _faction = baseCharacter._faction;

            _item = new ItemInstance[SlotCount];
            Array.Copy(items, _item, count);

            IsLoading = false;
            ActiveBuffs = new List<Buff>(baseCharacter.ActiveBuffs);
            RecalculateSetBonuses();
        }

        /// <summary>
        /// This is a variant of the above constructor used when recycling Character, assuming
        /// it was first created with the above constructor and same baseCharacter.
        /// </summary>
        internal void InitializeCharacter(object[] items, int count)
        {
            gemCountValid = false;
            Array.Copy(items, _item, count);
            RecalculateSetBonuses();
        }

        public Character(string name, string realm, CharacterRegion region, CharacterRace race, BossOptions boss,
            ItemInstance[] items, List<Buff> activeBuffs, string model)
        {
            Initialize();
            IsLoading = true;
            _name = name;
            _realm = realm;
            _region = region;
            _race = race;
            Array.Copy(items, _item, items.Length);

            IsLoading = false;
            ActiveBuffs = new List<Buff>(activeBuffs);
            SetFaction();
            CurrentModel = model;
            RecalculateSetBonuses();

            BossOptions = boss.Clone();
        }

        public Character Clone()
        {
            ItemInstance[] clonedItemInstances = new ItemInstance[SlotCount];
            for (int i = 0; i < clonedItemInstances.Length; i++)
            {
                ItemInstance itemInstance = _item[i];
                if (itemInstance != null) clonedItemInstances[i] = itemInstance.Clone();
            }
            Character clone = new Character(this.Name, this.Realm, this.Region, this.Race, this.BossOptions,
                clonedItemInstances, ActiveBuffs, CurrentModel);
            clone.CalculationOptions = this.CalculationOptions;
            clone.Class = this.Class;
            clone.AssignAllTalentsFromCharacter(this, true);
            clone.EnforceGemRequirements = this.EnforceGemRequirements;
            clone.PrimaryProfession = this.PrimaryProfession;
            clone.SecondaryProfession = this.SecondaryProfession;
            clone.WaistBlacksmithingSocketEnabled = this.WaistBlacksmithingSocketEnabled;
            clone.WristBlacksmithingSocketEnabled = this.WristBlacksmithingSocketEnabled;
            clone.HandsBlacksmithingSocketEnabled = this.HandsBlacksmithingSocketEnabled;
            clone.OptimizationRequirements = this.OptimizationRequirements;
            clone.CalculationToOptimize = this.CalculationToOptimize;
            clone.BossOptions = this.BossOptions;
            return clone;
        }
    
#if RAWR3
        public void Save(Stream writer)
        {
            SerializeCalculationOptions();
            SaveGemmingTemplateOverrides();
            SaveItemFilterEnabledOverride();
            _activeBuffsXml = new List<string>(_activeBuffs.ConvertAll(buff => buff.Name));

            XmlSerializer serializer = new XmlSerializer(typeof(Character));
            serializer.Serialize(writer, this);
            writer.Close();
        }
#else
        public void Save(string path)
        {
            SerializeCalculationOptions();
            SaveGemmingTemplateOverrides();
            SaveItemFilterEnabledOverride();
            _activeBuffsXml = new List<string>(_activeBuffs.ConvertAll(buff => buff.Name));
            if(ArmoryPets!=null)
                ArmoryPetsXml = new List<string>(ArmoryPets.ConvertAll(ArmoryPet => ArmoryPet.ToString()));

            using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Character));
                serializer.Serialize(writer, this);
                writer.Close();
            }
        }
#endif

#if RAWR3
        public void SaveBuffs(Stream writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Buff>));
            serializer.Serialize(writer, _activeBuffs);
            writer.Close();
        }
#else
        public void SaveBuffs(string path)
        {
            List<string> buffs = new List<string>(_activeBuffs.ConvertAll(buff => buff.Name));
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<string>));
                serializer.Serialize(writer, buffs);
                writer.Close();
            }
        }
#endif

#if !RAWR3
        public static Character Load(string path)
        {
            Character character;
            if (File.Exists(path))
            {
                try
                {
                    character = LoadFromXml(System.IO.File.ReadAllText(path));
                }
                catch (Exception)
                {
                    Log.Show("There was an error attempting to open this character.");
                    character = new Character();
                }
            }
            else
                character = new Character();

            return character;
        }
#endif
        public static Character LoadFromXml(string xml)
        {
            Character character;
            if (!string.IsNullOrEmpty(xml))
            {
                try
                {
                    xml = xml.Replace("<Region>en", "<Region>US").Replace("<Weapon>", "<MainHand>").Replace("</Weapon>", "</MainHand>").Replace("<Idol>", "<Ranged>").Replace("</Idol>", "</Ranged>").Replace("<WeaponEnchant>", "<MainHandEnchant>").Replace("</WeaponEnchant>", "</MainHandEnchant>").Replace("HolyPriest", "HealPriest");

                    if (xml.IndexOf("<CalculationOptions>") != xml.LastIndexOf("<CalculationOptions>"))
                    {
                        xml = xml.Substring(0, xml.IndexOf("<CalculationOptions>")) +
                            xml.Substring(xml.LastIndexOf("</CalculationOptions>") + "</CalculationOptions>".Length);
                    }

                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Character));
                    System.IO.StringReader reader = new System.IO.StringReader(xml);
                    character = (Character)serializer.Deserialize(reader);
                    character._activeBuffs = new List<Buff>(character._activeBuffsXml.ConvertAll(buff => Buff.GetBuffByName(buff)));
                    character._activeBuffs.RemoveAll(buff => buff == null);
                    character.ArmoryPets = new List<ArmoryPet>(character.ArmoryPetsXml.ConvertAll(armoryPet => ArmoryPet.GetPetByString(armoryPet)));
                    character.RecalculateSetBonuses(); // now you can call it
                    foreach (ItemInstance item in character.CustomItemInstances)
                    {
                        item.ForceDisplay = true;
                    }
                    reader.Close();
                }
                catch (Exception)
                {
#if !RAWR3
                    Log.Show("There was an error attempting to open this character. Most likely, it was saved with a previous version of Rawr, and isn't upgradable to the new format. Sorry. Please load your character from the armory to begin.");
#endif
                    character = new Character();
                }
            }
            else
                character = new Character();

            return character;
        }

        public void LoadBuffsFromXml(string path)
        {
            string xml = null;
#if !RAWR3
            if (File.Exists(path))
            {
                try
                {
                    xml = System.IO.File.ReadAllText(path);
                }
                catch (Exception)
                {
                    Log.Show("There was an error attempting to open this buff file.");
                }
            }
#endif
            if (!string.IsNullOrEmpty(xml))
            {
                try
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<string>));
                    System.IO.StringReader reader = new System.IO.StringReader(xml);
                    List<string> buffs = (List<string>)serializer.Deserialize(reader);
                    _activeBuffs = new List<Buff>(buffs.ConvertAll(buff => Buff.GetBuffByName(buff))); ;
                    _activeBuffs.RemoveAll(buff => buff == null);
                    OnCalculationsInvalidated();
                    reader.Close();
                }
                catch (Exception)
                {
#if !RAWR3
                    Log.Show("There was an error attempting to open this buffs file. Most likely, it was saved with a previous beta of Rawr, and isn't upgradable to the new format. Sorry. No buff changes have been applied.");
#endif
                }
            }
        }

        //public string ToCompressedString()
        //{
        //    //TODO: Just messing around with potential ways to serialize a character down to a string short enough to fit in a URL (<2000 characters)

        //    //List<object> objectsToSerialize = new List<object>();
        //    //objectsToSerialize.Add(Name);
        //    //objectsToSerialize.Add(Race);
        //    //objectsToSerialize.Add(Region);
        //    //objectsToSerialize.Add(Realm);
        //    //objectsToSerialize.Add(Class);
        //    //objectsToSerialize.Add(string.Join("|", _item.ConvertAll(itemInstance => itemInstance == null ? string.Empty : itemInstance.GemmedId).ToArray()));
        //    //objectsToSerialize.Add(string.Join("|", _activeBuffs.ConvertAll(buff=>buff.Name).ToArray()));
        //    //objectsToSerialize.Add(CurrentModel);
        //    //objectsToSerialize.Add(CurrentTalents.Data);
        //    //objectsToSerialize.Add(CurrentTalents.GlyphData);
        //    //objectsToSerialize.Add(EnforceGemRequirements);
        //    //objectsToSerialize.Add(WristBlacksmithingSocketEnabled);
        //    //objectsToSerialize.Add(WaistBlacksmithingSocketEnabled);
        //    //objectsToSerialize.Add(HandsBlacksmithingSocketEnabled);
        //    //objectsToSerialize.Add(CalculationOptions.GetXml());
        //    //objectsToSerialize.Add(string.Join("|", AvailableItems.ToArray()));

        //    //MemoryStream stream = new MemoryStream();
        //    //StreamWriter writer = new StreamWriter(stream);
        //    //writer.Write(objectsToSerialize[6].ToString());
        //    //string base64 = System.Convert.ToBase64String(stream.ToArray());


        //    //_serializedCalculationOptions.Clear();
        //    //SerializeCalculationOptions();
        //    //_activeBuffsXml = new List<string>(_activeBuffs.ConvertAll(buff => buff.Name));
        //    //if (this.Class != CharacterClass.DeathKnight) this.DeathKnightTalents = null;
        //    //if (this.Class != CharacterClass.Druid) this.DruidTalents = null;
        //    //if (this.Class != CharacterClass.Hunter) this.HunterTalents = null;
        //    //if (this.Class != CharacterClass.Mage) this.MageTalents = null;
        //    //if (this.Class != CharacterClass.Paladin) this.PaladinTalents = null;
        //    //if (this.Class != CharacterClass.Priest) this.PriestTalents = null;
        //    //if (this.Class != CharacterClass.Rogue) this.RogueTalents = null;
        //    //if (this.Class != CharacterClass.Shaman) this.ShamanTalents = null;
        //    //if (this.Class != CharacterClass.Warlock) this.WarlockTalents = null;
        //    //if (this.Class != CharacterClass.Warrior) this.WarriorTalents = null;

            
        //    ////MemoryStream stream = new MemoryStream();
        //    ////XmlSerializer serializer = new XmlSerializer(typeof(Character));
        //    ////serializer.Serialize(stream, this);
        //    ////StreamReader reader = new StreamReader(stream);
        //    ////string serializedCharacter = reader.ReadToEnd();
        //    ////reader.Close();
        //    ////stream.Close();
        //    ////stream.Dispose();

            

        //    //return "";
        //}

        public static Character FromCompressedString(string characterString)
        {
            return null;
        }
    }

    public interface ICalculationOptionBase
    {
        string GetXml();
    }

    public class ArmoryPet
    {
        public ArmoryPet(string family, string name, string speckey, string spec)
        {
            Family = PetFamilyIdToPetFamilyName(family);
            Name = name;
            Spec = spec;
            SpecKey = speckey;
        }
        public string Family;
        public string Name;
        private string _SpecKey = "";
        public string SpecKey {
            get {
                if (_SpecKey == "") { _SpecKey = PetFamilyToPetFamilyTree(Family); }
                return _SpecKey;
            }
            set {
                if (value == "") { _SpecKey = PetFamilyToPetFamilyTree(Family); }
                else { _SpecKey = value; }
            }
        }
        public string Spec;

        public override string ToString()
        {
            return Family + ": [" + Name + "] Spec: " + SpecKey + " '" + Spec + "'";
        }
        public static ArmoryPet GetPetByString(string input) {
            string family = "";
            string name = "";
            string specKey = "";
            string spec = "";
            try {
                int start = 0, end = input.IndexOf(':');
                family = input.Substring(start, end);
                start = input.IndexOf('[') + 1; end = input.IndexOf(']', start) - start;
                name = input.Substring(start, end);
                start = input.IndexOf("Spec:") + "Spec: ".Length; end = input.IndexOf(" '", start) - start;
                specKey = input.Substring(start, end);
                start = input.IndexOf("Spec:") + ("Spec: " + specKey + " '").Length; end = input.IndexOf("\'", start) - start;
                spec = input.Substring(start, end);
            } catch (Exception ex) {
                Rawr.Base.ErrorBox eb = new Rawr.Base.ErrorBox(
                    "Error converting character saved Armory Pets to class form",
                    ex.Message,
                    "GetPetByString(string input)",
                    "No Additional Info",
                    ex.StackTrace
                    );
            }

            return new ArmoryPet(family, name, specKey, spec);
        }

        public static string PetFamilyToPetFamilyTree(string family)
        {
            switch (family)
            {
                case "Bat": case "24":
                case "BirdOfPrey": case "26":
                case "Chimaera": case "38":
                case "Dragonhawk": case "30":
                case "NetherRay": case "34":
                case "Ravager": case "31":
                case "Serpent": case "35":
                case "Silithid": case "41":
                case "Spider": case "3":
                case "SporeBat": case "33":
                case "WindSerpent": case "27":
                    return "Cunning";

                case "Bear": case "4":
                case "Boar": case "5":
                case "Crab": case "8":
                case "Crocolisk": case "6":
                case "Gorilla": case "9":
                case "Rhino": case "43":
                case "Scorpid": case "20":
                case "Turtle": case "21":
                case "WarpStalker": case "32":
                case "Worm": case "42":
                    return "Tenacity";

                case "CarrionBird": case "7":
                case "Cat": case "2":
                case "CoreHound": case "45":
                case "Devilsaur": case "39":
                case "Hyena": case "25":
                case "Moth": case "37":
                case "Raptor": case "11":
                case "SpiritBeast": case "46":
                case "Tallstrider": case "12":
                case "Wasp": case "44":
                case "Wolf": case "1":
                    return "Ferocity";
            }

            // hmmm!
            return "None";
        }
        public static string PetFamilyIdToPetFamilyName(string familyid)
        {
            switch (familyid)
            {
                case "Bat": case "24": return "Bat";
                case "BirdOfPrey": case "26": return "BirdOfPrey";
                case "Chimaera": case "38": return "Chimaera";
                case "Dragonhawk": case "30": return "Dragonhawk";
                case "NetherRay": case "34": return "NetherRay";
                case "Ravager": case "31": return "Ravager";
                case "Serpent": case "35": return "Serpent";
                case "Silithid": case "41": return "Silithid";
                case "Spider": case "3": return "Spider";
                case "SporeBat": case "33": return "SporeBat";
                case "WindSerpent": case "27": return "WindSerpent";

                case "Bear": case "4": return "Bear";
                case "Boar": case "5": return "Boar";
                case "Crab": case "8": return "Crab";
                case "Crocolisk": case "6": return "Crocolisk";
                case "Gorilla": case "9": return "Gorilla";
                case "Rhino": case "43": return "Rhino";
                case "Scorpid": case "20": return "Scorpid";
                case "Turtle": case "21": return "Turtle";
                case "WarpStalker": case "32": return "WarpStalker";
                case "Worm": case "42": return "Worm";

                case "CarrionBird": case "7": return "CarrionBird";
                case "Cat": case "2": return "Cat";
                case "CoreHound": case "45": return "CoreHound";
                case "Devilsaur": case "39": return "Devilsaur";
                case "Hyena": case "25": return "Hyena";
                case "Moth": case "37": return "Moth";
                case "Raptor": case "11": return "Raptor";
                case "SpiritBeast": case "46": return "SpiritBeast";
                case "Tallstrider": case "12": return "Tallstrider";
                case "Wasp": case "44": return "Wasp";
                case "Wolf": case "1": return "Wolf";
            }

            return familyid; // it's already a name
        }
        public static string PetFamilyNameToPetFamilyId(string familyname)
        {
            switch (familyname)
            {
                case "Bat": case "24": return "24";
                case "BirdOfPrey": case "26": return "26";
                case "Chimaera": case "38": return "38";
                case "Dragonhawk": case "30": return "30";
                case "NetherRay": case "34": return "34";
                case "Ravager": case "31": return "31";
                case "Serpent": case "35": return "35";
                case "Silithid": case "41": return "41";
                case "Spider": case "3": return "3";
                case "SporeBat": case "33": return "33";
                case "WindSerpent": case "27": return "27";

                case "Bear": case "4": return "4";
                case "Boar": case "5": return "5";
                case "Crab": case "8": return "8";
                case "Crocolisk": case "6": return "6";
                case "Gorilla": case "9": return "9";
                case "Rhino": case "43": return "43";
                case "Scorpid": case "20": return "20";
                case "Turtle": case "21": return "21";
                case "WarpStalker": case "32": return "32";
                case "Worm": case "42": return "42";

                case "CarrionBird": case "7": return "7";
                case "Cat": case "2": return "2";
                case "CoreHound": case "45": return "45";
                case "Devilsaur": case "39": return "39";
                case "Hyena": case "25": return "25";
                case "Moth": case "37": return "37";
                case "Raptor": case "11": return "11";
                case "SpiritBeast": case "46": return "46";
                case "Tallstrider": case "12": return "12";
                case "Wasp": case "44": return "44";
                case "Wolf": case "1": return "1";
            }

            return familyname; // it's already an id
        }
    }
}
