using Plot.Core.Renderables.Axes;

namespace Plot.Core.Series.AxesMangers
{
    public interface IAxisLimitsManager
    {
        AxisLimits GetAxisLimits(AxisLimits viewLimits, AxisLimits dataLimits);
    }
}
