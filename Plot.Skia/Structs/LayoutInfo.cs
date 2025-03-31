namespace Plot.Skia
{
    internal readonly struct LayoutInfo
    {
        private readonly float _offset;
        private readonly float _size;

        internal LayoutInfo(float offset, float size)
        {
            _offset = offset;
            _size = size;
        }

        internal float Offset => _offset;
        internal float Size => _size;
    }
}
