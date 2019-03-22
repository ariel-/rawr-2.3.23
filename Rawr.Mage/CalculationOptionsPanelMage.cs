using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace Rawr.Mage
{
	public partial class CalculationOptionsPanelMage : CalculationOptionsPanelBase
	{
		public CalculationOptionsPanelMage()
		{
			InitializeComponent();
            foreach (MIPMethod value in Enum.GetValues(typeof(MIPMethod)))
            {
                comboBoxMIPMethod.Items.Add(value);
            }
            if (Properties.Settings.Default.MaxThreads < 1)
            {
                Properties.Settings.Default.MaxThreads = Environment.ProcessorCount;
                Properties.Settings.Default.Save();
            }
            numericUpDownMaxThreads.Value = Properties.Settings.Default.MaxThreads;
            checkBoxDebugCooldownSegmentation.Checked = Properties.Settings.Default.DebugSegmentation;
            if (Type.GetType("Mono.Runtime") != null)
            {
                SetUpCheckStateFormatting(this);
            }
		}

        private void SetUpCheckStateFormatting(Control parent)
        {
            if (parent == null) return;
            if (parent is CheckBox)
            {
                foreach (Binding binding in parent.DataBindings)
                {
                    if (binding.PropertyName == "CheckState")
                    {
                        binding.Format += new ConvertEventHandler(BooleanToCheckState);
                        binding.Parse += new ConvertEventHandler(CheckStateToBoolean);
                    }
                }
            }
            foreach (Control child in parent.Controls)
            {
                SetUpCheckStateFormatting(child);
            }
        }

        void BooleanToCheckState(object sender, ConvertEventArgs e)
        {
            if (e.Value is bool)
            {
                e.Value = (bool)e.Value ? CheckState.Checked : CheckState.Unchecked;
            }
        }

        void CheckStateToBoolean(object sender, ConvertEventArgs e)
        {
            if (e.Value is CheckState)
            {
                e.Value = ((CheckState)e.Value == CheckState.Checked);
            }
        }

        private bool loading = false;

        private static CooldownRestrictionsForm cooldownRestrictions;

		protected override void LoadCalculationOptions()
		{
            //if (Character.MageTalents == null) Character.MageTalents = new MageTalents();
            if (Character.CalculationOptions == null) Character.CalculationOptions = new CalculationOptionsMage(Character);
            CalculationOptionsMage calculationOptions = Character.CalculationOptions as CalculationOptionsMage;
            if (calculationOptions.ArcaneResist > 0 && calculationOptions.ArcaneResist <= 1)
            {
                if (calculationOptions.ArcaneResist == 1) calculationOptions.ArcaneResist = -1;
                else calculationOptions.ArcaneResist *= 400;
            }
            if (calculationOptions.FireResist > 0 && calculationOptions.FireResist <= 1)
            {
                if (calculationOptions.FireResist == 1) calculationOptions.FireResist = -1;
                else calculationOptions.FireResist *= 400;
            }
            if (calculationOptions.FrostResist > 0 && calculationOptions.FrostResist <= 1)
            {
                if (calculationOptions.FrostResist == 1) calculationOptions.FrostResist = -1;
                else calculationOptions.FrostResist *= 400;
            }
            if (calculationOptions.ShadowResist > 0 && calculationOptions.ShadowResist <= 1)
            {
                if (calculationOptions.ShadowResist == 1) calculationOptions.ShadowResist = -1;
                else calculationOptions.ShadowResist *= 400;
            }
            if (calculationOptions.NatureResist > 0 && calculationOptions.NatureResist <= 1)
            {
                if (calculationOptions.NatureResist == 1) calculationOptions.NatureResist = -1;
                else calculationOptions.NatureResist *= 400;
            }
            if (calculationOptions.HolyResist > 0 && calculationOptions.HolyResist <= 1)
            {
                if (calculationOptions.HolyResist == 1) calculationOptions.HolyResist = -1;
                else calculationOptions.HolyResist *= 400;
            }

            loading = true;
            calculationOptionsMageBindingSource.DataSource = calculationOptions;

            //BindingSource bs = calculationOptionsMageBindingSource;
            //bs.AddingNew += (s, ev) => System.Diagnostics.Trace.WriteLine("AddingNew");
            //bs.BindingComplete += (s, ev) => System.Diagnostics.Trace.WriteLine("BindingComplete");
            //bs.CurrentChanged += (s, ev) => System.Diagnostics.Trace.WriteLine("CurrentChanged");
            //bs.CurrentItemChanged += (s, ev) => System.Diagnostics.Trace.WriteLine("CurrentItemChanged");
            //bs.DataError += (s, ev) => System.Diagnostics.Trace.WriteLine("DataError");
            //bs.DataMemberChanged += (s, ev) => System.Diagnostics.Trace.WriteLine("DataMemberChanged");
            //bs.DataSourceChanged += (s, ev) => System.Diagnostics.Trace.WriteLine("DataSourceChanged");
            //bs.ListChanged += (s, ev) => System.Diagnostics.Trace.WriteLine("ListChanged");
            //bs.PositionChanged += (s, ev) => System.Diagnostics.Trace.WriteLine("PositionChanged");

            if (cooldownRestrictions != null && !cooldownRestrictions.IsDisposed)
            {
                cooldownRestrictions.Character = Character;
                //cooldownRestrictions.bindingSourceCalculationOptions.DataSource = calculationOptions;
            }

            loading = false;

            SolverLogForm.Instance.ToString(); // force create it on main thread
        }

        private void checkBoxSMP_CheckedChanged(object sender, EventArgs e)
        {
            //DisplaySMPWarning();
        }

        private void checkBoxSMPDisplay_CheckedChanged(object sender, EventArgs e)
        {
            //DisplaySMPWarning();
        }

        private void DisplaySMPWarning()
        {
            if (!Properties.Settings.Default.DisplayedSMPWarning)
            {
                MessageBox.Show("Rawr detected that this is the first time you are using Segmented Multi-Pass (SMP) algorithm or Sequence Reconstruction." + Environment.NewLine + Environment.NewLine + "Sequence Reconstruction will perform best in combination with SMP algorithm. Since SMP algorithm can be computationally very expensive it is recommended to use SMP for display only and not for item comparisons. In some situations with many available cooldowns the SMP algorithm might have difficulties finding a solution. In such a case it will indicate it exceeded its computation limit and display the last working solution. Sequence Reconstruction will attempt to convert the optimum spell cycles into a timed sequence, but at the moment it will not consider timing dependencies between mana consumables and cooldowns, which means that in certain situations the reconstructions provided might be of low quality.", "Rawr.Mage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Properties.Settings.Default.DisplayedSMPWarning = true;
                Properties.Settings.Default.Save();
            }
        }

        private void calculationOptionsMageBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            if (!loading) Character.OnCalculationsInvalidated();
        }

        private void buttonCustomSpellMix_Click(object sender, EventArgs e)
        {
            CalculationOptionsMage calculationOptions = Character.CalculationOptions as CalculationOptionsMage;
            if (calculationOptions.CustomSpellMix == null) calculationOptions.CustomSpellMix = new List<SpellWeight>();
            CustomSpellMixForm form = new CustomSpellMixForm(calculationOptions.CustomSpellMix);
            form.ShowDialog(this);
        }

        private void checkBoxDebugCooldownSegmentation_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DebugSegmentation = checkBoxDebugCooldownSegmentation.Checked;
            Properties.Settings.Default.Save();
            CharacterCalculationsMage.DebugCooldownSegmentation = checkBoxDebugCooldownSegmentation.Checked;
            if (Character != null) Character.OnCalculationsInvalidated();
        }

        private void buttonCooldownRestrictionsEditor_Click(object sender, EventArgs e)
        {
            if (cooldownRestrictions == null || cooldownRestrictions.IsDisposed)
            {
                cooldownRestrictions = new CooldownRestrictionsForm();
                cooldownRestrictions.Character = Character;
                //cooldownRestrictions.bindingSourceCalculationOptions.DataSource = calculationOptions;
            }
            cooldownRestrictions.Show();
            cooldownRestrictions.BringToFront();
        }

        private void buttonAdvancedSolverLog_Click(object sender, EventArgs e)
        {
            SolverLogForm.Instance.Show();
            SolverLogForm.Instance.BringToFront();
        }

        private void numericUpDownMaxThreads_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MaxThreads = (int)numericUpDownMaxThreads.Value;
            Properties.Settings.Default.Save();
            ArrayPool.MaximumPoolSize = (int)numericUpDownMaxThreads.Value;
        }

        private void buttonHotStreakUtilization_Click(object sender, EventArgs e)
        {
            string armor = "Molten Armor";
            CalculationOptionsMage calculationOptions = Character.CalculationOptions as CalculationOptionsMage;
            CalculationsMage calculations = (CalculationsMage)Calculations.Instance;
            Solver solver = new Solver(Character, calculationOptions, false, false, false, 0, armor, false, false, true, false);
            solver.Initialize(null);
            CastingState baseState = new CastingState(solver, 0, false, 0);

            FireCycleGenerator generator = new FireCycleGenerator(baseState);            

            GenericCycle c1 = new GenericCycle("test", baseState, generator.StateList, true);
            Cycle c2 = baseState.GetCycle(CycleId.FBLBPyro);

            Dictionary<string, SpellContribution> dict1 = new Dictionary<string,SpellContribution>();
            Dictionary<string, SpellContribution> dict2 = new Dictionary<string,SpellContribution>();
            c1.AddDamageContribution(dict1, 1.0f);
            c2.AddDamageContribution(dict2, 1.0f);

            float predicted = dict2["Pyroblast"].Hits / dict2["Fireball"].Hits;
            float actual =  dict1["Pyroblast"].Hits / dict1["Fireball"].Hits;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Pyro/Nuke Ratio:");
            sb.AppendLine();
            sb.AppendLine("Approximation Model: " + predicted);
            sb.AppendLine("Exact Model: " + actual);
            sb.AppendLine();
            // predicted = raw * (1 - wastedold)
            // actual = raw * (1 - wasted)
            // wasted = 1 - actual / predicted * (1 - wastedold)
            sb.AppendLine("Predicted Wasted Hot Streaks: " + (1 - actual / predicted));

            MessageBox.Show(sb.ToString());
        }

        private void buttonCalculationTiming_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch clock = new System.Diagnostics.Stopwatch();
            CalculationsMage calculations = (CalculationsMage)Calculations.Instance;
            Character character = Character;
            clock.Start();
            for (int i = 0; i < 10000; i++)
            {
                calculations.GetCharacterCalculations(character);
            }
            clock.Stop();
            MessageBox.Show("Calculating 10000 characters takes " + clock.Elapsed.TotalSeconds + " seconds.");
        }

        private void buttonComputeOptimalCycles_Click(object sender, EventArgs e)
        {
            CycleAnalyzer analyzer = new CycleAnalyzer(Character);
            analyzer.Show();
        }
    }
}
