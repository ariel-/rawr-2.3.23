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
    public class CacheSettings
    {
        static CacheSettings()
        {
            _default = new CacheSettings();

            _default.RelativeItemImageCache = "Data/images";
            _default.RelativeTalentImageCache = "Data/talent_images";
        }

        private static CacheSettings _default;
        public static CacheSettings Default { get { return _default; } set { _default = value; } }

        public string RelativeItemImageCache { get; set; }
        public string RelativeTalentImageCache { get; set; }
    }
}
