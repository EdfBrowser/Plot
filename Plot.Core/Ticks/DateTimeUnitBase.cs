using Plot.Core.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Plot.Core.Ticks
{
    public abstract class DateTimeUnitBase : IDateTimeUnit
    {
        public DateTimeUnitBase(CultureInfo culture, int maxTickCount)
        {
            m_culture = culture ?? CultureInfo.CurrentCulture;
            m_maxTickCount = maxTickCount;
        }

        protected DateTimeUnit m_kind;
        protected CultureInfo m_culture;
        protected int[] m_deltas;
        protected int m_maxTickCount;

        protected virtual DateTime Floor(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);
        }

        protected virtual DateTime Increment(DateTime value, int delta)
        {
            return value.AddSeconds(delta);
        }

        protected virtual string GetTickLabel(DateTime value)
        {
            string date = value.ToString("d", m_culture); // short date
            string time = value.ToString("T", m_culture); // long time
            return $"{date}\n{time}";
        }

        public (double[], string[]) GetTicksAndLabels(DateTime from, DateTime to, string format)
        {
            DateTime[] ticks = GetTicks(from, to, m_deltas, m_maxTickCount);
            string[] labels = (format is null) ?
                ticks.Select(t => GetTickLabel(t)).ToArray() :
                ticks.Select(t => t.ToString(format, m_culture)).ToArray();
            double[] positions = ticks.Select(t => t.ToOADate()).ToArray();
            return (positions, labels);
        }

        protected DateTime[] GetTicks(DateTime from, DateTime to, int[] deltas, int maxTickCount)
        {
            DateTime[] result = new DateTime[] { };
            foreach (var delta in deltas)
            {
                result = GetTicks(from, to, delta);
                if (result.Length <= maxTickCount)
                    return result;
            }
            return result;
        }

        protected virtual DateTime[] GetTicks(DateTime from, DateTime to, int delta)
        {
            var dates = new List<DateTime>();
            DateTime dt = Floor(from);
            while (dt <= to)
            {
                if (dt >= from)
                    dates.Add(dt);
                try
                {
                    dt = Increment(dt, delta);
                }
                catch
                {
                    break; // our date is larger than possible
                }
            }
            return dates.ToArray();
        }

    }
}
