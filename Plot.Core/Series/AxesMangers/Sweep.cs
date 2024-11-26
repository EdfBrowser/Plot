using Plot.Core.Renderables.Axes;

namespace Plot.Core.Series.AxesMangers
{
    public class Sweep : IAxisLimitsManager
    {
        public double ExpansionRatio { get; set; } = 0.5;

        public AxisLimits GetAxisLimits(AxisLimits viewLimits, AxisLimits dataLimits)
        {
            double xMin = dataLimits.m_xMin;
            double xMax = dataLimits.m_xMax;

            bool yOverflow = (dataLimits.m_yMin < viewLimits.m_yMin || dataLimits.m_yMax > viewLimits.m_yMax);
            double ySpanHalf = (dataLimits.m_ySpan / 2) * ExpansionRatio;
            double yMin = yOverflow ? dataLimits.m_yCenter - ySpanHalf : viewLimits.m_yMin;
            double yMax = yOverflow ? dataLimits.m_yCenter + ySpanHalf : viewLimits.m_yMax;

            return new AxisLimits(xMin, xMax, yMin, yMax);
        }
    }
}
