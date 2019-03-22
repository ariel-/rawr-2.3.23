using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Reflection;

namespace Rawr.Base
{
    public partial class Graph : Form
    {
        private Bitmap bitGraph;
        private Graphics g;
        private int graphHeight = 750;
        private int graphWidth = 800;

        public enum Style
        {
            DpsWarr,
            Mage,
        }

        public Graph()
        {
            bitGraph = new Bitmap(graphWidth, graphHeight);
            this.Height = graphHeight;
            this.Width = graphWidth;
            g = Graphics.FromImage(bitGraph);
            g.Clear(Color.White);
            InitializeComponent();
        }
       
        private void Graph_Load(object sender, EventArgs e)
        {
            pictureBoxGraph.Image = bitGraph;
            pictureBoxGraph.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxGraph.Height = graphHeight;
            pictureBoxGraph.Width = graphWidth;
        }

        public void SetupStatsGraph(Character character, Stats[] statsList, int scale, string explanatoryText, string calculation)
        {
            Cursor.Current = Cursors.WaitCursor;
            this.Text = "Graph of " + calculation;
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
            RenderStatsGraph(g, graphWidth, graphHeight, character, statsList, colors, scale, explanatoryText, calculation, Style.DpsWarr);
            Cursor.Current = Cursors.Default;
        }

        public static void RenderStatsGraph(Graphics g, int graphWidth, int graphHeight, Character character, Stats[] statsList, Color[] colors, int scale, string explanatoryText, string calculation, Style style)
        {
            CharacterCalculationsBase baseCalc = Calculations.GetCharacterCalculations(character);
            float baseFigure = GetCalculationValue(baseCalc, calculation);            
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            float graphOffset = graphWidth / 2.0f, graphStep = (graphWidth - 100) / 2.0f / scale;
            if (statsList.Length == 0 || statsList.Length > colors.Length) return; // more than 12 elements for the array would run out of colours
            float minDpsChange = 0f, maxDpsChange = 0f;
            PointF[][] points = new PointF[statsList.Length][];
            for (int index = 0; index < statsList.Length; index++)
            {
                Stats newStats = new Stats();
                points[index] = new PointF[2 * scale + 1];
                newStats.Accumulate(statsList[index], -scale - 1);
                for (int count = -scale; count <= scale; count++)
                {
                    newStats.Accumulate(statsList[index]);

                    CharacterCalculationsBase currentCalc = Calculations.GetCharacterCalculations(character, new Item() { Stats = newStats }, false, false, false);
                    float currentFigure = GetCalculationValue(currentCalc, calculation);
                    float dpsChange = currentFigure - baseFigure;
                    points[index][count + scale] = new PointF(graphOffset + count * graphStep, dpsChange);
                    if (dpsChange < minDpsChange)
                        minDpsChange = dpsChange;
                    if (dpsChange > maxDpsChange)
                        maxDpsChange = dpsChange;
                }
            }
            float DpsVariance = maxDpsChange - minDpsChange;
            if (DpsVariance == 0)
                DpsVariance = 1;
            for (int index = 0; index < statsList.Length; index++)
            {
                for (int count = -scale; count <= scale; count++)
                {
                    points[index][count + scale].Y = (int)((maxDpsChange - points[index][count + scale].Y) * (graphHeight - 48) / DpsVariance) + 20;
                }
                Brush statBrush = new SolidBrush(colors[index]);
                switch (style)
                {
                    case Style.DpsWarr:
                        g.DrawLines(new Pen(statBrush, 3), points[index]);
                        break;
                    case Style.Mage:
                        g.DrawLines(new Pen(statBrush, 1), points[index]);
                        break;
                }
            }

            RenderGrid(g, graphWidth, graphHeight, character, statsList, colors, scale, 1f, "F1", explanatoryText, calculation, style, minDpsChange, maxDpsChange, DpsVariance, true);
        }

