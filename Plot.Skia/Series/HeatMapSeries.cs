using System;
using System.Diagnostics;
using System.Linq;

namespace Plot.Skia
{
    // 1. 二维数组
    // 2. 数组值转换成颜色
    // 3. 填充
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

        private Rect GetDestRect(RenderContext rc)
        {
            Rect dataRect = GetDataRect(rc, X, Y);

            float left = X.GetPixel(0, dataRect);
            float right = X.GetPixel(Width - 1, dataRect);

            float top = Y.GetPixel(Height - 1, dataRect);
            float bottom = Y.GetPixel(0, dataRect);

            return new Rect(left, right, top, bottom);
        }

        public override void Render(RenderContext rc)
        {
            // 从intensity中获取最值
            double min = m_intensity.Cast<double>().Min();
            double max = m_intensity.Cast<double>().Max();

            Rect dataRect = rc.DataRect;

            int validPointCount = 0;

            int validMinX = int.MaxValue, validMaxX = int.MinValue;
            int validMinY = int.MaxValue, validMaxY = int.MinValue;

            int invalidMinY = int.MaxValue, invalidMaxY = int.MinValue;

            // 遍历整个数据，检查哪些点在 dataRect 区域内
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float px = X.GetPixel(x, dataRect);
                    float py = Y.GetPixel(y, dataRect);

                    if (rc.DataRect.Contains(px, py))
                    {
                        validPointCount++;

                        validMinX = Math.Min(validMinX, x);
                        validMaxX = Math.Max(validMaxX, x);
                        validMinY = Math.Min(validMinY, y);
                        validMaxY = Math.Max(validMaxY, y);
                    }
                    //else
                    //{
                    //    invalidMinY = Math.Min(invalidMinY, y);
                    //    invalidMaxY = Math.Max(invalidMaxY, y);
                    //}
                }
            }

            if (validPointCount == 0)
                return;

            Debug.WriteLine($"validPointCount:{validPointCount}");
            // 更新有效区域的宽度和高度
            int validWidth = validMaxX - validMinX + 1;
            int validHeight = validMaxY - validMinY + 1;

            uint[] argbs = new uint[validWidth * validHeight];

            // 映射数值为color
            for (int y = validMinY; y <= validMaxY; y++)
            {
                for (int x = validMinX; x <= validMaxX; x++)
                {
                    double value = 0;
                    if (validMaxY < Height - 1)
                        value = m_intensity[Height - 1 - (validMaxY - y), x];
                    else if (validMinY > 0)
                        value = m_intensity[y - validMinY, x];
                    else
                        value = m_intensity[y, x];
                    // 进行normalize，转换成(0-1)
                    double normalized = (value - min) / (max - min);
                    // 没啥作用其实
                    normalized = normalized.Clamp(0, 1);

                    Color color = ColorMap.GetColor(normalized);

                    int index = (y - validMinY) * validWidth + (x - validMinX);
                    argbs[index] = color.PremultipliedARGB;
                }
            }

            Size<int> size = new Size<int>(validWidth, validHeight);
            Debug.WriteLine($"w/h:{validWidth}/{validHeight}");
            float left = X.GetPixel(validMinX, dataRect);
            float right = X.GetPixel(validMaxX, dataRect);

            float top = Y.GetPixel(validMaxY, dataRect);
            float bottom = Y.GetPixel(validMinY, dataRect);

            Rect destRect = new Rect(left, right, top, bottom);
            HeatmapStyle.Render(rc.Canvas, argbs, size, destRect);
        }

        public override void Dispose()
        {
        }
    }
}
