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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Rawr.Silverlight
{
    public partial class Page : UserControl
    {
        ServiceReference.RawrServiceClient proxy;
        private string character;

        public Page()
        {
            InitializeComponent();
            proxy = new ServiceReference.RawrServiceClient();

            proxy.GetSupportedModelsCompleted += new EventHandler<Rawr.Silverlight.ServiceReference.GetSupportedModelsCompletedEventArgs>(proxy_GetSupportedModelsCompleted);
            proxy.GetCharacterDisplayCalculationValuesCompleted += new EventHandler<Rawr.Silverlight.ServiceReference.GetCharacterDisplayCalculationValuesCompletedEventArgs>(proxy_GetCharacterDisplayCalculationValuesCompleted);

            proxy.GetSupportedModelsAsync();
        }

        void proxy_GetCharacterDisplayCalculationValuesCompleted(object sender, Rawr.Silverlight.ServiceReference.GetCharacterDisplayCalculationValuesCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ResultsTextBlock.Text = e.Error.ToString();
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, string> kvp in e.Result)
                {
                    sb.AppendFormat("{0}\t:{1}\n", kvp.Key, kvp.Value);
                }
                ResultsTextBlock.Text = sb.ToString();
            }
        }

        void proxy_GetSupportedModelsCompleted(object sender, Rawr.Silverlight.ServiceReference.GetSupportedModelsCompletedEventArgs e)
        {
            ModelComboBox.Items.Clear();
            ModelComboBox.ItemsSource = e.Result;
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() ?? false)
            {
                UploadTextBox.Text = dialog.SelectedFile.Name;
                character = dialog.SelectedFile.OpenText().ReadToEnd();
            }
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (character != null && ModelComboBox.SelectedItem != null)
            {
                proxy.GetCharacterDisplayCalculationValuesAsync(character, (string)ModelComboBox.SelectedItem);
            }
        }
    }
}
