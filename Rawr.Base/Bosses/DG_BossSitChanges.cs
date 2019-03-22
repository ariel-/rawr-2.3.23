using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr.Bosses
{
    public partial class DG_BossSitChanges : Form
    {
        #region Constructors
        public DG_BossSitChanges() { InitializeComponent(); }
        public DG_BossSitChanges(List<Impedance> list, Flags f)
        {
            InitializeComponent();
            Flag = f;
            TheList = list;
            string s = "";
            switch (f) {
                case Flags.Stun: { s = "Stuns"; break; }
                case Flags.Move: { s = "Moves"; break; }
                case Flags.Fear: { s = "Fears"; break; }
                case Flags.Root: { s = "Roots"; break; }
                default: { s = "Disarms"; break; } // Disarm
            }
            this.Text = this.Text.Replace("SitChanges", s);
            SetListBox();
        }
        #endregion
        #region Variables
        public enum Flags {
            Stun = 0,
            Move,
            Root,
            Fear,
            Disarm,
        }
        Flags Flag = Flags.Stun;
        protected List<Impedance> _TheList = null;
        public List<Impedance> TheList
        {
            get { return _TheList ?? (_TheList = new List<Impedance>()); }
            set { _TheList = value; }
        }
        #endregion
        #region Functions
        private void SetListBox()
        {
            LB_TheList.Items.Clear();
            switch (Flag) {
                /*case Flags.Move: {
                    foreach (Impedence s in TheList)
                    {
                        string str = s.ToString();
                        LB_TheList.Items.Add(str);
                    }
                    break;
                }
                case Flags.Fear: {
                    foreach (Impedence s in TheList)
                    {
                        string str = s.ToString();
                        LB_TheList.Items.Add(str);
                    }
                    break;
                }
                case Flags.Root: {
                    foreach (Impedence s in TheList)
                    {
                        string str = s.ToString();
                        LB_TheList.Items.Add(str);
                    }
                    break;
                }
                case Flags.Disarm: {
                    foreach (Impedence s in TheList)
                    {
                        string str = s.ToString();
                        LB_TheList.Items.Add(str);
                    }
                    break;
                }*/
                default: {
                    foreach (Impedance s in TheList)
                    {
                        string str = s.ToString();
                        LB_TheList.Items.Add(str);
                    }
                    break;
                }
            }
        }
        private void BT_Delete_Click(object sender, EventArgs e) {
            int index = LB_TheList.SelectedIndex;
            if (index == -1) { return; }
            switch (Flag) {
                /*case Flags.Move: { MoveList.RemoveAt(index); break; }
                case Flags.Fear: { FearList.RemoveAt(index); break; }
                case Flags.Root: { RootList.RemoveAt(index); break; }
                case Flags.Disarm: { DisarmList.RemoveAt(index); break; }*/
                default: { TheList.RemoveAt(index); break; }
            }
            SetListBox();
        }
        private void BT_Add_Click(object sender, EventArgs e) {
            switch (Flag)
            {
                /*case Flags.Move: {
                    Impedence s = new Impedence()
                    {
                        Frequency = (float)NUD_Freq.Value,
                        Duration = (float)NUD_Dur.Value,
                        Chance = ((float)NUD_Chance.Value) / 100f,
                        Breakable = CK_Breakable.Checked,
                    };
                    MoveList.Add(s);
                    break;
                }
                case Flags.Fear: {
                    Impedence s = new Impedence()
                    {
                        Frequency = (float)NUD_Freq.Value,
                        Duration = (float)NUD_Dur.Value,
                        Chance = ((float)NUD_Chance.Value) / 100f,
                        Breakable = CK_Breakable.Checked,
                    };
                    FearList.Add(s);
                    break;
                }
                case Flags.Root: {
                    Impedence s = new Impedence()
                    {
                        Frequency = (float)NUD_Freq.Value,
                        Duration = (float)NUD_Dur.Value,
                        Chance = ((float)NUD_Chance.Value) / 100f,
                        Breakable = CK_Breakable.Checked,
                    };
                    RootList.Add(s);
                    break;
                }
                case Flags.Disarm: {
                    Impedence s = new Impedence()
                    {
                        Frequency = (float)NUD_Freq.Value,
                        Duration = (float)NUD_Dur.Value,
                        Chance = ((float)NUD_Chance.Value) / 100f,
                        Breakable = CK_Breakable.Checked,
                    };
                    DisarmList.Add(s);
                    break;
                }*/
                default: {
                    Impedance s = new Impedance()
                    {
                        Frequency = (float)NUD_Freq.Value,
                        Duration = (float)NUD_Dur.Value,
                        Chance = ((float)NUD_Chance.Value) / 100f,
                        Breakable = CK_Breakable.Checked,
                    };
                    TheList.Add(s);
                    break;
                }
            }
            SetListBox();
        }
        #endregion
    }
}
