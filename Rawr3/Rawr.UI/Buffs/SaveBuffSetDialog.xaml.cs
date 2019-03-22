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
    public partial class SaveBuffSetDialog : ChildWindow
    {
        private List<Buff> BuffSet;

        public SaveBuffSetDialog(List<Buff> newBuffSet)
        {
            InitializeComponent();
            BuffSet = newBuffSet;

            SavedBuffSetList saved = SavedBuffSet.AllSets;
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
                SavedBuffSet set = UpdateCombo.SelectedItem as SavedBuffSet;
                set.SetAsString = BuffSet.ToString();
            }
            else
            {
                SavedBuffSet.AllSets.Add(new SavedBuffSet(NewText.Text, BuffSet));
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

