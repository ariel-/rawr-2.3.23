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

namespace Rawr.Elemental
{
    public partial class CalculationOptionsPanelElemental : UserControl, ICalculationOptionsPanel
    {
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

        #region ICalculationOptionsPanel Members
        public UserControl PanelControl { get { return this; } }

        CalculationOptionsElemental calcOpts = null;

        private Character character;
        public Character Character
        {
            get { return character; }
            set
            {
                // Kill any old event connections
                if (character != null && character.CalculationOptions != null
                    && character.CalculationOptions is CalculationOptionsElemental)
                    ((CalculationOptionsElemental)character.CalculationOptions).PropertyChanged
                        -= new PropertyChangedEventHandler(CalculationOptionsPanelElemental_PropertyChanged);
                // Apply the new character
                character = value;
                // Load the new CalcOpts
                LoadCalculationOptions();
                // Model Specific Code
                // Set the Data Context
                LayoutRoot.DataContext = calcOpts;
                // Add new event connections
                calcOpts.PropertyChanged += new PropertyChangedEventHandler(CalculationOptionsPanelElemental_PropertyChanged);
                // Run it once for any special UI config checks
                CalculationOptionsPanelElemental_PropertyChanged(null, new PropertyChangedEventArgs(""));
            }
        }

        private bool _loadingCalculationOptions;
        public void LoadCalculationOptions()
        {
            _loadingCalculationOptions = true;
            if (Character.CalculationOptions == null) Character.CalculationOptions = new CalculationOptionsElemental();
            calcOpts = Character.CalculationOptions as CalculationOptionsElemental;
            // Model Specific Code
            //
            _loadingCalculationOptions = false;
        }

        void CalculationOptionsPanelElemental_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
    }
}
