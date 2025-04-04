using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Plot.Core.Draws
{
    public static class GDI
    {
        public static Font Font(string fontName = null, float fontSize = 14, bool bold = false, FontFamily fontFamily = null)
        {
            if (fontName != null)
            {
                fontFamily = fontFamily ?? new FontFamily(fontName);
            }
            fontFamily = fontFamily ?? SystemFonts.DefaultFont.FontFamily;
            FontStyle fontStyle = bold ? FontStyle.Bold : FontStyle.Regular;
            return new Font(fontFamily, fontSize, fontStyle, GraphicsUnit.Pixel);
        }


        public static SizeF MeasureStringUsingTemporaryGraphics(string text, Font font)
        {
            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics gfx = Graphics(bmp, true, 1f))
            {
                SizeF sizef = MeasureString(gfx, text, font);
                return sizef;
            }
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


        public static Brush Brush(Color color, float alpha = 1) => new SolidBrush(Color.FromArgb((int)(alpha * 255), color));


        public static Pen Pen(Color color, float width, float alpha = 1f) => new Pen(Color.FromArgb((int)(alpha * 255), color), width);


    }
}
