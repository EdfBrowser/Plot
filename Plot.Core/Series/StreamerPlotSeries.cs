using Plot.Core.Draws;
using Plot.Core.Renderables.Axes;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Plot.Core.Series
{
    public class StreamerPlotSeries : IPlotSeries
    {
        public StreamerPlotSeries(Axis xAxis, Axis yAxis, int sampleRate)
        {
            XAxis = xAxis;
            YAxis = yAxis;
            SampleRate = sampleRate;

            int length = (int)Math.Ceiling(xAxis.Dims.Span);
            Data = new double[length * sampleRate];
        }

        public Axis XAxis { get; }
        public Axis YAxis { get; }

        public Color Color { get; set; } = Color.Red;
        public float LineWidth { get; set; } = 1f;
        public string Label { get; set; } = null;

        public int SampleRate { get; }
        public double SampleInterval => 1.0 / SampleRate;

        public int NextIndex { get; private set; } = 0;

        public double[] Data { get; }

        public double OffsetX { get; set; } = 0;

        public void Add(double value)
        {
            Data[NextIndex] = value;
            NextIndex = (NextIndex + 1) % Data.Length;
        }

        public void AddRange(IEnumerable<double> values)
        {
            foreach (double value in values)
                Add(value);
        }

        public void ValidateData() { }

        public void Plot(Bitmap bmp, bool lowQuality, float scale)
        {
            if (Data.Length == 0) return;

            PlotDimensions Dims = XAxis.CreatePlotDimensions(YAxis, scale);

            // Swipe Right
            PointF[] points = new PointF[Data.Length];

            for (int i = 0; i < Data.Length; i++)
            {
                int index = (NextIndex + i) % Data.Length; // 循环索引
                double dx = index * SampleInterval;
                if (XAxis.AxisTick.TickGenerator.LabelFormat == Enum.TickLabelFormat.DateTime)
                    dx += dx * 1.0 / 24 / 3600 + OffsetX;
                else
                    dx += OffsetX;
                float x = Dims.GetPixelX(dx);
                float y = Dims.GetPixelY(Data[index]);

                points[index] = new PointF(x, y);
            }
            using (var gfx = GDI.Graphics(bmp, Dims, lowQuality, true))
            using (var pen = GDI.Pen(Color, LineWidth))
            {
                if (points.Length > 1)
                    gfx.DrawLines(pen, points);

                gfx.DrawLine(pen,
                    points[NextIndex].X,
                    Dims.m_plotOffsetY,
                    points[NextIndex].X,
                    Dims.m_plotOffsetY + Dims.m_plotHeight);
            }
        }

    }
}
