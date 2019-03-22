using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Reflection;
#if SILVERLIGHT
using System.Windows.Browser;
#else
using System.Web;
#endif
using System.Text;

namespace Rawr
{
    public class ElitistArmoryService
    {
        private const string URL_CHAR_REQ = "http://www.elitistarmory.com/rawr/req/{0}/{1}/{2}/{3}";
        private const string URL_CHAR_QUEUE = "http://www.elitistarmory.com/rawr/queue/{0}/{1}/{2}";
        private const string URL_CHAR_DATA = "http://www.elitistarmory.com/rawr/char/{0}/{1}/{2}";
        private const string URL_ITEM = "http://www.elitistarmory.com/rawr/item/{0}";
        private WebClient _webClient;

        public ElitistArmoryService()
        {
            _webClient = new WebClient();
            _webClient.Encoding = Encoding.UTF8; // elitistarmory uses UTF8 encoding
            _webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(_webClient_DownloadStringCompleted);
            _queueTimer.Tick += new EventHandler(CheckQueueAsync);
        }

        public event EventHandler<EventArgs<string>> ProgressChanged;
        private string _progress = "Requesting Character...";
        public string Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                if (ProgressChanged != null)
                {
                    ProgressChanged(this, new EventArgs<string>(value));
                }
            }
        }

        private bool _canceled = false;
        public void CancelAsync()
        {
            _webClient.CancelAsync();
            _canceled = true;
        }

        private void _webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                XDocument xdoc;
                using (StringReader sr = new StringReader(e.Result))
                {
                    xdoc = XDocument.Load(sr);
                }

                if (xdoc.Root.Name == "queue")
                {
                    Progress = "Queued (Position: " + xdoc.Root.Attribute("position").Value + ")";
                    _queueTimer.Start();
                }
                else if (xdoc.Root.Name == "characterInfo")
                {
                    Progress = "Parsing Character Data...";
                    BackgroundWorker bwParseCharacter = new BackgroundWorker();
                    bwParseCharacter.WorkerReportsProgress = true;
                    bwParseCharacter.DoWork += new DoWorkEventHandler(bwParseCharacter_DoWork);
                    bwParseCharacter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwParseCharacter_RunWorkerCompleted);
                    bwParseCharacter.ProgressChanged += new ProgressChangedEventHandler(bwParse_ProgressChanged);
                    bwParseCharacter.RunWorkerAsync(xdoc);
                }
                else if (xdoc.Root.Name == "itemData")
                {
                    Progress = "Parsing Item Data...";
                    BackgroundWorker bwParseItem = new BackgroundWorker();
                    bwParseItem.WorkerReportsProgress = true;
                    bwParseItem.DoWork += new DoWorkEventHandler(bwParseItem_DoWork);
                    bwParseItem.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwParseItem_RunWorkerCompleted);
                    bwParseItem.ProgressChanged += new ProgressChangedEventHandler(bwParse_ProgressChanged);
                    bwParseItem.RunWorkerAsync(xdoc);
                }
            }
        }

        private void bwParse_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.UserState.ToString();
        }

        private DispatcherTimer _queueTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(5) };
        private CharacterRegion _lastRegion;
        private string _lastRealm;
        private string _lastName;
        private int _lastItemId;
        private bool _lastRequestWasItem = false;
        private void CheckQueueAsync(object sender, EventArgs e)
        {
            _queueTimer.Stop();
            if (!_canceled)
            {
                if (_lastRequestWasItem)
                {
                    _webClient.DownloadStringAsync(new Uri(string.Format(URL_ITEM, _lastItemId)));
                    this.Progress = "Downloading Item Data...";
                }
                else
                {
                    _webClient.DownloadStringAsync(new Uri(string.Format(URL_CHAR_QUEUE,
                        _lastRegion.ToString().ToLower(), _lastRealm.ToLower(), _lastName.ToLower())));
                    this.Progress = "Downloading Character Data...";
                }
            }
        }

        #region Characters
        public event EventHandler<EventArgs<Character>> GetCharacterCompleted;
        public void GetCharacterAsync(CharacterRegion region, string realm, string name, bool forceRefresh)
        {
            _lastRegion = region;
            _lastRealm = realm;
            _lastName = name;
            _canceled = false;
            _lastRequestWasItem = false;
            _webClient.DownloadStringAsync(new Uri(string.Format(URL_CHAR_REQ,
                region.ToString().ToLower(), UrlEncode(realm), UrlEncode(name), forceRefresh ? "1" : "0")));
            this.Progress = "Downloading Character Data...";
        }

        private string UrlEncode(string text)
        {
            // elitistarmory expect space to be encoded as %20
            return HttpUtility.UrlEncode(text).Replace("+", "%20");
        }

        void bwParseCharacter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                KeyValuePair<Character, Dictionary<CharacterSlot, ItemInstance>> kvp = (KeyValuePair<Character, Dictionary<CharacterSlot, ItemInstance>>)e.Result;
                Character character = kvp.Key;
                Dictionary<CharacterSlot, ItemInstance> items = kvp.Value;

                //Handle items here, due to threading issues (this is on the main UI thread)
                foreach (KeyValuePair<CharacterSlot, ItemInstance> item in items)
                {
                    character[item.Key] = item.Value;

                    if (item.Value.Id > 0 && !character.AvailableItems.Contains(item.Value.Id.ToString()))
                        character.AvailableItems.Add(item.Value.Id.ToString());
                    if (item.Value.Enchant != null && item.Value.EnchantId > 0)
                    {
                        string enchantString = (-1 * (item.Value.Enchant.Id + (10000 * (int)item.Value.Enchant.Slot))).ToString();
                        if (!character.AvailableItems.Contains(enchantString))
                            character.AvailableItems.Add(enchantString);
                    }
                }

                Progress = "Complete!";
                if (this.GetCharacterCompleted != null)
                    this.GetCharacterCompleted(this, new EventArgs<Character>(character));
            }
        }

        private void bwParseCharacter_DoWork(object sender, DoWorkEventArgs e)
        {
            XDocument xdoc = e.Argument as XDocument;
            Character character = new Character();
            try
            {
                XElement xchar = xdoc.Root.Element("character");
                character.Region = (CharacterRegion)Enum.Parse(typeof(CharacterRegion), xchar.Attribute("region").Value, true);
                character.Realm = xchar.Attribute("realm").Value;
                character.Name = xchar.Attribute("name").Value;
                character.ClassIndex = int.Parse(xchar.Attribute("class_id").Value);
                character.RaceIndex = int.Parse(xchar.Attribute("race_id").Value);
                character.EnforceGemRequirements = true;
                character.WaistBlacksmithingSocketEnabled = true;
                if (character.Race == CharacterRace.Draenei) character.ActiveBuffs.Add(Buff.GetBuffByName("Heroic Presence"));

                Dictionary<CharacterSlot, ItemInstance> items = new Dictionary<CharacterSlot, ItemInstance>();
                foreach (XElement xequipment in xchar.Element("equipInfo").Elements("equipment"))
                {
                    ItemInstance item = new ItemInstance(string.Format("{0}.{1}.{2}.{3}.{4}",
                        (xequipment.Attribute("item_id") ?? new XAttribute("item_id", "0")).Value,
                        (xequipment.Attribute("gem1_id") ?? new XAttribute("gem1_id", "0")).Value,
                        (xequipment.Attribute("gem2_id") ?? new XAttribute("gem2_id", "0")).Value,
                        (xequipment.Attribute("gem3_id") ?? new XAttribute("gem3_id", "0")).Value,
                        (xequipment.Attribute("enchant_id") ?? new XAttribute("enchant_id", "0")).Value));
                    items[Character.GetCharacterSlotFromId(int.Parse(xequipment.Attribute("slot").Value) + 1)] = item;
                }

                foreach (XElement xtalent in xchar.Element("talentInfo").Elements("talent"))
                {
                    if (xtalent.Attribute("active").Value == "1")
                    {
                        string talentCode = xtalent.Attribute("value").Value;
                        switch (character.Class)
                        {
                            case CharacterClass.Warrior:
                                character.WarriorTalents = new WarriorTalents(talentCode);
                                //character.WarriorTalentsCata = new WarriorTalentsCata(talentCode);
                                if (character.WarriorTalents.Devastate > 0) character.CurrentModel = "ProtWarr";
                                else character.CurrentModel = "DPSWarr";
                                break;
                            case CharacterClass.Paladin:
                                character.PaladinTalents = new PaladinTalents(talentCode);
                                if (character.PaladinTalents.HolyShield > 0) character.CurrentModel = "ProtPaladin";
                                else if (character.PaladinTalents.CrusaderStrike > 0) character.CurrentModel = "Retribution";
                                else character.CurrentModel = "Healadin";
                                break;
                            case CharacterClass.Hunter:
                                character.HunterTalents = new HunterTalents(talentCode);
                                character.CurrentModel = "Hunter";
                                break;
                            case CharacterClass.Rogue:
                                character.RogueTalents = new RogueTalents(talentCode);
                                character.CurrentModel = "Rogue";
                                break;
                            case CharacterClass.Priest:
                                character.PriestTalents = new PriestTalents(talentCode);
                                if (character.PriestTalents.Shadowform > 0) character.CurrentModel = "ShadowPriest";
                                else character.CurrentModel = "HealPriest";
                                break;
                            case CharacterClass.Shaman:
                                character.ShamanTalents = new ShamanTalents(talentCode);
                                if (character.ShamanTalents.ElementalMastery > 0) character.CurrentModel = "Elemental";
                                else if (character.ShamanTalents.Stormstrike > 0) character.CurrentModel = "Enhance";
                                else character.CurrentModel = "RestoSham";
                                break;
                            case CharacterClass.Mage:
                                character.MageTalents = new MageTalents(talentCode);
                                character.CurrentModel = "Mage";
                                break;
                            case CharacterClass.Warlock:
                                character.WarlockTalents = new WarlockTalents(talentCode);
                                character.CurrentModel = "Warlock";
                                break;
                            case CharacterClass.Druid:
                                character.DruidTalents = new DruidTalents(talentCode);
                                if (character.DruidTalents.ProtectorOfThePack > 0) character.CurrentModel = "Bear";
                                else if (character.DruidTalents.LeaderOfThePack > 0) character.CurrentModel = "Cat";
                                else if (character.DruidTalents.MoonkinForm > 0) character.CurrentModel = "Moonkin";
                                else character.CurrentModel = "Tree";
                                break;
                            case CharacterClass.DeathKnight:
                                character.DeathKnightTalents = new DeathKnightTalents(talentCode);
                                if (character.DeathKnightTalents.Anticipation > 0) character.CurrentModel = "TankDK";
                                else character.CurrentModel = "DPSDK";
                                break;
                            default:
                                break;
                        }

                        TalentsBase talents = character.CurrentTalents;
                        Dictionary<string, PropertyInfo> glyphProperties = new Dictionary<string, PropertyInfo>();
                        foreach (PropertyInfo pi in talents.GetType().GetProperties())
                        {
                            GlyphDataAttribute[] glyphDatas = pi.GetCustomAttributes(typeof(GlyphDataAttribute), true) as GlyphDataAttribute[];
                            if (glyphDatas.Length > 0)
                            {
                                GlyphDataAttribute glyphData = glyphDatas[0];
                                glyphProperties[glyphData.Name] = pi;
                            }
                        }

                        foreach (XElement glyph in xtalent.Element("glyphInfo").Elements("glyph"))
                        {
                            PropertyInfo pi;
                            if (glyphProperties.TryGetValue(glyph.Attribute("name").Value, out pi))
                            {
                                pi.SetValue(talents, true, null);
                            }
                        }
                    }
                }

                //TODO: Implement pet talent info parsing
                if (character.Class == CharacterClass.Hunter)
                {
                    List<ArmoryPet> pets = new List<ArmoryPet>();
                    // Pull Pet(s) Info if you are a Hunter
                    foreach (XElement xtalent in xchar.Element("petTalentInfo").Elements("talent"))
                    {
                        string petname  = xtalent.Attribute("name").Value;
                        string fam      = xtalent.Attribute("family_id").Value;
                        string SpecKey  = "";// xtalent.Attribute("key").Value;
                        string spec     = xtalent.Attribute("value").Value;
                        //
                        bool processed = false;
                        foreach (ArmoryPet a in pets) {
                            if (a.Name == petname) { processed = true; break; } // This one has already been done
                        }
                        if (!processed) { pets.Add(new ArmoryPet(fam, petname, SpecKey, spec)); }
                    }
                    if (pets.Count > 0) { character.ArmoryPets = pets; }
                }

                foreach (XElement xprofession in xchar.Element("professionInfo").Elements("profession"))
                {
                    Profession profession = (Profession)(int.Parse(xprofession.Attribute("id").Value));
                    if (character.PrimaryProfession == Profession.None)
                        character.PrimaryProfession = profession;
                    else
                        character.SecondaryProfession = profession;

                    switch (profession)
                    {
                        case Profession.Mining:
                            character.ActiveBuffs.Add(Buff.GetBuffByName("Toughness"));
                            break;
                        case Profession.Skinning:
                            character.ActiveBuffs.Add(Buff.GetBuffByName("Master of Anatomy"));
                            break;
                        case Profession.Blacksmithing:
                            character.WristBlacksmithingSocketEnabled = true;
                            character.HandsBlacksmithingSocketEnabled = true;
                            break;
                    }
                }

                Calculations.GetModel(character.CurrentModel).SetDefaults(character);

                e.Result = new KeyValuePair<Character, Dictionary<CharacterSlot, ItemInstance>>(character, items);
            }
            catch (Exception ex)
            {
                (sender as BackgroundWorker).ReportProgress(0, ex.Message + "|" + ex.StackTrace);
            }
        }
        #endregion

        #region Items
        public event EventHandler<EventArgs<Item>> GetItemCompleted;
        public void GetItemAsync(int itemId)
        {
            _lastItemId = itemId;
            _lastRequestWasItem = true;
            _webClient.DownloadStringAsync(new Uri(string.Format(URL_ITEM, itemId)));
            this.Progress = "Downloading Item Data...";
        }

        void bwParseItem_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                Progress = "Complete!";
                if (this.GetItemCompleted != null)
                    this.GetItemCompleted(this, new EventArgs<Item>(e.Result as Item));
            }
        }

        private void bwParseItem_DoWork(object sender, DoWorkEventArgs e)
        {
            XDocument xdoc = e.Argument as XDocument;
            int id = 0;
            try
            {
                XElement xtooltip = xdoc.Root.Element("page").Element("itemTooltips").Element("itemTooltip");
                ItemLocation location = LocationFactory.Create(xdoc, xdoc.Root.Element("item").Attribute("id").Value);

                ItemQuality quality = ItemQuality.Common;
                ItemType type = ItemType.None;
                ItemSlot socketColor1 = ItemSlot.None;
                ItemSlot socketColor2 = ItemSlot.None;
                ItemSlot socketColor3 = ItemSlot.None;
                Stats socketStats = new Stats();
                string name = string.Empty;
                string iconPath = string.Empty;
                string setName = string.Empty;
                ItemSlot slot = ItemSlot.None;
                Stats stats = new Stats();
                int inventoryType = -1;
                int classId = -1;
                string subclassName = string.Empty;
                int minDamage = 0;
                int maxDamage = 0;
                ItemDamageType damageType = ItemDamageType.Physical;
                float speed = 0f;
                List<string> requiredClasses = new List<string>();
                bool unique = false;
                int itemLevel = 0;

                foreach (XElement node in xtooltip.SelectNodes("id")) { id = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("name")) { name = node.Value; }
                foreach (XElement node in xtooltip.SelectNodes("icon")) { iconPath = node.Value; }
                foreach (XElement node in xtooltip.SelectNodes("maxCount")) { unique = node.Value == "1"; }
                foreach (XElement node in xtooltip.SelectNodes("overallQualityId")) { quality = (ItemQuality)Enum.Parse(typeof(ItemQuality), node.Value, false); }
                foreach (XElement node in xtooltip.SelectNodes("classId")) { classId = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("equipData/inventoryType")) { inventoryType = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("equipData/subclassName")) { subclassName = node.Value; }
                foreach (XElement node in xtooltip.SelectNodes("damageData/damage/min")) { minDamage = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("damageData/damage/max")) { maxDamage = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("damageData/damage/type")) { damageType = (ItemDamageType)Enum.Parse(typeof(ItemDamageType), node.Value, false); }
                foreach (XElement node in xtooltip.SelectNodes("damageData/speed")) { speed = float.Parse(node.Value, System.Globalization.CultureInfo.InvariantCulture); }
                foreach (XElement node in xtooltip.SelectNodes("setData/name")) { setName = node.Value; }
                foreach (XElement node in xtooltip.SelectNodes("allowableClasses/class")) { requiredClasses.Add(node.Value); }

                itemLevel = int.Parse(xtooltip.Element("itemLevel").Value);

                if (inventoryType >= 0)
                    slot = GetItemSlot(inventoryType, classId);
                if (!string.IsNullOrEmpty(subclassName))
                    type = GetItemType(subclassName, inventoryType, classId);

                // fix class restrictions on BOP items that can only be made by certain classes
                switch (id)
                {
                    case 35181:
                    case 32495:
                        requiredClasses.Add("Priest");
                        break;
                    case 32476:
                    case 35184:
                    case 32475:
                    case 34355:
                        requiredClasses.Add("Shaman");
                        break;
                    case 32474:
                    case 34356:
                        requiredClasses.Add("Hunter");
                        break;
                    case 46106:
                    case 32479:
                    case 32480:
                    case 46109:
                        requiredClasses.Add("Druid");
                        break;
                    case 32478:
                    case 34353:
                        requiredClasses.Add("Druid");
                        requiredClasses.Add("Rogue");
                        break;
                }

                foreach (XElement node in xtooltip.SelectNodes("bonusAgility")) { stats.Agility = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusAttackPower")) { stats.AttackPower = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("armor")) { stats.Armor = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusDefenseSkillRating")) { stats.DefenseRating = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusDodgeRating")) { stats.DodgeRating = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusParryRating")) { stats.ParryRating = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusBlockRating")) { stats.BlockRating = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusBlockValue")) { stats.BlockValue = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("blockValue")) { stats.BlockValue = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusResilienceRating")) { stats.Resilience = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusStamina")) { stats.Stamina = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusIntellect")) { stats.Intellect = int.Parse(node.Value); }

                foreach (XElement node in xtooltip.SelectNodes("bonusStrength")) { stats.Strength = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusHitRating")) { stats.HitRating = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusHasteRating")) { stats.HasteRating = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusCritRating")) { stats.CritRating = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusExpertiseRating")) { stats.ExpertiseRating = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusArmorPenetration")) { stats.ArmorPenetrationRating = int.Parse(node.Value); }

                foreach (XElement node in xtooltip.SelectNodes("arcaneResist")) { stats.ArcaneResistance = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("fireResist")) { stats.FireResistance = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("frostResist")) { stats.FrostResistance = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("natureResist")) { stats.NatureResistance = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("shadowResist")) { stats.ShadowResistance = int.Parse(node.Value); }

                foreach (XElement node in xtooltip.SelectNodes("bonusCritSpellRating")) { stats.CritRating = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusHitSpellRating")) { stats.HitRating = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusHasteSpellRating")) { stats.HasteRating = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusSpellPower")) { stats.SpellPower = int.Parse(node.Value); }

                foreach (XElement node in xtooltip.SelectNodes("bonusMana")) { stats.Mana = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusSpirit")) { stats.Spirit = int.Parse(node.Value); }
                foreach (XElement node in xtooltip.SelectNodes("bonusManaRegen")) { stats.Mp5 = int.Parse(node.Value); }

                if (slot == ItemSlot.Finger ||
                    slot == ItemSlot.MainHand ||
                    slot == ItemSlot.Neck ||
                    (slot == ItemSlot.OffHand && type != ItemType.Shield) ||
                    slot == ItemSlot.OneHand ||
                    slot == ItemSlot.Trinket ||
                    slot == ItemSlot.TwoHand)
                {
                    stats.BonusArmor += stats.Armor;
                    stats.Armor = 0f;
                }

                if (slot == ItemSlot.Back)
                {
                    float baseArmor = 0;
                    switch (quality)
                    {
                        case ItemQuality.Temp:
                        case ItemQuality.Poor:
                        case ItemQuality.Common:
                        case ItemQuality.Uncommon:
                            baseArmor = (float)itemLevel * 1.19f + 5.1f;
                            break;

                        case ItemQuality.Rare:
                            baseArmor = ((float)itemLevel + 26.6f) * 16f / 25f;
                            break;

                        case ItemQuality.Epic:
                        case ItemQuality.Legendary:
                        case ItemQuality.Artifact:
                        case ItemQuality.Heirloom:
                            baseArmor = ((float)itemLevel + 358f) * 7f / 26f;
                            break;
                    }

                    baseArmor = (float)Math.Floor(baseArmor);
                    stats.BonusArmor = stats.Armor - baseArmor;
                    stats.Armor = baseArmor;
                }

                foreach (XElement node in xtooltip.SelectNodes("spellData/spell"))
                {
                    bool isEquip = false;
                    bool isUse = false;
                    string spellDesc = null;
                    foreach (XElement childNode in node.Elements())
                    {
                        if (childNode.Name == "trigger")
                        {
                            isEquip = childNode.Value == "1";
                            isUse = childNode.Value == "0";
                        }
                        if (childNode.Name == "desc")
                            spellDesc = childNode.Value;
                    }

                    //parse Use/Equip lines
                    if (isUse) SpecialEffects.ProcessUseLine(spellDesc, stats, true, id);
                    if (isEquip) SpecialEffects.ProcessEquipLine(spellDesc, stats, true, itemLevel, id);
                }

                List<XElement> socketNodes = new List<XElement>(xtooltip.SelectNodes("socketData/socket"));
                if (socketNodes.Count > 0) socketColor1 = (ItemSlot)Enum.Parse(typeof(ItemSlot), socketNodes[0].Attribute("color").Value, false);
                if (socketNodes.Count > 1) socketColor2 = (ItemSlot)Enum.Parse(typeof(ItemSlot), socketNodes[1].Attribute("color").Value, false);
                if (socketNodes.Count > 2) socketColor3 = (ItemSlot)Enum.Parse(typeof(ItemSlot), socketNodes[2].Attribute("color").Value, false);
                string socketBonusesString = string.Empty;
                foreach (XElement node in xtooltip.SelectNodes("socketData/socketMatchEnchant")) { socketBonusesString = node.Value.Trim('+'); }
                if (!string.IsNullOrEmpty(socketBonusesString))
                {
                    try
                    {
                        List<string> socketBonuses = new List<string>();
                        string[] socketBonusStrings = socketBonusesString.Split(new string[] { " and ", " & ", ", " }, StringSplitOptions.None);
                        foreach (string socketBonusString in socketBonusStrings)
                        {
                            if (socketBonusString.LastIndexOf('+') > 2 && socketBonusString.LastIndexOf('+') < socketBonusString.Length - 3)
                            {
                                socketBonuses.Add(socketBonusString.Substring(0, socketBonusString.IndexOf(" +")));
                                socketBonuses.Add(socketBonusString.Substring(socketBonusString.IndexOf(" +") + 1));
                            }
                            else
                                socketBonuses.Add(socketBonusString);
                        }
                        foreach (string socketBonus in socketBonuses)
                        {
                            int socketBonusValue = 0;
                            if (socketBonus.IndexOf(' ') > 0) socketBonusValue = int.Parse(socketBonus.Substring(0, socketBonus.IndexOf(' ')));
                            switch (socketBonus.Substring(socketBonus.IndexOf(' ') + 1))
                            {
                                case "Agility":
                                    socketStats.Agility = socketBonusValue;
                                    break;
                                case "Stamina":
                                    socketStats.Stamina = socketBonusValue;
                                    break;
                                case "Dodge Rating":
                                    socketStats.DodgeRating = socketBonusValue;
                                    break;
                                case "Parry Rating":
                                    socketStats.ParryRating = socketBonusValue;
                                    break;
                                case "Block Rating":
                                    socketStats.BlockRating = socketBonusValue;
                                    break;
                                case "Block Value":
                                    socketStats.BlockValue = socketBonusValue;
                                    break;
                                case "Defense Rating":
                                    socketStats.DefenseRating = socketBonusValue;
                                    break;
                                case "Hit Rating":
                                    socketStats.HitRating = socketBonusValue;
                                    break;
                                case "Haste Rating":
                                    socketStats.HasteRating = socketBonusValue;
                                    break;
                                case "Expertise Rating":
                                    socketStats.ExpertiseRating = socketBonusValue;
                                    break;
                                case "Armor Penetration Rating":
                                    socketStats.ArmorPenetrationRating = socketBonusValue;
                                    break;
                                case "Strength":
                                    socketStats.Strength = socketBonusValue;
                                    break;
                                case "Healing":
                                    //case "Healing +4 Spell Damage":
                                    //case "Healing +3 Spell Damage":
                                    //case "Healing +2 Spell Damage":
                                    //case "Healing +1 Spell Damage":
                                    //case "Healing and +4 Spell Damage":
                                    //case "Healing and +3 Spell Damage":
                                    //case "Healing and +2 Spell Damage":
                                    //case "Healing and +1 Spell Damage":
                                    if (socketBonusValue == 0)
                                        socketStats.SpellPower = (float)Math.Round(int.Parse(socketBonuses[0].Substring(0, socketBonuses[0].IndexOf(' '))) / 1.88f);
                                    else
                                        socketStats.SpellPower = (float)Math.Round(socketBonusValue / 1.88f);
                                    break;
                                case "Spell Damage":
                                    // Only update Spell Damage if its not already set (Incase its an old heal bonus)
                                    if (socketStats.SpellPower == 0)
                                        socketStats.SpellPower = socketBonusValue;
                                    //sockets.Stats.Healing = socketBonusValue;
                                    break;
                                case "Spell Power":
                                    socketStats.SpellPower = socketBonusValue;
                                    break;
                                case "Crit Rating":
                                case "Crit Strike Rating":
                                case "Critical Rating":
                                case "Critical Strike Rating":
                                    socketStats.CritRating = socketBonusValue;
                                    break;
                                case "Attack Power":
                                    socketStats.AttackPower = socketBonusValue;
                                    break;
                                case "Weapon Damage":
                                    socketStats.WeaponDamage = socketBonusValue;
                                    break;
                                case "Resilience":
                                case "Resilience Rating":
                                    socketStats.Resilience = socketBonusValue;
                                    break;
                                //case "Spell Damage and Healing":
                                //    sockets.Stats.SpellDamageRating = socketBonusValue;
                                //    sockets.Stats.Healing = socketBonusValue;
                                //    break;
                                case "Spell Hit Rating":
                                    socketStats.HitRating = socketBonusValue;
                                    break;
                                case "Intellect":
                                    socketStats.Intellect = socketBonusValue;
                                    break;
                                case "Spell Crit":
                                case "Spell Crit Rating":
                                case "Spell Critical":
                                case "Spell Critical Rating":
                                case "Spell Critical Strike Rating":
                                    socketStats.CritRating = socketBonusValue;
                                    break;
                                case "Spell Haste Rating":
                                    socketStats.HasteRating = socketBonusValue;
                                    break;
                                case "Spirit":
                                    socketStats.Spirit = socketBonusValue;
                                    break;
                                case "Mana every 5 seconds":
                                case "Mana ever 5 Sec":
                                case "mana per 5 sec":
                                case "mana per 5 sec.":
                                case "Mana per 5 sec.":
                                case "Mana per 5 Seconds":
                                    socketStats.Mp5 = socketBonusValue;
                                    break;
                            }
                        }
                    }
                    catch { }
                }
                foreach (XElement nodeGemProperties in xtooltip.SelectNodes("gemProperties"))
                {
                    List<string> gemBonuses = new List<string>();
                    string[] gemBonusStrings = nodeGemProperties.Value.Split(new string[] { " and ", " & ", ", " }, StringSplitOptions.None);
                    foreach (string gemBonusString in gemBonusStrings)
                    {
                        if (gemBonusString.IndexOf('+') != gemBonusString.LastIndexOf('+'))
                        {
                            gemBonuses.Add(gemBonusString.Substring(0, gemBonusString.IndexOf(" +")));
                            gemBonuses.Add(gemBonusString.Substring(gemBonusString.IndexOf(" +") + 1));
                        }
                        else
                            gemBonuses.Add(gemBonusString);
                    }
                    foreach (string gemBonus in gemBonuses)
                    {
                        if (gemBonus == "Spell Damage +6")
                        {
                            stats.SpellPower = 6.0f;
                        }
                        else if (gemBonus == "2% Increased Armor Value from Items")
                        {
                            stats.BaseArmorMultiplier = 0.02f;
                        }
                        else if (gemBonus == "Stamina +6")
                        {
                            stats.Stamina = 6.0f;
                        }
                        else if (gemBonus == "Chance to restore mana on spellcast")
                        {
                            stats.ManaRestoreOnCast_5_15 = 300; // IED
                        }
                        else if (gemBonus == "Chance on spellcast - next spell cast in half time" || gemBonus == "Chance to Increase Spell Cast Speed")
                        {
                            stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast, new Stats() { HasteRating = 320 }, 6, 45, 0.15f));
                        }
                        else if (gemBonus == "+10% Shield Block Value")
                        {
                            stats.BonusBlockValueMultiplier = 0.1f;
                        }
                        else if (gemBonus == "+2% Intellect")
                        {
                            stats.BonusIntellectMultiplier = 0.02f;
                        }
                        else if (gemBonus == "2% Reduced Threat")
                        {
                            stats.ThreatReductionMultiplier = 0.02f;
                        }
                        else if (gemBonus == "3% Increased Critical Healing Effect")
                        {
                            stats.BonusCritHealMultiplier = 0.03f;
                        }
                        else
                        {
                            try
                            {
                                int gemBonusValue = int.Parse(gemBonus.Substring(0, gemBonus.IndexOf(' ')).Trim('+').Trim('%'));
                                switch (gemBonus.Substring(gemBonus.IndexOf(' ') + 1).Trim())
                                {
                                    case "to All Stats":
                                    case "All Stats":
                                        stats.Agility = gemBonusValue;
                                        stats.Strength = gemBonusValue;
                                        stats.Stamina = gemBonusValue;
                                        stats.Intellect = gemBonusValue;
                                        stats.Spirit = gemBonusValue;
                                        break;
                                    case "Resist All":
                                        stats.ArcaneResistance = gemBonusValue;
                                        stats.FireResistance = gemBonusValue;
                                        stats.FrostResistance = gemBonusValue;
                                        stats.NatureResistance = gemBonusValue;
                                        stats.ShadowResistance = gemBonusValue;
                                        break;
                                    case "Increased Critical Damage":
                                        stats.BonusCritMultiplier = (float)gemBonusValue / 100f;
                                        stats.BonusSpellCritMultiplier = (float)gemBonusValue / 100f; // both melee and spell crit use the same text, would have to disambiguate based on other stats
                                        break;
                                    case "Agility":
                                        stats.Agility = gemBonusValue;
                                        break;
                                    case "Stamina":
                                        stats.Stamina = gemBonusValue;
                                        break;
                                    case "Dodge Rating":
                                        stats.DodgeRating = gemBonusValue;
                                        break;
                                    case "Parry Rating":
                                        stats.ParryRating = gemBonusValue;
                                        break;
                                    case "Block Rating":
                                        stats.BlockRating = gemBonusValue;
                                        break;
                                    case "Defense Rating":
                                        stats.DefenseRating = gemBonusValue;
                                        break;
                                    case "Hit Rating":
                                        stats.HitRating = gemBonusValue;
                                        break;
                                    case "Haste Rating":
                                        stats.HasteRating = gemBonusValue;
                                        break;
                                    case "Expertise Rating":
                                        stats.ExpertiseRating = gemBonusValue;
                                        break;
                                    case "Armor Penetration Rating":
                                        stats.ArmorPenetrationRating = gemBonusValue;
                                        break;
                                    case "Strength":
                                        stats.Strength = gemBonusValue;
                                        break;
                                    case "Crit Rating":
                                    case "Crit Strike Rating":
                                    case "Critical Rating":
                                    case "Critical Strike Rating":
                                        stats.CritRating = gemBonusValue;
                                        break;
                                    case "Attack Power":
                                        stats.AttackPower = gemBonusValue;
                                        break;
                                    case "Weapon Damage":
                                        stats.WeaponDamage = gemBonusValue;
                                        break;
                                    case "Resilience":
                                    case "Resilience Rating":
                                        stats.Resilience = gemBonusValue;
                                        break;
                                    case "Spell Hit Rating":
                                        stats.HitRating = gemBonusValue;
                                        break;
                                    case "Spell Haste Rating":
                                        stats.HasteRating = gemBonusValue;
                                        break;
                                    case "Spell Damage":
                                        // Ignore spell damage from gem if Healing has already been applied, as it might be a "9 Healing 3 Spell" gem. 
                                        if (stats.SpellPower == 0)
                                            stats.SpellPower = gemBonusValue;
                                        break;
                                    case "Spell Damage and Healing":
                                        stats.SpellPower = gemBonusValue;
                                        break;
                                    case "Healing":
                                        stats.SpellPower = (float)Math.Round(gemBonusValue / 1.88f);
                                        break;
                                    case "Spell Power":
                                        stats.SpellPower = gemBonusValue;
                                        break;
                                    case "Spell Crit":
                                    case "Spell Crit Rating":
                                    case "Spell Critical":
                                    case "Spell Critical Rating":
                                        stats.CritRating = gemBonusValue;
                                        break;
                                    case "Mana every 5 seconds":
                                    case "Mana ever 5 Sec":
                                    case "mana per 5 sec":
                                    case "mana per 5 sec.":
                                    case "Mana per 5 Seconds":
                                        stats.Mp5 = gemBonusValue;
                                        break;
                                    case "Intellect":
                                        stats.Intellect = gemBonusValue;
                                        break;
                                    case "Spirit":
                                        stats.Spirit = gemBonusValue;
                                        break;
                                }
                            }
                            catch { }
                        }
                    }
                }
                string desc = string.Empty;
                foreach (XElement node in xtooltip.SelectNodes("desc")) { desc = node.Value; }
                if (desc.Contains("Matches any socket"))
                {
                    slot = ItemSlot.Prismatic;
                }
                else if (desc.Contains("Matches a "))
                {
                    bool red = desc.Contains("Red");
                    bool blue = desc.Contains("Blue");
                    bool yellow = desc.Contains("Yellow");
                    slot = red && blue && yellow ? ItemSlot.Prismatic :
                        red && blue ? ItemSlot.Purple :
                        blue && yellow ? ItemSlot.Green :
                        red && yellow ? ItemSlot.Orange :
                        red ? ItemSlot.Red :
                        blue ? ItemSlot.Blue :
                        yellow ? ItemSlot.Yellow :
                        ItemSlot.None;
                }
                else if (desc.Contains("meta gem slot"))
                    slot = ItemSlot.Meta;

                Item item = new Item()
                {
                    Id = id,
                    Name = name,
                    Quality = quality,
                    Type = type,
                    IconPath = iconPath,
                    Slot = slot,
                    SetName = setName,
                    Stats = stats,
                    SocketColor1 = socketColor1,
                    SocketColor2 = socketColor2,
                    SocketColor3 = socketColor3,
                    SocketBonus = socketStats,
                    MinDamage = minDamage,
                    MaxDamage = maxDamage,
                    DamageType = damageType,
                    Speed = speed,
                    RequiredClasses = string.Join("|", requiredClasses.ToArray()),
                    Unique = unique,
                    ItemLevel = itemLevel,
                };
                e.Result = item;
            }
            catch (Exception ex)
            {
                (sender as BackgroundWorker).ReportProgress(0, ex.Message + "|" + ex.StackTrace);
            }
        }

        private static ItemType GetItemType(string subclassName, int inventoryType, int classId)
        {
            switch (subclassName.ToLower())
            {
                case "cloth":
                    return ItemType.Cloth;

                case "leather":
                    return ItemType.Leather;

                case "mail":
                    return ItemType.Mail;

                case "plate":
                    return ItemType.Plate;

                case "dagger":
                    return ItemType.Dagger;

                case "fist weapon":
                    return ItemType.FistWeapon;

                case "axe":
                    if (inventoryType == 17)
                        return ItemType.TwoHandAxe;
                    else
                        return ItemType.OneHandAxe;

                case "mace":
                    if (inventoryType == 17)
                        return ItemType.TwoHandMace;
                    else
                        return ItemType.OneHandMace;

                case "sword":
                    if (inventoryType == 17)
                        return ItemType.TwoHandSword;
                    else
                        return ItemType.OneHandSword;

                case "polearm":
                    return ItemType.Polearm;

                case "staff":
                    return ItemType.Staff;

                case "shield":
                    return ItemType.Shield;

                case "bow":
                    return ItemType.Bow;

                case "crossbow":
                    return ItemType.Crossbow;

                case "gun":
                    return ItemType.Gun;

                case "wand":
                    return ItemType.Wand;

                case "thrown":
                    return ItemType.Thrown;

                case "idol":
                    return ItemType.Idol;

                case "libram":
                    return ItemType.Libram;

                case "totem":
                    return ItemType.Totem;

                case "arrow":
                    return ItemType.Arrow;

                case "bullet":
                    return ItemType.Bullet;

                case "quiver":
                    return ItemType.Quiver;

                case "ammo pouch":
                    return ItemType.AmmoPouch;

                case "sigil":
                    return ItemType.Sigil;

                default:
                    return ItemType.None;
            }
        }

        private static ItemSlot GetItemSlot(int inventoryType, int classId)
        {
            switch (classId)
            {
                case 6:
                    return ItemSlot.Projectile;

                case 11:
                    return ItemSlot.ProjectileBag;
            }

            switch (inventoryType)
            {
                case 1:
                    return ItemSlot.Head;

                case 2:
                    return ItemSlot.Neck;

                case 3:
                    return ItemSlot.Shoulders;

                case 16:
                    return ItemSlot.Back;

                case 5:
                case 20:
                    return ItemSlot.Chest;

                case 4:
                    return ItemSlot.Shirt;

                case 19:
                    return ItemSlot.Tabard;

                case 9:
                    return ItemSlot.Wrist;

                case 10:
                    return ItemSlot.Hands;

                case 6:
                    return ItemSlot.Waist;

                case 7:
                    return ItemSlot.Legs;

                case 8:
                    return ItemSlot.Feet;

                case 11:
                    return ItemSlot.Finger;

                case 12:
                    return ItemSlot.Trinket;

                case 13:
                    return ItemSlot.OneHand;

                case 17:
                    return ItemSlot.TwoHand;

                case 21:
                    return ItemSlot.MainHand;

                case 14:
                case 22:
                case 23:
                    return ItemSlot.OffHand;

                case 15:
                case 25:
                case 26:
                case 28:
                    return ItemSlot.Ranged;

                case 24:
                    return ItemSlot.Projectile;

                case 27:
                    return ItemSlot.ProjectileBag;

                default:
                    return ItemSlot.None;
            }
        }
        #endregion

    }
}





