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

namespace Rawr.Warlock
{
    public partial class TextInputDialog : ChildWindow
    {
        public TextInputDialog(string title, string message, string start)
        {
            InitializeComponent();
        }

        public static string Text = "";
        public static TextInputDialog d = null;
        public static void Show(string title, string message, string start)
        {
            d = new TextInputDialog(title, message, start);
            d.Closed += new EventHandler(Dialog_Closed);
            d.Show();
        }

        public static void Dialog_Closed(object sender, EventArgs e) {
            if (d.DialogResult.GetValueOrDefault(false)) {
                Text = d.inputBox.Text;
            } else {
                Text = "";
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

