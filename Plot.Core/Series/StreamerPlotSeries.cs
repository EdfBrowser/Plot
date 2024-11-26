using Plot.Core.Draws;
using Plot.Core.Renderables.Axes;
using Plot.Core.Series.AxesMangers;
using System;
using System.Drawing;
using System.Linq;

namespace Plot.Core.Series
{
    public class StreamerPlotSeries : IPlotSeries
    {
        public StreamerPlotSeries(Axis xAxis, Axis yAxis, Figure figure, int sampleRate)
        {
            XAxis = xAxis;
            YAxis = yAxis;
            Figure = figure;
            SampleRate = sampleRate;

            Data = new double[SampleRate];
        }

        public Axis XAxis { get; }
        public Axis YAxis { get; }
        public Figure Figure { get; }

        public Color Color { get; set; } = Color.Red;
        public float LineWidth { get; set; } = 1f;
        public string Label { get; set; } = null;

        public IAxisLimitsManager AxisLimitsManager { get; private set; } = new Sweep();
        public bool ManageAxisLimits { get; set; } = true;

        public int SampleRate { get; }
        public float SampleInterval => 1.0f / SampleRate;

        public int NextIndex { get; private set; } = 0;
        public double DataMin { get; private set; } = double.MaxValue;
        public double DataMax { get; private set; } = double.MinValue;

        public double[] Data { get; }

        public long Count { get; private set; }

        public float Scale { get; set; } = 1f;
        public PlotDimensions Dims => Figure.GetDimensions(XAxis, YAxis, Scale);

        public void Add(double value)
        {
            Count++;

            Data[NextIndex] = value;
            NextIndex = (NextIndex + 1) % SampleRate;
        }


        public void ValidateData() { }

        public AxisLimits GetAxisLimits()
        {
            DataMin = Data.Min();
            DataMax = Data.Max();

            double xMin = Count > SampleRate ? (Count - SampleRate) * SampleInterval : 0;
            double xMax = Count < SampleRate ? Data.Length * SampleInterval : Count * SampleInterval;

            return new AxisLimits(xMin, xMax, DataMin, DataMax);
        }

        public void Plot(Bitmap bmp, bool lowQuality, float scale)
        {
            Scale = scale;

            if (Data.Length == 0) return;

            if (ManageAxisLimits)
            {
                AxisLimits viewLimit = Figure.GetAxisLimits(XAxis, YAxis);
                AxisLimits dataLimit = GetAxisLimits();

                AxisLimits limits = AxisLimitsManager.GetAxisLimits(viewLimit, dataLimit);

                Figure.SetAxisLimits(limits, XAxis, YAxis);
            }

            // Swipe Right
            PointF[] points = new PointF[Data.Length];
            double xMin = GetAxisLimits().m_xMin;

            for (int i = 0; i < Data.Length; i++)
            {
                int index = (NextIndex + i) % Data.Length; // 循环索引
                float dx = (float)(index * SampleInterval + xMin);
                float x = Dims.GetPixelX(dx);
                float y = Dims.GetPixelY((float)Data[index]);

                points[index] = new PointF(x, y);
            }
            using (var gfx = GDI.Graphics(bmp, Dims, lowQuality, true))
            using (var pen = GDI.Pen(Color, LineWidth))
            {
                if (points.Length > 1)
                    gfx.DrawLines(pen, points);
            }
        }
    }
}
