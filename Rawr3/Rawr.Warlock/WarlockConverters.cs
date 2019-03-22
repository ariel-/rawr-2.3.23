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
using System.Windows.Data;
using System.Globalization;

namespace Rawr.Warlock
{
    public class PercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(float)) return System.Convert.ToSingle(value) * 100.0f;
            if (targetType == typeof(double)) return System.Convert.ToDouble(value) * 100.0d;
            return System.Convert.ToDouble(value) * 100.0d;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(float)) return System.Convert.ToSingle(value) / 100f;
            if (targetType == typeof(double)) return System.Convert.ToDouble(value) / 100d;
            return System.Convert.ToDouble(value) / 100d;
        }
    }
}
