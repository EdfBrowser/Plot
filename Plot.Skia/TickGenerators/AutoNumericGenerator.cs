using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Plot.Skia
{
    internal class AutoNumericGenerator : BaseTickGenerator, ITickGenerator
    {
        public IEnumerable<Tick> Ticks { get; private set; }
        public Func<double, string> FormatLabel { get; set; }

        public void Generate(Range range, Edge direction, float axisLength, LabelStyle tickLabelStyle)
        {
            GenerateTicks(range, direction, axisLength, 12f, tickLabelStyle);
        }

        private void GenerateTicks(Range range, Edge direction, float axisLength,
            float labelLength, LabelStyle tickLabelStyle)
        {
            float currentLabelLength = labelLength;
            float maxSize = float.NegativeInfinity;
            string maxText = string.Empty;

            List<double> majorPositionsList;
            IEnumerable<string> majorLabels;
            do
            {
                IEnumerable<double> majorPositions
                    = GenerateNumericTickPositions(range, axisLength, currentLabelLength);
                majorPositionsList = majorPositions.ToList();

                majorLabels = MeasuredLabels(majorPositionsList, direction, tickLabelStyle,
                    ref maxText, ref maxSize);

                // 使用预给出的labelLength值重新分配tick
                if (currentLabelLength < maxSize)
                    currentLabelLength = maxSize;
                else
                    break;

            } while (true);

            IEnumerable<double> minorPositions
                = GenerateMinorPositions(majorPositionsList, range);

            List<string> majorLabelsList = majorLabels.ToList();
            List<double> minorPositionsList = minorPositions.ToList();

            Ticks = CombineTicks(majorPositionsList, majorLabelsList, minorPositionsList);
        }

        private IEnumerable<Tick> CombineTicks(
            IReadOnlyList<double> majorPositions, IReadOnlyList<string> majorLabels,
            IReadOnlyList<double> minorPositions)
        {
            // 主刻度
            for (int i = 0; i < majorPositions.Count; i++)
            {
                yield return Tick.Major(majorPositions[i], majorLabels[i]);
            }

            // 次刻度
            foreach (double pos in minorPositions)
            {
                yield return Tick.Minor(pos);
            }
        }

        private IEnumerable<double> GenerateMinorPositions(
            IReadOnlyList<double> majorTicks, Range range)
        {
            if (majorTicks.Count < 2) yield break;

            double majorSpace = majorTicks[1] - majorTicks[0];
            double minorSpace = majorSpace / 5;

            // 生成主刻度之前的次刻度
            for (double majorPos = majorTicks[0] - majorSpace; majorPos >= range.Low; majorPos -= majorSpace)
            {
                foreach (double minorPos in GenerateMinorsForMajor(majorPos, minorSpace, range))
                {
                    yield return minorPos;
                }
            }

            // 生成所有主刻度之间的次刻度
            foreach (double majorPos in majorTicks)
            {
                foreach (double minorPos in GenerateMinorsForMajor(majorPos, minorSpace, range))
                {
                    yield return minorPos;
                }
            }
        }


        private IEnumerable<double> GenerateMinorsForMajor(
            double majorPos, double minorSpacing, Range range)
        {
            for (int i = 1; i < MinorCount; i++)
            {
                double pos = majorPos + minorSpacing * i;
                if (pos > range.High) yield break;
                if (pos >= range.Low) yield return pos;
            }
        }


        private IEnumerable<string> MeasuredLabels(IReadOnlyList<double> positions,
            Edge direction, LabelStyle style,
            ref string maxText, ref float maxSize)
        {
            bool vertical = direction.Vertical();

            IEnumerable<string> labels = FormatLabels(positions);
            foreach (string label in labels)
            {
                SizeF measuredValue = style.Measure(label, force: true);
                float size;
                if (vertical)
                    size = measuredValue.Height;
                else
                    size = measuredValue.Width;

                if (size > maxSize)
                {
                    maxSize = size;
                    maxText = label;
                }
            }

            return labels;
        }

        private IEnumerable<string> FormatLabels(IReadOnlyList<double> positions)
        {
            foreach (double pos in positions)
            {
                yield return GetPositionLabel(pos);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override string GetPositionLabel(double value)
        {
            if (FormatLabel != null)
                return FormatLabel.Invoke(value);

            return value.ToString();
        }
    }
}
