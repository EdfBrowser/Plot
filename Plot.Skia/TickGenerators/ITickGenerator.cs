namespace Plot.Skia
{
    public interface ITickGenerator
    {
        Tick[] Ticks { get; }

        void Generate(PixelRange range, Edge direction, float axisLength,
            LabelStyle tickLabelStyle);
    }
}
