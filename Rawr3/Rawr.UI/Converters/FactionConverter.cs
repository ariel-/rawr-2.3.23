using System;
using System.Globalization;
using System.Windows.Data;
using System.Reflection;

namespace Rawr.UI
{
    public class FactionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(ItemFaction)) return (ItemFaction)value;
            if (targetType == typeof(string)) return ((ItemFaction)(value)).ToString();
            if (targetType == typeof(int)) return System.Convert.ToInt32(value);
            return System.Convert.ToDouble(value);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(string) && targetType == typeof(ItemFaction)) return StringToItemFaction((string)value);
            if (targetType == typeof(ItemFaction)) return (ItemFaction)value;
            if (targetType == typeof(string)) return ((ItemFaction)(value)).ToString();
            if (targetType == typeof(int)) return System.Convert.ToInt32(value);
            return System.Convert.ToDouble(value);
        }
        private ItemFaction StringToItemFaction(string source) {
            if (source == "Alliance") return ItemFaction.Alliance;
            if (source == "Horde") return ItemFaction.Horde;
            return ItemFaction.Neutral;
        }
    }
}
