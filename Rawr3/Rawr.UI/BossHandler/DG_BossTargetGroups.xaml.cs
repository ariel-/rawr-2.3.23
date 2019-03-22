using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rawr.UI
{
    public partial class DG_BossTargetGroups : ChildWindow
    {
        static DG_BossTargetGroups()
        {
        }

        public DG_BossTargetGroups()
        {
            InitializeComponent();
        }

        public DG_BossTargetGroups(List<TargetGroup> list)
        {
            InitializeComponent();
            TheList = list;
            SetListBox();
        }

        #region Variables
        protected List<TargetGroup> _TheList = null;
        public List<TargetGroup> TheList
        {
            get { return _TheList ?? (_TheList = new List<TargetGroup>()); }
            set { _TheList = value; }
        }
        #endregion

        private void SetListBox()
        {
            LB_TheList.Items.Clear();
            foreach (TargetGroup s in TheList)
            {
                string str = s.ToString();
                LB_TheList.Items.Add(str);
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
		}

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BT_Add_Click(object sender, RoutedEventArgs e)
        {
            TargetGroup s = new TargetGroup()
            {
                Frequency = (float)NUD_Freq.Value,
                Duration = (float)NUD_Dur.Value,
                Chance = ((float)NUD_Chance.Value) / 100f,
                NearBoss = (bool)CK_NearBoss.IsChecked,
                NumTargs = (float)NUD_NumTargs.Value,
            };
            TheList.Add(s);
            SetListBox();
        }

        private void BT_Delete_Click(object sender, RoutedEventArgs e)
        {
            int index = LB_TheList.SelectedIndex;
            if (index == -1) { return; }
            TheList.RemoveAt(index);
            SetListBox();
        }
	}
}

