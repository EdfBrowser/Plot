using SkiaSharp;

namespace Plot.Skia
{
    public interface IMarkerShape
    {
        void Render(SKCanvas canvas, SKPaint paint, PointF p, float Size);
    }
}
