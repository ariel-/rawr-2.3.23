using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr.Forms
{

    public partial class EditEnchants : Form
    {
        private List<Enchant> _Enchants;
		public EditEnchants(List<Enchant> enchants)
        {
            InitializeComponent();
            _Enchants = Rawr.Utilities.Clone<List<Enchant>>(enchants);
			List<string> catNames = new List<string>();
            foreach (CharacterSlot slot in Enum.GetValues(typeof(CharacterSlot)))
            {
				catNames.Add(slot.ToString());    
            }
			catNames.Sort();
			for (int i = 0; i < catNames.Count; i++)
			{
				listView1.Groups.Add(catNames[i], catNames[i]);
			}
			listView1.Sorting = SortOrder.Ascending;
        }

		public List<Enchant> EditedEnchants
        {
            get { return _Enchants; }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void Edit_Load(object sender, EventArgs e)
        {
            LoadListView();
        }

        private void LoadListView()
        {
            for(int i=0;i<_Enchants.Count;i++)
            {
                Enchant currentEnchant = _Enchants[i];
                if (string.IsNullOrEmpty(Filter.Text) || currentEnchant.Name.ToLower().Contains(Filter.Text.ToLower()))
                {
                    ListViewItem lvi = new ListViewItem(currentEnchant.Name, listView1.Groups[currentEnchant.Slot.ToString()]);
                    lvi.Tag = currentEnchant;
                    listView1.Items.Add(lvi);
                }
            }
			listView1.Sort();
        }

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView1.SelectedIndices.Count > 0)
			{
				Enchant enchant = listView1.SelectedItems[0].Tag as Enchant;
				EnchantName.Text = enchant.Name;
				Stats.SelectedObject = enchant.Stats;
			}
		}
    }
}
