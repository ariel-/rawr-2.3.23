using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Rawr.Base;

namespace Rawr.TankDK {
    public partial class CalculationOptionsPanelTankDK : Rawr.CalculationOptionsPanelBase {
        public CalculationOptionsPanelTankDK() 
        { 
            InitializeComponent(); 
        }

        protected override void LoadCalculationOptions() 
        {
            _loadingCalculationOptions = true;
            if (Character.CalculationOptions == null) 
            { 
                Character.CalculationOptions = new CalculationOptionsTankDK();
            }

            options = Character.CalculationOptions as CalculationOptionsTankDK;

            for (int i = 0; i < cmbAttackerLevel.Items.Count; i++) {
                if (cmbAttackerLevel.Items[i] as string == options.TargetLevel.ToString()) {
                    cmbAttackerLevel.SelectedItem = cmbAttackerLevel.Items[i];
                    break;
                }
            }

            comboChartType.SelectedItem = comboChartType.Items[(int)options.cType];

            numThreatWeight.Value = (decimal)options.ThreatWeight;
            numSurvivalWeight.Value = (decimal)options.SurvivalWeight;
            numMitigationWeight.Value = (decimal)options.MitigationWeight;
            cb_AdditiveMitigation.Checked = options.AdditiveMitigation;
            cbExperimental.Checked = options.bExperimental;
            numBossAttackSpeed.Value = (decimal)options.BossAttackSpeed;
            numIncomingDamage.Value = options.IncomingDamage;
            numIncFromMagicFrequency.Value = (decimal)options.IncomingFromMagicFrequency;
            nudTargetArmor.Value = (decimal)options.BossArmor;
            tbFightLength.Value = (int)options.FightLength;
            numTargets.Value = (int)options.uNumberTargets;
            cb_ParryHaste.Checked = options.bParryHaste;

            numIncMagicDamage.Value = options.IncomingMagicDamage;
            numBleedTickFreq.Value = (decimal)options.BleedTickFrequency;
            numBleedPerTick.Value = options.IncomingBleedDamage;

            _loadingCalculationOptions = false;
        }

        private bool _loadingCalculationOptions = false;

        private CalculationOptionsTankDK options;

        private void cmbAttackerLevel_SelectedIndexChanged(object sender, EventArgs e) {
            if (!_loadingCalculationOptions) {
                options.TargetLevel = int.Parse(cmbAttackerLevel.SelectedItem.ToString());
                Character.OnCalculationsInvalidated();
            }
        }
        private void numThreatWeight_ValueChanged(object sender, EventArgs e) {
            if (!_loadingCalculationOptions) {
                options.ThreatWeight = (float)(numThreatWeight.Value);
                Character.OnCalculationsInvalidated();
            }
        }
        private void numBossAttackSpeed_ValueChanged(object sender, EventArgs e) {
            if (!_loadingCalculationOptions) {
                options.BossAttackSpeed = (float)(numBossAttackSpeed.Value);
                Character.OnCalculationsInvalidated();
            }
        }
        private void numSurvivalWeight_ValueChanged(object sender, EventArgs e) {
            if (!_loadingCalculationOptions) {
                options.SurvivalWeight = (float)(numSurvivalWeight.Value);
                Character.OnCalculationsInvalidated();
            }
        }
        private void numMitigationWeight_ValueChanged(object sender, EventArgs e)
        {
            if (!_loadingCalculationOptions)
            {
                options.MitigationWeight = (float)(numMitigationWeight.Value);
                Character.OnCalculationsInvalidated();
            }
        }

