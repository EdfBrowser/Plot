using SkiaSharp;
using System;

namespace Plot.Skia
{
    public class LineStyle : IDisposable
    {
        private readonly SKPaint m_sKPaint;
        public LineStyle()
        {
            m_sKPaint = new SKPaint();
            Width = 1f;
            Color = Color.Black;
            AntiAlias = false;
        }


        public float Width { get; set; }
        public Color Color { get; set; }
        public bool AntiAlias { get; set; }

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

        internal void Render(SKCanvas canvas, PointF[] points)
        {
            Apply();
            using (SKPath path = new SKPath())
            {
                path.MoveTo(points[0].ToSKPoint());
                for (int i = 1; i < points.Length; i++)
                    path.LineTo(points[i].ToSKPoint());

                canvas.DrawPath(path, m_sKPaint);
            }
        }
    }
}
