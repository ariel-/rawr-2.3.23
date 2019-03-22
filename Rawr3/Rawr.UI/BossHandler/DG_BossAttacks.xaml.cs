using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class DG_BossAttacks : ChildWindow
    {
        static DG_BossAttacks()
        {
        }

        public DG_BossAttacks()
        {
            InitializeComponent();
        }

        public DG_BossAttacks(List<Attack> list)
        {
            InitializeComponent();
            TheList = list;
            SetListBox();
        }

        #region Variables
        protected List<Attack> _TheList = null;
        public List<Attack> TheList
        {
            get { return _TheList ?? (_TheList = new List<Attack>()); }
            set { _TheList = value; }
        }
        #endregion

        private void SetListBox()
        {
            LB_TheList.Items.Clear();
            foreach (Attack s in TheList)
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
            try
            {
                Attack retVal = new Attack()
                {
                    // Basics
                    Name = "Dynamic",
                    DamageType = (ItemDamageType)CB_DmgType.SelectedIndex,
                    DamagePerHit = (float)NUD_DmgPerHit.Value,
                    DamageIsPerc = (bool)CK_DamageIsPerc.IsChecked,
                    MaxNumTargets = (float)NUD_MaxNumTargs.Value,
                    AttackSpeed = (float)NUD_AtkSpeed.Value,
                    AttackType = (ATTACK_TYPES)CB_AtkType.SelectedIndex,
                    UseParryHaste = (bool)CK_UseParryHaste.IsChecked,
                    Interruptable = (bool)CK_Interruptable.IsChecked,
                    IsTheDefaultMelee = (bool)CK_IsDefaultMelee.IsChecked,
                    IsDualWielding = (bool)CK_IsDualWielding.IsChecked,
                    // Player Avoidances
                    Missable = (bool)CK_Missable.IsChecked,
                    Dodgable = (bool)CK_Dodgable.IsChecked,
                    Parryable = (bool)CK_Parryable.IsChecked,
                    Blockable = (bool)CK_Blockable.IsChecked,
                    // Targeting Ignores
                    IgnoresMTank = (bool)CK_IgnoresMTank.IsChecked,
                    IgnoresOTank = (bool)CK_IgnoresOTank.IsChecked,
                    IgnoresTTank = (bool)CK_IgnoresTTank.IsChecked,
                    IgnoresHealers = (bool)CK_IgnoresHealers.IsChecked,
                    IgnoresMeleeDPS = (bool)CK_IgnoresMeleeDPS.IsChecked,
                    IgnoresRangedDPS = (bool)CK_IgnoresRangedDPS.IsChecked,
                };
                TheList.Add(retVal);
                SetListBox();
            } catch (Exception ex) {
                new ErrorWindow() { Message = "Didn't work\r\n" + ex.Message }.Show();
            }
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

