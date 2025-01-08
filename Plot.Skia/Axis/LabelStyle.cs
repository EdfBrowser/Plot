using SkiaSharp;
using System;
using System.Linq;

namespace Plot.Skia
{
    internal class LabelStyle
    {
        internal Color Color { get; set; }
        internal bool AntiAlias { get; set; }
        internal float FontSize { get; set; } = 12f;

        internal string Text { get; set; } = string.Empty;

        internal float Measure(string text)
        {
            string[] lines = string.IsNullOrEmpty(text)
                ? Array.Empty<string>() : text.Split('\n');

            using (SKTypeface typeface = SKTypeface.FromFamilyName("Consolas"))
            using (SKFont font = new SKFont(typeface, FontSize))
            using (SKPaint brush = new SKPaint())
            {
                brush.IsAntialias = AntiAlias;
                brush.Color = Color.ToSkColor();

                float lineHeight = font.GetFontMetrics(out SKFontMetrics metrics);
                float[] lineWidths = lines
                    .Select(x => font.MeasureText(x, brush))
                    .ToArray();

                return lineWidths.Length == 0 ? 0 : lineWidths.Max();
            }
        }



        internal (string largestText, float actualMaxLength)
            MeasureHighestString(string[] tickLabels)
        {
            float maxHeight = 0;
            string maxText = string.Empty;

            for (int i = 0; i < tickLabels.Length; i++)
            {
                float size = Measure(tickLabels[i]);
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
                float size = Measure(tickLabels[i]);
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
