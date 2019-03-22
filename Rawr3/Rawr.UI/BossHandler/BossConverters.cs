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

namespace Rawr.UI
{
    public class AttackTypeValueConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ATTACK_TYPES atValue = (ATTACK_TYPES)value;
            if (atValue == ATTACK_TYPES.AT_MELEE) return "Melee";
            else if (atValue == ATTACK_TYPES.AT_RANGED) return "Ranged";
            else if (atValue == ATTACK_TYPES.AT_AOE) return "AoE";
            return ATTACK_TYPES.AT_MELEE;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string atValue = (string)value;
            switch (atValue)
            {
                case "Melee": return ATTACK_TYPES.AT_MELEE;
                case "Ranged": return ATTACK_TYPES.AT_RANGED;
                case "AoE": return ATTACK_TYPES.AT_AOE;
            }
            return null;
        }
        #endregion
    }
    public class DamageTypeValueConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ItemDamageType dtValue = (ItemDamageType)value;
            if (dtValue == ItemDamageType.Physical) return "Physical";
            else if (dtValue == ItemDamageType.Holy) return "Holy";
            else if (dtValue == ItemDamageType.Fire) return "Fire";
            else if (dtValue == ItemDamageType.Nature) return "Nature";
            else if (dtValue == ItemDamageType.Frost) return "Frost";
            else if (dtValue == ItemDamageType.Shadow) return "Shadow";
            else if (dtValue == ItemDamageType.Arcane) return "Arcane";
            return ItemDamageType.Physical;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string atValue = (string)value;
            switch (atValue)
            {
                case "Physical": return ItemDamageType.Physical;
                case "Holy": return ItemDamageType.Holy;
                case "Fire": return ItemDamageType.Fire;
                case "Nature": return ItemDamageType.Nature;
                case "Frost": return ItemDamageType.Frost;
                case "Shadow": return ItemDamageType.Shadow;
                case "Arcane": return ItemDamageType.Arcane;
            }
            return null;
        }
        #endregion
    }
}
