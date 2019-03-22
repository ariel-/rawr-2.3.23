using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace Rawr
{
    public partial class ItemComparison : UserControl
    {
		private CharacterSlot _characterSlot = CharacterSlot.None;
        public ComparisonGraph.ComparisonSort Sort
        {
            get
            {
                return comparisonGraph1.Sort;
            }
            set
            {
                comparisonGraph1.Sort = value;
				if (_characterSlot != CharacterSlot.None)
					LoadGearBySlot(_characterSlot);
            }
        }

        private Character _character;
        public Character Character
        {
            get
            {
                return _character;
            }
            set
            {
                _character = value;
                comparisonGraph1.Character = _character;
            }
		}

		public ComparisonGraph ComparisonGraph { get { return comparisonGraph1; } }

        public ItemComparison()
        {
            InitializeComponent();
        }

		private int _calculationCount = 0;
		private ComparisonCalculationBase[] _itemCalculations = null;
		private AutoResetEvent _autoResetEvent = null;
        public void LoadGearBySlot(CharacterSlot slot) { LoadGearBySlot(slot, ItemSlot.None); }
        public void LoadGearBySlot(CharacterSlot slot, ItemSlot gemColour)
		{
			Calculations.ClearCache();
            //_itemCalculations = new List<ComparisonCalculationBase>();
			_characterSlot = slot;
			bool useMultithreading = Calculations.SupportsMultithreading && Rawr.Properties.GeneralSettings.Default.UseMultithreading;
			bool presorted = false;
            if (Character != null)
            {
                if ((int)slot >= 0 && (int)slot <= 20)
                { //Normal Gear Slots
					presorted = true;
                    bool seenEquippedItem = (Character[slot] == null);
					
					List<ItemInstance> relevantItemInstances = Character.GetRelevantItemInstances(slot);
					_itemCalculations = new ComparisonCalculationBase[relevantItemInstances.Count];
					_calculationCount = 0;
					_autoResetEvent = new AutoResetEvent(!useMultithreading);
					DateTime before = DateTime.Now;
                    if (relevantItemInstances.Count > 0)
                    {
                        foreach (ItemInstance item in relevantItemInstances)
                        {
                            if (!seenEquippedItem && Character[slot].Equals(item)) seenEquippedItem = true;
                            //Trace.WriteLine("Queuing WorkItem for item: " + item.ToString());
                            //Queue each item into the ThreadPool
                            if (useMultithreading)
                                ThreadPool.QueueUserWorkItem(GetItemInstanceCalculations, item);
                            else
                                GetItemInstanceCalculations(item);
                        }
                        //Wait for all items to be processed
                        _autoResetEvent.WaitOne();
                        //Trace.WriteLine(DateTime.Now.Subtract(before).Ticks);
                        //Trace.WriteLine("Finished all Calculations");
                    }

                    // add item
					List<ComparisonCalculationBase> listItemCalculations = new List<ComparisonCalculationBase>(_itemCalculations);
                    if (!seenEquippedItem)
                        listItemCalculations.Add(Calculations.GetItemCalculations(Character[slot], Character, slot));

					_itemCalculations = FilterTopXGemmings(listItemCalculations);
                }
                else
                { //Gems/Metas
                    Character.ClearRelevantGems(); // we need to reset relevant items for gems to allow colour selection
					List<Item> relevantItems = Character.GetRelevantItems(slot, gemColour);
					_itemCalculations = new ComparisonCalculationBase[relevantItems.Count];
					_calculationCount = 0;
                    if (relevantItems.Count > 0)
                    {
                        _autoResetEvent = new AutoResetEvent(!useMultithreading);
                        //DateTime before = DateTime.Now;
                        foreach (Item item in relevantItems)
                        {
                                if (useMultithreading)
                                    ThreadPool.QueueUserWorkItem(GetItemCalculations, item);
                                else
                                    GetItemCalculations(item);
                        }
                        //Wait for all items to be processed
                        _autoResetEvent.WaitOne();
                        //Trace.WriteLine(DateTime.Now.Subtract(before).Ticks);
                    }
                }
            }

            comparisonGraph1.RoundValues = true;
            comparisonGraph1.CustomRendered = false;
			if (presorted)
				comparisonGraph1.LoadItemCalculationsPreSorted(_itemCalculations);
			else
				comparisonGraph1.ItemCalculations = _itemCalculations;
            comparisonGraph1.EquipSlot = slot == CharacterSlot.Gems || slot == CharacterSlot.Metas ?
				CharacterSlot.None : slot;
        }

        // takes a list of items and returns a sorted filtered array of the top X gemmings
        private ComparisonCalculationBase[] FilterTopXGemmings(List<ComparisonCalculationBase> listItemCalculations)
        {
            List<ComparisonCalculationBase> filteredItemCalculations = new List<ComparisonCalculationBase>();
            int maxGemmings = Properties.GeneralSettings.Default.CountGemmingsShown;
            listItemCalculations.Sort(new System.Comparison<ComparisonCalculationBase>(comparisonGraph1.CompareItemCalculations));
            Dictionary<int, int> countItem = new Dictionary<int, int>();
            foreach (ComparisonCalculationBase itemCalculation in listItemCalculations)
            {
                int itemId = itemCalculation.ItemInstance.Id;
                if (!countItem.ContainsKey(itemId)) countItem.Add(itemId, 0);
                if (countItem[itemId]++ < maxGemmings ||
                    itemCalculation.Equipped || itemCalculation.ItemInstance.ForceDisplay)
                {
                    //Debug.Print("Itemid: " + itemId + " Instances of that item : " + countItem[itemId]);
                    filteredItemCalculations.Add(itemCalculation);
                }
            }
            return filteredItemCalculations.ToArray();
        }

		private void GetItemInstanceCalculations(object item)
		{
			//Trace.WriteLine("Starting Calculation for: " + item.ToString());
			ComparisonCalculationBase result = Calculations.GetItemCalculations((ItemInstance)item, Character, _characterSlot);
			_itemCalculations[Interlocked.Increment(ref _calculationCount) - 1] = result;
			if (_calculationCount == _itemCalculations.Length) _autoResetEvent.Set();
			//Trace.WriteLine("Finished Calculation for: " + item.ToString());
		}

		private void GetItemCalculations(object item)
		{
			//Trace.WriteLine("Starting Calculation for: " + item.ToString());
			ComparisonCalculationBase result = Calculations.GetItemCalculations((Item)item, Character, _characterSlot);
			_itemCalculations[Interlocked.Increment(ref _calculationCount) - 1] = result;
			if (_calculationCount == _itemCalculations.Length) _autoResetEvent.Set();
			//Trace.WriteLine("Finished Calculation for: " + item.ToString());
		}

        public void LoadEnchantsBySlot(ItemSlot slot, CharacterCalculationsBase currentCalculations)
        {
            if (Character != null)
            {
                comparisonGraph1.RoundValues = true;
                comparisonGraph1.CustomRendered = false;
                comparisonGraph1.ItemCalculations = Calculations.GetEnchantCalculations(slot, Character, currentCalculations, false).ToArray();
				comparisonGraph1.EquipSlot = CharacterSlot.None;
				_characterSlot = CharacterSlot.None;
            }
        }

        public void LoadBuffs(CharacterCalculationsBase currentCalculations, string filter)
        {
            if (Character != null)
            {
                comparisonGraph1.RoundValues = true;
                comparisonGraph1.CustomRendered = false;
                comparisonGraph1.ItemCalculations = Calculations.GetBuffCalculations(Character, currentCalculations, filter).ToArray();
				comparisonGraph1.EquipSlot = CharacterSlot.None;
				_characterSlot = CharacterSlot.None;
            }
        }

        /// <summary>
        /// Direct Upgrades
        /// </summary>
        /// <param name="currentCalculations"></param>
        /// <param name="divideByCost">When true it only includes items with positive cost and shows upgrade value / cost</param>
        public void LoadAvailableGear(CharacterCalculationsBase currentCalculations, bool divideByCost)
        {
            List<ComparisonCalculationBase> itemCalculations = new List<ComparisonCalculationBase>();
            SortedList<ItemSlot, CharacterSlot> slotMap = new SortedList<ItemSlot, CharacterSlot>();
            if (Character != null)
            {
                SortedList<string, ItemInstance> items = new SortedList<string, ItemInstance>();

                float Finger1 = (Character[CharacterSlot.Finger1] == null ? 0 : Calculations.GetItemCalculations(
                    Character[CharacterSlot.Finger1], Character, CharacterSlot.Finger1).OverallPoints);
                float Finger2 = (Character[CharacterSlot.Finger2] == null ? 0 : Calculations.GetItemCalculations(
                    Character[CharacterSlot.Finger2], Character, CharacterSlot.Finger2).OverallPoints);

                float Trinket1 = (Character[CharacterSlot.Trinket1] == null ? 0 : Calculations.GetItemCalculations(
                    Character[CharacterSlot.Trinket1], Character, CharacterSlot.Trinket1).OverallPoints);
                float Trinket2 = (Character[CharacterSlot.Trinket2] == null ? 0 : Calculations.GetItemCalculations(
                    Character[CharacterSlot.Trinket2], Character, CharacterSlot.Trinket2).OverallPoints);

                if (Finger2 < Finger1)
                {
                    slotMap[ItemSlot.Finger] = CharacterSlot.Finger2;
                }

                if (Trinket2 < Trinket1)
                {
                    slotMap[ItemSlot.Trinket] = CharacterSlot.Trinket2;
                }

                float MainHand = (Character[CharacterSlot.MainHand] == null ? 0 : Calculations.GetItemCalculations(
                    Character[CharacterSlot.MainHand], Character, CharacterSlot.MainHand).OverallPoints);
                float OffHand = (Character[CharacterSlot.OffHand] == null ? 0 : Calculations.GetItemCalculations(
                    Character[CharacterSlot.OffHand], Character, CharacterSlot.OffHand).OverallPoints);

                if (MainHand > OffHand)
                {
                    slotMap[ItemSlot.OneHand] = CharacterSlot.OffHand;

                }

                foreach (KeyValuePair<ItemSlot, CharacterSlot> kvp in Item.DefaultSlotMap)
                {
                    try
                    {
                        ItemSlot iSlot = kvp.Key;
                        CharacterSlot slot;

                        if (slotMap.ContainsKey(iSlot))
                        {
                            slot = slotMap[iSlot];
                        }
                        else
                        {
                            slot = kvp.Value;
                        }
                        if (slot != CharacterSlot.None)
                        {
                            ComparisonCalculationBase slotCalc;
							ItemInstance currentItem = Character[slot];
							if (currentItem == null)
								slotCalc = Calculations.CreateNewComparisonCalculation();
							else
								slotCalc = Calculations.GetItemCalculations(currentItem, Character, slot);

                            foreach (ItemInstance item in Character.GetRelevantItemInstances(slot))
                            {
								if (!items.ContainsKey(item.GemmedId) && (currentItem == null || currentItem.GemmedId != item.GemmedId))
                                {
									if (currentItem != null && currentItem.Item.Unique)
                                    {
                                        CharacterSlot otherSlot = CharacterSlot.None;
                                        switch (slot)
                                        {
                                            case CharacterSlot.Finger1:
                                                otherSlot = CharacterSlot.Finger2;
                                                break;
                                            case CharacterSlot.Finger2:
                                                otherSlot = CharacterSlot.Finger1;
                                                break;
                                            case CharacterSlot.Trinket1:
                                                otherSlot = CharacterSlot.Trinket2;
                                                break;
                                            case CharacterSlot.Trinket2:
                                                otherSlot = CharacterSlot.Trinket1;
                                                break;
                                            case CharacterSlot.MainHand:
                                                otherSlot = CharacterSlot.OffHand;
                                                break;
                                            case CharacterSlot.OffHand:
                                                otherSlot = CharacterSlot.MainHand;
                                                break;
                                        }
                                        if (otherSlot != CharacterSlot.None && Character[otherSlot] != null && (Character[otherSlot].Id == item.Id || (item.Item.UniqueId != null && item.Item.UniqueId.Contains(Character[otherSlot].Id))))
                                        {
                                            continue;
                                        }

                                    }

                                    if (!divideByCost || item.Item.Cost > 0.0f)
                                    {
                                        ComparisonCalculationBase itemCalc = Calculations.GetItemCalculations(item, Character, slot);
                                        //bool include = false;
                                        //for (int i = 0; i < itemCalc.SubPoints.Length; i++)
                                        //{
                                        //    itemCalc.SubPoints[i] -= slotCalc.SubPoints[i];
                                        //    include |= itemCalc.SubPoints[i] > 0;
                                        //}
                                        //itemCalc.OverallPoints -= slotCalc.OverallPoints;
                                        //if ( itemCalc.OverallPoints > 0)
                                        //{
                                        //    itemCalculations.Add(itemCalc);
                                        //}

                                        float difference = itemCalc.OverallPoints - slotCalc.OverallPoints;
                                        if (difference > 0)
                                        {
                                            itemCalc.SubPoints = new float[itemCalc.SubPoints.Length];
                                            if (divideByCost)
                                            {
                                                itemCalc.OverallPoints = difference / item.Item.Cost;
                                            }
                                            else
                                            {
                                                itemCalc.OverallPoints = difference;
                                            }
                                            itemCalculations.Add(itemCalc);
                                        }
                                    }

                                    items[item.GemmedId] = item;
                                }
                            }
                        }

                    }
                    catch (Exception)
                    {
                    }
                }
            }
            comparisonGraph1.RoundValues = true;
            comparisonGraph1.CustomRendered = false;
            comparisonGraph1.DisplayMode = ComparisonGraph.GraphDisplayMode.Overall;
            comparisonGraph1.ItemCalculations = FilterTopXGemmings(itemCalculations);
            comparisonGraph1.EquipSlot = CharacterSlot.AutoSelect;
			comparisonGraph1.SlotMap = slotMap;
			_characterSlot = CharacterSlot.None;
        }

        public ComparisonGraph.GraphDisplayMode DisplayMode
        {
            get { return comparisonGraph1.DisplayMode; }
            set { comparisonGraph1.DisplayMode = value; }
        }

        public void LoadCurrentGearEnchantsBuffs(CharacterCalculationsBase currentCalculations)
        {
            List<ComparisonCalculationBase> itemCalculations = new List<ComparisonCalculationBase>();
            if (Character != null)
            {
                foreach (CharacterSlot slot in Enum.GetValues(typeof(CharacterSlot)))
                    if (Character[slot] != null)
                        itemCalculations.Add(Calculations.GetItemCalculations(Character[slot], Character, slot));

                foreach (ComparisonCalculationBase calc in Calculations.GetEnchantCalculations(ItemSlot.None, Character, currentCalculations, true))
                    itemCalculations.Add(calc);

                foreach (ComparisonCalculationBase calc in Calculations.GetBuffCalculations(Character, currentCalculations, "Current"))
                    itemCalculations.Add(calc);
            }

            comparisonGraph1.RoundValues = true;
            comparisonGraph1.CustomRendered = false;
            comparisonGraph1.ItemCalculations = itemCalculations.ToArray();
			comparisonGraph1.EquipSlot = CharacterSlot.None;
			_characterSlot = CharacterSlot.None;
        }

        public void LoadTalents()
        {
            List<ComparisonCalculationBase> talentCalculations = new List<ComparisonCalculationBase>();
            Character baseChar = Character.Clone();
            Character newChar = Character.Clone();
            CharacterCalculationsBase currentCalc;
            CharacterCalculationsBase newCalc;
            ComparisonCalculationBase compare;
            currentCalc = Calculations.GetCharacterCalculations(baseChar, null, false, true, false);
            foreach (PropertyInfo pi in baseChar.CurrentTalents.GetType().GetProperties())
            {
                TalentDataAttribute[] talentDatas = pi.GetCustomAttributes(typeof(TalentDataAttribute), true) as TalentDataAttribute[];
                int orig = 0;
                if (talentDatas.Length > 0)
                {
                    TalentDataAttribute talentData = talentDatas[0];
                    orig = baseChar.CurrentTalents.Data[talentData.Index];
                    if (talentData.MaxPoints == (int)pi.GetValue(baseChar.CurrentTalents, null)) {
                        newChar.CurrentTalents.Data[talentData.Index]--;
                        newCalc = Calculations.GetCharacterCalculations(newChar, null, false, true, false);
                        compare = Calculations.GetCharacterComparisonCalculations(newCalc, currentCalc, talentData.Name, talentData.MaxPoints == orig, orig != 0);
                    } else {
                        newChar.CurrentTalents.Data[talentData.Index]++;
                        newCalc = Calculations.GetCharacterCalculations(newChar, null, false, true, false);
                        compare = Calculations.GetCharacterComparisonCalculations(currentCalc, newCalc, talentData.Name, talentData.MaxPoints == orig, orig != 0);
                    }
                    string text = string.Format("Current Rank {0}/{1}\r\n\r\n", orig, talentData.MaxPoints);
                    if        (orig == 0) {
                        // We originally didn't have it, so first rank is next rank
                        text += "Next Rank:\r\n";
                        text += talentData.Description[0];
                    } else if (orig >= talentData.MaxPoints) {
                        // We originally were at max, so there isn't a next rank, just show the capped one
                        text += talentData.Description[talentData.MaxPoints - 1];
                    } else {
                        // We aren't at 0 or MaxPoints originally, so it's just a point in between
                        text += talentData.Description[orig - 1];
                        text += "\r\n\r\nNext Rank:\r\n";
                        text += talentData.Description[orig];
                    }
                    compare.Description = text;
                    compare.Item = null;
                    talentCalculations.Add(compare);
                    newChar.CurrentTalents.Data[talentData.Index] = orig;
                }
            }
            comparisonGraph1.RoundValues = true;
            comparisonGraph1.CustomRendered = false;
            comparisonGraph1.ItemCalculations = talentCalculations.ToArray();
			comparisonGraph1.EquipSlot = CharacterSlot.None;
			_characterSlot = CharacterSlot.None;
        }

        public void LoadGlyphs()
        {
            List<ComparisonCalculationBase> glyphCalculations = new List<ComparisonCalculationBase>();
            Character baseChar = Character.Clone();
            Character newChar = Character.Clone();
            CharacterCalculationsBase currentCalc;
            CharacterCalculationsBase newCalc;
            ComparisonCalculationBase compare;
            bool orig;
            List<string> relevant = Calculations.GetModel(Character.CurrentModel).GetRelevantGlyphs();
            currentCalc = Calculations.GetCharacterCalculations(baseChar, null, false, true, false);
            foreach (PropertyInfo pi in baseChar.CurrentTalents.GetType().GetProperties())
            {
                GlyphDataAttribute[] glyphDatas = pi.GetCustomAttributes(typeof(GlyphDataAttribute), true) as GlyphDataAttribute[];
                if (glyphDatas.Length > 0)
                {
                    GlyphDataAttribute glyphData = glyphDatas[0];
                    if (relevant == null || relevant.Contains(glyphData.Name))
                    {
                        orig = baseChar.CurrentTalents.GlyphData[glyphData.Index];
                        if (orig)
                        {
                            newChar.CurrentTalents.GlyphData[glyphData.Index] = false;
                            newCalc = Calculations.GetCharacterCalculations(newChar, null, false, true, false);
                            compare = Calculations.GetCharacterComparisonCalculations(newCalc, currentCalc, glyphData.Name, orig, false);
                        }
                        else
                        {
                            newChar.CurrentTalents.GlyphData[glyphData.Index] = true;
                            newCalc = Calculations.GetCharacterCalculations(newChar, null, false, true, false);
                            compare = Calculations.GetCharacterComparisonCalculations(currentCalc, newCalc, glyphData.Name, orig, false);
                        }
                        compare.Description = glyphData.Description; // JOTHAY: WTB Tooltips that show info on these charts
                        compare.Item = null;
                        glyphCalculations.Add(compare);
                        newChar.CurrentTalents.GlyphData[glyphData.Index] = orig;
                    }
                }
            }

            comparisonGraph1.RoundValues = true;
            comparisonGraph1.CustomRendered = false;
            comparisonGraph1.ItemCalculations = glyphCalculations.ToArray();
            comparisonGraph1.EquipSlot = CharacterSlot.None;
            _characterSlot = CharacterSlot.None;
        }

        public void LoadTalentSpecs(TalentPicker picker)
        {
            List<ComparisonCalculationBase> talentCalculations = new List<ComparisonCalculationBase>();
            if (Character != null)
            {
                Character baseChar = Character.Clone();
                switch (baseChar.Class)
                {
                    case CharacterClass.Warrior: baseChar.WarriorTalents = new WarriorTalents(); break;
                    case CharacterClass.Paladin: baseChar.PaladinTalents = new PaladinTalents(); break;
                    case CharacterClass.Hunter: baseChar.HunterTalents = new HunterTalents(); break;
                    case CharacterClass.Rogue: baseChar.RogueTalents = new RogueTalents(); break;
                    case CharacterClass.Priest: baseChar.PriestTalents = new PriestTalents(); break;
                    case CharacterClass.Shaman: baseChar.ShamanTalents = new ShamanTalents(); break;
                    case CharacterClass.Mage: baseChar.MageTalents = new MageTalents(); break;
                    case CharacterClass.Warlock: baseChar.WarlockTalents = new WarlockTalents(); break;
                    case CharacterClass.Druid: baseChar.DruidTalents = new DruidTalents(); break;
                    case CharacterClass.DeathKnight: baseChar.DeathKnightTalents = new DeathKnightTalents(); break;
                    default: baseChar.DruidTalents = new DruidTalents(); break;
                }
                CharacterCalculationsBase baseCalc = Calculations.GetCharacterCalculations(baseChar, null, false, true, false);

                ComparisonCalculationBase compare;
                Character newChar;
                foreach (SavedTalentSpec spec in picker.SpecsFor(Character.Class))
                {
                    newChar = Character.Clone();
                    newChar.CurrentTalents = spec.TalentSpec();
                    compare = Calculations.GetCharacterComparisonCalculations(baseCalc, newChar, spec.ToString(), spec == picker.CurrentSpec());
                    compare.Item = null;
                    talentCalculations.Add(compare);
                }
            }

            comparisonGraph1.RoundValues = true;
            comparisonGraph1.CustomRendered = false;
            comparisonGraph1.ItemCalculations = talentCalculations.ToArray();
			comparisonGraph1.EquipSlot = CharacterSlot.None;
			_characterSlot = CharacterSlot.None;
        }

        public void LoadRelativeStatValues()
        {
            comparisonGraph1.RoundValues = true;
            comparisonGraph1.CustomRendered = false;
            comparisonGraph1.ItemCalculations = CalculationsBase.GetRelativeStatValues(Character);
			comparisonGraph1.EquipSlot = CharacterSlot.None;
			_characterSlot = CharacterSlot.None;
        }

        public void LoadCustomChart(string chartName)
        {
            comparisonGraph1.RoundValues = true;
            comparisonGraph1.CustomRendered = false;
            comparisonGraph1.ItemCalculations = Calculations.GetCustomChartData(Character, chartName);
			comparisonGraph1.EquipSlot = CharacterSlot.None;
			_characterSlot = CharacterSlot.None;
        }

        public void LoadCustomRenderedChart(string chartName)
        {
            comparisonGraph1.RoundValues = true;
            comparisonGraph1.CustomRendered = true;
            comparisonGraph1.CustomRenderedChartName = chartName;
            comparisonGraph1.ItemCalculations = null;
			comparisonGraph1.EquipSlot = CharacterSlot.None;
			_characterSlot = CharacterSlot.None;
        }
	}
}
