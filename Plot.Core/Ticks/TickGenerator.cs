using Plot.Core.Draws;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace Plot.Core.Ticks
{
    public enum TickLabelFormat { Numeric, DateTime };
    public class TickGenerator
    {

        private TickCollection m_tickCollection = TickCollection.Empty;

        public bool IsVertical { get; set; } = true;

        public float TickDensity { get; set; } = 1f;
        public int Radix { get; set; } = 10;
        public bool IntegerPositionsOnly { get; set; } = false;
        public CultureInfo Culture { get; set; } = CultureInfo.DefaultThreadCurrentCulture;
        public SizeF LargestLabelSize { get; private set; }
        public float LargestLabelWidth => LargestLabelSize.Width;
        public float LargestLabelHeight => LargestLabelSize.Height;

        public TickLabelFormat LabelFormat { get; set; } = TickLabelFormat.Numeric;

        public void ReCalculate(PlotDimensions dims, Font tickFont)
        {
            int initialTickCount = (int)(10 * TickDensity);

            // 先给定label的大小
            {
                float labelWidth = 15;
                float labelHeight = 12;
                SizeF labelSize = new SizeF(labelWidth, labelHeight);

                ReCalculatePositionsNumeric(dims, labelSize, initialTickCount);
            }

            // use the results of the first pass to estimate the size of the largest tick label
            LargestLabelSize = GetMaxLabelSize(tickFont);

            // re-calculate position by largest label size

            ReCalculatePositionsNumeric(dims, LargestLabelSize, null);
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

        private void ReCalculatePositionsNumeric(PlotDimensions dims, SizeF labelSize, int? initialTickCount)
        {
            float low, high, tickSpacing;
            int maxTickCount;

            if (IsVertical)
            {
                low = dims.m_yMin - dims.m_pxPerUnitY; // add a extra pixel to capture the edge tick
                high = dims.m_yMax + dims.m_pxPerUnitY;
                maxTickCount = initialTickCount.HasValue ? initialTickCount.Value : (int)(dims.m_plotHeight / labelSize.Height * TickDensity);
                tickSpacing = (float)GetIdealTickSpacing(low, high, maxTickCount, Radix);
            }
            else
            {
                low = dims.m_xMin - dims.m_pxPerUnitX; // add a extra pixel to capture the edge tick
                high = dims.m_xMax + dims.m_pxPerUnitX;
                maxTickCount = initialTickCount.HasValue ? initialTickCount.Value : (int)(dims.m_plotWidth / labelSize.Width * TickDensity);
                tickSpacing = (float)GetIdealTickSpacing(low, high, maxTickCount, Radix);
            }


            // now  that tick-spacing is known, start to generate list of ticks
            float firstTickOffset = low % tickSpacing;
            int tickCount = (int)((high - low) / tickSpacing) + 2;
            tickCount = tickCount > 1000 ? 1000 : tickCount;
            tickCount = tickCount < 1 ? 1 : tickCount;

            float[] tickPositionsMajor = Enumerable.Range(0, tickCount)
                .Select(x => low + x * tickSpacing - firstTickOffset)
                .Where(x => x >= low && x <= high)
                .ToArray();

            if (tickPositionsMajor.Length < 2)
            {
                float tickBelow = low - firstTickOffset;
                float firstTick = tickPositionsMajor.Length > 0 ? tickPositionsMajor[0] : tickBelow;
                float nextTick = tickBelow + tickSpacing;
                tickPositionsMajor = new float[] { firstTick, nextTick };
            }

            if (IntegerPositionsOnly)
            {
                int firstTick = (int)tickPositionsMajor[0];
                tickPositionsMajor = tickPositionsMajor.Where(x => x == (int)x).Distinct().ToArray();

                if (tickPositionsMajor.Length < 2)
                    tickPositionsMajor = new float[] { firstTick - 1, firstTick, firstTick + 1 };
            }

            string[] labels = GetTicksLabel(tickPositionsMajor);

            float[] tickPositionsMinor = GetMinorPositions(tickPositionsMajor, low, high);

            m_tickCollection = new TickCollection(tickPositionsMajor, tickPositionsMinor, labels);
        }

        public float[] GetMinorPositions(float[] majorTicks, float min, float max)
        {
            int divisions = 5;

            if (majorTicks is null || majorTicks.Length < 2)
                return Array.Empty<float>();

            float majorTickSpacing = majorTicks[1] - majorTicks[0];
            float minorTickSpacing = majorTickSpacing / divisions;

            List<float> majorTicksWithPadding = new List<float>();
            majorTicksWithPadding.Add(majorTicks[0] - majorTickSpacing);
            majorTicksWithPadding.AddRange(majorTicks);

            List<float> minorTicks = new List<float>();
            foreach (var majorTickPosition in majorTicksWithPadding)
            {
                for (int i = 1; i < divisions; i++)
                {
                    float minorTickPosition = majorTickPosition + minorTickSpacing * i;
                    if ((minorTickPosition > min) && (minorTickPosition < max))
                        minorTicks.Add(minorTickPosition);
                }
            }

            return minorTicks.ToArray();
        }

        private string[] GetTicksLabel(float[] positions)
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

        private string GetNumericLabel(float value)
        {
            // if the number is round or large, use the numeric format
            bool isRounded = (int)value == value;
            bool isLargeted = Math.Abs(value) > 1000;
            if (isRounded || isLargeted)
                return value.ToString("N0", Culture);

            // otherwise the number is probably small or very precise to use the general format (with slight rounding)
            return Math.Round(value, 10).ToString("G", Culture);
        }

        private float GetIdealTickSpacing(float low, float high, int maxTickCount, int radix)
        {
            float range = high - low;
            int exponent = (int)Math.Log(range, radix);
            // 初始化三个值
            List<float> tickSpacings = new List<float>() { (float)Math.Pow(radix, exponent) };
            tickSpacings.Add(tickSpacings.Last());
            tickSpacings.Add(tickSpacings.Last());


            float[] divBy;
            if (radix == 10)
            {
                divBy = new float[] { 2, 2, 2.5f }; // 10,5,2.5,1
            }
            else
            {
                throw new NotImplementedException($"Unsupport the radix: {radix}");
            }

            int divisions = 0;
            int tickCount = 0;
            while ((tickCount < maxTickCount) && (tickSpacings.Count < 1000))
            {
                tickSpacings.Add(tickSpacings.Last() / divBy[divisions]);
                tickCount = (int)(range / tickSpacings.Last());
            }

            int startIndex = 3;
            // 
            float maxSpacing = range / 2.0f;
            float idealSpacing = tickSpacings[tickSpacings.Count - startIndex];

            while (idealSpacing > maxSpacing && startIndex >= 1)
            {
                idealSpacing = tickSpacings[tickSpacings.Count - startIndex];
                startIndex--;
            }

            return idealSpacing;
        }

        private Tick[] GetMajorTicks(float min, float max)
        {
            if (m_tickCollection.Major is null || m_tickCollection.Major.Length == 0)
                return Array.Empty<Tick>();

            List<Tick> ticks = new List<Tick>();

            for (int i = 0; i < m_tickCollection.Major.Length; i++)
            {
                Tick tick = new Tick(
                    position: m_tickCollection.Major[i],
                    label: m_tickCollection.Labels[i],
                    isMajor: true,
                    isDateTime: LabelFormat == TickLabelFormat.DateTime);

                ticks.Add(tick);
            }

            return ticks.Where(x => x.m_position >= min && x.m_position <= max).OrderBy(x => x.m_position).ToArray();
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

        public Tick[] GetTicks(float min, float max)
        {
            return GetMajorTicks(min, max).Concat(GetMinorTicks()).ToArray();
        }

        public Tick[] GetVisibleMajorTicks(PlotDimensions dims)
        {
            float high, low;

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

            return GetMajorTicks(low, high);
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
