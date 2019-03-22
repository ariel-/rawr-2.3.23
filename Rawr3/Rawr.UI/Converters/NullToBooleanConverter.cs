using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Data;
using System.Globalization;

namespace Rawr.UI
{
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
}
