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

namespace Rawr.TankDK
{
    public class GraphTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CalculationType cType = (CalculationType)value;
            if (cType == CalculationType.SMT) return "Default";
            if (cType == CalculationType.Burst) return "Burst";
            else return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string survivalSoftCap = (string)value;
            switch (survivalSoftCap)
            {
                case "Default": return CalculationType.SMT;
                case "Burst": return CalculationType.Burst;
            }
            return null;
        }

        #endregion
    }
}
