using System;
using System.Windows.Forms;
using System.Drawing;

namespace Rawr
{
    public partial class FormItemInstance : Form, IFormItemSelectionProvider
	{
        private FormItemSelection _formItemSelection;
        public FormItemSelection FormItemSelection
        {
            get
            {
                if (_formItemSelection == null || _formItemSelection.IsDisposed)
                {
                    _formItemSelection = new FormItemSelection();
                    _formItemSelection.Character = FormMain.Instance.FormItemSelection.Character;
                    _formItemSelection.CurrentCalculations = FormMain.Instance.FormItemSelection.CurrentCalculations;
                }
                return _formItemSelection;
            }
        }

        public FormItemInstance()
        {
            InitializeComponent();
        }

        public CharacterSlot CharacterSlot
        {
            get
            {
                return itemButtonWithEnchant.CharacterSlot;
            }
            set
            {
                itemButtonWithEnchant.CharacterSlot = value;
            }
        }

        public ItemInstance ItemInstance
        {
            get
            {
                return itemButtonWithEnchant.SelectedItem;
            }
            set
            {
                if (value != null)
                {
                    itemButtonWithEnchant.SelectedItem = value;
                    gem1Button.SelectedItem = value.Gem1;
                    gem2Button.SelectedItem = value.Gem2;
                    gem3Button.SelectedItem = value.Gem3;
                }
                else
                {
                    itemButtonWithEnchant.SelectedItem = null;
                    gem1Button.SelectedItem = null;
                    gem2Button.SelectedItem = null;
                    gem3Button.SelectedItem = null;
                }
                UpdateSockets();
            }
        }

        private void gem1Button_Leave(object sender, EventArgs e)
        {
            if (ItemInstance != null)
            {
                ItemInstance copy = ItemInstance.Clone();
                copy.Gem1 = gem1Button.SelectedItem;
                itemButtonWithEnchant.SelectedItem = copy;
            }
        }

        private void gem2Button_Leave(object sender, EventArgs e)
        {
            if (ItemInstance != null)
            {
                ItemInstance copy = ItemInstance.Clone();
                copy.Gem2 = gem2Button.SelectedItem;
                itemButtonWithEnchant.SelectedItem = copy;
            }
        }

        private void gem3Button_Leave(object sender, EventArgs e)
        {
            if (ItemInstance != null)
            {
                ItemInstance copy = ItemInstance.Clone();
                copy.Gem3 = gem3Button.SelectedItem;
                itemButtonWithEnchant.SelectedItem = copy;
            }
        }

        private void itemButtonWithEnchant_Leave(object sender, EventArgs e)
        {
            UpdateSockets();
        }

        private void UpdateSockets()
        {
            if (ItemInstance != null && ItemInstance.Item != null)
            {
                Item item = ItemInstance.Item;
                bool blacksmithingSocket = true;
                Character character = FormItemSelection.Character;
                if ((itemButtonWithEnchant.CharacterSlot == CharacterSlot.Waist && character.WaistBlacksmithingSocketEnabled)
                    || (itemButtonWithEnchant.CharacterSlot == CharacterSlot.Waist && character.WaistBlacksmithingSocketEnabled)
                    || (itemButtonWithEnchant.CharacterSlot == CharacterSlot.Waist && character.WaistBlacksmithingSocketEnabled))
                {
                    blacksmithingSocket = true;
                }
                ItemSlot slot = item.SocketColor1;
                if (slot == ItemSlot.None && blacksmithingSocket)
                {
                    slot = ItemSlot.Prismatic;
                    blacksmithingSocket = false;
                }
                SetSocketColor(gem1Button, slot);
                slot = item.SocketColor2;
                if (slot == ItemSlot.None && blacksmithingSocket)
                {
                    slot = ItemSlot.Prismatic;
                    blacksmithingSocket = false;
                }
                SetSocketColor(gem2Button, slot);
                slot = item.SocketColor3;
                if (slot == ItemSlot.None && blacksmithingSocket)
                {
                    slot = ItemSlot.Prismatic;
                    blacksmithingSocket = false;
                }
                SetSocketColor(gem3Button, slot);
            }
            else
            {
                SetSocketColor(gem1Button, ItemSlot.None);
                SetSocketColor(gem2Button, ItemSlot.None);
                SetSocketColor(gem3Button, ItemSlot.None);
            }
        }

        private void SetSocketColor(ItemButton button, ItemSlot slot)
        {
            switch (slot)
            {
                case ItemSlot.Blue:
                    button.BackColor = Color.FromKnownColor(KnownColor.Blue);
                    button.CharacterSlot = CharacterSlot.Gems;
                    break;
                case ItemSlot.Red:
                    button.BackColor = Color.FromKnownColor(KnownColor.Red);
                    button.CharacterSlot = CharacterSlot.Gems;
                    break;
                case ItemSlot.Yellow:
                    button.BackColor = Color.FromKnownColor(KnownColor.Yellow);
                    button.CharacterSlot = CharacterSlot.Gems;
                    break;
                case ItemSlot.Prismatic:
                    button.BackColor = Color.FromKnownColor(KnownColor.Silver);
                    button.CharacterSlot = CharacterSlot.Gems;
                    break;
                case ItemSlot.Meta:
                    button.BackColor = Color.FromKnownColor(KnownColor.Gray);
                    button.CharacterSlot = CharacterSlot.Metas;
                    break;
                default:
                    button.BackColor = SystemColors.Control;
                    button.CharacterSlot = CharacterSlot.Gems;
                    break;
            }
        }

        private void gem1Button_Click(object sender, EventArgs e)
        {
            ItemInstance item = ItemInstance.Clone();
            item.Gem1 = null;
            SetFormItemSelection(item);
        }

        private void gem2Button_Click(object sender, EventArgs e)
        {
            ItemInstance item = ItemInstance.Clone();
            item.Gem2 = null;
            SetFormItemSelection(item);
        }

        private void gem3Button_Click(object sender, EventArgs e)
        {
            ItemInstance item = ItemInstance.Clone();
            item.Gem3 = null;
            SetFormItemSelection(item);
        }

        private void SetFormItemSelection(ItemInstance item)
        {
            if (FormMain.Instance.FormItemSelection.Character != null)
            {
                Character emptyGemCharacter = FormMain.Instance.FormItemSelection.Character.Clone();
                emptyGemCharacter[CharacterSlot] = item;
                _formItemSelection.Character = emptyGemCharacter;
                _formItemSelection.CurrentCalculations = Calculations.GetCharacterCalculations(emptyGemCharacter);
            }
        }
    }
}