using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

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

        public static Graphics Graphics(Bitmap bmp, bool lowQuality = false, double scale = 1.0)
        {
            Graphics gfx = System.Drawing.Graphics.FromImage(bmp);
            gfx.SmoothingMode = lowQuality ? SmoothingMode.HighSpeed : SmoothingMode.AntiAlias;
            gfx.TextRenderingHint = lowQuality ? TextRenderingHint.SingleBitPerPixelGridFit : TextRenderingHint.AntiAlias;
            gfx.ScaleTransform((float)scale, (float)scale);
            return gfx;
        }


        public static Brush Brush(Color color, float alpha) => new SolidBrush(Color.FromArgb((int)(alpha * 255), color));


        public static Pen Pen(Color color, float width, float alpha) => new Pen(Color.FromArgb((int)(alpha * 255), color), width);
    }
}
