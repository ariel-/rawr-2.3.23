using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Rawr.Rogue
{
    public partial class CalculationOptionsPanelRogue : CalculationOptionsPanelBase
    {
        private readonly Dictionary<int, string> _armorBosses = new Dictionary<int, string>();
        private static readonly CalculationOptionsRogue _calcOpts = new CalculationOptionsRogue();

        public CalculationOptionsPanelRogue()
        {
            InitializeComponent();
        }

        protected override void LoadCalculationOptions()
        {
            _loadingCalculationOptions = true;
            if (Character.CalculationOptions == null)
                Character.CalculationOptions = new CalculationOptionsRogue();

            CalculationOptionsRogue calcOpts = Character.CalculationOptions as CalculationOptionsRogue;
            comboBoxTargetLevel.SelectedItem = calcOpts.TargetLevel.ToString();
            numericUpDownTargetArmor.Value = calcOpts.TargetArmor;
            checkBoxPoisonable.Checked = calcOpts.TargetPoisonable;
            checkBoxBleed.Checked = calcOpts.BleedIsUp;
            numericUpDownDuration.Value = calcOpts.Duration;
            numericUpDownLagVariance.Value = calcOpts.LagVariance;
            checkBoxRupt.Checked = calcOpts.CustomUseRupt;
            checkBoxExpose.Checked = calcOpts.CustomUseExpose;
            checkBoxTotT.Checked = calcOpts.CustomUseTotT;
            comboBoxCPG.SelectedIndex = calcOpts.CustomCPG;
            comboBoxSnD.SelectedItem = calcOpts.CustomCPSnD.ToString();
            comboBoxFinisher.SelectedIndex = calcOpts.CustomFinisher;
            comboBoxCPFinisher.SelectedItem = calcOpts.CustomCPFinisher.ToString();
            comboBoxMHPoison.SelectedIndex = calcOpts.CustomMHPoison;
            comboBoxOHPoison.SelectedIndex = calcOpts.CustomOHPoison;
            trackBarTrinketOffset.Value = (int)(calcOpts.TrinketOffset * 2);

            labelTrinketOffset.Text = string.Format(labelTrinketOffset.Tag.ToString(), calcOpts.TrinketOffset);

            _loadingCalculationOptions = false;
        }

        private void UpdateCalculations()
        {
            if (Character != null && Character.CalculationOptions != null)
            {
                Character.CalculationOptions = _calcOpts;
                Character.OnCalculationsInvalidated();
            }
        }

        private bool _loadingCalculationOptions = false;
        private void calculationOptionControl_Changed(object sender, EventArgs e)
        {
            if (!_loadingCalculationOptions)
            {
                CalculationOptionsRogue calcOpts = Character.CalculationOptions as CalculationOptionsRogue;
                calcOpts.TargetLevel = int.Parse(comboBoxTargetLevel.SelectedItem.ToString());
                calcOpts.TargetArmor = (int)numericUpDownTargetArmor.Value;
                calcOpts.TargetPoisonable = checkBoxPoisonable.Checked;
                calcOpts.BleedIsUp = checkBoxBleed.Checked;
                calcOpts.Duration = (int)numericUpDownDuration.Value;
                calcOpts.LagVariance = (int)numericUpDownLagVariance.Value;
                calcOpts.ForceCustom = checkBoxCustom.Checked;
                calcOpts.CustomUseRupt = checkBoxRupt.Checked;
                calcOpts.CustomUseExpose = checkBoxExpose.Checked;
                calcOpts.CustomUseTotT = checkBoxTotT.Checked;
                calcOpts.CustomCPG = comboBoxCPG.SelectedIndex;
                calcOpts.CustomCPSnD = int.Parse(comboBoxSnD.SelectedItem.ToString());
                calcOpts.CustomFinisher = comboBoxFinisher.SelectedIndex;
                calcOpts.CustomCPFinisher = int.Parse(comboBoxCPFinisher.SelectedItem.ToString());
                calcOpts.CustomMHPoison = comboBoxMHPoison.SelectedIndex;
                calcOpts.CustomOHPoison = comboBoxOHPoison.SelectedIndex;
                calcOpts.TrinketOffset = (float)trackBarTrinketOffset.Value / 2f;
                calcOpts.PTRMode = checkBoxPTR.Checked;

                labelTrinketOffset.Text = string.Format(labelTrinketOffset.Tag.ToString(), calcOpts.TrinketOffset);

                Character.OnCalculationsInvalidated();
            }
        }
    }
}