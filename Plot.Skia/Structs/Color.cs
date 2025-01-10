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

        internal byte Red => m_red;
        internal byte Green => m_green;
        internal byte Blue => m_blue;
        internal byte Alpha => m_alpha;

        public static Color White => new Color(255, 255, 255);
        public static Color Black => new Color(0, 0, 0);
        public static Color Gray => new Color(128, 128, 128);

        internal SKColor ToSkColor() => new SKColor(Red, Green, Blue, Alpha);
    }
}
