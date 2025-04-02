namespace Plot.Skia
{
    internal readonly struct SizeF
    {
        private readonly float m_width;
        private readonly float m_height;

        internal SizeF(float width, float height)
        {
            m_width = width;
            m_height = height;
        }

        public static SizeF Empty => new SizeF(0.0f, 0.0f);

        internal float Width => m_width;
        internal float Height => m_height;

        internal bool Contains(SizeF size)
            => Width >= size.Width && Height >= size.Height;
    }
}
