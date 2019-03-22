using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Rawr.Cat
{
	public partial class CalculationOptionsPanelCat : CalculationOptionsPanelBase
	{
		private Dictionary<int, string> armorBosses = new Dictionary<int, string>();

		public CalculationOptionsPanelCat()
		{
			InitializeComponent();
		}

		protected override void LoadCalculationOptions()
		{
			_loadingCalculationOptions = true;
			if (Character.CalculationOptions == null)
				Character.CalculationOptions = new CalculationOptionsCat();

			CalculationOptionsCat calcOpts = Character.CalculationOptions as CalculationOptionsCat;
			comboBoxTargetLevel.SelectedItem = calcOpts.TargetLevel.ToString();
			numericUpDownTargetArmor.Value = calcOpts.TargetArmor;
			comboBoxFerociousBite.SelectedItem = calcOpts.CustomCPFerociousBite.ToString();
			checkBoxRip.Checked = calcOpts.CustomUseRip;
			checkBoxRake.Checked = calcOpts.CustomUseRake;
			checkBoxShred.Checked = calcOpts.CustomUseShred;
			comboBoxSavageRoar.SelectedItem = calcOpts.CustomCPSavageRoar.ToString();
			trackBarTrinketOffset.Value = (int)(calcOpts.TrinketOffset * 2);
			numericUpDownDuration.Value = calcOpts.Duration;
			numericUpDownLagVariance.Value = calcOpts.LagVariance;

			labelTrinketOffset.Text = string.Format(labelTrinketOffset.Tag.ToString(), calcOpts.TrinketOffset);
			
			_loadingCalculationOptions = false;
		}

		private bool _loadingCalculationOptions = false;
		private void calculationOptionControl_Changed(object sender, EventArgs e)
		{
			if (!_loadingCalculationOptions)
			{
				CalculationOptionsCat calcOpts = Character.CalculationOptions as CalculationOptionsCat;
				calcOpts.TargetLevel = int.Parse(comboBoxTargetLevel.SelectedItem.ToString());
				calcOpts.TargetArmor = (int)numericUpDownTargetArmor.Value;
				calcOpts.CustomCPFerociousBite = int.Parse(comboBoxFerociousBite.SelectedItem.ToString());
				calcOpts.CustomUseRip = checkBoxRip.Checked;
				calcOpts.CustomUseRake = checkBoxRake.Checked;
				calcOpts.CustomUseShred = checkBoxShred.Checked;
				calcOpts.CustomCPSavageRoar = int.Parse(comboBoxSavageRoar.SelectedItem.ToString());
				calcOpts.TrinketOffset = (float)trackBarTrinketOffset.Value / 2f;
				calcOpts.Duration = (int)numericUpDownDuration.Value;
				calcOpts.LagVariance = (int)numericUpDownLagVariance.Value;

				labelTrinketOffset.Text = string.Format(labelTrinketOffset.Tag.ToString(), calcOpts.TrinketOffset);

				Character.OnCalculationsInvalidated();
			}
		}

	}
}
