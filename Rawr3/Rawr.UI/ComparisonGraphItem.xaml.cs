using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Rawr.UI
{
    public partial class ComparisonGraphItem : UserControl
    {

        private ItemInstance itemInstance;
        public ItemInstance ItemInstance
        {
            get { return itemInstance; }
            set
            {
                itemInstance = value;
                if (itemInstance != null)
                    ContextItemName.Header = itemInstance.Item.Name;
                character_AvailableItemsChanged(this, EventArgs.Empty);
            }
        }

        private CharacterSlot slot;
        public CharacterSlot Slot
        {
            get { return slot; }
            set
            {
                slot = value;
            }
        }

        private Item item;
        public Item OtherItem
        {
            get { return item; }
            set
            {
                item = value;
                character_AvailableItemsChanged(this, EventArgs.Empty);
            }
        }


        private Character character;
        public Character Character
        {
            get { return character; }
            set
            {
                if (character != null)
                    character.AvailableItemsChanged -= new EventHandler(character_AvailableItemsChanged);
                
                character = value;

                if (character != null)
                    character.AvailableItemsChanged += new EventHandler(character_AvailableItemsChanged);
                
                character_AvailableItemsChanged(this, EventArgs.Empty);
            }
        }

        private void character_AvailableItemsChanged(object sender, EventArgs e)
        {
            if (Character != null && ((ItemInstance != null && ItemInstance.Id > 0) || (OtherItem != null && OtherItem.Id != 0)))
            {
                AvailableImage.Visibility = Visibility.Visible;
                ItemAvailability itemAvailability;
                if (ItemInstance != null) itemAvailability = Character.GetItemAvailability(ItemInstance);
                else itemAvailability = Character.GetItemAvailability(OtherItem);
                switch (itemAvailability)
                {
                    case ItemAvailability.RegemmingAllowed:
                        AvailableImage.Source = new BitmapImage(new Uri("Images/Diamond.png", UriKind.Relative));
                        break;
                    case ItemAvailability.RegemmingAllowedWithEnchantRestrictions:
                        AvailableImage.Source = new BitmapImage(new Uri("Images/Diamond3.png", UriKind.Relative));
                        break;
                    case ItemAvailability.Available:
                        AvailableImage.Source = new BitmapImage(new Uri("Images/Diamond2.png", UriKind.Relative));
                        break;
                    case ItemAvailability.AvailableWithEnchantRestrictions:
                        AvailableImage.Source = new BitmapImage(new Uri("Images/Diamond4.png", UriKind.Relative));
                        break;
                    case ItemAvailability.NotAvailable:
                        AvailableImage.Source = new BitmapImage(new Uri("Images/DiamondOutline.png", UriKind.Relative));
                        break;
                }

            }
            else
            {
                AvailableImage.Visibility = Visibility.Collapsed;
            }
        }

        public string Title
        {
            get { return TextLabel.Text; }
            set { TextLabel.Text = value; }
        }

        public bool Equipped
        {
            get { return EquippedRect.Visibility == Visibility.Visible; }
            set { EquippedRect.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool PartEquipped
        {
            get { return PartEquippedRect.Visibility == Visibility.Visible; }
            set { PartEquippedRect.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        public float MinScale { get; set; }
        public float MaxScale { get; set; }

        private List<ComparisonGraphBar> rects;
        private List<float> values;
        public float this[int i]
        {
            get { return values[i]; }
            set
            {
                values[i] = value;
                rects[i].Title = Math.Round(value, 2).ToString();
                TotalLabel.Text = Math.Round(values.Sum(), 2).ToString();
                ChangedSize(this, null);
            }
        }

        public void SetColors(IEnumerable<Color> colors)
        {
            rects = new List<ComparisonGraphBar>();
            values = new List<float>(rects.Count);

            foreach (Color c in colors)
            {
                ComparisonGraphBar r = new ComparisonGraphBar() { Color = c };
                r.Height = 30;
                r.Margin = new Thickness(0, 4, 0, 4);
                rects.Add(r);
                values.Add(0f);
            }
        }

        public ComparisonGraphItem(IEnumerable<Color> colors) : this() { SetColors(colors); }
        public ComparisonGraphItem()
        {
            // Required to initialize variables
            InitializeComponent();
            NameGrid.Width = GraphBarStart;
        }

        private float _GraphBarStart = 142f;
        public float GraphBarStart
        {
            get { return _GraphBarStart + (Rawr.Properties.GeneralSettings.Default.ItemNameWidthSetting * 20); }
            set { _GraphBarStart = value; }
        }

        private void ChangedSize(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (ActualWidth > (GraphBarStart + 8))//150
            {
                NameGridCol.Width = new GridLength(GraphBarStart);
                NameGrid.Width = GraphBarStart;
                PositiveStack.Children.Clear();
                NegativeStack.Children.Clear();

                int minTick = (int)(-MinScale / (MaxScale - MinScale) * 8);
                int maxTick = (int)(MaxScale / (MaxScale - MinScale) * 8);

                if (minTick == 0) NegativeStack.Visibility = Visibility.Collapsed;
                else
                {
                    Grid.SetColumnSpan(NegativeStack, minTick);
                    NegativeStack.Visibility = Visibility.Visible;
                }
                if (maxTick == 0) PositiveStack.Visibility = Visibility.Collapsed;
                else
                {
                    Grid.SetColumn(PositiveStack, minTick + 1);
                    Grid.SetColumnSpan(PositiveStack, maxTick);
                    PositiveStack.Visibility = Visibility.Visible;
                }
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i] > 0)
                    {
                        rects[i].Width = (ActualWidth - (GraphBarStart + 9)) * (values[i] / (MaxScale - MinScale)); // 151
                        PositiveStack.Children.Add(rects[i]);
                    }
                    else
                    {
                        rects[i].Width = (ActualWidth - (GraphBarStart + 9)) * (-values[i] / (MaxScale - MinScale)); // 151
                        NegativeStack.Children.Add(rects[i]);
                    }
                }
                PositiveStack.Children.Add(TotalLabel);
            }
        }

        private void AvailableClicked(object sender, MouseButtonEventArgs e)
        {
            if (ItemInstance != null && ItemInstance.Id != 0)
                Character.ToggleItemAvailability(ItemInstance, (Keyboard.Modifiers & ModifierKeys.Shift) == 0);
            else if (OtherItem != null && OtherItem.Id != 0)
                Character.ToggleItemAvailability(OtherItem, (Keyboard.Modifiers & ModifierKeys.Shift) == 0);

            e.Handled = true;
        }


        private void EditItem(object sender, RoutedEventArgs e)
        {
            new ItemEditor() { CurrentItem = ItemInstance.Item }.Show();
        }

        private void OpenInWowhead(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("http://www.wowhead.com/?item=" + ItemInstance.Id), "_blank");
#else
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("http://www.wowhead.com/?item=" + SelectedItemInstance.Id));
#endif
        }

        private void RefreshItemFromArmory(object sender, RoutedEventArgs e)
        {
            Item.LoadFromId(ItemInstance.Id, true, true, false);
        }

        private void RefreshItemFromWowhead(object sender, RoutedEventArgs e)
        {
            Item.LoadFromId(ItemInstance.Id, true, true, true);
        }

        private void EquipItem(object sender, RoutedEventArgs e)
        {
            Character[Slot] = ItemInstance;
        }

        private void AddCustomGemming(object sender, RoutedEventArgs e)
        {
            CustomItemInstance custom = new CustomItemInstance(Character, ItemInstance);
            custom.Show();
        }

        private void DeleteCustomGemming(object sender, RoutedEventArgs e)
        {
            Character.CustomItemInstances.Remove(ItemInstance);
            ItemCache.OnItemsChanged();
        }

        private void EvaluateUpgrade(object sender, RoutedEventArgs e)
        {
            OptimizeWindow optimizer = new OptimizeWindow(Character);
            optimizer.Show();
            optimizer.EvaluateUpgrades(ItemInstance.Item);
        }

        private void ContextMenuItem_Opened(object sender, RoutedEventArgs e)
        {
            if (ItemInstance == null)
                ContextMenuItem.IsOpen = false;
        }

        private void NameGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //TODO: *Usually* this is enough, but sometimes the positioning gets off, and it stays where the last context menu was.
            //Not really sure why, but I guess we should set the context menu location here too?
            ContextMenuService.GetContextMenu(NameGrid).IsOpen = true;
        }
    }
}