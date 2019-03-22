using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Media.Imaging;
//using log4net;

namespace Rawr
{
    public static class Icons
    {

        public static BitmapImage ItemIcon(string name)
        {
            if (name == null) return null;
            else return NewBitmapImage(new Uri(string.Format("http://www.wowarmory.com/wow-icons/_images/64x64/{0}.jpg", name.ToLower())));
        }

        public static BitmapImage TreeBackground(CharacterClass charClass, string talentTree)
        {
            return TalentIcon(charClass, talentTree, "background", true);
        }

        public static BitmapImage TalentIcon(CharacterClass charClass, string talentTree, string talentName, bool on)
        {
            talentTree = talentTree.Replace(" ", "");
            talentName = talentName.Replace(" ", "");
            talentName = talentName.Replace(":", "");
            Uri uri = new Uri(string.Format("http://www.worldofwarcraft.com/shared/global/talents/{0}/images/{1}/{2}{3}.jpg",
                charClass.ToString().ToLower(), talentTree.ToLower(), talentName.ToLower(), on ? "" : "-off"), UriKind.Absolute);
            return NewBitmapImage(uri);
        }

        public static BitmapImage NewBitmapImage(Uri uri)
        {
            // this thing is throwing InvalidDeploymentException in WPF when you're catching handled exceptions
            // there's nothing wrong actually, it works when run normally
#if SILVERLIGHT
            return new BitmapImage(uri);
#else
            BitmapImage ret = new BitmapImage();
            ret.BeginInit();
            ret.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            ret.UriSource = uri;
            ret.EndInit();
            return ret;
#endif
        }
    }
}