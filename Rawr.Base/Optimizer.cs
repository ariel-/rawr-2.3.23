using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Xml.Serialization;
using System.Reflection;

namespace Rawr
{
    // keep this in main namespace not to upset serializers
    
    public class OptimizationRequirement
    {
        public string Calculation { get; set; }
        public bool LessThan { get; set; }
        public float Value { get; set; }
    }

    public enum OptimizationMethod
    {
        GeneticAlgorithm,
        SimulatedAnnealing
    }

    public enum GreedyOptimizationMethod
    {
        AllCombinations,
        SingleChanges,
        GreedyBest
    }
}

namespace Rawr.Optimizer
{
    public delegate void OptimizeCharacterProgressChangedEventHandler(object sender, OptimizeCharacterProgressChangedEventArgs e);

    public class OptimizeCharacterProgressChangedEventArgs : ProgressChangedEventArgs
    {
        private float bestValue;

        public OptimizeCharacterProgressChangedEventArgs(int progressPercentage, float bestValue)
            : base(progressPercentage, null)
        {
            this.bestValue = bestValue;
        }

        public float BestValue
        {
            get
            {
                return bestValue;
            }
        }
    }

    public delegate void ComputeUpgradesProgressChangedEventHandler(object sender, ComputeUpgradesProgressChangedEventArgs e);

    public class ComputeUpgradesProgressChangedEventArgs : ProgressChangedEventArgs
    {
        private int itemProgressPercentage;
        private string currentItem;

        public ComputeUpgradesProgressChangedEventArgs(int progressPercentage, int itemProgressPercentage, string currentItem)
            : base(progressPercentage, null)
        {
            this.itemProgressPercentage = itemProgressPercentage;
            this.currentItem = currentItem;
        }

        public int ItemProgressPercentage
        {
            get
            {
                return itemProgressPercentage;
            }
        }

        public string CurrentItem
        {
            get
            {
                return currentItem;
            }
        }
    }

    public delegate void OptimizeCharacterCompletedEventHandler(object sender, OptimizeCharacterCompletedEventArgs e);

    public class OptimizeCharacterCompletedEventArgs : AsyncCompletedEventArgs
    {
        private Character optimizedCharacter;
        private float optimizedCharacterValue;
        private Character currentCharacter;
        private float currentCharacterValue;
        private bool injected;

        public OptimizeCharacterCompletedEventArgs(Character optimizedCharacter, float optimizedCharacterValue, Character currentCharacter, float currentCharacterValue, bool injected, Exception error, bool cancelled)
            : base(error, cancelled, null)
        {
            this.optimizedCharacter = optimizedCharacter;
            this.optimizedCharacterValue = optimizedCharacterValue;
            this.currentCharacter = currentCharacter;
            this.currentCharacterValue = currentCharacterValue;
            this.injected = injected;
        }

        public Character OptimizedCharacter
        {
            get
            {
                RaiseExceptionIfNecessary();
                return optimizedCharacter;
            }
        }

        public Character CurrentCharacter
        {
            get
            {
                RaiseExceptionIfNecessary();
                return currentCharacter;
            }
        }

        public float OptimizedCharacterValue
        {
            get
            {
                RaiseExceptionIfNecessary();
                return optimizedCharacterValue;
            }
        }

        public float CurrentCharacterValue
        {
            get
            {
                RaiseExceptionIfNecessary();
                return currentCharacterValue;
            }
        }

        public bool CurrentCharacterInjected
        {
            get
            {
                RaiseExceptionIfNecessary();
                return injected;
            }
        }
    }

    public delegate void ComputeUpgradesCompletedEventHandler(object sender, ComputeUpgradesCompletedEventArgs e);

    public class ComputeUpgradesCompletedEventArgs : AsyncCompletedEventArgs
    {
        private Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> upgrades;

        public ComputeUpgradesCompletedEventArgs(Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> upgrades, Exception error, bool cancelled)
            : base(error, cancelled, null)
        {
            this.upgrades = upgrades;
        }

        public Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> Upgrades
        {
            get
            {
                RaiseExceptionIfNecessary();
                return upgrades;
            }
        }
    }

    public delegate void EvaluateUpgradeCompletedEventHandler(object sender, EvaluateUpgradeCompletedEventArgs e);

    public class EvaluateUpgradeCompletedEventArgs : AsyncCompletedEventArgs
    {
        private float upgradeValue;
        private ComparisonCalculationUpgrades upgrade;

        public EvaluateUpgradeCompletedEventArgs(float upgradeValue, ComparisonCalculationUpgrades upgrade, Exception error, bool cancelled)
            : base(error, cancelled, null)
        {
            this.upgradeValue = upgradeValue;
            this.upgrade = upgrade;
        }

        public float UpgradeValue
        {
            get
            {
                RaiseExceptionIfNecessary();
                return upgradeValue;
            }
        }

        public ComparisonCalculationUpgrades Upgrade
        {
            get
            {
                RaiseExceptionIfNecessary();
                return upgrade;
            }
        }
    }

    
    public class ComparisonCalculationUpgrades : ComparisonCalculationBase
    {
        public override string Name { get; set; }
        public override string Description { get; set; }
        public override float OverallPoints { get; set; }
        public override float[] SubPoints { get; set; }
        public override ItemInstance ItemInstance { get; set; }
        public override bool Equipped { get; set; }
        public override bool PartEquipped { get; set; }

