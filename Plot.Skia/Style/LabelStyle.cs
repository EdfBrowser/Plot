using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    public class LabelStyle : IDisposable
    {
        private readonly Dictionary<string, SizeF> _measureCache;
        private readonly SKPaint _sKPaint;
        private readonly SKFont _sKFont;

        public LabelStyle()
        {
            _measureCache = new Dictionary<string, SizeF>();

            _sKPaint = new SKPaint();
            _sKFont = new SKFont();

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
        public SKTextAlign TextAlign { get; set; } = SKTextAlign.Left;

        private void Apply()
        {
            _sKFont.Size = FontSize;
            _sKFont.Typeface = SKTypeface.FromFamilyName(FontFamily);

            _sKPaint.Style = SKPaintStyle.Fill;
            _sKPaint.Color = Color.ToSkColor();
            _sKPaint.IsAntialias = AntiAlias;
        }

        public void Dispose()
        {
            _sKFont?.Dispose();
            _sKPaint?.Dispose();
        }

        internal float Ascent()
        {
            Apply();
            return Math.Abs(_sKFont.Metrics.Ascent);
        }

        internal float Descent()
        {
            Apply();
            return _sKFont.Metrics.Descent;
        }

        internal void ClearMeasureCache()
        {
            _measureCache.Clear();
        }

        internal void Render(SKCanvas canvas, PointF p)
        {
            if (!Renderable) return;

            Apply();
            if (Text.Contains('\n'))
            {
                string[] lines = Text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    SKPoint skpoint = p.ToSKPoint();
                    skpoint.Offset(0, i * _sKFont.Spacing);
                    canvas.DrawText(lines[i], skpoint, TextAlign, _sKFont, _sKPaint);
                }
            }
            else
                canvas.DrawText(Text, p.ToSKPoint(), TextAlign, _sKFont, _sKPaint);
        }

        internal SizeF Measure(string text, bool force = false)
        {
            if (string.IsNullOrEmpty(text))
                return SizeF.Empty;

            if (!force)
            {
                if (_measureCache.TryGetValue(text, out var cachedSize))
                    return cachedSize;
            }

            string[] lines = text.Split('\n');

            Apply();

            float lineHeight = _sKFont.GetFontMetrics(out _);
            float maxWidth = 0f;

            foreach (var line in lines)
            {
                float lineWidth = _sKFont.MeasureText(line, _sKPaint);
                maxWidth = Math.Max(maxWidth, lineWidth);
            }

            var size = new SizeF(maxWidth, lineHeight * lines.Length);
            _measureCache[text] = size;
            return size;
        }
    }
}
