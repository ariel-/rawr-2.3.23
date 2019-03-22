using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Rawr.CustomControls;

namespace Rawr.Hunter
{
    public partial class PetBuffSelector : UserControl
    {
        public PetBuffSelector()
        {
            InitializeComponent();
            if (!this.DesignMode)
            {
                BuildControls();
                //Calculations.ModelChanged += new EventHandler(Calculations_ModelChanged);
                //Rawr.UserControls.Options.GeneralSettings.DisplayBuffChanged += new EventHandler(GeneralSettings_DisplayBuffChanged);
                //FormMain.RaceChanged += new EventHandler(Character_RaceChanged);
            }
        }

        private CalculationOptionsHunter options;
        private bool _loadingBuffs = false;
        private Character _character = null;

        public Character character {
            get { return _character; }
            set {
                if (_character != null) {
                    _character.CalculationsInvalidated -= new EventHandler(Character_ItemsChanged);
                }
                _character = value;

                if (_character != null) {
                    _character.CalculationsInvalidated += new EventHandler(Character_ItemsChanged);
                    options = character.CalculationOptions as CalculationOptionsHunter;
                    LoadBuffsFromOptions();
                }

                UpdateEnabledStates();
            }
        }

        Dictionary<string, GroupBox> GroupBoxes = new Dictionary<string, GroupBox>();
        Dictionary<Buff, CheckBox> CheckBoxes = new Dictionary<Buff, CheckBox>();
        private void BuildControls()
        {
            this.Controls.Clear();
            this.GroupBoxes.Clear();
            this.CheckBoxes.Clear();
            this.SuspendLayout();

            List<Buff> buffs = CalculationsHunter.RelevantPetBuffs;

            foreach (Buff buff in buffs)
            {
                if (!GroupBoxes.ContainsKey(buff.Group))
                {
                    GroupBox groupBox = new GroupBox();
                    groupBox.Text = buff.Group;
                    groupBox.Tag = buff.Group;
                    groupBox.Font = new Font(this.Font.FontFamily, 7f);
                    groupBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                    groupBox.MouseMove += new MouseEventHandler(BuffSelector_MouseMove);
                    groupBox.Dock = DockStyle.Top;
                    GroupBoxes.Add(buff.Group, groupBox);
                    this.Controls.Add(groupBox);
                    groupBox.BringToFront();
                }
            }

            foreach (Buff buff in buffs)
            {
                ExtendedToolTipCheckBox checkBox = new ExtendedToolTipCheckBox();
                checkBox.Tag = buff;
                if (Rawr.Properties.GeneralSettings.Default.DisplayBuffSource && buff.Source != null)
                    checkBox.Text = buff.Name + " (" + buff.Source + ")";
                else
                    checkBox.Text = buff.Name;
                checkBox.AutoSize = true;
                checkBox.Font = this.Font;
                checkBox.Dock = DockStyle.Top;
                checkBox.ToolTipText = buff.Stats.ToString();
                checkBox.CheckedChanged += new EventHandler(checkBoxBuff_CheckedChanged);
                GroupBoxes[buff.Group].Controls.Add(checkBox);
                checkBox.BringToFront();
                CheckBoxes.Add(buff, checkBox);

                foreach (Buff improvement in buff.Improvements)
                {
                    ExtendedToolTipCheckBox checkBoxImprovement = new ExtendedToolTipCheckBox();
                    checkBoxImprovement.Tag = improvement;
                    if (Rawr.Properties.GeneralSettings.Default.DisplayBuffSource && improvement.Source != null)
                        checkBoxImprovement.Text = improvement.Name + " (" + improvement.Source + ")";
                    else
                        checkBoxImprovement.Text = improvement.Name;
                    checkBoxImprovement.Padding = new Padding(8 + checkBoxImprovement.Padding.Left,
                        checkBoxImprovement.Padding.Top, checkBoxImprovement.Padding.Right, checkBoxImprovement.Padding.Bottom);
                    checkBoxImprovement.AutoSize = true;
                    checkBoxImprovement.Font = this.Font;
                    checkBoxImprovement.Dock = DockStyle.Top;
                    checkBoxImprovement.ToolTipText = improvement.Stats.ToString();
                    checkBoxImprovement.CheckedChanged += new EventHandler(checkBoxBuff_CheckedChanged);
                    GroupBoxes[buff.Group].Controls.Add(checkBoxImprovement);
                    checkBoxImprovement.BringToFront();
                    CheckBoxes.Add(improvement, checkBoxImprovement);
                }
            }

            foreach (GroupBox groupBox in GroupBoxes.Values)
            {
                groupBox.Height = groupBox.Controls[0].Bottom + 2;
            }
            UpdateEnabledStates();
            this.ResumeLayout();
        }

