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

namespace Rawr.UI
{
    public partial class SaveTalentSpecDialog : ChildWindow
    {
        private TalentsBase Talents;
        private int Tree1, Tree2, Tree3;

        public SaveTalentSpecDialog(TalentsBase spec, int tree1, int tree2, int tree3)
        {
            InitializeComponent();
            Talents = spec;
            Tree1 = tree1;
            Tree2 = tree2;
            Tree3 = tree3;

            SavedTalentSpecList saved = SavedTalentSpec.SpecsFor(spec.GetClass());
            if (saved.Count > 0)
            {
                UpdateCombo.ItemsSource = saved;
                UpdateCombo.SelectedIndex = 0;
            }
            else
            {
                UpdateCombo.IsEnabled = false;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateCombo.SelectedIndex >= 0)
            {
                SavedTalentSpec spec = UpdateCombo.SelectedItem as SavedTalentSpec;
                spec.Spec = Talents.ToString();
                spec.Tree1 = Tree1;
                spec.Tree2 = Tree2;
                spec.Tree3 = Tree3;
            }
            else
            {
                SavedTalentSpec.AllSpecs.Add(new SavedTalentSpec(NewText.Text, Talents, Tree1, Tree2, Tree3));                
            }
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private bool responding;
        private void New_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!responding)
            {
                responding = true;
                UpdateCombo.SelectedIndex = -1;
                responding = false;
            }
        }

        private void Update_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!responding)
            {
                responding = true;
                NewText.Text = "";
                responding = false;
            }
        }
    }
}
