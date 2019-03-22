using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

namespace Rawr.Warlock
{
    public partial class CalculationOptionsPanelWarlock : CalculationOptionsPanelBase
    {
        #region properties
        private CalculationOptionsWarlock _options;
        private int _ignoreCount;
        #endregion

        #region methods
        private void RefreshRotationPanel() {

            ++_ignoreCount;

            Rotation rotation = _options.GetActiveRotation();

            rotationCombo.Items.Clear();
            foreach (Rotation r in _options.Rotations) {
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

            --_ignoreCount;
        }
        private void RefreshRotationButtons() {

            int itemCount = rotationList.Items.Count;
            int curIndex = rotationList.SelectedIndex;

            rotationAddButton.Enabled = rotationMenu.SelectedIndex >= 0;
            rotationUpButton.Enabled = curIndex > 0;
            rotationDownButton.Enabled
                = curIndex >= 0 && curIndex < itemCount - 1;
            rotationClearButton.Enabled = itemCount > 0;

            rotationErrorLabel.Text = _options.GetActiveRotation().GetError();
        }
        private void RotationSwap(int swapWith) {

            int oldIndex = rotationList.SelectedIndex;
            int newIndex = oldIndex + swapWith;
            Utilities.SwapElements(GetActivePriorities(), oldIndex, newIndex);
            RefreshRotationPanel();
            rotationList.SelectedIndex = newIndex;
        }
        private List<string> GetActivePriorities() {

            return _options.GetActiveRotation().SpellPriority;
        }
        private string PromptForRotationName(string title, string start) {

            string error = null;
            while (true) {
                string message = "Choose a name:";
                if (error != null) {
                    message = error + message;
                }
                string name
                    = TextInputDialog.Show(title, message, start);
                if (name == null) {
                    return null;
                }
                if (name.Length == 0) {
                    error = "The name cannot be blank.  ";
                    continue;
                }

                error = null;
                foreach (Rotation rotation in _options.Rotations) {
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

        #region initialization
        public CalculationOptionsPanelWarlock() {
            InitializeComponent();
            //
            petCombo.Items.Add("None");
            foreach (string pet in Pet.ALL_PETS) {
                petCombo.Items.Add(pet);
            }
        }
        protected override void LoadCalculationOptions() {
            if (Character.CalculationOptions == null) {
                Character.CalculationOptions = CalculationOptionsWarlock.MakeDefaultOptions();
            }
            _options = (CalculationOptionsWarlock) Character.CalculationOptions;
            ++_ignoreCount;

            // options tab
            petCombo.SelectedItem = _options.Pet;
            imbueCombo.SelectedItem = _options.Imbue;
            targetLevelCombo.Text = _options.TargetLevel.ToString();
            fightLengthSpinner.Value = (decimal) _options.Duration;
            latencySpinner.Value = (decimal) _options.Latency * 1000;
            thirtyFiveSpinner.Value = (decimal) _options.ThirtyFive * 100;
            twentyFiveSpinner.Value = (decimal) _options.TwentyFive * 100;
            RefreshRotationPanel();

            // raid buff tab
            raidSPSpinner.Value = (decimal) _options.PerSP;
            raidTotemCheckbox.Checked = _options.ConvertTotem;
            raidTotemSpinner.Value = (decimal) _options.PerFlametongue;
            raidMagicSpinner.Value = (decimal) _options.PerMagicBuff;
            raidCritSpinner.Value = (decimal) _options.PerCritBuff;
            raidIntSpinner.Value = (decimal) _options.PerInt;
            raidSpiSpinner.Value = (decimal) _options.PerSpi;
            raidHealthSpinner.Value = (decimal) _options.PerHealth;

            // debug tab
            procCheckbox.Checked = _options.NoProcs;

            --_ignoreCount;
        }
        #endregion

        #region options tab event handlers

        private void petCombo_SelectedIndexChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.Pet = (string) petCombo.SelectedItem;
            Character.OnCalculationsInvalidated();
        }

        private void infernalCheck_CheckedChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }
            

            Character.OnCalculationsInvalidated();
        }

        private void targetLevelCombo_SelectedIndexChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.TargetLevel = Convert.ToInt32(targetLevelCombo.Text);
            Character.OnCalculationsInvalidated();
        }

        private void fightLengthSpinner_ValueChanged(
            object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.Duration = (float) fightLengthSpinner.Value;
            Character.OnCalculationsInvalidated();
        }

        private void latencySpinner_ValueChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.Latency = (float) latencySpinner.Value / 1000f;
            Character.OnCalculationsInvalidated();
        }

        private void rotationAddButton_Click(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            ++_ignoreCount;
            GetActivePriorities().Add((string) rotationMenu.SelectedItem);
            RefreshRotationPanel();
            Character.OnCalculationsInvalidated();
            --_ignoreCount;
        }

        private void rotationRemoveButton_Click(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            int index = rotationList.SelectedIndex;
            GetActivePriorities().RemoveAt(index);
            RefreshRotationPanel();
            int itemCount = rotationList.Items.Count;
            if (itemCount > 0) {
                rotationList.SelectedIndex = Math.Min(index, itemCount - 1);
            }
            Character.OnCalculationsInvalidated();
        }

