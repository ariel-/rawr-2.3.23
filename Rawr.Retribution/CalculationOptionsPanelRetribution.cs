using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Rawr.Retribution
{
    public partial class CalculationOptionsPanelRetribution : CalculationOptionsPanelBase
    {

        public CalculationOptionsPanelRetribution()
        {
            InitializeComponent();
        }

        private bool loading;

        protected override void LoadCalculationOptions()
        {
            loading = true;
            if (Character.CalculationOptions == null)
                Character.CalculationOptions = new CalculationOptionsRetribution();

            CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;

            cmbMobType.SelectedIndex = (int)calcOpts.Mob;

            if (calcOpts.Seal == SealOf.Blood)
            {
                calcOpts.Seal = SealOf.Vengeance;
                Character.OnCalculationsInvalidated();
            }
            cmbSeal.SelectedIndex = (int)calcOpts.Seal - 1;
            
            cmbLength.Value = (decimal)calcOpts.FightLength;

            nudTimeUnder20.Value = (decimal)(calcOpts.TimeUnder20 * 100);
            nudInFront.Value = (decimal)(calcOpts.InFront * 100);
            nudConsEff.Value = (decimal)(calcOpts.ConsEff * 100);
            nudHoR.Value = (decimal)(calcOpts.HoREff * 100);
            nudTargetSwitch.Value = (decimal)calcOpts.TargetSwitches;
            nudTargets.Value = (decimal)calcOpts.Targets;

            chkBloodlust.Checked = calcOpts.Bloodlust;

            nudDelay.Value = (decimal)calcOpts.Delay;
            nudWait.Value = (decimal)calcOpts.Wait;
            nudStackTrinket.Value = (decimal)calcOpts.StackTrinketReset;
            nudTargetLevel.Value = (decimal)calcOpts.TargetLevel;

            nudJudge.Value = (decimal)calcOpts.JudgeCD;
            nudCS.Value = (decimal)calcOpts.CSCD;
            nudDS.Value = (decimal)calcOpts.DSCD;
            nudCons.Value = (decimal)calcOpts.ConsCD;
            nudExo.Value = (decimal)calcOpts.ExoCD;
            nudJudge20.Value = (decimal)calcOpts.JudgeCD20;
            nudCS20.Value = (decimal)calcOpts.CSCD20;
            nudDS20.Value = (decimal)calcOpts.DSCD20;
            nudCons20.Value = (decimal)calcOpts.ConsCD20;
            nudExo20.Value = (decimal)calcOpts.ExoCD20;
            nudHoW20.Value = (decimal)calcOpts.HoWCD20;

            if (calcOpts.Rotations.Count == 0) 
                calcOpts.Rotations.Add(SimulatorParameters.DefaultRotation());
            butDelRotation.Enabled = calcOpts.Rotations.Count > 1;
            buildRotationCombo();
            cmbRotations.SelectedIndex = 0;
            showRotation(0);

            if (calcOpts.SimulateRotation) 
                radRotSim.Checked = true;
            else 
                radEffectiveCD.Checked = true;
            SetSimulateRotation(calcOpts.SimulateRotation);

            textExperimental.Text = calcOpts.Experimental;
            CalculationsRetribution.Experimental = calcOpts.Experimental;

#if DEBUG
            buttonStatGraph.Visible = true;
            buttonEcdGraph.Visible = true;
#else
            buttonStatGraph.Visible = true;            
            buttonEcdGraph.Visible = true;            
#endif
            loading = false;
        }

        private void cmbMobType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.Mob = (MobType)Enum.Parse(typeof(MobType), cmbMobType.Text);
                Character.OnCalculationsInvalidated();
            }
        }

        private void cmbLength_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.FightLength = (float)cmbLength.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudDelay_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.Delay = (float)nudDelay.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void SetSimulateRotation(bool rot)
        {
            if (rot)
            {
                listRotation.Enabled = true;
                lblDelay.Enabled = true;
                lblWait.Enabled = true;
                butRotationDown.Enabled = true;
                butRotationUp.Enabled = true;
                nudDelay.Enabled = true;
                nudWait.Enabled = true;
                nudJudge.Enabled = false;
                nudJudge20.Enabled = false;
                nudCS.Enabled = false;
                nudCS20.Enabled = false;
                nudDS.Enabled = false;
                nudDS20.Enabled = false;
                nudCons.Enabled = false;
                nudCons20.Enabled = false;
                nudExo.Enabled = false;
                nudExo20.Enabled = false;
                nudHoW20.Enabled = false;
                lblPost20.Enabled = false;
                lblPre20.Enabled = false;
                lblJudge.Enabled = false;
                lblCS.Enabled = false;
                lblDS.Enabled = false;
                lblCons.Enabled = false;
                lblExo.Enabled = false;
                lblHoW.Enabled = false;
            }
            else
            {
                listRotation.Enabled = false;
                lblDelay.Enabled = false;
                lblWait.Enabled = false;
                butRotationDown.Enabled = false;
                butRotationUp.Enabled = false;
                nudDelay.Enabled = false;
                nudWait.Enabled = false;
                nudJudge.Enabled = true;
                nudJudge20.Enabled = true;
                nudCS.Enabled = true;
                nudCS20.Enabled = true;
                nudDS.Enabled = true;
                nudDS20.Enabled = true;
                nudCons.Enabled = true;
                nudCons20.Enabled = true;
                nudExo.Enabled = true;
                nudExo20.Enabled = true;
                nudHoW20.Enabled = true;
                lblPost20.Enabled = true;
                lblPre20.Enabled = true;
                lblJudge.Enabled = true;
                lblCS.Enabled = true;
                lblDS.Enabled = true;
                lblCons.Enabled = true;
                lblExo.Enabled = true;
                lblHoW.Enabled = true;
            }
        }

        #region Effective Cooldowns
        private void radRotSim_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                SetSimulateRotation(radRotSim.Checked);
                calcOpts.SimulateRotation = radRotSim.Checked;
                Character.OnCalculationsInvalidated();
            }
        }

        private void txtJudge_TextChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.Delay = (float)nudDelay.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudJudge_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.JudgeCD = (float)nudJudge.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudJudge20_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.JudgeCD20 = (float)nudJudge20.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudCS_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.CSCD = (float)nudCS.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudCS20_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.CSCD20 = (float)nudCS20.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudDS_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.DSCD = (float)nudDS.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudDS20_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.DSCD20 = (float)nudDS20.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudCons_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.ConsCD = (float)nudCons.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudCons20_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.ConsCD20 = (float)nudCons20.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudExo_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.ExoCD = (float)nudExo.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudExo20_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.ExoCD20 = (float)nudExo20.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudHoW20_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.HoWCD20 = (float)nudHoW20.Value;
                Character.OnCalculationsInvalidated();
            }
        }
        #endregion

        private void nudWait_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.Wait = (float)nudWait.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudTargetLevel_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.TargetLevel = (int)nudTargetLevel.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void cmbSeal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.Seal = (SealOf)Enum.Parse(typeof(SealOf), cmbSeal.Text);
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudStackTrinket_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.StackTrinketReset = (int)nudStackTrinket.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudTargets_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.Targets = (float)nudTargets.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudTimeUnder20_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.TimeUnder20 = (float)nudTimeUnder20.Value / 100f;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudInFront_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.InFront = (float)nudInFront.Value / 100f;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudConsEff_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.ConsEff = (float)nudConsEff.Value / 100f;
                Character.OnCalculationsInvalidated();
            }
        }

        private void chkBloodlust_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.Bloodlust = chkBloodlust.Checked;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudHoR_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.HoREff = (float)nudHoR.Value / 100f;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudTargetSwitch_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.TargetSwitches = (float)nudTargetSwitch.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void butNewRotation_Click(object sender, EventArgs e)
        {
            CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
            calcOpts.Rotations.Add((Ability[])calcOpts.Rotations[cmbRotations.SelectedIndex].Clone());
            buildRotationCombo();
            cmbRotations.SelectedIndex = calcOpts.Rotations.Count - 1;
            butDelRotation.Enabled = calcOpts.Rotations.Count > 1;
            Character.OnCalculationsInvalidated();
        }

        private void buildRotationCombo()
        {
            bool wasLoading = loading;
            loading = true;
            int oldIndex = cmbRotations.SelectedIndex;
            CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
            List<string> rotationList = new List<string>(calcOpts.Rotations.Count);
            foreach (Ability[] rotation in calcOpts.Rotations)
            {
                rotationList.Add(SimulatorParameters.RotationString(rotation));
            }
            cmbRotations.DataSource = rotationList;
            cmbRotations.SelectedIndex = oldIndex;
            loading = wasLoading;
        }

        private void showRotation(int number)
        {
            CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
            List<string> abilityList = new List<string>(calcOpts.Rotations[number].Length);
            foreach (Ability ability in calcOpts.Rotations[number])
            {
                abilityList.Add(SimulatorParameters.AbilityString(ability));
            }
            listRotation.Items.Clear();
            listRotation.Items.AddRange(abilityList.ToArray());
        }

        private void butRotationUp_Click(object sender, EventArgs e)
        {
            CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
            int selected = listRotation.SelectedIndex;
            if ((selected > 0) && (selected < (int)Ability.Last + 1))
            {
                Ability[] rotation = calcOpts.Rotations[cmbRotations.SelectedIndex];

                Ability tempAbility = rotation[selected - 1];
                rotation[selected - 1] = rotation[selected];
                rotation[selected] = tempAbility;

                showRotation(cmbRotations.SelectedIndex);
                buildRotationCombo();
                listRotation.SelectedIndex = selected - 1;

                Character.OnCalculationsInvalidated();
            }
        }

        private void butRotationDown_Click(object sender, EventArgs e)
        {
            CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
            int selected = listRotation.SelectedIndex;
            if (selected >= 0 && selected < 5)
            {
                Ability[] rotation = calcOpts.Rotations[cmbRotations.SelectedIndex];

                Ability tempAbility = rotation[selected + 1];
                rotation[selected + 1] = rotation[selected];
                rotation[selected] = tempAbility;

                showRotation(cmbRotations.SelectedIndex);
                buildRotationCombo();
                listRotation.SelectedIndex = selected + 1;

                Character.OnCalculationsInvalidated();
            }
        }

        private void cmbRotations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loading) showRotation(cmbRotations.SelectedIndex);
        }

        private void butDelRotation_Click(object sender, EventArgs e)
        {
            CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;

            calcOpts.Rotations.RemoveAt(cmbRotations.SelectedIndex);
            cmbRotations.SelectedIndex = 0;
            buildRotationCombo();
            butDelRotation.Enabled = calcOpts.Rotations.Count > 1;

            Character.OnCalculationsInvalidated();
        }

        private void textExperimental_TextChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsRetribution calcOpts = Character.CalculationOptions as CalculationOptionsRetribution;
                calcOpts.Experimental = textExperimental.Text;
                CalculationsRetribution.Experimental = textExperimental.Text;
                Character.OnCalculationsInvalidated();
            }
        }

        private void btnStatGraph_Click(object sender, EventArgs e)
        {
            GraphForm graphForm = new GraphForm(Character, true);
            graphForm.Show();
        }

        private void btnEcdGraph_Click(object sender, EventArgs e)
        {
            GraphForm graphForm = new GraphForm(Character, false);
            graphForm.Show();
        }
    }
}
