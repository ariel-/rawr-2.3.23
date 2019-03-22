using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;

namespace Rawr
{
	public class StatGraphRenderer
	{
		public int MinimumX = -100;
		public int MaximumX = 100;
		public int GranularityX = 10;
		public string StatX = "Agility";

		private Pen PenStat;
		private Brush BrushStat;
		private Pen PenAxis;
		private Brush BrushAxis;
		private Font FontLabels;
		private Pen PenGrid;

		private float MinimumY = 0f;
		private float MaximumY = 0f;
		private float Width = 0f;
		private float Height = 0f;
		//private float RangeX = 0f;
		//private float RangeY = 0f;
		private float ConversionX = 0f;
		private float ConversionY = 0f;

		public StatGraphRenderer()
		{
			PenStat = new Pen(Color.FromArgb(224, 192, 0, 255));
			BrushStat = new SolidBrush(Color.FromArgb(128, 192, 0, 255));
			PenAxis = new Pen(Color.FromArgb(224, 0, 0, 0));
			BrushAxis = new SolidBrush(Color.FromArgb(224, 0, 0, 0));
			FontLabels = new Font(Control.DefaultFont.FontFamily, 9f);
			PenGrid = new Pen(Color.FromArgb(224, 224, 244));
		}

		public void Render(Character character, System.Drawing.Graphics g, int width, int height)
		{
			//MinimumX = -1000; MaximumX = 1000; GranularityX = 2;

			Width = width - 16; Height = height - 24;
			MinimumY = MaximumY = 0f;
			string[] rotations;
			float[] graphData = BuildGraphData(character, out MinimumY, out MaximumY, out rotations);
			MinimumY = 0f;//(float)Math.Floor(MinimumY / 10f) * 10f;
			MaximumY = 5f;//(float)Math.Ceiling(MaximumY / 10f) * 10f;
			//RangeX = MaximumX - MinimumX;
			//RangeY = MaximumY - MinimumY;
			ConversionX = (width - 16f) / (MaximumX - MinimumX);
			ConversionY = (height - 24f) / (MaximumY - MinimumY);

			PointF[] points = new PointF[graphData.Length + 2];
			points[0] = GetScreenPoint(MinimumX, 0);
			points[points.Length - 1] = GetScreenPoint(MaximumX, 0);
			byte[] types = new byte[points.Length];
			types[0] = (byte)PathPointType.Start;
			types[types.Length - 1] = (byte)PathPointType.Line;
			float x = 0, integralY = 0f; 
			for (int i = 1; i <= graphData.Length; i++)
			{
				x = MinimumX + (float)(i - 1) / (float)GranularityX;
				integralY = 0f;
				if (i < graphData.Length - 1) integralY = (graphData[i] - graphData[i - 1]);
				points[i] = GetScreenPoint(x, integralY);
				types[i] = (byte)PathPointType.Line;
			}

			GraphicsPath pathData = new GraphicsPath(points, types, FillMode.Winding);

			g.FillPath(BrushStat, pathData);
			g.DrawPath(PenStat, pathData);
		}

		private PointF GetScreenPoint(float xData, float yData)
		{
			return new PointF(8f + (xData - MinimumX) * ConversionX,
					8f + Height - (yData - MinimumY) * ConversionY);
		}

		private float[] BuildGraphData(Character character, out float minY, out float maxY, out string[] rotations)
		{
			minY = float.MaxValue;
			maxY = 0f;
			float[] overallData = new float[(MaximumX - MinimumX) * GranularityX + 1];
			rotations = new string[overallData.Length];
			Item itemStats = new Item() { Stats = new Stats() };
			PropertyInfo propertyX = typeof(Stats).GetProperty(StatX);
			float y, x;
			for (int i = 0; i < overallData.Length; i++)
			{
				try
				{
					x = MinimumX + (float)i / (float)GranularityX;
					propertyX.SetValue(itemStats.Stats, x, null);
					CharacterCalculationsBase calcs = Calculations.GetCharacterCalculations(character, itemStats,
						false, true, false);
					y = calcs.OverallPoints;
					//rotations[i] = calcs.GetCharacterDisplayCalculationValues()["Optimal Rotation"];
					overallData[i] = y;
					minY = Math.Min(minY, y);
					maxY = Math.Max(maxY, y);
				}
				catch (Exception ex)
				{
					ex.ToString();
				}
			}

			return overallData;
		}
	}
}
