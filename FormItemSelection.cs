using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Rawr
{
	public partial class FormItemSelection : Form
	{
		private Character _character;
		public Character Character
		{
			get { return _character; }
			set
			{
                if (_character != null)
                {
                    _character.CalculationsInvalidated -= new EventHandler(_character_ItemsChanged);
                }
                _character = value;
                _characterSlot = CharacterSlot.None;
                if (_character != null)
                {
                    _character.CalculationsInvalidated += new EventHandler(_character_ItemsChanged);
                }
			}
		}

		private CharacterCalculationsBase _currentCalculations;
		public CharacterCalculationsBase CurrentCalculations
		{
			get { return _currentCalculations; }
			set { _currentCalculations = value; }
		}

		private ComparisonCalculationBase[] _itemCalculations;
		public ComparisonCalculationBase[] ItemCalculations
		{
			get { return _itemCalculations; }
			set
			{
				if (value == null)
				{
					_itemCalculations = new ComparisonCalculationBase[0];
				}
				else
				{
					List<ComparisonCalculationBase> calcs = new List<ComparisonCalculationBase>(value);
					calcs.Sort(new System.Comparison<ComparisonCalculationBase>(CompareItemCalculations));
					_itemCalculations = calcs.ToArray();
				}
				RebuildItemList();
			}
		}

		private ComparisonGraph.ComparisonSort _sort = ComparisonGraph.ComparisonSort.Overall;
		public ComparisonGraph.ComparisonSort Sort
		{
			get { return _sort; }
			set
			{
				_sort = value;
				ItemCalculations = ItemCalculations;
			}
		}

		protected int CompareItemCalculations(ComparisonCalculationBase a, ComparisonCalculationBase b)
		{
			if (Sort == ComparisonGraph.ComparisonSort.Overall)
				return a.OverallPoints.CompareTo(b.OverallPoints);
			else if (Sort == ComparisonGraph.ComparisonSort.Alphabetical)
				return b.Name.CompareTo(a.Name);
			else
				return a.SubPoints[(int)Sort].CompareTo(b.SubPoints[(int)Sort]);
		}

		public FormItemSelection()
		{
			InitializeComponent();

			overallToolStripMenuItem.Tag = -1;
			alphabeticalToolStripMenuItem.Tag = -2;

			Calculations_ModelChanged(null, null);

			this.Activated += new EventHandler(FormItemSelection_Activated);
            ItemCache.Instance.ItemsChanged += new EventHandler(ItemCache_ItemsChanged);
			Calculations.ModelChanged += new EventHandler(Calculations_ModelChanged);
		}

		void Calculations_ModelChanged(object sender, EventArgs e)
		{
			_characterSlot = CharacterSlot.None;

			sortToolStripMenuItem_Click(overallToolStripMenuItem, EventArgs.Empty);
			toolStripDropDownButtonSort.DropDownItems.Clear();
			toolStripDropDownButtonSort.DropDownItems.Add(overallToolStripMenuItem);
			toolStripDropDownButtonSort.DropDownItems.Add(alphabeticalToolStripMenuItem);

			foreach (string name in Calculations.SubPointNameColors.Keys)
			{
				ToolStripMenuItem toolStripMenuItemSubPoint = new ToolStripMenuItem(name);
				toolStripMenuItemSubPoint.Tag = toolStripDropDownButtonSort.DropDownItems.Count - 2;
				toolStripMenuItemSubPoint.Click += new System.EventHandler(this.sortToolStripMenuItem_Click);
				toolStripDropDownButtonSort.DropDownItems.Add(toolStripMenuItemSubPoint);
			}
		}

		void ItemCache_ItemsChanged(object sender, EventArgs e)
		{
			CharacterSlot characterSlot = _characterSlot;
			_characterSlot = CharacterSlot.None;
			LoadItemsBySlot(characterSlot);
		}

        void _character_ItemsChanged(object sender, EventArgs e)
        {
            _characterSlot = CharacterSlot.None;
        }
        
        void FormItemSelection_Activated(object sender, EventArgs e)
		{
			panelItems.AutoScrollPosition = new Point(0, 0);
		}

		private void CheckToHide(object sender, EventArgs e)
		{
			if (Visible)
			{
                ItemToolTip.Instance.Hide(this);
                this.Hide();
			}
		}

		private void timerForceActivate_Tick(object sender, System.EventArgs e)
		{
			this.timerForceActivate.Enabled = false;
			this.Activate();
		}

		public void ForceShow()
		{
			this.timerForceActivate.Enabled = true;
		}

		public void Show(ItemButton button, CharacterSlot slot)
		{
			_button = button;
			if (_buttonEnchant != null)
			{
				_characterSlot = CharacterSlot.None;
				_buttonEnchant = null;
			}
			this.SetAutoLocation(button);
			this.LoadItemsBySlot(slot);
			base.Show();
		}

		public void Show(ItemButtonWithEnchant button, CharacterSlot slot)
		{
			_buttonEnchant = button;
			if (_button != null)
			{
				_characterSlot = CharacterSlot.None;
				_button = null;
			}
			this.SetAutoLocation(button);
			this.LoadEnchantsBySlot(slot);
			base.Show();
		}

		public void SetAutoLocation(Control ctrl)
		{
			Point ctrlScreen = ctrl.Parent.PointToScreen(ctrl.Location);
			Point location = new Point(ctrlScreen.X + ctrl.Width, ctrlScreen.Y);
			Rectangle workingArea = System.Windows.Forms.Screen.GetWorkingArea(ctrl.Parent);
			if (location.X < workingArea.Left)
				location.X = workingArea.Left;
			if (location.Y < workingArea.Top)
				location.Y = workingArea.Top;
			if (location.X + this.Width > workingArea.Right)
				location.X = ctrlScreen.X - this.Width;
			if (location.Y + this.Height > workingArea.Bottom)
				location.Y = workingArea.Bottom - this.Height;
			this.Location = location;
		}

		private void sortToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			ComparisonGraph.ComparisonSort sort = ComparisonGraph.ComparisonSort.Overall;
			foreach (ToolStripItem item in toolStripDropDownButtonSort.DropDownItems)
			{
				if (item is ToolStripMenuItem)
				{
					(item as ToolStripMenuItem).Checked = item == sender;
					if ((item as ToolStripMenuItem).Checked)
					{
						toolStripDropDownButtonSort.Text = item.Text;
						sort = (ComparisonGraph.ComparisonSort)((int)item.Tag);
					}
				}
			}
			this.Sort = sort;//(ComparisonGraph.ComparisonSort)Enum.Parse(typeof(ComparisonGraph.ComparisonSort), toolStripDropDownButtonSort.Text);
			this.Cursor = Cursors.Default;
		}

		private CharacterSlot _characterSlot = CharacterSlot.None;
		private ItemButton _button;
		private ItemButtonWithEnchant _buttonEnchant;
		public void LoadItemsBySlot(CharacterSlot slot)
		{
			if (_characterSlot != slot)
			{
				_characterSlot = slot;
				List<ComparisonCalculationBase> itemCalculations = new List<ComparisonCalculationBase>();
                if ((int)slot >= 0 && (int)slot <= 20)
                {
                    if (this.Character != null)
                    {
                        bool seenEquippedItem = Character[slot] == null;
                        foreach (ItemInstance item in Character.GetRelevantItemInstances(slot))
                        {
                            if (!seenEquippedItem && Character[slot].Equals(item)) seenEquippedItem = true;
                            if (item.Item.FitsInSlot(slot, Character))
                            {
                                itemCalculations.Add(Calculations.GetItemCalculations(item, this.Character, slot));
                            }
                        }
                        if (!seenEquippedItem)
                        {
                            itemCalculations.Add(Calculations.GetItemCalculations(Character[slot], this.Character, slot));
                        }
                    }
                    ComparisonCalculationBase emptyCalcs = Calculations.CreateNewComparisonCalculation();
                    emptyCalcs.Name = "Empty";
                    emptyCalcs.Item = new Item();
                    emptyCalcs.Item.Name = "Empty";
                    emptyCalcs.ItemInstance = new ItemInstance();
                    emptyCalcs.ItemInstance.Id = -1;
                    emptyCalcs.Equipped = this.Character[slot] == null;
                    itemCalculations.Add(emptyCalcs);
                }
                else
                {
                    if (this.Character != null)
                    {
                        bool seenEquippedItem = false;
                        if (_button != null && _button.SelectedItem == null)
                        {
                            seenEquippedItem = true;
                        }
                        foreach (Item item in Character.GetRelevantItems(slot))
                        {
                            if (!seenEquippedItem && _button != null && item.Equals(_button.SelectedItem)) seenEquippedItem = true;
                            if (item.FitsInSlot(slot, Character))
                            {
                                if(slot == CharacterSlot.Gems)
                                    AddGemToItemCalculations(itemCalculations, item);
                                else
                                    itemCalculations.Add(Calculations.GetItemCalculations(item, this.Character, slot));
                            }
                        }
                        if (!seenEquippedItem && _button != null && _button.SelectedItem != null)
                        {
                            itemCalculations.Add(Calculations.GetItemCalculations(_button.SelectedItem, this.Character, slot));
                        }
                    }
                    ComparisonCalculationBase emptyCalcs = Calculations.CreateNewComparisonCalculation();
                    emptyCalcs.Name = "Empty";
                    emptyCalcs.Item = new Item();
                    emptyCalcs.Item.Name = "Empty";
                    emptyCalcs.ItemInstance = new ItemInstance();
                    emptyCalcs.ItemInstance.Id = -1;
                    if (_button != null && _button.SelectedItem != null)
                    {
                        emptyCalcs.Equipped = _button.SelectedItem == null;
                    }
                    else
                    {
                        emptyCalcs.Equipped = false;
                    }
                    itemCalculations.Add(emptyCalcs);
                }
				itemCalculations.Sort(new System.Comparison<ComparisonCalculationBase>(CompareItemCalculations));
                Dictionary<int, int> countItem = new Dictionary<int, int>();
                List<ComparisonCalculationBase> filteredItemCalculations = new List<ComparisonCalculationBase>();

                for (int i = itemCalculations.Count - 1; i >= 0; i--)
                //foreach (ComparisonCalculationBase itemCalculation in itemCalculations)
                {
                    ComparisonCalculationBase itemCalculation = itemCalculations[i];
                    int itemId = (itemCalculation.ItemInstance == null ? itemCalculation.Item.Id : itemCalculation.ItemInstance.Id);
                    if (!countItem.ContainsKey(itemId)) countItem.Add(itemId, 0);
                    if (countItem[itemId]++ < Properties.GeneralSettings.Default.CountGemmingsShown ||
                        itemCalculation.Equipped || itemCalculation.ItemInstance.ForceDisplay)
                    {
                        filteredItemCalculations.Add(itemCalculation);
                    }
                }
                ItemCalculations = filteredItemCalculations.ToArray();
			}
		}

        private void AddGemToItemCalculations(List<ComparisonCalculationBase> itemCalculations, Item item)
        {
            if (item.IsJewelersGem)
            {
                if (!Rawr.Properties.GeneralSettings.Default.HideProfEnchants || this.Character.HasProfession(Profession.Jewelcrafting))
                {
                    if (Character.JewelersGemCount < 3)
                        itemCalculations.Add(Calculations.GetItemCalculations(item, this.Character, CharacterSlot.Gems));
                    else
                    {
                        Item nullItem = item.Clone();
                        nullItem.Stats = new Stats();
                        itemCalculations.Add(Calculations.GetItemCalculations(nullItem, this.Character, CharacterSlot.Gems));
                    }
                }
            }
            else if (item.IsLimitedGem)
            {
                if (!Character.IsUniqueGemEquipped(item))
                    itemCalculations.Add(Calculations.GetItemCalculations(item, this.Character, CharacterSlot.Gems));
                else
                {
                    Item nullItem = item.Clone();
                    nullItem.Stats = new Stats();
                    itemCalculations.Add(Calculations.GetItemCalculations(nullItem, this.Character, CharacterSlot.Gems));
                }
            }
            else
                itemCalculations.Add(Calculations.GetItemCalculations(item, this.Character, CharacterSlot.Gems));
        }

		private void LoadEnchantsBySlot(CharacterSlot slot)
		{
			if (_characterSlot != slot)
			{
				_characterSlot = slot;
				List<ComparisonCalculationBase> itemCalculations = null;
				if (Character != null && CurrentCalculations != null)
				{
					itemCalculations = Calculations.GetEnchantCalculations(Item.GetItemSlotByCharacterSlot(slot), Character, CurrentCalculations, false);
                }
                itemCalculations.Sort(new System.Comparison<ComparisonCalculationBase>(CompareItemCalculations));
                ItemCalculations = itemCalculations.ToArray();
			}
		}

		private List<ItemSelectorItem> _itemSelectorItems = new List<ItemSelectorItem>();
		private void RebuildItemList()
		{
            if (this.InvokeRequired)
            {
                Invoke((MethodInvoker)RebuildItemList);
                //InvokeHelper.Invoke(this, "RebuildItemList", null);
                return;
            }
			panelItems.SuspendLayout();

			////Replaced this...
			//while (panelItems.Controls.Count < this.ItemCalculations.Length)
			//	panelItems.Controls.Add(new ItemSelectorItem());
			//while (panelItems.Controls.Count > this.ItemCalculations.Length)
			//	panelItems.Controls.RemoveAt(panelItems.Controls.Count - 1);

			////...with this, so as not to constantly create and remove lots of controls
			int currentItemLength = panelItems.Controls.Count;
			int targetItemLength = this.ItemCalculations.Length;
			if (currentItemLength < targetItemLength)
			{
				int itemSelectorsToCreate = targetItemLength - _itemSelectorItems.Count;
                for (int i = 0; i < itemSelectorsToCreate; i++)
                {
                    _itemSelectorItems.Add(new ItemSelectorItem());
                }
                for (int i = currentItemLength; i < targetItemLength; i++)
                {
                    panelItems.Controls.Add(_itemSelectorItems[i]);
                }
			}
			else if (currentItemLength > targetItemLength)
			{
                for (int i = currentItemLength; i > targetItemLength; i--)
                {
                    panelItems.Controls.RemoveAt(i - 1);
                }
			}

			float maxRating = 0;
            for (int i = 0; i < this.ItemCalculations.Length; i++)
			{
				ItemSelectorItem ctrl = panelItems.Controls[i] as ItemSelectorItem;
                ComparisonCalculationBase calc = this.ItemCalculations[i];
				if (_button != null)
				{
					calc.Equipped = (calc.ItemInstance == _button.SelectedItemInstance && calc.Item == _button.SelectedItem) || (calc.Item.Id == 0 && _button.SelectedItem == null);
					ctrl.IsEnchant = false;
				}
				if (_buttonEnchant != null)
				{
					calc.Equipped = Math.Abs(calc.Item.Id % 10000) == _buttonEnchant.SelectedEnchantId;
					ctrl.IsEnchant = true;
				}
				ctrl.ItemCalculation = calc;
				ctrl.Character = Character;
				ctrl.CharacterSlot = _characterSlot;
				ctrl.Sort = this.Sort;
				ctrl.HideToolTip();
				bool visible = string.IsNullOrEmpty(this.toolStripTextBoxFilter.Text) || calc.Name.ToLower().Contains(this.toolStripTextBoxFilter.Text.ToLower());
				ctrl.Visible = visible;
				if (visible)
				{
					float calcRating;
                    if (Sort == ComparisonGraph.ComparisonSort.Overall || this.Sort == ComparisonGraph.ComparisonSort.Alphabetical)
                    {
                        calcRating = calc.OverallPoints;
                    }
                    else
                    {
                        calcRating = calc.SubPoints[(int)Sort];
                    }
					maxRating = Math.Max(maxRating, calcRating);
				}
			}
			panelItems.ResumeLayout(true);
            foreach (ItemSelectorItem ctrl in panelItems.Controls)
            {
                ctrl.MaxRating = maxRating;
            }
		}

		private void toolStripTextBoxFilter_TextChanged(object sender, EventArgs e)
		{
			RebuildItemList();
		}

		public void Select(ItemInstance item)
		{
			if (_button != null)
				_button.SelectedItemInstance = item == null ? null : item.Clone();
            //if (_buttonEnchant != null)
            //{
            //    ItemInstance copy = _buttonEnchant.SelectedItem.Clone();
            //    copy.EnchantId = item == null ? 0 : Math.Abs(item.Id % 10000);
            //    _buttonEnchant.SelectedItem = copy;
            //}
			_characterSlot = CharacterSlot.None;
			ItemToolTip.Instance.Hide(this);
			this.Hide();
		}

        public void Select(Item item)
        {
            if (_button != null)
                _button.SelectedItem = item;
            if (_buttonEnchant != null)
            {
                if(_buttonEnchant.SelectedItem != null)
                {
                    ItemInstance copy = _buttonEnchant.SelectedItem.Clone();
                    copy.EnchantId = item == null ? 0 : Math.Abs(item.Id % 10000);
                    _buttonEnchant.SelectedItem = copy;
                }
            }
            _characterSlot = CharacterSlot.None;
            ItemToolTip.Instance.Hide(this);
            this.Hide();
        }
    }

	public interface IFormItemSelectionProvider
	{
		FormItemSelection FormItemSelection { get; }
	}
}