        private void checkBoxBuff_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loadingBuffs)
            {
                UpdateEnabledStates();
                UpdateCharacterBuffs();
                if (character != null) character.OnCalculationsInvalidated();
            }
        }

        private void UpdateCharacterBuffs()
        {
            List<Buff> activeBuffs = new List<Buff>();
            foreach (CheckBox checkBox in CheckBoxes.Values)
                if (checkBox.Checked && checkBox.Enabled)
                    activeBuffs.Add((checkBox.Tag as Buff));
            options.petActiveBuffs = activeBuffs;
        }

        private void UpdateEnabledStates()
        {
            List<string> currentConflictNames = new List<string>();

            foreach (CheckBox checkBox in CheckBoxes.Values)
            {
                if (checkBox.Checked)
                {
                    Buff buff = checkBox.Tag as Buff;
                    foreach (string conflictName in buff.ConflictingBuffs)
                        if (!string.IsNullOrEmpty(conflictName) && !currentConflictNames.Contains(conflictName))
                            currentConflictNames.Add(conflictName);
                }
            }

            foreach (CheckBox checkBox in CheckBoxes.Values)
            {
                checkBox.Enabled = true;
                Buff buff = checkBox.Tag as Buff;
                if (!string.IsNullOrEmpty(buff.SetName))
                {
                    checkBox.Enabled = false;
                    continue;
                }
                if (string.IsNullOrEmpty(buff.Group))
                {
                    checkBox.Enabled = CheckBoxes[GetParentBuff(buff)].Checked;
                    if (!checkBox.Enabled) continue;
                }
                if (!checkBox.Checked)
                {
                    foreach (string buffName in buff.ConflictingBuffs)
                    {
                        if (currentConflictNames.Contains(buffName))
                        {
                            checkBox.Enabled = false;
                            break;
                        }
                    }
                }
            }
        }

        private Buff GetParentBuff(Buff childBuff)
        {
            foreach (Buff parentBuff in CalculationsHunter.RelevantPetBuffs)
                if (parentBuff.Improvements.Contains(childBuff))
                    return parentBuff;
            return null;
        }

        private ExtendedToolTipCheckBox disabledToolTipControl = null;
        void BuffSelector_MouseMove(object sender, MouseEventArgs e)
        {
            Control child = (sender as Control).GetChildAtPoint(e.Location);
            if (child != null && child.Enabled == false)
            {
                if (disabledToolTipControl != child && child is ExtendedToolTipCheckBox)
                {
                    if (disabledToolTipControl != null)
                    {
                        disabledToolTipControl.ForceHideToolTip();
                    }
                    disabledToolTipControl = child as ExtendedToolTipCheckBox;
                    disabledToolTipControl.ForceShowToolTip();
                }
            }
            else if (disabledToolTipControl != null)
            {
                disabledToolTipControl.ForceHideToolTip();
                disabledToolTipControl = null;
            }
        }

        public void LoadBuffsFromOptions()
        {
            _loadingBuffs = true;
            foreach (CheckBox checkBox in CheckBoxes.Values)
                checkBox.Checked = false;
            foreach (Buff buff in options.petActiveBuffs)
            {
                if (buff != null)
                {
                    if (CheckBoxes.ContainsKey(buff))
                        CheckBoxes[buff].Checked = true;
                }
            }
            _loadingBuffs = false;
        }

        void Character_ItemsChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                Invoke((EventHandler)Character_ItemsChanged, sender, e);
                return;
            }
            LoadBuffsFromOptions();
            UpdateEnabledStates();
        }
    }
}
