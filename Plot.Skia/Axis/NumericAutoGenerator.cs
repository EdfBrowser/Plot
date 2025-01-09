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

        public Tick[] Ticks { get; set; }
        internal int MinorDivCount { get; set; }
        public float LargestLabelLength { get; set; }

        public void Generate(PixelRange range, Edge edge, float axisLength, LabelStyle labelStyle)
        {
            Ticks = GenerateTicks(range, edge, axisLength, 12f, labelStyle)
                 .Where(x => range.Contains(x.Position))
                 .ToArray();
        }

        private IEnumerable<Tick> GenerateTicks(PixelRange range, Edge edge, float axisLength,
            float largestLabelLength, LabelStyle labelStyle)
        {
            float labelWidth = Math.Max(0, largestLabelLength);

            double[] majorTickPositions = TickSpacingCalculator
                .GenerateTickPositions(range, axisLength, labelWidth)
                .ToArray();
            string[] majorTickLabels = majorTickPositions
                .Select(GetNumericLabel)
                .ToArray();


            (string largestText, float actualMaxLength) = edge.Vertical()
                ? labelStyle.MeasureHighestString(majorTickLabels)
                : labelStyle.MeasureWidestString(majorTickLabels);

            LargestLabelLength = actualMaxLength;

            return actualMaxLength > largestLabelLength
                ? GenerateTicks(range, edge, axisLength, actualMaxLength, labelStyle)
                : GenerateFinalTicks(majorTickPositions, majorTickLabels, range);
        }

        private IEnumerable<Tick> GenerateFinalTicks(double[] positions, string[] tickLabels, PixelRange range)
        {
            IEnumerable<Tick> majorTicks = positions
                .Select((p, i) => Tick.Major(p, tickLabels[i]));

            IEnumerable<Tick> minorTicks = GetMinorPositions(positions, range)
                .Select(Tick.Minor);

            return majorTicks.Concat(minorTicks);
        }

        private IEnumerable<double> GetMinorPositions(double[] majorTicks, PixelRange range)
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
    }
}
