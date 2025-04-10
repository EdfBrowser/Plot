namespace Plot.Skia
{
    public readonly struct Size<T>
    {
        private readonly T _width;
        private readonly T _height;

        public Size(T width, T height)
        {
            _width = width;
            _height = height;
        }

        public T Width => _width;
        public T Height => _height;

        public static Size<float> Empty => new Size<float>(0.0f, 0.0f);
    }
}
