using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    internal class DateTimeAutoGenerator : ITickGenerator
    {
        private readonly IReadOnlyList<ITimeUnit> m_theseTimeUnits;

        internal DateTimeAutoGenerator()
        {
            m_theseTimeUnits = new List<ITimeUnit>()
            {
               new SecondTimeUnit(),
               new MinuteTimeUnit(),
               new HourTimeUnit(),
               new DayTimeUnit(),
               new MonthTimeUnit(),
               new YearTimeUnit(),
            };
        }

        public Tick[] Ticks { get; private set; }

        // TODO: 重新设计
        public void Generate(PixelRange range, Edge direction, float axisLength, LabelStyle tickLabelStyle)
        {
            PixelSizeMutable tickLabelBound = new PixelSizeMutable(16, 12);

            while (true)
            {
                (IList<Tick> ticks, PanelSize? largestTickLabelSize) =
                    GenerateDateTimeTicks(range, direction, axisLength,
                                tickLabelBound.ToPixelSize, tickLabelStyle);

                if (ticks != null)
                {
                    Ticks = ticks.ToArray();
                    return;
                }

                if (largestTickLabelSize.HasValue)
                {
                    float largestW =
                        Math.Max(tickLabelBound.Width, largestTickLabelSize.Value.Width);
                    float largestH =
                        Math.Max(tickLabelBound.Height, largestTickLabelSize.Value.Height);

                    tickLabelBound.Set(largestW, largestH);
                    continue;
                }

                throw new InvalidOperationException(
                    $"{nameof(ticks)} and {nameof(largestTickLabelSize)} are both null");
            }
        }

        private (IList<Tick> ticks, PanelSize? labelSize)
            GenerateDateTimeTicks(PixelRange range, Edge direction, float axisLength,
            PanelSize tickLabelBound, LabelStyle tickLabelStyle)
        {
            float labelLength = direction.Vertical()
                ? tickLabelBound.Height : tickLabelBound.Width;
            int targetTickCount = (int)(axisLength / labelLength);

            TimeSpan span = TimeSpan.FromDays(range.Span);

            (ITimeUnit niceTimeUnit, int niceIncrement)
                = GetAppropriateTimeUnit(span, targetTickCount);

            double min = Math.Max(range.Low, DateTime.MinValue.ToOADate());
            double max = Math.Min(range.High, DateTime.MaxValue.ToOADate());

            DateTime minDT = DateTime.FromOADate(min);
            DateTime maxDt = DateTime.FromOADate(max);

            IList<Tick> ticks = new List<Tick>();
            for (DateTime dt = minDT; dt <= maxDt; dt = niceTimeUnit.Next(dt, niceIncrement))
            {
                string tickLabel = dt.ToString(niceTimeUnit.GetFormatString());

                PanelSize tickLabelSize = tickLabelStyle.Measure(tickLabel);

                if (!tickLabelBound.Contains(tickLabelSize))
                    return (null, tickLabelSize);

                double tickPosition = dt.ToOADate();
                ticks.Add(Tick.Major(tickPosition, tickLabel));
            }

            return (ticks, null);
        }

        private (ITimeUnit timeUnit, int increment)
            GetAppropriateTimeUnit(TimeSpan timeSpan, int targetTickCount = 10)
        {
            foreach (ITimeUnit timeUnit in m_theseTimeUnits)
            {
                long totalCount = timeSpan.Ticks / timeUnit.MinSize.Ticks;
                foreach (int increment in timeUnit.Divisors)
                {
                    long estimatedTickCount = totalCount / increment;
                    if (estimatedTickCount < targetTickCount)
                        return (timeUnit, increment);
                }
            }

            return (m_theseTimeUnits.Last(), m_theseTimeUnits.Last().Divisors[0]);
        }
    }
}
