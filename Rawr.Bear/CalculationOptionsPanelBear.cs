using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Rawr.Bear
{
	public partial class CalculationOptionsPanelBear : CalculationOptionsPanelBase
	{
		public CalculationOptionsPanelBear()
		{
			InitializeComponent();
		}

		protected override void LoadCalculationOptions()
		{
			_loadingCalculationOptions = true;
			if (Character.CalculationOptions == null)
				Character.CalculationOptions = new CalculationOptionsBear();
			//if (!Character.CalculationOptions.ContainsKey("TargetLevel"))
			//    Character.CalculationOptions["TargetLevel"] = "73";
			//if (!Character.CalculationOptions.ContainsKey("ThreatScale"))
			//    Character.CalculationOptions["ThreatScale"] = "1";
			//if (!Character.CalculationOptions.ContainsKey("EnforceMetagemRequirements"))
			//    Character.CalculationOptions["EnforceMetagemRequirements"] = "No";

			CalculationOptionsBear calcOpts = Character.CalculationOptions as CalculationOptionsBear;
			comboBoxTargetLevel.SelectedItem = calcOpts.TargetLevel.ToString();
            numericUpDownThreatValue.Value = (decimal)calcOpts.ThreatScale;
			numericUpDownTargetArmor.Value = (decimal)calcOpts.TargetArmor;
			numericUpDownSurvivalSoftCap.Value = calcOpts.SurvivalSoftCap;
			numericUpDownTargetDamage.Value = calcOpts.TargetDamage;
			numericUpDownTargetAttackSpeed.Value = (decimal)calcOpts.TargetAttackSpeed;
			trackBarTemporarySurvivalEffectValue.Value = (int)(100f * calcOpts.TemporarySurvivalScale);

			checkBoxMaul.Checked = calcOpts.CustomUseMaul;
            checkBoxMangle.Checked = calcOpts.CustomUseMangle;
            checkBoxSwipe.Checked = calcOpts.CustomUseSwipe;
			checkBoxFaerieFire.Checked = calcOpts.CustomUseFaerieFire;
			checkBoxLacerate.Checked = calcOpts.CustomUseLacerate;
			checkBoxTargetParryHastes.Checked = calcOpts.TargetParryHastes;
			
			switch (numericUpDownThreatValue.Value.ToString())
			{
				case "0": comboBoxThreatValue.SelectedIndex = 0; break;
				case "10": comboBoxThreatValue.SelectedIndex = 1; break; 
				case "50": comboBoxThreatValue.SelectedIndex = 2; break; 
				case "100": comboBoxThreatValue.SelectedIndex = 3; break; 
				default: comboBoxThreatValue.SelectedIndex = 4; break;
			}

			switch (numericUpDownTargetDamage.Value.ToString())
			{
				case "30000": comboBoxTargetDamage.SelectedIndex = 0; break; //Normal Dungeons
				case "37000": comboBoxTargetDamage.SelectedIndex = 1; break; //Heroic Dungeons
				case "40000": comboBoxTargetDamage.SelectedIndex = 2; break; //T7 Raids (10)
				case "47000": comboBoxTargetDamage.SelectedIndex = 3; break; //T7 Raids (25)
				case "55000": comboBoxTargetDamage.SelectedIndex = 4; break; //T8 Raids (10)
				case "75000": comboBoxTargetDamage.SelectedIndex = 5; break; //T8 Raids (10, Hard)
				case "71000": comboBoxTargetDamage.SelectedIndex = 6; break; //T8 Raids (25)
				case "90000": comboBoxTargetDamage.SelectedIndex = 7; break; //T8 Raids (25, Hard)
				case "70000": comboBoxTargetDamage.SelectedIndex = 8; break; //T9 Raids (10)
				case "85000": comboBoxTargetDamage.SelectedIndex = 9; break; //T9 Raids (10, Heroic)
				case "80000": comboBoxTargetDamage.SelectedIndex = 10; break; //T9 Raids (25)
				case "95000": comboBoxTargetDamage.SelectedIndex = 11; break; //T9 Raids (25, Heroic)

				case "92000": comboBoxTargetDamage.SelectedIndex = 12; break; //T10 Raids (10)
				case "120000": comboBoxTargetDamage.SelectedIndex = 13; break; //T10 Raids (10, Heroic)
				case "100000": comboBoxTargetDamage.SelectedIndex = 14; break; //T10 Raids (25)
				case "150000": comboBoxTargetDamage.SelectedIndex = 15; break; //T10 Raids (25, Heroic)
				case "105000": comboBoxTargetDamage.SelectedIndex = 16; break; //Lich King (10)
				case "160000": comboBoxTargetDamage.SelectedIndex = 17; break; //Lich King (10, Heroic)
				case "155000": comboBoxTargetDamage.SelectedIndex = 18; break; //Lich King (25)
				case "200000": comboBoxTargetDamage.SelectedIndex = 19; break; //Lich King (25, Heroic)
				default: comboBoxTargetDamage.SelectedIndex = 20; break; //Custom...
			}

			switch (numericUpDownSurvivalSoftCap.Value.ToString())
			{
				case "90000": comboBoxSurvivalSoftCap.SelectedIndex = 0; break; //Normal Dungeons
				case "110000": comboBoxSurvivalSoftCap.SelectedIndex = 1; break; //Heroic Dungeons
				case "120000": comboBoxSurvivalSoftCap.SelectedIndex = 2; break; //T7 Raids (10)
				case "140000": comboBoxSurvivalSoftCap.SelectedIndex = 3; break; //T7 Raids (25)
				case "170000": comboBoxSurvivalSoftCap.SelectedIndex = 4; break; //T8 Raids (10)
				case "195000": comboBoxSurvivalSoftCap.SelectedIndex = 5; break; //T8 Raids (10, Hard)
				case "185000": comboBoxSurvivalSoftCap.SelectedIndex = 6; break; //T8 Raids (25)
				case "215000": comboBoxSurvivalSoftCap.SelectedIndex = 7; break; //T8 Raids (25, Hard)
				case "180000": comboBoxSurvivalSoftCap.SelectedIndex = 8; break; //T9 Raids (10)
				case "210000": comboBoxSurvivalSoftCap.SelectedIndex = 9; break; //T9 Raids (10, Heroic)
				case "190000": comboBoxSurvivalSoftCap.SelectedIndex = 10; break; //T9 Raids (25)
				case "225000": comboBoxSurvivalSoftCap.SelectedIndex = 11; break; //T9 Raids (25, Heroic)

				case "300000": comboBoxSurvivalSoftCap.SelectedIndex = 12; break; //T10 Raids (10)
				case "355000": comboBoxSurvivalSoftCap.SelectedIndex = 13; break; //T10 Raids (10, Heroic)
				case "350000": comboBoxSurvivalSoftCap.SelectedIndex = 14; break; //T10 Raids (25)
				case "400000": comboBoxSurvivalSoftCap.SelectedIndex = 15; break; //T10 Raids (25, Heroic)
				case "360000": comboBoxSurvivalSoftCap.SelectedIndex = 16; break; //Lich King (10)
				case "410000": comboBoxSurvivalSoftCap.SelectedIndex = 17; break; //Lich King (10, Heroic)
				case "405000": comboBoxSurvivalSoftCap.SelectedIndex = 18; break; //Lich King (25)
				case "500000": comboBoxSurvivalSoftCap.SelectedIndex = 19; break; //Lich King (25, Heroic)
				default: comboBoxSurvivalSoftCap.SelectedIndex = 20; break;
			}

			_loadingCalculationOptions = false;
		}
	
		private bool _loadingCalculationOptions = false;
		private void calculationOptionControl_Changed(object sender, EventArgs e)
		{
			if (!_loadingCalculationOptions)
			{
				CalculationOptionsBear calcOpts = Character.CalculationOptions as CalculationOptionsBear;
				calcOpts.TargetLevel = int.Parse(comboBoxTargetLevel.SelectedItem.ToString());
				calcOpts.ThreatScale = (float)numericUpDownThreatValue.Value;
				calcOpts.TemporarySurvivalScale = (float)trackBarTemporarySurvivalEffectValue.Value / 100f;
				calcOpts.TargetArmor = (int)numericUpDownTargetArmor.Value;
				calcOpts.SurvivalSoftCap = (int)numericUpDownSurvivalSoftCap.Value;
				calcOpts.TargetDamage = (int)numericUpDownTargetDamage.Value;
				calcOpts.TargetAttackSpeed = (float)numericUpDownTargetAttackSpeed.Value;

                calcOpts.CustomUseMaul = checkBoxMaul.Checked;
                calcOpts.CustomUseMangle = checkBoxMangle.Checked;
                calcOpts.CustomUseSwipe = checkBoxSwipe.Checked;
				calcOpts.CustomUseFaerieFire = checkBoxFaerieFire.Checked;
				calcOpts.CustomUseLacerate = checkBoxLacerate.Checked;
				calcOpts.TargetParryHastes = checkBoxTargetParryHastes.Checked;

				Character.OnCalculationsInvalidated();
			}
        }
		private void comboBoxThreatValue_SelectedIndexChanged(object sender, EventArgs e)
        {
			numericUpDownThreatValue.Enabled = comboBoxThreatValue.SelectedIndex == 4;
			if (comboBoxThreatValue.SelectedIndex < 4)
				numericUpDownThreatValue.Value = (new decimal[] { 0.0001M, 10, 50, 100 })[comboBoxThreatValue.SelectedIndex];
		}

		private void comboBoxTargetDamage_SelectedIndexChanged(object sender, EventArgs e)
		{
			numericUpDownTargetDamage.Enabled = comboBoxTargetDamage.SelectedIndex == 20;
			if (comboBoxTargetDamage.SelectedIndex < 20)
				numericUpDownTargetDamage.Value =
					(new decimal[] { 30000, 37000, 40000, 47000, 55000, 75000, 71000, 90000, 70000, 85000, 80000, 95000, 92000, 120000, 100000, 150000, 105000, 160000, 155000, 200000 })
					[comboBoxTargetDamage.SelectedIndex];
		}

		private void comboBoxSurvivalSoftCap_SelectedIndexChanged(object sender, EventArgs e)
		{
			numericUpDownSurvivalSoftCap.Enabled = comboBoxSurvivalSoftCap.SelectedIndex == 20;
			if (comboBoxSurvivalSoftCap.SelectedIndex < 20)
				numericUpDownSurvivalSoftCap.Value =
					(new decimal[] { 90000, 110000, 120000, 140000, 170000, 195000, 185000, 215000, 180000, 210000, 190000, 225000, 300000, 355000, 350000, 400000, 360000, 410000, 405000, 500000 })
					[comboBoxSurvivalSoftCap.SelectedIndex];
		}
	}
}
