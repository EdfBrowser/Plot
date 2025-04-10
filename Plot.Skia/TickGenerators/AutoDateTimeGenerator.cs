using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Plot.Skia
{
    internal class AutoDateTimeGenerator : BaseTickGenerator, ITickGenerator
    {
        private const double m_year = 365.25;
        private const double m_month = 30.5;
        private const double m_day = 1.0;
        private const double m_hour = m_day / 24.0;
        private const double m_minute = m_hour / 60.0;
        private const double m_second = m_minute / 60.0;

        private readonly Dictionary<string, double> m_timeUnits
            = new Dictionary<string, double>
            {
                { "second", m_second },
                { "minute", m_minute },
                { "hour", m_hour },
                { "day", m_day },
                { "month", m_month },
                { "year", m_year }
            };

        private readonly Dictionary<string, int[]> m_multiples
            = new Dictionary<string, int[]>
            {
                { "second", new int[] { 1, 2, 5, 10, 30, 60 } },
                { "minute", new int[] { 1, 2, 5, 10, 30, 60 } },
                { "hour", new int[] { 1, 2, 4, 8, 12 } },
                { "day", new int[] { 1, 2, 5, 10, 30 } },
                { "month", new int[] { 1, 2, 4, 8, 12 } },
                { "year", new int[] { 1 } }
            };

        internal AutoDateTimeGenerator()
        {
            OriginDateTime = new DateTime(1899, 12, 31, 0, 0, 0);
            DateTimeFormat = "yyyy/MM/dd\nHH:mm:ss";
        }

        internal DateTime OriginDateTime { get; set; }
        internal string DateTimeFormat { get; set; }

        public IEnumerable<Tick> Ticks { get; private set; }

        public void Generate(Range range, Edge direction, float axisLength, LabelStyle tickLabelStyle)
        {
            GenerateTicks(range, direction, axisLength, 12f, tickLabelStyle);
        }

        private void GenerateTicks(Range range, Edge direction,
            float axisLength, float labelLength, LabelStyle tickLabelStyle)
        {
            float currentLabelLength = labelLength;
            float maxSize = float.NegativeInfinity;
            string maxText = string.Empty;

            IEnumerable<double> majorPositions;
            IEnumerable<string> majorLabels;
            do
            {
                majorPositions = GenerateDateTimeTickPositions(range, axisLength, currentLabelLength);

                majorLabels = MeasuredLabels(majorPositions, direction, tickLabelStyle,
                    ref maxText, ref maxSize);

                // 使用预给出的labelLength值重新分配tick
                if (currentLabelLength < maxSize)
                    currentLabelLength = maxSize;
                else
                    break;

            } while (true);

            List<double> majorPositionsList = majorPositions.ToList();
            List<double> minorPositionsList
                = GenerateMinorPositions(majorPositionsList, range).ToList();
            List<string> majorLabelsList = majorLabels.ToList();

            Ticks = CombineTicks(majorPositionsList, majorLabelsList, minorPositionsList);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override string GetPositionLabel(double value)
            => OriginDateTime.AddSeconds(value).ToString(DateTimeFormat);

        private IEnumerable<double> GenerateDateTimeTickPositions(
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

            double firstTick = (range.Low / increment) * increment;

            for (double pos = firstTick; pos <= range.High; pos += increment)
            {
                yield return pos;
            }
        }


        private IEnumerable<double> GenerateIntervals()
        {
            foreach (var unit in m_timeUnits)
            {
                foreach (var multiple in m_multiples[unit.Key])
                {
                    yield return unit.Value * multiple;
                }
            }
        }
    }
}
