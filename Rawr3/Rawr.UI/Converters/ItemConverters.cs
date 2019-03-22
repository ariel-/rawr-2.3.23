using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace Rawr.UI
{

    public class GemVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ItemInstance instance = value as ItemInstance;
            if (instance != null)
            {
                if (parameter.ToString() == "1")
                    return instance.Gem1Id > 0 ? Visibility.Visible : Visibility.Collapsed;
                if (parameter.ToString() == "2")
                    return instance.Gem2Id > 0 ? Visibility.Visible : Visibility.Collapsed;
                if (parameter.ToString() == "3")
                    return instance.Gem3Id > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    public class ItemQualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Item item = value as Item;
            if (item == null) return new SolidColorBrush(Colors.Black);
            ItemQuality quality = item.Quality;
            switch (quality)
            {
                case ItemQuality.Artifact:
                case ItemQuality.Heirloom:
                    return new SolidColorBrush(Colors.Yellow);
                case ItemQuality.Legendary:
                    return new SolidColorBrush(Colors.Orange);
                case ItemQuality.Epic:
                    return new SolidColorBrush(Colors.Purple);
                case ItemQuality.Rare:
                    return new SolidColorBrush(Colors.Blue);
                case ItemQuality.Uncommon:
                    return new SolidColorBrush(Colors.Green);
                case ItemQuality.Common:
                    return new SolidColorBrush(Colors.Gray);
                case ItemQuality.Poor:
                    return new SolidColorBrush(Colors.DarkGray);
                case ItemQuality.Temp:
                default:
                    return new SolidColorBrush(Colors.Black);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
