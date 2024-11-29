using Plot.Core.Renderables.Axes;

namespace Plot.Core.Series.AxisLimitsManger
{
    public interface IAxisLimitsManager
    {
        AxisLimits GetAxisLimits(AxisLimits viewLimits, AxisLimits dataLimits);
    }
}
