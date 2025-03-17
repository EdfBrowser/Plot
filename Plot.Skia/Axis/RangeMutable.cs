namespace Plot.Skia
{
    public class RangeMutable
    {
        internal RangeMutable(double low, double high)
        {
            Low = low;
            High = high;
        }

        internal double Low { get; set; }
        internal double High { get; set; }

        internal double Span => High - Low;
        internal double Center => (High + Low) / 2;

        internal bool HasBeenSet => Valid;

        internal bool Valid => !(double.IsNaN(Span) || double.IsInfinity(Span) || Low > High || Low == High);

        public Range ToRange => new Range(Low, High);

        internal static RangeMutable NotSet
            => new RangeMutable(double.PositiveInfinity, double.NegativeInfinity);

        internal void Expand(double paddingRatio)
        {
            double padding = Span * paddingRatio / 2.0;
            Set(Low - padding, High + padding);
        }

        internal void Set(double low, double high)
        {
            Low = low;
            High = high;
        }

        internal void Pan(double units)
        {
            Low += units;
            High += units;
        }

        internal void Zoom(double frac, double from)
        {
            double leftSpan = from - Low;
            double rightSpan = High - from;
            Low = from - leftSpan / frac;
            High = from + rightSpan / frac;
        }
    }
}
