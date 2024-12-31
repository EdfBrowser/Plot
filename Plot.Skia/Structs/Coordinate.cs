namespace Plot.Skia.Structs
{
    internal readonly struct Coordinate
    {
        private readonly float m_x;
        private readonly float m_y;

        public Coordinate(float x, float y)
        {
            m_x = x;
            m_y = y;
        }

        public float X => m_x;
        public float Y => m_y;
    }
}
