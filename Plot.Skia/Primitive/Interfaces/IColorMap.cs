namespace Plot.Skia
{
    public interface IColorMap
    {
        string Name { get; }

        Color GetColor(double position);
    }
}