        [XmlIgnore]
        public override Item Item
        {
            get
            {
                if (ItemInstance != null) return ItemInstance.Item;
                return null;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }

    public class OptimizerCharacter
    {
        public object[] Items;
        public Character Character;
        private const int characterSlots = 19;
        public DirectUpgradeEntry DirectUpgradeEntry { get; set; }
        public Item ExtraItem { get; set; }

        public OptimizerCharacter()
        {
        }

        public OptimizerCharacter(Character character, bool includeFood, bool includeElixir, bool includeTalents)
        {
            Character = character;
            if (includeFood || includeElixir || includeTalents)
            {
                Items = new object[characterSlots + 4];
                Array.Copy(character._item, Items, characterSlots);
                if (includeFood || includeElixir)
                {
                    foreach (Buff buff in character.ActiveBuffs)
                    {
                        if (buff.Group == "Elixirs and Flasks")
                        {
                            bool isbattle = buff.ConflictingBuffs.Contains("Battle Elixir");
                            bool isguardian = buff.ConflictingBuffs.Contains("Guardian Elixir");
                            if (isbattle && isguardian)
                            {
                                Items[characterSlots + 1] = buff;
                            }
                            else if (isbattle)
                            {
                                Items[characterSlots + 1] = buff;
                            }
                            else if (isguardian)
                            {
                                Items[characterSlots + 2] = buff;
                            }
                        }
                        else if (buff.Group == "Food")
                        {
                            Items[characterSlots] = buff;
                        }
                    }
                }
                Items[characterSlots + 3] = character.CurrentTalents;
            }
            else
            {
                Items = character._item;
            }
        }
    }

    public class ItemInstanceOptimizer : OptimizerBase<object, OptimizerCharacter, CharacterCalculationsBase>
    {
        private Character _character;
        private string _calculationToOptimize;
        private List<OptimizationRequirement> _requirements;
        private CalculationsBase model;

        private const int characterSlots = 19;

        public GreedyOptimizationMethod GreedyOptimizationMethod { get; set; }

        private class UniqueItemValidator : OptimizerRangeValidatorBase<object>
        {
            public override bool IsValid(object[] items)
            {
                if (items[StartSlot] != null && items[EndSlot] != null)
                {
                    Item itema = ((ItemInstance)items[StartSlot]).Item;
                    Item itemb = ((ItemInstance)items[EndSlot]).Item;
                    return !(itema.Unique && (itema.Id == itemb.Id || (itema.UniqueId != null && itema.UniqueId.Contains(itemb.Id))));
                }
                return true;
            }
        }

        private bool optimizeFood;
        private bool optimizeElixirs;
        private bool optimizeTalents;
        private bool mutateTalents;
        private bool mixology;

        public ItemInstanceOptimizer()
        {
            slotCount = characterSlots;
            slotItems = new List<object>[characterSlots + 4];
            validators = new List<OptimizerRangeValidatorBase<object>>() {
                new UniqueItemValidator() { StartSlot = (int)CharacterSlot.Finger1, EndSlot = (int)CharacterSlot.Finger2 },
                new UniqueItemValidator() { StartSlot = (int)CharacterSlot.Trinket1, EndSlot = (int)CharacterSlot.Trinket2 },
                new UniqueItemValidator() { StartSlot = (int)CharacterSlot.MainHand, EndSlot = (int)CharacterSlot.OffHand },
            };
            optimizeCharacterProgressChangedDelegate = new SendOrPostCallback(PrivateOptimizeCharacterProgressChanged);
            optimizeCharacterCompletedDelegate = new SendOrPostCallback(PrivateOptimizeCharacterCompleted);
            computeUpgradesProgressChangedDelegate = new SendOrPostCallback(PrivateComputeUpgradesProgressChanged);
            computeUpgradesCompletedDelegate = new SendOrPostCallback(PrivateComputeUpgradesCompleted);
            evaluateUpgradeProgressChangedDelegate = new SendOrPostCallback(PrivateEvaluateUpgradeProgressChanged);
            evaluateUpgradeCompletedDelegate = new SendOrPostCallback(PrivateEvaluateUpgradeCompleted);

            slotItemsRandom = new List<KeyedList<KeyedList<ItemInstance>>>[characterSlots];
            minJeweler = new int[characterSlots];
            SupportsRecycling = true;
        }

        public void InitializeItemCache(Character character, CalculationsBase model, AvailableItemGenerator itemGenerator)
        {
            _character = character;
            Model = model;
            slotCount = characterSlots;
            PopulateAvailableIds(itemGenerator);
        }

        public void InitializeItemCache(Character character, List<string> availableItems, bool overrideRegem, bool overrideReenchant, bool templateGemsEnabled, CalculationsBase model, bool optimizeFood, bool optimizeElixirs, bool mixology, List<TalentsBase> talentSpecs, bool mutateTalents)
        {
            InitializeItemCache(character, availableItems, overrideRegem, overrideReenchant, templateGemsEnabled, model, optimizeFood, optimizeElixirs, mixology, talentSpecs, mutateTalents, false);
        }

        public void InitializeItemCache(Character character, List<string> availableItems, bool overrideRegem, bool overrideReenchant, bool templateGemsEnabled, CalculationsBase model, bool optimizeFood, bool optimizeElixirs, bool mixology, List<TalentsBase> talentSpecs, bool mutateTalents, bool positiveCostItemsAvailable)
        {
            _character = character;
            Model = model;

            List<Buff> food = new List<Buff>(); ;
            List<Buff> battle = new List<Buff>();
            List<Buff> guardian = new List<Buff>();
            List<Buff> flask = new List<Buff>();
            foreach (Buff buff in Buff.AllBuffs.FindAll(buff => model.IsBuffRelevant(buff, character)))
            {
                if (buff.Group == "Elixirs and Flasks")
                {
                    bool isbattle = buff.ConflictingBuffs.Contains("Battle Elixir");
                    bool isguardian = buff.ConflictingBuffs.Contains("Guardian Elixir");
                    if (isbattle && isguardian)
                    {
                        flask.Add(buff);
                    }
                    else if (isbattle)
                    {
                        battle.Add(buff);
                    }
                    else if (isguardian)
                    {
                        guardian.Add(buff);
                    }
                }
                else if (buff.Group == "Food")
                {
                    food.Add(buff);
                }
            }
            food = AvailableItemGenerator.FilterList(food);
            battle = AvailableItemGenerator.FilterList(battle);
            guardian = AvailableItemGenerator.FilterList(guardian);
            flask = AvailableItemGenerator.FilterList(flask);
            battle.AddRange(flask);
            slotItems[characterSlots] = food.ConvertAll(buff => (object)buff);
            slotItems[characterSlots + 1] = battle.ConvertAll(buff => (object)buff);
            slotItems[characterSlots + 2] = guardian.ConvertAll(buff => (object)buff);

            this.optimizeFood = optimizeFood;
            this.optimizeElixirs = optimizeElixirs;
            this.mixology = mixology;
            this.optimizeTalents = (talentSpecs != null && talentSpecs.Count > 0);
            this.mutateTalents = mutateTalents;

            if (optimizeTalents)
            {
                slotItems[characterSlots + 3] = talentSpecs.ConvertAll(spec => (object)spec);
                talentItem = new TalentItem[100];
                TalentsBase talents = talentSpecs[0];
                foreach (PropertyInfo pi in talents.GetType().GetProperties())
                {
                    TalentDataAttribute[] td = pi.GetCustomAttributes(typeof(TalentDataAttribute), true) as TalentDataAttribute[];
                    if (td.Length > 0)
                    {
                        talentItem[td[0].Index] = new TalentItem() { pi = pi, talentData = td[0] };
                        if (td[0].Index + 1 > talentItemCount)
                        {
                            talentItemCount = td[0].Index + 1;
                        }
                    }
                }
                for (int i = 0; i < talentItemCount; i++)
                {
                    if (talentItem[i].talentData.Prerequisite >= 0)
                    {
                        talentItem[talentItem[i].talentData.Prerequisite].childList.Add(i);
                    }
                }
            }
            else
            {
                slotItems[characterSlots + 3] = new List<object>();
            }

            if (optimizeFood || optimizeElixirs || optimizeTalents)
            {
                slotCount = characterSlots + 4;
            }
            else
            {
                slotCount = characterSlots;
            }

            PopulateAvailableIds(availableItems, templateGemsEnabled, overrideRegem, overrideReenchant, positiveCostItemsAvailable);
        }

        public CalculationsBase Model
        {
            get
            {
                return model;
            }
            set
            {
                model = value;
                ThreadPoolValuation = model.SupportsMultithreading && Properties.GeneralSettings.Default.UseMultithreading;
            }
        }

        protected override int EffectiveMaxConcurrencyLevel
        {
            get
            {
                int limit = model.MaxDegreeOfParallelism;
#if SILVERLIGHT
                int procCount = 4;
#else
                int procCount = Environment.ProcessorCount;
#endif
                if (limit == -1)
                {
                    return procCount;
                }
                return Math.Min(procCount, limit);
            }
        }

        public void InitializeItemCache(List<ItemInstance> availableItems)
        {
            List<ItemInstance>[] slotItems = new List<ItemInstance>[characterSlots];
            for (int i = 0; i < slotCount; i++)
            {
                slotItems[i] = new List<ItemInstance>();
            }

            foreach (ItemInstance gemmedItem in availableItems)
            {
                if (gemmedItem != null)
                {
                    Item item = gemmedItem.Item;
                    if (item != null)
                    {
                        for (int i = 0; i < slotCount; i++)
                        {
                            if (item.FitsInSlot((CharacterSlot)i, _character)) slotItems[i].Add(gemmedItem);
                        }
                    }
                }
            }

            for (int i = 0; i < slotCount; i++)
            {
                CharacterSlot slot = (CharacterSlot)i;
                if (slot == CharacterSlot.Finger1 || slot == CharacterSlot.Finger2 || slot == CharacterSlot.Trinket1 || slot == CharacterSlot.Trinket2 || slot == CharacterSlot.MainHand || slot == CharacterSlot.OffHand || slotItems[i].Count == 0)
                {
                    slotItems[i].Add(null);
                }
            }

            for (int i = 0; i < slotCount; i++)
            {
                CharacterSlot slot = (CharacterSlot)i;
                if (slot != CharacterSlot.Finger1 && slot != CharacterSlot.Finger2 && slot != CharacterSlot.Trinket1 && slot != CharacterSlot.Trinket2)
                {
                    slotItems[i] = AvailableItemGenerator.FilterList(slotItems[i], false);
                }
            }

            for (int i = 0; i < slotCount; i++)
            {
                this.slotItems[i] = slotItems[i].ConvertAll(itemInstance => (object)itemInstance);
            }

            itemAvailable = new Dictionary<string, bool>();

            // populate the list for random sampling
            for (int slot = 0; slot < slotCount; slot++)
            {
                slotItemsRandom[slot] = new List<KeyedList<KeyedList<ItemInstance>>>();
                foreach (ItemInstance itemInstance in slotItems[slot])
                {
                    string gemmedId = ((object)itemInstance == null) ? "0.0.0.0.0" : itemInstance.GemmedId;
                    string key1 = ((object)itemInstance == null) ? "0" : itemInstance.Id.ToString();
                    string key2 = ((object)itemInstance == null) ? "0" : itemInstance.EnchantId.ToString();
                    itemAvailable[gemmedId] = true;
                    KeyedList<KeyedList<ItemInstance>> list1 = slotItemsRandom[slot].Find(list => list.Key == key1);
                    if (list1 == null)
                    {
                        list1 = new KeyedList<KeyedList<ItemInstance>>();
                        list1.Key = key1;
                        slotItemsRandom[slot].Add(list1);
                    }
                    KeyedList<ItemInstance> list2 = list1.Find(list => list.Key == key2);
                    if (list2 == null)
                    {
                        list2 = new KeyedList<ItemInstance>();
                        list2.Key = key2;
                        list1.Add(list2);
                    }
                    list2.Add(itemInstance);
                }
            }

            pairSlotMap = new int[slotCount];
            pairSlotMap[(int)CharacterSlot.Back] = -1;
            pairSlotMap[(int)CharacterSlot.Chest] = -1;
            pairSlotMap[(int)CharacterSlot.Feet] = -1;
            pairSlotMap[(int)CharacterSlot.Finger1] = (int)CharacterSlot.Finger2;
            pairSlotMap[(int)CharacterSlot.Finger2] = (int)CharacterSlot.Finger1;
            pairSlotMap[(int)CharacterSlot.Hands] = -1;
            pairSlotMap[(int)CharacterSlot.Head] = -1;
            pairSlotMap[(int)CharacterSlot.Legs] = -1;
            pairSlotMap[(int)CharacterSlot.MainHand] = (int)CharacterSlot.OffHand;
            pairSlotMap[(int)CharacterSlot.OffHand] = (int)CharacterSlot.MainHand;
            pairSlotMap[(int)CharacterSlot.Neck] = -1;
            pairSlotMap[(int)CharacterSlot.Projectile] = -1;
            pairSlotMap[(int)CharacterSlot.ProjectileBag] = -1;
            pairSlotMap[(int)CharacterSlot.Ranged] = -1;
            pairSlotMap[(int)CharacterSlot.Shoulders] = -1;
            pairSlotMap[(int)CharacterSlot.Trinket1] = (int)CharacterSlot.Trinket2;
            pairSlotMap[(int)CharacterSlot.Trinket2] = (int)CharacterSlot.Trinket1;
            pairSlotMap[(int)CharacterSlot.Waist] = -1;
            pairSlotMap[(int)CharacterSlot.Wrist] = -1;

            itemCacheInitialized = true;
        }

        private enum OptimizationOperation
        {
            OptimizeCharacter,
            ComputeUpgrades,
            EvaluateUpgrade
        }

        private OptimizationOperation currentOperation;

        #region Asynchronous Pattern Implementation
        private void PrivateOptimizeCharacterProgressChanged(object state)
        {
            OnOptimizeCharacterProgressChanged(state as OptimizeCharacterProgressChangedEventArgs);
        }

        protected void OnOptimizeCharacterProgressChanged(OptimizeCharacterProgressChangedEventArgs e)
        {
            if (OptimizeCharacterProgressChanged != null)
            {
                OptimizeCharacterProgressChanged(this, e);
            }
        }

        private void PrivateOptimizeCharacterCompleted(object state)
        {
            isBusy = false;
            cancellationPending = false;
            OnOptimizeCharacterCompleted(state as OptimizeCharacterCompletedEventArgs);
        }

        protected void OnOptimizeCharacterCompleted(OptimizeCharacterCompletedEventArgs e)
        {
            if (OptimizeCharacterCompleted != null)
            {
                OptimizeCharacterCompleted(this, e);
            }
        }

        private void PrivateComputeUpgradesProgressChanged(object state)
        {
            OnComputeUpgradesProgressChanged(state as ComputeUpgradesProgressChangedEventArgs);
        }

        protected void OnComputeUpgradesProgressChanged(ComputeUpgradesProgressChangedEventArgs e)
        {
            if (ComputeUpgradesProgressChanged != null)
            {
                ComputeUpgradesProgressChanged(this, e);
            }
        }

        private void PrivateComputeUpgradesCompleted(object state)
        {
            isBusy = false;
            cancellationPending = false;
            OnComputeUpgradesCompleted(state as ComputeUpgradesCompletedEventArgs);
        }

        protected void OnComputeUpgradesCompleted(ComputeUpgradesCompletedEventArgs e)
        {
            if (ComputeUpgradesCompleted != null)
            {
                ComputeUpgradesCompleted(this, e);
            }
        }

        private void PrivateEvaluateUpgradeProgressChanged(object state)
        {
            OnEvaluateUpgradeProgressChanged(state as ProgressChangedEventArgs);
        }

        protected void OnEvaluateUpgradeProgressChanged(ProgressChangedEventArgs e)
        {
            if (EvaluateUpgradeProgressChanged != null)
            {
                EvaluateUpgradeProgressChanged(this, e);
            }
        }

        private void PrivateEvaluateUpgradeCompleted(object state)
        {
            isBusy = false;
            cancellationPending = false;
            OnEvaluateUpgradeCompleted(state as EvaluateUpgradeCompletedEventArgs);
        }

        protected void OnEvaluateUpgradeCompleted(EvaluateUpgradeCompletedEventArgs e)
        {
            if (EvaluateUpgradeCompleted != null)
            {
                EvaluateUpgradeCompleted(this, e);
            }
        }

        private bool isBusy;

        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
        }

        private AsyncOperation asyncOperation;

        public event OptimizeCharacterCompletedEventHandler OptimizeCharacterCompleted;
        public event OptimizeCharacterProgressChangedEventHandler OptimizeCharacterProgressChanged;
        public event ComputeUpgradesProgressChangedEventHandler ComputeUpgradesProgressChanged;
        public event ComputeUpgradesCompletedEventHandler ComputeUpgradesCompleted;
        public event ProgressChangedEventHandler EvaluateUpgradeProgressChanged;
        public event EvaluateUpgradeCompletedEventHandler EvaluateUpgradeCompleted;

        private SendOrPostCallback optimizeCharacterProgressChangedDelegate;
        private SendOrPostCallback optimizeCharacterCompletedDelegate;
        private SendOrPostCallback computeUpgradesProgressChangedDelegate;
        private SendOrPostCallback computeUpgradesCompletedDelegate;
        private SendOrPostCallback evaluateUpgradeProgressChangedDelegate;
        private SendOrPostCallback evaluateUpgradeCompletedDelegate;

        public void OptimizeCharacterAsync(Character character, int thoroughness, bool injectCharacter)
        {
            OptimizeCharacterAsync(character, character.CalculationToOptimize, character.OptimizationRequirements, thoroughness, injectCharacter);
        }

        public void OptimizeCharacterAsync(Character character, string calculationToOptimize, List<OptimizationRequirement> requirements, int thoroughness, bool injectCharacter)
        {
            if (isBusy) throw new InvalidOperationException("Optimizer is working on another operation.");
            isBusy = true;
            cancellationPending = false;
            asyncOperation = AsyncOperationManager.CreateOperation(null);
            ThreadPool.QueueUserWorkItem(delegate
            {
                OptimizeCharacterThreadStart(character, calculationToOptimize, requirements, thoroughness, injectCharacter);
            });
        }

        private void OptimizeCharacterThreadStart(Character character, string calculationToOptimize, List<OptimizationRequirement> requirements, int thoroughness, bool injectCharacter)
        {
            Exception error = null;
            Character optimizedCharacter = null;
            float optimizedCharacterValue = 0.0f;
            float currentCharacterValue = 0.0f;
            bool injected = false;
            try
            {
                optimizedCharacter = PrivateOptimizeCharacter(character, calculationToOptimize, requirements, thoroughness, injectCharacter, out injected, out error);
                if (optimizedCharacter != null)
                {
                    optimizedCharacterValue = GetOptimizationValue(optimizedCharacter, model.GetCharacterCalculations(optimizedCharacter, null, false, optimizeTalents, false), calculationToOptimize, requirements);
                }
                currentCharacterValue = GetOptimizationValue(character, model.GetCharacterCalculations(character, null, false, optimizeTalents, false), calculationToOptimize, requirements);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            asyncOperation.PostOperationCompleted(optimizeCharacterCompletedDelegate, new OptimizeCharacterCompletedEventArgs(optimizedCharacter, optimizedCharacterValue, character, currentCharacterValue, injected, error, cancellationPending));
        }


        public void ComputeUpgradesAsync(Character character, string calculationToOptimize, List<OptimizationRequirement> requirements, int thoroughness)
        {
            ComputeUpgradesAsync(character, calculationToOptimize, requirements, thoroughness, null);
        }

        public void ComputeUpgradesAsync(Character character, int thoroughness, Item singleItemUpgrades)
        {
            ComputeUpgradesAsync(character, character.CalculationToOptimize, character.OptimizationRequirements, thoroughness, singleItemUpgrades);
        }

        public void ComputeUpgradesAsync(Character character, string calculationToOptimize, List<OptimizationRequirement> requirements, int thoroughness, Item singleItemUpgrades)
        {
            if (isBusy) throw new InvalidOperationException("Optimizer is working on another operation.");
            isBusy = true;
            cancellationPending = false;
            asyncOperation = AsyncOperationManager.CreateOperation(null);
            ThreadPool.QueueUserWorkItem(delegate
            {
                ComputeUpgradesThreadStart(character, calculationToOptimize, requirements, thoroughness, singleItemUpgrades);
            });
        }

        private void ComputeUpgradesThreadStart(Character character, string calculationToOptimize, List<OptimizationRequirement> requirements, int thoroughness, Item singleItemUpgrades)
        {
            Exception error = null;
            Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> upgrades = null;
            try
            {
                upgrades = PrivateComputeUpgrades(character, calculationToOptimize, requirements, thoroughness, singleItemUpgrades, out error);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            asyncOperation.PostOperationCompleted(computeUpgradesCompletedDelegate, new ComputeUpgradesCompletedEventArgs(upgrades, error, cancellationPending));
        }

        public void EvaluateUpgradeAsync(Character character, int thoroughness, ItemInstance upgrade)
        {
            EvaluateUpgradeAsync(character, character.CalculationToOptimize, character.OptimizationRequirements, thoroughness, upgrade);
        }

        public void EvaluateUpgradeAsync(Character character, string calculationToOptimize, List<OptimizationRequirement> requirements, int thoroughness, ItemInstance upgrade)
        {
            if (isBusy) throw new InvalidOperationException("Optimizer is working on another operation.");
            isBusy = true;
            cancellationPending = false;
            asyncOperation = AsyncOperationManager.CreateOperation(null);
            ThreadPool.QueueUserWorkItem(delegate
            {
                EvaluateUpgradeThreadStart(character, calculationToOptimize, requirements, thoroughness, upgrade);
            });
        }

        private void EvaluateUpgradeThreadStart(Character character, string calculationToOptimize, List<OptimizationRequirement> requirements, int thoroughness, ItemInstance upgrade)
        {
            Exception error = null;
            ComparisonCalculationUpgrades comparisonUpgrade = null;
            float upgradeValue = 0f;
            try
            {
                upgradeValue = PrivateEvaluateUpgrade(character, calculationToOptimize, requirements, thoroughness, upgrade, out error, out comparisonUpgrade);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            asyncOperation.PostOperationCompleted(evaluateUpgradeCompletedDelegate, new EvaluateUpgradeCompletedEventArgs(upgradeValue, comparisonUpgrade, error, cancellationPending));
        }
        #endregion

        protected override void ReportProgress(int progressPercentage, float bestValue)
        {
            if (!cancellationPending && asyncOperation != null)
            {
                switch (currentOperation)
                {
                    case OptimizationOperation.OptimizeCharacter:
                        asyncOperation.Post(optimizeCharacterProgressChangedDelegate, new OptimizeCharacterProgressChangedEventArgs(progressPercentage, bestValue));
                        break;
                    case OptimizationOperation.ComputeUpgrades:
                        asyncOperation.Post(computeUpgradesProgressChangedDelegate, new ComputeUpgradesProgressChangedEventArgs(itemProgressPercentage, progressPercentage, currentItem));
                        break;
                    case OptimizationOperation.EvaluateUpgrade:
                        asyncOperation.Post(evaluateUpgradeProgressChangedDelegate, new ProgressChangedEventArgs(progressPercentage, null));
                        break;
                }
            }
        }

        public Character OptimizeCharacter(Character character, string calculationToOptimize, List<OptimizationRequirement> requirements, int thoroughness, bool injectCharacter)
        {
            if (isBusy) throw new InvalidOperationException("Optimizer is working on another operation.");
            isBusy = true;
            cancellationPending = false;
            asyncOperation = null;
            Exception error;
            bool injected;
            Character optimizedCharacter = PrivateOptimizeCharacter(character, calculationToOptimize, requirements, thoroughness, injectCharacter, out injected, out error);
            if (error != null) throw error;
            isBusy = false;
            return optimizedCharacter;
        }

        private Character PrivateOptimizeCharacter(Character character, string calculationToOptimize, List<OptimizationRequirement> requirements, int thoroughness, bool injectCharacter, out bool injected, out Exception error)
        {
            if (!itemCacheInitialized) throw new InvalidOperationException("Optimization item cache was not initialized.");
            error = null;
            _character = character;
            Model = Calculations.GetModel(_character.CurrentModel);
            _calculationToOptimize = calculationToOptimize;
            _requirements = requirements;
            _thoroughness = thoroughness;

            currentOperation = OptimizationOperation.OptimizeCharacter;
            Character optimizedCharacter = null;
            float bestValue = 0.0f;
            injected = false;
            lockedSlot = CharacterSlot.None;

            try
            {
                if (_thoroughness == 1)
                {
                    // if we just start from current character and look for direct upgrades
                    // then we have to deal with items that are currently equipped, but are not
                    // currently available
                    MarkEquippedItemsAsValid(_character);
                }

                OptimizerCharacter optCharacter;
                if (injectCharacter || _thoroughness == 1)
                {
                    optCharacter = Optimize(new OptimizerCharacter(character, optimizeFood, optimizeElixirs, optimizeTalents), out bestValue, out injected);
                }
                else
                {
                    optCharacter = Optimize(out bestValue);
                }
                optimizedCharacter = optCharacter != null ? optCharacter.Character : null;
            }
            catch (Exception ex)
            {
                error = ex;
            }

            ReportProgress(100, bestValue);
            return optimizedCharacter;
        }

        public Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> ComputeUpgrades(Character character, string calculationToOptimize, List<OptimizationRequirement> requirements, int thoroughness, Item singleItemUpgrades)
        {
            if (isBusy) throw new InvalidOperationException("Optimizer is working on another operation.");
            isBusy = true;
            cancellationPending = false;
            asyncOperation = null;
            Exception error;
            Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> upgrades = PrivateComputeUpgrades(character, calculationToOptimize, requirements, thoroughness, singleItemUpgrades, out error);
            if (error != null) throw error;
            isBusy = false;
            return upgrades;
        }

        private int itemProgressPercentage = 0;
        private string currentItem = "";

        private void MarkEquippedItemsAsValid(Character character)
        {
            for (int i = 0; i < characterSlots; i++)
            {
                ItemInstance item = character[(CharacterSlot)i];
                if ((object)item != null && item.Id != 0 && !slotItems[i].Contains(item))
                {
                    slotItems[i].Add(item);
                    itemAvailable[item.GemmedId] = true;
                    KeyedList<KeyedList<ItemInstance>> list1 = slotItemsRandom[i].Find(list => list.Key == item.Id.ToString());
                    if (list1 == null)
                    {
                        list1 = new KeyedList<KeyedList<ItemInstance>>();
                        list1.Key = item.Id.ToString();
                        slotItemsRandom[i].Add(list1);
                    }
                    KeyedList<ItemInstance> list2 = list1.Find(list => list.Key == item.EnchantId.ToString());
                    if (list2 == null)
                    {
                        list2 = new KeyedList<ItemInstance>();
                        list2.Key = item.EnchantId.ToString();
                        list1.Add(list2);
                    }
                    list2.Add(item);
                }
            }
        }

        private Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> PrivateComputeUpgrades(Character character, string calculationToOptimize, List<OptimizationRequirement> requirements, int thoroughness, Item singleItemUpgrades, out Exception error)
        {
            if (!itemCacheInitialized) throw new InvalidOperationException("Optimization item cache was not initialized.");
            error = null;
            _character = character;
            Model = Calculations.GetModel(_character.CurrentModel);
            _calculationToOptimize = calculationToOptimize;
            _requirements = requirements;
            _thoroughness = thoroughness;

            currentOperation = OptimizationOperation.ComputeUpgrades;
            Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> upgrades = null;
            try
            {
                // make equipped gear/enchant valid
                MarkEquippedItemsAsValid(_character);

                upgrades = new Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>>();

                Item[] items = ItemCache.GetRelevantItems(model, _character.Race);
                CharacterSlot[] slots = new CharacterSlot[] { CharacterSlot.Back, CharacterSlot.Chest, CharacterSlot.Feet, CharacterSlot.Finger1, CharacterSlot.Hands, CharacterSlot.Head, CharacterSlot.Legs, CharacterSlot.MainHand, CharacterSlot.Neck, CharacterSlot.OffHand, CharacterSlot.Projectile, CharacterSlot.ProjectileBag, CharacterSlot.Ranged, CharacterSlot.Shoulders, CharacterSlot.Trinket1, CharacterSlot.Waist, CharacterSlot.Wrist };
                foreach (CharacterSlot slot in slots)
                    upgrades[slot] = new List<ComparisonCalculationUpgrades>();

                CharacterCalculationsBase baseCalculations = model.GetCharacterCalculations(_character);
                float baseValue = GetOptimizationValue(_character, baseCalculations, calculationToOptimize, requirements);
                Dictionary<int, Item> itemById = new Dictionary<int, Item>();
                foreach (Item item in items)
                {
                    itemById[item.Id] = item;
                }

                if (singleItemUpgrades != null)
                {
                    items = new Item[] { singleItemUpgrades };
                }
                else
                {
                    items = new List<Item>(itemById.Values).ToArray();
                }

                OptimizerCharacter __baseCharacter = new OptimizerCharacter(_character, optimizeFood, optimizeElixirs, optimizeTalents);
                OptimizerCharacter __character;
                for (int i = 0; i < items.Length; i++)
                {
                    Item item = items[i];
                    currentItem = item.Name;
                    itemProgressPercentage = (int)Math.Round((float)i / ((float)items.Length / 100f));
                    if (cancellationPending)
                    {
                        return null;
                    }
                    ReportProgress(0, 0);
                    foreach (CharacterSlot slot in slots)
                    {
                        if (item.FitsInSlot(slot, _character, true))
                        {
                            List<ComparisonCalculationUpgrades> comparisons = upgrades[slot];
                            PopulateLockedItems(item);
                            lockedSlot = slot;
                            List<object> savedItems = slotItems[(int)lockedSlot];
                            slotItems[(int)lockedSlot] = lockedItems;
                            if (lockedSlot == CharacterSlot.Finger1 && Item.ItemsAreConsideredUniqueEqual(_character.Finger2.Item, item))
                            {
                                lockedSlot = CharacterSlot.Finger2;
                            }
                            if (lockedSlot == CharacterSlot.Trinket1 && Item.ItemsAreConsideredUniqueEqual(_character.Trinket2.Item, item))
                            {
                                lockedSlot = CharacterSlot.Trinket2;
                            }
                            __character = BuildSingleItemSwapIndividual(__baseCharacter, (int)lockedSlot, lockedItems[0]);
                            if (lockedSlot == CharacterSlot.MainHand && _character.OffHand != null && Item.ItemsAreConsideredUniqueEqual(_character.OffHand.Item, item))
                            {
                                // can't dual wield unique items, so make the other slot empty
                                __character = BuildSingleItemSwapIndividual(__character, (int)CharacterSlot.OffHand, null);
                            }
                            if (lockedSlot == CharacterSlot.OffHand && Item.ItemsAreConsideredUniqueEqual(_character.MainHand.Item, item))
                            {
                                // can't dual wield unique items, so make the other slot empty
                                __character = BuildSingleItemSwapIndividual(__character, (int)CharacterSlot.MainHand, null);
                            }
                            // instead of just putting in the first gemming on the list select the best one
                            float best = -10000000f;
                            CharacterCalculationsBase bestCalculations;
                            Character bestCharacter;
                            if (lockedItems.Count > 1)
                            {
                                OptimizerCharacter directUpgradeCharacter = LookForDirectItemUpgrades(lockedItems, (int)lockedSlot, best, __character, null, out bestCalculations).Value;
                                if (directUpgradeCharacter != null)
                                {
                                    __character = directUpgradeCharacter;
                                }
                            }
                            if (_thoroughness > 1)
                            {
                                int saveThoroughness = _thoroughness;
                                _thoroughness = 1;
                                float injectValue;
                                bool injected;
                                OptimizerCharacter inject = Optimize(__character, 0, out injectValue, out bestCalculations, out injected);
                                _thoroughness = saveThoroughness;
                                OptimizerCharacter OC = Optimize(inject, injectValue, out best, out bestCalculations, out injected);
                                if (null == OC)
                                {
                                    // Optimize can return null, but none of the calls that depend on bestCharacter 
                                    // can handle bestCharacter when it is null.  
                                    bestCharacter = new Character();
                                }
                                else
                                {
                                    bestCharacter = OC.Character;
                                }
                            }
                            else
                            {
                                bool injected;
                                bestCharacter = Optimize(__character, 0, out best, out bestCalculations, out injected).Character;
                            }
                            if (best > baseValue)
                            {
                                ItemInstance bestItem = bestCharacter[lockedSlot];
                                ComparisonCalculationUpgrades itemCalc = new ComparisonCalculationUpgrades();
                                itemCalc.ItemInstance = bestItem;
                                itemCalc.CharacterItems = bestCharacter.GetItems();
                                itemCalc.Name = item.Name;
                                itemCalc.Equipped = false;
                                itemCalc.OverallPoints = best - baseValue;

                                comparisons.Add(itemCalc);
                            }
                            slotItems[(int)slot] = savedItems;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            ReportProgress(100, 0f);
            return upgrades;
        }

        public float EvaluateUpgrade(Character character, string calculationToOptimize, List<OptimizationRequirement> requirements, int thoroughness, ItemInstance upgrade, out ComparisonCalculationUpgrades comparisonUpgrade)
        {
            if (isBusy) throw new InvalidOperationException("Optimizer is working on another operation.");
            isBusy = true;
            cancellationPending = false;
            asyncOperation = null;
            Exception error;
            float upgradeValue = PrivateEvaluateUpgrade(character, calculationToOptimize, requirements, thoroughness, upgrade, out error, out comparisonUpgrade);
            if (error != null) throw error;
            isBusy = false;
            return upgradeValue;
        }

        private float PrivateEvaluateUpgrade(Character character, string calculationToOptimize, List<OptimizationRequirement> requirements, int thoroughness, ItemInstance upgrade, out Exception error, out ComparisonCalculationUpgrades comparisonUpgrade)
        {
            if (!itemCacheInitialized) throw new InvalidOperationException("Optimization item cache was not initialized.");
            error = null;
            _character = character;
            Model = Calculations.GetModel(_character.CurrentModel);
            _calculationToOptimize = calculationToOptimize;
            _requirements = requirements;
            _thoroughness = thoroughness;

            currentOperation = OptimizationOperation.EvaluateUpgrade;
            comparisonUpgrade = null;
            float upgradeValue = 0f;
            try
            {
                // make equipped gear/enchant valid
                // this is currently only called after calculate upgrades already marks items as valid, but we might have to do this here also if things change
                // MarkEquippedItemsAsValid(_character);

                CharacterSlot[] slots = new CharacterSlot[] { CharacterSlot.Back, CharacterSlot.Chest, CharacterSlot.Feet, CharacterSlot.Finger1, CharacterSlot.Hands, CharacterSlot.Head, CharacterSlot.Legs, CharacterSlot.MainHand, CharacterSlot.Neck, CharacterSlot.OffHand, CharacterSlot.Projectile, CharacterSlot.ProjectileBag, CharacterSlot.Ranged, CharacterSlot.Shoulders, CharacterSlot.Trinket1, CharacterSlot.Waist, CharacterSlot.Wrist };
                CharacterCalculationsBase baseCalculations = model.GetCharacterCalculations(_character);
                float baseValue = GetOptimizationValue(_character, baseCalculations, calculationToOptimize, requirements);

                OptimizerCharacter __baseCharacter = new OptimizerCharacter(_character, optimizeFood, optimizeElixirs, optimizeTalents);
                OptimizerCharacter __character;
                ItemInstance item = upgrade;
                foreach (CharacterSlot slot in slots)
                {
                    if (item.Item.FitsInSlot(slot, _character, true))
                    {
                        lockedItems = new List<object>() { item };
                        lockedSlot = slot;
                        if (lockedSlot == CharacterSlot.Finger1 && item.Item.Unique && (object)_character.Finger2 != null && (_character.Finger2.Id == item.Id || (item.Item.UniqueId != null && item.Item.UniqueId.Contains(_character.Finger2.Id))))
                        {
                            lockedSlot = CharacterSlot.Finger2;
                        }
                        if (lockedSlot == CharacterSlot.Trinket1 && item.Item.Unique && (object)_character.Trinket2 != null && (_character.Trinket2.Id == item.Id || (item.Item.UniqueId != null && item.Item.UniqueId.Contains(_character.Trinket2.Id))))
                        {
                            lockedSlot = CharacterSlot.Trinket2;
                        }
                        List<object> savedItems = slotItems[(int)lockedSlot];
                        slotItems[(int)lockedSlot] = lockedItems;
                        __character = BuildSingleItemSwapIndividual(__baseCharacter, (int)lockedSlot, upgrade);
                        if (lockedSlot == CharacterSlot.MainHand && item.Item.Unique && (object)_character.OffHand != null && (_character.OffHand.Id == item.Id || (item.Item.UniqueId != null && item.Item.UniqueId.Contains(_character.OffHand.Id))))
                        {
                            // can't dual wield unique items, so make the other slot empty
                            __character = BuildSingleItemSwapIndividual(__character, (int)CharacterSlot.OffHand, null);
                        }
                        if (lockedSlot == CharacterSlot.OffHand && item.Item.Unique && (object)_character.MainHand != null && (_character.MainHand.Id == item.Id || (item.Item.UniqueId != null && item.Item.UniqueId.Contains(_character.MainHand.Id))))
                        {
                            // can't dual wield unique items, so make the other slot empty
                            __character = BuildSingleItemSwapIndividual(__character, (int)CharacterSlot.MainHand, null);
                        }
                        float best;
                        CharacterCalculationsBase bestCalculations;
                        Character bestCharacter;
                        if (_thoroughness > 1)
                        {
                            int saveThoroughness = _thoroughness;
                            _thoroughness = 1;
                            float injectValue;
                            bool injected;
                            OptimizerCharacter inject = Optimize(__character, 0, out injectValue, out bestCalculations, out injected);
                            _thoroughness = saveThoroughness;
                            bestCharacter = Optimize(inject, injectValue, out best, out bestCalculations, out injected).Character;
                        }
                        else
                        {
                            bool injected;
                            bestCharacter = Optimize(__character, 0, out best, out bestCalculations, out injected).Character;
                        }
                        if ((object)bestCharacter[lockedSlot] == null || bestCharacter[lockedSlot].Id != item.Id) throw new Exception("There was an internal error in Optimizer when evaluating upgrade.");
                        upgradeValue = best - baseValue;
                        if (upgradeValue < 0 && ((object)_character[lockedSlot] == null || _character[lockedSlot].Id != item.Id)) upgradeValue = 0f;

                        comparisonUpgrade = new ComparisonCalculationUpgrades();
                        comparisonUpgrade.ItemInstance = upgrade;
                        comparisonUpgrade.CharacterItems = bestCharacter.GetItems();
                        comparisonUpgrade.Name = upgrade.Item.Name;
                        comparisonUpgrade.Equipped = false;
                        comparisonUpgrade.OverallPoints = upgradeValue;

                        slotItems[(int)lockedSlot] = savedItems;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            ReportProgress(100, 0f);
            return upgradeValue;
        }

        private bool itemCacheInitialized;

        int[] pairSlotList = new int[] { (int)CharacterSlot.Finger1, (int)CharacterSlot.MainHand, (int)CharacterSlot.Trinket1 };
        int[] pairSlotMap;
        Dictionary<string, bool> itemAvailable = new Dictionary<string, bool>();
        private class KeyedList<T> : List<T>
        {
            public string Key { get; set; }
        }
        List<KeyedList<KeyedList<ItemInstance>>>[] slotItemsRandom;
        int[] minJeweler;
        List<object> lockedItems;
        CharacterSlot lockedSlot = CharacterSlot.None;
        AvailableItemGenerator itemGenerator;

        public AvailableItemGenerator ItemGenerator
        {
            get
            {
                return itemGenerator;
            }
        }

        private void PopulateLockedItems(Item item)
        {
            lockedItems = itemGenerator.GetPossibleGemmedItemsForItem(item, item.Id.ToString()).ConvertAll(itemInstance => (object)itemInstance);
        }

        private void PopulateAvailableIds(List<string> availableItems, bool templateGemsEnabled, bool overrideRegem, bool overrideReenchant)
        {
            PopulateAvailableIds(availableItems, templateGemsEnabled, overrideRegem, overrideReenchant, false);
        }

        private void PopulateAvailableIds(List<string> availableItems, bool templateGemsEnabled, bool overrideRegem, bool overrideReenchant, bool positiveCostItemsAvailable)
        {
            PopulateAvailableIds(new AvailableItemGenerator(availableItems, GreedyOptimizationMethod != GreedyOptimizationMethod.AllCombinations, templateGemsEnabled, overrideRegem, overrideReenchant, true, positiveCostItemsAvailable, _character, model));
        }

        private void PopulateAvailableIds(AvailableItemGenerator itemGenerator)
        {
            this.itemGenerator = itemGenerator;
            for (int slot = 0; slot < characterSlots; slot++)
            {
                slotItems[slot] = itemGenerator.SlotItems[slot].ConvertAll(itemInstance => (object)itemInstance);
            }

            itemAvailable = itemGenerator.ItemAvailable;

            // populate the list for random sampling
            for (int slot = 0; slot < characterSlots; slot++)
            {
                slotItemsRandom[slot] = new List<KeyedList<KeyedList<ItemInstance>>>();
                int minJeweler = slotItems[slot].Count > 0 ? 3 : 0;
                foreach (ItemInstance itemInstance in slotItems[slot])
                {
                    // jeweler data
                    int jewelerCount = itemInstance == null ? 0 : itemInstance.JewelerCount;
                    if (jewelerCount < minJeweler)
                    {
                        minJeweler = jewelerCount;
                    }

                    // random sampling
                    string gemmedId = ((object)itemInstance == null) ? "0.0.0.0.0" : itemInstance.GemmedId;
                    string key1 = ((object)itemInstance == null) ? "0" : itemInstance.Id.ToString();
                    string key2 = ((object)itemInstance == null) ? "0" : itemInstance.EnchantId.ToString();
                    KeyedList<KeyedList<ItemInstance>> list1 = slotItemsRandom[slot].Find(list => list.Key == key1);
                    if (list1 == null)
                    {
                        list1 = new KeyedList<KeyedList<ItemInstance>>();
                        list1.Key = key1;
                        slotItemsRandom[slot].Add(list1);
                    }
                    KeyedList<ItemInstance> list2 = list1.Find(list => list.Key == key2);
                    if (list2 == null)
                    {
                        list2 = new KeyedList<ItemInstance>();
                        list2.Key = key2;
                        list1.Add(list2);
                    }
                    list2.Add(itemInstance);
                }

                this.minJeweler[slot] = minJeweler;
            }

            pairSlotMap = new int[characterSlots];
            pairSlotMap[(int)CharacterSlot.Back] = -1;
            pairSlotMap[(int)CharacterSlot.Chest] = -1;
            pairSlotMap[(int)CharacterSlot.Feet] = -1;
            pairSlotMap[(int)CharacterSlot.Finger1] = (int)CharacterSlot.Finger2;
            pairSlotMap[(int)CharacterSlot.Finger2] = (int)CharacterSlot.Finger1;
            pairSlotMap[(int)CharacterSlot.Hands] = -1;
            pairSlotMap[(int)CharacterSlot.Head] = -1;
            pairSlotMap[(int)CharacterSlot.Legs] = -1;
            pairSlotMap[(int)CharacterSlot.MainHand] = (int)CharacterSlot.OffHand;
            pairSlotMap[(int)CharacterSlot.OffHand] = (int)CharacterSlot.MainHand;
            pairSlotMap[(int)CharacterSlot.Neck] = -1;
            pairSlotMap[(int)CharacterSlot.Projectile] = -1;
            pairSlotMap[(int)CharacterSlot.ProjectileBag] = -1;
            pairSlotMap[(int)CharacterSlot.Ranged] = -1;
            pairSlotMap[(int)CharacterSlot.Shoulders] = -1;
            pairSlotMap[(int)CharacterSlot.Trinket1] = (int)CharacterSlot.Trinket2;
            pairSlotMap[(int)CharacterSlot.Trinket2] = (int)CharacterSlot.Trinket1;
            pairSlotMap[(int)CharacterSlot.Waist] = -1;
            pairSlotMap[(int)CharacterSlot.Wrist] = -1;

            itemCacheInitialized = true;
        }

        public string GetWarningPromptIfNeeded()
        {
            int gemLimit = 12;
            int itemLimit = 512;
            int enchantLimit = 8;

            List<string> emptyList = new List<string>();
            List<string> tooManyList = new List<string>();

            CalculateWarnings(itemGenerator.GemItems, "Gems", emptyList, tooManyList, gemLimit);
            CalculateWarnings(itemGenerator.MetaGemItems, "Meta Gems", emptyList, tooManyList, gemLimit);

            CalculateWarnings(slotItems[(int)CharacterSlot.Head], "Head Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotItems[(int)CharacterSlot.Neck], "Neck Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotItems[(int)CharacterSlot.Shoulders], "Shoulder Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotItems[(int)CharacterSlot.Back], "Back Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotItems[(int)CharacterSlot.Chest], "Chest Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotItems[(int)CharacterSlot.Wrist], "Wrist Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotItems[(int)CharacterSlot.Hands], "Hands Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotItems[(int)CharacterSlot.Waist], "Waist Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotItems[(int)CharacterSlot.Legs], "Legs Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotItems[(int)CharacterSlot.Feet], "Feet Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotItems[(int)CharacterSlot.Finger1], "Finger Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotItems[(int)CharacterSlot.Trinket1], "Trinket Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotItems[(int)CharacterSlot.MainHand], "Main Hand Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotItems[(int)CharacterSlot.OffHand], "Offhand Items", null, tooManyList, itemLimit);
            CalculateWarnings(slotItems[(int)CharacterSlot.Ranged], "Ranged Items", null, tooManyList, itemLimit);
            CalculateWarnings(slotItems[(int)CharacterSlot.Projectile], "Projectile Items", null, tooManyList, itemLimit);
            CalculateWarnings(slotItems[(int)CharacterSlot.ProjectileBag], "Projectile Bag Items", null, tooManyList, itemLimit);

            CalculateWarnings(itemGenerator.SlotEnchants[(int)CharacterSlot.Back], "Back Enchants", emptyList, null, enchantLimit);
            CalculateWarnings(itemGenerator.SlotEnchants[(int)CharacterSlot.Chest], "Chest Enchants", emptyList, null, enchantLimit);
            CalculateWarnings(itemGenerator.SlotEnchants[(int)CharacterSlot.Feet], "Feet Enchants", emptyList, null, enchantLimit);
            CalculateWarnings(itemGenerator.SlotEnchants[(int)CharacterSlot.Finger1], "Finger Enchants", null, null, enchantLimit);
            CalculateWarnings(itemGenerator.SlotEnchants[(int)CharacterSlot.Hands], "Hands Enchants", emptyList, null, enchantLimit);
            CalculateWarnings(itemGenerator.SlotEnchants[(int)CharacterSlot.Head], "Head Enchants", emptyList, null, enchantLimit);
            CalculateWarnings(itemGenerator.SlotEnchants[(int)CharacterSlot.Legs], "Legs Enchants", emptyList, null, enchantLimit);
            CalculateWarnings(itemGenerator.SlotEnchants[(int)CharacterSlot.Shoulders], "Shoulder Enchants", emptyList, null, enchantLimit);
            CalculateWarnings(itemGenerator.SlotEnchants[(int)CharacterSlot.MainHand], "Main Hand Enchants", emptyList, null, enchantLimit);
            CalculateWarnings(itemGenerator.SlotEnchants[(int)CharacterSlot.OffHand], "Offhand Enchants", null, null, enchantLimit);
            CalculateWarnings(itemGenerator.SlotEnchants[(int)CharacterSlot.Ranged], "Ranged Enchants", null, null, enchantLimit);
            CalculateWarnings(itemGenerator.SlotEnchants[(int)CharacterSlot.Wrist], "Wrist Enchants", emptyList, null, enchantLimit);

            if (emptyList.Count + tooManyList.Count > 0)
            {
                if (emptyList.Count > 5) {
                    emptyList.RemoveRange(5, emptyList.Count - 5);
                    emptyList.Add("...");
                }
                if (tooManyList.Count > 5) {
                    tooManyList.RemoveRange(5, tooManyList.Count - 5);
                    tooManyList.Add("...");
                }
                if (tooManyList.Count == 0) {
                    // good sizes but some are empty
                    return "You have not selected any of the following:\r\n\r\n\t"
                        + string.Join("\r\n\t", emptyList.ToArray())
                        + "\r\n\r\nTo select the items, go to their related slot charts and mark them with a Green Diamond. For more information, see the Rawr Documenation pages at rawr.codeplex.com"
                        + "\r\n\r\nDo you want to continue with the optimization?";
                } else if (emptyList.Count == 0) {
                    return "The following slots have a very large number of items selected:\r\n\r\n\t"
                        + string.Join("\r\n\t", tooManyList.ToArray())
                        + "\r\n\r\nDo you want to continue with the optimization?";
                } else {
                    return "You have not selected any of the following:\r\n\r\n\t"
                        + string.Join("\r\n\t", emptyList.ToArray())
                        + "\r\n\r\nTo select the items, go to their related slot charts and mark them with a Green Diamond. For more information, see the Rawr Documenation pages at rawr.codeplex.com"
                        + "\r\n\r\nThe following slots have a very large number of items selected:\r\n\r\n\t"
                        + string.Join("\r\n\t", tooManyList.ToArray())
                        + "\r\n\r\nDo you want to continue with the optimization?";
                }
            }
            return null;
        }

        private void CalculateWarnings(System.Collections.IList list, string group, List<string> emptyList, List<string> tooManyList, int tooManyLimit)
        {
            object el0 = (list.Count > 0) ? list[0] : null;
            if (emptyList != null && (list.Count == 0 || (list.Count == 1 && (el0 == null || (el0 is Enchant && ((Enchant)el0).Id == 0))))) emptyList.Add(group);
            if (tooManyList != null && list.Count > tooManyLimit) tooManyList.Add(group);
        }

        public string CheckOneHandedWeaponUniqueness()
        {
            bool nonUniqueOneHander = false;
            foreach (ItemInstance item in slotItems[(int)CharacterSlot.MainHand])
            {
                if (item != null && item.Item != null)
                {
                    if((item.Item.Type == ItemType.OneHandAxe || item.Item.Type == ItemType.OneHandMace
                        || item.Item.Type == ItemType.OneHandSword || item.Item.Type == ItemType.FistWeapon) && item.Item.Slot == ItemSlot.OneHand && !item.Item.Unique)
                    {
                        if (slotItems[(int)CharacterSlot.OffHand].Contains(item))
                        {
                            nonUniqueOneHander = true;
                            break;
                        }
                    }
                }
            }
            return nonUniqueOneHander ? "You have a one-handed weapon marked available that is not unique." + Environment.NewLine +
                                        "The optimizer will assume you have two of these until you mark it unique." + Environment.NewLine + Environment.NewLine +
                                        "Do you want to continue and let the optimizer assume you have two available?"
                                : null;
        }

        public static float GetOptimizationValue(Character character, CalculationsBase model)
        {
            float ignore;
            return GetCalculationsValue(character, model.GetCharacterCalculations(character), character.CalculationToOptimize, character.OptimizationRequirements, out ignore);
        }

        public static float GetOptimizationValue(Character character, CalculationsBase model, bool referenceCalculation)
        {
            float ignore;
            return GetCalculationsValue(character, model.GetCharacterCalculations(character, null, referenceCalculation, false, false), character.CalculationToOptimize, character.OptimizationRequirements, out ignore);
        }

        public static float GetOptimizationValue(Character character, CharacterCalculationsBase valuation)
        {
            float ignore;
            return GetCalculationsValue(character, valuation, character.CalculationToOptimize, character.OptimizationRequirements, out ignore);
        }

        public static float GetOptimizationValue(Character character, CharacterCalculationsBase valuation, string calculation, List<OptimizationRequirement> requirements)
        {
            float ignore;
            return GetCalculationsValue(character, valuation, calculation, requirements, out ignore);
        }

        protected override float GetOptimizationValue(OptimizerCharacter individual, CharacterCalculationsBase valuation)
        {
            float ignore;
            return GetCalculationsValue(individual.Character, valuation, _calculationToOptimize, _requirements, out ignore);
        }

        protected float GetOptimizationValue(OptimizerCharacter individual, CharacterCalculationsBase valuation, out float nonJewelerValue)
        {
            return GetCalculationsValue(individual.Character, valuation, _calculationToOptimize, _requirements, out nonJewelerValue);
        }

        protected override CharacterCalculationsBase GetValuation(OptimizerCharacter individual)
        {
            bool oldVolatility = Item.OptimizerManagedVolatiliy;
            try
            {
                Item.OptimizerManagedVolatiliy = true;
                return model.GetCharacterCalculations(individual.Character, individual.ExtraItem, false, optimizeTalents, false);
            }
            finally
            {
                Item.OptimizerManagedVolatiliy = oldVolatility;
            }
        }

        protected override object GetItem(OptimizerCharacter individual, int slot)
        {
            return individual.Items[slot];
        }

        protected override object[] GetItems(OptimizerCharacter individual)
        {
            return individual.Items;
        }

        protected override object[] GetRecycledItems(OptimizerCharacter recycledIndividual)
        {
            if (recycledIndividual == null) return null;
            return recycledIndividual.Items;
        }

        protected override OptimizerCharacter GenerateIndividual(object[] items, bool canUseArray, OptimizerCharacter recycledIndividual)
        {
            Character character;
            if (recycledIndividual == null)
            {
                character = new Character(_character, items, characterSlots);
            }
            else
            {
                character = recycledIndividual.Character;
                character.InitializeCharacter(items, characterSlots);
            }
            if (optimizeFood)
            {
                Buff food = (Buff)items[characterSlots];
                if (food != null && !character.ActiveBuffs.Contains(food))
                {
                    character.ActiveBuffsAdd(food);
                    CalculationsBase.RemoveConflictingBuffs(character.ActiveBuffs, food);
                }
            }
            if (optimizeElixirs)
            {
                Buff battle = (Buff)items[characterSlots + 1];
                Buff guardian = (Buff)items[characterSlots + 2];
                if (battle != null && battle.ConflictingBuffs.Contains("Guardian Elixir"))
                {
                    // flask
                    if (battle != null)
                    {
                        if (!character.ActiveBuffs.Contains(battle))
                        {
                            character.ActiveBuffsAdd(battle);
                            if (mixology && battle.Improvements.Count > 0)
                            {
                                // flask of the north doesn't have an improvement (mixology only)
                                character.ActiveBuffsAdd(battle.Improvements[0]);
                            }
                            CalculationsBase.RemoveConflictingBuffs(character.ActiveBuffs, battle);
                        }
                        else if (mixology)
                        {
                            // make sure we have all improvements
                            foreach (Buff improvement in battle.Improvements)
                            {
                                if (!character.ActiveBuffs.Contains(improvement))
                                {
                                    character.ActiveBuffsAdd(improvement);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (battle != null)
                    {
                        if (!character.ActiveBuffs.Contains(battle))
                        {
                            character.ActiveBuffsAdd(battle);
                            if (mixology) character.ActiveBuffsAdd(battle.Improvements[0]);
                            CalculationsBase.RemoveConflictingBuffs(character.ActiveBuffs, battle);
                        }
                        else if (mixology)
                        {
                            // make sure we have all improvements
                            foreach (Buff improvement in battle.Improvements)
                            {
                                if (!character.ActiveBuffs.Contains(improvement))
                                {
                                    character.ActiveBuffsAdd(improvement);
                                }
                            }
                        }
                    }
                    if (guardian != null)
                    {
                        if (!character.ActiveBuffs.Contains(guardian))
                        {
                            character.ActiveBuffsAdd(guardian);
                            if (mixology) character.ActiveBuffsAdd(guardian.Improvements[0]);
                            CalculationsBase.RemoveConflictingBuffs(character.ActiveBuffs, guardian);
                        }
                        else if (mixology)
                        {
                            // make sure we have all improvements
                            foreach (Buff improvement in guardian.Improvements)
                            {
                                if (!character.ActiveBuffs.Contains(improvement))
                                {
                                    character.ActiveBuffsAdd(improvement);
                                }
                            }
                        }
                    }
                }
            }
            if (optimizeTalents)
            {
                character.CurrentTalents = (TalentsBase)items[characterSlots + 3];
            }
            if (recycledIndividual == null)
            {
                return new OptimizerCharacter() 
                { 
                    Character = character,
                    Items = canUseArray ? items : (object[])items.Clone()
                };
            }
            else
            {
                recycledIndividual.Character = character;
                if (canUseArray)
                {
                    recycledIndividual.Items = items;
                }
                else
                {
                    if (recycledIndividual.Items != null)
                    {
                        Array.Copy(items, 0, recycledIndividual.Items, 0, items.Length);
                    }
                    else
                    {
                        recycledIndividual.Items = (object[])items.Clone();
                    }
                }
                return recycledIndividual;
            }
        }

        private static float GetCalculationsValue(Character character, CharacterCalculationsBase calcs, string calculation, List<OptimizationRequirement> requirements, out float nonJewelerValue)
        {
            float gemValue = -100000 * character.GemRequirementsInvalid;
            float nonJewelerGemValue = -100000 * character.NonjewelerGemRequirementsInvalid;
            float ret = 0;
            foreach (OptimizationRequirement requirement in requirements)
            {
                float calcValue = GetCalculationValue(character, calcs, requirement.Calculation);
                if (requirement.LessThan)
                {
                    if (!(calcValue <= requirement.Value))
                        ret += requirement.Value - calcValue;
                }
                else
                {
                    if (!(calcValue >= requirement.Value))
                        ret += calcValue - requirement.Value;
                }
            }

            if (ret < 0)
            {
                nonJewelerValue = ret + nonJewelerGemValue;
                return ret + gemValue;
            }
            else
            {
                float value = GetCalculationValue(character, calcs, calculation);
                nonJewelerValue = value + nonJewelerGemValue;
                return value + gemValue;
            }
        }

        private static float GetCalculationValue(Character character, CharacterCalculationsBase calcs, string calculation)
        {
            if (calculation == null || calculation == "[Overall]")
            {
                return calcs.OverallPoints;
            }
            else if (calculation == "[Cost]")
            {
                return GetNonAvailableItemCost(character);
            }
            else if (calculation.StartsWith("[SubPoint ", StringComparison.Ordinal))
            {
                return calcs.SubPoints[int.Parse(calculation.Substring(10).TrimEnd(']'))];
            }
            else if (calculation.StartsWith("[Talent ", StringComparison.Ordinal))
            {
                return character.CurrentTalents.Data[int.Parse(calculation.Substring(8).TrimEnd(']'))];
            }
            else if (calculation.StartsWith("[Glyph ", StringComparison.Ordinal))
            {
                return character.CurrentTalents.GlyphData[int.Parse(calculation.Substring(7).TrimEnd(']'))] ? 1 : 0;
            }
            else
            {
                return calcs.GetOptimizableCalculationValue(calculation);
            }
        }

        private static float GetNonAvailableItemCost(Character character)
        {
            float cost = 0.0f;
            for (int slot = 0; slot < characterSlots; slot++)
            {
                ItemInstance itemInstance = character._item[slot];
                if (itemInstance != null)
                {
                    Item item = itemInstance.Item;
                    if (item != null && item.AvailabilityInformation != null)
                    {
                        if (item.AvailabilityInformation.PositiveCostItem)
                        {
                            cost += item.Cost;
                        }
                    }
                }
            }
            return cost;
        }

        protected override OptimizerCharacter PostProcess(OptimizerCharacter bestIndividual)
        {
            if (bestIndividual != null)
            {
                itemGenerator.NormalizeCharacter(bestIndividual.Character, false); // this makes it nonnormal, but i don't think it should create any problems
                if (lockedSlot != CharacterSlot.OffHand && !bestIndividual.Character.CurrentCalculations.IncludeOffHandInCalculations(bestIndividual.Character))
                {
                    bestIndividual.Character[CharacterSlot.OffHand] = null;
                }
            }
            return bestIndividual;
        }

        protected override KeyValuePair<float, OptimizerCharacter> LookForDirectItemUpgrades(List<object> items, int slot, float best, OptimizerCharacter bestIndividual, object[] itemList, out CharacterCalculationsBase bestValuation)
        {
            if (slot >= characterSlots || GreedyOptimizationMethod == GreedyOptimizationMethod.AllCombinations || (GreedyOptimizationMethod == GreedyOptimizationMethod.SingleChanges && slot == (int)lockedSlot))
            {
                return base.LookForDirectItemUpgrades(items, slot, best, bestIndividual, null, out bestValuation);
            }
            else if (GreedyOptimizationMethod == GreedyOptimizationMethod.SingleChanges)
            {
                return LookForDirectItemUpgradesSingleChanges(slot, best, bestIndividual, out bestValuation);
            }
            else if (GreedyOptimizationMethod == GreedyOptimizationMethod.GreedyBest)
            {
                return LookForDirectItemUpgradesGreedyBest(slot, best, bestIndividual, out bestValuation);
            }
            bestValuation = null;
            return new KeyValuePair<float, OptimizerCharacter>(float.NegativeInfinity, null);
        }

        private KeyValuePair<float, OptimizerCharacter> LookForDirectItemUpgradesGreedyBest(int slot, float best, OptimizerCharacter bestIndividual, out CharacterCalculationsBase bestValuation)
        {
            //Array.Clear(jewelerItems, 0, 4);
            OptimizerCharacter swappedIndividual;
            bestValuation = null;
            float value;
            bool foundUpgrade = false;

            // we'll look what is the best gem for each color, separating the limited gems
            // then we'll construct a few item instances for each item and evaluate them
            if (itemGenerator.SlotItems[slot].Count > 1 || (itemGenerator.SlotItems[slot].Count == 1 && itemGenerator.SlotItems[slot][0] != null) || slot == (int)lockedSlot)
            {
                object[] itemList = (object[])GetItems(bestIndividual).Clone();
                itemList[slot] = null;
                swappedIndividual = GenerateIndividual(itemList, true, null);
                float bestBlueValue = float.NegativeInfinity;
                Item bestBlueGem = null;
                float bestYellowValue = float.NegativeInfinity;
                Item bestYellowGem = null;
                float bestRedValue = float.NegativeInfinity;
                Item bestRedGem = null;
                float bestBlueJewelerValue = float.NegativeInfinity;
                Item bestBlueJewelerGem = null;
                float bestYellowJewelerValue = float.NegativeInfinity;
                Item bestYellowJewelerGem = null;
                float bestRedJewelerValue = float.NegativeInfinity;
                Item bestRedJewelerGem = null;
                float bestJewelerValue = float.NegativeInfinity;
                Item bestJewelerGem = null;
                float bestMetaValue = float.NegativeInfinity;
                Item bestMetaGem = null;
                float bestNonLimitedValue = float.NegativeInfinity;
                Item bestNonLimitedGem = null;
                float bestEnchantValue = float.NegativeInfinity;
                Enchant bestEnchant = null;
                float bestOneHandEnchantValue = float.NegativeInfinity;
                Enchant bestOneHandEnchant = null;
                if (slot == (int)CharacterSlot.Head)
                {
                    foreach (Item gem in itemGenerator.MetaGemItems)
                    {
                        swappedIndividual.ExtraItem = gem;
                        CharacterCalculationsBase valuation;
                        float nonJewelerValue;
                        value = GetOptimizationValue(swappedIndividual, valuation = GetValuation(swappedIndividual), out nonJewelerValue);
                        if (value > bestMetaValue)
                        {
                            bestMetaValue = value;
                            bestMetaGem = gem;
                        }
                    }
                }
                foreach (Item gem in itemGenerator.GemItems)
                {
                    swappedIndividual.ExtraItem = gem;
                    CharacterCalculationsBase valuation;
                    float nonJewelerValue;
                    value = GetOptimizationValue(swappedIndividual, valuation = GetValuation(swappedIndividual), out nonJewelerValue);
                    if (Item.GemMatchesSlot(gem, ItemSlot.Blue) && !gem.IsLimitedGem && value > bestBlueValue)
                    {
                        bestBlueValue = value;
                        bestBlueGem = gem;
                    }
                    if (Item.GemMatchesSlot(gem, ItemSlot.Red) && !gem.IsLimitedGem && value > bestRedValue)
                    {
                        bestRedValue = value;
                        bestRedGem = gem;
                    }
                    if (Item.GemMatchesSlot(gem, ItemSlot.Yellow) && !gem.IsLimitedGem && value > bestYellowValue)
                    {
                        bestYellowValue = value;
                        bestYellowGem = gem;
                    }
                    if (gem.IsJewelersGem && Item.GemMatchesSlot(gem, ItemSlot.Blue) && value > bestBlueJewelerValue)
                    {
                        bestBlueJewelerValue = value;
                        bestBlueJewelerGem = gem;
                    }
                    if (gem.IsJewelersGem && Item.GemMatchesSlot(gem, ItemSlot.Yellow) && value > bestYellowJewelerValue)
                    {
                        bestYellowJewelerValue = value;
                        bestYellowJewelerGem = gem;
                    }
                    if (gem.IsJewelersGem && Item.GemMatchesSlot(gem, ItemSlot.Red) && value > bestRedJewelerValue)
                    {
                        bestRedJewelerValue = value;
                        bestRedJewelerGem = gem;
                    }
                    if (gem.IsJewelersGem && value > bestJewelerValue)
                    {
                        bestJewelerValue = value;
                        bestJewelerGem = gem;
                    }
                    if (!gem.IsLimitedGem && value > bestNonLimitedValue)
                    {
                        bestNonLimitedValue = value;
                        bestNonLimitedGem = gem;
                    }
                }
                if (itemGenerator.SlotEnchants[slot] != null)
                {
                    foreach (Enchant enchant in itemGenerator.SlotEnchants[slot])
                    {
                        swappedIndividual.ExtraItem = new Item() { Stats = enchant.Stats };
                        CharacterCalculationsBase valuation;
                        float nonJewelerValue;
                        value = GetOptimizationValue(swappedIndividual, valuation = GetValuation(swappedIndividual), out nonJewelerValue);
                        if (value > bestEnchantValue)
                        {
                            bestEnchantValue = value;
                            bestEnchant = enchant;
                        }
                        if (enchant.Slot == ItemSlot.OneHand && value > bestOneHandEnchantValue)
                        {
                            bestOneHandEnchantValue = value;
                            bestOneHandEnchant = enchant;
                        }
                    }
                }

                List<object> list = new List<object>();
                List<Item> rawItems = itemGenerator.SlotRawItems[slot];
                if (slot == (int)lockedSlot)
                {
                    rawItems = new List<Item>() { ((ItemInstance)lockedItems[0]).Item };
                }
                foreach (Item item in rawItems)
                {
                    if (item.AvailabilityInformation == null || item.AvailabilityInformation.GenerativeEnchants.Count > 0)
                    {
                        Enchant enchant = bestEnchant;
                        if (enchant != null)
                        {
                            if (enchant.Slot == ItemSlot.OffHand)
                            {
                                if (item.Type != ItemType.Shield)
                                {
                                    enchant = bestOneHandEnchant;
                                }
                            }
                            else if (enchant.Slot == ItemSlot.TwoHand)
                            {
                                if (item.Slot != ItemSlot.TwoHand)
                                {
                                    enchant = bestOneHandEnchant;
                                }
                            }
                        }
                        int gemCount = itemGenerator.GetItemGemCount(item);
                        // first generate best nonlimited without matching sockets
                        Item[] gems = new Item[4];
                        float[] values = new float[4];
                        bool matches = true;
                        for (int g = 1; g <= gemCount; g++)
                        {
                            if (item.GetSocketColor(g) == ItemSlot.Meta)
                            {
                                gems[g] = bestMetaGem;
                            }
                            else
                            {
                                gems[g] = bestNonLimitedGem;
                                if (!Item.GemMatchesSlot(bestNonLimitedGem, item.GetSocketColor(g)))
                                {
                                    matches = false;
                                }
                            }
                        }
                        ItemInstance itemInstance = new ItemInstance(item, gems[1], gems[2], gems[3], enchant);
                        list.Add(itemInstance);
                        // add jewelers one by one into worst slot
                        for (int i = 0; i < gemCount; i++)
                        {
                            int score = 0;
                            int bestg = 0;
                            for (int g = 1; g <= gemCount; g++)
                            {
                                if (item.GetSocketColor(g) != ItemSlot.Meta && !gems[g].IsJewelersGem && bestJewelerValue > bestNonLimitedValue)
                                {
                                    /*if (Item.GemMatchesSlot(gems[g], item.GetSocketColor(g)))
                                    {*/
                                    if (score < 1)
                                    {
                                        score = 1;
                                        bestg = g;
                                    }
                                    /*}
                                    else
                                    {
                                        if (score < 2)
                                        {
                                            score = 2;
                                            bestg = g;
                                        }
                                    }*/
                                }
                            }
                            if (score > 0)
                            {
                                gems[bestg] = bestJewelerGem;
                                itemInstance = new ItemInstance(item, gems[1], gems[2], gems[3], enchant);
                                list.Add(itemInstance);
                            }
                            else
                            {
                                break;
                            }
                        }
                        // now generate best nonlimited with matching sockets
                        // but only if the nonmatching actually does not match sockets
                        if (!matches)
                        {
                            for (int g = 1; g <= gemCount; g++)
                            {
                                switch (item.GetSocketColor(g))
                                {
                                    case ItemSlot.Meta:
                                        gems[g] = bestMetaGem;
                                        values[g] = bestMetaValue;
                                        break;
                                    case ItemSlot.Red:
                                        gems[g] = bestRedGem;
                                        values[g] = bestRedValue;
                                        break;
                                    case ItemSlot.Yellow:
                                        gems[g] = bestYellowGem;
                                        values[g] = bestYellowValue;
                                        break;
                                    case ItemSlot.Blue:
                                        gems[g] = bestBlueGem;
                                        values[g] = bestBlueValue;
                                        break;
                                    case ItemSlot.Prismatic:
                                        gems[g] = bestNonLimitedGem;
                                        values[g] = bestNonLimitedValue;
                                        break;
                                }
                            }
                            itemInstance = new ItemInstance(item, gems[1], gems[2], gems[3], enchant);
                            list.Add(itemInstance);
                            // add jewelers one by one into worst slot
                            for (int i = 0; i < gemCount; i++)
                            {
                                float score = 0;
                                int bestg = 0;
                                for (int g = 1; g <= gemCount; g++)
                                {
                                    Item jewelerGem;
                                    float jewelerValue = 0.0f;
                                    switch (item.GetSocketColor(g))
                                    {
                                        case ItemSlot.Blue:
                                            jewelerGem = bestBlueJewelerGem;
                                            jewelerValue = bestBlueJewelerValue;
                                            break;
                                        case ItemSlot.Yellow:
                                            jewelerGem = bestYellowJewelerGem;
                                            jewelerValue = bestYellowJewelerValue;
                                            break;
                                        case ItemSlot.Red:
                                            jewelerGem = bestRedJewelerGem;
                                            jewelerValue = bestRedJewelerValue;
                                            break;
                                        case ItemSlot.Prismatic:
                                            jewelerGem = bestJewelerGem;
                                            jewelerValue = bestJewelerValue;
                                            break;
                                    }
                                    if (item.GetSocketColor(g) != ItemSlot.Meta && !gems[g].IsJewelersGem && jewelerValue > values[g])
                                    {
                                        float newScore = jewelerValue - values[g];
                                        if (newScore > score)
                                        {
                                            score = newScore;
                                            bestg = g;
                                        }
                                    }
                                }
                                if (score > 0)
                                {
                                    gems[bestg] = bestJewelerGem;
                                    values[bestg] = bestJewelerValue;
                                    itemInstance = new ItemInstance(item, gems[1], gems[2], gems[3], enchant);
                                    list.Add(itemInstance);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (item.AvailabilityInformation != null && item.AvailabilityInformation.GenerativeEnchants.Count == 0)
                    {
                        foreach (DirectUpgradeEntry entry in item.AvailabilityInformation.SingleDirectUpgradeList)
                        {
                            list.Add(entry.ItemInstance);
                        }
                    }
                }
                return base.LookForDirectItemUpgrades(list, slot, best, bestIndividual, itemList, out bestValuation);
            }
            if (foundUpgrade)
                return new KeyValuePair<float, OptimizerCharacter>(best, bestIndividual);
            return new KeyValuePair<float, OptimizerCharacter>(float.NegativeInfinity, null);
        }

        private List<DirectUpgradeEntry> directValuationsListSingleChanges;

        private void ThreadPoolDirectUpgradeValuationSingleChanges(object ignore)
        {
            OptimizerCharacter swappedIndividual = null;
            DirectUpgradeEntry directUpgradeEntry = null;
            float value = 0;
            CharacterCalculationsBase valuation = null;
            // get initial work item
            lock (directValuationLock)
            {
                if (directValuationsIndex >= directValuationsListSingleChanges.Count)
                {
                    // everything is queued up already, drop out
                    startedThreads--;
                    if (startedThreads == 0)
                    {
                        Monitor.Pulse(directValuationLock);
                    }
                    return;
                }
                remainingThreadsToSpawn--;
                if (remainingThreadsToSpawn > 0)
                {
                    startedThreads++;
                    ThreadPool.QueueUserWorkItem(ThreadPoolDirectUpgradeValuationSingleChanges);
                }
                directUpgradeEntry = directValuationsListSingleChanges[directValuationsIndex++];
                directValuationsTemplate[directValuationsSlot] = directUpgradeEntry.ItemInstance;
                if (IsIndividualValid(directValuationsTemplate))
                {
                    swappedIndividual = GenerateIndividual(directValuationsTemplate, false, swappedIndividual);
                    //swappedIndividual.DirectUpgradeEntry = entry;
                }
            }

            do
            {
                if (swappedIndividual != null)
                {
                    value = GetOptimizationValue(swappedIndividual, valuation = GetValuation(swappedIndividual));
                }

                lock (directValuationLock)
                {
                    directValuationsComplete++;
                    if (swappedIndividual != null && value > bestDirectValue)
                    {
                        bestDirectValue = value;
                        bestDirectValuation = valuation;
                        bestDirectIndividual = BuildCopyIndividual(swappedIndividual, bestDirectIndividual);
                        bestDirectIndividual.DirectUpgradeEntry = directUpgradeEntry;
                        directValuationFoundUpgrade = true;
                    }
                    if (directValuationsComplete >= directValuationsListSingleChanges.Count)
                    {
                        startedThreads--;
                        if (startedThreads == 0)
                        {
                            Monitor.Pulse(directValuationLock);
                        }
                        return;
                    }
                    // get more work
                    if (directValuationsIndex < directValuationsListSingleChanges.Count)
                    {
                        directUpgradeEntry = directValuationsListSingleChanges[directValuationsIndex++];
                        directValuationsTemplate[directValuationsSlot] = directUpgradeEntry.ItemInstance;
                        if (IsIndividualValid(directValuationsTemplate))
                        {
                            swappedIndividual = GenerateIndividual(directValuationsTemplate, false, swappedIndividual);
                            //swappedIndividual.DirectUpgradeEntry = entry;
                        }
                        else
                        {
                            swappedIndividual = null;
                        }
                    }
                    else
                    {
                        // everything is queued up already
                        startedThreads--;
                        return;
                    }
                }
            } while (true);
        }

        private KeyValuePair<float, OptimizerCharacter> LookForDirectItemUpgradesSingleChanges(int slot, float best, OptimizerCharacter bestIndividual, out CharacterCalculationsBase bestValuation)
        {
            OptimizerCharacter swappedIndividual;
            bestValuation = null;
            float value;
            bool foundUpgrade = false;
            object[] itemList = (object[])GetItems(bestIndividual).Clone();

            // we don't want to look at all possible gem/enchant combinations
            // instead do a greedy search within each item for gems/enchants
            var map = itemGenerator.SlotDirectUpgrades[slot];
            foreach (var list in map)
            {
                // find best within the list and then recurse on child list
                List<DirectUpgradeEntry> childList = list;
                while (childList != null)
                {
                    bool useThreadPool = ThreadPoolValuation && childList.Count > 1;
                    List<DirectUpgradeEntry> bestList = null;
                    if (useThreadPool)
                    {
                        bestDirectValue = float.NegativeInfinity;
                        directValuationFoundUpgrade = false;
                        directValuationsIndex = 0;
                        directValuationsComplete = 0;
                        directValuationsListSingleChanges = childList;
                        directValuationsTemplate = itemList;
                        directValuationsSlot = slot;
                        remainingThreadsToSpawn = EffectiveMaxConcurrencyLevel;

                        startedThreads = 1;
                        ThreadPoolDirectUpgradeValuationSingleChanges(null);

                        lock (directValuationLock)
                        {
                            while (directValuationsComplete < directValuationsListSingleChanges.Count || startedThreads > 0) Monitor.Wait(directValuationLock);
                            if (directValuationFoundUpgrade)
                            {
                                if (bestDirectValue > best)
                                {
                                    best = bestDirectValue;
                                    bestValuation = bestDirectValuation;
                                    bestIndividual = bestDirectIndividual;
                                    foundUpgrade = true;
                                }
                                bestList = bestDirectIndividual.DirectUpgradeEntry.DirectUpgradeList;
                            }
                            bestDirectIndividual = null;
                            bestDirectValuation = null;
                            directValuationsListSingleChanges = null;
                            directValuationsTemplate = null;
                        }
                    }
                    else
                    {
                        float bestTemp = float.NegativeInfinity;
                        foreach (DirectUpgradeEntry entry in childList)
                        {
                            itemList[slot] = entry.ItemInstance;
                            if (IsIndividualValid(itemList))
                            {
                                swappedIndividual = GenerateIndividual(itemList, false, null);
                                CharacterCalculationsBase valuation;
                                value = GetOptimizationValue(swappedIndividual, valuation = GetValuation(swappedIndividual));
                                ItemInstance itemInstance = entry.ItemInstance;
                                if (value > best)
                                {
                                    best = value;
                                    bestValuation = valuation;
                                    bestIndividual = swappedIndividual;
                                    foundUpgrade = true;
                                }
                                if (value > bestTemp)
                                {
                                    bestTemp = value;
                                    bestList = entry.DirectUpgradeList;
                                }
                            }
                        }
                    }
                    childList = bestList;
                }
            }
            if (foundUpgrade)
                return new KeyValuePair<float, OptimizerCharacter>(best, bestIndividual);
            return new KeyValuePair<float, OptimizerCharacter>(float.NegativeInfinity, null);
        }

        protected override object GetRandomItem(int slot, object[] items)
        {
            if (slot >= characterSlots)
            {
                return base.GetRandomItem(slot, items);
            }
            else if (lockedSlot == (CharacterSlot)slot)
            {
                return lockedItems[Rnd.Next(lockedItems.Count)];
            }
            else
            {
                // select random item such that jeweler count won't exceed maximum
                // first count how many jewelers at minimum we will have assuming what we have so far and the minimum available from the rest of slots
                int min = 0;
                for (int s = 0; s < slot; s++)
                {
                    ItemInstance item = (ItemInstance)items[s];
                    if (item != null)
                    {
                        min += item.JewelerCount;
                    }
                }
                for (int s = slot + 1; s < characterSlots; s++)
                {
                    min += minJeweler[s];
                }
                int max = 3 - min; // we can use at most this many if we want to be possible to be feasible
                ItemInstance result = null;
                int count = 0;
                Random rand = Rnd;
                do
                {
                    KeyedList<KeyedList<ItemInstance>> list1 = slotItemsRandom[slot][rand.Next(slotItemsRandom[slot].Count)];
                    KeyedList<ItemInstance> list2 = list1[rand.Next(list1.Count)];
                    result = list2[rand.Next(list2.Count)];
                } while (count++ < 10 && result != null && result.JewelerCount > max);
                return result;
            }
        }

        protected override OptimizerCharacter BuildChildIndividual(OptimizerCharacter father, OptimizerCharacter mother, OptimizerCharacter recycledIndividual)
        {
            // jewelcrafter preserving crossover
            // an alternative option would be to add optimizer constraints that restarted character construction if jewelcrafter
            // gems exceeded limit, it is not clear without testing which option is better
            return GeneratorBuildIndividual(
                delegate(int slot, object[] items)
                {
                    if (slot < characterSlots)
                    {
                        int min = 0;
                        for (int s = 0; s < slot; s++)
                        {
                            ItemInstance item = (ItemInstance)items[s];
                            if (item != null)
                            {
                                min += item.JewelerCount;
                            }
                        }
                        for (int s = slot + 1; s < characterSlots; s++)
                        {
                            ItemInstance item1 = (ItemInstance)father.Items[s];
                            int c1 = item1 == null ? 0 : item1.JewelerCount;
                            ItemInstance item2 = (ItemInstance)mother.Items[s];
                            int c2 = item2 == null ? 0 : item2.JewelerCount;
                            min += Math.Min(c1, c2);
                        }
                        int max = 3 - min;
                        ItemInstance f = (ItemInstance)father.Items[slot];
                        int fc = f == null ? 0 : f.JewelerCount;
                        ItemInstance m = (ItemInstance)mother.Items[slot];
                        int mc = m == null ? 0 : m.JewelerCount;
                        if (fc > max)
                        {
                            // we need a unique check otherwise we can end in a dead loop because the min computed above underestimates
                            if ((object)m != null && m.Item.Unique && slot > 0 && items[slot - 1] != null && (((ItemInstance)items[slot - 1]).Id == m.Id || (m.Item.UniqueId != null && m.Item.UniqueId.Contains(((ItemInstance)items[slot - 1]).Id))))
                            {
                                return f;
                            }
                            return m;
                        }
                        else if (mc > max)
                        {
                            if ((object)f != null && f.Item.Unique && slot > 0 && items[slot - 1] != null && (((ItemInstance)items[slot - 1]).Id == f.Id || (f.Item.UniqueId != null && f.Item.UniqueId.Contains(((ItemInstance)items[slot - 1]).Id))))
                            {
                                return m;
                            }
                            return f;
                        }
                        else
                        {
                            return Rnd.NextDouble() < 0.5d ? f : m;
                        }
                    }
                    else
                    {
                        return Rnd.NextDouble() < 0.5d ? GetItem(father, slot) : GetItem(mother, slot);
                    }
                },
                recycledIndividual);
        }

        private ItemInstance ReplaceGem(ItemInstance item, int index, Item gem)
        {
            ItemInstance copy = new ItemInstance(item.Item, item.Gem1, item.Gem2, item.Gem3, item.Enchant);
            copy.SetGem(index, gem);
            return copy;
            // alternatively construct gemmedid and retrieve from cache, trading memory footprint for dictionary access
            //Item copy = new Item(item.Name, item.Quality, item.Type, item.Id, item.IconPath, item.Slot,
            //    item.SetName, item.Unique, item.Stats.Clone(), item.Sockets.Clone(), 0, 0, 0, item.MinDamage,
            //    item.MaxDamage, item.DamageType, item.Speed, item.RequiredClasses);
            //copy.SetGemInternal(1, item.Gem1);
            //copy.SetGemInternal(2, item.Gem2);
            //copy.SetGemInternal(3, item.Gem3);
            //copy.SetGemInternal(index, gem);
            //return copy;
            //string gemmedId = string.Format("{0}.{1}.{2}.{3}", item.Id, (index == 1) ? gem.Id : item.Gem1Id, (index == 2) ? gem.Id : item.Gem2Id, (index == 3) ? gem.Id : item.Gem3Id);
            //return ItemCache.FindItemById(gemmedId, true, false);
        }

        private struct GemInformation
        {
            public CharacterSlot Slot;
            public int Index;
            public Item Gem;
            public ItemSlot Socket;
        }

        private OptimizerCharacter BuildReplaceGemMutantCharacter(OptimizerCharacter parent, OptimizerCharacter recycledIndividual, out bool successful)
        {
            object[] items = GetRecycledItems(recycledIndividual) ?? new object[slotCount];
            //object[] items = new object[slotCount];
            Array.Copy(parent.Items, items, slotCount);
            successful = false;

            // do the work

            // build a list of possible mutation points
            List<GemInformation> locationList = new List<GemInformation>();
            for (int slot = 0; slot < characterSlots; slot++)
            {
                if ((object)items[slot] != null)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        Item gem = ((ItemInstance)items[slot]).GetGem(i);
                        if (gem != null) locationList.Add(new GemInformation() { Slot = (CharacterSlot)slot, Index = i, Gem = gem, Socket = ((ItemInstance)items[slot]).Item.GetSocketColor(i) });
                    }
                }
            }

            Random rand = Rnd;
            if (locationList.Count > 0)
            {
                int numberMutations = rand.Next(1, 2);
                for (int i = 0; i < numberMutations; i++)
                {
                    // randomly select mutation point
                    int mutationIndex = rand.Next(locationList.Count);

                    // mutate
                    GemInformation mutation = locationList[mutationIndex];
                    Item newGem;
                    if (mutation.Socket == ItemSlot.Meta)
                    {
                        newGem = itemGenerator.MetaGemItems[rand.Next(itemGenerator.MetaGemItems.Length)];
                    }
                    else
                    {
                        newGem = itemGenerator.GemItems[rand.Next(itemGenerator.GemItems.Length)];
                    }
                    ItemInstance newItem = ReplaceGem((ItemInstance)items[(int)mutation.Slot], mutation.Index, newGem);
                    //Dictionary<int, bool> dict;
                    // make sure the item and item-enchant combo is allowed
                    //Enchant enchant = parent.GetEnchantBySlot(mutation.Slot);
                    //bool valid;
                    if ((lockedSlot == mutation.Slot && lockedItems.Contains(newItem)) || (lockedSlot != mutation.Slot && itemAvailable.ContainsKey(newItem.GemmedId)))
                    {
                        items[(int)mutation.Slot] = newItem;
                        successful = true;
                    }
                }
            }

            // create character
            return GenerateIndividual(items, true, recycledIndividual);
        }

        private OptimizerCharacter BuildSwapGemMutantCharacter(OptimizerCharacter parent, OptimizerCharacter recycledIndividual, out bool successful)
        {
            object[] items = GetRecycledItems(recycledIndividual) ?? new object[slotCount];
            //object[] items = new object[slotCount];
            Array.Copy(parent.Items, items, slotCount);
            successful = false;

            // do the work

            // build a list of possible mutation points
            // make sure not to do meta gem swaps
            List<GemInformation> locationList = new List<GemInformation>();
            for (int slot = 0; slot < characterSlots; slot++)
            {
                if ((object)items[slot] != null)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        Item gem = ((ItemInstance)items[slot]).GetGem(i);
                        if (gem != null && gem.Slot != ItemSlot.Meta) locationList.Add(new GemInformation() { Slot = (CharacterSlot)slot, Index = i, Gem = gem, Socket = ((ItemInstance)items[slot]).Item.GetSocketColor(i) });
                    }
                }
            }

            Random rand = Rnd;
            if (locationList.Count > 1)
            {
                GemInformation mutation1;
                GemInformation mutation2;
                int tries = 0;
                // randomly select mutation point
                bool promising;
                do
                {
                    promising = true;
                    int mutationIndex1 = rand.Next(locationList.Count);
                    int mutationIndex2 = rand.Next(locationList.Count);
                    mutation1 = locationList[mutationIndex1];
                    mutation2 = locationList[mutationIndex2];
                    if (mutation1.Gem.Slot == mutation2.Gem.Slot) promising = false;

                    int matchNow = 0;
                    if (Item.GemMatchesSlot(mutation1.Gem, mutation1.Socket)) matchNow++;
                    if (Item.GemMatchesSlot(mutation2.Gem, mutation2.Socket)) matchNow++;
                    if (matchNow == 2) promising = false;
                    int matchThen = 0;
                    if (Item.GemMatchesSlot(mutation1.Gem, mutation2.Socket)) matchThen++;
                    if (Item.GemMatchesSlot(mutation2.Gem, mutation1.Socket)) matchThen++;
                    if (tries < 50)
                    {
                        if (mutation1.Socket == mutation2.Socket) promising = false;
                        if (matchThen <= matchNow) promising = false;
                    }
                    else
                    {
                        // allow 1 to 1 trade, because the other socket bonus might be better
                        if (mutation1.Socket == mutation2.Socket && mutation1.Gem.IsLimitedGem == mutation2.Gem.IsLimitedGem) promising = false;
                        if (matchThen < matchNow || matchThen == 0) promising = false;
                    }
                    tries++;
                } while (tries < 100 && !promising);

                if (promising)
                {
                    // mutate
                    ItemInstance item1 = ReplaceGem((ItemInstance)items[(int)mutation1.Slot], mutation1.Index, mutation2.Gem);
                    ItemInstance item2 = ReplaceGem((ItemInstance)items[(int)mutation2.Slot], mutation2.Index, mutation1.Gem);
                    if (((lockedSlot == mutation1.Slot && lockedItems.Contains(item1)) || (lockedSlot != mutation1.Slot && itemAvailable.ContainsKey(item1.GemmedId))) && ((lockedSlot == mutation2.Slot && lockedItems.Contains(item2)) || (lockedSlot != mutation2.Slot && itemAvailable.ContainsKey(item2.GemmedId))))
                    {
                        successful = true;
                        items[(int)mutation1.Slot] = item1;
                        items[(int)mutation2.Slot] = item2;
                    }
                }
            }

            // create character

            if (successful)
            {
                return GenerateIndividual(items, true, recycledIndividual);
            }
            return null;
        }

        protected override OptimizerCharacter BuildMutantIndividual(OptimizerCharacter parent, OptimizerCharacter recycledIndividual)
        {
            bool successful;
            OptimizerCharacter mutant = null;
            Random rand = Rnd;
            if (optimizeTalents && mutateTalents && rand.NextDouble() < 0.5)
            {
                return BuildMutateTalentsCharacter(parent, recycledIndividual);
            }
            else if (itemGenerator == null || rand.NextDouble() < 0.9)
            {
                return base.BuildMutantIndividual(parent, recycledIndividual);
            }
            else if (rand.NextDouble() < 0.5)
            {
                mutant = BuildReplaceGemMutantCharacter(parent, recycledIndividual, out successful);
                if (!successful)
                {
                    return base.BuildMutantIndividual(parent, recycledIndividual);
                }
            }
            else
            {
                mutant = BuildSwapGemMutantCharacter(parent, recycledIndividual, out successful);
                if (!successful)
                {
                    return base.BuildMutantIndividual(parent, recycledIndividual);
                }
            }
            return mutant;
        }

        private class TalentItem
        {
            public PropertyInfo pi;
            public TalentDataAttribute talentData;
            public List<int> childList = new List<int>();
        }

        private TalentItem[] talentItem = new TalentItem[100];
        private int talentItemCount;

        private OptimizerCharacter BuildMutateTalentsCharacter(OptimizerCharacter parent, OptimizerCharacter recycledIndividual)
        {
            OptimizerCharacter optCharacter = GenerateIndividual(parent.Items, false, recycledIndividual);
            object[] items = optCharacter.Items;
            Character character = optCharacter.Character;
#if RAWR3
            TalentsBase talents = ((TalentsBase)items[characterSlots + 3]).Clone();
#else
            TalentsBase talents = (TalentsBase)((ICloneable)items[characterSlots + 3]).Clone();
#endif
            items[characterSlots + 3] = talents;
            int[,] treeCount = new int[3, 11];
            for (int j = 0; j < talentItemCount; j++)
            {
                treeCount[talentItem[j].talentData.Tree, talentItem[j].talentData.Row - 1] += talents.Data[j];
            }
            // add the talent somewhere
            bool talentAdded = false;
            Random rand = Rnd;
            do
            {
                int index = rand.Next(talentItemCount);
                if (talents.Data[index] == talentItem[index].talentData.MaxPoints) continue;
                int p = talentItem[index].talentData.Prerequisite;
                if (p >= 0 && talents.Data[p] < talentItem[p].talentData.MaxPoints) continue;
                int points = 0;
                for (int k = 0; k < talentItem[index].talentData.Row - 1; k++)
                {
                    points += treeCount[talentItem[index].talentData.Tree, k];
                }
                if (points < 5 * (talentItem[index].talentData.Row - 1)) continue;
                // we're good, we can add the talent point
                talents.Data[index]++;
                treeCount[talentItem[index].talentData.Tree, talentItem[index].talentData.Row - 1]++;
                talentAdded = true;
            } while (!talentAdded);
            // pick a talent with some points invested that we can take points out of
            bool talentRemoved = false;
            do
            {
                int index = rand.Next(talentItemCount);
                if (talents.Data[index] == 0) continue;
                bool locked = false;
                foreach (int child in talentItem[index].childList)
                {
                    if (talents.Data[child] > 0)
                    {
                        locked = true;
                        break;
                    }
                }
                if (!locked)
                {
                    int i = 1;
                    int pts = 0;
                    int _row = talentItem[index].talentData.Row - 1;
                    int tree = talentItem[index].talentData.Tree;
                    for (i = 0; i <= _row; i++) pts += treeCount[tree, i];
                    pts = pts - 1;
                    for (i = _row + 1; i < 11; i++)
                    {
                        if (treeCount[tree, i] > 0)
                        {
                            if (pts >= i * 5)
                            {
                                pts += treeCount[tree, i];
                            }
                            else
                            {
                                i = -1;
                                break;
                            }
                        }
                    }
                    if (i >= 0)
                    {
                        // we're good, we can remove the talent point
                        talents.Data[index]--;
                        treeCount[talentItem[index].talentData.Tree, talentItem[index].talentData.Row - 1]--;
                        talentRemoved = true;
                    }
                }
            } while (!talentRemoved);
            character.CurrentTalents = talents;
            return optCharacter;
        }
    }

    #region ItemOptimizer
    public class ItemOptimizer : OptimizerBase<object, Character, CharacterCalculationsBase>
    {
        private Character _character;
        private string _calculationToOptimize;
        private OptimizationRequirement[] _requirements;
        private CalculationsBase model;

        public CalculationsBase Model
        {
            get
            {
                return model;
            }
            set
            {
                model = value;
				ThreadPoolValuation = model.SupportsMultithreading && Properties.GeneralSettings.Default.UseMultithreading;
            }
        }

        private class UniqueItemValidator : OptimizerRangeValidatorBase<object>
        {
            public override bool IsValid(object[] items)
            {
                Item item1 = items[StartSlot] as Item;
                Item item2 = items[EndSlot] as Item;
                return !(item1 != null && item2 != null && item1.Unique && (item1.Id == item2.Id || (item1.UniqueId != null && item1.UniqueId.Contains(item2.Id))));
            }
        }

        private class ItemAvailableValidator : OptimizerRangeValidatorBase<object>
        {
            private bool enchantable;

            public override bool IsValid(object[] items)
            {
                Item item = items[StartSlot] as Item;
                Item gem1 = items[StartSlot + 1] as Item;
                Item gem2 = items[StartSlot + 2] as Item;
                Item gem3 = items[StartSlot + 3] as Item;
                Enchant enchant = items[StartSlot + 4] as Enchant;
                int gemCount = item.AvailabilityInformation.GemCount;
                string key = string.Format("{0}.{1}.{2}.{3}.{4}", 
                    item != null ? item.Id : 0,
                    gem1 != null && gemCount >= 1 ? gem1.Id : 0,
                    gem2 != null && gemCount >= 2 ? gem2.Id : 0,
                    gem3 != null && gemCount >= 3 ? gem3.Id : 0,
                    enchantable && enchant != null ? enchant.Id : 0);
                return item.AvailabilityInformation.ItemAvailable.ContainsKey(key);
            }

            public ItemAvailableValidator(bool enchantable, int slot)
            {
                this.enchantable = enchantable;
                this.StartSlot = slot;
                if (enchantable)
                {
                    EndSlot = slot + 4;
                }
                else
                {
                    EndSlot = slot + 3;
                }
            }
        }

        private const int characterSlots = 19;

        public ItemOptimizer()
        {
            slotCount = characterSlots * 5;
            slotItems = new List<object>[slotCount];
            validators = new List<OptimizerRangeValidatorBase<object>>() {
                new UniqueItemValidator() { StartSlot = 5 * (int)CharacterSlot.Finger1, EndSlot = 5 * (int)CharacterSlot.Finger2 },
                new UniqueItemValidator() { StartSlot = 5 * (int)CharacterSlot.Trinket1, EndSlot = 5 * (int)CharacterSlot.Trinket2 },
                new UniqueItemValidator() { StartSlot = 5 * (int)CharacterSlot.MainHand, EndSlot = 5 * (int)CharacterSlot.OffHand },
                new ItemAvailableValidator(false, 5 * (int)CharacterSlot.Projectile),
                new ItemAvailableValidator(true, 5 * (int)CharacterSlot.Head),
                new ItemAvailableValidator(false, 5 * (int)CharacterSlot.Neck),
                new ItemAvailableValidator(true, 5 * (int)CharacterSlot.Shoulders),
                new ItemAvailableValidator(true, 5 * (int)CharacterSlot.Chest),
                new ItemAvailableValidator(false, 5 * (int)CharacterSlot.Waist),
                new ItemAvailableValidator(true, 5 * (int)CharacterSlot.Legs),
                new ItemAvailableValidator(true, 5 * (int)CharacterSlot.Feet),
                new ItemAvailableValidator(true, 5 * (int)CharacterSlot.Wrist),
                new ItemAvailableValidator(true, 5 * (int)CharacterSlot.Hands),
                new ItemAvailableValidator(true, 5 * (int)CharacterSlot.Finger1),
                new ItemAvailableValidator(true, 5 * (int)CharacterSlot.Finger2),
                new ItemAvailableValidator(false, 5 * (int)CharacterSlot.Trinket1),
                new ItemAvailableValidator(false, 5 * (int)CharacterSlot.Trinket2),
                new ItemAvailableValidator(true, 5 * (int)CharacterSlot.Back),
                new ItemAvailableValidator(true, 5 * (int)CharacterSlot.MainHand),
                new ItemAvailableValidator(true, 5 * (int)CharacterSlot.OffHand),
                new ItemAvailableValidator(true, 5 * (int)CharacterSlot.Ranged),
                new ItemAvailableValidator(false, 5 * (int)CharacterSlot.ProjectileBag),
            };
            optimizeCharacterProgressChangedDelegate = new SendOrPostCallback(PrivateOptimizeCharacterProgressChanged);
            optimizeCharacterCompletedDelegate = new SendOrPostCallback(PrivateOptimizeCharacterCompleted);
            optimizeCharacterThreadStartDelegate = new OptimizeCharacterThreadStartDelegate(OptimizeCharacterThreadStart);
            computeUpgradesProgressChangedDelegate = new SendOrPostCallback(PrivateComputeUpgradesProgressChanged);
            computeUpgradesCompletedDelegate = new SendOrPostCallback(PrivateComputeUpgradesCompleted);
            computeUpgradesThreadStartDelegate = new ComputeUpgradesThreadStartDelegate(ComputeUpgradesThreadStart);
            evaluateUpgradeProgressChangedDelegate = new SendOrPostCallback(PrivateEvaluateUpgradeProgressChanged);
            evaluateUpgradeCompletedDelegate = new SendOrPostCallback(PrivateEvaluateUpgradeCompleted);
            evaluateUpgradeThreadStartDelegate = new EvaluateUpgradeThreadStartDelegate(EvaluateUpgradeThreadStart);
        }

        public void InitializeItemCache(Character character, List<string> availableItems, bool overrideRegem, bool overrideReenchant, bool templateGemsEnabled, CalculationsBase model)
        {
            _character = character;
            Model = model;
            PopulateAvailableIds(availableItems, templateGemsEnabled, overrideRegem, overrideReenchant);
        }

        private enum OptimizationOperation
        {
            OptimizeCharacter,
            ComputeUpgrades,
            EvaluateUpgrade
        }

        private OptimizationOperation currentOperation;

        #region Asynchronous Pattern Implementation
        private void PrivateOptimizeCharacterProgressChanged(object state)
        {
            OnOptimizeCharacterProgressChanged(state as OptimizeCharacterProgressChangedEventArgs);
        }

        protected void OnOptimizeCharacterProgressChanged(OptimizeCharacterProgressChangedEventArgs e)
        {
            if (OptimizeCharacterProgressChanged != null)
            {
                OptimizeCharacterProgressChanged(this, e);
            }
        }

        private void PrivateOptimizeCharacterCompleted(object state)
        {
            isBusy = false;
            cancellationPending = false;
            OnOptimizeCharacterCompleted(state as OptimizeCharacterCompletedEventArgs);
        }

        protected void OnOptimizeCharacterCompleted(OptimizeCharacterCompletedEventArgs e)
        {
            if (OptimizeCharacterCompleted != null)
            {
                OptimizeCharacterCompleted(this, e);
            }
        }

        private void PrivateComputeUpgradesProgressChanged(object state)
        {
            OnComputeUpgradesProgressChanged(state as ComputeUpgradesProgressChangedEventArgs);
        }

        protected void OnComputeUpgradesProgressChanged(ComputeUpgradesProgressChangedEventArgs e)
        {
            if (ComputeUpgradesProgressChanged != null)
            {
                ComputeUpgradesProgressChanged(this, e);
            }
        }

        private void PrivateComputeUpgradesCompleted(object state)
        {
            isBusy = false;
            cancellationPending = false;
            OnComputeUpgradesCompleted(state as ComputeUpgradesCompletedEventArgs);
        }

        protected void OnComputeUpgradesCompleted(ComputeUpgradesCompletedEventArgs e)
        {
            if (ComputeUpgradesCompleted != null)
            {
                ComputeUpgradesCompleted(this, e);
            }
        }

        private void PrivateEvaluateUpgradeProgressChanged(object state)
        {
            OnEvaluateUpgradeProgressChanged(state as ProgressChangedEventArgs);
        }

        protected void OnEvaluateUpgradeProgressChanged(ProgressChangedEventArgs e)
        {
            if (EvaluateUpgradeProgressChanged != null)
            {
                EvaluateUpgradeProgressChanged(this, e);
            }
        }

        private void PrivateEvaluateUpgradeCompleted(object state)
        {
            isBusy = false;
            cancellationPending = false;
            OnEvaluateUpgradeCompleted(state as EvaluateUpgradeCompletedEventArgs);
        }

        protected void OnEvaluateUpgradeCompleted(EvaluateUpgradeCompletedEventArgs e)
        {
            if (EvaluateUpgradeCompleted != null)
            {
                EvaluateUpgradeCompleted(this, e);
            }
        }

        private bool isBusy;

        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
        }

        private AsyncOperation asyncOperation;
        private delegate void OptimizeCharacterThreadStartDelegate(Character character, string calculationToOptimize, OptimizationRequirement[] requirements, int thoroughness, bool injectCharacter);
        private delegate void ComputeUpgradesThreadStartDelegate(Character character, string calculationToOptimize, OptimizationRequirement[] requirements, int thoroughness, Item singleItemUpgrades);
        private delegate void EvaluateUpgradeThreadStartDelegate(Character character, string calculationToOptimize, OptimizationRequirement[] requirements, int thoroughness, ItemInstance upgrade);

        public event OptimizeCharacterCompletedEventHandler OptimizeCharacterCompleted;
        public event OptimizeCharacterProgressChangedEventHandler OptimizeCharacterProgressChanged;
        public event ComputeUpgradesProgressChangedEventHandler ComputeUpgradesProgressChanged;
        public event ComputeUpgradesCompletedEventHandler ComputeUpgradesCompleted;
        public event ProgressChangedEventHandler EvaluateUpgradeProgressChanged;
        public event EvaluateUpgradeCompletedEventHandler EvaluateUpgradeCompleted;

        private SendOrPostCallback optimizeCharacterProgressChangedDelegate;
        private SendOrPostCallback optimizeCharacterCompletedDelegate;
        private OptimizeCharacterThreadStartDelegate optimizeCharacterThreadStartDelegate;
        private SendOrPostCallback computeUpgradesProgressChangedDelegate;
        private SendOrPostCallback computeUpgradesCompletedDelegate;
        private ComputeUpgradesThreadStartDelegate computeUpgradesThreadStartDelegate;
        private SendOrPostCallback evaluateUpgradeProgressChangedDelegate;
        private SendOrPostCallback evaluateUpgradeCompletedDelegate;
        private EvaluateUpgradeThreadStartDelegate evaluateUpgradeThreadStartDelegate;

        public void OptimizeCharacterAsync(Character character, string calculationToOptimize, OptimizationRequirement[] requirements, int thoroughness, bool injectCharacter)
        {
            if (isBusy) throw new InvalidOperationException("Optimizer is working on another operation.");
            isBusy = true;
            cancellationPending = false;
            asyncOperation = AsyncOperationManager.CreateOperation(null);
            optimizeCharacterThreadStartDelegate.BeginInvoke(character, calculationToOptimize, requirements, thoroughness, injectCharacter, null, null);
        }

        private void OptimizeCharacterThreadStart(Character character, string calculationToOptimize, OptimizationRequirement[] requirements, int thoroughness, bool injectCharacter)
        {
            Exception error = null;
            Character optimizedCharacter = null;
            float optimizedCharacterValue = 0.0f;
            float currentCharacterValue = 0.0f;
            bool injected = false;
            try
            {
                optimizedCharacter = PrivateOptimizeCharacter(character, calculationToOptimize, requirements, thoroughness, injectCharacter, out injected, out error);
                if (optimizedCharacter != null)
                {
                    optimizedCharacterValue = GetOptimizationValue(optimizedCharacter, model.GetCharacterCalculations(optimizedCharacter));
                }
                currentCharacterValue = GetOptimizationValue(character, model.GetCharacterCalculations(character));
            }
            catch (Exception ex)
            {
                error = ex;
            }
            asyncOperation.PostOperationCompleted(optimizeCharacterCompletedDelegate, new OptimizeCharacterCompletedEventArgs(optimizedCharacter, optimizedCharacterValue, character, currentCharacterValue, injected, error, cancellationPending));
        }

        public void ComputeUpgradesAsync(Character character, string calculationToOptimize, OptimizationRequirement[] requirements, int thoroughness)
        {
            ComputeUpgradesAsync(character, calculationToOptimize, requirements, thoroughness, null);
        }

        public void ComputeUpgradesAsync(Character character, string calculationToOptimize, OptimizationRequirement[] requirements, int thoroughness, Item singleItemUpgrades)
        {
            if (isBusy) throw new InvalidOperationException("Optimizer is working on another operation.");
            isBusy = true;
            cancellationPending = false;
            asyncOperation = AsyncOperationManager.CreateOperation(null);
            computeUpgradesThreadStartDelegate.BeginInvoke(character, calculationToOptimize, requirements, thoroughness, singleItemUpgrades, null, null);
        }

        private void ComputeUpgradesThreadStart(Character character, string calculationToOptimize, OptimizationRequirement[] requirements, int thoroughness, Item singleItemUpgrades)
        {
            Exception error = null;
            Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> upgrades = null;
            try
            {
                upgrades = PrivateComputeUpgrades(character, calculationToOptimize, requirements, thoroughness, singleItemUpgrades, out error);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            asyncOperation.PostOperationCompleted(computeUpgradesCompletedDelegate, new ComputeUpgradesCompletedEventArgs(upgrades, error, cancellationPending));
        }

        public void EvaluateUpgradeAsync(Character character, string calculationToOptimize, OptimizationRequirement[] requirements, int thoroughness, ItemInstance upgrade)
        {
            if (isBusy) throw new InvalidOperationException("Optimizer is working on another operation.");
            isBusy = true;
            cancellationPending = false;
            asyncOperation = AsyncOperationManager.CreateOperation(null);
            evaluateUpgradeThreadStartDelegate.BeginInvoke(character, calculationToOptimize, requirements, thoroughness, upgrade, null, null);
        }

        private void EvaluateUpgradeThreadStart(Character character, string calculationToOptimize, OptimizationRequirement[] requirements, int thoroughness, ItemInstance upgrade)
        {
            Exception error = null;
            float upgradeValue = 0f;
            try
            {
                upgradeValue = PrivateEvaluateUpgrade(character, calculationToOptimize, requirements, thoroughness, upgrade, out error);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            asyncOperation.PostOperationCompleted(evaluateUpgradeCompletedDelegate, new EvaluateUpgradeCompletedEventArgs(upgradeValue, null, error, cancellationPending));
        }
        #endregion

        protected override void ReportProgress(int progressPercentage, float bestValue)
        {
            if (!cancellationPending && asyncOperation != null)
            {
                switch (currentOperation)
                {
                    case OptimizationOperation.OptimizeCharacter:
                        asyncOperation.Post(optimizeCharacterProgressChangedDelegate, new OptimizeCharacterProgressChangedEventArgs(progressPercentage, bestValue));
                        break;
                    case OptimizationOperation.ComputeUpgrades:
                        asyncOperation.Post(computeUpgradesProgressChangedDelegate, new ComputeUpgradesProgressChangedEventArgs(itemProgressPercentage, progressPercentage, currentItem));
                        break;
                    case OptimizationOperation.EvaluateUpgrade:
                        asyncOperation.Post(evaluateUpgradeProgressChangedDelegate, new ProgressChangedEventArgs(progressPercentage, null));
                        break;
                }
            }
        }

        public Character OptimizeCharacter(Character character, string calculationToOptimize, OptimizationRequirement[] requirements, int thoroughness, bool injectCharacter)
        {
            if (isBusy) throw new InvalidOperationException("Optimizer is working on another operation.");
            isBusy = true;
            cancellationPending = false;
            asyncOperation = null;
            Exception error;
            bool injected;
            Character optimizedCharacter = PrivateOptimizeCharacter(character, calculationToOptimize, requirements, thoroughness, injectCharacter, out injected, out error);
            if (error != null) throw error;
            isBusy = false;
            return optimizedCharacter;
        }

        private Character PrivateOptimizeCharacter(Character character, string calculationToOptimize, OptimizationRequirement[] requirements, int thoroughness, bool injectCharacter, out bool injected, out Exception error)
        {
            if (!itemCacheInitialized) throw new InvalidOperationException("Optimization item cache was not initialized.");
            error = null;
            _character = character;
            Model = Calculations.GetModel(_character.CurrentModel);
            _calculationToOptimize = calculationToOptimize;
            _requirements = requirements;
            _thoroughness = thoroughness;

            currentOperation = OptimizationOperation.OptimizeCharacter;
            Character optimizedCharacter = null;
            float bestValue = 0.0f;
            injected = false;
            lockedSlot = CharacterSlot.None;

            try
            {
                if (_thoroughness == 1)
                {
                    // if we just start from current character and look for direct upgrades
                    // then we have to deal with items that are currently equipped, but are not
                    // currently available
                    MarkEquippedItemsAsValid(_character);
                }

                if (injectCharacter || _thoroughness == 1)
                {
                    optimizedCharacter = Optimize(character, out bestValue, out injected);
                }
                else
                {
                    optimizedCharacter = Optimize(out bestValue);
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            ReportProgress(100, bestValue);
            return optimizedCharacter;
        }

        public Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> ComputeUpgrades(Character character, string calculationToOptimize, OptimizationRequirement[] requirements, int thoroughness, Item singleItemUpgrades)
        {
            if (isBusy) throw new InvalidOperationException("Optimizer is working on another operation.");
            isBusy = true;
            cancellationPending = false;
            asyncOperation = null;
            Exception error;
            Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> upgrades = PrivateComputeUpgrades(character, calculationToOptimize, requirements, thoroughness, singleItemUpgrades, out error);
            if (error != null) throw error;
            isBusy = false;
            return upgrades;
        }

        private int itemProgressPercentage = 0;
        private string currentItem = "";

        private void MarkEquippedItemsAsValid(Character character)
        {
            for (int i = 0; i < slotCount; i++)
            {
                ItemInstance item = character[(CharacterSlot)i];
                if ((object)item != null && item.Id != 0)
                {
                    if (!slotItems[5 * i].Contains(item.Item))
                    {
                        slotItems[5 * i].Add(item.Item);
                    }
                    Item it = item.Item;
                    if (it != null)
                    {
                        if (it.AvailabilityInformation == null)
                        {
                            itemGenerator.GenerateItemAvailabilityInformation(it);
                        }
                        if (!it.AvailabilityInformation.ItemAvailable.ContainsKey(item.GemmedId))
                        {
                            it.AvailabilityInformation.ItemList.Add(item);
                            it.AvailabilityInformation.ItemAvailable[item.GemmedId] = true;
                        }
                    }
                }
            }
        }

        private Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> PrivateComputeUpgrades(Character character, string calculationToOptimize, OptimizationRequirement[] requirements, int thoroughness, Item singleItemUpgrades, out Exception error)
        {
            if (!itemCacheInitialized) throw new InvalidOperationException("Optimization item cache was not initialized.");
            error = null;
            _character = character;
            Model = Calculations.GetModel(_character.CurrentModel);
            _calculationToOptimize = calculationToOptimize;
            _requirements = requirements;
            _thoroughness = thoroughness;

            currentOperation = OptimizationOperation.ComputeUpgrades;
            Character saveCharacter = _character;
            Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> upgrades = null;
            try
            {
                // make equipped gear/enchant valid
                MarkEquippedItemsAsValid(_character);

                upgrades = new Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>>();

                Item[] items = ItemCache.GetRelevantItems(model, _character.Race);
                CharacterSlot[] slots = new CharacterSlot[] { CharacterSlot.Back, CharacterSlot.Chest, CharacterSlot.Feet, CharacterSlot.Finger1, CharacterSlot.Hands, CharacterSlot.Head, CharacterSlot.Legs, CharacterSlot.MainHand, CharacterSlot.Neck, CharacterSlot.OffHand, CharacterSlot.Projectile, CharacterSlot.ProjectileBag, CharacterSlot.Ranged, CharacterSlot.Shoulders, CharacterSlot.Trinket1, CharacterSlot.Waist, CharacterSlot.Wrist };
                foreach (CharacterSlot slot in slots)
                    upgrades[slot] = new List<ComparisonCalculationUpgrades>();

                CharacterCalculationsBase baseCalculations = model.GetCharacterCalculations(_character);
                float baseValue = GetOptimizationValue(_character, baseCalculations);
                Dictionary<int, Item> itemById = new Dictionary<int, Item>();
                foreach (Item item in items)
                {
                    itemById[item.Id] = item;
                }

                if (singleItemUpgrades != null)
                {
                    items = new Item[] { singleItemUpgrades };
                }
                else
                {
                    items = new List<Item>(itemById.Values).ToArray();
                }

                for (int i = 0; i < items.Length; i++)
                {
                    Item item = items[i];
                    currentItem = item.Name;
                    itemProgressPercentage = (int)Math.Round((float)i / ((float)items.Length / 100f));
                    if (cancellationPending)
                    {
                        return null;
                    }
                    ReportProgress(0, 0);
                    foreach (CharacterSlot slot in slots)
                    {
                        if (item.FitsInSlot(slot, _character))
                        {
                            List<ComparisonCalculationUpgrades> comparisons = upgrades[slot];
                            PopulateLockedItems(item);
                            lockedSlot = slot;
                            if (lockedSlot == CharacterSlot.Finger1 && item.Unique && (object)_character.Finger2 != null && (_character.Finger2.Id == item.Id || (item.UniqueId != null && item.UniqueId.Contains(_character.Finger2.Id))))
                            {
                                lockedSlot = CharacterSlot.Finger2;
                            }
                            if (lockedSlot == CharacterSlot.Trinket1 && item.Unique && (object)_character.Trinket2 != null && (_character.Trinket2.Id == item.Id || (item.UniqueId != null && item.UniqueId.Contains(_character.Trinket2.Id))))
                            {
                                lockedSlot = CharacterSlot.Trinket2;
                            }
                            _character = BuildSingleItemSwapIndividual(_character, (int)lockedSlot, lockedItems[0]);
                            // instead of just putting in the first gemming on the list select the best one
                            float best = -10000000f;
                            CharacterCalculationsBase bestCalculations;
                            Character bestCharacter;
                            if (lockedItems.Count > 1)
                            {
                                Character directUpgradeCharacter = LookForDirectItemUpgrades(null, lockedSlot, best, _character, out bestCalculations).Value;
                                if (directUpgradeCharacter != null)
                                {
                                    _character = directUpgradeCharacter;
                                }
                            }
                            if (_thoroughness > 1)
                            {
                                int saveThoroughness = _thoroughness;
                                _thoroughness = 1;
                                float injectValue;
                                bool injected;
                                Character inject = Optimize(_character, 0, out injectValue, out bestCalculations, out injected);
                                _thoroughness = saveThoroughness;
                                bestCharacter = Optimize(inject, injectValue, out best, out bestCalculations, out injected);
                            }
                            else
                            {
                                bool injected;
                                bestCharacter = Optimize(_character, 0, out best, out bestCalculations, out injected);
                            }
                            if (best > baseValue)
                            {
                                ItemInstance bestItem = bestCharacter[lockedSlot];
                                ComparisonCalculationUpgrades itemCalc = new ComparisonCalculationUpgrades();
                                itemCalc.ItemInstance = bestItem;
                                itemCalc.CharacterItems = bestCharacter.GetItems();
                                itemCalc.Name = item.Name;
                                itemCalc.Equipped = false;
                                itemCalc.OverallPoints = best - baseValue;

                                comparisons.Add(itemCalc);
                            }
                            _character = saveCharacter;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                _character = saveCharacter;
            }

            ReportProgress(100, 0f);
            return upgrades;
        }

        public float EvaluateUpgrade(Character character, string calculationToOptimize, OptimizationRequirement[] requirements, int thoroughness, ItemInstance upgrade)
        {
            if (isBusy) throw new InvalidOperationException("Optimizer is working on another operation.");
            isBusy = true;
            cancellationPending = false;
            asyncOperation = null;
            Exception error;
            float upgradeValue = PrivateEvaluateUpgrade(character, calculationToOptimize, requirements, thoroughness, upgrade, out error);
            if (error != null) throw error;
            isBusy = false;
            return upgradeValue;
        }

        private float PrivateEvaluateUpgrade(Character character, string calculationToOptimize, OptimizationRequirement[] requirements, int thoroughness, ItemInstance upgrade, out Exception error)
        {
            if (!itemCacheInitialized) throw new InvalidOperationException("Optimization item cache was not initialized.");
            error = null;
            _character = character;
            Model = Calculations.GetModel(_character.CurrentModel);
            _calculationToOptimize = calculationToOptimize;
            _requirements = requirements;
            _thoroughness = thoroughness;

            currentOperation = OptimizationOperation.EvaluateUpgrade;
            Character saveCharacter = _character;
            float upgradeValue = 0f;
            try
            {
                // make equipped gear/enchant valid
                // this is currently only called after calculate upgrades already marks items as valid, but we might have to do this here also if things change
                // MarkEquippedItemsAsValid(_character);

                CharacterSlot[] slots = new CharacterSlot[] { CharacterSlot.Back, CharacterSlot.Chest, CharacterSlot.Feet, CharacterSlot.Finger1, CharacterSlot.Hands, CharacterSlot.Head, CharacterSlot.Legs, CharacterSlot.MainHand, CharacterSlot.Neck, CharacterSlot.OffHand, CharacterSlot.Projectile, CharacterSlot.ProjectileBag, CharacterSlot.Ranged, CharacterSlot.Shoulders, CharacterSlot.Trinket1, CharacterSlot.Waist, CharacterSlot.Wrist };
                CharacterCalculationsBase baseCalculations = model.GetCharacterCalculations(_character);
                float baseValue = GetOptimizationValue(_character, baseCalculations);

                ItemInstance item = upgrade;
                foreach (CharacterSlot slot in slots)
                {
                    if (item.Item.FitsInSlot(slot, _character))
                    {
                        lockedItems = new List<ItemInstance>() { item };
                        lockedSlot = slot;
                        if (lockedSlot == CharacterSlot.Finger1 && item.Item.Unique && (object)_character.Finger2 != null && (_character.Finger2.Id == item.Id || (item.Item.UniqueId != null && item.Item.UniqueId.Contains(_character.Finger2.Id))))
                        {
                            lockedSlot = CharacterSlot.Finger2;
                        }
                        if (lockedSlot == CharacterSlot.Trinket1 && item.Item.Unique && (object)_character.Trinket2 != null && (_character.Trinket2.Id == item.Id || (item.Item.UniqueId != null && item.Item.UniqueId.Contains(_character.Trinket2.Id))))
                        {
                            lockedSlot = CharacterSlot.Trinket2;
                        }
                        _character = BuildSingleItemSwapIndividual(_character, (int)lockedSlot, upgrade);
                        float best;
                        CharacterCalculationsBase bestCalculations;
                        Character bestCharacter;
                        if (_thoroughness > 1)
                        {
                            int saveThoroughness = _thoroughness;
                            _thoroughness = 1;
                            float injectValue;
                            bool injected;
                            Character inject = Optimize(_character, 0, out injectValue, out bestCalculations, out injected);
                            _thoroughness = saveThoroughness;
                            bestCharacter = Optimize(inject, injectValue, out best, out bestCalculations, out injected);
                        }
                        else
                        {
                            bool injected;
                            bestCharacter = Optimize(_character, 0, out best, out bestCalculations, out injected);
                        }
                        if ((object)bestCharacter[lockedSlot] == null || bestCharacter[lockedSlot].Id != item.Id) throw new Exception("There was an internal error in Optimizer when evaluating upgrade.");
                        upgradeValue = best - baseValue;
                        if (upgradeValue < 0 && ((object)saveCharacter[lockedSlot] == null || saveCharacter[lockedSlot].Id != item.Id)) upgradeValue = 0f;
                        _character = saveCharacter;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                _character = saveCharacter;
            }

            ReportProgress(100, 0f);
            return upgradeValue;
        }

        private bool itemCacheInitialized;

        int[] pairSlotList = new int[] { (int)CharacterSlot.Finger1, (int)CharacterSlot.MainHand, (int)CharacterSlot.Trinket1 };
        int[] pairSlotMap;
        List<ItemInstance>[] slotList;
        List<ItemInstance> lockedItems;
        CharacterSlot lockedSlot = CharacterSlot.None;
        AvailableItemGenerator itemGenerator;

        private void PopulateLockedItems(Item item)
        {
            lockedItems = itemGenerator.GetPossibleGemmedItemsForItem(item, item.Id.ToString());
        }

        private void PopulateAvailableIds(List<string> availableItems, bool templateGemsEnabled, bool overrideRegem, bool overrideReenchant)
        {
            itemGenerator = new AvailableItemGenerator(availableItems, false, templateGemsEnabled, overrideRegem, overrideReenchant, true, _character, model);
            slotList = itemGenerator.SlotItems;

            for (int i = 0; i < characterSlots; i++)
            {
                Dictionary<int, bool> itemUnique = new Dictionary<int, bool>();
                slotItems[5 * i] = new List<object>();
                foreach (ItemInstance itemInstance in slotList[i])
                {
                    int itemId = 0;
                    if (itemInstance != null)
                    {
                        itemId = itemInstance.Id;
                    }
                    if (!itemUnique.ContainsKey(itemId))
                    {
                        slotItems[5 * i].Add(itemInstance != null ? itemInstance.Item : null);
                        itemUnique[itemId] = true;
                    }
                }
            }

            pairSlotMap = new int[slotCount];
            pairSlotMap[(int)CharacterSlot.Back] = -1;
            pairSlotMap[(int)CharacterSlot.Chest] = -1;
            pairSlotMap[(int)CharacterSlot.Feet] = -1;
            pairSlotMap[(int)CharacterSlot.Finger1] = (int)CharacterSlot.Finger2;
            pairSlotMap[(int)CharacterSlot.Finger2] = (int)CharacterSlot.Finger1;
            pairSlotMap[(int)CharacterSlot.Hands] = -1;
            pairSlotMap[(int)CharacterSlot.Head] = -1;
            pairSlotMap[(int)CharacterSlot.Legs] = -1;
            pairSlotMap[(int)CharacterSlot.MainHand] = (int)CharacterSlot.OffHand;
            pairSlotMap[(int)CharacterSlot.OffHand] = (int)CharacterSlot.MainHand;
            pairSlotMap[(int)CharacterSlot.Neck] = -1;
            pairSlotMap[(int)CharacterSlot.Projectile] = -1;
            pairSlotMap[(int)CharacterSlot.ProjectileBag] = -1;
            pairSlotMap[(int)CharacterSlot.Ranged] = -1;
            pairSlotMap[(int)CharacterSlot.Shoulders] = -1;
            pairSlotMap[(int)CharacterSlot.Trinket1] = (int)CharacterSlot.Trinket2;
            pairSlotMap[(int)CharacterSlot.Trinket2] = (int)CharacterSlot.Trinket1;
            pairSlotMap[(int)CharacterSlot.Waist] = -1;
            pairSlotMap[(int)CharacterSlot.Wrist] = -1;

            itemCacheInitialized = true;
        }

        public string GetWarningPromptIfNeeded()
        {
            int gemLimit = 8;
            int itemLimit = 512;
            //int enchantLimit = 8;

            List<string> emptyList = new List<string>();
            List<string> tooManyList = new List<string>();

            CalculateWarnings(itemGenerator.GemItems, "Gems", emptyList, tooManyList, gemLimit);
            CalculateWarnings(itemGenerator.MetaGemItems, "Meta Gems", emptyList, tooManyList, gemLimit);

            CalculateWarnings(slotList[(int)CharacterSlot.Head], "Head Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotList[(int)CharacterSlot.Neck], "Neck Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotList[(int)CharacterSlot.Shoulders], "Shoulder Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotList[(int)CharacterSlot.Back], "Back Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotList[(int)CharacterSlot.Chest], "Chest Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotList[(int)CharacterSlot.Wrist], "Wrist Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotList[(int)CharacterSlot.Hands], "Hands Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotList[(int)CharacterSlot.Waist], "Waist Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotList[(int)CharacterSlot.Legs], "Legs Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotList[(int)CharacterSlot.Feet], "Feet Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotList[(int)CharacterSlot.Finger1], "Finger Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotList[(int)CharacterSlot.Trinket1], "Trinket Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotList[(int)CharacterSlot.MainHand], "Main Hand Items", emptyList, tooManyList, itemLimit);
            CalculateWarnings(slotList[(int)CharacterSlot.OffHand], "Offhand Items", null, tooManyList, itemLimit);
            CalculateWarnings(slotList[(int)CharacterSlot.Ranged], "Ranged Items", null, tooManyList, itemLimit);
            CalculateWarnings(slotList[(int)CharacterSlot.Projectile], "Projectile Items", null, tooManyList, itemLimit);
            CalculateWarnings(slotList[(int)CharacterSlot.ProjectileBag], "Projectile Bag Items", null, tooManyList, itemLimit);

            //CalculateWarnings(backEnchants, "Back Enchants", emptyList, tooManyList, enchantLimit);
            //CalculateWarnings(chestEnchants, "Chest Enchants", emptyList, tooManyList, enchantLimit);
            //CalculateWarnings(feetEnchants, "Feet Enchants", emptyList, tooManyList, enchantLimit);
            //CalculateWarnings(fingerEnchants, "Finger Enchants", null, tooManyList, enchantLimit);
            //CalculateWarnings(handsEnchants, "Hands Enchants", emptyList, tooManyList, enchantLimit);
            //CalculateWarnings(headEnchants, "Head Enchants", emptyList, tooManyList, enchantLimit);
            //CalculateWarnings(legsEnchants, "Legs Enchants", emptyList, tooManyList, enchantLimit);
            //CalculateWarnings(shouldersEnchants, "Shoulder Enchants", emptyList, tooManyList, enchantLimit);
            //CalculateWarnings(mainHandEnchants, "Main Hand Enchants", emptyList, tooManyList, enchantLimit);
            //CalculateWarnings(offHandEnchants, "Offhand Enchants", null, tooManyList, enchantLimit);
            //CalculateWarnings(rangedEnchants, "Ranged Enchants", null, tooManyList, enchantLimit);
            //CalculateWarnings(wristEnchants, "Wrist Enchants", emptyList, tooManyList, enchantLimit);

            if (emptyList.Count + tooManyList.Count > 0)
            {
                if (emptyList.Count > 5)
                {
                    emptyList.RemoveRange(5, emptyList.Count - 5);
                    emptyList.Add("...");
                }
                if (tooManyList.Count > 5)
                {
                    tooManyList.RemoveRange(5, tooManyList.Count - 5);
                    tooManyList.Add("...");
                }
                if (tooManyList.Count == 0)
                {
                    // good sizes but some are empty
                    return "You have not selected any of the following:" + Environment.NewLine + Environment.NewLine + "\t" + string.Join(Environment.NewLine + "\t", emptyList.ToArray()) + Environment.NewLine + Environment.NewLine + "Do you want to continue with the optimization?";
                }
                else if (emptyList.Count == 0)
                {
                    return "The following slots have a very large number of items selected :" + Environment.NewLine + Environment.NewLine + "\t" + string.Join(Environment.NewLine + "\t", tooManyList.ToArray()) + Environment.NewLine + Environment.NewLine + "Do you want to continue with the optimization?";
                }
                else
                {
                    return "You have not selected any of the following:" + Environment.NewLine + Environment.NewLine + "\t" + string.Join(Environment.NewLine + "\t", emptyList.ToArray()) + Environment.NewLine + Environment.NewLine + "The following slots have a very large number of items selected :" + Environment.NewLine + Environment.NewLine + "\t" + string.Join(Environment.NewLine + "\t", tooManyList.ToArray()) + Environment.NewLine + Environment.NewLine + "Do you want to continue with the optimization?";
                }
            }
            return null;
        }

        private void CalculateWarnings(System.Collections.IList list, string group, List<string> emptyList, List<string> tooManyList, int tooManyLimit)
        {
            object el0 = (list.Count > 0) ? list[0] : null;
            if (emptyList != null && (list.Count == 0 || (list.Count == 1 && (el0 == null || (el0 is Enchant && ((Enchant)el0).Id == 0))))) emptyList.Add(group);
            if (tooManyList != null && list.Count > tooManyLimit) tooManyList.Add(group);
        }

        public static float GetOptimizationValue(Character character, CalculationsBase model)
        {
            return GetCalculationsValue(character, model.GetCharacterCalculations(character), character.CalculationToOptimize, character.OptimizationRequirements.ToArray());
        }

        protected override float GetOptimizationValue(Character individual, CharacterCalculationsBase valuation)
        {
            return GetCalculationsValue(individual, valuation, _calculationToOptimize, _requirements);
        }

        protected override CharacterCalculationsBase GetValuation(Character individual)
        {
            bool oldVolatility = Item.OptimizerManagedVolatiliy;
            try
            {
                Item.OptimizerManagedVolatiliy = true;
                return model.GetCharacterCalculations(individual);
            }
            finally
            {
                Item.OptimizerManagedVolatiliy = oldVolatility;
            }
        }

        protected override object GetItem(Character individual, int slot)
        {
            int characterSlot = slot / 5;
            ItemInstance itemInstance = individual[(CharacterSlot)characterSlot];
            if (itemInstance == null) return null;
            switch (slot % 5)
            {
                case 0:
                    return itemInstance.Item;
                case 1:
                    return itemInstance.Gem1;
                case 2:
                    return itemInstance.Gem2;
                case 3:
                    return itemInstance.Gem3;
                case 4:
                    return itemInstance.Enchant;
            }
            return null;
        }

        protected override object[] GetItems(Character individual)
        {
            object[] items = new object[slotCount];
            for (int i = 0; i < characterSlots; i++)
            {
                ItemInstance itemInstance = individual[(CharacterSlot)i];
                if (itemInstance != null)
                {
                    items[i * 5] = itemInstance.Item;
                    items[i * 5 + 1] = itemInstance.Gem1;
                    items[i * 5 + 2] = itemInstance.Gem2;
                    items[i * 5 + 3] = itemInstance.Gem3;
                    items[i * 5 + 4] = itemInstance.Enchant;
                }
            }
            return items;
        }

        protected override Character GenerateIndividual(object[] items, bool canUseArray, Character recycledIndividual)
        {
            Item[] gems = new Item[3];
            ItemInstance[] itemInstances = new ItemInstance[characterSlots];
            for (int i = 0; i < characterSlots; i++)
            {
                Item item = items[i * 5] as Item;
                if (item != null)
                {
                    Array.Clear(gems, 0, 3);
                    for (int j = 0; j < item.AvailabilityInformation.GemCount; j++)
                    {
                        gems[j] = items[i * 5 + 1 + j] as Item;
                    }
                    itemInstances[i] = new ItemInstance(item, gems[0], gems[1], gems[2], items[i * 5 + 4] as Enchant);
                }
            }
            Character character = new Character(_character.Name, _character.Realm, _character.Region, _character.Race, _character.BossOptions,
                itemInstances, _character.ActiveBuffs, _character.CurrentModel);
            character.CalculationOptions = _character.CalculationOptions;
            character.BossOptions = _character.BossOptions;
            character.Class = _character.Class;
            character.AssignAllTalentsFromCharacter(_character, false);
            character.EnforceGemRequirements = _character.EnforceGemRequirements;
            //character.RecalculateSetBonuses();
            return character;
        }

        private static float GetCalculationsValue(Character character, CharacterCalculationsBase calcs, string calculation, OptimizationRequirement[] requirements)
        {
            float gemValue = -100000 * character.GemRequirementsInvalid;
            float ret = 0;
            foreach (OptimizationRequirement requirement in requirements)
            {
                float calcValue = GetCalculationValue(character, calcs, requirement.Calculation);
                if (requirement.LessThan)
                {
                    if (!(calcValue <= requirement.Value))
                        ret += requirement.Value - calcValue;
                }
                else
                {
                    if (!(calcValue >= requirement.Value))
                        ret += calcValue - requirement.Value;
                }
            }

            if (ret < 0) return ret + gemValue;
            else return GetCalculationValue(character, calcs, calculation) + gemValue;
        }

        private static float GetCalculationValue(Character character, CharacterCalculationsBase calcs, string calculation)
        {
            if (calculation == null || calculation == "[Overall]")
            {
                return calcs.OverallPoints;
            }
            else if (calculation.StartsWith("[SubPoint ", StringComparison.Ordinal))
            {
                return calcs.SubPoints[int.Parse(calculation.Substring(10).TrimEnd(']'))];
            }
            else if (calculation.StartsWith("[Talent ", StringComparison.Ordinal))
            {
                return character.CurrentTalents.Data[int.Parse(calculation.Substring(8).TrimEnd(']'))];
            }
            else if (calculation.StartsWith("[Glyph ", StringComparison.Ordinal))
            {
                return character.CurrentTalents.GlyphData[int.Parse(calculation.Substring(7).TrimEnd(']'))] ? 1 : 0;
            }
            else
            {
                return calcs.GetOptimizableCalculationValue(calculation);
            }
        }

        protected override object GetRandomItem(int slot, object[] items)
        {
            int characterSlot = slot / 5;
            if (lockedSlot == (CharacterSlot)characterSlot)
            {
                ItemInstance itemInstance = lockedItems[Rnd.Next(lockedItems.Count)];
                switch (slot % 5)
                {
                    case 0:
                        return itemInstance != null ? itemInstance.Item : null;
                    case 1:
                        return itemInstance != null ? itemInstance.Gem1 : null;
                    case 2:
                        return itemInstance != null ? itemInstance.Gem2 : null;
                    case 3:
                        return itemInstance != null ? itemInstance.Gem3 : null;
                    case 4:
                        return itemInstance != null ? itemInstance.Enchant : null;
                }
            }
            else
            {
                Item item;
                switch (slot % 5)
                {
                    case 0:
                        return base.GetRandomItem(slot, items);
                    case 1:
                        item = items[5 * characterSlot] as Item;
                        if (item != null)
                        {
                            List<ItemInstance> list = item.AvailabilityInformation.ItemList;
                            return list[Rnd.Next(list.Count)].Gem1;
                        }
                        else
                        {
                            return null;
                        }
                    case 2:
                        item = items[5 * characterSlot] as Item;
                        if (item != null)
                        {
                            List<ItemInstance> list = item.AvailabilityInformation.ItemList;
                            return list[Rnd.Next(list.Count)].Gem2;
                        }
                        else
                        {
                            return null;
                        }
                    case 3:
                        item = items[5 * characterSlot] as Item;
                        if (item != null)
                        {
                            List<ItemInstance> list = item.AvailabilityInformation.ItemList;
                            return list[Rnd.Next(list.Count)].Gem3;
                        }
                        else
                        {
                            return null;
                        }
                    case 4:
                        item = items[5 * characterSlot] as Item;
                        if (item != null)
                        {
                            List<ItemInstance> list = item.AvailabilityInformation.ItemList;
                            return list[Rnd.Next(list.Count)].Enchant;
                        }
                        else
                        {
                            return null;
                        }
                }
                return null;
            }
            return null;
        }

        protected override KeyValuePair<float, Character> LookForDirectItemUpgrades(List<object> items, int slot, float best, Character bestIndividual, object[] itemList, out CharacterCalculationsBase bestValuation)
        {
            if (slot % 5 != 0)
            {
                bestValuation = null;
                return new KeyValuePair<float, Character>(float.NegativeInfinity, null);
            }
            int characterSlot = slot / 5;
            return LookForDirectItemUpgrades(slotList[characterSlot], (CharacterSlot)characterSlot, best, bestIndividual, out bestValuation);
        }

        private KeyValuePair<float, Character> LookForDirectItemUpgrades(List<ItemInstance> items, CharacterSlot slot, float best, Character bestCharacter, out CharacterCalculationsBase bestCalculations)
        {
            Character charSwap;
            bestCalculations = null;
            float value;
            bool foundUpgrade = false;
            if (slot == lockedSlot) items = lockedItems;
            ItemInstance[] itemList = (ItemInstance[])bestCharacter._item.Clone();
            foreach (ItemInstance item in items)
            {
                int pairSlot = pairSlotMap[(int)slot];
                if ((object)item != null && ((object)bestCharacter[slot] == null || bestCharacter[slot].GemmedId != item.GemmedId) && !(pairSlot >= 0 && (object)bestCharacter[(CharacterSlot)pairSlot] != null && item.Item.Unique && (bestCharacter[(CharacterSlot)pairSlot].Id == item.Id || (item.Item.UniqueId != null && item.Item.UniqueId.Contains(bestCharacter[(CharacterSlot)pairSlot].Id)))))
                {
                    itemList[(int)slot] = item;
                    charSwap = new Character(_character.Name, _character.Realm, _character.Region, _character.Race, _character.BossOptions,
                        itemList, _character.ActiveBuffs, _character.CurrentModel);
                    charSwap.CalculationOptions = _character.CalculationOptions;
                    charSwap.BossOptions = _character.BossOptions;
                    charSwap.Class = _character.Class;
                    charSwap.AssignAllTalentsFromCharacter(_character, false);
                    charSwap.EnforceGemRequirements = _character.EnforceGemRequirements;
                    CharacterCalculationsBase calculations;
                    value = GetOptimizationValue(charSwap, calculations = model.GetCharacterCalculations(charSwap));
                    if (value > best)
                    {
                        best = value;
                        bestCalculations = calculations;
                        bestCharacter = charSwap;
                        foundUpgrade = true;
                    }
                }
            }
            if (foundUpgrade)
                return new KeyValuePair<float, Character>(best, bestCharacter);
            return new KeyValuePair<float, Character>(float.NegativeInfinity, null);
        }
    }
#endregion
}
