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
    public partial class IncreaseQuota : ChildWindow
    {

        private int size;

        public IncreaseQuota(int kilobytes)
        {
            size = kilobytes;
            InitializeComponent();

            QuotaLabel.Text = string.Format("Rawr requires at least {0} mb to run. You need to increase the storage quota for this application to run.", Math.Round(size / 1024d, 2));
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = FileUtils.EnsureQuota(size);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

