using SkiaSharp;
using System;
using System.Collections.Generic;

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

        internal void Render(SKCanvas canvas, IEnumerable<PointF> points)
        {
            if (!Renderable) return;

            Apply();
            bool move = true;
            using (SKPath path = new SKPath())
            {
                foreach (PointF p in points)
                {
                    if (float.IsNaN(p.X) || float.IsNaN(p.Y))
                    {
                        move = true;
                        continue;
                    }

                    if (move)
                    {
                        path.MoveTo(p.ToSKPoint());
                        move = false;
                    }
                    else
                        path.LineTo(p.ToSKPoint());
                }

                canvas.DrawPath(path, m_sKPaint);
            }
        }
    }
}
