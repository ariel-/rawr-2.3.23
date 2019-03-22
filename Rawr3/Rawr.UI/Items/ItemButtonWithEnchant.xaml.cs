using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Rawr;
using System.Windows.Controls.Primitives;

namespace Rawr.UI
{
    public partial class ItemButtonWithEnchant : UserControl
    {

        private CharacterSlot slot;
        public CharacterSlot Slot
        {
            get { return slot; }
            set
            {
                slot = value;
                ComparisonItemList.Slot = slot;
                ComparisonEnchantList.Slot = slot;
            }
        }

        public ItemInstance Item;
        private Character character;
        public Character Character
        {
            get { return character; }
            set
            {
                if (character != null) character.AvailableItemsChanged -= new EventHandler(character_CalculationsInvalidated);
                character = value;
                ComparisonItemList.Character = character;
                ComparisonItemListGem1.Character = character;
                ComparisonItemListGem2.Character = character;
                ComparisonItemListGem3.Character = character;
                ComparisonEnchantList.Character = character;
                if (character != null)
                {
                    character.CalculationsInvalidated += new EventHandler(character_CalculationsInvalidated);
                    character_CalculationsInvalidated(null, null);
                }
            }
        }

        private ItemInstance gear;
        private Item enchant;
        public void character_CalculationsInvalidated(object sender, EventArgs e)
        {
            if (character != null)
            {
                Item = character[Slot];
                if (Item == null) {
                    IconImage.Source = null;
                    EnchantButton.Content = "";
                    gear = null;
                    gear = null;
                    IconImageGem1.Source = null; GemButton1.Visibility = Visibility.Collapsed;
                    IconImageGem2.Source = null; GemButton2.Visibility = Visibility.Collapsed;
                    IconImageGem3.Source = null; GemButton3.Visibility = Visibility.Collapsed;
                } else {
                    IconImage.Source = Icons.ItemIcon(Item.Item.IconPath);
                    EnchantButton.Content = Item.Enchant.ShortName;
                    gear = Item;

                    Item eItem = new Item();
                    eItem.Name = Item.Enchant.Name;
                    eItem.Quality = ItemQuality.Temp;
                    eItem.Stats = Item.Enchant.Stats;
                    enchant = eItem;

                    // There are several special sockets, we need to account for them
                    int nonBSSocketCount = (Item.Item.SocketColor1 != ItemSlot.None ? 1 : 0)
                                         + (Item.Item.SocketColor2 != ItemSlot.None ? 1 : 0)
                                         + (Item.Item.SocketColor3 != ItemSlot.None ? 1 : 0);
                    bool BSSlot_Wrist = Slot == CharacterSlot.Wrist && Character.WristBlacksmithingSocketEnabled && (Character.PrimaryProfession == Profession.Blacksmithing || Character.SecondaryProfession == Profession.Blacksmithing);
                    bool BSSlot_Glove = Slot == CharacterSlot.Hands && Character.HandsBlacksmithingSocketEnabled && (Character.PrimaryProfession == Profession.Blacksmithing || Character.SecondaryProfession == Profession.Blacksmithing);
                    bool BSSlot_Waist = Slot == CharacterSlot.Waist && Character.WaistBlacksmithingSocketEnabled;
                    // If there is no gem socket there, hide the selector
                    GemButton1.Visibility = Item.Item.SocketColor1 != ItemSlot.None
                                            || (nonBSSocketCount == 0 && (BSSlot_Wrist || BSSlot_Glove || BSSlot_Waist))
                        ? Visibility.Visible : Visibility.Collapsed;
                    GemButton2.Visibility = Item.Item.SocketColor2 != ItemSlot.None
                                            || (nonBSSocketCount == 1 && (BSSlot_Wrist || BSSlot_Glove || BSSlot_Waist))
                        ? Visibility.Visible : Visibility.Collapsed;
                    GemButton3.Visibility = Item.Item.SocketColor3 != ItemSlot.None
                                            || (nonBSSocketCount == 2 && (BSSlot_Wrist || BSSlot_Glove || BSSlot_Waist))
                        ? Visibility.Visible : Visibility.Collapsed;
                    // Use the Gem's image if it exists
                    IconImageGem1.Source = Item.Gem1 != null ? Icons.ItemIcon(Item.Gem1.IconPath) : null;
                    IconImageGem2.Source = Item.Gem2 != null ? Icons.ItemIcon(Item.Gem2.IconPath) : null;
                    IconImageGem3.Source = Item.Gem3 != null ? Icons.ItemIcon(Item.Gem3.IconPath) : null;
                    // Remove any previously registered Events that can be fired
                    ComparisonItemListGem1.SelectedItemsGemChanged -= new EventHandler(ComparisonItemListGem1_SelectedItemsGemChanged);
                    ComparisonItemListGem2.SelectedItemsGemChanged -= new EventHandler(ComparisonItemListGem2_SelectedItemsGemChanged);
                    ComparisonItemListGem3.SelectedItemsGemChanged -= new EventHandler(ComparisonItemListGem3_SelectedItemsGemChanged);
                    // Set up the selector so that it uses the right slot type
                    ComparisonItemListGem1.Slot = Item.Item.SocketColor1 == ItemSlot.Meta ? CharacterSlot.Metas : CharacterSlot.Gems;
                    ComparisonItemListGem2.Slot = CharacterSlot.Gems;
                    ComparisonItemListGem3.Slot = CharacterSlot.Gems;
                    // Set the Gem lists' selected items to the core item's gems
                    ComparisonItemListGem1.SelectedItem = Item.Gem1;
                    ComparisonItemListGem2.SelectedItem = Item.Gem2;
                    ComparisonItemListGem3.SelectedItem = Item.Gem3;
                    // Let the comp lists know they need to behave differently from a normal list
                    ComparisonItemListGem1.IsAnItemsGem = true;
                    ComparisonItemListGem2.IsAnItemsGem = true;
                    ComparisonItemListGem3.IsAnItemsGem = true;
                    // Register the Events that can be fired
                    ComparisonItemListGem1.SelectedItemsGemChanged += new EventHandler(ComparisonItemListGem1_SelectedItemsGemChanged);
                    ComparisonItemListGem2.SelectedItemsGemChanged += new EventHandler(ComparisonItemListGem2_SelectedItemsGemChanged);
                    ComparisonItemListGem3.SelectedItemsGemChanged += new EventHandler(ComparisonItemListGem3_SelectedItemsGemChanged);
                }
            }
        }

