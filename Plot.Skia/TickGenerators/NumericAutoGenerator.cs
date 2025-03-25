using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    internal class NumericAutoGenerator : BaseTickGenerator, ITickGenerator
    {
        public Tick[] Ticks { get; private set; }

        public void Generate(Range range, Edge direction, float axisLength, LabelStyle tickLabelStyle)
        {
            Ticks = GenerateTicks(range, direction, axisLength, 12f, tickLabelStyle)
                 .ToArray();
        }

        private IEnumerable<Tick> GenerateTicks(Range range, Edge direction, float axisLength,
            float labelLength, LabelStyle tickLabelStyle)
        {
            float labelWidth = Math.Max(0, labelLength);

            IEnumerable<double> tickPositions
                = GenerateNumericTickPositions(range, axisLength, labelWidth);
            IEnumerable<string> tickLabels = tickPositions
                .Select(GetPositionLabel);

            (string largestText, float actualMaxLength) = direction.Vertical()
                ? MeasureString(tickLabels, x => tickLabelStyle.Measure(x).Height)
                : MeasureString(tickLabels, x => tickLabelStyle.Measure(x).Width);

            return actualMaxLength > labelLength
                ? GenerateTicks(range, direction, axisLength, actualMaxLength, tickLabelStyle)
                : GenerateFinalTicks(tickPositions, tickLabels, range);
        }

        protected override string GetPositionLabel(double value)
        {
            // if the number is round or large, use the numeric format
            //bool isRounded = (int)value == value;
            //bool isLargeted = Math.Abs(value) > 1_000_000;
            //if (isRounded || isLargeted)
            //    return value.ToString("N0");

            // otherwise the number is probably small or very precise to use the general format (with slight rounding)
            return Math.Round(value, 10).ToString("G");
        }
    }
}
