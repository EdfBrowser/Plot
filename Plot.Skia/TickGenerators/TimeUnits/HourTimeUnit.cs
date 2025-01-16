using System;
using System.Collections.Generic;
using System.Globalization;

namespace Plot.Skia
{
    internal class HourTimeUnit : ITimeUnit
    {
        public DateTimeFormatInfo DateTimeFormat => CultureInfo.CurrentCulture.DateTimeFormat;

        public IReadOnlyList<int> Divisors => StandardDivisors.m_dozenal;

        public TimeSpan MinSize => TimeSpan.FromHours(1);

        public string GetFormatString()
            => $"{DateTimeFormat.ShortDatePattern}\n{DateTimeFormat.ShortTimePattern}";

        public DateTime Snap(DateTime dateTime)
            => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                dateTime.Hour, 0, 0);

        public DateTime Next(DateTime dateTime, int increment = 1)
            => dateTime.AddHours(increment);
        public int GetTickCount(DateTime minDT, DateTime maxDT, int inc)
           => (int)((maxDT - minDT).TotalHours / inc) + 1;
        public DateTime GetTick(DateTime minDT, int index, int inc)
            => Next(minDT, inc * index);
    }
}
