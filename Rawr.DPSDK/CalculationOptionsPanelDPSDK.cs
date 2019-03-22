using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr.DPSDK
{
    public partial class CalculationOptionsPanelDPSDK : CalculationOptionsPanelBase
    {

        public CalculationOptionsPanelDPSDK()
        {
            InitializeComponent();
        }
        protected override void LoadCalculationOptions()
        {
			if (Character.CalculationOptions == null)
				Character.CalculationOptions = new CalculationOptionsDPSDK();

			CalculationOptionsDPSDK calcOpts = Character.CalculationOptions as CalculationOptionsDPSDK;

            if (calcOpts.rotation == null)
                calcOpts.rotation = new Rotation();
           

            cbTargetLevel.SelectedItem = calcOpts.TargetLevel.ToString();
            nudFightLength.Value = (int)(calcOpts.FightLength * 60);
            cbDisplayCalcs.Checked = calcOpts.GetRefreshForDisplayCalcs;
            cbGhoul.Checked = calcOpts.Ghoul;
            cbRefCalcs.Checked = calcOpts.GetRefreshForReferenceCalcs;
            cbSignificantChange.Checked = calcOpts.GetRefreshForSignificantChange;
            cbExperimental.Checked = calcOpts.m_bExperimental;

            nudTargetArmor.Value = calcOpts.BossArmor;
        }

        private void cbTargetLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculationOptionsDPSDK calcOpts = Character.CalculationOptions as CalculationOptionsDPSDK;
            calcOpts.TargetLevel = int.Parse(cbTargetLevel.SelectedItem.ToString());
            Character.OnCalculationsInvalidated();
        }

        private void rbUnholyPresence_CheckedChanged(object sender, EventArgs e)
        {
            if (rbUnholyPresence.Checked)
            {
                CalculationOptionsDPSDK calcOpts = Character.CalculationOptions as CalculationOptionsDPSDK;
                calcOpts.CurrentPresence = CalculationOptionsDPSDK.Presence.Unholy;
                Character.OnCalculationsInvalidated();
            }
        }
        private void rbBloodPresence_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBloodPresence.Checked)
            {
                CalculationOptionsDPSDK calcOpts = Character.CalculationOptions as CalculationOptionsDPSDK;
                calcOpts.CurrentPresence = CalculationOptionsDPSDK.Presence.Blood;
                Character.OnCalculationsInvalidated();
            }
        }

        #region StatsGraph
        private Stats[] BuildStatsList()
        {
            List<Stats> statsList = new List<Stats>();
            statsList.Add(new Stats() { Strength = 1f });
//            statsList.Add(new Stats() { Stamina = 1f });
            statsList.Add(new Stats() { Agility = 1f });
            statsList.Add(new Stats() { AttackPower = 2f });
            statsList.Add(new Stats() { CritRating = 1f });
            statsList.Add(new Stats() { HitRating = 1f });
            statsList.Add(new Stats() { ExpertiseRating = 1f });
            statsList.Add(new Stats() { HasteRating = 1f });
            statsList.Add(new Stats() { ArmorPenetrationRating = 1f });
//            statsList.Add(new Stats() { DefenseRating = 1f });
//            statsList.Add(new Stats() { DodgeRating = 1f });
//            statsList.Add(new Stats() { ParryRating = 1f });
            return statsList.ToArray();
        }

        private void btnGraph_Click(object sender, EventArgs e)
        {
            Stats[] statsList = BuildStatsList();
            Rawr.Base.Graph graph = new Rawr.Base.Graph();
            string explanatoryText = "This graph shows how adding or subtracting\nmultiples of a stat affects your Overall Score.\n\nAt the Zero position is your current Overall.\n" +
                         "To the right of the zero vertical is adding stats.\nTo the left of the zero vertical is subtracting stats.\n" +
                         "The vertical axis shows the amount of Overall points added or lost";
            graph.SetupStatsGraph(Character, statsList, 1000, explanatoryText, null);
            graph.Show();
        }
        #endregion
        
        private void nudTargetArmor_ValueChanged(object sender, EventArgs e)
        {
            CalculationOptionsDPSDK calcOpts = Character.CalculationOptions as CalculationOptionsDPSDK;
            calcOpts.BossArmor = (int)nudTargetArmor.Value;
            Character.OnCalculationsInvalidated();
        }
        private void nudFightLength_ValueChanged(object sender, EventArgs e)
        {
            CalculationOptionsDPSDK calcOpts = Character.CalculationOptions as CalculationOptionsDPSDK;
            calcOpts.FightLength = (float)(nudFightLength.Value) / 60f;
            Character.OnCalculationsInvalidated();
        }

        private void cbGhoul_CheckedChanged(object sender, EventArgs e)
        {
            CalculationOptionsDPSDK calcOpts = Character.CalculationOptions as CalculationOptionsDPSDK;
            calcOpts.Ghoul = cbGhoul.Checked;
            Character.OnCalculationsInvalidated();
        }
        
        private void btnRotation_Click(object sender, EventArgs e)
        {
            CalculationOptionsDPSDK calcOpts = Character.CalculationOptions as CalculationOptionsDPSDK;
            RotationViewer RV = new RotationViewer(calcOpts, Character);
            RV.ShowDialog();            
            Character.OnCalculationsInvalidated();
        }

        private void BloodwormUptime_Scroll(object sender, EventArgs e)
        {
            CalculationOptionsDPSDK calcOpts = Character.CalculationOptions as CalculationOptionsDPSDK;
            calcOpts.BloodwormsUptime = BloodwormUptime.Value / 100f;
            lbBloodwormTime.Text = (BloodwormUptime.Value / 100f).ToString("P");
            Character.OnCalculationsInvalidated();
        }

        private void GhoulUptime_Scroll(object sender, EventArgs e)
        {
            CalculationOptionsDPSDK calcOpts = Character.CalculationOptions as CalculationOptionsDPSDK;
            calcOpts.GhoulUptime = GhoulUptime.Value / 100f;
            lbGhoulTime.Text = (GhoulUptime.Value / 100f).ToString("P");
            Character.OnCalculationsInvalidated();
        }

        private void KMProcUsage_Scroll(object sender, EventArgs e)
        {
            CalculationOptionsDPSDK calcOpts = Character.CalculationOptions as CalculationOptionsDPSDK;
            calcOpts.KMProcUsage = KMProcUsage.Value / 100f;
            lbKMProcUsage.Text = (KMProcUsage.Value / 100f).ToString("P");
            Character.OnCalculationsInvalidated();
        }

        private void cbDisplayCalcs_CheckedChanged(object sender, EventArgs e)
        {
            CalculationOptionsDPSDK calcOpts = Character.CalculationOptions as CalculationOptionsDPSDK;
            calcOpts.GetRefreshForDisplayCalcs = cbDisplayCalcs.Checked;
            Character.OnCalculationsInvalidated();
        }

        private void cbRefCalcs_CheckedChanged(object sender, EventArgs e)
        {
            CalculationOptionsDPSDK calcOpts = Character.CalculationOptions as CalculationOptionsDPSDK;
            calcOpts.GetRefreshForReferenceCalcs = cbRefCalcs.Checked;
            Character.OnCalculationsInvalidated();
        }

        private void cbSignificantChange_CheckedChanged(object sender, EventArgs e)
        {
            CalculationOptionsDPSDK calcOpts = Character.CalculationOptions as CalculationOptionsDPSDK;
            calcOpts.GetRefreshForSignificantChange = cbSignificantChange.Checked;
            Character.OnCalculationsInvalidated();
        }

        private void cbExperimental_CheckedChanged(object sender, EventArgs e)
        {
            CalculationOptionsDPSDK calcOpts = Character.CalculationOptions as CalculationOptionsDPSDK;
            calcOpts.m_bExperimental = cbExperimental.Checked;
            Character.OnCalculationsInvalidated();
        }

    }
}