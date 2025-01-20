using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    public class SignalSeries : ISeries
    {
        private readonly IXAxis m_x;
        private readonly IYAxis m_y;

        public SignalSeries(IXAxis x, IYAxis y)
        {
            m_x = x;
            m_y = y;
            SampleRate = 1.0;
            SeriesLineStyle = new LineStyle();
            MarkerStyle = new MarkerStyle() { Shape = MarkerShape.OpenCircle };
            LowDensityMode = false;
        }

        public IXAxis X => m_x;
        public IYAxis Y => m_y;
        public double[] Data { get; set; }
        public double SampleRate { get; set; }
        public double SampleInterval => 1.0 / SampleRate;

        public LineStyle SeriesLineStyle { get; set; }
        public MarkerStyle MarkerStyle { get; set; }

        public bool LowDensityMode { get; set; }

        public void Render(RenderContext rc)
        {
            if (Data == null) return;

            if (PointsPerPixel(rc) < 1 || LowDensityMode)
                RenderLowDensity(rc);
            else
                RenderHighDensity(rc);
        }

        public RangeMutable GetXLimit()
            => new RangeMutable(0, (Data.Length - 1) * SampleInterval);

        public RangeMutable GetYLimit()
            => GetYLimit(0, Data.Length - 1);

        // TODO: 优化速度
        private RangeMutable GetYLimit(int startIndex, int endIndex)
        {
            double min = double.PositiveInfinity, max = double.NegativeInfinity;
            for (int i = startIndex; i <= endIndex; i++)
            {
                min = Math.Min(min, Data[i]);
                max = Math.Max(max, Data[i]);
            }

            return new RangeMutable(min, max);
        }

        private void RenderLowDensity(RenderContext rc)
        {
            RangeMutable range = m_x.RangeMutable;
            int i1 = GetIndex(range.Low);
            int i2 = GetIndex(range.High + SampleInterval);

            if (i1 == i2) return;

            Rect dataRect = rc.GetDataRect(m_x);

            List<PointF> points = new List<PointF>();

            for (int i = i1; i <= i2; i++)
            {
                float x = m_x.GetPixel(GetX(i), dataRect);
                float y = m_y.GetPixel(GetY(i), dataRect);
                PointF p = new PointF(x, y);
                points.Add(p);
            }

            if (!points.Any()) return;

            SeriesLineStyle.Render(rc.Canvas, points.ToArray());

            double pointsPerPx = PointsPerPixel(rc);
            if (pointsPerPx < 1)
            {
                double radius = Math.Sqrt(.2 / pointsPerPx);
                MarkerStyle.Size = (float)(radius * 2f);
                MarkerStyle.Render(rc.Canvas, points.ToArray());
            }
        }

        private void RenderHighDensity(RenderContext rc)
        {
            Rect dataRect = rc.GetDataRect(m_x);
            double unitPerPx = m_x.Width / dataRect.Width;

            IList<PointF> points = new List<PointF>();

            for (int i = 0; i < dataRect.Width; i++)
            {
                float px = dataRect.Left + i;
                double min = m_x.GetWorld(px, dataRect);
                double max = min + Math.Abs(unitPerPx);

                int i1 = GetIndex(min);
                int i2 = GetIndex(max);

                // 跳过超出索引范围的
                if (i2 == i1) continue;

                // TODO: 判断i2-i1是否大于1
                float y1 = m_y.GetPixel(GetY(i1), dataRect);
                float y4 = m_y.GetPixel(GetY(i2), dataRect);

                RangeMutable yLimit = GetYLimit(i1, i2);
                float y2 = m_y.GetPixel(yLimit.Low, dataRect);
                float y3 = m_y.GetPixel(yLimit.High, dataRect);

                float y = Enumerable.Max(new float[] { y1, y4, y2, y3 });

                points.Add(new PointF(px, y));
            }

            if (!points.Any()) return;

            SeriesLineStyle.Render(rc.Canvas, points.ToArray());
        }

        private int GetIndex(double x)
        {
            int i = (int)(x * SampleRate);

            {
                i = Math.Max(i, 0);
                i = Math.Min(i, Data.Length - 1);
            }
            return i;
        }

        private double GetX(int index)
            => index * SampleInterval;

        private double GetY(int index)
        {
            return Data[index];
        }

        private double PointsPerPixel(RenderContext rc)
        {
            Rect dataRect = rc.GetDataRect(m_x);
            return (m_x.Width * SampleRate) / dataRect.Width;
        }

        public void Dispose()
        {
            SeriesLineStyle.Dispose();
            MarkerStyle.Dispose();
        }
    }
}
