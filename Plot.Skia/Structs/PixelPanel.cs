using SkiaSharp;

namespace Plot.Skia
{
    internal readonly struct PixelPanel
    {
        private readonly float m_left;
        private readonly float m_right;
        private readonly float m_top;
        private readonly float m_bottom;

        internal PixelPanel(float left, float right, float top, float bottom)
        {
            m_left = left;
            m_right = right;
            m_top = top;
            m_bottom = bottom;
        }


        internal float Horizontal => Left + Right;
        internal float Vertical => Top + Bottom;

        internal float Left => m_left;
        internal float Right => m_right;
        internal float Top => m_top;
        internal float Bottom => m_bottom;

        internal float Width => m_right - m_left;
        internal float Height => m_bottom - m_top;

        internal Coordinate TopLeft => new Coordinate(m_left, m_top);
        internal Coordinate TopRight => new Coordinate(m_right, m_top);
        internal Coordinate BottomLeft => new Coordinate(m_left, m_bottom);
        internal Coordinate BottomRight => new Coordinate(m_right, m_bottom);
    }

    internal static class PixelPanelExtensions
    {
        internal static PixelPanel ToPixelPanel(this SKRect rect)
        {
            return new PixelPanel(rect.Left, rect.Right, rect.Bottom, rect.Top);
        }
    }
}
