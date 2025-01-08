namespace Plot.Skia
{
    internal readonly struct Coordinate
    {
        private readonly float m_x;
        private readonly float m_y;

        internal Coordinate(float x, float y)
        {
            m_x = x;
            m_y = y;
        }

        internal float X => m_x;
        internal float Y => m_y;
    }
}
