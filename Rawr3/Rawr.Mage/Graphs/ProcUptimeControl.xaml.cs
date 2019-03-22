using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Charting;

namespace Rawr.Mage.Graphs
{
    /// <summary>
    /// Interaction logic for ProcUptimeControl.xaml
    /// </summary>
    public partial class ProcUptimeControl : UserControl
    {
        private static ProcUptimeControl instance = new ProcUptimeControl();

        public static ProcUptimeControl Instance
        {
            get
            {
                return instance;
            }
        }

        public ProcUptimeControl()
        {
            InitializeComponent();
        }

        public struct TimeData
        {
            public DateTime Time { get; set; }
            public double Value { get; set; }
        }

        public void UpdateGraph(DisplayCalculations calculations)
        {
            Chart.Series.Clear();
            Chart.Axes.Clear();

            DateTime baseTime = new DateTime(2000, 1, 1, 0, 0, 0);

            List<SpecialEffect> effectList = new List<SpecialEffect>();
            effectList.AddRange(calculations.SpellPowerEffects);
            effectList.AddRange(calculations.HasteRatingEffects);

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

            Style dateTimeAxisLabelStyle = new Style(typeof(DateTimeAxisLabel));
            dateTimeAxisLabelStyle.Setters.Add(new Setter(DateTimeAxisLabel.MinutesIntervalStringFormatProperty, "{0:m:ss}"));
            dateTimeAxisLabelStyle.Setters.Add(new Setter(DateTimeAxisLabel.SecondsIntervalStringFormatProperty, "{0:m:ss}"));
            DateTimeAxis timeAxis = new DateTimeAxis()
            {
                //Title = "Time",
                Minimum = baseTime,
                Maximum = baseTime + TimeSpan.FromSeconds(calculations.CalculationOptions.FightDuration),
                IntervalType = DateTimeIntervalType.Seconds,
                AxisLabelStyle = dateTimeAxisLabelStyle,
                Orientation = AxisOrientation.X,
                ShowGridLines = true,
                Location = AxisLocation.Top,
            };

            for (int i = 0; i < effectList.Count; i++)
            {
                float procs = 0.0f;
                float triggers = 0.0f;
                for (int j = 0; j < calculations.SolutionVariable.Count; j++)
                {
                    if (calculations.Solution[j] > 0)
                    {
                        Cycle c = calculations.SolutionVariable[j].Cycle;
                        if (c != null)
                        {
                            switch (effectList[i].Trigger)
                            {
                                case Trigger.DamageSpellCrit:
                                case Trigger.SpellCrit:
                                    triggers += (float)calculations.Solution[j] * c.Ticks / c.CastTime;
                                    procs += (float)calculations.Solution[j] * c.CritProcs / c.CastTime;
                                    break;
                                case Trigger.DamageSpellHit:
                                case Trigger.SpellHit:
                                    triggers += (float)calculations.Solution[j] * c.Ticks / c.CastTime;
                                    procs += (float)calculations.Solution[j] * c.HitProcs / c.CastTime;
                                    break;
                                case Trigger.SpellMiss:
                                    triggers += (float)calculations.Solution[j] * c.Ticks / c.CastTime;
                                    procs += (float)calculations.Solution[j] * (1 - c.HitProcs) / c.CastTime;
                                    break;
                                case Trigger.DamageSpellCast:
                                case Trigger.SpellCast:
                                    if (effectList[i].Stats.ValkyrDamage > 0)
                                    {
                                        triggers += (float)calculations.Solution[j] * c.CastProcs2 / c.CastTime;
                                        procs += (float)calculations.Solution[j] * c.CastProcs2 / c.CastTime;
                                    }
                                    else
                                    {
                                        triggers += (float)calculations.Solution[j] * c.CastProcs / c.CastTime;
                                        procs += (float)calculations.Solution[j] * c.CastProcs / c.CastTime;
                                    }
                                    break;
                                case Trigger.MageNukeCast:
                                    triggers += (float)calculations.Solution[j] * c.NukeProcs / c.CastTime;
                                    procs += (float)calculations.Solution[j] * c.NukeProcs / c.CastTime;
                                    break;
                                case Trigger.DamageDone:
                                case Trigger.DamageOrHealingDone:
                                    triggers += (float)calculations.Solution[j] * c.DamageProcs / c.CastTime;
                                    procs += (float)calculations.Solution[j] * c.DamageProcs / c.CastTime;
                                    break;
                                case Trigger.DoTTick:
                                    triggers += (float)calculations.Solution[j] * c.DotProcs / c.CastTime;
                                    procs += (float)calculations.Solution[j] * c.DotProcs / c.CastTime;
                                    break;
                            }
                        }
                    }
                }
                float triggerInterval = calculations.CalculationOptions.FightDuration / triggers;
                float triggerChance = Math.Min(1.0f, procs / triggers);

                int steps = 200;
                TimeData[] plot = new TimeData[steps + 1];
                for (int tick = 0; tick <= steps; tick++)
                {
                    float time = tick / (float)steps * calculations.CalculationOptions.FightDuration;
                    plot[tick] = new TimeData() { Time = baseTime + TimeSpan.FromSeconds(time), Value = effectList[i].GetUptimePlot(triggerInterval, triggerChance, 3.0f, time) };
                }

                Style style = new Style(typeof(LineDataPoint));
                style.Setters.Add(new Setter(LineDataPoint.TemplateProperty, Resources["LineDataPointTemplate"]));
                style.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, new SolidColorBrush(colors[i])));
                Chart.Series.Add(new LineSeries()
                {
                    Title = effectList[i].ToString(),
                    ItemsSource = plot,
                    IndependentValuePath = "Time",
                    DependentValuePath = "Value",
                    DataPointStyle = style,
                    IndependentAxis = timeAxis,
                });
            }
        }
    }
}
