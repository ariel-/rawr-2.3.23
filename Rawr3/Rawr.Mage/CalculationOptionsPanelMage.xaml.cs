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
using System.Text;
using System.Windows.Data;

namespace Rawr.Mage
{
    public partial class CalculationOptionsPanelMage : UserControl, ICalculationOptionsPanel
    {
        private int advancedSolverIndex;

        public CalculationOptionsPanelMage()
        {
            InitializeComponent();

            CalculationsMage.AdvancedSolverChanged += new EventHandler(CalculationsMage_AdvancedSolverChanged);
            CalculationsMage.AdvancedSolverLogUpdated += new EventHandler(CalculationsMage_AdvancedSolverLogUpdated);           
            advancedSolverIndex = OptionsTab.Items.CastList<TabItem>().FindIndex(tab => (string)tab.Header == "Solver Log");
        }

        private void OptionsTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OptionsTab != null && OptionsTab.SelectedIndex == advancedSolverIndex)
            {
                AdvancedSolverLog.Text = CalculationsMage.AdvancedSolverLog;
            }
        }

        void CalculationsMage_AdvancedSolverLogUpdated(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke((Action<object, EventArgs>)CalculationsMage_AdvancedSolverLogUpdated, sender, e);
            }
            else
            {
                if (OptionsTab.SelectedIndex == advancedSolverIndex)
                {
                    AdvancedSolverLog.Text = CalculationsMage.AdvancedSolverLog;
#if !SILVERLIGHT
                    AdvancedSolverLog.ScrollToEnd();
#endif
                }
            }
        }

        void CalculationsMage_AdvancedSolverChanged(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke((Action<object, EventArgs>)CalculationsMage_AdvancedSolverChanged, sender, e);
            }
            else
            {
                AdvancedSolverCancel.IsEnabled = !CalculationsMage.IsSolverEnabled(null);
            }
        }

        public UserControl PanelControl { get { return this; } }

        private Character character;
        public Character Character
        {
            get
            {
                return character;
            }
            set
            {
                if (character != null && character.CalculationOptions != null && character.CalculationOptions is CalculationOptionsMage)
                {
                    ((CalculationOptionsMage)character.CalculationOptions).PropertyChanged -= new PropertyChangedEventHandler(CalculationOptionsPanelMage_PropertyChanged);
                }

                character = value;
                if (character.CalculationOptions == null) character.CalculationOptions = new CalculationOptionsMage(character);
                LayoutRoot.DataContext = Character.CalculationOptions;

                ((CalculationOptionsMage)character.CalculationOptions).PropertyChanged += new PropertyChangedEventHandler(CalculationOptionsPanelMage_PropertyChanged);

            }
        }

        void CalculationOptionsPanelMage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Character.OnCalculationsInvalidated();
        }

        private void AdvancedSolverCancel_Click(object sender, RoutedEventArgs e)
        {
            CalculationsMage.CancelAsync();
        }

        private void CooldownEditor_Click(object sender, RoutedEventArgs e)
        {
            //if (cooldownRestrictions == null || cooldownRestrictions.IsDisposed)
            //{
            //    cooldownRestrictions = new CooldownRestrictionsForm();
            //    cooldownRestrictions.Character = Character;
            //    //cooldownRestrictions.bindingSourceCalculationOptions.DataSource = calculationOptions;
            //}
            //cooldownRestrictions.Show();
            //cooldownRestrictions.BringToFront();
        }

        private void HotStreakUtilization_Click(object sender, RoutedEventArgs e)
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

            Dictionary<string, SpellContribution> dict1 = new Dictionary<string, SpellContribution>();
            Dictionary<string, SpellContribution> dict2 = new Dictionary<string, SpellContribution>();
            c1.AddDamageContribution(dict1, 1.0f);
            c2.AddDamageContribution(dict2, 1.0f);

            float predicted = dict2["Pyroblast"].Hits / dict2["Fireball"].Hits;
            float actual = dict1["Pyroblast"].Hits / dict1["Fireball"].Hits;

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

        private void CalculationTiming_Click(object sender, RoutedEventArgs e)
        {
            CalculationsMage calculations = (CalculationsMage)Calculations.Instance;
            Character character = Character;
#if SILVERLIGHT
            DateTime start = DateTime.Now;
#else
            System.Diagnostics.Stopwatch clock = new System.Diagnostics.Stopwatch();
            clock.Start();
#endif
            for (int i = 0; i < 10000; i++)
            {
                calculations.GetCharacterCalculations(character);
            }
#if SILVERLIGHT
            MessageBox.Show("Calculating 10000 characters takes " + DateTime.Now.Subtract(start).TotalSeconds + " seconds.");
#else
            clock.Stop();
            MessageBox.Show("Calculating 10000 characters takes " + clock.Elapsed.TotalSeconds + " seconds.");
#endif
        }

        private void CycleAnalyzer_Click(object sender, RoutedEventArgs e)
        {
            CycleAnalyzerDialog analyzer = new CycleAnalyzerDialog(Character);
#if SILVERLIGHT
            analyzer.Show();
#else
            ((Window)analyzer).Show();
#endif
        }

        private void CustomSpellMix_Click(object sender, RoutedEventArgs e)
        {
            CalculationOptionsMage calculationOptions = Character.CalculationOptions as CalculationOptionsMage;
            if (calculationOptions.CustomSpellMix == null) calculationOptions.CustomSpellMix = new List<SpellWeight>();
            CustomSpellMixDialog dialog = new CustomSpellMixDialog(calculationOptions.CustomSpellMix);
            dialog.Show();
        }

        #region Silverlight workaround
        public class ActualSizePropertyProxy : FrameworkElement, INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public FrameworkElement Element
            {
                get { return (FrameworkElement)GetValue(ElementProperty); }
                set { SetValue(ElementProperty, value); }
            }

            public double ActualHeightValue
            {
                get { return Element == null ? 0 : Element.ActualHeight; }
            }

            public double ActualWidthValue
            {
                get { return Element == null ? 0 : Element.ActualWidth; }
            }

            public static readonly DependencyProperty ElementProperty =
                DependencyProperty.Register("Element", typeof(FrameworkElement), typeof(ActualSizePropertyProxy),
                                            new PropertyMetadata(null, OnElementPropertyChanged));

            private static void OnElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                ((ActualSizePropertyProxy)d).OnElementChanged(e);
            }

            private void OnElementChanged(DependencyPropertyChangedEventArgs e)
            {
                FrameworkElement oldElement = (FrameworkElement)e.OldValue;
                FrameworkElement newElement = (FrameworkElement)e.NewValue;

                newElement.SizeChanged += new SizeChangedEventHandler(Element_SizeChanged);
                if (oldElement != null)
                {
                    oldElement.SizeChanged -= new SizeChangedEventHandler(Element_SizeChanged);
                }
                NotifyPropChange();
            }

            private void Element_SizeChanged(object sender, SizeChangedEventArgs e)
            {
                NotifyPropChange();
            }

            private void NotifyPropChange()
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ActualWidthValue"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ActualHeightValue"));
                }
            }
        } 

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Silverlight workaround
            // LayoutRoot
            // MaxHeight="{Binding Path=ActualHeight, RelativeSource={RelativeSource Mode=FindAncestor, AncestorType={x:Type ScrollViewer}}}"
            DependencyObject obj = this;
            while (obj != null)
            {
                if (obj is ScrollViewer)
                {
                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
            if (obj != null)
            {
                ActualSizePropertyProxy proxy = new ActualSizePropertyProxy();
                proxy.Element = (FrameworkElement)obj;
                LayoutRoot.SetBinding(Grid.MaxHeightProperty, new Binding()
                {
                    Mode = BindingMode.OneWay,
                    Path = new PropertyPath("ActualHeightValue"),
                    Source = proxy,
                });
            }
        }
        #endregion
    }
}
