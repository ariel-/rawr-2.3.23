using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Rawr.CustomControls;

namespace Rawr.ProtWarr {
	public partial class CalculationOptionsPanelProtWarr : CalculationOptionsPanelBase {
		private Dictionary<int, string> armorBosses = new Dictionary<int, string>();

		public CalculationOptionsPanelProtWarr() {
			InitializeComponent();
            armorBosses.Add((int)StatConversion.NPC_ARMOR[80 - 80], ": Level 80 Mobs");
            armorBosses.Add((int)StatConversion.NPC_ARMOR[81 - 80], ": Level 81 Mobs");
            armorBosses.Add((int)StatConversion.NPC_ARMOR[82 - 80], ": Level 82 Mobs");
            armorBosses.Add((int)StatConversion.NPC_ARMOR[83 - 80], ": Ulduar Bosses");
        }

		protected override void LoadCalculationOptions() {
			_loadingCalculationOptions = true;
            if (Character.CalculationOptions == null) { Character.CalculationOptions = new CalculationOptionsProtWarr(); }

			CalculationOptionsProtWarr calcOpts = Character.CalculationOptions as CalculationOptionsProtWarr;

            // Attacker Stats
			CB_TargetLevel.Text = calcOpts.TargetLevel.ToString();
            CB_TargetArmor.Text = (CB_TargetArmor.Items.Contains(calcOpts.TargetArmor.ToString()) ? calcOpts.TargetArmor.ToString() : CB_TargetArmor.Items[0].ToString());
            Bar_BossAttackValue.Value = calcOpts.BossAttackValue;
            Bar_BossAttackSpeed.Value = (int)(calcOpts.BossAttackSpeed / 0.25f);
            CB_UseParryHaste.Checked = calcOpts.UseParryHaste;
            // Stupid hack since you can't put in newlines into the VS editor properties
            LB_UseParryHaste.ToolTipText = LB_UseParryHaste.ToolTipText.Replace("May not", Environment.NewLine + "May not");

            // Fix for Saves With TankPoints Still Enabled
            if (calcOpts.RankingMode == 2)
                calcOpts.RankingMode = 1;

            // Ranking System
            if (calcOpts.ThreatScale > 24.0f) { calcOpts.ThreatScale = 8f; }// Old scale value being saved, reset to default
            if (calcOpts.MitigationScale > 1.0f) { calcOpts.MitigationScale = (1f / 8f); }// Old scale value being saved, reset to default
            Bar_ThreatScale.Value = Convert.ToInt32(calcOpts.ThreatScale / 8f / 0.1f);
            Bar_MitigationScale.Value = Convert.ToInt32((calcOpts.MitigationScale * 8.0f / 0.1f));
            RB_MitigationScale.Checked = (calcOpts.RankingMode == 1);
            //RB_TankPoints.Checked = (calcOpts.RankingMode == 2);
            RB_BurstTime.Checked = (calcOpts.RankingMode == 3);
            RB_DamageOutput.Checked = (calcOpts.RankingMode == 4);
            Bar_ThreatScale.Enabled = LB_ThreatScale.Enabled = (calcOpts.RankingMode != 4);
            Bar_MitigationScale.Enabled = LB_MitigationScaleValue.Enabled = (calcOpts.RankingMode == 1);

            // Warrior Abilities
            CB_UseVigilance.Checked = calcOpts.UseVigilance;
            Bar_VigilanceValue.Enabled = calcOpts.UseVigilance;
            Bar_VigilanceValue.Value = (int)calcOpts.VigilanceValue;
            Bar_HSFrequency.Value = (int)Math.Round(calcOpts.HeroicStrikeFrequency * 100.0f, 0);

            // Label Text
            LB_BossAttackValue.Text = Bar_BossAttackValue.Value.ToString();
            LB_BossAttackSpeedValue.Text = String.Format("{0:0.00}s", ((float)(Bar_BossAttackSpeed.Value) * 0.25f));
            LB_ThreatScale.Text = String.Format("{0:0.0}", ((float)(Bar_ThreatScale.Value) * 0.1f));
            LB_MitigationScaleValue.Text = String.Format("{0:0.0}", ((float)(Bar_MitigationScale.Value) * 0.1f));
            LB_VigilanceValue.Text = Bar_VigilanceValue.Value.ToString();
            LB_HSFrequencyValue.Text = Bar_HSFrequency.Value.ToString() + "%";

			_loadingCalculationOptions = false;
		}

		private bool _loadingCalculationOptions = false;

