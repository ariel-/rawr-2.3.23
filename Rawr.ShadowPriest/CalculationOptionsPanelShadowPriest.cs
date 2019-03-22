using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Rawr.ShadowPriest
{
    public partial class CalculationOptionsPanelShadowPriest : CalculationOptionsPanelBase
    {
        public CalculationOptionsPanelShadowPriest()
        {
            InitializeComponent();
        }

        private bool loading;

        protected override void LoadCalculationOptions()
        {
            loading = true;
            if (Character.CalculationOptions == null)
                Character.CalculationOptions = new CalculationOptionsShadowPriest();

            CalculationOptionsShadowPriest calcOpts = Character.CalculationOptions as CalculationOptionsShadowPriest;
            
            cbTargetLevel.SelectedIndex = calcOpts.TargetLevel;
            
            trkFightLength.Value = (int)calcOpts.FightLength;
            lblFightLength.Text = trkFightLength.Value + " minute fight.";

            trkFSR.Value = (int)calcOpts.FSRRatio;
            lblFSR.Text = trkFSR.Value + "% time spent in FSR";

            trkDelay.Value = (int)calcOpts.Delay;
            lblDelay.Text = trkDelay.Value + "ms Game/Brain Latency";

            trkShadowfiend.Value = (int)calcOpts.Shadowfiend;
            lblShadowfiend.Text = trkShadowfiend.Value + "% effect from Shadowfiend.";

            trkReplenishment.Value = (int)calcOpts.Replenishment;
            lblReplenishment.Text = trkReplenishment.Value + "% effect from Replenishment.";

            trkJoW.Value = (int)calcOpts.JoW;
            lblJoW.Text = trkJoW.Value + "% effect from JoW.";

            trkSurvivability.Value = (int)calcOpts.Survivability;
            lblSurvivability.Text = trkSurvivability.Value + "% Focus on Survivability.";

            trkMoveFrequency.Value = (int)calcOpts.MoveFrequency;
            lblMoveFrequency.Text = trkMoveFrequency.Value + " seconds between each need for movement.";

            trkMoveDuration.Value = (int)calcOpts.MoveDuration;
            lblMoveDuration.Text = trkMoveDuration.Value + " seconds of movement each time.";

            cbPtr.Checked = calcOpts.PTR;
            
            cmbManaAmt.SelectedIndex = calcOpts.ManaPot;

            if (calcOpts.SpellPriority == null)
                calcOpts.SpellPriority = new List<string>(Spell.ShadowSpellList);
            lsSpellPriopity.Items.Clear();
            lsSpellPriopity.Items.AddRange(calcOpts.SpellPriority.ToArray());
            
            loading = false;
        }

        private void cmbManaAmt_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsShadowPriest calcOpts = Character.CalculationOptions as CalculationOptionsShadowPriest;
                calcOpts.ManaPot = cmbManaAmt.SelectedIndex;
                Character.OnCalculationsInvalidated();
            }
        }
               
        private void bChangePriority_Click(object sender, EventArgs e)
        {
            CalculationOptionsShadowPriest calcOpts = Character.CalculationOptions as CalculationOptionsShadowPriest;
            SpellPriorityForm priority = new SpellPriorityForm(calcOpts.SpellPriority, lsSpellPriopity, Character);
            priority.Show();
        }

        private void cbTargetLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsShadowPriest calcOpts = Character.CalculationOptions as CalculationOptionsShadowPriest;
                calcOpts.TargetLevel = cbTargetLevel.SelectedIndex;
                Character.OnCalculationsInvalidated();
            }
        }

        private void trkFightLength_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsShadowPriest calcOpts = Character.CalculationOptions as CalculationOptionsShadowPriest;
                calcOpts.FightLength = trkFightLength.Value;
                lblFightLength.Text = trkFightLength.Value + " minute fight.";
                Character.OnCalculationsInvalidated();
            }
        }

        private void trkFSR_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsShadowPriest calcOpts = Character.CalculationOptions as CalculationOptionsShadowPriest;
                calcOpts.FSRRatio = trkFSR.Value;
                lblFSR.Text = trkFSR.Value + "% time spent in FSR";
                Character.OnCalculationsInvalidated();
            }
        }

        private void trkDelay_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsShadowPriest calcOpts = Character.CalculationOptions as CalculationOptionsShadowPriest;
                calcOpts.Delay = trkDelay.Value;
                lblDelay.Text = trkDelay.Value + "ms Game/Brain Latency";
                Character.OnCalculationsInvalidated();
            }
        }

        private void trkShadowfiend_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsShadowPriest calcOpts = Character.CalculationOptions as CalculationOptionsShadowPriest;
                calcOpts.Shadowfiend = trkShadowfiend.Value;
                lblShadowfiend.Text = trkShadowfiend.Value + "% effect from Shadowfiend.";
                Character.OnCalculationsInvalidated();
            }
        }

        private void trkReplenishment_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsShadowPriest calcOpts = Character.CalculationOptions as CalculationOptionsShadowPriest;
                calcOpts.Replenishment = trkReplenishment.Value;
                lblReplenishment.Text = trkReplenishment.Value + "% effect from Replenishment.";
                Character.OnCalculationsInvalidated();
            }
        }

        private void trkJoW_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsShadowPriest calcOpts = Character.CalculationOptions as CalculationOptionsShadowPriest;
                calcOpts.JoW = trkJoW.Value;
                lblJoW.Text = trkJoW.Value + "% effect from JoW.";
                Character.OnCalculationsInvalidated();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsShadowPriest calcOpts = Character.CalculationOptions as CalculationOptionsShadowPriest;
                calcOpts.Survivability = trkSurvivability.Value;
                lblSurvivability.Text = trkSurvivability.Value + "% Focus on Survivability.";
                Character.OnCalculationsInvalidated();
            }
        }

        private void trkMoveFrequency_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsShadowPriest calcOpts = Character.CalculationOptions as CalculationOptionsShadowPriest;
                calcOpts.MoveFrequency = trkMoveFrequency.Value;
                lblMoveFrequency.Text = trkMoveFrequency.Value + " seconds between each need for Movement.";
                Character.OnCalculationsInvalidated();
            }
        }

        private void trkMoveDuration_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsShadowPriest calcOpts = Character.CalculationOptions as CalculationOptionsShadowPriest;
                calcOpts.MoveDuration = trkMoveDuration.Value;
                lblMoveDuration.Text = trkMoveDuration.Value + " seconds of movement each time.";
                Character.OnCalculationsInvalidated();
            }
        }

        private void cbPtr_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsShadowPriest calcOpts = Character.CalculationOptions as CalculationOptionsShadowPriest;
                calcOpts.PTR = cbPtr.Checked;
                Character.OnCalculationsInvalidated();
            }
        }
    }
}
