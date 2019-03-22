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
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace Rawr.UI
{
    public partial class ChartPicker : UserControl
    {
        public GraphDisplay GraphDisplay { get; set; }

        public ObservableCollection<ChartPickerItem> Items { get; set; }

        public static readonly DependencyProperty PrimaryItemProperty =
            DependencyProperty.Register("PrimaryItem", typeof(ChartPickerItem), typeof(ChartPicker), new PropertyMetadata(null));
        public ChartPickerItem PrimaryItem
        {
            get { return (ChartPickerItem)GetValue(PrimaryItemProperty); }
            set { SetValue(PrimaryItemProperty, value); }
        }


        public ChartPicker()
        {
            InitializeComponent();
            PopupUtilities.RegisterPopup(this, PopupChartPicker, Toggle, Close);
            Items = new ObservableCollection<ChartPickerItem>();
            Items.Add(new ChartPickerItem("Gear", "Head", "Neck", "Shoulders", "Back", "Chest", "Wrist", "Hands", "Waist", "Legs", "Feet", "Finger 1", "Finger 2", "Trinket 1", "Trinket 2", "Main Hand", "Off Hand", "Ranged", "Projectile", "Projectile Bag"));
            Items.Add(new ChartPickerItem("Enchants", "Head", "Shoulders", "Back", "Chest", "Wrist", "Hands", "Legs", "Feet", "Finger 1", "Finger 2", "Main Hand", "Off Hand", "Ranged"));
            Items.Add(new ChartPickerItem("Gems", "All Normal", "Red", "Blue", "Yellow", "Meta"));
            Items.Add(new ChartPickerItem("Buffs", "All", "Food", "Elixirs and Flasks", "Scrolls", "Potion", "Raid Buffs", "Raid Debuffs", "Set Bonuses"));
            Items.Add(new ChartPickerItem("Talents and Glyphs", "Individual Talents", "Individual Talents (Full)", "Talent Specs", "Glyphs"));
            Items.Add(new ChartPickerItem("Equipped", "All", "Gear", "Enchants", "Buffs"));
            Items.Add(new ChartPickerItem("Available", "Gear", "Enchants"));
            Items.Add(new ChartPickerItem("Direct Upgrades", "Gear", "Gear / Cost", "Enchants"));
            Items.Add(new ChartPickerItem("Stat Values", "Relative Stat Values"));
            //new ChartPickerItem("{0} Specific")

            Calculations.ModelChanged += new EventHandler(Calculations_ModelChanged);

            this.DataContext = this;

			_timer = new DispatcherTimer();
			_timer.Interval = TimeSpan.FromMilliseconds(150);
			_timer.Tick += new EventHandler(_timer_Tick);
        }

        void Calculations_ModelChanged(object sender, EventArgs e)
        {
            bool specificWasSelected = false;
            ChartPickerItem last = Items.Last();
            if (last.Header.EndsWith(" Specific"))
            {
                specificWasSelected = PrimaryItem == last;
                Items.Remove(last);
            }
            last = new ChartPickerItem(Calculations.Instance.Name + " Specific", Calculations.CustomChartNames);
            Items.Add(last);
            if (specificWasSelected)
            {
                PrimaryItem = last;
                SetCurrentGraph();
            }
        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChartPickerItem mousedOverItem = ((FrameworkElement)sender).DataContext as ChartPickerItem;
            _timer.Stop();
			_lastMousedOverItem = mousedOverItem;
			_timer.Start();
		}

		private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			ChartPickerItem mousedOverItem = ((FrameworkElement)sender).DataContext as ChartPickerItem;
			if (_lastMousedOverItem == mousedOverItem)
			{
				_lastMousedOverItem = null;
				_timer.Stop();
			}
		}

		private DispatcherTimer _timer;
		private ChartPickerItem _lastMousedOverItem;
		private void _timer_Tick(object sender, EventArgs e)
		{
			_timer.Stop();
			if (_lastMousedOverItem == null) return;

			PrimaryItem = _lastMousedOverItem;
			ListBoxSecondary.SelectedItem = PrimaryItem.SelectedItem;
		}

        private void Grid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChartPickerItem clickedItem = ((FrameworkElement)sender).DataContext as ChartPickerItem;
            if (ListBoxSecondary.Items.Contains(clickedItem))
                PrimaryItem.SelectedItem = clickedItem;
            SetCurrentGraph();
            Close();
        }

        private void Background_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void SetCurrentGraph()
        {
            TextBlockChartButtonPrimary.Text = PrimaryItem.Header;
            TextBlockChartButtonSecondary.Text = PrimaryItem.SelectedItem == null ? "" : PrimaryItem.SelectedItem.Header;
            GraphDisplay.CurrentGraph = string.Format("{0}|{1}", PrimaryItem, PrimaryItem.SelectedItem);
        }

        private void Close()
        {
            PrimaryItem = Items.FirstOrDefault(chartPickerItem => chartPickerItem.Header == GraphDisplay.CurrentGraph.Split('|')[0]);
            PopupChartPicker.IsOpen = false;
        }

        public class ChartPickerItem : DependencyObject
        {
            public static readonly DependencyProperty ItemsProperty =
                DependencyProperty.Register("Items", typeof(ChartPickerItem[]), typeof(ChartPickerItem), new PropertyMetadata(null));
            public ChartPickerItem[] Items
            {
                get { return (ChartPickerItem[])GetValue(ItemsProperty); }
                set { SetValue(ItemsProperty, value); }
            }

            public static readonly DependencyProperty HeaderProperty =
                DependencyProperty.Register("Header", typeof(string), typeof(ChartPickerItem), new PropertyMetadata(null));
            public string Header
            {
                get { return (string)GetValue(HeaderProperty); }
                set { SetValue(HeaderProperty, value); }
            }

            public static readonly DependencyProperty SelectedItemProperty =
                DependencyProperty.Register("SelectedItem", typeof(ChartPickerItem), typeof(ChartPickerItem), new PropertyMetadata(null));
            public ChartPickerItem SelectedItem
            {
                get { return (ChartPickerItem)GetValue(SelectedItemProperty); }
                set { SetValue(SelectedItemProperty, value); }
            }

            public ChartPickerItem() { }
            public ChartPickerItem(string header, params string[] children)
            {
                Header = header;
                Items = children.Select(child => new ChartPickerItem(child)).ToArray();
                SelectedItem = Items.FirstOrDefault();
            }

            public override string ToString()
            {
                return Header;
            }
        }
    }
}
