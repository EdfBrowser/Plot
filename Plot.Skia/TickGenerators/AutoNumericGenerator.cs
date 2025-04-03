using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Plot.Skia
{
    internal class AutoNumericGenerator : BaseTickGenerator, ITickGenerator
    {
        private readonly double[] _divBy10 = new[] { 2.0, 2.0, 2.5 }; // 静态预定义除数

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

            IEnumerable<double> majorPositions;
            IEnumerable<string> majorLabels;
            do
            {
                majorPositions = GenerateNumericTickPositions(range, axisLength, currentLabelLength);

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
        {
            if (FormatLabel != null)
                return FormatLabel.Invoke(value);

            return value.ToString();
        }

        private IEnumerable<double> GenerateNumericTickPositions(Range range,
            float axisLength, float labelWidth)
        {
            double idealSpace = GetIdealTickSpace(range, axisLength, labelWidth);
            double firstTick = (range.Low / idealSpace) * idealSpace;

            for (double pos = firstTick; pos <= range.High; pos += idealSpace)
            {
                yield return pos;
            }
        }

        private double GetIdealTickSpace(Range range, float axisLength, float labelWidth)
        {
            // 通过像素来计算个数
            int targetTickCount = Math.Max(1, (int)(axisLength / labelWidth));
            // 通过实际范围来计算个数
            double rangeSpan = range.Span;
            int exponent = (int)Math.Log(range.Span, 10);
            double initialSpace = Math.Pow(10, exponent);
            double neededSpace = CalculateNeededSpace(labelWidth);

            IEnumerable<double> candidates
                = GenerateSpaceCandidates(initialSpace, rangeSpan, targetTickCount).Reverse();

            foreach (double space in candidates)
            {
                double tickCount = rangeSpan / space;
                double spacePerTick = axisLength / tickCount;

                if (spacePerTick >= neededSpace)
                    return space;
            }

            return initialSpace;
        }

        private IEnumerable<double> GenerateSpaceCandidates(
            double initialSpace, double rangeSpan, int targetTickCount)
        {
            double current = initialSpace;
            int divIndex = 0;

            yield return current;

            while (rangeSpan / current < targetTickCount)
            {
                current /= _divBy10[divIndex % _divBy10.Length];
                divIndex++;
                yield return current;
            }
        }

        private double CalculateNeededSpace(float labelWidth)
        {
            if (labelWidth < 10) return labelWidth * 2;
            if (labelWidth < 25) return labelWidth * 1.5;
            return labelWidth * 1.2;
        }
    }
}
