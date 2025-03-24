using SkiaSharp;

namespace Plot.Skia
{
    internal class OpenCircle : IMarkerShape
    {
        public void Render(SKCanvas canvas, SKPaint paint, PointF p, float Size)
        {
            float radius = Size / 2;
            canvas.DrawCircle(p.ToSKPoint(), radius, paint);
        }
    }
}
