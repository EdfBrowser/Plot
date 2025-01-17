using SkiaSharp;
using System;

namespace Plot.Skia
{
    public class MarkerStyle : IDisposable
    {
        private readonly SKPaint m_sKPaint;

        internal MarkerStyle()
        {
            m_sKPaint = new SKPaint();

            Shape = MarkerShape.None;
            Size = 12f;
            MarkerColor = Color.Red;
            AntiAlias = false;
        }

        public MarkerShape Shape { get; set; }
        public float Size { get; set; }
        public Color MarkerColor { get; set; }
        public bool AntiAlias { get; set; }

        private void Apply()
        {
            m_sKPaint.Style = SKPaintStyle.Stroke;
            m_sKPaint.Color = MarkerColor.ToSkColor();
            m_sKPaint.IsAntialias = AntiAlias;
        }

        public void Dispose()
        {
            m_sKPaint?.Dispose();
        }

        internal void Render(SKCanvas canvas, PointF p)
        {
            Apply();
            Shape.GetMarker().Render(canvas, m_sKPaint, p, Size);
        }

        internal void Render(SKCanvas canvas, PointF[] points)
        {
            Apply();
            IMarkerShape shape = Shape.GetMarker();

            foreach (PointF p in points)
            {
                shape.Render(canvas, m_sKPaint, p, Size);
            }
        }
    }
}
