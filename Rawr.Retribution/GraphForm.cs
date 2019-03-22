using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr.Retribution
{
    public partial class GraphForm : Form
    {
        private Graphics gfx;
        private Bitmap bmp = null;

        public GraphForm(Character character, bool bStats)
        {
            InitializeComponent();

            if (!bStats)
                Text = "Effective Cooldown Graph";

            // Create ARGB bitmap matching the size of the picturebox and associate with picturebox
            bmp = new Bitmap(pictureBoxGraph.Size.Width, pictureBoxGraph.Size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            pictureBoxGraph.Image = bmp;

            // Greate & initialise GDI+ graphics object via bitmap
            gfx = Graphics.FromImage(bmp);
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            
            #region Coordinates and scaling.
            /* 
             *   +-----------------------------------------------------+-rcBmp  ---
             *   |                                                     |         |EndY
             *   |    ^                                                |        ---
             *   |    |                                                |
             *   |    |                                                |
             *   |    |                                                |
             *   |    |                                                |
             *   |    |                                                |
             *   |    |                                                |
             *   |    |                                                |
             *   |    |                                                |
             *   |    |                                                |
             *   |    |                                                |
             *   |    |                                                |
             *   |    |                                                |
             *   |    |                                                |
             *   |    +--------------------------------------------->  | ---
             *   |                                                     |  |  OrgY
             *   +-----------------------------------------------------+ ---
             *   
             *   |----|OrgX                                         |--|EndX
             * 
             * 
             * 
             */

            // Apply a translateion transformation. We'll move the 0,0 to the 0,0 of the chart we want to display
            // This will make most of the rest of the charting easier since all translation are then done by GDI+
            // It also makes working with 'y' a bit easier as it's just flipped in sign rather than translated also.
            int OrgX = 50;  // Leftmost edge of char
            int OrgY = 50;  // Bottom edge of char
            int EndX = 15;  // Margin right of chart
            int EndY = 15;  // Margin top of chart
            gfx.TranslateTransform(OrgX, pictureBoxGraph.Size.Height - OrgY);

            // Bitmap coordinates translated back into transform
            Rectangle rcBmp = new Rectangle(0, 0, pictureBoxGraph.Size.Width, pictureBoxGraph.Size.Height);
            rcBmp.Offset(-OrgX, -(pictureBoxGraph.Size.Height - OrgY));

            // rcChart is image rectangle, watch out, Y has positive value !
            Rectangle rcChart = new Rectangle(0, 0, pictureBoxGraph.Size.Width - OrgX - EndX, pictureBoxGraph.Size.Height - OrgY - EndY);
            int YAxisValueX = - 5; // Right alignment of values on the Y axis
            int XAxisValueY = + 5; // Top margin of values on the X axis
            int YChartMargin = 5;
            #endregion

            #region Define and calculate charts
            ChartData[] aCharts;
            float DpsMax;
            float DpsMin;
            float DpsScaling;
            if (bStats)
            {
                aCharts = new ChartData[] {
                    new ChartData(rcChart.Width, Color.FromArgb(255, 192, 0, 0), "1 Strength", new Stats() { Strength = 1 }),
                    new ChartData(rcChart.Width, Color.FromArgb(255, 192, 192, 96), "1.167 Spell Power", new Stats() { SpellPower = 7/6 }),
                    new ChartData(rcChart.Width, Color.FromArgb(255, 0, 0, 192), "1 Armor Pen.", new Stats() { ArmorPenetrationRating = 1 }),
                    new ChartData(rcChart.Width, Color.FromArgb(255, 192, 0, 192), "1 Hit Rating", new Stats() { HitRating = 1 }),
                    new ChartData(rcChart.Width, Color.FromArgb(255, 0, 192, 0), "1 Expertise Rating", new Stats() { ExpertiseRating = 1 }),
                    new ChartData(rcChart.Width, Color.FromArgb(255, 192, 127, 96), "1 Agility", new Stats() { Agility = 1 }),
                    new ChartData(rcChart.Width, Color.FromArgb(255, 192, 127, 0), "2 Attack Power", new Stats() { AttackPower = 2 }),
                    new ChartData(rcChart.Width, Color.FromArgb(255, 96, 127, 192), "1 Crit Rating", new Stats() { CritRating = 1 }),
                    new ChartData(rcChart.Width, Color.FromArgb(255, 0, 0, 0), "1 Haste Rating", new Stats() { HasteRating = 1 }),
                };

                // Calculate charts
                CalculationsRetribution Calc = new CalculationsRetribution();
                DpsMax = 0;
                DpsMin = 999999999;
                foreach (ChartData cd in aCharts)
                {
                    for (int count = 0; count < rcChart.Width; count++)
                    {
                        Stats chartstats = cd.stats.Clone();
                        chartstats *= count - (rcChart.Width / 2);

                        CharacterCalculationsRetribution chartCalc = Calc.GetCharacterCalculations(character, new Item() { Stats = chartstats }) as CharacterCalculationsRetribution;
                        float Dps = chartCalc.DPSPoints;

                        if (Dps > DpsMax)
                            DpsMax = Dps;
                        if (Dps < DpsMin)
                            DpsMin = Dps;

                        cd.dps[count] = Dps;
                    }
                }
                DpsScaling = (rcChart.Height - 2 * YChartMargin/* some extra margin*/) / (DpsMax - DpsMin);
            }
            else 
            {
                aCharts = new ChartData[] {
                    new ChartData(rcChart.Width, Color.FromArgb(255, 192, 0, 0), "White", new Stats() { HasteRating = 1 }),
                    new ChartData(rcChart.Width, Color.FromArgb(255, 192, 192, 96), "CS", new Stats() { HasteRating = 1 }),
                    new ChartData(rcChart.Width, Color.FromArgb(255, 0, 0, 192), "J", new Stats() { HasteRating = 1 }),
                    new ChartData(rcChart.Width, Color.FromArgb(255, 192, 0, 192), "DS", new Stats() { HasteRating = 1 }),
                    new ChartData(rcChart.Width, Color.FromArgb(255, 0, 192, 0), "Cons", new Stats() { HasteRating = 1 }),
                    new ChartData(rcChart.Width, Color.FromArgb(255, 192, 127, 96), "Exo", new Stats() { HasteRating = 1 }),
                    //new ChartData(rcChart.Width, Color.FromArgb(255, 192, 127, 0), "HoW", new Stats() { HasteRating = 1 }),
                };

                // Calculate charts
                CalculationsRetribution Calc = new CalculationsRetribution();
                DpsMax = 0;
                DpsMin = 999999999;
                Stats stats = new Stats() { HasteRating = 1 };
                for (int count = 0; count < rcChart.Width; count++)
                {
                    Stats chartstats = stats.Clone();
                    chartstats *= count - (rcChart.Width / 2);

                    CharacterCalculationsRetribution chartCalc = Calc.GetCharacterCalculations(character, new Item() { Stats = chartstats }) as CharacterCalculationsRetribution;
                    float Ecd;

                    Ecd = chartCalc.AttackSpeed;
                    if (Ecd > DpsMax)
                        DpsMax = Ecd;
                    if (Ecd < DpsMin)
                        DpsMin = Ecd;
                    aCharts[0].dps[count] = Ecd;

                    Ecd = 1 / chartCalc.Solution.GetAbilityUsagePerSecond(Ability.CrusaderStrike);
                    if (Ecd > 50)  // infinity fix
                        Ecd = 0;
                    if (Ecd > DpsMax)
                        DpsMax = Ecd;
                    if (Ecd < DpsMin)
                        DpsMin = Ecd;
                    aCharts[1].dps[count] = Ecd;

                    Ecd = 1 / chartCalc.Solution.GetAbilityUsagePerSecond(Ability.Judgement);
                    if (Ecd > 50)
                        Ecd = 0;
                    if (Ecd > DpsMax)
                        DpsMax = Ecd;
                    if (Ecd < DpsMin)
                        DpsMin = Ecd;
                    aCharts[2].dps[count] = Ecd;

                    Ecd = 1 / chartCalc.Solution.GetAbilityUsagePerSecond(Ability.DivineStorm);
                    if (Ecd > 50)
                        Ecd = 0;
                    if (Ecd > DpsMax)
                        DpsMax = Ecd;
                    if (Ecd < DpsMin)
                        DpsMin = Ecd;
                    aCharts[3].dps[count] = Ecd;
                    Ecd = 1 / chartCalc.Solution.GetAbilityUsagePerSecond(Ability.Consecration);
                    if (Ecd > 50)
                        Ecd = 0;
                    if (Ecd > DpsMax)
                        DpsMax = Ecd;
                    if (Ecd < DpsMin)
                        DpsMin = Ecd;
                    aCharts[4].dps[count] = Ecd;
                    Ecd = 1 / chartCalc.Solution.GetAbilityUsagePerSecond(Ability.Exorcism);
                    if (Ecd > 50)
                        Ecd = 0;
                    if (Ecd > DpsMax)
                        DpsMax = Ecd;
                    if (Ecd < DpsMin)
                        DpsMin = Ecd;
                    aCharts[5].dps[count] = Ecd;
                    /*Ecd = 1 / chartCalc.Solution.GetAbilityUsagePerSecond(Ability.HammerOfWrath);
                    if (Ecd > 50)
                        Ecd = 0;
                    if (Ecd > DpsMax)
                        DpsMax = Ecd;
                    if (Ecd < DpsMin)
                        DpsMin = Ecd;
                    aCharts[6].dps[count] = Ecd;     */
                }
                DpsScaling = (rcChart.Height - 2 * YChartMargin/* some extra margin*/) / (DpsMax - DpsMin);
            }
            #endregion

            #region Fonts, Pens, Brushes and other helper variables
            Font fntValue = new Font("Arial", 8);
            Font fntTitle = new Font("Arial", 16);

            Pen penBlack = new Pen(Color.FromArgb(255, 0, 0, 0));
            Pen penGray50 = new Pen(Color.FromArgb(255, 127, 127, 127));
            Pen penGray75 = new Pen(Color.FromArgb(255, 196, 196, 196));
            Pen penGray90 = new Pen(Color.FromArgb(255, 230, 230, 230));
            Pen penRed = new Pen(Color.FromArgb(255, 255, 0, 0));

            Brush brBlack = new SolidBrush(Color.FromArgb(255, 0, 0, 0));
            Brush brWhite = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
            Brush brRed = new SolidBrush(Color.FromArgb(255, 255, 0, 0));

            StringFormat fmtRAlignCenter = new StringFormat();
            fmtRAlignCenter.Alignment = StringAlignment.Far;
            fmtRAlignCenter.LineAlignment = StringAlignment.Center;

            StringFormat fmtRAlignBottom = new StringFormat();
            fmtRAlignBottom.Alignment = StringAlignment.Far;
            fmtRAlignBottom.LineAlignment = StringAlignment.Far;

            StringFormat fmtLAlignTop = new StringFormat();
            fmtLAlignTop.Alignment = StringAlignment.Near;
            fmtLAlignTop.LineAlignment = StringAlignment.Near;

            StringFormat fmtCAlignTop = new StringFormat();
            fmtCAlignTop.Alignment = StringAlignment.Center;
            fmtCAlignTop.LineAlignment = StringAlignment.Near;
            #endregion

            // Clear bitmap to all white.
            gfx.FillRectangle(brWhite, rcBmp);

            #region Draw Axis and titles
            // Stats ticks (draw this first, so other axis information is drawn on top)
            const int Steps=10; // Chart is per 1 stat, typically, 20 stats go in a iLevel.
            for (int i = 1; i < (rcChart.Width / 2) / Steps; i++)
            {
                Pen pen;
                if (i % 5 == 0)
                {
                    pen = penGray75;
                    gfx.DrawString((-i * 10).ToString(), fntValue, brBlack, rcChart.Width / 2 - i * 10, XAxisValueY, fmtCAlignTop);
                    gfx.DrawString((i * 10).ToString(), fntValue, brBlack, rcChart.Width / 2 + i * 10, XAxisValueY, fmtCAlignTop);
                }
                else
                    pen = penGray90;

                gfx.DrawLine(pen, rcChart.Width / 2 - i * 10, 0, rcChart.Width / 2 - i * 10, -rcChart.Height);
                gfx.DrawLine(pen, rcChart.Width / 2 + i * 10, 0, rcChart.Width / 2 + i * 10, -rcChart.Height);
            }
            // DPS ticks (Draw this first, so other axis information is drawn on top)
            const int MaxDpsLines = 25;
            float DpsDelta = DpsMax - DpsMin;
            int[] Ranges = { 5, 10, 25, 50, 100, 250, 500, 1000, 2500, 5000, 10000, 25000, 50000, };
            int Range = 1;
            foreach (int r in Ranges)
            {
                if (MaxDpsLines * r > DpsDelta)
                {
                    Range = r;
                    break;
                }
            }
            int Y = (int)(Math.Floor(DpsMin / Range) * Range);
            do
            {
                float Y2 = (Y - DpsMin) * DpsScaling;
                if (Y2 > 0)
                {
                    gfx.DrawLine(penGray75, 0, -Y2, rcChart.Width - 5, -Y2);
                    gfx.DrawString(Y.ToString(), fntValue, brBlack, YAxisValueX, -Y2, fmtRAlignCenter);
                }

                Y += Range;
            }
            while (Y < DpsMax);
            
            // Y Axis
            gfx.DrawLine(penBlack, 0, 0, 0, -rcChart.Height);
            gfx.DrawString(bStats ? "DPS" : "ECD", fntTitle, brBlack, 5, -(rcChart.Height - 5), fmtLAlignTop);

            // X-axis
            gfx.DrawLine(penBlack, 0, 0, rcChart.Width, 0); 
            gfx.DrawString(bStats ? "Stats" : "Haste Rating", fntTitle, brBlack, rcChart.Width - 5, -5, fmtRAlignBottom);

            // Center Y Axis.
            gfx.DrawLine(penBlack, rcChart.Width / 2, 0, rcChart.Width / 2, -rcChart.Height);
            gfx.DrawString("0", fntValue, brBlack, rcChart.Width / 2, XAxisValueY, fmtCAlignTop);
            
            // "Experimental" text
            gfx.DrawString("Experimental", fntTitle, brRed, rcChart.Width-5, OrgY, fmtRAlignBottom);
            #endregion

            // Draw charts
            int l = 0;
            foreach (ChartData cd in aCharts)
            {
                for (int count = 0; count < rcChart.Width - 1; count++)
                    gfx.DrawLine(cd.pen, count, -(cd.dps[count] - DpsMin) * DpsScaling - YChartMargin, count + 1, -(cd.dps[count + 1] - DpsMin) * DpsScaling - YChartMargin);
                gfx.DrawString(cd.name, fntValue, cd.brush, 5, -(rcChart.Height - fntTitle.GetHeight() - 5 - l*fntValue.GetHeight()));
                l++;
            }
            Invalidate();
            Update();
        }
    }

    /// <summary>
    /// Helper class for charting
    /// </summary>
    public class ChartData
    {
        public ChartData(int points, Color color, string name, Stats stats)
        { 
            dps = new float[points];
            brush = new SolidBrush(color);
            pen = new Pen(brush, 2);
            this.name = name;
            this.stats = stats;
        }

        public float[] dps;
        public Pen pen;
        public Brush brush;
        public string name;
        public Stats stats;
    };

}
