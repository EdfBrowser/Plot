using SkiaSharp;
using System;

namespace Plot.Skia
{
    // TODO: 废除，使用lineStyle
    public class TickStyle : IDisposable
    {
        private readonly SKPaint m_sKPaint;
        public TickStyle()
        {
            m_sKPaint = new SKPaint();
            Length = 4f;
            Width = 1f;
            Color = Color.Black;
            AntiAlias = false;
        }

        public float Length { get; set; }
        public float Width { get; set; }
        public Color Color { get; set; }
        public bool AntiAlias { get; set; }
        public bool Renderable { get; set; } = true;

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
            if (!Renderable) return;

            Apply();
            canvas.DrawLine(p1.ToSKPoint(), p2.ToSKPoint(), m_sKPaint);
        }
    }
}
