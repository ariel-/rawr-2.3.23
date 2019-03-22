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

namespace Rawr.HealPriest
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
    public class eRoleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type origType = value.GetType();
            if (targetType == typeof(string) && origType == typeof(eRole )) return eRoleToString((eRole)value);
            if (targetType == typeof(string) && origType == typeof(int   )) return eRoleToString((eRole)(int)value);
            if (targetType == typeof(int)    && origType == typeof(string)) return System.Convert.ToInt32(value);
            if (targetType == typeof(int)    && origType == typeof(eRole )) return (int)value;
            if (targetType == typeof(eRole)  && origType == typeof(string)) return StringToeRole((string)value);
            if (targetType == typeof(eRole)  && origType == typeof(int   )) return (eRole)value;
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type origType = value.GetType();
            if (targetType == typeof(string) && origType == typeof(eRole )) return eRoleToString((eRole)value);
            if (targetType == typeof(string) && origType == typeof(int   )) return eRoleToString((eRole)(int)value);
            if (targetType == typeof(int)    && origType == typeof(string)) return System.Convert.ToInt32(value);
            if (targetType == typeof(int)    && origType == typeof(eRole )) return (int)value;
            if (targetType == typeof(eRole)  && origType == typeof(string)) return StringToeRole((string)value);
            if (targetType == typeof(eRole)  && origType == typeof(int   )) return (eRole)value;
            return value;
        }
        private eRole StringToeRole(string e) {
            switch (e) {
                case "Auto Tank (Rawr picks from Talents)" : { return eRole.AUTO_Tank; }
                case "Auto Raid (Rawr picks from Talents)" : { return eRole.AUTO_Raid; }
                case "Greater Heal Spam (GH)" : { return eRole.Greater_Heal; }
                case "Flash Heal Spam (FH)" : { return eRole.Flash_Heal; }
                case "Circle/Prayer of Healing Spam (CoH/ProH)" : { return eRole.CoH_PoH; }
                case "Holy-Tank (Renew/ProM/GH)" : { return eRole.Holy_Tank; }
                case "Holy-Raid (ProM/CoH/FH/ProH)" : { return eRole.Holy_Raid; }
                case "Disc-Tank (Penance/PW:S/ProM/GH)" : { return eRole.Disc_Tank_GH; }
                case "Disc-Tank (Penance/PW:S/ProM/FH)" : { return eRole.Disc_Tank_FH; }
                case "Disc-Raid (PW:S/Penance/Flash)" : { return eRole.Disc_Raid; }
                case "Custom Role (You pick!)" : { return eRole.CUSTOM; }
                case "Holy-Raid-Renew (ProM/CoH/Renew/FH)": { return eRole.Holy_Raid_Renew; }
            }
            return eRole.CUSTOM;
        }
        private string eRoleToString(eRole e) {
            switch (e) {
                case eRole.AUTO_Tank: { return "Auto Tank (Rawr picks from Talents)"; }
                case eRole.AUTO_Raid: { return "Auto Raid (Rawr picks from Talents)"; }
                case eRole.Greater_Heal : { return "Greater Heal Spam (GH)"; }
                case eRole.Flash_Heal: { return "Flash Heal Spam (FH)"; }
                case eRole.CoH_PoH : { return "Circle/Prayer of Healing Spam (CoH/ProH)"; }
                case eRole.Holy_Tank : { return "Holy-Tank (Renew/ProM/GH)"; }
                case eRole.Holy_Raid : { return "Holy-Raid (ProM/CoH/FH/ProH)"; }
                case eRole.Disc_Tank_GH : { return "Disc-Tank (Penance/PW:S/ProM/GH)"; }
                case eRole.Disc_Tank_FH : { return "Disc-Tank (Penance/PW:S/ProM/FH)"; }
                case eRole.Disc_Raid : { return "Disc-Raid (PW:S/Penance/Flash)"; }
                case eRole.CUSTOM : { return "Custom Role (You pick!)"; }
                case eRole.Holy_Raid_Renew: { return "Holy-Raid-Renew (ProM/CoH/Renew/FH)"; }
            }
            return "Custom Role (You pick!)";
        }
    }
}
