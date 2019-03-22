using System;
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
	public partial class ComparisonGraphBar : UserControl
	{

        public Color Color
        {
            set { BaseRect.Fill = new SolidColorBrush(value); }
        }

        public string Title
        {
            get { return TextLabel.Text; }
            set { TextLabel.Text = value; }
        }

		public ComparisonGraphBar()
		{
			// Required to initialize variables
			InitializeComponent();
		}
	}
}