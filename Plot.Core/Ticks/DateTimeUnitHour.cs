using Plot.Core.Enum;
using System;
using System.Globalization;

namespace Plot.Core.Ticks
{
    public class DateTimeUnitHour : DateTimeUnitBase
    {
        public DateTimeUnitHour(CultureInfo culture, int maxTickCount)
            : base(culture, maxTickCount)
        {
            m_kind = DateTimeUnit.Hour;
            m_deltas = new int[] { 1, 2, 4, 8, 12, 24 };
        }

        protected override DateTime Floor(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day);
        }

        protected override DateTime Increment(DateTime value, int delta)
        {
            return value.AddHours(delta);
        }

        protected override string GetTickLabel(DateTime value)
        {
            string date = value.ToString("d", m_culture); // short date
            string time = value.ToString("t", m_culture); // short time
            return $"{date}\n{time}";
        }
    }
}
