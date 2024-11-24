using Plot.Core.Renderables.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plot.Core.Series.AxesMangers
{
    public class Full : IAxisManager
    {
        public double ExpansionRatio { get; set; } = 1.25;

        public AxisLimits GetAxisLimits(AxisLimits viewLimit, AxisLimits dataLimit)
        {
            bool xOverflow = viewLimit.m_xMin > dataLimit.m_xMin || viewLimit.m_xMax < dataLimit.m_xMax;
            double xMin = xOverflow ? dataLimit.m_xMin : viewLimit.m_xMin;
            double xMax = xOverflow ? dataLimit.m_xMax * ExpansionRatio : viewLimit.m_xMax;

            bool yOverflow = viewLimit.m_yMin > dataLimit.m_yMin || viewLimit.m_yMax < dataLimit.m_yMax;
            double ySpanHalf = (dataLimit.m_ySpan / 2) * ExpansionRatio;
            double yMin = yOverflow ? dataLimit.m_yCenter - ySpanHalf : viewLimit.m_yMin;
            double yMax = yOverflow ? dataLimit.m_yCenter + ySpanHalf : viewLimit.m_yMax;


            return new AxisLimits(xMin, xMax, yMin, yMax);
        }
    }
}
