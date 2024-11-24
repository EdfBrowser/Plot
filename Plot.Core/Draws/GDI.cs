using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Plot.Core.Draws
{
    public static class GDI
    {
        public static Font Font(string fontName = null, float fontSize = 12, bool bold = false, FontFamily fontFamily = null)
        {
            if (fontName != null)
            {
                fontFamily = fontFamily ?? new FontFamily(fontName);
            }
            fontFamily = fontFamily ?? SystemFonts.DefaultFont.FontFamily;
            FontStyle fontStyle = bold ? FontStyle.Bold : FontStyle.Regular;
            return new Font(fontFamily, fontSize, fontStyle, GraphicsUnit.Pixel);
        }

        public static SizeF MeasureString(Graphics gfx, string text, Font font)
        {
            SizeF size = gfx.MeasureString(text, font);

            // ensure the measured height is at least the font size
            size.Height = Math.Max(font.Size, size.Height);

            return size;
        }

        public static Graphics Graphics(Bitmap bmp, bool lowQuality, float scale)
        {
            Graphics gfx = System.Drawing.Graphics.FromImage(bmp);
            gfx.SmoothingMode = lowQuality ? SmoothingMode.HighSpeed : SmoothingMode.AntiAlias;
            gfx.TextRenderingHint = lowQuality ? TextRenderingHint.SingleBitPerPixelGridFit : TextRenderingHint.AntiAlias;
            gfx.ScaleTransform(scale, scale);
            return gfx;
        }

        public static Graphics Graphics(Bitmap bmp, PlotDimensions dims, bool lowQuality, bool clip = false)

        {
            Graphics gfx = Graphics(bmp, lowQuality, dims.m_scaleFactor);
            if (clip)
            {
                float left = (float)Math.Round(dims.m_dataOffsetX) + 1;
                float top = (float)Math.Round(dims.m_dataOffsetY) + 1;
                float width = (float)Math.Round(dims.m_dataWidth) - 1;
                float height = (float)Math.Round(dims.m_dataHeight) - 1;
                gfx.Clip = new Region(new RectangleF(left, top, width, height));
            }

            return gfx;
        }


        public static Brush Brush(Color color, int alpha = 1) => new SolidBrush(Color.FromArgb(alpha * 255, color));


        public static Pen Pen(Color color, float width, int alpha = 1) => new Pen(Color.FromArgb(alpha * 255, color), width);
    }
}
