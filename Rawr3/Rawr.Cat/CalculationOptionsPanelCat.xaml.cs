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
using System.ComponentModel;

namespace Rawr.Cat
{
    public partial class CalculationOptionsPanelCat : UserControl, ICalculationOptionsPanel
    {
        public CalculationOptionsPanelCat()
        {
            InitializeComponent();
            this.LayoutRoot.SelectAll();
            foreach (AccordionItem item in LayoutRoot.Items)
            {
                item.IsSelected = true;
            }
        }

        #region ICalculationOptionsPanel Members
        public UserControl PanelControl { get { return this; } }

        private Character character;
        public Character Character
        {
            get { return character; }
            set {
                character = value;
                LoadCalculationOptions();
            }
        }

        private bool _loadingCalculationOptions;
        public void LoadCalculationOptions()
        {
            _loadingCalculationOptions = true;
            if (Character.CalculationOptions == null) Character.CalculationOptions = new CalculationOptionsCat();
            this.DataContext = Character.CalculationOptions as CalculationOptionsCat;
            this.LayoutRoot.SelectAll();
            _loadingCalculationOptions = false;
        }

        public void CalculationOptionsPanelCat_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_loadingCalculationOptions) { return; }
            // This would handle any special changes, especially combobox assignments, but not when the pane is trying to load
            if (e.PropertyName == "SomeProperty")
            {
                // Model Specific Code
            }
            //
            if (Character != null) { Character.OnCalculationsInvalidated(); }
        }
        #endregion
    }
}
