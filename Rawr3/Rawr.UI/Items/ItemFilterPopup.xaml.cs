using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace Rawr.UI
{
    public partial class ItemFilterPopup : UserControl
    {
        public ItemFilterPopup()
        {
            // Required to initialize variables
            InitializeComponent();
            PopupUtilities.RegisterPopup(this, PopupFilter, Toggle, Close);
            FilterTree.ItemsSource = ItemFilter.FilterList.FilterList;
        }

        private void Close()
        {
            PopupFilter.IsOpen = false;
        }

        private void Background_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}