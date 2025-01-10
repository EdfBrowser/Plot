using SkiaSharp;
using System;

namespace Plot.Skia
{
    public class BackgroundStyle : IDisposable
    {
        private readonly SKPaint m_sKPaint;
        internal BackgroundStyle()
        {
            m_sKPaint = new SKPaint();
            Color = Color.White;
            AntiAlias = false;
        }

        public Color Color { get; set; }
        public bool AntiAlias { get; set; }

        private void Apply()
        {
            m_sKPaint.Style = SKPaintStyle.Fill;
            m_sKPaint.Color = Color.ToSkColor();
            m_sKPaint.IsAntialias = AntiAlias;
        }

        public void Dispose()
        {
            m_sKPaint?.Dispose();
        }

        internal void Render(SKCanvas canvas, PixelPanel rect)
        {
            Apply();
            canvas.DrawRect(rect.ToSKRect(), m_sKPaint);
        }
    }
}
