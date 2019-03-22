using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Rawr.Hunter
{
    public partial class PetTalentPicker : UserControl
    {
        CalculationOptionsHunter CalcOpts = null;

        private Character character;
        public Character Character
        {
            get { return character; }
            set
            {
                if (character != null) { character.ClassChanged -= new EventHandler(character_ClassChanged); }
                character = value;
                if (character != null)
                {
                    CalcOpts = character.CalculationOptions as CalculationOptionsHunter;
                    character.ClassChanged += new EventHandler(character_ClassChanged);
                    character_ClassChanged(this, EventArgs.Empty);
                }
            }
        }

        public void RefreshSpec() { character_ClassChanged(this, EventArgs.Empty); }
        private void character_ClassChanged(object sender, EventArgs e)
        {
            if (Character.Class != CharacterClass.Hunter) return;
            Tree1.Talents = CalcOpts.PetTalents;
            TreeTab1.Header = Tree1.TreeName;
            Tree2.Talents = CalcOpts.PetTalents;
            TreeTab2.Header = Tree2.TreeName;
            Tree3.Talents = CalcOpts.PetTalents;
            TreeTab3.Header = Tree3.TreeName;
            
            UpdateSavedSpecs();
        }

        public PetTalentPicker()
        {
            // Required to initialize variables
            InitializeComponent();
            Tree1.Tree = 0;
            Tree2.Tree = 1;
            Tree3.Tree = 2;

            Scroll1.SetIsMouseWheelScrollingEnabled(true);
            Scroll2.SetIsMouseWheelScrollingEnabled(true);
            Scroll3.SetIsMouseWheelScrollingEnabled(true);

            Tree1.TalentsChanged += new EventHandler(TalentsChanged);
            Tree2.TalentsChanged += new EventHandler(TalentsChanged);
            Tree3.TalentsChanged += new EventHandler(TalentsChanged);
        }

        public void TalentsChanged(object sender, EventArgs e)
        {
            CalcOpts.PetTalents = (PetTalents)(sender == Tree1 ? Tree1.Talents :
                                              (sender == Tree2 ? Tree2.Talents :
                                              (sender == Tree3 ? Tree3.Talents :
                                               CalcOpts.PetTalents)));
            UpdateSavedSpecs();
            Character.OnCalculationsInvalidated();
        }

        public bool HasCustomSpec { get; private set; }

        private bool updating;
        private void UpdateSavedSpecs()
        {
            SavedPetTalentSpecList savedSpecs = SavedPetTalentSpec.SpecsFor(CalculationOptionsHunter.PetFamilyToPetFamilyTree(CalcOpts.PetFamily));
            SavedPetTalentSpec current = null;
            updating = true;
            foreach (SavedPetTalentSpec sts in savedSpecs)
            {
                if (sts.Equals(CalcOpts.PetTalents))
                {
                    current = sts;
                    break;
                }
            }

            if (current != null)
            {
                HasCustomSpec = false;
                SavedCombo.ItemsSource = savedSpecs;
                SavedCombo.SelectedItem = current;
                SaveDeleteButton.Content = "Delete";
            }
            else
            {
                PetFamilyTree ftree = CalculationOptionsHunter.PetFamilyToPetFamilyTree(CalcOpts.PetFamily);
                int treepts = (ftree == PetFamilyTree.Cunning  ? Tree1.Points() :
                              (ftree == PetFamilyTree.Ferocity ? Tree2.Points() :
                              (ftree == PetFamilyTree.Tenacity ? Tree3.Points() :
                               Tree1.Points() + Tree2.Points() + Tree3.Points())));
                HasCustomSpec = true;
                current = new SavedPetTalentSpec("Custom", CalcOpts.PetTalents, ftree, treepts);
                SavedPetTalentSpecList currentList = new SavedPetTalentSpecList();
                currentList.AddRange(savedSpecs);
                currentList.Add(current);
                SavedCombo.ItemsSource = null;
                SavedCombo.ItemsSource = currentList;
                SavedCombo.SelectedItem = current;
                SaveDeleteButton.Content = "Save";
            }
            updating = false;
        }

        private void SaveDelete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SavedPetTalentSpec currentSpec = SavedCombo.SelectedItem as SavedPetTalentSpec;
            if (HasCustomSpec) {
                PetFamilyTree ftree = CalculationOptionsHunter.PetFamilyToPetFamilyTree(CalcOpts.PetFamily);
                int treepts = (ftree == PetFamilyTree.Cunning ? Tree1.Points() :
                              (ftree == PetFamilyTree.Ferocity ? Tree2.Points() :
                              (ftree == PetFamilyTree.Tenacity ? Tree3.Points() :
                               Tree1.Points() + Tree2.Points() + Tree3.Points())));
                FormSavePetTalentSpec dialog = new FormSavePetTalentSpec(CalcOpts.PetTalents, ftree, treepts);
                dialog.Closed += new EventHandler(dialog_Closed);
                dialog.Show();
            } else {
                SavedPetTalentSpec.AllSpecs.Remove(currentSpec);
                UpdateSavedSpecs();
            }
        }

        private void dialog_Closed(object sender, EventArgs e)
        {
            if (((ChildWindow)sender).DialogResult.GetValueOrDefault(false))
            {
                UpdateSavedSpecs();
            }
        }

        private void SavedCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!updating)
            {
                SavedPetTalentSpec newSpec = SavedCombo.SelectedItem as SavedPetTalentSpec;
                CalcOpts.PetTalents = newSpec.TalentSpec();
                character_ClassChanged(this, EventArgs.Empty);
                Character.OnCalculationsInvalidated();
            }
        }
    }
}