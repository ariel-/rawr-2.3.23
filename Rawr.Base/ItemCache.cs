using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
#if RAWR3
using System.Linq;
using System.Threading;
using System.Windows.Threading;
#endif

namespace Rawr
{
	public static class ItemCache
	{
#if !RAWR3
		public static readonly string SavedFilePath = 
            Path.Combine(
                Path.Combine(
                    Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), 
                    "Data"),
                "ItemCache.xml");
#endif

		private static ItemCacheInstance _instance = new ItemCacheInstance();
		public static ItemCacheInstance Instance
		{
			get { return _instance; }
			set { _instance = value; }
		}
#if RAWR3
		public static Dictionary<int, Item> Items { get { return _instance.Items; } }
#else
        public static SortedDictionary<int, Item> Items { get { return _instance.Items; } }
#endif

        public static void InvalidateCachedStats() { Instance.InvalidateCachedStats(); }
	
		public static Item FindItemById(int id) { return _instance.FindItemById(id); }
        public static bool ContainsItemId(int id) { return _instance.ContainsItemId(id); }

		public static Item AddItem(Item item) { return _instance.AddItem(item); }
		public static Item AddItem(Item item, bool raiseEvent) { return _instance.AddItem(item, raiseEvent); }

		public static void DeleteItem(Item item) { _instance.DeleteItem(item); }
		public static void DeleteItem(Item item, bool raiseEvent) { _instance.DeleteItem(item, raiseEvent); }

		public static Item[] AllItems { get { return _instance.AllItems; } }
		public static Item[] RelevantItems { get { return _instance.RelevantItems; } }

        public static Item[] GetUnfilteredRelevantItems(CalculationsBase model, CharacterRace race) { return _instance.GetUnfilteredRelevantItems(model, race); }
        public static Item[] GetRelevantItems(CalculationsBase model, CharacterRace race) { return _instance.GetRelevantItems(model, race); }

        public static void AutoSetUniqueId(Item item) { _instance.AutoSetUniqueId(item); }

		public static void OnItemsChanged() { _instance.OnItemsChanged(); }
#if RAWR3
        public static void Save(TextWriter writer) { _instance.Save(writer); }
        public static void Load(TextReader reader) { _instance.Load(reader); }
#else
		public static void Save() { _instance.Save(); }
		public static void Load() { _instance.Load(); }
#endif

