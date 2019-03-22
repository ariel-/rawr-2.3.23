using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr
{
	public partial class FormItemEditor : Form, IFormItemSelectionProvider
	{
		private bool _firstLoad = true;
        private bool _loadingItem = false;

		private FormItemSelection _formItemSelection;
		public FormItemSelection FormItemSelection
		{
			get
			{
				if (_formItemSelection == null || _formItemSelection.IsDisposed)
				{
					_formItemSelection = new FormItemSelection();
					_formItemSelection.Character = FormMain.Instance.FormItemSelection.Character;
				}
				return _formItemSelection;
			}
		}

        private int selectedId;
		private ListViewItem _selectedItem;
        public ListViewItem SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				if (_selectedItem != null)
				{

					Item oldItem = _selectedItem.Tag as Item;
                    oldItem.InvalidateCachedData();
					_selectedItem.Text = oldItem.Name;
					//oldItem.IdsChanged -= new EventHandler(Item_IdsChanged);
					string slot = oldItem.Slot.ToString();
					if (oldItem.Slot == ItemSlot.Red || oldItem.Slot == ItemSlot.Orange || oldItem.Slot == ItemSlot.Yellow
						 || oldItem.Slot == ItemSlot.Green || oldItem.Slot == ItemSlot.Blue || oldItem.Slot == ItemSlot.Purple
						 || oldItem.Slot == ItemSlot.Prismatic || oldItem.Slot == ItemSlot.Meta) slot = "Gems";
					_selectedItem.Group = listViewItems.Groups["listViewGroup" + slot];
					//_selectedItem.ImageKey = oldItem.IconPath;
					listViewItems.Sort();
                    if (oldItem.Id != selectedId)
                    {
                        // we changed the id of the item, sanitize the item cache
                        // we're not allowed to keep lock while adding the item since it triggers event that
                        // will most likely require a lock on it also, resulting in deadlock
                        // we only need to keep lock while removing the item since the add item already
                        // locks when modifying the dictionary
                        _changingItemCache = true;
                        lock (ItemCache.Items)
                        {
                            ItemCache.Items.Remove(selectedId); // clean the entry at old id
                        }
                        ItemCache.AddItem(oldItem); // insert it at the new id and clear whatever was at that id before
                        _changingItemCache = false;
                    }
				}

				_selectedItem = value;

				Item selectedItem = _selectedItem.Tag as Item;
                selectedId = selectedItem.Id;
                selectedItem.InvalidateCachedData();
                if (selectedItem.IsGem) ItemCache.InvalidateCachedStats();
				//selectedItem.IdsChanged += new EventHandler(Item_IdsChanged);
				//_equippedSlots = _character.GetEquippedSlots(selectedItem);
					
				textBoxName.DataBindings.Clear();
                textBoxSetName.DataBindings.Clear();
                textBoxItemLevel.DataBindings.Clear();
                textBoxIcon.DataBindings.Clear();
                textBoxNote.DataBindings.Clear();
                textBoxCost.DataBindings.Clear();

				numericUpDownId.DataBindings.Clear();
				numericUpDownMin.DataBindings.Clear();
				numericUpDownMax.DataBindings.Clear();
				numericUpDownSpeed.DataBindings.Clear();
                numericUpDownBonus1.DataBindings.Clear();
                checkBoxUnique.DataBindings.Clear();
                comboBoxDamageType.DataBindings.Clear();
                comboBoxSlot.DataBindings.Clear();
				comboBoxQuality.DataBindings.Clear();
				comboBoxType.DataBindings.Clear();
				comboBoxFaction.DataBindings.Clear();
				comboBoxSocket1.DataBindings.Clear();
				comboBoxSocket2.DataBindings.Clear();
				comboBoxSocket3.DataBindings.Clear();
				//itemButtonGem1.DataBindings.Clear();
				//itemButtonGem2.DataBindings.Clear();
				//itemButtonGem3.DataBindings.Clear();

				if (selectedItem != null)
				{
                    _loadingItem = true;
                    textBoxName.DataBindings.Add("Text", selectedItem, "Name");
                    textBoxItemLevel.DataBindings.Add("Text", selectedItem, "ItemLevel");
                    textBoxSetName.DataBindings.Add("Text", selectedItem, "SetName");
                    textBoxIcon.DataBindings.Add("Text", selectedItem, "IconPath");
                    textBoxCost.DataBindings.Add("Text", selectedItem, "Cost");
                    numericUpDownId.DataBindings.Add("Value", selectedItem, "Id");
					numericUpDownMin.DataBindings.Add("Value", selectedItem, "MinDamage");
					numericUpDownMax.DataBindings.Add("Value", selectedItem, "MaxDamage");
					numericUpDownSpeed.DataBindings.Add("Value", selectedItem, "Speed");

                    checkBoxUnique.DataBindings.Add("Checked", selectedItem, "Unique");
                    comboBoxQuality.DataBindings.Add("Text", selectedItem, "Quality");
					comboBoxSlot.DataBindings.Add("Text", selectedItem, "SlotString");
					comboBoxType.DataBindings.Add("Text", selectedItem, "TypeString");
					comboBoxFaction.DataBindings.Add("Text", selectedItem, "FactionString");
                    comboBoxDamageType.DataBindings.Add("Text", selectedItem, "DamageType");
                    comboBoxSocket1.DataBindings.Add("Text", selectedItem, "SocketColor1String");
					comboBoxSocket2.DataBindings.Add("Text", selectedItem, "SocketColor2String");
					comboBoxSocket3.DataBindings.Add("Text", selectedItem, "SocketColor3String");
					//itemButtonGem1.DataBindings.Add("SelectedItemId", selectedItem, "Gem1Id");
					//itemButtonGem2.DataBindings.Add("SelectedItemId", selectedItem, "Gem2Id");
					//itemButtonGem3.DataBindings.Add("SelectedItemId", selectedItem, "Gem3Id");

                    textBoxSource.Text = selectedItem.LocationInfo[0].Description;
                    if(selectedItem.LocationInfo[1] != null) textBoxSource.Text += "\r\n" + selectedItem.LocationInfo[1].Description;
                    textBoxNote.DataBindings.Add("Text", selectedItem.LocationInfo[0], "Note");

                    propertyGridStats.SelectedObject = selectedItem.Stats;

                    string requiredClassesString = "";
                    if (selectedItem.RequiredClasses != null) requiredClassesString = selectedItem.RequiredClasses;
                    string[] requiredClasses = requiredClassesString.Split('|');
                    for (int i = 0; i < checkedListBoxRequiredClasses.Items.Count; i++)
                    {
                        checkedListBoxRequiredClasses.SetItemChecked(i, Array.IndexOf<string>(requiredClasses, (string)checkedListBoxRequiredClasses.Items[i]) >= 0);
                    }

                    var socketBonuses = selectedItem.SocketBonus.Values(x=> x > 0).GetEnumerator();
                    if (!socketBonuses.MoveNext())
                    {
                        comboBoxBonus1.SelectedIndex = 0;
                    }
                    else
                    {
                        // this doesn't always trigger a index changed event, so can't rely on this to set the binding for numeric up down
                        comboBoxBonus1.SelectedIndex = comboBoxBonus1.Items.IndexOf(Extensions.DisplayName(socketBonuses.Current.Key).Trim());
                        
                        numericUpDownBonus1.Value = (decimal)socketBonuses.Current.Value;

                        // do we really need to write value here?
                        for (int i = 0; i < numericUpDownBonus1.DataBindings.Count; i++)
                        {
                            numericUpDownBonus1.DataBindings[i].WriteValue();
                        }

                        // adding this because otherwise there were problems with data binding the up down when starting from item context menu
                        numericUpDownBonus1.DataBindings.Clear();
                        numericUpDownBonus1.DataBindings.Add("Value", selectedItem.SocketBonus, socketBonuses.Current.Key.Name);

/*
 // if there is ever more than one socket
                        if (!socketBonuses.MoveNext())
                        {
                            comboBoxBonus2.SelectedIndex = 0;
                        }
                        else
                        {

                            comboBoxBonus2.SelectedIndex = comboBoxBonus1.Items.IndexOf(Extensions.SpaceCamel(socketBonuses.Current.Key.Name));
                            numericUpDownBonus2.Value = (decimal)socketBonuses.Current.Value;
                        }
*/
                    }
                    UpdateSpecialEffects();

                    _loadingItem = false;
				}
			}
		}

        //void Item_IdsChanged(object sender, EventArgs e)
        //{
        //    Item item = (sender as Item);
        //    foreach (CharacterSlot slot in _equippedSlots)
        //        _character[slot] = item;
        //}

		//private CharacterSlot[] _equippedSlots;

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
            }
        }

        public FormItemEditor(Character character)
		{
			InitializeComponent();
			_character = character;
			//listViewItems.SmallImageList = ItemIcons.SmallIcons;
            LoadItems();



            comboBoxBonus1.Tag = numericUpDownBonus1;
            comboBoxBonus1.Items.Add("None");
            //comboBoxBonus1.Items.AddRange(Stats.StatNames);

            foreach(String s in Stats.StatNames){
                comboBoxBonus1.Items.Add(s.Trim());
            }


 /*
           comboBoxBonus2.Tag = numericUpDownBonus2;
            comboBoxBonus2.Items.Add("None");
            comboBoxBonus2.Items.AddRange(Stats.StatNames);
*/

			ItemCache.Instance.ItemsChanged += new EventHandler(ItemCache_ItemsChanged);
			this.FormClosing += new FormClosingEventHandler(FormItemEditor_FormClosing);
		}

		public FormItemEditor(Character character, Item initialSelection)
		{
			InitializeComponent();
			_character = character;
			//listViewItems.SmallImageList = ItemIcons.SmallIcons;
			_loadingItem = true;
			textBoxFilter.Text = initialSelection.Name;
			_loadingItem = false;
			LoadItems();

			comboBoxBonus1.Tag = numericUpDownBonus1;
			comboBoxBonus1.Items.Add("None");
			//comboBoxBonus1.Items.AddRange(Stats.StatNames);

            foreach (String s in Stats.StatNames) {
                comboBoxBonus1.Items.Add(s.Trim());
            }


			/*
					  comboBoxBonus2.Tag = numericUpDownBonus2;
					   comboBoxBonus2.Items.Add("None");
					   comboBoxBonus2.Items.AddRange(Stats.StatNames);
		   */

            ListViewItem lvi = SelectItem(initialSelection, true);

			ItemCache.Instance.ItemsChanged += new EventHandler(ItemCache_ItemsChanged);
			this.FormClosing += new FormClosingEventHandler(FormItemEditor_FormClosing);
		}

		void FormItemEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.textBoxName.Focus(); //Force data changes to be applied through databinding...
			this.textBoxIcon.Focus(); //databinding doesn't seem to set the new value until focus has left the control
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
		}

		private bool _changingItemCache = false;
		private void ItemCache_ItemsChanged(object sender, EventArgs e)
		{
			if (!_changingItemCache && SelectedItem != null)
			{
				Item selectedItem = SelectedItem.Tag as Item;
				//LoadItems();
				SelectItem(selectedItem, true);
			}
		}

		void comboBoxSocket_TextChanged(object sender, EventArgs e)
		{
			//itemButtonGem1.CharacterSlot = comboBoxSocket1.Text == ItemSlot.Meta.ToString() ? CharacterSlot.Metas : CharacterSlot.Gems;
			//itemButtonGem2.CharacterSlot = comboBoxSocket2.Text == ItemSlot.Meta.ToString() ? CharacterSlot.Metas : CharacterSlot.Gems;
			//itemButtonGem3.CharacterSlot = comboBoxSocket3.Text == ItemSlot.Meta.ToString() ? CharacterSlot.Metas : CharacterSlot.Gems;
		}

		private void LoadItems() { LoadItems(ItemCache.AllItems); }
		private void LoadItems(Item[] items)
		{
			listViewItems.Items.Clear();
			List<ListViewItem> itemsToAdd = new List<ListViewItem>();
			foreach (Item item in items)
			{
				if (string.IsNullOrEmpty(textBoxFilter.Text) || item.Name.ToLower().Contains(textBoxFilter.Text.ToLower()))
				{
					string slot = item.Slot.ToString();
					if (item.Slot == ItemSlot.Red || item.Slot == ItemSlot.Orange || item.Slot == ItemSlot.Yellow
						 || item.Slot == ItemSlot.Green || item.Slot == ItemSlot.Blue || item.Slot == ItemSlot.Purple
						 || item.Slot == ItemSlot.Prismatic || item.Slot == ItemSlot.Meta) slot = "Gems";
					ListViewItem lvi = new ListViewItem(item.Name, listViewItems.Groups["listViewGroup" + slot]);
					lvi.Tag = item;
					//lvi.ImageKey = EnsureIconPath(item.IconPath);
					itemsToAdd.Add(lvi);
				}
			}
			listViewItems.Items.AddRange(itemsToAdd.ToArray());
            listViewItems.Sort();
		}

		private string EnsureIconPath(string iconPath)
		{
			if (!ItemIcons.SmallIcons.Images.ContainsKey(iconPath))
			{
				try
				{
					/*imageListItems.Images.Add(iconPath, */ItemIcons.GetItemIcon(iconPath, true)/*)*/;
				}
				catch { }
			}
			return iconPath;
		}

		private void listViewItems_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewItems.SelectedIndices.Count > 0)
			{
				SelectedItem = listViewItems.SelectedItems[0];
                SelectedItem.EnsureVisible();
			}
		}

		private void FormItemEditor_Load(object sender, EventArgs e)
		{
            listViewItems.Sort();
            //force a paint, without this, the sorting doesn't render correctly 
            //until the user causes a paint event over the moved items (scrolling off the screen or clicking on the affected ones.
            //have to do the DoEvents to force it to process too otherwise the EnsureVisible won't work correctly in the activation event 
            //(I couldn't get it to won't work here at all)
            listViewItems.Invalidate(true);
            Application.DoEvents();
            if (listViewItems.Items.Count > 0 && listViewItems.SelectedIndices.Count == 0)
            {
                ListViewItem item = FindFirstItem();
                if (item != null)
                {
                    item.Selected = true;
                }
                else
                {
                    listViewItems.Items[0].Selected = true;
                }
            }
		}

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			FormEnterId form = new FormEnterId();
			if (form.ShowDialog(this) == DialogResult.OK)
			{
				_changingItemCache = true;
				try
				{
					WebRequestWrapper.ResetFatalErrorIndicator();
                    int itemId = form.Value;
                    if (itemId > 0)
					    AddItemById(form.Value, form.UseArmory, form.UseWowhead);
                    else
                        AddItemByName(form.ItemName, form.UseArmory, form.UseWowhead);
				}
				finally
				{
					_changingItemCache = false;
				}
			}
            form.Dispose();
		}

        private void AddItemByName(string name, bool useArmory, bool useWowhead)
        {
            Item newItem = null;

            // ignore empty strings
            if (name.Length <= 0) return;

            // try the armory (if requested)
            if (useArmory)
            {
                Int32 item_id = Armory.GetItemIdByName(name);
                if (item_id > 0)
                {
                    newItem = Item.LoadFromId(item_id, true, true, false);
                }
            }

            // try wowhead (if requested)
            if ((newItem == null) && useWowhead)
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
            }

            if (newItem == null)
            {
                MessageBox.Show("Unable to load item: " + name + ".", "Item not found.", MessageBoxButtons.OK);
            }
            else
            {
                AddNewItemToListView(newItem);
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
                if ((newItem == null) && useWowhead)
                {
                    newItem = Wowhead.GetItem(id.ToString(), true);
                    if (newItem != null) ItemCache.AddItem(newItem, true);
                }
                if ((newItem == null) && useWowhead)
                {
                    newItem = Wowhead.GetItem("ptr", id.ToString(), true);
                    if (newItem != null) ItemCache.AddItem(newItem, true);
                }

                if (newItem == null)
                {
					if (MessageBox.Show("Unable to load item " + id.ToString() + ". Would you like to create the item blank and type in the values yourself?", "Item not found. Create Blank?", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						newItem = new Item("New Item", ItemQuality.Epic, ItemType.None, id, "temp", ItemSlot.Head, string.Empty, false, new Stats(), new Stats(), ItemSlot.None, ItemSlot.None, ItemSlot.None, 0, 0, ItemDamageType.Physical, 0f, string.Empty);
						ItemCache.AddItem(newItem);
					}
                }
                {
                    AddNewItemToListView(newItem);
                }
            }
		}

        private void AddNewItemToListView(Item newItem)
        {
            // show the item we just added (if we added one)
            if (newItem != null)
            {
                ListViewItem newLvi = new ListViewItem(newItem.Name, 0, listViewItems.Groups["listViewGroup" + newItem.Slot.ToString()]);
                newLvi.Tag = newItem;
                //newLvi.ImageKey = EnsureIconPath(newItem.IconPath);
                string slot = newItem.Slot.ToString();
                if (newItem.Slot == ItemSlot.Red || newItem.Slot == ItemSlot.Orange || newItem.Slot == ItemSlot.Yellow
                     || newItem.Slot == ItemSlot.Green || newItem.Slot == ItemSlot.Blue || newItem.Slot == ItemSlot.Purple
                     || newItem.Slot == ItemSlot.Prismatic || newItem.Slot == ItemSlot.Meta) slot = "Gems";
                newLvi.Group = listViewItems.Groups["listViewGroup" + slot];

                listViewItems.Items.Add(newLvi);
                listViewItems.Sort();
                listViewItems.SelectedIndices.Clear();
                newLvi.Selected = true;
                newLvi.EnsureVisible();

            }
        }

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			
            if (listViewItems.SelectedItems.Count > 0)
            {
                Item item = listViewItems.SelectedItems[0].Tag as Item;
                if (MessageBox.Show("Are you sure you want to delete " + item.Name + " from your item cache?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _changingItemCache = true;
                    ItemCache.DeleteItem(item);
                    _changingItemCache = false;
                    listViewItems.Items.Remove(listViewItems.SelectedItems[0]); 
                    if (listViewItems.Items.Count > 0)
                    {
                        listViewItems.SelectedIndices.Add(0);
                    }
                }
            }
			
		}

		private void buttonFillSockets_Click(object sender, EventArgs e)
		{
			/*FormFillSockets form = new FormFillSockets();
			if (form.ShowDialog(this) == DialogResult.OK)
			{
				foreach (Item item in ItemCache.AllItems)
				{
					if (item.SocketColor1 != ItemSlot.None && (!form.FillEmptySockets || item.Gem1Id == 0))
					{
						switch (item.SocketColor1)
						{
							case ItemSlot.Red:
								item.Gem1 = form.GemRed;
								break;

							case ItemSlot.Blue:
								item.Gem1 = form.GemBlue;
								break;

							case ItemSlot.Yellow:
								item.Gem1 = form.GemYellow;
								break;

							case ItemSlot.Meta:
								item.Gem1 = form.GemMeta;
								break;
						}
					}
					if (item.Sockets.Color2 != ItemSlot.None && (!form.FillEmptySockets || item.Gem2Id == 0))
					{
						switch (item.Sockets.Color2)
						{
							case ItemSlot.Red:
								item.Gem2 = form.GemRed;
								break;

							case ItemSlot.Blue:
								item.Gem2 = form.GemBlue;
								break;

							case ItemSlot.Yellow:
								item.Gem2 = form.GemYellow;
								break;

							case ItemSlot.Meta:
								item.Gem2 = form.GemMeta;
								break;
						}
					}
					if (item.Sockets.Color3 != ItemSlot.None && (!form.FillEmptySockets || item.Gem3Id == 0))
					{
						switch (item.Sockets.Color3)
						{
							case ItemSlot.Red:
								item.Gem3 = form.GemRed;
								break;

							case ItemSlot.Blue:
								item.Gem3 = form.GemBlue;
								break;

							case ItemSlot.Yellow:
								item.Gem3 = form.GemYellow;
								break;

							case ItemSlot.Meta:
								item.Gem3 = form.GemMeta;
								break;
						}
					}
				}
			}
            form.Dispose();*/
		}

		private void buttonDuplicate_Click(object sender, EventArgs e)
		{
			/*Item item = listViewItems.SelectedItems[0].Tag as Item;
			Item copy = new Item(item.Name, item.Quality, item.Type, item.Id, item.IconPath, item.Slot, item.SetName, item.Unique, item.Stats.Clone(),
				item.SocketBonus.Clone(), 0, 0, 0, item.MinDamage, item.MaxDamage, item.DamageType, item.Speed, item.RequiredClasses);
			_changingItemCache = true;
			ItemCache.AddItem(copy, false, true);
			_changingItemCache = false;

			ListViewItem newLvi = new ListViewItem(copy.Name, 0, listViewItems.Groups["listViewGroup" + copy.Slot.ToString()]);
			newLvi.Tag = copy;
			//newLvi.ImageKey = EnsureIconPath(copy.IconPath);
			string slot = copy.Slot.ToString();
			if (copy.Slot == ItemSlot.Red || copy.Slot == ItemSlot.Orange || copy.Slot == ItemSlot.Yellow
				 || copy.Slot == ItemSlot.Green || copy.Slot == ItemSlot.Blue || copy.Slot == ItemSlot.Purple
				 || copy.Slot == ItemSlot.Prismatic || copy.Slot == ItemSlot.Meta) slot = "Gems";
			newLvi.Group = listViewItems.Groups["listViewGroup" + slot];

			listViewItems.Items.Add(newLvi);
			newLvi.Selected = true;
			listViewItems.Sort();
			newLvi.EnsureVisible();*/
		}

        internal ListViewItem SelectItem(Item item, bool force) 
		{
			bool found = false;
            foreach (ListViewItem lvi in listViewItems.Items)
            {
                if (lvi.Tag == item)
                {
                    lvi.Selected = true;
                    lvi.EnsureVisible();
                    found = true;
                    return lvi;
                }
            }
			if (!found && force)
			{
				string filter = textBoxFilter.Text;
				textBoxFilter.Text = "";
                foreach (ListViewItem lvi in listViewItems.Items)
                {
                    if (lvi.Tag == item)
                    {
                        lvi.Selected = true;
                        lvi.EnsureVisible();
                        found = true;
                        return lvi;
                    }
                }
                if (!found)
                {
                    textBoxFilter.Text = filter;
                }
			}
            return null;
		}

        private void comboBoxBonus_SelectedIndexChanged(object sender, EventArgs e)
        {
            Item selectedItem = SelectedItem.Tag as Item;
            UpdateStatDataBindings(sender as ComboBox, selectedItem.SocketBonus, true);
        }

        private void comboBoxStat_SelectedIndexChanged(object sender, EventArgs e)
        {
            Item selectedItem = SelectedItem.Tag as Item;
            UpdateStatDataBindings(sender as ComboBox, selectedItem.Stats, false);
        }

        private void UpdateStatDataBindings(ComboBox combo, Stats boundStats, bool clearExistingValue)
        {
            if (null != combo)
            {
                NumericUpDown ud = (combo.Tag as NumericUpDown);
                if (ud != null)
                {
                    decimal value = ud.Value;
                    if(clearExistingValue)
                    {
                        ud.Value = 0;

                        for (int i = 0; i < ud.DataBindings.Count; i++)
                        {
                            ud.DataBindings[i].WriteValue();
                        }
                        ud.DataBindings.Clear();
                        ud.Enabled = (combo.SelectedIndex > 0);
                    }
                    
                    if (ud.Enabled)
                    {
                        string v = Extensions.UnDisplayName(combo.Items[combo.SelectedIndex].ToString()).Name;
                        ud.DataBindings.Add("Value", boundStats, v);
                       
                        if(clearExistingValue)
                        {
                            ud.Value = value;
                            for (int i = 0; i < ud.DataBindings.Count; i++)
                            {
                                ud.DataBindings[i].WriteValue();
                            }
                        }
                    }
                }
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.textBoxName.Focus(); //Force data changes to be applied through databinding...
            this.textBoxIcon.Focus(); //databinding doesn't seem to set the new value until focus has left the control
            if (SelectedItem != null)
            {
                Item selectedItem = SelectedItem.Tag as Item;
                selectedItem.InvalidateCachedData();
                // sanitize item cache
                if (selectedItem.Id != selectedId)
                {
                    // we changed the id of the item, sanitize the item cache
                    _changingItemCache = true;
                    lock (ItemCache.Items)
                    {
                        ItemCache.Items.Remove(selectedId); // clean the entry at old id
                    }
                    ItemCache.AddItem(selectedItem); // insert it at the new id and clear whatever was at that id before
                    _changingItemCache = false;
                }
                Character.OnCalculationsInvalidated();
            }
        }

        private void FormItemEditor_Activated(object sender, EventArgs e)
        {
            if ( _firstLoad && listViewItems.SelectedIndices.Count > 0)
            {
                //EnsureVisible works correctly in Load, except that the list is then reordered by the sort, and doesn't shift
                //the scroll correctly if redone in Load, it only seemed to work in another event handle.
                listViewItems.Items[listViewItems.SelectedIndices[0]].EnsureVisible();
            }
        }

        // Being lazy with this since its a known case and probably won't change
        private ListViewItem FindFirstItem()
        {
            ListViewItem ret = null;
            if (listViewItems.Groups.Count > 0 && listViewItems.Groups[0].Items.Count > 0)
            {
                ret = listViewItems.Groups[0].Items[0];
                for (int i = 1; i < listViewItems.Groups[0].Items.Count; i++)
                {
                    if (string.Compare(ret.Text, listViewItems.Groups[0].Items[i].Text) > 0)
                    {
                        ret = listViewItems.Groups[0].Items[i];
                    }
                }
            }
            return ret;
        }

		private void buttonDeleteDuplicates_Click(object sender, EventArgs e)
		{
			/*if (listViewItems.SelectedItems.Count > 0)
			{
				Item itemToSave = listViewItems.SelectedItems[0].Tag as Item;
				if (MessageBox.Show("Are you sure you want to delete all instances of " + itemToSave.Name + " except the selected one?", "Confirm Delete Duplicates", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					_changingItemCache = true;
					Cursor = Cursors.WaitCursor;
					List<Item> itemsToDelete = new List<Item>(ItemCache.Instance.FindAllItemsById(itemToSave.Id));
					Item itemUngemmed = ItemCache.FindItemById(itemToSave.Id.ToString() + ".0.0.0", false, false);
					if (itemUngemmed != null) itemsToDelete.Add(itemUngemmed);
					if (itemsToDelete.Contains(itemToSave)) itemsToDelete.Remove(itemToSave);
					foreach (Item itemToDelete in itemsToDelete)
						ItemCache.DeleteItem(itemToDelete);
					foreach (ListViewItem lvi in listViewItems.Items)
						if (itemsToDelete.Contains(lvi.Tag as Item))
							listViewItems.Items.Remove(lvi);
					Cursor = Cursors.Default;
					_changingItemCache = false;
				}
			}*/
        }

        private void checkedListBoxRequiredClasses_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!_loadingItem)
            {
                Item selectedItem = _selectedItem.Tag as Item;
                List<string> requiredClasses = new List<string>();
                foreach (string requiredClass in checkedListBoxRequiredClasses.CheckedItems)
                {
                    requiredClasses.Add(requiredClass);
                }
                if (e.NewValue == CheckState.Checked)
                {
                    requiredClasses.Add((string)checkedListBoxRequiredClasses.Items[e.Index]);
                }
                else
                {
                    requiredClasses.Remove((string)checkedListBoxRequiredClasses.Items[e.Index]);
                }
                if (selectedItem != null)
                {
                    selectedItem.RequiredClasses = string.Join("|", requiredClasses.ToArray());
                }
            }
        }

        private void butAddSpecialEffect_Click(object sender, EventArgs e)
        {
            FormEditSpecialEffect form = new FormEditSpecialEffect();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                Item selectedItem = SelectedItem.Tag as Item;
                selectedItem.Stats.AddSpecialEffect(new SpecialEffect(form.Trigger, form.Stats,
                    form.Duration, form.Cooldown, form.Chance, form.Stacks));
                UpdateSpecialEffects();
            }
            form.Dispose();
        }

        private void butEditSpecialEffect_Click(object sender, EventArgs e)
        {
            SpecialEffect eff = cmbSpecialEffects.SelectedItem as SpecialEffect;
            if (eff != null && typeof(SpecialEffect) == eff.GetType())
            {
                FormEditSpecialEffect form = new FormEditSpecialEffect(eff.Stats, eff.Trigger, eff.Duration, eff.Cooldown, eff.Chance, eff.MaxStack);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    Item selectedItem = SelectedItem.Tag as Item;
                    selectedItem.Stats.RemoveSpecialEffect(eff);
					form.Stats.InvalidateSparseData();
                    selectedItem.Stats.AddSpecialEffect(new SpecialEffect(form.Trigger, form.Stats,
                        form.Duration, form.Cooldown, form.Chance, form.Stacks));
                    UpdateSpecialEffects();
                }
                form.Dispose();
            }
        }

        private void butDeleteSpecialEffect_Click(object sender, EventArgs e)
        {
            SpecialEffect eff = cmbSpecialEffects.SelectedItem as SpecialEffect;
            if (eff != null && typeof(SpecialEffect) == eff.GetType())
            {
                Item selectedItem = SelectedItem.Tag as Item;
                selectedItem.Stats.RemoveSpecialEffect(eff);
                UpdateSpecialEffects();
            }
        }

        private void UpdateSpecialEffects()
        {
            Item selectedItem = SelectedItem.Tag as Item;
            Stats stats = selectedItem.Stats;
            if (stats.ContainsSpecialEffect())
            {
                List<SpecialEffect> dataSource = new List<SpecialEffect>();
                foreach (SpecialEffect effect in stats.SpecialEffects())
                {
                    dataSource.Add(effect);
                }
                cmbSpecialEffects.DataSource = dataSource;
                cmbSpecialEffects.Enabled = true;
                butEditSpecialEffect.Enabled = true;
                butDeleteSpecialEffect.Enabled = true;
            }
            else
            {
                cmbSpecialEffects.DataSource = null;
                cmbSpecialEffects.Enabled = false;
                butEditSpecialEffect.Enabled = false;
                butDeleteSpecialEffect.Enabled = false;
            }
        }

        private void textBoxFilter_KeyDown(object sender, KeyEventArgs e) {

        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            doFilter();
        }

        private void doFilter()
        {
			if (!_loadingItem)
			{
				Item selectedItem = null;
				if (SelectedItem != null) selectedItem = SelectedItem.Tag as Item;
				LoadItems();
				if (selectedItem != null) SelectItem(selectedItem, false);
			}
        }
    }
}
