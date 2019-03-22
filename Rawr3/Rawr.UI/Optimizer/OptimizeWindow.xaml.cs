using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Rawr.Optimizer;
using Rawr.Properties;
using System.ComponentModel;
using System.Reflection;

namespace Rawr.UI
{
    public partial class OptimizeWindow : ChildWindow
    {

        private Character character;
        private ItemInstanceOptimizer optimizer;
        private Item itemToEvaluate;

        private string[] talentList;

        public void EvaluateUpgrades(Item itemToEvaluate)
        {
            this.itemToEvaluate = itemToEvaluate;
            UpgradesButton_Click(null, null);
            this.itemToEvaluate = null;
        }

        private void InitializeTalentList(Character character)
        {
            talentList = new string[character.CurrentTalents.Data.Length];
            foreach (PropertyInfo pi in character.CurrentTalents.GetType().GetProperties())
            {
                TalentDataAttribute[] talentDatas = pi.GetCustomAttributes(typeof(TalentDataAttribute), true) as TalentDataAttribute[];
                if (talentDatas.Length > 0)
                {
                    TalentDataAttribute talentData = talentDatas[0];
                    talentList[talentData.Index] = talentData.Name;
                }
            }
        }

        public OptimizeWindow(Character c)
        {
            InitializeTalentList(c);

            InitializeComponent();

            character = c;

            optimizer = new ItemInstanceOptimizer();
            optimizer.OptimizeCharacterProgressChanged += new OptimizeCharacterProgressChangedEventHandler(optimizer_OptimizeCharacterProgressChanged);
            optimizer.OptimizeCharacterCompleted += new OptimizeCharacterCompletedEventHandler(optimizer_OptimizeCharacterCompleted);
            optimizer.ComputeUpgradesProgressChanged += new ComputeUpgradesProgressChangedEventHandler(optimizer_ComputeUpgradesProgressChanged);
            optimizer.ComputeUpgradesCompleted += new ComputeUpgradesCompletedEventHandler(optimizer_ComputeUpgradesCompleted);

            List<string> calcsToOptimizeStrings = new List<string>();
            calcsToOptimizeStrings.Add("Overall Rating");
            foreach (string subpoints in Calculations.SubPointNameColors.Keys)
                calcsToOptimizeStrings.Add(subpoints + " Rating");
            CalculationToOptimizeCombo.Tag = Calculations.SubPointNameColors.Count;
            CalculationToOptimizeCombo.ItemsSource = calcsToOptimizeStrings;
            CalculationToOptimizeCombo.SelectedIndex = 0;

            ThoroughnessSlider.Value = OptimizerSettings.Default.Thoroughness;
            OverrideRegemCheck.IsChecked = OptimizerSettings.Default.OverrideRegem;
            OverrideReenchantCheck.IsChecked = OptimizerSettings.Default.OverrideReenchant;

            string calculationString = character.CalculationToOptimize;
            if (string.IsNullOrEmpty(calculationString)) calculationString = OptimizerSettings.Default.CalculationToOptimize;
            if (calculationString != null)
            {
                if (calculationString.StartsWith("[Overall]"))
                {
                    CalculationToOptimizeCombo.SelectedIndex = 0;
                }
                else if (calculationString.StartsWith("[SubPoint "))
                {
                    calculationString = calculationString.Substring(10).TrimEnd(']');
                    int index = int.Parse(calculationString);
                    if (index < Calculations.SubPointNameColors.Count)
                    {
                        CalculationToOptimizeCombo.SelectedIndex = index + 1;
                    }
                }
                else
                {
                    if (Array.IndexOf(Calculations.OptimizableCalculationLabels, calculationString) >= 0)
                    {
                        CalculationToOptimizeCombo.SelectedItem = calculationString;
                    }
                }
            }

            if (character.OptimizationRequirements != null)
                foreach (OptimizationRequirement requirement in character.OptimizationRequirements)
                    AddRequirement(requirement);
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = optimizer.IsBusy;
        }

        private void AddRequirement_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddRequirement(null);
        }

