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
    public partial class ClassModelPicker : UserControl
    {
        public GraphDisplay GraphDisplay { get; set; }

        public ObservableCollection<ClassModelPickerItem> Items { get; set; }

        public static readonly DependencyProperty PrimaryItemProperty =
            DependencyProperty.Register("PrimaryItem",
            typeof(ClassModelPickerItem),
            typeof(ClassModelPicker),
            new PropertyMetadata(null));

        public ClassModelPickerItem PrimaryItem
        {
            get { return (ClassModelPickerItem)GetValue(PrimaryItemProperty); }
            set { SetValue(PrimaryItemProperty, value); }
        }

        public ClassModelPicker()
        {
            InitializeComponent();
            PopupUtilities.RegisterPopup(this, PopupClassModelPicker, Toggle, Close);
            Items = new ObservableCollection<ClassModelPickerItem>();
            Items.Add(new ClassModelPickerItem("Death Knight", "DPSDK", "TankDK"));
            Items.Add(new ClassModelPickerItem("Druid", "Bear", "Cat", "Moonkin", "Tree"));
            Items.Add(new ClassModelPickerItem("Hunter", "Hunter"));
            Items.Add(new ClassModelPickerItem("Mage", "Mage"));
            Items.Add(new ClassModelPickerItem("Paladin", "Healadin", "ProtPaladin", "Retribution"));
            Items.Add(new ClassModelPickerItem("Priest", "HealPriest", "ShadowPriest"));
            Items.Add(new ClassModelPickerItem("Rogue", "Rogue"));
            Items.Add(new ClassModelPickerItem("Shaman", "Elemental", "Enhance", "RestoSham"));
            Items.Add(new ClassModelPickerItem("Warlock", "Warlock"));
            Items.Add(new ClassModelPickerItem("Warrior", "DPSWarr", "ProtWarr"));
            //new ClassModelPickerItem("{0} Specific")

            Calculations.ModelChanged += new EventHandler(Calculations_ModelChanged);

            this.DataContext = this;

			_timer = new DispatcherTimer();
			_timer.Interval = TimeSpan.FromMilliseconds(150);
			_timer.Tick += new EventHandler(_timer_Tick);
        }

        void Calculations_ModelChanged(object sender, EventArgs e)
        {
            // This is replicated from ChartPicker, but it can handle
            // this itself so we don't need to write out own code here
        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ClassModelPickerItem mousedOverItem = ((FrameworkElement)sender).DataContext as ClassModelPickerItem;
            _timer.Stop();
			_lastMousedOverItem = mousedOverItem;
			_timer.Start();
		}

		private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			ClassModelPickerItem mousedOverItem = ((FrameworkElement)sender).DataContext as ClassModelPickerItem;
			if (_lastMousedOverItem == mousedOverItem)
			{
				_lastMousedOverItem = null;
				_timer.Stop();
			}
		}

		private DispatcherTimer _timer;
		private ClassModelPickerItem _lastMousedOverItem;
		private void _timer_Tick(object sender, EventArgs e)
		{
			_timer.Stop();
			if (_lastMousedOverItem == null) return;

			PrimaryItem = _lastMousedOverItem;
			ListBoxSecondary.SelectedItem = PrimaryItem.SelectedItem;
		}

        private void Grid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ClassModelPickerItem clickedItem = ((FrameworkElement)sender).DataContext as ClassModelPickerItem;
            if (ListBoxSecondary.Items.Contains(clickedItem))
                PrimaryItem.SelectedItem = clickedItem;
            SetCurrentModel();
            Close();
        }

        private void Background_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void SetCurrentModel()
        {
            TextBlockClassModelButtonPrimary.Text = PrimaryItem.Header;
            TextBlockClassModelButtonSecondary.Text = PrimaryItem.SelectedItem == null ? "" : PrimaryItem.SelectedItem.Header;
            foreach (string item in MainPage.Instance.ModelCombo.Items)
            {
                if (item == PrimaryItem.SelectedItem.Header)
                {
                    MainPage.Instance.ModelCombo.SelectedItem = item;
                }
            }
            //GraphDisplay.CurrentGraph = string.Format("{0}|{1}", PrimaryItem, PrimaryItem.SelectedItem);
        }

        private void Close()
        {
            PrimaryItem = Items.FirstOrDefault(classModelPickerItem => true/*classModelPickerItem.Header == GraphDisplay.CurrentGraph.Split('|')[0]*/);
            PopupClassModelPicker.IsOpen = false;
        }

        public class ClassModelPickerItem : DependencyObject
        {
            public static readonly DependencyProperty ItemsProperty =
                DependencyProperty.Register("Items", typeof(ClassModelPickerItem[]), typeof(ClassModelPickerItem), new PropertyMetadata(null));
            public ClassModelPickerItem[] Items
            {
                get { return (ClassModelPickerItem[])GetValue(ItemsProperty); }
                set { SetValue(ItemsProperty, value); }
            }

            public static readonly DependencyProperty HeaderProperty =
                DependencyProperty.Register("Header", typeof(string), typeof(ClassModelPickerItem), new PropertyMetadata(null));
            public string Header
            {
                get { return (string)GetValue(HeaderProperty); }
                set { SetValue(HeaderProperty, value); }
            }

            public static readonly DependencyProperty SelectedItemProperty =
                DependencyProperty.Register("SelectedItem", typeof(ClassModelPickerItem), typeof(ClassModelPickerItem), new PropertyMetadata(null));
            public ClassModelPickerItem SelectedItem
            {
                get { return (ClassModelPickerItem)GetValue(SelectedItemProperty); }
                set { SetValue(SelectedItemProperty, value); }
            }

            public ClassModelPickerItem() { }
            public ClassModelPickerItem(string header, params string[] children)
            {
                Header = header;
                Items = children.Select(child => new ClassModelPickerItem(child)).ToArray();
                SelectedItem = Items.FirstOrDefault();
            }

            public override string ToString()
            {
                return Header;
            }
        }
    }
}
