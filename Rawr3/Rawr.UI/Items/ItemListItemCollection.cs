using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Generic;

namespace Rawr.UI
{
	public class ItemListItemCollection : DependencyObject, IEnumerable<ItemListItem>, ICollection<ItemListItem>, INotifyCollectionChanged
	{
		private List<ItemListItem> _list = new List<ItemListItem>();

		public static readonly DependencyProperty FilterProperty =
			DependencyProperty.Register("Filter", typeof(string), typeof(ItemListItemCollection), 
			new PropertyMetadata(new PropertyChangedCallback(FilterProperty_Changed)));
		public string Filter
		{
			get { return (string)GetValue(FilterProperty); }
			set { SetValue(FilterProperty, value); }
		}

		private static void FilterProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			ItemListItemCollection collection = sender as ItemListItemCollection;
			if (collection != null)
			{
				collection.OnCollectionChanged();
			}
		}

		public static readonly DependencyProperty SortProperty =
			DependencyProperty.Register("Sort", typeof(ComparisonSort), 
			typeof(ItemListItemCollection), new PropertyMetadata(ComparisonSort.Overall, 
				new PropertyChangedCallback(SortProperty_Changed)));
		public ComparisonSort Sort
		{
			get { return (ComparisonSort)GetValue(SortProperty); }
			set { SetValue(SortProperty, value); }
		}

		private static void SortProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			ItemListItemCollection collection = sender as ItemListItemCollection;
			if (collection != null)
			{
				collection.OnCollectionChanged();
			}
		}


		#region INotifyCollectionChanged Members
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		private void OnCollectionChanged()
		{
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		#endregion

		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<ItemListItem>)this).GetEnumerator();
		}
		#endregion

		#region IEnumerable<ItemListItem> Members
		IEnumerator<ItemListItem> IEnumerable<ItemListItem>.GetEnumerator()
		{
			var filteredList = _list
				.Where(listItem => listItem.Name.ToLower().Contains((Filter ?? string.Empty).ToLower()));
			var sortedList = ApplySort(filteredList);

            Dictionary<int, int> countItem = new Dictionary<int, int>();
            foreach (var itemCalculation in sortedList)
            {
                int itemId = (itemCalculation.ItemInstance == null ? itemCalculation.Item.Id : itemCalculation.ItemInstance.Id);
                if (!countItem.ContainsKey(itemId)) countItem.Add(itemId, 0);
                if (countItem[itemId]++ < Properties.GeneralSettings.Default.CountGemmingsShown ||
                    itemCalculation.Equipped || (itemCalculation.ItemInstance != null && itemCalculation.ItemInstance.ForceDisplay))
                {
                    yield return itemCalculation;
                }
            }
			//return sortedList.GetEnumerator();
		}

		private IOrderedEnumerable<ItemListItem> ApplySort(IEnumerable<ItemListItem> filteredList)
		{
			if (Sort == ComparisonSort.Overall)
				return filteredList.OrderByDescending(itemListItem => itemListItem.OverallRating);
			else if (Sort == ComparisonSort.Alphabetical)
				return filteredList.OrderBy<ItemListItem, string>(itemListItem => itemListItem.Name);
			else
				return filteredList.OrderByDescending(itemListItem => 
					itemListItem.Ratings[(int)Sort].Width);
		}
		#endregion

		#region ICollection<ItemListItem> Members

		public void Add(ItemListItem item)
		{
			_list.Add(item);
			OnCollectionChanged();
		}

		public void AddRange(IEnumerable<ItemListItem> items)
		{
			_list.AddRange(items);
			OnCollectionChanged();
		}

		public void Clear()
		{
			_list.Clear();
			OnCollectionChanged();
		}

		public bool Contains(ItemListItem item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(ItemListItem[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _list.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(ItemListItem item)
		{
			return _list.Remove(item);
		}

		public void RemoveRange(int index, int count)
		{
			_list.RemoveRange(index, count);
		}

		#endregion
	}
}