		private void calculationOptionControl_Changed(object sender, EventArgs e) {
			if (!_loadingCalculationOptions) {
				CalculationOptionsProtWarr calcOpts = Character.CalculationOptions as CalculationOptionsProtWarr;
				// Attacker Stats
                LB_BossAttackValue.Text = Bar_BossAttackValue.Value.ToString();
                LB_BossAttackSpeedValue.Text = String.Format("{0:0.00}s", ((float)(Bar_BossAttackSpeed.Value) * 0.25f));
				// Ranking System
                LB_ThreatScale.Text = String.Format("{0:0.0}", ((float)(Bar_ThreatScale.Value) * 0.1f));
				LB_MitigationScaleValue.Text = String.Format("{0:0.0}", ((float)(Bar_MitigationScale.Value) * 0.1f));
                // Warrior Abilities
                LB_VigilanceValue.Text = Bar_VigilanceValue.Value.ToString();
                LB_HSFrequencyValue.Text = Bar_HSFrequency.Value.ToString() + "%";

				calcOpts.TargetLevel = int.Parse(CB_TargetLevel.Text.ToString());
                calcOpts.TargetArmor = int.Parse(CB_TargetArmor.Text);
                calcOpts.BossAttackValue = Bar_BossAttackValue.Value;
                calcOpts.BossAttackSpeed = ((float)(Bar_BossAttackSpeed.Value) * 0.25f);
                calcOpts.ThreatScale = ((float)(Bar_ThreatScale.Value) * 0.1f * 8.0f);
                calcOpts.MitigationScale = ((float)(Bar_MitigationScale.Value) * 0.1f / 8.0f);
                calcOpts.VigilanceValue = Bar_VigilanceValue.Value;
                calcOpts.HeroicStrikeFrequency = ((float)Bar_HSFrequency.Value / 100.0f);

				Character.OnCalculationsInvalidated();
			}
		}

        private void checkBoxUseParryHaste_CheckedChanged(object sender, EventArgs e) {
            if (!_loadingCalculationOptions) {
                CalculationOptionsProtWarr calcOpts = Character.CalculationOptions as CalculationOptionsProtWarr;
                calcOpts.UseParryHaste = CB_UseParryHaste.Checked;
                Character.OnCalculationsInvalidated();
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e) {
            if (!_loadingCalculationOptions) {
                CalculationOptionsProtWarr calcOpts = Character.CalculationOptions as CalculationOptionsProtWarr;
                //if      (RB_TankPoints.Checked  ) { calcOpts.RankingMode = 2; Bar_ThreatScale.Value = 10; }
                if (RB_BurstTime.Checked   )      { calcOpts.RankingMode = 3; Bar_ThreatScale.Value =  0; }
                else if (RB_DamageOutput.Checked) { calcOpts.RankingMode = 4; Bar_ThreatScale.Value = 10; }
                else                              { calcOpts.RankingMode = 1; Bar_ThreatScale.Value = 10; }
                Bar_ThreatScale.Enabled = LB_ThreatScale.Enabled = (calcOpts.RankingMode != 4);
                Bar_MitigationScale.Enabled = LB_MitigationScaleValue.Enabled = (calcOpts.RankingMode == 1);

                Character.OnCalculationsInvalidated();
            }
        }

        private void checkBoxUseVigilance_CheckedChanged(object sender, EventArgs e) {
            if (!_loadingCalculationOptions) {
                CalculationOptionsProtWarr calcOpts = Character.CalculationOptions as CalculationOptionsProtWarr;
                calcOpts.UseVigilance = CB_UseVigilance.Checked;
                Bar_VigilanceValue.Enabled = CB_UseVigilance.Checked;
                Character.OnCalculationsInvalidated();
            }
        }

        private void extendedToolTipMitigtionScale_Click(object sender, EventArgs e) {
            if (!RB_MitigationScale.Checked)
                RB_MitigationScale.Checked = true;
        }

        //private void extendedToolTipTankPoints_Click(object sender, EventArgs e) {
        //    if (!RB_TankPoints.Checked)
        //        RB_TankPoints.Checked = true;
        //}

        private void extendedToolTipBurstTime_Click(object sender, EventArgs e) {
            if (!RB_BurstTime.Checked)
                RB_BurstTime.Checked = true;
        }

        private void extendedToolTipDamageOutput_Click(object sender, EventArgs e) {
            if (!RB_DamageOutput.Checked)
                RB_DamageOutput.Checked = true;
        }
	}
}
