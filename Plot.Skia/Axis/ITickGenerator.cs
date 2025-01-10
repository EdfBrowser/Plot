namespace Plot.Skia
{
    internal interface ITickGenerator
    {
        Tick[] Ticks { get; }

        void Generate(PixelRange range, Edge edge, float axisLength,
            LabelStyle labelStyle);
    }
}
