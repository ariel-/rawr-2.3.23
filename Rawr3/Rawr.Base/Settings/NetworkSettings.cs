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
    public class NetworkSettings
    {

        public static bool UseAspx { get; set; }

        static NetworkSettings()
        {
            UseAspx = false;

            _default = new NetworkSettings();

            _default.ProxyServer = "";
            _default.ProxyPort = 0;    
            _default.UseDefaultProxySettings = true;
            _default.ProxyUserName = "";
            _default.ProxyPassword = "";
            _default.WoWItemIconURI = "http://www.wowarmory.com/wow-icons/_images/64x64/";
            _default.MaxHttpRequests = 5;
            _default.ProxyType = "None";
            _default.UserAgent_IE7 = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; bgft) ";
            _default.UserAgent_IE6 = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1) ";
            _default.UserAgent_FireFox2 = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.8.1.4) Gecko/20070515 Firefox/2.0.0.4";
            _default.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.8.1.4) Gecko/20070515 Firefox/2.0.0.4";
            _default.ClassTalentURI = "http://www.worldofwarcraft.com/shared/global/talents/{0}/data.js";
            _default.CharacterTalentURI = "Armory.php?{0}*character-talents.xml*r={1}&cn={2}";//"http://{0}.wowarmory.com/character-talents.xml?r={1}&cn={2}";
            _default.CharacterSheetURI = "Armory.php?{0}*character-sheet.xml*r={1}&cn={2}";//"http://{0}.wowarmory.com/character-sheet.xml?r={1}&cn={2}";
            _default.ItemToolTipSheetURI = "Armory.php?item-tooltip.xml*i={0}";//"http://www.wowarmory.com/item-tooltip.xml?i={0}";
            _default.ItemUpgradeURI = "http://{0}.wowarmory.com/search.xml?searchType=items&pr={1}&pn={2}&pi={3}";
            _default.TalentIconURI = "http://www.worldofwarcraft.com/shared/global/talents/{0}/images/{1}/{2}.jpg";
            _default.ItemInfoURI = "Armory.php?item-info.xml*i={0}";//"http://www.wowarmory.com/item-info.xml?i={0}";
            _default.DownloadItemInfo = false;
            _default.ProxyRequiresAuthentication = false;
            _default.UseDefaultAuthenticationForProxy = false;
            _default.ProxyDomain = "";
            _default.ItemWowheadURI = "http://{0}.wowhead.com/?item={1}&xml";
            _default.ItemSearchURI = "Armory.php?{0}*search.xml*searchQuery={1}&searchType=items";//"http://{0}.wowarmory.com/search.xml?searchQuery={1}&searchType=items";
            _default.ItemWowheadUpgradeURI = "http://{0}.wowhead.com/?items&filter={1}";
            _default.ArmoryTalentIconURI = "http://www.wowarmory.com/wow-icons/_images/_talents43x43/{0}";

        }

        private static NetworkSettings _default;
        public static NetworkSettings Default
        {
            get { return _default; }
            set { _default = value; }
        }

        public string ProxyServer { get; set; }
        public int ProxyPort { get; set; }
        public bool UseDefaultProxySettings { get; set; }
        public string ProxyUserName { get; set; }
        public string ProxyPassword { get; set; }
        public string WoWItemIconURI { get; set; }
        public int MaxHttpRequests { get; set; }
        public string ProxyType { get; set; }
        public string UserAgent_IE7 { get; set; }
        public string UserAgent_IE6 { get; set; }
        public string UserAgent_FireFox2 { get; set; }
        public string UserAgent { get; set; }
        public string ClassTalentURI { get; set; }

        private string characterTalentURI;
        public string CharacterTalentURI
        {
            get
            {
                if (UseAspx) return characterTalentURI.Replace("php", "aspx");
                else return characterTalentURI;
            }
            set { characterTalentURI = value; }
        }  

        private string characterSheetURI;
        public string CharacterSheetURI
        {
            get
            {
                if (UseAspx) return characterSheetURI.Replace("php", "aspx");
                else return characterSheetURI;
            }
            set { characterSheetURI = value; }
        }

        private string itemToolTipSheetURI;
        public string ItemToolTipSheetURI
        {
            get
            {
                if (UseAspx) return itemToolTipSheetURI.Replace("php", "aspx");
                else return itemToolTipSheetURI;
            }
            set { itemToolTipSheetURI = value; }
        }   

        public string ItemUpgradeURI { get; set; }
        public string TalentIconURI { get; set; }

        private string itemInfoURI;
        public string ItemInfoURI
        {
            get
            {
                if (UseAspx) return itemInfoURI.Replace("php", "aspx");
                else return itemInfoURI;
            }
            set { itemInfoURI = value; }
        }  

        public bool DownloadItemInfo { get; set; } 
        public bool ProxyRequiresAuthentication { get; set; } 
        public bool UseDefaultAuthenticationForProxy { get; set; }
        public string ProxyDomain { get; set; }
        public string ItemWowheadURI { get; set; }

        private string itemSearchURI;
        public string ItemSearchURI
        {
            get
            {
                if (UseAspx) return itemSearchURI.Replace("php", "aspx");
                else return itemSearchURI;
            }
            set { itemSearchURI = value; }
        }
        public string ItemWowheadUpgradeURI { get; set; }
        public string ArmoryTalentIconURI { get; set; }

    }
}
