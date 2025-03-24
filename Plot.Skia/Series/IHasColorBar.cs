namespace Plot.Skia
{
    public interface IHasColorBar
    {
        IColorMap ColorMap { get; }
        Range GetRange();
    }
}
