namespace Plot.Skia
{
    internal class PixelSizeMutable
    {
        internal PixelSizeMutable(float width, float height)
        {
            Width = width;
            Height = height;
        }

        internal float Width { get; set; }
        internal float Height { get; set; }

        internal SizeF ToPixelSize => new SizeF(Width, Height);

        internal void Set(float w, float h)
        {
            Width = w;
            Height = h;
        }
    }
}
