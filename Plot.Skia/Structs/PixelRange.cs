using System;

namespace Plot.Skia
{
    public readonly struct PixelRange
    {
        private readonly double m_low;
        private readonly double m_high;

        public PixelRange(double low, double high)
        {
            m_low = low;
            m_high = high;
        }

        internal double Low => m_low;
        internal double High => m_high;

        internal double Span => m_high - m_low;

        public static PixelRange DefaultNumeric => new PixelRange(-10, 10);
        public static PixelRange DefaultDateTime(DateTime dt)
            => new PixelRange(dt.ToOADate(), dt.AddSeconds(10).ToOADate());
        public static PixelRange SpecifyDateTime(DateTime dt, double range)
            => new PixelRange(dt.ToOADate(), dt.AddSeconds(range).ToOADate());

        internal bool Contains(double value) => m_low <= value && m_high >= value;
    }
}
