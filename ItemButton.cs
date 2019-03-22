using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Rawr
{
	public class ItemButton : Button
	{
		public ItemButton()
		{
			this.Size = new System.Drawing.Size(70, 70);
			this.Text = string.Empty;
			this.MouseUp += new MouseEventHandler(ItemButton_MouseClick);
			this.MouseEnter += new EventHandler(ItemButton_MouseEnter);
			this.MouseLeave += new EventHandler(ItemButton_MouseLeave);

			ItemToolTip.Instance.SetToolTip(this, "");

			//ItemCache.Instance.ItemsChanged += new EventHandler(ItemCache_ItemsChanged);
		}

        public IFormItemSelectionProvider FormItemSelection { get; set; }

		private bool _readOnly = false;
		public bool ReadOnly
		{
			get { return _readOnly; }
			set { _readOnly = value; }
		}

        //private bool _itemsChanging = false;
        //void ItemCache_ItemsChanged(object sender, EventArgs e)
        //{
        //    /*if (SelectedItem != null && !_itemsChanging)
        //    {
        //        _itemsChanging = true;
        //        SelectedItem = ItemCache.FindItemById(SelectedItem.GemmedId);
        //        _itemsChanging = false;
        //    }*/
        //}

		void ItemButton_MouseLeave(object sender, EventArgs e)
		{
			ItemToolTip.Instance.Hide(this);
		}

		void ItemButton_MouseEnter(object sender, EventArgs e)
		{
            if (SelectedItemInstance != null)
            {
                int tipX = this.Width;
                if (Parent.PointToScreen(Location).X + tipX + 249 > System.Windows.Forms.Screen.GetWorkingArea(this).Right)
                    tipX = -309;
                ItemToolTip.Instance.Show(Character, SelectedItemInstance, this, new Point(tipX, 0));
            }
			else if (SelectedItem != null)
			{
				int tipX = this.Width;
				if (Parent.PointToScreen(Location).X + tipX + 249 > System.Windows.Forms.Screen.GetWorkingArea(this).Right)
					tipX = -309;
				ItemToolTip.Instance.Show(Character, SelectedItem, this, new Point(tipX, 0));
			}
		}

		void ItemButton_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && !ReadOnly)
			{
				IFormItemSelectionProvider form = (FormItemSelection ?? (this.FindForm() as IFormItemSelectionProvider));
				if (form != null)
					form.FormItemSelection.Show(this, CharacterSlot);
			}
			else if (e.Button == MouseButtons.Right && SelectedItemInstance != null)
			{
                if (Control.ModifierKeys == Keys.Shift)
                    ItemContextualMenu.Instance.EquipCustomisedItem(SelectedItemInstance, CharacterSlot);
                else
                    ItemContextualMenu.Instance.Show(Character, SelectedItemInstance, CharacterSlot, false);
			}
		}

		private CharacterSlot _characterSlot = CharacterSlot.Head;
		public CharacterSlot CharacterSlot
		{
			get { return _characterSlot; }
			set
			{
				_characterSlot = value;
				//Items = Items;
			}
		}

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
				if (_character != null)
				{
					_character.CalculationsInvalidated += new EventHandler(_character_ItemsChanged);
					SelectedItemInstance = _character[CharacterSlot];
				}
			}
		}

		void _character_ItemsChanged(object sender, EventArgs e)
		{
            UpdateDim();
		}
        private void UpdateDim()
        {
            if (Character != null && CharacterSlot == CharacterSlot.OffHand)
            {
                _dimIcon = !Calculations.IncludeOffHandInCalculations(Character);
            }
            else
            {
                _dimIcon = false;
            }
        }

		public int SelectedItemId
		{
			get
			{
				if (SelectedItem == null) return 0;
				else return SelectedItem.Id;
			}
			set
			{
				if (value == 0) SelectedItem = null;
				else SelectedItem = ItemCache.FindItemById(value);
			}
		}

        private bool itemHidden;
        public bool ItemHidden
        {
            get
            {
                return itemHidden;
            }
            set
            {
                itemHidden = value;
                UpdateSelectedItem();
            }
        }

		public void UpdateSelectedItem()
		{
			if (Character != null)
			{
				_selectedItemInstance = Character[CharacterSlot];
                if (_selectedItemInstance == null)
                {
                    _selectedItem = null;
                }
                else
                {
                    _selectedItem = _selectedItemInstance.Item;
                }
			}
			this.Text = string.Empty;
            UpdateDim();
            if (ItemHidden)
            {
                this.ItemIcon = null;
            }
            else
            {
                this.ItemIcon = _selectedItem != null ? ItemIcons.GetItemIcon(_selectedItem, this.Width < 64) : null;
            }
		}

		public event EventHandler SelectedItemChanged;
		private Item _selectedItem;
		public Item SelectedItem
		{
			get
			{
				return _selectedItem;
			}
			set
			{
                if (Character == null)
                {
                    _selectedItemInstance = null;
                    _selectedItem = value;
                    UpdateSelectedItem();
                }
				//foreach (ToolStripMenuItem menuItem in menu.Items)
				//	menuItem.Checked = (menuItem.Tag == _selectedItem);
				if (SelectedItemChanged != null) SelectedItemChanged(this, EventArgs.Empty);
			}
		}

        private ItemInstance _selectedItemInstance;
        public ItemInstance SelectedItemInstance
        {
            get
            {
                return _selectedItemInstance;
            }
            set
            {
                if (Character != null && !Character.IsLoading)
                    Character[CharacterSlot] = value;
                else
                {
                    _selectedItemInstance = value;
                    if (_selectedItemInstance != null)
                    {
                        _selectedItem = _selectedItemInstance.Item;
                    }
                    else
                    {
                        _selectedItem = null;
                    }
                }
                UpdateSelectedItem();
            }
        }

		private bool _dimIcon = false;
		private Image _itemIcon = null;
		public Image ItemIcon
		{
			get { return _itemIcon; }
			set
			{
				_itemIcon = value;
				if (_dimIcon && value != null)
					this.Image = GetDimIcon(value);
				else
					this.Image = value;
			}
		}

		private Image GetDimIcon(Image icon)
		{
			try
			{
                Bitmap original = new Bitmap(icon);
                Bitmap bmp = new Bitmap(original.Width, original.Height);
				Graphics g = Graphics.FromImage(bmp);
				System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();
				System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix();
				cm.Matrix33 = 0.5f;
				ia.SetColorMatrix(cm);
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0,
                                original.Width, original.Height, GraphicsUnit.Pixel, ia);
				g.Dispose();
				ia.Dispose();
                original.Dispose();
				return bmp;
			}
			catch
			{
				return null;
			}
		}

		//public Item[] _sortedItems = new Item[0];
		//public Item[] _items = new Item[0];
		//public Item[] Items
		//{
		//    get
		//    {
		//        return _items;
		//    }
		//    set
		//    {
		//        _items = value;
		//        if (_items == null) _items = new Item[0];
				
		//        //_items = new List<Item>(sortedItems.Values).ToArray();
		//        SortedList<string, Item> sortedItems = new SortedList<string, Item>();
		//        foreach (Item item in _items)
		//            if (item.FitsInSlot(CharacterSlot))
		//                sortedItems.Add(item.Name + item.GemmedId + item.GetHashCode().ToString(), item);
		//        _sortedItems = new List<Item>(sortedItems.Values).ToArray();

		//        //menu.Items.Clear();
		//        //ToolStripMenuItem menuItem = new ToolStripMenuItem("None");
		//        //menuItem.Height = 36;
		//        //menuItem.Click += new EventHandler(menuItem_Click);
		//        //menuItem.Checked = (SelectedItem == null);
		//        //menu.Items.Add(menuItem);
		//        //foreach (Item item in sortedItems.Values)
		//        //{
		//        //    //menu.Items.Add(item.ToString(), ItemIcons.GetItemIcon(item, true));
		//        //    menuItem = new ToolStripMenuItem(item.ToString(), ItemIcons.GetItemIcon(item, true));
		//        //    menuItem.Height = 36;
		//        //    menuItem.ImageScaling = ToolStripItemImageScaling.None;
		//        //    menuItem.Click += new EventHandler(menuItem_Click);
		//        //    menuItem.Tag = item;
		//        //    menuItem.Checked = (SelectedItem == item);
		//        //    menu.Items.Add(menuItem);
		//        //}
		//        SelectedItem = _selectedItem;
		//    }
		//}

		//void menuItem_Click(object sender, EventArgs e)
		//{
		//    SelectedItem = (sender as ToolStripMenuItem).Tag as Item;
		//}
	}


	//public class GemButton : Button
	//{
	//    ToolTip tip = new ToolTip();
	//    ContextMenuStrip menu = new ContextMenuStrip();
	//    public GemButton()
	//    {
	//        this.Size = new System.Drawing.Size(70, 70);
	//        this.Text = string.Empty;
	//        this.Click += new EventHandler(GemButton_Click);
	//        tip.AutomaticDelay = 0;
	//        tip.InitialDelay = 0;
	//        tip.ReshowDelay = 0;
	//        tip.AutoPopDelay = 0;
	//        tip.UseAnimation = false;
	//        tip.UseFading = false;
	//    }

	//    void GemButton_Click(object sender, EventArgs e)
	//    {
	//        menu.Show(this, this.Width, 0);
	//    }

	//    private Gem _selectedGem;
	//    public Gem SelectedGem
	//    {
	//        get
	//        {
	//            return _selectedGem;
	//        }
	//        set
	//        {
	//            _selectedGem = value;
	//            this.Text = string.Empty;
	//            if (_selectedGem != null)
	//            {
	//                tip.SetToolTip(this, _selectedGem.ToString());
	//                this.Image = ItemIcons.GetItemIcon(_selectedGem);
	//                foreach (ToolStripMenuItem menuItem in menu.Items)
	//                    menuItem.Checked = (menuItem.Tag == _selectedGem);
	//            }
	//        }
	//    }

	//    public Gem[] _gems = new Gem[0];
	//    public Gem[] Gems
	//    {
	//        get
	//        {
	//            return _gems;
	//        }
	//        set
	//        {
	//            _gems = value;
	//            if (_gems == null) _gems = new Gem[0];

	//            menu.Items.Clear();
	//            foreach (Gem gem in _gems)
	//            {
	//                menu.Items.Add(gem.ToString(), ItemIcons.GetItemIcon(gem, true));
	//                ToolStripMenuItem menuItem = menu.Items[menu.Items.Count - 1] as ToolStripMenuItem;
	//                menuItem.Height = 36;
	//                menuItem.ImageScaling = ToolStripItemImageScaling.None;
	//                menuItem.Click += new EventHandler(menuItem_Click);
	//                menuItem.Tag = gem;
	//                menuItem.Checked = (gem == SelectedGem);
	//            }
	//        }
	//    }

	//    void menuItem_Click(object sender, EventArgs e)
	//    {
	//        SelectedGem = (sender as ToolStripMenuItem).Tag as Gem;
	//    }
	//}
}
