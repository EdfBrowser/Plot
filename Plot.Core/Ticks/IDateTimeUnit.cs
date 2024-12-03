using System;

namespace Plot.Core.Ticks
{
    public interface IDateTimeUnit
    {
        (double[], string[]) GetTicksAndLabels(DateTime from, DateTime to, string format);
    }
}
