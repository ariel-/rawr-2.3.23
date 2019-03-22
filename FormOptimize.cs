using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Rawr.Optimizer;
using System.Reflection;

namespace Rawr
{
	public partial class FormOptimize : Form
	{
		private Character _character;
        private ItemInstanceOptimizer _optimizer;
        private Item _itemToEvaluate;

        private string[] talentList;

        public void EvaluateUpgrades(Item itemToEvaluate)
        {
            // Let's ensure that we're not seeing a null value incoming
            if (null != itemToEvaluate)
            {
                _itemToEvaluate = itemToEvaluate;
                buttonUpgrades_Click(null, null);
                _itemToEvaluate = null;
            }
        }

        private void InitializeTalentList(Character character)
        {
            talentList = new string[character.CurrentTalents.Data.Length];
            foreach (PropertyInfo pi in character.CurrentTalents.GetType().GetProperties())
            {
                TalentDataAttribute[] talentDatas = pi.GetCustomAttributes(typeof(TalentDataAttribute), true) as TalentDataAttribute[];
                if (talentDatas.Length > 0)
                {
                    TalentDataAttribute talentData = talentDatas[0];
                    talentList[talentData.Index] = talentData.Name;
                }
            }
        }

		public FormOptimize(Character character)
		{
            InitializeTalentList(character);

			InitializeComponent();
			_character = character;
            _optimizer = new ItemInstanceOptimizer();
            _optimizer.OptimizeCharacterProgressChanged += new OptimizeCharacterProgressChangedEventHandler(_optimizer_OptimizeCharacterProgressChanged);
            _optimizer.OptimizeCharacterCompleted += new OptimizeCharacterCompletedEventHandler(_optimizer_OptimizeCharacterCompleted);
            _optimizer.ComputeUpgradesProgressChanged += new ComputeUpgradesProgressChangedEventHandler(_optimizer_ComputeUpgradesProgressChanged);
            _optimizer.ComputeUpgradesCompleted += new ComputeUpgradesCompletedEventHandler(_optimizer_ComputeUpgradesCompleted);

			comboBoxCalculationToOptimize.Items.Add("Overall Rating");
			comboBoxCalculationToOptimize.Tag = Calculations.SubPointNameColors.Count;
			foreach (string subPoint in Calculations.SubPointNameColors.Keys)
				comboBoxCalculationToOptimize.Items.Add(subPoint + " Rating");
			comboBoxCalculationToOptimize.Items.AddRange(Calculations.OptimizableCalculationLabels);
			comboBoxCalculationToOptimize.SelectedIndex = 0;

            checkBoxOverrideRegem.Checked = Properties.Optimizer.Default.OverrideRegem;
            checkBoxOverrideReenchant.Checked = Properties.Optimizer.Default.OverrideReenchant;
            trackBarThoroughness.Value = Properties.Optimizer.Default.Thoroughness;
            string calculationString = character.CalculationToOptimize;
            if (string.IsNullOrEmpty(calculationString)) calculationString = Properties.Optimizer.Default.CalculationToOptimize;
            if (calculationString != null)
            {
                if (calculationString.StartsWith("[Overall]", StringComparison.Ordinal))
                {
                    comboBoxCalculationToOptimize.SelectedIndex = 0;
                }
                else if (calculationString.StartsWith("[SubPoint ", StringComparison.Ordinal))
                {
                    calculationString = calculationString.Substring(10).TrimEnd(']');
                    int index = int.Parse(calculationString);
                    if (index < Calculations.SubPointNameColors.Count)
                    {
                        comboBoxCalculationToOptimize.SelectedIndex = index + 1;
                    }
                }
                else
                {
                    if (Array.IndexOf(Calculations.OptimizableCalculationLabels, calculationString) >= 0)
                    {
                        comboBoxCalculationToOptimize.SelectedItem = calculationString;
                    }
                }
            }
            if (character.OptimizationRequirements != null)
            {
                for (int i = 0; i < character.OptimizationRequirements.Count; i++) buttonAddRequirement_Click(null, null);
                int reqIndex = 0;
                foreach (Control ctrl in groupBoxRequirements.Controls)
                {
                    if (ctrl is Panel)
                    {
                        foreach (Control reqCtrl in ctrl.Controls)
                        {
                            switch (reqCtrl.Name)
                            {
                                case "comboBoxRequirementCalculation":
                                    ComboBox reqComboBox = (ComboBox)reqCtrl;
                                    calculationString = character.OptimizationRequirements[reqIndex].Calculation;
                                    if (calculationString.StartsWith("[Overall]", StringComparison.Ordinal))
                                    {
                                        reqComboBox.SelectedIndex = 0;
                                    }
                                    else if (calculationString.StartsWith("[SubPoint ", StringComparison.Ordinal))
                                    {
                                        calculationString = calculationString.Substring(10).TrimEnd(']');
                                        int index = int.Parse(calculationString);
                                        if (index < Calculations.SubPointNameColors.Count)
                                        {
                                            reqComboBox.SelectedIndex = index + 1;
                                        }
                                    }
                                    else if (calculationString.StartsWith("[Talent ", StringComparison.Ordinal))
                                    {
                                        reqComboBox.SelectedItem = "Talent";
                                    }
                                    else if (calculationString.StartsWith("[Cost]", StringComparison.Ordinal))
                                    {
                                        reqComboBox.SelectedItem = "Cost";
                                    }
                                    else
                                    {
                                        if (Array.IndexOf(Calculations.OptimizableCalculationLabels, calculationString) >= 0)
                                        {
                                            reqComboBox.SelectedItem = calculationString;
                                        }
                                    }
                                    break;
                                case "comboBoxRequirementTalent":
                                    calculationString = character.OptimizationRequirements[reqIndex].Calculation;
                                    if (calculationString.StartsWith("[Talent ", StringComparison.Ordinal))
                                    {
                                        calculationString = calculationString.Substring(8).TrimEnd(']');
                                        int index = int.Parse(calculationString);
                                        if (index < talentList.Length)
                                        {
                                            ComboBox reqComboBoxTalent = (ComboBox)reqCtrl;
                                            reqComboBoxTalent.SelectedIndex = index;
                                        }
                                    }
                                    break;
                                case "comboBoxRequirementGreaterLessThan":
                                    (reqCtrl as ComboBox).SelectedIndex = character.OptimizationRequirements[reqIndex].LessThan ? 1 : 0;
                                    break;

                                case "numericUpDownRequirementValue":
                                    (reqCtrl as NumericUpDown).Value = (decimal)character.OptimizationRequirements[reqIndex].Value;
                                    break;
                            }
                        }
                        reqIndex++;
                    }
                }
            }
		}

