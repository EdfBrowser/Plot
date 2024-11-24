using Plot.Core.Draws;
using Plot.Core.Renderables.Axes;
using Plot.Core.Series.AxesMangers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Plot.Core.Series
{
    public class SampleDataSeries : IPlotSeries
    {
        public SampleDataSeries(Axis xAxis, Axis yAxis, Figure figure, int sampleRate)
        {
            XAxis = xAxis;
            YAxis = yAxis;
            Figure = figure;
            SampleRate = sampleRate;

            Data = new double[SampleRate];
            for (int i = 0; i < Data.Length; i++)
            {
                Add(0);
            }
        }

        public Axis XAxis { get; }
        public Axis YAxis { get; }
        public Figure Figure { get; }

        public Color Color { get; set; } = Color.Red;
        public float LineWidth { get; set; } = 1f;
        public string Label { get; set; } = null;

        public IAxisManager AxisManager { get; private set; } = new Full();
        public bool ManageAxisLimits { get; set; } = true;

        public int SampleRate { get; }
        public float SampleInterval => 1.0f / SampleRate;

        public int NextIndex { get; private set; } = 0;
        public double DataMin { get; private set; } = double.MaxValue;
        public double DataMax { get; private set; } = double.MinValue;

        public double[] Data { get; }

        public long Count { get; private set; }

        public void Add(double value)
        {
            Count++;
            Data[NextIndex++] = value;
            if (NextIndex >= SampleRate) NextIndex = 0;

            DataMin = Math.Min(DataMin, value);
            DataMax = Math.Max(DataMax, value);
        }


        public void ValidateData() { }

        public AxisLimits GetAxisLimits()
        {
            double xMin = 0;
            double xMax = Data.Length * SampleInterval;

            return new AxisLimits(xMin, xMax, DataMin, DataMax);
        }

        public void Plot(Bitmap bmp, PlotDimensions dims, bool lowQuailty)
        {
            if (Data.Length == 0) return;

            if (ManageAxisLimits)
            {
                AxisLimits viewLimit = Figure.GetAxisLimits(XAxis, YAxis);
                AxisLimits dataLimit = GetAxisLimits();

                AxisLimits limits = AxisManager.GetAxisLimits(viewLimit, dataLimit);

                Figure.SetAxisLimits(limits, XAxis, YAxis);
            }

            // Swipe Right

            int newestCount = NextIndex;
            int oldestCount = Data.Length - newestCount;

            PointF[] newestPoint = new PointF[newestCount];
            PointF[] oldestPoint = new PointF[oldestCount];

            for (int i = 0; i < newestPoint.Length; i++)
            {
                float dx = i * SampleInterval;
                float x = dims.GetPixelX(dx);
                float y = dims.GetPixelY((float)Data[i]);
                newestPoint[i] = new PointF(x, y);
            }

            for (int i = 0; i < oldestPoint.Length; i++)
            {
                float dx = (i + NextIndex) * SampleInterval;
                float x = dims.GetPixelX(dx);
                float y = dims.GetPixelY((float)Data[i + NextIndex]);
                oldestPoint[i] = new PointF(x, y);
            }

            using (var gfx = GDI.Graphics(bmp, dims, lowQuailty, true))
            using (var pen = GDI.Pen(Color, LineWidth))
            {
                if (newestPoint.Length > 1)
                    gfx.DrawLines(pen, newestPoint);
                if (oldestPoint.Length > 1)
                    gfx.DrawLines(pen, oldestPoint);

            }
        }
    }
}
