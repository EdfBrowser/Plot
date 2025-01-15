using System;

namespace Plot.Skia
{
    public readonly struct Range
    {
        private readonly double m_low;
        private readonly double m_high;

        public Range(double low, double high)
        {
            m_low = low;
            m_high = high;
        }

        internal double Low => m_low;
        internal double High => m_high;

        internal double Span => m_high - m_low;

        public static Range DefaultNumeric => new Range(-10, 10);
        public static Range DefaultDateTime(DateTime dt, double range)
            => new Range(dt.ToOADate(), dt.AddSeconds(range).ToOADate());

        internal bool Contains(double value) => m_low <= value && m_high >= value;
    }
}
