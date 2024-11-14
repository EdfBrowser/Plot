using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Plot.Core
{
    internal class GDI
    {

        public static SizeF MeasureString(Graphics gfx, string text, Font font)
        {
            SizeF size = gfx.MeasureString(text, font);

            // ensure the measured height is at least the font size
            size.Height = Math.Max(font.Size, size.Height);

            return size;
        }

        public static Graphics CreateGraphics(Bitmap bmp, bool lowQuality = false, double scale = 1.0)
        {
            Graphics gfx = Graphics.FromImage(bmp);
            gfx.SmoothingMode = lowQuality ? SmoothingMode.HighSpeed : SmoothingMode.AntiAlias;
            //gfx.TextRenderingHint = lowQuality ? LowQualityTextRenderingHint : HighQualityTextRenderingHint;
            gfx.ScaleTransform((float)scale, (float)scale);
            return gfx;
        }


        public static Brush Brush(Color color, float alpha) => new SolidBrush(Color.FromArgb((int)(alpha * 255), color));
    }
}
