using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Rawr
{
	public partial class ItemButtonWithEnchant : UserControl
	{
		public ItemButtonWithEnchant()
		{
			InitializeComponent();
			buttonEnchant.MouseEnter += new EventHandler(buttonEnchant_MouseEnter);
			buttonEnchant.MouseLeave += new EventHandler(buttonEnchant_MouseLeave);

			ItemToolTip.Instance.SetToolTip(this, "");
		}

        private IFormItemSelectionProvider _formItemSelection;
        public IFormItemSelectionProvider FormItemSelection
        {
            get { return _formItemSelection; }
            set
            {
                _formItemSelection = value;
                itemButtonItem.FormItemSelection = value;
            }
        }

		public override string Text { get { return itemButtonItem.Text; } set { itemButtonItem.Text = value; } }
		public CharacterSlot CharacterSlot { get { return itemButtonItem.CharacterSlot; } set { itemButtonItem.CharacterSlot = value; } }
        public ItemInstance SelectedItem { get { return itemButtonItem.SelectedItemInstance; } set { itemButtonItem.SelectedItemInstance = value; UpdateSelectedItem(); } }
		public int SelectedItemId { get { return itemButtonItem.SelectedItemId; } set { itemButtonItem.SelectedItemId = value; } }
		public Image ItemIcon { get { return itemButtonItem.ItemIcon; } set { itemButtonItem.ItemIcon = value; } }
		public bool UseVisualStyleBackColor { get { return itemButtonItem.UseVisualStyleBackColor; } set { itemButtonItem.UseVisualStyleBackColor = value; } }
		
		private Character _character;
		public Character Character
		{
			get { return _character; }
			set
			{
				if (_character != null)
				{
					_character.CalculationsInvalidated -= new EventHandler(_character_CalculationsInvalidated);
				}
				_character = itemButtonItem.Character = value;
				if (_character != null)
				{
					_character.CalculationsInvalidated += new EventHandler(_character_CalculationsInvalidated);
					//SelectedEnchant = _character.GetEnchantBySlot(CharacterSlot);
				}
			}
		}

		void _character_CalculationsInvalidated(object sender, EventArgs e)
		{
			UpdateSelectedItem();
		}
		public int SelectedEnchantId
		{
			get
			{
				if (SelectedItem == null || SelectedItem.Enchant == null) return 0;
				else return SelectedItem.Enchant.Id;
			}
		}

        public Enchant SelectedEnchant
		{
			get
			{
                if (SelectedItem == null) return null;
                else return SelectedItem.Enchant;
            }
		}

        public bool ItemHidden
        {
            get
            {
                return itemButtonItem.ItemHidden;
            }
            set
            {
                itemButtonItem.ItemHidden = value;
                UpdateSelectedItem();
            }
        }

		public void UpdateSelectedItem()
		{
            if (ItemHidden)
            {
                buttonEnchant.Text = "";
            }
            else
            {
                if (Width < 50 || Height < 60)
                {
                    if (SelectedEnchant == null) buttonEnchant.Text = "";
                    else buttonEnchant.Text = SelectedEnchant.ReallyShortName;
                }
                else if (SelectedEnchant == null) buttonEnchant.Text = "";
                else buttonEnchant.Text = SelectedEnchant.ShortName;
            }
            itemButtonItem.UpdateSelectedItem();
		}

		private void buttonEnchant_Click(object sender, EventArgs e)
		{
			IFormItemSelectionProvider form = (FormItemSelection ?? (this.FindForm() as IFormItemSelectionProvider));
			if (form != null)
				form.FormItemSelection.Show(this, CharacterSlot);
		}

		void buttonEnchant_MouseLeave(object sender, EventArgs e)
		{
			ItemToolTip.Instance.Hide(this);
		}

		void buttonEnchant_MouseEnter(object sender, EventArgs e)
		{
            if (SelectedEnchant != null)
			{
				int tipX = this.Width;
				Item itemEnchant = new Item(SelectedEnchant.Name, ItemQuality.Temp, ItemType.None,
					-1 * (SelectedEnchant.Id + (10000 * (int)SelectedEnchant.Slot)), null, ItemSlot.None, null, false, SelectedEnchant.Stats, null, ItemSlot.None, ItemSlot.None, ItemSlot.None, 0, 0,
					ItemDamageType.Physical, 0, null);
				
				if (Parent.PointToScreen(Location).X + tipX + 249 > System.Windows.Forms.Screen.GetWorkingArea(this).Right)
					tipX = -249;
				ItemToolTip.Instance.Show(Character, itemEnchant, this, new Point(tipX, itemButtonItem.Height));
			}
		}
	}
}
