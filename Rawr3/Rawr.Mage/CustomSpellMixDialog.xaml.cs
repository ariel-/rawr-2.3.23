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
using System.Windows.Data;
using System.Reflection;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;

namespace Rawr.Mage
{
    public partial class CustomSpellMixDialog : ChildWindow
    {
        public CustomSpellMixDialog(List<SpellWeight> spellMix)
        {
            InitializeComponent();

            DataContext = new ObservableCollectionWrapper<SpellWeight>(spellMix);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            SpellWeight spellWeight = new SpellWeight() { Spell = SpellEnumConverter.Default };
            ((ObservableCollectionWrapper<SpellWeight>)DataContext).Add(spellWeight);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (Data.SelectedIndex >= 0)
            {
                ((ObservableCollectionWrapper<SpellWeight>)DataContext).RemoveAt(Data.SelectedIndex);
            }
        }
    }

    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            PropertyInfo propertyInfo = value.GetType().GetProperty("Count");
            if (propertyInfo != null)
            {
                int count = (int)propertyInfo.GetValue(value, null);
                return count > 0;
            }
            if (!(value is bool || value is bool?)) return true;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ObservableCollectionWrapper<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollectionWrapper()
        {
        }

        public ObservableCollectionWrapper(IList<T> list)
            : base(list)
        {
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            this.OnCollectionReset();
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            T item = base[oldIndex];
            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, item);
            this.OnPropertyChanged("Item[]");
            //this.OnCollectionChanged(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
            this.OnCollectionReset();
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        private void OnCollectionReset()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected override void RemoveItem(int index)
        {
            T item = base[index];
            base.RemoveItem(index);
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
        }

        protected override void SetItem(int index, T item)
        {
            T oldItem = base[index];
            base.SetItem(index, item);
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
        }
    }

    public class SpellEnumConverter : IValueConverter
    {
        static SpellEnumConverter()
        {
            InitializeValuesAndDescription();
        }

        /// <summary>
        /// Get the description attribute for one enum value
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>The description attribute of an enum, if any</returns>
        private static string GetDescription(Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : null;
        }

        private static Dictionary<int, string> spellToName;
        private static Dictionary<string, SpellId> nameToSpell;
        private static List<string> nameList;

        public static SpellId Default
        {
            get
            {
                return nameToSpell[nameList[0]];
            }
        }

        public List<string> NameList
        {
            get
            {
                return nameList;
            }
        }

        /// <summary>
        /// Gets a list of key/value pairs for an enum, using the description attribute as value
        /// </summary>
        /// <param name="enumType">typeof(your enum type)</param>
        /// <returns>A list of KeyValuePairs with enum values and descriptions</returns>
        private static void InitializeValuesAndDescription()
        {
            spellToName = new Dictionary<int, string>();
            nameToSpell = new Dictionary<string, SpellId>();
            nameList = new List<string>();
            List<KeyValuePair<SpellId, string>> kvPairList = new List<KeyValuePair<SpellId, string>>();

            foreach (SpellId enumValue in EnumHelper.GetValues(typeof(SpellId)))
            {
                string description = GetDescription(enumValue);
                if (description != null)
                {
                    spellToName[(int)enumValue] = description;
                    nameToSpell[description] = enumValue;
                    nameList.Add(description);
                }
            }

            nameList.Sort();
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string name;
            spellToName.TryGetValue((int)value, out name);
            return name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return SpellId.None;
            }
            SpellId spell;
            nameToSpell.TryGetValue((string)value, out spell);
            return spell;
        }
    }
}

