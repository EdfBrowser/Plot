using Plot.Core.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Plot.Core.Ticks
{
    public static class DateTimeUnitFactory
    {
        public static IDateTimeUnit Create(DateTimeUnit kind, CultureInfo culture, int maxTickCount)
        {
            switch (kind)
            {
                case DateTimeUnit.Hour:
                    return new DateTimeUnitHour(culture, maxTickCount);
                case DateTimeUnit.Minute:
                    return new DateTimeUnitMinute(culture, maxTickCount);
                case DateTimeUnit.Second:
                    return new DateTimeUnitSecond(culture, maxTickCount);
                default:
                    throw new NotImplementedException($"unsupported kind type {kind}");
            }
        }

        // TODO: DateTime结构体传递问题
        public static IDateTimeUnit CreateBestUnit(DateTime from, DateTime to, CultureInfo culture, int maxTickCount)
        {
            double daysApart = to.ToOADate() - from.ToOADate();

            int halfDensity = maxTickCount / 2;

            // tick unit borders in days
            var tickUnitBorders = new List<(DateTimeUnit kind, double border)?>
            {
                (DateTimeUnit.Hour, 1.0 / 24 * halfDensity),
                (DateTimeUnit.Minute, 1.0 / 24 / 60 * halfDensity),
                (DateTimeUnit.Second, 1.0 / 24 / 3600 * halfDensity),
            };

            var bestTickUnitKind = tickUnitBorders.FirstOrDefault(tr => daysApart > tr.Value.border);
            bestTickUnitKind = bestTickUnitKind ?? tickUnitBorders.Last(); // last tickUnit if not found best

            return Create(bestTickUnitKind.Value.kind, culture, maxTickCount);
        }
    }
}
