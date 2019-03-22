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
    public partial class EditItemFilter : ChildWindow
    {

        private ItemFilterRegex selectedFilter;
        public ItemFilterRegex SelectedFilter
        {
            get { return selectedFilter; }
            set
            {
                selectedFilter = value;
                DataContext = selectedFilter;
                if (selectedFilter == null)
                {
                    SubfilterButton.IsEnabled = false;
                    MoveUpButton.IsEnabled = false;
                    MoveDownButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                }
                else
                {
                    SubfilterButton.IsEnabled = true;
                    MoveUpButton.IsEnabled = true;
                    MoveDownButton.IsEnabled = true;
                    DeleteButton.IsEnabled = true;
                }
            }
        }

        public EditItemFilter()
        {
            InitializeComponent();
            FilterTree.ItemsSource = ItemFilter.FilterList;
            SelectedFilter = null;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ItemCache.OnItemsChanged();
            this.DialogResult = true;
        }

        private void SelectedFilterChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedFilter = FilterTree.SelectedItem as ItemFilterRegex;
        }

        private void NewFilter(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SelectedFilter == null || SelectedFilter.Parent == null) ItemFilter.FilterList.Add(new ItemFilterRegex());
            else SelectedFilter.Parent.RegexList.Add(new ItemFilterRegex());
        }

        private void NewSubfilter(object sender, System.Windows.RoutedEventArgs e)
        {
            SelectedFilter.RegexList.Add(new ItemFilterRegex());
        }

        private void MoveUpFilter(object sender, System.Windows.RoutedEventArgs e)
        {
            ItemFilterRegexList list = null;
            if (SelectedFilter.Parent == null) list = ItemFilter.FilterList;
            else list = SelectedFilter.Parent.RegexList;
            int currentIndex = list.FindIndex(f => f == SelectedFilter);
            if (currentIndex > 0 && list.Count > currentIndex + 1)
            {
                ItemFilterRegex current = list[currentIndex];
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (i == currentIndex) list[currentIndex] = list[currentIndex - 1];
                    else if (i == currentIndex - 1) list[currentIndex - 1] = current;
                    else list[i] = list[i];
                }
            }
        }

        private void DeleteFilter(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SelectedFilter != null)
            {
                if (SelectedFilter.Parent == null) ItemFilter.FilterList.Remove(SelectedFilter);
                else SelectedFilter.Parent.RegexList.Remove(SelectedFilter);
            }
        }

        private void MoveDownFilter(object sender, System.Windows.RoutedEventArgs e)
        {
            ItemFilterRegexList list = null;
            if (SelectedFilter.Parent == null) list = ItemFilter.FilterList;
            else list = SelectedFilter.Parent.RegexList;
            int currentIndex = list.FindIndex(f => f == SelectedFilter);
            if (currentIndex < list.Count - 1 && currentIndex >= 0)
            {
                ItemFilterRegex current = list[currentIndex];
                for (int i = 0; i < list.Count; i++)
                {
                    if (i == currentIndex) list[currentIndex] = list[currentIndex + 1];
                    else if (i == currentIndex + 1) list[currentIndex + 1] = current;
                    else list[i] = list[i];
                }
            }
        }
    }
}

