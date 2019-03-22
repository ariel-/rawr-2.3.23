using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System;

namespace Rawr
{
    public partial class FormEnterNameRealm : Form
    {
        public FormEnterNameRealm()
        {
            InitializeComponent();
            if (Rawr.Properties.Recent.Default.RecentChars != null) {
                int count = Rawr.Properties.Recent.Default.RecentChars.Count;
                if (count > 0) {
                    string[] autocomplete = new string[count];
                    Rawr.Properties.Recent.Default.RecentChars.CopyTo(autocomplete, 0);
                    textBoxName.AutoCompleteCustomSource.AddRange(autocomplete);
                    textBoxName.Text = Rawr.Properties.Recent.Default.RecentChars[count - 1];
                }
            } else { Rawr.Properties.Recent.Default.RecentChars = new System.Collections.Specialized.StringCollection(); }
            if (Rawr.Properties.Recent.Default.RecentServers != null) {
                int count = Rawr.Properties.Recent.Default.RecentServers.Count;
                if (count > 0) {
                    string[] autocomplete = new string[count];
                    Rawr.Properties.Recent.Default.RecentServers.CopyTo(autocomplete, 0);
                    textBoxRealm.AutoCompleteCustomSource.AddRange(autocomplete);
                    textBoxRealm.Text = Rawr.Properties.Recent.Default.RecentServers[count - 1];
                }
            } else { Rawr.Properties.Recent.Default.RecentServers = new System.Collections.Specialized.StringCollection(); }
            if (Rawr.Properties.Recent.Default.RecentRegion != null) {
                comboBoxRegion.Text = Rawr.Properties.Recent.Default.RecentRegion;
            } else { Rawr.Properties.Recent.Default.RecentRegion = "US"; }
        }

        public string CharacterName
        {
            get { return textBoxName.Text; }
        }

        public string Realm
        {
            get { return textBoxRealm.Text; }
        }

        public CharacterRegion ArmoryRegion
        {
            get { return (CharacterRegion)Enum.Parse(typeof(CharacterRegion), comboBoxRegion.Text); }
        }

        private void FormEnterNameRealm_Activated(object sender, System.EventArgs e)
        {
            this.textBoxName.Focus();
        }
    }
}