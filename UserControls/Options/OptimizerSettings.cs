using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Rawr.UserControls.Options
{
	public partial class OptimizerSettings : UserControl, IOptions
	{
        public OptimizerSettings()
		{
			InitializeComponent();
			//cannot be in load, because its possible this tab won't show, and the values will not be initialized.
			//if this happens, then the users settings will be cleared.
            WarningsEnabledCheckBox.Checked = Properties.Optimizer.Default.WarningsEnabled;
            switch (Properties.Optimizer.Default.OptimizationMethod)
            {
                case OptimizationMethod.GeneticAlgorithm:
                    comboBoxOptimizationMethod.SelectedIndex = 0;
                    break;
                case OptimizationMethod.SimulatedAnnealing:
                    comboBoxOptimizationMethod.SelectedIndex = 1;
                    break;
            }
            switch (Properties.Optimizer.Default.GreedyOptimizationMethod)
            {
                case GreedyOptimizationMethod.AllCombinations:
                    comboBoxGreedyOptimizationMethod.SelectedIndex = 0;
                    break;
                case GreedyOptimizationMethod.SingleChanges:
                    comboBoxGreedyOptimizationMethod.SelectedIndex = 1;
                    break;
                case GreedyOptimizationMethod.GreedyBest:
                    comboBoxGreedyOptimizationMethod.SelectedIndex = 2;
                    break;
            }
            checkBoxTemplateGemsEnabled.Checked = Properties.Optimizer.Default.TemplateGemsEnabled;
        }


		#region IOptions Members

		public void Save()
		{
            Properties.Optimizer.Default.WarningsEnabled = WarningsEnabledCheckBox.Checked;
            switch (comboBoxOptimizationMethod.SelectedIndex)
            {
                case 0:
                    Properties.Optimizer.Default.OptimizationMethod = OptimizationMethod.GeneticAlgorithm;
                    break;
                case 1:
                    Properties.Optimizer.Default.OptimizationMethod = OptimizationMethod.SimulatedAnnealing;
                    break;
            }
            switch (comboBoxGreedyOptimizationMethod.SelectedIndex)
            {
                case 0:
                    Properties.Optimizer.Default.GreedyOptimizationMethod = GreedyOptimizationMethod.AllCombinations;
                    break;
                case 1:
                    Properties.Optimizer.Default.GreedyOptimizationMethod = GreedyOptimizationMethod.SingleChanges;
                    break;
                case 2:
                    Properties.Optimizer.Default.GreedyOptimizationMethod = GreedyOptimizationMethod.GreedyBest;
                    break;
            }
            Properties.Optimizer.Default.TemplateGemsEnabled = checkBoxTemplateGemsEnabled.Checked;
            Properties.Optimizer.Default.Save();
		}

		public void Cancel()
		{
			//NOOP;
		}

		public bool HasValidationErrors()
		{
			return CheckChildrenValidation(this);
		}

		private bool CheckChildrenValidation(Control control)
		{
			bool invalid = false;

			for (int i = 0; i < control.Controls.Count; i++)
			{
				if (!String.IsNullOrEmpty(errorProvider1.GetError(control.Controls[i])))
				{
					invalid = true;
					break;
				}
				else
				{
					invalid = CheckChildrenValidation(control.Controls[i]);
					if (invalid)
					{
						break;
					}
				}
			}

			return invalid;
		}

		public string DisplayName
		{
			get { return "Optimizer Settings"; }
		}


		public string TreePosition
		{
			get { return DisplayName; }
		}

		public Image MenuIcon
		{
			get { return null; }
		}

		#endregion
	}
}