        public ItemButtonWithEnchant()
        {
            // Required to initialize variables
            InitializeComponent();
            PopupUtilities.RegisterPopup(this, ListPopup, EnchantPopup, Close);
            ComparisonEnchantList.IsEnchantList = true;
            PopupUtilities.RegisterPopup(this, ListPopupGem1, Close);
            PopupUtilities.RegisterPopup(this, ListPopupGem2, Close);
            PopupUtilities.RegisterPopup(this, ListPopupGem3, Close);
        }

        private void Close()
        {
            if (ListPopup.IsOpen) { ListPopup.IsOpen = false; }
            if (EnchantPopup.IsOpen) { EnchantPopup.IsOpen = false; }
            if (ListPopupGem1.IsOpen) { ListPopupGem1.IsOpen = false; }
            if (ListPopupGem2.IsOpen) { ListPopupGem2.IsOpen = false; }
            if (ListPopupGem3.IsOpen) { ListPopupGem3.IsOpen = false; }
        }

        private void EnsurePopupsVisible()
        {
            GeneralTransform transform = TransformToVisual(App.Current.RootVisual);
            double distBetweenBottomOfPopupAndBottomOfWindow = App.Current.RootVisual.DesiredSize.Height - transform.Transform(new Point(0, ComparisonItemList.Height)).Y;
            if (distBetweenBottomOfPopupAndBottomOfWindow < 0) {
                ListPopup.VerticalOffset = distBetweenBottomOfPopupAndBottomOfWindow;
            } 
            distBetweenBottomOfPopupAndBottomOfWindow = App.Current.RootVisual.RenderSize.Height - transform.Transform(new Point(0, 66 + ComparisonEnchantList.Height)).Y;
            if (distBetweenBottomOfPopupAndBottomOfWindow < 0) {
                EnchantPopup.VerticalOffset = 66 + distBetweenBottomOfPopupAndBottomOfWindow;
            }
        }
        private void EnsureGem1PopupsVisible()
        {
            GeneralTransform transform = TransformToVisual(App.Current.RootVisual);
            double distBetweenBottomOfPopupAndBottomOfWindow = App.Current.RootVisual.RenderSize.Height - transform.Transform(new Point(0, ComparisonItemListGem1.Height)).Y;
            if (distBetweenBottomOfPopupAndBottomOfWindow < 0)
            {
                ListPopupGem1.VerticalOffset = distBetweenBottomOfPopupAndBottomOfWindow;
            }
        }
        private void EnsureGem2PopupsVisible()
        {
            GeneralTransform transform = TransformToVisual(App.Current.RootVisual);
            double distBetweenBottomOfPopupAndBottomOfWindow = App.Current.RootVisual.RenderSize.Height - transform.Transform(new Point(0, ComparisonItemListGem2.Height)).Y;
            if (distBetweenBottomOfPopupAndBottomOfWindow < 0)
            {
                ListPopupGem2.VerticalOffset = distBetweenBottomOfPopupAndBottomOfWindow;
            }
        }
        private void EnsureGem3PopupsVisible()
        {
            GeneralTransform transform = TransformToVisual(App.Current.RootVisual);
            double distBetweenBottomOfPopupAndBottomOfWindow = App.Current.RootVisual.RenderSize.Height - transform.Transform(new Point(0, ComparisonItemListGem3.Height)).Y;
            if (distBetweenBottomOfPopupAndBottomOfWindow < 0)
            {
                ListPopupGem3.VerticalOffset = distBetweenBottomOfPopupAndBottomOfWindow;
            }
        }