        private void AddRequirement(OptimizationRequirement requirement)
        {
            Grid requirementGrid = new Grid();
            requirementGrid.Style = Resources["RequirementGridStyle"] as Style;
            requirementGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            requirementGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            requirementGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            requirementGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            requirementGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            ComboBox requirementCalculationCombo = new ComboBox();
            requirementCalculationCombo.Style = Resources["RequirementComboStyle"] as Style;

            List<string> requirements = new List<string>();
            requirements.Add("Overall Rating");
            foreach (string subPoint in Calculations.SubPointNameColors.Keys)
                requirements.Add(subPoint + " Rating");
            requirements.AddRange(Calculations.OptimizableCalculationLabels);
            requirements.Add("Talent");
            requirements.Add("Cost");
            requirementCalculationCombo.ItemsSource = requirements;
            requirementCalculationCombo.Tag = Calculations.SubPointNameColors.Count;
            requirementCalculationCombo.Tag = 0;
            Grid.SetColumn(requirementCalculationCombo, 0);
            Grid.SetColumnSpan(requirementCalculationCombo, 2);
            requirementCalculationCombo.SelectionChanged += requirementCalculationCombo_SelectionChanged;
            requirementGrid.Children.Add(requirementCalculationCombo);

            ComboBox requirementTalentsCombo = new ComboBox();
            requirementTalentsCombo.Style = Resources["RequirementComboStyle"] as Style;
            requirementTalentsCombo.ItemsSource = talentList;
            requirementTalentsCombo.Visibility = Visibility.Collapsed;
            Grid.SetColumn(requirementTalentsCombo, 1);
            requirementGrid.Children.Add(requirementTalentsCombo);

            ComboBox requirementGreaterCombo = new ComboBox();
            requirementGreaterCombo.Style = Resources["RequirementGreaterComboStyle"] as Style;
            requirementGreaterCombo.ItemsSource = new string[] { "≥", "≤" };
            requirementGreaterCombo.Tag = 1;
            Grid.SetColumn(requirementGreaterCombo, 2);
            requirementGrid.Children.Add(requirementGreaterCombo);

            NumericUpDown requirementNum = new NumericUpDown();
            requirementNum.Style = Resources["RequirementNumStyle"] as Style;
            requirementNum.Tag = 2;
            Grid.SetColumn(requirementNum, 3);
            requirementGrid.Children.Add(requirementNum);

            Button requirementRemoveButton = new Button();
            requirementRemoveButton.Style = Resources["RequirementRemoveStyle"] as Style;
            requirementRemoveButton.Click += new RoutedEventHandler(RemoveRequirement_Click);
            Grid.SetColumn(requirementRemoveButton, 4);
            requirementGrid.Children.Add(requirementRemoveButton);

            if (requirement == null)
            {
                requirementCalculationCombo.SelectedIndex = 0;
                requirementGreaterCombo.SelectedIndex = 0;
                requirementNum.Value = 0;
            }
            else
            {
                string calculationString = requirement.Calculation;
                if (calculationString.StartsWith("[Overall]", StringComparison.Ordinal))
                {
                    requirementCalculationCombo.SelectedIndex = 0;
                }
                else if (calculationString.StartsWith("[SubPoint ", StringComparison.Ordinal))
                {
                    calculationString = calculationString.Substring(10).TrimEnd(']');
                    int index = int.Parse(calculationString);
                    if (index < Calculations.SubPointNameColors.Count)
                    {
                        requirementCalculationCombo.SelectedIndex = index + 1;
                    }
                }
                else if (calculationString.StartsWith("[Talent ", StringComparison.Ordinal))
                {
                    requirementCalculationCombo.SelectedItem = "Talent";
                    string talent = calculationString.Substring(8).TrimEnd(']');
                    int index = int.Parse(talent);
                    if (index < talentList.Length)
                    {
                        requirementTalentsCombo.SelectedIndex = index;
                    }
                }
                else if (calculationString.StartsWith("[Cost]", StringComparison.Ordinal))
                {
                    requirementCalculationCombo.SelectedItem = "Cost";
                }
                else
                {
                    if (Array.IndexOf(Calculations.OptimizableCalculationLabels, calculationString) >= 0)
                    {
                        requirementCalculationCombo.SelectedItem = calculationString;
                    }
                }
                requirementGreaterCombo.SelectedIndex = requirement.LessThan ? 1 : 0;
                requirementNum.Value = (double)requirement.Value;
            }

            RequirementsStack.Children.Add(requirementGrid);
        }

