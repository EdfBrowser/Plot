namespace Plot.Skia
{
    internal interface ITickGenerator
    {
        Tick[] Ticks { get; set; }

        void Generate(CoordinateRange range, Edge edge, float axisLength,
            LabelStyle labelStyle);
    }
}
