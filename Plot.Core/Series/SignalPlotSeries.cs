using Plot.Core.Draws;
using Plot.Core.Renderables.Axes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Plot.Core.Series
{
    public class SignalPlotSeries : IPlotSeries
    {
        public SignalPlotSeries(Axis xAxis, Axis yAxis)
        {
            XAxis = xAxis;
            YAxis = yAxis;
        }

        public Axis XAxis { get; }
        public Axis YAxis { get; }

        public Color Color { get; set; } = Color.Red;
        public float LineWidth { get; set; } = 1f;
        public string Label { get; set; } = null;

        public int SampleRate { get; set; }
        public double SampleInterval => 1.0 / SampleRate;

        public double DataMin { get; private set; } = double.MaxValue;
        public double DataMax { get; private set; } = double.MinValue;

        public double[] Data { get; set; }

        public long Count => Data.Length;


        public AxisLimits GetAxisLimits()
        {
            return new AxisLimits(0, Data.Length * SampleInterval, Data.Min(), Data.Max());
        }

        public void Plot(Bitmap bmp, bool lowQuality, float scale)
        {
            PlotDimensions Dims = XAxis.CreatePlotDimensions(YAxis, scale);

            if (Data == null || Data.Length == 0) return;
            List<PointF> points = new List<PointF>();

            double lastPointX = Count * SampleInterval;
            float dataMinPx = Dims.GetPixelX(Dims.m_xMin);
            float dataMaxPx = Dims.GetPixelX(lastPointX - Dims.m_xMin);

            double dataPointsPerpx = Dims.m_unitsPerPxX / SampleInterval;

            if (dataPointsPerpx < 1)
            {
                // LOW DENSITY
                // Per pixel only draw one point
                double l = dataMinPx * dataPointsPerpx;
                double r = l + dataPointsPerpx * Dims.m_plotWidth;
                for (int i = (int)Math.Max(0, l - 2); i < (int)Math.Min(r + 3, Count - 1); i++)
                {
                    float x = Dims.GetPixelX(i * SampleInterval);
                    float y = Dims.GetPixelY(Data[i]);

                    points.Add(new PointF(x, y));
                }
            }
            else
            {
                // HIGHE DENSITY
                // Per pixel lastly draw one point
                for (float i = Math.Max(0, dataMinPx); i < Math.Min(Dims.m_plotWidth, dataMaxPx); i++)
                {
                    int l = (int)Math.Round(dataPointsPerpx * (i - dataMinPx));
                    int r = (int)Math.Round(l + dataPointsPerpx);
                    l = Math.Max(l, 0);
                    r = Math.Min((int)Count - 1, r);
                    r = Math.Max(r, 0);
                    if (l == r)
                        continue;
                    (double min, double max) = GetMaxMinValue(l, r - l);

                    points.Add(new PointF(i, Dims.GetPixelY(min)));
                    points.Add(new PointF(i, Dims.GetPixelY(max)));
                }
            }

            using (var gfx = GDI.Graphics(bmp, Dims, lowQuality, true))
            using (var pen = GDI.Pen(Color, LineWidth))
            {
                if (points.Count > 1)
                    gfx.DrawLines(pen, points.ToArray());
            }
        }

        private (double, double) GetMaxMinValue(int l, int r)
        {
            double min = double.MaxValue, max = double.MinValue;
            for (int i = l; i < l + r; i++)
            {
                min = Math.Min(min, Data[i]);
                max = Math.Max(max, Data[i]);
            }

            return (min, max);
        }

        public void ValidateData()
        {

        }
    }
}
