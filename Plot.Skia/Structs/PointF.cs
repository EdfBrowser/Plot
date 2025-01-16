using SkiaSharp;

namespace Plot.Skia
{
    public readonly struct PointF
    {
        private readonly float m_x;
        private readonly float m_y;

        public PointF(float x, float y)
        {
            m_x = x;
            m_y = y;
        }


        internal float X => m_x;
        internal float Y => m_y;

        internal static PointF NoSet
            => new PointF(float.PositiveInfinity, float.NegativeInfinity);


        internal SKPoint ToSKPoint() => new SKPoint(X, Y);

        internal PointF Translate(float x, float y) => new PointF(m_x + x, m_y + y);
    }
}
