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
        }

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

            using (SKBitmap bmp = new SKBitmap(size.Width, size.Height))
            {
                SKImageInfo imageInfo = bmp.Info;
                //bmp.SetPixels();
                bmp.InstallPixels(imageInfo, handle.AddrOfPinnedObject(),
                    imageInfo.RowBytes, (ptr, ctx) => handle.Free());

                canvas.DrawBitmap(bmp, destRect.ToSKRect(), m_sKPaint);
            }
        }
    }
}
