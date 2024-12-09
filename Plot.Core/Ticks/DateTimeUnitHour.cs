using Plot.Core.Enum;
using System;
using System.Globalization;

namespace Plot.Core.Ticks
{
    public class DateTimeUnitHour : DateTimeUnitBase
    {
        public DateTimeUnitHour(CultureInfo culture, int maxTickCount, double? manualSpacing)
            : base(culture, maxTickCount, manualSpacing)
        {
            m_kind = DateTimeUnit.Hour;
            m_deltas = new double[] { 1.0, 2.0, 4.0, 8.0, 12.0, 24.0 };
        }

        protected override DateTime Floor(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day);
        }

        protected override DateTime Increment(DateTime value, double delta)
        {
            return value.AddHours(delta);
        }

        protected override string GetTickLabel(DateTime value)
        {
            string date = value.ToString("d", m_culture); // short date
            string time = value.ToString("t", m_culture); // short time
            return $"{date} {time}";
        }
    }
}
