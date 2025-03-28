using SkiaSharp;
using System;
using System.Linq;

namespace Plot.Skia
{
    public class LabelStyle : IDisposable
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

        public Color Color { get; set; }
        public bool AntiAlias { get; set; }
        public float FontSize { get; set; }
        public string Text { get; set; }
        public string FontFamily { get; set; }
        public bool Renderable { get; set; } = true;

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
            return Math.Abs(m_sKFont.Metrics.Ascent);
        }

        internal float Descent()
        {
            Apply();
            return m_sKFont.Metrics.Descent;
        }

        internal void Render(SKCanvas canvas, PointF p, SKTextAlign textAlign)
        {
            if (!Renderable) return;

            Apply();
            if (Text.Contains('\n'))
            {
                string[] lines = Text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    SKPoint skpoint = p.ToSKPoint();
                    skpoint.Offset(0, i * m_sKFont.Spacing);
                    canvas.DrawText(lines[i], skpoint, textAlign, m_sKFont, m_sKPaint);
                }
            }
            else
                canvas.DrawText(Text, p.ToSKPoint(), textAlign, m_sKFont, m_sKPaint);
        }

        internal SizeF Measure(string text)
        {
            string[] lines = string.IsNullOrEmpty(text)
                ? Array.Empty<string>() : text.Split('\n');

            Apply();

            float lineHeight = m_sKFont.GetFontMetrics(out SKFontMetrics metrics);
            float[] lineWidths = lines
                .Select(x => m_sKFont.MeasureText(x, m_sKPaint))
                .ToArray();

            float width = lineWidths.Length == 0 ? 0 : lineWidths.Max();
            float height = lineHeight * lines.Length;
            return new SizeF(width, height);
        }
    }
}
