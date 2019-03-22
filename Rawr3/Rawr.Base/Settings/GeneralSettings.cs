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

namespace Rawr.Properties
{
	public class GeneralSettings
	{
		static GeneralSettings()
		{
			_default = new GeneralSettings();

			_default.CountGemmingsShown = 3;
			_default.UseMultithreading = true;
			_default.Locale = "en";
			_default.DisplayBuffSource = false;
			_default.DisplayGemNames = false;
            //_default.DisplayExtraItemInfo = false;
			_default.ProcEffectMode = 0;
			_default.CombinationEffectMode = 0;
            _default.ItemNameWidthSetting = 0;

            _default.WelcomeScreenSeen = false;

            _default.EnforceGemRequirements = true;
            _default.EnforceGemRequirements_Meta = true;
            _default.EnforceGemRequirements_JC = true;
            _default.EnforceGemRequirements_Unique = true;
		}

		private static GeneralSettings _default;
		public static GeneralSettings Default { get { return _default; } set { _default = value; } }

		public int CountGemmingsShown { get; set; }
		public bool UseMultithreading { get; set; }
		public string Locale { get; set; }
		public bool DisplayBuffSource { get; set; }
		public bool DisplayGemNames { get; set; }
		public int ProcEffectMode { get; set; }
		//public bool DisplayExtraItemInfo { get; set; }
        public bool HideProfEnchants { get; set; }
        public bool EnforceGemRequirements { get; set; }
        public bool EnforceGemRequirements_Meta { get; set; }
        public bool EnforceGemRequirements_JC { get; set; }
        public bool EnforceGemRequirements_Unique { get; set; }
		public int CombinationEffectMode { get; set; }
        public int ItemNameWidthSetting { get; set; } // 0 = Normal (142 px), 1 = Wide (162 px), 2 = Widest (182 px)
        public bool WelcomeScreenSeen { get; set; }
	}
}
