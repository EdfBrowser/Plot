namespace Plot.Skia
{
    public class RangeMutable
    {
        internal RangeMutable(double min, double max)
        {
            Min = min;
            Max = max;
        }

        internal double Min { get; set; }
        internal double Max { get; set; }

        internal double Span => Max - Min;
        internal double Center => (Max + Min) / 2;

        internal bool HasBeenSet
            => !(double.IsNaN(Span) || double.IsInfinity(Span)) && Span != 0;

        internal Range ToRange => new Range(Min, Max);

        internal static RangeMutable NotSet
            => new RangeMutable(double.PositiveInfinity, double.NegativeInfinity);

        internal void Set(double min, double max)
        {
            Min = min;
            Max = max;
        }

        internal void Pan(double units)
        {
            Min += units;
            Max += units;
        }

        internal void Zoom(double frac, double from)
        {
            double leftSpan = from - Min;
            double rightSpan = Max - from;
            Min = from - leftSpan / frac;
            Max = from + rightSpan / frac;
        }
    }
}
