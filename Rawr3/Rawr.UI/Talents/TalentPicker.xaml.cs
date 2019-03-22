using System;
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
    public partial class TalentPicker : UserControl
    {
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
                    character.ClassChanged += new EventHandler(character_ClassChanged);
                    character_ClassChanged(this, EventArgs.Empty);
                }
            }
        }

        public void RefreshSpec() { character_ClassChanged(this, EventArgs.Empty); }
        private void character_ClassChanged(object sender, EventArgs e)
        {
            Tree1.Talents = Character.CurrentTalents;
            TreeTab1.Header = Tree1.TreeName;
            Tree2.Talents = Character.CurrentTalents;
            TreeTab2.Header = Tree2.TreeName;
            Tree3.Talents = Character.CurrentTalents;
            TreeTab3.Header = Tree3.TreeName;
            Glyph.Character = Character;
            
            UpdateSavedSpecs();
        }

        public TalentPicker()
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
            Glyph.TalentsChanged += new EventHandler(TalentsChanged);
        }

        public void TalentsChanged(object sender, EventArgs e)
        {
            UpdateSavedSpecs();
            Character.OnTalentChange();
            Character.OnCalculationsInvalidated();
        }

        public bool HasCustomSpec { get; private set; }

        private bool updating;
        private void UpdateSavedSpecs()
        {
            SavedTalentSpecList savedSpecs = SavedTalentSpec.SpecsFor(Character.Class);
            SavedTalentSpec current = null;
            updating = true;
            foreach (SavedTalentSpec sts in savedSpecs)
            {
                if (sts.Equals(Character.CurrentTalents))
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
                HasCustomSpec = true;
                current = new SavedTalentSpec("Custom", Character.CurrentTalents, Tree1.Points(), Tree2.Points(), Tree3.Points());
                SavedTalentSpecList currentList = new SavedTalentSpecList();
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
            SavedTalentSpec currentSpec = SavedCombo.SelectedItem as SavedTalentSpec;
            if (HasCustomSpec)
            {
                SaveTalentSpecDialog dialog = new SaveTalentSpecDialog(currentSpec.TalentSpec(),
                    currentSpec.Tree1, currentSpec.Tree2, currentSpec.Tree3);
                dialog.Closed += new EventHandler(dialog_Closed);
                dialog.Show();
            }
            else
            {
                SavedTalentSpec.AllSpecs.Remove(currentSpec);
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
                SavedTalentSpec newSpec = SavedCombo.SelectedItem as SavedTalentSpec;
                Character.CurrentTalents = newSpec.TalentSpec();
                Character.OnTalentChange();
                character_ClassChanged(this, EventArgs.Empty);
                Character.OnCalculationsInvalidated();
            }
        }

    }
}