        public static void SaveItemCost(TextWriter writer) { _instance.SaveItemCost(writer); }
        public static void LoadItemCost(TextReader reader) { _instance.LoadItemCost(reader); }
        public static void ResetItemCost() { _instance.ResetItemCost(); }
        public static void LoadTokenItemCost(string token) { _instance.LoadTokenItemCost(token); }
	}

	public class ItemCacheInstance
	{
		public ItemCacheInstance() {  }
		public ItemCacheInstance(ItemCacheInstance instanceToClone)
		{
#if RAWR3
			_items = new Dictionary<int, Item>();
#else
            _items = new SortedDictionary<int, Item>();
#endif
            lock (instanceToClone.Items)
            {
                foreach (KeyValuePair<int, Item> kvp in instanceToClone.Items)
                {
                    _items[kvp.Key] = kvp.Value;
                }
            }
		}

#if RAWR3
        private Dictionary<int, Item> _items;
        public Dictionary<int, Item> Items
        {
            get
            {
                if (_items == null) _items = new Dictionary<int, Item>();
                return _items;
            }
        }
#else
        private SortedDictionary<int, Item> _items;
        public SortedDictionary<int, Item> Items
		{
			get
			{
				if (_items == null)
					Load();
				return _items;
			}
		}
#endif

        public void InvalidateCachedStats()
        {
            lock (Items)
            {
                foreach (Item item in Items.Values)
                {
                    item.InvalidateCachedData();
                }
            }
        }

		public Item FindItemById(int id)
		{
			if (id > 0)
			{
				Item item;
                lock (Items)
                {
                    Items.TryGetValue(id, out item);
                }
				return item;
			}
			return null;
		}

        public bool ContainsItemId(int id)
        {
            if (id > 0)
            {
                lock (Items)
                {
                    return Items.ContainsKey(id);
                }
            }
            return false;
        }

#if RAWR3
        private bool dirtySinceLastAdd;
#endif

		public Item AddItem(Item item) { return AddItem(item, true); }
		public Item AddItem(Item item, bool raiseEvent)
		{
			if (item == null) return null;
			//Chasing the lies no one believed...

            Item cachedItem;
            lock (Items)
            {
                if (Items.TryGetValue(item.Id, out cachedItem))
                {
                    cachedItem.Delete();
                }
                item.LastChange = DateTime.Now;
                Items[item.Id] = item;
                AutoSetUniqueId(item);
            }

            if (raiseEvent)
            {
#if RAWR3
                // when loading a new character we might get a long stream of new items from armory service
                // if we trigger event for every single one the application becomes more or less unresponsive
                // this is all happening on the main thread, armory service background worker is pushing things
                // on the dispatcher so add items will actually be delayed by however long it takes to
                // compute all calculations for charts
                // so send this via thread pool, this way it'll be pushed after all the backlog of items
                dirtySinceLastAdd = true;
#if SILVERLIGHT
                Dispatcher dispatcher = System.Windows.Deployment.Current.Dispatcher;
#else
                Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
#endif
                ThreadPool.QueueUserWorkItem((object state) =>
                {                    
                    dispatcher.BeginInvoke((Action)(() =>
                    {
                        System.Diagnostics.Debug.WriteLine("Trying to trigger recalc " + dirtySinceLastAdd);
                        if (dirtySinceLastAdd)
                        {
                            OnItemsChanged();
                            dirtySinceLastAdd = false;
                        }
                    }));
                });
#else
                OnItemsChanged();
#endif
            }
			return item;
		}

		public void DeleteItem(Item item) { DeleteItem(item, true); }
		public void DeleteItem(Item item, bool raiseEvent)
		{
			if (item != null)
			{
                lock (Items)
                {
                    Item cachedItem;
                    if (Items.TryGetValue(item.Id, out cachedItem))
                    {
                        cachedItem.Delete();
                        Items.Remove(item.Id);
                    }
                }
			}
			if (raiseEvent) OnItemsChanged();
		}

		private Item[] _allItems = null;
		public Item[] AllItems
		{
			get
			{
				if (_allItems == null)
				{
                    lock (Items)
                    {
                        _allItems = new List<Item>(Items.Values).ToArray();
                    }
				}
				return _allItems;
			}
		}

		private Item[] _relevantItems = null;
		public Item[] RelevantItems
		{
			get
			{
				if (_relevantItems == null)
				{
					_relevantItems = GetRelevantItemsInternal(Calculations.Instance);
				}
				return _relevantItems;
			}
		}

        public Item[] GetUnfilteredRelevantItems(CalculationsBase model, CharacterRace race)
        {
            List<Item> itemList = new List<Item>(AllItems).FindAll(new Predicate<Item>(
                delegate(Item item) 
                { 
                    return model.IsItemRelevant(item) && item.FitsFaction(race); 
                }));
            return itemList.ToArray();
        }

        private CalculationsBase lastModel;
        private CharacterRace lastRace;
        private Item[] cachedRelevantItems;
        private object syncLock = new object();

        public Item[] GetRelevantItems(CalculationsBase model, CharacterRace race)
        {
            if (cachedRelevantItems == null || model != lastModel || race != lastRace)
            {
                lock (syncLock)
                {
                    // test again because of race conditions, but we still want to avoid the lock if we can because that'll be the majority case
                    if (cachedRelevantItems == null || model != lastModel || race != lastRace)
                    {

                        List<Item> itemList = new List<Item>(AllItems).FindAll(new Predicate<Item>(
                            delegate(Item item)
                            {
                                return model.IsItemRelevant(item) && ItemFilter.IsItemRelevant(model, item) && item.FitsFaction(race);
                            }));
                        cachedRelevantItems = itemList.ToArray();
                        lastModel = model;
                        lastRace = race;
                    }
                }
            }
            return cachedRelevantItems;
        }

        internal Item[] GetRelevantItemsInternal(CalculationsBase model)
        {
            List<Item> itemList = new List<Item>(AllItems).FindAll(new Predicate<Item>(
                delegate(Item item) { return model.IsItemRelevant(item) && ItemFilter.IsItemRelevant(model, item); }));
            return itemList.ToArray();
        }

		public event EventHandler ItemsChanged;
		public void OnItemsChanged()
		{
			_allItems = null;
			_relevantItems = null;
            cachedRelevantItems = null;
			if (ItemsChanged != null) ItemsChanged(null, null);
		}

        // load/save of item cost data
        public void LoadItemCost(TextReader reader)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<int, float>));
                SerializableDictionary<int, float> data = (SerializableDictionary<int, float>)serializer.Deserialize(reader);
                reader.Close();

                // reset all costs to 0, cost files only store nonzero cost items
                foreach (Item item in AllItems)
                {
                    item.Cost = 0.0f;
                }

                foreach (var kvp in data)
                {
                    Item item;
                    lock (Items)
                    {
                        if (Items.TryGetValue(kvp.Key, out item))
                        {
                            item.Cost = kvp.Value;
                        }
                    }
                }
                // don't need to invalidate relevant caches, but still trigger event to refresh graphs etc.
                if (ItemsChanged != null) ItemsChanged(null, null);
            }
            catch { }
        }

        public void SaveItemCost(TextWriter writer)
        {
            SerializableDictionary<int, float> data = new SerializableDictionary<int, float>();
            foreach (Item item in AllItems)
            {
                if (item.Cost > 0.0f)
                {
                    data.Add(item.Id, item.Cost);
                }
            }
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<int, float>));
            serializer.Serialize(writer, data);
            writer.Close();
        }

        public void ResetItemCost()
        {
            foreach (Item item in AllItems)
            {
                item.Cost = 0.0f;
            }
            // don't need to invalidate relevant caches, but still trigger event to refresh graphs etc.
            if (ItemsChanged != null) ItemsChanged(null, null);
        }

        public void LoadTokenItemCost(string token)
        {
            foreach (Item item in AllItems)
            {
                item.Cost = 0.0f; 
                if (item.LocationInfo[0].Source == ItemSource.Vendor)
                {
                    VendorItem vendor = item.LocationInfo[0] as VendorItem;
                    int count;
                    vendor.TokenMap.TryGetValue(token, out count);
                    item.Cost = count;
                }
            }
            // don't need to invalidate relevant caches, but still trigger event to refresh graphs etc.
            if (ItemsChanged != null) ItemsChanged(null, null);
        }

        private Dictionary<string, List<Item>> uniqueItemByName = new Dictionary<string, List<Item>>();

        /// <summary>
        /// Matches the item against known rules for unique item groups and sets the UniqueId list.
        /// </summary>
        /// <param name="item">Item for which to auto set the UniqueId list.</param>
        public void AutoSetUniqueId(Item item)
        {
            if (item.Unique && (item.Slot == ItemSlot.Trinket || item.Slot == ItemSlot.Finger || item.Slot == ItemSlot.OneHand)) // all items that have UniqueId rules are marked as Unique
            {
                // find all items in item cache with same name
                Item item251 = null, item258 = null, item264 = null, item271 = null, item277 = null, item284 = null;

                lock (Items)
                {
                    List<Item> list;
                    if (!uniqueItemByName.TryGetValue(item.Name, out list))
                    {
                        list = new List<Item>();
                        uniqueItemByName[item.Name] = list;
                    }
                    list.Add(item);

                    // we want the loop on Items because we don't want to trigger AllItems cache rebuilds
                    foreach (Item i in list)
                    {
                        Item validItem;
                        Items.TryGetValue(i.Id, out validItem);
                        if (i == validItem)
                        {
                            if (i.ItemLevel == 251)
                            {
                                item251 = i;
                            }
                            else if (i.ItemLevel == 258)
                            {
                                item258 = i;
                            }
                            else if (i.ItemLevel == 264)
                            {
                                item264 = i;
                            }
                            else if (i.ItemLevel == 271)
                            {
                                item271 = i;
                            }
                            else if (i.ItemLevel == 277)
                            {
                                item277 = i;
                            }
                            else if (i.ItemLevel == 284)
                            {
                                item284 = i;
                            }
                        }
                    }
                }

                // normal/heroic pair 10 man ICC with same name
                if ((object)item251 != null && (object)item264 != null)
                {
                    item251.UniqueId = new List<int>() { item251.Id, item264.Id };
                    item264.UniqueId = new List<int>() { item251.Id, item264.Id };
                }

                // normal/heroic pair 25 man ICC with same name
                if ((object)item264 != null && (object)item277 != null)
                {
                    item264.UniqueId = new List<int>() { item264.Id, item277.Id };
                    item277.UniqueId = new List<int>() { item264.Id, item277.Id };
                }

                // normal/heroic pair 10 man RS with same name
                if ((object)item258 != null && (object)item271 != null)
                {
                    item258.UniqueId = new List<int>() { item258.Id, item271.Id };
                    item271.UniqueId = new List<int>() { item258.Id, item271.Id };
                }

                // normal/heroic pair 25 man RS with same name
                if ((object)item271 != null && (object)item284 != null)
                {
                    item271.UniqueId = new List<int>() { item271.Id, item284.Id };
                    item284.UniqueId = new List<int>() { item271.Id, item284.Id };
                }
                
                // special rules for Ashen Verdict Rings
                // Ashen Band of Courage
                if (item.Id == 50375 || item.Id == 50404 || item.Id == 50388 || item.Id == 50403)
                {
                    item.UniqueId = new List<int>() { 50375, 50404, 50388, 50403 };
                }
                // Ashen Band of Destruction
                else if (item.Id == 50377 || item.Id == 50398 || item.Id == 50384 || item.Id == 50397)
                {
                    item.UniqueId = new List<int>() { 50377, 50398, 50384, 50397 };
                }
                // Ashen Band of Might
                else if (item.Id == 52572 || item.Id == 52570 || item.Id == 52569 || item.Id == 52571)
                {
                    item.UniqueId = new List<int>() { 52572, 52570, 52569, 52571 };
                }
                // Ashen Band of Vengeance
                else if (item.Id == 50402 || item.Id == 50387 || item.Id == 50401 || item.Id == 50376)
                {
                    item.UniqueId = new List<int>() { 50402, 50387, 50401, 50376 };
                }
                // Ashen Band of Wisdom
                else if (item.Id == 50400 || item.Id == 50386 || item.Id == 50399 || item.Id == 50378)
                {
                    item.UniqueId = new List<int>() { 50400, 50386, 50399, 50378 };
                }
            }
        }

