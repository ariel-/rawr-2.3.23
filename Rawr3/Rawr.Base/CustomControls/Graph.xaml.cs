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
using System.Reflection;
using System.Windows.Controls.DataVisualization;

namespace Rawr
{
    public partial class Graph : UserControl
    {
        public Graph()
        {
            InitializeComponent();
        }

        private static Graph instance = new Graph();

        public static Graph Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetupStatsGraph(Character character, Stats[] statsList, int scale, string explanatoryText, string calculation)
        {
            Chart.Title = "Graph of " + calculation;
            Color[] colors = new Color[] {
                Color.FromArgb(255,202,180,96), 
                Color.FromArgb(255,101,225,240),
                Color.FromArgb(255,0,4,3), 
                Color.FromArgb(255,238,238,30),
                Color.FromArgb(255,45,112,63), 
                Color.FromArgb(255,121,72,210), 
                Color.FromArgb(255,217,100,54), 
                Color.FromArgb(255,210,72,195), 
                Color.FromArgb(255,206,189,191), 
                Color.FromArgb(255,255,0,0), 
                Color.FromArgb(255,0,255,0), 
                Color.FromArgb(255,0,0,255), 
            };
            UpdateStatsGraph(character, statsList, colors, scale, explanatoryText, calculation);            
        }

        public void UpdateStatsGraph(Character character, Stats[] statsList, Color[] colors, int scale, string explanatoryText, string calculation)
        {
            CharacterCalculationsBase baseCalc = Calculations.GetCharacterCalculations(character);
            float baseFigure = GetCalculationValue(baseCalc, calculation);
            if (statsList.Length == 0 || statsList.Length > colors.Length) return; // more than 12 elements for the array would run out of colours
            Point[][] points = new Point[statsList.Length][];
            Chart.Series.Clear();
            for (int index = 0; index < statsList.Length; index++)
            {
                Stats newStats = new Stats();
                points[index] = new Point[2 * scale + 1];
                newStats.Accumulate(statsList[index], -scale - 1);

                for (int count = -scale; count <= scale; count++)
                {
                    newStats.Accumulate(statsList[index]);

                    CharacterCalculationsBase currentCalc = Calculations.GetCharacterCalculations(character, new Item() { Stats = newStats }, false, false, false);
                    float currentFigure = GetCalculationValue(currentCalc, calculation);
                    float dpsChange = currentFigure - baseFigure;
                    points[index][count + scale] = new Point(count, dpsChange);
                }
                Style dataPointStyle = new Style(typeof(LineDataPoint));
                dataPointStyle.Setters.Add(new Setter(DataPoint.TemplateProperty, Resources["InvisibleDataPointTemplate"]));
                dataPointStyle.Setters.Add(new Setter(DataPoint.BackgroundProperty, new SolidColorBrush(colors[index])));
                Chart.Series.Add(new LineSeries()
                {
                    Title = statsList[index].ToString(),
                    ItemsSource = points[index],
                    IndependentValuePath = "X",
                    DependentValuePath = "Y",
                    DataPointStyle = dataPointStyle,
                });
            }
            Chart.Axes.Clear();
            Chart.Axes.Add(new LinearAxis()
            {
                Orientation = AxisOrientation.X,
                Title = "Stat Change",
                ShowGridLines = true,
            });
            Chart.Axes.Add(new LinearAxis()
            {
                Orientation = AxisOrientation.Y,
                Title = calculation,
                ShowGridLines = true,
            });
            orgDataDirty = true;
        }

