namespace Plot.Skia
{
    public class PixelRangeMutable
    {
        internal PixelRangeMutable(double min, double max)
        {
            Min = min;
            Max = max;
        }

        internal double Min { get; set; }
        internal double Max { get; set; }

        internal double Span => Max - Min;

        internal bool HasBeenSet
            => !(double.IsNaN(Span) || double.IsInfinity(Span)) && Span != 0;

        internal Range ToPixelRange => new Range(Min, Max);

        internal static PixelRangeMutable NotSet
            => new PixelRangeMutable(double.PositiveInfinity, double.NegativeInfinity);

        internal void Set(double min, double max)
        {
            Min = min;
            Max = max;
        }
    }
}
