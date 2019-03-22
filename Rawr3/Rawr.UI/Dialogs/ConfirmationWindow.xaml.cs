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
    public partial class ConfirmationWindow : ChildWindow
    {
		public static void ShowDialog(string message, EventHandler callback)
		{
			ConfirmationWindow window = new ConfirmationWindow() { Message = message };
			if (callback != null) window.Closed += callback;
			window.Show();
		}

		public string Message
        {
            get { return MessageText.Text; }
            set { MessageText.Text = value; }
        }

		public ConfirmationWindow()
        {
            InitializeComponent();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

