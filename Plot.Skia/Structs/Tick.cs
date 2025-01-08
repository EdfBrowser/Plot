namespace Plot.Skia
{
    internal readonly struct Tick
    {
        private readonly double m_position;
        private readonly string m_label;
        private readonly bool m_major;

        internal Tick(double position, string label, bool major)
        {
            m_position = position;
            m_label = label;
            m_major = major;
        }

        internal double Position => m_position;
        internal string Label => m_label;
        internal bool MajorPos => m_major;

        internal static Tick Major(double pos, string label) => new Tick(pos, label, true);

        internal static Tick Minor(double pos) => new Tick(pos, string.Empty, false);
    }
}