        void requirementCalculationCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox requirementCalculationCombo = (ComboBox)sender;
            Grid grid = (Grid)requirementCalculationCombo.Parent;
            ComboBox requirementTalentsCombo = (ComboBox)grid.Children[1];
            if ((string)requirementCalculationCombo.SelectedItem == "Talent")
            {
                Grid.SetColumnSpan(requirementCalculationCombo, 1);
                requirementTalentsCombo.Visibility = Visibility.Visible;
            }
            else
            {
                Grid.SetColumnSpan(requirementCalculationCombo, 2);
                requirementTalentsCombo.Visibility = Visibility.Collapsed;
            }
        }

        private void RemoveRequirement_Click(object sender, RoutedEventArgs e)
        {
            Grid grid = (Grid)((Button)sender).Parent;
            ComboBox requirementCalculationCombo = (ComboBox)grid.Children[0];
            requirementCalculationCombo.SelectionChanged -= requirementCalculationCombo_SelectionChanged;

            RequirementsStack.Children.Remove(grid);
        }

        private List<TalentsBase> GetOptimizeTalentSpecs()
        {
            List<TalentsBase> talentSpecs = null;
            if (TalentsCheck.IsChecked.GetValueOrDefault(false) || TalentSpecsCheck.IsChecked.GetValueOrDefault(false))
            {
                talentSpecs = new List<TalentsBase>();
                foreach (SavedTalentSpec spec in SavedTalentSpec.SpecsFor(character.Class))
                {
                    TalentsBase talents = spec.TalentSpec();
                    int totalPoints = 0;
                    for (int i = 0; i < talents.Data.Length; i++)
                    {
                        totalPoints += talents.Data[i];
                    }
                    if (totalPoints == character.Level - 9)
                    {
                        talentSpecs.Add(talents);
                    }
                }
            }
            return talentSpecs;
        }

        private string GetCalculationStringFromComboBox(ComboBox comboBox, ComboBox comboBoxTalent)
        {
            if (comboBox.SelectedIndex == 0)
                return "[Overall]";
            else if (comboBox.SelectedIndex <= (int)comboBox.Tag)
                return string.Format("[SubPoint {0}]", comboBox.SelectedIndex - 1);
            else if ((string)comboBox.SelectedItem == "Talent")
                return string.Format("[Talent {0}]", comboBoxTalent.SelectedIndex);
            else if ((string)comboBox.SelectedItem == "Cost")
                return "[Cost]";
            else
                return comboBox.SelectedItem as string;
        }

        private void ControlsEnabled(bool enabled)
        {
            OptimizeButton.IsEnabled = UpgradesButton.IsEnabled = OverrideRegemCheck.IsEnabled =
                OverrideReenchantCheck.IsEnabled = ThoroughnessSlider.IsEnabled = MixologyCheck.IsEnabled =
                ElixirsFlasksCheck.IsEnabled = FoodCheck.IsEnabled = TalentsCheck.IsEnabled = TalentSpecsCheck.IsEnabled =
                CalculationToOptimizeCombo.IsEnabled = AddRequirementButton.IsEnabled = enabled;
            MaxScoreLabel.Text = string.Empty;
            AltProgress.Value = MainProgress.Value = 0;
            Title = "Optimizer";
            if (enabled)
            {
                DoneButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                DoneButton.Visibility = Visibility.Collapsed;
                CancelButton.Visibility = Visibility.Visible;
            }
            foreach (UIElement requirementGrid in RequirementsStack.Children)
            {
                foreach (UIElement requirementControl in ((Grid)requirementGrid).Children)
                {
                    if (requirementControl is Control)
                    {
                        ((Control)requirementControl).IsEnabled = enabled;
                    }
                }
            }
        }

        private List<OptimizationRequirement> GetOptimizationRequirements()
        {
            bool ignore;
            return GetOptimizationRequirements(out ignore);
        }

        private List<OptimizationRequirement> GetOptimizationRequirements(out bool costRequirement)
        {
            costRequirement = false;
            List<OptimizationRequirement> requirements = new List<OptimizationRequirement>();
            foreach (UIElement requirementGrid in RequirementsStack.Children)
            {
                Grid grid = requirementGrid as Grid;
                if (grid != null)
                {
                    OptimizationRequirement requirement = new OptimizationRequirement();
                    requirement.Calculation = GetCalculationStringFromComboBox(grid.Children[0] as ComboBox, grid.Children[1] as ComboBox);
                    requirement.LessThan = (grid.Children[2] as ComboBox).SelectedIndex == 1;
                    requirement.Value = (float)((grid.Children[3] as NumericUpDown).Value);
                    requirements.Add(requirement);
                    if (requirement.Calculation == "[Cost]")
                    {
                        costRequirement = true;
                    }
                }
            }
            return requirements;
        }

        private void optimizer_OptimizeCharacterProgressChanged(object sender, OptimizeCharacterProgressChangedEventArgs e)
        {
            MaxScoreLabel.Text = e.BestValue.ToString();
            AltProgress.Value = e.ProgressPercentage;
            MainProgress.Value = Math.Max(e.ProgressPercentage, MainProgress.Value);

            Title = string.Format("Optimizer - {0}% Complete", MainProgress.Value);
        }

        private void optimizer_ComputeUpgradesProgressChanged(object sender, ComputeUpgradesProgressChangedEventArgs e)
        {
            MaxScoreLabel.Text = e.CurrentItem;
            AltProgress.Value = e.ItemProgressPercentage;
            MainProgress.Value = e.ProgressPercentage;

            Title = string.Format("Optimizer - {0}% Complete", MainProgress.Value);
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            OptimizerSettings.Default.OverrideRegem = OverrideRegemCheck.IsChecked.GetValueOrDefault();
            OptimizerSettings.Default.OverrideReenchant = OverrideReenchantCheck.IsChecked.GetValueOrDefault();
            OptimizerSettings.Default.Thoroughness = (int)ThoroughnessSlider.Value;
            OptimizerSettings.Default.CalculationToOptimize = GetCalculationStringFromComboBox(CalculationToOptimizeCombo, null);
            character.OptimizationRequirements = GetOptimizationRequirements();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (optimizer.IsBusy) optimizer.CancelAsync();
        }

        private void OptimizeButton_Click(object sender, RoutedEventArgs e)
        {
            bool overrideRegem = OverrideRegemCheck.IsChecked.GetValueOrDefault();
            bool overrideReenchant = OverrideReenchantCheck.IsChecked.GetValueOrDefault();
            int thoroughness = (int)ThoroughnessSlider.Value;
            string calculationToOptimize = GetCalculationStringFromComboBox(CalculationToOptimizeCombo, null);
            bool costRequirement;
            List<OptimizationRequirement> requirements = GetOptimizationRequirements(out costRequirement);

            optimizer.OptimizationMethod = OptimizerSettings.Default.OptimizationMethod;
            optimizer.GreedyOptimizationMethod = OptimizerSettings.Default.GreedyOptimizationMethod;

            optimizer.InitializeItemCache(character, character.AvailableItems, overrideRegem, overrideReenchant,
                OptimizerSettings.Default.TemplateGemsEnabled, Calculations.Instance,
                FoodCheck.IsChecked.GetValueOrDefault(), ElixirsFlasksCheck.IsChecked.GetValueOrDefault(),
                MixologyCheck.IsChecked.GetValueOrDefault(), GetOptimizeTalentSpecs(), TalentsCheck.IsChecked.GetValueOrDefault(), costRequirement);

            if (OptimizerSettings.Default.WarningsEnabled)
            {
                string prompt = optimizer.GetWarningPromptIfNeeded();
                if (prompt != null)
                {
                    if (MessageBox.Show(prompt, "Optimizer Warning", MessageBoxButton.OKCancel) != MessageBoxResult.OK) { ControlsEnabled(true); return; }
                }
                prompt =optimizer.CheckOneHandedWeaponUniqueness();
                if (prompt != null)
                {
                    if (MessageBox.Show(prompt, "Optimizer Warning", MessageBoxButton.OKCancel) != MessageBoxResult.OK) { ControlsEnabled(true); return; }
                }
                if (!optimizer.ItemGenerator.IsCharacterValid(character, out prompt, true))
                {
                    if (MessageBox.Show(prompt, "Optimizer Warning", MessageBoxButton.OKCancel) != MessageBoxResult.OK) { ControlsEnabled(true); return; }
                }
            }

            CancelButton.Visibility = Visibility.Visible;
            DoneButton.Visibility = Visibility.Collapsed;

            ControlsEnabled(false);
            optimizer.OptimizeCharacterAsync(character, calculationToOptimize, requirements, thoroughness, false);
        }

        private void optimizer_OptimizeCharacterCompleted(object sender, OptimizeCharacterCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MaxScoreLabel.Text = string.Empty;
                ControlsEnabled(true);
                AltProgress.Value = MainProgress.Value = 0;
            }
            else
            {
                AltProgress.Value = MainProgress.Value = 100;
                Character bestCharacter = e.OptimizedCharacter;
                if (bestCharacter == null)
                {
                    ControlsEnabled(true);
                    MessageBox.Show("Sorry, Rawr was unable to find a gearset to meet your requirements.", "Rawr Optimizer Results", MessageBoxButton.OK);
                }

                if (character != null)
                {
                    OptimizerResults results = new OptimizerResults(character, bestCharacter);
                    results.Closed += new EventHandler(results_Closed);
                    results.Show();
                }
                else ControlsEnabled(true);
            }
        }

        private void results_Closed(object sender, EventArgs e)
        {
            OptimizerResults results = sender as OptimizerResults;
            if (results.DialogResult.GetValueOrDefault())
            {
                character.IsLoading = true;
                character.SetItems(results.BestCharacter);
                character.ActiveBuffs = results.BestCharacter.ActiveBuffs;
                if (TalentsCheck.IsChecked.GetValueOrDefault())
                {
                    character.CurrentTalents = results.BestCharacter.CurrentTalents;
                    MainPage.Instance.TalentPicker.RefreshSpec();
                }
                character.IsLoading = false;
                character.OnCalculationsInvalidated();
                DialogResult = true;
            }
            else ControlsEnabled(true);
        }

        private void UpgradesButton_Click(object sender, RoutedEventArgs e)
        {
            bool overrideRegem = OverrideRegemCheck.IsChecked.GetValueOrDefault();
            bool overrideReenchant = OverrideReenchantCheck.IsChecked.GetValueOrDefault();
            int thoroughness = (int)Math.Ceiling((float)ThoroughnessSlider.Value / 10f);
            string calculationToOptimize = GetCalculationStringFromComboBox(CalculationToOptimizeCombo, null);
            List<OptimizationRequirement> requirements = GetOptimizationRequirements();

            if ((overrideReenchant || overrideRegem || thoroughness > 100) && OptimizerSettings.Default.WarningsEnabled)
            {
                if (MessageBox.Show("The upgrade evaluations perform an optimization for each relevant item. With your settings this might take a long time. Consider using lower thoroughness and no overriding of regem and reenchant options." + Environment.NewLine + Environment.NewLine + "Do you want to continue with upgrade evaluations?", "Optimizer Warning", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            optimizer.OptimizationMethod = OptimizerSettings.Default.OptimizationMethod;
            optimizer.GreedyOptimizationMethod = OptimizerSettings.Default.GreedyOptimizationMethod;

            optimizer.InitializeItemCache(character, character.AvailableItems, overrideRegem, overrideReenchant,
                OptimizerSettings.Default.TemplateGemsEnabled, Calculations.Instance,
                FoodCheck.IsChecked.GetValueOrDefault(), ElixirsFlasksCheck.IsChecked.GetValueOrDefault(),
                MixologyCheck.IsChecked.GetValueOrDefault(), GetOptimizeTalentSpecs(), false);
            if (OptimizerSettings.Default.WarningsEnabled)
            {
                string prompt = optimizer.GetWarningPromptIfNeeded();
                if (prompt != null)
                {
                    if (MessageBox.Show(prompt, "Optimizer Warning", MessageBoxButton.OKCancel) != MessageBoxResult.OK) { ControlsEnabled(true); return; }
                }
            }

            ControlsEnabled(false);

            optimizer.ComputeUpgradesAsync(character, calculationToOptimize, requirements, thoroughness, itemToEvaluate);
        }

        private void optimizer_ComputeUpgradesCompleted(object sender, ComputeUpgradesCompletedEventArgs e)
        {
            if (e.Cancelled) ControlsEnabled(true);
            else
            {
                AltProgress.Value = MainProgress.Value = 100;
                UpgradesComparison upgrades = new UpgradesComparison(e.Upgrades, null);
                Close();
                upgrades.Show();
            }
        }

    }
}

