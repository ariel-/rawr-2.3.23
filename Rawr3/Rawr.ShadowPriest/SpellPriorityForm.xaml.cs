using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rawr.ShadowPriest
{
    public partial class SpellPriorityForm : ChildWindow
    {
        public List<string> SpellPriority { get; protected set; }
        public ListBox MirrowList { get; set; }
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

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            if (cmbSpells.SelectedItem == null)
                return;

            lsSpellPriority.Items.Add(cmbSpells.SelectedItem);
            cmbSpells.Items.RemoveAt(cmbSpells.SelectedIndex);
            cmbSpells.SelectedItem = "";
        }

        private void bUp_Click(object sender, RoutedEventArgs e)
        {
            if (lsSpellPriority.SelectedItem == null || lsSpellPriority.SelectedIndex == 0)
                return;

            lsSpellPriority.Items.Insert(lsSpellPriority.SelectedIndex - 1, lsSpellPriority.SelectedItem);
            lsSpellPriority.Items.RemoveAt(lsSpellPriority.SelectedIndex);
        }

        private void bDown_Click(object sender, RoutedEventArgs e)
        {
            if (lsSpellPriority.SelectedItem == null || lsSpellPriority.SelectedIndex == lsSpellPriority.Items.Count - 1)
                return;

            lsSpellPriority.Items.Insert(lsSpellPriority.SelectedIndex + 2, lsSpellPriority.SelectedItem);
            lsSpellPriority.Items.RemoveAt(lsSpellPriority.SelectedIndex);
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lsSpellPriority.SelectedItem == null)
                return;

            cmbSpells.Items.Add(lsSpellPriority.SelectedItem);
            lsSpellPriority.Items.RemoveAt(lsSpellPriority.SelectedIndex);
        }

        private void bClear_Click(object sender, RoutedEventArgs e)
        {
            if (lsSpellPriority.Items.Count == 0)
                return;

            foreach (Object o in lsSpellPriority.Items)
                cmbSpells.Items.Add(o);

            lsSpellPriority.Items.Clear();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

