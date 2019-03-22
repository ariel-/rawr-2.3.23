using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.DataVisualization;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace Rawr.Mage.Graphs
{
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(TimeIntervalDataPoint))]
    [StyleTypedProperty(Property = "LegendItemStyle", StyleTargetType = typeof(LegendItem))]
    [TemplatePart(Name = DataPointSeries.PlotAreaName, Type = typeof(Canvas))]
    public class TimeIntervalSeries : ColumnBarBaseSeries<TimeIntervalDataPoint>
    {
        private Binding _startTimeBinding;

        public Binding StartTimeBinding
        {
            get
            {
                return _startTimeBinding;
            }
            set
            {
                if (value != _startTimeBinding)
                {
                    _startTimeBinding = value;
                    Refresh();
                }
            }
        }

        public string StartTimePath
        {
            get
            {
                return (null != StartTimeBinding) ? StartTimeBinding.Path.Path : null;
            }
            set
            {
                if (null == value)
                {
                    StartTimeBinding = null;
                }
                else
                {
                    StartTimeBinding = new Binding(value);
                }
            }
        }

        private Binding _endTimeBinding;

        public Binding EndTimeBinding
        {
            get
            {
                return _endTimeBinding;
            }
            set
            {
                if (value != _endTimeBinding)
                {
                    _endTimeBinding = value;
                    Refresh();
                }
            }
        }

        public string EndTimePath
        {
            get
            {
                return (null != EndTimeBinding) ? EndTimeBinding.Path.Path : null;
            }
            set
            {
                if (null == value)
                {
                    EndTimeBinding = null;
                }
                else
                {
                    EndTimeBinding = new Binding(value);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the TimeIntervalSeries class.
        /// </summary>
        public TimeIntervalSeries()
        {
        }

        protected override void PrepareDataPoint(DataPoint dataPoint, object dataContext)
        {
            base.PrepareDataPoint(dataPoint, dataContext);
            TimeIntervalDataPoint timeIntervalDataPoint = (TimeIntervalDataPoint)dataPoint;
            timeIntervalDataPoint.SetBinding(TimeIntervalDataPoint.StartTimeProperty, StartTimeBinding);
            timeIntervalDataPoint.SetBinding(TimeIntervalDataPoint.EndTimeProperty, EndTimeBinding);
        }

        /// <summary>
        /// Acquire a horizontal category axis and a vertical linear axis.
        /// </summary>
        /// <param name="firstDataPoint">The first data point.</param>
        protected override void GetAxes(DataPoint firstDataPoint)
        {
            GetAxes(
                firstDataPoint,
                (axis) => axis.Orientation == AxisOrientation.Y,
                () => new CategoryAxis { Orientation = AxisOrientation.Y },
                (axis) =>
                {
                    IRangeAxis rangeAxis = axis as IRangeAxis;
                    return rangeAxis != null && axis.Orientation == AxisOrientation.X;
                },
                () =>
                {
                    IRangeAxis rangeAxis = CreateRangeAxisFromData(firstDataPoint.DependentValue);
                    rangeAxis.Orientation = AxisOrientation.X;
                    if (rangeAxis == null)
                    {
                        throw new InvalidOperationException();
                    }
                    DisplayAxis axis = rangeAxis as DisplayAxis;
                    if (axis != null)
                    {
                        axis.ShowGridLines = true;
                    }
                    return rangeAxis;
                });
        }

        /// <summary>
        /// Updates each point.
        /// </summary>
        /// <param name="dataPoint">The data point to update.</param>
        protected override void UpdateDataPoint(DataPoint dataPoint)
        {
            TimeIntervalDataPoint intervalDataPoint = (TimeIntervalDataPoint)dataPoint;
            if (SeriesHost == null)
            {
                return;
            }

            //object category = dataPoint.ActualIndependentValue ?? (this.ActiveDataPoints.IndexOf(dataPoint) + 1);
            //Range<UnitValue> coordinateRange = GetCategoryRange(category);

            /*if (!coordinateRange.HasData)
            {
                return;
            }
            else if (coordinateRange.Maximum.Unit != Unit.Pixels || coordinateRange.Minimum.Unit != Unit.Pixels)
            {
                throw new InvalidOperationException();
            }

            double minimum = (double)coordinateRange.Minimum.Value;
            double maximum = (double)coordinateRange.Maximum.Value;*/

            // we'll just ignore the category axis and position the intervals where we want

            IEnumerable<TimeIntervalSeries> barSeries = SeriesHost.Series.OfType<TimeIntervalSeries>().Where(series => series.ActualIndependentAxis == ActualIndependentAxis);
            int numberOfSeries = barSeries.Count();
            double coordinateRangeHeight = 10.0;
            double segmentHeight = coordinateRangeHeight * 0.8;
            //double barHeight = segmentHeight / numberOfSeries;
            int seriesIndex = barSeries.IndexOf(this);

            double startTimePointX = ActualDependentRangeAxis.GetPlotAreaCoordinate(intervalDataPoint.StartTime).Value;
            double endTimePointX = ActualDependentRangeAxis.GetPlotAreaCoordinate(intervalDataPoint.EndTime).Value;

            double offset = /*seriesIndex * Math.Round(barHeight) +*/ coordinateRangeHeight * 0.1;
            CategoryAxis categoryAxis = ActualIndependentAxis as CategoryAxis;            
            double dataPointY = categoryAxis.ActualHeight - 10.0 * numberOfSeries + seriesIndex * 10 + offset;

            /*if (GetIsDataPointGrouped(category))
            {
                // Multiple DataPoints share this category; offset and overlap them appropriately
                IGrouping<object, DataPoint> categoryGrouping = GetDataPointGroup(category);
                int index = categoryGrouping.IndexOf(dataPoint);
                dataPointY += (index * (barHeight * 0.2)) / (categoryGrouping.Count() - 1);
                barHeight *= 0.8;
                Canvas.SetZIndex(dataPoint, -index);
            }*/

            if (ValueHelper.CanGraph(startTimePointX) && ValueHelper.CanGraph(endTimePointX) && ValueHelper.CanGraph(dataPointY))
            {
                dataPoint.Visibility = Visibility.Visible;

                double top = Math.Round(dataPointY);
                double height = Math.Round(segmentHeight);

                double left = Math.Round(Math.Min(startTimePointX, endTimePointX) - 0.5);
                double right = Math.Round(Math.Max(startTimePointX, endTimePointX) - 0.5);
                double width = right - left + 1;

                Canvas.SetLeft(dataPoint, left);
                Canvas.SetTop(dataPoint, top);
                dataPoint.Width = width;
                dataPoint.Height = height;
            }
            else
            {
                dataPoint.Visibility = Visibility.Collapsed;
            }
        }
    }
}
