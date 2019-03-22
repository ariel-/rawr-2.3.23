using System;
using System.Collections.Generic;
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
    public class RecentSettings
    {
        static RecentSettings()
		{
            _default = new RecentSettings();

			_default.RecentFiles = "";
            _default.RecentModel = "Bear";

            _default.RecentChars = new List<string>() { };
            _default.RecentServers = new List<string>() { };
            _default.RecentRegion = "US";

            _default.RecentCharProfiler = "C:\\Program Files\\World of Warcraft\\WTF\\Account\\";
		}

        private static RecentSettings _default;
        public static RecentSettings Default { get { return _default; } set { _default = value; } }

        /// <summary>
        /// For Main Form, so it remembers where to load files from
        /// <para>NOTE: This appears to be getting set with some other setting, this is not in use</para>
        /// </summary>
        public string RecentFiles { get; set; }
        /// <summary>For Main Form, so it starts on the last model used</summary>
        public string RecentModel { get; set; }
        /// <summary>For Load from Armory dialog, so we can remember last several characters loaded</summary>
        public List<string> RecentChars { get; set; }
        /// <summary>For Load from Armory dialog, so we can remember last several characters loaded</summary>
        public List<string> RecentServers { get; set; }
        /// <summary>For Load from Armory dialog, so we can remember last several characters loaded</summary>
        public string RecentRegion { get; set; }
        /// <summary>For Load from Character Profiler dialog, so we can remember where the last toon was pulled from the addon</summary>
        public string RecentCharProfiler { get; set; }
    }
}
