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

namespace Rawr.Healadin
{
    public partial class CalculationOptionsPanelHealadin : UserControl, ICalculationOptionsPanel

    {
        public CalculationOptionsPanelHealadin()
        {
            InitializeComponent();
        }

        #region ICalculationOptionsPanel Members
        public UserControl PanelControl { get { return this; } }

        CalculationOptionsHealadin calcOpts = null;

        private Character character;
        public Character Character
        {
            get { return character; }
            set
            {
                // Kill any old event connections
                if (character != null && character.CalculationOptions != null
                    && character.CalculationOptions is CalculationOptionsHealadin)
                    ((CalculationOptionsHealadin)character.CalculationOptions).PropertyChanged
                        -= new PropertyChangedEventHandler(CalculationOptionsPanelHealadin_PropertyChanged);
                // Apply the new character
                character = value;
                // Load the new CalcOpts
                LoadCalculationOptions();
                // Model Specific Code
                // Set the Data Context
                LayoutRoot.DataContext = calcOpts;
                // Add new event connections
                calcOpts.PropertyChanged += new PropertyChangedEventHandler(CalculationOptionsPanelHealadin_PropertyChanged);
                // Run it once for any special UI config checks
                CalculationOptionsPanelHealadin_PropertyChanged(null, new PropertyChangedEventArgs(""));
            }
        }

        private bool _loadingCalculationOptions;
        public void LoadCalculationOptions()
        {
            _loadingCalculationOptions = true;
            if (Character.CalculationOptions == null) Character.CalculationOptions = new CalculationOptionsHealadin();
            calcOpts = Character.CalculationOptions as CalculationOptionsHealadin;
            // Model Specific Code
            //
            _loadingCalculationOptions = false;
        }

        void CalculationOptionsPanelHealadin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_loadingCalculationOptions) { return; }
            // This would handle any special changes, especially combobox assignments, but not when the pane is trying to load
            if (e.PropertyName == "SomeProperty")
            {
                // Do some code
            }
            //
            if (Character != null) { Character.OnCalculationsInvalidated(); }
        }
        #endregion

        private void btnResetBurstScale_Click(object sender, RoutedEventArgs e) {
            calcOpts.BurstScale = .4f;
        }

        private void btnResetActivity_Click(object sender, RoutedEventArgs e) {
            calcOpts.Activity = .85f;
        }

        private void btnResetHolyShock_Click(object sender, RoutedEventArgs e) {
            calcOpts.HolyShock = .15f;
        }

        private void btnResetIoLHolyLight_Click(object sender, RoutedEventArgs e) {
            calcOpts.IoLHolyLight = .9f;
        }

        private void btnResetSSUptime_Click(object sender, RoutedEventArgs e) {
            calcOpts.SSUptime = 1f;
        }

        private void btnResetFoLOnTank_Click(object sender, RoutedEventArgs e) {
            calcOpts.FoLOnTank = .25f;
        }

        private void btnResetReplenishment_Click(object sender, RoutedEventArgs e) {
            calcOpts.Replenishment = .9f;
        }

        private void btnResetBoLUp_Click(object sender, RoutedEventArgs e) {
            calcOpts.BoLUp = 1f;
        }
    }
}
