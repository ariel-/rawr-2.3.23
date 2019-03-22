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

namespace Rawr.Hunter
{
    public partial class FormSavePetTalentSpec : ChildWindow
    {
        private PetTalentsBase PetTalents;
        private int Tree;
        private PetFamilyTree Class;

        public FormSavePetTalentSpec(PetTalentsBase spec, PetFamilyTree newClass, int tree)
        {
            InitializeComponent();
            PetTalents = spec;
            Tree = tree;
            Class = newClass;
            CB_Trees.SelectedItem = Class;

            SavedPetTalentSpecList saved = SavedPetTalentSpec.SpecsFor(Class);
            if (saved.Count > 0) {
                CB_TalentSpecs.ItemsSource = saved;
                CB_TalentSpecs.SelectedIndex = 0;
            } else {
                CB_TalentSpecs.IsEnabled = false;
            }
        }

        private void BT_OK_Click(object sender, RoutedEventArgs e)
        {
            if (CB_TalentSpecs.SelectedIndex >= 0)
            {
                SavedPetTalentSpec spec = CB_TalentSpecs.SelectedItem as SavedPetTalentSpec;
                spec.Spec = PetTalents.ToString();
                spec.Class = (PetFamilyTree)CB_Trees.SelectedItem;
                spec.Tree = Tree;
            }
            else
            {
                SavedPetTalentSpec.AllSpecs.Add(new SavedPetTalentSpec(TB_NewSpecName.Text, PetTalents, (PetFamilyTree)CB_Trees.SelectedItem, Tree));
            }
            this.DialogResult = true;
        }

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private bool responding;
        private void New_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (responding) return;
            responding = true;
            CB_TalentSpecs.SelectedIndex = -1;
            responding = false;
        }

        private void Update_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (responding) return;
            responding = true;
            TB_NewSpecName.Text = "";
            responding = false;
        }
    }
}
