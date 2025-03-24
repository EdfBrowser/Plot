using SkiaSharp;
using System;
using System.Runtime.InteropServices;

namespace Plot.Skia
{
    public class HeatmapStyle : IDisposable
    {
        private readonly SKPaint m_sKPaint;
        public HeatmapStyle()
        {
            m_sKPaint = new SKPaint();
            Smooth = false;
        }

        public bool Smooth { get; set; }

        public void Dispose()
        {
            m_sKPaint?.Dispose();
        }

        private void Apply()
        {
        }

        internal void Render(SKCanvas canvas, uint[] argb, Size<int> size, Rect destRect)
        {
            Apply();

            // 获取托管对象的句柄，并且钉住
            GCHandle handle = GCHandle.Alloc(argb, GCHandleType.Pinned);

            SKImageInfo info = new SKImageInfo(
                size.Width, size.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
            using (SKImage image = SKImage.FromPixels(info, handle.AddrOfPinnedObject()))
            {
                SKFilterMode mode = Smooth ? SKFilterMode.Linear : SKFilterMode.Nearest;
                SKSamplingOptions options = new SKSamplingOptions(mode);
                canvas.DrawImage(image, destRect.ToSKRect(), options, m_sKPaint);
            }

            if (handle.IsAllocated)
                handle.Free();
        }
    }
}
