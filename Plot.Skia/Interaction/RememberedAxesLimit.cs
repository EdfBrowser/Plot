using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    internal class RememberedAxesLimit
    {
        private readonly Dictionary<IAxis, Range> m_axesRange;

        internal RememberedAxesLimit(Figure figure)
        {
            m_axesRange = figure.AxisManager.Axes.ToDictionary(
                x => x, x => x.RangeMutable.ToRange);
        }

        internal void Recall()
        {
            foreach (var axisRange in m_axesRange)
                axisRange.Key.RangeMutable.Set(
                    axisRange.Value.Low, axisRange.Value.High);
        }
    }
}
