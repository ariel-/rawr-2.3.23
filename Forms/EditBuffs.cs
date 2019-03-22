using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr.Forms
{

    public partial class EditBuffs : Form
    {
        private List<Buff> _buffs;
        public EditBuffs(List<Buff> buffs)
        {
            InitializeComponent();
            _buffs = Rawr.Utilities.Clone<List<Buff>>(buffs);
			List<string> catNames = new List<string>();
            //foreach (Buff.BuffCategory cat in Enum.GetValues(typeof(Buff.BuffCategory)))
            //{
			//	catNames.Add(cat.ToString());    
            //}
			catNames.Sort();
			for (int i = 0; i < catNames.Count; i++)
			{
				listView1.Groups.Add(catNames[i], catNames[i]);
			}
			listView1.Sorting = SortOrder.Ascending;
        }

		public List<Buff> EditedBuffs
        {
            get { return _buffs; }
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
            for(int i=0;i<_buffs.Count;i++)
            {
                Buff currentBuff = _buffs[i];
                if (string.IsNullOrEmpty(Filter.Text) || currentBuff.Name.ToLower().Contains(Filter.Text.ToLower()))
                {
                    //ListViewItem lvi = new ListViewItem(currentBuff.Name, listView1.Groups[currentBuff.Category.ToString()]);
                    //lvi.Tag = currentBuff;
                    //listView1.Items.Add(lvi);
                }
            }
			listView1.Sort();
        }

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView1.SelectedIndices.Count > 0)
			{
				Buff buff = listView1.SelectedItems[0].Tag as Buff;
				BuffName.Text = buff.Name;
				Stats.SelectedObject = buff.Stats;
			}
		}
    }
}
