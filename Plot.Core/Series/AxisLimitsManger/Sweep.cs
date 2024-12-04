using Plot.Core.Renderables.Axes;

namespace Plot.Core.Series.AxisLimitsManger
{
    public class Sweep : IAxisLimitsManager
    {
        public double ExpansionRatio { get; set; } = 0.005;

        public AxisLimits GetAxisLimits(AxisLimits viewLimits, AxisLimits dataLimits)
        {
            double xMin = dataLimits.m_xMin;
            double xMax = dataLimits.m_xMax;

            bool yOverflow = (dataLimits.m_yMin < viewLimits.m_yMin || dataLimits.m_yMax > viewLimits.m_yMax);
            double ySpanHalf = (dataLimits.m_ySpan / 2) * ExpansionRatio;
            double yMin = yOverflow ? ySpanHalf - dataLimits.m_yCenter : viewLimits.m_yMin;
            double yMax = yOverflow ? ySpanHalf + dataLimits.m_yCenter : viewLimits.m_yMax;

            return new AxisLimits(xMin, xMax, yMin, yMax);
        }
    }
}
