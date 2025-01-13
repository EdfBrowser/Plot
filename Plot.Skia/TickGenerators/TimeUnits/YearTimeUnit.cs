using System;
using System.Collections.Generic;
using System.Globalization;

namespace Plot.Skia
{
    internal class YearTimeUnit : ITimeUnit
    {
        public DateTimeFormatInfo DateTimeFormat => CultureInfo.CurrentCulture.DateTimeFormat;

        public IReadOnlyList<int> Divisors => StandardDivisors.m_years;

        public TimeSpan MinSize => TimeSpan.FromDays(365);

        public string GetFormatString() => $"yyyy";

        public DateTime Snap(DateTime dateTime)
            => new DateTime(dateTime.Year, 1, 1, 0, 0, 0);

        public DateTime Next(DateTime dateTime, int increment = 1)
            => dateTime.AddYears(increment);
    }
}
