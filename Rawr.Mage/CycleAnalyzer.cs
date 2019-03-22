using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr.Mage
{
    public partial class CycleAnalyzer : Form
    {
        Character character;
        CycleGenerator generator;
        CastingState castingState;
        Cycle wand;

        public CycleAnalyzer(Character character)
        {
            InitializeComponent();

            this.character = character;
            comboBoxCycleGenerator.SelectedIndex = 0;
        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            if (castingState == null || generator == null)
            {
                return;
            }
            string name = textBoxControlString.Text;
            if (name.Length != generator.ControlOptions.Length) return;

            for (int i = 0; i < generator.ControlOptions.Length; i++)
            {
                generator.ControlValue[i] = int.Parse(name[i].ToString());
            }

            try
            {
                GenericCycle generic = new GenericCycle(name, castingState, generator.StateList, true);

                StringBuilder sb = new StringBuilder();

                sb.AppendLine(generic.DamagePerSecond + " Dps");
                sb.AppendLine(generic.ManaPerSecond + " Mps");
                sb.AppendLine(generic.ThreatPerSecond + " Tps");

                sb.AppendLine();

                sb.AppendLine(generic.SpellDistribution);

                textBoxResult.Text = sb.ToString();
            }
            catch (OutOfMemoryException /*ex*/)
            {
                textBoxResult.Text = "State Space too complex to solve, please select a different cycle solver.";
            }
        }

        private void buttonOptimal_Click(object sender, EventArgs e)
        {
            if (castingState == null || generator == null)
            {
                return;
            }
            if (buttonOptimal.Text == "Cancel")
            {
                backgroundWorker.CancelAsync();
            }
            else
            {
                buttonOptimal.Text = "Cancel";
                backgroundWorker.RunWorkerAsync();
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = generator.Analyze(castingState, wand, backgroundWorker);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                StringBuilder sb = new StringBuilder();
                Cycle lastCycle = null;
                foreach (Cycle cycle in (List<Cycle>)e.Result)
                {
                    sb.Append(cycle.Name + ": " + cycle.DamagePerSecond + " dps, " + cycle.ManaPerSecond + " mps");
                    if (lastCycle != null)
                    {
                        sb.Append(", " + (cycle.DamagePerSecond - lastCycle.DamagePerSecond) / (cycle.ManaPerSecond - lastCycle.ManaPerSecond) + " dpm tradeoff");
                    }
                    sb.AppendLine();
                    lastCycle = cycle;
                }
                textBoxResult.Text = sb.ToString();
            }
            else
            {
                textBoxResult.Text = "";
            }
            buttonOptimal.Text = "Optimal";
            statusLabel.Text = "";
            statusProgressBar.Value = 0;
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            statusLabel.Text = (string)e.UserState;
            statusProgressBar.Value = e.ProgressPercentage;
        }

        private void comboBoxCycleGenerator_SelectedIndexChanged(object sender, EventArgs e)
        {
            string armor = "Molten Armor";
            CalculationOptionsMage calculationOptions = character.CalculationOptions as CalculationOptionsMage;
            CalculationsMage calculations = (CalculationsMage)Calculations.Instance;
            Solver solver = new Solver(character, calculationOptions, false, false, false, 0, armor, false, false, false, false);
            solver.Initialize(null);

            switch (comboBoxCycleGenerator.Text)
            {
                case "Arcane (MB/2T10 duration collapsed)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new ArcaneCycleGenerator(castingState, true, false, true, false, true, true);
                    break;
                case "Arcane (Arcane Power, MB/2T10 duration collapsed)":
                    castingState = new CastingState(solver, (int)StandardEffect.ArcanePower, false, 0);
                    generator = new ArcaneCycleGenerator(castingState, true, false, true, false, true, true);
                    break;
                case "Arcane (ABar on cooldown only, MB/2T10 duration/ABar cooldown collapsed)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new ArcaneCycleGenerator(castingState, true, true, true, true, true, true);
                    break;
                case "Arcane Beta":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new ArcaneCycleGeneratorBeta(castingState, true, true, true, true, true, true);
                    break;
                case "Arcane (no ABar, MB duration collapsed)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new ArcaneCycleGenerator(castingState, false, true, true, true, false, true);
                    break;
                case "Arcane Movement (average 1 per 10 sec for 1 sec)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new ArcaneMovementCycleGenerator(castingState, 0.1f, 1.0f, true, false, true, true, true, true);
                    break;
                case "Frost":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator(castingState, true, false);
                    break;
                case "Frost (no latency combos)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator(castingState, false, false);
                    break;
                case "Frost+Deep Freeze":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator2(castingState, true, true, 30.0f, false, false, false);
                    break;
                case "Frost+Deep Freeze (no latency combos)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator2(castingState, false, true, 30.0f, false, false, false);
                    break;
                case "Frost+Deep Freeze (2T10 duration collapsed)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator2(castingState, true, true, 30.0f, true, false, false);
                    break;
                case "Frost+Deep Freeze (2T10 duration collapsed, no latency combos)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator2(castingState, false, true, 30.0f, true, false, false);
                    break;
                case "Frost+Deep Freeze (FOF instants on last charge only, 2T10 duration collapsed)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator2(castingState, true, true, 30.0f, true, true, false);
                    break;
                case "Frost+Deep Freeze (FOF instants on last charge only, 2T10 duration collapsed, no latency combos)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator2(castingState, false, true, 30.0f, true, true, false);
                    break;
                case "Frost+Deep Freeze (FOF instants on last charge only, 2T10 duration collapsed, 10 sec Deep Freeze cooldown)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator2(castingState, true, true, 10.0f, true, true, false);
                    break;
                case "Frost+Deep Freeze (FOF instants on last charge only, 2T10 duration collapsed, 10 sec Deep Freeze cooldown, no latency combos)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator2(castingState, false, true, 10.0f, true, true, false);
                    break;
                case "FrostFFB+Deep Freeze":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator2(castingState, true, true, 30.0f, false, false, true);
                    break;
                case "FrostFFB+Deep Freeze (no latency combos)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator2(castingState, false, true, 30.0f, false, false, true);
                    break;
                case "FrostFFB+Deep Freeze (2T10 duration collapsed)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator2(castingState, true, true, 30.0f, true, false, true);
                    break;
                case "FrostFFB+Deep Freeze (2T10 duration collapsed, no latency combos)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator2(castingState, false, true, 30.0f, true, false, true);
                    break;
                case "FrostFFB+Deep Freeze (FOF instants on last charge only, 2T10 duration collapsed)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator2(castingState, true, true, 30.0f, true, true, true);
                    break;
                case "FrostFFB+Deep Freeze (FOF instants on last charge only, 2T10 duration collapsed, no latency combos)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator2(castingState, false, true, 30.0f, true, true, true);
                    break;
                case "FrostFFB+Deep Freeze (FOF instants on last charge only, 2T10 duration collapsed, 10 sec Deep Freeze cooldown)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator2(castingState, true, true, 10.0f, true, true, true);
                    break;
                case "FrostFFB+Deep Freeze (FOF instants on last charge only, 2T10 duration collapsed, 10 sec Deep Freeze cooldown, no latency combos)":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FrostCycleGenerator2(castingState, false, true, 10.0f, true, true, true);
                    break;
                case "Fire":
                    castingState = new CastingState(solver, 0, false, 0);
                    generator = new FireCycleGenerator(castingState);
                    break;
            }

            if (castingState == null || generator == null)
            {
                return;
            }

            if (character.Ranged != null)
            {
                wand = new WandTemplate(solver, (MagicSchool)character.Ranged.Item.DamageType, character.Ranged.Item.MinDamage, character.Ranged.Item.MaxDamage, character.Ranged.Item.Speed).GetSpell(castingState);
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(generator.StateDescription);

            sb.AppendLine("");
            for (int i = 0; i < generator.ControlOptions.Length; i++)
            {
                sb.Append(i);
                sb.Append(": ");
                sb.Append(generator.StateList[Array.IndexOf(generator.ControlIndex, i)]);
                sb.Append(": ");
                foreach (var kvp in generator.SpellMap[i])
                {
                    sb.Append(kvp.Value);
                    sb.Append("=");
                    sb.Append(kvp.Key);
                    sb.Append("  ");
                }
                sb.AppendLine();
            }

            textBoxDescription.Text = sb.ToString();
            textBoxControlString.Text = new string('0', generator.ControlOptions.Length);

            //textBoxControlString.SelectAll();
            textBoxControlString.Focus();

            buttonCalculate_Click(null, EventArgs.Empty);
        }

        private void CycleAnalyzer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();
                backgroundWorker.DoWork -= new DoWorkEventHandler(backgroundWorker_DoWork);
                backgroundWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
                backgroundWorker.ProgressChanged -= new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            }
        }
    }
}
