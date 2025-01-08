using System;

namespace Plot.Skia
{
    internal class CoordinateRangeMutable
    {
        internal CoordinateRangeMutable(double min, double max)
        {
            Min = min;
            Max = max;
        }

        internal double Min { get; set; }
        internal double Max { get; set; }

        internal double Span => Max - Min;

        internal bool HasBeenSet
            => (!double.IsNaN(Span) &&!double.IsInfinity(Span)) && Span != 0;

        internal CoordinateRange ToCoordinateRange => new CoordinateRange(Min, Max);

        internal static CoordinateRangeMutable NotSet
            => new CoordinateRangeMutable(double.PositiveInfinity, double.NegativeInfinity);

        internal void Set(double min, double max)
        {
            Min = min;
            Max = max;
        }
    }
}
