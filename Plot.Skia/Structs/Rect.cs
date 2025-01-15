using SkiaSharp;
using System;

namespace Plot.Skia
{
    public readonly struct Rect : IEquatable<Rect>
    {
        private readonly float m_left;
        private readonly float m_right;
        private readonly float m_top;
        private readonly float m_bottom;

        internal Rect(float left, float right, float top, float bottom)
        {
            m_left = left;
            m_right = right;
            m_top = top;
            m_bottom = bottom;
        }


        internal Rect(PointF location, SizeF dataRect)
            : this(location.X, location.X + dataRect.Width,
                  location.Y, location.Y + dataRect.Height)
        { }

        internal float Hoffset => Left + Right;
        internal float Voffset => Top + Bottom;

        internal float Left => m_left;
        internal float Right => m_right;
        internal float Top => m_top;
        internal float Bottom => m_bottom;

        public float Width => m_right - m_left;
        public float Height => m_bottom - m_top;

        internal PointF TopLeft => new PointF(m_left, m_top);
        internal PointF TopRight => new PointF(m_right, m_top);
        internal PointF BottomLeft => new PointF(m_left, m_bottom);
        internal PointF BottomRight => new PointF(m_right, m_bottom);

        internal Rect WithPan(float x, float y)
            => new Rect(Left + x, Right + x, Top + y, Bottom + y);

        internal SKRect ToSKRect() => new SKRect(Left, Top, Right, Bottom);

        public bool Equals(Rect other)
            => Equals(Left, other.Left) &&
               Equals(Right, other.Right) &&
               Equals(Top, other.Top) &&
               Equals(Bottom, other.Bottom);
    }

    internal static class RectExtensions
    {
        internal static Rect ToRect(this SKRect rect)
        {
            return new Rect(rect.Left, rect.Right, rect.Top, rect.Bottom);
        }
    }
}
