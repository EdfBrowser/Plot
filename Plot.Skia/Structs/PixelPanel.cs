namespace Plot.Skia.Structs
{
    internal readonly struct PixelPanel
    {
        private readonly float m_left;
        private readonly float m_right;
        private readonly float m_top;
        private readonly float m_bottom;

        public PixelPanel(float left, float right, float top, float bottom)
        {
            m_left = left;
            m_right = right;
            m_top = top;
            m_bottom = bottom;
        }


        public float Left => m_left;
        public float Right => m_right;
        public float Top => m_top;
        public float Bottom => m_bottom;

        public float Width => m_right - m_left;
        public float Height => m_bottom - m_top;

        public Coordinate TopLeft => new Coordinate(m_left, m_top);
        public Coordinate TopRight => new Coordinate(m_right, m_top);
        public Coordinate BottomLeft => new Coordinate(m_left, m_bottom);
        public Coordinate BottomRight => new Coordinate(m_right, m_bottom);
    }
}
