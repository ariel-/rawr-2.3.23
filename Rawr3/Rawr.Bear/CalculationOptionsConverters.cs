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

namespace Rawr.Bear
{
    public class ThreatValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            float threatValue = (float)value;
            if (threatValue == 0f) return "None";
            if (threatValue == 10f) return "MT";
            if (threatValue == 50f) return "OT";
            if (threatValue == 100f) return "Crazy About Threat";
            else return "Custom...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string threatValue = (string)value;
            switch (threatValue)
            {
                case "None": return 0f;
                case "MT": return 10f;
                case "OT": return 50f;
                case "Crazy About Threat": return 100f;
            }
            return null;
        }

        #endregion
    }

    public class SurvivalSoftCapConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int survivalSoftCap = (int)value;
            if (survivalSoftCap == 90000) return "Normal Dungeons";
            if (survivalSoftCap == 110000) return "Heroic Dungeons";
            if (survivalSoftCap == 120000) return "T7 Raids (10)";
            if (survivalSoftCap == 130000) return "T7 Raids (25)";
            if (survivalSoftCap == 170000) return "T8 Raids (10)";
            if (survivalSoftCap == 195000) return "T8 Raids (10, Hard)";
            if (survivalSoftCap == 185000) return "T8 Raids (25)";
            if (survivalSoftCap == 215000) return "T8 Raids (25, Hard)";
            if (survivalSoftCap == 180000) return "T9 Raids (10)";
            if (survivalSoftCap == 210000) return "T9 Raids (10, Heroic)";
            if (survivalSoftCap == 190000) return "T9 Raids (25)";
			if (survivalSoftCap == 225000) return "T9 Raids (25, Heroic)";
			if (survivalSoftCap == 250000) return "T10 Raids (10)";
			if (survivalSoftCap == 300000) return "T10 Raids (10, Heroic)";
			if (survivalSoftCap == 300000) return "T10 Raids (25)";
			if (survivalSoftCap == 350000) return "T10 Raids (25, Heroic)";
            else return "Custom...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string survivalSoftCap = (string)value;
            switch (survivalSoftCap)
            {
                case "Normal Dungeons": return 90000;
                case "Heroic Dungeons": return 110000;
                case "T7 Raids (10)": return 120000;
                case "T7 Raids (25)": return 130000;
                case "T8 Raids (10)": return 170000;
                case "T8 Raids (10, Hard)": return 195000;
                case "T8 Raids (25)": return 185000;
				case "T8 Raids (25, Hard)": return 215000;
				case "T9 Raids (10)": return 180000;
				case "T9 Raids (10, Heroic)": return 210000;
				case "T9 Raids (25)": return 190000;
				case "T9 Raids (25, Heroic)": return 225000;
				case "T10 Raids (10)": return 250000;
				case "T10 Raids (10, Heroic)": return 300000;
				case "T10 Raids (25)": return 300000;
				case "T10 Raids (25, Heroic)": return 350000;
            }
            return null;
        }

        #endregion
    }

    public class TargetDamageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int survivalSoftCap = (int)value;
            if (survivalSoftCap == 30000) return "Normal Dungeons";
            if (survivalSoftCap == 37000) return "Heroic Dungeons";
            if (survivalSoftCap == 40000) return "T7 Raids (10)";
            if (survivalSoftCap == 47000) return "T7 Raids (25)";
            if (survivalSoftCap == 55000) return "T8 Raids (10)";
            if (survivalSoftCap == 75000) return "T8 Raids (10, Hard)";
            if (survivalSoftCap == 71000) return "T8 Raids (25)";
            if (survivalSoftCap == 90000) return "T8 Raids (25, Hard)";
            if (survivalSoftCap == 70000) return "T9 Raids (10)";
            if (survivalSoftCap == 85000) return "T9 Raids (10, Heroic)";
            if (survivalSoftCap == 80000) return "T9 Raids (25)";
			if (survivalSoftCap == 95000) return "T9 Raids (25, Heroic)";
			if (survivalSoftCap == 90000) return "T9 Raids (10)";
			if (survivalSoftCap == 105000) return "T9 Raids (10, Heroic)";
			if (survivalSoftCap == 100000) return "T9 Raids (25)";
			if (survivalSoftCap == 120000) return "T9 Raids (25, Heroic)";
            else return "Custom...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string survivalSoftCap = (string)value;
            switch (survivalSoftCap)
            {
                case "Normal Dungeons": return 30000;
                case "Heroic Dungeons": return 37000;
                case "T7 Raids (10)": return 40000;
                case "T7 Raids (25)": return 47000;
                case "T8 Raids (10)": return 55000;
                case "T8 Raids (10, Hard)": return 75000;
                case "T8 Raids (25)": return 71000;
				case "T8 Raids (25, Hard)": return 90000;
				case "T9 Raids (10": return 70000;
				case "T9 Raids (10, Heroic)": return 85000;
				case "T9 Raids (25)": return 80000;
				case "T9 Raids (25, Heroic)": return 95000;
				case "T10 Raids (10": return 90000;
				case "T10 Raids (10, Heroic)": return 105000;
				case "T10 Raids (25)": return 100000;
				case "T10 Raids (25, Heroic)": return 120000;
            }
            return null;
        }

        #endregion
    }
}
