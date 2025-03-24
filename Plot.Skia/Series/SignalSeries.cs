using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    public class SignalSeries : BaseSeries
    {
        public SignalSeries(IXAxis x, IYAxis y, ISignalSource signalSource)
            : base(x, y)
        {
            SignalSource = signalSource;
            SeriesLineStyle = new LineStyle() { AntiAlias = true };
            MarkerStyle = new MarkerStyle() { Shape = MarkerShape.OpenCircle, AntiAlias = true };
            LowDensityMode = false;
        }

        public ISignalSource SignalSource { get; }
        public LineStyle SeriesLineStyle { get; set; }
        public MarkerStyle MarkerStyle { get; set; }
        public bool LowDensityMode { get; set; }

        public override void Render(RenderContext rc)
        {
            if (!SignalSource.GetYs().Any()) return;

            if (PointsPerPixel(rc) < 1 || LowDensityMode)
                RenderLowDensity(rc);
            else
                RenderHighDensity(rc);
        }

        public override RangeMutable GetXLimit() => SignalSource.GetXLimit();

        public override RangeMutable GetYLimit() => SignalSource.GetYLimit();

        private void RenderLowDensity(RenderContext rc)
        {
            RangeMutable range = X.RangeMutable;
            int i1 = SignalSource.GetIndex(range.Low);
            int i2 = SignalSource.GetIndex(range.High + SignalSource.SampleInterval);

            if (i1 == i2) return;

            Rect dataRect = GetDataRect(rc, X, Y);

            List<PointF> points = new List<PointF>();

            for (int i = i1; i <= i2; i++)
            {
                float x = X.GetPixel(SignalSource.GetX(i), dataRect);
                float y = Y.GetPixel(SignalSource.GetY(i), dataRect);

                // 超过画图区域设为NAN
                if (!rc.DataRect.Contains(x, y))
                {
                    x = float.NaN;
                    y = float.NaN;
                }

                PointF p = new PointF(x, y);
                points.Add(p);
            }

            if (!points.Any()) return;

            SeriesLineStyle.Render(rc.Canvas, points);

            //double pointsPerPx = PointsPerPixel(rc);
            //if (pointsPerPx < 1)
            //{
            //    double radius = Math.Sqrt(.2 / pointsPerPx);
            //    MarkerStyle.Size = (float)(radius * 4f);
            //    MarkerStyle.Render(rc.Canvas, points);
            //}
        }

        private void RenderHighDensity(RenderContext rc)
        {
            Rect dataRect = GetDataRect(rc, X, Y);
            double unitPerPx = X.Width / dataRect.Width;

            IList<PointF> points = new List<PointF>();

            for (int i = 0; i < dataRect.Width; i++)
            {
                float px = dataRect.Left + i;
                double min = X.GetWorld(px, dataRect);
                double max = min + Math.Abs(unitPerPx);

                // 获取该单位下的点数索引
                int i1 = SignalSource.GetIndex(min);
                int i2 = SignalSource.GetIndex(max);

                // 跳过超出索引范围的
                if (i2 == i1) continue;

                float y1 = Y.GetPixel(SignalSource.GetY(i1), dataRect);
                float y4 = Y.GetPixel(SignalSource.GetY(i2), dataRect);

                float y = Math.Max(y1, y4);

                // 若大于1个点，需要计算下面的
                if ((i2 - i1) > 1)
                {
                    // i1-i2之间y轴最大值和最小值
                    RangeMutable yLimit = SignalSource.GetYLimit(i1, i2);
                    float y2 = Y.GetPixel(yLimit.Low, dataRect);
                    float y3 = Y.GetPixel(yLimit.High, dataRect);
                    y = Math.Max(y, Math.Max(y2, y3));
                }

                // 超过画图区域设为NAN
                if (!rc.DataRect.Contains(px, y))
                {
                    px = float.NaN;
                    y = float.NaN;
                }

                points.Add(new PointF(px, y));
            }

            if (!points.Any()) return;

            SeriesLineStyle.Render(rc.Canvas, points);
        }

        private double PointsPerPixel(RenderContext rc)
        {
            Rect dataRect = GetDataRect(rc, X, Y);
            // 1个单位需要sampleRate个点，width个单位就需要width * sampleRate
            return (X.Width / SignalSource.SampleInterval) / dataRect.Width;
        }

        public override void Dispose()
        {
            SeriesLineStyle.Dispose();
            MarkerStyle.Dispose();
        }
    }
}
