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
using Rawr;

namespace Rawr.UI
{
    public partial class SpecialEffectEditor : ChildWindow
    {
        public SpecialEffect SpecialEffect { get; set; }

        public SpecialEffectEditor() : this(null) { }
        public SpecialEffectEditor(SpecialEffect eff)
        {
            InitializeComponent();
            TriggerCombo.ItemsSource = EnumHelper.GetValues<Trigger>().Select(e => e.ToString());

            if (eff == null)
            {
                Title = "Add Special Effect...";
                SpecialEffect = new SpecialEffect();
                EffectStats.CurrentStats = new Stats();
            }
            else
            {
                Title = "Edit Special Effect...";
                SpecialEffect = eff;
                EffectStats.CurrentStats = eff.Stats.Clone();
            }

            TriggerCombo.SelectedIndex = (int)SpecialEffect.Trigger;
            DurationNum.Value = SpecialEffect.Duration;
            CooldownNum.Value = SpecialEffect.Cooldown;
            StacksNum.Value = SpecialEffect.MaxStack;
            if (SpecialEffect.Chance > 0)
            {
                ChanceCombo.SelectedIndex = 0;
                ChanceNum.Value = SpecialEffect.Chance * 100;
            }
            else
            {
                ChanceCombo.SelectedIndex = 1;
                ChanceNum.Value = -SpecialEffect.Chance;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            SpecialEffect.Trigger = (Trigger)TriggerCombo.SelectedIndex;
            SpecialEffect.Duration = (float)DurationNum.Value;
            SpecialEffect.Cooldown = (float)CooldownNum.Value;
            SpecialEffect.MaxStack = (int)StacksNum.Value;
            SpecialEffect.Stats = EffectStats.CurrentStats;
            if (ChanceCombo.SelectedIndex == 0) SpecialEffect.Chance = (float)(ChanceNum.Value / 100);
            else SpecialEffect.Chance = -(float)ChanceNum.Value;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

