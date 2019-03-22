using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace Rawr.UI
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
#if SILVERLIGHT
            if (value == null) return Visibility.Collapsed;
            PropertyInfo propertyInfo = value.GetType().GetProperty("Count");
            if (propertyInfo != null)
            {
                int count = (int)propertyInfo.GetValue(value, null);
                return count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            if (!(value is bool || value is bool?)) return Visibility.Visible;
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
#else
            if (value == null) return Visibility.Hidden;
            PropertyInfo propertyInfo = value.GetType().GetProperty("Count");
            if (propertyInfo != null)
            {
                int count = (int)propertyInfo.GetValue(value, null);
                return count > 0 ? Visibility.Visible : Visibility.Hidden;
            }
            if (!(value is bool || value is bool?)) return Visibility.Visible;
            return (bool)value ? Visibility.Visible : Visibility.Hidden;
#endif
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
