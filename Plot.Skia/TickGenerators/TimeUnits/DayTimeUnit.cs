using System;
using System.Collections.Generic;
using System.Globalization;

namespace Plot.Skia
{
    internal class DayTimeUnit : ITimeUnit
    {
        public DateTimeFormatInfo DateTimeFormat => CultureInfo.CurrentCulture.DateTimeFormat;

        public IReadOnlyList<int> Divisors => StandardDivisors.m_days;

        public TimeSpan MinSize => TimeSpan.FromDays(1);

        public string GetFormatString() => $"d";

        public DateTime Snap(DateTime dateTime)
            => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);

        public DateTime Next(DateTime dateTime, int increment = 1)
            => dateTime.AddDays(increment);
    }
}
