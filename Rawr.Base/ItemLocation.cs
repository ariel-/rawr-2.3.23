using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace Rawr
{
    public enum ItemSource
    {
        NotFound,           // doesnt exist in the armory
        None,               // sourceType.none
        Unknown,            // Failed to parse the sourcetype or the extended data
        Vendor,
        Faction,
        PVP,
        Crafted,
        WorldDrop,
        StaticDrop,
        Quest,
        Container,
    }

    public enum ReputationLevel
    {
        Hated,
        Hostile,
        Unfriendly,
        Neutral,
        Friendly,
        Honored,
        Revered,
        Exalted,
    }

    public enum BindsOn
    {
        None,   // 0: No Soulbonding
        BoP,    // 1: Binds on Pickup
        BoE,    // 2: Binds on Equip
        BoA,    // 3: Binds on Account
        BoU,    // 4: Binds on Use
    }

    delegate ItemLocation Construct();
    
    [GenerateArraySerializer]
    [XmlInclude(typeof(StaticDrop))]
    [XmlInclude(typeof(NoSource))]
    [XmlInclude(typeof(UnknownItem))]
    [XmlInclude(typeof(WorldDrop))]
    [XmlInclude(typeof(PvpItem))]
    [XmlInclude(typeof(VendorItem))]
    [XmlInclude(typeof(FactionItem))]
    [XmlInclude(typeof(CraftedItem))]
    [XmlInclude(typeof(QuestItem))]
    [XmlInclude(typeof(ContainerItem))]
    public class ItemLocation
    {
        [XmlIgnore]
        public ItemSource Source{get;set;}
        public string Note { get; set; }
        private string _description;
        [XmlIgnore]
        public virtual string Description { get { return _description; } }
        public ItemLocation() : this("Unknown location") { }
        public ItemLocation(string desc) {
            Source = ItemSource.Unknown;
            _description = desc;
        }
        public virtual ItemLocation Fill(XmlDocument doc, XmlNode node, string itemId)
        {
            return this;
        }
        public static ItemLocation Construct() {
            return new ItemLocation("");
        }
    }
    
    public class NoSource : ItemLocation {
        [XmlIgnore]
        public override string Description { get { return "Armory reports no source, try Wowhead"; } }
        public NoSource() : base("") { Source = ItemSource.None; }
        public new static ItemLocation Construct() {
            return new NoSource();
        }
    }
    
    public class UnknownItem : ItemLocation {
        [XmlIgnore]
        public override string Description { get { return "Not found on armory"; } }
        public UnknownItem() : base("") { Source = ItemSource.NotFound; }
        public new static ItemLocation Construct() {
            return new UnknownItem();
        }
    }
    
    public class VendorItem : ItemLocation
    {
        public SerializableDictionary<string, int> TokenMap
        {
            get { return _tokenMap; }
            set { _tokenMap = value; }
        }

        private SerializableDictionary<string, int> _tokenMap = new SerializableDictionary<string, int>();

        public int Cost { get; set; }
        public string VendorName { get; set; }
        public string VendorArea { get; set; }
     
        [XmlIgnore]
        public string CostString {
            get {
                int total = Cost;
                int gold = total / 10000;
                total -= gold * 10000;
                int silver = total / 100;
                total -= silver * 100;

                if (gold > 0) {
                    return string.Format("{0}g{1}s{2}c", gold, silver, total);
                } else if (silver > 0) {
                    return string.Format("{1}s{2}c", gold, silver, total);
                } else if (total > 0) {
                    return string.Format("{2}c", gold, silver, total);
                }
                return "";
            }
        }

        public VendorItem() {
            Source = ItemSource.Vendor;
        }

        [XmlIgnore]
        public override string Description
        {
            get {
                if (_tokenMap.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    bool first = true;
                    foreach (string key in _tokenMap.Keys)
                    {
                        if (!first)
                        {
                            sb.Append("and ");
                        }
                        sb.AppendFormat("{0} [{1}] ", _tokenMap[key], key);
                        first = false;
                    }
                    if (Cost > 0)
                    {
                        return string.Format("Purchasable with {1}and {0}", CostString, sb.ToString());
                    }
                    else
                    {
                        return string.Format("Purchasable with {0}", sb.ToString());
                    }
                }
                else if (Cost > 0)
                {
                    return string.Format("Purchasable with {0}", CostString);
                }
                else
                {
                    return string.Format("Sold by {0} at {1}", VendorName, VendorArea);
                }
            }
        }

        public override ItemLocation Fill(XmlDocument doc, XmlNode node, string itemId)
        {
			if (doc != null)
			{
                // Save the informations of the vendor.
                string _vendorName = "Unknown";
                string _vendorArea = "*";
                XmlNodeList listVendor = doc.SelectNodes("/page/itemInfo/item/vendors/creature");
                if (listVendor.Count > 0)
                {
                    _vendorName = listVendor[0].Attributes["name"].Value.Replace("quot;", "'");
                    _vendorArea = listVendor[0].Attributes["area"].Value;
                }

                foreach (XmlNode subNode in doc.SelectNodes("/page/itemInfo/item/cost/token"))
				{
					string tokenId = subNode.Attributes["id"].Value;
					int count = int.Parse(subNode.Attributes["count"].Value);

					string tokenName = null;

					if (!_idToNameMap.TryGetValue(tokenId, out tokenName))
					{
                        WebRequestWrapper wrw = new WebRequestWrapper();
						doc = wrw.DownloadItemInformation(int.Parse(tokenId));

						_idToNameMap[tokenId] = tokenName = doc.SelectSingleNode("/page/itemInfo/item/@name").InnerText;
					}

                    _tokenMap[tokenName] = count;
				}

                // The armory (falsly) list a buy price for some items which have a cost in badges.
                // So only check for a gold price if there's not also a token cost.
                Cost = 0;
                if (_tokenMap.Count == 0)
                {
                    XmlNode costNode = doc.SelectSingleNode("/page/itemInfo/item/cost/@buyPrice");
                    if (costNode != null)
                    {
                        Cost = int.Parse(costNode.InnerText);
                    }
                }

                if (listVendor.Count > 0)
                {
                    VendorName = _vendorName;
                    VendorArea = _vendorArea;
                }
                else if (_tokenMap.Count == 0)
                {
                    return new ItemLocation("Vendor");
                }
			}

            return this;
        }
        public static new ItemLocation Construct()
        {
            VendorItem item = new VendorItem();
            return item;
        }

        static SortedList<string, string> _idToNameMap = new SortedList<string, string>();
        static SortedList<string, string> _bossToAreaMap = new SortedList<string, string>();
    }
    
    public class FactionItem : ItemLocation
    {
        [XmlIgnore]
        public string CostString {
            get 
            {
                int total = Cost;
                int gold = total / 10000;
                total -= gold * 10000;
                int silver = total / 100;
                total -= silver * 100;

                if (gold > 0) {
                    return string.Format("{0}g{1}s{2}c", gold, silver, total);
                } else if (silver > 0) {
                    return string.Format("{1}s{2}c", gold, silver, total);
                }
                return string.Format("{2}c", gold, silver, total);
            }
        }

        public string FactionName { get; set; }
        public ReputationLevel Level { get; set; }
        public int Cost { get; set; }

        public SerializableDictionary<string, int> TokenMap 
        {
            get { return _tokenMap; }
            set { _tokenMap = value; }
        }

        static SortedList<string, string> tokenIDMap = new SortedList<string, string>();
        private SerializableDictionary<string, int> _tokenMap = new SerializableDictionary<string, int>();

        public FactionItem() { Source = ItemSource.Faction; }

        public override string Description {
            get 
            {
                if (_tokenMap.Count > 0) {
                    StringBuilder sb = new StringBuilder();
                    foreach (string key in _tokenMap.Keys) {
                        sb.AppendFormat("{0} [{1}] ", _tokenMap[key], key);
                    }
                    if(Cost>0) {
                        return string.Format("Purchasable for {3}{2} and requires {0} - {1}", FactionName, Level.ToString(), CostString, sb.ToString());
                    } else {
                        return string.Format("Purchasable for {2} and requires {0} - {1}", FactionName, Level.ToString(), sb.ToString());
                    }
                } else if (Cost > 0) {
                    return string.Format("Purchasable for {2} and requires {0} - {1}", FactionName, Level.ToString(), CostString);
                } else {
                    return string.Format("Purchasable and requires {0} - {1}", FactionName, Level.ToString());
                }
            }
        }

        public override ItemLocation Fill(XmlDocument doc, XmlNode node, string itemId)
        {
            XmlNode subNode = doc.SelectSingleNode("/page/itemInfo/item/rewardFromQuests/quest[1]");

            if (subNode != null)
            {
                return QuestItem.Construct().Fill(doc, node, itemId);
            }
            subNode = doc.SelectSingleNode("/page/itemInfo/item/cost/@buyPrice");

            if (subNode != null) {
                Cost = int.Parse(subNode.InnerText);
            } else {
                Cost = 0;
            }

            foreach (XmlNode token in doc.SelectNodes("/page/itemInfo/item/cost/token"))
            {
                int Count = int.Parse(token.Attributes["count"].Value);
                string id = token.Attributes["id"].Value;
                string tokenName;
                lock (tokenIDMap)
                {
                    if (!tokenIDMap.TryGetValue(id, out tokenName))
                    {
                        WebRequestWrapper wrw2 = new WebRequestWrapper();
                        XmlDocument doc2 = wrw2.DownloadItemInformation(int.Parse(id));

                        tokenIDMap[id] = tokenName = doc2.SelectSingleNode("/page/itemInfo/item/@name").InnerText;
                    }
                }

                _tokenMap[tokenName] = Count;
            }

            subNode = node.SelectSingleNode("/page/itemTooltips/itemTooltip/requiredFaction");
            if(subNode != null)
            {
                FactionName = subNode.Attributes["name"].Value;
                Level = (ReputationLevel)System.Enum.Parse(typeof(ReputationLevel), subNode.Attributes["rep"].Value);
            }

            return this;
        }
        public static new ItemLocation Construct() {
            FactionItem item = new FactionItem();
            return item;
        }
    }
    
    public class PvpItem : ItemLocation
    {
        public string PointType { get; set; }
        public int Points { get; set; }
        public string TokenType { get; set; }
        public int TokenCount { get; set; }
        public string PointTypeA { get; set; }
        public int PointsA { get; set; }
        public int RequiredArenaRating { get; set; }
        public string VendorName { get; set; }
        public string VendorArea { get; set; }

        public PvpItem() {
            Source = ItemSource.PVP;
        }

        [XmlIgnore]
        public override string Description {
            get {
                string RAR = RequiredArenaRating > 0 ? string.Format(" and Requires an Arena Rating of {0}", RequiredArenaRating) : "";
                if (TokenCount > 0 && PointsA > 0 && Points > 0) {
                    return string.Format("Purchasable for {0} {1} Points, {2} {3} Points and {4} [{5}]" + RAR, Points, PointType, PointsA, PointTypeA, TokenCount, TokenType);
                } else if (TokenCount > 0 && Points > 0) {
                    return string.Format("Purchasable for {0} {1} Points and {1} [{2}]" + RAR, Points, PointType, TokenCount, TokenType);
                } else if (TokenCount > 0) {
                    return string.Format("Purchasable for {0} [{1}]" + RAR, TokenCount, TokenType);
                } else if (PointsA > 0 && Points > 0) {
                    return string.Format("Purchasable for {0} {1} Points and {2} {3} Points" + RAR, Points, PointType, PointsA, PointTypeA);
                } else if (Points > 0) {
                    return string.Format("Purchasable for {0} {1} Points" + RAR, Points, PointType);
                } else if (VendorName != null) {
                    return string.Format("PvP item sold by {0} at {1}" + RAR, VendorName, VendorArea);
                }
                return "Purchasable PvP Item" + RAR;
            }
        }

        public override ItemLocation Fill(XmlDocument doc, XmlNode node, string itemId)
        {
            string TokenId;
            XmlNode subNodeH = doc.SelectSingleNode("/page/itemInfo/item/cost/@honor");
            XmlNode subNodeA = doc.SelectSingleNode("/page/itemInfo/item/cost/@arena");
            XmlNode subNodeRAR = node.SelectSingleNode("page/itemTooltips/itemTooltip/requiredPersonalArenaRating/@personalArenaRating");
            XmlNodeList vendorList = doc.SelectNodes("/page/itemInfo/item/vendors/creature");

            if (subNodeRAR != null) {
                RequiredArenaRating = int.Parse(subNodeRAR.InnerText);
            }

            if (subNodeH != null && subNodeA != null) {
                Points = int.Parse(subNodeH.InnerText);
                PointType = "Honor";
                subNodeH = doc.SelectSingleNode("/page/itemInfo/item/cost/token/@count");
                if (subNodeH != null) {
                    TokenCount = int.Parse(subNodeH.InnerText);
                    TokenId = doc.SelectSingleNode("/page/itemInfo/item/cost/token").Attributes["id"].Value;

                    if (_tokenMap.ContainsKey(TokenId)) {
                        TokenType = _tokenMap[TokenId];
                    } else {
                        WebRequestWrapper wrw = new WebRequestWrapper();
                        doc = wrw.DownloadItemInformation(int.Parse(TokenId));

                        TokenType = doc.SelectSingleNode("/page/itemInfo/item").Attributes["name"].Value;

                        _tokenMap[TokenId] = TokenType;
                    }
                }
                subNodeA = doc.SelectSingleNode("/page/itemInfo/item/cost/@arena");
                if (subNodeA != null) {
                    PointsA = int.Parse(subNodeA.InnerText);
                    PointTypeA = "Arena";
                }
            }
            else if (subNodeH != null)
            {
                Points = int.Parse(subNodeH.InnerText);
                PointType = "Honor";
                subNodeH = doc.SelectSingleNode("/page/itemInfo/item/cost/token/@count");
                if (subNodeH != null)
                {
                    TokenCount = int.Parse(subNodeH.InnerText);
                    TokenId = doc.SelectSingleNode("/page/itemInfo/item/cost/token").Attributes["id"].Value;

                    if (_tokenMap.ContainsKey(TokenId))
                    {
                        TokenType = _tokenMap[TokenId];
                    }
                    else
                    {
                        WebRequestWrapper wrw = new WebRequestWrapper();
                        doc = wrw.DownloadItemInformation(int.Parse(TokenId));

                        TokenType = doc.SelectSingleNode("/page/itemInfo/item").Attributes["name"].Value;

                        _tokenMap[TokenId] = TokenType;
                    }
                }
            }
            else if (subNodeA != null)
            {
                Points = int.Parse(subNodeA.InnerText);
                PointType = "Arena";
                subNodeH = doc.SelectSingleNode("/page/itemInfo/item/cost/token/@count");
                if (subNodeH != null)
                {
                    TokenCount = int.Parse(subNodeH.InnerText);
                    TokenId = doc.SelectSingleNode("/page/itemInfo/item/cost/token").Attributes["id"].Value;

                    if (_tokenMap.ContainsKey(TokenId))
                    {
                        TokenType = _tokenMap[TokenId];
                    }
                    else
                    {
                        WebRequestWrapper wrw2 = new WebRequestWrapper();
                        doc = wrw2.DownloadItemInformation(int.Parse(TokenId));

                        TokenType = doc.SelectSingleNode("/page/itemInfo/item").Attributes["name"].Value;

                        _tokenMap[TokenId] = TokenType;
                    }
                }
            } 
            else if (vendorList.Count > 0) 
            {
                VendorName = vendorList[0].Attributes["name"].Value;
                VendorArea = vendorList[0].Attributes["area"].Value;
            }
            if (string.IsNullOrEmpty(PointType)) { PointType = "Unknown"; }
            return this;
        }
        public static new ItemLocation Construct()
        {
            PvpItem item = new PvpItem();
            return item;
        }
        static SortedList<string, string> _tokenMap = new SortedList<string, string>();
    }
    
    public class StaticDrop : ItemLocation
    {
        public string Area{get;set;}
        public bool Heroic{get;set;}
        public string Boss{get;set;}

        [XmlIgnore]
        public override string Description
        {
            get
            {
                return string.Format("Drops from {0} in {1}{2}", Boss, Heroic ? "Heroic " : "", Area);
            }
        }
        public StaticDrop()
        {
            Source = ItemSource.StaticDrop;
        }

        public override ItemLocation Fill(XmlDocument doc, XmlNode node, string itemId)
        {
            XmlNode subNodeArea = doc.SelectSingleNode("/page/itemInfo/item/dropCreatures/creature[1]/@area");
            if (subNodeArea != null)
            {
                Area = subNodeArea.InnerText;
            }
            XmlNode subNodeHeroic = doc.SelectSingleNode("/page/itemInfo/item/dropCreatures/creature[1]/@heroic");
            if (subNodeHeroic != null)
            {
                Heroic = (subNodeHeroic.InnerText == "1");
            }

            XmlNode subNodeBoss = doc.SelectSingleNode("/page/itemInfo/item/dropCreatures/creature[1]/@name");
            if (subNodeBoss != null)
            {
                Boss = subNodeBoss.InnerText;
            }
            return this;
        }
        public static new ItemLocation Construct()
        {
            StaticDrop item = new StaticDrop();
            return item;
        }
    }
    
    public class WorldDrop : ItemLocation
    {
        public string Location { get; set; }
        public bool Heroic { get; set; }

        [XmlIgnore]
        public override string Description
        {
            get
            {
                if(Location != null)
                {
                    return string.Format("Trash drop in {0}{1}", Heroic ? "Heroic " : "", Location);
                }
                return "World Drop";
            }
        }
        public WorldDrop()
        {
            Source = ItemSource.WorldDrop;
        }


        public override ItemLocation Fill(XmlDocument doc, XmlNode node, string itemId)
        {
            XmlNode subNodeArea = doc.SelectSingleNode("/page/itemInfo/item/dropCreatures/creature[1]/@area");
            if(subNodeArea != null)
            {
                Location = subNodeArea.InnerText;
            }
            XmlNode subNodeHeroic = doc.SelectSingleNode("/page/itemInfo/item/dropCreatures/creature[1]/@heroic");
            if (subNodeHeroic != null)
            {
                Heroic = (subNodeHeroic.InnerText == "1");
            }

            return this;
        }
        public static new ItemLocation Construct()
        {
            WorldDrop item = new WorldDrop();
            return item;
        }

    }
    
    public class CraftedItem : ItemLocation
    {
        private CraftedItem()
        {
            Source = ItemSource.Crafted;
        }
        [XmlIgnore]
        public override string Description
        {
            get
            {
                if(Skill == "Unknown")
                {
                    if (SpellName != null)
                    {
                        return string.Format("Created via {0}", SpellName);
                    }
                    return "Crafted";
                }
                StringBuilder basic = new StringBuilder();

                if (Bind != BindsOn.None)
                {
                    if (Level != 0)
                    {
                    basic.AppendFormat("{0} crafted {1}({2}) ", Bind.ToString(), Skill, Level);
                }
                else
                {
                        basic.AppendFormat("{0} crafted {1} ", Bind.ToString(), Skill);
                    }
                }
                else
                {
                    if (Level != 0)
                    {
                    basic.AppendFormat("Crafted {1}({2})", Bind.ToString(), Skill, Level);
                }
                    else
                    {
                        basic.AppendFormat("Crafted {1}", Bind.ToString(), Skill);
                    }
                }
                if (BopMats.Count > 0)
                {
                    basic.Append(" using ");
                    foreach (string key in BopMats.Keys)
                    {
                        basic.AppendFormat("{0} {1}", BopMats[key], key);
                    }
                }
                return basic.ToString();
            }
        }


        public string Skill {get;set;}
        public int Level {get;set;}
        public BindsOn Bind {get;set;}
        public string SpellName{get;set;}


        public SerializableDictionary<string, int> BopMats
        {
            get
            {
                return _bopMats;
            }
        }

        private SerializableDictionary<string, int> _bopMats = new SerializableDictionary<string, int>();
        static SortedList<string, BindsOn> _materialBindMap = new SortedList<string, BindsOn>();
        public override ItemLocation Fill(XmlDocument doc, XmlNode node, string itemId)
        {
            XmlNode subNode = doc.SelectSingleNode("/page/itemInfo/item/createdBy/spell/item");

            if (subNode != null)
            {
                Bind = (BindsOn) Enum.Parse(typeof(BindsOn), node.SelectSingleNode("/page/itemTooltips/itemTooltip/bonding").InnerText);
            }
            else
            {
                subNode = doc.SelectSingleNode("/page/itemInfo/item");
                if (subNode.Attributes["requiredSkill"]!= null)
                {
                    Bind = BindsOn.BoP;
                }
                else
                {
                    Bind = BindsOn.BoE;
                }
            }
            if (subNode.Attributes["requiredSkill"] != null)
            {
                Skill = subNode.Attributes["requiredSkill"].Value;
                Level = int.Parse(subNode.Attributes["requiredSkillRank"].Value);
            }
            else 
            {
                Skill = "Unknown";
            }

            subNode = doc.SelectSingleNode("/page/itemInfo/item/createdBy/spell/@name");
            if (subNode != null)
            {
                SpellName = subNode.InnerText;
            }


            foreach(XmlNode reagent in doc.SelectNodes("/page/itemInfo/item/createdBy/spell/reagent"))
            {
                string name = reagent.Attributes["name"].Value;
                int count = int.Parse(reagent.Attributes["count"].Value);
                if (!_materialBindMap.ContainsKey(name))
                {
                    WebRequestWrapper wrw = new WebRequestWrapper();
                    doc = wrw.DownloadItemToolTipSheet(reagent.Attributes["id"].Value);

                    _materialBindMap[name] = (BindsOn) Enum.Parse(typeof(BindsOn), doc.SelectSingleNode("/page/itemTooltips/itemTooltip/bonding").InnerText);
                }

                if (_materialBindMap[name] == BindsOn.BoP)
                {
                    BopMats[name] = count;
                }

            }

            return this;
        }
        public static new ItemLocation Construct()
        {
            CraftedItem item = new CraftedItem();
            return item;
        }
    }
    
    public class QuestItem : ItemLocation
    {
        private QuestItem()
        {
            Source = ItemSource.Quest;
        }
        [XmlIgnore]
        public override string Description
        {
            get
            {
                return string.Format("Reward from [{0}{1}{2}] {3} in {4}", MinLevel > 0 ? MinLevel.ToString() : "", Party > 0 ? "g" : Type, Party > 0 ? Party.ToString() : "", Quest, Area);
            }
        }


        public String Area {get;set;}
        public String Quest {get;set;}
        public int MinLevel {get;set;}
        public int Party {get;set;}
        public String Type {get;set;}

        public override ItemLocation Fill(XmlDocument doc, XmlNode node, string itemId)
        {
            XmlNode subNode = doc.SelectSingleNode("/page/itemInfo/item/rewardFromQuests/quest[1]");

            Area = subNode.Attributes["area"].Value;
            Quest = subNode.Attributes["name"].Value;
            MinLevel = int.Parse(subNode.Attributes["reqMinLevel"].Value);
            Party = int.Parse(subNode.Attributes["suggestedPartySize"].Value);
            if (subNode.Attributes["type"] != null)
            {
                switch (subNode.Attributes["type"].Value)
                {
                    case "Dungeon": Type = "d"; break;
                    case "Raid": Type = "r"; break;
                    default: Type = ""; break;
                }
            }
            else
            {
                Type = "";
            }
            return this;
        }
        public static new ItemLocation Construct()
        {
            QuestItem item = new QuestItem();
            return item;
        }
    }
    
    public class ContainerItem : ItemLocation
    {
        public ContainerItem()
        {
            Source = ItemSource.Container;
        }
        [XmlIgnore]
        public override string Description
        {
            get
            {
                return string.Format("Found in {0} in {1}{2}", Container, Heroic ? "Heroic " : "", Area);
            }
        }

        public bool Heroic { get; set; }
        public String Area { get; set; }
        public String Container { get; set; }
        public int MinLevel { get; set; }
        public int Party { get; set; }

        public override ItemLocation Fill(XmlDocument doc, XmlNode node, string itemId)
        {
            XmlNode subNode = doc.SelectSingleNode("/page/itemInfo/item/containerObjects/object[1]");

            Heroic = (subNode.Attributes["heroic"] != null && subNode.Attributes["heroic"].Value == "1");
            Area = /*(Heroic ? "Heroic " : "") +*/ subNode.Attributes["area"].Value;
			if (Area.Contains("(25)") && !Area.StartsWith("Heroic "))
			{
				if (doc.SelectSingleNode("/page/itemInfo/item").Attributes["level"].Value == "258")
					Area = "Heroic " + Area;
			}
			else if (Area.Contains("(10)") && !Area.StartsWith("Heroic "))
			{
				if (doc.SelectSingleNode("/page/itemInfo/item").Attributes["level"].Value == "245")
					Area = "Heroic " + Area;
			}
            Area = Area.Replace("Heroic Heroic", "Heroic");
            Container = subNode.Attributes["name"].Value;
            return this;
        }
        public static new ItemLocation Construct()
        {
            ContainerItem item = new ContainerItem();
            return item;
        }
    }

    [XmlRoot("dictionary")]
    [GenerateSerializer]
    public class ItemLocationDictionary : SerializableDictionary<string, ItemLocation[]>
    {
    }

    public static class LocationFactory
    {
        static LocationFactory()
        {
            _LocationFactory = new SortedList<string, Construct>();

            _LocationFactory["sourceType.worldDrop"] = WorldDrop.Construct;
            _LocationFactory["sourceType.creatureDrop"] = StaticDrop.Construct;
            _LocationFactory["sourceType.pvpReward"] = PvpItem.Construct;
            _LocationFactory["sourceType.factionReward"] = FactionItem.Construct;
            _LocationFactory["sourceType.vendor"] = VendorItem.Construct;
            _LocationFactory["sourceType.createdByPlans"] = CraftedItem.Construct;
            _LocationFactory["sourceType.createdBySpell"] = CraftedItem.Construct;
            _LocationFactory["sourceType.questReward"] = QuestItem.Construct;
            _LocationFactory["sourceType.gameObjectDrop"] = ContainerItem.Construct;
            _LocationFactory["sourceType.none"] = NoSource.Construct;
        }

        static ItemLocationDictionary _allLocations = new ItemLocationDictionary();

        public static ItemLocation[] Lookup(int id)
        {
            return Lookup(id.ToString());
        }

        private static ItemLocation[] unknownLocation = { new ItemLocation("Unknown Location, please refresh"), null };

        public static ItemLocation[] Lookup(string id)
        {
            ItemLocation[] item;
            if(_allLocations.TryGetValue(id, out item))
            {
                return item;
            }
            return unknownLocation;
            //item = new ItemLocation[] { new ItemLocation("Unknown Location, please refresh"), null };
            //return item;
        }

        public static void Save(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), fileName), false, Encoding.UTF8))
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ItemLocationDictionary));
                serializer.Serialize(writer, _allLocations);
                writer.Close();
            }
        }

        public static void Load(string fileName)
        {
            ItemLocationDictionary sourceInfo = null;
            _allLocations.Clear();
            if (File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), fileName)))
            {
                try
                {
                    string xml = System.IO.File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), fileName));
                    
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ItemLocationDictionary));
                    System.IO.StringReader reader = new System.IO.StringReader(xml);
                    sourceInfo = (ItemLocationDictionary)serializer.Deserialize(reader);
                    reader.Close();
                    _allLocations = sourceInfo;
                }
                catch (Exception)
                {
                }
            }

        }

        public static ItemLocation Create(XmlNode node, string itemId)
        {
            ItemLocation item = null;

            try
            {
                if (node != null && node.SelectSingleNode("page/itemTooltips/itemTooltip/itemSource") != null)
                {
                    WebRequestWrapper wrw = new WebRequestWrapper();
                    XmlDocument doc = wrw.DownloadItemInformation(int.Parse(itemId));
                    string sourceType = node.SelectSingleNode("page/itemTooltips/itemTooltip/itemSource").Attributes["value"].Value;

                    // If the armory has an item source set to creatureDrop, check to see if it might also be a vendor sold item
                    // and if so, switch it's type to vendor
                    if (sourceType == "sourceType.creatureDrop")
                    {
			            if (doc != null)
			            {
				            XmlNode subNode = doc.SelectSingleNode("/page/itemInfo/item/cost/token");
                            if (subNode != null)
                            {
                                sourceType = "sourceType.vendor";
                            }
                        }
                    }
                    // If the armory has an item source set to vendor, check to see if it cost honor or arena point to purchase and if so,
                    // switch it's type to pvpItem
                    if (sourceType == "sourceType.vendor")
                    {
                        if (doc != null)
                        {
                            XmlNode subNodeH = doc.SelectSingleNode("/page/itemInfo/item/cost/@honor");
                            XmlNode subNodeA = doc.SelectSingleNode("/page/itemInfo/item/cost/@arena");

                            if (subNodeH != null || subNodeA != null)
                            {
                                sourceType = "sourceType.pvpReward";
                            }
                        }
                    }
                    if (_LocationFactory.ContainsKey(sourceType))
                    {
                        item = _LocationFactory[sourceType]();
                        item = item.Fill(doc, node, itemId);
                    }
                    else
                    {
                        throw new Exception("Unrecognized item source " + sourceType);
                    }
                }
                else
                {
                    item = UnknownItem.Construct();
                }
            }
            catch (System.Exception e)
            {
                item = new ItemLocation("Failed - " + e.Message);
            }
            ItemLocation[] prev = { null, null };
            _allLocations.TryGetValue(itemId, out prev);
            if (prev != null) item.Note = prev[0].Note;
            _allLocations[itemId] = new ItemLocation[] {item, null};

            return item;
        }

		public static void Add(string itemId, ItemLocation itemLocation)
		{
            lock (_allLocations)
		    {
                _allLocations[itemId] = new ItemLocation[] { itemLocation, null };
            }
		}

        public static void Add(string itemId, ItemLocation[] itemLocation, bool allow2ndsource)
        {
            lock (_allLocations)
            {
                _allLocations[itemId] = itemLocation;
            }
        }

        static SortedList<string, Construct> _LocationFactory = null;
    }
}