        private void MainButton_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            MainPage.Tooltip.Hide();
            EnsurePopupsVisible();
            ComparisonItemList.IsShown = true;
            ListPopup.IsOpen = true;
            ComparisonItemList.Focus();
        }
        private void GemButton1_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Tooltip.Hide();
            EnsureGem1PopupsVisible();
            ComparisonItemListGem1.IsShown = true;
            ListPopupGem1.IsOpen = true;
            ComparisonItemListGem1.Focus();
        }
        private void GemButton2_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Tooltip.Hide();
            EnsureGem2PopupsVisible();
            ComparisonItemListGem2.IsShown = true;
            ListPopupGem2.IsOpen = true;
            ComparisonItemListGem2.Focus();
        }
        private void GemButton3_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Tooltip.Hide();
            EnsureGem3PopupsVisible();
            ComparisonItemListGem3.IsShown = true;
            ListPopupGem3.IsOpen = true;
            ComparisonItemListGem3.Focus();
        }
        private void EnchantButton_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            MainPage.Tooltip.Hide();
            EnsurePopupsVisible();
            ComparisonEnchantList.IsShown = true;
            EnchantPopup.IsOpen = true;
            ComparisonEnchantList.Focus();
        }

        #region Returning Gem Events
        public void ComparisonItemListGem1_SelectedItemsGemChanged(object sender, EventArgs e) { if (Item != null) Item.Gem1 = ComparisonItemListGem1.SelectedItem; character.OnCalculationsInvalidated(); character_CalculationsInvalidated(null, null); }
        public void ComparisonItemListGem2_SelectedItemsGemChanged(object sender, EventArgs e) { if (Item != null) Item.Gem2 = ComparisonItemListGem2.SelectedItem; character.OnCalculationsInvalidated(); character_CalculationsInvalidated(null, null); }
        public void ComparisonItemListGem3_SelectedItemsGemChanged(object sender, EventArgs e) { if (Item != null) Item.Gem3 = ComparisonItemListGem3.SelectedItem; character.OnCalculationsInvalidated(); character_CalculationsInvalidated(null, null); }
        #endregion

        private void MainButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!ListPopup.IsOpen)
            {
                MainPage.Tooltip.ItemInstance = gear;
                MainPage.Tooltip.Show(MainButton, 72, 0);
            }
        }
        private void GemButton1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!ListPopup.IsOpen && !ListPopupGem1.IsOpen && !ListPopupGem2.IsOpen && !ListPopupGem3.IsOpen)
            {
                MainPage.Tooltip.Item = Item.Gem1;
                MainPage.Tooltip.Show(GemButton1, 22, 0);
            }
        }
        private void GemButton2_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!ListPopup.IsOpen && !ListPopupGem1.IsOpen && !ListPopupGem2.IsOpen && !ListPopupGem3.IsOpen)
            {
                MainPage.Tooltip.Item = Item.Gem2;
                MainPage.Tooltip.Show(GemButton2, 22, 0);
            }
        }
        private void GemButton3_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!ListPopup.IsOpen && !ListPopupGem1.IsOpen && !ListPopupGem2.IsOpen && !ListPopupGem3.IsOpen)
            {
                MainPage.Tooltip.Item = Item.Gem3;
                MainPage.Tooltip.Show(GemButton3, 22, 0);
            }
        }
        private void EnchantButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!EnchantPopup.IsOpen)
            {
                MainPage.Tooltip.Item = enchant;
                MainPage.Tooltip.Show(EnchantButton, 72, 0);
            }
        }

        private void AnyButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MainPage.Tooltip.Hide();
        }
    }
}