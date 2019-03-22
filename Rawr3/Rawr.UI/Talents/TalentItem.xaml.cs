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

namespace Rawr.UI
{
    public partial class TalentItem : UserControl
    {

        public TalentTree TalentTree { get { return (TalentTree)(((Grid)Parent).Parent); } }

        private TalentDataAttribute talentData;
        public TalentDataAttribute TalentData
        {
            get { return talentData; }
            set
            {
                talentData = value;
                current =  talentData == null ? 0 : (int)Math.Min(TalentTree.Talents.Data[talentData.Index], talentData.MaxPoints);
            }
        }

        public int current;
        public int Current
        {
            get { return current; }
            set
            {
                if (talentData != null && value >= 0 && value <= talentData.MaxPoints && CanPutPoints())
                {
                    current = value;
                    TalentTree.Talents.Data[TalentData.Index] = current;
                    TalentTree.RankChanged();
                }
            }
        }

        public bool IsMaxRank() { return current == talentData.MaxPoints; }

        public bool CanPutPoints()
        {
            return TalentTree.PointsBelowRow(talentData.Row) >= (talentData.Row - 1) * 5 &&
                (talentData.Prerequisite < 0
                || TalentTree.Talents.Data[talentData.Prerequisite] == TalentTree.GetAttribute(talentData.Prerequisite).MaxPoints);
        }

        public void Update()
        {
            if (talentData != null)
            {
                Brush b;
                if (Current == talentData.MaxPoints)
                {
                    b = new SolidColorBrush(Colors.Yellow);
                    OverlayImage.Source = Icons.NewBitmapImage(new Uri("../Images/icon-over-yellow.png", UriKind.Relative));
                }
                else
                {
                    if (CanPutPoints()) b = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                    else b = new SolidColorBrush(Colors.White);

                    if (Current > 0) OverlayImage.Source = Icons.NewBitmapImage(new Uri("../Images/icon-over-green.png", UriKind.Relative));
                    else OverlayImage.Source = Icons.NewBitmapImage(new Uri("../Images/icon-over-grey.png", UriKind.Relative));
                }

                RankLabel.Text = string.Format("{0}/{1}", Current, talentData.MaxPoints);
                RankLabel.Foreground = b;

                TalentImage.Source = Icons.TalentIcon(TalentTree.Class, TalentTree.TreeName, talentData.Name, Current > 0);

                ToolTipService.SetToolTip(this, GetTooltipString());
                this.Visibility = Visibility.Visible;
            }
            else
            {
                this.Visibility = Visibility.Collapsed;
            }
        }

        private string wrapText(string toWrap)
        {
            int wrapWidth = 63;
            if (toWrap.Length <= wrapWidth) { return toWrap; } // Don't bother wrapping

            string retVal = toWrap;
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
            string n = talentData.Name + "\r\n";
            if (Current == 0) {
                return string.Format(n + "Next Rank:\n{0}", wrapText(talentData.Description[0]));
            } else if (Current == talentData.MaxPoints) {
                return string.Format(n + "Max Points:\n{0}", wrapText(talentData.Description[talentData.MaxPoints - 1]));
            } else {
                return string.Format(n + "{0}\n\nNext Rank:\n{1}", wrapText(talentData.Description[Current - 1]), wrapText(talentData.Description[Current]));
            }
        }

        public TalentItem()
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
}