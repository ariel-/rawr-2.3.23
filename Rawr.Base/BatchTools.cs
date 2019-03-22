using System;
using System.Collections.Generic;
using System.Text;
using Rawr.Optimizer;
using System.ComponentModel;
using System.IO;

namespace Rawr
{
    public class BatchTools : INotifyPropertyChanged
    {
        ItemInstanceOptimizer optimizer;
        BatchOptimizer batchOptimizer;

        public bool IsBusy
        {
            get
            {
                return optimizer.IsBusy || (batchOptimizer != null && batchOptimizer.IsBusy);
            }
        }

        private enum AsyncOperation
        {
            None,
            Optimize,
            BuildUpgradeList,
            BatchOptimize,
            BuildBatchUpgradeList,
            ProgressiveOptimize,
            BuildProgressiveUpgradeList
        }

        AsyncOperation currentOperation;
        int batchIndex;
        AvailableItemGenerator itemGenerator;
        Item[] itemList;
        int itemIndex;
        ItemInstance optimizedItemInstance;
        Character workingCharacter;

        // optimize state
        int optimizerRound;
        Character bestRoundCharacter;
        float bestRoundValue;

        // build upgrade list state
        int upgradeListPhase;
        private class UpgradeEntry
        {
            public ItemInstance Item { get; set; }
            public float Value { get; set; }
            private List<float> valueList = new List<float>();
            public List<float> ValueList
            {
                get
                {
                    return valueList;
                }
            }
        }
        Dictionary<CharacterSlot, Dictionary<string, UpgradeEntry>> upgradeList;
        IEnumerator<UpgradeEntry> upgradeListEnumerator;

        IEnumerator<UpgradeEntry> GetUpgradeListEnumerator()
        {
            foreach (KeyValuePair<CharacterSlot, Dictionary<string, UpgradeEntry>> kvp in upgradeList)
            {
                foreach (UpgradeEntry entry in kvp.Value.Values)
                {
                    yield return entry;
                }
            }
        }

        private BatchCharacterList batchCharacterList;
        public BatchCharacterList BatchCharacterList 
        {
            get
            {
                return batchCharacterList;
            }
            set
            {
                batchCharacterList = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("BatchCharacterList"));
            }
        }

        private BatchCharacter CurrentBatchCharacter
        {
            get
            {
                if (batchIndex < 0 || batchIndex >= BatchCharacterList.Count) return null;
                return BatchCharacterList[batchIndex];
            }
        }

        private bool batchListReady = false;

        public BatchTools()
        {
            MaxRounds = 3;
            BatchCharacterList = new BatchCharacterList();
            batchListReady = true;

            optimizer = new ItemInstanceOptimizer();
            optimizer.OptimizeCharacterProgressChanged += new OptimizeCharacterProgressChangedEventHandler(_optimizer_OptimizeCharacterProgressChanged);
            optimizer.OptimizeCharacterCompleted += new OptimizeCharacterCompletedEventHandler(_optimizer_OptimizeCharacterCompleted);
            optimizer.ComputeUpgradesProgressChanged += new ComputeUpgradesProgressChangedEventHandler(_optimizer_ComputeUpgradesProgressChanged);
            optimizer.ComputeUpgradesCompleted += new ComputeUpgradesCompletedEventHandler(_optimizer_ComputeUpgradesCompleted);
            optimizer.EvaluateUpgradeProgressChanged += new ProgressChangedEventHandler(_optimizer_EvaluateUpgradeProgressChanged);
            optimizer.EvaluateUpgradeCompleted += new EvaluateUpgradeCompletedEventHandler(_optimizer_EvaluateUpgradeCompleted);
        }

        public Item SingleItemUpgrade { get; set; }

        public Item GetSingleItemUpgrade(string itemIdOrName)
        {
            if (string.IsNullOrEmpty(itemIdOrName))
            {
                return null;
            }
            else
            {
                int id;
                if (int.TryParse(itemIdOrName, out id))
                {
                    return ItemCache.FindItemById(id);
                }
                else
                {
                    // try to match by name
                    foreach (Item item in ItemCache.AllItems)
                    {
                        if (item.Name == itemIdOrName)
                        {
                            return item;
                        }
                    }
                }
                return null;
            }
        }

        public event EventHandler StatusUpdated;
        public event EventHandler OperationCompleted;
        public event EventHandler UpgradeListCompleted;

        public string Status { get; private set; }
        public int Progress { get; private set; }
        public string[] CustomSubpoints { get; private set; }
        public Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> Upgrades { get; private set; }

        public void UpdateStatusLabel()
        {
            if (batchListReady)
            {
                Status = string.Format("{0} / {1}", BatchCharacterList.Score, BatchCharacterList.NewScore);
                Progress = 0;
                if (StatusUpdated != null)
                {
                    StatusUpdated(this, EventArgs.Empty);
                }
            }
        }

        void _optimizer_OptimizeCharacterProgressChanged(object sender, OptimizeCharacterProgressChangedEventArgs e)
        {
            switch (currentOperation)
            {
                case AsyncOperation.Optimize:
                case AsyncOperation.ProgressiveOptimize:
                    Status = string.Format("[{2}/{3}] {0}: {1}", CurrentBatchCharacter.Name, e.BestValue, batchIndex + 1, BatchCharacterList.Count);
                    Progress = e.ProgressPercentage;
                    if (StatusUpdated != null)
                    {
                        StatusUpdated(this, EventArgs.Empty);
                    }
                    break;
                case AsyncOperation.BuildProgressiveUpgradeList:
                    Status = string.Format("[{4}/{5}] {1}: [{2}/{3}] {0}", CurrentBatchCharacter.Name, itemList[itemIndex].Name, batchIndex + 1, BatchCharacterList.Count, itemIndex + 1, itemList.Length);
                    Progress = e.ProgressPercentage;
                    if (StatusUpdated != null)
                    {
                        StatusUpdated(this, EventArgs.Empty);
                    }
                    break;
            }
        }

        public int MaxRounds { get; set; }
        public int Thoroughness { get; set; }
        public bool OverrideRegem { get; set; }
        public bool OverrideReenchant { get; set; }
        public bool TemplateGemsEnabled { get; set; }
        public OptimizationMethod OptimizationMethod {get;set;}
        public GreedyOptimizationMethod GreedyOptimizationMethod { get; set; }