/*
 * 
<itemData> 
<item id='51931' type='plate'> 
<sourceInfo> 
<source area='Icecrown Citadel' id='36612' is_heroic='1' name='Lord Marrowgar' players='10' source_type='drop'> 
</source> 
</sourceInfo> 
</item> 
<page globalSearch="1" lang="en_us" requestUrl="/item-tooltip.xml"> 
  <itemxtooltips> 
    <itemxtooltip> 
      <id>51931</id> 
      <name>Ancient Skeletal Boots</name> 
      <icon>inv_boots_plate_14</icon> 
      <heroic>1</heroic> 
      <overallQualityId>4</overallQualityId> 
      <bonding>1</bonding> 
      <classId>4</classId> 
      <equipData> 
        <inventoryType>8</inventoryType> 
        <subclassName>Plate</subclassName> 
      </equipData> 
      <damageData/> 
      <bonusStamina>92</bonusStamina> 
      <bonusIntellect>92</bonusIntellect> 
      <armor armorBonus="0">1815</armor> 
      <socketData> 
        <socket color="Yellow"/> 
        <socket color="Red"/> 
      </socketData> 
      <durability current="75" max="75"/> 
      <requiredLevel>80</requiredLevel> 
      <itemLevel>264</itemLevel> 
      <bonusSpellPower>122</bonusSpellPower> 
      <bonusCritRating>80</bonusCritRating> 
      <bonusManaRegen>32</bonusManaRegen> 
      <itemSource areaId="4812" areaName="Icecrown Citadel (10)" creatureId="36612" creatureName="Lord Marrowgar" difficulty="h" dropRate="2" value="sourceType.creatureDrop"/> 
    </itemxtooltip> 
  </itemxtooltips> 
</page> 
 
</itemData> 
 * 
 * 
 * 
 * 
 * 
TODO: Handle errors.
 * 
 * Item Errors:
 * badItemID - Either a blank item id or something that fails to parse into a number (or 0) is passed
 * queueFailure - It couldn't queue the item, but no data is found, should never happen
 * noItem - Item ID entered doesn't exist (Armory returned no data)
 * maintenance
 * 500
 * 503
 *
 * Character Errors:
 * ...
 * maintenance
 * 500
 * 503
*/