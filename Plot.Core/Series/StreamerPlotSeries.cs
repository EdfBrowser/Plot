using Plot.Core.Draws;
using Plot.Core.Renderables.Axes;
using System.Collections.Generic;
using System.Drawing;

namespace Plot.Core.Series
{
    public class StreamerPlotSeries : IPlotSeries
    {
        private int m_sampleRate = 1;
        public int m_maxDataPoints = 1;

        public StreamerPlotSeries(Axis xAxis, Axis yAxis)
        {
            XAxis = xAxis;
            YAxis = yAxis;
        }

        public Axis XAxis { get; }
        public Axis YAxis { get; }

        public Color Color { get; set; } = Color.Red;
        public float LineWidth { get; set; } = 1f;
        public string Label { get; set; } = null;

        public int SampleRate
        {
            get => m_sampleRate;
            set
            {
                m_sampleRate = value;
                m_maxDataPoints = value * (int)XAxis.Dims.Span;
            }
        }
        public double SampleInterval => 1.0 / SampleRate;
        // TODO: 移除不在视界内的数据
        public List<double> Data { get; private set; } = new List<double>();

        public void AddSample(double sample)
        {
            //if (Data.Count >= m_maxDataPoints)
            //    Data.RemoveAt(0);

            Data.Add(sample);
        }

        public void ValidateData() { }

        public void Plot(Bitmap bmp, bool lowQuality, float scale)
        {
            if (Data == null || Data.Count == 0) return;

            //CleanOldData(XAxis.Dims.Min, XAxis.Dims.Max);

            List<PointF> points = new List<PointF>();
            for (int i = 0; i < Data.Count; i++)
            {
                double dx = i * SampleInterval;

                float x = XAxis.Dims.GetPixel(dx);
                float y = YAxis.Dims.GetPixel(Data[i]);

                if (x > XAxis.Dims.PlotOffsetPx && x < (XAxis.Dims.PlotOffsetPx + XAxis.Dims.DataSizePx))
                    if (y > YAxis.Dims.PlotOffsetPx && y < (YAxis.Dims.PlotOffsetPx + YAxis.Dims.DataSizePx))
                        points.Add(new PointF(x, y));
            }

            using (var gfx = GDI.Graphics(bmp, lowQuality, scale))
            using (var pen = GDI.Pen(Color, LineWidth))
                if (points.Count > 1)
                    gfx.DrawLines(pen, points.ToArray());
        }

        //public void CleanOldData(double xmin, double xmax)
        //{
        //    // 清除不在视界内的数据
        //    //Data.RemoveAll(sample => (Data.IndexOf(sample) * SampleInterval + OffsetX) < xmin);
        //    //||
        //    //(Data.IndexOf(sample) * SampleInterval + OffsetX) > xmax);
        //}
    }
}
