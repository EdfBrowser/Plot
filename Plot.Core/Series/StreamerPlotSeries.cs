using Plot.Core.Draws;
using Plot.Core.Renderables.Axes;
using Plot.Core.Series.AxisLimitsManger;
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

            Data = new double[SampleRate];
        }

        public Axis XAxis { get; }
        public Axis YAxis { get; }

        public Color Color { get; set; } = Color.Red;
        public float LineWidth { get; set; } = 1f;
        public string Label { get; set; } = null;

        public IAxisLimitsManager AxisLimitsManager { get; private set; } = new Sweep();
        public bool ManageAxisLimits { get; set; } = true;

        public int SampleRate { get; }
        public float SampleInterval => 1.0f / SampleRate;

        public int NextIndex { get; private set; } = 0;
        public double DataMinX { get; private set; } = double.MaxValue;
        public double DataMaxX { get; private set; } = double.MinValue;
        public double DataMinY { get; private set; } = double.MaxValue;
        public double DataMaxY { get; private set; } = double.MinValue;

        public double[] Data { get; }

        public long Count { get; private set; }

        public void Add(double value)
        {
            Count++;

            Data[NextIndex] = value;
            NextIndex = (NextIndex + 1) % SampleRate;
        }


        public void ValidateData() { }

        public AxisLimits GetAxisLimits()
        {
            DataMinY = Data.Min();
            DataMaxY = Data.Max();

            DataMinX = Count > SampleRate ? (Count - SampleRate) * SampleInterval : 0;
            DataMaxX = Count > SampleRate ? Count * SampleInterval : Data.Length * SampleInterval;

            return new AxisLimits(DataMinX, DataMaxX, DataMinY, DataMaxY);
        }

        public void Plot(Bitmap bmp, bool lowQuality, float scale)
        {

            if (Data.Length == 0) return;

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

            float offsetMinX = (float)DataMinX;
            for (int i = 0; i < Data.Length; i++)
            {
                int index = (NextIndex + i) % Data.Length; // 循环索引
                float dx = index * SampleInterval + offsetMinX;
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
