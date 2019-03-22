using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rawr.UI
{
	public partial class ItemDisplay : UserControl
	{
        public ItemInstance DisplayedItem { get { return DataContext as ItemInstance; } }

		public ItemDisplay(ItemInstance itemInstance)
		{
            DataContext = itemInstance;
			InitializeComponent();
		}

		private void ShowTooltip(object sender, MouseEventArgs e)
		{
            MainPage.Tooltip.ItemInstance = DisplayedItem;
            MainPage.Tooltip.Show(this, ActualWidth, 0);
		}

		private void HideTooltip(object sender, MouseEventArgs e)
		{
            MainPage.Tooltip.Hide();
		}
	}
}