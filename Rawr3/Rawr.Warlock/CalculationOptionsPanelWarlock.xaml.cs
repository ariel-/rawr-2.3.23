using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rawr.Warlock
{
    public partial class CalculationOptionsPanelWarlock : UserControl, ICalculationOptionsPanel
    {
        public CalculationOptionsPanelWarlock()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region ICalculationOptionsPanel Members
        public UserControl PanelControl { get { return this; } }

        CalculationOptionsWarlock calcOpts = null;

        private Character character;
        public Character Character
        {
            get { return character; }
            set
            {
                // Kill any old event connections
                if (character != null && character.CalculationOptions != null
                    && character.CalculationOptions is CalculationOptionsWarlock)
                    ((CalculationOptionsWarlock)character.CalculationOptions).PropertyChanged
                        -= new PropertyChangedEventHandler(CalculationOptionsPanelWarlock_PropertyChanged);
                // Apply the new character
                character = value;
                // Load the new CalcOpts
                LoadCalculationOptions();
                // Model Specific Code
                // Set the Data Context
                LayoutRoot.DataContext = calcOpts;
                // Add new event connections
                calcOpts.PropertyChanged += new PropertyChangedEventHandler(CalculationOptionsPanelWarlock_PropertyChanged);
                // Run it once for any special UI config checks
                CalculationOptionsPanelWarlock_PropertyChanged(null, new PropertyChangedEventArgs(""));
            }
        }

        private bool _loadingCalculationOptions;
        public void LoadCalculationOptions()
        {
            _loadingCalculationOptions = true;
            if (Character.CalculationOptions == null) Character.CalculationOptions = new CalculationOptionsWarlock();
            calcOpts = Character.CalculationOptions as CalculationOptionsWarlock;
            // Model Specific Code
            RefreshRotationPanel();
            //
            _loadingCalculationOptions = false;
        }

        void CalculationOptionsPanelWarlock_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_loadingCalculationOptions) { return; }
            // This would handle any special changes, especially combobox assignments, but not when the pane is trying to load
            if (e.PropertyName == "SomeProperty")
            {
                // Do some code
            }
            //
            if (Character != null) { Character.OnCalculationsInvalidated(); }
        }
        #endregion

        #region Options Tab
        #region Methods
        private void RefreshRotationPanel() {
            _loadingCalculationOptions = true;

            Rotation rotation = calcOpts.GetActiveRotation();

            rotationCombo.Items.Clear();
            foreach (Rotation r in calcOpts.Rotations) {
                rotationCombo.Items.Add(r.Name);
            }
            rotationCombo.SelectedItem = rotation.Name;

            rotationMenu.Items.Clear();
            foreach (string spell in Spell.ALL_SPELLS) {
                if (!GetActivePriorities().Contains(spell)
                    && !fillerCombo.Items.Contains(spell)
                    && !executeCombo.Items.Contains(spell)) {

                    rotationMenu.Items.Add(spell);
                }
            }

            rotationList.Items.Clear();
            foreach (string spell in GetActivePriorities()) {
                rotationList.Items.Add(spell);
            }

            fillerCombo.SelectedItem = rotation.Filler;
            if (rotation.Execute == null) {
                executeCombo.SelectedItem = "None";
            } else {
                executeCombo.SelectedItem = rotation.Execute;
            }

            RefreshRotationButtons();

            _loadingCalculationOptions = false;
        }
        private void RefreshRotationButtons() {

            int itemCount = rotationList.Items.Count;
            int curIndex = rotationList.SelectedIndex;

            rotationAddButton.IsEnabled = rotationMenu.SelectedIndex >= 0;
            rotationUpButton.IsEnabled = curIndex > 0;
            rotationDownButton.IsEnabled = curIndex >= 0 && curIndex < itemCount - 1;
            rotationClearButton.IsEnabled = itemCount > 0;

            rotationErrorLabel.Text = calcOpts.GetActiveRotation().GetError();
        }
        private void RotationSwap(int swapWith) {

            int oldIndex = rotationList.SelectedIndex;
            int newIndex = oldIndex + swapWith;
            Utilities.SwapElements(GetActivePriorities(), oldIndex, newIndex);
            RefreshRotationPanel();
            rotationList.SelectedIndex = newIndex;
        }
        private List<string> GetActivePriorities() {
            return calcOpts.GetActiveRotation().SpellPriority;
        }
        private string PromptForRotationName(string title, string start) {
            string error = null;
            while (true) {
                string message = "Choose a name:";
                if (error != null) {
                    message = error + message;
                }
                string name = TextInputDialog.Text;
                //string name = TextInputDialog.Show(title, message, start);
                if (name == null) {
                    return null;
                }
                if (name.Length == 0) {
                    error = "The name cannot be blank.  ";
                    continue;
                }

                error = null;
                foreach (Rotation rotation in calcOpts.Rotations) {
                    if (rotation.Name == name) {
                        error = "There is already a rotation by that name.  ";
                        break;
                    }
                }
                if (error == null) {
                    return name;
                }
            }
        }
        #endregion
        private void rotationAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_loadingCalculationOptions) { return; }
            _loadingCalculationOptions = true;
            GetActivePriorities().Add((string) rotationMenu.SelectedItem);
            RefreshRotationPanel();
            Character.OnCalculationsInvalidated();
            _loadingCalculationOptions = false;
        }
        private void rotationRemoveButton_Click(object sender, RoutedEventArgs e)
        {

            if (_loadingCalculationOptions) { return; }

            int index = rotationList.SelectedIndex;
            GetActivePriorities().RemoveAt(index);
            RefreshRotationPanel();
            int itemCount = rotationList.Items.Count;
            if (itemCount > 0) {
                rotationList.SelectedIndex = Math.Min(index, itemCount - 1);
            }
            Character.OnCalculationsInvalidated();
        }
        private void rotationUpButton_Click(object sender, RoutedEventArgs e)
        {

            if (_loadingCalculationOptions) { return; }

            RotationSwap(-1);
            Character.OnCalculationsInvalidated();
        }
        private void rotationDownButton_Click(object sender, RoutedEventArgs e)
        {

            if (_loadingCalculationOptions) { return; }

            RotationSwap(1);
            Character.OnCalculationsInvalidated();
        }
        private void rotationClearButton_Click(object sender, RoutedEventArgs e)
        {

            if (_loadingCalculationOptions) { return; }

            GetActivePriorities().Clear();
            RefreshRotationPanel();
            Character.OnCalculationsInvalidated();
        }
        private void rotationMenu_SelectedIndexChanged(object sender, SelectionChangedEventArgs e) {

            if (_loadingCalculationOptions) { return; }

            RefreshRotationButtons();
        }
        private void rotationList_SelectedIndexChanged(object sender, SelectionChangedEventArgs e) {

            if (_loadingCalculationOptions) { return; }

            RefreshRotationButtons();
        }
        private void fillerCombo_SelectedIndexChanged(object sender, SelectionChangedEventArgs e) {

            if (_loadingCalculationOptions) { return; }

            calcOpts.GetActiveRotation().Filler
                = (string) fillerCombo.SelectedItem;
            Character.OnCalculationsInvalidated();
        }
        private void rotationCombo_SelectedIndexChanged(object sender, SelectionChangedEventArgs e) {
            if (_loadingCalculationOptions) { return; }
            calcOpts.ActiveRotationIndex = rotationCombo.SelectedIndex;
            RefreshRotationPanel();
            Character.OnCalculationsInvalidated();
        }
        private void newRotationButton_Click(object sender, RoutedEventArgs e)
        {

            if (_loadingCalculationOptions) { return; }

            string name = PromptForRotationName("New Rotation", "");
            if (name == null) {
                return;
            }
            calcOpts.ActiveRotationIndex = calcOpts.Rotations.Count;
            calcOpts.Rotations.Add(new Rotation(name, "Shadow Bolt", null));

            RefreshRotationPanel();
            Character.OnCalculationsInvalidated();
        }
        private void rotationRenameButton_Click(object sender, RoutedEventArgs e) {

            if (_loadingCalculationOptions) { return; }

            Rotation rotation = calcOpts.GetActiveRotation();
            string name
                = PromptForRotationName("Rename Rotation", rotation.Name);
            if (name == null) { return; }
            rotation.Name = name;

            RefreshRotationPanel();
            Character.OnCalculationsInvalidated();
        }
        private void deleteRotationButton_Click(object sender, RoutedEventArgs e) {

            if (_loadingCalculationOptions) { return; }

            calcOpts.RemoveActiveRotation();
            if (calcOpts.Rotations.Count == 0) {
                calcOpts.Rotations.Add(new Rotation("New Rotation", "Shadow Bolt", null));
                calcOpts.ActiveRotationIndex = 0;
            }

            RefreshRotationPanel();
            Character.OnCalculationsInvalidated();
        }
        private void executeCombo_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_loadingCalculationOptions) { return; }

            if ((string)executeCombo.SelectedItem == "None") {
                calcOpts.GetActiveRotation().Execute = null;
            } else {
                calcOpts.GetActiveRotation().Execute = (string)executeCombo.SelectedItem;
            }
            Character.OnCalculationsInvalidated();
        }
        #endregion
        #region Debug Tab
        private void TimerButton_Click(object sender, RoutedEventArgs e)
        {
            /*System.Diagnostics.Stopwatch clock = new System.Diagnostics.Stopwatch();
            CalculationsWarlock calculations = (CalculationsWarlock)Calculations.Instance;
            Character character = Character;
            clock.Start();
            for (int i = 0; i < 4000; i++) {
                calculations.GetCharacterCalculations(character);
            }
            clock.Stop();
            MessageBox.Show(clock.Elapsed.TotalSeconds + " seconds.");*/
        }
        #endregion
    }
}
