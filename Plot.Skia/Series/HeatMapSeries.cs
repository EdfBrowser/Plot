using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;

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

        //private int MinimumIndex => 0;
        //private int MaximumIndex => int.MaxValue;
        //private int MinRenderringIndex => Math.Max(0, MinimumIndex);
        //private int MaxRenderringIndex => Math.Min(Width - 1, MaximumIndex);

        public HeatMapSeries(IXAxis x, IYAxis y, double[,] intensity)
            : base(x, y)
        {
            m_intensity = intensity;

            ColorMap = new JetCMP();
        }

        public IColorMap ColorMap { get; set; }

        public override RangeMutable GetXLimit()
            => new RangeMutable(0, Width - 1);

        public override RangeMutable GetYLimit()
            => new RangeMutable(0, Height - 1);

        public int GetXIndex(double x)
        {
            // 第0个单位需要0个点
            // 第1个单位需要1 * sampleRate个点....
            int i = (int)(x);

            {
                i = Math.Max(i, 0);
                i = Math.Min(i, Width - 1);
            }

            return i;
        }

        public int GetYIndex(double x)
        {
            // 第0个单位需要0个点
            // 第1个单位需要1 * sampleRate个点....
            int i = (int)(x);

            {
                i = Math.Max(i, 0);
                i = Math.Min(i, Height - 1);
            }

            return i;
        }

        // 1. 遍历数组绘制矩形（设置paint的color）
        // 2. 
        public override void Render(RenderContext rc)
        {
            // 从intensity中获取最值
            double min = m_intensity.Cast<double>().Min();
            double max = m_intensity.Cast<double>().Max();


            RangeMutable xLimit = X.RangeMutable;
            RangeMutable yLimit = Y.RangeMutable;

            int i1 = GetXIndex(xLimit.Low);
            int i2 = GetXIndex(xLimit.High + xLimit.Span / Width);

            int i3 = GetYIndex(yLimit.Low);
            int i4 = GetYIndex(yLimit.High + yLimit.Span / Height);

            if (i1 == i2) return;
            if (i3 == i4) return;

            //IList<uint> argbs = new List<uint>();
            int width = i2 - i1;
            int height = i4 - i3;
            uint[] argbArray = new uint[(width + 1) * (height + 1)];

            // 映射数值为color
            for (int y = i3; y <= i4; y++)
            {
                for (int x = i1; x <= i2; x++)
                {
                    double value = m_intensity[y, x];
                    // 进行normalize，转换成(0-1)
                    double normalized = (value - min) / (max - min);
                    // 没啥作用其实
                    normalized = normalized.Clamp(0, 1);

                    Color color = ColorMap.GetColor(normalized);

                    argbArray[y * width + x] = color.PremultipliedARGB;
                }
            }


            Rect dataRect = GetDataRect(rc, X, Y);

            xLimit = GetXLimit();
            yLimit = GetYLimit();

            float left = X.GetPixel(xLimit.Low, dataRect);
            float right = X.GetPixel(xLimit.High, dataRect);

            // TODO: 到底是top还是bottom
            float bottom = Y.GetPixel(yLimit.Low, dataRect);
            float top = Y.GetPixel(yLimit.High, dataRect);

            Rect bmpRect = new Rect(left, right, top, bottom);

            // 获取托管对象的句柄，并且钉住
            GCHandle handle = GCHandle.Alloc(argbArray, GCHandleType.Pinned);

            SKImageInfo imageInfo = new SKImageInfo(width, height);
            using (SKBitmap bmp = new SKBitmap(imageInfo))
            {
                //bmp.SetPixels();
                bmp.InstallPixels(imageInfo, handle.AddrOfPinnedObject(),
                    imageInfo.RowBytes, (ptr, ctx) => handle.Free());

                using (SKPaint paint = new SKPaint())
                    rc.Canvas.DrawBitmap(bmp, bmpRect.ToSKRect(), paint);
            }
        }

        public override void Dispose()
        {
        }
    }
}
