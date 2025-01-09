using SkiaSharp;

namespace Plot.Skia
{
    internal class BackgroundStyle
    {
        internal BackgroundStyle()
        {
            Color = Color.White;
            AntiAlias = false;
        }

        internal Color Color { get; set; }
        internal bool AntiAlias { get; set; }

        internal void Render(SKCanvas canvas, PixelPanel rect)
        {
            using (SKPaint paint = new SKPaint())
            {
                paint.Style = SKPaintStyle.Fill;
                paint.Color = Color.ToSkColor();
                paint.IsAntialias = AntiAlias;

                canvas.DrawRect(rect.ToSKRect(), paint);
            }
        }
    }
}
