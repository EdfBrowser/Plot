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

        public int GetTickCount(DateTime minDT, DateTime maxDT, int inc)
            => (int)((maxDT - minDT).TotalSeconds / inc) + 1;
        public DateTime GetTick(DateTime minDT, int index, int inc)
            => Next(minDT, inc * index);
    }
}