#if RAWR3
        public void Save(TextWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ItemList));
            serializer.Serialize(writer, new ItemList(AllItems));
            writer.Close();
        }

        public void Load(TextReader reader)
        {
            _items = new Dictionary<int, Item>();
            List<Item> listItems = new List<Item>();
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ItemList));
                listItems = (List<Item>)serializer.Deserialize(reader);
                reader.Close();
            }
            catch { }

            if (listItems != null)
            {
                foreach (Item item in listItems)
                {
                    AddItem(item, false);
                }
            }
            Calculations.ModelChanged += new EventHandler(Calculations_ModelChanged);
        }
#else
		public void Save()
		{
#if !AGGREGATE_ITEMS
            using (StreamWriter writer = new StreamWriter(ItemCache.SavedFilePath, false, Encoding.UTF8))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ItemList));
                serializer.Serialize(writer, new ItemList(AllItems));
                writer.Close();
            }

			LocationFactory.Save(Path.Combine("Data", "ItemSource.xml"));
			ItemFilter.Save(Path.Combine("Data", "ItemFilter.xml"));
#else
            //this is handy for debugging
            foreach (Item item in AllItems)
            {
                try
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Item));
                    StringBuilder sb = new StringBuilder();
                    System.IO.StringWriter writer = new System.IO.StringWriter(sb);
                    serializer.Serialize(writer, item);
                    writer.Close();
                    System.IO.File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "ItemCache-"+item.Name+".xml"), sb.ToString());

                }
                catch (System.Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
#endif
        }

		public void Load()
		{
			_items = new SortedDictionary<int, Item>();
			List<Item> listItems = new List<Item>();
			if (File.Exists(ItemCache.SavedFilePath)) {
				try {
					string xml = System.IO.File.ReadAllText(ItemCache.SavedFilePath).Replace("/images/icons/", "");
					xml = xml.Replace("<Slot>Weapon</Slot", "<Slot>TwoHand</Slot>").Replace("<Slot>Idol</Slot", "<Slot>Ranged</Slot>").Replace("<Slot>Robe</Slot", "<Slot>Chest</Slot>");
					System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ItemList));
					System.IO.StringReader reader = new System.IO.StringReader(xml);
					listItems = (List<Item>)serializer.Deserialize(reader);
					reader.Close();
				} catch (Exception) {
					Log.Show("Rawr was unable to load the Item Cache. It appears to have been made with a previous incompatible version of Rawr. Please use the ItemCache included with this version of Rawr to start from.");
				}
			}
			foreach (Item item in listItems) {
				//item.Stats.ConvertStatsToWotLKEquivalents();
                //item.Sockets.Stats.ConvertStatsToWotLKEquivalents();
				//if (item.Type == ItemType.Leather) UpdateArmorFromWowhead(item);
				AddItem(item, false);
			}

			LocationFactory.Load(Path.Combine("Data", "ItemSource.xml"));
			ItemFilter.Load(Path.Combine("Data", "ItemFilter.xml"));
			Calculations.ModelChanged += new EventHandler(Calculations_ModelChanged);
		}
#endif
		void Calculations_ModelChanged(object sender, EventArgs e)
		{
			_relevantItems = null;
        }

		//private void UpdateArmorFromWowhead(Item item)
		//{
		//    WebRequestWrapper wrw = new WebRequestWrapper();
		//    string wowheadXml = wrw.DownloadText(string.Format("http://wotlk.wowhead.com/?item={0}&xml", item.Id));
		//    wowheadXml = wowheadXml.Substring(0, wowheadXml.LastIndexOf(" Armor<"));
		//    wowheadXml = wowheadXml.Substring(wowheadXml.LastIndexOf('>')+1);
		//    item.Stats.Armor = int.Parse(wowheadXml);
		//}
	}
}
