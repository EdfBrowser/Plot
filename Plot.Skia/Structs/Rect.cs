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

        internal float HorizontalCenter => (Left + Right) / 2;
        internal float VerticalCenter => (Top + Bottom) / 2;

        internal float Left => m_left;
        internal float Right => m_right;
        internal float Top => m_top;
        internal float Bottom => m_bottom;

        public float Width => m_right - m_left;
        public float Height => m_bottom - m_top;

        internal PointF TopLeft => new PointF(Left, Top);
        internal PointF TopRight => new PointF(Right, Top);
        internal PointF BottomLeft => new PointF(Left, Bottom);
        internal PointF BottomRight => new PointF(Right, Bottom);

        internal Rect WithPan(PointF p) => WithPan(p.X, p.Y);

        internal Rect WithPan(float x, float y)
            => new Rect(Left + x, Right + x, Top + y, Bottom + y);

        internal SKRect ToSKRect() => new SKRect(Left, Top, Right, Bottom);

        internal bool ContainsX(float val) => (Left <= val && val <= Right);
        internal bool ContainsY(float val) => (Top <= val && val <= Bottom);

        internal bool Contains(PointF p) => Contains(p.X, p.Y);

        internal bool Contains(float x, float y) => ContainsX(x) && ContainsY(y);

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
