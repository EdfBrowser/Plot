using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    internal class DateTimeAutoGenerator : ITickGenerator
    {
        internal DateTime? OriginDateTime { get; set; }

        public Tick[] Ticks { get; private set; }

        public void Generate(Range range, Edge direction, float axisLength, LabelStyle tickLabelStyle)
        {
            Ticks = GenerateTicks(range, direction, axisLength, 12f, tickLabelStyle)
                 .ToArray();
        }

        private IEnumerable<Tick> GenerateTicks(Range range, Edge direction,
            float axisLength, float labelLength, LabelStyle tickLabelStyle)
        {
            float labelWidth = Math.Max(0, labelLength);

            DateTime minDT = OriginDateTime ?? new DateTime(1899, 12, 31, 0, 0, 0);

            (ITimeUnit timeUnit, IEnumerable<double> tickPositions)
                = TickSpacingCalculator.GenerateDateTimeTickPositions(range, axisLength, labelWidth);

            string[] tickLabels = tickPositions
               .Select(x => GetDateTimeLabel(x, minDT, timeUnit))
               .ToArray();

            (string largestText, float actualMaxLength) = direction.Vertical()
                ? MeasureHighestString(tickLabels, tickLabelStyle)
                : MeasureWidestString(tickLabels, tickLabelStyle);

            return actualMaxLength > labelLength
                ? GenerateTicks(range, direction, axisLength, actualMaxLength, tickLabelStyle)
                : GenerateFinalTicks(tickPositions.ToArray(), tickLabels, range);
        }

        private IEnumerable<Tick> GenerateFinalTicks(double[] positions, string[] tickLabels, Range range)
        {
            IEnumerable<Tick> majorTicks = positions
                .Select((p, i) => Tick.Major(p, tickLabels[i]));

            return majorTicks;
        }

        private string GetDateTimeLabel(double value, DateTime dt, ITimeUnit timeUnit)
        {
            int increment = (int)(value / timeUnit.MinSize.TotalSeconds);
            string fm = timeUnit.GetFormatString();
            return timeUnit.Next(dt, increment).ToString(fm);
        }

        private (string largestText, float actualMaxLength)
          MeasureHighestString(string[] tickLabels, LabelStyle labelStyle)
        {
            float maxHeight = 0;
            string maxText = string.Empty;

            for (int i = 0; i < tickLabels.Length; i++)
            {
                float size = labelStyle.Measure(tickLabels[i]).Height;
                if (size > maxHeight)
                {
                    maxHeight = size;
                    maxText = tickLabels[i];
                }
            }

            return (maxText, maxHeight);
        }

        private (string largestText, float actualMaxLength)
            MeasureWidestString(string[] tickLabels, LabelStyle labelStyle)
        {
            float maxWidth = 0;
            string maxText = string.Empty;

            for (int i = 0; i < tickLabels.Length; i++)
            {
                float size = labelStyle.Measure(tickLabels[i]).Width;
                if (size > maxWidth)
                {
                    maxWidth = size;
                    maxText = tickLabels[i];
                }
            }

            return (maxText, maxWidth);
        }
    }
}
