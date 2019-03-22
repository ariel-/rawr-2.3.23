using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

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
    };

    public enum BindsOn
    {
        None,
        BoP,
        BoE,
        BoA,
        BoU,
    };

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
        public virtual string Description
        {
            get{return _description;}
        }
        public ItemLocation():this("Unknown location")
        {            
        }
        public ItemLocation(string desc)
        {
            Source = ItemSource.Unknown;
            _description = desc;
        }
        public virtual ItemLocation Fill(XDocument xdoc, string itemId)
        {
            return this;
        }
        public static ItemLocation Construct()
        {
            return new ItemLocation("");
        }

        public override string ToString()
        {
            return Description;
        }
    }

    
    public class NoSource : ItemLocation
    {
        public NoSource():base("")
        {
            Source = ItemSource.None;
        }
        public new static ItemLocation Construct()
        {
            return new NoSource();
        }
    }
    
    public class UnknownItem : ItemLocation
    {
        [XmlIgnore]
        public override string Description
        {
            get
            {
                return "Not found on armory";
            }
        }

        public UnknownItem()
            : base("")
        {
            Source = ItemSource.NotFound;
        }
        public new static ItemLocation Construct()
        {
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
        public string CostString
        {
            get
            {
                int total = Cost;
                int gold = total / 10000;
                total -= gold * 10000;
                int silver = total / 100;
                total -= silver * 100;

                if (gold > 0)
                {
                    return string.Format("{0}g{1}s{2}c", gold, silver, total);
                }
                else if (silver > 0)
                {
                    return string.Format("{1}s{2}c", gold, silver, total);
                }
                else if (total > 0)
                {
                    return string.Format("{2}c", gold, silver, total);
                }
                return "";
            }

        }



        public VendorItem()
        {
            Source = ItemSource.Vendor;
        }

        [XmlIgnore]
        public override string Description
        {
            get
            {
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


        public override ItemLocation Fill(XDocument xdoc, string itemId)
        {
			if (xdoc != null)
			{
				XElement subNode = xdoc.SelectSingleNode("itemData/page/itemInfo/item/cost/token");
				if (subNode != null)
				{

					string tokenId = subNode.Attribute("id").Value;
					int count = int.Parse(subNode.Attribute("count").Value);

					string Boss = null;
					string Area = null;

					if (!_idToBossMap.ContainsKey(tokenId))
					{
                        //itemInfo = wrw.DownloadItemInformation(int.Parse(tokenId));

                        //List<XElement> list = new List<XElement>(xdoc.SelectNodes("/itemData/page/itemInfo/item/dropCreatures/creature"));

                        //subNode = list[0];
                        //if (list.Count == 1)
                        //{
                        //    Boss = subNode.Attribute("name").Value;
                        //    Area = subNode.Attribute("area").Value;
                        //    _idToBossMap[tokenId] = Boss;
                        //    _bossToAreaMap[Boss] = Area;
                        //}
                        //else if (list.Count > 1)
                        //{
                        //    Boss = subNode.SelectSingleNode("/itemData/page/itemInfo/item").Attribute("name").Value;
                        //    Area = "*";
                        //}
                        //else
                        //{
                        //    Boss = "Unknown";
                        //    Area = "*";
                        //}

                        //_idToBossMap[tokenId] = Boss;
                        //_bossToAreaMap[Boss] = Area;
                        _idToBossMap[tokenId] = "<nyi>";
                        _bossToAreaMap[Boss] = "<nyi>";
					}
					else
					{
						Boss = _idToBossMap[tokenId];
						Area = _bossToAreaMap[Boss];
					}

                    _tokenMap[Boss] = count;
				}
                else
                {
					List<XElement> list = new List<XElement>(xdoc.SelectNodes("/itemData/page/itemInfo/item/vendors/creature"));
                    if (list.Count > 0)
                    {
                        VendorName = list[0].Attribute("name").Value;
                        VendorArea = list[0].Attribute("area").Value;
                    }
                    else
                    {
                        return new ItemLocation("Vendor");
                    }
                }
			}

            return this;
        }
        public static new ItemLocation Construct()
        {
            VendorItem item = new VendorItem();
            return item;
        }

        static Dictionary<string, string> _idToBossMap = new Dictionary<string, string>();
        static Dictionary<string, string> _bossToAreaMap = new Dictionary<string, string>();
    }
  
    
    public class FactionItem : ItemLocation
    {

        [XmlIgnore]
        public string CostString
        {
            get
            {
                int total = Cost;
                int gold = total / 10000;
                total -= gold * 10000;
                int silver = total / 100;
                total -= silver * 100;

                if (gold > 0)
                {
                    return string.Format("{0}g{1}s{2}c", gold, silver, total);
                }
                else if (silver > 0)
                {
                    return string.Format("{1}s{2}c", gold, silver, total);
                }
                return string.Format("{2}c", gold, silver, total);
            }

        }
        public string FactionName {get;set;}
        public ReputationLevel Level{get;set;}
        public int Cost{get;set;}
        public SerializableDictionary<string, int> TokenMap
        {
            get
            {
                return _tokenMap;
            }
            set
            {
                _tokenMap = value;
            }
        }


        static Dictionary<string, string> tokenIDMap = new Dictionary<string, string>();
        private SerializableDictionary<string, int> _tokenMap = new SerializableDictionary<string, int>();

        public FactionItem()
        {
            Source = ItemSource.Faction;
        }

        public override string Description
        {
            get
            {
                if (_tokenMap.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string key in _tokenMap.Keys)
                    {
                        sb.AppendFormat("{0} [{1}] ", _tokenMap[key], key);
                    }

                    if(Cost>0)
                    {
                        return string.Format("Purchasable for {3}{2} and requires {0} - {1}", FactionName, Level.ToString(), CostString, sb.ToString());

                    }
                    else
                    {
                        return string.Format("Purchasable for {2} and requires {0} - {1}", FactionName, Level.ToString(), sb.ToString());

                    }
                }
                else if (Cost > 0)
                {
                    return string.Format("Purchasable for {2} and requires {0} - {1}", FactionName, Level.ToString(), CostString);
                }
                else
                {
                    return string.Format("Purchasable and requires {0} - {1}", FactionName, Level.ToString());
                }

            }
        }

		public override ItemLocation Fill(XDocument xdoc, string itemId)
        {
            NetworkUtils wrw = new NetworkUtils();

			XElement subNode = xdoc.SelectSingleNode("/itemData/page/itemInfo/item/rewardFromQuests/quest[1]");

            if (subNode != null)
            {
				return QuestItem.Construct().Fill(xdoc, itemId);
            }


            XElement cost = xdoc.SelectSingleNode("/itemData/page/itemInfo/item/cost");
            if (cost != null)
            {
                XAttribute priceAttr = cost.Attribute("buyPrice");

                if (priceAttr != null)
                {
                    Cost = int.Parse(subNode.Value);
                }
                else
                {
                    Cost = 0;
                }
            }

			foreach (XElement token in xdoc.SelectNodes("/itemData/page/itemInfo/item/cost/token"))
            {
                int Count = int.Parse(token.Attribute("count").Value);
                string id = token.Attribute("id").Value;
                if(!tokenIDMap.ContainsKey(id))
                {
                    //NetworkUtils wrw2 = new NetworkUtils();
                   // XDocument doc2 = wrw.DownloadItemInformation(int.Parse(id));

                    tokenIDMap[id] = "<nyi>";//doc2.SelectSingleNode("/itemData/page/itemInfo/item/@name").Value;
                }

                _tokenMap[tokenIDMap[id]] = Count;
            }


			subNode = xdoc.SelectSingleNode("/itemData/page/itemTooltips/itemTooltip/requiredFaction");
            if(subNode != null)
            {
                FactionName = subNode.Attribute("name").Value;
                Level = (ReputationLevel)System.Enum.Parse(typeof(ReputationLevel), subNode.Attribute("rep").Value, false);
            }

            return this;
        }
        public static new ItemLocation Construct()
        {
            FactionItem item = new FactionItem();
            return item;
        }
    }
   
    
    public class PvpItem : ItemLocation
    {
        public string PointType{get;set;}
        public int Points{get;set;}
        public string TokenType{get;set;}
        public int TokenCount{get;set;}

        public PvpItem()
        {
            Source = ItemSource.PVP;
        }

        [XmlIgnore]
        public override string Description
        {
            get
            {
                string points = string.Format("Purchasable for {0} {1} Points", Points, PointType);
                if (TokenCount > 0)
                {
                    if (Points > 0)
                    {
                        return string.Format("{0} and {1} [{2}]", points, TokenCount, TokenType);
                    }
                    else
                    {
                        return string.Format("Purchasable for {1} [{2}]", points, TokenCount, TokenType);
                    }
                }
                return points;
            }
        }

        public override ItemLocation Fill(XDocument xdoc, string itemId)
        {
            string TokenId;
            XElement cost = xdoc.SelectSingleNode("/itemData/page/itemInfo/item/cost");
            if (cost != null)
            {
                XAttribute subNode = cost.Attribute("honor");

                if (subNode != null)
                {
                    Points = int.Parse(subNode.Value);
                    PointType = "Honor";
                    subNode = xdoc.SelectSingleNode("/itemData/page/itemInfo/item/cost/token").Attribute("count");
                    if (subNode != null)
                    {

                        TokenCount = int.Parse(subNode.Value);
                        TokenId = xdoc.SelectSingleNode("/itemData/page/itemInfo/item/cost/token").Attribute("id").Value;

                        if (_tokenMap.ContainsKey(TokenId))
                        {
                            TokenType = _tokenMap[TokenId];
                        }
                        else
                        {
                            //wrw = new NetworkUtils();
                            //itemInfo = wrw.DownloadItemInformation(int.Parse(TokenId));
                            //TokenType = xdoc.SelectSingleNode("/itemData/page/itemInfo/item").Attribute("name").Value;

                            _tokenMap[TokenId] = "<nyi>";// TokenType;
                        }
                    }
                }
                else
                {
                    subNode = cost.Attribute("arena");
                    if (subNode != null)
                    {
                        Points = int.Parse(subNode.Value);
                        PointType = "Arena";
                    }
                }
            }

            return this;
        }
        public static new ItemLocation Construct()
        {
            PvpItem item = new PvpItem();
            return item;
        }
        static Dictionary<string, string> _tokenMap = new Dictionary<string, string>();
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

        public override ItemLocation Fill(XDocument xdoc,string itemId)
        {
            XElement subNode = xdoc.SelectSingleNode("itemData/page/itemTooltips/itemTooltip/itemSource");
            Area = subNode.Attribute("areaName").Value;
            Heroic = ("h" == subNode.Attribute("difficulty").Value);
            Boss = subNode.Attribute("creatureName").Value;
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
                    return string.Format("Trash drop in {0}{1}", Location, Heroic ? " Heroic" : "");
                }
                return "World Drop";
            }
        }
        public WorldDrop()
        {
            Source = ItemSource.WorldDrop;
        }


        public override ItemLocation Fill(XDocument xdoc, string itemId)
        {
            XAttribute subNodeArea = xdoc.SelectSingleNode("/itemData/item/sourceInfo/source").Attribute("area");
            if(subNodeArea != null)
            {
                Location = subNodeArea.Value;
            }
            XAttribute subNodeHeroic = xdoc.SelectSingleNode("/itemData/item/sourceInfo/source").Attribute("is_heroic");
            if (subNodeHeroic != null)
            {
                Heroic = (subNodeHeroic.Value == "1");
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
        public CraftedItem()
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
                    basic.AppendFormat("{0} crafted {1}({2}) ", Bind.ToString(), Skill, Level);
                }
                else
                {
                    basic.AppendFormat("Crafted {1}({2})", Bind.ToString(), Skill, Level);
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
        static Dictionary<string, BindsOn> _materialBindMap = new Dictionary<string, BindsOn>();
        public override ItemLocation Fill(XDocument xdoc, string itemId)
        {


            XElement subNode = xdoc.SelectSingleNode("/itemData/page/itemInfo/item/createdBy/spell/item");

            if (subNode != null)
            {
                Bind = (BindsOn) Enum.Parse(typeof(BindsOn), xdoc.SelectSingleNode("/itemData/page/itemTooltips/itemTooltip/bonding").Value, false);
            }
            else
            {
                subNode = xdoc.SelectSingleNode("/itemData/page/itemInfo/item");
                if (subNode != null)
                {
                    if (subNode.Attribute("requiredSkill") != null)
                    {
                        Bind = BindsOn.BoP;
                    }
                    else
                    {
                        Bind = BindsOn.BoE;
                    }
                }
            }
            if (subNode != null && subNode.Attribute("requiredSkill") != null)
            {
                Skill = subNode.Attribute("requiredSkill").Value;
                Level = int.Parse(subNode.Attribute("requiredSkillRank").Value);
            }
            else 
            {
                Skill = "Unknown";
            }

            XElement spell = xdoc.SelectSingleNode("/itemData/page/itemInfo/item/createdBy/spell");
            if (spell != null)
            {
                XAttribute attr = spell.Attribute("name");
                if (attr != null)
                {
                    SpellName = attr.Value;
                }
            }


            foreach(XElement reagent in xdoc.SelectNodes("/itemData/page/itemInfo/item/createdBy/spell/reagent"))
            {
                string name = reagent.Attribute("name").Value;
                int count = int.Parse(reagent.Attribute("count").Value);
                if (!_materialBindMap.ContainsKey(name))
                {
                    //wrw = new NetworkUtils();
                    //itemInfo = wrw.DownloadItemToolTipSheet(reagent.Attribute("id").Value);
                    //_materialBindMap[name] = (BindsOn) Enum.Parse(typeof(BindsOn), xdoc.SelectSingleNode("/itemData/page/itemTooltips/itemTooltip/bonding").Value, false);
                    _materialBindMap[name] = BindsOn.None;
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
        public QuestItem()
        {
            Source = ItemSource.Quest;
        }
        [XmlIgnore]
        public override string Description
        {
            get
            {
                return string.Format("Reward from [{0}{1}{2}] {3} in {4}", MinLevel, Party > 0 ? "g" : "", Party > 0 ? Party.ToString() : "", Quest, Area);                
            }
        }


        public String Area {get;set;}
        public String Quest {get;set;}
        public int MinLevel {get;set;}
        public int Party {get;set;}

        public override ItemLocation Fill(XDocument xdoc, string itemId)
        {

            XElement subNode = xdoc.SelectSingleNode("/itemData/item/sourceInfo/source");

            Area = subNode.Attribute("area").Value;
            Quest = subNode.Attribute("name").Value;
            //MinLevel = int.Parse(subNode.Attribute("reqMinLevel").Value);
            //Party = int.Parse(subNode.Attribute("suggestedPartySize").Value);
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

        public override ItemLocation Fill(XDocument xdoc, string itemId)
        {
            XElement subNode = xdoc.SelectSingleNode("/itemData/item/sourceInfo/source");

            Area = subNode.Attribute("area").Value;
            Heroic = subNode.Attribute("is_heroic").Value == "1";
			Container = subNode.Attribute("name").Value;
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
            _LocationFactory = new Dictionary<string, Construct>();

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

        public static ItemLocation[] Lookup(string id)
        {
            ItemLocation[] item = {null, null};
            if(_allLocations.TryGetValue(id, out item))
            {
                return item;
            }
            item = new ItemLocation[] { new ItemLocation("Unknown Location, please refresh"), null };

            return item;
        }

        public static void Save(TextWriter writer)
        {
            System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(ItemLocationDictionary));
            serializer.Serialize(writer, _allLocations);
            writer.Close();
        }

        public static void Load(TextReader reader)
        {
            ItemLocationDictionary sourceInfo = null;
            _allLocations.Clear();
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ItemLocationDictionary));
                sourceInfo = (ItemLocationDictionary)serializer.Deserialize(reader);
                reader.Close();
                _allLocations = sourceInfo;
            }
            catch (Exception) { }
        }

        public static ItemLocation Create(XDocument xdoc, string itemId)
        {
            ItemLocation item = null;

            try {
				if (xdoc != null && xdoc.SelectSingleNode("itemData/page/itemTooltips/itemTooltip/itemSource") != null)
                {
					string sourceType = xdoc.SelectSingleNode("itemData/page/itemTooltips/itemTooltip/itemSource").Attribute("value").Value;
                    if (_LocationFactory.ContainsKey(sourceType)) {
                        item = _LocationFactory[sourceType]();
						item = item.Fill(xdoc, itemId);
                    } else {
                        throw new Exception("Unrecognized item source " + sourceType);
                    }
                } else {
                    item = UnknownItem.Construct();
                }
            }
            catch (System.Exception e)
            {
                item = new ItemLocation("Failed - " + e.Message);
            }
            ItemLocation[] prev = {null,null};
            _allLocations.TryGetValue(itemId, out prev);
            if (prev != null && prev[0] != null) item.Note = prev[0].Note;
            _allLocations[itemId] = new ItemLocation[] { item, null };

            return item;
        }

		public static void Add(string itemId, ItemLocation itemLocation)
		{
			if (_allLocations.ContainsKey(itemId)) _allLocations.Remove(itemId);
            _allLocations.Add(itemId, new ItemLocation[] { itemLocation, null });
		}

        public static void Add(string itemId, ItemLocation[] itemLocation, bool allow2ndsource)
        {
            if (_allLocations.ContainsKey(itemId)) _allLocations.Remove(itemId);
            _allLocations.Add(itemId, itemLocation);
        }

        static Dictionary<string, Construct> _LocationFactory = null;
    }
}
