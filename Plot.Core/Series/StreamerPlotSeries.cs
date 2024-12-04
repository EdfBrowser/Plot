using Plot.Core.Draws;
using Plot.Core.Renderables.Axes;
using Plot.Core.Series.AxisLimitsManger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Plot.Core.Series
{
    public class StreamerPlotSeries : IPlotSeries
    {
        public StreamerPlotSeries(Axis xAxis, Axis yAxis, int sampleRate)
        {
            XAxis = xAxis;
            YAxis = yAxis;
            SampleRate = sampleRate;

            Data = new double[SampleRate * 6];
        }

        public Axis XAxis { get; }
        public Axis YAxis { get; }

        public Color Color { get; set; } = Color.Red;
        public float LineWidth { get; set; } = 1f;
        public string Label { get; set; } = null;

        public IAxisLimitsManager AxisLimitsManager { get; private set; } = new Sweep();
        public bool ManageAxisLimits { get; set; } = false;

        public int SampleRate { get; }
        public double SampleInterval => 1.0 / SampleRate;

        public int NextIndex { get; private set; } = 0;
        public double DataMinX { get; private set; } = double.MaxValue;
        public double DataMaxX { get; private set; } = double.MinValue;
        public double DataMinY { get; private set; } = double.MaxValue;
        public double DataMaxY { get; private set; } = double.MinValue;
        public double OffsetX { get; set; } = 0;

        public double[] Data { get; }

        public long Count { get; private set; }

        public void Add(double value)
        {
            Count++;

            Data[NextIndex] = value;
            NextIndex = (NextIndex + 1) % Data.Length;
        }

        public void AddRange(IEnumerable<double> values)
        {
            foreach (double value in values)
                Add(value);
        }

        public void ValidateData() { }

        private DateTime m_minTime => DateTime.FromOADate(OffsetX);
        private DateTime m_maxTime => DateTime.FromOADate(OffsetX);
        public AxisLimits GetAxisLimits()
        {
            DataMinY = Data.Min();
            DataMaxY = Data.Max();

            var minTime = Count > Data.Length ? m_minTime.AddSeconds((Count - Data.Length) * SampleInterval) : m_minTime;
            var maxTime = Count > Data.Length ? m_maxTime.AddSeconds(Count * SampleInterval) : m_maxTime.AddSeconds(Data.Length * SampleInterval);
            DataMinX = minTime.ToOADate();
            DataMaxX = maxTime.ToOADate();

            return new AxisLimits(DataMinX, DataMaxX, DataMinY, DataMaxY);
        }

        public void Plot(Bitmap bmp, bool lowQuality, float scale)
        {
            if (Data.Length == 0) return;

            // TODO: 提取到Axis类中，这不属于plot的职责
            if (ManageAxisLimits)
            {
                AxisLimits viewLimit = XAxis.GetAxisLimits(YAxis);
                AxisLimits dataLimit = GetAxisLimits();

                AxisLimits limits = AxisLimitsManager.GetAxisLimits(viewLimit, dataLimit);

                XAxis.SetAxisLimits(YAxis, limits);
            }

            PlotDimensions Dims = XAxis.CreatePlotDimensions(YAxis, scale);

            // Swipe Right
            PointF[] points = new PointF[Data.Length];

            for (int i = 0; i < Data.Length; i++)
            {
                int index = (NextIndex + i) % Data.Length; // 循环索引
                var time = DateTime.FromOADate(DataMinX);
                double dx = time.AddSeconds(index * SampleInterval).ToOADate();
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
