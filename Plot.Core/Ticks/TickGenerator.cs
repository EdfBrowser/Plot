using Plot.Core.Draws;
using Plot.Core.Enum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace Plot.Core.Ticks
{
    public class TickGenerator
    {
        private TickCollection m_tickCollection = TickCollection.Empty;
        private TickCollection m_manualTick = null;

        public bool IsVertical { get; set; } = true;

        public float TickDensity { get; set; } = 1f;
        public int Radix { get; set; } = 10;
        public CultureInfo Culture { get; set; } = CultureInfo.DefaultThreadCurrentCulture;
        public SizeF LargestLabelSize { get; private set; }
        public float LargestLabelWidth => LargestLabelSize.Width;
        public float LargestLabelHeight => LargestLabelSize.Height;

        public TickLabelFormat LabelFormat { get; set; } = TickLabelFormat.Numeric;
        public double? MajorDiv { get; set; }
        public int? MinorDivCount { get; set; }

        public string DateTimeFormatString { get; set; }

        public void Recalculate(PlotDimensions dims, Font tickFont)
        {
            if (m_manualTick != null)
                RecalculateManualTicks(dims, tickFont);
            else
                RecalculateAutomatic(dims, tickFont);
        }

        private void RecalculateManualTicks(PlotDimensions dims, Font tickFont)
        {
            double min = IsVertical ? dims.m_yMin : dims.m_xMin;
            double max = IsVertical ? dims.m_yMax : dims.m_xMax;

            var visibleIndexs = Enumerable.Range(0, m_manualTick.Major.Length)
                                .Where(x => m_manualTick.Major[x] >= min)
                                .Where(x => m_manualTick.Major[x] <= max);
            double[] tickMajor = visibleIndexs.Select(x => m_manualTick.Major[x]).ToArray();
            string[] tickLabels = visibleIndexs.Select(x => m_manualTick.Labels[x]).ToArray();

            m_tickCollection = new TickCollection(tickMajor, null, tickLabels);

            LargestLabelSize = GetMaxLabelSize(tickFont);
        }

        public void SetManualTick(double[] positions, string[] labels)
            => m_manualTick = new TickCollection(positions, null, labels);

        // TODO: 策略模式？
        private void RecalculateAutomatic(PlotDimensions dims, Font tickFont)
        {
            int initialTickCount = (int)(10 * TickDensity);
            if (LabelFormat == TickLabelFormat.DateTime)
            {
                float labelWidth = 20f;
                float labelHeight = 24f;
                SizeF labelSize = new SizeF(labelWidth, labelHeight);
                RecalculatePositionsDateTime(dims, labelSize, initialTickCount);
            }
            else if (LabelFormat == TickLabelFormat.Numeric)
            {
                float labelWidth = 15f;
                float labelHeight = 12f;
                SizeF labelSize = new SizeF(labelWidth, labelHeight);

                RecalculatePositionsNumeric(dims, labelSize, initialTickCount);
            }

            // use the results of the first pass to estimate the size of the largest tick label
            LargestLabelSize = GetMaxLabelSize(tickFont);

            // re-calculate position by largest label size
            if (LabelFormat == TickLabelFormat.DateTime)
            {
                RecalculatePositionsDateTime(dims, LargestLabelSize, null);
            }
            else if (LabelFormat == TickLabelFormat.Numeric)
            {
                RecalculatePositionsNumeric(dims, LargestLabelSize, null);
            }
        }

        private void RecalculatePositionsDateTime(PlotDimensions dims, SizeF labelSize, int? initialTickCount)
        {
            double low, high;
            int maxTickCount;

            if (IsVertical)
            {
                low = dims.m_yMin - dims.m_unitsPerPxY;
                high = dims.m_yMax + dims.m_unitsPerPxY;
                maxTickCount = initialTickCount ?? (int)(dims.m_plotHeight / labelSize.Height * TickDensity);
            }
            else
            {
                low = dims.m_xMin - dims.m_unitsPerPxX;
                high = dims.m_xMax + dims.m_unitsPerPxX;
                maxTickCount = initialTickCount ?? (int)(dims.m_plotWidth / labelSize.Width * TickDensity);
            }

            if (low > high) return;

            low = Math.Max(low, DateTime.MinValue.ToOADate());
            high = Math.Min(high, DateTime.MaxValue.ToOADate());

            DateTime from = DateTime.FromOADate(low);
            DateTime to = DateTime.FromOADate(high);

            IDateTimeUnit tickUnit = DateTimeUnitFactory.CreateBestUnit(from, to, Culture, maxTickCount);
            (double[] tickPositionsMajor, string[] tickLabels) = tickUnit.GetTicksAndLabels(from, to, DateTimeFormatString);
            tickLabels = tickLabels.Select(x => x.Trim()).ToArray();

            m_tickCollection = new TickCollection(tickPositionsMajor, null, tickLabels);
        }

        private void RecalculatePositionsNumeric(PlotDimensions dims, SizeF labelSize, int? initialTickCount)
        {
            double low, high, tickSpacing;
            int maxTickCount;

            if (IsVertical)
            {
                low = dims.m_yMin - dims.m_unitsPerPxY; // add a extra pixel to capture the edge tick
                high = dims.m_yMax + dims.m_unitsPerPxY;
                maxTickCount = initialTickCount ?? (int)(dims.m_plotHeight / labelSize.Height * TickDensity);
                tickSpacing = MajorDiv ?? GetIdealTickSpacing(low, high, maxTickCount, Radix);
            }
            else
            {
                low = dims.m_xMin - dims.m_unitsPerPxX; // add a extra pixel to capture the edge tick
                high = dims.m_xMax + dims.m_unitsPerPxX;
                maxTickCount = initialTickCount ?? (int)(dims.m_plotWidth / labelSize.Width * TickDensity);
                tickSpacing = MajorDiv ?? GetIdealTickSpacing(low, high, maxTickCount, Radix);
            }


            // now  that tick-spacing is known, start to generate list of ticks
            double firstTickOffset = low % tickSpacing;
            int tickCount = (int)((high - low) / tickSpacing) + 2;
            tickCount = tickCount > 1000 ? 1000 : tickCount;
            tickCount = tickCount < 1 ? 1 : tickCount;

            double[] tickPositionsMajor = Enumerable.Range(0, tickCount)
                                            .Select(x => low + x * tickSpacing - firstTickOffset)
                                            .Where(x => x >= low && x <= high)
                                            .ToArray();

            if (tickPositionsMajor.Length < 2)
            {
                double tickBelow = low - firstTickOffset;
                double firstTick = tickPositionsMajor.Length > 0 ? tickPositionsMajor[0] : tickBelow;
                double nextTick = tickBelow + tickSpacing;
                tickPositionsMajor = new double[] { firstTick, nextTick };
            }

            string[] labels = GetTicksLabel(tickPositionsMajor);

            double[] tickPositionsMinor = GetMinorPositions(tickPositionsMajor, low, high);

            m_tickCollection = new TickCollection(tickPositionsMajor, tickPositionsMinor, labels);
        }

        private double[] GetMinorPositions(double[] majorTicks, double min, double max)
        {
            int divCount = MinorDivCount ?? 4;
            divCount += 1;

            if (majorTicks is null || majorTicks.Length < 2)
                return Array.Empty<double>();

            double majorTickSpacing = majorTicks[1] - majorTicks[0];
            double minorTickSpacing = majorTickSpacing / divCount;

            List<double> minorTicks = new List<double>();
            foreach (var majorTickPosition in majorTicks)
            {
                for (int i = 1; i < divCount; i++)
                {
                    double minorTickPosition = majorTickPosition + minorTickSpacing * i;
                    if ((minorTickPosition > min) && (minorTickPosition < max))
                        minorTicks.Add(minorTickPosition);
                }
            }

            return minorTicks.ToArray();
        }

        private string[] GetTicksLabel(double[] positions)
        {
            if (positions.Length == 0)
                return Array.Empty<string>();

            string[] labels = new string[positions.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                labels[i] = GetNumericLabel(positions[i]);

                //if (labels[i] == "-0")
                //    labels[i] = "0";
            }


            return labels;
        }

        private string GetNumericLabel(double value)
        {
            // if the number is round or large, use the numeric format
            bool isRounded = (int)value == value;
            bool isLargeted = Math.Abs(value) > 1000;
            if (isRounded || isLargeted)
                return value.ToString("N0", Culture);

            // otherwise the number is probably small or very precise to use the general format (with slight rounding)
            return Math.Round(value, 3).ToString("G", Culture);
        }

        private double GetIdealTickSpacing(double low, double high, int maxTickCount, int radix = 10)
        {
            double range = high - low;
            int exponent = (int)Math.Log(range, radix);
            // 初始化三个值
            List<double> tickSpacings = new List<double>() { Math.Pow(radix, exponent) };
            tickSpacings.Add(tickSpacings.Last());
            tickSpacings.Add(tickSpacings.Last());


            double[] divBy;
            if (radix == 10)
            {
                divBy = new double[] { 2, 2, 2.5 }; // 10,5,2.5,1
            }
            else
            {
                throw new NotImplementedException($"Unsupport the radix: {radix}");
            }

            int divisions = 0;
            int tickCount = 0;
            while ((tickCount < maxTickCount) && (tickSpacings.Count < 1000))
            {
                tickSpacings.Add(tickSpacings.Last() / divBy[divisions++ % divBy.Length]);
                tickCount = (int)(range / tickSpacings.Last());
            }

            int startIndex = 3;
            // 
            double maxSpacing = range / 2.0;
            double idealSpacing = tickSpacings[tickSpacings.Count - startIndex];

            while (idealSpacing > maxSpacing && startIndex >= 1)
            {
                idealSpacing = tickSpacings[tickSpacings.Count - startIndex];
                startIndex--;
            }

            return idealSpacing;
        }

        private SizeF GetMaxLabelSize(Font tickFont)
        {
            if (m_tickCollection.Labels is null || m_tickCollection.Labels.Length == 0)
                return new SizeF(0, 0);

            string largestString = "00";
            foreach (string s in m_tickCollection.Labels.Where(x => string.IsNullOrEmpty(x) == false))
                if (s.Length > largestString.Length)
                    largestString = s;

            return GDI.MeasureStringUsingTemporaryGraphics(largestString, tickFont);
        }

        private Tick[] GetMajorTicks()
        {
            if (m_tickCollection.Major is null || m_tickCollection.Major.Length == 0)
                return Array.Empty<Tick>();

            Tick[] ticks = new Tick[m_tickCollection.Major.Length];

            for (int i = 0; i < m_tickCollection.Major.Length; i++)
            {
                ticks[i] = new Tick(
                    position: m_tickCollection.Major[i],
                    label: m_tickCollection.Labels[i],
                    isMajor: true,
                    isDateTime: LabelFormat == TickLabelFormat.DateTime);

            }

            return ticks;
        }

        private Tick[] GetMinorTicks()
        {
            if (m_tickCollection.Minor is null || m_tickCollection.Minor.Length == 0)
                return Array.Empty<Tick>();

            Tick[] ticks = new Tick[m_tickCollection.Minor.Length];
            for (int i = 0; i < ticks.Length; i++)
            {
                ticks[i] = new Tick(
                    position: m_tickCollection.Minor[i],
                    label: null,
                    isMajor: false,
                    isDateTime: LabelFormat == TickLabelFormat.DateTime);
            }

            return ticks;
        }

        public Tick[] GetVisibleMajorTicks(PlotDimensions dims)
        {
            double high, low;

            if (IsVertical)
            {
                low = dims.m_yMin - dims.m_unitsPerPxY; // add an extra pixel to capture the edge tick
                high = dims.m_yMax + dims.m_unitsPerPxY; // add an extra pixel to capture the edge tick
            }
            else
            {
                low = dims.m_xMin - dims.m_pxPerUnitX; // add an extra pixel to capture the edge tick
                high = dims.m_xMax + dims.m_pxPerUnitX; // add an extra pixel to capture the edge tick
            }

            return GetMajorTicks()
                .Where(x => x.m_position >= low && x.m_position <= high).OrderBy(x => x.m_position)
                .ToArray();
        }

        public Tick[] GetVisibleMinorTicks(PlotDimensions dims)
        {
            double high, low;
            if (IsVertical)
            {
                low = dims.m_yMin - dims.m_unitsPerPxY; // add an extra pixel to capture the edge tick
                high = dims.m_yMax + dims.m_unitsPerPxY; // add an extra pixel to capture the edge tick
            }
            else
            {
                low = dims.m_xMin - dims.m_pxPerUnitX; // add an extra pixel to capture the edge tick
                high = dims.m_xMax + dims.m_pxPerUnitX; // add an extra pixel to capture the edge tick
            }

            return GetMinorTicks()
                .Where(t => t.m_position >= low && t.m_position <= high)
                .ToArray();
        }
    }
}
