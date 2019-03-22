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
using Rawr.Mage.SequenceReconstruction;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.DataVisualization;

namespace Rawr.Mage.Graphs
{
    public partial class SequenceReconstructionControl : UserControl
    {
        private static SequenceReconstructionControl instance = new SequenceReconstructionControl();

        public static SequenceReconstructionControl Instance
        {
            get
            {
                return instance;
            }
        }

        public SequenceReconstructionControl()
        {
            InitializeComponent();
        }

        public struct TimeData
        {
            public DateTime Time { get; set; }
            public double Value { get; set; }
        }

        public struct TimeIntervalData
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public object Category { get; set; }
        }

        [StyleTypedProperty(Property = "GridLineStyle", StyleTargetType = typeof(Line))]
        [StyleTypedProperty(Property = "MajorTickMarkStyle", StyleTargetType = typeof(Line))]
        [StyleTypedProperty(Property = "MinorTickMarkStyle", StyleTargetType = typeof(Line))]
        [StyleTypedProperty(Property = "AxisLabelStyle", StyleTargetType = typeof(NumericAxisLabel))]
        [StyleTypedProperty(Property = "TitleStyle", StyleTargetType = typeof(Title))]
        [TemplatePart(Name = AxisGridName, Type = typeof(Grid))]
        [TemplatePart(Name = AxisTitleName, Type = typeof(Title))]
        public class OffsetLinearAxis : LinearAxis
        {
            public double Offset { get; set; }

            protected override UnitValue GetPlotAreaCoordinate(object value, Range<IComparable> currentRange, double length)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (currentRange.HasData)
                {
                    double doubleValue = ValueHelper.ToDouble(value);
                    Range<double> actualDoubleRange = currentRange.ToDoubleRange();

                    double pixelLength = Math.Max(length - 1 - Offset, 0);
                    double rangelength = actualDoubleRange.Maximum - actualDoubleRange.Minimum;

                    return new UnitValue(Offset + (doubleValue - actualDoubleRange.Minimum) * (pixelLength / rangelength), Unit.Pixels);
                }

                return UnitValue.NaN();
            }

