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

namespace Rawr.UI
{
    public partial class BuffsControl : UserControl
    {

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
                    LoadBuffsFromCharacter();
                }
                UpdateEnabledStates();
                UpdateSavedSets();
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
            LoadBuffsFromCharacter();
            UpdateEnabledStates();
            UpdateSavedSets();
        }

        private bool _loadingBuffsFromCharacter;
        private void LoadBuffsFromCharacter()
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
                UpdateSavedSets();
            }
        }

        private void UpdateCharacterBuffs()
        {
            List<Buff> activeBuffs = new List<Buff>();
            foreach (CheckBox checkBox in CheckBoxes.Values)
                if (checkBox.IsChecked.GetValueOrDefault(false) && checkBox.IsEnabled)
                    activeBuffs.Add((checkBox.Tag as Buff));
            Character.ActiveBuffs = activeBuffs;
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
                if (Character != null && buff.Name == "Heroic Presence")
                {
                    if (Character.Race == CharacterRace.Draenei ||
                        Character.Faction == CharacterFaction.Horde)
                    {
                        checkBox.IsEnabled = false;
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(buff.Group) && buff.Group == "Profession Buffs") {
                    checkBox.IsEnabled = false;
                    continue;
                }
                if (buff.Name.Contains("Mixology") && !character.HasProfession(Profession.Alchemy)) {
                    checkBox.IsEnabled = false;
                    checkBox.Visibility = Visibility.Collapsed;
                    continue;
                } else if (buff.Name.Contains("Mixology") && character.HasProfession(Profession.Alchemy)) {
                    checkBox.IsEnabled = true;
                    checkBox.Visibility = Visibility.Visible;
                    continue;
                }
                if (!string.IsNullOrEmpty(buff.SetName))
                {
                    checkBox.IsEnabled = false;
                    continue;
                }
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
            Dictionary<string, GroupBox> buffGroups = new Dictionary<string, GroupBox>();
            if (Buff.RelevantBuffs != null)
            {
                foreach (Buff b in Buff.RelevantBuffs)
                {
                    GroupBox gb;
                    if (buffGroups.ContainsKey(b.Group)) gb = buffGroups[b.Group];
                    else
                    {
                        gb = new GroupBox();
                        gb.Header = b.Group;
                        StackPanel sp = new StackPanel();
                        gb.Content = sp;
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
                    ((StackPanel)gb.Content).Children.Add(buffCb);
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
                        ((StackPanel)gb.Content).Children.Add(buffCb);
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
                UpdateSavedSets();
                Character.OnCalculationsInvalidated();
            }
        }

        public BuffsControl()
        {
            InitializeComponent();
            TheScroll.SetIsMouseWheelScrollingEnabled(true);

            OptionsDialog.DisplayBuffChanged += new EventHandler(GeneralSettings_DisplayBuffChanged);
            OptionsDialog.HideProfessionsChanged += new EventHandler(GeneralSettings_HideProfessionsChanged);
        }

        void GeneralSettings_DisplayBuffChanged(object sender, EventArgs e)
        {
            BuildControls();
        }

        void GeneralSettings_HideProfessionsChanged(object sender, EventArgs e)
        {
            BuildControls();
        }

        #region Saving Buff Sets
        public bool HasCustomSets { get; private set; }

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
                SaveBuffSetDialog dialog = new SaveBuffSetDialog(currentSet.BuffSet);
                dialog.Closed += new EventHandler(dialog_Closed);
                dialog.Show();
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
                LoadBuffsFromCharacter(); // This updates the pane with new checkboxes
                UpdateCharacterBuffs(); // This updates the character against the boxes actually checked, should fix the double-buff issue seen
                Character.OnCalculationsInvalidated();
            }
        }
        #endregion

        private void BT_BuffsByRaidMembers_Click(object sender, RoutedEventArgs e)
        {
            DG_BuffsByRaidMembers dialog = new DG_BuffsByRaidMembers();
            dialog.Closed += new EventHandler(BuffsByRaidMembers_Closed);
            dialog.Show();
        }
        private void BuffsByRaidMembers_Closed(object sender, EventArgs e) {
            if (((ChildWindow)sender).DialogResult.GetValueOrDefault(false))
            {
                // do something
            }
        }
    }
}