        public static void RenderScalingGraph(Graphics g, int graphWidth, int graphHeight, Character character, Stats[] statsList, Stats baseStat, bool requiresReferenceCalculations, Color[] colors, int scale, string explanatoryText, string calculation, Style style)
        {
            CharacterCalculationsBase baseCalc = Calculations.GetCharacterCalculations(character);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            float graphOffset = graphWidth / 2.0f, graphStep = (graphWidth - 100) / 2.0f / scale;
            if (statsList.Length == 0 || statsList.Length > colors.Length) return; // more than 12 elements for the array would run out of colours
            float minDpsChange = 0f, maxDpsChange = 0f;
            PointF[][] points = new PointF[statsList.Length][];
            // extract property data for relative stats calculations
            KeyValuePair<PropertyInfo, float>[] properties = new KeyValuePair<PropertyInfo,float>[statsList.Length];
            for (int index = 0; index < statsList.Length; index++)
            {
                var p = statsList[index].Values(x => x > 0);
                foreach (var kvp in p)
                {
                    properties[index] = kvp;
                }
                points[index] = new PointF[2 * scale + 1];
            }
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
                    points[index][count + scale] = new PointF(graphOffset + count * graphStep, dpsChange);
                    if (dpsChange < minDpsChange)
                        minDpsChange = dpsChange;
                    if (dpsChange > maxDpsChange)
                        maxDpsChange = dpsChange;
                }
            }
            // restore reference calculation
            if (requiresReferenceCalculations)
            {
                Stats newStats = new Stats();
                Item item = new Item() { Stats = newStats };
                Calculations.GetCharacterCalculations(character, item, true, false, false);
            }
            // increase the spread a bit to so that you can see if something is at the edges and straight
            float DpsVariance = maxDpsChange - minDpsChange;
            minDpsChange -= DpsVariance * 0.05f;
            maxDpsChange += DpsVariance * 0.05f;
            DpsVariance = maxDpsChange - minDpsChange;
            if (DpsVariance == 0)
                DpsVariance = 1;
            for (int index = 0; index < statsList.Length; index++)
            {
                for (int count = -scale; count <= scale; count++)
                {
                    points[index][count + scale].Y = (int)((maxDpsChange - points[index][count + scale].Y) * (graphHeight - 48) / DpsVariance) + 20;
                }
                Brush statBrush = new SolidBrush(colors[index]);
                switch (style)
                {
                    case Style.DpsWarr:
                        g.DrawLines(new Pen(statBrush, 3), points[index]);
                        break;
                    case Style.Mage:
                        g.DrawLines(new Pen(statBrush, 1), points[index]);
                        break;
                }
            }
            float unit = 1f;
            var bp = baseStat.Values(x => x > 0);
            foreach (var kvp in bp)
            {
                unit = kvp.Value;
            }
            RenderGrid(g, graphWidth, graphHeight, character, statsList, colors, scale, unit, "F", explanatoryText, calculation, style, minDpsChange, maxDpsChange, DpsVariance, false);
        }

        private static void RenderGrid(Graphics g, int graphWidth, int graphHeight, Character character, Stats[] statsList, Color[] colors, int scale, float unit, string yFormat, string explanatoryText, string calculation, Style style, float minDpsChange, float maxDpsChange, float DpsVariance, bool zeroCentered)
        {
            float graphOffset = graphWidth / 2.0f, graphStep = (graphWidth - 100) / 2.0f / scale;

            #region Graph X Ticks
            float graphStart = graphOffset - scale * graphStep;
            float graphEnd = graphOffset + scale * graphStep;
            float activeWidth = graphEnd - graphStart;

            float maxScale = 2 * scale;
            float[] ticks = new float[] {(float)Math.Round(graphStart + activeWidth * 0.5f),
							(float)Math.Round(graphStart + activeWidth * 0.75f),
							(float)Math.Round(graphStart + activeWidth * 0.25f),
							(float)Math.Round(graphStart + activeWidth * 0.125f),
							(float)Math.Round(graphStart + activeWidth * 0.375f),
							(float)Math.Round(graphStart + activeWidth * 0.625f),
							(float)Math.Round(graphStart + activeWidth * 0.875f)};
            Pen ZeroLine = new Pen(Color.FromArgb(100, 0, 0, 0), 3);
            Pen black200 = new Pen(Color.FromArgb(200, 0, 0, 0));
            if (style == Style.Mage)
            {
                ZeroLine = black200;
            }
            Pen black150 = new Pen(Color.FromArgb(150, 0, 0, 0));
            Pen black75 = new Pen(Color.FromArgb(75, 0, 0, 0));
            Pen black50 = new Pen(Color.FromArgb(50, 0, 0, 0));
            Pen black25 = new Pen(Color.FromArgb(25, 0, 0, 0));
            StringFormat formatTick = new StringFormat();
            formatTick.LineAlignment = StringAlignment.Far;
            formatTick.Alignment = StringAlignment.Center;
            Brush black200brush = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
            Brush black150brush = new SolidBrush(Color.FromArgb(150, 0, 0, 0));
            Brush black75brush = new SolidBrush(Color.FromArgb(75, 0, 0, 0));
            Brush black50brush = new SolidBrush(Color.FromArgb(50, 0, 0, 0));
            Brush black25brush = new SolidBrush(Color.FromArgb(25, 0, 0, 0));

            g.DrawLine(black200, graphStart - 4, 20, graphEnd + 4, 20);
            g.DrawLine(black200, graphStart, 16, graphStart, graphHeight - 16);
            g.DrawLine(black200, graphEnd, 16, graphEnd, 19);
            g.DrawLine(ZeroLine, ticks[0], 16, ticks[0], 19);
            g.DrawLine(black150, ticks[1], 16, ticks[1], 19);
            g.DrawLine(black150, ticks[2], 16, ticks[2], 19);
            g.DrawLine(black75, ticks[3], 16, ticks[3], 19);
            g.DrawLine(black75, ticks[4], 16, ticks[4], 19);
            g.DrawLine(black75, ticks[5], 16, ticks[5], 19);
            g.DrawLine(black75, ticks[6], 16, ticks[6], 19);
            g.DrawLine(black75, graphEnd, 21, graphEnd, graphHeight - 16);
            g.DrawLine(ZeroLine, ticks[0], 21, ticks[0], graphHeight - 16);
            g.DrawLine(black50, ticks[1], 21, ticks[1], graphHeight - 16);
            g.DrawLine(black50, ticks[2], 21, ticks[2], graphHeight - 16);
            g.DrawLine(black25, ticks[3], 21, ticks[3], graphHeight - 16);
            g.DrawLine(black25, ticks[4], 21, ticks[4], graphHeight - 16);
            g.DrawLine(black25, ticks[5], 21, ticks[5], graphHeight - 16);
            g.DrawLine(black25, ticks[6], 21, ticks[6], graphHeight - 16);
            g.DrawLine(black200, graphStart - 4, graphHeight - 20, graphEnd + 4, graphHeight - 20);

            Font tickFont;
            switch (style)
            {
                case Style.Mage:
                    tickFont = new Font("Verdana", 10f, GraphicsUnit.Pixel);
                    break;
                case Style.DpsWarr:
                default:
                    tickFont = new Font("Calibri", 11);
                    break;
            }
            g.DrawString((unit * (-scale)).ToString(), tickFont, black200brush, graphStart, 16, formatTick);
            g.DrawString((unit * (maxScale - scale)).ToString(), tickFont, black200brush, graphEnd, 16, formatTick);
            g.DrawString((unit * (maxScale * 0.5f - scale)).ToString(), tickFont, black200brush, ticks[0], 16, formatTick);
            g.DrawString((unit * (maxScale * 0.75f - scale)).ToString(), tickFont, black150brush, ticks[1], 16, formatTick);
            g.DrawString((unit * (maxScale * 0.25f - scale)).ToString(), tickFont, black150brush, ticks[2], 16, formatTick);
            g.DrawString((unit * (maxScale * 0.125f - scale)).ToString(), tickFont, black75brush, ticks[3], 16, formatTick);
            g.DrawString((unit * (maxScale * 0.375f - scale)).ToString(), tickFont, black75brush, ticks[4], 16, formatTick);
            g.DrawString((unit * (maxScale * 0.625f - scale)).ToString(), tickFont, black75brush, ticks[5], 16, formatTick);
            g.DrawString((unit * (maxScale * 0.875f - scale)).ToString(), tickFont, black75brush, ticks[6], 16, formatTick);

            g.DrawString((unit * (-scale)).ToString(), tickFont, black200brush, graphStart, graphHeight - 16, formatTick);
            g.DrawString((unit * (maxScale - scale)).ToString(), tickFont, black200brush, graphEnd, graphHeight - 16, formatTick);
            g.DrawString((unit * (maxScale * 0.5f - scale)).ToString(), tickFont, black200brush, ticks[0], graphHeight - 16, formatTick);
            g.DrawString((unit * (maxScale * 0.75f - scale)).ToString(), tickFont, black150brush, ticks[1], graphHeight - 16, formatTick);
            g.DrawString((unit * (maxScale * 0.25f - scale)).ToString(), tickFont, black150brush, ticks[2], graphHeight - 16, formatTick);
            g.DrawString((unit * (maxScale * 0.125f - scale)).ToString(), tickFont, black75brush, ticks[3], graphHeight - 16, formatTick);
            g.DrawString((unit * (maxScale * 0.375f - scale)).ToString(), tickFont, black75brush, ticks[4], graphHeight - 16, formatTick);
            g.DrawString((unit * (maxScale * 0.625f - scale)).ToString(), tickFont, black75brush, ticks[5], graphHeight - 16, formatTick);
            g.DrawString((unit * (maxScale * 0.875f - scale)).ToString(), tickFont, black75brush, ticks[6], graphHeight - 16, formatTick);
            g.DrawString("Stat Change", tickFont, black200brush, activeWidth / 2 + 50, graphHeight, formatTick);
            #endregion

            #region Graph Y ticks
            Int32 zeroPoint = (int)(maxDpsChange * (graphHeight - 48) / DpsVariance) + 20;
            formatTick.Alignment = StringAlignment.Near;
            StringFormat formatName = new StringFormat(StringFormatFlags.DirectionVertical);
            g.DrawString(calculation, tickFont, black200brush, graphStart - 50, graphHeight / 2, formatName);
            g.DrawString(maxDpsChange.ToString(yFormat, CultureInfo.InvariantCulture), tickFont, black200brush, graphStart - 50, 30, formatTick);
            g.DrawString(minDpsChange.ToString(yFormat, CultureInfo.InvariantCulture), tickFont, black200brush, graphStart - 50, graphHeight - 12, formatTick);
            float[] dpsChange;
            if (maxDpsChange > 0 && minDpsChange < 0 && zeroCentered)
            {
                dpsChange = new float[] { maxDpsChange * .75f, maxDpsChange * .5f, maxDpsChange * .25f, 0f, minDpsChange * .25f, minDpsChange * .5f, minDpsChange * .75f };
            }
            else
            {
                dpsChange = new float[] { minDpsChange + DpsVariance * .125f, minDpsChange + DpsVariance * .25f, minDpsChange + DpsVariance * .375f, minDpsChange + DpsVariance * .5f, minDpsChange + DpsVariance * .625f, minDpsChange + DpsVariance * .75f, minDpsChange + DpsVariance * .875f };
            }
            for (int index = 0; index < 7; index++)
            {
                float dps = dpsChange[index];
                Pen pen;
                Brush brush;
                if (index == 3)
                {
                    pen = ZeroLine;
                    brush = black200brush;
                }
                else if (index % 2 == 1)
                {
                    pen = black150;
                    brush = black150brush;
                }
                else
                {
                    pen = black75;
                    brush = black75brush;
                }
                float pointY = (int)((maxDpsChange - dps) * (graphHeight - 48) / DpsVariance) + 20;
                g.DrawLine(pen, graphStart, pointY, graphEnd, pointY);
                g.DrawString(dps.ToString(yFormat, CultureInfo.InvariantCulture), tickFont, brush, graphStart - 50, pointY + 10, formatTick);
            }
            #endregion

            #region Key Legend
            switch (style)
            {
                case Style.Mage:
                    int legendY = (int)(graphHeight * 0.5f) + 16;
                    for (int i = 0; i < statsList.Length; i++)
                    {
                        g.DrawLine(new Pen(colors[i]), new Point((int)graphOffset + 20, legendY + 7), new Point((int)graphOffset + 50, legendY + 7));
                        g.DrawString(statsList[i].ToString(), tickFont, Brushes.Black, new Point((int)graphOffset + 60, legendY));

                        legendY += 16;
                    }
                    break;
                case Style.DpsWarr:
                default:
                    Font nameFont = new Font("Calibri", 12, FontStyle.Bold);
                    int nameX = (int)(activeWidth * .667f + graphStart);
                    for (int index = 0; index < statsList.Length; index++)
                    {
                        Brush nameBrush = new SolidBrush(colors[index]);
                        int nameY = (int)(graphHeight * .5f) + (int)(index * nameFont.Height);
                        g.DrawString(statsList[index].ToString(), nameFont, nameBrush, new PointF(nameX, nameY));
                    }
                    break;
            }
            #endregion

            #region Explanatory Text
            g.DrawString(explanatoryText, tickFont, black150brush, activeWidth * .1f, graphHeight * .05f);
            #endregion
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
    }
}
