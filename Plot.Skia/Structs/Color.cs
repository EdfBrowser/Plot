using SkiaSharp;

namespace Plot.Skia
{
    public readonly struct Color
    {
        private readonly byte m_red;
        private readonly byte m_green;
        private readonly byte m_blue;
        private readonly byte m_alpha;


        public Color(byte red, byte green, byte blue, byte alpha = 255)
        {
            m_red = red;
            m_green = green;
            m_blue = blue;
            m_alpha = alpha;
        }

        public Color(float red, float green, float blue, float alpha = 1f)
        {
            m_red = (byte)(red * 255);
            m_green = (byte)(green * 255);
            m_blue = (byte)(blue * 255);
            m_alpha = (byte)(alpha * 255);
        }

        internal byte R => m_red;
        internal byte G => m_green;
        internal byte B => m_blue;
        internal byte A => m_alpha;

        public static Color White => new Color(255, 255, 255);
        public static Color Black => new Color(0, 0, 0);
        public static Color Gray => new Color(128, 128, 128);
        public static Color Red => new Color(255, 0, 0);
    }

    internal static class ColorExtensions
    {
        internal static SKColor ToSkColor(this Color color)
            => new SKColor(color.R, color.G, color.B, color.A);
    }
}
