using SkiaSharp;

namespace Plot.Skia
{
    internal class TickStyle
    {
        public TickStyle()
        {
            Length = 4f;
            Width = 1f;
            Color = Color.Black;
            AntiAlias = false;
        }

        internal float Length { get; set; }
        internal float Width { get; set; }
        internal Color Color { get; set; }
        internal bool AntiAlias { get; set; }

        internal void Render(SKCanvas canvas, PointF p1, PointF p2)
        {
            using (SKPaint paint = new SKPaint())
            {
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeWidth = Width;
                paint.Color = Color.ToSkColor();
                paint.IsAntialias = AntiAlias;

                canvas.DrawLine(p1.ToSKPoint(), p2.ToSKPoint(), paint);
            }
        }
    }
}
