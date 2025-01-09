using SkiaSharp;
using System;
using System.Linq;

namespace Plot.Skia
{
    internal class LabelStyle
    {
        public LabelStyle()
        {
            Color = Color.Black;
            AntiAlias = false;
            FontSize = 12f;
            Text = string.Empty;
            FontFamily = "Consolas";
        }

        internal Color Color { get; set; }
        internal bool AntiAlias { get; set; }
        internal float FontSize { get; set; }
        internal string Text { get; set; }
        internal string FontFamily { get; set; }

        internal void Render(SKCanvas canvas, PointF p)
        {
            using (SKFont font = new SKFont())
            using (SKPaint paint = new SKPaint())
            {
                font.Size = FontSize;
                font.Typeface = SKTypeface.FromFamilyName(FontFamily);

                paint.Style = SKPaintStyle.Fill;
                paint.Color = Color.ToSkColor();
                paint.IsAntialias = AntiAlias;

                canvas.DrawText(Text, p.ToSKPoint(), SKTextAlign.Center, font, paint);
            }
        }

        internal (float width, float height) Measure(string text)
        {
            string[] lines = string.IsNullOrEmpty(text)
                ? Array.Empty<string>() : text.Split('\n');

            using (SKFont font = new SKFont())
            using (SKPaint paint = new SKPaint())
            {
                font.Size = FontSize;
                font.Typeface = SKTypeface.FromFamilyName(FontFamily);

                paint.Style = SKPaintStyle.Fill;
                paint.Color = Color.ToSkColor();
                paint.IsAntialias = AntiAlias;

                float lineHeight = font.GetFontMetrics(out SKFontMetrics metrics);
                float[] lineWidths = lines
                    .Select(x => font.MeasureText(x, paint))
                    .ToArray();

                return (lineWidths.Length == 0 ? 0 : lineWidths.Max(), lineHeight);
            }
        }



        internal (string largestText, float actualMaxLength)
            MeasureHighestString(string[] tickLabels)
        {
            float maxHeight = 0;
            string maxText = string.Empty;

            for (int i = 0; i < tickLabels.Length; i++)
            {
                float size = Measure(tickLabels[i]).height;
                if (size > maxHeight)
                {
                    maxHeight = size;
                    maxText = tickLabels[i];
                }
            }

            return (maxText, maxHeight);
        }

        internal (string largestText, float actualMaxLength)
            MeasureWidestString(string[] tickLabels)
        {
            float maxWidth = 0;
            string maxText = string.Empty;

            for (int i = 0; i < tickLabels.Length; i++)
            {
                float size = Measure(tickLabels[i]).width;
                if (size > maxWidth)
                {
                    maxWidth = size;
                    maxText = tickLabels[i];
                }
            }

            return (maxText, maxWidth);
        }
    }
}
