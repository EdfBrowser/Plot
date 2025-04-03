namespace Plot.Skia
{
    public interface IMeasureable
    {
        float Space { get; set; }
        float Measure(bool force = false);
    }
}
