using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rawr.UI
{
    public partial class CustomItemInstance : ChildWindow
    {

        private Character character;
        public Character Character
        {
            get { return character; }
            set { character = value; }
        }

        private ItemInstance customInstance;
        public ItemInstance CustomInstance
        {
            get { return customInstance; }
            set {
                string pt = "\r\n- start";
                try {
                    customInstance = value;
                    pt += "\r\n- if";
                    /*itemButtonWithEnchant.ComparisonItemList.IsPopulated = false;
                    itemButtonWithEnchant.ComparisonEnchantList.IsPopulated = false;
                    gem1Button.ComparisonItemList.IsPopulated = false;
                    gem2Button.ComparisonItemList.IsPopulated = false;
                    gem3Button.ComparisonItemList.IsPopulated = false;
                    itemButtonWithEnchant.ComparisonItemList.IsShown = true;
                    itemButtonWithEnchant.ComparisonEnchantList.IsShown = true;
                    gem1Button.ComparisonItemList.IsShown = true;
                    gem2Button.ComparisonItemList.IsShown = true;
                    gem3Button.ComparisonItemList.IsShown = true;*/
                    if (customInstance != null)
                    {
                        pt += "\r\n- customInstance != null";
                        itemButtonWithEnchant.ComparisonItemList.SelectedItem = value.Item;
                        pt += "\r\n- gembuttons1";
                        gem1Button.ComparisonItemList.SelectedItem = value.Gem1;
                        pt += "\r\n- gembuttons2";
                        gem2Button.ComparisonItemList.SelectedItem = value.Gem2;
                        pt += "\r\n- gembuttons3";
                        gem3Button.ComparisonItemList.SelectedItem = value.Gem3;
                    } else {
                        pt += "\r\n- customInstance == null";
                        itemButtonWithEnchant.ComparisonItemList.SelectedItem = null;
                        pt += "\r\n- gembuttons1";
                        gem1Button.ComparisonItemList.SelectedItem = null;
                        pt += "\r\n- gembuttons2";
                        gem2Button.ComparisonItemList.SelectedItem = null;
                        pt += "\r\n- gembuttons3";
                        gem3Button.ComparisonItemList.SelectedItem = null;
                    }
                    pt += "\r\n- UpdateSockets()";
                    UpdateSockets();
                    pt += "\r\n- end";
                    /*new ErrorWindow() {
                        Message = "Set Custom Instance worked:"
                                  + "\r\nItem: " + (itemButtonWithEnchant.ComparisonItemList.SelectedItem != null ? itemButtonWithEnchant.ComparisonItemList.SelectedItem.Id.ToString() : "null")
                                  + "\r\nGem 1: " + (gem1Button.ComparisonItemList.SelectedItem != null ? gem1Button.ComparisonItemList.SelectedItem.Id.ToString() : "null")
                                  + "\r\nGem 2: " + (gem2Button.ComparisonItemList.SelectedItem != null ? gem2Button.ComparisonItemList.SelectedItem.Id.ToString() : "null")
                                  + "\r\nGem 3: " + (gem3Button.ComparisonItemList.SelectedItem != null ? gem3Button.ComparisonItemList.SelectedItem.Id.ToString() : "null")
                    }.Show();*/
                } catch (Exception ex) {
                    new ErrorWindow() {
                        Message = "Trying to set Custom Item Instance"
                                  + "\r\n\r\nMessage:" + ex.Message
                                  + "\r\nInner Exception:" + ex.InnerException
                                  + "\r\n\r\nStackTrace:" + ex.StackTrace
                                  + "\r\n\r\nPoint:" + pt
                    }.Show();
                } finally {
                }
            }
        }

        public CustomItemInstance(Character character, ItemInstance baseInstance)
        {
            InitializeComponent();
            Character = character;
            CustomInstance = baseInstance.Clone();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    
        // ================================================================================================
        private ItemListControl _formItemSelection;
        public ItemListControl FormItemSelection
        {
            get
            {
                if (_formItemSelection == null)
                    _formItemSelection = new ItemListControl();
                return _formItemSelection;
            }
        }

        public CharacterSlot CharacterSlot
        {
            get { return itemButtonWithEnchant.Slot; }
            set { itemButtonWithEnchant.Slot = value; }
        }

        private void gem1Button_Leave(object sender, EventArgs e)
        {
            if (CustomInstance != null)
            {
                ItemInstance copy = CustomInstance.Clone();
                copy.Gem1 = gem1Button.ComparisonItemList.SelectedItem;
                itemButtonWithEnchant.Item = copy;
            }
        }

        private void gem2Button_Leave(object sender, EventArgs e)
        {
            if (CustomInstance != null)
            {
                ItemInstance copy = CustomInstance.Clone();
                copy.Gem2 = gem2Button.ComparisonItemList.SelectedItem;
                itemButtonWithEnchant.Item = copy;
            }
        }

        private void gem3Button_Leave(object sender, EventArgs e)
        {
            if (CustomInstance != null)
            {
                ItemInstance copy = CustomInstance.Clone();
                copy.Gem3 = gem3Button.ComparisonItemList.SelectedItem;
                itemButtonWithEnchant.Item = copy;
            }
        }

        private void itemButtonWithEnchant_Leave(object sender, EventArgs e)
        {
            UpdateSockets();
        }

        private void UpdateSockets()
        {
            if (CustomInstance != null && CustomInstance.Item != null)
            {
                Item item = CustomInstance.Item;
                bool blacksmithingSocket = true;
                Character character = FormItemSelection.Character;
                if ((itemButtonWithEnchant.Slot == CharacterSlot.Waist && character.WaistBlacksmithingSocketEnabled)
                    || (itemButtonWithEnchant.Slot == CharacterSlot.Waist && character.WaistBlacksmithingSocketEnabled)
                    || (itemButtonWithEnchant.Slot == CharacterSlot.Waist && character.WaistBlacksmithingSocketEnabled))
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
            /*itemButtonWithEnchant.ComparisonItemList.IsPopulated = false;
            itemButtonWithEnchant.ComparisonEnchantList.IsPopulated = false;
            gem1Button.ComparisonItemList.IsPopulated = false;
            gem2Button.ComparisonItemList.IsPopulated = false;
            gem3Button.ComparisonItemList.IsPopulated = false;
            itemButtonWithEnchant.ComparisonItemList.IsShown = true;
            itemButtonWithEnchant.ComparisonEnchantList.IsShown = true;
            gem1Button.ComparisonItemList.IsShown = true;
            gem2Button.ComparisonItemList.IsShown = true;
            gem3Button.ComparisonItemList.IsShown = true;*/
        }

        private void SetSocketColor(ItemButton button, ItemSlot slot)
        {
            switch (slot)
            {
                case ItemSlot.Blue:
                    //button.Background = Color.FromKnownColor(KnownColor.Blue);
                    button.Slot = CharacterSlot.Gems;
                    break;
                case ItemSlot.Red:
                    //button.Background = Color.FromKnownColor(KnownColor.Red);
                    button.Slot = CharacterSlot.Gems;
                    break;
                case ItemSlot.Yellow:
                    //button.Background = Color.FromKnownColor(KnownColor.Yellow);
                    button.Slot = CharacterSlot.Gems;
                    break;
                case ItemSlot.Prismatic:
                    //button.Background = Color.FromKnownColor(KnownColor.Silver);
                    button.Slot = CharacterSlot.Gems;
                    break;
                case ItemSlot.Meta:
                    //button.Background = Color.FromKnownColor(KnownColor.Gray);
                    button.Slot = CharacterSlot.Metas;
                    break;
                default:
                    //button.Background = SystemColors.ControlColor;
                    button.Slot = CharacterSlot.Gems;
                    break;
            }
        }

        private void gem1Button_Click(object sender, EventArgs e)
        {
            ItemInstance item = CustomInstance.Clone();
            item.Gem1 = null;
            SetFormItemSelection(item);
        }

        private void gem2Button_Click(object sender, EventArgs e)
        {
            ItemInstance item = CustomInstance.Clone();
            item.Gem2 = null;
            SetFormItemSelection(item);
        }

        private void gem3Button_Click(object sender, EventArgs e)
        {
            ItemInstance item = CustomInstance.Clone();
            item.Gem3 = null;
            SetFormItemSelection(item);
        }

        private void SetFormItemSelection(ItemInstance item)
        {
            if (MainPage.Instance.Character != null)
            {
                Character emptyGemCharacter = MainPage.Instance.Character.Clone();
                emptyGemCharacter[CharacterSlot] = item;
                _formItemSelection.Character = emptyGemCharacter;
                //_formItemSelection.Character.CurrentCalculations = Calculations.GetCharacterCalculations(emptyGemCharacter);
            }
        }    
    }
}