        void _optimizer_OptimizeCharacterCompleted(object sender, OptimizeCharacterCompletedEventArgs e)
        {
            int maxRounds = MaxRounds;

            switch (currentOperation)
            {
                case AsyncOperation.Optimize:
                    {
                        if (e.Cancelled || e.Error != null)
                        {
                            currentOperation = AsyncOperation.None;
                            UpdateStatusLabel();
                            if (OperationCompleted != null)
                            {
                                OperationCompleted(this, EventArgs.Empty);
                            }
                            break;
                        }
                        if (e.OptimizedCharacterValue > e.CurrentCharacterValue + 0.00001f)
                        {
                            Character _character = CurrentBatchCharacter.Character;
                            Character bestCharacter = e.OptimizedCharacter;
                            _character.SetItems(bestCharacter);

                            //CurrentBatchCharacter.UnsavedChanges = true;
                            //CurrentBatchCharacter.NewScore = e.OptimizedCharacterValue;
                            //CurrentBatchCharacter.NewScore = ItemInstanceOptimizer.GetOptimizationValue(_character, CurrentBatchCharacter.Model); // on item change always evaluate with equipped gear first (needed by mage module to store incremental data)

                            optimizerRound = 0;
                        }
                        else if (Math.Abs(e.OptimizedCharacterValue - e.CurrentCharacterValue) < 0.00001f && !e.CurrentCharacterInjected)
                        {
                            optimizerRound = maxRounds;
                        }
                        else
                        {
                            optimizerRound++;
                        }
                        if (optimizerRound >= maxRounds)
                        {
                            do
                            {
                                batchIndex++;
                            } while (batchIndex < BatchCharacterList.Count && CurrentBatchCharacter.Character == null);
                            optimizerRound = 0;
                        }

                        if (batchIndex < BatchCharacterList.Count)
                        {
                            OptimizeCurrentBatchCharacter();
                        }
                        else
                        {
                            currentOperation = AsyncOperation.None;
                            UpdateStatusLabel();
                            if (OperationCompleted != null)
                            {
                                OperationCompleted(this, EventArgs.Empty);
                            }
                        }
                    }
                    break;
                case AsyncOperation.ProgressiveOptimize:
                    {
                        if (e.Cancelled || e.Error != null)
                        {
                            bestRoundCharacter = null;
                            currentOperation = AsyncOperation.None;
                            UpdateStatusLabel();
                            if (OperationCompleted != null)
                            {
                                OperationCompleted(this, EventArgs.Empty);
                            }
                            break;
                        }
                        // try to find at least same value, although sometimes it's possible it doesn't exist
                        // anymore due to restrictions
                        if (e.OptimizedCharacterValue > bestRoundValue + 0.00001f)
                        {
                            bestRoundCharacter = e.OptimizedCharacter;
                            bestRoundValue = e.OptimizedCharacterValue;
                        }

                        if (e.OptimizedCharacterValue > e.CurrentCharacterValue + 0.00001f)
                        {
                            Character _character = CurrentBatchCharacter.Character;
                            Character bestCharacter = e.OptimizedCharacter;
                            _character.SetItems(bestCharacter);

                            optimizerRound = 0;
                        }
                        else if (Math.Abs(e.OptimizedCharacterValue - e.CurrentCharacterValue) < 0.00001f && !e.CurrentCharacterInjected)
                        {
                            optimizerRound = maxRounds;
                        }
                        else
                        {
                            optimizerRound++;
                        }
                        if (optimizerRound >= maxRounds)
                        {
                            // we have to use the best one we get, even if it's lower value than what we had before
                            Character _character = CurrentBatchCharacter.Character;
                            // verify if it's actually different so we don't make it dirty when not necessary
                            bool dirty = false;
                            for (int slot = 0; slot < Character.OptimizableSlotCount; slot++)
                            {
                                if (slot != (int)CharacterSlot.OffHand || bestRoundCharacter.CurrentCalculations.IncludeOffHandInCalculations(bestRoundCharacter))
                                {
                                    if (slot == (int)CharacterSlot.Finger1 || slot == (int)CharacterSlot.Trinket1)
                                    {
                                        // ignore if we have 1/2 swap
                                        if (!((_character._item[slot] == bestRoundCharacter._item[slot] && _character._item[slot + 1] == bestRoundCharacter._item[slot + 1]) ||
                                            (_character._item[slot] == bestRoundCharacter._item[slot + 1] && _character._item[slot + 1] == bestRoundCharacter._item[slot])))
                                        {
                                            dirty = true;
                                            break;
                                        }
                                        slot++;
                                    }
                                    else
                                    {
                                        if (_character._item[slot] != bestRoundCharacter._item[slot])
                                        {
                                            dirty = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (dirty)
                            {
                                _character.SetItems(bestRoundCharacter);
                            }
                            // we have to perform item restrictions on the active item generator
                            // so that we don't regem/reenchant already used items
                            itemGenerator.AddItemRestrictions(bestRoundCharacter);
                            // move to next batch character
                            do
                            {
                                batchIndex++;
                            } while (batchIndex < BatchCharacterList.Count && (CurrentBatchCharacter.Character == null || CurrentBatchCharacter.Locked));
                            bestRoundCharacter = null;
                            bestRoundValue = float.NegativeInfinity;
                            optimizerRound = 0;
                        }

                        if (batchIndex < BatchCharacterList.Count)
                        {
                            int _thoroughness = Thoroughness;
                            Character character = CurrentBatchCharacter.Character.Clone();
                            // regularize character with current item restrictions
                            itemGenerator.RegularizeCharacter(character);
                            optimizer.InitializeItemCache(character, CurrentBatchCharacter.Model, itemGenerator);
                            optimizer.OptimizeCharacterAsync(character, _thoroughness, true);
                        }
                        else
                        {
                            bestRoundCharacter = null;
                            currentOperation = AsyncOperation.None;
                            UpdateStatusLabel();
                            if (OperationCompleted != null)
                            {
                                OperationCompleted(this, EventArgs.Empty);
                            }
                        }
                    }
                    break;
                case AsyncOperation.BuildProgressiveUpgradeList:
                    {
                        if (e.Cancelled || e.Error != null)
                        {
                            currentOperation = AsyncOperation.None;
                            UpdateStatusLabel();
                            if (OperationCompleted != null)
                            {
                                OperationCompleted(this, EventArgs.Empty);
                            }
                            break;
                        }
                        // we optimized the character with new item restrictions
                        // update the working character and start upgrade evaluation
                        workingCharacter = e.OptimizedCharacter;
                        int _thoroughness = Thoroughness;
                        // evaluate what we get by using the optimized item
                        optimizer.EvaluateUpgradeAsync(workingCharacter, _thoroughness, optimizedItemInstance);
                    }
                    break;
            }
        }

        void _optimizer_ComputeUpgradesProgressChanged(object sender, ComputeUpgradesProgressChangedEventArgs e)
        {
            switch (currentOperation)
            {
                case AsyncOperation.BuildUpgradeList:
                    Status = string.Format("[{2}/{3}] {0}: {1}", CurrentBatchCharacter.Name, e.CurrentItem, batchIndex + 1, BatchCharacterList.Count);
                    Progress = e.ProgressPercentage;
                    if (StatusUpdated != null)
                    {
                        StatusUpdated(this, EventArgs.Empty);
                    }
                    break;
                case AsyncOperation.BuildProgressiveUpgradeList:
                    Status = string.Format("[{4}/{5}] {1}: [{2}/{3}] {0}", CurrentBatchCharacter.Name, e.CurrentItem, batchIndex + 1, BatchCharacterList.Count, itemIndex + 1, itemList.Length);
                    Progress = e.ItemProgressPercentage;
                    if (StatusUpdated != null)
                    {
                        StatusUpdated(this, EventArgs.Empty);
                    }
                    break;
            }
        }

        void _optimizer_ComputeUpgradesCompleted(object sender, ComputeUpgradesCompletedEventArgs e)
        {
            switch (currentOperation)
            {
                case AsyncOperation.BuildUpgradeList:
                    if (e.Cancelled || e.Error != null)
                    {
                        currentOperation = AsyncOperation.None;
                        UpdateStatusLabel();
                        if (OperationCompleted != null)
                        {
                            OperationCompleted(this, EventArgs.Empty);
                        }
                        break;
                    }
                    if (upgradeListPhase == 0)
                    {
                        foreach (KeyValuePair<CharacterSlot, List<ComparisonCalculationUpgrades>> kvp in e.Upgrades)
                        {
                            Dictionary<string, UpgradeEntry> map;
                            if (!upgradeList.TryGetValue(kvp.Key, out map))
                            {
                                map = new Dictionary<string, UpgradeEntry>();
                                upgradeList[kvp.Key] = map;
                            }
                            foreach (ComparisonCalculationBase comp in kvp.Value)
                            {
                                string key = comp.ItemInstance.GemmedId;
                                UpgradeEntry upgradeEntry;
                                if (!map.TryGetValue(key, out upgradeEntry))
                                {
                                    upgradeEntry = new UpgradeEntry();
                                    upgradeEntry.Item = comp.ItemInstance;
                                    map[key] = upgradeEntry;
                                }
                            }
                        }
                        do
                        {
                            batchIndex++;
                        } while (batchIndex < BatchCharacterList.Count && CurrentBatchCharacter.Character == null);
                        if (batchIndex < BatchCharacterList.Count)
                        {
                            ComputeUpgradesCurrentBatchCharacter();
                        }
                        else
                        {
                            upgradeListPhase = 1;
                            batchIndex = 0;
                            upgradeListEnumerator = GetUpgradeListEnumerator();
                            if (upgradeListEnumerator.MoveNext())
                            {
                                EvaluateUpgradeCurrentBatchCharacter(true);
                            }
                            else
                            {
                                // upgrade list is empty, abort
                                currentOperation = AsyncOperation.None;
                                UpdateStatusLabel();
                                if (OperationCompleted != null)
                                {
                                    OperationCompleted(this, EventArgs.Empty);
                                }
                                break;
                            }
                        }
                    }
                    break;
                case AsyncOperation.BuildProgressiveUpgradeList:
                    if (e.Cancelled || e.Error != null)
                    {
                        currentOperation = AsyncOperation.None;
                        UpdateStatusLabel();
                        if (OperationCompleted != null)
                        {
                            OperationCompleted(this, EventArgs.Empty);
                        }
                        break;
                    }
                    bool foundUpgrade = false;
                    foreach (KeyValuePair<CharacterSlot, List<ComparisonCalculationUpgrades>> kvp in e.Upgrades)
                    {
                        Dictionary<string, UpgradeEntry> map;
                        if (!upgradeList.TryGetValue(kvp.Key, out map))
                        {
                            map = new Dictionary<string, UpgradeEntry>();
                            upgradeList[kvp.Key] = map;
                        }
                        if (kvp.Value.Count > 0)
                        {
                            ComparisonCalculationUpgrades comp = kvp.Value[0];
                            upgradeListPhase = 1; // item was used, from now on we do evaluate upgrade on specific item instance only
                            optimizedItemInstance = comp.ItemInstance;
                            // make item restrictions based on best character
                            itemGenerator.AddItemRestrictions(comp.CharacterItems, workingCharacter.CurrentCalculations.IncludeOffHandInCalculations(workingCharacter));
                            foundUpgrade = true;
                            string key = comp.ItemInstance.Id.ToString();
                            UpgradeEntry upgradeEntry;
                            if (!map.TryGetValue(key, out upgradeEntry))
                            {
                                upgradeEntry = new UpgradeEntry();
                                map[key] = upgradeEntry;
                            }
                            upgradeEntry.Item = comp.ItemInstance;
                            upgradeEntry.Value += comp.OverallPoints * CurrentBatchCharacter.Weight;
                            upgradeEntry.ValueList.Add(comp.OverallPoints);
                            break;
                        }
                    }
                    if (!foundUpgrade)
                    {
                        // make item restrictions based on best character without using the item
                        itemGenerator.AddItemRestrictions(workingCharacter);
                        // if we're evaluating an item that was already marked available then we must restrict to that version
                        // if it was used in this character
                        Item item = itemList[itemIndex];
                        CharacterSlot slot = Character.GetCharacterSlotByItemSlot(item.Slot);
                        if (upgradeListPhase == 0)
                        {
                            foreach (CharacterSlot s in Character.CharacterSlots)
                            {
                                ItemInstance itemInstance = workingCharacter[s];
                                if ((object)itemInstance != null && itemInstance.Id == item.Id)
                                {
                                    upgradeListPhase = 1;
                                    optimizedItemInstance = itemInstance;
                                    break;
                                }
                            }
                        }
                        Dictionary<string, UpgradeEntry> map;
                        if (!upgradeList.TryGetValue(slot, out map))
                        {
                            map = new Dictionary<string, UpgradeEntry>();
                            upgradeList[slot] = map;
                        }
                        string key = itemList[itemIndex].Id.ToString();
                        UpgradeEntry upgradeEntry;
                        if (!map.TryGetValue(key, out upgradeEntry))
                        {
                            upgradeEntry = new UpgradeEntry();
                            map[key] = upgradeEntry;
                        }
                        if (upgradeListPhase == 1)
                        {
                            upgradeEntry.Item = optimizedItemInstance;
                        }
                        upgradeEntry.ValueList.Add(0.0f);
                    }
                    // move to next character
                    do
                    {
                        batchIndex++;
                    } while (batchIndex < BatchCharacterList.Count && CurrentBatchCharacter.Character == null);
                    if (batchIndex < BatchCharacterList.Count)
                    {
                        if (upgradeListPhase == 0)
                        {
                            // so far we haven't made any changes yet
                            // we're working under assumption that the starting batch is valid i.e. an item will have the same gemming in all characters
                            int _thoroughness = Thoroughness;
                            workingCharacter = CurrentBatchCharacter.Character;
                            optimizer.InitializeItemCache(workingCharacter, CurrentBatchCharacter.Model, itemGenerator);
                            optimizer.ComputeUpgradesAsync(workingCharacter, _thoroughness, itemList[itemIndex]);
                        }
                        else
                        {
                            // we made item restrictions, first we have to optimize character without the item
                            int _thoroughness = Thoroughness;
                            workingCharacter = CurrentBatchCharacter.Character.Clone();
                            // regularize character with current item restrictions
                            itemGenerator.RegularizeCharacter(workingCharacter);
                            optimizer.InitializeItemCache(workingCharacter, CurrentBatchCharacter.Model, itemGenerator);
                            optimizer.OptimizeCharacterAsync(workingCharacter, _thoroughness, true);
                        }
                    }
                    else
                    {
                        // we finished all characters for this item
                        // move to next item
                        itemIndex++;
                        if (itemIndex < itemList.Length)
                        {
                            batchIndex = 0;
                            upgradeListPhase = 0;
                            int _thoroughness = Thoroughness;
                            // we have to reinitialize item generator because of the restrictions we made
                            //CreateBatchItemGenerator();
                            itemGenerator.RestoreAvailabilityInformation();
                            optimizer.InitializeItemCache(CurrentBatchCharacter.Character, CurrentBatchCharacter.Model, itemGenerator);
                            workingCharacter = CurrentBatchCharacter.Character;
                            optimizer.ComputeUpgradesAsync(CurrentBatchCharacter.Character, _thoroughness, itemList[itemIndex]);
                        }
                        else
                        {
                            // we're done
                            WrapUpProgressiveUpgradeList();
                        }
                    }
                    break;
            }
        }

        void WrapUpProgressiveUpgradeList()
        {
            currentOperation = AsyncOperation.None;
            UpdateStatusLabel();
            if (OperationCompleted != null)
            {
                OperationCompleted(this, EventArgs.Empty);
            }

            float totalValue = 0f;
            foreach (BatchCharacter batchCharacter in BatchCharacterList)
            {
                if (batchCharacter.Character != null)
                {
                    totalValue += batchCharacter.Weight;
                }
            }

            Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> upgrades = new Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>>();

            foreach (var kvp in upgradeList)
            {
                upgrades[kvp.Key] = new List<ComparisonCalculationUpgrades>();
                Dictionary<int, UpgradeEntry> filtered = new Dictionary<int, UpgradeEntry>();
                foreach (UpgradeEntry entry in kvp.Value.Values)
                {
                    if (entry.Value > 0)
                    {
                        ComparisonCalculationUpgrades itemCalc = new ComparisonCalculationUpgrades();
                        itemCalc.ItemInstance = entry.Item;
                        itemCalc.CharacterItems = null;
                        itemCalc.Name = entry.Item.Item.Name;
                        itemCalc.Equipped = false;
                        itemCalc.OverallPoints = entry.Value / totalValue;
                        itemCalc.SubPoints = entry.ValueList.ToArray();

                        upgrades[kvp.Key].Add(itemCalc);
                    }
                }
            }
            List<string> customSubpoints = new List<string>();
            foreach (BatchCharacter batchCharacter in BatchCharacterList)
            {
                customSubpoints.Add(batchCharacter.Name);
            }
            CustomSubpoints = customSubpoints.ToArray();
            Upgrades = upgrades;
            if (UpgradeListCompleted != null)
            {
                UpgradeListCompleted(this, EventArgs.Empty);
            }
        }

        void _optimizer_EvaluateUpgradeProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (currentOperation)
            {
                case AsyncOperation.BuildUpgradeList:
                    Status = string.Format("[{2}/{3}] {0}: {1}", CurrentBatchCharacter.Name, upgradeListEnumerator.Current.Item.Item.Name, batchIndex + 1, BatchCharacterList.Count);
                    Progress = e.ProgressPercentage;
                    if (StatusUpdated != null)
                    {
                        StatusUpdated(this, EventArgs.Empty);
                    }
                    break;
                case AsyncOperation.BuildProgressiveUpgradeList:
                    Status = string.Format("[{4}/{5}] {1}: [{2}/{3}] {0}", CurrentBatchCharacter.Name, itemList[itemIndex].Name, batchIndex + 1, BatchCharacterList.Count, itemIndex + 1, itemList.Length);
                    Progress = e.ProgressPercentage;
                    if (StatusUpdated != null)
                    {
                        StatusUpdated(this, EventArgs.Empty);
                    }
                    break;
            }
        }

        void _optimizer_EvaluateUpgradeCompleted(object sender, EvaluateUpgradeCompletedEventArgs e)
        {
            switch (currentOperation)
            {
                case AsyncOperation.BuildUpgradeList:
                    upgradeListEnumerator.Current.Value += e.UpgradeValue * CurrentBatchCharacter.Weight;
                    upgradeListEnumerator.Current.ValueList.Add(e.UpgradeValue);
                    if (upgradeListEnumerator.MoveNext())
                    {
                        EvaluateUpgradeCurrentBatchCharacter(false);
                    }
                    else
                    {
                        do
                        {
                            batchIndex++;
                        } while (batchIndex < BatchCharacterList.Count && CurrentBatchCharacter.Character == null);
                        if (batchIndex < BatchCharacterList.Count)
                        {
                            upgradeListEnumerator = GetUpgradeListEnumerator();
                            upgradeListEnumerator.MoveNext();
                            EvaluateUpgradeCurrentBatchCharacter(true);
                        }
                        else
                        {
                            currentOperation = AsyncOperation.None;
                            UpdateStatusLabel();
                            if (OperationCompleted != null)
                            {
                                OperationCompleted(this, EventArgs.Empty);
                            }

                            float totalValue = 0f;
                            foreach (BatchCharacter batchCharacter in BatchCharacterList)
                            {
                                if (batchCharacter.Character != null)
                                {
                                    totalValue += batchCharacter.Weight;
                                }
                            }

                            Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> upgrades = new Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>>();

                            foreach (var kvp in upgradeList)
                            {
                                Dictionary<int, UpgradeEntry> filtered = new Dictionary<int, UpgradeEntry>();
                                foreach (UpgradeEntry entry in kvp.Value.Values)
                                {
                                    UpgradeEntry existingEntry;
                                    filtered.TryGetValue(entry.Item.Id, out existingEntry);
                                    if (entry.Value > 0 && (existingEntry == null || entry.Value > existingEntry.Value))
                                    {
                                        filtered[entry.Item.Id] = entry;
                                    }
                                }

                                upgrades[kvp.Key] = new List<ComparisonCalculationUpgrades>();
                                foreach (UpgradeEntry entry in filtered.Values)
                                {
                                    ComparisonCalculationUpgrades itemCalc = new ComparisonCalculationUpgrades();
                                    itemCalc.ItemInstance = entry.Item;
                                    itemCalc.CharacterItems = null;
                                    itemCalc.Name = entry.Item.Item.Name;
                                    itemCalc.Equipped = false;
                                    itemCalc.OverallPoints = entry.Value / totalValue;
                                    itemCalc.SubPoints = entry.ValueList.ToArray();

                                    upgrades[kvp.Key].Add(itemCalc);                                    
                                }
                            }
                            List<string> customSubpoints = new List<string>();
                            foreach (BatchCharacter batchCharacter in BatchCharacterList)
                            {
                                customSubpoints.Add(batchCharacter.Name);
                            }
                            Upgrades = upgrades;
                            CustomSubpoints = customSubpoints.ToArray();
                            if (UpgradeListCompleted != null)
                            {
                                UpgradeListCompleted(this, EventArgs.Empty);
                            }
                        }
                    }
                    break;
                case AsyncOperation.BuildProgressiveUpgradeList:
                    if (e.Cancelled || e.Error != null)
                    {
                        currentOperation = AsyncOperation.None;
                        UpdateStatusLabel();
                        if (OperationCompleted != null)
                        {
                            OperationCompleted(this, EventArgs.Empty);
                        }
                        break;
                    }
                    CharacterSlot slot = Character.GetCharacterSlotByItemSlot(optimizedItemInstance.Slot);
                    Dictionary<string, UpgradeEntry> map;
                    if (!upgradeList.TryGetValue(slot, out map))
                    {
                        map = new Dictionary<string, UpgradeEntry>();
                        upgradeList[slot] = map;
                    }
                    string key = optimizedItemInstance.Id.ToString();
                    UpgradeEntry upgradeEntry;
                    if (!map.TryGetValue(key, out upgradeEntry))
                    {
                        upgradeEntry = new UpgradeEntry();
                        upgradeEntry.Item = optimizedItemInstance;
                        map[key] = upgradeEntry;
                    }
                    if (e.UpgradeValue > 0)
                    {
                        // make item restrictions based on best character
                        itemGenerator.AddItemRestrictions(e.Upgrade.CharacterItems, workingCharacter.CurrentCalculations.IncludeOffHandInCalculations(workingCharacter));
                        upgradeEntry.Value += e.UpgradeValue * CurrentBatchCharacter.Weight;
                        upgradeEntry.ValueList.Add(e.UpgradeValue);
                    }
                    else
                    {
                        // make item restrictions based on best character without using the item
                        itemGenerator.AddItemRestrictions(workingCharacter);
                        upgradeEntry.ValueList.Add(0.0f);
                    }
                    // move to next character
                    do
                    {
                        batchIndex++;
                    } while (batchIndex < BatchCharacterList.Count && CurrentBatchCharacter.Character == null);
                    if (batchIndex < BatchCharacterList.Count)
                    {                        
                        // we made item restrictions, first we have to optimize character without the item
                        int _thoroughness = Thoroughness;
                        workingCharacter = CurrentBatchCharacter.Character.Clone();
                        // regularize character with current item restrictions
                        itemGenerator.RegularizeCharacter(workingCharacter);
                        optimizer.InitializeItemCache(workingCharacter, CurrentBatchCharacter.Model, itemGenerator);
                        optimizer.OptimizeCharacterAsync(workingCharacter, _thoroughness, true);
                    }
                    else
                    {
                        // we finished all characters for this item
                        // move to next item
                        itemIndex++;
                        if (itemIndex < itemList.Length)
                        {
                            batchIndex = 0;
                            upgradeListPhase = 0;
                            int _thoroughness = Thoroughness;
                            // we have to reinitialize item generator because of the restrictions we made
                            //CreateBatchItemGenerator();
                            itemGenerator.RestoreAvailabilityInformation();
                            optimizer.InitializeItemCache(CurrentBatchCharacter.Character, CurrentBatchCharacter.Model, itemGenerator);
                            workingCharacter = CurrentBatchCharacter.Character;
                            optimizer.ComputeUpgradesAsync(CurrentBatchCharacter.Character, _thoroughness, itemList[itemIndex]);
                        }
                        else
                        {
                            // we're done
                            WrapUpProgressiveUpgradeList();
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Sets available gear to all characters in BatchCharacterList.
        /// </summary>
        /// <param name="availableGear"></param>
        public void SetAvailableGear(List<string> availableGear)
        {
            foreach (BatchCharacter character in BatchCharacterList)
            {
                if (character.Character != null)
                {
                    character.Character.AvailableItems = new List<string>(availableGear);
                    character.UnsavedChanges = true;
                }
            }
        }

        /// <summary>
        /// For each character in BatchCharacterList replaces equipped unavailable gear with the closest available one.
        /// </summary>
        public void ReplaceUnavailableGear()
        {
            foreach (BatchCharacter character in BatchCharacterList)
            {
                if (character.Character != null)
                {
                    for (int slot = 0; slot < 19; slot++)
                    {
                        ItemInstance item = character.Character[(CharacterSlot)slot];
                        if (item != null)
                        {
                            string id = item.Id.ToString();
                            string anyGem = id + ".*.*.*";
                            string onlyGemmedId = string.Format("{0}.{1}.{2}.{3}", item.Id, item.Gem1Id, item.Gem2Id, item.Gem3Id);
                            string anyEnchant = string.Format("{0}.{1}.{2}.{3}.*", item.Id, item.Gem1Id, item.Gem2Id, item.Gem3Id);
                            List<string> list = character.Character.AvailableItems.FindAll(x => x.StartsWith(id, StringComparison.Ordinal));
                            List<string> sublist;
                            if (list.Contains(onlyGemmedId + ".*"))
                            {
                                // available
                            }
                            else if ((sublist = list.FindAll(x => x.StartsWith(onlyGemmedId, StringComparison.Ordinal))).Count > 0)
                            {
                                if (sublist.Contains(item.GemmedId))
                                {
                                    // available
                                }
                                else
                                {
                                    // have to replace enchant
                                    string s = sublist[0];
                                    character.Character.SetEnchantBySlot((CharacterSlot)slot, Enchant.FindEnchant(int.Parse(s.Substring(s.LastIndexOf('.') + 1)), item.Slot, character.Character));
                                    character.UnsavedChanges = true;
                                }
                            }
                            else if (list.Contains(id))
                            {
                                // available
                            }
                            else if ((sublist = list.FindAll(x => x.StartsWith(anyGem, StringComparison.Ordinal))).Count > 0)
                            {
                                if (sublist.Contains(anyGem + "." + item.EnchantId))
                                {
                                    // available
                                }
                                else
                                {
                                    // have to replace enchant
                                    string s = sublist[0];
                                    character.Character.SetEnchantBySlot((CharacterSlot)slot, Enchant.FindEnchant(int.Parse(s.Substring(s.LastIndexOf('.') + 1)), item.Slot, character.Character));
                                    character.UnsavedChanges = true;
                                }
                            }
                            else if (list.Count > 0)
                            {
                                string s = list[0];
                                if (s.EndsWith("*"))
                                {
                                    s = s.TrimEnd('*') + item.EnchantId;
                                }
                                item = new ItemInstance(s);
                                character.Character[(CharacterSlot)slot] = item;
                                character.UnsavedChanges = true;
                            }
                            else
                            {
                                foreach (string s in character.Character.AvailableItems)
                                {
                                    ItemInstance newItem = null;
                                    if (s.IndexOf('.') < 0)
                                    {
                                        if (!s.StartsWith("-", StringComparison.Ordinal))
                                        {
                                            newItem = item.Clone();
                                            newItem.Id = int.Parse(s);
                                            if (newItem.Item.SocketColor1 == ItemSlot.None) newItem.Gem1 = null;
                                            if (newItem.Item.SocketColor2 == ItemSlot.None) newItem.Gem2 = null;
                                            if (newItem.Item.SocketColor3 == ItemSlot.None) newItem.Gem3 = null;
                                        }
                                    }
                                    else
                                    {
                                        string[] slist = s.Split('.');
                                        if (slist[1] == "*")
                                        {
                                            newItem = item.Clone();
                                            newItem.Id = int.Parse(slist[0]);
                                            if (newItem.Item.SocketColor1 == ItemSlot.None) newItem.Gem1 = null;
                                            if (newItem.Item.SocketColor2 == ItemSlot.None) newItem.Gem2 = null;
                                            if (newItem.Item.SocketColor3 == ItemSlot.None) newItem.Gem3 = null;
                                        }
                                        else
                                        {
                                            newItem = item.Clone();
                                            newItem.Id = int.Parse(slist[0]);
                                            newItem.Gem1Id = int.Parse(slist[1]);
                                            newItem.Gem2Id = int.Parse(slist[2]);
                                            newItem.Gem3Id = int.Parse(slist[3]);
                                        }
                                    }
                                    if (newItem != null && newItem.Item.FitsInSlot((CharacterSlot)slot, character.Character))
                                    {
                                        character.Character[(CharacterSlot)slot] = newItem;
                                        string se = s.Substring(s.LastIndexOf('.') + 1);
                                        if (se != "*")
                                        {
                                            character.Character.SetEnchantBySlot((CharacterSlot)slot, Enchant.FindEnchant(int.Parse(se), item.Slot, character.Character));
                                        }
                                        character.UnsavedChanges = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves all characters in BatchCharacterList.
        /// </summary>
        public void SaveCharacters()
        {
            foreach (BatchCharacter character in BatchCharacterList)
            {
                if (character.Character != null && character.UnsavedChanges)
                {
#if !RAWR3
                    character.Character.Save(character.AbsolutePath);
#else
                    using (Stream stream = new FileStream(character.AbsolutePath, FileMode.Create))
                    {
                        character.Character.Save(stream);
                    }
#endif
                    character.UnsavedChanges = false;
                }
            }
        }

        /// <summary>
        /// Saves a copy of all characters in BatchCharacterList.
        /// </summary>
        public void SaveCharactersAsCopy()
        {
            DateTime now = DateTime.Now;

            foreach (BatchCharacter character in BatchCharacterList)
            {
                if (character.Character != null && character.UnsavedChanges)
                {
                    string copyPath = Path.ChangeExtension(character.AbsolutePath, null) + " " + now.ToString("yyyy-M-d H-m") + ".xml";
#if !RAWR3
                    character.Character.Save(copyPath);
#else
                    using (Stream stream = new FileStream(copyPath, FileMode.Create))
                    {
                        character.Character.Save(stream);
                    }
#endif
                }
            }
        }

        /// <summary>
        /// Optimizes each character.
        /// </summary>
        public void Optimize()
        {
            if (currentOperation != AsyncOperation.None) return;

            currentOperation = AsyncOperation.Optimize;
            optimizer.OptimizationMethod = OptimizationMethod;
            optimizer.GreedyOptimizationMethod = GreedyOptimizationMethod;

            batchIndex = 0;
            optimizerRound = 0;
            OptimizeCurrentBatchCharacter();
        }

        private void OptimizeCurrentBatchCharacter()
        {
            int _thoroughness = Thoroughness;
            if (CurrentBatchCharacter.Character != null)
            {
                if (optimizerRound == 0)
                {
                    bool _overrideRegem = OverrideRegem;
                    bool _overrideReenchant = OverrideReenchant;
                    optimizer.InitializeItemCache(CurrentBatchCharacter.Character, CurrentBatchCharacter.Character.AvailableItems, _overrideRegem, _overrideReenchant, TemplateGemsEnabled, CurrentBatchCharacter.Model, false, false, false, null, false);
                }
                optimizer.OptimizeCharacterAsync(CurrentBatchCharacter.Character, _thoroughness, true);
            }
        }

        /// <summary>
        /// Cancels current operation.
        /// </summary>
        public void Cancel()
        {
            if ((currentOperation == AsyncOperation.Optimize || currentOperation == AsyncOperation.BuildUpgradeList || currentOperation == AsyncOperation.ProgressiveOptimize || currentOperation == AsyncOperation.BuildProgressiveUpgradeList) && optimizer.IsBusy)
            {
                optimizer.CancelAsync();
            }
            if ((currentOperation == AsyncOperation.BatchOptimize || currentOperation == AsyncOperation.BuildBatchUpgradeList) && batchOptimizer.IsBusy)
            {
                batchOptimizer.CancelAsync();
            }
        }

        /// <summary>
        /// Computes upgrade list.
        /// </summary>
        public void BuildUpgradeList()
        {
            if (currentOperation != AsyncOperation.None) return;

            currentOperation = AsyncOperation.BuildUpgradeList;
            optimizer.OptimizationMethod = OptimizationMethod;
            optimizer.GreedyOptimizationMethod = GreedyOptimizationMethod;

            batchIndex = 0;
            upgradeListPhase = 0;
            upgradeList = new Dictionary<CharacterSlot, Dictionary<string, UpgradeEntry>>();
            ComputeUpgradesCurrentBatchCharacter();
        }

        private void ComputeUpgradesCurrentBatchCharacter()
        {
            int _thoroughness = Thoroughness;
            bool _overrideRegem = OverrideRegem;
            bool _overrideReenchant = OverrideReenchant;
            if (CurrentBatchCharacter.Character != null)
            {
                optimizer.InitializeItemCache(CurrentBatchCharacter.Character, CurrentBatchCharacter.Character.AvailableItems, _overrideRegem, _overrideReenchant, TemplateGemsEnabled, CurrentBatchCharacter.Model, false, false, false, null, false);
                optimizer.ComputeUpgradesAsync(CurrentBatchCharacter.Character, _thoroughness, SingleItemUpgrade);
            }
        }

        private void EvaluateUpgradeCurrentBatchCharacter(bool initializeCache)
        {
            int _thoroughness = Thoroughness;
            bool _overrideRegem = OverrideRegem;
            bool _overrideReenchant = OverrideReenchant;
            if (initializeCache) optimizer.InitializeItemCache(CurrentBatchCharacter.Character, CurrentBatchCharacter.Character.AvailableItems, _overrideRegem, _overrideReenchant, TemplateGemsEnabled, CurrentBatchCharacter.Model, false, false, false, null, false);
            optimizer.EvaluateUpgradeAsync(CurrentBatchCharacter.Character, _thoroughness, upgradeListEnumerator.Current.Item);
        }

        /// <summary>
        /// Optimizes all characters at the same time based on weights.
        /// </summary>
        public void BatchOptimize()
        {
            int thoroughness = Thoroughness;
            bool overrideRegem = OverrideRegem;
            bool overrideReenchant = OverrideReenchant;

            List<KeyValuePair<Character, float>> batchList = BatchCharacterList.GetBatchOptimizerList();
            if (batchList[0].Key != null)
            {
                batchOptimizer = new BatchOptimizer(batchList, overrideRegem, overrideReenchant, TemplateGemsEnabled);
                batchOptimizer.OptimizeBatchProgressChanged += new OptimizeCharacterProgressChangedEventHandler(batchOptimizer_OptimizeBatchProgressChanged);
                batchOptimizer.OptimizeBatchCompleted += new OptimizeBatchCompletedEventHandler(batchOptimizer_OptimizeBatchCompleted);

                currentOperation = AsyncOperation.BatchOptimize;

                batchOptimizer.OptimizeCharacterAsync(thoroughness);
            }
        }

        void batchOptimizer_OptimizeBatchCompleted(object sender, OptimizeBatchCompletedEventArgs e)
        {
            batchOptimizer.OptimizeBatchProgressChanged -= new OptimizeCharacterProgressChangedEventHandler(batchOptimizer_OptimizeBatchProgressChanged);
            batchOptimizer.OptimizeBatchCompleted -= new OptimizeBatchCompletedEventHandler(batchOptimizer_OptimizeBatchCompleted);

            if (e.Cancelled || e.Error != null)
            {
                currentOperation = AsyncOperation.None;
                UpdateStatusLabel();
                if (OperationCompleted != null)
                {
                    OperationCompleted(this, EventArgs.Empty);
                }
                return;
            }
            for (int i = 0; i < BatchCharacterList.Count; i++)
            {
                Character _character = BatchCharacterList[i].Character;
                Character bestCharacter = e.OptimizedBatch.Character[i];

                _character.SetItems(bestCharacter);

                //BatchCharacterList[i].UnsavedChanges = true;
                //CurrentBatchCharacter.NewScore = e.OptimizedCharacterValue;
                //BatchCharacterList[i].NewScore = ItemInstanceOptimizer.GetOptimizationValue(_character, CurrentBatchCharacter.Model); // on item change always evaluate with equipped gear first (needed by mage module to store incremental data)
            }
            currentOperation = AsyncOperation.None;
            UpdateStatusLabel();
            if (OperationCompleted != null)
            {
                OperationCompleted(this, EventArgs.Empty);
            }
        }

        void batchOptimizer_OptimizeBatchProgressChanged(object sender, OptimizeCharacterProgressChangedEventArgs e)
        {
            Status = string.Format("{0} / {1}", BatchCharacterList.Score, e.BestValue);
            Progress = e.ProgressPercentage;
            if (StatusUpdated != null)
            {
                StatusUpdated(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Computes upgrade list based on batch optimizer.
        /// </summary>
        public void BuildBatchUpgradeList()
        {
            int thoroughness = Thoroughness;
            bool overrideRegem = OverrideRegem;
            bool overrideReenchant = OverrideReenchant;

            List<KeyValuePair<Character, float>> batchList = BatchCharacterList.GetBatchOptimizerList();
            if (batchList[0].Key != null)
            {
                batchOptimizer = new BatchOptimizer(batchList, overrideRegem, overrideReenchant, TemplateGemsEnabled);
                batchOptimizer.ComputeUpgradesProgressChanged += new ComputeUpgradesProgressChangedEventHandler(batchOptimizer_ComputeUpgradesProgressChanged);
                batchOptimizer.ComputeUpgradesCompleted += new ComputeUpgradesCompletedEventHandler(batchOptimizer_ComputeUpgradesCompleted);

                currentOperation = AsyncOperation.BuildBatchUpgradeList;

                batchOptimizer.ComputeUpgradesAsync(thoroughness, SingleItemUpgrade);
            }
        }

        void batchOptimizer_ComputeUpgradesCompleted(object sender, ComputeUpgradesCompletedEventArgs e)
        {
            batchOptimizer.ComputeUpgradesProgressChanged -= new ComputeUpgradesProgressChangedEventHandler(batchOptimizer_ComputeUpgradesProgressChanged);
            batchOptimizer.ComputeUpgradesCompleted -= new ComputeUpgradesCompletedEventHandler(batchOptimizer_ComputeUpgradesCompleted);
            currentOperation = AsyncOperation.None;
            UpdateStatusLabel();
            if (OperationCompleted != null)
            {
                OperationCompleted(this, EventArgs.Empty);
            }
            if (!e.Cancelled)
            {
                CustomSubpoints = null;
                Upgrades = e.Upgrades;
                if (UpgradeListCompleted != null)
                {
                    UpgradeListCompleted(this, EventArgs.Empty);
                }
            }
        }

        void batchOptimizer_ComputeUpgradesProgressChanged(object sender, ComputeUpgradesProgressChangedEventArgs e)
        {
            Status = e.CurrentItem;
            Progress = e.ProgressPercentage;
            if (StatusUpdated != null)
            {
                StatusUpdated(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Optimizes all characters, each subsequent character having lower priority.
        /// </summary>
        public void ProgressiveOptimize()
        {
            if (currentOperation != AsyncOperation.None || BatchCharacterList.Count == 0 || BatchCharacterList[0].Character == null) return;

            currentOperation = AsyncOperation.ProgressiveOptimize;
            optimizer.OptimizationMethod = OptimizationMethod;
            optimizer.GreedyOptimizationMethod = GreedyOptimizationMethod;

            batchIndex = 0;
            optimizerRound = 0;
            bestRoundCharacter = null;
            bestRoundValue = float.NegativeInfinity;

            int _thoroughness = Thoroughness;
            if (CurrentBatchCharacter != null)
            {
                CreateBatchItemGenerator();
                optimizer.InitializeItemCache(CurrentBatchCharacter.Character, CurrentBatchCharacter.Model, itemGenerator);
                while (CurrentBatchCharacter != null && CurrentBatchCharacter.Locked)
                {
                    batchIndex++;
                }
                optimizer.OptimizeCharacterAsync(CurrentBatchCharacter.Character, _thoroughness, true);
            }
            else
            {
                currentOperation = AsyncOperation.None;
            }
        }

        private void CreateBatchItemGenerator()
        {
            bool _overrideRegem = OverrideRegem;
            bool _overrideReenchant = OverrideReenchant;
            Character[] characterList = new Character[BatchCharacterList.Count];
            CalculationsBase[] modelList = new CalculationsBase[BatchCharacterList.Count];
            for (int i = 0; i < BatchCharacterList.Count; i++)
            {
                characterList[i] = BatchCharacterList[i].Character;
                modelList[i] = BatchCharacterList[i].Model;
            }
            itemGenerator = new AvailableItemGenerator(BatchCharacterList[0].Character.AvailableItems, optimizer.GreedyOptimizationMethod != GreedyOptimizationMethod.AllCombinations, TemplateGemsEnabled, _overrideRegem, _overrideReenchant, false, characterList, modelList);
            // create item restrictions for locked characters
            for (int i = 0; i < BatchCharacterList.Count; i++)
            {
                if (BatchCharacterList[i].Locked)
                {
                    itemGenerator.AddItemRestrictions(BatchCharacterList[i].Character);
                }
            }
        }

        /// <summary>
        /// Computes upgrade list based on progressive optimizer.
        /// </summary>
        public void BuildProgressiveUpgradeList()
        {
            if (currentOperation != AsyncOperation.None || BatchCharacterList.Count == 0 || BatchCharacterList[0].Character == null) return;

            currentOperation = AsyncOperation.BuildProgressiveUpgradeList;
            optimizer.OptimizationMethod = OptimizationMethod;
            optimizer.GreedyOptimizationMethod = GreedyOptimizationMethod;

            batchIndex = 0;
            optimizerRound = 0;
            upgradeListPhase = 0;

            int _thoroughness = Thoroughness;
            Character character = CurrentBatchCharacter.Character;
            Dictionary<int, Item> itemById = new Dictionary<int, Item>();
            Item single = SingleItemUpgrade;
            if (single != null)
            {
                itemList = new Item[] { single };
            }
            else
            {
                for (int i = 0; i < BatchCharacterList.Count; i++)
                {
                    CalculationsBase model = BatchCharacterList[i].Model;
                    Item[] items = ItemCache.GetRelevantItems(model, BatchCharacterList[i].Character.Race);
                    foreach (Item item in items)
                    {
                        if (item != null && !item.IsGem)
                        {
                            itemById[item.Id] = item;
                        }
                    }
                }
                itemList = new List<Item>(itemById.Values).ToArray();
            }
            CreateBatchItemGenerator();
            itemGenerator.SaveAvailabilityInformation();
            optimizer.InitializeItemCache(CurrentBatchCharacter.Character, CurrentBatchCharacter.Model, itemGenerator);
            itemIndex = 0;
            upgradeList = new Dictionary<CharacterSlot, Dictionary<string, UpgradeEntry>>();
            workingCharacter = character;
            optimizer.ComputeUpgradesAsync(CurrentBatchCharacter.Character, _thoroughness, itemList[itemIndex]);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
