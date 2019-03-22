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

namespace Rawr.ShadowPriest
{
    public partial class CalculationOptionsPanelShadowPriest : UserControl, ICalculationOptionsPanel
    {
        public CalculationOptionsPanelShadowPriest()
        {
            InitializeComponent();
        }

        #region ICalculationOptionsPanel Members
        public UserControl PanelControl { get { return this; } }

        CalculationOptionsShadowPriest calcOpts = null;

        private Character character;
        public Character Character
        {
            get { return character; }
            set
            {
                // Kill any old event connections
                if (character != null && character.CalculationOptions != null
                    && character.CalculationOptions is CalculationOptionsShadowPriest)
                    ((CalculationOptionsShadowPriest)character.CalculationOptions).PropertyChanged
                        -= new PropertyChangedEventHandler(CalculationOptionsPanelShadowPriest_PropertyChanged);
                // Apply the new character
                character = value;
                // Load the new CalcOpts
                LoadCalculationOptions();
                // Model Specific Code
                // Set the Data Context
                LayoutRoot.DataContext = calcOpts;
                // Add new event connections
                calcOpts.PropertyChanged += new PropertyChangedEventHandler(CalculationOptionsPanelShadowPriest_PropertyChanged);
                // Run it once for any special UI config checks
                CalculationOptionsPanelShadowPriest_PropertyChanged(null, new PropertyChangedEventArgs(""));
            }
        }

        private bool _loadingCalculationOptions;
        public void LoadCalculationOptions()
        {
            _loadingCalculationOptions = true;
            if (Character.CalculationOptions == null) Character.CalculationOptions = new CalculationOptionsShadowPriest();
            calcOpts = Character.CalculationOptions as CalculationOptionsShadowPriest;
            // Model Specific Code
            if (calcOpts.SpellPriority == null)
                calcOpts.SpellPriority = new List<string>(Spell.ShadowSpellList);
            //
            _loadingCalculationOptions = false;
        }

        void CalculationOptionsPanelShadowPriest_PropertyChanged(object sender, PropertyChangedEventArgs e)
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

        SpellPriorityForm priority = null;
        private void bChangePriority_Click(object sender, RoutedEventArgs e)
        {
            priority = new SpellPriorityForm(calcOpts.SpellPriority, lsSpellPriopity, Character);
            priority.Closed += new EventHandler(bChangePriority_Closed);
            priority.Show();
        }
        private void bChangePriority_Closed(object sender, EventArgs e) {
            if (priority.DialogResult.GetValueOrDefault(false)) {
                List<string> newList = new List<string>();
                foreach (Object o in priority.lsSpellPriority.Items) {
                    newList.Add(o.ToString());
                }
                calcOpts.SpellPriority = newList;
                //character.OnCalculationsInvalidated();
            }
        }
    }
}
