using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr.ShadowPriest
{
    public partial class SpellPriorityForm : Form
    {
        public List<string> SpellPriority { get; protected set; }
        private ListBox MirrowList { get; set; }
        private Character character;

        public SpellPriorityForm(List<string> spells, ListBox mirrowList, Character _character)
        {
            SpellPriority = spells;
            MirrowList = mirrowList;
            character = _character;

            InitializeComponent();
            foreach (string spell in Spell.ShadowSpellList)
            {
                if (spells.Contains(spell))
                    lsSpellPriority.Items.Add(spell);
                else
                    cmbSpells.Items.Add(spell);
            }
        }

        private void bAdd_Click(object sender, EventArgs e)
        {
            if (cmbSpells.SelectedItem == null)
                return;

            lsSpellPriority.Items.Add(cmbSpells.SelectedItem);
            cmbSpells.Items.RemoveAt(cmbSpells.SelectedIndex);
            cmbSpells.SelectedText = "";
        }

        private void bUp_Click(object sender, EventArgs e)
        {
            if (lsSpellPriority.SelectedItem == null || lsSpellPriority.SelectedIndex == 0)
                return;

            lsSpellPriority.Items.Insert(lsSpellPriority.SelectedIndex - 1, lsSpellPriority.SelectedItem);
            lsSpellPriority.Items.RemoveAt(lsSpellPriority.SelectedIndex);
        }

        private void bDown_Click(object sender, EventArgs e)
        {
            if (lsSpellPriority.SelectedItem == null || lsSpellPriority.SelectedIndex == lsSpellPriority.Items.Count - 1)
                return;

            lsSpellPriority.Items.Insert(lsSpellPriority.SelectedIndex + 2, lsSpellPriority.SelectedItem);
            lsSpellPriority.Items.RemoveAt(lsSpellPriority.SelectedIndex);
        }

        private void bRemove_Click(object sender, EventArgs e)
        {
            if (lsSpellPriority.SelectedItem == null)
                return;

            cmbSpells.Items.Add(lsSpellPriority.SelectedItem);
            lsSpellPriority.Items.RemoveAt(lsSpellPriority.SelectedIndex);
        }

        private void bClear_Click(object sender, EventArgs e)
        {
            if (lsSpellPriority.Items.Count == 0)
                return;

            foreach(Object o in lsSpellPriority.Items)
                cmbSpells.Items.Add(o);

            lsSpellPriority.Items.Clear();
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            SpellPriority.Clear();
            MirrowList.Items.Clear();
            foreach (Object o in lsSpellPriority.Items)
            {
                SpellPriority.Add(o.ToString());
                MirrowList.Items.Add(o);
            }
            character.OnCalculationsInvalidated();
            Hide();
        }        
    }
}
