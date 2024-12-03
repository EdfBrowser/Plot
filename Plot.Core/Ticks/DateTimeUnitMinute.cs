using Plot.Core.Enum;
using System;
using System.Globalization;

namespace Plot.Core.Ticks
{
    public class DateTimeUnitMinute : DateTimeUnitBase
    {
        public DateTimeUnitMinute(CultureInfo culture, int maxTickCount)
            : base(culture, maxTickCount)
        {
            m_kind = DateTimeUnit.Minute;
            m_deltas = new int[] { 1, 2, 5, 10, 15, 30 };
        }

        protected override DateTime Floor(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, 0, 0);
        }

        protected override DateTime Increment(DateTime value, int delta)
        {
            return value.AddMinutes(delta);
        }

        protected override string GetTickLabel(DateTime value)
        {
            string date = value.ToString("d", m_culture); // short date
            string time = value.ToString("t", m_culture); // short time
            return $"{date}\n{time}";
        }
    }
}
