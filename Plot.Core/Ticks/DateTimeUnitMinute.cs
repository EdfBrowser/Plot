using Plot.Core.Enum;
using System;
using System.Globalization;

namespace Plot.Core.Ticks
{
    public class DateTimeUnitMinute : DateTimeUnitBase
    {
        public DateTimeUnitMinute(CultureInfo culture, int maxTickCount, double? manualSpacing)
            : base(culture, maxTickCount, manualSpacing)
        {
            m_kind = DateTimeUnit.Minute;
            m_deltas = new double[] { 1.0, 2.0, 5.0, 10.0, 15.0, 30.0 };
        }

        protected override DateTime Floor(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, 0, 0);
        }

        protected override DateTime Increment(DateTime value, double delta)
        {
            return value.AddMinutes(delta);
        }

        protected override string GetTickLabel(DateTime value)
        {
            string date = value.ToString("d", m_culture); // short date
            string time = value.ToString("t", m_culture); // short time
            return $"{date} {time}";
        }
    }
}
