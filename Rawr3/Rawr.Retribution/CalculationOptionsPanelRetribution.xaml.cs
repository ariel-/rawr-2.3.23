using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rawr.Retribution
{
    public partial class CalculationOptionsPanelRetribution : UserControl, ICalculationOptionsPanel
    {
        public CalculationOptionsPanelRetribution()
        {
            InitializeComponent();
        }

        private bool isLoading = false;

        private CheckBox RotationCheckBox(int abilityIndex)
        {
            if (abilityIndex == 0) return SimulatorCheck1;
            else if (abilityIndex == 1) return SimulatorCheck2;
            else if (abilityIndex == 2) return SimulatorCheck3;
            else if (abilityIndex == 3) return SimulatorCheck4;
            else if (abilityIndex == 4) return SimulatorCheck5;
            else if (abilityIndex == 5) return SimulatorCheck6;
            else return null;
        }

        private TextBlock RotationLabel(int abilityIndex)
        {
            if (abilityIndex == 0) return SimulatorLabel1;
            else if (abilityIndex == 1) return SimulatorLabel2;
            else if (abilityIndex == 2) return SimulatorLabel3;
            else if (abilityIndex == 3) return SimulatorLabel4;
            else if (abilityIndex == 4) return SimulatorLabel5;
            else if (abilityIndex == 5) return SimulatorLabel6;
            else return null;
        }

        private void UpdateRotation()
        {
            isLoading = true;
            for (int i = 0; i < 6; i++)
            {
                // FIX
                //RotationCheckBox(i).IsChecked = CalcOpts.Selected[i];
                //RotationLabel(i).Text = RotationParameters.AbilityString(CalcOpts.Order[i]);
            }
            isLoading = false;
        }

        #region ICalculationOptionsPanel Members
        public UserControl PanelControl { get { return this; } }

        private CalculationOptionsRetribution CalcOpts
        {
            get { return DataContext as CalculationOptionsRetribution; }
        }

        private Character character;
        public Character Character
        {
            get
            {
                return character;
            }
            set
            {
                if (character != null && character.CalculationOptions != null
                    && character.CalculationOptions is CalculationOptionsRetribution)
                    ((CalculationOptionsRetribution)character.CalculationOptions).PropertyChanged
                        -= new PropertyChangedEventHandler(calcOpts_PropertyChanged);

                character = value;
                if (character.CalculationOptions == null)
                    character.CalculationOptions = new CalculationOptionsRetribution();

                CalculationOptionsRetribution calcOpts = character.CalculationOptions as CalculationOptionsRetribution;
                
                DataContext = calcOpts;
                UpdateRotation();

                calcOpts.PropertyChanged += new PropertyChangedEventHandler(calcOpts_PropertyChanged);
            }
        }

        private void calcOpts_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "EffectiveCD")
            {
                Character.OnCalculationsInvalidated();
            }
        }
        #endregion

        private void SimulatorCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!isLoading)
            {
                int abilityIndex = int.Parse(((StackPanel)((CheckBox)sender).Parent).Tag.ToString());
                // FIX
                //CalcOpts.Selected[abilityIndex] = ((CheckBox)sender).IsChecked.GetValueOrDefault();
                UpdateRotation();
                Character.OnCalculationsInvalidated();
            }
        }

        private void RotationUpClick(object sender, RoutedEventArgs e)
        {
            int abilityIndex = int.Parse(((StackPanel)RotationList.SelectedItem).Tag.ToString());
            if (abilityIndex > 0)
            {
                // FIX
                //Ability temp = CalcOpts.Order[abilityIndex];
                //CalcOpts.Order[abilityIndex] = CalcOpts.Order[abilityIndex - 1];
                //CalcOpts.Order[abilityIndex - 1] = temp;

                //bool tempSel = CalcOpts.Selected[abilityIndex];
                //CalcOpts.Selected[abilityIndex] = CalcOpts.Selected[abilityIndex - 1];
                //CalcOpts.Selected[abilityIndex - 1] = tempSel;

                RotationList.SelectedIndex = abilityIndex - 1;
                UpdateRotation();
                Character.OnCalculationsInvalidated();
            }
        }

        private void RotationDownClick(object sender, RoutedEventArgs e)
        {
            int abilityIndex = int.Parse(((StackPanel)RotationList.SelectedItem).Tag.ToString());
            if (abilityIndex < 5)
            {
                // FIX
                //Ability temp = CalcOpts.Order[abilityIndex];
                //CalcOpts.Order[abilityIndex] = CalcOpts.Order[abilityIndex + 1];
                //CalcOpts.Order[abilityIndex + 1] = temp;

                //bool tempSel = CalcOpts.Selected[abilityIndex];
                //CalcOpts.Selected[abilityIndex] = CalcOpts.Selected[abilityIndex + 1];
                //CalcOpts.Selected[abilityIndex + 1] = tempSel;

                RotationList.SelectedIndex = abilityIndex + 1;
                UpdateRotation();
                Character.OnCalculationsInvalidated();
            }
        }

        private void SetRotationControls(bool simulateRotation)
        {
            foreach (UIElement element in RotationGrid.Children)
            {
                if (element is Control) ((Control)element).IsEnabled = simulateRotation;
            }
            foreach (UIElement element in EffectiveCDGrid.Children)
            {
                if (element is Control) ((Control)element).IsEnabled = !simulateRotation;
            }
        }
    }
}