        private void rotationUpButton_Click(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            RotationSwap(-1);
            Character.OnCalculationsInvalidated();
        }

        private void rotationDownButton_Click(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            RotationSwap(1);
            Character.OnCalculationsInvalidated();
        }

        private void rotationClearButton_Click(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            GetActivePriorities().Clear();
            RefreshRotationPanel();
            Character.OnCalculationsInvalidated();
        }

        private void rotationMenu_SelectedIndexChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            RefreshRotationButtons();
        }

        private void rotationList_SelectedIndexChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            RefreshRotationButtons();
        }

        private void fillerCombo_SelectedIndexChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.GetActiveRotation().Filler
                = (string) fillerCombo.SelectedItem;
            Character.OnCalculationsInvalidated();
        }

        private void rotationCombo_SelectedIndexChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.ActiveRotationIndex = rotationCombo.SelectedIndex;
            RefreshRotationPanel();
            Character.OnCalculationsInvalidated();
        }

        private void newRotationButton_Click(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            string name = PromptForRotationName("New Rotation", "");
            if (name == null) {
                return;
            }
            _options.ActiveRotationIndex = _options.Rotations.Count;
            _options.Rotations.Add(new Rotation(name, "Shadow Bolt", null));

            RefreshRotationPanel();
            Character.OnCalculationsInvalidated();
        }

        private void rotationRenameButton_Click(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            Rotation rotation = _options.GetActiveRotation();
            string name
                = PromptForRotationName("Rename Rotation", rotation.Name);
            if (name == null) {
                return;
            }
            rotation.Name = name;

            RefreshRotationPanel();
            Character.OnCalculationsInvalidated();
        }

        private void deleteRotationButton_Click(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.RemoveActiveRotation();
            if (_options.Rotations.Count == 0) {
                _options.Rotations.Add(
                    new Rotation("New Rotation", "Shadow Bolt", null));
                _options.ActiveRotationIndex = 0;
            }

            RefreshRotationPanel();
            Character.OnCalculationsInvalidated();
        }

        private void imbueCombo_SelectedIndexChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.Imbue = imbueCombo.Text;
            Character.OnCalculationsInvalidated();
        }

        private void executeCombo_SelectedIndexChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            if (executeCombo.Text == "None") {
                _options.GetActiveRotation().Execute = null;
            } else {
                _options.GetActiveRotation().Execute = executeCombo.Text;
            }
            Character.OnCalculationsInvalidated();
        }

        private void thirtyFiveSpinner_ValueChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.ThirtyFive = (float) thirtyFiveSpinner.Value / 100f;
            Character.OnCalculationsInvalidated();
        }

        private void twentyFiveSpinner_ValueChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.TwentyFive = (float) twentyFiveSpinner.Value / 100f;
            Character.OnCalculationsInvalidated();
        }

        #endregion

        #region raid buff tab event handlers

        private void raidSPSpinner_ValueChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.PerSP = (float) raidSPSpinner.Value;
            Character.OnCalculationsInvalidated();
        }

        private void raidTotemCheckbox_CheckedChanged(
            object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.ConvertTotem = raidTotemCheckbox.Checked;
            Character.OnCalculationsInvalidated();
        }

        private void raidTotemSpinner_ValueChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.PerFlametongue = (float) raidTotemSpinner.Value;
            Character.OnCalculationsInvalidated();
        }

        private void raidMagicSpinner_ValueChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.PerMagicBuff = (float) raidMagicSpinner.Value;
            Character.OnCalculationsInvalidated();
        }

        private void raidCritSpinner_ValueChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.PerCritBuff = (float) raidCritSpinner.Value;
            Character.OnCalculationsInvalidated();
        }

        private void raidIntSpinner_ValueChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.PerInt = (float) raidIntSpinner.Value;
            Character.OnCalculationsInvalidated();
        }

        private void raidSpiSpinner_ValueChanged(object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.PerSpi = (float) raidSpiSpinner.Value;
            Character.OnCalculationsInvalidated();
        }

        private void raidHealthSpinner_ValueChanged(
            object sender, EventArgs e) {

            if (_ignoreCount > 0) {
                return;
            }

            _options.PerHealth = (float) raidHealthSpinner.Value;
            Character.OnCalculationsInvalidated();
        }

        #endregion

        #region debug stuff

        private void procCheckbox_CheckedChanged(object sender, EventArgs e) {

            _options.NoProcs = procCheckbox.Checked;
            Character.OnCalculationsInvalidated();
        }

        private void TimerButton_Click(object sender, EventArgs e) {

            System.Diagnostics.Stopwatch clock
                = new System.Diagnostics.Stopwatch();
            CalculationsWarlock calculations
                = (CalculationsWarlock) Calculations.Instance;
            Character character = Character;
            clock.Start();
            for (int i = 0; i < 4000; i++) {
                calculations.GetCharacterCalculations(character);
            }
            clock.Stop();
            MessageBox.Show(clock.Elapsed.TotalSeconds + " seconds.");
        }

        #endregion
    }
}
