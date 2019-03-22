using System;
using System.Reflection;
using System.Collections.Generic;
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
	public partial class TalentTree : UserControl
	{
		public TalentTree()
		{
			// Required to initialize variables
			InitializeComponent();
            talentAttributes = new Dictionary<int, TalentDataAttribute>();
            prereqArrows = new Dictionary<int, Image>();
            belowRow = new Dictionary<int, int>();
		}

        public EventHandler TalentsChanged;

        public CharacterClass Class { get; set; }

        private TalentsBase talents;
        public TalentsBase Talents
        {
            get { return talents; }
            set
            {
                if (value != null)
                {
                    talents = value;
                    for (int r = 1; r <= 11; r++)
                    {
                        for (int c = 1; c <= 4; c++)
                        {
                            this[r, c].TalentData = null;
                        }
                    }
                    talentAttributes.Clear();
                    belowRow.Clear();
                    if (prereqArrows != null)
                    {
                        foreach (KeyValuePair<int, Image> kvp in prereqArrows)
                        {
                            GridPanel.Children.Remove(kvp.Value);
                        }
                    }
                    prereqArrows.Clear();
                    Class = talents.GetClass();
                    TreeName = ((string[])Talents.GetType().GetField("TreeNames").GetValue(Talents))[Tree];
                    BackgroundImage.Source = Icons.TreeBackground(Class, TreeName);
                    foreach (PropertyInfo pi in Talents.GetType().GetProperties())
                    {
                        TalentDataAttribute[] talentDatas = pi.GetCustomAttributes(typeof(TalentDataAttribute), true) as TalentDataAttribute[];
                        if (talentDatas.Length > 0)
                        {
                            TalentDataAttribute talentData = talentDatas[0];
                            if (talentData.Tree != Tree) continue;
                            this[talentData.Row, talentData.Column].TalentData = talentData;
                            talentAttributes[talentData.Index] = talentData;
                        }
                    }
                    for (int r = 1; r <= 11; r++)
                    {
                        for (int c = 1; c <= 4; c++)
                        {
                            this[r, c].Update();
                        }
                    }
                    UpdatePrereqs();
                }
            }
        }

        public int Tree { get; set; }
        public string TreeName { get; private set; }

        private Dictionary<int,Image> prereqArrows;
        public void UpdatePrereqs()
        {
            foreach (KeyValuePair<int, TalentDataAttribute> kvp in talentAttributes)
            {
                if (kvp.Value.Prerequisite >= 0)
                {
                    TalentDataAttribute prereq = GetAttribute(kvp.Value.Prerequisite);
                    int row = kvp.Value.Row - prereq.Row;
                    int col = kvp.Value.Column - prereq.Column;

                    string suffix;
                    if (kvp.Value.MaxPoints == Talents.Data[kvp.Value.Index]) suffix = "-yellow.png";
                    else if (PointsBelowRow(kvp.Value.Row) >= (kvp.Value.Row - 1) * 5
                            && Talents.Data[prereq.Index] == prereq.MaxPoints) suffix = "-green.png";
                    else suffix = "-grey.png";

                    Image image;
                    if (prereqArrows.ContainsKey(kvp.Value.Index))
                    {
                        image = prereqArrows[kvp.Value.Index];
                    }
                    else
                    {
                        image = new Image();
                        Grid.SetColumn(image, kvp.Value.Column);
                        Grid.SetRow(image, kvp.Value.Row);
                        image.Stretch = Stretch.None;
                        image.HorizontalAlignment = HorizontalAlignment.Left;
                        image.VerticalAlignment = VerticalAlignment.Top;
                        GridPanel.Children.Add(image);
                        prereqArrows[kvp.Value.Index] = image;
                    }

                    if (row == 0)
                    {
                        if (col == -1) 
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("../Images/across-left" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(47, 15, 0, 0);
                        }
                        else if (col == 1)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("../Images/across-right" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(-17, 15, 0, 0);
                        }
                    }
                    else if (row == 1)
                    {
                        if (col == -1)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("../Images/down-left" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(13, -44, 0, 0);
                            Grid.SetRowSpan(image, 2);
                        }
                        else if (col == 0)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("../Images/down-1" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(14, -17, 0, 0);
                        }
                        else if (col == 1)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("../Images/down-right" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(-18, -44, 0, 0);
                        }
                    }
                    else if (row == 2)
                    {
                        if (col == -1) {
                            image.Source = Icons.NewBitmapImage(new Uri("../Images/down-2-left" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(-18, 20, 0, 0);
                            Grid.SetRowSpan(image, 2);  
                        }
                        else if (col == 0)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("../Images/down-2" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(13, -81, 0, 0);
                            Grid.SetRowSpan(image, 2);
                        }
                        else if (col == 1)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("../Images/down-2-right" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(46, 20, 0, 0);
                            Grid.SetRowSpan(image, 2);
                        }
                    }
                    else if (row == 3)
                    {
                        if (col == -1)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("../Images/down-3-left" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(-18, 84, 0, 0);
                            Grid.SetRowSpan(image, 2);
                        }
                        else if (col == 0)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("../Images/down-3" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(13, -145, 0, 0);
                            Grid.SetRowSpan(image, 2);
                        }
                        else if (col == 1)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("../Images/down-3-right" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(46, 84, 0, 0);
                            Grid.SetRowSpan(image, 2);
                        }
                    }
                    else if (row == 4)
                    {
                        if (col == 0)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("../Images/down-4" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(13, -209, 0, 0);
                            Grid.SetRowSpan(image, 2);
                        }
                    }
                }
            }
        }

        public void RankChanged()
        {
            belowRow.Clear();
            for (int r = 1; r <= 11; r++)
            {
                for (int c = 1; c <= 4; c++)
                {
                    this[r, c].Update();
                }
            }
            UpdatePrereqs();
            if (TalentsChanged != null) TalentsChanged.Invoke(this, EventArgs.Empty);
        }

        private Dictionary<int, int> belowRow;
        public int PointsBelowRow(int row)
        {
            if (belowRow.ContainsKey(row)) return belowRow[row];
            else
            {
                int pts = 0;
                for (int r = 1; r < row; r++)
                {
                    for (int c = 1; c <= 4; c++)
                    {
                        pts += this[r, c].Current;
                    }
                }
                belowRow[row] = pts;
                return pts;
            }
        }
        public int Points() { return PointsBelowRow(12); }

        private Dictionary<int, TalentDataAttribute> talentAttributes;
        public TalentDataAttribute GetAttribute(int index)
        {
            if (talentAttributes.ContainsKey(index)) return talentAttributes[index];
            else return null;
        }

        private TalentItem this[int row, int col]
        {
            get
            {
                if (row == 1)
                {
                    if (col == 1) return Talent_1_1;
                    if (col == 2) return Talent_1_2;
                    if (col == 3) return Talent_1_3;
                    if (col == 4) return Talent_1_4;
                }
                else if (row == 2)
                {
                    if (col == 1) return Talent_2_1;
                    if (col == 2) return Talent_2_2;
                    if (col == 3) return Talent_2_3;
                    if (col == 4) return Talent_2_4;
                }
                else if (row == 3)
                {
                    if (col == 1) return Talent_3_1;
                    if (col == 2) return Talent_3_2;
                    if (col == 3) return Talent_3_3;
                    if (col == 4) return Talent_3_4;
                }
                else if (row == 4)
                {
                    if (col == 1) return Talent_4_1;
                    if (col == 2) return Talent_4_2;
                    if (col == 3) return Talent_4_3;
                    if (col == 4) return Talent_4_4;
                }
                else if (row == 5)
                {
                    if (col == 1) return Talent_5_1;
                    if (col == 2) return Talent_5_2;
                    if (col == 3) return Talent_5_3;
                    if (col == 4) return Talent_5_4;
                }
                else if (row == 6)
                {
                    if (col == 1) return Talent_6_1;
                    if (col == 2) return Talent_6_2;
                    if (col == 3) return Talent_6_3;
                    if (col == 4) return Talent_6_4;
                }
                else if (row == 7)
                {
                    if (col == 1) return Talent_7_1;
                    if (col == 2) return Talent_7_2;
                    if (col == 3) return Talent_7_3;
                    if (col == 4) return Talent_7_4;
                }
                else if (row == 8)
                {
                    if (col == 1) return Talent_8_1;
                    if (col == 2) return Talent_8_2;
                    if (col == 3) return Talent_8_3;
                    if (col == 4) return Talent_8_4;
                }
                else if (row == 9)
                {
                    if (col == 1) return Talent_9_1;
                    if (col == 2) return Talent_9_2;
                    if (col == 3) return Talent_9_3;
                    if (col == 4) return Talent_9_4;
                }
                else if (row == 10)
                {
                    if (col == 1) return Talent_10_1;
                    if (col == 2) return Talent_10_2;
                    if (col == 3) return Talent_10_3;
                    if (col == 4) return Talent_10_4;
                }
                else if (row == 11)
                {
                    if (col == 1) return Talent_11_1;
                    if (col == 2) return Talent_11_2;
                    if (col == 3) return Talent_11_3;
                    if (col == 4) return Talent_11_4;
                }
                return null;
            }
        }

	}
}