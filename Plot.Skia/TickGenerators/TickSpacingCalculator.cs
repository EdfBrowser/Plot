using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    internal static class TickSpacingCalculator
    {
        private static readonly IReadOnlyList<ITimeUnit> m_theseTimeUnits
           = new List<ITimeUnit>() {
               new SecondTimeUnit(),
               new MinuteTimeUnit(),
               new HourTimeUnit(),
               new DayTimeUnit(),
               new MonthTimeUnit(),
               new YearTimeUnit(),
           };

        internal static IEnumerable<double> GenerateNumericTickPositions(Range range, float axisLength, float labelWidth,
            int radix = 10)
        {
            double idealSpacing = GetIdealTickSpacing(range, axisLength, labelWidth, radix);

            // now  that tick-spacing is known, start to generate list of ticks
            double firstTickOffset = range.Low % idealSpacing;
            int tickCount = (int)(range.Span / idealSpacing) + 2;
            tickCount = tickCount > 1000 ? 1000 : tickCount;
            tickCount = tickCount < 1 ? 1 : tickCount;

            List<double> tickPositionsMajor = Enumerable.Range(0, tickCount)
                                            .Select(x => range.Low + x * idealSpacing - firstTickOffset)
                                            .Where(range.Contains)
                                            .ToList();

            if (tickPositionsMajor.Count < 2)
            {
                double tickBelow = range.Low - firstTickOffset;
                double firstTick = tickPositionsMajor.Count > 0 ? tickPositionsMajor[0] : tickBelow;
                double nextTick = tickBelow + idealSpacing;
                tickPositionsMajor = new List<double>() { firstTick, nextTick };
            }

            return tickPositionsMajor;
        }

        internal static (ITimeUnit, IEnumerable<DateTime>)
            GenerateDateTimeTickPositions(Range range, float axisLength, float labelWidth)
        {
            int targetTickCount = (int)(axisLength / labelWidth);

            TimeSpan ts = TimeSpan.FromDays(range.Span);
            (ITimeUnit niceTimeUnit, int niceIncrement)
                = GetAppropriateTimeUnit(ts, targetTickCount);


            double min = Math.Max(range.Low, DateTime.MinValue.ToOADate());
            double max = Math.Min(range.High, DateTime.MaxValue.ToOADate());

            DateTime minDT = DateTime.FromOADate(min);
            DateTime maxDT = DateTime.FromOADate(max);

            int tickCount = niceTimeUnit.GetTickCount(minDT, maxDT, niceIncrement);

            IEnumerable<DateTime> tickPositions = Enumerable.Range(0, tickCount)
                .Select(x => niceTimeUnit.GetTick(minDT, x, niceIncrement))
                .Where(x => range.Contains(x.ToOADate()));

            return (niceTimeUnit, tickPositions);
        }

        private static double GetIdealTickSpacing(Range range, float axisLength, float labelWidth,
            int radix = 10)
        {
            int targetTickCount = (int)(axisLength / labelWidth);
            int exponent = (int)Math.Log(range.Span, radix);
            List<double> tickSpacings = new List<double>() { Math.Pow(radix, exponent) };


            double[] divBy;
            if (radix == 10)
                divBy = new double[] { 2, 2, 2.5 }; // 10,5,2.5,1
            else
                throw new NotImplementedException($"Unsupport the radix: {radix}");

            int divisions = 0;
            int tickCount = 0;
            while (tickCount < targetTickCount)
            {
                tickSpacings.Add(tickSpacings.Last() / divBy[divisions++ % divBy.Length]);
                tickCount = (int)(range.Span / tickSpacings.Last());
            }

            // choose the densest tick spacing that is still good
            for (int i = 0; i < tickSpacings.Count; i++)
            {
                double thisTickSpacing = tickSpacings[tickSpacings.Count - 1 - i];
                double thisTickCount = range.Span / thisTickSpacing;
                double spacePerTick = axisLength / thisTickCount;
                double neededSpacePerTick = labelWidth;

                // add more space between small labels
                if (neededSpacePerTick < 10)
                    neededSpacePerTick *= 2;
                else if (neededSpacePerTick < 25)
                    neededSpacePerTick *= 1.5;
                else
                    neededSpacePerTick *= 1.2;

                if (spacePerTick > neededSpacePerTick)
                {
                    return thisTickSpacing;
                }
            }

            return tickSpacings[0];
        }


        private static (ITimeUnit, int) GetAppropriateTimeUnit(
            TimeSpan ts, int targetTickCount)
        {
            long totalTicks = ts.Ticks;
            foreach (ITimeUnit timeUnit in m_theseTimeUnits)
            {
                long unitSizeTicks = timeUnit.MinSize.Ticks;
                foreach (int increment in timeUnit.Divisors)
                {
                    long tickCount = totalTicks / (unitSizeTicks * increment);
                    if (tickCount <= targetTickCount)
                        return (timeUnit, increment);
                }
            }

            ITimeUnit lastUnit = m_theseTimeUnits.Last();
            return (lastUnit, lastUnit.Divisors.Last());
        }
    }
}
