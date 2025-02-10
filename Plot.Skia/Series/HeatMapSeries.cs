using System;

namespace Plot.Skia
{
    public class HeatMapSeries : BaseSeries
    {
        private readonly double[,] m_intensity;

        private int Height => m_intensity.GetLength(0);
        private int Width => m_intensity.GetLength(1);

        public HeatMapSeries(IXAxis x, IYAxis y, double[,] intensity)
            : base(x, y)
        {
            m_intensity = intensity;

            ColorMap = new ViridisCMP();
            HeatmapStyle = new HeatmapStyle();
        }

        public IColorMap ColorMap { get; set; }
        public HeatmapStyle HeatmapStyle { get; set; }

        public override RangeMutable GetXLimit()
            => new RangeMutable(0, Width - 1);

        public override RangeMutable GetYLimit()
            => new RangeMutable(0, Height - 1);

        public override void Render(RenderContext rc)
        {
            (int validMinX, int validMaxX, int validMinY, int validMaxY)
                = GetValidRegion(rc);

            // 超过数据区域的部分不渲染
            if (validMinX > validMaxX || validMinY > validMaxY) return;

            (double min, double max) = CalculateIntensityRange();

            uint[] argbs = GenerateColorMap(
                validMinX, validMaxX, validMinY, validMaxY, min, max);

            RenderToCanvas(rc, validMinX, validMaxX, validMinY, validMaxY, argbs);
        }

        private (int minX, int maxX, int minY, int maxY) GetValidRegion(RenderContext rc)
        {
            Rect dataRect = rc.DataRect;

            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;

            // 确定有效区域
            for (int j = 0; j < Height; j++)
            {
                float y = Y.GetPixel(j, dataRect);
                if (!dataRect.ContainsY(y)) continue;

                for (int i = 0; i < Width; i++)
                {
                    float x = X.GetPixel(i, dataRect);
                    if (dataRect.ContainsX(x))
                    {
                        minX = Math.Min(minX, i);
                        maxX = Math.Max(maxX, i);
                        minY = Math.Min(minY, j);
                        maxY = Math.Max(maxY, j);
                    }
                }
            }

            return (minX, maxX, minY, maxY);
        }

        private (double min, double max) CalculateIntensityRange()
        {
            double min = double.MaxValue;
            double max = double.MinValue;

            foreach (double value in m_intensity)
            {
                if (value < min) min = value;
                if (value > max) max = value;
            }
            return (min, max);
        }

        private uint[] GenerateColorMap(int minX, int maxX, int minY, int maxY,
            double min, double max)
        {
            double range = max - min;
            if (range == 0)
                throw new ArgumentException("The value of the max-min is 0");
            //range = 1; // 防止除以零

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            uint[] argbs = new uint[height * width];

            for (int y = minY; y <= maxY; y++)
            {
                // 从数组末尾开始填充
                int offset = (maxY - y) * width;
                for (int x = minX; x <= maxX; x++)
                {
                    // 从height-1 增加到height - 1 - y
                    double value = m_intensity[Height - 1 - y, x];

                    // 进行normalize，转换成(0-1)
                    double normalized = (value - min) / range;
                    //normalized = normalized.Clamp(0, 1);

                    int index = offset + (x - minX);
                    argbs[index] = ColorMap.GetColor(normalized).PremultipliedARGB;
                }
            }

            return argbs;
        }

        private void RenderToCanvas(RenderContext rc, int minX, int maxX, int minY,
            int maxY, uint[] argbs)
        {
            Rect dataRect = rc.DataRect;
            float left = X.GetPixel(minX, dataRect);
            float right = X.GetPixel(maxX, dataRect);
            float top = Y.GetPixel(maxY, dataRect);
            float bottom = Y.GetPixel(minY, dataRect);

            HeatmapStyle.Render(
                rc.Canvas,
                argbs,
                new Size<int>(maxX - minX + 1, maxY - minY + 1),
                new Rect(left, right, top, bottom)
            );
        }

        public override void Dispose()
        {
            HeatmapStyle.Dispose();
        }
    }
}
