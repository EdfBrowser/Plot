namespace Plot.Skia
{
    internal readonly struct PixelColumn
    {
        private readonly float m_endY;
        private readonly float m_midMaxY;
        private readonly float m_midMinY;
        private readonly float m_startY;
        private readonly float m_position;

        internal PixelColumn(float position, float startY, float midMinY,
            float midMaxY, float endY)
        {
            m_position = position;
            m_startY = startY;
            m_midMinY = midMinY;
            m_midMaxY = midMaxY;
            m_endY = endY;
        }

        internal float Position => m_position;
        internal float StartY => m_startY;
        internal float MidMinY => m_midMinY;
        internal float MidMaxY => m_midMaxY;
        internal float EndY => m_endY;
    }
}
