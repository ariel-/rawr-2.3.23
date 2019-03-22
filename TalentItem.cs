using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Rawr
{
    public class TalentItem
    {
        private TalentTree _talentTree;
        public TalentTree TalentTree
        {
            get { return _talentTree; }
            set
            {
                _talentTree = value;
                UpdateIcon();
            }
        }

        private string _talentName;
        public string TalentName
        {
            get { return _talentName; }
            set
            {
                _talentName = value;
                UpdateIcon();
            }
        }

        private int _locked = 0;

        private int _index = -1;
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private int _row = 1;
        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }
        private int _col = 1;
        public int Col
        {
            get { return _col; }
            set { _col = value; }
        }

        private string _iconName;
        private string[] _description = new string[] { "" };
        public string[] Description { get { return _description; } set { _description = value; } }

        private int _currentRank = 0;
        public int CurrentRank
        {
            get { return _currentRank; }
            set
            {
                if (_locked == 0 && value >= 0 && value <= _maxRank && _talentTree.TotalPoints() >= _row * 5 && 
                    (_prereq == null || _prereq._currentRank == _prereq._maxRank))
                {
                    if (_prereq != null) {
                        if (_currentRank > 0 && value == 0) _prereq._locked--;
                        else if (_currentRank == 0 && value > 0) _prereq._locked++;
                    }
                    int i = 1;
                    if (value < _currentRank)
                    {
                        int pts = 0;
                        for (i = 0; i <=_row; i++) pts += _talentTree.Points[i];
                        pts = pts - _currentRank + value;
                        for (i = _row + 1; i < 11; i++)
                        {
                            if (_talentTree.Points[i] > 0)
                            {
                                if (pts >= i * 5)
                                {
                                    pts += _talentTree.Points[i];
                                }
                                else
                                {
                                    i = -1;
                                    break;
                                }
                            }
                        }
                    }
                    if (i >= 0)
                    {
                        _talentTree.Points[_row] = _talentTree.Points[_row] - _currentRank + value;
                        _currentRank = value;
                        UpdateOverlay();
                        UpdateIcon();
                        OnCurrentRankChanged();
                    }
                }
            }
        }
        private int _maxRank = 5;
        public int MaxRank
        {
            get { return _maxRank; }
            set
            {
                _maxRank = value;
                UpdateOverlay();
                UpdateIcon();
            }
        }

        private TalentItem _prereq;
        public TalentItem Prereq
        {
            get { return _prereq; }
            set {
                if (_currentRank > 0 && _prereq != null) _prereq._locked--;
                _prereq = value;
                if (_currentRank > 0 && _prereq != null) _prereq._locked++;
            }
        }

        private Image _icon;
        private void UpdateIcon() { _icon = ItemIcons.GetTalentIcon(_talentTree.CharacterClass, _talentTree.TreeName, _talentName + (_currentRank == 0 ? "-off" : ""), (_currentRank == 0 ? "grey/" : "") + _iconName + ".jpg"); }

        private Image _overlay;
        private void UpdateOverlay()
        {
            if (_currentRank == 0) _overlay = Properties.Resources.icon_over_grey;
            else if (_currentRank == MaxRank) _overlay = Properties.Resources.icon_over_yellow;
            else _overlay = Properties.Resources.icon_over_green;
        }

        public TalentItem(TalentTree talentTree, string talentName, int row, int col, int index, string[] description, string icon, int currentRank, int maxRank)
            : this(talentTree, talentName, row, col, index, description, icon, currentRank, maxRank, null) { }

        public TalentItem(TalentTree talentTree, string talentName, int row, int col, int index, string[] description, string icon, int currentRank, int maxRank, TalentItem prereq)
        {
            _talentTree = talentTree;
            _talentName = talentName;
            _row = row;
            _col = col;
            _index = index;
            _description = description;
            _iconName = icon;
            _maxRank = maxRank;
            _currentRank = currentRank > maxRank ? maxRank : currentRank;
            Prereq = prereq;
            UpdateIcon();
            UpdateOverlay();
        }

        public void Draw(Graphics g)
        {
            int y = 8 + Row * 65;
            int x = 8 + Col * 65;

            String rank = string.Format("{0}/{1}", _currentRank, _maxRank);
            Font font = new Font("Verdana", 8);

            Brush brush;
            if (_currentRank == _maxRank) brush = Brushes.Yellow;
            else if (_talentTree.TotalPoints() >= _row * 5 && (_prereq == null || _prereq._currentRank == _prereq._maxRank)) brush = Brushes.Lime;
            else brush = Brushes.White;

            //It seems that the recent armory update changed the size of the talent icons from 43x43 down to 31x31!
            //This results in talent icons that are too small for the current layout :/
            //The simplest fix is to scale the (new) talent icons from 31x31 up to 43x43 via DrawImage().
            g.DrawImage(_icon, x, y, 44, 44);
            g.DrawImageUnscaled(_overlay, x, y);
            g.DrawString(rank, font, brush, x + 31, y + 39);

            if (_prereq != null)
            {
                Image arrowImage = null;
                int preRow = _row - _prereq.Row;
                int preCol = _col - _prereq.Col;
                int color;
                if (_currentRank == _maxRank) color = 2;
                else if (_talentTree.TotalPoints() >= _row * 5 && (_prereq == null || _prereq._currentRank == _prereq._maxRank)) color = 1;
                else color = 0;

                int offsetX = 0;
                int offsetY = 0;

                if (preCol == 0)
                {
                    if (preRow == 1)
                    {
                        if (color == 2) arrowImage = Properties.Resources.down_1_yellow;
                        else if (color == 1) arrowImage = Properties.Resources.down_1_green;
                        else arrowImage = Properties.Resources.down_1_grey;
                    }
                    else if (preRow == 2)
                    {
                        if (color == 2) arrowImage = Properties.Resources.down_2_yellow;
                        else if (color == 1) arrowImage = Properties.Resources.down_2_green;
                        else arrowImage = Properties.Resources.down_2_grey;
                    }
                    else if (preRow == 3)
                    {
                        if (color == 2) arrowImage = Properties.Resources.down_3_yellow;
                        else if (color == 1) arrowImage = Properties.Resources.down_3_green;
                        else arrowImage = Properties.Resources.down_3_grey;
                    }
                    else if (preRow == 4)
                    {
                        if (color == 2) arrowImage = Properties.Resources.down_4_yellow;
                        else if (color == 1) arrowImage = Properties.Resources.down_4_green;
                        else arrowImage = Properties.Resources.down_4_grey;
                    }
                    offsetX = 13;
                    offsetY = 47 - (preRow * 65);
                }
                else if (preCol == -1)
                {
                    if (preRow == 0)
                    {
                        if (color == 0) arrowImage = Properties.Resources.across_left_grey;
                        else if (color == 1) arrowImage = Properties.Resources.across_left_green;
                        else arrowImage = Properties.Resources.across_left_yellow;
                        offsetX = 43;
                        offsetY = 14;
                    }
                    else if (preRow == 1)
                    {
                        if (color == 0) arrowImage = Properties.Resources.down_left_grey;
                        else if (color == 1) arrowImage = Properties.Resources.down_left_green;
                        else arrowImage = Properties.Resources.down_left_yellow;
                        offsetX = 14;
                        offsetY = -45;
                    }
                    else if (preRow == 2)
                    {
                        if (color == 0) arrowImage = Properties.Resources.down_2_left_grey;
                        else if (color == 1) arrowImage = Properties.Resources.down_2_left_green;
                        else arrowImage = Properties.Resources.down_2_left_yellow;
                        offsetX = 14;
                        offsetY = -110;
                    }
                }
                else if (preCol == 1)
                {
                    if (preRow == 0)
                    {
                        if (color == 0) arrowImage = Properties.Resources.across_right_grey;
                        else if (color == 1) arrowImage = Properties.Resources.across_right_green;
                        else arrowImage = Properties.Resources.across_right_yellow;
                        offsetX = -20;
                        offsetY = 17;
                    }
                    else if (preRow == 1)
                    {
                        if (color == 0) arrowImage = Properties.Resources.down_right_grey;
                        else if (color == 1) arrowImage = Properties.Resources.down_right_green;
                        else arrowImage = Properties.Resources.down_right_yellow;
                        offsetX = -20;// 45;
                        offsetY = -45;// 20;
                    }
                    else if (preRow == 2)
                    {
                        if (color == 0) arrowImage = Properties.Resources.down_2_right_grey;
                        else if (color == 1) arrowImage = Properties.Resources.down_2_right_green;
                        else arrowImage = Properties.Resources.down_2_right_yellow;
                        offsetX = -20;
                        offsetY = -110;// 20;
                    }
                }
                if (arrowImage != null)
                {
                    g.DrawImageUnscaled(arrowImage, x + offsetX, y + offsetY);                 
                }
            }

        }

        public event EventHandler CurrentRankChanged;
        public void OnCurrentRankChanged()
        {
            if (CurrentRankChanged != null)
                CurrentRankChanged(this, EventArgs.Empty);
        }

        public string TooltipText()
        {
            string text = string.Format("{0}\r\nRank {1}/{2}\r\n\r\n", TalentName, CurrentRank, MaxRank);
            if (CurrentRank == 0) text += Description[0];
            else if (CurrentRank >= MaxRank) text += Description[MaxRank - 1];
            else text += string.Format("{0}\r\n\r\nNext Rank:\r\n{1}", Description[CurrentRank - 1], Description[CurrentRank]);

            return text;
        }

        public override string ToString()
        {
            return _talentName + "(" + _currentRank + "/" + _maxRank + ")";
        }

    }


}
