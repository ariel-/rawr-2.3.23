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
using System.Text.RegularExpressions;

namespace Rawr.UI
{
    public partial class EnterId : ChildWindow
    {
        public int Value
        {
            get
            {
                String input = textItemId.Text;

                Regex wowhead = new Regex(@"http://www.wowhead.com/\?item=([-+]?\d+)");
                Match m = wowhead.Match(input);

                if (m.Success)
                {
                    return int.Parse(m.Groups[1].Value);
                }

                Regex wowheadptr = new Regex(@"http://ptr.wowhead.com/\?item=([-+]?\d+)");
                m = wowheadptr.Match(input);

                if (m.Success)
                {
                    return int.Parse(m.Groups[1].Value);
                }

                Regex thottbot = new Regex(@"http://thottbot.com/i([-+]?\d+)");
                m = thottbot.Match(input);

                if (m.Success)
                {
                    return int.Parse(m.Groups[1].Value);
                }

                Regex numeric = new Regex(@"([-+]?\d+)");
                m = numeric.Match(input);

                if (m.Success)
                {
                    return int.Parse(m.Groups[1].Value);
                }

                return 0;
            }
        }

        public string ItemName
        {
            get { return textItemId.Text; }
        }

        public EnterId()
        {
            InitializeComponent();
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

