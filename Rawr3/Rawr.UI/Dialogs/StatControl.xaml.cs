using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rawr.UI
{
	public partial class StatControl : UserControl
	{

        private Stats currentStats;
        public Stats CurrentStats
        {
            get { return currentStats; }
            set
            {
                currentStats = value;

                StatsStack.Children.Clear();
                if (currentStats != null)
                {
                    var nonZeroStats = currentStats.Values(x => x != 0);
                    foreach (PropertyInfo info in nonZeroStats.Keys)
                    {
                        AddNewRow(info, nonZeroStats[info]);
                    }
                }
            }
        }

        public void AddNewRow(PropertyInfo stat, float value)
        {
            Grid statsGrid = new Grid();
            ColumnDefinition cd = new ColumnDefinition();
            cd.Width = new GridLength(1, GridUnitType.Star);
            statsGrid.ColumnDefinitions.Add(cd);
            cd = new ColumnDefinition();
            cd.Width = new GridLength(1, GridUnitType.Star);
            statsGrid.ColumnDefinitions.Add(cd);
            cd = new ColumnDefinition();
            cd.Width = new GridLength(30);
            statsGrid.ColumnDefinitions.Add(cd);

            TextBox tb = new TextBox();
            tb.Text = Extensions.DisplayName(stat).Substring(1);
            tb.IsEnabled = false;
            tb.Margin = new Thickness(2);
            statsGrid.Children.Add(tb);
            Grid.SetColumn(tb, 0);

            TextBox statBox = new TextBox();
            statBox.Text = value.ToString();
            statBox.Tag = stat;
            statBox.Margin = new Thickness(2);
            statBox.LostFocus += new RoutedEventHandler(statBox_LostFocus);
            statsGrid.Children.Add(statBox);
            Grid.SetColumn(statBox, 1);

            Button deleteButton = new Button();
            deleteButton.Style = Resources["StatsControlCloseStyle"] as Style;
            deleteButton.Tag = stat;
            deleteButton.Click += new RoutedEventHandler(deleteButton_Click);
            statsGrid.Children.Add(deleteButton);
            Grid.SetColumn(deleteButton, 2);

            StatsStack.Children.Add(statsGrid);
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            PropertyInfo info = ((Button)sender).Tag as PropertyInfo;
            MethodInfo set = info.GetSetMethod();
            object[] param = new object[1] { 0 };
            set.Invoke(currentStats, param);
            StatsStack.Children.Remove((UIElement)((Button)sender).Parent);
        }

        private void statBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentStats != null)
            {
                TextBox text = sender as TextBox;
                PropertyInfo info = text.Tag as PropertyInfo;
                try
                {
                    object[] param = new object[1] { float.Parse(text.Text) };
                    info.GetSetMethod().Invoke(currentStats, param);
                }
                catch { }
                text.Text = ((float)info.GetGetMethod().Invoke(CurrentStats, null)).ToString();
            }
        }

		public StatControl()
		{
			// Required to initialize variables
			InitializeComponent();

            if (Stats.PropertyInfoCache != null)
            {
                List<string> statNames = new List<string>();
                foreach (PropertyInfo info in Stats.PropertyInfoCache)
                {
                    if (Stats.IsPercentage(info)) statNames.Add(Extensions.DisplayName(info));
                    else statNames.Add(Extensions.DisplayName(info).Substring(1));
                }
                AddStatBox.ItemsSource = statNames;
            }
		}

		private void AddStatButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            foreach (PropertyInfo info in Stats.PropertyInfoCache)
            {
                string statName;
                if (Stats.IsPercentage(info)) statName = Extensions.DisplayName(info);
                else statName = Extensions.DisplayName(info).Substring(1);
                if (statName.Equals(AddStatBox.Text))
                {
                    AddNewRow(info, 0);
                    AddStatBox.Text = "";
                    break;
                }
            }
		}
	}
}