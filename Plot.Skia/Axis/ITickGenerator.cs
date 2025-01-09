namespace Plot.Skia
{
    internal interface ITickGenerator
    {
        Tick[] Ticks { get; set; }
        float LargestLabelLength { get; set; }

        void Generate(PixelRange range, Edge edge, float axisLength,
            LabelStyle labelStyle);
    }
}
