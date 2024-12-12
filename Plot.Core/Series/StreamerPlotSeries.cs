using Plot.Core.Draws;
using Plot.Core.Renderables.Axes;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Plot.Core.Series
{
    public class StreamerPlotSeries : IPlotSeries
    {
        public StreamerPlotSeries(Axis xAxis, Axis yAxis, int pointCount)
        {
            XAxis = xAxis;
            YAxis = yAxis;

            Data = new double[pointCount];
        }

        public Axis XAxis { get; }
        public Axis YAxis { get; }

        public Color Color { get; set; } = Color.Red;
        public float LineWidth { get; set; } = 1f;
        public string Label { get; set; } = null;

        public int SampleRate { get; set; }
        public double SampleInterval => 1.0 / SampleRate;
        public double[] Data { get; }

        public void ValidateData() { }

        public void Plot(Bitmap bmp, bool lowQuality, float scale)
        {
            if (Data == null || Data.Length == 0) return;

            PlotDimensions Dims = XAxis.CreatePlotDimensions(YAxis, scale);

            PointF[] points = new PointF[Data.Length];
            for (int i = 0; i < Data.Length; i++)
            {
                double dx = i * SampleInterval + Dims.m_xMin;
                float x = Dims.GetPixelX(dx);
                float y = Dims.GetPixelY(Data[i]);

                points[i] = new PointF(x, y);
            }

            using (var gfx = GDI.Graphics(bmp, Dims, lowQuality, true))
            using (var pen = GDI.Pen(Color, LineWidth))
                gfx.DrawLines(pen, points);
        }
    }
}
