using Plot.Skia.Enums;
using Plot.Skia.Structs;
using System.Drawing;

namespace Plot.Skia.Axes
{
    internal class NumericAutoGenerator : ITickGenerator
    {
        public Tick[] Tick { get; set; }
        public double TickDensity { get; set; } = 1.0;

        public SizeF LargestLabelSize { get; set; }

        public void GenerateTick(Edge edge, double range, float size)
        {
            {
                int initialTickCount = (int)(10 * TickDensity);

                float labelWidth = 15f;
                float labelHeight = 12f;
                SizeF labelSize = new SizeF(labelWidth, labelHeight);
                RecalculatePositionsNumeric(edge, range, size, labelSize,
                    initialTickCount);

                // use the results of the first pass to estimate the size of the largest tick label
                LargestLabelSize = GetMaxLabelSize(tickFont);
            }

            {
                RecalculatePositionsNumeric(edge, range, size, LargestLabelSize, null);

                // use the results of the first pass to estimate the size of the largest tick label
                LargestLabelSize = GetMaxLabelSize(tickFont);
            }
        }

        private void RecalculatePositionsNumeric(Edge edge, double range, float size,
            SizeF labelSize, int? initialTickCount)
        {
            double low, high, tickSpacing;
            int maxTickCount;

            if (edge.Vertical())
            {
                maxTickCount = initialTickCount ?? (int)(size / labelSize.Height * TickDensity);
                tickSpacing = GetIdealTickSpacing(range, maxTickCount, 10);
            }
            else
            {
                low = dims.m_xMin - dims.m_unitsPerPxX; // add a extra pixel to capture the edge tick
                      high = dims.m_xMax + dims.m_unitsPerPxX;
                maxTickCount = initialTickCount ?? (int)(dims.m_plotWidth / labelSize.Width * TickDensity);
                tickSpacing = GetIdealTickSpacing(range, maxTickCount, 10);
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

            if (LabelFormat == TickLabelFormat.Numeric)
            {
                double[] tickPositionsMinor = GetMinorPositions(tickPositionsMajor, low, high);
                m_tickCollection = new TickCollection(tickPositionsMajor, tickPositionsMinor, labels);
            }
            else
            {
                m_tickCollection = new TickCollection(tickPositionsMajor, null, labels);
            }
        }
    }
}
