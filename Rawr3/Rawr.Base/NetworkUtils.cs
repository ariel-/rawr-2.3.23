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
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Rawr
{
    public class NetworkUtils
    {

        public XDocument Result { get; private set; }
        public event EventHandler DocumentReady;

        public NetworkUtils() { }
        public NetworkUtils(EventHandler handler) : this()
        {
            DocumentReady += handler;
        }

        public void DownloadDocument(Uri uri)
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(uri);
        }

        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled == false)
            {
                using (StringReader sr = new StringReader(e.Result))
                {
                    Result = XDocument.Load(sr);
                }
            }
            else
            {
                Result = null;
            }
            if (DocumentReady != null) DocumentReady.Invoke(this, EventArgs.Empty);
        }

        public void GetCharacterSheetDocument(string characterName, string realm, CharacterRegion region)
        {
            DownloadDocument(new Uri(string.Format(Properties.NetworkSettings.Default.CharacterSheetURI,
                _domains[region], realm, characterName), UriKind.Relative));
        }

        public void GetTalentTreeDocument(string characterName, string realm, CharacterRegion region)
        {
            DownloadDocument(new Uri(string.Format(Properties.NetworkSettings.Default.CharacterTalentURI,
                _domains[region], realm, characterName), UriKind.Relative));
        }

        private static Dictionary<CharacterRegion, string> _domains;
        static NetworkUtils()
        {
            _domains = new Dictionary<CharacterRegion, string>();
            _domains.Add(CharacterRegion.US, "www");
            _domains.Add(CharacterRegion.EU, "eu");
            _domains.Add(CharacterRegion.KR, "kr");
            _domains.Add(CharacterRegion.TW, "tw");
            _domains.Add(CharacterRegion.CN, "cn");
        }

        internal void DownloadItemSearch(string itemName)
        {
            DownloadDocument(new Uri(string.Format(Properties.NetworkSettings.Default.ItemSearchURI, "www", itemName), UriKind.Relative));
        }

        internal void DownloadItemToolTipSheet(int id)
        {
            DownloadDocument(new Uri(string.Format(Properties.NetworkSettings.Default.ItemToolTipSheetURI, id), UriKind.Relative));
        }

        internal void DownloadItemInformation(int id)
        {
            DownloadDocument(new Uri(string.Format(Properties.NetworkSettings.Default.ItemInfoURI, id), UriKind.Relative));
        }
    }
}
