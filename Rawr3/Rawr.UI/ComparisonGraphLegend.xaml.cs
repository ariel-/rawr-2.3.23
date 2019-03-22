using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Rawr.Optimizer;

namespace Rawr.UI
{
	public partial class ComparisonGraphLegend : UserControl
	{
        private DisplayMode mode;
        public DisplayMode Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                if (mode == DisplayMode.Overall)
                {
                    Dictionary<string, Color> overallLegend = new Dictionary<string, Color>();
                    overallLegend["Overall"] = Colors.Purple;
                    LegendItems = overallLegend;
                }
            }
        }

        private Dictionary<string, Color> legendItems;
        public Dictionary<string, Color> LegendItems
        {
            get { return legendItems; }
            set
            {
				if (legendItems != value)
				{
					legendItems = value;
					if (legendItems != null)
					{
						LegendStack.Children.Clear();
						foreach (KeyValuePair<string, Color> kvp in legendItems)
						{
							LegendStack.Children.Add(new ComparisonGraphBar() { Color = kvp.Value, Title = kvp.Key });
						}
					}
				}
            }
        }

        public ComparisonGraphLegend()
		{
			// Required to initialize variables
			InitializeComponent();

            mode = DisplayMode.Subpoints;
		}

        public enum DisplayMode
        {
            Subpoints,
            Overall
        }
    }
}