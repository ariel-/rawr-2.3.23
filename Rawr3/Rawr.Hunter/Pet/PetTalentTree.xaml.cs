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

namespace Rawr.Hunter
{
    public partial class PetTalentTree : UserControl
    {
        public PetTalentTree()
        {
            // Required to initialize variables
            InitializeComponent();
            talentAttributes = new Dictionary<int, PetTalentDataAttribute>();
            prereqArrows = new Dictionary<int, Image>();
            belowRow = new Dictionary<int, int>();
        }

        public EventHandler TalentsChanged;

        public CharacterClass Class { get; set; }

        private PetTalentsBase talents;
        public PetTalentsBase Talents
        {
            get { return talents; }
            set
            {
                try
                {
                    if (value != null)
                    {
                        talents = value;
                        for (int r = 1; r <= 6; r++)
                        {
                            for (int c = 1; c <= 4; c++)
                            {
                                this[r, c].petTalentData = null;
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
                            PetTalentDataAttribute[] talentDatas = pi.GetCustomAttributes(typeof(PetTalentDataAttribute), true) as PetTalentDataAttribute[];
                            if (talentDatas.Length > 0)
                            {
                                PetTalentDataAttribute talentData = talentDatas[0];
                                if (talentData.Trees[Tree] != true/*Tree*/) continue;
                                this[talentData.Rows[Tree], talentData.Columns[Tree]].petTalentData = talentData;
                                talentAttributes[talentData.Index] = talentData;
                            }
                        }
                        for (int r = 1; r <= 6; r++)
                        {
                            for (int c = 1; c <= 4; c++)
                            {
                                this[r, c].Update();
                            }
                        }
                        UpdatePrereqs();
                    }
                } catch (Exception ex) {
                    new Rawr.Base.ErrorBox() { Message = ex.Message, StackTrace = ex.StackTrace, Title = "Error setting the talents in PetTalentTree", Function = "PetTalentTree.set_Talents(value)" }.Show();
                }
            }
        }

        public int Tree { get; set; }
        public string TreeName { get; private set; }

        private Dictionary<int,Image> prereqArrows;
        public void UpdatePrereqs()
        {
            foreach (KeyValuePair<int, PetTalentDataAttribute> kvp in talentAttributes)
            {
                if (kvp.Value.Prerequisites[Tree] >= 0)
                {
                    PetTalentDataAttribute prereq = GetAttribute(kvp.Value.Prerequisites[Tree]);
                    int row = kvp.Value.Rows[Tree] - prereq.Rows[Tree];
                    int col = kvp.Value.Columns[Tree] - prereq.Columns[Tree];

                    string suffix;
                    if (kvp.Value.MaxPoints == Talents.Data[kvp.Value.Index]) suffix = "-yellow.png";
                    else if (PointsBelowRow(kvp.Value.Rows[Tree]) >= (kvp.Value.Rows[Tree] - 1) * 5
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
                        Grid.SetColumn(image, kvp.Value.Columns[Tree]);
                        Grid.SetRow(image, kvp.Value.Rows[Tree]);
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
                            image.Source = Icons.NewBitmapImage(new Uri("/Rawr.Hunter;component/Resources/across-left" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(47, 15, 0, 0);
                        }
                        else if (col == 1)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("/Rawr.Hunter;component/Resources/across-right" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(-17, 15, 0, 0);
                        }
                    }
                    else if (row == 1)
                    {
                        if (col == -1)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("/Rawr.Hunter;component/Resources/down-left" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(13, -44, 0, 0);
                            Grid.SetRowSpan(image, 2);
                        }
                        else if (col == 0)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("/Rawr.Hunter;component/Resources/down-1" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(14, -17, 0, 0);
                        }
                        else if (col == 1)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("/Rawr.Hunter;component/Resources/down-right" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(-18, -44, 0, 0);
                        }
                    }
                    else if (row == 2)
                    {
                        if (col == -1) {
                            image.Source = Icons.NewBitmapImage(new Uri("/Rawr.Hunter;component/Resources/down-2-left" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(-18, 20, 0, 0);
                            Grid.SetRowSpan(image, 2);  
                        }
                        else if (col == 0)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("/Rawr.Hunter;component/Resources/down-2" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(13, -81, 0, 0);
                            Grid.SetRowSpan(image, 2);
                        }
                        else if (col == 1)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("/Rawr.Hunter;component/Resources/down-2-right" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(46, 20, 0, 0);
                            Grid.SetRowSpan(image, 2);
                        }
                    }
                    else if (row == 3)
                    {
                        if (col == -1)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("/Rawr.Hunter;component/Resources/down-3-left" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(-18, 84, 0, 0);
                            Grid.SetRowSpan(image, 2);
                        }
                        else if (col == 0)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("/Rawr.Hunter;component/Resources/down-3" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(13, -145, 0, 0);
                            Grid.SetRowSpan(image, 2);
                        }
                        else if (col == 1)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("/Rawr.Hunter;component/Resources/down-3-right" + suffix, UriKind.Relative));
                            image.Margin = new Thickness(46, 84, 0, 0);
                            Grid.SetRowSpan(image, 2);
                        }
                    }
                    else if (row == 4)
                    {
                        if (col == 0)
                        {
                            image.Source = Icons.NewBitmapImage(new Uri("/Rawr.Hunter;component/Resources/down-4" + suffix, UriKind.Relative));
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
            for (int r = 1; r <= 6; r++)
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
        public int Points() { return PointsBelowRow(7); }

        private Dictionary<int, PetTalentDataAttribute> talentAttributes;
        public PetTalentDataAttribute GetAttribute(int index)
        {
            if (talentAttributes.ContainsKey(index)) return talentAttributes[index];
            else return null;
        }

        private PetTalentItem this[int row, int col]
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
                return null;
            }
        }
    }
}