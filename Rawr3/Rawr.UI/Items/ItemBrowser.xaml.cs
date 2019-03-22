using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace Rawr.UI
{
    public partial class ItemBrowser : ChildWindow
    {

        private ItemEditor itemEditor;
        public ItemEditor ItemEditor
        {
            get
            {
                if (itemEditor == null)
                {
                    itemEditor = new ItemEditor();
                    itemEditor.Closed += new EventHandler(ItemEditor_Closed);
                }
                return itemEditor;
            }
        }

        private EnterId enterId;
        public EnterId EnterId
        {
            get
            {
                if (enterId == null)
                {
                    enterId = new EnterId();
                    enterId.Closed += new EventHandler(EnterId_Closed);
                }
                return enterId;
            }
        }

        public ItemBrowser()
        {
            InitializeComponent();
            ItemCache.Instance.ItemsChanged += new EventHandler(Instance_ItemsChanged);
            UpdateItemList();
        }

        private void Instance_ItemsChanged(object sender, EventArgs e)
        {
            UpdateItemList();
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void UpdateItemList()
        {
            IEnumerable<Item> items = ItemCache.AllItems;
            if (!string.IsNullOrEmpty(NameText.Text)) items = items.Where(i =>
                i.Name.ToLower().Contains(NameText.Text.ToLower()));

            if (QualityList.SelectedItems.Count > 0)
            {
                List<ItemQuality> qualities = new List<ItemQuality>();
                foreach (ListBoxItem lbi in QualityList.SelectedItems)
                {
                    qualities.Add((ItemQuality)Enum.Parse(typeof(ItemQuality), lbi.Content.ToString(), false));
                    items = items.Where(i => qualities.Contains(i.Quality));
                }
            }

            if (TypeList.SelectedItems.Count > 0)
            {
                List<ItemType> types = new List<ItemType>();
                foreach (ListBoxItem lbi in TypeList.SelectedItems)
                {
                    types.Add((ItemType)Enum.Parse(typeof(ItemType), lbi.Content.ToString(), false));
                    items = items.Where(i => types.Contains(i.Type));
                }
            }

            if (SlotList.SelectedItems.Count > 0)
            {
                List<ItemSlot> slots = new List<ItemSlot>();
                foreach (ListBoxItem lbi in SlotList.SelectedItems)
                {
                    slots.Add((ItemSlot)Enum.Parse(typeof(ItemSlot), lbi.Content.ToString().Replace(" ", ""), false));
                    items = items.Where(i => slots.Contains(i.Slot));
                }
            }

            if (ClassCombo.SelectedIndex > 0)
            {
                string reqClass = ((CharacterClass)ClassCombo.SelectedIndex).ToString();
                items = items.Where(i => string.IsNullOrEmpty(i.RequiredClasses)
                    || i.RequiredClasses.Split('|').Contains(reqClass));
            }

            if (!string.IsNullOrEmpty(MinLevelText.Text))
            {
                try
                {
                    int min = int.Parse(MinLevelText.Text);
                    items = items.Where(i => i.ItemLevel >= min);
                }
                catch { }
            }

            if (!string.IsNullOrEmpty(MaxLevelText.Text))
            {
                try
                {
                    int max = int.Parse(MaxLevelText.Text);
                    items = items.Where(i => i.ItemLevel <= max);
                }
                catch { }
            }

#if SILVERLIGHT
			PagedCollectionView itemsPCV = new PagedCollectionView(items);
			itemsPCV.GroupDescriptions.Add(new PropertyGroupDescription("Slot"));
            ItemGrid.ItemsSource = itemsPCV;            
#else
            // TODO implement paging in WPF
            ItemGrid.ItemsSource = items;
#endif
        }

        private void AddButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EnterId.Show();
        }

        void EnterId_Closed(object sender, EventArgs e)
        {
            if (EnterId.DialogResult.GetValueOrDefault(false))
            {
                //_changingItemCache = true;
                try
                {
                    //WebRequestWrapper.ResetFatalErrorIndicator();
                    int itemId = EnterId.Value;
                    if (itemId > 0)
                        AddItemById(EnterId.Value, true, true);
                    else
                        AddItemByName(EnterId.ItemName, true, true);
                }
                finally
                {
                    //_changingItemCache = false;
                }
            }
        }

        private void AddItemByName(string name, bool useArmory, bool useWowhead)
        {
            // ignore empty strings
            if (name.Length <= 0) return;

            // try the armory (if requested)
            //if (useArmory)
            //{
                //TODO-ARMORY-ITEM: Armory.GetItemIdByName(name, armoryId => AddItemByName(name, armoryId, useWowhead));
            //}
            else
            {
                AddItemByName(name, 0, useWowhead);
            }
        }

        private void AddItemByName(string name, int armoryId, bool useWowhead)
        {
            Item newItem = null;

            // try the armory (if requested)
            if (armoryId > 0)
            {
                newItem = Item.LoadFromId(armoryId, true, true, false);
            }

            // try wowhead (if requested)
            // TODO add back when we add wowhead support
            /*if ((newItem == null) && useWowhead)
            {
                // make sure we don't get some bad input that is going to mess with our gem info passing
                if (!name.Contains("."))
                {
                    // need to add + where the spaces are
                    string wowhead_name = name.Replace(' ', '+');
                    // we can now pass it through the normal URI
                    newItem = Wowhead.GetItem(wowhead_name + ".0.0.0", true);
                    if (newItem != null) ItemCache.AddItem(newItem, true);
                }
            }*/

            if (newItem == null)
            {
                MessageBox.Show("Unable to load item: " + name + ".", "Item not found.", MessageBoxButton.OK);
            }
            else
            {
                UpdateItemList();
                // TODO this does not work, figure out how to select an item
                ItemGrid.SelectedItem = newItem;
            }
        }

        private void AddItemById(int id, bool useArmory, bool useWowhead) { AddItemsById(new int[] { id }, useArmory, useWowhead); }
        private void AddItemsById(int[] ids, bool useArmory, bool useWowhead)
        {
            foreach (int id in ids)
            {
                Item newItem = null;

                // try the armory (if requested)
                if (useArmory)
                {
                    newItem = Item.LoadFromId(id, true, true, false);
                }

                // try wowhead (if requested)
                /*if ((newItem == null) && useWowhead)
                {
                    newItem = Wowhead.GetItem(id.ToString(), true);
                    if (newItem != null) ItemCache.AddItem(newItem, true);
                }
                if ((newItem == null) && useWowhead)
                {
                    newItem = Wowhead.GetItem("ptr", id.ToString(), true);
                    if (newItem != null) ItemCache.AddItem(newItem, true);
                }*/

                if (newItem == null)
                {
                    // TODO don't use message box
                    if (MessageBox.Show("Unable to load item " + id.ToString() + ". Would you like to create the item blank and type in the values yourself?", "Item not found. Create Blank?", MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        newItem = new Item("New Item", ItemQuality.Epic, ItemType.None, id, "temp", ItemSlot.Head, string.Empty, false, new Stats(), new Stats(), ItemSlot.None, ItemSlot.None, ItemSlot.None, 0, 0, ItemDamageType.Physical, 0f, string.Empty);
                        ItemCache.AddItem(newItem);
                    }
                }
                else
                {
                    UpdateItemList();
                    // TODO this does not work, figure out how to select an item
                    ItemGrid.SelectedItem = newItem;
                }
            }
        }

        private void EditButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Item i = ItemGrid.SelectedItem as Item;
            if (i != null)
            {
                ItemEditor.CurrentItem = i;
                ItemEditor.Show();
            }
        }

        private void ItemEditor_Closed(object sender, EventArgs e)
        {
            if (((ChildWindow)sender).DialogResult.GetValueOrDefault(false))
            {
                UpdateItemList();
            }
        }

        private void ItemGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Item i = ItemGrid.SelectedItem as Item;
            if (i == null)
            {
                EditButton.IsEnabled = false;
                RefreshButton.IsEnabled = false;
            }
            else
            {
                EditButton.IsEnabled = true;
                RefreshButton.IsEnabled = true;
            }
        }

        private void NameText_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateItemList();
        }

        private void RefreshButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Item i = ItemGrid.SelectedItem as Item;
            if (i != null)
            {
                Item.LoadFromId(i.Id, true, false, false);
            }
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateItemList();
        }

        private void LevelChanged(object sender, TextChangedEventArgs e)
        {
            UpdateItemList();
        }
    }
}

