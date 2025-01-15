using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    internal class NumericAutoGenerator : ITickGenerator
    {
        public NumericAutoGenerator()
        {
            MinorDivCount = 5;
        }

        public Tick[] Ticks { get; private set; }
        internal int MinorDivCount { get; set; }

        public void Generate(Range range, Edge direction, float axisLength, LabelStyle tickLabelStyle)
        {
            Ticks = GenerateTicks(range, direction, axisLength, 12f, tickLabelStyle)
                 .Where(x => range.Contains(x.Position))
                 .ToArray();
        }

        private IEnumerable<Tick> GenerateTicks(Range range, Edge direction, float axisLength,
            float labelLength, LabelStyle tickLabelStyle)
        {
            float labelWidth = Math.Max(0, labelLength);

            double[] majorTickPositions = TickSpacingCalculator
                .GenerateTickPositions(range, axisLength, labelWidth)
                .ToArray();
            string[] majorTickLabels = majorTickPositions
                .Select(GetNumericLabel)
                .ToArray();


            (string largestText, float actualMaxLength) = direction.Vertical()
                ? MeasureHighestString(majorTickLabels, tickLabelStyle)
                : MeasureWidestString(majorTickLabels, tickLabelStyle);

            return actualMaxLength > labelLength
                ? GenerateTicks(range, direction, axisLength, actualMaxLength, tickLabelStyle)
                : GenerateFinalTicks(majorTickPositions, majorTickLabels, range);
        }

        private IEnumerable<Tick> GenerateFinalTicks(double[] positions, string[] tickLabels, Range range)
        {
            IEnumerable<Tick> majorTicks = positions
                .Select((p, i) => Tick.Major(p, tickLabels[i]));

            IEnumerable<Tick> minorTicks = GetMinorPositions(positions, range)
                .Select(Tick.Minor);

            return majorTicks.Concat(minorTicks);
        }

        private IEnumerable<double> GetMinorPositions(double[] majorTicks, Range range)
        {
            if (majorTicks is null || majorTicks.Length < 2)
                return Array.Empty<double>();

            double majorTickSpacing = majorTicks[1] - majorTicks[0];
            double minorTickSpacing = majorTickSpacing / MinorDivCount;

            List<double> majorTicksOffsetOne = new List<double>() { majorTicks[0] - majorTickSpacing };
            majorTicksOffsetOne.AddRange(majorTicks);

            List<double> minorTicks = new List<double>();

            foreach (var majorTickPosition in majorTicksOffsetOne)
            {
                for (int i = 1; i < MinorDivCount; i++)
                {
                    double minorTickPosition = majorTickPosition + minorTickSpacing * i;
                    if (range.Contains(minorTickPosition))
                        minorTicks.Add(minorTickPosition);
                }
            }

            return minorTicks;
        }

        private string GetNumericLabel(double value)
        {
            // if the number is round or large, use the numeric format
            bool isRounded = (int)value == value;
            bool isLargeted = Math.Abs(value) > 1000;
            if (isRounded || isLargeted)
                return value.ToString("N0");

            // otherwise the number is probably small or very precise to use the general format (with slight rounding)
            return Math.Round(value, 3).ToString("G");
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
