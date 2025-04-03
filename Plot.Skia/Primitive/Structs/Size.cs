using System;

namespace Plot.Skia
{
    internal readonly struct Size<T> where T : IComparable<T>
    {
        private readonly T m_width;
        private readonly T m_height;

        internal Size(T width, T height)
        {
            m_width = width;
            m_height = height;
        }

        internal T Width => m_width;
        internal T Height => m_height;

        internal bool Contains(Size<T> size)
            => Width.CompareTo(size.Width) >= 0 &&
            Height.CompareTo(size.Height) >= 0;

        internal static Size<float> Empty => new Size<float>(0.0f, 0.0f);
    }
}
