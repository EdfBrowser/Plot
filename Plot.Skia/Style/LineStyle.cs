using SkiaSharp;
using System;

namespace Plot.Skia
{
    internal class LineStyle : IDisposable
    {
        private readonly SKPaint m_sKPaint;
        public LineStyle()
        {
            m_sKPaint = new SKPaint();
            Width = 1f;
            Color = Color.Black;
            AntiAlias = false;
        }


        internal float Width { get; set; }
        internal Color Color { get; set; }
        internal bool AntiAlias { get; set; }

        public void Dispose()
        {
            m_sKPaint?.Dispose();
        }

        private void Apply()
        {
            m_sKPaint.Style = SKPaintStyle.Stroke;
            m_sKPaint.StrokeWidth = Width;
            m_sKPaint.Color = Color.ToSkColor();
            m_sKPaint.IsAntialias = AntiAlias;
        }

        internal void Render(SKCanvas canvas, PointF p1, PointF p2)
        {
            Apply();
            canvas.DrawLine(p1.ToSKPoint(), p2.ToSKPoint(), m_sKPaint);
        }
    }
}
