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

        public bool IsVertical { get; set; } = true;

        public float TickDensity { get; set; } = 1f;
        public int Radix { get; set; } = 10;
        public CultureInfo Culture { get; set; } = CultureInfo.DefaultThreadCurrentCulture;
        public SizeF LargestLabelSize { get; private set; }
        public float LargestLabelWidth => LargestLabelSize.Width;
        public float LargestLabelHeight => LargestLabelSize.Height;

        public TickLabelFormat LabelFormat { get; set; } = TickLabelFormat.Numeric;

        // TODO: 策略模式？
        public void ReCalculate(PlotDimensions dims, Font tickFont)
        {
            int initialTickCount = (int)(10 * TickDensity);
            if (LabelFormat == TickLabelFormat.DateTime)
            {
                float labelWidth = 20;
                float labelHeight = 24;
                SizeF labelSize = new SizeF(labelWidth, labelHeight);
                ReCalculatePositionsDateTime(dims, labelSize, initialTickCount);
            }
            else if (LabelFormat == TickLabelFormat.Numeric)
            {
                float labelWidth = 15;
                float labelHeight = 12;
                SizeF labelSize = new SizeF(labelWidth, labelHeight);

                ReCalculatePositionsNumeric(dims, labelSize, initialTickCount);
            }

            // use the results of the first pass to estimate the size of the largest tick label
            LargestLabelSize = GetMaxLabelSize(tickFont);

            // re-calculate position by largest label size
            if (LabelFormat == TickLabelFormat.DateTime)
            {
                ReCalculatePositionsDateTime(dims, LargestLabelSize, null);
            }
            else if (LabelFormat == TickLabelFormat.Numeric)
            {
                ReCalculatePositionsNumeric(dims, LargestLabelSize, null);
            }
        }

        private void ReCalculatePositionsDateTime(PlotDimensions dims, SizeF labelSize, int? initialTickCount)
        {
            float low, high;
            int maxTickCount;

            if (IsVertical)
            {
                low = dims.m_yMin - dims.m_unitsPerPxY; // add a extra pixel to capture the edge tick
                high = dims.m_yMax + dims.m_unitsPerPxY;
                maxTickCount = initialTickCount ?? (int)(dims.m_plotHeight / labelSize.Height * TickDensity);
            }
            else
            {
                low = dims.m_xMin - dims.m_unitsPerPxX; // add a extra pixel to capture the edge tick
                high = dims.m_xMax + dims.m_unitsPerPxX;
                maxTickCount = initialTickCount ?? (int)(dims.m_plotWidth / labelSize.Width * TickDensity);
            }

            if (low > high) return;

            low = (float)Math.Max(low, DateTime.MinValue.ToOADate());
            high = (float)Math.Min(high, DateTime.MaxValue.ToOADate());

            DateTime from = DateTime.FromOADate(low);
            DateTime to = DateTime.FromOADate(high);

            float range = high - low; // range in days

            // Calculate number of days and fractional part (time)
            int days = (int)(range); // integer part (full days)
            float fractional = range - days; // fractional part (time portion)

            // Variables to store hours, minutes, and seconds when needed
            int hours = 0;
            int minutes = 0;
            int seconds = 0;

            // Determine intervals based on range size
            int[] incs = Array.Empty<int>();
            List<DateTime> ticks = new List<DateTime>();
            //DateTime[] ticks = Array.Empty<DateTime>(); // Array.Empty<T>(); 一个预先创建好的空数组
            // 根据范围大小来选择增量
            if (range < 1) // 小于一天的范围
            {
                if (fractional < 0.1) // 小于 10% 一天，通常是 2-3 小时
                {
                    incs = new int[] { 1, 2, 3 }; // 按小时增量
                }
                else if (fractional < 0.5) // 小于 50% 一天，通常是 12 小时
                {
                    incs = new int[] { 1, 2, 4 }; // 按小时增量
                }
                else
                {
                    incs = new int[] { 6, 12, 24 }; // 增加增量（如每 6 小时，12 小时，或每 24 小时）
                }
            }
            else if (range > 1 && range < 7)
            {
                incs = new int[] { 1, 8, 16, 24 }; // 增加增量（如每 6 小时，12 小时，或每 24 小时）
            }
            else if (range < 7) // 小于一周的范围
            {
                incs = new int[] { 1, 2, 3 }; // 按天增量
            }
            else if (range < 30) // 小于一个月的范围
            {
                incs = new int[] { 1, 2, 5, 7 }; // 按天增量或每周增量
            }
            else if (range < 365) // 小于一年的范围
            {
                incs = new int[] { 1, 2, 3, 6 }; // 按月增量
            }
            else // 大于一年的范围
            {
                incs = new int[] { 1, 2, 5 }; // 按年增量
            }

            // 如果没有合适的增量，返回
            if (incs.Length == 0) return;

            // 根据选择的增量来生成刻度
            foreach (int inc in incs)
            {
                DateTime current = from;
                ticks.Clear();
                while (current <= to)
                {
                    ticks.Add(current);

                    // 按照选择的增量添加时间
                    if (range < 1) // 小于一天的范围，使用小时或更小的增量
                    {
                        current = current.AddHours(inc); // 按小时增量
                    }
                    else if (range > 1 && range < 7) // 小于一天的范围，使用小时或更小的增量
                    {
                        current = current.AddHours(inc); // 按小时增量
                    }
                    else if (range < 7) // 小于一周，使用天级增量
                    {
                        current = current.AddDays(inc); // 按天增量
                    }
                    else if (range < 30) // 小于一个月，使用天级增量或周增量
                    {
                        current = current.AddDays(inc); // 按天增量
                    }
                    else if (range < 365) // 小于一年，使用月级增量
                    {
                        current = current.AddMonths(inc); // 按月增量
                    }
                    else // 超过一年，使用年级增量
                    {
                        current = current.AddYears(inc); // 按年增量
                    }
                }

                // 如果生成的刻度数目小于或等于 maxTickCount，跳出循环
                if (ticks.Count <= maxTickCount)
                    break;
            }

            string[] labels = ticks
                                .Select(t => $"{t.ToString("d", Culture)} {t.ToString("T", Culture)}")
                                .Select(x => x.Trim())
                                .ToArray();
            float[] tickPositionsMajor = ticks.Select(x => (float)x.ToOADate()).ToArray();
            m_tickCollection = new TickCollection(tickPositionsMajor, null, labels);
        }

        private void ReCalculatePositionsNumeric(PlotDimensions dims, SizeF labelSize, int? initialTickCount)
        {
            float low, high, tickSpacing;
            int maxTickCount;

            if (IsVertical)
            {
                low = dims.m_yMin - dims.m_unitsPerPxY; // add a extra pixel to capture the edge tick
                high = dims.m_yMax + dims.m_unitsPerPxY;
                maxTickCount = initialTickCount ?? (int)(dims.m_plotHeight / labelSize.Height * TickDensity);
                tickSpacing = (float)GetIdealTickSpacing(low, high, maxTickCount, Radix);
            }
            else
            {
                low = dims.m_xMin - dims.m_unitsPerPxX; // add a extra pixel to capture the edge tick
                high = dims.m_xMax + dims.m_unitsPerPxX;
                maxTickCount = initialTickCount ?? (int)(dims.m_plotWidth / labelSize.Width * TickDensity);
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
            return Math.Round(value, 3).ToString("G", Culture);
        }

        private float GetIdealTickSpacing(float low, float high, int maxTickCount, int radix = 10)
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
                tickSpacings.Add(tickSpacings.Last() / divBy[divisions++ % divBy.Length]);
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


        private SizeF GetMaxLabelSize(Font tickFont)
        {
            if (m_tickCollection.Labels is null || m_tickCollection.Labels.Length == 0)
                return new SizeF(0, 0);

            string largestString = "00";
            foreach (string s in m_tickCollection.Labels.Where(x => string.IsNullOrEmpty(x) == false))
                if (s.Length > largestString.Length)
                    largestString = s;

            if (LabelFormat == TickLabelFormat.DateTime)
            {
                // widen largest string based on the longest month name
                foreach (string s in new DateTimeFormatInfo().MonthGenitiveNames)
                {
                    string s2 = s + " " + "1985";
                    if (s2.Length > largestString.Length)
                        largestString = s2;
                }
            }

            return GDI.MeasureStringUsingTemporaryGraphics(largestString, tickFont);
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

            return GetMinorTicks()
                .Where(t => t.m_position >= low && t.m_position <= high)
                .ToArray();
        }
    }
}
