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
    public partial class PetBuffSelector : UserControl
    {
        private CalculationOptionsHunter options;

        private Character character;
        public Character Character
        {
            get { return character; }
            set
            {
                if (character != null) { character.CalculationsInvalidated -= new EventHandler(Character_ItemsChanged); }
                character = value;
                _loadingBuffsFromCharacter = true;
                BuildControls();
                if (character != null)
                {
                    character.CalculationsInvalidated += new EventHandler(Character_ItemsChanged);
                    options = character.CalculationOptions as CalculationOptionsHunter;
                    LoadBuffsFromOptions();
                }
                UpdateEnabledStates();
                //UpdateSavedSets();
                _loadingBuffsFromCharacter = false;
            }
        }

        private void Character_ItemsChanged(object sender, EventArgs e)
        {
            if (!CheckAccess())
            {
                Dispatcher.BeginInvoke((EventHandler)Character_ItemsChanged, sender, e);
                //Invoke((EventHandler)Character_ItemsChanged, sender, e);
                //InvokeHelper.Invoke(this, "Character_ItemsChanged", new object[] { null, null });
                return;
            }
            LoadBuffsFromOptions();
            UpdateEnabledStates();
            //UpdateSavedSets();
        }

        private bool _loadingBuffsFromCharacter;
        /*private void LoadBuffsFromCharacter()
        {
            try
            {
                _loadingBuffsFromCharacter = true;

                if (CheckBoxes == null || Character == null || Character.ActiveBuffs == null) return;
                foreach (KeyValuePair<Buff, CheckBox> kvp in CheckBoxes)
                {
                    kvp.Value.IsChecked = false;
                }
                List<CheckBox> toCheck = new List<CheckBox>();
                foreach (Buff b in Character.ActiveBuffs)
                {
                    if (CheckBoxes.ContainsKey(Buff.GetBuffByName(b.Name)))
                    {
                        toCheck.Add(CheckBoxes[Buff.GetBuffByName(b.Name)]);
                    }
                }
                foreach (CheckBox cb in toCheck) cb.IsChecked = true;
            }
            finally
            {
                _loadingBuffsFromCharacter = false;
                //UpdateSavedSets();
            }
        }*/

        public void LoadBuffsFromOptions()
        {
            try
            {
                if (CheckBoxes == null || Character == null || options.petActiveBuffs == null) return;
                _loadingBuffsFromCharacter = true;
                foreach (KeyValuePair<Buff, CheckBox> kvp in CheckBoxes)
                    kvp.Value.IsChecked = false;
                List<CheckBox> toCheck = new List<CheckBox>();
                foreach (Buff b in options.petActiveBuffs)
                {
                    if (CheckBoxes.ContainsKey(Buff.GetBuffByName(b.Name)))
                    {
                        toCheck.Add(CheckBoxes[Buff.GetBuffByName(b.Name)]);
                    }
                }
                foreach (CheckBox cb in toCheck) cb.IsChecked = true;
            }
            finally
            {
                _loadingBuffsFromCharacter = false;
                //UpdateSavedSets();
            }
        }

        private void UpdateCharacterBuffs()
        {
            List<Buff> activeBuffs = new List<Buff>();
            foreach (CheckBox checkBox in CheckBoxes.Values)
                if (checkBox.IsChecked.GetValueOrDefault(false) && checkBox.IsEnabled)
                    activeBuffs.Add((checkBox.Tag as Buff));
            options.petActiveBuffs = activeBuffs;
        }

        public void UpdateEnabledStates()
        {
            List<string> currentConflictNames = new List<string>();
            foreach (CheckBox checkBox in CheckBoxes.Values)
            {
                if (checkBox.IsChecked.GetValueOrDefault(false))
                {
                    Buff buff = checkBox.Tag as Buff;
                    foreach (string conflictName in buff.ConflictingBuffs)
                        if (!string.IsNullOrEmpty(conflictName) && !currentConflictNames.Contains(conflictName))
                            currentConflictNames.Add(conflictName);
                }
            }

            foreach (CheckBox checkBox in CheckBoxes.Values)
            {
                checkBox.IsEnabled = true;
                //Buff buff = checkBox.Tag as Buff;
                Buff buff = (Buff)checkBox.Tag;
                if (string.IsNullOrEmpty(buff.Group))
                {
                    if (FindParent.ContainsKey(buff) && !CheckBoxes[FindParent[buff]].IsChecked.GetValueOrDefault(false))
                    {
                        checkBox.IsEnabled = false;
                        continue;
                    }
                }
                if (!checkBox.IsChecked.GetValueOrDefault(false))
                {
                    foreach (string buffName in buff.ConflictingBuffs)
                    {
                        if (currentConflictNames.Contains(buffName))
                        {
                            checkBox.IsEnabled = false;
                            break;
                        }
                    }
                }
            }
        }

        private Dictionary<Buff, CheckBox> CheckBoxes;
        private Dictionary<Buff, Buff> FindParent;
        private void BuildControls()
        {
            if (CheckBoxes == null) CheckBoxes = new Dictionary<Buff, CheckBox>();
            else CheckBoxes.Clear();

            if (FindParent == null) FindParent = new Dictionary<Buff, Buff>();
            else FindParent.Clear();

            BuffStack.Children.Clear();
            Dictionary<string, Grid /*GroupBox*/> buffGroups = new Dictionary<string, Grid/*GroupBox*/>();
            if (CalculationsHunter.RelevantPetBuffs != null)
            {
                foreach (Buff b in CalculationsHunter.RelevantPetBuffs)
                {
                    Grid/*GroupBox*/ gb;
                    if (buffGroups.ContainsKey(b.Group)) gb = buffGroups[b.Group];
                    else
                    {
                        gb = new Grid/*GroupBox*/();
                        //gb.Header = b.Group;
                        StackPanel sp = new StackPanel();
                        gb.Children.Add(/*Content =*/ sp);
                        BuffStack.Children.Add(gb);
                        buffGroups[b.Group] = gb;
                    }
                    CheckBox buffCb = new CheckBox();
                    if (Rawr.Properties.GeneralSettings.Default.DisplayBuffSource && b.Source != null) {
                        buffCb.Content = b.Name + " (" + b.Source + ")";
                    } else {
                        buffCb.Content = b.Name;
                    }
                    ToolTipService.SetToolTip(buffCb, Calculations.GetRelevantStats(b.Stats).ToString());
                    buffCb.Checked += new RoutedEventHandler(buffCb_CheckedChange);
                    buffCb.Unchecked += new RoutedEventHandler(buffCb_CheckedChange);
                    buffCb.Tag = b;
                    ((StackPanel)gb.Children[0]/*Content*/).Children.Add(buffCb);
                    CheckBoxes[b] = buffCb;

                    foreach (Buff i in b.Improvements)
                    {
                        buffCb = new CheckBox();
                        if (Rawr.Properties.GeneralSettings.Default.DisplayBuffSource && i.Source != null) {
                            buffCb.Content = i.Name + " (" + i.Source + ")";
                        } else {
                            buffCb.Content = i.Name;
                        }
                        ToolTipService.SetToolTip(buffCb, Calculations.GetRelevantStats(i.Stats).ToString());
                        buffCb.Margin = new Thickness(16, 0, 0, 0);
                        buffCb.Checked += new RoutedEventHandler(buffCb_CheckedChange);
                        buffCb.Unchecked += new RoutedEventHandler(buffCb_CheckedChange);
                        buffCb.Tag = i;
                        CheckBoxes[i] = buffCb;
                        FindParent[i] = b;
                        ((StackPanel)gb.Children[0]/*Content*/).Children.Add(buffCb);
                    }
                }
            }
        }

        void buffCb_CheckedChange(object sender, RoutedEventArgs e)
        {
            if (!_loadingBuffsFromCharacter && Character != null)
            {
                CheckBox cb = (CheckBox)sender;
                Buff b = (Buff)cb.Tag;
                UpdateEnabledStates();
                UpdateCharacterBuffs();
                //UpdateSavedSets();
                Character.OnCalculationsInvalidated();
            }
        }

        public PetBuffSelector()
        {
            InitializeComponent();
            //TheScroll.SetIsMouseWheelScrollingEnabled(true);

            //OptionsDialog.DisplayBuffChanged += new EventHandler(GeneralSettings_DisplayBuffChanged);
            //OptionsDialog.HideProfessionsChanged += new EventHandler(GeneralSettings_HideProfessionsChanged);
        }

        void GeneralSettings_DisplayBuffChanged(object sender, EventArgs e)
        {
            BuildControls();
        }

        void GeneralSettings_HideProfessionsChanged(object sender, EventArgs e)
        {
            BuildControls();
        }
        /*public bool HasCustomSets { get; private set; }

        private bool updating;
        private void UpdateSavedSets()
        {
            SavedBuffSetList savedSets = SavedBuffSet.AllSets;
            SavedBuffSet current = null;
            updating = true;
            foreach (SavedBuffSet sbs in savedSets)
            {
                if (sbs.Equals(Character.ActiveBuffs))
                {
                    current = sbs;
                    break;
                }
            }

            if (current != null)
            {
                HasCustomSets = false;
                SavedCombo.ItemsSource = savedSets;
                SavedCombo.SelectedItem = current;
                SaveDeleteButton.Content = "Delete";
            }
            else
            {
                HasCustomSets = true;
                current = new SavedBuffSet("Custom", Character.ActiveBuffs);
                SavedBuffSetList currentList = new SavedBuffSetList();
                currentList.AddRange(savedSets);
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
            SavedBuffSet currentSet = SavedCombo.SelectedItem as SavedBuffSet;
            if (HasCustomSets)
            {
                //SaveBuffSetDialog dialog = new SaveBuffSetDialog(currentSet.BuffSet);
                //dialog.Closed += new EventHandler(dialog_Closed);
                //dialog.Show();
            }
            else
            {
                SavedBuffSet.AllSets.Remove(currentSet);
                UpdateSavedSets();
            }
        }

        private void dialog_Closed(object sender, EventArgs e)
        {
            if (((ChildWindow)sender).DialogResult.GetValueOrDefault(false))
            {
                UpdateSavedSets();
            }
        }

        private void SavedCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!updating)
            {
                SavedBuffSet newSet = SavedCombo.SelectedItem as SavedBuffSet;
                Character.ActiveBuffs = newSet.BuffSet;
                LoadBuffsFromCharacter();
                Character.OnCalculationsInvalidated();
            }
        }*/
    }
}
