namespace Plot.Skia.Structs
{
    internal readonly struct Tick
    {
        private readonly double m_position;
        private readonly string m_label;
        private readonly bool m_major;

        public Tick(double position, string label, bool major)
        {
            m_position = position;
            m_label = label;
            m_major = major;
        }

        public double Position => m_position;
        public string Label => m_label;
        public bool Major => m_major;
    }
}
