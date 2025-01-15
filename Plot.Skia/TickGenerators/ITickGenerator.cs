namespace Plot.Skia
{
    public interface ITickGenerator
    {
        Tick[] Ticks { get; }

        void Generate(Range range, Edge direction, float axisLength,
            LabelStyle tickLabelStyle);
    }
}