		private void FormOptimize_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = _optimizer.IsBusy;
		}

        private List<TalentsBase> GetOptimizeTalentSpecs()
        {
            List<TalentsBase> talentSpecs = null;
            if (checkBoxOptimizeTalents.Checked || checkBoxMutateTalents.Checked)
            {
                talentSpecs = new List<TalentsBase>();
                foreach (SavedTalentSpec spec in FormMain.Instance.TalentPicker.SpecsFor(_character.Class))
                {
                    TalentsBase talents = spec.TalentSpec();
                    int totalPoints = 0;
                    for (int i = 0; i < talents.Data.Length; i++)
                    {
                        totalPoints += talents.Data[i];
                    }
                    if (totalPoints == _character.Level - 9)
                    {
                        talentSpecs.Add(talents);
                    }
                }
            }
            return talentSpecs;
        }

        /// <summary>
        /// Re-enables the requirements list, function separated for convenience
        /// </summary>
        private void EnableAllReqs() {
            foreach (Control ctrl in groupBoxRequirements.Controls)
            {
                ctrl.Enabled = true;
            }
        }

		private void buttonOptimize_Click(object sender, EventArgs e)
		{
            bool _overrideRegem = checkBoxOverrideRegem.Checked;
            bool _overrideReenchant = checkBoxOverrideReenchant.Checked;
            int _thoroughness = trackBarThoroughness.Value;
			string _calculationToOptimize = GetCalculationStringFromComboBox(comboBoxCalculationToOptimize, null);
            List<OptimizationRequirement> requirements = new List<OptimizationRequirement>();
            bool costRequirement = false;
			foreach (Control ctrl in groupBoxRequirements.Controls)
			{
				ctrl.Enabled = false;
                Panel panel = ctrl as Panel;
				if (panel != null) 
				{
                    OptimizationRequirement requirement = new OptimizationRequirement();
                    requirement.Calculation = GetCalculationStringFromComboBox(panel.Controls["comboBoxRequirementCalculation"] as ComboBox, panel.Controls["comboBoxRequirementTalent"] as ComboBox);
                    requirement.LessThan = (panel.Controls["comboBoxRequirementGreaterLessThan"] as ComboBox).SelectedIndex == 1;
                    requirement.Value = (float)((panel.Controls["numericUpDownRequirementValue"] as NumericUpDown).Value);
					requirements.Add(requirement);
                    if (requirement.Calculation == "[Cost]")
                    {
                        costRequirement = true;
                    }
				}
			}

            _optimizer.OptimizationMethod = Properties.Optimizer.Default.OptimizationMethod;
            _optimizer.GreedyOptimizationMethod = Properties.Optimizer.Default.GreedyOptimizationMethod;

            _optimizer.InitializeItemCache(_character, _character.AvailableItems, _overrideRegem, _overrideReenchant, Properties.Optimizer.Default.TemplateGemsEnabled, Calculations.Instance, checkBoxOptimizeFood.Checked, checkBoxOptimizeElixir.Checked, checkBoxMixology.Checked, GetOptimizeTalentSpecs(), checkBoxMutateTalents.Checked, costRequirement);
            if (Properties.Optimizer.Default.WarningsEnabled)
            {
                string prompt = _optimizer.GetWarningPromptIfNeeded();
                if (prompt != null)
                {
                    if (MessageBox.Show(prompt, "Optimizer Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) { EnableAllReqs(); return; }
                }
                prompt = _optimizer.CheckOneHandedWeaponUniqueness();
                if (prompt != null)
                {
                    if (MessageBox.Show(prompt, "Optimizer Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) { EnableAllReqs(); return; }
                }
                if (!_optimizer.ItemGenerator.IsCharacterValid(_character, out prompt, true))
                {
                    if (MessageBox.Show(prompt, "Optimizer Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) { EnableAllReqs(); return; }
                }
            }

			buttonOptimize.Text = "Optimizing...";
			buttonCancel.Text = "Cancel";
            buttonOptimize.Enabled = buttonUpgrades.Enabled = checkBoxOverrideRegem.Enabled = checkBoxOverrideReenchant.Enabled =
                trackBarThoroughness.Enabled = checkBoxMixology.Enabled = checkBoxOptimizeElixir.Enabled =
				checkBoxOptimizeFood.Enabled = checkBoxOptimizeTalents.Enabled = checkBoxMutateTalents.Enabled =
				comboBoxCalculationToOptimize.Enabled = false;

            buttonCancel.DialogResult = DialogResult.None;

            _optimizer.OptimizeCharacterAsync(_character, _calculationToOptimize, requirements, _thoroughness, false);
		}

		private string GetCalculationStringFromComboBox(ComboBox comboBox, ComboBox comboBoxTalent)
		{
			if (comboBox.SelectedIndex == 0)
				return "[Overall]";
			else if (comboBox.SelectedIndex <= (int)comboBox.Tag)
				return string.Format("[SubPoint {0}]", comboBox.SelectedIndex - 1);
            else if (comboBox.Text == "Talent")
                return string.Format("[Talent {0}]", comboBoxTalent.SelectedIndex);
            else if (comboBox.Text == "Cost")
                return "[Cost]";
			else
				return comboBox.Text;
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			if (_optimizer.IsBusy) _optimizer.CancelAsync();
		}

		void _optimizer_OptimizeCharacterProgressChanged(object sender, OptimizeCharacterProgressChangedEventArgs e)
		{
			labelMax.Text = e.BestValue.ToString();
			progressBarAlt.Value = e.ProgressPercentage;
			progressBarMain.Value = Math.Max(e.ProgressPercentage, progressBarMain.Value);

            Text = string.Format("{0}% Complete - Rawr Optimizer", progressBarMain.Value);
		}

        void _optimizer_OptimizeCharacterCompleted(object sender, OptimizeCharacterCompletedEventArgs e)
		{
            buttonCancel.DialogResult = DialogResult.Cancel;
            if (e.Cancelled)
            {
                labelMax.Text = string.Empty;
                buttonOptimize.Text = "Optimize";
                buttonCancel.Text = "Close";
                buttonOptimize.Enabled = buttonUpgrades.Enabled = checkBoxOverrideRegem.Enabled = checkBoxOverrideReenchant.Enabled =
                    trackBarThoroughness.Enabled = checkBoxMixology.Enabled = checkBoxOptimizeElixir.Enabled =
                    checkBoxOptimizeFood.Enabled = checkBoxOptimizeTalents.Enabled = checkBoxMutateTalents.Enabled =
                    comboBoxCalculationToOptimize.Enabled = true;
                foreach (Control ctrl in groupBoxRequirements.Controls)
                    ctrl.Enabled = true;
                progressBarAlt.Value = progressBarMain.Value = 0;
            }
            else
            {
                progressBarAlt.Value = progressBarMain.Value = 100;
                Character bestCharacter = e.OptimizedCharacter;
                if (bestCharacter == null)
                {
                    labelMax.Text = string.Empty;
                    buttonOptimize.Text = "Optimize";
                    buttonCancel.Text = "Close";
                    buttonOptimize.Enabled = buttonUpgrades.Enabled = checkBoxOverrideRegem.Enabled = checkBoxOverrideReenchant.Enabled =
                        trackBarThoroughness.Enabled = checkBoxMixology.Enabled = checkBoxOptimizeElixir.Enabled =
                        checkBoxOptimizeFood.Enabled = checkBoxOptimizeTalents.Enabled = checkBoxMutateTalents.Enabled =
                        comboBoxCalculationToOptimize.Enabled = true;
                    foreach (Control ctrl in groupBoxRequirements.Controls)
                        ctrl.Enabled = true;
                    progressBarAlt.Value = progressBarMain.Value = 0;
                    MessageBox.Show(this, "Sorry, Rawr was unable to find a gearset to meet your requirements.", "Rawr Optimizer Results");
                }

                if (_character != null)
                {
                    FormOptimizeResult result = new FormOptimizeResult(_character, bestCharacter);
                    result.SetOptimizerScores(e.CurrentCharacterValue, e.OptimizedCharacterValue);
                    result.ShowDialog(this);
//                    OptimizerResults results = new OptimizerResults(_character, bestCharacter, checkBoxOptimizeTalents.Checked);
//                    string msg = e.OptimizedCharacterValue >= 0 ?
//                        string.Format("The Optimizer found a gearset with a score of {0}. (Your currently equipped gear has a score of {1}) Would you like to equip the optimized gear?",
//                            e.OptimizedCharacterValue, e.CurrentCharacterValue) :
//                        "The Optimizer was not able to meet all the requirements. Would you like to equip the gear that is closest to meeting them?";
                    if (result.DialogResult == DialogResult.Yes)
                    {
                        //Loading new items while IsLoading==true causes properties to be reset to their previously cached values, 
                        //so load all the items beforehand, then put them into the character all at once.
                        _character.IsLoading = true;
                        _character.SetItems(bestCharacter, false, false);
                        _character.ActiveBuffs = bestCharacter.ActiveBuffs;
                        if (checkBoxOptimizeTalents.Checked || checkBoxMutateTalents.Checked)
                        {
                            _character.CurrentTalents = bestCharacter.CurrentTalents;
                        }
                        _character.IsLoading = false;
                        _character.OnCalculationsInvalidated();
                        if (checkBoxOptimizeTalents.Checked || checkBoxMutateTalents.Checked)
                        {
                            FormMain.Instance.TalentPicker.Talents = _character.CurrentTalents;
                        }
                        Close();
                        return;
                    }
                    result.Dispose();
                }
                labelMax.Text = string.Empty;
                buttonOptimize.Text = "Optimize";
                buttonCancel.Text = "Close";
                buttonOptimize.Enabled = buttonUpgrades.Enabled = checkBoxOverrideRegem.Enabled = checkBoxOverrideReenchant.Enabled =
                    trackBarThoroughness.Enabled = checkBoxMixology.Enabled = checkBoxOptimizeElixir.Enabled =
                    checkBoxOptimizeFood.Enabled = checkBoxOptimizeTalents.Enabled = checkBoxMutateTalents.Enabled =
                    comboBoxCalculationToOptimize.Enabled = true;
                foreach (Control ctrl in groupBoxRequirements.Controls)
                    ctrl.Enabled = true;
                progressBarAlt.Value = progressBarMain.Value = 0;
            }
		}

        void _optimizer_ComputeUpgradesProgressChanged(object sender, ComputeUpgradesProgressChangedEventArgs e)
        {
            labelMax.Text = e.CurrentItem;
            progressBarAlt.Value = e.ItemProgressPercentage;
            progressBarMain.Value = e.ProgressPercentage;

            Text = string.Format("{0}% Complete - Rawr Optimizer", progressBarMain.Value);
        }

        public bool ShowUpgradeComparison { get; set; }

        void _optimizer_ComputeUpgradesCompleted(object sender, ComputeUpgradesCompletedEventArgs e)
        {
            buttonCancel.DialogResult = DialogResult.Cancel;
            if (e.Cancelled)
            {
                labelMax.Text = string.Empty;
				buttonUpgrades.Text = "Build Upgrade List";
				buttonCancel.Text = "Close";
				buttonOptimize.Enabled = buttonUpgrades.Enabled = checkBoxOverrideRegem.Enabled = checkBoxOverrideReenchant.Enabled =
					trackBarThoroughness.Enabled = checkBoxMixology.Enabled = checkBoxOptimizeElixir.Enabled =
					checkBoxOptimizeFood.Enabled = checkBoxOptimizeTalents.Enabled = checkBoxMutateTalents.Enabled =
					comboBoxCalculationToOptimize.Enabled = true;
				foreach (Control ctrl in groupBoxRequirements.Controls)
					ctrl.Enabled = true;
                progressBarAlt.Value = progressBarMain.Value = 0;
            }
            else
            {
                progressBarAlt.Value = progressBarMain.Value = 100;
                FormUpgradeComparison.Instance.LoadData(e.Upgrades, null);
                ShowUpgradeComparison = true;
                Close();
            }
        }

		private void buttonAddRequirement_Click(object sender, EventArgs e)
		{
			buttonAddRequirement.Top += 29;

			Panel panelRequirement = new System.Windows.Forms.Panel();
			ComboBox comboBoxRequirementCalculation = new System.Windows.Forms.ComboBox();
            ComboBox comboBoxRequirementTalent = new System.Windows.Forms.ComboBox();
            ComboBox comboBoxRequirementGreaterLessThan = new System.Windows.Forms.ComboBox();
			NumericUpDown numericUpDownRequirementValue = new System.Windows.Forms.NumericUpDown();
			Button buttonRemoveRequirement = new System.Windows.Forms.Button();
			panelRequirement.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(numericUpDownRequirementValue)).BeginInit();
			// 
			// panelRequirement
			// 
			panelRequirement.Controls.Add(numericUpDownRequirementValue);
			panelRequirement.Controls.Add(buttonRemoveRequirement);
			panelRequirement.Controls.Add(comboBoxRequirementGreaterLessThan);
            panelRequirement.Controls.Add(comboBoxRequirementTalent);
            panelRequirement.Controls.Add(comboBoxRequirementCalculation);
			panelRequirement.Dock = System.Windows.Forms.DockStyle.Top;
			panelRequirement.Location = new System.Drawing.Point(3, 16);
			panelRequirement.Name = "panelRequirement";
			panelRequirement.Size = new System.Drawing.Size(294, 29);
			panelRequirement.TabIndex = 6;
			// 
			// comboBoxRequirementCalculation
			// 
            comboBoxRequirementCalculation.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
			comboBoxRequirementCalculation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboBoxRequirementCalculation.FormattingEnabled = true;
			comboBoxRequirementCalculation.Location = new System.Drawing.Point(64, 4);
			comboBoxRequirementCalculation.Name = "comboBoxRequirementCalculation";
			comboBoxRequirementCalculation.Size = new System.Drawing.Size(133, 21);
			comboBoxRequirementCalculation.TabIndex = 3;
            comboBoxRequirementCalculation.DropDownWidth = 233;
            comboBoxRequirementCalculation.SelectedIndexChanged += new EventHandler(comboBoxRequirementCalculation_SelectedIndexChanged);
            // 
            // comboBoxRequirementTalent
            // 
            comboBoxRequirementTalent.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            comboBoxRequirementTalent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxRequirementTalent.FormattingEnabled = true;
            comboBoxRequirementTalent.Location = new System.Drawing.Point(119, 4);
            comboBoxRequirementTalent.Name = "comboBoxRequirementTalent";
            comboBoxRequirementTalent.Size = new System.Drawing.Size(78, 21);
            comboBoxRequirementTalent.Visible = false;
            comboBoxRequirementTalent.TabIndex = 3;
            comboBoxRequirementTalent.DropDownWidth = 233;
            // 
			// comboBoxRequirementGreaterLessThan
			// 
			comboBoxRequirementGreaterLessThan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			comboBoxRequirementGreaterLessThan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboBoxRequirementGreaterLessThan.Items.AddRange(new object[] {
            "≥",
            "≤"});
			comboBoxRequirementGreaterLessThan.Location = new System.Drawing.Point(195, 4);
			comboBoxRequirementGreaterLessThan.Name = "comboBoxRequirementGreaterLessThan";
			comboBoxRequirementGreaterLessThan.Size = new System.Drawing.Size(30, 21);
			comboBoxRequirementGreaterLessThan.TabIndex = 3;
			// 
			// numericUpDownRequirementValue
			// 
			numericUpDownRequirementValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			numericUpDownRequirementValue.Location = new System.Drawing.Point(231, 5);
			numericUpDownRequirementValue.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			numericUpDownRequirementValue.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
			numericUpDownRequirementValue.Name = "numericUpDownRequirementValue";
			numericUpDownRequirementValue.Size = new System.Drawing.Size(60, 20);
			numericUpDownRequirementValue.TabIndex = 6;
			numericUpDownRequirementValue.ThousandsSeparator = true;
            numericUpDownRequirementValue.DecimalPlaces = 2;
			// 
			// buttonRemoveRequirement
			// 
			buttonRemoveRequirement.Location = new System.Drawing.Point(3, 3);
			buttonRemoveRequirement.Name = "buttonRemoveRequirement";
			buttonRemoveRequirement.Size = new System.Drawing.Size(55, 23);
			buttonRemoveRequirement.TabIndex = 5;
			buttonRemoveRequirement.Text = "Remove";
			buttonRemoveRequirement.UseVisualStyleBackColor = true;
			buttonRemoveRequirement.Click += new EventHandler(buttonRemoveRequirement_Click);


			comboBoxRequirementCalculation.Items.Add("Overall Rating");
			comboBoxRequirementCalculation.Tag = Calculations.SubPointNameColors.Count;
			foreach (string subPoint in Calculations.SubPointNameColors.Keys)
				comboBoxRequirementCalculation.Items.Add(subPoint + " Rating");
            comboBoxRequirementCalculation.Items.AddRange(Calculations.OptimizableCalculationLabels);
            comboBoxRequirementCalculation.Items.Add("Talent");
            comboBoxRequirementCalculation.Items.Add("Cost");

            comboBoxRequirementTalent.Items.AddRange(talentList);
			
			comboBoxRequirementCalculation.SelectedIndex = comboBoxRequirementTalent.SelectedIndex = comboBoxRequirementGreaterLessThan.SelectedIndex = 0;
			groupBoxRequirements.Controls.Add(panelRequirement);
            ((System.ComponentModel.ISupportInitialize)(numericUpDownRequirementValue)).EndInit();
            panelRequirement.ResumeLayout();
            panelRequirement.BringToFront();
		}

        void comboBoxRequirementCalculation_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBoxRequirementCalculation = (ComboBox)sender;
            Panel panel = (Panel)comboBoxRequirementCalculation.Parent;
            ComboBox comboBoxRequirementTalent = (ComboBox)panel.Controls["comboBoxRequirementTalent"];
            if ((string)comboBoxRequirementCalculation.SelectedItem == "Talent")
            {
                comboBoxRequirementCalculation.Size = new System.Drawing.Size(55, 21);
                comboBoxRequirementTalent.Visible = true;
            }
            else
            {
                comboBoxRequirementCalculation.Size = new System.Drawing.Size(133, 21);
                comboBoxRequirementTalent.Visible = false;
            }
        }

		void buttonRemoveRequirement_Click(object sender, EventArgs e)
		{
            // need to remove event handlers on child controls, otherwise it'll cause lingering references
            Button buttonRemoveRequirement = (Button)sender;
            Panel panel = (Panel)buttonRemoveRequirement.Parent;
            ComboBox comboBoxRequirementTalent = (ComboBox)panel.Controls["comboBoxRequirementCalculation"];

            comboBoxRequirementTalent.SelectedIndexChanged -= new EventHandler(comboBoxRequirementCalculation_SelectedIndexChanged);
            buttonRemoveRequirement.Click -= new EventHandler(buttonRemoveRequirement_Click);

            panel.Parent.Controls.Remove(panel);
			buttonAddRequirement.Top -= 29;
		}
		
        private void buttonUpgrades_Click(object sender, EventArgs e)
        {
            bool _overrideRegem = checkBoxOverrideRegem.Checked;
            bool _overrideReenchant = checkBoxOverrideReenchant.Checked;
            int _thoroughness = (int)Math.Ceiling((float)trackBarThoroughness.Value / 10f);
            string _calculationToOptimize = GetCalculationStringFromComboBox(comboBoxCalculationToOptimize, null);
            List<OptimizationRequirement> requirements = new List<OptimizationRequirement>();
            foreach (Control ctrl in groupBoxRequirements.Controls)
            {
				ctrl.Enabled = false;
                Panel panel = ctrl as Panel;
                if (panel != null)
                {
                    OptimizationRequirement requirement = new OptimizationRequirement();
                    requirement.Calculation = GetCalculationStringFromComboBox(panel.Controls["comboBoxRequirementCalculation"] as ComboBox, panel.Controls["comboBoxRequirementTalent"] as ComboBox);
                    requirement.LessThan = (panel.Controls["comboBoxRequirementGreaterLessThan"] as ComboBox).SelectedIndex == 1;
                    requirement.Value = (float)((panel.Controls["numericUpDownRequirementValue"] as NumericUpDown).Value);
                    requirements.Add(requirement);
                }
            }

            if ((_overrideReenchant || _overrideRegem || _thoroughness > 100) && Properties.Optimizer.Default.WarningsEnabled)
            {
                if (MessageBox.Show("The upgrade evaluations perform an optimization for each relevant item. With your settings this might take a long time. Consider using lower thoroughness and no overriding of regem and reenchant options." + Environment.NewLine + Environment.NewLine + "Do you want to continue with upgrade evaluations?", "Optimizer Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    EnableAllReqs(); return;
                }
            }

            _optimizer.OptimizationMethod = Properties.Optimizer.Default.OptimizationMethod;
            _optimizer.GreedyOptimizationMethod = Properties.Optimizer.Default.GreedyOptimizationMethod;

            _optimizer.InitializeItemCache(_character, _character.AvailableItems, _overrideRegem, _overrideReenchant, Properties.Optimizer.Default.TemplateGemsEnabled, Calculations.Instance, checkBoxOptimizeFood.Checked, checkBoxOptimizeElixir.Checked, checkBoxMixology.Checked, GetOptimizeTalentSpecs(), checkBoxMutateTalents.Checked);
            if (Properties.Optimizer.Default.WarningsEnabled)
            {
                string prompt = _optimizer.GetWarningPromptIfNeeded();
                if (prompt != null)
                {
                    if (MessageBox.Show(prompt, "Optimizer Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) { EnableAllReqs(); return; }
                }
            }

			buttonUpgrades.Text = "Calculating...";
			buttonCancel.Text = "Cancel";
			buttonOptimize.Enabled = buttonUpgrades.Enabled = checkBoxOverrideRegem.Enabled = checkBoxOverrideReenchant.Enabled =
				trackBarThoroughness.Enabled = checkBoxMixology.Enabled = checkBoxOptimizeElixir.Enabled =
				checkBoxOptimizeFood.Enabled = checkBoxOptimizeTalents.Enabled = checkBoxMutateTalents.Enabled =
				comboBoxCalculationToOptimize.Enabled = false;
            buttonCancel.DialogResult = DialogResult.None;

            _optimizer.ComputeUpgradesAsync(_character, _calculationToOptimize, requirements, _thoroughness, _itemToEvaluate);
        }

        private void FormOptimize_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Optimizer.Default.OverrideRegem = checkBoxOverrideRegem.Checked;
            Properties.Optimizer.Default.OverrideReenchant = checkBoxOverrideReenchant.Checked;
            Properties.Optimizer.Default.Thoroughness = trackBarThoroughness.Value;
            Properties.Optimizer.Default.CalculationToOptimize = GetCalculationStringFromComboBox(comboBoxCalculationToOptimize, null);
            Properties.Optimizer.Default.Save();

            SaveOptimizationParameters();
        }

        private void SaveOptimizationParameters()
        {
            _character.CalculationToOptimize = GetCalculationStringFromComboBox(comboBoxCalculationToOptimize, null);
            List<OptimizationRequirement> requirements = new List<OptimizationRequirement>();
            foreach (Control ctrl in groupBoxRequirements.Controls)
            {
                Panel panel = ctrl as Panel;
                if (panel != null)
                {
                    OptimizationRequirement requirement = new OptimizationRequirement();
                    requirement.Calculation = GetCalculationStringFromComboBox(panel.Controls["comboBoxRequirementCalculation"] as ComboBox, panel.Controls["comboBoxRequirementTalent"] as ComboBox);
                    requirement.LessThan = (panel.Controls["comboBoxRequirementGreaterLessThan"] as ComboBox).SelectedIndex == 1;
                    requirement.Value = (float)((panel.Controls["numericUpDownRequirementValue"] as NumericUpDown).Value);
                    requirements.Add(requirement);
                }
            }
            _character.OptimizationRequirements = requirements;
        }
 
        private void progressBarMain_Click(object sender, EventArgs e)
        {

        }

        private void progressBarAlt_Click(object sender, EventArgs e)
        {

        }

        private void FormOptimize_Load(object sender, EventArgs e)
        {

        }
    }
}