        private void numIncomingDamage_ValueChanged(object sender, EventArgs e) {
            if (!_loadingCalculationOptions) {
                options. IncomingDamage = (uint)(numIncomingDamage.Value);
                Character.OnCalculationsInvalidated();
            }
        }
        private void numPercIncFromMagic_ValueChanged(object sender, EventArgs e) {
            if (!_loadingCalculationOptions) {
                options.IncomingFromMagicFrequency = (float)(numIncFromMagicFrequency.Value);
                Character.OnCalculationsInvalidated();
            }
        }
        private void btnRotation_Click(object sender, EventArgs e) {
            CalculationOptionsTankDK calcOpts = Character.CalculationOptions as CalculationOptionsTankDK;
            RotationViewer RV = new RotationViewer(calcOpts, Character);
            RV.ShowDialog();
            Character.OnCalculationsInvalidated();
        }
        private void nudTargetArmor_ValueChanged(object sender, EventArgs e) {
            if (!_loadingCalculationOptions) {
                options.BossArmor = (int)nudTargetArmor.Value;
                Character.OnCalculationsInvalidated();
            }
        }
        private void tbFightLength_Scroll(object sender, EventArgs e) {
            if (!_loadingCalculationOptions) {
                options.FightLength = (int)tbFightLength.Value;
                Character.OnCalculationsInvalidated();
            }
        }
        private void numTargets_ValueChanged(object sender, EventArgs e) {
            if (!_loadingCalculationOptions) {
                options.uNumberTargets = (uint)numTargets.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void comboChartType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loadingCalculationOptions)
            {
                options.cType = (CalculationType)comboChartType.SelectedIndex;
                Character.OnCalculationsInvalidated();
            }
        }

        private void cb_AdditiveMitigation_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loadingCalculationOptions)
            {
                options.AdditiveMitigation = cb_AdditiveMitigation.Checked;
                Character.OnCalculationsInvalidated();
            }
        }

        private void cbExperimental_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loadingCalculationOptions)
            {
                options.bExperimental = cbExperimental.Checked;
                Character.OnCalculationsInvalidated();
            }
        }

        private void cb_ParryHaste_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loadingCalculationOptions)
            {
                options.bParryHaste = cb_ParryHaste.Checked;
                Character.OnCalculationsInvalidated();
            }
        }

        private void numIncMagicDamage_ValueChanged(object sender, EventArgs e)
        {
            if (!_loadingCalculationOptions)
            {
                options.IncomingMagicDamage = (uint)numIncMagicDamage.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void numBleedTickFreq_ValueChanged(object sender, EventArgs e)
        {
            if (!_loadingCalculationOptions)
            {
                options.BleedTickFrequency = (float)numBleedTickFreq.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void numBleedPerTick_ValueChanged(object sender, EventArgs e)
        {
            if (!_loadingCalculationOptions)
            {
                options.IncomingBleedDamage = (uint)numBleedPerTick.Value;
                Character.OnCalculationsInvalidated();
            }

        }

        private void cbUseOnUse_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loadingCalculationOptions)
            {
                options.bUseOnUseAbilities = cbUseOnUse.Checked;
                Character.OnCalculationsInvalidated();
                if (cbUseOnUse.Checked == false)
                    MessageBox.Show("WARNING: Disabling this option will make all 'OnUse' abilities have 0 value in the system.");
            }
        }

        #region StatsGraph
        private Stats[] BuildStatsList()
        {
            List<Stats> statsList = new List<Stats>();
            statsList.Add(new Stats() { Strength = 1f });
            statsList.Add(new Stats() { Stamina = 1f });
            statsList.Add(new Stats() { Agility = 1f });
            statsList.Add(new Stats() { AttackPower = 2f });
            statsList.Add(new Stats() { CritRating = 1f });
            statsList.Add(new Stats() { HitRating = 1f });
            statsList.Add(new Stats() { ExpertiseRating = 1f });
            statsList.Add(new Stats() { HasteRating = 1f });
            statsList.Add(new Stats() { ArmorPenetrationRating = 1f });
            statsList.Add(new Stats() { DefenseRating = 1f });
            statsList.Add(new Stats() { DodgeRating = 1f });
            statsList.Add(new Stats() { ParryRating = 1f });
            return statsList.ToArray();
        }

        private void btnStatsGraph_Click_1(object sender, EventArgs e)
        {
            Stats[] statsList = BuildStatsList();
            Graph graph = new Graph();
            string explanatoryText = "This graph shows how adding or subtracting\nmultiples of a stat affects your Overall Score.\n\nAt the Zero position is your current Overall.\n" +
                         "To the right of the zero vertical is adding stats.\nTo the left of the zero vertical is subtracting stats.\n" +
                         "The vertical axis shows the amount of Overall points added or lost";
            graph.SetupStatsGraph(Character, statsList, 1000, explanatoryText, null);
            graph.Show();
        }
        #endregion

    }
}
