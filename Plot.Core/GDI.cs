using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Plot.Core
{
    internal class GDI
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

        public static Graphics Graphics(Bitmap bmp, bool lowQuality = false, float scale = 1f)
        {
            Graphics gfx = System.Drawing.Graphics.FromImage(bmp);
            gfx.SmoothingMode = lowQuality ? SmoothingMode.HighSpeed : SmoothingMode.AntiAlias;
            gfx.TextRenderingHint = lowQuality ? TextRenderingHint.SingleBitPerPixelGridFit : TextRenderingHint.AntiAlias;
            gfx.ScaleTransform(scale, scale);
            return gfx;
        }


        public static Brush Brush(Color color, int alpha) => new SolidBrush(Color.FromArgb((int)(alpha * 255), color));


        public static Pen Pen(Color color, float width, int alpha) => new Pen(Color.FromArgb((int)(alpha * 255), color), width);
    }
}
