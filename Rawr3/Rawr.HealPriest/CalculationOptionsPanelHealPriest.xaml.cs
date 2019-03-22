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

namespace Rawr.HealPriest
{
    public partial class CalculationOptionsPanelHealPriest : UserControl, ICalculationOptionsPanel
    {
        public CalculationOptionsPanelHealPriest()
        {
            InitializeComponent();
        }

        #region ICalculationOptionsPanel Members
        public UserControl PanelControl { get { return this; } }

        CalculationOptionsHealPriest calcOpts = null;

        private Character character;
        public Character Character
        {
            get { return character; }
            set
            {
                // Kill any old event connections
                if (character != null && character.CalculationOptions != null
                    && character.CalculationOptions is CalculationOptionsHealPriest)
                    ((CalculationOptionsHealPriest)character.CalculationOptions).PropertyChanged
                        -= new PropertyChangedEventHandler(CalculationOptionsPanelHealPriest_PropertyChanged);
                // Apply the new character
                character = value;
                // Load the new CalcOpts
                LoadCalculationOptions();
                // Model Specific Code
                // Set the Data Context
                LayoutRoot.DataContext = calcOpts;
                // Add new event connections
                calcOpts.PropertyChanged += new PropertyChangedEventHandler(CalculationOptionsPanelHealPriest_PropertyChanged);
                // Run it once for any special UI config checks
                CalculationOptionsPanelHealPriest_PropertyChanged(null, new PropertyChangedEventArgs(""));
            }
        }

        private bool _loadingCalculationOptions;
        public void LoadCalculationOptions()
        {
            _loadingCalculationOptions = true;
            if (Character.CalculationOptions == null) Character.CalculationOptions = new CalculationOptionsHealPriest();
            calcOpts = Character.CalculationOptions as CalculationOptionsHealPriest;
            // Model Specific Code
            //
            _loadingCalculationOptions = false;
            CalculationOptionsPanelHealPriest_PropertyChanged(null, new PropertyChangedEventArgs("Role"));
        }

        void CalculationOptionsPanelHealPriest_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_loadingCalculationOptions) { return; }
            // This would handle any special changes, especially combobox assignments, but not when the pane is trying to load
            if (e.PropertyName == "Role") {
                // Do some code
                bool isCustom = calcOpts.Role == eRole.CUSTOM;
                numFlashHealCast.IsEnabled = isCustom;
                numBindingHealCast.IsEnabled = isCustom;
                numGreaterHealCast.IsEnabled = isCustom;
                numPenanceCast.IsEnabled = isCustom;
                numRenewCast.IsEnabled = isCustom; numRenewTicks.IsEnabled = isCustom;
                numProMCast.IsEnabled = isCustom; numProMTicks.IsEnabled = isCustom;
                numPoHCast.IsEnabled = isCustom;
                numPWSCast.IsEnabled = isCustom;
                numCoHCast.IsEnabled = isCustom;
                numHolyNovaCast.IsEnabled = isCustom;
                numDivineHymnCast.IsEnabled = isCustom;
                numDispelCast.IsEnabled = isCustom;
                numMDCast.IsEnabled = isCustom;
            }
            //
            if (Character != null) { Character.OnCalculationsInvalidated(); }
        }
        #endregion
    }
}
