namespace Plot.Skia
{
    internal readonly struct PanelSize
    {
        private readonly float m_width;
        private readonly float m_height;

        internal PanelSize(float width, float height)
        {
            m_width = width;
            m_height = height;
        }

        internal float Width => m_width;
        internal float Height => m_height;

        internal bool Contains(PanelSize size)
            => Width >= size.Width && Height >= size.Height;
    }
}