            protected override IComparable GetValueAtPosition(UnitValue value)
            {
                if (ActualRange.HasData && ActualLength != 0.0)
                {
                    if (value.Unit == Unit.Pixels)
                    {
                        double coordinate = value.Value - Offset;
                        Range<double> actualDoubleRange = ActualRange.ToDoubleRange();

                        double rangelength = actualDoubleRange.Maximum - actualDoubleRange.Minimum;
                        double output = ((coordinate * (rangelength / (ActualLength - Offset))) + actualDoubleRange.Minimum);

                        return output;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                return null;
            }
        }

        private class ZeroLineCanvas : Canvas, IAxisListener
        {
            private DisplayAxis _axis;

            /// <summary>
            /// Gets the axis that the grid lines are connected to.
            /// </summary>
            public DisplayAxis Axis
            {
                get { return _axis; }
                set
                {
                    if (_axis != value)
                    {
                        DisplayAxis oldValue = _axis;
                        _axis = value;
                        if (oldValue != _axis)
                        {
                            OnAxisPropertyChanged(oldValue, value);
                        }
                    }
                }
            }

            /// <summary>
            /// AxisProperty property changed handler.
            /// </summary>
            /// <param name="oldValue">Old value.</param>
            /// <param name="newValue">New value.</param>
            private void OnAxisPropertyChanged(DisplayAxis oldValue, DisplayAxis newValue)
            {
                if (newValue != null)
                {
                    newValue.RegisteredListeners.Add(this);
                }

                if (oldValue != null)
                {
                    oldValue.RegisteredListeners.Remove(this);
                }
            }

            /// <summary>
            /// Instantiates a new instance of the DisplayAxisGridLines class.
            /// </summary>
            /// <param name="axis">The axis used by the DisplayAxisGridLines.</param>
            public ZeroLineCanvas(DisplayAxis axis)
            {
                this.Axis = axis;
                this.SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
            }

            /// <summary>
            /// Redraws grid lines when the size of the control changes.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">Information about the event.</param>
            private void OnSizeChanged(object sender, SizeChangedEventArgs e)
            {
                Invalidate();
            }

            /// <summary>
            /// Redraws grid lines when the axis is invalidated.
            /// </summary>
            /// <param name="axis">The invalidated axis.</param>
            public void AxisInvalidated(IAxis axis)
            {
                Invalidate();
            }

            public int BarCount { get; set; }

            /// <summary>
            /// Draws the grid lines.
            /// </summary>
            public void Invalidate()
            {
                Children.Clear();

                Line zeroLine = new Line { Style = Axis.GridLineStyle };
                double maximumHeight = Math.Max(Math.Round(ActualHeight - 1), 0);
                double maximumWidth = Math.Max(Math.Round(ActualWidth - 1), 0);
                double position = BarCount * 10.0;
                zeroLine.Y1 = zeroLine.Y2 = maximumHeight - Math.Round(position - (zeroLine.StrokeThickness / 2));
                zeroLine.X1 = 0.0;
                zeroLine.X2 = maximumWidth;

                Children.Add(zeroLine);
            }
        }

        private ZeroLineCanvas zeroLineCanvas;

        public void UpdateGraph(CalculationOptionsMage calculationOptions)
        {
            DisplayCalculations calculations = calculationOptions.Calculations;

            Chart.Series.Clear();
            Chart.Axes.Clear();

            Chart.Text = null;

            if (calculationOptions.SequenceReconstruction == null)
            {
                Chart.Text = "Sequence reconstruction data is not available.";
            }
            else
            {
                List<EffectCooldown> cooldownList = calculationOptions.Calculations.CooldownList;

                /*brushSubPoints = new Brush[cooldownList.Count];
                colorSubPointsA = new Color[cooldownList.Count];
                colorSubPointsB = new Color[cooldownList.Count];
                for (int i = 0; i < cooldownList.Count; i++)
                {
                    Color baseColor = cooldownList[i].Color;
                    brushSubPoints[i] = new SolidBrush(Color.FromArgb(baseColor.R / 2, baseColor.G / 2, baseColor.B / 2));
                    colorSubPointsA[i] = Color.FromArgb(baseColor.A / 2, baseColor.R / 2, baseColor.G / 2, baseColor.B / 2);
                    colorSubPointsB[i] = Color.FromArgb(baseColor.A / 2, baseColor);
                }
                StringFormat formatSubPoint = new StringFormat();
                formatSubPoint.Alignment = StringAlignment.Center;
                formatSubPoint.LineAlignment = StringAlignment.Center;

                int maxWidth = 1;
                for (int i = 0; i < cooldownList.Count; i++)
                {
                    string subPointName = cooldownList[i].Name;
                    int widthSubPoint = (int)Math.Ceiling(g.MeasureString(subPointName, fontLegend).Width + 2f);
                    if (widthSubPoint > maxWidth) maxWidth = widthSubPoint;
                }
                for (int i = 0; i < cooldownList.Count; i++)
                {
                    string cooldownName = cooldownList[i].Name;
                    rectSubPoint = new Rectangle(2, legendY, maxWidth, 16);
                    blendSubPoint = new System.Drawing.Drawing2D.ColorBlend(3);
                    blendSubPoint.Colors = new Color[] { colorSubPointsA[i], colorSubPointsB[i], colorSubPointsA[i] };
                    blendSubPoint.Positions = new float[] { 0f, 0.5f, 1f };
                    brushSubPointFill = new System.Drawing.Drawing2D.LinearGradientBrush(rectSubPoint, colorSubPointsA[i], colorSubPointsB[i], 67f);
                    brushSubPointFill.InterpolationColors = blendSubPoint;

                    g.FillRectangle(brushSubPointFill, rectSubPoint);
                    g.DrawRectangle(new Pen(brushSubPointFill), rectSubPoint);
                    g.DrawRectangle(new Pen(brushSubPointFill), rectSubPoint);
                    g.DrawRectangle(new Pen(brushSubPointFill), rectSubPoint);

                    g.DrawString(cooldownName, fontLegend, brushSubPoints[i], rectSubPoint, formatSubPoint);
                    legendY += 16;
                }*/

                if (calculationOptions.AdviseAdvancedSolver)
                {
                    Chart.Text = "Sequence Reconstruction was not fully successful, it is recommended that you enable more options in advanced solver (segment cooldowns, integral mana consumables, advanced constraints options)!";
                }

                /*g.DrawLine(Pens.Aqua, new Point(maxWidth + 40, 10), new Point(maxWidth + 80, 10));
                g.DrawString("Mana", fontLegend, Brushes.Black, new Point(maxWidth + 90, 2));
                g.DrawLine(Pens.Red, new Point(maxWidth + 40, 26), new Point(maxWidth + 80, 26));
                g.DrawString("Dps", fontLegend, Brushes.Black, new Point(maxWidth + 90, 18));*/

                List<SequenceItem> sequence = calculationOptions.SequenceReconstruction.sequence;

                List<TimeData> manaList = new List<TimeData>();

                float mana = calculations.StartingMana;
                int gemCount = 0;
                float time = 0;
                Color manaFill = Color.FromArgb(50, 0, 0, 255);
                float maxMana = calculations.BaseStats.Mana;
                float maxDps = 100;
                DateTime baseTime = new DateTime(2000, 1, 1, 0, 0, 0);
                manaList.Add(new TimeData() { Time = baseTime, Value = mana });
                for (int i = 0; i < sequence.Count; i++)
                {
                    int index = sequence[i].Index;
                    VariableType type = sequence[i].VariableType;
                    float duration = (float)sequence[i].Duration;
                    Cycle cycle = sequence[i].Cycle;
                    if (cycle != null && cycle.DamagePerSecond > maxDps) maxDps = cycle.DamagePerSecond;
                    CastingState state = sequence[i].CastingState;
                    float mps = (float)sequence[i].Mps;
                    if (sequence[i].IsManaPotionOrGem)
                    {
                        duration = 0;
                        if (sequence[i].VariableType == VariableType.ManaGem)
                        {
                            mana += (float)((1 + calculations.BaseStats.BonusManaGem) * calculations.ManaGemValue);
                            gemCount++;
                        }
                        else if (sequence[i].VariableType == VariableType.ManaPotion)
                        {
                            mana += (float)((1 + calculations.BaseStats.BonusManaPotion) * calculations.ManaPotionValue);
                        }
                        if (mana < 0) mana = 0;
                        if (mana > maxMana)
                        {
                            mana = maxMana;
                        }
                        manaList.Add(new TimeData() { Time = baseTime + TimeSpan.FromSeconds(time), Value = mana });
                    }
                    else
                    {
                        if (sequence[i].IsEvocation)
                        {
                            switch (sequence[i].VariableType)
                            {
                                case VariableType.Evocation:
                                    mps = -(float)calculationOptions.Calculations.EvocationRegen;
                                    break;
                                case VariableType.EvocationIV:
                                    mps = -(float)calculationOptions.Calculations.EvocationRegenIV;
                                    break;
                                case VariableType.EvocationHero:
                                    mps = -(float)calculationOptions.Calculations.EvocationRegenHero;
                                    break;
                                case VariableType.EvocationIVHero:
                                    mps = -(float)calculationOptions.Calculations.EvocationRegenIVHero;
                                    break;
                            }
                        }
                        float partTime = duration;
                        if (mana - mps * duration < 0) partTime = mana / mps;
                        else if (mana - mps * duration > maxMana) partTime = (mana - maxMana) / mps;
                        mana -= mps * duration;
                        if (mana < 0) mana = 0;
                        if (mana > maxMana)
                        {
                            mana = maxMana;
                        }
                        manaList.Add(new TimeData() { Time = baseTime + TimeSpan.FromSeconds(time + partTime), Value = mana });
                        if (partTime < duration)
                        {
                            manaList.Add(new TimeData() { Time = baseTime + TimeSpan.FromSeconds(time + duration), Value = mana });
                        }
                    }
                    time += duration;
                }

                Style dateTimeAxisLabelStyle = new Style(typeof(DateTimeAxisLabel));
                dateTimeAxisLabelStyle.Setters.Add(new Setter(DateTimeAxisLabel.MinutesIntervalStringFormatProperty, "{0:m:ss}"));
                dateTimeAxisLabelStyle.Setters.Add(new Setter(DateTimeAxisLabel.SecondsIntervalStringFormatProperty, "{0:m:ss}"));
                DateTimeAxis timeAxis = new DateTimeAxis()
                {
                    //Title = "Time",
                    Minimum = baseTime,
                    Maximum = baseTime + TimeSpan.FromSeconds(calculationOptions.FightDuration),
                    IntervalType = DateTimeIntervalType.Seconds,
                    AxisLabelStyle = dateTimeAxisLabelStyle,
                    Orientation = AxisOrientation.X,
                    ShowGridLines = true,
                    Location = AxisLocation.Top,
                };

                Style hiddenCategoryLabelStyle = new Style(typeof(AxisLabel));
                hiddenCategoryLabelStyle.Setters.Add(new Setter(NumericAxisLabel.VisibilityProperty, Visibility.Collapsed));

                CategoryAxis categoryAxis = new CategoryAxis()
                {
                    AxisLabelStyle = hiddenCategoryLabelStyle,
                    Orientation = AxisOrientation.Y,
                    MajorTickMarkStyle = null,
                };

                int barCount = 0;
                for (int cooldown = 0; cooldown < cooldownList.Count; cooldown++)
                {
                    List<TimeIntervalData> data = new List<TimeIntervalData>();
                    //blendSubPoint = new System.Drawing.Drawing2D.ColorBlend(3);
                    //blendSubPoint.Colors = new Color[] { colorSubPointsA[cooldown], colorSubPointsB[cooldown], colorSubPointsA[cooldown] };
                    //blendSubPoint.Positions = new float[] { 0f, 0.5f, 1f };
                    bool on = false;
                    float timeOn = 0.0f;
                    time = 0;
                    for (int i = 0; i < sequence.Count; i++)
                    {
                        float duration = (float)sequence[i].Duration;
                        if (sequence[i].IsManaPotionOrGem) duration = 0;
                        if (on && !sequence[i].CastingState.EffectsActive(cooldownList[cooldown]) && !sequence[i].IsManaPotionOrGem)
                        {
                            on = false;
                            if (time > timeOn)
                            {
                                data.Add(new TimeIntervalData() { Start = baseTime + TimeSpan.FromSeconds(timeOn), End = baseTime + TimeSpan.FromSeconds(time), Category = cooldownList[cooldown].Name });
                            }
                        }
                        else if (!on && sequence[i].CastingState.EffectsActive(cooldownList[cooldown]))
                        {
                            on = true;
                            timeOn = time;
                        }
                        time += duration;
                    }
                    if (on)
                    {
                        if (time - timeOn > 0)
                        {
                            data.Add(new TimeIntervalData() { Start = baseTime + TimeSpan.FromSeconds(timeOn), End = baseTime + TimeSpan.FromSeconds(time), Category = cooldownList[cooldown].Name });
                        }
                    }
                    if (data.Count > 0)
                    {
                        barCount++;
                        Style timeIntervalStyle = new Style(typeof(TimeIntervalDataPoint));
                        timeIntervalStyle.Setters.Add(new Setter(TimeIntervalDataPoint.BackgroundProperty, new SolidColorBrush(cooldownList[cooldown].Color)));
                        Chart.Series.Add(new TimeIntervalSeries()
                        {
                            Title = cooldownList[cooldown].Name,
                            ItemsSource = data,
                            IndependentValuePath = "Category",
                            DependentValuePath = "End",
                            StartTimePath = "Start",
                            EndTimePath = "End",
                            DataPointStyle = timeIntervalStyle,
                            DependentRangeAxis = timeAxis,
                            IndependentAxis = categoryAxis
                        });
                    }
                }

                Style hiddenNumericLabelStyle = new Style(typeof(NumericAxisLabel));
                hiddenNumericLabelStyle.Setters.Add(new Setter(NumericAxisLabel.VisibilityProperty, Visibility.Collapsed));

                Chart.Series.Add(new AreaSeries()
                {
                    Title = "Mana",
                    ItemsSource = manaList,
                    IndependentValuePath = "Time",
                    DependentValuePath = "Value",
                    DataPointStyle = (Style)Resources["ManaStyle"],
                    Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF)),
                    DependentRangeAxis = new OffsetLinearAxis()
                    {
                        Minimum = 0,
                        Offset = barCount * 10.0,
                        Orientation = AxisOrientation.Y,
                        AxisLabelStyle = hiddenNumericLabelStyle,
                        MajorTickMarkStyle = null,
                        MinorTickMarkStyle = null,                        
                    },
                    IndependentAxis = timeAxis,
                    LegendItemStyle = null,
                });

                maxDps *= 1.1f;
                List<TimeData> list = new List<TimeData>();
                time = 0.0f;
                for (int i = 0; i < sequence.Count; i++)
                {
                    int index = sequence[i].Index;
                    VariableType type = sequence[i].VariableType;
                    float duration = (float)sequence[i].Duration;
                    Cycle cycle = sequence[i].Cycle;
                    CastingState state = sequence[i].CastingState;
                    float mps = (float)sequence[i].Mps;
                    if (sequence[i].IsManaPotionOrGem) duration = 0;
                    float dps = 0;
                    if (cycle != null)
                    {
                        dps = cycle.DamagePerSecond;
                    }
                    if (duration > 0)
                    {
                        list.Add(new TimeData() { Time = baseTime + TimeSpan.FromSeconds(time + 0.1f * duration), Value = dps });
                        list.Add(new TimeData() { Time = baseTime + TimeSpan.FromSeconds(time + 0.9f * duration), Value = dps });
                    }
                    time += duration;
                }

                Chart.Series.Add(new LineSeries()
                {
                    Title = "Dps",
                    ItemsSource = list,
                    IndependentValuePath = "Time",
                    DependentValuePath = "Value",
                    DataPointStyle = (Style)Resources["DpsStyle"],
                    DependentRangeAxis = new OffsetLinearAxis()
                    {
                        Offset = barCount * 10.0,
                        Minimum = 0,
                        Orientation = AxisOrientation.Y,
                        AxisLabelStyle = hiddenNumericLabelStyle,
                        MajorTickMarkStyle = null,
                        MinorTickMarkStyle = null,
                    },
                    IndependentAxis = timeAxis,
                });

                if (zeroLineCanvas == null)
                {
                    zeroLineCanvas = new ZeroLineCanvas(categoryAxis);
                    zeroLineCanvas.BarCount = barCount;
                }
                else
                {
                    zeroLineCanvas.BarCount = barCount;
                    zeroLineCanvas.Axis = categoryAxis;                    
                }

                ISeriesHost host = (ISeriesHost)Chart;
                if (!host.BackgroundElements.Contains(zeroLineCanvas))
                {
                    host.BackgroundElements.Add(zeroLineCanvas);
                }
            }
        }
    }
}
