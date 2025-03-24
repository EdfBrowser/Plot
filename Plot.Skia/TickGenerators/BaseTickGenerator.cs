using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    internal abstract class BaseTickGenerator
    {
        private const double m_year = 365.25;
        private const double m_month = 30.5;
        private const double m_day = 1.0;
        private const double m_hour = m_day / 24.0;
        private const double m_minute = m_hour / 60.0;
        private const double m_second = m_minute / 60.0;

        private static readonly Dictionary<string, double> m_timeUnits
            = new Dictionary<string, double>
            {
                { "second", m_second },
                { "minute", m_minute },
                { "hour", m_hour },
                { "day", m_day },
                { "month", m_month },
                { "year", m_year }
            };

        private static readonly Dictionary<string, int[]> m_multiples
            = new Dictionary<string, int[]>
            {
                { "second", new int[] { 1, 2, 5, 10, 30, 60 } },
                { "minute", new int[] { 1, 2, 5, 10, 30, 60 } },
                { "hour", new int[] { 1, 2, 4, 8, 12 } },
                { "day", new int[] { 1, 2, 5, 10, 30 } },
                { "month", new int[] { 1, 2, 4, 8, 12 } },
                { "year", new int[] { 1 } }
            };

        protected BaseTickGenerator()
        {
            MinorCount = 5;
        }

        protected int MinorCount { get; set; }

        protected abstract string GetPositionLabel(double value);

        protected (string largestText, float actualMaxLength)
          MeasureString(IEnumerable<string> tickLabels, Func<string, float> getSize)
        {
            float maxSize = 0;
            string maxText = string.Empty;

            foreach (string tickLabel in tickLabels)
            {
                float size = getSize(tickLabel);
                if (size > maxSize)
                {
                    maxSize = size;
                    maxText = tickLabel;
                }
            }

            return (maxText, maxSize);
        }

        protected IEnumerable<Tick> GenerateFinalTicks(IEnumerable<double> positions,
          IEnumerable<string> tickLabels, Range range)
        {
            IEnumerable<Tick> majorTicks = positions
                .Select((p, i) => Tick.Major(p, tickLabels.ElementAt(i)));

            IEnumerable<Tick> minorTicks = GetMinorPositions(positions, range)
                .Select(Tick.Minor);

            return majorTicks.Concat(minorTicks);
        }

        protected IEnumerable<double> GetMinorPositions(
            IEnumerable<double> majorTicks, Range range)
        {
            if (majorTicks is null || majorTicks.Count() < 2)
                return Array.Empty<double>();

            List<double> minorPositions = new List<double>();

            double firstPos = majorTicks.ElementAt(0);
            double secondPos = majorTicks.ElementAt(1);

            double majorTickSpacing = secondPos - firstPos;
            double minorTickSpacing = majorTickSpacing / MinorCount;

            // 如果第一个刻度的起点不在“0”，需要生成第一个刻度前面空白区域的刻度
            double previous = firstPos - majorTickSpacing;
            List<double> niceMajorPositions = new List<double>() { previous };
            niceMajorPositions.AddRange(majorTicks);

            foreach (var majorPosition in niceMajorPositions)
            {
                for (int i = 1; i < MinorCount; i++)
                {
                    double minorPosition = majorPosition + minorTickSpacing * i;
                    if (range.Contains(minorPosition))
                        minorPositions.Add(minorPosition);
                }
            }

            return minorPositions;
        }

        protected IEnumerable<double> GenerateNumericTickPositions(Range range,
            float axisLength, float labelWidth, int radix = 10)
        {
            double idealSpacing = GetIdealTickSpacing(range, axisLength, labelWidth, radix);

            // now  that tick-spacing is known, start to generate list of ticks
            double firstTickOffset = range.Low % idealSpacing;
            int tickCount = (int)(range.Span / idealSpacing) + 1;
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

        internal static IEnumerable<double> GenerateDateTimeTickPositions(
            Range range, float axisLength, float labelWidth)
        {
            int targetTickCount = (int)(axisLength / labelWidth);

            // TODO: 仿造数字刻度生成来修改完善
            IEnumerable<double> intervals = GenerateIntervals();
            double span = range.Span;
            double increment = intervals.Last();
            foreach (double interval in intervals)
            {
                int count = (int)(span / interval);
                if (count <= targetTickCount)
                {
                    increment = interval;
                    break;
                }
            }

            double firstTickOffset = range.Low % increment;
            int tickCount = (int)(span / increment) + 2;
            //tickCount = tickCount < 1 ? 1 : tickCount;

            //Debug.WriteLine($"tick/span: {tickCount}/{span}");


            IEnumerable<double> tickPositions = Enumerable
                .Range(0, tickCount)
                .Select(x => x * increment + range.Low - firstTickOffset)
                .Where(range.Contains);

            return tickPositions;
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


        private static IEnumerable<double> GenerateIntervals()
        {
            IList<double> intervals = new List<double>();

            foreach (var unit in m_timeUnits)
            {
                foreach (var multiple in m_multiples[unit.Key])
                {
                    intervals.Add(unit.Value * multiple);
                }
            }

            return intervals;
        }
    }
}
