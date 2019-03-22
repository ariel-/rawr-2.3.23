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

namespace Rawr.UI
{

    public class GemNameConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ItemInstance instance = value as ItemInstance;
            if (instance != null)
            {
                if (parameter.ToString() == "1")
                    return instance.Gem1Id > 0 ? instance.Gem1.Name : "";
                if (parameter.ToString() == "2")
                    return instance.Gem2Id > 0 ? instance.Gem2.Name : "";
                if (parameter.ToString() == "3")
                    return instance.Gem3Id > 0 ? instance.Gem3.Name : "";
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class GemColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ItemSlot gemType = ItemSlot.None;

            if (value is Item)
            {
                Item item = value as Item;
                if (parameter.ToString() == "1") gemType = item.SocketColor1;
                else if (parameter.ToString() == "2") gemType = item.SocketColor2;
                else if (parameter.ToString() == "3") gemType = item.SocketColor3;
            }
            else if (value is ItemInstance)
            {
                ItemInstance instance = value as ItemInstance;
                Item item = instance.Item;
                if (parameter.ToString() == "1")
                {
                    if (instance.Gem1Id > 0)
                    {
                        if (item.SocketColor1 == ItemSlot.None) gemType = ItemSlot.Prismatic;
                        else gemType = item.SocketColor1;
                    }
                }
                else if (parameter.ToString() == "2")
                {
                    if (instance.Gem2Id > 0)
                    {
                        if (item.SocketColor2 == ItemSlot.None) gemType = ItemSlot.Prismatic;
                        else gemType = item.SocketColor2;
                    }
                }
                else if (parameter.ToString() == "3")
                {
                    if (instance.Gem3Id > 0)
                    {
                        if (item.SocketColor3 == ItemSlot.None) gemType = ItemSlot.Prismatic;
                        else gemType = item.SocketColor3;
                    }
                }
            }
            
            switch (gemType)
            {
                case ItemSlot.Red:
                    return new SolidColorBrush(Colors.Red);
                case ItemSlot.Yellow:
                    return new SolidColorBrush(Colors.Yellow);
                case ItemSlot.Blue:
                    return new SolidColorBrush(Colors.Blue);
                case ItemSlot.Orange:
                    return new SolidColorBrush(Colors.Orange);
                case ItemSlot.Purple:
                    return new SolidColorBrush(Colors.Purple);
                case ItemSlot.Green:
                    return new SolidColorBrush(Colors.Green);
                case ItemSlot.Prismatic:
                    return new SolidColorBrush(Colors.LightGray);
                case ItemSlot.Meta:
                    return new SolidColorBrush(Colors.Gray);
                default:
                    return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
