using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Rawr.Healadin
{
    public partial class CalculationOptionsPanelHealadin : CalculationOptionsPanelBase
    {
        public CalculationOptionsPanelHealadin()
        {
            InitializeComponent();
        }

        private bool loading;

        protected override void LoadCalculationOptions()
        {
            loading = true;
            if (Character.CalculationOptions == null)
                Character.CalculationOptions = new CalculationOptionsHealadin();

			CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
            cmbLength.Value = (decimal)calcOpts.Length;

            nudDivinePlea.Value = (decimal)calcOpts.DivinePlea;
            nudGHL.Value = (decimal)calcOpts.GHL_Targets;

			trkActivity.Value = (int)(calcOpts.Activity * 100);
            lblActivity.Text = trkActivity.Value + "%";

            chkSpiritIrrelevant.Checked = calcOpts.HitIrrelevant;
            chkHitIrrelevant.Checked = calcOpts.SpiritIrrelevant;
            if (CalculationsHealadin.IsHitIrrelevant != calcOpts.HitIrrelevant || CalculationsHealadin.IsSpiritIrrelevant != calcOpts.SpiritIrrelevant)
            {
                CalculationsHealadin.IsSpiritIrrelevant = calcOpts.SpiritIrrelevant;
                CalculationsHealadin.IsHitIrrelevant = calcOpts.HitIrrelevant;
                ItemCache.OnItemsChanged();
            }

            chkJotP.Checked = calcOpts.JotP;
            chkJudgement.Checked = calcOpts.Judgement;
            chkLoHSelf.Checked = calcOpts.LoHSelf;

            trkReplenishment.Value = (int)Math.Round(calcOpts.Replenishment * 100);
            lblReplenishment.Text = trkReplenishment.Value + "%";
            
            trkBoLUp.Value = (int)Math.Round(calcOpts.BoLUp * 100);
            lblBoLUp.Text = trkBoLUp.Value + "%";

            trkBurstScale.Value = (int)Math.Round(calcOpts.BurstScale * 100);
            lblBurstScale.Text = trkBurstScale.Value + "%";

            trkHS.Value = (int)Math.Round(calcOpts.HolyShock * 100);
            lblHS.Text = trkHS.Value + "%";

            trkSacredShield.Value = (int)Math.Round(calcOpts.SSUptime * 100);
            lblSacredShield.Text = trkSacredShield.Value + "%";

            trkFlashOfLightOnTank.Value = (int)Math.Round(calcOpts.FoLOnTank * 100);
            lblFlashOfLightOnTank.Text = trkFlashOfLightOnTank.Value + "%";

            chkIoL.Checked = calcOpts.InfusionOfLight;
            trkIoLRatio.Value = (int)Math.Round(calcOpts.IoLHolyLight * 100f);
            lblIoLHL.Text = trkIoLRatio.Value + "% HL";
            lblIoLFoL.Text = (100 - trkIoLRatio.Value) + "% FoL";
            trkIoLRatio.Enabled = calcOpts.InfusionOfLight;
            lblIoLHL.Enabled = calcOpts.InfusionOfLight;
            lblIoLFoL.Enabled = calcOpts.InfusionOfLight;

            loading = false;
        }
 
        private void cmbLength_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                calcOpts.Length = (float)cmbLength.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void trkActivity_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                lblActivity.Text = trkActivity.Value + "%";
                calcOpts.Activity = trkActivity.Value / 100f;
                Character.OnCalculationsInvalidated();
            }
        }

        private void trkReplenishment_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                lblReplenishment.Text = trkReplenishment.Value + "%";
                calcOpts.Replenishment = trkReplenishment.Value / 100f;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudDivinePlea_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                calcOpts.DivinePlea = (float)nudDivinePlea.Value;
                Character.OnCalculationsInvalidated();
            }
        }

        private void chkJotP_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                calcOpts.JotP = chkJotP.Checked;
                Character.OnCalculationsInvalidated();
            }
        }

        private void trkBoLUp_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                lblBoLUp.Text = trkBoLUp.Value + "%";
                calcOpts.BoLUp = trkBoLUp.Value / 100f;
                Character.OnCalculationsInvalidated();
            }
        }

        private void trkHS_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                calcOpts.HolyShock = trkHS.Value / 100f;
                lblHS.Text = trkHS.Value + "%";
                Character.OnCalculationsInvalidated();
            }
        }


        private void chkLoHSelf_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                calcOpts.LoHSelf = chkLoHSelf.Checked;
                Character.OnCalculationsInvalidated();
            }
        }

        private void nudGHL_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                calcOpts.GHL_Targets = (float)nudGHL.Value;
                Character.OnCalculationsInvalidated();
            }
        }


        private void trkBurstScale_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                calcOpts.BurstScale = trkBurstScale.Value / 100f;
                lblBurstScale.Text = trkBurstScale.Value + "%";
                Character.OnCalculationsInvalidated();
            }
        }

        private void chkIoL_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                calcOpts.InfusionOfLight = chkIoL.Checked;
                trkIoLRatio.Enabled = calcOpts.InfusionOfLight;
                lblIoLHL.Enabled = calcOpts.InfusionOfLight;
                lblIoLFoL.Enabled = calcOpts.InfusionOfLight;
                Character.OnCalculationsInvalidated();
            }
        }

        private void trkIoLRatio_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                calcOpts.IoLHolyLight = trkIoLRatio.Value / 100f;
                lblIoLHL.Text = trkIoLRatio.Value + "% HL";
                lblIoLFoL.Text = (100 - trkIoLRatio.Value) + "% FoL";
                Character.OnCalculationsInvalidated();
            }
        }

        private void trkSacredShield_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                calcOpts.SSUptime = trkSacredShield.Value / 100f;
                lblSacredShield.Text = trkSacredShield.Value + "%";
                Character.OnCalculationsInvalidated();
            }
        }

        private void trkFlashOfLightOnTank_Scroll(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                calcOpts.FoLOnTank = trkFlashOfLightOnTank.Value / 100f;
                lblFlashOfLightOnTank.Text = trkFlashOfLightOnTank.Value + "%";
                Character.OnCalculationsInvalidated();
            }
        }

        private void chkJudgement_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                calcOpts.Judgement = chkJudgement.Checked;
                Character.OnCalculationsInvalidated();
            }
        }

        private void chkHitIrrelevant_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                calcOpts.HitIrrelevant = chkHitIrrelevant.Checked;
                CalculationsHealadin.IsHitIrrelevant = chkHitIrrelevant.Checked;
                ItemCache.OnItemsChanged();
            }
        }

        private void chkSpiritIrrelevant_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                CalculationOptionsHealadin calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
                calcOpts.SpiritIrrelevant = chkSpiritIrrelevant.Checked;
                CalculationsHealadin.IsSpiritIrrelevant = chkSpiritIrrelevant.Checked;
                ItemCache.OnItemsChanged();
            }
        }

    }

}
