using System;
using System.Collections.Generic;
using System.Globalization;

namespace Plot.Skia
{
    internal class SecondTimeUnit : ITimeUnit
    {
        public DateTimeFormatInfo DateTimeFormat => CultureInfo.CurrentCulture.DateTimeFormat;

        public IReadOnlyList<int> Divisors => StandardDivisors.m_sexagesimal;

        public TimeSpan MinSize => TimeSpan.FromSeconds(1);

        public string GetFormatString()
            => $"{DateTimeFormat.ShortDatePattern}\n{DateTimeFormat.LongTimePattern}";

        public DateTime Snap(DateTime dateTime)
            => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                dateTime.Hour, dateTime.Minute, dateTime.Second);

        public DateTime Next(DateTime dateTime, int increment = 1)
            => dateTime.AddSeconds(increment);
    }
}
