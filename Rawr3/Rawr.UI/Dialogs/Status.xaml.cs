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
    public partial class Status : ChildWindow
    {
        private const string OVERALL_PROGRESS = "{0} of {1} Tasks have completed successfully";

        private List<StatusEventArgs> statusUpdates;
        private List<StatusErrorEventArgs> statusErrors;

        public Status()
        {
            InitializeComponent();
            Expanded = true;
            statusUpdates = new List<StatusEventArgs>();
            statusErrors = new List<StatusErrorEventArgs>();
            StatusMessaging.StatusUpdate += new StatusMessaging.StatusUpdateDelegate(StatusMessaging_StatusUpdate);
            StatusMessaging.StatusError += new StatusMessaging.StatusErrorDelegate(StatusMessaging_StatusError);

#if SILVERLIGHT
            Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(Status_Closing);
#else
            Closing += new System.ComponentModel.CancelEventHandler(Status_Closing);
#endif
            Closed += new EventHandler(Status_Closed);

            TasksData.ItemsSource = statusUpdates;
            ErrorData.ItemsSource = statusErrors;
        }

        private void Status_Closed(object sender, EventArgs e)
        {
            statusUpdates.Clear();
            statusErrors.Clear();
        }

        private void Status_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!AllowedToClose) e.Cancel = true;
        }

        public bool AllowedToClose { get; set; }
        private bool Expanded { get; set; }

        public bool HasErrors
        {
            get { return statusErrors.Count > 0; }
        }

        public void SwitchToErrorTab()
        {
            DetailsTab.SelectedIndex = 1;
        }

        private void StatusMessaging_StatusUpdate(StatusEventArgs args)
        {
            bool found = false;
            if (args != null && !String.IsNullOrEmpty(args.Key))
            {
                for (int i = 0; i < statusUpdates.Count; i++)
                {
                    if (statusUpdates[i].Key == args.Key)
                    {
                        statusUpdates[i] = args;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    statusUpdates.Add(args);
                }
            }

            AllowedToClose = true;
            int doneCount = 0;
            foreach (StatusEventArgs e in statusUpdates)
            {
                AllowedToClose = AllowedToClose && e.Done;
                if (e.Done) doneCount++;
            }

            ProgressText.Text = string.Format(OVERALL_PROGRESS, doneCount, statusUpdates.Count);
            ProgressBar.Value = statusUpdates.Count == 0 ? 0 : ((double)doneCount / (double)statusUpdates.Count * 100d);

            TasksData.ItemsSource = null;
            TasksData.ItemsSource = statusUpdates;

            if (AllowedToClose && statusErrors.Count == 0) this.DialogResult = true;
        }

        private void StatusMessaging_StatusError(StatusErrorEventArgs args)
        {
            statusErrors.Add(args);
            ErrorData.ItemsSource = statusErrors;
            SwitchToErrorTab();
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (Expanded)
            {
                DetailsTab.Visibility = Visibility.Collapsed;
                DetailsButton.Content = "Details >>";
                Expanded = false;
            }
            else
            {
                DetailsTab.Visibility = Visibility.Visible;
                DetailsButton.Content = "Details <<";
                Expanded = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

