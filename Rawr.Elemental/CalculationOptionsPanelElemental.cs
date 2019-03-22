using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Elemental
{
    public partial class CalculationOptionsPanelElemental : CalculationOptionsPanelBase
    {
        private bool loading = false;

        // The fire ele checkbox is currently set to "Visible = False" so that the UI is in place, but it is inaccessible at this moment

        public CalculationOptionsPanelElemental()
        {
            InitializeComponent();

            // Set about text
            tbModuleNotes.Text =
                "Notes:\r\n" +
                "For the estimator, it is assumed you use Flametongue weapon and Water Shield.\r\n" +
                "Trinkets, Elemental Mastery and Clearcasting are modelled by calculating their average value during the fight.\r\n" +
                "\r\n" +
                "Assumed rotation:\r\n" +
                "- Flame shock up.\n" +
                "- Lava Burst whenever off cooldown.\r\n" +
                "- Cast Thunderstorm whenever available if using Thunderstorm\r\n" +
                "- Cast the highest DPS option between lightning bolt, fire nova, and chain lightning.\r\n" +
                "\r\n" +
                "Legend:\r\n" +
                "FS - Flame Shock\r\n" +
                "LvB - Lava Burst\r\n" +
                "LB - Lightning Bolt\r\n" +
                "CL - Chain Lightning [which is then followed by the number of targets hit, such as CL2 hits 2 targets]\r\n" +
                "FN - Fire Nova [which is followed by the number of targets hit]\r\n" +
                "ST - Searing Totem\r\n" +
                "MT - Magma Totem [which is followed by the number of targets hit]\r\n";
        }

        protected override void LoadCalculationOptions()
        {
            loading = true;

            CalculationOptionsElemental calcOpts;

            if (Character.CalculationOptions == null)
            {
                calcOpts = new CalculationOptionsElemental();
                Character.CalculationOptions = calcOpts;
            }
            else
                calcOpts = (CalculationOptionsElemental)Character.CalculationOptions;

            tbBSRatio.Value = calcOpts.BSRatio;
            int burst = 100 - tbBSRatio.Value;
            int sust = tbBSRatio.Value;
            lblBSRatio.Text = "Ratio: "+burst + "% Burst, "+sust + "% Sustained.";

            trkFightLength.Value = calcOpts.FightDuration / 15;
            int m = trkFightLength.Value / 4;
            int s = calcOpts.FightDuration - 60 * m;
            lblFightLength.Text = "Fight duration: " + m + ":" + s;

            trkTargets.Value = calcOpts.NumberOfTargets;
            lbTargets.Text = "Number of targets: " + calcOpts.NumberOfTargets;

            textBoxCastLatency.Text = calcOpts.LatencyCast.ToString(System.Globalization.CultureInfo.InvariantCulture);
            textBoxGCDLatency.Text = calcOpts.LatencyGcd.ToString(System.Globalization.CultureInfo.InvariantCulture);

            cbThunderstorm.Checked = calcOpts.UseThunderstorm;
            cbFireNova.Checked = calcOpts.UseFireNova;
            cbChainLightning.Checked = calcOpts.UseChainLightning;
            cbFireDpsTotem.Checked = calcOpts.UseDpsTotem;
            cbFireEle.Checked = calcOpts.UseFireEle;

            loading = false;
        }


        private float parseFloat(string s)
        {
            float tmp;
            float.TryParse(s, out tmp);

            if (tmp > 0)
                return tmp;
            
            return 0;
        }

        private void trkFightLength_Scroll(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsElemental calcOpts = Character.CalculationOptions as CalculationOptionsElemental;
            calcOpts.FightDuration = trkFightLength.Value * 15;
            int m = trkFightLength.Value / 4;
            int s = calcOpts.FightDuration - 60 * m;
            lblFightLength.Text = "Fight duration: "+m+":"+s;
            Character.OnCalculationsInvalidated();
        }

        private void tbBSRatio_Scroll(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsElemental calcOpts = Character.CalculationOptions as CalculationOptionsElemental;
            calcOpts.BSRatio = tbBSRatio.Value;
            int burst = 100 - tbBSRatio.Value;
            int sust = tbBSRatio.Value;
            lblBSRatio.Text = "Ratio: "+burst + "% Burst, "+sust + "% Sustained.";
            Character.OnCalculationsInvalidated();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsElemental calcOpts = Character.CalculationOptions as CalculationOptionsElemental;
            calcOpts.UseThunderstorm = cbThunderstorm.Checked;
            Character.OnCalculationsInvalidated();
        }

        private void textBoxCastLatency_TextChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsElemental calcOpts = Character.CalculationOptions as CalculationOptionsElemental;
            float lag;
            if(!float.TryParse(textBoxCastLatency.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out lag))
            {
                textBoxCastLatency.Text = calcOpts.LatencyCast.ToString(System.Globalization.CultureInfo.InvariantCulture);
                return;
            }
            calcOpts.LatencyCast = lag;
            Character.OnCalculationsInvalidated();
        }

        private void textBoxGCDLatency_TextChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsElemental calcOpts = Character.CalculationOptions as CalculationOptionsElemental;
            float lag;
            if (!float.TryParse(textBoxGCDLatency.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out lag))
            {
                textBoxGCDLatency.Text = calcOpts.LatencyGcd.ToString(System.Globalization.CultureInfo.InvariantCulture);
                return;
            }
            calcOpts.LatencyGcd = lag;
            Character.OnCalculationsInvalidated();
        }

        private void trkTargets_Scroll(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsElemental calcOpts = Character.CalculationOptions as CalculationOptionsElemental;
            calcOpts.NumberOfTargets = trkTargets.Value;
            lbTargets.Text = "Number of Targets: " + trkTargets.Value;
            Character.OnCalculationsInvalidated();
        }

        private void cbFireNova_CheckedChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsElemental calcOpts = Character.CalculationOptions as CalculationOptionsElemental;
            calcOpts.UseFireNova = cbFireNova.Checked;
            Character.OnCalculationsInvalidated();
        }

        private void cbChainLightning_CheckedChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsElemental calcOpts = Character.CalculationOptions as CalculationOptionsElemental;
            calcOpts.UseChainLightning = cbChainLightning.Checked;
            Character.OnCalculationsInvalidated();
        }

        private void cbFireTotem_CheckedChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsElemental calcOpts = Character.CalculationOptions as CalculationOptionsElemental;
            calcOpts.UseDpsTotem = cbFireDpsTotem.Checked;
            Character.OnCalculationsInvalidated();
        }

        private void cbFireEle_CheckedChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsElemental calcOpts = Character.CalculationOptions as CalculationOptionsElemental;
            calcOpts.UseFireEle = cbFireEle.Checked;
            Character.OnCalculationsInvalidated();
        }
    }
}
