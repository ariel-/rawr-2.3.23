using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls.DataVisualization.Charting.Primitives;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls;

namespace Rawr.Mage.Graphs
{
    [TemplatePart(Name = "ChartArea", Type = typeof(EdgePanel))]
    [TemplatePart(Name = "Legend", Type = typeof(Legend))]
    [StyleTypedProperty(Property = "TitleStyle", StyleTargetType = typeof(Title))]
    [StyleTypedProperty(Property = "LegendStyle", StyleTargetType = typeof(Legend))]
    [StyleTypedProperty(Property = "ChartAreaStyle", StyleTargetType = typeof(EdgePanel))]
    [StyleTypedProperty(Property = "PlotAreaStyle", StyleTargetType = typeof(Grid))]
    [ContentProperty("Series")]
    public class ChartWithText : Chart
    {
        #if !SILVERLIGHT
        /// <summary>
        /// Initializes the static members of the TimeIntervalDataControl class.
        /// </summary>
        static ChartWithText()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartWithText), new FrameworkPropertyMetadata(typeof(ChartWithText)));
        }

#endif
        /// <summary>
        /// Initializes a new instance of the TimeIntervalDataControl class.
        /// </summary>
        public ChartWithText()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(ChartWithText);
#endif
        }

        public object Text
        {
            get { return GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Identifies the Title dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(object),
                typeof(ChartWithText),
                null);
    }
}
