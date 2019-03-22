using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Text;
using System.Windows.Forms;

namespace Rawr.Moonkin
{
    public partial class CalculationOptionsPanelMoonkin : CalculationOptionsPanelBase
    {
        public CalculationOptionsPanelMoonkin()
        {
            InitializeComponent();
        }

        // Load the options into the form
        protected override void LoadCalculationOptions()
        {
			if (Character.CalculationOptions == null)
				Character.CalculationOptions = new CalculationOptionsMoonkin();

			CalculationOptionsMoonkin calcOpts = Character.CalculationOptions as CalculationOptionsMoonkin;
            cmbTargetLevel.SelectedItem = calcOpts.TargetLevel.ToString();
			txtLatency.Text = calcOpts.Latency.ToString();
            txtFightLength.Text = calcOpts.FightLength.ToString();
            chkInnervate.Checked = calcOpts.Innervate;
            //chkManaPots.Checked = calcOpts.ManaPots;
            //cmbPotType.SelectedItem = calcOpts.ManaPotType;
            //cmbPotType.Enabled = chkManaPots.Checked;
            txtInnervateDelay.Text = calcOpts.InnervateDelay.ToString();
            txtInnervateDelay.Enabled = chkInnervate.Checked;
            trkReplenishmentUptime.Value = (int)(calcOpts.ReplenishmentUptime * 100);
            trkTreantLifespan.Value = (int)(calcOpts.TreantLifespan * 100);
            cmbUserRotation.SelectedItem = calcOpts.UserRotation;
            chkPtrMode.Checked = calcOpts.PTRMode;
        }

        private void cmbTargetLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
			CalculationOptionsMoonkin calcOpts = Character.CalculationOptions as CalculationOptionsMoonkin;
			calcOpts.TargetLevel = int.Parse(cmbTargetLevel.SelectedItem.ToString());
            Character.OnCalculationsInvalidated();
        }

        private void txtLatency_TextChanged(object sender, EventArgs e)
        {
			CalculationOptionsMoonkin calcOpts = Character.CalculationOptions as CalculationOptionsMoonkin;
			calcOpts.Latency = float.Parse(txtLatency.Text);
            Character.OnCalculationsInvalidated();
        }

        private void txtFightLength_Leave(object sender, EventArgs e)
        {
			CalculationOptionsMoonkin calcOpts = Character.CalculationOptions as CalculationOptionsMoonkin;
			calcOpts.FightLength = float.Parse(txtFightLength.Text);
            Character.OnCalculationsInvalidated();
        }

        private void chkInnervate_CheckedChanged(object sender, EventArgs e)
        {
			CalculationOptionsMoonkin calcOpts = Character.CalculationOptions as CalculationOptionsMoonkin;
			calcOpts.Innervate = chkInnervate.Checked;
            txtInnervateDelay.Enabled = chkInnervate.Checked;
            Character.OnCalculationsInvalidated();
        }

        /*private void chkManaPots_CheckedChanged(object sender, EventArgs e)
        {
			CalculationOptionsMoonkin calcOpts = Character.CalculationOptions as CalculationOptionsMoonkin;
			calcOpts.ManaPots = chkManaPots.Checked;
            cmbPotType.Enabled = chkManaPots.Checked;
            Character.OnCalculationsInvalidated();
        }

        private void cmbPotType_SelectedIndexChanged(object sender, EventArgs e)
        {
			CalculationOptionsMoonkin calcOpts = Character.CalculationOptions as CalculationOptionsMoonkin;
			calcOpts.ManaPotType = cmbPotType.SelectedItem.ToString();
            Character.OnCalculationsInvalidated();
        }*/

        private void txtInnervateDelay_Leave(object sender, EventArgs e)
        {
			CalculationOptionsMoonkin calcOpts = Character.CalculationOptions as CalculationOptionsMoonkin;
			calcOpts.InnervateDelay = float.Parse(txtInnervateDelay.Text);
            Character.OnCalculationsInvalidated();
        }

        private void trkReplenishmentUptime_ValueChanged(object sender, EventArgs e)
        {
            CalculationOptionsMoonkin calcOpts = Character.CalculationOptions as CalculationOptionsMoonkin;
            calcOpts.ReplenishmentUptime = trkReplenishmentUptime.Value / 100.0f;
            lblUptimeValue.Text = trkReplenishmentUptime.Value.ToString();
            Character.OnCalculationsInvalidated();
        }

        private void trkTreantLifespan_ValueChanged(object sender, EventArgs e)
        {
            CalculationOptionsMoonkin calcOpts = Character.CalculationOptions as CalculationOptionsMoonkin;
            calcOpts.TreantLifespan = trkTreantLifespan.Value / 100.0f;
            lblLifespanValue.Text = trkTreantLifespan.Value.ToString();
            Character.OnCalculationsInvalidated();
        }

        private void cmbUserRotation_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculationOptionsMoonkin calcOpts = Character.CalculationOptions as CalculationOptionsMoonkin;
            calcOpts.UserRotation = cmbUserRotation.SelectedItem.ToString();
            Character.OnCalculationsInvalidated();
        }

        private void chkPtrMode_CheckedChanged(object sender, EventArgs e)
        {
            CalculationOptionsMoonkin calcOpts = Character.CalculationOptions as CalculationOptionsMoonkin;
            calcOpts.PTRMode = chkPtrMode.Checked;
            Character.OnCalculationsInvalidated();
        }
    }
}
