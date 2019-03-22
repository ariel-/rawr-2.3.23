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
using System.Windows.Controls.DataVisualization.Charting;

namespace Rawr.Mage.Graphs
{
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Unselected", GroupName = "SelectionStates")]
    [TemplateVisualState(Name = "Selected", GroupName = "SelectionStates")]
    [TemplateVisualState(Name = "Shown", GroupName = "RevealStates")]
    [TemplateVisualState(Name = "Hidden", GroupName = "RevealStates")]
    public class TimeIntervalDataPoint : DataPoint
    {
#if !SILVERLIGHT
        /// <summary>
        /// Initializes the static members of the TimeIntervalDataControl class.
        /// </summary>
        static TimeIntervalDataPoint()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeIntervalDataPoint), new FrameworkPropertyMetadata(typeof(TimeIntervalDataPoint)));
        }

#endif
        /// <summary>
        /// Initializes a new instance of the TimeIntervalDataControl class.
        /// </summary>
        public TimeIntervalDataPoint()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(TimeIntervalDataPoint);
#endif
        }

        public DateTime StartTime
        {
            get { return (DateTime)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register(
                "StartTime",
                typeof(DateTime),
                typeof(TimeIntervalDataPoint),
                new PropertyMetadata(default(DateTime)));

        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register(
                "EndTime",
                typeof(DateTime),
                typeof(TimeIntervalDataPoint),
                new PropertyMetadata(default(DateTime)));
    }
}
