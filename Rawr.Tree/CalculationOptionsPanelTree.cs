using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Tree {
    public partial class CalculationOptionsPanelTree : CalculationOptionsPanelBase {
        private bool loading = false;
        public CalculationOptionsPanelTree() {
            InitializeComponent();

            // Set about text
            tbModuleNotes.Text =
                "The module is currently undergoing lots of work.\r\n\r\n" +
                "You're welcome to send feedback via the CodePlex site.\r\n\r\n" +
                "A small list of things I plan to do:\r\n- Fix haste trinkets.\r\n- Improve UI (e.g. better profiles).\r\n- Add Healing Touch in spell rotations.\r\n- Lifebloom x2 stacks.\r\n- Fix custom chart problems (custom colors)";
        }
        protected void RefreshProfile()
        {
            CalculationOptionsTree calcOpts = (CalculationOptionsTree)Character.CalculationOptions;

            tbSpellProfileName.Text = calcOpts.Current.Name;

            trkFightLength.Value = calcOpts.Current.FightDuration / 5;
            int m = trkFightLength.Value / 12;
            int s = calcOpts.Current.FightDuration - 60 * m;
            lblFightLength.Text = "Fight duration: " + m + ":" + (s < 10 ? "0" : "") + s;

            tkReplenishment.Value = calcOpts.Current.ReplenishmentUptime;
            lblReplenishment.Text = tkReplenishment.Value + "% of fight spent with Replenishment.";

            tbRejuvCF.Value = calcOpts.Current.RejuvFrac;
            tbRegrowthCF.Value = calcOpts.Current.RegrowthFrac;
            tbLifebloomCF.Value = calcOpts.Current.LifebloomFrac;
            tbNourishCF.Value = calcOpts.Current.NourishFrac;
            lblRejuvFrac.Text = "Rejuvenation casting time: " + tbRejuvCF.Value + "%.";
            lblRegrowthFrac.Text = "Regrowth casting time: " + tbRegrowthCF.Value + "%.";
            lblLifebloomFrac.Text = "Lifebloom casting time: " + tbLifebloomCF.Value + "%.";
            lblNourishFrac.Text = "Nourish casting time: " + tbNourishCF.Value + "%.";

            tbLifebloomStackAmount.Value = calcOpts.Current.LifebloomStackAmount;
            tbRejuvAmount.Value = calcOpts.Current.RejuvAmount;
            tbRegrowthAmount.Value = calcOpts.Current.RegrowthAmount;
            lblLifebloomStackAmount.Text = "Number of maintained Lifebloom Stacks: " + tbLifebloomStackAmount.Value;
            lblRejuvAmount.Text = "Number of maintained Rejuvenations: " + tbRejuvAmount.Value;
            lblRegrowthAmount.Text = "Number of maintained Regrowths: " + tbRegrowthAmount.Value;

            cbLifebloomStackType.SelectedIndex = calcOpts.Current.LifebloomStackType;

            tbNourish1.Value = calcOpts.Current.Nourish1;
            tbNourish2.Value = calcOpts.Current.Nourish2;
            tbNourish3.Value = calcOpts.Current.Nourish3;
            tbNourish4.Value = calcOpts.Current.Nourish4;
            lblNourish1.Text = "Nourish on 1 hot: " + tbNourish1.Value + "%.";
            lblNourish2.Text = "Nourish on 2 hots: " + tbNourish2.Value + "%.";
            lblNourish3.Text = "Nourish on 3 hots: " + tbNourish3.Value + "%.";
            lblNourish4.Text = "Nourish on 4 hots: " + tbNourish4.Value + "%.";

            tbLivingSeed.Value = calcOpts.Current.LivingSeedEfficiency;
            lblLivingSeed.Text = "Living Seed effectiveness: " + (float)tbLivingSeed.Value + "%.";

            tbWildGrowth.Value = calcOpts.Current.WildGrowthPerMinute;
            lblWG.Text = tbWildGrowth.Value + " Wild Growth casts per minute.";

            tbSwiftmendPerMin.Value = calcOpts.Current.SwiftmendPerMinute;
            lblSwiftMend.Text = "Swiftmend casts per minute: " + tbSwiftmendPerMin.Value;

            cbInnervate.Checked = calcOpts.Current.Innervates > 0;

            tbIdlePercentage.Value = calcOpts.Current.IdleCastTimePercent;
            lblIdleFraction.Text = "Idle time: " + tbIdlePercentage.Value + "%.";

            cbIgnoreNaturesGrace.Checked = calcOpts.IgnoreNaturesGrace;
            switch (calcOpts.ProcType)
            {
                case 0:
                default:
                    rbProcsNone.Checked = true;
                    rbProcsAverage.Checked = false;
                    rbProcsMax.Checked = false;
                    break;
                case 1:
                    rbProcsNone.Checked = false;
                    rbProcsAverage.Checked = true;
                    rbProcsMax.Checked = false;
                    break;
                case 2:
                    rbProcsNone.Checked = false;
                    rbProcsAverage.Checked = false;
                    rbProcsMax.Checked = true;
                    break;
            }

            tbRevitalize.Value = calcOpts.Current.RevitalizePPM;
            lblRevitalize.Text = "Revitalize procs per minute: " + (float)calcOpts.Current.RevitalizePPM;

            adjustTimeRedrawAndSelect("", calcOpts.Current);
            tbTimeAdjust.Enabled = false;

            adjustManaRedrawAndSelect("", calcOpts.Current);
            tbManaAdjust.Enabled = false;
        }
        protected override void LoadCalculationOptions() {
            loading = true;

            CalculationOptionsTree calcOpts;

            if (Character.CalculationOptions == null){
                calcOpts = new CalculationOptionsTree();
                Character.CalculationOptions = calcOpts;
            } else {
                calcOpts = (CalculationOptionsTree)Character.CalculationOptions;
            }

            tbSingleTargetMax.Value = calcOpts.SingleTarget / 100;
            lblSingleTargetMax.Text = "Single Target Max Healing Target: " + calcOpts.SingleTarget + " hps";

            tbSustained.Value = calcOpts.SustainedTarget / 100;
            lblSustained.Text = "Sustained Healing Target: " + calcOpts.SustainedTarget + " hps";

            tbSurvMulti.Value = calcOpts.SurvValuePer100;
            lblSurvMulti.Text = calcOpts.SurvValuePer100.ToString() + " points per 100 survival (effective) health";

            cbSingleTargetRotation.SelectedIndex = calcOpts.SingleTargetRotation;

            cbSpellProfiles.Items.Clear();

            if (calcOpts.Profiles.Count == 0)
            {
                #region Defaults
                SpellProfile tankHealing = new SpellProfile()
                {
                    Name = "Tank Healing",
                    FightDuration = 300,
                    ReplenishmentUptime = 90,
                    WildGrowthPerMinute = 0,
                    Innervates = 1,
                    SwiftmendPerMinute = 3,
                    IdleCastTimePercent = 0,
                    RejuvFrac = 0,
                    RegrowthFrac = 0,
                    LifebloomFrac = 0,
                    NourishFrac = 100,
                    LifebloomStackAmount = 1,
                    LifebloomStackType = 0,
                    RejuvAmount = 1,
                    RegrowthAmount = 1,
                    Nourish1 = 0,
                    Nourish2 = 0,
                    Nourish3 = 100,
                    Nourish4 = 0,
                    ReduceOOMNourish = 100,
                    ReduceOOMRejuv = 0,
                    ReduceOOMRegrowth = 50,
                    ReduceOOMLifebloom = 50,
                    ReduceOOMWildGrowth = 100,
                    ReduceOOMRegrowthOrder = 0,
                    ReduceOOMNourishOrder = 1,
                    ReduceOOMLifebloomOrder = 2,
                    ReduceOOMWildGrowthOrder = 3,
                    ReduceOOMRejuvOrder = 4,
                    RevitalizePPM = 4,
                    LivingSeedEfficiency = 50
                };

                SpellProfile tankHealing2 = new SpellProfile()
                {
                    Name = "Tank Healing (no Lifebloom)",
                    FightDuration = 300,
                    ReplenishmentUptime = 90,
                    WildGrowthPerMinute = 0,
                    Innervates = 1,
                    SwiftmendPerMinute = 3,
                    IdleCastTimePercent = 0,
                    RejuvFrac = 0,
                    RegrowthFrac = 0,
                    LifebloomFrac = 0,
                    NourishFrac = 100,
                    LifebloomStackAmount = 0,
                    LifebloomStackType = 0,
                    RejuvAmount = 1,
                    RegrowthAmount = 1,
                    Nourish1 = 0,
                    Nourish2 = 0,
                    Nourish3 = 100,
                    Nourish4 = 0,
                    ReduceOOMNourish = 100,
                    ReduceOOMRejuv = 0,
                    ReduceOOMRegrowth = 50,
                    ReduceOOMLifebloom = 50,
                    ReduceOOMWildGrowth = 100,
                    ReduceOOMRegrowthOrder = 0,
                    ReduceOOMNourishOrder = 1,
                    ReduceOOMLifebloomOrder = 2,
                    ReduceOOMWildGrowthOrder = 3,
                    ReduceOOMRejuvOrder = 4,
                    RevitalizePPM = 4,
                    LivingSeedEfficiency = 50
                };

                SpellProfile raidHealing = new SpellProfile()
                {
                    Name = "Raid Healing (RJ/LB/SM/WG/N)",
                    FightDuration = 300,
                    ReplenishmentUptime = 90,
                    WildGrowthPerMinute = 8,
                    Innervates = 1,
                    SwiftmendPerMinute = 3,
                    IdleCastTimePercent = 0,
                    RejuvFrac = 60,
                    RegrowthFrac = 0,
                    LifebloomFrac = 20,
                    NourishFrac = 20,
                    LifebloomStackAmount = 0,
                    LifebloomStackType = 0,
                    RejuvAmount = 0,
                    RegrowthAmount = 0,
                    Nourish1 = 70,
                    Nourish2 = 0,
                    Nourish3 = 0,
                    Nourish4 = 0,
                    ReduceOOMNourish = 100,
                    ReduceOOMRejuv = 0,
                    ReduceOOMRegrowth = 100,
                    ReduceOOMLifebloom = 50,
                    ReduceOOMWildGrowth = 50,
                    ReduceOOMRegrowthOrder = 0,
                    ReduceOOMNourishOrder = 1,
                    ReduceOOMLifebloomOrder = 2,
                    ReduceOOMWildGrowthOrder = 3,
                    ReduceOOMRejuvOrder = 4,
                    RevitalizePPM = 7,
                    LivingSeedEfficiency = 50
                };

                SpellProfile raidHealing2 = new SpellProfile()
                {
                    Name = "Raid Healing (only WG/RJ)",
                    FightDuration = 300,
                    ReplenishmentUptime = 90,
                    WildGrowthPerMinute = 10,
                    Innervates = 1,
                    SwiftmendPerMinute = 0,
                    IdleCastTimePercent = 0,
                    RejuvFrac = 100,
                    RegrowthFrac = 0,
                    LifebloomFrac = 0,
                    NourishFrac = 0,
                    LifebloomStackAmount = 0,
                    LifebloomStackType = 0,
                    RejuvAmount = 0,
                    RegrowthAmount = 0,
                    Nourish1 = 0,
                    Nourish2 = 0,
                    Nourish3 = 0,
                    Nourish4 = 0,
                    ReduceOOMNourish = 100,
                    ReduceOOMRejuv = 100,
                    ReduceOOMRegrowth = 100,
                    ReduceOOMLifebloom = 100,
                    ReduceOOMWildGrowth = 100,
                    ReduceOOMRegrowthOrder = 0,
                    ReduceOOMNourishOrder = 1,
                    ReduceOOMLifebloomOrder = 2,
                    ReduceOOMWildGrowthOrder = 3,
                    ReduceOOMRejuvOrder = 4,
                    RevitalizePPM = 7,
                    LivingSeedEfficiency = 50
                };

                calcOpts.Profiles.Add(tankHealing);
                calcOpts.Profiles.Add(tankHealing2);
                calcOpts.Profiles.Add(raidHealing);
                calcOpts.Profiles.Add(raidHealing2);
                #endregion
            }
            
            foreach (SpellProfile profile in calcOpts.Profiles)
            {
                cbSpellProfiles.Items.Add(profile);
            }
            btnSpellProfileAdd.Enabled = tbSpellProfileName.Text.Length > 0 && isUniqueName(tbSpellProfileName.Text);
            btnSpellProfileLoad.Enabled = cbSpellProfiles.SelectedItem != null;
            btnSpellProfileDelete.Enabled = cbSpellProfiles.SelectedItem != null;

            RefreshProfile();

            loading = false;
        }
        private float parseFloat(string s) {
            float tmp;
            float.TryParse(s, out tmp);
            if (tmp > 0f) { return tmp; }
            return 0f;
        }
        private void trkFightLength_Scroll(object sender, EventArgs e) {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.Current.FightDuration = trkFightLength.Value * 5;
            int m = trkFightLength.Value / 12;
            int s = calcOpts.Current.FightDuration - 60 * m;
            lblFightLength.Text = "Fight duration: " + m + ":" + (s < 10 ? "0" : "") + s;
            Character.OnCalculationsInvalidated();
        }
        private void tkReplenishment_Scroll(object sender, EventArgs e) {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.Current.ReplenishmentUptime = tkReplenishment.Value;
            lblReplenishment.Text = tkReplenishment.Value + "% of fight spent with Replenishment.";
            Character.OnCalculationsInvalidated();
        }
        private void tbSurvMulti_Scroll(object sender, EventArgs e) {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.SurvValuePer100 = tbSurvMulti.Value;
            lblSurvMulti.Text = calcOpts.SurvValuePer100.ToString() + " points per 100 survival (effective) health";
            Character.OnCalculationsInvalidated();
        }
        private void cbInnervate_CheckedChanged(object sender, EventArgs e) {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.Current.Innervates = cbInnervate.Checked ? 1 : 0;
            Character.OnCalculationsInvalidated();
        }
        private void tbWildGrowth_Scroll(object sender, EventArgs e)
        {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.Current.WildGrowthPerMinute = tbWildGrowth.Value;
            lblWG.Text = tbWildGrowth.Value + " Wild Growth casts per minute.";
            Character.OnCalculationsInvalidated();
        }
        private void tbSwiftmend_Scroll(object sender, EventArgs e) {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.Current.SwiftmendPerMinute = tbSwiftmendPerMin.Value;
            lblSwiftMend.Text = "Swiftmend casts per minute: " + tbSwiftmendPerMin.Value;
            Character.OnCalculationsInvalidated();
        }
        private void tbIdlePercentage_Scroll(object sender, EventArgs e)
        {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.Current.IdleCastTimePercent = tbIdlePercentage.Value;
            lblIdleFraction.Text = "Idle time: " + tbIdlePercentage.Value + "%.";
            Character.OnCalculationsInvalidated();
        }

        private void tbAvgLifebloomStacks_Scroll(object sender, EventArgs e)
        {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.Current.LifebloomStackAmount = tbLifebloomStackAmount.Value;
            lblLifebloomStackAmount.Text = "Number of maintained Lifebloom Stacks: " + (float)tbLifebloomStackAmount.Value;
            Character.OnCalculationsInvalidated();
        }

        private void tbRejuvAmount_Scroll(object sender, EventArgs e)
        {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.Current.RejuvAmount = tbRejuvAmount.Value;
            lblRejuvAmount.Text = "Number of maintained Rejuvenations: " + (float)tbRejuvAmount.Value;
            Character.OnCalculationsInvalidated();
        }

        private void tbRegrowthAmount_Scroll(object sender, EventArgs e)
        {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.Current.RegrowthAmount = tbRegrowthAmount.Value;
            lblRegrowthAmount.Text = "Number of maintained Regrowths: " + (float)tbRegrowthAmount.Value;
            Character.OnCalculationsInvalidated();
        }

        private void cbLifebloomStackType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.Current.LifebloomStackType = cbLifebloomStackType.SelectedIndex;
            Character.OnCalculationsInvalidated();
        }

        private void tbNourish1_Scroll(object sender, EventArgs e)
        {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;

            calcOpts.Current.Nourish1 = tbNourish1.Value;

            int max = 100 - (calcOpts.Current.Nourish2 + calcOpts.Current.Nourish3 + calcOpts.Current.Nourish4);
            if (max < 0) max = 0;
            if (calcOpts.Current.Nourish1 > max)
            {
                calcOpts.Current.Nourish1 = max;
                tbNourish1.Value = max;
            }
            
            lblNourish1.Text = "Nourish on 1 hot: " + tbNourish1.Value + "%.";
            Character.OnCalculationsInvalidated();
        }

        private void tbNourish2_Scroll(object sender, EventArgs e)
        {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;

            calcOpts.Current.Nourish2 = tbNourish2.Value;

            int max = 100 - (calcOpts.Current.Nourish1 + calcOpts.Current.Nourish3 + calcOpts.Current.Nourish4);
            if (max < 0) max = 0;
            if (calcOpts.Current.Nourish2 > max)
            {
                calcOpts.Current.Nourish2 = max;
                tbNourish2.Value = max;
            }

            lblNourish2.Text = "Nourish on 2 hots: " + tbNourish2.Value + "%.";
            Character.OnCalculationsInvalidated();
        }

        private void tbNourish3_Scroll(object sender, EventArgs e)
        {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;

            calcOpts.Current.Nourish3 = tbNourish3.Value;

            int max = 100 - (calcOpts.Current.Nourish1 + calcOpts.Current.Nourish2 + calcOpts.Current.Nourish4);
            if (max < 0) max = 0;
            if (calcOpts.Current.Nourish3 > max)
            {
                calcOpts.Current.Nourish3 = max;
                tbNourish3.Value = max;
            }

            lblNourish3.Text = "Nourish on 3 hots: " + tbNourish3.Value + "%.";
            Character.OnCalculationsInvalidated();
        }

        private void tbNourish4_Scroll(object sender, EventArgs e)
        {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;

            calcOpts.Current.Nourish4 = tbNourish4.Value;

            int max = 100 - (calcOpts.Current.Nourish1 + calcOpts.Current.Nourish2 + calcOpts.Current.Nourish3);
            if (max < 0) max = 0;
            if (calcOpts.Current.Nourish4 > max)
            {
                calcOpts.Current.Nourish4 = max;
                tbNourish4.Value = max;
            }

            lblNourish4.Text = "Nourish on 4 hots: " + tbNourish4.Value + "%.";
            Character.OnCalculationsInvalidated();
        }

        private void tbLivingSeed_Scroll(object sender, EventArgs e)
        {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.Current.LivingSeedEfficiency = tbLivingSeed.Value;
            lblLivingSeed.Text = "Living Seed effectiveness: " + (float)tbLivingSeed.Value + "%.";
            Character.OnCalculationsInvalidated();
        }

        private void tbSingleTargetMax_Scroll(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.SingleTarget = tbSingleTargetMax.Value * 100;
            lblSingleTargetMax.Text = "Single Target Max Healing Target: " + calcOpts.SingleTarget + " hps";
            Character.OnCalculationsInvalidated();
        }

        private void tbSustained_Scroll(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.SustainedTarget = tbSustained.Value * 100;
            lblSustained.Text = "Sustained Healing Target: " + calcOpts.SustainedTarget + " hps";
            Character.OnCalculationsInvalidated();
        }

        private void cbSingleTargetRotation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.SingleTargetRotation = cbSingleTargetRotation.SelectedIndex;
            Character.OnCalculationsInvalidated();
        }

        private void tbRejuvCF_Scroll(object sender, EventArgs e)
        {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.Current.RejuvFrac = tbRejuvCF.Value;
            lblRejuvFrac.Text = "Rejuvenation casting time: " + (float)tbRejuvCF.Value + "%.";
            Character.OnCalculationsInvalidated();
        }

        private void tbRegrowthCF_Scroll(object sender, EventArgs e)
        {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.Current.RegrowthFrac = tbRegrowthCF.Value;
            lblRegrowthFrac.Text = "Regrowth casting time: " + (float)tbRegrowthCF.Value + "%.";
            Character.OnCalculationsInvalidated();
        }

        private void tbLifebloomCF_Scroll(object sender, EventArgs e)
        {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.Current.LifebloomFrac = tbLifebloomCF.Value;
            lblLifebloomFrac.Text = "Lifebloom casting time: " + (float)tbLifebloomCF.Value + "%.";
            Character.OnCalculationsInvalidated();
        }

        private void tbNourishCF_Scroll(object sender, EventArgs e)
        {
            if (loading) { return; }
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.Current.NourishFrac = tbNourishCF.Value;
            lblNourishFrac.Text = "Nourish casting time: " + (float)tbNourishCF.Value + "%.";
            Character.OnCalculationsInvalidated();
        }

        private void cbIgnoreNaturesGrace_CheckedChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.IgnoreNaturesGrace = cbIgnoreNaturesGrace.Checked;
            Character.OnCalculationsInvalidated();
        }

        private void tbRejuvRevitalize_Scroll(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            calcOpts.Current.RevitalizePPM = tbRevitalize.Value;
            lblRevitalize.Text = "Revitalize procs per minute: " + (float)calcOpts.Current.RevitalizePPM;
            Character.OnCalculationsInvalidated();
        }

        private bool isUniqueName(string name)
        {
            if (loading) return true;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;

            foreach (SpellProfile sp in calcOpts.Profiles)
            {
                if (sp.Name.Equals(name)) return false;
            }

            return true;
        }

        private void btnSpellProfileAdd_Click(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;

            if (!isUniqueName(calcOpts.Current.Name))
            {
                return;
            }

            SpellProfile profile = (SpellProfile)calcOpts.Current.Clone();
            calcOpts.Profiles.Add(profile);

            cbSpellProfiles.Items.Add(profile);
            cbSpellProfiles.SelectedItem = profile;
            calcOpts.Current = profile;
            btnSpellProfileAdd.Enabled = tbSpellProfileName.Text.Length > 0 && isUniqueName(tbSpellProfileName.Text);
            btnSpellProfileLoad.Enabled = cbSpellProfiles.SelectedItem != null;
            btnSpellProfileDelete.Enabled = cbSpellProfiles.SelectedItem != null;
        }

        private void btnSpellProfileDelete_Click(object sender, EventArgs e)
        {
            // Delete current selection, confirm button
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;

            if (cbSpellProfiles.SelectedItem != null)
            {
                SpellProfile item = (SpellProfile)cbSpellProfiles.SelectedItem;
                calcOpts.Profiles.Remove(item);
                cbSpellProfiles.Items.Remove(item);
                btnSpellProfileAdd.Enabled = tbSpellProfileName.Text.Length > 0 && isUniqueName(tbSpellProfileName.Text);
                btnSpellProfileLoad.Enabled = cbSpellProfiles.SelectedItem != null;
                btnSpellProfileDelete.Enabled = cbSpellProfiles.SelectedItem != null;
            }
        }

        private void tbSpellProfileName_TextChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            btnSpellProfileAdd.Enabled = tbSpellProfileName.Text.Length > 0 && isUniqueName(tbSpellProfileName.Text);
            calcOpts.Current.Name = tbSpellProfileName.Text;
        }

        private void cbSpellProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;

            btnSpellProfileLoad.Enabled = cbSpellProfiles.SelectedItem != null;
            btnSpellProfileAdd.Enabled = tbSpellProfileName.Text.Length > 0 && isUniqueName(tbSpellProfileName.Text);
            btnSpellProfileDelete.Enabled = cbSpellProfiles.SelectedItem != null;

            Character.OnCalculationsInvalidated();
        }

        private void btnSpellProfileLoad_Click(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;

            btnSpellProfileDelete.Enabled = cbSpellProfiles.SelectedItem != null;
            calcOpts.Current = ((SpellProfile)cbSpellProfiles.SelectedItem).Clone();

            loading = true;
            RefreshProfile();
            loading = false;

            btnSpellProfileAdd.Enabled = false;
            btnSpellProfileLoad.Enabled = cbSpellProfiles.SelectedItem != null;
            btnSpellProfileDelete.Enabled = cbSpellProfiles.SelectedItem != null;

            Character.OnCalculationsInvalidated();
        }
        
        private void btnOOMup_Click(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            if (lbOOMspells.SelectedItem == null) { return; }

            if (oomItems[lbOOMspells.SelectedIndex].Equals("Rejuvenation"))
            {
                calcOpts.Current.ReduceOOMRejuvOrder = Math.Max(0, calcOpts.Current.ReduceOOMRejuvOrder - 1);
                adjustManaRedrawAndSelect("Rejuvenation", calcOpts.Current);
            }
            else if (oomItems[lbOOMspells.SelectedIndex].Equals("Regrowth"))
            {
                calcOpts.Current.ReduceOOMRegrowthOrder = Math.Max(0, calcOpts.Current.ReduceOOMRegrowthOrder - 1);
                adjustManaRedrawAndSelect("Regrowth", calcOpts.Current);
            }
            else if (oomItems[lbOOMspells.SelectedIndex].Equals("Lifebloom"))
            {
                calcOpts.Current.ReduceOOMLifebloomOrder = Math.Max(0, calcOpts.Current.ReduceOOMLifebloomOrder - 1);
                adjustManaRedrawAndSelect("Lifebloom", calcOpts.Current);
            }
            else if (oomItems[lbOOMspells.SelectedIndex].Equals("Nourish"))
            {
                calcOpts.Current.ReduceOOMNourishOrder = Math.Max(0, calcOpts.Current.ReduceOOMNourishOrder - 1);
                adjustManaRedrawAndSelect("Nourish", calcOpts.Current);
            }
            else if (oomItems[lbOOMspells.SelectedIndex].Equals("Wild Growth"))
            {
                calcOpts.Current.ReduceOOMWildGrowthOrder = Math.Max(0, calcOpts.Current.ReduceOOMWildGrowthOrder - 1);
                adjustManaRedrawAndSelect("Wild Growth", calcOpts.Current);
            }

            Character.OnCalculationsInvalidated();
        }

        private void btnOOMdown_Click(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            if (lbOOMspells.SelectedItem == null) { return; }

            if (oomItems[lbOOMspells.SelectedIndex].Equals("Rejuvenation"))
            {
                calcOpts.Current.ReduceOOMRejuvOrder = Math.Min(4, calcOpts.Current.ReduceOOMRejuvOrder+1);
                adjustManaRedrawAndSelect("Rejuvenation", calcOpts.Current);
            }
            else if (oomItems[lbOOMspells.SelectedIndex].Equals("Regrowth"))
            {
                calcOpts.Current.ReduceOOMRegrowthOrder = Math.Min(4, calcOpts.Current.ReduceOOMRegrowthOrder + 1);
                adjustManaRedrawAndSelect("Regrowth", calcOpts.Current);
            }
            else if (oomItems[lbOOMspells.SelectedIndex].Equals("Lifebloom"))
            {
                calcOpts.Current.ReduceOOMLifebloomOrder = Math.Min(4, calcOpts.Current.ReduceOOMLifebloomOrder + 1);
                adjustManaRedrawAndSelect("Lifebloom", calcOpts.Current);
            }
            else if (oomItems[lbOOMspells.SelectedIndex].Equals("Nourish"))
            {
                calcOpts.Current.ReduceOOMNourishOrder = Math.Min(4, calcOpts.Current.ReduceOOMNourishOrder + 1);
                adjustManaRedrawAndSelect("Nourish", calcOpts.Current);
            }
            else if (oomItems[lbOOMspells.SelectedIndex].Equals("Wild Growth"))
            {
                calcOpts.Current.ReduceOOMWildGrowthOrder = Math.Min(4, calcOpts.Current.ReduceOOMWildGrowthOrder + 1);
                adjustManaRedrawAndSelect("Wild Growth", calcOpts.Current);
            }

            Character.OnCalculationsInvalidated();
        }

        private void lbOOMspells_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            if (lbOOMspells.SelectedItem == null) { return; }

            if (oomItems[lbOOMspells.SelectedIndex].Equals("Rejuvenation"))
            {
                tbManaAdjust.Enabled = true;
                tbManaAdjust.Value = calcOpts.Current.ReduceOOMRejuv;
            }
            else if (oomItems[lbOOMspells.SelectedIndex].Equals("Regrowth"))
            {
                tbManaAdjust.Enabled = true;
                tbManaAdjust.Value = calcOpts.Current.ReduceOOMRegrowth;
            }
            else if (oomItems[lbOOMspells.SelectedIndex].Equals("Lifebloom"))
            {
                tbManaAdjust.Enabled = true;
                tbManaAdjust.Value = calcOpts.Current.ReduceOOMLifebloom;
            }
            else if (oomItems[lbOOMspells.SelectedIndex].Equals("Nourish"))
            {
                tbManaAdjust.Enabled = true;
                tbManaAdjust.Value = calcOpts.Current.ReduceOOMNourish;
            }
            else if (oomItems[lbOOMspells.SelectedIndex].Equals("Wild Growth"))
            {
                tbManaAdjust.Enabled = true;
                tbManaAdjust.Value = calcOpts.Current.ReduceOOMWildGrowth;
            }
            else
            {
                tbManaAdjust.Enabled = false;
            }
        }

        private void tbOOMpercentage_Scroll(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            if (lbOOMspells.SelectedItem == null) { return; }

            if (oomItems[lbOOMspells.SelectedIndex].Equals("Rejuvenation"))
            {
                calcOpts.Current.ReduceOOMRejuv = tbManaAdjust.Value;
                adjustManaRedrawAndSelect("Rejuvenation", calcOpts.Current);
            }
            else if (oomItems[lbOOMspells.SelectedIndex].Equals("Regrowth"))
            {
                calcOpts.Current.ReduceOOMRegrowth = tbManaAdjust.Value;
                adjustManaRedrawAndSelect("Regrowth", calcOpts.Current);
                tbManaAdjust.Enabled = true;
                tbManaAdjust.Value = calcOpts.Current.ReduceOOMRegrowth;
            }
            else if (oomItems[lbOOMspells.SelectedIndex].Equals("Lifebloom"))
            {
                calcOpts.Current.ReduceOOMLifebloom = tbManaAdjust.Value;
                adjustManaRedrawAndSelect("Lifebloom", calcOpts.Current);
            }
            else if (oomItems[lbOOMspells.SelectedIndex].Equals("Nourish"))
            {
                calcOpts.Current.ReduceOOMNourish = tbManaAdjust.Value;
                adjustManaRedrawAndSelect("Nourish", calcOpts.Current);
            }
            else if (oomItems[lbOOMspells.SelectedIndex].Equals("Wild Growth"))
            {
                calcOpts.Current.ReduceOOMWildGrowth = tbManaAdjust.Value;
                adjustManaRedrawAndSelect("Wild Growth", calcOpts.Current);
            }
            else
            {
                tbManaAdjust.Enabled = false;
            } 
            
            Character.OnCalculationsInvalidated();
        }

        private List<String> oomItems;

        private void adjustManaRedrawAndSelect(String spell, SpellProfile profile)
        {
            oomItems = new List<String>();
            lbOOMspells.Items.Clear();
            int index = -1;
            for (int i = 1; i <= 5; i++)
            {
                int count = 0;
                if (profile.ReduceOOMRejuvOrder == i - 1)
                {
                    lbOOMspells.Items.Add((i.ToString() + ". ") + " Rejuvenation until " + profile.ReduceOOMRejuv + "%.");
                    oomItems.Add("Rejuvenation");
                    count++;
                    if (spell.Equals("Rejuvenation"))
                    {
                        index = lbOOMspells.Items.Count - 1;
                        tbManaAdjust.Enabled = true;
                        tbManaAdjust.Value = profile.ReduceOOMRejuv;
                    }
                }
                if (profile.ReduceOOMRegrowthOrder == i - 1)
                {
                    lbOOMspells.Items.Add((i.ToString() + ". ") + " Regrowth until " + profile.ReduceOOMRegrowth + "%.");
                    oomItems.Add("Regrowth");
                    count++;
                    if (spell.Equals("Regrowth"))
                    {
                        index = lbOOMspells.Items.Count - 1;
                        tbManaAdjust.Enabled = true;
                        tbManaAdjust.Value = profile.ReduceOOMRegrowth;
                    }
                }
                if (profile.ReduceOOMLifebloomOrder == i - 1)
                {
                    lbOOMspells.Items.Add((i.ToString() + ". ") + " Lifebloom until " + profile.ReduceOOMLifebloom + "%.");
                    oomItems.Add("Lifebloom");
                    count++;
                    if (spell.Equals("Lifebloom"))
                    {
                        index = lbOOMspells.Items.Count - 1;
                        tbManaAdjust.Enabled = true;
                        tbManaAdjust.Value = profile.ReduceOOMLifebloom;
                    }
                }
                if (profile.ReduceOOMNourishOrder == i - 1)
                {
                    lbOOMspells.Items.Add((i.ToString() + ". ") + " Nourish until " + profile.ReduceOOMNourish + "%.");
                    oomItems.Add("Nourish");
                    count++;
                    if (spell.Equals("Nourish"))
                    {
                        index = lbOOMspells.Items.Count - 1;
                        tbManaAdjust.Enabled = true;
                        tbManaAdjust.Value = profile.ReduceOOMNourish;
                    }
                }
                if (profile.ReduceOOMWildGrowthOrder == i - 1)
                {
                    lbOOMspells.Items.Add((i.ToString() + ". ") + " Wild Growth until " + profile.ReduceOOMWildGrowth + "%.");
                    oomItems.Add("Wild Growth");
                    count++;
                    if (spell.Equals("Wild Growth"))
                    {
                        index = lbOOMspells.Items.Count - 1;
                        tbManaAdjust.Enabled = true;
                        tbManaAdjust.Value = profile.ReduceOOMWildGrowth;
                    }
                }
            }

            if (index == -1)
            {
                tbManaAdjust.Enabled = false;
            }
            else
            {
                lbOOMspells.SelectedIndex = index;
            }
        }

        private void lbTimeAdjust_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            if (lbTimeAdjust.SelectedItem == null) { return; }

            if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Rejuvenation"))
            {
                tbTimeAdjust.Enabled = true;
                tbTimeAdjust.Value = calcOpts.Current.AdjustTimeRejuv;
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Regrowth"))
            {
                tbTimeAdjust.Enabled = true;
                tbTimeAdjust.Value = calcOpts.Current.AdjustTimeRegrowth;
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Nourish"))
            {
                tbTimeAdjust.Enabled = true;
                tbTimeAdjust.Value = calcOpts.Current.AdjustTimeNourish;
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Lifebloom"))
            {
                tbTimeAdjust.Enabled = true;
                tbTimeAdjust.Value = calcOpts.Current.AdjustTimeLifebloom;
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Swiftmend"))
            {
                tbTimeAdjust.Enabled = true;
                tbTimeAdjust.Value = calcOpts.Current.AdjustTimeSwiftmend;
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Wild Growth"))
            {
                tbTimeAdjust.Enabled = true;
                tbTimeAdjust.Value = calcOpts.Current.AdjustTimeWildGrowth;
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Idle Time"))
            {
                tbTimeAdjust.Enabled = true;
                tbTimeAdjust.Value = calcOpts.Current.AdjustTimeIdle;
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Managed Rejuvenation"))
            {
                tbTimeAdjust.Enabled = true;
                tbTimeAdjust.Value = calcOpts.Current.AdjustTimeManagedRejuv;
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Managed Regrowth"))
            {
                tbTimeAdjust.Enabled = true;
                tbTimeAdjust.Value = calcOpts.Current.AdjustTimeManagedRegrowth;
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Managed Lifebloom Stack"))
            {
                tbTimeAdjust.Enabled = true;
                tbTimeAdjust.Value = calcOpts.Current.AdjustTimeManagedLifebloomStack;
            }
        }

        private void btnTimeUp_Click(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            if (lbTimeAdjust.SelectedItem == null) { return; }

            if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Rejuvenation"))
            {
                calcOpts.Current.AdjustTimeRejuvOrder = Math.Max(0, calcOpts.Current.AdjustTimeRejuvOrder - 1);
                adjustTimeRedrawAndSelect("Rejuvenation", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Regrowth"))
            {
                calcOpts.Current.AdjustTimeRegrowthOrder = Math.Max(0, calcOpts.Current.AdjustTimeRegrowthOrder - 1);
                adjustTimeRedrawAndSelect("Regrowth", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Nourish"))
            {
                calcOpts.Current.AdjustTimeNourishOrder = Math.Max(0, calcOpts.Current.AdjustTimeNourishOrder - 1);
                adjustTimeRedrawAndSelect("Nourish", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Lifebloom"))
            {
                calcOpts.Current.AdjustTimeLifebloomOrder = Math.Max(0, calcOpts.Current.AdjustTimeLifebloomOrder - 1);
                adjustTimeRedrawAndSelect("Lifebloom", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Swiftmend"))
            {
                calcOpts.Current.AdjustTimeSwiftmendOrder = Math.Max(0, calcOpts.Current.AdjustTimeSwiftmendOrder - 1);
                adjustTimeRedrawAndSelect("Swiftmend", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Wild Growth"))
            {
                calcOpts.Current.AdjustTimeWildGrowthOrder = Math.Max(0, calcOpts.Current.AdjustTimeWildGrowthOrder - 1);
                adjustTimeRedrawAndSelect("Wild Growth", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Idle Time"))
            {
                calcOpts.Current.AdjustTimeIdleOrder = Math.Max(0, calcOpts.Current.AdjustTimeIdleOrder - 1);
                adjustTimeRedrawAndSelect("Idle Time", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Managed Rejuvenation"))
            {
                calcOpts.Current.AdjustTimeManagedRejuvOrder = Math.Max(0, calcOpts.Current.AdjustTimeManagedRejuvOrder - 1);
                adjustTimeRedrawAndSelect("Managed Rejuvenation", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Managed Regrowth"))
            {
                calcOpts.Current.AdjustTimeManagedRegrowthOrder = Math.Max(0, calcOpts.Current.AdjustTimeManagedRegrowthOrder - 1);
                adjustTimeRedrawAndSelect("Managed Regrowth", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Managed Lifebloom Stack"))
            {
                calcOpts.Current.AdjustTimeManagedLifebloomStackOrder = Math.Max(0, calcOpts.Current.AdjustTimeManagedLifebloomStackOrder - 1);
                adjustTimeRedrawAndSelect("Managed Lifebloom Stack", calcOpts.Current);
            }

            Character.OnCalculationsInvalidated();
        }

        private void btnTimeDown_Click(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            if (lbTimeAdjust.SelectedItem == null) { return; }

            if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Rejuvenation"))
            {
                calcOpts.Current.AdjustTimeRejuvOrder = Math.Min(9, calcOpts.Current.AdjustTimeRejuvOrder + 1);
                adjustTimeRedrawAndSelect("Rejuvenation", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Regrowth"))
            {
                calcOpts.Current.AdjustTimeRegrowthOrder = Math.Min(9, calcOpts.Current.AdjustTimeRegrowthOrder + 1);
                adjustTimeRedrawAndSelect("Regrowth", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Nourish"))
            {
                calcOpts.Current.AdjustTimeNourishOrder = Math.Min(9, calcOpts.Current.AdjustTimeNourishOrder + 1);
                adjustTimeRedrawAndSelect("Nourish", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Lifebloom"))
            {
                calcOpts.Current.AdjustTimeLifebloomOrder = Math.Min(9, calcOpts.Current.AdjustTimeLifebloomOrder + 1);
                adjustTimeRedrawAndSelect("Lifebloom", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Swiftmend"))
            {
                calcOpts.Current.AdjustTimeSwiftmendOrder = Math.Min(9, calcOpts.Current.AdjustTimeSwiftmendOrder + 1);
                adjustTimeRedrawAndSelect("Swiftmend", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Wild Growth"))
            {
                calcOpts.Current.AdjustTimeWildGrowthOrder = Math.Min(9, calcOpts.Current.AdjustTimeWildGrowthOrder + 1);
                adjustTimeRedrawAndSelect("Wild Growth", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Idle Time"))
            {
                calcOpts.Current.AdjustTimeIdleOrder = Math.Min(9, calcOpts.Current.AdjustTimeIdleOrder + 1);
                adjustTimeRedrawAndSelect("Idle Time", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Managed Rejuvenation"))
            {
                calcOpts.Current.AdjustTimeManagedRejuvOrder = Math.Min(9, calcOpts.Current.AdjustTimeManagedRejuvOrder + 1);
                adjustTimeRedrawAndSelect("Managed Rejuvenation", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Managed Regrowth"))
            {
                calcOpts.Current.AdjustTimeManagedRegrowthOrder = Math.Min(9, calcOpts.Current.AdjustTimeManagedRegrowthOrder + 1);
                adjustTimeRedrawAndSelect("Managed Regrowth", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Managed Lifebloom Stack"))
            {
                calcOpts.Current.AdjustTimeManagedLifebloomStackOrder = Math.Min(9, calcOpts.Current.AdjustTimeManagedLifebloomStackOrder + 1);
                adjustTimeRedrawAndSelect("Managed Lifebloom Stack", calcOpts.Current);
            }

            Character.OnCalculationsInvalidated();
        }

        private void tbTimeAdjust_Scroll(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;
            if (lbTimeAdjust.SelectedItem == null) { return; }

            if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Rejuvenation"))
            {
                calcOpts.Current.AdjustTimeRejuv = tbTimeAdjust.Value;
                adjustTimeRedrawAndSelect("Rejuvenation", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Regrowth"))
            {
                calcOpts.Current.AdjustTimeRegrowth = tbTimeAdjust.Value;
                adjustTimeRedrawAndSelect("Regrowth", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Nourish"))
            {
                calcOpts.Current.AdjustTimeNourish = tbTimeAdjust.Value;
                adjustTimeRedrawAndSelect("Nourish", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Lifebloom"))
            {
                calcOpts.Current.AdjustTimeLifebloom = tbTimeAdjust.Value;
                adjustTimeRedrawAndSelect("Lifebloom", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Swiftmend"))
            {
                calcOpts.Current.AdjustTimeSwiftmend = tbTimeAdjust.Value;
                adjustTimeRedrawAndSelect("Swiftmend", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Wild Growth"))
            {
                calcOpts.Current.AdjustTimeWildGrowth = tbTimeAdjust.Value;
                adjustTimeRedrawAndSelect("Wild Growth", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Idle Time"))
            {
                calcOpts.Current.AdjustTimeIdle = tbTimeAdjust.Value;
                adjustTimeRedrawAndSelect("Idle Time", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Managed Rejuvenation"))
            {
                calcOpts.Current.AdjustTimeManagedRejuv = tbTimeAdjust.Value;
                adjustTimeRedrawAndSelect("Managed Rejuvenation", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Managed Regrowth"))
            {
                calcOpts.Current.AdjustTimeManagedRegrowth = tbTimeAdjust.Value;
                adjustTimeRedrawAndSelect("Managed Regrowth", calcOpts.Current);
            }
            else if (timeItems[lbTimeAdjust.SelectedIndex].Equals("Managed Lifebloom Stack"))
            {
                calcOpts.Current.AdjustTimeManagedLifebloomStack = tbTimeAdjust.Value;
                adjustTimeRedrawAndSelect("Managed Lifebloom Stack", calcOpts.Current);
            }
            else
            {
                tbTimeAdjust.Enabled = false;
            }

            Character.OnCalculationsInvalidated();
        }

        private List<String> timeItems;

        private void adjustTimeRedrawAndSelect(String spell, SpellProfile profile)
        {
            timeItems = new List<String>();
            lbTimeAdjust.Items.Clear();
            int index = -1;

            for (int i = 1; i <= 10; i++)
            {
                int count = 0;
                if (profile.AdjustTimeRejuvOrder == i - 1)
                {
                    lbTimeAdjust.Items.Add((i.ToString() + ". ") + " Rejuvenation until " + profile.AdjustTimeRejuv + "%.");
                    timeItems.Add("Rejuvenation");
                    count++;
                    if (spell.Equals("Rejuvenation"))
                    {
                        index = lbTimeAdjust.Items.Count - 1;
                        tbTimeAdjust.Enabled = true;
                        tbTimeAdjust.Value = profile.AdjustTimeRejuv;
                    }
                }
                if (profile.AdjustTimeRegrowthOrder == i - 1)
                {
                    lbTimeAdjust.Items.Add((i.ToString() + ". ") + " Regrowth until " + profile.AdjustTimeRegrowth + "%.");
                    timeItems.Add("Regrowth");
                    count++;
                    if (spell.Equals("Regrowth"))
                    {
                        index = lbTimeAdjust.Items.Count - 1;
                        tbTimeAdjust.Enabled = true;
                        tbTimeAdjust.Value = profile.AdjustTimeRegrowth;
                    }
                }
                if (profile.AdjustTimeNourishOrder == i - 1)
                {
                    lbTimeAdjust.Items.Add((i.ToString() + ". ") + " Nourish until " + profile.AdjustTimeNourish + "%.");
                    timeItems.Add("Nourish");
                    count++;
                    if (spell.Equals("Nourish"))
                    {
                        index = lbTimeAdjust.Items.Count - 1;
                        tbTimeAdjust.Enabled = true;
                        tbTimeAdjust.Value = profile.AdjustTimeNourish;
                    }
                }
                if (profile.AdjustTimeLifebloomOrder == i - 1)
                {
                    lbTimeAdjust.Items.Add((i.ToString() + ". ") + " Lifebloom until " + profile.AdjustTimeLifebloom + "%.");
                    timeItems.Add("Lifebloom");
                    count++;
                    if (spell.Equals("Lifebloom"))
                    {
                        index = lbTimeAdjust.Items.Count - 1;
                        tbTimeAdjust.Enabled = true;
                        tbTimeAdjust.Value = profile.AdjustTimeLifebloom;
                    }
                }
                if (profile.AdjustTimeSwiftmendOrder == i - 1)
                {
                    lbTimeAdjust.Items.Add((i.ToString() + ". ") + " Swiftmend until " + profile.AdjustTimeSwiftmend + "%.");
                    timeItems.Add("Swiftmend");
                    count++;
                    if (spell.Equals("Swiftmend"))
                    {
                        index = lbTimeAdjust.Items.Count - 1;
                        tbTimeAdjust.Enabled = true;
                        tbTimeAdjust.Value = profile.AdjustTimeSwiftmend;
                    }
                }
                if (profile.AdjustTimeWildGrowthOrder == i - 1)
                {
                    lbTimeAdjust.Items.Add((i.ToString() + ". ") + " Wild Growth until " + profile.AdjustTimeWildGrowth + "%.");
                    timeItems.Add("Wild Growth");
                    count++;
                    if (spell.Equals("Wild Growth"))
                    {
                        index = lbTimeAdjust.Items.Count - 1;
                        tbTimeAdjust.Enabled = true;
                        tbTimeAdjust.Value = profile.AdjustTimeWildGrowth;
                    }
                }
                if (profile.AdjustTimeIdleOrder == i - 1)
                {
                    lbTimeAdjust.Items.Add((i.ToString() + ". ") + " Idle Time until " + profile.AdjustTimeIdle + "%.");
                    timeItems.Add("Idle Time");
                    count++;
                    if (spell.Equals("Idle Time"))
                    {
                        index = lbTimeAdjust.Items.Count - 1;
                        tbTimeAdjust.Enabled = true;
                        tbTimeAdjust.Value = profile.AdjustTimeIdle;
                    }
                }
                if (profile.AdjustTimeManagedRejuvOrder == i - 1)
                {
                    lbTimeAdjust.Items.Add((i.ToString() + ". ") + " Maintained Rejuvenation until " + profile.AdjustTimeManagedRejuv + "%.");
                    timeItems.Add("Managed Rejuvenation");
                    count++;
                    if (spell.Equals("Managed Rejuvenation"))
                    {
                        index = lbTimeAdjust.Items.Count - 1;
                        tbTimeAdjust.Enabled = true;
                        tbTimeAdjust.Value = profile.AdjustTimeManagedRejuv;
                    }
                }
                if (profile.AdjustTimeManagedRegrowthOrder == i - 1)
                {
                    lbTimeAdjust.Items.Add((i.ToString() + ". ") + " Maintained Regrowth until " + profile.AdjustTimeManagedRegrowth + "%.");
                    timeItems.Add("Managed Regrowth");
                    count++;
                    if (spell.Equals("Managed Regrowth"))
                    {
                        index = lbTimeAdjust.Items.Count - 1;
                        tbTimeAdjust.Enabled = true;
                        tbTimeAdjust.Value = profile.AdjustTimeManagedRegrowth;
                    }
                }
                if (profile.AdjustTimeManagedLifebloomStackOrder == i - 1)
                {
                    lbTimeAdjust.Items.Add((i.ToString() + ". ") + " Maintained Lifebloom Stack until " + profile.AdjustTimeManagedLifebloomStack + "%.");
                    timeItems.Add("Managed Lifebloom Stack");
                    count++;
                    if (spell.Equals("Managed Lifebloom Stack"))
                    {
                        index = lbTimeAdjust.Items.Count - 1;
                        tbTimeAdjust.Enabled = true;
                        tbTimeAdjust.Value = profile.AdjustTimeManagedLifebloomStack;
                    }
                }
            }

            if (index == -1)
            {
                tbTimeAdjust.Enabled = false;
            }
            else
            {
                lbTimeAdjust.SelectedIndex = index;
            }
        }

        private void rbProcsNone_CheckedChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;

            if (rbProcsNone.Checked)
            {
                calcOpts.ProcType = 0;
                rbProcsAverage.Checked = false;
                rbProcsMax.Checked = false;
            }

            Character.OnCalculationsInvalidated();
        }

        private void rbProcsAverage_CheckedChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;

            if (rbProcsAverage.Checked)
            {
                calcOpts.ProcType = 1;
                rbProcsNone.Checked = false;
                rbProcsMax.Checked = false;
            }

            Character.OnCalculationsInvalidated();
        }

        private void rbProcsMax_CheckedChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CalculationOptionsTree calcOpts = Character.CalculationOptions as CalculationOptionsTree;

            if (rbProcsMax.Checked)
            {
                calcOpts.ProcType = 2;
                rbProcsNone.Checked = false;
                rbProcsAverage.Checked = false;
            }

            Character.OnCalculationsInvalidated();
        }
    }
}
