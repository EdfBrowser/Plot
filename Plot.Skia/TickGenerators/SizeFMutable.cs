namespace Plot.Skia
{
    internal class SizeFMutable
    {
        internal SizeFMutable(float width, float height)
        {
            Width = width;
            Height = height;
        }

        internal float Width { get; set; }
        internal float Height { get; set; }

        internal SizeF ToSizeF => new SizeF(Width, Height);

        internal void Set(float w, float h)
        {
            Width = w;
            Height = h;
        }
    }
}
