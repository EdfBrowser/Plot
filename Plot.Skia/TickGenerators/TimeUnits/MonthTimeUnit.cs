using System;
using System.Collections.Generic;
using System.Globalization;

namespace Plot.Skia
{
    internal class MonthTimeUnit : ITimeUnit
    {
        public DateTimeFormatInfo DateTimeFormat => CultureInfo.CurrentCulture.DateTimeFormat;

        public IReadOnlyList<int> Divisors => StandardDivisors.m_months;

        public TimeSpan MinSize => TimeSpan.FromDays(28);

        public string GetFormatString() => $"d";

        public DateTime Snap(DateTime dateTime)
            => new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);

        public DateTime Next(DateTime dateTime, int increment = 1)
            => dateTime.AddMonths(increment);
    }
}
