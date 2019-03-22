using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace Rawr.Hunter
{
    public partial class PetTalentItem : UserControl
    {

        public PetTalentTree PetTalentTree { get { return (PetTalentTree)(((Grid)Parent).Parent); } }

        private PetTalentDataAttribute pettalentData;
        public PetTalentDataAttribute petTalentData
        {
            get { return pettalentData; }
            set
            {
                pettalentData = value;
                current = pettalentData == null ? 0 : (int)Math.Min(PetTalentTree.Talents.Data[pettalentData.Index], pettalentData.MaxPoints);
            }
        }

        public int current;
        public int Current
        {
            get { return current; }
            set
            {
                if (pettalentData != null && value >= 0 && value <= pettalentData.MaxPoints && CanPutPoints())
                {
                    current = value;
                    PetTalentTree.Talents.Data[pettalentData.Index] = current;
                    PetTalentTree.RankChanged();
                }
            }
        }

        public bool IsMaxRank() { return current == pettalentData.MaxPoints; }

        public bool CanPutPoints()
        {
            return PetTalentTree.PointsBelowRow(pettalentData.Rows[PetTalentTree.Tree]) >= (pettalentData.Rows[PetTalentTree.Tree] - 1) * 3 &&
                (pettalentData.Prerequisites[PetTalentTree.Tree] < 0
                || PetTalentTree.Talents.Data[pettalentData.Prerequisites[PetTalentTree.Tree]] == PetTalentTree.GetAttribute(pettalentData.Prerequisites[PetTalentTree.Tree]).MaxPoints);
        }

        public void Update()
        {
            if (pettalentData != null)
            {
                Brush b;
                if (Current == pettalentData.MaxPoints)
                {
                    b = new SolidColorBrush(Colors.Yellow);
                    OverlayImage.Source = Icons.NewBitmapImage(new Uri("/Rawr.Hunter;component/Resources/icon-over-yellow.png", UriKind.Relative));
                }
                else
                {
                    if (CanPutPoints()) b = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                    else b = new SolidColorBrush(Colors.White);

                    if (Current > 0) OverlayImage.Source = Icons.NewBitmapImage(new Uri("/Rawr.Hunter;component/Resources/icon-over-green.png", UriKind.Relative));
                    else OverlayImage.Source = Icons.NewBitmapImage(new Uri("/Rawr.Hunter;component/Resources/icon-over-grey.png", UriKind.Relative));
                }

                RankLabel.Text = string.Format("{0}/{1}", Current, pettalentData.MaxPoints);
                RankLabel.Foreground = b;

                TalentImage.Source = Icons.TalentIcon(PetTalentTree.Class, PetTalentTree.TreeName, pettalentData.Icon, Current > 0);

                ToolTipService.SetToolTip(this, GetTooltipString());
                this.Visibility = Visibility.Visible;
            }
            else
            {
                this.Visibility = Visibility.Collapsed;
            }
        }

        public string WrapText(string source) {
            int wrapWidth = 63;
            if (source.Length <= wrapWidth) { return source; } // Don't bother wrapping

            string retVal = source;
            bool eos = false;
            bool foundspace = false;
            int i = wrapWidth;

            while (!eos)
            {
                while (!foundspace && i >= 0)
                {
                    if (retVal[i] == ' ') { foundspace = true; break; }
                    i--; // didn't find a space so backtrack a char
                }
                if (foundspace)
                {
                    retVal = retVal.Insert(i + 1, "\r\n"); // +1 because we want it after the space
                    i++; foundspace = false;
                }
                // Continue to next part of string unless we're at or close to the end
                if (i + wrapWidth >= retVal.Length - 1) { eos = true; } else { i += wrapWidth; }
            }

            return retVal;
        }

        public string GetTooltipString()
        {
            int cur = Current, max = pettalentData == null ? -1 : pettalentData.MaxPoints;
            try {
                string n = pettalentData.Name + "\r\n";
                if (Current == 0) {
                    return string.Format(n + "Next Rank:\n{0}", WrapText(pettalentData.Description[0]));
                } else if (Current == pettalentData.MaxPoints) {
                    return string.Format(n + "Max Points:\n{0}", WrapText(pettalentData.Description[pettalentData.MaxPoints - 1]));
                } else {
                    return string.Format(n + "{0}\n\nNext Rank:\n{1}", WrapText(pettalentData.Description[Current - 1]), WrapText(pettalentData.Description[Current]));
                }
            } catch (Exception ex) {
                new Rawr.Base.ErrorBox() {
                    Message = ex.Message, StackTrace = ex.StackTrace,
                    Title = "Error setting the talents in PetTalentTree",
                    Function = "PetTalentTree.set_Talents(value)",
                    Info = string.Format("Talent: {0} [{1}/{2}]", pettalentData == null ? "bad data" : pettalentData.Name, cur, max)
                }.Show();
            }
            return "";
        }

        public PetTalentItem()
        {
            // Required to initialize variables
            InitializeComponent();
        }

        void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            //Handling this event makes it not throw the exception up to the user. This occurs when a talent image cannot be found, which is fine to ignore.
        }

        private void TalentClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0) Current--;
            else Current++;
        }
        
    }

    public class Icons
    {
        public static BitmapImage TreeBackground(CharacterClass charClass, string talentTree)
        {
            return TalentIcon(charClass, talentTree, "background", true);
        }

        public static BitmapImage TalentIcon(CharacterClass charClass, string talentTree, string talentName, bool on)
        {
            talentTree = talentTree.Replace(" ", "");
            talentName = talentName.Replace(" ", "");
            talentName = talentName.Replace(":", "");
            Uri uri;

            if (talentName.ToLower() == "background") {
                int index = 0;
                if (talentTree == "Cunning") { index = 3; } else if (talentTree == "Ferocity") { index = 1; } else if (talentTree == "Tenacity") { index = 2; }
                uri = new Uri(string.Format("http://static.wowhead.com/images/wow/hunterpettalents/live/bg_{0}.jpg", index), UriKind.Absolute);
            } else {
                uri = new Uri(string.Format("http://www.wowarmory.com/wow-icons/_images/_talents31x31/{1}{0}.jpg", talentName.ToLower(), on ? "" : "grey/"), UriKind.Absolute);
            }
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