        public void UpdateScalingGraph(Character character, Stats[] statsList, Stats baseStat, bool requiresReferenceCalculations, Color[] colors, int scale, string explanatoryText, string calculation)
        {
            CharacterCalculationsBase baseCalc = Calculations.GetCharacterCalculations(character);
            if (statsList.Length == 0 || statsList.Length > colors.Length) return; // more than 12 elements for the array would run out of colours
            Point[][] points = new Point[statsList.Length][];
            // extract property data for relative stats calculations
            KeyValuePair<PropertyInfo, float>[] properties = new KeyValuePair<PropertyInfo, float>[statsList.Length];
            for (int index = 0; index < statsList.Length; index++)
            {
                var p = statsList[index].Values(x => x > 0);
                foreach (var kvp in p)
                {
                    properties[index] = kvp;
                }
                points[index] = new Point[2 * scale + 1];
            }
            float unit = 1f;
            var bp = baseStat.Values(x => x > 0);
            foreach (var kvp in bp)
            {
                unit = kvp.Value;
            }
            Chart.Series.Clear();
            for (int count = -scale; count <= scale; count++)
            {
                Stats newStats = new Stats();
                newStats.Accumulate(baseStat, count);
                Item item = new Item() { Stats = newStats };
                if (requiresReferenceCalculations)
                {
                    Calculations.GetCharacterCalculations(character, item, true, false, false);
                }
                for (int index = 0; index < statsList.Length; index++)
                {
                    ComparisonCalculationBase currentCalc = CalculationsBase.GetRelativeStatValue(character, properties[index].Key, item, properties[index].Value);
                    float dpsChange = GetCalculationValue(currentCalc, calculation);
                    points[index][count + scale] = new Point(count * unit, dpsChange);
                }
            }
            for (int index = 0; index < statsList.Length; index++)
            {
                Style dataPointStyle = new Style(typeof(LineDataPoint));
                dataPointStyle.Setters.Add(new Setter(DataPoint.TemplateProperty, Resources["InvisibleDataPointTemplate"]));
                dataPointStyle.Setters.Add(new Setter(DataPoint.BackgroundProperty, new SolidColorBrush(colors[index])));
                Chart.Series.Add(new LineSeries()
                {
                    Title = statsList[index].ToString(),
                    ItemsSource = points[index],
                    IndependentValuePath = "X",
                    DependentValuePath = "Y",
                    DataPointStyle = dataPointStyle,
                });
            }
            Chart.Axes.Clear();
            Chart.Axes.Add(new LinearAxis()
            {
                Orientation = AxisOrientation.X,
                Title = "Stat Change",
                ShowGridLines = true,
            });
            Chart.Axes.Add(new LinearAxis()
            {
                Orientation = AxisOrientation.Y,
                Title = calculation,
                ShowGridLines = true,
            });
            // restore reference calculation
            if (requiresReferenceCalculations)
            {
                Stats newStats = new Stats();
                Item item = new Item() { Stats = newStats };
                Calculations.GetCharacterCalculations(character, item, true, false, false);
            }
            orgDataDirty = true;
        }

        private static float GetCalculationValue(CharacterCalculationsBase calcs, string calculation)
        {
            if (calculation == null || calculation == "Overall Rating")
                return calcs.OverallPoints;
            else
            {
                int index = 0;
                foreach (string subPoint in Calculations.SubPointNameColors.Keys)
                {
                    if (calculation.StartsWith(subPoint, StringComparison.Ordinal))
                        return calcs.SubPoints[index];
                    index++;
                }
                return 0f;
            }
        }

        private static float GetCalculationValue(ComparisonCalculationBase calcs, string calculation)
        {
            if (calculation == null || calculation == "Overall Rating")
                return calcs.OverallPoints;
            else
            {
                int index = 0;
                foreach (string subPoint in Calculations.SubPointNameColors.Keys)
                {
                    if (calculation.StartsWith(subPoint, StringComparison.Ordinal))
                        return calcs.SubPoints[index];
                    index++;
                }
                return 0f;
            }
        }

        public static string[] GetCalculationNames()
        {
            List<string> names = new List<string>();
            names.Add("Overall Rating");
            foreach (string subPoint in Calculations.SubPointNameColors.Keys)
                names.Add(subPoint + " Rating");
            return names.ToArray();
        }

        // original axis boundaries
        private double orgMinX;
        private double orgMaxX;
        private double orgMinY;
        private double orgMaxY;
        private bool orgDataDirty;

        private void Chart_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (orgDataDirty)
            {
                orgMinX = (double)((LinearAxis)Chart.Axes[0]).ActualMinimum;
                orgMaxX = (double)((LinearAxis)Chart.Axes[0]).ActualMaximum;
                orgMinY = (double)((LinearAxis)Chart.Axes[1]).ActualMinimum;
                orgMaxY = (double)((LinearAxis)Chart.Axes[1]).ActualMaximum;
                orgDataDirty = false;
            }

            LinearAxis xAxis = Chart.Axes[0] as LinearAxis;
            LinearAxis yAxis = Chart.Axes[1] as LinearAxis;
            double xAxisHit = (double)((IRangeAxis)xAxis).GetValueAtPosition(new UnitValue(e.GetPosition(xAxis).X, Unit.Pixels));
            double yAxisHit = (double)((IRangeAxis)yAxis).GetValueAtPosition(new UnitValue(yAxis.ActualHeight - e.GetPosition(yAxis).Y, Unit.Pixels));

            double curMinX = xAxis.ActualMinimum.Value;
            double curMaxX = xAxis.ActualMaximum.Value;
            double curMinY = yAxis.ActualMinimum.Value;
            double curMaxY = yAxis.ActualMaximum.Value;

            double zoomFactor = e.Delta < 0 ? 1.2 : 1 / 1.2;

            double newMinX = xAxisHit - zoomFactor * (xAxisHit - curMinX);
            double newMaxX = newMinX + (curMaxX - curMinX) * zoomFactor;
            double newMinY = yAxisHit - zoomFactor * (yAxisHit - curMinY);
            double newMaxY = newMinY + (curMaxY - curMinY) * zoomFactor;

            yAxis.Maximum = newMaxY > orgMaxY ? orgMaxY : newMaxY;
            xAxis.Maximum = newMaxX > orgMaxX ? orgMaxX : newMaxX;
            yAxis.Minimum = newMinY < orgMinY ? orgMinY : newMinY;
            xAxis.Minimum = newMinX < orgMinX ? orgMinX : newMinX;
        }
    }
}
