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

        public Color(uint argb)
        {
            // byte 取最低8bit
            m_alpha = (byte)(argb >> 24);
            m_red = (byte)(argb >> 16);
            m_green = (byte)(argb >> 8);
            m_blue = (byte)(argb >> 0);
        }


        internal byte R => m_red;
        internal byte G => m_green;
        internal byte B => m_blue;
        internal byte A => m_alpha;

        internal uint PremultipliedARGB
        {
            get
            {
                byte r = (byte)((m_red * m_alpha) / 255);
                byte g = (byte)((m_green * m_alpha) / 255);
                byte b = (byte)((m_blue * m_alpha) / 255);

                return
                    ((uint)m_alpha << 24) |
                    ((uint)m_red << 16) |
                    ((uint)m_green << 8) |
                    ((uint)m_blue << 0);
            }
        }

        public static Color FromARGB(uint argb) => new Color(argb);
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
