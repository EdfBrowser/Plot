using SkiaSharp;
using System;
using System.Drawing;
using System.Linq;

namespace Plot.Skia
{
    internal class LabelStyle : IDisposable
    {
        private readonly SKPaint m_sKPaint;
        private readonly SKFont m_sKFont;

        public LabelStyle()
        {
            m_sKPaint = new SKPaint();
            m_sKFont = new SKFont();

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

        private void Apply()
        {
            m_sKFont.Size = FontSize;
            m_sKFont.Typeface = SKTypeface.FromFamilyName(FontFamily);

            m_sKPaint.Style = SKPaintStyle.Fill;
            m_sKPaint.Color = Color.ToSkColor();
            m_sKPaint.IsAntialias = AntiAlias;
        }

        public void Dispose()
        {
            m_sKFont?.Dispose();
            m_sKPaint?.Dispose();
        }

        internal float Ascent()
        {
            Apply();
            return -m_sKFont.Metrics.Ascent;
        }

        internal void Render(SKCanvas canvas, PointF p, SKTextAlign textAlign)
        {
            Apply();

            canvas.DrawText(Text, p.ToSKPoint(), textAlign, m_sKFont, m_sKPaint);
        }

        internal (float width, float height) Measure(string text)
        {
            string[] lines = string.IsNullOrEmpty(text)
                ? Array.Empty<string>() : text.Split('\n');

            Apply();

            float lineHeight = m_sKFont.GetFontMetrics(out SKFontMetrics metrics);
            float[] lineWidths = lines
                .Select(x => m_sKFont.MeasureText(x, m_sKPaint))
                .ToArray();

            float width = lineWidths.Length == 0 ? 0 : lineWidths.Max();
            return (width, lineHeight);
        }

    }
}
